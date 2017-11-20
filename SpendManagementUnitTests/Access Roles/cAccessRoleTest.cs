using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cAccessRoleTest and is intended
    ///to contain all cAccessRoleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cAccessRoleTest
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
        ///A test for cAccessRole Constructor
        ///</summary>
        [TestMethod()]
        public void cAccessRoleConstructorTest()
        {
            int roleID = 1337;
            string roleName = string.Empty;
            string description = string.Empty;
            SortedList<SpendManagementElement, cElementAccess> elements = null;
            AccessRoleLevel accessLevel = new AccessRoleLevel();
            int createdBy = 0;
            DateTime createdOn = new DateTime();
            Nullable<int> modifiedBy = new Nullable<int>();
            Nullable<DateTime> modifiedOn = new Nullable<DateTime>();
            bool canAmendDesignatedCostCode = false;
            bool canAmendDesignatedDepartment = false;
            bool canAmendDesignatedProjectCode = false;
            Nullable<Decimal> MinimumExpenseClaimAmount = new Nullable<Decimal>();
            Nullable<Decimal> maximumExpenseClaimAmount = new Nullable<Decimal>();
            List<int> accessableAccessRoles = null;
            SortedList<int, cCustomEntityAccess> customEntityAccess = new SortedList<int,cCustomEntityAccess>();

            #region blank + null values
            cAccessRole target = new cAccessRole(roleID, roleName, description, elements, accessLevel, createdBy, createdOn, modifiedBy, modifiedOn, canAmendDesignatedCostCode, canAmendDesignatedDepartment, canAmendDesignatedProjectCode, MinimumExpenseClaimAmount, maximumExpenseClaimAmount, accessableAccessRoles, customEntityAccess);
            Assert.AreEqual(roleID, target.RoleID, "Access Role ID does not match");
            Assert.AreEqual(roleName, target.RoleName);
            Assert.AreEqual(description, target.RoleName);
            Assert.AreEqual(elements, target.ElementAccess);
            Assert.AreEqual(accessLevel, target.AccessLevel);
            Assert.AreEqual(createdBy, target.CreatedBy);
            Assert.AreEqual(createdOn, target.CreatedOn);
            Assert.AreEqual(modifiedBy, target.ModifiedBy);
            Assert.AreEqual(modifiedOn, target.ModifiedOn);
            Assert.AreEqual(canAmendDesignatedCostCode, target.CanEditCostCode);
            Assert.AreEqual(canAmendDesignatedDepartment, target.CanEditDepartment);
            Assert.AreEqual(canAmendDesignatedProjectCode, target.CanEditProjectCode);
            Assert.AreEqual(MinimumExpenseClaimAmount, target.ExpenseClaimMinimumAmount);
            Assert.AreEqual(maximumExpenseClaimAmount, target.ExpenseClaimMaximumAmount);
            Assert.AreEqual(accessableAccessRoles, target.AccessRoleLinks);
            #endregion blank + null values
             

            modifiedOn = DateTime.Now;
            modifiedBy = cGlobalVariables.EmployeeID;
            MinimumExpenseClaimAmount = 12;
            maximumExpenseClaimAmount = 100;
            accessableAccessRoles = new List<int>();
            accessableAccessRoles.Add(-1);
            accessableAccessRoles.Add(-2);
            accessLevel = AccessRoleLevel.AllData;
            elements = new SortedList<SpendManagementElement, cElementAccess>();
            elements.Add(SpendManagementElement.Advances, new cElementAccess(1, true, true, true, true));



            #region non blank or null values
            target = new cAccessRole(roleID, roleName, description, elements, accessLevel, createdBy, createdOn, modifiedBy, modifiedOn, canAmendDesignatedCostCode, canAmendDesignatedDepartment, canAmendDesignatedProjectCode, MinimumExpenseClaimAmount, maximumExpenseClaimAmount, accessableAccessRoles, customEntityAccess);
            Assert.AreEqual(roleID, target.RoleID, "Access Role ID does not match");
            Assert.AreEqual(roleName, target.RoleName);
            Assert.AreEqual(description, target.RoleName);
            Assert.AreEqual(elements, target.ElementAccess);
            Assert.AreEqual(accessLevel, target.AccessLevel);
            Assert.AreEqual(createdBy, target.CreatedBy);
            Assert.AreEqual(createdOn, target.CreatedOn);
            Assert.AreEqual(modifiedBy, target.ModifiedBy);
            Assert.AreEqual(modifiedOn, target.ModifiedOn);
            Assert.AreEqual(canAmendDesignatedCostCode, target.CanEditCostCode);
            Assert.AreEqual(canAmendDesignatedDepartment, target.CanEditDepartment);
            Assert.AreEqual(canAmendDesignatedProjectCode, target.CanEditProjectCode);
            Assert.AreEqual(MinimumExpenseClaimAmount, target.ExpenseClaimMinimumAmount);
            Assert.AreEqual(maximumExpenseClaimAmount, target.ExpenseClaimMaximumAmount);
            Assert.AreEqual(accessableAccessRoles, target.AccessRoleLinks);
            #endregion non blank or null values

        }
    }
}
