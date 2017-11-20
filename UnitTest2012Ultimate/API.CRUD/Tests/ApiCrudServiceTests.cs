namespace UnitTest2012Ultimate.API.CRUD.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;

    using EsrGo2FromNhsWcfLibrary;
    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using UnitTest2012Ultimate.API.CRUD.Objects;

    /// <summary>
    /// The API CRUD service tests.
    /// </summary>
    [TestClass]
    public class ApiCrudServiceTests
    {
//        #region Employees

//        /// <summary>
//        /// The API crud create employee.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "employeeid", new List<object> { 1 } },
//                                 {
//                                     "firstname",
//                                     new List<object> { "firstname", "anothername" }
//                                 }
//                             };

//            var employees = new List<Employee>
//                                {
//                                    new Employee
//                                        {
//                                            username = "UTUsername",
//                                            firstname = "firstname",
//                                            surname = "surname"
//                                        }
//                                };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Employee));
//            List<Employee> result = apiCrudService.CreateEmployees("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), employees);
//            int i = 0;
//            foreach (Employee employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All employees.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeesReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "employeeid", new List<object> { 1 } },
//                                 { "username", new List<object> { "username" } },
//                                 {
//                                     "password",
//                                     new List<object> { "vlUnCTgO1bFJXxfib/suGQ==" }
//                                 },
//                                 { "title", new List<object> { "Mr" } },
//                                 { "firstname", new List<object> { "firstname" } },
//                                 { "surname", new List<object> { "surname" } },
//                                 { "mileagetotal", new List<object> { 1 } },
//                                 { "email", new List<object> { "email@server.com" } },
//                                 { "currefnum", new List<object> { 2 } },
//                                 { "curclaimno", new List<object> { 3 } },
//                                 { "payroll", new List<object> { "payroll" } },
//                                 { "position", new List<object> { "position" } },
//                                 { "telno", new List<object> { "telno" } },
//                                 { "creditor", new List<object> { "creditor" } },
//                                 { "archived", new List<object> { false } },
//                                 { "groupid", new List<object> { 5 } },
//                                 {
//                                     "lastchange",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "additems", new List<object> { 7 } },
//                                 { "costcodeid", new List<object> { 8 } },
//                                 { "departmentid", new List<object> { 8 } },
//                                 { "extension", new List<object> { "extension" } },
//                                 { "pagerno", new List<object> { "pagerno" } },
//                                 { "mobileno", new List<object> { "mobileno" } },
//                                 { "faxno", new List<object> { "faxno" } },
//                                 { "homeemail", new List<object> { "homeemail" } },
//                                 { "linemanager", new List<object> { 9 } },
//                                 { "advancegroupid", new List<object> { 10 } },
//                                 { "active", new List<object> { false } },
//                                 { "primarycountry", new List<object> { 13 } },
//                                 { "primarycurrency", new List<object> { 14 } },
//                                 { "verified", new List<object> { false } },
//                                 {
//                                     "licenceexpiry",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "licencelastchecked",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 },
//                                 { "licencecheckedby", new List<object> { 15 } },
//                                 {
//                                     "licencenumber", new List<object> { "licencenumber" }
//                                 },
//                                 { "groupidcc", new List<object> { 16 } },
//                                 { "groupidpc", new List<object> { 17 } },
//                                 {
//                                     "CreatedOn",
//                                     new List<object> { new DateTime(2012, 12, 15) }
//                                 },
//                                 { "CreatedBy", new List<object> { 18 } },
//                                 {
//                                     "ModifiedOn",
//                                     new List<object> { new DateTime(2012, 12, 16) }
//                                 },
//                                 { "ModifiedBy", new List<object> { 19 } },
//                                 { "country", new List<object> { "country" } },
//                                 { "ninumber", new List<object> { "ninumber" } },
//                                 { "maidenname", new List<object> { "maidenname" } },
//                                 { "middlenames", new List<object> { "middlenames" } },
//                                 { "gender", new List<object> { "gender" } },
//                                 {
//                                     "dateofbirth",
//                                     new List<object> { new DateTime(2012, 12, 17) }
//                                 },
//                                 {
//                                     "hiredate",
//                                     new List<object> { new DateTime(2012, 12, 18) }
//                                 },
//                                 {
//                                     "terminationdate",
//                                     new List<object> { new DateTime(2012, 12, 19) }
//                                 },
//                                 { "homelocationid", new List<object> { 20 } },
//                                 { "officelocationid", new List<object> { 21 } },
//                                 { "name", new List<object> { "name" } },
//                                 {
//                                     "accountnumber", new List<object> { "accountnumber" }
//                                 },
//                                 { "accounttype", new List<object> { "accounttype" } },
//                                 { "sortcode", new List<object> { "sortcode" } },
//                                 { "reference", new List<object> { "reference" } },
//                                 { "localeID", new List<object> { 22 } },
//                                 { "NHSTrustID", new List<object> { 23 } },
//                                 { "logonCount", new List<object> { 24 } },
//                                 { "retryCount", new List<object> { 25 } },
//                                 { "firstLogon", new List<object> { false } },
//                                 { "licenceAttachID", new List<object> { 26 } },
//                                 { "defaultSubAccountId", new List<object> { 27 } },
//                                 { "CreationMethod", new List<object> { '4' } },
//                                 {
//                                     "mileagetotaldate",
//                                     new List<object> { new DateTime(2012, 12, 21) }
//                                 },
//                                 { "adminonly", new List<object> { false } },
//                                 { "locked", new List<object> { true } },
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Employee));
//            List<Employee> result = apiCrudService.ReadAllEmployees("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (Employee employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read employee.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "employeeid", new List<object> { 1 } },
//                                 { "username", new List<object> { "username" } },
//                                 {
//                                     "password",
//                                     new List<object> { "vlUnCTgO1bFJXxfib/suGQ==" }
//                                 },
//                                 { "title", new List<object> { "Mr" } },
//                                 { "firstname", new List<object> { "firstname" } },
//                                 { "surname", new List<object> { "surname" } },
//                                 { "mileagetotal", new List<object> { 1 } },
//                                 { "email", new List<object> { "email@server.com" } },
//                                 { "currefnum", new List<object> { 2 } },
//                                 { "curclaimno", new List<object> { 3 } },
//                                 { "payroll", new List<object> { "payroll" } },
//                                 { "position", new List<object> { "position" } },
//                                 { "telno", new List<object> { "telno" } },
//                                 { "creditor", new List<object> { "creditor" } },
//                                 { "archived", new List<object> { false } },
//                                 { "groupid", new List<object> { 5 } },
//                                 {
//                                     "lastchange",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "additems", new List<object> { 7 } },
//                                 { "costcodeid", new List<object> { 8 } },
//                                 { "departmentid", new List<object> { 8 } },
//                                 { "extension", new List<object> { "extension" } },
//                                 { "pagerno", new List<object> { "pagerno" } },
//                                 { "mobileno", new List<object> { "mobileno" } },
//                                 { "faxno", new List<object> { "faxno" } },
//                                 { "homeemail", new List<object> { "homeemail" } },
//                                 { "linemanager", new List<object> { 9 } },
//                                 { "advancegroupid", new List<object> { 10 } },
//                                 { "active", new List<object> { false } },
//                                 { "primarycountry", new List<object> { 13 } },
//                                 { "primarycurrency", new List<object> { 14 } },
//                                 { "verified", new List<object> { false } },
//                                 {
//                                     "licenceexpiry",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "licencelastchecked",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 },
//                                 { "licencecheckedby", new List<object> { 15 } },
//                                 {
//                                     "licencenumber", new List<object> { "licencenumber" }
//                                 },
//                                 { "groupidcc", new List<object> { 16 } },
//                                 { "groupidpc", new List<object> { 17 } },
//                                 {
//                                     "CreatedOn",
//                                     new List<object> { new DateTime(2012, 12, 15) }
//                                 },
//                                 { "CreatedBy", new List<object> { 18 } },
//                                 {
//                                     "ModifiedOn",
//                                     new List<object> { new DateTime(2012, 12, 16) }
//                                 },
//                                 { "ModifiedBy", new List<object> { 19 } },
//                                 { "country", new List<object> { "country" } },
//                                 { "ninumber", new List<object> { "ninumber" } },
//                                 { "maidenname", new List<object> { "maidenname" } },
//                                 { "middlenames", new List<object> { "middlenames" } },
//                                 { "gender", new List<object> { "gender" } },
//                                 {
//                                     "dateofbirth",
//                                     new List<object> { new DateTime(2012, 12, 17) }
//                                 },
//                                 {
//                                     "hiredate",
//                                     new List<object> { new DateTime(2012, 12, 18) }
//                                 },
//                                 {
//                                     "terminationdate",
//                                     new List<object> { new DateTime(2012, 12, 19) }
//                                 },
//                                 { "homelocationid", new List<object> { 20 } },
//                                 { "officelocationid", new List<object> { 21 } },
//                                 { "name", new List<object> { "name" } },
//                                 {
//                                     "accountnumber", new List<object> { "accountnumber" }
//                                 },
//                                 { "accounttype", new List<object> { "accounttype" } },
//                                 { "sortcode", new List<object> { "sortcode" } },
//                                 { "reference", new List<object> { "reference" } },
//                                 { "localeID", new List<object> { 22 } },
//                                 { "NHSTrustID", new List<object> { 23 } },
//                                 { "logonCount", new List<object> { 24 } },
//                                 { "retryCount", new List<object> { 25 } },
//                                 { "firstLogon", new List<object> { false } },
//                                 { "licenceAttachID", new List<object> { 26 } },
//                                 { "defaultSubAccountId", new List<object> { 27 } },
//                                 { "CreationMethod", new List<object> { '4' } },
//                                 {
//                                     "mileagetotaldate",
//                                     new List<object> { new DateTime(2012, 12, 21) }
//                                 },
//                                 { "adminonly", new List<object> { false } },
//                                 { "locked", new List<object> { true } },
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Employee));
//            Employee result = apiCrudService.ReadEmployee("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields);
//        }

