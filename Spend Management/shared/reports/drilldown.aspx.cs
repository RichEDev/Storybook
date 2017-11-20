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

public partial class reports_drilldown : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;

            

            Guid reportid = new Guid(Request.QueryString["reportid"]);
            Guid basetable = new Guid(Request.QueryString["basetable"]);
            createDrillDown(user.AccountID, basetable, user.EmployeeID, reportid, user.CurrentSubAccountId);
        }
    }

    private void createDrillDown(int accountid, Guid basetable, int employeeid, Guid reportid, int subAccountID)
    {
        
        

        cReports clsreports = new cReports((int)ViewState["accountid"],subAccountID);
        ArrayList items = clsreports.CreateDrillDownList(basetable, employeeid, subAccountID);
        
        object[] values;

        for (int i = 0; i < items.Count; i++)
        {
            values = (object[])items[i];

            optlist.Items.Add(new ListItem((string)values[1], values[0].ToString()));
        }

        if (optlist.Items.FindByValue(reportid.ToString()) != null)
        {
            optlist.Items.FindByValue(reportid.ToString()).Selected = true;
        }
    }
}
