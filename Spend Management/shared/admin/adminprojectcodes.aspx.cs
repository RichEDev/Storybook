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
using Spend_Management;
using System.Text;


namespace Spend_Management.shared.admin
{
	/// <summary>
	/// Summary description for adminprojectcodes.
	/// </summary>
	public partial class adminprojectcodes : Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";
			
			Title = "Project Codes";
            Master.title = Title;
            Master.helpid = 1017;

			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProjectCodes, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                string[] gridData = CreateGrid(cmbfilter.SelectedValue);
                litgrid.Text = gridData[1];
                this.lnkNewProjectCode.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ProjectCodes,true);
                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ProjCodesGridVars", cGridNew.generateJS_init("ProjCodesGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, user.CurrentActiveModule), true);
			}
		}

        [WebMethod(EnableSession=true)]
        public static string[] CreateGrid(string FilterVal)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cGridNew newgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridProjectcodes", "select projectcodeid, projectcode, description, archived from project_codes");

            if (FilterVal != "2")
            {
                int val = int.Parse(FilterVal);
                newgrid.addFilter(((cFieldColumn)newgrid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { val }, null, ConditionJoiner.None);
            }
            bool allowEdit = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ProjectCodes, true);
            newgrid.enablearchiving = allowEdit;
            newgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ProjectCodes, true);
            newgrid.enableupdating = allowEdit;
            newgrid.editlink = "aeprojectcode.aspx?projectcodeid={projectcodeid}";
            newgrid.deletelink = "javascript:deleteProjectcode({projectcodeid});";
            newgrid.archivelink = "javascript:changeArchiveStatus({projectcodeid});";
            newgrid.ArchiveField = "archived";
            newgrid.getColumnByName("projectcodeid").hidden = true;
            newgrid.getColumnByName("archived").hidden = true;
            newgrid.KeyField = "projectcodeid";
            newgrid.EmptyText = "There are no project codes to display";
            return newgrid.generateGrid();
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
