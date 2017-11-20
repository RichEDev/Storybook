using System.Text.RegularExpressions;
using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace tempMobileUnitTests
{
    /// <summary>
    ///This is a test class for cMobileDevicesTest and is intended
    ///to contain all cMobileDevicesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cMobileDevicesTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{

        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion


        /// <summary>
        ///A test for getMobileItemByID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileItemByIDNotExistsTest()
        {
            int employeeID = Moqs.CurrentUser().EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(Moqs.CurrentUser(), GlobalVariables.MetabaseConnectionString);

                ExpenseItem savedItem = devices.getMobileItemByID(-1);

                Assert.IsNull(savedItem);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        /// <summary>
        ///A test for getMobileItemByID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileItemByIDTest()
        {
            int employeeID = Moqs.CurrentUser().EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            ExpenseItem savedItem = null;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(Moqs.CurrentUser(), GlobalVariables.MetabaseConnectionString);
                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(category.categoryid));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                ExpenseItem mobileItem = cMobileExpenseItemObject.Template(subcatid: subcat.subcatid);

                int newItemID = devices.saveMobileItem(employeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.HasValue ? mobileItem.MobileDeviceTypeId.Value : 1);

                Assert.IsTrue(newItemID > 0);

                savedItem = devices.getMobileItemByID(newItemID);

                cCompareAssert.AreEqual(mobileItem, savedItem, new List<string> {"mobileID"});
            }
            finally
            {
                if(savedItem != null)
                {
                    cMobileExpenseItemObject.TearDown(savedItem.MobileID);
                }
                cSubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        /// <summary>
        ///A test for MobileItemHasReceipt
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void MobileItemHasReceiptTrueTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            ExpenseItem savedItem = null;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(category.categoryid));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                savedItem = cMobileExpenseItemObject.New(cMobileExpenseItemObject.Template(subcatid: subCatId), true);
                Assert.IsNotNull(savedItem);

                bool actual = cMobileDevices.MobileItemHasReceipt(currentUser.AccountID, savedItem.MobileID);

                Assert.IsTrue(actual);
            }
            finally
            {
                if(savedItem != null)
                {
                    cMobileExpenseItemObject.TearDown(savedItem.MobileID);
                }
                cSubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        /// <summary>
        ///A test for MobileItemHasReceipt
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void MobileItemHasReceiptFalseTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            ExpenseItem savedItem = null;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(category.categoryid));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                savedItem = cMobileExpenseItemObject.New(cMobileExpenseItemObject.Template(subcatid: subCatId));
                Assert.IsNotNull(savedItem);

                bool actual = cMobileDevices.MobileItemHasReceipt(currentUser.AccountID, savedItem.MobileID);

                Assert.IsFalse(actual);
            }
            finally
            {
                if(savedItem != null)
                {
                    cMobileExpenseItemObject.TearDown(savedItem.MobileID);
                }
                cSubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        /// <summary>
        ///A test for GetMobileDevicesByEmployeeId
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileDevicesByEmployeeIdTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device1 = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID, deviceName: "Dev1"));
            int mobileDeviceId1 = device1.MobileDeviceID;
            Assert.IsTrue(mobileDeviceId1 > 0);

            cMobileDevice device2 = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID, deviceName: "Dev2"));
            int mobileDeviceId2 = device2.MobileDeviceID;
            Assert.IsTrue(mobileDeviceId2 > 0);

            try
            {
                Assert.IsTrue(mobileDeviceId1 > 0);
                Assert.IsTrue(mobileDeviceId2 > 0);

                cMobileDevices devs = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                Dictionary<int, cMobileDevice> empDevices = devs.GetMobileDevicesByEmployeeId(employeeID);

                Assert.IsNotNull(empDevices);
                Assert.AreEqual(2, empDevices.Count);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId1);
                cMobileDeviceObject.TearDown(mobileDeviceId2);
            }
        }

        /// <summary>
        ///A test for GetMobileDeviceById
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileDeviceByIdFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice origDevice = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));

            try
            {
                Assert.IsNotNull(origDevice);

                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                cMobileDevice searchDevice = mobileDevices.GetMobileDeviceById(origDevice.MobileDeviceID);

                Assert.IsNotNull(searchDevice);
                cCompareAssert.AreEqual(origDevice, searchDevice);
            }
            finally
            {
                cMobileDeviceObject.TearDown(origDevice.MobileDeviceID);
            }
        }

        /// <summary>
        ///A test for GetMobileDeviceById
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileDeviceByIdNotFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                cMobileDevice searchDevice = mobileDevices.GetMobileDeviceById(-1);

                Assert.IsNull(searchDevice);
            }
            catch
            {
                Assert.Fail();
            }
        }

        /// <summary>
        ///A test for GetDeviceByPairingKey
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetDeviceByPairingKeyFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            const string testPairingKey = "00123-45678-987654";
            cMobileDevice origDevice = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID, pairingKey: testPairingKey));

            try
            {
                Assert.IsNotNull(origDevice);

                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                cMobileDevice searchDevice = mobileDevices.GetDeviceByPairingKey(testPairingKey);

                Assert.IsNotNull(searchDevice);
                cCompareAssert.AreEqual(origDevice, searchDevice, new List<string> { "MobileDeviceID" });
            }
            finally
            {
                cMobileDeviceObject.TearDown(origDevice.MobileDeviceID);
            }
        }

        /// <summary>
        ///A test for GetDeviceByPairingKey
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetDeviceByPairingKeyNotFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            const string testPairingKey = "00123-45678-987654";

            try
            {
                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                cMobileDevice searchDevice = mobileDevices.GetDeviceByPairingKey(testPairingKey);

                Assert.IsNull(searchDevice);
            }
            catch
            {
                Assert.Fail();
            }
        }

        /// <summary>
        ///A test for DeleteMobileItemByID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void DeleteMobileItemByIDTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int subCatId = 0;
            int categoryId = 0;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                ExpenseItem origMobileItem = cMobileExpenseItemObject.New(cMobileExpenseItemObject.Template(subcatid: subCatId));
                Assert.IsNotNull(origMobileItem);

                ExpenseItem savedItem = mobileDevices.getMobileItemByID(origMobileItem.MobileID);
                Assert.IsNotNull(savedItem); // check item has saved

                bool successful = mobileDevices.DeleteMobileItemByID(origMobileItem.MobileID);
                Assert.IsTrue(successful);

                ExpenseItem deleteItem = mobileDevices.getMobileItemByID(origMobileItem.MobileID);
                Assert.IsNull(deleteItem); // ensure deleted item not retrievable from db
            }
            finally
            {
                cSubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void CreateDropDownTestWithoutNoneOption()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            
            try
            {
                DropDownList lst = new DropDownList();
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                devices.CreateDropDown(ref lst, false);

                ListItem item = lst.Items.FindByValue("0");
                Assert.IsNull(item); // None option should not exist
            }
            finally
            {

            }
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void CreateDropDownTestWithNoneOption()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            
            try
            {
                DropDownList lst = new DropDownList();
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                devices.CreateDropDown(ref lst, true);

                ListItem item = lst.Items.FindByValue("0");
                Assert.IsNotNull(item); // None option should exist
            }
            finally
            {

            }
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void CreateDropDownTestWithPreSelection()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            
            try
            {
                DropDownList lst = new DropDownList();
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                devices.CreateDropDown(ref lst, true, 3); // preselect mobile device type 3

                Assert.IsNotNull(lst.SelectedItem); // Pre selected item should exist
                Assert.AreEqual("3", lst.SelectedItem.Value);
            }
            finally
            {
                
            }
        }

        /// <summary>
        /// Delete Mobile Device Test
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void DeleteMobileDeviceTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                bool deleted = devices.DeleteMobileDevice(mobileDeviceId, currentUser.EmployeeID, null);
                Assert.IsTrue(deleted);

                cMobileDevices devices2 = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                Assert.IsNull(devices2.GetMobileDeviceById(mobileDeviceId));
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileDeviceSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID);
            int mobileDeviceId = 0;

            try
            {
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                mobileDeviceId = devices.SaveMobileDevice(device, currentUser.EmployeeID);

                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices recachedDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cMobileDevice newDevice = recachedDevices.GetMobileDeviceById(mobileDeviceId);
                Assert.IsNotNull(newDevice);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileDeviceDuplicateTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID, deviceName: "MyDevice"));
            int mobileDeviceId = device.MobileDeviceID;
            
            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cMobileDevice duplicateDevice = cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID, deviceName: "MyDevice");
                int duplicateDeviceId = devices.SaveMobileDevice(duplicateDevice, currentUser.EmployeeID);

                Assert.AreEqual(-1, duplicateDeviceId);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GeneratePairingKeyTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                string pairingKey = devices.GeneratePairingKey(currentUser.EmployeeID);

                Regex regex = new Regex("^[0-9]{5}-[0-9]{5}-[0-9]{6}$", RegexOptions.Compiled);
                Assert.IsTrue(regex.IsMatch(pairingKey));
                Assert.AreEqual(currentUser.AccountID.ToString("00000"), pairingKey.Substring(0,5));
                Assert.AreEqual(currentUser.EmployeeID.ToString("000000"), pairingKey.Substring(12,6));
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void PairMobileDeviceSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID, deviceName: "MyDevice", serialKey: ""));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cPairingKey pairingKey = new cPairingKey(device.PairingKey);
                MobileReturnCode returnCode = devices.PairMobileDevice(pairingKey, "be21d28605cf201291c4a2ad0ae93a5f0dfffbbb");
                Assert.AreEqual(MobileReturnCode.Success, returnCode);

                cMobileDevices recachedDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cMobileDevice newDevice = recachedDevices.GetDeviceByPairingKey(pairingKey.PairingKey);
                Assert.AreEqual(pairingKey.PairingKey, newDevice.PairingKey);
                Assert.AreEqual("be21d28605cf201291c4a2ad0ae93a5f0dfffbbb", newDevice.SerialKey);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void PairMobileDeviceKeyInUseTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID, deviceName: "MyDevice"));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cPairingKey pairingKey = new cPairingKey(device.PairingKey);
                MobileReturnCode returnCode = devices.PairMobileDevice(pairingKey, "be21d28605cf201291c4a2ad0ae93a5f0dfffbbb");
                Assert.AreEqual(MobileReturnCode.PairingKeyInUse, returnCode);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void AuthenticateSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                MobileReturnCode returnCode = devices.authenticate(device.PairingKey, device.SerialKey, currentUser.EmployeeID);
                Assert.AreEqual(MobileReturnCode.Success, returnCode);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void AuthenticateFailArchivedTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            cEmployees employees = new cEmployees(currentUser.AccountID);
            
            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                employees.changeStatus(currentUser.EmployeeID, true);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                MobileReturnCode returnCode = devices.authenticate(device.PairingKey, device.SerialKey, currentUser.EmployeeID);
                Assert.AreEqual(MobileReturnCode.EmployeeArchived, returnCode);
            }
            finally
            {
                // un-archive employee
                employees.changeStatus(currentUser.EmployeeID, false);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        public void AuthenticateFailNotActiveTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            cEmployees employees = new cEmployees(currentUser.AccountID);

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                // Need to set an employee to inactive!
                Assert.Fail("Need to set an employee to inactive!");
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        public void AuthenticateFailUnknownTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                MobileReturnCode returnCode = devices.authenticate(device.PairingKey, device.SerialKey, -999);
                Assert.AreEqual(MobileReturnCode.EmployeeUnknown, returnCode);
            }
            finally 
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileReceiptTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileItemTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                ExpenseItem mobileItem = cMobileExpenseItemObject.Template(mobiledevicetypeid: 2, subcatid: subCatId);

                int mobileItemID = devices.saveMobileItem(currentUser.EmployeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.Value);

                Assert.IsTrue(mobileItemID > 0);
            }
            finally 
            {
                cSubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileItemInvalidReasonTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                ExpenseItem mobileItem = cMobileExpenseItemObject.Template(reasonid: 999999999, mobiledevicetypeid: 2, subcatid: subCatId);

                int mobileItemID = devices.saveMobileItem(currentUser.EmployeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.Value);

                Assert.AreEqual(-5, mobileItemID);
            }
            finally
            {
                cSubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileItemInvalidAllowanceTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                
                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                ExpenseItem mobileItem = cMobileExpenseItemObject.Template(allowancetypeid: 999999999, mobiledevicetypeid: 2, subcatid: subCatId);

                int mobileItemID = devices.saveMobileItem(currentUser.EmployeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.Value);

                Assert.AreEqual(-16, mobileItemID);
            }
            finally
            {
                cSubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileItemInvalidCurrencyTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                ExpenseItem mobileItem = cMobileExpenseItemObject.Template(currencyid: 999999999, mobiledevicetypeid: 2, subcatid: subCatId);

                int mobileItemID = devices.saveMobileItem(currentUser.EmployeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.Value);

                Assert.AreEqual(-6, mobileItemID);
            }
            finally
            {
                cSubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileDeviceTypeByIdSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cMobileDeviceType mType = devices.GetMobileDeviceTypeById(1);
                Assert.IsNotNull(mType);

                Assert.AreEqual(1, mType.DeviceTypeId);
                Assert.AreEqual("iphone", mType.DeviceTypeDescription.ToLower());
            }
            finally
            {
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileDeviceTypeByIdFailTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cMobileDeviceType mType = devices.GetMobileDeviceTypeById(-1);
                Assert.IsNull(mType);
            }
            finally
            {
            }
        }


        // Can't test this method as it turns a binary db record into a file and saves it to a folder on the web site.

        ///// <summary>
        /////A test for saveReceiptFromMobile
        /////</summary>
        //[TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        //public void saveReceiptFromMobileTest()
        //{
        //    int accountID = 0; // TODO: Initialize to an appropriate value
        //    cMobileDevices target = new cMobileDevices(accountID); // TODO: Initialize to an appropriate value
        //    int mobileID = 0; // TODO: Initialize to an appropriate value
        //    int expenseID = 0; // TODO: Initialize to an appropriate value
        //    int claimID = 0; // TODO: Initialize to an appropriate value
        //    target.saveReceiptFromMobile(mobileID, expenseID, claimID);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}
    }
}
