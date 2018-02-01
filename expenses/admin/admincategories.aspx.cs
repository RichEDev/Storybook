namespace expenses
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Services;
    using System.Web.UI;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
	/// Summary description for admincategories.
	/// </summary>
	public partial class admincategories : Page
	{

		cCategories clscategory;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			Title = "Expense Categories";
            Master.title = Title;
            Master.helpid = 1022;
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseCategories, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

			    string[] gridData = createGrid(user.AccountID, user.EmployeeID);
			    litgrid.Text = gridData[1];

			    // set the sel.grid javascript variables
			    Page.ClientScript.RegisterStartupScript(this.GetType(), "CatsGridVars", cGridNew.generateJS_init("CatsGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            }
		}

	    private string[] createGrid(int accountid, int employeeid)
	    {
	        cTables clstables = new cTables(accountid);
	        cFields clsfields = new cFields(accountid);
	        List<cNewGridColumn> columns = new List<cNewGridColumn>();
	        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("6ECCC7C6-C6DA-44D1-AABA-A2E9CFF48918"))));
	        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("44C2A53A-DB33-45AF-AF66-D0055AAD48EC"))));
	        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("0744BEC1-5C4D-4B5E-90CF-8D3D9F187DBF"))));
	        cGridNew clsgrid = new cGridNew(accountid, employeeid, "gridCats", clstables.GetTableByID(new Guid("75C247C2-457E-4B14-BBEC-1391CD77FB9E")), columns);
	        clsgrid.getColumnByName("categoryid").hidden = true;
	        clsgrid.KeyField = "categoryid";
	        clsgrid.enabledeleting = true;
	        clsgrid.deletelink = "javascript:deleteCategory({categoryid});";
	        clsgrid.enableupdating = true;
	        clsgrid.editlink = "aecategory.aspx?action=2&categoryid={categoryid}";
	        return clsgrid.generateGrid();
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

        [WebMethod(EnableSession = true)]
        public static int deleteCategory(int accountid, int categoryid)
        {
            cCategories clscategory = new cCategories(accountid);
            return clscategory.deleteCategory(categoryid);
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
