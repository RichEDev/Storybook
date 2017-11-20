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
using Auto_Tests.UIMaps.MyMobileDevicesAccessRoleUIMapClasses;
using Auto_Tests.Tools;


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Access_Roles.MyMobileDevicesAccessRoles
{
    [CodedUITest]
    public class MyMobileDevicesAccessRoleUITest
    {
        private string _accessRoleElement = "Mobile Devices";
        private string _accessRole = "MyMobileDevicesClaimantAccessRole";
        private int _mobiledeviceonlyaccessroleID { get; set; }
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();
        private MyMobileDevicesAccessRoleUIMap _myMobileDevicesAccessRoleMethods = new MyMobileDevicesAccessRoleUIMap();
        private readonly static ProductType _executingProduct = ProductType.expenses;
        private int _claimantID = MyMobileDevicesAccessRoleMethods.GetEmployeeIDByUsername(EnumHelper.GetEnumDescription(EmployeeType.Claimant), _executingProduct);
        private int _adminID = MyMobileDevicesAccessRoleMethods.GetEmployeeIDByUsername(EnumHelper.GetEnumDescription(EmployeeType.Administrator), _executingProduct);
        private static int accountId;

        public MyMobileDevicesAccessRoleUITest()
        {
            _sharedMethods = new SharedMethodsUIMap();
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.CloseOnPlaybackCleanup = false;
            browser.Maximized = true;
            _sharedMethods.Logon(_executingProduct, LogonType.administrator);
            accountId = AutoTools.GetAccountID(_executingProduct);
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            _sharedMethods.CloseBrowserWindow();
        }

        #region 40336 Successfully view My Mobile devices page when access role is applied
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Access Roles"), TestMethod]
        public void MyMobileDevicesAccessRoleSuccessfullyGrantClaimantAccessRoleToMyMobileDevices_UITest()
        {
            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);
            CacheUtilities.DeleteCachedEmployeeAccessRoles(accountId, _claimantID.ToString());

            #region login as claimant and assert no access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");
 
            _myMobileDevicesAccessRoleMethods.ValidateNoPermissionsPage();
            #endregion

            #region login as administrator and grant access
            _sharedMethods.NavigateToPage(_executingProduct, "/home.aspx");

            _sharedMethods.ClickExitExpensesPane();

            _sharedMethods.Logon(_executingProduct, LogonType.administrator);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/accessRoles.aspx");

            _myMobileDevicesAccessRoleMethods.ClickEditFieldLink(_accessRole);

            _myMobileDevicesAccessRoleMethods.ClickAddViewAccessRole(_accessRoleElement);

            _myMobileDevicesAccessRoleMethods.PressSaveAccessRole();

            _sharedMethods.NavigateToPage(_executingProduct, "/home.aspx");

            _sharedMethods.ClickExitExpensesPane();
            #endregion

            #region login as claimant and assert access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidatePermissionsToMobileDevicesPage();
            #endregion

        }
        #endregion

        #region MyMobileDevicesAccessRole Unsuccessfully Grant Claimant AccessRole To My Mobile Devices
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Access Roles"), TestMethod]
        public void MyMobileDevicesAccessRoleUnsuccessfullyGrantClaimantAccessRoleToMyMobileDevices_UITest()
        {
            CacheUtilities.DeleteCachedEmployeeAccessRoles(accountId, _claimantID.ToString());

            #region login as claimant and assert no access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidateNoPermissionsPage();
            #endregion

            #region login as administrator and fail to grant access by clicking cancel
            _sharedMethods.NavigateToPage(_executingProduct, "/home.aspx");

            _sharedMethods.ClickExitExpensesPane();

            _sharedMethods.Logon(_executingProduct, LogonType.administrator);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/accessRoles.aspx");

            _myMobileDevicesAccessRoleMethods.ClickEditFieldLink(_accessRole);

            _myMobileDevicesAccessRoleMethods.ClickAddViewAccessRole(_accessRoleElement);

            _myMobileDevicesAccessRoleMethods.PressCancelSavingAccessRole();

            _sharedMethods.NavigateToPage(_executingProduct, "/home.aspx");

            _sharedMethods.ClickExitExpensesPane();
            #endregion

            #region login as claimant and assert no access still
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidateNoPermissionsPage();
            #endregion

        }
        #endregion

        #region 40338  Unsuccessfully view My Mobile devices page when access role without permissions is applied
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Access Roles"), TestMethod]
        public void MyMobileDevicesAccessRoleSuccessfullyDenyClaimantAccessRoleToMyMobileDevices_UITest()
        {
            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);
            CacheUtilities.DeleteCachedEmployeeAccessRoles(accountId, _claimantID.ToString());

            #region login as claimant and assert access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidatePermissionsToMobileDevicesPage();
            #endregion

            #region login as administrator and deny access
            _sharedMethods.NavigateToPage(_executingProduct, "/home.aspx");

            _sharedMethods.ClickExitExpensesPane();

            _sharedMethods.Logon(_executingProduct, LogonType.administrator);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/accessRoles.aspx");

            _myMobileDevicesAccessRoleMethods.ClickEditFieldLink(_accessRole);

            _myMobileDevicesAccessRoleMethods.ClickAddViewAccessRole(_accessRoleElement);

            _myMobileDevicesAccessRoleMethods.PressSaveAccessRole();

            _sharedMethods.NavigateToPage(_executingProduct, "/home.aspx");

            _sharedMethods.ClickExitExpensesPane();
            #endregion

            #region login as claimant and assert no access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidateNoPermissionsPage();
            #endregion

        }
        #endregion

        #region MyMobileDevicesAccessRole Unsuccessfully Deny Claimant AccessRole To My Mobile Devices
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Access Roles"), TestMethod]
        public void MyMobileDevicesAccessRoleUnsuccessfullyDenyClaimantAccessRoleToMyMobileDevices_UITest()
        {
            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);
            CacheUtilities.DeleteCachedEmployeeAccessRoles(accountId, _claimantID.ToString());

            #region login as claimant and assert access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidatePermissionsToMobileDevicesPage();
            #endregion

            #region login as administrator and fail to deny access by clicking cancel
            _sharedMethods.NavigateToPage(_executingProduct, "/home.aspx");

            _sharedMethods.ClickExitExpensesPane();

            _sharedMethods.Logon(_executingProduct, LogonType.administrator);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/admin/accessRoles.aspx");

            _myMobileDevicesAccessRoleMethods.ClickEditFieldLink(_accessRole);

            _myMobileDevicesAccessRoleMethods.ClickAddViewAccessRole(_accessRoleElement);

            _myMobileDevicesAccessRoleMethods.PressCancelSavingAccessRole();

            _sharedMethods.NavigateToPage(_executingProduct, "/home.aspx");

            _sharedMethods.ClickExitExpensesPane();
            #endregion

            #region login as claimant and assert access still
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidatePermissionsToMobileDevicesPage();
            #endregion

        }
        #endregion

        #region  40339 Successfully verify My Mobile Devices page is not accessible when logging in with a user where an access role is not set
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Access Roles"), TestMethod]
        public void MyMobileDevicesAccessRoleUnsuccessfullyViewMyMobileDevicesWhereNoAccessRoleIsApplied_UITest()
        {
            CacheUtilities.DeleteCachedEmployeeAccessRoles(accountId, _claimantID.ToString());

            #region login as claimant and assert no access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidateNoPermissionsPage();
            #endregion

        }
        #endregion

        #region MyMobileDevicesAccessRole Successfully Verify Access To My Mobile Devices When Enable Mobile Devices Is Set To True
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Access Roles"), TestMethod]
        public void MyMobileDevicesAccessRoleSuccessfullyVerifyAccessToMyMobiledevicesWhenEnableMobileDevicesIsSetToTrue_UITest()
        {
            MobileDevicesMethods.SetMobileDevices("0", _executingProduct);
            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);
            CacheUtilities.DeleteCachedEmployeeAccessRoles(accountId, _claimantID.ToString());

            #region login as claimant with access role and verify no access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidateNoPermissionsPage();
            #endregion

            #region turn on general options mobile devices functionality
            MobileDevicesMethods.SetMobileDevices("1", _executingProduct);
            #endregion

            #region login as claimant with access role and verify access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidatePermissionsToMobileDevicesPage();
            #endregion
        }
        #endregion

        #region 40345 Unsuccessfully view My Mobile Devices page when mobile devices option is disabled
        [TestCategory("Mobile Devices"), TestCategory("Expenses"), TestCategory("Access Roles"), TestMethod]
        public void MyMobileDevicesAccessRoleSuccessfullyVerifyNoAccessToMyMobiledevicesWhenEnableMobileDevicesIsSetToFalse_UITest()
        {
            ImportFormDataToEx_CodedUIDatabase(testContextInstance.TestName);
            CacheUtilities.DeleteCachedEmployeeAccessRoles(accountId, _claimantID.ToString());

            #region login as claimant with access role and verify access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidatePermissionsToMobileDevicesPage();
            #endregion

            #region turn off general options mobile devices functionality
            MobileDevicesMethods.SetMobileDevices("0", _executingProduct);
            #endregion

            #region login as claimant with access role and verify no access
            _sharedMethods.Logon(_executingProduct, LogonType.claimant);

            _sharedMethods.NavigateToPage(_executingProduct, "/shared/information/MyMobileDevices.aspx");

            _myMobileDevicesAccessRoleMethods.ValidateNoPermissionsPage();
            #endregion
        }
        #endregion

        #region Additional test attributes


        [TestInitialize()]
        public void MyTestInitialize()
        {
            _mobiledeviceonlyaccessroleID = MyMobileDevicesAccessRoleMethods.CreateMobileDevicesAccessRole(_accessRole, _claimantID, _executingProduct);
            MobileDevicesMethods.SetMobileDevices("1", _executingProduct);
        }


        [TestCleanup()]
        public void MyTestCleanup()
        {
            MyMobileDevicesAccessRoleMethods.DeleteMobileDevicesAccessRole(_mobiledeviceonlyaccessroleID, _executingProduct);
        }

        #endregion

        private void ImportFormDataToEx_CodedUIDatabase(string test)
        {

            switch (test)
            {
                case "MyMobileDevicesAccessRoleSuccessfullyGrantClaimantAccessRoleToMyMobileDevices_UITest":
                case "MyMobileDevicesAccessRoleUnsuccessfullyGrantClaimantAccessRoleToMyMobileDevices_UITest":
                    MyMobileDevicesAccessRoleMethods.SetMobileDevicesAccessRoleElement(_mobiledeviceonlyaccessroleID, 0, _executingProduct);
                    MyMobileDevicesAccessRoleMethods.AssignClaimantMobileDevicesAccessRole(_claimantID, _mobiledeviceonlyaccessroleID, _executingProduct);
                    break;
                case "MyMobileDevicesAccessRoleSuccessfullyDenyClaimantAccessRoleToMyMobileDevices_UITest":
                case "MyMobileDevicesAccessRoleUnsuccessfullyDenyClaimantAccessRoleToMyMobileDevices_UITest":
                case "MyMobileDevicesAccessRoleSuccessfullyVerifyAccessToMyMobiledevicesWhenEnableMobileDevicesIsSetToTrue_UITest":
                case "MyMobileDevicesAccessRoleSuccessfullyVerifyNoAccessToMyMobiledevicesWhenEnableMobileDevicesIsSetToFalse_UITest":
                    MyMobileDevicesAccessRoleMethods.SetMobileDevicesAccessRoleElement(_mobiledeviceonlyaccessroleID, 1, _executingProduct);
                    MyMobileDevicesAccessRoleMethods.AssignClaimantMobileDevicesAccessRole(_claimantID, _mobiledeviceonlyaccessroleID, _executingProduct);
                    break;
                default:
                    break;
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
