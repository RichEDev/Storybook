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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Generic_Information.Task_Types
{
    /// <summary>
    /// Summary description for Task Types
    /// </summary>
    [CodedUITest]
    public class TaskTypesTests
    {
        public TaskTypesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.TaskTypesUIMapClasses.TaskTypesUIMap cTaskTypes = new UIMaps.TaskTypesUIMapClasses.TaskTypesUIMap();

        /// <summary>
        /// This test ensures that a new task type can be successfully created
        /// </summary>
        [TestMethod]
        public void TaskTypesSuccessfullyCreateNewTaskType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the task types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=120");

            /// Ensure the cancel button can be used
            cTaskTypes.AddTaskTypeWithCancel();
            cTaskTypes.ValidateTaskTypeDoesNotExist();

            /// Add task type
            cTaskTypes.AddTaskType();
            cTaskTypes.ValidateAddTaskType();
        }


        /// <summary>
        /// This test ensures that a duplicate task type cannot be created
        /// </summary>
        [TestMethod]
        public void TaskTypesUnSuccessfullyCreateNewTaskTypeWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the task types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=120");

            /// Add a duplicate task type
            cTaskTypes.AddTaskType();
            cTaskTypes.ValidateAddDuplicateTaskType();
        }


        /// <summary>
        /// This test ensures that a task type can be successfully edited
        /// </summary>
        [TestMethod]
        public void TaskTypesSuccessfullyEditTaskType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the task types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=120");

            /// Ensure the cancel button can be used
            cTaskTypes.EditTaskTypeWithCancel();
            cTaskTypes.ValidateAddTaskType();

            /// Edit a task type
            cTaskTypes.EditTaskType();
            cTaskTypes.ValidateEditTaskType();

            /// Reset the values for future tests
            cTaskTypes.EditTaskTypeResetValues();
        }


        /// <summary>
        /// This test ensures that a task type can be successfully deleted
        /// </summary>
        [TestMethod]
        public void TaskTypesSuccessfullyDeleteTaskType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the task types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=120");

            /// Ensure the cancel button can be used
            cTaskTypes.DeleteTaskTypeWithCancel();
            cTaskTypes.ValidateAddTaskType();

            /// Delete a task type
            cTaskTypes.DeleteTaskType();
            cTaskTypes.ValidateTaskTypeDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a task type can be successfully archived
        /// </summary>
        [TestMethod]
        public void TaskTypesSuccessfullyArchiveTaskType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the task types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=120");

            /// Ensure the cancel button can be used
            cTaskTypes.ArchiveTaskTypeWithCancel();
            cTaskTypes.ValidateTaskTypeIsNotArchived();

            /// Archive a task type
            cTaskTypes.ArchiveTaskType();
            cTaskTypes.ValidateTaskTypeIsArchived();
        }


        /// <summary>
        /// This test ensures that a task type can be successfully un-archived
        /// </summary>
        [TestMethod]
        public void TaskTypesSuccessfullyUnArchiveTaskType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            /// Navigate to the task types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=120");

            /// Ensure the cancel button can be used
            cTaskTypes.UnArchiveTaskTypeWithCancel();
            cTaskTypes.ValidateTaskTypeIsArchived();

            /// Un-archive a task type
            cTaskTypes.UnArchiveTaskType();
            cTaskTypes.ValidateTaskTypeIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the task types page layout is correct
        /// </summary>
        [TestMethod]
        public void TaskTypesSuccessfullyValidateTaskTypesPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the task types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=120");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cTaskTypes.ValidateTaskTypesPageLayoutExpectedValues.UIJamesLloyd20SeptembePaneDisplayText =
                cGlobalVariables.EntireUsername(ProductType.framework, LogonType.administrator);

            /// Validate page layout
            cTaskTypes.ValidateTaskTypesPageLayout();
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
