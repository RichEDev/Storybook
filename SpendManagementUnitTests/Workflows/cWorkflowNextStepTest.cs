using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cWorkflowNextStepTest and is intended
    ///to contain all cWorkflowNextStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cWorkflowNextStepTest
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
        ///A test for cWorkflowNextStep Constructor
        ///</summary>
        [TestMethod()]
        public void cWorkflowNextStepConstructorTest()
        {
            WorkflowStatus status = new WorkflowStatus();
            cWorkflowStep step = null;
            cWorkflowNextStep target = new cWorkflowNextStep(status, step);

            Assert.AreEqual(status, target.Status);
            Assert.AreEqual(step, target.NextStep);
        }
    }
}