//        /// <summary>
//        /// The API crud Update employee.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "employeeid", new List<object> { 1 } },
//                                 { "username", new List<object> { "username" } },
//                                 {
//                                     "password",
//                                     new List<object> { "vlUnCTgO1bFJXxfib/suGQ==" }
//                                 },
//                                 { "title", new List<object> { "Mr" } },
//                                 { "firstname", new List<object> { "firstname" } },
//                                 { "surname", new List<object> { "surname" } },
//                                 { "mileagetotal", new List<object> { 1 } },
//                                 { "email", new List<object> { "email@server.com" } },
//                                 { "currefnum", new List<object> { 2 } },
//                                 { "curclaimno", new List<object> { 3 } },
//                                 { "postcode", new List<object> { "postcode" } },
//                                 { "payroll", new List<object> { "payroll" } },
//                                 { "position", new List<object> { "position" } },
//                                 { "telno", new List<object> { "telno" } },
//                                 { "creditor", new List<object> { "creditor" } },
//                                 { "archived", new List<object> { false } },
//                                 { "groupid", new List<object> { 5 } },
//                                 {
//                                     "lastchange",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "additems", new List<object> { 7 } },
//                                 { "costcodeid", new List<object> { 8 } },
//                                 { "departmentid", new List<object> { 8 } },
//                                 { "extension", new List<object> { "extension" } },
//                                 { "pagerno", new List<object> { "pagerno" } },
//                                 { "mobileno", new List<object> { "mobileno" } },
//                                 { "faxno", new List<object> { "faxno" } },
//                                 { "homeemail", new List<object> { "homeemail" } },
//                                 { "linemanager", new List<object> { 9 } },
//                                 { "advancegroupid", new List<object> { 10 } },
//                                 { "active", new List<object> { false } },
//                                 { "primarycountry", new List<object> { 13 } },
//                                 { "primarycurrency", new List<object> { 14 } },
//                                 { "verified", new List<object> { false } },
//                                 {
//                                     "licenceexpiry",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "licencelastchecked",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 },
//                                 { "licencecheckedby", new List<object> { 15 } },
//                                 {
//                                     "licencenumber", new List<object> { "licencenumber" }
//                                 },
//                                 { "groupidcc", new List<object> { 16 } },
//                                 { "groupidpc", new List<object> { 17 } },
//                                 {
//                                     "CreatedOn",
//                                     new List<object> { new DateTime(2012, 12, 15) }
//                                 },
//                                 { "CreatedBy", new List<object> { 18 } },
//                                 {
//                                     "ModifiedOn",
//                                     new List<object> { new DateTime(2012, 12, 16) }
//                                 },
//                                 { "ModifiedBy", new List<object> { 19 } },
//                                 { "country", new List<object> { "country" } },
//                                 { "ninumber", new List<object> { "ninumber" } },
//                                 { "maidenname", new List<object> { "maidenname" } },
//                                 { "middlenames", new List<object> { "middlenames" } },
//                                 { "gender", new List<object> { "gender" } },
//                                 {
//                                     "dateofbirth",
//                                     new List<object> { new DateTime(2012, 12, 17) }
//                                 },
//                                 {
//                                     "hiredate",
//                                     new List<object> { new DateTime(2012, 12, 18) }
//                                 },
//                                 {
//                                     "terminationdate",
//                                     new List<object> { new DateTime(2012, 12, 19) }
//                                 },
//                                 { "homelocationid", new List<object> { 20 } },
//                                 { "officelocationid", new List<object> { 21 } },
//                                 { "name", new List<object> { "name" } },
//                                 {
//                                     "accountnumber", new List<object> { "accountnumber" }
//                                 },
//                                 { "accounttype", new List<object> { "accounttype" } },
//                                 { "sortcode", new List<object> { "sortcode" } },
//                                 { "reference", new List<object> { "reference" } },
//                                 { "localeID", new List<object> { 22 } },
//                                 { "NHSTrustID", new List<object> { 23 } },
//                                 { "logonCount", new List<object> { 24 } },
//                                 { "retryCount", new List<object> { 25 } },
//                                 { "firstLogon", new List<object> { false } },
//                                 { "licenceAttachID", new List<object> { 26 } },
//                                 { "defaultSubAccountId", new List<object> { 27 } },
//                                 { "CreationMethod", new List<object> { '4' } },
//                                 {
//                                     "mileagetotaldate",
//                                     new List<object> { new DateTime(2012, 12, 21) }
//                                 },
//                                 { "adminonly", new List<object> { false } },
//                                 { "Locked", new List<object> { true } },
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Employee));
//            var employees = new List<Employee> { new Employee { username = "changed" } };

//            List<Employee> result = apiCrudService.UpdateEmployees("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), employees);
//            int i = 0;
//            foreach (Employee employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete employee.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "employeeid", new List<object> { 1 } },
//                                 { "username", new List<object> { "username" } },
//                                 {
//                                     "password",
//                                     new List<object> { "vlUnCTgO1bFJXxfib/suGQ==" }
//                                 },
//                                 { "title", new List<object> { "Mr" } },
//                                 { "firstname", new List<object> { "firstname" } },
//                                 { "surname", new List<object> { "surname" } },
//                                 { "mileagetotal", new List<object> { 1 } },
//                                 { "email", new List<object> { "email@server.com" } },
//                                 { "currefnum", new List<object> { 2 } },
//                                 { "curclaimno", new List<object> { 3 } },
//                                 { "payroll", new List<object> { "payroll" } },
//                                 { "position", new List<object> { "position" } },
//                                 { "telno", new List<object> { "telno" } },
//                                 { "creditor", new List<object> { "creditor" } },
//                                 { "archived", new List<object> { false } },
//                                 { "groupid", new List<object> { 5 } },
//                                 {
//                                     "lastchange",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "additems", new List<object> { 7 } },
//                                 { "costcodeid", new List<object> { 8 } },
//                                 { "departmentid", new List<object> { 8 } },
//                                 { "extension", new List<object> { "extension" } },
//                                 { "pagerno", new List<object> { "pagerno" } },
//                                 { "mobileno", new List<object> { "mobileno" } },
//                                 { "faxno", new List<object> { "faxno" } },
//                                 { "homeemail", new List<object> { "homeemail" } },
//                                 { "linemanager", new List<object> { 9 } },
//                                 { "advancegroupid", new List<object> { 10 } },
//                                 { "active", new List<object> { false } },
//                                 { "primarycountry", new List<object> { 13 } },
//                                 { "primarycurrency", new List<object> { 14 } },
//                                 { "verified", new List<object> { false } },
//                                 {
//                                     "licenceexpiry",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "licencelastchecked",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 },
//                                 { "licencecheckedby", new List<object> { 15 } },
//                                 {
//                                     "licencenumber", new List<object> { "licencenumber" }
//                                 },
//                                 { "groupidcc", new List<object> { 16 } },
//                                 { "groupidpc", new List<object> { 17 } },
//                                 {
//                                     "CreatedOn",
//                                     new List<object> { new DateTime(2012, 12, 15) }
//                                 },
//                                 { "CreatedBy", new List<object> { 18 } },
//                                 {
//                                     "ModifiedOn",
//                                     new List<object> { new DateTime(2012, 12, 16) }
//                                 },
//                                 { "ModifiedBy", new List<object> { 19 } },
//                                 { "country", new List<object> { "country" } },
//                                 { "ninumber", new List<object> { "ninumber" } },
//                                 { "maidenname", new List<object> { "maidenname" } },
//                                 { "middlenames", new List<object> { "middlenames" } },
//                                 { "gender", new List<object> { "gender" } },
//                                 {
//                                     "dateofbirth",
//                                     new List<object> { new DateTime(2012, 12, 17) }
//                                 },
//                                 {
//                                     "hiredate",
//                                     new List<object> { new DateTime(2012, 12, 18) }
//                                 },
//                                 {
//                                     "terminationdate",
//                                     new List<object> { new DateTime(2012, 12, 19) }
//                                 },
//                                 { "homelocationid", new List<object> { 20 } },
//                                 { "officelocationid", new List<object> { 21 } },
//                                 { "name", new List<object> { "name" } },
//                                 {
//                                     "accountnumber", new List<object> { "accountnumber" }
//                                 },
//                                 { "accounttype", new List<object> { "accounttype" } },
//                                 { "sortcode", new List<object> { "sortcode" } },
//                                 { "reference", new List<object> { "reference" } },
//                                 { "localeID", new List<object> { 22 } },
//                                 { "NHSTrustID", new List<object> { 23 } },
//                                 { "logonCount", new List<object> { 24 } },
//                                 { "retryCount", new List<object> { 25 } },
//                                 { "firstLogon", new List<object> { false } },
//                                 { "licenceAttachID", new List<object> { 26 } },
//                                 { "defaultSubAccountId", new List<object> { 27 } },
//                                 { "CreationMethod", new List<object> { '4' } },
//                                 {
//                                     "mileagetotaldate",
//                                     new List<object> { new DateTime(2012, 12, 21) }
//                                 },
//                                 { "adminonly", new List<object> { false } },
//                                 { "locked", new List<object> { true } },
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Employee),  1);

