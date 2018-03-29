namespace Spend_Management.shared.admin
{
    using System;
    using System.Web;
    using System.Web.Services;
    using System.Web.UI;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers.EnumDescription;
    using SpendManagementLibrary.Helpers.Response;

    /// <summary>
	/// Summary description for admincostcodes.
	/// </summary>
	public partial class admincostcodes : Page
	{		

        [WebMethod(EnableSession=true)]
		public static string[] CreateGrid(string FilterVal)
		{
            CurrentUser user = cMisc.GetCurrentUser();
            cCostcodes clscostcodes = new cCostcodes(user.AccountID);
            cGridNew newgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridCostcodes", clscostcodes.GridSql);
            
            if (FilterVal != "2")
            {
                int val = int.Parse(FilterVal);
                newgrid.addFilter(((cFieldColumn)newgrid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { val }, null, ConditionJoiner.None);
            }
            bool allowEdit = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.CostCodes, true);
            newgrid.enablearchiving = allowEdit;
            newgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.CostCodes, true);
            newgrid.enableupdating = allowEdit;
            newgrid.editlink = "aecostcode.aspx?costcodeid={costcodeid}";
            newgrid.deletelink = "javascript:deleteCostcode({costcodeid});";
            newgrid.archivelink = "javascript:changeArchiveStatus({costcodeid});";
            newgrid.ArchiveField = "archived";
            newgrid.getColumnByName("costcodeid").hidden = true;
            newgrid.getColumnByName("archived").hidden = true;
            newgrid.KeyField = "costcodeid";
            return newgrid.generateGrid();
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			Title = "Cost Codes";
            Master.title = Title;
            Master.helpid = 1014;
			

			if (IsPostBack == false)
			{
                
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CostCodes, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                string[] gridData = CreateGrid(cmbfilter.SelectedValue);
                litgrid.Text = gridData[1].ToString();
                this.lnkNewCostCode.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.CostCodes, true);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "CostCodeGridVars", cGridNew.generateJS_init("CostCodeGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);
                
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

        [WebMethod(EnableSession = true)]
        public static int deleteCostcode(int accountid, int costcodeid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCostcodes clscostcodes = new cCostcodes(accountid);
            return clscostcodes.DeleteCostCode(costcodeid, user.EmployeeID);
        }

        /// <summary>
        /// Changes a cost code status from archived to unarchived an vice versa.
        /// </summary>
        /// <param name="accountId">
        /// The accountId.
        /// </param>
        /// <param name="costCodeId">
        /// The Id of the cost code to change the status of..
        /// </param>
        /// <returns>
        /// A <see cref="OutcomeResponse"/> with the result of the action.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public static OutcomeResponse changeStatus(int accountId, int costCodeId)
        {
            var response = new OutcomeResponse();

            cCostcodes costCodes = new cCostcodes(accountId);
            cCostCode costCode = costCodes.GetCostcodeById(costCodeId);

            var outcome = costCodes.ChangeStatus(costCodeId, !costCode.Archived);

            if (outcome == ChangeCostCodeArchiveStatus.Success)
            {
                response.Success = true;
                return response;
            }
            
            response.Success = false;
            response.Message = outcome.GetDescription();

            return response;
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
