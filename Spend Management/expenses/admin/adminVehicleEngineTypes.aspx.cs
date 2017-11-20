namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.UI;

	/// <summary>
	/// Summary description for adminVehicleEngineTypes.
	/// </summary>
	public partial class adminVehicleEngineTypes : Page
	{
		
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

        protected string RedirectUrl
        {
            get
            {
                return SiteMap.CurrentNode == null ? "/categorymenu.aspx" : SiteMap.CurrentNode.ParentNode.Url;
            }
        }

		protected void Page_Load(object sender, EventArgs e)
		{
		    var user = cMisc.GetCurrentUser();

	        user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.VehicleEngineType, true, true);

            this.Populate(user);
		}

	    private void Populate(ICurrentUser user)
	    {
	        this.Title = "Vehicle Engine Types";
	        Master.title = this.Title;

	        this.lblNew.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.VehicleEngineType, true);

	        var gridData = adminVehicleEngineTypes.CreateGrid(user);
	        this.litGrid.Text = gridData[1];

	        // set the sel.grid javascript variables
	        Page.ClientScript.RegisterStartupScript(this.GetType(), "VetsGridVars", cGridNew.generateJS_init("VetsGridVars", new List<string> { gridData[0] }, user.CurrentActiveModule), true);
	    }

	    private static string[] CreateGrid(ICurrentUser user)
	    {
	        var vetsGrid = new cGridNew(user.AccountID, user.EmployeeID, "gridVets", "select VehicleEngineTypeId, Name, Code from VehicleEngineTypes")
            {
                EmptyText = "There are currently no Vehicle engine types to display.",
                enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.VehicleEngineType, true),
                editlink = "aeVehicleEngineType.aspx?VehicleEngineTypeId={VehicleEngineTypeId}",
                enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.VehicleEngineType, true),
                KeyField = "VehicleEngineTypeId",
                deletelink = "javascript: SEL.Vet.Delete({VehicleEngineTypeId});"
            };
	        vetsGrid.getColumnByName("VehicleEngineTypeId").hidden = true;
	        return vetsGrid.generateGrid();
	    }

	}
}