//            Employee result = apiCrudService.DeleteEmployees("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields);
//        }

//        #endregion

//        #region esrAssignment

//        /// <summary>
//        /// The API crud create employee.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "esrAssignID", new List<object> { 1 } },
//                                 { "PayrollName", new List<object> { "Payrollname" } }
//                             };

//            var esrAssignment = new List<EsrAssignment>
//                                    {
//                                        new EsrAssignment
//                                            {
//                                                esrAssignID = 1,
//                                                PayrollName = "Payrollname"
//                                            }
//                                    };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignment));
//            List<EsrAssignment> result = apiCrudService.CreateEsrAssignments(
//                "metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrAssignment);
//            int i = 0;
//            foreach (EsrAssignment esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Assignment.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "esrAssignID", new List<object> { 1, 2 } },
//                                 { "PayrollName", new List<object> { "Payrollname", "another" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignment));
//            List<EsrAssignment> result = apiCrudService.ReadAllEsrAssignment("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (EsrAssignment employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Assignment.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "esrAssignID", new List<object> { 1 } },
//                                 { "PayrollName", new List<object> { "Payrollname" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignment));
//            EsrAssignment result = apiCrudService.ReadEsrAssignment("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields);
//        }

//        /// <summary>
//        /// The API crud Update ESR Assignment.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "esrAssignID", new List<object> { 1 } },
//                                 { "PayrollName", new List<object> { "Payrollname" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignment));
//            var esrAssignment = new List<EsrAssignment> { new EsrAssignment { esrAssignID = 1, PayrollName = "updated" } };

//            List<EsrAssignment> result = apiCrudService.UpdateEsrAssignments("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrAssignment);
//            int i = 0;
//            foreach (EsrAssignment esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Assignment.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "esrAssignID", new List<object> { 1 } },
//                                 { "PayrollName", new List<object> { "Payrollname" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignment),  1);

//            EsrAssignment result = apiCrudService.DeleteEsrAssignment("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields);
//        }

//        #endregion

//        #region esrPerson

//        /// <summary>
//        /// The API crud create employee.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPersonCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRPersonId", new List<object> { 1, 2 } },
//                                 { "FirstName", new List<object> { "firstname", "another" } },
//                                 { "MiddleNames", new List<object> { "middlename", "another" } },
//                                 { "LastName", new List<object> { "lastname", "more" } },
//                             };

//            var esrPerson = new List<EsrPerson>
//                                {
//                                    new EsrPerson
//                                        {
//                                            ESRPersonId = 1, FirstName = "firstname", MiddleNames = "middlename", LastName = "lastname"
//                                        }
//                                };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPerson));
//            List<EsrPerson> result = apiCrudService.CreateEsrPersons("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrPerson);
//            int i = 0;
//            foreach (EsrPerson esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Person.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPersonReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>();

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPerson));
//            List<EsrPerson> result = apiCrudService.ReadAllEsrPerson("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (EsrPerson employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Person.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPersonReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//{
//                                 { "ESRPersonId", new List<object> { 1L } },
//                                 { "FirstName", new List<object> { "firstname" } },
//                                 { "MiddleNames", new List<object> { "middlename" } },
//                                 { "LastName", new List<object> { "lastname" } },
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPerson));
//            EsrPerson result = apiCrudService.ReadEsrPerson("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields);
//        }

//        /// <summary>
//        /// The API crud Update ESR Person.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPersonUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//{
//                                 { "ESRPersonId", new List<object> { 1L } },
//                                 { "FirstName", new List<object> { "firstname" } },
//                                 { "MiddleNames", new List<object> { "middlename" } },
//                                 { "LastName", new List<object> { "lastname" } },
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPerson));
//            var esrPerson = new List<EsrPerson> { new EsrPerson { ESRPersonId = 1, FirstName = "updated", MiddleNames = "updated", LastName = "updated" } };

//            List<EsrPerson> result = apiCrudService.UpdateEsrPersons("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrPerson);
//            int i = 0;
//            foreach (EsrPerson esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Person.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPersonDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//{
//                                 { "ESRPersonId", new List<object> { 1L } },
//                                 { "FirstName", new List<object> { "firstname" } },
//                                 { "MiddleNames", new List<object> { "middlename" } },
//                                 { "LastName", new List<object> { "lastname" } },
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPerson),  1);

//            EsrPerson result = apiCrudService.DeleteEsrPerson("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields);
//        }

//        #endregion

//        #region esrPhone

//        /// <summary>
//        /// The API crud create employee.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPhoneCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRPhoneId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 10L } },
//                                 { "PhoneType", new List<object> { "mobile" } },
//                                 { "PhoneNumber", new List<object> { "1234" } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 }
//                             };

//            var esrPhone = new List<EsrPhone>
//                               {
//                                   new EsrPhone
//                                       {
//                                           ESRPhoneId = 1,
//                                           ESRPersonId = 10,
//                                           PhoneType = "mobile",
//                                           PhoneNumber = "1234"
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPhone));
//            List<EsrPhone> result = apiCrudService.CreateEsrPhones("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrPhone);
//            int i = 0;
//            foreach (EsrPhone esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Phone.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPhoneReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRPhoneId", new List<object> { 1L, 2L } },
//                                 { "ESRPersonId", new List<object> { 10L, 11L } },
//                                 { "PhoneType", new List<object> { "mobile", "home" } },
//                                 { "PhoneNumber", new List<object> { "1234", "4321" } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 12),
//                                             new DateTime(2012, 12, 15)
//                                         }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 13),
//                                             new DateTime(2012, 12, 16)
//                                         }
//                                 },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 14),
//                                             new DateTime(2012, 12, 17)
//                                         }
//                                 }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPhone));
//            List<EsrPhone> result = apiCrudService.ReadAllEsrPhone("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (EsrPhone employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Phone.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPhoneReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRPhoneId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 10L } },
//                                 { "PhoneType", new List<object> { "mobile" } },
//                                 { "PhoneNumber", new List<object> { "1234" } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPhone));
//            EsrPhone result = apiCrudService.ReadEsrPhone("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields);
//        }

//        /// <summary>
//        /// The API crud Update ESR Phone.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPhoneUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>();

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPhone));
//            var esrPhone = new List<EsrPhone> { new EsrPhone { ESRPhoneId = 1, ESRPersonId = 2, PhoneType = "mobile", PhoneNumber = "1234" } };

//            List<EsrPhone> result = apiCrudService.UpdateEsrPhones("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrPhone);
//            int i = 0;
//            foreach (EsrPhone esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Phone.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPhoneDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "ESRPhoneId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 10L } },
//                                 { "PhoneType", new List<object> { "mobile" } },
//                                 { "PhoneNumber", new List<object> { "1234" } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPhone), 1);

//            EsrPhone result = apiCrudService.DeleteEsrPhone("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields);
//        }

//        #endregion

//        #region esrVehicle

//        /// <summary>
//        /// The API crud create employee.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrVehicleCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRVehicleAllocationId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 3L } },
//                                 { "ESRAssignmentId", new List<object> { 5L } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "RegistrationNumber", new List<object> { "abc123" } },
//                                 { "Make", new List<object> { "make1" } },
//                                 { "Model", new List<object> { "make1" } },
//                                 { "Ownership", new List<object> { "make1" } },
//                                 {
//                                     "InitialRegistrationDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "EngineCC", new List<object> { 1900 } },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 }
//                             };

