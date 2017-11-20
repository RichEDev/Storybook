using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Text;

namespace Spend_Management
{
    using Spend_Management.shared.code;

    public partial class aeDocTemplatesAssociation : System.Web.UI.Page
    {
        public int nDocID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            int nDocID = 0;
            if (ViewState["docid"] == null)
            {
                nDocID = Convert.ToInt32(Request["docid"]);
                ViewState["docid"] = nDocID;
            }

            CurrentUser user = cMisc.GetCurrentUser();
            DocumentTemplate cTemplates = new DocumentTemplate(user.AccountID, user.CurrentSubAccountId, user.EmployeeID);
            string[] gridData = null;

            // set the sel.grid javascript variables
            List<string> jsBlockObjects = new List<string>();

            if (!this.IsPostBack)
            {
                Master.title = "New Template Association";
                if (nDocID > 0)
                {
                    cDocumentTemplate template = cTemplates.getTemplateById(nDocID);
                    Master.PageSubTitle = cMisc.DotonateString(template.DocumentName, 50);
                }
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, true, true);
                bool bAllowAccess = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentTemplates, true, false);

                gridData = cTemplates.GetMergeAssociationGrid(user, nDocID, bAllowAccess);

                litGrid.Text = gridData[2];
                jsBlockObjects.Add(gridData[1]);

                litGrid.Text = gridData[2];
                
                cmdok.Attributes.Add("onclick", "if (ensureSelectionMade('gMergeAssociations') == false) { return false; }");
            }

            string[] availAssocGridData = cTemplates.GetAvailableAssociationsGrid(nDocID, user);
            ltGridAssociationChoices.Text = availAssocGridData[2];

            jsBlockObjects.Add(availAssocGridData[1]);
            
            Page.ClientScript.RegisterStartupScript(this.GetType(), "docTempAssocGridVars", cGridNew.generateJS_init("docTempAssocGridVars", jsBlockObjects, user.CurrentActiveModule), true);            
        }

        protected void btnClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("admindoctemplates.aspx");
        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            DocumentTemplate cTemplates = new DocumentTemplate(user.AccountID, user.CurrentSubAccountId, user.EmployeeID);
            List<string> lstMessages = new List<string>();

            int nDocID = (int)ViewState["docid"];
            string[] recordIds = Request.Form["selectgMergeAssociations"].Split(',');

            lstMessages = cTemplates.SaveNewAssociations(nDocID, recordIds);
            lblMessage.Text = createMessageHtml(lstMessages);
            if (lblMessage.Text != "")
            {
                mdlMessage.Show();
            }
            string[] assocChoiceGridData = cTemplates.GetAvailableAssociationsGrid(nDocID, user);
            ltGridAssociationChoices.Text = assocChoiceGridData[2];
            string[] mergeAssocGridData = cTemplates.GetMergeAssociationGrid(user, nDocID, user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentTemplates, true, false));
            litGrid.Text = mergeAssocGridData[2];

            // set the sel.grid javascript variables
            List<string> jsBlockObjects = new List<string>();
            jsBlockObjects.Add(assocChoiceGridData[1]);
            jsBlockObjects.Add(mergeAssocGridData[1]);

            Page.ClientScript.RegisterStartupScript(this.GetType(), "docTempAssocGridVars", cGridNew.generateJS_init("docTempAssocGridVars", jsBlockObjects, user.CurrentActiveModule), true);
            pnlAddAssocs.Attributes["display"] = "none";
        }

        private string createMessageHtml(List<string> messages)
        {
            StringBuilder sb = new StringBuilder();
            int nSuccess = 0;
            int nDuplicate = 0;
            bool bError = false;

            nSuccess = messages.Count(x => x == "success");
            nDuplicate = messages.Count(x => x.Contains("duplicate"));
            bError = messages.Count(x => x.Contains("error")) > 0;
            if (messages.Count == 0)
            {
                bError = true;
            }
            if (nDuplicate > 0)
            {
                sb.Append("<p>");
                sb.Append(nDuplicate);
                sb.Append(" were not added as they are already associated</p>");
            }
            if (bError)
                sb.Append("<p>There has been an error adding the associations. Please contact your administrator.</p>");

            return sb.ToString();
        }
    }
}
