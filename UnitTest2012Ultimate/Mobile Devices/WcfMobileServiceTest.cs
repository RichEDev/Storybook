namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Mobile;

    using Spend_Management;

    using AddExpenseItemResult = Spend_Management.AddExpenseItemResult;
    using AddExpensesScreenDetails = Spend_Management.AddExpensesScreenDetails;
    using CategoryResult = Spend_Management.CategoryResult;
    using ClaimToCheckCountResult = Spend_Management.ClaimToCheckCountResult;
    using ClaimToCheckResult = Spend_Management.ClaimToCheckResult;
    using CurrencyResult = Spend_Management.CurrencyResult;
    using EmployeeBasic = Spend_Management.EmployeeBasic;
    using ExpenseItem = Spend_Management.ExpenseItem;
    using ExpenseItemResult = Spend_Management.ExpenseItemResult;
    using ExpenseItemsResult = Spend_Management.ExpenseItemsResult;
    using GeneralOptions = Spend_Management.GeneralOptions;
    using ReasonResult = Spend_Management.ReasonResult;
    using ServiceResultMessage = Spend_Management.ServiceResultMessage;
    using SubcatResult = Spend_Management.SubcatResult;
    using UploadReceiptResult = Spend_Management.UploadReceiptResult;

    /// <summary>
    /// Summary description for WcfMobileServiceTest
    /// </summary>
    [TestClass]
    public class WcfMobileServiceTest
    {
        public WcfMobileServiceTest()
        {
           
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();

            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;

            // ensure that mobile devices are enabled in general options
            cAccountSubAccounts subaccs = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties properties = subaccs.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
            if(properties.UseMobileDevices == false)
            {
                properties.UseMobileDevices = true;
                subaccs.SaveAccountProperties(properties, currentUser.EmployeeID, currentUser.Delegate != null ? currentUser.Delegate.EmployeeID : (int?)null);
            }

            DBConnection db = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));

            // force access role to have permission for mobile devices if it doesn't already
            const string sql = "select top 1 accessRoles.roleID from accessRoles inner join accessRoleElementDetails on accessRoleElementDetails.roleID = accessRoles.roleID where elementID = @elementID"; // find roleID that has mobile devices
            db.sqlexecute.Parameters.AddWithValue("@elementID", (int) SpendManagementElement.MobileDevices);
            int roleID = db.getIntSum(sql);
            if(roleID == 0)
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

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            DBConnection db = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));

            // clear out any mobile devices for the user
            db.sqlexecute.Parameters.Clear();
            db.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            db.ExecuteSQL("delete from mobileDevices where employeeID = @employeeID");
            var employees = new cEmployees(GlobalTestVariables.AccountId);
            var testEmployee = employees.GetEmployeeById(GlobalTestVariables.EmployeeId);
            if (testEmployee != null)
            {
                if (testEmployee.Archived)
                {
                    testEmployee.Archived = false;
                    testEmployee.Save(currentUser);
                }
            }
        }
        
        // Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup() 
        //{
        //}
        

        #endregion

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetEmployeeSubCatsTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ExpenseItemResult result = api.GetEmployeeSubCats(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetEmployeeSubCats", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsNotNull(result.List);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetSubcatListTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                SubcatResult result = api.GetSubcatList(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetSubcatList", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsNotNull(result.List);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            List<int> mobileExpenseIds = new List<int>();
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;
            int itemRoleId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;
                Assert.IsTrue(claimId > 0);

                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCategory.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(expCategory.categoryid, subcat: "SaveExpenseSuccessTest Subcat"));
                subcatId = subcat.subcatid;
                Assert.IsTrue(subcatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Assert.IsTrue(itemRoleId > 0);

                List<ExpenseItem> items = new List<ExpenseItem>
                {
                    new ExpenseItem
                    {
                        dtDate = DateTime.UtcNow,
                        Total = Convert.ToDecimal(2.34),
                        SubcatID = subcatId,
                        OtherDetails = "UT Mobile Expense",
                        ItemNotes = "UT Mobile Expense item notes"
                    }
                };

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                AddExpenseItemResult result = api.SaveExpense(device.PairingKey, device.SerialKey, items);

                Assert.IsNotNull(result);
                mobileExpenseIds = (from x in result.List.Values
                                    where x > 0
                                    select x).ToList();

                Assert.AreEqual("SaveExpense", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsNotNull(result.List);
                foreach(KeyValuePair<int, int> kvp in result.List)
                {
                    Assert.IsTrue(kvp.Value > 0); // negative return code means expense failed to add
                }
            }
            finally
            {
                cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                foreach(int mobileExpenseId in mobileExpenseIds)
                {
                    devices.DeleteMobileItemByID(mobileExpenseId);
                }

                cClaimObject.TearDown(claimId);
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subcatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseInvalidReasonTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;
            int itemRoleId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;
                Assert.IsTrue(claimId > 0);

                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCategory.categoryid;
                Assert.IsTrue(categoryId > 0);
                
                cSubcat subcat = SubcatObject.New(SubcatObject.Template(expCategory.categoryid, subcat: "SaveExpenseInvalidReasonTest Subcat"));
                subcatId = subcat.subcatid;
                Assert.IsTrue(subcatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Assert.IsTrue(itemRoleId > 0);

                List<ExpenseItem> items = new List<ExpenseItem>
                {
                    new ExpenseItem
                    {
                        dtDate = DateTime.UtcNow,
                        Total = Convert.ToDecimal(2.34),
                        SubcatID = subcatId,
                        ReasonID = 999999999,
                        OtherDetails = "UT Mobile Expense",
                        ItemNotes = "UT Mobile Expense item notes"
                    }
                };

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                AddExpenseItemResult result = api.SaveExpense(device.PairingKey, device.SerialKey, items);

                Assert.IsNotNull(result);

                Assert.AreEqual("SaveExpense", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.ReasonIsInvalid, (MobileReturnCode)result.List[0]);
            }
            finally
            {
                cClaimObject.TearDown(claimId);
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subcatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseInvalidCurrencyTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;
            int itemRoleId = 0;
            int expenseId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;
                Assert.IsTrue(claimId > 0);

                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCategory.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(expCategory.categoryid, subcat: "SaveExpenseInvalidCurrencyTest Subcat"));
                subcatId = subcat.subcatid;
                Assert.IsTrue(subcatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Assert.IsTrue(itemRoleId > 0);

                List<ExpenseItem> items = new List<ExpenseItem>
                {
                    new ExpenseItem
                    {
                        dtDate = DateTime.UtcNow,
                        Total = Convert.ToDecimal(2.34),
                        SubcatID = subcatId,
                        CurrencyID = -1,
                        OtherDetails = "UT Mobile Expense",
                        ItemNotes = "UT Mobile Expense item notes"
                    }
                };

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                
                AddExpenseItemResult result = api.SaveExpense(device.PairingKey, device.SerialKey, items);

                Assert.IsNotNull(result);

                Assert.AreEqual("SaveExpense", result.FunctionName);
                expenseId = (int)result.ReturnCode;
                Assert.AreEqual(MobileReturnCode.CurrencyIsInvalid, (MobileReturnCode)result.List[0]);

            }
            finally
            {
                cExpenseItemObject.TearDown(claimId, expenseId);
                SubcatObject.TearDown(subcatId);
                cClaimObject.TearDown(claimId);
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                MobileDeviceObject.TearDown(mobileDeviceId);
                cExpenseCategoryObject.TearDown(categoryId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseInvalidAllowanceTypeTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;
            int itemRoleId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;
                Assert.IsTrue(claimId > 0);

                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCategory.categoryid;
                Assert.IsTrue(categoryId > 0);
                
                cSubcat subcat = SubcatObject.New(SubcatObject.Template(expCategory.categoryid, subcat: "SaveExpenseInvalidAllowanceTypeTest Subcat"));
                subcatId = subcat.subcatid;
                Assert.IsTrue(subcatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Assert.IsTrue(itemRoleId > 0);

                List<ExpenseItem> items = new List<ExpenseItem>
                {
                    new ExpenseItem
                    {
                        dtDate = DateTime.UtcNow,
                        Total = Convert.ToDecimal(2.34),
                        SubcatID = subcatId,
                        AllowanceTypeID = -1,
                        OtherDetails = "UT Mobile Expense",
                        ItemNotes = "UT Mobile Expense item notes"
                    }
                };

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                AddExpenseItemResult result = api.SaveExpense(device.PairingKey, device.SerialKey, items);

                Assert.IsNotNull(result);

                Assert.AreEqual("SaveExpense", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.AllowanceIsInvalid, (MobileReturnCode)result.List[0]);
            }
            finally
            {
                cClaimObject.TearDown(claimId);
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subcatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseInvalidSubcatTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;
            int itemRoleId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;
                Assert.IsTrue(claimId > 0);

                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCategory.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(expCategory.categoryid, subcat: "SaveExpenseInvalidSubcatTest Subcat"));
                subcatId = subcat.subcatid;
                Assert.IsTrue(subcatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Assert.IsTrue(itemRoleId > 0);

                List<ExpenseItem> items = new List<ExpenseItem>
                {
                    new ExpenseItem
                    {
                        dtDate = DateTime.UtcNow,
                        Total = Convert.ToDecimal(2.34),
                        SubcatID = 9999999,
                        OtherDetails = "UT Mobile Expense",
                        ItemNotes = "UT Mobile Expense item notes"
                    }
                };

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                AddExpenseItemResult result = api.SaveExpense(device.PairingKey, device.SerialKey, items);

                Assert.IsNotNull(result);

                Assert.AreEqual("SaveExpense", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.SubCatIsInvalid, (MobileReturnCode)result.List[0]);
            }
            finally
            {
                cClaimObject.TearDown(claimId);
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subcatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetEmployeeBasicDetailsTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                EmployeeBasic result = api.GetEmployeeBasicDetails(device.PairingKey, device.SerialKey, currentUser.EmployeeID);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetEmployeeBasicDetails", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void PairDeviceSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID, serialKey: "", pairingKey: WcfServiceObject.GeneratePairingKey(employeeID)));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.PairDevice(device.PairingKey, "be21d28605cf201291c4a2ad0ae93a5f0deeabee");

                Assert.IsNotNull(result);
                Assert.AreEqual("PairDevice", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.AreEqual("success", result.Message);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void PairDeviceReusePairingKeyTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.PairDevice(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("PairDevice", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.AreEqual("success", result.Message);

            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void PairDevicePairingKeyInUseTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                
                ServiceResultMessage result = api.PairDevice(device.PairingKey, "be21d28605cf201291c4a2ad0ae93a5f0deeabee");
                
                Assert.IsNotNull(result);
                Assert.AreEqual("PairDevice", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.PairingKeyInUse, result.ReturnCode);
                Assert.AreEqual("fail", result.Message);

            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void ValidatePairingKeyInvalidFormatTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            
            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);

                ServiceResultMessage result = api.ValidatePairingKey("11-12345-000121", null);

                Assert.IsNotNull(result);
                Assert.AreEqual("ValidatePairingKey", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.PairingKeyFormatInvalid, result.ReturnCode);
            }
            finally
            {
            
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void ValidatePairingKeySerialKeyMismatchTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);

                ServiceResultMessage result = api.ValidatePairingKey(device.PairingKey, "be21d28605cf201291c4a2ad0ae93a5f0eeeeeee");

                Assert.IsNotNull(result);
                Assert.AreEqual("ValidatePairingKey", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.PairingKeySerialKeyMismatch, result.ReturnCode);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void ValidatePairingKeyAccountInvalidTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                string invalidPairingKey = "99999" + WcfServiceObject.GeneratePairingKey(currentUser.EmployeeID).Substring(5);
                ServiceResultMessage result = api.ValidatePairingKey(invalidPairingKey, null);

                Assert.IsNotNull(result);
                Assert.AreEqual("ValidatePairingKey", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.AccountInvalid, result.ReturnCode);
            }
            finally
            {

            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void ValidatePairingKeyMobileDevicesDisabledTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            cAccountSubAccounts subaccs = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties properties = subaccs.getFirstSubAccount().SubAccountProperties;

            try
            {
                // disable mobile devices
                properties.UseMobileDevices = false;
                subaccs.SaveAccountProperties(properties, currentUser.EmployeeID, currentUser.Delegate != null ? currentUser.Delegate.EmployeeID : (int?)null);

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.ValidatePairingKey(WcfServiceObject.GeneratePairingKey(currentUser.EmployeeID), null);

                Assert.IsNotNull(result);
                Assert.AreEqual("ValidatePairingKey", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.MobileDevicesDisabled, result.ReturnCode);
            }
            finally
            {
                properties.UseMobileDevices = true;
                subaccs.SaveAccountProperties(properties, currentUser.EmployeeID, currentUser.Delegate != null ? currentUser.Delegate.EmployeeID : (int?)null);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void ValidatePairingKeyNotFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            
            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.ValidatePairingKey(WcfServiceObject.GeneratePairingKey(currentUser.EmployeeID), null);

                Assert.IsNotNull(result);
                Assert.AreEqual("ValidatePairingKey", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.PairingKeyNotFound, result.ReturnCode);
            }
            finally
            {

            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetAddEditExpensesScreenSetupTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                AddExpensesScreenDetails result = api.GetAddEditExpensesScreenSetup(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetAddEditExpensesScreenSetup", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.AreEqual(3, result.Fields.Count);
                Assert.IsTrue(result.Fields.ContainsKey("reason"));
                Assert.IsTrue(result.Fields.ContainsKey("currency"));
                Assert.IsTrue(result.Fields.ContainsKey("otherdetails"));
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetExpenseItemCategoriesTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                CategoryResult result = api.GetExpenseItemCategories(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetExpenseItemCategories", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsTrue(result.List.Count > 0);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetReasonsListTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ReasonResult result = api.GetReasonsList(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetReasonsList", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsTrue(result.List.Count > 0);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetGeneralOptionsTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                GeneralOptions result = api.GetGeneralOptions(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetGeneralOptions", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetCurrencyListTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                CurrencyResult result = api.GetCurrencyList(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetCurrencyList", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsTrue(result.List.Count > 0);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void AuthenticateEmployeeArchivedTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            cEmployees emps = new cEmployees(currentUser.AccountID);
            int employeeID = currentUser.EmployeeID;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;
            int itemRoleId = 0;
            var employee = emps.GetEmployeeById(employeeID);

            try
            {
                cCategory expCat = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCat.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(categoryId, subcat: "AuthenticateEmployeeArchivedTest Subcat"));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);
                
                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Assert.IsTrue(itemRoleId > 0);

                // archive employee
                
                employee.Archived = true;
                employee.Save(currentUser);
                
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.Authenticate(device.PairingKey, device.SerialKey);

                Assert.AreEqual(MobileReturnCode.EmployeeArchived, result.ReturnCode);
                Assert.AreEqual("Authenticate", result.FunctionName);
            }
            finally
            {
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);

                if (employee != null)
                {
                    employee.Archived = true;
                    employee.Save(currentUser);
                }
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void AuthenticateSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int categoryId = 0;
            int subCatId = 0;
            int itemRoleId = 0;

            try
            {
                cCategory expCat = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCat.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(categoryId, subcat: "AuthenticateSuccessTest Subcat"));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Assert.IsTrue(itemRoleId > 0);

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.Authenticate(device.PairingKey, device.SerialKey);

                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.AreEqual("Authenticate", result.FunctionName);
            }
            finally
            {
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void UploadReceiptTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int mobileExpenseItemId = 0;
            int categoryId = 0;
            int subCatId = 0;
            int itemRoleId = 0;

            try
            {
                cCategory expCat = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCat.categoryid;
                Assert.IsTrue(categoryId > 0);

                cSubcat subcat = SubcatObject.New(SubcatObject.Template(categoryId, subcat: "UploadReceiptTest Subcat"));
                subCatId = subcat.subcatid;
                Assert.IsTrue(subCatId > 0);

                cItemRole itemRole = cItemRoleObject.New(cItemRoleObject.Template(), subcat, currentUser.EmployeeID);
                itemRoleId = itemRole.itemroleid;
                Assert.IsTrue(itemRoleId > 0);

                SpendManagementLibrary.Mobile.ExpenseItem mobileExpenseItem = MobileExpenseItemObject.New(MobileExpenseItemObject.Template(subcatid: subCatId, otherdetails: "Test Expense", total: (decimal)9.87, date: DateTime.UtcNow));
                mobileExpenseItemId = mobileExpenseItem.MobileID;
                Assert.IsTrue(mobileExpenseItemId > 0);

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                byte[] byteArray = encoding.GetBytes("This is a string to be encoded as though it were a receipt image");
                UploadReceiptResult result = api.UploadReceipt(device.PairingKey, device.SerialKey, mobileExpenseItem.MobileID, Convert.ToBase64String(byteArray));

                Assert.IsNotNull(result);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.AreEqual("UploadReceipt", result.FunctionName);
                Assert.AreEqual(mobileExpenseItem.MobileID, result.MobileID);
            }
            finally
            {
                if (mobileExpenseItemId > 0)
                {
                    MobileExpenseItemObject.TearDown(mobileExpenseItemId);
                }
                cItemRoleObject.TearDown(currentUser.EmployeeID, itemRoleId);
                SubcatObject.TearDown(subCatId);
                cExpenseCategoryObject.TearDown(categoryId);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetClaimsAwaitingApprovalTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ClaimToCheckResult result = api.GetClaimsAwaitingApproval(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetClaimsAwaitingApproval", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsNotNull(result.List);
            }
            finally 
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetClaimsAwaitingApprovalCountTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ClaimToCheckCountResult result = api.GetClaimsAwaitingApprovalCount(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetClaimsAwaitingApprovalCount", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsNotNull(result.Count);
            }
            finally
            {
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetExpenseItemsByClaimIDTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUserMobileDevicesMock().Object;
            MobileDevice device = MobileDeviceObject.New(MobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ExpenseItemsResult result = api.GetExpenseItemsByClaimID(device.PairingKey, device.SerialKey, claim.claimid);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetExpenseItemsByClaimID", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsNotNull(result.List);
            }
            finally
            {
                cClaimObject.TearDown(claim.claimid);
                MobileDeviceObject.TearDown(mobileDeviceId);
            }
        }


        /// <summary>
        /// The get device type Operating System details.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod()]
        public void GetDeviceTypeOsDetails()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                var devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                Assert.IsTrue(devices.MobileDeviceOsTypes.Count != 0, "No Device Operating System Types found in mobileDeviceOSTypes table in metabase so cannot continue");
                var mobileDevicesService = new svcMobileDevices();

                // Check that every Device Type has a valid DeviceType OS
                foreach (KeyValuePair<int, MobileDeviceType> keyValuePair in devices.MobileDeviceTypes)
                {
                    string[] returnValues = mobileDevicesService.GetMobileActivateHelpText(keyValuePair.Value.DeviceOsTypeId);
                    Assert.IsNotNull(returnValues);
                    Assert.IsTrue(returnValues.GetLength(0) == 2, string.Format("Two items expected from device id {0}, os id {1} but returned {2} items", keyValuePair.Value.DeviceTypeId, keyValuePair.Value.DeviceOsTypeId, returnValues.GetLength(0)));
                }
            }
            finally
            {
            }
        }
    }
}