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


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Work_Locations
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class WorkLocationTests
    {
        public WorkLocationTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.EmployeeWorkLocationsClasses.EmployeeWorkLocations cEmployeeWorkLocationMethods = new UIMaps.EmployeeWorkLocationsClasses.EmployeeWorkLocations();

        public void deletemethod()
        {
            cDatabaseConnection dbo = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.expenses));
            dbo.ExecuteSQL("DELETE FROM companies WHERE Company='mannys home'");

        }


        [TestMethod]
        public void EmployeesSuccessfullyAddExisitngWorkAddress()
        {

            #region Shared Admin Exp logon

            //Login Successfully as Administrator
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            #endregion

            //Navigate to Employees
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            //Select and edit test employee
            cEmployeeWorkLocationMethods.SearchTestEmployee();
            cEmployeeWorkLocationMethods.EditTestEmployee();

            //Select Work Addresses
            cEmployeeWorkLocationMethods.ClickWorkAddresses();

            //Add Work Address
            cEmployeeWorkLocationMethods.ClickNewWorkAddress();

            cEmployeeWorkLocationMethods.EnterExistingAddressParams.UIAddressEditText = "CodedUI Address";
            
            cEmployeeWorkLocationMethods.EnterExistingAddress();
            cEmployeeWorkLocationMethods.EnterAddressStartDate();
            //cEmployeeHomeLocationMethods.EnterAddressStartDateManually();
            cEmployeeWorkLocationMethods.PressSave();

            //Verify adding new work address
            cEmployeeWorkLocationMethods.AssertAddingNewWorkAddressExpectedValues.UIItem28032011CellInnerText = DateTime.Now.ToString("dd/MM/yyyy");

            cEmployeeWorkLocationMethods.AssertAddingNewWorkAddressExpectedValues.UIDarrensHouseCellInnerText = "CodedUI Address";
            cEmployeeWorkLocationMethods.AssertAddingNewWorkAddressExpectedValues.UIItem9AnsonCloseCellInnerText = "Low Moor Road";
            cEmployeeWorkLocationMethods.AssertAddingNewWorkAddressExpectedValues.UILN65THCellInnerText = "LN6 3JY";

            cEmployeeWorkLocationMethods.AssertAddingNewWorkAddress();

        }

        [TestMethod]
        public void EmployeesSuccessfullyEditExistingWorkAddress()
        {
            #region Shared Admin Exp logon

            //Login Successfully as Administrator
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            #endregion

            //Navigate to Employees
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            //Select and edit test employee
            cEmployeeWorkLocationMethods.SearchTestEmployee();
            cEmployeeWorkLocationMethods.EditTestEmployee();

            //Select Work Addresses
            cEmployeeWorkLocationMethods.ClickWorkAddresses();

            //Edit Work Address
            cEmployeeWorkLocationMethods.ClickEditWorkAddress();

            cEmployeeWorkLocationMethods.EnterNewAddressParams.UIAddressEdit1Text = "New CodedUI Address";

            cEmployeeWorkLocationMethods.EnterNewAddress();
            cEmployeeWorkLocationMethods.EnterNewAddressStartDate();
            cEmployeeWorkLocationMethods.PressSave();

            //Verify editing new work address

            cEmployeeWorkLocationMethods.AssertEditingWorkAddressExpectedValues.UIMartinshouseCellInnerText = "New CodedUI Address";
            cEmployeeWorkLocationMethods.AssertEditingWorkAddressExpectedValues.UIItem11aOldRectoryGarCellInnerText = "Hibaldstow Road";
            cEmployeeWorkLocationMethods.AssertEditingWorkAddressExpectedValues.UILN12FECellInnerText = "LN6 3PX";

            cEmployeeWorkLocationMethods.AssertEditingWorkAddressExpectedValues.UIItem28032011CellInnerText = DateTime.Now.ToString("dd/MM/yyyy");
            cEmployeeWorkLocationMethods.AssertEditingWorkAddress();
        }

        [TestMethod]
        public void EmployeesSuccessfullyDeleteWorkAddress()
        {

            #region Shared Admin Exp logon

            //Login Successfully as Administrator
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            #endregion

            //Navigate to Employees
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            //Select and edit test employee
            cEmployeeWorkLocationMethods.SearchTestEmployee();
            cEmployeeWorkLocationMethods.EditTestEmployee();

            //Select Work Addresses
            cEmployeeWorkLocationMethods.ClickWorkAddresses();

            //Delete Work Address
            cEmployeeWorkLocationMethods.DeleteWorkAddress();

            //Verify Deleting Work Address
            cEmployeeWorkLocationMethods.AssertDeletingWorkAddress();

        }

        [TestMethod]
        public void EmployeesSuccessfullyAddNewWorkAddress()
        {

            #region Shared Admin Exp logon

            //Login Successfully as Administrator
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            #endregion

            //Navigate to Employees
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            //Select and edit test employee
            cEmployeeWorkLocationMethods.SearchTestEmployee();
            cEmployeeWorkLocationMethods.EditTestEmployee();

            //Select Work Addresses
            cEmployeeWorkLocationMethods.ClickWorkAddresses();

            //Add Work Address
            cEmployeeWorkLocationMethods.ClickNewWorkAddress();
            cEmployeeWorkLocationMethods.EnterNewUniqueAddress();
            cEmployeeWorkLocationMethods.EnterAddressStartDate();
            //cEmployeeHomeLocationMethods.EnterAddressStartDateManually();
            cEmployeeWorkLocationMethods.PressSave();

            //Verify adding new work address
            cEmployeeWorkLocationMethods.AssertAddingUniqueWorkAddressExpectedValues.UIItem28032011CellInnerText = DateTime.Now.ToString("dd/MM/yyyy");
            cEmployeeWorkLocationMethods.AssertAddingUniqueWorkAddress();

            EmployeesSuccessfullyDeleteWorkAddress();
            deletemethod();
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
