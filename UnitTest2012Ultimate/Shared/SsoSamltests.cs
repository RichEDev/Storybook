using System.Security.Cryptography.X509Certificates;
using SpendManagementLibrary;

namespace UnitTest2012Ultimate.Shared
{
    using System;
    using System.Text;
    using System.Xml;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Sso;

    /// <summary>
    ///     The SSO SAML tests.
    /// </summary>
    [TestClass]
    public class SsoSamltests
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        private X509Certificate2 _selCertificate;
        private const string HostName = "www.sel-expenses.com";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The my class clean up.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        ///     The my test initialize.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            var crypto = new cSecureData();
            var password = crypto.Decrypt(GlobalVariables.GetAppSetting("SelPrivateCertificatePassword"));
            this._selCertificate = new X509Certificate2(GlobalVariables.StaticContentFolderPath + GlobalVariables.GetAppSetting("SelPrivateCertificateSCPath"), password, X509KeyStorageFlags.MachineKeySet);
        }

        /// <summary>
        ///     The SAML broken signature assertion.
        ///     Change the xml after signature, this should return valid equals false.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon")]
        [TestCategory("SSO")]
        [TestCategory("SAML")]
        public void SamlBrokenSignatureAssertion()
        {
            var saml = new Saml(this._selCertificate);
            var result = saml.DeserializeSaml(HostName, this.GetAssertion("broken"));
            Assert.IsNotNull(result);
            Assert.IsFalse(result[0].Valid, string.Format("Message = {0}", result[0].Message));
        }

        /// <summary>
        ///     The SAML corrupt assertion.
        ///     Insert a random string in place of Base64 Encoded string.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon")]
        [TestCategory("SSO")]
        [TestCategory("SAML")]
        public void SamlCorruptAssertion()
        {
            var saml = new Saml(this._selCertificate);
            var result = saml.DeserializeSaml(HostName, "abcd");
            Assert.IsNotNull(result);
            Assert.IsFalse(result[0].Valid, string.Format("Message = {0}", result[0].Message));
        }

        /// <summary>
        ///     The SAML invalid certificate.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon")]
        [TestCategory("SSO")]
        [TestCategory("SAML")]
        public void SamlInvalidCertificate()
        {
            var saml = new Saml(new X509Certificate2());
            var result = saml.DeserializeSaml(HostName, this.GetAssertion());
            Assert.IsNotNull(result);
            Assert.IsFalse(result[0].Valid, string.Format("Message = {0}", result[0].Message));
            Assert.IsTrue(result[0].Message != null);
        }

        /// <summary>
        ///     The SAML assertion.
        ///     A valid signed assertion.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon")]
        [TestCategory("SSO")]
        [TestCategory("SAML")]
        public void SamlValidAssertion() // This Unit test will fail until User Story 70005 is completed
        {
            var saml = new Saml(this._selCertificate);
            var result = saml.DeserializeSaml(HostName, this.GetAssertion());
            Assert.IsNotNull(result);
            Assert.IsTrue(result[0].Valid || (!result[0].Valid && result[0].Message == "SAML Assertion not within time period"), string.Format("Message = {0}", result[0].Message));
        }

        /// <summary>
        /// The SAML version 1 assertion.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon")]
        [TestCategory("SSO")]
        [TestCategory("SAML")]
        public void SamlVersion1Assertion()
        {
            var saml = new Saml(this._selCertificate);
            var result = saml.DeserializeSaml(HostName, this.GetVersion1Assertion());
            Assert.IsNotNull(result);
            Assert.IsFalse(result[0].Valid, string.Format("Message = {0}", result[0].Message));
        }

        /// <summary>
        /// The SAML KPMG assertion.
        /// </summary>
        [TestMethod]
        [TestCategory("Logon")]
        [TestCategory("SSO")]
        [TestCategory("SAML")]
        public void SamlVersionAssertionKpmg()
        {
            var saml = new Saml(this._selCertificate);
            var result = saml.DeserializeSaml(HostName, this.GetAssertionKpmg());
            Assert.IsNotNull(result);
            Assert.IsTrue(result[0].Valid || (!result[0].Valid && result[0].Message == "SAML Assertion not within time period"), string.Format("Message = {0}", result[0].Message));
        }

        /// <summary>
        ///     The test clean up.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get assertion.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetAssertion(string subject = "Logon")
        {
            string assertion =
                string.Format(
                    "<Response xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ID=\"_a08ccb2e-60cf-4f47-b121-79ac79ac6c26\" Version=\"2.0\" IssueInstant=\"2013-05-28T14:10:26.3860612Z\" Destination=\"http://localhost/wolverine/expenses/shared/sso.aspx\" xmlns=\"urn:oasis:names:tc:SAML:2.0:protocol\"><Issuer xmlns=\"urn:oasis:names:tc:SAML:2.0:assertion\">sel.expenses.com</Issuer><Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><SignedInfo><CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /><SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" /><Reference URI=\"#_a08ccb2e-60cf-4f47-b121-79ac79ac6c26\"><Transforms><Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\" /><Transform Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /></Transforms><DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" /><DigestValue>Nht0Zic/KVcksQZ0rD8q3RynD1o=</DigestValue></Reference></SignedInfo><SignatureValue>OdzIXU/kCN772aPno2VknxViBqiVGO8/oS20iJBfUKGl+yN7JdnolhYV18YomyLF6o8zGE4Cg+nzgva7ss1KIu7OBhYirQsBq9qxCYdpC7yipe3000cl2lCn8ErjsFfrQAAX0cD0q92thX1Dm3p2dx26VS8w2K5qySlfuWgoiHXiKGso48hSGOe0Tp6tTXflzYw1M9Dj0+/yGMFwLzpUOaKoTMEkJMcXx3P6B9fap3oopINhNHaHHD5Xl+FuSn8NBN0AKPkV1EL2/I4qjWhGFmc1onNADyVLEp1Snbf1c6PS9u+J+Ora7DYlWW4af7EdRm10aTC/IpAzHD2QKHEQCA==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIFUDCCBDigAwIBAgISESEdAT6Ac6RN2lignaP3ktLqMA0GCSqGSIb3DQEBBQUAMF0xCzAJBgNVBAYTAkJFMRkwFwYDVQQKExBHbG9iYWxTaWduIG52LXNhMTMwMQYDVQQDEypHbG9iYWxTaWduIE9yZ2FuaXphdGlvbiBWYWxpZGF0aW9uIENBIC0gRzIwHhcNMTIxMjA0MTcyNjI5WhcNMTcwMzIwMjM1OTU4WjCBhzELMAkGA1UEBhMCR0IxFTATBgNVBAgMDExpbmNvbG5zaGlyZTEQMA4GA1UEBwwHTGluY29sbjESMBAGA1UECwwJVGVjaG5pY2FsMR4wHAYDVQQKDBVTb2Z0d2FyZSAoRXVyb3BlKSBMdGQxGzAZBgNVBAMMEiouc2VsLWV4cGVuc2VzLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAJs1OE9xA10NuWx+RK45ip06GmU4SFcTnXdniDk9nQygG0ofZSRt9AeJKolx8YMY1EMQMDy2/vLr337MrFiQUNx3JkB6RDN8ZRThob4CT6/iFNAHr3g7MLUHZxnfV9msNPrKQydWndRTG8z/AElx2IbJIehF/Hz10pcWHTXb9TVvPwbAkWlF6L1We5IiIthdKLVSJzbm3J02jr4VMbg4Api+yD5IpFFu3tmq44n9awYl4aD5G71yw+Gx30y6CGry8FberV8Vb2u1TNP3Ds0GiWikUh8ohUrj3jEZ2+v8vOHsPZrk7JdHDa6l3GgREcK6OSYVJ/aVAbZZJmwvkf7+WKsCAwEAAaOCAd0wggHZMA4GA1UdDwEB/wQEAwIFoDBMBgNVHSAERTBDMEEGCSsGAQQBoDIBFDA0MDIGCCsGAQUFBwIBFiZodHRwczovL3d3dy5nbG9iYWxzaWduLmNvbS9yZXBvc2l0b3J5LzAvBgNVHREEKDAmghIqLnNlbC1leHBlbnNlcy5jb22CEHNlbC1leHBlbnNlcy5jb20wCQYDVR0TBAIwADAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwRQYDVR0fBD4wPDA6oDigNoY0aHR0cDovL2NybC5nbG9iYWxzaWduLmNvbS9ncy9nc29yZ2FuaXphdGlvbnZhbGcyLmNybDCBlgYIKwYBBQUHAQEEgYkwgYYwRwYIKwYBBQUHMAKGO2h0dHA6Ly9zZWN1cmUuZ2xvYmFsc2lnbi5jb20vY2FjZXJ0L2dzb3JnYW5pemF0aW9udmFsZzIuY3J0MDsGCCsGAQUFBzABhi9odHRwOi8vb2NzcDIuZ2xvYmFsc2lnbi5jb20vZ3Nvcmdhbml6YXRpb252YWxnMjAdBgNVHQ4EFgQUZJVXf3bjSwMt8OprELV38Eu2dLUwHwYDVR0jBBgwFoAUXUayjcRLdBy77fVztjq3OI91nn4wDQYJKoZIhvcNAQEFBQADggEBAJuEdfOd3NkWvn7qlcqPl/JHWol6oFE9b9Zr9h38TnfWmlx9HxfQGWMa7g90g9WAbhiRMnWYPIpqf9fiw+nvPQYVM8KbU3DQ7wSy47tIi8B55keAuMVh164p5S3yjXozXqrpTjnPzc5EB5hbHhKH6Nn8769fzxjSsiUAD006+IDLRgmECeQkVThi5cRNGfTYTZLVlfkC7C0QbF8OlWQ3IQ1gh8htoYIiWgBPqcAfKM38NR3ercAZTJBXBgjc3egZLw41qlLq+INXeT0UnmpcnoLZu5wLkTljgefDEh5TO6QqqjtrZh/SAbcg+rgZazDHaPdsAZGgrJtljjAvLcsSrsk=</X509Certificate></X509Data></KeyInfo></Signature><Status><StatusCode Value=\"urn:oasis:names:tc:SAML:2.0:status:Success\" /></Status><Assertion Version=\"2.0\" ID=\"_08e2fa27-43fa-45ab-91b4-ffbc368f92ab\" IssueInstant=\"2013-05-28T14:10:26.3860612Z\" xmlns=\"urn:oasis:names:tc:SAML:2.0:assertion\"><Issuer>sel.expenses.com</Issuer><Subject><NameID NameQualifier=\"http://localhost/\">{0}</NameID><SubjectConfirmation Method=\"urn:oasis:names:tc:SAML:2.0:cm:bearer\"><SubjectConfirmationData NotOnOrAfter=\"2013-05-28T14:15:26.3860612Z\" Recipient=\"http://localhost/wolverine/expenses/shared/sso.aspx\" /></SubjectConfirmation></Subject><Conditions NotBefore=\"2013-05-28T14:10:26.3860612Z\" NotOnOrAfter=\"2013-05-28T14:15:26.3860612Z\"><AudienceRestriction><Audience>http://localhost/</Audience></AudienceRestriction></Conditions><AuthnStatement AuthnInstant=\"2013-05-28T14:10:26.3860612Z\"><AuthnContext><AuthnContextClassRef>AuthnContextClassRef</AuthnContextClassRef></AuthnContext></AuthnStatement><AttributeStatement><Attribute Name=\"companyid\" NameFormat=\"urn:oasis:names:tc:SAML:2.0:attrname-format:basic\"><AttributeValue xsi:type=\"xsd:string\">testing</AttributeValue></Attribute><Attribute Name=\"username\" NameFormat=\"urn:oasis:names:tc:SAML:2.0:attrname-format:basic\"><AttributeValue xsi:type=\"xsd:string\">james</AttributeValue></Attribute></AttributeStatement></Assertion></Response>",
                    subject);

            return Convert.ToBase64String(Encoding.Default.GetBytes(assertion));
        }

        /// <summary>
        /// The get version 1 assertion.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetVersion1Assertion()
        {
            var result =
                "<samlp:Response xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\" ResponseID=\"_4935524b-c91c-47a9-82c6-4626239c2af6\" MajorVersion=\"1\" MinorVersion=\"1\" IssueInstant=\"2013-05-30T07:55:44.0106174Z\" Recipient=\"http://localhost/wolverine/expenses/shared/sso.aspx\" xmlns:samlp=\"urn:oasis:names:tc:SAML:1.0:protocol\"><Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><SignedInfo><CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /><SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" /><Reference URI=\"#_4935524b-c91c-47a9-82c6-4626239c2af6\"><Transforms><Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\" /><Transform Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /></Transforms><DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" /><DigestValue>hYhlFh5DedXM3YIf+FTy4u1R/x0=</DigestValue></Reference></SignedInfo><SignatureValue>bvod4ivOINXFLjN0BBOs45JMB2jKOeSAJvyIoWczxBIt+fxcE6GQZwmkRfUsnnSxeExc8Oh+TLnJnXd9pfF8jCACTH33e/QLCTd3qXBHp/FYW/5E+UJ4ZvVBgx7/SCgeaavTIf/VakHfxDCiFpjoc6lNCaq+3f4Qlhr8vBKz1m2S2G8BuN8ofbA7UOb/0ZBWmiZfvCq/jJCG/gIlUKVLrW6ePSG1wSzm1vnMddg3rPtI0ZiM3TVAXjONBE5QSY2RCsGArqDJJJo3r3pDTkn7iEyblyQaPYtK55lGtZwimi1prWB1q06HbtcFFtWpjIGI+Irf9nqUu3+F7Dhb33BM/w==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIFUDCCBDigAwIBAgISESEdAT6Ac6RN2lignaP3ktLqMA0GCSqGSIb3DQEBBQUAMF0xCzAJBgNVBAYTAkJFMRkwFwYDVQQKExBHbG9iYWxTaWduIG52LXNhMTMwMQYDVQQDEypHbG9iYWxTaWduIE9yZ2FuaXphdGlvbiBWYWxpZGF0aW9uIENBIC0gRzIwHhcNMTIxMjA0MTcyNjI5WhcNMTcwMzIwMjM1OTU4WjCBhzELMAkGA1UEBhMCR0IxFTATBgNVBAgMDExpbmNvbG5zaGlyZTEQMA4GA1UEBwwHTGluY29sbjESMBAGA1UECwwJVGVjaG5pY2FsMR4wHAYDVQQKDBVTb2Z0d2FyZSAoRXVyb3BlKSBMdGQxGzAZBgNVBAMMEiouc2VsLWV4cGVuc2VzLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAJs1OE9xA10NuWx+RK45ip06GmU4SFcTnXdniDk9nQygG0ofZSRt9AeJKolx8YMY1EMQMDy2/vLr337MrFiQUNx3JkB6RDN8ZRThob4CT6/iFNAHr3g7MLUHZxnfV9msNPrKQydWndRTG8z/AElx2IbJIehF/Hz10pcWHTXb9TVvPwbAkWlF6L1We5IiIthdKLVSJzbm3J02jr4VMbg4Api+yD5IpFFu3tmq44n9awYl4aD5G71yw+Gx30y6CGry8FberV8Vb2u1TNP3Ds0GiWikUh8ohUrj3jEZ2+v8vOHsPZrk7JdHDa6l3GgREcK6OSYVJ/aVAbZZJmwvkf7+WKsCAwEAAaOCAd0wggHZMA4GA1UdDwEB/wQEAwIFoDBMBgNVHSAERTBDMEEGCSsGAQQBoDIBFDA0MDIGCCsGAQUFBwIBFiZodHRwczovL3d3dy5nbG9iYWxzaWduLmNvbS9yZXBvc2l0b3J5LzAvBgNVHREEKDAmghIqLnNlbC1leHBlbnNlcy5jb22CEHNlbC1leHBlbnNlcy5jb20wCQYDVR0TBAIwADAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwRQYDVR0fBD4wPDA6oDigNoY0aHR0cDovL2NybC5nbG9iYWxzaWduLmNvbS9ncy9nc29yZ2FuaXphdGlvbnZhbGcyLmNybDCBlgYIKwYBBQUHAQEEgYkwgYYwRwYIKwYBBQUHMAKGO2h0dHA6Ly9zZWN1cmUuZ2xvYmFsc2lnbi5jb20vY2FjZXJ0L2dzb3JnYW5pemF0aW9udmFsZzIuY3J0MDsGCCsGAQUFBzABhi9odHRwOi8vb2NzcDIuZ2xvYmFsc2lnbi5jb20vZ3Nvcmdhbml6YXRpb252YWxnMjAdBgNVHQ4EFgQUZJVXf3bjSwMt8OprELV38Eu2dLUwHwYDVR0jBBgwFoAUXUayjcRLdBy77fVztjq3OI91nn4wDQYJKoZIhvcNAQEFBQADggEBAJuEdfOd3NkWvn7qlcqPl/JHWol6oFE9b9Zr9h38TnfWmlx9HxfQGWMa7g90g9WAbhiRMnWYPIpqf9fiw+nvPQYVM8KbU3DQ7wSy47tIi8B55keAuMVh164p5S3yjXozXqrpTjnPzc5EB5hbHhKH6Nn8769fzxjSsiUAD006+IDLRgmECeQkVThi5cRNGfTYTZLVlfkC7C0QbF8OlWQ3IQ1gh8htoYIiWgBPqcAfKM38NR3ercAZTJBXBgjc3egZLw41qlLq+INXeT0UnmpcnoLZu5wLkTljgefDEh5TO6QqqjtrZh/SAbcg+rgZazDHaPdsAZGgrJtljjAvLcsSrsk=</X509Certificate></X509Data></KeyInfo></Signature><samlp:Status><samlp:StatusCode Value=\"samlp:Success\" /></samlp:Status><saml:Assertion MajorVersion=\"1\" MinorVersion=\"1\" AssertionID=\"_751e2e7e-135d-4e6d-b575-bae9022527ac\" Issuer=\"Me\" IssueInstant=\"2013-05-30T07:55:44.0146158Z\"><saml:Conditions NotBefore=\"2013-05-30T07:55:44.0146158Z\" NotOnOrAfter=\"2013-05-30T08:05:44.0146158Z\" /><saml:AuthenticationStatement AuthenticationMethod=\"urn:oasis:names:tc:SAML:1.0:am:password\" AuthenticationInstant=\"2013-05-30T07:55:44.0156154Z\"><saml:Subject><saml:NameIdentifier NameQualifier=\"http://localhost\">onlyme</saml:NameIdentifier><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject><saml:SubjectLocality IPAddress=\"fe80::2c9c:407:540:3cfb%11\" /></saml:AuthenticationStatement><saml:AttributeStatement><saml:Subject><saml:NameIdentifier NameQualifier=\"http://localhost\">onlyme</saml:NameIdentifier><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject><saml:Attribute AttributeName=\"username\" AttributeNamespace=\"http://localhost\"><saml:AttributeValue xmlns:q1=\"http://www.w3.org/2001/XMLSchema\" p7:type=\"q1:string\" xmlns:p7=\"http://www.w3.org/2001/XMLSchema-instance\">james</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"companyid\" AttributeNamespace=\"http://localhost\"><saml:AttributeValue xmlns:q2=\"http://www.w3.org/2001/XMLSchema\" p7:type=\"q2:string\" xmlns:p7=\"http://www.w3.org/2001/XMLSchema-instance\">testing</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"email\" AttributeNamespace=\"http://localhost\"><saml:AttributeValue xmlns:q3=\"http://www.w3.org/2001/XMLSchema\" p7:type=\"q3:string\" xmlns:p7=\"http://www.w3.org/2001/XMLSchema-instance\">dunkiep@hotmail.co.uk</saml:AttributeValue></saml:Attribute></saml:AttributeStatement></saml:Assertion></samlp:Response>";
            return Convert.ToBase64String(Encoding.Default.GetBytes(result));
        }

        private string GetAssertionKpmg()
        {
            var result = "<samlp:Response ID=\"_f7c6ac56-8ede-4086-bf84-d04aee505ca8\" Version=\"2.0\" IssueInstant=\"2013-06-04T09:18:21Z\" Destination=\"https://chronos6.sel-expenses.com/shared/sso.aspx\" xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\"><saml:Issuer xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\">urn:KPMGE_SSO</saml:Issuer><Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><SignedInfo><CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\" /><SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" /><Reference URI=\"#_f7c6ac56-8ede-4086-bf84-d04aee505ca8\"><Transforms><Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\" /><Transform Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"><InclusiveNamespaces PrefixList=\"#default samlp saml ds xs xsi\" xmlns=\"http://www.w3.org/2001/10/xml-exc-c14n#\" /></Transform></Transforms><DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" /><DigestValue>IfIR48pBehCiH7hDhFxxm0ibQBk=</DigestValue></Reference></SignedInfo><SignatureValue>ezrIbQ530j7mUzaXesak319MKeNXpja+Rkeo6OK0nlhKeiHHaODSqPa+p9Ujw2Q4QSSq4OMj/T1uG3dbSD9LwuANisRPRIbghHIAvfg//wfzoQSOEGLwxaRSL63OyvHbERopCieJH+hgQF5hWpKgPnKYgiQjvs0Ny/Yr22SG/jw=</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIFIzCCBAugAwIBAgIRAILDN6EEyhbIIu7nP+AFaWMwDQYJKoZIhvcNAQEFBQAwgYkxCzAJBgNVBAYTAkdCMRswGQYDVQQIExJHcmVhdGVyIE1hbmNoZXN0ZXIxEDAOBgNVBAcTB1NhbGZvcmQxGjAYBgNVBAoTEUNPTU9ETyBDQSBMaW1pdGVkMS8wLQYDVQQDEyZDT01PRE8gSGlnaCBBc3N1cmFuY2UgU2VjdXJlIFNlcnZlciBDQTAeFw0wOTEyMTAwMDAwMDBaFw0xMTEyMTAyMzU5NTlaMIGhMQswCQYDVQQGEwJHQjERMA8GA1UEERMIRUM0WSA4QkIxDzANBgNVBAgTBkxvbmRvbjEPMA0GA1UEBxMGTG9uZG9uMRswGQYDVQQJExI4IFNhbGlzYnVyeSBTcXVhcmUxDTALBgNVBAoTBEtQTUcxETAPBgNVBAsTCEVsaXRlU1NMMR4wHAYDVQQDExVwb3J0YWwucmV3YXJkd2lzZS5jb20wgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBALsginSJzM1stLBuYn+jrHeqerdUjWrhnikbkpitrxgDl5rlCcmGorm3wohIiAAQ0vzdtnXaX8uNSnPctvk2S0A7a080HNtpkMhnnEMlnVlTOuHvNlDbF5mpgKIgjXcdafgH3DGfFNRT6VcCK/Ru/XlJLNyUB0znijg7KyRazHO/AgMBAAGjggHuMIIB6jAfBgNVHSMEGDAWgBRgWc2Ax8Xjq4wv/GvlWwr1D95L/zAdBgNVHQ4EFgQU1xypYrdE0cUeQc1IygDvc+asMvUwDgYDVR0PAQH/BAQDAgWgMAwGA1UdEwEB/wQCMAAwNAYDVR0lBC0wKwYIKwYBBQUHAwEGCCsGAQUFBwMCBgorBgEEAYI3CgMDBglghkgBhvhCBAEwRgYDVR0gBD8wPTA7BgwrBgEEAbIxAQIBAwQwKzApBggrBgEFBQcCARYdaHR0cHM6Ly9zZWN1cmUuY29tb2RvLm5ldC9DUFMwTgYDVR0fBEcwRTBDoEGgP4Y9aHR0cDovL2NybC5jb21vZG9jYS5jb20vQ29tb2RvSGlnaEFzc3VyYW5jZVNlY3VyZVNlcnZlckNBLmNybDB/BggrBgEFBQcBAQRzMHEwSQYIKwYBBQUHMAKGPWh0dHA6Ly9jcnQuY29tb2RvY2EuY29tL0NvbW9kb0hpZ2hBc3N1cmFuY2VTZWN1cmVTZXJ2ZXJDQS5jcnQwJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmNvbW9kb2NhLmNvbTA7BgNVHREENDAyghVwb3J0YWwucmV3YXJkd2lzZS5jb22CGXd3dy5wb3J0YWwucmV3YXJkd2lzZS5jb20wDQYJKoZIhvcNAQEFBQADggEBAEFp2OQeWyjTsDcXFNAbeNwmikeSMNGj5nzQ6VL7oWPnh0h9qI1lO998CgvjjPtUVhRFBB12egFFWpE00eMB1F3hocxo16yi9boO7zHIYGWhs3ZQ4HFU6ERSlOi5Mydopvyii0S4vPNUPtrydnv3CpEZMRopa0lyzgZyr2Uded7bR5hPE8DPxyOPAW5HJU2sZwKAJy4pw9BRpNhUvWXG9CCZ3CQVqSSyL6Hh1DgKVi/ZqWczyq5r3ad43Kq/6IPss12rmjiWo24KAbgzDljAAthYr9C+dET70Dx3P4gf3py171Yoth9rQ7W3melkXFU3qTh6ifXbhUmCgVUdrTg7eGk=</X509Certificate></X509Data></KeyInfo></Signature><samlp:Status><samlp:StatusCode Value=\"urn:oasis:names:tc:SAML:2.0:status:Success\" /></samlp:Status><saml:EncryptedAssertion xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\"><EncryptedData Type=\"http://www.w3.org/2001/04/xmlenc#Element\" xmlns=\"http://www.w3.org/2001/04/xmlenc#\"><EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#aes256-cbc\" /><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><EncryptedKey xmlns=\"http://www.w3.org/2001/04/xmlenc#\"><EncryptionMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#rsa-1_5\" /><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><X509Data><X509Certificate>MIIFUDCCBDigAwIBAgISESEdAT6Ac6RN2lignaP3ktLqMA0GCSqGSIb3DQEBBQUAMF0xCzAJBgNVBAYTAkJFMRkwFwYDVQQKExBHbG9iYWxTaWduIG52LXNhMTMwMQYDVQQDEypHbG9iYWxTaWduIE9yZ2FuaXphdGlvbiBWYWxpZGF0aW9uIENBIC0gRzIwHhcNMTIxMjA0MTcyNjI5WhcNMTcwMzIwMjM1OTU4WjCBhzELMAkGA1UEBhMCR0IxFTATBgNVBAgMDExpbmNvbG5zaGlyZTEQMA4GA1UEBwwHTGluY29sbjESMBAGA1UECwwJVGVjaG5pY2FsMR4wHAYDVQQKDBVTb2Z0d2FyZSAoRXVyb3BlKSBMdGQxGzAZBgNVBAMMEiouc2VsLWV4cGVuc2VzLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAJs1OE9xA10NuWx+RK45ip06GmU4SFcTnXdniDk9nQygG0ofZSRt9AeJKolx8YMY1EMQMDy2/vLr337MrFiQUNx3JkB6RDN8ZRThob4CT6/iFNAHr3g7MLUHZxnfV9msNPrKQydWndRTG8z/AElx2IbJIehF/Hz10pcWHTXb9TVvPwbAkWlF6L1We5IiIthdKLVSJzbm3J02jr4VMbg4Api+yD5IpFFu3tmq44n9awYl4aD5G71yw+Gx30y6CGry8FberV8Vb2u1TNP3Ds0GiWikUh8ohUrj3jEZ2+v8vOHsPZrk7JdHDa6l3GgREcK6OSYVJ/aVAbZZJmwvkf7+WKsCAwEAAaOCAd0wggHZMA4GA1UdDwEB/wQEAwIFoDBMBgNVHSAERTBDMEEGCSsGAQQBoDIBFDA0MDIGCCsGAQUFBwIBFiZodHRwczovL3d3dy5nbG9iYWxzaWduLmNvbS9yZXBvc2l0b3J5LzAvBgNVHREEKDAmghIqLnNlbC1leHBlbnNlcy5jb22CEHNlbC1leHBlbnNlcy5jb20wCQYDVR0TBAIwADAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwRQYDVR0fBD4wPDA6oDigNoY0aHR0cDovL2NybC5nbG9iYWxzaWduLmNvbS9ncy9nc29yZ2FuaXphdGlvbnZhbGcyLmNybDCBlgYIKwYBBQUHAQEEgYkwgYYwRwYIKwYBBQUHMAKGO2h0dHA6Ly9zZWN1cmUuZ2xvYmFsc2lnbi5jb20vY2FjZXJ0L2dzb3JnYW5pemF0aW9udmFsZzIuY3J0MDsGCCsGAQUFBzABhi9odHRwOi8vb2NzcDIuZ2xvYmFsc2lnbi5jb20vZ3Nvcmdhbml6YXRpb252YWxnMjAdBgNVHQ4EFgQUZJVXf3bjSwMt8OprELV38Eu2dLUwHwYDVR0jBBgwFoAUXUayjcRLdBy77fVztjq3OI91nn4wDQYJKoZIhvcNAQEFBQADggEBAJuEdfOd3NkWvn7qlcqPl/JHWol6oFE9b9Zr9h38TnfWmlx9HxfQGWMa7g90g9WAbhiRMnWYPIpqf9fiw+nvPQYVM8KbU3DQ7wSy47tIi8B55keAuMVh164p5S3yjXozXqrpTjnPzc5EB5hbHhKH6Nn8769fzxjSsiUAD006+IDLRgmECeQkVThi5cRNGfTYTZLVlfkC7C0QbF8OlWQ3IQ1gh8htoYIiWgBPqcAfKM38NR3ercAZTJBXBgjc3egZLw41qlLq+INXeT0UnmpcnoLZu5wLkTljgefDEh5TO6QqqjtrZh/SAbcg+rgZazDHaPdsAZGgrJtljjAvLcsSrsk=</X509Certificate></X509Data></KeyInfo><CipherData><CipherValue>mBqILq5JSuk869GNDkf9gJmnM+Ox3PhaSFN2kcHN1boCN3tHADd3GsvhwCkMY6eGQEzYKPVsgs5YeebaAGSDp3c5trRo1thmllcChIZAzxSAnh5YakAXbT7sKk6hyNBXxrv3OsN+vWEMtgwIne3ZmAkhP6Jd/hHdYKsd6nlLbH9xwXk+QNmcHCrHDHg0+4CA6FBqLWKhrgSFwArV/Z6d2NXh7DGWaj45bbjrZDa/qPc/yNSx7C5iGRJz7KBW5/LQGytTN0g9yNfoz909ESG0Jcf1NzaKPWfm1hbncj3mBIb06olxxH6q+LLoPAZB2RZofcq0EQNoxt27dXxplQoQ+A==</CipherValue></CipherData></EncryptedKey></KeyInfo><CipherData><CipherValue>6u1Hfq8CNfb0Lm/tZGAmapMr3coONCKMvRPeP7HtXhOLCnMjMFNI8bRlxjzq4kMxdzVopcuB/ac4wbWIDomDJl13D704VfXNnuh6C/pgKdho+HQd2lY3akH7BjBUJz/egQ1S9dqIflIJGO9HnBQKA1xH7n8gkRoPqLpZwexhTYt2nDz9pRvUBYiZTmhsHzP29BEEZO9k+qUqHHfOxTFOWAk6pVa9vFP8j5/73smaxDK0IOunf2cDZDQZIpz2K7wu5+liQaLO9iWF2CM44B4XgzrE9TiMivUYLFmUGms19DW2DzITUgP8gxZ46SyKRYfz7WVV3HH+XOAjvSulzbXKg5ELIuMoeK2zAb1ic5ZitLl1YdTGAurrI1A+iV2hJFkJwj6lmu6gd/Fin+VUdiAFcIbajYazd2IZblJzs/YsanE3GSEaa54LftTBUJUtwxs7OeWUqZvs2p/DwzDJQXO28yXV6ub05CijGRlpolYyt0QXnAEKR1j8s6Rsimcpa6zqC4UIYwS7eGae7hkTQmdmwyDKPRlZMCrDCJAGBYQbsxeZLBclCom5Mu+zGfzMqLUkNGGezSNdXsSTfozuN7DWfE5T9mYfAE94+30J8xHodpsbncYu5KrqVSaTlFuArBbahApq7rIcQNeRQMHRDxs7FCsdKEHIhkBs4mNJQ0aWgMYYFylNo231NCDbLmoieqUH1AElYEktpkfcVwgbQBlPQ3nkB57JLROPHnj/z8V/9vL9BSgDaMl5Sx9IiM/QZFl2zvA/J4c9fhvvXPw4umr8Lg5odqrEsWEs9LVlRnZoWQhpOt05YB1nk5V64T0NmedkozbVXUDcOqRPpMq5GNOVlskBRT/yxzi4uyOcEJTGhtOux/aag+9+IkYoBtzvv/XftqbR4uRz/s4PC0ZCemI0SMiNAS2rdeWIgjNXElWBaqkOo7rBV2W30JKTNoGCSVHzpd0xCGBeau9iSRGHXzHyiWh4A6Zpea7tdTq71RKre3Ecrzh8zdbXRkgwZvuN8GPtdunw4VJbfBd+iz7cqS55xy1IyceRDZzUNZ4o8EDh9ZOofTVbJ2Uee/Lkx2H3lk6fTO20CGc/0EJj82miZX9hLWDK+498BO+3kfGHEhsovP3jL/VcGv5Y+dzTtMOX5I0m6eweRnmpKt+jxbwhOqBE3UzOlLOyY3krpcihF5SekhQ5kWr4FFgJwmTSELRKOZrJWngCBYZFYmHZqC8wLQsjGzKLb19FH3sqm/0nN4ddvaPFRYE1dzDj+f+s3MaZj7JWlADCMaQxt/n2H9y2o3/uYZQmYqreOjo62zG4/boy57FZwVfRBO4Y+mWmJ4LEnM8SwYPnicOeM23A7TfeCABp0BlXp0lYS62NIGSVRype9Fo=</CipherValue></CipherData></EncryptedData></saml:EncryptedAssertion></samlp:Response>";
            return Convert.ToBase64String(Encoding.Default.GetBytes(result));
        }

        #endregion
    }
}