namespace SqlDataAccess.Tests.AccessRoles
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.AccessRoles;
    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.ReportsAccess;
    using BusinessLogic.AccessRoles.Scopes;
    using BusinessLogic.Accounts;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;

    using CacheDataAccess.Caching;

    using Common.Logging;
    using Common.Logging.NullLogger;
    
    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SqlDataAccess.Tests.Helpers;

    using SQLDataAccess;
    using SQLDataAccess.AccessRoles;

    using Utilities.Cryptography;

    using Xunit;

    public class SqlAccessRolesFactoryTests 
    {
        public class Ctor : Fixture
        {
            [Fact]
            public void NullCacheFactory_Ctor_ThrowsArgumentNull()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccessRolesFactory(null, this.CustomerDatabaseConnection, this.Logger));
            }

            [Fact]
            public void NullCustomerDataConnection_Ctor_ThrowsArgumentNull()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccessRolesFactory(this.CacheFactory, null, this.Logger));
            }

            [Fact]
            public void NullLogger_Ctor_ThrowsArgumentNull()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccessRolesFactory(this.CacheFactory, this.CustomerDatabaseConnection, null));
            }
        }

        public class Get_ById : Fixture
        {
            
        }

        public class Get : Fixture
        {
            [Fact]
            public void Calling_Get_ThrowsArgumentNull()
            {
                Assert.Throws<NotImplementedException>(() => this.Sut.Get());
            }
        }

        public class GetByPredicate : Fixture
        {
            [Fact]
            public void Calling_GetByPredicate_ThrowsArgumentNull()
            {
                Assert.Throws<NotImplementedException>(() => this.Sut.Get(role => role.Description == string.Empty));
            }
        }

        public class Indexer : Fixture
        {
            [Fact]
            public void NonExistingAccessRole_Indexer_ReturnsNull()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<AccountCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(12);

                // Make the mocked  data connection GetReader() return the mocked dataReader
                this.CustomerDatabaseConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.CustomerDatabaseConnection.ClearReceivedCalls();

                Assert.Null(this.Sut[55]);

                // Ensure that the get reader method was called to retrieve the data from the data connection object
                this.CustomerDatabaseConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccessRole>());

            }

            [Fact]
            public void ExistingAccessRoleNotInCache_Indexer_GoesToSqlAndReturnsCorrectly()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<AccountCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetSingleRecordReader("name", "description", (decimal)2.5, (decimal)10, true, true, true, (short)3, true, true, true, true);
                dataReaderAccessRoles.GetOrdinal("rolename").Returns(0);
                dataReaderAccessRoles.GetOrdinal("description").Returns(1);
                dataReaderAccessRoles.GetOrdinal("expenseClaimMinimumAmount").Returns(2);
                dataReaderAccessRoles.GetOrdinal("expenseClaimMaximumAmount").Returns(3);
                dataReaderAccessRoles.GetOrdinal("employeesCanAmendDesignatedCostCode").Returns(4);
                dataReaderAccessRoles.GetOrdinal("employeesCanAmendDesignatedDepartment").Returns(5);
                dataReaderAccessRoles.GetOrdinal("employeesCanAmendDesignatedProjectCode").Returns(6);
                dataReaderAccessRoles.GetOrdinal("roleAccessLevel").Returns(7);
                dataReaderAccessRoles.GetOrdinal("allowWebsiteAccess").Returns(8);
                dataReaderAccessRoles.GetOrdinal("allowMobileAccess").Returns(9);
                dataReaderAccessRoles.GetOrdinal("allowApiAccess").Returns(10);
                dataReaderAccessRoles.GetOrdinal("employeesMustHaveBankAccount").Returns(11);

                DbDataReader dataReaderAccessScopes = new DataReaderHelper().GetSingleRecordReader(8, true, true, true, true);
                dataReaderAccessScopes.GetOrdinal("elementID").Returns(0);
                dataReaderAccessScopes.GetOrdinal("updateAccess").Returns(1);
                dataReaderAccessScopes.GetOrdinal("insertAccess").Returns(2);
                dataReaderAccessScopes.GetOrdinal("deleteAccess").Returns(3);
                dataReaderAccessScopes.GetOrdinal("viewAccess").Returns(4);

                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDatabaseConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles, dataReaderAccessScopes);
                this.CustomerDatabaseConnection.ClearReceivedCalls();

                IExpensesAccessRole accessRole = this.Sut[55] as IExpensesAccessRole;

                Assert.Equal(55, accessRole.Id);
                Assert.Equal("name", accessRole.Name);
                Assert.Equal("description", accessRole.Description);
                Assert.True(accessRole.CanEditCostCode);
                Assert.True(accessRole.CanEditProjectCode);
                Assert.True(accessRole.CanEditDepartment);
                Assert.Equal(2.5m, accessRole.ClaimMinimumAmount);
                Assert.Equal(10m, accessRole.ClaimMaximumAmount);
                Assert.True(accessRole.MustHaveBankAccount);
                
                // AccessScopes
                Assert.True(accessRole.AccessScopes[ModuleElements.Allowances].Add);
                Assert.True(accessRole.AccessScopes[ModuleElements.Allowances].Edit);
                Assert.True(accessRole.AccessScopes[ModuleElements.Allowances].View);
                Assert.True(accessRole.AccessScopes[ModuleElements.Allowances].Delete);

                Assert.True(accessRole.ApplicationScopes.Contains(typeof(ApiAccess)));
                Assert.True(accessRole.ApplicationScopes.Contains(typeof(WebsiteAccess)));
                Assert.True(accessRole.ApplicationScopes.Contains(typeof(MobileAccess)));

                Assert.IsType<AllDataReportsAccess>(accessRole.ReportsAccess);

                // Ensure that the get reader method was called to retrieve the data for the access role 
                // and a second one was called to get the data for the access scopes
                this.CustomerDatabaseConnection.Received(2).GetReader(Arg.Any<string>());

                // Ensure that the access role gets added to cache
                this.CacheFactory.Received(1).Save(Arg.Any<IAccessRole>());
            }

            [Fact]
            public void ExistingEmployeeResponsible_Indexer_ReturnsCorrectly()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<AccountCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetSingleRecordReader("name", "description", (decimal)2.5, (decimal)10, true, true, true, (short)1, true, true, true, true);
                dataReaderAccessRoles.GetOrdinal("roleAccessLevel").Returns(7);

                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDatabaseConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles);

                IExpensesAccessRole accessRole = this.Sut[55] as IExpensesAccessRole;

                Assert.IsType<EmployeesResponsibleFor>(accessRole.ReportsAccess);
            }

            [Fact]
            public void ExistingSelectedAccessRoles_Indexer_ReturnsCorrectly()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<AccountCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetSingleRecordReader("name", "description", (decimal)2.5, (decimal)10, true, true, true, (short)2, true, true, true, true);
                dataReaderAccessRoles.GetOrdinal("roleAccessLevel").Returns(7);

                DbDataReader dataReaderLinkedAccessRoles = new DataReaderHelper().GetSingleRecordReader(5);

                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDatabaseConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles, dataReaderLinkedAccessRoles);

                IExpensesAccessRole accessRole = this.Sut[55] as IExpensesAccessRole;

                Assert.IsType<SelectedAccessRoles>(accessRole.ReportsAccess);
                Assert.Equal(5, ((SelectedAccessRoles)accessRole.ReportsAccess)[0]);
            }


            [Fact]
            public void UnknownReportAccessType_Indexer_Throws_ArgumentException()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<AccountCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetSingleRecordReader("name", "description", (decimal)2.5, (decimal)10, true, true, true, int.MaxValue, true, true, true, true);
                dataReaderAccessRoles.GetOrdinal("roleAccessLevel").Returns(7);
                
                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDatabaseConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles);

                Assert.Throws<ArgumentException>(() => this.Sut[55]);
            }
        }

        public class Delete : Fixture
        {
            [Fact]
            public void Calling_Delete_ThrowsArgumentNull()
            {
                Assert.Throws<NotImplementedException>(() => this.Sut.Delete(56));
            }
        }

        public class Add : Fixture
        {
            [Fact]
            public void Calling_Add_ThrowsArgumentNull()
            {
                Assert.Throws<NotImplementedException>(() => this.Sut.Save(new AccessRole(int.MaxValue, "name", "desc", new ApplicationScopeCollection(), new AllDataReportsAccess(), new AccessScopeCollection())));
            }
        }

        public abstract class Fixture
        {
            public ILog Logger { get; }

            public RepositoryBase<IAccessRole, int> RepositoryBase { get; }

            public ICache<IAccessRole, int> Cache { get; }

            public IAccount Account { get; }

            public IAccountCacheFactory<IAccessRole, int> CacheFactory { get; }

            public SqlAccessRolesFactory Sut { get; }

            public ICustomerDataConnection<SqlParameter> CustomerDatabaseConnection { get; }

            protected Fixture()
            {
                this.Account = new Account(262, new DatabaseCatalogue(new DatabaseServer(5, "."), "database", "username", "stg5koFJ9k/MQEBJuXV6DA==", new ExpensesCryptography()), false);

                this.Logger = new NullLoggerWrapper();
                this.RepositoryBase = new RepositoryBase<IAccessRole, int>(this.Logger);
                
                this.Cache = Substitute.For<ICache<IAccessRole, int>>();
                this.CacheFactory = Substitute.For<IAccountCacheFactory<IAccessRole, int>>();

                this.CustomerDatabaseConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();
                this.CustomerDatabaseConnection.Parameters = new SqlDataParameters();

                this.Sut = new SqlAccessRolesFactory(this.CacheFactory, this.CustomerDatabaseConnection, this.Logger);
            }
        }
    }
}
