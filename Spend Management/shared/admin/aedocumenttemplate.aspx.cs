namespace Spend_Management
{
    using System;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Text;
    using System.Globalization;

    using AjaxControlToolkit;
    using SpendManagementLibrary;

    using Spend_Management.shared.code;

    using DocumentMergeProject = Spend_Management.shared.code.DocumentMergeProject;

    public partial class aedocumenttemplate : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                CurrentUser curUser = cMisc.GetCurrentUser();

                RedirectIfNoMergeProjects(curUser);

                cmdOK.Attributes.Add("onclick", "if (validateform('vgMain') == false) { return false; }");

                this.PopulateMergeProjects(curUser);

                if (Request.QueryString["docid"] != null)
                {
                    Master.title = "Edit Document Template";
                    lblUpload.Text = "Replace existing file";
                    lblUpload.CssClass = "";
                    reqUpload.Enabled = false;
                    curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.DocumentTemplates, true, true);
                    hiddenDocId.Value = Request.QueryString["docid"];
                    this.GetDocInfo(int.Parse(hiddenDocId.Value));
                }
                else
                {
                    Master.title = "New Document Template";
                    Master.PageSubTitle = "Document Template Details";
                    lblUpload.Text = "File to upload*";
                    lblUpload.CssClass = "mandatory";
                    reqUpload.Enabled = true;
                    curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.DocumentTemplates, true, true);   
                    hiddenDocId.Value = "0";
                    cmbMergeProject.SelectedIndex = 0;
                }
                Master.enablenavigation = false;
            }
        }

        private void RedirectIfNoMergeProjects(CurrentUser user)
        {
            var dmps = new DocumentMergeProject(user);
            if (dmps.Projects.Count == 0)
                Response.Redirect("~/shared/admin/admindocmergeprojects.aspx?nomerge=true");           
        }

        private void PopulateMergeProjects(CurrentUser user)
        {
            var dmps = new DocumentMergeProject(user);
            var projects = (from x in dmps.Projects.Values
                            orderby x.MergeProjectName
                            select new { id = x.MergeProjectId.ToString(), description = x.MergeProjectName }).ToList();
            projects.Insert(0, new { id = "", description = "[None]" });
            cmbMergeProject.DataSource = projects;
            cmbMergeProject.DataTextField = "description";
            cmbMergeProject.DataValueField = "id";
            cmbMergeProject.DataBind();
        }

        protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/shared/admin/admindoctemplates.aspx", true);
        }

        protected void cmdOK_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            var templates = new DocumentTemplate(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID);
            int templateId;

            if (!fuDocTemplate.HasFile && int.Parse(hiddenDocId.Value) == 0)
            {
                templateId = -2;
            }
            else
            {
                // check that doc template is a Word doc, as no compatibility for anthing else at present
                if (!fuDocTemplate.FileName.ToUpper().EndsWith(".DOC") && !fuDocTemplate.FileName.ToUpper().EndsWith(".DOCX") && int.Parse(hiddenDocId.Value) == 0)
                {
                    templateId = -4;
                }
                else
                {
                    string uploadFile = fuDocTemplate.PostedFile.FileName;
                    string contentType = "application/msword"; //fuDocTemplate.PostedFile.ContentType.ToString();
                    if (fuDocTemplate.FileName.ToUpper().EndsWith(".DOCX"))
                    {
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    }
                    string[] path = uploadFile.Split('\\');
                    string directory = "";
                    string filename = path[path.Length - 1];
                    for (int x = 0; x < path.Length - 1; x++)
                    {
                        directory += path[x] + "\\";
                    }
                    // temp hardcoded.. need a combo box
                    int nMergeProjectId = Convert.ToInt32(cmbMergeProject.SelectedValue);
                    if (nMergeProjectId != 0)
                    {
                        var newTemplate = new cDocumentTemplate(int.Parse(hiddenDocId.Value), txtDocName.Text, directory, filename, txtDocDesc.Text, contentType, DateTime.Now, curUser.EmployeeID, DateTime.Now, curUser.EmployeeID, nMergeProjectId);
                        templateId = templates.StoreDocument(newTemplate, fuDocTemplate.FileBytes);
                    }
                    else
                    {
                        templateId = -3;
                    }
                }
            }
            if(templateId < 0)
            {               
                var pnlModal = new Panel { ID = "pnlModal", CssClass = "errorModal" };
                pnlModal.Style.Add(HtmlTextWriterStyle.Padding, "4px");
                pnlMessageHolder.Controls.Add(pnlModal);
                var litModal = new Literal { ID = "litModal" };
                pnlModal.Controls.Add(litModal);
                var hlModal = new HyperLink();
                hlModal.Style.Add(HtmlTextWriterStyle.Display, "none");
                hlModal.ID = "hlModal";
                pnlMessageHolder.Controls.Add(hlModal);
                var mpe = new ModalPopupExtender
                          {
                              ID = "mpeMessage",
                              TargetControlID = hlModal.ID,
                              PopupControlID = pnlModal.ID,
                              BackgroundCssClass = "modalMasterBackground"
                          };
                mpe.Show();
                pnlMessageHolder.Controls.Add(mpe);
                var sb = new StringBuilder();
                switch (templateId)
                {
                    case -1:
                        sb.Append("<p>Cannot save record as a template of that name already exists.</p>");
                        break;
                    case -2:
                        sb.Append("<p>No template file selected for upload.</p>");
                        break;
                    case -3:
                        sb.Append("<p>Please select a valid document configuration project.</p>");
                        break;
                    case -4:
                        sb.Append("<p>Invalid template type. Only MS Word documents permitted (DOC / DOCX)</p>");
                        break;
                    default:
                        sb.Append("<p>Unknown error occurred attempting to add template</p>");
                        break;
                }
                sb.Append("<p><img src=\"/shared/images/buttons/btn_close.png\" title=\"Close\" onclick=\"$find('" + mpe.ClientID + "').hide();\"></p>");
                litModal.Text = sb.ToString();
            }
            else
            {
                Response.Redirect("~/shared/admin/admindoctemplates.aspx", true);
            }
        }

        private void GetDocInfo(int docId)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            var templates = new DocumentTemplate(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID);
            if (docId > 0)
            {
                cDocumentTemplate curTemplate = templates.getTemplateById(docId);

                if (curTemplate != null)
                {
                    txtDocDesc.Text = curTemplate.DocumentDescription;
                    txtDocName.Text = curTemplate.DocumentName;
                    cmbMergeProject.Items.FindByValue(curTemplate.MergeProjectId.ToString(CultureInfo.InvariantCulture)).Selected = true;
                    reqUpload.Enabled = false;
                    litMessage.Text = "<img src=\"/shared/images/icons/warning.png\" />&nbsp;Selection of a new file will overwrite the existing stored document.";
                    Master.PageSubTitle = cMisc.DotonateString(curTemplate.DocumentName, 50);
                }
            }
            else
            {
                txtDocDesc.Text = "";
                txtDocName.Text = "";
                Master.PageSubTitle = "Document Template Details";
            }
        }
    }
}
