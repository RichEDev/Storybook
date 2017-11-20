using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Collections.Generic;
using SpendManagementUnitTests.Global_Objects;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cESRImportTest and is intended
    ///to contain all cESRImportTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cESRImportTest
    {


        private TestContext testContextInstance;

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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cCompanies clsComps = new cCompanies(cGlobalVariables.AccountID);

            cGlobalPropertiesObject.UpdateUsernameFormat("");
            cGlobalPropertiesObject.UpdateHomeAddressFormat("");

            #region Remove any employee and associated details added

            cEmployee emp = clsEmps.GetEmployeeById(cGlobalVariables.OutboundEmployeID);

            if (emp != null)
            {
                if (emp.employeeid > 0)
                {
                    List<int> lstEmpAccessRoles = clsEmps.GetAccessRoles(cGlobalVariables.OutboundEmployeID, cGlobalVariables.SubAccountID);
                    if (lstEmpAccessRoles.Count > 0)
                    {
                        clsEmps.deleteEmployeeAccessRole(cGlobalVariables.OutboundEmployeID, lstEmpAccessRoles[0], cGlobalVariables.SubAccountID);
                    }

                    clsEmps.changeStatus(cGlobalVariables.OutboundEmployeID, true);
                    clsEmps.deleteEmployee(cGlobalVariables.OutboundEmployeID);
                    clsComps.deleteCompany(cGlobalVariables.HomeAddressID);
                    clsComps.deleteCompany(cGlobalVariables.WorkAddressID);
                }
            }

            #endregion

            #region Remove any line manager

            cEmployee lineManEmp = clsEmps.GetEmployeeById(cGlobalVariables.OutboundLinemanagerEmployeeID);
            clsEmps.GetHomeLocations(cGlobalVariables.OutboundLinemanagerEmployeeID);
            clsEmps.GetWorkLocations(cGlobalVariables.OutboundLinemanagerEmployeeID);

            if (lineManEmp != null)
            {
                if (lineManEmp.employeeid > 0)
                {
                    List<int> lstEmpAccessRoles = clsEmps.GetAccessRoles(cGlobalVariables.OutboundLinemanagerEmployeeID, cGlobalVariables.SubAccountID);
                    if (lstEmpAccessRoles.Count > 0)
                    {
                        clsEmps.deleteEmployeeAccessRole(cGlobalVariables.OutboundLinemanagerEmployeeID, lstEmpAccessRoles[0], cGlobalVariables.SubAccountID);
                    }

                    clsEmps.changeStatus(cGlobalVariables.OutboundLinemanagerEmployeeID, true);
                    clsEmps.deleteEmployee(cGlobalVariables.OutboundLinemanagerEmployeeID);

                    if (lineManEmp._HomeLocations.Count > 0)
                    {
                        clsComps.deleteCompany(lineManEmp._HomeLocations.Values[0].LocationID);
                    }
                    if (lineManEmp._WorkLocations.Count > 0)
                    {
                        clsComps.deleteCompany(lineManEmp._WorkLocations.Values[0].LocationID);
                    }
                }
            }

            #endregion

            cESRTrustObject.DeleteTrust();
            cImportTemplateObject.DeleteImportTemplate();
        }
        
        #endregion


        /// <summary>
        ///A test to validate the ESR Outbound import with a valid file format
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ValidateImportWithAValidFileFormatTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int AccountID = cGlobalVariables.AccountID;
            int TemplateID = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();
            cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);

            bool valid = target.validateImport(0);
            Assert.AreEqual(true, valid);
        }

        /// <summary>
        ///A test to validate the ESR Outbound import with no header record
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ValidateImportWithNoHeaderRecordTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int AccountID = cGlobalVariables.AccountID;
            int TemplateID = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithNoHeaderRecord();
            cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);

            bool valid = target.validateImport(0);
            Assert.AreEqual(false, valid);
        }

        /// <summary>
        ///A test to validate the ESR Outbound import with no footer record
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ValidateImportWithNoFooterRecordTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int AccountID = cGlobalVariables.AccountID;
            int TemplateID = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithNoFooterRecord();
            cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);

            bool valid = target.validateImport(0);
            Assert.AreEqual(false, valid);
        }

        /// <summary>
        ///A test to validate the ESR Outbound import with an invalid file format
        ///The no of records in the footer record does not match the number of records in the file
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ValidateImportWithInvalidNumberRecordsTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int AccountID = cGlobalVariables.AccountID;
            int TemplateID = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithInvalidNumberRecords();
            cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);

            bool valid = target.validateImport(0);
            Assert.AreEqual(false, valid);
        }

        /// <summary>
        ///A test to save an employee where the employee number in the ESR Outbound file is set to nothing
        ///and the employeeID is 0 for the assignment number import
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportOutboundWhereEmployeeNumberSetToNothingAndEmployeeIDIsZeroForAssignmentsTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int AccountID = cGlobalVariables.AccountID;
            int TemplateID = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithEmployeeNumberSetToNothing();
            cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);

            target.importOutboundData(0);

            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

            Assert.AreEqual(0, empID);
        }

        /// <summary>
        ///Add a new employee from an ESR Outbound employee record where 'Employee Number', 'First Name', 'Last Name', 'Employee Address Line 1', 
        ///'Employee Address Postcode', 'Employee Address Country' are set as standard to cover all if statements associated. The home address should not exist either
        ///Set the 'Title' to have a value as this can be left blank on the ESR File record, but not allowed to be added as a blank value to the database.
        ///Also set a username format and a home address format so these areas can be covered in the method.
        ///Check an ID is returned and then check the added values in the database match what has been added, including the formats set for the username and home location.
        ///
        /// Save a new assignment with all required values set, work location does not exist and 'Assignment Location' has a value
        /// Check the values saved match what has been added. This includes the new work address, whether it has been assigned to the employee and the Assignment Record details.
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportOutboundWithFullInfoForEmployeeAndAssignmentTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int AccountID = cGlobalVariables.AccountID;
                int TemplateID = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

                //Set the formats to make sure the values are correct when the import has ran
                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")");

                cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);
                target.importOutboundData(0);

                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

                Assert.IsTrue(empID > 0);

                cGlobalVariables.OutboundEmployeID = empID;

                cEmployee emp = clsEmps.GetEmployeeById(empID);

                Assert.IsNotNull(emp);

                #region Check employee record values

                //Make sure the username format is correct too
                Assert.AreEqual("10805521", emp.username);
                Assert.AreEqual("10805521", emp.payroll);
                Assert.AreEqual("Mrs.", emp.title);
                Assert.AreEqual("Scott", emp.surname);
                Assert.AreEqual("Debra", emp.firstname);
                Assert.AreEqual("Jane", emp.middlenames);
                Assert.AreEqual(new DateTime(1994, 08, 01), emp.hiredate);
                Assert.AreEqual(null, emp.terminationdate);
                Assert.AreEqual("61 Westwood Road", emp.address1);
                Assert.AreEqual("", emp.city);
                Assert.AreEqual("BB12 0HR", emp.postcode);
                Assert.AreEqual("GB", emp.country);
                Assert.AreEqual("test@test.com", emp.email);
                Assert.IsTrue(emp.password != "");

                clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
                cEmployeeHomeLocation homeAddress = emp._HomeLocations.Values[0];

                //If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress);

                cCompanies clsComps = new cCompanies(cGlobalVariables.AccountID);

                cCompany comp = clsComps.GetCompanyById(homeAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the home address format
                Assert.AreEqual("10805521__Home", comp.company);

                #endregion

                #region Check assignment record values

                cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, empID);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                Assert.IsNotNull(assignment);

                Assert.AreEqual("10805521", assignment.assignmentnumber);
                Assert.AreEqual(new DateTime(1994, 08, 01), assignment.earliestassignmentstartdate);
                Assert.AreEqual(null, assignment.finalassignmentenddate);
                Assert.AreEqual(ESRAssignmentStatus.ActiveAssignment, assignment.assignmentstatus);
                Assert.AreEqual("Church Street", assignment.assignmentaddress1);
                Assert.AreEqual("BB11 2DL", assignment.assignmentaddresspostcode);
                Assert.AreEqual(true, assignment.primaryassignment);
                Assert.AreEqual("638 96 St Peter's Centre", assignment.assignmentlocation);

                //This is an assignment value mapped to the employee table
                Assert.AreEqual("Nursing and Midwifery Registered|Community Nurse", emp.position);

                clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
                cEmployeeWorkLocation workAddress = emp._WorkLocations.Values[0];

                Assert.IsNotNull(workAddress);

                comp = clsComps.GetCompanyById(workAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the work address name is the assignment location
                Assert.AreEqual("638 96 St Peter's Centre", comp.company);

                #endregion

                cGlobalVariables.HomeAddressID = homeAddress.LocationID;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;
            }
            finally
            {
                //Set the formats to defaults
                cGlobalPropertiesObject.UpdateUsernameFormat("");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("");
            }
        }

        /// <summary>
        /// Same as the previous except with no username or home address formats set
        /// </summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportOutboundWithFullInfoNoUsernameOrHomeLocationFormatSetTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int AccountID = cGlobalVariables.AccountID;
                int TemplateID = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

                cGlobalPropertiesObject.UpdateUsernameFormat(""); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(""); // resets to default

                cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);

                target.importOutboundData(0);

                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

                Assert.IsTrue(empID > 0);

                cGlobalVariables.OutboundEmployeID = empID;

                cEmployee emp = clsEmps.GetEmployeeById(empID);

                Assert.IsNotNull(emp);

                #region Check employee record values

                //Make sure the username format is correct too
                Assert.AreEqual("Debra.Scott", emp.username);
                Assert.AreEqual("10805521", emp.payroll);
                Assert.AreEqual("Mrs.", emp.title);
                Assert.AreEqual("Scott", emp.surname);
                Assert.AreEqual("Debra", emp.firstname);
                Assert.AreEqual("Jane", emp.middlenames);
                Assert.AreEqual(new DateTime(1994, 08, 01), emp.hiredate);
                Assert.AreEqual(null, emp.terminationdate);
                Assert.AreEqual("61 Westwood Road", emp.address1);
                Assert.AreEqual("", emp.city);
                Assert.AreEqual("BB12 0HR", emp.postcode);
                Assert.AreEqual("GB", emp.country);
                Assert.AreEqual("test@test.com", emp.email);
                Assert.IsTrue(emp.password != "");

                clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
                cEmployeeHomeLocation homeAddress = emp._HomeLocations.Values[0];

                //If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress);

                cCompanies clsComps = new cCompanies(cGlobalVariables.AccountID);

                cCompany comp = clsComps.GetCompanyById(homeAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the home address format
                Assert.AreEqual("61 Westwood RoadBB12 0HR", comp.company);

                #endregion

                #region Check assignment record values

                cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, empID);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                Assert.IsNotNull(assignment);

                Assert.AreEqual("10805521", assignment.assignmentnumber);
                Assert.AreEqual(new DateTime(1994, 08, 01), assignment.earliestassignmentstartdate);
                Assert.AreEqual(null, assignment.finalassignmentenddate);
                Assert.AreEqual(ESRAssignmentStatus.ActiveAssignment, assignment.assignmentstatus);
                Assert.AreEqual("Church Street", assignment.assignmentaddress1);
                Assert.AreEqual("BB11 2DL", assignment.assignmentaddresspostcode);
                Assert.AreEqual(true, assignment.primaryassignment);
                Assert.AreEqual("638 96 St Peter's Centre", assignment.assignmentlocation);

                //This is an assignment value mapped to the employee table
                Assert.AreEqual("Nursing and Midwifery Registered|Community Nurse", emp.position);

                clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
                cEmployeeWorkLocation workAddress = emp._WorkLocations.Values[0];

                Assert.IsNotNull(workAddress);

                comp = clsComps.GetCompanyById(workAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the work address name is the assignment location
                Assert.AreEqual("638 96 St Peter's Centre", comp.company);

                #endregion

                cGlobalVariables.HomeAddressID = homeAddress.LocationID;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(""); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(""); // resets to default
            }
        }

        /// <summary>
        /// Import an ESR Outbound file with no employee title set
        /// </summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportOutboundWithNoEmployeeTitleSetTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int AccountID = cGlobalVariables.AccountID;
                int TemplateID = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithNoTitle();

                cGlobalPropertiesObject.UpdateUsernameFormat(""); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(""); // resets to default

                cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);

                target.importOutboundData(0);

                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

                Assert.IsTrue(empID > 0);

                cGlobalVariables.OutboundEmployeID = empID;

                cEmployee emp = clsEmps.GetEmployeeById(empID);

                Assert.IsNotNull(emp);

                #region Check employee record values

                //Make sure the username format is correct too
                Assert.AreEqual("Debra.Scott", emp.username);
                Assert.AreEqual("10805521", emp.payroll);
                Assert.AreEqual(".", emp.title);
                Assert.AreEqual("Scott", emp.surname);
                Assert.AreEqual("Debra", emp.firstname);
                Assert.AreEqual("Jane", emp.middlenames);
                Assert.AreEqual(new DateTime(1994, 08, 01), emp.hiredate);
                Assert.AreEqual(null, emp.terminationdate);
                Assert.AreEqual("61 Westwood Road", emp.address1);
                Assert.AreEqual("", emp.city);
                Assert.AreEqual("BB12 0HR", emp.postcode);
                Assert.AreEqual("GB", emp.country);
                Assert.AreEqual("test@test.com", emp.email);
                Assert.IsTrue(emp.password != "");

                clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
                cEmployeeHomeLocation homeAddress = emp._HomeLocations.Values[0]; ;

                //If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress);

                cCompanies clsComps = new cCompanies(cGlobalVariables.AccountID);

                cCompany comp = clsComps.GetCompanyById(homeAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the home address format
                Assert.AreEqual("61 Westwood RoadBB12 0HR", comp.company);

                #endregion

                #region Check assignment record values

                cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, empID);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                Assert.IsNotNull(assignment);

                Assert.AreEqual("10805521", assignment.assignmentnumber);
                Assert.AreEqual(new DateTime(1994, 08, 01), assignment.earliestassignmentstartdate);
                Assert.AreEqual(null, assignment.finalassignmentenddate);
                Assert.AreEqual(ESRAssignmentStatus.ActiveAssignment, assignment.assignmentstatus);
                Assert.AreEqual("Church Street", assignment.assignmentaddress1);
                Assert.AreEqual("BB11 2DL", assignment.assignmentaddresspostcode);
                Assert.AreEqual(true, assignment.primaryassignment);
                Assert.AreEqual("638 96 St Peter's Centre", assignment.assignmentlocation);

                //This is an assignment value mapped to the employee table
                Assert.AreEqual("Nursing and Midwifery Registered|Community Nurse", emp.position);

                clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
                cEmployeeWorkLocation workAddress = emp._WorkLocations.Values[0];

                Assert.IsNotNull(workAddress);

                comp = clsComps.GetCompanyById(workAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the work address name is the assignment location
                Assert.AreEqual("638 96 St Peter's Centre", comp.company);

                #endregion

                cGlobalVariables.HomeAddressID = homeAddress.LocationID;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(""); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(""); // resets to default
            }
        }

        /// <summary>
        /// Run an import where the employee and assignemnt record already exist and update some data such as the
        /// email address for the employee and final assignment end date for the assignment
        /// </summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportEditedOutboundWithFullInfoForEmployeeAndAssignmentTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int AccountID = cGlobalVariables.AccountID;
                int TemplateID = cGlobalVariables.TemplateID;

                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

                //Set the formats to make sure the values are correct when the import has ran
                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")");

                cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);
                target.importOutboundData(0);

                //Edit the employee and assignment
                arrFileData = cImportTemplateObject.EditedDummyESROutboundFileInfo();
                target = new cESRImport(AccountID, TemplateID, arrFileData);
                target.importOutboundData(0);

                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

                Assert.IsTrue(empID > 0);

                cGlobalVariables.OutboundEmployeID = empID;

                cEmployee emp = clsEmps.GetEmployeeById(empID);

                Assert.IsNotNull(emp);

                #region Check employee record values

                //Make sure the username format is correct too
                Assert.AreEqual("10805521", emp.username);
                Assert.AreEqual("10805521", emp.payroll);
                Assert.AreEqual("Mrs.", emp.title);
                Assert.AreEqual("Scott", emp.surname);
                Assert.AreEqual("Debra", emp.firstname);
                Assert.AreEqual("Jane", emp.middlenames);
                Assert.AreEqual(new DateTime(1994, 08, 01), emp.hiredate);
                Assert.AreEqual(null, emp.terminationdate);
                Assert.AreEqual("61 Westwood Road", emp.address1);
                Assert.AreEqual("", emp.city);
                Assert.AreEqual("BB12 0HR", emp.postcode);
                Assert.AreEqual("GB", emp.country);
                Assert.AreEqual("editedTest@test.com", emp.email);
                Assert.IsTrue(emp.password != "");

                clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
                cEmployeeHomeLocation homeAddress = emp._HomeLocations.Values[0];

                //If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress);

                cCompanies clsComps = new cCompanies(cGlobalVariables.AccountID);

                cCompany comp = clsComps.GetCompanyById(homeAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the home address format
                Assert.AreEqual("10805521__Home", comp.company);

                #endregion

                #region Check assignment record values

                cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, empID);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                Assert.IsNotNull(assignment);

                Assert.AreEqual("10805521", assignment.assignmentnumber);
                Assert.AreEqual(new DateTime(1994, 08, 01), assignment.earliestassignmentstartdate);
                Assert.AreEqual(new DateTime(2010, 05, 01), assignment.finalassignmentenddate);
                Assert.AreEqual(ESRAssignmentStatus.ActiveAssignment, assignment.assignmentstatus);
                Assert.AreEqual("Church Street", assignment.assignmentaddress1);
                Assert.AreEqual("BB11 2DL", assignment.assignmentaddresspostcode);
                Assert.AreEqual(true, assignment.primaryassignment);
                Assert.AreEqual("638 96 St Peter's Centre", assignment.assignmentlocation);

                //This is an assignment value mapped to the employee table
                Assert.AreEqual("Nursing and Midwifery Registered|Community Nurse", emp.position);

                clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
                cEmployeeWorkLocation workAddress = emp._WorkLocations.Values[0];

                Assert.IsNotNull(workAddress);

                comp = clsComps.GetCompanyById(workAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the work address name is the assignment location
                Assert.AreEqual("638 96 St Peter's Centre", comp.company);

                #endregion

                cGlobalVariables.HomeAddressID = homeAddress.LocationID;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(""); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(""); // resets to default
            }
        }

        /// <summary>
        /// Import ESR Outbound file with home and office addresses already existing but not assigned to the employee
        /// </summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportOutboundWithHomeAndWorkAddressesExistingButNotAssignedTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int AccountID = cGlobalVariables.AccountID;
                int TemplateID = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

                cCompanies clsComps = new cCompanies(cGlobalVariables.AccountID);
                cGlobalCountries clsGlobalCountries = new cGlobalCountries();
                cGlobalCountry globCountry = clsGlobalCountries.getGlobalCountryByAlphaCode("GB");

                //Create home address
                clsComps.saveCompany(new cCompany(0, "10805521__Home", "", false, "", true, true, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, "61 Westwood Road", "Burnley", "", "", "BB12 0HR", globCountry.globalcountryid, 0, true, new SortedList<int, object>(), true, cCompany.AddressCreationMethod.ESROutboundAdded));

                //Create work address
                clsComps.saveCompany(new cCompany(0, "638 96 St Peter's Centre", "", false, "", true, true, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, "Church Street", "", "Burnley", "", "BB11 2DL", globCountry.globalcountryid, 0, true, new SortedList<int, object>(), false, cCompany.AddressCreationMethod.ESROutboundAdded));

                //Set the formats to make sure the values are correct when the import has ran
                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")");

                cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);
                target.importOutboundData(0);

                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);

                int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

                Assert.IsTrue(empID > 0);

                cGlobalVariables.OutboundEmployeID = empID;

                cEmployee emp = clsEmps.GetEmployeeById(empID);

                Assert.IsNotNull(emp);

                #region Check employee record values

                //Make sure the username format is correct too
                Assert.AreEqual("10805521", emp.username);
                Assert.AreEqual("10805521", emp.payroll);
                Assert.AreEqual("Mrs.", emp.title);
                Assert.AreEqual("Scott", emp.surname);
                Assert.AreEqual("Debra", emp.firstname);
                Assert.AreEqual("Jane", emp.middlenames);
                Assert.AreEqual(new DateTime(1994, 08, 01), emp.hiredate);
                Assert.AreEqual(null, emp.terminationdate);
                Assert.AreEqual("61 Westwood Road", emp.address1);
                Assert.AreEqual("", emp.city);
                Assert.AreEqual("BB12 0HR", emp.postcode);
                Assert.AreEqual("GB", emp.country);
                Assert.AreEqual("test@test.com", emp.email);
                Assert.IsTrue(emp.password != "");

                clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
                cEmployeeHomeLocation homeAddress = emp._HomeLocations.Values[0]; ;

                //If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress);

                clsComps = new cCompanies(cGlobalVariables.AccountID);

                cCompany comp = clsComps.GetCompanyById(homeAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the home address format
                Assert.AreEqual("10805521__Home", comp.company);

                #endregion

                #region Check assignment record values

                cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, empID);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                Assert.IsNotNull(assignment);

                Assert.AreEqual("10805521", assignment.assignmentnumber);
                Assert.AreEqual(new DateTime(1994, 08, 01), assignment.earliestassignmentstartdate);
                Assert.AreEqual(null, assignment.finalassignmentenddate);
                Assert.AreEqual(ESRAssignmentStatus.ActiveAssignment, assignment.assignmentstatus);
                Assert.AreEqual("Church Street", assignment.assignmentaddress1);
                Assert.AreEqual("BB11 2DL", assignment.assignmentaddresspostcode);
                Assert.AreEqual(true, assignment.primaryassignment);
                Assert.AreEqual("638 96 St Peter's Centre", assignment.assignmentlocation);

                //This is an assignment value mapped to the employee table
                Assert.AreEqual("Nursing and Midwifery Registered|Community Nurse", emp.position);

                clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
                cEmployeeWorkLocation workAddress = emp._WorkLocations.Values[0];

                Assert.IsNotNull(workAddress);

                comp = clsComps.GetCompanyById(workAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the work address name is the assignment location
                Assert.AreEqual("638 96 St Peter's Centre", comp.company);

                #endregion

                cGlobalVariables.HomeAddressID = homeAddress.LocationID;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(""); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(""); // resets to default
            }
        }

        /// <summary>
        /// Import outbound ESR file where the assignment location is set to nothing
        /// </summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportOutboundWithAssignmentLocationSetToNothingTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int AccountID = cGlobalVariables.AccountID;
                int TemplateID = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithNoAssignmentLocationSet();

                //Set the formats to make sure the values are correct when the import has ran
                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")");

                cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);
                target.importOutboundData(0);

                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

                Assert.IsTrue(empID > 0);

                cGlobalVariables.OutboundEmployeID = empID;

                cEmployee emp = clsEmps.GetEmployeeById(empID);

                Assert.IsNotNull(emp);

                #region Check employee record values

                //Make sure the username format is correct too
                Assert.AreEqual("10805521", emp.username);
                Assert.AreEqual("10805521", emp.payroll);
                Assert.AreEqual("Mrs.", emp.title);
                Assert.AreEqual("Scott", emp.surname);
                Assert.AreEqual("Debra", emp.firstname);
                Assert.AreEqual("Jane", emp.middlenames);
                Assert.AreEqual(new DateTime(1994, 08, 01), emp.hiredate);
                Assert.AreEqual(null, emp.terminationdate);
                Assert.AreEqual("61 Westwood Road", emp.address1);
                Assert.AreEqual("", emp.city);
                Assert.AreEqual("BB12 0HR", emp.postcode);
                Assert.AreEqual("GB", emp.country);
                Assert.AreEqual("test@test.com", emp.email);

                clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
                cEmployeeHomeLocation homeAddress = emp._HomeLocations.Values[0]; ;

                //If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress);

                cCompanies clsComps = new cCompanies(cGlobalVariables.AccountID);

                cCompany comp = clsComps.GetCompanyById(homeAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the home address format
                Assert.AreEqual("10805521__Home", comp.company);

                #endregion

                #region Check assignment record values

                cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, empID);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                Assert.IsNotNull(assignment);

                Assert.AreEqual("10805521", assignment.assignmentnumber);
                Assert.AreEqual(new DateTime(1994, 08, 01), assignment.earliestassignmentstartdate);
                Assert.AreEqual(null, assignment.finalassignmentenddate);
                Assert.AreEqual(ESRAssignmentStatus.ActiveAssignment, assignment.assignmentstatus);
                Assert.AreEqual("Church Street", assignment.assignmentaddress1);
                Assert.AreEqual("BB11 2DL", assignment.assignmentaddresspostcode);
                Assert.AreEqual(true, assignment.primaryassignment);
                Assert.AreEqual("", assignment.assignmentlocation);

                //This is an assignment value mapped to the employee table
                Assert.AreEqual("Nursing and Midwifery Registered|Community Nurse", emp.position);

                clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
                cEmployeeWorkLocation workAddress = emp._WorkLocations.Values[0];

                Assert.IsNotNull(workAddress);

                comp = clsComps.GetCompanyById(workAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the work address name is not the assignment location (as this is blank), should default to address 1st line and postcode
                Assert.AreEqual("Church StreetBB11 2DL", comp.company);

                #endregion

                cGlobalVariables.HomeAddressID = homeAddress.LocationID;
                cGlobalVariables.WorkAddressID = workAddress.LocationID;
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(""); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(""); // resets to default
            }
        }

        /// <summary>
        /// Import outbound ESR file where the assignment number is set to nothing but there is assignment
        /// information mapped to the employee record.
        /// This should still update the employee with the assignment information  
        /// </summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportOutboundWithAssignmentSetToNothingTest()
        {
            try
            {
                cImportTemplateObject.CreateImportTemplate();

                int AccountID = cGlobalVariables.AccountID;
                int TemplateID = cGlobalVariables.TemplateID;
                byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithAssignmentNumberSetToNothing();

                //Set the formats to make sure the values are correct when the import has ran
                cGlobalPropertiesObject.UpdateUsernameFormat("[Employee Number]");
                cGlobalPropertiesObject.UpdateHomeAddressFormat("CONCATENATE(\"[Employee Number]_[Employee's Address Town]_Home\")");

                cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);
                target.importOutboundData(0);

                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

                Assert.IsTrue(empID > 0);

                cGlobalVariables.OutboundEmployeID = empID;

                cEmployee emp = clsEmps.GetEmployeeById(empID);

                Assert.IsNotNull(emp);
                #region Check employee record values

                //Make sure the username format is correct too
                Assert.AreEqual("10805521", emp.username);
                Assert.AreEqual("10805521", emp.payroll);
                Assert.AreEqual("Mrs.", emp.title);
                Assert.AreEqual("Scott", emp.surname);
                Assert.AreEqual("Debra", emp.firstname);
                Assert.AreEqual("Jane", emp.middlenames);
                Assert.AreEqual(new DateTime(1994, 08, 01), emp.hiredate);
                Assert.AreEqual(null, emp.terminationdate);
                Assert.AreEqual("61 Westwood Road", emp.address1);
                Assert.AreEqual("", emp.city);
                Assert.AreEqual("BB12 0HR", emp.postcode);
                Assert.AreEqual("GB", emp.country);
                Assert.AreEqual("test@test.com", emp.email);
                Assert.IsTrue(emp.password != "");

                clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
                cEmployeeHomeLocation homeAddress = emp._HomeLocations.Values[0]; ;

                //If ok then the import has associated the new address to the employees home
                Assert.IsNotNull(homeAddress);

                cCompanies clsComps = new cCompanies(cGlobalVariables.AccountID);

                cCompany comp = clsComps.GetCompanyById(homeAddress.LocationID);

                Assert.IsNotNull(comp);

                //Check the home address format
                Assert.AreEqual("10805521__Home", comp.company);

                #endregion

                #region Check assignment record values

                cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, empID);
                cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

                Assert.IsNull(assignment);

                //This is an assignment value mapped to the employee table
                Assert.AreEqual("Nursing and Midwifery Registered|Community Nurse", emp.position);

                #endregion

                cGlobalVariables.HomeAddressID = homeAddress.LocationID;
            }
            finally
            {
                cGlobalPropertiesObject.UpdateUsernameFormat(""); // resets to default
                cGlobalPropertiesObject.UpdateHomeAddressFormat(""); // resets to default
            }
        }

        /// <summary>
        /// Import ESR Outbound file with an employee having home and work addresses already existing 
        /// but the file has new home and office addresses
        /// </summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportEditedOutboundWithNewHomeAndWorkAddressesTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int AccountID = cGlobalVariables.AccountID;
            int TemplateID = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo();

            cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);
            target.importOutboundData(0);

            //Edit the employee and assignment
            arrFileData = cImportTemplateObject.EditedDummyESROutboundFileInfoWithNewHomeAndWorkAddresses();
            target = new cESRImport(AccountID, TemplateID, arrFileData);
            target.importOutboundData(0);

            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

            Assert.IsTrue(empID > 0);

            cGlobalVariables.OutboundEmployeID = empID;

            cEmployee emp = clsEmps.GetEmployeeById(empID);

            Assert.IsNotNull(emp);

            #region Check employee record values

            //Make sure the username format is correct too
            Assert.AreEqual("Debra.Scott", emp.username);
            Assert.AreEqual("10805521", emp.payroll);
            Assert.AreEqual("Mrs.", emp.title);
            Assert.AreEqual("Scott", emp.surname);
            Assert.AreEqual("Debra", emp.firstname);
            Assert.AreEqual("Jane", emp.middlenames);
            Assert.AreEqual(new DateTime(1994, 08, 01), emp.hiredate);
            Assert.AreEqual(null, emp.terminationdate);
            Assert.AreEqual("1 Home Test Lane", emp.address1);
            Assert.AreEqual("TestTown", emp.city);
            Assert.AreEqual("NG35 4TY", emp.postcode);
            Assert.AreEqual("GB", emp.country);
            Assert.AreEqual("editedTest@test.com", emp.email);
            Assert.IsTrue(emp.password != "");

            clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
            cEmployeeHomeLocation homeAddress = emp._HomeLocations.Values[1]; ;

            //If ok then the import has associated the new address to the employees home
            Assert.IsNotNull(homeAddress);

            cCompanies clsComps = new cCompanies(cGlobalVariables.AccountID);

            cCompany comp = clsComps.GetCompanyById(homeAddress.LocationID);

            Assert.IsNotNull(comp);

            //Check the home address format
            Assert.AreEqual("1 Home Test LaneNG35 4TY", comp.company);

            #endregion

            #region Check assignment record values

            cESRAssignments clsAssign = new cESRAssignments(cGlobalVariables.AccountID, empID);
            cESRAssignment assignment = clsAssign.getAssignmentByAssignmentNumber("10805521");

            Assert.IsNotNull(assignment);

            Assert.AreEqual("10805521", assignment.assignmentnumber);
            Assert.AreEqual(new DateTime(1994, 08, 01), assignment.earliestassignmentstartdate);
            Assert.AreEqual(new DateTime(2010, 05, 01), assignment.finalassignmentenddate);
            Assert.AreEqual(ESRAssignmentStatus.ActiveAssignment, assignment.assignmentstatus);
            Assert.AreEqual("1 Work Test lane", assignment.assignmentaddress1);
            Assert.AreEqual("NG12 8YU", assignment.assignmentaddresspostcode);
            Assert.AreEqual(true, assignment.primaryassignment);
            Assert.AreEqual("Unit Test Work", assignment.assignmentlocation);

            //This is an assignment value mapped to the employee table
            Assert.AreEqual("Nursing and Midwifery Registered|Community Nurse", emp.position);

            clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
            cEmployeeWorkLocation workAddress = emp._WorkLocations.Values[1];

            Assert.IsNotNull(workAddress);


            comp = clsComps.GetCompanyById(workAddress.LocationID);

            Assert.IsNotNull(comp);

            //Check the work address name is the assignment location
            Assert.AreEqual("Unit Test Work", comp.company);

            #endregion

            clsComps.deleteCompany(homeAddress.LocationID);
            clsComps.deleteCompany(workAddress.LocationID);

            clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
            clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
            cGlobalVariables.HomeAddressID = emp._HomeLocations.Values[0].LocationID;
            cGlobalVariables.WorkAddressID = emp._WorkLocations.Values[0].LocationID;
        }

        /// <summary>
        /// Import an ESR Outbound file where an employee line manager is being set when the employee line manager is added
        /// after the employee associated
        /// </summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportOutboundWithLineManagerNotPreExistingTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int AccountID = cGlobalVariables.AccountID;
            int TemplateID = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithLineManagerNotPreExisting();

            cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);
            target.importOutboundData(0);

            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

            Assert.IsTrue(empID > 0);

            int LineManagerEmpID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "21272386");

            Assert.IsTrue(LineManagerEmpID > 0);

            cGlobalVariables.OutboundEmployeID = empID;
            cGlobalVariables.OutboundLinemanagerEmployeeID = LineManagerEmpID;

            cEmployee emp = clsEmps.GetEmployeeById(empID);

            Assert.IsNotNull(emp);

            Assert.AreEqual(emp.linemanager, LineManagerEmpID);

            clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
            clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
            cGlobalVariables.HomeAddressID = emp._HomeLocations.Values[0].LocationID;
            cGlobalVariables.WorkAddressID = emp._WorkLocations.Values[0].LocationID;
        }

        /// <summary>
        /// Import an ESR Outbound file where an employee line manager is being set when the employee line manager is added
        /// after the employee associated
        /// </summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void ImportOutboundWithLineManagerPreExistingTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int AccountID = cGlobalVariables.AccountID;
            int TemplateID = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfoWithLineManagerPreExisting();

            cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);
            target.importOutboundData(0);

            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "10805521");

            Assert.IsTrue(empID > 0);

            int LineManagerEmpID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "21272386");

            Assert.IsTrue(LineManagerEmpID > 0);

            cGlobalVariables.OutboundEmployeID = empID;
            cGlobalVariables.OutboundLinemanagerEmployeeID = LineManagerEmpID;

            cEmployee emp = clsEmps.GetEmployeeById(empID);

            Assert.IsNotNull(emp);

            Assert.AreEqual(emp.linemanager, LineManagerEmpID);

            clsEmps.GetHomeLocations(emp.employeeid); // populate _HomeLocations
            clsEmps.GetWorkLocations(emp.employeeid); // populate _WorkLocations
            cGlobalVariables.HomeAddressID = emp._HomeLocations.Values[0].LocationID;
            cGlobalVariables.WorkAddressID = emp._WorkLocations.Values[0].LocationID;
        }

        /// <summary>
        ///A test for cESRImport Constructor
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cESRImportConstructorTest()
        {
            cImportTemplateObject.CreateImportTemplate();

            int AccountID = cGlobalVariables.AccountID;
            int TemplateID = cGlobalVariables.TemplateID;
            byte[] arrFileData = cImportTemplateObject.CreateDummyESROutboundFileInfo(); 
            cESRImport target = new cESRImport(AccountID, TemplateID, arrFileData);
            Assert.IsNotNull(target);
        }
    }
}
