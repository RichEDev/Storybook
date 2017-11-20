using System.Web.UI.WebControls;
using SpendManagementHelpers.TreeControl;
using SpendManagementLibrary.DocumentMerge;

namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using Spend_Management.shared.code;
    using SpendManagementLibrary;
    
    using AjaxControlToolkit;
    using System.Web.Services;

    /// <summary>
    /// Allows administration of the document merge configuration.
    /// </summary>
    public partial class AdminDocMerge : Page
    {
        #region properties

        private const string ErrorMessage = "An error has been encountered. If the problem persists, please contact your administrator";

        /// <summary>
        /// The current project id.
        /// </summary>
        public static int CurrentProjectId { get; private set; }

        #endregion

        #region page events
        /// <summary>
        /// Main Page Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            mdlRepSource.RepositionMode = ModalPopupRepositionMode.RepositionOnWindowResizeAndScroll;

            if (this.IsPostBack)
            {
                return;
            }

            CurrentUser user = cMisc.GetCurrentUser();
            

            // populate the available documents - not used but currently checks the cache (needs removing)
            var templates = new DocumentTemplate(user.AccountID, user.CurrentSubAccountId, user.EmployeeID);
            int mergeProjectId;

            if (int.TryParse(this.Request.QueryString["mpid"], out mergeProjectId))
            {
                user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.DocumentConfigurations, false, true);
                var documentMergeProject = new DocumentMergeProject(user);
                TorchProject project = documentMergeProject.GetProjectById(mergeProjectId);

                if (project != null)
                {
                    this.txtProjectName.Text = project.MergeProjectName;
                    this.txtProjectDescription.Text = project.MergeProjectDescription;
                    this.txtBlankText.Text = project.BlankTableText;
                    this.ViewState["docMergeProjectId"] = mergeProjectId;
                    this.ViewState["docMergeProjectname"] = project.MergeProjectName;
                    this.hdnDefaultDocumentGroupingId.Value = project.DefaultDocumentGroupingConfigId != null ? Convert.ToString(project.DefaultDocumentGroupingConfigId) : "0";               
                }
            }
            else
            {
                user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.DocumentConfigurations, false, true);
                this.ViewState["docMergeProjectId"] = 0;
                this.ViewState["docMergeProjectname"] = "New Configuration";
                this.hdnDefaultDocumentGroupingId.Value = "0";

            }

            CurrentProjectId = (int)this.ViewState["docMergeProjectId"];

            hdnReportSortingName.Value = "0";

            string title = "Document Configuration";
            if (CurrentProjectId > 0)
            {
                title = "Edit " + title;
                this.Master.title = title;
            }
            else
            {
                title = "New " + title;
                this.Master.title = title;
            }

            this.Master.enablenavigation = false;
            this.Master.ShowSubMenus = true;
            this.Master.PageSubTitle = cMisc.DotonateString(this.ViewState["docMergeProjectname"].ToString(), 50);

            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "currentProjectId", "currentProjectId = " + Convert.ToInt32(this.ViewState["docMergeProjectId"].ToString()) + ";", true);
            var jsObjects = new List<string>();
            var mergeService = new svcDocumentMerge();
            object[] info = mergeService.ProcessGridRequest(Convert.ToInt32(this.ViewState["docMergeProjectId"].ToString()), "reports");
            this.divReportSources.InnerHtml = info[2].ToString();
            jsObjects.Add(info[1].ToString());
            this.SetTabs();
            this.configurationTabContainer.ActiveTabIndex = 0;
            var panel = new Panel();
            var filterRuntime = new CheckBox();

            TreeControls.CreateCriteriaModalPopup(filteringTab.Controls, GlobalVariables.StaticContentLibrary, ref panel, ref filterRuntime, domain: "SEL.DocMerge", filterValidationGroup: "vgFilter");
            SMgrProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.timepicker-0.3.2.js"));
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "SourceGridVars", cGridNew.generateJS_init("SourceGridVars", jsObjects, user.CurrentActiveModule), true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "filterDomIDS", "$(document).ready(function () {" + TreeControls.GeneratePanelDomIDsForFilterModal(true, false, panel, null, null, filterRuntime, "SEL.DocMerge") + "});", true);     
        }

        #endregion

        #region user events
        /// <summary>
        /// Save the document configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSaveClick(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (this.SaveConfiguration())
                    Response.Redirect("~/shared/admin/admindocmergeprojects.aspx", true);
            }
            catch (Exception)
            {
                ShowMessageModal(ErrorMessage, "Save Document Configuration");
            }
        }

        /// <summary>
        /// Cancel the document configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCancelClick(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.Redirect("~/shared/admin/admindocmergeprojects.aspx", true);
            }
            catch (Exception)
            {
                ShowMessageModal(ErrorMessage, "Save Document Configuration");
            }
        }

        #endregion

        #region methods/functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="heading"></param>
        private void ShowMessageModal(string message, string heading)
        {
            HtmlContainerControl divValidator = this.divValidation;
            divValidator.InnerHtml = string.Format("<div id='divValidation' runat='server' class=\"errorModalSubject\">{0}</div><br /><div class=\"errorModalBody\">{1}</div>", heading, message);
            lblValidation.Text = "whatever";
            mdlValidation.Show();
        }

        /// <summary>
        /// Saves the current configuration
        /// </summary>
        /// <returns>True or False as a success flag</returns>
        private bool SaveConfiguration()
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            var mergeProjects = new DocumentMergeProject(curUser);

            // new project
            var newProject = new TorchProject(CurrentProjectId, this.txtProjectName.Text, this.txtProjectDescription.Text, DateTime.Now, curUser.EmployeeID, DateTime.Now, curUser.EmployeeID, Convert.ToInt32(hdnDefaultDocumentGroupingId.Value), curUser.AccountID, txtBlankText.Text);

            int newId = mergeProjects.UpdateProject(newProject, curUser.EmployeeID);

            switch (newId)
            {
                case -1:
                    this.ShowMessageModal("A document configuration of this name already exists.", "Page Validation Failed");
                    return false;
                case 0:
                    this.ShowMessageModal("Failed to add document configuration to the database. Please contact your administrator", "Document Configuration Error");
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Update the merge project id.
        /// </summary>
        /// <param name="newId"></param>
        [WebMethod(EnableSession = true)]
        public static void UpdateMergeProjectId(int newId)
        {
            CurrentProjectId = newId;
        }

        /// <summary>
        /// Set the correct tabs on the page.
        /// </summary>
        private void SetTabs()
        {
            this.reportSourcesTab.Enabled = true;
            this.groupingTab.Enabled = true;
        }

        #endregion
    }
}
