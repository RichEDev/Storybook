using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cWorkflowEntityDetailsTest and is intended
    ///to contain all cWorkflowEntityDetailsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cWorkflowEntityDetailsTest
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
        ///A test for WorkflowID
        ///</summary>
        [TestMethod()]
        public void WorkflowIDTest()
        {
            int entityID = 0; // TODO: Initialize to an appropriate value
            int workflowID = 0; // TODO: Initialize to an appropriate value
            int stepNumber = 0; // TODO: Initialize to an appropriate value
            cEmployee entityOwner = null; // TODO: Initialize to an appropriate value
            cEmployee assignedApprover = null; // TODO: Initialize to an appropriate value
            cWorkflowEntityDetails target = new cWorkflowEntityDetails(entityID, workflowID, stepNumber, entityOwner, assignedApprover); // TODO: Initialize to an appropriate value
            Assert.AreEqual(entityID, target.EntityID);
            Assert.AreEqual(workflowID, target.WorkflowID);
            Assert.AreEqual(stepNumber, target.StepNumber);
            Assert.AreEqual(entityOwner, target.EntityOwner);
            Assert.AreEqual(assignedApprover, target.EntityAssignedApprover);
        }
    }
}
