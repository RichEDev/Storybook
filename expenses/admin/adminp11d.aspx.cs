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

using expenses.admin;
using System.Web.Services;
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
    using SpendManagementLibrary.Employees;

	/// <summary>
	/// Summary description for adminp11d.
	/// </summary>
	public partial class adminp11d : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			
			Title = "P11D Categories";
            Master.title = Title;
            Master.helpid = 1016;
			
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.P11D, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
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

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
            gridp11d.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridp11d_InitializeDataSource);
        }

        void gridp11d_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            cP11dcats clspdcats = new cP11dcats((int)ViewState["accountid"]);
            gridp11d.DataSource = clspdcats.getGrid();
        }
        protected void gridp11d_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            #region Sorting
            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
            Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
            cGridSort sortorder = reqemp.GetGridSortOrders().GetBy(Grid.Allowances);
            if (sortorder != null)
            {
                if (e.Layout.Bands[0].SortedColumns.Count == 0)
                {
                    if (e.Layout.Bands[0].Columns.FromKey(sortorder.columnname) != null)
                    {
                        e.Layout.Bands[0].Columns.FromKey(sortorder.columnname).SortIndicator = (SortIndicator)sortorder.sortorder;
                        e.Layout.Bands[0].SortedColumns.Add(sortorder.columnname);
                    }
                }
            }
            #endregion
            e.Layout.Bands[0].Columns.FromKey("pdcatid").Hidden = true;
            
            e.Layout.Bands[0].Columns.FromKey("pdname").Header.Caption = "P11D Category";
            if (e.Layout.Bands[0].Columns.FromKey("edit") == null)
            {
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("delete").Width = Unit.Pixel(25);
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("edit", "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("edit").Width = Unit.Pixel(25);
            }
        }

        protected void gridp11d_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells.FromKey("edit").Value = "<a href=\"aep11d.aspx?action=2&pdcatid=" + e.Row.Cells.FromKey("pdcatid").Value + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
            e.Row.Cells.FromKey("delete").Value = "<a href=\"javascript:deleteP11d(" + e.Row.Cells.FromKey("pdcatid").Value + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
        }


        [WebMethod(EnableSession = true)]
        public static int deleteP11d(int accountid, int pdcatid)
        {
            cP11dcats clsp11d = new cP11dcats(accountid);
            return clsp11d.deleteP11dCat(pdcatid);
        }

        protected void gridp11d_SortColumn(object sender, Infragistics.WebUI.UltraWebGrid.SortColumnEventArgs e)
        {
            UltraWebGrid grid = (UltraWebGrid)sender;
            byte direction = (byte)grid.Columns[e.ColumnNo].SortIndicator;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.Employee.GetGridSortOrders().Add(Grid.P11dCategories, grid.Columns[e.ColumnNo].Key, direction);
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
}
