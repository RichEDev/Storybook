using System;
using System.Web;
using System.Web.UI;
namespace Spend_Management
{
    using BusinessLogic;

    using SEL.FeatureFlags;

    /// <summary>
	/// Summary description for rptlist.
	/// </summary>
	public partial class rptlist : Page
	{
	    /// <summary>
	    /// Gets or sets an instance of <see cref="IFeatureFlagManager"/>.
	    /// </summary>
	    [Dependency]
        public IFeatureFlagManager FeatureFlagManager { get; set; }

	    /// <summary>
	    /// Gets or sets a value indicating whether to use the new style report viewer "View.aspx".
	    /// </summary>
	    public bool NewStyle { get; set; }

		protected void Page_Load(object sender, System.EventArgs e)
		{
		    this.NewStyle = this.FeatureFlagManager?.IsEnabled("Syncfusion report viewer") ?? true;

			Title = "Reports";
            Master.title = "Reports";

            if (IsPostBack == false)
            {
                int action;
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                        Master.helpid = 1027;
                        break;
                    default:
                        Master.helpid = 1115;
                        break;
                }

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, false) == true)
                {
                    litoptions.Text = "<a href=\"aereport.aspx\" class=\"submenuitem\">Create Report</a> <a href=\"folders.aspx\" class=\"submenuitem\">Report Categories</a>";
                }
                litoptions.Text += "<a href=\"myschedules.aspx\" class=\"submenuitem\">My Schedules</a>";

                cReports clsreports = new cReports(user.AccountID, user.CurrentSubAccountId);
                ViewState["reports"] = user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, true);
                ViewState["reportsreadonly"] = user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ClaimantReports, true);

                if (Request.QueryString["action"] != null)
                {
                    action = int.Parse(Request.QueryString["action"]);
                    if (action == 3)
                    {
                        
                        clsreports.deleteReport(new Guid(Request.QueryString["reportid"].ToString()));
                    }
                }

                bool claimants = false;
                if (Request.QueryString["claimants"] != null)
                {
                    claimants = true;
                }

                ViewState["claimants"] = claimants;

                if (claimants == false)
                {
                    user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reports, true, true);
                }

                var folders = new cReportFolders(user.AccountID);
                gridreports_cmbFilter.Items.AddRange(folders.CreateDropDown());

                string[] gridData = clsreports.generateGrid(user, claimants);
                litGrid.Text = gridData[1];

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ReportsGridVars", cGridNew.generateJS_init("ReportsGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);
                
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
