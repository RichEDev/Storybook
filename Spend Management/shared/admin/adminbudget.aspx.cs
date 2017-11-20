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
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementLibrary;
using Spend_Management;
using System.Collections.Generic;

namespace Spend_Management
{
	/// <summary>
	/// Summary description for adminbudget.
	/// </summary>
	public partial class adminbudget : Page
	{
	
		
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			
			Title = "Budget Holders";
            Master.title = Title;
            Master.helpid = 1030;

			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.BudgetHolders, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                generateGrid();
			}
		}

        private void generateGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsTables = new cTables(user.AccountID);
            var clsFields = new cFields(user.AccountID);
            
            var columns = new List<cNewGridColumn>();
            cTable baseTable = clsTables.GetTableByName("budgetholders");
            var budgetHolderId = clsFields.GetFieldByID(new Guid("4876EA25-6E1B-499F-B8F3-2979969F2855"));
            var budgetHolder = clsFields.GetFieldByID(new Guid("28E88047-145A-45E5-AFEB-3F18A4683BA5"));
            var description = clsFields.GetFieldByID(new Guid("7A470F7D-D081-4D64-B663-2A0AC2A7C221"));
            var employeeFullName = clsFields.GetFieldByID(new Guid("B5DD3E91-DA63-4E9B-B933-FEC64831D920"));
            columns.Add(new cFieldColumn(budgetHolderId));
            columns.Add(new cFieldColumn(budgetHolder));
            columns.Add(new cFieldColumn(description));
            columns.Add(new cFieldColumn(employeeFullName));

            var clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridBudgetHolders", baseTable, columns);
               
            clsgrid.getColumnByName("budgetholderid").hidden = true;
            clsgrid.KeyField = "budgetholderid";
            clsgrid.enabledeleting = true;
            clsgrid.editlink = "aebudget.aspx?budgetholderid={budgetholderid}";
            clsgrid.deletelink = "javascript:SEL.BudgetHolders.deleteBudgetHolder({budgetholderid});";
            clsgrid.enableupdating = true;
            clsgrid.getColumnByIndex(3).HeaderText = "Employee Responsible"; // employee full name function
            clsgrid.EmptyText = "There are no budget holders to display.";

            string[] gridData = clsgrid.generateGrid();
            litgrid.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "budgetHolderGridVars", cGridNew.generateJS_init("budgetHolderGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);

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
