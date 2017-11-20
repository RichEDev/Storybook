using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using Spend_Management;

namespace tempMobileUnitTests
{
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
        }

        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetEmployeeSubCatsTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetReceiptByIdFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int expItemId = 0;
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;
                
                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId=expCategory.categoryid;
                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId: expCategory.categoryid));
                subcatId =subcat.subcatid;

                // create an expense item
                cExpenseItem newItem = cExpenseItemObject.New(cExpenseItemObject.Template(subcat.subcatid, claim.claimid, total: Convert.ToDecimal((11.53))));
                expItemId = newItem.expenseid;

                // how do we add a receipt?
                cExpenseItems clsExpItems = new cExpenseItems(currentUser.AccountID, currentUser.EmployeeID);
                clsExpItems.attachreceipt(newItem, "receipts/unit_test_receipt.jpg", currentUser.EmployeeID);
                
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ReceiptResult result = api.GetReceiptByID(device.PairingKey, device.SerialKey, expItemId);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetReceiptByID", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsNotNull(result.Receipt);
                Assert.AreEqual("success", result.Message);
            }
            finally
            {
                cClaims clsClaims = new cClaims(currentUser.AccountID);
                cClaim claim = clsClaims.getClaimById(claimId);
                cExpenseItem item = claim.getExpenseItemById(expItemId);

                if (item != null)
                {
                    cExpenseItems clsExpItems = new cExpenseItems(currentUser.AccountID, currentUser.EmployeeID);
                    clsExpItems.deleteReceipt(item);
                }
                cExpenseItemObject.TearDown(claimId, expItemId);
                cSubcatObject.TearDown(subcatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cClaimObject.TearDown(claimId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetReceiptByIdNotFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                // create expense with receipt
                //Assert.Inconclusive();

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ReceiptResult result = api.GetReceiptByID(device.PairingKey, device.SerialKey, -1);
                
                Assert.IsNotNull(result);
                Assert.AreEqual("GetReceiptByID", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.AreEqual("", result.Receipt);
                Assert.AreEqual("file not found", result.Message);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetSubcatListTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            List<int> mobileExpenseIds = new List<int>();
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;

                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCategory.categoryid;
                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId: expCategory.categoryid));
                subcatId = subcat.subcatid;
                
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

                cClaims clsClaims = new cClaims(currentUser.AccountID);
                cClaim claim = clsClaims.getClaimById(claimId);
                cSubcatObject.TearDown(subcatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cClaimObject.TearDown(claimId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseInvalidReasonTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;

                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCategory.categoryid;
                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId: expCategory.categoryid));
                subcatId = subcat.subcatid;

                List<ExpenseItem> items = new List<ExpenseItem>
                {
                    new ExpenseItem
                    {
                        dtDate = DateTime.UtcNow,
                        Total = Convert.ToDecimal(2.34),
                        SubcatID = subcatId,
                        ReasonID = -1,
                        OtherDetails = "UT Mobile Expense",
                        ItemNotes = "UT Mobile Expense item notes"
                    }
                };

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                AddExpenseItemResult result = api.SaveExpense(device.PairingKey, device.SerialKey, items);

                Assert.IsNotNull(result);

                Assert.AreEqual("SaveExpense", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.ReasonIsInvalid, result.ReturnCode);
            }
            finally
            {
                cClaims clsClaims = new cClaims(currentUser.AccountID);
                cClaim claim = clsClaims.getClaimById(claimId);
                cSubcatObject.TearDown(subcatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cClaimObject.TearDown(claimId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseInvalidCurrencyTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;

                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCategory.categoryid;
                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId: expCategory.categoryid));
                subcatId = subcat.subcatid;

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
                Assert.AreEqual(MobileReturnCode.CurrencyIsInvalid, result.ReturnCode);
            }
            finally
            {
                cClaims clsClaims = new cClaims(currentUser.AccountID);
                cClaim claim = clsClaims.getClaimById(claimId);
                cSubcatObject.TearDown(subcatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cClaimObject.TearDown(claimId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseInvalidAllowanceTypeTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;
            int claimId = 0;
            int subcatId = 0;
            int categoryId = 0;

            try
            {
                // Create a category and subcat
                cClaim claim = cClaimObject.New(cClaimObject.Template(employeeid: currentUser.EmployeeID));
                claimId = claim.claimid;

                cCategory expCategory = cExpenseCategoryObject.New(cExpenseCategoryObject.Template());
                categoryId = expCategory.categoryid;
                cSubcat subcat = cSubcatObject.New(cSubcatObject.Template(categoryId: expCategory.categoryid));
                subcatId = subcat.subcatid;

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
                Assert.AreEqual(MobileReturnCode.AllowanceIsInvalid, result.ReturnCode);
            }
            finally
            {
                cClaims clsClaims = new cClaims(currentUser.AccountID);
                cClaim claim = clsClaims.getClaimById(claimId);
                cSubcatObject.TearDown(subcatId);
                cExpenseCategoryObject.TearDown(categoryId);
                cClaimObject.TearDown(claimId);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void SaveExpenseInvalidSubcatTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
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
                Assert.AreEqual(MobileReturnCode.SubCatIsInvalid, result.ReturnCode);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetEmployeeBasicDetailsTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void PairDeviceSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID, serialKey: ""));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void PairDeviceReusePairingKeyTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void PairDevicePairingKeyInUseTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void ValidatePairingKeyInvalidFormatTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            
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
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
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

            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void ValidatePairingKeyAccountInvalidTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                string invalidPairingKey = "99999" + WcfServiceObject.GeneratePairingKey(currentUser.EmployeeID).Substring(6);
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
            ICurrentUser currentUser = Moqs.CurrentUser();
            cAccountSubAccounts subaccs = new cAccountSubAccounts(currentUser.AccountID);
            cAccountProperties properties = subaccs.getFirstSubAccount().SubAccountProperties;

            try
            {
                // disable mobile devices
                properties.UseMobileDevices = false;
                subaccs.SaveAccountProperties(properties, currentUser.EmployeeID);

                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.ValidatePairingKey(WcfServiceObject.GeneratePairingKey(currentUser.EmployeeID), null);

                Assert.IsNotNull(result);
                Assert.AreEqual("ValidatePairingKey", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.MobileDevicesDisabled, result.ReturnCode);
            }
            finally
            {
                properties.UseMobileDevices = true;
                subaccs.SaveAccountProperties(properties, currentUser.EmployeeID);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void ValidatePairingKeyNotFoundTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            
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
            ICurrentUser currentUser = Moqs.CurrentUser();
           cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
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

            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetExpenseItemCategoriesTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetReasonsListTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                string pairingKey = WcfServiceObject.GeneratePairingKey(currentUser.EmployeeID);
                ReasonResult result = api.GetReasonsList(device.PairingKey, device.SerialKey);

                Assert.IsNotNull(result);
                Assert.AreEqual("GetReasonsList", result.FunctionName);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.IsTrue(result.List.Count > 0);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetGeneralOptionsTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetCurrencyListTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void AuthenticateEmployeeUnknownTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                string pairingKey = WcfServiceObject.GeneratePairingKey(99999999);
                ServiceResultMessage result = api.Authenticate(pairingKey, "be21d28605cf201291c4a2ad0ae93a5f0daaabbb");

                Assert.AreEqual(MobileReturnCode.EmployeeUnknown, result.ReturnCode);
                Assert.AreEqual("Authenticate", result.FunctionName);
            }
            finally
            {
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void AuthenticateEmployeeArchivedTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cEmployees emps = new cEmployees(currentUser.AccountID);
            int employeeID = currentUser.EmployeeID;
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: employeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                // archive employee
                emps.changeStatus(currentUser.EmployeeID, true);
                
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.Authenticate(device.PairingKey, device.SerialKey);

                Assert.AreEqual(MobileReturnCode.EmployeeArchived, result.ReturnCode);
                Assert.AreEqual("Authenticate", result.FunctionName);
            }
            finally
            {
                emps.changeStatus(currentUser.EmployeeID, false);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void AuthenticateSuccessTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.Authenticate(device.PairingKey, device.SerialKey);

                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.AreEqual("Authenticate", result.FunctionName);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void AuthenticateFailTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;

            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                ServiceResultMessage result = api.Authenticate(device.PairingKey, "be21d28605cf201291c4a2ad0ae93a5f0daaabbf");

                Assert.AreEqual(MobileReturnCode.AuthenticationFailed, result.ReturnCode);
                Assert.AreEqual("Authenticate", result.FunctionName);
            }
            finally
            {
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void UploadReceiptTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
            int mobileDeviceId = device.MobileDeviceID;
            ExpenseItem mobileExpenseItem = cMobileExpenseItemObject.New(cMobileExpenseItemObject.Template(otherdetails: "Test Expense", total: (decimal)9.87, date: DateTime.UtcNow));
            
            try
            {
                MobileAPI api = WcfServiceObject.GetMobileAPI(currentUser);
                UploadReceiptResult result = api.UploadReceipt(device.PairingKey, device.SerialKey, mobileExpenseItem.MobileID, "This is a string to be encoded as though it were a receipt image");

                Assert.IsNotNull(result);
                Assert.AreEqual(MobileReturnCode.Success, result.ReturnCode);
                Assert.AreEqual("UploadReceipt", result.FunctionName);
                Assert.AreEqual(mobileExpenseItem.MobileID, result.MobileID);
            }
            finally
            {
                cMobileExpenseItemObject.TearDown(mobileExpenseItem.MobileID);
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetClaimsAwaitingApprovalTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetClaimsAwaitingApprovalCountTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("MobileSvc"), TestMethod]
        public void GetExpenseItemsByClaimIDTest()
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevice device = cMobileDeviceObject.New(cMobileDeviceObject.Template(employeeId: currentUser.EmployeeID));
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
                cMobileDeviceObject.TearDown(mobileDeviceId);
            }
        }
    }
}