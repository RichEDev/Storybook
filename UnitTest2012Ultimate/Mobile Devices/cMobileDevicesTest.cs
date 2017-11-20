
namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web.UI.WebControls;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Mobile;

    using Spend_Management;

    using ExpenseItem = Spend_Management.ExpenseItem;

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

            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;

            // ensure that mobile devices are enabled in general options
            cAccountSubAccounts subaccs = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties properties = subaccs.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
            if (properties.UseMobileDevices == false)
            {
                properties.UseMobileDevices = true;
                subaccs.SaveAccountProperties(properties, currentUser.EmployeeID, currentUser.Delegate != null ? currentUser.Delegate.EmployeeID : (int?)null);
            }

            DBConnection db = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));

            // force access role to have permission for mobile devices if it doesn't already
            const string sql = "select top 1 accessRoles.roleID from accessRoles inner join accessRoleElementDetails on accessRoleElementDetails.roleID = accessRoles.roleID where elementID = @elementID"; // find roleID that has mobile devices
            db.sqlexecute.Parameters.AddWithValue("@elementID", (int)SpendManagementElement.MobileDevices);
            int roleID = db.getIntSum(sql);
            if (roleID == 0)
            {
                Assert.Fail("Must have mobile devices added to at least one access role");
            }

            // no role so add
            db.sqlexecute.Parameters.Clear();
            const string insertSQL = "if not exists (select accessRoleID from employeeAccessRoles where accessRoleID = @roleID and employeeID = @employeeID) begin insert into employeeAccessRoles (employeeID, accessRoleID, subAccountID) values (@employeeID, @roleID, @subAccountID); end";
            db.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            db.sqlexecute.Parameters.AddWithValue("@roleID", roleID);
            db.sqlexecute.Parameters.AddWithValue("@subAccountID", currentUser.CurrentSubAccountId);
            db.ExecuteSQL(insertSQL);
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
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(Moqs.CurrentUser(), GlobalVariables.MetabaseConnectionString);

                SpendManagementLibrary.Mobile.ExpenseItem savedItem = devices.getMobileItemByID(-1);

                Assert.IsNull(savedItem);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        /// <summary>
        ///A test for getMobileItemByID
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileItemByIDTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = Moqs.CurrentUser().EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            SpendManagementLibrary.Mobile.ExpenseItem savedItem = null;
            int categoryId = 0;
            int subCatId = 0;
            int itemRoleId = 0;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Debug.WriteLine("### categoryId = " + categoryId.ToString());
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(category.categoryid, subcat: "GetMobileItemByIDTest Subcat"));
                subCatId = subcat.subcatid;
                Debug.WriteLine("### subCatId = " + subCatId.ToString());
                Assert.IsTrue(subCatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, employeeID);
                itemRoleId = itemRole.itemroleid;
                Debug.WriteLine("### itemRoleId = " + itemRoleId.ToString());
                Assert.IsTrue(itemRoleId > 0);

                ExpenseItem mobileItem = MobileExpenseItemObject.Template(subcatid: subcat.subcatid);

                int newItemID = devices.saveMobileItem(employeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.HasValue ? mobileItem.MobileDeviceTypeId.Value : 1, 1, 1);

                Debug.WriteLine("### newItemID = " + newItemID.ToString());
                Assert.IsTrue(newItemID > 0);

                savedItem = devices.getMobileItemByID(newItemID);

                cCompareAssert.AreEqual(mobileItem, savedItem, new List<string> { "mobileID" });
            }
            finally
            {
                if (savedItem != null)
                {
                    MobileExpenseItemObject.TearDown(savedItem.MobileID);
                }
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        /// <summary>
        /// A test for MobileItemHasReceipt
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void MobileItemHasReceiptTrueTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeId = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeId));
            int mobileDeviceId = device.MobileDeviceID;
            SpendManagementLibrary.Mobile.ExpenseItem savedItem = null;
            int categoryId = 0;
            int subCatId = 0;
            int itemRoleId = 0;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId);
                Assert.IsTrue(mobileDeviceId > 0);

                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Debug.WriteLine("### categoryId = " + categoryId);
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(category.categoryid, subcat: "MobileItemHasReceiptTrueTest Subcat"));
                subCatId = subcat.subcatid;
                Debug.WriteLine("### subCatId = " + subCatId);
                Assert.IsTrue(subCatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Debug.WriteLine("### itemRoleId = " + itemRoleId);
                Assert.IsTrue(itemRoleId > 0);

                savedItem = MobileExpenseItemObject.New(MobileExpenseItemObject.Template(subcatid: subCatId), true);
                Assert.IsNotNull(savedItem);

                bool actual = cMobileDevices.MobileItemHasReceipt(currentUser.AccountID, savedItem.MobileID);

                Assert.IsTrue(actual);
            }
            finally
            {
                if (savedItem != null)
                {
                    MobileExpenseItemObject.TearDown(savedItem.MobileID);
                }

                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        /// <summary>
        ///A test for MobileItemHasReceipt
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void MobileItemHasReceiptFalseTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            SpendManagementLibrary.Mobile.ExpenseItem savedItem = null;
            int categoryId = 0;
            int subCatId = 0;
            int itemRoleId = 0;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Debug.WriteLine("### categoryId = " + categoryId.ToString());
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(category.categoryid, subcat: "MobileItemHasReceiptFalseTest Subcat"));
                subCatId = subcat.subcatid;
                Debug.WriteLine("### subCatId = " + subCatId.ToString());
                Assert.IsTrue(subCatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Debug.WriteLine("### itemRoleId = " + itemRoleId.ToString());
                Assert.IsTrue(itemRoleId > 0);

                savedItem = MobileExpenseItemObject.New(MobileExpenseItemObject.Template(subcatid: subCatId));
                Assert.IsNotNull(savedItem);

                bool actual = cMobileDevices.MobileItemHasReceipt(currentUser.AccountID, savedItem.MobileID);

                Assert.IsFalse(actual);
            }
            finally
            {
                if (savedItem != null)
                {
                    MobileExpenseItemObject.TearDown(savedItem.MobileID);
                }
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        /// <summary>
        ///A test for GetMobileDevicesByEmployeeId
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileDevicesByEmployeeIdTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeId = currentUser.EmployeeID, mobileDeviceId1 = 0, mobileDeviceId2 = 0;

            try
            {
                MobileDevice device1 = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeId, deviceName: "Dev1"), currentUser);
                MobileDevice device2 = null;

                Assert.IsNotNull(device1);
                mobileDeviceId1 = device1.MobileDeviceID;
                Debug.WriteLine("### mobileDeviceId1 = " + mobileDeviceId1.ToString());
                Assert.IsTrue(mobileDeviceId1 > 0);

                device2 = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeId, deviceName: "Dev2"), currentUser);
                Assert.IsNotNull(device2);
                Debug.WriteLine("### mobileDeviceId2 = " + mobileDeviceId2.ToString());
                mobileDeviceId2 = device2.MobileDeviceID;
                Assert.IsTrue(mobileDeviceId2 > 0);

                cMobileDevices devs = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                Dictionary<int, MobileDevice> empDevices = devs.GetMobileDevicesByEmployeeId(employeeId);

                Assert.IsNotNull(empDevices);
                Assert.IsTrue(empDevices.Count >= 2);
                Assert.IsTrue(empDevices.Values.Any(o => o.MobileDeviceID == mobileDeviceId1));
                Assert.IsTrue(empDevices.Values.Any(o => o.MobileDeviceID == mobileDeviceId2));
            }
            finally
            {
                if (mobileDeviceId2 > 0)
                {
                    MobileDeviceObject.TearDown(mobileDeviceId2);
                }

                if (mobileDeviceId1 > 0)
                {
                    MobileDeviceObject.TearDown(mobileDeviceId1);
                }
            }
        }

        /// <summary>
        ///A test for GetMobileDeviceById
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileDeviceByIdFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            MobileDevice origDevice = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + origDevice.MobileDeviceID.ToString());
                Assert.IsNotNull(origDevice);

                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                MobileDevice searchDevice = mobileDevices.GetMobileDeviceById(origDevice.MobileDeviceID);

                Assert.IsNotNull(searchDevice);
                cCompareAssert.AreEqual(origDevice, searchDevice);
            }
            finally
            {
                MobileDeviceObject.TearDown(origDevice.MobileDeviceID);
            }
        }

        /// <summary>
        ///A test for GetMobileDeviceById
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileDeviceByIdNotFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;

            try
            {
                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                MobileDevice searchDevice = mobileDevices.GetMobileDeviceById(-1);

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
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            const string testPairingKey = "00123-45678-987654";
            MobileDevice origDevice = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID, pairingKey: testPairingKey));

            try
            {
                Assert.IsNotNull(origDevice);

                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                MobileDevice searchDevice = mobileDevices.GetDeviceByPairingKey(testPairingKey);

                Assert.IsNotNull(searchDevice);
                cCompareAssert.AreEqual(origDevice, searchDevice, new List<string> { "MobileDeviceID" });
            }
            finally
            {
                MobileDeviceObject.TearDown(origDevice.MobileDeviceID);
            }
        }

        /// <summary>
        ///A test for GetDeviceByPairingKey
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetDeviceByPairingKeyNotFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            const string testPairingKey = "00123-45678-987654";

            try
            {
                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                MobileDevice searchDevice = mobileDevices.GetDeviceByPairingKey(testPairingKey);

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
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int subCatId = 0;
            int categoryId = 0;
            int itemRoleId = 0;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices mobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Debug.WriteLine("### categoryId = " + categoryId.ToString());
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(categoryId, subcat: "DeleteMobileItemByIDTest Subcat"));
                subCatId = subcat.subcatid;
                Debug.WriteLine("### subCatId = " + subCatId.ToString());
                Assert.IsTrue(subCatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(rolename: "Mobile Devices Item Role"), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Debug.WriteLine("### itemRoleId = " + itemRoleId.ToString());
                Assert.IsTrue(itemRoleId > 0);

                SpendManagementLibrary.Mobile.ExpenseItem origMobileItem = MobileExpenseItemObject.New(MobileExpenseItemObject.Template(subcatid: subCatId));
                Assert.IsNotNull(origMobileItem);

                SpendManagementLibrary.Mobile.ExpenseItem savedItem = mobileDevices.getMobileItemByID(origMobileItem.MobileID);
                Assert.IsNotNull(savedItem); // check item has saved

                bool successful = mobileDevices.DeleteMobileItemByID(origMobileItem.MobileID);
                Assert.IsTrue(successful);

                SpendManagementLibrary.Mobile.ExpenseItem deleteItem = mobileDevices.getMobileItemByID(origMobileItem.MobileID);
                Assert.IsNull(deleteItem); // ensure deleted item not retrievable from db
            }
            finally
            {
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
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
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                bool deleted = devices.DeleteMobileDevice(mobileDeviceId, currentUser.EmployeeID, null);
                Assert.IsTrue(deleted);

                cMobileDevices devices2 = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                Assert.IsNull(devices2.GetMobileDeviceById(mobileDeviceId));
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileDeviceSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.Template(employeeId: currentUser.EmployeeID);
            int mobileDeviceId = 0;

            try
            {
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                mobileDeviceId = devices.SaveMobileDevice(device, currentUser.EmployeeID);

                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices recachedDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                MobileDevice newDevice = recachedDevices.GetMobileDeviceById(mobileDeviceId);
                Assert.IsNotNull(newDevice);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileDeviceDuplicateTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID, deviceName: "MyDevice"));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                MobileDevice duplicateDevice = MobileDeviceObject.Template(employeeId: currentUser.EmployeeID, deviceName: "MyDevice");
                int duplicateDeviceId = devices.SaveMobileDevice(duplicateDevice, currentUser.EmployeeID);

                Debug.WriteLine("### duplicateDeviceId = " + duplicateDeviceId.ToString());
                Assert.AreEqual(-1, duplicateDeviceId);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
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
                Debug.WriteLine("### pairingKey = " + pairingKey);

                Regex regex = new Regex("^[0-9]{5}-[0-9]{5}-[0-9]{6}$", RegexOptions.Compiled);
                Assert.IsTrue(regex.IsMatch(pairingKey));
                Assert.AreEqual(currentUser.AccountID.ToString("00000"), pairingKey.Substring(0, 5));
                Assert.AreEqual(currentUser.EmployeeID.ToString("000000"), pairingKey.Substring(12, 6));
            }
            catch
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Ensures that a mobile device can be paired
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void PairMobileDeviceSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int mobileDeviceId = 0;

            try
            {
                string deviceName = "UT-PairMobileDeviceSuccess-" + ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString().Substring(5); //random name to avoid collision
                MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID, deviceName: deviceName, serialKey: string.Empty));
                mobileDeviceId = device.MobileDeviceID;

                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                PairingKey pairingKey = new PairingKey(device.PairingKey);
                
                MobileReturnCode returnCode = devices.PairMobileDevice(pairingKey, "be21d28605cf201291c4a2ad0ae93a5f0dfffbbb");
                Debug.WriteLine("### PairMobileDevice return code = " + ((int)returnCode).ToString());
                Assert.AreEqual(MobileReturnCode.Success, returnCode);

                cMobileDevices recachedDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                MobileDevice newDevice = recachedDevices.GetDeviceByPairingKey(pairingKey.Pairingkey);
                Assert.AreEqual(pairingKey.Pairingkey, newDevice.PairingKey);
                Assert.AreEqual("be21d28605cf201291c4a2ad0ae93a5f0dfffbbb", newDevice.SerialKey);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void PairMobileDeviceKeyInUseTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID, deviceName: "MyDevice"));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                PairingKey pairingKey = new PairingKey(device.PairingKey);
                MobileReturnCode returnCode = devices.PairMobileDevice(pairingKey, "be21d28605cf201291c4a2ad0ae93a5f0dfffbbb");
                Debug.WriteLine("### PairMobileDevice return code = " + ((int)returnCode).ToString());
                Assert.AreEqual(MobileReturnCode.PairingKeyInUse, returnCode);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void AuthenticateSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                MobileReturnCode returnCode = devices.authenticate(device.PairingKey, device.SerialKey, currentUser.EmployeeID);
                Debug.WriteLine("### authenticate return code = " + ((int)returnCode).ToString());
                Assert.AreEqual(MobileReturnCode.Success, returnCode);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void AuthenticateFailArchivedTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            var employees = new cEmployees(currentUser.AccountID);
            var employee = employees.GetEmployeeById(currentUser.EmployeeID);
            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                employee.Archived = true;
                employee.Save(currentUser);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                MobileReturnCode returnCode = devices.authenticate(device.PairingKey, device.SerialKey, currentUser.EmployeeID);
                Debug.WriteLine("### authenticate return code = " + ((int)returnCode).ToString());
                Assert.AreEqual(MobileReturnCode.EmployeeArchived, returnCode);
            }
            finally
            {
                // un-archive employee
                if (employee != null)
                {
                    employee.Archived = false;
                    employee.Save(currentUser);
                }
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        public void AuthenticateFailNotActiveTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            cEmployees employees = new cEmployees(currentUser.AccountID);

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                // Need to set an employee to inactive!
                Assert.Fail("Need to set an employee to inactive!");
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        public void AuthenticateFailUnknownTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                MobileReturnCode returnCode = devices.authenticate(device.PairingKey, device.SerialKey, -999);
                Debug.WriteLine("### authenticate return code = " + ((int)returnCode).ToString());
                Assert.AreEqual(MobileReturnCode.EmployeeUnknown, returnCode);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        public void SaveMobileReceiptTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileItemTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;
            int itemRoleId = 0;
            int mobileItemID = 0;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Debug.WriteLine("### categoryId = " + categoryId.ToString());
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(categoryId, subcat: "SaveMobileItemTest Subcat"));
                subCatId = subcat.subcatid;
                Debug.WriteLine("### subCatId = " + subCatId.ToString());
                Assert.IsTrue(subCatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Debug.WriteLine("### itemRoleId = " + itemRoleId.ToString());
                Assert.IsTrue(itemRoleId > 0);

                ExpenseItem mobileItem = MobileExpenseItemObject.Template(mobiledevicetypeid: 2, subcatid: subCatId);

                mobileItemID = devices.saveMobileItem(currentUser.EmployeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.Value, 1, 1);

                Debug.WriteLine("### mobileItemID = " + mobileItemID.ToString());
                Assert.IsTrue(mobileItemID > 0);
            }
            finally
            {
                if (mobileItemID > 0)
                {
                    MobileExpenseItemObject.TearDown(mobileItemID);
                }
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileItemInvalidReasonTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Debug.WriteLine("### categoryId = " + categoryId.ToString());
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(categoryId, subcat: "SaveMobileItemInvalidReasonTest Subcat"));
                subCatId = subcat.subcatid;
                Debug.WriteLine("### subCatId = " + subCatId.ToString());
                Assert.IsTrue(subCatId > 0);

                ExpenseItem mobileItem = MobileExpenseItemObject.Template(reasonid: 999999999, mobiledevicetypeid: 2, subcatid: subCatId);

                int mobileItemID = devices.saveMobileItem(currentUser.EmployeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.Value, 1, 1);
                
                Debug.WriteLine("### mobileItemID = " + mobileItemID.ToString());
                Assert.AreEqual(-5, mobileItemID);
            }
            finally
            {
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileItemInvalidAllowanceTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Debug.WriteLine("### categoryId = " + categoryId.ToString());
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(categoryId, subcat: "SaveMobileItemInvalidAllowanceTest Subcat"));
                subCatId = subcat.subcatid;
                Debug.WriteLine("### subCatId = " + subCatId.ToString());
                Assert.IsTrue(subCatId > 0);

                ExpenseItem mobileItem = MobileExpenseItemObject.Template(allowancetypeid: 999999999, mobiledevicetypeid: 2, subcatid: subCatId);

                int mobileItemID = devices.saveMobileItem(currentUser.EmployeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.Value, 1, 1);

                Debug.WriteLine("### mobileItemID = " + mobileItemID.ToString());
                Assert.AreEqual(-16, mobileItemID);
            }
            finally
            {
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void SaveMobileItemInvalidCurrencyTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;

            try
            {
                Debug.WriteLine("### mobileDeviceId = " + mobileDeviceId.ToString());
                Assert.IsTrue(mobileDeviceId > 0);

                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                cCategory category = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = category.categoryid;
                Debug.WriteLine("### categoryId = " + categoryId.ToString());
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(categoryId, subcat: "SaveMobileItemInvalidCurrencyTest Subcat"));
                subCatId = subcat.subcatid;
                Debug.WriteLine("### subCatId = " + subCatId.ToString());
                Assert.IsTrue(subCatId > 0);

                ExpenseItem mobileItem = MobileExpenseItemObject.Template(currencyid: 999999999, mobiledevicetypeid: 2, subcatid: subCatId);

                int mobileItemID = devices.saveMobileItem(currentUser.EmployeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileItem.MobileDeviceTypeId.Value, 1, 1);

                Debug.WriteLine("### mobileItemID = " + mobileItemID.ToString());
                Assert.AreEqual(-6, mobileItemID);
            }
            finally
            {
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void GetMobileDeviceTypeByIdSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                MobileDeviceType mType = devices.GetMobileDeviceTypeById(1);
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
                MobileDeviceType mType = devices.GetMobileDeviceTypeById(-1);
                Assert.IsNull(mType);
            }
            finally
            {
            }
        }

        /// <summary>
        /// The mobile device operating system type object is valid.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void MobileDeviceOsTypeObjectIsValid()
        {
            string methodName = new StackFrame().GetMethod().Name;
            const int OperatingSystemId = 1234;
            string mobileInstallFrom = methodName;
            const string MobileImage = "image";
            try
            {
                var target = new MobileDeviceOsType(OperatingSystemId, mobileInstallFrom, MobileImage);
                Assert.IsTrue(target != null, "The return value is null, was expecting a valid Object");
                Assert.IsTrue(target.GetType() == typeof(MobileDeviceOsType), string.Format("Wrong object type returned, was expecting {0}, but got {1}", typeof(MobileDeviceOsType), target.GetType()));
                Assert.IsTrue(target.MobileDeviceOsTypeId == OperatingSystemId, string.Format("Device Os Id, was expecting {0} but got {1}", OperatingSystemId, target.MobileDeviceOsTypeId));
                Assert.IsTrue(target.MobileDeviceInstallFrom == mobileInstallFrom, string.Format("Install From, was expecting {0} but got {1}", mobileInstallFrom, target.MobileDeviceInstallFrom));
                Assert.IsTrue(target.MobileDeviceImage == MobileImage, string.Format("Image name, was expecting {0} but got {1}", MobileImage, target.MobileDeviceImage));
            }
            finally
            {
            }
        }

        /// <summary>
        /// The mobile device type object is valid.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void MobileDeviceTypeObjectIsValid()
        {
            string methodName = new StackFrame().GetMethod().Name;
            const int MobiledeviceTypeId = 1234;
            string mobileDeviceTypeDescription = methodName;
            const int MobileDeviceOsTypeId = 4321;
            try
            {
                var target = new MobileDeviceType(MobiledeviceTypeId, mobileDeviceTypeDescription, MobileDeviceOsTypeId);
                Assert.IsTrue(target != null, "The return value is null, was expecting a valid Object");
                Assert.IsTrue(target.GetType() == typeof(MobileDeviceType), string.Format("Wrong object type returned, was expecting {0}, but got {1}", typeof(MobileDeviceType), target.GetType()));
                Assert.IsTrue(target.DeviceTypeId == MobiledeviceTypeId, string.Format("DeviceTypeId , was expecting {0} but got {1}", MobiledeviceTypeId, target.DeviceTypeId));
                Assert.IsTrue(target.DeviceTypeDescription == mobileDeviceTypeDescription, string.Format("DeviceTypeDescription , was expecting {0} but got {1}", mobileDeviceTypeDescription, target.DeviceTypeDescription));
                Assert.IsTrue(target.DeviceOsTypeId == MobileDeviceOsTypeId, string.Format("DeviceOsTypeId, was expecting {0} but got {1}", MobileDeviceOsTypeId, target.DeviceOsTypeId));
            }
            finally
            {
            }
        }

        /// <summary>
        /// The mobile Pairing key object is valid.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void MobileDevicePairingKeyObjectIsValid()
        {
            string methodName = new StackFrame().GetMethod().Name;
            
            string pairingKey = methodName;
            
            try
            {
                var target = new PairingKey(pairingKey);
                Assert.IsTrue(target != null, "The return value is null, was expecting a valid Object");
                Assert.IsTrue(target.GetType() == typeof(PairingKey), string.Format("Wrong object type returned, was expecting {0}, but got {1}", typeof(PairingKey), target.GetType()));
                Assert.IsTrue(target.Pairingkey == pairingKey, string.Format("DeviceTypeId , was expecting {0} but got {1}", pairingKey, target.Pairingkey));
                Assert.IsTrue(target.PairingKeyValid == false, string.Format("Pairing key is valid should be false but returned true"));
            }
            finally
            {
            }
        }

        /// <summary>
        /// Test the 'MobileDeviceTypeOsReader' StoredProcedure
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void MobileDeviceTypeOsReaderReadAll()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevicesBase mobileBase = new cMobileDevices(currentUser.AccountID);
            try
            {
                var privateMobileBase = new PrivateObject(mobileBase);
                var operatingSystemCacheList = (Dictionary<int, MobileDeviceOsType>)privateMobileBase.Invoke("GetMobileDeviceOsTypeListToCache");

                var metabaseConnectionString = GlobalVariables.MetabaseConnectionString;
                var reqDbCon = new DBConnection(metabaseConnectionString);
                var noOfRows = reqDbCon.GetDataSet("select * from mobileDeviceOSTypes");
                
                Assert.IsTrue(operatingSystemCacheList != null, "The return value is null, was expecting a valid Object");
                Assert.IsTrue(noOfRows != null, "Query did not return a valid object");
                Assert.IsTrue(noOfRows.Tables.Count == 1, "No data table returned from query");
                Assert.IsTrue(operatingSystemCacheList.Count == noOfRows.Tables[0].Rows.Count, string.Format("Number of rows, was expecting {0} but got {1}", noOfRows.Tables[0].Rows.Count, operatingSystemCacheList.Count));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Test the 'MobileDeviceTypeReader' StoredProcedure
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("MobileDevices"), TestMethod()]
        public void MobileDeviceTypeReaderReadAll()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevicesBase mobileBase = new cMobileDevices(currentUser.AccountID);
            try
            {
                var privateMobileBase = new PrivateObject(mobileBase);
                var operatingSystemCacheList = (Dictionary<int, MobileDeviceType>)privateMobileBase.Invoke("GetMobileDeviceTypeListToCache");

                var metabaseConnectionString = GlobalVariables.MetabaseConnectionString;
                var reqDbCon = new DBConnection(metabaseConnectionString);
                var noOfRows = reqDbCon.GetDataSet("select * from mobileDeviceTypes");

                Assert.IsTrue(operatingSystemCacheList != null, "The return value is null, was expecting a valid Object");
                Assert.IsTrue(noOfRows != null, "Query did not return a valid object");
                Assert.IsTrue(noOfRows.Tables.Count == 1, "No data table returned from query");
                Assert.IsTrue(operatingSystemCacheList.Count == noOfRows.Tables[0].Rows.Count, string.Format("Number of rows, was expecting {0} but got {1}", noOfRows.Tables[0].Rows.Count, operatingSystemCacheList.Count));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
