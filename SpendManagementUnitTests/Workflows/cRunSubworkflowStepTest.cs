using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cRunSubworkflowStepTest and is intended
    ///to contain all cRunSubworkflowStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cRunSubworkflowStepTest
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
        ///A test for cRunSubworkflowStep Constructor
        ///</summary>
        [TestMethod()]
        public void cRunSubworkflowStepConstructorTest()
        {
            int workflowid = 0; // TODO: Initialize to an appropriate value
            int workflowstepid = 0; // TODO: Initialize to an appropriate value
            string descripion = string.Empty; // TODO: Initialize to an appropriate value
            WorkFlowStepAction action = new WorkFlowStepAction(); // TODO: Initialize to an appropriate value
            int subworkflowid = 0; // TODO: Initialize to an appropriate value
            int parentStepID = 0; // TODO: Initialize to an appropriate value
            Nullable<int> formID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cRunSubworkflowStep target = new cRunSubworkflowStep(workflowid, workflowstepid, descripion, action, subworkflowid, parentStepID, formID);

            Assert.AreEqual(workflowid, target.WorkflowID);
            Assert.AreEqual(workflowstepid, target.WorkflowStepID);
            Assert.AreEqual(descripion, target.Description);
            Assert.AreEqual(action, target.Action);
            Assert.AreEqual(subworkflowid, target.SubWorkflowID);
            Assert.AreEqual(parentStepID, target.ParentStepID);
            Assert.AreEqual(formID, target.FormID);

            formID = 12;
            descripion = "Test";

            target = new cRunSubworkflowStep(workflowid, workflowstepid, descripion, WorkFlowStepAction.RunSubWorkflow, subworkflowid, parentStepID, formID);

            Assert.AreEqual(formID, target.FormID);
            Assert.AreEqual(descripion, target.Description);
        }
    }
}
