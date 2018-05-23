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
using SpendManagementLibrary;
using System.Text;


namespace Spend_Management
{
    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Reasons;

    /// <summary>
	/// Summary description for adminreasons.
	/// </summary>
	public partial class adminreasons : Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IReason"/>
        /// </summary>
        [Dependency]
	    public IDataFactoryArchivable<IReason, int, int> ReasonFactory { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			
			Title = "Reasons";
            Master.title = Title;
            Master.helpid = 1018;
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Reasons, true, true);
                string[] gridData = createGrid();
                litgrid.Text = gridData[1];
                this.lnkNewReason.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Reasons,true);
                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ReasonsGridVars", cGridNew.generateJS_init("ReasonsGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);
			}
		}

        private string[] createGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridReasons", this.GetGrid());
            bool allowEdit = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Reasons, true);
            clsgrid.EmptyText = "There are currently no Reasons to display.";
            clsgrid.archivelink = "javascript:changeArchiveStatus({reasonid});";
            clsgrid.enablearchiving = allowEdit;
            clsgrid.ArchiveField = "archived";
            clsgrid.enableupdating = allowEdit;
            clsgrid.editlink = "aereason.aspx?reasonid={reasonid}";
            clsgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Reasons, true);
            clsgrid.KeyField = "reasonid";
            clsgrid.deletelink = "javascript:deleteReason({reasonid});";
            clsgrid.getColumnByName("reasonid").hidden = true;
            clsgrid.getColumnByName("archived").hidden = true;
            return clsgrid.generateGrid();
        }

        private string GetGrid()
        {
            return "select reasonid, reason, description, archived from reasons";
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
