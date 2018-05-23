using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Text;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    public partial class userdefinedFieldGroupings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Title = "Userdefined Groupings";
            Master.title = Title;

            if (!IsPostBack)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserdefinedGroupings, true, true);

                switch (user.CurrentActiveModule)
                {
                    case Modules.Contracts:
                        Master.helpid = 1147;
                        break;
                    default:
                        Master.helpid = 1021;
                        break;
                }

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                if (!user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.UserdefinedGroupings, true, false))
                {
                    lnkAddUDFGroup.Visible = false;
                }

                string[] gridData = createGrid();
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "udfGroupingsGridVars", cGridNew.generateJS_init("udfGroupingsGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            }
        }

        private string[] createGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cUserdefinedFieldGroupings clsgroupings = new cUserdefinedFieldGroupings((int)ViewState["accountid"]);
            cGridNew newgrid = new cGridNew((int)ViewState["accountid"], (int)ViewState["employeeid"], "gridGroupings", clsgroupings.GetGrid());
            //if (cmbfilter.SelectedValue != "1")
            //{
            //    if (cmbfilter.SelectedValue == "2")
            //    {
            //        newgrid.addFilter(newgrid.getColumnByName("archived").field, ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.None);
            //    }
            //    else
            //    {
            //        newgrid.addFilter(newgrid.getColumnByName("archived").field, ConditionType.Equals, new object[] { 2 }, null, ConditionJoiner.None);
            //    }
            //}
            //newgrid.enablearchiving = true;
            newgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.UserdefinedGroupings, true);
            newgrid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.UserdefinedGroupings, true); 
            newgrid.editlink = "aeuserdefinedgrouping.aspx?userdefinedgroupid={userdefinedGroupID}";
            newgrid.deletelink = "javascript:deleteUserdefinedGrouping({userdefinedGroupID});";
            //newgrid.archivelink = "javascript:changeArchiveStatus({departmentid});";
            //newgrid.ArchiveField = "archived";
            newgrid.getColumnByName("userdefinedGroupID").hidden = true;
            newgrid.EmptyText = "No Groupings Defined";
            newgrid.CssClass = "datatbl";
            //newgrid.getColumnByName("archived").hidden = true;
            newgrid.KeyField = "userdefinedGroupID";
            return newgrid.generateGrid();
        }

        [WebMethod(EnableSession = true)]
        public static void deleteUserdefinedGrouping(int accountid, int employeeid, int userdefinedgroupid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.UserdefinedGroupings, true))
            {
                cUserdefinedFieldGroupings groupings = new cUserdefinedFieldGroupings(accountid);
                groupings.DeleteGrouping(userdefinedgroupid, employeeid);
            }
        }

        protected void btnCancel_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.Contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=tailoring", true);
                    break;
                default:
                    Response.Redirect("~/tailoringmenu.aspx");
                    break;
            }
        }
    }
}
