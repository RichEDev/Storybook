using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
    /// <summary>
    /// Summary description for adminqeforms.
    /// </summary>
    public partial class adminqeforms : Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.Expires = 60;
            Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
            Response.AddHeader("pragma", "no-cache");
            Response.AddHeader("cache-control", "private");
            Response.CacheControl = "no-cache";

            Title = "Quick Entry Form Design";
            Master.title = Title;
            Master.helpid = 1045;
            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.QuickEntryForms, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;


                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

                if (reqAccount.QuickEntryFormsEnabled == false)
                {
                    Response.Redirect("../home.aspx?", true);
                }

                cQeForms clsforms = new cQeForms(user.AccountID);

                if (Request.QueryString["callback"] != null)
                {
                    if (Request.QueryString["action"] == "3")
                    {
                        clsforms.deleteForm(int.Parse(Request.Form["quickentryid"]));
                        Response.End();
                    }
                }
                int i;
                cGridColumn newcol;
                cGridRow reqrow;
                cGrid clsgrid = new cGrid(clsforms.getGrid(), true, false, Grid.QuickEntryForms, "name");
                clsgrid.getColumn("quickentryid").hidden = true;
                clsgrid.getColumn("name").description = "Name";
                clsgrid.getColumn("description").description = "Description";
                newcol = new cGridColumn("Delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", "S", "", false, true);
                clsgrid.gridcolumns.Insert(0, newcol);
                newcol = new cGridColumn("Edit", "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">", "S", "", false, true);
                clsgrid.gridcolumns.Insert(0, newcol);
                clsgrid.tblclass = "datatbl";
                clsgrid.tableid = "quickentry";
                clsgrid.idcolumn = clsgrid.getColumn("quickentryid");
                clsgrid.getData();
                for (i = 0; i < clsgrid.gridrows.Count; i++)
                {
                    reqrow = (cGridRow)clsgrid.gridrows[i];
                    reqrow.getCellByName("Edit").thevalue = "<a href=\"aeqeform.aspx?action=2&quickentryid=" + reqrow.getCellByName("quickentryid").thevalue + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
                    reqrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteQeForm(" + reqrow.getCellByName("quickentryid").thevalue + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
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

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }

        #endregion
    }
}