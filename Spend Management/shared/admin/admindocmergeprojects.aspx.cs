using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using Syncfusion.DocIO.DLS;
using System.Text;

namespace Spend_Management
{
    using Spend_Management.shared.code;

    public partial class admindocmergeprojects : System.Web.UI.Page
    {
        //CurrentUser curUser = cMisc.GetCurrentUser();
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            Master.PageSubTitle = "Document Configuration";
            Title = Master.PageSubTitle;

            if (!this.IsPostBack)
            {
                if (Request["nomerge"] == "true")
                {
                    const string Message = "At least one document configuration is required before configuring document templates.";
                    divMessage.InnerHtml = "<div id='divMessage' runat='server' class=\"errorModalSubject\">" + "Automatic Page Redirection Message" + "</div><br /><div class=\"errorModalBody\">" + Message + "</div>";
                    mdlMessage.Show();                    
                }
                lnkAddProject.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.DocumentConfigurations, false);
                string[] gridData = CreateGrid();
                divConfigurations.InnerHtml = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "EmpsGridVars", cGridNew.generateJS_init("EmpsGridVars", new List<string>() { gridData[0] }, curUser.CurrentActiveModule), true);
            }
        }

        protected void lnkAddProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/shared/admin/admindocmerge.aspx", true);
        }

        private string[] CreateGrid()
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentConfigurations, false, true);

            var projects = new DocumentMergeProject(curUser);

            var clsgrid = new cGridNew(curUser.AccountID, curUser.EmployeeID, "docmergeprojects", projects.DocMergeTemplatesGridSql);
            clsgrid.getColumnByName("mergeprojectid").hidden = true;
            clsgrid.getColumnByName("project_name").hidden = false;
            clsgrid.getColumnByName("createddate").hidden = true;
            clsgrid.getColumnByName("createdby").hidden = true;
            clsgrid.KeyField = "mergeprojectid";
            clsgrid.EmptyText = "There are currently no document configurations defined.";
            clsgrid.enabledeleting = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentConfigurations, false);
            clsgrid.deletelink = "javascript:SEL.DocMerge.DeleteProject({mergeprojectid});";
            clsgrid.enableupdating = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.DocumentConfigurations, false);
            clsgrid.editlink = "admindocmerge.aspx?mpid={mergeprojectid}";
            clsgrid.CssClass = "datatbl";
            clsgrid.PagerPosition = PagerPosition.TopAndBottom;
            clsgrid.showfooters = false;
            clsgrid.showheaders = true;
            clsgrid.enabledeleting = curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentConfigurations, false);
            clsgrid.enableupdating = curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.DocumentConfigurations, false);
            clsgrid.SortedColumn = clsgrid.getColumnByName("project_name");
            return clsgrid.generateGrid();
        }

        /// <summary>
        /// Close button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCloseClick(object sender, ImageClickEventArgs e)
        {
            Response.Redirect((SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url, true);
        }

    }
}
