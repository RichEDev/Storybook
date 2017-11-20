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


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Administration.Employees.Access_Roles
{
    /// <summary>
    /// Summary description for Access Roles Tests. These tests were written by Dylan - the logic in some of them is, therefore, quirky.
    /// </summary>
    [CodedUITest]
    public class AccessRolesTests
    {
        public AccessRolesTests()
        {
        }


        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.AccessRolesUIMapClasses.AccessRolesUIMap cAccessRoles = new UIMaps.AccessRolesUIMapClasses.AccessRolesUIMap();


        /// <summary>
        /// 28195 - Successfully Create Access Role
        /// </summary>
        [TestMethod]
        public void AccessRolesSuccessfullyCreateAccessRole()
        {          
            /// Logon and navigate to Access Role Summary page
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accessRoles.aspx");
     
            /// Select new access role link and populate basic details
            cAccessRoles.SelectNewAccessRole();
            cAccessRoles.EnterAccessRoleNameAndDescription();

            /// Set Reporting level
            //cAccessRoles.SelectReportAccessDataFollowingAccessRoles();
            //cAccessRoles.SelectReportAccessAllData();
            cAccessRoles.SelectReportAccessDataFollowingAccessRoles();
            cAccessRoles.SelectAccessRolesToReportFrom();

            /// Set full access (View,Add,Edit,Delete) for all Access Role Elements
            cAccessRoles.SetViewAddEditDeleteForAllElementsExpenses();
            cAccessRoles.SelectSave();

            /// Check that the access role has been created successfully
            cAccessRoles.ValidateAccessRoleCorrectOnSummaryPage();          
        }


        /// <summary>
        /// 28196 - Successfully Edit Access Role
        /// </summary>
        [TestMethod]
        public void AccessRolesSuccessfullyEditAccessRole()
        {
            /// Logon to Expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to access role page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accessRoles.aspx");

            /// Edit the access role
            cAccessRoles.SelectEditAccessRole();

            /// Set new values and edit access role record
            cAccessRoles.EnterAccessRoleNameAndDescriptionParams.UIRoleNameEditText = "__Coded UI Access Role - Updated";
            cAccessRoles.EnterAccessRoleNameAndDescriptionParams.UIDescriptionEditText = "__Coded UI Access Role Description - Edited";
            cAccessRoles.EnterAccessRoleNameAndDescription();

            /// Set reporting levels
            cAccessRoles.SelectReportAccessAllData();

            /// Clear all of the Add, Edit, Delete, View and save
            cAccessRoles.UnSelectAllExpensesElements();
            cAccessRoles.SelectSave();

            /// Check that the access roles has been updated successfully
            cAccessRoles.ValidateAccessRoleCorrectOnSummaryPageExpectedValues.UI__CodedUIAccessRoleCellInnerText = "__Coded UI Access Role - Updated";
            cAccessRoles.ValidateAccessRoleCorrectOnSummaryPage();

            /// Edit the access role and ensure it has been updated
            cAccessRoles.SelectEditAccessRole();
            cAccessRoles.ValidateAccessRoleEditedDetails();
            cAccessRoles.ValidateExpensesElementsNotSelected();
        }


        /// <summary>
        /// 28198 - Successfully Delete Access Role
        /// </summary>
        [TestMethod]
        public void AccessRolesSuccessfullyDeleteAccessRoleRecord()
        {
            /// Logon to Expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to access role page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accessRoles.aspx"); 

            /// Press the delete access role icon and press OK
            cAccessRoles.SelectDeleteAccessRole();

            //Check that the access roles has been deleted successfully
            cAccessRoles.ValidateAccessRoleCorrectNotOnSummaryPage();
        }


        /// <summary>
        /// 28199 - Successfully Successfully Cancel Access Role Record Where Chagnes Have Been Made
        /// </summary>
        [TestMethod]
        public void AccessRolesSuccessfullyCancelAccessRoleRecordWhereChangesHaveBeenMade()
        {
            /// Logon to Expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to access role page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accessRoles.aspx"); 
            
            /// Enter new access role information
            cAccessRoles.SelectNewAccessRole();
            cAccessRoles.EnterAccessRoleNameAndDescriptionParams.UIRoleNameEditText = "This will not be saved";
            cAccessRoles.EnterAccessRoleNameAndDescriptionParams.UIDescriptionEditText = "This will not be saved - desc";
            cAccessRoles.EnterAccessRoleNameAndDescription();

            /// Cancel the new access role page
            cAccessRoles.SelectCancel();

            /// Check that the access roles has been deleted successfully
            cAccessRoles.ValidateAccessRoleCorrectNotOnSummaryPageExpectedValues.UI__CodedUIAccessRoleCellInnerText = "This will not be saved";
            cAccessRoles.ValidateAccessRoleCorrectNotOnSummaryPage();
        }


        /// <summary>
        /// 28233 - Unsuccessfully Save New Access Role Where Role Name Already Exists
        /// </summary>
        [TestMethod]
        public void AccessRolesUnsuccessfullySaveNewAccessRoleWhereRoleNameAlreadyExists()
        {
            /// Logon to Expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to access role page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accessRoles.aspx"); 

            for(int x = 0; x < 2; x++)
            {
                /// Enter new access role information and save new record
                cAccessRoles.SelectNewAccessRole();
                cAccessRoles.EnterAccessRoleNameAndDescriptionParams.UIRoleNameEditText = "Duplicate Access Role";
                cAccessRoles.EnterAccessRoleNameAndDescriptionParams.UIDescriptionEditText = "Duplicate Access Role - desc";
                cAccessRoles.EnterAccessRoleNameAndDescription();
                cAccessRoles.SelectSave();
            }   

            /// Check that an user validation error message has been raised
            cAccessRoles.ValidateDuplicateAccessRoleNameErrorMsg();

            /// Close error message
            cAccessRoles.SelectCloseDuplicateAccessRoleErrorMsg();

            /// Cancel the new access role page
            cAccessRoles.SelectCancel();

            /// Remove the test duplicate access role
            cAccessRoles.DeleteDuplicateAccessRole();               
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
