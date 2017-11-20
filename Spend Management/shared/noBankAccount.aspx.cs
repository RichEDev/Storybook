
namespace Spend_Management.shared
{
    using System;

    public partial class NoBankAccount : System.Web.UI.Page
    {
        /// <summary>
        /// Restricted  Access page, used if the current user does not have a bank account when one if required to record and expense item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "No Active Bank Account Configured";
            Master.title = Title;

            var sbErrorTxt = new System.Text.StringBuilder();
            sbErrorTxt.Append("<div class=\"inputpanel\">You do not currently have an active bank account configured within Expenses. <br />");
            sbErrorTxt.Append( "A bank account must be created within My Details or you will be unable to add any expense items to your claim.");
            sbErrorTxt.Append("</div>");
            sbErrorTxt.AppendFormat("<div class=\"inputpanel\">To return to the welcome page please <a href=\"{0}://{1}{2}/home.aspx\" title=\"Home\">Click Here</a>.</div>", (Request.IsSecureConnection ? "https" : "http"), Request.Url.Host, cMisc.Path);

            Master.LitMenu = sbErrorTxt.ToString();
        }
    }
}