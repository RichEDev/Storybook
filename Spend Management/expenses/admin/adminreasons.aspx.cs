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
	/// <summary>
	/// Summary description for adminreasons.
	/// </summary>
	public partial class adminreasons : Page
	{
		
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
            cReasons clsreasons =new cReasons(user.AccountID);
            cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridReasons", clsreasons.getGrid());
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
        public static int deleteReason(int accountid, int reasonid)
        {
            cReasons clsreasons = new cReasons(accountid);
            return clsreasons.deleteReason(reasonid);
        }

        /// <summary>
        /// Update status of the reason
        /// </summary>
        /// <param name="accountId">
        /// current account ID
        /// </param>
        /// <param name="reasonId">
        /// Reason ID which needs to be updated
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public static int ChangeStatus(int accountId, int reasonId)
        {
            cReasons clsreasons = new cReasons(accountId);
            cReason reason = clsreasons.getReasonById(reasonId);
            return clsreasons.ChangeStatus(reasonId, !reason.Archive);
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
