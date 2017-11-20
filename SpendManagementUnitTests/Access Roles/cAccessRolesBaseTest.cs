using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections;
using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace SpendManagementUnitTests
{
    /// <summary>
    ///This is a test class for cAccessRolesBaseTest and is intended
    ///to contain all cAccessRolesBaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cAccessRolesBaseTest
    { 
        private int AccountID = 205;// cGlobalVariables.AccountID;
        private int EmployeeID = cGlobalVariables.EmployeeID;

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
        ///A test for SaveAccessRole
        ///</summary>
        [TestMethod()]
        public void SaveAccessRoleTest()
        {
            string sDateNow = DateTime.Now.ToString();
            cAccessRoles target = new cAccessRoles(AccountID, cAccounts.getConnectionString(AccountID));
            int accessRoleID = 0; // 0 for a new access role
            int savedAccessRoleID = 0;
            string accessRoleName = "Unit Test Access Role " + sDateNow;
            string description = "Unit Test Description " + sDateNow;
            short roleAccessLevel = (short)AccessRoleLevel.AllData;
            object[,] elementDetails = null;
            Nullable<Decimal> maximumClaimAmount = new Nullable<Decimal>();
            Nullable<Decimal> minimumClaimAmount = new Nullable<Decimal>();
            bool canAdjustCostCodes = false;
            bool canAdjustDepartment = false;
            bool canAdjustProjectCodes = false;
            object[] lstReportableAccessRoles = null;
            Nullable<int> delegateID = null;
            object[][][][] jsArray = new object[0][][][];
            int actual;
            
            // Save a new access role with null values where available
            actual = target.SaveAccessRole(EmployeeID, accessRoleID, accessRoleName, description, roleAccessLevel, elementDetails, maximumClaimAmount, minimumClaimAmount, canAdjustCostCodes, canAdjustDepartment, canAdjustProjectCodes, lstReportableAccessRoles, delegateID, jsArray);

            if (actual < 1)
            {
                Assert.Fail("Incorrect response from SaveAccessRole (" + actual + ") should be the new accessRoleID (int) - test 1");
            }

            savedAccessRoleID = actual;

            actual = target.SaveAccessRole(EmployeeID, accessRoleID, accessRoleName, description, roleAccessLevel, elementDetails, maximumClaimAmount, minimumClaimAmount, canAdjustCostCodes, canAdjustDepartment, canAdjustProjectCodes, lstReportableAccessRoles, delegateID, jsArray);

            if ((ReturnValues)actual != ReturnValues.AlreadyExists)
            {
                Assert.Fail("Incorrect response from SaveAccessRole (" + actual + ") should return a string value saying it the name already exists - test 2");
            }

            target.DeleteAccessRole(savedAccessRoleID, EmployeeID, delegateID); // cleanup
            target.DeleteAccessRole(actual, EmployeeID, delegateID); // cleanup

            maximumClaimAmount = 1;
            minimumClaimAmount = 1;

            elementDetails = new object[2,4];

            elementDetails[0, 0] = true;
            elementDetails[0, 1] = true;
            elementDetails[0, 2] = true;
            elementDetails[0, 3] = true;
            elementDetails[1, 0] = false;
            elementDetails[1, 1] = false;
            elementDetails[1, 2] = false;
            elementDetails[1, 3] = true;
            
            lstReportableAccessRoles = new object[] { savedAccessRoleID };

            actual = target.SaveAccessRole(EmployeeID, 0, "Unit Test Access Role " + DateTime.Now.ToString(), "Unit Test Description " + DateTime.Now.ToString(), (short)AccessRoleLevel.EmployeesResponsibleFor, elementDetails, maximumClaimAmount, minimumClaimAmount, canAdjustCostCodes, canAdjustDepartment, canAdjustProjectCodes, lstReportableAccessRoles, delegateID, jsArray);

            if (actual < 1)
            {
                Assert.Fail("Incorrect response from SaveAccessRole (" + actual + ") should be the new accessRoleID (int) - test 1");
            }

            target.DeleteAccessRole(actual, EmployeeID, delegateID); // clean up
        }

        /// <summary>
        ///A test for GetAccessRoleByID
        ///</summary>
        [TestMethod()]
        public void GetAccessRoleByIDTest()
        {
            cAccessRoles target = new cAccessRoles(AccountID, cAccounts.getConnectionString(AccountID));
            int accessRoleID = target.AccessRoles.Values[0].RoleID;
            cAccessRole expected = target.AccessRoles.Values[0]; // TODO: Initialize to an appropriate value
            cAccessRole actual = target.GetAccessRoleByID(accessRoleID);
            Assert.AreEqual(expected, actual);
        }

        internal virtual cAccessRolesBase CreatecAccessRolesBase()
        {
            // TODO: Instantiate an appropriate concrete class.
            cAccessRolesBase target = null;
            return target;
        }

        /// <summary>
        ///A test for AccessRoleNameAlreadyExists
        ///</summary>
        [TestMethod()]
        public void AccessRoleNameAlreadyExistsTest()
        {
            cAccessRoles target = new cAccessRoles(AccountID, cAccounts.getConnectionString(AccountID));

            string RoleOne = "Role One 1" + DateTime.Now.ToString();
            string RoleTwo = "Role Two 2" + DateTime.Now.ToString();
            
            int AccessRoleOneID = target.SaveAccessRole(EmployeeID, 0, RoleOne, "", 1, null, null, null, false, false, false, null, null, null);           

            if (target.AccessRoleNameAlreadyExists(RoleOne, AccessRoleOneID) == true)
            {
                Assert.Fail("Should return false as it will only match the one we want to update");
            }

            target = new cAccessRoles(AccountID, cAccounts.getConnectionString(AccountID));
            int AccessRoleTwoID = target.SaveAccessRole(EmployeeID, 0, RoleTwo, "", 1, null, null, null, false, false, false, null, null, null);
            
            if (target.AccessRoleNameAlreadyExists(RoleOne, null) == false)
            {
                Assert.Fail("Should return true, the name already exists");
            }

            target = new cAccessRoles(AccountID, cAccounts.getConnectionString(AccountID));

            if (target.AccessRoleNameAlreadyExists(RoleOne, AccessRoleTwoID) == false)
            {
                Assert.Fail("Should return true as the name already exists for a different role");
            }

            target.DeleteAccessRole(AccessRoleOneID, EmployeeID, null); // Cleanup
            target.DeleteAccessRole(AccessRoleTwoID, EmployeeID, null); // Cleanup
        }
    }
}
