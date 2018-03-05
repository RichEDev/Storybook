namespace SqlDataAccess.Tests.Accounts
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SQLDataAccess;
    using SQLDataAccess.Accounts;
    using SQLDataAccess.Elements;
    using SqlDataAccess.Tests.Helpers;

    using Utilities.Cryptography;

    using Xunit;

    public class SqlAccountWithElementFactoryTests
    {
        /// <summary>
        /// Tests to ensure the Constructor of <see cref="SqlAccountWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class Constructor : SqlAccountWithElementsFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="MetabaseCacheFactory{T,TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCacheFactory_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountWithElementsFactory(null, this.SqlAccountFactory, this.SqlElementFactory, this.AccountWithElementsMetabaseDataConnections));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="SqlAccountFactory"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullSqlAccountFactory_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountWithElementsFactory(this.AccountWithElementsCacheFactory, null, this.SqlElementFactory, this.AccountWithElementsMetabaseDataConnections));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="SqlElementFactory"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullSqlElementFactory_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountWithElementsFactory(this.AccountWithElementsCacheFactory, this.SqlAccountFactory ,null, this.AccountWithElementsMetabaseDataConnections));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IMetabaseDataConnection{T}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullMetabaseDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountWithElementsFactory(this.AccountWithElementsCacheFactory, this.SqlAccountFactory, this.SqlElementFactory, null));
            }
        }

        /// <summary>
        /// Tests to ensure the Indexer method of <see cref="SqlAccountWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class Indexer : SqlAccountWithElementsFactoryFixture
        {
            /// <summary>
            /// Fail to get the <see cref="IAccount"/> by index from cache and sql so return null
            /// </summary>
            [Fact]
            public void FailToGetAccountFromCacheOrSql_Indexer_ReturnsNull()
            {
                this.AccountCacheFactory[1].ReturnsNull();
                this.AccountCacheFactory.ClearReceivedCalls();

                this.AccountWithElementsCacheFactory[1].ReturnsNull();
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                IAccountWithElement accountWithElements = this.SUT[1];

                Assert.Null(accountWithElements);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Get <see cref="IAccount"/> by index from cache and convert to <see cref="IAccountWithElement"/>
            /// </summary>
            [Fact]
            public void GetAccountFromCache_Indexer_ReturnsAccountConvertedToAccountWithElement()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                this.AccountWithElementsCacheFactory[account.Id].ReturnsNull();
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                this.AccountCacheFactory[account.Id].Returns(account);
                this.AccountCacheFactory.ClearReceivedCalls();

                //Mock the IDataReader
                DbDataReader elementDataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();

                IAccountWithElement accounWithElements = this.SUT[1];
                
                Assert.NotNull(accounWithElements);
                Assert.Equal(account.Id, accounWithElements.Id);
                    
                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(0).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccount>());
            }


            /// <summary>
            /// Get <see cref="IAccount"/> by index from sql and convert to <see cref="IAccountWithElement"/>
            /// </summary>
            [Fact]
            public void GetAccountFromSql_Indexer_ReturnsAccountConvertedToAccountWithElement()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                this.AccountWithElementsCacheFactory[account.Id].ReturnsNull();
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                this.AccountCacheFactory[account.Id].ReturnsNull();
                this.AccountCacheFactory.ClearReceivedCalls();

                // Mock the IDataReader
                DbDataReader accountDataReader = new DataReaderHelper().GetSingleRecordReader(1, "TheDatabaseCatalogue", "TheUsername", "ThePassword", 11);
                DbDataReader accountWithElementsDataReader = new DataReaderHelper().GetEmptyRecordReader(2);
                DbDataReader elementDataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountDataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountWithElementsDataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                IAccountWithElement accountWithElement = this.SUT[1];

                Assert.NotNull(accountWithElement);
                Assert.Equal(account.Id, accountWithElement.Id);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Get a <see cref="IAccount"/> by index from Sql and Decorate them with the list of <see cref="IElement"/>
            /// </summary>
            /// <param name="elementId">Element Id for the <see cref="AccountWithElements"/></param>
            /// <param name="categoryId">Category Id for the <see cref="AccountWithElements"/></param>
            /// <param name="name">Name for the <see cref="AccountWithElements"/></param>
            /// <param name="description">Description for the <see cref="AccountWithElements"/></param>
            /// <param name="canEdit">Can edit state for the <see cref="AccountWithElements"/></param>
            /// <param name="canAdd">Can add state for the <see cref="AccountWithElements"/></param>
            /// <param name="canDelete">Can delete state for the <see cref="AccountWithElements"/></param>
            /// <param name="accessRolesApplicable">Access role applicable state for the <see cref="AccountWithElements"/></param>
            [Theory]
            [InlineData(11, 1, "Foo", "FooBar", false, true, true, true)]
            [InlineData(8, 71, "Test", "Testing", true, false, false, true)]
            [InlineData(51, 341, "Bar", "BarFoo", false, false, false, false)]
            public void GetAccountFromSqlAndDecorateWithElements_Indexer_ReturnsAccountConvertedToAccountWithElement
                (int elementId, int categoryId, string name, string description, bool canEdit, bool canAdd, bool canDelete, bool accessRolesApplicable)
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                this.AccountWithElementsCacheFactory[account.Id].ReturnsNull();
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                this.AccountCacheFactory[account.Id].ReturnsNull();
                this.AccountCacheFactory.ClearReceivedCalls();

                // Mock the IDataReader
                DbDataReader accountDataReader = new DataReaderHelper().GetSingleRecordReader(1, "TheDatabaseCatalogue", "TheUsername", "ThePassword", 11);

                DbDataReader elementDataReader = new DataReaderHelper().GetSingleRecordReader(1, elementId, categoryId, name, description, canEdit, canAdd, canDelete, "", accessRolesApplicable);
                elementDataReader.GetOrdinal("elementID").Returns(1);
                elementDataReader.GetOrdinal("categoryID").Returns(2);
                elementDataReader.GetOrdinal("elementName").Returns(3);
                elementDataReader.GetOrdinal("description").Returns(4);
                elementDataReader.GetOrdinal("accessRolesCanEdit").Returns(5);
                elementDataReader.GetOrdinal("accessRolesCanAdd").Returns(6);
                elementDataReader.GetOrdinal("accessRolesCanDelete").Returns(7);
                elementDataReader.GetOrdinal("elementFriendlyName").Returns(8);
                elementDataReader.GetOrdinal("accessRolesApplicable").Returns(9);

                DbDataReader accountsWithElementsDataReader = new DataReaderHelper().GetSingleRecordReader(1, elementId);
                accountsWithElementsDataReader.GetOrdinal("accountID").Returns(0);
                accountsWithElementsDataReader.GetOrdinal("elementID").Returns(1);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountDataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountsWithElementsDataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();

                IAccountWithElement accountWithElement = this.SUT[1];

                Assert.NotNull(accountWithElement);

                Assert.Equal(elementId, accountWithElement.LicencedElements[0].Id);
                Assert.Equal(categoryId, accountWithElement.LicencedElements[0].CategoryId);
                Assert.Equal(name, accountWithElement.LicencedElements[0].Name);
                Assert.Equal(description, accountWithElement.LicencedElements[0].Description);
                Assert.Equal(canEdit, accountWithElement.LicencedElements[0].AccessRolesCanEdit);
                Assert.Equal(canAdd, accountWithElement.LicencedElements[0].AccessRolesCanAdd);
                Assert.Equal(canDelete, accountWithElement.LicencedElements[0].AccessRolesCanDelete);
                Assert.Equal(accessRolesApplicable, accountWithElement.LicencedElements[0].AccessRolesApplicable);
                Assert.Equal(account.Id, accountWithElement.Id);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Get the <see cref="IAccountWithElement"/> from cache
            /// </summary>
            [Fact]
            public void GetAccountWIthElementsFromCache_Indexer_ReturnsAccountWithElements()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)

                IAccountWithElement account = Substitute.For<IAccountWithElement>();
                account.Id = 1;

                this.AccountWithElementsCacheFactory[account.Id].Returns(account);
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                IAccountWithElement accountWithElements = this.SUT[account.Id];

                Assert.NotNull(accountWithElements);
                Assert.Equal(account.Id, accountWithElements.Id);
            }
        }

        /// <summary>
        /// Tests to ensure the Get method of <see cref="SqlAccountWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class Get : SqlAccountWithElementsFactoryFixture
        {
            /// <summary>
            /// Fail to Get the list of <see cref="IAccount"/> from sql
            /// </summary>
            [Fact]
            public void FailToGetAccountFromSql_Get_ReturnsNull()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.AccountWithElementsCache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                IList<IAccountWithElement> accountsWithElements = this.SUT.Get();

                Assert.Null(accountsWithElements);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Get the list of <see cref="IAccount"/> from sql and convert to <see cref="IAccountWithElement"/>
            /// </summary>
            [Fact]
            public void GetAccountFromSql_Get_ReturnsAccountConvertedToAccountWithElement()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.AccountWithElementsCache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;
                
                // Mock the IDataReader
                DbDataReader accountDataReader = new DataReaderHelper().GetSingleRecordReader(1, "TheDatabaseCatalogue", "TheUsername", "ThePassword", 11);
                DbDataReader accountWithElementsDataReader = new DataReaderHelper().GetEmptyRecordReader(2);
                DbDataReader elementDataReader = new DataReaderHelper().GetEmptyRecordReader(9);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountDataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountWithElementsDataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                IList<IAccountWithElement> accountsWithElements = this.SUT.Get();

                Assert.NotNull(accountsWithElements[0]);

                Assert.Equal(account.Id, accountsWithElements[0].Id);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
            }

            /// <summary>
            /// Get a list of <see cref="IAccount"/> from Sql and Decorate them with the list of <see cref="IElement"/>
            /// </summary>
            /// <param name="elementId">Element Id for the <see cref="AccountWithElements"/></param>
            /// <param name="categoryId">Category Id for the <see cref="AccountWithElements"/></param>
            /// <param name="name">Name for the <see cref="AccountWithElements"/></param>
            /// <param name="description">Description for the <see cref="AccountWithElements"/></param>
            /// <param name="canEdit">Can edit state for the <see cref="AccountWithElements"/></param>
            /// <param name="canAdd">Can add state for the <see cref="AccountWithElements"/></param>
            /// <param name="canDelete">Can delete state for the <see cref="AccountWithElements"/></param>
            /// <param name="accessRolesApplicable">Access role applicable state for the <see cref="AccountWithElements"/></param>
            [Theory]
            [InlineData(11, 1, "Foo", "FooBar", false, true, true, true)]
            [InlineData(8, 71, "Test", "Testing", true, false, false, true)]
            [InlineData(51, 341, "Bar", "BarFoo", false, false, false, false)]
            public void GetAccountFromSqlAndDecorateWithElements_Get_ReturnsAccountConvertedToAccountWithElement
                (int elementId, int categoryId, string name, string description, bool canEdit, bool canAdd, bool canDelete, bool accessRolesApplicable)
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.AccountWithElementsCache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                // Mock the IDataReader
                DbDataReader accountDataReader = new DataReaderHelper().GetSingleRecordReader(1, "TheDatabaseCatalogue", "TheUsername", "ThePassword", 11);

                DbDataReader elementDataReader = new DataReaderHelper().GetSingleRecordReader(1, elementId, categoryId, name, description, canEdit, canAdd, canDelete, "", accessRolesApplicable);
                elementDataReader.GetOrdinal("elementID").Returns(1);
                elementDataReader.GetOrdinal("categoryID").Returns(2);
                elementDataReader.GetOrdinal("elementName").Returns(3);
                elementDataReader.GetOrdinal("description").Returns(4);
                elementDataReader.GetOrdinal("accessRolesCanEdit").Returns(5);
                elementDataReader.GetOrdinal("accessRolesCanAdd").Returns(6);
                elementDataReader.GetOrdinal("accessRolesCanDelete").Returns(7);
                elementDataReader.GetOrdinal("elementFriendlyName").Returns(8);
                elementDataReader.GetOrdinal("accessRolesApplicable").Returns(9);

                DbDataReader accountsWithElementsDataReader = new DataReaderHelper().GetSingleRecordReader(1, elementId);
                accountsWithElementsDataReader.GetOrdinal("accountID").Returns(0);
                accountsWithElementsDataReader.GetOrdinal("elementID").Returns(1);


                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountDataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountsWithElementsDataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();

                IList<IAccountWithElement> accountsWithElements = this.SUT.Get();

                Assert.NotNull(accountsWithElements);

                Assert.Equal(elementId, accountsWithElements[0].LicencedElements[0].Id);
                Assert.Equal(categoryId, accountsWithElements[0].LicencedElements[0].CategoryId);
                Assert.Equal(name, accountsWithElements[0].LicencedElements[0].Name);
                Assert.Equal(description, accountsWithElements[0].LicencedElements[0].Description);
                Assert.Equal(canEdit, accountsWithElements[0].LicencedElements[0].AccessRolesCanEdit);
                Assert.Equal(canAdd, accountsWithElements[0].LicencedElements[0].AccessRolesCanAdd);
                Assert.Equal(canDelete, accountsWithElements[0].LicencedElements[0].AccessRolesCanDelete);
                Assert.Equal(accessRolesApplicable, accountsWithElements[0].LicencedElements[0].AccessRolesApplicable);
                Assert.Equal(account.Id, accountsWithElements[0].Id);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
                this.ElementMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
                this.AccountWithElementsMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccountWithElement is returned from the SqlAccountWithElementsFactory Get method it is then inserted into cache.
                this.AccountWithElementsCacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IAccountWithElement>());
            }

            /// <summary>
            /// Get the <see cref="IAccountWithElement"/> from cache
            /// </summary>
            [Fact]
            public void GetAccountWIthElementsFromCache_Get_ReturnsAccountWithElements()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)

                IAccountWithElement account = Substitute.For<IAccountWithElement>();
                account.Id = 1;

                List<IAccountWithElement> accounts
                    = Substitute.For<List<IAccountWithElement>>();
                accounts.Add(account);

                this.AccountWithElementsCache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").ReturnsForAnyArgs(accounts);

                IList<IAccountWithElement> accountsWithElements = this.SUT.Get();

                Assert.NotNull(accountsWithElements);
                Assert.Equal(account.Id, accountsWithElements[0].Id);
            }
        }

        /// <summary>
        /// Tests to ensure the Get By Id method of <see cref="SqlAccountWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class GetById : SqlAccountWithElementsFactoryFixture
        {
            /// <summary>
            /// Fail to get <see cref="IAccount"/> by Id from cache or sql
            /// </summary>
            [Fact]
            public void FailToGetAccountFromCacheOrSql_GetById_ReturnsNull()
            {
                this.AccountCacheFactory[1].ReturnsNull();
                this.AccountCacheFactory.ClearReceivedCalls();

                this.AccountWithElementsCacheFactory[1].ReturnsNull();
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                IAccountWithElement accountWithElements = this.SUT.Get(1);

                Assert.Null(accountWithElements);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Get the <see cref="IAccount"/> by Id from cache and convert to <see cref="IAccountWithElement"/>
            /// </summary>
            [Fact]
            public void GetAccountFromCache_GetById_ReturnsAccountConvertedToAccountWithElement()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                this.AccountWithElementsCacheFactory[account.Id].ReturnsNull();
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                this.AccountCacheFactory[account.Id].Returns(account);
                this.AccountCacheFactory.ClearReceivedCalls();

                //Mock the IDataReader
                DbDataReader elementDataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();

                IAccountWithElement accounWithElements = this.SUT.Get(1);

                Assert.NotNull(accounWithElements);
                Assert.Equal(account.Id, accounWithElements.Id);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(0).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Get the <see cref="IAccount"/> by Id from sql and convert to <see cref="IAccountWithElement"/>
            /// </summary>
            [Fact]
            public void GetAccountFromSql_GetById_ReturnsAccountConvertedToAccountWithElement()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                this.AccountWithElementsCacheFactory[account.Id].ReturnsNull();
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                this.AccountCacheFactory[account.Id].ReturnsNull();
                this.AccountCacheFactory.ClearReceivedCalls();

                // Mock the IDataReader
                DbDataReader accountDataReader = new DataReaderHelper().GetSingleRecordReader(1, "TheDatabaseCatalogue", "TheUsername", "ThePassword", 11);
                DbDataReader accountWithElementsDataReader = new DataReaderHelper().GetEmptyRecordReader(2);
                DbDataReader elementDataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountDataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountWithElementsDataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                IAccountWithElement accountWithElement = this.SUT.Get(1);

                Assert.NotNull(accountWithElement);
                Assert.Equal(account.Id, accountWithElement.Id);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Get the <see cref="IAccount"/> by id from Sql and Decorate them with the list of <see cref="IElement"/>
            /// </summary>
            /// <param name="elementId">Element Id for the <see cref="AccountWithElements"/></param>
            /// <param name="categoryId">Category Id for the <see cref="AccountWithElements"/></param>
            /// <param name="name">Name for the <see cref="AccountWithElements"/></param>
            /// <param name="description">Description for the <see cref="AccountWithElements"/></param>
            /// <param name="canEdit">Can edit state for the <see cref="AccountWithElements"/></param>
            /// <param name="canAdd">Can add state for the <see cref="AccountWithElements"/></param>
            /// <param name="canDelete">Can delete state for the <see cref="AccountWithElements"/></param>
            /// <param name="accessRolesApplicable">Access role applicable state for the <see cref="AccountWithElements"/></param>
            [Theory]
            [InlineData(11, 1, "Foo", "FooBar", false, true, true, true)]
            [InlineData(8, 71, "Test", "Testing", true, false, false, true)]
            [InlineData(51, 341, "Bar", "BarFoo", false, false, false, false)]
            public void GetAccountFromSqlAndDecorateWithElements_GetById_ReturnsAccountConvertedToAccountWithElement
                (int elementId, int categoryId, string name, string description, bool canEdit, bool canAdd, bool canDelete, bool accessRolesApplicable)
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                this.AccountWithElementsCacheFactory[account.Id].ReturnsNull();
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                this.AccountCacheFactory[account.Id].ReturnsNull();
                this.AccountCacheFactory.ClearReceivedCalls();

                // Mock the IDataReader
                DbDataReader accountDataReader = new DataReaderHelper().GetSingleRecordReader(1, "TheDatabaseCatalogue", "TheUsername", "ThePassword", 11);

                DbDataReader elementDataReader = new DataReaderHelper().GetSingleRecordReader(1, elementId, categoryId, name, description, canEdit, canAdd, canDelete, "", accessRolesApplicable);
                elementDataReader.GetOrdinal("elementID").Returns(1);
                elementDataReader.GetOrdinal("categoryID").Returns(2);
                elementDataReader.GetOrdinal("elementName").Returns(3);
                elementDataReader.GetOrdinal("description").Returns(4);
                elementDataReader.GetOrdinal("accessRolesCanEdit").Returns(5);
                elementDataReader.GetOrdinal("accessRolesCanAdd").Returns(6);
                elementDataReader.GetOrdinal("accessRolesCanDelete").Returns(7);
                elementDataReader.GetOrdinal("elementFriendlyName").Returns(8);
                elementDataReader.GetOrdinal("accessRolesApplicable").Returns(9);

                DbDataReader accountsWithElementsDataReader = new DataReaderHelper().GetSingleRecordReader(1, elementId);
                accountsWithElementsDataReader.GetOrdinal("accountID").Returns(0);
                accountsWithElementsDataReader.GetOrdinal("elementID").Returns(1);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountDataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountsWithElementsDataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();

                IAccountWithElement accountWithElement = this.SUT.Get(1);

                Assert.NotNull(accountWithElement);

                Assert.Equal(elementId, accountWithElement.LicencedElements[0].Id);
                Assert.Equal(categoryId, accountWithElement.LicencedElements[0].CategoryId);
                Assert.Equal(name, accountWithElement.LicencedElements[0].Name);
                Assert.Equal(description, accountWithElement.LicencedElements[0].Description);
                Assert.Equal(canEdit, accountWithElement.LicencedElements[0].AccessRolesCanEdit);
                Assert.Equal(canAdd, accountWithElement.LicencedElements[0].AccessRolesCanAdd);
                Assert.Equal(canDelete, accountWithElement.LicencedElements[0].AccessRolesCanDelete);
                Assert.Equal(accessRolesApplicable, accountWithElement.LicencedElements[0].AccessRolesApplicable);
                Assert.Equal(account.Id, accountWithElement.Id);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Get the <see cref="IAccountWithElement"/> from cache
            /// </summary>
            [Fact]
            public void GetAccountWIthElementsFromCache_GetById_ReturnsAccountWithElements()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)

                IAccountWithElement account = Substitute.For<IAccountWithElement>();
                account.Id = 1;

                this.AccountWithElementsCacheFactory[account.Id].Returns(account);
                this.AccountWithElementsCacheFactory.ClearReceivedCalls();

                IAccountWithElement accountWithElements = this.SUT.Get(account.Id);

                Assert.NotNull(accountWithElements);
                Assert.Equal(account.Id, accountWithElements.Id);
            }
        }

        /// <summary>
        /// Tests to ensure the Get By Predicate method of <see cref="SqlAccountWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class GetByPredicate : SqlAccountWithElementsFactoryFixture
        {
            /// <summary>
            /// Fail to get <see cref="IAccount"/> from cache or sql return null when getting <see cref="IAccountWithElement"/> by predicate
            /// </summary>
            [Fact]
            public void FailToGetAccountFromSql_GetByPredicate_ReturnsNull()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.AccountWithElementsCache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);
                DbDataReader accountWithElementsDataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();
                
                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountWithElementsDataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();


                IList<IAccountWithElement> accountWithElement = this.SUT.Get(predicateAccountWithElement => predicateAccountWithElement.Id == 1);

                Assert.Null(accountWithElement);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Pass null predicate return null
            /// </summary>
            [Fact]
            public void PassNullPredicate_GetByPredicate_ReturnsNull()
            {
                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                IList<IAccountWithElement> accountsWithElements = this.SUT.Get(null);

                Assert.Null(accountsWithElements);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(0).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.AccountCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Get the <see cref="IAccount"/> from Sql and Decorate them with the list of <see cref="IElement"/> before returning the matching <see cref="AccountWithElements"/> predicate
            /// </summary>
            /// <param name="elementId">Element Id for the <see cref="AccountWithElements"/></param>
            /// <param name="categoryId">Category Id for the <see cref="AccountWithElements"/></param>
            /// <param name="name">Name for the <see cref="AccountWithElements"/></param>
            /// <param name="description">Description for the <see cref="AccountWithElements"/></param>
            /// <param name="canEdit">Can edit state for the <see cref="AccountWithElements"/></param>
            /// <param name="canAdd">Can add state for the <see cref="AccountWithElements"/></param>
            /// <param name="canDelete">Can delete state for the <see cref="AccountWithElements"/></param>
            /// <param name="accessRolesApplicable">Access role applicable state for the <see cref="AccountWithElements"/></param>
            [Theory]
            [InlineData(11, 1, "Foo", "FooBar", false, true, true, true)]
            [InlineData(8, 71, "Test", "Testing", true, false, false, true)]
            [InlineData(51, 341, "Bar", "BarFoo", false, false, false, false)]
            public void GetAccountFromSqlAndDecorateWithElements_GetByPredicate_ReturnsAccountConvertedToAccountWithElement
                (int elementId, int categoryId, string name, string description, bool canEdit, bool canAdd, bool canDelete, bool accessRolesApplicable)
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.AccountWithElementsCache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                this.AccountCacheFactory[account.Id].ReturnsNull();
                this.AccountCacheFactory.ClearReceivedCalls();

                // Mock the IDataReader
                DbDataReader accountDataReader = new DataReaderHelper().GetSingleRecordReader(1, "TheDatabaseCatalogue", "TheUsername", "ThePassword", 11);

                DbDataReader elementDataReader = new DataReaderHelper().GetSingleRecordReader(1, elementId, categoryId, name, description, canEdit, canAdd, canDelete, "", accessRolesApplicable);
                elementDataReader.GetOrdinal("elementID").Returns(1);
                elementDataReader.GetOrdinal("categoryID").Returns(2);
                elementDataReader.GetOrdinal("elementName").Returns(3);
                elementDataReader.GetOrdinal("description").Returns(4);
                elementDataReader.GetOrdinal("accessRolesCanEdit").Returns(5);
                elementDataReader.GetOrdinal("accessRolesCanAdd").Returns(6);
                elementDataReader.GetOrdinal("accessRolesCanDelete").Returns(7);
                elementDataReader.GetOrdinal("elementFriendlyName").Returns(8);
                elementDataReader.GetOrdinal("accessRolesApplicable").Returns(9);

                DbDataReader accountsWithElementsDataReader = new DataReaderHelper().GetSingleRecordReader(1, elementId);
                accountsWithElementsDataReader.GetOrdinal("accountID").Returns(0);
                accountsWithElementsDataReader.GetOrdinal("elementID").Returns(1);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.AccountMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountDataReader);
                this.AccountMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                this.AccountWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountsWithElementsDataReader);
                this.AccountWithElementsMetabaseDataConnections.ClearReceivedCalls();

                IList<IAccountWithElement> accountsWithElements = this.SUT.Get(predicateAccountWithElements => predicateAccountWithElements.Id == 1);

                Assert.NotNull(accountsWithElements);

                Assert.Equal(elementId, accountsWithElements[0].LicencedElements[0].Id);
                Assert.Equal(categoryId, accountsWithElements[0].LicencedElements[0].CategoryId);
                Assert.Equal(name, accountsWithElements[0].LicencedElements[0].Name);
                Assert.Equal(description, accountsWithElements[0].LicencedElements[0].Description);
                Assert.Equal(canEdit, accountsWithElements[0].LicencedElements[0].AccessRolesCanEdit);
                Assert.Equal(canAdd, accountsWithElements[0].LicencedElements[0].AccessRolesCanAdd);
                Assert.Equal(canDelete, accountsWithElements[0].LicencedElements[0].AccessRolesCanDelete);
                Assert.Equal(accessRolesApplicable, accountsWithElements[0].LicencedElements[0].AccessRolesApplicable);
                Assert.Equal(account.Id, accountsWithElements[0].Id);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.AccountMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
            }

            /// <summary>
            /// Get the <see cref="IAccountWithElement"/> from cache
            /// </summary>
            [Fact]
            public void GetAccountWIthElementsFromCache_GetByPredicate_ReturnsAccountWithElements()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                IAccountWithElement account = Substitute.For<IAccountWithElement>();
                account.Id = 1;

                List<IAccountWithElement> accounts
                    = Substitute.For<List<IAccountWithElement>>();
                accounts.Add(account);

                this.AccountWithElementsCache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").ReturnsForAnyArgs(accounts);

                IList<IAccountWithElement> accountsWithElements = this.SUT.Get(predicateAccountWithElements => predicateAccountWithElements.Id == 1);

                Assert.NotNull(accountsWithElements);
                Assert.Equal(account.Id, accountsWithElements[0].Id);
            }
        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="SqlAccountWithElementsFactory"/>
    /// </summary>
    public class SqlAccountWithElementsFactoryFixture
    {
        /// <summary>
        /// Mock'd <see cref="ICryptography"/>
        /// </summary>
        public ICryptography Cryptography { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public IMetabaseCacheFactory<IAccount, int> AccountCacheFactory { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public MetabaseCacheFactory<IAccountWithElement, int> AccountWithElementsCacheFactory { get; }

        /// <summary>
        /// Mock'd <see cref="ICache{T,TK}"/>
        /// </summary>
        public ICache<IAccountWithElement, int> AccountWithElementsCache { get; }

        /// <summary>
        /// Mock'd <see cref="ICache{T,TK}"/>
        /// </summary>
        public ICache<IAccount, int> AccountCache { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public MetabaseCacheFactory<IElement, int> ElementsCacheFactory { get; }

        /// <summary>
        /// Mock'd <see cref="DatabaseServerRepository"/>
        /// </summary>
        public IDataFactory<IDatabaseServer, int> DatabaseServerRepository { get; }

        /// <summary>
        /// Mock'd <see cref="IMetabaseDataConnection{T}"/>
        /// </summary>
        public IMetabaseDataConnection<SqlParameter> AccountMetabaseDataConnections { get; }

        /// <summary>
        /// Mock'd <see cref="IMetabaseDataConnection{T}"/>
        /// </summary>
        public IMetabaseDataConnection<SqlParameter> AccountWithElementsMetabaseDataConnections { get; }

        /// <summary>
        /// Mock'd <see cref="IMetabaseDataConnection{T}"/>
        /// </summary>
        public IMetabaseDataConnection<SqlParameter> ElementMetabaseDataConnections { get; }

        /// <summary>
        /// System Under Test - <see cref="SqlAccountWithElementsFactory"/>
        /// </summary>
        public SqlAccountWithElementsFactory SUT { get; }

        /// <summary>
        /// Mock'd Backing Instance of the <see cref="SqlAccountFactory"/>
        /// </summary>
        public SqlAccountFactory SqlAccountFactory { get; }

        /// <summary>
        /// Mock'd Backing Instance of the <see cref="SqlElementFactory"/>
        /// </summary>
        public SqlElementFactory SqlElementFactory { get; }

        public SqlAccountWithElementsFactoryFixture()
        {
            this.AccountCacheFactory = Substitute.For<IMetabaseCacheFactory<IAccount, int>>();

            this.AccountCache = Substitute.For<ICache<IAccount, int>>();

            this.AccountWithElementsCache = Substitute.For<ICache<IAccountWithElement, int>>();

            this.AccountWithElementsCacheFactory = Substitute.For<MetabaseCacheFactory<IAccountWithElement, int>>(new RepositoryBase<IAccountWithElement, int>(Substitute.For<ILog>()), this.AccountWithElementsCache, Substitute.For<MetabaseCacheKey<int>>(), Substitute.For<ILog>());

            this.ElementsCacheFactory = Substitute.For<MetabaseCacheFactory<IElement, int>>(new RepositoryBase<IElement, int>(Substitute.For<ILog>()), Substitute.For<ICache<IElement, int>>(), Substitute.For<MetabaseCacheKey<int>>(), Substitute.For<ILog>());

            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();
            this.Cryptography = Substitute.For<ICryptography>();
            
            this.AccountMetabaseDataConnections = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            this.AccountMetabaseDataConnections.Parameters = new SqlDataParameters();

            this.AccountWithElementsMetabaseDataConnections = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            this.AccountWithElementsMetabaseDataConnections.Parameters = new SqlDataParameters();

            this.ElementMetabaseDataConnections = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            this.ElementMetabaseDataConnections.Parameters = new SqlDataParameters();

            this.SqlAccountFactory = new SqlAccountFactory(this.AccountCacheFactory, this.AccountMetabaseDataConnections, this.DatabaseServerRepository, this.Cryptography);

            this.SqlElementFactory = new SqlElementFactory(this.ElementsCacheFactory, this.ElementMetabaseDataConnections);

            this.SUT = new SqlAccountWithElementsFactory(this.AccountWithElementsCacheFactory, this.SqlAccountFactory, this.SqlElementFactory, this.AccountWithElementsMetabaseDataConnections);
        }
    }
}

