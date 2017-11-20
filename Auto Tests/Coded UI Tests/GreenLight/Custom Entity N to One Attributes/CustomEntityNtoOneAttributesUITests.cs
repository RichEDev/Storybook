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
using Auto_Tests.Tools;
using Auto_Tests.UIMaps.CustomEntityNtoOneAttributesUIMapClasses;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using System.Configuration;
using System.Linq;
using Auto_Tests.UIMaps.CustomEntitiesUIMapClasses;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Forms;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;


namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_N_to_One_Attributes
{
    /// <summary>
    /// Summary description for CustomEntityNto1AttributesUITests
    /// </summary>
    [CodedUITest]
    public class CustomEntityNtoOneAttributesUITests
    {
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();

        private static List<CustomEntity> customEntities;

        private CustomEntityNtoOneAttributesUIMap cCustomEntitiesNtoOneAttributesMethods = new CustomEntityNtoOneAttributesUIMap();
       
        private CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();

        /// <summary>
        /// Current product to execute tests on
        /// Framework / Expenses
        /// </summary>
        private static ProductType _executingProduct;

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize(); 
            cSharedMethods = new SharedMethodsUIMap();
            _executingProduct = cGlobalVariables.GetProductFromAppConfig();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            cSharedMethods.Logon(_executingProduct, LogonType.administrator);
            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();
            Assert.IsNotNull(customEntities);
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            cSharedMethods.CloseBrowserWindow();
        }

        #region Successfully validate mandatory properties of n to one attribute
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyValidateMandatoryPropertiesforNtoOneAttributes_UITest()
        {
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to new N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            ///validate asterisks not fired
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.RelatedToAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayFieldAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.LookupFieldsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument1.SuggestionsLimitAsterisk));

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///Validate Message displayed
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributesExpectedValues.UICtl00_pnlMasterPopupPane1DisplayText = string.Format("Message from {0}\r\n\r\n\r\nPl" +
            "ease enter a Display name for this attribute.\r\nPlease select a Related to from the list.\r\nPlease select a Display field.\r\nPlease add a Lookup field to match.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributes();

            ///close message
            cCustomEntitiesNtoOneAttributesMethods.PressClose();

            cCustomEntitiesNtoOneAttributesMethods.ClickonGeneralDetailsTab();

            ///validate asterisks  firedwhere applicable
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayNameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.RelatedToAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayFieldAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.LookupFieldsAsterisk));

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            cCustomEntitiesNtoOneAttributesMethods.ClickonGeneralDetailsTab();
            ///set n to 1 control properties
            ntooneattributes.EntityComboBx.SelectedItem = GetTableNamebyTableID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relatedTable);

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///Validate Message displayed
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributesExpectedValues.UICtl00_pnlMasterPopupPane1DisplayText = string.Format("Message from {0}\r\n\r\n\r\nPl" +
            "ease enter a Display name for this attribute.\r\nPlease select a Display field.\r\nPlease add a Lookup field to match.", EnumHelper.GetEnumDescription(_executingProduct));

            ///close message
            cCustomEntitiesNtoOneAttributesMethods.PressClose();

            ///validate asterisks  firedwhere applicable
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.RelatedToAsterisk));
            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayFieldAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.LookupFieldsAsterisk));

            cCustomEntitiesNtoOneAttributesMethods.ClickonGeneralDetailsTab();
            /// set n to 1 control properties
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[33].DisplayName;

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///Validate Message displayed
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributesExpectedValues.UICtl00_pnlMasterPopupPane1DisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Display field.\r\nPlease add a Lookup field to match.", EnumHelper.GetEnumDescription(_executingProduct));
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributes();

            ///close message
            cCustomEntitiesNtoOneAttributesMethods.PressClose();


            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.RelatedToAsterisk));
            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayFieldAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.LookupFieldsAsterisk));

            string FieldNameToSelect = GetDisplayFieldNamebyFieldID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relationshipdisplayfield);

            /// set n to 1 control properties
            ntooneattributes.DisplayFieldComboBx.SelectedItem = FieldNameToSelect;

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///Validate Message displayed
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributesExpectedValues.UICtl00_pnlMasterPopupPane1DisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease add a Lookup field to match.", EnumHelper.GetEnumDescription(_executingProduct));
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributes();

            ///close message
            cCustomEntitiesNtoOneAttributesMethods.PressClose();
            cCustomEntitiesNtoOneAttributesMethods.ClickonGeneralDetailsTab();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayFieldAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.RelatedToAsterisk));
            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayFieldAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.LookupFieldsAsterisk));

            /// set n to 1 control properties
            cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();

            ntooneattributes.LookupFieldComboBx.SelectedItem = FieldNameToSelect;

            cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();

            ntooneattributes.SuggestionTxt.Text = "-1";

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///Validate Message displayed
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributesExpectedValues.UICtl00_pnlMasterPopupPane1DisplayText = string.Format("Message from {0}\r\n\r\n\r\nSuggestions limit must be a numeric value between 0 and 999.", EnumHelper.GetEnumDescription(_executingProduct));
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributes();

            ///close message
            cCustomEntitiesNtoOneAttributesMethods.PressClose();
            cCustomEntitiesNtoOneAttributesMethods.ClickonGeneralDetailsTab();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.RelatedToAsterisk));
            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.DisplayFieldAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.LookupFieldsAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument1.SuggestionsLimitAsterisk));

            ntooneattributes.SuggestionTxt.Text = "1";

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();
        }
        #endregion

        #region Successfully validate page standards for n to one attributes Modal
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyValidatePagestandardsforNtoOneAttributesModal_UITest()
        {
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to new N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            //Validate tabbing order
            CustomEntityNtoOneAttributes controls = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);
            Assert.IsTrue(controls.DisplayNameTxt.HasFocus);

            //Set focus to mandatory
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.Mandatory.HasFocus);

            //Set focus to display name
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.EntityComboBx.HasFocus);

            //Set focus to description
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.DescriptionTxt.HasFocus);

            //Set focus to tooltip
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.ToolTipTxt.HasFocus);

            controls.EntityComboBx.SelectedItem = "Employees";
            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            //Set focus to display field
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.DisplayFieldComboBx.HasFocus);

            //Set focus to lookup fields to match
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.LookupFieldCollection.HasFocus);

            //Tab sets Add focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.AddImg.HasFocus);

            //Tab sets delete focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.DeleteImg.HasFocus);

            //Tab sets suggestion limits
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.SuggestionTxt.HasFocus);

            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.SaveButton.HasFocus);

            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(controls.CancelButton.HasFocus);

            ///validate n to 1 modal
            cCustomEntitiesNtoOneAttributesMethods.ValidateNewNtoOneAttributeModalHeader();
            cCustomEntitiesNtoOneAttributesMethods.ValidateDiplayFieldLabel();
            cCustomEntitiesNtoOneAttributesMethods.ValidateEntityLabelExpectedValues.UIRelatedtoLabelInnerText = "Related to*";
            cCustomEntitiesNtoOneAttributesMethods.ValidateEntityLabel();
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryLabel();
            cCustomEntitiesNtoOneAttributesMethods.ValidateDescriptionLabel();
            cCustomEntitiesNtoOneAttributesMethods.ValidateTooltipLabel();
            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();
            cCustomEntitiesNtoOneAttributesMethods.ValidateLookupFieldsLabel();
            cCustomEntitiesNtoOneAttributesMethods.ValidateNoofSugestionsLabel();
            cCustomEntitiesNtoOneAttributesMethods.ValidateDisplayDropdownSelectedItem();

            HtmlInputButton saveButton = cCustomEntitiesNtoOneAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UISaveButton;
            string saveButtonExpectedText = "save";
            Assert.AreEqual(saveButtonExpectedText, saveButton.DisplayText);

            HtmlInputButton cancelButton = cCustomEntitiesNtoOneAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICancelButton;
            string cancelButtonExpectedText = "cancel";
            Assert.AreEqual(cancelButtonExpectedText, cancelButton.DisplayText);

        }
        #endregion

        #region Successfully verify shortcut keys for n to one attributes Modal
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyVerifyShortcutKeysForNtoOneAttributesModal_UITest()
        {
            int indexe = 0;
            for (int indexer = 0; indexer < customEntities[0].attribute.Count; indexer++)
            {
                customEntities[0].attribute[indexer]._attributeid = 0;
                if (customEntities[0].attribute[indexer]._fieldType == FieldType.Relationship)
                {
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[indexer], customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].attribute[indexer]._attributeid = result;
                    indexe++;
                    if (indexe == 1)
                    {
                        indexer = customEntities[0].attribute.Count;
                    }
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to new N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            ///Close modal using shortcut
            Keyboard.SendKeys("{Escape}");

            ///Navigate to edit N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName);

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = GetDisplayFieldNamebyFieldID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34])._relationshipdisplayfield);
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute matchFieldsList = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33];
            UITestControlCollection collectionOflistItems = ntooneattributes.LookupFieldCollection.Items;


            ///delete lookup fields using short cuts
            foreach (CustomEntitiesUtilities.RelationshipMatchFieldListItem lookupFields in matchFieldsList._matchFieldListItems)
            {
                //Set selected item
                ntooneattributes.LookupFieldCollection.SelectedItems = new string[] { GetDisplayFieldNamebyFieldID(lookupFields._fieldid) };
                Keyboard.PressModifierKeys(ModifierKeys.Control);
                Keyboard.SendKeys("2");
                Keyboard.ReleaseModifierKeys(ModifierKeys.Control);
            }

            matchFieldsList = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34];
            foreach (CustomEntitiesUtilities.RelationshipMatchFieldListItem item in matchFieldsList._matchFieldListItems)
            {
                string MatchFieldName = GetDisplayFieldNamebyFieldID(item._fieldid);
                /// set n to 1 lookup field to match control properties
                ///open Lookup fields using shortcut
                Keyboard.PressModifierKeys(ModifierKeys.Control);
                Keyboard.SendKeys("1");
                Keyboard.ReleaseModifierKeys(ModifierKeys.Control);
                ntooneattributes.LookupFieldComboBx.SelectedItem = MatchFieldName;
                cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();
            }

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///validat attributes grid for created attribute
            cCustomEntitiesNtoOneAttributesMethods.ValidateAttributesGrid(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName, customEntities[0].attribute[33]._description, "Relationship", customEntities[0].attribute[33]._isAuditIdenity.ToString());

            ///click edit icon on custom entity
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName);

            //Refreshes the controls
            ntooneattributes.FindControls();

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attributeToAdd = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34]);

            //Assert attribute has correct property values
            Assert.AreEqual(ntooneattributes.EntityComboBx.SelectedItem, "Employees");
            Assert.AreEqual(ntooneattributes.EntityComboBx.Enabled, false);
            Assert.AreEqual(Nto1DisplayFieldProperty, ntooneattributes.DisplayFieldComboBx.SelectedItem);
            int index = 0;
            foreach (HtmlListItem item in ntooneattributes.LookupFieldCollection.Items)
            {
                Assert.AreEqual(GetDisplayFieldNamebyFieldID(attributeToAdd._matchFieldListItems[index++]._fieldid), item.InnerText);
            }
            string sMaxRowExpected = "";
            if (attributeToAdd._maxRows != null)
            {
                sMaxRowExpected = attributeToAdd._maxRows.ToString();
            }

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();
        }
        #endregion
        
        #region Successfully add n to one attribute to basetable
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyAddNtoOneAttributetoBaseTables_UITest()
        {
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to new N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            string Nto1EntityProperty = GetTableNamebyTableID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relatedTable);
            ntooneattributes.EntityComboBx.SelectedItem = Nto1EntityProperty;
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[33].DisplayName;
            ntooneattributes.DescriptionTxt.Text = customEntities[0].attribute[33]._description;
            ntooneattributes.ToolTipTxt.Text = customEntities[0].attribute[33]._tooltip;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = GetDisplayFieldNamebyFieldID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relationshipdisplayfield);
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute matchFieldsList = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33];
            foreach (CustomEntitiesUtilities.RelationshipMatchFieldListItem item in matchFieldsList._matchFieldListItems)
            {
                string MatchFieldName = GetDisplayFieldNamebyFieldID(item._fieldid);
                /// set n to 1 lookup field to match control properties
                cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();
                ntooneattributes.LookupFieldComboBx.SelectedItem = MatchFieldName;
                cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();
            }

            ntooneattributes.SuggestionTxt.Text = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._maxRows.ToString();

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///validat attributes grid for created attribute
            cCustomEntitiesNtoOneAttributesMethods.ValidateAttributesGrid(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName, customEntities[0].attribute[33]._description, "Relationship", customEntities[0].attribute[33]._isAuditIdenity.ToString());

            ///click edit icon on custom entity
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName);

            //Refreshes the controls
            ntooneattributes.FindControls();

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attributeToAdd = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33]);

            //Assert attribute has correct property values
            Assert.AreEqual(Nto1EntityProperty, ntooneattributes.EntityComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd.DisplayName, ntooneattributes.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, ntooneattributes.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, ntooneattributes.ToolTipTxt.Text);
            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();
            Assert.AreEqual(Nto1DisplayFieldProperty, ntooneattributes.DisplayFieldComboBx.SelectedItem);
            int index = 0;
            foreach (HtmlListItem item in ntooneattributes.LookupFieldCollection.Items)
            {
                Assert.AreEqual(GetDisplayFieldNamebyFieldID(attributeToAdd._matchFieldListItems[index++]._fieldid), item.InnerText);
            }
            string sMaxRowExpected = "";
            if (attributeToAdd._maxRows != null)
            {
                sMaxRowExpected = attributeToAdd._maxRows.ToString();
            }
            Assert.AreEqual(sMaxRowExpected, ntooneattributes.SuggestionTxt.Text);
            Assert.AreEqual(false, ntooneattributes.EntityComboBx.Enabled);

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();

        }
        #endregion

        #region Successfully edit N to One attribute to basetable
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyEditNtoOneAttributetoBaseTables_UITest()
        {
            int indexe = 0;
            for (int indexer = 0; indexer < customEntities[0].attribute.Count; indexer++)
            {
                customEntities[0].attribute[indexer]._attributeid = 0;
                if (customEntities[0].attribute[indexer]._fieldType == FieldType.Relationship)
                {
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[indexer], customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].attribute[indexer]._attributeid = result;
                    indexe++;
                    if (indexe == 1)
                    {
                        indexer = customEntities[0].attribute.Count;
                    }
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to edit N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName);

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[34].DisplayName;
            ntooneattributes.DescriptionTxt.Text = customEntities[0].attribute[34]._description;
            ntooneattributes.ToolTipTxt.Text = customEntities[0].attribute[34]._tooltip;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = GetDisplayFieldNamebyFieldID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34])._relationshipdisplayfield);
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute matchFieldsList = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33];
            UITestControlCollection collectionOflistItems = ntooneattributes.LookupFieldCollection.Items;

            //delete all lookup fields
            foreach (CustomEntitiesUtilities.RelationshipMatchFieldListItem lookupFields in matchFieldsList._matchFieldListItems)
            {
                //Set selected item
                ntooneattributes.LookupFieldCollection.SelectedItems = new string[] { GetDisplayFieldNamebyFieldID(lookupFields._fieldid) };
                cCustomEntitiesNtoOneAttributesMethods.ClickDeleteLookupFieldIcon();
            }

            matchFieldsList = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34];
            foreach (CustomEntitiesUtilities.RelationshipMatchFieldListItem item in matchFieldsList._matchFieldListItems)
            {
                string MatchFieldName = GetDisplayFieldNamebyFieldID(item._fieldid);
                /// set n to 1 lookup field to match control properties
                cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();
                ntooneattributes.LookupFieldComboBx.SelectedItem = MatchFieldName;
                cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();
            }

            ntooneattributes.SuggestionTxt.Text = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34])._maxRows.ToString();

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///validat attributes grid for created attribute
            cCustomEntitiesNtoOneAttributesMethods.ValidateAttributesGrid(cSharedMethods.browserWindow, customEntities[0].attribute[34].DisplayName, customEntities[0].attribute[34]._description, "Relationship", customEntities[0].attribute[34]._isAuditIdenity.ToString());

            ///click edit icon on custom entity
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[34].DisplayName);

            //Refreshes the controls
            ntooneattributes.FindControls();

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attributeToAdd = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34]);

            //Assert attribute has correct property values
            Assert.AreEqual(ntooneattributes.EntityComboBx.SelectedItem, "Employees");
            Assert.AreEqual(ntooneattributes.EntityComboBx.Enabled, false);
            Assert.AreEqual(attributeToAdd.DisplayName, ntooneattributes.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, ntooneattributes.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, ntooneattributes.ToolTipTxt.Text);

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            Assert.AreEqual(Nto1DisplayFieldProperty, ntooneattributes.DisplayFieldComboBx.SelectedItem);
            int index = 0;
            foreach (HtmlListItem item in ntooneattributes.LookupFieldCollection.Items)
            {
                Assert.AreEqual(GetDisplayFieldNamebyFieldID(attributeToAdd._matchFieldListItems[index++]._fieldid), item.InnerText);
            }
            string sMaxRowExpected = "";
            if (attributeToAdd._maxRows != null)
            {
                sMaxRowExpected = attributeToAdd._maxRows.ToString();
            }
            Assert.AreEqual(sMaxRowExpected, ntooneattributes.SuggestionTxt.Text);
            Assert.AreEqual(false, ntooneattributes.EntityComboBx.Enabled);

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();

        }
        #endregion

        #region Unsuccessfully add N to One attribute to basetable
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CustomEntityNToOneAttributesUnsuccessfullyAddNtoOneAttributestoBaseTables_UITest()
        {
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to new N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            string Nto1EntityProperty = GetTableNamebyTableID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relatedTable);
            ntooneattributes.EntityComboBx.SelectedItem = Nto1EntityProperty;
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[33].DisplayName;
            ntooneattributes.DescriptionTxt.Text = customEntities[0].attribute[33]._description;
            ntooneattributes.ToolTipTxt.Text = customEntities[0].attribute[33]._tooltip;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = GetDisplayFieldNamebyFieldID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relationshipdisplayfield);
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;
            /// set n to 1 control properties
            cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();

            ntooneattributes.LookupFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;
            cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();
            ntooneattributes.SuggestionTxt.Text = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._maxRows.ToString();

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();

            ///validat attributes grid for created attribute
            cCustomEntitiesNtoOneAttributesMethods.ValidateAttributesGrid(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName, customEntities[0].attribute[33]._description, "Relationship", customEntities[0].attribute[33]._isAuditIdenity.ToString());

        }
        #endregion

        #region Unsuccessfully edit N to One attribute to basetable
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesUnsuccessfullyEditNtoOneAttributetoBaseTables_UITest()
        {
            int indexe = 0;
            for (int indexer = 0; indexer < customEntities[0].attribute.Count; indexer++)
            {
                customEntities[0].attribute[indexer]._attributeid = 0;
                if (customEntities[0].attribute[indexer]._fieldType == FieldType.Relationship)
                {
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[indexer], customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].attribute[indexer]._attributeid = result;
                    indexe++;
                    if (indexe == 1)
                    {
                        indexer = customEntities[0].attribute.Count;
                    }
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to Edit N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName);

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[34].DisplayName;
            ntooneattributes.DescriptionTxt.Text = customEntities[0].attribute[34]._description;
            ntooneattributes.ToolTipTxt.Text = customEntities[0].attribute[34]._tooltip;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = GetDisplayFieldNamebyFieldID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relationshipdisplayfield);
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute matchFieldsList = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34];
            foreach (CustomEntitiesUtilities.RelationshipMatchFieldListItem item in matchFieldsList._matchFieldListItems)
            {
                string MatchFieldName = GetDisplayFieldNamebyFieldID(item._fieldid);
                /// set n to 1 lookup field to match control properties
                cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();
                ntooneattributes.LookupFieldComboBx.SelectedItem = MatchFieldName;
                cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();
            }

            ntooneattributes.SuggestionTxt.Text = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34])._maxRows.ToString();

            ///Cancel Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();

            ///validat attributes grid for created attribute
            cCustomEntitiesNtoOneAttributesMethods.ValidateAttributesGrid(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName, customEntities[0].attribute[33]._description, "Relationship", customEntities[0].attribute[33]._isAuditIdenity.ToString());

            ///click edit icon on custom entity
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName);

            //Refreshes the controls
            ntooneattributes.FindControls();

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attributeToAdd = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33]);

            //Assert attribute has correct property values
            Assert.AreEqual(ntooneattributes.EntityComboBx.SelectedItem, "Employees");
            Assert.AreEqual(ntooneattributes.EntityComboBx.Enabled, false);
            Assert.AreEqual(attributeToAdd.DisplayName, ntooneattributes.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, ntooneattributes.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, ntooneattributes.ToolTipTxt.Text);

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            Assert.AreEqual(Nto1DisplayFieldProperty, ntooneattributes.DisplayFieldComboBx.SelectedItem);
            foreach (HtmlListItem item in ntooneattributes.LookupFieldCollection.Items)
            {
                Assert.IsNotNull(attributeToAdd._matchFieldListItems.Find(x => GetDisplayFieldNamebyFieldID(x._fieldid) == item.InnerText));
            }
            string sMaxRowExpected = "";
            if (attributeToAdd._maxRows != null)
            {
                sMaxRowExpected = attributeToAdd._maxRows.ToString();
            }
            Assert.AreEqual(sMaxRowExpected, ntooneattributes.SuggestionTxt.Text);
            Assert.AreEqual(false, ntooneattributes.EntityComboBx.Enabled);

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();
        }
        #endregion

        #region Successfully delete N to One attribute to basetable
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyDeleteNtoOneAttributetoBaseTables_UITest()
        {
            int index = 0;
            for (int indexer = 0; indexer < customEntities[0].attribute.Count; indexer++)
            {
                customEntities[0].attribute[indexer]._attributeid = 0;
                if (customEntities[0].attribute[indexer]._fieldType == FieldType.Relationship)
                {
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[indexer], customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].attribute[indexer]._attributeid = result;
                    index++;
                    if (index == 1)
                    {
                        indexer = customEntities[0].attribute.Count;
                    }
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Click delete on N to One Attribute to delete
            cCustomEntitiesNtoOneAttributesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName);

            ///Clcik Confirm deletion of Attribute
            cCustomEntitiesNtoOneAttributesMethods.ClickConfirmDeletionOfAttribute();

            ///validat attributes grid for N to One attribute deleted
            cCustomEntitiesNtoOneAttributesMethods.ValidateAttributeDeletion(customEntities[0].attribute[33].DisplayName);

        }
        #endregion

        #region Unsuccessfully delete N to One attribute to basetable
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesUnsuccessfullyDeleteNtoOneAttributetoBaseTables_UITest()
        {
            int index = 0;
            for (int indexer = 0; indexer < customEntities[0].attribute.Count; indexer++)
            {
                customEntities[0].attribute[indexer]._attributeid = 0;
                if (customEntities[0].attribute[indexer]._fieldType == FieldType.Relationship)
                {
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[indexer], customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].attribute[indexer]._attributeid = result;
                    index++;
                    if (index == 1)
                    {
                        indexer = customEntities[0].attribute.Count;
                    }
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Click delete on N to One Attribute to delete
            cCustomEntitiesNtoOneAttributesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName);

            ///Cancel Deletion of Attribute
            cCustomEntitiesNtoOneAttributesMethods.ClickCancelDeletionOfAttribute();

            ///validat attributes grid for N to One attribute not deleted
            cCustomEntitiesNtoOneAttributesMethods.ValidateAttributesGrid(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName, customEntities[0].attribute[33]._description, "Relationship", customEntities[0].attribute[33]._isAuditIdenity.ToString());

        }
        #endregion

        #region Unsuccessfully add duplicate N to One attribute to base table
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesUnsuccessfullyAddDuplicateNtoOneAttributetoBaseTables_UITest()
        {
            int index = 0;
            for (int indexer = 0; indexer < customEntities[0].attribute.Count; indexer++)
            {
                customEntities[0].attribute[indexer]._attributeid = 0;
                if (customEntities[0].attribute[indexer]._fieldType == FieldType.Relationship)
                {
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[indexer], customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].attribute[indexer]._attributeid = result;
                    index++;
                    if (index == 1)
                    {
                        indexer = customEntities[0].attribute.Count;
                    }
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to new N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            string Nto1EntityProperty = GetTableNamebyTableID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relatedTable);
            ntooneattributes.EntityComboBx.SelectedItem = Nto1EntityProperty;
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[33].DisplayName;
            ntooneattributes.DescriptionTxt.Text = customEntities[0].attribute[33]._description;
            ntooneattributes.ToolTipTxt.Text = customEntities[0].attribute[33]._tooltip;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = GetDisplayFieldNamebyFieldID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relationshipdisplayfield);
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute matchFieldsList = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33];
            foreach (CustomEntitiesUtilities.RelationshipMatchFieldListItem item in matchFieldsList._matchFieldListItems)
            {
                string MatchFieldName = GetDisplayFieldNamebyFieldID(item._fieldid);
                /// set n to 1 lookup field to match control properties
                cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();
                ntooneattributes.LookupFieldComboBx.SelectedItem = MatchFieldName;
                cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();
            }

            ntooneattributes.SuggestionTxt.Text = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._maxRows.ToString();

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributesExpectedValues.UICtl00_pnlMasterPopupPane1DisplayText = string.Format("Message from {0}\r\n\r\n\r\nAn attribute or relationship with this Display name already exists.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributes();

            ///close message
            cCustomEntitiesNtoOneAttributesMethods.PressClose();
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();
        }
        #endregion

        #region Unsuccessfully edit duplicate N to One attribute to base table
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesUnsuccessfullyEditDuplicateNtoOneAttributetoBaseTables_UITest()
        {
            int index = 0;
            for (int indexer = 0; indexer < customEntities[0].attribute.Count; indexer++)
            {
                customEntities[0].attribute[indexer]._attributeid = 0;
                if (customEntities[0].attribute[indexer]._fieldType == FieldType.Relationship)
                {
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[indexer], customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].attribute[indexer]._attributeid = result;
                    index++;
                    if (index == 2)
                    {
                        indexer = customEntities[0].attribute.Count;
                    }
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to Edit N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[33].DisplayName);

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[34].DisplayName;
            ntooneattributes.DescriptionTxt.Text = customEntities[0].attribute[34]._description;
            ntooneattributes.ToolTipTxt.Text = customEntities[0].attribute[34]._tooltip;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = GetDisplayFieldNamebyFieldID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34])._relationshipdisplayfield);
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;

            ntooneattributes.SuggestionTxt.Text = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[34])._maxRows.ToString();

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributesExpectedValues.UICtl00_pnlMasterPopupPane1DisplayText = string.Format("Message from {0}\r\n\r\n\r\nAn attribute or relationship with this Display name already exists.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntitiesNtoOneAttributesMethods.ValidateMandatoryPropertiesofNtoOneAttributes();

            ///close message
            cCustomEntitiesNtoOneAttributesMethods.PressClose();
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();
        }
        #endregion

        #region Successfully add n to one attribute to custom entity
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyAddNtoOneAttributetoCustomEntity_UITest()
        {
            foreach (var attribute in customEntities[1].attribute)
            {
                if (attribute._fieldType != FieldType.Relationship && attribute._fieldType != FieldType.List)
                {
                    attribute._attributeid = 0;
                    int result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntities[1], attribute, _executingProduct);
                    Assert.IsTrue(result > 0);
                    attribute._attributeid = result;
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to new N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            string Nto1EntityProperty = customEntities[1].pluralName;
            ntooneattributes.EntityComboBx.SelectedItem = Nto1EntityProperty;
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[35].DisplayName;
            ntooneattributes.DescriptionTxt.Text = customEntities[0].attribute[35]._description;
            ntooneattributes.ToolTipTxt.Text = customEntities[0].attribute[35]._tooltip;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = customEntities[1].attribute[0].DisplayName;
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;

            string MatchFieldName = customEntities[1].attribute[1].DisplayName;
            /// set n to 1 lookup field to match control properties
            cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();
            ntooneattributes.LookupFieldComboBx.SelectedItem = MatchFieldName;
            cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();


            ntooneattributes.SuggestionTxt.Text = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[35])._maxRows.ToString();

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///validat attributes grid for created attribute
            cCustomEntitiesNtoOneAttributesMethods.ValidateAttributesGrid(cSharedMethods.browserWindow, customEntities[0].attribute[35].DisplayName, customEntities[0].attribute[35]._description, "Relationship", customEntities[0].attribute[35]._isAuditIdenity.ToString());

            ///click edit icon on custom entity
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[35].DisplayName);

            //Refreshes the controls
            ntooneattributes.FindControls();

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attributeToAdd = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[35]);
            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attributeTolookup = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[1].attribute[1]);

            //Assert attribute has correct property values
            Assert.AreEqual(Nto1EntityProperty, ntooneattributes.EntityComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd.DisplayName, ntooneattributes.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, ntooneattributes.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, ntooneattributes.ToolTipTxt.Text);

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            Assert.AreEqual(Nto1DisplayFieldProperty, ntooneattributes.DisplayFieldComboBx.SelectedItem);
            foreach (HtmlListItem item in ntooneattributes.LookupFieldCollection.Items)
            {
                Assert.AreEqual(attributeTolookup.DisplayName, item.InnerText);
            }
            string sMaxRowExpected = "";
            if (attributeToAdd._maxRows != null)
            {
                sMaxRowExpected = attributeToAdd._maxRows.ToString();
            }
            Assert.AreEqual(sMaxRowExpected, ntooneattributes.SuggestionTxt.Text);
            Assert.AreEqual(false, ntooneattributes.EntityComboBx.Enabled);

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();

        }
        #endregion

        #region Successfully edit n to one attribute to custom entity
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyEditNtoOneAttributetoCustomEntity_UITest()
        {
            foreach (var attribute in customEntities[1].attribute)
            {
                if (attribute._fieldType != FieldType.Relationship && attribute._fieldType != FieldType.List)
                {
                    attribute._attributeid = 0;
                    int result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntities[1], attribute, _executingProduct);
                    Assert.IsTrue(result > 0);
                    attribute._attributeid = result;
                }
            }

            var tmpatt = (from x in customEntities[0].attribute
                          where x.DisplayName == "N : 1 Relationship Attribute to Custom Entity"
                          select x).FirstOrDefault();
            if (tmpatt != null)
            {
                CustomEntitiesUtilities.CustomEntityNtoOneAttribute updateatt = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)tmpatt;
                updateatt._relatedTable = customEntities[1].tableId;
                updateatt._relationshipdisplayfield = customEntities[1].attribute[1].FieldId;
                CustomEntitiesUtilities.RelationshipMatchFieldListItem listmatch = new CustomEntitiesUtilities.RelationshipMatchFieldListItem();
                listmatch._fieldid = customEntities[1].attribute[0].FieldId;
                updateatt._matchFieldListItems.Clear();
                updateatt._matchFieldListItems.Add(listmatch);
                if (updateatt._fieldType == FieldType.Relationship)
                {
                    updateatt._attributeid = 0;
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], updateatt, customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    updateatt._attributeid = result;
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to edit N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[35].DisplayName);

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            string Nto1EntityProperty = customEntities[1].pluralName;
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[36].DisplayName;
            ntooneattributes.DescriptionTxt.Text = customEntities[0].attribute[36]._description;
            ntooneattributes.ToolTipTxt.Text = customEntities[0].attribute[36]._tooltip;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = customEntities[1].attribute[0].DisplayName;
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;

            //Clear selected item
            ntooneattributes.LookupFieldCollection.SelectedItems = new string[] { customEntities[1].attribute[0].DisplayName };
            cCustomEntitiesNtoOneAttributesMethods.ClickDeleteLookupFieldIcon();

            /// set n to 1 lookup field to match control properties
            string MatchFieldName = customEntities[1].attribute[1].DisplayName;
            cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();
            ntooneattributes.LookupFieldComboBx.SelectedItem = MatchFieldName;
            cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();

            ntooneattributes.SuggestionTxt.Text = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[36])._maxRows.ToString();

            ///Save attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///validat attributes grid for created attribute
            cCustomEntitiesNtoOneAttributesMethods.ValidateAttributesGrid(cSharedMethods.browserWindow, customEntities[0].attribute[36].DisplayName, customEntities[0].attribute[36]._description, "Relationship", customEntities[0].attribute[36]._isAuditIdenity.ToString());

            ///click edit icon on custom entity
            cCustomEntitiesNtoOneAttributesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].attribute[36].DisplayName);

            //Refreshes the controls
            ntooneattributes.FindControls();

            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attributeToAdd = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[36]);
            CustomEntitiesUtilities.CustomEntityNtoOneAttribute attributeTolookup = ((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[1].attribute[1]);

            //Assert attribute has correct property values
            Assert.AreEqual(Nto1EntityProperty, ntooneattributes.EntityComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd.DisplayName, ntooneattributes.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, ntooneattributes.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, ntooneattributes.ToolTipTxt.Text);

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            Assert.AreEqual(Nto1DisplayFieldProperty, ntooneattributes.DisplayFieldComboBx.SelectedItem);
            foreach (HtmlListItem item in ntooneattributes.LookupFieldCollection.Items)
            {
                Assert.AreEqual(attributeTolookup.DisplayName, item.InnerText);
            }
            string sMaxRowExpected = "";
            if (attributeToAdd._maxRows != null)
            {
                sMaxRowExpected = attributeToAdd._maxRows.ToString();
            }
            Assert.AreEqual(sMaxRowExpected, ntooneattributes.SuggestionTxt.Text);
            Assert.AreEqual(false, ntooneattributes.EntityComboBx.Enabled);

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();

        }
        #endregion

        #region Successfully Verify new n to one attribute lists all attribute types as possible display field to custom entity
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyVerifyNewNtoOneAttributeListsAllAttributeTypesAsPossibleDisplayFieldToCustomEntity_UITest()
        {
            for (int indexer = 0; indexer <= 16; indexer++)
            {
                customEntities[0].attribute[indexer]._attributeid = 0;
                if (customEntities[0].attribute[indexer]._fieldType != FieldType.Relationship)
                {
                    int result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntities[0], (CustomEntitiesUtilities.CustomEntityAttribute)customEntities[0].attribute[indexer], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].attribute[indexer]._attributeid = result;
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[1].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to new N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            string Nto1EntityProperty = customEntities[0].pluralName;
            ntooneattributes.EntityComboBx.SelectedItem = Nto1EntityProperty;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            ///create list of expected attributes
            List<string> expectedoptionslist = new List<string>();
            for (int indexer = 0; indexer <= 16; indexer++)
            {
                expectedoptionslist.Add(customEntities[0].attribute[indexer].DisplayName);
            }
            expectedoptionslist.Sort();
            expectedoptionslist.Insert(0,"[None]");

            ///Assert list matches in count with dropdown options count
            Assert.AreEqual(expectedoptionslist.Count, ntooneattributes.DisplayFieldComboBx.Items.Count);
            bool OptionsCorrect = true;
            for (int i = 0; i <= ntooneattributes.DisplayFieldComboBx.Items.Count-1; i++)
            {
                if (((HtmlListItem)(ntooneattributes.DisplayFieldComboBx.Items[i])).InnerText != expectedoptionslist[i])
                {
                    OptionsCorrect = false;
                    break;
                }
            }
            Assert.IsTrue(OptionsCorrect);

            cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();
            ///Assert list matches in count with dropdown options count
            Assert.AreEqual(expectedoptionslist.Count, ntooneattributes.LookupFieldComboBx.Items.Count);
            bool OptionsCorrect2 = true;
            for (int i = 0; i <= ntooneattributes.LookupFieldComboBx.Items.Count - 1; i++)
            {
                if (((HtmlListItem)(ntooneattributes.LookupFieldComboBx.Items[i])).InnerText != expectedoptionslist[i])
                {
                    OptionsCorrect2 = false;
                    break;
                }
            }
            Assert.IsTrue(OptionsCorrect2);

            cCustomEntitiesNtoOneAttributesMethods.PressCancelSavingLookupFieldButton();
            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressCancel();

        }
        #endregion

        #region Unsuccessfully add N to One attribute to base table where Suggestions limit is invalid
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesUnsuccessfullyAddNtoOneAttributeWhereSuggestionslimitIsInvalid_UITest()
        {
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///Navigate to new N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            CustomEntityNtoOneAttributes ntooneattributes = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            ///set n to 1 control properties
            string Nto1EntityProperty = GetTableNamebyTableID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relatedTable);
            ntooneattributes.EntityComboBx.SelectedItem = Nto1EntityProperty;
            ntooneattributes.DisplayNameTxt.Text = customEntities[0].attribute[33].DisplayName;
            ntooneattributes.DescriptionTxt.Text = customEntities[0].attribute[33]._description;
            ntooneattributes.ToolTipTxt.Text = customEntities[0].attribute[33]._tooltip;

            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            string Nto1DisplayFieldProperty = GetDisplayFieldNamebyFieldID(((CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[0].attribute[33])._relationshipdisplayfield);
            ntooneattributes.DisplayFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;
            /// set n to 1 control properties
            cCustomEntitiesNtoOneAttributesMethods.ClickAddLookupFieldIcon();

            ntooneattributes.LookupFieldComboBx.SelectedItem = Nto1DisplayFieldProperty;
            cCustomEntitiesNtoOneAttributesMethods.PressSaveLookupFieldButton();
            ntooneattributes.SuggestionTxt.Text = "-1";

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///validat message displayed
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpensesExpectedValues.UICtl00_pnlMasterPopupPane2InnerText = string.Format("Message from {0}\r\n\r\n\r\nSuggestions limit must be a numeric value between 0 and 999.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpenses();

            cCustomEntitiesNtoOneAttributesMethods.PressClose();

            ntooneattributes.SuggestionTxt.Text = ")%";

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///validat message displayed
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpensesExpectedValues.UICtl00_pnlMasterPopupPane2InnerText = string.Format("Message from {0}\r\n\r\n\r\nSuggestions limit must be a numeric value between 0 and 999.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpenses();

            cCustomEntitiesNtoOneAttributesMethods.PressClose();
            ntooneattributes.SuggestionTxt.Text = "NaN";

            ///Cancel saving attribute
            cCustomEntitiesNtoOneAttributesMethods.PressSave();

            ///validat message displayed
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpensesExpectedValues.UICtl00_pnlMasterPopupPane2InnerText = string.Format("Message from {0}\r\n\r\n\r\nSuggestions limit must be a numeric value between 0 and 999.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpenses();

            cCustomEntitiesNtoOneAttributesMethods.PressClose();
        }
        #endregion

        #region Unsuccessfully delete standard attribute referenced by n to one attribute in another custom entity
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesUnsuccessfullyDeleteStandardAttributeReferencedByNtoOneinAnotherCustomEntity_UITest()
        {
            foreach (var attribute in customEntities[1].attribute)
            {
                if (attribute._fieldType != FieldType.Relationship && attribute._fieldType != FieldType.List)
                {
                    attribute._attributeid = 0;
                    int result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntities[1], attribute, _executingProduct);
                    attribute._attributeid = result;
                }
            }
            var tmpatt = (from x in customEntities[0].attribute
                          where x.DisplayName == "N : 1 Relationship Attribute to Custom Entity"
                          select x).FirstOrDefault();
            if (tmpatt != null)
            {
                CustomEntitiesUtilities.CustomEntityNtoOneAttribute updateatt = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)tmpatt;
                updateatt._relatedTable = customEntities[1].tableId;
                updateatt._relationshipdisplayfield = customEntities[1].attribute[1].FieldId;
                Auto_Tests.Tools.CustomEntitiesUtilities.RelationshipMatchFieldListItem li = new CustomEntitiesUtilities.RelationshipMatchFieldListItem();
                li._fieldid = customEntities[1].attribute[0].FieldId;
                updateatt._matchFieldListItems.Clear();
                updateatt._matchFieldListItems.Add(li);
                if (updateatt._fieldType == FieldType.Relationship)
                {
                    updateatt._attributeid = 0;
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], updateatt, customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    updateatt._attributeid = result;
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[1].entityId);

            ///Navigate to Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickAttributesLink();

            ///click delete N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, customEntities[1].attribute[0].DisplayName);

            cCustomEntitiesNtoOneAttributesMethods.ClickConfirmDeletionOfAttribute();

            ///verify attribute not deleted
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpensesExpectedValues.UICtl00_pnlMasterPopupPane2InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe attribute cannot be deleted as it is used as a look-up field in a n:1 relationship attribute.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpenses();
            cCustomEntitiesNtoOneAttributesMethods.PressClose();

            ///click delete N to One Attributes
            cCustomEntitiesNtoOneAttributesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, customEntities[1].attribute[1].DisplayName);

            cCustomEntitiesNtoOneAttributesMethods.ClickConfirmDeletionOfAttribute();

            ///verify attribute not deleted
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpensesExpectedValues.UICtl00_pnlMasterPopupPane2InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe attribute cannot be deleted as it is used as a display field or lookup display field in a n:1 relationship attribute.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpenses();
            cCustomEntitiesNtoOneAttributesMethods.PressClose();
        }

        #endregion

        #region Unsuccessfully delete Custom Entity whose attribute is referenced by n to one attribute in another custom entity
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesUnsuccessfullyDeleteCustomEntitywhoseAttributeisReferencedByNtoOneinAnotherCustomEntity_UITest()
        {
            foreach(var attribute in customEntities[1].attribute)
            {
                if (attribute._fieldType != FieldType.Relationship && attribute._fieldType != FieldType.List)
                {
                    attribute._attributeid = 0;
                    int result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntities[1], attribute, _executingProduct);
                    Assert.IsTrue(result > 0);
                    attribute._attributeid = result;
                }
            }

            var tmpatt = (from x in customEntities[0].attribute
                          where x.DisplayName == "N : 1 Relationship Attribute to Custom Entity"
                          select x).FirstOrDefault();
            if (tmpatt != null)
            {
                CustomEntitiesUtilities.CustomEntityNtoOneAttribute updateatt = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)tmpatt;
                updateatt._attributeid = 0;
                updateatt._relatedTable = customEntities[1].tableId;
                updateatt._relationshipdisplayfield = customEntities[1].attribute[1].FieldId;
                Auto_Tests.Tools.CustomEntitiesUtilities.RelationshipMatchFieldListItem li = new CustomEntitiesUtilities.RelationshipMatchFieldListItem();
                li._fieldid = customEntities[1].attribute[0].FieldId;
                updateatt._matchFieldListItems.Clear();
                updateatt._matchFieldListItems.Add(li);
                if (updateatt._fieldType == FieldType.Relationship)
                {
                    updateatt._attributeid = 0;
                    int result = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], updateatt, customEntities[1], _executingProduct);
                    Assert.IsTrue(result > 0);
                    updateatt._attributeid = result;
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            /// Navigate to custom Entities
            ///cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[1]._entityId);
            ///cCustomEntitiesNtoOneAttributesMethods.PressCancelCE();
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            ///click delete Custom Entity
            cCustomEntitiesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, customEntities[1].entityName);
       
            cCustomEntitiesNtoOneAttributesMethods.ClickConfirmDeletionOfCustomEntity();

            ///verify attribute not deleted
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpensesExpectedValues.UICtl00_pnlMasterPopupPane2InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe delete request was denied as a n:1 relationship references this GreenLight.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntitiesNtoOneAttributesMethods.ValidateMessageFromExpenses();
            cCustomEntitiesNtoOneAttributesMethods.PressClose();

            ///verify custom entity displayed in grid

        }
        #endregion

        #region Successfully verify has max length when entering and using copy/ paste
        [TestCategory("Greenlight Many To One"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityNToOneAttributesSuccessfullyVerifyMaximumFieldSizesOnNToOneModal_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntitiesMethods.ClickAttributesLink();
            
            cCustomEntitiesNtoOneAttributesMethods.ClickNewNtoOneAttributesLink();

            CustomEntityNtoOneAttributes nToOneUserControls = new CustomEntityNtoOneAttributes(cCustomEntitiesNtoOneAttributesMethods);

            Assert.AreEqual(4000, nToOneUserControls.DescriptionTxt.GetMaxLength());
            Assert.AreEqual(250, nToOneUserControls.DisplayNameTxt.MaxLength);
            Assert.AreEqual(4000, nToOneUserControls.ToolTipTxt.GetMaxLength());
            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();
            Assert.AreEqual(3, nToOneUserControls.SuggestionTxt.MaxLength);

            cCustomEntitiesNtoOneAttributesMethods.ClickonGeneralDetailsTab();

            Clipboard.Clear();
            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }

            cSharedMethods.PasteText(nToOneUserControls.DescriptionTxt);
            Assert.AreEqual(4000, nToOneUserControls.DescriptionTxt.GetMaxLength());

            Clipboard.Clear();
            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }

            cSharedMethods.PasteText(nToOneUserControls.ToolTipTxt);
            Assert.AreEqual(4000, nToOneUserControls.ToolTipTxt.GetMaxLength());

            Clipboard.Clear();
            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }

            cSharedMethods.PasteText(nToOneUserControls.DisplayNameTxt);
            Assert.AreEqual(250, nToOneUserControls.DisplayNameTxt.MaxLength);

            nToOneUserControls.EntityComboBx.SelectedItem = "Employees";
            cCustomEntitiesNtoOneAttributesMethods.ClickonFieldsTab();

            Clipboard.Clear();
            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }

            cSharedMethods.PasteText(nToOneUserControls.SuggestionTxt);
            Assert.AreEqual(3, nToOneUserControls.SuggestionTxt.MaxLength);
         
        }
        #endregion

        #region Additional test attributes

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            cSharedMethods = new SharedMethodsUIMap();
            foreach (CustomEntity customEntity in customEntities)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntity.entityName = "Custom Entity " + genericName.ToString();
                customEntity.pluralName = "Custom Entity " + genericName.ToString();
                customEntity.description = "Custom Entity " + genericName.ToString();
                customEntity.entityId = 0;
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                customEntity.entityId = result;
                Assert.IsTrue(result > 0);
            }
            //CacheUtilities.DeleteCachedTablesAndFields();
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Assert.IsNotNull(customEntities);
            foreach (CustomEntity customEntity in customEntities)
            {
                int result = CustomEntityDatabaseAdapter.DeleteCustomEntity(customEntity.entityId, _executingProduct);
                Assert.AreEqual(0, result);
            }
            //CacheUtilities.DeleteCachedTablesAndFields();
        }

        #endregion


        /// <summary>
        /// class for controls on the N to 1 attributes page
        /// </summary>
        internal class CustomEntityNtoOneAttributes
        {
            internal HtmlComboBox EntityComboBx { get; private set; }
            internal HtmlEdit DisplayNameTxt { get; private set; }
            internal HtmlCheckBox Mandatory { get; private set; }
            internal cHtmlTextAreaWrapper DescriptionTxt { get; private set; }
            internal cHtmlTextAreaWrapper ToolTipTxt { get; private set; }
            internal HtmlComboBox DisplayFieldComboBx { get; private set; }
            internal HtmlComboBox LookupFieldComboBx { get; private set; }
            internal HtmlEdit SuggestionTxt { get; private set; }
            internal HtmlList LookupFieldCollection { get; set; }
            internal HtmlInputButton SaveButton { get; set; }
            internal HtmlInputButton CancelButton { get; set; }
            internal HtmlImage AddImg { get; set; }
            internal HtmlImage DeleteImg { get; set; }

            protected CustomEntityNtoOneAttributesUIMap _cCustomEntityNtoOneAttributesMethods;
            protected ControlLocator<HtmlControl> _ControlLocator { get; private set; }
            protected FieldType _fieldType;

            internal CustomEntityNtoOneAttributes(CustomEntityNtoOneAttributesUIMap cCustomEntityNtoOneAttributesMethods, FieldType type = FieldType.NotSet)
            {
                _cCustomEntityNtoOneAttributesMethods = cCustomEntityNtoOneAttributesMethods;
                _fieldType = type;
                _ControlLocator = new ControlLocator<HtmlControl>();
                FindControls();
            }

            /// <summary>
            /// Locates controls that are declared within this class
            /// </summary>
            internal virtual void FindControls()
            {
                EntityComboBx = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_tabConRelFields_tabRelDefinition_cmbntoOnerelationshipentity", new HtmlComboBox(_cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument));
                DisplayNameTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConRelFields_tabRelDefinition_txtntoOnerelationshipname", new HtmlEdit(_cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument));
                Mandatory = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabConRelFields_tabRelDefinition_chkntoOnerelationshipmandatory", new HtmlCheckBox(_cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument));
                DescriptionTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl("ctl00_contentmain_tabConRelFields_tabRelDefinition_txtntoOnerelationshipdescription", new cHtmlTextAreaWrapper(cSharedMethods.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_tabConRelFields_tabRelDefinition_txtntoOnerelationshipdescription", _cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument));
                ToolTipTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl("ctl00_contentmain_tabConRelFields_tabRelDefinition_txtntoOnerelationshiptooltip", new cHtmlTextAreaWrapper(cSharedMethods.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_tabConRelFields_tabRelDefinition_txtntoOnerelationshiptooltip", _cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument));
                DisplayFieldComboBx = (HtmlComboBox)_ControlLocator.findControl("cmbmtodisplayfield", new HtmlComboBox(_cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument));
                LookupFieldComboBx = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_lstFieldItemList", new HtmlComboBox(_cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument));
                LookupFieldCollection = (HtmlList)_ControlLocator.findControl("cmbmtomatchfields", new HtmlList(_cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument));
                SuggestionTxt = (HtmlEdit)_ControlLocator.findControl("txtmtomaxrows", new HtmlEdit(_cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument));
                //SaveButton = (HtmlButton)_ControlLocator.findControl("ctl00_contentmain_btnSaveRelationship", new HtmlButton(_cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane));
                //CancelButton = (HtmlButton)_ControlLocator.findControl("ctl00_contentmain_btnCancelRelationship", new HtmlButton(_cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICtl00_contentmain_taPane));

                SaveButton = _cCustomEntityNtoOneAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UISaveButton;
                CancelButton = _cCustomEntityNtoOneAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICancelButton;

                AddImg = _cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument2.UITabRelFieldsPane.UINewMatchFieldImage;
                DeleteImg = _cCustomEntityNtoOneAttributesMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument2.UITabRelFieldsPane.UIDeleteMatchFieldImage;


            }

        }

        /// <summary>
        /// Reads custom entity data required by codedui tests from lithium
        ///</summary>
        private static List<CustomEntity> ReadCustomEntityDataFromLithium()
        {
            List<CustomEntity> cDatabaseCustomEntity;

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string SQLString = "SELECT TOP 2 entityid, entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, audienceViewType, enablePopupWindow, defaultPopupView, tableid, createdby FROM customEntities";


            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(SQLString))
            {
                #region Set Database Columns
                int entityNameOrdinal = reader.GetOrdinal("entity_name");
                int pluralNameOrdinal = reader.GetOrdinal("plural_name");
                int descriptionOrdinal = reader.GetOrdinal("description");
                int enableCurrenciesOrdinal = reader.GetOrdinal("enableCurrencies");
                int defaultCurrencyIDOrdinal = reader.GetOrdinal("defaultCurrencyID");
                int createdOnOrdinal = reader.GetOrdinal("createdon");
                int enableAttachmentsOrdinal = reader.GetOrdinal("enableAttachments");
                int allowDocumentMergeAccessOrdinal = reader.GetOrdinal("allowdocmergeaccess");
                int audienceViewTypeOrdinal = reader.GetOrdinal("audienceViewType");
                int createdByOrdinal = reader.GetOrdinal("createdby");
                int entityIDOrdinal = reader.GetOrdinal("entityid");
                #endregion

                cDatabaseCustomEntity = new List<CustomEntity>();

                while (reader.Read())
                {
                    CustomEntity customEntity = new CustomEntity();
                    #region Set Custom Entity variables

                    customEntity.entityName = reader.GetString(entityNameOrdinal);
                    customEntity.pluralName = reader.GetString(pluralNameOrdinal);
                    customEntity.description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal);
                    customEntity.enableCurrencies = reader.GetBoolean(enableCurrenciesOrdinal);
                    customEntity.defaultCurrencyId = reader.IsDBNull(defaultCurrencyIDOrdinal) ? null : reader.GetString(defaultCurrencyIDOrdinal);
                    customEntity.date = reader.GetDateTime(createdOnOrdinal);
                    customEntity.enableAttachments = reader.GetBoolean(enableAttachmentsOrdinal);
                    customEntity.allowDocumentMerge = reader.GetBoolean(allowDocumentMergeAccessOrdinal);
                    customEntity.AudienceViewType = (AudienceViewType)reader.GetInt16(audienceViewTypeOrdinal);
                    customEntity.userId = AutoTools.GetEmployeeIDByUsername(_executingProduct);
                    customEntity.entityId = reader.GetInt32(entityIDOrdinal);

                    customEntity.attribute = ReadAttributeDataFromLithium(customEntity.entityId);
                    customEntity.entityId = 0;
                    cDatabaseCustomEntity.Add(customEntity);
                    #endregion
                }
                reader.Close();

            }
            return cDatabaseCustomEntity;
        }


        private static List<CustomEntitiesUtilities.CustomEntityAttribute> ReadAttributeDataFromLithium(int customEntityID)
        {
            List<CustomEntitiesUtilities.CustomEntityAttribute> cDatabaseAttribute = new List<CustomEntitiesUtilities.CustomEntityAttribute>();

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT createdby, modifiedby, display_name, description, tooltip, mandatory, fieldtype, relatedtable, relationshipdisplayfield, createdon, maxlength, format, defaultvalue, precision, relationshiptype, related_entity, is_audit_identity, advicePanelText, is_unique, attributeid FROM customEntityAttributes WHERE entityid = @entityid ";
            db.sqlexecute.Parameters.AddWithValue("@entityid", customEntityID);

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
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
                int relationshipDisplayField = reader.GetOrdinal("relationshipdisplayfield");
                #endregion


                while (reader.Read())
                {
                    CustomEntitiesUtilities.CustomEntityNtoOneAttribute attribute = new CustomEntitiesUtilities.CustomEntityNtoOneAttribute();
                    #region Set values
                    FieldType type = (FieldType)reader.GetByte(fieldTypeOrdinal);
                    int attributeID = reader.GetInt32(attributeIDOrdinal);
                    attribute._createdBy = AutoTools.GetEmployeeIDByUsername(_executingProduct);
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
                    attribute._relationshipType = reader.IsDBNull(relationshipTypeOrdinal) ? (short?)null : reader.GetByte(relationshipTypeOrdinal);
                    attribute._relatedEntityId = reader.IsDBNull(relatedEntityOrdinal) ? (int?)null : reader.GetInt32(relatedEntityOrdinal);
                    attribute._isAuditIdenity = reader.GetBoolean(isAuditIdentityOrdinal);
                    attribute._commentText = reader.IsDBNull(advicePanelTextOrdinal) ? null : reader.GetString(advicePanelTextOrdinal);
                    attribute._isUnique = reader.GetBoolean(isUniqueOrdinal);
                    attribute._relatedTable = reader.GetGuid(relatedTableOrdinal);
                    attribute._relationshipdisplayfield = reader.GetGuid(relationshipDisplayField);
                    #endregion
                    cDatabaseAttribute.Add(attribute);
                }
                reader.Close();
            }

            db.sqlexecute.Parameters.Clear();

            return cDatabaseAttribute;
        }


        internal class CachePopulatorForAddTest : CachePopulator
        {
            internal CachePopulatorForAddTest(ProductType productType) : base(productType) { }

            public override string GetSQLStringForCustomEntity()
            {
                return "SELECT TOP 2 entityid, entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, audienceViewType, enablePopupWindow, defaultPopupView, tableid, createdby FROM customEntities";
            }

            public override void PopulateAttributes(ref CustomEntity entity)
            {
                //db.sqlexecute.Parameters.AddWithValue("@fieldtype", 9);
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
                    int boolAttributeOrdinal = reader.GetOrdinal("boolAttribute");
                    #endregion

                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityAttribute attribute = null;
                        FieldType type = (FieldType)reader.GetByte(fieldTypeOrdinal);
                        int attributeID = reader.GetInt32(attributeIDOrdinal);

                        if (type == FieldType.List)
                        {
                            CustomEntitiesUtilities.cCustomEntityListAttribute listAttribute = new CustomEntitiesUtilities.cCustomEntityListAttribute();
                            CachePopulator.PopulateListItems(ref listAttribute, attributeID);
                            attribute = listAttribute;
                        }
                        else
                        {
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
                            attribute = newNToOneAttribute;
                        }
                        #region Set values

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
                        attribute.EnableImageLibrary = reader.GetBoolean(boolAttributeOrdinal);

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
        }

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


        private string GetTableNamebyTableID(Guid relatedtable)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));

            string strSQL2 = "select description from tables where tableid = @relatedTable";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@relatedtable", relatedtable);
            string entity = dbex_CodedUI.getStringValue(strSQL2);

            return entity;
        }

        private string GetDisplayFieldNamebyFieldID(Guid displayname)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));

            string strSQL2 = "select description from fields where fieldid = @displayName";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@displayName", displayname);
            string displayfield = dbex_CodedUI.getStringValue(strSQL2);

            return displayfield;
        }
    }
} 
