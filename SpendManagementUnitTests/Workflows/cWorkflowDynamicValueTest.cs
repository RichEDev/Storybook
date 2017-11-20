﻿using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cWorkflowDynamicValueTest and is intended
    ///to contain all cWorkflowDynamicValueTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cWorkflowDynamicValueTest
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
        ///A test for ValueFormula
        ///</summary>
        [TestMethod()]
        public void ValueFormulaTest()
        {
            int dynamicValueID = 0; // TODO: Initialize to an appropriate value
            string valueFormula = string.Empty; // TODO: Initialize to an appropriate value
            cWorkflowDynamicValue target = new cWorkflowDynamicValue(dynamicValueID, valueFormula, new Guid()); // TODO: Initialize to an appropriate value

            Assert.AreEqual(dynamicValueID, target.DynamicValueID);
            Assert.AreEqual(valueFormula, target.ValueFormula);
            
        }
    }
}
