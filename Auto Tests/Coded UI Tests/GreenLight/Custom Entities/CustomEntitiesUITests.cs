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
using Auto_Tests.UIMaps.CustomEntitiesUIMapClasses;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Auto_Tests.Tools;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Threading;
using Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entity_Forms;

namespace Auto_Tests.Coded_UI_Tests.GreenLight.Custom_Entities
{
    /// <summary>
    /// Summary description for CustomEntitiesUITests
    /// </summary>
    [CodedUITest]
    public class CustomEntitiesUITests
    {
        /// <summary>
        /// Custom Entities UI map
        /// </summary>
        private static CustomEntitiesUIMap CustomEntitiesUIMap;
        /// <summary>
        /// Shared methods UI map
        /// </summary>
        private static SharedMethodsUIMap SharedMethodsUIMap;
        /// <summary>
        /// Custom Entities read from database
        /// </summary>
        private List<CustomEntity> cachedData;

        private static ProductType _executingProduct;

        /// <summary>
        /// Sets up test suite by starting IE and logging in to the product
        /// </summary>
        /// <param name="ctx"></param>
        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            SharedMethodsUIMap = new SharedMethodsUIMap();
            CustomEntitiesUIMap = new CustomEntitiesUIMap();
            _executingProduct = cGlobalVariables.GetProductFromAppConfig();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            SharedMethodsUIMap.Logon(_executingProduct, LogonType.administrator);
        }

