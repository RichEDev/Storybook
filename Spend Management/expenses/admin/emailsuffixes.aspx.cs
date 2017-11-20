using System;
using System.Web;
using Spend_Management;
using SpendManagementLibrary;

public partial class admin_emailsuffixes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            Title = "E-mail Suffixes";
            Master.title = Title;
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmailSuffixes, true, true);
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;

            cEmailSuffixes clssuffixes = new cEmailSuffixes(user.AccountID);

            if (Request.QueryString["action"] != null)
            {
                Spend_Management.Action action = (Spend_Management.Action)byte.Parse(Request.QueryString["action"]);
                if (action == Spend_Management.Action.Delete)
                {
                    clssuffixes.deleteSuffix(int.Parse(Request.Form["suffixid"]));
                }
            }
            
            cGrid clsgrid = new cGrid(user.AccountID, clssuffixes.getGrid(), true, false, Grid.EmailSuffixes);
            cGridColumn newcol;
            cGridRow reqrow;
            newcol = new cGridColumn("Delete", "<img alt=\"Delete\" src=\"../../shared/images/icons/delete2_blue.gif\">", "S", "", false, true);
            clsgrid.gridcolumns.Insert(0, newcol);
            newcol = new cGridColumn("Edit", "<img alt=\"Edit\" src=\"../../shared/images/icons/edit_blue.gif\">", "S", "", false, true);
            clsgrid.gridcolumns.Insert(0, newcol);

            clsgrid.getColumn("suffixid").hidden = true;
            clsgrid.getColumn("suffix").description = "E-mail Suffix";
            clsgrid.tblclass = "datatbl";
            clsgrid.tableid = "suffixes";
            clsgrid.idcolumn = clsgrid.getColumn("suffixid");
            clsgrid.getData();
            for (int i = 0; i < clsgrid.gridrows.Count; i++)
            {
                reqrow = (cGridRow)clsgrid.gridrows[i];
                reqrow.getCellByName("Edit").thevalue = "<a href=\"aeemailsuffix.aspx?action=2&suffixid=" + reqrow.getCellByName("suffixid").thevalue + "\"><img alt=\"Edit\" src=\"../../shared/images/icons/edit.gif\"></a>";
                reqrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteSuffix(" + reqrow.getCellByName("suffixid").thevalue + ");\"><img alt=\"Delete\" src=\"../../shared/images/icons/delete2.gif\"></a>";
            }
            litgrid.Text = clsgrid.CreateGrid();
        }
    }

    /// <summary>
    /// Close button event function
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

        Response.Redirect(sPreviousURL, true);
    }
}
