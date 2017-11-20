namespace expenses
{
    using System;

    public partial class Logon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated && Session["OdometerReadingsOnLogon"] == null)
            {
                Response.Redirect("~/home.aspx", true);
            } 
            else
            {
                Session.Remove("OdometerReadingsOnLogon");
                Response.Redirect("~/shared/logon.aspx", true);
            }
        }
    }
}
