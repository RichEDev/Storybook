// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Software Europe Ltd" file="cJoinsTest.cs">
//   Copyright (c) Software Europe Ltd. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// ReSharper disable InconsistentNaming
namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;

    /// <summary>
    /// This is a test class for cJoinsTest and is intended
    /// to contain all cJoinsTest Unit Tests
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestClass]
    public class cJoinsTest
    {
        #region testcontext
        private TestContext testContextInstance;

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
        #endregion testcontext

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region GetJoinViaSQL

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_JoinViaNull()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string>();

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkContext = trunk;
            ViaBranch trunkExpected = new ViaBranch("employees");
            ViaBranch contextBranch = trunkExpected;

            const bool Expected = false;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, null, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);
            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);

            foreach (Guid k in contextBranch.UnderBranches.Keys)
            {
                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);
                Assert.IsTrue(trunkContext.UnderBranches.ContainsKey(k));

                trunkContext = trunkContext.UnderBranches[k];
                contextBranch = contextBranch.UnderBranches[k];

                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);

                if (contextBranch == null && trunkContext == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_JoinViaJoinViaListNull()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            JoinVia joinVia = new JoinVia(0, "Created By", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"));

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string>();

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkContext = trunk;
            ViaBranch trunkExpected = new ViaBranch("employees");
            ViaBranch contextBranch = trunkExpected;

            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinVia, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);
            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);

            foreach (Guid k in contextBranch.UnderBranches.Keys)
            {
                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);
                Assert.IsTrue(trunkContext.UnderBranches.ContainsKey(k));

                trunkContext = trunkContext.UnderBranches[k];
                contextBranch = contextBranch.UnderBranches[k];

                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);

                if (contextBranch == null && trunkContext == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_EmployeesBaseTableToEmployeesViaCreatedBy()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField createdByField = fields.GetBy(employeesTable.TableID, "createdBy");
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            JoinVia joinVia = new JoinVia(0, "Created By", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string> { " LEFT JOIN [employees] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07] ON [employees].[CreatedBy] = [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid]" };

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkContext = trunk;
            ViaBranch trunkExpected = new ViaBranch("employees");
            ViaBranch contextBranch = trunkExpected;
            ViaBranch branch = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07");
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);
            contextBranch = trunkExpected;

            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinVia, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);
            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);
            Assert.AreEqual(expectedJoinStrings[0], joinStrings[0]);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);

            foreach (Guid k in contextBranch.UnderBranches.Keys)
            {
                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);
                Assert.IsTrue(trunkContext.UnderBranches.ContainsKey(k));

                trunkContext = trunkContext.UnderBranches[k];
                contextBranch = contextBranch.UnderBranches[k];

                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);

                if (contextBranch == null && trunkContext == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_EmployeesBaseTableToEmployeesViaCreatedByToEmployeesViaCreatedBy()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField createdByField = fields.GetBy(employeesTable.TableID, "createdBy");
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            JoinVia joinVia = new JoinVia(0, "Created By: Created By", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) }, { 1, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string> { " LEFT JOIN [employees] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07_1] ON [employees].[CreatedBy] = [141e6ea7-44e9-4449-a2a4-08bf52effc07_1].[employeeid] LEFT JOIN [employees] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07] ON [141e6ea7-44e9-4449-a2a4-08bf52effc07_1].[CreatedBy] = [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid]" };

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkContext = trunk;
            ViaBranch trunkExpected = new ViaBranch("employees");
            ViaBranch contextBranch = trunkExpected;
            ViaBranch branch = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07_1");
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);
            contextBranch = branch;
            branch = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07");
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);
            contextBranch = trunkExpected;
            
            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinVia, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);
            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);
            Assert.AreEqual(expectedJoinStrings[0], joinStrings[0]);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);

            foreach (Guid k in contextBranch.UnderBranches.Keys)
            {
                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);
                Assert.IsTrue(trunkContext.UnderBranches.ContainsKey(k));

                trunkContext = trunkContext.UnderBranches[k];
                contextBranch = contextBranch.UnderBranches[k];

                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);

                if (contextBranch == null && trunkContext == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_EmployeesBaseTableToEmployeesViaCreatedByToEmployeesViaCreatedByAndEmployeesBaseTableToEmployeesViaCreatedBy()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField createdByField = fields.GetBy(employeesTable.TableID, "createdBy");
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            JoinVia joinViaOne = new JoinVia(0, "Created By: Created By", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) }, { 1, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) } });

            JoinVia joinViaTwo = new JoinVia(0, "Created By", new Guid("96c76564-dba2-4eac-b716-f31a0b155494"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string> { " LEFT JOIN [employees] AS [96c76564-dba2-4eac-b716-f31a0b155494] ON [employees].[CreatedBy] = [96c76564-dba2-4eac-b716-f31a0b155494].[employeeid] LEFT JOIN [employees] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07] ON [96c76564-dba2-4eac-b716-f31a0b155494].[CreatedBy] = [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid]", string.Empty };

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkContext = trunk;
            ViaBranch trunkExpected = new ViaBranch("employees");
            ViaBranch contextBranch = trunkExpected;
            ViaBranch branch = new ViaBranch("96c76564-dba2-4eac-b716-f31a0b155494");
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);
            contextBranch = branch;
            branch = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07");
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);
            contextBranch = trunkExpected;

            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaTwo, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);
            Assert.AreEqual(expectedJoinStrings[0], joinStrings[0]);
            Assert.AreEqual(expectedJoinStrings[1], joinStrings[1]);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);

            foreach (Guid k in contextBranch.UnderBranches.Keys)
            {
                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);
                Assert.IsTrue(trunkContext.UnderBranches.ContainsKey(k));

                trunkContext = trunkContext.UnderBranches[k];
                contextBranch = contextBranch.UnderBranches[k];

                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);

                if (contextBranch == null && trunkContext == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_EmployeesBaseTableToEmployeesViaCreatedByAndEmployeesBaseTableToEmployeesViaCreatedByToEmployeesViaCreatedBy()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField createdByField = fields.GetBy(employeesTable.TableID, "createdBy");
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            JoinVia joinViaOne = new JoinVia(0, "Created By", new Guid("96c76564-dba2-4eac-b716-f31a0b155494"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) } });

            JoinVia joinViaTwo = new JoinVia(0, "Created By: Created By", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) }, { 1, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string> { " LEFT JOIN [employees] AS [96c76564-dba2-4eac-b716-f31a0b155494] ON [employees].[CreatedBy] = [96c76564-dba2-4eac-b716-f31a0b155494].[employeeid]", " LEFT JOIN [employees] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07] ON [96c76564-dba2-4eac-b716-f31a0b155494].[CreatedBy] = [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid]" };

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkContext = trunk;
            ViaBranch trunkExpected = new ViaBranch("employees");
            ViaBranch contextBranch = trunkExpected;
            ViaBranch branch = new ViaBranch("96c76564-dba2-4eac-b716-f31a0b155494");
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);
            contextBranch = branch;
            branch = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07");
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);
            contextBranch = trunkExpected;

            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaTwo, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);
            Assert.AreEqual(expectedJoinStrings[0], joinStrings[0]);
            Assert.AreEqual(expectedJoinStrings[1], joinStrings[1]);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);

            foreach (Guid k in contextBranch.UnderBranches.Keys)
            {
                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);
                Assert.IsTrue(trunkContext.UnderBranches.ContainsKey(k));

                trunkContext = trunkContext.UnderBranches[k];
                contextBranch = contextBranch.UnderBranches[k];

                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);

                if (contextBranch == null && trunkContext == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_EmployeesBaseTableToEmployeesUserdefined()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cTable employeesUserdefinedTable = employeesTable.GetUserdefinedTable();
            List<cField> employeesUserdefinedFields = fields.GetFieldsByTableID(employeesUserdefinedTable.TableID);

            if (employeesUserdefinedFields.Count == 0)
            {
                Assert.Inconclusive("The test cannot be completed without at least one employee userdefined field.");
            }

            cField employeesUserdefinedField = employeesUserdefinedFields[0];
            if (employeesUserdefinedFields.Count > 1)
            {
                foreach (cField userdefinedField in employeesUserdefinedFields.Where(userdefinedField => userdefinedField.IsForeignKey != true && userdefinedField.FieldID != employeesUserdefinedTable.PrimaryKeyID))
                {
                    employeesUserdefinedField = userdefinedField;
                }
            }

            JoinVia joinViaOne = new JoinVia(0, "User Defined Fields", new Guid("96c76564-dba2-4eac-b716-f31a0b155494"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(employeesUserdefinedTable.TableID, JoinViaPart.IDType.Table) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string> { " LEFT JOIN [userdefined_employees] AS [96c76564-dba2-4eac-b716-f31a0b155494] ON [employees].[employeeid] = [96c76564-dba2-4eac-b716-f31a0b155494].[employeeid]" };

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkContext = trunk;
            ViaBranch trunkExpected = new ViaBranch("employees");
            ViaBranch contextBranch = trunkExpected;
            ViaBranch branch = new ViaBranch("96c76564-dba2-4eac-b716-f31a0b155494");
            contextBranch.UnderBranches.Add(employeesUserdefinedTable.TableID, branch);
            contextBranch = trunkExpected;

            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, employeesUserdefinedField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);
            Assert.AreEqual(expectedJoinStrings[0], joinStrings[0]);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);

            foreach (Guid k in contextBranch.UnderBranches.Keys)
            {
                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);
                Assert.IsTrue(trunkContext.UnderBranches.ContainsKey(k));

                trunkContext = trunkContext.UnderBranches[k];
                contextBranch = contextBranch.UnderBranches[k];

                Assert.AreEqual(trunkContext.TableName, contextBranch.TableName);

                if (contextBranch == null && trunkContext == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_IncorrectJoinTableEntrySavedExpensesCurrentBaseTableToEmployeesAndSavedExpensesCurrentBaseTableToEmployeesUserdefined()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable savedExpensesTable = tables.GetTableByID(new Guid("D70D9E5F-37E2-4025-9492-3BCF6AA746A8"));
            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField usernameField = fields.GetBy(employeesTable.TableID, "username"); 
            cTable employeesUserdefinedTable = employeesTable.GetUserdefinedTable();
            List<cField> employeesUserdefinedFields = fields.GetFieldsByTableID(employeesUserdefinedTable.TableID);

            if (employeesUserdefinedFields.Count == 0)
            {
                Assert.Inconclusive("The test cannot be completed without at least one employee userdefined field.");
            }

            cField employeesUserdefinedField = employeesUserdefinedFields[0];
            if (employeesUserdefinedFields.Count > 1)
            {
                foreach (cField userdefinedField in employeesUserdefinedFields.Where(userdefinedField => userdefinedField.IsForeignKey != true && userdefinedField.FieldID != employeesUserdefinedTable.PrimaryKeyID))
                {
                    employeesUserdefinedField = userdefinedField;
                }
            }

            JoinVia joinViaOne = new JoinVia(0, "Employees", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(employeesTable.TableID, JoinViaPart.IDType.Table) } });

            JoinVia joinViaTwo = new JoinVia(0, "Employees: User Defined Fields", new Guid("96c76564-dba2-4eac-b716-f31a0b155494"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(employeesTable.TableID, JoinViaPart.IDType.Table) }, { 1, new JoinViaPart(employeesUserdefinedTable.TableID, JoinViaPart.IDType.Table) } });

            List<string> joinStrings = new List<string>();

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkExpected = new ViaBranch("employees");
            ViaBranch contextBranch = trunkExpected;
            ViaBranch branch = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07");
            contextBranch.UnderBranches.Add(employeesTable.TableID, branch);
            contextBranch = branch;
            branch = new ViaBranch("96c76564-dba2-4eac-b716-f31a0b155494");
            contextBranch.UnderBranches.Add(employeesUserdefinedTable.TableID, branch);

            const bool Expected = false;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, usernameField, savedExpensesTable.TableID);

            Assert.AreEqual(Expected, actual); // will be false as the savedexpenses doesn't have a properly set up joinbreakdown (the steps don't have destinationkeys) -- remove test if they're fixed or change to another broken one

            actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaTwo, employeesUserdefinedField, savedExpensesTable.TableID);

            Assert.AreEqual(Expected, actual);
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_TeamsBaseTableToEmployeesAndTeamsBaseTableToEmployeesUserdefined()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable teamsTable = tables.GetTableByID(new Guid("FA495951-4D06-49AD-9F85-D67F9EFF4A27"));
            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");
            cTable employeesUserdefinedTable = employeesTable.GetUserdefinedTable();
            List<cField> employeesUserdefinedFields = fields.GetFieldsByTableID(employeesUserdefinedTable.TableID);

            if (employeesUserdefinedFields.Count == 0)
            {
                Assert.Inconclusive("The test cannot be completed without at least one employee userdefined field.");
            }

            cField employeesUserdefinedField = employeesUserdefinedFields[0];
            if (employeesUserdefinedFields.Count > 1)
            {
                foreach (cField userdefinedField in employeesUserdefinedFields.Where(userdefinedField => userdefinedField.IsForeignKey != true && userdefinedField.FieldID != employeesUserdefinedTable.PrimaryKeyID))
                {
                    employeesUserdefinedField = userdefinedField;
                }
            }

            JoinVia joinViaOne = new JoinVia(0, "Employees", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(employeesTable.TableID, JoinViaPart.IDType.Table) } });

            JoinVia joinViaTwo = new JoinVia(0, "Employees: User Defined Fields", new Guid("96c76564-dba2-4eac-b716-f31a0b155494"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(employeesTable.TableID, JoinViaPart.IDType.Table) }, { 1, new JoinViaPart(employeesUserdefinedTable.TableID, JoinViaPart.IDType.Table) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string> { " LEFT JOIN [teamemps] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07_1] ON [teams].[teamid] = [141e6ea7-44e9-4449-a2a4-08bf52effc07_1].[teamid] LEFT JOIN [employees] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07] ON [141e6ea7-44e9-4449-a2a4-08bf52effc07_1].[employeeid] = [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid]", " LEFT JOIN [userdefined_employees] AS [96c76564-dba2-4eac-b716-f31a0b155494] ON [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid] = [96c76564-dba2-4eac-b716-f31a0b155494].[employeeid]" };

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees"); // start the tree with the base table
            ViaBranch treeContext = trunk; // get a renamed handle for later tree navigation and comparisons, as - though it starts there - it will not stay at the "trunk"

            ViaBranch trunkExpected = new ViaBranch("employees");                       // START the expected tree at the base table
            ViaBranch contextBranch = trunkExpected;                                    // SET CONTEXT - this is a reference to the current point we are at in the join; at the start, the trunk
            ViaBranch branch = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07");   // create a new branch to store on the tree with its AS name, this is a new step in the join
            contextBranch.UnderBranches.Add(employeesTable.TableID, branch);            // ADD BRANCH - put the new branch into the tree under the current context,
                                                                                        //              indexed either by its field id for a field join
                                                                                        //              or by its endpoint table id for a table join
            contextBranch = branch;                                                     // SET CONTEXT
            branch = new ViaBranch("96c76564-dba2-4eac-b716-f31a0b155494");             // create a new branch
            contextBranch.UnderBranches.Add(employeesUserdefinedTable.TableID, branch); // ADD BRANCH
            contextBranch = trunkExpected;                                              // SET CONTEXT back to the trunk ready for later comparisons

            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, usernameField, teamsTable.TableID);

            Assert.AreEqual(Expected, actual);

            actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaTwo, employeesUserdefinedField, teamsTable.TableID);

            Assert.AreEqual(Expected, actual);

            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);
            Assert.AreEqual(expectedJoinStrings[0], joinStrings[0]);
            Assert.AreEqual(expectedJoinStrings[1], joinStrings[1]);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);

            foreach (Guid k in contextBranch.UnderBranches.Keys)
            {
                Assert.AreEqual(treeContext.TableName, contextBranch.TableName);
                Assert.IsTrue(treeContext.UnderBranches.ContainsKey(k));

                treeContext = treeContext.UnderBranches[k];
                contextBranch = contextBranch.UnderBranches[k];

                Assert.AreEqual(treeContext.TableName, contextBranch.TableName);

                if (contextBranch == null && treeContext == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_TeamsBaseTableToEmployeesUserdefinedAndTeamsBaseTableToEmployees()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable teamsTable = tables.GetTableByID(new Guid("FA495951-4D06-49AD-9F85-D67F9EFF4A27"));
            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");
            cTable employeesUserdefinedTable = employeesTable.GetUserdefinedTable();
            List<cField> employeesUserdefinedFields = fields.GetFieldsByTableID(employeesUserdefinedTable.TableID);

            if (employeesUserdefinedFields.Count == 0)
            {
                Assert.Inconclusive("The test cannot be completed without at least one employee userdefined field.");
            }

            cField employeesUserdefinedField = employeesUserdefinedFields[0];
            if (employeesUserdefinedFields.Count > 1)
            {
                foreach (cField userdefinedField in employeesUserdefinedFields.Where(userdefinedField => userdefinedField.IsForeignKey != true && userdefinedField.FieldID != employeesUserdefinedTable.PrimaryKeyID))
                {
                    employeesUserdefinedField = userdefinedField;
                }
            }

            JoinVia joinViaOne = new JoinVia(0, "Employees: User Defined Fields", new Guid("96c76564-dba2-4eac-b716-f31a0b155494"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(employeesTable.TableID, JoinViaPart.IDType.Table) }, { 1, new JoinViaPart(employeesUserdefinedTable.TableID, JoinViaPart.IDType.Table) } });

            JoinVia joinViaTwo = new JoinVia(0, "Employees", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(employeesTable.TableID, JoinViaPart.IDType.Table) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string> { " LEFT JOIN [teamemps] AS [96c76564-dba2-4eac-b716-f31a0b155494_1] ON [teams].[teamid] = [96c76564-dba2-4eac-b716-f31a0b155494_1].[teamid] LEFT JOIN [employees] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07] ON [96c76564-dba2-4eac-b716-f31a0b155494_1].[employeeid] = [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid] LEFT JOIN [userdefined_employees] AS [96c76564-dba2-4eac-b716-f31a0b155494] ON [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid] = [96c76564-dba2-4eac-b716-f31a0b155494].[employeeid]", string.Empty };

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees"); // start the tree with the base table
            ViaBranch treeContext = trunk; // get a renamed handle for later tree navigation and comparisons, as - though it starts there - it will not stay at the "trunk"

            ViaBranch trunkExpected = new ViaBranch("employees");                       // START the expected tree at the base table
            ViaBranch contextBranch = trunkExpected;                                    // SET CONTEXT - this is a reference to the current point we are at in the join; at the start, the trunk
            ViaBranch branch = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07");   // create a new branch to store on the tree with its AS name, this is a new step in the join
            contextBranch.UnderBranches.Add(employeesTable.TableID, branch);            // ADD BRANCH - put the new branch into the tree under the current context,
            //              indexed either by its field id for a field join
            //              or by its endpoint table id for a table join
            contextBranch = branch;                                                     // SET CONTEXT
            branch = new ViaBranch("96c76564-dba2-4eac-b716-f31a0b155494");             // create a new branch
            contextBranch.UnderBranches.Add(employeesUserdefinedTable.TableID, branch); // ADD BRANCH
            contextBranch = trunkExpected;                                              // SET CONTEXT back to the trunk ready for later comparisons

            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, employeesUserdefinedField, teamsTable.TableID);

            Assert.AreEqual(Expected, actual);

            actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaTwo, usernameField, teamsTable.TableID);

            Assert.AreEqual(Expected, actual);

            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);
            Assert.AreEqual(expectedJoinStrings[0], joinStrings[0]);
            Assert.AreEqual(expectedJoinStrings[1], joinStrings[1]);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);

            foreach (Guid k in contextBranch.UnderBranches.Keys)
            {
                Assert.AreEqual(treeContext.TableName, contextBranch.TableName);
                Assert.IsTrue(treeContext.UnderBranches.ContainsKey(k));

                treeContext = treeContext.UnderBranches[k];
                contextBranch = contextBranch.UnderBranches[k];

                Assert.AreEqual(treeContext.TableName, contextBranch.TableName);

                if (contextBranch == null && treeContext == null)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_NoJoinForCostCodesBaseTableToEmployees()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable costCodesTable = tables.GetTableByID(new Guid("02009E21-AA1D-4E0D-908A-4E9D73DDFBDF"));
            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            JoinVia joinViaOne = new JoinVia(0, "Employees", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(employeesTable.TableID, JoinViaPart.IDType.Table) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string>();

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("cost_codes"); // start the tree with the base table

            ViaBranch trunkExpected = new ViaBranch("cost_codes");                       // START the expected tree at the base table

            const bool Expected = false;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, usernameField, costCodesTable.TableID);

            Assert.AreEqual(Expected, actual);

            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);

            cCompareAssert.AreEqual(trunkExpected, trunk);
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_IncorrectTrunkForCostCodesBaseTableToEmployees()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable costCodesTable = tables.GetTableByID(new Guid("02009E21-AA1D-4E0D-908A-4E9D73DDFBDF"));
            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            JoinVia joinViaOne = new JoinVia(0, "Employees", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(employeesTable.TableID, JoinViaPart.IDType.Table) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string>();

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch(); // start the tree with the base table
            
            ViaBranch trunkExpected = new ViaBranch("costcodes");                       // START the expected tree at the base table
            
            const bool Expected = false;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, usernameField, costCodesTable.TableID);

            Assert.AreEqual(Expected, actual);

            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);

            Assert.AreNotEqual(trunkExpected.TableName, trunk.TableName);
            cCompareAssert.AreEqual(trunkExpected.UnderBranches, trunk.UnderBranches);
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_EmployeesBaseTableToEmployeesViaNonExistantField()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            JoinVia joinVia = new JoinVia(0, "Team Id", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(Guid.Empty, JoinViaPart.IDType.Field) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string>();

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkExpected = new ViaBranch("employees");

            const bool Expected = false;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinVia, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);
            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);

            Assert.AreEqual(trunkExpected.TableName, trunk.TableName);
            Assert.AreEqual(trunkExpected.UnderBranches.Count, trunk.UnderBranches.Count);
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_EmployeesBaseTableToEmployeesViaNonForeignKey()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");
            cField teamdIdField = fields.GetFieldByID(new Guid("E0E2C87D-0F15-4386-8BEF-000BA43A9840"));

            JoinVia joinVia = new JoinVia(0, "Team Id", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(teamdIdField.FieldID, JoinViaPart.IDType.Field) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string>();

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");
            ViaBranch trunkExpected = new ViaBranch("employees");

            const bool Expected = false;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinVia, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);
            Assert.AreEqual(expectedJoinStrings.Count, joinStrings.Count);

            cCompareAssert.AreEqual(trunkExpected, trunk);
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_EmployeesBaseTableToEmployeesViaCreatedByAndEmployeesBaseTableToEmployeesViaModifedByAndEmployeesBaseTableToEmployeesViaCreatedByToEmployeesUserdefined()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField modifiedByField = fields.GetBy(employeesTable.TableID, "modifiedby");
            cField createdByField = fields.GetBy(employeesTable.TableID, "createdby");
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");
            cTable employeesUserdefinedTable = employeesTable.GetUserdefinedTable();
            List<cField> employeesUserdefinedFields = fields.GetFieldsByTableID(employeesUserdefinedTable.TableID);

            if (employeesUserdefinedFields.Count == 0)
            {
                Assert.Inconclusive("The test cannot be completed without at least one employee userdefined field.");
            }

            cField employeesUserdefinedField = employeesUserdefinedFields[0];
            if (employeesUserdefinedFields.Count > 1)
            {
                foreach (cField userdefinedField in employeesUserdefinedFields.Where(userdefinedField => userdefinedField.IsForeignKey != true && userdefinedField.FieldID != employeesUserdefinedTable.PrimaryKeyID))
                {
                    employeesUserdefinedField = userdefinedField;
                }
            }

            JoinVia joinViaOne = new JoinVia(0, "Created By", new Guid("c62e0b10-2e84-42a7-9884-fa0f4ecdc82d"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) } });

            JoinVia joinViaTwo = new JoinVia(0, "Modified By", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(modifiedByField.FieldID, JoinViaPart.IDType.Field) } });

            JoinVia joinViaThree = new JoinVia(0, "Created By: User Defined Fields", new Guid("96c76564-dba2-4eac-b716-f31a0b155494"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) }, { 1, new JoinViaPart(employeesUserdefinedTable.TableID, JoinViaPart.IDType.Table) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string> { " LEFT JOIN [employees] AS [c62e0b10-2e84-42a7-9884-fa0f4ecdc82d] ON [employees].[CreatedBy] = [c62e0b10-2e84-42a7-9884-fa0f4ecdc82d].[employeeid]", " LEFT JOIN [employees] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07] ON [employees].[ModifiedBy] = [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid]", " LEFT JOIN [userdefined_employees] AS [96c76564-dba2-4eac-b716-f31a0b155494] ON [c62e0b10-2e84-42a7-9884-fa0f4ecdc82d].[employeeid] = [96c76564-dba2-4eac-b716-f31a0b155494].[employeeid]" };

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");   // start the tree with the base table

            ViaBranch trunkExpected = new ViaBranch("employees");                       // START the expected tree at the base table
            ViaBranch contextBranch = trunkExpected;                                    // SET CONTEXT - this is a reference to the current point we are at in the join; at the start, the trunk
            ViaBranch branch = new ViaBranch("c62e0b10-2e84-42a7-9884-fa0f4ecdc82d");   // create a new branch to store on the tree with its AS name, this is a new step in the join
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);            // ADD BRANCH - put the new branch into the tree under the current context,
                                                                                        //              indexed either by its field id for a field join
                                                                                        //              or by its endpoint table id for a table join
            ViaBranch branch2 = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07");  // create a new branch
            contextBranch.UnderBranches.Add(modifiedByField.FieldID, branch2);          // ADD BRANCH
            contextBranch = branch;                                                     // SET CONTEXT to previous branch (createdby)
            branch = new ViaBranch("96c76564-dba2-4eac-b716-f31a0b155494");             // create a new branch
            contextBranch.UnderBranches.Add(employeesUserdefinedTable.TableID, branch); // ADD BRANCH

            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaTwo, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaThree, employeesUserdefinedField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            cCompareAssert.AreEqual(expectedJoinStrings, joinStrings);
            cCompareAssert.AreEqual(trunkExpected, trunk);
        }

        /// <summary>
        /// A test for GetJoinViaSQL
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        [TestCategory("Spend Management"), TestCategory("Greenlight"), TestCategory("Query Builder")]
        public void cJoins_GetJoinViaSQL_EmployeesBaseTableToEmployeesViaCreatedByAndEmployeesBaseTableToEmployeesViaModifedByAndEmployeesBaseTableToEmployeesViaCreatedByToEmployeesViaCreatedBy()
        {
            ICurrentUserBase currentUserBase = Moqs.CurrentUser();
            cTables tables = new cTables(currentUserBase.AccountID);
            cFields fields = new cFields(currentUserBase.AccountID);
            cJoins joins = new cJoins(currentUserBase.AccountID);

            cTable employeesTable = tables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            cField modifiedByField = fields.GetBy(employeesTable.TableID, "modifiedby");
            cField createdByField = fields.GetBy(employeesTable.TableID, "createdby");
            cField usernameField = fields.GetBy(employeesTable.TableID, "username");

            JoinVia joinViaOne = new JoinVia(0, "Created By", new Guid("c62e0b10-2e84-42a7-9884-fa0f4ecdc82d"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) } });

            JoinVia joinViaTwo = new JoinVia(0, "Modified By", new Guid("141e6ea7-44e9-4449-a2a4-08bf52effc07"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(modifiedByField.FieldID, JoinViaPart.IDType.Field) } });

            JoinVia joinViaThree = new JoinVia(0, "Created By: Created By", new Guid("96c76564-dba2-4eac-b716-f31a0b155494"), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) }, { 1, new JoinViaPart(createdByField.FieldID, JoinViaPart.IDType.Field) } });

            List<string> joinStrings = new List<string>();
            List<string> expectedJoinStrings = new List<string> { " LEFT JOIN [employees] AS [c62e0b10-2e84-42a7-9884-fa0f4ecdc82d] ON [employees].[CreatedBy] = [c62e0b10-2e84-42a7-9884-fa0f4ecdc82d].[employeeid]", " LEFT JOIN [employees] AS [141e6ea7-44e9-4449-a2a4-08bf52effc07] ON [employees].[ModifiedBy] = [141e6ea7-44e9-4449-a2a4-08bf52effc07].[employeeid]", " LEFT JOIN [employees] AS [96c76564-dba2-4eac-b716-f31a0b155494] ON [c62e0b10-2e84-42a7-9884-fa0f4ecdc82d].[CreatedBy] = [96c76564-dba2-4eac-b716-f31a0b155494].[employeeid]" };

            // this tests the path finding mechanism is generating correctly
            ViaBranch trunk = new ViaBranch("employees");   // start the tree with the base table

            ViaBranch trunkExpected = new ViaBranch("employees");                       // START the expected tree at the base table
            ViaBranch contextBranch = trunkExpected;                                    // SET CONTEXT - this is a reference to the current point we are at in the join; at the start, the trunk
            ViaBranch branch = new ViaBranch("c62e0b10-2e84-42a7-9884-fa0f4ecdc82d");   // create a new branch to store on the tree with its AS name, this is a new step in the join
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);            // ADD BRANCH - put the new branch into the tree under the current context,
                                                                                        //              indexed either by its field id for a field join
                                                                                        //              or by its endpoint table id for a table join
            ViaBranch branch2 = new ViaBranch("141e6ea7-44e9-4449-a2a4-08bf52effc07");  // create a new branch
            contextBranch.UnderBranches.Add(modifiedByField.FieldID, branch2);          // ADD BRANCH
            contextBranch = branch;                                                     // SET CONTEXT to previous branch (createdby)
            branch = new ViaBranch("96c76564-dba2-4eac-b716-f31a0b155494");             // create a new branch
            contextBranch.UnderBranches.Add(createdByField.FieldID, branch);            // ADD BRANCH

            const bool Expected = true;
            bool actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaOne, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaTwo, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            actual = joins.GetJoinViaSQL(ref joinStrings, ref trunk, joinViaThree, usernameField, employeesTable.TableID);

            Assert.AreEqual(Expected, actual);

            cCompareAssert.AreEqual(expectedJoinStrings, joinStrings);
            cCompareAssert.AreEqual(trunkExpected, trunk);
        }

        #endregion GetJoinViaSQL
    }
}
// ReSharper restore InconsistentNaming
