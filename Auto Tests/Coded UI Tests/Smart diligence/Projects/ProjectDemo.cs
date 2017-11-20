using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Auto_Tests.Tools;


namespace Auto_Tests.Coded_UI_Tests.Smart_diligence.Projects
{
    
    /// <summary>
    /// Summary description for ProjectDemo
    /// </summary>
    [CodedUITest]
    public class ProjectDemo
    {
        public ProjectDemo()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap csharesMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.ProjectDemoUIMapClasses.ProjectDemoUIMap cProjectDemoMethods = new UIMaps.ProjectDemoUIMapClasses.ProjectDemoUIMap();

        [TestMethod]
        public void AddProject()
        {
            //Click the add projects link
            cProjectDemoMethods.ClickAddProject();

            //populat the project details 
            cProjectDemoMethods.PopulateGeneralDetails();

            ////add populate save Further Enq
            //cProjectDemoMethods.EnterFurtherEnqTab();
            //cProjectDemoMethods.ClickAddFurtherEnq();
            //cProjectDemoMethods.PopulateAndSaveFurtherEnqParams.UIDateEditText = "11";
            //cProjectDemoMethods.PopulateAndSaveFurtherEnq();


            //test zone

              //add populate save project key date
            cProjectDemoMethods.ClickAddKeyDate();
            cProjectDemoMethods.PopulateAndSaveKeyDateParams.UIDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveKeyDate();


            //add populate save issues
            cProjectDemoMethods.EnterIssuesTab();
            cProjectDemoMethods.ClickAddIssues();
            //cProjectDemoMethods.PopulateAndSaveIssuesToChecklistParams.UIReviewedDateEditText = "11";
            //cProjectDemoMethods.PopulateAndSaveIssuesToChecklist();
            //cProjectDemoMethods.PopulateAndSaveIssueParams.UIReviewedDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveIssue();



            //add populate save PCA
            cProjectDemoMethods.EnterPCATab();
            cProjectDemoMethods.ClickAddPCA();
            cProjectDemoMethods.PopulateAndSavePCAParams.UIDeadlineEditText = "11";
            cProjectDemoMethods.PopulateAndSavePCA();



            //add populate save Further Enq
            cProjectDemoMethods.EnterFurtherEnqTab();
            cProjectDemoMethods.ClickAddFurtherEnq();
            cProjectDemoMethods.PopulateAndSaveFurtherEnqParams.UIDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveFurtherEnq();



            //add populate save contract checklist
            cProjectDemoMethods.EnterCommercialTab();
            cProjectDemoMethods.ClickAddContractReviewDetails();
            cProjectDemoMethods.PopulateInitialDetailsParams.UIReviewDateEditText = "11";
            cProjectDemoMethods.PopulateInitialDetails();

            //add populate save issue
            cProjectDemoMethods.EnterIssuesTab();
            cProjectDemoMethods.ClickAddIssuesToChecklist();
            //cProjectDemoMethods.PopulateAndSaveIssuesToChecklistParams.UIReviewedDateEditText = "11";
            //cProjectDemoMethods.PopulateAndSaveIssuesToChecklist();
            //cProjectDemoMethods.PopulateAndSaveIssueParams.UIReviewedDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveIssue();

            //add populate save PCA
            cProjectDemoMethods.EnterPCATabInChecklist();
            cProjectDemoMethods.ClickAddPCAToCheckList();
            cProjectDemoMethods.PopulateAndSavePCAParams.UIDeadlineEditText = "11";
            cProjectDemoMethods.PopulateAndSavePCA();

            //add populate save Further Enq
            cProjectDemoMethods.EnterFurtherEnqTabInChecklist();
            cProjectDemoMethods.ClickAddFurtherEnq();
            cProjectDemoMethods.PopulateAndSaveFurtherEnqParams.UIDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveFurtherEnq();


            //end zone
        }

        [TestMethod]
        public void AddKeyDate()
        {
            cProjectDemoMethods.move3();
            ///cProjectDemoMethods.movedown();

        }

