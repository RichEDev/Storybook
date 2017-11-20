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
using Auto_Tests.UIMaps.CustomEntityAttributesUIMapClasses;
using Auto_Tests.UIMaps.CustomEntitiesUIMapClasses;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Auto_Tests.Tools;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Threading;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Forms;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;

namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Attributes
{
    /// <summary>
    /// Summary description for CustomEntityAttributesUITests
    /// </summary>
    [CodedUITest]
    public class CustomEntityAttributesUITests
    {
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();

        private static List<CustomEntity> cachedData;

        private CustomEntityAttributesUIMap cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();

        private CustomEntitiesUIMap cCustomEntitiesMethods = new CustomEntitiesUIMap();

        private static ProductType _executingProduct = cGlobalVariables.GetProductFromAppConfig();

        public CustomEntityAttributesUITests()
        {
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            cSharedMethods.Logon(_executingProduct, LogonType.administrator);
            cachedData = ReadCustomEntityDataFromLithium();
            Assert.IsNotNull(cachedData);
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            cSharedMethods.CloseBrowserWindow();
        }

        /// <summary>
        /// Used to validate the attributes form page follows standards
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidatePageStandardsOnAttributesFormPage_UITest()
        {
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // Click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            DateTime dt = DateTime.Now;
            string day = dt.ToString("dd");
            string monthName = dt.ToString("MMMM");
            string year = dt.ToString("yyyy");
            string currentTimeStr = day + " " + monthName + " " + year;

            //Page Validation//<Mr James Lloyd | Active Sub Account: Main Account | 16 December 2011>
            if (_executingProduct == ProductType.expenses)
            {
                cSharedMethods.VerifyPageLayout("GreenLight: " + cachedData[0].entityName, "Before you can continue, please confirm the action required at the bottom of your screen.", "Company PolicyHelp & SupportExit", "Mr James Lloyd | Developer | " + currentTimeStr, "Page Options GreenLight Details Attributes Forms Views Help");
            }
            else
            {
                cSharedMethods.VerifyPageLayout("GreenLight: " + cachedData[0].entityName, "Before you can continue, please confirm the action required at the bottom of your screen.", "About | Exit", "Mr James Lloyd | Developer | " + currentTimeStr, "Page Options GreenLight Details Attributes Forms Views Help");
            }
            HtmlInputButton saveButton = cCustomEntitiesMethods.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton;
            string saveButtonExpectedText = "save";
            Assert.AreEqual(saveButtonExpectedText, saveButton.DisplayText);

            HtmlInputButton cancelButton = cCustomEntitiesMethods.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UICancelButton;
            string cancelButtonExpectedText = "cancel";
            Assert.AreEqual(cancelButtonExpectedText, cancelButton.DisplayText);
        }

        /// <summary>
        /// Used to validate the attributes form page follows standards
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidatePageStandardsOnAttributesModal_UITest()
        {
            string test = testContextInstance.TestName;

            //Insert attribute to the database
            ImportAttributeDataToEx_CodedUIDatabase(test);
            
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // Click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            // Click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateNewAttributeModalHeader();
            cCustomEntitiesAttributesMethods.ValidateDisplayNameField();
            cCustomEntitiesAttributesMethods.ValidateDescriptionField();
            cCustomEntitiesAttributesMethods.ValidateTooltipField();
            cCustomEntitiesAttributesMethods.ValidateMandatoryField();
            cCustomEntitiesAttributesMethods.ValidateTypeField();
            cCustomEntitiesAttributesMethods.ValidateTypeFieldDropdownList();
            cCustomEntitiesAttributesMethods.ValidateUsedForAuditField();
            cCustomEntitiesAttributesMethods.ValidateUniqueField();

            HtmlInputButton saveButton = cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UISaveButton;
            string saveButtonExpectedText = "save";
            Assert.AreEqual(saveButtonExpectedText, saveButton.DisplayText);

            HtmlInputButton cancelButton = cCustomEntitiesAttributesMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICancelButton;
            string cancelButtonExpectedText = "cancel";
            Assert.AreEqual(cancelButtonExpectedText, cancelButton.DisplayText);

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);
            #region Validate tabbing order when no type is selected 
            Assert.IsTrue(textAttribute.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsMandatory.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion

            cCustomEntitiesAttributesMethods.ValidateFieldsWhenTypeTextIsSelected();
            #region Validate tabbing order for attribute of type Text
            textAttribute.DisplayNameTxt.SetFocus();
            Assert.IsTrue(textAttribute.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsMandatory.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.FormatComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.MaxLengthTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.DisplayWidthComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion
            #region Validate tabbing order for attribute of type Integer
            textAttribute.TypeComboBx.SelectedItem = "Integer";
            textAttribute.DisplayNameTxt.SetFocus();
            Assert.IsTrue(textAttribute.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsMandatory.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion

            cCustomEntitiesAttributesMethods.ValidateFieldsWhenTypeDecimalIsSelected();
            #region Validate tabbing order for attribute of type Decimal
            CustomEntityAttributeDecimal decimalAttribute = new CustomEntityAttributeDecimal(cCustomEntitiesAttributesMethods);
            decimalAttribute.DisplayNameTxt.SetFocus();
            Assert.IsTrue(textAttribute.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(decimalAttribute.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(decimalAttribute.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(decimalAttribute.IsMandatory.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(decimalAttribute.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(decimalAttribute.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(decimalAttribute.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(decimalAttribute.PrecisionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion
            #region Validate tabbing order for attribute of type Currency
            textAttribute.TypeComboBx.SelectedItem = "Currency";
            textAttribute.DisplayNameTxt.SetFocus();
            Assert.IsTrue(textAttribute.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsMandatory.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion

            cCustomEntitiesAttributesMethods.ValidateFieldsWhenTypeYesNoIsSelected();
            #region Validate tabbing order for attribute of type Yes/No
            CustomEntityAttributeYesNo ynAttribute = new CustomEntityAttributeYesNo(cCustomEntitiesAttributesMethods);
            ynAttribute.DisplayNameTxt.SetFocus();
            Assert.IsTrue(ynAttribute.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(ynAttribute.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(ynAttribute.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(ynAttribute.IsMandatory.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(ynAttribute.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(ynAttribute.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(ynAttribute.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(ynAttribute.DefaultValue.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion

            cCustomEntitiesAttributesMethods.ValidateFieldsWhenTypeListIsSelected();
            cCustomEntitiesAttributesMethods.ValidateFieldsWhenTypeDateIsSelected();
            #region Validate tabbing order for attribute of type Date
            CustomEntityAttributeDate dateAttribute = new CustomEntityAttributeDate(cCustomEntitiesAttributesMethods);
            dateAttribute.DisplayNameTxt.SetFocus();
            Assert.IsTrue(dateAttribute.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(dateAttribute.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(dateAttribute.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(dateAttribute.IsMandatory.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(dateAttribute.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(dateAttribute.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(dateAttribute.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(dateAttribute.FormatCbx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion

            cCustomEntitiesAttributesMethods.ValidateFieldsWhenTypeLargeTextIsSelected();
            #region Validate tabbing order for attribute of type Large Text
            textAttribute.DisplayNameTxt.SetFocus();
            Assert.IsTrue(textAttribute.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.DescriptionTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TooltipTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsMandatory.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsAudit.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.IsUnique.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.LargeFormatComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(textAttribute.LargeMaxLengthTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion

            cCustomEntitiesAttributesMethods.ValidateCommentAdviceTextFieldExpectedValues.UICommentadvicetextLabelInnerText = "Comment advice text*";
            cCustomEntitiesAttributesMethods.ValidateFieldsWhenTypeCommentIsSelected();
            #region Validate tabbing order for attribute of type Comment
            CustomEntityAttributeSummary sumAttribute = new CustomEntityAttributeSummary(cCustomEntitiesAttributesMethods);
            sumAttribute.DisplayNameTxt.SetFocus();
            Assert.IsTrue(sumAttribute.DisplayNameTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(sumAttribute.DescriptionTxt.HasFocus);
            Assert.IsFalse(textAttribute.IsMandatory.Enabled);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(sumAttribute.TypeComboBx.HasFocus);
            Keyboard.SendKeys("{Tab}");
            // display in mobile app attribute
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(sumAttribute.CommentTxt.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus);
            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus);
            #endregion

            Keyboard.SendKeys("{Escape}");

            //Click Edit attribute
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[0].DisplayName);

            //Validate modal header
            cCustomEntitiesAttributesMethods.ValidateNewAttributeModalHeaderExpectedValues.UINewAttributePaneDisplayText = "Attribute: " + cachedData[0].attribute[0].DisplayName;

            Keyboard.SendKeys("{Escape}");

            //Delete Attribute from the database
            CustomEntitiesUtilities.DeleteCustomEntityAttribute(cachedData[0], cachedData[0].attribute[0], _executingProduct);
        }

        # region successfully add attributes

        # region Add StandardSingleLine
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityStandardSingleLineTextAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();


            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[0].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[0]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[0]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[0]._mandatory;
            textAttribute.TypeComboBx.SelectedItem = cachedData[0].attribute[0]._fieldType.ToString();
            //textAttribute.IsAudit.Checked = cachedData[0].attribute[0]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[0]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[0].EnableForMobile;

                ///GetEnumDescription
            string expectedValue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[0]._format);
            if (expectedValue == EnumHelper.GetEnumDescription(Format.Single_line))
            {
                textAttribute.FormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(Format.Single_line);
                textAttribute.MaxLengthTxt.Text = cachedData[0].attribute[0]._maxLength.ToString();
                textAttribute.DisplayWidthComboBx.SelectedItem = "Standard";
            }
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[0].DisplayName, cachedData[0].attribute[0]._description, EnumHelper.GetEnumDescription(FieldType.Text), "False");
                
            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[0].DisplayName);

            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[0];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            //Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)attributeToAdd._format), textAttribute.FormatComboBx.SelectedItem);
            Assert.AreEqual("Standard", textAttribute.DisplayWidthComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._maxLength, Convert.ToInt32(textAttribute.MaxLengthTxt.Text));
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add WideSingleLine
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityWideSingleLineTextAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);


            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[14].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[14]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[14]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[14]._mandatory;
            textAttribute.TypeComboBx.SelectedItem = cachedData[0].attribute[14]._fieldType.ToString();
            textAttribute.IsAudit.Checked = cachedData[0].attribute[14]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[14]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[14].EnableForMobile;


            ///GetEnumDescription
            string expectedValue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[14]._format);
            if (expectedValue == EnumHelper.GetEnumDescription(Format.SingleLineWide))
            {
                textAttribute.FormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(Format.Single_line);
                textAttribute.MaxLengthTxt.Text = cachedData[0].attribute[14]._maxLength.ToString();
                textAttribute.DisplayWidthComboBx.SelectedItem = "Wide";
            }
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[14].DisplayName, cachedData[0].attribute[14]._description, EnumHelper.GetEnumDescription(FieldType.Text), cachedData[0].attribute[14]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[14].DisplayName);

            //Refresh the attribute values
            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[14];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual("Single Line", textAttribute.FormatComboBx.SelectedItem);
            Assert.AreEqual("Wide", textAttribute.DisplayWidthComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._maxLength.ToString(), textAttribute.MaxLengthTxt.Text);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);


            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add Integer
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityIntegerAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[2].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[2]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[2]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[2]._mandatory;
            textAttribute.TypeComboBx.SelectedItem = cachedData[0].attribute[2]._fieldType.ToString();
            textAttribute.IsAudit.Checked = cachedData[0].attribute[2]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[2]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[2].EnableForMobile;


            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[2].DisplayName, cachedData[0].attribute[2]._description, EnumHelper.GetEnumDescription(FieldType.Integer), cachedData[0].attribute[2]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[2].DisplayName);

            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[2];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);


            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add decimal
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityDecimalAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeDecimal decimalAttribute = new CustomEntityAttributeDecimal(cCustomEntitiesAttributesMethods);

            decimalAttribute.DisplayNameTxt.Text = cachedData[0].attribute[3].DisplayName;
            decimalAttribute.DescriptionTxt.Text = cachedData[0].attribute[3]._description;
            decimalAttribute.TooltipTxt.Text = cachedData[0].attribute[3]._tooltip;
            decimalAttribute.IsMandatory.Checked = cachedData[0].attribute[3]._mandatory;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[3].EnableForMobile;

            if (cachedData[0].attribute[3]._fieldType.ToString() == "Number")
            {
                decimalAttribute.TypeComboBx.SelectedItem = "Decimal";
            }
            decimalAttribute.IsAudit.Checked = cachedData[0].attribute[3]._isAuditIdenity;
            decimalAttribute.IsUnique.Checked = cachedData[0].attribute[3]._isUnique;

            decimalAttribute.PrecisionTxt.Text = cachedData[0].attribute[3]._precision.ToString();

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[3].DisplayName, cachedData[0].attribute[3]._description, EnumHelper.GetEnumDescription(FieldType.Number), cachedData[0].attribute[3]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[3].DisplayName);

            //Refreshes the controls
            decimalAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[3];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, decimalAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, decimalAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, decimalAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, decimalAttribute.IsMandatory.Checked);
            Assert.AreEqual("Decimal", decimalAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, decimalAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, decimalAttribute.IsUnique.Checked);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
 
            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add Currency
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityCurrencyAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);


            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[4].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[4]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[4]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[4]._mandatory;
            textAttribute.TypeComboBx.SelectedItem = cachedData[0].attribute[4]._fieldType.ToString();
            textAttribute.IsAudit.Checked = cachedData[0].attribute[4]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[4]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[4].EnableForMobile;

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[4].DisplayName, cachedData[0].attribute[4]._description, EnumHelper.GetEnumDescription(FieldType.Currency), cachedData[0].attribute[4]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[4].DisplayName);

            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[4];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add MultilineLargeText
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityMultilineLargeTextAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();


            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);
            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[8].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[8]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[8]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[8]._mandatory;
            textAttribute.IsAudit.Checked = cachedData[0].attribute[8]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[8]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[8].EnableForMobile;

            string Value = EnumHelper.GetEnumDescription((FieldType)cachedData[0].attribute[8]._fieldType);
            if (Value == EnumHelper.GetEnumDescription(FieldType.LargeText))
            {
                textAttribute.TypeComboBx.SelectedItem = Value;
                textAttribute.LargeFormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(Format.Multi_line).ToString();
                textAttribute.LargeMaxLengthTxt.Text = cachedData[0].attribute[8]._maxLength.ToString();
            }

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[8].DisplayName, cachedData[0].attribute[8]._description, EnumHelper.GetEnumDescription(FieldType.LargeText), cachedData[0].attribute[8]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[8].DisplayName);


            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[8];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)attributeToAdd._format), textAttribute.LargeFormatComboBx.SelectedItem);
            string mxlengthExpected = "";
            if (attributeToAdd._commentText != null)
            {
                mxlengthExpected = attributeToAdd._maxLength.ToString();
            }
            Assert.AreEqual(mxlengthExpected, textAttribute.LargeMaxLengthTxt.Text.ToString());

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add FormattedLargeText
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityFormattedLargeTextAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();


            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);
            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[9].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[9]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[9]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[9]._mandatory;
            textAttribute.IsAudit.Checked = cachedData[0].attribute[9]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[9]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[9].EnableForMobile;

            string Value = EnumHelper.GetEnumDescription((FieldType)cachedData[0].attribute[9]._fieldType);
            if (Value == EnumHelper.GetEnumDescription(FieldType.LargeText))
            {
                textAttribute.TypeComboBx.SelectedItem = Value;
                textAttribute.LargeFormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(Format.FormattedTextBox).ToString();
            }

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[9].DisplayName, cachedData[0].attribute[9]._description, EnumHelper.GetEnumDescription(FieldType.LargeText), cachedData[0].attribute[9]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[9].DisplayName);


            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[9];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)attributeToAdd._format), textAttribute.LargeFormatComboBx.SelectedItem);
            string mxlengthExpected = "";
            if (attributeToAdd._commentText != null)
            {
                mxlengthExpected = attributeToAdd._maxLength.ToString();
            }
            cCustomEntitiesAttributesMethods.PressCancel();
        }
        #endregion

        # region Add Date
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityDateAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeDate dateAttribute = new CustomEntityAttributeDate(cCustomEntitiesAttributesMethods);


            dateAttribute.DisplayNameTxt.Text = cachedData[0].attribute[6].DisplayName;
            dateAttribute.DescriptionTxt.Text = cachedData[0].attribute[6]._description;
            dateAttribute.TooltipTxt.Text = cachedData[0].attribute[6]._tooltip;
            dateAttribute.TypeComboBx.SelectedItem  = EnumHelper.GetEnumDescription((FieldType)cachedData[0].attribute[6]._fieldType);
            dateAttribute.IsMandatory.Checked = cachedData[0].attribute[6]._mandatory;
            dateAttribute.IsAudit.Checked = cachedData[0].attribute[6]._isAuditIdenity;
            dateAttribute.IsUnique.Checked = cachedData[0].attribute[6]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[6].EnableForMobile;

            string expectedvalue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[6]._format);
            if (expectedvalue == EnumHelper.GetEnumDescription(Format.Date_Only))
            {
                dateAttribute.FormatCbx.SelectedItem = EnumHelper.GetEnumDescription(Format.Date_Only);

            }

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            if (EnumHelper.GetEnumDescription(FieldType.DateTime) == "Date")
            {
                string GridValue = "Date/Time";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[6].DisplayName, cachedData[0].attribute[6]._description, GridValue, cachedData[0].attribute[6]._isAuditIdenity.ToString());

            }
            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[6].DisplayName);

            //Refreshes the controls
            dateAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[6];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, dateAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, dateAttribute.DescriptionTxt.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), dateAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)attributeToAdd._format), dateAttribute.FormatCbx.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add Time
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityTimeAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeDate dateAttribute = new CustomEntityAttributeDate(cCustomEntitiesAttributesMethods);


            dateAttribute.DisplayNameTxt.Text = cachedData[0].attribute[7].DisplayName;
            dateAttribute.DescriptionTxt.Text = cachedData[0].attribute[7]._description;
            dateAttribute.TooltipTxt.Text = cachedData[0].attribute[7]._tooltip;
            dateAttribute.TypeComboBx.SelectedItem = EnumHelper.GetEnumDescription((FieldType)cachedData[0].attribute[7]._fieldType);
            dateAttribute.IsMandatory.Checked = cachedData[0].attribute[7]._mandatory;
            dateAttribute.IsAudit.Checked = cachedData[0].attribute[7]._isAuditIdenity;
            dateAttribute.IsUnique.Checked = cachedData[0].attribute[7]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[7].EnableForMobile;

            string expectedvalue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[7]._format);
            if (expectedvalue == EnumHelper.GetEnumDescription(Format.Time_Only))
            {
                dateAttribute.FormatCbx.SelectedItem = EnumHelper.GetEnumDescription(Format.Time_Only);

            }

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            if (EnumHelper.GetEnumDescription(FieldType.DateTime) == "Date")
            {
                string GridValue = "Date/Time";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[7].DisplayName, cachedData[0].attribute[7]._description, GridValue, cachedData[0].attribute[7]._isAuditIdenity.ToString());

            }
            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[7].DisplayName);

            //Refreshes the controls
            dateAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[7];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, dateAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, dateAttribute.DescriptionTxt.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), dateAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)attributeToAdd._format), dateAttribute.FormatCbx.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add DateandTime
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityDateandTimeAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeDate dateAttribute = new CustomEntityAttributeDate(cCustomEntitiesAttributesMethods);


            dateAttribute.DisplayNameTxt.Text = cachedData[0].attribute[5].DisplayName;
            dateAttribute.DescriptionTxt.Text = cachedData[0].attribute[5]._description;
            dateAttribute.TooltipTxt.Text = cachedData[0].attribute[5]._tooltip;
            dateAttribute.TypeComboBx.SelectedItem = EnumHelper.GetEnumDescription((FieldType)cachedData[0].attribute[5]._fieldType);
            dateAttribute.IsMandatory.Checked = cachedData[0].attribute[5]._mandatory;
            dateAttribute.IsAudit.Checked = cachedData[0].attribute[5]._isAuditIdenity;
            dateAttribute.IsUnique.Checked = cachedData[0].attribute[5]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[5].EnableForMobile;

            string expectedvalue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[5]._format);
            if (expectedvalue == EnumHelper.GetEnumDescription(Format.Date_And_Time))
            {
                dateAttribute.FormatCbx.SelectedItem = EnumHelper.GetEnumDescription(Format.Date_And_Time);

            }

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            if (EnumHelper.GetEnumDescription(FieldType.DateTime) == "Date")
            {
                string GridValue = "Date/Time";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[5].DisplayName, cachedData[0].attribute[5]._description, GridValue, cachedData[0].attribute[5]._isAuditIdenity.ToString());

            }
            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[5].DisplayName);

            //Refreshes the controls
            dateAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[5];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, dateAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, dateAttribute.DescriptionTxt.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), dateAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format)attributeToAdd._format), dateAttribute.FormatCbx.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add [Yes]Yes/No
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityYesAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeYesNo ynAttribute = new CustomEntityAttributeYesNo(cCustomEntitiesAttributesMethods);


            ynAttribute.DisplayNameTxt.Text = cachedData[0].attribute[12].DisplayName;
            ynAttribute.DescriptionTxt.Text = cachedData[0].attribute[12]._description;
            ynAttribute.TooltipTxt.Text = cachedData[0].attribute[12]._tooltip;
            ynAttribute.TypeComboBx.SelectedItem = EnumHelper.GetEnumDescription((FieldType)cachedData[0].attribute[12]._fieldType);
            ynAttribute.IsMandatory.Checked = cachedData[0].attribute[12]._mandatory;
            ynAttribute.IsAudit.Checked = cachedData[0].attribute[12]._isAuditIdenity;
            ynAttribute.IsUnique.Checked = cachedData[0].attribute[12]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[12].EnableForMobile;

            string expectedvalue =cachedData[0].attribute[12]._defaultValue;
            if (expectedvalue == "Yes")
            {
                ynAttribute.DefaultValue.SelectedItem = expectedvalue;

            }

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateUpdateDefaultValuesForYesAttribute();
            cCustomEntitiesAttributesMethods.PressOKOnJSWindow();

            if (EnumHelper.GetEnumDescription(FieldType.TickBox) == "Yes/No")
            {
                string GridValue = "Tickbox";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[12].DisplayName, cachedData[0].attribute[12]._description, GridValue, cachedData[0].attribute[12]._isAuditIdenity.ToString());

                ///click edit to verify attributes properties
                cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[12].DisplayName);
            }

            //Refreshes the controls
            ynAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[12];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, ynAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, ynAttribute.DescriptionTxt.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), ynAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._defaultValue, ynAttribute.DefaultValue.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add [No]Yes/No
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityNoAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeYesNo ynAttribute = new CustomEntityAttributeYesNo(cCustomEntitiesAttributesMethods);


            ynAttribute.DisplayNameTxt.Text = cachedData[0].attribute[13].DisplayName;
            ynAttribute.DescriptionTxt.Text = cachedData[0].attribute[13]._description;
            ynAttribute.TooltipTxt.Text = cachedData[0].attribute[13]._tooltip;
            ynAttribute.TypeComboBx.SelectedItem = EnumHelper.GetEnumDescription((FieldType)cachedData[0].attribute[13]._fieldType);
            ynAttribute.IsMandatory.Checked = cachedData[0].attribute[13]._mandatory;
            ynAttribute.IsAudit.Checked = cachedData[0].attribute[13]._isAuditIdenity;
            ynAttribute.IsUnique.Checked = cachedData[0].attribute[13]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[13].EnableForMobile;

            string expectedvalue = cachedData[0].attribute[13]._defaultValue;
            if (expectedvalue == "No")
            {
                ynAttribute.DefaultValue.SelectedItem = expectedvalue;
            }

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateUpdateDefaultValuesForNoAttribute();
            cCustomEntitiesAttributesMethods.PressOKOnJSWindow();

            if (EnumHelper.GetEnumDescription(FieldType.TickBox) == "Yes/No")
            {
                string GridValue = "Tickbox";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[13].DisplayName, cachedData[0].attribute[13]._description, GridValue, cachedData[0].attribute[13]._isAuditIdenity.ToString());

                ///click edit to verify attributes properties
                cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[13].DisplayName);
            }

            //Refreshes the controls
            ynAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[13];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, ynAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, ynAttribute.DescriptionTxt.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), ynAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._defaultValue, ynAttribute.DefaultValue.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Add Comment
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyAddCustomEntityCommentAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeSummary sumAttribute = new CustomEntityAttributeSummary(cCustomEntitiesAttributesMethods);


            sumAttribute.DisplayNameTxt.Text = cachedData[0].attribute[10].DisplayName;
            sumAttribute.DescriptionTxt.Text = cachedData[0].attribute[10]._description;
            string Value = EnumHelper.GetEnumDescription((FieldType)cachedData[0].attribute[10]._fieldType);
            sumAttribute.TypeComboBx.SelectedItem = Value;
            sumAttribute.CommentTxt.Text = cachedData[0].attribute[10]._commentText;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[10].EnableForMobile;


            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[10].DisplayName, cachedData[0].attribute[10]._description, EnumHelper.GetEnumDescription(FieldType.Comment), cachedData[0].attribute[10]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[10].DisplayName);

            //Refreshes the controls
            sumAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[10];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, sumAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, sumAttribute.DescriptionTxt.Text);
            string sCommentExpected = "";
            if (attributeToAdd._commentText != null)
            {
                sCommentExpected = attributeToAdd._commentText;
            }
            Assert.AreEqual(sCommentExpected, sumAttribute.CommentTxt.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), sumAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        #endregion

        # region successfully edit attributes

        # region Edit StandardSingleLine
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityStandardSingleLineTextAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            Thread.Sleep(4000);
            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[0].DisplayName);


            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[17].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[17]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[17]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[17]._mandatory;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[17]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[17].EnableForMobile;

            ///GetEnumDescription
            string expectedValue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[17]._format);
            if (expectedValue == EnumHelper.GetEnumDescription(Format.SingleLineWide))
            {
                //textAttribute.FormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(FORMAT.Single_line);
                textAttribute.DisplayWidthComboBx.SelectedItem = "Wide";
            }
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[17].DisplayName, cachedData[0].attribute[17]._description, EnumHelper.GetEnumDescription(FieldType.Text), cachedData[0].attribute[0]._isAuditIdenity.ToString());

            Thread.Sleep(4000);

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[17].DisplayName);


            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[17];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(cachedData[0].attribute[0]._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            //Assert.AreEqual("Single Line", textAttribute.FormatComboBx.SelectedItem);
            Assert.AreEqual(textAttribute.FormatComboBx.Enabled, false);
            Assert.AreEqual("Wide", textAttribute.DisplayWidthComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._maxLength, Convert.ToInt32(textAttribute.MaxLengthTxt.Text));
            Assert.AreEqual(textAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(textAttribute.IsAudit.Enabled, false);
            Assert.AreEqual(textAttribute.MaxLengthTxt.Enabled, false);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);


            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit WideSingleLine
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityWideSingleLineTextAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[14].DisplayName);

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);


            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[30].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[30]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[30]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[30]._mandatory;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[30]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[30].EnableForMobile;


            ///GetEnumDescription
            //string expectedValue = EnumHelper.GetEnumDescription((FORMAT)cachedData[0].attribute[30]._format);
            //if (expectedValue == EnumHelper.GetEnumDescription(FORMAT.SingleLineWide))
            //{
            //    textAttribute.FormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(FORMAT.Single_line);
            //    textAttribute.DisplayWidthComboBx.SelectedItem = "Standard";
            //}
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[30].DisplayName, cachedData[0].attribute[30]._description, EnumHelper.GetEnumDescription(FieldType.Text), cachedData[0].attribute[30]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[30].DisplayName);

            //Refresh the attribute values
            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[30];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            //Assert.AreEqual(EnumHelper.GetEnumDescription(FORMAT.Single_line), textAttribute.FormatComboBx.SelectedItem);
            Assert.AreEqual(textAttribute.FormatComboBx.Enabled, false);
            //Assert.AreEqual("Standard", textAttribute.DisplayWidthComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._maxLength.ToString(), textAttribute.MaxLengthTxt.Text);
            Assert.AreEqual(textAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(textAttribute.IsAudit.Enabled, true);
            Assert.AreEqual(textAttribute.MaxLengthTxt.Enabled, false);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);


            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit Integer
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityIntegerAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            ///click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[2].DisplayName);

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[19].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[19]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[19]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[19]._mandatory;
            textAttribute.IsAudit.Checked = cachedData[0].attribute[19]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[19]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[19].EnableForMobile;


            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[19].DisplayName, cachedData[0].attribute[19]._description, EnumHelper.GetEnumDescription(FieldType.Integer), cachedData[0].attribute[19]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[19].DisplayName);

            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[19];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.IsFalse(textAttribute.TypeComboBx.Enabled);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(textAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(textAttribute.IsAudit.Enabled, true);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit decimal
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityDecimalAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[3].DisplayName);

            CustomEntityAttributeDecimal decimalAttribute = new CustomEntityAttributeDecimal(cCustomEntitiesAttributesMethods);

            decimalAttribute.DisplayNameTxt.Text = cachedData[0].attribute[20].DisplayName;
            decimalAttribute.DescriptionTxt.Text = cachedData[0].attribute[20]._description;
            decimalAttribute.TooltipTxt.Text = cachedData[0].attribute[20]._tooltip;
            decimalAttribute.IsMandatory.Checked = cachedData[0].attribute[20]._mandatory;
            decimalAttribute.IsUnique.Checked = cachedData[0].attribute[20]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[20].EnableForMobile;


            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[20].DisplayName, cachedData[0].attribute[20]._description, EnumHelper.GetEnumDescription(FieldType.Number), cachedData[0].attribute[20]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[20].DisplayName);

            //Refreshes the controls
            decimalAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[20];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, decimalAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, decimalAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, decimalAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, decimalAttribute.IsMandatory.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, decimalAttribute.IsUnique.Checked);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(decimalAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(decimalAttribute.IsAudit.Enabled, true);
            Assert.AreEqual(decimalAttribute.PrecisionTxt.Enabled, false);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit Currency
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityCurrencyAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[4].DisplayName);

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);


            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[21].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[21]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[21]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[21]._mandatory;
            textAttribute.IsAudit.Checked = cachedData[0].attribute[21]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[21]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[21].EnableForMobile;


            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[21].DisplayName, cachedData[0].attribute[21]._description, EnumHelper.GetEnumDescription(FieldType.Currency), cachedData[0].attribute[21]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[21].DisplayName);

            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[21];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(textAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(textAttribute.IsAudit.Enabled, true);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        #region Edit MultilineLargeText
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityMultilineLargeTextAttributestoCustomEntity_UITest()
        {

            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[8].DisplayName);

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);
            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[25].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[25]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[25]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[25]._mandatory;
            textAttribute.IsAudit.Checked = cachedData[0].attribute[25]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[25]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[25].EnableForMobile;


            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[25].DisplayName, cachedData[0].attribute[25]._description, EnumHelper.GetEnumDescription(FieldType.LargeText), cachedData[0].attribute[25]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[25].DisplayName);


            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[25];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format.Multi_line)), textAttribute.LargeFormatComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(textAttribute.LargeFormatComboBx.Enabled, false);
            Assert.AreEqual(attributeToAdd._maxLength.ToString(), textAttribute.LargeMaxLengthTxt.Text.ToString());
            Assert.AreEqual(textAttribute.LargeMaxLengthTxt.Enabled, false);
            Assert.AreEqual(textAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(textAttribute.IsAudit.Enabled, true);
            Assert.AreEqual(textAttribute.LargeMaxLengthTxt.Enabled, false);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        #region Edit FormattedLargeText
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityFormattedLargeTextAttributestoCustomEntity_UITest()
        {

            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[9].DisplayName);

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);
            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[26].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[26]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[26]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[26]._mandatory;
            textAttribute.IsAudit.Checked = cachedData[0].attribute[26]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[26]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[26].EnableForMobile;

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[26].DisplayName, cachedData[0].attribute[26]._description, EnumHelper.GetEnumDescription(FieldType.LargeText), cachedData[0].attribute[26]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[26].DisplayName);


            //Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[26];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, textAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, textAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._tooltip, textAttribute.TooltipTxt.Text);
            Assert.AreEqual(attributeToAdd._mandatory, textAttribute.IsMandatory.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription(attributeToAdd._fieldType), textAttribute.TypeComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);
            Assert.AreEqual(attributeToAdd._isUnique, textAttribute.IsUnique.Checked);
            Assert.AreEqual(EnumHelper.GetEnumDescription((Format.FormattedTextBox)), textAttribute.LargeFormatComboBx.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(textAttribute.FormatComboBx.Enabled, true);
            Assert.AreEqual(textAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(textAttribute.IsAudit.Enabled, true);
            Assert.AreEqual(textAttribute.LargeMaxLengthTxt.Enabled, false);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit Date
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityDateAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[6].DisplayName);

            CustomEntityAttributeDate dateAttribute = new CustomEntityAttributeDate(cCustomEntitiesAttributesMethods);

            dateAttribute.DisplayNameTxt.Text = cachedData[0].attribute[23].DisplayName;
            dateAttribute.DescriptionTxt.Text = cachedData[0].attribute[23]._description;
            dateAttribute.TooltipTxt.Text = cachedData[0].attribute[23]._tooltip;
            dateAttribute.IsMandatory.Checked = cachedData[0].attribute[23]._mandatory;
            dateAttribute.IsAudit.Checked = cachedData[0].attribute[23]._isAuditIdenity;
            dateAttribute.IsUnique.Checked = cachedData[0].attribute[23]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[23].EnableForMobile;


            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            if (EnumHelper.GetEnumDescription(FieldType.DateTime) == "Date")
            {
                string GridValue = "Date/Time";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[23].DisplayName, cachedData[0].attribute[23]._description, GridValue, cachedData[0].attribute[23]._isAuditIdenity.ToString());

            }
            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[23].DisplayName);

            //Refreshes the controls
            dateAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[23];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, dateAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, dateAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(dateAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(dateAttribute.IsAudit.Enabled, true);
            Assert.AreEqual(dateAttribute.FormatCbx.Enabled, false);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit Time
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityTimeAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[7].DisplayName);

            CustomEntityAttributeDate dateAttribute = new CustomEntityAttributeDate(cCustomEntitiesAttributesMethods);

            dateAttribute.DisplayNameTxt.Text = cachedData[0].attribute[24].DisplayName;
            dateAttribute.DescriptionTxt.Text = cachedData[0].attribute[24]._description;
            dateAttribute.TooltipTxt.Text = cachedData[0].attribute[24]._tooltip;
            dateAttribute.IsMandatory.Checked = cachedData[0].attribute[24]._mandatory;
            dateAttribute.IsAudit.Checked = cachedData[0].attribute[24]._isAuditIdenity;
            dateAttribute.IsUnique.Checked = cachedData[0].attribute[24]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[24].EnableForMobile;

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            if (EnumHelper.GetEnumDescription(FieldType.DateTime) == "Date")
            {
                string GridValue = "Date/Time";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[24].DisplayName, cachedData[0].attribute[24]._description, GridValue, cachedData[0].attribute[24]._isAuditIdenity.ToString());

            }
            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[24].DisplayName);

            //Refreshes the controls
            dateAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[24];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, dateAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, dateAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(dateAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(dateAttribute.IsAudit.Enabled, true);
            Assert.AreEqual(dateAttribute.FormatCbx.Enabled, false);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit DateandTime
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityDateandTimeAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[5].DisplayName);

            CustomEntityAttributeDate dateAttribute = new CustomEntityAttributeDate(cCustomEntitiesAttributesMethods);

            dateAttribute.DisplayNameTxt.Text = cachedData[0].attribute[21].DisplayName;
            dateAttribute.DescriptionTxt.Text = cachedData[0].attribute[21]._description;
            dateAttribute.TooltipTxt.Text = cachedData[0].attribute[21]._tooltip;
            dateAttribute.IsMandatory.Checked = cachedData[0].attribute[21]._mandatory;
            dateAttribute.IsAudit.Checked = cachedData[0].attribute[21]._isAuditIdenity;
            dateAttribute.IsUnique.Checked = cachedData[0].attribute[21]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[21].EnableForMobile;

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            if (EnumHelper.GetEnumDescription(FieldType.DateTime) == "Date")
            {
                string GridValue = "Date/Time";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[21].DisplayName, cachedData[0].attribute[21]._description, GridValue, cachedData[0].attribute[21]._isAuditIdenity.ToString());

            }
            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[21].DisplayName);

            //Refreshes the controls
            dateAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[21];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, dateAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, dateAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(dateAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(dateAttribute.IsAudit.Enabled, true);
            Assert.AreEqual(dateAttribute.FormatCbx.Enabled, false);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit [Yes]Yes/No
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityYesAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[12].DisplayName);

            CustomEntityAttributeYesNo ynAttribute = new CustomEntityAttributeYesNo(cCustomEntitiesAttributesMethods);


            ynAttribute.DisplayNameTxt.Text = cachedData[0].attribute[28].DisplayName;
            ynAttribute.DescriptionTxt.Text = cachedData[0].attribute[28]._description;
            ynAttribute.TooltipTxt.Text = cachedData[0].attribute[28]._tooltip;
            ynAttribute.IsMandatory.Checked = cachedData[0].attribute[28]._mandatory;
            ynAttribute.IsUnique.Checked = cachedData[0].attribute[28]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[28].EnableForMobile;

            string expectedvalue = cachedData[0].attribute[28]._defaultValue;
            if (expectedvalue == "No")
            {
                ynAttribute.DefaultValue.SelectedItem = expectedvalue;

            }

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            if (EnumHelper.GetEnumDescription(FieldType.TickBox) == "Yes/No")
            {
                string GridValue = "Tickbox";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[28].DisplayName, cachedData[0].attribute[28]._description, GridValue, cachedData[0].attribute[28]._isAuditIdenity.ToString());

                ///click edit to verify attributes properties
                cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[28].DisplayName);
            }

            //Refreshes the controls
            ynAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[28];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, ynAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, ynAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._defaultValue, ynAttribute.DefaultValue.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(ynAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(ynAttribute.IsAudit.Enabled, true);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit [No]Yes/No
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityNoAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[13].DisplayName);

            CustomEntityAttributeYesNo ynAttribute = new CustomEntityAttributeYesNo(cCustomEntitiesAttributesMethods);


            ynAttribute.DisplayNameTxt.Text = cachedData[0].attribute[29].DisplayName;
            ynAttribute.DescriptionTxt.Text = cachedData[0].attribute[29]._description;
            ynAttribute.TooltipTxt.Text = cachedData[0].attribute[29]._tooltip;
            ynAttribute.IsMandatory.Checked = cachedData[0].attribute[29]._mandatory;
            ynAttribute.IsUnique.Checked = cachedData[0].attribute[29]._isUnique;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[29].EnableForMobile;

            string expectedvalue = cachedData[0].attribute[29]._defaultValue;
            if (expectedvalue == "Yes")
            {
                ynAttribute.DefaultValue.SelectedItem = expectedvalue;

            }

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            if (EnumHelper.GetEnumDescription(FieldType.TickBox) == "Yes/No")
            {
                string GridValue = "Tickbox";

                ///verify attribute is added to the grid
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[29].DisplayName, cachedData[0].attribute[29]._description, GridValue, cachedData[0].attribute[29]._isAuditIdenity.ToString());

                ///click edit to verify attributes properties
                cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[29].DisplayName);
            }

            //Refreshes the controls
            ynAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[29];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, ynAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, ynAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd._defaultValue, ynAttribute.DefaultValue.SelectedItem);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            Assert.AreEqual(ynAttribute.TypeComboBx.Enabled, false);
            Assert.AreEqual(ynAttribute.IsAudit.Enabled, true);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Edit Comment
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyEditCustomEntityCommentAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[10].DisplayName);

            CustomEntityAttributeSummary sumAttribute = new CustomEntityAttributeSummary(cCustomEntitiesAttributesMethods);


            sumAttribute.DisplayNameTxt.Text = cachedData[0].attribute[27].DisplayName;
            sumAttribute.DescriptionTxt.Text = cachedData[0].attribute[27]._description;
            sumAttribute.CommentTxt.Text = cachedData[0].attribute[27]._commentText;
            cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked = cachedData[0].attribute[27].EnableForMobile;

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///verify attribute is added to the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[27].DisplayName, cachedData[0].attribute[27]._description, EnumHelper.GetEnumDescription(FieldType.Comment), cachedData[0].attribute[27]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[27].DisplayName);

            //Refreshes the controls
            sumAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[27];

            //Assert attribute has correct property values
            Assert.AreEqual(attributeToAdd.DisplayName, sumAttribute.DisplayNameTxt.Text);
            Assert.AreEqual(attributeToAdd._description, sumAttribute.DescriptionTxt.Text);
            Assert.AreEqual(attributeToAdd.EnableForMobile, cCustomEntitiesAttributesMethods.GreenlightAttributeControlsWinow.GreenLightAttributeControlsDocument.DisplayInMobileAppCheckBox.Checked);
            string sCommentExpected = "";
            if (attributeToAdd._commentText != null)
            {
                sCommentExpected = attributeToAdd._commentText;
            }
            Assert.AreEqual(sCommentExpected, sumAttribute.CommentTxt.Text);
            Assert.AreEqual(sumAttribute.TypeComboBx.Enabled, false);

            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        #endregion

        #region Unsuccessfully add attibute when cancel is clicked

        # region Unscusessfully Add StandardSingleLine
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesUnsuccessfullyAddCustomEntityAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();


            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[0].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[0]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[0]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[0]._mandatory;
            textAttribute.TypeComboBx.SelectedItem = cachedData[0].attribute[0]._fieldType.ToString();
            //textAttribute.IsAudit.Checked = cachedData[0].attribute[0]._isAuditIdenity;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[0]._isUnique;

            ///GetEnumDescription
            string expectedValue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[0]._format);
            if (expectedValue == EnumHelper.GetEnumDescription(Format.Single_line))
            {
                textAttribute.FormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(Format.Single_line);
                textAttribute.MaxLengthTxt.Text = cachedData[0].attribute[0]._maxLength.ToString();
                textAttribute.DisplayWidthComboBx.SelectedItem = "Standard";
            }

            ///press cancel button
            cCustomEntitiesAttributesMethods.PressCancel();

            ///verify attribute is added to the grid ......Not
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[0].DisplayName, cachedData[0].attribute[0]._description, EnumHelper.GetEnumDescription(FieldType.Text), cachedData[0].attribute[0]._isAuditIdenity.ToString(), true, true, false);

        }
        #endregion

        #endregion

        #region Unsuccessfully edit attibute when cancel is clicked

        # region Edit StandardSingleLine
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesUnsuccessfullyEditAttributestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[0].DisplayName);


            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[17].DisplayName;
            textAttribute.DescriptionTxt.Text = cachedData[0].attribute[17]._description;
            textAttribute.TooltipTxt.Text = cachedData[0].attribute[17]._tooltip;
            textAttribute.IsMandatory.Checked = cachedData[0].attribute[17]._mandatory;
            textAttribute.IsUnique.Checked = cachedData[0].attribute[17]._isUnique;

            ///GetEnumDescription
            string expectedValue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[17]._format);
            if (expectedValue == EnumHelper.GetEnumDescription(Format.Single_line))
            {
                textAttribute.FormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(Format.Single_line);
                textAttribute.DisplayWidthComboBx.SelectedItem = "Wide";
            }
            ///press cancel button
            cCustomEntitiesAttributesMethods.PressCancel();

            ///verify attribute is updated to the grid....Not
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[0].DisplayName, cachedData[0].attribute[0]._description, EnumHelper.GetEnumDescription(FieldType.Text), cachedData[0].attribute[0]._isAuditIdenity.ToString());

        }
        #endregion

        #endregion

        # region Successfully validate Mandatory properties when adding attributes

        # region Validate StandardSingleLine Mandatory  Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforStandardSingleLineAttributes_UITest()
        {

            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();
            //Verify mandatory astricks appear
            
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
            "e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[0].DisplayName;
            
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            textAttribute.TypeComboBx.SelectedItem = cachedData[0].attribute[0]._fieldType.ToString();

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Format for this attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            ///GetEnumDescription
            string expectedValue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[0]._format);
            if (expectedValue == EnumHelper.GetEnumDescription(Format.Single_line))
            {
                textAttribute.FormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(Format.Single_line);

                ///Save attribute
                cCustomEntitiesAttributesMethods.PressSaveAttribute();

                cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Display width.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
                cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

                cCustomEntitiesAttributesMethods.PressClose();
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
                Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
                textAttribute.DisplayWidthComboBx.SelectedItem = "Standard";
            }
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();
            //Verify mandatory asterisks appear

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");
        }
        #endregion

        # region Validate WideSingleLine Mandatory  Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforWideSingleLineAttributes_UITest()
        {

            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
            "e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            
            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[14].DisplayName;

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            textAttribute.TypeComboBx.SelectedItem = cachedData[0].attribute[14]._fieldType.ToString();

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Format for this attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            ///GetEnumDescription
            string expectedValue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[14]._format);
            if (expectedValue == EnumHelper.GetEnumDescription(Format.Single_line))
            {
                textAttribute.FormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(Format.Single_line);

                ///Save attribute
                cCustomEntitiesAttributesMethods.PressSaveAttribute();

                cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Display width.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
                cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

                cCustomEntitiesAttributesMethods.PressClose();

                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
                Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
                Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

                textAttribute.DisplayWidthComboBx.SelectedItem = "Wide";
            }
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();
            cCustomEntitiesAttributesMethods.PressClose();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");

            //click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");
        }
        #endregion

        # region Validate Integer  Mandatory  Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforIntegerAttributes_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();
            
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
            "e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            
            cCustomEntitiesAttributesMethods.PressClose();

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = "Integer Attribute";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            

            textAttribute.TypeComboBx.SelectedItem = "Integer";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");
        }
        #endregion

        # region Validate Decimal  Mandatory  Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforDecimalAttributes_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
            "e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            

            CustomEntityAttributeDecimal decimalAttribute = new CustomEntityAttributeDecimal(cCustomEntitiesAttributesMethods);

            decimalAttribute.DisplayNameTxt.Text = "Decimal Attribute";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            decimalAttribute.TypeComboBx.SelectedItem = "Decimal";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a number for Precision (decimal places 1-5).", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            
            decimalAttribute.PrecisionTxt.Text= "2";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");
        }
        #endregion

        # region Validate Currency  Mandatory  Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforCurrencyAttributes_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
            "e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = "Currency Attribute";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            textAttribute.TypeComboBx.SelectedItem = "Currency";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");
        }
        #endregion

        # region Validate MultiLineLargeText Mandatory  Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforMultiLineLargeTextAttributes_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
            "e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = "MultiLineLargeText";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));


            textAttribute.TypeComboBx.SelectedItem = "Large Text";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Format for this attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));


            textAttribute.LargeFormatComboBx.SelectedItem = "Multiple Line";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");
        }
        #endregion

        # region Validate FormattedLargeText Mandatory  Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforFormattedLargeTextAttributes_UITest()
        {

            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
            "e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = "FormattedLargeText";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));

            textAttribute.TypeComboBx.SelectedItem = "Large Text";
            
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Format for this attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse (ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsTrue (ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
           
            textAttribute.LargeFormatComboBx.SelectedItem = "Formatted Text Box";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");
        }
        #endregion

        # region Validate Date Mandatory Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforDateAttributes_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
            "e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            CustomEntityAttributeDate dateAttribute = new CustomEntityAttributeDate(cCustomEntitiesAttributesMethods);

            dateAttribute.DisplayNameTxt.Text = "Date Attribute";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            dateAttribute.TypeComboBx.SelectedItem = "Date";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Format for this attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            dateAttribute.FormatCbx.SelectedItem = EnumHelper.GetEnumDescription(Format.Date_Only);

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");
        }

        #endregion

        # region Validate YesNo Mandatory Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforYesNoAttributes_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            ///Save attribute
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
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            CustomEntityAttributeYesNo ynAttribute = new CustomEntityAttributeYesNo(cCustomEntitiesAttributesMethods);

            ynAttribute.DisplayNameTxt.Text = "CheckBox Attribute";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();


            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));

            ynAttribute.TypeComboBx.SelectedItem = "Yes/No";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Default value for this attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));

            ynAttribute.DefaultValue.SelectedItem = "Yes";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();
            cCustomEntitiesAttributesMethods.ValidateUpdateDefaultValuesForYesAttribute();
            cCustomEntitiesAttributesMethods.PressOKOnJSWindow();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");
        }

        #endregion

        # region Validate Comment Mandatory Properties
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforCommentAttribute_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Display name for this attribute.\r\nPleas" +
            "e select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.CommentAdviceTextAsterisk));
            CustomEntityAttributeSummary sumAttribute = new CustomEntityAttributeSummary(cCustomEntitiesAttributesMethods);

            sumAttribute.DisplayNameTxt.Text = "Comment Attribute";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();


            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Type for the attribute.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();
            
            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.CommentAdviceTextAsterisk));

            sumAttribute.TypeComboBx.SelectedItem = "Comment";

            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributesExpectedValues.UICtl00_pnlMasterPopupPaneDisplayText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a value for Comment advice text.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMandatoryPropertiesOfAttributes();

            cCustomEntitiesAttributesMethods.PressClose();

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.CommentAdviceTextAsterisk));

            //Close attributes modal
            Keyboard.SendKeys("{Escape}");

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DefaultValueAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.DisplayWidthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.FormatAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.ListItemsAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.TypeAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow4.UICustomEntityCustomEnDocument.LargetextFormat));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow5.UICustomEntityCustomEnDocument.DateFormatAsterisk));
        }
        #endregion

        #endregion

        # region Successfully validate Audit Identifier properties functionality of attributes
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateAuditIdentifierpropertiesfunctionalityofattributes_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            ///click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///Validate default audit identifier
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid("ID", "Built in identification field for the GreenLight records", "Integer", "True", false, false);

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();


            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[0].DisplayName;
            textAttribute.TypeComboBx.SelectedItem = cachedData[0].attribute[0]._fieldType.ToString();

            /// as audit identifier
            textAttribute.IsAudit.Checked = cachedData[0].attribute[0]._isAuditIdenity;

            ///confirm audit identifier
            cCustomEntitiesAttributesMethods.ClickOkToConfirmAuditIdentifier();

            ///GetEnumDescription
            string expectedValue = EnumHelper.GetEnumDescription((Format)cachedData[0].attribute[0]._format);
            if (expectedValue == EnumHelper.GetEnumDescription(Format.Single_line))
            {
                textAttribute.FormatComboBx.SelectedItem = EnumHelper.GetEnumDescription(Format.Single_line);
                textAttribute.DisplayWidthComboBx.SelectedItem = "Standard";
            }
            ///Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();


            ///Validate default audit identifier changed 
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid("ID", "Built in identification field for the GreenLight records", "Integer", "False", false, false);

            ///verify New  audit identifier added to Grid 
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[0].DisplayName, " ", EnumHelper.GetEnumDescription(FieldType.Text), cachedData[0].attribute[0]._isAuditIdenity.ToString());

            ///click edit to verify attributes properties
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[0].DisplayName);


            ///Refreshes the controls
            textAttribute.FindControls();

            CustomEntitiesUtilities.CustomEntityAttribute attributeToAdd = cachedData[0].attribute[0];

            ///Assert attribute has audit identifier property values
            Assert.AreEqual(attributeToAdd._isAuditIdenity, textAttribute.IsAudit.Checked);

            cCustomEntitiesAttributesMethods.PressCancel();

            ///Delete current audit identifier
            cCustomEntitiesAttributesMethods.ClickDeleteFieldLink(cachedData[0].attribute[0].DisplayName);

            ///confirm deletion of audit identifier
            cCustomEntitiesAttributesMethods.ClickOkToConfirmAuditIdentifier();

            ///Validate default audit identifier restored
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid("ID", "Built in identification field for the GreenLight records", "Integer", "True", false, false);

        }
        #endregion

        # region Susessfully verify correct list of attribute types to custom entity
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyVerifyCorrectListofAttributeTypestoCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            List<string> allattributestypes = new List<string>();
            allattributestypes.Insert(0, "[None]");
            allattributestypes.Insert(1, EnumHelper.GetEnumDescription(FieldType.Text));
            allattributestypes.Insert(2, EnumHelper.GetEnumDescription(FieldType.Integer));
            allattributestypes.Insert(3, "Decimal");
            allattributestypes.Insert(4, EnumHelper.GetEnumDescription(FieldType.Currency));
            allattributestypes.Insert(5, EnumHelper.GetEnumDescription(FieldType.TickBox));
            allattributestypes.Insert(6, EnumHelper.GetEnumDescription(FieldType.List));
            allattributestypes.Insert(7, EnumHelper.GetEnumDescription(FieldType.DateTime));
            allattributestypes.Insert(8, EnumHelper.GetEnumDescription(FieldType.LargeText));
            allattributestypes.Insert(9, EnumHelper.GetEnumDescription(FieldType.Comment));
            allattributestypes.Insert(10, EnumHelper.GetEnumDescription(FieldType.Attachment));
            allattributestypes.Insert(11, EnumHelper.GetEnumDescription(FieldType.Contact));


            ///Assert list matches in count with dropdown options count
            Assert.AreEqual(allattributestypes.Count, textAttribute.TypeComboBx.Items.Count);
            bool OptionsCorrect = true;
            for (int i = 0; i <= textAttribute.TypeComboBx.Items.Count - 1; i++)
            {
                if (((HtmlListItem)(textAttribute.TypeComboBx.Items[i])).InnerText != allattributestypes[i])
                {
                    OptionsCorrect = false;
                    break;
                }
            }
            Assert.IsTrue(OptionsCorrect);

            ///press cancel button
            cCustomEntitiesAttributesMethods.PressCancel();

        }
        #endregion

        # region Validate Default Attributes
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyValidateDefaultAttributesInCustomEntity_UITest()
        {

            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            List<Row> trow = new List<Row>();
            //trow.Add(new Row("a", "a", "Text", false));
            trow.Add(new Row("Created By", "Built in attribute to show the user who created the data for this GreenLight record.", "Relationship", false, false, false));
            trow.Add(new Row("Created On", "Built in attribute to show the date the data for this GreenLight record was created on", "Date/Time", false, false, false));
            trow.Add(new Row("ID", "Built in identification field for the GreenLight records", "Integer", true, false, false));
            trow.Add(new Row("Modified By", "Built in attribute to show the user that modified the data for this GreenLight record", "Relationship", false, false, false));
            trow.Add(new Row("Modified On", "Built in attribute to show the date the data for this GreenLight record was modified on", "Date/Time", false, false, false));


            ///verify default attributes
            foreach (Row i in trow)
            {
                cCustomEntitiesAttributesMethods.ValidateAttributesGrid(i.DisplayName, i.Description, i.Type, i.UsedForAudit.ToString(), i.CanEdit, i.CanDelete);
            }

        }
        #endregion

        /// <summary>
        /// Successfully Delete Attribute 
        /// </summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyDeleteCustomEntityAttributes_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            //Click Attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Click delete
            cCustomEntitiesAttributesMethods.ClickDeleteFieldLink(cachedData[0].attribute[0].DisplayName);

            //Confirm deletion
            cCustomEntitiesAttributesMethods.PressOKToConfirmAttributeDeletion();

            //Validate deletion
            cCustomEntitiesAttributesMethods.ValidateAttributeDeletion(cachedData[0].attribute[0].DisplayName);
        }

        /// <summary>
        /// Successfully Cancel Deleting Attribute 
        /// </summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyCancelDeletingCustomEntityAttributes_UITest()
        {
            string test = testContextInstance.TestName;

            ImportAttributeDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            //Click Attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Click delete
            cCustomEntitiesAttributesMethods.ClickDeleteFieldLink(cachedData[0].attribute[0].DisplayName);

            //Cancel deletion
            cCustomEntitiesAttributesMethods.PressCancelToCancelAttributeDeletion();

            //Validate attribute is still displayed on the grid
            cCustomEntitiesAttributesMethods.ValidateAttributesGrid(cachedData[0].attribute[0].DisplayName, cachedData[0].attribute[0]._description, EnumHelper.GetEnumDescription((FieldType)cachedData[0].attribute[0]._fieldType), cachedData[0].attribute[0]._isAuditIdenity.ToString());
     
            //Delete Attribute from the database
            //CustomEntitiesUtilities.DeleteCustomEntityAttribute(cachedData[0], cachedData[0].attribute[0]);
        }

        /// <summary>
        /// Unsuccessfully add Attribute when duplicate information is used 
        /// </summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesUnsuccessfullyAddDuplicateAttribute_UITest()
        { 
            string test = testContextInstance.TestName;
            ImportAttributeDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            //Click Attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Click New Attribute
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            //Populate fields with duplicated data
            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[2].DisplayName;
            textAttribute.TypeComboBx.SelectedItem = cachedData[0].attribute[2]._fieldType.ToString();

            //Press Save
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nAn attribute or relationship with this Display name already exists.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModal();
            
            //Close modal
            cCustomEntitiesAttributesMethods.ClickCloseDuplicateValidationModal();

            //Close Attributes modal
            cCustomEntitiesAttributesMethods.PressCancel();

            //Delete Attribute from the database
            //CustomEntitiesUtilities.DeleteCustomEntityAttribute(cachedData[0], cachedData[0].attribute[2]);
        }

        /// <summary>
        /// Unsuccessfully edit Attribute when duplicate information is used 
        /// </summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesUnsuccessfullyEditDuplicateAttribute_UITest()
        {
            string test = testContextInstance.TestName;
            ImportAttributeDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            //Click Attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Click Edit Attribute
            cCustomEntitiesAttributesMethods.ClickEditFieldLink(cachedData[0].attribute[1].DisplayName);

            //Populate fields with duplicated data
            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = cachedData[0].attribute[0].DisplayName;

            //Press Save
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nAn attribute or relationship with this Display name already exists.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModal();

            //Close modal
            cCustomEntitiesAttributesMethods.ClickCloseDuplicateValidationModal();

            //Close Attributes modal
            cCustomEntitiesAttributesMethods.PressCancel();

            //Delete Attribute from the database
            //CustomEntitiesUtilities.DeleteCustomEntityAttribute(cachedData[0], cachedData[0].attribute[0]);
            //CustomEntitiesUtilities.DeleteCustomEntityAttribute(cachedData[0], cachedData[0].attribute[1]);
        }

        /// <summary>
        /// Successfully Sort Custom Entities Attribute Grid
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullySortAttributesGrid_UITest()
        {
            string test = testContextInstance.TestName;

            cSharedMethods.RestoreDefaultSortingOrder("gridAttributes", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Ensure Table is sorted correctly

            //Sorts Attribute Grid by Display Name column
            HtmlHyperlink displayNameLink = cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.UITbl_gridAttributesTable.UIDisplayNameHyperlink;
            cCustomEntitiesAttributesMethods.ClickTableHeader(displayNameLink);
            cCustomEntitiesAttributesMethods.VerifyCorrectSortingOrderForTable(SortAttributesByColumn.DisplayName, EnumHelper.TableSortOrder.DESC, cachedData[0].entityId, _executingProduct);
            cCustomEntitiesAttributesMethods.ClickTableHeader(displayNameLink);
            cCustomEntitiesAttributesMethods.VerifyCorrectSortingOrderForTable(SortAttributesByColumn.DisplayName, EnumHelper.TableSortOrder.ASC, cachedData[0].entityId, _executingProduct);

            //Sorts Attribute Grid by Description column
            displayNameLink = cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.UITbl_gridAttributesTable.UIDescriptionHyperlink;
            cCustomEntitiesAttributesMethods.ClickTableHeader(displayNameLink);
            cCustomEntitiesAttributesMethods.VerifyCorrectSortingOrderForTable(SortAttributesByColumn.Description, EnumHelper.TableSortOrder.ASC, cachedData[0].entityId, _executingProduct);
            cCustomEntitiesAttributesMethods.ClickTableHeader(displayNameLink);
            cCustomEntitiesAttributesMethods.VerifyCorrectSortingOrderForTable(SortAttributesByColumn.Description, EnumHelper.TableSortOrder.DESC, cachedData[0].entityId, _executingProduct);

            //Sorts Attribute Grid by Field Type column
            displayNameLink = cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.UITbl_gridAttributesTable1.UITypeHyperlink;
            cCustomEntitiesAttributesMethods.ClickTableHeader(displayNameLink);
            cCustomEntitiesAttributesMethods.VerifyCorrectSortingOrderForTable(SortAttributesByColumn.FieldType, EnumHelper.TableSortOrder.ASC, cachedData[0].entityId, _executingProduct);
            cCustomEntitiesAttributesMethods.ClickTableHeader(displayNameLink);
            cCustomEntitiesAttributesMethods.VerifyCorrectSortingOrderForTable(SortAttributesByColumn.FieldType, EnumHelper.TableSortOrder.DESC, cachedData[0].entityId, _executingProduct);

            //Sorts Attribute Grid by Used For Audit column
            displayNameLink = cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.UITbl_gridAttributesTable.UIUsedForAuditHyperlink;
            cCustomEntitiesAttributesMethods.ClickTableHeader(displayNameLink);
            cCustomEntitiesAttributesMethods.VerifyCorrectSortingOrderForTable(SortAttributesByColumn.UsedForAudit, EnumHelper.TableSortOrder.ASC, cachedData[0].entityId, _executingProduct);
            cCustomEntitiesAttributesMethods.ClickTableHeader(displayNameLink);
            cCustomEntitiesAttributesMethods.VerifyCorrectSortingOrderForTable(SortAttributesByColumn.UsedForAudit, EnumHelper.TableSortOrder.DESC, cachedData[0].entityId, _executingProduct);

            cSharedMethods.RestoreDefaultSortingOrder("gridAttributes", _executingProduct);
        }

        /// <summary>
        /// 38604 - Unsuccessfully add Attribute named Entity Currency on custom entity that is not a monetary record
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesUnsuccessfullyAddAttributeNamedEntityCurrencyOnNonMonetaryRecordCE_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            //Click Attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Click New Attribute
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            //Validate red asterisk do not display next to Display name
            Assert.IsFalse(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument18.UIItemPane.WaitForControlNotExist(5));

            //Populate fields with duplicated data
            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = "GreenLight Currency";
            textAttribute.TypeComboBx.SelectedItem = "Text";
            textAttribute.FormatComboBx.SelectedItem = "Multiple Line";

            //Press Save
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.VerifyEntityCurrencyAsReservedWordModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nGreenLight Currency is not allowed as a Display name for this attribute (reserved keyword).\r\nIt is used as a predefined attribute by the system.",
                new object[] { EnumHelper.GetEnumDescription(_executingProduct) });

            cCustomEntitiesAttributesMethods.VerifyEntityCurrencyAsReservedWordModal();

            //Close modal
            cCustomEntitiesAttributesMethods.PressCloseEntityCurrencyValidationModal();

            //Validate red asterisk displays next to Display name
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument18.UIItemPane.WaitForControlExist(5));
        }

        /// <summary>
        /// 38605 - Unsuccessfully add Attribute named Entity Currency on custom entity that is a monetary record
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesUnsuccessfullyAddAttributeNamedEntityCurrencyOnMonetaryRecordCE_UITest()
        {
            #region Insert custom entity that is a monetary record in the database
            cachedData[1].entityId = 0;
            int result = CustomEntityDatabaseAdapter.CreateCustomEntity(cachedData[1], _executingProduct);
            Assert.IsTrue(result > 0);
            cachedData[1].entityId = result;
            //CacheUtilities.DeleteCachedTablesAndFields();

            #endregion

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[1].entityId);

            //Click Attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Click New Attribute
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            //Validate red asterisk do not display next to Display name
            Assert.IsFalse(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument18.UIItemPane.WaitForControlNotExist(5));

            //Populate fields with duplicated data
            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = "GreenLight Currency";
            textAttribute.TypeComboBx.SelectedItem = "Text";
            textAttribute.FormatComboBx.SelectedItem = "Multiple Line";

            //Press Save
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            /*GreenLight Currency is not allowed as a Display name for this attribute (reserved keyword).*/

            //Validate modal
            cCustomEntitiesAttributesMethods.VerifyEntityCurrencyAsReservedWordModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nGreenLight Currency is not allowed as a Display name for this attribute (reserved keyword).\r\nIt is used as a predefined attribute by the system.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.VerifyEntityCurrencyAsReservedWordModal();

            //Close modal
            cCustomEntitiesAttributesMethods.PressCloseEntityCurrencyValidationModal();

            //Validate red asterisk displays next to Display name
            Assert.IsTrue(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument18.UIItemPane.WaitForControlExist(5));
        }

        /// <summary>
        /// 38345 - Unsuccessfully add Attribute of type Text where invalid data are used 
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesUnsuccessfullyAddAttributeOfTypeTextWhereInvalidDataAreUsed_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            //Click Attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Click New Attribute
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.DisplayNameTxt.Text = "Text Attribute Invalid Data Test";
            textAttribute.TypeComboBx.SelectedItem = "Text";

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk));

            #region Validate Maximum Length for Single Line attribute
            textAttribute.FormatComboBx.SelectedItem = "Single Line";
            textAttribute.DisplayWidthComboBx.SelectedItem = "Standard";
            textAttribute.MaxLengthTxt.Text = "a";
            
            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a number greater than or equal to 0 for Maximum length.\r\nPlease select a number less than or equal to 500 for Maximum length.",
                new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModal();

            //Close validation modal
            cCustomEntitiesAttributesMethods.PressCloseMaximumLengthValidationModal();

            //Validate red asterisk is displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk1));

            textAttribute.MaxLengthTxt.Text = "£";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModal();

            //Close validation modal
            cCustomEntitiesAttributesMethods.PressCloseMaximumLengthValidationModal();

            //Validate red asterisk is displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk1));

            textAttribute.MaxLengthTxt.Text = "-1";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a number greater than or equal to 0 for Maximum length.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModal();

            //Close validation modal
            cCustomEntitiesAttributesMethods.PressCloseMaximumLengthValidationModal();

            //Validate red asterisk is displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk1));

            textAttribute.MaxLengthTxt.Text = "999";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a number less than or equal to 500 for Maximum length.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModal();

            //Close validation modal
            cCustomEntitiesAttributesMethods.PressCloseMaximumLengthValidationModal();

            //Validate red asterisk is displayed
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk1));
            #endregion

            #region Validate Maximum Length for Multiple Line attribute
            textAttribute.FormatComboBx.SelectedItem = "Multiple Line";
            textAttribute.MaxLengthTxt.Text = "a";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a number greater than or equal to 0 for Maximum length.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModal();

            //Close validation modal
            cCustomEntitiesAttributesMethods.PressCloseMaximumLengthValidationModal();

            //Validate red asterisk is displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk1));

            textAttribute.MaxLengthTxt.Text = "-1";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a number greater than or equal to 0 for Maximum length.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModal();

            //Close validation modal
            cCustomEntitiesAttributesMethods.PressCloseMaximumLengthValidationModal();

            //Validate red asterisk is displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk1));

            textAttribute.MaxLengthTxt.Text = "£";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a number greater than or equal to 0 for Maximum length.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidateMaximumLengthModal();

            //Close validation modal
            cCustomEntitiesAttributesMethods.PressCloseMaximumLengthValidationModal();

            //Validate red asterisk is displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntitymyCustomWindow.UICustomEntitymyCustomDocument.MaximumLengthAsterisk1));
            #endregion

            cCustomEntitiesAttributesMethods.PressCancel();
        }

        /// <summary>
        /// 38345 - Unsuccessfully add Attribute of type Decimal where invalid data are used 
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesUnsuccessfullyAddAttributeOfTypeDecimalWhereInvalidDataAreUsed_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeDecimal decimalAttribute = new CustomEntityAttributeDecimal(cCustomEntitiesAttributesMethods);
            
            decimalAttribute.DisplayNameTxt.Text = "Decimal Attribute Invalid Data Test";
            decimalAttribute.TypeComboBx.SelectedItem = "Decimal";

            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow7.UICustomEntitymyCustomDocument.InvalidDataPrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow8.UICustomEntityCustomEnDocument.InvalidDataPrecisionAsterisk2));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));
           
            decimalAttribute.PrecisionTxt.Text = "a";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidatePrecisionModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Precision value greater than 0.\r\nPlease select a Precision value less than 6.",
                new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidatePrecisionModal();

            //Close modal
            cCustomEntitiesAttributesMethods.PressClosePercisionModal();

            //Validate red asterisk appears
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow7.UICustomEntitymyCustomDocument.InvalidDataPrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow8.UICustomEntityCustomEnDocument.InvalidDataPrecisionAsterisk2));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));

            decimalAttribute.PrecisionTxt.Text = "£";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidatePrecisionModal();

            //Close modal
            cCustomEntitiesAttributesMethods.PressClosePercisionModal();

            //Validate red asterisk appears
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow7.UICustomEntitymyCustomDocument.InvalidDataPrecisionAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow8.UICustomEntityCustomEnDocument.InvalidDataPrecisionAsterisk2));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));

            decimalAttribute.PrecisionTxt.Text = "0";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidatePrecisionModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Precision value greater than 0.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidatePrecisionModal();

            //Close modal
            cCustomEntitiesAttributesMethods.PressClosePercisionModal();

            //Validate red asterisk appears
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow7.UICustomEntitymyCustomDocument.InvalidDataPrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow8.UICustomEntityCustomEnDocument.InvalidDataPrecisionAsterisk2));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));

            decimalAttribute.PrecisionTxt.Text = "6";

            //Save attribute
            cCustomEntitiesAttributesMethods.PressSaveAttribute();

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidatePrecisionModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease select a Precision value less than 6.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntitiesAttributesMethods.ValidatePrecisionModal();

            //Close modal
            cCustomEntitiesAttributesMethods.PressClosePercisionModal();

            //Validate red asterisk appears
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow8.UICustomEntityCustomEnDocument.InvalidDataPrecisionAsterisk2));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow7.UICustomEntitymyCustomDocument.InvalidDataPrecisionAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow3.UICustomEntityCustomEnDocument.PrecisionAsterisk));

            cCustomEntitiesAttributesMethods.PressCancel();
        }

        /// <summary>
        /// 37865 - Successfully verify maximum size for fields on attribute modal 
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesSuccessfullyVerifyMaximumLengthOnFieldsOnAttributesModal_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            //Click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            //Click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            //Build Search Space for Attribute of Type Text
            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            textAttribute.TypeComboBx.SelectedItem = "Text";

            //Validate max length for fields
            Assert.AreEqual(250, textAttribute.DisplayNameTxt.MaxLength);
            Assert.AreEqual(4000, textAttribute.DescriptionTxt.GetMaxLength());
            Assert.AreEqual(4000, textAttribute.TooltipTxt.GetMaxLength());
            Assert.AreEqual(6, textAttribute.MaxLengthTxt.MaxLength);

            //Test Copy/Paste truncates text
            Clipboard.Clear();
            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }

            cSharedMethods.PasteText(textAttribute.DisplayNameTxt);
            Assert.AreEqual(250, textAttribute.DisplayNameTxt.Text.Length);

            cSharedMethods.PasteText(textAttribute.DescriptionTxt);
            Assert.AreEqual(4000, textAttribute.DescriptionTxt.GetMaxLength());

            cSharedMethods.PasteText(textAttribute.TooltipTxt);
            Assert.AreEqual(4000, textAttribute.TooltipTxt.GetMaxLength());

            cSharedMethods.PasteText(textAttribute.MaxLengthTxt);
            Assert.AreEqual(6, textAttribute.MaxLengthTxt.Text.Length);

            //Build Search Space for Attribute of Type Decimal
            CustomEntityAttributeDecimal decimalAttribute = new CustomEntityAttributeDecimal(cCustomEntitiesAttributesMethods);

            decimalAttribute.TypeComboBx.SelectedItem = "Decimal";

            //Validate max length for fields
            Assert.AreEqual(1, decimalAttribute.PrecisionTxt.MaxLength);

            //Test Copy/Paste truncates text
            cSharedMethods.PasteText(decimalAttribute.PrecisionTxt);
            Assert.AreEqual(1, decimalAttribute.PrecisionTxt.Text.Length);

            //Build Search space for Attribute of Typa Large Text 
            CustomEntityAttributeText largeTextAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);

            largeTextAttribute.TypeComboBx.SelectedItem = "Large Text";

            //Validate max length for fields
            Assert.AreEqual(6, largeTextAttribute.LargeMaxLengthTxt.MaxLength);

            //Test Copy/Paste truncates text
            cSharedMethods.PasteText(largeTextAttribute.LargeMaxLengthTxt);
            Assert.AreEqual(6, largeTextAttribute.LargeMaxLengthTxt.Text.Length);

            //Build Search space for Attribute of Type Comment
            CustomEntityAttributeSummary commentAttribute = new CustomEntityAttributeSummary(cCustomEntitiesAttributesMethods);

            commentAttribute.TypeComboBx.SelectedItem = "Comment";

            //Validate max length for fields
            Assert.AreEqual(4000, commentAttribute.CommentTxt.GetMaxLength());

            //Test Copy/Paste truncates text
            cSharedMethods.PasteText(commentAttribute.CommentTxt);
            Assert.AreEqual(4000, commentAttribute.CommentTxt.GetMaxLength());

            //Build Search space for Attribute of Type List

            commentAttribute.TypeComboBx.SelectedItem = "List";

            //Click Add List item
            cCustomEntitiesAttributesMethods.ClickAddListItemIcon();

            ControlLocator<HtmlEdit> locator = new ControlLocator<HtmlEdit>();
            HtmlEdit itemName = locator.findControl("ctl00_contentmain_txtlistitem", new HtmlEdit(cCustomEntitiesAttributesMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument7.UIAddEditListItemItemsPane.UIItemPane));

            //Validate max length for fields
            Assert.AreEqual(150, itemName.MaxLength);

            //Test Copy/Paste truncates text
            cSharedMethods.PasteText(itemName);
            Assert.AreEqual(150, itemName.Text.Length);

            Clipboard.Clear();

            cCustomEntitiesAttributesMethods.ClickSaveBtnForEditingListItem();

            cCustomEntitiesAttributesMethods.PressCancel();
        }

        /// <summary>
        /// 38607, 38608, 38609, 38610, 38611 - Unsuccessfully create attribute with display name ID, Created By, Created On, Modified By, Modified On
        ///</summary>
        [TestCategory("Greenlight Attributes"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityAttributesUnsuccessfullyAddAttributeWhereDefaultAttributesNamesAreUsed_UITest()
        {
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + cachedData[0].entityId);

            // click attributes link
            cCustomEntitiesAttributesMethods.ClickAttributesLink();

            ///click new attributes link
            cCustomEntitiesAttributesMethods.ClickNewAttributesLink();

            CustomEntityAttributeText textAttribute = new CustomEntityAttributeText(cCustomEntitiesAttributesMethods);
            textAttribute.TypeComboBx.SelectedItem = "Integer";
            #region Set Display name to be ID
            textAttribute.DisplayNameTxt.Text = "ID";
          
            ///Save attribute
            //cCustomEntitiesAttributesMethods.PressSaveAttribute();
            Keyboard.SendKeys("{Enter}");

            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nAn attribute or relationship with this Display name already exists.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModal();

            //Close modal
            cCustomEntitiesAttributesMethods.ClickCloseDuplicateValidationModal();
            #endregion
            #region Set Display name to be Created By
            textAttribute.DisplayNameTxt.Text = "Created By";

            ///Save attribute
            //cCustomEntitiesAttributesMethods.PressSaveAttribute();
            Keyboard.SendKeys("{Enter}");

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModal();

            //Close modal
            cCustomEntitiesAttributesMethods.ClickCloseDuplicateValidationModal();
            #endregion
            #region Set Display name to be Created On
            textAttribute.DisplayNameTxt.Text = "Created On";

            ///Save attribute
            //cCustomEntitiesAttributesMethods.PressSaveAttribute();
            Keyboard.SendKeys("{Enter}");

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModal();

            //Close modal
            cCustomEntitiesAttributesMethods.ClickCloseDuplicateValidationModal();
            #endregion
            #region Set Display name to be Modified By
            textAttribute.DisplayNameTxt.Text = "Modified By";

            ///Save attribute
            //cCustomEntitiesAttributesMethods.PressSaveAttribute();
            Keyboard.SendKeys("{Enter}");

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModal();

            //Close modal
            cCustomEntitiesAttributesMethods.ClickCloseDuplicateValidationModal();
            #endregion
            #region Set Display name to be Modified On
            textAttribute.DisplayNameTxt.Text = "Modified On";

            ///Save attribute
            //cCustomEntitiesAttributesMethods.PressSaveAttribute();
            Keyboard.SendKeys("{Enter}");

            //Validate modal
            cCustomEntitiesAttributesMethods.ValidateDuplicateDataModal();

            //Close modal
            cCustomEntitiesAttributesMethods.ClickCloseDuplicateValidationModal();
            #endregion
            //Close Attributes modal
            Keyboard.SendKeys("{Escape}");
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:


        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            cSharedMethods = new SharedMethodsUIMap();
            cCustomEntitiesMethods = new CustomEntitiesUIMap();
            cCustomEntitiesAttributesMethods = new CustomEntityAttributesUIMap();
            Guid genericName = new Guid();
            genericName = Guid.NewGuid();
            cachedData[0].entityName = "Custom Entity " + genericName.ToString();
            cachedData[0].pluralName = "Custom Entity " + genericName.ToString();
            cachedData[0].description = "Custom Entity " + genericName.ToString();
            cachedData[0].entityId = 0;
            int result = CustomEntityDatabaseAdapter.CreateCustomEntity(cachedData[0], _executingProduct);
            Assert.IsTrue(result > 0);
            cachedData[0].entityId = result;
            //CacheUtilities.DeleteCachedTablesAndFields();
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Assert.IsNotNull(cachedData);
            foreach (CustomEntity customEntity in cachedData)
            {
                int result = CustomEntityDatabaseAdapter.DeleteCustomEntity(customEntity.entityId, _executingProduct);
                //   Assert.AreEqual(0, result);
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

        /// <summary>
        /// Reads custom entity data required by codedui tests from lithium
        ///</summary>
        private static List<CustomEntity> ReadCustomEntityDataFromLithium()
        {
            List<CustomEntity> cDatabaseCustomEntity;

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string SQLString = "SELECT TOP 2 entityid, entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, AudienceViewType, enablePopupWindow, defaultPopupView, createdby, entityid FROM customEntities";
            
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
                int audiencesOrdinal = reader.GetOrdinal("audienceViewType");
                var defaultPopupViewOrdinal = reader.GetOrdinal("defaultPopupView");
                var enablePopupWindowrdinal = reader.GetOrdinal("enablePopupWindow");
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
                    customEntity.AudienceViewType = (AudienceViewType)reader.GetInt16(audiencesOrdinal);
                    customEntity.userId = AutoTools.GetEmployeeIDByUsername(_executingProduct);
                    customEntity.entityId = reader.GetInt32(entityIDOrdinal);
                    customEntity.EnablePopupWindow = reader.GetBoolean(enablePopupWindowrdinal);
                    customEntity.DefaultPopupView = null;
                    customEntity.attribute = ReadAttributeDataFromLithium(customEntity.entityId);
                    customEntity.entityId = 0;
                    cDatabaseCustomEntity.Add(customEntity);
                    #endregion 
                }
                reader.Close();
                
            }
            return cDatabaseCustomEntity;
        }

        /// <summary>
        /// Reads the attribute data required by codedui tests from Lithium
        ///</summary>
        private static List<CustomEntitiesUtilities.CustomEntityAttribute> ReadAttributeDataFromLithium(int customEntityID)
        {
            List<CustomEntitiesUtilities.CustomEntityAttribute>  cDatabaseAttribute = new List<CustomEntitiesUtilities.CustomEntityAttribute>();

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT createdby, modifiedby, display_name, description, relatedtable, tooltip, mandatory, fieldtype, createdon, maxlength, format, defaultvalue, precision, relationshiptype, related_entity, is_audit_identity, advicePanelText, is_unique, attributeid, boolAttribute FROM customEntityAttributes WHERE entityid = @entityid ";
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
                int boolAttributeOrdinal = reader.GetOrdinal("boolAttribute");
                //int enableOnMobileOrdinal = reader.GetOrdinal("DisplayInMobile");
                #endregion  

                
                while (reader.Read())
                {
                    CustomEntitiesUtilities.CustomEntityAttribute attribute;
                    #region Set values 
                    FieldType type = (FieldType)reader.GetByte(fieldTypeOrdinal);
                    //int attributeID = reader.GetInt32(attributeIDOrdinal);

                    attribute = new CustomEntitiesUtilities.CustomEntityAttribute();
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
                    attribute._isAuditIdenity = reader.GetBoolean(isAuditIdentityOrdinal);
                    attribute._commentText = reader.IsDBNull(advicePanelTextOrdinal) ? null : reader.GetString(advicePanelTextOrdinal);
                    attribute._isUnique = reader.GetBoolean(isUniqueOrdinal);
                    attribute.EnableImageLibrary = reader.GetBoolean(boolAttributeOrdinal);
                    attribute.EnableForMobile = true;
                    #endregion
                    cDatabaseAttribute.Add(attribute);
                }
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();

            return cDatabaseAttribute;
        }

        /// <summary>
        /// Imports attribute data to the database that the codedui will run on
        ///</summary>
        private void ImportAttributeDataToEx_CodedUIDatabase(string test)
        {
            int result;
            CustomEntity customEntity = new CustomEntity();
            customEntity = cachedData[0];
         //   foreach (CustomEntitiesUtilities.CustomEntity customEntity in cachedData)
         //   {  
                #region Insert data needed for each test
                switch(test)
                {
                    case "CustomEntityAttributesSuccessfullyDeleteCustomEntityAttributes_UITest":
                    case "CustomEntityAttributesSuccessfullyCancelDeletingCustomEntityAttributes_UITest":
                    case "CustomEntityAttributesSuccessfullyValidatePageStandardsOnAttributesModal_UITest":
                        customEntity.attribute[0]._attributeid = 0;
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[0], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[0]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                    case "CustomEntityAttributesSuccessfullyAddCustomEntityAttributeOfTypeList_UITest":
                        for (int i = 15; i < 17; i++)
                        {
                            result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[i], _executingProduct);
                            Assert.IsTrue(result > 0);
                            customEntity.attribute[i]._attributeid = result;
                            break;
                        }
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                    
                    case "CustomEntityAttributesSuccessfullySortAttributesGrid_UITest":
                        for (int i = 0; i < 10; i++)
                        {
                            result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[i], _executingProduct);
                            Assert.IsTrue(result > 0);
                            customEntity.attribute[i]._attributeid = result;
                            break;
                        }
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                    case "CustomEntityAttributesSuccessfullyEditCustomEntityStandardSingleLineTextAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[0], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[0]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                    case "CustomEntityAttributesSuccessfullyEditCustomEntityWideSingleLineTextAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[14], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[14]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesUnsuccessfullyAddDuplicateAttribute_UITest":
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityIntegerAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[2], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[2]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityDecimalAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[3], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[3]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityCurrencyAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[4], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[4]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityMultilineLargeTextAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[8], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[8]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityFormattedLargeTextAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[9], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[9]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityDateAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[6], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[6]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityTimeAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[7], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[7]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityDateandTimeAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[5], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[5]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityYesAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[12], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[12]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityNoAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[13], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[13]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyEditCustomEntityCommentAttributestoCustomEntity_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[10], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[10]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesSuccessfullyValidateMandatoryPropertiesforWideSingleLineAttributes_UITest":
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[14], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[14]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesUnsuccessfullyEditAttributestoCustomEntity_UITest":
                        customEntity.attribute[0]._attributeid = 0;
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[0], _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.attribute[0]._attributeid = result;
                        //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
                   case "CustomEntityAttributesUnsuccessfullyEditDuplicateAttribute_UITest":
                        for (int i = 0; i < 2; i++)
                        {
                            customEntity.attribute[i]._attributeid = 0;
                            result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[i], _executingProduct);
                            Assert.IsTrue(result > 0);
                            customEntity.attribute[i]._attributeid = result;
                            //CacheUtilities.DeleteCachedTablesAndFields();
                        }
                        break;
                    default:
                        customEntity.attribute[0]._attributeid = 0;
                        result = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntity, customEntity.attribute[0], _executingProduct);
                            Assert.IsTrue(result > 0);
                            customEntity.attribute[0]._attributeid = result;
                            //CacheUtilities.DeleteCachedTablesAndFields();
                        break;
              //  }
                #endregion
            }
        }
    }
}
