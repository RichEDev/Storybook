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

using expenses.Old_App_Code;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses.admin
{
    public partial class unallocated_cards : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Unallocated Cards";
            Master.title = Title;
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

                cCardStatements clsstatements = new cCardStatements(user.AccountID);
                cCardStatement statement = clsstatements.getStatementById(statementid);
                usrunallocatedcardnumbers.provider = statement.Corporatecard.cardprovider.cardprovider;
                usrunallocatedcardnumbers.accountid = user.AccountID;
                usrunallocatedcardnumbers.statementid = statementid;
            }
        }
    }
}
