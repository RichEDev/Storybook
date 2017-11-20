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
using Auto_Tests.UIMaps.CustomEntityAttributesUIMapClasses;
using Auto_Tests.UIMaps.CustomEntityOneToNAttributesUIMapClasses;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Auto_Tests.UIMaps.CustomEntitiesUIMapClasses;
using Auto_Tests.UIMaps.CustomEntityViewsUIMapClasses;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Forms;
using System.Text;

namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_One_to_N_Attributes
{
    /// <summary>
    /// Summary description for CustomEntityOnetoNAttributesUITests
    /// </summary>
    [CodedUITest]
    public class CustomEntityOnetoNAttributesUITests
    {
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();
        private static List<CustomEntity> customEntities;
        private static ProductType _executingProduct;

        private CustomEntityAttributesUIMap cCustomEntityAttributesMethods;
        private CustomEntityOneToNAttributesUIMap cCustomEntityOneToNMethods;
        private CustomEntitiesUIMap cCustomEntitiesMethods;
        private CustomEntityViewsUIMap cCustomEntityViewsMethods;
        private string emptylist = "[None]";

        public CustomEntityOnetoNAttributesUITests()
        {
            cCustomEntityAttributesMethods = new CustomEntityAttributesUIMap();
            cCustomEntityOneToNMethods = new CustomEntityOneToNAttributesUIMap();
            cCustomEntitiesMethods = new CustomEntitiesUIMap();
            cCustomEntityViewsMethods = new CustomEntityViewsUIMap();
            cSharedMethods = new SharedMethodsUIMap();
        }

        #region 37904 - Successfully verify custom entity new 1:n relationship modal meets standards
        /// <summary>
        /// 37904 - Successfully verify custom entity new 1:n relationship modal meets standards
        /// 37914 - Successfully verify Entity field displays only custom entities with a view on 1:n relationship modal
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyVerifyOneToNModalMeetsStandards_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            //Validate Display name has focus when entering the modal
            Assert.IsTrue(onetonControls.DisplayNameTxt.HasFocus);

            //Validate Labels
            cCustomEntityOneToNMethods.ValidateOneToNModalTitleExpectedValues.UIDivOneToManyRelationPaneInnerText = "New 1:n Relationship Attribute";
            cCustomEntityOneToNMethods.ValidateOneToNModalTitle();
            cCustomEntityOneToNMethods.ValidateDisplayNameLabel();
            cCustomEntityOneToNMethods.ValidateGreenLightLabel();
            cCustomEntityOneToNMethods.ValidateDescriptionLabel();
            cCustomEntityOneToNMethods.ValidateViewLabel();

            //Validate Correct custom entities display in the dropdown list 
            //Future work: Should be updated to work for every database and with different entries
            List<string> EntityList = CustomEntitiesUtilities.GetEntitiesFromDatabase(_executingProduct);
            StringBuilder entityDropdownCollection = new StringBuilder();
            foreach (string Entity in EntityList)
            {
                if (Entity != customEntities[0].pluralName)
                {
                    entityDropdownCollection.Append(Entity);
                }
            }
            Assert.AreEqual("[None]" + entityDropdownCollection, onetonControls.entitylist.InnerText);

            //Validate buttons display on the modal
            HtmlInputButton saveButton = cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UISaveButton;
            string saveButtonExpectedText = "save";
            Assert.AreEqual(saveButtonExpectedText, saveButton.DisplayText);

            HtmlInputButton cancelButton = cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton;
            string cancelButtonExpectedText = "cancel";
            Assert.AreEqual(cancelButtonExpectedText, cancelButton.DisplayText);

            #region Validate tabbing order
            onetonControls.DisplayNameTxt.SetFocus();
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(onetonControls.entitylist.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(onetonControls.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(onetonControls.viewlist.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion
           
            //Close 1:n relationship modal
            Keyboard.SendKeys("{Enter}");
        }
        #endregion

        #region 36575 - Successfully add n to one attribute in customEntity, 37916 - Successfully verify new 1:n relationship modal displays no information
        /// <summary>
        /// 36575 - Successfully add n to one attribute in customEntity
        /// 37916 - Successfully verify new 1:n relationship modal displays no information
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyCreateOneToNAttributeInCustomEnity_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            Assert.AreEqual(onetonControls.DisplayNameTxt.Text, "");
            Assert.AreEqual(onetonControls.DescriptionTxt.Text, "");
            Assert.AreEqual(onetonControls.entitylist.SelectedItem, emptylist);
            Assert.AreEqual(onetonControls.entitylist.Enabled, true);
            Assert.AreEqual(onetonControls.viewlist.SelectedItem, emptylist);
            Assert.AreEqual(onetonControls.viewlist.Enabled, true);

            onetonControls.DisplayNameTxt.Text = customEntities[0].attribute[37].DisplayName;
            onetonControls.entitylist.SelectedItem = customEntities[1].pluralName;
            onetonControls.DescriptionTxt.Text = customEntities[0].attribute[37]._description;
            onetonControls.viewlist.SelectedItem = customEntities[1].view[0]._viewName;

            cCustomEntityOneToNMethods.PressSaveOneToNModal();

            cCustomEntityAttributesMethods.ValidateAttributesGrid(customEntities[0].attribute[37].DisplayName, customEntities[0].attribute[37]._description, EnumHelper.GetEnumDescription((FieldType)customEntities[0].attribute[37]._fieldType), customEntities[0].attribute[37]._isAuditIdenity.ToString());

            cCustomEntityAttributesMethods.ClickEditFieldLink(customEntities[0].attribute[37].DisplayName);

            onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            CustomEntitiesUtilities.CustomEntityOnetoNAttribute onetontoAdd = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[37];

            Assert.AreEqual(onetonControls.DisplayNameTxt.Text, onetontoAdd.DisplayName);
            Assert.AreEqual(onetonControls.entitylist.Enabled, false);
            Assert.AreEqual(onetonControls.entitylist.SelectedItem, customEntities[1].pluralName);
            Assert.AreEqual(onetonControls.DescriptionTxt.Text, onetontoAdd._description);
            Assert.AreEqual(onetonControls.viewlist.SelectedItem, customEntities[1].view[0]._viewName);
            Assert.AreEqual(onetonControls.viewlist.Enabled, true);

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);

            //Click New mobile device modal and validate controls are empty 
            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            Assert.AreEqual(onetonControls.DisplayNameTxt.Text, "");
            Assert.AreEqual(onetonControls.DescriptionTxt.Text, "");
            Assert.AreEqual(onetonControls.entitylist.SelectedItem, emptylist);
            Assert.AreEqual(onetonControls.viewlist.SelectedItem, emptylist);

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);
        }
        #endregion

        #region 37901 - Successfully cancel adding custom entity 1:n Relationship
        /// <summary>
        /// 37901 - Successfully cancel adding custom entity 1:n Relationship
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyCancelAddingOneToNAttributeInCustomEnity_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            //Initialise search space
            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            //Populate fields
            onetonControls.DisplayNameTxt.Text = customEntities[0].attribute[37].DisplayName;
            onetonControls.entitylist.SelectedItem = customEntities[1].pluralName;
            onetonControls.DescriptionTxt.Text = customEntities[0].attribute[37]._description;
            onetonControls.viewlist.SelectedItem = customEntities[1].view[0]._viewName;

            //Press Cancel
            cCustomEntityOneToNMethods.PressCancelSaveOneToNModal();

            //Validate 1:n does not display on the grid
            cCustomEntityAttributesMethods.ValidateAttributesGrid(customEntities[0].attribute[37].DisplayName, customEntities[0].attribute[37]._description, EnumHelper.GetEnumDescription((FieldType)customEntities[0].attribute[37]._fieldType), customEntities[0].attribute[37]._isAuditIdenity.ToString(), true, true, false);
        }
        #endregion

        #region 38334 - Unsuccessfully edit n to one attribute where duplicated details are used
        /// <summary>
        /// 38334 - Unsuccessfully edit n to one attribute where duplicated details are used
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesUnsuccessfullyEditOneToNAttributeWhereDuplicateDetailsAreUsedInCustomEnity_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityAttributesMethods.ClickEditFieldLink(customEntities[0].attribute[37].DisplayName);

            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            onetonControls.DescriptionTxt.Text = customEntities[0].attribute[38]._description;

            onetonControls.DisplayNameTxt.Text = customEntities[0].attribute[38].DisplayName;

            cCustomEntityOneToNMethods.PressSaveOneToNModal();

            cCustomEntityOneToNMethods.ValidateOneToNMessageModalExpectedValues.UIMessageFromExpensesModalInnerText = String.Format("Message from {0}\r\n\r\n\r\nAn attribute or relationship with this Display name already exists.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntityOneToNMethods.ValidateOneToNMessageModal();

            cCustomEntityOneToNMethods.PressCloseOneToNValidationModal();
        }

        #endregion

        #region 38333 - Unsuccessfully edit n to one attribute where mandatory fields are missing
        /// <summary>
        ///  38333 - Unsuccessfully edit n to one attribute where mandatory fields are missing
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesUnsuccessfullyEditOneToNAttributeWhereMandatoryFieldsareMissingInCustomEnity_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityAttributesMethods.ClickEditFieldLink(customEntities[0].attribute[37].DisplayName);

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));
             
            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);
            #region Leave Display Name empty
            onetonControls.DisplayNameTxt.Text = string.Empty;

            cCustomEntityOneToNMethods.PressSaveOneToNModal();

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            cCustomEntityOneToNMethods.ValidateOneToNMessageModalExpectedValues.UIMessageFromExpensesModalInnerText = String.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.", EnumHelper.GetEnumDescription(_executingProduct));
            cCustomEntityOneToNMethods.ValidateOneToNMessageModal();

            cCustomEntityOneToNMethods.PressCloseOneToNValidationModal();
            #endregion
            #region Leave View field empty
            onetonControls.viewlist.SelectedItem = "[None]";

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UISaveButton);

            cCustomEntityOneToNMethods.ValidateOneToNMessageModalExpectedValues.UIMessageFromExpensesModalInnerText = String.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPlease select a View to be used.", EnumHelper.GetEnumDescription(_executingProduct));
            cCustomEntityOneToNMethods.ValidateOneToNMessageModal();

            cCustomEntityOneToNMethods.PressCloseOneToNValidationModal();
            #endregion

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);
            #region Validate asterisks do not display when editing the same attribute
            cCustomEntityAttributesMethods.ClickEditFieldLink(customEntities[0].attribute[37].DisplayName);

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);
            #endregion
        }
        #endregion

        #region 37902 - Unsuccessfully add custom entity 1:n relationship where mandatory fields are missing
        /// <summary>
        ///  37902 - Unsuccessfully add custom entity 1:n relationship where mandatory fields are missing
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesUnsuccessfullyCreateOneToNAttributeWhereMandatoryFieldsareMissingInCustomEnity_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            //Validate asterisks do not display
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            #region Populate Display Name field then empty it and assert the asterisk is displayed
            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);
            onetonControls.DisplayNameTxt.Text = "Mandatory fields test";
            onetonControls.entitylist.SetFocus();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            onetonControls.DisplayNameTxt.Text = string.Empty;
            onetonControls.entitylist.SetFocus();

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));
            #endregion
            #region Populate Related to field then empty it and assert the asterisk is displayed
            onetonControls.entitylist.SelectedItem = customEntities[1].pluralName;

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            onetonControls.entitylist.SelectedItem = "[None]";

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));
            #endregion
            #region Populate View field then empty it and assert the asterisk is displayed
            onetonControls.entitylist.SelectedItem = customEntities[1].pluralName;
            onetonControls.viewlist.SelectedItem = customEntities[1].view[0]._viewName;

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            onetonControls.viewlist.SelectedItem = "[None]";

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            onetonControls.entitylist.SelectedItem = "[None]";
            #endregion

            //Press Save
            cCustomEntityOneToNMethods.PressSaveOneToNModal();

            //Validate modal is displayed
            cCustomEntityOneToNMethods.ValidateOneToNMessageModalExpectedValues.UIMessageFromExpensesModalInnerText = String.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPlease select a Related to from the list.\r\nPlease select a View to be used.", EnumHelper.GetEnumDescription(_executingProduct));
            cCustomEntityOneToNMethods.ValidateOneToNMessageModal();

            cCustomEntityOneToNMethods.PressCloseOneToNValidationModal();

            //Validate asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);

            #region Validate asterisks do not display when opening New 1:N Relationship modal
            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIDisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIGreenLightAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityOneToNMethods.UINewGreenLightWindowsWindow.UINewGreenLightDocument.UIViewAsterisk));

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);
            #endregion
        }
        #endregion

        #region Successfully edit n to one attribute in customEntity
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyEditNtoOneAttributeInCustomEntity_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //Click Attributes link
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityAttributesMethods.ClickEditFieldLink(customEntities[0].attribute[37].DisplayName);

            cCustomEntityOneToNMethods.ValidateOneToNModalTitleExpectedValues.UIDivOneToManyRelationPaneInnerText = "1:n Relationship Attribute: " + customEntities[0].attribute[37].DisplayName;
            cCustomEntityOneToNMethods.ValidateOneToNModalTitle();

            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            onetonControls.DisplayNameTxt.Text = customEntities[0].attribute[38].DisplayName;
            Assert.AreEqual(onetonControls.entitylist.Enabled, false);
            onetonControls.DescriptionTxt.Text = customEntities[0].attribute[38]._description;
            onetonControls.viewlist.SelectedItem = customEntities[1].view[1]._viewName;

            cCustomEntityOneToNMethods.PressSaveOneToNModal();

            cCustomEntityAttributesMethods.ValidateAttributesGrid(customEntities[0].attribute[38].DisplayName, customEntities[0].attribute[38]._description, EnumHelper.GetEnumDescription((FieldType)customEntities[0].attribute[38]._fieldType), customEntities[0].attribute[38]._isAuditIdenity.ToString());

            cCustomEntityAttributesMethods.ClickEditFieldLink(customEntities[0].attribute[38].DisplayName);

            onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            CustomEntitiesUtilities.CustomEntityOnetoNAttribute onetontoAdd = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[38];

            Assert.AreEqual(onetonControls.DisplayNameTxt.Text, onetontoAdd.DisplayName);
            Assert.AreEqual(onetonControls.entitylist.Enabled, false);
            Assert.AreEqual(onetonControls.DescriptionTxt.Text, onetontoAdd._description);
            Assert.AreEqual(onetonControls.viewlist.SelectedItem, customEntities[1].view[1]._viewName);
            Assert.AreEqual(onetonControls.viewlist.Enabled, true);
        }
        #endregion

        #region 37900 - Unsuccessfully edit n to one attribute in customEntity
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesUnsuccessfullyEditNtoOneAttributeInCustomEntity_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //Click Attributes link
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityAttributesMethods.ClickEditFieldLink(customEntities[0].attribute[37].DisplayName);

            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            onetonControls.DisplayNameTxt.Text = customEntities[0].attribute[38].DisplayName;
            Assert.AreEqual(onetonControls.entitylist.Enabled, false);
            onetonControls.DescriptionTxt.Text = customEntities[0].attribute[38]._description;
            onetonControls.viewlist.SelectedItem = customEntities[1].view[1]._viewName;

            cCustomEntityOneToNMethods.PressCancelSaveOneToNModal();

            cCustomEntityAttributesMethods.ValidateAttributesGrid(customEntities[0].attribute[37].DisplayName, customEntities[0].attribute[37]._description, EnumHelper.GetEnumDescription((FieldType)customEntities[0].attribute[37]._fieldType), customEntities[0].attribute[37]._isAuditIdenity.ToString());

            cCustomEntityAttributesMethods.ClickEditFieldLink(customEntities[0].attribute[37].DisplayName);

            onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            CustomEntitiesUtilities.CustomEntityOnetoNAttribute onetontoAdd = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[37];

            Assert.AreEqual(onetonControls.DisplayNameTxt.Text, onetontoAdd.DisplayName);
            Assert.AreEqual(onetonControls.entitylist.Enabled, false);
            Assert.AreEqual(onetonControls.DescriptionTxt.Text, onetontoAdd._description);
            Assert.AreEqual(onetonControls.viewlist.SelectedItem, customEntities[1].view[0]._viewName);
            Assert.AreEqual(onetonControls.viewlist.Enabled, true);
        }
        #endregion

        #region 37903 - Unsuccessfully add duplicate n to one attribute in customEntity
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesUnsuccessfullyAddDuplicateOneToNAttributeInCustomEnity_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            Assert.AreEqual(onetonControls.DisplayNameTxt.Text, "");
            Assert.AreEqual(onetonControls.DescriptionTxt.Text, "");
            Assert.AreEqual(onetonControls.entitylist.SelectedItem, emptylist);
            Assert.AreEqual(onetonControls.entitylist.Enabled, true);
            Assert.AreEqual(onetonControls.viewlist.SelectedItem, emptylist);
            Assert.AreEqual(onetonControls.viewlist.Enabled, true);

            onetonControls.DisplayNameTxt.Text = customEntities[0].attribute[37].DisplayName;
            onetonControls.entitylist.SelectedItem = customEntities[3].pluralName;
            onetonControls.viewlist.SelectedItem = customEntities[3].view[0]._viewName;

            cCustomEntityOneToNMethods.PressSaveOneToNModal();

            cCustomEntityOneToNMethods.ValidateOneToNMessageModalExpectedValues.UIMessageFromExpensesModalInnerText = string.Format("Message from {0}\r\n\r\n\r\nAn attribute or relationship with this Display name already exists.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntityOneToNMethods.ValidateOneToNMessageModal();
        }
        #endregion

        #region 37898 - Successfully delete custom entity 1:n Relationship
        /// <summary>
        /// 37898 - Successfully delete custom entity 1:n Relationship
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyDeleteNtoOneAttributeInCustomEntity_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntityAttributesMethods.ClickAttributesLink();

            //Click Attributes link
            cCustomEntityAttributesMethods.ClickAttributesLink();

            //Click delete
            cCustomEntityAttributesMethods.ClickDeleteFieldLink(customEntities[0].attribute[37].DisplayName);

            //Confirm deletion
            cCustomEntityAttributesMethods.PressOKToConfirmAttributeDeletion();

            //Validate deletion
            cCustomEntityAttributesMethods.ValidateAttributeDeletion(customEntities[0].attribute[37].DisplayName);
        }
        #endregion

        #region 37899 - Unuccessfully deleting custom entity 1:n Relationship
        /// <summary>
        /// 37899 - Successfully cancel deleting custom entity 1:n Relationship
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyCancelDeletingNtoOneAttributeInCustomEntity_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //Click Attributes link
            cCustomEntityAttributesMethods.ClickAttributesLink();

            //Click delete
            cCustomEntityAttributesMethods.ClickDeleteFieldLink(customEntities[0].attribute[37].DisplayName);

            //Cancel deletion
            cCustomEntityAttributesMethods.PressCancelToCancelAttributeDeletion();

            //Validate attribute is still displayed on the grid
            cCustomEntityAttributesMethods.ValidateAttributesGrid(customEntities[0].attribute[37].DisplayName, customEntities[0].attribute[37]._description, EnumHelper.GetEnumDescription((FieldType)customEntities[0].attribute[37]._fieldType), customEntities[0].attribute[37]._isAuditIdenity.ToString());
        }
        #endregion

        #region 37993 - Unsuccessfully delete custom entity where an 1:n relationship exists
        /// <summary>
        /// 37993 - Unsuccessfully delete custom entity where an 1:n relationship exists
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesUnsuccessfullyDeleteCustomEntityWhereAnNtoOneAttributeExists_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            //Click delete
            cCustomEntitiesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, customEntities[1].entityName);

            //Confirm deletion
            cCustomEntitiesMethods.PressOKDeleteCustomEntity();

            //Validate modal
            cCustomEntityOneToNMethods.ValidateOneToNMessageModalExpectedValues.UIMessageFromExpensesModalInnerText = string.Format("Message from {0}\r\n\r\n\r\nThe delete request was denied as a 1:n relationship to this GreenLight exists.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntityOneToNMethods.ValidateOneToNMessageModal();

            //Press Close modal
            cCustomEntityOneToNMethods.PressCloseOneToNValidationModal();

            //Ensure that the custom entity still displays on the grid
            cCustomEntitiesMethods.ValidateCustomEntityViewTable(cSharedMethods.browserWindow, customEntities[1].entityName, customEntities[1].description);
        }
        #endregion

        #region 39570 - Unsuccessfully delete Custom Entity View where  a relationship exist in a Second Custom Entity
        /// <summary>
        /// 39570 - Unsuccessfully delete Custom Entity View where  a relationship exist in a Second Custom Entity
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesUnsuccessfullyDeleteCustomEntityViewWhereAnNtoOneAttributeExists_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[1].entityId);

            //Click Views Link
            cCustomEntityViewsMethods.ClickViewsLink();

            //Click Delete against the view
            cCustomEntityViewsMethods.ClickDeleteFieldLink(customEntities[1].view[0]._viewName);

            //Click OK to confirm deletion
            cCustomEntityViewsMethods.ClickConfirmDeleteViewLink();

            //Validate modal
            cCustomEntityOneToNMethods.ValidateOneToNMessageModalExpectedValues.UIMessageFromExpensesModalInnerText = string.Format("Message from {0}\r\n\r\n\r\nThis view cannot be deleted as it is currently in use within a relationship.", EnumHelper.GetEnumDescription(_executingProduct)); 
            cCustomEntityOneToNMethods.ValidateOneToNMessageModal();

            //Press Close modal
            cCustomEntityOneToNMethods.PressCloseOneToNValidationModal();

            //Validate that the view still displays on the grid
            cCustomEntityViewsMethods.ValidateViewTable(customEntities[1].view[0]._viewName, customEntities[1].view[0]._description, true);
        }
        #endregion

        #region 37912 - Successfully verify maximum length for fields on 1:n relationshio modal, 37913 - Successfully verify truncate when copy/paste on fields for 1:n relationship modal
        /// <summary>
        /// 37912 - Successfully verify maximum length for fields on 1:n relationshio modal
        /// 37913 - Successfully verify truncate when copy/paste on fields for 1:n relationship modal
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyVerifyMaxLengthOnOneToNAttributeModal_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            ///Navigate to Attributes
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            //Initialise search space
            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            //Check max length
            Assert.AreEqual(250, onetonControls.DisplayNameTxt.MaxLength);
            onetonControls.DescriptionTxt.WaitForControlCondition(tc => onetonControls.DescriptionTxt.Text.Length == 4000, 200);
            Assert.AreEqual(4000, onetonControls.DescriptionTxt.GetMaxLength());

            //Copy and paste text - ensure truncation occurs
            Clipboard.Clear();
            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }

            cSharedMethods.PasteText(onetonControls.DisplayNameTxt);
            Assert.AreEqual(250, onetonControls.DisplayNameTxt.Text.Length);
            Clipboard.Clear();

            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }
            cSharedMethods.PasteText(onetonControls.DescriptionTxt);
            Mouse.Click();
            onetonControls.DescriptionTxt.WaitForControlCondition(tc => onetonControls.DescriptionTxt.Text.Length == 4000, 200);
            Assert.AreEqual(4000, onetonControls.DescriptionTxt.Text.Length);

            //Press Cancel
            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UICancelButton);
        }
        #endregion

        #region 41692 - Successfully add 1:n relationship on custom entity where 1:n relationship to another custom entity exists
        /// <summary>
        /// 41692 - Successfully add 1:n relationship on custom entity where 1:n relationship to another custom entity exists
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyAddOneToNRelationshipInCustomEntityWhereOneToNRelatioshipExists_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityAttributesMethods.ClickAttributesLink();

            #region Create a 1:n relationship with a different custom entity
            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            onetonControls.DisplayNameTxt.Text = "1:N relationship with " + customEntities[3].entityName;
            onetonControls.entitylist.SelectedItem = customEntities[3].pluralName;
            onetonControls.DescriptionTxt.Text = customEntities[0].attribute[37]._description;
            onetonControls.viewlist.SelectedItem = customEntities[3].view[0]._viewName;

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument.UISaveButton);

            cCustomEntityAttributesMethods.ValidateAttributesGrid(onetonControls.DisplayNameTxt.Text, customEntities[0].attribute[37]._description, EnumHelper.GetEnumDescription((FieldType)customEntities[0].attribute[37]._fieldType), customEntities[0].attribute[37]._isAuditIdenity.ToString());
            #endregion
        }
        #endregion
        
        #region 41517 - Unsuccessfully add 1:n relationship when an 1:n relationship with the same custom entity exists
        /// <summary>
        /// 41517 - Unsuccessfully add 1:n relationship when an 1:n relationship with the same custom entity exists
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesUnsuccessfullyAddOneToNRelationshipInCustomEntityWhereOneToNRelatioshipToTheSameCustomEntityExists_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();

            CustomEntityOnetoNControls onetonControls = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            ///Assert existing custom entity used in existing 1:n relationship does not display on the list
            foreach (var entityList in onetonControls.entitylist.Items)
            {
                Assert.AreNotEqual(customEntities[1].pluralName, ((HtmlListItem)entityList).DisplayText);
            }
        }
        #endregion

        #region 41606 -  Unsuccessfully create circular 1: N relationships between two or more Custom entities
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesUnsuccessfullyCreateCircularOnetoNrelationshipsBetweenTwoOrMoreCustomEntities_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            #region Navigate to a entity two -> oneton attribute
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[1].entityId);

            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();
            #endregion

            #region verify loop cant be made to custom entity oone
            CustomEntityOnetoNControls onetoncontrols = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            foreach (var entityList in onetoncontrols.entitylist.Items)
            {
                Assert.AreNotEqual(customEntities[3].pluralName, ((HtmlListItem)entityList).DisplayText);
            }
            #endregion

            #region Navigate to a entity three -> oneton attribute
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[3].entityId);

            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();
            #endregion

            #region verify loop cant be made to custom entity oone
            onetoncontrols = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);

            foreach (var entityList in onetoncontrols.entitylist.Items)
            {
                Assert.AreNotEqual(customEntities[4].pluralName, ((HtmlListItem)entityList).DisplayText);
            }
            #endregion
        }
        #endregion

        #region 37997 - Custom Entities - Succesfully delete a custom entity where an 1:n relationship exists

        /// <summary>
        /// 37997 Creates a 1:n relationship in a Custom entity before attempting to delete the custom entity
        /// </summary>
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyDeleteCustomEntityWhereContainsAnOneToNRelationship_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityAttributesMethods.ClickAttributesLink();

            //Ensure 1:n Relationship to CE2 exists
            cCustomEntityAttributesMethods.ValidateAttributesGrid(customEntities[0].attribute[37].DisplayName, customEntities[0].attribute[37]._description, EnumHelper.GetEnumDescription((FieldType)customEntities[0].attribute[37]._fieldType), customEntities[0].attribute[37]._isAuditIdenity.ToString());

            //Navigate out and delete Custom entity 1
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");
            
            //Verify deleted
            cCustomEntitiesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, customEntities[0].entityName);

            //Confirm delete
            cCustomEntitiesMethods.PressOKDeleteCustomEntity();

            //Validate deletion
            cCustomEntitiesMethods.ValidateCustomEntityDeletion(customEntities[0].entityName);

            //Reset the entityID back to 0 so the clean up doesn't attempt to delete it!
            customEntities[0].entityId = 0;
        }
        #endregion

        #region Successfully create 1: N relationships on Custom entities C to D where a 1:N exists on A to B and B to C
        [TestCategory("Greenlight One To Many"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityOneToNAttributesSuccessfullyAddOneToNRelationshipIn3rdCustomEntityWhereOneToNExistsOn1stTo2ndAndOn2ndTo3rdCustomEntity_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            #region Navigate to a entity three -> oneton attribute
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[3].entityId);

            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();
            #endregion

            #region populate values for 1:n to entity 4 and press save ad validate attributes grid
            CustomEntityOnetoNControls onetoncontrols = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);
            onetoncontrols.DisplayNameTxt.Text = customEntities[0].attribute[38].DisplayName;
            string description = "One to N relationship from Custom Entity Three to Four";
            onetoncontrols.DescriptionTxt.Text = description;
            onetoncontrols.entitylist.SelectedItem = customEntities[4].pluralName;
            onetoncontrols.viewlist.SelectedItem = customEntities[4].view[0]._viewName;
            cCustomEntityOneToNMethods.PressSaveOneToNModal();

            cCustomEntityAttributesMethods.ValidateAttributesGrid(customEntities[0].attribute[38].DisplayName, description, EnumHelper.GetEnumDescription((FieldType)customEntities[0].attribute[38]._fieldType), customEntities[0].attribute[38]._isAuditIdenity.ToString());
            #endregion

        }
        #endregion

        #region successfully create 1:n relationship on custom entity where the second has 1:n relationships with two other custom entities
        [TestCategory("Greenlight One To Many"), TestMethod]
        public void  CustomEntityOneToNAttributesSuccessfullyAddOneToNRelationshipOnCustomEntityWhereTheSecondHasOnetoNRelationshipWithTwoOtherCustomEntities_UITest()
        {
            Import1ToNDataToEx_CodedUIDatabase(testContextInstance.TestName);

            #region Navigate to a entity three -> oneton attribute
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[3].entityId);

            cCustomEntityAttributesMethods.ClickAttributesLink();

            cCustomEntityOneToNMethods.ClickNewOneToNRelationship();
            #endregion

            #region populate values for 1:n to entity 4 and press save ad validate attributes grid
            CustomEntityOnetoNControls onetoncontrols = new CustomEntityOnetoNControls(cCustomEntityOneToNMethods);
            onetoncontrols.DisplayNameTxt.Text = customEntities[0].attribute[38].DisplayName;
            string description = "One to N relationship from Custom Entity Three to Four";
            onetoncontrols.DescriptionTxt.Text = description;
            onetoncontrols.entitylist.SelectedItem = customEntities[4].pluralName;
            onetoncontrols.viewlist.SelectedItem = customEntities[4].view[0]._viewName;
            cCustomEntityOneToNMethods.PressSaveOneToNModal();

            cCustomEntityAttributesMethods.ValidateAttributesGrid(customEntities[0].attribute[38].DisplayName, description, EnumHelper.GetEnumDescription((FieldType)customEntities[0].attribute[38]._fieldType), customEntities[0].attribute[38]._isAuditIdenity.ToString());
            #endregion
        }
        #endregion

        #region Additional test attributes
        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            cSharedMethods = new SharedMethodsUIMap();
            _executingProduct = cGlobalVariables.GetProductFromAppConfig();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            cSharedMethods.Logon(_executingProduct, LogonType.administrator);
            CachePopulatorForOnetoN CustomEntityDataFromLithium = new CachePopulatorForOnetoN(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();

            Assert.IsNotNull(customEntities);
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            cSharedMethods.CloseBrowserWindow();
        }

        #endregion

        #region Test Init and cleanup methods

        [TestInitialize()]
        public void MyTestInitialize()
        {
            for (int entityindexer = customEntities.Count - 1; entityindexer > -1; entityindexer--)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntities[entityindexer].entityName = "Custom Entity " + genericName.ToString();
                customEntities[entityindexer].pluralName = "Custom Entity " + genericName.ToString();
                customEntities[entityindexer].description = "Custom Entity " + genericName.ToString();
                customEntities[entityindexer].entityId = 0;
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntities[entityindexer], _executingProduct);
                customEntities[entityindexer].entityId = result;
                Assert.IsTrue(result > 0);
                if (entityindexer == 1 || entityindexer == 3 || entityindexer == 4)
                {
                    foreach (CustomEntitiesUtilities.CustomEntityView view in customEntities[entityindexer].view)
                    {
                        view._viewid = 0;
                        CustomEntitiesUtilities.CreateCustomEntityView(customEntities[entityindexer], view, _executingProduct);
                        Assert.IsTrue(view._viewid > 0);
                    }
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();
        }

          [TestCleanup()]
           public void MyTestCleanup()
           {
               Assert.IsNotNull(customEntities);
               for (int entityindexer = 0; entityindexer < customEntities.Count; entityindexer++)
               {
                   if (customEntities[entityindexer].entityId > 0)
                   {
                       int result = CustomEntityDatabaseAdapter.DeleteCustomEntity(customEntities[entityindexer].entityId, _executingProduct);
                       Assert.AreEqual(0, result);
                   }
               }
               //CacheUtilities.DeleteCachedTablesAndFields();
           }

        #endregion

        private void Import1ToNDataToEx_CodedUIDatabase(string test)
        {
            int resultatt;
            switch (test)
            {
                case "CustomEntityOneToNAttributesSuccessfullyEditNtoOneAttributeInCustomEntity_UITest":
                case "CustomEntityOneToNAttributesSuccessfullyDeleteNtoOneAttributeInCustomEntity_UITest":
                case "CustomEntityOneToNAttributesSuccessfullyCancelDeletingNtoOneAttributeInCustomEntity_UITest":
                case "CustomEntityOneToNAttributesUnsuccessfullyDeleteCustomEntityWhereAnNtoOneAttributeExists_UITest":
                case "CustomEntityOneToNAttributesUnsuccessfullyDeleteCustomEntityViewWhereAnNtoOneAttributeExists_UITest":
                case "CustomEntityOneToNAttributesUnsuccessfullyEditNtoOneAttributeInCustomEntity_UITest":
                case "CustomEntityOneToNAttributesUnsuccessfullyAddDuplicateOneToNAttributeInCustomEnity_UITest":
                case "CustomEntityOneToNAttributesUnsuccessfullyEditOneToNAttributeWhereMandatoryFieldsareMissingInCustomEnity_UITest":
                case "CustomEntityOneToNAttributesSuccessfullyAddOneToNRelationshipInCustomEntityWhereOneToNRelatioshipExists_UITest":
                case "CustomEntityOneToNAttributesUnsuccessfullyAddOneToNRelationshipInCustomEntityWhereOneToNRelatioshipToTheSameCustomEntityExists_UITest":
                case "CustomEntityOneToNAttributesSuccessfullyDeleteCustomEntityWhereContainsAnOneToNRelationship_UITest":
                    CustomEntitiesUtilities.CustomEntityOnetoNAttribute onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[37];
                    onetonattribute._attributeid = 0;
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], onetonattribute, customEntities[1], _executingProduct);
                    Assert.IsTrue(resultatt > 0);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityOneToNAttributesUnsuccessfullyEditOneToNAttributeWhereDuplicateDetailsAreUsedInCustomEnity_UITest":
                    onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[37];
                    onetonattribute._attributeid = 0;
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], onetonattribute, customEntities[1], _executingProduct);

                    onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[38];
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], onetonattribute, customEntities[1], _executingProduct);
                    Assert.IsTrue(resultatt > 0);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityOneToNAttributesUnsuccessfullyCreateCircularOnetoNrelationshipsBetweenTwoOrMoreCustomEntities_UITest":
                    onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[37];
                    onetonattribute._attributeid = 0;
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[1], onetonattribute, customEntities[3], _executingProduct);

                    onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[38];
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[3], onetonattribute, customEntities[4], _executingProduct);
                    Assert.IsTrue(resultatt > 0);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityOneToNAttributesSuccessfullyAddOneToNRelationshipIn3rdCustomEntityWhereOneToNExistsOn1stTo2ndAndOn2ndTo3rdCustomEntity_UITest":
                    onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[37];
                    onetonattribute._attributeid = 0;
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], onetonattribute, customEntities[1], _executingProduct);

                    onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[38];
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[1], onetonattribute, customEntities[3], _executingProduct);
                    Assert.IsTrue(resultatt > 0);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                case "CustomEntityOneToNAttributesSuccessfullyAddOneToNRelationshipOnCustomEntityWhereTheSecondHasOnetoNRelationshipWithTwoOtherCustomEntities_UITest":
                    onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[37];
                    onetonattribute._attributeid = 0;
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], onetonattribute, customEntities[3], _executingProduct);

                    onetonattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[38];
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[1], onetonattribute, customEntities[3], _executingProduct);
                    Assert.IsTrue(resultatt > 0);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                default:
                    CustomEntitiesUtilities.CustomEntityOnetoNAttribute oneToNAttribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[0].attribute[37];
                    oneToNAttribute._attributeid = 0;
                    resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[0], oneToNAttribute, customEntities[1], _executingProduct);
                    Assert.IsTrue(resultatt > 0);
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
            }
        }

        internal class CachePopulatorForOnetoN : CachePopulator
        {

            public CachePopulatorForOnetoN(ProductType productType) : base (productType)
            {
            }
            public override string GetSQLStringForCustomEntity()
            {
                return "SELECT TOP 5 entityid, entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, enableAudiences, enablePopupWindow, defaultPopupView, tableid, createdby FROM customEntities";
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
                        if (!reader.IsDBNull(menuidOrdinal))
                        {
                            view._menuid = reader.GetInt32(menuidOrdinal);
                        }

                        view._allowAdd = false;/*reader.GetBoolean(allowAddOrdinal);*/
                        view._allowEdit = false;/*reader.GetBoolean(allowEditOrdinal);*/
                        view._allowDelete = reader.GetBoolean(allowDeleteOrdinal);
                        view._allowApproval = reader.GetBoolean(allowApprovalOrdinal);
                        view._createdOn = DateTime.Now;
                        //if (!reader.IsDBNull(addFormOrdinal))
                        //{
                            view.addform = null /*PopulateViewsFormDropdown(entity.form, reader.GetInt32(addFormOrdinal))*/;
                        //}
                        //if (!reader.IsDBNull(editFormOrdinal))
                        //{
                            view.editform = null /*PopulateViewsFormDropdown(entity.form, reader.GetInt32(editFormOrdinal))*/;
                        //}
                        if (!reader.IsDBNull(sortColumnJoinViaIDOrdinal))
                        {
                            //view.sortColumn_joinViaID = reader.GetInt32(sortColumnJoinViaIDOrdinal);
                        }
                        view.sortColumn = new CustomEntitiesUtilities.GreenLightSortColumn();

                        view.sortColumn._fieldID = reader.GetGuid(sortColumnOrdinal);
                        view.sortColumn._sortDirection = reader.GetByte(sortOrderOrdinal);
                        view._menuIcon = reader.IsDBNull(menuIconOrdinal) ? string.Empty : reader.GetString(menuIconOrdinal);
                        view._viewid = 0;
                        entity.view.Add(view);
                        #endregion
                    }
                    reader.Close();
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
                    int boolAttributeOrdinal = reader.GetOrdinal("boolAttribute");
                    #endregion

                    while (reader.Read())
                    {
                        CustomEntitiesUtilities.CustomEntityAttribute attribute = null;
                        FieldType type = (FieldType)reader.GetByte(fieldTypeOrdinal);
                        RelationshipType rtype = RelationshipType.ManyToOne;
                        if (!reader.IsDBNull(relationshipTypeOrdinal))
                        {
                             rtype = (RelationshipType)reader.GetByte(relationshipTypeOrdinal);
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
                        else if (type == FieldType.Relationship && rtype == RelationshipType.OneToMany)
                        {
                            CustomEntitiesUtilities.CustomEntityOnetoNAttribute newOneToNAttribute = new CustomEntitiesUtilities.CustomEntityOnetoNAttribute();
                            newOneToNAttribute._relationshipType = reader.GetByte(relationshipTypeOrdinal); ;
                            newOneToNAttribute._relatedTable = reader.GetGuid(relatedTableOrdinal);
                            newOneToNAttribute._viewId = reader.GetInt32(viewdIdOrdinal);
                            newOneToNAttribute._relatedEntityId = reader.GetInt32(relatedEntityOrdinal);
                            attribute = newOneToNAttribute;
                        }
                        else if (type == FieldType.Relationship && rtype == RelationshipType.ManyToOne)
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
            #endregion

        }

        internal class CustomEntityOnetoNControls
        {
            internal HtmlEdit DisplayNameTxt { get; set; }
            internal cHtmlTextAreaWrapper DescriptionTxt { get; set; }
            internal HtmlComboBox entitylist { get; set; }
            internal HtmlComboBox viewlist { get; set; }
            internal cHtmlTextAreaWrapper tooltipTxt { get; set; }

            protected CustomEntityOneToNAttributesUIMap _cCustomEntityOneToNMethods;
            protected ControlLocator<HtmlControl> _ControlLocator { get; private set; }

            internal CustomEntityOnetoNControls(CustomEntityOneToNAttributesUIMap cCustomEntityOneToNMethods)
            {
                _cCustomEntityOneToNMethods = cCustomEntityOneToNMethods;
                _ControlLocator = new ControlLocator<HtmlControl>();
                FindControls();
            }

            /// <summary>
            /// Locates controls that are declared within this class
            /// </summary>
            internal void FindControls()
            {
                DisplayNameTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_txtOnetonrelationshipname", new HtmlEdit(_cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument2));
                DescriptionTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl("ctl00_contentmain_txtOnetonrelationshipdescription", new cHtmlTextAreaWrapper(cSharedMethods.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_txtOnetonrelationshipdescription", _cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument2));
                entitylist = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_cmbOnetonrelationshipentity", new HtmlComboBox(_cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument2));
                viewlist = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_cmbOnetonrelationshipview", new HtmlComboBox(_cCustomEntityOneToNMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument2));
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
    }
}
