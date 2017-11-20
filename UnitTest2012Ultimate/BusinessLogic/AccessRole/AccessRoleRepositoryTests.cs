namespace UnitTest2012Ultimate.BusinessLogic.AccessRoles
{
    using System;
    using System.Data;

    using global::BusinessLogic.AccessRoles;
    using global::BusinessLogic.AccessRoles.ReportsAccess;
    using global::BusinessLogic.Accounts;
    using global::BusinessLogic.CurrentUser;
    using global::BusinessLogic.Databases;
    using global::BusinessLogic.DataConnections;
    using global::BusinessLogic.Interfaces;
    using global::BusinessLogic.Logging;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SQLDataAccess.AccessRoles;

    using Utilities.Cryptography;

    /// <summary>
    /// The access roles repository tests.
    /// </summary>
    [TestClass]
    public class AccessRoleRepositoryTests
    {
        /// <summary>
        /// The base project code repository tests.
        /// </summary>
        [TestMethod]
        public void BaseAccessRoleRepositoryTest()
        {
            var accessRolesRepo = new TestAccesRolesRepository();

            // Check that AccessRole123 is not in memory
            IAccessRole result = accessRolesRepo[123];
            Assert.IsNull(result);

            // Check we can add AccessRole with Id of 1 to the Repo
            IAccessRole addAccessRole1ToRepo = accessRolesRepo.Add(new AccessRole(1, "AccessRoleName", "AccessRoleDesc", true, true, true, new AllDataReportsAccess(), new ElementAccessCollection(1)));
            Assert.IsTrue(addAccessRole1ToRepo.Id == 1);

            // Get AccessRole with Id of 1 from Repo
            result = accessRolesRepo[1];
            Assert.IsTrue(result.Id == 1);

            // Check we can add AccessRole with Id of 123 to the Repo
            IAccessRole addAccessRole123ToRepo = accessRolesRepo.Add(new AccessRole(123, "AccessRoleName", "AccessRoleDesc", true, true, true, new AllDataReportsAccess(), new ElementAccessCollection(1)));
            Assert.IsTrue(addAccessRole123ToRepo.Id == 123);

            // Get AccessRole with Id of 123 from Repo
            IAccessRole accessRole123FromRepo = accessRolesRepo[123];

            // Check that the Result from the Adding of the AccessRole 123 is the same as the AccessRole 123 retrieved from the Repo
            Assert.IsTrue(addAccessRole123ToRepo == accessRole123FromRepo);
        }

        /// <summary>
        /// Tests the SQL repo element of AccessRoles
        /// </summary>
        [TestMethod]
        public void SqlAccessRoleRepositoryTest()
        {      
            var logger = new Mock<ILogger>();
            logger.SetupAllProperties();
            var dataParameters = new Mock<DataParameters>();
            dataParameters.SetupAllProperties();

            var accountRepository = new Mock<AccountRepository>();
            accountRepository.SetupAllProperties();
            var cryptography = new Mock<ICryptography>();
            cryptography.SetupAllProperties();
            var databaseCatalogue = new DatabaseCatalogue(new DatabaseServer(1, "localhost"), "test", "test", "test", cryptography.Object);

            accountRepository.Setup(x => x[It.IsAny<int>()]).Returns(new Account(123, databaseCatalogue));

            var ds = new DataSet();

            DataTable accessRoleDt = BuildAccessRoleDataTable();
            ds.Tables.Add(accessRoleDt);

            DataTable accessRoleElementDetailsDt = BuildAccessRoleElementDetailsDataTable();
            ds.Tables.Add(accessRoleElementDetailsDt);

            DataTable accessRoleLinkDt = BuildAccessRoleLinkDataTable();
            ds.Tables.Add(accessRoleLinkDt);

            var dataconnection = new TestDataConnection(dataParameters.Object, ds);
            var accessRolesFactory = new SqlAccessRolesFactory(dataconnection);

            IAccessRole result = accessRolesFactory[1];

            Assert.IsTrue(result.Id == 1);
            Assert.IsTrue(result.Description == "desc1");
     
            result = accessRolesFactory[2];
            Assert.IsTrue(result.Id == 2);
            Assert.IsTrue(result.Description == "desc2");
       //     Assert.IsTrue(result.CustomEntityAccess[1].;
        }
     
        private static DataTable BuildAccessRoleDataTable()
        {
            var testData = new DataTable("accessRoles");
            testData.Columns.Add("roleId", typeof(int));
            testData.Columns.Add("roleName");
            testData.Columns.Add("description");
            testData.Columns.Add("expenseClaimMinimumAmount", typeof(decimal));
            testData.Columns.Add("expenseClaimMaximumAmount", typeof(decimal));
            testData.Columns.Add("employeesCanAmendDesignatedCostCode", typeof(bool));
            testData.Columns.Add("employeesCanAmendDesignatedDepartment", typeof(bool));
            testData.Columns.Add("employeesCanAmendDesignatedProjectCode", typeof(bool));
            testData.Columns.Add("roleAccessLevel", typeof(int));
            testData.Columns.Add("allowWebsiteAccess", typeof(bool));
            testData.Columns.Add("allowMobileAccess", typeof(bool));
            testData.Columns.Add("allowApiAccess", typeof(bool));
            testData.Columns.Add("employeesMustHaveBankAccount", typeof(bool));

            var dataRow = testData.NewRow();
            dataRow[0] = 1;
            dataRow[1] = "testRole1";
            dataRow[2] = "desc1";
            dataRow[3] = 0;
            dataRow[4] = 100;
            dataRow[5] = false;
            dataRow[6] = false;
            dataRow[7] = false;
            dataRow[8] = 1;
            dataRow[9] = false;
            dataRow[10] = false;
            dataRow[11] = false;
            dataRow[12] = false;
            testData.Rows.Add(dataRow);

            dataRow = testData.NewRow();
            dataRow[0] = 2;
            dataRow[1] = "testRole2";
            dataRow[2] = "desc2";
            dataRow[3] = 0;
            dataRow[4] = 100;
            dataRow[5] = false;
            dataRow[6] = false;
            dataRow[7] = false;
            dataRow[8] = 1;
            dataRow[9] = false;
            dataRow[10] = false;
            dataRow[11] = false;
            dataRow[12] = false;
            testData.Rows.Add(dataRow);
            return testData;
        }

        private static DataTable BuildAccessRoleLinkDataTable()
        {
            DataRow dataRow;
            var testData3 = new DataTable("dbo.accessRolesLink");
            testData3.Columns.Add("primaryAccessRoleID", typeof(int));
            testData3.Columns.Add("secondaryAccessRoleID", typeof(int));

            dataRow = testData3.NewRow();
            dataRow[0] = 1;
            dataRow[1] = 2;
            testData3.Rows.Add(dataRow);
            return testData3;
        }

        private static DataTable BuildAccessRoleElementDetailsDataTable()
        {
            DataRow dataRow;
            var testData2 = new DataTable("dbo.accessRoleElementDetails");
            testData2.Columns.Add("elementID", typeof(int));
            testData2.Columns.Add("updateAccess", typeof(bool));
            testData2.Columns.Add("insertAccess", typeof(bool));
            testData2.Columns.Add("deleteAccess", typeof(bool));
            testData2.Columns.Add("viewAccess", typeof(bool));
            testData2.Columns.Add("roleId", typeof(int));

            dataRow = testData2.NewRow();
            dataRow[0] = 1;
            dataRow[1] = false;
            dataRow[2] = false;
            dataRow[3] = false;
            dataRow[4] = false;
            dataRow[5] = 1;

            testData2.Rows.Add(dataRow);
            return testData2;
        }
    }

    internal class TestAccesRolesRepository : AccessRolesRepository
    {

    }
}
