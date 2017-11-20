using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cApprovalStepTest and is intended
    ///to contain all cApprovalStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cApprovalStepTest
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
        ///A test for ShowDeclaration
        ///</summary>
        [TestMethod()]
        public void ApprovalStepPropertyTest()
        {
            int workflowID = 0; // TODO: Initialize to an appropriate value
            int workflowStepID = 0; // TODO: Initialize to an appropriate value
            string description = string.Empty; // TODO: Initialize to an appropriate value
            WorkFlowStepAction action = new WorkFlowStepAction(); // TODO: Initialize to an appropriate value
            int parentStepID = 0; // TODO: Initialize to an appropriate value
            WorkflowEntityType approverType = new WorkflowEntityType(); // TODO: Initialize to an appropriate value
            int approverID = 0; // TODO: Initialize to an appropriate value
            bool oneClickSignOff = false; // TODO: Initialize to an appropriate value
            bool filterItems = false; // TODO: Initialize to an appropriate value
            List<cWorkflowCriteria> criteria = null; // TODO: Initialize to an appropriate value
            bool showDeclaration = false; // TODO: Initialize to an appropriate value
            Nullable<int> formID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> relatedStepID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            string question = string.Empty; // TODO: Initialize to an appropriate value
            string trueResponse = string.Empty; // TODO: Initialize to an appropriate value
            string falseResponse = string.Empty; // TODO: Initialize to an appropriate value
            Nullable<int> emailTemplateID = new Nullable<int>(); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            cApprovalStep target = new cApprovalStep(workflowID, workflowStepID, description, action, parentStepID, approverType, approverID, oneClickSignOff, filterItems, criteria, showDeclaration, formID, relatedStepID, question, trueResponse, falseResponse, emailTemplateID, message); // TODO: Initialize to an appropriate value

            Assert.AreEqual(workflowID, target.WorkflowID);
            Assert.AreEqual(workflowStepID, target.WorkflowStepID);
            Assert.AreEqual(description, target.Description);
            Assert.AreEqual(parentStepID, target.ParentStepID);
            Assert.AreEqual(approverType, target.ApproverType);
            Assert.AreEqual(approverID, target.ApproverID);
            Assert.AreEqual(oneClickSignOff, target.OneClickSignOff);
            Assert.AreEqual(filterItems, target.FilterItems);
            Assert.AreEqual(criteria, target.FilteredItems);
            Assert.AreEqual(showDeclaration, target.ShowDeclaration);
            Assert.AreEqual(formID, target.FormID);
            Assert.AreEqual(relatedStepID, target.RelatedStepID);
            Assert.AreEqual(question, target.Question);
            Assert.AreEqual(trueResponse, target.TrueOption);
            Assert.AreEqual(falseResponse, target.FalseOption);
            Assert.AreEqual(emailTemplateID, target.EmailTemplateID);
        }
    }
}
