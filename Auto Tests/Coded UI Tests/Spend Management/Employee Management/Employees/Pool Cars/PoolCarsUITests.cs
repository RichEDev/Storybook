
namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Pool_Cars
{
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

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Cars;

    using Auto_Tests.UIMaps.CarsNewUIMapClasses;
    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
    using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
    using Auto_Tests.UIMaps.PoolCarsUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;

    using Auto_Tests.Product_Variables.PageUrlSurfices;


    /// <summary>
    /// These tests ensure that pool cars can be added and removed from employee records. Currently, these tests do not test the functionality of
    /// Pool Cars throughout the product.
    /// </summary>
    [CodedUITest]
    public class PoolCarsUITests
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private static readonly ProductType ExecutingProduct = cGlobalVariables.GetProductFromAppConfig();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();

        /// <summary>
        /// pool cars methods UI Map
        /// </summary>
        private static PoolCarsUIMap poolCarsMethods;

        /// <summary>
        /// employees methods UI Map
        /// </summary>
        private static EmployeesNewUIMap employeesMethods;

        /// <summary>
        /// assignments methods UI Map
        /// </summary>
        private static ESRAssignmentsUIMap esrAssignmentsMethods;

        /// <summary>
        /// cars methods UI Map
        /// </summary>
        private static CarsNewUIMap carsMethods;

        /// <summary>
        /// Cached list of Employees
        /// </summary>
        private static List<Employees> employees;

        /// <summary>
        /// Cached list of cars
        /// </summary>
        private static List<Cars> cars;

        #region Additional test attributes

        /// <summary>
        /// The class init.
        /// </summary>
        /// <param name="ctx">
        /// The ctx.
        /// </param>
        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            _sharedMethods.Logon(ExecutingProduct, LogonType.administrator);
            employees = EmployeesRepository.PopulateEmployee();
            cars = CarsRepository.PopulateCar();
        }

        /// <summary>
        /// The class clean up.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanUp()
        {
            _sharedMethods.CloseBrowserWindow();
        }

        /// <summary>
        /// The my test initialize.
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Guid genericName = Guid.NewGuid();
            employees[0].UserName = string.Format(
                "{0}{1}", new object[] { "Custom Emp ", genericName });
            int empId = EmployeesRepository.CreateEmployee(employees[0], ExecutingProduct);
            Assert.IsTrue(empId > 0);
            genericName = Guid.NewGuid();
            employees[1].UserName = string.Format(
                "{0}{1}", new object[] { "Custom Emp ", genericName });
            empId = EmployeesRepository.CreateEmployee(employees[1], ExecutingProduct);
            Assert.IsTrue(empId > 0);
            foreach (var car in cars)
            {
                car.employeeID = 0;
                car.createdBy = employees[0].employeeID;
                car.insuranceCheckedBy = car.createdBy;
                car.motCheckedBy = car.insuranceCheckedBy;
                car.serviceCheckedby = car.insuranceCheckedBy;
                car.taxCheckedBy = car.insuranceCheckedBy;
            }

            employeesMethods = new EmployeesNewUIMap();
            poolCarsMethods = new PoolCarsUIMap();
            esrAssignmentsMethods = new ESRAssignmentsUIMap();
            carsMethods = new CarsNewUIMap();
        }

        /// <summary>
        /// The my test cleanup.
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {            
            foreach (var car in cars)
            {
                if (car.carID > 0)
                {
                    CarsRepository.DeletePoolCarUser(car.carID, employees[0].employeeID, ExecutingProduct);
                    CarsRepository.DeleteCar(car.carID, car.employeeID.Value, car.createdBy, ExecutingProduct);
                }
            }
            foreach (var employee in employees)
            {
                if (employee.employeeID > 0)
                {
                    EmployeesRepository.DeleteEmployee(employee.employeeID, ExecutingProduct);
                }
            }

        }

        #endregion

        /// <summary>
        /// The successfully add pool car to employee
        /// </summary>
        [TestCategory("Pool Cars"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void PoolCarsSuccessfullyAddPoolCarsToEmployee_UITest()
        {
            Employees employeeToEdit = employees[0];
            Cars poolCarToAdd = cars[0];

            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // Navigate to test employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // click cars link
            poolCarsMethods.ClickPoolCarsLink();

            // Go to the pool cars page
            poolCarsMethods.ClickNewPoolCarLink();

            // Select poolcar to add
            poolCarsMethods.EmployeePoolCarControlsWindow.EmployeePoolCarControlsDocument.PoolCarComboBox.SelectedItem = poolCarToAdd.CarIdentity;

            // press save pool car
            poolCarsMethods.PressSavePoolCar();

            //validate grid
            esrAssignmentsMethods.ValidateGrid(this.PoolCarGrid, poolCarToAdd.CarGridValues);
        }

        /// <summary>
        /// The successfully delete pool car from employee
        /// </summary>
        [TestCategory("Pool Cars"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void PoolCarsSuccessfullyDeletePoolCarsFromEmployee_UITest()
        {
            Employees employeeToEdit = employees[0];
            Cars poolCarToDelete = cars[0];

            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // Navigate to test employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // click cars link
            poolCarsMethods.ClickPoolCarsLink();

            // click delete pool car
            esrAssignmentsMethods.ClickDeleteGridRow(this.PoolCarGrid, poolCarToDelete.make);

            // Confirm delete message
            employeesMethods.PressOkOnBrowserValidation();

            // validate record deleted
            esrAssignmentsMethods.ValidateGrid(this.PoolCarGrid, poolCarToDelete.CarGridValues, false);
        }

        /// <summary>
        /// The import data to testing database.
        /// </summary>
        /// <param name="testName">
        /// The test name.
        /// </param>
        internal void ImportDataToTestingDatabase(string testName)
        {
            int carID = 0;
            switch (testName)
            {
                case "PoolCarsSuccessfullyDeletePoolCarsFromEmployee_UITest":
                    carID = CarsRepository.createCar(cars[0], ExecutingProduct);
                    Assert.IsTrue(carID > 0);
                    CarsRepository.AddPoolCarUser(carID, employees[0].employeeID, ExecutingProduct);
                    break;
                case "PoolCarsSuccessfullyAddPoolCarsToEmployee_UITest":
                    carID = CarsRepository.createCar(cars[0], ExecutingProduct);
                    Assert.IsTrue(carID > 0);
                    break;
                default:
                    carID = CarsRepository.createCar(cars[0], ExecutingProduct);
                    Assert.IsTrue(carID > 0);
                    break;
            }
        }

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

        /// <summary>
        /// The Pool Cars Grid
        /// </summary>
        public Microsoft.VisualStudio.TestTools.UITesting.HtmlControls.HtmlTable PoolCarGrid 
        { 
            get 
            {
                return poolCarsMethods.PoolCarGridWindow.PoolCarGridDocument.PoolCarsGrid;
            } 
        }
    }
}
