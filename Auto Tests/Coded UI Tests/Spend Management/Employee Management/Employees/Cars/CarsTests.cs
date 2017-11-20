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


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Cars
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class CarsTests
    {
        public CarsTests()
        {
        }


        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.CarsUIMapClasses.CarsUIMap cCars = new UIMaps.CarsUIMapClasses.CarsUIMap();


        [TestMethod]
        public void CarsSuccessfullyAddEmployeeCar()
        {
            /// Logon to Expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to Employees Page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            
            /// View employee
            cCars.FindEmployeeForCarParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cCars.FindEmployeeForCar();
            cCars.EditEmployeeForCar();
            cCars.AddCarinEmployees();
            cCars.AddBasicCarDetails();
            cCars.ClickDutyofCareTab();
            cCars.CarTaxExpiryDateParams.UIExpiryDateEditText = "__/__/____";
            cCars.CarTaxExpiryDate();
            cSharedMethods.TypeInDate("01/01/2012");
            cCars.CarTaxLastCheckedDateParams.UILastCheckedEditText = "__/__/____";
            cCars.CarTaxLastCheckedDate();
            cSharedMethods.TypeInDate("01/01/2011");
            cCars.CarTaxCheckedBy();
            cCars.CarMOTTestNumber();
            cCars.CarMOTExpiryDateParams.UIExpiryDateEdit4Text = "__/__/____";
            cCars.CarMOTExpiryDate();
            cSharedMethods.TypeInDate("01/01/2012");
            cCars.CarMOTLastCheckedDateParams.UILastCheckedEdit4Text = "__/__/____";
            cCars.CarMOTLastCheckedDate();
            cSharedMethods.TypeInDate("01/01/2011");
            cCars.CarMOTCheckedBy();
            cCars.CarInsuranceNumber();
            cCars.CarInsuranceDetailsExpiryDateParams.UIExpiryDateEdit2Text = "__/__/____";
            cCars.CarInsuranceDetailsExpiryDate();
            cSharedMethods.TypeInDate("01/01/2012");
            cCars.CarInsuranceDetailsLastCheckedDateParams.UILastCheckedEdit2Text = "__/__/____";
            cCars.CarInsuranceDetailsLastCheckedDate();
            cSharedMethods.TypeInDate("01/01/2011");
            cCars.CarInsuranceCheckedBy();
            cCars.CarServiceDetailsExpiryDateParams.UIExpiryDateEditText = "__/__/____";
            cCars.CarServiceDetailsExpiryDate();
            cSharedMethods.TypeInDate("01/01/2012");
            cCars.CarServiceDetailsLastCheckedDateParams.UILastCheckedEdit3Text = "__/__/____";
            cCars.CarServiceDetailsLastCheckedDate();
            cSharedMethods.TypeInDate("01/01/2011");
            cCars.CarServiceDetailsCheckedBy();
            cCars.ClickOdometerReadingTab();
            cCars.AddOdometerReadingsCar();
            cCars.SaveCarinEmployee();
            cCars.AsserCarModel();
            cCars.AssertCaeRegistration();
            cCars.AssertCarFuelType();
            cCars.AssertCarStatus();            
        }


        [TestMethod]
        public void CarsSuccessfullyDeleteEmployeeCar()
        {
            /// Logon to Expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to Employees Page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            cCars.FindEmployeeForCarParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cCars.FindEmployeeForCar();
            cCars.EditEmployeeForCar();
            cCars.SelectCars();
            cCars.DeleteCar();
            cCars.SelectCars();
            cCars.AssertCarDeleted();

        }


        [TestMethod]
        public void CarsSuccessfullyEditEmployeeCar()
        {
            /// Logon to Expenses
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to Employees Page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            cCars.FindEmployeeForCarParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(ProductType.expenses);
            cCars.FindEmployeeForCar();
            cCars.EditEmployeeForCar();
            cCars.SelectCars();
            cCars.EditEmployeeCar();
            cCars.EditCarGeneralDetails();
            cCars.ClickDutyofCareTab();
            cCars.EditCarTaxExpiryDateParams.UIExpiryDateEditText = "__/__/____";
            cCars.EditCarTaxExpiryDate();
            cSharedMethods.TypeInDate("02/01/2012");
            cCars.EditCarTaxLastCheckedDateParams.UILastCheckedEditText = "__/__/____";
            cCars.EditCarTaxLastCheckedDate();
            cSharedMethods.TypeInDate("02/01/2012");
            cCars.EditCarTaxCheckedBy();
            cCars.EditCarMOTNumber();
            cCars.EditCarMOTExpiryDateParams.UIExpiryDateEdit1Text = "__/__/____";
            cCars.EditCarMOTExpiryDate();
            cSharedMethods.TypeInDate("02/01/2012");
            cCars.EditCarMOTLastCheckedDateParams.UILastCheckedEdit1Text = "__/__/____";
            cCars.EditCarMOTLastCheckedDate();
            cSharedMethods.TypeInDate("02/01/2012");
            cCars.EditCarMOTCheckedBy();
            cCars.EditCarInsuranceNumber();
            cCars.EditCarInsuranceExpiryDateParams.UIExpiryDateEdit2Text = "__/__/____";
            cCars.EditCarInsuranceExpiryDate();
            cSharedMethods.TypeInDate("02/01/2012");
            cCars.EditCarInsuranceLastCheckedDateParams.UILastCheckedEdit2Text = "__/__/____";
            cCars.EditCarInsuranceLastCheckedDate();
            cSharedMethods.TypeInDate("02/01/2012");
            cCars.EditCarInsuranceCheckedBy();
            cCars.EditCarServiceDetailsExpiryDateParams.UIExpiryDateEdit3Text = "__/__/____";
            cCars.EditCarServiceDetailsExpiryDate();
            cSharedMethods.TypeInDate("02/01/2012");
            cCars.EditCarServiceLastCheckedDateParams.UILastCheckedEdit3Text = "__/__/____";
            cCars.EditCarServiceLastCheckedDate();
            cSharedMethods.TypeInDate("02/01/2012");
            cCars.EditCarServiceCheckedBy();
            cCars.ClickOdometerReadingTab();
            cCars.EditCarOdometerReadingDetails();
            cCars.EditCarAddOdometerReading();
            cCars.EditCarAddReadingDateParams.UIReadingdateEditText = "__/__/____";
            cCars.EditCarAddReadingDate();
            cSharedMethods.TypeInDate("02/01/2012");
            cCars.EditCarAddOdometerReadings();
            cCars.EditCarSaveOdometerReading();
            cCars.EditCarSaveChanges();

            /// Assert the edit
            cCars.ValidateCarEdited();
            cCars.EditEmployeeForCar2();
            cCars.ValidateEditedCarGeneralDetails();
            cCars.ValidateEditedCarDutyofCareExpectedValues.UICheckedByEdit1Text = "newtest, newtest [xasdf]";
            cCars.ValidateEditedCarDutyofCareExpectedValues.UICheckedByEditText = "Test2, New [xxxNew2]";
            cCars.ValidateEditedCarDutyofCareExpectedValues.UICheckedByEdit2Text = "Test2, New [xxxNew2]";
            cCars.ValidateEditedCarDutyofCareExpectedValues.UICheckedByEdit3Text = "Test2, New [xxxNew2]";
            cCars.ValidateEditedCarDutyofCare();
            cCars.ValidateEditedCarOdometerReadings();            
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
