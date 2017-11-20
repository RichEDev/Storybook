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
using Auto_Tests.UIMaps.GeneralOptionsUIMapClasses;
using Auto_Tests.UIMaps.AttachmentTypesUIMapClasses;
using Auto_Tests.Tools;
using System.Configuration;

namespace Auto_Tests.Coded_UI_Tests.Expenses.System_Options.Attachment_Types
{
    /// <summary>
    /// Summary description for AttachmentTypesUITests
    /// </summary>
    [CodedUITest]
    public class AttachmentTypesUITests
    {
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();
        private static GeneralOptionsUIMap cGeneralOptionsMethods;
        private static AttachmentTypesUIMap cAttachmentTypesMethods;
        private readonly static ProductType _executingProduct = ProductType.expenses;
        private AttachmentTypesMethods _attachmentTypesMethods = new AttachmentTypesMethods();

        public AttachmentTypesUITests()
        {
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            browser.Maximized = true;
            cSharedMethods.Logon(_executingProduct, LogonType.administrator);
        }
       
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            cSharedMethods.CloseBrowserWindow();
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            cGeneralOptionsMethods = new GeneralOptionsUIMap();
            cAttachmentTypesMethods = new AttachmentTypesUIMap();
        }

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}

        #endregion

        /// <summary>
        /// 40719 - Successfully verify png is not added as an attachment type when mobile devices are enabled and receipts are disabled 
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesSuccessfullyVerifyPNGIsNotAddedWhenMobileDevicesAreEnabledAndReceiptsAreDisabled_UITest()
        {
            //Ensure receipts and mobile devices are disabled 
            SetReceiptsAndMobileDevices("0", "0");

            //Ensure png does not exist as an attachment type
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/accountOptions.aspx");

            //Enable mobile devices and save
            cGeneralOptionsMethods.ClickMobileDevicesTab();

            cGeneralOptionsMethods.ClickEnableMobileDevices();

            cGeneralOptionsMethods.PressGeneralOptionsSaveButton();

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            //Validate PNG has not been automatically added to the grid 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", false);
        }

        /// <summary>
        /// 40720 - Unsuccessfully delete png from attachment types page when mobile devices and receipts are enabled 
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesUnsuccessfullyDeletePNGFromAttachmentTypesWhenMobileDevicesAndReceiptsAreEnabled_UITest()
        {
            //Ensure receipts and mobile devices are enabled 
            SetReceiptsAndMobileDevices("1", "1");

            //Ensure png does not exist as an attachment type and then add it
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);
            _attachmentTypesMethods.AddAttachmentTypeToDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            cAttachmentTypesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, "PNG");

            Keyboard.SendKeys("{Enter}");

            cAttachmentTypesMethods.ValidatePNGMandatoryForMobileDevicesModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = "Message from " + EnumHelper.GetEnumDescription(_executingProduct) + "\r\n\r\n\r\nThis attachment type is currently a mandatory requirem" +
                                                                                                                                  "ent for use with mobile devices. It cannot be deleted while mobile device receipt attachments are in use.";
            cAttachmentTypesMethods.ValidatePNGMandatoryForMobileDevicesModal();
            cAttachmentTypesMethods.PressOKInPNGMandatoryForMobileDevicesModal();

            //Validate PNG has not been deleted from the grid 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", true);
        }

        /// <summary>
        /// 40721 - Successfully delete png from attachment types page when mobile devices disabled and receipts are enabled 
        ///
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesSuccessfullyDeletePNGFromAttachmentTypesWhenMobileDevicesDisabledAndReceiptsEnabled_UITest()
        {
            //Ensure receipts are enabled and mobile devices are disabled 
            SetReceiptsAndMobileDevices("1", "0");

            //Ensure png does not exist as an attachment type and then add it
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);
            _attachmentTypesMethods.AddAttachmentTypeToDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            cAttachmentTypesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, "PNG");

            Keyboard.SendKeys("{Enter}");

            //Validate PNG has been deleted from the grid 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", false);
        }

        /// <summary>
        /// 40821 - Successfully verify png is added as an attachment type when mobile device is enabled in general options and receipts are enabled
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesSuccessfullyVerifyPNGIsAddedInAttachmentTypesWhenEnablingMobileDevicesWhilstReceiptsAreEnabled_UITest()
        {
            //Ensure receipts and mobile devices are disabled 
            SetReceiptsAndMobileDevices("0", "0");
            //Ensure png does not exist as an attachment type 
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);
            
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/accountOptions.aspx");
            //AutoTools.WaitForPage(cGeneralOptionsMethods.UIGeneralOptionsWindowWindow.UIGeneralOptionsDocument2.UINewExpensesHyperlink, 300000);
            //Enable receipts
            cGeneralOptionsMethods.ClickNewExpensesPage();
            cGeneralOptionsMethods.ClickNewExpensesOtherPreferencesTab();
            cGeneralOptionsMethods.ClickEnableReceipts();

            //Enable mobile devices 
            cGeneralOptionsMethods.ClickGeneralOptionsLink();
            cGeneralOptionsMethods.ClickMobileDevicesTab();
            cGeneralOptionsMethods.ClickEnableMobileDevices();

            //Validate modal is displayed
            cGeneralOptionsMethods.ValidatePNGWilBeAutomaticallyAddedModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = "Message from " + EnumHelper.GetEnumDescription(_executingProduct) + "\r\n\r\n\r\nEnabling this option will automatically permit .PNG fi" +
                                                                                                                               "le type attachments for use by mobile device receipts.\r\n\r\nThis is not currently one of the permitted attachment types.";
            cGeneralOptionsMethods.ValidatePNGWilBeAutomaticallyAddedModal();
            cGeneralOptionsMethods.PressOKInPNGWillAutomaticallyBeAddedModal();

            //Press Save
            cGeneralOptionsMethods.PressGeneralOptionsSaveButton();

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            //Validate PNG has been added to the grid 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", true);
        }

        /// <summary>
        /// 40822 - Successfully verify png can be deleted from the attachment types page when mobile device is enabled and receipts are disabled
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesSuccessfullyDeletePNGFromAttachmentTypesWhenMobileDevicesAreEnabledAndReceiptsAreDisabled_UITest()
        {
            //Ensure receipts are disabled and mobile devices are enabled 
            SetReceiptsAndMobileDevices("0", "1");

            //Ensure png does exists as an attachment type 
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);
            _attachmentTypesMethods.AddAttachmentTypeToDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            cAttachmentTypesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, "PNG");

            Keyboard.SendKeys("{Enter}");

            //Validate PNG has been deleted from the grid 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", false);
        }

        /// <summary>
        /// 40823 - Successfully verify png is added as an attachment type when mobile device is enabled and receipts are changed from disabled to enabled
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesSuccessfullyVerifyPNGIsAddedOnAttachmentTypesWhenEnablingReceiptsAndMobileDevicesAreEnabled_UITest()
        {
            //Ensure receipts and mobile devices are disabled 
            SetReceiptsAndMobileDevices("0", "0");

            //Ensure png does not exist as an attachment type 
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/accountOptions.aspx");

            //Enable mobile devices 
            cGeneralOptionsMethods.ClickMobileDevicesTab();
            cGeneralOptionsMethods.ClickEnableMobileDevices();

            //Enable receipts
            cGeneralOptionsMethods.ClickNewExpensesPage();
            cGeneralOptionsMethods.ClickNewExpensesOtherPreferencesTab();
            cGeneralOptionsMethods.ClickEnableReceipts();

            //Validate modal is displayed
            cGeneralOptionsMethods.ValidatePNGWilBeAutomaticallyAddedModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = "Message from " + EnumHelper.GetEnumDescription(_executingProduct) + "\r\n\r\n\r\nEnabling this option will automatically permit .PNG fi" +
                                                                                                                               "le type attachments for use by mobile device receipts.\r\n\r\nThis is not currently one of the permitted attachment types.";
            cGeneralOptionsMethods.ValidatePNGWilBeAutomaticallyAddedModal();
            cGeneralOptionsMethods.PressOKInPNGWillAutomaticallyBeAddedModal();

            //Press Save
            cGeneralOptionsMethods.PressGeneralOptionsSaveButton();

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            //Validate PNG has been added to the grid 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", true);
        }

        /// <summary>
        /// 40877 - Unsuccessfully archive png on attachment types page when mobile device and receipts are enabled in general options
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesUnsuccessfullyArchivePNGOnAttachmentTypesWhenMobileDevicesAndReceiptsAreEnabled_UITest()
        {
            //Ensure receipts and mobile devices are enabled 
            SetReceiptsAndMobileDevices("1", "1");

            //Ensure png does not exist as an attachment type and then add it
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);
            _attachmentTypesMethods.AddAttachmentTypeToDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            cAttachmentTypesMethods.ClickArchiveFieldLink(cSharedMethods.browserWindow, "PNG");

            Keyboard.SendKeys("{Enter}");

            cAttachmentTypesMethods.ValidatePNGMandatoryForMobileDevicesModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = "Message from Expenses\r\n\r\n\r\nThis attachment type is currently a mandatory requirement for use with mobile devices. It cannot be archived while mobile device receipt attachments are in use.";
            cAttachmentTypesMethods.ValidatePNGMandatoryForMobileDevicesModal();
            cAttachmentTypesMethods.PressOKInPNGMandatoryForMobileDevicesModal();

            //Validate PNG has not been deleted from the grid 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", true, false);
        }

        /// <summary>
        /// 41193 - Successfully delete png from the attachment types page when mobile device and receipts are disabled in general options
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesSuccessfullyDeletePNGFromAttachmentTypesWhenMobileDevicesAndReceiptsAreDisabled_UITest()
        {
            //Ensure receipts and mobile devices are disabled 
            SetReceiptsAndMobileDevices("0", "0");

            //Ensure png does not exist as an attachment type and then add it
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);
            _attachmentTypesMethods.AddAttachmentTypeToDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            cAttachmentTypesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, "PNG");

            Keyboard.SendKeys("{Enter}");

            //Validate PNG has not been deleted from the grid 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", false);
        }

        /// <summary>
        /// 41191 - Successfully verify png can be archived on the attachment types page when mobile device is enabled and receipts are disabled
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesSuccessfullyArchivePNGFromAttachmentTypesWhenMobileDevicesEnabledAndReceiptsAreDisabled_UITest()
        {
            //Ensure receipts are disabled and mobile devices are enabled 
            SetReceiptsAndMobileDevices("0", "1");

            //Ensure png does not exist as an attachment type and then add it
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);
            _attachmentTypesMethods.AddAttachmentTypeToDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            cAttachmentTypesMethods.ClickArchiveFieldLink(cSharedMethods.browserWindow, "PNG");

            Keyboard.SendKeys("{Enter}");

            //Validate PNG has been archived 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", true, true);
        }

        /// <summary>
        /// 41192 - Successfully verify png can be archived from the attachment types page when mobile device is disabled and receipts are enabled
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesSuccessfullyArchivePNGFromAttachmentTypesWhenMobileDevicesDisabledAndReceiptsAreEnabled_UITest()
        {
            //Ensure receipts are enabled and mobile devices are disabled 
            SetReceiptsAndMobileDevices("1", "0");

            //Ensure png does not exist as an attachment type and then add it
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);
            _attachmentTypesMethods.AddAttachmentTypeToDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            cAttachmentTypesMethods.ClickArchiveFieldLink(cSharedMethods.browserWindow, "PNG");

            Keyboard.SendKeys("{Enter}");

            //Validate PNG has been archived 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", true, true);
        }

        /// <summary>
        /// 41194 - Successfully archive png from the attachment types page when mobile device and receipts are disabled in general options
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Attachment Types"), TestMethod]
        public void AttachmentTypesSuccessfullyArchivePNGFromAttachmentTypesWhenMobileDevicesAndReceiptsAreDisabled_UITest()
        {
            //Ensure receipts and mobile devices are disabled 
            SetReceiptsAndMobileDevices("0", "0");

            //Ensure png does not exist as an attachment type and then add it
            _attachmentTypesMethods.DeleteAttachmentTypeFromDB("png", _executingProduct);
            _attachmentTypesMethods.AddAttachmentTypeToDB("png", _executingProduct);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/AttachmentTypes.aspx");

            cAttachmentTypesMethods.ClickArchiveFieldLink(cSharedMethods.browserWindow, "PNG");

            Keyboard.SendKeys("{Enter}");

            //Validate PNG has been archived 
            cAttachmentTypesMethods.ValidateAttachmentTypesTable(cSharedMethods.browserWindow, "PNG", true, true);
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

        /// <summary>
        /// Enables or disabled receipts in general options
        ///</summary>
        public void SetReceiptsAndMobileDevices(string receiptsStringValue, string mobileDevicesStringValue)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));

            string receiptsSQL = "UPDATE accountProperties SET stringValue = @receiptsStringValue WHERE stringKey = 'attachReceipts'";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@receiptsStringValue", receiptsStringValue);
            dbex_CodedUI.ExecuteSQL(receiptsSQL);
            dbex_CodedUI.sqlexecute.Parameters.Clear();

            string mobileDevicesSQL = "UPDATE accountProperties SET stringValue = @mobileDevicesStringValue WHERE stringKey = 'useMobileDevices'";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@mobileDevicesStringValue", mobileDevicesStringValue);
            dbex_CodedUI.ExecuteSQL(mobileDevicesSQL);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }
    }
}