//            var esrVehicle = new List<EsrVehicle>
//                                 {
//                                     new EsrVehicle
//                                         {
//                                             ESRAssignmentId = 5,
//                                             ESRPersonId = 3,
//                                             ESRVehicleAllocationId = 1,
//                                             EffectiveStartDate = new DateTime(2012, 12, 12),
//                                             EffectiveEndDate = new DateTime(2012, 12, 12),
//                                             RegistrationNumber = "abc123",
//                                             Make = "make1",
//                                             Model = "make1",
//                                             Ownership = "make1",
//                                             InitialRegistrationDate =
//                                                 new DateTime(2012, 12, 12),
//                                             EngineCC = 1900,
//                                             ESRLastUpdate = new DateTime(2012, 12, 12)
//                                         }
//                                 };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrVehicle));
//            List<EsrVehicle> result = apiCrudService.CreateEsrVehicles("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrVehicle);
//            int i = 0;
//            foreach (EsrVehicle esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Vehicle.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrVehicleReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRVehicleAllocationId", new List<object> { 1L, 2L } },
//                                 { "ESRPersonId", new List<object> { 3L, 4L } },
//                                 { "ESRAssignmentId", new List<object> { 5L, 6L } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 12),
//                                             new DateTime(2012, 12, 15)
//                                         }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 12),
//                                             new DateTime(2012, 12, 15)
//                                         }
//                                 },
//                                 {
//                                     "RegistrationNumber",
//                                     new List<object> { "abc123", "123abc" }
//                                 },
//                                 { "Make", new List<object> { "make1", "make2" } },
//                                 { "Model", new List<object> { "make1", "make2" } },
//                                 { "Ownership", new List<object> { "make1", "make2" } },
//                                 {
//                                     "InitialRegistrationDate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 12),
//                                             new DateTime(2012, 12, 15)
//                                         }
//                                 },
//                                 { "EngineCC", new List<object> { 1900, 1800 } },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 12),
//                                             new DateTime(2012, 12, 15)
//                                         }
//                                 }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrVehicle));
//            List<EsrVehicle> result = apiCrudService.ReadAllEsrVehicle("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (EsrVehicle employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Vehicle.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrVehicleReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "ESRVehicleAllocationId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 3L } },
//                                 { "ESRAssignmentId", new List<object> { 5L } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "RegistrationNumber", new List<object> { "abc123" } },
//                                 { "Make", new List<object> { "make1" } },
//                                 { "Model", new List<object> { "make1" } },
//                                 { "Ownership", new List<object> { "make1" } },
//                                 {
//                                     "InitialRegistrationDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "EngineCC", new List<object> { 1900 } },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrVehicle));
//            EsrVehicle result = apiCrudService.ReadEsrVehicle("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields);
//        }

//        /// <summary>
//        /// The API crud Update ESR Vehicle.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrVehicleUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>();

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrVehicle));
//            var esrVehicle = new List<EsrVehicle> { new EsrVehicle { ESRPersonId = 1, ESRAssignmentId = 2, ESRVehicleAllocationId = 3, EngineCC = 1900 } };

//            List<EsrVehicle> result = apiCrudService.UpdateEsrVehicles("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrVehicle);
//            int i = 0;
//            foreach (EsrVehicle esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Vehicle.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrVehicleDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRVehicleAllocationId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 3L } },
//                                 { "ESRAssignmentId", new List<object> { 5L } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "RegistrationNumber", new List<object> { "abc123" } },
//                                 { "Make", new List<object> { "make1" } },
//                                 { "Model", new List<object> { "make1" } },
//                                 { "Ownership", new List<object> { "make1" } },
//                                 {
//                                     "InitialRegistrationDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 { "EngineCC", new List<object> { 1900 } },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrVehicle), 1);

//            EsrVehicle result = apiCrudService.DeleteEsrVehicle("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields);
//        }

//        #endregion

//        #region esrAddress

//        /// <summary>
//        /// The API crud create employee.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAddressCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                { "ESRAddressId", new List<object> { 1L, 2L } },
//                                 { "ESRPersonId", new List<object> { 10L, 11L } },
//                                 { "AddressType", new List<object> { "mobile", "home" } },
//                                 { "AddressStyle", new List<object> { "1234", "4321" } },
//                                 { "PrimaryFlag", new List<object> { "1234", "4321" } },
//                                 { "AddressLine1", new List<object> { "1234", "4321" } },
//                                 { "AddressLine2", new List<object> { "1234", "4321" } },
//                                 { "AddressLine3", new List<object> { "1234", "4321" } },
//                                 { "AddressTown", new List<object> { "1234", "4321" } },
//                                 { "AddressCounty", new List<object> { "1234", "4321" } },
//                                 { "AddressPostcode", new List<object> { "1234", "4321" } },
//                                 { "AddressCountry", new List<object> { "1234", "4321" } },
//                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
//                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
//                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
//                             };

//            var esrAddress = new List<EsrAddress>
//                                 {
//                                     new EsrAddress
//                                         {
//                                             ESRAddressId = 5,
//                                             ESRPersonId = 3,
//                                             EffectiveStartDate = new DateTime(2012, 12, 12),
//                                             EffectiveEndDate = new DateTime(2012, 12, 12),
//                                             ESRLastUpdate = new DateTime(2012, 12, 12)
//                                         }
//                                 };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAddress));
//            List<EsrAddress> result = apiCrudService.CreateEsrAddresss("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrAddress);
//            int i = 0;
//            foreach (EsrAddress esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Address.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAddressReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                { "ESRAddressId", new List<object> { 1L, 2L } },
//                                 { "ESRPersonId", new List<object> { 10L, 11L } },
//                                 { "AddressType", new List<object> { "mobile", "home" } },
//                                 { "AddressStyle", new List<object> { "1234", "4321" } },
//                                 { "PrimaryFlag", new List<object> { "1234", "4321" } },
//                                 { "AddressLine1", new List<object> { "1234", "4321" } },
//                                 { "AddressLine2", new List<object> { "1234", "4321" } },
//                                 { "AddressLine3", new List<object> { "1234", "4321" } },
//                                 { "AddressTown", new List<object> { "1234", "4321" } },
//                                 { "AddressCounty", new List<object> { "1234", "4321" } },
//                                 { "AddressPostcode", new List<object> { "1234", "4321" } },
//                                 { "AddressCountry", new List<object> { "1234", "4321" } },
//                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12), new DateTime(2012, 12, 15) } },
//                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13), new DateTime(2012, 12, 16) } },
//                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14), new DateTime(2012, 12, 17) } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAddress));
//            List<EsrAddress> result = apiCrudService.ReadAllEsrAddress("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (EsrAddress employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Address.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAddressReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                { "ESRAddressId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 10L } },
//                                 { "AddressType", new List<object> { "mobile" } },
//                                 { "AddressStyle", new List<object> { "1234" } },
//                                 { "PrimaryFlag", new List<object> { "1234" } },
//                                 { "AddressLine1", new List<object> { "1234" } },
//                                 { "AddressLine2", new List<object> { "1234" } },
//                                 { "AddressLine3", new List<object> { "1234" } },
//                                 { "AddressTown", new List<object> { "1234" } },
//                                 { "AddressCounty", new List<object> { "1234" } },
//                                 { "AddressPostcode", new List<object> { "1234" } },
//                                 { "AddressCountry", new List<object> { "1234" } },
//                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
//                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13) } },
//                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14) } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAddress));
//            EsrAddress result = apiCrudService.ReadEsrAddress("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields);
//        }

//        /// <summary>
//        /// The API crud Update ESR Address.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAddressUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>();

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAddress));
//            var esrAddress = new List<EsrAddress> { new EsrAddress { ESRPersonId = 1, ESRAddressId = 2, AddressLine1 = "line1" } };

//            List<EsrAddress> result = apiCrudService.UpdateEsrAddresss("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrAddress);
//            int i = 0;
//            foreach (EsrAddress esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Address.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAddressDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                               { "ESRAddressId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 10L } },
//                                 { "AddressType", new List<object> { "mobile" } },
//                                 { "AddressStyle", new List<object> { "1234" } },
//                                 { "PrimaryFlag", new List<object> { "1234" } },
//                                 { "AddressLine1", new List<object> { "1234" } },
//                                 { "AddressLine2", new List<object> { "1234" } },
//                                 { "AddressLine3", new List<object> { "1234" } },
//                                 { "AddressTown", new List<object> { "1234" } },
//                                 { "AddressCounty", new List<object> { "1234" } },
//                                 { "AddressPostcode", new List<object> { "1234" } },
//                                 { "AddressCountry", new List<object> { "1234" } },
//                                 { "EffectiveStartDate", new List<object> { new DateTime(2012, 12, 12) } },
//                                 { "EffectiveEndDate", new List<object> { new DateTime(2012, 12, 13) } },
//                                 { "ESRLastUpdate", new List<object> { new DateTime(2012, 12, 14) } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAddress), 1);

//            EsrAddress result = apiCrudService.DeleteEsrAddress("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields);
//        }

//        #endregion

//        #region esrAssignmentLocation

