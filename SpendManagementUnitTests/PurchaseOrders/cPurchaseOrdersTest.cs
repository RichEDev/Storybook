using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cPurchaseOrdersTest and is intended
    ///to contain all cPurchaseOrdersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cPurchaseOrdersTest
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
        ///A test for AccountID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void AccountIDTest()
        {
            int accountid = cGlobalVariables.AccountID; // Initialise variable from the global parameter
            cPurchaseOrders target = new cPurchaseOrders(accountid);
            int actual;
            actual = target.AccountID;

            Assert.AreEqual(accountid, actual, "The cPurchaseOrders AccountID is not being set correctly."); 

        }

        /// <summary>
        ///A test for SavePurchaseOrder
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void SavePurchaseOrderTest()
        {
            #region Nulls/Blanks/defaults, should not save a purchaseorder and should return List<int>(){0,0} 
            int accountid = cGlobalVariables.AccountID; // TODO: Initialize to an appropriate value
            cEmployees clsEmployees = new cEmployees(accountid);
            cEmployee clsEmployee = clsEmployees.GetEmployeeById(cGlobalVariables.EmployeeID);
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            string title = string.Empty; // TODO: Initialize to an appropriate value
            int supplierID = 0; // TODO: Initialize to an appropriate value
            int countryID = 0; // TODO: Initialize to an appropriate value
            int currencyID = 0; // TODO: Initialize to an appropriate value
            Nullable<int> parentPurchaseOrderId = null; // TODO: Initialize to an appropriate value
            string purchaseOrderNumber = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<DateTime> dateApproved = null; // TODO: Initialize to an appropriate value
            Nullable<DateTime> dateOrdered = null; // TODO: Initialize to an appropriate value
            string comments = string.Empty; // TODO: Initialize to an appropriate value
            PurchaseOrderState orderState = PurchaseOrderState.Unsubmitted;
            PurchaseOrderType orderType = PurchaseOrderType.Individual;
            Nullable<PurchaseOrderRecurrence> orderRecurrence = null; // TODO: Initialize to an appropriate value
            Nullable<DateTime> orderStartDate = null; // TODO: Initialize to an appropriate value
            Nullable<DateTime> orderEndDate = null; // TODO: Initialize to an appropriate value
            List<int> calendarPoints = null; // TODO: Initialize to an appropriate value
            int employeeID = clsEmployee.employeeid;
            DateTime createdOn = DateTime.Today;
            Dictionary<int, cPurchaseOrderProduct> purchaseOrderProducts = null; // TODO: Initialize to an appropriate value
            
            List<int> expected = new List<int>();
            expected.Add(0);
            expected.Add(0);
            List<int> actual;
            actual = target.SavePurchaseOrder(purchaseOrderID, title, supplierID, countryID, currencyID, parentPurchaseOrderId, purchaseOrderNumber, dateApproved, dateOrdered, comments, orderState, orderType, orderRecurrence, orderStartDate, orderEndDate, calendarPoints, employeeID, createdOn, purchaseOrderProducts);

            if (actual == null || actual.Count == 0)
            {
                Assert.Fail("SavePurchaseOrder did not correctly save the purchase order and did not return the correct default \"not saved\" response.");
            }

            if (expected.Count != actual.Count)
            {
                Assert.Fail("SavePurchaseOrder did not return the correct number of return values.");
            }

            foreach (int x in expected)
            {
                if (!actual.Contains(x))
                {
                    Assert.Fail("SavePurchaseOrder returned the correct number of values but did not match the expected return values.");
                }
            }
            #endregion Nulls/Blanks/defaults, should not save a purchaseorder and should return List<int>(){0,0}

            #region create some purchaseorderproducts
            //cProducts clsProducts = new cProducts(accountid);
            //int tempProductId = clsProducts.UpdateProduct(new cProduct(0, "Unit Test ProductCode", "Unit Test Product Name " + DateTime.Now.ToString(), "Description", null, "", null, "", 0, "", false, new DateTime(), employeeID, null, null), employeeID); //new cProductCategory(0,"UT Cat Desc",false,new DateTime(),employeeID,null,null)
            //clsProducts = new cProducts(accountid);
            //cProduct tempProduct = clsProducts.GetProductById(tempProductId);
            //cUnits clsUnits = new cUnits(accountid, -1);
            //cUnit tempUnit = clsUnits.getUnitById(1); //new cUnit(234,-1,"jg",false,new DateTime(),123,null,null);
            //cDepartments clsDepartments = new cDepartments(accountid);
            //int tempDepartmentId = clsDepartments.saveDepartment(new cDepartment(0, "Unit Test Department", "Unit Test Department Description", false, new DateTime(), employeeID, null, null, new SortedList<int, object>()));
            //clsDepartments = new cDepartments(accountid);
            //cDepartment tempDepartment = clsDepartments.GetDepartmentById(tempDepartmentId);
            //cCostcodes clsCostCodes = new cCostcodes(accountid);
            //int tempCostCodesId = clsCostCodes.saveCostcode(new cCostCode(0, "Unit Test Cost Code", "Unit Test Cost Code Description", false, new DateTime(), employeeID, null, null, new SortedList<int, object>()));
            //clsCostCodes = new cCostcodes(accountid);
            //cCostCode tempCostCode = clsCostCodes.GetCostCodeById(tempCostCodesId);
            //cProjectCodes clsProjectCodes = new cProjectCodes(accountid);
            //int tempProjectCodes = clsProjectCodes.saveProjectCode(new cProjectCode(0, "Unit Test Project Code", "Unit Test Project Code Description", false, false, new DateTime(), employeeID, null, null, new SortedList<int, object>()));
            //clsProjectCodes = new cProjectCodes(accountid);
            //cProjectCode tempProjectCode = clsProjectCodes.getProjectCodeById(tempProductId);
            ////cDepartment tempDepartment = new cDepartment(0, "Unit Test Department", "Unit Test Department Description", false, new DateTime(), employeeID, null, null, new SortedList<int, object>());
            ////cCostCode tempCostCode = new cCostCode(0, "Unit Test Cost Code", "Unit Test Cost Code Description", false, new DateTime(), employeeID, null, null, new SortedList<int, object>());
            ////cProjectCode tempProjectCode = new cProjectCode(0, "Unit Test Project Code", "Unit Test Project Code Description", false, false, new DateTime(), employeeID, null, null, new SortedList<int, object>());
            //cPurchaseOrderProductCostCentre tempCostCentre = new cPurchaseOrderProductCostCentre(null,tempDepartment,tempCostCode,tempProjectCode,100);
            //List<cPurchaseOrderProductCostCentre> lstCostCentres = new List<cPurchaseOrderProductCostCentre>() { tempCostCentre };
            //cPurchaseOrderProduct tempPurchaseOrderProduct = new cPurchaseOrderProduct(0, tempProduct, tempUnit, (decimal)1.44, (decimal)7.32, lstCostCentres);
            #endregion


            #region New non-recurring purchaseorder, basic values, user's defaults
            //purchaseOrderID = 0;
            //title = "New Unit Test Non-recurring PO " + DateTime.Now.ToString();
            //supplierID = 5; // Software Europe Ltd - better way of getting this? does it matter?
            //countryID = clsEmployee.primarycountry;
            //currencyID = clsEmployee.primarycurrency;
            //parentPurchaseOrderId = null;
            //purchaseOrderNumber = string.Empty;
            //dateApproved = null;
            //dateOrdered = null;
            //comments = "A new purchase order created to unit test the SavePurchaseOrder method.";
            //orderState = PurchaseOrderState.Unsubmitted;
            //orderType = PurchaseOrderType.Individual;
            //orderRecurrence = null;
            //orderStartDate = null;
            //orderEndDate = null;
            //calendarPoints = null;
            //employeeID = clsEmployee.employeeid;
            //createdOn = DateTime.Today;
            //purchaseOrderProducts = new Dictionary<int,cPurchaseOrderProduct>();
            //purchaseOrderProducts.Add(0,tempPurchaseOrderProduct);

            cPurchaseOrder po = new cPurchaseOrderObject().CreatePurchaseOrder();

            expected = new List<int>(){0,0};
            actual = target.SavePurchaseOrder(po.PurchaseOrderID, po.Title, po.Supplier.SupplierId, po.Country.countryid, po.Currency.currencyid, po.ParentPurchaseOrderID, po.PurchaseOrderNumber, po.DateApproved, po.DateOrdered, po.Comments, po.PurchaseOrderState, po.OrderType, po.OrderRecurrence, po.OrderStartDate, po.OrderEndDate, po.CalendarPoints, cGlobalVariables.EmployeeID, po.CreatedOn, po.PurchaseOrderProducts);
            //actual = target.SavePurchaseOrder(purchaseOrderID, title, supplierID, countryID, currencyID, parentPurchaseOrderId, purchaseOrderNumber, dateApproved, dateOrdered, comments, orderState, orderType, orderRecurrence, orderStartDate, orderEndDate, calendarPoints, employeeID, createdOn, purchaseOrderProducts);

            if (actual == null || actual.Count == 0)
            {
                Assert.Fail("SavePurchaseOrder did not correctly save the purchase order and did not return the correct default \"not saved\" response.");
            }

            if (expected.Count != actual.Count)
            {
                Assert.Fail("SavePurchaseOrder did not return the correct number of return values.");
            }

            if (actual[0] <= 0)
            {
                Assert.Fail("SavePurchaseOrder returned the correct number of values but did not match the expected positive integer return values that would indicate a Purchase Order was created.");
            }
            if (actual[1] <= 0)
            {
                Assert.Fail("SavePurchaseOrder returned the correct number of values but did not match the expected positive integer return values that would indicate a Purchase Order Product was created.");
            }
            #endregion New non-recurring purchaseorder, basic values, user's defaults

            #region New non-recurring purchaseorder, all values
            #endregion New non-recurring purchaseorder, all values

            #region New recurring purchaseorder, non-nullable values
            #endregion New recurring purchaseorder, non-nullable values

            #region New recurring purchaseorder, all values
            #endregion New recurring purchaseorder, all values

            #region Update recurring purchaseorder, all values
            #endregion Update recurring purchaseorder, all values
        }

        ///// <summary>
        /////A test for saveHistoryItem
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void saveHistoryItemTest1()
        //{
        //    int accountid = cGlobalVariables.AccountID; // TODO: Initialize to an appropriate value
        //    cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
        //    cPurchaseOrderHistoryItem item = null; // TODO: Initialize to an appropriate value
        //    target.saveHistoryItem(item);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for saveHistoryItem
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void saveHistoryItemTest()
        //{
        //    int accountid = 0; // TODO: Initialize to an appropriate value
        //    cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
        //    int purchaseOrderId = 0; // TODO: Initialize to an appropriate value
        //    string comment = string.Empty; // TODO: Initialize to an appropriate value
        //    string createdbystring = string.Empty; // TODO: Initialize to an appropriate value
        //    DateTime createdon = new DateTime(); // TODO: Initialize to an appropriate value
        //    int createdby = 0; // TODO: Initialize to an appropriate value
        //    target.saveHistoryItem(purchaseOrderId, comment, createdbystring, createdon, createdby);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for GetTotalWithTaxByUnitCostTimesQuantity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetTotalWithTaxByUnitCostTimesQuantityTest()
        {
            Decimal unitCost = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal quantity = new Decimal(); // TODO: Initialize to an appropriate value
            cSalesTax salesTax = null; // TODO: Initialize to an appropriate value
            Decimal expected = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal actual;
            actual = cPurchaseOrders.GetTotalWithTaxByUnitCostTimesQuantity(unitCost, quantity, salesTax);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetTotalByUnitCostTimesQuantity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetTotalByUnitCostTimesQuantityTest()
        {
            Decimal unitCost = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal quantity = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal expected = new Decimal(); // TODO: Initialize to an appropriate value
            Decimal actual;
            actual = cPurchaseOrders.GetTotalByUnitCostTimesQuantity(unitCost, quantity);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetTaxByUnitCostTimesQuantity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetTaxByUnitCostTimesQuantityTest()
        {
            Decimal unitCost = new Decimal(1.57); // TODO: Initialize to an appropriate value
            Decimal quantity = new Decimal(2.43); // TODO: Initialize to an appropriate value
            cSalesTax salesTax = new cSalesTax(0, "17.5%", DateTime.Now, cGlobalVariables.EmployeeID, null, null, false, new Decimal(17.5));
            Decimal expected = new Decimal(0.67); // ((1.57 * 2.43) * 17.5/100)
            Decimal actual;
            actual = cPurchaseOrders.GetTaxByUnitCostTimesQuantity(unitCost, quantity, salesTax);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetRelatedPurchaseOrdersTable
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetRelatedPurchaseOrdersTableTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            int employeeID = 0; // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            cGridNew expected = null; // TODO: Initialize to an appropriate value
            cGridNew actual;
            actual = target.GetRelatedPurchaseOrdersTable(employeeID, purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPurchaseOrderRecurringScheduleMonths
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetPurchaseOrderRecurringScheduleMonthsTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            List<int> expected = null; // TODO: Initialize to an appropriate value
            List<int> actual;
            actual = target.GetPurchaseOrderRecurringScheduleMonths(purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPurchaseOrderRecurringScheduleDays
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetPurchaseOrderRecurringScheduleDaysTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            List<int> expected = null; // TODO: Initialize to an appropriate value
            List<int> actual;
            actual = target.GetPurchaseOrderRecurringScheduleDays(purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPurchaseOrderByPurchaseOrderNumber
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetPurchaseOrderByPurchaseOrderNumberTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            string purchaseOrderNumber = string.Empty; // TODO: Initialize to an appropriate value
            cPurchaseOrder expected = null; // TODO: Initialize to an appropriate value
            cPurchaseOrder actual;
            actual = target.GetPurchaseOrderByPurchaseOrderNumber(purchaseOrderNumber);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPurchaseOrderByID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetPurchaseOrderByIDTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrder expected = null; // TODO: Initialize to an appropriate value
            cPurchaseOrder actual;
            actual = target.GetPurchaseOrderByID(purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPurchaseOrderApprovalDetails
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetPurchaseOrderApprovalDetailsTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            int employeeID = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrder expected = null; // TODO: Initialize to an appropriate value
            cPurchaseOrder actual;
            actual = target.GetPurchaseOrderApprovalDetails(purchaseOrderID, employeeID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetProductDetailsTable
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetProductDetailsTableTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            Nullable<int> purchaseOrderID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Table expected = null; // TODO: Initialize to an appropriate value
            Table actual;
            actual = target.GetProductDetailsTable(purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetNetByUnitCostTimesQuantity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetNetByUnitCostTimesQuantityTest()
        {
            Decimal unitCost = new Decimal(1.57);
            Decimal quantity = new Decimal(2.43);
            Decimal expected = new Decimal(3.82); // (3.8151 rounded to 2 decimal places)
            Decimal actual;
            actual = cPurchaseOrders.GetNetByUnitCostTimesQuantity(unitCost, quantity);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetGeneralDetailsTable
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetGeneralDetailsTableTest()
        {
            int accountid = cGlobalVariables.AccountID; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            //Nullable<int> purchaseOrderID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> purchaseOrderID = null;
            int employeeID = 0; // TODO: Initialize to an appropriate value
            //Panel expected = null; // TODO: Initialize to an appropriate value
            Panel actual;
            actual = target.GetGeneralDetailsTable(purchaseOrderID, employeeID);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
            if (actual.FindControl("ddlOrderType") == null)
            {
                Assert.Fail("The Order Type dropdown is not present");
            }
            else if (actual.FindControl("ddlOrderType") != null && actual.FindControl("ddlOrderType").HasControls() == false)
            {
                Assert.Fail("The Order Type dropdown does not contain any values");
            }
        }

        /// <summary>
        ///A test for GetDeliveryDetailsTable
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetDeliveryDetailsTableTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            int employeeID = 0; // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            cGridNew expected = null; // TODO: Initialize to an appropriate value
            cGridNew actual;
            actual = target.GetDeliveryDetailsTable(employeeID, purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetChildPurchaseOrders
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void GetChildPurchaseOrdersTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
            List<int> expected = null; // TODO: Initialize to an appropriate value
            List<int> actual;
            actual = target.GetChildPurchaseOrders(purchaseOrderID);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DisableField
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        [UrlToTest("http://localhost:3237/")]
        public void DisableFieldTest()
        {
            int accountid = 0; // TODO: Initialize to an appropriate value
            cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
            cPurchaseOrder po = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.DisableField(po);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///// <summary>
        /////A test for DeletePurchaseOrder
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void DeletePurchaseOrderTest()
        //{
        //    int accountid = 0; // TODO: Initialize to an appropriate value
        //    cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
        //    int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
        //    int employeeID = 0; // TODO: Initialize to an appropriate value
        //    target.DeletePurchaseOrder(purchaseOrderID, employeeID);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for ChangePurchaseOrderApprovalState
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Inetpub\\wwwroot\\Spend Management\\Spend Management_root\\Spend Management", "/")]
        //[UrlToTest("http://localhost:3237/")]
        //public void ChangePurchaseOrderApprovalStateTest()
        //{
        //    int accountid = 0; // TODO: Initialize to an appropriate value
        //    cPurchaseOrders target = new cPurchaseOrders(accountid); // TODO: Initialize to an appropriate value
        //    int purchaseOrderID = 0; // TODO: Initialize to an appropriate value
        //    PurchaseOrderState approvalState = new PurchaseOrderState(); // TODO: Initialize to an appropriate value
        //    int employeeID = 0; // TODO: Initialize to an appropriate value
        //    target.ChangePurchaseOrderApprovalState(purchaseOrderID, approvalState, employeeID);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}
    }
}