        [TestMethod]
        public void AddIssue()
        {

            //add populate save issues
            cProjectDemoMethods.EnterIssuesTab();
            cProjectDemoMethods.ClickAddIssues();
            //cProjectDemoMethods.PopulateAndSaveIssue();
            cProjectDemoMethods.PopulateAndSaveIssuesToChecklistParams.UIReviewedDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveIssuesToChecklist();


        }

        [TestMethod]
        public void PCA()
        {

            //add populate save PCA
            cProjectDemoMethods.EnterPCATabInChecklist();
            cProjectDemoMethods.ClickAddPCA();
            cProjectDemoMethods.PopulateAndSavePCAParams.UIDeadlineEditText = "11";
            cProjectDemoMethods.PopulateAndSavePCA();


        }

        [TestMethod]
        public void FurtherEnq()
        {

            //add populate save Further Enq
            cProjectDemoMethods.EnterFurtherEnqTab();
            cProjectDemoMethods.ClickAddFurtherEnq();
            cProjectDemoMethods.PopulateAndSaveFurtherEnqParams.UIDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveFurtherEnq();


        }

        [TestMethod]
        public void commercial()
        {
            //add populate save contract checklist
            cProjectDemoMethods.EnterCommercialTab();
            cProjectDemoMethods.ClickAddContractReviewDetails();
            cProjectDemoMethods.PopulateInitialDetailsParams.UIReviewDateEditText = "11";
            cProjectDemoMethods.PopulateInitialDetails();

            //add populate save issue
            cProjectDemoMethods.EnterIssuesTab();
            cProjectDemoMethods.ClickAddIssuesToChecklist();
            cProjectDemoMethods.PopulateAndSaveIssuesToChecklistParams.UIReviewedDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveIssuesToChecklist();

            //add populate save PCA
            cProjectDemoMethods.EnterPCATabInChecklist();
            cProjectDemoMethods.ClickAddPCAToCheckList();
            cProjectDemoMethods.PopulateAndSavePCAParams.UIDeadlineEditText = "11";
            cProjectDemoMethods.PopulateAndSavePCA();

            //add populate save Further Enq
            cProjectDemoMethods.EnterFurtherEnqTabInChecklist();
            cProjectDemoMethods.ClickAddFurtherEnq();
            cProjectDemoMethods.PopulateAndSaveFurtherEnqParams.UIDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveFurtherEnq();
        }

        [TestMethod]
        public void LoanAndSecurity()
        {
            //add populate save Loan and Security checklist
            cProjectDemoMethods.EnterLoanAndSecurityTab();
            cProjectDemoMethods.ClickAddLoanAndSecurity();
            cProjectDemoMethods.PopulateMandatoryFieldsParams.UIReviewDateEditText = "11";
            cProjectDemoMethods.PopulateMandatoryFields();
            //cProjectDemoMethods.PopulateInitialDetailsParams.UIReviewDateEditText = "11";
            //cProjectDemoMethods.PopulateInitialDetails();

            //add populate save issue
            cProjectDemoMethods.EnterIssuesTab();
            //cProjectDemoMethods.ClickAddIssuesToChecklist();
            //cProjectDemoMethods.PopulateAndSaveIssuesToChecklistParams.UIReviewedDateEditText = "11";
            //cProjectDemoMethods.PopulateAndSaveIssuesToChecklist();
            cProjectDemoMethods.ClickAddIssues();
            cProjectDemoMethods.PopulateAndSaveIssue();

            ////add populate save PCA
            //cProjectDemoMethods.EnterPCATab();
            //cProjectDemoMethods.ClickAddPCAToCheckList();
            //cProjectDemoMethods.PopulateAndSavePCAParams.UIDeadlineEditText = "11";
            //cProjectDemoMethods.PopulateAndSavePCA();

            //add populate save Further Enq
            cProjectDemoMethods.EnterFurtherEnqTabInChecklist();
            cProjectDemoMethods.ClickAddFurtherEnq();
            cProjectDemoMethods.PopulateAndSaveFurtherEnqParams.UIDateEditText = "11";
            cProjectDemoMethods.PopulateAndSaveFurtherEnq();
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        #endregion

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
        private TestContext testContextInstance;
    }
}
