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
using System.Configuration;
using Auto_Tests.UIMaps.CustomEntitiesUIMapClasses;
using Auto_Tests.UIMaps.CustomEntityAttributesUIMapClasses;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using System.Threading;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;


namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Attributes
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class CustomEntityListAttributesUITests
    {
        private static SharedMethodsUIMap cSharedMethods;

        private CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();
        private CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();
        /// <summary>
        /// Current product to execute on
        /// Framework / Expenses
        /// </summary>
        private static ProductType _executingProduct;
        private List<CustomEntity> customEntities;


        /// <summary>
        /// Sets up test suite by starting E.logging in
        /// </summary>
        /// <param name="ctx"></param>
        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            cSharedMethods = new SharedMethodsUIMap();
            _executingProduct = cGlobalVariables.GetProductFromAppConfig();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            cSharedMethods.Logon(_executingProduct, LogonType.administrator);
        }

        /// <summary>
        /// Clean up test suite
        /// Closes browser window to deal with modal errors
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            cSharedMethods.CloseBrowserWindow();
        }

        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributesSuccessfullyAddAttribute_UITest()
        {
            CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();
            CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();

            //Insert new C.E into database
            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            List<CustomEntity> customEntities = CustomEntityDataFromLithium.PopulateCache();

            foreach (CustomEntity customEntity in customEntities)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntity.entityName = "Custom Entity " + genericName.ToString();
                customEntity.pluralName = "Custom Entity " + genericName.ToString();
                customEntity.description = "Custom Entity " + genericName.ToString();
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                customEntity.entityId = result;
                Assert.IsTrue(result > 0);
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            //Edit entity
            cCustomEntitiesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, customEntities[0].entityName);

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Add list attribute
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            //Update Type control to list


            //Add List items
            CustomEntityListAttribute attributefields = new CustomEntityListAttribute(cCustomEntitiesAttributesMethods);
            attributefields.DisplayNameTxt.Text = customEntities[0].attribute[0].DisplayName;
            attributefields.DescriptionTxt.Text = customEntities[0].attribute[0]._description;
            attributefields.DisplayWidth.SelectedItem = EnumHelper.GetEnumDescription((Format)customEntities[0].attribute[0]._format);
            //verify list items

            //Pre cast the type and convert to list attribute - safer than using 'as'!
            CustomEntitiesUtilities.cCustomEntityListAttribute listAtt = (CustomEntitiesUtilities.cCustomEntityListAttribute)customEntities[0].attribute[0];
            foreach (CustomEntitiesUtilities.EntityListItem listItem in listAtt._listItems)
            {
                cCustomEntitiesAttributesMethods.ClickAddListItemIcon();
                ControlLocator<HtmlEdit> locator = new ControlLocator<HtmlEdit>();
                HtmlEdit itemName = locator.findControl("ctl00_contentmain_txtlistitem", new HtmlEdit(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument7.UIAddEditListItemItemsPane.UIItemPane));
                itemName.Text = listItem._textItem;
                cCustomEntitiesAttributesMethods.SaveListItem();
            }

            //Press save!
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Now hit edit
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(customEntities[0].attribute[0].DisplayName);

            //Ensure that the items have been entered correctly

            attributefields.FindControls();
            Assert.AreEqual(customEntities[0].attribute[0].DisplayName, attributefields.DisplayNameTxt.Text);
            Assert.AreEqual(customEntities[0].attribute[0]._description, attributefields.DescriptionTxt.Text);

            int index = 0;
            foreach (HtmlListItem item in attributefields.ListItems.Items)
            {
                Assert.AreEqual(listAtt._listItems[index++]._textItem, item.InnerText);
            }

            foreach (CustomEntity customEntity in customEntities)
            {
                CustomEntityDatabaseAdapter.DeleteCustomEntity(customEntity.entityId, _executingProduct);
            }

        }

        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributeSuccessfullyDeleteListItems_UITest()
        {
            CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();
            CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();

            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();

            foreach (CustomEntity customEntity in customEntities)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntity.entityName = "Custom Entity " + genericName.ToString();
                customEntity.pluralName = "Custom Entity " + genericName.ToString();
                customEntity.description = "Custom Entity " + genericName.ToString();
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                customEntity.entityId = result;
                foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in customEntity.attribute)
                {
                    CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, attribute, _executingProduct);
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");


            //Iterate through all in list and delete
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlList listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            CustomEntitiesUtilities.cCustomEntityListAttribute listAtt = (CustomEntitiesUtilities.cCustomEntityListAttribute)customEntities[0].attribute[0];


            foreach (HtmlListItem item in listItemsControl.Items)
            {
                listItemsControl.SelectedItems = new string[] { item.InnerText };
                cCustomEntitiesAttributesMethods.ClickDeleteInListAttribute();
            }

            //Assert nothing is in the list
            listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            Assert.AreEqual(0, listItemsControl.ItemCount);

            //Click cancel
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Modal should be displayed
            cCustomEntitiesAttributesMethods.AssertCorrectValidationMessageAppearsWhenAddingEmptyItemToListExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease add a List item.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.AssertCorrectValidationMessageAppearsWhenAddingEmptyItemToList();

        }

        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributeUnSuccessfullyAddDuplicationItemToList_UITest()
        {
            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();

            foreach (CustomEntity customEntity in customEntities)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntity.entityName = "Custom Entity " + genericName.ToString();
                customEntity.pluralName = "Custom Entity " + genericName.ToString();
                customEntity.description = "Custom Entity " + genericName.ToString();
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                customEntity.entityId = result;
                foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in customEntity.attribute)
                {
                    CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, attribute, _executingProduct);
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            //cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            //cCustomEntitiesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, "Standard List");

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");

            cCustomEntitiesAttributesMethods.ClickAddListItemIcon();

            ControlLocator<HtmlControl> listItemWindow = new ControlLocator<HtmlControl>();

            HtmlEdit listItemText = (HtmlEdit)listItemWindow.findControl("ctl00_contentmain_txtlistitem", new HtmlEdit(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument15));
            cCustomEntitiesAttributesMethods.AssertNewListItemLabelIsMandatory();

            //Enter an existing list item and click save
            CustomEntitiesUtilities.cCustomEntityListAttribute listAttribute = (CustomEntitiesUtilities.cCustomEntityListAttribute)customEntities[0].attribute[0];
            listItemText.Text = listAttribute._listItems[0]._textItem;


            cCustomEntitiesAttributesMethods.SaveListItem();

            //Ensure pressing ESC. doesn't close the validation modal screen
            cCustomEntitiesAttributesMethods.AssertListItemModalValueAlreadyExistsExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nThis value is already in the list.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.AssertListItemModalValueAlreadyExists();

            cCustomEntitiesAttributesMethods.ClickCloseOnValueAlreadyExistsMessageModal();
        }

        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributesUnsuccessfullyAddBlankListItem_UITest()
        {
            Playback.PlaybackSettings.DelayBetweenActions = 0;

            CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();
            CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();

            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();

            foreach (CustomEntity customEntity in customEntities)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntity.entityName = "Custom Entity " + genericName.ToString();
                customEntity.pluralName = "Custom Entity " + genericName.ToString();
                customEntity.description = "Custom Entity " + genericName.ToString();
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                customEntity.entityId = result;
                foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in customEntity.attribute)
                {
                    CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, attribute, _executingProduct);
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");

            cCustomEntitiesAttributesMethods.ClickAddListItemIcon();

            cCustomEntitiesAttributesMethods.SaveListItem();

            //Assert message
            cCustomEntitiesAttributesMethods.AssertCorrectValidationMessageAppearsWhenAddingEmptyItemToListExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease add a List item.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.AssertCorrectValidationMessageAppearsWhenAddingEmptyItemToList();

            //Close modal
            cCustomEntitiesAttributesMethods.ClickCloseOnBlankListItemValidationModal();

            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;

        }

        /// <summary>
        /// 
        /// </summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributesSuccessfullyEditListItem_UITest()
        {

            CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();
            CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();

            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();

            foreach (CustomEntity customEntity in customEntities)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntity.entityName = "Custom Entity " + genericName.ToString();
                customEntity.pluralName = "Custom Entity " + genericName.ToString();
                customEntity.description = "Custom Entity " + genericName.ToString();
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                customEntity.entityId = result;
                foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in customEntity.attribute)
                {
                    CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, attribute, _executingProduct);
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");

            //Verify the list items are correct
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlList listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            UITestControlCollection collectionOflistItems = listItemsControl.Items;


            //expectedListobject
            CustomEntitiesUtilities.cCustomEntityListAttribute attributes = (CustomEntitiesUtilities.cCustomEntityListAttribute)customEntities[0].attribute[0];

            //Edit all items within the list
            foreach (CustomEntitiesUtilities.EntityListItem listItem in attributes._listItems)
            {
                listItemsControl.SelectedItems = new string[] { listItem._textItem };
                cCustomEntitiesAttributesMethods.ClickEditLinkforListItem();
                HtmlEdit listItemEditText = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtlistitem", new HtmlEdit(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument7.UIAddEditListItemItemsPane.UIItemPane));
                listItemEditText.Text = listItem._textItem + "_Edited";
                cCustomEntitiesAttributesMethods.ClickSaveBtnForEditingListItem();

            }
        }

        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributesSuccessfullyCancelDeleteListItem_UITest()
        {
            CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();
            CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();

            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();

            foreach (CustomEntity customEntity in customEntities)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntity.entityName = "Custom Entity " + genericName.ToString();
                customEntity.pluralName = "Custom Entity " + genericName.ToString();
                customEntity.description = "Custom Entity " + genericName.ToString();
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                customEntity.entityId = result;
                foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in customEntity.attribute)
                {
                    CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, attribute, _executingProduct);
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");


            //Iterate through all in list and delete
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlList listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            CustomEntitiesUtilities.cCustomEntityListAttribute listAtt = (CustomEntitiesUtilities.cCustomEntityListAttribute)customEntities[0].attribute[0];


            foreach (HtmlListItem item in listItemsControl.Items)
            {
                listItemsControl.SelectedItems = new string[] { item.InnerText };
                cCustomEntitiesAttributesMethods.ClickDeleteInListAttribute();
            }

            //Assert nothing is in the list
            listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            Assert.AreEqual(0, listItemsControl.ItemCount);

            //Click cancel
            cCustomEntitiesAttributesMethods.PressCancel();


            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");

            listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));

            //Assert items havent changed
            Assert.AreEqual(6, listItemsControl.ItemCount);
            int index = 0;
            foreach (HtmlListItem item in listItemsControl.Items)
            {
                Assert.AreEqual(listAtt._listItems[index++]._textItem, item.InnerText);
            }
        }

        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributesSuccessfullyCancelEditListItem_UITest()
        {
            CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();
            CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();

            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();

            foreach (CustomEntity customEntity in customEntities)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntity.entityName = "Custom Entity " + genericName.ToString();
                customEntity.pluralName = "Custom Entity " + genericName.ToString();
                customEntity.description = "Custom Entity " + genericName.ToString();
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                customEntity.entityId = result;
                foreach (CustomEntitiesUtilities.CustomEntityAttribute attribute in customEntity.attribute)
                {
                    CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, attribute, _executingProduct);
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");

            //Verify the list items are correct
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlList listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            UITestControlCollection collectionOflistItems = listItemsControl.Items;

            //expectedListobject
            CustomEntitiesUtilities.cCustomEntityListAttribute attributes = (CustomEntitiesUtilities.cCustomEntityListAttribute)customEntities[0].attribute[0];
            //Edit all items within the list
            foreach (CustomEntitiesUtilities.EntityListItem listItem in attributes._listItems)
            {
                listItemsControl.SelectedItems = new string[] { listItem._textItem };
                cCustomEntitiesAttributesMethods.ClickEditLinkforListItem();
                HtmlEdit listItemEditText = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtlistitem", new HtmlEdit(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument7.UIAddEditListItemItemsPane.UIItemPane));
                listItemEditText.Text = listItem._textItem + "_Edited";
                cCustomEntitiesAttributesMethods.ClickSaveBtnForEditingListItem();
            }

            //Now click cancel on the actual attribute window
            cCustomEntitiesAttributesMethods.PressCancel();
            //Edit and verify
            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");

            listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            CustomEntitiesUtilities.cCustomEntityListAttribute listAtt = (CustomEntitiesUtilities.cCustomEntityListAttribute)customEntities[0].attribute[0];
            int index = 0;
            foreach (HtmlListItem item in listItemsControl.Items)
            {
                Assert.AreEqual(listAtt._listItems[index++]._textItem, item.InnerText);
            }


        }

        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributesSuccessfullyVerifyMandatoryFields_UITest()
        {
            CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();
            CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();

            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();

            foreach (CustomEntity customEntity in customEntities)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntity.entityName = "Custom Entity " + genericName.ToString();
                customEntity.pluralName = "Custom Entity " + genericName.ToString();
                customEntity.description = "Custom Entity " + genericName.ToString();
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                Assert.IsTrue(result > 0);
                customEntity.entityId = result;
            }
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntitiesMethods.ClickAttributesLink();

            cCustomEntitiesMethods.ClickNewAttributeLink();

            //Verify no astricks appear

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
"e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            CustomEntityListAttribute toolLocator = new CustomEntityListAttribute(cCustomEntitiesAttributesMethods);

            toolLocator.DisplayNameTxt.Text = customEntities[0].entityName;
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk)); ;
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow6.UICustomEntityCustomEnDocument.ListItemsAsterisk));

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow6.UICustomEntityCustomEnDocument.MissingListItemsAndDisplayWidthErrorModal.InnerText;

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            //Add list item
            //Pre cast the type and convert to list attribute - safer than using 'as'!
            CustomEntitiesUtilities.cCustomEntityListAttribute listAtt = (CustomEntitiesUtilities.cCustomEntityListAttribute)customEntities[0].attribute[0];
            foreach (CustomEntitiesUtilities.EntityListItem listItem in listAtt._listItems)
            {
                cCustomEntitiesAttributesMethods.ClickAddListItemIcon();
                ControlLocator<HtmlEdit> locator = new ControlLocator<HtmlEdit>();
                HtmlEdit itemName = locator.findControl("ctl00_contentmain_txtlistitem", new HtmlEdit(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument7.UIAddEditListItemItemsPane.UIItemPane));
                itemName.Text = listItem._textItem;
                cCustomEntitiesAttributesMethods.SaveListItem();
            }

            cCustomEntitiesAttributesMethods.PressSaveAttribute();
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Display width", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk)); ;

            toolLocator.DisplayWidth.SelectedItem = "Standard";
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

        }

        /// <summary>
        /// 39838, 39839, 39840 - Successfully verify Keyboard shortcuts for opening New List item modal, Edit List item modal and deleting list item
        /// </summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributesSuccessfullyVerifyKeyboardShortcutsForListItems_UITest()
        {
            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();

            Guid genericName = new Guid();
            genericName = Guid.NewGuid();
            customEntities[0].entityName = "Custom Entity " + genericName.ToString();
            customEntities[0].pluralName = "Custom Entity " + genericName.ToString();
            customEntities[0].description = "Custom Entity " + genericName.ToString();
            int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntities[0], _executingProduct);
            customEntities[0].entityId = result;

            CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntities[0], customEntities[0].attribute[0], _executingProduct);
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");

            //Press Ctrl+1 to open new list item modal
            Thread.Sleep(1000);
            Keyboard.SendKeys("1", ModifierKeys.Control);
            //Verify modal is displayed 
            cCustomEntitiesAttributesMethods.ValidateListItemModal();

            //Press Escape to cancel out of the modal
            Keyboard.SendKeys("{Escape}");

            Thread.Sleep(1000);
            ////Select First list item
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlList listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            CustomEntitiesUtilities.cCustomEntityListAttribute attributes = (CustomEntitiesUtilities.cCustomEntityListAttribute)customEntities[0].attribute[0];
            CustomEntitiesUtilities.EntityListItem listItem = new CustomEntitiesUtilities.EntityListItem();
            listItem = attributes._listItems[0];
            listItemsControl.SelectedItems = new string[] { listItem._textItem };
            UITestControl selectedListItem = listItemsControl.Items[0];

            //Press Ctrl+3 to open the edit list item modal
            Keyboard.SendKeys("3", ModifierKeys.Control);

            //Validate modal is displayed
            cCustomEntitiesAttributesMethods.ValidateListItemModalExpectedValues.UINewListItemPaneInnerText = "List item: " + listItem._textItem;

            //Press Escape to cancel out of the modal
            Keyboard.SendKeys("{Escape}");

            Thread.Sleep(1000);
            //Press Ctrl+2 to delete a list item
            Keyboard.SendKeys("2", ModifierKeys.Control);

            //Assert there is 1 less item in the list 
            listItemsControl = (HtmlList)cControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlList(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            Assert.IsFalse(listItemsControl.Items.Contains(selectedListItem));
        }

        ///<summary>
        /// Page validation of the list items modal and tabbing order
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityListAttributesSuccessfullyVerifyTabbingOrderOnListAttribute_UITest()
        {
            CachePopulatorForAddTest CustomEntityDataFromLithium = new CachePopulatorForAddTest(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();
            Guid genericName = new Guid();
            genericName = Guid.NewGuid();
            customEntities[0].entityName = "Custom Entity " + genericName.ToString();
            customEntities[0].pluralName = "Custom Entity " + genericName.ToString();
            customEntities[0].description = "Custom Entity " + genericName.ToString();
            int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntities[0], _executingProduct);
            customEntities[0].entityId = result;

            CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntities[0], customEntities[0].attribute[0], _executingProduct);
            //CacheUtilities.DeleteCachedTablesAndFields();

            //Navigate to page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityListAttribute attributefields = new CustomEntityListAttribute(cCustomEntitiesAttributesMethods);

            attributefields.TypeComboBx.SelectedItem = "List";

            HtmlInputButton saveButton = cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UISaveButton;
            HtmlInputButton cancelButton = cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICancelButton;
            #region Validate tabbing order in new attribute of type list
            attributefields.DisplayNameTxt.SetFocus();
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.IsMandatory.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            //ListItems doesnt have focus when empty!
            //Assert.IsTrue(attributefields.ListItems.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIDivListOptionsPane.UINewListItemImage.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIDivListOptionsPane.UIEditListItemImage.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIDivListOptionsPane.UIDeleteListItemImage.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.DisplayWidth.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);

            cCustomEntitiesAttributesMethods.ClickAddListItemIcon();
            cCustomEntitiesAttributesMethods.ValidateListItemModalHeader();
            cCustomEntitiesAttributesMethods.ValidateListItemFieldLabel();
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIListitemEdit.HasFocus);
            Keyboard.SendKeys("{Tab}");

            // archive list item label gains focus
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIListItemSaveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIListItemCancelButton.HasFocus);

            Keyboard.SendKeys("{Escape}");
            Keyboard.SendKeys("{Escape}");
            #endregion

            #region Validate tabbing order in editing attribute of type list
            cCustomEntitiesAttributesMethods.ClickEditFieldLink("Standard List");

            Assert.IsTrue(attributefields.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.IsMandatory.HasFocus);
            Assert.IsFalse(attributefields.TypeComboBx.Enabled);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");

            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIDivListOptionsPane.UINewListItemImage.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIDivListOptionsPane.UIEditListItemImage.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIDivListOptionsPane.UIDeleteListItemImage.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(attributefields.DisplayWidth.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);

            Keyboard.SendKeys("{Escape}");
            #endregion
        }

        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
            cSharedMethods = new SharedMethodsUIMap();
            cCustomEntitiesMethods = new CustomEntitiesUIMap();
            cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();
        }


        ////Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (customEntities != null)
            {
                foreach (CustomEntity entity in customEntities)
                {
                    CustomEntityDatabaseAdapter.DeleteCustomEntity(entity.entityId, _executingProduct);
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();
        }

        #endregion

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
    }

    internal class CachePopulatorForAddTest : CachePopulator
    {

        internal CachePopulatorForAddTest(ProductType executingProduct) : base(executingProduct) { }

        public override string GetSQLStringForCustomEntity()
        {
            return "SELECT TOP 1 entityid, entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, audienceViewType, enablePopupWindow, defaultPopupView, tableid, createdby FROM customEntities";
        }

        public override void PopulateAttributes(ref CustomEntity entity)
        {
            string strSQL = "SELECT createdby, modifiedby, display_name, description, tooltip, mandatory, fieldtype, createdon, maxlength, format, defaultvalue, precision, relationshiptype, related_entity, is_audit_identity, advicePanelText, is_unique, attributeid FROM customEntityAttributes WHERE fieldtype = @fieldtype ";
            db.sqlexecute.Parameters.AddWithValue("@fieldtype", 4);
            entity.attribute = new List<CustomEntitiesUtilities.CustomEntityAttribute>();
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
                #endregion

                while (reader.Read())
                {
                    CustomEntitiesUtilities.cCustomEntityListAttribute attribute = new CustomEntitiesUtilities.cCustomEntityListAttribute();
                    #region Set values

                    FieldType type = (FieldType)reader.GetByte(fieldTypeOrdinal);
                    int attributeID = reader.GetInt32(attributeIDOrdinal);

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
                    if (attribute._fieldType == FieldType.List)
                    {
                        //GetListItems
                        CachePopulator.PopulateListItems(ref attribute, attributeID);
                    }
                    entity.attribute.Add(attribute);
                    #endregion
                }
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();
        }

        //    private void PopulateListItems(ref CustomEntitiesUtilities.cCustomEntityListAttribute attribute, int customEntityId)
        //    {
        //        cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

        //        string strSQL = "SELECT [attributeid] ,[item], [order] FROM customEntityAttributeListItems WHERE attributeid = @attributeid";
        //        db.sqlexecute.Parameters.AddWithValue("@attributeid", customEntityId);

        //        List<CustomEntitiesUtilities.EntityListItem> customEntityListItems = new List<CustomEntitiesUtilities.EntityListItem>();
        //        using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
        //        {
        //            int attributeIdOrdinal = reader.GetOrdinal("attributeid");
        //            int itemOrdinal = reader.GetOrdinal("item");
        //            int orderOrdinal = reader.GetOrdinal("order");

        //            while (reader.Read())
        //            {
        //                int attributeId = reader.GetInt32(attributeIdOrdinal);
        //                string text = reader.IsDBNull(itemOrdinal) ? null : reader.GetString(itemOrdinal);
        //                int order = reader.GetInt32(orderOrdinal);
        //                customEntityListItems.Add(new CustomEntitiesUtilities.EntityListItem(attributeId, text, order));
        //            }
        //        }
        //        attribute._listItems = customEntityListItems;
        //    }
    }

    /// <summary>
    /// Locator class for fields on ListAttributes aspx page
    /// </summary>
    internal class CustomEntityListAttribute : CustomEntityBaseType
    {
        internal HtmlComboBox ListItems { get; private set; }
        internal HtmlComboBox DisplayWidth { get; private set; }


        internal CustomEntityListAttribute(CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods)
            : base(cCustomEntitiesAttributesMethods, FieldType.List)
        { }

        internal override void FindControls()
        {
            base.FindControls();
            if (TypeComboBx.Enabled)
            {
                TypeComboBx.SelectedItem = EnumHelper.GetEnumDescription(_fieldType);
                ListItems = (HtmlComboBox)_ControlLocator.findControl("lbllstitems", new HtmlComboBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            }
            else
            {
                ListItems = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_lstitems", new HtmlComboBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
            }
            DisplayWidth = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_cmbDisplayWidth", new HtmlComboBox(_cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument1.UICtl00_contentmain_pnPane));
        }
    }
}
