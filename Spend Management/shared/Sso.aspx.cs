
namespace Spend_Management.shared
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Modules;

    using code.Authentication;

    using Common.Cryptography;

    using global::Sso;
    using global::Sso.AttribStatement;

    using SpendManagementLibrary;

    /// <summary>
    /// The Single Sign On logon page.
    /// </summary>
    public partial class Sso : System.Web.UI.Page
    {
        /// <summary>
        /// Gets or sets the module.
        /// </summary>
        protected Modules Module { get; set; }

        /// <summary>
        /// A public instance of <see cref="IEncryptor"/>
        /// </summary>
        [Dependency]
        public IEncryptor Encryptor { get; set; }

        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        /// <summary>
        /// The page_ pre initialise.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Response.Expires = 60;
            Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
            Response.AddHeader("pragma", "no-cache");
            Response.AddHeader("cache-control", "private");
            Response.CacheControl = "no-cache";
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Saml.DebugMessage("Sso.aspx.Page_Load()");
                this.Module = HostManager.GetModule(Request.Url.Host);

                var samlRequest = Request.Form["SAMLResponse"];
                Saml.DebugMessage("samlRequest = " + (String.IsNullOrWhiteSpace(samlRequest) ? "undefined" : samlRequest));

                if (samlRequest != null)
                {
                    Saml.DebugMessage("Reading password from config file");
                    var crypto = new cSecureData();
                    var password = GlobalVariables.GetAppSetting("SelPrivateCertificatePassword");

                    Saml.DebugMessage("Encrypted password is " + password);
                    password = crypto.Decrypt(password);

                    var certificateFilename = GlobalVariables.StaticContentFolderPath + GlobalVariables.GetAppSetting("SelPrivateCertificateSCPath");
                    Saml.DebugMessage("Loading certificate from " + certificateFilename);
                    var saml = new Saml(new X509Certificate2(certificateFilename, password, X509KeyStorageFlags.MachineKeySet));

                    SingleSignOn ssoForRedirect = null;
                    var results = saml.DeserializeSaml(Request.Url.Host, samlRequest);
                    foreach (var result in results)
                    {
                        if (results.Length == 1)
                        {
                            ssoForRedirect = result.SsoConfig;
                        }

                        if (result.Valid)
                        {
                            string companyId = null;
                            string identifier = null;
                            var companyIdAttributes = result.SsoConfig.CompanyIdAttribute.Split(',');
                            var identifierAttributes = result.SsoConfig.IdAttribute.Split(',');
                            foreach (SsoAttribute ssoAttribute in result.AttributeStatement.Attributes)
                            {
                                if (companyId == null && companyIdAttributes.Any(att => string.Compare(ssoAttribute.Name, att, true, CultureInfo.InvariantCulture) == 0) && ssoAttribute.Values.Count > 0)
                                {
                                    companyId = ssoAttribute.Values[0];
                                }

                                if (identifier == null && identifierAttributes.Any(att => string.Compare(ssoAttribute.Name, att, true, CultureInfo.InvariantCulture) == 0) && ssoAttribute.Values.Count > 0)
                                {
                                    identifier = ssoAttribute.Values[0];
                                }
                            }

                            Saml.DebugMessage(string.Format("companyId={0}\r\nidentifier={1}", companyId, identifier));

                            try
                            {
                                if (string.IsNullOrEmpty(companyId))
                                {
                                    throw new InvalidOperationException("Company ID was not specified.");
                                }

                                if (string.IsNullOrEmpty(identifier))
                                {
                                    throw new InvalidOperationException("Identifier was not specified.");
                                }

                                cAccount account = cAccounts.CachedAccounts.Values.FirstOrDefault(acc => acc.companyid == companyId);

                                if (account == null)
                                {
                                    throw new InvalidOperationException("Account not found.");
                                }

                                Saml.DebugMessage(String.Format("account.accountid = {0}", account.accountid));

                                if (account.accountid != result.AccountId)
                                {
                                    throw new InvalidOperationException(String.Format("The account ID ({0}) for the specified Company ID ({1}) does not match the account ID ({2}) for the Single Sign-on configuration.", account.accountid, companyId, result.AccountId));
                                }

                                ssoForRedirect = result.SsoConfig;

                                if (account.archived)
                                {
                                    throw new InvalidOperationException("Account is archived.");
                                }

                                if (!account.HasLicensedElement(SpendManagementElement.SingleSignOn))
                                {
                                    throw new InvalidOperationException("Single Sign-on is not licensed on this account.");
                                }

                                var logon = new Logon(this.Encryptor);
                                var employeeIdAccountId = logon.FindEmployee(account, identifier, result.SsoConfig.IdLookupFieldId);

                                Saml.DebugMessage(string.Format("employeeIdAccountId = {0}", employeeIdAccountId));

                                try
                                {
                                    logon.LogonUser(
                                        employeeIdAccountId,
                                        account.accountid,
                                        this.Request,
                                        this.Session,
                                        this.Response,
                                        true,
                                        false,
                                        this.Module);
                                }
                                catch (System.Threading.ThreadAbortException)
                                {
                                    // The LogonUser method does a redirect to the homepage so this thread gets immediately aborted *on success*
                                    Saml.DebugMessage("Employee logged in successfully");
                                    throw;
                                }
                            }
                            catch (InvalidOperationException ex)
                            {
                                if (result.SsoConfig == ssoForRedirect)
                                {
                                    this.litMessage.Text = "Unable to validate credentials.";
                                    this.litDetail.Text = ex.Message + " Please contact your administrator.";
                                }

                                Saml.DebugMessage("Invalid Operation - " + ex.Message);
                            }
                        }
                        else
                        {
                            if (results.Length == 1)
                            {
                                this.litMessage.Text = "Unable to validate credentials.";
                            }

                            var message = "Result is invalid";

                            if (!string.IsNullOrEmpty(result.Message))
                            {
                                if (results.Length == 1)
                                {
                                    this.litDetail.Text = result.Message;
                                }

                                message += " - " + result.Message;
                            }

                            Saml.DebugMessage(message);
                        }
                    }

                    if (ssoForRedirect == null)
                    {
                        if (results.Length > 1)
                        {
                            this.litMessage.Text = "Unable to validate credentials.";
                            this.litDetail.Text = "Please contact your administrator.";
                        }
                    }
                    else if (!String.IsNullOrEmpty(ssoForRedirect.LoginErrorUrl))
                    {
                        Response.Redirect(ssoForRedirect.LoginErrorUrl);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Saml.DebugMessage(String.Format("Unhandled exception - {0}\n\n{1}", ex.GetType().FullName, ex));
            }
        }
    }
}