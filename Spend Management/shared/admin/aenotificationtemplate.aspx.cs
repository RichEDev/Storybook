namespace Spend_Management
{
    using System;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using Spend_Management.shared.webServices;
    using Newtonsoft.Json;

    using SpendManagementLibrary.Helpers;

    public partial class aenotificationtemplate : System.Web.UI.Page
    {

        #region properties

        public int accountid
        {
            get
            {
                if (ViewState["accountid"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["accountid"];
                }
            }
            set { ViewState["accountid"] = value; }
        }

        public int templateID
        {
            get
            {
                if (ViewState["emailtemplateID"] == null)
                {
                    return 0;
                }

                return (int)ViewState["emailtemplateID"];

            }

            set { ViewState["emailtemplateID"] = value; }
        }

        public Guid areaTableID
        {
            get
            {
                if (ViewState["areaTableID"] == null)
                {
                    return Guid.Empty;
                }

                return (Guid)ViewState["areaTableID"];

            }

            set
            {
                ViewState["areaTableID"] = value;
            }
        }

        /// <summary>
        /// Get Tree node based on base table id
        /// </summary>
        /// <param name="baseTable">Base talbe id</param>
        /// <param name="reportService">An instance of the webservice svcReports</param>
        /// <returns>The JSON string of the tree data.</returns>
        private string AddNodesInWebTree(string baseTable, svcReports reportService)
        {
            var lstNodes = reportService.GetEasyTreeNodes(baseTable);
            return JsonConvert.SerializeObject(lstNodes);
        }


        public Action action
        {
            get
            {
                if (ViewState["action"] == null)
                {
                    ViewState["action"] = Action.Add;
                }
                return (Action)ViewState["action"];
            }

            set { ViewState["action"] = value; }
        }


        #endregion
        
        protected void Page_Load(object sender, EventArgs e)
        {
         
            Master.PageSubTitle = "Notification Template Details";
            Master.enablenavigation = false;

            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Emails, true, true);

            this.accountid = user.AccountID;

            if (Request.QueryString["templateid"] != null)
            {
                user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Emails, true, true);
                this.templateID = Convert.ToInt32(Request.QueryString["templateid"]);
            }
            else
            {
                user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Emails, true, true);
                Title = @"Notification Template: New";
            }

            Master.title = Title;

            if (Request.QueryString["action"] != null)
            {
                this.action = (Action)Convert.ToByte(Request.QueryString["action"]);
            }
            if (Request.QueryString["areaTableID"] != null)
            {
                this.areaTableID = new Guid(Request.QueryString["areaTableID"]);
            }

            var isAdmin = user.Employee.AdminOverride;

            rtBodyText.BasePath = GlobalVariables.StaticContentLibrary + "/ckeditor";
            this.rtBodyText.CssClass = "easytree-droppable";
            if (isAdmin) CKEditorExtensions.Configure(rtBodyText, EditorMode.ShortNoIFrameSelAdmin);
            else CKEditorExtensions.Configure(rtBodyText, EditorMode.ShortNoIFrame);
            this.rtBodyText.Height = 560;

            txtNotes.BasePath = GlobalVariables.StaticContentLibrary + "/ckeditor";
            this.txtNotes.CssClass = "easytree-droppable";
            if (isAdmin) CKEditorExtensions.Configure(txtNotes, EditorMode.ShortNoIFrameSelAdmin);
            else CKEditorExtensions.Configure(txtNotes, EditorMode.ShortNoIFrame);

            //Creating jstree css path and add ref in page header
            var customurl = GlobalVariables.StaticContentLibrary + "/JsTreeThemes/default/style.min.css";
            Literal cssFile = new Literal() { Text = @"<link href=""" + this.ResolveUrl(customurl) + @""" type=""text/css"" rel=""stylesheet"" />" };
            this.Page.Header.Controls.Add(cssFile);
            FillAreaDropDown(user);
            if (!IsPostBack)
            {
                var reportService = new svcReports();

                this.employeeTreeData.Value = this.AddNodesInWebTree("618db425-f430-4660-9525-ebab444ed754", reportService);
                
                NotificationTemplates notificationTemplates = new NotificationTemplates(user);
                cTables clstables = new cTables(accountid);
                bool update = false;
                this.areaTableID = new Guid(cmbArea.SelectedValue);
                cTable tblattachments = clstables.GetTableByName("emailTemplate_attachments");
                cField field = tblattachments.GetKeyField();
                usrAttachments.IDField = field.FieldName;
                usrAttachments.RecordID = 0;
                usrAttachments.TableName = tblattachments.TableName;
                usrAttachments.iFrameName = "iFrEmailAttach";
                usrAttachments.MultipleAttachments = true;
                
                if (!isAdmin) this.divSystemTemplate.Style.Add("display", "none");
               
                this.notesHeader.Style.Add("display", "none");
                this.noteswrapper.Style.Add("display", "none");
                this.ChkSendNote.Style.Add("display", "none");
                this.lblSendNote.Style.Add("display", "none");

                this.sendMobileNotifcationCheckboxDiv.Style.Add("display", "none");        
                this.chkCanEmailNotification.Style.Add("display", "none");
                this.lblSendMobileNotification.Style.Add("display", "none");
                this.mobileNotificationHeader.Style.Add("display", "none");            
                this.mobileNotificationWrapper.Style.Add("display", "none");

                if (ViewState["emailtemplateID"] != null)
                {
                    update = true;
                    cmbArea.Enabled = false;
                    usrAttachments.RecordID = (int)ViewState["emailtemplateID"];
                    NotificationTemplate notification = notificationTemplates.GetNotificationTemplateById((int)ViewState["emailtemplateID"]);
                    if (notification != null)
                    {
                        areaTableID = notification.BaseTableId;
                        lblTitle.Text = @"General Details";

                        if (notification.SystemTemplate)
                        {
                            this.rtBodyText.Height = 200;
                            this.notesHeader.Style.Add("display", "block");
                            this.noteswrapper.Style.Add("display", "block");
                            this.ChkSendNote.Style.Add("display", "inline-block");
                            this.lblSendNote.Style.Add("display", "inline-block");
                        }

                        if (notificationTemplates.PermittedMobileNotificationTemplateIds.Contains(notification.TemplateId))
                        {        
                            //Show Mobile Notification email template options
                            this.sendMobileNotifcationCheckboxDiv.Style.Add("display", "block"); 
                            this.chkCanEmailNotification.Style.Add("display", "inline-block");
                            this.lblSendMobileNotification.Style.Add("display", "inline-block");
                            this.mobileNotificationWrapper.Style.Add("display", "block");
                            this.mobileNotificationHeader.Style.Add("display", "block");  
                        }

                        var master = ((smForm)this.Page.Master);

                        if (master != null)
                        {
                            var title = "Notification Template: " + notification.TemplateName;
                            master.title = title;
                        }

                        var defaultSelectedItem = this.cmbArea.Items.FindByValue(this.cmbArea.SelectedValue);
                        this.SetBaseTitle(defaultSelectedItem.Text);
                        var item = this.cmbArea.Items.FindByValue(this.areaTableID.ToString());
                        if (item != null)
                        {
                            if (defaultSelectedItem != null)
                            {
                                defaultSelectedItem.Selected = false;
                            }
                            item.Selected = true;
                            this.SetBaseTitle(item.Text);
                        }


                        var notificationTemplatesService = new svcNotificationTemplates();
                        var result = notificationTemplatesService.GetGreenLightAttributes(this.areaTableID.ToString());
                        this.cmbGreenLightAttribute.Items.Clear();
                        this.cmbGreenLightAttribute.Items.Insert(0, new ListItem("[None]", Convert.ToString(Guid.Empty)));
                        foreach (CustomEntityEmailAttribute customEntityEmailAttribute in result)
                        {
                            var listItem = new ListItem(customEntityEmailAttribute.Name, customEntityEmailAttribute.Id.ToString());
                            if (!string.IsNullOrEmpty(customEntityEmailAttribute.Owner))
                            {
                                listItem.Attributes.Add("data-category", customEntityEmailAttribute.Owner);
                            }

                            this.cmbGreenLightAttribute.Items.Add(listItem);
                        }
                        
                        this.txtTemplateName.Text = notification.TemplateName;
                        if (notification.RecipientTypes != null)
                        {
                            foreach (sSendDetails sendDet in notification.RecipientTypes)
                            {
                                switch (sendDet.recType)
                                {
                                    case recipientType.to:
                                        hdnTo.Value += sendDet.sender + "; ";
                                        break;
                                    case recipientType.cc:
                                        hdnCC.Value += sendDet.sender + "; ";
                                        break;
                                    case recipientType.bcc:
                                        hdnBCC.Value += sendDet.sender + "; ";
                                        break;
                                }
                            }
                        }

                        this.cmbPriority.Items.FindByText(notification.Priority.ToString()).Selected = true;

                        this.Page.ClientScript.RegisterClientScriptBlock(
                            this.GetType(),
                            "variables",
                            string.Format("var isSystemTemp = true;", notification.SystemTemplate),
                            true);

                        this.rtBodyText.Text = notification.Body.details;
                        this.txtNotes.Text = notification.Note.details;
                        this.PopulateSubject(notification.Subject.details);
                        this.bodyHtml.Value = notification.Body.details;
                        this.noteHtml.Value = notification.Note.details;
                        this.txtMobileNotificationMessage.Text = notification.MobileNotificationMessage;

                        this.chkSystemTemplate.Checked = notification.SystemTemplate;

                        this.ChkSendNote.Checked = notification.SendNote.HasValue ? notification.SendNote.Value : false;
                        this.ChkSendEmail.Checked = notification.SendEmail.HasValue ? notification.SendEmail.Value : false;

                        this.chkCanEmailNotification.Checked = notification.CanSendMobileNotification.HasValue
                                                                   ? notification.CanSendMobileNotification.Value
                                                                   : false;

                        this.baseTreeData.Value = this.AddNodesInWebTree(
                            Convert.ToString(this.areaTableID),
                            reportService);
                    }
                }
                else
                {
                    switch (user.CurrentActiveModule)
                    {
                        case Modules.SpendManagement:
                        case Modules.SmartDiligence:
                        case Modules.contracts:
                            // Set default report base to contract details
                            this.SetDefaultSelectedItem(new Guid("998e51fa-2c23-467e-b90f-75c44d1838bc"));
                            this.baseTreeData.Value = this.AddNodesInWebTree("998e51fa-2c23-467e-b90f-75c44d1838bc", reportService);
                            break;
                        case Modules.expenses:
                            // Set default report base to expenses
                            this.SetDefaultSelectedItem(new Guid("d70d9e5f-37e2-4025-9492-3bcf6aa746a8"));
                            this.baseTreeData.Value = this.AddNodesInWebTree("d70d9e5f-37e2-4025-9492-3bcf6aa746a8", reportService);
                            break;
                    }
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "variables", "var isSystemTemp = false;", true);
                }

                usrAttachments.createGrid();

                //Recipient team dropdown
                var clsteams = new cTeams(accountid);
                cmbTeam.Items.AddRange(clsteams.CreateDropDown(-1));
                cmbTeam.Items.Insert(0, new ListItem("[None]", "0"));

                //Recipient Budget holder dropdown
                var clsbudget = new cBudgetholders(accountid);
                cmbBudget.Items.AddRange(clsbudget.CreateDropDown().ToArray());
                cmbBudget.Items.Insert(0, new ListItem("[None]", "0"));

                if (ViewState["emailtemplateID"] != null)
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "notificationtemplateid ", "var notificationtemplateid  = " + (int)ViewState["emailtemplateID"] + ";", true);
                }
                else
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "notificationtemplateid ", "var notificationtemplateid  = 0;", true);
                }

                if (ViewState["areaTableID"] != null)
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "table", "var tableid = '" + (Guid)ViewState["areaTableID"] + "';", true);
                }
                else
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "table", "var tableid = '" + Guid.Empty + "';", true);
                }

                if (update)
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "update", "var update = true;", true);
                }
                else
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "update", "var update = false;", true);
                }

                reportService.Dispose();
            }

        }

        private void PopulateSubject(string details)
        {
            var subjectText = details;
            var subjectSpans = new StringBuilder();
            var regex = new Regex(@"<span.*?\[.*?\].*?<\/span>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(details);
            foreach (Match match in matches)
            {
                subjectSpans.Append(match.Value);
                var placeHolderRegex = new Regex(@"\[.*?\]", RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(match.Value);
                subjectText = subjectText.Replace(match.Value, placeHolderRegex.Value);
            }

            this.rtSubject.Text = subjectText;
            this.subjectFields.InnerHtml = subjectSpans.ToString();
        }

        /// <summary>
        /// Fills dropdownlist with Area
        /// </summary>
        /// <param name="user">Current user object</param>
        private void FillAreaDropDown(CurrentUser user)
        {
            var clsTables = new cTables(user.AccountID);
            if (!string.IsNullOrEmpty(this.cmbArea.SelectedValue))
            {
                this.areaTableID = new Guid(this.cmbArea.SelectedValue);
            }

            this.cmbArea.Items.Clear();

            var listItemFactory = new ListItemFactory(user, new cCustomEntities(user));

            var listArea =  listItemFactory.CreateList(clsTables.GetItemsForReportDropDown(user.CurrentActiveModule));

            var listAreaItems = new List<ListItem>();

            foreach (var listItem in listArea)
            {
                if (listItem.Attributes["data-category"] == "Expenses" &&
                    (listItem.Text == @"Employees" || listItem.Text == @"Expense Items" ||
                     listItem.Text == @"Vehicles" || listItem.Text == @"Advances" || listItem.Text == @"Card Providers" ||
                     listItem.Attributes["customEntity"] == "True"))
                {
                    listAreaItems.Add(listItem);
                }
                else if (listItem.Attributes["data-category"] != "Expenses")
                {
                    listAreaItems.Add(listItem);
                }
            }

            this.cmbArea.Items.AddRange(listAreaItems.ToArray());

            var selectedItem = this.cmbArea.SelectedItem;
            selectedItem.Selected = false;

            var item = this.cmbArea.Items.FindByValue(this.areaTableID.ToString());
            if (item != null) item.Selected = true;

            this.cmbGreenLightAttribute.Items.Insert(0, new ListItem("[None]", Convert.ToString(Guid.Empty)));
        }
        
        /// <summary>
        /// Select item based on base table id
        /// </summary>
        /// <param name="tableId">Base table id</param>
        private void SetDefaultSelectedItem(Guid tableId)
        {
            this.cmbArea.SelectedIndex = -1;
            if (this.cmbArea.Items.FindByValue(tableId.ToString()) != null)
            {
                this.cmbArea.Items.FindByValue(tableId.ToString()).Selected = true;
                this.SetBaseTitle(this.cmbArea.SelectedItem.Text);
            }
        }

        private void SetBaseTitle(string title)
        {
            var selectedtext = title;
            if (selectedtext.Length > 17)
            {
                this.baseTreeHeader.Title = selectedtext;
                selectedtext = selectedtext.Substring(0, 17) + "...";
            }
            else
            {
                this.baseTreeHeader.Title = "";
            }
            this.baseTreeHeader.InnerText = selectedtext;
        }

    }
}
