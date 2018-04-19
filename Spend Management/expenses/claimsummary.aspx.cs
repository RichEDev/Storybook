namespace Spend_Management
{
    using System;
    using System.Web;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;

	/// <summary>
	/// Summary description for claimsummary.
	/// </summary>
	public partial class claimsummary : Page
	{
	    /// <summary>
	    /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IGeneralOptions"/>
	    /// </summary>
	    [Dependency]
	    public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        protected void Page_Load(object sender, EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			if (IsPostBack == false)
			{
				var claimType = ClaimStage.Current;
                
				int claimId;

				if (Request["claimtype"] != null)
				{
					claimType = (ClaimStage)byte.Parse(Request["claimtype"]);
				}
                
                CurrentUser user = cMisc.GetCurrentUser();

			    var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithClaim();

                var claims = new cClaims(user.AccountID);

				if (generalOptions.Claim.SingleClaim == false && claimType == ClaimStage.Current) 
				{
					litoptions.Text += "<a href=\"../aeclaim.aspx\" class=\"submenuitem\">New Claim</a>";
				}

				switch (claimType)
				{
					case ClaimStage.Current: 
                        switch (claims.getCount(user.EmployeeID, ClaimStage.Current)) 
						{
							case 0:
								Response.Redirect("../aeclaim.aspx",true);
								break;
							case 1:
								if (generalOptions.Claim.SingleClaim)
								{
                                    claimId = claims.getDefaultClaim(ClaimStage.Current, user.EmployeeID);
                                    Response.Redirect("claimViewer.aspx?claimid=" + claimId, true);
								}
								break;
						}
						break;
					case ClaimStage.Submitted: //view submitted
                        if (claims.getCount(user.EmployeeID, ClaimStage.Submitted) == 1) 
                        {
                            claimId = claims.getDefaultClaim(ClaimStage.Submitted, user.EmployeeID);
                            Response.Redirect("claimViewer.aspx?claimid=" + claimId, true);
                        }
						break;
					case ClaimStage.Previous: //view submitted
                        if (claims.getCount(user.EmployeeID, ClaimStage.Previous) == 1) 
                        {
                            claimId = claims.getDefaultClaim(ClaimStage.Previous, user.EmployeeID);
                            Response.Redirect("claimViewer.aspx?claimid=" + claimId, true);
                        }
						break;
				}

                string[] gridData = claims.GetClaimSummaryGrid(user.EmployeeID, claimType);
                litGrid.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ClaimGridVars", cGridNew.generateJS_init("ClaimGridVars", new System.Collections.Generic.List<string> { gridData[0] }, user.CurrentActiveModule), true);

                switch (claimType)
                {
                    case ClaimStage.Current:
                        Title = "Current Claims";
                        break;
                    case ClaimStage.Previous:
                        Title = "Previous Claims";
                        break;
                    case ClaimStage.Submitted:
                        Title = "Submitted Claims";
                        break;
                }

                Master.title = Title;
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

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, ImageClickEventArgs e)
        {
            string previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(previousUrl, true);
        }
	}
}
