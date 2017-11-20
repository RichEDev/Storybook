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
using expenses;
using SpendManagementLibrary;
using Spend_Management;

public partial class claimsmenu : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            Title = "My Claims";
            Master.Title = Title;
            CurrentUser user = cMisc.GetCurrentUser();

            cClaims clsclaims = new cClaims(user.AccountID);

            Master.AddMenuItem("document_into", 48, "Current Claims (" + clsclaims.getCount(user.EmployeeID, ClaimStage.Current) + ")", "Claims that have yet to be submitted into the approval process. Add, edit or delete expense items and claims. Submit a claim for approval.", "expenses/claimsummary.aspx?claimtype=1");
            Master.AddMenuItem("document_out", 48, "Submitted Claims (" + clsclaims.getCount(user.EmployeeID, ClaimStage.Submitted) + ")", "Claims that are currently being approved. View the status of these claims or amend any returned items.", "expenses/claimsummary.aspx?claimtype=2");
            Master.AddMenuItem("document_lock", 48, "Previous Claims (" + clsclaims.getCount(user.EmployeeID, ClaimStage.Previous) + ")", "Historical claims that have been approved.", "expenses/claimsummary.aspx?claimtype=3");
        }
    }
}
