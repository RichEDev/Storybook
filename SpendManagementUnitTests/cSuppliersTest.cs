using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cSuppliersTest and is intended
    ///to contain all cSuppliersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cSuppliersTest
    {
        private TestContext testContextInstance;

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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void cSuppliersTest_MyTestInitialize()
        {
            cSupplierObject.CreateSupplier();
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void cSuppliersTest_MyTestCleanup()
        {
            if (cGlobalVariables.SupplierID > 0)
            {
                cSupplierObject.DeleteSupplier();
            }
            if (cGlobalVariables.SupplierCategoryID > 0)
            {
                cSupplierObject.DeleteSupplierCategory();
            }
            if (cGlobalVariables.SupplierStatusID > 0)
            {
                cSupplierObject.DeleteSupplierStatus();
            }
            if (cGlobalVariables.FinancialStatusID > 0)
            {
                cSupplierObject.DeleteFinancialStatus();
            }
        }
        #endregion


        /// <summary>
        ///A test for getSupplierById
        ///</summary>
        [TestMethod()]
        public void cSuppliersTest_getSupplierByIdTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cSuppliers target = new cSuppliers(accountid, subaccountid);

            cSupplier actual = target.getSupplierById(cGlobalVariables.SupplierID);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for DeleteSupplier
        ///</summary>
        [TestMethod()]
        public void cSuppliersTest_DeleteSupplierTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cSuppliers target = new cSuppliers(accountid, subaccountid);
            int supplierId = cGlobalVariables.SupplierID;
            target.DeleteSupplier(supplierId);

            cSupplier actual = target.getSupplierById(cGlobalVariables.SupplierID);
            Assert.IsNull(actual);

            cGlobalVariables.SupplierID = 0;
        }

        ///// <summary>
        /////A test for SupplierExists
        /////</summary>
        //[TestMethod()]
        //public void cSuppliersTest_SupplierExistsTest()
        //{
        //    int accountid = cGlobalVariables.AccountID;
        //    int subaccountid = cGlobalVariables.SubAccountID;
        //    cSuppliers target = new cSuppliers(accountid, subaccountid);
        //    cSupplier globalSupplier = target.getSupplierById(cGlobalVariables.SupplierID);
        //    string supplier_name = globalSupplier.SupplierName; ;
        //    bool expected = true;
        //    bool actual;
        //    actual = target.SupplierExists(supplier_name);
        //    Assert.AreEqual(expected, actual);

        //    actual = target.SupplierExists("**ABCDEF**");
        //    Assert.AreNotEqual(expected, actual);
        //}

        /// <summary>
        ///A test for UpdateSupplier for update of existing supplier
        ///</summary>
        [TestMethod()]
        public void cSuppliersTest_UpdateSupplierTest_Update()
        {    
            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTables clsTables = new cTables(accountid);

            cSuppliers target = new cSuppliers(accountid, subaccountid);
            cSupplier globalSupplier = target.getSupplierById(cGlobalVariables.SupplierID);
            cSupplierStatus newStatus = cSupplierObject.CreateSupplierStatus(false);
            cSupplierCategory newCategory = cSupplierObject.CreateSupplierCategory();

            cFinancialStatus newFStatus = cSupplierObject.CreateFinancialStatus();
            cCurrencies currencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cAddress newAddress = new cAddress(globalSupplier.PrimaryAddress.AddressId, "ADDR_TITLE", "ADDR1", "ADDR2", "TOWN", "COUNTY", "PCODE", 0, "01234 567890", "00000 000000", false, DateTime.Now, cGlobalVariables.EmployeeID, DateTime.Now, cGlobalVariables.EmployeeID);

            cSupplier amendedSupplier = new cSupplier(globalSupplier.SupplierId, globalSupplier.subAccountId, globalSupplier.SupplierName + "_AMENDED", newStatus, newCategory, globalSupplier.SupplierCode + "_AMENDED", newAddress, globalSupplier.WebURL + "_AMENDED", 5, newFStatus, currencies.getCurrencyByAlphaCode("GBP").currencyid, 777.77, 777, globalSupplier.SupplierContacts, null, null, string.Empty, string.Empty, false, false);
            
            int actual = target.UpdateSupplier(amendedSupplier);

            cSupplier expected = target.getSupplierById(cGlobalVariables.SupplierID);
            cCompareAssert.AreEqual(amendedSupplier, expected); // check that amended supplier is equal to the one retrieved from db after save
            cCompareAssert.AreEqual(amendedSupplier.PrimaryAddress, expected.PrimaryAddress);
            cCompareAssert.AreEqual(amendedSupplier.subAccountId, expected.SupplierCategory);
            cCompareAssert.AreEqual(amendedSupplier.SupplierStatus, expected.SupplierStatus);
        }

        /// <summary>
        ///A test for UpdateSupplier for update of existing supplier
        ///</summary>
        [TestMethod()]
        public void cSuppliersTest_UpdateSupplierTest_Add()
        {
            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cSuppliers target = new cSuppliers(accountid, subaccountid);

            cAddress newAddress = new cAddress(0, "ADDR_TITLE", "ADDR1", "ADDR2", "TOWN", "COUNTY", "PCODE", 0, "01234 567890", "H", false, DateTime.Now, cGlobalVariables.EmployeeID, DateTime.Now, cGlobalVariables.EmployeeID);

            cSupplierObject.DeleteSupplier(); // clean up the default created supplier

            cSupplierStatus newStatus = cSupplierObject.CreateSupplierStatus(false);
            cSupplierCategory newCategory = cSupplierObject.CreateSupplierCategory();
            
            cFinancialStatus newFStatus = cSupplierObject.CreateFinancialStatus();
            cCurrencies currencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            cSupplier newsupplier = new cSupplier(0, cGlobalVariables.SubAccountID, "NEWSUPPLIER", newStatus, newCategory, "ABC123", newAddress, "http://www.abc.com", 1, newFStatus, currencies.getCurrencyByAlphaCode("GBP").currencyid, 999.88, 333, null, null, null, string.Empty, string.Empty, false, false);
            cGlobalVariables.SupplierID = target.UpdateSupplier(newsupplier);

            cSupplier expected_supplier = target.getSupplierById(cGlobalVariables.SupplierID);

            // check supplier fields
            Assert.IsTrue(expected_supplier.SupplierId > 0);
            Assert.IsTrue(expected_supplier.AnnualTurnover == 999.88);
            Assert.IsTrue(expected_supplier.FinancialYearEnd == 1);
            cCompareAssert.AreEqual(expected_supplier.LastFinancialStatus, newFStatus);
            Assert.IsTrue(expected_supplier.NumberOfEmployees == 333);
            Assert.IsTrue(expected_supplier.subAccountId == cGlobalVariables.SubAccountID);
            cCompareAssert.AreEqual(expected_supplier.SupplierCategory, newCategory);
            Assert.IsTrue(expected_supplier.SupplierCode == "ABC123");
            Assert.IsTrue(expected_supplier.SupplierName == "NEWSUPPLIER");
            cCompareAssert.AreEqual(expected_supplier.SupplierStatus, newStatus);
            Assert.IsTrue(expected_supplier.TurnoverCurrencyId == currencies.getCurrencyByAlphaCode("GBP").currencyid);
            Assert.IsTrue(expected_supplier.WebURL == "http://www.abc.com");
            
            // check address
            Assert.IsTrue(newsupplier.PrimaryAddress.AddressId > 0);
            Assert.IsTrue(newsupplier.PrimaryAddress.AddressLine1 == "ADDR1");
            Assert.IsTrue(newsupplier.PrimaryAddress.AddressLine2 == "ADDR2");
            Assert.IsTrue(newsupplier.PrimaryAddress.AddressTitle == "ADDR_TITLE");
            Assert.IsTrue(newsupplier.PrimaryAddress.County == "COUNTY");
            Assert.IsTrue(newsupplier.PrimaryAddress.CreatedById == cGlobalVariables.EmployeeID);
            Assert.IsTrue(newsupplier.PrimaryAddress.IsPrivateAddress == false);
            Assert.IsTrue(newsupplier.PrimaryAddress.LastModifiedById == cGlobalVariables.EmployeeID);
            Assert.IsTrue(newsupplier.PrimaryAddress.PostCode == "PCODE");
            Assert.IsTrue(newsupplier.PrimaryAddress.Switchboard == "01234 567890");
            Assert.IsTrue(newsupplier.PrimaryAddress.Town == "TOWN");
        }


        ///// <summary>
        /////A test for getListItems
        /////</summary>
        //[TestMethod()]
        //public void cSuppliersTest_getListItemsTest()
        //{
        //    int accountid = cGlobalVariables.AccountID;
        //    int subaccountid = cGlobalVariables.SubAccountID;
        //    cSuppliers target = new cSuppliers(accountid, subaccountid);

        //    ListItem[] expected = target.getListItems(false);

        //    Assert.IsTrue(expected.Length > 0);
        //    foreach (ListItem li in expected)
        //    {
        //        if (li.Text == "[None]" || li.Value == "0")
        //        {
        //            Assert.Fail("getListItems() returned [None] entry incorrectly");
        //        }
        //    }

        //    ListItem[] expectedWithNone = target.getListItems(true);
        //    bool found = false;
        //    Assert.IsTrue(expectedWithNone.Length > 0);

        //    foreach (ListItem li in expectedWithNone)
        //    {
        //        if (li.Text == "[None]" || li.Value == "0")
        //        {
        //            found = true;
        //            break;
        //        }
        //    }

        //    if (!found)
        //    {
        //        Assert.Fail("getListItems() doesn't include the [None] entry");
        //    }
        //}

        /// <summary>
        ///A test for getListItemsForContractAdd
        ///</summary>
        [TestMethod()]
        public void cSuppliersTest_getListItemsForContractAddTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cSuppliers target = new cSuppliers(accountid, subaccountid);

            cSupplierObject.DeleteSupplier(); // clean up the default created supplier

            cSupplier supplier = cSupplierObject.CreateSupplierWithCategoriesAndStatus(true);

            ListItem[] expected = target.getListItemsForContractAdd(false);

            foreach (ListItem li in expected)
            {
                if (li.Text == "[None]" || li.Value == "0")
                {
                    Assert.Fail("getListItemsForContractAdd() returned [None] entry incorrectly");
                }
                if (li.Text == "NEWSUPPLIER" || li.Value == cGlobalVariables.SupplierID.ToString())
                {
                    Assert.Fail("getListItemsForContractAdd() contained supplier when deny_contract_add property equals 1");
                }
            }

            ListItem[] expectedWithNone = target.getListItemsForContractAdd(true);
            bool found = false;
            Assert.IsTrue(expectedWithNone.Length > 0);

            foreach (ListItem li in expectedWithNone)
            {                
                if (li.Text == "[None]" || li.Value == "0")
                {
                    found = true;
                }

                if (li.Text == "NEWSUPPLIER" || li.Value == cGlobalVariables.SupplierID.ToString())
                {
                    Assert.Fail("getListItemsForContractAdd() contained supplier when deny_contract_add property equals 1");
                }
            }

            if (!found)
            {
                Assert.Fail("getListItemsForContractAdd() doesn't include the [None] entry");
            }
            found = false;
            bool supplierFound = false;

            cSupplierObject.DeleteSupplier();
            supplier = cSupplierObject.CreateSupplierWithCategoriesAndStatus(false);

            expected = target.getListItemsForContractAdd(false);

            foreach (ListItem li in expected)
            {
                if (li.Text == "[None]" || li.Value == "0")
                {
                    Assert.Fail("getListItemsForContractAdd() returned [None] entry incorrectly");
                }
                if (li.Text == "NEWSUPPLIER" || li.Value == cGlobalVariables.SupplierID.ToString())
                {
                    supplierFound = true;        
                }
            }

            if (!supplierFound)
            {
                Assert.Fail("getListItemsForContractAdd() did not contain supplier when deny_contract_add property equals 0 and includeNone is false");
            }


            supplierFound = false;
            expectedWithNone = target.getListItemsForContractAdd(true);

            foreach (ListItem li in expectedWithNone)
            {
                if (li.Text == "[None]" || li.Value == "0")
                {
                    found = true;
                }

                if (li.Text == "NEWSUPPLIER" || li.Value == cGlobalVariables.SupplierID.ToString())
                {
                    supplierFound = true;
                }
            }

            if (!found)
            {
                Assert.Fail("getListItemsForContractAdd() doesn't include the [None] entry");
            }
            if (!supplierFound)
            {
                Assert.Fail("getListItemsForContractAdd() did not contain supplier when deny_contract_add property equals 0 and includeNone is true");
            }
        }

        /// <summary>
        ///A test for getSupplierByName
        ///</summary>
        [TestMethod()]
        public void cSuppliersTest_getSupplierByNameTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cSuppliers target = new cSuppliers(accountid, subaccountid);

            string suppliername = "Unit Test Supplier " + DateTime.Now.ToShortDateString();
            
            cSupplier actual = target.getSupplierByName(suppliername);
            Assert.IsNotNull(actual);
            Assert.AreEqual(suppliername, actual.SupplierName);
            Assert.AreEqual(actual.SupplierId, cGlobalVariables.SupplierID);
        }

        ///// <summary>
        /////A test for getSuppliersByCategory
        /////</summary>
        //[TestMethod()]
        //public void cSuppliersTest_getSuppliersByCategoryTest()
        //{
        //    int accountid = cGlobalVariables.AccountID;
        //    int subaccountid = cGlobalVariables.SubAccountID;
        //    cSuppliers target = new cSuppliers(accountid, subaccountid);
        //    cSupplierCategory category = cSupplierObject.CreateSupplierCategory();
            
        //    // for default supplier created, should not be returned from function
        //    Dictionary<int, cSupplier> actual = target.getSuppliersByCategory(category);
        //    foreach (KeyValuePair<int, cSupplier> kvp in actual)
        //    {
        //        if (kvp.Key == cGlobalVariables.SupplierID)
        //        {
        //            Assert.Fail("getSuppliersByCategory() returned supplier with null category");
        //        }
        //    }

        //    // create supplier with category set
        //    cSupplierObject.DeleteSupplier();
        //    cSupplierObject.DeleteSupplierCategory();
        //    category = cSupplierObject.CreateSupplierWithCategoriesAndStatus(false).SupplierCategory;
        //    actual = target.getSuppliersByCategory(category);

        //    bool supplierFound = false;

        //    foreach (KeyValuePair<int, cSupplier> kvp in actual)
        //    {
        //        if (kvp.Key == cGlobalVariables.SupplierID)
        //        {
        //            supplierFound = true;
        //            break;
        //        }
        //    }

        //    if (!supplierFound)
        //    {
        //        Assert.Fail("getSuppliersByCategory() did not return supplier with category in list");
        //    }
        //}

        ///// <summary>
        /////A test for getSuppliersByStatus
        /////</summary>
        //[TestMethod()]
        //public void cSuppliersTest_getSuppliersByStatusTest()
        //{
        //    int accountid = cGlobalVariables.AccountID;
        //    int subaccountid = cGlobalVariables.SubAccountID;
        //    cSuppliers target = new cSuppliers(accountid, subaccountid);
        //    cSupplierStatus status = cSupplierObject.CreateSupplierStatus(false);

        //    // for default supplier, should not be returned
        //    Dictionary<int, cSupplier> actual = target.getSuppliersByStatus(status);
        //    foreach (KeyValuePair<int, cSupplier> kvp in actual)
        //    {
        //        if (kvp.Key == cGlobalVariables.SupplierID)
        //        {
        //            Assert.Fail("getSuppliersByStatus() returned supplier in list when status set to null");
        //        }
        //    }

        //    // create supplier with status set
        //    cSupplierObject.DeleteSupplier();
        //    cSupplierObject.DeleteSupplierStatus();
        //    status = cSupplierObject.CreateSupplierWithCategoriesAndStatus(false).SupplierStatus;

        //    bool supplierFound = false;
        //    actual = target.getSuppliersByStatus(status);

        //    foreach (KeyValuePair<int, cSupplier> kvp in actual)
        //    {
        //        if (kvp.Key == cGlobalVariables.SupplierID)
        //        {
        //            supplierFound = true;
        //            break;
        //        }
        //    }

        //    if (!supplierFound)
        //    {
        //        Assert.Fail("getSuppliersByStatus() did not return supplier with status in list");
        //    }
        //}
    }
}
