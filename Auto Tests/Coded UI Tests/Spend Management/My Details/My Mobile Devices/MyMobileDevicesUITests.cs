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
using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Access_Roles.MyMobileDevicesAccessRoles;
using Auto_Tests.UIMaps.MobileDevicesEmployeesPageUIMapClasses;

namespace Auto_Tests.Coded_UI_Tests.Spend_Management.My_Details.My_Mobile_Devices
{
    /// <summary>
    /// Summary description for MyMobileDevicesUITests
    /// </summary>
    [CodedUITest]
    public class MyMobileDevicesUITests
    {

        private static SharedMethodsUIMap cSharedMethods = new SharedMethodsUIMap();
        private  MyMobileDevicesUIMap cMyMobileDevicesMethods = new MyMobileDevicesUIMap();
        private static List<MobileDevicesMethods.MobileDevice> mobileDevices;
        private  AttachmentTypesUITests cAttachmentTypesMethods = new AttachmentTypesUITests();
        private  GeneralOptionsUIMap cGeneralOptionsMethods = new GeneralOptionsUIMap();
        private  MobileDevicesMethods cMobileDevicesMethods = new MobileDevicesMethods();
        private bool _runTestCleanup;
        private int claimantMobileDeviceID;
        private int claimantID;
        private static int AccessRoleId;
        private static int adminId;
        private static ProductType _executingProduct = ProductType.expenses;

        public MyMobileDevicesUITests() { }

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            browser.Maximized = true;
            adminId = AutoTools.GetEmployeeIDByUsername(_executingProduct);
            cSharedMethods.Logon(_executingProduct, LogonType.administrator);

            //General options - enable mobile devices
            MobileDevicesMethods.SetMobileDevices("1", _executingProduct);

            //Create access role for user
            if (!MyMobileDevicesAccessRoleMethods.AccessRoleExists("MobileDevicesAccessRole", _executingProduct))
            {
                AccessRoleId = MyMobileDevicesAccessRoleMethods.CreateMobileDevicesAccessRole("MobileDevicesAccessRole", MyMobileDevicesAccessRoleMethods.GetEmployeeIDByUsername(EnumHelper.GetEnumDescription(EmployeeType.Administrator), _executingProduct), _executingProduct);
                MyMobileDevicesAccessRoleMethods.SetMobileDevicesAccessRoleElement(AccessRoleId, 1, _executingProduct);
                MyMobileDevicesAccessRoleMethods.AssignClaimantMobileDevicesAccessRole(MyMobileDevicesAccessRoleMethods.GetEmployeeIDByUsername(EnumHelper.GetEnumDescription(EmployeeType.Administrator), _executingProduct), AccessRoleId, _executingProduct);
            } 
            MobileDevicesMethods.CachePopulator MobileDeviceDataFromLithium = new MobileDevicesMethods.CachePopulator();
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
        /// 40299 - Successfully verify my mobile devices page layout
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyValidatePageLayoutForMyMobileDevices_UITest()
        {
            _runTestCleanup = false;
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            DateTime dt = DateTime.Now;
            string day = dt.ToString("dd");
            string monthName = dt.ToString("MMMM");
            string year = dt.ToString("yyyy");

            string currentTimeStr = day + " " + monthName + " " + year;
            cSharedMethods.VerifyPageLayout("My Mobile Devices", "Home : My Details : My Mobile Devices", "Company PolicyHelp & SupportExit", "Mr James Lloyd | Developer | " + currentTimeStr, "Page OptionsNew Mobile Device Help");

            //ensure close button exists
            HtmlInputButton closeBtn = cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UIMyMobileDevicesCloseButton;
            string closeBtnExpectedText = "close";
            Assert.AreEqual(closeBtnExpectedText, closeBtn.DisplayText);
        }

