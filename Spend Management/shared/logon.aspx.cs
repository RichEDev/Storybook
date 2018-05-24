namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using Common.Cryptography;
    using Spend_Management.shared.code;
    using SpendManagementLibrary;

    using shared.code.Authentication;
    using shared.code.Logon;

    /// <summary>
    /// The logon page.
    /// </summary>
    public partial class logonPage : Page
    {
        /// <summary>
        /// Gets or sets the value indicating whether or not to remind the user about the forgotten details feature
        /// </summary>
        public bool ShowForgottenDetails { get; set; }

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
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
        /// Can't register as dependency due to logon being avoided for Page initialization
        /// </summary>
        private readonly IDataFactory<IProductModule, Modules> _productModuleFactory =
            FunkyInjector.Container.GetInstance<IDataFactory<IProductModule, Modules>>();

        /// <summary>
        /// The page_ pre init.
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
            if (this.Encryptor == null)
            {
                this.Encryptor = new Pbkdf2Encryptor();
            }

            this.tsm.Scripts.Insert(0, new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-1.9.0.min.js"));
            this.tsm.Scripts.Insert(1, new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-ui-1.9.2.custom.min.js"));
            this.tsm.Scripts.Insert(2, new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/bxSlider/jquery.bxslider.js"));
            this.jQueryUiCss.Href = GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-ui-1.9.2.custom.css";
            this.BannerSlider.Href = GlobalVariables.StaticContentLibrary + "/js/bxSlider/jquery.bxslider.css";

            this.Module = HostManager.GetModule(Request.Url.Host);
            var module = this._productModuleFactory[this.Module];

            var clsInfoMessages = new cInformationMessages();
            List<cInformationMessage> listInfoMessages = clsInfoMessages.GetMessages();

            var logonMessages = new LogonMessages();
            if (string.IsNullOrEmpty(Request.QueryString["previewId"]) || string.IsNullOrEmpty(User.Identity.Name))
            {
                List<LogonMessage> logonMessageList = logonMessages.GetAllActiveMessages(module.Id);
                BannerGenerator.DataSource = logonMessageList;
                BannerGenerator.DataBind();
            }
            else
            {
                this.lnkSelfRegistration.NavigateUrl = "#";
                this.btnLogon.Click -= this.btnLogon_Click;
            }

            this.txtCompanyID.Attributes.Add("AUTOCOMPLETE", "OFF");
            this.txtUsername.Attributes.Add("AUTOCOMPLETE", "OFF");
            this.userIPAddress.Text = string.Format("<!-- IP- {0}-->", Request.UserHostAddress);
            this.ShowForgottenDetails = false;
            this.pnlLocked.Visible = false;

            if (listInfoMessages.Count == 0)
            {
                this.informationcontainer.Visible = false;
            }
            else
            {
                var sb = new StringBuilder();

                foreach (cInformationMessage infoMessage in listInfoMessages)
                {
                    var message = string.Format("<p class='{0}'>{1}</p>", infoMessage.Title, infoMessage.Message);
                    sb.Append(message);
                }

                this.litInformationMessages.Text = sb.ToString();
            }

            this.favLink.Href = cMasterPageMethods.GetFavIcon(this.Module);

            // hide the self registration button as it is only applicable for expenses at present
            this.lnkSelfRegistration.Visible = false;
            const string Logon = "logon";
            const string LogonTo = "Logon to";

            var clsSecure = new cSecureData();
            switch (this.Module)
            {
                case Modules.Contracts:
                    this.imgBrandingLogo.ImageUrl = "~/shared/images/branding/FRA152-wp.png";
                    this.litPageTitle.Text = string.Format("{0} {1}", LogonTo, module.BrandNameHtml);
                    this.Title = string.Format("{0} {1}", module.BrandName, Logon);
                    break;
                case Modules.SmartDiligence:
                    this.imgBrandingLogo.ImageUrl = "~/shared/images/spacer.png";
                    this.litPageTitle.Text = string.Empty;
                    this.Title = string.Format("&reg; {0}", Logon);
                    break;
                case Modules.SpendManagement:
                    this.imgBrandingLogo.ImageUrl = "~/shared/images/branding/expenses76w_77h.png";
                    this.litPageTitle.Text = string.Format("{0} {1}", LogonTo, module.BrandNameHtml);
                    this.Title = string.Format("{0} {1}", module.BrandName, Logon);
                    break;
                case Modules.Expenses:
                    this.imgBrandingLogo.ImageUrl = "~/shared/images/branding/EXP152-wp.png";
                    this.litPageTitle.Text = string.Format("{0} {1}", LogonTo, module.BrandNameHtml);
                    this.Title = string.Format("{0} {1}", module.BrandName, Logon);
                    this.lnkSelfRegistration.Visible = true;
                    break;
                case Modules.Greenlight:
                    this.imgBrandingLogo.ImageUrl = "~/shared/images/branding/greenlightlogo.png";
                    this.litPageTitle.Text = string.Format("{0} {1}", LogonTo, module.BrandNameHtml);
                    this.Title = string.Format("{0} {1}", module.BrandName, Logon);
                    break;
                case Modules.GreenlightWorkforce:
                    this.imgBrandingLogo.ImageUrl = "~/shared/images/branding/greenlightworkforcelogo.png";
                    this.litPageTitle.Text = string.Format("{0} {1}", LogonTo, module.BrandNameHtml);
                    this.Title = string.Format("{0} {1}", module.BrandName, Logon);
                    break;
                case Modules.CorporateDiligence:
                    this.imgBrandingLogo.ImageUrl = "~/shared/images/branding/corporated.png";
                    this.litPageTitle.Text = string.Format("{0} {1}", LogonTo, module.BrandNameHtml);
                    this.Title = string.Format("{0} {1}", module.BrandName, Logon);
                    break;
                default:
                    this.imgBrandingLogo.ImageUrl = "~/shared/images/branding/expenses76w_77h.png";
                    this.litPageTitle.Text = string.Format("{0} {1}", LogonTo, module.BrandNameHtml);
                    this.Title = string.Format("{0} {1}", module.BrandName, Logon);
                    break;
            }

            if (string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]) == false && (Request.QueryString["ReturnUrl"].ToLower().Contains("logon.aspx") || Request.QueryString["ReturnUrl"].ToLower().Contains("default.aspx") || Request.QueryString["ReturnUrl"].ToLower().Contains("process.aspx") || Request.QueryString["ReturnUrl"].ToLower() == "%2f"))
            {
                cEventlog.LogEntry("Logon.aspx.cs: ReturnUrl ");
                Response.Redirect("~/shared/logon.aspx");
            }

            if (!this.IsPostBack)
            {
                try
                {
                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    if (authCookie != null)
                    {
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                        if (authTicket.Expired)
                        {
                            // session timedout and redirected to the logon page, remove any concurrent logon info
                            string[] ids = authTicket.Name.Split(',');
                            Guid manageId = new Guid(authTicket.UserData);
                            if (manageId != Guid.Empty)
                            {
                                cConcurrentUsers.LogoffUser(manageId, int.Parse(ids[0]));
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }

                if (Request.Cookies["LoginInfo"] != null)
                {
                    bool remember = false;
                    var httpCookie = Request.Cookies["LoginInfo"];
                    if (httpCookie != null)
                    {
                        bool.TryParse(httpCookie["remember"], out remember);
                    }

                    if (remember)
                    {
                        var cookie = Request.Cookies["LoginInfo"];
                        if (cookie != null)
                        {
                            this.txtCompanyID.Text = cookie["company"];
                        }

                        var httpCookie1 = Request.Cookies["LoginInfo"];
                        if (httpCookie1 != null)
                        {
                            this.txtUsername.Text = httpCookie1["username"];
                        }
                    }

                    this.chkRememberDetails.Checked = remember;
                }

                if (string.IsNullOrEmpty(Request.QueryString["a"]) == false && string.IsNullOrEmpty(Request.QueryString["b"]) == false && string.IsNullOrEmpty(Request.QueryString["c"]) == false)
                {
                    string companyId = HttpUtility.UrlDecode(clsSecure.Decrypt(Request.QueryString["a"]));
                    string username = HttpUtility.UrlDecode(clsSecure.Decrypt(Request.QueryString["b"]));
                    string password = HttpUtility.UrlDecode(clsSecure.Decrypt(Request.QueryString["c"]));
                    this.Logon(username, password, companyId, false);
                }
            }

            if (User.Identity.Name != string.Empty)
            {
                if (string.IsNullOrEmpty(Request.QueryString["previewId"]))
                {
                    Response.Redirect("~/shared/process.aspx?process=1", true);
                }
            }

            if (this.litMessage.Text == string.Empty)
            {
                this.litMessage.Text = "<div class=\"dividerfiller\"></div>";
            }

            var sbjs = new StringBuilder();
            sbjs.Append("LogonVars = function () {\n");
            sbjs.Append("SEL.Logon.ModalForgottenDetailsEmailAddressDomID = \"" + this.txtEmailAddress.ClientID + "\";\n");
            sbjs.Append("SEL.Logon.ModalForgottenDetailsMessageDomID = \"forgottenDetailsMessage\";\n");
            sbjs.Append("SEL.Logon.ModalForgottenDetailsSubmitSpanDomID = \"spanForgottenDetailsSubmit\";\n");
            sbjs.Append("SEL.Logon.ModalForgottenDetailsCloseSpanDomID = \"spanForgottenDetailsClose\";\n");
            sbjs.Append("SEL.Logon.HostName = \"" + Request.Url.Host + "\";\n");
            sbjs.Append("SEL.Logon.CompanyIDDomID = \"" + this.txtCompanyID.ClientID + "\";\n");
            sbjs.Append("SEL.Logon.UsernameDomID = \"" + this.txtUsername.ClientID + "\";\n");
            sbjs.Append("SEL.Logon.PasswordDomID = \"" + this.txtPassword.ClientID + "\";\n");

            sbjs.Append("SEL.Logon.SetLogonControlFocus();\n");
            sbjs.Append("};\n");

            sbjs.Append("LogonVars.registerClass(\"LogonVars\", Sys.Component);\n");
            sbjs.Append("Sys.Application.add_init(\n");
            sbjs.Append("\tfunction(){\n");
            sbjs.Append("\t\t$create(LogonVars, {\"id\":\"LogonVariables\"}, null, null, null)\n");
            sbjs.Append("\t}\n");
            sbjs.Append(");\n");

            sbjs.AppendLine("\n");
            sbjs.AppendLine("MasterPopupInfo = function () {");
            sbjs.AppendLine("SEL.MasterPopup.ModalDOMID = '" + this.mdlMasterPopup.ClientID + "'");
            sbjs.AppendLine("};");

            sbjs.AppendLine("MasterPopupInfo.registerClass(\"MasterPopupInfo\", Sys.Component);");
            sbjs.AppendLine("Sys.Application.add_init(");
            sbjs.AppendLine("\tfunction(){");
            sbjs.AppendLine("\t\t$create(MasterPopupInfo, {\"id\":\"MasterPopupInfoVariables\"}, null, null, null)");
            sbjs.AppendLine("\t}");
            sbjs.AppendLine(");");

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LogonVars", sbjs.ToString(), true);

        }

        /// <summary>
        /// The btn logon_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void btnLogon_Click(object sender, EventArgs e)
        {
            this.Logon(this.txtUsername.Text, this.txtPassword.Text, this.txtCompanyID.Text, true);
        }

        /// <summary>
        /// The logon.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <param name="usedForm">
        /// The used form.
        /// </param>
        private void Logon(string username, string password, string companyId, bool usedForm)
        {
            this.txtCompanyID.Text = companyId.Trim();
            this.txtUsername.Text = username.Trim();
            var logon = new Logon(this.Encryptor);

            if (usedForm && (!this.rfCompanyID.IsValid || !this.rfUsername.IsValid))
            {
                this.litMessage.Text = "Please complete all of the required fields.";
            }
            else
            {
                this.litMessage.Text = logon.LogonUser(
                    username, companyId, password, this.Request, this.Session, this.Response, false, this.chkRememberDetails.Checked, this.Module);
            }

            this.ShowForgottenDetails = logon.ShowForgottenDetails;
        }


        /// <summary>
        /// Checks for Null or Empty string before populating html control
        /// </summary>
        /// <param name="ColumnValue">value of the field</param>
        /// <returns>Bool value</returns>
        public bool CheckForNullOrEmpty(object ColumnValue)
        {
            if (String.IsNullOrEmpty(ColumnValue.ToString()) || ColumnValue == DBNull.Value)
                return true;

            return false;
        }
    }
}