        /// <summary>
        /// Clean up test suite
        /// Closes browser window to deal with modal errors
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            SharedMethodsUIMap.CloseBrowserWindow();
        }

        /// <summary>
        /// Successfully verify custom entity summary page layout
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyValidatePageLayoutForView_UITest()
        {
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            DateTime dt = DateTime.Now;
            string day = dt.ToString("dd");
            string monthName = dt.ToString("MMMM");
            string year = dt.ToString("yyyy");

            string currentTimeStr = day + " " + monthName + " " + year;
            if (_executingProduct == ProductType.expenses)
            {
                SharedMethodsUIMap.VerifyPageLayout("GreenLights", "Home : Administrative Settings : GreenLight Management : GreenLights", "Company PolicyHelp & SupportExit", "Mr James Lloyd | Developer | " + currentTimeStr, "Page OptionsNew GreenLight");
            }
            else
            {
                SharedMethodsUIMap.VerifyPageLayout("GreenLights", "Home : Administrative Settings : GreenLight Management : GreenLights", "AboutExit", "Mr James Lloyd | Developer | " + currentTimeStr, "Page OptionsNew GreenLight");
            }
            //ensure close button exists
            HtmlInputButton closeBtn = CustomEntitiesUIMap.UICustomEntitiesWindowWindow.UICustomEntitiesDocument1.UICloseButton;
            string closeBtnExpectedText = "close";
            Assert.AreEqual(closeBtnExpectedText, closeBtn.DisplayText);

            CustomEntitiesUIMap.ClickNewCustomEntity();
            
            
            //verify tabbing order
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlEdit txtEntityName = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtentityname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlEdit pluralNameText = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtpluralname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlTextArea txtDescription = (HtmlTextArea)cControlLocator.findControl("ctl00_contentmain_txtdescription", new HtmlTextArea(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox attachmentCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkenableattachments", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox documentMergeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkallowdocmerge", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox audienceViewType = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkaudienceViewType", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlComboBox currency = (HtmlComboBox)cControlLocator.findControl("ctl00_contentmain_ddlDefaultCurrency", new HtmlComboBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));

            HtmlInputButton saveButton = CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton;
            HtmlInputButton cancelButton = CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UICancelButton;

            //Verify tabbing order when enable currency record is not set!
            Assert.IsTrue(txtEntityName.HasFocus, "Greenlight Name textbox does not have focus");

            //Tab hit, therefore plural name should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(pluralNameText.HasFocus, "Plural Name textbox does not have focus");

            Keyboard.SendKeys("{TAB}");
            // Owner field has focus

            //Tab hit, therefore description should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(txtDescription.HasFocus, "Description textbox does not have focus");

            //Tab hit, Enable monetary record
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(monetaryRecordCheckBox.HasFocus, "Monetary Record checkbox does not have focus");


            //Tab hit, enable attachments should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(attachmentCheckBox.HasFocus, "Attachment checkbox does not have focus");

            //Tab hit, enable audiences should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(audienceViewType.HasFocus, "Enable Audiences checkbox does not have focus");

            //Tab hit, enable document merge should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(documentMergeCheckBox.HasFocus, "Document Merge checkbox does not have focus");

            // ** Verify tabbing order when monetary record is set ** //
            monetaryRecordCheckBox.Checked = true;
            txtEntityName.SetFocus();
            Assert.IsTrue(txtEntityName.HasFocus, "Greenlight Name textbox does not have focus");

            //Tab hit, therefore plural name should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(pluralNameText.HasFocus, "Plural Name textbox does not have focus");

            //Tab hit, therefore description should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(txtDescription.HasFocus, "Description textbox does not have focus");

            //Tab hit, Enable monetary record
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(monetaryRecordCheckBox.HasFocus, "Monetary Record checkbox does not have focus");

            //Tab hit, default currency should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(currency.HasFocus, "Currency textbox does not have focus");

            //Tab hit, enable attachments should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(attachmentCheckBox.HasFocus, "Attachment checkbox does not have focus");

            //Tab hit, enable audiences should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(audienceViewType.HasFocus, "Enable Audiences checkbox does not have focus");

            //Tab hit, enable document merge should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(documentMergeCheckBox.HasFocus, "Document Merge checkbox does not have focus");
        }

        /// <summary>
        /// Successfully verify custom entity form page layout
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyValidatePageLayoutForNewCustomEntity_UITest()
        {
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            CustomEntitiesUIMap.ClickNewCustomEntity();

            DateTime dt = DateTime.Now;
            string day = dt.ToString("dd");
            string monthName = dt.ToString("MMMM");
            string year = dt.ToString("yyyy");

            string currentTimeStr = day + " " + monthName + " " + year;
            //cSharedMethods.VerifyPageLayout("New Custom Entity", "Before you can continue, please confirm the action required at the bottom of your screen.", "Switch Sub Account | Company Policy | Help & Support | Exit", "Mr James Lloyd | Active Sub Account: Main Account | " + currentTimeStr, "Page Options Entity Details Attributes Forms Views");

            HtmlInputButton saveButton =  CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton;
            string saveButtonExpectedText = "save";
            Assert.AreEqual(saveButtonExpectedText, saveButton.DisplayText);

            HtmlInputButton cancelButton = CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UICancelButton;
            string cancelButtonExpectedText = "cancel";
            Assert.AreEqual(cancelButtonExpectedText, cancelButton.DisplayText);
        }

        /// <summary>
        /// This test relates to 38341
        /// Custom Entities : Unsuccessfully edit custom entity where duplicated details are used
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntityUnsuccessfullyEditDuplicate_UITest()
        {
            //Add two C.E's 
            cachedData = ReadDataFromLithium();
            ImportDataToEx_CodedUIDatabase(ref cachedData);
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");
            //Edit first C.E. to become the other and click save
            CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, cachedData[0].entityName);

            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlEdit txtEntityName = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtentityname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlEdit pluralNameText = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtpluralname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));


            txtEntityName.Text = cachedData[1].entityName;
            pluralNameText.Text = cachedData[1].pluralName;

            //Press Save
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

           
            //Verify modal

            CustomEntitiesUIMap.AssertDuplicateExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = String.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.", new object [] { EnumHelper.GetEnumDescription(_executingProduct) });

            
            //CustomEntitiesUIMap.AssertDuplicateExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = "Message from Expenses\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.";
            CustomEntitiesUIMap.AssertDuplicate();

            //Close the modal
            CustomEntitiesUIMap.PressCloseDuplicateValidationModal();
        }

        /// <summary>
        /// Successfully Add Custom Entity 
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyCreateCustomEntity_UITest()
        {
            cachedData = ReadDataFromLithium();
            Boolean ShouldMonetaryRecordBeVisibleWhenEditing = false;

            foreach (CustomEntity entityToCreate in cachedData)
            {
                SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");
            
                CustomEntitiesUIMap.ClickNewCustomEntity();
                //Build space for controls
                ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
                HtmlEdit txtEntityName = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtentityname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                HtmlEdit pluralNameText = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtpluralname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                HtmlTextArea txtDescription = (HtmlTextArea)cControlLocator.findControl("ctl00_contentmain_txtdescription", new HtmlTextArea(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                HtmlCheckBox monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                HtmlCheckBox attachmentCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkenableattachments", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                HtmlCheckBox documentMergeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkallowdocmerge", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                HtmlCheckBox audienceViewType = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkaudienceViewType", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                HtmlComboBox currency = (HtmlComboBox)cControlLocator.findControl("ctl00_contentmain_ddlDefaultCurrency", new HtmlComboBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                txtEntityName.Text = entityToCreate.entityName;
                pluralNameText.Text = entityToCreate.pluralName;
                txtDescription.Text = entityToCreate.description;
                monetaryRecordCheckBox.Checked = entityToCreate.enableCurrencies;
                attachmentCheckBox.Checked = entityToCreate.enableAttachments;
                documentMergeCheckBox.Checked = entityToCreate.allowDocumentMerge;
                audienceViewType.Checked = entityToCreate.AudienceViewType != AudienceViewType.NoAudience;
                if (monetaryRecordCheckBox.Checked)
                {
                    Assert.IsTrue(currency.Enabled);
                    currency.SelectedItem = entityToCreate.defaultCurrencyId;
                    ShouldMonetaryRecordBeVisibleWhenEditing = true;
                }
                else
                {
                    ShouldMonetaryRecordBeVisibleWhenEditing = false;
                    Assert.IsFalse(currency.Enabled);
                }

                //Press Save
                SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

                //Ensure entity is created
                CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, entityToCreate.entityName);

                //Refresh the attribute values
                txtEntityName = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtentityname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                pluralNameText = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtpluralname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                txtDescription = (HtmlTextArea)cControlLocator.findControl("ctl00_contentmain_txtdescription", new HtmlTextArea(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                attachmentCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkenableattachments", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                documentMergeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkallowdocmerge", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                audienceViewType = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkaudienceViewType", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
                currency = (HtmlComboBox)cControlLocator.findControl("ctl00_contentmain_ddlDefaultCurrency", new HtmlComboBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));

                //Assert text values
                Assert.AreEqual(entityToCreate.entityName, txtEntityName.Text);
                Assert.AreEqual(entityToCreate.pluralName, pluralNameText.Text);
                Assert.AreEqual(entityToCreate.description, txtDescription.Text);
                Assert.AreEqual(entityToCreate.enableCurrencies, monetaryRecordCheckBox.Checked);
                Assert.AreEqual(entityToCreate.enableAttachments, attachmentCheckBox.Checked);
                Assert.AreEqual(entityToCreate.allowDocumentMerge, documentMergeCheckBox.Checked);
                Assert.AreEqual(entityToCreate.AudienceViewType, audienceViewType.Checked);

                //Is monetary record diabaled and default currency field is not!
                if (ShouldMonetaryRecordBeVisibleWhenEditing)
                {
                    Assert.IsFalse(monetaryRecordCheckBox.Enabled);
                    Assert.IsTrue(currency.Enabled);
                    if (currency.Enabled)
                    {
                        Assert.IsTrue(monetaryRecordCheckBox.Checked);
                    }
                    else
                    {
                        Assert.IsFalse(monetaryRecordCheckBox.Checked);
                    }
                    Assert.AreEqual(string.IsNullOrEmpty(entityToCreate.defaultCurrencyId) ? "[None]" : entityToCreate.defaultCurrencyId, currency.SelectedItem);
                }
                else
                {
                    ///MSDN : Returns true if the control DOES exist before the timeout!
                    Assert.IsFalse(monetaryRecordCheckBox.Enabled);
                    Assert.IsFalse(currency.Enabled);
                    Assert.IsFalse(monetaryRecordCheckBox.Checked);
                }

                //Delete the custom entity from the database
                int result = CustomEntityDatabaseAdapter.DeleteCustomEntity(CustomEntityDatabaseAdapter.GetCustomEntityIdByName(entityToCreate.entityName, _executingProduct), _executingProduct);
                Assert.AreEqual(0, result);
            }
        }

        /// <summary>
        /// Successfully Cancel Adding Custom Entity 
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyCancelCreationOfCustomEntity_UITest()
        {
            cachedData = ReadDataFromLithium();
            SharedMethodsUIMap.Logon(_executingProduct, LogonType.administrator);
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");
            CustomEntitiesUIMap.ClickNewCustomEntity();

            //Build space for controls
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlEdit txtEntityName = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtentityname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlEdit pluralNameText = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtpluralname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlTextArea txtDescription = (HtmlTextArea)cControlLocator.findControl("ctl00_contentmain_txtdescription", new HtmlTextArea(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox attachmentCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkenableattachments", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox documentMergeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkallowdocmerge", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox audienceViewType = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkaudienceViewType", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            CustomEntity entityToCreate = cachedData[0];

            txtEntityName.Text = entityToCreate.entityName;
            pluralNameText.Text = entityToCreate.pluralName;
            txtDescription.Text = entityToCreate.description;
            monetaryRecordCheckBox.Checked = entityToCreate.enableCurrencies;
            attachmentCheckBox.Checked = entityToCreate.enableAttachments;
            audienceViewType.Checked = entityToCreate.AudienceViewType != AudienceViewType.NoAudience;
            documentMergeCheckBox.Checked = entityToCreate.allowDocumentMerge;
           
            //Press Cancel
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UICancelButton);

            //Ensure entity is not created
            CustomEntitiesUIMap.ValidateCustomEntityViewTable(SharedMethodsUIMap.browserWindow, entityToCreate.entityName, entityToCreate.description, false);
        }

        /// <summary>
        /// Successfully Edit Custom Entity 
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyEditCustomEntity_UITest()
        {
            cachedData = ReadDataFromLithium();
            ImportDataToEx_CodedUIDatabase(ref cachedData);
            foreach (CustomEntity entityToEdit in cachedData)
            {
                SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

                //CustomEntitiesUtilities.CustomEntity entityToEdit = cachedData[0];
                //CustomEntitiesUtilities.CustomEntity dataToInput = cachedData[1];

                //Click Edit against an existing custom entity

                CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, entityToEdit.entityName);

                //Populate fields with data

                SearchSpace cCEDetailsPage = BuildSearchSpace();
                cCEDetailsPage.entityNameTextBox.Text = entityToEdit.entityName + "Edited";
                cCEDetailsPage.pluralNameTextBox.Text = entityToEdit.pluralName + "Edited";
                cCEDetailsPage.DescriptionTextArea.Text = entityToEdit.description + "Edited";
                //CEDetailsPage.monetaryRecordCheckBox.Checked = dataToInput._enableCurrencies;
                cCEDetailsPage.attachmentCheckBox.Checked = !entityToEdit.enableAttachments;
                cCEDetailsPage.documentMergeCheckBox.Checked = !entityToEdit.allowDocumentMerge;
                cCEDetailsPage.audienceViewTypeCheckBox.Checked = entityToEdit.AudienceViewType == AudienceViewType.NoAudience;

                //If current C.E has currency enabled on insert then check if controls can be found!
                if (entityToEdit.enableCurrencies)
                {
                    //cCEDetailsPage.monetaryRecordCheckBox.Checked = dataToInput._enableCurrencies;
                    //ShouldMonetaryRecordBeVisibleWhenEditing = true;
                    Assert.IsTrue(cCEDetailsPage.currencyList.Enabled);
                    Assert.IsFalse(cCEDetailsPage.monetaryRecordCheckBox.Enabled);
                    Assert.IsTrue(cCEDetailsPage.monetaryRecordCheckBox.Checked);
                    cCEDetailsPage.currencyList.SelectedItem = entityToEdit.defaultCurrencyId;
                }
                else
                {
                    Assert.IsFalse(cCEDetailsPage.monetaryRecordCheckBox.WaitForControlNotExist(10));
                    Assert.IsFalse(cCEDetailsPage.currencyList.WaitForControlNotExist(10));
                }

                //Press Save Custom Entity
                SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

                //Validate changes are reflected on the table
                CustomEntitiesUIMap.ValidateCustomEntityViewTable(SharedMethodsUIMap.browserWindow, entityToEdit.entityName + "Edited", entityToEdit.description + "Edited");

                //Click Edit 
                CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, entityToEdit.entityName + "Edited");

                ////Refresh the attribute values
                cCEDetailsPage = BuildSearchSpace();

                //Assert text values
                Assert.AreEqual(entityToEdit.entityName + "Edited", cCEDetailsPage.entityNameTextBox.Text);
                Assert.AreEqual(entityToEdit.pluralName + "Edited", cCEDetailsPage.pluralNameTextBox.Text);
                Assert.AreEqual(entityToEdit.description + "Edited", cCEDetailsPage.DescriptionTextArea.Text);
                if (entityToEdit.enableCurrencies)
                {
                    Assert.AreEqual(entityToEdit.enableCurrencies, cCEDetailsPage.monetaryRecordCheckBox.Checked);
                    Assert.IsFalse(cCEDetailsPage.monetaryRecordCheckBox.Enabled);
                }
                else
                {
                    Assert.IsFalse(cCEDetailsPage.monetaryRecordCheckBox.WaitForControlNotExist(10));
                    Assert.IsFalse(cCEDetailsPage.currencyList.WaitForControlNotExist(10));
                }
                Assert.AreEqual(!entityToEdit.enableAttachments, cCEDetailsPage.attachmentCheckBox.Checked);
                Assert.AreEqual(!entityToEdit.allowDocumentMerge, cCEDetailsPage.documentMergeCheckBox.Checked);
                Assert.AreEqual(entityToEdit.AudienceViewType == AudienceViewType.NoAudience, cCEDetailsPage.audienceViewTypeCheckBox.Checked);

                ////Delete the custom entity from the database
            }
            Assert.AreEqual(0, CustomEntityDatabaseAdapter.DeleteCustomEntity(CustomEntityDatabaseAdapter.GetCustomEntityIdByName(cachedData[0].entityName + "Edited", _executingProduct), _executingProduct));
            Assert.AreEqual(0, CustomEntityDatabaseAdapter.DeleteCustomEntity(CustomEntityDatabaseAdapter.GetCustomEntityIdByName(cachedData[1].entityName + "Edited", _executingProduct), _executingProduct));  
        }

        /// <summary>
        /// Successfully Cancel Editing Custom Entity 
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyCancelEditCustomEntity_UITest()
        {
            cachedData = ReadDataFromLithium();
            //Insert a new custom entity into the database
            Assert.IsTrue(CustomEntityDatabaseAdapter.CreateCustomEntity(cachedData[0], _executingProduct) > 0);
            //CacheUtilities.DeleteCachedTablesAndFields();
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            //Click edit
            CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, cachedData[0].entityName);

            //Edit the entity
            //Build space for controls
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlEdit txtEntityName = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtentityname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlEdit pluralNameText = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtpluralname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlTextArea txtDescription = (HtmlTextArea)cControlLocator.findControl("ctl00_contentmain_txtdescription", new HtmlTextArea(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            //HtmlCheckBox monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(cCustomEntitiesMethods.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox attachmentCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkenableattachments", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox documentMergeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkallowdocmerge", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlCheckBox audienceViewType = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkaudienceViewType", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
           // HtmlComboBox currency = (HtmlComboBox)cControlLocator.findControl("ctl00_contentmain_ddlDefaultCurrency", new HtmlComboBox(cCustomEntitiesMethods.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));

            txtEntityName.Text = cachedData[0].entityName + "Edit";
            pluralNameText.Text = cachedData[0].pluralName + "Edit";
            txtDescription.Text = cachedData[0].description + "Edit";
            attachmentCheckBox.Checked = !cachedData[0].enableAttachments;
            audienceViewType.Checked = cachedData[0].AudienceViewType == AudienceViewType.NoAudience;
            documentMergeCheckBox.Checked = !cachedData[0].allowDocumentMerge;

            //Click cancel
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UICancelButton);
           
            //Click edit and verify
            CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, cachedData[0].entityName);
            txtEntityName = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtentityname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            pluralNameText = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtpluralname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            txtDescription = (HtmlTextArea)cControlLocator.findControl("ctl00_contentmain_txtdescription", new HtmlTextArea(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            //monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(cCustomEntitiesMethods.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            attachmentCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkenableattachments", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            documentMergeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkallowdocmerge", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            audienceViewType = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkaudienceViewType", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));

            //Assert text values
            Assert.AreEqual(cachedData[0].entityName, txtEntityName.Text);
            Assert.AreEqual(cachedData[0].pluralName, pluralNameText.Text);
            Assert.AreEqual(cachedData[0].description, txtDescription.Text);
            Assert.AreEqual(cachedData[0].enableAttachments, attachmentCheckBox.Checked);
            Assert.AreEqual(cachedData[0].allowDocumentMerge, documentMergeCheckBox.Checked);
            Assert.AreEqual(cachedData[0].AudienceViewType, audienceViewType.Checked);

            ////Delete the custom entity from the database
            CustomEntityDatabaseAdapter.DeleteCustomEntity(CustomEntityDatabaseAdapter.GetCustomEntityIdByName(cachedData[0].entityName, _executingProduct), _executingProduct);
        }

        /// <summary>
        /// Successfully Delete Custom Entity 
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyDeleteCustomEntity_UITest()
        {
            cachedData = ReadDataFromLithium();
            ImportDataToEx_CodedUIDatabase(ref cachedData);

            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            //Click delete
            CustomEntitiesUIMap.ClickDeleteFieldLink(SharedMethodsUIMap.browserWindow, cachedData[0].entityName);

            //Confirm deletion
            CustomEntitiesUIMap.PressOKDeleteCustomEntity();

            //Validate deletion
            CustomEntitiesUIMap.ValidateCustomEntityDeletion(cachedData[0].entityName);
        }

        /// <summary>
        /// Successfully Cancel Deleting Custom Entity 
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyCancelDeleteCustomEntity_UITest()
        {
            cachedData = ReadDataFromLithium();

            //Save new CE
            Assert.IsTrue(CustomEntityDatabaseAdapter.CreateCustomEntity(cachedData[0], _executingProduct) > 0);
            //CacheUtilities.DeleteCachedTablesAndFields();
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            //Click delete
            CustomEntitiesUIMap.ClickDeleteFieldLink(SharedMethodsUIMap.browserWindow, cachedData[0].entityName);

            //Click cancel on delete dialog
            CustomEntitiesUIMap.ClickCancelDeleteCustomEntity();

            //Ensure that the item can still be found
            CustomEntitiesUIMap.ValidateCustomEntityViewTable(SharedMethodsUIMap.browserWindow, cachedData[0].entityName, cachedData[0].description);

            CustomEntityDatabaseAdapter.DeleteCustomEntity(CustomEntityDatabaseAdapter.GetCustomEntityIdByName(cachedData[0].entityName, _executingProduct), _executingProduct);
        
        }

        /// <summary>
        /// Unsuccessfully Add Custom Entity Where Mandatory Fields Are Missing 
        /// UIItemPane corresponds to the red asterisk of Entity name field
        /// UIItemPane1 corresponds to the red asterisk of Plural name field
        /// UIItemPane2 corresponds to the red asterisk of Default Currency field
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntityUnsuccessfullyAddCustomEntityWhereMandatoryFieldsAreMissing_UITest()
        {
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");
            //Click New Custom Entity
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitiesWindowWindow.UICustomEntitiesDocument.UIPageOptionsNewCustomPane.UINewCustomEntityHyperlink);

            SearchSpace cCEDetailsPage = BuildSearchSpace();

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Press Save leaving mandatory fields empty
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

            //Validate mandatory fields are missing
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.", new object [] { EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));
           
            //Enable currencies
            cCEDetailsPage.monetaryRecordCheckBox.Checked = true;

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Press Save leaving mandatory fields empty
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

            //Validate mandatory fields are missing
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) }); ;
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Populate Entity Name and Plural Name
            cCEDetailsPage.entityNameTextBox.Text = "Codedui Test";
            cCEDetailsPage.pluralNameTextBox.Text = "Codedui Tests";

            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

            //Validate Currency Modal
            CustomEntitiesUIMap.ValidateMandatoryCurrencyModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object [] { EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.ValidateMandatoryCurrencyModal();

            //Close currency modal
            CustomEntitiesUIMap.PressCloseCurrencyValidationModal();

            //Validate red asterisks are displayed
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Cancel out of the page 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UICancelButton);

            //Navigate back to the new custom entity page
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitiesWindowWindow.UICustomEntitiesDocument.UIPageOptionsNewCustomPane.UINewCustomEntityHyperlink);

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));
        }

        /// <summary>
        /// Unsuccessfully Edit Custom Entity Where Mandatory Fields Are Missing 
        /// UIItemPane corresponds to the red asterisk of Entity name field
        /// UIItemPane1 corresponds to the red asterisk of Plural name field
        /// UIItemPane2 corresponds to the red asterisk of Default Currency field
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntityUnsuccessfullyEditCustomEntityWhereMandatoryFieldsAreMissing_UITest()
        {
            cachedData = ReadDataFromLithium();
            ImportDataToEx_CodedUIDatabase(ref cachedData);

            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            //Click Edit against an existing custom entity that is not a monetary record
            CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, cachedData[0].entityName);

            //Reset mandatory fields to blank
            SearchSpace cCEDetailsPage = BuildSearchSpace();

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
           
            //Set mandatory fields to blank
            cCEDetailsPage.entityNameTextBox.Text = "";
            cCEDetailsPage.pluralNameTextBox.Text = "";
            cCEDetailsPage.DescriptionTextArea.Text = "";

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));

            //Press Save
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

            //Validate mandatory fields modal
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) }); ;
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close validation modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
     
            //Exit Custom Entity details page

            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UICancelButton);

            //Click Edit against an existing custom entity that is a monetary record
            CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, cachedData[1].entityName);

            //Reset mandatory fields to blank
            SearchSpace cCEDetailsPage1 = BuildSearchSpace();

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Set Default currency to none
            cCEDetailsPage1.currencyList.SelectedItem = "[None]";

            //Validate red asterisks are displayed
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));
           
            //Press Save
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

            //Validate Currency Modal
            CustomEntitiesUIMap.ValidateMandatoryCurrencyModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object [] { EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.ValidateMandatoryCurrencyModal();

            //Close currency modal
            CustomEntitiesUIMap.PressCloseCurrencyValidationModal();

            //Validate red asterisks are displayed
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));
            
            //Reset remaining mandatory fields to blank
            cCEDetailsPage1.entityNameTextBox.Text = "";
            cCEDetailsPage1.pluralNameTextBox.Text = "";
            cCEDetailsPage.DescriptionTextArea.Text = "";

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Press Save
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

            //Validate mandatory fields modal
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object [] { EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close validation modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));
        }
        
         /// <summary>
        /// Unsuccessfully Add Custom Entity where Duplicate Data are used
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntityUnsuccessfullyAddDuplicateCustomEntity_UITest()
        {
            cachedData = ReadDataFromLithium();
            ImportDataToEx_CodedUIDatabase(ref cachedData);

            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            CustomEntity dataToInput = cachedData[0];

            //Click New Custom Entity Link
            CustomEntitiesUIMap.ClickNewCustomEntity();

            //Populate the fields with data
            SearchSpace cCEDetailsPage = BuildSearchSpace();
            cCEDetailsPage.entityNameTextBox.Text = dataToInput.entityName;
            cCEDetailsPage.pluralNameTextBox.Text = dataToInput.pluralName;
            cCEDetailsPage.DescriptionTextArea.Text = dataToInput.description;
            cCEDetailsPage.monetaryRecordCheckBox.Checked = dataToInput.enableCurrencies;
            cCEDetailsPage.attachmentCheckBox.Checked = dataToInput.enableAttachments;
            cCEDetailsPage.documentMergeCheckBox.Checked = dataToInput.allowDocumentMerge;
            cCEDetailsPage.audienceViewTypeCheckBox.Checked = dataToInput.AudienceViewType != AudienceViewType.NoAudience;

            //Press Save Custom Entity
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

            //Validate Modal 
            CustomEntitiesUIMap.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.", new object [] { EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.ValidateDuplicateDataModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressCloseDuplicateValidationModal();
        }

        /// <summary>
        /// Unsuccessfully Edit Custom Entity where Duplicate Data are used
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntityUnsuccessfullyEditDuplicateCustomEntity_UITest()
        {
            cachedData = ReadDataFromLithium();
            ImportDataToEx_CodedUIDatabase(ref cachedData);

            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            CustomEntity dataToInput = cachedData[0];

            //Click Edit against a custom entity
            CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, cachedData[1].entityName);

            //Populate details with duplicate information
            SearchSpace cCEDetailsPage = BuildSearchSpace();
            cCEDetailsPage.entityNameTextBox.Text = dataToInput.entityName;
            cCEDetailsPage.pluralNameTextBox.Text = dataToInput.pluralName;

            //Press Save Custom Entity
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);

            //Validate Modal 
            CustomEntitiesUIMap.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.", new object [] {EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.ValidateDuplicateDataModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressCloseDuplicateValidationModal();
        }

        /// <summary>
        /// Successfully verify Custom Entity Details page maintains correct data when adding/editing custom entity 
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyVerifyCustomEntityDetailsPageDisplaysCorrectData_UITest()
        {
            cachedData = ReadDataFromLithium();

            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            foreach (CustomEntity customEntity in cachedData)
            {
                //Click New Custom Entity link 
                CustomEntitiesUIMap.ClickNewCustomEntity();

                //Build Search Space
                SearchSpace cCEDetailsPage = BuildSearchSpace();

                //Validate that no data display on the fields
                Assert.AreEqual("", cCEDetailsPage.entityNameTextBox.Text);
                Assert.AreEqual("", cCEDetailsPage.pluralNameTextBox.Text);
                Assert.AreEqual("", cCEDetailsPage.DescriptionTextArea.Text);
                Assert.IsFalse(cCEDetailsPage.monetaryRecordCheckBox.Checked);
                Assert.IsFalse(cCEDetailsPage.attachmentCheckBox.Checked);
                Assert.IsFalse(cCEDetailsPage.documentMergeCheckBox.Checked);
                Assert.IsFalse(cCEDetailsPage.audienceViewTypeCheckBox.Checked);
                if (!cCEDetailsPage.monetaryRecordCheckBox.Checked)
                {
                    Assert.IsFalse(cCEDetailsPage.currencyList.Enabled);
                   
                }
                //Populate fields
                cCEDetailsPage.entityNameTextBox.Text = customEntity.entityName;
                cCEDetailsPage.pluralNameTextBox.Text = customEntity.pluralName;
                cCEDetailsPage.DescriptionTextArea.Text = customEntity.description;
                cCEDetailsPage.monetaryRecordCheckBox.Checked = customEntity.enableCurrencies;
                cCEDetailsPage.attachmentCheckBox.Checked = customEntity.enableAttachments;
                cCEDetailsPage.documentMergeCheckBox.Checked = customEntity.allowDocumentMerge;
                cCEDetailsPage.audienceViewTypeCheckBox.Checked = customEntity.AudienceViewType != AudienceViewType.NoAudience;
                if (cCEDetailsPage.monetaryRecordCheckBox.Checked)
                {
                    Assert.IsTrue(cCEDetailsPage.currencyList.Enabled);
                    cCEDetailsPage.currencyList.SelectedItem = customEntity.defaultCurrencyId;
                }
                else
                {
                    Assert.IsFalse(cCEDetailsPage.currencyList.Enabled);
                }

                //Save Custom Entity
                SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UISaveButton);
            }

            //Click edit against the custom entity that was created first
            CustomEntitiesUIMap.ClickEditFieldLink(SharedMethodsUIMap.browserWindow, cachedData[0].entityName);

            //Build Search Space
            SearchSpace cCEEditPage = BuildSearchSpace();
            
            //Verify it still shows correct data
            Assert.AreEqual(cachedData[0].entityName, cCEEditPage.entityNameTextBox.Text);
            Assert.AreEqual(cachedData[0].pluralName, cCEEditPage.pluralNameTextBox.Text);
            Assert.AreEqual(cachedData[0].description, cCEEditPage.DescriptionTextArea.Text);
            Assert.AreEqual(cachedData[0].enableCurrencies, cCEEditPage.monetaryRecordCheckBox.Checked);
            Assert.AreEqual(cachedData[0].enableAttachments, cCEEditPage.attachmentCheckBox.Checked);
            Assert.AreEqual(cachedData[0].allowDocumentMerge, cCEEditPage.documentMergeCheckBox.Checked);
            Assert.AreEqual(cachedData[0].AudienceViewType, cCEEditPage.audienceViewTypeCheckBox.Checked);
        }

        /// <summary>
        /// Successfully Sort Custom Entity Grid
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullySortCustomEntityGrid_UITest()
        {
            //Ensure Table is using default sorting order
            SharedMethodsUIMap.RestoreDefaultSortingOrder("gridEntities", _executingProduct);
            cachedData = ReadDataFromLithium();

            ImportDataToEx_CodedUIDatabase(ref cachedData);

            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            //Sorts Custom Entity by Entity Name column
            HtmlHyperlink displayNameLink = CustomEntitiesUIMap.UICustomEntitiesWindowWindow.UICustomEntitiesDocument.UITbl_gridEntitiesTable.UIEntityNameHyperlink;
            CustomEntitiesUIMap.ClickTableHeader(displayNameLink);
            CustomEntitiesUIMap.VerifyCorrectSortingOrderForTable(SortCustomEntitiesByColumn.EntityName, EnumHelper.TableSortOrder.DESC, _executingProduct);
            CustomEntitiesUIMap.ClickTableHeader(displayNameLink);
            CustomEntitiesUIMap.VerifyCorrectSortingOrderForTable(SortCustomEntitiesByColumn.EntityName, EnumHelper.TableSortOrder.ASC, _executingProduct);

            //Sorts Custom Entity by Description column
            displayNameLink = CustomEntitiesUIMap.UICustomEntitiesWindowWindow.UICustomEntitiesDocument.UITbl_gridEntitiesTable.UIDescriptionHyperlink;
            CustomEntitiesUIMap.ClickTableHeader(displayNameLink);
            CustomEntitiesUIMap.VerifyCorrectSortingOrderForTable(SortCustomEntitiesByColumn.Description, EnumHelper.TableSortOrder.ASC, _executingProduct);
            Thread.Sleep(1000);
            displayNameLink = CustomEntitiesUIMap.UICustomEntitiesWindowWindow.UICustomEntitiesDocument.UITbl_gridEntitiesTable.UIDescriptionHyperlink;
            Thread.Sleep(1000);
            CustomEntitiesUIMap.ClickTableHeader(displayNameLink);
            CustomEntitiesUIMap.VerifyCorrectSortingOrderForTable(SortCustomEntitiesByColumn.Description, EnumHelper.TableSortOrder.DESC, _executingProduct);

            SharedMethodsUIMap.RestoreDefaultSortingOrder("gridEntities", _executingProduct);
        }

        /// <summary>
        /// Test 38626 - When enabling monetary record the currency label should display mandatory characteristics
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySucessfullyVerifyDefaultCurrencyBecomesMandatoryWhenMonetaryRecordIsEnabled_UITest()
        {
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");
            CustomEntitiesUIMap.ClickNewCustomEntity();

            //Build search space
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            HtmlCheckBox monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlComboBox currency = (HtmlComboBox)cControlLocator.findControl("ctl00_contentmain_ddlDefaultCurrency", new HtmlComboBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            HtmlLabel defaultCurrencyLbl = (HtmlLabel)cControlLocator.findControl("ctl00_contentmain_lblDefaultCurrency", new HtmlLabel(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));

            //Assert Default currency is disabled and label is not madatory.
            Assert.AreEqual("Default currency", defaultCurrencyLbl.DisplayText);
            Assert.IsFalse(monetaryRecordCheckBox.Checked);
            Assert.IsFalse(currency.Enabled);
            
            //Refresh controls
            monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            currency = (HtmlComboBox)cControlLocator.findControl("ctl00_contentmain_ddlDefaultCurrency", new HtmlComboBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            defaultCurrencyLbl = (HtmlLabel)cControlLocator.findControl("ctl00_contentmain_lblDefaultCurrency", new HtmlLabel(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));

            //enable the monetary record
            monetaryRecordCheckBox.Checked = true;

            //ensure that default currency is enabled and label is updated
            Assert.AreEqual("Default currency*", defaultCurrencyLbl.DisplayText);
            Assert.IsTrue(monetaryRecordCheckBox.Checked);
            Assert.IsTrue(currency.Enabled);
        }

        /// <summary>
        /// Test 39202 - Unsuccessfully add custom entity where duplicate information is used and no UI save is performed
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntityUnsuccessfullyAddCustomEntityWhereDuplicateInformationIsUsedAndNoUISaveIsPerformed_UITest() 
        {
            cachedData = ReadDataFromLithium();
            ImportDataToEx_CodedUIDatabase(ref cachedData);

            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            CustomEntity dataToInput = cachedData[0];

            //Click New Custom Entity Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitiesWindowWindow.UICustomEntitiesDocument.UIPageOptionsNewCustomPane.UINewCustomEntityHyperlink);

            //Populate the fields with data
            SearchSpace cCEDetailsPage = BuildSearchSpace();
            cCEDetailsPage.entityNameTextBox.Text = dataToInput.entityName;
            cCEDetailsPage.pluralNameTextBox.Text = dataToInput.pluralName;

            //Click Attributes Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIAttributesHyperlink);

            //Click New Attribute 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIPgOptAttributesPane.UINewAttributeHyperlink);

            //Validate Modal 
            CustomEntitiesUIMap.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.", new object [] {EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.ValidateDuplicateDataModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressCloseDuplicateValidationModal();

            //Click New n:1 Relationship 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIPgOptAttributesPane.UINewn1RelationshipHyperlink);

            //Validate Modal
            CustomEntitiesUIMap.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.",new object [] {EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.ValidateDuplicateDataModal();

            //Close Validation Modal
            CustomEntitiesUIMap.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.",new object [] {EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.PressCloseDuplicateValidationModal();

            //Click New 1:n Relationship 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIPgOptAttributesPane.UINew1nRelationshipHyperlink);

            //Validate Modal 
            CustomEntitiesUIMap.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            CustomEntitiesUIMap.ValidateDuplicateDataModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressCloseDuplicateValidationModal();

            //Click New Summary Attribute
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIPgOptAttributesPane.UINewSummaryAttributeHyperlink);

            //Validate Modal 
            CustomEntitiesUIMap.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.",new object [] {EnumHelper.GetEnumDescription(_executingProduct)});
            CustomEntitiesUIMap.ValidateDuplicateDataModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressCloseDuplicateValidationModal();

            //Click Forms Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIFormsHyperlink);

            //Click New Form 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UINewFormPane.UINewFormHyperlink);

            //Validate Modal 
            CustomEntitiesUIMap.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            CustomEntitiesUIMap.ValidateDuplicateDataModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressCloseDuplicateValidationModal();

            //Click Views Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIViewsHyperlink);

            //Click New View 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UINewViewPane.UINewViewHyperlink);

            //Validate Modal 
            CustomEntitiesUIMap.ValidateDuplicateDataModalExpectedValues.UIDivMasterPopupPane1InnerText = string.Format("Message from {0}\r\n\r\n\r\nThe GreenLight name and Plural name you have provided already exist.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });

            CustomEntitiesUIMap.ValidateDuplicateDataModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressCloseDuplicateValidationModal();
        }

        /// <summary>
        /// Test 39199 - Unsuccessfully add custom entity where mandatory fields are missing and no UI save is performed
        /// UIItemPane corresponds to the red asterisk of Entity name field
        /// UIItemPane1 corresponds to the red asterisk of Plural name field
        /// UIItemPane2 corresponds to the red asterisk of Default Currency field
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntityUnsuccessfullyAddCustomEntityWhereMandatoryFieldsAreMissingAndNoUISaveIsPerformed_UITest() 
        {
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");

            //Click New Custom Entity Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitiesWindowWindow.UICustomEntitiesDocument.UIPageOptionsNewCustomPane.UINewCustomEntityHyperlink);

            SearchSpace cCEDetailsPage = BuildSearchSpace();

            //Enable currencies
            cCEDetailsPage.monetaryRecordCheckBox.Checked = true;

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Click Attributes Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIAttributesHyperlink);

            //Click New Attribute 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIPgOptAttributesPane.UINewAttributeHyperlink);

            //Validate mandatory fields are missing
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) }); ;
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Click Entity Details Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIEntityDetailsHyperlink);

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Click Attributes Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIAttributesHyperlink);

            //Click New n:1 Relationship 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIPgOptAttributesPane.UINewn1RelationshipHyperlink);

            //Validate mandatory fields are missing
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Click Entity Details Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIEntityDetailsHyperlink);

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Click Attributes Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIAttributesHyperlink);

            //Click New 1:n Relationship 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIPgOptAttributesPane.UINew1nRelationshipHyperlink);

            //Validate mandatory fields are missing
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Click Entity Details Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIEntityDetailsHyperlink);

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Click Attributes Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIAttributesHyperlink);

            //Click New Summary Attribute
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIPgOptAttributesPane.UINewSummaryAttributeHyperlink);

            //Validate mandatory fields are missing
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Click Entity Details Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIEntityDetailsHyperlink);

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Click Forms Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIFormsHyperlink);

            //Click New Form 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UINewFormPane.UINewFormHyperlink);

            //Validate mandatory fields are missing
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Click Entity Details Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIEntityDetailsHyperlink);

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));

            //Click Views Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UIViewsHyperlink);

            //Click New View 
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UICustomEntitymyCEWindWindow.UICustomEntitymyCEDocument.UINewViewPane.UINewViewHyperlink);

            //Validate mandatory fields are missing
            CustomEntitiesUIMap.ValidateMandatoryFieldsModalExpectedValues.UIDivMasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n\r\nPlease enter a GreenLight name.\r\nPlease enter a Plural name.\r\nA Default currency must be selected when creating a GreenLight involving monetary records.", new object[] { EnumHelper.GetEnumDescription(_executingProduct) });
            CustomEntitiesUIMap.ValidateMandatoryFieldsModal();

            //Close Validation Modal
            CustomEntitiesUIMap.PressOKOnMandatoryFieldsValidationModal();

            //Click Entity Details Link
            SharedMethodsUIMap.SetFocusOnControlAndPressEnter(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIEntityDetailsHyperlink);

            //Validate red asterisks are displayed
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane1));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(CustomEntitiesUIMap.UINewCustomEntityWindoWindow.UINewCustomEntityDocument.UIItemPane2));
        }

        /// <summary>
        /// Successfully verify maximum length of fields in custom entities
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyVerifyMaximumFieldSizesOnCustomEntities_UITest()
        {
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/custom_entities.aspx");
            CustomEntitiesUIMap.ClickNewCustomEntity();

            SearchSpace cCEDetailsPage = BuildSearchSpace();

            Assert.AreEqual(250, cCEDetailsPage.entityNameTextBox.MaxLength);
            Assert.AreEqual(250, cCEDetailsPage.pluralNameTextBox.MaxLength);
            Assert.AreEqual(4000, cCEDetailsPage.DescriptionTextArea.GetMaxLength());

            Clipboard.Clear();
            try { Clipboard.SetText(Strings.LongString); }
            catch (Exception) { }

            SharedMethodsUIMap.PasteText(cCEDetailsPage.entityNameTextBox);

            Assert.AreEqual(250, cCEDetailsPage.entityNameTextBox.Text.Length);

            SharedMethodsUIMap.PasteText(cCEDetailsPage.pluralNameTextBox);
            Assert.AreEqual(250, cCEDetailsPage.pluralNameTextBox.Text.Length);

            SharedMethodsUIMap.PasteText(cCEDetailsPage.DescriptionTextArea);
            Mouse.Click();
            Assert.AreEqual(4000, cCEDetailsPage.DescriptionTextArea.Text.Length);            
            Clipboard.Clear();
        }

        /// <summary>
        /// 38096 - Successfully verify query string accommodates invalid entries
        /// </summary>
        [TestCategory("Greenlight"), TestCategory("Greenlight Base"), TestMethod]
        public void CustomEntitySuccessfullyVerifyQueryStringAccomodatesForInvalidEntries_UITest()
        {
            //Navigate to an invalid page
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=37528uwyerw");

            //Validate Page
            CustomEntitiesUIMap.ValidateResourceNotFoundPage();

            //Navigate to an invalid page
            SharedMethodsUIMap.NavigateToPage(_executingProduct, "/shared/admin/aecustomentity.aspx?entityid=-3");

            //Validate Page
            CustomEntitiesUIMap.ValidateResourceNotFoundPage();
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            SharedMethodsUIMap = new SharedMethodsUIMap();
            CustomEntitiesUIMap = new CustomEntitiesUIMap();
        }

        ////Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (cachedData != null)
            {
                //Delete the C.E#s
                foreach (CustomEntity cEntity in cachedData)
                {
                    int id = CustomEntityDatabaseAdapter.GetCustomEntityIdByName(cEntity.entityName, _executingProduct);
                    if (id > 0) { CustomEntityDatabaseAdapter.DeleteCustomEntity(id, _executingProduct); }
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


         /// <summary>
        /// Reads the data that codedui tests will need from Lithium
        ///</summary>
        private static List<CustomEntity> ReadDataFromLithium()
        {
            StackTrace stackTrace = new StackTrace();

            List<CustomEntity> cDatabaseCustomEntity = new List<CustomEntity>();

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string SQLString;
            switch (stackTrace.GetFrame(1).GetMethod().Name)
            {
                case "CustomEntitySuccessfullyEditCustomEntity_UITest":
                case "CustomEntityUnsuccessfullyEditDuplicateCustomEntity_UITest":
                case "CustomEntitySuccessfullyVerifyCustomEntityDetailsPageDisplaysCorrectData_UITest":
                case "CustomEntityUnsuccessfullyEditCustomEntityWhereMandatoryFieldsAreMissing_UITest":
                case "CustomEntitySucessfullyVerifyDefaultCurrencyBecomesMandatoryWhenMonetaryRecordIsEnabled_UITest":
                case "CustomEntityUnsuccessfullyEditDuplicate_UITest":
                    SQLString = "SELECT TOP 2 entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, audienceViewType, enablePopupWindow, defaultPopupView, createdby FROM customEntities";
                    break;
                case "CustomEntityUnsuccessfullyAddDuplicateCustomEntity_UITest":
                case "CustomEntitySuccessfullyDeleteCustomEntity_UITest":
                case "CustomEntityUnsuccessfullyAddCustomEntityWhereDuplicateInformationIsUsedAndNoUISaveIsPerformed_UITest":
                    SQLString = "SELECT TOP 1 entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, audienceViewType, enablePopupWindow, defaultPopupView, createdby FROM customEntities";
                    break;
                case "CustomEntitySuccessfullySortCustomEntityGrid_UITest":
                    SQLString = "SELECT TOP 8 entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, audienceViewType, enablePopupWindow, defaultPopupView, createdby FROM customEntities";
                    break;
                default:
                    SQLString = "SELECT entity_name, plural_name, description, enableCurrencies, defaultCurrencyID, createdon, enableAttachments, allowdocmergeaccess, audienceViewType, enablePopupWindow, defaultPopupView, createdby FROM customEntities";
                    break;
                
             
            }
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(SQLString))
            {
                while (reader.Read())
                {
                    #region Set variables

                    var defaultPopupViewOrdinal = reader.GetOrdinal("defaultPopupView");
                    CustomEntity customEntity = new CustomEntity();

                    customEntity.entityName = "Custom Entity " + Guid.NewGuid().ToString();
                    customEntity.pluralName = customEntity.entityName;
                    customEntity.description = customEntity.entityName + " is used for Codeduis ";

                    //if (!reader.IsDBNull(0))
                    //{
                    //    customEntity.entityName = reader.GetString(0);
                    //}
                    //if (!reader.IsDBNull(1))
                    //{
                    //    customEntity.pluralName = reader.GetString(1);
                    //}
                    //if (!reader.IsDBNull(2))
                    //{
                    //    customEntity.description = reader.GetString(2);
                    //}
                    customEntity.enableCurrencies = reader.GetBoolean(3);
                    if (!reader.IsDBNull(4))
                    {
                        customEntity.defaultCurrencyId = reader.GetString(4);
                    }
                    else
                    {
                        customEntity.defaultCurrencyId = null;
                    }
                    customEntity.date = reader.GetDateTime(5);
                    customEntity.enableAttachments = reader.GetBoolean(6);
                    customEntity.allowDocumentMerge = reader.GetBoolean(7);
                    customEntity.AudienceViewType = (AudienceViewType)reader.GetInt16(8);
                    customEntity.EnablePopupWindow = reader.GetBoolean(9);
                    customEntity.DefaultPopupView = null;
                    //customEntity.DefaultPopupView = reader.IsDBNull(defaultPopupViewOrdinal) ? null : reader.GetInt32(defaultPopupViewOrdinal);
                    
                    customEntity.userId = AutoTools.GetEmployeeIDByUsername(_executingProduct);
                    cDatabaseCustomEntity.Add(customEntity);

                    #endregion
                }
                reader.Close();
            }
            return cDatabaseCustomEntity;
        }

        /// <summary>
        /// Imports data to the database that the codedui will run
        ///</summary>
        private void ImportDataToEx_CodedUIDatabase(ref List<CustomEntity> cDatabaseCustomEntity)
        {
            StackTrace stackTrace = new StackTrace();

            switch (stackTrace.GetFrame(1).GetMethod().Name)
            {
                /* case "CustomEntitySuccessfullyEditCustomEntity_UITest":
                 {
                     int result = CustomEntitiesUtilities.CreateCustomEntity(cDatabaseCustomEntity[0]);
                     Assert.IsTrue(result > 0);
                     cDatabaseCustomEntity[0]._entityId = result;
                     //CacheUtilities.DeleteCachedTablesAndFields();
                     break;
                 }*/
                default:
                {
                    
                    foreach (CustomEntity customEntity in cDatabaseCustomEntity)
                    {
                        int result = CustomEntityDatabaseAdapter.CreateCustomEntity(customEntity, _executingProduct);
                        Assert.IsTrue(result > 0);
                        customEntity.entityId = result;
                    }
                    //CacheUtilities.DeleteCachedTablesAndFields();
                    break;
                }
            }
        }

        /// <summary>
        /// Build search space for controls
        ///</summary>
        private SearchSpace BuildSearchSpace()
        {
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            SearchSpace newSearchSpace = new SearchSpace();
            newSearchSpace.entityNameTextBox = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtentityname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.pluralNameTextBox = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtpluralname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.DescriptionTextArea = (cHtmlTextAreaWrapper)cControlLocator.findControl("ctl00_contentmain_txtdescription", new cHtmlTextAreaWrapper(SharedMethodsUIMap.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_txtdescription", CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.attachmentCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkenableattachments", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.documentMergeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkallowdocmerge", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.audienceViewTypeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkaudienceViewType", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.currencyList = (HtmlComboBox)cControlLocator.findControl("ctl00_contentmain_ddlDefaultCurrency", new HtmlComboBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
        
            return newSearchSpace;
        }

        /// <summary>
        /// Build search space for controls
        ///</summary>
        private void BuildSearchSpace(out SearchSpace newSearchSpace)
        {
            ControlLocator<HtmlControl> cControlLocator = new ControlLocator<HtmlControl>();
            newSearchSpace = new SearchSpace();
            newSearchSpace.entityNameTextBox = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtentityname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.pluralNameTextBox = (HtmlEdit)cControlLocator.findControl("ctl00_contentmain_txtpluralname", new HtmlEdit(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.DescriptionTextArea = (cHtmlTextAreaWrapper)cControlLocator.findControl("ctl00_contentmain_txtdescription", new cHtmlTextAreaWrapper(SharedMethodsUIMap.ExtractHtmlMarkUpFromPage(), "ctl00_contentmain_txtdescription",  CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.monetaryRecordCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkEnableCurrencies", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.attachmentCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkenableattachments", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.documentMergeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkallowdocmerge", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.audienceViewTypeCheckBox = (HtmlCheckBox)cControlLocator.findControl("ctl00_contentmain_chkaudienceViewType", new HtmlCheckBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
            newSearchSpace.currencyList = (HtmlComboBox)cControlLocator.findControl("ctl00_contentmain_ddlDefaultCurrency", new HtmlComboBox(CustomEntitiesUIMap.UICustomEntityNewWindoWindow.UICustomEntityNewDocument.UIPgGeneralPane.UIGeneralDetailsEntityPane));
        }

        /// <summary>
        /// Used to define all the different types of controls that exist in the Custom Entity Details page
        ///</summary>
        private class SearchSpace
        {
            /// <summary>
            /// Entity name text box
            /// </summary>
            public HtmlEdit entityNameTextBox { get; set; }
            /// <summary>
            /// Plural name text box
            /// </summary>
            public HtmlEdit pluralNameTextBox { get; set; }
            /// <summary>
            ///Enable monetary record checkbox
            /// </summary>
            public HtmlCheckBox monetaryRecordCheckBox { get; set; }
            /// <summary>
            /// Enable attachments checkbox
            /// </summary>
            public HtmlCheckBox attachmentCheckBox { get; set; }
            /// <summary>
            /// Allow document merge checkbox
            /// </summary>
            public HtmlCheckBox documentMergeCheckBox { get; set; }
            /// <summary>
            /// Enable audiences checkbox
            /// </summary>
            public HtmlCheckBox audienceViewTypeCheckBox { get; set; }
            /// <summary>
            /// List of currency
            /// </summary>
            public HtmlComboBox currencyList {get; set;}
            /// <summary>
            /// Description text area
            /// </summary>
            public cHtmlTextAreaWrapper DescriptionTextArea { get; set; }

        }
    }
}
