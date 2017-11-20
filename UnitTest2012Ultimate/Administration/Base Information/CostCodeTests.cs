// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CostCodeTests.cs" company="Software (Europe) Ltd">
//   Copyright (c) Software (Europe) Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnitTest2012Ultimate.Administration.Base_Information
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Spend_Management;

    /// <summary>
    /// The cost code tests.
    /// </summary>
    [TestClass]
    public class CostCodeTests
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

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

        /// <summary>
        /// The cost code constructor test old style.
        /// </summary>
        [TestMethod]
        public void CostCodeConstructorTestOldStyle()
        {
            var costcodes = new cCostcodes(GlobalTestVariables.AccountId);
            Assert.IsNotNull(costcodes);
            Assert.IsTrue(costcodes.Count > 0);
        }

        /// <summary>
        /// The my test clean up.
        /// </summary>
        [TestCleanup]
        public void MyTestCleanUp()
        {
            HelperMethods.ClearTestDelegateID();
        }

        #endregion


    }
}