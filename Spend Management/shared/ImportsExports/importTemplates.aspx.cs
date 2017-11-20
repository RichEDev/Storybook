using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;

namespace Spend_Management
{
    public partial class importTemplates : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportTemplates, true, true);

            Title = "Import Templates";
            Master.PageSubTitle = Title;
            this.Master.helpid = 1122;

            if (!IsPostBack)
            {
                if (currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ImportTemplates, true))
                {
                    HyperLink lnkAddMapping = new HyperLink();
                    lnkAddMapping.NavigateUrl = "aeImportTemplate.aspx";
                    lnkAddMapping.Text = "Add Import Template Mapping";
                    lnkAddMapping.CssClass = "submenuitem";
                    pnlLinks.Controls.Add(lnkAddMapping);
                }

                const string sSQL = "SELECT importTemplates.templateID, importTemplates.templateName, importTemplates.applicationType, importTemplates.isAutomated, esrTrusts.trustName, esrTrusts.trustVPD FROM dbo.importTemplates"; //, createdOn, createdBy, modifiedOn, modifiedBy

                cGridNew grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "importTemplates", sSQL);
                grid.KeyField = "templateID";
                grid.enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ImportTemplates, true);
                grid.enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ImportTemplates, true);
                grid.deletelink = "javascript:DeleteImportTemplate({templateID});";
                grid.editlink = "aeImportTemplate.aspx?templateid={templateID}";
                grid.EmptyText = "There are no Import Templates to display";

                grid.getColumnByName("templateID").hidden = true;

                ((cFieldColumn)grid.getColumnByName("applicationType")).addValueListItem((int)ApplicationType.ESROutboundImport, "ESR Outbound V1.0");
                ((cFieldColumn)grid.getColumnByName("applicationType")).addValueListItem((int)ApplicationType.ExcelImport, "Excel Import");
                ((cFieldColumn)grid.getColumnByName("applicationType")).addValueListItem((int)ApplicationType.EsrOutboundImportV2, "ESR Outbound V2.0");

                Literal lit = new Literal();
                string[] gridData = grid.generateGrid();
                lit.Text = gridData[1];

                pnlGrid.Controls.Add(lit);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "TemplateGridVars", cGridNew.generateJS_init("TemplateGridVars", new List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
            }
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
