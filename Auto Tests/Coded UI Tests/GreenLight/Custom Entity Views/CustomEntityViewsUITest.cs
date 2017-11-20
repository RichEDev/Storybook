using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
using Auto_Tests.UIMaps.CustomEntityViewsUIMapClasses;
using Auto_Tests.Tools;
using System.Configuration;
using Auto_Tests.Coded_UI_Tests.GreenLight.CustomVEntityViews.ViewsDatabaseAdaptor;
using System.Windows.Forms;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Forms;
using Auto_Tests.UIMaps.CustomEntityFormsUIMapClasses;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Data.SqlClient;
using Auto_Tests.Product_Variables.ModalMessages;
using Auto_Tests.UIMaps.CustomEntityAttributesUIMapClasses;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Auto_Tests.UIMaps.UserDefinedFieldsUIMapClasses;
using Auto_Tests.UIMaps.CustomEntityNtoOneAttributesUIMapClasses;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_N_to_One_Attributes;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;
using Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.User_Defined_Fields;

namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Views
{
    using System.Linq;

    /// <summary>
    /// Custom Entity Views Automated Test Suite
    /// </summary>
    [CodedUITest]
    public class CustomEntityViewsUITest
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private readonly static ProductType _executingProduct = cGlobalVariables.GetProductFromAppConfig();
        /// <summary>
        /// Cached list of Greenlights
        /// </summary>
        private static List<CustomEntity> _customEntities;
        /// <summary>
        /// Cached list of UDF
        /// </summary>
        private static List<UserDefinedFields> _userDefinedFields;
        /// <summary>
        /// Custom Entity Forms UI Map
        /// </summary>
        private CustomEntityFormsUIMap _customEntityFormsUIMap = new CustomEntityFormsUIMap();
        /// <summary>
        /// Custom Entity Views UI Mape
        /// </summary>
        private CustomEntityViewsUIMap _customEntityViewsUIMap = new CustomEntityViewsUIMap();
        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();
        /// <summary>
        /// List of filters that have been set up proir to running the test
        /// </summary>
        private Dictionary<CustomEntitiesUtilities.CustomEntityViewFilter, CustomEntitiesUtilities.CustomEntityAttribute> _enabledFilters = new Dictionary<CustomEntitiesUtilities.CustomEntityViewFilter, CustomEntitiesUtilities.CustomEntityAttribute>();
        /// <summary>
        /// Standard empty drop down text label
        /// </summary>
        private string EmptyDropDown = "[None]";
        private List<string> GridText = new List<string>();

        #region Additional test attributes

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            _sharedMethods.RestoreDefaultSortingOrder("gridAttributes", _executingProduct);
            _sharedMethods.Logon(_executingProduct, LogonType.administrator);
            CachePopulatorForViews CustomEntityDataFromLithium = new CachePopulatorForViews(_executingProduct);
            _customEntities = CustomEntityDataFromLithium.PopulateCache();
            _userDefinedFields = UserDefinedFieldsRepository.PopulateUserDefinedFields();

            Assert.IsNotNull(_customEntities);
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            _sharedMethods.CloseBrowserWindow();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            for (int entityIndex = _customEntities.Count - 1; entityIndex > -1; entityIndex--)
            {
                //Random random = new Random();
                //int count = random.Next(15, 20);
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                _customEntities[entityIndex].entityName = "Custom Entity " + genericName.ToString();
                _customEntities[entityIndex].pluralName = "Custom Entity " + genericName.ToString();
                _customEntities[entityIndex].description = "Custom Entity " + genericName.ToString();
                _customEntities[entityIndex].entityId = 0;
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(_customEntities[entityIndex], _executingProduct);
                _customEntities[entityIndex].entityId = result;
                Assert.IsTrue(result > 0, "Greenlight creation failed");
                for (int indexer = 0; indexer < _customEntities[entityIndex].attribute.Count; indexer++)
                {
                    CustomEntitiesUtilities.CustomEntityAttribute attribute= _customEntities[entityIndex].attribute[indexer];
                    if (!attribute.SystemAttribute)
                    {
                        if (attribute._fieldType != FieldType.Relationship && attribute.GetType() != typeof(UserDefinedFields) && attribute.GetType() != typeof(UserDefinedFieldTypeList))
                        {
                            attribute._attributeid = 0;
                            int resultatt = CustomEntitiesUtilities.CreateCustomEntityAttribute(_customEntities[entityIndex], (CustomEntitiesUtilities.CustomEntityAttribute)attribute, _executingProduct);
                            Assert.IsTrue(resultatt > 0);
                        }
                        else if (attribute.GetType() == typeof(UserDefinedFields) && entityIndex == 0 || attribute.GetType() == typeof(UserDefinedFieldTypeList) && entityIndex == 0)
                        {
                            attribute._attributeid = 0;
                            int resultatt = UserDefinedFieldsRepository.CreateUserDefinedField((UserDefinedFields)attribute, _executingProduct);
                            Assert.IsTrue(resultatt > 0);
                        }
                        else
                        {
                            if (attribute.GetType() == typeof(CustomEntitiesUtilities.CustomEntityNtoOneAttribute))
                            {
                                CustomEntitiesUtilities.CustomEntityNtoOneAttribute updateatt = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)attribute;
                                updateatt._attributeid = 0;
                                int attributeId = CustomEntitiesUtilities.CreateCustomEntityRelationship(_customEntities[entityIndex], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)updateatt, _customEntities[entityIndex + 1], _executingProduct);
                                Assert.IsTrue(attributeId > 0);
                            }
                        }
                    }
                }
                foreach (CustomEntitiesUtilities.CustomEntityForm form in _customEntities[entityIndex].form)
                {
                    CustomEntitiesUtilities.CreateCustomEntityForm(_customEntities[entityIndex], form, _executingProduct);
                    Assert.IsTrue(form._formid > 0);
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            Assert.IsNotNull(_customEntities);
            for (int entityindexer = 0; entityindexer < _customEntities.Count; entityindexer++)
            {
                int result = CustomEntityDatabaseAdapter.DeleteCustomEntity(_customEntities[entityindexer].entityId, _executingProduct);
                Assert.AreEqual(0, result);
            }
            foreach (var udf in _userDefinedFields)
            {
                if (udf._attributeid > 0)
                {
                    UserDefinedFieldsRepository.DeleteUserDefinedField(udf, _executingProduct);
                }
            }
            foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in _customEntities[0].attribute)
            {
                if (attribute._fieldType == FieldType.Relationship)
                {
                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute nToOneRelationship = attribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                    nToOneRelationship._isExpanded = false;
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();
        }

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
        #endregion

        #region General Details
        #region 38001 Successfully verify maximum size of fields on views modal
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifyMaximumSizeOfFieldsOnViewsModal_UITest()
        {
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickNewViewLink();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            //Check max length
            Assert.AreEqual(100, viewControls.ViewNameTxt.MaxLength);
            Assert.AreEqual(4000, viewControls.DescriptionTxt.GetMaxLength());
            Assert.AreEqual(4000, viewControls.menuDescriptionTxt.GetMaxLength());

            //Copy and paste text - ensure truncation occurs
            Clipboard.Clear();
            try { Clipboard.SetText(Strings.BasicString); }
            catch (Exception) { }

            _customEntityFormsUIMap.RightClickAndPaste(viewControls.ViewNameTxt);
            viewControls.ViewNameTxt.WaitForControlCondition(tc =>
            {
                return viewControls.ViewNameTxt.Text.Length == 100;
            }, 1000);

            Assert.AreEqual(100, viewControls.ViewNameTxt.Text.Length);
            Clipboard.Clear();

            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }
            _customEntityFormsUIMap.RightClickAndPaste(viewControls.DescriptionTxt);

            //Wait for condition allows main thread to execute whilst waiting
            //for the text property to change. This will fail if the 
            //main thread doesn't respond within 2 seconds or get max length doesn't
            //equal 4000  after 2 seconds.
            //viewControls.DescriptionTxt.WaitForControlPropertyEqual(, 4000);
            viewControls.DescriptionTxt.WaitForControlCondition(tc =>
            {
                return viewControls.DescriptionTxt.GetMaxLength() == 4000;
            }, 2000);

            Clipboard.Clear();
            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }
            _customEntityFormsUIMap.RightClickAndPaste(viewControls.menuDescriptionTxt);

            //Wait for condition allows main thread to execute whilst waiting
            // for the text property to change. This will fail if the 
            //main thread doesn't respond within 2 seconds or get max length doesn't
            // equal 4000  after 2 seconds.
            viewControls.DescriptionTxt.WaitForControlCondition(tc =>
            {
                return viewControls.DescriptionTxt.GetMaxLength() == 4000;
            }, 2000);
            Clipboard.Clear();
        }
        #endregion

        #region 38094 Successfully verify page standards on views modal
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifyPageStandardsOnViewsModal_UITest()
        {
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickNewViewLink();

            _customEntityViewsUIMap.ValidateNewViewModalHeader();

            _customEntityViewsUIMap.ValidateGeneralDetailsSectionHeader();

            _customEntityViewsUIMap.ValidateViewNameLabel();

            _customEntityViewsUIMap.ValidateDescriptionLabel();

            _customEntityViewsUIMap.ValidateMenuDisplayOptionsSectionHeader();

            _customEntityViewsUIMap.ValidateMenuDropDownLabel();

            _customEntityViewsUIMap.ValidateMenuDescriptionLabel();

            _customEntityViewsUIMap.ValidateOptionsSectionHeader();

            _customEntityViewsUIMap.ValidateAddFormDropDownLabel();

            _customEntityViewsUIMap.ValidateEditFormDropDownLabel();

            _customEntityViewsUIMap.ValidateAddFormDropDownLabel();

            _customEntityViewsUIMap.ValidateEditFormDropDownLabel();

            _customEntityViewsUIMap.ValidateAllowDeleteTickBoxLabel();

            _customEntityViewsUIMap.ValidateAllowApprovalTickBoxLabel();
        }
        #endregion

        #region 36562 Successfully add view to custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddViewToCustomEntity_UITest()
        {
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            foreach (CustomEntitiesUtilities.CustomEntityView view in _customEntities[0].view)
            {
                _customEntityViewsUIMap.ClickNewViewLink();

                CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

                #region Ensure on page load that default values exist!

                Assert.AreEqual(string.Empty, viewControls.ViewNameTxt.Text);
                Assert.AreEqual(string.Empty, viewControls.DescriptionTxt.Text);
                Assert.AreEqual("[None]", viewControls.menulist.SelectedItem);
                Assert.AreEqual(string.Empty, viewControls.menuDescriptionTxt.Text);
                Assert.AreEqual("[None]", viewControls.addformlistlist.SelectedItem);
                Assert.AreEqual("[None]", viewControls.editformlist.SelectedItem);
                Assert.AreEqual(false, viewControls.AllowDeleteOption.Checked);
                Assert.AreEqual(false, viewControls.AllowApprovalOption.Checked);
                #endregion

                #region Populate page controls with values from database

                viewControls.ViewNameTxt.Text = view._viewName;
                viewControls.DescriptionTxt.Text = view._description;
                viewControls.menuDescriptionTxt.Text = view._menudescription;

                if (view._menuid != null)
                {
                    viewControls.menulist.SelectedItem = EnumHelper.GetEnumDescription((MenuItems)view._menuid);
                }
                if (view.addform != null)
                {
                    viewControls.addformlistlist.SelectedItem = view.addform._formName;
                }

                if (view.editform != null)
                {
                    viewControls.editformlist.SelectedItem = view.editform._formName;
                }

                viewControls.AllowDeleteOption.Checked = view._allowDelete;
                viewControls.AllowApprovalOption.Checked = view._allowApproval;

                #endregion

                _customEntityViewsUIMap.PressSaveOnViewModal();

                _customEntityViewsUIMap.ValidateViewTable(view._viewName, view._description, true);

                _customEntityViewsUIMap.ClickEditFieldLink(view._viewName);

                viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

                Assert.AreEqual(view._viewName, viewControls.ViewNameTxt.Text);
                Assert.AreEqual(view._description, viewControls.DescriptionTxt.Text);
                Assert.AreEqual(view._menuid == null ? "[None]" : EnumHelper.GetEnumDescription((MenuItems)view._menuid), viewControls.menulist.SelectedItem);
                Assert.AreEqual(view._menudescription, viewControls.menuDescriptionTxt.Text);
                Assert.AreEqual(view.addform == null ? "[None]" : view.addform._formName, viewControls.addformlistlist.SelectedItem);
                Assert.AreEqual(view.editform == null ? "[None]" : view.editform._formName, viewControls.editformlist.SelectedItem);
                Assert.AreEqual(view._allowDelete, viewControls.AllowDeleteOption.Checked);
                Assert.AreEqual(view._allowApproval, viewControls.AllowApprovalOption.Checked);

                _customEntityViewsUIMap.PressCancelOnViewModal();
            }
        }
        #endregion

        #region 37981 Unsuccessfully add view to custom entity where cancel is clicked
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddViewToCustomEntity_UITest()
        {
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickNewViewLink();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            Assert.AreEqual(string.Empty, viewControls.ViewNameTxt.Text);
            Assert.AreEqual(string.Empty, viewControls.DescriptionTxt.Text);
            Assert.AreEqual("[None]", viewControls.menulist.SelectedItem);
            Assert.AreEqual(string.Empty, viewControls.menuDescriptionTxt.Text);
            Assert.AreEqual("[None]", viewControls.addformlistlist.SelectedItem);
            Assert.AreEqual("[None]", viewControls.editformlist.SelectedItem);
            Assert.AreEqual(false, viewControls.AllowDeleteOption.Checked);
            Assert.AreEqual(false, viewControls.AllowApprovalOption.Checked);

            viewControls.ViewNameTxt.Text = _customEntities[0].view[0]._viewName;
            viewControls.DescriptionTxt.Text = _customEntities[0].view[0]._description;
            viewControls.menuDescriptionTxt.Text = _customEntities[0].view[0]._menudescription;
            viewControls.menulist.SelectedItem = EnumHelper.GetEnumDescription((MenuItems)_customEntities[0].view[0]._menuid);
            viewControls.addformlistlist.SelectedItem = _customEntities[0].view[0].addform._formName;
            viewControls.editformlist.SelectedItem = _customEntities[0].view[0].editform._formName;
            viewControls.AllowDeleteOption.Checked = _customEntities[0].view[0]._allowDelete;
            viewControls.AllowApprovalOption.Checked = _customEntities[0].view[0]._allowApproval;

            _customEntityViewsUIMap.PressCancelOnViewModal();

            _customEntityViewsUIMap.ValidateViewTable(_customEntities[0].view[0]._viewName, _customEntities[0].view[0]._description, false);

            _customEntityViewsUIMap.ClickNewViewLink();

            viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            Assert.AreEqual(string.Empty, viewControls.ViewNameTxt.Text);
            Assert.AreEqual(string.Empty, viewControls.DescriptionTxt.Text);
            Assert.AreEqual("[None]", viewControls.menulist.SelectedItem);
            Assert.AreEqual(string.Empty, viewControls.menuDescriptionTxt.Text);
            Assert.AreEqual("[None]", viewControls.addformlistlist.SelectedItem);
            Assert.AreEqual("[None]", viewControls.editformlist.SelectedItem);
            Assert.AreEqual(false, viewControls.AllowDeleteOption.Checked);
            Assert.AreEqual(false, viewControls.AllowApprovalOption.Checked);
        }
        #endregion

        #region 36643 Unsuccessfully add view to custom entity where madatory fields a missing
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddViewToCustomEntityWhereMadatoryFieldsAreMissing_UITest()
        {
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickNewViewLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(_customEntityViewsUIMap.UIGreenLightgreenlightWindow.UIGreenLightgreenlightDocument.UIViewNameAsterisk));

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a View name.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });

            _customEntityViewsUIMap.ValidateMessageModal();

            _customEntityViewsUIMap.PressCloseOnMessageModal();

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(_customEntityViewsUIMap.UIGreenLightgreenlightWindow.UIGreenLightgreenlightDocument.UIViewNameAsterisk));

            _customEntityViewsUIMap.PressCancelOnViewModal();

            _customEntityViewsUIMap.ValidateViewTable(_customEntities[0].view[0]._viewName, _customEntities[0].view[0]._description, false);
        }
        #endregion

        #region 36646 Unsuccessfully add duplicate view to custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddDuplicateViewToCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickNewViewLink();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            Assert.AreEqual(string.Empty, viewControls.ViewNameTxt.Text);
            Assert.AreEqual(string.Empty, viewControls.DescriptionTxt.Text);
            Assert.AreEqual("[None]", viewControls.menulist.SelectedItem);
            Assert.AreEqual(string.Empty, viewControls.menuDescriptionTxt.Text);
            Assert.AreEqual("[None]", viewControls.addformlistlist.SelectedItem);
            Assert.AreEqual("[None]", viewControls.editformlist.SelectedItem);
            Assert.AreEqual(false, viewControls.AllowDeleteOption.Checked);
            Assert.AreEqual(false, viewControls.AllowApprovalOption.Checked);

            viewControls.ViewNameTxt.Text = _customEntities[0].view[0]._viewName;
            viewControls.DescriptionTxt.Text = _customEntities[0].view[0]._description;
            viewControls.menulist.SelectedItem = EnumHelper.GetEnumDescription((MenuItems)_customEntities[0].view[0]._menuid);
            viewControls.menuDescriptionTxt.Text = _customEntities[0].view[0]._menudescription;
            viewControls.addformlistlist.SelectedItem = _customEntities[0].view[0].addform._formName;
            viewControls.editformlist.SelectedItem = _customEntities[0].view[0].editform._formName;
            viewControls.AllowDeleteOption.Checked = _customEntities[0].view[0]._allowDelete;
            viewControls.AllowApprovalOption.Checked = _customEntities[0].view[0]._allowApproval;

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nThe View name you have entered already exists.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });

            _customEntityViewsUIMap.ValidateMessageModal();
        }
        #endregion

        #region 36644 Successfully edit view to custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyEditViewToCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            viewControls.ViewNameTxt.Text = _customEntities[0].view[1]._viewName;
            viewControls.DescriptionTxt.Text = _customEntities[0].view[1]._description;
            viewControls.menulist.SelectedItem = EnumHelper.GetEnumDescription((MenuItems)_customEntities[0].view[1]._menuid);
            viewControls.menuDescriptionTxt.Text = _customEntities[0].view[1]._menudescription;
            viewControls.addformlistlist.SelectedItem = _customEntities[0].view[1].addform._formName;
            viewControls.editformlist.SelectedItem = _customEntities[0].view[1].editform._formName;
            viewControls.AllowDeleteOption.Checked = _customEntities[0].view[1]._allowDelete;
            viewControls.AllowApprovalOption.Checked = _customEntities[0].view[1]._allowApproval;

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ValidateViewTable(_customEntities[0].view[1]._viewName, _customEntities[0].view[1]._description, true);

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[1]._viewName);

            viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            CustomEntitiesUtilities.CustomEntityView viewtoAdd = _customEntities[0].view[1];

            Assert.AreEqual(viewtoAdd._viewName, viewControls.ViewNameTxt.Text);
            Assert.AreEqual(viewtoAdd._description, viewControls.DescriptionTxt.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((MenuItems)viewtoAdd._menuid), viewControls.menulist.SelectedItem);
            Assert.AreEqual(viewtoAdd._menudescription, viewControls.menuDescriptionTxt.Text);
            Assert.AreEqual(viewtoAdd.addform._formName, viewControls.addformlistlist.SelectedItem);
            Assert.AreEqual(viewtoAdd.editform._formName, viewControls.editformlist.SelectedItem);
            Assert.AreEqual(viewtoAdd._allowDelete, viewControls.AllowDeleteOption.Checked);
            Assert.AreEqual(viewtoAdd._allowApproval, viewControls.AllowApprovalOption.Checked);

            _customEntityViewsUIMap.PressCancelOnViewModal();
        }
        #endregion

        #region 38005 Unsuccessfully edit view to custom entity where cancel is clicked
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyEditViewToCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            viewControls.ViewNameTxt.Text = _customEntities[0].view[1]._viewName;
            viewControls.DescriptionTxt.Text = _customEntities[0].view[1]._description;
            viewControls.menulist.SelectedItem = EnumHelper.GetEnumDescription((MenuItems)_customEntities[0].view[1]._menuid);
            viewControls.menuDescriptionTxt.Text = _customEntities[0].view[1]._menudescription;
            viewControls.addformlistlist.SelectedItem = _customEntities[0].view[1].addform._formName;
            viewControls.editformlist.SelectedItem = _customEntities[0].view[1].editform._formName;
            viewControls.AllowDeleteOption.Checked = _customEntities[0].view[1]._allowDelete;
            viewControls.AllowApprovalOption.Checked = _customEntities[0].view[1]._allowApproval;

            _customEntityViewsUIMap.PressCancelOnViewModal();

            _customEntityViewsUIMap.ValidateViewTable(_customEntities[0].view[0]._viewName, _customEntities[0].view[0]._description, true);

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            CustomEntitiesUtilities.CustomEntityView viewtoAdd = _customEntities[0].view[0];

            Assert.AreEqual(viewtoAdd._viewName, viewControls.ViewNameTxt.Text);
            Assert.AreEqual(viewtoAdd._description, viewControls.DescriptionTxt.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((MenuItems)viewtoAdd._menuid), viewControls.menulist.SelectedItem);
            Assert.AreEqual(viewtoAdd._menudescription, viewControls.menuDescriptionTxt.Text);
            Assert.AreEqual(viewtoAdd.addform._formName, viewControls.addformlistlist.SelectedItem);
            Assert.AreEqual(viewtoAdd.editform._formName, viewControls.editformlist.SelectedItem);
            Assert.AreEqual(viewtoAdd._allowDelete, viewControls.AllowDeleteOption.Checked);
            Assert.AreEqual(viewtoAdd._allowApproval, viewControls.AllowApprovalOption.Checked);

            _customEntityViewsUIMap.PressCancelOnViewModal();
        }
        #endregion

        #region Unsuccessfully edit duplicate view to custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyEditDuplicateViewToCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[1]._viewName);

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            viewControls.ViewNameTxt.Text = _customEntities[0].view[0]._viewName;
            viewControls.DescriptionTxt.Text = _customEntities[0].view[0]._description;
            viewControls.menulist.SelectedItem = EnumHelper.GetEnumDescription((MenuItems)_customEntities[0].view[0]._menuid);
            viewControls.menuDescriptionTxt.Text = _customEntities[0].view[0]._menudescription;
            viewControls.addformlistlist.SelectedItem = _customEntities[0].view[0].addform._formName;
            viewControls.editformlist.SelectedItem = _customEntities[0].view[0].editform._formName;
            viewControls.AllowDeleteOption.Checked = _customEntities[0].view[0]._allowDelete;
            viewControls.AllowApprovalOption.Checked = _customEntities[0].view[0]._allowApproval;

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nThe View name you have entered already exists.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
        }
        #endregion

        #region 36645 Successfully delete view to custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyDeleteViewToCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickDeleteFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickConfirmDeleteViewLink();

            _customEntityViewsUIMap.ValidateViewTable(_customEntities[0].view[0]._viewName, _customEntities[0].view[0]._description, false);

        }
        #endregion

        #region 38006 Unsuccessfully delete view to custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyDeleteViewToCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickDeleteFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickCancelDeleteViewLink();

            _customEntityViewsUIMap.ValidateViewTable(_customEntities[0].view[0]._viewName, _customEntities[0].view[0]._description, true);

        }
        #endregion

        #region 38004 Successfully sort views grid in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight View General Details"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullySortViewsTableinCustomEntity_UITest()
        {
            _sharedMethods.RestoreDefaultSortingOrder("gridViews", _executingProduct);

            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            HtmlHyperlink displayNameLink = _customEntityViewsUIMap.UIGreenLightAdministrationViewsWindow.UIGreenLightAdministrationViewsDocument.UITbl_gridViewsTable.UIViewNameHyperlink;
            _customEntityViewsUIMap.ClickTableHeader(displayNameLink);
            _customEntityViewsUIMap.VerifyCorrectSortingOrderForTable(SortViewsByColumn.ViewName, EnumHelper.TableSortOrder.DESC, _customEntities[0].entityId, _executingProduct);
            _customEntityViewsUIMap.ClickTableHeader(displayNameLink);
            _customEntityViewsUIMap.VerifyCorrectSortingOrderForTable(SortViewsByColumn.ViewName, EnumHelper.TableSortOrder.ASC, _customEntities[0].entityId, _executingProduct);

            //Sort by description
            displayNameLink = _customEntityViewsUIMap.UIGreenLightAdministrationViewsWindow.UIGreenLightAdministrationViewsDocument.UITbl_gridViewsTable.UIDescriptionHyperlink;
            _customEntityViewsUIMap.ClickTableHeader(displayNameLink);
            _customEntityViewsUIMap.VerifyCorrectSortingOrderForTable(SortViewsByColumn.Description, EnumHelper.TableSortOrder.ASC, _customEntities[0].entityId, _executingProduct);
            _customEntityViewsUIMap.ClickTableHeader(displayNameLink);
            _customEntityViewsUIMap.VerifyCorrectSortingOrderForTable(SortViewsByColumn.Description, EnumHelper.TableSortOrder.DESC, _customEntities[0].entityId, _executingProduct);



        }
        #endregion
        #endregion

        #region Sorting
        #region 38028 successfully add Sort column and sort direction to view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddSortColumnAndSortDirectionToViewInCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, EmptyDropDown);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EmptyDropDown);
            Assert.AreEqual(viewControls.SortColumnlist.Enabled, true);
            Assert.AreEqual(viewControls.SortDirectionlist.Enabled, false);

            viewControls.SortColumnlist.SelectedItem = _customEntities[0].attribute[0].DisplayName;
            Assert.AreEqual(viewControls.SortDirectionlist.Enabled, true);
            viewControls.SortDirectionlist.SelectedItem = EnumHelper.GetEnumDescription(SortDirection.Ascending);

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, _customEntities[0].attribute[0].DisplayName);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EnumHelper.GetEnumDescription(SortDirection.Ascending));

            viewControls.SortDirectionlist.SelectedItem = EnumHelper.GetEnumDescription(SortDirection.Descending);

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, _customEntities[0].attribute[0].DisplayName);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EnumHelper.GetEnumDescription(SortDirection.Descending));
        }
        #endregion

        #region unsuccessfully add Sort column and sort direction to view in custom entity where cancel is clicked
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddSortColumnAndDirectionToViewInCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, EmptyDropDown);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EmptyDropDown);
            Assert.AreEqual(viewControls.SortColumnlist.Enabled, true);
            Assert.AreEqual(viewControls.SortDirectionlist.Enabled, false);

            viewControls.SortColumnlist.SelectedItem = _customEntities[0].attribute[0].DisplayName;
            Assert.AreEqual(viewControls.SortDirectionlist.Enabled, true);
            viewControls.SortDirectionlist.SelectedItem = EnumHelper.GetEnumDescription(SortDirection.Ascending);

            _customEntityViewsUIMap.PressCancelOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, EmptyDropDown);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EmptyDropDown);
        }
        #endregion

        #region 38026 unsuccessfully add Sort column to view in custom entity where sort direction is Missing
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddSortColumnToViewInCustomEntityWhereSortDirectionIsMissing_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            viewControls.SortColumnlist.SelectedItem = _customEntities[0].attribute[0].DisplayName;

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Sort direction.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
        }
        #endregion

        #region successfully edit sort column to view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyEditSortColumnAndSortDirectionToViewInCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, _customEntities[0].attribute[0].DisplayName);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EnumHelper.GetEnumDescription(SortDirection.Ascending));
            Assert.AreEqual(viewControls.SortColumnlist.Enabled, true);
            Assert.AreEqual(viewControls.SortDirectionlist.Enabled, true);

            viewControls.SortColumnlist.SelectedItem = _customEntities[0].attribute[1].DisplayName;
            viewControls.SortDirectionlist.SelectedItem = EnumHelper.GetEnumDescription(SortDirection.Descending);

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, _customEntities[0].attribute[1].DisplayName);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EnumHelper.GetEnumDescription(SortDirection.Descending));
        }
        #endregion

        #region 40962 unsuccessfully edit sort column to view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyEditSortColumnAndSortDirectionToViewInCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, _customEntities[0].attribute[0].DisplayName);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EnumHelper.GetEnumDescription(SortDirection.Ascending));
            Assert.AreEqual(viewControls.SortColumnlist.Enabled, true);
            Assert.AreEqual(viewControls.SortDirectionlist.Enabled, true);

            viewControls.SortColumnlist.SelectedItem = _customEntities[0].attribute[1].DisplayName;
            viewControls.SortDirectionlist.SelectedItem = EnumHelper.GetEnumDescription(SortDirection.Descending);

            _customEntityViewsUIMap.PressCancelOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, _customEntities[0].attribute[0].DisplayName);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EnumHelper.GetEnumDescription(SortDirection.Ascending));
        }
        #endregion

        #region 40106 successfully remove sort column and direction to view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyRemoveSortColumnAndSortDirectionToViewInCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, _customEntities[0].attribute[0].DisplayName);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EnumHelper.GetEnumDescription(SortDirection.Ascending));
            Assert.AreEqual(viewControls.SortColumnlist.Enabled, true);
            Assert.AreEqual(viewControls.SortDirectionlist.Enabled, true);

            viewControls.SortColumnlist.SelectedItem = EmptyDropDown;

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();

            viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(viewControls.SortColumnlist.SelectedItem, EmptyDropDown);
            Assert.AreEqual(viewControls.SortDirectionlist.Enabled, false);
            Assert.AreEqual(viewControls.SortDirectionlist.SelectedItem, EmptyDropDown);
        }
        #endregion

        #region 38010 successfully verify Sorting Tab is Disabled when adding view in custom entity
        /// <summary>
        /// 38010 successfully verify Sorting Tab is Disabled when adding view in custom entity
        /// 38145 Successfully verify sorting tab is enabled when columns are selected
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifySortingColumnDisabledWhenAddingNewViewToCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickNewViewLink();

            _customEntityViewsUIMap.ValidateSortingTabDisabled();

            _customEntityViewsUIMap.ClickColumnsTab();

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            SelectAvailableFieldForColumn(_customEntities[0].attribute[0].FieldId.ToString());
            _customEntityViewsUIMap.ClickMoveColumnSelectionRight();

            _customEntityViewsUIMap.ValidateSortingTabEnabled();
        }
        #endregion
        #endregion

        #region Columns
        #region columns drag and drop
        #region Successfully add all basic attributes to selected fields
        /// <summary>
        /// Drags all basic attributes to the selected fields, saves and then verifies the results.
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddAllBasicAttributesToSelectedFields_UITest()
        {
            string selectedFields = string.Empty;
            List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields = ConstructExpectedAvailableFields(0);
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickNewViewLink();

            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.GeneralDetails));

            viewControls.ViewNameTxt.Text = "MyView";

            _customEntityViewsUIMap.ClickColumnsTab();

            // Filter through the list getting one of each type to drag
            Dictionary<FieldType, CustomEntitiesUtilities.CustomEntityAttribute> DistinctFieldTypeAttributes = new Dictionary<FieldType, CustomEntitiesUtilities.CustomEntityAttribute>();
            foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in _customEntities[0].attribute)
            {
                if (!DistinctFieldTypeAttributes.ContainsKey(attribute._fieldType))
                {
                    if (attribute._fieldType != FieldType.Relationship)
                    {
                        DistinctFieldTypeAttributes.Add(attribute._fieldType, attribute);
                    }
                }
            }

            selectedFields = "There are no columns selected.";
            foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in DistinctFieldTypeAttributes.Values)
            {
                if (attribute._fieldType != FieldType.Comment)
                {
                    DragBasicAttributeToColumnsArea(attribute.FieldId.ToString());
                    selectedFields = ColumnBuilder(selectedFields, attribute.DisplayName);
                    expectedAvailableFields.Remove(attribute);
                }
            }

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);

            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);

            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Verify
            _customEntityViewsUIMap.ClickEditFieldLink("MyView");

            Assert.AreEqual("MyView", viewControls.ViewNameTxt.Text);

            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
        }
        #endregion

        #region Successfully add N to One attributes to selected fields
        /// <summary>
        /// Successfully drag and drop n to one attributes from custom entities to chosen columns and verify
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddNToOneAttributesToSelectedFields_UITest()
        {
            string selectedFields = string.Empty;
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> columms tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #endregion

            List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields = ConstructExpectedAvailableFields(0);

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute nTo1Relationship = null;
            foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in _customEntities[0].attribute)
            {
                if (attribute._fieldType == FieldType.Relationship && attribute.DisplayName == "N : 1 Relationship Attribute to Custom Entity")
                {
                    nTo1Relationship = attribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                }
            }
            //Throw exception when attribute is NULL!! - can't be found within the list of attributes for CE.
            if (nTo1Relationship == null)
            {
                throw new Exception(String.Format("Can't find an attribute in {0} that is of type {1}", new object[] { _customEntities[0].entityName, FieldType.Relationship }));
            }

            foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in expectedAvailableFields)
            {
                if (attribute.DisplayName == nTo1Relationship.DisplayName)
                {
                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute expectedNToOneRelationship = attribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                    expectedNToOneRelationship._baseTableFields = ReadRelatedNToOneAttributes(_customEntities[0].entityId, nTo1Relationship.DisplayName);
                }
            }


            //Expand the attribute for n:1
            ExpandNToOneNodeForColumns(nTo1Relationship.FieldId.ToString());
            nTo1Relationship._isExpanded = true;
            //select 3 attibutes to move to columns
            for (int attributeindex = 0; attributeindex < 4; attributeindex++)
            {
                var ntoOneAttribute = _customEntities[1].attribute[attributeindex];
                if (!(ntoOneAttribute.SystemAttribute) && ntoOneAttribute._fieldType != FieldType.Comment && ntoOneAttribute._fieldType != FieldType.Relationship)
                {
                    SelectNTo1RelationshipForColumn(nTo1Relationship.FieldId.ToString(), ntoOneAttribute.FieldId.ToString());
                    _customEntityViewsUIMap.ClickMoveColumnSelectionRight();

                    selectedFields = ColumnBuilder(selectedFields, ntoOneAttribute.DisplayName, nTo1Relationship.DisplayName);
                    foreach (CustomEntitiesUtilities.CustomEntityAttribute currentAttribute in expectedAvailableFields)
                    {
                        if (currentAttribute.DisplayName == nTo1Relationship.DisplayName)
                        {
                            CustomEntitiesUtilities.CustomEntityNtoOneAttribute currentNTo1Relationship = currentAttribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                            foreach (CustomEntitiesUtilities.Field field in currentNTo1Relationship._baseTableFields)
                            {
                                if (field.FieldId == ntoOneAttribute.FieldId)
                                {
                                    currentNTo1Relationship._baseTableFields.Remove(field);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);

            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);

            #region save and return to verify columns
            _customEntityViewsUIMap.ClickSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            ExpandNToOneNodeForColumns(nTo1Relationship.FieldId.ToString());

            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            #endregion
        }
        #endregion

        #region 38018 unsuccessfully add columns for view in custom entity where cancel is pressed
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddColumnsForViewInCustomEntity_UITest()
        {
            string selectedFields = string.Empty;
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> columms tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #endregion

            #region add new columns and verify new columns
            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            for (int attributeindexer = 17; attributeindexer < _customEntities[0].attribute.Count; attributeindexer++)
            {
                if (_customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Comment && _customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Relationship)
                {
                    SelectAvailableFieldForColumn(_customEntities[0].attribute[attributeindexer].FieldId.ToString());
                    _customEntityViewsUIMap.ClickMoveColumnSelectionRight();
                    selectedFields = ColumnBuilder(selectedFields, _customEntities[0].attribute[attributeindexer].DisplayName);
                }
            }
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            #endregion

            #region cancel save and return to verify no columns saved
            _customEntityViewsUIMap.PressCancelOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            selectedFields = "There are no columns selected.";
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            #endregion

        }
        #endregion

        #region 40179 - Successfully delete attribute from view column
        /// <summary>
        /// 40179 - Successfully delete attribute from view column
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyDeleteColumnsFromViewInCustomEntity_UITest()
        {
            string selectedFields = string.Empty;
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields = ConstructExpectedAvailableFields(17);

            #region navigate to entity -> view -> columms tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #endregion

            #region remove all columns and verify no coloumns
            for (int attributeindexer = 0; attributeindexer < 17; attributeindexer++)
            {
                if (_customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Comment)
                {
                    RemoveBasicAttributeFromSelectedColumn(_customEntities[0].attribute[attributeindexer].FieldId.ToString());
                    expectedAvailableFields.Add((_customEntities[0].attribute[attributeindexer]));
                }
            }

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(ColumnBuilder(selectedFields, null), viewcontrols.columnsDropPane.InnerText);
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            #endregion

            #region save the view, edit it and verify columns
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));

            Assert.AreEqual((ColumnBuilder(selectedFields, null)), viewcontrols.columnsDropPane.InnerText);
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            #endregion
        }
        #endregion

        #region successfully edit columns for view in custom entity
        /// <summary>
        /// 40180 - Successfully add attribute immediately after deleting attribute from view columns
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyEditColumnsForViewInCustomEntity_UITest()
        {
            string selectedFields = string.Empty;
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields = ConstructExpectedAvailableFields(17);

            #region navigate to entity -> view -> columms tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #endregion

            #region remove all columns and verify no coloumns
            for (int attributeindexer = 0; attributeindexer < 17; attributeindexer++)
            {
                if (_customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Comment)
                {
                    RemoveBasicAttributeFromSelectedColumn(_customEntities[0].attribute[attributeindexer].FieldId.ToString());
                    expectedAvailableFields.Add((_customEntities[0].attribute[attributeindexer]));
                }
            }

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(ColumnBuilder(selectedFields, null), viewcontrols.columnsDropPane.InnerText);
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            #endregion

            #region add new columns and verify new columns
            selectedFields = string.Empty;
            for (int attributeindexer = 17; attributeindexer < 22; attributeindexer++)
            {
                if (_customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Comment && _customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Relationship)
                {
                    DragBasicAttributeToColumnsArea(_customEntities[0].attribute[attributeindexer].FieldId.ToString());
                    ///build a string of controls as they are addedd to drop area
                    selectedFields = ColumnBuilder(selectedFields, _customEntities[0].attribute[attributeindexer].DisplayName);
                    expectedAvailableFields.Remove(_customEntities[0].attribute[attributeindexer]);
                }
            }

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));

            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            #endregion
        }
        #endregion
        #endregion

        #region columns buttons
        #region 38275 38276 successfully verify move up and move down buttons work correctly in columns tab
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifyMoveUpAndMoveDownButtonsWorkCorrectlyInColumnsTabOfViewInCustomEntity_UITest()
        {
            string selectedFields = string.Empty;
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> columms tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #endregion

            #region define column list
            List<Auto_Tests.Tools.CustomEntitiesUtilities.CustomEntityAttribute> ColumnSet = new List<Auto_Tests.Tools.CustomEntitiesUtilities.CustomEntityAttribute>();
            for (int attributeindexer = 0; attributeindexer < 17; attributeindexer++)
            {
                if (_customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Comment)
                {
                    ColumnSet.Add(_customEntities[0].attribute[attributeindexer]);
                }
            }
            #endregion

            #region select column/ number of places to move down and press move
            int columnindextomovedown = 2;
            int destinationindex = 7;
            SelectField(ColumnSet[columnindextomovedown].FieldId.ToString());
            ///move selected field a number of times
            for (int columnindex = columnindextomovedown; columnindex <= destinationindex; columnindex++)
            {
                _customEntityViewsUIMap.ClickMoveColumnSelectionDown();
                MoveObjectWithinList(ColumnSet, columnindex, columnindex + 1);
            }
            #endregion

            #region select column/ number of places to move up and press move
            int columnindextomoveup = 9;
            destinationindex = 1;
            SelectField(ColumnSet[columnindextomoveup].FieldId.ToString());
            for (int columnindex = columnindextomoveup; columnindex >= destinationindex; columnindex--)
            {
                _customEntityViewsUIMap.ClickMoveColumnSelectionUp();
                MoveObjectWithinList(ColumnSet, columnindex, columnindex - 1);
            }
            #endregion

            #region validate columns ordering
            for (int listindex = 0; listindex < ColumnSet.Count; listindex++)
            {
                selectedFields = ColumnBuilder(selectedFields, ColumnSet[listindex].DisplayName);
            }

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));

            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            #endregion

            #region save order and validate order saved
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            #endregion
        }
        #endregion

        #region 38042 successfully verify move Right button work correctly in columns tab
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifyMoveRightButtonWorkCorrectlyInColumnsTabOfViewInCustomEntity_UITest()
        {
            string selectedFields = string.Empty;
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> columms tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #endregion

            #region add new columns and verify new columns
            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            for (int attributeindexer = 0; attributeindexer < 17; attributeindexer++)
            {
                if (_customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Comment && _customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Relationship)
                {
                    SelectAvailableFieldForColumn(_customEntities[0].attribute[attributeindexer].FieldId.ToString());
                    _customEntityViewsUIMap.ClickMoveColumnSelectionRight();
                    selectedFields = ColumnBuilder(selectedFields, _customEntities[0].attribute[attributeindexer].DisplayName);
                }
            }
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);

            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));

            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            #endregion
        }
        #endregion

        #region 38271 successfully verify remove selection button work correctly in columns tab, 38273 Successfully verify remove all button works correctly in columns tab
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestCategory("Debug"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifyRemoveSelectionAndRemoveAllButtonsWorkCorrectlyInColumnsTabOfViewInCustomEntity_UITest()
        {

            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            string selectedColumns = string.Empty;
            string availableFieldsPane = string.Empty;

            #region navigate to entity -> view -> columms tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #endregion

            #region define available fields and selected column lists
            List<Auto_Tests.Tools.CustomEntitiesUtilities.CustomEntityAttribute> ColumnSet = new List<Auto_Tests.Tools.CustomEntitiesUtilities.CustomEntityAttribute>();
            for (int attributeindexer = 0; attributeindexer < 17; attributeindexer++)
            {
                if (_customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Comment)
                {
                    ColumnSet.Add(_customEntities[0].attribute[attributeindexer]);
                }
            }
            List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields = ConstructExpectedAvailableFields(17);
            #endregion

            #region remove columns from droppane
            for (int columnindextoremove = 10; columnindextoremove < ColumnSet.Count; columnindextoremove++)
            {
                SelectField(ColumnSet[columnindextoremove].FieldId.ToString());
                _customEntityViewsUIMap.ClickRemoveColumnSelection();
                expectedAvailableFields.Add(ColumnSet[columnindextoremove]);
                ColumnSet.Remove(ColumnSet[columnindextoremove]);
            }
            #endregion

            #region validate Avalaible fields and Selected columns
            for (int listindex = 0; listindex < ColumnSet.Count; listindex++)
            {
                selectedColumns = ColumnBuilder(selectedColumns, ColumnSet[listindex].DisplayName);
            }

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));

            Assert.AreEqual(selectedColumns, viewcontrols.columnsDropPane.InnerText);

            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            #endregion

            #region save and validate Available fields and Chosen columns
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            Assert.AreEqual(selectedColumns, viewcontrols.columnsDropPane.InnerText);
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            #endregion

            #region remove all columns from droppane
            _customEntityViewsUIMap.ClickRemoveAllColumns();

            foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in ColumnSet)
            {
                expectedAvailableFields.Add(attribute);
            }

            _sharedMethods.SetFocusOnControlAndPressEnter(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UISaveButton1);

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            selectedColumns = ColumnBuilder(selectedColumns, null);
            Assert.AreEqual(selectedColumns, viewcontrols.columnsDropPane.InnerText);
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            _sharedMethods.SetFocusOnControlAndPressEnter(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);
            #endregion
        }
        #endregion

        #region 40961 successfully verify sort tab is disabled upon removing all columns
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifySortTabIsDisabledUponDeletingAllColumnsForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> columms tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #endregion

            ///verify sort tab enabled
            _customEntityViewsUIMap.ValidateSortingTabEnabled();

            #region remove all columns and assert no columns
            _customEntityViewsUIMap.ClickRemoveAllColumns();

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));

            //   Assert.AreEqual(ColumnBuilder(), viewcontrols.dropPane.InnerText);
            #endregion

            ///verify sort tab disabled
            _customEntityViewsUIMap.ValidateSortingTabDisabled();
        }
        #endregion

        #region successfully verify sort column and sort direction is reset upon removing columns used
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifySortColumnAndDirectionResetUponRemovingColumnUsedForSortInViewOfCustomEntity_UITest()
        {
            ///insert data required for thiOftest
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> sort tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickSortingTab();
            #endregion

            ///validate set sort column and direction
            CustomEntityViewsControls sortcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));
            Assert.AreEqual(sortcontrols.SortColumnlist.SelectedItem, _customEntities[0].attribute[0].DisplayName);
            Assert.AreEqual(sortcontrols.SortDirectionlist.SelectedItem, EnumHelper.GetEnumDescription(SortDirection.Ascending));

            #region remove designated sort column from view
            _customEntityViewsUIMap.ClickColumnsTab();

            SelectField(_customEntities[0].attribute[0].FieldId.ToString());

            _customEntityViewsUIMap.ClickRemoveColumnSelection();
            #endregion

            ///verify sort column and direction reset
            _customEntityViewsUIMap.ClickSortingTab();

            sortcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Sorting));

            Assert.AreEqual(sortcontrols.SortColumnlist.SelectedItem, EmptyDropDown);
            Assert.AreEqual(sortcontrols.SortDirectionlist.SelectedItem, EmptyDropDown);
            Assert.AreEqual(sortcontrols.SortColumnlist.Enabled, true);
            Assert.AreEqual(sortcontrols.SortDirectionlist.Enabled, false);

        }
        #endregion

        #region 41481 - Successfully add base table fields to selected fields, 42046 - Successfully remove base table fields from view columns
        /// <summary>
        /// 41481 - Successfully add base table fields from n to one relationship to chosen columns and verify
        /// 42046 - Successfully remove base table fields from view columns
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddBaseTableFieldsAsColumnsForViewInCustomEntity_UITest()
        {
            string selectedFields = string.Empty;
            ImportDataToTestingDatabase(testContextInstance.TestName);

            List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields = ConstructExpectedAvailableFields(0);
            List<CustomEntitiesUtilities.Field> expectedSelectedFields = new List<Auto_Tests.Tools.CustomEntitiesUtilities.Field>();

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            #region Expand n:1 relationship to base table and add base fields to selected fields
            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attribute = _customEntities[0].attribute[33] as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;

            //Expand the attribute for n:1
            ExpandNToOneNodeForColumns(attribute.FieldId.ToString());
            attribute._isExpanded = true;

            //Move three base fields to the selected fields area 
            for (int index = 0; index < 3; index++)
            {
                SelectNTo1RelationshipForColumn(attribute.FieldId.ToString(), attribute._baseTableFields[index].FieldId.ToString());
                if (!attribute._baseTableFields[index]._isForeignKey)
                {
                    _customEntityViewsUIMap.ClickMoveColumnSelectionRight();
                    //Add the field you moved to your expected selected fields 
                    expectedSelectedFields.Add(attribute._baseTableFields[index]);
                    selectedFields = ColumnBuilder(selectedFields, attribute._baseTableFields[index].DisplayName, _customEntities[0].attribute[33].DisplayName);
                    //Remove the field you moved from your expected available fields
                    foreach (CustomEntitiesUtilities.CustomEntityAttribute currentAttribute in expectedAvailableFields)
                    {
                        if (currentAttribute._attributeid == _customEntities[0].attribute[33]._attributeid)
                        {
                            CustomEntitiesUtilities.CustomEntityNtoOneAttribute currentNTo1Relationship = currentAttribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                            foreach (CustomEntitiesUtilities.Field field in currentNTo1Relationship._baseTableFields)
                            {
                                if (field.FieldId == attribute._baseTableFields[index].FieldId)
                                {
                                    currentNTo1Relationship._baseTableFields.Remove(field);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            //Save the view
            _customEntityViewsUIMap.ClickSaveOnViewModal();
            #endregion

            #region Navigate back to the view, expand the n:1 relationship and validate both available and selected fields
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            ExpandNToOneNodeForColumns(attribute.FieldId.ToString());

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);

            //Verify the available fields section
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            #endregion

            #region Remove all selected fields
            _customEntityViewsUIMap.ClickRemoveAllColumns();

            foreach (CustomEntitiesUtilities.CustomEntityAttribute currentAttribute in expectedAvailableFields)
            {
                if (currentAttribute._attributeid == _customEntities[0].attribute[33]._attributeid)
                {
                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute currentNTo1Relationship = currentAttribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                    foreach (CustomEntitiesUtilities.Field field in expectedSelectedFields)
                    {
                        currentNTo1Relationship._baseTableFields.Add(field);
                    }
                }
            }

            _sharedMethods.SetFocusOnControlAndPressEnter(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UISaveButton1);

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            //Expand the attribute for n:1
            ExpandNToOneNodeForColumns(attribute.FieldId.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            selectedFields = ColumnBuilder(selectedFields);
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            _sharedMethods.SetFocusOnControlAndPressEnter(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);
            attribute._isExpanded = false;
            #endregion
        }
        #endregion

        #region 41480 - Successfully add UDF to selected fields, 42054 - Successfully remove UDF from views columns
        /// <summary>
        /// 41480 - Successfully add UDF to selected fields
        /// 42054 - Successfully remove UDF from views columns
        /// Prerequisite at least 2 UDFs need to exist in the system for the n:1 relationship with base table we use, currently with employees
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddUDFsAsColumnsForViewInCustomEntity_UITest()
        {
            string selectedFields = string.Empty;
            ImportDataToTestingDatabase(testContextInstance.TestName);

            List<CustomEntitiesUtilities.CustomEntityAttribute> expectedAvailableFields = ConstructExpectedAvailableFields(0);
            List<CustomEntitiesUtilities.Field> expectedSelectedFields = new List<Auto_Tests.Tools.CustomEntitiesUtilities.Field>();

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #region Expand n:1 relationship to base table and add UDFs to selected fields
            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attribute = _customEntities[0].attribute[33] as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;

            //Expand the attribute for n:1
            ExpandNToOneNodeForColumns(attribute.FieldId.ToString());
            //Expand User defined field folder of the n:1 relationship
            ExpandUDFNodeInAvailableFieldsForColumns(attribute.FieldId.ToString(), attribute._udfFolder.FieldId.ToString());

            for (int index = attribute._UDFFields.Count - 1; index > 0; index--)
            {
                //Select UDF and move it 
                SelectFieldForColumn(string.Format("k{0}_g{1}_n{2}", new object[] { attribute.FieldId.ToString(), attribute._udfFolder.FieldId.ToString(), attribute._UDFFields[index].FieldId.ToString() }));
                _customEntityViewsUIMap.ClickMoveColumnSelectionRight();
                //Add the field you moved to your expected selected fields 
                expectedSelectedFields.Add(attribute._UDFFields[index]);
                selectedFields = ColumnBuilder(selectedFields, attribute._UDFFields[index].DisplayName, attribute.DisplayName, userDefinedField: "User Defined Fields");
                //Remove the field from your expected available fields
                foreach (CustomEntitiesUtilities.CustomEntityAttribute currentAttribute in expectedAvailableFields)
                {
                    if (currentAttribute._attributeid == _customEntities[0].attribute[33]._attributeid)
                    {
                        CustomEntitiesUtilities.CustomEntityNtoOneAttribute currentNTo1Relationship = currentAttribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                        currentNTo1Relationship._isExpanded = true;
                        currentNTo1Relationship._udfFolder._isExpanded = true;
                        foreach (CustomEntitiesUtilities.Field udfField in currentNTo1Relationship._UDFFields)
                        {
                            if (udfField.FieldId == attribute._UDFFields[index].FieldId)
                            {
                                currentNTo1Relationship._UDFFields.Remove(udfField);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            //Save the view
            _customEntityViewsUIMap.ClickSaveOnViewModal();
            #endregion

            #region Navigate back to the view, expand the n:1 relationship and validate both available and selected fields
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            ExpandNToOneNodeForColumns(attribute.FieldId.ToString());
            ExpandUDFNodeInAvailableFieldsForColumns(attribute.FieldId.ToString(), attribute._udfFolder.FieldId.ToString());
            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);

            //Verify the available fields section
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            #endregion

            #region Remove all selected fields
            _customEntityViewsUIMap.ClickRemoveAllColumns();

            foreach (CustomEntitiesUtilities.CustomEntityAttribute currentAttribute in expectedAvailableFields)
            {
                if (currentAttribute._attributeid == _customEntities[0].attribute[33]._attributeid)
                {
                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute currentNTo1Relationship = currentAttribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                    foreach (CustomEntitiesUtilities.Field field in expectedSelectedFields)
                    {
                        currentNTo1Relationship._UDFFields.Add(field);
                    }
                }
            }

            _sharedMethods.SetFocusOnControlAndPressEnter(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UISaveButton1);

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            //Expand the attribute for n:1
            ExpandNToOneNodeForColumns(attribute.FieldId.ToString());
            ExpandUDFNodeInAvailableFieldsForColumns(attribute.FieldId.ToString(), attribute._udfFolder.FieldId.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            selectedFields = ColumnBuilder(selectedFields, null);
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
            _customEntityViewsUIMap.ValidateAvailableFields(expectedAvailableFields);
            _sharedMethods.SetFocusOnControlAndPressEnter(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);
            #endregion
        }
        #endregion

        #region 42109 - Successfully set view columns by selecting fields from 2 levels deep
        /// <summary>
        /// 42109 - Successfully set view columns by selecting fields from 2 levels deep
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddTwoLevelsDeepFieldsAsColumnsForViewInCustomEntity_UITest()
        {
            string selectedFields = string.Empty;
            ImportDataToTestingDatabase(testContextInstance.TestName);

            List<CustomEntitiesUtilities.Field> expectedSelectedFields = new List<Auto_Tests.Tools.CustomEntitiesUtilities.Field>();

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute nToOneRelationship = _customEntities[0].attribute[35] as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;

            //Read the system attributes for the custom entity that the n:1 relationship in the previous line has and add them as its attributes
            List<CustomEntitiesUtilities.CustomEntityAttribute> attributeList = new List<CustomEntitiesUtilities.CustomEntityAttribute>();
            attributeList.AddRange(_customEntities[1].attribute);
            attributeList.AddRange(ReadSystemAttributesFieldIDs(_customEntities[1].entityId));

            //Expand the attribute for n:1
            ExpandNToOneNodeForColumns(nToOneRelationship.FieldId.ToString());

            foreach (CustomEntitiesUtilities.CustomEntityAttribute firstLevelField in attributeList)
            {
                if (firstLevelField.DisplayName == "Created By")
                {
                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute createdByRelationship = firstLevelField as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                    ExpandSecondLevelRelationship(nToOneRelationship.FieldId.ToString(), firstLevelField.FieldId.ToString());
                    ConstructNToOneRelationshipFields(createdByRelationship);
                    for (int index = 3; index > 0; index--)
                    {
                        if (!createdByRelationship._baseTableFields[index]._isForeignKey)
                        {
                            SelectFieldForColumn(string.Format("k{0}_k{1}_n{2}", new object[] { nToOneRelationship.FieldId.ToString(), createdByRelationship.FieldId.ToString(), createdByRelationship._baseTableFields[index].FieldId.ToString() }));
                            _customEntityViewsUIMap.ClickMoveColumnSelectionRight();
                            selectedFields = ColumnBuilder(selectedFields, createdByRelationship._baseTableFields[index].DisplayName, nToOneRelationship.DisplayName, firstLevelField.DisplayName);
                        }
                    }
                    break;
                }
            }

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);

            //Save the view
            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Verify
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(selectedFields, viewcontrols.columnsDropPane.InnerText);
        }
        #endregion
        #endregion

        #region Attributes manipulation
        #region 41491 - Successfully edit attribute when in use as Column for Greenlight view
        /// <summary>
        /// 41491 - Custom Entities Views : Successfully Edit attribute when in use as Column for GreenLight View.
        /// Edits the attribute whilst it currently in use via selected fields and ensures that it is reflected in
        /// the admin side of the view.
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestCategory("Debug"), TestMethod]
        public void CustomEntityViewsSuccessfullyEditAttributeWhenInUseAsColumnForGreenlightView_UITest()
        {
            //Set up test Variables
            string expectedSelectedFields = string.Empty;
            string editedAttributeName = "MyAttribute";

            CustomEntityAttributesUIMap attributesUIMap = new CustomEntityAttributesUIMap();

            ImportDataToTestingDatabase(testContextInstance.TestName);
            CustomEntitiesUtilities.CustomEntityAttribute attributeToEdit = _customEntities[0].attribute.Find(ceAttribute => ceAttribute._fieldType == FieldType.Text &&
                ceAttribute._format == (short)Format.Single_line);

            //Navigate to the CE page
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            //Click views
            _customEntityViewsUIMap.ClickViewsLink();

            //Click edit against the view
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            //Click the columns tab
            _customEntityViewsUIMap.ClickColumnsTab();

            //Assert selected fields
            for (int attributeIndexer = 0; attributeIndexer < 17; attributeIndexer++)
            {
                CustomEntitiesUtilities.CustomEntityAttribute attribute = _customEntities[0].attribute[attributeIndexer];
                if (attribute._fieldType != FieldType.Comment && attribute._fieldType != FieldType.Relationship && attribute.GetType() != typeof(UserDefinedFields) && attribute.GetType() != typeof(UserDefinedFieldTypeList))
                {
                    expectedSelectedFields = ColumnBuilder(expectedSelectedFields, attribute.DisplayName);
                }
            }
            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(expectedSelectedFields, viewcontrols.columnsDropPane.InnerText);

            //Close the view modal
            Keyboard.SendKeys("{Esc}");

            //Click the attributes link
            attributesUIMap.ClickAttributesLink();

            _customEntityViewsUIMap.FilterAttributesParams.UIDisplayNameEditText = "Standard Single Text";
            _customEntityViewsUIMap.FilterAttributes();
            Keyboard.SendKeys("{ENTER}");

            //Click edit against the attribute the attribue
            attributesUIMap.ClickEditFieldLink(attributeToEdit.DisplayName);

            //Find all the controls for the Attributes panel
            CustomEntityAttributeText AttributesUIControls = new CustomEntityAttributeText(attributesUIMap);

            //Update the displayname text to be the value of edited attribute name and save
            AttributesUIControls.DisplayNameTxt.Text = editedAttributeName;
            Keyboard.SendKeys("{ENTER}");

            //Click views link
            _customEntityViewsUIMap.ClickViewsLink();

            //Click edit against the view
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            //Navigate to the columns tab
            _customEntityViewsUIMap.ClickColumnsTab();

            ////Assert selected fields
            expectedSelectedFields = string.Empty;
            for (int attributeIndexer = 0; attributeIndexer < 17; attributeIndexer++)
            {
                CustomEntitiesUtilities.CustomEntityAttribute attribute = _customEntities[0].attribute[attributeIndexer];
                if (attribute._fieldType != FieldType.Comment && attribute._fieldType != FieldType.Relationship && attribute.GetType() != typeof(UserDefinedFields) && attribute.GetType() != typeof(UserDefinedFieldTypeList))
                {
                    if (attributeToEdit._attributeid == attribute._attributeid)
                    {
                        expectedSelectedFields = ColumnBuilder(expectedSelectedFields, editedAttributeName);
                    }
                    else
                    {
                        expectedSelectedFields = ColumnBuilder(expectedSelectedFields, attribute.DisplayName);
                    }
                }
            }
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(expectedSelectedFields, viewcontrols.columnsDropPane.InnerText);
        }
        #endregion

        #region 41492 Custom entity views : Successfully delete attribute when in use as Column for Greenlight view
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestCategory("Debug"), TestMethod]
        public void CustomEntityViewsSuccessfullyDeleteAttributeWhenInUseAsColumnForViewInCustomEntity_UITest()
        {
            _sharedMethods.RestoreDefaultSortingOrder("gridAttributes", _executingProduct);

            string expectedSelectedFields = string.Empty;

            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            //Get a sub list of all basic attributes

            List<CustomEntitiesUtilities.CustomEntityAttribute> ExpectedAvailableFields = ConstructExpectedAvailableFields(17);

            _customEntityViewsUIMap.ValidateAvailableFields(ExpectedAvailableFields);

            //Assert selected fields
            for (int attributeIndexer = 0; attributeIndexer < 17; attributeIndexer++)
            {
                CustomEntitiesUtilities.CustomEntityAttribute attribute = _customEntities[0].attribute[attributeIndexer];
                if (attribute._fieldType != FieldType.Comment && attribute._fieldType != FieldType.Relationship && attribute.GetType() != typeof(UserDefinedFields) && attribute.GetType() != typeof(UserDefinedFieldTypeList))
                {
                    expectedSelectedFields = ColumnBuilder(expectedSelectedFields, attribute.DisplayName);
                }
            }

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(expectedSelectedFields, viewcontrols.columnsDropPane.InnerText);

            Keyboard.SendKeys("{Esc}");
            CustomEntityAttributesUIMap attributesUIMap = new CustomEntityAttributesUIMap();
            attributesUIMap.ClickAttributesLink();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToDelete = _customEntities[0].attribute[0];
            _customEntityViewsUIMap.FilterAttributesParams.UIDisplayNameEditText = attributeToDelete.DisplayName;
            _customEntityViewsUIMap.FilterAttributes();

            Keyboard.SendKeys("{Enter}");

            attributesUIMap.ClickDeleteFieldLink(attributeToDelete.DisplayName);
            attributesUIMap.PressOKToConfirmAttributeDeletion();
            Playback.Wait(2000);
            //Validate deletion
            attributesUIMap.ValidateAttributeDeletion(attributeToDelete.DisplayName);
            //Confirm deletion

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();

            _customEntityViewsUIMap.ValidateAvailableFields(ExpectedAvailableFields);

            //Assert selected fields
            expectedSelectedFields = string.Empty;
            //The attribute that gets deleted is the first one in customEntities[0].attribute list so we start the for loop for the second position
            for (int attributeIndexer = 1; attributeIndexer < 17; attributeIndexer++)
            {
                CustomEntitiesUtilities.CustomEntityAttribute attribute = _customEntities[0].attribute[attributeIndexer];
                if (attribute._fieldType != FieldType.Comment && attribute._fieldType != FieldType.Relationship && attribute.GetType() != typeof(UserDefinedFields) && attribute.GetType() != typeof(UserDefinedFieldTypeList))
                {
                    expectedSelectedFields = ColumnBuilder(expectedSelectedFields, attribute.DisplayName);
                }
            }
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Columns));
            Assert.AreEqual(expectedSelectedFields, viewcontrols.columnsDropPane.InnerText);
        }
        #endregion

        #region 41477 Custom entity views : Successfully update attribute in chosen columns after sorting order is set
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyUpdateAttributeInChosenColumnsAfterSortingOrderIsSet_UITest()
        {
            string expectedSelectedFields = string.Empty;

            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            #region navigate to entity -> view -> columms tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickColumnsTab();
            #endregion
        }
        #endregion

        #region 42055 UnSuccessfully Delete UDF when in use as Column for Greenlight views
        /// <summary>
        /// Custom entity views successfully verify that a udf attribute cannot be deleleted when
        /// its in use by a column.
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyDeleteUDFAttributeWhenInUseAsColumn_UITest()
        {
            #region Setup test
            UserDefinedFieldsUIMap UDFUIMap = new UserDefinedFieldsUIMap();
            ImportDataToTestingDatabase(testContextInstance.TestName);
            #endregion

            #region Verify that the UDF exists for the test
            //_sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminuserdefined.aspx");

            //HtmlTable htmlTable = (HtmlTable)UDFUIMap.UIUserDefinedFieldsWinWindow.UIUserDefinedFieldsDocument.UITbl_gridFieldsTable;
            //htmlTable.WaitForControlReady();
            //Assert.IsTrue(GridHelpers.FindRowInGridForId(htmlTable, htmlTable.Rows, _userDefinedFields[0].DisplayName) > 0, _userDefinedFields[0].DisplayName + " does not exist!");

            #endregion

            #region navigate to entity -> view -> Columns tab
            //_sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            //_customEntityViewsUIMap.ClickViewsLink();

            //_customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            //_customEntityViewsUIMap.ClickColumnsTab();
            #endregion

            //ExpandNToOneNodeForColumns(_customEntities[0].attribute[33].FieldId.ToString());

            //ExpandUDFNodeInAvailableFieldsForColumns(_customEntities[0].attribute[33].FieldId.ToString(), "972ac42d-6646-4efc-9323-35c2c9f95b62");

            //SelectUDFFieldForColumn(_customEntities[0].attribute[33].FieldId.ToString(), "972ac42d-6646-4efc-9323-35c2c9f95b62", _userDefinedFields[0].FieldId.ToString());


            //_customEntityViewsUIMap.ClickMoveColumnSelectionRight();

            #region Save the view
            //Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;

            //_customEntityViewsUIMap.ClickSaveOnViewModal();


            #endregion

            #region Attempt to delete the UDF attribute and verify it can't be deleted
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/adminuserdefined.aspx");


            UDFUIMap.ClickDeleteUdfField(_sharedMethods.browserWindow, _userDefinedFields[0].DisplayName);

            UDFUIMap.ValidateModalMessageExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.UDFCannotBeDeleted, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            UDFUIMap.ValidateModalMessage();

            //UDFUIMap.ValidateCannotDeleteDeleteUDFWhenInUseByViewModalMessage();
            #endregion

            #region Close the error modal window
            //Keyboard.SendKeys("{Enter}");
            UDFUIMap.PressCloseButtonOnModalMessage();
            #endregion
        }
        #endregion
        #endregion
        #endregion

        #region Filters
        #region filters drag and drop
        #region Add filters
        #region standard text
        #region 39558 successfully add filter on attribute of type standard text for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeStandardTextForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[0];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as like and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Like);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int stringfiltercount = 5;
            ///loop through filters for standard text and create all
            for (int filterindexer = 0; filterindexer < stringfiltercount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueTwo; }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 0; filterindexer < stringfiltercount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());

                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[0].DisplayName, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                /// click edit grid entry to validate its individual properties
                ClickEditFilter(copy, filtertosave._fieldid.ToString());

                //refresh filter controls to validate each set property
                viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                Assert.AreEqual(EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType), viewcontrols.filterCriteriaOption.SelectedItem);
                if (filtertosave._valueOne != string.Empty) { Assert.AreEqual(filtertosave._valueOne, viewcontrols.textFilterValue1Txt.Text); }
                if (filtertosave._valueTwo != string.Empty) { Assert.AreEqual(filtertosave._valueTwo, viewcontrols.textFilterValue2Txt.Text); }
                numberoofcopies++;
                _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            }
            #endregion
        }
        #endregion
        #endregion

        #region Large text
        #region 41451 successfully add filter on attribute of type large text for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeLargeTextForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            for (int filterindexer = 0; filterindexer < 5; filterindexer++)
            {
                _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[8].FieldId;
            }
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[0];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as like and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Like);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int stringfiltercount = 5;
            ///loop through filters for standard text and create all
            for (int filterindexer = 0; filterindexer < stringfiltercount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueTwo; }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 0; filterindexer < stringfiltercount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());

                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[8].DisplayName, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                /// click edit grid entry to validate its individual properties
                ClickEditFilter(copy, filtertosave._fieldid.ToString());

                //refresh filter controls to validate each set property
                viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                Assert.AreEqual(EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType), viewcontrols.filterCriteriaOption.SelectedItem);
                if (filtertosave._valueOne != string.Empty) { Assert.AreEqual(filtertosave._valueOne, viewcontrols.textFilterValue1Txt.Text); }
                if (filtertosave._valueTwo != string.Empty) { Assert.AreEqual(filtertosave._valueTwo, viewcontrols.textFilterValue2Txt.Text); }
                numberoofcopies++;
                _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            }
            #endregion
        }
        #endregion
        #endregion

        #region Formatted Large text
        #region successfully add filter on attribute of type Formatted large text for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeFormattedLargeTextForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            for (int filterindexer = 3; filterindexer < 5; filterindexer++)
            {
                _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[9].FieldId;
            }
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[0];
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int stringfiltercount = 5;
            ///loop through filters for standard text and create all
            for (int filterindexer = 3; filterindexer < stringfiltercount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueTwo; }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 3; filterindexer < stringfiltercount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());

                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[9].DisplayName, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                /// click edit grid entry to validate its individual properties
                ClickEditFilter(copy, filtertosave._fieldid.ToString());

                //refresh filter controls to validate each set property
                viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                Assert.AreEqual(EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType), viewcontrols.filterCriteriaOption.SelectedItem);
                if (filtertosave._valueOne != string.Empty) { Assert.AreEqual(filtertosave._valueOne, viewcontrols.textFilterValue1Txt.Text); }
                if (filtertosave._valueTwo != string.Empty) { Assert.AreEqual(filtertosave._valueTwo, viewcontrols.textFilterValue2Txt.Text); }
                numberoofcopies++;
                _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            }
            #endregion
        }
        #endregion
        #endregion

        #region number
        #region 41449 successfully add filter on attribute of type number for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeNumberForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[5];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than or equal to and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than or and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1 + ViewModalMessages.EmptyFieldForNumber2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 7;
            ///loop through filters for number and create all
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo; }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());

                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[2].DisplayName, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                /// click edit grid entry to validate its individual properties
                ClickEditFilter(copy, filtertosave._fieldid.ToString());

                //refresh filter controls to validate each set property
                viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                Assert.AreEqual(conditiontype, viewcontrols.filterCriteriaOption.SelectedItem);
                if (filtertosave._valueOne != string.Empty) { Assert.AreEqual(filtertosave._valueOne, viewcontrols.textFilterValue1Txt.Text); }
                if (filtertosave._valueTwo != string.Empty) { Assert.AreEqual(filtertosave._valueTwo, viewcontrols.textFilterValue2Txt.Text); }
                _customEntityViewsUIMap.PressCancelOnViewFilterModal();
                numberoofcopies++;
            }
            #endregion
        }
        #endregion

        #region unsuccessfully add filter on attribute of type number for view in custom entity where invalid characters are used
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeNumberWhereInvalidValuesAreUsedForFilterOfViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            var filtertosave = _customEntities[0].view[0].filters[5];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///set criteria as equals Number1 as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForNumber1 + ViewModalMessages.ValidFieldRangeForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal Number1 as above 2147483647 and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            viewcontrols.textFilterValue1Txt.Text = "2147483648";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.ValidFieldRangeForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than Number1 as a decimal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            viewcontrols.textFilterValue1Txt.Text = "0.7";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForNumber1 + ViewModalMessages.ValidFieldRangeForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between Number1 greater than number 2 and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            viewcontrols.textFilterValue1Txt.Text = "4";
            viewcontrols.textFilterValue2Txt.Text = "2";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForNumber2GreaterThanNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            #endregion
        }
        #endregion
        #endregion

        #region decimal
        #region successfully add filter on attribute of type decimal for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeDecimalForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._fieldid = _customEntities[0].attribute[3].FieldId;
                if (_customEntities[0].view[0].filters[filterindex]._valueOne != string.Empty) { _customEntities[0].view[0].filters[filterindex]._valueOne += ".000"; }
                if (_customEntities[0].view[0].filters[filterindex]._valueTwo != string.Empty) { _customEntities[0].view[0].filters[filterindex]._valueTwo += ".000"; }
            }
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[5];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than or equal to and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than or and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1 + ViewModalMessages.EmptyFieldForDecimal2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 7;
            ///loop through filters for number and create all
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo; }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());

                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[3].DisplayName, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                /// click edit grid entry to validate its individual properties
                ClickEditFilter(copy, filtertosave._fieldid.ToString());

                //refresh filter controls to validate each set property
                viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                Assert.AreEqual(conditiontype, viewcontrols.filterCriteriaOption.SelectedItem);
                if (filtertosave._valueOne != string.Empty) { Assert.AreEqual(filtertosave._valueOne, viewcontrols.textFilterValue1Txt.Text); }
                if (filtertosave._valueTwo != string.Empty) { Assert.AreEqual(filtertosave._valueTwo, viewcontrols.textFilterValue2Txt.Text); }
                _customEntityViewsUIMap.PressCancelOnViewFilterModal();
                numberoofcopies++;
            }
            #endregion

            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._valueOne = _customEntities[0].view[0].filters[filterindex]._valueOne.Replace(".000", "");
                _customEntities[0].view[0].filters[filterindex]._valueTwo = _customEntities[0].view[0].filters[filterindex]._valueTwo.Replace(".000", "");
            }
        }
        #endregion

        #region unsuccessfully add filter on attribute of type decimal for view in custom entity where invalid characters are used
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeDecimalWhereInvalidValuesAreUsedForFilterOfViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._fieldid = _customEntities[0].attribute[3].FieldId;
            }
            CustomEntityViewsControls viewcontrols = null;
            var filtertosave = _customEntities[0].view[0].filters[5];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///set criteria as equals, Number1 as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal, Number1 as special characters and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            viewcontrols.textFilterValue1Txt.Text = ",.<>/?;'!£$%";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than, Number1 as an invalid decimal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            viewcontrols.textFilterValue1Txt.Text = "0.7.8";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between, Number1 equal to number 2 and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            viewcontrols.textFilterValue1Txt.Text = "4";
            viewcontrols.textFilterValue2Txt.Text = "4";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDecimal1GreaterThanDecimal2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            #endregion
        }
        #endregion
        #endregion

        #region currency
        #region successfully add filter on attribute of type currency for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeCurrencyForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._fieldid = _customEntities[0].attribute[4].FieldId;
                if (_customEntities[0].view[0].filters[filterindex]._valueOne != string.Empty) { _customEntities[0].view[0].filters[filterindex]._valueOne += ".00"; }
                if (_customEntities[0].view[0].filters[filterindex]._valueTwo != string.Empty) { _customEntities[0].view[0].filters[filterindex]._valueTwo += ".00"; }
            }
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[5];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than or equal to and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than or and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1 + ViewModalMessages.EmptyFieldForCurrency2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 7;
            ///loop through filters for number and create all
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo; }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());

                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[4].DisplayName, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                /// click edit grid entry to validate its individual properties
                ClickEditFilter(copy, filtertosave._fieldid.ToString());

                //refresh filter controls to validate each set property
                viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                Assert.AreEqual(conditiontype, viewcontrols.filterCriteriaOption.SelectedItem);
                if (filtertosave._valueOne != string.Empty) { Assert.AreEqual(filtertosave._valueOne, viewcontrols.textFilterValue1Txt.Text); }
                if (filtertosave._valueTwo != string.Empty) { Assert.AreEqual(filtertosave._valueTwo, viewcontrols.textFilterValue2Txt.Text); }
                _customEntityViewsUIMap.PressCancelOnViewFilterModal();
                numberoofcopies++;
            }
            #endregion

            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._valueOne = _customEntities[0].view[0].filters[filterindex]._valueOne.Replace(".00", "");
                _customEntities[0].view[0].filters[filterindex]._valueTwo = _customEntities[0].view[0].filters[filterindex]._valueTwo.Replace(".00", "");
            }
        }
        #endregion

        #region unsuccessfully add filter on attribute of type currency for view in custom entity where invalid characters are used
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeCurrencyWhereInvalidValuesAreUsedForFilterOfViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._fieldid = _customEntities[0].attribute[4].FieldId;
            }
            CustomEntityViewsControls viewcontrols = null;
            var filtertosave = _customEntities[0].view[0].filters[5];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///set criteria as equals, Currency1 as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal, Currency1 as special characters and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            viewcontrols.textFilterValue1Txt.Text = ",.<>/?;'!£$%";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than, Currency1 as an invalid decimal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            viewcontrols.textFilterValue1Txt.Text = "0.7.8";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between, Currency1 equal to Currency2 and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            viewcontrols.textFilterValue1Txt.Text = "4";
            viewcontrols.textFilterValue2Txt.Text = "4";
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForCurrency1GreaterThanCurrency2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            #endregion
        }
        #endregion
        #endregion

        #region checkbox
        #region 41454 successfully add filter on attribute of type currency for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeCheckboxForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            string yesNoValue = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[12];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyDropDownForYesOrNo, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyDropDownForYesOrNo, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 2;
            ///loop through filters for number and create all
            for (int filterindexer = 12; filterindexer < numberfiltercount + 12; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                yesNoValue = EnumHelper.GetEnumDescription((YesOrNo)Convert.ToInt32(filtertosave._valueOne));
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                viewcontrols.cmbFilterOption.SelectedItem = yesNoValue;
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 12; filterindexer < numberfiltercount + 12; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());

                yesNoValue = EnumHelper.GetEnumDescription((YesOrNo)Convert.ToInt32(filtertosave._valueOne));
                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[12].DisplayName, (ConditionType)filtertosave._conditionType, yesNoValue);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                /// click edit grid entry to validate its individual properties
                ClickEditFilter(copy, filtertosave._fieldid.ToString());

                //refresh filter controls to validate each set property
                viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                Assert.AreEqual(conditiontype, viewcontrols.filterCriteriaOption.SelectedItem);
                Assert.AreEqual(yesNoValue, viewcontrols.cmbFilterOption.SelectedItem);
                _customEntityViewsUIMap.PressCancelOnViewFilterModal();
                numberoofcopies++;
            }
            #endregion
        }
        #endregion
        #endregion

        #region date
        #region 41459 successfully add filter on attribute of type date for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeDateForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[14];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.On);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as not on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NotOn);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.After);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Before);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrAfter);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrBefore);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1 + ViewModalMessages.EmptyFieldForDate2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x days and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXDays);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x days and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXDays);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x weeks and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXWeeks);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x weeks and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXWeeks);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x months and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXMonths);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x months and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXMonths);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x years and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXYears);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x years and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXYears);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 38;
            ///loop through filters for number and create all
            for (int filterindexer = 14; filterindexer < numberfiltercount + 14; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty)
                {
                    string DateTime1 = filtertosave._valueOne;
                    string[] DateAndTime1 = Regex.Split(DateTime1, " ");
                    if (DateAndTime1.Length > 1)
                    {
                        viewcontrols.textFilterValue1Txt.Text = DateAndTime1[0];
                    }
                    else
                    {
                        viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
                    }
                }
                if (filtertosave._valueTwo != string.Empty)
                {
                    string DateTime2 = filtertosave._valueTwo;
                    string[] DateAndTime2 = Regex.Split(DateTime2, " ");
                    if (DateAndTime2.Length > 1)
                    {
                        viewcontrols.textFilterValue2Txt.Text = DateAndTime2[0];
                    }
                    else
                    {
                        viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo;
                    }
                }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 14; filterindexer < numberfiltercount + 14; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());

                string DateTime1 = filtertosave._valueOne;
                string[] DateAndTime1 = Regex.Split(DateTime1, " ");

                string DateTime2 = filtertosave._valueTwo;
                string[] DateAndTime2 = Regex.Split(DateTime2, " ");

                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[6].DisplayName, (ConditionType)filtertosave._conditionType, DateAndTime1[0], DateAndTime2[0]);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                /// click edit grid entry to validate its individual properties
                //ClickEditFilter(copy, filtertosave._fieldid.ToString());

                ////refresh filter controls to validate each set property
                //viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                //conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                //Assert.AreEqual(conditiontype, viewcontrols.filterCriteriaOption.SelectedItem);
                //if (filtertosave._valueOne != string.Empty)
                //{
                //    Assert.AreEqual(DateAndTime1[0], viewcontrols.textFilterValue1Txt.Text);
                //}
                //if (filtertosave._valueTwo != string.Empty)
                //{
                //    Assert.AreEqual(DateAndTime2[0], viewcontrols.textFilterValue2Txt.Text);
                //}
                //_customEntityViewsUIMap.PressCancelOnViewFilterModal();
                numberoofcopies++;
            }
            #endregion
        }
        #endregion

        #region unsuccessfully add filter on attribute of type date for view in custom entity where invalid characters are used
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeDateWhereInvalidValuesAreUsedForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[14];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            /////set criteria as before, Date 1 as 'MM/DD/YYYY' and validate message on save
            //viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Before);
            //viewcontrols.textFilterValue1Txt.Text = "MM/DD/YYYY";
            //Keyboard.SendKeys(FormGlobalProperties.TAB);
            //Keyboard.SendKeys(FormGlobalProperties.ENTER);
            //cCustomEntityViewsMethods.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            //cCustomEntityViewsMethods.ValidateMessageModal();
            //Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.After);
            viewcontrols.textFilterValue1Txt.Text = "010101";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as between, Date 1 as Greater Than Date2 and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            viewcontrols.textFilterValue1Txt.Text = "01/04/2012";
            viewcontrols.textFilterValue2Txt.Text = "01/03/2012";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDate1GreaterThanDate2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x days, Number of days as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXDays);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDays + ViewModalMessages.ValidFieldRangeForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x days, Number of days as -tive and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXDays);
            viewcontrols.textFilterValue1Txt.Text = "-6";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.ValidFieldRangeForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x weeks, Number of weeks as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXWeeks);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForWeeks + ViewModalMessages.ValidFieldRangeForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x weeks, Number of weeks as fraction and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXWeeks);
            viewcontrols.textFilterValue1Txt.Text = "3/2";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForWeeks + ViewModalMessages.ValidFieldRangeForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x months, Number of months as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXMonths);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForMonths + ViewModalMessages.ValidFieldRangeForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x months, Number of months as char and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXMonths);
            viewcontrols.textFilterValue1Txt.Text = "£?%:";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForMonths + ViewModalMessages.ValidFieldRangeForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x years, Number of years as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXYears);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForYears + ViewModalMessages.ValidFieldRangeForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x years, Number of years as decimal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXYears);
            viewcontrols.textFilterValue1Txt.Text = "0.6";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForYears + ViewModalMessages.ValidFieldRangeForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion
        }
        #endregion
        #endregion

        #region datetime
        #region successfully add filter on attribute of type date time for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeDateTimeForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            for (int filterindex = 14; filterindex < 52; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._fieldid = _customEntities[0].attribute[5].FieldId;
            }
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[14];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.On);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDateAndTime1 + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as not on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NotOn);
            viewcontrols.textFilterValue1Txt.Text = "01/01/2012";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.After);
            viewcontrols.timeFilterValue1Txt.Text = "06:00";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDateAndTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Before);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDateAndTime1 + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrAfter);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDateAndTime1 + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrBefore);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDateAndTime1 + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            ///bug investigate manny
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDateAndTime1 + ViewModalMessages.EmptyFieldForTime1 + ViewModalMessages.EmptyFieldForDateAndTime2 + ViewModalMessages.EmptyFieldForTime2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x days and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXDays);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x days and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXDays);
            //Keyboard.SendKeys(FormGlobalProperties.TAB);
            //Keyboard.SendKeys(FormGlobalProperties.ENTER);


            //TDOO Manaul
            HtmlInputButton saveBtn = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow12.UIGreenLightCustomEntiDocument.UISaveButton;
            Mouse.Click(saveBtn);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x weeks and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXWeeks);
            //Keyboard.SendKeys(FormGlobalProperties.TAB);
            //Keyboard.SendKeys(FormGlobalProperties.ENTER);
            Mouse.Click(saveBtn);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x weeks and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXWeeks);
            //Keyboard.SendKeys(FormGlobalProperties.TAB);
            //Keyboard.SendKeys(FormGlobalProperties.ENTER);
            Mouse.Click(saveBtn);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x months and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXMonths);
            //Keyboard.SendKeys(FormGlobalProperties.TAB);
            //Keyboard.SendKeys(FormGlobalProperties.ENTER);
            Mouse.Click(saveBtn);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x months and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXMonths);
            //Keyboard.SendKeys(FormGlobalProperties.TAB);
            //Keyboard.SendKeys(FormGlobalProperties.ENTER);
            Mouse.Click(saveBtn);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x years and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXYears);
            //Keyboard.SendKeys(FormGlobalProperties.TAB);
            //Keyboard.SendKeys(FormGlobalProperties.ENTER);
            Mouse.Click(saveBtn);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x years and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXYears);
           // Keyboard.SendKeys(FormGlobalProperties.TAB);
           // Keyboard.SendKeys(FormGlobalProperties.ENTER);
            Mouse.Click(saveBtn);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 38;
            ///loop through filters for number and create all
            for (int filterindexer = 14; filterindexer < numberfiltercount + 14; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty)
                {
                    string DateTime1 = filtertosave._valueOne;
                    string[] DateAndTime1 = Regex.Split(DateTime1, " ");
                    if (DateAndTime1.Length > 1)
                    {
                        viewcontrols.textFilterValue1Txt.Text = DateAndTime1[0];
                        viewcontrols.timeFilterValue1Txt.Text = DateAndTime1[1];
                    }
                    else
                    {
                        viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
                    }
                }
                if (filtertosave._valueTwo != string.Empty)
                {
                    string DateTime2 = filtertosave._valueTwo;
                    string[] DateAndTime2 = Regex.Split(DateTime2, " ");
                    if (DateAndTime2.Length > 1)
                    {
                        viewcontrols.textFilterValue2Txt.Text = DateAndTime2[0];
                        viewcontrols.timeFilterValue2Txt.Text = DateAndTime2[1];
                    }
                    else
                    {
                        viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo;
                    }
                }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 14; filterindexer < numberfiltercount + 14; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());
                switch ((ConditionType)filtertosave._conditionType)
                {
                    case ConditionType.LastXDays:
                    case ConditionType.NextXDays:
                    case ConditionType.LastXWeeks:
                    case ConditionType.NextXWeeks:
                    case ConditionType.NextXMonths:
                    case ConditionType.LastXMonths:
                    case ConditionType.LastXYears:
                    case ConditionType.NextXYears:
                        filtertosave._valueOne += " ";
                        break;
                }

                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[5].DisplayName, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                ///// click edit grid entry to validate its individual properties
                //ClickEditFilter(copy, filtertosave._fieldid.ToString());

                ////refresh filter controls to validate each set property
                //viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                //conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                //Assert.AreEqual(conditiontype, viewcontrols.filterCriteriaOption.SelectedItem);
                //if (filtertosave._valueOne != string.Empty) { Assert.AreEqual(filtertosave._valueOne.Split(' ')[0], viewcontrols.textFilterValue1Txt.Text); }
                //if (filtertosave._valueTwo != string.Empty) { Assert.AreEqual(filtertosave._valueTwo.Split(' ')[0], viewcontrols.textFilterValue2Txt.Text); }
                //_customEntityViewsUIMap.PressCancelOnViewFilterModal();
                numberoofcopies++;
            }
            #endregion
        }
        #endregion

        #region unsuccessfully add filter on attribute of type date time for view in custom entity where invalid characters are used
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeDateTimeWhereInvalidValuesAreUsedForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            for (int filterindex = 14; filterindex < 52; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._fieldid = _customEntities[0].attribute[5].FieldId;
            }
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[14];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///set criteria as before, Date AND time 1 as 'MM/DD/YYYY' and Time 1 'HH:SS' and validate message on save
            //viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Before);
            //viewcontrols.textFilterValue1Txt.Text = "MM/DD/YYYY";
            //viewcontrols.timeFilterValue1Txt.Text = "HH:SS";
            //Keyboard.SendKeys(FormGlobalProperties.TAB);
            //Keyboard.SendKeys(FormGlobalProperties.ENTER);
            //cCustomEntityViewsMethods.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDateAndTime1 + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            //cCustomEntityViewsMethods.ValidateMessageModal();
            //Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as after, Date AND time 1 as '010101' and Time 1 '24:00' and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.After);
            viewcontrols.textFilterValue1Txt.Text = "010101";
            viewcontrols.timeFilterValue1Txt.Text = "24:00";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDateAndTime1 + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as between, Date and time 1 and Time 1 as Greater Than Date and time 2 and Time 2 validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            viewcontrols.textFilterValue1Txt.Text = "01/04/2012";
            viewcontrols.textFilterValue2Txt.Text = "01/04/2012";
            viewcontrols.timeFilterValue1Txt.Text = "23:00";
            viewcontrols.timeFilterValue2Txt.Text = "12:00";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDateAndTime1GreaterThanDateAndTime2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x days, Number of days as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXDays);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForDays + ViewModalMessages.ValidFieldRangeForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x days, Number of days as -tive and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXDays);
            viewcontrols.textFilterValue1Txt.Text = "-6";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.ValidFieldRangeForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x weeks, Number of weeks as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXWeeks);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForWeeks + ViewModalMessages.ValidFieldRangeForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x weeks, Number of weeks as fraction and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXWeeks);
            viewcontrols.textFilterValue1Txt.Text = "3/2";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForWeeks + ViewModalMessages.ValidFieldRangeForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x months, Number of months as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXMonths);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForMonths + ViewModalMessages.ValidFieldRangeForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x months, Number of months as char and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXMonths);
            viewcontrols.textFilterValue1Txt.Text = "£?%:";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForMonths + ViewModalMessages.ValidFieldRangeForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x years, Number of years as NaN and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXYears);
            viewcontrols.textFilterValue1Txt.Text = "NaN";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForYears + ViewModalMessages.ValidFieldRangeForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x years, Number of years as decimal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXYears);
            viewcontrols.textFilterValue1Txt.Text = "0.6";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForYears + ViewModalMessages.ValidFieldRangeForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion
        }
        #endregion
        #endregion

        #region time
        #region 41456 successfully add filter on attribute of type time for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeTimeForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[52];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as not on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.After);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Before);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrAfter);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrBefore);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1 + ViewModalMessages.EmptyFieldForTime2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 8;
            ///loop through filters for number and create all
            for (int filterindexer = 52; filterindexer < numberfiltercount + 52; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.timeFilterValue1Txt.Text = filtertosave._valueOne; }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.timeFilterValue2Txt.Text = filtertosave._valueTwo; }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            int numberoofcopies = 0;
            for (int filterindexer = 52; filterindexer < numberfiltercount + 52; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());

                string tablerowentry = GetFilterGridRowText(_customEntities[0].attribute[7].DisplayName, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                ///validate entry displayed in grid
                Assert.AreEqual(tablerowentry, GridRowcontrol.InnerText);

                /// click edit grid entry to validate its individual properties
                ClickEditFilter(copy, filtertosave._fieldid.ToString());

                //refresh filter controls to validate each set property
                viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                Assert.AreEqual(conditiontype, viewcontrols.filterCriteriaOption.SelectedItem);
                if (filtertosave._valueOne != string.Empty) { Assert.AreEqual(filtertosave._valueOne, viewcontrols.timeFilterValue1Txt.Text); }
                if (filtertosave._valueTwo != string.Empty) { Assert.AreEqual(filtertosave._valueTwo, viewcontrols.timeFilterValue2Txt.Text); }
                _customEntityViewsUIMap.PressCancelOnViewFilterModal();
                numberoofcopies++;
            }
            #endregion
        }
        #endregion

        #region Unsuccessfully add filter on attribute of type time for view in custom entity where invalid characters are used
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeTimeWhereInvalidValuesAreUsedForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[52];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed

            ///set criteria as equals, Time 1 as 'HH:SS' and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            viewcontrols.timeFilterValue1Txt.Text = "HH:SS";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as does not equal, Time 1 as decimal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            viewcontrols.timeFilterValue1Txt.Text = "0.7";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as after, Time 1 as characters and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.After);
            viewcontrols.timeFilterValue1Txt.Text = "<>'#";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as before, Time 1 as 24:00 and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Before);
            viewcontrols.timeFilterValue1Txt.Text = "24:00";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or after, Time 1 as 23:00 and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrAfter);
            viewcontrols.timeFilterValue1Txt.Text = "23/00";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as between, Time 1 as Greater Than Time 2 and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            viewcontrols.timeFilterValue1Txt.Text = "10:00";
            viewcontrols.timeFilterValue2Txt.Text = "09:00";
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.InvalidFieldForTime1GreaterThanTime2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion
        }
        #endregion
        #endregion

        #region list
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFilterOnCustomEntityAttributeOfTypeList_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to CE -> Views -> Filters Tab

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            CustomEntitiesUtilities.cCustomEntityListAttribute listAttribute = null;
            foreach (CustomEntitiesUtilities.CustomEntityAttribute att in _customEntities[0].attribute)
            {
                if (att.GetType() == typeof(CustomEntitiesUtilities.cCustomEntityListAttribute))
                {
                    listAttribute = (CustomEntitiesUtilities.cCustomEntityListAttribute)att;
                    break;
                }
            }

            SelectAvailableFieldForFilter(null, "  " + listAttribute.DisplayName);

            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

            #region Validate all validation modals for type list

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));

            //leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyListSelecton, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyListSelecton, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion


            Dictionary<string, bool> filterCriteria = new Dictionary<string, bool>()
            {
                {EnumHelper.GetEnumDescription(ConditionType.Equals), true}, 
                {EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual), true},
                {EnumHelper.GetEnumDescription(ConditionType.ContainsData), false},
                {EnumHelper.GetEnumDescription(ConditionType.DoesNotContainData), false}
            };

            StringBuilder expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
            foreach (KeyValuePair<string, bool> fCriteria in filterCriteria)
            {
                expectedSelectedFilterValue.Append(listAttribute.DisplayName);
                SelectAvailableFieldForFilter(null, "  " + listAttribute.DisplayName);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

                viewcontrols.filterCriteriaOption.SelectedItem = fCriteria.Key;
                expectedSelectedFilterValue.Append(fCriteria.Key);
                //If flag is true - means that it needs valid list items
                if (fCriteria.Value)
                {
                    foreach (CustomEntitiesUtilities.EntityListItem lItem in listAttribute._listItems)
                    {
                        _customEntityViewsUIMap.MoveSelectedListItem(lItem._textItem);
                    }
                    expectedSelectedFilterValue.Append("standard list item... and 5 others");
                }
                else
                {
                    expectedSelectedFilterValue.Append("-");
                }
                expectedSelectedFilterValue.Append("Standard List");
                _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            }

            #region save, edit and verify results
            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion

        }
        #endregion
        #endregion

        #region edit filters
        #region 39542 successfully edit filter on attribute of type time for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyEditFiltersForAttributeOfAllTypeForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region build grid text
            CustomEntityViewsControls viewcontrols = null;
            List<HtmlCustom> GridControls = new List<HtmlCustom>();
            List<string> GridText = new List<string>();
            string filterattribute = string.Empty;
            //int numberoofcopies = 0;
            HtmlCustom GridRowControl = new HtmlCustom();
            string tablerowentry = string.Empty;

            filterattribute = _customEntities[0].attribute[0].DisplayName;
            var filtertoedit = _customEntities[0].view[0].filters[0];
            GridRowControl = GridRow("", filtertoedit._fieldid.ToString());
            GridControls.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertoedit._conditionType, filtertoedit._valueOne, filtertoedit._valueTwo);
            GridText = GetGridText(tablerowentry);

            filterattribute = _customEntities[0].attribute[2].DisplayName;
            filtertoedit = _customEntities[0].view[0].filters[5];
            GridRowControl = GridRow("", filtertoedit._fieldid.ToString());
            GridControls.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertoedit._conditionType, filtertoedit._valueOne, filtertoedit._valueTwo);
            GridText = GetGridText(tablerowentry);

            filterattribute = _customEntities[0].attribute[2].DisplayName;
            filtertoedit = _customEntities[0].view[0].filters[11];
            GridRowControl = GridRow("copy_", filtertoedit._fieldid.ToString());
            GridControls.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertoedit._conditionType, filtertoedit._valueOne, filtertoedit._valueTwo);
            GridText = GetGridText(tablerowentry);

            filterattribute = _customEntities[0].attribute[12].DisplayName;
            filtertoedit = _customEntities[0].view[0].filters[13];
            GridRowControl = GridRow("", filtertoedit._fieldid.ToString());
            GridControls.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertoedit._conditionType, EnumHelper.GetEnumDescription((YesOrNo)Convert.ToInt32(filtertoedit._valueOne)), filtertoedit._valueTwo);
            GridText = GetGridText(tablerowentry);

            filterattribute = _customEntities[0].attribute[5].DisplayName;
            filtertoedit = _customEntities[0].view[0].filters[14];
            GridRowControl = GridRow("", filtertoedit._fieldid.ToString());
            GridControls.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertoedit._conditionType, filtertoedit._valueOne, filtertoedit._valueTwo);
            GridText = GetGridText(tablerowentry);

            filterattribute = _customEntities[0].attribute[5].DisplayName;
            filtertoedit = _customEntities[0].view[0].filters[48];
            GridRowControl = GridRow("copy_", filtertoedit._fieldid.ToString());
            GridControls.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertoedit._conditionType, filtertoedit._valueOne, filtertoedit._valueTwo);
            GridText = GetGridText(tablerowentry);

            filterattribute = _customEntities[0].attribute[5].DisplayName;
            filtertoedit = _customEntities[0].view[0].filters[51];
            GridRowControl = GridRow("copy_copy_", filtertoedit._fieldid.ToString());
            GridControls.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertoedit._conditionType, filtertoedit._valueOne, filtertoedit._valueTwo);
            GridText = GetGridText(tablerowentry);

            filterattribute = _customEntities[0].attribute[7].DisplayName;
            filtertoedit = _customEntities[0].view[0].filters[54];
            GridRowControl = GridRow("", filtertoedit._fieldid.ToString());
            GridControls.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertoedit._conditionType, filtertoedit._valueOne, filtertoedit._valueTwo);
            GridText = GetGridText(tablerowentry);

            filterattribute = _customEntities[0].attribute[7].DisplayName;
            filtertoedit = _customEntities[0].view[0].filters[55];
            GridRowControl = GridRow("copy_", filtertoedit._fieldid.ToString());
            GridControls.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertoedit._conditionType, filtertoedit._valueOne, filtertoedit._valueTwo);
            GridText = GetGridText(tablerowentry);
            #endregion

            #region verify filters before editing
            string expectedFiltersText = FilterPaneString(GridText);
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            Assert.AreEqual(expectedFiltersText, viewcontrols.filtersDropPane.InnerText);
            #endregion

            #region edit existing filters
            filtertoedit = _customEntities[0].view[0].filters[0];
            var filtertosave = _customEntities[0].view[0].filters[1];
            ClickEditFilter("", filtertoedit._fieldid.ToString());
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
            viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            filtertoedit = _customEntities[0].view[0].filters[5];
            filtertosave = _customEntities[0].view[0].filters[6];
            ClickEditFilter("", filtertoedit._fieldid.ToString());
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
            viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            filtertoedit = _customEntities[0].view[0].filters[11];
            filtertosave = _customEntities[0].view[0].filters[7];
            ClickEditFilter("copy_", filtertoedit._fieldid.ToString());
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
            viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            filtertoedit = _customEntities[0].view[0].filters[13];
            filtertosave = _customEntities[0].view[0].filters[12];
            ClickEditFilter("", filtertoedit._fieldid.ToString());
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
            viewcontrols.cmbFilterOption.SelectedItem = EnumHelper.GetEnumDescription((YesOrNo)Convert.ToInt32(filtertosave._valueOne));
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            filtertoedit = _customEntities[0].view[0].filters[14];
            filtertosave = _customEntities[0].view[0].filters[15];
            ClickEditFilter("", filtertoedit._fieldid.ToString());
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
            string DateTime1 = filtertosave._valueOne;
            string[] DateAndTime1 = Regex.Split(DateTime1, " ");
            viewcontrols.textFilterValue1Txt.Text = DateAndTime1[0];
            viewcontrols.timeFilterValue1Txt.Text = DateAndTime1[1];
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            filtertoedit = _customEntities[0].view[0].filters[48];
            filtertosave = _customEntities[0].view[0].filters[45];
            ClickEditFilter("copy_", filtertoedit._fieldid.ToString());
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
            viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            filtertoedit = _customEntities[0].view[0].filters[51];
            filtertosave = _customEntities[0].view[0].filters[50];
            ClickEditFilter("copy_copy_", filtertoedit._fieldid.ToString());
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            filtertoedit = _customEntities[0].view[0].filters[54];
            filtertosave = _customEntities[0].view[0].filters[56];
            ClickEditFilter("", filtertosave._fieldid.ToString());
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
            viewcontrols.timeFilterValue1Txt.Text = filtertosave._valueOne;
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            filtertoedit = _customEntities[0].view[0].filters[55];
            filtertosave = _customEntities[0].view[0].filters[57];
            ClickEditFilter("copy_", filtertoedit._fieldid.ToString());
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
            viewcontrols.timeFilterValue1Txt.Text = filtertosave._valueOne;
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            #endregion

            #region build grid text after editing
            GridText = new List<string>();
            tablerowentry = string.Empty;
            List<string> GridTextEdited = new List<string>();
            List<HtmlCustom> GridControlsEdited = new List<HtmlCustom>();
            filterattribute = _customEntities[0].attribute[0].DisplayName;
            var filteredited = _customEntities[0].view[0].filters[1];
            GridRowControl = GridRow("copy_", filteredited._fieldid.ToString());
            GridControlsEdited.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filteredited._conditionType, filteredited._valueOne, filteredited._valueTwo);
            GridTextEdited.Add(tablerowentry);

            filterattribute = _customEntities[0].attribute[2].DisplayName;
            filteredited = _customEntities[0].view[0].filters[6];
            GridRowControl = GridRow("copy_copy_", filteredited._fieldid.ToString());
            GridControlsEdited.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filteredited._conditionType, filteredited._valueOne, filteredited._valueTwo);
            GridTextEdited.Add(tablerowentry);

            filterattribute = _customEntities[0].attribute[2].DisplayName;
            filteredited = _customEntities[0].view[0].filters[7];
            GridRowControl = GridRow("copy_copy_copy_", filteredited._fieldid.ToString());
            GridControlsEdited.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filteredited._conditionType, filteredited._valueOne, filteredited._valueTwo);
            GridTextEdited.Add(tablerowentry);

            filterattribute = _customEntities[0].attribute[12].DisplayName;
            filteredited = _customEntities[0].view[0].filters[12];
            GridRowControl = GridRow("copy_", filteredited._fieldid.ToString());
            GridControlsEdited.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filteredited._conditionType, EnumHelper.GetEnumDescription((YesOrNo)Convert.ToInt32(filteredited._valueOne)), filteredited._valueTwo);
            GridTextEdited.Add(tablerowentry);

            filterattribute = _customEntities[0].attribute[5].DisplayName;
            filteredited = _customEntities[0].view[0].filters[15];
            GridRowControl = GridRow("copy_copy_copy_", filteredited._fieldid.ToString());
            GridControlsEdited.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filteredited._conditionType, filteredited._valueOne, filteredited._valueTwo);
            GridTextEdited.Add(tablerowentry);

            filterattribute = _customEntities[0].attribute[5].DisplayName;
            filteredited = _customEntities[0].view[0].filters[45];
            GridRowControl = GridRow("copy_copy_copy_copy_", filteredited._fieldid.ToString());
            GridControlsEdited.Add(GridRowControl);
            //filteredited._valueOne += " "; //included due to browser spacing anomalies
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filteredited._conditionType, filteredited._valueOne, filteredited._valueTwo);
            GridTextEdited.Add(tablerowentry);

            filterattribute = _customEntities[0].attribute[5].DisplayName;
            filteredited = _customEntities[0].view[0].filters[50];
            GridRowControl = GridRow("copy_copy_copy_copy_copy_", filteredited._fieldid.ToString());
            GridControlsEdited.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filteredited._conditionType, filteredited._valueOne, filteredited._valueTwo);
            GridTextEdited.Add(tablerowentry);

            filterattribute = _customEntities[0].attribute[7].DisplayName;
            filteredited = _customEntities[0].view[0].filters[56];
            GridRowControl = GridRow("copy_copy_", filteredited._fieldid.ToString());
            GridControlsEdited.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filteredited._conditionType, filteredited._valueOne, filteredited._valueTwo);
            GridTextEdited.Add(tablerowentry);

            filterattribute = _customEntities[0].attribute[7].DisplayName;
            filteredited = _customEntities[0].view[0].filters[57];
            GridRowControl = GridRow("copy_copy_copy_", filteredited._fieldid.ToString());
            GridControlsEdited.Add(GridRowControl);
            tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filteredited._conditionType, filteredited._valueOne, filteredited._valueTwo);
            GridTextEdited.Add(tablerowentry);
            #endregion

            #region save and return to validate filter order
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            expectedFiltersText = string.Empty;
            expectedFiltersText = FilterPaneString(GridTextEdited);
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            Assert.AreEqual(expectedFiltersText, viewcontrols.filtersDropPane.InnerText);
            #endregion

        }
        #endregion
        #endregion

        #region 39560 Unsuccessfully add filter on attribute for view in custom entity where cancel is clicked
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyAddFiltersForAttributeWhereCancelIsClickedForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region add all filters required and build required controls
            CustomEntityViewsControls viewcontrols = null;
            var filtertosave = _customEntities[0].view[0].filters[0];
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            string conditiontype = string.Empty;
            int stringfiltercount = 5;
            ///loop through filters for standard text and create all
            for (int filterindexer = 0; filterindexer < stringfiltercount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueTwo; }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region build grid text before save or cancel
            List<HtmlCustom> GridControls = new List<HtmlCustom>();
            int numberoofcopies = 1;
            for (int filterindexer = 0; filterindexer < stringfiltercount; filterindexer++)
            {
                string filterattribute;
                filterattribute = _customEntities[0].attribute[0].DisplayName;
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());
                GridControls.Add(GridRowcontrol);
                string tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                GridText = GetGridText(tablerowentry);
                numberoofcopies++;
            }
            #endregion

            #region validate all filters displayed on grid before save or cancel
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            string expectedFiltersPaneText = FilterPaneString(GridText);
            Assert.AreEqual(expectedFiltersPaneText, viewcontrols.filtersDropPane.InnerText);
            #endregion

            #region click cancel and return to view
            _customEntityViewsUIMap.PressCancelOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region verify no filter
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            GridText = new List<string>();
            expectedFiltersPaneText = FilterPaneString(GridText);
            Assert.AreEqual(expectedFiltersPaneText, viewcontrols.filtersDropPane.InnerText);
            #endregion
        }
        #endregion

        #region 39544 Unsuccessfully edit filter on attribute for view in custom entity where cancel is clicked
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyEditFiltersForAttributeWhereCancelIsClickedForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region build grid text before editing
            CustomEntityViewsControls viewcontrols = null;
            var filtertosave = _customEntities[0].view[0].filters[0];
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            string conditiontype = string.Empty;
            int stringfiltercount = 5;
            List<HtmlCustom> GridControls = new List<HtmlCustom>();
            int numberoofcopies = 0;
            for (int filterindexer = 0; filterindexer < stringfiltercount; filterindexer++)
            {
                string filterattribute;
                filterattribute = _customEntities[0].attribute[0].DisplayName;
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());
                GridControls.Add(GridRowcontrol);
                string tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                GridText = GetGridText(tablerowentry);
                numberoofcopies++;
            }
            #endregion

            #region edit existing filters
            //viewcontrols = new CustomEntityViewsControls(cCustomEntityViewsMethods, EnumHelper.GetEnumDescription(TabName.Filters));
            ClickEditFilter("", filtertosave._fieldid.ToString());
            viewcontrols.textFilterValue1Txt.Text = viewcontrols.textFilterValue1Txt.Text + "EDITED";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            ClickEditFilter("copy_", filtertosave._fieldid.ToString());
            viewcontrols.textFilterValue1Txt.Text = viewcontrols.textFilterValue1Txt.Text + "EDITED";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            #endregion

            #region UNDONE validate edited filters displayed on grid before save or cancel
            //viewcontrols = new CustomEntityViewsControls(cCustomEntityViewsMethods, EnumHelper.GetEnumDescription(TabName.Filters));
            //string expectedFiltersPaneText = FilterPaneString(GridText);
            //Assert.AreEqual(expectedFiltersPaneText, viewcontrols.filtersDropPane.InnerText);
            #endregion

            #region click cancel and return to view
            _customEntityViewsUIMap.PressCancelOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region verify no change to exising filters
            string expectedFiltersPaneText = FilterPaneString(GridText);
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            expectedFiltersPaneText = FilterPaneString(GridText);
            Assert.AreEqual(expectedFiltersPaneText, viewcontrols.filtersDropPane.InnerText);
            #endregion
        }
        #endregion

        #region 40163 Unsuccessfully drag filter outside of drop area in view of custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccessfullyDragFilterOutsideOfDropAreaInViewOfCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            DragBasicAttributeOutsideFilterDropArea(_customEntities[0].attribute[0].FieldId.ToString());
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            expectedSelectedFilterValue.Append("There are no filters selected.");
            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
        }
        #endregion
        #endregion

        #region filters buttons
        #region  39540 successfully verify move Right button work correctly in filterd tab
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifyMoveRightButtonWorkCorrectlyInFiltersTabOfViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region select filter to move, click move right  and verify modal displayed
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[5];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            Assert.AreEqual(EmptyDropDown, viewcontrols.filterCriteriaOption.SelectedItem);
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

        }
        #endregion

        #region 39537 39539 successfully verify Remove selection and remove all button work correctly in filterd tab
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifyRemoveSelectionAndRemoveAllButtonsWorkCorrectlyInFiltersTabOfViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region build grid text
            CustomEntityViewsControls viewcontrols = null;
            List<HtmlCustom> GridControls = new List<HtmlCustom>();
            List<string> GridText = new List<string>();
            int numberoofcopies = 0;
            for (int filterindexer = 0; filterindexer < 12; filterindexer++)
            {
                string filterattribute;
                if (filterindexer < 5)
                {
                    filterattribute = _customEntities[0].attribute[0].DisplayName;
                }
                else
                {
                    if (filterindexer == 5) { numberoofcopies = 0; }
                    filterattribute = _customEntities[0].attribute[2].DisplayName;
                }
                var filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());
                GridControls.Add(GridRowcontrol);
                string tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                GridText = GetGridText(tablerowentry);
                numberoofcopies++;
            }
            #endregion

            #region remove selected filters
            for (int filterindexer = 0; filterindexer < 12; filterindexer++)
            {
                var filtertosave = _customEntities[0].view[0].filters[filterindexer];
                if (filterindexer == 2)
                {
                    Mouse.Click(GridControls[filterindexer].GetChildren()[0]);
                    _customEntityViewsUIMap.ClickRemoveFilterSelection();
                    GridText.Remove(GridText[filterindexer]);
                    GridControls.Remove(GridControls[filterindexer]);
                }

                if (filterindexer == 7)
                {
                    Mouse.Click(GridControls[filterindexer].GetChildren()[0]);
                    _customEntityViewsUIMap.ClickRemoveFilterSelection();
                    GridText.Remove(GridText[filterindexer]);
                    GridControls.Remove(GridControls[filterindexer]);
                }
            }
            #endregion

            #region save and return to validate remaining Filters
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            string expectedFiltersText = FilterPaneString(GridText);
            Assert.AreEqual(expectedFiltersText, viewcontrols.filtersDropPane.InnerText);
            #endregion

            #region return and remove all fields
            _customEntityViewsUIMap.ClickRemoveAllFilters();
            GridText = new List<string>();
            #endregion

            #region save and return to validate no Filters remain
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            expectedFiltersText = FilterPaneString(GridText);
            Assert.AreEqual(expectedFiltersText, viewcontrols.filtersDropPane.InnerText);
            #endregion
        }
        #endregion

        #region 39538 39541 successfully verify move Up and move down buttons work correctly in filterd tab
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifyMoveUpAndMoveDownButtonsWorkCorrectlyInFiltersTabOfViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region build grid text
            CustomEntityViewsControls viewcontrols = null;
            List<HtmlCustom> GridControls = new List<HtmlCustom>();
            List<string> GridText = new List<string>();
            int numberoofcopies = 0;
            for (int filterindexer = 0; filterindexer < 12; filterindexer++)
            {
                string filterattribute;
                if (filterindexer < 5)
                {
                    filterattribute = _customEntities[0].attribute[0].DisplayName;
                }
                else
                {
                    if (filterindexer == 5) { numberoofcopies = 0; }
                    filterattribute = _customEntities[0].attribute[2].DisplayName;
                }
                var filtertosave = _customEntities[0].view[0].filters[filterindexer];

                ///create correct number of copies
                string copy = CreateCopy(numberoofcopies);

                ///get correct copy of custom GridRow row control
                HtmlCustom GridRowcontrol = GridRow(copy, filtertosave._fieldid.ToString());
                GridControls.Add(GridRowcontrol);
                string tablerowentry = GetFilterGridRowText(filterattribute, (ConditionType)filtertosave._conditionType, filtertosave._valueOne, filtertosave._valueTwo);

                GridText = GetGridText(tablerowentry);
                numberoofcopies++;
            }
            #endregion

            #region select filter/ number of places to move down and press move
            int filterindextomovedown = 2;
            int destinationindex = 7;
            Mouse.Click(GridControls[filterindextomovedown].GetChildren()[0]);
            ///move selected field a number of times
            for (int filterindex = filterindextomovedown; filterindex <= destinationindex; filterindex++)
            {
                _customEntityViewsUIMap.ClickMoveFilterSelectionDown();
                MoveObjectWithinList(GridText, filterindex, filterindex + 1);
                MoveObjectWithinList(GridControls, filterindex, filterindex + 1);
            }
            #endregion

            #region select filter/ number of places to move up and press move
            int filterindextomoveup = 9;
            destinationindex = 1;
            Mouse.Click(GridControls[filterindextomoveup].GetChildren()[0]);
            for (int filterindex = filterindextomoveup; filterindex >= destinationindex; filterindex--)
            {
                _customEntityViewsUIMap.ClickMoveFilterSelectionUp();
                MoveObjectWithinList(GridText, filterindex, filterindex - 1);
                MoveObjectWithinList(GridControls, filterindex, filterindex - 1);
            }
            #endregion

            #region validate filter ordering
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            string expectedFiltersText = FilterPaneString(GridText);
            Assert.AreEqual(expectedFiltersText, viewcontrols.filtersDropPane.InnerText);
            #endregion

            #region save and validate filter order
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            Assert.AreEqual(expectedFiltersText, viewcontrols.filtersDropPane.InnerText);
            #endregion
        }
        #endregion
        #endregion

        #region  38035 successfully verify modal layout for filters tab in views of custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifyModalLayoutForFiltersTabInViewOfCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            _customEntityViewsUIMap.ValidateFiltersSectionHeader();

            _customEntityViewsUIMap.ValidateFiltersSectionComment();

            _customEntityViewsUIMap.ValidateFiltersAvailableFieldsSectionHeader();

            _customEntityViewsUIMap.ValidateFiltersSelectedFiltersSectionHeader();

            #region drag required string attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[0];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            _customEntityViewsUIMap.ValidateFilterCriteriaDropdownLabel();

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);

            _customEntityViewsUIMap.ValidateValue1TextBoxLabel();

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            #region drag required number attributes
            filtertosave = _customEntities[0].view[0].filters[5];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            #endregion

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);

            _customEntityViewsUIMap.ValidateNumber1TextBoxLabel();

            _customEntityViewsUIMap.ValidateNumber2TextBoxLabel();

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            #region drag required yesno attributes
            filtertosave = _customEntities[0].view[0].filters[12];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            #endregion

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);

            _customEntityViewsUIMap.ValidateYesNoDropDownLabel();

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            #region drag required date attributes
            filtertosave = _customEntities[0].view[0].filters[14];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            #endregion

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);

            _customEntityViewsUIMap.ValidateDate1TextBoxLabel();

            _customEntityViewsUIMap.ValidateDate2TextBoxLabel();

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXDays);

            _customEntityViewsUIMap.ValidateNumberOfDaysTextBoxLabel();

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXWeeks);

            _customEntityViewsUIMap.ValidateNumberOfWeeksTextBoxLabel();

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXMonths);

            _customEntityViewsUIMap.ValidateNumberOfMonthsTextBoxLabel();

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXYears);

            _customEntityViewsUIMap.ValidateNumberOfYearsTextBoxLabel();

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            #region drag required date attributes
            filtertosave = _customEntities[0].view[0].filters[52];
            SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            #endregion

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);

            _customEntityViewsUIMap.ValidateTime1TextBoxLabel();

            _customEntityViewsUIMap.ValidateTime2TextBoxLabel();

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
        }

        #endregion

        #region apply filters to base tables
        #region 41462 Custom Entity views Successfully add a filter on a base table field of type String (s)
        /// <summary>
        /// Custom entity views - Ensures that a filter can be applied to a base table attribute of type string.
        /// The values are hardcoded since the fieldid is static and thus doesn't change. This test uses the 
        /// Account holder name to ensure its working.
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFilterOnBaseTableFieldOfTypeString_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            //Navigate to the CE Admin page
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            //click filters tab
            _customEntityViewsUIMap.ClickFiltersTab();

            //Expand the N:1 relationship to employees
            CustomEntitiesUtilities.CustomEntityNtoOneAttribute nTo1Relationship = null;
            foreach (var attribute in _customEntities[0].attribute)
            {

                if (attribute._fieldType == FieldType.Relationship && attribute.DisplayName == "N : 1 Relationship to Base Table")
                {
                    nTo1Relationship = attribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                }
            }
            //Throw exception when attribute is NULL!! - can't be found within the list of attributes for CE.
            if (nTo1Relationship == null)
            {
                throw new Exception(String.Format("Can't find an attribute in {0} that is of type {1}", new object[] { _customEntities[0].entityName, FieldType.Relationship }));
            }

            //Expand the attribute for n:1
            ExpandNToOneNodeForFilters(nTo1Relationship.FieldId.ToString());
            _customEntityViewsUIMap = new CustomEntityViewsUIMap();
            _customEntityViewsUIMap.ClickBaseTableField("  Account Number");

            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            //set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            //set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            //set criteria as like and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Like);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NotLike);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();

            #region add all filters required
            string conditionType = string.Empty;
            int stringFilterCount = 5;
            var filtertosave = _customEntities[0].view[0].filters[0];
            StringBuilder expectedFilterPaneText = new StringBuilder("Selected Filters \r\n \r\n");
            ///loop through filters for standard text and create all
            for (int filterindexer = 0; filterindexer < stringFilterCount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                _customEntityViewsUIMap.ClickBaseTableField("  Account Number");
                expectedFilterPaneText.Append("Account Number");
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditionType = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedFilterPaneText.Append(conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditionType;

                if (filtertosave._valueOne != string.Empty)
                {
                    expectedFilterPaneText.Append(filtertosave._valueOne);
                    viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
                }
                else
                {
                    expectedFilterPaneText.Append("-");
                }
                if (filtertosave._valueTwo != string.Empty)
                {
                    expectedFilterPaneText.Append(" and ");
                    viewcontrols.textFilterValue1Txt.Text = filtertosave._valueTwo;
                    expectedFilterPaneText.Append(filtertosave._valueTwo);
                }
                expectedFilterPaneText.Append("Account Number");
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedFilterPaneText.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
        }
        #endregion

        #region 41463 Successfully add filter on base table field of type number
        /// <summary>
        /// Custom entity views - Ensures that a filter can be applied to a base table attribute of type number (N / I).
        /// The values are hardcoded since the fieldid is static and thus doesn't change. This test uses the 
        /// Employee ID to ensure its working.
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFilterOnBaseTableFieldOfTypeNumber_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            //Navigate to the CE Admin page
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            //click filters tab
            _customEntityViewsUIMap.ClickFiltersTab();

            //Expand the N:1 relationship to employees
            CustomEntitiesUtilities.CustomEntityNtoOneAttribute nTo1Relationship = null;
            foreach (var attribute in _customEntities[0].attribute)
            {

                if (attribute._fieldType == FieldType.Relationship && attribute.DisplayName == "N : 1 Relationship to Base Table")
                {
                    nTo1Relationship = attribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                }
            }
            //Throw exception when attribute is NULL!! - can't be found within the list of attributes for CE.
            if (nTo1Relationship == null)
            {
                throw new Exception(String.Format("Can't find an attribute in {0} that is of type {1}", new object[] { _customEntities[0].entityName, FieldType.Relationship }));
            }

            //Expand the attribute for n:1
            ExpandNToOneNodeForFilters(nTo1Relationship.FieldId.ToString());

            _customEntityViewsUIMap = new CustomEntityViewsUIMap();
            _customEntityViewsUIMap.ClickBaseTableField("  Current Reference Number");

            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));


            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than or equal to and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than or and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1 + ViewModalMessages.EmptyFieldForNumber2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            string conditiontype = string.Empty;
            int numberfiltercount = 7;
            var filtertosave = _customEntities[0].view[0].filters[0];
            ///loop through filters for number and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                expectedSelectedFilterValue.Append("Creation Method");
                filtertosave = _customEntities[0].view[0].filters[filterindexer];

                _customEntityViewsUIMap.ClickBaseTableField("  Creation Method");

                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty)
                {
                    viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
                    expectedSelectedFilterValue.Append(filtertosave._valueOne);
                }
                if (filtertosave._valueTwo != string.Empty)
                {
                    expectedSelectedFilterValue.Append(" and ");
                    viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo;
                    expectedSelectedFilterValue.Append(filtertosave._valueTwo);
                }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
                expectedSelectedFilterValue.Append("Creation Method");
            }

            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
        }

        #endregion

        #region 41474 Successfully add filter on base table field of type checkbox
        /// <summary>
        /// Custom entity views - Ensures that a filter can be applied to a base table attribute of type checkbox (X).
        /// The values are hardcoded since the fieldid is static and thus doesn't change. This test uses the 
        /// Active field to ensure its working.
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFilterOnBaseTableFieldOfTypeCheckBox_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region   #region navigate to entity -> view -> filters tab
            //Navigate to the CE Admin page
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            //click filters tab
            _customEntityViewsUIMap.ClickFiltersTab();

            #endregion

            #region drag required attributes and build required controls
            //Expand the N:1 relationship to employees
            CustomEntitiesUtilities.CustomEntityNtoOneAttribute nTo1Relationship = null;
            foreach (var attribute in _customEntities[0].attribute)
            {

                if (attribute._fieldType == FieldType.Relationship && attribute.DisplayName == "N : 1 Relationship to Base Table")
                {
                    nTo1Relationship = attribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                }
            }
            //Throw exception when attribute is NULL!! - can't be found within the list of attributes for CE.
            if (nTo1Relationship == null)
            {
                throw new Exception(String.Format("Can't find an attribute in {0} that is of type {1}", new object[] { _customEntities[0].entityName, FieldType.Relationship }));
            }

            //Expand the attribute for n:1
            ExpandNToOneNodeForFilters(nTo1Relationship.FieldId.ToString());

            _customEntityViewsUIMap = new CustomEntityViewsUIMap();
            _customEntityViewsUIMap.ClickBaseTableField("  Active");

            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));

            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyDropDownForYesOrNo, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyDropDownForYesOrNo, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            string conditiontype = string.Empty;
            int numberfiltercount = 2;
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            ///loop through filters for number and create all
            for (int filterindexer = 12; filterindexer < numberfiltercount + 12; filterindexer++)
            {
                expectedSelectedFilterValue.Append("Active");
                var filtertosave = _customEntities[0].view[0].filters[filterindexer];
                _customEntityViewsUIMap.ClickBaseTableField("  Active");
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);

                string yesNoValue = EnumHelper.GetEnumDescription((YesOrNo)Convert.ToInt32(filtertosave._valueOne));
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                expectedSelectedFilterValue.Append(conditiontype);
                viewcontrols.cmbFilterOption.SelectedItem = yesNoValue;
                expectedSelectedFilterValue.Append(yesNoValue);
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
                expectedSelectedFilterValue.Append("Active");
            }
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }

        #endregion

        #region 41470 Successfully add filter on base table field of type date / datetime
        /// <summary>
        /// Custom entity views - Ensures that a filter can be applied to a base table attribute of type checkbox (X).
        /// The values are hardcoded since the fieldid is static and thus doesn't change. This test uses the 
        /// Active field to ensure its working.
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFilterOnBaseTableFieldOfTypeDateTime_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region   #region navigate to entity -> view -> filters tab
            //Navigate to the CE Admin page
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            //click filters tab
            _customEntityViewsUIMap.ClickFiltersTab();

            #endregion

            #region drag required attributes and build required controls
            //Expand the N:1 relationship to employees
            CustomEntitiesUtilities.CustomEntityNtoOneAttribute nTo1Relationship = null;
            foreach (var attribute in _customEntities[0].attribute)
            {

                if (attribute._fieldType == FieldType.Relationship && attribute.DisplayName == "N : 1 Relationship to Base Table")
                {
                    nTo1Relationship = attribute as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;
                }
            }
            //Throw exception when attribute is NULL!! - can't be found within the list of attributes for CE.
            if (nTo1Relationship == null)
            {
                throw new Exception(String.Format("Can't find an attribute in {0} that is of type {1}", new object[] { _customEntities[0].entityName, FieldType.Relationship }));
            }

            //Expand the attribute for n:1
            ExpandNToOneNodeForFilters(nTo1Relationship.FieldId.ToString());

            _customEntityViewsUIMap = new CustomEntityViewsUIMap();
            _customEntityViewsUIMap.ClickBaseTableField("  Hire Date");

            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));

            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.On);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as not on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NotOn);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.After);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Before);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrAfter);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrBefore);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1 + ViewModalMessages.EmptyFieldForDate2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x days and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXDays);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x days and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXDays);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x weeks and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXWeeks);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x weeks and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXWeeks);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x months and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXMonths);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x months and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXMonths);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x years and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXYears);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x years and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXYears);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion

            #region add all filters required
            string conditiontype = string.Empty;
            int numberfiltercount = 38;
            CustomEntitiesUtilities.CustomEntityViewFilter filtertosave;
            StringBuilder expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");

            ///loop through filters for number and create all
            for (int filterindexer = 14; filterindexer < numberfiltercount + 14; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                expectedSelectedFilterValue.Append("Hire Date");
                _customEntityViewsUIMap.ClickBaseTableField("  Hire Date");
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                expectedSelectedFilterValue.Append(conditiontype);
                if (filtertosave._valueOne != string.Empty)
                {
                    string DateTime1 = filtertosave._valueOne;
                    string[] DateAndTime1 = Regex.Split(DateTime1, " ");
                    if (DateAndTime1.Length > 1)
                    {
                        viewcontrols.textFilterValue1Txt.Text = DateAndTime1[0];
                        expectedSelectedFilterValue.Append(DateAndTime1[0]);
                    }
                    else
                    {
                        viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
                        expectedSelectedFilterValue.Append(filtertosave._valueOne);
                    }
                    if (filtertosave._valueTwo != string.Empty)
                    {
                        expectedSelectedFilterValue.Append(" and ");
                        string DateTime2 = filtertosave._valueTwo;
                        string[] DateAndTime2 = Regex.Split(DateTime2, " ");
                        if (DateAndTime2.Length > 1)
                        {
                            viewcontrols.textFilterValue2Txt.Text = DateAndTime2[0];
                            expectedSelectedFilterValue.Append(DateAndTime2[0]);
                        }
                        else
                        {
                            viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo;
                            expectedSelectedFilterValue.Append(filtertosave._valueTwo);
                        }
                    }
                }
                else { expectedSelectedFilterValue.Append("-"); };
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
                expectedSelectedFilterValue.Append("Hire Date");
            }
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion

        #region 41465 Successfully add filter on base Table field of type list/gen list
        /// <summary>
        /// Successfully adds filter on base table field of type genlist / lists
        /// 
        /// SELECT fields_base.description, dbo.fields_base.fieldtype, dbo.tables_base.tablename, dbo.fields_base.genlist, dbo.fields_base.valuelist
        /// FROM dbo.fields_base INNER JOIN
        /// dbo.tables_base ON dbo.fields_base.tableid = dbo.tables_base.tableid
        ///WHERE (dbo.fields_base.valuelist = 1);
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFilterOnBaseTableFieldOfTypeGenList_UITest()
        {
            if (_executingProduct == ProductType.expenses)
            {

                #region setup test
                ImportDataToTestingDatabase(testContextInstance.TestName);
                Dictionary<string, bool> filterCriteria = new Dictionary<string, bool>()
                {
                    {EnumHelper.GetEnumDescription(ConditionType.Equals), true}, 
                    {EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual), true},
                    {EnumHelper.GetEnumDescription(ConditionType.ContainsData), false},
                    {EnumHelper.GetEnumDescription(ConditionType.DoesNotContainData), false}
                };
                CustomEntitiesUtilities.CustomEntityNtoOneAttribute updateatt = new CustomEntitiesUtilities.CustomEntityNtoOneAttribute();
                updateatt._attributeid = 0;
                updateatt.DisplayName = "N:1 to Relationship To Cars";
                updateatt._description = "desc";
                updateatt._fieldType = FieldType.Relationship;
                System.Guid carsBaseTableID = new Guid("A184192F-74B6-42F7-8FDB-6DCF04723CEF");
                updateatt._relatedTable = carsBaseTableID;
                System.Guid carsModel = new Guid("99A078D9-F82C-4474-BDDE-6701D4BD51EA");
                updateatt._relationshipdisplayfield = carsModel;
                updateatt.FieldId = carsModel;
                updateatt._maxRows = 2;
                updateatt._createdBy = AutoTools.GetEmployeeIDByUsername(_executingProduct);
                List<CustomEntitiesUtilities.RelationshipMatchFieldListItem> rmf = new List<CustomEntitiesUtilities.RelationshipMatchFieldListItem>();
                rmf.Add(new CustomEntitiesUtilities.RelationshipMatchFieldListItem(0, carsModel, 0));
                updateatt._matchFieldListItems = rmf;
                int attributeId = CustomEntitiesUtilities.CreateCustomEntityRelationship(_customEntities[0], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)updateatt, _customEntities[1], _executingProduct);
                Assert.IsTrue(attributeId > 0);
                //CacheUtilities.DeleteCachedTablesAndFields();
                #endregion

                #region navigate to CE -> Views -> Filters Tab
                _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

                _customEntityViewsUIMap.ClickViewsLink();

                _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

                _customEntityViewsUIMap.ClickFiltersTab();
                #endregion

                #region find attribute and move
                string NtoOneToBaseId = updateatt.FieldId.ToString();
                ExpandNToOneNodeForFilters(NtoOneToBaseId);
                //_customEntityViewsUIMap.ExpandNToOneToCarsNode();

                //select the attribute for n:1
                //_customEntityViewsUIMap.ClickVehicleEngineType();
                SelectNTo1RelationshipForColumn(NtoOneToBaseId, "24172542-3e15-4fca-b4f5-d7ffef9eed4e");

                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

                #endregion

                #region Validate all validation modals for type list

                CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));

                //leave criteria blank and validate message on save
                viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
                Keyboard.SendKeys(FormGlobalProperties.TAB);
                Keyboard.SendKeys(FormGlobalProperties.ENTER);
                _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
                _customEntityViewsUIMap.ValidateMessageModal();
                Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

                viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
                _customEntityViewsUIMap.ClickSaveOnListFilterModal();
                _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyListSelecton, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
                _customEntityViewsUIMap.ValidateMessageModal();
                Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

                viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
                _customEntityViewsUIMap.ClickSaveOnListFilterModal();
                _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyListSelecton, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
                _customEntityViewsUIMap.ValidateMessageModal();
                Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
                Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
                #endregion

                #region Add all filter types
                List<string> engineTypes = new List<string>()
            {   
                "Petrol", "Diesel", "LPG"
            };

                StringBuilder expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
                foreach (KeyValuePair<string, bool> fCriteria in filterCriteria)
                {
                    expectedSelectedFilterValue.Append("Vehicle Engine Typ...");
                    _customEntityViewsUIMap.ClickVehicleEngineType();
                    _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

                    viewcontrols.filterCriteriaOption.SelectedItem = fCriteria.Key;
                    expectedSelectedFilterValue.Append(fCriteria.Key);
                    //If flag is true - means that it needs valid list items
                    if (fCriteria.Value)
                    {
                        foreach (string eType in engineTypes)
                        {
                            _customEntityViewsUIMap.MoveSelectedListItem(eType);
                        }
                        expectedSelectedFilterValue.Append("Petrol and 2 others");
                    }
                    else
                    {
                        expectedSelectedFilterValue.Append("-");
                    }
                    expectedSelectedFilterValue.Append("Vehicle Engine Type");
                    _customEntityViewsUIMap.ClickSaveOnListFilterModal();
                }
                #endregion

                #region save, edit and verify results
                _customEntityViewsUIMap.ClickSaveOnViewModal();

                //Click edit against the CE 
                _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

                _customEntityViewsUIMap.ClickFiltersTab();

                _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
                _customEntityViewsUIMap.AssertSelectedFilters();
                #endregion
            }
        }

        #endregion
        #endregion

        #region apply filters to udfs
        #region 41452 successfully add filter on UDF of type standard text for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeStandardTextForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;

            //expand n to one to base table
            string NtoOneToBaseId = _customEntities[0].attribute[33].FieldId.ToString();
            ExpandNToOneNodeForFilters(NtoOneToBaseId);

            //expand udf folder
            ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
            var filtertosave = _customEntities[0].view[0].filters[0];

            //drag udf to filter area
            SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed

            //Add all filter criterias and their failure message
            Dictionary<ConditionType, string> filterConditionErrorMessageMap = new Dictionary<ConditionType, string>()
            {
                {ConditionType.None,  string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) })},
                {ConditionType.Equals, string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) })},
                {ConditionType.DoesNotEqual, string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) })},
                {ConditionType.Like, string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) })}
            };

            //Loops through each kvp and assserts modal message is correct
            foreach (KeyValuePair<ConditionType, string> iterator in filterConditionErrorMessageMap)
            {
                _customEntityViewsUIMap.VerifyFilterValidationMessageOnSave(viewcontrols, iterator.Key, iterator.Value);
            }

            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int stringfiltercount = 5;
            ///loop through filters for standard text and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 0; filterindexer < stringfiltercount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
                string attributename = TruncateDisplayName(_userDefinedFields[0].DisplayName);
                expectedSelectedFilterValue.Append(attributename);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty)
                {
                    viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; expectedSelectedFilterValue.Append(filtertosave._valueOne);
                    if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo; expectedSelectedFilterValue.Append(" and "); expectedSelectedFilterValue.Append(filtertosave._valueTwo); }
                }
                else { expectedSelectedFilterValue.Append("-"); }
                expectedSelectedFilterValue.Append(_userDefinedFields[0].DisplayName);
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion

        #region 41457 successfully add filter on UDF of type large text for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeLargeTextForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            for (int filterindexer = 0; filterindexer < 5; filterindexer++)
            {
                _customEntities[0].view[0].filters[filterindexer]._fieldid = _userDefinedFields[6].FieldId;
            }
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;

            //expand n to one to base table
            string NtoOneToBaseId = _customEntities[0].attribute[33].FieldId.ToString();
            ExpandNToOneNodeForFilters(NtoOneToBaseId);

            //expand udf folder
            ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
            var filtertosave = _customEntities[0].view[0].filters[0];

            //drag udf to filter area
            SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            Dictionary<ConditionType, string> filterConditionErrorMessageMap = new Dictionary<ConditionType, string>()
            {
                {ConditionType.None,  string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) })},
                {ConditionType.Equals, string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) })},
                {ConditionType.DoesNotEqual, string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) })},
                {ConditionType.Like, string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForValue1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) })}

            };

            //Loops through each kvp and assserts modal message is correct
            foreach (KeyValuePair<ConditionType, string> iterator in filterConditionErrorMessageMap)
            {
                _customEntityViewsUIMap.VerifyFilterValidationMessageOnSave(viewcontrols, iterator.Key, iterator.Value);
            }
            #endregion
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();

            #region add all filters required
            conditiontype = string.Empty;
            int stringfiltercount = 5;
            ///loop through filters for standard text and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 0; filterindexer < stringfiltercount; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
                string attributename = TruncateDisplayName(_userDefinedFields[6].DisplayName);
                expectedSelectedFilterValue.Append(attributename);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty)
                {
                    viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; expectedSelectedFilterValue.Append(filtertosave._valueOne);
                    if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo; expectedSelectedFilterValue.Append(" and "); expectedSelectedFilterValue.Append(filtertosave._valueTwo); }
                }
                else { expectedSelectedFilterValue.Append("-"); }
                expectedSelectedFilterValue.Append(_userDefinedFields[6].DisplayName);
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion

        #region 41450 successfully add filter on UDF of type number for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeNumberForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;

            //expand n to one to base table
            string NtoOneToBaseId = _customEntities[0].attribute[33].FieldId.ToString();
            ExpandNToOneNodeForFilters(NtoOneToBaseId);

            //expand udf folder
            ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
            var filtertosave = _customEntities[0].view[0].filters[5];

            //drag udf to filter area
            SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than or equal to and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than or and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForNumber1 + ViewModalMessages.EmptyFieldForNumber2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 7;
            ///loop through filters for number and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
                string attributename = TruncateDisplayName(_userDefinedFields[2].DisplayName);
                expectedSelectedFilterValue.Append(attributename);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; expectedSelectedFilterValue.Append(filtertosave._valueOne); }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo; expectedSelectedFilterValue.Append(" and "); expectedSelectedFilterValue.Append(filtertosave._valueTwo); }
                expectedSelectedFilterValue.Append(_userDefinedFields[2].DisplayName);
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion

        #region 41450 successfully add filter on UDF of type decimal for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeDecimalForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._fieldid = _userDefinedFields[3].FieldId;
                if (_customEntities[0].view[0].filters[filterindex]._valueOne != string.Empty) { _customEntities[0].view[0].filters[filterindex]._valueOne += ".000"; }
                if (_customEntities[0].view[0].filters[filterindex]._valueTwo != string.Empty) { _customEntities[0].view[0].filters[filterindex]._valueTwo += ".000"; }
            }
            string conditiontype = string.Empty;

            //expand n to one to base table
            string NtoOneToBaseId = _customEntities[0].attribute[33].FieldId.ToString();
            ExpandNToOneNodeForFilters(NtoOneToBaseId);

            //expand udf folder
            ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
            var filtertosave = _customEntities[0].view[0].filters[5];

            //drag udf to filter area
            SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than or equal to and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than or and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDecimal1 + ViewModalMessages.EmptyFieldForDecimal2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 7;
            ///loop through filters for number and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
                string attributename = TruncateDisplayName(_userDefinedFields[3].DisplayName);
                expectedSelectedFilterValue.Append(attributename);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; expectedSelectedFilterValue.Append(filtertosave._valueOne); }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo; expectedSelectedFilterValue.Append(" and "); expectedSelectedFilterValue.Append(filtertosave._valueTwo); }
                expectedSelectedFilterValue.Append(_userDefinedFields[3].DisplayName);
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion

            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._valueOne = _customEntities[0].view[0].filters[filterindex]._valueOne.Replace(".000", "");
                _customEntities[0].view[0].filters[filterindex]._valueTwo = _customEntities[0].view[0].filters[filterindex]._valueTwo.Replace(".000", "");
            }
        }
        #endregion

        #region 41450 successfully add filter on UDF of type currency for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeCurrencyForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._fieldid = _userDefinedFields[4].FieldId;
                if (_customEntities[0].view[0].filters[filterindex]._valueOne != string.Empty) { _customEntities[0].view[0].filters[filterindex]._valueOne += ".00"; }
                if (_customEntities[0].view[0].filters[filterindex]._valueTwo != string.Empty) { _customEntities[0].view[0].filters[filterindex]._valueTwo += ".00"; }
            }
            string conditiontype = string.Empty;

            //expand n to one to base table
            string NtoOneToBaseId = _customEntities[0].attribute[33].FieldId.ToString();
            ExpandNToOneNodeForFilters(NtoOneToBaseId);

            //expand udf folder
            ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
            var filtertosave = _customEntities[0].view[0].filters[5];

            //drag udf to filter area
            SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThan);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as greater than or equal to and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.GreaterThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as less than or and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LessThanEqualTo);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForCurrency1 + ViewModalMessages.EmptyFieldForCurrency2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 7;
            ///loop through filters for number and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 5; filterindexer < numberfiltercount + 5; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
                string attributename = TruncateDisplayName(_userDefinedFields[4].DisplayName);
                expectedSelectedFilterValue.Append(attributename);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty) { viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne; expectedSelectedFilterValue.Append(filtertosave._valueOne); }
                if (filtertosave._valueTwo != string.Empty) { viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo; expectedSelectedFilterValue.Append(" and "); expectedSelectedFilterValue.Append(filtertosave._valueTwo); }
                expectedSelectedFilterValue.Append(_userDefinedFields[4].DisplayName);
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion

            for (int filterindex = 5; filterindex < 12; filterindex++)
            {
                _customEntities[0].view[0].filters[filterindex]._valueOne = _customEntities[0].view[0].filters[filterindex]._valueOne.Replace(".00", "");
                _customEntities[0].view[0].filters[filterindex]._valueTwo = _customEntities[0].view[0].filters[filterindex]._valueTwo.Replace(".00", "");
            }
        }
        #endregion

        #region 41473 successfully add filter on UDF of type checkbox for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeCheckboxForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;

            //expand n to one to base table
            string NtoOneToBaseId = _customEntities[0].attribute[33].FieldId.ToString();
            ExpandNToOneNodeForFilters(NtoOneToBaseId);

            //expand udf folder
            ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
            string yesNoValue = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[12];
            //drag udf to filter area
            SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyDropDownForYesOrNo, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyDropDownForYesOrNo, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();
            _customEntityViewsUIMap.PressCancelOnViewFilterModal();
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 2;
            ///loop through filters for number and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 12; filterindexer < numberfiltercount + 12; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
                string attributename = TruncateDisplayName(_userDefinedFields[5].DisplayName);
                expectedSelectedFilterValue.Append(attributename);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                yesNoValue = EnumHelper.GetEnumDescription((YesOrNo)Convert.ToInt32(filtertosave._valueOne));
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                viewcontrols.cmbFilterOption.SelectedItem = yesNoValue;
                expectedSelectedFilterValue.Append(yesNoValue);
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
                expectedSelectedFilterValue.Append(_userDefinedFields[5].DisplayName);
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion

        #region 41471 successfully add filter on UDF of type date for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeDateForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;

            //expand n to one to base table
            string NtoOneToBaseId = _customEntities[0].attribute[33].FieldId.ToString();
            ExpandNToOneNodeForFilters(NtoOneToBaseId);

            //expand udf folder
            ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
            var filtertosave = _customEntities[0].view[0].filters[14];

            //drag udf to filter area
            SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.On);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as not on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NotOn);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.After);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Before);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrAfter);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrBefore);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDate1 + ViewModalMessages.EmptyFieldForDate2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x days and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXDays);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x days and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXDays);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForDays, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x weeks and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXWeeks);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x weeks and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXWeeks);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForWeeks, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x months and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXMonths);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x months and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXMonths);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForMonths, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as last x years and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.LastXYears);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as next x years and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.NextXYears);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForYears, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 38;
            ///loop through filters for number and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 14; filterindexer < numberfiltercount + 14; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
                string attributename = TruncateDisplayName(_userDefinedFields[7].DisplayName);
                expectedSelectedFilterValue.Append(attributename);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty)
                {
                    string DateTime1 = filtertosave._valueOne;
                    string[] DateAndTime1 = Regex.Split(DateTime1, " ");
                    if (DateAndTime1.Length > 1)
                    {
                        viewcontrols.textFilterValue1Txt.Text = DateAndTime1[0];
                        expectedSelectedFilterValue.Append(DateAndTime1[0]);
                    }
                    else
                    {
                        viewcontrols.textFilterValue1Txt.Text = filtertosave._valueOne;
                        expectedSelectedFilterValue.Append(filtertosave._valueOne);
                    }

                    if (filtertosave._valueTwo != string.Empty)
                    {
                        string DateTime2 = filtertosave._valueTwo;
                        string[] DateAndTime2 = Regex.Split(DateTime2, " ");
                        if (DateAndTime2.Length > 1)
                        {
                            viewcontrols.textFilterValue2Txt.Text = DateAndTime2[0];
                            expectedSelectedFilterValue.Append(" and ");
                            expectedSelectedFilterValue.Append(DateAndTime2[0]);
                        }
                        else
                        {
                            viewcontrols.textFilterValue2Txt.Text = filtertosave._valueTwo;
                            expectedSelectedFilterValue.Append(" and ");
                            expectedSelectedFilterValue.Append(filtertosave._valueTwo);
                        }
                    }
                }
                else { expectedSelectedFilterValue.Append("-"); }
                expectedSelectedFilterValue.Append(_userDefinedFields[7].DisplayName);
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion

        #region 41458 successfully add filter on UDF of type time for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeTimeForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;

            //expand n to one to base table
            string NtoOneToBaseId = _customEntities[0].attribute[33].FieldId.ToString();
            ExpandNToOneNodeForFilters(NtoOneToBaseId);

            //expand udf folder
            ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
            var filtertosave = _customEntities[0].view[0].filters[52];

            //drag udf to filter area
            SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as not on and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.After);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Before);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or after and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrAfter);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as on or before and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.OnOrBefore);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            ///set criteria as between and validate message on save
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Between);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForTime1 + ViewModalMessages.EmptyFieldForTime2, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 8;
            ///loop through filters for number and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 52; filterindexer < numberfiltercount + 52; filterindexer++)
            {
                filtertosave = _customEntities[0].view[0].filters[filterindexer];
                SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", filtertosave._fieldid.ToString());
                string attributename = TruncateDisplayName(_userDefinedFields[9].DisplayName);
                expectedSelectedFilterValue.Append(attributename);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                if (filtertosave._valueOne != string.Empty)
                {
                    viewcontrols.timeFilterValue1Txt.Text = filtertosave._valueOne;
                    expectedSelectedFilterValue.Append(filtertosave._valueOne);
                    if (filtertosave._valueTwo != string.Empty) { viewcontrols.timeFilterValue2Txt.Text = filtertosave._valueTwo; expectedSelectedFilterValue.Append(" and "); expectedSelectedFilterValue.Append(filtertosave._valueTwo); }
                }
                else { expectedSelectedFilterValue.Append("-"); }
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
                expectedSelectedFilterValue.Append(_userDefinedFields[9].DisplayName);
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion

        #region susseccfully add filter on UDF of type list for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeListForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region locate the List UDF
            UserDefinedFieldTypeList listUDF = null;
            //find the list udf
            foreach(var udf in _userDefinedFields) 
            {
                if(udf.GetType() == typeof(UserDefinedFieldTypeList))
                {
                    listUDF = (UserDefinedFieldTypeList)udf;
                    break;
                }
            }

            if (listUDF == null) throw new ArgumentException(String.Format("CustomEntity {0} does not contain a UDF of type list. Test will halt!", _customEntities[0].entityName));
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;

            //expand n to one to base table
            string NtoOneToBaseId = _customEntities[0].attribute[33].FieldId.ToString();
            ExpandNToOneNodeForFilters(NtoOneToBaseId);

            //expand udf folder
            ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
           

            //drag udf to filter area
            SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62",listUDF.FieldId.ToString());
            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            Dictionary<string, bool> filterCriteria = new Dictionary<string, bool>()
            {
                {EnumHelper.GetEnumDescription(ConditionType.Equals), true}, 
                {EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual), true},
                {EnumHelper.GetEnumDescription(ConditionType.ContainsData), false},
                {EnumHelper.GetEnumDescription(ConditionType.DoesNotContainData), false}
            };

            #region Verify error modals
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            Keyboard.SendKeys(FormGlobalProperties.TAB);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyListSelecton, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyListSelecton, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion

            StringBuilder expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
          
            foreach (KeyValuePair<string, bool> fCriteria in filterCriteria)
            {  
                string firstListItem = null;
                int totalNumberOfListItems = 0;
                expectedSelectedFilterValue.Append(listUDF.DisplayName);
          //      ExpandNToOneNodeForFilters(NtoOneToBaseId);

                //expand udf folder
        //        ExpandUDFNodeInAvailableFieldsForFilters(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62");
                SelectUDFFieldForFilter(NtoOneToBaseId, "972ac42d-6646-4efc-9323-35c2c9f95b62", listUDF.FieldId.ToString());
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                viewcontrols.filterCriteriaOption.SelectedItem = fCriteria.Key;
                expectedSelectedFilterValue.Append(fCriteria.Key);
                //If flag is true - means that it needs valid list items
                if (fCriteria.Value)
                {
                    foreach (CustomEntitiesUtilities.EntityListItem eType in listUDF.UserDefinedFieldListItems)
                    {
                        if (string.IsNullOrEmpty(firstListItem))
                        {
                            firstListItem = eType._textItem;
                        }
                        else
                        {
                            totalNumberOfListItems++;
                        }
                        _customEntityViewsUIMap.MoveSelectedListItem(eType._textItem);
                    }
                    expectedSelectedFilterValue.Append(string.Format("{0} and {1} others", new object[] {firstListItem, totalNumberOfListItems}));
                    
                }
                else
                {
                    expectedSelectedFilterValue.Append("-");
                }
                expectedSelectedFilterValue.Append(listUDF.DisplayName);
                _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            }

            #region save, edit and verify results
            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion

        }
        #endregion
        #endregion

        #region 40176 successfully add two filter of the same type on the same attribute for view in custom entity
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddTwoFiltersOfTheSameTypeOnTheSameAttribForViewInCustomEntity_UITest()
        {
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            string yesNoValue = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[12];
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region add all filters required
            conditiontype = string.Empty;
            int numberfiltercount = 2;
            ///loop through filters for number and create all
            StringBuilder expectedSelectedFilterValue = new StringBuilder();
            expectedSelectedFilterValue.Append("Selected Filters \r\n \r\n");
            for (int filterindexer = 12; filterindexer < numberfiltercount + 12; filterindexer++)
            {
                string attributename = TruncateDisplayName(_customEntities[0].attribute[12].DisplayName);
                expectedSelectedFilterValue.Append(attributename);
                SelectAvailableFieldForFilter(filtertosave._fieldid.ToString());
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
                if (viewcontrols == null)
                {
                    viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
                }
                conditiontype = EnumHelper.GetEnumDescription((ConditionType)filtertosave._conditionType);
                expectedSelectedFilterValue.Append(conditiontype);
                yesNoValue = EnumHelper.GetEnumDescription((YesOrNo)Convert.ToInt32(filtertosave._valueOne));
                viewcontrols.filterCriteriaOption.SelectedItem = conditiontype;
                viewcontrols.cmbFilterOption.SelectedItem = yesNoValue;
                expectedSelectedFilterValue.Append(yesNoValue);
                _customEntityViewsUIMap.PressSaveOnViewFilterModal();
                expectedSelectedFilterValue.Append(_customEntities[0].attribute[12].DisplayName);
            }
            #endregion

            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion

        #region 41487 Custom Entity views : Successfully delete attribute whilst in use by a filter in view
        /// <summary>
        /// Successfully deletes filters that are added to the view by deleting the actual attribute from the
        /// attributes page and asserts the results
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestCategory("Debug"), TestMethod]
        public void CustomEntityViewsSuccessfullyDeleteAttributeWhenInUseByFilterForViewInCustomEntity_UITest()
        {
            #region test setup
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            //Ensure valid filters exist otherwise throw exception and halt test execution
            if (_enabledFilters.Count == 0)
            {
                //Throw exception and fail test
                throw new ArgumentException("No filters set up, test cannot progress!");
            }
            CustomEntityAttributesUIMap customEntityAttributeUIMap = new CustomEntityAttributesUIMap();
            CustomEntitiesUtilities.CustomEntityAttribute attributeToDelete = _customEntities[0].attribute[0];
            #endregion

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            Keyboard.SendKeys("{Esc}");
            #endregion

            #region Delete attribute and verify the results
            List<CustomEntitiesUtilities.CustomEntityAttribute> listOfDeletedAttributes = new List<CustomEntitiesUtilities.CustomEntityAttribute>();
            foreach (KeyValuePair<CustomEntitiesUtilities.CustomEntityViewFilter, CustomEntitiesUtilities.CustomEntityAttribute> filterCriterias in _enabledFilters)
            {
                #region Delete attribute(s)
                if (!listOfDeletedAttributes.Contains(filterCriterias.Value))
                {
                    customEntityAttributeUIMap.ClickAttributesLink();
                    _customEntityViewsUIMap.FilterAttributesParams.UIDisplayNameEditText = filterCriterias.Value.DisplayName;
                    _customEntityViewsUIMap.FilterAttributes();

                    Keyboard.SendKeys("{Enter}"); 

                    customEntityAttributeUIMap.ClickDeleteFieldLink(filterCriterias.Value.DisplayName);

                    customEntityAttributeUIMap.PressOKToConfirmAttributeDeletion();

                    listOfDeletedAttributes.Add(filterCriterias.Value);

                }
                #endregion

                #region verify that attribute is no longer used in filter

                _customEntityViewsUIMap.ClickViewsLink();

                _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

                _customEntityViewsUIMap.ClickFiltersTab();

                StringBuilder sb = new StringBuilder("Selected Filters \r\n \r\n");
                
                foreach (var expectedFilterOnView in _enabledFilters)
                {
                    if(!listOfDeletedAttributes.Contains(expectedFilterOnView.Value))
                    {
                        CustomEntitiesUtilities.CustomEntityViewFilter filter = expectedFilterOnView.Key;
                        sb.Append(expectedFilterOnView.Value.DisplayName);
                        sb.Append(EnumHelper.GetEnumDescription((ConditionType)filter._conditionType));
                        if (expectedFilterOnView.Value._fieldType == FieldType.TickBox)
                        {
                            sb.Append(EnumHelper.GetEnumDescription((YesOrNo)Convert.ToUInt32(filter._valueOne)));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(filter._valueOne))
                            {
                                sb.Append(filter._valueOne);
                            }
                            else { sb.Append("-"); }
                            if (!string.IsNullOrEmpty(filter._valueTwo))
                            {
                                sb.Append(" and ");
                                sb.Append(filter._valueTwo);
                            }
                        }
                        sb.Append(expectedFilterOnView.Value.DisplayName);
                    }
                  
                }  
                if ( listOfDeletedAttributes.Count == 5)
                    {
                        sb.Append("There are no filters selected.");
                    }
                #endregion

                #region Assert filter pane
                _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = sb.ToString();
                _customEntityViewsUIMap.AssertSelectedFilters();

                Keyboard.SendKeys("{Esc}");
                #endregion
            }
            #endregion
           
        }

        #endregion

        #region 41495 Edit attributes when in use by a filter
        /// <summary>
        /// 41495 Successfully Edit attribute when in use as Filters for GreenLight View
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestCategory("Debug"), TestMethod]
        public void CustomEntityViewsSuccessfullyEditAttributesWhenInUseOnFiltersForViewInCustomEntity_UITest()
        {
            #region test setup
            _sharedMethods.RestoreDefaultSortingOrder("gridAttributes", _executingProduct);
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);

            //Ensure valid filters exist otherwise throw exception and halt test execution
            if (_enabledFilters.Count == 0)
            {
                //Throw exception and fail test
                throw new ArgumentException("No filters set up, test cannot progress!");
            }

            CustomEntityAttributesUIMap customEntityAttributeUIMap = new CustomEntityAttributesUIMap();

            #endregion

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            Keyboard.SendKeys("{Esc}");
            #endregion


            List<CustomEntitiesUtilities.CustomEntityAttribute> listOfEditedAttributes = new List<CustomEntitiesUtilities.CustomEntityAttribute>();
            List<CustomEntitiesUtilities.CustomEntityAttribute> orignalAttributes = new List<CustomEntitiesUtilities.CustomEntityAttribute>();
            
            int counter = 0;
            foreach (KeyValuePair<CustomEntitiesUtilities.CustomEntityViewFilter, CustomEntitiesUtilities.CustomEntityAttribute> filterCriterias in _enabledFilters)
            {
                #region Edit attribute(s)
                if (!listOfEditedAttributes.Contains(filterCriterias.Value))
                {
                    customEntityAttributeUIMap.ClickAttributesLink();
                    _customEntityViewsUIMap.FilterAttributesParams.UIDisplayNameEditText = filterCriterias.Value.DisplayName;
                    _customEntityViewsUIMap.FilterAttributes();

                    Keyboard.SendKeys("{Enter}");

                    customEntityAttributeUIMap.ClickEditFieldLink(filterCriterias.Value.DisplayName);

                    CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(customEntityAttributeUIMap);

                    textAttribute.DisplayNameTxt.Text = "Bob" + counter;

                    customEntityAttributeUIMap.PressSaveAttribute();

                    orignalAttributes.Add(ExtensionMethods.DeepClone<CustomEntitiesUtilities.CustomEntityAttribute>( filterCriterias.Value));

                    filterCriterias.Value.DisplayName = "Bob" + counter;

                    listOfEditedAttributes.Add(filterCriterias.Value);

                    counter++;
                }
                #endregion

                #region verify that attribute displays new name

                _customEntityViewsUIMap.ClickViewsLink();

                _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

                _customEntityViewsUIMap.ClickFiltersTab();

                StringBuilder sb = new StringBuilder("Selected Filters \r\n \r\n");

                foreach (var expectedFilterOnView in _enabledFilters)
                {
                    CustomEntitiesUtilities.CustomEntityAttribute attribute;

                    CustomEntitiesUtilities.CustomEntityViewFilter filter = expectedFilterOnView.Key;
                    if (listOfEditedAttributes.Contains(expectedFilterOnView.Value))
                    {
                        int indexOfAttribute = listOfEditedAttributes.IndexOf(expectedFilterOnView.Value);
                        attribute = listOfEditedAttributes[indexOfAttribute];
                    }
                    else
                    {
                        attribute = expectedFilterOnView.Value;
                    }
                    sb.Append(attribute.DisplayName);
                    sb.Append(EnumHelper.GetEnumDescription((ConditionType)filter._conditionType));
                    if (expectedFilterOnView.Value._fieldType == FieldType.TickBox)
                    {
                        sb.Append(EnumHelper.GetEnumDescription((YesOrNo)Convert.ToUInt32(filter._valueOne)));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(filter._valueOne))
                        {
                            sb.Append(filter._valueOne);
                        }
                        else { sb.Append("-"); }
                        if (!string.IsNullOrEmpty(filter._valueTwo))
                        {
                            sb.Append(" and ");
                            sb.Append(filter._valueTwo);
                        }
                    }
                    sb.Append(attribute.DisplayName);
                }

                #endregion

                #region Assert filter pane
                _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = sb.ToString();
                _customEntityViewsUIMap.AssertSelectedFilters();

                Keyboard.SendKeys("{Esc}");
                #endregion
            }

            //Reset attributes
            foreach(var edited in listOfEditedAttributes) 
            {
                customEntityAttributeUIMap.ClickAttributesLink();
                _customEntityViewsUIMap.FilterAttributesParams.UIDisplayNameEditText = edited.DisplayName;
                _customEntityViewsUIMap.FilterAttributes();

                Keyboard.SendKeys("{Enter}");

                customEntityAttributeUIMap.ClickEditFieldLink(edited.DisplayName);

                CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(customEntityAttributeUIMap);

                string oldName = string.Empty;
                foreach (var originalAttribute in orignalAttributes)
                {
                    if (originalAttribute._attributeid == edited._attributeid)
                    {
                        oldName = originalAttribute.DisplayName;
                        foreach (var globalAttribute in _customEntities[0].attribute)
                        {
                            if (originalAttribute._attributeid == globalAttribute._attributeid)
                            {
                                globalAttribute.DisplayName = oldName;
                            }
                        }
                        break;
                    }
                }
                textAttribute.DisplayNameTxt.Text = oldName;

                customEntityAttributeUIMap.PressSaveAttribute();

            }
        }

        #endregion

        #region 41697 Successfully add filter on custom entity attribute of type greenlight currency
        /// <summary>
        ///  41697 Successfully add filter on custom entity attribute of type greenlight currency
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddFilterOnCustomEntityAttributeOfTypeGreenlightCurrency_UITest() 
        {

            //locate a custom entity that uses currency
            #region test setup
            CustomEntity currencyEntity = null;
            foreach (CustomEntity entity in _customEntities)
            {
                if (entity.enableCurrencies) { currencyEntity = entity; break; }
            }
            if (currencyEntity == null) { throw new ArgumentException("This test requires a CE with enabled Currencies. Test will halt"); }
         
            ///insert data required for this test
            ImportDataToTestingDatabase(testContextInstance.TestName);
        

            CustomEntityAttributesUIMap customEntityAttributeUIMap = new CustomEntityAttributesUIMap();
            CustomEntitiesUtilities.CustomEntityAttribute attributeToEdit = _customEntities[0].attribute[0];

            Dictionary<string, bool> expectedFiltersForCurrency = new Dictionary<string, bool>()
            {
                {"[None]", false},{"Equals", true},{"Does Not Equal",true},{ "Contains Data",false},{"Does Not Contain Data",false}
            };

            #endregion

            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + currencyEntity.entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(currencyEntity.view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region click currency attribute and move to selected filters
            SelectAvailableFieldForFilter(null, "  GreenLight Currency");

            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, "Filters");
            UITestControlCollection actualFilterCertia = viewControls.filterCriteriaOption.Items;
            Assert.AreEqual(expectedFiltersForCurrency.Count, actualFilterCertia.Count);
            
            foreach(UITestControl filterCritera in actualFilterCertia) 
            {
                Assert.IsTrue(expectedFiltersForCurrency.ContainsKey(((HtmlListItem)filterCritera).DisplayText));
            }

            #endregion

            #region set all scenarios that should prompt validation message and verify message displayed
            ///leave criteria blank and validate message on save
            viewControls.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.None);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyFieldForFilterCriteria, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as equals and validate message on save
            viewControls.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyListSelecton, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            ///set criteria as does not equal and validate message on save
            viewControls.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.DoesNotEqual);
            _customEntityViewsUIMap.PressSaveOnViewFilterModal();
            _customEntityViewsUIMap.ValidateMessageModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n" + ViewModalMessages.EmptyListSelecton, new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            _customEntityViewsUIMap.ValidateMessageModal();
            _customEntityViewsUIMap.PressCloseOnMessageModal();

            Keyboard.SendKeys("{Esc}");
            #endregion

            #region Add all filter criteria and verify results
            expectedFiltersForCurrency.Remove("[None]");

            StringBuilder expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
            List<string> selectedCurrencies = new List<string>()
            {
                "US Dollar", "Pound Sterling"
            };
            foreach (KeyValuePair<string, bool> fCriteria in expectedFiltersForCurrency)
            {
                expectedSelectedFilterValue.Append("GreenLight Currenc...");
                SelectAvailableFieldForFilter(null, "  GreenLight Currency");

                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

                viewControls.filterCriteriaOption.SelectedItem = fCriteria.Key;
                expectedSelectedFilterValue.Append(fCriteria.Key);
                //If flag is true - means that it needs valid list items
                if (fCriteria.Value)
                {
                    foreach (string eType in selectedCurrencies)
                    {
                        _customEntityViewsUIMap.MoveSelectedListItem(eType);
                    }
                    expectedSelectedFilterValue.Append("US Dollar and 1 other");
                }
                else
                {
                    expectedSelectedFilterValue.Append("-");
                }

                expectedSelectedFilterValue.Append("GreenLight Currency");
                _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            }

            #endregion

            #region save, edit and verify results
            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
            
        }
        #endregion


        #region 42061 Unsuccessfully delete list item from greenlight attribute when list item is used in view filter
         ///<summary>
         ///Test ensures that once a filter of type list is setup, the list items that are used by that filter
         ///cannot be deleted.
         ///</summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsUnsuccesfullyDeleteListItemWhenInUseByFilterInCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region navigate to CE -> Views -> Filters Tab

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region find an attribute of type list
            CustomEntitiesUtilities.cCustomEntityListAttribute listAttribute = null;
            foreach (CustomEntitiesUtilities.CustomEntityAttribute att in _customEntities[0].attribute)
            {
                if (att.GetType() == typeof(CustomEntitiesUtilities.cCustomEntityListAttribute))
                {
                    listAttribute = (CustomEntitiesUtilities.cCustomEntityListAttribute)att;
                    break;
                }
            }
            if (listAttribute == null)
            {
                throw new ArgumentException("Cannot be list attribute, halting test!");
            }
            #endregion

            #region create a list item filter
            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));

            Dictionary<string, bool> filterCriteria = new Dictionary<string, bool>()
            {
                {EnumHelper.GetEnumDescription(ConditionType.Equals), true}, 
            };

            StringBuilder expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
            foreach (KeyValuePair<string, bool> fCriteria in filterCriteria)
            {
                expectedSelectedFilterValue.Append(listAttribute.DisplayName);
                SelectAvailableFieldForFilter(null, "  " + listAttribute.DisplayName);
                _customEntityViewsUIMap.ClickMoveFilterSelectionRight();

                viewcontrols.filterCriteriaOption.SelectedItem = fCriteria.Key;
                expectedSelectedFilterValue.Append(fCriteria.Key);
                //If flag is true - means that it needs valid list items
                if (fCriteria.Value)
                {
                    foreach (CustomEntitiesUtilities.EntityListItem lItem in listAttribute._listItems)
                    {
                        _customEntityViewsUIMap.MoveSelectedListItem(lItem._textItem);
                    }
                    expectedSelectedFilterValue.Append("standard list item... and 5 others");
                }
                else
                {
                    expectedSelectedFilterValue.Append("-");
                }
                expectedSelectedFilterValue.Append("Standard List");
                _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            }
            #endregion

            #region save, edit and verify results
            _customEntityViewsUIMap.ClickSaveOnViewModal();

            //Click edit against the CE 
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();

            #endregion

            #region Navigate to attributes, attempt to delete the list items
            Keyboard.SendKeys("{Esc}");

            CustomEntityAttributesUIMap attributesUIMap = new CustomEntityAttributesUIMap();

            attributesUIMap.ClickAttributesLink();

            _customEntityViewsUIMap.FilterAttributesParams.UIDisplayNameEditText = listAttribute.DisplayName;

            _customEntityViewsUIMap.FilterAttributes();

            Keyboard.SendKeys("{Enter}");

            attributesUIMap.ClickEditFieldLink(listAttribute.DisplayName);

            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlList listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(_sharedMethods.browserWindow));

            foreach (HtmlListItem item in listItemsControl.Items)
            {
                listItemsControl.SelectedItems = new string[] { item.InnerText };
                attributesUIMap.ClickDeleteInListAttribute();
                attributesUIMap.AssertListItemInUseOnGreenlightFilterViewModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nThis list item cannot be removed as it is in use on a GreenLight View Filter.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
                attributesUIMap.AssertListItemInUseOnGreenlightFilterViewModal();
                Keyboard.SendKeys("{Esc}");
            }

            Keyboard.SendKeys("{Esc}");
            #endregion

            #region verify that filter hasn't changed
            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();


            #endregion
        }
        #endregion

        #region Succefully verify meID is accepted when in use by filter for number in custom entity
        /// <summary>
        /// Ensures that @Me_ID can be applied to a filter of by number
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccefullyVerifyMeIDIsAcceptedWhenInUseByFilterForNumberInCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);
            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[5];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region Enter me Id into criteria
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);

            viewcontrols.textFilterValue1Txt.Text = "@Me_Id";

            Keyboard.SendKeys("{Enter}");
            #endregion

          
            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = "Selected Filters \r\n \r\nNumberEquals@ME_IDNumber";
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion

        #region Succefully verify me is accepted when in use by filter for string in custom entity
        /// <summary>
        /// Ensures that @Me_ID can be applied to a filter of by number
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccefullyVerifyMeIsAcceptedWhenInUseByFilterForStringInCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);
            #region navigate to entity -> view -> filters tab
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region drag required attributes and build required controls
            CustomEntityViewsControls viewcontrols = null;
            string conditiontype = string.Empty;
            var filtertosave = _customEntities[0].view[0].filters[0];
            DragBasicAttributeToFiltersArea(filtertosave._fieldid.ToString());
            viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));
            #endregion

            #region Enter me Id into criteria
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);

            viewcontrols.textFilterValue1Txt.Text = "@Me";

            Keyboard.SendKeys("{Enter}");
            #endregion


            #region save and return to verify each filter
            _customEntityViewsUIMap.PressSaveOnViewModal();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region validate info displaye on grid and then edit to validate set properties
            
            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = "Selected Filters \r\n \r\nStandard Single Te...Equals@MEStandard Single Text";
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion

        }
        #endregion

        #region successfully verify list filter modal layout in view of custom entity
        /// <summary>
        /// Successfully verifies that the list filter modal works correctly.
        /// Ensures Add All and Remove all buttons work
        /// Ensures Dragging with the mouse works
        /// Ensures that the '+' and the delete work correctly
        /// Ensures that the search works correctly
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccesfullyVerifyFilterListModalLayoutInViewOfCustomEntity_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            #region Navigate to CE -> Views -> Filters Tab

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickFiltersTab();
            #endregion

            #region Find list attribute
            CustomEntitiesUtilities.cCustomEntityListAttribute listAttribute = null;
            foreach (CustomEntitiesUtilities.CustomEntityAttribute att in _customEntities[0].attribute)
            {
                if (att.GetType() == typeof(CustomEntitiesUtilities.cCustomEntityListAttribute))
                {
                    listAttribute = (CustomEntitiesUtilities.cCustomEntityListAttribute)att;
                    break;
                }
            }
            #endregion

            #region Move list attribute into Selected filters to prompt filter window
            CustomEntityViewsControls viewcontrols = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Filters));

            SelectAvailableFieldForFilter(null, "  " + listAttribute.DisplayName);

            _customEntityViewsUIMap.ClickMoveFilterSelectionRight();
            #endregion

            #region Set Equals as filter criteria and populate list items
            viewcontrols.filterCriteriaOption.SelectedItem = EnumHelper.GetEnumDescription(ConditionType.Equals);

            foreach (CustomEntitiesUtilities.EntityListItem lItem in listAttribute._listItems)
            {
                _customEntityViewsUIMap.MoveSelectedListItem(lItem._textItem);
            }

            _customEntityViewsUIMap.RemoveSelectedListItem(listAttribute._listItems[0]._textItem);

            _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            #endregion

            #region Verify selected filters pane
            StringBuilder expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);
            expectedSelectedFilterValue.Append(EnumHelper.GetEnumDescription(ConditionType.Equals));
            expectedSelectedFilterValue.Append(string.Format("{0} and {1} others", new object[] { "standard list item...", listAttribute._listItems.Count - 2 }));
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion

            #region Click edit and update filter criteria list items using the + and x icons
            ControlLocator<HtmlControl> controlLocator = new ControlLocator<HtmlControl>();
            HtmlControl editImage = controlLocator.findControl("edit_img_copy_copy_n" + listAttribute.FieldId, new HtmlControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument14.UIControlSelectedFilters));

            _customEntityViewsUIMap = new CustomEntityViewsUIMap();
            Mouse.Click(editImage);

            _customEntityViewsUIMap.RemoveSelectedListItem(listAttribute._listItems[1]._textItem);
            _customEntityViewsUIMap.RemoveSelectedListItem(listAttribute._listItems[2]._textItem);
            _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            #endregion

            #region Verify selected filters pane
            expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);
            expectedSelectedFilterValue.Append(EnumHelper.GetEnumDescription(ConditionType.Equals));
            expectedSelectedFilterValue.Append(string.Format("{0} and {1} others", new object[] { "standard list item...", listAttribute._listItems.Count - 4 }));
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);

            #endregion

            #region Click edit and update filter criteria list items using Search
            editImage = controlLocator.findControl("edit_img_copy_copy_copy_n" + listAttribute.FieldId, new HtmlControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument14.UIControlSelectedFilters));
            _customEntityViewsUIMap = new CustomEntityViewsUIMap();
            Mouse.Click(editImage);

            _customEntityViewsUIMap.EnterSearchCriteriaOnListFilterModalParams.UIItemEditText = listAttribute._listItems[0]._textItem;
            _customEntityViewsUIMap.EnterSearchCriteriaOnListFilterModal();

            _customEntityViewsUIMap.MoveSelectedListItem(listAttribute._listItems[0]._textItem);

            _customEntityViewsUIMap.EnterSearchCriteriaOnListFilterModalParams.UIItemEditText = listAttribute._listItems[1]._textItem;
            _customEntityViewsUIMap.EnterSearchCriteriaOnListFilterModal();

            _customEntityViewsUIMap.MoveSelectedListItem(listAttribute._listItems[1]._textItem);

            _customEntityViewsUIMap.EnterSearchCriteriaOnListFilterModalParams.UIItemEditText = listAttribute._listItems[2]._textItem;
            _customEntityViewsUIMap.EnterSearchCriteriaOnListFilterModal();

            _customEntityViewsUIMap.MoveSelectedListItem(listAttribute._listItems[2]._textItem);

            _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            #endregion

            #region Verify selected filters pane
            expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);
            expectedSelectedFilterValue.Append(EnumHelper.GetEnumDescription(ConditionType.Equals));
            expectedSelectedFilterValue.Append(string.Format("{0} and {1} others", new object[] { "standard list item...", listAttribute._listItems.Count - 1 }));
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);
            #endregion

            #region Manipulate list filter via Remove All / Add All buttons
            editImage = controlLocator.findControl("edit_img_copy_copy_copy_copy_n" + listAttribute.FieldId, new HtmlControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument14.UIControlSelectedFilters));
            _customEntityViewsUIMap = new CustomEntityViewsUIMap();
            Mouse.Click(editImage);

            _customEntityViewsUIMap.ClickRemoveAllListItemsFromSelectedListFilter();


            _customEntityViewsUIMap.ClickAddAllListItemToSelectedPane();

            _customEntityViewsUIMap.ClickSaveOnListFilterModal();

            #endregion

            #region Verify selected filters pane
            expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);
            expectedSelectedFilterValue.Append(EnumHelper.GetEnumDescription(ConditionType.Equals));
            expectedSelectedFilterValue.Append(string.Format("{0} and {1} others", new object[] { "standard list item...", listAttribute._listItems.Count }));
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);
            #endregion

            #region Manipulate list filters by mouse drag and drop (Left -> right only)
            editImage = controlLocator.findControl("edit_img_copy_copy_copy_copy_copy_n" + listAttribute.FieldId, new HtmlControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument14.UIControlSelectedFilters));
            //HtmlControl editImage = controlLocator.findControl("edit_img_copy_copy_n" + listAttribute.FieldId, new HtmlControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow16.UIGreenLightCustomEntiDocument14.UIControlSelectedFilters));

            _customEntityViewsUIMap = new CustomEntityViewsUIMap();
            Mouse.Click(editImage);
            _customEntityViewsUIMap.ClickRemoveAllListItemsFromSelectedListFilter();

            _customEntityViewsUIMap.MoveSelectedListItem(listAttribute._listItems[0]._textItem);

            _customEntityViewsUIMap.ClickSaveOnListFilterModal();
            #endregion

            #region Verify selected filters pane
            expectedSelectedFilterValue = new StringBuilder("Selected Filters \r\n \r\n");
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);
            expectedSelectedFilterValue.Append(EnumHelper.GetEnumDescription(ConditionType.Equals));
            expectedSelectedFilterValue.Append(string.Format("{0}", new object[] { "standard list item... " }));
            expectedSelectedFilterValue.Append(listAttribute.DisplayName);

            _customEntityViewsUIMap.AssertSelectedFiltersExpectedValues.UISelectedFiltersAccouPaneInnerText = expectedSelectedFilterValue.ToString();
            _customEntityViewsUIMap.AssertSelectedFilters();
            #endregion
        }
        #endregion
        #endregion

        #region Icon
        #region 45413 - Successfully add icon to view
        /// <summary>
        /// 45413 - Successfully add icon to view
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyAddIconToView_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);
            _customEntityViewsUIMap.ClickViewsLink();
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            //Click the Icon Tab
            _customEntityViewsUIMap.ClickIconTab();

            //Verify default selected icon is window_dialog.png
            _customEntityViewsUIMap.ValidateSelectedIcon();
            _customEntityViewsUIMap.ValidateSelectedIconText();
            //Verify 30 icons are displayed when entering the Icons tab for the first time
            Assert.AreEqual(30, _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIViewIconResultsPane.GetChildren().Count);

            //Select an icon from the existing ones
            UITestControl selectedImage = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIViewIconResultsPane.GetChildren()[0].GetChildren()[0];
            Mouse.Click(selectedImage);

            Playback.Wait(500);

            //Validate selected icon and selected icon text
            _customEntityViewsUIMap.ValidateSelectedIconExpectedValues.UIStaticicons48plainwiImageFriendlyName = selectedImage.FriendlyName.ToString();
            _customEntityViewsUIMap.ValidateSelectedIcon();
            _customEntityViewsUIMap.ValidateSelectedIconTextExpectedValues.UIWindow_dialogpngPaneDisplayText = selectedImage.FriendlyName.Substring(selectedImage.FriendlyName.LastIndexOf("/") + 1);
            _customEntityViewsUIMap.ValidateSelectedIconText();

            //Press Save
            _customEntityViewsUIMap.PressSaveOnViewModal();

            //Edit the view and validate icon is saved
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);
            _customEntityViewsUIMap.ClickIconTab();
            _customEntityViewsUIMap.ValidateSelectedIconExpectedValues.UIStaticicons48plainwiImageFriendlyName = selectedImage.FriendlyName.ToString();
            _customEntityViewsUIMap.ValidateSelectedIcon();
            _customEntityViewsUIMap.ValidateSelectedIconTextExpectedValues.UIWindow_dialogpngPaneDisplayText = selectedImage.FriendlyName.Substring(selectedImage.FriendlyName.LastIndexOf("/") + 1);
            _customEntityViewsUIMap.ValidateSelectedIconText();
         }
        #endregion

        #region 45416 - Successfully cancel adding icon to view
        /// <summary>
        /// 45416 - Successfully cancel adding icon to view
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyCancelAddingIconToView_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);
            _customEntityViewsUIMap.ClickViewsLink();
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);
            _customEntityViewsUIMap.ClickIconTab();

            //Select an icon from the existing ones
            UITestControl selectedImage = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIViewIconResultsPane.GetChildren()[0].GetChildren()[0];
            Mouse.Click(selectedImage);

            //Press Cancel
            _customEntityViewsUIMap.PressCancelOnViewModal();

            //Edit the view and validate icon is saved
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);
            _customEntityViewsUIMap.ClickIconTab();
            _customEntityViewsUIMap.ValidateSelectedIcon();
            _customEntityViewsUIMap.ValidateSelectedIconText();
        }
        #endregion

        #region 45417 - Successfully search for view icon, 45421 - Successfully verify maximum length property is applied on search field of the Icon tab, 45423 - Successfully search for view icon where special characters are used, 45553 - Successfully clear search details on view icon tab
        /// <summary>
        /// 45417 - Successfully search for view icon
        /// 45421 - Successfully verify maximum length property is applied on search field of the Icon tab
        /// 45423 - Successfully search for view icon where special characters are used
        /// 45553 - Successfully clear search details on view icon tab
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyVerifySearchFieldInViewsIconTab_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);
            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);
            _customEntityViewsUIMap.ClickViewsLink();
            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);
            _customEntityViewsUIMap.ClickIconTab();

            //Define the controls of the icon tab
            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Icon));        

            //Verify maximum length
            Assert.AreEqual(21, viewControls.searchTxt.MaxLength);

            //Copy and paste text - ensure truncation occurs
            Clipboard.Clear();
            Clipboard.SetText(Strings.BasicString);
            _customEntityFormsUIMap.RightClickAndPaste(viewControls.searchTxt);
            viewControls.searchTxt.WaitForControlCondition(tc =>
            {
                return viewControls.searchTxt.Text.Length == 21;
            }, 1000);
            Assert.AreEqual(21, viewControls.searchTxt.Text.Length);
            Clipboard.Clear();

            //Search for an icon and verify the results
            UITestControl selectedImage = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIViewIconResultsPane.GetChildren()[0].GetChildren()[0];
            string selectedImageText = selectedImage.FriendlyName.Substring(selectedImage.FriendlyName.LastIndexOf("/") + 1);
            viewControls.searchTxt.Text = selectedImageText;
            _customEntityViewsUIMap.ClickMagnifyingGlassOnSearchBox();
            Assert.AreEqual(1, _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIViewIconResultsPane.GetChildren().Count);
            UITestControl expectedImage = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIViewIconResultsPane.GetChildren()[0].GetChildren()[0];
            Assert.AreEqual(selectedImageText, expectedImage.FriendlyName.Substring(expectedImage.FriendlyName.LastIndexOf("/") + 1));

            //Clear the search box
            _customEntityViewsUIMap.ClickClearSearchOptions();
            Assert.AreEqual("Search...", viewControls.searchTxt.Text);

            //Enter special characters and verify the results
            viewControls.searchTxt.Text = "\\\"" + selectedImageText;
            Keyboard.SendKeys("{Enter}");
            expectedImage = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIViewIconResultsPane.GetChildren()[0].GetChildren()[0];
            Assert.AreEqual(selectedImageText, expectedImage.FriendlyName.Substring(expectedImage.FriendlyName.LastIndexOf("/") + 1));
        }
        #endregion

        #region 45420 - Successfully browse through view icons
        /// <summary>
        /// 45420 - Successfully browse through view icons
        /// </summary>
        [TestCategory("Greenlight Views"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityViewsSuccessfullyBrowseThroughViewIcons_UITest()
        {
            ImportDataToTestingDatabase(testContextInstance.TestName);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + _customEntities[0].entityId);

            _customEntityViewsUIMap.ClickViewsLink();

            _customEntityViewsUIMap.ClickEditFieldLink(_customEntities[0].view[0]._viewName);

            _customEntityViewsUIMap.ClickIconTab();

            //Verify that the right arrow is enabled and the left is disabled
            Assert.AreEqual("active", _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIItemPane.Class);
            Assert.AreEqual(null, _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIItemPane1.Class);

            //Go to the next page, verify both arrows are enabled
            _customEntityViewsUIMap.ClickRightArrowOnIconTab();
            Assert.AreEqual("active", _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIItemPane.Class);
            Assert.AreEqual("active", _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIItemPane1.Class);

            //Search for a specific icon from the first page and verify both controls are disabled
            _customEntityViewsUIMap.ClickLeftArrowOnIconTab();
            UITestControl selectedImage = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIViewIconResultsPane.GetChildren()[0].GetChildren()[0];
            string selectedImageText = selectedImage.FriendlyName.Substring(selectedImage.FriendlyName.LastIndexOf("/") + 1);
            CustomEntityViewsControls viewControls = new CustomEntityViewsControls(_customEntityViewsUIMap, EnumHelper.GetEnumDescription(TabName.Icon));
            viewControls.searchTxt.Text = selectedImageText;
            Keyboard.SendKeys("{Enter}");
            Assert.AreEqual(null, _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIItemPane.Class);
            Assert.AreEqual(null, _customEntityViewsUIMap.UIGreenLightCustomEntiWindow17.UIGreenLightCustomEntiDocument.UIItemPane1.Class);
        }
        #endregion
        #endregion

        #region drag and drop attributes / fields
        public HtmlCustom GridRow(string copy, string controlId)
        {
            #region Variable Declarations
            UIControlLocator<HtmlCustom> controlLocator = new UIControlLocator<HtmlCustom>();
            string jtreecontrolid = copy + "copy_n" + controlId;
            HtmlCustom rowcontrol = controlLocator.findControl(jtreecontrolid, new HtmlCustom(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow13.UIGreenLightCustomEntiDocument));
            #endregion
            return rowcontrol;
        }

        private string CreateCopy(int numberofcopies)
        {
            string mastercopy = "copy_";
            string copies = string.Empty;
            for (int timestocopy = 0; timestocopy < numberofcopies; timestocopy++)
            {
                copies += mastercopy;
            }
            return copies;
        }

        public void ClickEditFilter(string copy, string controlId)
        {
            string image = "edit_img_";
            string jtreecontrolid = image + copy +"copy_n" + controlId;
            #region Variable Declarations
            ControlLocator<HtmlImage> controlLocator = new ControlLocator<HtmlImage>();
            HtmlImage EditIcon = controlLocator.findControl(jtreecontrolid, new HtmlImage(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow13.UIGreenLightCustomEntiDocument));
            EditIcon.EnsureClickable();
            Mouse.Click(EditIcon);
            #endregion
        }

        public void SelectAvailableFieldForFilter(string controlId, string innerText = null)
        {
            string jtreecontrolid = "n" + controlId;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToSelect = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow11.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane), innerText);
            #endregion
            UITestControl hyperLinkOfFieldToMove = fieldToSelect.GetChildren()[1];
            fieldToSelect.EnsureClickable();
            Mouse.Click(hyperLinkOfFieldToMove);
        }

        public void DragNTo1RelationshipAttribute(string relationshipExpandedId, string fieldId)
        {
            string jtreecontrolid = string.Format("k{0}_n{1}", new object[] { relationshipExpandedId, fieldId }); ;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToMove = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
            UITestControl hyperLinkOfFieldToMove = fieldToMove.GetChildren()[1];

            HtmlDiv uITherearenocolumnsselPane = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument.UITherearenocolumnsselPane;
            #endregion

            // Reset flag to ensure that play back stops if there is an error.
            Playback.PlaybackSettings.ContinueOnError = false;

            // Move 'Wide List' link from (127, 10) to 'There are no columns selected.' pane (162, 93)
            //uITherearenocolumnsselPane.EnsureClickable(new Point(162, 93));
            Mouse.StartDragging(hyperLinkOfFieldToMove, new Point(127, 10));
            Mouse.StopDragging(uITherearenocolumnsselPane, new Point(162, 50));
        }

        public void SelectField(string controlId)
        {
            string jtreecontrolid = "copy_n" + controlId;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToSelect = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument.UITherearenocolumnsselPane));
            #endregion
            UITestControl hyperLinkOfFieldToMove = fieldToSelect.GetChildren()[0];
            fieldToSelect.EnsureClickable();
            Mouse.Click(hyperLinkOfFieldToMove);
        }

        public void SelectAvailableFieldForColumn(string controlId)
        {
            string jtreecontrolid = "n" + controlId;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToSelect = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
            #endregion
            UITestControl hyperLinkOfFieldToMove = fieldToSelect.GetChildren()[1];
            fieldToSelect.EnsureClickable();
            Mouse.Click(hyperLinkOfFieldToMove);
        }

        public void SelectNTo1RelationshipForColumn(string relationshipExpandedId, string fieldId)
        {
            string jtreecontrolid = string.Format("k{0}_n{1}", new object[] { relationshipExpandedId, fieldId }); ;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToSelect = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
            #endregion
            UITestControl hyperLinkOfFieldToMove = fieldToSelect.GetChildren()[1];
            fieldToSelect.EnsureClickable();
            Mouse.Click(hyperLinkOfFieldToMove);
        }

        public void SelectFieldForColumn(string jtreecontrolid)
        {
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToSelect = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
            #endregion
            UITestControl hyperLinkOfFieldToMove = fieldToSelect.GetChildren()[1];
            fieldToSelect.EnsureClickable();
            Mouse.Click(hyperLinkOfFieldToMove);
        }

        private void DragBasicAttributeToColumnsArea(string controlid)
        {
            int waitForReadyTimeOut = Playback.PlaybackSettings.WaitForReadyTimeout;
            Playback.PlaybackSettings.WaitForReadyTimeout = 0;
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;

            string jtreecontrolid = "n" + controlid;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToMove = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
            UITestControl hyperLinkOfFieldToMove = fieldToMove.GetChildren()[1];
            HtmlDiv uITherearenocolumnsselPane = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument.UITherearenocolumnsselPane;
            #endregion


            //TherearenocolumnsselPane.EnsureClickable(new Point(162, 93));
            Mouse.MouseDragSpeed = 400;
            Mouse.Click(hyperLinkOfFieldToMove);
            Mouse.StartDragging(hyperLinkOfFieldToMove, new Point(127, 10));
            Mouse.StopDragging(uITherearenocolumnsselPane, new Point(309, 244));
            Mouse.Click(uITherearenocolumnsselPane);

            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            Playback.PlaybackSettings.WaitForReadyTimeout = waitForReadyTimeOut;

        }

        private void DragBasicAttributeOutsideFilterDropArea(string controlid)
        {
            string jtreecontrolid = "n" + controlid;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToMove = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow11.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane));
            UITestControl hyperLinkOfFieldToMove = fieldToMove.GetChildren()[1];

            HtmlDiv UITherearenofiltersselPane = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow9.UIGreenLightCustomEntiDocument.UITherearenofiltersselPane;
            #endregion

            // Reset flag to ensure that play back stops if there is an error.
            Playback.PlaybackSettings.ContinueOnError = false;

            // Move 'Wide List' link from (127, 10) to 'There are no columns selected.' pane (162, 93)
            UITherearenofiltersselPane.EnsureClickable(new Point(162, 93));
            Mouse.MouseDragSpeed = 400;
            Mouse.StartDragging(hyperLinkOfFieldToMove, new Point(77, 4));
            Mouse.StopDragging(130, 50);
        }

        private void DragBasicAttributeToFiltersArea(string controlid)
        {
            string jtreecontrolid = "n" + controlid;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToMove = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow11.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane));
            UITestControl hyperLinkOfFieldToMove = fieldToMove.GetChildren()[1];

            HtmlDiv UITherearenofiltersselPane = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow9.UIGreenLightCustomEntiDocument.UITherearenofiltersselPane;
            #endregion

            // Reset flag to ensure that play back stops if there is an error.
            Playback.PlaybackSettings.ContinueOnError = false;

            // Move 'Wide List' link from (127, 10) to 'There are no columns selected.' pane (162, 93)
            UITherearenofiltersselPane.EnsureClickable(new Point(162, 93));
            Mouse.MouseDragSpeed = 400;
            Mouse.StartDragging(hyperLinkOfFieldToMove, new Point(77, 4));
            Mouse.StopDragging(UITherearenofiltersselPane, new Point(130, 50));
        }

        public void SelectUDFFieldForColumn(string relationshipExpandedID, string controlId, string udfID)
        {
            string jtreecontrolid = "k" + relationshipExpandedID + "_g" + controlId + "_n" + udfID;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToSelect = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
            #endregion
            UITestControl hyperLinkOfFieldToMove = fieldToSelect.GetChildren()[1];
            fieldToSelect.EnsureClickable();
            Mouse.Click(hyperLinkOfFieldToMove);
        }

        public void SelectUDFFieldForFilter(string relationshipExpandedID, string controlId, string udfID)
        {
            string jtreecontrolid = "k" + relationshipExpandedID + "_g" + controlId + "_n" + udfID;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToSelect = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow11.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane));
            #endregion
            UITestControl hyperLinkOfFieldToMove = fieldToSelect.GetChildren()[1];
            fieldToSelect.EnsureClickable();
            Mouse.Click(hyperLinkOfFieldToMove);
        }

        private void DragUDFToFiltersArea(string relationshipExpandedID, string controlId, string udfID)
        {
            string jtreecontrolid = "k" + relationshipExpandedID + "_g" + controlId + "_n" + udfID;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToMove = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow11.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane));
            UITestControl hyperLinkOfFieldToMove = fieldToMove.GetChildren()[1];

            HtmlDiv UITherearenofiltersselPane = _customEntityViewsUIMap.UIGreenLightCustomEntiWindow9.UIGreenLightCustomEntiDocument.UITherearenofiltersselPane;
            #endregion

            // Reset flag to ensure that play back stops if there is an error.
            Playback.PlaybackSettings.ContinueOnError = false;

            // Move 'Wide List' link from (127, 10) to 'There are no columns selected.' pane (162, 93)
            UITherearenofiltersselPane.EnsureClickable(new Point(162, 93));
            Mouse.MouseDragSpeed = 400;
            Mouse.StartDragging(hyperLinkOfFieldToMove, new Point(77, 4));
            Mouse.StopDragging(UITherearenofiltersselPane, new Point(130, 50));
        }

        public UITestControl ExpandNToOneNodeForColumns(string controlId)
        {
            string jtreecontrolid = "k" + controlId;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToExpand = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
            #endregion

            fieldToExpand.EnsureClickable();
            Mouse.Click(fieldToExpand);
            return fieldToExpand;
        }

        public UITestControl ExpandNToOneNodeForFilters(string controlId)
        {
            string jtreecontrolid = "k" + controlId;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToExpand = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow11.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane));
            #endregion

            fieldToExpand.EnsureClickable();
            Mouse.Click(fieldToExpand);
            return fieldToExpand;
        }

        public void ExpandSecondLevelRelationship(string relationshipExpandedID, string controlId)
        {
            string jtreecontrolid = "k" + relationshipExpandedID + "_k" + controlId;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToExpand = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow15.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane));
            #endregion
            fieldToExpand.EnsureClickable();
            Mouse.Click(fieldToExpand.GetChildren()[1]);
        }

        public void ExpandUDFNodeInAvailableFieldsForColumns(string relationshipExpandedID, string controlId)
        {
            string jtreecontrolid = "k" + relationshipExpandedID + "_g" + controlId;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToExpand = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument));
            #endregion
            fieldToExpand.EnsureClickable();
            Mouse.Click(fieldToExpand.GetChildren()[1]);
        }

        public void ExpandUDFNodeInAvailableFieldsForFilters(string relationshipExpandedID, string controlId)
        {
            string jtreecontrolid = "k" + relationshipExpandedID + "_g" + controlId;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();
            UITestControl fieldToExpand = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow11.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane));
            #endregion
            fieldToExpand.EnsureClickable();
            Mouse.Click(fieldToExpand.GetChildren()[1]);
        }

        public void RemoveBasicAttributeFromSelectedColumn(string controlid)
        {
            string jtreecontrolid = "copy_n" + controlid;
            #region Variable Declarations
            UIControlLocator<UITestControl> controlLocator = new UIControlLocator<UITestControl>();

            UITestControl fieldToMove = controlLocator.findControl(jtreecontrolid, new UITestControl(_customEntityViewsUIMap.UIGreenLightCustomEntiWindow4.UIGreenLightCustomEntiDocument.UITherearenocolumnsselPane));
            UITestControl hyperLinkOfFieldToMove = fieldToMove.GetChildren()[0];

            #endregion

            // Reset flag to ensure that play back stops if there is an error.
            Playback.PlaybackSettings.ContinueOnError = false;
            Mouse.Click(hyperLinkOfFieldToMove, new Point(127, 10));
            Mouse.StartDragging(hyperLinkOfFieldToMove, new Point(127, 10));
            Mouse.StopDragging(-300, 0);
            Mouse.Click();
        }
        #endregion

        internal class CachePopulatorForViews : CachePopulator
        {

            internal CachePopulatorForViews(ProductType executingProduct) : base(executingProduct) { }
            public override string GetSQLStringForCustomEntity()
            {
                return "SELECT TOP 2 entityid, entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, enableAudiences, enablePopupWindow, defaultPopupView, tableid, createdby FROM customEntities";
            }

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
                        view._description = reader.GetString(descriptionOrdinal);
                        view._modifiedBy = reader.IsDBNull(modifiedByOrdinal) ? null : reader.GetString(modifiedByOrdinal);
                        view._createdBy = AutoTools.GetEmployeeIDByUsername(_executingProduct).ToString();
                        view._menudescription = reader.GetString(menuDescriptionOrdinal);
                        if(!reader.IsDBNull(menuidOrdinal)) 
                        {
                            view._menuid = reader.GetInt32(menuidOrdinal);
                        }
                        
                        view._allowAdd = reader.GetBoolean(allowAddOrdinal);
                        view._allowEdit = reader.GetBoolean(allowEditOrdinal);
                        view._allowDelete = reader.GetBoolean(allowDeleteOrdinal);
                        view._allowApproval = reader.GetBoolean(allowApprovalOrdinal);
                        view._createdOn = DateTime.Now; 

                        if(!reader.IsDBNull(addFormOrdinal))
                        {
                            view.addform = PopulateViewsFormDropdown(entity.form, reader.GetInt32(addFormOrdinal));
                        }
                        if (!reader.IsDBNull(editFormOrdinal))
                        {
                            view.editform = PopulateViewsFormDropdown(entity.form, reader.GetInt32(editFormOrdinal));
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
                        viewField._joinViaId = reader.IsDBNull(joinViaIDOrdinal) ? 0 : reader.GetInt32(joinViaIDOrdinal);
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
                    #endregion

                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityViewFilter viewFilter = new CustomEntitiesUtilities.CustomEntityViewFilter();
                        #region Set values
                        viewFilter._viewId = view._viewid;
                        viewFilter._fieldid = reader.GetGuid(fieldIdOrdinal);
                        viewFilter._conditionType = reader.GetByte(conditionOrdinal);
                        viewFilter._valueOne = reader.GetString(valueOrdinal);
                        viewFilter._order = reader.GetByte(orderOrdinal);
                        viewFilter._joinViaId = reader.IsDBNull(joinViaIDOrdinal) ? 0 : reader.GetInt32(joinViaIDOrdinal);
                        viewFilter._valueTwo = reader.GetString(value2Ordinal);
                        view.filters.Add(viewFilter);
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
                    int isAuditIdentityOrdinal = reader.GetOrdinal("is_audit_identity");
                    int advicePanelTextOrdinal = reader.GetOrdinal("advicePanelText");
                    int isUniqueOrdinal = reader.GetOrdinal("is_unique");
                    int attributeIDOrdinal = reader.GetOrdinal("attributeid");
                    int relatedTableOrdinal = reader.GetOrdinal("relatedtable");
                    int relationshipDisplayFieldOrdinal = reader.GetOrdinal("relationshipdisplayfield");
                    int maxRows = reader.GetOrdinal("maxRows");
                    int systeAttributeOrdinal = reader.GetOrdinal("System_attribute");
                    #endregion

                    while (reader.Read())
                    {
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
                            //PopulateUserDefinedFields(ref entity);
                            //ConstructNToOneRelationshipFields(newNToOneAttribute);
                            attribute = newNToOneAttribute;
                        }
                        else if (type == FieldType.Relationship && relType == RelationshipType.OneToMany) { break; }

                        #region Set values
                        attribute._attributeid = reader.GetInt32(attributeIDOrdinal);
                        attribute._createdBy = AutoTools.GetEmployeeIDByUsername(ExecutingProduct);
                        attribute._modifiedBy = reader.IsDBNull(modifiedByOrdinal) ? null : reader.GetString(modifiedByOrdinal);
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
                    int checkDefaultValuesOrdinal = reader.GetOrdinal("CheckDefaultValues");
                    var saveAndDuplicateButtonTextOrdinal = reader.GetOrdinal("SaveAndDuplicateButtonText");
                    var showSaveAndDuplicateOrdinal = reader.GetOrdinal("showSaveAndDuplicate");
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
                        form._modifiedBy = reader.IsDBNull(modifiedByOrdinal) ? null : reader.GetString(modifiedByOrdinal);
                        form._showBreadcrumbs = reader.GetBoolean(showBreadCrumbsOrdinal);
                        form._showSaveAndNew = reader.GetBoolean(showSaveAndNewOrdinal);
                        form._showSubMenus = reader.GetBoolean(showSubMenusOrdinal);
                        form._saveAndNewButtonText = reader.IsDBNull(saveAndNewButtonTextOrdinal) ? null : reader.GetString(saveAndNewButtonTextOrdinal);
                        form._saveButtonText = reader.IsDBNull(saveButtonTextOrdinal) ? null : reader.GetString(saveButtonTextOrdinal);
                        form._cancelButtonText = reader.IsDBNull(cancelButtonTextOrdinal) ? null : reader.GetString(cancelButtonTextOrdinal);
                        form._showSave = reader.GetBoolean(showSaveOrdinal);
                        form._showCancel = reader.GetBoolean(showCancelOrdinal);
                        form._saveAndStayButtonText = reader.IsDBNull(saveAndStayButtonTextOrdinal) ? null : reader.GetString(saveAndStayButtonTextOrdinal);
                        form._saveAndDuplicateButtonText = reader.IsDBNull(saveAndDuplicateButtonTextOrdinal) ? null : reader.GetString(saveAndDuplicateButtonTextOrdinal);
                        form._showSaveAndDuplicate = reader.GetBoolean(showSaveAndDuplicateOrdinal);
                        form._showSaveAndStay = reader.GetBoolean(showSaveAndStayOrdinal);
                        form._checkDefaultValues = reader.GetBoolean(checkDefaultValuesOrdinal);
                        entity.form.Add(form);
                        form._formid = 0;
                        #endregion
                    }
                    reader.Close();
                }
                db.sqlexecute.Parameters.Clear();
            }
            #endregion

        }

        private void ImportDataToTestingDatabase(string test)
        {
            int filterindexer;
            int resultview;
            foreach (CustomEntity ce in _customEntities)
            {
                foreach (CustomEntitiesUtilities.CustomEntityView view in ce.view)
                {
                    view._viewid = 0;
                }
            }
            switch (test)
            {
                case "CustomEntityViewsSuccessfullyEditViewToCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyEditViewToCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyDeleteViewToCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyDeleteViewToCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyAddDuplicateViewToCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyAddColumnsForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyVerifyMoveRightButtonWorkCorrectlyInColumnsTabOfViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyVerifySortingColumnDisabledWhenAddingNewViewToCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddBaseTableFieldsAsColumnsForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddTwoLevelsDeepFieldsAsColumnsForViewInCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyDragFilterOutsideOfDropAreaInViewOfCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddAllBasicAttributesToSelectedFields_UITest":
                case "CustomEntityViewsSuccessfullyAddNToOneAttributesToSelectedFields_UITest":
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    _customEntities[0].view[1].addform = _customEntities[0].form[1];
                    _customEntities[0].view[1].editform = _customEntities[0].form[1];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    Assert.IsTrue(resultview > 0);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsUnsuccessfullyEditDuplicateViewToCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullySortViewsTableinCustomEntity_UITest":
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    Assert.IsTrue(resultview > 0);
                    _customEntities[0].view[1].addform = _customEntities[0].form[1];
                    _customEntities[0].view[1].editform = _customEntities[0].form[1];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[1], _executingProduct);
                    Assert.IsTrue(resultview > 0);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsSuccessfullyEditSortColumnAndSortDirectionToViewInCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyEditSortColumnAndSortDirectionToViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyRemoveSortColumnAndSortDirectionToViewInCustomEntity_UITest":
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    for (int columnindexer = 0; columnindexer < _customEntities[0].view[0].fields.Count; columnindexer++)
                    {
                        _customEntities[0].view[0].fields[columnindexer]._fieldid = _customEntities[0].attribute[columnindexer].FieldId;
                        CustomEntitiesUtilities.CreateViewFields(_customEntities[0].view[0], _customEntities[0].view[0].fields[columnindexer], _executingProduct);
                    }
                    _customEntities[0].view[0].sortColumn._fieldID = _customEntities[0].attribute[0].FieldId;
                    CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsSuccessfullyDeleteAttributeWhenInUseAsColumnForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyUpdateAttributeInChosenColumnsAfterSortingOrderIsSet_UITest":
                case "CustomEntityViewsSuccessfullyVerifySortTabIsDisabledUponDeletingAllColumnsForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyVerifySortColumnAndDirectionResetUponRemovingColumnUsedForSortInViewOfCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyVerifyMoveUpAndMoveDownButtonsWorkCorrectlyInColumnsTabOfViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyEditColumnsForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyVerifyRemoveSelectionAndRemoveAllButtonsWorkCorrectlyInColumnsTabOfViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyDeleteColumnsFromViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyEditAttributeWhenInUseAsColumnForGreenlightView_UITest":
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    for (int columnindexer = 0; columnindexer < _customEntities[0].view[0].fields.Count; columnindexer++)
                    {
                        _customEntities[0].view[0].fields[columnindexer]._fieldid = _customEntities[0].attribute[columnindexer].FieldId;
                        if (_customEntities[0].attribute[columnindexer]._fieldType != FieldType.Relationship && _customEntities[0].attribute[columnindexer]._fieldType != FieldType.Comment && columnindexer < 17)
                        {
                            CustomEntitiesUtilities.CreateViewFields(_customEntities[0].view[0], _customEntities[0].view[0].fields[columnindexer], _executingProduct);
                        }
                    }
                    _customEntities[0].view[0].sortColumn._fieldID = _customEntities[0].attribute[0].FieldId;
                    CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeStandardTextForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeLargeTextForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeFormattedLargeTextForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeNumberForViewInCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeNumberWhereInvalidValuesAreUsedForFilterOfViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeDecimalForViewInCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeDecimalWhereInvalidValuesAreUsedForFilterOfViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeCurrencyForViewInCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeCurrencyWhereInvalidValuesAreUsedForFilterOfViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeCheckboxForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeDateForViewInCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeDateWhereInvalidValuesAreUsedForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeDateTimeForViewInCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeDateTimeWhereInvalidValuesAreUsedForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForAttributeOfTypeTimeForViewInCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyAddFiltersForAttributeOfTypeTimeWhereInvalidValuesAreUsedForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyVerifyMoveRightButtonWorkCorrectlyInFiltersTabOfViewInCustomEntity_UITest":
                case "CustomEntityViewsUnsuccessfullyAddFiltersForAttributeWhereCancelIsClickedForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddTwoFiltersOfTheSameTypeOnTheSameAttribForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyVerifyModalLayoutForFiltersTabInViewOfCustomEntity_UITest":
                case "CustomEntityViewsSuccefullyVerifyMeIDIsAcceptedWhenInUseByFilterForNumberInCustomEntity_UITest":   
                case "CustomEntityViewsSuccefullyVerifyMeIsAcceptedWhenInUseByFilterForStringInCustomEntity_UITest":    
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    filterindexer = 0;
                    while (filterindexer < 5)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[0].FieldId;
                        filterindexer++;
                    }
                    while (filterindexer < 12)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[2].FieldId;
                        filterindexer++;
                    }
                    while (filterindexer < 14)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[12].FieldId;
                        filterindexer++;
                    }
                    while (filterindexer < 52)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[6].FieldId;
                        filterindexer++;
                    }
                    while (filterindexer < 60)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[7].FieldId;
                        filterindexer++;
                    }
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeStandardTextForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeLargeTextForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeNumberForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeDecimalForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeCurrencyForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeCheckboxForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeDateForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeTimeForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyAddFiltersForUDFOfTypeListForViewInCustomEntity_UITest":
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    foreach (var udf in _userDefinedFields)
                    {
                        UserDefinedFieldsRepository.CreateUserDefinedField(udf, _executingProduct);
                    }
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    filterindexer = 0;
                    while (filterindexer < 5)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _userDefinedFields[0].FieldId;
                        filterindexer++;
                    }
                    while (filterindexer < 12)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _userDefinedFields[2].FieldId;
                        filterindexer++;
                    }
                    while (filterindexer < 14)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _userDefinedFields[5].FieldId;
                        filterindexer++;
                    }
                    while (filterindexer < 52)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _userDefinedFields[7].FieldId;
                        filterindexer++;
                    }
                    while (filterindexer < 60)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _userDefinedFields[9].FieldId;
                        filterindexer++;
                    }
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsSuccessfullyDeleteAttributeWhenInUseByFilterForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyEditFiltersForAttributeOfAllTypeForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyEditDecimalAttributeWhenInUseOnFiltersForViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyEditAttributesWhenInUseOnFiltersForViewInCustomEntity_UITest":
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    for (filterindexer = 0; filterindexer < _customEntities[0].view[0].filters.Count; filterindexer++)
                    {
                        if (filterindexer < 5)
                        {
                            _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[0].FieldId;
                        }
                        else if (filterindexer < 12)
                        {
                            _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[2].FieldId;
                        }
                        else if (filterindexer < 14)
                        {
                            _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[12].FieldId;
                        }
                        else if (filterindexer < 52)
                        {
                            _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[5].FieldId;
                        }
                        else
                        {
                            _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[7].FieldId;
                        }
                    }

                    List<int> filterIndexes = new List<int>() {0,5,11,13,14,48,51,54,55};
                    foreach(int index in filterIndexes)
                    {
                        var entity = _customEntities[0].Clone();
                        var customEntity = entity as CustomEntity;
                        //Create custom entity view filter
                        if (customEntity != null)
                        {
                            CustomEntitiesUtilities.CreateViewFilters(view: customEntity.view[0], filtertoSave: customEntity.view[0].filters[index], executingProduct: _executingProduct);
                            //Perform reverse lookup on filter to get attribute
                            foreach (var entityAttribute in customEntity.attribute.Where(entityAttribute => customEntity.view[0].filters[index]._fieldid == entityAttribute.FieldId))
                            {
                                //if(!_enabledFilters.ContainsKey(entityAttribute))
                                // _enabledFilters.Add(entityAttribute, _customEntities[0].view[0].filters[index]);
                                this._enabledFilters.Add(customEntity.view[0].filters[index], entityAttribute);
                            }
                        }
                    }
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsSuccessfullyVerifyRemoveSelectionAndRemoveAllButtonsWorkCorrectlyInFiltersTabOfViewInCustomEntity_UITest":
                case "CustomEntityViewsSuccessfullyVerifyMoveUpAndMoveDownButtonsWorkCorrectlyInFiltersTabOfViewInCustomEntity_UITest":
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    filterindexer = 0;
                    while (filterindexer < 5)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[0].FieldId;
                        CustomEntitiesUtilities.CreateViewFilters(view: _customEntities[0].view[0], filtertoSave: _customEntities[0].view[0].filters[filterindexer], executingProduct: _executingProduct);
                        filterindexer++;
                    }
                    while (filterindexer < 12)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[2].FieldId;
                        CustomEntitiesUtilities.CreateViewFilters(view: _customEntities[0].view[0], filtertoSave: _customEntities[0].view[0].filters[filterindexer], executingProduct: _executingProduct);
                        filterindexer++;
                    }
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsUnsuccessfullyEditFiltersForAttributeWhereCancelIsClickedForViewInCustomEntity_UITest":
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    for (filterindexer = 0; filterindexer < 5; filterindexer++)
                    {
                        _customEntities[0].view[0].filters[filterindexer]._fieldid = _customEntities[0].attribute[0].FieldId;
                        CustomEntitiesUtilities.CreateViewFilters(view: _customEntities[0].view[0], filtertoSave: _customEntities[0].view[0].filters[filterindexer], executingProduct: _executingProduct);
                    }
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsSuccessfullyAddFilterOnCustomEntityAttributeOfTypeGreenlightCurrency_UITest":
                    CustomEntity currencyEntity = null;
                    foreach (CustomEntity entity in _customEntities)
                    {
                    if (entity.enableCurrencies) { currencyEntity = entity; break; }
                    } if(currencyEntity ==  null) 
                    {
                        throw new ArgumentException("Could not find custom entity with currency flag enabled!");
                    }
                    currencyEntity.view[0].addform = currencyEntity.form[0];
                    currencyEntity.view[0].editform = currencyEntity.form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(currencyEntity, currencyEntity.view[0], _executingProduct);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsUnsuccessfullyDeleteUDFAttributeWhenInUseAsColumn_UITest":
                    foreach (var udf in _userDefinedFields)
                    {
                        UserDefinedFieldsRepository.CreateUserDefinedField(udf, _executingProduct);
                    }
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    CustomEntitiesUtilities.CustomEntityViewField udfViewField = new CustomEntitiesUtilities.CustomEntityViewField(0, _customEntities[0].view[0]._viewid, 1, _userDefinedFields[0].FieldId, 0);
                    CustomEntitiesUtilities.CreateViewFields(_customEntities[0].view[0], udfViewField, _executingProduct);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityViewsSuccessfullyAddUDFsAsColumnsForViewInCustomEntity_UITest":
                    foreach (var udf in _userDefinedFields)
                    {
                        UserDefinedFieldsRepository.CreateUserDefinedField(udf, _executingProduct);
                    }
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                default:
                    _customEntities[0].view[0].addform = _customEntities[0].form[0];
                    _customEntities[0].view[0].editform = _customEntities[0].form[0];
                    resultview = CustomEntitiesUtilities.CreateCustomEntityView(_customEntities[0], _customEntities[0].view[0], _executingProduct);
                    foreach (CustomEntity ce in _customEntities)
                    {
                        for (int columnindexer = 0; columnindexer < ce.view[0].fields.Count; columnindexer++)
                        {
                            ce.view[0].fields[columnindexer]._fieldid = ce.attribute[columnindexer].FieldId;
                            if (ce.attribute[columnindexer]._fieldType != FieldType.Relationship && ce.attribute[columnindexer]._fieldType != FieldType.Comment)
                            {
                                CustomEntitiesUtilities.CreateViewFields(ce.view[0], ce.view[0].fields[columnindexer], _executingProduct);
                            }
                        }
                    }
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
            }
        }

        private List<Guid> GetFieldIDbyParentEntity(int entityid)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
            string strSQL2 = "select fieldid FROM customEntityAttributes where entityid = '" + entityid + "' and system_attribute = 'false' order by attributeid";

            return dbex_CodedUI.getGuidList(strSQL2); 
        }

        private string ColumnBuilder(string pane, string fieldname = null, string parent = null, string parent2 = null, string userDefinedField = null)
        {
            if (fieldname != null)
            {
                if (pane == "There are no columns selected.") 
                {
                    pane = string.Empty; 
                }
                //pane += "  ";
                if (parent != null)
                {
                    pane += fieldname + "(" + parent;
                    if (parent2 != null)
                    {
                        pane += ": " + parent2;
                    }
                    if (userDefinedField != null)
                    {
                        pane += ": " + userDefinedField;
                    }
                    pane += ")";
                }
                else
                {
                    pane += fieldname;
                }

                //String.Format("{0}{1}({2})", pane, fieldname, parent); 
            }
            else
            {
                pane = "There are no columns selected.";
            }
            return pane;
        }


        /// <summary>
        /// Moves object at position to new position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="currentpostition">current position of object to be moved</param>
        /// <param name="newposition">new position to place object</param>
        private void MoveObjectWithinList<T>(IList<T> list, int currentpostition, int newposition)
        {
            T columnindex = list[currentpostition];
            list[currentpostition] = list[newposition];
            list[newposition] = columnindex;
        }

        /// <summary>
        /// Creates a list of custom entity attributes that is the expected attributes after we import the data in lithium
        /// </summary>
        private List<CustomEntitiesUtilities.CustomEntityAttribute> ConstructExpectedAvailableFields(int attributeindexer)
        {
            List<CustomEntitiesUtilities.CustomEntityAttribute> availableFields = new List<CustomEntitiesUtilities.CustomEntityAttribute>();

            for (; attributeindexer < _customEntities[0].attribute.Count; attributeindexer++)
            {
                if (_customEntities[0].attribute[attributeindexer]._fieldType != FieldType.Comment)
                {
                    availableFields.Add(_customEntities[0].attribute[attributeindexer]);
                }
                if (_customEntities[0].attribute[attributeindexer]._fieldType == FieldType.Relationship)
                {
                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute nTo1Relationship = _customEntities[0].attribute[attributeindexer] as CustomEntitiesUtilities.CustomEntityNtoOneAttribute;

                    ConstructNToOneRelationshipFields(nTo1Relationship);

                    if (nTo1Relationship._UDFFields.Count > 0)
                    {
                        CreateUDFFolderInExpectedAvailableFieldsIfUDFsExist(nTo1Relationship);
                    }
                }
            }
            List<CustomEntitiesUtilities.CustomEntityAttribute> systemAttributesList = ReadSystemAttributesFieldIDs(_customEntities[0].entityId);
            foreach (CustomEntitiesUtilities.CustomEntityAttribute systemAttribute in systemAttributesList)
            {
                availableFields.Add(systemAttribute);
            }
            return availableFields;
        }

        ///// <summary>
        ///// Returns the related table for an N:1 Relationship with a custom entity using the entityID and the name of the relationship 
        ///// </summary>
        private static Guid GetNToOneRelatedTable(int entityID, string relationshipName)
        {
            Guid relatedTable = Guid.Empty;
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@relationshipName", relationshipName);
            SqlDataReader reader = dbex_CodedUI.GetReader("SELECT relatedtable FROM customEntityAttributes WHERE entityid = @entityID AND display_name = @relationshipName");
            while (reader.Read())
            {
                relatedTable = reader.GetGuid(0);
            }
            dbex_CodedUI.sqlexecute.Parameters.Clear();
            reader.Close();
            return relatedTable;
        }

        /// <summary>
        /// Returns a list of attributes that consist the fields of an n:1 relationship between two custom entities
        /// </summary>
        private List<CustomEntitiesUtilities.Field> ReadRelatedNToOneAttributes(int entityID, string relationshipName)
        {
            List<CustomEntitiesUtilities.Field> relatedAttributes = new List<CustomEntitiesUtilities.Field>();
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@tableID", GetNToOneRelatedTable(entityID, relationshipName));
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@fieldFrom", FieldFrom.GreenlightAttribute);
            SqlDataReader reader = dbex_CodedUI.GetReader("SELECT fieldid, description, IsForeignKey, tableid, RelatedTable FROM fields WHERE tableid = @tableID and fieldFrom = @fieldFrom");
            while (reader.Read())
            {
                CustomEntitiesUtilities.Field attribute = new CustomEntitiesUtilities.Field(reader.GetGuid(0), reader.GetString(1), reader.GetBoolean(2), FieldFrom.GreenlightAttribute, reader.GetGuid(3), reader.IsDBNull(4) ? Guid.Empty : reader.GetGuid(4));
                relatedAttributes.Add(attribute);
            }
            dbex_CodedUI.sqlexecute.Parameters.Clear();
            reader.Close();
            return relatedAttributes;
        }

        /// <summary>
        /// Returns a list of the system attributes of a custom entity
        /// </summary>
        private static List<CustomEntitiesUtilities.CustomEntityAttribute> ReadSystemAttributesFieldIDs(int entityID)
        {
            List<CustomEntitiesUtilities.CustomEntityAttribute> systemAttributesList = new List<CustomEntitiesUtilities.CustomEntityAttribute>();
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@entityID", entityID);
            bool systemAttribute = true;
            SqlDataReader reader = dbex_CodedUI.GetReader("SELECT display_name, fieldid, relationshiptype, fieldtype, relatedtable FROM customEntityAttributes where entityid = @entityID and system_attribute = '" + systemAttribute + "'");
            while (reader.Read())
            {
                if (!reader.IsDBNull(2))
                {
                    if (reader.GetByte(2) == (byte)RelationshipType.ManyToOne)
                    {
                        CustomEntitiesUtilities.CustomEntityNtoOneAttribute relationship = new CustomEntitiesUtilities.CustomEntityNtoOneAttribute(reader.GetString(0), reader.GetGuid(1), reader.GetByte(2), (FieldType)reader.GetByte(3), reader.GetGuid(4), systemAttribute);
                        systemAttributesList.Add(relationship);
                    }
                }
                else
                {
                    CustomEntitiesUtilities.CustomEntityAttribute attribute = new CustomEntitiesUtilities.CustomEntityAttribute(reader.GetString(0), reader.GetGuid(1), (FieldType)reader.GetByte(3), systemAttribute);
                    systemAttributesList.Add(attribute);
                }
            }
            dbex_CodedUI.sqlexecute.Parameters.Clear();
            reader.Close();
            return systemAttributesList;
        }

        /// <summary>
        /// Constructs the lists of the existing fields for an n:1 relationship 
        /// </summary>
        private static void ConstructNToOneRelationshipFields(CustomEntitiesUtilities.CustomEntityNtoOneAttribute nToOneRelationship)
        {
            nToOneRelationship._baseTableFields = new List<CustomEntitiesUtilities.Field>();
            nToOneRelationship._UDFFields = new List<CustomEntitiesUtilities.Field>();
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@tableID", nToOneRelationship._relatedTable);
            SqlDataReader reader = dbex_CodedUI.GetReader("SELECT fieldid, description, IsForeignKey, fieldFrom, tableid, RelatedTable FROM fields WHERE tableid = @tableID or tableid = (select userdefined_table from tables where tableid = @tableID )");
            while (reader.Read())
            {
                if (reader.GetInt32(3) == (int)FieldFrom.BaseTable)
                {
                    CustomEntitiesUtilities.Field relationshipField = new CustomEntitiesUtilities.Field(reader.GetGuid(0), reader.GetString(1), reader.GetBoolean(2), (FieldFrom)reader.GetInt32(3), reader.GetGuid(4), reader.IsDBNull(5) ? Guid.Empty : reader.GetGuid(5));
                    nToOneRelationship._baseTableFields.Add(relationshipField);
                }
                else if (reader.GetInt32(3) == (int)FieldFrom.UDF)
                {
                    CustomEntitiesUtilities.Field relationshipField = new CustomEntitiesUtilities.Field(reader.GetGuid(0), reader.GetString(1), reader.GetBoolean(2), (FieldFrom)reader.GetInt32(3), reader.GetGuid(4), reader.IsDBNull(5) ? Guid.Empty : reader.GetGuid(5));
                    nToOneRelationship._UDFFields.Add(relationshipField);
                }
            }
            dbex_CodedUI.sqlexecute.Parameters.Clear();
            reader.Close();
        }

        /// <summary>
        /// Checks to see if the n:1 relationship we pass has UDFs and if it does it creates the User Defined Fields forder and adds it in the expected available fields list  
        /// </summary>
        private void CreateUDFFolderInExpectedAvailableFieldsIfUDFsExist(CustomEntitiesUtilities.CustomEntityNtoOneAttribute nTo1Relationship)
        {
            nTo1Relationship._udfFolder = new CustomEntitiesUtilities.Field();
            nTo1Relationship._udfFolder.FieldId = nTo1Relationship._UDFFields[0]._tabledId;
            nTo1Relationship._udfFolder.DisplayName = "User Defined Fields";
            nTo1Relationship._udfFolder._isExpanded = false;
            nTo1Relationship._udfFolder._fieldType = FieldType.Relationship;
        }

        private string TruncateDisplayName(string attributename)
        {
            string attributelabel = string.Empty;
            if (attributename.Length >= 20)
            {
                attributelabel = attributename.Substring(0, 18) + "...";
            }
            else
            {
                attributelabel = attributename;
            }
            return attributelabel;
        }

        private string GetFilterGridRowText(string attributename, ConditionType conditionType, string value1 = "", string value2 = "")
        {
            string filtercriteria = EnumHelper.GetEnumDescription(conditionType);
            string attributelabel = string.Empty;
            string rowinnertext = string.Empty;
            if (attributename.Length >= 20)
            {
                attributelabel = attributename.Substring(0, 18) + "...";
            }
            else
            {
                attributelabel = attributename;
            }
            rowinnertext = attributelabel + filtercriteria;
            if (value1 != string.Empty) 
            {
                rowinnertext += value1;
                if (value2 != string.Empty)
                {
                    rowinnertext += " and " + value2;
                }
            }
            else
            {
                rowinnertext += "-";
            }
            rowinnertext += attributename;
            return rowinnertext;
        }

        private List<string> GetGridText(string Gridrow)
        {
            GridText.Add(Gridrow);
            return GridText;
        }

        private string FilterPaneString(List<string> paneTextCollection)
        {
            if (paneTextCollection.Count == 0)
            {
                return "There are no filters selected.";
            }
            else
            {
                StringBuilder selectedFilterPane = new StringBuilder();
                foreach (string iterator in paneTextCollection)
                {
                    selectedFilterPane.Append(iterator);
                }
                return selectedFilterPane.ToString();
            }
        }
    }
}
