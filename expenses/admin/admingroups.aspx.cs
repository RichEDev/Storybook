using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
    /// <summary>
	/// Summary description for admingroups.
	/// </summary>
	public partial class admingroups : Page
	{
		cGroups clsgroups;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";

			Title = "Signoff Groups";
            Master.title = Title;
            Master.helpid = 1033;
			if (IsPostBack == false)
			{
                var user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
			    var grid = new cGridNew(user.AccountID, user.EmployeeID, "groupsGrid", cGroups.GetGridSQL())
			    {
			        KeyField = "groupid",
			        enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.SignOffGroups, true),
			        deletelink = "javascript:deleteGroup({groupid});",
			        enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SignOffGroups, true),
			        editlink = "aegroup.aspx?action=2&groupid={groupid}"
			    };

			    grid.getColumnByIndex(0).hidden = true;
			    var gridData = grid.generateGrid();
                litSignOffGroups.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SignOffGroupsVars", cGridNew.generateJS_init("SignOffGroupsVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
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
        public static int DeleteGroup(int accountId, int groupId)
        {
            cGroups groups = new cGroups(accountId);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            return groups.DeleteGroup(groupId, currentUser);
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
