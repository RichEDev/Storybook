using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using Spend_Management;

namespace UnitTest2012Ultimate
{
    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Summary description for cgridNewTests
    /// </summary>
    [TestClass]
    public class cGridNewTests
    {
        private string username;
        private string password;
        private cEmployees clsEmployees;
        private Employee reqGlobalEmployee;
        private string defaultPlainTextPassword;
        private int accountID;
        private int employeeID;
        private int delegateID;
        private int subAccountID;
        private int activeModule;       

       
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
        
         [TestInitialize()]
         public void MyTestInitialize()
         {
            username = string.Empty;
            password = string.Empty;
            clsEmployees = new cEmployees(GlobalTestVariables.AccountId);
            reqGlobalEmployee = null;
            defaultPlainTextPassword = "password";
            accountID = GlobalTestVariables.AccountId;
            employeeID = GlobalTestVariables.EmployeeId;
            delegateID = GlobalTestVariables.DelegateId;
            subAccountID = GlobalTestVariables.SubAccountId;
            activeModule = GlobalTestVariables.ActiveModuleId;

         }
         [TestCleanup]
         public void TestCleanup()
         {
             if (reqGlobalEmployee != null)
             {
                 reqGlobalEmployee.Delete(Moqs.CurrentUser());
             }
         }
         [ClassInitialize()]
         public static void MyClassInitialize(TestContext context)
         {
             GlobalAsax.Application_Start();
         }

         [ClassCleanup()]
         public static void MyClassCleanup()
         {
             GlobalAsax.Application_End();
         }

        #endregion

         [TestCategory("Spend Management"), TestCategory("cGridNew"), TestMethod()]
         public void cGridNew_CreateGrid_NoActionColumns()
         {
             int accountid = GlobalTestVariables.AccountId;
             cEmployees target = new cEmployees(accountid);
             int employeeid = GlobalTestVariables.EmployeeId;
             var testGrid = new cGridNew(accountid, employeeid, "testGrid", target.createGrid());
             Assert.IsTrue(testGrid.GetType() == typeof(cGridNew), "Returned type not cGridNew");
             var gridResult = testGrid.generateGrid();
             Assert.IsTrue(gridResult.Any(), "No Grid Created");
             if (gridResult.Any())
             {
                 var colCount = gridResult[1].ToString();
                 Regex aReg = new Regex("<th style=\"width:;\">");
                 var matches = aReg.Matches(colCount);
                 Assert.IsTrue(matches.Count == 9, "Wrong number of columns created");
             }
         }

        [TestCategory("Spend Management"),TestCategory("cGridNew"),TestMethod()]
        public void cGridNew_CreateGrid_IncludeActionColumn()
        {
            int accountid = GlobalTestVariables.AccountId;
            cEmployees target = new cEmployees(accountid);
            int employeeid = GlobalTestVariables.EmployeeId;
            var testGrid = new cGridNew(accountid, employeeid, "testGrid", target.createGrid());
            Assert.IsTrue(testGrid.GetType() == typeof(cGridNew), "Returned type not cGridNew");
            testGrid.addEventColumn("event","EventText","javascript:testmethod();","TooltipText");
            var gridResult = testGrid.generateGrid();
            
            Assert.IsTrue(gridResult.Any(), "No Grid Created");
            if (gridResult.Any())
            {
                var colCount = gridResult[1];
                Regex aReg = new Regex("<th");
                var matches = aReg.Matches(colCount);
                Assert.IsTrue(matches.Count == 10, "Wrong number of columns created");
                aReg = new Regex("<th style=\"width:;\">EventText");
                matches = aReg.Matches(colCount);
                Assert.IsTrue(matches.Count == 1, "Event Column not created");
            }
        }

