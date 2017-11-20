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

public partial class deletereason : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
        
        Master.showdummymenu = true;
        if (IsPostBack == false)
        {

            int expenseid, claimid, claimOwner;
            string editPreviousClaim;
            
            byte action;
            expenseid = int.Parse(Request.QueryString["expenseid"]);
            claimid = int.Parse(Request.QueryString["claimid"]);
            editPreviousClaim = Request.QueryString["editPrevClaim"] ?? "0";
            claimOwner = int.Parse(Request.QueryString["claimOwner"] ?? "0");
            
            action = byte.Parse(Request.QueryString["action"]);
            ViewState["expenseid"] = expenseid;
            ViewState["claimid"] = claimid;
            ViewState["editPreviousClaim"] = editPreviousClaim;
            ViewState["action"] = action;
            ViewState["claimOwner"] = claimOwner;
            

            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;

            

            switch (action)
            {
                case 2: //edit
                    Title = "Reason for Amendment";
                    lblreason.Text = "Reason for Amendment";

                    if (editPreviousClaim.Trim().Equals("1"))
                    {
                        reqreason.ErrorMessage = "Please enter a reason for amendment";
                    }
                    break;
                case 3: //delete
                    Title = "Reason for Deletion";
                    lblreason.Text = "Reason for Deletion";
                    break;
            }

            Master.title = Title;

        }
    }
    protected void cmdok_Click(object sender, ImageClickEventArgs e)
    {
        CurrentUser user = cMisc.GetCurrentUser();
        string reason = txtreason.Text;
        byte returncode = 0;
        byte action = (byte)ViewState["action"];
        bool editingPreviousClaim = (ViewState["editPreviousClaim"].ToString() == "1") ? true: false;
        cClaims clsclaims = new cClaims((int)ViewState["accountid"]);
        cClaim reqclaim = clsclaims.getClaimById((int)ViewState["claimid"]);
        cExpenseItem item = clsclaims.getExpenseItemById((int)ViewState["expenseid"]);
        string comment = "";

        switch (action)
        {
            case 2: //edit
                if (editingPreviousClaim)
                {
                    cEmployees employees = new cEmployees(user.AccountID);
                    comment = "Expense amended after approval \r\nComments: " + reason;
                }
                else
                {
                    comment = "Expense amended by authoriser.\r\nAuthoriser's Comments: " + reason;    
                }
                
                break;
            case 3: //delete
                comment = "Expense deleted by authoriser.\r\nAuthoriser's Comments: " + reason;
                returncode = clsclaims.deleteExpense(reqclaim, clsclaims.getExpenseItemById(int.Parse(Request.QueryString["expenseid"])), true, user);
                
                break;
                
        }
        int employeeid;
        if (Session["myid"] == null)
        {
            employeeid = (int)ViewState["employeeid"];
        }
        else
        {
            employeeid = (int)Session["myid"];
        }

        if (returncode != 1 && reqclaim.NumberOfItems > 0)
        {
            clsclaims.addComment(reqclaim, employeeid, comment, item.refnum);
        }

        if (editingPreviousClaim)
        {
            Response.Redirect(
                "expenses/claimViewer.aspx?claimid=" + ViewState["claimid"] + "&employeeId=" + reqclaim.employeeid.ToString() +
                "&claimSelector=1&" + ((Request.QueryString["flag"] != null) ? "&expenseid=" + Request.QueryString["flag"] : string.Empty), 
                true);
        }
        else
        {
            if (returncode == 1)
            {
                Response.Redirect("expenses/checkpaylist.aspx", true);
            }
            else
            {
                Response.Redirect("expenses/checkexpenselist.aspx?claimid=" + ViewState["claimid"], true);
            }
        }
        
    }

    protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
    {
        var editingPreviousClaim = ViewState["editPreviousClaim"].ToString();
        string claimid = ViewState["claimid"].ToString();
        string location = "expenses/checkexpenselist.aspx?claimid=" + claimid;
        if (editingPreviousClaim == "1")
        {
            location = "expenses/claimViewer.aspx?claimid=" + claimid + "&claimSelector=1" + 
                ((Request.QueryString["flag"] != null) ? "&expenseid=" + Request.QueryString["flag"] : string.Empty);
        }

        Response.Redirect(location, true);
    }
}
