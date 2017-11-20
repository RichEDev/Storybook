namespace Spend_Management
{
    using System;

    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Redirect(this.User.Identity.IsAuthenticated ? "~/home.aspx" : "~/shared/logon.aspx");
        }
    }
}
