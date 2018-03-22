namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.Script.Services;
    using System.Web.Services;
    using System.Web.UI.WebControls;

    using SpendManagementHelpers.TreeControl;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.GreenLight;

    /// <summary>
    /// Summary description for svcCustomEntities
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class svcCustomEntities : WebService
    {
        #region Public Methods

        /// <summary>
        /// Performs deletion of an individual custom entity data record
        /// </summary>
        /// <param name="entityid">ID of the record's parent entity</param>
        /// <param name="recordid">ID of the record to be deleted</param>
        /// <param name="viewid">ID of the view the record belongs to</param>
        /// <param name="attributeid">ID of the custom entity form</param>
        /// <returns>ID and first affected tab of the deleted record</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] deleteCustomEntityRecord(int entityid, int recordid, int viewid, int attributeid)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            
            if (!curUser.CheckAccessRole(AccessRoleType.Delete, CustomEntityElementType.View, entityid, viewid, false))
            {
                return new object[] { null, -2, null };
            }

            cCustomEntities clsEntities = new cCustomEntities(curUser);
            cCustomEntity curEntity = clsEntities.getEntityById(entityid);
            if (!curEntity.getViewById(viewid).allowdelete)
            {
                return new object[] { null, -3, null };
            }
            string sGridID = "grid" + entityid.ToString() + viewid.ToString() + attributeid.ToString();
            DataSet dsGrid = (DataSet)Session["cGridNewDataset_" + sGridID];
            Object[] ret = cCustomEntities.DeleteCustomEntityRecord(entityid, recordid, viewid, attributeid, curUser, ref dsGrid);
            Session["cGridNewDataset_" + sGridID] = dsGrid;
            return ret;
        }

        /// <summary>
        /// Performs archival of an individual custom entity data record
        /// </summary>
        /// <param name="entityid">ID of the record's parent entity</param>
        /// <param name="recordid">ID of the record to be archived</param>
        /// <param name="viewid">ID of the view the record belongs to</param>
        /// <param name="attributeid">ID of the custom entity form</param>
        /// <param name="archived">archived status of the record</param>
        /// <returns>ID and first affected tab of the archived record</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[]  ArchiveCustomEntityRecord(int entityid, int recordid, int viewid,string archived, int attributeid)
        {
            bool archive = (archived == "False") ? false : true;           
            CurrentUser currentUser = cMisc.GetCurrentUser();
             var entities = new cCustomEntities(currentUser);

            if (!currentUser.CheckAccessRole(AccessRoleType.Edit, CustomEntityElementType.View, entityid, viewid, false))
            {
                return new object[] { null, ErrorCodeArchive.NoArchiveAccess, null };
            }

            cCustomEntities clsEntities = new cCustomEntities(currentUser);
            cCustomEntity curEntity = clsEntities.getEntityById(entityid);
            if (!curEntity.getViewById(viewid).allowarchive)
            {
                return new object[] { null, ErrorCodeArchive.AllowArchiveOnViewDisabled, null };
            }
            string sGridID = "grid" + entityid.ToString() + viewid.ToString() + attributeid.ToString();           
            int successCode = entities.ArchiveCustomEntityRecord(entityid, attributeid, recordid, viewid, !archive);

            if (successCode != 0)
            {
                return new object[] { null, ErrorCodeArchive.ErrorOnArchive, null }; 
            }
            else
            {
                int firstaffectedtabid = cCustomEntities.getAffectedTabIds(attributeid, currentUser.AccountID).FirstOrDefault();               
                return new object[] { sGridID, recordid, firstaffectedtabid };
            }        

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cWorkflowNextStep runWorkflow(int workflowID, int recordID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cWorkflows clsWorkflows = new cWorkflows(currentUser);

            if (clsWorkflows.EntityInWorkflow(recordID, workflowID) == false)
            {
                clsWorkflows.InsertIntoWorkflow(recordID, workflowID, currentUser.EmployeeID);
            }

            cWorkflowNextStep reqNextStep = clsWorkflows.GetNextWorkflowStep(recordID, workflowID);

            if (reqNextStep == null)
            {
                return null;
            }
            else
            {
                switch (reqNextStep.NextStep.Action)
                {
                    case WorkFlowStepAction.ChangeCustomEntityForm:
                    case WorkFlowStepAction.ShowMessage:
                        return reqNextStep;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Decrypts the Entity details and passes to the generateOneToManyGrid method for processing.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>An object of the grid details</returns>
        [WebMethod(EnableSession = true)]
        public object[] getOTMTable(string data)
        {
            var secureData = new cSecureData();
            var decryptedEntityDetails = secureData.Decrypt(data);
            decryptedEntityDetails = decryptedEntityDetails.Replace("'", "");
            string[] entityDetailsSplit = decryptedEntityDetails.Split(',');

            int entityId = Convert.ToInt32(entityDetailsSplit[0]);
            int attributeId = Convert.ToInt32(entityDetailsSplit[1]);
            int viewId = Convert.ToInt32(entityDetailsSplit[2]);
            int tabId = Convert.ToInt32(entityDetailsSplit[3]);
            int formId = Convert.ToInt32(entityDetailsSplit[4]);
            int recId = Convert.ToInt32(entityDetailsSplit[5]);
            string relEntityIds = entityDetailsSplit[6];
            string relFormIds = entityDetailsSplit[7];
            string relRecordIds = entityDetailsSplit[8];
            string relTabIds = entityDetailsSplit[9];
            string divId = entityDetailsSplit[10];
            string relViewIds = entityDetailsSplit[11];

            CurrentUser curUser = cMisc.GetCurrentUser();
            var entities = new cCustomEntities(curUser);
            cCustomEntity entity = entities.getEntityById(entityId);
            cAttribute att = entity.getAttributeById(attributeId);
            cOneToManyRelationship oneToMany = (cOneToManyRelationship)att;

            var retData = new List<object> { divId };
            retData.AddRange(entities.generateOneToManyGrid(oneToMany, 0, viewId, tabId, formId, recId, relEntityIds, relFormIds, relRecordIds, relTabIds, relViewIds));

            return retData.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] getSummaryTable(int entityid, int attributeId, int viewId, int activeTab, int formid, int recordId, string divID)
        {
            List<string> retData = new List<string>();
            CurrentUser curUser = cMisc.GetCurrentUser();
            cCustomEntities clsEntities = new cCustomEntities(curUser);

            string[] tableData = clsEntities.generateSummaryGridNew(entityid, attributeId, viewId, activeTab, formid, recordId);
            //string[] tableData = clsEntities.generateSummaryGrid(entityid, attributeId, viewId, activeTab, formid, recordId);

            retData.Add(divID);
            retData.AddRange(tableData);

            return retData.ToArray();
        }

        /// <summary>
        /// get all the field definitions to be used on the form for the editor
        /// </summary>
        /// <param name="accountid">Current database account ID</param>
        /// <param name="entityid">Entity ID of the GreenLight</param>
        /// <param name="formid">Form Id that fields are being retrieved for</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<sCEFieldDetails> GetFieldDetails(int accountid, int entityid, int formid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cFields clsFields = new cFields(currentUser.AccountID);
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);

            List<sCEFieldDetails> lstFields = new List<sCEFieldDetails>();
            cField field;
            
            foreach (cAttribute attribute in entity.attributes.Values)
            {
                if (!attribute.IsSystemAttribute || (attribute.attributename == "GreenLightCurrency"))
                {
                    sCEFieldDetails CEFieldDetails = this.ConvertAttributeToCEFieldDetails(attribute);
                    lstFields.Add(CEFieldDetails);

                    // if n:1 field type, check for any trigger attributes and add those to the collection
                    if (attribute.fieldtype == FieldType.Relationship && attribute.GetType() == typeof(cManyToOneRelationship))
                    {
                        foreach (LookupDisplayField ldf in ((cManyToOneRelationship)attribute).TriggerLookupFields)
                        {
                            if (ldf.TriggerDisplayFieldId != null)
                            {
                                field = clsFields.GetFieldByID((Guid)ldf.TriggerDisplayFieldId);

                                if (field != null)
                                {
                                    string desc = field.Description;

                                    if (ldf.TriggerJoinVia != null)
                                    {
                                        desc = desc + " (" + ldf.TriggerJoinVia.Description + ")";
                                    }

                                    CEFieldDetails = this.ConvertAttributeToCEFieldDetails(
                                        ldf,
                                        true,
                                        attribute.displayname,
                                        desc);
                                }
                            }
                            else
                            {
                                CEFieldDetails = this.ConvertAttributeToCEFieldDetails(ldf, true, attribute.displayname);    
                            }
                            
                            lstFields.Add(CEFieldDetails);
                        }
                    }
                }
            }

            return lstFields;
        }

        /// <summary>
        /// Build the controls for the form so it can be output in the modal
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="entityid"></param>
        /// <param name="formid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] getForm(int accountid, int entityid, int formid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);
            cCustomEntityForm clsform = entity.getFormById(formid);

            sForm form = new sForm();
            List<sCEFormTab> lstTabs = new List<sCEFormTab>();
            sCEFormTab stTab;
            List<sCEFormSection> lstSections = null;
            sCEFormSection stSection;
            List<sCEFieldDetails> lstFields = null;
            sCEFieldDetails CEFieldDetails;
            cFields fields = new cFields(accountid);

            int tabNumber = 0;
            int sectionNumber = 0;
            int numberOfFields = 0;

            form.formName = clsform.formname;
            form.description = clsform.description;
            form.showSave = clsform.ShowSaveButton;
            form.saveButtonText = clsform.SaveButtonText;
            form.showSaveAndDuplicate = clsform.ShowSaveAndDuplicate;
            form.saveAndDuplicateButtonText = clsform.SaveAndDuplicateButtonText;
            form.showSaveAndStay = clsform.ShowSaveAndStayButton;
            form.saveAndStayButtonText = clsform.SaveAndStayButtonText;
            form.showCancel = clsform.ShowCancelButton;
            form.cancelButtonText = clsform.CancelButtonText;
            form.showBreadcrumbs = clsform.ShowBreadCrumbs;
            form.showSubMenus = clsform.ShowSubMenus;
            form.showSaveAndNew = clsform.ShowSaveAndNew;
            form.saveAndNewButtonText = clsform.SaveAndNewButtonText;
            if (entity.AllowMergeConfigAccess)
            {
                form.hideTorch = clsform.HideTorch;    
            }

            if (entity.EnableAttachments)
            {
                form.hideAttachments = clsform.HideAttachments;    
            }

            if (entity.AudienceView != AudienceViewType.NoAudience)
            {
                form.hideAudiences = clsform.HideAudiences;    
            }

            form.builtIn = clsform.BuiltIn;

            foreach (cCustomEntityFormTab tab in clsform.tabs.Values)
            {
                stTab = new sCEFormTab();
                stTab.TabName = tab.headercaption;
                stTab.TabControlName = "Tab_" + tabNumber;
                stTab.Order = tab.order;
                lstSections = new List<sCEFormSection>();
                sectionNumber = 0;

                foreach (cCustomEntityFormSection section in tab.sections)
                {
                    stSection = new sCEFormSection();
                    stSection.SectionName = section.headercaption;
                    stSection.SectionControlName = stTab.TabControlName + "_Section_" + sectionNumber;
                    stSection.Order = section.order;
                    lstFields = new List<sCEFieldDetails>();

                    List<cCustomEntityFormField> sectionfields = section.getFieldsForSection();

                    foreach (cCustomEntityFormField field in sectionfields)
                    {
                        #region Check field format and column span

                        var format = AttributeFormat.NotSet;
                        int columnSpan = 1;
                        int maxLength = 0;

                        if (field.attribute.GetType() == typeof(cCommentAttribute)
                            || field.attribute.GetType() == typeof(cSummaryAttribute)
                            || field.attribute.GetType() == typeof(cOneToManyRelationship))
                        {
                            columnSpan = 2;
                        }

                        if (field.attribute.GetType() == typeof(cTextAttribute))
                        {
                            var textField = (cTextAttribute)field.attribute;

                            switch (textField.format)
                            {
                                case AttributeFormat.FormattedText:
                                    format = AttributeFormat.FormattedText;
                                    columnSpan = 2;
                                    break;
                                case AttributeFormat.MultiLine:
                                    format = AttributeFormat.MultiLine;
                                    columnSpan = 2;
                                    break;
                                case AttributeFormat.SingleLine:
                                    format = AttributeFormat.SingleLine;
                                    break;
                                case AttributeFormat.SingleLineWide:
                                    format = AttributeFormat.SingleLineWide;
                                    columnSpan = 2;
                                    break;
                            }

                            if (textField.maxlength != null)
                            {
                                maxLength = textField.maxlength.Value;
                            }
                        }

                        if (field.attribute.GetType() == typeof(cListAttribute))
                        {
                            switch (((cListAttribute)field.attribute).format)
                            {
                                case AttributeFormat.ListStandard:
                                    format = AttributeFormat.ListStandard;
                                    break;
                                case AttributeFormat.ListWide:
                                    format = AttributeFormat.ListWide;
                                    columnSpan = 2;
                                    break;
                            }
                        }

                        #endregion

                        CEFieldDetails = new sCEFieldDetails
                            {
                                AttributeID = field.attribute.attributeid,
                                ControlName = "form_attribute_" + field.attribute.attributeid,
                                DisplayName = field.attribute.displayname,
                                Description = field.attribute.description,
                                Tooltip = field.attribute.tooltip,
                                LabelText = field.labelText,
                                Mandatory = field.IsMandatory ?? field.attribute.mandatory,
                                FieldType = field.attribute.fieldtype,
                                ReadOnly = field.isReadOnly,
                                Row = field.row,
                                Column = field.column,
                                ColumnSpan = columnSpan,
                                Format = format,
                                RelationshipType = (field.attribute.GetType() == typeof(cManyToOneRelationship)) ? 1 : 2,
                                FieldValue = string.Empty,
                                DefaultValue = field.DefaultValue,
                                MaxLength = maxLength,
                                MandatoryCheckOverride = field.IsMandatory.HasValue
                        };

                        if (field.attribute.GetType() == typeof(cCommentAttribute))
                        {
                            CEFieldDetails.CommentText = ((cCommentAttribute)field.attribute).commentText;
                        }

                        if (field.attribute.GetType() == typeof(LookupDisplayField))
                        {
                            LookupDisplayField ldf = (LookupDisplayField)field.attribute;
                            if (ldf != null)
                            {
                                cManyToOneRelationship manyToOne = (cManyToOneRelationship)this.getAttribute(accountid, entityid, ldf.TriggerAttributeId != null ? (int)ldf.TriggerAttributeId : 0);

                                CEFieldDetails.FieldValue = manyToOne.displayname;
                                if (ldf.TriggerDisplayFieldId != null)
                                {
                                    CEFieldDetails.DisplayName = fields.GetFieldByID((Guid)ldf.TriggerDisplayFieldId).Description;
                                }
                            }
                        }
                        CEFieldDetails.SortDisplayName = CEFieldDetails.FieldValue + CEFieldDetails.DisplayName;
                        lstFields.Add(CEFieldDetails);
                        numberOfFields++;
                    }

                    stSection.Fields = lstFields;
                    lstSections.Add(stSection);

                    sectionNumber++;
                }

                // Sort the sections in their order 
                lstSections.Sort((sec1, sec2) => sec1.Order.CompareTo(sec2.Order));

                stTab.Sections = lstSections;

                lstTabs.Add(stTab);

                tabNumber++;
            }

            // Sort the tabs in their order 
            lstTabs.Sort((tab1, tab2) => tab1.Order.CompareTo(tab2.Order));

            form.tabs = lstTabs;

            object[] ret = new object[2];
            ret[0] = form;
            ret[1] = numberOfFields;
            return ret;
        }

        /// <summary>
        /// Save form from Form Designer.
        /// </summary>
        /// <param name="accountid">
        /// account id.
        /// </param>
        /// <param name="employeeid">
        /// employee id.
        /// </param>
        /// <param name="entityid">
        /// entity id.
        /// </param>
        /// <param name="formid">
        /// form id.
        /// </param>
        /// <param name="formObj">
        /// form object.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int saveForm(int accountid, int employeeid, int entityid, int formid, sForm formObj)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            SortedList<int, cCustomEntityFormTab> tabs = new SortedList<int, cCustomEntityFormTab>();
            SortedList<int, cCustomEntityFormSection> sections = new SortedList<int, cCustomEntityFormSection>();
            SortedList<int, cCustomEntityFormField> attributes = new SortedList<int, cCustomEntityFormField>();
            int sectionId = 0;

            DateTime? modifiedon = null;
            int? modifiedby = null;
            DateTime createdon = DateTime.Now;
            int createdby = employeeid;
            bool currentBuiltIn = false;
            Guid? systemCustomEntityFormId = null;

            cCustomEntity entity = clsentities.getEntityById(entityid);

            if (formid > 0)
            {
                cCustomEntityForm oldform = entity.getFormById(formid);
                createdon = oldform.createdon;
                createdby = oldform.createdby;
                modifiedby = employeeid;
                modifiedon = DateTime.Now;
                currentBuiltIn = oldform.BuiltIn;
                systemCustomEntityFormId = oldform.SystemCustomEntityFormId;
            }

            // only allow the form to be set as built-in/system if the user is "adminonly"
            if (!currentUser.Employee.AdminOverride && !currentBuiltIn && formObj.builtIn)
            {
                formObj.builtIn = false;
            }

            // if the form is being set as "system", create a system identifier for it
            if (formObj.builtIn && (formid == 0 || currentBuiltIn == false))
            {
                systemCustomEntityFormId = Guid.NewGuid();
            }

            foreach (sCEFormTab tab in formObj.tabs)
            {
                cCustomEntityFormTab currentTab = new cCustomEntityFormTab(0, formid, tab.TabName, tab.Order);
                tabs.Add(tab.Order, currentTab);

                foreach (sCEFormSection section in tab.Sections)
                {
                    cCustomEntityFormSection currentsection = new cCustomEntityFormSection(sectionId, formid, section.SectionName, section.Order, currentTab);
                    sections.Add(sectionId, currentsection);

                    List<sCEFieldDetails> lstFields = section.Fields;

                    foreach (sCEFieldDetails fld in lstFields.Where(fld => fld.FieldType != FieldType.Spacer))
                    {
                        cCustomEntityFormField field;
                        if (fld.FieldType == FieldType.LookupDisplayField)
                        {
                            field = new cCustomEntityFormField(formid, entity.GetLookupDisplayFieldById(fld.AttributeID),
                                                               fld.ReadOnly, currentsection, fld.Column, fld.Row,
                                                               fld.LabelText, (fld.MandatoryCheckOverride.HasValue && fld.MandatoryCheckOverride.Value) ? fld.Mandatory : (bool?)null);
                        }
                        else
                        {
                            field = new cCustomEntityFormField(formid, entity.getAttributeById(fld.AttributeID),
                                                               fld.ReadOnly, currentsection, fld.Column, fld.Row,
                                                               fld.LabelText, (fld.MandatoryCheckOverride.HasValue && fld.MandatoryCheckOverride.Value) ? fld.Mandatory : (bool?)null, fld.DefaultValue);
                        }

                        if (!attributes.ContainsKey(field.attribute.attributeid))
                        {
                            attributes.Add(field.attribute.attributeid, field);
                        }
                    }

                    sectionId++;
                }
            }

            var form = new cCustomEntityForm(formid, entityid, formObj.formName, formObj.description, formObj.showSave, formObj.saveButtonText, formObj.showSaveAndDuplicate, formObj.saveAndDuplicateButtonText, formObj.showSaveAndStay, formObj.saveAndStayButtonText, formObj.showCancel, formObj.cancelButtonText, formObj.showSubMenus, formObj.showBreadcrumbs, createdon, createdby, modifiedon, modifiedby, tabs, sections, attributes, formObj.saveAndNewButtonText, formObj.showSaveAndNew, hideTorch: formObj.hideTorch, hideAttachments: formObj.hideAttachments, hideAudiences: formObj.hideAudiences, builtIn: formObj.builtIn, systemCustomEntityFormId: systemCustomEntityFormId);
            formid = clsentities.saveForm(entityid, form);

            // if the form is set as built-in/system but the GreenLight isn't, make the GreenLight built-in/system too
            if (formid > 0 && formObj.builtIn && !entity.BuiltIn)
            {
                entity.BuiltIn = true;
                clsentities.saveEntity(entity);
            }
            
            return formid;
        }

		/// <summary>
		/// Creates a copy of a GreenLight Form, using the specified form copy name
		/// </summary>
		/// <param name="entityid">The entityID of the Form being copied</param>
		/// <param name="formid">The formID of the Form being copied</param>
		/// <param name="formNameForCopy">The form name for the newly created form</param>
		/// <returns>The ID of the newly created GreenLight Form</returns>
		[WebMethod(EnableSession = true)]
		public int CopyForm(int entityid, int formid, string formNameForCopy)
		{
			if (string.IsNullOrWhiteSpace(formNameForCopy))
			{
				return -6;
			}

			cCustomEntities clsentities = new cCustomEntities(cMisc.GetCurrentUser());
			cCustomEntity entity = clsentities.getEntityById(entityid);
			cCustomEntityForm oldform = entity.getFormById(formid);

			if (oldform != null)
			{
				return clsentities.saveForm(entityid, oldform, formNameForCopy);
			}

			// Invalid formid value was passed, return -9
			return -9;
		}

		/// <summary>
		/// Gets the name of a GreenLight Form by the specified ID
		/// </summary>
		/// <param name="entityid">The entityID for the GreenLight Form</param>
		/// <param name="formid">The formID for the GreenLight Form</param>
		/// <returns>The Form name for the specified Form ID</returns>
		[WebMethod(EnableSession = true)]
		public string GetFormNameById(int entityid, int formid)
		{
			cCustomEntities clsentities = new cCustomEntities(cMisc.GetCurrentUser());
			cCustomEntity entity = clsentities.getEntityById(entityid);
			cCustomEntityForm form = entity.getFormById(formid);
			string retVal = string.Empty;

			if (form != null)
			{
				retVal = form.formname;
			}

			return retVal;
		}

    	[WebMethod(EnableSession = true)]
        public int saveEntity(int accountid, int employeeid, int entityid, string entityname, string pluralname, string description, bool enableattachments, AudienceViewType audienceViewType, bool allowdocmerge, bool enableCurrencies, int? defaultCurrencyID, bool enablePopupWindow, int? defaultPopupView, int formSelectionAttributeId, int? ownerId, int? supportContactId, string supportQuestion, bool enableLocking, bool builtIn)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);

            if (entityid > 0)
            {
                cCustomEntity entity = clsentities.getEntityById(entityid);
                entityid = clsentities.saveEntity(new cCustomEntity(entityid, entityname, pluralname, description, entity.createdon, entity.createdby, DateTime.Now, employeeid, new System.Collections.Generic.SortedList<int, cAttribute>(), new System.Collections.Generic.SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), entity.table, entity.AudienceTable, enableattachments, audienceViewType, allowdocmerge, false, null, null, enableCurrencies, defaultCurrencyID, enablePopupWindow, defaultPopupView, formSelectionAttributeId, ownerId, supportContactId, supportQuestion, enableLocking, builtIn));
            }
            else
            {
                entityid = clsentities.saveEntity(new cCustomEntity(entityid, entityname, pluralname, description, DateTime.Now, employeeid, null, null, new System.Collections.Generic.SortedList<int, cAttribute>(), new System.Collections.Generic.SortedList<int, cCustomEntityForm>(), new SortedList<int, cCustomEntityView>(), null, null, enableattachments, audienceViewType, allowdocmerge, false, null, null, enableCurrencies, defaultCurrencyID, enablePopupWindow, defaultPopupView, formSelectionAttributeId, ownerId, supportContactId, supportQuestion, enableLocking, builtIn));
            }
            return entityid;
        }

        /// <summary>
        /// Web method which saves the n:1 attribute relation to greenlight 
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        /// <param name="entityid">
        /// The entityid.
        /// </param>
        /// <param name="attributeid">
        /// The attributeid.
        /// </param>
        /// <param name="displayname">
        /// The displayname.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="tooltip">
        /// The tooltip.
        /// </param>
        /// <param name="mandatory">
        /// The mandatory.
        /// </param>
        /// <param name="builtIn">
        /// The built In.
        /// </param>
        /// <param name="tableid">
        /// The tableid.
        /// </param>
        /// <param name="displayFieldId">
        /// The display Field Id.
        /// </param>
        /// <param name="matchFieldIds">
        /// The match Field Ids.
        /// </param>
        /// <param name="autocompleteFieldIds">
        /// Field ods of the autocomplete result display fields
        /// </param>
        /// <param name="maxRows">
        /// The max Rows.
        /// </param>
        /// <param name="oJsonFilters">
        /// The o Json Filters.
        /// </param>
        /// <param name="oJsonTriggerFields">
        /// The o Json Trigger Fields.
        /// </param>
        /// <param name="Formid">
        /// The Formid.
        /// </param>
        /// <param name="isParentFilter">
        /// Is the filter type is Parent child filter
        /// </param>
        /// <returns>
        /// Id of thr attribute saved
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int saveManyToOneRelationship(int accountid, int employeeid, int entityid, int attributeid, string displayname, string description, string tooltip, bool mandatory, bool builtIn, Guid tableid, Guid displayFieldId, string[] matchFieldIds, string[] autocompleteFieldIds, int maxRows, JavascriptTreeData oJsonFilters, JavascriptTreeData oJsonTriggerFields, int Formid, bool isParentFilter)
        {			
			cCustomEntities clsentities = new cCustomEntities(cMisc.GetCurrentUser());

			return clsentities.saveManyToOneRelationship(accountid, employeeid, entityid, attributeid, displayname, description, tooltip, mandatory, builtIn, tableid, displayFieldId, matchFieldIds, autocompleteFieldIds, maxRows, oJsonFilters, oJsonTriggerFields, Formid, isParentFilter);
        }

        /// <summary>
        /// The save one to many relationship.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="employeeid">
        /// The employeeid.
        /// </param>
        /// <param name="entityid">
        /// The entityid.
        /// </param>
        /// <param name="attributeid">
        /// The attributeid.
        /// </param>
        /// <param name="displayname">
        /// The displayname.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="builtIn">
        /// The built in.
        /// </param>
        /// <param name="tooltip">
        /// The tooltip.
        /// </param>
        /// <param name="mandatory">
        /// The mandatory.
        /// </param>
        /// <param name="tableid">
        /// The tableid.
        /// </param>
        /// <param name="viewid">
        /// The viewid.
        /// </param>
        /// <param name="isParentFilter">
        /// Is this parent filter.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// Id of thr attribute saved
        /// </returns>
        [WebMethod(EnableSession = true)]
        public int saveOneToManyRelationship(int accountid, int employeeid, int entityid, int attributeid, string displayname, string description, bool builtIn, string tooltip, bool mandatory, Guid tableid, int viewid, bool isParentFilter)
        {
            string attributename = displayname; 
             DateTime createdon = DateTime.Now;
            int createdby = employeeid;
            DateTime? modifiedon = null;
            int? modifiedby = null;
            bool auditidentifier = false;
            bool currentBuiltIn = false;
            cTables clstables = new cTables(accountid);
            cTable relatedtable = clstables.GetTableByID(tableid);
            Guid fieldid = Guid.Empty;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity relatedentity = clsentities.getEntityByTableId(tableid);
            cAttribute attribute = null;

            cCustomEntity entity = clsentities.getEntityById(entityid);

            if (attributeid > 0)
            {
                attribute = entity.getAttributeById(attributeid);
                attributename = attribute.attributename;
                modifiedon = DateTime.Now;
                modifiedby = employeeid;
                createdby = attribute.createdby;
                createdon = attribute.createdon;
                fieldid = attribute.fieldid;
                auditidentifier = attribute.isauditidentifer;
                currentBuiltIn = attribute.BuiltIn;
            }

            // only allow the attribute to be set as built-in/system if the user is "adminonly"
            if (!currentUser.Employee.AdminOverride && !currentBuiltIn && builtIn)
            {
                builtIn = false;
            }

            attribute = new cOneToManyRelationship(attributeid, attributename, displayname, description, tooltip, mandatory, builtIn, createdon, createdby, modifiedon, modifiedby, relatedtable, fieldid, viewid, relatedentity.entityid, auditidentifier, entityid, false, false, false);
            int presaveId = attributeid;

            attributeid = clsentities.saveRelationship(entityid, attribute, isParentFilter); // -1 duplicate, -2 circular reference

            if (attributeid > 0)
            {
                clsentities.createDerivedTable(employeeid, presaveId, attributeid, entity, relatedtable, relatedentity);

                // if the attribute is set as built-in/system but the GreenLight isn't, make the GreenLight built-in/system too
                if (builtIn && !entity.BuiltIn)
                {
                    entity.BuiltIn = true;
                    clsentities.saveEntity(entity);
                }
            }

            return attributeid;
        }

        [WebMethod(EnableSession = true)]
        public int SaveView(int accountid, int employeeid, int entityid, int viewid, string viewname, string description, bool builtIn, int menuid, string menuDescription, bool showRecordCount, JavascriptTreeData oJsonFields, int addformid, int editformid, bool allowdelete, bool allowapproval, bool allowarchive, JavascriptTreeData oJsonFilters, jsGreenLightViewSortColumn oSortColumn, SpendManagementLibrary.SortDirection sortOrderDirection, string menuIconName, List<FormSelectionMapping> addMappings, List<FormSelectionMapping> editMappings, List<int> menuDisabledModuleIds)

        {
            cCustomEntities clsentities = new cCustomEntities(cMisc.GetCurrentUser());
            return clsentities.SaveView(accountid, employeeid, entityid, viewid, viewname, description, builtIn, menuid, menuDescription, showRecordCount, oJsonFields, addformid, editformid, allowdelete, allowapproval, allowarchive, oJsonFilters, oSortColumn, sortOrderDirection, menuIconName, addMappings, editMappings, menuDisabledModuleIds);
        }

        /// <summary>
        /// Web method to save an attribute
        /// </summary>
        /// <param name="accountid">AccountID</param>
        /// <param name="employeeid">EmployeeID</param>
        /// <param name="entityid">EntityID</param>
        /// <param name="attributeid">AttributeID</param>
        /// <param name="displayname">Dispplay Name</param>
        /// <param name="description">Description</param>
        /// <param name="tooltip">Tooltip Text</param>
        /// <param name="mandatory">Mandatory Attribute</param>
        /// <param name="fieldtype">Type Of Attribute</param>
        /// <param name="maxlength">Max Length of Text</param>
        /// <param name="format">Format of Attribute (Text or Date Format)</param>
        /// <param name="defaultvalue">Default Value</param>
        /// <param name="precision">Precision for Decimal</param>
        /// <param name="lstitems">Items if List Type</param>
        /// <param name="workflowid">Workflow ID</param>
        /// <param name="advicePanelText">Advice Text</param>
        /// <param name="auditidentifier">Attribute to be used as Audit Identifier</param>
        /// <param name="isunique">Is Unique in Value</param>
        /// <param name="populateExistingRecordsDefault"></param>
        /// <param name="displayInMobile">Whether to show the attribute for mobile users</param>
        /// <param name="boolAttribute">Specifies whether a user will have access to the image library when uploading an attachment or strip fonts from formatted text box</param>
        /// <param name="builtIn">Specifies whether the attribute is a system attribute</param>
        /// <param name="encrypted">Sepcifies whether the attribute has the data encrypted in the datastore.</param>
        /// <returns>Object array containing attribute id, 0 or 1 if Audit Identifier and Display Name</returns>
        [WebMethod(EnableSession = true)]
        public string[] saveAttribute(int accountid, int employeeid, int entityid, int attributeid, string displayname, string description, string tooltip, bool mandatory, FieldType fieldtype, int? maxlength, AttributeFormat format, string defaultvalue, byte precision, string[] lstitems, int workflowid, string advicePanelText, bool auditidentifier, bool isunique, bool populateExistingRecordsDefault, bool displayInMobile, bool boolAttribute, bool builtIn, bool encrypted)
        {
            return cCustomEntities.SaveGeneralAttribute(employeeid, entityid, attributeid, displayname, description, tooltip, mandatory, fieldtype, maxlength, format, defaultvalue, precision, lstitems, workflowid, advicePanelText, auditidentifier, isunique, populateExistingRecordsDefault, boolAttribute, displayInMobile, builtIn, encrypted);
        }

        [WebMethod(EnableSession = true)]
        public int[] deleteAttribute(int attributeid, int entityid, int accountid)
        {
            List<int> ret = new List<int>();
            CurrentUser user = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(user);
            int delegateid = 0;
            if (user.Delegate != null)
            {
                delegateid = user.Delegate.EmployeeID;
            }
            int retCode = clsentities.deleteAttribute(attributeid, user.EmployeeID, delegateid);
            ret.Add(attributeid);
            ret.Add(clsentities.getAuditIdentifierAttributeIDForEntity(entityid, accountid));
            ret.Add(retCode);
            return ret.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public int DeleteView(int viewid)
        {
            cCustomEntities clsentities = new cCustomEntities(cMisc.GetCurrentUser());
            return clsentities.deleteView(viewid);
        }

        [WebMethod(EnableSession = true)]
        public int deleteForm(int formid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(user);
            int delegateid = 0;
            if (user.Delegate != null)
            {
                delegateid = user.Delegate.EmployeeID;
            }
            return clsentities.deleteForm(formid, user.EmployeeID, delegateid);
        }

        /// <summary>
        /// Checks if the view is the default view for a new pop-up window.
        /// </summary>
        /// <param name="viewid"></param>
        /// <param name="accountid"></param>
        /// <returns>Count of views</returns> 
         [WebMethod(EnableSession = true)]
        public int checkViewDoesNotBelongToPopupView(int viewid, int accountid)
        {
            cCustomEntities clsentities = new cCustomEntities(cMisc.GetCurrentUser());
            return clsentities.checkViewDoesNotBelongToPopupView(viewid, accountid);
        }
        
        /// <summary>
        /// Checks if a specified List Attributes' List Item is in use on a CustomEntityViewFilter
        /// </summary>
        /// <param name="listItemID"></param>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int CheckListItemIsNotUsedWithinFilter(int entityID, int attributeID, int listItemID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities customEntities = new cCustomEntities(currentUser);
            cCustomEntity customEntity = customEntities.getEntityById(entityID);
            cAttribute attribute = customEntity.getAttributeById(attributeID);

            return customEntities.CheckListItemIsNotUsedWithinFilter(attribute.fieldid, listItemID);
        }

        [WebMethod(EnableSession = true)]
        public cAttribute getAttribute(int accountid, int entityid, int attributeid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);
            cAttribute attribute = entity.getAttributeById(attributeid);            

            if (attribute.GetType() == typeof(cListAttribute))
            {
                return new cListAttribute(attribute.attributeid, attribute.attributename, attribute.displayname, attribute.description, attribute.tooltip, attribute.mandatory, attribute.fieldtype, attribute.createdon, attribute.createdby, attribute.modifiedon, attribute.modifiedby, null, attribute.fieldid, attribute.isauditidentifer, attribute.isunique, ((cListAttribute)attribute).format, false, false, attribute.DisplayInMobile, attribute.BuiltIn, attribute.IsSystemAttribute);
            }
            return attribute;
        }

        /// <summary>
        /// Get custom entity list items.
        /// </summary>
        /// <param name="entityid">
        /// The entity id.
        /// </param>
        /// <param name="attributeid">
        /// The attribute id.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string[] getListItems(int entityid, int attributeid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var clsentities = new cCustomEntities(user);
            var entity = clsentities.getEntityById(entityid);
            var attribute = (cListAttribute)entity.getAttributeById(attributeid);
            var jss = new JavaScriptSerializer();
            var items = new string[attribute.items.Count];
            foreach (KeyValuePair<int, cListAttributeElement> kvp in attribute.items)
            {
                var listelement = (cListAttributeElement)kvp.Value;
                items[kvp.Key] = jss.Serialize(listelement);
            }
            
            return items;
        }

        [WebMethod(EnableSession = true)]
        public object[] getRelationship(int accountid, int entityid, int attributeid)
        {
            List<object> data = new List<object>();
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);
            cAttribute attribute = entity.getAttributeById(attributeid);

            if (attribute.GetType() == typeof(cManyToOneRelationship))
            {
                cManyToOneRelationship manytoone = (cManyToOneRelationship)attribute;
                data.Add(1);
                data.Add(manytoone.attributeid);
                data.Add(manytoone.attributename);
                data.Add(manytoone.displayname);
                data.Add(manytoone.description);
                data.Add(manytoone.mandatory);
                data.Add(manytoone.relatedtable.TableID);
                data.Add(manytoone.tooltip);
                data.Add(manytoone.AutoCompleteDisplayField);
                data.Add(manytoone.AutoCompleteMatchFieldIDList);
                data.Add(manytoone.AutoCompleteMatchRows);
                data.Add(manytoone.BuiltIn);
            }
            else if (attribute.GetType() == typeof(cOneToManyRelationship))
            {
                cOneToManyRelationship onetomany = (cOneToManyRelationship)attribute;
                data.Add(2);
                data.Add(onetomany.attributeid);
                data.Add(onetomany.attributename);
                data.Add(onetomany.displayname);
                data.Add(onetomany.description);
                data.Add(onetomany.mandatory);
                data.Add(onetomany.relatedtable.TableID);
                data.Add(onetomany.tooltip);
                data.Add(onetomany.viewid);
                data.Add(onetomany.BuiltIn);
            }

            return data.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public List<ListItem> getRelationshipViews(int accountid, string entityid, int attributeid)
        {
            return cCustomEntities.GetRelationshipViews(entityid, attributeid);
        }

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod]
        //public object[] populateViewFields(int accountid, int entityid, int viewid)
        //{
        //    List<object[]> nodes = new List<object[]>();
        //    List<object> currentNode;
        //    SortedList<Guid, sViewTreeItem> parentNodes = new SortedList<Guid, sViewTreeItem>();
        //    CurrentUser currentUser = cMisc.GetCurrentUser();
        //    cCustomEntities clsentities = new cCustomEntities(currentUser);
        //    cCustomEntity entity = clsentities.getEntityById(entityid);
        //    cCustomEntityView view = null;
        //    cFields clsfields = new cFields(accountid);

        //    if (viewid > 0)
        //    {
        //        view = entity.getViewById(viewid);
        //    }

        //    foreach (cAttribute attribute in entity.attributes.Values)
        //    {
        //        //if (attribute.GetType() == typeof(cManyToOneRelationship))
        //        //{
        //        //    cManyToOneRelationship relationship = (cManyToOneRelationship)attribute;
        //        //    cFields clsfields = new cFields(accountid);
        //        //    parentNode = new List<object>();
        //        //    parentNode.Add(attribute.displayname);
        //        //    parentNode.Add(attribute.attributeid);


        //        //    List<cField> fields = clsfields.getFieldsByTable(relationship.relatedtable.tableid);
        //        //    foreach (cField field in fields)
        //        //    {
        //        //        if (field.WorkflowSearch)
        //        //        {
        //        //            //Node childnode = new Node();
        //        //            //childnode.Text = field.description;
        //        //            //childnode.Tag = field.fieldid;
        //        //            //parentnode.Nodes.Add(childnode);
        //        //        }
        //        //    }

        //        //    nodes.Add(parentNode.ToArray());
        //        //}
        //        //else
        //        //{
        //        if ((view == null || !view.containsField(attribute.fieldid)) && attribute.GetType() != typeof(cCommentAttribute))
        //        {
        //            currentNode = new List<object>();
        //            currentNode.Add(attribute.displayname);
        //            currentNode.Add(attribute.fieldid);
        //            nodes.Add(currentNode.ToArray());
        //        }
        //        //}
        //        if (attribute.GetType() == typeof(cManyToOneRelationship)) // add a reference for the related table if theres not one already so we can pick up the related fields later
        //        {
        //            cManyToOneRelationship tmpRel = (cManyToOneRelationship)attribute;
        //            if (tmpRel.AliasTable != null)
        //            {
        //                if (parentNodes.ContainsKey(tmpRel.AliasTable.TableID) == false)
        //                {
        //                    sViewTreeItem viewTreeItem = new sViewTreeItem();
        //                    viewTreeItem.AttributeName = tmpRel.displayname;
        //                    viewTreeItem.relatedTableName = tmpRel.relatedtable.TableName;
        //                    viewTreeItem.relatedTableID = tmpRel.relatedtable.TableID;
        //                    viewTreeItem.AliasTable = tmpRel.AliasTable;
        //                    parentNodes.Add(tmpRel.AliasTable.TableID, viewTreeItem);
        //                }
        //            }
        //        }
        //    }

        //    if (parentNodes.Count > 0)
        //    {
        //        currentNode = new List<object>();
        //        currentNode.Add("Related Tables");
        //        currentNode.Add("#section#");
        //        nodes.Add(currentNode.ToArray());

        //        foreach (KeyValuePair<Guid, sViewTreeItem> kvp in parentNodes)
        //        {
        //            List<cField> fields = clsfields.GetFieldsByTableID(kvp.Value.relatedTableID);

        //            fields.Sort(delegate(cField f1, cField f2) { return f1.Description.CompareTo(f2.Description); });

        //            currentNode = new List<object>();
        //            currentNode.Add(kvp.Value.relatedTableName + " (For " + kvp.Value.AttributeName + ")");

        //            if (kvp.Value.AliasTable != null)
        //            {
        //                currentNode.Add("#table#" + kvp.Key.ToString());
        //            }
        //            else
        //            {
        //                currentNode.Add("#table#" + kvp.Value.relatedTableID.ToString());
        //            }

        //            nodes.Add(currentNode.ToArray());

        //            foreach (cField field in fields)
        //            {
        //                currentNode = new List<object>();
        //                currentNode.Add(field.Description);
        //                currentNode.Add(field.FieldID);
        //                nodes.Add(currentNode.ToArray());
        //            }
        //        }
        //    }

        //    return nodes.ToArray();
        //}

        //[WebMethod(EnableSession = true)]
        //public object[] populateSelectedViewFields(int accountid, int entityid, int viewid)
        //{
        //    List<object[]> nodes = new List<object[]>();
        //    List<object> currentNode;
        //    CurrentUser currentUser = cMisc.GetCurrentUser();
        //    cCustomEntities clsentities = new cCustomEntities(currentUser);
        //    cCustomEntity entity = clsentities.getEntityById(entityid);
        //    cCustomEntityView view = null;

        //    view = entity.getViewById(viewid);

        //    foreach (cCustomEntityViewField field in view.fields.Values)
        //    {
        //        currentNode = new List<object>();
        //        currentNode.Add(field.Field.Description);
        //        currentNode.Add(field.Field.FieldID);
        //        nodes.Add(currentNode.ToArray());
        //    }

        //    return nodes.ToArray();
        //}

        ///// <summary>
        ///// Get view filter rows
        ///// </summary>
        ///// <param name="entityid"></param>
        ///// <param name="viewid"></param>
        ///// <returns></returns>
        //[WebMethod(EnableSession = true)]
        //public List<List<string>> PopulateViewFilters(int entityid, int viewid)
        //{
        //    CurrentUser currentUser = cMisc.GetCurrentUser();
        //    cCustomEntities clsentities = new cCustomEntities(currentUser);
        //    cCustomEntity entity = clsentities.getEntityById(entityid);
        //    cFields clsfields = new cFields(currentUser.AccountID);

        //    cCustomEntityView view = null;
        //    List<List<string>> nodes = new List<List<string>>();
        //    List<string> lstStr;
        //    string str;
        //    StringBuilder elementHTML;

        //    if (viewid > 0)
        //    {
        //        view = entity.getViewById(viewid);
        //        if (view == null) { return nodes; }

        //        foreach (cFieldFilter f in view.filters.Values)
        //        {
        //            elementHTML = new StringBuilder();
        //            #region valuesarray
        //            lstStr = new List<string>();
        //            str = "<select><option value=\"" + f.Field.TableID.ToString() + "\">" + f.Field.ParentTable.Description + "</option></select>";
        //            lstStr.Add(str);
        //            str = "<select><option value=\"" + f.Field.FieldID.ToString() + "\">" + f.Field.Description + "</option></select>";
        //            lstStr.Add(str);
        //            str = "<select><option value=\"" + ((int)f.Conditiontype).ToString() + "\">" + f.Conditiontype.ToString() + "</option></select>";
        //            lstStr.Add(str);
        //            if (f.Field.GenList)
        //            {
        //                #region genlist
        //                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));
        //                cField stringKeyField = clsfields.GetFieldByID(f.Field.LookupTable.StringKeyFieldID);

        //                string strSQL = "SELECT [" + f.Field.LookupTable.PrimaryKey.FieldName + "] AS primaryKey, [" + stringKeyField.FieldName + "] AS keyField FROM [" + f.Field.LookupTable.TableName + "] ORDER BY [" + stringKeyField.FieldName + "];";
        //                System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL);

        //                int primaryKey;
        //                string keyField;
        //                List<ListItem> lstValues = new List<ListItem>();

        //                while (reader.Read())
        //                {
        //                    primaryKey = reader.GetInt32(0); // primaryKey
        //                    keyField = reader.GetString(1); // keyField
        //                    lstValues.Add(new ListItem(keyField, primaryKey.ToString()));
        //                }
        //                reader.Close();

        //                lstValues.Sort(delegate(ListItem li1, ListItem li2) { return li1.Text.CompareTo(li2.Text); });

        //                elementHTML.Append("<select>");
        //                foreach (ListItem li in lstValues)
        //                {
        //                    str = (li.Value == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                    elementHTML.Append("<option value=\"" + li.Value + "\"" + str + ">" + li.Text + "</option>");
        //                }
        //                str = ("@ME_ID" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ME_ID\"" + str + ">@ME_ID</option>");
        //                str = ("@ME" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ME\"" + str + ">@ME</option>");
        //                str = ("@ACTIVEMODULE_ID" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ACTIVEMODULE_ID\"" + str + ">@ACTIVEMODULE_ID</option>");
        //                str = ("@ACTIVEMODULE" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ACTIVEMODULE\"" + str + ">@ACTIVEMODULE</option>");
        //                str = ("@ACTIVESUBACCOUNT_ID" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ACTIVESUBACCOUNT_ID\"" + str + ">@ACTIVESUBACCOUNT_ID</option>");
        //                elementHTML.Append("</select>");
        //                #endregion genlist
        //            }
        //            else if (f.Field.ValueList)
        //            {
        //                #region valuelist
        //                elementHTML.Append("<select>");
        //                foreach (int k in f.Field.ListItems.Keys)
        //                {
        //                    str = (k.ToString() == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                    elementHTML.Append("<option value=\"" + k.ToString() + "\"" + str + ">" + f.Field.ListItems[k].ToString() + "</option>");
        //                }
        //                str = ("@ME_ID" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ME_ID\"" + str + ">@ME_ID</option>");
        //                str = ("@ME" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ME\"" + str + ">@ME</option>");
        //                str = ("@ACTIVEMODULE_ID" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ACTIVEMODULE_ID\"" + str + ">@ACTIVEMODULE_ID</option>");
        //                str = ("@ACTIVEMODULE" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ACTIVEMODULE\"" + str + ">@ACTIVEMODULE</option>");
        //                str = ("@ACTIVESUBACCOUNT_ID" == f.Value) ? " selected=\"selected\"" : string.Empty;
        //                elementHTML.Append("<option value=\"@ACTIVESUBACCOUNT_ID\"" + str + ">@ACTIVESUBACCOUNT_ID</option>");
        //                elementHTML.Append("</select>");
        //                #endregion valuelist
        //            }
        //            else
        //            {
        //                elementHTML.Append("<input type=\"text\" maxlength=\"150\" value=\"" + f.Value.Replace("\"", "&quot;") + "\" />");
        //            }
        //            lstStr.Add(elementHTML.ToString());

        //            #endregion valuesarray
        //            nodes.Add(lstStr);
        //        }
        //    }

        //    return nodes;
        //}


        [WebMethod(EnableSession = true)]
        public string[] getAttributesGrid(int entityid)
        {
            int entityID = (entityid > 0) ? entityid : 0;

            CurrentUser user = cMisc.GetCurrentUser();

            cCustomEntities clsentities = new cCustomEntities(user);
            cCustomEntity entity = clsentities.getEntityById(entityID);

            return clsentities.createAttributeGrid(entity);
        }

        [WebMethod(EnableSession = true)]
        public string[] getViewGrid(int entityid)
        {
            //string[] data = contextKey.Split(',');
            //int accountid = Convert.ToInt32(data[0]);
            //int entityid = Convert.ToInt32(data[1]);
            int entityID = (entityid > 0) ? entityid : 0;

            CurrentUser user = cMisc.GetCurrentUser();

            cCustomEntities clsentities = new cCustomEntities(user);
            cCustomEntity entity = clsentities.getEntityById(entityID);
            return clsentities.createViewGrid(entity);

            //cFields clsfields = new cFields(user.AccountID);
            //cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridViews", "select viewid, view_name, description from custom_entity_views");
            //if (entity != null)
            //{
            //    clsgrid.addFilter(clsfields.getFieldById(new Guid("135b0d53-0123-4154-b467-a499df1c7b84")), ConditionType.Equals, new object[] { entity.entityid }, null, ConditionJoiner.None);
            //}
            //else
            //{
            //    clsgrid.addFilter(clsfields.getFieldById(new Guid("135b0d53-0123-4154-b467-a499df1c7b84")), ConditionType.Equals, new object[] { -1 }, null, ConditionJoiner.None);
            //}
            //clsgrid.enabledeleting = true;
            //clsgrid.enableupdating = true;
            //clsgrid.editlink = "javascript:editView({viewid});";
            //clsgrid.deletelink = "javascript:deleteView({viewid});";
            //clsgrid.getColumnByName("viewid").hidden = true;
            //clsgrid.CssClass = "datatbl";
            //clsgrid.SortedColumn = clsgrid.getColumnByName("description");
            //clsgrid.KeyField = "viewid";

            //clsgrid.EmptyText = "There are currently no views defined for this entity.";
            //return clsgrid.generateGrid();
        }
 
        [WebMethod(EnableSession = true)]
        public string[] getFormGrid(int entityid)
        {
            //string[] data = contextKey.Split(',');
            //int accountid = Convert.ToInt32(data[0]);
            //int entityid = Convert.ToInt32(data[1]);
            int entityID = (entityid > 0) ? entityid : 0;

            CurrentUser user = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(user);
            cCustomEntity entity = clsentities.getEntityById(entityID);

            return clsentities.createFormGrid(entity);

            //cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridForms", "select formid, form_name, description from custom_entity_forms");
            //cFields clsfields = new cFields(user.AccountID);
            //if (entity != null)
            //{
            //    clsgrid.addFilter(clsfields.getFieldById(new Guid("b4662049-d92a-4b8b-8dba-69acd883cebb")), ConditionType.Equals, new object[] { entity.entityid }, null, ConditionJoiner.None);
            //}
            //else
            //{
            //    clsgrid.addFilter(clsfields.getFieldById(new Guid("b4662049-d92a-4b8b-8dba-69acd883cebb")), ConditionType.Equals, new object[] { -1 }, null, ConditionJoiner.None);
            //}
            //clsgrid.getColumnByName("formid").hidden = true;
            //clsgrid.enabledeleting = true;
            //clsgrid.enableupdating = true;
            //clsgrid.editlink = "javascript:editForm({formid});";
            //clsgrid.deletelink = "javascript:deleteForm({formid});";
            //clsgrid.KeyField = "formid";
            //clsgrid.CssClass = "datatbl";

            //clsgrid.EmptyText = "There are currently no forms defined for this entity.";
            //return clsgrid.generateGrid();
        }

        /// <summary>
        /// Get the view and return the view structure to the client side
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="entityid"></param>
        /// <param name="viewid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public sView getView(int accountid, int entityid, int viewid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser,true);
            cCustomEntity entity = clsentities.getEntityById(entityid);
            cCustomEntityView clsview = entity.getViewById(viewid);
            sView view = new sView();
            view.viewName = clsview.viewname;
            view.description = clsview.description;
            view.ShowRecordCount = clsview.showRecordCount;
            view.builtIn = clsview.BuiltIn;
            view.allowAdd = clsview.allowadd;
            if (clsview.DefaultAddForm != null)
            {
                view.addFormID = clsview.DefaultAddForm.formid;
            }

            view.allowEdit = clsview.allowedit;
            if (clsview.DefaultEditForm != null)
            {
                view.editFormID = clsview.DefaultEditForm.formid;
            }

            view.allowDelete = clsview.allowdelete;
            view.allowApproval = clsview.allowapproval;
            view.allowArchive= clsview.allowarchive;
            if (clsview.menuid.HasValue)
            {
                view.nMenuid = clsview.menuid.Value;
            }

            view.AddFormMappings = clsview.AddFormMappings.Any() ? clsview.AddFormMappings : new List<FormSelectionMapping>();
            view.EditFormMappings = clsview.EditFormMappings.Any() ? clsview.EditFormMappings : new List<FormSelectionMapping>();

            view.MenuDescription = clsview.MenuDescription;
            view.MenuDisabledModuleIds = clsview.MenuDisabledModuleIds;

            view.SortedColumnID = (clsview.SortColumn.JoinVia != null && clsview.SortColumn.JoinVia.JoinViaID > 0)
                ? clsview.SortColumn.FieldID + "_" + clsview.SortColumn.JoinVia.JoinViaID
                : "copy_n" + clsview.SortColumn.FieldID + "_0";

            view.SortOrderDirection = clsview.SortColumn.SortDirection;
            view.formDropDownOptions = entity.CreateFormDropDown(includeNone: true);

        	var iconUrl = GlobalVariables.StaticContentLibrary + "/icons/48/plain/" + clsview.MenuIcon;
        	view.MenuIcon = new ViewMenuIcon { IconUrl = iconUrl, IconName = clsview.MenuIcon };

            return view;
        }

        /// <summary>
        /// Retrieves List of dropdown list items for populating a control
        /// </summary>
        /// <param name="entityid">Entity ID to retrieve form list for</param>
        /// <returns>List of ListItems</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<ListItem> getViewForms(int entityid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);
            List<ListItem> forms = entity.CreateFormDropDown(includeNone: true);

            return forms;
        }

        [WebMethod(EnableSession = true)]
        public object[] getSummary(int accountid, int entityid, int attid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);
            List<sSummary> summaryItems = new List<sSummary>();
            List<sSummaryColumn> summaryCols = new List<sSummaryColumn>();

            List<object> retDetails = new List<object>();

            if (attid == 0)
            {
                retDetails.Add("");
                retDetails.Add("");
                retDetails.Add(0);
                retDetails.Add(summaryItems.ToArray());
                retDetails.Add(summaryCols.ToArray());
            }
            else
            {
                cAttribute att = entity.getAttributeById(attid);
                cSummaryAttribute summary_att = (cSummaryAttribute)att;

                foreach (KeyValuePair<int, cSummaryAttributeElement> kvp in summary_att.SummaryElements)
                {
                    cSummaryAttributeElement se = (cSummaryAttributeElement)kvp.Value;
                    sSummary summary;
                    summary.summary_attributeid = se.SummaryAttributeId;
                    summary.attributeid = se.AttributeId;
                    summary.otm_attributeid = se.OTM_AttributeId;
                    summary.order = se.Order;
                    summaryItems.Add(summary);
                }

                cFields clsFields = new cFields(accountid);

                foreach (KeyValuePair<int, cSummaryAttributeColumn> kvp in summary_att.SummaryColumns)
                {
                    cSummaryAttributeColumn sc = (cSummaryAttributeColumn)kvp.Value;
                    sSummaryColumn summarycol;

                    summarycol.columnid = sc.ColumnId;
                    summarycol.attributeid = att.attributeid;
                    summarycol.alt_header = sc.AlternateHeader;
                    summarycol.columnAttributeID = sc.ColumnAttributeID;
                    summarycol.order = sc.Order;
                    summarycol.width = sc.Width;
                    summarycol.default_sort = sc.DefaultSort;
                    summarycol.filterVal = sc.FilterValue;
                    summarycol.ismtoattribute = sc.IsMTOField;
                    summarycol.displayFieldId = (sc.DisplayFieldId == Guid.Empty ? "" : sc.DisplayFieldId.ToString());
                    summarycol.displayFieldName = (sc.DisplayFieldId == Guid.Empty ? "" : clsFields.GetFieldByID(sc.DisplayFieldId).Description);
                    summarycol.JoinViaID = sc.JoinViaObj == null ? 0 : sc.JoinViaObj.JoinViaID;
                    
                    summaryCols.Add(summarycol);
                }

                retDetails.Add(att.displayname);
                retDetails.Add(att.description);
                cCustomEntity srcEntity = clsentities.getEntityById(summary_att.SourceEntityID);
                retDetails.Add(srcEntity.table.TableID.ToString());
                retDetails.Add(summaryItems.ToArray());
                retDetails.Add(summaryCols.ToArray());
            }

            return retDetails.ToArray();
        }

        /// <summary>
        /// Return the summary sources for the entity
        /// </summary>
        /// <param name="entityid">Entity ID</param>
        /// <param name="related_entitytableid">Related Entity Table ID</param>
        /// <returns>String array of summary sources</returns>
        [WebMethod(EnableSession = true)]
        public string[] getSummarySources(int entityid, Guid related_entitytableid)
        {
            return cCustomEntities.SummarySources(entityid, related_entitytableid);
        }

        /// <summary>
        /// Creates the data for the new pop-up window views drop down list
        /// </summary>
        /// <param name="entityid">id of custom entity</param>
        /// <param name="formid">id of the pop-up view, is null then default entity view is selected</param>
        /// <param name="recordid">id of individual record</param>
        /// <returns>Array of listitems to be bound to the pop-up view drop down list</returns>
        [WebMethod(EnableSession = true)]
        public ListItem[] getPopupViews(int entityid, int? DefaultPopupView, int AccountID)
        {
            cCustomEntities clsentities = new cCustomEntities();
            return clsentities.createViewDropDown(entityid, DefaultPopupView, AccountID);
        }

        [WebMethod(EnableSession = true)]
        public int saveSummary(int entityid, int attributeid, string displayname, string description, string source_entity_tableid, sSummary[] include_rel_attributeid, sSummaryColumn[] summary_cols, jsSummaryColumnData[] joinViaData)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));
            cCustomEntities clsentities = new cCustomEntities(curUser);
            cCustomEntity entity = clsentities.getEntityById(entityid);

            string attributename = displayname;
            DateTime createdon = DateTime.Now;
            int createdby = curUser.EmployeeID;
            DateTime? modifiedon = null;
            int? modifiedby = null;
            Guid fieldid = Guid.Empty;
            cAttribute attribute = null;

            Dictionary<int, cSummaryAttributeElement> elements = new Dictionary<int, cSummaryAttributeElement>();
            Dictionary<int, cSummaryAttributeColumn> columns = new Dictionary<int, cSummaryAttributeColumn>();

            for (int x = 0; x < include_rel_attributeid.Length; x++)
            {
                cCustomEntity tmpEntity = clsentities.getEntityById(clsentities.getEntityIdByAttributeId(include_rel_attributeid[x].otm_attributeid));

                cSummaryAttributeElement element = new cSummaryAttributeElement(include_rel_attributeid[x].summary_attributeid, include_rel_attributeid[x].attributeid, include_rel_attributeid[x].otm_attributeid, include_rel_attributeid[x].order);
                elements.Add(x, element);
            }

            cCustomEntity source_entity = clsentities.getEntityByTableId(new Guid(source_entity_tableid));
            for (int x = 0; x < summary_cols.Length; x++)
            {
                int attId = summary_cols[x].columnAttributeID;

                if (attId > 0)
                {
                    cAttribute att = source_entity.getAttributeById(attId);
                    if (att != null)
                    {
                        bool isManyToOne = (att.GetType() == typeof(cManyToOneRelationship));
                        Guid displayFieldId = Guid.Empty;
                        JoinVia joinVia = null;

                        if (isManyToOne)
                        {
                            if (Guid.TryParseExact(summary_cols[x].displayFieldId, "D", out displayFieldId))
                            {
                                // parse columns to establish if joinVia required
                                string _displayFieldCrumbs = string.Empty;
                                SortedList<int, JoinViaPart> _joinViaList = new SortedList<int, JoinViaPart>();
                                int _joinViaID = summary_cols[x].JoinViaID;
                                foreach (jsSummaryColumnData scd in joinViaData)
                                {
                                    if (scd.columnid == summary_cols[x].columnid) // match up the joinVia data using columnid
                                    {
                                        string _joinViaDescription = scd.JoinViaCrumbs;
                                        string _joinViaIDs = scd.JoinViaPath;

                                        // for saving, we only need to parse out the via parts if we don't have a saved joinvia id already
                                        if (_joinViaID < 1)
                                        {
                                            _joinViaList = JoinVias.JoinViaPartsFromCompositeGuid(_joinViaIDs);
                                            _joinViaID = 0; // 0 causes the save on the joinVia list
                                        }
                                        joinVia = new JoinVia(_joinViaID, _joinViaDescription, Guid.NewGuid(), _joinViaList);
                                        break;
                                    }
                                }

                                if (joinVia == null && _joinViaID > 0)
                                {
                                    // summary column already has joinVia so retrieve existing
                                    JoinVias vias = new JoinVias(curUser);
                                    joinVia = vias.GetJoinViaByID(_joinViaID);
                                }
                            }
                        }
                        columns.Add(summary_cols[x].columnAttributeID, new cSummaryAttributeColumn(summary_cols[x].columnid, summary_cols[x].columnAttributeID, summary_cols[x].alt_header, summary_cols[x].width, summary_cols[x].order, summary_cols[x].default_sort, summary_cols[x].filterVal, isManyToOne, displayFieldId, joinVia));
                    }
                }
            }

            cSummaryAttribute summary_attribute;

            if (attributeid > 0)
            {
                attribute = entity.getAttributeById(attributeid);
                attributename = attribute.attributename;
                modifiedon = DateTime.Now;
                modifiedby = curUser.EmployeeID;
                createdby = attribute.createdby;
                createdon = attribute.createdon;
            }

            summary_attribute = new cSummaryAttribute(attributeid, attributename, displayname, description, createdon, createdby, modifiedon, modifiedby, Guid.Empty, elements, columns, source_entity.entityid, false, false, false);

            attributeid = clsentities.saveSummaryAttribute(entityid, summary_attribute);

            return attributeid;
        }

        /// <summary>
        /// Gets fields for a particular many to attribute in the form of tree nodes
        /// </summary>
        /// <param name="attributeId">Many To One attribute to retrieve field nodes for</param>
        /// <returns>jsTreeData object</returns>
        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetInitialTreeNodesForDisplayField(int attributeId)
        {
            cCustomEntities clsCustomEntities = new cCustomEntities(cMisc.GetCurrentUser());
            return clsCustomEntities.getDisplayFieldData(attributeId);
        }

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod]
        //public List<string> BuildCriteriaSelectTable(object[][] criteria, bool update, string baseTableID)
        //{

        //    CurrentUser currentUser = cMisc.GetCurrentUser();
        //    int rowIndex = 0;
        //    StringBuilder sbTable = new StringBuilder();
        //    List<string> lstReturnStrings = new List<string>();
        //    Guid fieldID;
        //    Int16 conditionType;
        //    CriteriaMode criteriaMode;
        //    bool runtime;
        //    string valueOne;
        //    string valueTwo;
        //    cField reqField;
        //    cFields clsFields = new cFields(currentUser.AccountID);


        //    List<sTableBasics> lstTables = this.GetAllowedTables(baseTableID);

        //    sbTable.Append("<table id=\"tbl\" class=\"datatbl\" style=\"width: 710px;\" border=\"1\">");
        //    sbTable.Append("<thead><tr>");
        //    sbTable.Append("<th width=\"20\"><img src=\"/shared/images/icons/delete2.gif\" height=\"16\" width=\"16\" alt=\"X\" title=\"X\" /></th>");
        //    sbTable.Append("<th style=\"width: 150px;\">Element</th>");
        //    sbTable.Append("<th style=\"width: 150px;\">Field</th>");
        //    sbTable.Append("<th style=\"width: 150px;\">Operator</th>");
        //    sbTable.Append("<th style=\"width: 290px;\">Value 1</th>");
        //    sbTable.Append("<th style=\"width: 145px;");
        //    if (update == true)
        //    {
        //        sbTable.Append(" display:none;");
        //    }
        //    sbTable.Append("\">Value 2</th>");
        //    sbTable.Append("<th style=\"width: 40px;\"");

        //    if (update == false)
        //    {
        //        sbTable.Append(" style=\"display: none;\"");
        //    }

        //    sbTable.Append(">Runtime</th></tr></thead>");
        //    sbTable.Append("<tbody>");

        //    for (rowIndex = 0; rowIndex < criteria.Length; rowIndex++)
        //    {
        //        fieldID = new Guid((string)criteria[rowIndex][0]);

        //        reqField = clsFields.getFieldById(fieldID);

        //        conditionType = Convert.ToInt16(criteria[rowIndex][1]);
        //        criteriaMode = (CriteriaMode)Convert.ToInt32(criteria[rowIndex][2]);
        //        runtime = Convert.ToBoolean(criteria[rowIndex][3]);
        //        valueOne = Convert.ToString(criteria[rowIndex][4]);
        //        valueTwo = Convert.ToString(criteria[rowIndex][5]);

        //        sbTable.Append("<tr id=\"" + rowIndex + "\">\n");
        //        sbTable.Append("<td classname=\"fieldstbl\"><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_1\"><img src=\"/shared/images/icons/");

        //        if (rowIndex == 0)
        //        {
        //            sbTable.Append("delete_disabled.png\"");
        //        }
        //        else
        //        {
        //            sbTable.Append("delete2.gif\" onclick=\"javascript:deleteRow(" + rowIndex + ");\" ");
        //        }

        //        sbTable.Append(" alt=\"Delete criteria\" title=\"Delete criteria\" align=\"absmiddle\" border=\"0\"></span></td>\n");
        //        sbTable.Append("<td id=\"" + rowIndex + "_0\"><span classname=\"hiddenclass\" class=\"hiddenclass\" id=\"" + rowIndex + "_2\"><a href=\"javascript:void(0);\">Element</a></span><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_3\"><select id=\"" + rowIndex + "_ddlTable\" onchange=\"selectTable(" + rowIndex + ");\">");

        //        foreach (sTableBasics table in lstTables)
        //        {
        //            sbTable.Append("<option value=\"" + table.TableID.ToString() + "\"");

        //            if (table.TableID == reqField.tableid)
        //            {
        //                sbTable.Append(" selected");
        //            }

        //            sbTable.Append(">" + table.Description + "</option>");
        //        }

        //        List<sFieldBasics> lstFields = this.GetTableFields(reqField.tableid.ToString(), update);

        //        sbTable.Append("</select></span></td>\n");
        //        sbTable.Append("<td><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_4\"><select id=\"" + rowIndex + "_ddlField\" onchange=\"selectField(" + rowIndex + ");\">");

        //        foreach (sFieldBasics field in lstFields)
        //        {
        //            sbTable.Append("<option value=\"" + field.FieldID.ToString() + "\"");

        //            if (field.FieldID == reqField.fieldid)
        //            {
        //                sbTable.Append(" selected");
        //            }

        //            sbTable.Append(">" + field.Description + "</option>");
        //        }



        //        sbTable.Append("</select></span></td>\n");

        //        object[] conditions = SelectableConditions(reqField.fieldid.ToString());

        //        #region Operator
        //        sbTable.Append("<td><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_5\"><select id=\"" + rowIndex + "_ddlCondition\" onchange=\"selectedOperatorChanged(" + rowIndex + ");\">");

        //        for (int i = 0; i < ((List<ListItem>)conditions[0]).Count; i++)
        //        {
        //            sbTable.Append("<option value=\"" + ((ListItem)((List<ListItem>)conditions[0])[i]).Value + "\"");

        //            if (conditionType.ToString() == ((ListItem)((List<ListItem>)conditions[0])[i]).Value)
        //            {
        //                sbTable.Append(" selected");
        //            }

        //            sbTable.Append(">" + ((ListItem)((List<ListItem>)conditions[0])[i]).Text + "</option>");
        //        }

        //        sbTable.Append("</select></span></td>\n");
        //        #endregion Operator

        //        #region Value One

        //        sbTable.Append("<td><span classname=\"visableclass\" class=\"visableclass\" id=\"" + rowIndex + "_6\">");

        //        if (((List<ListItem>)conditions[1]).Count == 0)
        //        {
        //            if (reqField.fieldtype == "X")
        //            {
        //                sbTable.Append("<select id=\"" + rowIndex + "_value\">");

        //                sbTable.Append("<option value=\"1\"");

        //                if (valueOne.ToString() == "1")
        //                {
        //                    sbTable.Append(" selected");
        //                }

        //                sbTable.Append(">Yes</option>");

        //                sbTable.Append("<option value=\"0\"");

        //                if (valueOne.ToString() == "0")
        //                {
        //                    sbTable.Append(" selected");
        //                }

        //                sbTable.Append(">No</option>");


        //                sbTable.Append("</select>");
        //            }
        //            else
        //            {
        //                sbTable.Append("<input id=\"" + rowIndex + "_value\" size=\"10\" type=\"text\" value=\"" + valueOne.ToString().Replace("\"", "&quot;") + "\">");
        //            }
        //        }
        //        else
        //        {
        //            sbTable.Append("<select id=\"" + rowIndex + "_value\">");

        //            string isSelected = string.Empty;
        //            string liValue = string.Empty;
        //            string liText = string.Empty;

        //            for (int i = 0; i < ((List<ListItem>)conditions[1]).Count; i++)
        //            {
        //                liValue = ((ListItem)((List<ListItem>)conditions[1])[i]).Value;
        //                liText = ((ListItem)((List<ListItem>)conditions[1])[i]).Text;

        //                isSelected = (valueOne == liValue) ? " selected=\"selected\"" : string.Empty;

        //                sbTable.Append("<option value=\"" + liValue + "\"" + isSelected + ">" + liText + "</option>");
        //            }

        //            sbTable.Append("</select>");
        //        }

        //        sbTable.Append("</span></td>\n");
        //        #endregion Value One

        //        #region Value Two

        //        sbTable.Append("<td");

        //        if (update == true)
        //        {
        //            sbTable.Append(" style=\"display: none;\"");
        //        }

        //        sbTable.Append("><span style=\"\" id=\"" + rowIndex + "_7\"");

        //        sbTable.Append(">");

        //        sbTable.Append("<input id=\"" + rowIndex + "_value2\" size=\"10\" type=\"text\"");

        //        if (conditionType != 8)
        //        {
        //            sbTable.Append(" style=\"display:none;\"");
        //        }
        //        else
        //        {
        //            sbTable.Append(" value=\"" + valueTwo.ToString() + "\"");
        //        }

        //        sbTable.Append(">");

        //        sbTable.Append("</span></td>\n");

        //        #endregion Value Two

        //        sbTable.Append("<td align=\"center\"");

        //        if (update == false)
        //        {
        //            sbTable.Append(" style=\"display: none;\"");
        //        }

        //        sbTable.Append("><span><input value=\"true\" id=\"" + rowIndex + "_runtime\" type=\"checkbox\"");

        //        if (runtime == true)
        //        {
        //            sbTable.Append(" checked=\"checked\"");
        //        }

        //        sbTable.Append("></span></td>\n");
        //        sbTable.Append("</tr>\n");
        //    }

        //    sbTable.Append("</tbody></table>");


        //    lstReturnStrings.Add(sbTable.ToString());
        //    lstReturnStrings.Add(rowIndex.ToString());

        //    return lstReturnStrings;
        //}

        /// <summary>
        /// Returns a list of tables that you can join your base table on to.
        /// </summary>
        /// <param name="baseTableID">Your base tableID as a string</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<sTableBasics> GetAllowedTables(string baseTableID)
        {
            Guid gBaseTableID;
            if (string.IsNullOrEmpty(baseTableID) == true || !Guid.TryParseExact(baseTableID, "D", out gBaseTableID))
            {
                throw new FormatException("baseTableID can not be converted to a Guid");
            }
            
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cTables clsTables = new cTables(currentUser.AccountID);
            List<sTableBasics> lstTables = clsTables.getAllowedTables(gBaseTableID, cAccounts.getConnectionString(currentUser.AccountID));
            return lstTables;
        }

        /// <summary>
        /// Returns a list of tables that you can join your entity on to.
        /// </summary>
        /// <param name="entityID">Your entity id</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<sTableBasics> GetEntityTables(int entityID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsEntities = new cCustomEntities(currentUser);
            cCustomEntity reqEntity = clsEntities.getEntityById(entityID);
            List<sTableBasics> lstTables = new List<sTableBasics>();
            cTables clsTables = new cTables(currentUser.AccountID);
            lstTables = clsTables.getAllowedTables(reqEntity.table.TableID, cAccounts.getConnectionString(currentUser.AccountID));
            return lstTables;
        }

        /// <summary>
        /// A list of cField option elements to be used in a select dropdown
        /// </summary>
        /// <param name="tableID">Table Guid in string form</param>
        /// <returns>List of ListItem made from cFields for the Table</returns>
        [WebMethod(EnableSession = true)]
        public List<ListItem> GetFieldsListItemsByTableID(string tableID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cFields clsFields = new cFields(currentUser.AccountID);
            List<cField> fields = clsFields.GetFieldsByTableID(new Guid(tableID));

            List<ListItem> lstFields = new List<ListItem>();
            if (fields.Count > 0)
            {
                fields.Sort(delegate(cField f1, cField f2) { return f1.Description.CompareTo(f2.Description); });
                lstFields.AddRange(from field in fields where field.FieldType != "L" select new ListItem(field.Description, field.FieldID.ToString()));
            }

            return lstFields;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldid"></param>
        /// <returns></returns>
        public List<ListItem> GetGenListItemsForField(Guid fieldid)
        {                        
            return FieldFilters.GetGenListItemsForField(fieldid, cMisc.GetCurrentUser());
        }

        /// <summary>
        /// Generate the html for the value field
        /// </summary>
        /// <param name="fieldID">field Guid</param>
        /// <returns>HTML element</returns>
        [WebMethod(EnableSession = true)]
        public string GetFilterValueElementByFieldID(string fieldID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cFields clsFields = new cFields(currentUser.AccountID);
            cField reqField = clsFields.GetFieldByID(new Guid(fieldID));

            StringBuilder elementHTML = new StringBuilder();
            if (reqField.GenList == true)
            {
                #region genlist
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));
                cField stringKeyField = clsFields.GetFieldByID(reqField.GetLookupTable().StringKeyFieldID);
                string strSQL = "SELECT [" + reqField.GetLookupTable().GetPrimaryKey().FieldName + "] AS primaryKey, [" + stringKeyField.FieldName + "] AS keyField FROM [" + reqField.GetLookupTable().TableName + "] ORDER BY [" + stringKeyField.FieldName + "];";
                int primaryKey;
                string keyField;
                List<ListItem> lstValues = new List<ListItem>();

                using (System.Data.SqlClient.SqlDataReader reader = expdata.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        primaryKey = reader.GetInt32(0); // primaryKey
                        keyField = reader.GetString(1); // keyField
                        lstValues.Add(new ListItem(keyField, primaryKey.ToString()));
                    }

                    reader.Close();
                }

                lstValues.Sort(delegate(ListItem li1, ListItem li2) { return li1.Text.CompareTo(li2.Text); });

                elementHTML.Append("<select>");
                foreach (ListItem li in lstValues)
                {
                    elementHTML.Append("<option value=\"" + li.Value + "\">" + li.Text + "</option>");
                }
                elementHTML.Append("<option value=\"@ME_ID\">@ME_ID</option>");
                elementHTML.Append("<option value=\"@ME\">@ME</option>");
                elementHTML.Append("<option value=\"@ACTIVEMODULE_ID\">@ACTIVEMODULE_ID</option>");
                elementHTML.Append("<option value=\"@ACTIVEMODULE\">@ACTIVEMODULE</option>");
                elementHTML.Append("<option value=\"@ACTIVESUBACCOUNT_ID\">@ACTIVESUBACCOUNT_ID</option>");
                elementHTML.Append("</select>");
                #endregion genlist
            }
            else if (reqField.ValueList == true)
            {
                #region valuelist
                elementHTML.Append("<select>");
                foreach (int k in reqField.ListItems.Keys)
                {
                    elementHTML.Append("<option value=\"" + k.ToString() + "\">" + reqField.ListItems[k].ToString() + "</option>");
                }
                elementHTML.Append("<option value=\"@ME_ID\">@ME_ID</option>");
                elementHTML.Append("<option value=\"@ME\">@ME</option>");
                elementHTML.Append("<option value=\"@ACTIVEMODULE_ID\">@ACTIVEMODULE_ID</option>");
                elementHTML.Append("<option value=\"@ACTIVEMODULE\">@ACTIVEMODULE</option>");
                elementHTML.Append("<option value=\"@ACTIVESUBACCOUNT_ID\">@ACTIVESUBACCOUNT_ID</option>");
                elementHTML.Append("</select>");
                #endregion valuelist
            }
            else
            {
                elementHTML.Append("<input type=\"text\" maxlength=\"150\" />");
            }

            return elementHTML.ToString();
        }

        /// <summary>
        /// Reject a custom entity from approvals view
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="recordid"></param>
        /// <param name="entityID"></param>
        [WebMethod(EnableSession = true)]
        public int Reject(int recordid, int entityID, string message)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cWorkflows clsworkflows = new cWorkflows(currentUser);
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);

            int workflowID = clsworkflows.GetWorkflowIDForEntity(clsCustomEntities.getEntityById(entityID).table, recordid);

            cWorkflow reqWorkflow = clsworkflows.GetWorkflowByID(workflowID);
            clsworkflows.UpdateApprovalStep(recordid, reqWorkflow.workflowid, false, message);
            return recordid;
        }

        [Obsolete] // only seems to be referenced in commented out code
        [WebMethod(EnableSession = true)]
        public object[] CheckForDuplicate(int entityId, int uniqueAttributeId, string uniqueFieldValue, int tabId, string controlID)
        {
            List<object> retVals = new List<object>();

            CurrentUser user = cMisc.GetCurrentUser();
            cCustomEntities clsEntities = new cCustomEntities(user);
            cCustomEntity entity = clsEntities.getEntityById(entityId);
            cTables clsTables = new cTables(user.AccountID);
            cFields clsFields = new cFields(user.AccountID);
            cQueryBuilder qb = new cQueryBuilder(user.AccountID, cAccounts.getConnectionString(user.AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, entity.table, clsTables, clsFields);
            qb.addColumn(clsFields.GetFieldByID(entity.getAttributeById(uniqueAttributeId).fieldid));
            //qb.addFilter(new cQueryFilter(clsFields.GetFieldByID(entity.getAttributeById(uniqueAttributeId).fieldid), ConditionType.Equals, new List<object>() { uniqueFieldValue }, null, ConditionJoiner.None)); UNCOMMENT IF THIS METHOD AS A WHOLE NEEDS TO BE USED AGAIN
            if (qb.GetCount() > 0)
                retVals.Add(false);
            else
                retVals.Add(true);

            retVals.Add(tabId);
            retVals.Add(controlID);
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public object[] GetUniqueFieldNames(int entityId, string controlID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cCustomEntities clsEntities = new cCustomEntities(user);
            cCustomEntity entity = clsEntities.getEntityById(entityId);
            List<object> retVals = new List<object>();
            retVals.Add(entityId);
            retVals.Add(entity.getUniqueAttributes());
            retVals.Add(entity.getUniqueAttributesFieldNames());
            retVals.Add(controlID);
            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public int DeleteEntity(int accountid, int entityid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.CustomEntities, true))
            {
                cCustomEntities clsentities = new cCustomEntities(user);
                int delegateID = 0;
                if (user.Delegate != null)
                {
                    delegateID = user.Delegate.EmployeeID;
                }

                int successCode = clsentities.deleteEntity(entityid, user.EmployeeID, delegateID);
                return successCode;
            }
            else
            {
                return -3;
            }
        }

        /// <summary>
        /// Returns a collection of list items for a drop down list containing tables or entities that can have a relationship established to them
        /// </summary>
        /// <param name="entityid">Current Entity ID</param>
        /// <param name="relationshipType">Relationship type (n:1 = 1; 1:n = 2)</param>
        /// <param name="isSummary"></param>
        /// <param name="excludeExistingOneToManys">Exclude any 1:n entities that already have a relationship on this Entity</param>
        /// <returns>List of ListItem elements</returns>
        [WebMethod(EnableSession = true)]
        public List<ListItem> getRelationshipDropDown(int entityid, int relationshipType, bool isSummary, bool excludeExistingOneToManys)
        {
            List<ListItem> relationshipDropDown = cCustomEntityRelationships.getRelationshipDropDownByRelationshipType(entityid, relationshipType, isSummary, excludeExistingOneToManys);
            return relationshipDropDown;
        }

        /// <summary>
        /// Gets the current selections for match fields when editing a n:1 attribute
        /// </summary>
        /// <param name="attributeid">Attribute ID</param>
        /// <returns>A list of items representing relationship match selections</returns>
        [WebMethod(EnableSession = true)]
        public List<ListItem> getRelationshipMatchSelections(int attributeid)
        {
            return cCustomEntities.getRelationshipMatchSelections(attributeid);
        }

        /// <summary>
        /// Gets the current selections for match fields when editing a n:1 attribute
        /// </summary>
        /// <param name="attributeid">Attribute ID</param>
        /// <returns>A list of items representing relationship match selections</returns>
        [WebMethod(EnableSession = true)]
        public List<ListItem> GetAutocompleteResultDisplaySelection(int attributeid)
        {
            var currentUser = cMisc.GetCurrentUser();
            var entities = new cCustomEntities(currentUser);
            var entity = entities.getEntityById(entities.getEntityIdByAttributeId(attributeid));
            var attribute = entity.getAttributeById(attributeid);
            var fields = new cFields(currentUser.AccountID);
            return entities.GetAutocompleteResultDisplaySelection(attribute, fields);
        }

        /// <summary>
        /// Evaluates the entity instances to see if the data for a particular attribute is unique or not.
        /// </summary>
        /// <param name="entityid">Entity ID</param>
        /// <param name="attributeid">Attribute ID</param>
        /// <returns>True if Unique</returns>
        [WebMethod(EnableSession = true)]
        public bool IsCustomEntityAttributeUniqueInInstances(int entityid, int attributeid)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cCustomEntities clsEntities = new cCustomEntities(curUser);

            return clsEntities.IsCustomEntityAttributeUniqueInInstances(entityid, attributeid);
        }
        
        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetInitialTreeNodes(int entityID)
        {
            cCustomEntities clsCustomEntities = new cCustomEntities(cMisc.GetCurrentUser());
            return clsCustomEntities.GetEntityData(entityID);
        }

		[WebMethod(EnableSession = true)]
		public JavascriptTreeData GetInitialTreeNodesForManyToOne(string relatedTable)
		{
			cCustomEntities customEntities = new cCustomEntities(cMisc.GetCurrentUser());
			return customEntities.GetManyToOneData(relatedTable);
		}

        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetBranchNodes(string fieldID, string crumbs, string nodeID)
        {
            Guid gFieldID;
            if (string.IsNullOrWhiteSpace(fieldID) || !Guid.TryParseExact(fieldID, "D", out gFieldID))
                return new JavascriptTreeData();
            //throw new FormatException("baseTableID can not be converted to a Guid");

            #region ensure the id with concatenated fieldids is as we expect
            if (string.IsNullOrWhiteSpace(nodeID) == false)
            {
                foreach (string s in nodeID.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Guid tmpGuid;
                    // need to skip the first character as this is a prefix and not part of the Guid
                    if (!Guid.TryParseExact(s.Substring(1), "D", out tmpGuid))
                        throw new FormatException("baseTableID can not be converted to a Guid");
                }
            }
            #endregion ensure the id with concatenated fieldids is as we expect

            cCustomEntities clsCustomEntities = new cCustomEntities(cMisc.GetCurrentUser());

            return clsCustomEntities.GetNodeData(gFieldID, crumbs, nodeID);
        }

        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetSelectedNodes(int entityID, int viewID)
        {
            return new cCustomEntities(cMisc.GetCurrentUser()).GetEntitySelectedColumnData(entityID, viewID);
        }

        /// <summary>
        /// Many To One treecombo drop area's initial load
        /// </summary>
        /// <param name="entityId">
        /// The greenlight id
        /// </param>
        /// <param name="attributeId">
        /// The greenlight Many To One attribute id
        /// </param>
        /// <param name="formId">
        /// The formid of greenlight Many To One attribute id
        /// </param>
        /// <param name="isParentFilter">
        /// Is this Parent Filter.
        /// </param>
        /// <returns>
        /// </returns>
		[WebMethod(EnableSession = true)]
        public JavascriptTreeData GetSelectedNodesForManyToOne(int entityId, int attributeId, int formId,bool isParentFilter)
		{
            return new cCustomEntities(cMisc.GetCurrentUser()).GetManyToOneSelectedColumnData(entityId, attributeId, formId, isParentFilter);
		}

        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetSelectedFilterNodes(int entityID, int viewID)
        {
            ICurrentUser user = cMisc.GetCurrentUser();
            return new cCustomEntities(user).GetEntitySelectedFilterData(entityID, viewID, user.AccountID, user.CurrentSubAccountId);
        }

        /// <summary>
        /// Many To One treecombo drop area's initial load showing Lookup Display Fields
        /// </summary>
        /// <param name="entityId">The green light id</param>
        /// <param name="attributeId">The green light Many To One attribute id</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public JavascriptTreeData GetSelectedNodesForLookupDisplayFields(int entityId, int attributeId)
        {
            return new cCustomEntities(cMisc.GetCurrentUser()).GetManyToOneSelectedLookupDisplayFields(entityId, attributeId);
        }

        #endregion

        /// <summary>
        /// Search all icons in the static library by file name
        /// </summary>
        /// <param name="fileName">The file name to search for</param>
        /// <param name="searchStartNumber">The number to start returning results from UPDATE THIS PLEASE</param>
        /// <returns>A list of file paths for the matching icons</returns>
        [WebMethod(EnableSession = true)]
        public ViewMenuIconResults SearchStaticIconsByFileName(string fileName, int searchStartNumber)
        {
            return cCustomEntities.GetViewIconsByName(fileName, searchStartNumber, GlobalVariables.StaticContentFolderPath, GlobalVariables.StaticContentLibrary);
        }

        /// <summary>
        /// Save the choice of the user due to a prompt for form selection purposes in GreenLight
        /// </summary>
        /// <returns>-1 on error</returns>
        [WebMethod(EnableSession = true)]
        public int SaveFormSelectionAttributeValue(int entityId, int viewId, string textValue, int? listValue)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityId);

            if (entity != null && entity.FormSelectionAttributeId.HasValue)
            {
                cAttribute attribute = entity.getAttributeById(entity.FormSelectionAttributeId.Value);
                cCustomEntityView view = entity.getViewById(viewId);

                if (view != null && view.AddFormMappings != null && view.AddFormMappings.Any())
                {
                    int formId;
                    if (attribute.fieldtype == FieldType.List && listValue.HasValue && listValue.Value >= 0)
                    {
                        formId = view.AddFormMappings.Where(x => x.ListValue == listValue.Value).Select(x => x.FormId).FirstOrDefault();

                        if (formId == 0 || entity.getFormById(formId).fields.Count == 0)
                        {
                            formId = view.DefaultAddForm.formid;
                        }

                        if (formId > 0)
                        {
                            return formId;
                        }
                    }
                    else if (attribute.fieldtype == FieldType.Text && textValue != null)
                    {
                        formId = view.AddFormMappings.Where(x => x.TextValue.ToUpperInvariant() == textValue.ToUpperInvariant()).Select(x => x.FormId).FirstOrDefault();

                        if (formId == 0 || entity.getFormById(formId).fields.Count == 0)
                        {
                            formId = view.DefaultAddForm.formid;
                        }

                        if (formId > 0)
                        {
                            return formId;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Get the correct edit formid for this entity
        /// </summary>
        /// <param name="entityId">The entity</param>
        /// <param name="viewId">The view that the entity is being edited from</param>
        /// <param name="id">The id of the entity record</param>
        /// <returns>An instance of a Custom Entity <see cref="CustomEntityFormDetails"/></returns>
        [WebMethod(EnableSession = true)]
        public CustomEntityFormDetails GetFormSelectionAttributeMappedEditFormId(int entityId, int viewId, int id)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityId);
            int listVal = 0;
            string textVal = string.Empty;
            var customEntityFormDetail = new CustomEntityFormDetails(-1, textVal, listVal);

            if (entity != null && entity.FormSelectionAttributeId.HasValue)
            {
                cAttribute attribute = entity.getAttributeById(entity.FormSelectionAttributeId.Value);
                cCustomEntityView view = entity.getViewById(viewId);

                if (view != null && view.EditFormMappings != null && view.EditFormMappings.Any())
                {
                    cTables clsTables = new cTables(currentUser.AccountID);
                    cFields clsFields = new cFields(currentUser.AccountID);
                    cQueryBuilder qb = new cQueryBuilder(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, entity.table, clsTables, clsFields);
                    qb.addColumn(clsFields.GetFieldByID(attribute.fieldid));
                    qb.addFilter(clsFields.GetFieldByID(entity.getKeyField().fieldid), ConditionType.Equals, new object[] { id }, null, ConditionJoiner.And, null);

                    using (IDataReader reader = qb.getReader())
                    {
                        if (reader.Read())
                        {
                            if (attribute.fieldtype == FieldType.List)
                            {
                                listVal = reader.IsDBNull(0) ? -1 : reader.GetInt32(0);
                            }
                            else
                            {
                                textVal = reader.IsDBNull(0) ? null : reader.GetString(0);
                            }
                        }
                    }

                    int formId;
                    if (attribute.fieldtype == FieldType.List && listVal >= 0 && view.EditFormMappings.Any(x => x.ListValue == listVal))
                    {
                        formId = view.EditFormMappings.Where(x => x.ListValue == listVal).Select(x => x.FormId).First();

                        if (formId == 0 || entity.getFormById(formId).fields.Count == 0)
                        {
                            formId = view.DefaultAddForm.formid;
                        }

                        if (formId > 0)
                        {
                            customEntityFormDetail.FormId = formId;
                            customEntityFormDetail.ListVal = listVal;
                            customEntityFormDetail.TextVal = textVal;
                            return customEntityFormDetail;
                        }
                    }
                    else if (attribute.fieldtype == FieldType.Text && textVal != null && view.EditFormMappings.Any(x => x.TextValue.ToUpperInvariant() == textVal.ToUpperInvariant()))
                    {
                        formId = view.EditFormMappings.Where(x => x.TextValue.ToUpperInvariant() == textVal.ToUpperInvariant()).Select(x => x.FormId).First();

                        if (formId == 0 || entity.getFormById(formId).fields.Count == 0)
                        {
                            formId = view.DefaultAddForm.formid;
                        }

                        if (formId > 0)
                        {
                            customEntityFormDetail.FormId = formId;
                            customEntityFormDetail.ListVal = listVal;
                            customEntityFormDetail.TextVal = textVal;
                            return customEntityFormDetail;
                        }
                    }
                }
            }
            
            return customEntityFormDetail;
        }

        /// <summary>
        /// Get the form selection attribute's info
        /// </summary>
        /// <param name="entityId">The GreenLight entity</param>
        /// <param name="attributeId">The attribute selected as the form selection attribute</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public object GetFormSelectionAttribute(int entityId, int attributeId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityId);
            cAttribute attribute = entity.getAttributeById(attributeId);

            if (attribute.GetType() == typeof(cListAttribute))
            {
                return new
                {
                    attributeType = "list",
                    attributeName = attribute.attributename,
                    displayName = attribute.displayname,
                    mandatory = attribute.mandatory,
                    items = ((cListAttribute)attribute).items.Select(x => new ListItem(x.Value.elementText, x.Value.elementValue.ToString())).ToList()
                };
            }
            else if (attribute.GetType() == typeof(cTextAttribute))
            {
                return new
                {
                    attributeType = "text",
                    attributeName = attribute.attributename,
                    displayName = attribute.displayname,
                    mandatory = attribute.mandatory,
                    maxLength = ((cTextAttribute)attribute).maxlength,
                    items = new List<ListItem>()
                };
            }

            return null;
        }


        /// <summary>
        /// Determine whether or not a custom entity has at least one form selection mapping
        /// </summary>
        /// <param name="entityId">The green light entity</param>
        /// <returns>A value indicating whether or not the custom entity has at least one form selection mapping</returns>
        [WebMethod(EnableSession = true)]
        public bool HasFormSelectionMappings(int entityId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomEntities clsentities = new cCustomEntities(currentUser);
            cCustomEntity entity = clsentities.getEntityById(entityId);

            return entity.HasFormSelectionMappings();
        }

        #region Private Methods

        /// <summary>
        /// Converts a cAttribute class object into sCEFieldDetails object for serialization to javascript for the form builder
        /// </summary>
        /// <param name="attribute">Attribute object to be converted into sCEFieldDetails object for form builder</param>
        /// <param name="IsReadonly">Specified whether the attribute is read-only (disabled)</param>
        /// <param name="fieldValue">Overrides the attributes default display name</param>
        /// <param name="displayName">Overrides the attribute field display name </param>
        /// <returns>Serializable CEFieldDetails object</returns>
        private sCEFieldDetails ConvertAttributeToCEFieldDetails(cAttribute attribute, bool IsReadonly = false, string fieldValue = "", string displayName = "")
        {
            #region Check text format and column span

            sCEFieldDetails CEFieldDetails = new sCEFieldDetails();

            AttributeFormat format = AttributeFormat.NotSet;
            int columnSpan = 1;
            int maxLength = 0;

            if (attribute.GetType() == typeof(cCommentAttribute) || attribute.GetType() == typeof(cSummaryAttribute) || attribute.GetType() == typeof(cOneToManyRelationship))
            {
                columnSpan = 2;
            }

            if (attribute.GetType() == typeof(cTextAttribute))
            {
                var textAttribute = (cTextAttribute)attribute;

                switch (textAttribute.format)
                {
                    case AttributeFormat.FormattedText:
                        format = AttributeFormat.FormattedText;
                        columnSpan = 2;
                        break;
                    case AttributeFormat.MultiLine:
                        format = AttributeFormat.MultiLine;
                        columnSpan = 2;
                        break;
                    case AttributeFormat.SingleLine:
                        format = AttributeFormat.SingleLine;
                        break;
                    case AttributeFormat.SingleLineWide:
                        format = AttributeFormat.SingleLineWide;
                        columnSpan = 2;
                        break;
                }

                if (textAttribute.maxlength != null)
                {
                    maxLength = textAttribute.maxlength.Value;
                }
            }

            if (attribute.GetType() == typeof(cListAttribute))
            {
                switch (((cListAttribute)attribute).format)
                {
                    case AttributeFormat.ListStandard:
                        format = AttributeFormat.ListStandard;
                        break;
                    case AttributeFormat.ListWide:
                        format = AttributeFormat.ListWide;
                        columnSpan = 2;
                        break;
                }
            }

            #endregion

            CEFieldDetails.AttributeID = attribute.attributeid;
            CEFieldDetails.ControlName = "form_attribute_" + attribute.attributeid;
            CEFieldDetails.DisplayName = displayName != string.Empty ? displayName : attribute.displayname;
            CEFieldDetails.Description = attribute.description;
            CEFieldDetails.FieldValue = !string.IsNullOrEmpty(fieldValue) ? fieldValue : string.Empty;
            CEFieldDetails.Tooltip = attribute.tooltip;
            CEFieldDetails.Mandatory = attribute.mandatory;
            CEFieldDetails.FieldType = attribute.fieldtype;
            CEFieldDetails.ReadOnly = IsReadonly;
            CEFieldDetails.Column = 0;
            CEFieldDetails.Row = 0;
            CEFieldDetails.Format = format;
            CEFieldDetails.RelationshipType = (attribute.GetType() == typeof(cManyToOneRelationship)) ? 1 : 2;
            CEFieldDetails.ColumnSpan = columnSpan;
            CEFieldDetails.MaxLength = maxLength;

            if (attribute.GetType() == typeof(cCommentAttribute))
            {
                CEFieldDetails.CommentText = ((cCommentAttribute)attribute).commentText;
            }

            if (attribute.GetType() == typeof(cManyToOneRelationship))
            {
                CEFieldDetails.SortDisplayName = CEFieldDetails.DisplayName + "!";
            }
            else
            {
                CEFieldDetails.SortDisplayName = fieldValue + CEFieldDetails.DisplayName;    
            }
            
            return CEFieldDetails;
        }

        private List<cCustomEntityFormTab> getTabs()
        {
            List<cCustomEntityFormTab> tabs = (List<cCustomEntityFormTab>)this.Session["tabs"];
            if (tabs == null)
            {
                tabs = new List<cCustomEntityFormTab>();
                this.Session["tabs"] = tabs;
            }

            return tabs;
        }

        #endregion
    }
}
