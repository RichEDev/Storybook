namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Windows.Input;

    using Auto_Tests.Coded_UI_Tests.GreenLight;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Imports_Exports.NHSTrusts;
    using Auto_Tests.Product_Variables.ModalMessages;
    using Auto_Tests.Product_Variables.PageUrlSurfices;
    using Auto_Tests.Tools;
    using Auto_Tests.UIMaps.EmployeesNewUIMapClasses;
    using Auto_Tests.UIMaps.ESRAssignmentsUIMapClasses;
    using Auto_Tests.UIMaps.SharedMethodsUIMapClasses;

    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using System.Text;

    /// <summary>
    /// Summary description for EmployeesUITests
    /// </summary>
    [CodedUITest]
    public class EmployeesUITests
    {
        /// <summary>
        /// Current Product in test run
        /// </summary>
        private static readonly  ProductType _executingProduct = cGlobalVariables.GetProductFromAppConfig();

        /// <summary>
        /// Shared methods UI Map
        /// </summary>
        private static SharedMethodsUIMap _sharedMethods = new SharedMethodsUIMap();

        /// <summary>
        /// employees methods UI Map
        /// </summary>
        private static EmployeesNewUIMap employeesMethods;

        /// <summary>
        /// esrassignments methods UI Map
        /// </summary>
        private static ESRAssignmentsUIMap esrAssignmentsMethods;

        /// <summary>
        /// Cached list of Employees
        /// </summary>
        private static List<Employees> employees;

        /// <summary>
        /// Administrator employee number
        /// </summary>
        private static int adminId;


        /// <summary>
        /// The nhs trusts.
        /// </summary>
        private static List<NHSTrusts> nhsTrusts;
            
        [ClassInitialize()]
        public static void ClassInit(TestContext ctx)
        {
            Playback.Initialize();
            BrowserWindow browser = BrowserWindow.Launch();
            browser.Maximized = true;
            browser.CloseOnPlaybackCleanup = false;
            _sharedMethods = new SharedMethodsUIMap();
            _sharedMethods.Logon(_executingProduct, LogonType.administrator);
            adminId = AutoTools.GetEmployeeIDByUsername(_executingProduct);
            employees = EmployeesRepository.PopulateEmployee();
            nhsTrusts = NHSTrustsRepository.PopulateNhsTrust();
        }

        [ClassCleanup]
        public static void ClassCleanUp()
        {
            _sharedMethods.CloseBrowserWindow();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            Guid genericName;
            foreach (var employee in employees)
            {
                genericName = Guid.NewGuid();
                employee.UserName = string.Format(
                    "{0}{1}", new object[] { "Custom Emp ", genericName });
            }

            foreach (var nhsTrust in nhsTrusts)
            {
                genericName = Guid.NewGuid();
                Random genericVpd = new Random();
                nhsTrust.TrustName = string.Format(
                    "{0}{1}", new object[] { "Custom Trust ", genericName });
                nhsTrust.TrustVPD = genericVpd.Next(100, 999).ToString();
            }

            employeesMethods = new EmployeesNewUIMap();
            esrAssignmentsMethods = new ESRAssignmentsUIMap();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (Employees employee in employees)
            {
                if (employee.employeeID > 0)
                {
                    EmployeesRepository.DeleteEmployee(employee.employeeID, _executingProduct);
                }
            }

            foreach (var nhsTrust in nhsTrusts)
            {
                if (nhsTrust.TrustID > 0)
                {
                    NHSTrustsRepository.DeleteNhsTrusts(nhsTrust.TrustID, _executingProduct, adminId);
                }
            }
        }

        #region General Details
        #region Successfully add employee
        /// <summary>
        /// successfully add new employee
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesSuccessfullyAddEmployee_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            #region Navigate to Employees Page
            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);

            // Click add new Employees link
            employeesMethods.ClickNewEmployeeLink();
            #endregion

            #region General Details
            // Populate Employee General Details
            Employees employeeToAdd = employees[0];
            Employees lineManager = employees[1];
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.UsernameTextBox.Text = employeeToAdd.UserName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.TitleTextBox.Text = employeeToAdd.Title;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.FirstNameTextBox.Text = employeeToAdd.FirstName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MiddleNameTextBox.Text = employeeToAdd.middleName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MaidenNameTextBox.Text = employeeToAdd.maidenName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SurnameTextBox.Text = employeeToAdd.Surname;
            employeesMethods.EmployeeAdditionalConrtolsWindow.EmployeeAdditonalControlsDocument.PreferredNameTextBox.Text = employeeToAdd.preferredName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.EmailAddressTextBox.Text = employeeToAdd.emailAddress;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.ExtensionNumberTextBox.Text = employeeToAdd.extensionNumber;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MobileNumberTextBox.Text = employeeToAdd.mobileNumber;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.PagerNumberTextBox.Text = employeeToAdd.PagerNumber;

            // not displayed on edit
            // _employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SendPasswordEmailCheckBox.Checked = true;
            // _employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SendWelcomeEmailCheckBox.Checked = true;
            #endregion

            #region Work Details
            employeesMethods.ClickWorkTab();

            // Populate Employee Work Details
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.CreditAccountTextBox.Text = employeeToAdd.creditAccount;
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.PayrollNumberTextBox.Text = employeeToAdd.payrollNumber;
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.PositionTextBox.Text = employeeToAdd.position;
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.NationalInsuranceNumTextBox.Text = employeeToAdd.nationalInsuranceNumner;
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.HireDateTextBox.Text = employeeToAdd.hireDate.Value.ToShortDateString().ToString().Replace("/","0");
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.TerminationDateTextBox.Text = employeeToAdd.terminationDate.Value.ToShortDateString().ToString().Replace("/", "0");
            employeesMethods.EmployeeAdditionalConrtolsWindow.EmployeeAdditonalControlsDocument.EmployeeNumberTextBox.Text = employeeToAdd.employeeNumner;
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.PrimaryCountryComboBox.SelectedItem = "United Kingdom";
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.PrimaryCurrencyComboBox.SelectedItem = "Pound Sterling";

            if (_executingProduct == ProductType.expenses)
            {
                string lineManagerFullname = string.Format("{0} {1} ({2})", new object[]{lineManager.FirstName, lineManager.Surname, lineManager.UserName});
                employeesMethods.EmployeeAdditionalWorkControlsWindows.EmployeeAdditionalWorkControlsDocument.LineManagerTextBox.Text = lineManagerFullname;
                employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.StartingMileageTextBox.Text = employeeToAdd.startingMileage.ToString();
                employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.StartingMileageDateTextBox.Text = employeeToAdd.startingMileageDate.Value.ToShortDateString().ToString().Replace("/", "0");
                employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.TrustComboBox.SelectedItem = nhsTrusts[0].TrustName;
                employeesMethods.EmployeeAdditionalConrtolsWindow.EmployeeAdditonalControlsDocument.NHSUniqueIDTextBox.Text = employeeToAdd.nhsUniqueID;
            }
            #endregion

            if (_executingProduct == ProductType.expenses)
            {
                #region Personal Details
                employeesMethods.ClickPersonalTab();

                // Populate Employee General Details
                string lastCheckedBy = string.Format("{0}, {1} [{2}]", new object[] { lineManager.Surname, lineManager.FirstName, lineManager.UserName });
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.EmailAddressTextBox.Text = employeeToAdd.pEmailAddress;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.TelephoneNumberTextBox.Text = employeeToAdd.telephoneNumber;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.FaxNumberTextBox.Text = employeeToAdd.faxNumber;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.LicenceNumberTextBox.Text = employeeToAdd.licenceNumber;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.LicenceExpiryDateTextBox.Text = employeeToAdd.licenceExpiryDate.Value.ToShortDateString().ToString().Replace("/", "0");
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.LastCheckedTextBox.Text = employeeToAdd.lastChecked.Value.ToShortDateString().ToString().Replace("/", "0");
                Clipboard.SetText(lastCheckedBy);
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.CheckedByTextBox.SetFocus();
                Keyboard.SendKeys("v", ModifierKeys.Control);
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.AccountHolderNameTextBox.Text = employeeToAdd.accountHolderName;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.AccountNumberTextBox.Text = employeeToAdd.accountNumber;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.AccountTypeTextBox.Text = employeeToAdd.accountType;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.SortCodeTextBox.Text = employeeToAdd.sortCode;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.AccountReferenceTextBox.Text = employeeToAdd.accountReference;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.GenderComboBox.SelectedItem = employeeToAdd.gender;
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.DateofBirthTextBox.Text = employeeToAdd.dateOfBirth.Value.ToShortDateString().ToString().Replace("/", "0");
                #endregion

                #region Claims Details
                employeesMethods.ClickClaimsTab();

                // Populate Employee General Details
                employeesMethods.EmployeeClaimsControlsWindow.EmployeeClaimsControlsDocument.SignoffGroupComboBox.SelectedItem = "Line Manager";
                employeesMethods.EmployeeClaimsControlsWindow.EmployeeClaimsControlsDocument.SignoffGroupCreditCaComboBox.SelectedItem = "Line Manager";
                employeesMethods.EmployeeClaimsControlsWindow.EmployeeClaimsControlsDocument.SignoffGroupPurchaseComboBox.SelectedItem = "Line Manager";
                employeesMethods.EmployeeClaimsControlsWindow.EmployeeClaimsControlsDocument.SignoffGroupAdvancesComboBox.SelectedItem = "Line Manager";
                #endregion

                #region Notification Details
                employeesMethods.ClickNotificationsTab();
                ///Populate Employee Notification Details
                employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.StandardCheckBox.Checked = true;
                employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.ESROutboundImportSumCheckBox.Checked = true;
                employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.ESROutboundImportStaCheckBox.Checked = true;
                employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.ESROutboundInvalidPoCheckBox.Checked = true;
                employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.ESROutboundLinemanagCheckBox.Checked = true;
                #endregion
            }

            // press save employee
            Mouse.Click(employeesMethods.EmployeeTabsAndButtonsWindow.EmployeeButtonsDocument.EmployeeButtonsPane.EmployeeSaveButton);

            // search for employee
            employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employeeToAdd.UserName;
            Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

            // return employee id
            employeeToAdd.employeeID = employeesMethods.ReturnEmployeeIDFromGrid(employeeToAdd.UserName);

            if (_executingProduct == ProductType.expenses)
            {
                // validate employees grid
                employeesMethods.ValidateEmployeesGrid(employeeToAdd.UserName, employeeToAdd.Title, employeeToAdd.FirstName, employeeToAdd.Surname, "Line Manager");
            }
            else
            {
                // validate employees grid
                employeesMethods.ValidateEmployeesGrid(employeeToAdd.UserName, employeeToAdd.Title, employeeToAdd.FirstName, employeeToAdd.Surname);
            }

            // click edit against employee
            employeesMethods.ClickEditFieldLink(employeeToAdd.UserName);

            // Verify Employee Data saved
            #region verify general details
            Assert.AreEqual(employeeToAdd.UserName, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.UsernameTextBox.Text);
            Assert.AreEqual(employeeToAdd.Title,employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.TitleTextBox.Text);
            Assert.AreEqual(employeeToAdd.FirstName, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.FirstNameTextBox.Text);
            Assert.AreEqual(employeeToAdd.middleName, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MiddleNameTextBox.Text);
            Assert.AreEqual(employeeToAdd.maidenName, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MaidenNameTextBox.Text);
            Assert.AreEqual(employeeToAdd.Surname, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SurnameTextBox.Text);
            Assert.AreEqual(employeeToAdd.preferredName, employeesMethods.EmployeeAdditionalConrtolsWindow.EmployeeAdditonalControlsDocument.PreferredNameTextBox.Text);
            Assert.AreEqual(employeeToAdd.emailAddress, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.EmailAddressTextBox.Text);
            Assert.AreEqual(employeeToAdd.extensionNumber, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.ExtensionNumberTextBox.Text);
            Assert.AreEqual(employeeToAdd.mobileNumber, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MobileNumberTextBox.Text);
            Assert.AreEqual(employeeToAdd.PagerNumber, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.PagerNumberTextBox.Text);

            // not displayed on edit
            // Assert.AreEqual(true, _employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SendPasswordEmailCheckBox.Checked);
            // Assert.AreEqual(true, _employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SendWelcomeEmailCheckBox.Checked);
            #endregion

            #region verify Work Details
            employeesMethods.ClickWorkTab();

            // Populate Employee Work Details
            Assert.AreEqual(employeeToAdd.creditAccount, employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.CreditAccountTextBox.Text);
            Assert.AreEqual(employeeToAdd.payrollNumber, employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.PayrollNumberTextBox.Text);
            Assert.AreEqual(employeeToAdd.position, employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.PositionTextBox.Text);
            Assert.AreEqual(employeeToAdd.nationalInsuranceNumner, employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.NationalInsuranceNumTextBox.Text);
            Assert.AreEqual(employeeToAdd.hireDate.Value.ToShortDateString(), employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.HireDateTextBox.Text);
            Assert.AreEqual(employeeToAdd.terminationDate.Value.ToShortDateString(), employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.TerminationDateTextBox.Text);
            Assert.AreEqual(employeeToAdd.employeeNumner, employeesMethods.EmployeeAdditionalConrtolsWindow.EmployeeAdditonalControlsDocument.EmployeeNumberTextBox.Text);
            Assert.AreEqual("United Kingdom", employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.PrimaryCountryComboBox.SelectedItem);
            Assert.AreEqual("Pound Sterling", employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.PrimaryCurrencyComboBox.SelectedItem);

            if (_executingProduct == ProductType.expenses)
            {
                string lineManagerFullname = string.Format("{0} {1} ({2})", new object[] { lineManager.FirstName, lineManager.Surname, lineManager.UserName });
                Assert.AreEqual(lineManagerFullname, employeesMethods.EmployeeAdditionalWorkControlsWindows.EmployeeAdditionalWorkControlsDocument.LineManagerTextBox.Text);
                Assert.AreEqual(employeeToAdd.startingMileage.ToString(), employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.StartingMileageTextBox.Text);
                Assert.AreEqual(employeeToAdd.startingMileageDate.Value.ToShortDateString(), employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.StartingMileageDateTextBox.Text);
                Assert.AreEqual(nhsTrusts[0].TrustName, employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.TrustComboBox.SelectedItem);
                Assert.AreEqual(employeeToAdd.nhsUniqueID, employeesMethods.EmployeeAdditionalConrtolsWindow.EmployeeAdditonalControlsDocument.NHSUniqueIDTextBox.Text);

            }

            #endregion

            if (_executingProduct == ProductType.expenses)
            {
                #region verify Personal Details
                employeesMethods.ClickPersonalTab();

                // Populate Employee General Details
                string lastCheckedBy = string.Format("{0}, {1} [{2}]", new object[] { lineManager.Surname, lineManager.FirstName, lineManager.UserName });
                Assert.AreEqual(employeeToAdd.pEmailAddress, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.EmailAddressTextBox.Text);
                Assert.AreEqual(employeeToAdd.telephoneNumber, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.TelephoneNumberTextBox.Text);
                Assert.AreEqual(employeeToAdd.faxNumber, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.FaxNumberTextBox.Text);
                Assert.AreEqual(employeeToAdd.licenceNumber,  employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.LicenceNumberTextBox.Text);
                Assert.AreEqual(employeeToAdd.licenceExpiryDate.Value.ToShortDateString(), employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.LicenceExpiryDateTextBox.Text);
                Assert.AreEqual(employeeToAdd.lastChecked.Value.ToShortDateString(), employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.LastCheckedTextBox.Text);
                Assert.AreEqual(lastCheckedBy, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.CheckedByTextBox.Text);
                Assert.AreEqual(employeeToAdd.accountHolderName, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.AccountHolderNameTextBox.Text);
                Assert.AreEqual(employeeToAdd.accountNumber, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.AccountNumberTextBox.Text);
                Assert.AreEqual(employeeToAdd.accountType, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.AccountTypeTextBox.Text);
                Assert.AreEqual(employeeToAdd.sortCode, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.SortCodeTextBox.Text);
                Assert.AreEqual(employeeToAdd.accountReference, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.AccountReferenceTextBox.Text);
                Assert.AreEqual(employeeToAdd.gender, employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.GenderComboBox.SelectedItem);
                Assert.AreEqual(employeeToAdd.dateOfBirth.Value.ToShortDateString(), employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.DateofBirthTextBox.Text);
                #endregion

                #region verify Claims Details
                employeesMethods.ClickClaimsTab();

                // Populate Employee General Details
                Assert.AreEqual("Line Manager", employeesMethods.EmployeeClaimsControlsWindow.EmployeeClaimsControlsDocument.SignoffGroupComboBox.SelectedItem);
                Assert.AreEqual("Line Manager", employeesMethods.EmployeeClaimsControlsWindow.EmployeeClaimsControlsDocument.SignoffGroupCreditCaComboBox.SelectedItem);
                Assert.AreEqual("Line Manager", employeesMethods.EmployeeClaimsControlsWindow.EmployeeClaimsControlsDocument.SignoffGroupPurchaseComboBox.SelectedItem);
                Assert.AreEqual("Line Manager", employeesMethods.EmployeeClaimsControlsWindow.EmployeeClaimsControlsDocument.SignoffGroupAdvancesComboBox.SelectedItem);
                #endregion

                #region verify Notification Details
                employeesMethods.ClickNotificationsTab();
                ///Populate Employee Notification Details
                Assert.AreEqual(true, employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.StandardCheckBox.Checked);
                Assert.AreEqual(true, employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.ESROutboundImportSumCheckBox.Checked);
                Assert.AreEqual(true, employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.ESROutboundImportStaCheckBox.Checked);
                Assert.AreEqual(true, employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.ESROutboundInvalidPoCheckBox.Checked);
                Assert.AreEqual(true, employeesMethods.EmployeeNotificationControlsWindow.EmployeeNotificationControlsDocument.ESROutboundLinemanagCheckBox.Checked);
                #endregion
            }
        }
        #endregion

        #region unsuccessfully add employee using cancel
        /// <summary>
        /// unsuccessfully edit new employee using cancel
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesUnsuccessfullyAddEmployeeWithCancelButton_UITest()
        {

            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);

            // Click add new Employees link
            employeesMethods.ClickNewEmployeeLink();

            #region add General Details
            ///Populate Employee General Details
            Employees employeeToAdd = employees[0];
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.UsernameTextBox.Text = employeeToAdd.UserName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.TitleTextBox.Text = employeeToAdd.Title;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.FirstNameTextBox.Text = employeeToAdd.FirstName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MiddleNameTextBox.Text = employeeToAdd.middleName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SurnameTextBox.Text = employeeToAdd.Surname;
            employeesMethods.EmployeeAdditionalConrtolsWindow.EmployeeAdditonalControlsDocument.PreferredNameTextBox.Text = employeeToAdd.preferredName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.EmailAddressTextBox.Text = employeeToAdd.emailAddress;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.ExtensionNumberTextBox.Text = employeeToAdd.extensionNumber;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MobileNumberTextBox.Text = employeeToAdd.mobileNumber;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.PagerNumberTextBox.Text = employeeToAdd.PagerNumber;
            #endregion

            //press cancel employee
            employeesMethods.PressCancelSaveEmployeeButton();

            // search for employee
            employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employeeToAdd.UserName;
            Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

            // validate employees grid
            employeesMethods.ValidateEmployeesGrid(employeeToAdd.UserName, employeeToAdd.Title, employeeToAdd.FirstName, employeeToAdd.Surname, " ", false);
        }
        #endregion

        #region unsuccessfully add employee without mandatory fields
        /// <summary>
        /// unsuccessfully add new employee without populating mandatory fields
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesUnsuccessfullyAddEmployeeWithoutMandatoryFields_UITest()
        {
            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);

            // Click add new Employees link
            employeesMethods.ClickNewEmployeeLink();

            // press save button
            employeesMethods.PressSaveEmployeeButton();

            // Validate warning messages
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}{2}{3}{4}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), EmployeeModalMessages.EmptyFieldForEmployeeUsername, EmployeeModalMessages.EmptyFieldForEmployeeTitle, EmployeeModalMessages.EmptyFieldForEmployeeFirstName, EmployeeModalMessages.EmptyFieldForEmployeeSurname});
            // employeesMethods.ValidateEmployeeModalMessage();
            Assert.AreEqual(employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText, employeesMethods.ModalMessage);

        }
        #endregion

        #region unsuccessfully add employee with invalid fields
        /// <summary>
        /// unsuccessfully add new employee with invalid fields
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesUnsuccessfullyAddEmployeeWithInvalidFields_UITest()
        {
            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);

            // Click add new Employees link
            employeesMethods.ClickNewEmployeeLink();

            #region Populate Employee mandatory Details 

            // Populate Employee mandatory Details
            Employees employeeToAdd = employees[0];
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.UsernameTextBox.Text = employeeToAdd.UserName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.TitleTextBox.Text = employeeToAdd.Title;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.FirstNameTextBox.Text = employeeToAdd.FirstName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SurnameTextBox.Text = employeeToAdd.Surname;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.EmailAddressTextBox.Text = employeeToAdd.extensionNumber;
            #endregion

            #region populate invalid Work Details
            employeesMethods.ClickWorkTab();

            // Populate invalid Employee Work Details
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.HireDateTextBox.Text = employeeToAdd.terminationDate.Value.ToShortDateString().ToString().Replace("/","0");
            employeesMethods.EmployeeWorkControlsWindow.EmployeeWorkControlsDocument.TerminationDateTextBox.Text = employeeToAdd.hireDate.Value.ToShortDateString().ToString().Replace("/", "0");

            if (_executingProduct == ProductType.expenses)
            {
                employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.StartingMileageTextBox.Text = employeeToAdd.gender;
            }

            #endregion

            if (_executingProduct == ProductType.expenses)
            {
                #region Populate invalid Personal Details
                employeesMethods.ClickPersonalTab();

                // Populate Employee General Details
                employeesMethods.EmployeePersonalControlsWindow.EmployeePersonalControlsDocument.EmailAddressTextBox.Text = employeeToAdd.telephoneNumber;
                #endregion

            }

            // press save button
            employeesMethods.PressSaveEmployeeButton();

            // Validate warning messages
            if (_executingProduct == ProductType.expenses)
            {
                employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}{2}{3}{4}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), EmployeeModalMessages.InvalideFieldForEmployeeEmailAddress, EmployeeModalMessages.InvalideFieldForEmployeeLeaveDateEarlierThanHireDate, EmployeeModalMessages.InvalideFieldForEmployeeStartingMileage, EmployeeModalMessages.InvalideFieldForEmployeeHomeEmailAddress });
            }
            else
            {
                employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}{2}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), EmployeeModalMessages.InvalideFieldForEmployeeEmailAddress, EmployeeModalMessages.InvalideFieldForEmployeeLeaveDateEarlierThanHireDate });
            }

            // employeesMethods.ValidateEmployeeModalMessage();
            Assert.AreEqual(employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText, employeesMethods.ModalMessage);

        }
        #endregion

        #region unsuccessfully add duplicate employee 
        /// <summary>
        /// successfully add new employee
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesUnsuccessfullyAddDuplicateEmployee_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);

            // Click add new Employees link
            employeesMethods.ClickNewEmployeeLink();

            // Add employee 
            #region General Details
            // Populate Employee General Details
            Employees employeeToAdd = employees[0];
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.UsernameTextBox.Text = employeeToAdd.UserName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.TitleTextBox.Text = employeeToAdd.Title;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.FirstNameTextBox.Text = employeeToAdd.FirstName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SurnameTextBox.Text = employeeToAdd.Surname;
            employeesMethods.PressSaveEmployeeButton();
            #endregion

            // verify message displayed
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), EmployeeModalMessages.DuplicateFieldForEmployeeUsername });
            // employeesMethods.ValidateEmployeeModalMessage();
            Assert.AreEqual(employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText, employeesMethods.ModalMessage);

        }
        #endregion

        #region successfully edit employee 
        /// <summary>
        /// successfully edit new employee
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesSuccessfullyEditEmployee_UITest()
        {

            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employees[0].employeeID);

            #region Edit General Details
            // Populate Employee General Details
            Employees employeeToEdit = employees[1];
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.UsernameTextBox.Text = employeeToEdit.UserName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.TitleTextBox.Text = employeeToEdit.Title;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.FirstNameTextBox.Text = employeeToEdit.FirstName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MiddleNameTextBox.Text = employeeToEdit.middleName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MaidenNameTextBox.Text = employeeToEdit.maidenName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SurnameTextBox.Text = employeeToEdit.Surname;
            employeesMethods.EmployeeAdditionalConrtolsWindow.EmployeeAdditonalControlsDocument.PreferredNameTextBox.Text = employeeToEdit.preferredName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.EmailAddressTextBox.Text = employeeToEdit.emailAddress;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.ExtensionNumberTextBox.Text = employeeToEdit.extensionNumber;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MobileNumberTextBox.Text = employeeToEdit.mobileNumber;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.PagerNumberTextBox.Text = employeeToEdit.PagerNumber;
            #endregion

            // press save employee
            employeesMethods.PressSaveEmployeeButton();

            // search for employee
            employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employeeToEdit.UserName;
            Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

            if (_executingProduct == ProductType.expenses)
            {
                // validate employees grid
                employeesMethods.ValidateEmployeesGrid(employeeToEdit.UserName, employeeToEdit.Title, employeeToEdit.FirstName, employeeToEdit.Surname, " ");
            }
            else
            {
                // validate employees grid
                employeesMethods.ValidateEmployeesGrid(employeeToEdit.UserName, employeeToEdit.Title, employeeToEdit.FirstName, employeeToEdit.Surname);
            }

            // click edit against employee
            employeesMethods.ClickEditFieldLink(employeeToEdit.UserName);

            #region verify edited general details
            Assert.AreEqual(employeeToEdit.UserName, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.UsernameTextBox.Text);
            Assert.AreEqual(employeeToEdit.Title, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.TitleTextBox.Text);
            Assert.AreEqual(employeeToEdit.FirstName, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.FirstNameTextBox.Text);
            Assert.AreEqual(employeeToEdit.middleName, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MiddleNameTextBox.Text);
            Assert.AreEqual(employeeToEdit.maidenName, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MaidenNameTextBox.Text);
            Assert.AreEqual(employeeToEdit.Surname, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SurnameTextBox.Text);
            Assert.AreEqual(employeeToEdit.preferredName, employeesMethods.EmployeeAdditionalConrtolsWindow.EmployeeAdditonalControlsDocument.PreferredNameTextBox.Text);
            Assert.AreEqual(employeeToEdit.emailAddress, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.EmailAddressTextBox.Text);
            Assert.AreEqual(employeeToEdit.extensionNumber, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.ExtensionNumberTextBox.Text);
            Assert.AreEqual(employeeToEdit.mobileNumber, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MobileNumberTextBox.Text);
            Assert.AreEqual(employeeToEdit.PagerNumber, employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.PagerNumberTextBox.Text);
            #endregion
        }
        #endregion

        #region unsuccessfully edit employee using cancel
        /// <summary>
        /// unsuccessfully edit new employee using cancel
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesUnsuccessfullyEditEmployeeWithCancelButton_UITest()
        {

            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employees[0].employeeID);

            #region Edit General Details
            // Populate Employee General Details
            Employees employeeToEdit = employees[1];
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.UsernameTextBox.Text = employeeToEdit.UserName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.TitleTextBox.Text = employeeToEdit.Title;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.FirstNameTextBox.Text = employeeToEdit.FirstName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MiddleNameTextBox.Text = employeeToEdit.middleName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SurnameTextBox.Text = employeeToEdit.Surname;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.EmailAddressTextBox.Text = employeeToEdit.emailAddress;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.ExtensionNumberTextBox.Text = employeeToEdit.extensionNumber;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.MobileNumberTextBox.Text = employeeToEdit.mobileNumber;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.PagerNumberTextBox.Text = employeeToEdit.PagerNumber;
            #endregion

            // press cancel employee
            employeesMethods.PressCancelSaveEmployeeButton();

            // search for employee
            employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employeeToEdit.UserName;
            Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

            // validate employees grid
            employeesMethods.ValidateEmployeesGrid(employeeToEdit.UserName, employeeToEdit.Title, employeeToEdit.FirstName, employeeToEdit.Surname, " ", false);
        }
        #endregion

        #region unsuccessfully edit duplicate employee
        /// <summary>
        /// successfully add new employee
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesUnsuccessfullyEditDuplicateEmployee_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.EditEmployeeUrl + employees[0].employeeID);

            // Add employee 
            #region General Details
            // Populate Employee General Details
            Employees employeeToEdit = employees[1];
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.UsernameTextBox.Text = employeeToEdit.UserName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.TitleTextBox.Text = employeeToEdit.Title;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.FirstNameTextBox.Text = employeeToEdit.FirstName;
            employeesMethods.EmployeeGeneralControlsWindow.EmployeeGeneralControlsDocument.SurnameTextBox.Text = employeeToEdit.Surname;
            employeesMethods.PressSaveEmployeeButton();
            #endregion

            // verify message displayed
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), EmployeeModalMessages.DuplicateFieldForEmployeeUsername });
            // employeesMethods.ValidateEmployeeModalMessage();
            Assert.AreEqual(employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText, employeesMethods.ModalMessage);

        }
        #endregion

        #region unsuccessfully delete emlployee when not archived
        /// <summary>
        /// Unsuccessfully delete emlployee when not archived
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesUnsuccessfullyDeleteEmployeeWhenNotArchived_UITest()
        {

            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);

            // search for employee
            employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employees[0].UserName;
            Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

            // click delete against employee
            employeesMethods.ClickDeleteFieldLink(employees[0].UserName);

            // Confirm deletion
            employeesMethods.PressOkOnBrowserValidation();
            
            // Validate message displayed
            employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText = string.Format("Message from {0}{1}", new object[] { EnumHelper.GetEnumDescription(_executingProduct), EmployeeModalMessages.DeleteEmployeeWhenUnarchived });
            // employeesMethods.ValidateEmployeeModalMessage();
            Assert.AreEqual(employeesMethods.ValidateEmployeeModalMessageExpectedValues.MasterPopupPaneInnerText, employeesMethods.ModalMessage);

        }
        #endregion

        #region successfully delete emlployee
        /// <summary>
        /// Unsuccessfully delete emlployee when not archived
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesSuccessfullyDeleteEmployee_UITest()
        {

            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            var employeeToDelete = employees[0];

            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);

            // search for employee
            employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employeeToDelete.UserName;
            Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

            // click archive against employee
            employeesMethods.ClickArchiveFieldLink(employeeToDelete.UserName);

            // click delete against employee
            employeesMethods.ClickDeleteFieldLink(employeeToDelete.UserName);

            // Confirm delete
            employeesMethods.PressOkOnBrowserValidation();

            // verify employee deleted
            employeesMethods.ValidateEmployeesGrid(employeeToDelete.UserName, employeeToDelete.Title, employeeToDelete.FirstName, employeeToDelete.Surname, " ", false);
        }
        #endregion

        #region Successfully Activate employee
        /// <summary>
        /// Successfully Activate an employee
        /// </summary>
        [TestCategory("Employee Details"), TestCategory("Employees"), TestCategory("Spend Management"), TestMethod]
        public void EmployeesSuccessfullyActivateEmployee_UITest()
        {
            this.ImportDataToTestingDatabase(this.testContextInstance.TestName);

            var employeeToActivate = employees[7];

            // Navigate to the select employee page
            _sharedMethods.NavigateToPage(_executingProduct, EmployeeUrlSuffixes.SearchEmployeeUrl);

            // search for employee
            employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employeeToActivate.UserName;
            Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

            // Check employee is Inactive
            employeesMethods.CheckInactiveFieldExists(employeeToActivate.UserName, true);

            // Click Inactive employee
            employeesMethods.ClickInactiveFieldLink(employeeToActivate.UserName);

            // Activate employee
            Mouse.Click(employeesMethods.EmployeeAdditionalButtonWindow.EmployeeEditDocument.EmployeeActivateButton);

            // Search for employee
            employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.UsernameTextBox.Text = employeeToActivate.UserName;
            Mouse.Click(employeesMethods.EmployeeSearchControlsWindow.EmployeeSearchControlsDocument.SearchButton);

            // Verify employee is Active
            employeesMethods.CheckInactiveFieldExists(employeeToActivate.UserName, false);

        }
        #endregion
        #endregion

        /// <summary>
        /// The import data to testing database.
        /// </summary>
        /// <param name="testName">
        /// The test name.
        /// </param>
        private void ImportDataToTestingDatabase(string testName)
        {
            int employeeid;
            switch (testName)
            {
                case "EmployeesSuccessfullyEditEmployee_UITest":
                case "EmployeesUnsuccessfullyDeleteEmployeeWhenNotArchived_UITest":
                case "EmployeesUnsuccessfullyAddDuplicateEmployee_UITest":
                case "EmployeesSuccessfullyDeleteEmployee_UITest":
                case "EmployeesUnsuccessfullyEditEmployeeWithCancelButton_UITest":
                    employeeid = EmployeesRepository.CreateEmployee(employees[0], _executingProduct);
                    Assert.IsTrue(employeeid > 0); 
                    break;
                case "EmployeesUnsuccessfullyEditDuplicateEmployee_UITest":
                    employeeid = EmployeesRepository.CreateEmployee(employees[0], _executingProduct);
                    Assert.IsTrue(employeeid > 0);
                    employeeid = EmployeesRepository.CreateEmployee(employees[1], _executingProduct);
                    Assert.IsTrue(employeeid > 0); 
                    break;
                case "EmployeesSuccessfullyAddEmployee_UITest":
                    employeeid = EmployeesRepository.CreateEmployee(employees[1], _executingProduct);
                    Assert.IsTrue(employeeid > 0); 
                    int nhsTrustsId = 0;
                    nhsTrustsId = NHSTrustsRepository.CreateNhsTrusts(nhsTrusts[0], _executingProduct, employeeid);
                    Assert.IsTrue(nhsTrustsId > 0);
                    break;
                case "EmployeesSuccessfullyActivateEmployee_UITest":
                    employees[7].active = false;
                    employeeid = EmployeesRepository.CreateEmployee(employees[7], _executingProduct);
                    Assert.IsTrue(employeeid > 0); 
                    break;
                default:
                    break;

            }
        }

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
