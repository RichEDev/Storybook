namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Employees;

    using Spend_Management;
    using SpendManagementLibrary;

    /// <summary>
    /// ESR import tests.
    /// </summary>
    [TestClass()]
    public class cESRImportTests
    {
        /// <summary>
        /// The username.
        /// </summary>
        private static string username;

        /// <summary>
        /// The password.
        /// </summary>
        private static string password;

        /// <summary>
        /// The employees.
        /// </summary>
        private static cEmployees employees;

        /// <summary>
        /// The global employee.
        /// </summary>
        private static Employee globalEmployee;

        /// <summary>
        /// The default plain text password.
        /// </summary>
        private static string defaultPlainTextPassword;

        /// <summary>
        /// The account id.
        /// </summary>
        private static int accountId;

        /// <summary>
        /// The employee id.
        /// </summary>
        private static int employeeId;

        /// <summary>
        /// The delegate id.
        /// </summary>
        private static int delegateId;

        /// <summary>
        /// The sub account id.
        /// </summary>
        private static int subAccountId;

        /// <summary>
        /// The active module.
        /// </summary>
        private static int activeModule;

        /// <summary>
        /// The test context instance.
        /// </summary>
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        #region Additional test attributes

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// The my class clean up.
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        /// <summary>
        /// The my test initialize.
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            username = string.Empty;
            password = string.Empty;
            employees = new cEmployees(GlobalTestVariables.AccountId);
            globalEmployee = null;
            defaultPlainTextPassword = "password";
            accountId = GlobalTestVariables.AccountId;
            employeeId = GlobalTestVariables.EmployeeId;
            delegateId = GlobalTestVariables.GetWebConfigIntValue("DelegateID");
            subAccountId = GlobalTestVariables.SubAccountId;
            activeModule = GlobalTestVariables.GetWebConfigIntValue("active_module");
        }

        /// <summary>
        /// The my test clean up.
        /// </summary>
        [TestCleanup]
        public void MyTestCleanup()
        {
            if (globalEmployee != null)
            {
                globalEmployee.Archived = true;
                globalEmployee.Save(Moqs.CurrentUser());
                globalEmployee.Delete(Moqs.CurrentUser());
            }

            var clsEmps = new cEmployees(GlobalTestVariables.AccountId);

            cGlobalPropertiesObject.UpdateUsernameFormat(string.Empty);
            cGlobalPropertiesObject.UpdateHomeAddressFormat(string.Empty);

            // Remove any employee and associated details added
            Employee emp = clsEmps.GetEmployeeById(cGlobalVariables.OutboundEmployeID);

            if (emp != null)
            {
                if (emp.EmployeeID > 0)
                {
                    var accessRoles = emp.GetAccessRoles();
                    if (accessRoles.Count > 0)
                    {
                        var roleid = accessRoles.AllAccessRoles.FirstOrDefault().Key;
                        accessRoles.Remove(roleid, GlobalTestVariables.SubAccountId, Moqs.CurrentUser());
                    }

                    emp.Archived = true;
                    var workLocations = emp.GetWorkAddresses();
                    var homeLocations = emp.GetHomeAddresses();
                    emp.Save(Moqs.CurrentUser());
                    emp.Delete(Moqs.CurrentUser());

                    foreach (cEmployeeHomeLocation employeeHomeAddress in homeLocations)
                    {
                        Address.Delete(Moqs.CurrentUser(), employeeHomeAddress.LocationID);
                    }

                    foreach (KeyValuePair<int, cEmployeeWorkLocation> employeeWorkAddress in workLocations)
                    {
                        Address.Delete(Moqs.CurrentUser(), employeeWorkAddress.Value.LocationID);
                    }
                }
            }

            // Remove any line manager
            Employee lineManEmp = clsEmps.GetEmployeeById(cGlobalVariables.OutboundLinemanagerEmployeeID);

            if (lineManEmp != null)
            {
                if (lineManEmp.EmployeeID > 0)
                {
                    var accessRoles = lineManEmp.GetAccessRoles();
                    if (accessRoles.Count > 0)
                    {
                        var roleid = accessRoles.AllAccessRoles.FirstOrDefault().Key;
                        accessRoles.Remove(roleid, GlobalTestVariables.SubAccountId, Moqs.CurrentUser());
                    }

                    lineManEmp.Archived = true;
                    lineManEmp.Save(Moqs.CurrentUser());
                    var homeAddresses = lineManEmp.GetHomeAddresses();
                    if (homeAddresses.Count > 0)
                    {
                        var home = homeAddresses.HomeLocations.ElementAt(0);
                        Address.Delete(Moqs.CurrentUser(), home.LocationID);
                    }

                    var workAddresses = lineManEmp.GetWorkAddresses();

                    if (workAddresses.Count > 0)
                    {
                        var work = workAddresses.WorkLocations.ElementAt(0).Value;
                        Address.Delete(Moqs.CurrentUser(), work.LocationID);
                    }

                    lineManEmp.Delete(Moqs.CurrentUser());

                }
            }

       

            cESRTrustObject.DeleteTrust();
            cImportTemplateObject.DeleteImportTemplate();
        }

        #endregion

        /// <summary>
        /// A test to validate the ESR Outbound import with a valid file format
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ValidateImportWithAValidFileFormatTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int templateId = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();
            var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);

            bool valid = target.validateImport(0);
            Assert.AreEqual(true, valid, "validateImport(0) returned False when True was expected.");
        }

        /// <summary>
        /// A test to validate the ESR Outbound import with no header record
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ValidateImportWithNoHeaderRecordTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int templateId = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithNoHeaderRecord();
            var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);

            bool valid = target.validateImport(0);
            Assert.AreEqual(false, valid, "validateImport(0) returned true when false was expected.");
        }

        /// <summary>
        /// A test to validate the ESR Outbound import with no footer record
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ValidateImportWithNoFooterRecordTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int templateId = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithNoFooterRecord();
            var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);

            bool valid = target.validateImport(0);
            Assert.AreEqual(false, valid, "validateImport(0) returned true when false was expected.");
        }

        /// <summary>
        /// A test to validate the ESR Outbound import with an invalid file format
        /// The no of records in the footer record does not match the number of records in the file
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ValidateImportWithInvalidNumberRecordsTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int templateId = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithInvalidNumberRecords();
            var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);

            bool valid = target.validateImport(0);
            Assert.AreEqual(false, valid, "validateImport(0) returned true when false was expected.");
        }

        /// <summary>
        /// A test to save an employee where the employee number in the ESR Outbound file is set to nothing
        /// and the employeeId is 0 for the assignment number import
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportOutboundWhereEmployeeNumberSetToNothingAndEmployeeIdIsZeroForAssignmentsTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int templateId = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithEmployeeNumberSetToNothing();
            var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
            target.importOutboundData(0);
            int empId = employees.getEmployeeidByAssignment(accountId, "10805521");

            Assert.AreEqual(0, empId, string.Format("Employee ID should be zero as the import has no assignment number. EmpId:{0}", empId));
        }

        /// <summary>
        /// Add a new employee from an ESR Outbound employee record where 'Employee Number', 'First Name', 'Last Name', 'Employee Address Line 1', 
        /// 'Employee Address Postcode', 'Employee Address Country' are set as standard to cover all if statements associated. The home address should not exist either
        /// Set the 'Title' to have a value as this can be left blank on the ESR File record, but not allowed to be added as a blank value to the database.
        /// Also set a username format and a home address format so these areas can be covered in the method.
        /// Check an ID is returned and then check the added values in the database match what has been added, including the formats set for the username and home location.
        /// -
        /// Save a new assignment with all required values set, work location does not exist and 'Assignment Location' has a value
        /// Check the values saved match what has been added. This includes the new work address, whether it has been assigned to the employee and the Assignment Record details.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportOutboundWithFullInfoForEmployeeAndAssignmentTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int templateId = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

                // Set the formats to make sure the values are correct when the import has ran
                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")");

                var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
                target.importOutboundData(0);

                int empId = employees.getEmployeeidByAssignment(accountId, "10805521");
                cGlobalVariables.OutboundEmployeID = empId;

                Employee emp = employees.GetEmployeeById(empId);
                cEmployeeHomeLocation homeAddress = emp.GetHomeAddresses().HomeLocations.FirstOrDefault();
                cGlobalVariables.HomeAddressID = homeAddress.LocationID;

                cEmployeeWorkLocation workAddress = emp.GetWorkAddresses().WorkLocations.FirstOrDefault().Value;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;

                Address compHome = Address.Get(GlobalTestVariables.AccountId, homeAddress.LocationID);

                var clsAssign = new cESRAssignments(accountId, empId);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                Address compWork = Address.Get(GlobalTestVariables.AccountId, workAddress.LocationID);

                // Asserts
                Assert.IsTrue(empId > 0, string.Format("No Employee found by getEmployeeidByAssignment({0}, {1})", accountId, "10805521"));
                Assert.IsNotNull(emp, string.Format("No Employee Record found by GetEmployeeById({0})", empId));

                // Make sure the username format is correct too
                this.CheckEmployeeasserts(emp);

                // If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress, string.Format("GetHomeAddresses({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(compHome, string.Format("Home Address not returned by Get({0}, {1})", GlobalTestVariables.AccountId, homeAddress.LocationID));

                // Check the home address format
                Assert.AreEqual("61 Westwood Road, BB12 0HR", compHome.FriendlyName, string.Format("Home address FriendlyName should equal {0} but has returned {1}, Address id = {2}", "61 Westwood Road, BB12 0HR", compHome.FriendlyName, compHome.Identifier));
                this.CheckAssignmentAsserts(assignment);
                Assert.IsNotNull(workAddress, string.Format("GetWorkAddresses({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(compWork, "Work address Get({0}, {1}) did not return a value", GlobalTestVariables.AccountId, workAddress.LocationID);

                // Check the work address name is the assignment location
                Assert.AreEqual("Church Street, BB11 2DL", compWork.FriendlyName, string.Format("Work address FriendlyName should equal {0} but {1} was returned.  Address Id = {2}", "Church Street, BB11 2DL", compWork.FriendlyName, compWork.Identifier));
            }
            finally
            {
                // Set the formats to defaults
                cGlobalPropertiesObject.UpdateUsernameFormat(string.Empty);
                cGlobalPropertiesObject.UpdateHomeAddressFormat(string.Empty);

                // This test should have OutboundEmployee ID, home location ID and Work Location ID
                if (cGlobalVariables.OutboundEmployeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "10805521");
                    cGlobalVariables.OutboundEmployeID = employee;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.HomeAddressID == 0)
                {
                    cGlobalVariables.HomeAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.WorkAddressID == 0)
                {
                    cGlobalVariables.WorkAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }
            }
        }

        /// <summary>
        /// Same as the previous except with no username or home address formats set
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportOutboundWithFullInfoNoUsernameOrHomeLocationFormatSetTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int templateId = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

                cGlobalPropertiesObject.UpdateUsernameFormat(string.Empty); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(string.Empty); // resets to default

                var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);

                target.importOutboundData(0);

                int empId = employees.getEmployeeidByAssignment(accountId, "10805521");
                cGlobalVariables.OutboundEmployeID = empId;
                Employee emp = employees.GetEmployeeById(empId);
                cEmployeeHomeLocation homeAddress = emp.GetHomeAddresses().HomeLocations.FirstOrDefault();
                cGlobalVariables.HomeAddressID = homeAddress.LocationID;

                Address compHome = Address.Get(GlobalTestVariables.AccountId, homeAddress.LocationID);
                var clsAssign = new cESRAssignments(accountId, empId);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");
                cEmployeeWorkLocation workAddress = emp.GetWorkAddresses().WorkLocations.FirstOrDefault().Value;
                Address compWork = Address.Get(GlobalTestVariables.AccountId, workAddress.LocationID);
                cGlobalVariables.WorkAddressID = workAddress.LocationID;

                // Asserts
                Assert.IsTrue(empId > 0, string.Format("getEmployeeByAssignment({0}, {1}) did not return a value", accountId, "10805521"));
                Assert.IsNotNull(emp, string.Format("GetEmployeeById({0}) did not return a value", empId));

                // Make sure the username format is correct too
                this.CheckEmployeeasserts(emp, usrname: "Debra.Scott");

                // If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress, string.Format("GetHomeAddresses({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(compHome, string.Format("Get({0}, {1}) did not return a value", GlobalTestVariables.AccountId, homeAddress.LocationID));

                // Check the home address format
                Assert.AreEqual("61 Westwood Road, BB12 0HR", compHome.FriendlyName, string.Format("Address FriendlyName should be {0}, but is actually {1}.  Address ID = {2}", "61 Westwood Road, BB12 0HR", compHome.FriendlyName, compHome.Identifier));
                this.CheckAssignmentAsserts(assignment);
                Assert.IsNotNull(workAddress, string.Format("GetWorkAddresses({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(compWork, "Get({0}, {1}) did not return a value", GlobalTestVariables.AccountId, workAddress.LocationID);

                // Check the work address name is the assignment location
                Assert.AreEqual("Church Street, BB11 2DL", compWork.FriendlyName, string.Format("Work address FriendlyName should equal {0} but {1} was returned.  Address Id = {2}", "Church Street, BB11 2DL", compWork.FriendlyName, compWork.Identifier));
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(string.Empty); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(string.Empty); // resets to default
                // This test should have OutboundEmployee ID, home location ID and Work Location ID
                if (cGlobalVariables.OutboundEmployeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "Debra.Scott");
                    cGlobalVariables.OutboundEmployeID = employee;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.HomeAddressID == 0)
                {
                    cGlobalVariables.HomeAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.WorkAddressID == 0)
                {
                    cGlobalVariables.WorkAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }
            }
        }

        /// <summary>
        /// Import an ESR Outbound file with no employee title set
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportOutboundWithNoEmployeeTitleSetTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int templateId = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithNoTitle();

                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]"); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")"); // resets to default

                var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);

                target.importOutboundData(0);

                int empId = employees.getEmployeeidByAssignment(accountId, "10805521");
                cGlobalVariables.OutboundEmployeID = empId;
                Employee emp = employees.GetEmployeeById(empId);
                var homeAddress = emp.GetHomeAddresses().HomeLocations.FirstOrDefault();
                cGlobalVariables.HomeAddressID = homeAddress.LocationID;

                Address comp = Address.Get(GlobalTestVariables.AccountId, homeAddress.LocationID);
                var clsAssign = new cESRAssignments(accountId, empId);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");
                cEmployeeWorkLocation workAddress = emp.GetWorkAddresses().WorkLocations.FirstOrDefault().Value;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;
                Address compWork = Address.Get(GlobalTestVariables.AccountId, workAddress.LocationID);

                // Asserts
                Assert.IsTrue(empId > 0, string.Format("getEmployeeidByAssignment({0}, {1}) did not return a value", accountId, "10805521"));
                Assert.IsNotNull(emp, string.Format("GetEmployeeById({0}) did not return a value", empId));

                // Make sure the username format is correct too
                this.CheckEmployeeasserts(emp, title: ".");

                // If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress, string.Format("GetHomeLocations({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(comp, string.Format("GetCompanyById({0}) did not return a value", homeAddress.LocationID));

                // Check the home address format
                Assert.AreEqual("61 Westwood Road, BB12 0HR", comp.FriendlyName, string.Format("Address FriendlyName should be equal to {0} but was actually {1}.  Address id={2}", "61 Westwood Road, BB12 0HR", comp.FriendlyName, comp.Identifier));
                this.CheckAssignmentAsserts(assignment);
                Assert.IsNotNull(workAddress, string.Format("GetWorkLocation({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(compWork, "GetCompanyById({0}) did not return a value", workAddress.LocationID);

                // Check the work address name is the assignment location
                Assert.AreEqual("Church Street, BB11 2DL", compWork.FriendlyName, string.Format("Work address FriendlyName should equal {0} but {1} was returned.  Address Id = {2}", "Church Street, BB11 2DL", compWork.FriendlyName, compWork.Identifier));
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(string.Empty); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(string.Empty); // resets to default
                // This test should have OutboundEmployee ID, home location ID and Work Location ID
                if (cGlobalVariables.OutboundEmployeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "10805521");
                    cGlobalVariables.OutboundEmployeID = employee;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.HomeAddressID == 0)
                {
                    cGlobalVariables.HomeAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.WorkAddressID == 0)
                {
                    cGlobalVariables.WorkAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }
            }
        }

        /// <summary>
        /// Run an import where the employee and assignment record already exist and update some data such as the
        /// email address for the employee and final assignment end date for the assignment
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportEditedOutboundWithFullInfoForEmployeeAndAssignmentTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int templateId = cGlobalVariables.TemplateID;

                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

                // Set the formats to make sure the values are correct when the import has ran
                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")");

                var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
                target.importOutboundData(0);

                // Edit the employee and assignment
                arrFileData = cImportTemplateObject.EditedDummyESROutboundFileInfo();
                target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
                target.importOutboundData(0);

                int empId = employees.getEmployeeidByAssignment(accountId, "10805521");
                cGlobalVariables.OutboundEmployeID = empId;

                Employee emp = employees.GetEmployeeById(empId);
                cEmployeeHomeLocation homeAddress = emp.GetHomeAddresses().HomeLocations.FirstOrDefault();

                Address comp = Address.Get(GlobalTestVariables.AccountId, homeAddress.LocationID);

                var clsAssign = new cESRAssignments(accountId, empId);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                cEmployeeWorkLocation workAddress = emp.GetWorkAddresses().WorkLocations.FirstOrDefault().Value;
                Address compWork = Address.Get(GlobalTestVariables.AccountId, workAddress.LocationID);

                // Asserts
                Assert.IsTrue(empId > 0, string.Format("getEmployeeidByAssignment({0}, {1}) did not return a value", accountId, "10805521"));
                Assert.IsNotNull(emp, string.Format("GetEmployeeById({0}) did not return a value", empId));

                // Make sure the username format is correct too
                this.CheckEmployeeasserts(emp, email: "editedTest@test.com");
                
                // If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress, "Employee.GetHomeAddresses().HomeLocations.FirstOrDefault() did not return a value");
                Assert.IsNotNull(comp, string.Format("Address.Get({0}, {1}) did not return a value", GlobalTestVariables.AccountId, homeAddress.LocationID));

                // Check the home address format
                Assert.AreEqual("61 Westwood Road, BB12 0HR", comp.FriendlyName, string.Format("Address FriendlyName should be {0}, but is actually {1}.  Address ID = {2}", "61 Westwood Road, BB12 0HR", comp.FriendlyName, comp.Identifier));
                this.CheckAssignmentAsserts(assignment, finalassignmentstartdate: "01/05/2010");
                Assert.IsNotNull(workAddress, string.Format("GetWorkLocation({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(compWork, "GetCompanyById({0}) did not return a value", workAddress.LocationID);

                // Check the work address name is the assignment location
                Assert.AreEqual("Church Street, BB11 2DL", compWork.FriendlyName, string.Format("Work address FriendlyName should equal {0} but {1} was returned.  Address Id = {2}", "Church Street, BB11 2DL", compWork.FriendlyName, compWork.Identifier));
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(string.Empty); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(string.Empty); // resets to default
                // This test should have OutboundEmployee ID, home location ID and Work Location ID
                if (cGlobalVariables.OutboundEmployeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "10805521");
                    cGlobalVariables.OutboundEmployeID = employee;
                }

                if (cGlobalVariables.OutboundEmployeID > 0)
                {
                    var address = employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID);

                    if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.HomeAddressID == 0)
                    {
                        cGlobalVariables.HomeAddressID = address.GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                    }

                    if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.WorkAddressID == 0)
                    {
                        cGlobalVariables.WorkAddressID = address.GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                    }
                }
            }
        }

        /// <summary>
        /// Import ESR Outbound file with home and office addresses already existing but not assigned to the employee
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportOutboundWithHomeAndWorkAddressesExistingButNotAssignedTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int templateId = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

                var clsGlobalCountries = new cGlobalCountries();
                cGlobalCountry globCountry = clsGlobalCountries.getGlobalCountryByAlphaCode("GB");

                // Create home address
                Address.Save(Moqs.CurrentUser(), 0, string.Empty, "61 Westwood Road", "", "", "Burnley", "", globCountry.GlobalCountryId, "BB12 0HR", "", "", "", 0, false, Address.AddressCreationMethod.EsrOutbound);

                // Create work address
                Address.Save(Moqs.CurrentUser(), 0, string.Empty, "Church Street", "", "", "Burnley", "", globCountry.GlobalCountryId, "BB12 0HR", "", "", "", 0, false, Address.AddressCreationMethod.EsrOutbound);

                // Set the formats to make sure the values are correct when the import has ran
                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")");

                var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
                target.importOutboundData(0);

                int empId = employees.getEmployeeidByAssignment(accountId, "10805521");
                cGlobalVariables.OutboundEmployeID = empId;

                Employee emp = employees.GetEmployeeById(empId);
                var homeAddress = emp.GetHomeAddresses().HomeLocations.FirstOrDefault();
                cGlobalVariables.HomeAddressID = homeAddress.LocationID;

                Address comp = Address.Get(GlobalTestVariables.AccountId, homeAddress.LocationID);

                var clsAssign = new cESRAssignments(accountId, empId);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                cEmployeeWorkLocation workAddress = emp.GetWorkAddresses().WorkLocations.FirstOrDefault().Value;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;
                Address compWork = Address.Get(GlobalTestVariables.AccountId, workAddress.LocationID);

                // Asserts
                Assert.IsTrue(empId > 0, string.Format("getEmployeeidByAssignment({0}, {1}) did not return a value", accountId, "10805521"));
                Assert.IsNotNull(emp, string.Format("GetEmployeeById({0}) did not return a value", empId));
                this.CheckEmployeeasserts(emp);

                // If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress, string.Format("GetHomeLocations({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(comp, string.Format("GetCompanyById({0}) did not return a value", homeAddress.LocationID));

                // Check the home address format
                Assert.AreEqual("61 Westwood Road, Burnley, BB12 0HR", comp.FriendlyName, string.Format("Address.FriendlyName should be equal to {0} but was actually {1}.  Address id={2}", "61 Westwood Road, Burnley, BB12 0HR", comp.FriendlyName, comp.Identifier));
                this.CheckAssignmentAsserts(assignment);
                Assert.IsNotNull(workAddress, string.Format("GetWorkLocation({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(compWork, "GetCompanyById({0}) did not return a value", workAddress.LocationID);

                // Check the work address name is the assignment location
                Assert.AreEqual("Church Street, BB11 2DL", compWork.FriendlyName, string.Format("Work address FriendlyName should equal {0} but {1} was returned.  Address Id = {2}", "Church Street, BB11 2DL", compWork.FriendlyName, compWork.Identifier));
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(string.Empty); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(string.Empty); // resets to default
                // This test should have OutboundEmployee ID, home location ID and Work Location ID
                if (cGlobalVariables.OutboundEmployeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "10805521");
                    cGlobalVariables.OutboundEmployeID = employee;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.HomeAddressID == 0)
                {
                    cGlobalVariables.HomeAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.WorkAddressID == 0)
                {
                    cGlobalVariables.WorkAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }
            }
        }

        /// <summary>
        /// Import outbound ESR file where the assignment location is set to nothing
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportOutboundWithAssignmentLocationSetToNothingTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int templateId = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithNoAssignmentLocationSet();

                // Set the formats to make sure the values are correct when the import has ran
                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")");

                var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
                target.importOutboundData(0);

                int empId = employees.getEmployeeidByAssignment(accountId, "10805521");
                cGlobalVariables.OutboundEmployeID = empId;

                Employee emp = employees.GetEmployeeById(empId);

                var homeAddress = emp.GetHomeAddresses().HomeLocations.FirstOrDefault();
                cGlobalVariables.HomeAddressID = homeAddress.LocationID;

                Address comp = Address.Get(GlobalTestVariables.AccountId, homeAddress.LocationID);
                var clsAssign = new cESRAssignments(accountId, empId);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");
                cEmployeeWorkLocation workAddress = emp.GetWorkAddresses().WorkLocations.FirstOrDefault().Value;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;
                Address compWork = Address.Get(GlobalTestVariables.AccountId, workAddress.LocationID);

                // Asserts
                Assert.IsTrue(empId > 0, string.Format("getEmployeeidByAssignment({0}, {1}) did not return a value", accountId, "10805521"));
                Assert.IsNotNull(emp, string.Format("GetEmployeeById({0}) did not return a value", empId));
                this.CheckEmployeeasserts(emp);

                // If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress, string.Format("GetHomeLocations({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(comp, string.Format("GetCompanyById({0}) did not return a value", homeAddress.LocationID));

                // Check the home address format
                Assert.AreEqual("61 Westwood Road, BB12 0HR", comp.FriendlyName, string.Format("Address.FriendlyName should be equal to {0} but was actually {1}.  Address id = {2}", "61 Westwood Road, BB12 0HR", comp.FriendlyName, comp.Identifier));
                this.CheckAssignmentAsserts(assignment, assignmentLocation: string.Empty);
                Assert.IsNotNull(workAddress, string.Format("GetWorkLocations({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(compWork, string.Format("GetCompanyById({0}) did not return a value", workAddress.LocationID));

                // Check the work address name is not the assignment location (as this is blank), should default to address 1st line and postcode
                Assert.AreEqual("Church Street, BB11 2DL", compWork.FriendlyName, string.Format("Address.FriendlyName shoule be equal to {0} but was actually {1}.  Address id = {2}", "Church Street, BB11 2DL", compWork.FriendlyName, compWork.Identifier));
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(string.Empty); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(string.Empty); // resets to default
                // This test should have OutboundEmployee ID, home location ID and Work Location ID
                if (cGlobalVariables.OutboundEmployeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "10805521");
                    cGlobalVariables.OutboundEmployeID = employee;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.HomeAddressID == 0)
                {
                    cGlobalVariables.HomeAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.WorkAddressID == 0)
                {
                    cGlobalVariables.WorkAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }
            }
        }

        /// <summary>
        /// Import ESR Outbound file with an employee having home and work addresses already existing 
        /// but the file has new home and office addresses
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportEditedOutboundWithNewHomeAndWorkAddressesTest()
        {
            Employee emp = null;
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int templateId = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

                var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
                target.importOutboundData(0);

                // Edit the employee and assignment
                arrFileData = cImportTemplateObject.EditedDummyESROutboundFileInfoWithNewHomeAndWorkAddresses();
                target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
                target.importOutboundData(0);

                int empId = employees.getEmployeeidByAssignment(accountId, "10805521");
                cGlobalVariables.OutboundEmployeID = empId;

                emp = employees.GetEmployeeById(empId);

                cEmployeeHomeLocation homeAddress = emp.GetHomeAddresses().HomeLocations.ElementAt(1);

                Address comp = Address.Get(GlobalTestVariables.AccountId, homeAddress.LocationID);

                var clsAssign = new cESRAssignments(accountId, empId);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                cEmployeeWorkLocation workAddress = emp.GetWorkAddresses().WorkLocations.ElementAt(1).Value;

                Address compWork = Address.Get(GlobalTestVariables.AccountId, workAddress.LocationID);

                // Asserts
                Assert.IsTrue(empId > 0, string.Format("getEmployeeidByAssignment({0}, {1}) did not return a value", accountId, "10805521"));
                Assert.IsNotNull(emp, string.Format("GetEmployeeById({0}) did not return a value", empId));

                // Make sure the username format is correct too
                this.CheckEmployeeasserts(emp, usrname: "Debra.Scott", address1: "1 Home Test Lane", postcode: "NG35 4TY", city: "TestTown", email: "editedTest@test.com");

                // If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress, string.Format("GetHomeLocations({0}) did not return a value", emp.EmployeeID));
                Assert.IsNotNull(comp, string.Format("GetCompanyById({0}) did not return a value", homeAddress.LocationID));

                // Check the home address format
                Assert.AreEqual("1 Home Test Lane, NG35 4TY", comp.FriendlyName, string.Format("Address.FriendlyName should return {0} but was actually {1}.  Address id={2}", "1 Home Test Lane, NG35 4TY", comp.FriendlyName, comp.Identifier));

                this.CheckAssignmentAsserts(assignment, finalassignmentstartdate: "01/05/2010", address1: "1 Work Test lane", postcode: "NG12 8YU", assignmentLocation: "Unit Test Work");
                Assert.IsNotNull(workAddress, string.Format("GetWorkLocations({0}) did not reutn a value", emp.EmployeeID));

                Assert.IsNotNull(compWork, string.Format("GetCompanyById({0}) did not return a value", workAddress.LocationID));

                // Check the work address name is the assignment location
                Assert.AreEqual("1 Work Test lane, NG12 8YU", compWork.FriendlyName, string.Format("Address.FriendlyName should have been {0} but was actually {1}, Address ID = {2}", "1 Work Test lane, NG12 8YU", compWork.FriendlyName, compWork.Identifier));
            }
            finally
            {
                if (emp != null)
                {
                    cGlobalVariables.HomeAddressID = emp.GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                    cGlobalVariables.WorkAddressID = emp.GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }

                // This test should have OutboundEmployee ID, home location ID and Work Location ID
                if (cGlobalVariables.OutboundEmployeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "Debra.Scott");
                    cGlobalVariables.OutboundEmployeID = employee;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.HomeAddressID == 0)
                {
                    cGlobalVariables.HomeAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.WorkAddressID == 0)
                {
                    cGlobalVariables.WorkAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }
            }
        }

        /// <summary>
        /// Import an ESR Outbound file where an employee line manager is being set when the employee line manager is added
        /// after the employee associated
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportOutboundWithLineManagerNotPreExistingTest()
        {
            Employee emp = null;
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int templateId = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithLineManagerNotPreExisting();

                var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
                target.importOutboundData(0);

                int empId = employees.getEmployeeidByAssignment(accountId, "10805521");
                cGlobalVariables.OutboundEmployeID = empId;

                int lineManagerEmpId = employees.getEmployeeidByAssignment(accountId, "21272386");
                
                cGlobalVariables.OutboundLinemanagerEmployeeID = lineManagerEmpId;
                emp = employees.GetEmployeeById(empId);

                // Asserts
                Assert.IsTrue(empId > 0, string.Format("getEmployeeidByAssignment({0}, {1}) did not return a value", accountId, "10805521"));
                this.CheckEmployeeasserts(emp, usrname: "Debra.Scott", address1: "1 Home Test Lane", city: "TestTown", postcode: "NG35 4TY", email: "editedTest@test.com");
                Assert.IsTrue(lineManagerEmpId > 0, string.Format("getEmployeeidByAssignment({0}, {1}) did not return a value", accountId, "21272386"));
   
                Assert.IsNotNull(emp, string.Format("GetEmployeeById({0}) did not return a value", empId));

                Assert.AreEqual(emp.LineManager, lineManagerEmpId, string.Format("Employees line manager <{0}>, does not match the expected Line Manager <{1}>. the current employee username is {2}", emp.LineManager, lineManagerEmpId, emp.Username));
            }
            finally
            {
                if (emp != null)
                {
                    cGlobalVariables.HomeAddressID = emp.GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                    cGlobalVariables.WorkAddressID = emp.GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }

                // This test should have OutboundEmployee ID, home location ID, Work Location ID, and OutboundLinemanager ID
                if (cGlobalVariables.OutboundEmployeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "Debra.Scott");
                    cGlobalVariables.OutboundEmployeID = employee;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.HomeAddressID == 0)
                {
                    cGlobalVariables.HomeAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.WorkAddressID == 0)
                {
                    cGlobalVariables.WorkAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }

                if (cGlobalVariables.OutboundLinemanagerEmployeeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "Catherine.Gill");
                    cGlobalVariables.OutboundLinemanagerEmployeeID = employee;
                }
            }
       }
    
        /// <summary>
        /// Import an ESR Outbound file where an employee line manager is being set when the employee line manager is added
        /// after the employee associated
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void ImportOutboundWithLineManagerPreExistingTest()
        {
            Employee emp = null;
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int templateId = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithLineManagerPreExisting();

                var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
                target.importOutboundData(0);

                int empId = employees.getEmployeeidByAssignment(accountId, "10805521");
                cGlobalVariables.OutboundEmployeID = empId;
                int lineManagerEmpId = employees.getEmployeeidByAssignment(accountId, "21272386");
                cGlobalVariables.OutboundLinemanagerEmployeeID = lineManagerEmpId;
                emp = employees.GetEmployeeById(empId);

                // Asserts
                this.CheckEmployeeasserts(emp, usrname: "Debra.Scott", address1: "1 Home Test Lane", city: "TestTown", postcode: "NG35 4TY", email: "editedTest@test.com");
                Assert.IsTrue(empId > 0, string.Format("getEmployeeidByAssignment({0}, {1}) did not return an employee", accountId, "10805521"));
                
                Assert.IsTrue(lineManagerEmpId > 0, string.Format("getEmployeeidByAssignment({0}, {1}) did not return a line manager", accountId, "21272386"));

                Assert.IsNotNull(emp, string.Format("GetEmployeeById({0}) did not return an employee", empId));

                Assert.AreEqual(emp.LineManager, lineManagerEmpId, string.Format("Expected to get en employee with line manager id = {0} but actually got {1}.  Employee id = {2}", lineManagerEmpId, emp.LineManager, emp.EmployeeID));
            }
            finally
            {
                if (emp != null)
                {
                    cGlobalVariables.HomeAddressID = emp.GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                    cGlobalVariables.WorkAddressID = emp.GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }

                // This test should have OutboundEmployee ID, home location ID, Work Location ID, and OutboundLinemanager ID
                if (cGlobalVariables.OutboundEmployeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "Debra.Scott");
                    cGlobalVariables.OutboundEmployeID = employee;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.HomeAddressID == 0)
                {
                    cGlobalVariables.HomeAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetHomeAddresses().HomeLocations.FirstOrDefault().EmployeeLocationID;
                }

                if (cGlobalVariables.OutboundEmployeID != 0 && cGlobalVariables.WorkAddressID == 0)
                {
                    cGlobalVariables.WorkAddressID =
                        employees.GetEmployeeById(cGlobalVariables.OutboundEmployeID).GetWorkAddresses().WorkLocations.FirstOrDefault().Key;
                }

                if (cGlobalVariables.OutboundLinemanagerEmployeeID == 0)
                {
                    int employee = employees.getEmployeeidByUsername(accountId, "Catherine.Gill");
                    cGlobalVariables.OutboundLinemanagerEmployeeID = employee;
                }
            }
        }

        [TestCategory("Spend Management"), TestCategory("ESR"), TestCategory("ImportTemplates"), TestMethod]
        public void GetElementType_Valid()
        {
            cImportTemplates templates = new cImportTemplates(accountId);
            PrivateObject testObj = new PrivateObject(templates);

            ImportElementType expected = ImportElementType.Location;
            var actual = cImportTemplates.GetElementType("Location");
            Assert.AreEqual(expected, actual);
        }

        [TestCategory("Spend Management"), TestCategory("ESR"), TestCategory("ImportTemplates"), TestMethod]
        public void GetElementType_Invalid()
        {
            cImportTemplates templates = new cImportTemplates(accountId);
            PrivateObject testObj = new PrivateObject(templates);

            ImportElementType expected = ImportElementType.None;
            var actual = cImportTemplates.GetElementType("MyUnknownType");
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for cESRImport Constructor
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void CEsrImportConstructorTest()
        {
            cImportTemplateObject.CreateImportTemplate();
            int templateId = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();
            var target = new cESRImport(Moqs.CurrentUser(), templateId, arrFileData);
            Assert.IsNotNull(target, string.Format("cESRImport({0}, {1}, {2}) did not return a valid object", accountId, templateId, arrFileData));
        }

        /// <summary>
        /// The ESR duplicate trust VPD test.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("ESR"), TestMethod]
        public void CEsrDuplicateTrustVpdTest()
        {
            cImportTemplateObject.CreateImportTemplate();
            int createdTemplateID = cGlobalVariables.NHSTrustID;
            Assert.IsTrue(createdTemplateID > 0, "Template ID should be a positive integer");
            cImportTemplateObject.CreateImportTemplate(false);
            Assert.IsTrue(cGlobalVariables.NHSTrustID == -2, "Should return -2 which is duplicate VPD.");
            cGlobalVariables.NHSTrustID = createdTemplateID;
        }
        #region Private Methods

        /// <summary>
        /// The check assignment asserts for Import tests.
        /// </summary>
        /// <param name="assignment">
        /// The assignment.
        /// </param>
        /// <param name="assignmentnumber">
        /// The assignment number.
        /// </param>
        /// <param name="assignmentLocation">
        /// The assignment Location.
        /// </param>
        /// <param name="earliestassignmentstartdate">
        /// The earliest assignment start date.
        /// </param>
        /// <param name="finalassignmentstartdate">
        /// The final assignment start date.
        /// </param>
        /// <param name="esrAssignmentStatus">
        /// The ESR Assignment Status.
        /// </param>
        /// <param name="address1">
        /// The address 1.
        /// </param>
        /// <param name="postcode">
        /// The postcode.
        /// </param>
        /// <param name="primaryassignment">
        /// The primary assignment.
        /// </param>
        private void CheckAssignmentAsserts(cESRAssignment assignment, string assignmentnumber = "10805521", string assignmentLocation = "638 96 St Peter's Centre", string earliestassignmentstartdate = "01/08/1994", string finalassignmentstartdate = "", ESRAssignmentStatus esrAssignmentStatus = ESRAssignmentStatus.ActiveAssignment, string address1 = "Church Street", string postcode = "BB11 2DL", bool primaryassignment = true)
        {
            DateTime earlistAssignmentStartDate;
            DateTime.TryParse(earliestassignmentstartdate, out earlistAssignmentStartDate);
            DateTime finalAssignmentStartDate;
            DateTime.TryParse(finalassignmentstartdate, out finalAssignmentStartDate);
            
            Assert.IsNotNull(assignment, string.Format("getAssignmentByAssignmentNumber({0}) did not return a value", "10805521"));
            Assert.AreEqual(assignmentnumber, assignment.assignmentnumber, string.Format("Assignment number should equal {0}, but {1} was returned.  Assignment ID = {2}", assignmentnumber, assignment.assignmentnumber, assignment.assignmentid));
            Assert.AreEqual(earlistAssignmentStartDate, assignment.earliestassignmentstartdate, string.Format("Assignment start date should equal {0}, but {1} was returned.  Assignment ID = {2}", earlistAssignmentStartDate, assignment.earliestassignmentstartdate, assignment.assignmentid));
            if (finalassignmentstartdate == string.Empty)
            {
                Assert.AreEqual(null, assignment.finalassignmentenddate, string.Format("Assignment final end date should equal {0}, but {1} was returned.  Assignment ID = {2}", "<null>", assignment.finalassignmentenddate, assignment.assignmentid));
            }
            else
            {
                Assert.AreEqual(finalAssignmentStartDate, assignment.finalassignmentenddate, string.Format("Assignment final end date should equal {0}, but {1} was returned.  Assignment ID = {2}", finalAssignmentStartDate, assignment.finalassignmentenddate, assignment.assignmentid));    
            }

            Assert.AreEqual(esrAssignmentStatus, assignment.assignmentstatus, string.Format("Assignment status should equal {0}, but {1} was returned.  Assignment ID = {2}", esrAssignmentStatus, assignment.assignmentstatus, assignment.assignmentid));
            Assert.AreEqual(address1, assignment.assignmentaddress1, string.Format("Assignment address 1 should equal {0}, but {1} was returned.  Assignment ID = {2}", address1, assignment.assignmentaddress1, assignment.assignmentid));
            Assert.AreEqual(postcode, assignment.assignmentaddresspostcode, string.Format("Assignment postcode should equal {0}, but {1} was returned.  Assignment ID = {2}", postcode, assignment.assignmentaddresspostcode, assignment.assignmentid));
            Assert.AreEqual(primaryassignment, assignment.primaryassignment, string.Format("Assignment primary assignment should equal {0}, but {1} was returned.  Assignment ID = {2}", primaryassignment, assignment.primaryassignment, assignment.assignmentid));
            Assert.AreEqual(assignmentLocation, assignment.assignmentlocation, string.Format("Assignment location should equal {0}, but {1} was returned.  Assignment ID = {2}", assignmentLocation, assignment.assignmentlocation, assignment.assignmentid));
        }

        /// <summary>
        /// The check employee asserts.
        /// </summary>
        /// <param name="employee">
        /// The employee.
        /// </param>
        /// <param name="usrname">
        /// The user name.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="payroll">
        /// The payroll.
        /// </param>
        /// <param name="surname">
        /// The surname.
        /// </param>
        /// <param name="firstname">
        /// The first name.
        /// </param>
        /// <param name="middlenames">
        /// The middle names.
        /// </param>
        /// <param name="address1">
        /// The address 1.
        /// </param>
        /// <param name="city">
        /// The city.
        /// </param>
        /// <param name="postcode">
        /// The postcode.
        /// </param>
        /// <param name="country">
        /// The country.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        private void CheckEmployeeasserts(Employee employee, string usrname = "10805521", string title = "Mrs.", string payroll = "10805521", string surname = "Scott", string firstname = "Debra", string middlenames = "Jane", string address1 = "61 Westwood Road", string city = "", string postcode = "BB12 0HR", string country = "GB", string position = "Nursing and Midwifery Registered|Community Nurse", string email = "test@test.com")
        {
            Assert.AreEqual(usrname, employee.Username, string.Format("Employee username should be {0} but is actually {1}.  EmployeeID={2}", usrname, employee.Username, employee.EmployeeID));
            Assert.AreEqual(title, employee.Title, string.Format("Employee title should be {0} but is actually {1}.  EmployeeID={2}", title, employee.Title, employee.EmployeeID));

            Assert.AreEqual(payroll, employee.PayrollNumber, string.Format("Employee payroll should be {0} but is actually {1}.  EmployeeID={2}", payroll, employee.PayrollNumber, employee.EmployeeID));
            Assert.AreEqual(surname, employee.Surname, string.Format("Employee surname should be {0} but is actually {1}.  EmployeeID={2}", surname, employee.Surname, employee.EmployeeID));
            Assert.AreEqual(firstname, employee.Forename, string.Format("Employee firstname should be {0} but is actually {1}.  EmployeeID={2}", firstname, employee.Forename, employee.EmployeeID));
            Assert.AreEqual(middlenames, employee.MiddleNames, string.Format("Employee middlenames should be {0} but is actually {1}.  EmployeeID={2}", middlenames, employee.MiddleNames, employee.EmployeeID));
            Assert.AreEqual(new DateTime(1994, 08, 01), employee.HiredDate, string.Format("Employee Start date should be {0} but is actually {1}.  EmployeeID={2}", "01/08/1994", employee.HiredDate.ToString(), employee.EmployeeID));
            Assert.AreEqual(null, employee.TerminationDate, string.Format("Employee termination date should be {0} but is actually {1}.  EmployeeID={2}", "<null>", employee.TerminationDate.ToString(), employee.EmployeeID));
            Assert.IsTrue(employee.Password != string.Empty, string.Format("Employee password should be {0} but is actually {1}.  EmployeeID={2}", "<Not Empty>", employee.Password, employee.EmployeeID));
            Assert.AreEqual(email, employee.EmailAddress, string.Format("Employee email should be {0} but is actually {1}.  EmployeeID={2}", email, employee.EmailAddress, employee.EmployeeID));

            // This is an assignment value mapped to the employee table
            Assert.AreEqual(position, employee.Position, string.Format("Employee position should equal {0}, but {1} was returned.  Employee ID = {2}", position, employee.Position, employee.EmployeeID));
        }

        #endregion
    }
}
