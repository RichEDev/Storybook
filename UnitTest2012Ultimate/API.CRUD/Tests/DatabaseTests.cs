namespace UnitTest2012Ultimate.API.CRUD.Tests
{
    using System;
    using System.Collections.Generic;

    using EsrGo2FromNhsWcfLibrary;
    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Crud;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;

    using Spend_Management;

    using UnitTest2012Ultimate.API.CRUD.Objects;

    using Action = global::EsrGo2FromNhsWcfLibrary.Base.Action;

    /// <summary>
    /// The database tests.
    /// </summary>
    [TestClass]
    public class DatabaseTests
    {
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
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion


        #region Accounts

        /// <summary>
        /// Connect to global account test.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbConnectToGlobalAccount()
        {
            var database = new ApiDbConnection("metabase", GlobalTestVariables.AccountId);
            var result = database.ConnectionStringValid;
            Assert.IsTrue(result, "Connection string not valid so not able to read account data");
        }

        /// <summary>
        /// Connect to global account test.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbConnectToInvalidAccount()
        {
            var database = new ApiDbConnection("metabase", 99999999);
            var result = database.ConnectionStringValid;
            Assert.IsTrue(!result, "Connection string valid was expecting invalid");
        }

        #endregion

        #region Employee

        /// <summary>
        /// Create Employee Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEmployeeCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "employeeid", new List<object> { 1, 2 } },
                                 {
                                     "firstname",
                                     new List<object> { "firstname", "anothername" }
                                 }
                             };

            var employeeData = DataAccessFactory.CreateDataAccess<Employee>(ApiDataBaseMoq.Database(fields, typeof(Employee), 1));
            var expected = new List<Employee> { new Employee { employeeid = 1234, firstname = "firstname", Action = Action.Create } };
            var result = employeeData.Create(expected);
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all employees test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEmployeesGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "employeeid", new List<object> { 1, 2 } },
                                 {
                                     "firstname",
                                     new List<object> { "firstname", "anothername" }
                                 }
                             };

            var employeeData = DataAccessFactory.CreateDataAccess<Employee>(ApiDataBaseMoq.Database(fields, typeof(Employee)));
            var result = employeeData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single employee test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEmployeeGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var employeeData = DataAccessFactory.CreateDataAccess<Employee>(
                ApiDataBaseMoq.Database(fields, typeof(Employee)));
            var result = employeeData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single employee test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEmployeeGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "employeeid", new List<object> { 1 } },
                                 { "username", new List<object> { "username" } },
                                 {
                                     "password",
                                     new List<object> { "vlUnCTgO1bFJXxfib/suGQ==" }
                                 },
                                 { "title", new List<object> { "Mr" } },
                                 { "firstname", new List<object> { "firstname" } },
                                 { "surname", new List<object> { "surname" } },
                                 { "mileagetotal", new List<object> { 1 } },
                                 { "email", new List<object> { "email@server.com" } },
                                 { "currefnum", new List<object> { 2 } },
                                 { "curclaimno", new List<object> { 3 } },
                                 { "payroll", new List<object> { "payroll" } },
                                 { "position", new List<object> { "position" } },
                                 { "telno", new List<object> { "telno" } },
                                 { "creditor", new List<object> { "creditor" } },
                                 { "archived", new List<object> { false } },
                                 { "groupid", new List<object> { 5 } },
                                 {
                                     "lastchange",
                                     new List<object> { new DateTime(2012, 12, 12) }
                                 },
                                 { "additems", new List<object> { 7 } },
                                 { "costcodeid", new List<object> { 8 } },
                                 { "departmentid", new List<object> { 8 } },
                                 { "extension", new List<object> { "extension" } },
                                 { "pagerno", new List<object> { "pagerno" } },
                                 { "mobileno", new List<object> { "mobileno" } },
                                 { "faxno", new List<object> { "faxno" } },
                                 { "homeemail", new List<object> { "homeemail" } },
                                 { "linemanager", new List<object> { 9 } },
                                 { "advancegroupid", new List<object> { 10 } },
                                 { "active", new List<object> { false } },
                                 { "primarycountry", new List<object> { 13 } },
                                 { "primarycurrency", new List<object> { 14 } },
                                 { "verified", new List<object> { false } },
                                 {
                                     "licenceexpiry",
                                     new List<object> { new DateTime(2012, 12, 13) }
                                 },
                                 {
                                     "licencelastchecked",
                                     new List<object> { new DateTime(2012, 12, 14) }
                                 },
                                 { "licencecheckedby", new List<object> { 15 } },
                                 {
                                     "licencenumber", new List<object> { "licencenumber" }
                                 },
                                 { "groupidcc", new List<object> { 16 } },
                                 { "groupidpc", new List<object> { 17 } },
                                 {
                                     "CreatedOn",
                                     new List<object> { new DateTime(2012, 12, 15) }
                                 },
                                 { "CreatedBy", new List<object> { 18 } },
                                 {
                                     "ModifiedOn",
                                     new List<object> { new DateTime(2012, 12, 16) }
                                 },
                                 { "ModifiedBy", new List<object> { 19 } },
                                 { "country", new List<object> { "country" } },
                                 { "ninumber", new List<object> { "ninumber" } },
                                 { "maidenname", new List<object> { "maidenname" } },
                                 { "middlenames", new List<object> { "middlenames" } },
                                 { "gender", new List<object> { "gender" } },
                                 {
                                     "dateofbirth",
                                     new List<object> { new DateTime(2012, 12, 17) }
                                 },
                                 {
                                     "hiredate",
                                     new List<object> { new DateTime(2012, 12, 18) }
                                 },
                                 {
                                     "terminationdate",
                                     new List<object> { new DateTime(2012, 12, 19) }
                                 },
                                 { "homelocationid", new List<object> { 20 } },
                                 { "officelocationid", new List<object> { 21 } },
                                 { "name", new List<object> { "name" } },
                                 {
                                     "accountnumber", new List<object> { "accountnumber" }
                                 },
                                 { "accounttype", new List<object> { "accounttype" } },
                                 { "sortcode", new List<object> { "sortcode" } },
                                 { "reference", new List<object> { "reference" } },
                                 { "localeID", new List<object> { 22 } },
                                 { "NHSTrustID", new List<object> { 23 } },
                                 { "logonCount", new List<object> { 24 } },
                                 { "retryCount", new List<object> { 25 } },
                                 { "firstLogon", new List<object> { false } },
                                 { "licenceAttachID", new List<object> { 26 } },
                                 { "defaultSubAccountId", new List<object> { 27 } },
                                 { "CreationMethod", new List<object> { '4' } },
                                 {
                                     "mileagetotaldate",
                                     new List<object> { new DateTime(2012, 12, 21) }
                                 },
                                 { "adminonly", new List<object> { false } },
                                 { "locked", new List<object> { true } },
                             };

            var employeeData = DataAccessFactory.CreateDataAccess<Employee>(ApiDataBaseMoq.Database(fields, typeof(Employee)));
            var result = employeeData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        /// <summary>
        /// Update employee test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEmployeesUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "employeeid", new List<object> { 1, 2 } }, 
                                 {
                                     "firstname", 
                                     new List<object> { "Updated", "anothername" }
                                 },
                                 { "reference", new List<object>{ "a ref", "a" }}
                             };

            var employeeData = DataAccessFactory.CreateDataAccess<Employee>(ApiDataBaseMoq.Database(fields, typeof(Employee), 1));
            var employee = new List<Employee>
                               {
                                   new Employee
                                       {
                                           employeeid = 1, 
                                           firstname = "Updated", 
                                           reference = "a ref", 
                                           Action = Action.Create
                                       }
                               };

            var result = employeeData.Update(employee);
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update employee test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEmployeesDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "employeeid", new List<object> { 1, 2 } },
                                 {
                                     "firstname",
                                     new List<object> { "firstname", "anothername" }
                                 }
                             };

            var employeeData = DataAccessFactory.CreateDataAccess<Employee>(ApiDataBaseMoq.Database(fields, typeof(Employee), 1));

            var result = employeeData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrPerson

        /// <summary>
        /// Create ESR Person Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPersonCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRPersonId", new List<object> { 1, 2 } },
                                 { "FirstName", new List<object> { "firstname", "another" } },
                                 { "MiddleNames", new List<object> { "middlename", "another" } },
                                 { "LastName", new List<object> { "lastname", "more" } },
                             };

            var esrPersonData = DataAccessFactory.CreateDataAccess<EsrPerson>(ApiDataBaseMoq.Database(fields, typeof(EsrPerson), 1));
            var expected = new List<EsrPerson> { new EsrPerson { ESRPersonId = 1, FirstName = "firstname", MiddleNames = "middlename", LastName = "lastname", Action = Action.Create } };
            List<EsrPerson> result = esrPersonData.Create(expected);
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR Persons test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPersonGet()
        {
            var fields = new SortedList<string, List<object>>
                            {
                                 { "ESRPersonId", new List<object> { 1L, 2L } },
                                 { "FirstName", new List<object> { "firstname", "another" } },
                                 { "MiddleNames", new List<object> { "middlename", "another" } },
                                 { "LastName", new List<object> { "lastname", "more" } },
                             };

            var esrPersonData = DataAccessFactory.CreateDataAccess<EsrPerson>(ApiDataBaseMoq.Database(fields, typeof(EsrPerson)));
            var result = esrPersonData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR Person test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrPersonGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var esrPersonData = DataAccessFactory.CreateDataAccess<EsrPerson>(ApiDataBaseMoq.Database(fields, typeof(EsrPerson)));
            var result = esrPersonData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            Helper.CheckFields(result, fields);
        }

        /// <summary>
        /// Get single ESR Person test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrPersonGetSingle()
        {
            var fields = new SortedList<string, List<object>>
{
                                 { "ESRPersonId", new List<object> { 1L, 2L } },
                                 { "FirstName", new List<object> { "firstname", "another"} },
                                 { "MiddleNames", new List<object> { "middlename", "another" } },
                                 { "LastName", new List<object> { "lastname", "more" } },
                             };
            var esrPersonData = DataAccessFactory.CreateDataAccess<EsrPerson>(ApiDataBaseMoq.Database(fields, typeof(EsrPerson)));
            var result = esrPersonData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR Person test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPersonsUpdate()
        {
            var fields = new SortedList<string, List<object>>
{
                                 { "ESRPersonId", new List<object> { 1L, 2L } },
                                 { "FirstName", new List<object> { "Updated", "another"} },
                                 { "MiddleNames", new List<object> { "middlename", "another" } },
                                 { "LastName", new List<object> { "lastname", "more" } },
                             };
            var esrPersonData = DataAccessFactory.CreateDataAccess<EsrPerson>(ApiDataBaseMoq.Database(fields, typeof(EsrPerson), 1));
            var esrPerson = new List<EsrPerson> { new EsrPerson { FirstName = "Updated", Action = Action.Create, LastName = "lastname", MiddleNames = "middlename", ESRPersonId = 1 } };

            var result = esrPersonData.Update(esrPerson);
            Helper.CheckFields(result[0], fields, 0);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR Person test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPersonsDelete()
        {
            var fields = new SortedList<string, List<object>>
{
                                 { "ESRPersonId", new List<object> { 1L } },
                                 { "FirstName", new List<object> { "firstname" } },
                                 { "MiddleNames", new List<object> { "middlename" } },
                                 { "LastName", new List<object> { "lastname" } },
                             };
            var esrPersonData = DataAccessFactory.CreateDataAccess<EsrPerson>(ApiDataBaseMoq.Database(fields, typeof(EsrPerson), 1));
            var result = esrPersonData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrVehicle

        /// <summary>
        /// Create ESR Vehicle Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrVehicleCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRVehicleAllocationId", new List<object> { 1L, 2L } },
                                 { "ESRPersonId", new List<object> { 3L, 4L } },
                                 { "ESRAssignmentId", new List<object> { 5L, 6L } },
                                 { "RegistrationNumber", new List<object> { "abc123", "123abc" } },
                                 { "Make", new List<object> { "make1", "make2" } },
                                 { "Model", new List<object> { "make1", "make2" } },
                                 { "Ownership", new List<object> { "make1", "make2" } },
                                 { "EngineCC", new List<object> { 1900, 1800 } }
                             };

            var esrVehicleData = DataAccessFactory.CreateDataAccess<EsrVehicle>(ApiDataBaseMoq.Database(fields, typeof(EsrVehicle), 1));
            var expected = new List<EsrVehicle>
                               {
                                   new EsrVehicle
                                       {
                                           ESRVehicleAllocationId = 1,
                                           ESRPersonId = 3,
                                           ESRAssignmentId = 5,
                                           RegistrationNumber = "abc123",
                                           Make = "make1",
                                           Model = "make1",
                                           Ownership = "make1",
                                           EngineCC = 1900, Action = Action.Create
                                       }
                               };
            var result = esrVehicleData.Create(expected);
            Helper.CheckFields(result[0], fields);

            // Expect Failure here as the Mock will not allow the our parameter to be set with the current version.
            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR Vehicles test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrVehiclesGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                { "ESRVehicleAllocationId", new List<object> { 1L, 2L } },
                                 { "ESRPersonId", new List<object> { 3L, 4L } },
                                 { "ESRAssignmentId", new List<object> { 5L, 6L } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "RegistrationNumber", new List<object> { "abc123", "123abc" } },
                                 { "Make", new List<object> { "make1", "make2" } },
                                 { "Model", new List<object> { "make1", "make2" } },
                                 { "Ownership", new List<object> { "make1", "make2" } },
                                 { "InitialRegistrationDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EngineCC", new List<object> { 1900, 1800 } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) }
                                 }
                             };

            var esrVehicleData = DataAccessFactory.CreateDataAccess<EsrVehicle>(ApiDataBaseMoq.Database(fields, typeof(EsrVehicle)));
            var result = esrVehicleData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESRVehicle test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrVehicleGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var dataAccess = DataAccessFactory.CreateDataAccess<EsrVehicle>(ApiDataBaseMoq.Database(fields, typeof(EsrVehicle)));
            var result = dataAccess.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Get single ESRVehicle test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrVehicleGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRVehicleAllocationId", new List<object> { 1L } },
                                 { "ESRPersonId", new List<object> { 3L } },
                                 { "ESRAssignmentId", new List<object> { 5L } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "RegistrationNumber", new List<object> { "abc123" } },
                                 { "Make", new List<object> { "make1" } },
                                 { "Model", new List<object> { "make1" } },
                                 { "Ownership", new List<object> { "make1" } },
                                 { "InitialRegistrationDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EngineCC", new List<object> { 1900 } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 12) }
                                 }
                             };

            var esrVehicleData = DataAccessFactory.CreateDataAccess<EsrVehicle>(ApiDataBaseMoq.Database(fields, typeof(EsrVehicle)));
            var result = esrVehicleData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR Person test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrVehiclesUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                { "ESRVehicleAllocationId", new List<object> { 1L, 2L } },
                                 { "ESRPersonId", new List<object> { 3L, 4L } },
                                 { "ESRAssignmentId", new List<object> { 5L, 6L } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "RegistrationNumber", new List<object> { "abc123", "123abc" } },
                                 { "Make", new List<object> { "make1", "make2" } },
                                 { "Model", new List<object> { "model", "make2" } },
                                 { "Ownership", new List<object> { "ownership", "make2" } },
                                 { "InitialRegistrationDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EngineCC", new List<object> { 1900, 1800 } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) }
                                 }
                             };

            var esrVehicleData = DataAccessFactory.CreateDataAccess<EsrVehicle>(ApiDataBaseMoq.Database(fields, typeof(EsrVehicle), 1));
            var esrVehicle = new List<EsrVehicle>
                                 {
                                     new EsrVehicle
                                         {
                                             ESRVehicleAllocationId = 1,
                                             ESRPersonId = 3,
                                             Make = "make1",
                                             Model = "model",
                                             Ownership = "ownership",
                                             EngineCC = 1900, Action = Action.Create,
                                             RegistrationNumber = "abc123",
                                             InitialRegistrationDate = new DateTime(2012, 12, 12),
                                             ESRLastUpdate = new DateTime(2012, 12, 12),
                                             ESRAssignmentId = 5,
                                             EffectiveEndDate = new DateTime(2012, 12, 12),
                                             EffectiveStartDate = new DateTime(2012, 12, 12)
                                         }
                                 };

            var result = esrVehicleData.Update(esrVehicle);
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR Person test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrVehiclesDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                { "ESRVehicleAllocationId", new List<object> { 1L } },
                                 { "ESRPersonId", new List<object> { 3L } },
                                 { "ESRAssignmentId", new List<object> { 5L } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "RegistrationNumber", new List<object> { "abc123" } },
                                 { "Make", new List<object> { "make1" } },
                                 { "Model", new List<object> { "make1" } },
                                 { "Ownership", new List<object> { "make1" } },
                                 { "InitialRegistrationDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EngineCC", new List<object> { 1900 } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 12) }
                                 }
                             };

            var esrVehicleData = DataAccessFactory.CreateDataAccess<EsrVehicle>(ApiDataBaseMoq.Database(fields, typeof(EsrVehicle), 1));

            var result = esrVehicleData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrPhone

        /// <summary>
        /// Create EsrPhone Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPhoneCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRPhoneId", new List<object> { 1L, 2L } },
                                 { "ESRPersonId", new List<object> { 10L, 11L } },
                                 { "PhoneType", new List<object> { "mobile", "home" } },
                                 { "PhoneNumber", new List<object> { "1234", "4321" } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
                             };

            var esrPhoneData = DataAccessFactory.CreateDataAccess<EsrPhone>(ApiDataBaseMoq.Database(fields, typeof(EsrPhone), 1));
            var expected = new List<EsrPhone>
                               {
                                   new EsrPhone
                                       {
                                           ESRPhoneId = 1,
                                           ESRPersonId = 10,
                                           PhoneType = "mobile",
                                           PhoneNumber = "1234",
                                           EffectiveStartDate = new DateTime(2012, 12, 12),
                                           EffectiveEndDate = new DateTime(2012, 12, 13),
                                           ESRLastUpdate = new DateTime(2012, 12, 14), Action = Action.Create
                                       }
                               };
            var result = esrPhoneData.Create(expected);
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR Persons test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPhonesGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                  { "ESRPhoneId", new List<object> { 1L, 2L } },
                                 { "ESRPersonId", new List<object> { 10L, 11L } },
                                 { "PhoneType", new List<object> { "mobile", "home" } },
                                 { "PhoneNumber", new List<object> { "1234", "4321" } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
                             };

            var esrPhoneData = DataAccessFactory.CreateDataAccess<EsrPhone>(ApiDataBaseMoq.Database(fields, typeof(EsrPhone)));
            var result = esrPhoneData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single EsrPhone test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrPhoneGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EsrPhoneData = DataAccessFactory.CreateDataAccess<EsrPhone>(
                ApiDataBaseMoq.Database(fields, typeof(EsrPhone)));
            var result = EsrPhoneData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR Phone test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrPhoneGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                  { "ESRPhoneId", new List<object> { 1L } },
                                 { "ESRPersonId", new List<object> { 10L } },
                                 { "PhoneType", new List<object> { "mobile" } },
                                 { "PhoneNumber", new List<object> { "1234" } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14) } }
                             };

            var esrPhoneData = DataAccessFactory.CreateDataAccess<EsrPhone>(
                ApiDataBaseMoq.Database(fields, typeof(EsrPhone)));
            var result = esrPhoneData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR Person test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPhonesUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                { "ESRPhoneId", new List<object> { 1, 2 } },
                                 { "ESRPersonId", new List<object> { 10, 11 } },
                                 { "PhoneType", new List<object> { "mobile", "home" } },
                                 { "PhoneNumber", new List<object> { "1234", "4321" } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
                             };

            var esrPhoneData = DataAccessFactory.CreateDataAccess<EsrPhone>(ApiDataBaseMoq.Database(fields, typeof(EsrPhone), 1));
            var esrPhone = new List<EsrPhone>
                               {
                                   new EsrPhone
                                       {
                                           ESRPhoneId = 1,
                                           ESRPersonId = 10,
                                           PhoneType = "mobile",
                                           PhoneNumber = "1234", Action = Action.Create,
                                           EffectiveEndDate = new DateTime(2012, 12, 13),
                                           EffectiveStartDate = new DateTime(2012, 12, 12),
                                           ESRLastUpdate = new DateTime(2012, 12, 14)
                                       }
                               };

            var result = esrPhoneData.Update(esrPhone);
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR Person test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPhonesDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRPhoneId", new List<object> { 1L } },
                                 { "ESRPersonId", new List<object> { 10L } },
                                 { "PhoneType", new List<object> { "mobile" } },
                                 { "PhoneNumber", new List<object> { "1234"} },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14) } }
                             };

            var esrPhoneData = DataAccessFactory.CreateDataAccess<EsrPhone>(ApiDataBaseMoq.Database(fields, typeof(EsrPhone), 1));

            var result = esrPhoneData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrAssignment

        /// <summary>
        /// Create ESR Assignment Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "esrAssignID", new List<object> { 1, 2 } },
                                 {
                                     "PayrollName",
                                     new List<object> { "PayrollName", "another name" }
                                 }
                             };

            var esrAssignmentData = DataAccessFactory.CreateDataAccess<EsrAssignment>(ApiDataBaseMoq.Database(fields, typeof(EsrAssignment), 1));
            var expected = new List<EsrAssignment> { new EsrAssignment { ESRPersonId = 1, esrAssignID = 1, PayrollName = "PayrollName", Action = Action.Create } };
            var result = esrAssignmentData.Create(expected);
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR Persons test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentsGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                
                                 { "esrAssignID", new List<object> { 1, 2 } },
                                 {
                                     "PayrollName",
                                     new List<object> { "Payrollname", "another name" }
                                 }
                             };

            var EsrAssignmentData = DataAccessFactory.CreateDataAccess<EsrAssignment>(ApiDataBaseMoq.Database(fields, typeof(EsrAssignment)));
            var result = EsrAssignmentData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR Assignment test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EsrAssignmentData = DataAccessFactory.CreateDataAccess<EsrAssignment>(
                ApiDataBaseMoq.Database(fields, typeof(EsrAssignment)));
            var result = EsrAssignmentData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR Assignment test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "esrAssignID", new List<object> { 1 } },
                                 {
                                     "PayrollName",
                                     new List<object> { "Payrollname" }
                                 }
                             };

            var esrAssignmentData = DataAccessFactory.CreateDataAccess<EsrAssignment>(
                ApiDataBaseMoq.Database(fields, typeof(EsrAssignment)));
            var result = esrAssignmentData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be success , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR Person test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentsUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                { "esrAssignID", new List<object> { 1, 2 } },
                                 {
                                     "PayrollName",
                                     new List<object> { "Payrollname", "other name"}
                                 }
                             };

            var esrAssignmentData = DataAccessFactory.CreateDataAccess<EsrAssignment>(ApiDataBaseMoq.Database(fields, typeof(EsrAssignment), 1));
            var esrAssignment = new EsrAssignment { Action = Action.Create, esrAssignID = 1, PayrollName = "Payrollname"};

            var result = esrAssignmentData.Update(new List<EsrAssignment> { esrAssignment });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR Assignment test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentsDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                { "esrAssignID", new List<object> { 1 } },
                                 {
                                     "PayrollName",
                                     new List<object> { "Payrollname" }
                                 }
                             };

            var esrAssignmentData = DataAccessFactory.CreateDataAccess<EsrAssignment>(ApiDataBaseMoq.Database(fields, typeof(EsrAssignment), 1));
            var result = esrAssignmentData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrAddress

        /// <summary>
        /// Create EsrAddress Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAddressCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRAddressId", new List<object> { 1, 2 } },
                                 { "ESRPersonId", new List<object> { 10, 11 } },
                                 { "AddressType", new List<object> { "mobile", "home" } },
                                 { "AddressStyle", new List<object> { "1234", "4321" } },
                                 { "PrimaryFlag", new List<object> { "1234", "4321" } },
                                 { "AddressLine1", new List<object> { "1234", "4321" } },
                                 { "AddressLine2", new List<object> { "1234", "4321" } },
                                 { "AddressLine3", new List<object> { "1234", "4321" } },
                                 { "AddressTown", new List<object> { "1234", "4321" } },
                                 { "AddressCounty", new List<object> { "1234", "4321" } },
                                 { "AddressPostcode", new List<object> { "1234", "4321" } },
                                 { "AddressCountry", new List<object> { "1234", "4321" } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
                             };

            var esrAddressData = DataAccessFactory.CreateDataAccess<EsrAddress>(ApiDataBaseMoq.Database(fields, typeof(EsrAddress), 1));
            var expected = new EsrAddress
            {
                ESRAddressId = 1,
                ESRPersonId = 10,
                AddressType = "mobile",
                AddressStyle = "1234",
                PrimaryFlag = "1234",
                AddressLine1 = "1234",
                AddressLine2 = "1234",
                AddressLine3 = "1234",
                AddressTown = "1234",
                AddressCounty = "1234",
                AddressPostcode = "1234",
                EffectiveStartDate = new DateTime(2012, 12, 12),
                EffectiveEndDate = new DateTime(2012, 12, 13),
                ESRLastUpdate = new DateTime(2012, 12, 14),
                Action = Action.Create,
                AddressCountry = "1234"
            };
            var result = esrAddressData.Create(new List<EsrAddress> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR Persons test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAddresssGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                  { "ESRAddressId", new List<object> { 1L, 2L } },
                                 { "ESRPersonId", new List<object> { 10L, 11L } },
                                 { "AddressType", new List<object> { "mobile", "home" } },
                                 { "AddressStyle", new List<object> { "1234", "4321" } },
                                 { "PrimaryFlag", new List<object> { "1234", "4321" } },
                                 { "AddressLine1", new List<object> { "1234", "4321" } },
                                 { "AddressLine2", new List<object> { "1234", "4321" } },
                                 { "AddressLine3", new List<object> { "1234", "4321" } },
                                 { "AddressTown", new List<object> { "1234", "4321" } },
                                 { "AddressCounty", new List<object> { "1234", "4321" } },
                                 { "AddressPostcode", new List<object> { "1234", "4321" } },
                                 { "AddressCountry", new List<object> { "1234", "4321" } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
                             };

            var esrAddressData = DataAccessFactory.CreateDataAccess<EsrAddress>(ApiDataBaseMoq.Database(fields, typeof(EsrAddress)));
            var result = esrAddressData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single EsrAddress test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrAddressGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EsrAddressData = DataAccessFactory.CreateDataAccess<EsrAddress>(
                ApiDataBaseMoq.Database(fields, typeof(EsrAddress)));
            var result = EsrAddressData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR Address test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrAddressGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRAddressId", new List<object> { 1L } },
                                 { "ESRPersonId", new List<object> { 10L } },
                                 { "AddressType", new List<object> { "mobile" } },
                                 { "AddressStyle", new List<object> { "1234"} },
                                 { "PrimaryFlag", new List<object> { "1234"} },
                                 { "AddressLine1", new List<object> { "1234"} },
                                 { "AddressLine2", new List<object> { "1234"} },
                                 { "AddressLine3", new List<object> { "1234"} },
                                 { "AddressTown", new List<object> { "1234"} },
                                 { "AddressCounty", new List<object> { "1234"} },
                                 { "AddressPostcode", new List<object> { "1234"} },
                                 { "AddressCountry", new List<object> { "1234"} },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14) } }
                             };

            var esrAddressData = DataAccessFactory.CreateDataAccess<EsrAddress>(
                ApiDataBaseMoq.Database(fields, typeof(EsrAddress)));
            var result = esrAddressData.Read(1L);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR Person test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAddresssUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                { "ESRAddressId", new List<object> { 1, 2 } },
                                 { "ESRPersonId", new List<object> { 10, 11 } },
                                 { "AddressType", new List<object> { "mobile", "home" } },
                                 { "AddressStyle", new List<object> { "1234", "4321" } },
                                 { "PrimaryFlag", new List<object> { "1234", "4321" } },
                                 { "AddressLine1", new List<object> { "1234", "4321" } },
                                 { "AddressLine2", new List<object> { "1234", "4321" } },
                                 { "AddressLine3", new List<object> { "1234", "4321" } },
                                 { "AddressTown", new List<object> { "1234", "4321" } },
                                 { "AddressCounty", new List<object> { "1234", "4321" } },
                                 { "AddressPostcode", new List<object> { "1234", "4321" } },
                                 { "AddressCountry", new List<object> { "1234", "4321" } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
                             };

            var esrAddressData = DataAccessFactory.CreateDataAccess<EsrAddress>(ApiDataBaseMoq.Database(fields, typeof(EsrAddress), 1));
            var esrAddress = new EsrAddress
            {
                ESRAddressId = 1,
                ESRPersonId = 10,
                AddressType = "mobile",
                AddressStyle = "1234",
                PrimaryFlag = "1234",
                AddressLine1 = "1234",
                AddressLine2 = "1234",
                AddressLine3 = "1234",
                AddressTown = "1234",
                AddressCounty = "1234",
                AddressPostcode = "1234",
                EffectiveStartDate = new DateTime(2012, 12, 12),
                EffectiveEndDate = new DateTime(2012, 12, 13),
                ESRLastUpdate = new DateTime(2012, 12, 14),
                Action = Action.Create,
                AddressCountry = "1234"
            };
            var result = esrAddressData.Update(new List<EsrAddress> { esrAddress });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR Person test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAddresssDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRAddressId", new List<object> { 1L } },
                                 { "ESRPersonId", new List<object> { 10L } },
                                 { "AddressType", new List<object> { "mobile" } },
                                 { "AddressStyle", new List<object> { "1234"} },
                                 { "PrimaryFlag", new List<object> { "1234"} },
                                 { "AddressLine1", new List<object> { "1234"} },
                                 { "AddressLine2", new List<object> { "1234"} },
                                 { "AddressLine3", new List<object> { "1234"} },
                                 { "AddressTown", new List<object> { "1234"} },
                                 { "AddressCounty", new List<object> { "1234"} },
                                 { "AddressPostcode", new List<object> { "1234"} },
                                 { "AddressCountry", new List<object> { "1234"} },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14) } }
                             };

            var esrAddressData = DataAccessFactory.CreateDataAccess<EsrAddress>(ApiDataBaseMoq.Database(fields, typeof(EsrAddress), 1));

            var result = esrAddressData.Delete(1L);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrAssignmentCostings

        /// <summary>
        /// Create EsrAssignmentCostings Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentCostingsCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRCostingAllocationId", new List<object> { 1, 2 } },
                                 { "ESRPersonId", new List<object> { 10, 11 } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
                             };

            var esrAssignmentCostingsData = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings), 1));
            var expected = new EsrAssignmentCostings
                               {
                                   ESRCostingAllocationId = 1,
                                   ESRPersonId = 10,
                                   EffectiveStartDate = new DateTime(2012, 12, 12),
                                   EffectiveEndDate = new DateTime(2012, 12, 13),
                                   ESRLastUpdate = new DateTime(2012, 12, 14)
                                   ,
                                   Action = Action.Create
                               };
            var result = esrAssignmentCostingsData.Create(new List<EsrAssignmentCostings> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR AssignmentCostings test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentCostingssGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                  { "ESRCostingAllocationId", new List<object> { 1L, 2L } },
                                 { "ESRPersonId", new List<object> { 10L, 11L } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
                             };

            var esrAssignmentCostingsData = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings)));
            var result = esrAssignmentCostingsData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single EsrAssignmentCostings test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentCostingsGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EsrAssignmentCostingsData = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(
                ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings)));
            var result = EsrAssignmentCostingsData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR AssignmentCostings test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentCostingsGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                  { "ESRCostingAllocationId", new List<object> { 1L } },
                                 { "ESRPersonId", new List<object> { 10L } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14) } }
                             };

            var esrAssignmentCostingsData = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(
                ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings)));
            var result = esrAssignmentCostingsData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR AssignmentCostings test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentCostingssUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                { "ESRCostingAllocationId", new List<object> { 1, 2 } },
                                 { "ESRPersonId", new List<object> { 10, 11 } },
                             };

            var esrAssignmentCostingsData = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings), 1));
            var esrAssignmentCostings = new EsrAssignmentCostings { ESRCostingAllocationId = 1, ESRPersonId = 10, Action = Action.Create };

            var result = esrAssignmentCostingsData.Update(new List<EsrAssignmentCostings> { esrAssignmentCostings });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR AssignmentCostings test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrAssignmentCostingssDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRCostingAllocationId", new List<object> { 1L } },
                                 { "ESRPersonId", new List<object> { 10L } },
                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13) } },
                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14) } }
                             };

            var esrAssignmentCostingsData = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings), 1));

            var result = esrAssignmentCostingsData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        //#region EsrElements

        ///// <summary>
        ///// Create ESR ElementFields Test Mocked.
        ///// </summary>
        //[TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        //public void ApiDbEsrElementCreate()
        //{
        //    var fields = new SortedList<string, List<object>>
        //                     {
        //                         { "globalElementID", new List<object> { 1, 2 } },
        //                         { "elementID", new List<object> { 10, 11 } },
        //                     };

        //    var EsrElementData = DataAccessFactory.CreateDataAccess<EsrElement>(ApiDataBaseMoq.Database(fields, typeof(EsrElement), 10));
        //    var expected = new EsrElement
        //    {
        //        globalElementID = 1,
        //        elementID = 10,
        //        Action = Action.Create
        //    };
        //    var result = EsrElementData.Create(new List<EsrElement> { expected });
        //    Helper.CheckFields(result[0], fields);

        //    Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        //}

        ///// <summary>
        ///// Get all ESR ElementFields test Mocked
        ///// </summary>
        //[TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        //public void ApiDbEsrElementsGet()
        //{
        //    var fields = new SortedList<string, List<object>>
        //                     {
        //                         { "globalElementID", new List<object> { 1, 2 } },
        //                         { "elementID", new List<object> { 10, 11 } },
        //                     };

        //    var EsrElementData = DataAccessFactory.CreateDataAccess<EsrElement>(ApiDataBaseMoq.Database(fields, typeof(EsrElement)));
        //    var result = EsrElementData.ReadAll();
        //    Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
        //    Helper.CheckFields(result[0], fields, 0);
        //    Helper.CheckFields(result[1], fields, 1);
        //}

        ///// <summary>
        ///// Get single ESR ElementFields test Mocked
        ///// </summary>
        //[TestMethod]
        //[TestCategory("APICRUD")]
        //[TestCategory("ApiDatabase")]
        //public void ApiDbEsrElementGetSinglethatDoesNotExist()
        //{
        //    var fields = new SortedList<string, List<object>>();
        //    var EsrElementData = DataAccessFactory.CreateDataAccess<EsrElement>(
        //        ApiDataBaseMoq.Database(fields, typeof(EsrElement)));
        //    var result = EsrElementData.Read(2);
        //    Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
        //    if (result != null)
        //    {
        //        Helper.CheckFields(result, fields, 0);
        //    }
        //}

        ///// <summary>
        ///// Get single ESR EsrElement test Mocked
        ///// </summary>
        //[TestMethod]
        //[TestCategory("APICRUD")]
        //[TestCategory("ApiDatabase")]
        //public void ApiDbEsrElementGetSingle()
        //{
        //    var fields = new SortedList<string, List<object>>
        //                     {
        //                         { "globalElementID", new List<object> { 1, 2 } },
        //                         { "elementID", new List<object> { 10, 11 } },
        //                     };

        //    var EsrElementData = DataAccessFactory.CreateDataAccess<EsrElement>(
        //        ApiDataBaseMoq.Database(fields, typeof(EsrElement)));
        //    var result = EsrElementData.Read(1);
        //    Assert.IsTrue(
        //        result.ActionResult.Result == ApiActionResult.Success,
        //        string.Format("Result should be sucess , was actually {0}", result.ActionResult));
        //    Helper.CheckFields(result, fields, 0);
        //}

        ///// <summary>
        ///// Update ESR EsrElement test Mocked
        ///// </summary>
        //[TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        //public void ApiDbEsrElementsUpdate()
        //{
        //    var fields = new SortedList<string, List<object>>
        //                     {
        //                         { "globalElementID", new List<object> { 1, 2 } },
        //                         { "elementID", new List<object> { 10, 11 } },
        //                     };

        //    var EsrElementData = DataAccessFactory.CreateDataAccess<EsrElement>(ApiDataBaseMoq.Database(fields, typeof(EsrElement), 10));
        //    var EsrElement = new EsrElement { globalElementID = 1, elementID = 10, Action = Action.Create };

        //    var result = EsrElementData.Update(new List<EsrElement> { EsrElement });
        //    Helper.CheckFields(result[0], fields, 0);


        //    Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        //}

        ///// <summary>
        ///// Update ESR EsrElement test Mocked
        ///// </summary>
        //[TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        //public void ApiDbEsrElementsDelete()
        //{
        //    var fields = new SortedList<string, List<object>>
        //                     {
        //                         { "globalElementID", new List<object> { 1, 2 } },
        //                         { "elementID", new List<object> { 1, 11 } },
        //                     };

        //    var EsrElementData = DataAccessFactory.CreateDataAccess<EsrElement>(ApiDataBaseMoq.Database(fields, typeof(EsrElement), 1));

        //    var result = EsrElementData.Delete(1);

        //    Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult.Result));
        //    Helper.CheckFields(result, fields);
        //}

        //#endregion

        #region EsrTrusts

        /// <summary>
        /// Create ESR ElementFields Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrTrustCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "trustID", new List<object> { 1, 2 } },
                                 { "trustVPD", new List<object> { "10", "11" } }
                             };

            var EsrTrustData = DataAccessFactory.CreateDataAccess<EsrTrust>(ApiDataBaseMoq.Database(fields, typeof(EsrTrust), 1));
            var expected = new EsrTrust
            {
                trustID = 1,
                trustVPD = "10",
                Action = Action.Create
            };
            var result = EsrTrustData.Create(new List<EsrTrust> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrTrustsGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "trustID", new List<object> { 1, 2 } },
                                 { "trustVPD", new List<object> { "10", "11" } },
                             };

            var EsrTrustData = DataAccessFactory.CreateDataAccess<EsrTrust>(ApiDataBaseMoq.Database(fields, typeof(EsrTrust)));
            var result = EsrTrustData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR ElementFields test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrTrustGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EsrTrustData = DataAccessFactory.CreateDataAccess<EsrTrust>(
                ApiDataBaseMoq.Database(fields, typeof(EsrTrust)));
            var result = EsrTrustData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR EsrTrust test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrTrustGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "trustID", new List<object> { 1, 2 } },
                                 { "trustVPD", new List<object> { "10", "11" } }
                             };

            var EsrTrustData = DataAccessFactory.CreateDataAccess<EsrTrust>(
                ApiDataBaseMoq.Database(fields, typeof(EsrTrust)));
            var result = EsrTrustData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR EsrTrust test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrTrustsUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "trustID", new List<object> { 1, 2 } },
                                 { "trustVPD", new List<object> { "10", "11" } },
                             };

            var EsrTrustData = DataAccessFactory.CreateDataAccess<EsrTrust>(ApiDataBaseMoq.Database(fields, typeof(EsrTrust), 1));
            var EsrTrust = new EsrTrust { trustVPD = "10", trustID = 1, Action = Action.Create };

            var result = EsrTrustData.Update(new List<EsrTrust> { EsrTrust });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR EsrTrust test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrTrustsDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "trustID", new List<object> { 1, 2 } },
                                 { "trustVPD", new List<object> { "10", "11" } },
                             };

            var EsrTrustData = DataAccessFactory.CreateDataAccess<EsrTrust>(ApiDataBaseMoq.Database(fields, typeof(EsrTrust), 1));

            var result = EsrTrustData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrLocations

        /// <summary>
        /// Create ESR ElementFields Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrLocationCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRLocationId", new List<object> { 1, 2 } },
                                 { "AddressLine1", new List<object> { "10", "11" } },
                             };

            var EsrLocationData = DataAccessFactory.CreateDataAccess<EsrLocation>(ApiDataBaseMoq.Database(fields, typeof(EsrLocation), 1));
            var expected = new EsrLocation
            {
                ESRLocationId = 1,
                AddressLine1 = "10",
                Action = Action.Create
            };
            var result = EsrLocationData.Create(new List<EsrLocation> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrLocationsGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRLocationId", new List<object> { 1L, 2L } },
                                 { "AddressLine1", new List<object> { "10", "11" } },
                             };

            var EsrLocationData = DataAccessFactory.CreateDataAccess<EsrLocation>(ApiDataBaseMoq.Database(fields, typeof(EsrLocation)));
            var result = EsrLocationData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR ElementFields test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrLocationGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EsrLocationData = DataAccessFactory.CreateDataAccess<EsrLocation>(
                ApiDataBaseMoq.Database(fields, typeof(EsrLocation)));
            var result = EsrLocationData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR EsrLocation test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrLocationGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRLocationId", new List<object> { 1L, 2L } },
                                 { "AddressLine1", new List<object> { "10", "11" } },
                             };

            var EsrLocationData = DataAccessFactory.CreateDataAccess<EsrLocation>(
                ApiDataBaseMoq.Database(fields, typeof(EsrLocation)));
            var result = EsrLocationData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR EsrLocation test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrLocationsUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRLocationId", new List<object> { 1, 2 } },
                                 { "AddressLine1", new List<object> { "10", "11" } },
                             };

            var EsrLocationData = DataAccessFactory.CreateDataAccess<EsrLocation>(ApiDataBaseMoq.Database(fields, typeof(EsrLocation), 1));
            var EsrLocation = new EsrLocation { ESRLocationId = 1, AddressLine1 = "10", Action = Action.Create };

            var result = EsrLocationData.Update(new List<EsrLocation> { EsrLocation });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR EsrLocation test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrLocationsDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRLocationId", new List<object> { 1, 2 } },
                                 { "AddressLine1", new List<object> { "10", "11" } },
                             };

            var EsrLocationData = DataAccessFactory.CreateDataAccess<EsrLocation>(ApiDataBaseMoq.Database(fields, typeof(EsrLocation), 1));

            var result = EsrLocationData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrOrganisations

        /// <summary>
        /// Create ESR ElementFields Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrOrganisationCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESROrganisationId", new List<object> { 1, 2 } },
                                 { "ESRLocationId", new List<object> { 2, 4 } }
                             };

            var EsrOrganisationData = DataAccessFactory.CreateDataAccess<EsrOrganisation>(ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation), 1));
            var expected = new EsrOrganisation
            {
                ESROrganisationId = 1,
                ESRLocationId = 2,
                Action = Action.Create
            };
            var result = EsrOrganisationData.Create(new List<EsrOrganisation> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrOrganisationsGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESROrganisationId", new List<object> { 1L, 2L } },
                                 { "ESRLocationId", new List<object> { 2L, 4L } }
                             };

            var EsrOrganisationData = DataAccessFactory.CreateDataAccess<EsrOrganisation>(ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation)));
            var result = EsrOrganisationData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR ElementFields test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrOrganisationGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EsrOrganisationData = DataAccessFactory.CreateDataAccess<EsrOrganisation>(
                ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation)));
            var result = EsrOrganisationData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR EsrOrganisation test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrOrganisationGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESROrganisationId", new List<object> { 1L, 2L } },
                                 { "ESRLocationId", new List<object> { 2L, 4L } }
                             };

            var EsrOrganisationData = DataAccessFactory.CreateDataAccess<EsrOrganisation>(
                ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation)));
            var result = EsrOrganisationData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR EsrOrganisation test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrOrganisationsUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESROrganisationId", new List<object> { 1, 2 } },
                                 { "ESRLocationId", new List<object> { 2, 4 } }
                             };

            var EsrOrganisationData = DataAccessFactory.CreateDataAccess<EsrOrganisation>(ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation), 1));
            var EsrOrganisation = new EsrOrganisation { ESROrganisationId = 1, ESRLocationId = 2, Action = Action.Create };

            var result = EsrOrganisationData.Update(new List<EsrOrganisation> { EsrOrganisation });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR EsrOrganisation test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrOrganisationsDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESROrganisationId", new List<object> { 1L, 2L } },
                                 { "ESRLocationId", new List<object> { 2L, 4L } }
                             };

            var EsrOrganisationData = DataAccessFactory.CreateDataAccess<EsrOrganisation>(ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation), 1));

            var result = EsrOrganisationData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrPositions

        /// <summary>
        /// Create ESR ElementFields Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPositionCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRPositionId", new List<object> { 1, 2 } },
                                 { "JobRole", new List<object> { "job", "desc" } }
                             };

            var EsrPositionData = DataAccessFactory.CreateDataAccess<EsrPosition>(ApiDataBaseMoq.Database(fields, typeof(EsrPosition), 1));
            var expected = new EsrPosition
            {
                ESRPositionId = 1,
                JobRole = "job",
                Action = Action.Create
            };
            var result = EsrPositionData.Create(new List<EsrPosition> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPositionsGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRPositionId", new List<object> { 1L, 2L } },
                                 { "JobRole", new List<object> { "job", "desc" } }
                             };

            var EsrPositionData = DataAccessFactory.CreateDataAccess<EsrPosition>(ApiDataBaseMoq.Database(fields, typeof(EsrPosition)));
            var result = EsrPositionData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR ElementFields test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrPositionGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EsrPositionData = DataAccessFactory.CreateDataAccess<EsrPosition>(
                ApiDataBaseMoq.Database(fields, typeof(EsrPosition)));
            var result = EsrPositionData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR EsrPosition test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrPositionGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRPositionId", new List<object> { 1L, 2L } },
                                 { "JobRole", new List<object> { "job", "desc" } }
                             };

            var EsrPositionData = DataAccessFactory.CreateDataAccess<EsrPosition>(
                ApiDataBaseMoq.Database(fields, typeof(EsrPosition)));
            var result = EsrPositionData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR EsrPosition test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPositionsUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRPositionId", new List<object> { 1, 2 } },
                                 { "JobRole", new List<object> { "job", "desc" } }
                             };

            var EsrPositionData = DataAccessFactory.CreateDataAccess<EsrPosition>(ApiDataBaseMoq.Database(fields, typeof(EsrPosition), 1));
            var EsrPosition = new EsrPosition { ESRPositionId = 1, JobRole = "job", Action = Action.Create };

            var result = EsrPositionData.Update(new List<EsrPosition> { EsrPosition });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR EsrPosition test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrPositionsDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "ESRPositionId", new List<object> { 1, 2 } },
                                 { "JobRole", new List<object> { "job", "desc" } }
                             };

            var EsrPositionData = DataAccessFactory.CreateDataAccess<EsrPosition>(ApiDataBaseMoq.Database(fields, typeof(EsrPosition), 1));

            var result = EsrPositionData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EsrElementSubCats

        /// <summary>
        /// Create ESR ElementFields Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrElementSubCatCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "elementSubcatID", new List<object> { 1, 2 } },
                                 { "subcatID", new List<object> { 2, 3 } },
                                 { "elementID", new List<object> { 2, 3 } }
                             };

            var EsrElementSubCatData = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat), 1));
            var expected = new EsrElementSubCat
            {
                elementSubcatID = 1,
                subcatID = 2,
                elementID = 2,
                Action = Action.Create
            };
            var result = EsrElementSubCatData.Create(new List<EsrElementSubCat> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrElementSubCatsGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "elementSubcatID", new List<object> { 1, 2 } },
                                 { "subcatID", new List<object> { 2, 3 } },
                                 { "elementID", new List<object> { 2, 3 } }
                             };

            var EsrElementSubCatData = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat)));
            var result = EsrElementSubCatData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR ElementFields test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrElementSubCatGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EsrElementSubCatData = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(
                ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat)));
            var result = EsrElementSubCatData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR EsrElementSubCat test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEsrElementSubCatGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "elementSubcatID", new List<object> { 1 } },
                                 { "subcatID", new List<object> { 2 } },
                                 { "elementID", new List<object> { 2 } }
                             };

            var EsrElementSubCatData = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(
                ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat)));
            var result = EsrElementSubCatData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR EsrElementSubCat test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrElementSubCatsUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "elementSubcatID", new List<object> { 1, 2 } },
                                 { "subcatID", new List<object> { 2, 3 } },
                                 { "elementID", new List<object> { 2, 3 } }
                             };

            var EsrElementSubCatData = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat), 1));
            var EsrElementSubCat = new EsrElementSubCat
            {
                elementSubcatID = 1,
                subcatID = 2,
                elementID = 2,
                Action = Action.Create
            };

            var result = EsrElementSubCatData.Update(new List<EsrElementSubCat> { EsrElementSubCat });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR EsrElementSubCat test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEsrElementSubCatsDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "elementSubcatID", new List<object> { 1, 2 } },
                                 { "subcatID", new List<object> { 2, 3 } },
                                 { "elementID", new List<object> { 2, 3 } }
                             };

            var EsrElementSubCatData = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat), 1));

            var result = EsrElementSubCatData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region Cars

        /// <summary>
        /// Create ESR ElementFields Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbCarCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "carid", new List<object> { 1, 2 } },
                                 { "employeeid", new List<object> { GlobalTestVariables.EmployeeId, GlobalTestVariables.EmployeeId } }
                             };

            var carData = DataAccessFactory.CreateDataAccess<Car>(ApiDataBaseMoq.Database(fields, typeof(Car), 1));
            var expected = new Car
            {
                carid = 1,
                employeeid = GlobalTestVariables.EmployeeId,
                Action = Action.Create
            };
            var result = carData.Create(new List<Car> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbCarsGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "carid", new List<object> { 1, 2 } },
                                 { "employeeid", new List<object> { GlobalTestVariables.EmployeeId, GlobalTestVariables.EmployeeId } }
                             };

            var CarData = DataAccessFactory.CreateDataAccess<Car>(ApiDataBaseMoq.Database(fields, typeof(Car)));
            var result = CarData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR ElementFields test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbCarGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var CarData = DataAccessFactory.CreateDataAccess<Car>(
                ApiDataBaseMoq.Database(fields, typeof(Car)));
            var result = CarData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR Car test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbCarGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                              { "carid", new List<object> { 1, 2 } },
                                 { "employeeid", new List<object> { GlobalTestVariables.EmployeeId, GlobalTestVariables.EmployeeId } }
                             };

            var CarData = DataAccessFactory.CreateDataAccess<Car>(
                ApiDataBaseMoq.Database(fields, typeof(Car)));
            var result = CarData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR Car test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbCarsUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                               { "carid", new List<object> { 1, 2 } },
                                 { "employeeid", new List<object> { GlobalTestVariables.EmployeeId, GlobalTestVariables.EmployeeId } }
                             };

            var CarData = DataAccessFactory.CreateDataAccess<Car>(ApiDataBaseMoq.Database(fields, typeof(Car), 1));
            var Car = new Car
            {
                carid = 1,
                employeeid = GlobalTestVariables.EmployeeId
                ,
                Action = Action.Create
            };
            var result = CarData.Update(new List<Car> { Car });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR Car test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbCarsDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                               { "carid", new List<object> { 1, 2 } },
                                 { "employeeid", new List<object> { GlobalTestVariables.EmployeeId, GlobalTestVariables.EmployeeId } }
                             };

            var CarData = DataAccessFactory.CreateDataAccess<Car>(ApiDataBaseMoq.Database(fields, typeof(Car), 1));

            var result = CarData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region Addresses

        /// <summary>
        /// Create ESR ElementFields Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbAddressesCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "AddressID", new List<object> { 1, 2 } },
                                 { "Line1", new List<object> { "10 Street Name", "27 Road Name" } }
                             };

            var addressData = DataAccessFactory.CreateDataAccess<Address>(ApiDataBaseMoq.Database(fields, typeof(Address), 1));
            var expected = new Address
            {
                AddressID = 1,
                Line1 = "10 Street Name",
                Action = Action.Create
            };
            var result = addressData.Create(new List<Address> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbAddressesGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "AddressID", new List<object> { 1, 2 } },
                                 { "Line1", new List<object> { "10 Street Name", "27 Road Name" } }
                             };

            var addressData = DataAccessFactory.CreateDataAccess<Address>(ApiDataBaseMoq.Database(fields, typeof(Address)));
            var result = addressData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR ElementFields test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbAddressGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var addressData = DataAccessFactory.CreateDataAccess<Address>(
                ApiDataBaseMoq.Database(fields, typeof(Address)));
            var result = addressData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR Company test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbAddressGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "AddressID", new List<object> { 1, 2 } },
                                 { "Line1", new List<object> { "10 Street Name", "27 Road Name" } }
                             };

            var addressData = DataAccessFactory.CreateDataAccess<Address>(
                ApiDataBaseMoq.Database(fields, typeof(Address)));
            var result = addressData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR Company test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbAddressesUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "AddressID", new List<object> { 1, 2 } },
                                 { "Line1", new List<object> { "10 Street Name", "27 Road Name" } }
                             };

            var addressData = DataAccessFactory.CreateDataAccess<Address>(ApiDataBaseMoq.Database(fields, typeof(Address), 1));
            var address = new Address
            {
                AddressID = 1,
                Line1 = "10 Street Name"
                ,
                Action = Action.Create
            };
            var result = addressData.Update(new List<Address> { address });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR Company test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbAddressesDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "AddressID", new List<object> { 1, 2 } },
                                 { "Line1", new List<object> { "10 Street Name", "27 Road Name" } }
                             };

            var addressData = DataAccessFactory.CreateDataAccess<Address>(ApiDataBaseMoq.Database(fields, typeof(Address), 1));

            var result = addressData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region EmployeeHomeAddresss

        /// <summary>
        /// Create ESR ElementFields Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEmployeeHomeAddressCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "EmployeeHomeAddressId", new List<object> { 1, 2 } },
                                 { "AddressId", new List<object> { 2, 3 } }
                             };

            var EmployeeHomeAddressData = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress), 1));
            var expected = new EmployeeHomeAddress
            {
                EmployeeHomeAddressId = 1,
                AddressId = 2,
                Action = Action.Create
            };
            var result = EmployeeHomeAddressData.Create(new List<EmployeeHomeAddress> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEmployeeHomeAddresssGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "EmployeeHomeAddressId", new List<object> { 1, 2 } },
                                 { "AddressId", new List<object> { 2, 3 } }
                             };

            var EmployeeHomeAddressData = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress)));
            var result = EmployeeHomeAddressData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR ElementFields test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEmployeeHomeAddressGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var EmployeeHomeAddressData = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(
                ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress)));
            var result = EmployeeHomeAddressData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR EmployeeHomeAddress test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbEmployeeHomeAddressGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                              { "EmployeeHomeAddressId", new List<object> { 1, 2 } },
                                 { "AddressId", new List<object> { 2, 3 } }
                             };

            var EmployeeHomeAddressData = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(
                ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress)));
            var result = EmployeeHomeAddressData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR EmployeeHomeAddress test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEmployeeHomeAddresssUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                               { "EmployeeHomeAddressId", new List<object> { 1, 2 } },
                                 { "AddressId", new List<object> { 2, 3 } }
                             };

            var EmployeeHomeAddressData = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress), 1));
            var EmployeeHomeAddress = new EmployeeHomeAddress
            {
                EmployeeHomeAddressId = 1,
                AddressId = 2,
                Action = Action.Create
            };
            var result = EmployeeHomeAddressData.Update(new List<EmployeeHomeAddress> { EmployeeHomeAddress });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR EmployeeHomeAddress test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbEmployeeHomeAddresssDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                               { "EmployeeHomeAddressId", new List<object> { 1, 2 } },
                                 { "AddressId", new List<object> { 2, 3 } }
                             };

            var EmployeeHomeAddressData = DataAccessFactory.CreateDataAccess<EmployeeHomeAddress>(ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress), 1));

            var result = EmployeeHomeAddressData.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region TemplateMappings

        /// <summary>
        /// Create ESR ElementFields Test Mocked.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbTemplateMappingCreate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "templateID", new List<object> { 1, 2 } },
                                 { "mandatory", new List<object> { true, false } }
                             };

            var TemplateMappingData = DataAccessFactory.CreateDataAccess<TemplateMapping>(ApiDataBaseMoq.Database(fields, typeof(TemplateMapping), 1));
            var expected = new TemplateMapping
            {
                templateID = 1,
                mandatory = true,
                Action = Action.Create
            };
            var result = TemplateMappingData.Create(new List<TemplateMapping> { expected });
            Helper.CheckFields(result[0], fields);

            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Success);
        }

        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbTemplateMappingsGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "templateID", new List<object> { 1, 2 } },
                                 { "mandatory", new List<object> { true, false } }
                             };

            var TemplateMappingData = DataAccessFactory.CreateDataAccess<TemplateMapping>(ApiDataBaseMoq.Database(fields, typeof(TemplateMapping)));
            var result = TemplateMappingData.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        /// <summary>
        /// Get single ESR ElementFields test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbTemplateMappingGetSinglethatDoesNotExist()
        {
            var fields = new SortedList<string, List<object>>();
            var TemplateMappingData = DataAccessFactory.CreateDataAccess<TemplateMapping>(
                ApiDataBaseMoq.Database(fields, typeof(TemplateMapping)));
            var result = TemplateMappingData.Read(2);
            Assert.IsTrue(result == null, string.Format("Result should be null,Action Result was actually {0}", result != null ? result.ActionResult.Result : ApiActionResult.NoAction));
            if (result != null)
            {
                Helper.CheckFields(result, fields, 0);
            }
        }

        /// <summary>
        /// Get single ESR TemplateMapping test Mocked
        /// </summary>
        [TestMethod]
        [TestCategory("APICRUD")]
        [TestCategory("ApiDatabase")]
        public void ApiDbTemplateMappingGetSingle()
        {
            var fields = new SortedList<string, List<object>>
                             {
                              { "templateID", new List<object> { 1, 2 } },
                                 { "mandatory", new List<object> { true, false } }
                             };

            var TemplateMappingData = DataAccessFactory.CreateDataAccess<TemplateMapping>(
                ApiDataBaseMoq.Database(fields, typeof(TemplateMapping)));
            var result = TemplateMappingData.Read(1);
            Assert.IsTrue(
                result.ActionResult.Result == ApiActionResult.Success,
                string.Format("Result should be sucess , was actually {0}", result.ActionResult));
            Helper.CheckFields(result, fields, 0);
        }

        /// <summary>
        /// Update ESR TemplateMapping test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbTemplateMappingsUpdate()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                { "templateID", new List<object> { 1, 2 } },
                                 { "mandatory", new List<object> { true, false } }
                             };

            var TemplateMappingData = DataAccessFactory.CreateDataAccess<TemplateMapping>(ApiDataBaseMoq.Database(fields, typeof(TemplateMapping), 1));
            var TemplateMapping = new TemplateMapping
            {
                templateID = 1,
                mandatory = true,
                Action = Action.Create
            };
            var result = TemplateMappingData.Update(new List<TemplateMapping> { TemplateMapping });
            Helper.CheckFields(result[0], fields, 0);


            Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Success, string.Format("Action result should have been {0}, but returned {1}", ApiActionResult.Success, result[0].ActionResult));
        }

        /// <summary>
        /// Update ESR TemplateMapping test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbTemplateMappingsDelete()
        {
            var fields = new SortedList<string, List<object>>
                             {
                               { "templateID", new List<object> { 1, 2 } },
                                 { "mandatory", new List<object> { true, false } }
                             };

            var dataAccess = DataAccessFactory.CreateDataAccess<TemplateMapping>(ApiDataBaseMoq.Database(fields, typeof(TemplateMapping), 1));

            var result = dataAccess.Delete(1);

            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Action result should be Deleted but returned {0}", result.ActionResult));
            Helper.CheckFields(result, fields);
        }

        #endregion

        #region AccountProperties
        /// <summary>
        /// Get all ESR ElementFields test Mocked
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        public void ApiDbAccountPropertiesGet()
        {
            var fields = new SortedList<string, List<object>>
                             {
                                 { "subAccountId", new List<object> { 1, 1} },
                                 { "stringKey", new List<object> { "key1", "key2" } },
                                 { "StringValue", new List<object> { "value", "value2" } }
                             };

            var dataAccess = DataAccessFactory.CreateDataAccess<AccountProperty>(ApiDataBaseMoq.Database(fields, typeof(AccountProperty)));
            var result = dataAccess.ReadAll();
            Assert.IsTrue(result.Count == 2, string.Format("Record count should be 2, was actually {0}", result.Count));
            Helper.CheckFields(result[0], fields, 0);
            Helper.CheckFields(result[1], fields, 1);
        }

        #endregion

        #region Database Storedprocedure

        #region Employees

        /// <summary>
        /// Delete Employees Test .
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIdeleteEmployees()
        {
            var employees = new cEmployees(GlobalTestVariables.AccountId);
            var oldEmployee = employees.GetEmployeeById(GlobalTestVariables.EmployeeId);
            int newId = 0;
            try
            {
                var employeeDataAccess = DataAccessFactory.CreateDataAccess<Employee>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var employee = new Employee { firstname = oldEmployee.Forename, middlenames = oldEmployee.MiddleNames, surname = oldEmployee.Surname, username = oldEmployee.Username + "UT", Action = Action.Create };
                var newEmployees = employeeDataAccess.Create(new List<Employee> { employee });
                Assert.IsTrue(newEmployees[0].employeeid > 0, string.Format("Return code should be positive number but was actually {0} - {1}", newEmployees[0].employeeid, newEmployees[0].ActionResult.Message));
                newId = newEmployees[0].employeeid;
                newEmployees[0].archived = true;
                employeeDataAccess.Update(newEmployees);
                var result = employeeDataAccess.Delete(newEmployees[0].employeeid);
                Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, result.ActionResult.Message);
            }
            finally
            {
                var employee = employees.GetEmployeeById(newId);
                if (employee != null)
                {
                    employee.Archived = true;
                    employee.Save(Moqs.CurrentUser());
                    employee.Delete(Moqs.CurrentUser());
                }
            }
        }

        #endregion
        
        #region EsrPhone

        /// <summary>
        /// Delete Employees Test .
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIdeleteEsrPhone()
        {
            var phone = new EsrPhone { ESRPersonId = 1, ESRPhoneId = 100, PhoneNumber = "1234", PhoneType = "mobile", EffectiveStartDate = new DateTime(2012, 12, 12), Action = Action.Create };
            var person = new EsrPerson { ESRPersonId = 1, Action = Action.Create };
            var personCreate = new List<EsrPerson>();
            var phoneCreate = new List<EsrPhone>();
            try
            {
                var personDataAccess = DataAccessFactory.CreateDataAccess<EsrPerson>(
                    new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                personCreate = personDataAccess.Create(new List<EsrPerson> { person });
                var phoneDataAccess = DataAccessFactory.CreateDataAccess<EsrPhone>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                phoneCreate = phoneDataAccess.Create(new List<EsrPhone> { phone });
                Assert.IsTrue(phone.ESRPhoneId > 0, string.Format("Return code should be positive number but was actually {0}", phone.ESRPhoneId));
                var result = phoneDataAccess.Delete(phone.ESRPhoneId);
                Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Return code is {0} but should be {1}", result.ActionResult.Result, ApiActionResult.Deleted));
            }
            finally
            {
                var personDataAccess = DataAccessFactory.CreateDataAccess<EsrPerson>(
                       new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var phoneDataAccess = DataAccessFactory.CreateDataAccess<EsrPhone>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                if (phoneCreate != null)
                {
                    var result = phoneDataAccess.Delete(phoneCreate[0].ESRPhoneId);
                }
                if (personCreate != null)
                {
                    personDataAccess.Delete(personCreate[0].ESRPersonId);
                }
            }
        }
        #endregion

        #region EsrPerson

        /// <summary>
        /// Delete Employees Test .
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIdeleteEsrPerson()
        {
            var Person = new List<EsrPerson> { new EsrPerson { ESRPersonId = 100, Action = Action.Create } };
            try
            {
                var PersonDataAccess = DataAccessFactory.CreateDataAccess<EsrPerson>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                Person = PersonDataAccess.Create(Person);
                Assert.IsTrue(Person[0].ESRPersonId > 0, string.Format("Return code should be positive number but was actually {0}", Person[0].ESRPersonId));
                var result = PersonDataAccess.Delete(Person[0].ESRPersonId);
            }
            finally
            {
                var PersonDataAccess = DataAccessFactory.CreateDataAccess<EsrPerson>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var result = PersonDataAccess.Delete(Person[0].ESRPersonId);
            }
        }
        #endregion

        #region EsrAddress

        /// <summary>
        /// Delete Employees Test .
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIdeleteEsrAddress()
        {
            var address = new List<EsrAddress>
                              {
                                  new EsrAddress
                                      {
                                          ESRAddressId = 100,
                                          EffectiveEndDate = new DateTime(2012, 12, 12),
                                          EffectiveStartDate = new DateTime(2012, 12, 12),
                                          AddressStyle = "Classic",
                                          AddressLine1 = "line1",
                                          AddressLine2 = "line2",
                                          AddressLine3 = "line3",
                                          AddressTown = "town",
                                          AddressPostcode = "postcode",
                                          AddressCounty = "county",
                                          AddressCountry = "GB",
                                          AddressType = "home",
                                          PrimaryFlag = "yes",
                                          ESRPersonId = 1, Action = Action.Create
                                      }
                              };
            var person = new List<EsrPerson> { new EsrPerson { ESRPersonId = 1, Action = Action.Create } };
            try
            {
                var personDataAccess = DataAccessFactory.CreateDataAccess<EsrPerson>(
                    new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                person = personDataAccess.Create(person);
                var addressDataAccess = DataAccessFactory.CreateDataAccess<EsrAddress>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                address = addressDataAccess.Create(address);
                Assert.IsTrue(address[0].ESRAddressId > 0, string.Format("Return code should be positive number but was actually {0} - {1}", address[0].ESRAddressId, address[0].ActionResult.Message));
                var result = addressDataAccess.Delete(address[0].ESRAddressId);
                Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
            }
            finally
            {
                var addressDataAccess = DataAccessFactory.CreateDataAccess<EsrAddress>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var result = addressDataAccess.Delete(address[0].ESRAddressId);
                var personDataAccess = DataAccessFactory.CreateDataAccess<EsrPerson>(
                    new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                personDataAccess.Delete(person[0].ESRPersonId);
            }
        }
        #endregion

        #region EsrAssignmentCostings

        /// <summary>
        /// Delete AssignmentCostings Test .
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIdeleteEsrAssignmentCosting()
        {
            var assignmentCostings = new List<EsrAssignmentCostings>
                                         {
                                             new EsrAssignmentCostings
                                                 {
                                                     ESRPersonId = 1,
                                                     ESRCostingAllocationId
                                                         = 100,
                                                     EffectiveStartDate
                                                         =
                                                         new DateTime(
                                                         2012, 12, 12), Action = Action.Create
                                                 }
                                         };
            var person = new List<EsrPerson> { new EsrPerson { ESRPersonId = 1, Action = Action.Create, FirstName = "UT", LastName = "esrassignmentcostings", Title = "UT"} };
            try
            {
                var personDataAccess = DataAccessFactory.CreateDataAccess<EsrPerson>(
                    new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                person = personDataAccess.Create(person);
                var dataAccess = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                assignmentCostings = dataAccess.Create(assignmentCostings);
                Assert.IsTrue(assignmentCostings[0].ESRCostingAllocationId > 0, string.Format("Return code should be positive number but was actually {0}", assignmentCostings[0].ESRCostingAllocationId));
                var result = dataAccess.Delete(assignmentCostings[0].ESRCostingAllocationId);
                Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Return code is {0} but should be {1}", result.ActionResult.Result, ApiActionResult.Deleted));
            }
            finally
            {
                var personDataAccess = DataAccessFactory.CreateDataAccess<EsrPerson>(
                       new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var AssignmentCostingsDataAccess = DataAccessFactory.CreateDataAccess<EsrAssignmentCostings>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var result = AssignmentCostingsDataAccess.Delete(assignmentCostings[0].ESRCostingAllocationId);
                personDataAccess.Delete(person[0].ESRPersonId);
            }
        }
        #endregion

        //#region EsrElement

        ///// <summary>
        ///// Delete EsrElement Test .
        ///// </summary>
        //[TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabase")]
        //public void ApiDbStoredProcAPIdeleteEsrElement()
        //{
        //    var EsrElement = new List<EsrElement> { new EsrElement() };

        //    var trustDataAccess = DataAccessFactory.CreateDataAccess<EsrTrust>(
        //            new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
        //    var esrTrust = new List<EsrTrust>
        //                       {
        //                           new EsrTrust
        //                               {
        //                                   archived = false,
        //                                   runSequenceNumber = 1,
        //                                   trustVPD = "123", Action = Action.Create,
        //                                   ESRVersionNumber = 2
        //                               }
        //                       };
        //    try
        //    {
        //        esrTrust = trustDataAccess.Create(esrTrust);
        //        Assert.IsTrue(esrTrust[0].ActionResult.Result == ApiActionResult.Success, string.Format("trust not created {0}", esrTrust[0].ActionResult.Message));

        //        EsrElement[0].NHSTrustID = esrTrust[0].trustID;
        //        EsrElement[0].globalElementID = 10;
        //        EsrElement[0].Action = Action.Create;

        //        var esrElementDataAccess = DataAccessFactory.CreateDataAccess<EsrElement>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
        //        EsrElement = esrElementDataAccess.Create(EsrElement);
        //        Assert.IsTrue(EsrElement[0].ActionResult.Result == ApiActionResult.Success, string.Format("Element not created, return code {0}, message {1}", EsrElement[0].ActionResult.Result, EsrElement[0].ActionResult.Message));
        //        Assert.IsTrue(EsrElement[0].elementID > 0, string.Format("Return code should be positive number but was actually {0}", EsrElement[0].elementID));
        //        var result = esrElementDataAccess.Delete(EsrElement[0].elementID);
        //        Assert.IsTrue(result != null, "Delete did not return an object");
        //        Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Return code is {0} but should be {1}", result.ActionResult.Result, ApiActionResult.Deleted));
        //    }
        //    finally
        //    {
        //        var esrElementDataAccess = DataAccessFactory.CreateDataAccess<EsrElement>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
        //        esrElementDataAccess.Delete(EsrElement[0].elementID);
        //        trustDataAccess.Delete(esrTrust[0]);
        //    }
        //}
        //#endregion

        #region EsrOrganisation

        /// <summary>
        /// Delete EsrOrganisation Test .
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIdeleteEsrOrganisation()
        {
            var esrOrganisation = new List<EsrOrganisation> { new EsrOrganisation() };
            var esrLocation = new List<EsrLocation>
                                  {
                                      new EsrLocation
                                          {
                                              AddressLine1 = "line1",
                                              Description = "description",
                                              ESRLastUpdate = new DateTime(2012, 12, 12), Action = Action.Create
                                          }
                                  };
            var esrLocationDataAccess = DataAccessFactory.CreateDataAccess<EsrLocation>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));

            try
            {
                esrLocation = esrLocationDataAccess.Create(esrLocation);
                Assert.IsTrue(esrLocation[0].ActionResult.Result == ApiActionResult.Success, string.Format("location create failed, {0}", esrLocation[0].ActionResult.Message));


                var esrOrganisationDataAccess = DataAccessFactory.CreateDataAccess<EsrOrganisation>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                esrOrganisation[0].ESROrganisationId = 100;
                esrOrganisation[0].ESRLocationId = esrLocation[0].ESRLocationId;
                esrOrganisation[0].EffectiveFrom = new DateTime(2012, 12, 12);
                esrOrganisation[0].ESRLastUpdateDate = new DateTime(2012, 12, 12);
                esrOrganisation[0].Action = Action.Create;

                esrOrganisation = esrOrganisationDataAccess.Create(esrOrganisation);
                Assert.IsTrue(esrOrganisation[0].ESROrganisationId > 0, string.Format("Return code should be positive number but was actually {0}", esrOrganisation[0].ESROrganisationId));
                var result = esrOrganisationDataAccess.Delete(esrOrganisation[0].ESROrganisationId);
                Assert.IsTrue(result != null, "Delete did not return an object");
                Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Return code is {0} but should be {1}", result.ActionResult.Result, ApiActionResult.Deleted));
            }
            finally
            {
                var esrOrganisationDataAccess = DataAccessFactory.CreateDataAccess<EsrOrganisation>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var result = esrOrganisationDataAccess.Delete(esrOrganisation[0].ESROrganisationId);
                esrLocationDataAccess.Delete(esrLocation[0].ESRLocationId);
            }
        }
        #endregion

        #region EsrLocation

        /// <summary>
        /// Delete EsrLocation Test .
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIdeleteEsrLocation()
        {
            var EsrLocation = new List<EsrLocation>
                                  {
                                      new EsrLocation
                                          {
                                              AddressLine1 = "Line 1",
                                              Description = "Desc",
                                              AddressLine2 = "Line 2",
                                              ESRLocationId = 100,
                                              ESRLastUpdate = new DateTime(2012, 12, 12), Action = Action.Create
                                          }
                                  };
            var person = new EsrPerson { ESRPersonId = 1 };
            try
            {
                var esrLocationDataAccess = DataAccessFactory.CreateDataAccess<EsrLocation>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                EsrLocation = esrLocationDataAccess.Create(EsrLocation);
                Assert.IsTrue(EsrLocation[0].ESRLocationId > 0, string.Format("Return code should be positive number but was actually {0}", EsrLocation[0].ESRLocationId));
                var result = esrLocationDataAccess.Delete(EsrLocation[0].ESRLocationId);
                Assert.IsTrue(result != null, "Delete did not return an object");
                Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Return code is {0} but should be {1}", result.ActionResult.Result, ApiActionResult.Deleted));
            }
            finally
            {

                var esrLocationDataAccess = DataAccessFactory.CreateDataAccess<EsrLocation>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var result = esrLocationDataAccess.Delete(EsrLocation[0].ESRLocationId);
            }
        }
        #endregion
        #region EsrPosition

        /// <summary>
        /// Delete EsrPosition Test .
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIdeleteEsrPosition()
        {
            var esrPosition = new List<EsrPosition>
                                  {
                                      new EsrPosition
                                          {
                                              ESRPositionId = 100,
                                              EffectiveFromDate = new DateTime(2012, 12, 12),
                                              ESRLastUpdateDate = new DateTime(2012, 12, 12), Action = Action.Create
                                          }
                                  };
            var person = new EsrPerson { ESRPersonId = 1 };
            try
            {
                var esrPositionDataAccess = DataAccessFactory.CreateDataAccess<EsrPosition>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                esrPosition = esrPositionDataAccess.Create(esrPosition);
                Assert.IsTrue(esrPosition[0].ESRPositionId > 0, string.Format("Return code should be positive number but was actually {0}", esrPosition[0].ESRPositionId));
                var result = esrPositionDataAccess.Delete(esrPosition[0].ESRPositionId);
                Assert.IsTrue(result != null, "Delete did not return an object");
                Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Return code is {0} but should be {1}", result.ActionResult.Result, ApiActionResult.Deleted));
            }
            finally
            {

                var esrPositionDataAccess = DataAccessFactory.CreateDataAccess<EsrPosition>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var result = esrPositionDataAccess.Delete(esrPosition[0].ESRPositionId);
            }
        }
        #endregion
        //#region EsrElementSubCat

        ///// <summary>
        ///// Delete ESRELEMENTSUBCAT Test .
        ///// </summary>
        //[TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        //public void ApiDbStoredProcAPIdeleteEsrElementSubCat()
        //{
        //    var subcats = new cSubcats(GlobalTestVariables.AccountId);
        //    var subcat = new cSubcat();
        //    foreach (SubcatBasic subcatBasic in subcats.GetSortedList())
        //    {
        //        subcat = subcats.GetSubcatById(subcatBasic.SubcatId);
        //        break;
        //    }

        //    var esrElementSubCat = new List<EsrElementSubCat>
        //                               {
        //                                   new EsrElementSubCat
        //                                       {
        //                                           elementID = 100,
        //                                           subcatID = subcat.subcatid, Action = Action.Create
        //                                       }
        //                               };

        //    try
        //    {
        //        var esrElementSubCatDataAccess = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
        //        esrElementSubCat = esrElementSubCatDataAccess.Create(esrElementSubCat);
        //        Assert.IsTrue(esrElementSubCat[0].elementSubcatID > 0, string.Format("Return code should be positive number but was actually {0}", esrElementSubCat[0].elementSubcatID));
        //        var result = esrElementSubCatDataAccess.Delete(esrElementSubCat[0].elementSubcatID);
        //        Assert.IsTrue(result != null, "Delete did not return an object");
        //        Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Return code is {0} but should be {1}", result.ActionResult.Result, ApiActionResult.Deleted));
        //    }
        //    finally
        //    {
        //        var esrElementSubCatDataAccess = DataAccessFactory.CreateDataAccess<EsrElementSubCat>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
        //        esrElementSubCatDataAccess.Delete(esrElementSubCat[0].elementSubcatID);
        //    }
        //}
        //#endregion
        #region Car

        /// <summary>
        /// Delete Car Test .
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIdeleteCar()
        {
            var Car = new List<Car>
                          {
                              new Car
                                  {
                                      employeeid = GlobalTestVariables.EmployeeId,
                                      make = "ford",
                                      model = "model", Action = Action.Create
                                  }
                          };
            var person = new EsrPerson { ESRPersonId = 1 };
            try
            {
                var carDataAccess = DataAccessFactory.CreateDataAccess<Car>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                Car = carDataAccess.Create(Car);
                Assert.IsTrue(Car[0].ActionResult.Result == ApiActionResult.Success, string.Format("car not created, {0}", Car[0].ActionResult.Message));
                Assert.IsTrue(Car[0].carid > 0, string.Format("Return code should be positive number but was actually {0}", Car[0].carid));

                var result = carDataAccess.Delete(Car[0].carid);
                Assert.IsTrue(result != null, "Delete did not return an object");
                Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted, string.Format("Return code is {0} but should be {1}", result.ActionResult.Result, ApiActionResult.Deleted));
            }
            finally
            {
                var carDataAccess = DataAccessFactory.CreateDataAccess<Car>(new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var result = carDataAccess.Delete(Car[0].carid);
            }
        }
        #endregion

        /// <summary>
        /// The API DB stored proc API get template mappings.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIgetTemplateMappings()
        {
            try
            {
                var templateMap = DataAccessFactory.CreateDataAccess<TemplateMapping>(
                    new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var result = templateMap.ReadAll();
                Assert.IsTrue(result != null);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Create ESR Assignments Test with foreign key failure.
        /// </summary>
        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiDatabaseStoredProcedure")]
        public void ApiDbStoredProcAPIsaveVehicleWithForeignKeyFail()
        {
            List<EsrVehicle> result = null;
            var person = new EsrPerson { ESRPersonId = 3 };
            try
            {
                var dataAccess = DataAccessFactory.CreateDataAccess<EsrVehicle>(
                    new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                var esrVehicle = new List<EsrVehicle>
                                     {
                                         new EsrVehicle
                                             {
                                                 ESRVehicleAllocationId = 1,
                                                 ESRPersonId = 3,
                                                 EffectiveEndDate = new DateTime(2012, 12, 12),
                                                 EffectiveStartDate =
                                                     new DateTime(2012, 12, 12), Action = Action.Create
                                             }
                                     };
                result = dataAccess.Create(esrVehicle);

                Assert.IsTrue(result[0].ActionResult.Result == ApiActionResult.Failure, "Action result returned {0} and not {1}", result[0].ActionResult, ApiActionResult.Failure);
                Assert.IsTrue(result[0].ActionResult.Message != string.Empty, "Should return an error message but error was blank");
            }
            finally
            {
                if (result != null)
                {
                    var esrVehicleDataAccess = DataAccessFactory.CreateDataAccess<EsrVehicle>(
                    new ApiDbConnection("metabase", GlobalTestVariables.AccountId));
                    esrVehicleDataAccess.Delete(result[0].ESRVehicleAllocationId);
                }
            }
        }

        #endregion
    }
}
