namespace Sso
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Xml;

    using ComponentSpace.SAML2.Assertions;
    using ComponentSpace.SAML2.Exceptions;
    using ComponentSpace.SAML2.Protocols;

    using SpendManagementLibrary;

    using Sso.AttribStatement;

    using Attribute = ComponentSpace.SAML2.Assertions.AttributeStatement;

    /// <summary>
    ///     The SAML class.
    /// </summary>
    public class Saml
    {
        #region Fields

        /// <summary>
        /// debug mode flag.
        /// </summary>
        private const bool DebugMode = true;

        /// <summary>
        /// SEL's private certificate.
        /// </summary>
        private readonly X509Certificate2 _selCertificate;

        /// <summary>
        ///     The xml doc.
        /// </summary>
        private XmlDocument xmlDoc;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Initialises a new instance of the <see cref="Saml"/> class.
        /// </summary>
        /// <param name="selCertificate">
        /// SEL's private certificate.
        /// </param>
        public Saml(X509Certificate2 selCertificate)
        {
            this._selCertificate = selCertificate;
        }

        /// <summary>
        /// The deserialize SAML.
        /// </summary>
        /// <param name="hostname">
        /// The hostname of this request
        /// </param>
        /// <param name="samlRequest">
        /// The SAML request.
        /// </param>
        /// <param name="sso">
        /// The Single sign-on configuration
        /// </param>
        /// <returns>
        /// The <see cref="SamlResponse"/>.
        /// </returns>
        public SamlResponse[] DeserializeSaml(string hostname, string samlRequest)
        {
            Saml.DebugMessage(string.Format("DeserialiseSaml - package = {0}", samlRequest));
            byte[] bytes = Convert.FromBase64String(samlRequest);

            if (this._selCertificate == null)
            {
                var selCertIsNull = new SamlResponse { Valid = false, Message = string.Format("SEL certificate is null") };
                Saml.DebugMessage(selCertIsNull.Message);
                return new[] { selCertIsNull };
            }

            this.xmlDoc = new XmlDocument();
            try
            {
                this.xmlDoc.LoadXml(Encoding.Default.GetString(bytes));
                Saml.DebugMessage(string.Format("Loaded XML - {0}", this.xmlDoc.InnerXml));
                
                var samlResponse = new SAMLResponse(this.xmlDoc.DocumentElement);

                if (!samlResponse.IsSuccess())
                {
                    var samlResponseFailed = new SamlResponse { Valid = false, Message = samlResponse.Status.StatusCode.Code };
                    Saml.DebugMessage(samlResponseFailed.Message);
                    return new[] { samlResponseFailed };
                }

                // Load up the SSO configuration.
                Saml.DebugMessage(String.Format("SingleSignOn.GetByIssuerUri('{0}')", samlResponse.Issuer.NameIdentifier));
                var ssoAccounts = SingleSignOn.GetByIssuer(hostname, samlResponse.Issuer.NameIdentifier);

                if (!ssoAccounts.Any())
                {
                    var issuerNotFound = new SamlResponse { Valid = false, Message = string.Format("The specified Issuer is not configured in SEL") };
                    Saml.DebugMessage(issuerNotFound.Message);
                    return new[] { issuerNotFound };
                }

                Saml.DebugMessage(String.Format("Found {0} Single Sign-on configuration{1}", ssoAccounts.Length, ssoAccounts.Length == 1 ? string.Empty : "s"));
                var responses = new List<SamlResponse>();
                foreach (var ssoAccount in ssoAccounts)
                {
                    var sso = ssoAccount.Item2;

                    Saml.DebugMessage(String.Format("SSO configuration - \r\n" +
                                                    "AccountId:\t\t{0}\r\n" +
                                                    "Issuer:\t\t\t{1}\r\n" +
                                                    "Company ID attribute:\t{2}\r\n" +
                                                    "Identifier attribute:\t{3}\r\n" +
                                                    "Identifier lookup field:\t{4}\r\n" +
                                                    "Login error URL:\t\t{5}\r\n" +
                                                    "Timeout URL:\t\t{6}\r\n" +
                                                    "Exit URL:\t\t\t{7}\r\n" +
                                                    "Identity provider public certificate:\r\n{8}",
                        ssoAccount.Item1,
                        sso.IssuerUri,
                        sso.CompanyIdAttribute,
                        sso.IdAttribute,
                        sso.IdLookupFieldId,
                        sso.LoginErrorUrl,
                        sso.TimeoutUrl,
                        sso.ExitUrl,
                        sso.PublicCertificateAsString));

                    // Load up the certificate.
                    Saml.DebugMessage("Loading certificate from SSO config");
                    var x509Certificate = new X509Certificate2();
                    x509Certificate.Import(sso.PublicCertificate);
                    Saml.DebugMessage("Certificate Loaded:\r\n" + x509Certificate);

                    if (!this.IsSignatureValid(samlResponse, x509Certificate))
                    {
                        var signatureInvalid = new SamlResponse { Valid = false, Message = "Signature invalid." };
                        Saml.DebugMessage(signatureInvalid.Message);
                        responses.Add(signatureInvalid);
                        continue;
                    }

                    var validResponse = new SamlResponse { Valid = true, AccountId = ssoAccount.Item1, SsoConfig = sso };
                    foreach (EncryptedAssertion encryptedAssertion in samlResponse.GetEncryptedAssertions())
                    {
                        var assert = encryptedAssertion.Decrypt(this._selCertificate.PrivateKey, null);

                        foreach (Attribute statement in assert.GetAttributeStatements())
                        {
                            validResponse.AttributeStatement.Attributes.AddRange(GetAttributes(statement));
                        }
                    }

                    foreach (SAMLAssertion assertion in samlResponse.GetAssertions())
                    {
                        foreach (Attribute statement in assertion.GetAttributeStatements())
                        {
                            validResponse.AttributeStatement.Attributes.AddRange(GetAttributes(statement));
                        }
                    }

                    foreach (XmlElement xmlAssertion in samlResponse.GetSignedAssertions())
                    {
                        var assertion = new SAMLAssertion(xmlAssertion);
                        foreach (Attribute statement in assertion.GetAttributeStatements())
                        {
                            validResponse.AttributeStatement.Attributes.AddRange(GetAttributes(statement));
                        }
                    }

                    Saml.DebugMessage("Attributes from SAML assertion:\r\n" +
                                      string.Join("\r\n", validResponse.AttributeStatement.Attributes.Select(attribute =>
                                          string.Format("{0}={1}", attribute.Name, string.Join(" | ", attribute.Values)))));

                    responses.Add(validResponse);
                }

                return responses.ToArray();
            }
            catch (Exception e)
            {
                var exceptionResponse = new SamlResponse { Valid = false, Message = e.Message };
                Saml.DebugMessage(exceptionResponse.Message);
                return new[] { exceptionResponse };
            }
        }

        private bool IsSignatureValid(SAMLResponse xmlSAMLResponse, X509Certificate2 certificate)
        {
            try
            {
                if (certificate != null)
                {
                    return SAMLMessageSignature.Verify(xmlSAMLResponse.ToXml(), certificate);
                }

                return SAMLMessageSignature.Verify(xmlSAMLResponse.ToXml());

            }
            catch (SAMLSignatureException)
            {
                try
                {
                    return xmlSAMLResponse.GetSignedAssertions().Any(sA => SAMLAssertionSignature.Verify(sA, certificate));
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The get attributes.
        /// </summary>
        /// <param name="attributeStatement">
        /// The attribute statement.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<SsoAttribute> GetAttributes(ComponentSpace.SAML2.Assertions.AttributeStatement attributeStatement)
        {
            var result = new List<SsoAttribute>();
            foreach (SAMLAttribute attribute in attributeStatement.GetUnencryptedAttributes())
            {
                if (attribute.Values != null && attribute.Values.Count > 0)
                {
                    var attrib = new SsoAttribute(attribute.Name.Split('/').Last(), attribute.Values[0].ToString());
                    result.Add(attrib);
                }
            }

            foreach (EncryptedAttribute attribute in attributeStatement.GetEncryptedAttributes())
            {
                var decryptedAttribute = attribute.Decrypt(this._selCertificate.PrivateKey, null, null);
                if (decryptedAttribute.Values != null && decryptedAttribute.Values.Count > 0)
                {
                    var attrib = new SsoAttribute(decryptedAttribute.Name.Split('/').Last(), decryptedAttribute.Values[0].ToString());
                    result.Add(attrib);
                }
            }
            

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Write debug message if debug mode = true.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void DebugMessage(string message)
        {
            if (DebugMode)
            {
                cEventlog.LogEntry(message);
            }
        }

        #endregion
    }
}