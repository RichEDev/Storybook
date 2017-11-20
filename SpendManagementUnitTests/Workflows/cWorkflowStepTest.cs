using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cWorkflowStepTest and is intended
    ///to contain all cWorkflowStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cWorkflowStepTest
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
        ///A test for cWorkflowStep Constructor
        ///</summary>
        [TestMethod()]
        public void cWorkflowStepConstructorTest()
        {
            int workflowid = 0; // TODO: Initialize to an appropriate value
            int workflowstepid = 0; // TODO: Initialize to an appropriate value
            string description = string.Empty; // TODO: Initialize to an appropriate value
            WorkFlowStepAction action = new WorkFlowStepAction(); // TODO: Initialize to an appropriate value
            int parentStepID = 0; // TODO: Initialize to an appropriate value
            bool showQuestionDialog = false; // TODO: Initialize to an appropriate value
            string question = string.Empty; // TODO: Initialize to an appropriate value
            string trueOption = string.Empty; // TODO: Initialize to an appropriate value
            string falseOption = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<int> formID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> relatedStepID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cWorkflowStep target = new cWorkflowStep(workflowid, workflowstepid, description, action, parentStepID, showQuestionDialog, question, trueOption, falseOption, formID, relatedStepID);

            Assert.AreEqual(workflowid, target.WorkflowID);
            Assert.AreEqual(workflowstepid, target.WorkflowStepID);
            Assert.AreEqual(description, target.Description);
            Assert.AreEqual(action, target.Action);
            Assert.AreEqual(parentStepID, target.ParentStepID);
            Assert.AreEqual(showQuestionDialog, target.ShowQuestion);
            Assert.AreEqual(question, target.Question);
            Assert.AreEqual(trueOption, target.TrueOption);
            Assert.AreEqual(falseOption, target.FalseOption);
            Assert.AreEqual(formID, target.FormID);
            Assert.AreEqual(relatedStepID, target.RelatedStepID);
        }
    }
}
