// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApprovalMatriciesUnitTests.cs" company="Software (Europe) Ltd">
//   Copyright (c) Software (Europe) Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnitTest2012Ultimate.ApprovalMatricies
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Caching;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary.Enumerators;

    using Spend_Management;
    using SpendManagementLibrary;

    /// <summary>
    /// The approval matrices unit tests.
    /// </summary>
    [TestClass]
    public class ApprovalMatriciesUnitTests // ApprovalMatrices
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        System.Web.Caching.Cache webCache = System.Web.HttpRuntime.Cache;
        Utilities.DistributedCaching.Cache distributedCache = new Utilities.DistributedCaching.Cache();

        #endregion

        // You can use the following additional attributes as you write your tests:

        // Use ClassInitialize to run code before running the first test in the class

        // Use ClassCleanup to run code after all tests in a class have run
        #region Public Methods and Operators

        /// <summary>
        /// The my class clean up.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();

        }

        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }

        /// <summary>
        /// Restrict the levels used so that only levels above the claimant can be used.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Approval Matrices")]
        public void ApprovalMatrixRestictLevelsByClaimantLevelWhenClaimantIsEmployee()
        {
            var employeeId = GlobalTestVariables.EmployeeId;
            var budgetholders = this.CreateBudgetHolders();
            var teams = this.CreateTeams(10, 15, 4, GlobalTestVariables.EmployeeId);
            var matrix = this.CreateMatrix(SpendManagementElement.Employees, 10, 100, 4, GlobalTestVariables.EmployeeId);

            var stage = new cStage(1, SignoffType.ApprovalMatrix, matrix.ApprovalMatrixId, 2, StageInclusionType.Always, 0, 0, 0, 0,SignoffType.ApprovalMatrix, 0, 0, false, false, false, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, GlobalTestVariables.EmployeeId, true, false, false, false, false, null);
            var result = Spend_Management.shared.code.ApprovalMatrix.ApprovalMatrices.GetClaimantsLevel(employeeId, matrix, stage, budgetholders, teams);
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Approval Matrices")]
        public void ApprovalMatrixRestictLevelsByClaimantLevelWhenClaimantIsInATeam()
        {
            var employeeId = GlobalTestVariables.EmployeeId;
            var budgetholders = this.CreateBudgetHolders();
            var teams = this.CreateTeams(10, 15, 4, GlobalTestVariables.EmployeeId);
            var matrix = this.CreateMatrix(SpendManagementElement.Teams, 10, 100, 4, 4);

            var stage = new cStage(1, SignoffType.ApprovalMatrix, matrix.ApprovalMatrixId, 2, StageInclusionType.Always, 0, 0, 0, 0, SignoffType.ApprovalMatrix, 0, 0, false, false, false, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, GlobalTestVariables.EmployeeId, true, false, false, false, false, null);
            var result = Spend_Management.shared.code.ApprovalMatrix.ApprovalMatrices.GetClaimantsLevel(employeeId, matrix, stage, budgetholders, teams);
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result);
        }

        [TestMethod, TestCategory("Spend Management"), TestCategory("Approval Matrices")]
        public void ApprovalMatrixRestictLevelsByClaimantLevelWhenClaimantIsABudgetsHolder()
        {
            var employeeId = GlobalTestVariables.EmployeeId;
            var budgetholders = this.CreateBudgetHolders();
            var teams = this.CreateTeams(10, 15, 4, GlobalTestVariables.EmployeeId);
            var matrix = this.CreateMatrix(SpendManagementElement.BudgetHolders, 10, 100, 4, 4);
            
            var stage = new cStage(1, SignoffType.ApprovalMatrix, matrix.ApprovalMatrixId, 2, StageInclusionType.Always, 0, 0, 0, 0, SignoffType.ApprovalMatrix, 0, 0, false, false, false, false, DateTime.Now, GlobalTestVariables.EmployeeId, DateTime.Now, GlobalTestVariables.EmployeeId, true, false, false, false, false, null);
            var result = Spend_Management.shared.code.ApprovalMatrix.ApprovalMatrices.GetClaimantsLevel(employeeId, matrix, stage, budgetholders, teams);
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result);
        }
        #endregion

        private ApprovalMatrix CreateMatrix(SpendManagementElement levelType, int numberOfLevels, decimal step, int indexOfRelId, int relId)
        {
            var approvalMatrixId = 1;
            var name = string.Format("UnitTestApprovalMatrix-{0}", levelType);
            var description = name;
            var defaultApproverKey = string.Format("25, {0}", GlobalTestVariables.EmployeeId);
            var approvalMatrixLevels = new List<ApprovalMatrixLevel>();
            var levelValue = step;
            var level = (int)levelType;
            for (int i = 1; i < numberOfLevels; i++)
            {
                approvalMatrixLevels.Add(new ApprovalMatrixLevel(i, approvalMatrixId, levelValue, string.Format("{0},{1}", level, i)));
                levelValue = levelValue + step;
            }

            if (indexOfRelId > 0 && indexOfRelId < numberOfLevels)
            {
                approvalMatrixLevels[indexOfRelId - 1].ApproverKey = string.Format("{0},{1}", level, relId);
            }

            return new ApprovalMatrix(approvalMatrixId, name, description, defaultApproverKey, approvalMatrixLevels);
        }

        private cTeams CreateTeams(int numberOfTeams, int numberOfMembers, int indexOfEmployee, int employeeId)
        {
            var memberId = 1;
            for (int i = 1; i < numberOfTeams; i++)
            {
                this.distributedCache.Delete(GlobalTestVariables.AccountId, cTeams.CacheArea, i.ToString(CultureInfo.InvariantCulture));

                var team = new List<int>();
                for (int j = 1; j < numberOfMembers; j++)
                {
                   team.Add(memberId);
                    memberId++;
                }

                if (i == indexOfEmployee)
                {
                    team.Add(employeeId);
                }

                this.distributedCache.Add(GlobalTestVariables.AccountId, cTeams.CacheArea, i.ToString(CultureInfo.InvariantCulture), new cTeam(GlobalTestVariables.AccountId, i, string.Format("Team {0}", i), string.Format("Team {0}", i), team, null, DateTime.Now, null, null, null));
            }

            return new cTeams(GlobalTestVariables.AccountId);
        }

        private cBudgetholders CreateBudgetHolders()
        {
            this.webCache.Remove("budgetholders" + GlobalTestVariables.AccountId);
            var budgetHolders = new SortedList<int, cBudgetHolder>();
            budgetHolders.Add(1, new cBudgetHolder(1, "test1", "test1", 1, null, null, null, null));
            budgetHolders.Add(2, new cBudgetHolder(2, "test2", "test2", 2, null, null, null, null));
            budgetHolders.Add(3, new cBudgetHolder(3, "test3", "test3", 3, null, null, null, null));
            budgetHolders.Add(4, new cBudgetHolder(4, "test4", "test4", GlobalTestVariables.EmployeeId, null, null, null, null));
            budgetHolders.Add(5, new cBudgetHolder(5, "test5", "test5", 5, null, null, null, null));
            budgetHolders.Add(6, new cBudgetHolder(6, "test6", "test6", 6, null, null, null, null));
            this.webCache.Add("budgetholders" + GlobalTestVariables.AccountId, budgetHolders, null, DateTime.Now.AddSeconds(1), TimeSpan.Zero, CacheItemPriority.Low, null);
            return new cBudgetholders(GlobalTestVariables.AccountId);
        }
    }
}