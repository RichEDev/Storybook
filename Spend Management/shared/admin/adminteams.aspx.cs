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
using SpendManagementLibrary;
using System.Web.Services;
using System.Text;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    /// <summary>
	/// Summary description for adminteams.
	/// </summary>
	public partial class adminteams : Page
	{	

		protected void Page_Load(object sender, System.EventArgs e)
		{
            CurrentUser currentUser = cMisc.GetCurrentUser();
			Title = "Teams";
            Master.PageSubTitle = Title;
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.Contracts:
                    Master.helpid = 1045;
                    break;
                default:
                    Master.helpid = 1035;
                    break;
            }

			if (IsPostBack == false)
			{
                currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Teams, true, true);

                Label1.Visible = currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Teams, true);

                ViewState["accountid"] = currentUser.AccountID;
                ViewState["employeeid"] = currentUser.EmployeeID;

                const string sSQL = "select teamid, teamname, description from teams";
                cGridNew gridTeams = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridTeams", sSQL);

                gridTeams.KeyField = "teamid";
                gridTeams.EmptyText = "There are no Teams defined.";
                gridTeams.getColumnByName("teamid").hidden = true;

                gridTeams.enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Teams, true);
                gridTeams.enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Teams, true);
                gridTeams.editlink = "aeteam.aspx?teamid={teamid}";
                gridTeams.deletelink = "javascript:DeleteTeam({teamid});";

                gridTeams.addEventColumn("teammembers", "/shared/images/icons/users3.png", "javascript:ShowTeamMembers({teamid});", "Members", "View Team Members");

                string[] gridData = gridTeams.generateGrid();
                litTeamsGrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "TeamsGridVars", cGridNew.generateJS_init("TeamsGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
			}
		}

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.Contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=employee", true);
                    break;
                default:
                    Response.Redirect("~/usermanagementmenu.aspx", true);
                    break;
            }
        }
	}
}
