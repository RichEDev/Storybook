using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;
using Auto_Tests.Tools;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using System.Linq;
using System.Text;
using Auto_Tests.UIMaps.CustomEntityFormsClientUIMapClasses;
using Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.User_Defined_Fields;


namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Forms_Client
{
    /// <summary>
    /// Summary description for CustomEntityFormsClientUITest
    /// </summary>
    [CodedUITest]
    public class CustomEntityFormsClientUITest
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private readonly static ProductType _executingProduct = cGlobalVariables.GetProductFromAppConfig();
        /// <summary>
        /// Cached list of Greenlights
        /// </summary>
        internal static List<CustomEntity> _customEntities;
        /// <summary>
        /// Cached list of join vias
        /// </summary>
        internal static List<CustomEntitiesUtilities.JoinVia> JoinVias;
        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();
        /// <summary>
        /// Custom Entity Forms Client UI Mape
        /// </summary>
        private CustomEntityFormsClientUIMap _customEntityFormsClientUIMap = new CustomEntityFormsClientUIMap();
        #region Additional test attributes

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            _sharedMethods.StartIE(_executingProduct);
            _sharedMethods.Logon(_executingProduct, LogonType.administrator);
            CachePopulatorForFormsClient CustomEntityDataFromLithium = new CachePopulatorForFormsClient(_executingProduct);
            JoinVias = CustomEntityDataFromLithium.PopulateJoinVias();
            _customEntities = CustomEntityDataFromLithium.PopulateCache();

            Assert.IsNotNull(_customEntities);
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            _sharedMethods.CloseBrowserWindow();
        }
        //You can use the following additional attributes as you write your tests:

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {

        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Assert.IsNotNull(_customEntities);
            //for (int entityindexer = 0; entityindexer < _customEntities.Count; entityindexer++)
            //{
            //    int result = CustomEntityDatabaseAdapter.DeleteCustomEntity(_customEntities[entityindexer].entityId, _executingProduct);
            //    Assert.AreEqual(0, result);
            //}
        }
        #endregion

        #region CustomEntityFormsClientSuccessfullyCreateCustomEntityClientRecord
        [TestMethod]
        public void CustomEntityFormsClientSuccessfullyCreateCustomEntityClientRecord_UITestNew()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to form clientside
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/aeentity.aspx?viewid=" + _customEntities[0].view[0]._viewid + "&entityid=" + _customEntities[0].entityId + "&formid=" + _customEntities[0].form[0]._formid + "&tabid=" + _customEntities[0].form[0].tabs[0]._tabid);
            #endregion

            #region populate all fields in clientside
            CustomEntityFormsClientControls clientFormControls = new CustomEntityFormsClientControls(_customEntityFormsClientUIMap);
            clientFormControls.standardSingleLineTxt.Text = _customEntities[0].attribute[0].attributeData.clientData1;
            string mText = _customEntities[0].attribute[1].attributeData.clientData1.Substring(0, 39);
            clientFormControls.multilineTxt.Text = mText;
            clientFormControls.numberTxt.Text = _customEntities[0].attribute[2].attributeData.clientData1;
            clientFormControls.decimalTxt.Text = _customEntities[0].attribute[3].attributeData.clientData1;
            clientFormControls.currencyTxt.Text = _customEntities[0].attribute[4].attributeData.clientData1;
            string dateTimeDateTxt = ConvertDBDateTimeToUIDate(_customEntities[0].attribute[6].attributeData.clientData1);
            string dateTimeTimeTxt = ConvertDBDateTimeToUITime(_customEntities[0].attribute[7].attributeData.clientData1);
            clientFormControls.dateTimeDateTxt.Text = ConvertDBDateTimeToUIDate(_customEntities[0].attribute[5].attributeData.clientData1);
            clientFormControls.dateTimeTimeTxt.Text = ConvertDBDateTimeToUITime(_customEntities[0].attribute[5].attributeData.clientData1);
            clientFormControls.dateTxt.Text = dateTimeDateTxt;
            clientFormControls.timeTxt.Text = dateTimeTimeTxt;
            string mLargetext = _customEntities[0].attribute[8].attributeData.clientData1.Substring(0, 45);
            clientFormControls.multilineLargeTxt.Text = mLargetext;
            clientFormControls.yesCmb.SelectedItem = _customEntities[0].attribute[12].attributeData.clientData1;
            clientFormControls.noCmb.SelectedItem = _customEntities[0].attribute[13].attributeData.clientData1;
            clientFormControls.wideSingleLineTxt.Text = _customEntities[0].attribute[14].attributeData.clientData1;
            clientFormControls.standardListCmb.SelectedItem = _customEntities[0].attribute[15].attributeData.clientData1;
            clientFormControls.wideListCmb.SelectedItem = _customEntities[0].attribute[16].attributeData.clientData1;
            _customEntityFormsClientUIMap.PressSaveButton();
            #endregion

            #region edit new record and verify data saved
            _customEntityFormsClientUIMap.ClickEditFieldLink(_customEntities[0].attribute[0].attributeData.clientData1);
            clientFormControls = new CustomEntityFormsClientControls(_customEntityFormsClientUIMap);
            Assert.AreEqual(_customEntities[0].attribute[0].attributeData.clientData1, clientFormControls.standardSingleLineTxt.Text);
            Assert.AreEqual(mText, clientFormControls.multilineTxt.Text);
            Assert.AreEqual(_customEntities[0].attribute[2].attributeData.clientData1, clientFormControls.numberTxt.Text);
            Assert.AreEqual(_customEntities[0].attribute[3].attributeData.clientData1, clientFormControls.decimalTxt.Text);
            Assert.AreEqual(_customEntities[0].attribute[4].attributeData.clientData1, clientFormControls.currencyTxt.Text);
            Assert.AreEqual(dateTimeDateTxt, clientFormControls.dateTimeDateTxt.Text);
            Assert.AreEqual(dateTimeTimeTxt, clientFormControls.dateTimeDateTxt.Text);
            Assert.AreEqual(_customEntities[0].attribute[6].attributeData.clientData1, clientFormControls.dateTxt.Text);
            Assert.AreEqual(_customEntities[0].attribute[7].attributeData.clientData1, clientFormControls.timeTxt.Text);
            Assert.AreEqual(mLargetext, clientFormControls.multilineLargeTxt.Text);
            Assert.AreEqual(_customEntities[0].attribute[12].attributeData.clientData1, clientFormControls.yesCmb.SelectedItem);
            Assert.AreEqual(_customEntities[0].attribute[13].attributeData.clientData1, clientFormControls.noCmb.SelectedItem);
            Assert.AreEqual(_customEntities[0].attribute[14].attributeData.clientData1, clientFormControls.wideSingleLineTxt.Text);
            Assert.AreEqual(_customEntities[0].attribute[15].attributeData.clientData1, clientFormControls.standardListCmb.SelectedItem);
            Assert.AreEqual(_customEntities[0].attribute[16].attributeData.clientData1, clientFormControls.wideListCmb.SelectedItem);

            #endregion
        }
        #endregion

        public class CachePopulatorForFormsClient : CachePopulator
        {
            public CachePopulatorForFormsClient(ProductType executingProduct) : base(executingProduct) { }

            #region PopulateViews
            public override void PopulateViews(ref CustomEntity entity)
            {
                entity.view = new List<CustomEntitiesUtilities.CustomEntityView>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSqlStringForViews(entity.entityId)))
                {
                    #region Set Database Columns
                    int viewidOrdinal = reader.GetOrdinal("viewid");
                    int viewNameOrdinal = reader.GetOrdinal("view_name");
                    int descriptionOrdinal = reader.GetOrdinal("description");
                    int createdByOrdinal = reader.GetOrdinal("createdby");
                    int modifiedByOrdinal = reader.GetOrdinal("modifiedby");
                    int menuidOrdinal = reader.GetOrdinal("menuid");
                    int menuDescriptionOrdinal = reader.GetOrdinal("MenuDescription");
                    int allowAddOrdinal = reader.GetOrdinal("allowadd");
                    int addFormOrdinal = reader.GetOrdinal("add_formid");
                    int allowEditOrdinal = reader.GetOrdinal("allowedit");
                    int editFormOrdinal = reader.GetOrdinal("edit_formid");
                    int allowDeleteOrdinal = reader.GetOrdinal("allowdelete");
                    int allowApprovalOrdinal = reader.GetOrdinal("allowapproval");
                    int sortColumnOrdinal = reader.GetOrdinal("SortColumn");
                    int sortOrderOrdinal = reader.GetOrdinal("SortOrder");
                    int sortColumnJoinViaIDOrdinal = reader.GetOrdinal("SortColumnJoinViaID");
                    int menuIconOrdinal = reader.GetOrdinal("MenuIcon");
                    #endregion

                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityView view = new CustomEntitiesUtilities.CustomEntityView();
                        #region Set values
                        view._viewid = reader.GetInt32(viewidOrdinal);
                        view._viewName = reader.GetString(viewNameOrdinal);
                        view._description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal);
                        //view._modifiedBy = reader.IsDBNull(modifiedByOrdinal) ? null : reader.GetString(modifiedByOrdinal);
                        view._createdBy = AutoTools.GetEmployeeIDByUsername(_executingProduct).ToString();
                        view._menudescription = reader.IsDBNull(menuDescriptionOrdinal) ? null : reader.GetString(menuDescriptionOrdinal);
                        if (!reader.IsDBNull(menuidOrdinal))
                        {
                            view._menuid = reader.GetInt32(menuidOrdinal);
                        }

                        view._allowAdd = reader.GetBoolean(allowAddOrdinal);
                        view._allowEdit = reader.GetBoolean(allowEditOrdinal);
                        view._allowDelete = reader.GetBoolean(allowDeleteOrdinal);
                        view._allowApproval = reader.GetBoolean(allowApprovalOrdinal);
                        view._createdOn = DateTime.Now;

                        if (!reader.IsDBNull(addFormOrdinal))
                        {
                            view.AddFormId = reader.GetInt32(addFormOrdinal);
                            view.addform = PopulateViewsFormDropdown(entity.form, view.AddFormId);
                        }
                        if (!reader.IsDBNull(editFormOrdinal))
                        {
                            view.EditFormId = reader.GetInt32(editFormOrdinal);
                            view.editform = PopulateViewsFormDropdown(entity.form, view.EditFormId);
                        }
                        if (!reader.IsDBNull(sortColumnJoinViaIDOrdinal))
                        {
                            //view.sortColumn_joinViaID = reader.GetInt32(sortColumnJoinViaIDOrdinal);
                        }
                        view.sortColumn = new CustomEntitiesUtilities.GreenLightSortColumn();

                        view.sortColumn._fieldID = reader.GetGuid(sortColumnOrdinal);
                        view.sortColumn._sortDirection = reader.GetByte(sortOrderOrdinal);
                        view._menuIcon = reader.IsDBNull(menuIconOrdinal) ? string.Empty : reader.GetString(menuIconOrdinal);
                        PopulateViewFields(ref view);
                        PopulateViewFilters(ref view);
                        view._viewid = 0;
                        entity.view.Add(view);
                        #endregion
                    }
                    reader.Close();
                }
            }

            private CustomEntitiesUtilities.CustomEntityForm PopulateViewsFormDropdown(List<CustomEntitiesUtilities.CustomEntityForm> forms, int formIdForView)
            {
                CustomEntitiesUtilities.CustomEntityForm form = null;
                if (forms != null)
                {
                    foreach (CustomEntitiesUtilities.CustomEntityForm formIterator in forms)
                    {
                        if (formIterator._formid == formIdForView)
                        {
                            form = formIterator;
                        }
                    }
                }
                return form;
            }
            #endregion

            #region PopulateViewFields
            public override void PopulateViewFields(ref CustomEntitiesUtilities.CustomEntityView view)
            {
                view.fields = new List<CustomEntitiesUtilities.CustomEntityViewField>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSqlStringViewFields(view._viewid)))
                {
                    #region Set Database Columns
                    int viewFieldIdOrdinal = reader.GetOrdinal("viewFieldId");
                    int viewIdOrdinal = reader.GetOrdinal("viewid");
                    int fieldIdOrdinal = reader.GetOrdinal("fieldid");
                    int orderOrdinal = reader.GetOrdinal("order");
                    int joinViaIDOrdinal = reader.GetOrdinal("joinViaID");
                    #endregion

                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityViewField viewField = new CustomEntitiesUtilities.CustomEntityViewField();
                        #region Set values
                        viewField._viewFieldId = reader.GetInt32(viewFieldIdOrdinal);
                        viewField._viewId = view._viewid;
                        viewField._fieldid = reader.GetGuid(fieldIdOrdinal);
                        viewField._order = reader.GetByte(orderOrdinal);
                        viewField._joinViaId = reader.IsDBNull(joinViaIDOrdinal) ? null : (int?)reader.GetInt32(joinViaIDOrdinal);
                        view.fields.Add(viewField);
                        #endregion
                    }
                }
            }

            #endregion

            #region PopulateViewFilters
            public override void PopulateViewFilters(ref CustomEntitiesUtilities.CustomEntityView view)
            {
                view.filters = new List<CustomEntitiesUtilities.CustomEntityViewFilter>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSqlStringViewFilters(view._viewid)))
                {
                    #region Set Database Columns
                    int viewIdOrdinal = reader.GetOrdinal("viewid");
                    int fieldIdOrdinal = reader.GetOrdinal("fieldid");
                    int conditionOrdinal = reader.GetOrdinal("condition");
                    int valueOrdinal = reader.GetOrdinal("value");
                    int orderOrdinal = reader.GetOrdinal("order");
                    int joinViaIDOrdinal = reader.GetOrdinal("joinViaID");
                    int value2Ordinal = reader.GetOrdinal("valueTwo");
                    int attributeIdOrdinal = reader.GetOrdinal("attributeid");
                    #endregion

                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityViewFilter viewFilter = new CustomEntitiesUtilities.CustomEntityViewFilter();
                        #region Set values
                        viewFilter._viewId = reader.IsDBNull(viewIdOrdinal) ? (int?)null : reader.GetInt32(viewIdOrdinal);
                        viewFilter._fieldid = reader.GetGuid(fieldIdOrdinal);
                        viewFilter._conditionType = reader.GetByte(conditionOrdinal);
                        viewFilter._valueOne = reader.GetString(valueOrdinal);
                        viewFilter._order = reader.GetByte(orderOrdinal);
                        viewFilter._joinViaId = reader.IsDBNull(joinViaIDOrdinal) ? null : (int?)reader.GetInt32(joinViaIDOrdinal);
                        viewFilter._valueTwo = reader.GetString(value2Ordinal);
                        viewFilter.AttributeId = reader.IsDBNull(attributeIdOrdinal) ? (int?)null : reader.GetInt32(attributeIdOrdinal);
                        view.filters.Add(viewFilter);
                        #endregion
                    }
                }
            }
            #endregion

            #region PopulateAttributeFilters
            public override void PopulateAttributeFilters(ref CustomEntitiesUtilities.CustomEntityNtoOneAttribute attribute, int attributeId)
            {
                attribute.Filters = new List<CustomEntitiesUtilities.CustomEntityViewFilter>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSqlStringAttributeFilters(attributeId)))
                {
                    #region Set Database Columns
                    int viewIdOrdinal = reader.GetOrdinal("viewid");
                    int fieldIdOrdinal = reader.GetOrdinal("fieldid");
                    int conditionOrdinal = reader.GetOrdinal("condition");
                    int valueOrdinal = reader.GetOrdinal("value");
                    int orderOrdinal = reader.GetOrdinal("order");
                    int joinViaIDOrdinal = reader.GetOrdinal("joinViaID");
                    int value2Ordinal = reader.GetOrdinal("valueTwo");
                    int attributeIdOrdinal = reader.GetOrdinal("attributeid");
                    #endregion

                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityViewFilter attributeFilter = new CustomEntitiesUtilities.CustomEntityViewFilter();
                        #region Set values
                        attributeFilter._viewId = reader.IsDBNull(viewIdOrdinal) ? (int?)null : reader.GetInt32(viewIdOrdinal);
                        attributeFilter._fieldid = reader.GetGuid(fieldIdOrdinal);
                        attributeFilter._conditionType = reader.GetByte(conditionOrdinal);
                        attributeFilter._valueOne = reader.GetString(valueOrdinal);
                        attributeFilter._order = reader.GetByte(orderOrdinal);
                        attributeFilter._joinViaId = reader.IsDBNull(joinViaIDOrdinal) ? null : (int?)reader.GetInt32(joinViaIDOrdinal);
                        attributeFilter._valueTwo = reader.GetString(value2Ordinal);
                        attributeFilter.AttributeId = reader.IsDBNull(attributeIdOrdinal) ? (int?)null : reader.GetInt32(attributeIdOrdinal);
                        attribute.Filters.Add(attributeFilter);
                        #endregion
                    }
                }
            }
            #endregion

            #region PopulateAttributes
            public override void PopulateAttributes(ref CustomEntity entity)
            {
                entity.attribute = new List<CustomEntitiesUtilities.CustomEntityAttribute>();

                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSqlstringForAttributes(entity.entityId)))
                {
                    #region Set Database Columns
                    int createdByOrdinal = reader.GetOrdinal("createdby");
                    int modifiedByOrdinal = reader.GetOrdinal("modifiedby");
                    int displayNameOrdinal = reader.GetOrdinal("display_name");
                    int descriptionOrdinal = reader.GetOrdinal("description");
                    int tooltipOrdinal = reader.GetOrdinal("tooltip");
                    int mandatoryOrdinal = reader.GetOrdinal("mandatory");
                    int fieldTypeOrdinal = reader.GetOrdinal("fieldtype");
                    int createdOnOrdinal = reader.GetOrdinal("createdon");
                    int maxLengthOrdinal = reader.GetOrdinal("maxlength");
                    int formatOrdinal = reader.GetOrdinal("format");
                    int defaultValueOrdinal = reader.GetOrdinal("defaultvalue");
                    int precisionOrdinal = reader.GetOrdinal("precision");
                    int relationshipTypeOrdinal = reader.GetOrdinal("relationshiptype");
                    int relatedEntityOrdinal = reader.GetOrdinal("related_entity");
                    int viewdIdOrdinal = reader.GetOrdinal("viewid");
                    int isAuditIdentityOrdinal = reader.GetOrdinal("is_audit_identity");
                    int advicePanelTextOrdinal = reader.GetOrdinal("advicePanelText");
                    int isUniqueOrdinal = reader.GetOrdinal("is_unique");
                    int attributeIDOrdinal = reader.GetOrdinal("attributeid");
                    int relatedTableOrdinal = reader.GetOrdinal("relatedtable");
                    int relationshipDisplayFieldOrdinal = reader.GetOrdinal("relationshipdisplayfield");
                    int maxRows = reader.GetOrdinal("maxRows");
                    int triggerAttributeIdOrdinal = reader.GetOrdinal("TriggerAttributeId");
                    int triggerJoinViaIdOrdinal = reader.GetOrdinal("TriggerJoinViaId");
                    int triggerDisplayFieldIdOrdinal = reader.GetOrdinal("TriggerDisplayFieldId");
                    int systeAttributeOrdinal = reader.GetOrdinal("System_attribute");
                    int boolAttributeOrdinal = reader.GetOrdinal("boolAttribute");
                    int fieldIdOrdinal = reader.GetOrdinal("fieldid");
                    //int record1Ordinal = reader.GetOrdinal("record1");
                    //int record2Ordinal = reader.GetOrdinal("record2");
                    #endregion

                    while (reader.Read())
                    {
                        #region Set values
                        CustomEntitiesUtilities.CustomEntityAttribute attribute = null;
                        RelationshipType relType = RelationshipType.None;
                        FieldType type = (FieldType)reader.GetByte(fieldTypeOrdinal);
                        if (type == FieldType.Relationship)
                        {
                            relType = (RelationshipType)reader.GetByte(relationshipTypeOrdinal);
                        }
                        int attributeID = reader.GetInt32(attributeIDOrdinal);

                        if (type == FieldType.List)
                        {
                            CustomEntitiesUtilities.cCustomEntityListAttribute listAttribute = new CustomEntitiesUtilities.cCustomEntityListAttribute();
                            CachePopulator.PopulateListItems(ref listAttribute, attributeID);
                            attribute = listAttribute;
                        }
                        else if (type != FieldType.Relationship && type != FieldType.List)
                        {
                            CustomEntitiesUtilities.CustomEntityAttribute sattribute = new CustomEntitiesUtilities.CustomEntityAttribute();
                            attribute = sattribute;
                        }
                        else if (type == FieldType.Relationship && relType == RelationshipType.ManyToOne)
                        {
                            string test = reader.GetString(displayNameOrdinal);
                            CustomEntitiesUtilities.CustomEntityNtoOneAttribute newNToOneAttribute = new CustomEntitiesUtilities.CustomEntityNtoOneAttribute();
                            if (!reader.IsDBNull(relatedTableOrdinal))
                            {
                                newNToOneAttribute._relatedTable = reader.GetGuid(relatedTableOrdinal);
                            }
                            if (!reader.IsDBNull(relationshipDisplayFieldOrdinal))
                            {
                                newNToOneAttribute._relationshipdisplayfield = reader.GetGuid(relationshipDisplayFieldOrdinal);
                            }
                            if (!reader.IsDBNull(relationshipTypeOrdinal))
                            {
                                newNToOneAttribute._relationshipType = reader.GetByte(relationshipTypeOrdinal);
                            }
                            newNToOneAttribute._maxRows = reader.IsDBNull(maxRows) ? (int?)null : reader.GetInt32(maxRows);
                            GetRelationshipMatchFields(ref newNToOneAttribute, attributeID);
                            PopulateAttributeFilters(ref newNToOneAttribute, attributeID);
                            attribute = newNToOneAttribute;
                        }
                        else if (type == FieldType.Relationship && relType == RelationshipType.OneToMany)
                        {
                            CustomEntitiesUtilities.CustomEntityOnetoNAttribute newOneToNAttribute = new CustomEntitiesUtilities.CustomEntityOnetoNAttribute();
                            newOneToNAttribute._relationshipType = reader.GetByte(relationshipTypeOrdinal); ;
                            newOneToNAttribute._relatedTable = reader.GetGuid(relatedTableOrdinal);
                            newOneToNAttribute._viewId = reader.GetInt32(viewdIdOrdinal);
                            newOneToNAttribute._relatedEntityId = reader.GetInt32(relatedEntityOrdinal);
                            attribute = newOneToNAttribute;
                        }

                        attribute._attributeid = reader.GetInt32(attributeIDOrdinal);
                        attribute._createdBy = AutoTools.GetEmployeeIDByUsername(ExecutingProduct);
                        //attribute._modifiedBy = reader.IsDBNull(modifiedByOrdinal) ? null : reader.GetString(modifiedByOrdinal);
                        attribute.DisplayName = reader.GetString(displayNameOrdinal);
                        attribute._description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal);
                        attribute._tooltip = reader.IsDBNull(tooltipOrdinal) ? null : reader.GetString(tooltipOrdinal);
                        attribute._mandatory = reader.GetBoolean(mandatoryOrdinal);
                        attribute._fieldType = type;
                        attribute._date = reader.GetDateTime(createdOnOrdinal);
                        attribute._maxLength = reader.IsDBNull(maxLengthOrdinal) ? (int?)null : reader.GetInt32(maxLengthOrdinal);
                        attribute._format = reader.IsDBNull(formatOrdinal) ? (short)Format.None : reader.GetByte(formatOrdinal);
                        attribute._defaultValue = reader.IsDBNull(defaultValueOrdinal) ? null : reader.GetString(defaultValueOrdinal);
                        attribute._precision = reader.IsDBNull(precisionOrdinal) ? (short?)null : reader.GetByte(precisionOrdinal);
                        attribute._isAuditIdenity = reader.GetBoolean(isAuditIdentityOrdinal);
                        attribute._commentText = reader.IsDBNull(advicePanelTextOrdinal) ? null : reader.GetString(advicePanelTextOrdinal);
                        attribute._isUnique = reader.GetBoolean(isUniqueOrdinal);
                        attribute.SystemAttribute = reader.GetBoolean(systeAttributeOrdinal);
                        attribute.TriggerAttributeId = reader.IsDBNull(triggerAttributeIdOrdinal) ? (int?)null : reader.GetInt32(triggerAttributeIdOrdinal);
                        attribute.TriggerJoinViaId = reader.IsDBNull(triggerJoinViaIdOrdinal) ? (int?)null : reader.GetInt32(triggerJoinViaIdOrdinal);
                        attribute.TriggerDisplayFieldID = reader.IsDBNull(triggerDisplayFieldIdOrdinal) ? (Guid?)null : reader.GetGuid(triggerDisplayFieldIdOrdinal);
                        attribute.EnableImageLibrary = reader.GetBoolean(boolAttributeOrdinal);
                        attribute.FieldId = reader.GetGuid(fieldIdOrdinal);
                        CustomEntitiesUtilities.AttributeData attributeData = new CustomEntitiesUtilities.AttributeData();
                        attributeData.clientData1 = null; /*reader.IsDBNull(record1Ordinal) ? null : reader.GetString(record1Ordinal);*/
                        attribute.attributeData = attributeData;
                        entity.attribute.Add(attribute);
                        #endregion
                    }
                    reader.Close();
                }
                db.sqlexecute.Parameters.Clear();
            }

            //private void PopulateListItems(ref CustomEntitiesUtilities.cCustomEntityListAttribute attribute, int customEntityId)
            //{
            //    cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            //    string strSQL = "SELECT [attributeid] ,[item], [order] FROM customEntityAttributeListItems WHERE attributeid = @attributeid";
            //    db.sqlexecute.Parameters.AddWithValue("@attributeid", customEntityId);

            //    List<CustomEntitiesUtilities.EntityListItem> customEntityListItems = new List<CustomEntitiesUtilities.EntityListItem>();
            //    using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            //    {
            //        int attributeIdOrdinal = reader.GetOrdinal("attributeid");
            //        int itemOrdinal = reader.GetOrdinal("item");
            //        int orderOrdinal = reader.GetOrdinal("order");

            //        while (reader.Read())
            //        {
            //            int attributeId = reader.GetInt32(attributeIdOrdinal);
            //            string text = reader.IsDBNull(itemOrdinal) ? null : reader.GetString(itemOrdinal);
            //            int order = reader.GetInt32(orderOrdinal);
            //            customEntityListItems.Add(new CustomEntitiesUtilities.EntityListItem(attributeId, text, order));
            //        }
            //    }
            //    attribute._listItems = customEntityListItems;
            //}

            private void GetRelationshipMatchFields(ref CustomEntitiesUtilities.CustomEntityNtoOneAttribute attribute, int attributeid)
            {
                cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

                string strSQL = "SELECT [matchFieldId] ,[attributeId], [fieldId] FROM customEntityAttributeMatchFields WHERE attributeid = @attributeid";
                db.sqlexecute.Parameters.AddWithValue("@attributeid", attributeid);

                List<CustomEntitiesUtilities.RelationshipMatchFieldListItem> relationshipMatchFieldListItem = new List<CustomEntitiesUtilities.RelationshipMatchFieldListItem>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
                {
                    int matchfieldidOrdinal = reader.GetOrdinal("matchFieldId");
                    int attributeIdOrdinal = reader.GetOrdinal("attributeId");
                    int fieldidOrdinal = reader.GetOrdinal("fieldId");

                    while (reader.Read())
                    {
                        int attributeId = reader.GetInt32(attributeIdOrdinal);
                        int matchfieldid = reader.GetInt32(matchfieldidOrdinal);
                        Guid fieldid = reader.GetGuid(fieldidOrdinal);
                        relationshipMatchFieldListItem.Add(new CustomEntitiesUtilities.RelationshipMatchFieldListItem(attributeId, fieldid, matchfieldid));
                    }
                }
                attribute._matchFieldListItems = relationshipMatchFieldListItem;
            }
            #endregion

            #region PopulateForms
            public override void PopulateForms(ref CustomEntity entity)
            {
                entity.form = new List<CustomEntitiesUtilities.CustomEntityForm>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSqlStringForForms(entity.entityId)))
                {
                    #region Set Database Columns
                    int formidOrdinal = reader.GetOrdinal("formid");
                    int formNameOrdinal = reader.GetOrdinal("form_name");
                    int descriptionOrdinal = reader.GetOrdinal("description");
                    int createdOnOrdinal = reader.GetOrdinal("createdon");
                    int createdByOrdinal = reader.GetOrdinal("createdby");
                    int modifiedByOrdinal = reader.GetOrdinal("modifiedby");
                    int showBreadCrumbsOrdinal = reader.GetOrdinal("showBreadCrumbs");
                    int showSaveAndNewOrdinal = reader.GetOrdinal("showSaveAndNew");
                    int showSubMenusOrdinal = reader.GetOrdinal("showSubMenus");
                    int saveAndNewButtonTextOrdinal = reader.GetOrdinal("SaveAndNewButtonText");
                    int saveButtonTextOrdinal = reader.GetOrdinal("SaveButtonText");
                    int cancelButtonTextOrdinal = reader.GetOrdinal("CancelButtonText");
                    int showSaveOrdinal = reader.GetOrdinal("showSave");
                    int showCancelOrdinal = reader.GetOrdinal("showCancel");
                    int saveAndStayButtonTextOrdinal = reader.GetOrdinal("SaveAndStayButtonText");
                    int showSaveAndStayOrdinal = reader.GetOrdinal("showSaveAndStay");
                    #endregion

                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityForm form = new CustomEntitiesUtilities.CustomEntityForm();
                        #region Set values
                        form._formid = reader.GetInt32(formidOrdinal);
                        form._createdBy = AutoTools.GetEmployeeIDByUsername(_executingProduct).ToString();
                        form._formName = reader.GetString(formNameOrdinal);
                        form._description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal);
                        form._date = reader.GetDateTime(createdOnOrdinal);
                        //form._modifiedBy = reader.IsDBNull(modifiedByOrdinal) ? null : reader.GetString(modifiedByOrdinal);
                        form._showBreadcrumbs = reader.GetBoolean(showBreadCrumbsOrdinal);
                        form._showSaveAndNew = reader.GetBoolean(showSaveAndNewOrdinal);
                        form._showSubMenus = reader.GetBoolean(showSubMenusOrdinal);
                        form._saveAndNewButtonText = reader.IsDBNull(saveAndNewButtonTextOrdinal) ? null : reader.GetString(saveAndNewButtonTextOrdinal);
                        form._saveButtonText = reader.IsDBNull(saveButtonTextOrdinal) ? null : reader.GetString(saveButtonTextOrdinal);
                        form._cancelButtonText = reader.IsDBNull(cancelButtonTextOrdinal) ? null : reader.GetString(cancelButtonTextOrdinal);
                        form._showSave = reader.GetBoolean(showSaveOrdinal);
                        form._showCancel = reader.GetBoolean(showCancelOrdinal);
                        form._saveAndStayButtonText = reader.IsDBNull(saveAndStayButtonTextOrdinal) ? null : reader.GetString(saveAndStayButtonTextOrdinal);
                        form._showSaveAndStay = reader.GetBoolean(showSaveAndStayOrdinal);
                        PopulateTabs(ref form);
                        entity.form.Add(form);
                        #endregion
                    }
                    reader.Close();
                }
                db.sqlexecute.Parameters.Clear();
            }
            #endregion

            #region PopulateTabs
            public void PopulateTabs(ref CustomEntitiesUtilities.CustomEntityForm form)
            {
                form.tabs = new List<CustomEntitiesUtilities.CustomEntityFormTab>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSqlStringForTabs(form._formid)))
                {
                    #region Set Database Columns
                    int tabidOrdinal = reader.GetOrdinal("tabid");
                    int formidOrdinal = reader.GetOrdinal("formid");
                    int headercaptionOrdinal = reader.GetOrdinal("header_caption");
                    int orderOrdinal = reader.GetOrdinal("order");
                    #endregion
                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityFormTab tab = new CustomEntitiesUtilities.CustomEntityFormTab();
                        #region Set values
                        tab._tabid = reader.GetInt32(tabidOrdinal);
                        tab._headercaption = reader.GetString(headercaptionOrdinal);
                        tab._order = reader.GetByte(orderOrdinal);
                        tab._formid = reader.GetInt32(formidOrdinal);
                        #endregion
                        PopulateSections(ref form, ref tab);
                        tab._tabid = 0;
                        form.tabs.Add(tab);
                    }
                }
                db.sqlexecute.Parameters.Clear();
            }
            #endregion

            #region PopulateSections
            public void PopulateSections(ref CustomEntitiesUtilities.CustomEntityForm form, ref CustomEntitiesUtilities.CustomEntityFormTab tab)
            {
                tab.sections = new List<CustomEntitiesUtilities.CustomEntityFormSection>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSqlStringForSections(tab._tabid)))
                {
                    #region Set Database Columns
                    int tabidOrdinal = reader.GetOrdinal("tabid");
                    int formidOrdinal = reader.GetOrdinal("formid");
                    int sectionidOrdinal = reader.GetOrdinal("sectionid");
                    int headercaptionOrdinal = reader.GetOrdinal("header_caption");
                    int orderOrdinal = reader.GetOrdinal("order");
                    #endregion
                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityFormSection section = new CustomEntitiesUtilities.CustomEntityFormSection();
                        #region Set values
                        section._formid = form._formid;
                        section._tabid = tab._tabid;
                        section._sectionid = reader.GetInt32(sectionidOrdinal);
                        section._headercaption = reader.GetString(headercaptionOrdinal);
                        section._order = reader.GetByte(orderOrdinal);
                        #endregion
                        populateFormFields(ref form, ref section);
                        section._sectionid = 0;
                        tab.sections.Add(section);
                    }
                }
                db.sqlexecute.Parameters.Clear();
            }
            #endregion

            #region PopulateFormFields
            public void populateFormFields(ref CustomEntitiesUtilities.CustomEntityForm form, ref CustomEntitiesUtilities.CustomEntityFormSection section)
            {
                section.fields = new List<CustomEntitiesUtilities.CustomEntityFormField>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(GetSqlStringForFields(section._sectionid)))
                {
                    #region Set Database Columns
                    int formidOrdinal = reader.GetOrdinal("formid");
                    int attributeidOrdinal = reader.GetOrdinal("attributeid");
                    int readonlyOrdinal = reader.GetOrdinal("readonly");
                    int sectionidOrdinal = reader.GetOrdinal("sectionid");
                    int rowOrdinal = reader.GetOrdinal("row");
                    int columnOrdinal = reader.GetOrdinal("column");
                    int labeltextOrdinal = reader.GetOrdinal("labelText");
                    #endregion
                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityFormField formfield = new CustomEntitiesUtilities.CustomEntityFormField();
                        #region Set values
                        formfield._formid = form._formid;
                        formfield._labelText = reader.IsDBNull(labeltextOrdinal) ? null : reader.GetString(labeltextOrdinal);
                        formfield._readOnly = reader.GetBoolean(readonlyOrdinal);
                        formfield._column = reader.GetByte(columnOrdinal);
                        formfield._row = reader.GetByte(rowOrdinal);
                        formfield._sectionid = section._sectionid;
                        formfield.AttributeId = reader.GetInt32(attributeidOrdinal);
                        #endregion
                        section.fields.Add(formfield);
                    }
                }
            }
            #endregion
        }

        private void ImportDataToTestingDatabase(string test)
        {  
            var oneToNAttribute = new List<CustomEntitiesUtilities.CustomEntityOnetoNAttribute>();
            var manyToOneAttributes = new List<CustomEntitiesUtilities.CustomEntityNtoOneAttribute>();

            switch (test)
            {
                case "CustomEntityFormsClientSuccessfullyCreateCustomEntityClientRecord_UITestNew":

                    var listOfAttribute = new List<CustomEntitiesUtilities.CustomEntityAttribute>();
                    var listOfSystemAttributes = new List<CustomEntitiesUtilities.CustomEntityAttribute>();

                    #region create all entities / update all attributes with parent entity / update all joinviaparts with table type join with new entity table id
                    foreach (var entity in _customEntities.Where(x => !string.IsNullOrEmpty(x.description)))
                    {
                        int result = CustomEntityDatabaseAdapter.CreateCustomEntity(entity, _executingProduct);
                        Assert.IsTrue(result > 0);
                        foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in entity.attribute)
                        {
                            attribute.ParentEntity = new CustomEntity();
                            attribute.ParentEntity = entity;
                        }
                    }

                    foreach (var joinVia in JoinVias)
                    {
                        foreach (var value in joinVia.JoinViaParts.Values)
                        {
                            if (value._type == CustomEntitiesUtilities.JoinViaPart.IDType.Table)
                            {
                                var relatedEntityID = (from ent in _customEntities where ent.OldTableId == value.RelatedID select ent).FirstOrDefault();

                                value.RelatedID = relatedEntityID != null ? relatedEntityID.tableId : value.RelatedID;
                            }
                        }
                    }
                    #endregion

                    #region create all standard attributes / add all attributes to a list / update all system attributes with new field id
                    foreach (var entity in _customEntities)
                    {
                        var attributes = from att in entity.attribute where !att.SystemAttribute && att._fieldType != FieldType.Relationship && att._fieldType != FieldType.LookupDisplayField && att.GetType() != typeof(UserDefinedFields) && att.GetType() != typeof(UserDefinedFieldTypeList) select att;

                        foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in attributes)
                        {
                            int result = CustomEntitiesUtilities.CreateCustomEntityAttribute(entity, attribute, _executingProduct);
                            Assert.IsTrue(result > 0);
                            listOfAttribute.Add(attribute);
                        }

                        var systemAttributes = from att in entity.attribute where att.SystemAttribute select att;

                        foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in systemAttributes)
                        {
                            attribute.OldFieldId = attribute.FieldId;
                            attribute.FieldId = CustomEntitiesUtilities.GetFieldIDbyFieldNameandTableID(entity.tableId, attribute.DisplayName, _executingProduct);
                            listOfAttribute.Add(attribute);
                        }
                    }
                    #endregion

                    #region create many to one /lookup display field / many to one filters
                    foreach (var entity in _customEntities)
                    {
                        var manytooneattributes = from att in entity.attribute where !att.SystemAttribute && att._fieldType == FieldType.Relationship && att.GetType() == typeof(CustomEntitiesUtilities.CustomEntityNtoOneAttribute) select att;
                        foreach (CustomEntitiesUtilities.CustomEntityAttribute att in manytooneattributes)
                        {
                            var relatedEntity = _customEntities.Where(enti => enti.OldTableId == ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)att)._relatedTable).FirstOrDefault();
                            if (relatedEntity == null)
                            {
                                relatedEntity = new CustomEntity { tableId = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)att)._relatedTable, attribute = new List<CustomEntitiesUtilities.CustomEntityAttribute> { new CustomEntitiesUtilities.CustomEntityAttribute { FieldId = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)att)._relationshipdisplayfield } } };
                            }
                            else
                            {
                                foreach (var matchfield in ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)att)._matchFieldListItems)
                                {
                                    var attribute = (from attr in relatedEntity.attribute where attr.OldFieldId == matchfield._fieldid select attr).FirstOrDefault();
                                    //matchfield._attributeId = att._attributeid;
                                    matchfield._fieldid = attribute.FieldId;
                                    //matchfield._fieldid = CustomEntitiesUtilities.GetFieldIDbyFieldNameandTableID(relatedEntity.tableId, attribute.DisplayName, _executingProduct);
                                }
                            }

                            int attributeId = CustomEntitiesUtilities.CreateCustomEntityRelationship(att.ParentEntity, (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)att, relatedEntity, _executingProduct);
                            Assert.IsTrue(attributeId > 0);
                            listOfAttribute.Add((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)att);
                        }
                    }


                    foreach (var joinVia in JoinVias)
                    {
                        if (joinVia.OLDjoinViaID == 0)
                        {
                            foreach (var value in joinVia.JoinViaParts.Values)
                            {
                                if (value._type == CustomEntitiesUtilities.JoinViaPart.IDType.Field)
                                {
                                    var relatedFieldID = (from attr in listOfAttribute where attr.OldFieldId == value.RelatedID select attr).FirstOrDefault();

                                    value.RelatedID = relatedFieldID != null ? relatedFieldID.FieldId : value.RelatedID;
                                }
                            }

                            int result = CustomEntitiesUtilities.SaveJoinVia(joinVia, _executingProduct);
                            Assert.IsTrue(result > 0);
                        }
                    }


                        var manytooneattributes1 = from att in listOfAttribute where !att.SystemAttribute && att._fieldType == FieldType.Relationship && att.GetType() == typeof(CustomEntitiesUtilities.CustomEntityNtoOneAttribute) select att;

                        foreach (CustomEntitiesUtilities.CustomEntityAttribute att in manytooneattributes1)
                        {
                            foreach (var filter in ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)att).Filters)
                            {
                                var customatt = (from attr in listOfAttribute where attr.OldFieldId == filter._fieldid select attr).FirstOrDefault();
                                var systematt = (from attr in listOfAttribute where attr.OldFieldId == filter._fieldid select attr).FirstOrDefault();
                                var fieldAtt = new CustomEntitiesUtilities.CustomEntityAttribute { OldFieldId = filter._fieldid };

                                if (customatt != null)
                                {
                                    filter._fieldid = customatt.FieldId;
                                    if (customatt.GetType() == typeof(CustomEntitiesUtilities.cCustomEntityListAttribute))
                                    {
                                        if (filter._valueOne != null)
                                        {
                                            filter._valueOne = (from item in ((CustomEntitiesUtilities.cCustomEntityListAttribute)customatt)._listItems where item.OldValueId == int.Parse(filter._valueOne) select item._valueid.ToString()).FirstOrDefault();
                                        }
                                        //if (filter._valueTwo != null)
                                        //{
                                        //    filter._va = (from item in ((CustomEntitiesUtilities.cCustomEntityListAttribute)customatt)._listItems where item.OldValueId.ToString() == filter._valueOne select item._valueid.ToString()).FirstOrDefault();
                                        //}
                                    }
                                }
                                else if (systematt != null)
                                {
                                    filter._fieldid = systematt.FieldId;
                                }
                                else
                                {
                                    filter._fieldid = fieldAtt.OldFieldId;
                                }

                                filter._joinViaId = filter._joinViaId != null ? (from joinvia in JoinVias where joinvia.OLDjoinViaID == filter._joinViaId select joinvia._joinViaID).FirstOrDefault() : filter._joinViaId;
                                CustomEntitiesUtilities.CreateViewFilters(attribute: att, filtertoSave: filter, executingProduct: _executingProduct);
                            }
                        }

                        foreach (var entity in _customEntities)
                        {
                            var lookupFields = entity.attribute.Where(x => x._fieldType == FieldType.LookupDisplayField);
                            foreach (var lookupField in lookupFields)
                            {
                                var relatedAttribute = entity.attribute.Where(x => x.OldAttributeId == lookupField.TriggerAttributeId).Select(x => x).FirstOrDefault();
                                lookupField.TriggerAttributeId = relatedAttribute._attributeid;

                                var customatt = (from attr in listOfAttribute where attr.OldFieldId == lookupField.TriggerDisplayFieldID select attr).FirstOrDefault();
                                var systematt = (from attr in listOfAttribute where attr.OldFieldId == lookupField.TriggerDisplayFieldID select attr).FirstOrDefault();
                                var fieldAtt = new CustomEntitiesUtilities.CustomEntityAttribute { OldFieldId = (Guid)lookupField.TriggerDisplayFieldID };

                                if (customatt != null)
                                {
                                    lookupField.TriggerDisplayFieldID = customatt.FieldId;
                                }
                                else if (systematt != null)
                                {
                                    lookupField.TriggerDisplayFieldID = systematt.FieldId;
                                }
                                else
                                {
                                    lookupField.TriggerDisplayFieldID = fieldAtt.OldFieldId;
                                }

                                lookupField.TriggerJoinViaId = lookupField.TriggerJoinViaId != null ? (from joinvia in JoinVias where joinvia.OLDjoinViaID == lookupField.TriggerJoinViaId select joinvia._joinViaID).FirstOrDefault() : lookupField.TriggerJoinViaId;
                                int attributeid = CustomEntitiesUtilities.CreateCustomEntityAttribute(entity, lookupField, _executingProduct);
                                Assert.IsTrue(attributeid > 0);
                            }
                        }
                    #endregion

                    #region create forms only
                        foreach (var entity in _customEntities)
                        {
                            foreach (var form in entity.form)
                            {
                                int resultform = CustomEntitiesUtilities.CreateCustomEntityForm(entity, form, _executingProduct);
                                Assert.IsTrue(resultform > 0);
                            }
                        }
                    #endregion

                    #region create views / fields /  filters
                    foreach (var entity in _customEntities)
                    {
                        foreach (var view in entity.view)
                        {
                            int resultview = CustomEntitiesUtilities.CreateCustomEntityView(entity, view, _executingProduct);
                            view._viewid = resultview;
                            view.addform = (from form in entity.form where view.AddFormId == form.FormOldId select form).FirstOrDefault();
                            view.editform = (from form in entity.form where view.EditFormId == form.FormOldId select form).FirstOrDefault();
                            Assert.IsTrue(view._viewid > 0);
                            foreach (var field in view.fields)
                            {
                                var customatt = (from attr in listOfAttribute where attr.OldFieldId == field._fieldid select attr).FirstOrDefault();
                                var systematt = (from attr in listOfAttribute where attr.OldFieldId == field._fieldid select attr).FirstOrDefault();
                                var fieldAtt = new CustomEntitiesUtilities.CustomEntityAttribute { OldFieldId = field._fieldid };

                                if (customatt != null)
                                {
                                    field._fieldid = customatt.FieldId;
                                }
                                else if (systematt != null)
                                {
                                    field._fieldid = systematt.FieldId;
                                }
                                else
                                {
                                    field._fieldid = fieldAtt.OldFieldId;
                                }

                                field._joinViaId = field._joinViaId != null ? (from joinvia in JoinVias where joinvia.OLDjoinViaID == field._joinViaId select joinvia._joinViaID).FirstOrDefault() : field._joinViaId;
                                CustomEntitiesUtilities.CreateViewFields(view, field, _executingProduct);
                            }

                            foreach (var filter in view.filters)
                            {
                                var customatt = (from attr in listOfAttribute where attr.OldFieldId == filter._fieldid select attr).FirstOrDefault();
                                var systematt = (from attr in listOfAttribute where attr.OldFieldId == filter._fieldid select attr).FirstOrDefault();
                                var fieldAtt = new CustomEntitiesUtilities.CustomEntityAttribute { OldFieldId = filter._fieldid };

                                if (customatt != null)
                                {
                                    filter._fieldid = customatt.FieldId;
                                    filter._fieldid = customatt.FieldId;
                                    if (customatt.GetType() == typeof(CustomEntitiesUtilities.cCustomEntityListAttribute))
                                    {
                                        if (filter._valueOne != null)
                                        {
                                            filter._valueOne = (from item in ((CustomEntitiesUtilities.cCustomEntityListAttribute)customatt)._listItems where item.OldValueId.ToString() == filter._valueOne select item._valueid.ToString()).FirstOrDefault();
                                        }
                                        //if (filter._valueTwo != null)
                                        //{
                                        //    filter._va = (from item in ((CustomEntitiesUtilities.cCustomEntityListAttribute)customatt)._listItems where item.OldValueId.ToString() == filter._valueOne select item._valueid.ToString()).FirstOrDefault();
                                        //}
                                    }
                                }
                                else if (systematt != null)
                                {
                                    filter._fieldid = systematt.FieldId;
                                }
                                else
                                {
                                    filter._fieldid = fieldAtt.OldFieldId;
                                }

                                filter._joinViaId = filter._joinViaId != null ? (from joinvia in JoinVias where joinvia.OLDjoinViaID == filter._joinViaId select joinvia._joinViaID).FirstOrDefault() : filter._joinViaId;
                                CustomEntitiesUtilities.CreateViewFilters(view: view, filtertoSave: filter, executingProduct: _executingProduct);
                            }
                        }
                    }
                    #endregion

                    #region create all one to many
                    foreach (var entity in _customEntities)
                    {
                        var onetomanyattributes = from att in entity.attribute where !att.SystemAttribute && att._fieldType == FieldType.Relationship && att.GetType() == typeof(CustomEntitiesUtilities.CustomEntityOnetoNAttribute) select att;
                        foreach (CustomEntitiesUtilities.CustomEntityAttribute att in onetomanyattributes)
                        {
                            var relatedEntity = _customEntities.Where(enti => enti.OldTableId == ((CustomEntitiesUtilities.CustomEntityOnetoNAttribute)att)._relatedTable).FirstOrDefault();
                            ((CustomEntitiesUtilities.CustomEntityOnetoNAttribute)att)._viewId = relatedEntity.view.Where(v => v.OldViewId == ((CustomEntitiesUtilities.CustomEntityOnetoNAttribute)att)._viewId).Select(x => x._viewid).FirstOrDefault();
                            int attributeId = CustomEntitiesUtilities.CreateCustomEntityRelationship(att.ParentEntity, (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)att, relatedEntity, _executingProduct);
                            Assert.IsTrue(attributeId > 0);
                        }
                    }
                    #endregion

                    #region create all form tabse/sections/fields
                    foreach (var entity in _customEntities)
                    {
                        foreach (var form in entity.form)
                        {
                            //int resultform = CustomEntitiesUtilities.CreateCustomEntityForm(entity, form, _executingProduct);
                            //Assert.IsTrue(resultform > 0);
                            foreach (var tab in form.tabs)
                            {
                                int resulttab = CustomEntitiesUtilities.CreateFormTabs(form._formid, tab, _executingProduct);
                                Assert.IsTrue(resulttab > 0);
                                foreach (var section in tab.sections)
                                {
                                    int resultsect = CustomEntitiesUtilities.CreateFormSection(form._formid, tab._tabid, section, _executingProduct);
                                    Assert.IsTrue(resultsect > 0);
                                    foreach (var field in section.fields)
                                    {
                                        field.AttributeId = entity.attribute.Where(att => att.OldAttributeId == field.AttributeId).Select(x => x._attributeid).FirstOrDefault();
                                        int resultfield = CustomEntitiesUtilities.CreateFormFields(form._formid, section._sectionid, field, _executingProduct);
                                        Assert.IsTrue(resultfield > 0);
                                    }
                                }
                            }
                        }
                    #endregion

                    }
                    break;
                case "CustomEntityFormsClientSuccessfullyCreateCustomEntityClientRecord_UITest":
                    for (int entityIndex = _customEntities.Count - 1; entityIndex > -1; entityIndex--)
                    {
                        #region create entity
                        int result = CustomEntityDatabaseAdapter.CreateCustomEntity(_customEntities[entityIndex], _executingProduct);
                        _customEntities[entityIndex].entityId = result;
                        Assert.IsTrue(result > 0);
                        foreach (var attribute in _customEntities[entityIndex].attribute)
                        {
                            attribute._mandatory = false;
                            #region create attribute
                            if (!attribute.SystemAttribute)
                            {
                                if (attribute._fieldType != FieldType.Relationship && attribute.GetType() != typeof(UserDefinedFields) && attribute.GetType() != typeof(UserDefinedFieldTypeList))
                                {
                                    #region create standard attribute
                                    attribute._attributeid = 0;
                                    int resultatt = CustomEntitiesUtilities.CreateCustomEntityAttribute(_customEntities[entityIndex], (CustomEntitiesUtilities.CustomEntityAttribute)attribute, _executingProduct);
                                    Assert.IsTrue(resultatt > 0);
                                    #endregion
                                }
                                else if (attribute._fieldType == FieldType.LookupDisplayField)
                                {

                                }
                                else if (attribute.GetType() == typeof(UserDefinedFields) && entityIndex == 0 || attribute.GetType() == typeof(UserDefinedFieldTypeList) && entityIndex == 0)
                                {
                                    #region create userdefinedfield
                                    attribute._attributeid = 0;
                                    int resultatt = UserDefinedFieldsRepository.CreateUserDefinedField((UserDefinedFields)attribute, _executingProduct);
                                    Assert.IsTrue(resultatt > 0);
                                    #endregion
                                }
                                else if (attribute._fieldType == FieldType.Relationship && attribute.GetType() == typeof(CustomEntitiesUtilities.CustomEntityNtoOneAttribute))
                                {
                                    #region create ntoone relationship
                                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute ntooneattribute = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)attribute;
                                    manyToOneAttributes.Add(ntooneattribute);
                                    ntooneattribute._attributeid = 0;
                                    int attributeId = CustomEntitiesUtilities.CreateCustomEntityRelationship(_customEntities[entityIndex], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)ntooneattribute, _customEntities[entityIndex + 1], _executingProduct);
                                    Assert.IsTrue(attributeId > 0);
                                    #endregion
                                }
                                else
                                {
                                    #region create oneton relationship
                                    CustomEntitiesUtilities.CustomEntityOnetoNAttribute onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)attribute;
                                    oneToNAttribute.Add(onetonattribute);
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        foreach (var form in _customEntities[entityIndex].form)
                        {

                            #region create all forms/tabs/sections/fields
                            CustomEntitiesUtilities.CreateCustomEntityForm(_customEntities[entityIndex], form, _executingProduct);
                            Assert.IsTrue(form._formid > 0);
                            foreach (CustomEntitiesUtilities.CustomEntityFormTab tab in form.tabs)
                            {
                                #region create tab
                                CustomEntitiesUtilities.CreateFormTabs(form._formid, tab, _executingProduct);
                                Assert.IsTrue(tab._tabid > 0);
                                foreach (CustomEntitiesUtilities.CustomEntityFormSection section in tab.sections)
                                {
                                    #region create section
                                    CustomEntitiesUtilities.CreateFormSection(form._formid, tab._tabid, section, _executingProduct);
                                    Assert.IsTrue(form._formid > 0);
                                    foreach (CustomEntitiesUtilities.CustomEntityFormField field in section.fields)
                                    {
                                        #region create field
                                        field.attribute = (from att in _customEntities[0].attribute
                                                           where att.DisplayName == field._labelText
                                                           select att).FirstOrDefault();
                                        CustomEntitiesUtilities.CreateFormFields(form._formid, section._sectionid, field, _executingProduct);
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                            #region create few forms/tabs/sections/fields
                            ////int attributeindex = 0;
                            //CustomEntitiesUtilities.CreateCustomEntityForm(_customEntities[entityIndex], form, _executingProduct);
                            //Assert.IsTrue(form._formid > 0);
                            //for (int formtabindex = 0; formtabindex < 1; formtabindex++)
                            //{
                            //    #region create tab
                            //    if (form.tabs.Count != 0)
                            //    {
                            //        var tab = form.tabs[formtabindex];
                            //        CustomEntitiesUtilities.CreateFormTabs(form._formid, tab, _executingProduct);
                            //        Assert.IsTrue(tab._tabid > 0);
                            //        for (int sectionindex = 0; sectionindex < 2; sectionindex++)
                            //        {
                            //            #region create section
                            //            var section = tab.sections[sectionindex];
                            //            CustomEntitiesUtilities.CreateFormSection(form._formid, tab._tabid, section, _executingProduct);
                            //            Assert.IsTrue(form._formid > 0);
                            //            for (int fieldindex = 0; fieldindex < section.fields.Count; fieldindex++)
                            //            {
                            //                #region create field
                            //                var field = section.fields[fieldindex];
                            //                field.attribute = (from att in _customEntities[entityIndex].attribute
                            //                                   where att.DisplayName == field._labelText
                            //                                   select att).FirstOrDefault();
                            //                CustomEntitiesUtilities.CreateFormFields(form._formid, section._sectionid, field, _executingProduct);
                            //                //attributeindex++;
                            //                #endregion
                            //            }
                            //            #endregion
                            //        }
                            //    }
                            //    #endregion
                            //}
                            #endregion
                        }
                        foreach (var view in _customEntities[entityIndex].view)
                        {
                            #region create view
                            if ((view._allowAdd == true && view.addform != null && view._allowEdit == true && view.editform != null) || (view._allowAdd == false && view.addform == null && view._allowEdit == false && view.editform == null))
                            {
                                int resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[entityIndex], view, _executingProduct);
                                view._viewid = resultview;
                                Assert.IsTrue(view._viewid > 0);
                                for (int viewfieldindex = 0; viewfieldindex < view.fields.Count; viewfieldindex++)
                                {
                                    #region create view field
                                    var attribute = _customEntities[entityIndex].attribute[viewfieldindex];
                                    if (viewfieldindex < 17 && attribute._fieldType != FieldType.Comment && attribute._fieldType != FieldType.LargeText && attribute._format != Convert.ToByte(Format.FormattedTextBox))
                                    {
                                        var field = view.fields[viewfieldindex];
                                        field._fieldid = attribute.FieldId;
                                        CustomEntitiesUtilities.CreateViewFields(view, field, _executingProduct);
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        foreach (var oneton in oneToNAttribute)
                        {
                            #region create oneton relationship
                            oneton._attributeid = 0;
                            int attributeId = CustomEntitiesUtilities.CreateCustomEntityRelationship(_customEntities[entityIndex], oneton, _customEntities[entityIndex + 1], _executingProduct);
                            Assert.IsTrue(attributeId > 0);
                            #endregion
                        }
                        #endregion
                    }
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                default:
                    for (int entityIndex = _customEntities.Count - 1; entityIndex > -1; entityIndex--)
                    {
                        #region create entity
                        int result = CustomEntityDatabaseAdapter.CreateCustomEntity(_customEntities[entityIndex], _executingProduct);
                        _customEntities[entityIndex].entityId = result;
                        Assert.IsTrue(result > 0);
                        foreach (var attribute in _customEntities[entityIndex].attribute)
                        {
                            attribute._mandatory = false;
                            #region create attribute
                            if (!attribute.SystemAttribute)
                            {
                                if (attribute._fieldType != FieldType.Relationship && attribute.GetType() != typeof(UserDefinedFields) && attribute.GetType() != typeof(UserDefinedFieldTypeList))
                                {
                                    #region create standard attribute
                                    attribute._attributeid = 0;
                                    int resultatt = CustomEntitiesUtilities.CreateCustomEntityAttribute(_customEntities[entityIndex], (CustomEntitiesUtilities.CustomEntityAttribute)attribute, _executingProduct);
                                    Assert.IsTrue(resultatt > 0);
                                    #endregion
                                }
                                else if (attribute.GetType() == typeof(UserDefinedFields) && entityIndex == 0 || attribute.GetType() == typeof(UserDefinedFieldTypeList) && entityIndex == 0)
                                {
                                    #region create userdefinedfield
                                    attribute._attributeid = 0;
                                    int resultatt = UserDefinedFieldsRepository.CreateUserDefinedField((UserDefinedFields)attribute, _executingProduct);
                                    Assert.IsTrue(resultatt > 0);
                                    #endregion
                                }
                                else if (attribute._fieldType == FieldType.Relationship && attribute.GetType() == typeof(CustomEntitiesUtilities.CustomEntityNtoOneAttribute))
                                {
                                    #region create ntoone relationship
                                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute ntooneattribute = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)attribute;
                                    ntooneattribute._attributeid = 0;
                                    int attributeId = CustomEntitiesUtilities.CreateCustomEntityRelationship(_customEntities[entityIndex], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)ntooneattribute, _customEntities[entityIndex + 1], _executingProduct);
                                    Assert.IsTrue(attributeId > 0);
                                    #endregion
                                }
                                else
                                {
                                    #region create oneton relationship
                                    CustomEntitiesUtilities.CustomEntityOnetoNAttribute onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)attribute;
                                    oneToNAttribute.Add(onetonattribute);
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        foreach (var form in _customEntities[entityIndex].form)
                        {

                            #region create all forms/tabs/sections/fields
                            CustomEntitiesUtilities.CreateCustomEntityForm(_customEntities[entityIndex], form, _executingProduct);
                            Assert.IsTrue(form._formid > 0);
                            foreach (CustomEntitiesUtilities.CustomEntityFormTab tab in form.tabs)
                            {
                                #region create tab
                                CustomEntitiesUtilities.CreateFormTabs(form._formid, tab, _executingProduct);
                                Assert.IsTrue(tab._tabid > 0);
                                foreach (CustomEntitiesUtilities.CustomEntityFormSection section in tab.sections)
                                {
                                    #region create section
                                    CustomEntitiesUtilities.CreateFormSection(form._formid, tab._tabid, section, _executingProduct);
                                    Assert.IsTrue(form._formid > 0);
                                    foreach (CustomEntitiesUtilities.CustomEntityFormField field in section.fields)
                                    {
                                        #region create field
                                        field.attribute = (from att in _customEntities[0].attribute
                                                           where att.DisplayName == field._labelText
                                                           select att).FirstOrDefault();
                                        CustomEntitiesUtilities.CreateFormFields(form._formid, section._sectionid, field, _executingProduct);
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                            #region create few forms/tabs/sections/fields
                            ////int attributeindex = 0;
                            //CustomEntitiesUtilities.CreateCustomEntityForm(_customEntities[entityIndex], form, _executingProduct);
                            //Assert.IsTrue(form._formid > 0);
                            //for (int formtabindex = 0; formtabindex < 1; formtabindex++)
                            //{
                            //    #region create tab
                            //    if (form.tabs.Count != 0)
                            //    {
                            //        var tab = form.tabs[formtabindex];
                            //        CustomEntitiesUtilities.CreateFormTabs(form._formid, tab, _executingProduct);
                            //        Assert.IsTrue(tab._tabid > 0);
                            //        for (int sectionindex = 0; sectionindex < 2; sectionindex++)
                            //        {
                            //            #region create section
                            //            var section = tab.sections[sectionindex];
                            //            CustomEntitiesUtilities.CreateFormSection(form._formid, tab._tabid, section, _executingProduct);
                            //            Assert.IsTrue(form._formid > 0);
                            //            for (int fieldindex = 0; fieldindex < section.fields.Count; fieldindex++)
                            //            {
                            //                #region create field
                            //                var field = section.fields[fieldindex];
                            //                field.attribute = (from att in _customEntities[entityIndex].attribute
                            //                                   where att.DisplayName == field._labelText
                            //                                   select att).FirstOrDefault();
                            //                CustomEntitiesUtilities.CreateFormFields(form._formid, section._sectionid, field, _executingProduct);
                            //                //attributeindex++;
                            //                #endregion
                            //            }
                            //            #endregion
                            //        }
                            //    }
                            //    #endregion
                            //}
                            #endregion
                        }
                        foreach (var view in _customEntities[entityIndex].view)
                        {
                            #region create view
                            if ((view._allowAdd == true && view.addform != null && view._allowEdit == true && view.editform != null) || (view._allowAdd == false && view.addform == null && view._allowEdit == false && view.editform == null))
                            {
                                int resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[entityIndex], view, _executingProduct);
                                view._viewid = resultview;
                                Assert.IsTrue(view._viewid > 0);
                                for (int viewfieldindex = 0; viewfieldindex < view.fields.Count; viewfieldindex++)
                                {
                                    #region create view field
                                    var attribute = _customEntities[entityIndex].attribute[viewfieldindex];
                                    if (viewfieldindex < 17 && attribute._fieldType != FieldType.Comment && attribute._fieldType != FieldType.LargeText && attribute._format != Convert.ToByte(Format.FormattedTextBox))
                                    {
                                        var field = view.fields[viewfieldindex];
                                        field._fieldid = attribute.FieldId;
                                        CustomEntitiesUtilities.CreateViewFields(view, field, _executingProduct);
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        foreach (var oneton in oneToNAttribute)
                        {
                            #region create oneton relationship
                            oneton._attributeid = 0;
                            int attributeId = CustomEntitiesUtilities.CreateCustomEntityRelationship(_customEntities[entityIndex], oneton, _customEntities[entityIndex + 1], _executingProduct);
                            Assert.IsTrue(attributeId > 0);
                            #endregion
                        }
                        #endregion
                    }
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
            }
        }
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public string ConvertDBDateTimeToUIDate(string dbDateTime)
        {
            DateTime date = Convert.ToDateTime(dbDateTime);
            string uiDate = date.ToShortDateString();
            uiDate.Replace('-', '/');
            return uiDate;
        }

        public string ConvertDBDateTimeToUITime(string dbDateTime)
        {
            DateTime date = Convert.ToDateTime(dbDateTime);
            string uiTime = date.ToShortTimeString().Substring(0, 5);
            return uiTime;
        }
    }
}