//        /// <summary>
//        /// The API crud create AssignmentLocation.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentLocationCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "EsrAssignId", new List<object> { 1 } },
//                                 { "EsrLocationId", new List<object> { 2 } },
//                                 {
//                                     "StartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 }
//                             };

//            var esrAssignmentLocations = new List<EsrAssignmentLocation>
//            {
//                new EsrAssignmentLocation
//                {
//                    Action = EsrGo2FromNhsWcfLibrary.Base.Action.Create,
//                    EsrAssignId = 1,
//                    EsrLocationId = 2,
//                    StartDate = new DateTime(2012, 12, 12)
//                }
//            };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentLocation));
//            List<EsrAssignmentLocation> result = apiCrudService.CreateAssignmentLocations("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrAssignmentLocations);
//            int i = 0;
//            foreach (EsrAssignmentLocation esrLoc in result)
//            {
//                Assert.IsTrue(esrLoc.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrLoc, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud create AssignmentLocation.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentLocationCreateMultiple()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "EsrAssignId", new List<object> { 1, 3 } },
//                                 { "EsrLocationId", new List<object> { 2, 4 } },
//                                 {
//                                     "StartDate",
//                                     new List<object> { new DateTime(2012, 12, 12), new DateTime(2011, 11, 11) }
//                                 }
//                             };

//            var esrAssignmentLocations = new List<EsrAssignmentLocation>
//            {
//                new EsrAssignmentLocation
//                {
//                    Action = EsrGo2FromNhsWcfLibrary.Base.Action.Create,
//                    EsrAssignId = 1,
//                    EsrLocationId = 2,
//                    StartDate = new DateTime(2012, 12, 12)
//                },
//                new EsrAssignmentLocation
//                {
//                    Action = EsrGo2FromNhsWcfLibrary.Base.Action.Create,
//                    EsrAssignId = 3,
//                    EsrLocationId = 4,
//                    StartDate = new DateTime(2011, 11, 11)
//                }
//            };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentLocation));
//            List<EsrAssignmentLocation> result = apiCrudService.CreateAssignmentLocations("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrAssignmentLocations);
//            int i = 0;
//            foreach (EsrAssignmentLocation esrLoc in result)
//            {
//                Assert.IsTrue(esrLoc.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrLoc, fields, i);
//                i += 1;
//            }
//        }

//        #endregion

//        #region esrAssignmentCostings

//        /// <summary>
//        /// The API crud create ASSIGNMENTCOSTINGS.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentCostingsCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRCostingAllocationId", new List<object> { 1 } },
//                                 { "ESRPersonId", new List<object> { 10L } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 }
//                             };

//            var esrAssignmentCostings = new List<EsrAssignmentCostings>
//                               {
//                                   new EsrAssignmentCostings
//                                       {
//                                           ESRCostingAllocationId = 1,
//                                           ESRPersonId = 10
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings));
//            List<EsrAssignmentCostings> result = apiCrudService.CreateEsrAssignmentCostingss("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrAssignmentCostings);
//            int i = 0;
//            foreach (EsrAssignmentCostings esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR ASSIGNMENTCOSTINGS.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentCostingsReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRCostingAllocationId", new List<object> { 1L, 2L } },
//                                 { "ESRPersonId", new List<object> { 10L, 11L } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 12),
//                                             new DateTime(2012, 12, 15)
//                                         }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 13),
//                                             new DateTime(2012, 12, 16)
//                                         }
//                                 },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object>
//                                         {
//                                             new DateTime(2012, 12, 14),
//                                             new DateTime(2012, 12, 17)
//                                         }
//                                 }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings));
//            List<EsrAssignmentCostings> result = apiCrudService.ReadAllEsrAssignmentCostings("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (EsrAssignmentCostings employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR ASSIGNMENTCOSTINGS.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentCostingsReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRCostingAllocationId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 10L } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings));
//            EsrAssignmentCostings result = apiCrudService.ReadEsrAssignmentCostings("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields);
//        }

//        /// <summary>
//        /// The API crud Update ESR ASSIGNMENTCOSTINGS.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentCostingsUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>();

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings));
//            var esrAssignmentCostings = new List<EsrAssignmentCostings> { new EsrAssignmentCostings { ESRCostingAllocationId = 1, ESRPersonId = 2 } };

//            List<EsrAssignmentCostings> result = apiCrudService.UpdateEsrAssignmentCostingss("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrAssignmentCostings);
//            int i = 0;
//            foreach (EsrAssignmentCostings esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR ASSIGNMENTCOSTINGS.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrAssignmentCostingsDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "ESRCostingAllocationId", new List<object> { 1L } },
//                                 { "ESRPersonId", new List<object> { 10L } },
//                                 {
//                                     "EffectiveStartDate",
//                                     new List<object> { new DateTime(2012, 12, 12) }
//                                 },
//                                 {
//                                     "EffectiveEndDate",
//                                     new List<object> { new DateTime(2012, 12, 13) }
//                                 },
//                                 {
//                                     "ESRLastUpdate",
//                                     new List<object> { new DateTime(2012, 12, 14) }
//                                 }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrAssignmentCostings), 1);

//            EsrAssignmentCostings result = apiCrudService.DeleteEsrAssignmentCostings("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields);
//        }

//        #endregion

//        #region EsrElementFields

//        /// <summary>
//        /// The API crud create ESRELEMENTFIELDS.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementFieldsCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "elementFieldID", new List<object> { 1 } },
//                                 { "elementID", new List<object> { 10 } }
//                             };

//            var esrElementFields = new List<EsrElementFields>
//                               {
//                                   new EsrElementFields
//                                       {
//                                           elementFieldID = 1,
//                                           elementID = 10
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementFields));
//            List<EsrElementFields> result = apiCrudService.CreateEsrElementFieldss("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrElementFields);
//            int i = 0;
//            foreach (EsrElementFields esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR ElementFields.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementFieldsReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "elementFieldID", new List<object> { 1 } },
//                                 { "elementID", new List<object> { 10 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementFields));
//            List<EsrElementFields> result = apiCrudService.ReadAllEsrElementFields("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (EsrElementFields employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR ElementFields.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementFieldsReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "elementFieldID", new List<object> { 1 } },
//                                 { "elementID", new List<object> { 10 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementFields));
//            EsrElementFields result = apiCrudService.ReadEsrElementFields("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields);
//        }

//        /// <summary>
//        /// The API crud Update ESR ElementFields.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementFieldsUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementFields));
//            var esrElementFields = new List<EsrElementFields>
//                                       {
//                                           new EsrElementFields
//                                               {
//                                                   elementFieldID = 1,
//                                                   elementID = 10
//                                               }
//                                       };

//            List<EsrElementFields> result = apiCrudService.UpdateEsrElementFieldss(
//                "metabase", GlobalTestVariables.AccountId.ToString(), esrElementFields);
//            int i = 0;
//            foreach (EsrElementFields esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR ElementFields.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementFieldsDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "elementFieldID", new List<object> { 1 } },
//                                 { "elementID", new List<object> { 10 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementFields), 1);

//            EsrElementFields result = apiCrudService.DeleteEsrElementFields("metabase", GlobalTestVariables.AccountId.ToString(), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region EsrElement

//        /// <summary>
//        /// The API crud create ESRElement.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "globalElementID", new List<object> { 1 } },
//                                 { "NHSTrustID", new List<object> { 10 } }
//                             };

//            var esrElement = new List<EsrElement>
//                               {
//                                   new EsrElement
//                                       {
//                                           globalElementID = 1,
//                                           NHSTrustID = 10
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElement));
//            List<EsrElement> result = apiCrudService.CreateEsrElements("metabase", GlobalTestVariables.AccountId.ToString(), esrElement);
//            int i = 0;
//            foreach (EsrElement esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Element.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "globalElementID", new List<object> { 1 } },
//                                 { "NHSTrustID", new List<object> { 10 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElement));
//            List<EsrElement> result = apiCrudService.ReadAllEsrElement("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (EsrElement employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Element.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "globalElementID", new List<object> { 1 } },
//                                 { "NHSTrustID", new List<object> { 10 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElement));
//            EsrElement result = apiCrudService.ReadEsrElement("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update ESR Element.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElement));
//            var esrElement = new List<EsrElement>
//                                       {
//                                           new EsrElement
//                                               {
//                                                   globalElementID = 1,
//                                                   NHSTrustID = 10
//                                               }
//                                       };

//            List<EsrElement> result = apiCrudService.UpdateEsrElements(
//                "metabase", GlobalTestVariables.AccountId.ToString(), esrElement);
//            int i = 0;
//            foreach (EsrElement esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Element.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "globalElementID", new List<object> { 1 } },
//                                 { "NHSTrustID", new List<object> { 10 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElement), 1);

//            EsrElement result = apiCrudService.DeleteEsrElement("metabase", GlobalTestVariables.AccountId.ToString(), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region EsrTrust

