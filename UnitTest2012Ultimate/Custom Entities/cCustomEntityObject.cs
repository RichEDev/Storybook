using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Spend_Management;
using SpendManagementLibrary;
using System.Reflection;
using System.Text;

namespace UnitTest2012Ultimate
{
    using SpendManagementLibrary.Definitions.JoinVia;

    internal class cCustomEntityObject
    {
        public static cCustomEntity New(cCustomEntity entity = null, ICurrentUser currentUser = null)
        {
            currentUser = currentUser ?? Moqs.CurrentUser();
            entity = entity ?? Template();

            cCustomEntities clsCustomEntities = null;
            int entityID = -1;

            try
            {
                clsCustomEntities = new cCustomEntities(currentUser);
                entityID = clsCustomEntities.saveEntity(entity);
                cCustomEntities clsCustomEntities2 = new cCustomEntities(currentUser);
                entity = clsCustomEntities2.getEntityById(entityID);
            }
            catch (Exception e)
            {
                try
                {
                    #region Cleanup
                    if (entityID > 0 && clsCustomEntities != null)
                    {
                        clsCustomEntities.deleteEntity(entityID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                    }
                    #endregion Cleanup
                }
                finally
                {
                    throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cCustomEntityViewObject).ToString() + ">", e);
                }
            }

            return entity;
        }

        public static bool TearDown(int entityID)
        {
            if (entityID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    int successVal = clsCustomEntities.deleteEntity(entityID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return successVal == 0 ? true : false;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public static cCustomEntity Template(int entityID = 0, string entityName = "UT CE <DateTime.UtcNow.Ticks> <CallingMethodName>", string pluralName = "UT CES <DateTime.UtcNow.Ticks>", string description = "A unit test GreenLight object", DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, SortedList<int, cAttribute> attributes = null, SortedList<int, cCustomEntityForm> forms = null, SortedList<int, cCustomEntityView> views = null, cTable table = null, cTable audienceTable = null, bool enableAttachments = false, AudienceViewType audienceViewType = AudienceViewType.NoAudience, bool allowDocumentMergeAccess = false, bool systemView = false, int systemDerivedEntityID = 0, int systemViewEntityID = 0, bool enableCurrencies = false, int? defaultCurrencyID = null, bool enablePopupWindow = false, int? defaultPopupView = null, int? formSelectionAttributeId = null, int? ownerId = null, int? supportContactId = null, string supportQuestion = "", bool enableLocking = false, bool builtIn = false)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            //entityName = (entityName == "UT CE <DateTime.UtcNow.Ticks>") ? "UT CE " + dt : entityName;
            entityName = (entityName == "UT CE <DateTime.UtcNow.Ticks> <CallingMethodName>")
                                    ? "UT CE " + dt + " " + new StackFrame(1).GetMethod().Name
                                    : entityName;
            pluralName = (pluralName == "UT CES <DateTime.UtcNow.Ticks>") ? "UT CES " + dt : pluralName;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            
            return new cCustomEntity(
                entityid: entityID,
                entityname: entityName,
                pluralname: pluralName,
                description: description,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                attributes: attributes,
                forms: forms,
                views: views,
                table: table,
                audienceTable: audienceTable,
                enableattachments: enableAttachments,
                audienceViewType: audienceViewType,
                allowdocmergeaccess: allowDocumentMergeAccess,
                systemview: systemView,
                system_derivedentityid: systemDerivedEntityID,
                systemview_entityid: systemViewEntityID,
                enableCurrencies: enableCurrencies,
                defaultCurrencyID: defaultCurrencyID,
                enablePopupWindow: enablePopupWindow,
                defaultPopupView: defaultPopupView,
                formSelectionAttributeId: formSelectionAttributeId,
                ownerId: ownerId,
                supportContactId: supportContactId,
                supportQuestion: supportQuestion,
                enableLocking : enableLocking,
                builtIn : builtIn
            );
        }

        public static cCustomEntity BasicCurrencyType()
        {
            #region Get a default currency
            ICurrentUser currentUser = Moqs.CurrentUser();

            cCurrencies clsCurrencies = new cCurrencies(currentUser.AccountID, currentUser.CurrentSubAccountId);
            cCurrency oCurrency = clsCurrencies.getCurrencyByAlphaCode("GBP");

            if (oCurrency == null)
            {
                try
                {
                    cGlobalCurrencies clsGlobalCurrencies = new cGlobalCurrencies();
                    cGlobalCurrency oGlobalCurrency = clsGlobalCurrencies.getGlobalCurrencyByAlphaCode("GBP");

                    int currencyID = clsCurrencies.saveCurrency(new cCurrency(currentUser.AccountID, 0, oGlobalCurrency.globalcurrencyid, (byte)1, (byte)1, false, DateTime.UtcNow, currentUser.EmployeeID, null, null));
                    clsCurrencies = new cCurrencies(currentUser.AccountID, currentUser.CurrentSubAccountId);
                    oCurrency = clsCurrencies.getCurrencyById(currencyID);
                }
                catch (Exception e)
                {
                    throw new Exception("Saving a GBP currency for <" + typeof(cCustomEntityObject) + ">.BasicCurrencyType failed with the following exception", e);
                }
            }
            #endregion Get a default currency

            return cCustomEntityObject.Template(enableCurrencies: true, defaultCurrencyID: oCurrency.currencyid);
        }
    }

    internal class cCustomEntityViewObject
    {
        public static cCustomEntityView New(int entityID, cCustomEntityView entity = null)
        {
            entity = entity ?? Template();

            int viewID = 0;
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = null;
            try
            {
                #region Setup
                clsCustomEntities = new cCustomEntities(currentUser);
                viewID = clsCustomEntities.saveView(entityID, entity);
                cCustomEntities clsCustomEntities2 = new cCustomEntities(currentUser);
                entity = clsCustomEntities2.getEntityById(entityID).getViewById(viewID);
                #endregion Setup
            }
            catch (Exception e)
            {
                try
                {
                    #region Cleanup
                    if (viewID > 0 && clsCustomEntities != null)
                    {
                        clsCustomEntities.deleteView(viewID);
                    }
                    #endregion Cleanup
                }
                finally
                {
                    throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cCustomEntityViewObject).ToString() + ">", e);
                }
            }

            return entity;
        }

        public static cCustomEntityView Template(int viewID = 0, int entityID = 0, string viewName = "UT CE View <DateTime.UtcNow.Ticks>", string description = "A unit test GreenLight view object", bool builtIn = false, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, int? menuID = null, string menuDescription = "A unit test GreenLight view object menu", SortedList<byte, cCustomEntityViewField> fields = null, bool allowAdd = false, cCustomEntityForm addForm = null, bool allowEdit = false, cCustomEntityForm editForm = null, bool allowDelete = false, SortedList<byte, FieldFilter> filters = null, bool allowApproval = false, GreenLightSortColumn sortColumn = null)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            viewName = (viewName == "UT CE View <DateTime.UtcNow.Ticks>") ? "UT CE View " + dt : viewName;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            sortColumn = sortColumn ?? new GreenLightSortColumn(Guid.Empty, SortDirection.None, null);

