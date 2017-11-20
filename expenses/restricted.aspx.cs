using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class restricted : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "You do not have permission to view this page";
        Master.Title = Title;

        System.Text.StringBuilder sbErrorTxt = new System.Text.StringBuilder();
        sbErrorTxt.Append("<div class=\"main-content-area\" style=\"min-height: 601px !important; background:#eeeeee !important;\"><div class=\"inputpanel\"><label>You do not have permission to view this page. </label>");
        sbErrorTxt.Append("<ul style=\"margin-top: 20px;\">");
        sbErrorTxt.Append("<li> This could be due to one of the following reasons: <br/> You are you trying to edit someone else's account, or trying to access administrative features. Please check with your System Administrator</li>");
        sbErrorTxt.Append("<li>Your account may have been disabled, or it may be awaiting activation. Please check with your System Administrator</li>");
        sbErrorTxt.Append("</ul>");
        sbErrorTxt.Append("</div>");
        sbErrorTxt.Append("<div class=\"inputpanel\">If you have verified your permissions with your System Administrator, but are still diverted to this screen, <a href=\"https://" + Request.Url.Host + "/shared/helpAndSupport.aspx\" title=\"Help &amp; Support\">click here</a> for further help</div>");
        sbErrorTxt.Append("<div class=\"inputpanel\">To return to the welcome page please <a href=\"https://" + Request.Url.Host + "/home.aspx\" title=\"Home\">click here</a>.</div></div>");

        Master.LitMenu = sbErrorTxt.ToString();

        System.Text.StringBuilder sbTitleBar = new System.Text.StringBuilder();
        sbTitleBar.Append("<ol class=\"breadcrumb\"><li><a href=\"/home.aspx\"><i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\" alt=\"\"></i> Home</a></li><li style=\"color: rgb(204, 204, 204);\"><label class=\"breadcrumb_arrow\">/</label>Unauthorised Section</li></ol>");
        Master.LitTitle = sbTitleBar.ToString();
    }


}