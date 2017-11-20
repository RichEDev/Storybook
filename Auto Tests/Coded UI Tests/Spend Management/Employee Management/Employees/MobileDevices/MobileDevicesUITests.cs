namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.MobileDevices
{
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
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using System.Configuration;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;
    using Auto_Tests.Tools;
    using Auto_Tests.UIMaps.MyMobileDevicesUIMapClasses;
    using System.Threading;
    using Auto_Tests.Coded_UI_Tests.Expenses.System_Options.Attachment_Types;
    using Auto_Tests.UIMaps.GeneralOptionsUIMapClasses;
    using Auto_Tests.UIMaps.MobileDevicesEmployeesPageUIMapClasses;
    using Auto_Tests.UIMaps.EmployeesUIMapClasses;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Access_Roles.MyMobileDevicesAccessRoles;
    using Auto_Tests.Product_Variables.ModalMessages;
    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;

    /// <summary>
    /// Summary description for MobileDevicesUITests
    /// </summary>
    [CodedUITest]
    public class MobileDevicesUITests
    {
        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();
        private static List<MobileDevicesMethods.MobileDevice> mobileDevices;
        private static List<Employees> employees;
        private AttachmentTypesUITests cAttachmentTypesMethods = new AttachmentTypesUITests();
        private GeneralOptionsUIMap cGeneralOptionsMethods = new GeneralOptionsUIMap();
        private MobileDevicesEmployeesPageUIMap cMobileDevicesEmployeesPageMethods = new MobileDevicesEmployeesPageUIMap();
        private MobileDevicesMethods cMobileDevicesMethods = new MobileDevicesMethods();
        private EmployeesUIMap cEmployeesUIMap;
        private EmployeesNewUIMap employeesMethods;
        private static int employeeID;
        private string test;
        private static int AccessRoleId;
        public int userID = 0;
        private static ProductType _executingProduct = ProductType.expenses;

        public MobileDevicesUITests()
        {
            cEmployeesUIMap = new EmployeesUIMap();
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            browser.Maximized = true;
            cSharedMethods.Logon(_executingProduct, LogonType.administrator);
            MobileDevicesMethods.CachePopulator MobileDeviceDataFromLithium = new MobileDevicesMethods.CachePopulator();

            //Create access role for user
            if (!MyMobileDevicesAccessRoleMethods.AccessRoleExists("MobileDevicesAccessRole", _executingProduct))
            {
                AccessRoleId = MyMobileDevicesAccessRoleMethods.CreateMobileDevicesAccessRole("MobileDevicesAccessRole", MyMobileDevicesAccessRoleMethods.GetEmployeeIDByUsername(EnumHelper.GetEnumDescription(EmployeeType.Administrator), _executingProduct), _executingProduct);
                MyMobileDevicesAccessRoleMethods.SetMobileDevicesAccessRoleElement(AccessRoleId, 1, _executingProduct);
                MyMobileDevicesAccessRoleMethods.AssignClaimantMobileDevicesAccessRole(MyMobileDevicesAccessRoleMethods.GetEmployeeIDByUsername(EnumHelper.GetEnumDescription(EmployeeType.Administrator), _executingProduct), AccessRoleId, _executingProduct);
            }
            employees = EmployeesRepository.PopulateEmployee();
            employeeID = AutoTools.GetEmployeeIDByUsername(_executingProduct);
            mobileDevices = MobileDeviceDataFromLithium.PopulateCache(_executingProduct);
            Assert.IsNotNull(mobileDevices);
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            MyMobileDevicesAccessRoleMethods.DeleteMobileDevicesAccessRole(AccessRoleId, _executingProduct);
            cSharedMethods.CloseBrowserWindow();
        }

        /// <summary>
        /// 40483 - Successfully verify employees' mobile devices page layout
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyValidatePageLayoutForMyMobileDevices_UITest()
        {
            var employeeToUse = employees[0];

            //Navigate to the Employees page and click the mobile devices link
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);
            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            //Find the current date/time
            DateTime dt = DateTime.Now;
            string day = dt.ToString("dd");
            string monthName = dt.ToString("MMMM");
            string year = dt.ToString("yyyy");
            string currentTimeStr = day + " " + monthName + " " + year;

            //Verify the page
            cSharedMethods.VerifyPageLayout(string.Format("Employee: {0}", employeeToUse.UserName), "Before you can continue, please confirm the action required at the bottom of your screen.", "Company PolicyHelp & SupportExit", "Mr James Lloyd | Developer | " + currentTimeStr, "Page Options Employee DetailsCarsPool CarsCorporate CardsWork AddressesHome AddressesMobile Devices Help");

            //ensure save and cancel buttons exist
            HtmlImage saveBtn = cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UICtl00_contentmain_spPane.UIEmployeesPageSaveImage;
            string saveBtnExpectedText = "Save";
            Assert.AreEqual(saveBtnExpectedText, saveBtn.HelpText);

            HtmlImage cancelBtn = cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIEmployeesPageCancelImage;
            string cancelBtnExpectedText = "Cancel";
            Assert.AreEqual(cancelBtnExpectedText, cancelBtn.HelpText);
        }

        /// <summary>
        /// 40482 - Successfully verify employees' mobile devices modal layout
        /// 
        /// NOTE: The titles of the add and edit modals are verified in the following tests
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyValidateMobileDevicesModalLayout_UITest()
        {
            var employeeToUse = employees[0];

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            //Validate labels of fields and buttons of the modal
            cMobileDevicesEmployeesPageMethods.ValidateLabel(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameLabel, "Name*");
            cMobileDevicesEmployeesPageMethods.ValidateLabel(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeLabel, "Type*");

            HtmlInputButton saveButton = cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton;
            HtmlInputButton cancelButton = cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton;

            //Verify tabbing order 
            mobileDeviceControls.NameTxt.SetFocus();

            //Tab hit, therefore Type should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(mobileDeviceControls.TypeOption.HasFocus);

            //Tab hit, therefore save button should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(saveButton.HasFocus);

            //Tab hit, therefore cancel button should have focus
            Keyboard.SendKeys("{TAB}");
            Assert.IsTrue(cancelButton.HasFocus);
            Keyboard.SendKeys("{Enter}");
        }

        /// <summary>
        /// 40469 - Successfully Add Mobile Device 
        /// 40288 - Successfully verify fields being cleared on new mobile device modal
        /// 40492 - Successfully verify data are correct when creating mobile devices successively
        /// 40486 - Successfully add mobile device where special characters are used
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyAddMobileDevice_UITest()
        {

            var employeeToUse = employees[0];

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            #region Add Mobile Device and add device with invalid characters
            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
            {
                cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

                //Validate modal title
                cMobileDevicesEmployeesPageMethods.ValidateMobileDeviceModalTitle();

                //Initialise search space
                MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

                Assert.AreEqual(string.Empty, mobileDeviceControls.NameTxt.Text);
                Assert.AreEqual("[None]", mobileDeviceControls.TypeOption.SelectedItem);

                //Populate fields
                mobileDeviceControls.NameTxt.Text = device.DeviceName;
                mobileDeviceControls.TypeOption.SelectedItem = device.DeviceType;

                //Press Save
                cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

                //Validate Mobile Device Activation Key modal is displayed
                string devicekey = cMobileDevicesMethods.GetMobileActivationcodeByEmployeeUserName(employeeToUse.employeeID, device.DeviceName, _executingProduct);
                cMobileDevicesEmployeesPageMethods.ValidateMobileDeviceActivationKeyModal();
                cMobileDevicesEmployeesPageMethods.ValidateCommentOnUnpairedMobileDeviceModalExpectedValues.UISpanPairingKeyInfoPaneInnerText = "The device has been registered successfully.\r\n\r\n\r\nThe Activation Key is: " + devicekey + "\r\n\r\nYou now need to activate your device.\r\n\r\nTo do this, launch the app, enter the 16-digit key into the Activation screen and tap the Activate button.\r\n\r\n\r\nDon't have the app?\r\n\r\nTo download, open a browser and enter the website address: get.expenses360.mobi\r\n\r\nOr visit " + device.VendorDetails.MobileInstallFrom + " and search for Expenses360";
                cMobileDevicesEmployeesPageMethods.ValidateCommentOnUnpairedMobileDeviceModal();
                Assert.AreEqual("/shared/images/" + device.VendorDetails.MobileImage, cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeeJamesDocument2.UISharedimagesandroidpImageonAdd.AbsolutePath);

                //Close the modal
                cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIActivationKeyModalCloseButton);

                //Validate mobile device is displayed on the grid
                cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, device.DeviceName, device.DeviceType, "False", true);

                //Click edit and validate data are correct
                cMobileDevicesEmployeesPageMethods.ClickEditFieldLink(cSharedMethods.browserWindow, device.DeviceName);

                mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

                Assert.AreEqual(device.DeviceName, mobileDeviceControls.NameTxt.Text);
                Assert.AreEqual(device.DeviceType, mobileDeviceControls.TypeOption.SelectedItem);

                cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);
            }
            #endregion
            #region Validate data are correct when creating devices successively
            cMobileDevicesEmployeesPageMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDevices[0].DeviceName);
            MobileDevicesControls newMobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            Assert.AreEqual(mobileDevices[0].DeviceName, newMobileDeviceControls.NameTxt.Text);
            Assert.AreEqual(mobileDevices[0].DeviceType, newMobileDeviceControls.TypeOption.SelectedItem);
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);
            #endregion
        }

        /// <summary>
        /// 40481 - Successfully Cancel Adding Mobile Device 
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyCancelAddingMobileDevice_UITest()
        {
            var employeeToUse = employees[0];

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            Assert.AreEqual(string.Empty, mobileDeviceControls.NameTxt.Text);
            Assert.AreEqual("[None]", mobileDeviceControls.TypeOption.SelectedItem);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;

            //Press Cancel
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);

            //Validate mobile device is not displayed on the grid
            cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "False", false);
        }

        /// <summary>
        /// 40472 - Successfully edit paired mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyEditPairedMobileDevice_UITest()
        {

            var employeeToUse = employees[0];

            test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
            {
                //Click Edit mobile device
                cMobileDevicesEmployeesPageMethods.ClickEditFieldLink(cSharedMethods.browserWindow, device.DeviceName);

                //Validate modal is open and has the correct title
                cMobileDevicesEmployeesPageMethods.ValidateMobileDeviceModalTitleExpectedValues.UINewMobileDevicePane1InnerText = "Mobile Device: " + device.DeviceName;
                cMobileDevicesEmployeesPageMethods.ValidateMobileDeviceModalTitle();

                //Initialise search space
                MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

                //Populate fields
                mobileDeviceControls.NameTxt.Text = device.DeviceName + " EDITED";

                //Validate Type field is disabled
                Assert.IsFalse(mobileDeviceControls.TypeOption.Enabled);

                //Validate correct message is displayed in the comment
                cMobileDevicesEmployeesPageMethods.ValidateCommentOnMobileDeviceModalExpectedValues.UISpanMobileDeviceInfoPaneInnerText = @"

The device has been linked to this account.


The Activation Key is: " + device.PairingKey + @"

The Activation Key cannot be used with a device other than the one it is currently assigned to.

For new devices, register a new device to obtain a new Activation Key.";
                cMobileDevicesEmployeesPageMethods.ValidateCommentOnMobileDeviceModal();
                Assert.AreEqual("/shared/images/" + device.VendorDetails.MobileImage, cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeeJamesDocument2.UISharedimagesandroidpImageonEdit.AbsolutePath);

                //Press Save
                cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

                //Validate mobile device is displayed on the grid
                cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text, device.DeviceType, "True", true);

                //Click Edit mobile device
                cMobileDevicesEmployeesPageMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text);

                //Initialise search space
                mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

                Assert.AreEqual(device.DeviceName + " EDITED", mobileDeviceControls.NameTxt.Text);
                Assert.IsFalse(mobileDeviceControls.TypeOption.Enabled);
                mobileDeviceControls.NameTxt.Text = device.DeviceName;

                //Press Cancel
                cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);
          }
        }

        /// <summary>
        /// 40472 - Successfully edit unpaired mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyEditUnpairedMobileDevice_UITest()
        {

            var employeeToUse = employees[0];

            test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
            {
                //Click Edit mobile device
                cMobileDevicesEmployeesPageMethods.ClickEditFieldLink(cSharedMethods.browserWindow, device.DeviceName);

                //Validate modal is open and has the correct title
                cMobileDevicesEmployeesPageMethods.ValidateMobileDeviceModalTitleExpectedValues.UINewMobileDevicePane1InnerText = "Mobile Device: " + device.DeviceName;
                cMobileDevicesEmployeesPageMethods.ValidateMobileDeviceModalTitle();

                //Initialise search space
                MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

                //Populate fields
                mobileDeviceControls.NameTxt.Text = device.DeviceName + " EDITED";

                //Validate Type field is disabled
                Assert.IsFalse(mobileDeviceControls.TypeOption.Enabled);

                //Validate correct message is displayed in the comment
                cMobileDevicesEmployeesPageMethods.ValidateCommentOnMobileDeviceModalExpectedValues.UISpanMobileDeviceInfoPaneInnerText = "The Activation Key is: " + device.PairingKey + @"

You now need to activate your device.

To do this, launch the app, enter the 16-digit key into the Activation screen and tap the Activate button.


Don't have the app?

To download, open a browser and enter the website address: get.expenses360.mobi

Or visit " + device.VendorDetails.MobileInstallFrom + " and search for Expenses360";
                cMobileDevicesEmployeesPageMethods.ValidateCommentOnMobileDeviceModal();
                Assert.AreEqual("/shared/images/" + device.VendorDetails.MobileImage, cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeeJamesDocument2.UISharedimagesandroidpImageonEdit.AbsolutePath);

                //Press Save
                cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

                //Validate mobile device is displayed on the grid
                cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text, device.DeviceType, "False", true);

                //Click Edit mobile device
                cMobileDevicesEmployeesPageMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text);

                //Initialise search space
                mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

                Assert.AreEqual(device.DeviceName + " EDITED", mobileDeviceControls.NameTxt.Text);
                Assert.IsFalse(mobileDeviceControls.TypeOption.Enabled);
                mobileDeviceControls.NameTxt.Text = device.DeviceName;

                //Press Cancel
                cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);
            }
        }

        /// <summary>
        /// 40291 - Successfully cancel editing mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyCancelEditingMobileDevice_UITest()
        {

            var employeeToUse = employees[0];

            test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            //Click Edit mobile device
            cMobileDevicesEmployeesPageMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDevices[0].DeviceName);

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName + " EDITED";

            //Press Cancel
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);

            //Validate mobile device is displayed on the grid
            cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text, mobileDevices[0].DeviceType, "False", false);
        }

        /// <summary>
        /// 40474 - Successfully delete paired mobile device, 40476 - Successfully delete unpaired mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyDeleteMobileDevice_UITest()
        {

            var employeeToUse = employees[0];

            test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
            {
                //Click Edit mobile device
                cMobileDevicesEmployeesPageMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, device.DeviceName);

                //Validate message in delete window
                if (string.IsNullOrEmpty(device.SerialKey))
                {
                    cMobileDevicesEmployeesPageMethods.ValidateUnpairedDeviceDeleteMessage();
                }
                else
                {
                    cMobileDevicesEmployeesPageMethods.ValidatePairedDeviceDeleteMessage();
                }

                //Press Ok
                Keyboard.SendKeys("{Enter}");

                //Validate mobile device is not displayed on the grid
                cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, device.DeviceName, device.DeviceType, "False", false);
            }
        }

        /// <summary>
        /// 40503 - Successfully cancel deleting paired mobile device, 40505 - Successfully cancel deleting unpaired mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyCancelDeletingMobileDevice_UITest()
        {

            test = testContextInstance.TestName;
            var employeeToUse = employees[0];


            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
            {
                //Click Edit mobile device
                cMobileDevicesEmployeesPageMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, device.DeviceName);

                //Press Cancel
                Keyboard.SendKeys("{Tab}");
                Keyboard.SendKeys("{Enter}");

                //Validate mobile device is displayed on the grid
                if (string.IsNullOrEmpty(device.SerialKey))
                {
                    cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, device.DeviceName, device.DeviceType, "False", true);
                }
                else
                {
                    cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, device.DeviceName, device.DeviceType, "True", true);
                }
            }
        }

        /// <summary>
        /// 40484 - Unsuccessfully Add Mobile Device where Duplicate Information is used
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageUnsuccessfullyAddDuplicateMobileDevice_UITest()
        {

            test = testContextInstance.TestName;
            var employeeToUse = employees[0];

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;

            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

            //Validate duplicate modal
            cMobileDevicesEmployeesPageMethods.ValidateDuplicateMobileDeviceModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}{1}", EnumHelper.GetEnumDescription(_executingProduct), MobileDevicesEmployeesPageModalMessages.DuplicateFieldForMobileDeviceName);
            Assert.AreEqual(cMobileDevicesEmployeesPageMethods.ValidateDuplicateMobileDeviceModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText, cMobileDevicesEmployeesPageMethods.ModalMessage);

            //Press Close
            cMobileDevicesEmployeesPageMethods.PressCloseDuplicateMobileDeviceModal();

            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);
        }

        /// <summary>
        /// 40485 - Unsuccessfully Edit Mobile Device where Duplicate Information is used
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageUnsuccessfullyEditDuplicateMobileDevice_UITest()
        {

            test = testContextInstance.TestName;
            var employeeToUse = employees[0];

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            cMobileDevicesEmployeesPageMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDevices[0].DeviceName);

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[1].DeviceName;

            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

            //Validate duplicate modal
            cMobileDevicesEmployeesPageMethods.ValidateDuplicateMobileDeviceModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = string.Format("Message from {0}{1}", EnumHelper.GetEnumDescription(_executingProduct), MobileDevicesEmployeesPageModalMessages.DuplicateFieldForMobileDeviceName);
            Assert.AreEqual(cMobileDevicesEmployeesPageMethods.ValidateDuplicateMobileDeviceModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText, cMobileDevicesEmployeesPageMethods.ModalMessage);

            //Press Close
            cMobileDevicesEmployeesPageMethods.PressCloseDuplicateMobileDeviceModal();

            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);
        }

        /// <summary>
        /// 40471 - Unsuccessfully Add Mobile Device where Mandatory fields are missing
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageUnsuccessfullyAddMobileDeviceWhereMandatoryFieldsMissing_UITest()
        {
            var employeeToUse = employees[0];

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeAsterisk));

            //Populate Name field, leave control, empty field, leave control and verify asterisk is shown 
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SetFocus();
            mobileDeviceControls.NameTxt.Text = "";
            mobileDeviceControls.TypeOption.SetFocus();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeAsterisk));

            //Leave fields empty and Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

            //Validate duplicate modal
            cMobileDevicesEmployeesPageMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText = String.Format("Message from {0}{1}{2}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), MobileDevicesEmployeesPageModalMessages.EmptyFieldForMobileDeviceName, MobileDevicesEmployeesPageModalMessages.EmptyFieldForMobileDeviceType });
            Assert.AreEqual(cMobileDevicesEmployeesPageMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText, cMobileDevicesEmployeesPageMethods.ModalMessage);

            //Press Close
            cMobileDevicesEmployeesPageMethods.PressCloseMandatoryFieldsModal();

            //Leave Type field empty, save and validate mandatory fields modal
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;
            mobileDeviceControls.TypeOption.SetFocus();
            mobileDeviceControls.TypeOption.SelectedItem = "[None]";
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeAsterisk));
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);
            cMobileDevicesEmployeesPageMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText = String.Format("Message from {0}{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), MobileDevicesEmployeesPageModalMessages.EmptyFieldForMobileDeviceType });
            Assert.AreEqual(cMobileDevicesEmployeesPageMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText, cMobileDevicesEmployeesPageMethods.ModalMessage);
            cMobileDevicesEmployeesPageMethods.PressCloseMandatoryFieldsModal();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeAsterisk));

            //Leave only Name field empty, save and validate mandatory fields modal
            mobileDeviceControls.NameTxt.Text = "";
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);
            cMobileDevicesEmployeesPageMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText = String.Format("Message from {0}{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), MobileDevicesEmployeesPageModalMessages.EmptyFieldForMobileDeviceName });
            Assert.AreEqual(cMobileDevicesEmployeesPageMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText, cMobileDevicesEmployeesPageMethods.ModalMessage); 
            cMobileDevicesEmployeesPageMethods.PressCloseMandatoryFieldsModal();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeAsterisk));

            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);

            //Validate red asterisks do not display next to the mandatory fields in new mobile device modal
            cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeAsterisk));
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);
        }

        /// <summary>
        /// 40473 - Unsuccessfully edit Mobile Device where Mandatory fields are missing
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageUnsuccessfullyEditMobileDeviceWhereMandatoryFieldsMissing_UITest()
        {

            test = testContextInstance.TestName;
            var employeeToUse = employees[0];

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            cMobileDevicesEmployeesPageMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDevices[0].DeviceName);

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeAsterisk));

            //Leave Name field empty and verify asterisk is shown 
            mobileDeviceControls.NameTxt.Text = "";
            cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton.SetFocus();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeAsterisk));

            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

            //Validate duplicate modal
            cMobileDevicesEmployeesPageMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText = String.Format("Message from {0}{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), MobileDevicesEmployeesPageModalMessages.EmptyFieldForMobileDeviceName });
            Assert.AreEqual(cMobileDevicesEmployeesPageMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText, cMobileDevicesEmployeesPageMethods.ModalMessage);
            //Press Close
            cMobileDevicesEmployeesPageMethods.PressCloseMandatoryFieldsModal();

            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);

            //Validate red asterisks do not display next to the mandatory fields in new mobile device modal
            cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UITypeAsterisk));
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);
        }

        /// <summary>
        /// 40488 - Successfully verify maximum size for fields on mobile devices modal
        /// 40489 - Successfully verify truncate when copy/paste on fields on mobile devices modal
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyVerifyMaximumSizeOfFieldsOnMobileDevicesModal_UITest()
        {
            var employeeToUse = employees[0];

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            //Validate maximum length on Name field
            Assert.AreEqual(100, mobileDeviceControls.NameTxt.MaxLength);

            Clipboard.Clear();
            try { Clipboard.SetText("Hello world I've been pasted here to test the max length of my field. I'm hoping i can fit and earn loads of $$$ and perhaps a photo on the wall of fame! "); }
            catch (Exception) { }

            cSharedMethods.PasteText(mobileDeviceControls.NameTxt);
            Assert.AreEqual(100, mobileDeviceControls.NameTxt.Text.Length);

            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalCancelButton);
        }

        /// <summary>
        /// 40475 - Successfully validate Mobile Device status is updated where device is changing from unpaired to paired
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyValidateMobileDeviceStatusFromUnpairedToPaired_UITest()
        {

            var employeeToUse = employees[0];

            test = testContextInstance.TestName;

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;

            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIActivationKeyModalCloseButton);

            //Validate mobile device is marked as disabled
            cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "False", true);

            //Inser serial key to the database in order to mark the device as paired
            InsertSerialKeyToDevice(mobileDevices[0].SerialKey, mobileDevices[0].DeviceName);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            //Validate mobile device is marked as enabled
            cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "True", true);
        }

        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyVerifyAdministratorAccessToMobileDevicesWhenEnableMobileDevicesIsSettoTrue_UITest()
        {
            var employeeToUse = employees[0];

            MobileDevicesMethods.SetMobileDevices("0", _executingProduct);

            #region verify MobileDevices Hidden
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ValidateMobileDevicesLinkDisplayed(false);
            #endregion

            #region turn on general option mobile devices
            MobileDevicesMethods.SetMobileDevices("1", _executingProduct);
            #endregion

            #region verify MobileDevices accessible
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ValidateMobileDevicesLinkDisplayed(true);

            #endregion
        }

        /// <summary>
        /// 40468 - Successfully validate Employee's page Mobile Devices grid layout (Sorting of grid)
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullySortMobileDevicesGrid_UITest()
        {

            test = testContextInstance.TestName;
            var employeeToUse = employees[0];


            RestoreDefaultSortingOrder("myMobileDevices");

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            //Validate message when grid does not have any data 
            cMobileDevicesEmployeesPageMethods.ValidateGridDoesNotHaveAnyData();

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            //Sorts Mobile Devices table by Name column
            HtmlHyperlink displayNameLink = cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument1.UITbl_myMobileDevicesTable.UINameHyperlink;
            cMobileDevicesEmployeesPageMethods.ClickTableHeader(displayNameLink);
            cMobileDevicesEmployeesPageMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.Name, EnumHelper.TableSortOrder.DESC, mobileDevices[0].EmployeeID, _executingProduct);
            cMobileDevicesEmployeesPageMethods.ClickTableHeader(displayNameLink);
            cMobileDevicesEmployeesPageMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.Name, EnumHelper.TableSortOrder.ASC, mobileDevices[0].EmployeeID, _executingProduct);

            //Sorts Mobile Devices table by Type column
            displayNameLink = cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument1.UITbl_myMobileDevicesTable.UITypeHyperlink;
            cMobileDevicesEmployeesPageMethods.ClickTableHeader(displayNameLink);
            cMobileDevicesEmployeesPageMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.Type, EnumHelper.TableSortOrder.ASC, mobileDevices[0].EmployeeID, _executingProduct);
            cMobileDevicesEmployeesPageMethods.ClickTableHeader(displayNameLink);
            cMobileDevicesEmployeesPageMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.Type, EnumHelper.TableSortOrder.DESC, mobileDevices[0].EmployeeID, _executingProduct);

            //Sorts Mobile Devices table by Activation Key column
            displayNameLink = cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument1.UITbl_myMobileDevicesTable.UIActivationKeyHyperlink;
            cMobileDevicesEmployeesPageMethods.ClickTableHeader(displayNameLink);
            cMobileDevicesEmployeesPageMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.PairingKey, EnumHelper.TableSortOrder.ASC, mobileDevices[0].EmployeeID, _executingProduct);
            cMobileDevicesEmployeesPageMethods.ClickTableHeader(displayNameLink);
            cMobileDevicesEmployeesPageMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.PairingKey, EnumHelper.TableSortOrder.DESC, mobileDevices[0].EmployeeID, _executingProduct);

            RestoreDefaultSortingOrder("myMobileDevices");
        }

        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyVerifyNoAdministratorAccessToMobileDevicesWhenEnableMobileDevicesIsSettoFalse_UITest()
        {
            var employeeToUse = employees[0];

            MobileDevicesMethods.SetMobileDevices("1", _executingProduct);

            #region verify MobileDevices accessible
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ValidateMobileDevicesLinkDisplayed(true);

            #endregion

            #region turn off general option mobile devices
            MobileDevicesMethods.SetMobileDevices("0", _executingProduct);
            #endregion

            #region verify MobileDevices Hidden
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ValidateMobileDevicesLinkDisplayed(false);

            #endregion
        }

        /// <summary>
        /// 41282 - Successfully add Mobile Device to a recently created employee 
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyAddMobileDeviceToARecentlyCreatedEmployee_UITest()
        {
    
                test = testContextInstance.TestName;
                var employeeToAdd = employees[2];

                cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/selectemployee.aspx");

                cEmployeesUIMap.ClickNewEmployeeLink();
                cEmployeesUIMap.PopulateLogonDetailsSectionParams.UIUsernameEditText = employeeToAdd.UserName;

                cEmployeesUIMap.PopulateEmployeeNameSectionParams.UIFirstNameEditText = employeeToAdd.FirstName;
                cEmployeesUIMap.PopulateEmployeeNameSectionParams.UISurnameEditText = employeeToAdd.Surname;
                cEmployeesUIMap.PopulateEmployeeNameSectionParams.UITitleEditText = employeeToAdd.Title;

                cEmployeesUIMap.PopulateLogonDetailsSection();
                cEmployeesUIMap.PopulateEmployeeNameSection();

                cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

                cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

                MobileDevicesControls mobileDevicesControl = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

                mobileDevicesControl.NameTxt.Text = "Joe's wonderful apple brain controller";
                mobileDevicesControl.TypeOption.SelectedItem = "iPad";

                //Press Save
                cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

                //Close the modal
                cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIActivationKeyModalCloseButton);

                //Validate mobile device is displayed on the grid
                cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, "Joe's wonderful apple brain controller", "iPad", "False", true);

                // press save employee
                employeesMethods.PressSaveEmployeeButton();

                // search for employee
                employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employeeToAdd.UserName;
                Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

                // return employee id
                employeeToAdd.employeeID = employeesMethods.ReturnEmployeeIDFromGrid(employeeToAdd.UserName);

                // click edit against employee
                employeesMethods.ClickEditFieldLink(employeeToAdd.UserName);

                cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

                cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

                MobileDevicesMethods methods = new MobileDevicesMethods();
                string activationKey = methods.GetMobileActivationcodeByEmployeeUserName(employeeToAdd.employeeID, "Joe's wonderful apple brain controller", _executingProduct);

                string[] activationKeyElements = activationKey.Split('-');
                string employeeIdOFActivationKey = activationKeyElements[activationKeyElements.Length - 1];

                int employeeIDLength = employeeToAdd.employeeID.ToString().Length;
                string expectedEmployeeIDOfActivationKey = employeeToAdd.employeeID.ToString();
                for(int i = 0; i < 6 - employeeIDLength; i++)
                {
                    expectedEmployeeIDOfActivationKey = expectedEmployeeIDOfActivationKey.Insert(0, "0");
                }

                Assert.AreEqual(expectedEmployeeIDOfActivationKey, employeeIdOFActivationKey);
                cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, "Joe's wonderful apple brain controller", "iPad", activationKey, "False", true);
        }

        /// <summary>
        /// 40963 - Successfully add Mobile Device to different employee using the employees page
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Employees"), TestMethod]
        public void MobileDevicesEmployeesPageSuccessfullyCreateMobileDeviceWithTheSameNameInDifferentEmployeeAccounts_UITest()
        {

            test = testContextInstance.TestName;
            var employeeToUse = employees[1];
            
            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/aeemployee.aspx?employeeid=" + employeeToUse.employeeID);

            cMobileDevicesEmployeesPageMethods.ClickMobileDevicesLink();

            cMobileDevicesEmployeesPageMethods.ClickNewMobileDeviceLink();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMobileDevicesEmployeesPageMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;

            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIMobileDeviceModalSaveButton);

            //Press Close
            cSharedMethods.SetFocusOnControlAndPressEnter(cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UIActivationKeyModalCloseButton);

            //Validate Mobile Device is added to the grid
            cMobileDevicesEmployeesPageMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "False", true);
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            foreach(var employee in employees)
            {
                employee.employeeID = 0;
            }

            var employeeID = EmployeesRepository.CreateEmployee(employees[0], _executingProduct);
            Assert.IsTrue(employeeID > 0, "TestInitialize Error : Employee could not be created");

            foreach (var mobileDevice in mobileDevices)
            {
                mobileDevice.EmployeeID = employeeID;            
            }

            employeeID = EmployeesRepository.CreateEmployee(employees[1], _executingProduct);
            Assert.IsTrue(employeeID > 0, "TestInitialize Error : Employee could not be created");

            MobileDevicesMethods.SetMobileDevices("1", _executingProduct);
            employeesMethods = new EmployeesNewUIMap();
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (var employee in employees)
            {
                if (employee.employeeID > 0)
                {
                    EmployeesRepository.DeleteEmployee(employee.employeeID, _executingProduct);
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

        /// <summary>
        /// Class for the controls that consist Mobile Devices modal
        ///</summary>
        internal class MobileDevicesControls
        {
            internal HtmlEdit NameTxt { get; set; }
            internal HtmlComboBox TypeOption { get; set; }

            protected MobileDevicesEmployeesPageUIMap _cMobileDevicesEmployeesPageMethods;
            protected ControlLocator<HtmlControl> _ControlLocator { get; private set; }

            internal MobileDevicesControls(MobileDevicesEmployeesPageUIMap cMobileDevicesEmployeesPageMethods)
            {
                _cMobileDevicesEmployeesPageMethods = cMobileDevicesEmployeesPageMethods;
                _ControlLocator = new ControlLocator<HtmlControl>();
                FindControls();
            }

            internal void FindControls()           
            {
                NameTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_usrMobileDevices_txtDeviceName", new HtmlEdit(_cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UICtl00_contentmain_usPane1));
                TypeOption = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_usrMobileDevices_ddlDeviceType", new HtmlComboBox(_cMobileDevicesEmployeesPageMethods.UIEmployeejamesWindowsWindow.UIEmployeejamesDocument.UICtl00_contentmain_usPane1));
            }
        }

        /// <summary>
        /// Imports data to the database that the codedui will run
        ///</summary>
        private void ImportDataToEx_CodedUIDatabase(string test)
        {
            MobileDevicesMethods mobileDeviceMethods = new MobileDevicesMethods();
            foreach (MobileDevicesMethods.MobileDevice mobileDevice in mobileDevices)
            {
                mobileDevice.MobileDeviceID = 0;
            }
            switch (test)
            {
                case "MobileDevicesEmployeesPageSuccessfullyCancelEditingMobileDevice_UITest":
                case "MobileDevicesEmployeesPageUnsuccessfullyAddDuplicateMobileDevice_UITest":
                case "MobileDevicesEmployeesPageUnsuccessfullyEditMobileDeviceWhereMandatoryFieldsMissing_UITest":
                case "MobileDevicesEmployeesPageSuccessfullyCreateMobileDeviceWithTheSameNameInDifferentEmployeeAccounts_UITest":
                    {
                        mobileDevices[0].MobileDeviceID = mobileDeviceMethods.CreateMobileDevice(mobileDevices[0], _executingProduct);
                        Assert.IsTrue(mobileDevices[0].MobileDeviceID > 0);
                        break;
                    }
                case "MobileDevicesEmployeesPageSuccessfullyEditPairedMobileDevice_UITest":
                    {
                        foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
                        {
                            device.SerialKey = mobileDevices[0].SerialKey;
                            device.MobileDeviceID = mobileDeviceMethods.CreateMobileDevice(device, _executingProduct);
                            Assert.IsTrue(device.MobileDeviceID > 0);
                        }
                        break;
                    }
                case "MobileDevicesEmployeesPageSuccessfullyEditUnpairedMobileDevice_UITest":
                    {
                        foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
                        {
                            string serialKey = device.SerialKey;
                            device.SerialKey = null;
                            device.MobileDeviceID = mobileDeviceMethods.CreateMobileDevice(device, _executingProduct);
                            Assert.IsTrue(device.MobileDeviceID > 0);
                            device.SerialKey = serialKey;
                        }
                        break;
                    }
                default:
                    {
                        foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
                        {
                           
                            device.MobileDeviceID = mobileDeviceMethods.CreateMobileDevice(device, _executingProduct);
                            Assert.IsTrue(device.MobileDeviceID > 0);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Restores the Default Sorting Order for the Forms Grid
        ///</summary>
        private void RestoreDefaultSortingOrder(string grid)
        {
            int employeeid = AutoTools.GetEmployeeIDByUsername(_executingProduct);
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));
            //Ensure employee is recaching
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@currentDate", DateTime.Now);
            dbex_CodedUI.ExecuteSQL("UPDATE employees SET CacheExpiry = @currentDate WHERE employeeID = @employeeID");
            dbex_CodedUI.sqlexecute.Parameters.Clear();

            //Ensure grid always uses default sorting order
            string strSQL2 = "DELETE FROM employeeGridSortOrders WHERE employeeID = @employeeID AND gridID = @grid";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@grid", grid);
            dbex_CodedUI.ExecuteSQL(strSQL2);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Insert to an existing mobile device a serial key 
        ///</summary>
        private void InsertSerialKeyToDevice(string serialKey, string deviceName)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(_executingProduct));

            string strSQL = "UPDATE mobileDevices SET deviceSerialKey = @serialKey WHERE deviceName = @deviceName";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@serialKey", serialKey);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@deviceName", deviceName);
            dbex_CodedUI.ExecuteSQL(strSQL);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }
    }
}
