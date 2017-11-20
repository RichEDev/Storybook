using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Collections.Generic;
using System;
using System.Web.UI.WebControls;
using System.Web.Security;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cWorkflowsTest and is intended
    ///to contain all cWorkflowsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cWorkflowsTest
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
        ///A test for accountid
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void accountidTest()
        {
            int accountid = cGlobalVariables.AccountID;
            cWorkflows target = new cWorkflows(new Moqs().CurrentUser());
            Assert.AreEqual(accountid, target.accountid);
        }

        /// <summary>
        ///A test for WorkflowInUse
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void WorkflowInUseTest()
        {
            cWorkflows clsWorkflows = new cWorkflows(new Moqs().CurrentUser());

            clsWorkflows.InsertIntoWorkflow(99, 999998, 33);
            Assert.AreNotEqual(true, clsWorkflows.WorkflowInUse(999998), "Workflow should return true as it is in use.");
            clsWorkflows.RemoveFromWorkflow(999998, 99);
            Assert.AreEqual(false, clsWorkflows.WorkflowInUse(999999), "Should return false as workflow is not in workflow.");
        }

        /// <summary>
        ///A test for WorkflowAlreadyExists
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void WorkflowAlreadyExistsTest()
        {
            Moqs Moqs = new Moqs();

            int workflowID = cWorkflowObject.CreateNewWorkflow();
            System.Threading.Thread.Sleep(1000);
            cWorkflows clsWorkflows = new cWorkflows(Moqs.CurrentUser());
            
            clsWorkflows = new cWorkflows(Moqs.CurrentUser());

            Random clsRandom = new Random();
            cWorkflow reqWorkflow = clsWorkflows.CachedWorkflows.Values[clsRandom.Next(clsWorkflows.CachedWorkflows.Count -1)];
            

            Assert.AreEqual(false, clsWorkflows.WorkflowAlreadyExists(reqWorkflow.workflowid, reqWorkflow.workflowname), "Should return false as the one matching workflow that exists is itself.");
            Assert.AreEqual(true, clsWorkflows.WorkflowAlreadyExists(-1, reqWorkflow.workflowname), "Should return true as the workflow name is already in use.");

            clsWorkflows.DeleteWorkflowByID(workflowID);
        }

        /// <summary>
        ///A test for SaveWorkFlow
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void SaveWorkFlowTest()
        {
            Moqs Moqs = new Moqs();

            int workflowID = cWorkflowObject.CreateNewWorkflow();
            int accountid = cGlobalVariables.AccountID;
            cWorkflows target = new cWorkflows(Moqs.CurrentUser());

            WorkflowType workflowType = WorkflowType.CustomTable; 
            string name = string.Empty; 
            string description = string.Empty; 
            Guid baseTableID = new Guid();
            bool canBeChild = false;
            bool runOnCreation = false;
            bool runOnChange = false; 
            bool runOnDelete = false;
            int employeeID = 0; 
            object[,] steps = null;
            int actual;

            actual = target.SaveWorkFlow(workflowID, workflowType, "Workflow Name " + DateTime.Now.ToString(), description, baseTableID, canBeChild, runOnCreation, runOnChange, runOnDelete, employeeID, steps);

            if (actual < 1)
            {
                Assert.Fail("Workflow not saved.");
            }

            target = new cWorkflows(Moqs.CurrentUser());

            target.DeleteWorkflowByID(workflowID);

            target = new cWorkflows(Moqs.CurrentUser());

            workflowID = cWorkflowObject.CreateNewWorkflow();

            if (actual < 1)
            {
                Assert.Fail("Workflow not saved.");
            }

            target = new cWorkflows(Moqs.CurrentUser());

            target.DeleteWorkflowByID(workflowID);
            target.DeleteWorkflowByID(actual);
        }

        /// <summary>
        ///A test for InsertIntoWorkflow
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void InsertIntoWorkflowTest()
        {
            Moqs Moqs = new Moqs();

            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            int workflowID = cWorkflowObject.CreateNewWorkflow();
            System.Threading.Thread.Sleep(2000);
            target = new cWorkflows(Moqs.CurrentUser());

            int response = target.InsertIntoWorkflow(9999, workflowID, cGlobalVariables.EmployeeID);
            Assert.AreEqual(1, response); // insert without issue

            response = target.InsertIntoWorkflow(9999, workflowID, cGlobalVariables.EmployeeID);

            Assert.AreEqual(-3, response); // -3 already in workflow

            response = target.InsertIntoWorkflow(9999, -1, cGlobalVariables.EmployeeID);

            Assert.AreEqual(-1, response); // -1 workflow not found

            target.RemoveFromWorkflow(workflowID, 9999);

            target = new cWorkflows(Moqs.CurrentUser());

            response = target.InsertIntoWorkflow(9999, workflowID, -1, cGlobalVariables.EmployeeID);

            Assert.AreEqual(-2, response); // -2 workflow step not found

            target.DeleteWorkflowByID(workflowID);
        }

        /// <summary>
        ///A test for IncrementEntityStepNumber
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void IncrementEntityStepNumberTest()
        {
            Moqs Moqs = new Moqs();

            int accountid = cGlobalVariables.AccountID;; // TODO: Initialize to an appropriate value
            int workflowID = cWorkflowObject.CreateNewWorkflow();
            cWorkflows target = new cWorkflows(Moqs.CurrentUser()); // TODO: Initialize to an appropriate value
            int entityID = 9999;

            int actual;
            actual = target.IncrementEntityStepNumber(entityID, -1);
            Assert.AreEqual(-1, actual);
            target.DeleteWorkflowByID(workflowID);
        }

        /// <summary>
        ///A test for GetWorkflowIDForEntity
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void GetWorkflowIDForEntityTest()
        {
            Moqs Moqs = new Moqs();
            
            int accountid = cGlobalVariables.AccountID;; 
            cWorkflows target = new cWorkflows(Moqs.CurrentUser()); 
            cTable baseTable = null; 
            int entityID = 0; 
            int expected = -1; 
            int actual;
            actual = target.GetWorkflowIDForEntity(baseTable, entityID);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetWorkflowByTableID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void GetWorkflowByTableIDTest()
        {
            Moqs Moqs = new Moqs();
        
            int accountid = cGlobalVariables.AccountID;
            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            int newWorkflowID = cWorkflowObject.CreateNewWorkflow();

            System.Threading.Thread.Sleep(2000);

            target = new cWorkflows(Moqs.CurrentUser());
            cWorkflow newWorkflow = target.GetWorkflowByID(newWorkflowID);

            cWorkflow actual;

            actual = target.GetWorkflowByTableID(Guid.Empty);
            Assert.AreEqual(null, actual);

            actual = target.GetWorkflowByTableID(newWorkflow.BaseTable.TableID);
            target.DeleteWorkflowByID(newWorkflow.workflowid);
            if (actual == null)
            {
                Assert.Fail("Should not return null as a workflow is present for it.");
            }
            
        }

        /// <summary>
        ///A test for GetWorkflowByID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void GetWorkflowByIDTest()
        {
            Moqs Moqs = new Moqs();

            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            cWorkflow reqWorkflow;
            int workflowID = cWorkflowObject.CreateNewWorkflow();

            System.Threading.Thread.Sleep(2000);

            target = new cWorkflows(Moqs.CurrentUser());
            reqWorkflow = target.GetWorkflowByID(workflowID);
            if (reqWorkflow == null)
            {
                Assert.Fail("Workflow exists but was not returned");
            }

            target.DeleteWorkflowByID(workflowID);
            System.Threading.Thread.Sleep(2000);

            target = new cWorkflows(Moqs.CurrentUser());

            reqWorkflow = target.GetWorkflowByID(workflowID);

            if (reqWorkflow != null)
            {
                Assert.Fail("Workflow does not exist but returned a workflow");
            }
            
        }

        /// <summary>
        ///A test for GetSelectableSubWorkflows
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void GetSelectableSubWorkflowsTest()
        {
            Moqs Moqs = new Moqs();
           
            int accountid = cGlobalVariables.AccountID;
            cWorkflows target = new cWorkflows(Moqs.CurrentUser());

            Guid baseTableID;
            int workflowID = 0;

            workflowID = cWorkflowObject.CreateNewWorkflow();
            System.Threading.Thread.Sleep(2000);
            target = new cWorkflows(Moqs.CurrentUser());
            baseTableID = target.GetWorkflowByID(workflowID).BaseTable.TableID;

            List<cWorkflow> expected = new List<cWorkflow>();
            List<cWorkflow> actual;
            
            actual = target.GetSelectableSubWorkflows(baseTableID, workflowID);
            Assert.AreEqual(0, actual.Count);
            System.Threading.Thread.Sleep(2000);
            target.DeleteWorkflowByID(workflowID);
            
        }

        /// <summary>
        ///A test for GetNextWorkflowStep
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void GetNextWorkflowStepTest()
        {
            Moqs Moqs = new Moqs();

            int workflowID = cWorkflowObject.CreateNewWorkflow();
            System.Threading.Thread.Sleep(1000);
            int accountid = cGlobalVariables.AccountID;
            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            cWorkflow reqWorkflow = target.GetWorkflowByID(workflowID);
            int entityID = 123456; 
            cWorkflowNextStep expected = new cWorkflowNextStep(WorkflowStatus.RequireUserInput, reqWorkflow.Steps.Values[0]);

            cWorkflowNextStep actual;
            target.InsertIntoWorkflow(entityID, workflowID, cGlobalVariables.EmployeeID);
            System.Threading.Thread.Sleep(1000);

            // approval step
            actual = target.GetNextWorkflowStep(entityID, workflowID);
            Assert.AreEqual(((cApprovalStep)actual.NextStep).ApproverID, ((cApprovalStep)expected.NextStep).ApproverID);
            // immitate approval 
            target.IncrementEntityStepNumber(entityID, workflowID);
            actual = target.GetNextWorkflowStep(entityID, workflowID);

            expected = new cWorkflowNextStep(WorkflowStatus.RequireUserInput, reqWorkflow.Steps.Values[7]);
            Assert.AreEqual((cDecisionStep)actual.NextStep, (cDecisionStep)expected.NextStep);

            // immitate decision true response
            target.UpdateDecisionStep(entityID, workflowID, true);
            

            actual = target.GetNextWorkflowStep(entityID, workflowID);

            expected = new cWorkflowNextStep(WorkflowStatus.Finished, reqWorkflow.Steps.Values[13]);

            Assert.AreEqual(expected.Status, actual.Status, "expected: Finished / actual: " + actual.Status.ToString());
            




            target.DeleteWorkflowByID(workflowID);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetCurrentWorkflowStepNumber
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void GetCurrentWorkflowStepNumberTest()
        {
            Moqs Moqs = new Moqs();

            int accountid = cGlobalVariables.AccountID;
            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            int entityID = 0; // TODO: Initialize to an appropriate value
            int workflowID = cWorkflowObject.CreateNewWorkflow();
            int actual;

            target = new cWorkflows(Moqs.CurrentUser());

            actual = target.GetCurrentWorkflowStepNumber(entityID, workflowID);

            Assert.AreEqual(-1, actual);

            target = new cWorkflows(Moqs.CurrentUser());

            target.InsertIntoWorkflow(9999, workflowID, cGlobalVariables.EmployeeID);

            target = new cWorkflows(Moqs.CurrentUser());

            actual = target.GetCurrentWorkflowStepNumber(9999, workflowID);
            target.DeleteWorkflowByID(workflowID);
            if (actual < 1)
            {
                Assert.Fail("Fail.");
            }
        }

        /// <summary>
        ///A test for GetCurrentEntityStatus
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void GetCurrentEntityStatusTest()
        {
            Moqs Moqs = new Moqs();

            int accountid = cGlobalVariables.AccountID;
            int entityID = 123;
            int workflowID = cWorkflowObject.CreateNewWorkflow();


            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            target.InsertIntoWorkflow(entityID, workflowID, cGlobalVariables.EmployeeID);
            cWorkflow reqWorkflow = target.GetWorkflowByID(workflowID);

            cWorkflowEntityDetails actual = target.GetCurrentEntityStatus(entityID, workflowID);

            target.DeleteWorkflowByID(workflowID);
        }

         /// <summary>
        ///A test for EntityInWorkflow
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void EntityInWorkflowTest()
        {
            Moqs Moqs = new Moqs();

            int accountid = cGlobalVariables.AccountID;
            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            Random random = new Random();
            int entityID = random.Next();
            int workflowID = cWorkflowObject.CreateNewWorkflow();
            bool actual;

            System.Threading.Thread.Sleep(12000);

            target = new cWorkflows(Moqs.CurrentUser());
            target.InsertIntoWorkflow(entityID, workflowID, cGlobalVariables.EmployeeID);
            actual = target.EntityInWorkflow(entityID, workflowID);

            
            Assert.AreEqual(true, actual);
            target.RemoveFromWorkflow(workflowID, entityID);
            target = new cWorkflows(Moqs.CurrentUser());
            actual = target.EntityInWorkflow(entityID, workflowID);
            Assert.AreEqual(false, actual);

            target.DeleteWorkflowByID(workflowID);
        }


        /// <summary>
        ///A test for DeleteWorkflowByID
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void DeleteWorkflowByIDTest()
        {
            Moqs Moqs = new Moqs();

            int accountid = cGlobalVariables.AccountID;
            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            int workflowID = cWorkflowObject.CreateNewWorkflow();
            Assert.AreEqual(true, target.DeleteWorkflowByID(workflowID));
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void CreateDropDownTest()
        {
            Moqs Moqs = new Moqs();

            int accountid = cGlobalVariables.AccountID;
            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            Guid tableid = new Guid();
            List<ListItem> expected = new List<ListItem>();
            List<ListItem> actual = target.CreateDropDown(tableid);
            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        ///A test for AdvanceToNextStep
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void AdvanceToNextStepTest()
        {
            Moqs Moqs = new Moqs();

            int accountid = cGlobalVariables.AccountID;; // TODO: Initialize to an appropriate value
            cWorkflows target = new cWorkflows(Moqs.CurrentUser()); // TODO: Initialize to an appropriate value
            int entityID = 0; // TODO: Initialize to an appropriate value
            int workflowID = 0; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.AdvanceToNextStep(entityID, workflowID, null, null);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cWorkflows Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void cWorkflowsConstructorTest()
        {
            Moqs Moqs = new Moqs();

            int accountid = cGlobalVariables.AccountID;; // TODO: Initialize to an appropriate value
            cWorkflows target = new cWorkflows(Moqs.CurrentUser());
            Assert.AreEqual(accountid, target.accountid);
        }
    }
}
