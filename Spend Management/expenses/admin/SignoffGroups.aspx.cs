using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementLibrary;
using Spend_Management;

namespace Spend_Management
{
    /// <summary>
	/// Summary description for admingroups.
	/// </summary>
	public partial class admingroups : Page
	{
        /// <summary>
        /// Page load method
        /// </summary>
        /// <param name="sender"><see cref="object"/></param>
        /// <param name="e"><see cref="EventArgs"/></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			Title = "Signoff Groups";
            Master.title = Title;
			if (IsPostBack == false)
			{
                var user = cMisc.GetCurrentUser();
                this.Label1.Visible = user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.SignOffGroups, true, true);
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.SignOffGroups, true, true);
                var gridData = this.CreateGrid(user);
                litSignOffGroups.Text = gridData[1];
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SignOffGroupsVars", cGridNew.generateJS_init("SignOffGroupsVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
			}
		}

        /// <summary>
        /// Create cGridNew
        /// </summary>
        /// <param name="user">Current user</param>
        /// <returns></returns>
        public string[] CreateGrid(CurrentUser user)
        {
            var grid = new cGridNew(user.AccountID, user.EmployeeID, "SignoffGroupsGrid", cGroups.GetGridSQL())
            {
                KeyField = "groupid",
                enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.SignOffGroups, true),
                deletelink = "javascript:SEL.SignoffGroups.SignoffGroup.Delete({groupid});",
                enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SignOffGroups, true),
                editlink = "aeSignoffGroup.aspx?groupid={groupid}"
            };

            grid.getColumnByName("groupname").HeaderText = "Name";
            grid.getColumnByName("description").HeaderText = "Description";
            grid.getColumnByIndex(0).hidden = true;
            return grid.generateGrid();
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
