namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Cars
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Windows.Input;

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;
    using Auto_Tests.Product_Variables.ModalMessages;
    using Auto_Tests.Product_Variables.PageUrlSurfices;
    using Auto_Tests.Tools;
    using Auto_Tests.UIMaps.CarsNewUIMapClasses;
    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
    using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;

    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;

    /// <summary>
    /// Summary description for CarsUITests
    /// </summary>
    [CodedUITest]
    public class CarsUITests
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
            foreach( var employee in employees)
            {
                employee.employeeCars = CarsRepository.PopulateCar(employee.employeeID, Cars.SqlItem);
            }

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
            foreach (var employee in employees)
            {
                foreach (var car in employee.employeeCars)
                {
                    car.employeeID = employee.employeeID;
                    car.createdBy = employee.employeeID;
                    car.insuranceCheckedBy = employee.employeeID;
                    car.motCheckedBy = car.insuranceCheckedBy;
                    car.serviceCheckedby = car.insuranceCheckedBy;
                    car.taxCheckedBy = car.insuranceCheckedBy;
                }
            }

            employeesMethods = new EmployeesNewUIMap();
            esrAssignmentsMethods = new ESRAssignmentsUIMap();
            carsMethods = new CarsNewUIMap();
        }

        /// <summary>
        /// The my test cleanup.
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (var employee in employees)
            {
                if (employee.employeeID > 0)
                {
                    EmployeesRepository.DeleteEmployee(employee.employeeID, ExecutingProduct);
                }
            }
        }

        #endregion

        #region Successfully add car to employee
        /// <summary>
        /// successfully add car to employee
        /// </summary>
        [TestCategory("Cars"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void CarsSuccessfullyAddEmployeeCar_UITest()
        {
            Employees employeeToEdit = employees[0];
            Employees lineManager = employees[1];
            Cars carToAdd = employeeToEdit.employeeCars[0];

            // Navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // Click cars link
            carsMethods.ClickCarsLink();

            // click add car link
            carsMethods.ClickNewCarLink();

            // populate car details
            #region General Details
            carsMethods.CarGeneralAdditionalControlsWindow.CarGeneralControlsDocument.NORadioButton.Selected = true;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.MakeTextBox.Text = carToAdd.make;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ModelTextBox.Text = carToAdd.model;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.RegistrationNumberTextBox.Text = carToAdd.regNumber;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.UnitofMeasureComboBox.SelectedItem = EnumHelper.GetEnumDescription((UnitOfMeasure)carToAdd.unitOfMeasure);
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineTypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((EngineType)carToAdd.carType);
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineSizeTextBox.Text = carToAdd.engineSize.ToString();
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.CarisactiveCheckBox.Checked = carToAdd.active;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ExemptfromHometoLocaCheckBox.Checked = carToAdd.exemptFromHomeToOffice;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.StartDateTextBox.Text = carToAdd.startDate.Value.ToShortDateString().Replace("/", "0");
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EndDateTextBox.Text = carToAdd.endDate.Value.ToShortDateString().Replace("/", "0");
            #endregion

            #region Duty of Care
            carsMethods.ClickDutyofCareTab();
            string lastCheckedBy = string.Format("{0}, {1} [{2}]", new object[] { lineManager.Surname, lineManager.FirstName, lineManager.UserName });
            Clipboard.SetText(lastCheckedBy);
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.TaxExpiryDateTextBox.Text = carToAdd.taxExpiry.Value.ToShortDateString().Replace("/", "0");
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.TaxLastCheckedTextBox.Text = carToAdd.taxLastChecked.Value.ToShortDateString().Replace("/", "0");
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.TaxCheckedByTextBox.SetFocus(); Keyboard.SendKeys("v", ModifierKeys.Control);

            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.MOTTestNumberTextBox.Text = carToAdd.motTestNumber;
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.MOTExpiryDateTextBox.Text = carToAdd.motExpiry.Value.ToShortDateString().Replace("/", "0");
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.MOTLastCheckedTextBox.Text = carToAdd.motLastChecked.Value.ToShortDateString().Replace("/", "0");
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.MOTCheckedByTextBox.SetFocus(); Keyboard.SendKeys("v", ModifierKeys.Control);

            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.InsuranceNumberTextBox.Text = carToAdd.insuranceNumber;
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.InsuranceExpiryDateTextBox.Text = carToAdd.insuranceExpiry.Value.ToShortDateString().Replace("/", "0");
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.InsuranceLastCheckedTextBox.Text = carToAdd.insuranceLastChecked.Value.ToShortDateString().Replace("/", "0");
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.InsuranceCheckedByTextBox.SetFocus(); Keyboard.SendKeys("v", ModifierKeys.Control);

            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.ServiceExpiryDateTextBox.Text = carToAdd.serviceExpiry.Value.ToShortDateString().Replace("/", "0");
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.ServiceLastCheckedTextBox.Text = carToAdd.serviceLastChecked.Value.ToShortDateString().Replace("/", "0");
            carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.ServiceCheckedByTextBox.SetFocus(); Keyboard.SendKeys("v", ModifierKeys.Control);
            Clipboard.Clear();
            #endregion

            #region Odometer Readings
            carsMethods.ClickOdometerTab();
            carsMethods.CarOdometerReadingControlsWindow.CarOdometerReadingControlsDocument.OdometerReadingRequiredCheckBox.Checked = carToAdd.fuelCard;
            carsMethods.CarOdometerReadingControlsWindow.CarOdometerReadingControlsDocument.StartOdometerReadingTextBox.Text = carToAdd.odometer.ToString();
            carsMethods.CarOdometerReadingControlsWindow.CarOdometerReadingControlsDocument.EndOdometerReadingTextBox.Text = carToAdd.endOdometer.ToString();
            #endregion

            // press save car
            carsMethods.PressSaveCarButton();

            // return to verify car saved
            esrAssignmentsMethods.ValidateGrid(this.CarGrid, carToAdd.CarGridValues);

            // click edit car
            esrAssignmentsMethods.ClickEditGridRow(this.CarGrid, carToAdd.make);

            #region validate General Details
            Assert.AreEqual(carToAdd.make, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.MakeTextBox.Text);
            Assert.AreEqual(carToAdd.model, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ModelTextBox.Text);
            Assert.AreEqual(carToAdd.regNumber, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.RegistrationNumberTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((UnitOfMeasure)carToAdd.unitOfMeasure), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.UnitofMeasureComboBox.SelectedItem);
            Assert.AreEqual(EnumHelper.GetEnumDescription((EngineType)carToAdd.carType), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineTypeComboBox.SelectedItem);
            Assert.AreEqual(carToAdd.active, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.CarisactiveCheckBox.Checked);
            Assert.AreEqual(carToAdd.exemptFromHomeToOffice, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ExemptfromHometoLocaCheckBox.Checked);
            Assert.AreEqual(carToAdd.startDate.Value.ToShortDateString(), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.StartDateTextBox.Text);
            Assert.AreEqual(carToAdd.endDate.Value.ToShortDateString(), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EndDateTextBox.Text);
            #endregion

            #region validate Duty of Care
            carsMethods.ClickDutyofCareTab();
            Assert.AreEqual(carToAdd.taxExpiry.Value.ToShortDateString(), carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.TaxExpiryDateTextBox.Text);
            Assert.AreEqual(carToAdd.taxLastChecked.Value.ToShortDateString(), carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.TaxLastCheckedTextBox.Text);
            Assert.AreEqual(lastCheckedBy, carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.TaxCheckedByTextBox.Text);


            Assert.AreEqual(carToAdd.motTestNumber, carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.MOTTestNumberTextBox.Text);
            Assert.AreEqual(carToAdd.motExpiry.Value.ToShortDateString(), carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.MOTExpiryDateTextBox.Text);
            Assert.AreEqual(carToAdd.motLastChecked.Value.ToShortDateString(), carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.MOTLastCheckedTextBox.Text);
            Assert.AreEqual(lastCheckedBy, carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.MOTCheckedByTextBox.Text);

            Assert.AreEqual(carToAdd.insuranceNumber, carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.InsuranceNumberTextBox.Text);
            Assert.AreEqual(carToAdd.insuranceExpiry.Value.ToShortDateString(), carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.InsuranceExpiryDateTextBox.Text);
            Assert.AreEqual(carToAdd.insuranceLastChecked.Value.ToShortDateString(), carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.InsuranceLastCheckedTextBox.Text);
            Assert.AreEqual(lastCheckedBy, carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.InsuranceCheckedByTextBox.Text);

            Assert.AreEqual(carToAdd.serviceExpiry.Value.ToShortDateString(), carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.ServiceExpiryDateTextBox.Text);
            Assert.AreEqual(carToAdd.serviceLastChecked.Value.ToShortDateString(), carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.ServiceLastCheckedTextBox.Text);
            Assert.AreEqual(lastCheckedBy, carsMethods.CarDutyOfCareControlsWindow.CarDutyOfCareControlsDocument.ServiceCheckedByTextBox.Text);
            #endregion

            #region validate Odometer Readings
            carsMethods.ClickOdometerTab();
            Assert.AreEqual(carToAdd.fuelCard, carsMethods.CarOdometerReadingControlsWindow.CarOdometerReadingControlsDocument.OdometerReadingRequiredCheckBox.Checked);
            Assert.AreEqual(carToAdd.odometer.ToString(), carsMethods.CarOdometerReadingControlsWindow.CarOdometerReadingControlsDocument.StartOdometerReadingTextBox.Text);
            Assert.AreEqual(carToAdd.endOdometer.ToString(), carsMethods.CarOdometerReadingControlsWindow.CarOdometerReadingControlsDocument.EndOdometerReadingTextBox.Text);
            #endregion

        }
        #endregion

        #region Successfully cancel add car to employee
        /// <summary>
        /// successfully cancel add car to employee
        /// </summary>
        [TestCategory("Cars"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void CarsSuccessfullyCancelAddEmployeeCar_UITest()
        {
            Employees employeeToEdit = employees[0];
            Cars carToAdd = employeeToEdit.employeeCars[0];

            // Navigate to employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // Click cars link
            carsMethods.ClickCarsLink();

            // click add car link
            carsMethods.ClickNewCarLink();

            // populate car details
            #region General Details
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.MakeTextBox.Text = carToAdd.make;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ModelTextBox.Text = carToAdd.model;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.RegistrationNumberTextBox.Text = carToAdd.regNumber;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.UnitofMeasureComboBox.SelectedItem = EnumHelper.GetEnumDescription((UnitOfMeasure)carToAdd.unitOfMeasure);
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineTypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((EngineType)carToAdd.carType);
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineSizeTextBox.Text = carToAdd.engineSize.ToString();
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.CarisactiveCheckBox.Checked = carToAdd.active;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ExemptfromHometoLocaCheckBox.Checked = carToAdd.exemptFromHomeToOffice;
            #endregion

            // press save car
            carsMethods.PressCancelCarButton();

            // return to verify car saved
            esrAssignmentsMethods.ValidateGrid(this.CarGrid, carToAdd.CarGridValues, false);

        }
        #endregion

        #region Unsuccessfully add car to employee without mandatory fields
        /// <summary>
        /// successfully add car to employee
        /// </summary>
        [TestCategory("Cars"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void CarsUnsuccessfullyAddEmployeeCarWithoutMandatoryFields_UITest()
        {
            Employees employeeToEdit = employees[0];

            // Navigate to test employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // Click cars link
            carsMethods.ClickCarsLink();

            // click add car link
            carsMethods.ClickNewCarLink();

            // press save car
            carsMethods.PressSaveCarButton();

            // validate modal message
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}\r\n\r\n{1}{2}{3}{4}", new object[] {ExecutingProduct, EmployeeModalMessages.EmptyFieldForCarMake, EmployeeModalMessages.EmptyFieldForCarModel, EmployeeModalMessages.EmptyFieldForCarRegistrationNumber, EmployeeModalMessages.EmptyFieldForEngineType, EmployeeModalMessages.EmptyFieldForEngineSize});

        }
        #endregion

        #region Successfully edit car to employee
        /// <summary>
        /// successfully add car to employee
        /// </summary>
        [TestCategory("Cars"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void CarsSuccessfullyEditEmployeeCar_UITest()
        {
            Employees employeeToEdit = employees[0];
            Cars carToEdit = employeeToEdit.employeeCars[0];
            Cars carToAdd = employeeToEdit.employeeCars[1];
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // Navigate to test employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // click cars link
            carsMethods.ClickCarsLink();

            // click edit car
            esrAssignmentsMethods.ClickEditGridRow(this.CarGrid, carToEdit.make);

            #region edit general details
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.MakeTextBox.Text = carToAdd.make;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ModelTextBox.Text = carToAdd.model;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.RegistrationNumberTextBox.Text = carToAdd.regNumber;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.UnitofMeasureComboBox.SelectedItem = EnumHelper.GetEnumDescription((UnitOfMeasure)carToAdd.unitOfMeasure);
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineTypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((EngineType)carToAdd.carType);
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineSizeTextBox.Text = carToAdd.engineSize.ToString();
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.CarisactiveCheckBox.Checked = carToAdd.active;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ExemptfromHometoLocaCheckBox.Checked = carToAdd.exemptFromHomeToOffice;
            if (carToAdd.startDate != null)
            {
                carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.StartDateTextBox.Text = carToAdd.startDate.Value.ToShortDateString().Replace("/", "0");
            }
            if (carToAdd.endDate != null)
            {
                carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EndDateTextBox.Text = carToAdd.endDate.Value.ToShortDateString().Replace("/", "0");
            }

            #endregion

            // press save
            carsMethods.PressSaveCarButton();

            // return to verify car saved
            esrAssignmentsMethods.ValidateGrid(this.CarGrid, carToAdd.CarGridValues);

            // click edit car
            esrAssignmentsMethods.ClickEditGridRow(this.CarGrid, carToAdd.make);

            #region verify general details
            Assert.AreEqual(carToAdd.make, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.MakeTextBox.Text);
            Assert.AreEqual(carToAdd.model, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ModelTextBox.Text);
            Assert.AreEqual(carToAdd.regNumber, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.RegistrationNumberTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((UnitOfMeasure)carToAdd.unitOfMeasure), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.UnitofMeasureComboBox.SelectedItem);
            Assert.AreEqual(EnumHelper.GetEnumDescription((EngineType)carToAdd.carType), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineTypeComboBox.SelectedItem);
            Assert.AreEqual(carToAdd.active, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.CarisactiveCheckBox.Checked);
            Assert.AreEqual(carToAdd.exemptFromHomeToOffice, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ExemptfromHometoLocaCheckBox.Checked);
            Assert.AreEqual(carToAdd.startDate.Value.ToShortDateString(), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.StartDateTextBox.Text);
            Assert.AreEqual(carToAdd.endDate.Value.ToShortDateString(), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EndDateTextBox.Text);
            #endregion


        }
        #endregion

        #region Successfully cancel edit car to employee
        /// <summary>
        /// successfully add car to employee
        /// </summary>
        [TestCategory("Cars"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void CarsSuccessfullyCancelEditEmployeeCar_UITest()
        {
            Employees employeeToEdit = employees[0];
            Cars carToEdit = employeeToEdit.employeeCars[0];
            Cars carToAdd = employeeToEdit.employeeCars[1];
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // Navigate to test employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // click cars link
            carsMethods.ClickCarsLink();

            // click edit car
            esrAssignmentsMethods.ClickEditGridRow(this.CarGrid, carToEdit.make);

            #region edit general details
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.MakeTextBox.Text = carToAdd.make;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ModelTextBox.Text = carToAdd.model;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.RegistrationNumberTextBox.Text = carToAdd.regNumber;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.UnitofMeasureComboBox.SelectedItem = EnumHelper.GetEnumDescription((UnitOfMeasure)carToAdd.unitOfMeasure);
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineTypeComboBox.SelectedItem = EnumHelper.GetEnumDescription((EngineType)carToAdd.carType);
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineSizeTextBox.Text = carToAdd.engineSize.ToString();
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.CarisactiveCheckBox.Checked = carToAdd.active;
            carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ExemptfromHometoLocaCheckBox.Checked = carToAdd.exemptFromHomeToOffice;
            if (carToAdd.startDate != null)
            {
                carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.StartDateTextBox.Text = carToAdd.startDate.Value.ToShortDateString().Replace("/", "0");
            }
            if (carToAdd.endDate != null)
            {
                carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EndDateTextBox.Text = carToAdd.endDate.Value.ToShortDateString().Replace("/", "0");
            }

            #endregion

            // press save
            carsMethods.PressCancelCarButton();

            // return to verify car saved
            esrAssignmentsMethods.ValidateGrid(this.CarGrid, carToEdit.CarGridValues);

            // click edit car
            esrAssignmentsMethods.ClickEditGridRow(this.CarGrid, carToEdit.make);

            #region verify general details
            Assert.AreEqual(carToEdit.make, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.MakeTextBox.Text);
            Assert.AreEqual(carToEdit.model, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ModelTextBox.Text);
            Assert.AreEqual(carToEdit.regNumber, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.RegistrationNumberTextBox.Text);
            Assert.AreEqual(EnumHelper.GetEnumDescription((UnitOfMeasure)carToEdit.unitOfMeasure), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.UnitofMeasureComboBox.SelectedItem);
            Assert.AreEqual(EnumHelper.GetEnumDescription((EngineType)carToEdit.carType), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EngineTypeComboBox.SelectedItem);
            Assert.AreEqual(carToEdit.active, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.CarisactiveCheckBox.Checked);
            Assert.AreEqual(carToEdit.exemptFromHomeToOffice, carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.ExemptfromHometoLocaCheckBox.Checked);
            Assert.AreEqual(carToEdit.startDate.Value.ToShortDateString(), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.StartDateTextBox.Text);
            Assert.AreEqual(carToEdit.endDate.Value.ToShortDateString(), carsMethods.CarGeneralControlsWindow.CarGeneralControlsDocument.EndDateTextBox.Text);
            #endregion

        }
        #endregion

        #region Successfully delete car to employee
        /// <summary>
        /// successfully delete car to employee
        /// </summary>
        [TestCategory("Cars"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void CarsSuccessfullyDeleteEmployeeCar_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            Employees employeeToEdit = employees[0];
            Cars carToDelete = employeeToEdit.employeeCars[0];

            // Navigate to test employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // Click cars link
            carsMethods.ClickCarsLink();

            // click delete car
            esrAssignmentsMethods.ClickDeleteGridRow(this.CarGrid, carToDelete.make);

            // Confirm delete
            employeesMethods.PressOkOnBrowserValidation();

            // validate grid
            esrAssignmentsMethods.ValidateGrid(this.CarGrid, carToDelete.CarGridValues, false);
        }
        #endregion

        #region Successfully cancel delete car to employee
        /// <summary>
        /// successfully cancel delete car to employee
        /// </summary>
        [TestCategory("Cars"), TestCategory("Employees"), TestCategory("Expenses"), TestMethod]
        public void CarsSuccessfullyCancelDeleteEmployeeCar_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);
            Employees employeeToEdit = employees[0];
            Cars carToDelete = employeeToEdit.employeeCars[0];

            // Navigate to test employee
            _sharedMethods.NavigateToPage(ExecutingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employeeToEdit.employeeID);

            // Click cars link
            carsMethods.ClickCarsLink();

            // click delete car
            carsMethods.ClickDeleteFieldLink(carToDelete.make);

            // cancel delete car
            employeesMethods.PressCancelOnBrowserValidation();

            // validate grid
            esrAssignmentsMethods.ValidateGrid(this.CarGrid, carToDelete.CarGridValues);
        }
        #endregion

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
                case "CarsSuccessfullyCancelEditEmployeeCar_UITest":
                case "CarsSuccessfullyDeleteEmployeeCar_UITest":
                case "CarsSuccessfullyCancelDeleteEmployeeCar_UITest":
                case "CarsSuccessfullyEditEmployeeCar_UITest":
                    carID = CarsRepository.createCar(employees[0].employeeCars[0], ExecutingProduct);
                    Assert.IsTrue(carID > 0);
                    break;
                default:
                    carID = CarsRepository.createCar(employees[0].employeeCars[0], ExecutingProduct);
                    Assert.IsTrue(carID > 0);

                    break;
            }
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
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

        public HtmlTable CarGrid
        {
            get
            {
                return carsMethods.CarGridWindow.CarGridDocument.CarGrid;
            }
        }
    }
}
