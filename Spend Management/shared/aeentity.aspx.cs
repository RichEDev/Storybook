namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using AjaxControlToolkit;

    using SpendManagementHelpers;

    using SpendManagementLibrary;

    using shared.code;

    using Syncfusion.DocIO.DLS;

    using System.IO;

    using shared.usercontrols;

    using NewRelic.Api.Agent;

    using SpendManagementHelpers.TreeControl;

    using SpendManagementLibrary.Helpers;

    using Spend_Management.shared.code.Helpers;

    using Utilities.StringManipulation;

    using DocumentMergeProject = shared.code.DocumentMergeProject;

    /// <summary>
    /// Add / Edit Custom Entity Record screen
    /// </summary>
    public partial class aeentity : System.Web.UI.Page
    {
        protected Dictionary<int, List<string>> otmTableID;
        protected Dictionary<int, List<string>> summaryTableID;
        protected List<string> scriptCmdStrings;

        public string rtEditorCntl;
        #region properties
        public ModalPopupExtender SaveErrorMessageModal
        {
            get { return mdlSaveErrorModal; }
        }

        /// <summary>
        /// Gets or sets whether we are using internet explorer with a version of 9 or less
        /// </summary>
        public bool Ie9OrLess { get; set; }

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            var ajaxFileUpload = this.editor.AjaxFileUpload;
            ajaxFileUpload.AllowedFileTypes = "jpg,jpeg,png,gif,bitmap";
            ajaxFileUpload.MaximumNumberOfFiles = 1;

            scrmgrProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-ui.datepicker-en-gb.js"));
            scrmgrProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.timepicker-0.3.2.js"));
            scrmgrProxy.Scripts.Add(new ScriptReference("~/shared/javascript/minify/sel.image-browser.js"));
            ScriptManager.RegisterStartupScript(this, this.GetType(), "disableBottomToolbar", "Sys.Application.add_load(function(){setTimeout(disableBottomToolbar,0); });", true); //  setTimeout(setPasteShortcut,0);
            if (this.Request.Browser.Browser == "IE" && this.Request.UserAgent != null && (this.Request.UserAgent.Contains("MSIE 10") || this.Request.UserAgent.Contains("MSIE 9") || this.Request.UserAgent.Contains("MSIE 8") || this.Request.UserAgent.Contains("MSIE 7")))
            {
                this.pluginBrowseButton.Visible = false;
            }
        }

        /// <summary>
        /// Default Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
            Response.CacheControl = "no-cache";

            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;
            ViewState["subaccountid"] = user.CurrentSubAccountId;
            cCustomEntities clsentities = new cCustomEntities(user);
            bool isSameScreen = false;
            Master.UseDynamicCSS = true;

            //skips over load events if posted back from the HTML extender AjaxFileUploader
            if (Request["start"] == null && Request["done"] == null && Request["complete"] == null && (Request.AcceptTypes[0] != "*/*" || Request.Form.Count > 0))
            {
                int formSelectionListValue = -1;
                string formSelectionTextValue = null;
                int formSelectionFormId = -1;

                if (!IsPostBack)
                {
                    int entityid;
                    if (!Int32.TryParse(Request.QueryString["entityid"], out entityid))
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }
                    ViewState["entityid"] = entityid;

                    int formid = 0;
                    int id = 0;
                    int viewid = 0;
                    int tabid = 0;
                    if (Request.QueryString["formid"] != null)
                    {
                        if (!Int32.TryParse(Request.QueryString["formid"], out formid))
                        {
                            Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                        }
                    }
                    ViewState["formid"] = formid;
                    if (Request.QueryString["id"] != null)
                    {
                        if (!Int32.TryParse(Request.QueryString["id"], out id))
                        {
                            Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                        }

                        ViewState["ceID"] = id;
                    }
                    ViewState["id"] = id;
                    if (Request.QueryString["viewid"] != null)
                    {
                        if (!Int32.TryParse(Request.QueryString["viewid"], out viewid))
                        {
                            Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                        }
                    }
                    ViewState["viewid"] = viewid;
                    if (Request.QueryString["tabid"] != null)
                    {
                        if (!Int32.TryParse(Request.QueryString["tabid"], out tabid))
                        {
                            Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                        }
                    }
                    ViewState["tabid"] = tabid;

                    var currentEntity = clsentities.getEntityById((int)ViewState["entityid"]);
                    var entityform = currentEntity.getFormById(formid);
                    var currentView = currentEntity.getViewById(viewid);

                    ////Check if the user has access to view the current record
                    if (Session["fromWorkflow"] == null)
                    {
                        if (id == 0 && !user.CheckAccessRole(AccessRoleType.Add, CustomEntityElementType.View, entityid, viewid, false))
                        {
                            this.Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
                        }

                        if (!clsentities.IsTheDataAccessibleToTheUser(currentView, currentEntity, id) && id != 0)
                        {
                            this.Response.Redirect(ErrorHandlerWeb.InsufficientAccess, true);
                        }
                    }
                    else if (Session["fromWorkflow"] != null && Session["fromWorkflowRequest"] != null)
                    {
                        Session.Remove("fromWorkflow");
                        Session.Remove("fromWorkflowRequest");
                    }
                    else
                    {
                        Session["fromWorkflowRequest"] = true;
                    }

                    if ((int)ViewState["tabid"] == 0)
                    {
                        if (currentEntity == null)
                        {
                            Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                        }
                        cCustomEntityForm tmpForm = currentEntity.getFormById((int)ViewState["formid"]);
                        if (tmpForm == null)
                        {
                            Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                        }

                        tabid = SetTabIdToDefault(tmpForm);
                    }

                    #region formselectionattribute
                    
                    if (formid > 0 && viewid > 0)
                    {
                        formSelectionFormId = formid;

                        if (formSelectionFormId > 0)
                        {
                            if (Request.QueryString["attributeid"] != null)
                            {
                                var attributeId = -1;
                                int.TryParse(Request.QueryString["attributeid"], out attributeId);
                                formSelectionListValue = attributeId;
                            }

                            if (Request.QueryString["attributetext"] != null)
                            {
                                formSelectionTextValue = Request.QueryString["attributetext"];
                            }
                        }

                        if (formSelectionFormId != -1)
                        {
                            ViewState["isFormSelection" + entityid] = true;
                            ViewState["FormSelectionForm" + entityid] = formSelectionFormId;
                            ViewState["FormSelectionView" + entityid] = viewid;
                            if (formSelectionListValue != -1)
                            {
                                ViewState["FormSelectionValue" + entityid] = formSelectionListValue;
                            }
                            else if (formSelectionTextValue != null)
                            {
                                ViewState["FormSelectionValue" + entityid] = formSelectionTextValue;
                            }
                        }
                    }
                    else if (ViewState["isFormSelection" + entityid] is bool && (bool)ViewState["isFormSelection" + entityid]
                        && ViewState["FormSelectionView" + entityid] != null && (ViewState["FormSelectionView" + entityid] as int? ?? -1) == viewid)
                    {
                        formSelectionFormId = ViewState["FormSelectionForm" + entityid] as int? ?? -1;
                    }

                    #endregion formselectionattribute

                    if (viewid > 0)
                    {
                        if (currentEntity.Views.ContainsKey(viewid) && Request["fromWorkflow"] == null)
                        {
                            var view = currentEntity.Views[viewid];
                            if (formSelectionFormId == -1)
                            {
                                formid = id > 0 ? view.DefaultEditForm.formid : view.DefaultAddForm.formid;
                            }
                            else
                            {
                                formid = formSelectionFormId;
                            }

                            if (formid != (int)ViewState["formid"])
                            {
                                ViewState["formid"] = formid;
                                entityform = currentEntity.getFormById(formid);
                                tabid = SetTabIdToDefault(entityform);
                            }
                        }
                        else
                        {
                            if (Request.QueryString["relviewid"] != null)
                            {
                                Int32.TryParse(Request.QueryString["relviewid"], out viewid);
                            }

                            ViewState["viewid"] = viewid;
                        }
                    }


                    if (entityform == null || entityform.fields.Count == 0)
                    {
                        return;
                    }

                    if (formSelectionFormId <= 0 && clsentities.CheckEmployeeHasAccessToForm(currentEntity, entityform, id) == false)
                    {
                        Response.Redirect("~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.", true);
                    }

                    var crumbs = new List<sEntityBreadCrumb>();

                    if (!string.IsNullOrEmpty(Request.QueryString["relentityid"]))
                    {
                        string[] arrRelentityid = Request.QueryString["relentityid"].Split(',');
                        string[] arrRelformid = Request.QueryString["relformid"].Split(',');
                        string[] arrRelrecordid = Request.QueryString["relrecordid"].Split(',');
                        string[] arrReltabid = Request.QueryString["reltabid"].Split(',');
                        string[] arrRelviewid = Request.QueryString["relviewid"].Split(',');

                        for (int i = 0; i < arrRelentityid.GetLength(0); i++)
                        {
                            // may have launched from a different tab, but on the same form, so don't place both in the crumbs collection
                            var entityId = 0;
                            var formId = 0;
                            var recordId = 0;
                            var tabId = 0;
                            var viewId = 0;
                            int.TryParse(arrRelentityid[i], out entityId);
                            int.TryParse(arrRelformid[i], out formId);
                            int.TryParse(arrRelrecordid[i], out recordId);
                            int.TryParse(arrReltabid[i], out tabId);
                            int.TryParse(arrRelviewid[i], out viewId);
                            var crumb = new sEntityBreadCrumb(entityId, formId, recordId, tabId, viewId);
                            crumbs.Add(crumb);
                        }
                    }

                    if (entityform.ShowBreadCrumbs == true)
                    {
                        if (Request.QueryString["fromsummary"] != null && crumbs.Count > 0)
                        {
                            sEntityBreadCrumb topCrumb = crumbs[0];
                            int curEntityId = entityid;
                            int curRecordId = id;

                            crumbs = new List<sEntityBreadCrumb>();

                            int loopCheck = 0;
                            bool complete = false;
                            while (!complete && loopCheck < 20)
                            {
                                sEntityBreadCrumb newCrumb = clsentities.getParentBreadcrumb(curEntityId, topCrumb, curRecordId);
                                if (newCrumb.EntityID != topCrumb.EntityID && newCrumb.EntityID != 0)
                                {
                                    crumbs.Insert(0, newCrumb);
                                    curEntityId = newCrumb.EntityID;
                                    curRecordId = newCrumb.RecordID;
                                }
                                else
                                {
                                    complete = true;
                                }
                                loopCheck++;
                            }
                            crumbs.Insert(0, new sEntityBreadCrumb(topCrumb.EntityID, topCrumb.FormID, topCrumb.RecordID, topCrumb.TabID, topCrumb.ViewID));
                        }
                        crumbs.Add(new sEntityBreadCrumb(entityid, formid, id, tabid, viewid));


                        ViewState["entitycrumbs"] = crumbs;
                    }
                    else
                    {
                        crumbs.Add(new sEntityBreadCrumb(entityid, formid, id, tabid, viewid));
                        ViewState["entitycrumbs"] = crumbs;
                    }

                    if (entityform.ShowSubMenus == false)
                    {
                        Master.ShowSubMenus = false;
                    }

                    //write array to page
                    StringBuilder clientarray = new StringBuilder();
                    clientarray.Append("var formFields = []\n");
                    foreach (cCustomEntityFormField field in entityform.fields.Values)
                    {
                        clientarray.Append("formFields.push([" + field.attribute.attributeid + ",'" + field.attribute.fieldtype + "']);\n");
                    }
                    ClientScript.RegisterClientScriptBlock(typeof(string), "formfields", clientarray.ToString(), true);
                    if (id > 0)
                    {
                        SortedList<int, object> record = clsentities.getEntityRecord(currentEntity, id, entityform);

                        if (currentEntity.EnableLocking)
                        {
                            aelocking.Active = true;
                            cAttribute auditIdentifier = currentEntity.getAuditIdentifier();
                            var recStr = string.Empty;
                            if (auditIdentifier.iskeyfield)
                            {
                                recStr = id.ToString();
                            }
                            else
                                if (record.ContainsKey(auditIdentifier.attributeid))
                            {
                                string fieldData = clsentities.formatFieldData(auditIdentifier, record[auditIdentifier.attributeid].ToString());

                                if (fieldData != string.Empty)
                                {
                                    recStr = fieldData;
                                }
                            }
                            int maxBreadCrumbStringLength = Convert.ToInt16(ConfigurationManager.AppSettings["CharacterDottingLength"]);
                            aelocking.Title = string.Format("{0} ({1})", currentEntity.entityname, cMisc.DotonateString(recStr, maxBreadCrumbStringLength));

                            aelocking.CustomEntityId = currentEntity.entityid;
                            aelocking.ItemId = id;
                            aelocking.Locked = !CustomEntityRecordLocking.LockElement(currentEntity.entityid, id, user);
                            if (aelocking.Locked)
                            {
                                var lockedBy = CustomEntityRecordLocking.IsRecordLocked(currentEntity.entityid, id, user);
                                aelocking.LockedBy =
                                    new cEmployees(user.AccountID).GetEmployeeById(lockedBy.EmployeeId).FullName;

                            }
                        }

                        ViewState["record"] = record;
                    }
                }
                else
                {
                    if (Request.Form.AllKeys.Contains("ctl00$contentmain$btnSaveAndStay"))
                    {
                        ViewState["ceID"] = ViewState["id"];
                        isSameScreen = true;
                    }
                    else if (Request.Form.AllKeys.Contains("ctl00$contentmain$btnSaveAndDuplicate") || Request.Form.AllKeys.Contains("ctl00$contentmain$btnSaveAndNew"))
                    {
                        isSameScreen = true;

                        var crumbs = (List<sEntityBreadCrumb>)ViewState["entitycrumbs"];

                        if (crumbs != null)
                        {
                            sEntityBreadCrumb crumb = crumbs.Last();
                            crumb.RecordID = 0;

                            crumbs[crumbs.Count - 1] = crumb;
                            ViewState["entitycrumbs"] = crumbs;
                        }

                    }
                }

                cCustomEntity entity = clsentities.getEntityById((int)ViewState["entityid"]);
                cCustomEntityForm curForm = entity.getFormById((int)ViewState["formid"]);

                if (isSameScreen == false)
                {
                    if (clsentities.IsUserAllowedAccessToAudienceRecord(entity, (int)ViewState["id"]) == false)
                    {
                        Response.Redirect("~/shared/restricted.aspx?reason=Audience%20access%20does%20not%20permit%20you%20to%20view%20this%20record.", true);
                    }
                }

                // Check parent entities to ensure user is not excluded by an audience on that record
                if (!string.IsNullOrEmpty(Request.QueryString["relentityid"]))
                {
                    string[] arrRelentityid = Request.QueryString["relentityid"].Split(',');
                    string[] arrRelrecordid = Request.QueryString["relrecordid"].Split(',');
                    for (int entityAudienceIdx = arrRelentityid.Length - 1; entityAudienceIdx >= 0; entityAudienceIdx--)
                    {
                        cCustomEntity relEntity = clsentities.getEntityById(int.Parse(arrRelentityid[entityAudienceIdx]));
                        if (relEntity.AudienceView != AudienceViewType.NoAudience)
                        {
                            SerializableDictionary<string, object> audienceStatus = clsentities.GetAudienceRecords(relEntity.entityid, user.EmployeeID, int.Parse(arrRelrecordid[entityAudienceIdx]));
                            if (audienceStatus.ContainsKey(int.Parse(arrRelrecordid[entityAudienceIdx]).ToString()))
                            {
                                cAudienceRecordStatus audRecStatus = (cAudienceRecordStatus)audienceStatus[int.Parse(arrRelrecordid[entityAudienceIdx]).ToString()];
                                if (audRecStatus.Status == 0 || (audRecStatus.Status > 0 && (!audRecStatus.CanView || !audRecStatus.CanEdit)))
                                {
                                    Response.Redirect("~/shared/restricted.aspx?reason=Audience%20access%20does%20not%20permit%20you%20to%20view%20this%20record.", true);
                                }
                            }
                        }
                    }
                }

                CSSButton cssBtn;

                if (curForm.ShowSaveButton && !aelocking.Locked)
                {
                    cssBtn = new CSSButton { ID = "cmdok", Text = curForm.SaveButtonText.ToLower(), CommandArgument = entity.entityid + "," + curForm.formid };
                    cssBtn.Command += this.cmdok_Click;
                    cssBtn.Attributes.Add("onclick", string.Format("javascript:  validateAndSubmit('vgCE_{0}')", entity.entityid));
                    pnlButtons.Controls.Add(cssBtn);
                }

                if (curForm.ShowSaveAndDuplicate && !aelocking.Locked)
                {
                    cssBtn = new CSSButton { ID = "btnSaveAndDuplicate", Text = curForm.SaveAndDuplicateButtonText.ToLower(), CommandArgument = entity.entityid + "," + curForm.formid + ",0" };
                    cssBtn.Command += this.btnSaveAndNew_Click;
                    cssBtn.Attributes.Add("onclick", "javascript:validateAndSubmit('vgCE_" + entity.entityid + "');");
                    pnlButtons.Controls.Add(cssBtn);
                }

                if (curForm.ShowSaveAndStayButton && !aelocking.Locked)
                {
                    cssBtn = new CSSButton { ID = "btnSaveAndStay", Text = curForm.SaveAndStayButtonText.ToLower(), CommandArgument = entity.entityid + "," + curForm.formid + ",1" };
                    cssBtn.Command += this.btnSaveAndNew_Click;
                    cssBtn.Attributes.Add("onclick", "javascript:validateAndSubmit('vgCE_" + entity.entityid + "');");
                    pnlButtons.Controls.Add(cssBtn);
                }

                if (curForm.ShowSaveAndNew && !aelocking.Locked)
                {
                    cssBtn = new CSSButton { ID = "btnSaveAndNew", Text = curForm.SaveAndNewButtonText, CommandArgument = entity.entityid + "," + curForm.formid + ",2" };
                    cssBtn.Command += this.btnSaveAndNew_Click;
                    cssBtn.Attributes.Add("onclick", "javascript:validateAndSubmit('vgCE_" + entity.entityid + "');");
                    pnlButtons.Controls.Add(cssBtn);
                }

                if (curForm.ShowCancelButton)
                {
                    cssBtn = new CSSButton { ID = "cmdcancel", Text = curForm.CancelButtonText.ToLower() };
                    cssBtn.Command += this.cmdcancel_Click;
                    pnlButtons.Controls.Add(cssBtn);
                }

                if (ViewState["ceID"] == null)
                {
                    ViewState["ceID"] = 0;
                }

                if (isSameScreen == false)
                {
                    Master.title = clsentities.GetMasterPageTitle(entity, curForm, (List<sEntityBreadCrumb>)ViewState["entitycrumbs"]);
                    Master.enablenavigation = false;
                    Master.PageSubTitle = cMisc.DotonateString(curForm.formname, 36);
                }

                bool hasEditor = false;
                otmTableID = new Dictionary<int, List<string>>();
                summaryTableID = new Dictionary<int, List<string>>();
                scriptCmdStrings = new List<string>();

                foreach (KeyValuePair<int, cCustomEntityFormField> kvp in curForm.fields)
                {
                    var curField = kvp.Value;
                    if (curField.attribute.fieldtype == FieldType.LargeText)
                    {

                        cTextAttribute txt = (cTextAttribute)curField.attribute;
                        if (txt.format == AttributeFormat.FormattedText)
                        {
                            AjaxControlToolkit.HtmlEditorExtender editor = new AjaxControlToolkit.HtmlEditorExtender();

                            editor.ClientIDMode = ClientIDMode.Static;
                            editor.ID = "rtEditor" + "_" + Guid.NewGuid();
                            editor.TargetControlID = txtHTMLEditor.ID;
                            editor.EnableSanitization = false;
                            editor.DisplaySourceTab = true;
                            rtEditorCntl = editor.ClientID;
                            hasEditor = true;
                            break;
                        }
                    }
                }

                if (!hasEditor)
                {
                    this.Page.Controls.Remove(editor);
                }

                clsentities.ImageLibraryModalPopupExtenderId = this.mdlImageLibrary.ClientID;
                var currentFormId = (int)ViewState["formid"];
                holderForm.Controls.Add(clsentities.generateForm(entity, currentFormId, (int)ViewState["viewid"], (int)ViewState["id"], (int)ViewState["tabid"], (List<sEntityBreadCrumb>)ViewState["entitycrumbs"], ref otmTableID, ref summaryTableID, ref scriptCmdStrings));

                TabContainer tabs = (TabContainer)holderForm.FindControl("tabs" + (int)ViewState["formid"]);
                tabs.OnClientActiveTabChanged = "setCETab";

                if (isSameScreen == false)
                {
                    CreateAttachmentsTab(entity, currentFormId);

                    CreateMergeConfigTab(entity, user, currentFormId);

                    CreateAudiencesTab(entity, currentFormId);

                    SetActiveTab(false);
                }

                if (!this.IsPostBack)
                {
                    if (ViewState["record"] != null)
                    {
                        this.populateRecordDetails(entity, (SortedList<int, object>)ViewState["record"], curForm);
                    }
                    else
                    {
                        this.PopulateDefaultValues(curForm, entity.FormSelectionAttributeId, formSelectionListValue, formSelectionTextValue);
                    }
                }

                if (curForm.ShowBreadCrumbs == true)
                {
                    createBreadcrumbTree((List<sEntityBreadCrumb>)ViewState["entitycrumbs"], (int)ViewState["viewid"]);
                }

                if (isSameScreen == false)
                {
                    CreateOtmGridAndAutoCompleteJavascript();
                }
            }

            var panel = new Panel();
            var filterRuntime = new CheckBox();
            TreeControls.CreateCriteriaModalPopup(filteringTab.Controls, GlobalVariables.StaticContentLibrary, ref panel, ref filterRuntime, domain: "SEL.DocMerge", filterValidationGroup: "vgFilter");
            scrmgrProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.timepicker-0.3.2.js"));
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "filterDomIDS", "$(document).ready(function () {" + TreeControls.GeneratePanelDomIDsForFilterModal(true, false, panel, null, null, filterRuntime, "SEL.DocMerge") + "});", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "GetStatusCompleteUnknownMax", String.Format("SEL.DocMerge.GetStatusCompleteUnknownMax = {0};", ConfigurationManager.AppSettings["DocMergeRetryStatusMax"]), true);
        }

        private int SetTabIdToDefault(cCustomEntityForm entityForm)
        {
            var tabid = 0;
            SortedList<byte, cCustomEntityFormTab> lstTabs = entityForm.getTabsForForm();
            if (lstTabs.Count > 0)
            {
                tabid = lstTabs.Values[0].tabid;
                ViewState["tabid"] = tabid;
            }
            return tabid;
        }


        public void SetActiveTab(bool isSameScreen)
        {
            var tabs = (TabContainer)holderForm.FindControl("tabs" + (int)ViewState["formid"]);

            if (isSameScreen)
            {
                int tmpTabIdx = 0;
                int.TryParse(hiddenCETab.Value, out tmpTabIdx);

                if (tmpTabIdx < tabs.Tabs.Count)
                {
                    switch (tabs.Tabs[tmpTabIdx].ID)
                    {
                        case "tabAttachments":
                            ViewState["tabid"] = -1;
                            break;
                        case "tabMergeConfigs":
                            ViewState["tabid"] = -2;
                            break;
                        case "tabAudiences":
                            ViewState["tabid"] = -3;
                            break;
                        default:
                            ViewState["tabid"] = int.Parse(tabs.Tabs[tmpTabIdx].ID.Replace("tab", ""));
                            break;
                    }
                }
            }

            if ((int)ViewState["tabid"] < 0)
            {
                string tabID = "";
                switch ((int)ViewState["tabid"])
                {
                    case -1:
                        tabID = "tabAttachments";
                        break;
                    case -2:
                        tabID = "tabMergeConfigs";
                        break;
                    case -3:
                        tabID = "tabAudiences";
                        break;
                    default:
                        break;
                }

                for (int tabLoop = 0; tabLoop < tabs.Tabs.Count; tabLoop++)
                {
                    if (tabs.Tabs[tabLoop].ID == tabID)
                    {
                        hiddenCETab.Value = tabLoop.ToString();
                        hiddenCETabID.Value = tabID;
                        break;
                    }
                }
            }
            else
            {
                for (int tabLoop = 0; tabLoop < tabs.Tabs.Count; tabLoop++)
                {
                    if (tabs.Tabs[tabLoop].ID == "tab" + (int)ViewState["tabid"])
                    {
                        hiddenCETab.Value = tabLoop.ToString();
                        hiddenCETabID.Value = ((int)ViewState["tabid"]).ToString();
                        break;
                    }
                }
            }

        }


        private void CreateOtmGridAndAutoCompleteJavascript()
        {
            StringBuilder sb = new StringBuilder();
            string initialTableLoad = "";

            foreach (KeyValuePair<int, List<string>> kvp in otmTableID)
            {
                List<string> tab_otmTableID = (List<string>)kvp.Value;
                int tabId = kvp.Key;

                bool hasSummary = false;
                if (summaryTableID.ContainsKey(tabId))
                {
                    if (summaryTableID[tabId].Count > 0)
                        hasSummary = true;
                }

                if (tab_otmTableID.Count > 0 || hasSummary)
                {
                    int curTabId;
                    int.TryParse(hiddenCETabID.Value, out curTabId);
                    if (tabId == curTabId) //(int)ViewState["tabid"])
                    {
                        initialTableLoad = "loadTables_tab" + tabId.ToString() + "();";
                    }
                    sb.Append("var tab" + tabId.ToString() + "loaded = false;\n");
                    sb.Append("function loadTables_tab" + tabId.ToString() + "()\n {\n"); // pageLoad()
                    sb.Append("if(tab" + tabId.ToString() + "loaded === true) { return; }\n");
                    bool hasElse = false;
              
                    for (int x = 0; x < tab_otmTableID.Count; x++)
                    {
                        if (!hasElse)
                        {
                            sb.Append(" else { ");
                            hasElse = true;
                        }

                        var clsSecure = new cSecureData();
                        var entityDetails = string.Format(tab_otmTableID[x], (int)ViewState["ceID"]);

                        string encryptedEntityDetials = clsSecure.Encrypt(entityDetails);
                        encryptedEntityDetials = "'" + encryptedEntityDetials + "'";
                        sb.Append("loadOTMTable(" + encryptedEntityDetials + ");\n");

                    }

                    if (hasSummary)
                    {
                        List<string> tab_summaryTableID = (List<string>)summaryTableID[tabId];

                        for (int x = 0; x < tab_summaryTableID.Count; x++)
                        {
                            if (!hasElse)
                            {
                                sb.Append(" else { ");
                                hasElse = true;
                            }
                            sb.Append("loadSummaryTable(" + tab_summaryTableID[x] + ");\n");
                        }
                    }
                    sb.Append("tab" + tabId.ToString() + "loaded = true;\n");


                    if (hasElse)
                        sb.Append(" }\n");

                    sb.Append("}\n");
                }
            }

            TabContainer tabs = (TabContainer)holderForm.FindControl("tabs" + (int)ViewState["formid"]);
            sb.Append("var tabContainer = '" + tabs.ClientID + "';\n");

            if (scriptCmdStrings.Count > 0)
            {
                sb.Append(AutoComplete.generateScriptRegisterBlock(scriptCmdStrings));
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "jsTableLoads", sb.ToString() + "$(document).ready(function() { " + initialTableLoad + " setTimeout(function () { setActiveCETab(); }, 0); } );", true);
        }

        private void CreateAudiencesTab(cCustomEntity entity, int currentFormId)
        {
            if (entity.AudienceView != AudienceViewType.NoAudience && (int)ViewState["ceID"] > 0 && !entity.Forms[currentFormId].HideAudiences)
            {
                var clsentities = new cCustomEntities(cMisc.GetCurrentUser());
                var currentRecordAudienceStatus = clsentities.GetAudienceRecordStatus(entity, (int)ViewState["ceID"]);
                TabPanel tabAudiences;
                var tabs = GetTabContainer(currentFormId, "tabAudiences", "Audiences", out tabAudiences);
                var audList = (audienceList)LoadControl("~/shared/usercontrols/audienceList.ascx");
                audList.baseTable = entity.AudienceTable;
                audList.entityIdentifier = entity.entityid;
                audList.parentRecordId = (int)ViewState["id"];
                audList.DeletePermission = (currentRecordAudienceStatus == null || currentRecordAudienceStatus.CanDelete);
                audList.EditPermission = (currentRecordAudienceStatus == null || currentRecordAudienceStatus.CanEdit);

                tabAudiences.Controls.Add(audList);
                tabs.Tabs.Add(tabAudiences);
            }
        }

        private void CreateAttachmentsTab(cCustomEntity entity, int currentFormId)
        {
            if (entity.EnableAttachments && !entity.Forms[currentFormId].HideAttachments)
            {
                TabPanel tabpnl;
                var tabs = GetTabContainer(currentFormId, "tabAttachments", "Attachments", out tabpnl);
                var usrAttachments = (attachmentList)LoadControl("~/shared/usercontrols/attachmentList.ascx");
                var clstables = new cTables((int)ViewState["accountid"]);
                var tblattachments = clstables.GetTableByName(entity.table.TableName + "_attachments");
                usrAttachments.IDField = tblattachments.GetPrimaryKey().FieldName;
                usrAttachments.TableName = tblattachments.TableName;
                usrAttachments.RecordID = (int)ViewState["id"];
                usrAttachments.iFrameName = "ceUploadIFrame";
                usrAttachments.MultipleAttachments = true;
                usrAttachments.WithPublishing = true;
                usrAttachments.createGrid((int)ViewState["ceID"]);
                tabpnl.Controls.Add(usrAttachments);
                tabs.Tabs.Add(tabpnl);
            }
        }

        private TabContainer GetTabContainer(int currentFormId, string controlId, string headerText, out TabPanel tabPanel)
        {
            var tabContainer = (TabContainer)holderForm.FindControl(string.Format("tabs{0}", currentFormId));

            tabPanel = new TabPanel { ID = controlId, HeaderText = headerText };

            return tabContainer;
        }

        private void CreateMergeConfigTab(cCustomEntity entity, CurrentUser user, int currentFormId)
        {
            if (entity.AllowMergeConfigAccess && (int)ViewState["id"] > 0 && !entity.Forms[currentFormId].HideTorch)
            {
                TabPanel mcpnl;
                var tabs = GetTabContainer(currentFormId, "tabMergeConfigs", "Torch", out mcpnl);

                var prjs = new DocumentMergeProject(user);
                var litProjectGrid = new Literal();
                var prjGrid = new cGridNew(user.AccountID, user.EmployeeID, "configgrid", prjs.GridSql);
                var fields = new cFields(user.AccountID);
                prjGrid.CssClass = "datatbl";
                prjGrid.getColumnByName("documentid").hidden = true;
                prjGrid.getColumnByName("mergeprojectid").hidden = true;
                prjGrid.KeyField = "documentid";
                prjGrid.addFilter(fields.GetFieldByID(new Guid("05BE3D95-4533-4E48-BEE1-45B80E75D986")), ConditionType.Equals, new object[] { (int)ViewState["id"] }, null, ConditionJoiner.None);
                prjGrid.EmptyText = "There are currently no document configurations defined.";

                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                        {
                            prjGrid.SortedColumn = prjGrid.getColumnByName("doc_name");
                        }
                        break;
                }

                prjGrid.enablearchiving = false;
                prjGrid.enabledeleting = false;
                prjGrid.enableupdating = false;
                prjGrid.enablepaging = true;
                prjGrid.pagesize = 10;
                prjGrid.PagerPosition = PagerPosition.TopAndBottom;

                prjGrid.addEventColumn("domerge", "/shared/images/icons/16/plain/documents_gear.png", "javascript:SEL.DocMerge.PerformMerge({mergeprojectid},{documentid}," + ((int)ViewState["id"]).ToString() + "," + ((int)ViewState["entityid"]).ToString() + ");", "Perform Torch", "Perform Torch using the default configuration");
                prjGrid.addEventColumn("domerge", "/shared/images/icons/16/plain/edit.png", "javascript:SEL.DocMerge.EditMerge({mergeprojectid}, {documentid});", "Edit Torch", "Edit the configurations");
                string[] gridData = prjGrid.generateGrid();
                litProjectGrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "prjDocMergeGridVars", cGridNew.generateJS_init("prjDocMergeGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);

                Literal litSectionHeader = new Literal();
                litSectionHeader.Text = "<div class=\"formpanel\"><div class=\"sectiontitle\">Available Torches</div>";

                mcpnl.Controls.Add(litSectionHeader);
                mcpnl.Controls.Add(litProjectGrid);
                litSectionHeader = new Literal();
                litSectionHeader.Text = "</div>";
                mcpnl.Controls.Add(litSectionHeader);

                // add the torch attachments control
                var templates = new DocumentTemplate(user.AccountID, user.CurrentSubAccountId, user.EmployeeID);
                int[] templateAssociationIds = templates.GetDocumentTemplateAssociationIdsByEntityRecord(entity.entityid, (int)ViewState["id"]);

                // filtering the grid can only occur if at least 1 doc template association exists, if not then don't display the section
                if (templateAssociationIds.Count() > 0)
                {
                    litSectionHeader = new Literal();
                    litSectionHeader.Text = "<div class=\"formpanel\"><div class=\"sectiontitle\">Torch History</div>";
                    mcpnl.Controls.Add(litSectionHeader);

                    var attachmentsControl = (torchGeneratedAttachmentList)LoadControl("~/shared/usercontrols/torchGeneratedAttachmentList.ascx");
                    var tables = new cTables(user.Account.accountid);
                    cTable attachmentsTable = tables.GetTableByName(entity.table.TableName + "_attachments");

                    attachmentsControl.IdField = attachmentsTable.GetPrimaryKey().FieldName;
                    attachmentsControl.TableName = attachmentsTable.TableName;
                    attachmentsControl.RecordIds = templateAssociationIds;
                    attachmentsControl.CreateGrid();
                    mcpnl.Controls.Add(attachmentsControl);

                    litSectionHeader = new Literal();
                    litSectionHeader.Text = "</div>";
                    mcpnl.Controls.Add(litSectionHeader);
                }

                tabs.Tabs.Add(mcpnl);
            }
        }


        private void populateRecordDetails(cCustomEntity entity, SortedList<int, object> record, cCustomEntityForm form)
        {
            TabContainer tabs = (TabContainer)holderForm.FindControl("tabs" + form.formid);
            TabPanel tab;
            cTextAttribute textatt;
            TextBox txtbox;
            DropDownList cmbbox;
            cDateTimeAttribute dateatt;
            DateTime date;
            decimal currency;
            int accountID = (int)ViewState["accountid"];
            string html_data;
            SortedList<byte, cCustomEntityFormTab> lstTabs = form.getTabsForForm();
            cFields clsfields = new cFields(accountID);
            cCustomEntities entities = new cCustomEntities();

            foreach (cCustomEntityFormTab aTab in lstTabs.Values)
            {
                tab = tabs.Tabs[lstTabs.IndexOfValue(aTab)];
                foreach (cCustomEntityFormSection section in aTab.sections)
                {
                    foreach (cCustomEntityFormField field in section.fields)
                    {
                        if (record.ContainsKey(field.attribute.attributeid))
                        {
                            switch (field.attribute.fieldtype)
                            {
                                case FieldType.Text:
                                case FieldType.Contact:
                                    txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                    if (txtbox != null)
                                    {
                                        if (record.ContainsKey(field.attribute.attributeid))
                                        {
                                            if (record[field.attribute.attributeid] != DBNull.Value)
                                            {
                                                txtbox.Text = (string)record[field.attribute.attributeid];
                                            }
                                        }

                                        if (entity.HasFormSelectionMappings() && entity.FormSelectionAttributeId.HasValue && field.attribute.attributeid == entity.FormSelectionAttributeId.Value)
                                        {
                                            txtbox.Enabled = false;
                                        }
                                    }

                                    break;

                                case FieldType.LargeText:
                                    textatt = (cTextAttribute)field.attribute;
                                    if (textatt.format == AttributeFormat.FormattedText)
                                    {
                                        HiddenField hidden = (HiddenField)tab.FindControl("txthidden" + field.attribute.attributeid);
                                        Panel pnl = (Panel)tab.FindControl("rtepanel" + field.attribute.attributeid);
                                        Literal lit = (Literal)pnl.FindControl("rteliteral" + field.attribute.attributeid);

                                        txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                        if (txtbox != null && pnl != null && hidden != null)
                                        {
                                            if (record.ContainsKey(field.attribute.attributeid))
                                            {
                                                if (record[field.attribute.attributeid] != DBNull.Value)
                                                {

                                                    entities.SaveHTMLEditorImagesToDisk(accountID, entity.entityid, field.attribute.attributeid);

                                                    html_data = (string)record[field.attribute.attributeid];

                                                    string imagePath = string.Empty;
                                                    if (html_data.Contains("SoftwareEurope?=".ToLower()))
                                                    {
                                                        var sharedDirectory = ConfigurationManager.AppSettings["HostedEntityImageLocation"].Split('\\');
                                                        var sharedURL = sharedDirectory.LastOrDefault();
                                                        if (sharedURL != null)
                                                        {
                                                            var user = cMisc.GetCurrentUser();

                                                            imagePath = GetImagePath(user, sharedURL);
                                                        }
                                                        html_data = html_data.Replace("SoftwareEurope?=".ToLower(), imagePath);
                                                    }

                                                    AttemptDocumentWrite(html_data, lit);

                                                    hidden.Value = Microsoft.JScript.GlobalObject.encodeURIComponent(html_data);

                                                    txtbox.Text = hidden.Value == string.Empty ? string.Empty : "X";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                        if (txtbox != null)
                                        {
                                            if (record.ContainsKey(field.attribute.attributeid))
                                            {
                                                if (record[field.attribute.attributeid] != DBNull.Value)
                                                {
                                                    txtbox.Text = (string)record[field.attribute.attributeid];
                                                }
                                            }
                                        }

                                    }

                                    break;

                                case FieldType.Currency:
                                    txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                    if (txtbox != null)
                                    {
                                        if (record.ContainsKey(field.attribute.attributeid))
                                        {
                                            if (record[field.attribute.attributeid] != DBNull.Value)
                                            {
                                                currency = (decimal)record[field.attribute.attributeid];
                                                txtbox.Text = currency.ToString("########0.00");
                                            }
                                        }
                                    }

                                    break;

                                case FieldType.Integer:
                                case FieldType.Number:
                                    txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                    if (txtbox != null)
                                    {
                                        if (record.ContainsKey(field.attribute.attributeid))
                                        {
                                            if (record[field.attribute.attributeid] != DBNull.Value)
                                            {
                                                txtbox.Text = record[field.attribute.attributeid].ToString();
                                            }
                                        }
                                    }

                                    break;

                                case FieldType.TickBox:
                                    cmbbox = (DropDownList)tab.FindControl("cmb" + field.attribute.attributeid);
                                    if (cmbbox != null)
                                    {
                                        if (record.ContainsKey(field.attribute.attributeid))
                                        {
                                            if (record[field.attribute.attributeid] != DBNull.Value)
                                            {
                                                foreach (ListItem item in cmbbox.Items)
                                                {
                                                    item.Selected = false;
                                                }
                                                if ((bool)record[field.attribute.attributeid] == true)
                                                {
                                                    cmbbox.Items.FindByValue("1").Selected = true;
                                                }
                                                else
                                                {
                                                    cmbbox.Items.FindByValue("0").Selected = true;
                                                }
                                            }
                                        }
                                    }

                                    break;

                                case FieldType.List:
                                    cmbbox = (DropDownList)tab.FindControl("cmb" + field.attribute.attributeid);
                                    if (cmbbox != null)
                                    {
                                        if (record.ContainsKey(field.attribute.attributeid))
                                        {
                                            foreach (ListItem item in cmbbox.Items)
                                            {
                                                item.Selected = false;
                                            }
                                            if (record[field.attribute.attributeid] != DBNull.Value)
                                            {
                                                if (cmbbox.Items.FindByValue(record[field.attribute.attributeid].ToString()) != null)
                                                {
                                                    var currentItem = cmbbox.Items.FindByValue(record[field.attribute.attributeid].ToString());
                                                    currentItem.Selected = true;
                                                    if (!currentItem.Enabled)
                                                    {
                                                        currentItem.Enabled = true;
                                                    }
                                                }
                                            }
                                        }

                                        if (entity.HasFormSelectionMappings() && entity.FormSelectionAttributeId.HasValue && field.attribute.attributeid == entity.FormSelectionAttributeId.Value)
                                        {
                                            cmbbox.Enabled = false;
                                        }
                                    }

                                    break;

                                case FieldType.DateTime:
                                    txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                    if (txtbox != null)
                                    {
                                        if (record.ContainsKey(field.attribute.attributeid))
                                        {
                                            if (record[field.attribute.attributeid] != DBNull.Value)
                                            {
                                                dateatt = (cDateTimeAttribute)field.attribute;
                                                if (record[field.attribute.attributeid] != DBNull.Value)
                                                {
                                                    date = (DateTime)record[field.attribute.attributeid];
                                                    switch (dateatt.format)
                                                    {
                                                        case AttributeFormat.DateOnly:
                                                            txtbox.Text = date.ToShortDateString();
                                                            break;
                                                        case AttributeFormat.TimeOnly:
                                                            txtbox.Text = date.ToShortTimeString();
                                                            break;
                                                        case AttributeFormat.DateTime:

                                                            TextBox timeTxtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid + "_time");
                                                            txtbox.Text = date.ToShortDateString();
                                                            timeTxtbox.Text = date.ToShortTimeString();
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;

                                case FieldType.RelationshipTextbox:
                                case FieldType.Relationship:
                                    if (field.attribute.GetType() == typeof(cManyToOneRelationship))
                                    {
                                        var selectinator = (Selectinator)tab.FindControl(field.attribute.ControlID);

                                        if (selectinator != null)
                                        {
                                            if (record.ContainsKey(field.attribute.attributeid))
                                            {
                                                if (record[field.attribute.attributeid] != DBNull.Value)
                                                {
                                                    var relationship = (cManyToOneRelationship)field.attribute;

                                                    // get the main display field
                                                    var autoCompleteDisplayField = clsfields.GetFieldByID(relationship.AutoCompleteDisplayField);
                                                    var autoCompleteTriggerFields = new List<JsAutoCompleteTriggerField>();

                                                    // get any trigger fields
                                                    if (relationship.TriggerLookupFields.Count > 0)
                                                    {
                                                        foreach (LookupDisplayField lookupDisplayField in relationship.TriggerLookupFields)
                                                        {
                                                            autoCompleteTriggerFields.Add(new JsAutoCompleteTriggerField { ControlId = "txt" + lookupDisplayField.attributeid, DisplayFieldId = lookupDisplayField.TriggerDisplayFieldId == null ? string.Empty : lookupDisplayField.TriggerDisplayFieldId.ToString(), JoinViaId = lookupDisplayField.TriggerJoinViaId ?? 0, DisplayValue = string.Empty, ParentControlId = "txt" + field.attribute.attributeid });
                                                        }
                                                    }

                                                    // get the string values to populate the fields with
                                                    var fieldValues = AutoComplete.GetDisplayAndTriggerFieldValues(cMisc.GetCurrentUser(), relationship.relatedtable.TableID, (int)record[field.attribute.attributeid], autoCompleteDisplayField, autoCompleteTriggerFields);
                                                    selectinator.SetValue(fieldValues[0].Item1, (int)record[field.attribute.attributeid]);

                                                    for (int i = 1; i < fieldValues.Count; i++)
                                                    {
                                                        string spanControlId = "txt" + relationship.TriggerLookupFields[i - 1].attributeid;

                                                        foreach (Label lookupDisplaySpan in (from TabPanel t in tabs.Tabs select t.FindControl(spanControlId)).OfType<Label>())
                                                        {
                                                            lookupDisplaySpan.Text = string.IsNullOrWhiteSpace(fieldValues[i].Item1) ? "&nbsp;" : fieldValues[i].Item1;
                                                            if (string.IsNullOrEmpty(fieldValues[i].Item2))
                                                            {
                                                                break;
                                                            }

                                                            lookupDisplaySpan.Attributes.Add("onclick", "javascript:viewFieldLevelAttachment('" + fieldValues[i].Item2 + "', 0, 0, 0, '" + lookupDisplaySpan.ClientID + "', true)");
                                                            lookupDisplaySpan.Attributes.Add("referencevalue",  fieldValues[i].Item2);
                                                            lookupDisplaySpan.CssClass = "lookupdisplayvalue attachmentLookUpDisplayField";
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;

                                case FieldType.Hyperlink:
                                    break;

                                case FieldType.CurrencyList:
                                    cmbbox = (DropDownList)tab.FindControl("ddl" + field.attribute.attributeid);
                                    if (cmbbox != null)
                                    {
                                        if (record.ContainsKey(field.attribute.attributeid))
                                        {
                                            foreach (ListItem item in cmbbox.Items)
                                            {
                                                item.Selected = false;
                                            }

                                            if (record[field.attribute.attributeid] != DBNull.Value)
                                            {
                                                if (cmbbox.Items.FindByValue(record[field.attribute.attributeid].ToString()) == null)
                                                {
                                                    // Add the currency to the list if it is archived
                                                    CurrentUser user = cMisc.GetCurrentUser();
                                                    cCurrencies currencies = new cCurrencies(user.AccountID,
                                                                                             user.CurrentSubAccountId);
                                                    cCurrency attCurrency = currencies.getCurrencyById((int)record[field.attribute.attributeid]);

                                                    if (attCurrency != null && attCurrency.archived)
                                                    {
                                                        cGlobalCurrencies globalCurrencies = new cGlobalCurrencies();
                                                        cGlobalCurrency globalCurrency =
                                                            globalCurrencies.getGlobalCurrencyById(attCurrency.globalcurrencyid);

                                                        if (globalCurrency != null)
                                                        {
                                                            cmbbox.Items.Add(new ListItem(globalCurrency.label, attCurrency.currencyid.ToString()));

                                                            // Sort the list alphabetically and select the newly added currency                       
                                                            cmbbox.DataSource = cmbbox.Items.Cast<ListItem>().OrderBy(o => o.Text).ToList();
                                                            cmbbox.DataTextField = "text";
                                                            cmbbox.DataValueField = "value";
                                                            cmbbox.DataBind();
                                                            cmbbox.Items.FindByValue(attCurrency.currencyid.ToString()).Selected = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    cmbbox.Items.FindByValue(record[field.attribute.attributeid].ToString()).Selected = true;
                                                    break;
                                                }
                                            }

                                            // Select the default currency if a user-specified value has not been set
                                            cmbbox.Items.FindByValue(entity.DefaultCurrencyID.Value.ToString()).Selected = true;
                                        }
                                    }

                                    break;
                                case FieldType.Attachment:
                                    var fileUploader = (FileUploader)tab.FindControl(field.attribute.ControlID);
                                    if (record[field.attribute.attributeid] != DBNull.Value)
                                    {
                                        var fileGuid = (Guid)record[field.attribute.attributeid];
                                        if (fileGuid != new Guid())
                                        {
                                            fileUploader.ViewId = (int)ViewState["viewid"];
                                            fileUploader.EntityId = (int)ViewState["entityid"];
                                            fileUploader.RecordId = (int)ViewState["id"];
                                            fileUploader.ControlId = fileUploader.ClientID;
                                            fileUploader.FileGuid = fileGuid;
                                        }
                                    }

                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static string GetImagePath(CurrentUser user, string sharedURL)
        {
            string hostName = HostManager.GetHostName(user.Account.HostnameIds,
                                                      user.CurrentActiveModule,
                                                      user.Account.companyid);
            return string.Format("http://{0}/{1}/", hostName, sharedURL);
        }

        /// <summary>
        /// Populates the form with any default field values
        /// </summary>
        /// <param name="form">The form to populate</param>
        /// <param name="formSelectionAttributeId">the attribute being used for form selections</param>
        /// <param name="listVal">the form selection list value if one was given</param>
        /// <param name="textVal">the form selection text value if one was given</param>
        private void PopulateDefaultValues(cCustomEntityForm form, int? formSelectionAttributeId = null, int listVal = -1, string textVal = null)
        {
            if (form.CheckDefaultValues || formSelectionAttributeId.HasValue)
            {
                TabContainer tabContainer = (TabContainer)this.holderForm.FindControl("tabs" + form.formid);

                SortedList<byte, cCustomEntityFormTab> tabList = form.getTabsForForm();

                foreach (cCustomEntityFormTab currentTab in tabList.Values)
                {
                    TabPanel tab = tabContainer.Tabs[tabList.IndexOfValue(currentTab)];

                    foreach (cCustomEntityFormSection section in currentTab.sections)
                    {
                        foreach (cCustomEntityFormField field in section.fields)
                        {
                            if (formSelectionAttributeId.HasValue && field.attribute.attributeid == formSelectionAttributeId.Value)
                            {
                                if (field.attribute.fieldtype == FieldType.Text)
                                {
                                    TextBox txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                    txtbox.Text = textVal ?? string.Empty;
                                    txtbox.Enabled = false;
                                }
                                else if (field.attribute.fieldtype == FieldType.List)
                                {
                                    DropDownList dropDownList = (DropDownList)tab.FindControl("cmb" + field.attribute.attributeid);
                                    if (dropDownList != null && dropDownList.Items.FindByValue(listVal.ToString(CultureInfo.InvariantCulture)) != null)
                                    {
                                        dropDownList.SelectedValue = listVal.ToString(CultureInfo.InvariantCulture);
                                        dropDownList.Enabled = false;
                                    }
                                }
                            }
                            else if (string.IsNullOrEmpty(field.DefaultValue) == false)
                            {
                                if (field.attribute.fieldtype == FieldType.Text ||
                                    field.attribute.fieldtype == FieldType.LargeText)
                                {
                                    var textatt = (cTextAttribute)field.attribute;

                                    TextBox txtbox;

                                    if (textatt.format == AttributeFormat.FormattedText)
                                    {
                                        HiddenField hidden = (HiddenField)tab.FindControl("txthidden" + textatt.attributeid);
                                        Panel pnl = (Panel)tab.FindControl("rtepanel" + textatt.attributeid);
                                        Literal lit = (Literal)pnl.FindControl("rteliteral" + textatt.attributeid);

                                        txtbox = (TextBox)tab.FindControl("txt" + textatt.attributeid);

                                        if (txtbox != null && pnl != null && hidden != null)
                                        {
                                            string htmlData = field.DefaultValue;

                                            AttemptDocumentWrite(htmlData, lit);

                                            hidden.Value = Microsoft.JScript.GlobalObject.encodeURIComponent(htmlData);

                                            txtbox.Text = hidden.Value == string.Empty ? string.Empty : "X";
                                        }
                                    }
                                    else
                                    {
                                        txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);

                                        if (txtbox != null)
                                        {
                                            txtbox.Text = field.DefaultValue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void AttemptDocumentWrite(string htmlData, ITextControl lit)
        {
            var doc = new WordDocument();
            doc.EnsureMinimal();
            doc.XHTMLValidateOption = XHTMLValidationType.None;
            htmlData = HttpUtility.UrlDecode(htmlData, Encoding.UTF8);

            var matches = HtmlImgMatches(htmlData);

            foreach (Match match in matches)
            {
                var imageTag = match.Value;

                if (imageTag.Contains(@"/>") == false)
                {
                    //add missing img tag break
                    imageTag = imageTag.Replace(">", "/>");
                }

                htmlData = htmlData.Replace(match.Value, imageTag);
            }
            try
            {
                doc.AddSection().AddParagraph().AppendHTML(htmlData);
                lit.Text = htmlData;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("the 'p' start tag"))
                {
                    HtmlUtility util = HtmlUtility.Instance;
                   htmlData = util.FixPTags(htmlData);

                    try
                    {
                        doc.AddSection().AddParagraph().AppendHTML(htmlData);
                        lit.Text = htmlData;

                    }
                    catch (Exception e)
                    {
                        lit.Text = "## Can't display or merge this text ##";
                    }
                }
            }

            doc.Close();
            doc = null;
        }

        /// <summary>
        /// cmdok_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdok_Click(object sender, CommandEventArgs e)
        {
            CSSButton btn = (CSSButton)sender;
            int entityid, formid, buttonType;
            string[] tmp = btn.CommandArgument.Split(',');
            entityid = Convert.ToInt32(tmp[0]);
            formid = Convert.ToInt32(tmp[1]);

            int newRecordID = SaveCustomEntity(entityid, formid);
            this.ClearLock();

            if (newRecordID > 0)
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity reqEntity = clsCustomEntities.getEntityById((int)ViewState["entityid"]);
                cWorkflows clsWorkflows = new cWorkflows(currentUser, newRecordID, reqEntity.table);
                cWorkflow reqWorkflow = clsWorkflows.GetWorkflowByTableID(reqEntity.table.TableID);
                if (reqWorkflow != null)
                {
                    if (string.IsNullOrEmpty(ViewState["id"].ToString()))
                    {
                        ViewState["id"] = newRecordID;
                    }
                    if (clsWorkflows.EntityInWorkflow(newRecordID, reqWorkflow.workflowid) == false)
                    {
                        clsWorkflows.InsertIntoWorkflow(newRecordID, reqWorkflow.workflowid, currentUser.EmployeeID);
                    }


                    cWorkflowNextStep reqNextWorkflowStep = clsWorkflows.GetNextWorkflowStep(newRecordID, reqWorkflow.workflowid);

                    if (reqNextWorkflowStep != null)
                    {
                        if (reqNextWorkflowStep.Status == WorkflowStatus.Finished)
                        {
                            // what to do if it finished
                            Response.Redirect("viewentities.aspx?entityid=" + (int)ViewState["entityid"] + "&viewid=" + (int)ViewState["viewid"]);
                        }
                        else
                        {
                            switch (reqNextWorkflowStep.NextStep.Action)
                            {
                                case WorkFlowStepAction.Decision:
                                    #region decision modal
                                    WorkflowUserControl wfDecision = new WorkflowUserControl();
                                    Page.LoadControl("~/shared/usercontrols/workflow.ascx");
                                    wfDecision.WorkflowID = reqWorkflow.workflowid;
                                    wfDecision.EntityID = newRecordID;
                                    wfDecision.reqWorkflow = reqWorkflow;
                                    Panel pnlModalDecision = new Panel();
                                    pnlModalDecision.ID = "pnlModalDecision";
                                    pnlModalDecision.CssClass = "errorModal";
                                    pnlModalDecision.Style.Add(HtmlTextWriterStyle.Padding, "4px");
                                    pnlModalDecision.Style.Add(HtmlTextWriterStyle.Display, "none");
                                    pnlWorkflowHolder.Controls.Add(pnlModalDecision);
                                    HyperLink hlModalDecision = new HyperLink();
                                    hlModalDecision.Style.Add(HtmlTextWriterStyle.Display, "none");
                                    hlModalDecision.ID = "hlModal";
                                    pnlWorkflowHolder.Controls.Add(hlModalDecision);
                                    ModalPopupExtender mpeDecision = new ModalPopupExtender();
                                    mpeDecision.ID = "mpeDecision";
                                    mpeDecision.TargetControlID = hlModalDecision.ID;
                                    mpeDecision.PopupControlID = pnlModalDecision.ID;
                                    mpeDecision.BackgroundCssClass = "modalMasterBackground";
                                    mpeDecision.Show();
                                    pnlWorkflowHolder.Controls.Add(mpeDecision);
                                    wfDecision.ModalPopupID = mpeDecision.ClientID;
                                    pnlModalDecision.Controls.Add(wfDecision);
                                    break;
                                    #endregion decision modal
                                case WorkFlowStepAction.ShowMessage:
                                    #region show message modal
                                    Panel pnlModal = new Panel();
                                    pnlModal.ID = "pnlModal";
                                    pnlModal.CssClass = "errorModal";
                                    pnlModal.Style.Add(HtmlTextWriterStyle.Padding, "4px");
                                    pnlModal.Style.Add(HtmlTextWriterStyle.Display, "none");
                                    pnlWorkflowHolder.Controls.Add(pnlModal);
                                    Literal litModal = new Literal();
                                    litModal.ID = "litModal";
                                    pnlModal.Controls.Add(litModal);
                                    HyperLink hlModal = new HyperLink();
                                    hlModal.Style.Add(HtmlTextWriterStyle.Display, "none");
                                    hlModal.ID = "hlModal";
                                    pnlWorkflowHolder.Controls.Add(hlModal);
                                    ModalPopupExtender mpe = new ModalPopupExtender();
                                    mpe.ID = "mpeMessage";

                                    mpe.TargetControlID = hlModal.ID;
                                    mpe.PopupControlID = pnlModal.ID;
                                    mpe.BackgroundCssClass = "modalMasterBackground";
                                    mpe.Show();
                                    pnlWorkflowHolder.Controls.Add(mpe);
                                    litModal.Text = ((cShowMessageStep)reqNextWorkflowStep.NextStep).Message + "<p><img src=\"images/buttons/btn_close.png\" title=\"Close\" onclick=\"$find('" + mpe.ClientID + "').hide();\"></p>";
                                    cWorkflowNextStep reqNextWorkflowStepPushAgain = clsWorkflows.GetNextWorkflowStep(newRecordID, reqWorkflow.workflowid);
                                    break;
                                    #endregion show message modal
                                case WorkFlowStepAction.Approval:
                                    #region approval message modal
                                    Panel pnlModalApproval = new Panel();
                                    pnlModalApproval.ID = "pnlModal";
                                    pnlModalApproval.CssClass = "errorModal";
                                    pnlModalApproval.Style.Add(HtmlTextWriterStyle.Padding, "4px");
                                    pnlWorkflowHolder.Controls.Add(pnlModalApproval);
                                    Literal litModalApproval = new Literal();
                                    litModalApproval.ID = "litModal";
                                    pnlModalApproval.Controls.Add(litModalApproval);
                                    HyperLink hlModalApproval = new HyperLink();
                                    hlModalApproval.Style.Add(HtmlTextWriterStyle.Display, "none");
                                    hlModalApproval.ID = "hlModal";
                                    pnlWorkflowHolder.Controls.Add(hlModalApproval);
                                    ModalPopupExtender mpeApproval = new ModalPopupExtender();
                                    mpeApproval.ID = "mpeMessage";
                                    mpeApproval.TargetControlID = hlModalApproval.ID;
                                    mpeApproval.PopupControlID = pnlModalApproval.ID;
                                    mpeApproval.BackgroundCssClass = "modalMasterBackground";
                                    mpeApproval.Show();
                                    pnlWorkflowHolder.Controls.Add(mpeApproval);
                                    litModalApproval.Text = ((cApprovalStep)reqNextWorkflowStep.NextStep).Message + "<p><img src=\"images/buttons/btn_close.png\" title=\"Close\" onclick=\"$find('" + mpeApproval.ClientID + "').hide();\"></p>";
                                    break;
                                    #endregion show message modal
                                case WorkFlowStepAction.ChangeCustomEntityForm:
                                    Session["fromWorkflow"] = true;
                                    Response.Redirect("aeentity.aspx?viewid=" + (int)ViewState["viewid"] + "&entityid=" + (int)ViewState["entityid"] + "&formid=" + ((cChangeFormStep)reqNextWorkflowStep.NextStep).FormID + "&id=" + newRecordID + "&fromWorkflow=true", true);
                                    break;
                                case WorkFlowStepAction.ErrorWarning:
                                    Panel pnlErrorWarning = new Panel();
                                    pnlErrorWarning.ID = "pnlModal";
                                    pnlErrorWarning.CssClass = "errorModal";
                                    pnlErrorWarning.Style.Add(HtmlTextWriterStyle.Padding, "4px");
                                    pnlErrorWarning.Style.Add(HtmlTextWriterStyle.Display, "none");
                                    pnlWorkflowHolder.Controls.Add(pnlErrorWarning);
                                    Literal litErrorWarning = new Literal();
                                    litErrorWarning.ID = "litModal";
                                    pnlErrorWarning.Controls.Add(litErrorWarning);
                                    HyperLink hlErrorWarning = new HyperLink();
                                    hlErrorWarning.Style.Add(HtmlTextWriterStyle.Display, "none");
                                    hlErrorWarning.ID = "hlModal";
                                    pnlWorkflowHolder.Controls.Add(hlErrorWarning);
                                    ModalPopupExtender mpeErrorWarning = new ModalPopupExtender();
                                    mpeErrorWarning.ID = "mpeMessage";
                                    mpeErrorWarning.TargetControlID = hlErrorWarning.ID;
                                    mpeErrorWarning.PopupControlID = pnlErrorWarning.ID;
                                    mpeErrorWarning.BackgroundCssClass = "modalMasterBackground";
                                    mpeErrorWarning.Show();
                                    pnlWorkflowHolder.Controls.Add(mpeErrorWarning);
                                    litErrorWarning.Text = reqNextWorkflowStep.NextStep.Description + "<p><img src=\"images/buttons/btn_close.png\" title=\"Close\" onclick=\"$find('" + mpeErrorWarning.ClientID + "').hide();\"></p>";

                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                }
                else
                {
                    redirectPage(false);
                }
            }
            else
            {
                #region Save Error Messages
                Panel pnlModal = new Panel();
                pnlModal.ID = "pnlModal";
                pnlModal.CssClass = "errorModal";
                pnlWorkflowHolder.Controls.Add(pnlModal);
                Literal litModal = new Literal();
                litModal.ID = "litModal";
                pnlModal.Controls.Add(litModal);
                HyperLink hlModal = new HyperLink();
                hlModal.Style.Add(HtmlTextWriterStyle.Display, "none");
                hlModal.ID = "hlModal";
                pnlWorkflowHolder.Controls.Add(hlModal);
                ModalPopupExtender mpe = new ModalPopupExtender();
                mpe.ID = "mpeMessage";
                mpe.TargetControlID = hlModal.ID;
                mpe.PopupControlID = pnlModal.ID;
                mpe.BackgroundCssClass = "modalMasterBackground";
                mpe.Show();
                pnlWorkflowHolder.Controls.Add(mpe);
                StringBuilder sb = new StringBuilder();
                cModules clsModules = new cModules();
                CurrentUser currentUser = cMisc.GetCurrentUser();
                cModule module = clsModules.GetModuleByID((int)currentUser.CurrentActiveModule);
                string sModuleName = "Message From " + module.BrandNameHTML;
                switch (newRecordID)
                {
                    case (int)ReturnValues.AlreadyExists:
                        if (ViewState["uniqueAttributeId"] != null)
                        {
                            int uniqueAttId = (int)ViewState["uniqueAttributeId"];
                            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                            cCustomEntity reqEntity = clsCustomEntities.getEntityById(entityid);
                            cAttribute att = reqEntity.attributes[uniqueAttId];
                            sb.Append("<div class=\"errorModalSubject\">");
                            sb.Append(sModuleName);
                            sb.Append("</div><br/><div class=\"errorModalBody\"><ul style=\"padding-bottom: 0px; list-style-type: none; margin: 0px; padding-left: 0px; padding-right: 0px; padding-top: 0px;\"><li>Cannot save record as the value for ");
                            sb.Append(att.displayname);
                            sb.Append(" already exists.</li></ul></div>");
                        }
                        else
                        {
                            sb.Append("<div class=\"errorModalSubject\">");
                            sb.Append(sModuleName);
                            sb.Append("</div><br/><div class=\"errorModalBody\"><ul style=\"padding-bottom: 0px; list-style-type: none; margin: 0px; padding-left: 0px; padding-right: 0px; padding-top: 0px;\"><li>Cannot save record as a unique field already exists. Unique field cannot be determined at this time.</li></ul></div>");
                        }
                        break;
                    case (int)ReturnValues.InvalidFile:
                        sb.Append("<div class=\"errorModalSubject\">");
                        sb.Append(sModuleName);
                        sb.Append("</div><br/><div class=\"errorModalBody\"><ul style=\"padding-bottom: 0px; list-style-type: none; margin: 0px; padding-left: 0px; padding-right: 0px; padding-top: 0px;\"><li>Selected file in not valid.</li></ul></div>");
                        break;
                    case (int)ReturnValues.InvalidFileType:
                        sb.Append("<div class=\"errorModalSubject\">");
                        sb.Append(sModuleName);
                        sb.Append("</div><br/><div class=\"errorModalBody\"><ul style=\"padding-bottom: 0px; list-style-type: none; margin: 0px; padding-left: 0px; padding-right: 0px; padding-top: 0px;\"><li>Selected file type is not currently supported.</li></ul></div>");
                        break;
                    case (int)ReturnValues.ErrorWithAttachmentDataSave:
                        sb.Append("<div class=\"errorModalSubject\">");
                        sb.Append(sModuleName);
                        sb.Append("</div><br/><div class=\"errorModalBody\"><ul style=\"padding-bottom: 0px; list-style-type: none; margin: 0px; padding-left: 0px; padding-right: 0px; padding-top: 0px;\"><li>Selected file type is not supported.</li></ul></div>");
                        break;
                    default:
                        sb.Append("<div class=\"errorModalSubject\">");
                        sb.Append(sModuleName);
                        sb.Append("</div><br/><div class=\"errorModalBody\"><ul style=\"padding-bottom: 0px; list-style-type: none; margin: 0px; padding-left: 0px; padding-right: 0px; padding-top: 0px;\"><li>Unknown error. Record not saved.</li></ul></div>");
                        break;
                }
                sb.Append("<div style=\"padding-bottom: 5px; padding-left: 5px; padding-right: 5px; padding-top: 0px;\"><img style=\"cursor: pointer;\" src=\"/shared/images/buttons/btn_close.png\" title=\"Close\" onclick=\"$find('" + mpe.ClientID + "').hide();\"/></div>");
                litModal.Text = sb.ToString();
                #endregion

                refreshRichTextEditors(entityid, formid);
            }
        }

        public void refreshRichTextEditors(int entityID, int formID)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            var clsentities = new cCustomEntities(curUser);
            cCustomEntity entity = clsentities.getEntityById(entityID);
            cCustomEntityForm form = entity.getFormById(formID);
            SortedList<byte, cCustomEntityFormTab> formtabs = form.getTabsForForm();
            var tabs = (TabContainer)this.holderForm.FindControl("tabs" + (int)ViewState["formid"]);

            // refresh any HTML editor panels
            foreach (KeyValuePair<byte, cCustomEntityFormTab> tab_kvp in formtabs)
            {
                var formtab = (cCustomEntityFormTab)tab_kvp.Value;

                foreach (KeyValuePair<byte, cCustomEntityFormSection> section_kvp in formtab.getSectionsForTab())
                {
                    var formsection = (cCustomEntityFormSection)section_kvp.Value;

                    foreach (cCustomEntityFormField field in formsection.getFieldsForSection())
                    {
                        if (field.attribute.fieldtype == FieldType.LargeText)
                        {
                            var tabPanel = (TabPanel)tabs.FindControl("tab" + formtab.tabid);

                            var txtatt = (cTextAttribute)field.attribute;
                            if (txtatt.format == AttributeFormat.FormattedText)
                            {
                                var txt = (HiddenField)tabPanel.FindControl("txthidden" + txtatt.attributeid);
                                var lit = (Literal)tabPanel.FindControl("rteliteral" + txtatt.attributeid);

                                if (txt != null && lit != null)
                                {
                                    AttemptDocumentWrite(txt.Value, lit);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// redirectPage: move to a new page within custom entity structure
        /// </summary>
        /// <param name="samePage">True if to redirect to the same page</param>
        public void redirectPage(bool samePage)
        {
            var viewid = (int)ViewState["viewid"];
            var entityid = (int)ViewState["entityid"];

            var crumbs = (List<sEntityBreadCrumb>)ViewState["entitycrumbs"];
            var customEntities = new cCustomEntities(cMisc.GetCurrentUser());

            if (crumbs == null || crumbs.Count == 1) //go back to summary
            {
                if (samePage)
                {
                    var newFormId = customEntities.getEntityById(entityid).Views[viewid].DefaultAddForm.formid;
                    Response.Redirect(string.Format("~/shared/aeentity.aspx?entityid={0}&viewid={1}&formid={2}&tabid=0", this.ViewState["entityid"], this.ViewState["viewid"], newFormId));
                }
                else
                {
                    Response.Redirect(string.Format("~/shared/viewentities.aspx?entityid={0}&viewid={1}", this.ViewState["entityid"], this.ViewState["viewid"]));
                }
            }
            else
            {
                int formid;
                int tabid;
                int id;

                string[] arrRelentityid = Request.QueryString["relentityid"].Split(',');
                string[] arrRelformid = Request.QueryString["relformid"].Split(',');
                string[] arrRelrecordid = Request.QueryString["relrecordid"].Split(',');
                string[] arrReltabid = Request.QueryString["reltabid"].Split(',');
                string[] arrRelviewid = Request.QueryString["relviewid"].Split(',');

                string popupformid;
                string popupentityid;
                string attributeid;
                string returnurl;
                string entityurl = "";
                string formurl = "";
                string recordurl = "";
                string taburl = "";
                for (int x = 0; x < arrRelentityid.Length - 1; x++)
                {
                    entityurl += "relentityid=" + arrRelentityid[x] + "&";
                    formurl += "relformid=" + arrRelformid[x] + "&";
                    recordurl += "relrecordid=" + arrRelrecordid[x] + "&";
                    taburl += "reltabid=" + arrReltabid[x] + "&";
                    taburl += "relviewid=" + arrRelviewid[x] + "&";
                }
                int.TryParse(arrRelentityid[arrRelentityid.Length - 1], out entityid);
                int.TryParse(arrRelformid[arrRelformid.Length - 1], out formid);
                int.TryParse(arrRelrecordid[arrRelrecordid.Length - 1], out id);
                int.TryParse(arrReltabid[arrReltabid.Length - 1], out tabid);
                int.TryParse(arrRelviewid[arrRelviewid.Length - 1], out viewid);

                string popupview = Request.QueryString["popupview"];

                //check if we have come from a pop-up window
                if (popupview == null)
                {
                    //rebuild redirect to return to the entity
                    returnurl = "~/shared/aeentity.aspx?";
                    returnurl += entityurl + formurl + recordurl + taburl + "viewid=" + viewid + "&entityid=" + entityid +
                                 "&formid=" + formid + "&tabid=" + tabid + "&id=" + id;
                }
                else
                {
                    //rebuild redirect to return to the pop-up window and apply the appropriate filters. 
                    popupentityid = Request.QueryString["entityid"];
                    attributeid = Request.QueryString["attributeid"];
                    returnurl = "~/shared/aeEntityPopup.aspx?";
                    returnurl += "entityid=" + popupentityid + "&viewID=" + popupview +
                                 "&relformID=" + formid + "&relentityid=" + entityid + "&reltabid=" + tabid +
                                 "&relrecordid=" + id + "&attributeid=" + attributeid + "&relviewid=" + viewid;
                }
                Response.Redirect(returnurl, true);
            }
        }

        /// <summary>
        /// cmdcancel_Click: Abort the entity without saving changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdcancel_Click(object sender, CommandEventArgs e)
        {
            this.ViewState["ceID"] = 0;
            CurrentUser currUser = cMisc.GetCurrentUser();
            var attachments = new cAttachments(currUser.AccountID, currUser.EmployeeID, currUser.CurrentSubAccountId, currUser.isDelegate ? (int?)currUser.Delegate.EmployeeID : null);
            int entityid = (int)ViewState["entityid"];
            attachments.RemoveOrphanAttachmentsOnCustomTables(entityid, currUser.EmployeeID, DateTime.Now);
            
            this.ClearLock();
            this.redirectPage(false);
        }

        /// <summary>
        /// btnSaveAndNew_Click: Execute the Save & New action on the entity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveAndNew_Click(object sender, CommandEventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            CSSButton btn = (CSSButton)sender;
            int entityid, formid, buttontype, retid;
            string[] tmp = btn.CommandArgument.Split(',');
            entityid = Convert.ToInt32(tmp[0]);
            formid = Convert.ToInt32(tmp[1]);
            buttontype = Convert.ToInt32(tmp[2]); // 0=Save and Duplicate, 1=Save and Stay, 2=Save and New
            retid = SaveCustomEntity(entityid, formid, buttontype);
            if (buttontype != 1)
            {
                this.ClearLock();
            }
            
            #region Save And New Messages

            Panel pnlModal = new Panel();
            pnlModal.ID = "pnlSaveAndNewModal";
            pnlModal.CssClass = "errorModal";
            pnlWorkflowHolder.Controls.Add(pnlModal);
            Literal litModal = new Literal();
            litModal.ID = "litSaveAndNewModal";
            pnlModal.Controls.Add(litModal);
            HyperLink hlModal = new HyperLink();
            hlModal.Style.Add(HtmlTextWriterStyle.Display, "none");
            hlModal.ID = "hlSaveAndNewModal";
            pnlWorkflowHolder.Controls.Add(hlModal);
            ModalPopupExtender mpe = new ModalPopupExtender();
            mpe.ID = "mpeSaveAndNewMessage";
            mpe.TargetControlID = hlModal.ID;
            mpe.PopupControlID = pnlModal.ID;
            mpe.BackgroundCssClass = "modalMasterBackground";
            mpe.Show();
            pnlWorkflowHolder.Controls.Add(mpe);
            var sb = new StringBuilder();
            var clsModules = new cModules();
            cModule module = clsModules.GetModuleByID((int)curUser.CurrentActiveModule);
            string sModuleName = "Message From " + module.BrandNameHTML;
            bool invalidFile = false;

            switch (retid)
            {
                case (int)ReturnValues.AlreadyExists:
                    sb.Append("<div class=\"errorModalSubject\">" + sModuleName + "</div><br /><div class=\"errorModalBody\">Cannot save record as one of the field values already exists in another record.</div>");
                    retid = 0; // if remains as -1, will cause error
                    break;
                case (int)ReturnValues.InvalidFile:
                    sb.Append("<div class=\"errorModalSubject\">" + sModuleName + "</div><br /><div class=\"errorModalBody\">Cannot save record as one or more of the attachments are invalid.</div>");
                    retid = 0; // if remains as -1, will cause error
                    invalidFile = true;
                    break;
                case (int)ReturnValues.InvalidFileType:
                    sb.Append("<div class=\"errorModalSubject\">" + sModuleName + "</div><br /><div class=\"errorModalBody\">Cannot save record as one or more of the attachments have an invalid file type.</div>");
                    retid = 0; // if remains as -1, will cause error
                    invalidFile = true;
                    break;
                case 0:
                    sb.Append("<div class=\"errorModalSubject\">" + sModuleName + "</div><br /><div class=\"errorModalBody\">Unknown problem encountered during save. The record was not saved.</div>");
                    break;
                default:
                    switch (buttontype)
                    {
                        case 0:
                            sb.Append("<div class=\"errorModalSubject\">" + sModuleName + "</div><br /><div class=\"errorModalBody\">The save was successful. The form has maintained the same field values to make it easier for you to submit another similar new entry.</div>");
                            break;
                        default:
                            sb.Append("<div class=\"errorModalSubject\">" + sModuleName + "</div><br /><div class=\"errorModalBody\">The save was successful.</div>");
                            break;
                    }

                    break;
            }

            sb.Append("<div style=\"padding-bottom: 5px; padding-left: 5px; padding-right: 5px; padding-top: 0px;\"><img style=\"cursor: pointer;\" src=\"/shared/images/buttons/btn_close.png\" title=\"Close\" onclick=\"$find('" + mpe.ClientID + "').hide();\"/></div>");
            litModal.Text = sb.ToString();

            #endregion Save And New Messages

            cCustomEntities clsentities = new cCustomEntities(curUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);
            cCustomEntityForm form = entity.getFormById(formid);

            if (buttontype == 0 && retid == 0)
            {
                //an error has occurred, do not reset IDs
                List<sEntityBreadCrumb> crumbs = (List<sEntityBreadCrumb>)ViewState["entitycrumbs"];

                if (crumbs != null)
                {
                    sEntityBreadCrumb crumb = crumbs.Last();
                    crumb.RecordID = (int)ViewState["ceID"];

                    crumbs[crumbs.Count - 1] = crumb;
                    ViewState["entitycrumbs"] = crumbs;
                }

                if (clsentities.IsUserAllowedAccessToAudienceRecord(entity, (int)ViewState["id"]) == false)
                {
                    Response.Redirect("~/shared/restricted.aspx?reason=Audience%20access%20does%20not%20permit%20you%20to%20view%20this%20record.", true);
                }

                Master.title = clsentities.GetMasterPageTitle(entity, form, (List<sEntityBreadCrumb>)ViewState["entitycrumbs"]);

                Master.PageSubTitle = cMisc.DotonateString(form.formname, 36);

                CreateAttachmentsTab(entity, formid);

                CreateMergeConfigTab(entity, curUser, formid);

                CreateAudiencesTab(entity, formid);

                if (form.ShowBreadCrumbs)
                {
                    createBreadcrumbTree((List<sEntityBreadCrumb>)ViewState["entitycrumbs"], (int)ViewState["viewid"]);
                }

                CreateOtmGridAndAutoCompleteJavascript();

                refreshRichTextEditors(entityid, formid);
            }
            else
            {
                if (buttontype == 0)
                {
                    this.ViewState["id"] = 0;
                    this.ViewState["ceID"] = 0;
                    this.CreateOtmGridAndAutoCompleteJavascript();
                    this.GenerateAttachmentData(form);
                }
                else
                {
                    if (retid > 0)
                    {
                        this.ViewState["id"] = retid;
                        this.ViewState["ceID"] = retid;

                        List<sEntityBreadCrumb> crumbs = (List<sEntityBreadCrumb>)this.ViewState["entitycrumbs"];

                        if (crumbs != null)
                        {
                            sEntityBreadCrumb crumb = crumbs.Last();
                            crumb.RecordID = (int)this.ViewState["ceID"];

                            crumbs[crumbs.Count - 1] = crumb;
                            this.ViewState["entitycrumbs"] = crumbs;
                        }
                    }

                    this.CreateAttachmentsTab(entity, formid);
                    this.CreateMergeConfigTab(entity, curUser, formid);
                    this.CreateAudiencesTab(entity, formid);
                    this.CreateOtmGridAndAutoCompleteJavascript();
                    if (buttontype == 2 && !invalidFile)
                    {
                        this.ClearDataFromFields();
                    }
                }

                if (form.ShowBreadCrumbs)
                {
                    createBreadcrumbTree((List<sEntityBreadCrumb>)ViewState["entitycrumbs"], (int)ViewState["viewid"]);
                }

                Master.title = clsentities.GetMasterPageTitle(entity, form, (List<sEntityBreadCrumb>)ViewState["entitycrumbs"]);

                Master.PageSubTitle = cMisc.DotonateString(form.formname, 36);

                refreshRichTextEditors(entityid, formid);
            }

            SetActiveTab(true);
        }

        private void ClearDataFromFields()
        {
            var crumbs = (List<sEntityBreadCrumb>)ViewState["entitycrumbs"];
            if (crumbs != null)
            {
                sEntityBreadCrumb crumb = crumbs.Last();
                crumb.RecordID = 0;

                crumbs[crumbs.Count - 1] = crumb;
                ViewState["entitycrumbs"] = crumbs;
            }

            var id = Request["relrecordid"];
            var tabid = Request["reltabid"];
            var view = Request["viewid"];

            //rebuild redirect to return to the pop-up window and apply the appropriate filters. 
            var formid = Request.QueryString["formid"];
            var entityid = Request.QueryString["entityid"];
            var relentityid = Request.QueryString["relentityid"];
            var relviewid = Request.QueryString["relviewid"];
            var relformid = Request.QueryString["relformid"];
            var returnurl = "~/shared/aeEntity.aspx?";
            var popupview = Request.QueryString["popupview"];
            if (relentityid != null)
            {
                returnurl += string.Format("entityid={0}&viewid={1}&formid={2}&relformid={3}&relentityid={4}&reltabid={5}&relrecordid={6}&tabid=0&relviewid={7}", entityid, view, formid, relformid, relentityid, tabid, id, relviewid);
            }
            else
            {
                returnurl += string.Format("entityid={0}&viewid={1}&formid={2}&tabid=0", entityid, view, formid);
            }

            if (popupview != null)
            {
                var attributeid = Request.QueryString["attributeid"];
                returnurl += string.Format("&popupview={0}&attributeid={1}", popupview, attributeid);
            }

            Response.Redirect(returnurl, true);
        }

        /// <summary>
        /// Save custom entity to the database
        /// </summary>
        /// <param name="entityid">ID of the entity being saved</param>
        /// <param name="formid">ID of the entity entry form</param>
        /// <returns></returns>
        public int SaveCustomEntity(int entityid, int formid, int buttonType = 3)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            var clsentities = new cCustomEntities(curUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);

            if (aelocking.Active && aelocking.Locked)
            {
                return (int)ViewState["id"];
            }

            cCustomEntityForm form = entity.getFormById(formid);
            var tabs = (TabContainer)holderForm.FindControl("tabs" + formid);
            var clsfields = new cFields((int)ViewState["accountid"]);
            SortedList<byte, cCustomEntityFormTab> lstTabs = form.getTabsForForm();
            var audit = new cAuditLog();
            var clsquery = new cUpdateQuery((int)ViewState["accountid"], cAccounts.getConnectionString((int)ViewState["accountid"]), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, entity.table, new cTables((int)ViewState["accountid"]), new cFields((int)ViewState["accountid"]));
            var crumbs = (List<sEntityBreadCrumb>)ViewState["entitycrumbs"];
            var auditEntries = new Dictionary<int, string>();
            var uniqueAttributeEntries = new Dictionary<int, string>();
            var attachments = new List<cAttachments>();

            if (crumbs != null && crumbs.Count > 1) //need to relate to primary record
            {
                cCustomEntity parentEntity = clsentities.getEntityById(crumbs[crumbs.Count - 2].EntityID);
                //find relationship
                List<cOneToManyRelationship> onetomany = parentEntity.findOneToManyRelationship(entityid);

                //add all one to many relationships
                foreach (var item in onetomany)
                {
                    clsquery.addColumn(clsfields.GetFieldByID(item.fieldid), crumbs[crumbs.Count - 2].RecordID);
                }
            }

            int id;
            int retId = 0;
            bool monetaryFieldSet = false;

            foreach (cCustomEntityFormTab aTab in lstTabs.Values)
            {
                TabPanel tab = tabs.Tabs[lstTabs.IndexOfValue(aTab)];
                foreach (cCustomEntityFormSection section in aTab.sections)
                {
                    foreach (cCustomEntityFormField field in section.fields)
                    {
                        TextBox txtbox;
                        DropDownList cmbbox;
                        switch (field.attribute.fieldtype)
                        {
                            case FieldType.Text:
                            case FieldType.Contact:
                                txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);

                                if (entity.FormSelectionAttributeId.HasValue && field.attribute.attributeid == entity.FormSelectionAttributeId.Value
                                    && ViewState["isFormSelection" + entityid] is bool && (bool)ViewState["isFormSelection" + entityid])
                                {
                                    clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), ViewState["FormSelectionValue" + entityid]);
                                    auditEntries.Add(field.attribute.attributeid, (string)ViewState["FormSelectionValue" + entityid]);
                                }
                                else if (txtbox.Text != "")
                                {
                                    clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), txtbox.Text);
                                    auditEntries.Add(field.attribute.attributeid, txtbox.Text.Trim());
                                }
                                else
                                {
                                    clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                    auditEntries.Add(field.attribute.attributeid, "");
                                }

                                if (field.attribute.isunique)
                                {
                                    uniqueAttributeEntries.Add(field.attribute.attributeid, txtbox.Text.Trim());
                                }

                                break;
                            case FieldType.LargeText:
                                var textatt = (cTextAttribute)field.attribute;
                                if (textatt.format == AttributeFormat.FormattedText)
                                {
                                    string tmpValue = string.Empty;
                                    var hidden = (HiddenField)tab.FindControl("txthidden" + field.attribute.attributeid);
                                    if (hidden.Value != string.Empty)
                                    {
                                     tmpValue = HttpUtility.UrlDecode(hidden.Value, Encoding.Default);
                                        HtmlUtility util = HtmlUtility.Instance;
                                        string htmlTagWhitelistTemplatePath =
                                            ConfigurationManager.AppSettings["PermittedHtmlTagsPath"];     
                                        tmpValue = util.SanitizeHtml(tmpValue, htmlTagWhitelistTemplatePath);
                                        StringManipulators.ReplaceAmpersandInHtmlString(ref tmpValue);
                                        if (tmpValue.Contains("<img"))
                                        {
                                            tmpValue = formatImageTags(tmpValue, entityid, field.attribute.attributeid,
                                                                       buttonType);
                                            if (tmpValue.Contains(@"/>") == false)
                                            {
                                                //fix missing img tag break
                                                tmpValue = tmpValue.Replace(">", "/>");
                                            }
                                        }
                                        byte[] data = Encoding.Default.GetBytes(tmpValue);
                                        string output = Encoding.UTF8.GetString(data);

                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), output);
                                        auditEntries.Add(field.attribute.attributeid, output.Trim());
                                    }
                                    else
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                        auditEntries.Add(field.attribute.attributeid, tmpValue);
                                    }

                                    if (field.attribute.isunique)
                                    {
                                        uniqueAttributeEntries.Add(field.attribute.attributeid, tmpValue.Trim());
                                    }
                                }
                                else
                                {
                                    txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                    if (txtbox.Text != "")
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), txtbox.Text);
                                        auditEntries.Add(field.attribute.attributeid, txtbox.Text.Trim());
                                    }
                                    else
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                        auditEntries.Add(field.attribute.attributeid, "");
                                    }

                                    if (field.attribute.isunique)
                                    {
                                        uniqueAttributeEntries.Add(field.attribute.attributeid, txtbox.Text.Trim());
                                    }
                                }
                                break;
                            case FieldType.Currency:
                            case FieldType.Number:
                                txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                if (txtbox.Text != "")
                                {
                                    clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), Convert.ToDecimal(txtbox.Text));
                                    auditEntries.Add(field.attribute.attributeid, txtbox.Text.Trim());
                                }
                                else
                                {
                                    clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                    auditEntries.Add(field.attribute.attributeid, "");
                                }

                                if (field.attribute.isunique)
                                {
                                    uniqueAttributeEntries.Add(field.attribute.attributeid, txtbox.Text.Trim());
                                }

                                break;
                            case FieldType.Integer:
                                txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                if (txtbox.Text != "")
                                {
                                    clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), Convert.ToInt32(txtbox.Text));
                                    auditEntries.Add(field.attribute.attributeid, txtbox.Text.Trim());
                                }
                                else
                                {
                                    clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                    auditEntries.Add(field.attribute.attributeid, "");
                                }

                                if (field.attribute.isunique)
                                {
                                    uniqueAttributeEntries.Add(field.attribute.attributeid, txtbox.Text.Trim());
                                }

                                break;
                            case FieldType.TickBox:
                                cmbbox = (DropDownList)tab.FindControl("cmb" + field.attribute.attributeid);
                                if (cmbbox != null)
                                {
                                    if (cmbbox.SelectedItem.Value != "-1")
                                    {
                                        int nVal;
                                        int.TryParse(cmbbox.SelectedItem.Value, out nVal);
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), nVal);
                                        auditEntries.Add(field.attribute.attributeid, cmbbox.SelectedItem.Text);
                                        if (field.attribute.isunique)
                                        {
                                            uniqueAttributeEntries.Add(field.attribute.attributeid, cmbbox.SelectedItem.Value);
                                        }
                                    }
                                    else
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                        auditEntries.Add(field.attribute.attributeid, "");
                                        if (field.attribute.isunique)
                                        {
                                            uniqueAttributeEntries.Add(field.attribute.attributeid, "");
                                        }
                                    }
                                }

                                break;
                            case FieldType.DateTime:
                                txtbox = (TextBox)tab.FindControl("txt" + field.attribute.attributeid);
                                var dt = new DateTime();
                                var dateatt = (cDateTimeAttribute)field.attribute;

                                if (txtbox.Text != "")
                                {
                                    switch (dateatt.format)
                                    {
                                        case AttributeFormat.DateTime:
                                            var timeTextBox =
                                                (TextBox)tab.FindControl("txt" + field.attribute.attributeid + "_time");

                                            dt = (timeTextBox == null) ? DateTime.Parse(txtbox.Text) : DateTime.Parse(txtbox.Text + " " + timeTextBox.Text);
                                            auditEntries.Add(field.attribute.attributeid, dt.ToString("MM/dd/yyyy HH:mm"));
                                            break;
                                        case AttributeFormat.DateOnly:
                                            dt = DateTime.Parse(txtbox.Text);
                                            auditEntries.Add(field.attribute.attributeid, dt.ToString("MM/dd/yyyy"));
                                            break;
                                        case AttributeFormat.TimeOnly:
                                            dt = DateTime.Parse("01/01/1900 " + txtbox.Text);
                                            auditEntries.Add(field.attribute.attributeid, dt.ToString("HH:mm"));
                                            break;
                                    }

                                    clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), dt);
                                    if (field.attribute.isunique)
                                    {
                                        uniqueAttributeEntries.Add(field.attribute.attributeid, dt.ToString("MM/dd/yyyy HH:mm"));
                                    }
                                }
                                else
                                {
                                    clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                    auditEntries.Add(field.attribute.attributeid, "");
                                    if (field.attribute.isunique)
                                    {
                                        uniqueAttributeEntries.Add(field.attribute.attributeid, "");
                                    }
                                }

                                break;
                            case FieldType.List:
                                cmbbox = (DropDownList)tab.FindControl("cmb" + field.attribute.attributeid);
                                if (cmbbox != null)
                                {
                                    if (entity.FormSelectionAttributeId.HasValue && field.attribute.attributeid == entity.FormSelectionAttributeId.Value && ViewState["isFormSelection" + entityid] is bool)
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), ViewState["FormSelectionValue" + entityid]);
                                        auditEntries.Add(field.attribute.attributeid, cmbbox.Items.FindByValue(((int)ViewState["FormSelectionValue" + entityid]).ToString()).Text);
                                        if (field.attribute.isunique)
                                        {
                                            uniqueAttributeEntries.Add(field.attribute.attributeid, ((int)ViewState["FormSelectionValue" + entityid]).ToString());
                                        }
                                    }
                                    else if (cmbbox.SelectedValue != " " && cmbbox.SelectedValue != "-1")
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), cmbbox.SelectedValue);
                                        auditEntries.Add(field.attribute.attributeid, cmbbox.SelectedItem.Text);
                                        if (field.attribute.isunique)
                                        {
                                            uniqueAttributeEntries.Add(field.attribute.attributeid, cmbbox.SelectedValue);
                                        }
                                    }
                                    else
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                        auditEntries.Add(field.attribute.attributeid, "");

                                        if (field.attribute.isunique)
                                        {
                                            uniqueAttributeEntries.Add(field.attribute.attributeid, "");
                                        }
                                    }
                                }

                                break;
                            case FieldType.Relationship:
                            case FieldType.RelationshipTextbox:
                                if (field.attribute.GetType() == typeof(cManyToOneRelationship))
                                {
                                    var selectinator = (Selectinator)tab.FindControl("txt" + field.attribute.attributeid);
                                    if (selectinator != null && selectinator.SelectedId != null && selectinator.Text != "" && selectinator.SelectedId != "0" && selectinator.SelectedId != "-1")
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), selectinator.SelectedId);
                                        auditEntries.Add(field.attribute.attributeid, selectinator.Text.Trim());
                                    }
                                    else
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                        auditEntries.Add(field.attribute.attributeid, "");
                                    }

                                    if (field.attribute.isunique)
                                    {
                                        uniqueAttributeEntries.Add(field.attribute.attributeid, selectinator.Text.Trim());
                                    }
                                }

                                break;
                            case FieldType.CurrencyList:
                                cmbbox = (DropDownList)tab.FindControl("ddl" + field.attribute.attributeid);
                                if (cmbbox != null)
                                {
                                    if (cmbbox.SelectedValue != " " && cmbbox.SelectedValue != "-1")
                                    {
                                        clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), cmbbox.SelectedValue);
                                        auditEntries.Add(field.attribute.attributeid, cmbbox.SelectedItem.Text.Trim());
                                        monetaryFieldSet = true;
                                    }
                                }

                                break;
                            case FieldType.Attachment:
                                var fileUploader = (FileUploader)tab.FindControl(field.attribute.ControlID);
                                if (fileUploader.HasFile && fileUploader.Changed)
                                {
                                    // if the control already has a file guid set, then we need to delete the existing attachment data
                                    if (fileUploader.FileGuid != Guid.Empty)
                                    {
                                        cCustomEntities.DeleteCustomEntityImageData(
                                                fileUploader.FileGuid,
                                                Convert.ToInt32(fileUploader.AttributeId),
                                                entityid,
                                                (int)ViewState["id"],
                                                entity.table.GetPrimaryKey().FieldName,
                                                curUser.AccountID);
                                        var fileUploadType = fileUploader.UploadType;
                                        fileUploader.FileGuid = Guid.Empty;
                                        fileUploader.UploadType = fileUploadType;
                                    }

                                    if (fileUploader.UploadType != string.Empty)
                                    {
                                        Stream attachmentStream;
                                        string fileName;
                                        if (fileUploader.UploadType == "FileBrowser")
                                        {
                                            attachmentStream = fileUploader.PostedFile.InputStream;
                                            string[] pathSplit = fileUploader.PostedFile.FileName.Split('\\');
                                            fileName = pathSplit[pathSplit.Length - 1];
                                        }
                                        else
                                        {
                                            if (fileUploader.ImageLibrarySelectedFilePath != string.Empty)
                                            {
                                                attachmentStream =
                                                    new FileStream(
                                                        this.Server.MapPath(fileUploader.ImageLibrarySelectedFilePath),
                                                        FileMode.Open,
                                                        FileAccess.Read);
                                                fileName = fileUploader.ImageLibrarySelectedFileName;
                                            }
                                            else
                                            {
                                                fileName = "N/A";
                                                attachmentStream = null;
                                            }
                                        }

                                        string fileType = fileName.Substring(
                                            fileName.LastIndexOf('.') + 1,
                                            fileName.Length - fileName.LastIndexOf('.') - 1);

                                        using (attachmentStream)
                                        {
                                            if (attachmentStream != null)
                                            {
                                                int? delegateId = null;
                                                if (curUser.isDelegate)
                                                {
                                                    delegateId = curUser.Delegate.EmployeeID;
                                                }

                                                var attachment = new cAttachments(
                                                    curUser.AccountID,
                                                    curUser.EmployeeID,
                                                    curUser.CurrentSubAccountId,
                                                    delegateId);
                                                cMimeType mimeType = attachment.checkMimeType(fileType);

                                                if (mimeType != null && !mimeType.Archived)
                                                {
                                                    byte[] buffer = attachment.getFileData(attachmentStream);
                                                    Guid newFileID = Guid.NewGuid();
                                                    string fileId = newFileID.ToString();

                                                    attachment = new cAttachments(
                                                        entityid,
                                                        field.attribute.attributeid,
                                                        buffer,
                                                        fileId,
                                                        fileName,
                                                        fileType);
                                                    clsquery.addColumn(
                                                        clsfields.GetFieldByID(field.attribute.fieldid),
                                                        fileId);
                                                    auditEntries.Add(field.attribute.attributeid, fileId);
                                                    attachments.Add(attachment);
                                                    fileUploader.FileGuid = newFileID;
                                                    fileUploader.Changed = false;
                                                }
                                                else
                                                {
                                                    retId = (int)ReturnValues.InvalidFileType;
                                                }
                                            }
                                            else
                                            {
                                                retId = (int)ReturnValues.InvalidFile;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!fileUploader.HasFile)
                                    {
                                        // if the control already has a file guid set, then we need to delete the existing attachment data
                                        if (fileUploader.FileGuid != Guid.Empty)
                                        {
                                            cCustomEntities.DeleteCustomEntityImageData(
                                                fileUploader.FileGuid,
                                                Convert.ToInt32(fileUploader.AttributeId),
                                                entityid,
                                                (int)ViewState["id"],
                                                entity.table.GetPrimaryKey().FieldName,
                                                curUser.AccountID);
                                            fileUploader.FileGuid = Guid.Empty;
                                        }
                                    }
                                    else
                                    {
                                        if (fileUploader.FileGuid != new Guid())
                                        {
                                            clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), fileUploader.FileGuid);
                                        }
                                        else
                                        {
                                            clsquery.addColumn(clsfields.GetFieldByID(field.attribute.fieldid), DBNull.Value);
                                        }

                                        auditEntries.Add(field.attribute.attributeid, "");
                                    }
                                }

                                break;
                        }

                        if (retId < 0)
                        {
                            break;
                        }
                    }

                    if (retId < 0)
                    {
                        break;
                    }
                }
            }

            if (entity.EnableCurrencies && monetaryFieldSet == false && entity.DefaultCurrencyID != null && retId >= 0)
            {
                var currencies = new cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId);
                cCurrency currency = currencies.getCurrencyById((int)entity.DefaultCurrencyID);
                var gcurrencies = new cGlobalCurrencies();

                clsquery.addColumn(clsfields.GetFieldByID(entity.getGreenLightCurrencyAttribute().fieldid), (int)entity.DefaultCurrencyID);
                auditEntries.Add(entity.getGreenLightCurrencyAttribute().attributeid, gcurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label);
            }

            if ((int)ViewState["id"] != 0 && retId >= 0)
            {
                cAttribute modifiedby = entity.getAttributeByName("modifiedby");
                cAttribute modifiedon = entity.getAttributeByName("modifiedon");
                if (modifiedby != null)
                {
                    clsquery.addColumn(clsfields.GetFieldByID(modifiedon.fieldid), DateTime.Now);
                }
                if (modifiedon != null)
                {
                    clsquery.addColumn(clsfields.GetFieldByID(modifiedby.fieldid), (int)ViewState["employeeid"]);
                }
            }
            else
            {
                if (retId >= 0)
                {
                    cAttribute createdby = entity.getAttributeByName("createdby");
                    cAttribute createdon = entity.getAttributeByName("createdon");
                    if (createdon != null)
                    {
                        clsquery.addColumn(clsfields.GetFieldByID(createdon.fieldid), DateTime.Now);
                    }
                    if (createdby != null)
                    {
                        clsquery.addColumn(clsfields.GetFieldByID(createdby.fieldid), (int)ViewState["employeeid"]);
                    }
                }
            }

            if ((int)ViewState["id"] != 0 && retId >= 0)
            {
                id = (int)ViewState["id"];
                retId = id;

                // audit log the update of the record
                SortedList<int, object> record = clsentities.getEntityRecord(entity, id, form);

                foreach (KeyValuePair<int, string> att in auditEntries)
                {
                    int attId = att.Key;
                    string newValue = att.Value;
                    if (!record.ContainsKey(attId)) // if not in record collection, then assigning default value (most likely entity default currency)
                        continue;

                    object recValue = record[attId];
                    string oldValue = Convert.ToString(recValue);
                    cAttribute currentAttribute = entity.attributes[attId];

                    switch (currentAttribute.fieldtype)
                    {
                        case FieldType.RelationshipTextbox:
                        case FieldType.Relationship:
                            if (entity.attributes[attId].GetType() == typeof(cManyToOneRelationship))
                            {
                                var rel = (cManyToOneRelationship)entity.attributes[attId];

                                var query = new cQueryBuilder((int)ViewState["accountid"], cAccounts.getConnectionString((int)ViewState["accountid"]), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, rel.relatedtable, new cTables((int)ViewState["accountid"]), new cFields((int)ViewState["accountid"]));
                                cField acDisplayField = clsfields.GetFieldByID(rel.AutoCompleteDisplayField);
                                query.addColumn(acDisplayField);
                                // null as on related as basetable already?    !!!!!
                                query.addFilter(rel.relatedtable.GetPrimaryKey(), ConditionType.Equals, new[] { recValue }, null, ConditionJoiner.None, null);
                                using (SqlDataReader reader = query.getReader())
                                {
                                    while (reader.Read())
                                    {
                                        if (reader.IsDBNull(0)) continue;

                                        switch (acDisplayField.FieldType)
                                        {
                                            case "D":
                                                oldValue = reader.GetDateTime(0).ToString("MM/dd/yyyy");
                                                break;
                                            case "DT":
                                                oldValue = reader.GetDateTime(0).ToString("MM/dd/yyyy HH:mm");
                                                break;
                                            case "T":
                                                oldValue = reader.GetDateTime(0).ToString("HH:mm");
                                                break;
                                            case "N":
                                            case "FI":
                                                oldValue = reader.GetInt32(0).ToString();
                                                break;
                                            case "C":
                                            case "FD":
                                            case "M":
                                                oldValue = reader.GetDecimal(0).ToString();
                                                break;
                                            default:
                                                oldValue = reader.GetString(0);
                                                break;
                                        }
                                        break;
                                    }
                                    reader.Close();
                                }
                            }
                            break;
                        case FieldType.DateTime:
                            if (recValue.ToString() == "")
                            {
                                oldValue = "";
                            }
                            else
                            {
                                switch (((cDateTimeAttribute)currentAttribute).format)
                                {
                                    case AttributeFormat.DateOnly:
                                        oldValue = Convert.ToDateTime(recValue).ToString("MM/dd/yyyy");
                                        break;
                                    case AttributeFormat.TimeOnly:
                                        oldValue = Convert.ToDateTime(recValue).ToString("HH:mm");
                                        break;
                                    default:
                                        oldValue = Convert.ToDateTime(recValue).ToString("MM/dd/yyyy HH:mm");
                                        break;
                                }
                            }
                            break;
                        case FieldType.TickBox:
                            oldValue = Convert.ToString(recValue) == "True" ? "Yes" : "No";
                            break;
                        case FieldType.Currency:
                            oldValue = recValue.ToString() == "" ? "" : Convert.ToDouble(recValue).ToString("0.00");
                            break;
                        case FieldType.List:
                            if (recValue.ToString() == "")
                            {
                                oldValue = "";
                            }
                            else
                            {
                                var lstAtt = (cListAttribute)currentAttribute;

                                oldValue = (from x in lstAtt.items.Values
                                            where x.elementValue.ToString() == recValue.ToString()
                                            select x.elementText).FirstOrDefault();
                            }
                            break;
                        case FieldType.CurrencyList:
                            if (recValue.ToString() == "")
                            {
                                oldValue = "";
                            }
                            else
                            {
                                var currencies = new cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId);
                                cCurrency currency = currencies.getCurrencyById((int)recValue);
                                if (currency != null)
                                {
                                    var gcurrencies = new cGlobalCurrencies();
                                    oldValue = gcurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label;
                                }
                            }
                            break;
                    }

                    if (oldValue != newValue)
                    {
                        // if the field is defined as is_unique, then check that no other record of that value exists
                        if (entity.attributes[attId].isunique && uniqueAttributeEntries.ContainsKey(attId))
                        {
                            if (!checkValueUnique(entity, attId, id, uniqueAttributeEntries[attId]))
                            {
                                // rollback transactions and return duplicate value error
                                retId = (int)ReturnValues.AlreadyExists;
                                break;
                            }
                        }

                        cAttribute auditAtt = entity.getAuditIdentifier();
                        string auditIdentifierData = string.Empty;

                        if (auditAtt != null)
                        {
                            if (auditEntries.ContainsKey(auditAtt.attributeid))
                            {
                                auditIdentifierData = auditEntries[auditAtt.attributeid];
                            }
                            else if (auditAtt.IsSystemAttribute && auditAtt.iskeyfield)
                            {
                                auditIdentifierData = retId.ToString();
                            }
                        }
                        if (auditIdentifierData == string.Empty)
                        {
                            // Audit identifier data is of no use, display the record ID
                            auditIdentifierData = entity.entityname + " (Record ID: " + retId + ")";
                        }
                        else
                        {
                            auditIdentifierData = entity.entityname + " (" + auditIdentifierData + ")";
                        }

                        audit.editRecord(entity.entityid, auditIdentifierData, SpendManagementElement.CustomEntities, entity.attributes[attId].fieldid, oldValue, newValue);
                    }
                }

                if (attachments.Count > 0 && retId >= 0)
                {
                    foreach (var item in attachments)
                    {
                        if (item.saveAttachmentData() != 1)
                        {
                            retId = (int)ReturnValues.ErrorWithAttachmentDataSave;
                        }
                    }
                }

                if (retId >= 0)
                {
                    // null as pk so always basetable   !!!!!!
                    clsquery.addFilter(entity.table.GetPrimaryKey(), ConditionType.Equals, new object[] { id }, null, ConditionJoiner.None, null);
                    clsquery.executeUpdateStatement();
                }
            }
            else
            {
                if (retId >= 0)
                {
                    int[] uniqueAttributes = entity.getUniqueAttributes();
                    if (uniqueAttributes.Length > 0)
                    {
                        for (int x = 0; x < uniqueAttributes.Length; x++)
                        {
                            if (uniqueAttributeEntries.ContainsKey(uniqueAttributes[x]))
                            {
                                if (
                                    !checkValueUnique(
                                        entity,
                                        uniqueAttributes[x],
                                        0,
                                        uniqueAttributeEntries[uniqueAttributes[x]]))
                                {
                                    retId = (int)ReturnValues.AlreadyExists;
                                    break;
                                }
                            }
                        }
                    }

                    if (attachments.Count > 0 && retId == 0)
                    {
                        foreach (var item in attachments)
                        {
                            int retCode = 0;
                            if (item.saveAttachmentData() != 1)
                            {
                                retCode = (int)ReturnValues.ErrorWithAttachmentDataSave;
                            }

                            retId = retCode;
                        }
                    }

                    if (retId == 0)
                    {
                        if (entity.FormSelectionAttributeId > 0 && entity.FormSelectionAttributeId.HasValue && ViewState["isFormSelection" + entityid] is bool && (bool)ViewState["isFormSelection" + entityid]
                            && !clsquery.lstColumns.Any(x => x.field.FieldID == entity.getAttributeById(entity.FormSelectionAttributeId.Value).fieldid))
                        {
                            clsquery.addColumn(clsfields.GetFieldByID(entity.getAttributeById(entity.FormSelectionAttributeId.Value).fieldid), ViewState["FormSelectionValue" + entityid]);
                        }

                        id = clsquery.executeInsertStatement();

                        if (id > 0)
                        {
                            cAttribute auditAtt = entity.getAuditIdentifier();
                            string auditIdentifierData = string.Empty;

                            if (auditAtt != null)
                            {
                                if (auditEntries.ContainsKey(auditAtt.attributeid))
                                {
                                    auditIdentifierData = auditEntries[auditAtt.attributeid];
                                }
                                else if (auditAtt.IsSystemAttribute && auditAtt.iskeyfield)
                                {
                                    auditIdentifierData = id.ToString();
                                }
                            }

                            if (auditIdentifierData == string.Empty)
                            {
                                auditIdentifierData = "Record ID: " + id;
                            }

                            auditIdentifierData = entity.entityname + " (" + auditIdentifierData + ")";
                            audit.addRecord(SpendManagementElement.CustomEntities, auditIdentifierData, id);
                        }

                        retId = id;
                        if (entity.EnableAttachments)
                        {
                            this.UpdateNewAttachmentsToCurrentId(entityid, id);
                        }
                    }
                }
            }

            return retId;
        }

        /// <summary>
        /// The update new attachments which are saved as id = zero to current id.
        /// </summary>
        /// <param name="entityid">
        /// The entity id.
        /// </param>
        /// <param name="id">
        /// The id to update to.
        /// </param>
        private void UpdateNewAttachmentsToCurrentId(int entityid, int id)
        {
            CurrentUser currUser = cMisc.GetCurrentUser();
            var attachments = new cAttachments(currUser.AccountID, currUser.EmployeeID, currUser.CurrentSubAccountId, currUser.isDelegate ? (int?)currUser.Delegate.EmployeeID : null);
            attachments.UpdateNewAttachmentsWithRealId(entityid, id);
            attachments.RemoveOrphanAttachmentsOnCustomTables(entityid, currUser.EmployeeID, DateTime.Now.AddHours(-1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdChildOK_Click(object sender, CommandEventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            CSSButton btn = (CSSButton)sender;
            int entityid, formid, buttonType;
            int nResult = 0;
            string[] tmp = btn.CommandArgument.Split(',');
            entityid = Convert.ToInt32(tmp[0]);
            formid = Convert.ToInt32(tmp[1]);
            buttonType = Convert.ToInt32(tmp[2]);
            nResult = SaveCustomEntity(entityid, formid);

            #region Save Messages
            Panel pnlModal = new Panel();
            pnlModal.ID = "pnlSaveModal";
            pnlModal.CssClass = "errorModal";
            pnlWorkflowHolder.Controls.Add(pnlModal);
            Literal litModal = new Literal();
            litModal.ID = "litSaveModal";
            pnlModal.Controls.Add(litModal);
            HyperLink hlModal = new HyperLink();
            hlModal.Style.Add(HtmlTextWriterStyle.Display, "none");
            hlModal.ID = "hlSaveModal";
            pnlWorkflowHolder.Controls.Add(hlModal);
            ModalPopupExtender mpe = new ModalPopupExtender();
            mpe.ID = "mpeSaveMessage";
            mpe.TargetControlID = hlModal.ID;
            mpe.PopupControlID = pnlModal.ID;
            mpe.BackgroundCssClass = "modalMasterBackground";
            mpe.Show();
            pnlWorkflowHolder.Controls.Add(mpe);
            StringBuilder sb = new StringBuilder();
            cModules clsModules = new cModules();
            cModule module = clsModules.GetModuleByID((int)curUser.CurrentActiveModule);
            string sModuleName = "Message From " + module.BrandNameHTML;

            switch (nResult)
            {
                case (int)ReturnValues.AlreadyExists:
                    sb.Append(
                        "<div class=\"errorModalSubject\">" + sModuleName + "</div><br /><div class=\"errorModalBody\">Cannot save record as one of the field values already exists in another record.</div>");
                    nResult = 0; // if remains as -1, will cause an error
                    break;
                case 0:
                    sb.Append(
                        "<div class=\"errorModalSubject\">" + sModuleName + "</div><br /><div class=\"errorModalBody\">Unknown problem encountered during save. The record was not saved.</div>");
                    break;
                default:
                    cCustomEntities clsentities = new cCustomEntities(curUser);
                    cCustomEntityForm form = clsentities.getEntityById(entityid).getFormById(formid);
                    sb.Append(
                        "<div class=\"errorModalSubject\">" + sModuleName + "</div><br /><div class=\"errorModalBody\">The save was successful.</div>");
                    ViewState["id"] = nResult;
                    createBreadcrumbTree((List<sEntityBreadCrumb>)ViewState["entitycrumbs"], (int)ViewState["viewid"]);
                    Master.PageSubTitle = cMisc.DotonateString(form.formname, 36);
                    break;
            }
            sb.Append("<div style=\"padding-bottom: 5px; padding-left: 5px; padding-right: 5px; padding-top: 0px;\"><img style=\"cursor: pointer;\" src=\"/shared/images/buttons/btn_close.png\" title=\"Close\" onclick=\"$find('" + mpe.ClientID + "').hide();\"/></div>");
            litModal.Text = sb.ToString();
            #endregion Save And New Messages
        }

        /// <summary>
        /// Construct tree structure to indicate where in custom entities the user is (like menu breadcrumbs)
        /// </summary>
        /// <param name="crumbs">List of entity breadcrumbs to indicate current level and form</param>
        /// <param name="currentViewID">ID of the Custom Entity View for which the Breadcrumb tree is to be created</param>
        private void createBreadcrumbTree(List<sEntityBreadCrumb> crumbs, int currentViewID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);

            int maxBreadCrumbStringLength = Convert.ToInt16(ConfigurationManager.AppSettings["CharacterDottingLength"].ToString());

            StringBuilder sb = new StringBuilder();
            int crumbIndex = 0;

            sb.Append("<ol class=\"breadcrumb\">");

            // create our path back to the home page
            sb.Append("<li><a title=\"Home page\" href=\"");
            sb.Append(cMisc.Path);
            sb.Append("/Home.aspx\"> <i><img src=\"/static/images/expense/menu-icons/bradcrums-dashboard-icon.png\" alt=\"\"></i>Home</a></li>");

            // need to detect menu where in the menu structure the entity is accessed from to create the correct breadcrumb start
            cCustomEntity rootE = clsentities.getEntityById(crumbs[0].EntityID);
            sb.Append(clsentities.GetInnerBreadcrumbs(rootE, crumbs[0].ViewID));

            //create our path back 
            sb.Append("<li><a title=\"");
            sb.Append(rootE.description);
            sb.Append("\" href=\"");
            sb.Append("viewentities.aspx?entityid=");
            sb.Append(rootE.entityid);
            sb.Append("&viewid=");
            sb.Append(crumbs[0].ViewID);
            sb.Append("\"\\><label class=\"breadcrumb_arrow\">/</label>");
            var view = rootE.getViewById(crumbs[0].ViewID);
            if (view != null)
            {
                sb.Append(view.viewname);
            }

            sb.Append("<label class=\"breadcrumb_arrow\">/</label></a> </li>");

            foreach (sEntityBreadCrumb crumb in crumbs)
            {
                cCustomEntity entity = clsentities.getEntityById(crumb.EntityID);
                cAttribute reqAttribute = entity.getAuditIdentifier();

                // If there is no audit identifier, use the defaults
                if (reqAttribute == null)
                {
                    if (crumb.RecordID == 0)
                    {
                        sb.Append(entity.entityname + " (New)");
                    }
                    else
                    {
                        sb.Append(entity.entityname);
                    }
                }
                else
                {
                    SortedList<int, object> rec = clsentities.getEntityRecord(entity, crumb.RecordID, entity.getFormById(crumb.FormID));

                    // construct the hyperlink url for the breadcrumb
                    string returnurl = cMisc.Path + "/shared/aeentity.aspx?";
                    string entityurl = "";
                    string formurl = "";
                    string recordurl = "";
                    string taburl = "";
                    for (int i = 0; i < crumbIndex; i++)
                    {
                        entityurl += "relentityid=" + crumbs[i].EntityID + "&";
                        formurl += "relformid=" + crumbs[i].FormID + "&";
                        recordurl += "relrecordid=" + crumbs[i].RecordID + "&";
                        taburl += "reltabid=" + crumbs[i].TabID + "&";
                        taburl += "relviewid=" + crumbs[i].ViewID + "&";
                    }
                    returnurl += entityurl + formurl + recordurl + taburl + "viewid=" + crumbs[crumbIndex].ViewID +
                                 "&entityid=" + crumbs[crumbIndex].EntityID + "&formid=" + crumbs[crumbIndex].FormID +
                                 "&tabid=" + crumbs[crumbIndex].TabID + "&id=" + crumbs[crumbIndex].RecordID;

                    int auditAttributeId = reqAttribute.attributeid;
                    string recStr = "";

                    if (rec.ContainsKey(auditAttributeId))
                    {
                        string fieldData = clsentities.formatFieldData(reqAttribute,
                                                                       rec[auditAttributeId].ToString());

                        if (fieldData != string.Empty)
                        {
                            recStr = fieldData;
                        }
                    }
                    else if (reqAttribute.IsSystemAttribute && reqAttribute.iskeyfield && crumb.RecordID > 0)
                    {
                        recStr = crumb.RecordID.ToString();
                    }

                    if (recStr != string.Empty)
                    {
                        if (crumbIndex < crumbs.Count - 1)
                        {
                            sb.Append("<li><a href=\"");
                            sb.Append(returnurl);
                            sb.Append("\"");
                            sb.Append(" title=\"");
                            sb.Append(recStr);
                            sb.Append(
                                "\" alt=\"\"><label class=\"breadcrumb_arrow\">/</label>");
                        sb.Append(entity.entityname + " (" + cMisc.DotonateString(recStr, maxBreadCrumbStringLength) +
                                  ")");
                    }
                    else
                    {
                            sb.Append("<li>" + entity.entityname + " (" + cMisc.DotonateString(recStr, maxBreadCrumbStringLength) +
                                  ")</li>");
                        }
                    }
                    else
                    {
                        // If the audit identifier information is empty, use the defaults
                        if (crumb.RecordID == 0)
                        {
                            sb.Append("<li>" + entity.entityname + " (New)</li>");
                        }
                        else
                        {
                            sb.Append("<li>" + entity.entityname + "</li>");
                        }
                    }
                }

                if (crumbIndex < crumbs.Count - 1)
                {
                    sb.Append("</a> </li>");
                }

                crumbIndex++;
            }

            sb.Append(@"</ol>");
            Master.EntityBreadCrumbText = sb.ToString();
        }

        private bool checkValueUnique(cCustomEntity entity, int attId, int recordId, string newValue)
        {
            if (newValue.Length > 0)
            {
                bool isUnique = true;
                ViewState["uniqueAttributeId"] = null;
                cFields fields = new cFields((int)ViewState["accountid"]);
                cQueryBuilder query = new cQueryBuilder((int)ViewState["accountid"],
                                                        cAccounts.getConnectionString((int)ViewState["accountid"]),
                                                        ConfigurationManager.ConnectionStrings["metabase"].
                                                            ConnectionString, entity.table,
                                                        new cTables((int)ViewState["accountid"]), fields);

                query.addColumn(entity.table.GetPrimaryKey(), SelectType.Count);

                cField field = fields.GetFieldByID(entity.attributes[attId].fieldid);

                query.addFilter(field, ConditionType.Equals, new object[] { newValue }, null, ConditionJoiner.None, null);
                // null as field appears to be on query basetable all the time
                if (recordId > 0)
                {
                    query.addFilter(entity.table.GetPrimaryKey(), ConditionType.DoesNotEqual, new object[] { recordId }, null,
                                    ConditionJoiner.And, null); // null as pk on bt ?    !!!!!!
                }

                if (query.GetCount() > 0)
                {
                    ViewState["uniqueAttributeId"] = attId;
                    isUnique = false;
                }

                return isUnique;
            }

            return true;
        }

        /// <summary>
        /// Saves the uploaded image to disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ajaxUploadEvents">Contains the uploaded file data</param>       
        protected void savefile(object sender, AjaxFileUploadEventArgs e)
        {
            NewRelic.DisableBrowserMonitoring(true);
            string fileExtension = Path.GetExtension(e.FileName);
            string guidFileName = Guid.NewGuid() + fileExtension;
            string fullpath = ConfigurationManager.AppSettings["HostedEntityImageLocation"] + "\\" + guidFileName;
            string imagePath = string.Empty;

            //Save the file to temporary directory 
            this.editor.AjaxFileUpload.SaveAs(fullpath);

            //Update HTMLeditor control with saved image path
            var sharedDirectory = ConfigurationManager.AppSettings["HostedEntityImageLocation"].Split('\\');
            var sharedURL = sharedDirectory.LastOrDefault();
            if (sharedURL != null)
            {
                imagePath = GetImagePath(cMisc.GetCurrentUser(), sharedURL);
            }

            imagePath += guidFileName;

            e.PostedUrl = Page.ResolveUrl(imagePath);
        }

        /// <summary>
        /// Process html image tags
        /// </summary>
        /// <param name="htmlstring">HTML string we are processing</param>
        /// <param name="entityID"></param>
        /// <param name="attributeID"></param>
        /// <param name="buttonType">Source of request i.e. Save, Save and Duplicate</param>
        /// <returns>
        ///formatted HTML string
        /// </returns>
        private string formatImageTags(string htmlString, int entityID, int attributeID, int buttonType)
        {
            var matches = HtmlImgMatches(htmlString);

            foreach (Match match in matches)
            {
                var imageTag = match.Value;

                if (imageTag.Contains(@"/>") == false)
                {
                    //add missing img tag break
                    imageTag = imageTag.Replace(">", "/>");
                }

                htmlString = htmlString.Replace(match.Value, imageTag);

                string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"">]+?)['""][^>]*?>";
                MatchCollection matchesImgSrc = Regex.Matches(imageTag, regexImgSrc,
                                                              RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in matchesImgSrc)
                {
                    string dataString;
                    string href = m.Groups[1].Value;

                    //check if we need to process image tag
                    if (href.Contains("SoftwareEurope?=".ToLower()) == false)
                    {
                        string fileID, fileType;
                        string[] fileDetails;

                        fileDetails = Path.GetFileName(href).Split('.');
                        fileID = fileDetails[0];
                        fileType = fileDetails[1];
                        byte[] imgByte = GetBinary(fileID, fileType);

                        if (saveImageData(entityID, attributeID, imgByte, fileID, fileType))
                        {
                            dataString = "SoftwareEurope?=".ToLower() + Convert.ToString(fileID) + "." + fileType;
                            htmlString = htmlString.Replace(href, dataString);

                            //delete images from temp location folder if Save or Save and New is clicked.
                            if (buttonType > 1)
                            {
                                var entities = new cCustomEntities();
                                entities.deleteTemporaryImages(fileID, fileType);
                            }
                        }
                    }
                }
            }
            return htmlString;
        }


        /// <summary>
        /// Finds all matches for the html img tag.
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns>
        /// A collection of matches for the html img tag
        /// </returns> 
        private static MatchCollection HtmlImgMatches(string htmlString)
        {
            var matches = Regex.Matches(htmlString, @"<img([\w\W]+?)>");
            return matches;
        }

        /// <summary>
        /// Converts the image in to binary for storage in the database
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>
        /// binary image data
        /// </returns> 
        public static byte[] GetBinary(string filename, string filetype)
        {
            // var filePath = HttpContext.Current.Server.MapPath(path);

            var filePath = ConfigurationManager.AppSettings["HostedEntityImageLocation"] + "\\" + filename + "." + filetype;

            byte[] buffer = new byte[0];

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {

                    buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, (int)fileStream.Length);
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry("Failed to read image\n\n" + ex.Message);
            }

            return buffer;
        }

        /// <summary>
        ///Saves the image data to the database
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="attributeID"></para>
        /// <param name="imgByte">Path to file</param>
        /// <param name="FileID"></param>
        /// <param name="fileType"></param>
        /// <param name="fileName"></param>
        /// <returns>
        /// result of save
        /// </returns> 
        public static bool saveImageData(int entityID, int attributeID, byte[] imgByte, string FileID, string fileType, string fileName = "")
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsentities = new cCustomEntities(user);
            int outcome;

            outcome = clsentities.SaveCustomeEntityImageData(entityID, attributeID, imgByte, FileID, fileType, fileName, user.AccountID);

            if (outcome == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the global Static Library Path
        /// </summary>
        /// <returns></returns>
        public string GetStaticLibPath()
        {
            return GlobalVariables.StaticContentLibrary;
        }


        /// <summary>
        /// Extracts the FileID of the attachments that make up the form and passes to DuplicateCustomEntityAttributeAttachmentData. 
        /// </summary>
        /// <param name="form"></param>
        private void GenerateAttachmentData(cCustomEntityForm form)
        {
            TabContainer tabContainer = (TabContainer)this.holderForm.FindControl("tabs" + form.formid);

            SortedList<byte, cCustomEntityFormTab> tabList = form.getTabsForForm();

            foreach (cCustomEntityFormTab currentTab in tabList.Values)
            {
                TabPanel tab = tabContainer.Tabs[tabList.IndexOfValue(currentTab)];

                foreach (cCustomEntityFormSection section in currentTab.sections)
                {
                    foreach (cCustomEntityFormField field in section.fields)
                    {
                        if (field.attribute.fieldtype == FieldType.Attachment)
                        {
                            var fileUploader = (FileUploader)tab.FindControl(field.attribute.ControlID);
                            Guid fileID = fileUploader.FileGuid;
                            CurrentUser user = cMisc.GetCurrentUser();
                            var clsentities = new cCustomEntities(user);
                            Guid outcome;
                            outcome = clsentities.DuplicateCustomEntityAttributeAttachmentData(fileID);

                            if (outcome != Guid.Empty)
                            {
                                fileUploader.FileGuid = outcome;
                                fileUploader.Changed = false;
                            }
                            else
                            {
                                {
                                    fileUploader.FileGuid = Guid.Empty;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ClearLock()
        {
            if (aelocking.Active && !aelocking.Locked)
            {
                var entityId = (int)ViewState["entityid"];
                var id = (int)ViewState["id"];
                CustomEntityRecordLocking.UnlockElement(entityId, id, cMisc.GetCurrentUser());
            }
        }
    }
}
