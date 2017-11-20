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


namespace Auto_Tests.Coded_UI_Tests.Expenses.Base_Information.Project_Codes
{
    /// <summary>
    /// Summary description for Project Codes Tests
    /// </summary>
    [CodedUITest]
    public class ExpensesProjectCodesTests
    {
        public ExpensesProjectCodesTests()
        {
        }
         
        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.ProjectCodesUIMapClasses.ProjectCodesUIMap cProjectCodes = new UIMaps.ProjectCodesUIMapClasses.ProjectCodesUIMap();


        [TestMethod]
        public void ProjectCodesSuccessfullyCreateProjectCode()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Project codes page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminprojectcodes.aspx");

            /// Click Add Project Code and populate fields
            cProjectCodes.ClickAddProjectCodeLink();
            cProjectCodes.PopulateProjectCodeDetails();            
            
            /// Press save and validate
            cProjectCodes.PressProjectCodeSaveButton();
            cProjectCodes.ValidateProjectCodeExists();
            
            /// Press edit and validate
            cProjectCodes.ClickProjectCodeEditIcon();
            cProjectCodes.ValidateProjectCodeDetails();
        }


        [TestMethod]
        public void ProjectCodesUnsuccessfullyCreateDuplicateProjectCode()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Project codes page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminprojectcodes.aspx");

            /// Click Add Project Code and populate fields
            cProjectCodes.ClickAddProjectCodeLink();
            cProjectCodes.PopulateProjectCodeDetails();

            /// Press save and validate duplicate details message
            cProjectCodes.PressProjectCodeSaveButton();
            cProjectCodes.ValidateDuplicateProjectCodeMessage();
            cProjectCodes.ClickDuplicateProjectCodeMessageOK();

        }


        [TestMethod]
        public void ProjectCodesSuccessfullyEditProjectCode()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Project codes page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminprojectcodes.aspx");

            /// Click edit Project Code and populate fields
            cProjectCodes.ClickProjectCodeEditIcon();
            cProjectCodes.PopulateEditedProjectCodeDetails();

            /// Press save and validate
            cProjectCodes.PressProjectCodeSaveButton();
            cProjectCodes.ValidateProjectCodeEdited();

            /// Press edit and validate
            cProjectCodes.ClickProjectCodeEditIcon();
            cProjectCodes.ValidateProjectCodeEditedDetails();
        }


        [TestMethod]
        public void ProjectCodesSuccessfullyArchiveProjectCode()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Project codes page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminprojectcodes.aspx");

            /// Click archive Project Code
            cProjectCodes.PressProjectCodeArchiveIcon();
            cProjectCodes.ValidateProjectCodeArchived();
        }


        [TestMethod]
        public void ProjectCodesSuccessfullyUnarchiveProjectCode()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Project codes page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminprojectcodes.aspx");

            /// Click un-archive Project Code
            cProjectCodes.PressProjectCodeUnarchiveIcon();
            cProjectCodes.ValidateProjectCodeNotArchived();
        }


        [TestMethod]
        public void ProjectCodesSuccessfullyDeleteProjectCode()
        {
            /// Logon to expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the Project codes page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminprojectcodes.aspx");

            /// Click delete Project Code
            cProjectCodes.PressProjectCodeDeleteIcon();
            cProjectCodes.ValidateProjectCodeDoesNotExist();
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
