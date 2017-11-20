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


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Home_Locations
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class HomeLocationTests
    {
        public HomeLocationTests()
        {
        }


        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.EmployeeHomeLocationsUIMapClasses.EmployeeHomeLocationsUIMap cEmployeeHomeLocationMethods = new UIMaps.EmployeeHomeLocationsUIMapClasses.EmployeeHomeLocationsUIMap();


        public void deletemethod()
        {
            cDatabaseConnection dbo = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.expenses));
            dbo.ExecuteSQL("DELETE FROM companies WHERE Company='mannys home'");

        }

        /// <summary>
        /// In order for this test to pass the Addresses should be ordered by date
        /// </summary>
        [TestMethod]
        public void EmployeesSuccessfullyAddExisitngHomeAddress()
        {

            #region Shared Admin Exp logon

            //Login Successfully as Administrator
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            #endregion

            //Navigate to Employees
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            //Select and edit test employee
            cEmployeeHomeLocationMethods.SearchTestEmployee();
            cEmployeeHomeLocationMethods.EditTestEmployee();

            //Select Home Addresses
            cEmployeeHomeLocationMethods.ClickHomeAddresses();

            //Add Home Address
            cEmployeeHomeLocationMethods.ClickNewHomeAddress();

            cEmployeeHomeLocationMethods.EnterExistingAddresssParams.UIAddressEditText = "CodedUI Address";

            cEmployeeHomeLocationMethods.EnterExistingAddresss();
            cEmployeeHomeLocationMethods.EnterAddressStartDate();
            //cEmployeeHomeLocationMethods.EnterAddressStartDateManually();
            cEmployeeHomeLocationMethods.PressSave();

            //Verify adding new home address

            cEmployeeHomeLocationMethods.AssertAddingNewHomeAddressExpectedValues.UIDarrensHouseCellInnerText = "CodedUI Address";
            cEmployeeHomeLocationMethods.AssertAddingNewHomeAddressExpectedValues.UIItem9AnsonCloseCellInnerText = "Low Moor Road";
            cEmployeeHomeLocationMethods.AssertAddingNewHomeAddressExpectedValues.UILN65THCellInnerText = "LN6 3JY";

            cEmployeeHomeLocationMethods.AssertAddingNewHomeAddressExpectedValues.UIItem28032011CellInnerText = DateTime.Now.ToString("dd/MM/yyyy");
            cEmployeeHomeLocationMethods.AssertAddingNewHomeAddress();

        }


        [TestMethod]
        public void EmployeesSuccessfullyEditExistingHomeAddress()
        {
            #region Shared Admin Exp logon

            //Login Successfully as Administrator
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            #endregion

            //Navigate to Employees
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            //Select and edit test employee
            cEmployeeHomeLocationMethods.SearchTestEmployee();
            cEmployeeHomeLocationMethods.EditTestEmployee();

            //Select Home Addresses
            cEmployeeHomeLocationMethods.ClickHomeAddresses();

            //Edit Home Address
            cEmployeeHomeLocationMethods.ClickEditHomeAddress();

            cEmployeeHomeLocationMethods.EnterExistingAddresssParams.UIAddressEditText = "New CodedUI Address";
           
            cEmployeeHomeLocationMethods.EnterExistingAddresss();
            cEmployeeHomeLocationMethods.EnterNewAddressStartDate();
            cEmployeeHomeLocationMethods.PressSave();

            //Verify editing new Home address

            cEmployeeHomeLocationMethods.AssertEdditingHomeAddressExpectedValues.UIItem11aOldRectoryGarCellInnerText = "Hibaldstow Road";
            cEmployeeHomeLocationMethods.AssertEdditingHomeAddressExpectedValues.UILN12FECellInnerText = "LN6 3PX";
            cEmployeeHomeLocationMethods.AssertEdditingHomeAddressExpectedValues.UIMartinshouseCellInnerText = "New CodedUI Address";

            cEmployeeHomeLocationMethods.AssertEdditingHomeAddressExpectedValues.UIItem28032011CellInnerText = DateTime.Now.ToString("dd/MM/yyyy");
            cEmployeeHomeLocationMethods.AssertEdditingHomeAddress();
        }

        [TestMethod]
        public void EmployeesSuccessfullyDeleteHomeAddress()
        {

            #region Shared Admin Exp logon
  
            //Login Successfully as Administrator
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            #endregion

            //Navigate to Employees
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            //Select and edit test employee
            cEmployeeHomeLocationMethods.SearchTestEmployee();
            cEmployeeHomeLocationMethods.EditTestEmployee();

            //Select Home Addresses
            cEmployeeHomeLocationMethods.ClickHomeAddresses();

            //Delete Home Address
            cEmployeeHomeLocationMethods.DeleteHomeAddress();

            //Verify Deleting Home Address
            //cEmployeeHomeLocationMethods.AssertDeletingHomeAddress();  //ensures that address is deleted and no address exists on the grid
            cEmployeeHomeLocationMethods.ValidateDeletingHomeAddress(); //ensures that address is deleted from the grid even if the grid is populated

          }


        /// <summary>
        /// In order for this test to work, the address 'mannys home' must not exist
        /// </summary>
        [TestMethod]
        public void EmployeesSuccessfullyAddNewHomeAddress()
        {

            #region Shared Admin Exp logon

            //Login Successfully as Administrator
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            #endregion

            //Navigate to Employees
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            //Select and edit test employee
            cEmployeeHomeLocationMethods.SearchTestEmployee();
            cEmployeeHomeLocationMethods.EditTestEmployee();

            //Select Work Addresses
            cEmployeeHomeLocationMethods.ClickHomeAddresses();

            //Add Work Address
            cEmployeeHomeLocationMethods.ClickNewHomeAddress();
            cEmployeeHomeLocationMethods.EnterNewUniqueAddress();
            cEmployeeHomeLocationMethods.EnterAddressStartDate();
            //cEmployeeHomeLocationMethods.EnterAddressStartDateManually();
            cEmployeeHomeLocationMethods.PressSave();

            //Verify adding new work address
            cEmployeeHomeLocationMethods.AssertAddingUniqueWorkAddressExpectedValues.UIItem28032011CellInnerText = DateTime.Now.ToString("dd/MM/yyyy");
            cEmployeeHomeLocationMethods.AssertAddingUniqueWorkAddress();

            EmployeesSuccessfullyDeleteHomeAddress();
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
