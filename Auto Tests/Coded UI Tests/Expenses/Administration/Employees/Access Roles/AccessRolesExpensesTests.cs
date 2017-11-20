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


namespace Auto_Tests.Coded_UI_Tests.Expenses.Administration.Employees.Access_Roles
{
    /// <summary>
    /// Summary description for AccessRolesExpensesTests
    /// </summary>
    [CodedUITest]
    public class AccessRolesExpensesTests
    {
        public AccessRolesExpensesTests()
        {
        }


        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.AccessRolesUIMapClasses.AccessRolesUIMap cAccessRoles = new UIMaps.AccessRolesUIMapClasses.AccessRolesUIMap();


        /// <summary>
        /// #### - Successfully Create Access Role for Expenses
        /// Set reporting access to Data from employees they approve
        /// </summary>
        [TestMethod]
        public void AccessRolesSuccessfullyCreateAccessRoleWithinExpenses()
        {
            //Logon as administrator user & load Access Role Summary page.
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/accessRoles.aspx");

            //If access role already exists then delete it
            if (cAccessRoles.UIAccessRolesWindowsInWindow.UIAccessRolesDocument.UITbl_accessRolesGridTable1.AccessRolesTable.InnerText.Contains(cAccessRoles.ValidateAccessRoleCorrectOnSummaryPageExpectedValues.UI__CodedUIAccessRoleCellInnerText))
            {
                cAccessRoles.SelectDeleteAccessRole();
            }

            //Create new access role.
            cAccessRoles.SelectNewAccessRole();
            cAccessRoles.EnterAccessRoleNameAndDescription();


            //Set Reporting level.
            cAccessRoles.SelectReportAccessAllData();
            cAccessRoles.SelectAccessRolesToEmployeesTheyApprove();

            //Set Employees permissions for amending Project/Cost/Department codes.
            cAccessRoles.SetEmployeesCodeAccess();

            //Set employees max/min claim amounts.
            cAccessRoles.SetMinMaxClaimAmounts();

            //Set Full (View,Add,Edit,Delete) for all Access Role Elements
            cAccessRoles.SetViewAddEditDeleteForAllElementsExpenses();


            cAccessRoles.SelectSave();

            //Check tha the access role has been created successfully
            cAccessRoles.ValidateAccessRoleCorrectOnSummaryPage();
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
