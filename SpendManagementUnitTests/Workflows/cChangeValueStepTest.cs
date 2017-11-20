using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cChangeValueStepTest and is intended
    ///to contain all cChangeValueStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cChangeValueStepTest
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
        ///A test for cChangeValueStep Constructor
        ///</summary>
        [TestMethod()]
        public void cChangeValueStepConstructorTest()
        {
            int workflowID = 0; // TODO: Initialize to an appropriate value
            int workflowStepID = 0; // TODO: Initialize to an appropriate value
            string description = string.Empty; // TODO: Initialize to an appropriate value
            WorkFlowStepAction action = new WorkFlowStepAction(); // TODO: Initialize to an appropriate value
            List<cWorkflowCriteria> criteria = null; // TODO: Initialize to an appropriate value
            int parentStepID = 0; // TODO: Initialize to an appropriate value
            Nullable<int> formID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cChangeValueStep target = new cChangeValueStep(workflowID, workflowStepID, description, action, criteria, parentStepID, formID);

            Assert.AreEqual(workflowID, target.WorkflowID);
            Assert.AreEqual(workflowStepID, target.WorkflowStepID);
            Assert.AreEqual(description, target.Description);
            Assert.AreEqual(action, target.Action);
            Assert.AreEqual(criteria, target.Criteria);
            Assert.AreEqual(parentStepID, target.ParentStepID);
            Assert.AreEqual(formID, target.FormID);
        }
    }
}
