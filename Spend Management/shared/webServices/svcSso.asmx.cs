namespace Spend_Management.shared.webServices
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.RegularExpressions;
    using System.Web.Script.Services;
    using System.Web.Services;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for svcSso
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcSso : WebService
    {
        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string[] Controls { get; set; }
        }
        
        /// <summary>
        /// Save the Single sign-on configuration to the database
        /// </summary>
        /// <param name="ssoNew">The Single Sign-on data to save</param>
        /// <returns>An object which contains true on success or a message on error</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Response Save(SingleSignOn ssoNew)
        {
            try
            {
                CurrentUser user = cMisc.GetCurrentUser();

                if (!svcSso.CurrentUserHasPermission(user))
                {
                    return new Response { Message = "Permission denied." };
                }

                var ssoOriginal = SingleSignOn.Get(user);

                var errors = svcSso.Validate(user, ssoOriginal, ssoNew);
                if (errors.Length > 0)
                {
                    return new Response
                    {
                        Message = String.Join("\n", errors.Select(i => i.Item1 )),
                        Controls = errors.Select(i => i.Item2).Distinct().ToArray()
                    };
                }

                if (ssoNew.PublicCertificate == null || ssoNew.PublicCertificate.Length == 0)
                {
                    ssoNew.PublicCertificate = ssoOriginal.PublicCertificate;
                }

                ssoNew.Save(user);

                return new Response { Success = true };
            }
            catch (Exception ex)
            {
                return new Response { Message = String.Format("Error: {0}, {1}", ex.GetType().Name, ex.Message) };
            }
        }

        private static bool CurrentUserHasPermission(ICurrentUser user)
        {
            return (user != null && user.Account != null &&
                    user.Account.HasLicensedElement(SpendManagementElement.SingleSignOn) &&
                    user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SingleSignOn, true));
        }

        private static Tuple<string, string>[] Validate(ICurrentUserBase user, SingleSignOn ssoOriginal, SingleSignOn ssoNew)
        {
            var errors = new List<Tuple<string, string>>();
            // This regex ensures that the comma-separated values entered are valid XML attribute names
            var xmlAttrValidator = new Regex(@"^([a-z_:][-a-z0-9_:.]*)(,[a-z_:][-a-z0-9_:.]*)*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            if (String.IsNullOrEmpty(ssoNew.IssuerUri))
            {
                errors.Add(Tuple.Create("Please enter an Issuer.", "txtIssuerUri"));
            }
            else if (!Regex.IsMatch(ssoNew.IssuerUri, @"^[a-z]+:(//)?[^\s/$.?#<>][^\s<>]*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
            {
                errors.Add(Tuple.Create("Please enter a valid Issuer.", "txtIssuerUri"));
            }

            if (ssoNew.PublicCertificate != null && ssoNew.PublicCertificate.Length > 0)
            {
                var x509Certificate = new X509Certificate2();
                try
                {
                    x509Certificate.Import(ssoNew.PublicCertificate);
                }
                catch
                {
                    errors.Add(Tuple.Create("Please select a valid Identity provider public certificate.", "fupIpPublicCertificate"));
                }
            }
            else if (ssoOriginal == null || ssoOriginal.PublicCertificate == null || ssoOriginal.PublicCertificate.Length == 0)
            {
                errors.Add(Tuple.Create("Please select your Identity provider public certificate.", "fupIpPublicCertificate"));
            }

            if (String.IsNullOrEmpty(ssoNew.CompanyIdAttribute))
            {
                errors.Add(Tuple.Create("Please enter a Company ID attribute.", "txtCompanyIdAttribute"));
            }
            else if (!xmlAttrValidator.IsMatch(ssoNew.CompanyIdAttribute))
            {
                errors.Add(Tuple.Create("Please enter a valid Company ID.", "txtCompanyIdAttribute"));
            }

            if (String.IsNullOrEmpty(ssoNew.IdAttribute))
            {
                errors.Add(Tuple.Create("Please enter an Identifier attribute.", "txtIdentifierAttribute"));
            }
            else if (!xmlAttrValidator.IsMatch(ssoNew.IdAttribute))
            {
                errors.Add(Tuple.Create("Please enter a valid Identifier attribute.", "txtIdentifierAttribute"));
            }

            if (ssoNew.IdLookupFieldId == Guid.Empty)
            {
                errors.Add(Tuple.Create("Please select an Identifier lookup field.", "ddlIdentifierLookupField"));
            }
            else if (new cFields(user.AccountID).GetFieldByID(ssoNew.IdLookupFieldId) == null)
            {
                errors.Add(Tuple.Create("Please select a valid Identifier lookup field.", "ddlIdentifierLookupField"));
            }

            return errors.ToArray();
        }
    }
}
