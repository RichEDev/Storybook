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
using System.Web.Services;
using Spend_Management;

namespace expenses.information
{
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Holidays;

    /// <summary>
	/// Summary description for holidays.
	/// </summary>
	public partial class holidays : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			Title = "Holidays";
            Master.title = Title;
            Master.helpid = 1166;
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
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
            gridholidays.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridholidays_InitializeDataSource);
        }

        void gridholidays_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID));
            Holidays holidays = new Holidays(connection);
            gridholidays.DataSource = holidays.GetHolidayDataSet(((int)ViewState["employeeid"]));
        }
        protected void gridholidays_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.FromKey("holidayid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("startdate").Header.Caption = "Start Date";
            e.Layout.Bands[0].Columns.FromKey("enddate").Header.Caption = "End Date";
            e.Layout.Bands[0].Columns.FromKey("startdate").Format = "dd/MM/yyyy";
            e.Layout.Bands[0].Columns.FromKey("enddate").Format = "dd/MM/yyyy";
            if (e.Layout.Bands[0].Columns.FromKey("edit") == null)
            {
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("delete").Width = Unit.Pixel(15);
                e.Layout.Bands[0].Columns.Insert(0, new Infragistics.WebUI.UltraWebGrid.UltraGridColumn("edit", "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">", Infragistics.WebUI.UltraWebGrid.ColumnType.HyperLink, ""));
                e.Layout.Bands[0].Columns.FromKey("edit").Width = Unit.Pixel(15);
            }
        }

        protected void gridholidays_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells.FromKey("edit").Value = "<a href=\"aeholiday.aspx?action=2&holidayid=" + e.Row.Cells.FromKey("holidayid").Value + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a>";
            e.Row.Cells.FromKey("delete").Value = "<a href=\"javascript:deleteHoliday(" + e.Row.Cells.FromKey("holidayid").Value + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a>";
        }

        [WebMethod(EnableSession = true)]
        public static void deleteHoliday (int accountid, int holidayid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID));
            Holidays holidays = new Holidays(connection);
            holidays.DeleteHoliday(holidayid);
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
