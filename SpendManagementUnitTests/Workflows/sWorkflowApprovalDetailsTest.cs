using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for sWorkflowApprovalDetailsTest and is intended
    ///to contain all sWorkflowApprovalDetailsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class sWorkflowApprovalDetailsTest
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
        ///A test for sWorkflowApprovalDetails Constructor
        ///</summary>
        [TestMethod()]
        public void sWorkflowApprovalDetailsConstructorTest()
        {
            WorkflowEntityType approverType = new WorkflowEntityType(); // TODO: Initialize to an appropriate value
            int approverID = 0; // TODO: Initialize to an appropriate value
            bool oneClickSignOff = false; // TODO: Initialize to an appropriate value
            bool FilterItems = false; // TODO: Initialize to an appropriate value
            Nullable<int> emailTemplateID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            sWorkflowApprovalDetails target = new sWorkflowApprovalDetails(approverType, approverID, oneClickSignOff, FilterItems, emailTemplateID);
            Assert.AreEqual(approverType, target.ApproverType);
            Assert.AreEqual(approverID, target.ApproverID);
            Assert.AreEqual(oneClickSignOff, target.OneClickSignOff);
            Assert.AreEqual(FilterItems, target.FilterItems);
            Assert.AreEqual(emailTemplateID, target.EmailTemplateID);
        }
    }
}
