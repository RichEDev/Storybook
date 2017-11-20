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

public partial class information_delegates : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            this.Master.UseDynamicCSS = true;
            Title = "Delegates";
            Master.title = Title;
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;

            cEmployees clsemployees = new cEmployees(user.AccountID);

            if (Request.QueryString["action"] != null)
            {
                if (Request.QueryString["action"] == "3")
                {
                    clsemployees.removeProxy(user.EmployeeID, int.Parse(Request.Form["employeeid"]));
                }
            }

            cGrid clsproxgrid = new cGrid(user.AccountID, clsemployees.getProxies(user.EmployeeID), true, false, Grid.Delegates);
            clsproxgrid.tblclass = "datatbl";
            clsproxgrid.emptytext = "There are no delegates currently assigned";
            clsproxgrid.getColumn("username").description = "Username";
            clsproxgrid.getColumn("empname").description = "Employee Name";
            clsproxgrid.getColumn("employeeid").hidden = true;

            cGridColumn newcol;
            cGridRow gridrow;
            newcol = new cGridColumn("Delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", "S", "", false, true);
            clsproxgrid.gridcolumns.Insert(0, newcol);
            clsproxgrid.idcolumn = clsproxgrid.getColumn("employeeid");
            clsproxgrid.tableid = "delegates";
            clsproxgrid.getData();

            for (int i = 0; i < clsproxgrid.gridrows.Count; i++)
            {
                gridrow = (cGridRow)clsproxgrid.gridrows[i];
                gridrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteDelegate(" + gridrow.getCellByName("employeeid").thevalue + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
            }
            litgrid.Text = clsproxgrid.CreateGrid();
        }
    }

    protected void cmdClose_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/mydetailsmenu.aspx", true);
    }
}
