namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Web.Services;
    using System.Web.UI;

    using SpendManagementLibrary;

	/// <summary>
	/// Summary description for adminmileage.
	/// </summary>
	public partial class adminmileage : Page
	{
	
		cMileagecats clsmileagecats;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";
			
			Title = "Vehicle Journey Rate Categories";
            Master.title = Title;
            Master.helpid = 1011;

			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleJourneyRateCategories, true, true);

                string[] gridData = createGrid();
                litGrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "mileageGridVars", cGridNew.generateJS_init("mileageGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
			}
		}

        public string[] createGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("903908FC-14B7-4DC2-BF21-32FB993D3A4E"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("fbb05d2e-4fd0-4135-ac51-c547ab9d3e8f"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("0d57533c-9723-4923-87c7-bf8d58a97046"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("7de68cb6-082f-4ec1-a378-75157bd3cb73"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("319df682-0bda-4c64-93ec-cb86109d8e3b"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("479d4914-48b9-4663-86c4-30f569c7ff81"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("18214da3-675f-47f4-b37c-906f9d76fe80"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("1ee53ae2-2cdf-41b4-9081-1789adf03459"))));
            columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("8ec9e07f-633e-46d1-b211-2a6258470560"))));

            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridMileage", clstables.GetTableByID(new Guid("5a83aeaf-86c8-48fb-aa2b-e7ab05a74a0b")), columns);
            clsgrid.getColumnByName("mileageid").hidden = true;
            clsgrid.KeyField = "mileageid";
            clsgrid.getColumnByName("catvalidcomment").hidden = true;
            clsgrid.enableupdating = true;
            clsgrid.editlink = "aemileage.aspx?mileageid={mileageid}";
            clsgrid.deletelink = "javascript:SEL.VehicleJourneyRate.Delete({mileageid});";
            ((cFieldColumn)clsgrid.getColumnByName("thresholdtype")).addValueListItem(0, "Annual");
            ((cFieldColumn)clsgrid.getColumnByName("thresholdtype")).addValueListItem(1, "Journey");
            ((cFieldColumn)clsgrid.getColumnByName("unit")).addValueListItem(0, "Mile");
            ((cFieldColumn)clsgrid.getColumnByName("unit")).addValueListItem(1, "KM");

            ((cFieldColumn)clsgrid.getColumnByID(new Guid("903908FC-14B7-4DC2-BF21-32FB993D3A4E"))).addValueListItem(0, "<input type=\"checkbox\" disabled=\"disabled\" />");
            ((cFieldColumn)clsgrid.getColumnByID(new Guid("903908FC-14B7-4DC2-BF21-32FB993D3A4E"))).addValueListItem(1, "<input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\" />");

            //clsgrid.addEventColumn("../../shared/images/icons/warning.png", "javascript:displayCategoryWarning('{catvalidcomment}', {mileageid});","","");
            clsgrid.enabledeleting = true;
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

	}
}
