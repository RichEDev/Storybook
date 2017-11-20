using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SpendManagementUnitTests.Global_Objects;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cESRElementMappingsTest and is intended
    ///to contain all cESRElementMappingsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cESRElementMappingsTest
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
        //public static void ESRElementClassInitialize(TestContext testContext)
        //{
        //}
        
        ////Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void ESRElementClassCleanup()
        //{

        //}
        
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            cESRTrustObject.CreateESRTrustGlobalVariable();
            cSubcatObject.CreateDummySubcat();
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            cESRTrustObject.DeleteTrust();
            cESRElementObjects.DeleteESRElement();
            cSubcatObject.DeleteSubcat();

            //Set the delegate to null for the current user
            System.Web.HttpContext.Current.Session["myid"] = null;
            cEmployeeObject.DeleteDelegateUTEmployee();
        }
        
        #endregion

        /// <summary>
        ///A test for adding an ESRElement with no duplicate existing and all property values set
        ///</summary>
        [TestMethod()]
        public void AddESRElementWithoutDuplicateExistingAndAllPropValuesSetTest()
        {
            int accountID = cGlobalVariables.AccountID;

            List<cESRElementField> lstElementFields = new List<cESRElementField>();
            cESRElementField elementField = new cESRElementField(0, 0, 7, new Guid("cd5fdd75-4f61-4d91-bd10-0e496fd58259"), 1, Aggregate.None);
            lstElementFields.Add(elementField);

            List<int> lstSubcats = new List<int>();
            lstSubcats.Add(cGlobalVariables.SubcatID);

            cESRElementMappings target = new cESRElementMappings(accountID, cGlobalVariables.NHSTrustID);
            cESRElement element = new cESRElement(0, 4, lstElementFields , lstSubcats, cGlobalVariables.NHSTrustID);
            cESRElement actual = null;
            int ID = target.saveESRElement(element);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.ESRElementID = ID;

            actual = target.getESRElementByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(element.GlobalElementID, actual.GlobalElementID);

            //Check the element field has saved correctly
            Assert.AreEqual(actual.Fields[0].ElementID, ID);
            Assert.AreEqual(actual.Fields[0].GlobalElementFieldID, lstElementFields[0].GlobalElementFieldID);
            Assert.AreEqual(actual.Fields[0].ReportColumnID, lstElementFields[0].ReportColumnID);
            Assert.AreEqual(actual.Fields[0].Order, lstElementFields[0].Order);
            Assert.AreEqual(actual.Fields[0].Aggregate, lstElementFields[0].Aggregate);

            Assert.AreEqual(element.Subcats[0], actual.Subcats[0]);
            Assert.AreEqual(element.NHSTrustID, actual.NHSTrustID);
        }

        /// <summary>
        /// Edit an ESR Element with the element fields and element subcats associated 
        /// Make sure property values have a value set to null or nothing for properties that can be
        /// Check the values of the save match the returned values from the database for the element , element fields and element subcats
        /// Check an ID is returned
        /// </summary>
        [TestMethod()]
        public void EditESRElementWithoutDuplicateExistingAndAllPropValuesSetTest()
        {
            int accountID = cGlobalVariables.AccountID;
            cESRElementMappings target = new cESRElementMappings(accountID, cGlobalVariables.NHSTrustID);
            cESRElement element = cESRElementObjects.CreateESRElementGlobalVariable();
            cESRElement actual = null;
            int ID = target.saveESRElement(new cESRElement(element.ElementID, 5, element.Fields, element.Subcats, cGlobalVariables.NHSTrustID));

            Assert.IsTrue(ID > 0);

            actual = target.getESRElementByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(5, actual.GlobalElementID);

            Assert.AreEqual(actual.Fields[0].ElementID, ID);
            Assert.AreEqual(actual.Fields[0].GlobalElementFieldID, element.Fields[0].GlobalElementFieldID);
            Assert.AreEqual(actual.Fields[0].ReportColumnID, element.Fields[0].ReportColumnID);
            Assert.AreEqual(actual.Fields[0].Order, element.Fields[0].Order);
            Assert.AreEqual(actual.Fields[0].Aggregate, element.Fields[0].Aggregate);

            Assert.AreEqual(element.Subcats[0], actual.Subcats[0]);
            Assert.AreEqual(element.NHSTrustID, actual.NHSTrustID);
        }

        /// <summary>
        /// Add an ESR Element as a delegate
        /// </summary>
        [TestMethod()]
        public void AddESRElementAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountID = cGlobalVariables.AccountID;

            List<cESRElementField> lstElementFields = new List<cESRElementField>();
            cESRElementField elementField = new cESRElementField(0, 0, 7, new Guid("cd5fdd75-4f61-4d91-bd10-0e496fd58259"), 1, Aggregate.None);
            lstElementFields.Add(elementField);

            List<int> lstSubcats = new List<int>();
            lstSubcats.Add(cGlobalVariables.SubcatID);

            cESRElementMappings target = new cESRElementMappings(accountID, cGlobalVariables.NHSTrustID);
            cESRElement element = new cESRElement(0, 4, lstElementFields, lstSubcats, cGlobalVariables.NHSTrustID);
            cESRElement actual = null;
            int ID = target.saveESRElement(element);
            cGlobalVariables.ESRElementID = ID;

            Assert.IsTrue(ID > 0);

            actual = target.getESRElementByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(element.GlobalElementID, actual.GlobalElementID);

            //Check the element field has saved correctly
            Assert.AreEqual(actual.Fields[0].ElementID, ID);
            Assert.AreEqual(actual.Fields[0].GlobalElementFieldID, lstElementFields[0].GlobalElementFieldID);
            Assert.AreEqual(actual.Fields[0].ReportColumnID, lstElementFields[0].ReportColumnID);
            Assert.AreEqual(actual.Fields[0].Order, lstElementFields[0].Order);
            Assert.AreEqual(actual.Fields[0].Aggregate, lstElementFields[0].Aggregate);

            Assert.AreEqual(element.Subcats[0], actual.Subcats[0]);
            Assert.AreEqual(element.NHSTrustID, actual.NHSTrustID);
        }

        /// <summary>
        /// Edit an ESR Element as a delegate
        /// </summary>
        [TestMethod()]
        public void EditESRElementAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountID = cGlobalVariables.AccountID;
            cESRElementMappings target = new cESRElementMappings(accountID, cGlobalVariables.NHSTrustID);
            cESRElement element = cESRElementObjects.CreateESRElementGlobalVariable();
            cESRElement actual = null;
            int ID = target.saveESRElement(new cESRElement(element.ElementID, 5, element.Fields, element.Subcats, cGlobalVariables.NHSTrustID));

            Assert.IsTrue(ID > 0);

            actual = target.getESRElementByID(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(5, actual.GlobalElementID);

            Assert.AreEqual(actual.Fields[0].ElementID, ID);
            Assert.AreEqual(actual.Fields[0].GlobalElementFieldID, element.Fields[0].GlobalElementFieldID);
            Assert.AreEqual(actual.Fields[0].ReportColumnID, element.Fields[0].ReportColumnID);
            Assert.AreEqual(actual.Fields[0].Order, element.Fields[0].Order);
            Assert.AreEqual(actual.Fields[0].Aggregate, element.Fields[0].Aggregate);

            Assert.AreEqual(element.Subcats[0], actual.Subcats[0]);
            Assert.AreEqual(element.NHSTrustID, actual.NHSTrustID);
        }        

        /// <summary>
        ///A test for getting the Global ESR Element by a valid ID
        ///</summary>
        [TestMethod()]
        public void GetGlobalESRElementByAValidIDTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);
            List<cGlobalESRElementField> lstGlobalElementFields = new List<cGlobalESRElementField>();
            lstGlobalElementFields.Add(new cGlobalESRElementField(25, 6, "Claim Start Date", false, false));
            lstGlobalElementFields.Add(new cGlobalESRElementField(26, 6, "Claim End Date", false, false));
            lstGlobalElementFields.Add(new cGlobalESRElementField(27, 6, "Period Cash Amount", false, true));
            lstGlobalElementFields.Add(new cGlobalESRElementField(28, 6, "Allowance Type", false, false));
            lstGlobalElementFields.Add(new cGlobalESRElementField(29, 6, "Scheme", false, false));

            cGlobalESRElement expected = new cGlobalESRElement(6, "Course Expenses NR NP NT NNI NHS", lstGlobalElementFields);
            cGlobalESRElement actual = target.GetGlobalESRElementByID(6);
            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for getting the Global ESR Element by an invalid ID
        ///</summary>
        [TestMethod()]
        public void GetGlobalESRElementByAnInvalidIDTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);
            
            cGlobalESRElement actual = target.GetGlobalESRElementByID(0);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///Get any ESR elements associated to a subcat by a valid subcat ID
        ///Return a list of elements.
        ///</summary>
        [TestMethod()]
        public void getESRElementsByAValidSubcatIDTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElement expected = cESRElementObjects.CreateESRElementGlobalVariable();

            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);

            List<cESRElement>  actual = target.getESRElementsBySubcatID(cGlobalVariables.SubcatID);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);

            cCompareAssert.AreEqual(expected, actual[0]);
        }

        /// <summary>
        ///Get any ESR elements associated to a subcat by an invalid subcat ID
        ///Return a list of elements.
        ///</summary>
        [TestMethod()]
        public void getESRElementsByAnInvalidSubcatIDTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;

            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);

            List<cESRElement> actual = target.getESRElementsBySubcatID(0);
            Assert.IsTrue(actual.Count == 0);
        }

        /// <summary>
        ///Get ESR Element record by a valid element ID
        ///Check an ESR Element object returns
        ///</summary>
        [TestMethod()]
        public void getESRElementWithAValidIDTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElement expected = cESRElementObjects.CreateESRElementGlobalVariable();

            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);

            cESRElement actual = target.getESRElementByID(cGlobalVariables.ESRElementID);
            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

         /// <summary>
        ///Get ESR Element record by a valid element ID
        ///Check an ESR Element object returns
        ///</summary>
        [TestMethod()]
        public void getESRElementWithAnInvalidIDTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;

            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);

            cESRElement actual = target.getESRElementByID(0);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test to delete an ESR Element from the database with a valid ID
        ///</summary>
        [TestMethod()]
        public void deleteESRElementWithAValidIDTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElementObjects.CreateESRElementGlobalVariable();
            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);
            target.deleteESRElement(cGlobalVariables.ESRElementID);
            Assert.IsNull(target.getESRElementByID(cGlobalVariables.ESRElementID));
        }

        /// <summary>
        ///A test to delete an ESR Element from the database with an invalid ID
        ///</summary>
        [TestMethod()]
        public void deleteESRElementWithAnInValidIDTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElementObjects.CreateESRElementGlobalVariable();
            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);
            target.deleteESRElement(0);
            Assert.IsNotNull(target.getESRElementByID(cGlobalVariables.ESRElementID));
        }

         /// <summary>
        ///A test to delete an ESR Element from the database with a valid ID as a delegate
        ///</summary>
        [TestMethod()]
        public void deleteESRElementWithAValidIDAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElementObjects.CreateESRElementGlobalVariable();
            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);
            target.deleteESRElement(cGlobalVariables.ESRElementID);
            Assert.IsNull(target.getESRElementByID(cGlobalVariables.ESRElementID));
        }

        /// <summary>
        ///A test for CreateGlobalElementDropDown
        ///</summary>
        [TestMethod()]
        public void CreateGlobalElementDropDownTest()
        {
            int accountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElementMappings target = new cESRElementMappings(accountID, NHSTrustID);
            
            List<ListItem> actual = target.CreateGlobalElementDropDown();
            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        /// An object with all its values set is added to the database and 
        /// then extracted via the cache list method to test the values returned hold up
        /// </summary>
        [TestMethod()]
        public void CacheListTestWithAllPropValuesSet()
        {
            int accountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            //Make sure there is nothing in the cache before running this test
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("esrElements" + accountID + NHSTrustID);

            cESRElement actual = cESRElementObjects.CreateESRElementGlobalVariable();

            //The method is called in the below constructor
            cESRElementMappings target = new cESRElementMappings(accountID, NHSTrustID);
            SortedList<int, cESRElement> expected = (SortedList<int, cESRElement>)Cache["esrElements" + accountID + NHSTrustID];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);

            Assert.AreEqual(actual.Fields[0].ElementID, expected[cGlobalVariables.ESRElementID].Fields[0].ElementID);
            Assert.AreEqual(actual.Fields[0].GlobalElementFieldID, expected[cGlobalVariables.ESRElementID].Fields[0].GlobalElementFieldID);
            Assert.AreEqual(actual.Fields[0].ReportColumnID, expected[cGlobalVariables.ESRElementID].Fields[0].ReportColumnID);
            Assert.AreEqual(actual.Fields[0].Order, expected[cGlobalVariables.ESRElementID].Fields[0].Order);
            Assert.AreEqual(actual.Fields[0].Aggregate, expected[cGlobalVariables.ESRElementID].Fields[0].Aggregate);

            Assert.AreEqual(actual.Subcats[0], expected[cGlobalVariables.ESRElementID].Subcats[0]);

            Cache.Remove("esrElements" + accountID + NHSTrustID);
        }

        /// <summary>
        ///A test to check that the Global ESR Elements are caching
        ///</summary>
        [TestMethod()]
        public void CacheGlobalESRElementsTest()
        {
            int accountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            //Make sure there is nothing in the cache before running this test
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("GlobalESRElements");

            //The method is called in the below constructor
            cESRElementMappings target = new cESRElementMappings(accountID, NHSTrustID);
            SortedList<int, cGlobalESRElement> expected = (SortedList<int, cGlobalESRElement>)Cache["GlobalESRElements"];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("GlobalESRElements");
        }

        /// <summary>
        ///A test for cESRElementMappings Constructor
        ///</summary>
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void cESRElementMappingsConstructorTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test to get any unmapped expense items
        ///</summary>
        [TestMethod()]
        public void GetUnmappedExpenseItemsTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElementObjects.CreateESRElementGlobalVariable();
            
            cSubcats clsSubcats = new cSubcats(AccountID);

            int tempSubcatID = clsSubcats.saveSubcat(new cSubcat(0, cGlobalVariables.CategoryID, "Unit Test dummy Mileage Item 2", "Unit Test dummy Mileage Item 2", true, false, false, false, false, false, 0, "UnitTest02", false, false, 0, false, false, CalculationType.PencePerMile, false, true, "Used for Unit Tests", false, 0, true, false, false, false, false, false, false, false, false, false, false, "", false, false, 0, 0, false, false, new SortedList<int, object>(), DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "Unit Test Mileage", true, true, new List<cCountrySubcat>(), new List<int>(), new List<int>(), new List<int>(), false, new List<cSubcatVatRate>(), true, HomeToLocationType.CalculateHomeAndOfficeToLocationDiff, null, false, null, false));

            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);
            
            List<cSubcat> actual = target.GetUnMappedExpenseItems();

            clsSubcats = new cSubcats(cGlobalVariables.AccountID);
            clsSubcats.deleteSubcat(tempSubcatID);

            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test to get any unmapped expense items
        ///</summary>
        [TestMethod()]
        public void GetUnmappedExpenseItemsWhereNoneAreUnmappedTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int NHSTrustID = cGlobalVariables.NHSTrustID;
            cESRElementObjects.CreateESRElementGlobalVariable();

            cESRElementMappings target = new cESRElementMappings(AccountID, NHSTrustID);
            List<cSubcat> actual = target.GetUnMappedExpenseItems();

            Assert.IsTrue(actual.Count == 0);
        }
    }
}
