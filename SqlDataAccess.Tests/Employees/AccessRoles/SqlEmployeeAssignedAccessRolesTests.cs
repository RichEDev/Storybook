namespace SqlDataAccess.Tests.Employees.AccessRoles
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Employees.AccessRoles;

    using CacheDataAccess.Caching;

    using Common.Logging;
    using Common.Logging.NullLogger;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SqlDataAccess.Tests.Helpers;

    using SQLDataAccess;
    using SQLDataAccess.Employees.AccessRoles;

    using Xunit;

    public class SqlEmployeeAssignedAccessRolesTests
    {
        public class Get : Fixture
        {
            // Get returns no records (no access roles or invalid employee etc

            [Fact]
            public void NoAssignedAccessRoles_Get_ReturnsEmptyCollection()
            {
                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetEmptyRecordReader(2);
                dataReaderAccessRoles.GetOrdinal("accessRoleID").Returns(0);
                dataReaderAccessRoles.GetOrdinal("subAccountID").Returns(1);

                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles);
                this.CustomerDataConnection.ClearReceivedCalls();

                Assert.Empty(this.Sut.Get(55));
            }

            [Fact]
            public void HasAssignedAccessRoles_Get_ReturnsPopulatedCollection()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.CacheFactory[Arg.Any<string>()].ReturnsNullForAnyArgs();

                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetSingleRecordReader(512, 5);
                dataReaderAccessRoles.GetOrdinal("accessRoleID").Returns(0);
                dataReaderAccessRoles.GetOrdinal("subAccountID").Returns(1);
                
                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles);
                this.CustomerDataConnection.ClearReceivedCalls();

                List<IAssignedAccessRole> assignedAccessRoles = this.Sut.Get(55).ToList();

                Assert.Equal(1, assignedAccessRoles.Count);
                Assert.Equal(55, assignedAccessRoles[0].EmployeeId);
                Assert.Equal(512, assignedAccessRoles[0].AccessRoleId);
                Assert.Equal(5, assignedAccessRoles[0].SubAccountId);

                // Ensure that the get reader method was called to retrieve the data for the access role 
                // and a second one was called to get the data for the access scopes
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that the access role gets added to cache
                this.CacheFactory.DidNotReceive().Save(Arg.Any<IAssignedAccessRole>());
            }
        }

        public class Get_Predicate : Fixture
        {
            [Fact]
            public void PredicateWithMatch_Get_ReturnsPopulatedCollection()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.CacheFactory[Arg.Any<string>()].ReturnsNullForAnyArgs();

                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetSingleRecordReader(66, 1);
                dataReaderAccessRoles.GetOrdinal("accessRoleID").Returns(0);
                dataReaderAccessRoles.GetOrdinal("subAccountID").Returns(1);

                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles);
                this.CustomerDataConnection.ClearReceivedCalls();

                Assert.Equal(1, this.Sut.Get(55, accessRole => accessRole.AccessRoleId == 66).ToList().Count);
                
                // Ensure that the get reader method was called to retrieve the data for the access role 
                // and a second one was called to get the data for the access scopes
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that the access role gets added to cache
                this.CacheFactory.DidNotReceive().Save(Arg.Any<IAssignedAccessRole>());
            }

            [Fact]
            public void PredicateWithNoMatches_Get_ReturnsEmptyCollection()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.CacheFactory[Arg.Any<string>()].ReturnsNullForAnyArgs();

                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetSingleRecordReader(11, 1);
                dataReaderAccessRoles.GetOrdinal("accessRoleID").Returns(0);
                dataReaderAccessRoles.GetOrdinal("subAccountID").Returns(1);

                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles);
                this.CustomerDataConnection.ClearReceivedCalls();

                Assert.Equal(0, this.Sut.Get(55, accessRole => accessRole.AccessRoleId == 66).ToList().Count);
                
                // Ensure that the get reader method was called to retrieve the data for the access role 
                // and a second one was called to get the data for the access scopes
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that the access role gets added to cache
                this.CacheFactory.DidNotReceive().Save(Arg.Any<IAssignedAccessRole>());
            }
        }

        public class Delete : Fixture
        {
            [Fact]
            public void Calling_Delete_ThrowsNotImplemented()
            {
                Assert.Throws<NotImplementedException>(() => this.Sut.Add(new AssignedAccessRole(1, 2, 3)));
            }
        }

        public class Add : Fixture
        {
            [Fact]
            public void Calling_Add_ThrowsNotImplemented()
            {
                Assert.Throws<NotImplementedException>(() => this.Sut.Delete(1, 2, 3));
            }
        }

        public class Indexer : Fixture
        {
            [Fact]
            public void EmployeeWithAccessRole_Indexer_ReturnsCollection()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.CacheFactory[Arg.Any<string>()].ReturnsNullForAnyArgs();

                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetSingleRecordReader(512, 5);
                dataReaderAccessRoles.GetOrdinal("accessRoleID").Returns(0);
                dataReaderAccessRoles.GetOrdinal("subAccountID").Returns(1);

                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles);
                this.CustomerDataConnection.ClearReceivedCalls();

                List<IAssignedAccessRole> assignedAccessRoles = this.Sut[55].ToList();

                Assert.Equal(1, assignedAccessRoles.Count);
                Assert.Equal(55, assignedAccessRoles[0].EmployeeId);
                Assert.Equal(512, assignedAccessRoles[0].AccessRoleId);
                Assert.Equal(5, assignedAccessRoles[0].SubAccountId);

                // Ensure that the get reader method was called to retrieve the data for the access role 
                // and a second one was called to get the data for the access scopes
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that the access role gets added to cache
                this.CacheFactory.DidNotReceive().Save(Arg.Any<IAssignedAccessRole>());
            }

            [Fact]
            public void EmployeeWithoutAccessRole_Indexer_ReturnsCollection()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.CacheFactory[Arg.Any<string>()].ReturnsNullForAnyArgs();

                // Mock the DbDataReader to return a valid access role
                DbDataReader dataReaderAccessRoles = new DataReaderHelper().GetEmptyRecordReader(2);
                dataReaderAccessRoles.GetOrdinal("accessRoleID").Returns(0);
                dataReaderAccessRoles.GetOrdinal("subAccountID").Returns(1);

                // Make the mocked data connection GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReaderAccessRoles);
                this.CustomerDataConnection.ClearReceivedCalls();

                Assert.Empty(this.Sut[55]);

                // Ensure that the get reader method was called to retrieve the data for the access role 
                // and a second one was called to get the data for the access scopes
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that the access role gets added to cache
                this.CacheFactory.DidNotReceive().Save(Arg.Any<IAssignedAccessRole>());
            }
        }

        public abstract class Fixture
        {
            public IAccountCacheFactory<IAssignedAccessRole, string> CacheFactory { get; }

            public ICustomerDataConnection<SqlParameter> CustomerDataConnection { get; }

            public ILog Logger  { get; }

            public SqlEmployeeAssignedAccessRoles Sut { get; }

            protected Fixture()
            {
                this.Logger = new NullLoggerWrapper();

                this.CustomerDataConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();
                this.CustomerDataConnection.Parameters = new SqlDataParameters();

                this.CacheFactory = Substitute.For<IAccountCacheFactory<IAssignedAccessRole, string>>();
                
                this.Sut = new SqlEmployeeAssignedAccessRoles(this.CacheFactory, this.CustomerDataConnection, this.Logger);
            }
    }
    }
}
