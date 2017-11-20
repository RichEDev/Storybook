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
using expenses.Old_App_Code.admin;

using SpendManagementLibrary;
using Spend_Management;
using expenses.Old_App_Code;

namespace expenses.admin
{
    public partial class editstatement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Edit Statement";
            Master.title = Title;
            Master.enablenavigation = false;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CorporateCards, true, true);

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

                if (reqAccount.CorporateCardsEnabled == false)
                {
                    Response.Redirect("~/home.aspx", true);
                }

                int statementid = Convert.ToInt32(Request.QueryString["statementid"]);
                ViewState["statementid"] = statementid;

                cCardStatements clsstatements = new cCardStatements(user.AccountID);
                cCardStatement statement = clsstatements.getStatementById(statementid);
                txtname.Text = statement.name;
                if (statement.statementdate.HasValue)
                {
                    txtstatementdate.Text = statement.statementdate.Value.ToShortDateString();
                }
            }
        }

        /// <summary>
        /// Event handler for next button
        /// </summary>
        /// <param name="sender">Page from which the request came</param>
        /// <param name="e">Event arguments passed in</param>
        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            string name = this.txtname.Text;
            DateTime? statementdate = null;

            if (this.txtstatementdate.Text.Trim().Length > 0)
            {
                statementdate = DateTime.Parse(this.txtstatementdate.Text);
            }

            var accountId = (int)ViewState["accountid"];
            var clsstatements = new cCardStatements(accountId);
            cCardStatement oldstatement = clsstatements.getStatementById((int)ViewState["statementid"]);

            var newstatement = new cCardStatement(accountId, oldstatement.CorporateCardId, oldstatement.statementid, name, statementdate, oldstatement.createdon, oldstatement.createdby, DateTime.Now, (int)ViewState["employeeid"]);
            
            //Check to see if user has changed the name and then if the new name exists
            if (string.Equals(newstatement.name, oldstatement.name, StringComparison.OrdinalIgnoreCase) == false && clsstatements.checkStatementNames(newstatement))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "js", $"alert('{cCardStatements.CannotSaveExistingStatementNamesMessage}');", true);
            }
            else
            {
                clsstatements.updateStatement(newstatement);
                Response.Redirect("statements.aspx", true);
            }
        }
    }
}