//        /// <summary>
//        /// The API crud create ESRTrust.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrTrustCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "trustVPD", new List<object> { "1" } },
//                                 { "runSequenceNumber", new List<object> { 10 } }
//                             };

//            var esrTrust = new List<EsrTrust>
//                               {
//                                   new EsrTrust
//                                       {
//                                           trustVPD = "1",
//                                           runSequenceNumber = 10
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrTrust));
//            List<EsrTrust> result = apiCrudService.CreateEsrTrusts("metabase", GlobalTestVariables.AccountId.ToString(), esrTrust);
//            int i = 0;
//            foreach (EsrTrust esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Trust.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrTrustReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "trustVPD", new List<object> { "1" } },
//                                 { "runSequenceNumber", new List<object> { 10 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrTrust));
//            List<EsrTrust> result = apiCrudService.ReadAllEsrTrust("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (EsrTrust employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Trust.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrTrustReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "trustVPD", new List<object> { "1" } },
//                                 { "runSequenceNumber", new List<object> { 10 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrTrust));
//            EsrTrust result = apiCrudService.ReadEsrTrust("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update ESR Trust.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrTrustUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrTrust));
//            var esrTrust = new List<EsrTrust>
//                                       {
//                                           new EsrTrust
//                                               {
//                                                   trustVPD = "1",
//                                                   runSequenceNumber = 10
//                                               }
//                                       };

//            List<EsrTrust> result = apiCrudService.UpdateEsrTrusts(
//                "metabase", GlobalTestVariables.AccountId.ToString(), esrTrust);
//            int i = 0;
//            foreach (EsrTrust esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Trust.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrTrustDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "trustVPD", new List<object> { "1" } },
//                                 { "runSequenceNumber", new List<object> { 10 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrTrust), 1);

//            EsrTrust result = apiCrudService.DeleteEsrTrust("metabase", GlobalTestVariables.AccountId.ToString(), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Find ESR Trust.
//        /// </summary>
//        //[TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        //public void ApiCrudFindEsrTrust()
//        //{
//        //    var apiCrudService = new ApiService(GetMockDataLog());
//        //    var fields = new SortedList<string, List<object>>
//        //                     {
//        //                          { "trustVPD", new List<object> { "127" } },
//        //                         { "runSequenceNumber", new List<object> { 10 } },
//        //                         { "trustName", new List<object>{ "trust name" } }
//        //                     };

//        //    var moqData = ApiDataBaseMoq.NormalDatabase(fields, typeof(EsrTrust), 1);
//        //    var cache = new MemoryCache("name")
//        //                    {
//        //                        {
//        //                            string.Format("Account_{0}", GlobalTestVariables.AccountId),
//        //                            new EsrTrust { trustVPD = "127", archived = false, trustName = "trust name", runSequenceNumber = 10 },
//        //                            new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) }
//        //                        },
//        //                        {
//        //                            "AccountList",
//        //                            new List<int> {GlobalTestVariables.AccountId},
//        //                            new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) }
//        //                        }
//        //                    };
//        //    moqData.Setup(x => x.Cache).Returns(cache);

//        //    apiCrudService.ApiDbConnection = moqData.Object;

//        //    EsrTrust result = apiCrudService.FindEsrTrust("metabase", "127");

//        //    Assert.IsTrue(result.AccountId == GlobalTestVariables.AccountId, "Should return global test account id");
            
//        //}

//        #endregion

//        #region EsrLocation

//        /// <summary>
//        /// The API crud create ESRLocation.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrLocationCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "Description", new List<object> { "1" } },
//                                 { "AddressLine1", new List<object> { "Line 1" } }
//                             };

//            var esrLocation = new List<EsrLocation>
//                               {
//                                   new EsrLocation
//                                       {
//                                           Description = "1",
//                                           AddressLine1 = "Line 1"
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrLocation));
//            var result = apiCrudService.CreateEsrLocations("metabase", GlobalTestVariables.AccountId.ToString(), esrLocation);
//            int i = 0;
//            foreach (EsrLocation esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Location.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrLocationReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "Description", new List<object> { "1" } },
//                                 { "AddressLine1", new List<object> { "Line 1" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrLocation));
//            List<EsrLocation> result = apiCrudService.ReadAllEsrLocation("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (EsrLocation employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Location.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrLocationReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "Description", new List<object> { "1" } },
//                                 { "AddressLine1", new List<object> { "Line 1" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrLocation));
//            EsrLocation result = apiCrudService.ReadEsrLocation("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update ESR Location.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrLocationUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrLocation));
//            var esrLocation = new List<EsrLocation>
//                                       {
//                                           new EsrLocation
//                                               {
//                                                   Description = "1",
//                                                    AddressLine1 = "Line 1"
//                                               }
//                                       };

//            List<EsrLocation> result = apiCrudService.UpdateEsrLocations(
//                "metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrLocation);
//            int i = 0;
//            foreach (EsrLocation esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Location.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrLocationDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "Description", new List<object> { "1" } },
//                                 { "AddressLine1", new List<object> { "Line 1" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrLocation), 1);

//            EsrLocation result = apiCrudService.DeleteEsrLocation("metabase", GlobalTestVariables.AccountId.ToString(), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region EsrOrganisation

//        /// <summary>
//        /// The API crud create ESROrganisation.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrOrganisationCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESROrganisationId", new List<object> { 1L } },
//                                 { "ESRLocationId", new List<object> { 2L } }
//                             };

//            var esrOrganisation = new List<EsrOrganisation>
//                               {
//                                   new EsrOrganisation
//                                       {
//                                           ESROrganisationId = 1,
//                                           ESRLocationId = 2,
//                                           EffectiveFrom = new DateTime(2012, 12, 12)
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation));
//            List<EsrOrganisation> result = apiCrudService.CreateEsrOrganisations("metabase", GlobalTestVariables.AccountId.ToString(), esrOrganisation);
//            int i = 0;
//            foreach (EsrOrganisation esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Organisation.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrOrganisationReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESROrganisationId", new List<object> { 1L } },
//                                 { "ESRLocationId", new List<object> { 2L } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation));
//            List<EsrOrganisation> result = apiCrudService.ReadAllEsrOrganisation("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (EsrOrganisation employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Organisation.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrOrganisationReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESROrganisationId", new List<object> { 1L } },
//                                 { "ESRLocationId", new List<object> { 2L } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation));
//            EsrOrganisation result = apiCrudService.ReadEsrOrganisation("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update ESR Organisation.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrOrganisationUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation));
//            var esrOrganisation = new List<EsrOrganisation>
//                                       {
//                                           new EsrOrganisation
//                                               {
//                                                   ESROrganisationId = 1,
//                                           ESRLocationId = 2,
//                                           EffectiveFrom = new DateTime(2012, 12, 12)
//                                               }
//                                       };

//            List<EsrOrganisation> result = apiCrudService.UpdateEsrOrganisations(
//                "metabase", GlobalTestVariables.AccountId.ToString(), esrOrganisation);
//            int i = 0;
//            foreach (EsrOrganisation esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Organisation.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrOrganisationDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "ESROrganisationId", new List<object> { 1L } },
//                                 { "ESRLocationId", new List<object> { 2L } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrOrganisation), 1);

//            EsrOrganisation result = apiCrudService.DeleteEsrOrganisation("metabase", GlobalTestVariables.AccountId.ToString(), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region EsrPosition

//        /// <summary>
//        /// The API crud create ESRPosition.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPositionCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRPositionId", new List<object> { 1L } },
//                                 { "EffectiveFromDate", new List<object> { new DateTime(2012, 12, 12) } }
//                             };

//            var esrPosition = new List<EsrPosition>
//                               {
//                                   new EsrPosition
//                                       {
//                                           ESRPositionId = 1,
//                                           EffectiveFromDate = new DateTime(2012, 12, 12)
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPosition));
//            List<EsrPosition> result = apiCrudService.CreateEsrPositions("metabase", GlobalTestVariables.AccountId.ToString(), esrPosition);
//            int i = 0;
//            foreach (EsrPosition esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR Position.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPositionReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRPositionId", new List<object> { 1L } },
//                                 { "EffectiveFromDate", new List<object> { new DateTime(2012, 12, 12) } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPosition));
//            List<EsrPosition> result = apiCrudService.ReadAllEsrPosition("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (EsrPosition employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR Position.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPositionReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "ESRPositionId", new List<object> { 1L } },
//                                 { "EffectiveFromDate", new List<object> { new DateTime(2012, 12, 12) } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPosition));
//            EsrPosition result = apiCrudService.ReadEsrPosition("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update ESR Position.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPositionUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPosition));
//            var esrPosition = new List<EsrPosition>
//                                       {
//                                           new EsrPosition
//                                               {
//                                                   ESRPositionId = 1,
//                                           EffectiveFromDate = new DateTime(2012, 12, 12)
//                                               }
//                                       };

