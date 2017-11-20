namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Forms
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using System.Windows.Input;

    using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities;
    using Auto_Tests.Tools;
    using Auto_Tests.UIMaps.ColoursUIMapClasses;
    using Auto_Tests.UIMaps.CustomEntitiesUIMapClasses;
    using Auto_Tests.UIMaps.CustomEntityAttributesUIMapClasses;
    using Auto_Tests.UIMaps.CustomEntityFormsNewUIMapClasses;
    using Auto_Tests.UIMaps.CustomEntityFormsUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;

    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;

    /// <summary>
    /// Summary description for CustonEntityForms
    /// </summary>
    [CodedUITest]
    public class CustomEntityFormsUITests
    {
        /// <summary>
        /// The custom entity forms new methods.
        /// </summary>
        private static CustomEntityFormsNewUIMap customEntityFormsNewMethods;

        /// <summary>
        /// The c shared methods.
        /// </summary>
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();

        /// <summary>
        /// The custom entities.
        /// </summary>
        private static List<CustomEntity> customEntities;

        /// <summary>
        /// The c custom entity forms methods.
        /// </summary>
        private CustomEntityFormsUIMap cCustomEntityFormsMethods = new CustomEntityFormsUIMap();

        /// <summary>
        /// The c custom entity methods.
        /// </summary>
        private CustomEntitiesUIMap cCustomEntityMethods = new CustomEntitiesUIMap();

        /// <summary>
        /// The c custom entity attribute methods.
        /// </summary>
        private CustomEntityAttributesUIMap cCustomEntityAttributeMethods = new CustomEntityAttributesUIMap();

        /// <summary>
        /// Test suite product will execute against
        /// </summary>
        private readonly static ProductType _executingProduct = cGlobalVariables.GetProductFromAppConfig();

        /// <summary>
        /// The text fields section.
        /// </summary>
        private static CustomEntitiesUtilities.CustomEntityFormSection textFieldsSection;

        /// <summary>
        /// The class init.
        /// </summary>
        /// <param name="ctx">
        /// The ctx.
        /// </param>
        [ClassInitialize]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            cSharedMethods = new SharedMethodsUIMap();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            browser.Maximized = true;
            cSharedMethods.Logon(_executingProduct, LogonType.administrator);
            CachePopulatorForForms CustomEntityDataFromLithium = new CachePopulatorForForms(_executingProduct);
            customEntities = CustomEntityDataFromLithium.PopulateCache();
            Assert.IsNotNull(customEntities);
        }

        /// <summary>
        /// The class clean up.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            cSharedMethods.CloseBrowserWindow();
        }

        #region 36564 - Successfully add Custom Entity form
        /// <summary>
        /// 36564 - Custom Entity Forms : Successfully add Custom Entity form
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsSuccessfullyAddFormToCustomEntity_UITest()
        {
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //click new form
            cCustomEntityMethods.ClickFormsLink();

            foreach (CustomEntitiesUtilities.CustomEntityForm ceForm in customEntities[0].form)
            {
                cCustomEntityMethods.ClickNewFormLink();

                //CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);

                Assert.AreEqual(string.Empty, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text);
                Assert.AreEqual(string.Empty, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox.Text);
                Assert.AreEqual(false, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowsubmenuCheckBox.Checked);
                Assert.AreEqual(false, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowbreadcrumbsCheckBox.Checked);
                Assert.AreEqual(string.Empty, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text);
                Assert.AreEqual(string.Empty, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text);
                Assert.AreEqual(string.Empty, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text);
                Assert.AreEqual(string.Empty, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.Text);
                Assert.AreEqual(string.Empty, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text);

                customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = ceForm._formName;
                customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox.Text = ceForm._description;
                customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowsubmenuCheckBox.Checked = ceForm._showSubMenus;
                customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowbreadcrumbsCheckBox.Checked = ceForm._showBreadcrumbs;
                customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = ceForm._saveButtonText;
                customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = ceForm._saveAndStayButtonText;
                customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = ceForm._saveAndNewButtonText;
                customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.Text = ceForm._saveAndDuplicateButtonText;
                customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text = ceForm._cancelButtonText;

                cCustomEntityFormsMethods.ClickSaveForm();

                cCustomEntityFormsMethods.ClickEditFieldLink(ceForm._formName);

                //formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);

                Assert.AreEqual(ceForm._formName, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text);
                Assert.AreEqual(ceForm._description, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox.Text);
                Assert.AreEqual(ceForm._showSubMenus, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowsubmenuCheckBox.Checked);
                Assert.AreEqual(ceForm._showBreadcrumbs, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowbreadcrumbsCheckBox.Checked);
                Assert.AreEqual(ceForm._saveButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text);
                Assert.AreEqual(ceForm._saveAndStayButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text);
                Assert.AreEqual(ceForm._saveAndNewButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text);
                Assert.AreEqual(ceForm._saveAndDuplicateButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.Text);
                Assert.AreEqual(ceForm._cancelButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text);

                cCustomEntityFormsMethods.ClickSaveForm();
            }
        }
        #endregion

        #region 37616 - Sucessfully cancel adding Custom Entity form
        /// <summary>
        /// 37616 - Custom Entity Forms : Sucessfully cancel adding Custom Entity form
        /// Also covers
        /// 39713 - Mandatory label style not applied on form modal load after editing
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsSuccessfullyCancelAddFormToCustomEntity_UITest()
        {
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //click new form
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityMethods.ClickNewFormLink();

            var ceForm = customEntities[0].form[0];

            //CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);

            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = ceForm._formName;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox.Text = ceForm._description;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowsubmenuCheckBox.Checked = ceForm._showSubMenus;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowbreadcrumbsCheckBox.Checked = ceForm._showBreadcrumbs;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = ceForm._saveButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = ceForm._saveAndStayButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = ceForm._saveAndNewButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.Text = ceForm._saveAndDuplicateButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text = ceForm._cancelButtonText;


            //Click new custom entity and verifiy that save button, save and stay, sabe and new, and cancel buttons are now mandatory
            ControlLocator<HtmlLabel> controlLocator = new ControlLocator<HtmlLabel>();
            HtmlLabel formButtonLabelField = controlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_lblsavebuttontext", new HtmlLabel(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
            Assert.AreEqual("Text for 'save' button", formButtonLabelField.DisplayText);

            //save and new button text
            formButtonLabelField = controlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_lblsaveandnewbuttontext", new HtmlLabel(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
            Assert.AreEqual("Text for 'save and new' button", formButtonLabelField.DisplayText);

            //Save and stay button text
            formButtonLabelField = controlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_lblsaveandstaybuttontext", new HtmlLabel(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
            Assert.AreEqual("Text for 'save and stay' button", formButtonLabelField.DisplayText);

            //Cancel button text
            formButtonLabelField = controlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_lblcancelbuttontext", new HtmlLabel(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
            Assert.AreEqual("Text for 'cancel' button", formButtonLabelField.DisplayText);


            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ValidateFormTable(cSharedMethods.browserWindow, customEntities[0].form[0]._formName, customEntities[0].form[0]._description, false);

            //Click new custom entity and verifiy that save button, save and stay, sabe and new, and cancel buttons are now mandatory

            cCustomEntityMethods.ClickNewFormLink();

            //Save button text

            formButtonLabelField = controlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_lblsavebuttontext", new HtmlLabel(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
            Assert.AreEqual("Text for 'save' button*", formButtonLabelField.DisplayText);

            //save and new button text
            formButtonLabelField = controlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_lblsaveandnewbuttontext", new HtmlLabel(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
            Assert.AreEqual("Text for 'save and new' button*", formButtonLabelField.DisplayText);

            //Save and stay button text
            formButtonLabelField = controlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_lblsaveandstaybuttontext", new HtmlLabel(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
            Assert.AreEqual("Text for 'save and stay' button*", formButtonLabelField.DisplayText);

            //Cancel button text
            formButtonLabelField = controlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_lblcancelbuttontext", new HtmlLabel(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
            Assert.AreEqual("Text for 'cancel' button*", formButtonLabelField.DisplayText);
        }
        #endregion

        #region 36634 - Successfully Delete Form
        /// <summary>
        /// 36634 - Custom Entity Forms : Successfully Delete Form
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsSuccessfullyDeleteFormToCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityFormsMethods.ClickFormsLink();

            //Click Delete
            cCustomEntityFormsMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, customEntities[0].form[0]._formName);

            //Validate prompt message for form deletion
            cCustomEntityFormsMethods.ValidateFormDeletionMessage();

            //Confirm form deletion
            cCustomEntityFormsMethods.PressOKConfirmFormDeletion();

            //Validate form has been deleted from the grid
            cCustomEntityFormsMethods.ValidateFormDeletion(customEntities[0].form[0]._formName);
        }
        #endregion

        #region 37618 - Successfully Cancel Deleting Form
        /// <summary>
        /// 37618 - Custom Entity Forms : Successfully Cancel Deleting Form
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsSuccessfullyCancelDeletingFormToCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityFormsMethods.ClickFormsLink();

            //Click Delete
            cCustomEntityFormsMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, customEntities[0].form[0]._formName);

            //Confirm form deletion
            cCustomEntityFormsMethods.PressCancelDeleteForm();

            //Validate form still displays on the grid
            cCustomEntityFormsMethods.ValidateFormTable(cSharedMethods.browserWindow, customEntities[0].form[0]._formName, customEntities[0].form[0]._description);
        }
        #endregion

        #region 36636 - Unsuccessfully add duplicate Form
        /// <summary>
        /// 36636 - Custom Entity Forms : Unsuccessfully add duplicate Form
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyAddDuplicateFormToCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            //Click New Form
            cCustomEntityMethods.ClickNewFormLink();

            //Populate fields with the same data as an existing form
            // CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);

            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = customEntities[0].form[0]._formName;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = customEntities[0].form[0]._saveButtonText;

            //Click Save Form
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal for duplicates is displayed
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateDuplicateDetailsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateDuplicateDetailsModalExpectedValues.UIDivMasterPopupPaneInnerText = "Message from Framework\r\n\r\n\r\nThe Form name you have entered already exists.";
                cCustomEntityFormsMethods.ValidateDuplicateDetailsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseDuplicateValidationModal();

            //Close Forms modal
            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }
        #endregion

        #region 39662 - Unsuccessfully edit form where duplicate information is used
        /// <summary>
        /// 39662 - Custom Entity Forms : Unsuccessfully edit form where duplicate information is used
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyEditDuplicateFormToCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            //Click Edit Form
            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[1]._formName);

            //Populate fields with the same data as an existing form
            // CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);

            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = customEntities[0].form[0]._formName;

            //Click Save Form
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal for duplicates is displayed
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateDuplicateDetailsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateDuplicateDetailsModalExpectedValues.UIDivMasterPopupPaneInnerText = "Message from Framework\r\n\r\n\r\nThe Form name you have entered already exists.";
                cCustomEntityFormsMethods.ValidateDuplicateDetailsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseDuplicateValidationModal();

            //Close Forms modal
            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }
        #endregion

        #region 36633 - Sucessfully Edit custom entity form
        /// <summary>
        /// 36633 - Custom Entity Forms : Sucessfully Edit custom entity form
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsSuccessfullyEditFormToCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();
            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            // CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);

            var ceForm = customEntities[0].form[1];

            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = ceForm._formName;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox.Text = ceForm._description;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowsubmenuCheckBox.Checked = ceForm._showSubMenus;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowbreadcrumbsCheckBox.Checked = ceForm._showBreadcrumbs;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = ceForm._saveButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = ceForm._saveAndStayButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = ceForm._saveAndNewButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.Text = ceForm._saveAndDuplicateButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text = ceForm._cancelButtonText;

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ValidateFormTable(cSharedMethods.browserWindow, ceForm._formName, ceForm._description);

            cCustomEntityFormsMethods.ClickEditFieldLink(ceForm._formName);

            //Verify update
            // formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);
            Assert.AreEqual(ceForm._formName, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text);
            Assert.AreEqual(ceForm._description, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox.Text);
            Assert.AreEqual(ceForm._showSubMenus, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowsubmenuCheckBox.Checked);
            Assert.AreEqual(ceForm._showBreadcrumbs, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowbreadcrumbsCheckBox.Checked);
            Assert.AreEqual(ceForm._saveButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text);
            Assert.AreEqual(ceForm._saveAndStayButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text);
            Assert.AreEqual(ceForm._saveAndNewButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text);
            Assert.AreEqual(ceForm._saveAndDuplicateButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.Text);
            Assert.AreEqual(ceForm._cancelButtonText, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text);

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

        }
        #endregion

        #region 37619 - Successfully Cancel Editing Form
        /// <summary>
        /// 37619 - Custom Entity Forms : Successfully Cancel Editing Form
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsSuccessfullyCancelEditingFormToCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);
            ///Enter as custom entity to insert attributes to 
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();
            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            // CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);
            var ceForm = customEntities[0].form[1];

            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = ceForm._formName;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox.Text = ceForm._description;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowsubmenuCheckBox.Checked = ceForm._showSubMenus;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.ShowbreadcrumbsCheckBox.Checked = ceForm._showBreadcrumbs;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = ceForm._saveButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = ceForm._saveAndStayButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = ceForm._saveAndNewButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.Text = ceForm._saveAndDuplicateButtonText;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text = ceForm._cancelButtonText;

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ValidateFormTable(cSharedMethods.browserWindow, customEntities[0].form[1]._formName, customEntities[0].form[1]._description, false);
        }
        #endregion

        #region 36632 - Unsuccessfully add form where mandatory fields are missing
        /// <summary>
        /// 36632 - Custom Entity Forms : Unsuccessfully add form where mandatory fields are missing
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyAddFormWhereMandatoryFieldsAreMissingToCustomEntity_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            //Click New Form link
            cCustomEntityMethods.ClickNewFormLink();

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));

            #region Leave all fields empty
            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.\r\nPlease enter text for at le" +
            "ast one of the Form buttons.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion

            #region Populate Form name but leave all button fields empty
            //Populate Form name field
            // CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);

            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = customEntities[0].form[0]._formName;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter text for at le" +
            "ast one of the Form buttons.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter text for at le" +
            "ast one of the Form buttons.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion

            #region Empty Form name field and populate Text for 'save' button field
            //Empty Form name field and populate Text for 'save' button
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = customEntities[0].form[0]._saveButtonText;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion

            #region Empty Text for 'save' button field and populate Text for 'save and new' button field
            //Empty Text for 'save' button field and populate Text for 'save and new' button field
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = customEntities[0].form[0]._saveAndNewButtonText;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion

            #region Empty Text for 'save and new' button field and populate Text for 'save and stay' button field
            //Empty Text for 'save and new' button field and populate Text for 'save and stay' button field
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = customEntities[0].form[0]._saveAndStayButtonText;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion

            #region Empty Text for 'save and stay' button field and populate Text for 'cancel' button field
            //Empty Text for 'save and stay' button field and populate Text for 'cancel' button field
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text = customEntities[0].form[0]._cancelButtonText;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion

            //Close Forms modal
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = customEntities[0].form[0]._formName;
            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            //Click New Form link
            cCustomEntityMethods.ClickNewFormLink();

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
        }
        #endregion

        #region 39663 - Unsuccessfully edit form where mandatory fields are missing
        /// <summary>
        /// 39663 - Custom Entity Forms : Unsuccessfully edit form where mandatory fields are missing
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyEditFormWhereMandatoryFieldsAreMissingToCustomEntity_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            //Click Edit Form
            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));

            //Empty mandatory fields
            // CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);

            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text = string.Empty;

            #region Leave all fields empty
            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.\r\nPlease enter text for at le" +
            "ast one of the Form buttons.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion
            #region Populate Form name but leave all button fields empty
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = customEntities[0].form[0]._formName;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter text for at le" +
            "ast one of the Form buttons.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter text for at le" +
            "ast one of the Form buttons.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion
            #region Empty Form name field and populate Text for 'save' button field
            //Empty Form name field and populate Text for 'save' button
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = customEntities[0].form[0]._saveButtonText;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion
            #region Empty Text for 'save' button field and populate Text for 'save and new' button field
            //Empty Text for 'save' button field and populate Text for 'save and new' button field
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = customEntities[0].form[0]._saveAndNewButtonText;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion
            #region Empty Text for 'save and new' button field and populate Text for 'save and stay' button field
            //Empty Text for 'save and new' button field and populate Text for 'save and stay' button field
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = customEntities[0].form[0]._saveAndStayButtonText;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion
            #region Empty Text for 'save and stay' button field and populate Text for 'cancel' button field
            //Empty Text for 'save and stay' button field and populate Text for 'cancel' button field
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = string.Empty;
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text = customEntities[0].form[0]._cancelButtonText;

            //Press Save 
            cCustomEntityFormsMethods.ClickSaveForm();

            //Validate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Expenses\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }

            //Close modal
            cCustomEntityFormsMethods.CloseMandatoryFieldsModal();

            //Validate red asterisks display next to the mandatory fields
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.FormNameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndNewButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.SaveAndStayButtonAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.CancelButtonAsterisk));
            #endregion

            //Close Forms modal
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text = customEntities[0].form[0]._formName;
            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }
        #endregion

        #region 38303 - Successfully Sort Forms Grid
        /// <summary>
        /// 38303 - Custom Entities : Successfully Sort Forms Grid
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullySortFormsGridToCustomEntity_UITest()
        {
            cSharedMethods.RestoreDefaultSortingOrder("gridForms", _executingProduct);

            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            //Sorts Forms table by Form Name column
            HtmlHyperlink displayNameLink = cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UITbl_gridFormsTable.UIFormNameHyperlink;
            cCustomEntityFormsMethods.ClickTableHeader(displayNameLink);
            cCustomEntityFormsMethods.VerifyCorrectSortingOrderForTable(SortFormsByColumn.FormName, EnumHelper.TableSortOrder.DESC, customEntities[0].entityId, _executingProduct);
            cCustomEntityFormsMethods.ClickTableHeader(displayNameLink);
            cCustomEntityFormsMethods.VerifyCorrectSortingOrderForTable(SortFormsByColumn.FormName, EnumHelper.TableSortOrder.ASC, customEntities[0].entityId, _executingProduct);

            //Sorts Forms table by Description column
            displayNameLink = cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UITbl_gridFormsTable.UIDescriptionHyperlink;
            cCustomEntityFormsMethods.ClickTableHeader(displayNameLink);
            cCustomEntityFormsMethods.VerifyCorrectSortingOrderForTable(SortFormsByColumn.Description, EnumHelper.TableSortOrder.ASC, customEntities[0].entityId, _executingProduct);
            displayNameLink = cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UITbl_gridFormsTable.UIDescriptionHyperlink;
            cCustomEntityFormsMethods.ClickTableHeader(displayNameLink);
            cCustomEntityFormsMethods.VerifyCorrectSortingOrderForTable(SortFormsByColumn.Description, EnumHelper.TableSortOrder.DESC, customEntities[0].entityId, _executingProduct);

            cSharedMethods.RestoreDefaultSortingOrder("gridForms", _executingProduct);
        }
        #endregion

        #region 39417 - Successfully verify maximum size for fields on forms modal
        /// <summary>
        ///  Successfully verify maximum size for fields on forms modal
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsSuccessfullyVerifyMaximumSizeOfFieldsOnFormsModal_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();
            cCustomEntityMethods.ClickNewFormLink();

            // CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);

            //Check max length
            Assert.AreEqual(100, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.MaxLength);
            // Assert.AreEqual(4000, ((cHtmlTextAreaWrapper)customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox).GetMaxLength());
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.MaxLength);
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.MaxLength);
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.MaxLength);
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.MaxLength);
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.MaxLength);

            //Copy and paste text - ensure truncation occurs
            Clipboard.Clear();
            try { Clipboard.SetText(Strings.BasicString); }
            catch (Exception) { }

            cCustomEntityFormsMethods.RightClickAndPaste(customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox);
            Assert.AreEqual(100, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.FormNameTextBox.Text.Length);
            Clipboard.Clear();

            try { Clipboard.SetText(Strings.BasicString); }
            catch (Exception) { }
            cCustomEntityFormsMethods.RightClickAndPaste(customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox);
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text.Length);
            Clipboard.Clear();

            try { Clipboard.SetText(Strings.BasicString); }
            catch (Exception) { }
            cCustomEntityFormsMethods.RightClickAndPaste(customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox);
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text.Length);
            Clipboard.Clear();

            try { Clipboard.SetText(Strings.BasicString); }
            catch (Exception) { }
            cCustomEntityFormsMethods.RightClickAndPaste(customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox);
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text.Length);
            Clipboard.Clear();

            try { Clipboard.SetText(Strings.BasicString); }
            catch (Exception) { }
            cCustomEntityFormsMethods.RightClickAndPaste(customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox);
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text.Length);
            Clipboard.Clear();

            try { Clipboard.SetText(Strings.BasicString); }
            catch (Exception) { }
            cCustomEntityFormsMethods.RightClickAndPaste(customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox);
            Assert.AreEqual(20, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandduplicTextBox.Text.Length);
            Clipboard.Clear();

            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }
            cCustomEntityFormsMethods.RightClickAndPaste(customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox);
            Mouse.Click();
            Assert.AreEqual(4000, customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.DescriptionTextBox.Text.Length);

        }
        #endregion

        #region 38093 - Succesfully verify page standards general details tab on forms modal
        /// <summary>
        /// 38093 - Succesfully verify page standards general details tab on forms modal
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsSuccessfullyVerifyPageStandardsGeneralDetailsTabOnFormsModal_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            //Click New Form Link
            cCustomEntityMethods.ClickNewFormLink();

            //Validate Field labels
            cCustomEntityFormsMethods.ValidateGeneralDetailsSectionName();
            cCustomEntityFormsMethods.ValidateFormNameField();
            cCustomEntityFormsMethods.ValidateFormNameIsMandatory();
            cCustomEntityFormsMethods.ValidateDescriptionField();
            cCustomEntityFormsMethods.ValidateShowSubMenuField();
            cCustomEntityFormsMethods.ValidateShowBreadcrumbsField();
            cCustomEntityFormsMethods.ValidateFormButtonsSectionName();
            cCustomEntityFormsMethods.ValidateCommentOnFormsModalExpectedValues.UIThefollowingcombinatPaneInnerText = cCustomEntityFormsMethods.ValidateCommentOnFormsModalExpectedValues.UIThefollowingcombinatPaneInnerText.Replace("four", "five");
            cCustomEntityFormsMethods.ValidateCommentOnFormsModal();
            cCustomEntityFormsMethods.ValidateSaveButtonTextField();
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextCancelButtonField();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatory();

            //Validate form button fields are not marked as mandatory when one of them is selected 
            // CustomEntityFormControls formControls = new CustomEntityFormControls(cCustomEntityFormsMethods);
            #region Populate save and validate all buttons aren't marked as mandatory
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = "save";
            Mouse.Click();
            cCustomEntityFormsMethods.ValidateSaveButtonTextFieldExpectedValues.UITextforsavebuttonLabelInnerText = "Text for \'save\' button";
            cCustomEntityFormsMethods.ValidateSaveButtonTextField();
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatoryExpectedValues.UITextforsavebuttonLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldExpectedValues.UITextforsaveandnewbutLabelInnerText = "Text for \'save and new\' button";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatoryExpectedValues.UITextforsaveandnewbutLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldExpectedValues.UITextforsaveandstaybuLabelInnerText = "Text for \'save and stay\' button";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatoryExpectedValues.UITextforsaveandstaybuLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldExpectedValues.UITextforcancelbuttonLabelInnerText = "Text for \'cancel\' button";
            cCustomEntityFormsMethods.ValidateTextCancelButtonField();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatoryExpectedValues.UITextforcancelbuttonLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatory();
            #endregion
            #region Populate save and new and validate all buttons aren't marked as mandatory
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsavebuttonTextBox.Text = string.Empty;
            Mouse.Click();
            cCustomEntityFormsMethods.ValidateSaveButtonTextFieldExpectedValues.UITextforsavebuttonLabelInnerText = "Text for \'save\' button*";
            cCustomEntityFormsMethods.ValidateSaveButtonTextField();
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatoryExpectedValues.UITextforsavebuttonLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldExpectedValues.UITextforsaveandnewbutLabelInnerText = "Text for \'save and new\' button*";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatoryExpectedValues.UITextforsaveandnewbutLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldExpectedValues.UITextforsaveandstaybuLabelInnerText = "Text for \'save and stay\' button*";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatoryExpectedValues.UITextforsaveandstaybuLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldExpectedValues.UITextforcancelbuttonLabelInnerText = "Text for \'cancel\' button*";
            cCustomEntityFormsMethods.ValidateTextCancelButtonField();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatoryExpectedValues.UITextforcancelbuttonLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatory();
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = "save and new";
            Mouse.Click();
            cCustomEntityFormsMethods.ValidateSaveButtonTextFieldExpectedValues.UITextforsavebuttonLabelInnerText = "Text for \'save\' button";
            cCustomEntityFormsMethods.ValidateSaveButtonTextField();
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatoryExpectedValues.UITextforsavebuttonLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldExpectedValues.UITextforsaveandnewbutLabelInnerText = "Text for \'save and new\' button";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatoryExpectedValues.UITextforsaveandnewbutLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldExpectedValues.UITextforsaveandstaybuLabelInnerText = "Text for \'save and stay\' button";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatoryExpectedValues.UITextforsaveandstaybuLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldExpectedValues.UITextforcancelbuttonLabelInnerText = "Text for \'cancel\' button";
            cCustomEntityFormsMethods.ValidateTextCancelButtonField();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatoryExpectedValues.UITextforcancelbuttonLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatory();
            #endregion
            #region Populate save and stay and validate all buttons aren't marked as mandatory
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandnewbutTextBox.Text = string.Empty;
            Mouse.Click();
            cCustomEntityFormsMethods.ValidateSaveButtonTextFieldExpectedValues.UITextforsavebuttonLabelInnerText = "Text for \'save\' button*";
            cCustomEntityFormsMethods.ValidateSaveButtonTextField();
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatoryExpectedValues.UITextforsavebuttonLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldExpectedValues.UITextforsaveandnewbutLabelInnerText = "Text for \'save and new\' button*";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatoryExpectedValues.UITextforsaveandnewbutLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldExpectedValues.UITextforsaveandstaybuLabelInnerText = "Text for \'save and stay\' button*";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatoryExpectedValues.UITextforsaveandstaybuLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldExpectedValues.UITextforcancelbuttonLabelInnerText = "Text for \'cancel\' button*";
            cCustomEntityFormsMethods.ValidateTextCancelButtonField();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatoryExpectedValues.UITextforcancelbuttonLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatory();
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = "save and stay";
            Mouse.Click();
            cCustomEntityFormsMethods.ValidateSaveButtonTextFieldExpectedValues.UITextforsavebuttonLabelInnerText = "Text for \'save\' button";
            cCustomEntityFormsMethods.ValidateSaveButtonTextField();
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatoryExpectedValues.UITextforsavebuttonLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldExpectedValues.UITextforsaveandnewbutLabelInnerText = "Text for \'save and new\' button";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatoryExpectedValues.UITextforsaveandnewbutLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldExpectedValues.UITextforsaveandstaybuLabelInnerText = "Text for \'save and stay\' button";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatoryExpectedValues.UITextforsaveandstaybuLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldExpectedValues.UITextforcancelbuttonLabelInnerText = "Text for \'cancel\' button";
            cCustomEntityFormsMethods.ValidateTextCancelButtonField();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatoryExpectedValues.UITextforcancelbuttonLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatory();
            #endregion
            #region Populate cancel and validate all buttons aren't marked as mandatory
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforsaveandstaybuTextBox.Text = string.Empty;
            Mouse.Click();
            cCustomEntityFormsMethods.ValidateSaveButtonTextFieldExpectedValues.UITextforsavebuttonLabelInnerText = "Text for \'save\' button*";
            cCustomEntityFormsMethods.ValidateSaveButtonTextField();
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatoryExpectedValues.UITextforsavebuttonLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldExpectedValues.UITextforsaveandnewbutLabelInnerText = "Text for \'save and new\' button*";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatoryExpectedValues.UITextforsaveandnewbutLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldExpectedValues.UITextforsaveandstaybuLabelInnerText = "Text for \'save and stay\' button*";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatoryExpectedValues.UITextforsaveandstaybuLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldExpectedValues.UITextforcancelbuttonLabelInnerText = "Text for \'cancel\' button*";
            cCustomEntityFormsMethods.ValidateTextCancelButtonField();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatoryExpectedValues.UITextforcancelbuttonLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatory();
            customEntityFormsNewMethods.CustomEntityFormControlsWindow.CustomEntityFormControlsDocument.TextforcancelbuttonTextBox.Text = "cancel";
            Mouse.Click();
            cCustomEntityFormsMethods.ValidateSaveButtonTextFieldExpectedValues.UITextforsavebuttonLabelInnerText = "Text for \'save\' button";
            cCustomEntityFormsMethods.ValidateSaveButtonTextField();
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatoryExpectedValues.UITextforsavebuttonLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldExpectedValues.UITextforsaveandnewbutLabelInnerText = "Text for \'save and new\' button";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatoryExpectedValues.UITextforsaveandnewbutLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldExpectedValues.UITextforsaveandstaybuLabelInnerText = "Text for \'save and stay\' button";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatoryExpectedValues.UITextforsaveandstaybuLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldExpectedValues.UITextforcancelbuttonLabelInnerText = "Text for \'cancel\' button";
            cCustomEntityFormsMethods.ValidateTextCancelButtonField();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatoryExpectedValues.UITextforcancelbuttonLabelClass = null;
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatory();
            #endregion

            //Validate buttons display on the modal
            HtmlInputButton saveButton = cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UISaveButton;
            string saveButtonExpectedText = "save";
            Assert.AreEqual(saveButtonExpectedText, saveButton.DisplayText);

            HtmlInputButton cancelButton = cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICancelButton;
            string cancelButtonExpectedText = "cancel";
            Assert.AreEqual(cancelButtonExpectedText, cancelButton.DisplayText);

            //Close forms modal
            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            //Click New Form Link
            cCustomEntityMethods.ClickNewFormLink();

            //Validate button labels are shown as mandatory
            cCustomEntityFormsMethods.ValidateSaveButtonTextFieldExpectedValues.UITextforsavebuttonLabelInnerText = "Text for \'save\' button*";
            cCustomEntityFormsMethods.ValidateSaveButtonTextField();
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatoryExpectedValues.UITextforsavebuttonLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldExpectedValues.UITextforsaveandnewbutLabelInnerText = "Text for \'save and new\' button*";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatoryExpectedValues.UITextforsaveandnewbutLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveAndNewButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldExpectedValues.UITextforsaveandstaybuLabelInnerText = "Text for \'save and stay\' button*";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonField();
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatoryExpectedValues.UITextforsaveandstaybuLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextSaveAndStayButtonFieldIsMandatory();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldExpectedValues.UITextforcancelbuttonLabelInnerText = "Text for \'cancel\' button*";
            cCustomEntityFormsMethods.ValidateTextCancelButtonField();
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatoryExpectedValues.UITextforcancelbuttonLabelClass = "mandatory";
            cCustomEntityFormsMethods.ValidateTextCancelButtonFieldIsMandatory();

            //Click on Form designer
            cCustomEntityFormsMethods.ClickFormDesign();

            //Ensure Info panel icon and tooltip is displayed
            cCustomEntityFormsMethods.VerifyShortcutTooltipExpectedValues.UIShowHotKeyInformatioImageAbsolutePath = "/static/icons/24/plain/information.png";
            cCustomEntityFormsMethods.VerifyShortcutTooltip();
            Mouse.Hover(cCustomEntityFormsMethods.UINewGreenLightWindowsWindow.UIGreenLightEXHIBITADocument.UIShowHotKeyInformatioImage);

            //Close forms modal
            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }
        #endregion

        #region 37935 - Successfully verify page standards on custom entity forms
        /// <summary>
        /// 37935 - Custom Entity Forms : Successfully verify page standards on custom entity forms 
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestCategory("Greenlight Form General Details"), TestMethod]
        public void CustomEntityFormsSuccessfullyValidatePageLayoutForFormsView_UITest()
        {
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            DateTime dt = DateTime.Now;
            string day = dt.ToString("dd");
            string monthName = dt.ToString("MMMM");
            string year = dt.ToString("yyyy");

            string currentTimeStr = day + " " + monthName + " " + year;

            if (_executingProduct == ProductType.expenses)
                cSharedMethods.VerifyPageLayout("GreenLight: " + customEntities[0].entityName, "Before you can continue, please confirm the action required at the bottom of your screen.", "Company PolicyHelp & SupportExit", "Mr James Lloyd | Developer | " + currentTimeStr, "Page Options GreenLight Details Attributes Forms Views Help");
            if (_executingProduct == ProductType.framework)
                cSharedMethods.VerifyPageLayout("GreenLight: " + customEntities[0].entityName, "Before you can continue, please confirm the action required at the bottom of your screen.", "About | Exit", "Mr James Lloyd | Developer | " + currentTimeStr, "Page Options GreenLight Details Attributes Forms Views Help");

            //Validate table is empty
            cCustomEntityFormsMethods.ValidateEmptyTable();

            //Ensure save button exists                                                                                                                                                                                                        
            HtmlInputButton saveButton = cCustomEntityMethods.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton;
            string saveButtonExpectedText = "save";
            Assert.AreEqual(saveButtonExpectedText, saveButton.DisplayText);

            //Ensure cancel button exists
            HtmlInputButton cancelButton = cCustomEntityMethods.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UICancelButton;
            string cancelButtonExpectedText = "cancel";
            Assert.AreEqual(cancelButtonExpectedText, cancelButton.DisplayText);
        }
        #endregion

        #region 38296 - Unsucessfully Add Tab On CustomEntity Form Where Mandatory Fields Are Missing
        /// <summary>
        /// 38296 - Unsucessfully add tab on custom entity form where mandatory fields are missing
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void UnsucessfullyAddTabOnCustomEntityFormWhereMandatoryFieldsAreMissing_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickNewTab();

            cCustomEntityFormsMethods.ClickSaveTab();

            cCustomEntityFormsMethods.ClickCloseOnValidationModalOfTab();

            cCustomEntityFormsMethods.ValidateAstrickIsPresentAfterValidationModalIsDisplayed();
        }
        #endregion

        #region 38297 - Unsucessfully add section on custom entity form where mandatory fields are missing ***
        /// <summary>
        /// 38297 - Unsucessfully add section on custom entity form where mandatory fields are missing
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsucessfullyAddSectionOnCustomEntityFormWhereMandatoryFieldsAreMissing_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickNewTab();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit tabNameControl = controlLocator.findControl("ctl00_contentmain_txttabheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));
            tabNameControl.Text = "Test";
            cCustomEntityFormsMethods.ClickSaveTab();
            cCustomEntityFormsMethods.ClickCogOnTab("Test");

            cCustomEntityFormsMethods.VerifyFormsContextMenuItems(new List<String>() { "Move left", "Move right", "Edit tab name", "New section", "Delete" }
             );

            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");
            tabNameControl = controlLocator.findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.VerifySectionNameIsMandatoryExpectedValues.UIDivMasterPopupPaneInnerText =
            string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Section name.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntityFormsMethods.VerifySectionNameIsMandatory();
            cCustomEntityFormsMethods.ClickCloseOnValidationModalOfSection();

            cCustomEntityFormsMethods.VerifySectionNameDisplaysAsterisk();

        }
        #endregion

        #region 38298 - Unsucessfully add tab on custom entity form where duplicate information is used
        ///
        ///
        ///
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsucessfullyAddTabOnCustomEntityFormWhereDuplicateInformationIsUsed_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickNewTab();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit tabNameControl = controlLocator.findControl("ctl00_contentmain_txttabheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));
            tabNameControl.Text = "Test";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ClickNewTab();
            tabNameControl.Text = "Test";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ValidateDuplicateTabModalExpectedValues.UIDivMasterPopupPaneInnerText =
            string.Format("Message from {0}\r\n\r\n\r\nCannot add a tab with this name as one already exists on this form.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntityFormsMethods.ValidateDuplicateTabModal();

        }

        #endregion

        #region 38299 - Unsucessfully add section on custom entity form where duplicate information is used***
        /// <summary>
        /// 
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormUnsucessfullyAddSectionOnCustomEntityFormWhereDuplicateInformationIsUsed_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickNewTab();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit tabNameControl = controlLocator.findControl("ctl00_contentmain_txttabheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));
            tabNameControl.Text = "Test";
            cCustomEntityFormsMethods.ClickSaveTab();
            cCustomEntityFormsMethods.ClickCogOnTab("Test");

            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");
            //cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");
            tabNameControl = controlLocator.findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));
            tabNameControl.Text = "section1";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ClickCogOnTab("Test");

            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");
            tabNameControl = controlLocator.findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));
            tabNameControl.Text = "section1";
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ValidateDuplicateSectionModalExpectedValues.UIDivMasterPopupPaneInnerText =
            string.Format("Message from {0}\r\n\r\n\r\nCannot add a section with this name as one already exists on this tab.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntityFormsMethods.ValidateDuplicateSectionModal();

        }
        #endregion

        #region 38301 - Sucessfully cancel adding tab on custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormSucessfullyCancelAddingTabOnCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickNewTab();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit tabNameControl = controlLocator.findControl("ctl00_contentmain_txttabheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));
            tabNameControl.Text = "Test";

            cCustomEntityFormsMethods.ClickCancelTab();


            cCustomEntityFormsMethods.VerifyCorrectTabsForForm(new List<String>() 
            { 
                customEntities[0].form[0].tabs[0]._headercaption, 
                customEntities[0].form[0].tabs[1]._headercaption, 
                "New Tab" 
            });
        }


        #endregion

        #region Sucessfully cancel editing tab on custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSucessfullyCancelEditingTabOnCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickNewTab();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit tabNameControl = controlLocator.findControl("ctl00_contentmain_txttabheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));
            tabNameControl.Text = "Test";
            cCustomEntityFormsMethods.ClickSaveTab();


            cCustomEntityFormsMethods.VerifyCorrectTabsForForm(new List<String>() 
            { 
                customEntities[0].form[0].tabs[0]._headercaption, 
                customEntities[0].form[0].tabs[1]._headercaption, 
                "Test",
                "New Tab" 
            });

            cCustomEntityFormsMethods.ClickCogOnTab("Test");
            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("Edit tab name");


            tabNameControl.Text = "Test_edited!";
            cCustomEntityFormsMethods.ClickCancelOnTabModal();

            // cCustomEntityFormsMethods.ClickEditIconOnTabContextMenu();

            cCustomEntityFormsMethods.VerifyCorrectTabsForForm(new List<String>() 
            { 
                customEntities[0].form[0].tabs[0]._headercaption, 
                customEntities[0].form[0].tabs[1]._headercaption, 
                "Test",
                "New Tab" 
            });


        }

        #endregion

        #region 39436 successfully add section with the same name in different tabs of same form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyAddSectionWithTheSameNameInDifferentTabsOfSameForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit SectionHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            //Verify modal message


            cCustomEntityFormsMethods.ValidateDuplicateSectionModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Section name.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntityFormsMethods.ValidateDuplicateSectionModal();
            cCustomEntityFormsMethods.ClickCloseOnValidationModalOfSection();
            cCustomEntityFormsMethods.VerifyMandatorySectionNameAsteriskIsDisplayed();

            SectionHeaderTxt.Text = customEntities[0].form[0].tabs[0].sections[0]._headercaption;

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[1]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");

            SectionHeaderTxt.Text = customEntities[0].form[0].tabs[0].sections[0]._headercaption;

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.VerifyCorrectSectionsAppearOnTab(customEntities[0].form[0].tabs[0]._headercaption, new List<string>() { customEntities[0].form[0].tabs[0].sections[0]._headercaption });

            cCustomEntityFormsMethods.VerifyCorrectSectionsAppearOnTab(customEntities[0].form[0].tabs[1]._headercaption, new List<string>() { customEntities[0].form[0].tabs[0].sections[0]._headercaption });
        }
        #endregion

        #region Copy Form
        #region 45102 - Successfully Create Copy of an existing Form
        /// <summary>
        /// 45102 - Successfully Create Copy of an existing Form
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyCreateCopyOfAnExistingForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            //Navigate to Greenlight Details page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //Select forms
            cCustomEntityMethods.ClickFormsLink();

            //Click Copy
            cCustomEntityFormsMethods.ClickCopyFormLink(customEntities[0].form[0]._formName);

            //Press Save the copy
            cCustomEntityFormsMethods.PressSaveCopy();

            //Validate Copy is added to the grid
            cCustomEntityFormsMethods.ValidateFormTable(cSharedMethods.browserWindow, customEntities[0].form[0]._formName + " (Copy)", customEntities[0].form[0]._description);
        }
        #endregion

        #region 45292 - Successfully Cancel Copying  an existing Form
        /// <summary>
        /// 45292 - Successfully Cancel Copying  an existing Form
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyCancelCopyingAnExistingForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            //Navigate to Greenlight Details page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //Select forms
            cCustomEntityMethods.ClickFormsLink();

            //Click Copy
            cCustomEntityFormsMethods.ClickCopyFormLink(customEntities[0].form[0]._formName);

            //Press Save the copy
            cCustomEntityFormsMethods.PressCancelCopy();

            //Validate Copy is added to the grid
            cCustomEntityFormsMethods.ValidateFormTable(cSharedMethods.browserWindow, customEntities[0].form[0]._formName + " (Copy)", customEntities[0].form[0]._description, false);
        }
        #endregion

        #region 45109 -  Unsuccessfully Copy an existing Form when name already exists
        /// <summary>
        /// 45109 -  Unsuccessfully Copy an existing Form when name already exists
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyCreateCopyOfAnExistingFormWhenDuplicateNameIsUsed_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            //Navigate to Greenlight Details page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //Select forms
            cCustomEntityMethods.ClickFormsLink();

            //Click Copy
            cCustomEntityFormsMethods.ClickCopyFormLink(customEntities[0].form[0]._formName);

            //Populate form name with the same details as an existing form
            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit formNameTxt = controlLocator.findControl("ctl00_contentmain_txtcopyformname", new HtmlEdit(cCustomEntityFormsMethods.UIGreenLightUKBATIER2MWindow1.UIGreenLightUKBATIER2MDocument));

            formNameTxt.Text = customEntities[0].form[0]._formName;

            //Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityFormsMethods.UIGreenLightUKBATIER2MWindow.UIGreenLightUKBATIER2MDocument.UISaveButton);

            //Validate duplicate modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateDuplicateDetailsModal();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateDuplicateDetailsModalExpectedValues.UIDivMasterPopupPaneInnerText = "Message from Framework\r\n\r\n\r\nThe Form name you have entered already exists.";
                cCustomEntityFormsMethods.ValidateDuplicateDetailsModal();
            }
            cCustomEntityFormsMethods.CloseDuplicateValidationModal();
        }
        #endregion

        #region 45290 -  Unsuccessfully Copy an existing Form when mandatory fields are missing
        /// <summary>
        /// 45290 -  Unsuccessfully Copy an existing Form when mandatory fields are missing
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyCreateCopyOfAnExistingFormWhenMandatoryFieldsAreMissing_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            //Navigate to Greenlight Details page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //Select forms
            cCustomEntityMethods.ClickFormsLink();

            //Click Copy
            cCustomEntityFormsMethods.ClickCopyFormLink(customEntities[0].form[0]._formName);

            //Validate Red Asterisk next to Form name is not shown
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UIGreenLightUKBATIER2MWindow1.UIGreenLightUKBATIER2MDocument.UIFormNameAsteriskInCopyModal));

            //Populate form name with the same details as an existing form
            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit formNameTxt = controlLocator.findControl("ctl00_contentmain_txtcopyformname", new HtmlEdit(cCustomEntityFormsMethods.UIGreenLightUKBATIER2MWindow1.UIGreenLightUKBATIER2MDocument));

            formNameTxt.Text = string.Empty;

            //Save
            Keyboard.SendKeys("{Enter}");

            //Validate mandatory fields modal
            if (_executingProduct == ProductType.expenses)
            {
                cCustomEntityFormsMethods.ValidateFormNameIsMandatory();
            }
            else
            {
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPane1DisplayText = "Message from Framework\r\n\r\n\r\nPlease enter a Form name.";
                cCustomEntityFormsMethods.ValidateMandatoryFieldsModal();
            }
            cCustomEntityFormsMethods.CloseDuplicateValidationModal();

            //Validate Red Asterisk next to Form name is shown
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cCustomEntityFormsMethods.UIGreenLightUKBATIER2MWindow1.UIGreenLightUKBATIER2MDocument.UIFormNameAsteriskInCopyModal));

            Keyboard.SendKeys("{Esc}");
        }
        #endregion

        #region 45316 -  Successfully verify maximum size of fields on Copy forms modal
        /// <summary>
        /// 45316 -  Successfully verify maximum size of fields on Copy forms modal
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyVerifyMaximumSizeOfFieldsOnCopyFormsModal_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            //Navigate to Greenlight Details page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //Select forms
            cCustomEntityMethods.ClickFormsLink();

            //Click Copy
            cCustomEntityFormsMethods.ClickCopyFormLink(customEntities[0].form[0]._formName);

            //Initialise the controls
            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit formNameTxt = controlLocator.findControl("ctl00_contentmain_txtcopyformname", new HtmlEdit(cCustomEntityFormsMethods.UIGreenLightUKBATIER2MWindow1.UIGreenLightUKBATIER2MDocument));

            formNameTxt.Text = string.Empty;

            //Verify maximum length property is applied
            Assert.AreEqual(100, formNameTxt.MaxLength);

            //Copy and paste text - ensure truncation occurs
            Clipboard.Clear();
            try { Clipboard.SetText(Strings.BasicString); }
            catch (Exception) { }

            cCustomEntityFormsMethods.RightClickAndPaste(formNameTxt);
            Assert.AreEqual(100, formNameTxt.Text.Length);
            Clipboard.Clear();

            Keyboard.SendKeys("{Esc}");
        }
        #endregion

        #region 45104 -  Successfully verify Copy form modal meets standards
        /// <summary>
        /// 45104 -  Successfully verify Copy form modal meets standards 
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyVerifyCopyFormsModalMeetsStandards_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            //Navigate to Greenlight Details page
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            //Select forms
            cCustomEntityMethods.ClickFormsLink();

            //Click Copy
            cCustomEntityFormsMethods.ClickCopyFormLink(customEntities[0].form[0]._formName);

            //Initialise the controls
            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit formNameTxt = controlLocator.findControl("ctl00_contentmain_txtcopyformname", new HtmlEdit(cCustomEntityFormsMethods.UIGreenLightUKBATIER2MWindow1.UIGreenLightUKBATIER2MDocument));

            Assert.IsTrue(formNameTxt.HasFocus, "Form name field does not have focus");

            cCustomEntityFormsMethods.ValidateFormNameField();

            //Validate buttons display on the modal
            HtmlInputButton saveButton = cCustomEntityFormsMethods.UIGreenLightUKBATIER2MWindow.UIGreenLightUKBATIER2MDocument.UISaveButton;
            string saveButtonExpectedText = "save";
            Assert.AreEqual(saveButtonExpectedText, saveButton.DisplayText);

            HtmlInputButton cancelButton = cCustomEntityFormsMethods.UIGreenLightUKBATIER2MWindow.UIGreenLightUKBATIER2MDocument.UICancelButton;
            string cancelButtonExpectedText = "cancel";
            Assert.AreEqual(cancelButtonExpectedText, cancelButton.DisplayText);

            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(saveButton.HasFocus, "Save button does not have focus");

            Keyboard.SendKeys("{Tab}");
            Assert.IsTrue(cancelButton.HasFocus, "Cancel button does not have focus");

            Keyboard.SendKeys("{Enter}");
        }
        #endregion
        #endregion

        #region Tab context menu
        #region 37915 Successfully Edit Form Tab Name in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyEditFormTabNameInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickEditTabName();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit TabHeaderTxt = controlLocator.findControl("ctl00_contentmain_txttabheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            TabHeaderTxt.Text = customEntities[0].form[0].tabs[1]._headercaption.ToString();

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            Keyboard.SendKeys(FormGlobalProperties.EDIT_TAB_KB_SHORTCUT, ModifierKeys.Control);

            Assert.AreEqual(TabHeaderTxt.Text, customEntities[0].form[0].tabs[1]._headercaption);

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }

        #endregion

        #region 38302 Unsuccessfully Edit Form Tab Name in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyEditFormTabNameInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickEditTabName();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit TabHeaderTxt = controlLocator.findControl("ctl00_contentmain_txttabheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            TabHeaderTxt.Text = customEntities[0].form[0].tabs[1]._headercaption.ToString();

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();
            Thread.Sleep(1000);

            Keyboard.SendKeys(FormGlobalProperties.EDIT_TAB_KB_SHORTCUT, ModifierKeys.Control);

            Assert.AreEqual(TabHeaderTxt.Text, customEntities[0].form[0].tabs[0]._headercaption);

            //Unsucessfully edit tab on custom entity form where mandatory fields are missing
            TabHeaderTxt.Text = string.Empty;
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ValidateDuplicateTabModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a Tab name.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });

            cCustomEntityFormsMethods.ValidateDuplicateTabModal();
            cCustomEntityFormsMethods.AssertTabNameAsteriskIsShown();
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }

        #endregion

        #region 39677 Unsuccessfully Edit Duplicate Form Tab Name in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyEditDuplicateFormTabNameInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickNewTab();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit TabHeaderTxt = controlLocator.findControl("ctl00_contentmain_txttabheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            TabHeaderTxt.Text = customEntities[0].form[0].tabs[0]._headercaption.ToString();

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ValidateDuplicateDetailsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nCannot add a tab with this name as one already exists on this form.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntityFormsMethods.ValidateDuplicateDetailsModal();
        }

        #endregion

        #region 37906 Successfully Delete Form Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDeleteFormTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickDeleteTab();

            cCustomEntityFormsMethods.ClickConfirmDeleteTab();

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = " New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();
        }
        #endregion

        #region 38309 Unsuccessfully Delete Form Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyDeleteFormTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickDeleteTab();

            cCustomEntityFormsMethods.ClickConfirmDeleteTab();

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[0]._headercaption + " New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();
        }
        #endregion

        #region Successfully Edit Form Tab Name and delete tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyEditFormTabNameAndDeleteTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption);

            cCustomEntityFormsMethods.ClickEditTabName();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit TabHeaderTxt = controlLocator.findControl("ctl00_contentmain_txttabheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            TabHeaderTxt.Text = customEntities[0].form[0].tabs[1]._headercaption.ToString();

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption);

            cCustomEntityFormsMethods.ClickDeleteTab();

            cCustomEntityFormsMethods.ClickConfirmDeleteTab();

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = " New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();

            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }

        #endregion

        #region Successfully Move Form Tab Left in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyMoveFormTabLeftInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[1]._headercaption);

            cCustomEntityFormsMethods.ClickMoveTabNameLeft();

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[1]._headercaption + customEntities[0].form[0].tabs[0]._headercaption + " " + "New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();
        }
        #endregion

        #region Successfully Move Form Tab Right in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyMoveFormTabRightInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickMoveTabNameRight();

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[1]._headercaption + customEntities[0].form[0].tabs[0]._headercaption + " " + "New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();
        }
        #endregion

        #region Unsuccessfully Move Form Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyMoveFormTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickMoveTabNameRight();

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[0]._headercaption + customEntities[0].form[0].tabs[1]._headercaption + " " + "New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();
        }
        #endregion

        #region 36581 Successfully Add Section To Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyAddSectionToTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");

            cCustomEntityFormsMethods.PopulateSectionNameTextBox(customEntities[0].form[0].tabs[0].sections[0]._headercaption);

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            //Add a new Tab to the form and esure no JS Error occurs
            cCustomEntityFormsMethods.ClickNewTab();

            cCustomEntityFormsMethods.PopulateTabNameTextBox("Hello");

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = customEntities[0].form[0].tabs[0].sections[0]._headercaption;

            cCustomEntityFormsMethods.ValidateSectionOrder();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[0]._headercaption + "Hello New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = customEntities[0].form[0].tabs[0].sections[0]._headercaption;

            cCustomEntityFormsMethods.ValidateSectionOrder();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[0]._headercaption + "Hello New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();
        }

        #endregion

        #region 38304 Unsuccessfully Add Section To Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyAddSectionToTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption.ToString());

            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit SectionHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            SectionHeaderTxt.Text = customEntities[0].form[0].tabs[0].sections[0]._headercaption;

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            //cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = customEntities[0].form[0].tabs[0].sections[0]._headercaption;

            //cCustomEntityFormsMethods.ValidateSectionOrder();
        }
        #endregion
        #endregion

        #region Section Context Menu
        #region 37907 Successfully Edit Form Section Name to Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyEditFormSectionNametoTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption));

            cCustomEntityFormsMethods.ClickEditSectionName();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit SectionHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            SectionHeaderTxt.Text = customEntities[0].form[0].tabs[0].sections[1]._headercaption.ToString();

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            //Does the context menu still work?

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption));

            cCustomEntityFormsMethods.VerifySectionContextMenu(new List<string>() { "Move up", "Move down", "Edit section name", "Delete" });
            cCustomEntityFormsMethods.ClickEditSectionName();

            Assert.AreEqual(SectionHeaderTxt.Text, customEntities[0].form[0].tabs[0].sections[1]._headercaption);

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            //Save the form and come back in to verify
            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption));

            cCustomEntityFormsMethods.VerifySectionContextMenu(new List<string>() { "Move up", "Move down", "Edit section name", "Delete" });
            cCustomEntityFormsMethods.ClickEditSectionName();

            Assert.AreEqual(SectionHeaderTxt.Text, customEntities[0].form[0].tabs[0].sections[1]._headercaption);

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

        }

        #endregion

        #region 38305 Unsuccessfully Edit Form Section Name to Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyEditFormSectionNametoTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption));

            cCustomEntityFormsMethods.ClickEditSectionName();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit SectionHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            SectionHeaderTxt.Text = customEntities[0].form[0].tabs[0].sections[1]._headercaption;

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            //Verify that the menu key works after updating

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption));
            cCustomEntityFormsMethods.VerifySectionContextMenu(new List<string>()
            {
                 "Move up", "Move down", "Edit section name", "Delete"
            });

            //Save form
            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption));

            cCustomEntityFormsMethods.ClickEditSectionName();

            Assert.AreEqual(SectionHeaderTxt.Text, customEntities[0].form[0].tabs[0].sections[0]._headercaption);

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }

        #endregion

        #region  39678 Unsuccessfully Edit Duplicate Form Section Name to Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyEditDuplicateFormSectionNametoTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption);

            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit SectionHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            SectionHeaderTxt.Text = customEntities[0].form[0].tabs[0].sections[0]._headercaption;

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ValidateDuplicateDetailsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nCannot add a section with this name as one already exists on this tab.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            cCustomEntityFormsMethods.ValidateDuplicateDetailsModal();
        }

        #endregion

        #region 37905 Successfully Delete Form Section from Tab in Custom Entity Form*_*
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDeleteFormSectionfromTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            HtmlImage sectionCogImage = GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption);
            cCustomEntityFormsMethods.ClickCogOnSection(sectionCogImage);

            cCustomEntityFormsMethods.ClickDeleteSection();

            cCustomEntityFormsMethods.ClickConfirmDeleteSection();

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.VerifyCorrectSectionsAppearOnTab(customEntities[0].form[0].tabs[0]._headercaption, new List<string>());
        }
        #endregion

        #region 38310 Unsuccessfully Delete Form Section from Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyDeleteFormSectionfromTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption));

            cCustomEntityFormsMethods.ClickDeleteSection();

            cCustomEntityFormsMethods.ClickConfirmDeleteSection();

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = customEntities[0].form[0].tabs[0].sections[0]._headercaption;

            cCustomEntityFormsMethods.ValidateSectionOrder();
        }
        #endregion

        #region Successfully Move Form Section Up in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyMoveFormSectionUpOnTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[1]._headercaption));

            cCustomEntityFormsMethods.ClickMoveSectionUp();

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = string.Format("{0}{1}{2}", customEntities[0].form[0].tabs[0].sections[1]._headercaption, Environment.NewLine, customEntities[0].form[0].tabs[0].sections[0]._headercaption);

            cCustomEntityFormsMethods.ValidateSectionOrder();
        }
        #endregion

        #region 38294 Successfully Move Form Section Down in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyMoveFormSectionDownOnTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption));

            cCustomEntityFormsMethods.ClickMoveSectionDown();

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = string.Format("{0}{1}{2}", customEntities[0].form[0].tabs[0].sections[1]._headercaption, Environment.NewLine, customEntities[0].form[0].tabs[0].sections[0]._headercaption);

            cCustomEntityFormsMethods.ValidateSectionOrder();
        }
        #endregion

        #region Unsuccessfully Move Form Section in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyMoveFormSectionOnTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption));

            cCustomEntityFormsMethods.ClickMoveSectionDown();

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = string.Format("{0}{1}{2}", customEntities[0].form[0].tabs[0].sections[0]._headercaption, Environment.NewLine, customEntities[0].form[0].tabs[0].sections[1]._headercaption);

            cCustomEntityFormsMethods.ValidateSectionOrder();
        }
        #endregion
        #endregion

        #region form fields context menu
        #region 39813 Successfully Edit Form Field Name in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyEditFormFieldNametoTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Edit label text");

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit FormFieldHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtfieldlabel", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            FormFieldHeaderTxt.Text = customEntities[0].attribute[18].DisplayName;

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Edit label Text");

            Assert.AreEqual(customEntities[0].attribute[18].DisplayName, FormFieldHeaderTxt.Text);

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }

        #endregion

        #region Unsuccessfully Edit Form Field Name in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyEditFormFieldNametoTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Edit label text");

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit FormFieldHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtfieldlabel", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            FormFieldHeaderTxt.Text = customEntities[0].attribute[18].DisplayName;

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Edit label text");

            FormFieldHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtfieldlabel", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            Assert.AreEqual(customEntities[0].attribute[1].DisplayName, FormFieldHeaderTxt.Text);

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);

            cCustomEntityFormsMethods.ClickCancelOnFormModal();
        }
        #endregion

        #region 36626 Successfully Delete Form Field in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDeleteFormFieldFromSectiontoTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Delete");

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = null;

            cCustomEntityFormsMethods.ValidateFormFieldOrder();
        }
        #endregion

        #region Unsuccessfully Delete Form Field in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyDeleteFormFieldFromSectiontoTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Delete");

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = "Spacer" + customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute.DisplayName;

            cCustomEntityFormsMethods.ValidateFormFieldOrder();
        }
        #endregion

        #region 39475 Successfully Erase Form Field Name in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyEraseFormFieldNameOnSectiontoTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Edit label text");

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit FormFieldHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtfieldlabel", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            FormFieldHeaderTxt.Text = customEntities[0].attribute[18].DisplayName;

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Remove label text");

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute.DisplayName + "*" + customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute.DisplayName;

            cCustomEntityFormsMethods.ValidateFormFieldOrder();
        }
        #endregion

        #region Unsuccessfully Erase Form Field Name in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyEraseFormFieldNameOnSectiontoTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Edit label text");

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit FormFieldHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtfieldlabel", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            FormFieldHeaderTxt.Text = customEntities[0].attribute[18].DisplayName;

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Remove label text");

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute.DisplayName + "*" + customEntities[0].attribute[18].DisplayName;

            cCustomEntityFormsMethods.ValidateFormFieldOrder();
        }
        #endregion

        #region 39521 Successfully Move Form Field Left in Section of Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyMoveFormFieldLeftinSectionOfTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Move left");

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute.DisplayName + customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute.DisplayName + "*";

            cCustomEntityFormsMethods.ValidateFormFieldOrder();

        }
        #endregion

        #region 39521 Successfully Move Form Field Right Section of Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyMoveFormFieldRightinSectionOfTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute._attributeid, "Move right");

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute.DisplayName + customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute.DisplayName + "*";

            cCustomEntityFormsMethods.ValidateFormFieldOrder();
        }
        #endregion

        #region Unsuccessfully Move Form Field in Section of Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyMoveFormFieldinSectionOfTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute._attributeid, "Move right");

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute.DisplayName + "*" + customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute.DisplayName;

            cCustomEntityFormsMethods.ValidateFormFieldOrder();
        }
        #endregion

        #region 38107 Successfully Make Form Field Readonly in Section of Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyMakeFormFieldReadonlyinSectionOfTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Read only");

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute.DisplayName + "*" + customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute.DisplayName + " (Read only)";

            cCustomEntityFormsMethods.ValidateFormFieldOrder();

        }
        #endregion

        #region Unsuccessfully Make Form Field Readonly in Section of Tab in Custom Entity Form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyMakeFormFieldReadonlyinSectionOfTabInCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute._attributeid, "Read only");

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute.DisplayName + "*" + customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute.DisplayName;

            cCustomEntityFormsMethods.ValidateFormFieldOrder();

        }
        #endregion

        #region 40165 - Unsucessfully drag and drop form field to custom entity form outside of the drop area
        /// <summary>
        /// 40165 -  Unsucessfully drag and drop Attributes to custom entity form outside of the drop area
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsucessfullyDragAndDropFormFieldToCustomEntityFormOutsideOfTheDropArea_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();

            cCustomEntityFormsMethods.ClickFormDesigner();
            ControlLocator<HtmlDiv> controlLocator = new ControlLocator<HtmlDiv>();
            HtmlDiv formDesignerPanel = controlLocator.findControl("ctl00_contentmain_tabConForms_body", new HtmlDiv(cSharedMethods.browserWindow));
            DragToDropField(customEntities[0].attribute[0].DisplayName, new Point(formDesignerPanel.BoundingRectangle.X - -100, 0));
            //  DragToDropField(customEntities[0].attribute[1].DisplayName, new Point(formDesignerPanel.BoundingRectangle.X - -100, 0));
            //  DragToDropField(customEntities[0].attribute[2].DisplayName, new Point(formDesignerPanel.BoundingRectangle.X - -100, 0));

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            //ensure they didnt get removed from Available fields
            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[0].DisplayName));
            //   Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[1].DisplayName));
            //    Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[2].DisplayName));
            Assert.IsFalse(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument9.UIAvailableFields.ControlDefinition.Contains("id=dialogDisabled"));

        }
        #endregion

        #region Default text

        #region Successfully add default text on Greenlight form field
        /// <summary>
        /// 49627 - Successfully add default text on Greenlight form field
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyAddDefaultTextOnFormField_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            foreach (var field in textFieldsSection.fields)
            {
                cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(field.attribute._attributeid));

                cCustomEntityFormsMethods.ClickIconOnFormField(field.attribute._attributeid, "Edit default field value");

                switch (field.attribute._format)
                {
                    case (short)Format.Single_line:
                    case (short)Format.SingleLineWide:
                        cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UIStandardTextDefaulttextEdit.Text = field._defaultValue;
                        break;
                    case (short)Format.Multi_line:
                        cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UITxtDefaultTextLargeEdit.Text = field._defaultValue;
                        break;
                    case (short)Format.FormattedTextBox:
                        cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.UIDefaultvalueforFormaEdit.Text = field._defaultValue;
                        break;
                    default:
                        break;
                }

                cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UISaveDefaultTextButton);
            }

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            foreach (var field in textFieldsSection.fields)
            {
                HtmlEdit formField = controlLocator.findControl(GetFormFieldID(field.attribute), new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

                if (field.attribute._format == (short)Format.FormattedTextBox)
                {
                    Assert.AreEqual(field._defaultValue, formField.InnerText);
                }
                else
                {
                    Assert.AreEqual(field._defaultValue, formField.Text);
                }
                cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(field.attribute._attributeid));

                cCustomEntityFormsMethods.ClickIconOnFormField(field.attribute._attributeid, "Edit default field value");

                switch (field.attribute._format)
                {
                    case (short)Format.Single_line:
                    case (short)Format.SingleLineWide:
                        Assert.AreEqual(field._defaultValue, cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UIStandardTextDefaulttextEdit.Text);
                        break;
                    case (short)Format.Multi_line:
                        Assert.AreEqual(field._defaultValue, cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UITxtDefaultTextLargeEdit.Text);
                        break;
                    case (short)Format.FormattedTextBox:
                        Assert.AreEqual(field._defaultValue, cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.UIDefaultvalueforFormaEdit.InnerText);
                        break;
                    default:
                        break;
                }

                cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UICancelDefaultTextButton);
            }
        }
        #endregion

        #region Successfully cancel adding default text on Greenlight form field
        /// <summary>
        /// 49625 - Successfully cancel adding default text on Greenlight form field
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyCancelAddingDefaultTextOnFormField_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(textFieldsSection.fields[0].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(textFieldsSection.fields[0].attribute._attributeid, "Edit default field value");

            switch (textFieldsSection.fields[0].attribute._format)
            {
                case (short)Format.Single_line:
                case (short)Format.SingleLineWide:
                    cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UIStandardTextDefaulttextEdit.Text = textFieldsSection.fields[0]._defaultValue;
                    break;
                case (short)Format.Multi_line:
                    cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UITxtDefaultTextLargeEdit.Text = textFieldsSection.fields[0]._defaultValue;
                    break;
                case (short)Format.FormattedTextBox:
                    cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.UIDefaultvalueforFormaEdit.Text = textFieldsSection.fields[0]._defaultValue;
                    break;
                default:
                    break;
            }

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UICancelDefaultTextButton);

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();

            HtmlEdit formField = controlLocator.findControl(GetFormFieldID(textFieldsSection.fields[0].attribute), new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            if (textFieldsSection.fields[0].attribute._format == (short)Format.FormattedTextBox)
            {
                Assert.AreEqual(string.Empty, formField.InnerText);
            }
            else
            {
                Assert.AreEqual(string.Empty, formField.Text);
            }
        }
        #endregion

        #region Successfully edit default text of Greenlight form field
        /// <summary>
        /// 49624 - Successfully edit default text of Greenlight form field
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyEditDefaultTextOnFormField_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            foreach (var field in textFieldsSection.fields)
            {
                cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(field.attribute._attributeid));

                cCustomEntityFormsMethods.ClickIconOnFormField(field.attribute._attributeid, "Edit default field value");

                switch (field.attribute._format)
                {
                    case (short)Format.Single_line:
                    case (short)Format.SingleLineWide:
                        cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UIStandardTextDefaulttextEdit.Text = field._defaultValue + " EDITED";
                        break;
                    case (short)Format.Multi_line:
                        cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UITxtDefaultTextLargeEdit.Text = field._defaultValue + " EDITED";
                        break;
                    case (short)Format.FormattedTextBox:
                        cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.UIDefaultvalueforFormaEdit.Text = field._defaultValue + " EDITED";
                        break;
                    default:
                        break;
                }

                cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UISaveDefaultTextButton);
            }

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            foreach (var field in textFieldsSection.fields)
            {
                HtmlEdit formField = controlLocator.findControl(GetFormFieldID(field.attribute), new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

                if (field.attribute._format == (short)Format.FormattedTextBox)
                {
                    Assert.AreEqual("EDITED " + field._defaultValue, formField.InnerText);
                }
                else
                {
                    Assert.AreEqual(field._defaultValue + " EDITED", formField.Text);
                }
                cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(field.attribute._attributeid));

                cCustomEntityFormsMethods.ClickIconOnFormField(field.attribute._attributeid, "Edit default field value");

                switch (field.attribute._format)
                {
                    case (short)Format.Single_line:
                    case (short)Format.SingleLineWide:
                        Assert.AreEqual(field._defaultValue + " EDITED", cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UIStandardTextDefaulttextEdit.Text);
                        break;
                    case (short)Format.Multi_line:
                        Assert.AreEqual(field._defaultValue + " EDITED", cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UITxtDefaultTextLargeEdit.Text);
                        break;
                    case (short)Format.FormattedTextBox:
                        Assert.AreEqual(field._defaultValue + " EDITED", cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.UIDefaultvalueforFormaEdit.InnerText);
                        break;
                    default:
                        break;
                }

                cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UICancelDefaultTextButton);
            }
        }
        #endregion

        #region Successfully cancel editing default text of Greenlight form field
        /// <summary>
        /// 49628 - Successfully cancel editing default text on Greenlight form field
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyCancelEditingDefaultTextOnFormField_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(textFieldsSection.fields[0].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(textFieldsSection.fields[0].attribute._attributeid, "Edit default field value");

            switch (textFieldsSection.fields[0].attribute._format)
            {
                case (short)Format.Single_line:
                case (short)Format.SingleLineWide:
                    cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UIStandardTextDefaulttextEdit.Text = textFieldsSection.fields[0]._defaultValue + " EDITED";
                    break;
                case (short)Format.Multi_line:
                    cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UITxtDefaultTextLargeEdit.Text = textFieldsSection.fields[0]._defaultValue + " EDITED";
                    break;
                case (short)Format.FormattedTextBox:
                    cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.UIDefaultvalueforFormaEdit.Text = textFieldsSection.fields[0]._defaultValue + " EDITED";
                    break;
                default:
                    break;
            }

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UICancelDefaultTextButton);

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();

            HtmlEdit formField = controlLocator.findControl(GetFormFieldID(textFieldsSection.fields[0].attribute), new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            if (textFieldsSection.fields[0].attribute._format == (short)Format.FormattedTextBox)
            {
                Assert.AreEqual(textFieldsSection.fields[0]._defaultValue, formField.InnerText);
            }
            else
            {
                Assert.AreEqual(textFieldsSection.fields[0]._defaultValue, formField.Text);
            }
        }
        #endregion

        #region Successfully copy form where default text on Greenlight form field is set
        /// <summary>
        /// 49629 - Successfully copy form where default text on Greenlight form field is set
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyCopyFormWhereDefaultTextOnFormFieldIsSet_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickCopyFormLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.PressSaveCopy();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName + " (Copy)");

            cCustomEntityFormsMethods.ClickFormDesign();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            foreach (var field in textFieldsSection.fields)
            {
                HtmlEdit formField = controlLocator.findControl(GetFormFieldID(field.attribute), new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

                if (field.attribute._format == (short)Format.FormattedTextBox)
                {
                    Assert.AreEqual(field._defaultValue, formField.InnerText);
                }
                else
                {
                    Assert.AreEqual(field._defaultValue, formField.Text);
                }
            }
        }
        #endregion

        #region  Successfully verify Greenlight form field maintains its default text when is removed and re-added to the form
        /// <summary>
        /// 49657 -  Successfully verify Greenlight form field maintains its default text when is removed and re-added to the form
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyFormFieldDefaultTextMaintainItsValueWhenReAddedToTheForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(textFieldsSection.fields[0].attribute._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(textFieldsSection.fields[0].attribute._attributeid, "Edit default field value");

            switch (textFieldsSection.fields[0].attribute._format)
            {
                case (short)Format.Single_line:
                case (short)Format.SingleLineWide:
                    cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UIStandardTextDefaulttextEdit.Text = textFieldsSection.fields[0]._defaultValue;
                    break;
                case (short)Format.Multi_line:
                    cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UITxtDefaultTextLargeEdit.Text = textFieldsSection.fields[0]._defaultValue;
                    break;
                case (short)Format.FormattedTextBox:
                    cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow3.UIGreenLightCustomEntiDocument.UIDefaultvalueforFormaEdit.Text = textFieldsSection.fields[0]._defaultValue;
                    break;
                default:
                    break;
            }

            cSharedMethods.SetFocusOnControlAndPressEnter(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument16.UISaveDefaultTextButton);

            cCustomEntityFormsMethods.ClickIconOnFormField(textFieldsSection.fields[0].attribute._attributeid, "Delete");

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();

            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();

            DragToDropField(textFieldsSection.fields[0].attribute.DisplayName, new Point(-300, 0));

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();

            HtmlEdit formField = controlLocator.findControl(GetFormFieldID(textFieldsSection.fields[0].attribute), new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            if (textFieldsSection.fields[0].attribute._format == (short)Format.FormattedTextBox)
            {
                Assert.AreEqual(textFieldsSection.fields[0]._defaultValue, formField.InnerText);
            }
            else
            {
                Assert.AreEqual(textFieldsSection.fields[0]._defaultValue, formField.Text);
            }
        }
        #endregion

        #endregion

        #endregion

        #region form fields drag and drop
        #region 36582 Successfully drag and drop form field on to custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDragandDropFormFieldOnToCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();

            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[0].DisplayName));
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[1].DisplayName));
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[2].DisplayName));

            DragToDropField(customEntities[0].attribute[0].DisplayName, new Point(-300, 0));
            DragToDropField(customEntities[0].attribute[1].DisplayName, new Point(-300, 0));
            DragToDropField(customEntities[0].attribute[2].DisplayName, new Point(-300, 0));

            Thread.Sleep(1000);
            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[0].DisplayName));
            Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[1].DisplayName));
            Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[2].DisplayName));
            //Ensure attributes that have been dragged no longer show on available fields 
            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = customEntities[0].attribute[0].DisplayName + "*" + customEntities[0].attribute[1].DisplayName + customEntities[0].attribute[2].DisplayName;
            cCustomEntityFormsMethods.ValidateFormFieldOrder();

        }
        #endregion

        #region Successfully drag and drop form field from first to second section on to custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDragandDropFormFieldFromFirstToSecondSectionOnToCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            DragToReplaceField(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute._attributeid), GenerateFormFieldLabelIdFromAttributeId(customEntities[0].form[0].tabs[0].sections[1].fields[0].attribute._attributeid));

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = string.Format("{0}{1}{2}Spacer{3}{4}*", customEntities[0].form[0].tabs[0].sections[0]._headercaption, Environment.NewLine, customEntities[0].form[0].tabs[0].sections[1]._headercaption, customEntities[0].form[0].tabs[0].sections[1].fields[0].attribute.DisplayName, customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute.DisplayName);

            cCustomEntityFormsMethods.ValidateSectionOrder();

        }
        #endregion

        #region 39768 Successfully drag and drop form field to Different Sections on to custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDragandDropFormFieldOnToDifferentSectionsOnCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();

            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[0].DisplayName));
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[1].DisplayName));
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[2].DisplayName));

            DragToDropField(customEntities[0].attribute[0].DisplayName, new Point(-300, -230));
            DragToDropField(customEntities[0].attribute[1].DisplayName, new Point(-300, 0));
            DragToDropField(customEntities[0].attribute[2].DisplayName, new Point(-300, 0));

            Thread.Sleep(1000);
            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[0].DisplayName));
            Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[1].DisplayName));
            Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[2].DisplayName));
            //Ensure attributes that have been dragged no longer show on available fields 
            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = string.Format("{0}{1}*{2}{3}{4}{5}", customEntities[0].form[0].tabs[0].sections[0]._headercaption, customEntities[0].attribute[0].DisplayName, Environment.NewLine, customEntities[0].form[0].tabs[0].sections[1]._headercaption, customEntities[0].attribute[1].DisplayName, customEntities[0].attribute[2].DisplayName);
            cCustomEntityFormsMethods.ValidateSectionOrder();
        }
        #endregion

        #region Unsuccessfully drag and drop form field on to custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsUnsuccessfullyDragandDropFormFieldOnToCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();

            DragToDropField(customEntities[0].attribute[0].DisplayName, new Point(-300, 0));
            DragToDropField(customEntities[0].attribute[1].DisplayName, new Point(-300, 0));
            DragToDropField(customEntities[0].attribute[2].DisplayName, new Point(-300, 0));

            cCustomEntityFormsMethods.ClickCancelOnFormModal();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = "Section 1";
            cCustomEntityFormsMethods.ValidateSectionOrder();

        }
        #endregion

        #region 39602 Successfully drag and drop N to One form field on to custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDragandDropNtoOneFormFieldOnToCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();

            DragToDropField(customEntities[0].attribute[33].DisplayName, new Point(-300, 0));

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            string attributeLabelTxt = string.Empty;
            if (customEntities[0].attribute[33].DisplayName.Length > 30)
            {
                attributeLabelTxt = customEntities[0].attribute[33].DisplayName.Substring(0, 30) + "...";
            }
            else
            {
                attributeLabelTxt = customEntities[0].attribute[33].DisplayName;
            }

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = attributeLabelTxt;


            cCustomEntityFormsMethods.ValidateFormFieldOrder();

        }
        #endregion

        #region 39601 Successfully drag and drop One to n form field on to custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDragandDropOnetoNFormFieldOnToCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();

            DragToDropField(customEntities[0].attribute[37].DisplayName, new Point(-300, 0));

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = "One To Many Column 1 One To Many Column 2One To Many Column 3One To Many Column 4One To Many Column 5\r\n1 : N Relationship to Custom E... information will be displayed here.";


            cCustomEntityFormsMethods.ValidateFormFieldOrder();

        }
        #endregion

        #region 40140 Successfully drag and drop form field after spacer on to custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDragandDropFormFieldAfterSpacerOnToCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();

            DragToDropField("Spacer", new Point(-300, 0));
            DragToDropField(customEntities[0].attribute[0].DisplayName, new Point(-300, 0));

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateFormFieldOrderExpectedValues.UIStandardSingleTextMuPaneInnerText = "Spacer" + customEntities[0].attribute[0].DisplayName + "*";
            cCustomEntityFormsMethods.ValidateFormFieldOrder();

        }
        #endregion

        #region Successfully remove form attributes via drag and dropping onto available fields
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyRemoveFormAttributesViaDragAndDropToAvailableFields_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();

            HtmlDiv availableFields = cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument9.UIAvailableFields;

            foreach (Auto_Tests.Tools.CustomEntitiesUtilities.CustomEntityFormField fields in customEntities[0].form[0].tabs[0].sections[0].fields)
            {

                ControlLocator<HtmlDiv> controlLocator = new ControlLocator<HtmlDiv>();
                HtmlDiv formField = controlLocator.findControl(GenerateFormFieldLabelIdFromAttributeId(fields.attribute._attributeid), new HtmlDiv(cSharedMethods.browserWindow));

                formField.EnsureClickable();
                Mouse.MouseDragSpeed = 600;
                Mouse.StartDragging(formField);
                Mouse.StopDragging(availableFields);
            }

            //Assert available fields dialog contains the correct number of elements
            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            foreach (Auto_Tests.Tools.CustomEntitiesUtilities.CustomEntityFormField fields in customEntities[0].form[0].tabs[0].sections[0].fields)
            {
                Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(fields.attribute.DisplayName));
            }

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            foreach (Auto_Tests.Tools.CustomEntitiesUtilities.CustomEntityFormField fields in customEntities[0].form[0].tabs[0].sections[0].fields)
            {
                Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(fields.attribute.DisplayName));
            }
        }
        #endregion
        #endregion

        #region section drag and drop
        #region Successfully drag and drop section on to custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDragandDropSectionInTabOnToCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            DragToReplaceSection("sectiontitle", customEntities[0].form[0].tabs[0].sections[1]._headercaption, customEntities[0].form[0].tabs[0].sections[0]._headercaption);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = string.Format("{0}Spacer{1}{2}{3}{4}*", customEntities[0].form[0].tabs[0].sections[1]._headercaption, customEntities[0].form[0].tabs[0].sections[1].fields[0].attribute.DisplayName, Environment.NewLine, customEntities[0].form[0].tabs[0].sections[0]._headercaption, customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute.DisplayName);

            cCustomEntityFormsMethods.ValidateSectionOrder();
        }
        #endregion

        #region Custom Entities : Successfully move section between tabs in form designer
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyMoveSectionBetweenTabsInFormDesigner_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            //Create a new section and then cancel - this covers bug
            //"open and cancel creating a new section on a tab with sections then attempt to move an existing section to the next tab"
            #region - 40860 open and cancel creating a new section on a tab with sections then attempt to move an existing section to the next tab
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_SECTION_KB_SHORTCUT, ModifierKeys.Control);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            #endregion

            //Click section in tab1 and move to tab2
            SectionControlLocator<HtmlDiv> controlLocator2 = new SectionControlLocator<HtmlDiv>();
            HtmlDiv sourceSection = (HtmlDiv)controlLocator2.findControl("sectiontitle", customEntities[0].form[0].tabs[0].sections[1]._headercaption, new HtmlDiv(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));

            TabControlLocator<HtmlSpan> controlLocator3 = new TabControlLocator<HtmlSpan>();
            HtmlSpan sourceTab = (HtmlSpan)controlLocator3.findControl("sm_tabheader_middle", customEntities[0].form[0].tabs[1]._headercaption, new HtmlSpan(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));

            DragAndDropControl(sourceSection, sourceTab);

            //Assert whether the drag and drap was successful!
            cCustomEntityFormsMethods.VerifyCorrectSectionsAppearOnTab(customEntities[0].form[0].tabs[0]._headercaption, new List<string>()
            { 
                customEntities[0].form[0].tabs[0].sections[0]._headercaption
            });

            cCustomEntityFormsMethods.VerifyCorrectSectionsAppearOnTab(customEntities[0].form[0].tabs[1]._headercaption, new List<string>()
            { 
                customEntities[0].form[0].tabs[0].sections[1]._headercaption 
            });

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.VerifyCorrectSectionsAppearOnTab(customEntities[0].form[0].tabs[0]._headercaption, new List<string>()
            { 
                customEntities[0].form[0].tabs[0].sections[0]._headercaption
            });

            cCustomEntityFormsMethods.VerifyCorrectSectionsAppearOnTab(customEntities[0].form[0].tabs[1]._headercaption, new List<string>()
            { 
                customEntities[0].form[0].tabs[0].sections[1]._headercaption 
            });

        }
        #endregion

        #region CustomEntities : Successfully move section with same name between tabs
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyMoveSectionWithSameNameBetweenTabsInFormDesigner_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection("Tab 1", customEntities[0].form[0].tabs[0].sections[0]._headercaption));
            cCustomEntityFormsMethods.ClickEditSectionName();

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit SectionHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtsectionheader", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            SectionHeaderTxt.Text = customEntities[0].form[0].tabs[1].sections[0]._headercaption.ToString();

            Keyboard.SendKeys(FormGlobalProperties.ENTER);


            //Click section in tab1 and move to tab2
            SectionControlLocator<HtmlDiv> controlLocator2 = new SectionControlLocator<HtmlDiv>();
            HtmlDiv sourceSection = (HtmlDiv)controlLocator2.findControl("sectiontitle", customEntities[0].form[0].tabs[1].sections[0]._headercaption, new HtmlDiv(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));

            TabControlLocator<HtmlSpan> controlLocator3 = new TabControlLocator<HtmlSpan>();
            HtmlSpan sourceTab = (HtmlSpan)controlLocator3.findControl("sm_tabheader_middle", customEntities[0].form[0].tabs[1]._headercaption, new HtmlSpan(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));

            DragAndDropControl(sourceSection, sourceTab);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.VerifyCorrectSectionsAppearOnTab(customEntities[0].form[0].tabs[0]._headercaption, new List<string>()
            { 
                customEntities[0].form[0].tabs[0].sections[1]._headercaption
            });

            cCustomEntityFormsMethods.VerifyCorrectSectionsAppearOnTab(customEntities[0].form[0].tabs[1]._headercaption, new List<string>()
            { 
                customEntities[0].form[0].tabs[1].sections[0]._headercaption,
                customEntities[0].form[0].tabs[1].sections[1]._headercaption, 
                customEntities[0].form[0].tabs[1].sections[0]._headercaption + " [1]" 
            });
        }
        #endregion

        #endregion

        #region drag and drop tab
        #region Successfully drag and drop Tab on to custom entity form
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyDragandDropTabInTabOnToCustomEntityForm_UITest()
        {
            string test = testContextInstance.TestName;

            ImportFormDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);
            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            UITestControl firstTab = cCustomEntityFormsMethods.GetTabControlByTabName(customEntities[0].form[0].tabs[1]._headercaption).GetChildren()[2];
            UITestControl secondTab = cCustomEntityFormsMethods.GetTabControlByTabName(customEntities[0].form[0].tabs[0]._headercaption).GetChildren()[1];
            DragAndDropControl(secondTab, firstTab);

            cCustomEntityFormsMethods.ClickSaveForm();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesigner();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[1]._headercaption + customEntities[0].form[0].tabs[0]._headercaption + " New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();

            //Add a new tab to the designer and move it
            cCustomEntityFormsMethods.ClickNewTab();

            cCustomEntityFormsMethods.PopulateTabNameTextBox("Test");

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[1]._headercaption + customEntities[0].form[0].tabs[0]._headercaption + "Test New Tab ";

            cCustomEntityFormsMethods.ValidateTabOrder();

            firstTab = cCustomEntityFormsMethods.GetTabControlByTabName("Test").GetChildren()[2];
            secondTab = cCustomEntityFormsMethods.GetTabControlByTabName(customEntities[0].form[0].tabs[0]._headercaption).GetChildren()[1];
            DragAndDropControl(secondTab, firstTab);

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[1]._headercaption + "Test" + customEntities[0].form[0].tabs[0]._headercaption + " New Tab ";
            cCustomEntityFormsMethods.ValidateTabOrder();

            firstTab = cCustomEntityFormsMethods.GetTabControlByTabName("Test").GetChildren()[2];
            secondTab = cCustomEntityFormsMethods.GetTabControlByTabName(customEntities[0].form[0].tabs[1]._headercaption).GetChildren()[1];
            DragAndDropControl(secondTab, firstTab);

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = "Test" + customEntities[0].form[0].tabs[1]._headercaption + customEntities[0].form[0].tabs[0]._headercaption + " New Tab ";
            cCustomEntityFormsMethods.ValidateTabOrder();

            //Ensure menu shows
            cCustomEntityFormsMethods.ClickCogOnTab("Test");
            cCustomEntityFormsMethods.ClickMoveTabNameLeft();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[0]._headercaption);
            cCustomEntityFormsMethods.ClickMoveTabNameLeft();

            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[1]._headercaption);
            cCustomEntityFormsMethods.ClickMoveTabNameLeft();

            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = "Test" + customEntities[0].form[0].tabs[1]._headercaption + customEntities[0].form[0].tabs[0]._headercaption + " New Tab ";
            cCustomEntityFormsMethods.ValidateTabOrder();

        }
        #endregion
        #endregion

        #region  Successfully verify that correct menu items appear for tab and their titles are present when hovering
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccesfullyVerifyThatCorrectContextMenuAppearsForFormFields_UITest()
        {

            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            foreach (var sectionsOnTab in customEntities[0].form[0].tabs[0].sections)
            {
                foreach (var formfields in sectionsOnTab.fields)
                {
                    if (formfields.attribute._attributeid != 0)
                    {
                        cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(formfields.attribute._attributeid));
                        cCustomEntityFormsMethods.AssertContextMenu(formfields.attribute._fieldType, formfields.attribute._attributeid);
                    }
                }
            }
        }
        #endregion

        #region Successfully verify Keyboard shortcuts tests
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyVerifyKeyboardShortCutsInForms_UITest()
        {
            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            //Add new tab using keyboard
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_TAB_KB_SHORTCUT, ModifierKeys.Control);
            cCustomEntityFormsMethods.PopulateTabNameTextBox(customEntities[0].form[0].tabs[0]._headercaption);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            //            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = customEntities[0].form[0].tabs[0]._headercaption + " New Tab";
            cCustomEntityFormsMethods.ValidateTabOrderExpectedValues.UITab1Tab2NewTabPaneInnerText = string.Format("{0} New Tab ", customEntities[0].form[0].tabs[0]._headercaption);

            cCustomEntityFormsMethods.ValidateTabOrder();

            //Add new section using keyboard
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_SECTION_KB_SHORTCUT, ModifierKeys.Control);
            cCustomEntityFormsMethods.PopulateSectionNameTextBox(customEntities[0].form[0].tabs[0].sections[0]._headercaption);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            //            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = "Section 1\r\nTestSection";

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = string.Format("{0}", customEntities[0].form[0].tabs[0].sections[0]._headercaption);

            cCustomEntityFormsMethods.ValidateSectionOrder();


            //Edit a tab
            Keyboard.SendKeys(FormGlobalProperties.EDIT_TAB_KB_SHORTCUT, ModifierKeys.Control);
            cCustomEntityFormsMethods.PopulateTabNameTextBox(customEntities[0].form[0].tabs[1]._headercaption);
            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            //Dock the Available fields
            //Get the current H / W of Available fields window when not docked...
            UITestControl htmlWindow = cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow;
            HtmlDiv htmlDocument = cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument8.UICtl00_contentmain_moPane;
            HtmlDiv availableFieldsDlg = cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7.UIAvailableFieldsclosePane;
            int heightOfAvailableFieldsdlg = availableFieldsDlg.BoundingRectangle.Height;
            int widthOfAvailableFieldsdlg = availableFieldsDlg.BoundingRectangle.Width;
            int oldXPosition = availableFieldsDlg.BoundingRectangle.X;

            //DOC FIELD LEFT ....
            Keyboard.SendKeys(FormGlobalProperties.DOCK_UNDOCK_AVAILABLE_FIELDS_LEFT_KB_SHORTCUT, ModifierKeys.Control);
            Thread.Sleep(2000);
            int heightOfAvailableFieldsdlgExpanded = availableFieldsDlg.BoundingRectangle.Height;
            int widthOfAvailableFieldsdlgExpanded = availableFieldsDlg.BoundingRectangle.Width;
            Assert.AreEqual(htmlDocument.BoundingRectangle.Y, availableFieldsDlg.BoundingRectangle.Y);
            Assert.AreEqual(widthOfAvailableFieldsdlg, widthOfAvailableFieldsdlgExpanded);
            Assert.IsTrue(heightOfAvailableFieldsdlg < heightOfAvailableFieldsdlgExpanded);
            Assert.IsTrue(heightOfAvailableFieldsdlgExpanded.Equals(htmlDocument.BoundingRectangle.Height));

            Keyboard.SendKeys("{5}", ModifierKeys.Control);
            Thread.Sleep(2000);

            Assert.AreEqual(widthOfAvailableFieldsdlg, widthOfAvailableFieldsdlgExpanded);
            Assert.IsTrue(availableFieldsDlg.BoundingRectangle.Height <= htmlDocument.BoundingRectangle.Height);
            Assert.AreEqual(oldXPosition, availableFieldsDlg.BoundingRectangle.X);

            //DOCK FIELDS RIGHT
            Keyboard.SendKeys(FormGlobalProperties.DOCK_UNDOCK_AVAILABLE_FIELDS_RIGHT_KB_SHORTCUT, ModifierKeys.Control);

            Assert.AreEqual(htmlDocument.BoundingRectangle.Y, availableFieldsDlg.BoundingRectangle.Y);
            Assert.AreEqual(widthOfAvailableFieldsdlg, widthOfAvailableFieldsdlgExpanded);
            Assert.IsTrue(heightOfAvailableFieldsdlg < heightOfAvailableFieldsdlgExpanded);
            Assert.IsTrue(heightOfAvailableFieldsdlgExpanded.Equals(htmlDocument.BoundingRectangle.Height));

            Keyboard.SendKeys(FormGlobalProperties.DOCK_UNDOCK_AVAILABLE_FIELDS_RIGHT_KB_SHORTCUT, ModifierKeys.Control);
            Assert.AreEqual(widthOfAvailableFieldsdlg, widthOfAvailableFieldsdlgExpanded);
            Assert.IsTrue(availableFieldsDlg.BoundingRectangle.Height <= htmlDocument.BoundingRectangle.Height);

            #region Tests to ensure that only one modal dialog can be shown at once!

            //Test to ensure that Editing tab dialog only appears and new section doesn't
            Keyboard.SendKeys(FormGlobalProperties.EDIT_TAB_KB_SHORTCUT, ModifierKeys.Control);
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_SECTION_KB_SHORTCUT, ModifierKeys.Control);

            Assert.IsTrue(cCustomEntityFormsMethods.HtmlTabControlEditText.HasFocus);
            Assert.IsFalse(cCustomEntityFormsMethods.HtmlSectionControlEditText.HasFocus);

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Assert.IsFalse(cCustomEntityFormsMethods.HtmlTabControlEditText.HasFocus);
            Assert.IsFalse(cCustomEntityFormsMethods.HtmlSectionControlEditText.HasFocus);

            //Test to ensure that new Section dialog only appears and edit tab doesn't
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_SECTION_KB_SHORTCUT, ModifierKeys.Control);
            Keyboard.SendKeys(FormGlobalProperties.EDIT_TAB_KB_SHORTCUT, ModifierKeys.Control);


            Assert.IsFalse(cCustomEntityFormsMethods.HtmlTabControlEditText.HasFocus);

            Assert.IsTrue(cCustomEntityFormsMethods.HtmlSectionControlEditText.HasFocus);

            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Assert.IsFalse(cCustomEntityFormsMethods.HtmlTabControlEditText.HasFocus);
            Assert.IsFalse((cCustomEntityFormsMethods.HtmlSectionControlEditText.HasFocus));

            //Test to ensure that new Section dialog only appears and new tab doesn't
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_SECTION_KB_SHORTCUT, ModifierKeys.Control);
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_TAB_KB_SHORTCUT, ModifierKeys.Control);


            Assert.IsFalse(cCustomEntityFormsMethods.HtmlTabControlEditText.HasFocus);

            Assert.IsTrue((cCustomEntityFormsMethods.HtmlSectionControlEditText.HasFocus));
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Assert.IsFalse(cCustomEntityFormsMethods.HtmlTabControlEditText.HasFocus);
            Assert.IsFalse(cCustomEntityFormsMethods.HtmlSectionControlEditText.HasFocus);


            //Test to ensure that new tab dialog only appears and new section doesn't
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_TAB_KB_SHORTCUT, ModifierKeys.Control);
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_SECTION_KB_SHORTCUT, ModifierKeys.Control);

            Assert.IsTrue(cCustomEntityFormsMethods.HtmlTabControlEditText.HasFocus);

            Assert.IsFalse(cCustomEntityFormsMethods.HtmlSectionControlEditText.HasFocus);
            Keyboard.SendKeys(FormGlobalProperties.ESCAPE);
            Assert.IsFalse(cCustomEntityFormsMethods.HtmlTabControlEditText.HasFocus);
            Assert.IsFalse(cCustomEntityFormsMethods.HtmlSectionControlEditText.HasFocus);
            #endregion
        }
        #endregion

        #region 40264 Successfully verify Adding a section on a tab that is not the current tab in focus will add to correct tab
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyVerifyAddingASectionOnATabThatIsNotThecurrentTabInFocusWillAddToCorrectTab_UITest()
        {
            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            //Get Available fields window
            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[1]._headercaption);

            cCustomEntityFormsMethods.ClickIconOnTabContextMenu("New section");

            cCustomEntityFormsMethods.PopulateSectionNameTextBox("Section 1");

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            //Bring the tab that now contains the new section into focus
            cCustomEntityFormsMethods.ClickTab(customEntities[0].form[0].tabs[1]._headercaption);

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = "Section 1";
            cCustomEntityFormsMethods.ValidateSectionOrder();

        }
        #endregion

        #region Successfully verify Attribute displays on the Available fields window when deleting it from form by deleting the tab it belongs
        [TestMethod]
        public void CustomEntityFormsSuccessfullyVerifyAttributeDisplaysOnTheAvailableFieldsWindowWhenDeletingItFromFormByDeletingTheTabItBelongs_UITest()
        {
            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            foreach (var tab in customEntities[0].form[0].tabs)
            {
                foreach (var section in tab.sections)
                {
                    foreach (var attribute in section.fields)
                    {
                        Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(attribute.attribute.DisplayName));
                    }
                }
            }

            cCustomEntityFormsMethods.ClickExpandAvailableFieldsDialog();
            Keyboard.SendKeys(FormGlobalProperties.DOCK_UNDOCK_AVAILABLE_FIELDS_LEFT_KB_SHORTCUT, ModifierKeys.Control);
            foreach (var attributeInSectionToRemove in customEntities[0].form[0].tabs[0].sections[0].fields)
            {
                cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(attributeInSectionToRemove.attribute._attributeid));
                cCustomEntityFormsMethods.ClickIconOnFormField(attributeInSectionToRemove.attribute._attributeid, "Delete");
            }

            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            foreach (var attribute in customEntities[0].form[0].tabs[0].sections[0].fields)
            {
                if (attribute.attribute._attributeid > 0)
                {
                    Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(attribute.attribute.DisplayName));
                }
            }

            //Delete a section and verify all is gone
            cCustomEntityFormsMethods.ClickCogOnSection(GetCogFromSection(customEntities[0].form[0].tabs[0]._headercaption, customEntities[0].form[0].tabs[0].sections[1]._headercaption));
            cCustomEntityFormsMethods.ClickDeleteSection();
            cCustomEntityFormsMethods.ClickConfirmDeleteSection();
            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            foreach (var attribute in customEntities[0].form[0].tabs[0].sections[1].fields)
            {
                if (attribute.attribute._attributeid > 0)
                {
                    Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(attribute.attribute.DisplayName));
                }
            }

            //delete the second tab 
            cCustomEntityFormsMethods.ClickCogOnTab(customEntities[0].form[0].tabs[1]._headercaption);
            cCustomEntityFormsMethods.ClickDeleteTab();
            cCustomEntityFormsMethods.ClickConfirmDeleteTab();

            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            foreach (var section in customEntities[0].form[0].tabs[1].sections)
            {
                foreach (var attribute in section.fields)
                {
                    if (attribute.attribute._attributeid > 0)
                    {
                        Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(attribute.attribute.DisplayName));
                    }
                }
            }
        }

        #endregion

        #region  Successfully add ordering to custom entity form ***
        [TestMethod]
        public void CustomEntityFormsSuccesfullyAddOrderingToFormFieldsInCustomEntityForm_UITest()
        {

            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

            cCustomEntityMethods.ClickFormsLink();

            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            cCustomEntityFormsMethods.ClickFormDesign();

            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_SECTION_KB_SHORTCUT, ModifierKeys.Control);

            cCustomEntityFormsMethods.PopulateSectionNameTextBox(customEntities[0].form[0].tabs[0].sections[0]._headercaption);

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            //Ensure that its worked
            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = customEntities[0].form[0].tabs[0].sections[0]._headercaption;
            cCustomEntityFormsMethods.ValidateSectionOrder();

            Keyboard.SendKeys(FormGlobalProperties.DOCK_UNDOCK_AVAILABLE_FIELDS_RIGHT_KB_SHORTCUT, ModifierKeys.Control);

            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[0].DisplayName));
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[1].DisplayName));
            Assert.IsTrue(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[2].DisplayName));

            DragToDropField(customEntities[0].attribute[0].DisplayName, new Point(-400, -60));
            DragToDropField(customEntities[0].attribute[1].DisplayName, new Point(-400, -60));
            DragToDropField(customEntities[0].attribute[2].DisplayName, new Point(-400, -60));

            cCustomEntityFormsMethods.RefreshAvailableFieldsCache();
            Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[0].DisplayName));
            Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[1].DisplayName));
            Assert.IsFalse(cCustomEntityFormsMethods.DoesAttributeExistInAvaialbleFields(customEntities[0].attribute[2].DisplayName));


            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = customEntities[0].form[0].tabs[0].sections[0]._headercaption +
                FormatMandatoryLabel(customEntities[0].attribute[0]) +
                FormatMandatoryLabel(customEntities[0].attribute[1]) +
                FormatMandatoryLabel(customEntities[0].attribute[2]);
            cCustomEntityFormsMethods.ValidateSectionOrder();


            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].attribute[0]._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].attribute[0]._attributeid, "Edit label text");

            ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
            HtmlEdit FormFieldHeaderTxt = controlLocator.findControl("ctl00_contentmain_txtfieldlabel", new HtmlEdit(cCustomEntityFormsMethods.UICustomEntityCustomEnWindow.UICustomEntityCustomEnDocument5));

            FormFieldHeaderTxt.Text = "Form name";

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.MoveMouseToControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].attribute[1]._attributeid));

            cCustomEntityFormsMethods.ClickIconOnFormField(customEntities[0].attribute[1]._attributeid, "Edit label text");

            FormFieldHeaderTxt.Text = "Description";

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = customEntities[0].form[0].tabs[0].sections[0]._headercaption +
             "Form name*Description" + FormatMandatoryLabel(customEntities[0].attribute[2]);
            cCustomEntityFormsMethods.ValidateSectionOrder();

            //Add a new section
            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_SECTION_KB_SHORTCUT, ModifierKeys.Control);


            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_SECTION_KB_SHORTCUT, ModifierKeys.Control);

            cCustomEntityFormsMethods.PopulateSectionNameTextBox(customEntities[0].form[0].tabs[0].sections[1]._headercaption);

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = customEntities[0].form[0].tabs[0].sections[0]._headercaption +
             "Form name*Description" + FormatMandatoryLabel(customEntities[0].attribute[2]) +
             customEntities[0].form[0].tabs[0].sections[1]._headercaption;
            cCustomEntityFormsMethods.ValidateSectionOrder();

            UIControlLocator<UITestControl> UITestController = new UIControlLocator<UITestControl>();

            DragToDropField(UITestController.findControl(GenerateFormFieldLabelIdFromAttributeId(customEntities[0].attribute[0]._attributeid), new UITestControl(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7)), new Point(49, 10));
            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = customEntities[0].form[0].tabs[0].sections[0]._headercaption +
              "Description" + FormatMandatoryLabel(customEntities[0].attribute[2]) +
              customEntities[0].form[0].tabs[0].sections[1]._headercaption + "Form name*";
            cCustomEntityFormsMethods.ValidateSectionOrder();

            //Add a new tab

            Keyboard.SendKeys(FormGlobalProperties.ADD_NEW_TAB_KB_SHORTCUT, ModifierKeys.Control);

            cCustomEntityFormsMethods.PopulateTabNameTextBox(customEntities[0].form[0].tabs[1]._headercaption);

            Keyboard.SendKeys(FormGlobalProperties.ENTER);

            //TODO: ASSERT GOES HERE

            //Ensure that its worked
            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = "Section 1DescriptionNumberSection 1 EDITEDForm name*";
            cCustomEntityFormsMethods.ValidateSectionOrder();

            cCustomEntityFormsMethods.ClickTab(customEntities[0].form[0].tabs[0]._headercaption);
            //Move the first second to the second tab and assert

            SectionControlLocator<HtmlDiv> controlLocator2 = new SectionControlLocator<HtmlDiv>();
            HtmlDiv sourceSection = (HtmlDiv)controlLocator2.findControl("sectiontitle", "Section 1", new HtmlDiv(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));

            TabControlLocator<HtmlSpan> controlLocator3 = new TabControlLocator<HtmlSpan>();
            HtmlSpan sourceTab = (HtmlSpan)controlLocator3.findControl("sm_tabheader_middle", customEntities[0].form[0].tabs[1]._headercaption, new HtmlSpan(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));

            DragAndDropControl(sourceSection, sourceTab);

            //Required in order to force the codedui to wait for the loading screen to dismiss.
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.AllThreads;
            cCustomEntityFormsMethods.ClickSaveForm();
            cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.UIThreadOnly;
            cCustomEntityFormsMethods.ValidateSectionOrderExpectedValues.UIFormTabsPaneInnerText = "Section 1 EDITEDForm name*Section 1DescriptionNumber";
            cCustomEntityFormsMethods.ValidateSectionOrder();

        }
        #endregion

        #region Successfully Verify Available fields Title And Icons Constrast Correctly

        /// <summary>
        /// Successfully verifies that when colour is updated it should still make the title of
        /// available fields readable. Black background = white text. white background = black text.
        /// </summary>
        [TestCategory("Greenlight Forms"), TestCategory("Greenlight"), TestMethod]
        public void CustomEntityFormsSuccessfullyVerifyAvailablefieldsTitleAndIconsConstrastCorrectly_UITest()
        {

            Dictionary<string, string> colourMap = new Dictionary<string, string>();
            colourMap.Add("ffffff", "rgb(0,0,0)");
            colourMap.Add("000000", "rgb(255,255,255)");

            ColoursUIMap colours = new ColoursUIMap();

            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);

            foreach (KeyValuePair<string, string> iterator in colourMap)
            {
                cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/colours.aspx");
                ControlLocator<HtmlEdit> controlLocator = new ControlLocator<HtmlEdit>();
                HtmlEdit backgroundColourTextBox = controlLocator.findControl("ctl00_contentmain_txtmenubg", new HtmlEdit(cSharedMethods.browserWindow));
                backgroundColourTextBox.Text = iterator.Key;
                colours.PressSave();

                cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=" + customEntities[0].entityId);

                cCustomEntityMethods.ClickFormsLink();

                cCustomEntityFormsMethods.ClickEditFieldLink(customEntities[0].form[0]._formName);

                cCustomEntityFormsMethods.ClickFormDesign();

                cCustomEntityFormsMethods.VerifyAvailableFieldsTitleTextColourExpectedValues.UIAvailableFieldsPaneControlDefinition = iterator.Value;
                cCustomEntityFormsMethods.VerifyAvailableFieldsTitleTextColour();
            }
        }
        #endregion

        #region Additional test attributes

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            customEntityFormsNewMethods = new CustomEntityFormsNewUIMap();

            for (int entityindexer = customEntities.Count - 1; entityindexer > -1; entityindexer--)
            {
                Guid genericName = new Guid();
                genericName = Guid.NewGuid();
                customEntities[entityindexer].entityName = "Custom Entity " + genericName.ToString();
                customEntities[entityindexer].pluralName = "Custom Entity " + genericName.ToString();
                customEntities[entityindexer].description = "Custom Entity " + genericName.ToString();
                customEntities[entityindexer].entityId = 0;
                int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntities[entityindexer], _executingProduct);
                Assert.IsTrue(result > 0);
                if (entityindexer == 1)
                {
                    foreach (CustomEntitiesUtilities.CustomEntityView view in customEntities[entityindexer].view)
                    {
                        CustomEntitiesUtilities.CreateCustomEntityView(customEntities[entityindexer], view, _executingProduct);
                        Assert.IsTrue(view._viewid > 0);
                    }
                }
                for (int attributeindexer = 0; attributeindexer < customEntities[entityindexer].attribute.Count; attributeindexer++)
                {
                    if (customEntities[entityindexer].attribute[attributeindexer]._fieldType != FieldType.Relationship)
                    {
                        int resultatt = CustomEntitiesUtilities.CreateCustomEntityAttribute(customEntities[entityindexer], (CustomEntitiesUtilities.CustomEntityAttribute)customEntities[entityindexer].attribute[attributeindexer], _executingProduct);
                        Assert.IsTrue(resultatt > 0);
                    }
                    else if (customEntities[entityindexer].attribute[attributeindexer].GetType() == typeof(CustomEntitiesUtilities.CustomEntityOnetoNAttribute))
                    {
                        CustomEntitiesUtilities.CustomEntityOnetoNAttribute onetoNattribute = (CustomEntitiesUtilities.CustomEntityOnetoNAttribute)customEntities[entityindexer].attribute[attributeindexer];
                        int resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[entityindexer], onetoNattribute, customEntities[entityindexer + 1], _executingProduct);
                        Assert.IsTrue(resultatt > 0);
                    }
                    else if (customEntities[entityindexer].attribute[attributeindexer].GetType() == typeof(CustomEntitiesUtilities.CustomEntityNtoOneAttribute))
                    {
                        CustomEntitiesUtilities.CustomEntityNtoOneAttribute ntoOneattribute = (CustomEntitiesUtilities.CustomEntityNtoOneAttribute)customEntities[entityindexer].attribute[attributeindexer];
                        int resultatt = CustomEntitiesUtilities.CreateCustomEntityRelationship(customEntities[entityindexer], ntoOneattribute, customEntities[entityindexer + 1], _executingProduct);
                        Assert.IsTrue(resultatt > 0);
                    }
                }
            }
            //CacheUtilities.DeleteCachedTablesAndFields();
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Assert.IsNotNull(customEntities);
            for (int entityindexer = 0; entityindexer < customEntities.Count; entityindexer++)
            {
                int result = CustomEntityDatabaseAdapter.DeleteCustomEntity(customEntities[entityindexer].entityId, _executingProduct);
                Assert.AreEqual(0, result);
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

        #endregion

        //private string AddSpacersToformDesignerFieldList()
        //{
        //    StringBuilder expectedValue = new StringBuilder();
        //    foreach (CustomEntitiesUtilities.CustomEntityFormSection sections in customEntities[0].form[0].tabs[0].sections)
        //    {
        //        expectedValue.Append(sections._headercaption);
        //        int rowCounter = 0;
        //        int columnCounter = 0;
        //        CustomEntitiesUtilities.CustomEntityFormField previousAttribute = null;
        //        foreach (CustomEntitiesUtilities.CustomEntityFormField attribute in sections.fields)
        //        {
        //            if (attribute != null && attribute.attribute != null)
        //            {
        //                if (rowCounter < attribute._row)
        //                {
        //                    if (previousAttribute == null)
        //                    {
        //                        expectedValue.Append("Spacer");
        //                    }
        //                    if (attribute.columnSpan == 2)
        //                    {
        //                        rowCounter++;
        //                    } if (previousAttribute != null && previousAttribute.columnSpan == 1 && attribute.columnSpan == 1 && previousAttribute._row != attribute._row)
        //                    {
        //                        expectedValue.Append("Spacer");
        //                    }
        //                    else
        //                    {
        //                        expectedValue.Append("Spacer");
        //                    }
        //                }
        //                while (rowCounter < attribute._row)
        //                {
        //                    if (previousAttribute != null && previousAttribute._column != 1 && previousAttribute.columnSpan != 2 && attribute.columnSpan == 2)
        //                    {
        //                        rowCounter++;
        //                    }
        //                    else
        //                    {
        //                        expectedValue.Append("Spacer");
        //                        rowCounter++;
        //                    }
        //                }
        //                if (attribute.columnSpan != 2)
        //                {
        //                    while (columnCounter <= attribute._column)
        //                    {
        //                        if (columnCounter == attribute._column)
        //                        {
        //                            expectedValue.Append(CheckAndUpdateLabelIfMandatoryOrComment(attribute));
        //                        }
        //                        else
        //                        {
        //                            expectedValue.Append("Spacer");
        //                        }
        //                        columnCounter++;
        //                    }
        //                }
        //                else
        //                {
        //                    expectedValue.Append(CheckAndUpdateLabelIfMandatoryOrComment(attribute));
        //                    rowCounter++;
        //                    columnCounter = 0;
        //                }
        //                if (attribute._column >= 1)
        //                {
        //                    rowCounter++;
        //                    columnCounter = 0;
        //                }
        //                previousAttribute = attribute;
        //            }
        //        }
        //    }
        //    return expectedValue.ToString();
        //}

        //private string CheckAndUpdateLabelIfMandatoryOrComment(CustomEntitiesUtilities.CustomEntityFormField attribute)
        //{
        //    if (attribute.attribute._fieldType == FieldType.Comment)
        //    {
        //        return attribute.attribute._displayName + " - Note that the size of this comment block will increase as the amount of text increases.";
        //    }
        //    return attribute.attribute._mandatory ? attribute.attribute._displayName + "*" : attribute.attribute._displayName;
        //}

        private string FormatMandatoryLabel(CustomEntitiesUtilities.CustomEntityAttribute attribute)
        {
            return attribute._mandatory ? attribute.DisplayName + "*" : attribute.DisplayName;
        }

        #region static helper methods
        private static HtmlImage GetCogFromSection(string tabName, string sectionName)
        {
            int tabIndexOnPage = customEntities[0].form[0].tabs.FindIndex(f => f._headercaption.Equals(tabName));
            int SectionIndexOnTab = customEntities[0].form[0].tabs[tabIndexOnPage].sections.FindIndex(S => S._headercaption.Equals(sectionName));
            string sectionCogId = string.Format("Tab_{0}_Section_{1}_Title", tabIndexOnPage, SectionIndexOnTab);


            ControlLocator<HtmlDiv> controlLocator = new ControlLocator<HtmlDiv>();
            HtmlDiv sectionDiv = controlLocator.findControl(sectionCogId, new HtmlDiv(cSharedMethods.browserWindow));
            return (HtmlImage)sectionDiv.GetChildren()[0];
        }
        #endregion

        #region methods for drag
        private static string GenerateFormFieldLabelIdFromAttributeId(int id)
        {
            return string.Format("lbl_{0}", id);
        }

        private void DragToDropField(string ControlName, Point coordinateToDropTo)
        {
            HtmlSpan formfield = cCustomEntityFormsMethods.GetAvailableField(ControlName);
            Mouse.MouseDragSpeed = 150;
            Mouse.StartDragging(formfield, new Point(5, 5));
            Mouse.StopDragging(formfield, coordinateToDropTo);
            Mouse.Click();
        }

        private void DragToDropField(UITestControl control, Point coordinateToDropTo)
        {
            Mouse.MouseDragSpeed = 150;
            Mouse.StartDragging(control, new Point(5, 5));
            Mouse.StopDragging(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7, coordinateToDropTo);
            Mouse.Click();
        }


        private void DragToReplaceField(string sourceFieldId, string destinationFieldId)
        {
            ControlLocator<HtmlLabel> controlLocator = new ControlLocator<HtmlLabel>();
            HtmlLabel sourceField = (HtmlLabel)controlLocator.findControl(sourceFieldId, new HtmlLabel(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));
            HtmlLabel destinationField = (HtmlLabel)controlLocator.findControl(destinationFieldId, new HtmlLabel(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));
            sourceField.EnsureClickable();
            Mouse.Click();
            Mouse.MouseDragSpeed = 45;
            Mouse.StartDragging(sourceField, new Point(168, 20));
            Mouse.StopDragging(destinationField, new Point(173, 35));
            Mouse.Click();
        }

        private void DragToReplaceSection(string classname, string sourceSectionId, string destinationSectionId)
        {
            SectionControlLocator<HtmlDiv> controlLocator = new SectionControlLocator<HtmlDiv>();
            HtmlDiv sourceSection = (HtmlDiv)controlLocator.findControl(classname, sourceSectionId, new HtmlDiv(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));
            HtmlDiv destinationSection = (HtmlDiv)controlLocator.findControl(classname, destinationSectionId, new HtmlDiv(cCustomEntityFormsMethods.UIGreenLightCustomEntiWindow.UIGreenLightCustomEntiDocument7));
            sourceSection.EnsureClickable();

            Mouse.MouseDragSpeed = 150;
            Mouse.StartDragging(sourceSection, new Point(477, -23));
            Mouse.StopDragging(destinationSection, new Point(477, -33));
            Mouse.Click();
        }

        private void DragAndDropControl(UITestControl controlToMove, UITestControl destinationControl)
        {
            controlToMove.EnsureClickable();
            Mouse.MouseDragSpeed = 50;
            Mouse.StartDragging(controlToMove);
            Mouse.StopDragging(destinationControl);
            Mouse.Click();
        }

        #endregion

        /// <summary>
        /// Class for the controls that consist the General Details Tab on Forms modal
        ///</summary>
        /*internal class CustomEntityFormControls
        {
            internal HtmlEdit FormNameTxt { get; set; }
            internal cHtmlTextAreaWrapper DescriptionTxt { get; set; }
            internal HtmlCheckBox ShowSubMenuOption { get; set; }
            internal HtmlCheckBox ShowBreadcrumbsOption { get; set; }
            internal HtmlEdit SaveButtonTxt { get; set; }
            internal HtmlEdit SaveAndNewButtonTxt { get; set; }
            internal HtmlEdit SaveAndStayButtonTxt { get; set; }
            internal HtmlEdit CancelButtonTxt{get; set; }

            protected CustomEntityFormsUIMap _cCustomEntityFormsMethods;
            protected ControlLocator<HtmlControl> _ControlLocator { get; private set; }

            internal CustomEntityFormControls(CustomEntityFormsUIMap cCustomEntityFormsMethods)
            {
                _cCustomEntityFormsMethods = cCustomEntityFormsMethods;
                _ControlLocator = new ControlLocator<HtmlControl> ();
                FindControls();
            }

            internal HtmlTextArea textArea { get; set; }
            /// <summary>
            /// Locates controls that are declared within this class
            /// </summary>
            internal void FindControls()
            {
                FormNameTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_txtformname", new HtmlEdit(_cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
                DescriptionTxt = (cHtmlTextAreaWrapper)_ControlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_txtformdescription", new cHtmlTextAreaWrapper(cSharedMethods.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_tabConForms_tabGenDet_txtformdescription", _cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
                ShowSubMenuOption = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_chkshowsubmenu", new HtmlCheckBox(_cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
                ShowBreadcrumbsOption = (HtmlCheckBox)_ControlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_chkshowbreadcrumbs", new HtmlCheckBox(_cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
                SaveButtonTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_txtsavebuttontext", new HtmlEdit(_cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
                SaveAndNewButtonTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_txtsaveandnewbuttontext", new HtmlEdit(_cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
                SaveAndStayButtonTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_txtsaveandstaybuttontext", new HtmlEdit(_cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
                CancelButtonTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_tabConForms_tabGenDet_txtcancelbuttontext", new HtmlEdit(_cCustomEntityFormsMethods.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UICtl00_contentmain_taPane));
            }
        }*/

        /// <summary>
        /// Reads the Form data required by codedui tests from Lithium
        ///</summary>
        internal class CachePopulatorForForms : CachePopulator
        {

            internal CachePopulatorForForms(ProductType productType) : base(productType) { }
            public override string GetSQLStringForCustomEntity()
            {
                return "SELECT TOP 2 entityid, entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, audiencesViewType, enablePopupWindow, defaultPopupView, tableid, createdby FROM customEntities";
            }

            #region PopulateAttributes
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
                        view._createdBy = AutoTools.GetEmployeeIDByUsername(ExecutingProduct).ToString();
                        view._menudescription = reader.GetString(menuDescriptionOrdinal);
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
                            view.addform = null /*PopulateViewsFormDropdown(entity.form, reader.GetInt32(addFormOrdinal))*/;
                        }
                        if (!reader.IsDBNull(editFormOrdinal))
                        {
                            view.editform = null /*PopulateViewsFormDropdown(entity.form, reader.GetInt32(editFormOrdinal))*/;
                        }
                        if (!reader.IsDBNull(sortColumnJoinViaIDOrdinal))
                        {
                            //view.sortColumn_joinViaID = reader.GetInt32(sortColumnJoinViaIDOrdinal);
                        }
                        view.sortColumn = new CustomEntitiesUtilities.GreenLightSortColumn();
                        view._menuIcon = reader.IsDBNull(menuIconOrdinal) ? string.Empty : reader.GetString(menuIconOrdinal);
                        view.sortColumn._fieldID = reader.GetGuid(sortColumnOrdinal);
                        view.sortColumn._sortDirection = reader.GetByte(sortOrderOrdinal);
                        view._viewid = 0;
                        entity.view.Add(view);
                        #endregion
                    }
                    reader.Close();
                }
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
                        PopulateTabs(ref form);
                        entity.form.Add(form);
                        form._formid = 0;
                        #endregion
                    }
                    reader.Close();
                }
                db.sqlexecute.Parameters.Clear();
            }
            #endregion

            #region PopulateTabs
            internal void PopulateTabs(ref CustomEntitiesUtilities.CustomEntityForm form)
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
            internal void PopulateSections(ref CustomEntitiesUtilities.CustomEntityForm form, ref CustomEntitiesUtilities.CustomEntityFormTab tab)
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
            internal void populateFormFields(ref CustomEntitiesUtilities.CustomEntityForm form, ref CustomEntitiesUtilities.CustomEntityFormSection section)
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
                    int defaultValueOrdinal = reader.GetOrdinal("DefaultValue");
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
                        formfield._defaultValue = reader.IsDBNull(defaultValueOrdinal) ? null : reader.GetString(defaultValueOrdinal);
                        formfield.AttributeId = reader.GetInt32(attributeidOrdinal);
                        #endregion
                        section.fields.Add(formfield);
                    }
                }
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

        /// <summary>
        /// Imports the form data into the codedui database
        ///</summary>
        private void ImportFormDataToEx_CodedUIDatabase(string test)
        {
            int result;
            int resulttab;
            int customEntityAttributeIndex = 0;
            //int resultsect;
            switch (test)
            {
                case "CustomEntityFormsSuccessfullySortFormsGrid_UITest":
                    foreach (CustomEntitiesUtilities.CustomEntityForm form in customEntities[0].form)
                    {
                        result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], form, _executingProduct);
                        Assert.IsTrue(result > 0);
                    }
                    break;
                case "CustomEntityFormsUnsuccessfullyEditDuplicateFormToCustomEntity_UITest":
                case "CustomEntityFormsSuccessfullySortFormsGridToCustomEntity_UITest":
                case "CustomEntityFormsUnsucessfullyAddSectionOnCustomEntityFormWhereMandatoryFieldsAreMissing_UITest":
                case "UnsucessfullyAddTabOnCustomEntityFormWhereMandatoryFieldsAreMissing_UITest":
                case "CustomEntityFormsSuccessfullyVerifyKeyboardShortCutsInForms_UITest":
                    for (int i = 0; i < 2; i++)
                    {
                        result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[i], _executingProduct);
                        Assert.IsTrue(result > 0);
                    }
                    break;
                case "CustomEntityFormsSuccessfullyEditFormTabNameInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyEditFormTabNamInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyDeleteFormTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyDeleteFormTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyAddSectionToTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyAddSectionToTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyEditDuplicateFormTabNameInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyEditFormTabNameAndDeleteTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccesfullyAddOrderingToFormFieldsInCustomEntityForm_UITest":
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    break;
                case "CustomEntityFormsSuccessfullyMoveFormTabLeftInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyMoveFormTabRightInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyMoveFormTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyDragandDropTabInTabOnToCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyVerifyAddingASectionOnATabThatIsNotThecurrentTabInFocusWillAddToCorrectTab_UITest":
                case "CustomEntityFormsSuccessfullyAddSectionWithTheSameNameInDifferentTabsOfSameForm_UITest":
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[1], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    break;
                case "CustomEntityFormsSuccessfullyEditFormSectionNametoTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyEditFormSectionNametoTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyEditDuplicateFormSectionNametoTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyDeleteFormSectionfromTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyDeleteFormSectionfromTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyDragandDropFormFieldOnToCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyDragandDropFormFieldOnToCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyDragandDropNtoOneFormFieldOnToCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyDragandDropOnetoNFormFieldOnToCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyDragandDropFormFieldAfterSpacerOnToCustomEntityForm_UITest":
                case "CustomEntityFormsUnsucessfullyDragAndDropFormFieldToCustomEntityFormOutsideOfTheDropArea_UITest":
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0]._tabid, customEntities[0].form[0].tabs[0].sections[0], _executingProduct);
                    break;
                case "CustomEntityFormsSuccessfullyMoveFormSectionUpOnTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyMoveFormSectionDownOnTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyMoveFormSectionOnTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyMoveSectionBetweenTabsInFormDesigner_UITest":
                case "CustomEntityFormsSuccessfullyDragandDropFormFieldOnToDifferentSectionsOnCustomEntityForm_UITest":
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[1], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0]._tabid, customEntities[0].form[0].tabs[0].sections[0], _executingProduct);
                    CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0]._tabid, customEntities[0].form[0].tabs[0].sections[1], _executingProduct);
                    break;
                case "CustomEntityFormsSuccessfullyEditFormFieldNametoTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyEditFormFieldNametoTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyDeleteFormFieldFromSectiontoTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyDeleteFormFieldFromSectiontoTabInCustomEntityForm_UITest":
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0]._tabid, customEntities[0].form[0].tabs[0].sections[0], _executingProduct);
                    customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute = customEntities[0].attribute[1];
                    string temp3 = customEntities[0].form[0].tabs[0].sections[0].fields[1]._defaultValue;
                    customEntities[0].form[0].tabs[0].sections[0].fields[1]._defaultValue = string.Empty;
                    CustomEntitiesUtilities.CreateFormFields(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0].sections[0]._sectionid, customEntities[0].form[0].tabs[0].sections[0].fields[1], _executingProduct);
                    customEntities[0].form[0].tabs[0].sections[0].fields[1]._defaultValue = temp3;
                    break;
                case "CustomEntityFormsSuccessfullyMoveFormFieldLeftinSectionOfTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyMoveFormFieldRightinSectionOfTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyMoveFormFieldinSectionOfTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyMakeFormFieldReadonlyinSectionOfTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyMakeFormFieldReadonlyinSectionOfTabInCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyEraseFormFieldNameOnSectiontoTabInCustomEntityForm_UITest":
                case "CustomEntityFormsUnsuccessfullyEraseFormFieldNameOnSectiontoTabInCustomEntityForm_UITest":
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0]._tabid, customEntities[0].form[0].tabs[0].sections[0], _executingProduct);
                    customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute = customEntities[0].attribute[0];
                    string temp2 = customEntities[0].form[0].tabs[0].sections[0].fields[0]._defaultValue;
                    customEntities[0].form[0].tabs[0].sections[0].fields[0]._defaultValue = string.Empty;
                    CustomEntitiesUtilities.CreateFormFields(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0].sections[0]._sectionid, customEntities[0].form[0].tabs[0].sections[0].fields[0], _executingProduct);
                    customEntities[0].form[0].tabs[0].sections[0].fields[0]._defaultValue = temp2;
                    temp2 = customEntities[0].form[0].tabs[0].sections[0].fields[1]._defaultValue;
                    customEntities[0].form[0].tabs[0].sections[0].fields[1]._defaultValue = string.Empty;
                    customEntities[0].form[0].tabs[0].sections[0].fields[1].attribute = customEntities[0].attribute[1];
                    CustomEntitiesUtilities.CreateFormFields(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0].sections[0]._sectionid, customEntities[0].form[0].tabs[0].sections[0].fields[1], _executingProduct);
                    customEntities[0].form[0].tabs[0].sections[0].fields[1]._defaultValue = temp2;
                    break;
                case "CustomEntityFormsSuccessfullyDragandDropSectionInTabOnToCustomEntityForm_UITest":
                case "CustomEntityFormsSuccessfullyDragandDropFormFieldFromFirstToSecondSectionOnToCustomEntityForm_UITest":
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0]._tabid, customEntities[0].form[0].tabs[0].sections[0], _executingProduct);
                    CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0]._tabid, customEntities[0].form[0].tabs[0].sections[1], _executingProduct);
                    customEntities[0].form[0].tabs[0].sections[0].fields[0].attribute = customEntities[0].attribute[0];
                    string temp1 = customEntities[0].form[0].tabs[0].sections[0].fields[0]._defaultValue;
                    customEntities[0].form[0].tabs[0].sections[0].fields[0]._defaultValue = string.Empty;
                    CustomEntitiesUtilities.CreateFormFields(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0].sections[0]._sectionid, customEntities[0].form[0].tabs[0].sections[0].fields[0], _executingProduct);
                    customEntities[0].form[0].tabs[0].sections[0].fields[0]._defaultValue = temp1;
                    temp1 = customEntities[0].form[0].tabs[0].sections[1].fields[0]._defaultValue;
                    customEntities[0].form[0].tabs[0].sections[1].fields[0]._defaultValue = string.Empty;
                    customEntities[0].form[0].tabs[0].sections[1].fields[0].attribute = customEntities[0].attribute[9];
                    CustomEntitiesUtilities.CreateFormFields(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0].sections[1]._sectionid, customEntities[0].form[0].tabs[0].sections[1].fields[0], _executingProduct);
                    customEntities[0].form[0].tabs[0].sections[1].fields[0]._defaultValue = temp1;
                    break;
                case "CustomEntityFormsSuccessfullyAddDefaultTextOnFormField_UITest":
                case "CustomEntityFormsSuccessfullyCancelAddingDefaultTextOnFormField_UITest":
                case "CustomEntityFormsSuccessfullyFormFieldDefaultTextMaintainItsValueWhenReAddedToTheForm_UITest":
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0]._tabid, customEntities[0].form[0].tabs[0].sections[0], _executingProduct);
                    textFieldsSection = TextFieldSectionCachePopulator(customEntities[0].form[0].tabs[0]);
                    foreach (var field in textFieldsSection.fields)
                    {
                        string temp = field._defaultValue;
                        field._defaultValue = string.Empty;
                        CustomEntitiesUtilities.CreateFormFields(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0].sections[0]._sectionid, field, _executingProduct);
                        field._defaultValue = temp;
                    }
                    break;
                case "CustomEntityFormsSuccessfullyEditDefaultTextOnFormField_UITest":
                case "CustomEntityFormsSuccessfullyCancelEditingDefaultTextOnFormField_UITest":
                case "CustomEntityFormsSuccessfullyCopyFormWhereDefaultTextOnFormFieldIsSet_UITest":
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0], _executingProduct);
                    Assert.IsTrue(resulttab > 0);
                    CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0]._tabid, customEntities[0].form[0].tabs[0].sections[0], _executingProduct);
                    textFieldsSection = TextFieldSectionCachePopulator(customEntities[0].form[0].tabs[0]);
                    foreach (var field in textFieldsSection.fields)
                    {
                        CustomEntitiesUtilities.CreateFormFields(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[0].sections[0]._sectionid, field, _executingProduct);
                    }
                    break;
                default:
                    result = CustomEntitiesUtilities.CreateCustomEntityForm(customEntities[0], customEntities[0].form[0], _executingProduct);
                    Assert.IsTrue(result > 0);
                    customEntities[0].form[0]._formid = result;
                    //int customEntityAttributeIndex = 0;
                    for (int tabindexer = 0; tabindexer < customEntities[0].form[0].tabs.Count; tabindexer++)
                    {
                        resulttab = CustomEntitiesUtilities.CreateFormTabs(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[tabindexer], _executingProduct);
                        Assert.IsTrue(resulttab > 0);
                        for (int sectindexer = 0; sectindexer < customEntities[0].form[0].tabs[tabindexer].sections.Count; sectindexer++)
                        {
                            CustomEntitiesUtilities.CreateFormSection(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[tabindexer]._tabid, customEntities[0].form[0].tabs[tabindexer].sections[sectindexer], _executingProduct);
                            for (int fieldindexer = 0; fieldindexer < customEntities[0].form[0].tabs[tabindexer].sections[sectindexer].fields.Count; fieldindexer++)
                            {
                                customEntities[0].form[0].tabs[tabindexer].sections[sectindexer].fields[fieldindexer].attribute = customEntities[0].attribute[customEntityAttributeIndex];
                                CustomEntitiesUtilities.CreateFormFields(customEntities[0].form[0]._formid, customEntities[0].form[0].tabs[tabindexer].sections[sectindexer]._sectionid, customEntities[0].form[0].tabs[tabindexer].sections[sectindexer].fields[fieldindexer], _executingProduct);
                                customEntityAttributeIndex++;
                            }
                        }
                    }
                    break;
            }
        }

        private void MoveObjectWithinCollection(IList collection, int oldIndex, int newIndex)
        {
            Object o = collection[oldIndex];
            collection.RemoveAt(oldIndex);
            collection.Insert(newIndex, o);
        }

        /// <summary>
        /// Creates a section that contains only text and large text attributes based on the current list of custom entities
        /// </summary>
        /// <param name="tab">The Greenlight tab which contains the section we are transfering to our section cache</param>
        /// <returns>A re-ordered form section</returns>
        private CustomEntitiesUtilities.CustomEntityFormSection TextFieldSectionCachePopulator(CustomEntitiesUtilities.CustomEntityFormTab tab)
        {
            var textFieldsSection = new CustomEntitiesUtilities.CustomEntityFormSection();
            textFieldsSection.fields = new List<CustomEntitiesUtilities.CustomEntityFormField>();

            textFieldsSection._formid = tab.sections[0]._formid;
            textFieldsSection._headercaption = tab.sections[0]._headercaption;
            textFieldsSection._order = tab.sections[0]._order;
            textFieldsSection._sectionid = tab.sections[0]._sectionid;
            textFieldsSection._tabid = tab.sections[0]._tabid;

            foreach (var attribute in customEntities[0].attribute)
            {
                if (attribute._fieldType == FieldType.Text || attribute._fieldType == FieldType.LargeText)
                {
                    foreach (var section in tab.sections)
                    {
                        foreach (var field in section.fields)
                        {
                            if (attribute.OldAttributeId == field.AttributeId)
                            {
                                field.attribute = attribute;
                                field.AttributeId = attribute._attributeid;
                                textFieldsSection.fields.Add(field);
                                break;
                            }
                        }
                    }
                }
            }

            return textFieldsSection;
        }

        /// <summary>
        /// Returns the id of a text field on the form designer.
        /// Can be expanded with a switch statement to return the id of every type of field (i.e. dropdown lists etc)
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private string GetFormFieldID(CustomEntitiesUtilities.CustomEntityAttribute attribute)
        {
            return string.Format("txt{0}", attribute.DisplayName.Replace(" ", string.Empty).ToLower());
        }
    }

    public class Strings
    {
        public static string BasicString
        {
            get
            {
                return @"Hello world I've been pasted here to test the max length of my field. I'm hoping i can fit and earn loads of $$$ and perhaps a photo on the wall of fame! ";
            }
        }

        public static string LongString
        {
            get
            {
                string returnValue = string.Empty;
                for (int i = 0; i < 27; i++)
                {
                    returnValue += BasicString;
                }
                return returnValue;
            }
        }

    }


}
