using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cESRAssignmentsTest and is intended
    ///to contain all cESRAssignmentsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cESRAssignmentsTest
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
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (cGlobalVariables.ESRAssignmentID > 0)
            {
                cESRAssignmentObject.DeleteESRAssignment();
            }

            //Set the delegate to null for the current user
            System.Web.HttpContext.Current.Session["myid"] = null;
            cEmployeeObject.DeleteDelegateUTEmployee();
        }
        //
        #endregion

        /// <summary>
        ///Add an ESR Assignment with all property values set
        ///Check the values of the save match the returned values from the database for the ESR Assignment
        ///Check an ID is returned
        ///</summary>
        [TestMethod()]
        public void AddESRAssignmentWithAllPropertyValuesSetTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            cESRAssignment assignment = new cESRAssignment(0, 0, "12345678" + DateTime.Now.Ticks.ToString(), new DateTime(2010, 01, 01), new DateTime(2015, 01, 01), ESRAssignmentStatus.ActiveAssignment, "", "", "", "Unit Test House", "Unit Test Lane", "Town", "County", "LN6 3JY", "GB", false, "87654321", "87654321", "Unit test supervisor", "", "", "", true, 0, "", 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", null, true, DateTime.Now, cGlobalVariables.EmployeeID, null, null);

            int ID = target.saveESRAssignment(assignment);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.ESRAssignmentID = ID;

            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);
            //target = new cESRAssignments(accountid, employeeid);

            cESRAssignment actual = target.getAssignmentById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(assignment.active, actual.active);
            Assert.AreEqual(assignment.assignmentnumber, actual.assignmentnumber);
            Assert.AreEqual(assignment.earliestassignmentstartdate, actual.earliestassignmentstartdate);
            Assert.AreEqual(assignment.finalassignmentenddate, actual.finalassignmentenddate);
            Assert.AreEqual(assignment.primaryassignment, actual.primaryassignment);
        }

        /// <summary>
        ///Edit an ESR Assignment with all property values set
        ///Check the values of the save match the returned values from the database for the ESR Assignment
        ///Check an ID is returned
        /// </summary>
        [TestMethod()]
        public void EditESRAssignmentWithAllPropertyValuesSetTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignment assignment = cESRAssignmentObject.CreateESRAssignment();

            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            string assignmentNum = "12131415" + DateTime.Now.Ticks.ToString();

            int ID = target.saveESRAssignment(new cESRAssignment(assignment.assignmentid, assignment.sysinternalassignmentid, assignmentNum, assignment.earliestassignmentstartdate, assignment.finalassignmentenddate, assignment.assignmentstatus, assignment.payrollpaytype, assignment.payrollname, assignment.payrollperiodtype, assignment.assignmentaddress1, assignment.assignmentaddress2, assignment.assignmentaddresstown, assignment.assignmentaddresscounty, assignment.assignmentaddresspostcode, assignment.assignmentaddresscountry, assignment.supervisorflag, assignment.supervisorassignmentnumber, assignment.supervisoremployementnumber, assignment.supervisorfullname, assignment.accrualplan, assignment.employeecategory, assignment.assignmentcategory, assignment.primaryassignment, assignment.normalhours, assignment.normalhoursfrequency, assignment.gradecontracthours, assignment.noofsessions, assignment.sessionsfrequency, assignment.workpatterndetails, assignment.workpatternstartday, assignment.flexibleworkingpattern, assignment.availabilityschedule, assignment.organisation, assignment.legalentity, assignment.positionname, assignment.jobrole, assignment.occupationcode, assignment.assignmentlocation, assignment.grade, assignment.jobname, assignment.group, assignment.tandaflag, assignment.nightworkeroptout, assignment.projectedhiredate, assignment.vacancyid, assignment.active, assignment.CreatedOn, assignment.CreatedBy, assignment.ModifiedOn, assignment.ModifiedBy));

            Assert.IsTrue(ID > 0);

            cGlobalVariables.ESRAssignmentID = ID;

            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);
            //target = new cESRAssignments(accountid, employeeid);

            cESRAssignment actual = target.getAssignmentById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(assignment.active, actual.active);
            Assert.AreEqual(assignmentNum, actual.assignmentnumber);
            Assert.AreEqual(assignment.earliestassignmentstartdate, actual.earliestassignmentstartdate);
            Assert.AreEqual(assignment.finalassignmentenddate, actual.finalassignmentenddate);
            Assert.AreEqual(assignment.primaryassignment, actual.primaryassignment);
        }

        /// <summary>
        /// This adds a new ESR Assignment to the database with any properties that can have no value or be null set 
        /// </summary>
        [TestMethod()]
        public void AddESRAssignmentWithPropValuesSetToNullOrNothingTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            cESRAssignment assignment = new cESRAssignment(0, 0, "12345678" + DateTime.Now.Ticks.ToString(), new DateTime(2010, 01, 01), null, ESRAssignmentStatus.ActiveAssignment, "", "", "", "Unit Test House", "Unit Test Lane", "Town", "County", "LN6 3JY", "GB", false, "87654321", "87654321", "Unit test supervisor", "", "", "", true, 0, "", 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", null, true, DateTime.Now, cGlobalVariables.EmployeeID, null, null);

            int ID = target.saveESRAssignment(assignment);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.ESRAssignmentID = ID;

            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);
            //target = new cESRAssignments(accountid, employeeid);

            cESRAssignment actual = target.getAssignmentById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(assignment.active, actual.active);
            Assert.AreEqual(assignment.assignmentnumber, actual.assignmentnumber);
            Assert.AreEqual(assignment.earliestassignmentstartdate, actual.earliestassignmentstartdate);
            Assert.AreEqual(assignment.finalassignmentenddate, actual.finalassignmentenddate);
            Assert.AreEqual(assignment.primaryassignment, actual.primaryassignment);
        }

        /// <summary>
        ///Edit an ESR Assignment with any properties that can have no value or be null set
        ///Check the values of the save match the returned values from the database for the ESR Assignment
        ///Check an ID is returned
        /// </summary>
        [TestMethod()]
        public void EditESRAssignmentWithPropValuesSetToNullOrNothingTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignment assignment = cESRAssignmentObject.CreateESRAssignment();

            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            string assignmentNum = "12131415" + DateTime.Now.Ticks.ToString();

            int ID = target.saveESRAssignment(new cESRAssignment(assignment.assignmentid, assignment.sysinternalassignmentid, assignmentNum, assignment.earliestassignmentstartdate, null, assignment.assignmentstatus, assignment.payrollpaytype, assignment.payrollname, assignment.payrollperiodtype, assignment.assignmentaddress1, assignment.assignmentaddress2, assignment.assignmentaddresstown, assignment.assignmentaddresscounty, assignment.assignmentaddresspostcode, assignment.assignmentaddresscountry, assignment.supervisorflag, assignment.supervisorassignmentnumber, assignment.supervisoremployementnumber, assignment.supervisorfullname, assignment.accrualplan, assignment.employeecategory, assignment.assignmentcategory, assignment.primaryassignment, assignment.normalhours, assignment.normalhoursfrequency, assignment.gradecontracthours, assignment.noofsessions, assignment.sessionsfrequency, assignment.workpatterndetails, assignment.workpatternstartday, assignment.flexibleworkingpattern, assignment.availabilityschedule, assignment.organisation, assignment.legalentity, assignment.positionname, assignment.jobrole, assignment.occupationcode, assignment.assignmentlocation, assignment.grade, assignment.jobname, assignment.group, assignment.tandaflag, assignment.nightworkeroptout, assignment.projectedhiredate, assignment.vacancyid, assignment.active, assignment.CreatedOn, assignment.CreatedBy, assignment.ModifiedOn, assignment.ModifiedBy));

            Assert.IsTrue(ID > 0);

            cGlobalVariables.ESRAssignmentID = ID;

            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);
            //target = new cESRAssignments(accountid, employeeid);

            cESRAssignment actual = target.getAssignmentById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(assignment.active, actual.active);
            Assert.AreEqual(assignmentNum, actual.assignmentnumber);
            Assert.AreEqual(assignment.earliestassignmentstartdate, actual.earliestassignmentstartdate);
            Assert.AreEqual(assignment.finalassignmentenddate, actual.finalassignmentenddate);
            Assert.AreEqual(assignment.primaryassignment, actual.primaryassignment);
        }

        /// <summary>
        /// Add an ESR Assignment as a delegate
        /// </summary>
        [TestMethod()]
        public void AddESRAssignmentAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            cESRAssignment assignment = new cESRAssignment(0, 0, "12345678" + DateTime.Now.Ticks.ToString(), new DateTime(2010, 01, 01), new DateTime(2015, 01, 01), ESRAssignmentStatus.ActiveAssignment, "", "", "", "Unit Test House", "Unit Test Lane", "Town", "County", "LN6 3JY", "GB", false, "87654321", "87654321", "Unit test supervisor", "", "", "", true, 0, "", 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", null, true, DateTime.Now, cGlobalVariables.EmployeeID, null, null);

            int ID = target.saveESRAssignment(assignment);

            Assert.IsTrue(ID > 0);

            cGlobalVariables.ESRAssignmentID = ID;

            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);
            //target = new cESRAssignments(accountid, employeeid);

            cESRAssignment actual = target.getAssignmentById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(assignment.active, actual.active);
            Assert.AreEqual(assignment.assignmentnumber, actual.assignmentnumber);
            Assert.AreEqual(assignment.earliestassignmentstartdate, actual.earliestassignmentstartdate);
            Assert.AreEqual(assignment.finalassignmentenddate, actual.finalassignmentenddate);
            Assert.AreEqual(assignment.primaryassignment, actual.primaryassignment);
        }

        /// <summary>
        /// Edit an ESR Assignment as a delegate
        /// </summary>
        [TestMethod()]
        public void EditESRAssignmentAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignment assignment = cESRAssignmentObject.CreateESRAssignment();

            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            string assignmentNum = "12131415" + DateTime.Now.Ticks.ToString();

            int ID = target.saveESRAssignment(new cESRAssignment(assignment.assignmentid, assignment.sysinternalassignmentid, assignmentNum, assignment.earliestassignmentstartdate, assignment.finalassignmentenddate, assignment.assignmentstatus, assignment.payrollpaytype, assignment.payrollname, assignment.payrollperiodtype, assignment.assignmentaddress1, assignment.assignmentaddress2, assignment.assignmentaddresstown, assignment.assignmentaddresscounty, assignment.assignmentaddresspostcode, assignment.assignmentaddresscountry, assignment.supervisorflag, assignment.supervisorassignmentnumber, assignment.supervisoremployementnumber, assignment.supervisorfullname, assignment.accrualplan, assignment.employeecategory, assignment.assignmentcategory, assignment.primaryassignment, assignment.normalhours, assignment.normalhoursfrequency, assignment.gradecontracthours, assignment.noofsessions, assignment.sessionsfrequency, assignment.workpatterndetails, assignment.workpatternstartday, assignment.flexibleworkingpattern, assignment.availabilityschedule, assignment.organisation, assignment.legalentity, assignment.positionname, assignment.jobrole, assignment.occupationcode, assignment.assignmentlocation, assignment.grade, assignment.jobname, assignment.group, assignment.tandaflag, assignment.nightworkeroptout, assignment.projectedhiredate, assignment.vacancyid, assignment.active, assignment.CreatedOn, assignment.CreatedBy, assignment.ModifiedOn, assignment.ModifiedBy));

            Assert.IsTrue(ID > 0);

            cGlobalVariables.ESRAssignmentID = ID;

            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);
            //target = new cESRAssignments(accountid, employeeid);
            
            cESRAssignment actual = target.getAssignmentById(ID);

            Assert.IsNotNull(actual);

            Assert.AreEqual(assignment.active, actual.active);
            Assert.AreEqual(assignmentNum, actual.assignmentnumber);
            Assert.AreEqual(assignment.earliestassignmentstartdate, actual.earliestassignmentstartdate);
            Assert.AreEqual(assignment.finalassignmentenddate, actual.finalassignmentenddate);
            Assert.AreEqual(assignment.primaryassignment, actual.primaryassignment);
        }

        /// <summary>
        ///A test to check the ESR assignment record is assigned to an expense item
        ///</summary>
        [TestMethod()]
        public void isAssignedToItemTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;

            cESRAssignmentObject.CreateESRAssignment();
            cSubcatObject.CreateDummySubcat();
            cClaimObject.CreateCurrentClaim();
            cExpenseObject.CreateExpenseItemWithESRAssignment();

            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            
            ClaimStage claimstage = ClaimStage.Current;
            bool expected = true;
            
            bool actual = target.isAssignedToItem(cGlobalVariables.ESRAssignmentID, claimstage);

            cClaimObject.deleteClaim();
            cSubcatObject.DeleteSubcat();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test to check the ESR assignment record is not assigned to an expense item
        ///</summary>
        [TestMethod()]
        public void isNotAssignedToItemTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;

            cESRAssignmentObject.CreateESRAssignment();
            cSubcatObject.CreateDummySubcat();
            cClaimObject.CreateCurrentClaim();
            cExpenseObject.CreateExpenseItem();

            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            ClaimStage claimstage = ClaimStage.Current;
            bool expected = false;

            bool actual = target.isAssignedToItem(cGlobalVariables.ESRAssignmentID, claimstage);

            cClaimObject.deleteClaim();
            cSubcatObject.DeleteSubcat();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for to get the available assignment List Items where a 'none' item is required
        ///</summary>
        [TestMethod()]
        public void GetAvailableAssignmentListItemsWithIncludeNoneTrueTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            bool includeNone = true;
            cESRAssignment assignment = cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            ListItem[] actual = target.getAvailableAssignmentListItems(includeNone);

            Assert.IsTrue(actual.Length > 0);
            Assert.AreEqual(actual[0].Text, "[None]");
            Assert.AreEqual(actual[0].Value, "0");
            Assert.AreEqual(actual[1].Text, assignment.assignmentnumber);
            Assert.AreEqual(actual[1].Value, assignment.assignmentid.ToString());
        }

        /// <summary>
        ///A test for to get the available assignment List Items where a 'none' item is not required
        ///</summary>
        [TestMethod()]
        public void GetAvailableAssignmentListItemsWithIncludeNoneFalseTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            bool includeNone = false;
            cESRAssignment assignment = cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            ListItem[] actual = target.getAvailableAssignmentListItems(includeNone);

            Assert.IsTrue(actual.Length > 0);
            Assert.AreEqual(actual[0].Text, assignment.assignmentnumber);
            Assert.AreEqual(actual[0].Value, assignment.assignmentid.ToString());
        }

        /// <summary>
        ///A test to get the ESR Assignment record by a valid ID
        ///</summary>
        [TestMethod()]
        public void GetAssignmentByAValidIDTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;

            cESRAssignment expected = cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            cESRAssignment actual = target.getAssignmentById(expected.assignmentid);
            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test to get the ESR Assignment record by an invalid ID
        ///</summary>
        [TestMethod()]
        public void GetAssignmentByAnInvalidIDTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;

            cESRAssignment expected = cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            cESRAssignment actual = target.getAssignmentById(0);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test to get the ESR Assignment record by an actual assignment number
        ///</summary>
        [TestMethod()]
        public void GetAssignmentByAValidAssignmentNumberTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;

            cESRAssignment expected = cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            cESRAssignment actual = target.getAssignmentByAssignmentNumber(expected.assignmentnumber);
            Assert.IsNotNull(actual);
            cCompareAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test to get the ESR Assignment record by an actual assignment number which is invalid
        ///</summary>
        [TestMethod()]
        public void GetAssignmentByAnInvalidAssignmentNumberTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;

            cESRAssignment expected = cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            cESRAssignment actual = target.getAssignmentByAssignmentNumber("99999999");
        }

        /// <summary>
        ///A test for deleting ESR Assignments from the database with a valid ID
        ///</summary>
        [TestMethod()]
        public void DeleteESRAssignmentWithAValidIDTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            
            target.deleteESRAssignment(cGlobalVariables.ESRAssignmentID);

            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);
            //target = new cESRAssignments(accountid, employeeid);

            cESRAssignment assignment = target.getAssignmentById(cGlobalVariables.ESRAssignmentID);
            Assert.IsNull(assignment);
        }

        /// <summary>
        ///A test for deleting ESR Assignments from the database with an invalid ID
        ///</summary>
        [TestMethod()]
        public void DeleteESRAssignmentWithAnInvalidIDTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            target.deleteESRAssignment(0);

            Assert.IsNotNull(target.getAssignmentById(cGlobalVariables.ESRAssignmentID));
        }

        ///A test for deleting ESR Assignments from the database with a valid ID as a delegate
        ///</summary>
        [TestMethod()]
        public void DeleteESRAssignmentAsADelegateTest()
        {
            //Set the delegate for the current user
            cEmployeeObject.CreateUTDelegateEmployee();
            System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            target.deleteESRAssignment(cGlobalVariables.ESRAssignmentID);

            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);
            //target = new cESRAssignments(accountid, employeeid);

            cESRAssignment assignment = target.getAssignmentById(cGlobalVariables.ESRAssignmentID);
            Assert.IsNull(assignment);
        }


        /// <summary>
        ///A test for CacheAssignments
        ///</summary>
        [TestMethod()]
        public void CacheAssignmentsTest()
        {
            int AccountID = cGlobalVariables.AccountID;
            int EmployeeID = cGlobalVariables.EmployeeID;
            
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
            Cache.Remove("esrAssignments" + AccountID + "_" + EmployeeID);

            cESRAssignmentObject.CreateESRAssignment();

            //The method is called in the below constructor
            cESRAssignments target = new cESRAssignments(AccountID, EmployeeID);
            Dictionary<int, cESRAssignment> expected = (Dictionary<int, cESRAssignment>)Cache["esrAssignments" + AccountID + "_" + EmployeeID];

            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Count > 0);
            Cache.Remove("esrAssignments" + AccountID + "_" + EmployeeID);
        }

        /// <summary>
        ///A test for archiving an ESR assignment record
        ///</summary>
        [TestMethod()]
        public void ArchiveAssignmentNumberTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            target.archiveAssignmentNumber(cGlobalVariables.ESRAssignmentID);

            cESRAssignment assignment = target.getAssignmentById(cGlobalVariables.ESRAssignmentID);

            Assert.AreEqual(false, assignment.active);
        }

        /// <summary>
        ///A test for to archive all ESR assignment records for a specific employee
        ///</summary>
        [TestMethod()]
        public void ArchiveAllEmployeeAssignmentNumbersTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignmentObject.CreateESRAssignment();
            cESRAssignments target = new cESRAssignments(accountid, employeeid);

            target.archiveAllEmployeeAssignmentNumbers();

            ////Interim solution to enable the SQL Cache Dependency to catch up
            //System.Threading.Thread.Sleep(1000);
            //target = new cESRAssignments(accountid, employeeid);

            cESRAssignment assignment = target.getAssignmentById(cGlobalVariables.ESRAssignmentID);

            Assert.AreEqual(false, assignment.active);
        }

        /// <summary>
        ///A test for cESRAssignments Constructor
        ///</summary>
        [TestMethod()]
        public void cESRAssignmentsConstructorTest()
        {
            int accountid = cGlobalVariables.AccountID;
            int employeeid = cGlobalVariables.EmployeeID;
            cESRAssignments target = new cESRAssignments(accountid, employeeid);
            Assert.IsNotNull(target);
        }
    }
}
