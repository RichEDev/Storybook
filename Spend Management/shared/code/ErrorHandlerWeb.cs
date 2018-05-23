namespace Spend_Management
{
    using System;
    using System.Text;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Used to generate error emails when errors happen in the application - used HttpContext to pass details to ErrorHandler
    /// </summary>
    public class ErrorHandlerWeb : ErrorHandler
    {
        /// <summary>
        /// When a record is not found, i.e. it's been deleted
        /// </summary>
        public static string MissingRecordUrl
        {
            get { return string.Format("{0}?missing", ErrorPageUrl); }
        }

        /// <summary>
        /// 404 error page
        /// </summary>
        public static string FourOhFourUrl
        {
            get { return string.Format("{0}?404", ErrorPageUrl); }
        }

        /// <summary>
        /// Gets the relative url to the insufficient/unauthorised access page.
        /// </summary>
        public static string InsufficientAccess
        {
            get
            {
                return "~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.";
            }
        }

        /// <summary>
        /// Gets the relative url to the requires bank account to claim expenses error page
        /// </summary>
        public static string NoBankAccount
        {
            get
            {
                return "~/shared/noBankAccount.aspx";
            }
        }


        public static string LogOut
        {
            get
            {
                return "~/shared/process.aspx?process=1";
            }
        }


        private const string ErrorPageUrl = "~/publicPages/error.aspx";

        /// <summary>
        /// Sends an error when no user context is available due to being ran from an automated process i.e. ESR outbound import.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool SendAutomatedProcessError(Exception ex)
        {
            var module = FunkyInjector.Container.GetInstance<IDataFactory<IProductModule, Modules>>()[GlobalVariables.DefaultModule];

            return GenerateEmail(null, null, module, null, null, ex, true);
        }
        
        /// <summary>
        /// Sends an error when user context is available
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool SendError(CurrentUser currentUser, Exception ex)
        {
            bool response = true;

            if (ex != null && ex.Message.StartsWith("Could not load type "))
            {
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Response.Redirect(FourOhFourUrl, true);
                }
            }
            else
            {
                var module = FunkyInjector.Container.GetInstance<IDataFactory<IProductModule, Modules>>()[GlobalVariables.DefaultModule];

                if (currentUser != null)
                {
                    cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(currentUser.Account.accountid);
                    cAccountSubAccount currentSubAccount = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId);

                    response = GenerateEmail(currentUser.Account, currentSubAccount, module, currentUser.Employee, currentUser.Delegate, ex);
                }
                else
                {
                    response = GenerateEmail(null, null, module, null, null, ex);
                }
            }

            return response;
        }

        private void GetBottomLinks(ref StringBuilder content, string websiteAddress, bool homePage, bool helpAndSupport)
        {
            if (!homePage && !helpAndSupport) return;

            content.AppendLine("<p><b>Where to go now?</b></p>");

            content.AppendLine("<ul>");
            if (homePage)
            {
                content.AppendFormat("<li>Return to the <a href=\"{0}{1}\">homepage</a>.</li>", websiteAddress, "/home.aspx");
            }
            if (helpAndSupport)
            {
                content.AppendFormat("<li>Access <a href=\"{0}{1}\">Help &amp; Support</a>.</li>", websiteAddress, "/shared/helpAndSupport.aspx");
            }
            content.AppendLine("</ul>");
        }

        /// <summary>
        /// Generates all the information required on the error.aspx page
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="websiteAddress"> </param>
        /// <returns></returns>
        public ErrorPageDetails GenerateErrorPage(string errorCode, string websiteAddress)
        {
            ErrorPageDetails errorPageDetail = new ErrorPageDetails();
            StringBuilder sb = new StringBuilder();

            bool selectedError = false;
            string errorImage = string.Empty;
            string errorTitle = string.Empty;

            errorPageDetail.CompanyLogoImageUrl = string.Format("{0}/images/branding/company/selenity.png", GlobalVariables.StaticContentLibrary);

            if (string.IsNullOrWhiteSpace(errorCode) == false)
            {
                switch (errorCode)
                {
                    case "404":
                        errorImage = "404.jpg";
                        errorTitle = "Page Not Found";
                        sb.AppendLine("<p><span id=\"errorHighlight\">Oops,</span> it looks like you have stumbled across a page that doesn't exist. The page may have packed its bags and moved away.</p>");
                        GetBottomLinks(ref sb, websiteAddress, true, true);
                        selectedError = true;
                        break;
                    case "missing":
                        errorImage = "missing.jpg";
                        errorTitle = "Record Not Found";
                        sb.AppendLine("<p><span id=\"errorHighlight\">Oh dear,</span> the page you are looking for cannot be found. Don't worry though we will help you get to the right place.</p>");
                        GetBottomLinks(ref sb, websiteAddress, true, true);
                        selectedError = true;
                        break;
                    case "permissions":
                        errorImage = "permissions.jpg";
                        errorTitle = "Insufficient Permissions";
                        sb.AppendLine("<p><span id=\"errorHighlight\">Unfortunately,</span> we cannot display the page you are trying to view due to insufficient permissions.</p>");
                        this.GetBottomLinks(ref sb, websiteAddress, true, true);
                        break;
                }
            }

            if (selectedError == false)
            {
                errorImage = "500.jpg";
                errorTitle = "Internal Server Error";

                sb.AppendLine("<p><span id=\"errorHighlight\">Sorry,</span> this page has encountered an error. We are working hard to fix it and appreciate your patience.</p>");

                GetBottomLinks(ref sb, websiteAddress, true, true);
            }

            errorPageDetail.PageTitle = errorTitle;
            errorPageDetail.ErrorImageUrl = string.Format("{0}/images/errors/{1}", GlobalVariables.StaticContentLibrary, errorImage);
            errorPageDetail.ErrorImageAlt = errorTitle;
            errorPageDetail.ErrorText = sb.ToString();

            return errorPageDetail;
        }
    }

    /// <summary>
    /// Holds all the information required on the error page
    /// </summary>
    public class ErrorPageDetails
    {
        /// <summary>
        /// The Url to the company logo image
        /// </summary>
        public string CompanyLogoImageUrl { get; set; }
        /// <summary>
        /// The alt text for the company logo
        /// </summary>
        public string CompanyLogoImageAlt { get; set; }
        /// <summary>
        /// The Url to thhe error image
        /// </summary>
        public string ErrorImageUrl { get; set; }
        /// <summary>
        /// The alt text for the error image
        /// </summary>
        public string ErrorImageAlt { get; set; }
        /// <summary>
        /// The text displayed on the error page
        /// </summary>
        public string ErrorText { get; set; }
        /// <summary>
        /// The title of the error page
        /// </summary>
        public string PageTitle { get; set; }
    }
}