//            List<EsrPosition> result = apiCrudService.UpdateEsrPositions(
//                "metabase", GlobalTestVariables.AccountId.ToString(), esrPosition);
//            int i = 0;
//            foreach (EsrPosition esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR Position.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrPositionDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "ESRPositionId", new List<object> { 1L } },
//                                 { "EffectiveFromDate", new List<object> { new DateTime(2012, 12, 12) } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrPosition), 1);

//            EsrPosition result = apiCrudService.DeleteEsrPosition("metabase", GlobalTestVariables.AccountId.ToString(), "1");
            
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region EsrElementSubCat

//        /// <summary>
//        /// The API crud create ESRElementSUBCAT.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementSubCatCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "elementSubcatID", new List<object> { 1, 2 } },
//                                 { "subcatID", new List<object> { 2, 3 } },
//                                 { "elementID", new List<object> { 2, 3 } }
//                             };

//            var esrElementSubCat = new List<EsrElementSubCat>
//                               {
//                                   new EsrElementSubCat
//                                       {
//                elementSubcatID = 1,
//                subcatID = 2,
//                elementID = 2
//            }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat));
//            List<EsrElementSubCat> result = apiCrudService.CreateEsrElementSubCats("metabase", GlobalTestVariables.AccountId.ToString(), esrElementSubCat);
//            int i = 0;
//            foreach (EsrElementSubCat esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR ElementSubCat.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementSubCatReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "elementSubcatID", new List<object> { 1, 2 } },
//                                 { "subcatID", new List<object> { 2, 3 } },
//                                 { "elementID", new List<object> { 2, 3 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat));
//            List<EsrElementSubCat> result = apiCrudService.ReadAllEsrElementSubCat("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (EsrElementSubCat employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR ElementSubCat.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementSubCatReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "elementSubcatID", new List<object> { 1, 2 } },
//                                 { "subcatID", new List<object> { 2, 3 } },
//                                 { "elementID", new List<object> { 2, 3 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat));
//            EsrElementSubCat result = apiCrudService.ReadEsrElementSubCat("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update ESR ElementSubCat.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementSubCatUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat));
//            var esrElementSubCat = new List<EsrElementSubCat>
//                                       {
//                                           new EsrElementSubCat
//                                               {
//                elementSubcatID = 1,
//                subcatID = 2,
//                elementID = 2
//            }
//                                       };

//            List<EsrElementSubCat> result = apiCrudService.UpdateEsrElementSubCats(
//                "metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), esrElementSubCat);
//            int i = 0;
//            foreach (EsrElementSubCat esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR ElementSubCat.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEsrElementSubCatDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "elementSubcatID", new List<object> { 1, 2 } },
//                                 { "subcatID", new List<object> { 2, 3 } },
//                                 { "elementID", new List<object> { 2, 3 } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EsrElementSubCat), 1);

//            EsrElementSubCat result = apiCrudService.DeleteEsrElementSubCat("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
            
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region Car

//        /// <summary>
//        /// The API crud create Car.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudCarCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "CarId", new List<object> { 1 } },
//                                 { "employeeid", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            var car = new List<Car>
//                               {
//                                   new Car
//                                       {
//                                           carid = 1,
//                                           employeeid = GlobalTestVariables.EmployeeId
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Car));
//            List<Car> result = apiCrudService.CreateCars("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), car);
//            int i = 0;
//            foreach (Car esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All Car.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudCarReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "carid", new List<object> { 1 } },
//                                 { "employeeid", new List<object> { GlobalTestVariables.EmployeeId, GlobalTestVariables.AlternativeEmployeeId } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Car));
//            List<Car> result = apiCrudService.ReadAllCar("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (Car employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read Car.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudCarReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "carid", new List<object> { 1 } },
//                                 { "employeeid", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Car));
//            Car result = apiCrudService.ReadCar("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update Car.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudCarUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Car));
//            var car = new List<Car>
//                                       {
//                                           new Car
//                                               {
//                                                   carid = 1,
//                                           employeeid = GlobalTestVariables.EmployeeId
//                                               }
//                                       };

//            List<Car> result = apiCrudService.UpdateCars(
//                "metabase", GlobalTestVariables.AccountId.ToString(), car);
//            int i = 0;
//            foreach (Car esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete Car.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudCarDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "CarId", new List<object> { 1 } },
//                                 { "employeeid", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Car), 1);

//            Car result = apiCrudService.DeleteCar("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
            
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region Address

//        /// <summary>
//        /// The API crud create Address.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudAddressCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "AddressID", new List<object> { 1 } },
//                                 { "Line1", new List<object> { "address1" } }
//                             };

//            var Address = new List<Address>
//                               {
//                                   new Address
//                                       {
//                                           AddressID = 1,
//                                           Line1 = "address1"
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Address));
//            List<Address> result = apiCrudService.CreateAddresses("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), Address);
//            int i = 0;
//            foreach (Address esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All Address.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudAddressReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "AddressID", new List<object> { 1 } },
//                                 { "Line1", new List<object> { "address1" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Address));
//            List<Address> result = apiCrudService.ReadAllAddresses("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (Address employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read Address.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudAddressReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "AddressID", new List<object> { 1 } },
//                                 { "Line1", new List<object> { "address1" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Address));
//            Address result = apiCrudService.ReadAddress("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update Address.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudAddressUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Address));
//            var Address = new List<Address>
//                                       {
//                                           new Address
//                                               {
//                                                   AddressID = 1,
//                                           Line1 = "address1"
//                                               }
//                                       };

//            List<Address> result = apiCrudService.UpdateAddresses(
//                "metabase", GlobalTestVariables.AccountId.ToString(), Address);
//            int i = 0;
//            foreach (Address esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete Address.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudAddressDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "AddressID", new List<object> { 1 } },
//                                 { "Line1", new List<object> { "address1" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Address), 1);

//            Address result = apiCrudService.DeleteAddresses("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region EmployeeHomeAddress

//        /// <summary>
//        /// The API crud create EmployeeHomeAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeHomeAddressCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "EmployeeHomeAddressId", new List<object> { 1 } },
//                                 { "EmployeeId", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            var EmployeeHomeAddress = new List<EmployeeHomeAddress>
//                               {
//                                   new EmployeeHomeAddress
//                                       {
//                                           EmployeeHomeAddressId = 1,
//                                           EmployeeId = GlobalTestVariables.EmployeeId
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress));
//            List<EmployeeHomeAddress> result = apiCrudService.CreateEmployeeHomeAddresses("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), EmployeeHomeAddress);
//            int i = 0;
//            foreach (EmployeeHomeAddress esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All EmployeeHomeAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeHomeAddressReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "EmployeeHomeAddressId", new List<object> { 1 } },
//                                 { "EmployeeId", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress));
//            List<EmployeeHomeAddress> result = apiCrudService.ReadAllEmployeeHomeAddresses("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (EmployeeHomeAddress employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read EmployeeHomeAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeHomeAddressReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "EmployeeHomeAddressId", new List<object> { 1 } },
//                                 { "EmployeeId", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress));
//            EmployeeHomeAddress result = apiCrudService.ReadEmployeeHomeAddress("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update EmployeeHomeAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeHomeAddressUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress));
//            var EmployeeHomeAddress = new List<EmployeeHomeAddress>
//                                       {
//                                           new EmployeeHomeAddress
//                                               {
//                                                   EmployeeHomeAddressId = 1,
//                                                    EmployeeId = GlobalTestVariables.EmployeeId
//                                               }
//                                       };

//            List<EmployeeHomeAddress> result = apiCrudService.UpdateEmployeeHomeAddresses(
//                "metabase", GlobalTestVariables.AccountId.ToString(), EmployeeHomeAddress);
//            int i = 0;
//            foreach (EmployeeHomeAddress esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete EmployeeHomeAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeHomeAddressDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "EmployeeHomeAddressId", new List<object> { 1 } },
//                                 { "EmployeeId", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeHomeAddress), 1);

//            EmployeeHomeAddress result = apiCrudService.DeleteEmployeeHomeAddresses("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region EmployeeWorkAddress

