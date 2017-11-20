using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Spend_Management;

namespace Spend_Management
{
    public partial class edittooltip : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Tooltips, true, true);

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                    Master.helpid = 1182;
                    break;
                default:
                    Master.helpid = 0;
                    break;
            }

            Master.enablenavigation = false;

            if (IsPostBack == false)
            {
                //int helpid = Convert.ToInt32(Request.QueryString["helpid"]);
                Guid tooltipID = new Guid(Request.QueryString["tooltipID"]);
                //ViewState["helpid"] = helpid;
                ViewState["tooltipID"] = tooltipID;
                
                ViewState["accountid"] = currentUser.AccountID;
                ViewState["employeeid"] = currentUser.EmployeeID;

                cHelp clshelp = new cHelp(currentUser.AccountID);
                cHelpItem item = clshelp.getHelpById(tooltipID);
                
                lbldescription.Text = item.description;
                txttooltip.Text = clshelp.ReplaceLineBreak(item.helptext);

                Title = "Tooltip: " + item.description;
                Master.title = Title;
                Master.PageSubTitle = "Edit Tooltip";
            }
        }

        protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("tooltips.aspx", true);
        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            string text = txttooltip.Text;
            string formattedText;
        
            cHelp clshelp = new cHelp((int)ViewState["accountid"]);
            formattedText = clshelp.ReplaceNewLine(text);
            clshelp.saveTooltip((Guid)ViewState["tooltipID"], formattedText);
            Response.Redirect("tooltips.aspx", true);
        }

        protected void cmdrestore_Click(object sender, EventArgs e)
        {
            cHelp clshelp = new cHelp((int)ViewState["accountid"]);
            clshelp.restoreTooltip((Guid)ViewState["tooltipID"]);
            Response.Redirect("tooltips.aspx", true);
        }
    }
}