        [TestCategory("Spend Management"), TestCategory("cGridNew"), TestMethod()]
        public void cGridNew_CreateGrid_IncludeTwoStateColumn()
        {
            int accountid = GlobalTestVariables.AccountId;
            cEmployees target = new cEmployees(accountid);
            int employeeid = GlobalTestVariables.EmployeeId;
            var testGrid = new cGridNew(accountid, employeeid, "testGrid", target.createGrid());
            Assert.IsTrue(testGrid.GetType() == typeof(cGridNew), "Returned type not cGridNew");
            testGrid.addTwoStateEventColumn("Locked", (cFieldColumn)testGrid.getColumnByName("locked"), false, true, "", "", "Lock Account", "Lock Account", "/static/icons/16/new-icons/lock.png", "javascript:changeLockedStatus({employeeid});", "Unlock Account", "Unlock Account");
            
            var gridResult = testGrid.generateGrid();
            Assert.IsTrue(gridResult.Any(), "No Grid Created");
            if (gridResult.Any())
            {
                var colCount = gridResult[1];
                Regex aReg = new Regex("<th style=\"width:;\">");
                var matches = aReg.Matches(colCount);
                Assert.IsTrue(matches.Count == 9, "Wrong number of columns created");
                aReg = new Regex("/static/icons/16/new-icons/lock.png\" /></a></th>");
                matches = aReg.Matches(colCount);
                Assert.IsTrue(matches.Count == 1,"Event column not showing lock icon");
            }
        }

        [TestCategory("Spend Management"), TestCategory("cGridNew"), TestMethod()]
        public void cGridNew_ClearFiltersForField_ValidFilter()
        {
            int accountid = GlobalTestVariables.AccountId;
            cCurrencies target = new cCurrencies(accountid, GlobalTestVariables.SubAccountId);
            int employeeid = GlobalTestVariables.EmployeeId;
            var testGrid = new cGridNew(accountid, employeeid, "testGrid", target.getGrid());
            testGrid.addFilter(((cFieldColumn)testGrid.getColumnByName("subAccountId")).field, ConditionType.Equals, new object[] { GlobalTestVariables.SubAccountId }, null, ConditionJoiner.None);
            testGrid.addFilter(((cFieldColumn)testGrid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);

            var gridResult = testGrid.generateGrid();
            Assert.IsTrue(gridResult.Any(), "No Grid Created");
            
            PrivateObject testGridObj = new PrivateObject(testGrid);
            List<cQueryFilter> lstCriteria = (List<cQueryFilter>)testGridObj.GetField("lstCriteria");
            Assert.AreEqual(lstCriteria.Count, 2);
            
            cFields clsFields = new cFields(accountid);
            testGrid.clearFiltersForField(clsFields.GetFieldByID(new Guid("5FED50D9-F5C7-497F-9EC3-827C9035261D")));

            lstCriteria = (List<cQueryFilter>)testGridObj.GetField("lstCriteria");
            Assert.AreEqual(lstCriteria.Count, 1);
        }

        [TestCategory("Spend Management"), TestCategory("cGridNew"), TestMethod()]
        public void cGridNew_ClearFiltersForField_InvalidFilter()
        {
            int accountid = GlobalTestVariables.AccountId;
            var target = new cCurrencies(accountid, GlobalTestVariables.SubAccountId);
            int employeeid = GlobalTestVariables.EmployeeId;
            var testGrid = new cGridNew(accountid, employeeid, "testGrid", target.getGrid());
            testGrid.addFilter(((cFieldColumn)testGrid.getColumnByName("subAccountId")).field, ConditionType.Equals, new object[] { GlobalTestVariables.SubAccountId }, null, ConditionJoiner.None);
            testGrid.addFilter(((cFieldColumn)testGrid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);

            var gridResult = testGrid.generateGrid();
            Assert.IsTrue(gridResult.Any(), "No Grid Created");

            var testGridObj = new PrivateObject(testGrid);
            var lstCriteria = (List<cQueryFilter>)testGridObj.GetField("lstCriteria");
            Assert.AreEqual(lstCriteria.Count, 2);

            var clsFields = new cFields(accountid);
            // try and clear a non-existent filter
            testGrid.clearFiltersForField(clsFields.GetFieldByID(new Guid("AA4D627C-03C2-4B1D-9FD8-D517E6A693DD")));

            lstCriteria = (List<cQueryFilter>)testGridObj.GetField("lstCriteria");
            Assert.AreEqual(lstCriteria.Count, 2);
        }
    }
}