//        /// <summary>
//        /// The API crud create EmployeeWorkAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeWorkAddressCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "EmployeeWorkAddressId", new List<object> { 1 } },
//                                 { "EmployeeId", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            var EmployeeWorkAddress = new List<EmployeeWorkAddress>
//                               {
//                                   new EmployeeWorkAddress
//                                       {
//                                           EmployeeWorkAddressId = 1,
//                                           EmployeeId = GlobalTestVariables.EmployeeId
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeWorkAddress));
//            List<EmployeeWorkAddress> result = apiCrudService.CreateEmployeeWorkAddresses("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), EmployeeWorkAddress);
//            int i = 0;
//            foreach (EmployeeWorkAddress esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All EmployeeWorkAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeWorkAddressReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "EmployeeWorkAddressId", new List<object> { 1 } },
//                                 { "EmployeeId", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeWorkAddress));
//            List<EmployeeWorkAddress> result = apiCrudService.ReadAllEmployeeWorkAddresses("metabase", GlobalTestVariables.AccountId.ToString());
//            int i = 0;
//            foreach (EmployeeWorkAddress employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read EmployeeWorkAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeWorkAddressReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "EmployeeWorkAddressId", new List<object> { 1 } },
//                                 { "EmployeeId", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeWorkAddress));
//            EmployeeWorkAddress result = apiCrudService.ReadEmployeeWorkAddress("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update EmployeeWorkAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeWorkAddressUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeWorkAddress));
//            var EmployeeWorkAddress = new List<EmployeeWorkAddress>
//                                       {
//                                           new EmployeeWorkAddress
//                                               {
//                                                   EmployeeWorkAddressId = 1,
//                                                    EmployeeId = GlobalTestVariables.EmployeeId
//                                               }
//                                       };

//            List<EmployeeWorkAddress> result = apiCrudService.UpdateEmployeeWorkAddresses(
//                "metabase", GlobalTestVariables.AccountId.ToString(), EmployeeWorkAddress);
//            int i = 0;
//            foreach (EmployeeWorkAddress esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete EmployeeWorkAddress.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudEmployeeWorkAddressDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "EmployeeWorkAddressId", new List<object> { 1 } },
//                                 { "EmployeeId", new List<object> { GlobalTestVariables.EmployeeId } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(EmployeeWorkAddress), 1);

//            EmployeeWorkAddress result = apiCrudService.DeleteEmployeeWorkAddresses("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");

//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region TemplateMapping

//        /// <summary>
//        /// The API crud create TemplateMapping.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudTemplateMappingCreateSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "templateID", new List<object> { 1 } },
//                                 { "mandatory", new List<object> { false } }
//                             };

//            var templateMapping = new List<TemplateMapping>
//                               {
//                                   new TemplateMapping
//                                       {
//                                           templateID = 1,
//                                           mandatory = false
//                                       }
//                               };
//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(TemplateMapping));
//            List<TemplateMapping> result = apiCrudService.CreateTemplateMappings("metabase", GlobalTestVariables.AccountId.ToString(), templateMapping);
//            int i = 0;
//            foreach (TemplateMapping esrAss in result)
//            {
//                Assert.IsTrue(esrAss.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAss, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read All ESR TemplateMapping.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudTemplateMappingReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "templateID", new List<object> { 1 } },
//                                 { "mandatory", new List<object> { false } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(TemplateMapping));
//            List<TemplateMapping> result = apiCrudService.ReadAllTemplateMapping("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (TemplateMapping employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Read ESR TemplateMapping.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudTemplateMappingReadSingle()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "templateID", new List<object> { 1 } },
//                                 { "mandatory", new List<object> { false } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(TemplateMapping));
//            TemplateMapping result = apiCrudService.ReadTemplateMapping("metabase", GlobalTestVariables.AccountId.ToString(), "1");
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Success);
//            Helper.CheckFields(result, fields, 0);
//        }

//        /// <summary>
//        /// The API crud Update ESR TemplateMapping.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudTemplateMappingUpdate()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>> { };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(TemplateMapping));
//            var templateMapping = new List<TemplateMapping>
//                                       {
//                                           new TemplateMapping
//                                               {templateID = 1,
//                                                   mandatory = false
//                                               }
//                                       };

//            List<TemplateMapping> result = apiCrudService.UpdateTemplateMappings(
//                "metabase", GlobalTestVariables.AccountId.ToString(), templateMapping);
//            int i = 0;
//            foreach (TemplateMapping esrAssign in result)
//            {
//                Assert.IsTrue(esrAssign.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(esrAssign, fields, i);
//                i += 1;
//            }
//        }

//        /// <summary>
//        /// The API crud Delete ESR TemplateMapping.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudTemplateMappingDelete()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                  { "templateID", new List<object> { 1 } },
//                                 { "mandatory", new List<object> { false } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(TemplateMapping), 1);

//            TemplateMapping result = apiCrudService.DeleteTemplateMapping("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), "1");
           
//            Assert.IsTrue(result.ActionResult.Result == ApiActionResult.Deleted);
//            Helper.CheckFields(result, fields, 0);
//        }

//        #endregion

//        #region Field

//        /// <summary>
//        /// The API crud Read All  Field.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudFieldReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var fields = new SortedList<string, List<object>>
//                             {
//                                 { "templateID", new List<object> { 1 } },
//                                 { "mandatory", new List<object> { false } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(fields, typeof(Field));
//            List<Field> result = apiCrudService.ReadAllFields("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (Field employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, fields, i);
//                i += 1;
//            }
//        }
//        #endregion

//        #region AccountProperties
//        /// <summary>
//        /// The API crud Read All AccountProperty.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudAccountPropertyReadAll()
//        {
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var accountPropertys = new SortedList<string, List<object>>
//                             {
//                                 { "subAccountId", new List<object> { 1 } },
//                                 { "stringKey", new List<object> { "key1" } },
//                                 { "StringValue", new List<object> { "value" } }
//                             };

//            apiCrudService.ApiDbConnection = ApiDataBaseMoq.Database(accountPropertys, typeof(AccountProperty));
//            List<AccountProperty> result = apiCrudService.ReadAllAccountProperties("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture));
//            int i = 0;
//            foreach (AccountProperty employee in result)
//            {
//                Assert.IsTrue(employee.ActionResult.Result == ApiActionResult.Success);
//                Helper.CheckFields(employee, accountPropertys, i);
//                i += 1;
//            }
//        }
//        #endregion
//        #region LookupValue
//        /// <summary>
//        /// The API crud Read All AccountProperty.
//        /// </summary>
//        [TestMethod, TestCategory("APICRUD"), TestCategory("ApiService")]
//        public void ApiCrudLookupValue()
//        {
//            const string Tableid = "59EBDE1E-3E1D-408A-8D86-5C35CAF7FD5F"; // Esr Person
//            const string Fieldid = "D9141B31-B265-4ACE-9525-E18D1B76BB63"; // esr person id
//            const string KeyValue = "54321";
//            const long IDValue = 12345;
//            var dataTable = new DataTable();
//            dataTable.Columns.Add("columnName");
//            var dataRow = dataTable.NewRow();
//            dataTable.Rows.Add(dataRow);
//            dataRow = dataTable.NewRow();
//            dataTable.Rows.Add(dataRow);
                                            
//            var apiCrudService = new ApiService(GetMockDataLog());
//            var currentDatabase = new Mock<IApiDbConnection>();
//            currentDatabase.SetupAllProperties();
//            currentDatabase.Setup(x => x.ConnectionStringValid).Returns(true);
//            currentDatabase.Setup(x => x.GetAccountDetails(GlobalTestVariables.AccountId)).Returns(true);
//            var mockReader = new Mock<IDataReader>();
//            mockReader.Setup(reader => reader.Read()).Returns(true);
//            long key;
//            long.TryParse(KeyValue, out key);
//            mockReader.Setup(reader => reader.GetInt64(0)).Returns(IDValue);
//            mockReader.Setup(reader => reader.GetInt64(1)).Returns(key);
//            mockReader.Setup(reader => reader.GetValue(0)).Returns(IDValue);
//            mockReader.Setup(reader => reader.GetValue(1)).Returns(key);
//            mockReader.Setup(reader => reader.GetSchemaTable()).Returns(dataTable);
//            mockReader.Setup(reader => reader.FieldCount).Returns(2);
//            currentDatabase.Setup(x => x.Sqlexecute).Returns(new SqlCommand());

//            currentDatabase.Setup(x => x.GetStoredProcReader(It.IsAny<string>())).Returns(mockReader.Object);

//            currentDatabase.Setup(x => x.ExecuteProc(It.IsAny<string>())).Returns(1);

//            apiCrudService.ApiDbConnection = currentDatabase.Object;
//            Lookup result = apiCrudService.ReadLookupValue("metabase", GlobalTestVariables.AccountId.ToString(CultureInfo.InvariantCulture), Tableid, Fieldid, KeyValue);
//            Assert.IsTrue(result != null);
//            Assert.IsTrue(result.SecondColumnValue == key);
//            Assert.IsTrue(result.FirstColumnValue == IDValue);
//        }
//        #endregion

//        /// <summary>
//        /// The get mock data log.
//        /// </summary>
//        /// <returns>
//        /// The <see cref="IApiDbConnection"/>.
//        /// </returns>
//        private static IApiDbConnection GetMockDataLog()
//        {
//            var logDataMock = new Mock<IApiDbConnection>();
//            logDataMock.Setup(x => x.ConnectionStringValid).Returns(true);
//            logDataMock.Setup(x => x.DebugLevel).Returns(Log.MessageLevel.Debug);
//            logDataMock.Setup(x => x.Sqlexecute).Returns(new SqlCommand());
//            return logDataMock.Object;
//        }
    }
}