            return new cCustomEntityView(
                viewid: viewID,
                entityid: entityID,
                viewname: viewName,
                description: description,
                builtIn: builtIn,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                menuid: menuID,
                menuDescription: menuDescription,
                fields: fields,
                allowadd: allowAdd,
                addform: addForm,
                allowedit: allowEdit,
                editform: editForm,
                allowdelete: allowDelete,
                filters: filters,
                allowapproval: allowApproval,
                SortColumn: sortColumn,
                systemCustomEntityViewId: null,
                showRecordCount:false,
                allowarchive:false       
            );
        }
    }

    internal class cCustomEntityFormObject
    {
        public static cCustomEntityForm New(int entityID, cCustomEntityForm entity = null)
        {
            entity = entity ?? Template();

            int formID = 0;
            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = null;
            try
            {
                #region Setup
                clsCustomEntities = new cCustomEntities(currentUser);
                formID = clsCustomEntities.saveForm(entityID, entity);
                cCustomEntities clsCustomEntities2 = new cCustomEntities(currentUser);
                entity = clsCustomEntities2.getEntityById(entityID).getFormById(formID);
                #endregion Setup
            }
            catch (Exception e)
            {
                try
                {
                    #region Cleanup
                    if (formID > 0 && clsCustomEntities != null)
                    {
                        clsCustomEntities.deleteForm(formID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                    }
                    #endregion Cleanup
                }
                finally
                {
                    throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cCustomEntityFormObject).ToString() + ">", e);
                }
            }

            return entity;
        }

        public static cCustomEntityForm Template(int formID = 0, int entityID = 0, string formName = "UT CE Form <DateTime.UtcNow.Ticks>", string description = "A unit test GreenLight form object", bool showSave = false, string saveText = "Save", bool showSaveAndDuplicate = false, string saveAndDuplicateText = "Save and Duplicate", bool showSaveAndStay = false, string saveAndStayText = "Save and Stay", bool showCancel = false, string cancelText = "Cancel", bool showSubMenus = false, bool showBreadcrumbs = false, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, SortedList<int, cCustomEntityFormTab> tabs = null, SortedList<int, cCustomEntityFormSection> sections = null, SortedList<int, cCustomEntityFormField> fields = null, string saveAndNewButtontext = "Save and New", bool showSaveAndNew = true)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            formName = (formName == "UT CE Form <DateTime.UtcNow.Ticks>") ? "UT CE Form " + dt : formName;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;

            return new cCustomEntityForm(
                formid: formID,
                entityid: entityID,
                formname: formName,
                description: description,
                showBreadcrumbs: showBreadcrumbs,
                showCancel: showCancel,
                cancelText: cancelText,
                showSave: showSave,
                saveText: saveText,
                showSaveAndDuplicate: showSaveAndDuplicate,
                saveAndDuplicateText: saveAndDuplicateText,
                showSaveAndStay: showSaveAndStay,
                saveAndStayText: saveAndStayText,
                showSubMenus: showSubMenus,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                tabs: tabs,
                sections: sections,
                fields: fields,
                showSaveAndNew: showSaveAndNew,
                saveAndNewText: saveAndNewButtontext);
        }

        public static cCustomEntityForm Basic(cCustomEntity entity = null)
        {
            cNumberAttribute attribute = (entity != null) ? (cNumberAttribute)entity.attributes.Values.Where(x => x.displayname == "ID").FirstOrDefault() : cNumberAttributeObject.Template();

            cCustomEntityFormTab tab = cCustomEntityFormTabObject.Template();
            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template(tab: tab);
            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: attribute, section: section);

            SortedList<int, cCustomEntityFormTab> lstTabs = new SortedList<int, cCustomEntityFormTab>() { { tab.order, tab } };
            SortedList<int, cCustomEntityFormSection> lstSections = new SortedList<int, cCustomEntityFormSection>() { { section.sectionid, section } };
            SortedList<int, cCustomEntityFormField> lstFields = new SortedList<int, cCustomEntityFormField>() { { field.attribute.attributeid, field } };

            return Template(tabs: lstTabs, sections: lstSections, fields: lstFields);
        }

        public static cCustomEntityForm BasicWithOneToManyFormAttribute(cCustomEntity entity, cCustomEntity relatedEntity)
        {
            cNumberAttribute numberAttribute = (cNumberAttribute)entity.attributes.Values.Where(x => x.displayname == "ID").FirstOrDefault();

            cOneToManyRelationship OTMRelationshipAttribute = cOneToManyRelationshipObject.New(entity.entityid, cOneToManyRelationshipObject.BasicOTM(entity.entityid, relatedEntity));

            cCustomEntityFormTab tab = cCustomEntityFormTabObject.Template();
            cCustomEntityFormSection section = cCustomEntityFormSectionObject.Template(tab: tab);
            cCustomEntityFormField fieldOne = cCustomEntityFormFieldObject.Template(attribute: numberAttribute, section: section);
            cCustomEntityFormField fieldTwo = cCustomEntityFormFieldObject.Template(attribute: OTMRelationshipAttribute, section: section);

            SortedList<int, cCustomEntityFormTab> lstTabs = new SortedList<int, cCustomEntityFormTab>() { { tab.order, tab } };
            SortedList<int, cCustomEntityFormSection> lstSections = new SortedList<int, cCustomEntityFormSection>() { { section.sectionid, section } };
            SortedList<int, cCustomEntityFormField> lstFields = new SortedList<int, cCustomEntityFormField>();
            lstFields.Add(fieldOne.attribute.attributeid, fieldOne);
            lstFields.Add(fieldTwo.attribute.attributeid, fieldTwo);

            return Template(tabs: lstTabs, sections: lstSections, fields: lstFields);
        }
    }

    internal class cCustomEntityFormTabObject
    {
        //public static cCustomEntityFormTab New(cCustomEntityFormTab entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cCustomEntityFormTab Template(int tabID = 0, int formID = 0, string headerCaption = "UT CE Form Tab <DateTime.UtcNow.Ticks>", byte order = 0)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            headerCaption = (headerCaption == "UT CE Form Tab <DateTime.UtcNow.Ticks>") ? "UT CE Form Tab " + dt : headerCaption;

            return new cCustomEntityFormTab(
                tabid: tabID,
                formid: formID,
                headercaption: headerCaption,
                order: order
            );
        }
    }

    internal class cCustomEntityFormSectionObject
    {
        //public static cCustomEntityFormSection New(cCustomEntityFormSection entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cCustomEntityFormSection Template(int sectionID = 0, int formID = 0, string headerCaption = "UT CE Form Section <DateTime.UtcNow.Ticks>", byte order = 0, cCustomEntityFormTab tab = null)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            headerCaption = (headerCaption == "UT CE Form Section <DateTime.UtcNow.Ticks>") ? "UT CE Form Section " + dt : headerCaption;

            return new cCustomEntityFormSection(
                sectionid: sectionID,
                formid: formID,
                headercaption: headerCaption,
                order: order,
                tab: tab
            );
        }
    }

    internal class cCustomEntityFormFieldObject
    {
        //public static cCustomEntityFormField New(cCustomEntityFormField entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cCustomEntityFormField Template(int formID = 0, cAttribute attribute = null, bool readOnly = false, cCustomEntityFormSection section = null, byte column = 0, byte row = 0, string labelText = "")
        {
            return new cCustomEntityFormField(
                formid: formID,
                attribute: attribute,
                breadonly: readOnly,
                section: section,
                column: column,
                row: row,
                labeltext: labelText
            );
        }
    }

    internal class cFieldFilterObject
    {
        //public static cFieldFilter New(cFieldFilter entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static FieldFilter Template(cField field = null, ConditionType conditionType = ConditionType.Equals, string valueOne = "", string valueTwo = "", byte order = 0, JoinVia via = null)
        {
            return new FieldFilter(
                field: field,
                conditiontype: conditionType,
                valueOne: valueOne,
                valueTwo: valueTwo,
                order: order,
                joinVia: via
            );
        }
    }

    internal class cHyperlinkAttributeObject
    {
        public static cHyperlinkAttribute New(int associatedCustomEntityID, cHyperlinkAttribute entity = null)
        {
            throw new InvalidOperationException("The <" + typeof(cHyperlinkAttributeObject).ToString() + "> is not used within GreenLight");

            //entity = (entity == null) ? Template() : entity;

            //if (associatedCustomEntityID < 1)
            //{
            //    throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid custom entity ID in <" + typeof(cHyperlinkAttributeObject).ToString() + ">.New");
            //}

            //ICurrentUser currentUser = Moqs.CurrentUser();
            //cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            //int attributeID = 0;

            //try
            //{
            //    #region Setup
            //    attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

            //    clsCustomEntities = new cCustomEntities(currentUser);
            //    cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
            //    entity = (cHyperlinkAttribute)customEntity.getAttributeById(attributeID);
            //    #endregion Setup
            //}
            //catch (Exception e)
            //{
            //    #region Cleanup
            //    if (attributeID > 0)
            //    {
            //        clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
            //    }
            //    #endregion Cleanup

            //    throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cHyperlinkAttributeObject).ToString() + ">", e);
            //}

            //return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cHyperlinkAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cHyperlinkAttributeObject).ToString() + ">.Delete");
            }
        }

        public static cHyperlinkAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false, FieldType fieldType = FieldType.NotSet, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false*/, bool isAuditIdentity = false, bool isUnique = false, bool allowEdit = false, bool allowDelete = false, string hyperlinkText = "HyperText <DateTime.UtcNow.Ticks>", string hyperlinkPath = "./<DateTime.UtcNow.Ticks>")
        {
            throw new InvalidOperationException("The <" + typeof(cHyperlinkAttributeObject).ToString() + "> is not used within GreenLight");

            //string dt = DateTime.UtcNow.Ticks.ToString();

            //string attributeName = "att" + attributeID;
            //displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            //toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            //fieldType = (fieldType == FieldType.NotSet) ? FieldType.Hyperlink : fieldType;
            //createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            //fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            //hyperlinkText = (hyperlinkText == "HyperText <DateTime.UtcNow.Ticks>") ? "HyperText " + dt : hyperlinkText;
            //hyperlinkPath = (hyperlinkPath == "./<DateTime.UtcNow.Ticks>") ? "./" + dt : hyperlinkPath;

            //return new cHyperlinkAttribute(
            //    attributeid: attributeID,
            //    attributename: attributeName,
            //    displayname: displayName,
            //    description: description,
            //    tooltip: toolTip,
            //    mandatory: mandatory,
            //    fieldtype: fieldType,
            //    createdon: createdOn,
            //    createdby: createdBy,
            //    modifiedon: modifiedOn,
            //    modifiedby: modifiedBy,
            //    fieldid: fieldID,
            //    //iskeyfield: isKeyField,
            //    isauditidentity: isAuditIdentity,
            //    isunique: isUnique,
            //    allowedit: allowEdit,
            //    allowdelete: allowDelete,

            //    hyperlinkText: hyperlinkText,
            //    hyperlinkPath: hyperlinkPath
            //);
        }
    }

    internal class cTextAttributeObject
    {
        public static cTextAttribute New(int associatedCustomEntityID, cTextAttribute entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cTextAttributeObject).ToString() + ">.New");
            }
            if (entity.fieldtype == FieldType.NotSet)
            {
                throw new ArgumentOutOfRangeException("The (cAttribute).fieldtype must be a valid text attribute FieldType in <" + typeof(cTextAttributeObject).ToString() + ">.New");
            }
            if (entity.format == AttributeFormat.NotSet)
            {
                throw new ArgumentOutOfRangeException("The (cAttribute).format must be a valid text attribute format in <" + typeof(cTextAttributeObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cTextAttribute)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cTextAttributeObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cTextAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cTextAttributeObject).ToString() + ">.Delete");
            }
        }

        public static cTextAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false, bool displayInMobile = false, bool builtIn = false, FieldType fieldType = FieldType.NotSet, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false*/, bool isAuditIdentity = false, bool isUnique = false, bool allowEdit = false, bool allowDelete = false, int? maxLength = null, AttributeFormat format = AttributeFormat.SingleLine)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            fieldType = (fieldType == FieldType.NotSet) ? FieldType.Text : fieldType;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            return new cTextAttribute(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                tooltip: toolTip,
                mandatory: mandatory,
                displayInMobile: displayInMobile,
                builtIn: builtIn,
                fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                //iskeyfield: isKeyField,
                isauditidentity: isAuditIdentity,
                isunique: isUnique,
                allowedit: allowEdit,
                allowdelete: allowDelete,

                maxlength: maxLength,
                format: format,
                removeFonts: false
            );
        }

        public static cTextAttribute BasicSingleLine(bool isUnique = false)
        {
            return Template(fieldType: FieldType.Text, format: AttributeFormat.SingleLine, isUnique: isUnique);
        }

        public static cTextAttribute BasicMultiLine(bool isUnique = false)
        {
            return Template(fieldType: FieldType.Text, format: AttributeFormat.MultiLine, isUnique: isUnique);
        }
    }

    internal class cDateTimeAttributeObject
    {
        public static cDateTimeAttribute New(int associatedCustomEntityID, cDateTimeAttribute entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cDateTimeAttributeObject).ToString() + ">.New");
            }
            if (entity.format == AttributeFormat.NotSet)
            {
                throw new ArgumentOutOfRangeException("The (cAttribute).format must be a valid datetime attribute format in <" + typeof(cDateTimeAttributeObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cDateTimeAttribute)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cDateTimeAttributeObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cDateTimeAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cDateTimeAttributeObject).ToString() + ">.Delete");
            }
        }

        //public static cDateTimeAttribute New(cDateTimeAttribute entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cDateTimeAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false, bool displayInMobile = false, bool builtIn =false, FieldType fieldType = FieldType.DateTime, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false*/, bool isAuditIdentity = false, bool isUnique = false, bool allowEdit = false, bool allowDelete = false, AttributeFormat format = AttributeFormat.NotSet)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            return new cDateTimeAttribute(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                tooltip: toolTip,
                mandatory: mandatory,
                displayInMobile: displayInMobile,
                builtIn: builtIn,
                fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                //iskeyfield: isKeyField,
                isauditidentity: isAuditIdentity,
                isunique: isUnique,
                allowedit: allowEdit,
                allowdelete: allowDelete,

                format: format
                );
        }

        public static cDateTimeAttribute BasicDateOnly()
        {
            return Template(format: AttributeFormat.DateOnly);
        }
    }

    internal class cNumberAttributeObject
    {
        public static cNumberAttribute New(int associatedCustomEntityID, cNumberAttribute entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cNumberAttributeObject).ToString() + ">.New");
            }
            if (entity.fieldtype == FieldType.NotSet)
            {
                throw new ArgumentOutOfRangeException("The (cAttribute).fieldtype must be a valid numeric attribute FieldType in <" + typeof(cNumberAttributeObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cNumberAttribute)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cNumberAttributeObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cNumberAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cNumberAttributeObject).ToString() + ">.Delete");
            }
        }

        //public static cNumberAttribute New(cNumberAttribute entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cNumberAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false, bool displayInMobile = false, bool builtIn = false, FieldType fieldType = FieldType.NotSet, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid(), bool isKeyField = false, bool isAuditIdentity = false, bool isUnique = false, bool allowEdit = false, bool allowDelete = false, byte precision = 2)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            fieldType = (fieldType == FieldType.NotSet) ? FieldType.Number : fieldType;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            return new cNumberAttribute(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                tooltip: toolTip,
                mandatory: mandatory,
                fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                iskeyfield: isKeyField,
                isauditidentity: isAuditIdentity,
                isunique: isUnique,
                allowedit: allowEdit,
                allowdelete: allowDelete,
                precision: precision,
                displayInMobile: displayInMobile,
                builtIn: builtIn

            );
        }

        public static cNumberAttribute BasicInteger(bool isUnique = false)
        {
            return Template(fieldType: FieldType.Integer, isUnique: isUnique);
        }
    }

    internal class cListAttributeObject
    {
        public static cListAttribute New(int associatedCustomEntityID, cListAttribute entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cListAttributeObject).ToString() + ">.New");
            }
            if (entity.fieldtype == FieldType.NotSet)
            {
                throw new ArgumentOutOfRangeException("The (cAttribute).fieldtype must be a valid list attribute FieldType in <" + typeof(cListAttributeObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cListAttribute)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cListAttributeObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cListAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cListAttributeObject).ToString() + ">.Delete");
            }
        }

        //public static cListAttribute New(cListAttribute entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cListAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool displayInMobile = false, bool builtIn = false, bool mandatory = false, FieldType fieldType = FieldType.NotSet, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false*/, bool isAuditIdentity = false, bool isUnique = false, AttributeFormat format = AttributeFormat.ListStandard, bool allowEdit = false, bool allowDelete = false, SortedList<int, cListAttributeElement> items = null)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            fieldType = (fieldType == FieldType.NotSet) ? FieldType.List : fieldType;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            return new cListAttribute(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                tooltip: toolTip,
                mandatory: mandatory,
                fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                //iskeyfield: isKeyField,
                isauditidentity: isAuditIdentity,
                isunique: isUnique,
                listformat: format,
                allowedit: allowEdit,
                allowdelete: allowDelete,
                items: items,
                displayInMobile: displayInMobile,
                builtIn: builtIn
            );
        }

        internal static cListAttribute BasicList(bool isUnique = false)
        {
            cListAttributeElement item1 = cListAttributeElementObject.Template(elementValue: 1, elementText: "Foo", sequence: 0);
            cListAttributeElement item2 = cListAttributeElementObject.Template(elementValue: 2, elementText: "Bar", sequence: 1);
            SortedList<int, cListAttributeElement> lstItems = new SortedList<int, cListAttributeElement> { { item2.elementOrder, item2 }, { item1.elementOrder, item1 } };

            return Template(fieldType: FieldType.List, items: lstItems, isUnique: isUnique);
        }
    }

    internal class cListAttributeElementObject
    {
        //public static cListAttributeElement New(cListAttributeElement entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cListAttributeElement Template(int elementValue = 0, string elementText = "UT CE List Attribute Element <DateTime.UtcNow.Ticks>", int sequence = 0)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            elementText = (elementText == "UT CE List Attribute Element <DateTime.UtcNow.Ticks>") ? "UT CE List Attribute Element " + dt : elementText;

            return new cListAttributeElement(
                elementText: elementText,
                elementValue: elementValue,
                sequence: sequence
            );
        }
    }

    internal class cTickboxAttributeObject
    {
        public static cTickboxAttribute New(int associatedCustomEntityID, cTickboxAttribute entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cTickboxAttributeObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cTickboxAttribute)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cTickboxAttributeObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cTickboxAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cTickboxAttributeObject).ToString() + ">.Delete");
            }
        }

        //public static cTickboxAttribute New(cTickboxAttribute entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cTickboxAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool displayInMobile = false, bool builtIn = false, bool mandatory = false, FieldType fieldType = FieldType.TickBox, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false*/, bool isAuditIdentity = false, bool isUnique = false, bool allowEdit = false, bool allowDelete = false, string defaultValue = "Default value for UT CE Tickbox Attribute <DateTime.UtcNow.Ticks>")
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            defaultValue = (defaultValue == "Default value for UT CE Tickbox Attribute <DateTime.UtcNow.Ticks>") ? "Default value for UT CE Tickbox Attribute " + dt : defaultValue;

            return new cTickboxAttribute(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                tooltip: toolTip,
                mandatory: mandatory,
                fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                //iskeyfield: isKeyField,
                isauditidentity: isAuditIdentity,
                isunique: isUnique,
                allowedit: allowEdit,
                allowdelete: allowDelete,
                defaultvalue: defaultValue,
                displayInMobile: displayInMobile,
                builtIn: builtIn
                );
        }
    }

    internal class cAttachmentAttributeObject
    {
        public static cAttachmentAttribute New(int associatedCustomEntityID, cAttachmentAttribute entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cAttachmentAttributeObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cAttachmentAttribute)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cAttachmentAttributeObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cAttachmentAttributeObject).ToString() + ">", e);
    }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cAttachmentAttributeObject).ToString() + ">.Delete");
            }
        }

        public static cAttachmentAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool displayInMobile = false, bool builtIn = false, bool mandatory = false, FieldType fieldType = FieldType.Attachment, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid(), AttributeFormat format = AttributeFormat.SingleLineWide, bool isAuditIdentity = false, bool isUnique = false, bool boolAttribute = false, bool allowEdit = false, bool allowDelete = false, bool system_attribute = false)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;
     
            return new cAttachmentAttribute(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                tooltip: toolTip,
                mandatory: mandatory,
                fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,        
                format: format,
                isauditidentity: isAuditIdentity,
                isunique: isUnique,
                includeImageLibrary: boolAttribute,
                allowedit: allowEdit,
                allowdelete: allowDelete,           
                system_attribute: system_attribute,
                displayInMobile: displayInMobile,
                builtIn: builtIn
            );
        }
    }

    internal class cRelationshipTextBoxAttributeObject
    {
        public static cRelationshipTextBoxAttribute New(int associatedCustomEntityID, cRelationshipTextBoxAttribute entity = null)
        {
            throw new InvalidOperationException("The <" + typeof(cRelationshipTextBoxAttributeObject).ToString() + "> is not used within GreenLight");

            //entity = (entity == null) ? Template() : entity;

            //if (associatedCustomEntityID < 1)
            //{
            //    throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cRelationshipTextBoxAttributeObject).ToString() + ">.New");
            //}

            //ICurrentUser currentUser = Moqs.CurrentUser();
            //cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            //int attributeID = 0;

            //try
            //{
            //    #region Setup
            //    attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

            //    clsCustomEntities = new cCustomEntities(currentUser);
            //    cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
            //    entity = (cRelationshipTextBoxAttribute)customEntity.getAttributeById(attributeID);
            //    #endregion Setup
            //}
            //catch (Exception e)
            //{
            //    #region Cleanup
            //    if (attributeID > 0)
            //    {
            //        clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
            //    }
            //    #endregion Cleanup

            //    throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cRelationshipTextBoxAttributeObject).ToString() + ">", e);
            //}

            //return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cRelationshipTextBoxAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cRelationshipTextBoxAttributeObject).ToString() + ">.Delete");
            }
        }

        //public static cRelationshipTextBoxAttribute New(cRelationshipTextBoxAttribute entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cRelationshipTextBoxAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false, FieldType fieldType = FieldType.RelationshipTextbox, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false*/, bool isAuditIdentity = false/*, bool isUnique = false*/, bool allowEdit = false, bool allowDelete = false, cTable relatedTable = null)
        {
            throw new InvalidOperationException("The <" + typeof(cRelationshipTextBoxAttributeObject).ToString() + "> is not used within GreenLight");
            //string dt = DateTime.UtcNow.Ticks.ToString();

            //string attributeName = "att" + attributeID;
            //displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            //toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            //createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            //fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            //return new cRelationshipTextBoxAttribute(
            //    attributeid: attributeID,
            //    attributename: attributeName,
            //    displayname: displayName,
            //    description: description,
            //    tooltip: toolTip,
            //    mandatory: mandatory,
            //    fieldtype: fieldType,
            //    createdon: createdOn,
            //    createdby: createdBy,
            //    modifiedon: modifiedOn,
            //    modifiedby: modifiedBy,
            //    fieldid: fieldID,
            //    //iskeyfield: isKeyField,
            //    isauditidentity: isAuditIdentity,
            //    //isunique: isUnique,
            //    allowedit: allowEdit,
            //    allowdelete: allowDelete,

            //    relatedtable: relatedTable
            //);
        }
    }

    internal class cManyToOneRelationshipObject
    {
        public static cManyToOneRelationship New(int associatedCustomEntityID, cManyToOneRelationship entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cManyToOneRelationshipObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveRelationship(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cManyToOneRelationship)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cManyToOneRelationshipObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cManyToOneRelationshipObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cManyToOneRelationshipObject).ToString() + ">.Delete");
            }
        }

        //public static cManyToOneRelationship New(cManyToOneRelationship entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cManyToOneRelationship Template(bool displayInMobile = false, int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false, bool builtIn = false, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid(), bool isAuditIdentity = false, bool allowEdit = true, bool allowDelete = true, cTable relatedTable = null, cTable aliasTable = null, Guid autocompleteDisplayFieldID = default(Guid), List<Guid> autoCompleteMatchFieldIDs = null, int autoCompleteMaxRows = 15)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            List<object> acVals = (relatedTable != null ? AutoComplete.getAutoCompleteQueryParams(relatedTable.TableName) : null);
            if (acVals != null)
            {
                autocompleteDisplayFieldID = (autocompleteDisplayFieldID == default(Guid) ? (Guid)acVals[0] : autocompleteDisplayFieldID);
                    autoCompleteMatchFieldIDs = (autoCompleteMatchFieldIDs == null ? (List<Guid>)acVals[1] : autoCompleteMatchFieldIDs);
            }

            return new cManyToOneRelationship(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                tooltip: toolTip,
                mandatory: mandatory,
                builtIn: builtIn,
                //fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                //iskeyfield: isKeyField,
                isauditidentity: isAuditIdentity,
                //isunique: isUnique,
                allowedit: allowEdit,
                allowdelete: allowDelete,

                relatedtable: relatedTable,
                AliasTable: aliasTable,
                autoCompleteDisplayFieldID: autocompleteDisplayFieldID,
                autoCompleteMatchFieldIDs: autoCompleteMatchFieldIDs,
                maxRows: autoCompleteMaxRows);
        }

        public static cManyToOneRelationship BasicMTO()
        {
            cTables clsTables = new cTables();
            return cManyToOneRelationshipObject.Template(relatedTable: clsTables.GetTableByName("employees"));
        }
    }

    /// <summary>
    /// Internal class for testing LookupDisplayField
    /// </summary>
    internal class LookupDisplayFieldObject
    {
        /// <summary>
        /// Create a dummy LookupField object attached to a custom entity
        /// </summary>
        /// <param name="associatedCustomEntityID">
        /// The associated custom entity id.
        /// </param>
        /// <param name="entity">
        /// The entity to add.
        /// </param>
        /// <returns>
        /// The <see cref="LookupDisplayField"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        public static LookupDisplayField New(int associatedCustomEntityID, LookupDisplayField entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(LookupDisplayField).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeId = 0;
            
            try
            {
                attributeId = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);
                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                foreach (var attribute in customEntity.attributes)
                {
                    if (attribute.Value.GetType() == typeof(cManyToOneRelationship))
                    {
                        cManyToOneRelationship manyToOne = (cManyToOneRelationship)attribute.Value;
                        manyToOne.TriggerLookupFields.Add(entity);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                if (attributeId > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeId, currentUser.EmployeeID, currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0);
                }
                

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(LookupDisplayField).ToString() + ">", e);
            }
            
            return entity;
        }

        /// <summary>
        /// A template for testing Lookup Fields
        /// </summary>
        /// <param name="attributeid">
        /// attribute id
        /// </param>
        /// <param name="attributename">
        /// The attribute name.
        /// </param>
        /// <param name="displayname">
        /// display name.
        /// </param>
        /// <param name="description">
        /// description.
        /// </param>
        /// <param name="tooltip">
        /// tooltip.
        /// </param>
        /// <param name="mandatory">
        /// is field mandatory.
        /// </param>
        /// <param name="fieldtype">
        /// field type.
        /// </param>
        /// <param name="createdby">
        /// created by user id.
        /// </param>
        /// <param name="fieldid">
        /// field id.
        /// </param>
        /// <param name="iskeyfield">
        /// is key field.
        /// </param>
        /// <param name="isauditidentity">
        /// is audit identity.
        /// </param>
        /// <param name="isunique">
        /// is unique.
        /// </param>
        /// <param name="allowEdit">
        /// allow edit.
        /// </param>
        /// <param name="allowDelete">
        /// allow delete.
        /// </param>
        /// <param name="system_attribute">
        /// system attribute.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static LookupDisplayField Template(int attributeid = 0, string attributename = "UT-LookupField-for-testing", string displayname = "UT LookupField for testing", string description = "UT-LookupField-for-testing", string tooltip = "UT-LookupField-for-testing", bool mandatory = false, FieldType fieldtype = FieldType.LookupDisplayField, int createdby = 0, Guid fieldid = new Guid(), bool iskeyfield = false, bool isauditidentity = false, bool isunique = false, bool allowEdit = false, bool allowDelete = false, bool system_attribute = false)
        {
            // Create a new ManyToOne in order to create a valid lookupField
            cTable employeesTable = new cTables().GetTableByName("employees");
            cCustomEntityFormField field = cCustomEntityFormFieldObject.Template(attribute: cManyToOneRelationshipObject.Template(relatedTable: employeesTable), readOnly: true);
            var triggerAttributeId = field.attribute.attributeid;
            var triggerJoinViaId = 0;
            var triggerJoinVia = new JoinVia(0, "test", new Guid());
            var triggerDisplayFieldId = field.attribute.fieldid;
            var createdon = DateTime.Parse("17/09/2012");
            return new LookupDisplayField(attributeid, attributename, displayname, description, tooltip, createdon, createdby, null, null, fieldid, triggerAttributeId, triggerJoinVia, triggerDisplayFieldId);
        }
    }

    internal class cOneToManyRelationshipObject
    {
        public static cOneToManyRelationship New(int associatedCustomEntityID, cOneToManyRelationship entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cManyToOneRelationshipObject).ToString() + ">.New");
            }

            if (entity.relatedtable == null)
            {
                throw new ArgumentOutOfRangeException("The (cAttribute).relatedtable must be a valid saved GreenLight table in <" + typeof(cOneToManyRelationshipObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveRelationship(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cOneToManyRelationship)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cOneToManyRelationshipObject).ToString() + ">", e);
            }

            return entity;
        }

        public static cOneToManyRelationship NewWithDerived(int associatedCustomEntityID, cOneToManyRelationship entity = null)
        {
            entity = entity ?? Template();

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cManyToOneRelationshipObject).ToString() + ">.New");
            }

            if (entity.relatedtable == null)
            {
                throw new ArgumentOutOfRangeException("The (cAttribute).relatedtable must be a valid saved GreenLight table in <" + typeof(cOneToManyRelationshipObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                int preSaveID = entity.attributeid;
                attributeID = clsCustomEntities.saveRelationship(associatedCustomEntityID, entity);

                clsCustomEntities.createDerivedTable(currentUser.EmployeeID, preSaveID, attributeID, clsCustomEntities.getEntityById(associatedCustomEntityID), entity.relatedtable, clsCustomEntities.getEntityByTableId(entity.relatedtable.TableID));

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cOneToManyRelationship)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cOneToManyRelationshipObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cOneToManyRelationshipObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cOneToManyRelationshipObject).ToString() + ">.Delete");
            }
        }

        public static cOneToManyRelationship Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false/*, FieldType fieldType = FieldType.NotSet*/, bool builtIn = false, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false*/, bool isAuditIdentity = false/*, bool isUnique = false*/, bool allowEdit = true, bool allowDelete = true, cTable relatedTable = null, int viewID = 0, int entityID = 0, int parentEntityID = 0)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            return new cOneToManyRelationship(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                tooltip: toolTip,
                mandatory: mandatory,
                builtIn: builtIn,
                //fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                //iskeyfield: isKeyField,
                isauditidentity: isAuditIdentity,
                //isunique: isUnique,
                allowedit: allowEdit,
                allowdelete: allowDelete,

                relatedtable: relatedTable,
                viewid: viewID,
                entityid: entityID,
                parent_entityid: parentEntityID
            );
        }

        public static cOneToManyRelationship BasicOTM(int entityID, cCustomEntity relatedCustomEntity)
        {
            cCustomEntityView view = relatedCustomEntity.Views.Values.FirstOrDefault();

            if (view == null)
            {
                throw new ArgumentOutOfRangeException("The GreenLight must have a valid view in <" + typeof(cManyToOneRelationshipObject).ToString() + ">.BasicOTM");
            }

            return cOneToManyRelationshipObject.Template(relatedTable: relatedCustomEntity.table, viewID: view.viewid, entityID: relatedCustomEntity.entityid, parentEntityID: entityID);
        }
    }

    internal class cRunWorkflowAttributeObject
    {
        public static cRunWorkflowAttribute New(int associatedCustomEntityID, cRunWorkflowAttribute entity = null)
        {
            throw new InvalidOperationException("The <" + typeof(cRunWorkflowAttributeObject).ToString() + "> is not currently used within GreenLight");

            //entity = (entity == null) ? Template() : entity;

            //if (associatedCustomEntityID < 1)
            //{
            //    throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid custom entity ID in <" + typeof(cRunWorkflowAttributeObject).ToString() + ">.New");
            //}

            //ICurrentUser currentUser = Moqs.CurrentUser();
            //cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            //int attributeID = 0;

            //try
            //{
            //    #region Setup
            //    attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

            //    clsCustomEntities = new cCustomEntities(currentUser);
            //    cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
            //    entity = (cRunWorkflowAttribute)customEntity.getAttributeById(attributeID);
            //    #endregion Setup
            //}
            //catch (Exception e)
            //{
            //    #region Cleanup
            //    if (attributeID > 0)
            //    {
            //        clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
            //    }
            //    #endregion Cleanup

            //    throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cRunWorkflowAttributeObject).ToString() + ">", e);
            //}

            //return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cRunWorkflowAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cRunWorkflowAttributeObject).ToString() + ">.Delete");
            }
        }

        //public static cRunWorkflowAttribute New(cRunWorkflowAttribute entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cRunWorkflowAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false/*, FieldType fieldType = FieldType.NotSet*/, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false, bool isAuditIdentity = false, bool isUnique = false*/, bool allowEdit = false, bool allowDelete = false, cWorkflow workflow = null)
        {
            throw new InvalidOperationException("The <" + typeof(cRunWorkflowAttributeObject).ToString() + "> is not currently used within GreenLight");
            //string dt = DateTime.UtcNow.Ticks.ToString();

            //string attributeName = "att" + attributeID;
            //displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            //toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            //createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            //fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            //return new cRunWorkflowAttribute(
            //    attributeid: attributeID,
            //    attributename: attributeName,
            //    displayname: displayName,
            //    description: description,
            //    tooltip: toolTip,
            //    mandatory: mandatory,
            //    //fieldtype: fieldType,
            //    createdon: createdOn,
            //    createdby: createdBy,
            //    modifiedon: modifiedOn,
            //    modifiedby: modifiedBy,
            //    fieldid: fieldID,
            //    //iskeyfield: isKeyField,
            //    //isauditidentity: isAuditIdentity,
            //    //isunique: isUnique,
            //    allowedit: allowEdit,
            //    allowdelete: allowDelete,

            //    workflow: workflow
            //);
        }

        public static cRunWorkflowAttribute BasicRunWorkflow()
        {
            throw new InvalidOperationException("The <" + typeof(cRunWorkflowAttributeObject).ToString() + "> is not currently used within GreenLight");
            //return Template(attributeName: "Run Workflow", workflow: cWorkflowObject.New());
        }
    }

    internal class cCurrencyListAttributeObject
    {
        public static cCurrencyListAttribute New(int associatedCustomEntityID, cCurrencyListAttribute entity = null)
        {
            entity = (entity == null) ? Template() : entity;

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cCurrencyListAttributeObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cCurrencyListAttribute)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cCurrencyListAttributeObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cCurrencyListAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cCurrencyListAttributeObject).ToString() + ">.Delete");
            }
        }

        //public static cCurrencyListAttribute New(cCurrencyListAttribute entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cCurrencyListAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute"/*, string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false, FieldType fieldType = FieldType.NotSet*/, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false, bool isAuditIdentity = false, bool isUnique = false*/, bool allowEdit = false, bool allowDelete = false, bool displayInMobile = false)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            //toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            return new cCurrencyListAttribute(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                //tooltip: toolTip,
                //mandatory: mandatory,
                //fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                //iskeyfield: isKeyField,
                //isauditidentity: isAuditIdentity,
                //isunique: isUnique,
                allowedit: allowEdit,
                allowdelete: allowDelete,
                displayInMobile: displayInMobile
            );
        }
    }

    internal class cSummaryAttributeObject
    {
        public static cSummaryAttribute New(int associatedCustomEntityID, cSummaryAttribute entity = null)
        {
            entity = (entity == null) ? Template() : entity;

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cSummaryAttributeObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveSummaryAttribute(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cSummaryAttribute)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cSummaryAttributeObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cSummaryAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cSummaryAttributeObject).ToString() + ">.Delete");
            }
        }

        public static cSummaryAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute"/*, string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool mandatory = false, FieldType fieldType = FieldType.NotSet*/, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false, bool isAuditIdentity = false, bool isUnique = false*/, bool allowEdit = false, bool allowDelete = false, Dictionary<int, cSummaryAttributeElement> summaryElements = null, Dictionary<int, cSummaryAttributeColumn> summaryColumns = null, int sourceEntityID = 0)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            //toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            return new cSummaryAttribute(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                //tooltip: toolTip,
                //mandatory: mandatory,
                //fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                //iskeyfield: isKeyField,
                //isauditidentity: isAuditIdentity,
                //isunique: isUnique,
                allowedit: allowEdit,
                allowdelete: allowDelete,

                summaryElements: summaryElements,
                summaryColumns: summaryColumns,
                source_entityid: sourceEntityID
            );
        }
    }

    internal class cSummaryAttributeElementObject
    {
        //public static cSummaryAttributeElement New(cSummaryAttributeElement entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cSummaryAttributeElement Template(int summaryAttributeID = 0, int attributeID = 0, int otmAttributeID = 0, int order = 0)
        {
            return new cSummaryAttributeElement(
                summaryattributeid: summaryAttributeID,
                attributeid: attributeID,
                otm_attributeid: otmAttributeID,
                order: order
            );
        }
    }

    internal class cSummaryAttributeColumnObject
    {
        //public static cSummaryAttributeColumn New(cSummaryAttributeColumn entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cSummaryAttributeColumn Template(int columnID = 0, int columnAttributeID = 0, string alternateHeader = "UT CE Summary Attribute Column <DateTime.UtcNow.Ticks>", int width = 0, int order = 0, bool sortColumn = false, string filterValue = "", bool isManyToOne = false, Guid displayFieldId = default(Guid), JoinVia joinViaObject = null)
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            alternateHeader = (alternateHeader == "UT CE Summary Attribute Column <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : alternateHeader;

            return new cSummaryAttributeColumn(
                columnattributeid: columnAttributeID,
                columnid: columnID,
                alternate_header: alternateHeader,
                width: width,
                order: order,
                sortColumn: sortColumn,
                filter_value: filterValue,
                ismanytooneattribute: isManyToOne,
                displayfieldid: displayFieldId,
                joinviaObj: joinViaObject

            );
        }
    }

    internal class cCommentAttributeObject
    {
        public static cCommentAttribute New(int associatedCustomEntityID, cCommentAttribute entity = null)
        {
            entity = (entity == null) ? Template() : entity;

            if (associatedCustomEntityID < 1)
            {
                throw new ArgumentOutOfRangeException("The associatedCustomEntityID must be a valid GreenLight ID in <" + typeof(cCommentAttributeObject).ToString() + ">.New");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();
            cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
            int attributeID = 0;

            try
            {
                #region Setup
                attributeID = clsCustomEntities.saveAttribute(associatedCustomEntityID, entity);

                clsCustomEntities = new cCustomEntities(currentUser);
                cCustomEntity customEntity = clsCustomEntities.getEntityById(associatedCustomEntityID);
                entity = (cCommentAttribute)customEntity.getAttributeById(attributeID);
                #endregion Setup
            }
            catch (Exception e)
            {
                #region Cleanup
                if (attributeID > 0)
                {
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));
                }
                #endregion Cleanup

                throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cCommentAttributeObject).ToString() + ">", e);
            }

            return entity;
        }

        public static bool TearDown(int attributeID)
        {
            if (attributeID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cCustomEntities clsCustomEntities = new cCustomEntities(currentUser);
                    clsCustomEntities.deleteAttribute(attributeID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0));

                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during teardown of unit test dummy object of type <" + typeof(cCommentAttributeObject).ToString() + ">", e);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The attributeID must be greater than 0 in <" + typeof(cCommentAttributeObject).ToString() + ">.Delete");
            }
        }

        //public static cAdviceAttribute New(cAdviceAttribute entity = null) { entity = (entity == null) ? Template() : entity; return entity; }

        public static cCommentAttribute Template(int attributeID = 0, string displayName = "UT CE Attribute <DateTime.UtcNow.Ticks>", string description = "A GreenLight attribute", string toolTip = "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>", bool displayInMobile = false, bool builtIn = false, bool mandatory = false/*, FieldType fieldType = FieldType.NotSet*/, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null, Guid fieldID = new Guid()/*, bool isKeyField = false*/, bool isAuditIdentity = false, bool isUnique = false, bool allowEdit = false, bool allowDelete = false, string adviceText = "Advice text for UT CE Advice Attribute <DateTime.UtcNow.Ticks>")
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            string attributeName = "att" + attributeID;
            displayName = (displayName == "UT CE Attribute <DateTime.UtcNow.Ticks>") ? "UT CE Attribute " + dt : displayName;
            toolTip = (toolTip == "Tooltip for UT CE Attribute <DateTime.UtcNow.Ticks>") ? "Tooltip for UT CE Attribute " + dt : toolTip;
            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;
            fieldID = (fieldID.ToString() == new Guid().ToString()) ? Guid.NewGuid() : fieldID;

            adviceText = (adviceText == "Advice text for UT CE Advice Attribute <DateTime.UtcNow.Ticks>") ? "Advice text for UT CE Advice Attribute " + dt : adviceText;

            return new cCommentAttribute(
                attributeid: attributeID,
                attributename: attributeName,
                displayname: displayName,
                description: description,
                tooltip: toolTip,
                mandatory: mandatory,
                builtIn: builtIn,
                //fieldtype: fieldType,
                createdon: createdOn,
                createdby: createdBy,
                modifiedon: modifiedOn,
                modifiedby: modifiedBy,
                fieldid: fieldID,
                //iskeyfield: isKeyField,
                isauditidentity: isAuditIdentity,
                isunique: isUnique,
                allowedit: allowEdit,
                allowdelete: allowDelete,
                displayInMobile: displayInMobile,
                commentText: adviceText
            );
        }
    }

    internal class CustomEntityRecord
    {
        /// <summary>
        /// Create a very basic record for an entity in the entity's table
        /// </summary>
        /// <param name="entity">The entity to insert a record for</param>
        /// <param name="outExpectedValueList">This is the list of values that *should* be returned when this record is retrieved from the database, for comparison purposes, not the actual values</param>
        /// <param name="parentOTMAttributeID">If this entity is the target of a one to many relationship from another, the one to many attribute of the parent entity to link it to</param>
        /// <param name="parentEntityRecordID">If this entity is the target of a one to many relationship from another, the record of the parent entity to link it to</param>
        /// <returns>The ID of the inserted record if successful</returns>
        public static int New(cCustomEntity entity, out Dictionary<int, object> outExpectedValueList, int parentOTMAttributeID = 0, int parentEntityRecordID = 0, SortedList<int, object> parameterList = null)
        {
            #region Check parameters
            if (entity == null)
            {
                throw new ArgumentNullException("Parameter 'entity' must be a valid saved GreenLight in <" + typeof(CustomEntityRecord).ToString() + ">.New");
            }
            if (entity.entityid <= 0)
            {
                throw new ArgumentOutOfRangeException("Parameter (cCustomEntity).entity must be a valid saved GreenLight ID in <" + typeof(CustomEntityRecord).ToString() + ">.New");
            }
            if (parentOTMAttributeID < 0)
            {
                throw new ArgumentOutOfRangeException("Parameter parentOTMAttributeID must be a valid saved GreenLight attribute ID in <" + typeof(CustomEntityRecord).ToString() + ">.New");
            }
            if (parentEntityRecordID < 0)
            {
                throw new ArgumentOutOfRangeException("Parameter parentEntityRecordID must be a valid saved GreenLight record ID in <" + typeof(CustomEntityRecord).ToString() + ">.New");
            }
            #endregion Check parameters

            #region Variable declarations
            Dictionary<int, object> values = new Dictionary<int, object>();
            Dictionary<string, string> query = new Dictionary<string, string>();
            Dictionary<string, string> validatedQuery = new Dictionary<string, string>();
            parameterList = (parameterList == null) ? new SortedList<int, object>() : parameterList;
            int key = -1;
            object value = null;
            string sqlValue = string.Empty;
            string columnName = string.Empty;
            string idColumnName = string.Empty;
            Type type;
            StringBuilder insertSql = new StringBuilder();
            int recordID = 0;
            #endregion Variable declarations

            foreach (cAttribute a in entity.attributes.Values)
            {
                #region Set up the lists of values and sql values
                key = a.attributeid;
                type = a.GetType();
                columnName = "att" + a.attributeid.ToString();

                if (a.displayname == "ID")
                {
                    idColumnName = columnName;
                    continue;
                }

                if (a.attributename == "CreatedBy" || a.attributename == "CreatedOn" || a.attributename == "ModifiedBy" || a.attributename == "ModifiedOn" || a.attributename == "GreenLightCurrency")
                {
                    columnName = a.attributename;
                }

                #region Set a default value by type

                if (parameterList.ContainsKey(key))
                {
                    object obj = new object();
                    parameterList.TryGetValue(key, out value);
                    if (parameterList.TryGetValue(key, out obj))
                    {
                        if (obj.GetType() == typeof(int))
                        {
                            sqlValue = obj.ToString();
                        }
                        else if (obj.GetType() == typeof(bool))
                        {
                            sqlValue = (bool)obj ? "1" : "0";
                        }
                        else if (obj.GetType() == typeof(DateTime))
                        {
                            sqlValue = "'" + ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        }
                        else
                        {
                            sqlValue = "'" + obj.ToString() + "'";
                        }
                    }
                }
                else if (type == typeof(cTextAttribute))
                {
                    value = "string";
                    sqlValue = "'string'";
                }
                else if (type == typeof(cListAttribute))
                {
                    value = 0;
                    sqlValue = "0";
                }
                else if (type == typeof(cNumberAttribute))
                {
                    value = 0;
                    sqlValue = "0";
                }
                else if (type == typeof(cCommentAttribute))
                {
                    value = null;
                }
                else if (type == typeof(cTickboxAttribute))
                {
                    value = false;
                    sqlValue = "0";
                }
                else if (type == typeof(cDateTimeAttribute))
                {
                    value = DateTime.UtcNow;
                    sqlValue = "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                }
                else if (type == typeof(cManyToOneRelationship))
                {
                    if (((cManyToOneRelationship)a).relatedtable.TableName == "employees")
                    {
                        value = GlobalTestVariables.EmployeeId;
                        sqlValue = GlobalTestVariables.EmployeeId.ToString();
                    }
                    else
                    {
                        value = null;
                        sqlValue = "";
                    }
                }
                else if (type == typeof(cOneToManyRelationship))
                {
                    value = null;
                    sqlValue = "";
                }
                else if (type == typeof(cSummaryAttribute))
                {
                    value = null;
                    sqlValue = "";
                }
                else
                {
                    value = null;
                    sqlValue = "";
                }
                #endregion Set a default value by type

                if (value != null) values.Add(key, value);
                if (value != null) query.Add(columnName, sqlValue);
                #endregion Set up the lists of values and sql values
            }

            if (parentOTMAttributeID > 0 && parentEntityRecordID > 0)
            {
                #region Add a OtM value if there is one
                values.Add(parentOTMAttributeID, parentEntityRecordID);
                query.Add("att" + parentOTMAttributeID, parentEntityRecordID.ToString());
                #endregion Add a OtM value if there is one
            }

            if (values.Count > 0)
            {
                #region Find the entity table
                DBConnection data = null;
                string tableName = string.Empty;
                string sql = string.Empty;
                try
                {
                    data = new DBConnection(cAccounts.getConnectionString(Moqs.CurrentUser().AccountID));

                    sql = "SELECT @returnVal = [TABLE_NAME] FROM INFORMATION_SCHEMA.TABLES WHERE [TABLE_NAME] = (SELECT 'custom_' + [masterTableName] FROM [customEntities] WHERE [entityid] = @masterTableNameEntityID)";
                    data.sqlexecute.Parameters.AddWithValue("@masterTableNameEntityID", entity.entityid);

                    data.sqlexecute.Parameters.Add("@returnVal", System.Data.SqlDbType.NVarChar, 1000);
                    data.sqlexecute.Parameters["@returnVal"].Direction = System.Data.ParameterDirection.Output;
                    data.ExecuteSQL(sql);

                    tableName = (string)data.sqlexecute.Parameters["@returnVal"].Value;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed trying to find the database table related to the GreenLight in <" + typeof(CustomEntityRecord).ToString() + ">.New", e);
                }
                #endregion Find the entity table

                #region Examine the columns in the custom entity's table
                if (tableName != string.Empty)
                {
                    data.sqlexecute.Parameters.Clear();
                    sql = "SELECT [COLUMN_NAME], [IS_NULLABLE], [DATA_TYPE] FROM INFORMATION_SCHEMA.COLUMNS WHERE [TABLE_NAME] = (SELECT 'custom_' + [masterTableName] FROM [customEntities] WHERE [entityid] = @masterTableNameEntityID)";
                    data.sqlexecute.Parameters.AddWithValue("@masterTableNameEntityID", entity.entityid);

                    #region Check all the columns will all be populated with necessary values
                    using (System.Data.SqlClient.SqlDataReader reader = data.GetReader(sql))
                    {
                        string tmpColName = string.Empty;
                        while (reader.Read())
                        {
                            tmpColName = reader.GetString(0);
                            if (tmpColName == idColumnName) // no need to check the id column
                            {
                                continue;
                            }
                            else if (!query.ContainsKey(tmpColName)  // value has not been created for column of that name
                                && reader.GetString(1) == "NO")      // and it's not nullable so we can't skip it
                            {
                                throw new ArgumentNullException("A value can not be created for one of the columns in the table <" + tableName + "> in <" + typeof(CustomEntityRecord).ToString() + ">.New");
                            }
                            else if (!query.ContainsKey(tmpColName)  // value has not been created for column of that name
                                && reader.GetString(1) == "YES")      // but it's nullable so we can skip it
                            {
                                continue;
                            }
                            else
                            {
                                validatedQuery.Add(tmpColName, query[tmpColName]);
                            }
                        }
                        reader.Close();

                        if (query.Count != validatedQuery.Count)
                        {
                            throw new ArgumentNullException("The query values contain more columns than exist in table <" + tableName + "> in <" + typeof(CustomEntityRecord).ToString() + ">.New");
                        }
                    }
                    #endregion Check all the columns will all be populated with necessary values

                    #region Populate the columns with necessary values
                    insertSql.Append("INSERT INTO [" + tableName + "] ([");
                    insertSql.Append(string.Join("], [", validatedQuery.Keys));
                    insertSql.Append("]) VALUES (");
                    insertSql.Append(string.Join(", ", validatedQuery.Values));
                    insertSql.Append("); SELECT @returnVal = @@IDENTITY;");
                    #endregion Populate the columns with necessary values
                }
                data.sqlexecute.Parameters.Clear();
                #endregion Examine the columns in the custom entity's table

                #region Insert values into the entity's table
                try
                {
                    data.sqlexecute.Parameters.Add("@returnVal", System.Data.SqlDbType.Int);
                    data.sqlexecute.Parameters["@returnVal"].Direction = System.Data.ParameterDirection.Output;
                    data.ExecuteSQL(insertSql.ToString());

                    recordID = (int)data.sqlexecute.Parameters["@returnVal"].Value;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed trying to insert record values for the GreenLight in <" + typeof(CustomEntityRecord).ToString() + ">.New", e);
                }
                #endregion Insert values into the entity's table
            }

            outExpectedValueList = values;
            return recordID;
        }
    }
}