        /// <summary>
        /// 40300 - Successfully verify my mobile devices modal layout
        /// 
        /// NOTE: The titles of the add and edit modals are verified in the following tests
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyValidateMyMobileDevicesModalLayout_UITest()
        {
            _runTestCleanup = false;
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            cMyMobileDevicesMethods.ClickNewMobileDevice();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            //Validate labels of fields and buttons of the modal
            cMyMobileDevicesMethods.ValidateLabel(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameLabel, "Name*");
            cMyMobileDevicesMethods.ValidateLabel(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeLabel, "Type*");

            HtmlInputButton saveButton = cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton;
            HtmlInputButton cancelButton = cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton;

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
        /// 33173 - Successfully Add Mobile Device 
        /// 40290 - Successfully verify fields being cleared on new mobile device modal
        /// 40333 - Successfully verify data are correct when creating mobile devices successively
        /// 40321 - Successfully add mobile device where special characters are used
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyAddMobileDevice_UITest()
        {
            _runTestCleanup = true;
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            #region Add Mobile Device
            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
            {
                cMyMobileDevicesMethods.ClickNewMobileDevice();

                //Validate modal title
                cMyMobileDevicesMethods.ValidateMobileDeviceModalTitle();

                //Initialise search space
                MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

                //validate fields display no data
                Assert.AreEqual(string.Empty, mobileDeviceControls.NameTxt.Text);
                Assert.AreEqual("[None]", mobileDeviceControls.TypeOption.SelectedItem);

                //Populate fields
                mobileDeviceControls.NameTxt.Text = device.DeviceName;
                mobileDeviceControls.TypeOption.SelectedItem = device.DeviceType;
                
                //Press Save
                cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);
                string devicekey = cMobileDevicesMethods.GetMobileActivationcodeByEmployeeUserName(adminId, device.DeviceName, _executingProduct);
                //Validate Mobile Device Activation Key modal is displayed
                cMyMobileDevicesMethods.ValidateMobileDeviceActivationKeyModal();
                cMyMobileDevicesMethods.ValidateCommentOnMobileDeviceActivationKeyModalExpectedValues.UISpanPairingKeyInfoPaneInnerText = "The device has been registered successfully.\r\n\r\n\r\nThe Activation Key is: " + devicekey + "\r\n\r\nYou now need to activate your device.\r\n\r\nTo do this, launch the app, enter the 16-digit key into the Activation screen and tap the Activate button.\r\n\r\n\r\nDon't have the app?\r\n\r\nTo download, open a browser and enter the website address: get.expenses360.mobi\r\n\r\nOr visit " + device.VendorDetails.MobileInstallFrom + " and search for Expenses360";
                cMyMobileDevicesMethods.ValidateCommentOnMobileDeviceActivationKeyModal();
                Assert.AreEqual("/shared/images/" + device.VendorDetails.MobileImage, cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument1.UISharedimagesandroidpImage.AbsolutePath);

                //Close the modal
                cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICloseButton);

                //Validate mobile device is displayed on the grid
                cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, device.DeviceName, device.DeviceType, "False", true);

                //Click edit and validate data are correct
                cMyMobileDevicesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, device.DeviceName);

                mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

                Assert.AreEqual(device.DeviceName, mobileDeviceControls.NameTxt.Text);
                Assert.AreEqual(device.DeviceType, mobileDeviceControls.TypeOption.SelectedItem);

                cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);
            }
            #endregion

            #region Validate data are correct when creating devices successively
            cMyMobileDevicesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDevices[0].DeviceName);
            MobileDevicesControls newMobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            Assert.AreEqual(mobileDevices[0].DeviceName, newMobileDeviceControls.NameTxt.Text);
            Assert.AreEqual(mobileDevices[0].DeviceType, newMobileDeviceControls.TypeOption.SelectedItem);
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);
            #endregion
        }

        /// <summary>
        /// 40293 - Successfully Cancel Adding Mobile Device 
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyCancelAddingMobileDevice_UITest()
        {
            _runTestCleanup = false;
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            cMyMobileDevicesMethods.ClickNewMobileDevice();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            Assert.AreEqual(string.Empty, mobileDeviceControls.NameTxt.Text);
            Assert.AreEqual("[None]", mobileDeviceControls.TypeOption.SelectedItem);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;

            //Press Cancel
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);

            //Validate mobile device is not displayed on the grid
            cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "False", false);
        }

        /// <summary>
        /// 33175 - Successfully edit paired mobile device, 40787 - Successfully edit unpaired mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyEditPairedMobileDevice_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
            {
                //Click Edit mobile device
                cMyMobileDevicesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, device.DeviceName);

                //Validate modal is open and has the correct title
                cMyMobileDevicesMethods.ValidateMobileDeviceModalTitleExpectedValues.UIMobileDevicesadaPaneInnerText = "Mobile Device: " + device.DeviceName;
                cMyMobileDevicesMethods.ValidateMobileDeviceModalTitle();

                //Initialise search space
                MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

                //Populate fields
                mobileDeviceControls.NameTxt.Text = device.DeviceName + " EDITED";

                //Validate Type field is disabled
                Assert.IsFalse(mobileDeviceControls.TypeOption.Enabled);

                //Validate correct message is displayed in the comment
                cMyMobileDevicesMethods.ValidateCommentOnMobileDeviceModalExpectedValues.UIDivMobileDeviceInfoPaneInnerText = " \r\n\r\nThe device has been linked to this account.\r\n\r\n\r\nThe Activation Key is: " + device.PairingKey + "\r\n\r\nThe Activation Key cannot be used with a device other than the one it is currently assigned to.\r\n\r\nFor new devices, register a new device to obtain a new Activation Key. ";
                cMyMobileDevicesMethods.ValidateCommentOnMobileDeviceModal();
                Assert.AreEqual("/shared/images/" + device.VendorDetails.MobileImage, cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument1.UISharedimagesandroidpImage1.AbsolutePath);

                //Press Save
                cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);

                //Validate mobile device is displayed on the grid
                cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text, device.DeviceType, "True", true);
      
                //Click Edit mobile device
                cMyMobileDevicesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text);

                //Initialise search space
                mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

                Assert.AreEqual(device.DeviceName + " EDITED", mobileDeviceControls.NameTxt.Text);
                Assert.IsFalse(mobileDeviceControls.TypeOption.Enabled);
                mobileDeviceControls.NameTxt.Text = device.DeviceName; 
                cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);
            }
        }

        /// <summary>
        /// 40787 - Successfully edit unpaired mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyEditUnpairedMobileDevice_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
            {
                //Click Edit mobile device
                cMyMobileDevicesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, device.DeviceName);

                //Validate modal is open and has the correct title
                cMyMobileDevicesMethods.ValidateMobileDeviceModalTitleExpectedValues.UIMobileDevicesadaPaneInnerText = "Mobile Device: " + device.DeviceName;
                cMyMobileDevicesMethods.ValidateMobileDeviceModalTitle();

                //Initialise search space
                MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

                //Populate fields
                mobileDeviceControls.NameTxt.Text = device.DeviceName + " EDITED";

                //Validate Type field is disabled
                Assert.IsFalse(mobileDeviceControls.TypeOption.Enabled);

                //Validate correct message is displayed in the comment
                cMyMobileDevicesMethods.ValidateMobileDeviceActivationKeyModal();
                cMyMobileDevicesMethods.ValidateCommentOnUnpairedMobileDeviceModalExpectedValues.UIDivMobileDeviceInfoPane1InnerText = " The Activation Key is: " + device.PairingKey + "\r\n\r\nYou now need to activate your device.\r\n\r\nTo do this, launch the app, enter the 16-digit key into the Activation screen and tap the Activate button.\r\n\r\n\r\nDon't have the app?\r\n\r\nTo download, open a browser and enter the website address: get.expenses360.mobi\r\n\r\nOr visit " + device.VendorDetails.MobileInstallFrom + " and search for Expenses360\r\n ";
                cMyMobileDevicesMethods.ValidateCommentOnUnpairedMobileDeviceModal();
                Assert.AreEqual("/shared/images/" + device.VendorDetails.MobileImage, cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument1.UISharedimagesandroidpImage1.AbsolutePath);

                //Press Save
                cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);

                //Validate mobile device is displayed on the grid
                cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text, device.DeviceType, "False", true);

                //Click Edit mobile device
                cMyMobileDevicesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text);

                //Initialise search space
                mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

                Assert.AreEqual(device.DeviceName + " EDITED", mobileDeviceControls.NameTxt.Text);
                Assert.IsFalse(mobileDeviceControls.TypeOption.Enabled);
                mobileDeviceControls.NameTxt.Text = device.DeviceName;
                cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);
            }
        }

        /// <summary>
        /// 40292 - Successfully cancel editing mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyCancelEditingMobileDevice_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            //Click Edit mobile device
            cMyMobileDevicesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDevices[0].DeviceName);

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName + " EDITED";

            //Press Cancel
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);

            //Validate mobile device is displayed on the grid
            cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDeviceControls.NameTxt.Text, mobileDevices[0].DeviceType, "False", false);
        }

        /// <summary>
        /// 33179 - Successfully delete paired mobile device, 40287 - Successfully delete unpaired mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyDeleteMobileDevice_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
            {
                //Click Edit mobile device
                cMyMobileDevicesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, device.DeviceName);

                //Validate message in delete window
                if(string.IsNullOrEmpty(device.SerialKey))
                {
                    cMyMobileDevicesMethods.ValidateUnpairedDeviceDeleteMessage();
                }
                else
                {
                    cMyMobileDevicesMethods.ValidatePairedDeviceDeleteMessage();
                }

                //Press Ok
                cMyMobileDevicesMethods.PressOkDeleteMobileDevice();
             
                //Validate mobile device is not displayed on the grid
                cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, device.DeviceName, device.DeviceType, "False", false);
            }
        }

        /// <summary>
        /// 40477 - Successfully cancel deleting paired mobile device, 40478 - Successfully cancel deleting unpaired mobile device
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyCancelDeletingMobileDevice_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            //Click Delete mobile device
            cMyMobileDevicesMethods.ClickDeleteFieldLink(cSharedMethods.browserWindow, mobileDevices[0].DeviceName);

            //Press Cancel
            cMyMobileDevicesMethods.PressCancelDeleteMobileDevice();

            //Validate mobile device is displayed on the grid
            if (string.IsNullOrEmpty(mobileDevices[0].SerialKey))
            {
                cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "False", true);
            }
            else
            {
                cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "True", true);
            }
        }

        /// <summary>
        /// 40319 - Unsuccessfully Add Mobile Device where Duplicate Information is used
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesUnsuccessfullyAddDuplicateMobileDevice_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            cMyMobileDevicesMethods.ClickNewMobileDevice();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;

            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);

            //Validate duplicate modal
            cMyMobileDevicesMethods.ValidateDuplicateMobileDeviceModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = "Message from "+ EnumHelper.GetEnumDescription(_executingProduct) + "\r\n\r\n\r\nThis mobile device has already been added to the system.";
            cMyMobileDevicesMethods.ValidateDuplicateMobileDeviceModal();

            //Press Close
            cMyMobileDevicesMethods.PressCloseDuplicateMobileDeviceModal();
          
            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);
        }

        /// <summary>
        /// 40320 - Unsuccessfully Edit Mobile Device where Duplicate Information is used
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesUnsuccessfullyEditDuplicateMobileDevice_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            cMyMobileDevicesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDevices[0].DeviceName);

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[1].DeviceName;
  
            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);

            //Validate duplicate modal
            cMyMobileDevicesMethods.ValidateDuplicateMobileDeviceModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = "Message from " + EnumHelper.GetEnumDescription(_executingProduct) + "\r\n\r\n\r\nThis mobile device has already been added to the system.";
            cMyMobileDevicesMethods.ValidateDuplicateMobileDeviceModal();

            //Press Close
            cMyMobileDevicesMethods.PressCloseDuplicateMobileDeviceModal();

            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);
        }

        /// <summary>
        /// 33174 - Unsuccessfully Add Mobile Device where Mandatory fields are missing
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesUnsuccessfullyAddMobileDeviceWhereMandatoryFieldsMissing_UITest()
        {
            _runTestCleanup = false;
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            cMyMobileDevicesMethods.ClickNewMobileDevice();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeAsterisk));

            //Populate Name field, leave control, empty field, leave control and verify asterisk is shown 
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SetFocus();
            mobileDeviceControls.NameTxt.Text = "";
            mobileDeviceControls.TypeOption.SetFocus();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeAsterisk));

            //Leave fields empty and Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);

            //Validate duplicate modal
            cMyMobileDevicesMethods.ValidateMandatoryFieldsModal();

            //Press Close
            cMyMobileDevicesMethods.PressCloseMandatoryFieldsModal();

            //Leave Type field empty, save and validate mandatory fields modal
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SetFocus(); // for I.E 11 Compatibility
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;
            mobileDeviceControls.TypeOption.SelectedItem = "[None]";
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeAsterisk));
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);
            cMyMobileDevicesMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText = "Message from " + EnumHelper.GetEnumDescription(_executingProduct) + "\r\n\r\n\r\nPlease select a Type for the mobile device.";
            cMyMobileDevicesMethods.ValidateMandatoryFieldsModal();
            cMyMobileDevicesMethods.PressCloseMandatoryFieldsModal();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk));
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeAsterisk));

            //Leave only Name field empty, save and validate mandatory fields modal
            mobileDeviceControls.NameTxt.Text = "";
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);
            cMyMobileDevicesMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText = "Message from " + EnumHelper.GetEnumDescription(_executingProduct) + "\r\n\r\n\r\nPlease enter a Name for the mobile device.";
            cMyMobileDevicesMethods.ValidateMandatoryFieldsModal();
            cMyMobileDevicesMethods.PressCloseMandatoryFieldsModal();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeAsterisk));

            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);

            //Validate red asterisks do not display next to the mandatory fields in new mobile device modal
            cMyMobileDevicesMethods.ClickNewMobileDevice();
            cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk.WaitForControlExist();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeAsterisk));
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);
        }

        /// <summary>
        /// 33177 - Unsuccessfully edit Mobile Device where Mandatory fields are missing
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesUnsuccessfullyEditMobileDeviceWhereMandatoryFieldsMissing_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            cMyMobileDevicesMethods.ClickEditFieldLink(cSharedMethods.browserWindow, mobileDevices[0].DeviceName);

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            //Validate red asterisks do not display next to the mandatory fields
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeAsterisk));

            //Leave Name field empty and verify asterisk is shown 
            mobileDeviceControls.NameTxt.Text = "";
            cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton.SetFocus();
            Assert.IsTrue(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeAsterisk));

            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);

            //Validate duplicate modal
            cMyMobileDevicesMethods.ValidateMandatoryFieldsModalExpectedValues.UICtl00_pnlMasterPopupPane1InnerText = "Message from " + EnumHelper.GetEnumDescription(_executingProduct) + "\r\n\r\n\r\nPlease enter a Name for the mobile device.";
            cMyMobileDevicesMethods.ValidateMandatoryFieldsModal();

            //Press Close
            cMyMobileDevicesMethods.PressCloseMandatoryFieldsModal();

            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);

            //Validate red asterisks do not display next to the mandatory fields in new mobile device modal
            cMyMobileDevicesMethods.ClickNewMobileDevice();
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UINameAsterisk));
            Assert.IsFalse(ValidationAsterisk.IsValidationAsteriskShown(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITypeAsterisk));
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);
        }

        /// <summary>
        /// 40327 - Successfully verify maximum size for fields on mobile devices modal
        /// 40329 - Successfully verify truncate when copy/paste on fields on mobile devices modal
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyVerifyMaximumSizeOfFieldsOnMobileDevicesModal_UITest()
        {
            _runTestCleanup = false;
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            cMyMobileDevicesMethods.ClickNewMobileDevice();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            //Validate maximum length on Name field
            Assert.AreEqual(100, mobileDeviceControls.NameTxt.MaxLength);

            Clipboard.Clear();
            try { Clipboard.SetText("Hello world I've been pasted here to test the max length of my field. I'm hoping i can fit and earn loads of $$$ and perhaps a photo on the wall of fame! "); }
            catch (Exception) { }

            cSharedMethods.PasteText(mobileDeviceControls.NameTxt);
            Assert.AreEqual(100, mobileDeviceControls.NameTxt.Text.Length);

            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICancelButton);
        }

        /// <summary>
        /// 33180 - Successfully validate Mobile Device status is updated where device is changing from unpaired to paired
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyValidateMobileDeviceStatusFromUnpairedToPaired_UITest()
        {
            _runTestCleanup = true;
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            cMyMobileDevicesMethods.ClickNewMobileDevice();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;

            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);

            //Close the modal
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICloseButton);

            //Validate mobile device is marked as disabled
            cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "False", true);

            //Inser serial key to the database in order to mark the device as paired
            InsertSerialKeyToDevice(mobileDevices[0].SerialKey, mobileDevices[0].DeviceName);
            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            //Validate mobile device is marked as enabled
            cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "True", true);
        }

        /// <summary>
        /// 40824 - Successfully verify message is displayed when disabling receipts where mobile devices are active
        ///</summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MobileDevicesSuccessfullyVerifyMessageWhenDisablingReceiptsWhilstMobileDevicesActive_UITest()
        {
            _runTestCleanup = false;
            //Ensure receipts and mobile devices are enabled 
            cAttachmentTypesMethods.SetReceiptsAndMobileDevices("1", "1");

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/admin/accountOptions.aspx");

            //Disable receipts
            cGeneralOptionsMethods.ClickNewExpensesPage();
            cGeneralOptionsMethods.ClickNewExpensesOtherPreferencesTab();
            cGeneralOptionsMethods.ClickDisableReceipts();

            //Validate modal is displayed
            cMyMobileDevicesMethods.ValidateReceiptsWillBeDisabledForMobileModalExpectedValues.UICtl00_pnlMasterPopupPaneInnerText = "Message from " + EnumHelper.GetEnumDescription(_executingProduct) + "\r\n\r\n\r\nDisabling this option will prevent mobile device users from being able to upload receipts from their device.";
            cMyMobileDevicesMethods.ValidateReceiptsWillBeDisabledForMobileModal();
            cMyMobileDevicesMethods.ClickOKOnReceiptsWillBeDisabledForMobileModal();

            //Press Save
            cGeneralOptionsMethods.PressGeneralOptionsSaveButton();
        }

        /// <summary>
        /// 33172 - Successfully validate My Mobile Devices grid layout (Sorting of grid)
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullySortMyMobileDevicesGrid_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            RestoreDefaultSortingOrder("myMobileDevices");

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            //Validate message when grid does not have any data 
            cMyMobileDevicesMethods.ValidateGridDoesNotHaveAnyData();

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            //Sorts Mobile Devices table by Name column
            HtmlHyperlink displayNameLink = cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITbl_myMobileDevicesTable1.UINameHyperlink;
            cMyMobileDevicesMethods.ClickTableHeader(displayNameLink);
            cMyMobileDevicesMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.Name, EnumHelper.TableSortOrder.DESC, mobileDevices[0].EmployeeID, _executingProduct);
            Thread.Sleep(1000);
            cMyMobileDevicesMethods.ClickTableHeader(displayNameLink);
            cMyMobileDevicesMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.Name, EnumHelper.TableSortOrder.ASC, mobileDevices[0].EmployeeID, _executingProduct);

            //Sorts Mobile Devices table by Type column
            displayNameLink = cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITbl_myMobileDevicesTable1.UITypeHyperlink;
            cMyMobileDevicesMethods.ClickTableHeader(displayNameLink);
            cMyMobileDevicesMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.Type, EnumHelper.TableSortOrder.ASC, mobileDevices[0].EmployeeID, _executingProduct);
            Thread.Sleep(1000);
            cMyMobileDevicesMethods.ClickTableHeader(displayNameLink);
            cMyMobileDevicesMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.Type, EnumHelper.TableSortOrder.DESC, mobileDevices[0].EmployeeID, _executingProduct);

            //Sorts Mobile Devices table by Activation Key column
            displayNameLink = cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UITbl_myMobileDevicesTable1.UIActivationKeyHyperlink;
            cMyMobileDevicesMethods.ClickTableHeader(displayNameLink);
            cMyMobileDevicesMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.PairingKey, EnumHelper.TableSortOrder.ASC, mobileDevices[0].EmployeeID, _executingProduct);
            Thread.Sleep(1000);
            cMyMobileDevicesMethods.ClickTableHeader(displayNameLink);
            cMyMobileDevicesMethods.VerifyCorrectSortingOrderForTable(SortMobileDevicesByColumn.PairingKey, EnumHelper.TableSortOrder.DESC, mobileDevices[0].EmployeeID, _executingProduct);

            RestoreDefaultSortingOrder("myMobileDevices");
        }

        /// <summary>
        /// 40850 - Successfully create mobile device with the same name in different employee accounts
        /// </summary>
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestMethod]
        public void MyMobileDevicesSuccessfullyCreateMobileDeviceWithTheSameNameInDifferenEmployeeAccounts_UITest()
        {
            _runTestCleanup = true;
            string test = testContextInstance.TestName;

            ImportDataToEx_CodedUIDatabase(test);

            cSharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            cMyMobileDevicesMethods.ClickNewMobileDevice();

            //Initialise search space
            MobileDevicesControls mobileDeviceControls = new MobileDevicesControls(cMyMobileDevicesMethods);

            //Populate fields
            mobileDeviceControls.NameTxt.Text = mobileDevices[0].DeviceName;
            mobileDeviceControls.TypeOption.SelectedItem = mobileDevices[0].DeviceType;

            //Press Save
            cSharedMethods.SetFocusOnControlAndPressEnter(cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UISaveButton);

            //Validate Mobile Device is added to the grid
            cMyMobileDevicesMethods.ValidateMyMobileDevicesTable(cSharedMethods.browserWindow, mobileDevices[0].DeviceName, mobileDevices[0].DeviceType, "False", true);
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (_runTestCleanup == true)
            {
                if (mobileDevices != null)
                {
                    switch (TestContext.TestName)
                    { 
                        case "MyMobileDevicesSuccessfullyValidateMobileDeviceStatusFromUnpairedToPaired_UITest":
                        case "MyMobileDevicesUnsuccessfullyAddDuplicateMobileDevice_UITest":
                        case "MyMobileDevicesSuccessfullyCancelEditingMobileDevice_UITest":
                        case "MyMobileDevicesUnsuccessfullyEditMobileDeviceWhereMandatoryFieldsMissing_UITest":
                        case "MyMobileDevicesSuccessfullyCancelDeletingMobileDevice_UITest":
                            mobileDevices[0].MobileDeviceID = MobileDevicesMethods.getDeviceIDByDeviceName(mobileDevices[0].DeviceName, mobileDevices[0].EmployeeID, _executingProduct);
                            if (mobileDevices[0].MobileDeviceID > 0) { MobileDevicesMethods.DeleteMobileDevice(mobileDevices[0].MobileDeviceID, _executingProduct, mobileDevices[0].EmployeeID, null); };
                            mobileDevices[0].MobileDeviceID = 0;
                            break;
                        case "MyMobileDevicesSuccessfullyCreateMobileDeviceWithTheSameNameInDifferenEmployeeAccounts_UITest":
                            mobileDevices[0].MobileDeviceID = MobileDevicesMethods.getDeviceIDByDeviceName(mobileDevices[0].DeviceName, mobileDevices[0].EmployeeID, _executingProduct);
                            if (mobileDevices[0].MobileDeviceID > 0) { MobileDevicesMethods.DeleteMobileDevice(mobileDevices[0].MobileDeviceID, _executingProduct, mobileDevices[0].EmployeeID, null); };
                            mobileDevices[0].MobileDeviceID = 0;
                            MobileDevicesMethods.DeleteMobileDevice(claimantMobileDeviceID, _executingProduct, claimantID, null); 
                            break;
                        case "MyMobileDevicesSuccessfullyDeleteMobileDevice_UITest":
                            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
                            {
                                device.MobileDeviceID = 0;
                            }
                            break;
                        default :
                            foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
                            {
                                device.MobileDeviceID = MobileDevicesMethods.getDeviceIDByDeviceName(device.DeviceName, device.EmployeeID, _executingProduct);
                                if (device.MobileDeviceID > 0) { MobileDevicesMethods.DeleteMobileDevice(device.MobileDeviceID, _executingProduct, device.EmployeeID, null); }
                                device.MobileDeviceID = 0;
                            }
                            break;
                    }
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

            protected MyMobileDevicesUIMap _cMyMobileDevicesMethods;
            protected ControlLocator<HtmlControl> _ControlLocator { get; private set; }

            internal MobileDevicesControls(MyMobileDevicesUIMap cMyMobileDevicesMethods)
            {
                _cMyMobileDevicesMethods = cMyMobileDevicesMethods;
                _ControlLocator = new ControlLocator<HtmlControl>();
                FindControls();
            }

            internal void FindControls()
            {
                NameTxt = (HtmlEdit)_ControlLocator.findControl("ctl00_contentmain_usrMobileDevices_txtDeviceName", new HtmlEdit(_cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICtl00_contentmain_usPane));
                TypeOption = (HtmlComboBox)_ControlLocator.findControl("ctl00_contentmain_usrMobileDevices_ddlDeviceType", new HtmlComboBox(_cMyMobileDevicesMethods.UIMyMobileDevicesWindoWindow.UIMyMobileDevicesDocument.UICtl00_contentmain_usPane));
            }
        }

        /// <summary>
        /// Imports data to the database that the codedui will run
        ///</summary>
        private void ImportDataToEx_CodedUIDatabase(string test)
        {
            MobileDevicesMethods mobileDeviceMethods = new MobileDevicesMethods();
            switch (test)
            {
                case "MyMobileDevicesSuccessfullyCancelEditingMobileDevice_UITest":
                case "MyMobileDevicesUnsuccessfullyAddDuplicateMobileDevice_UITest":
                case "MyMobileDevicesUnsuccessfullyEditMobileDeviceWhereMandatoryFieldsMissing_UITest":
                {
                    mobileDevices[0].MobileDeviceID = mobileDeviceMethods.CreateMobileDevice(mobileDevices[0], _executingProduct);
                    Assert.IsTrue(mobileDevices[0].MobileDeviceID > 0);
                    break;
                }
                case "MyMobileDevicesSuccessfullyCreateMobileDeviceWithTheSameNameInDifferenEmployeeAccounts_UITest":
                {
                    claimantID = AutoTools.GetEmployeeIDByUsername(_executingProduct, true);
                 
                    //Import the first mobile device to the users account 
                    int employeeid = mobileDevices[0].EmployeeID;
                    mobileDevices[0].EmployeeID = claimantID;
                    claimantMobileDeviceID = mobileDeviceMethods.CreateMobileDevice(mobileDevices[0], _executingProduct);
                    Assert.IsTrue(claimantMobileDeviceID > 0);
                    mobileDevices[0].EmployeeID = employeeid;
                    break;
                }
                case "MyMobileDevicesSuccessfullyEditPairedMobileDevice_UITest":
                {
                    foreach (MobileDevicesMethods.MobileDevice device in mobileDevices)
                    {
                        device.SerialKey = mobileDevices[0].SerialKey;
                        device.MobileDeviceID = mobileDeviceMethods.CreateMobileDevice(device, _executingProduct);
                        Assert.IsTrue(device.MobileDeviceID > 0);
                    }
                    break;
                }
                case "MyMobileDevicesSuccessfullyEditUnpairedMobileDevice_UITest":
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
                case "MyMobileDevicesSuccessfullyCancelDeletingMobileDevice_UITest":
                {
                    mobileDevices[0].MobileDeviceID = mobileDeviceMethods.CreateMobileDevice(mobileDevices[0], _executingProduct);
                    Assert.IsTrue(mobileDevices[0].MobileDeviceID > 0);
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
