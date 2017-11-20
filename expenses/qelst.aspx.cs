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

using expenses.Old_App_Code;
using Spend_Management;
using SpendManagementLibrary;

namespace expenses
{
	/// <summary>
	/// Summary description for qelst.
	/// </summary>
	public partial class qelst : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Quick Entry Forms";
            Master.title = Title;

			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

                if (reqAccount.QuickEntryFormsEnabled == false)
                {
                    Response.Redirect("home.aspx?", true);
                }
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
            base.OnInitComplete(e);
            gridforms.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridforms_InitializeDataSource);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

        //protected override void OnInit(EventArgs e)
        //{
        //    base.OnInitComplete(e);
        //    gridforms.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridforms_InitializeDataSource);
        //}

        void gridforms_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            cQeForms clsforms = new cQeForms((int)ViewState["accountid"]);

            gridforms.DataSource = clsforms.getGrid();
        }
        protected void gridforms_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.FromKey("quickentryid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("name").Header.Caption = "Name";
            e.Layout.Bands[0].Columns.FromKey("description").Header.Caption = "Description";
            
        }

        protected void gridforms_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells.FromKey("name").Value = "<a href=\"qeform.aspx?quickentryid=" + e.Row.Cells.FromKey("quickentryid").Value + "\">" + e.Row.Cells.FromKey("name").Value + "</a>";
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
