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
using System.Web.Services;
using expenses.Old_App_Code;

using Spend_Management;
using SpendManagementLibrary;

namespace expenses.admin
{
    using System.Collections.Generic;

    public partial class statements : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Corporate Card Statements";
            Master.title = Title;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CorporateCards, true, true);

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);
                cCardStatements clsstatements = new cCardStatements(user.AccountID);

                if (reqAccount.CorporateCardsEnabled == false)
                {
                    Response.Redirect("~/home.aspx", true);
                }

                string[] gridData = clsstatements.createGrid(user.AccountID, user.EmployeeID);
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CardStatementsGridVars", cGridNew.generateJS_init("CardStatementsGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            }
        }


        [WebMethod(EnableSession = true)]
        public static bool deleteStatement(int accountid, int statementid)
        {
            cCardStatements clsstatements = new cCardStatements(accountid);
            return clsstatements.deleteStatement(statementid);
        }
    }
}
