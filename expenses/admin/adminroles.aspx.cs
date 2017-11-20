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
using ExpensesLibrary;
using System.Web.Services;
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementLibrary;

namespace expenses
{
	/// <summary>
	/// Summary description for adminroles.
	/// </summary>
	public partial class adminroles : Page
	{
		cRoles clsroles;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			
			Title = "Roles";
            Master.title = Title;
            Master.helpid = 1034;
			int action = 0;
            CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
            ViewState["accountid"] = user.accountid;
            ViewState["employeeid"] = user.employeeid;

			cEmployees clsemployees = new cEmployees(user.accountid);
			cEmployee reqemp;
			reqemp = clsemployees.GetEmployeeById(user.employeeid);
			
			

				clsroles = new  cRoles(reqemp.accountid);

                cRole role = clsroles.getRoleById(reqemp.roleid);
                cMisc clsmisc = new cMisc(reqemp.accountid);

                cGlobalProperties clsproperties = clsmisc.getGlobalProperties(reqemp.accountid);
                if ((!Master.isDelegate && role.employeeadmin == false) || (Master.isDelegate && !clsproperties.delemployeeadmin))
                {
                    Response.Redirect("../restricted.aspx?", true);
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
            gridroles.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridroles_InitializeDataSource);

        }

        void gridroles_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            clsroles = new cRoles((int)ViewState["accountid"]);
            gridroles.DataSource = clsroles.getGrid();
        }

        protected void gridroles_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            #region Sorting
            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
            cEmployee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
            cGridSort sortorder = reqemp.getGridSort(Grid.Allowances);
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
            e.Layout.Bands[0].Columns.FromKey("roleid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("rolename").Header.Caption = "Role Name";
            e.Layout.Bands[0].Columns.FromKey("description").Header.Caption = "Description";
            if (e.Layout.Bands[0].Columns.FromKey("delete") == null)
            {
             
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("delete").Width = Unit.Pixel(15);
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("edit", "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("edit").Width = Unit.Pixel(15);
            }
        }

        protected void gridroles_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells.FromKey("edit").Value = "<a href=\"aerole.aspx?action=2&roleid=" + e.Row.Cells.FromKey("roleid").Value + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
            e.Row.Cells.FromKey("delete").Value = "<a href=\"javascript:deleteRole(" + e.Row.Cells.FromKey("roleid").Value + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
        }



        [WebMethod]
        public static bool deleteRole(int accountid, int roleid)
        {
            cRoles clsroles = new cRoles(accountid);
            return clsroles.deleteRole(roleid);
        }

        protected void gridroles_SortColumn(object sender, Infragistics.WebUI.UltraWebGrid.SortColumnEventArgs e)
        {
            UltraWebGrid grid = (UltraWebGrid)sender;
            byte direction = (byte)grid.Columns[e.ColumnNo].SortIndicator;
            cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
            clsmisc.saveDefaultSort(Grid.Roles, grid.Columns[e.ColumnNo].Key, direction);
        }

	}
}
