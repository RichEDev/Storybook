namespace Spend_Management.publicPages
{
    using System;

    /// <summary>
    /// Used to display errors in the web applications
    /// </summary>
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ErrorHandlerWeb errorHandler = new ErrorHandlerWeb();

            string errorCode = string.Empty;

            if (Request.QueryString.Count > 0)
            {
                if (Request.QueryString[0] == "404" || Request.QueryString[0] == "missing" || Request.QueryString[0] == "permissions")
                {
                    errorCode = Request.QueryString[0];
                }
            }

            ErrorPageDetails errorPageDetails = errorHandler.GenerateErrorPage(errorCode, cMisc.Path);
            
            Page.Title = errorPageDetails.PageTitle;
            this.errorTitle.Text = errorPageDetails.PageTitle;
            this.errorText.Text = errorPageDetails.ErrorText;

            this.errorImageContent.ImageUrl = errorPageDetails.ErrorImageUrl;
            this.errorImageContent.ToolTip = errorPageDetails.ErrorImageAlt;

            this.companyLogo.ImageUrl = errorPageDetails.CompanyLogoImageUrl;
            this.companyLogo.ToolTip = errorPageDetails.CompanyLogoImageAlt;
        }
   }
}