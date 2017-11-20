using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cMovetoStepStepTest and is intended
    ///to contain all cMovetoStepStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cMovetoStepStepTest
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
        ///A test for StepID
        ///</summary>
        [TestMethod()]
        public void StepIDTest()
        {
            int workflowid = 0; 
            int workflowstepid = 0; 
            string descripion = string.Empty; 
            WorkFlowStepAction action = new WorkFlowStepAction(); 
            int stepID = 0; 
            int parentStepID = 0; 
            Nullable<int> formID = new Nullable<int>(); 
            cMovetoStepStep target = new cMovetoStepStep(workflowid, workflowstepid, descripion, action, stepID, parentStepID, formID); 
            Assert.AreEqual(stepID, target.StepID);
        }
    }
}
