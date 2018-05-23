namespace SqlDataAccess.Tests.ProductModules
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Elements;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;
    using BusinessLogic.ProductModules.Elements;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SqlDataAccess.Tests.Helpers;

    using SQLDataAccess;
    using SQLDataAccess.Elements;
    using SQLDataAccess.ProductModules;

    using Utilities.Cryptography;

    using Xunit;

    public class SqlProductModulesWithElementsFactoryTests
    {
        /// <summary>
        /// Tests to ensure the Constructor of <see cref="SqlProductModulesWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class Constructor : SqlProductModulesWithElementsFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="MetabaseCacheFactory{T,TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCacheFactory_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlProductModulesWithElementsFactory(null, this.SqlProductModulesFactory, this.SqlElementFactory, this.IProductModuleWithElementsMetabaseDataConnections));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="SqlProductModulesFactory"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullSqlAccountFactory_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlProductModulesWithElementsFactory(this.ProductModuleWithElementsCacheFactory, null, this.SqlElementFactory, this.IProductModuleWithElementsMetabaseDataConnections));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="SqlElementFactory"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullSqlElementFactory_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlProductModulesWithElementsFactory(this.ProductModuleWithElementsCacheFactory, this.SqlProductModulesFactory, null, this.IProductModuleWithElementsMetabaseDataConnections));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IMetabaseDataConnection{T}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullMetabaseDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlProductModulesWithElementsFactory(this.ProductModuleWithElementsCacheFactory, this.SqlProductModulesFactory, this.SqlElementFactory, null));
            }
        }

        /// <summary>
        /// Tests to ensure the Indexer method of <see cref="SqlProductModulesWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class Indexer : SqlProductModulesWithElementsFactoryFixture
        {
            /// <summary>
            /// Fail to get <see cref="IProductModuleWithElements"/> from cache or sql return null when getting <see cref="IProductModuleWithElements"/> by predicate
            /// </summary>
            [Fact]
            public void FailToGetAccountFromSql_GetByPredicate_ReturnsNull()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.ProductModuleWithElementsCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();
                this.ProductModuleCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);
                DbDataReader IProductModuleWithElementsDataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.ProductModuleMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.ProductModuleMetabaseDataConnections.ClearReceivedCalls();

                this.IProductModuleWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(IProductModuleWithElementsDataReader);
                this.IProductModuleWithElementsMetabaseDataConnections.ClearReceivedCalls();


                IList<IProductModuleWithElements> accountWithElement = this.SUT.Get(predicateAccountWithElement => predicateAccountWithElement.Id == 1);

                Assert.Null(accountWithElement);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.ProductModuleMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IProductModuleWithElements is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.ProductModuleCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IProductModuleWithElements>());
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
                this.ProductModuleMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.ProductModuleMetabaseDataConnections.ClearReceivedCalls();

                IList<IProductModuleWithElements> accountsWithElements = this.SUT.Get(null);

                Assert.Null(accountsWithElements);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.ProductModuleMetabaseDataConnections.Received(0).GetReader(Arg.Any<string>());

                // Ensure that when an IProductModuleWithElements is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.ProductModuleCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IProductModuleWithElements>());
            }

            /// <summary>
            /// Get the <see cref="IProductModuleWithElements"/> from Sql and Decorate them with the list of <see cref="IElement"/> before returning the matching <see cref="IProductModuleWithElements"/> predicate
            /// </summary>
            /// <param name="elementId">Element Id for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="categoryId">Category Id for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="name">Name for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="description">Description for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="canEdit">Can edit state for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="canAdd">Can add state for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="canDelete">Can delete state for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="accessRolesApplicable">Access role applicable state for the <see cref="IProductModuleWithElements"/></param>
            [Theory]
            [InlineData(11, 1, "Foo", "FooBar", false, true, true, true)]
            [InlineData(8, 71, "Test", "Testing", true, false, false, true)]
            [InlineData(51, 341, "Bar", "BarFoo", false, false, false, false)]
            public void GetAccountFromSqlAndDecorateWithElements_GetByPredicate_ReturnsAccountConvertedToAccountWithElement
                (int elementId, int categoryId, string name, string description, bool canEdit, bool canAdd, bool canDelete, bool accessRolesApplicable)
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.ProductModuleWithElementsCacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();
                this.ProductModuleCacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader productModuleDataReader = new DataReaderHelper().GetSingleRecordReader((int)Modules.Expenses, "Expenses", "ExpensesDescription", "ExpensesBrandName", "ExpensesBrandNameHTML");
                productModuleDataReader.GetOrdinal("moduleID").Returns(0);
                productModuleDataReader.GetOrdinal("moduleName").Returns(1);
                productModuleDataReader.GetOrdinal("description").Returns(2);
                productModuleDataReader.GetOrdinal("brandName").Returns(3);
                productModuleDataReader.GetOrdinal("brandNameHTML").Returns(4);

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

                DbDataReader accountsWithElementsDataReader = new DataReaderHelper().GetSingleRecordReader((int)Modules.Expenses, elementId);
                accountsWithElementsDataReader.GetOrdinal("accountID").Returns(0);
                accountsWithElementsDataReader.GetOrdinal("elementID").Returns(1);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.ProductModuleMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(productModuleDataReader);
                this.ProductModuleMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                this.IProductModuleWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountsWithElementsDataReader);
                this.IProductModuleWithElementsMetabaseDataConnections.ClearReceivedCalls();

                var productModuleWithElements = this.SUT[Modules.Expenses];

                Assert.NotNull(productModuleWithElements);

                Assert.Equal(elementId, productModuleWithElements.Elements[0].Id);
                Assert.Equal(categoryId, productModuleWithElements.Elements[0].CategoryId);
                Assert.Equal(name, productModuleWithElements.Elements[0].Name);
                Assert.Equal(description, productModuleWithElements.Elements[0].Description);
                Assert.Equal(canEdit, productModuleWithElements.Elements[0].AccessRolesCanEdit);
                Assert.Equal(canAdd, productModuleWithElements.Elements[0].AccessRolesCanAdd);
                Assert.Equal(canDelete, productModuleWithElements.Elements[0].AccessRolesCanDelete);
                Assert.Equal(accessRolesApplicable, productModuleWithElements.Elements[0].AccessRolesApplicable);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.ProductModuleMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
            }

            /// <summary>
            /// Get the <see cref="IProductModuleWithElements"/> from cache
            /// </summary>
            [Fact]
            public void GetIProductModuleWithElementsFromCache_GetByPredicate_ReturnsIProductModuleWithElements()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                IProductModuleWithElements productModuleWithElements = Substitute.For<IProductModuleWithElements>();
                productModuleWithElements.Id = (int)Modules.Expenses;

                List<IProductModuleWithElements> accounts
                    = Substitute.For<List<IProductModuleWithElements>>();
                accounts.Add(productModuleWithElements);

                this.ProductModuleWithElementsCacheFactory[(int)Modules.Expenses].Returns(productModuleWithElements);

                var accountsWithElements = this.SUT[Modules.Expenses];

                Assert.NotNull(accountsWithElements);
                Assert.Equal(productModuleWithElements.Id, accountsWithElements.Id);
            }
        }

        /// <summary>
        /// Tests to ensure the Add method of <see cref="SqlProductModulesWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class Add : SqlProductModulesWithElementsFactoryFixture
        {
            /// <summary>
            /// Adding a <see cref="IProductModule"/> to <see cref="SqlProductModulesWithElementsFactory"/> should add the <see cref="IProductModuleWithElements"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="SqlProductModulesFactory"/>
            /// </summary>
            [Fact]
            public void ValidProductModule_Add_ShouldAddToCache()
            {
                IProductModuleWithElements productModuleWithElements = Substitute.For<IProductModuleWithElements>();

                // Add the ProductModule
                this.SUT.Save(productModuleWithElements);

                // Ensure the ProductModule was added to cache
                this.ProductModuleWithElementsCacheFactory.Received(1).Save(productModuleWithElements);
            }

            /// <summary>
            /// Adding <see langword="null"/> to <see cref="SqlProductModulesWithElementsFactory"/> should return null
            /// </summary>
            [Fact]
            public void NullEntity_Add_ShouldReturnNull()
            {
                Assert.Null(this.SUT.Save(null));
            }
        }

        /// <summary>
        /// Tests to ensure the Get method of <see cref="SqlProductModulesWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class Get : SqlProductModulesWithElementsFactoryFixture
        {
            /// <summary>
            /// Fail to Get the list of <see cref="IProductModuleWithElements"/> from sql
            /// </summary>
            [Fact]
            public void FailToGetAccountFromSql_Get_ReturnsNull()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.ProductModuleWithElementsCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();
                this.ProductModuleCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.ProductModuleMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.ProductModuleMetabaseDataConnections.ClearReceivedCalls();

                IList<IProductModuleWithElements> accountsWithElements = this.SUT.Get();

                Assert.Null(accountsWithElements);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.ProductModuleMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IProductModuleWithElements is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.ProductModuleCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IProductModuleWithElements>());
            }

            /// <summary>
            /// Get a list of <see cref="IProductModuleWithElements"/> from Sql and Decorate them with the list of <see cref="IElement"/>
            /// </summary>
            /// <param name="elementId">Element Id for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="categoryId">Category Id for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="name">Name for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="description">Description for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="canEdit">Can edit state for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="canAdd">Can add state for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="canDelete">Can delete state for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="accessRolesApplicable">Access role applicable state for the <see cref="IProductModuleWithElements"/></param>
            [Theory]
            [InlineData(11, 1, "Foo", "FooBar", false, true, true, true)]
            [InlineData(8, 71, "Test", "Testing", true, false, false, true)]
            [InlineData(51, 341, "Bar", "BarFoo", false, false, false, false)]
            public void GetAccountFromSqlAndDecorateWithElements_Get_ReturnsAccountConvertedToAccountWithElement
                (int elementId, int categoryId, string name, string description, bool canEdit, bool canAdd, bool canDelete, bool accessRolesApplicable)
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.ProductModuleWithElementsCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();
                this.ProductModuleCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader productModuleDataReader = new DataReaderHelper().GetSingleRecordReader((int)Modules.Expenses, "Expenses", "ExpensesDescription", "ExpensesBrandName", "ExpensesBrandNameHTML");
                productModuleDataReader.GetOrdinal("moduleID").Returns(0);
                productModuleDataReader.GetOrdinal("moduleName").Returns(1);
                productModuleDataReader.GetOrdinal("description").Returns(2);
                productModuleDataReader.GetOrdinal("brandName").Returns(3);
                productModuleDataReader.GetOrdinal("brandNameHTML").Returns(4);

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

                DbDataReader accountsWithElementsDataReader = new DataReaderHelper().GetSingleRecordReader((int)Modules.Expenses, elementId);
                accountsWithElementsDataReader.GetOrdinal("accountID").Returns(0);
                accountsWithElementsDataReader.GetOrdinal("elementID").Returns(1);


                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.ProductModuleMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(productModuleDataReader);
                this.ProductModuleMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                this.IProductModuleWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountsWithElementsDataReader);
                this.IProductModuleWithElementsMetabaseDataConnections.ClearReceivedCalls();

                IList<IProductModuleWithElements> productModuleWithElements = this.SUT.Get();

                Assert.NotNull(productModuleWithElements);

                Assert.Equal(elementId, productModuleWithElements[0].Elements[0].Id);
                Assert.Equal(categoryId, productModuleWithElements[0].Elements[0].CategoryId);
                Assert.Equal(name, productModuleWithElements[0].Elements[0].Name);
                Assert.Equal(description, productModuleWithElements[0].Elements[0].Description);
                Assert.Equal(canEdit, productModuleWithElements[0].Elements[0].AccessRolesCanEdit);
                Assert.Equal(canAdd, productModuleWithElements[0].Elements[0].AccessRolesCanAdd);
                Assert.Equal(canDelete, productModuleWithElements[0].Elements[0].AccessRolesCanDelete);
                Assert.Equal(accessRolesApplicable, productModuleWithElements[0].Elements[0].AccessRolesApplicable);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.ProductModuleMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
                this.ElementMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
                this.IProductModuleWithElementsMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IProductModuleWithElements is returned from the SqlProductModulesWithElementsFactory Get method it is then inserted into cache.
                this.ProductModuleWithElementsCacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IProductModuleWithElements>());
            }

            /// <summary>
            /// Get the <see cref="IProductModuleWithElements"/> from cache
            /// </summary>
            [Fact]
            public void GetIProductModuleWithElementsFromCache_Get_ReturnsIProductModuleWithElements()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)

                IProductModuleWithElements account = Substitute.For<IProductModuleWithElements>();
                account.Id = 1;

                List<IProductModuleWithElements> accounts
                    = Substitute.For<List<IProductModuleWithElements>>();
                accounts.Add(account);

                this.ProductModuleWithElementsCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsForAnyArgs(accounts);

                IList<IProductModuleWithElements> accountsWithElements = this.SUT.Get();

                Assert.NotNull(accountsWithElements);
                Assert.Equal(account.Id, accountsWithElements[0].Id);
            }
        }

        /// <summary>
        /// Tests to ensure the Get By Predicate method of <see cref="SqlProductModulesWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class GetByPredicate : SqlProductModulesWithElementsFactoryFixture
        {
            /// <summary>
            /// Fail to get <see cref="IProductModuleWithElements"/> from cache or sql return null when getting <see cref="IProductModuleWithElements"/> by predicate
            /// </summary>
            [Fact]
            public void FailToGetAccountFromSql_GetByPredicate_ReturnsNull()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.ProductModuleWithElementsCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();
                this.ProductModuleCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);
                DbDataReader IProductModuleWithElementsDataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.ProductModuleMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.ProductModuleMetabaseDataConnections.ClearReceivedCalls();
                
                this.IProductModuleWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(IProductModuleWithElementsDataReader);
                this.IProductModuleWithElementsMetabaseDataConnections.ClearReceivedCalls();


                IList<IProductModuleWithElements> accountWithElement = this.SUT.Get(predicateAccountWithElement => predicateAccountWithElement.Id == 1);

                Assert.Null(accountWithElement);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.ProductModuleMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IProductModuleWithElements is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.ProductModuleCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IProductModuleWithElements>());
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
                this.ProductModuleMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.ProductModuleMetabaseDataConnections.ClearReceivedCalls();

                IList<IProductModuleWithElements> accountsWithElements = this.SUT.Get(null);

                Assert.Null(accountsWithElements);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.ProductModuleMetabaseDataConnections.Received(0).GetReader(Arg.Any<string>());

                // Ensure that when an IProductModuleWithElements is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.ProductModuleCacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IProductModuleWithElements>());
            }

            /// <summary>
            /// Get the <see cref="IProductModuleWithElements"/> from Sql and Decorate them with the list of <see cref="IElement"/> before returning the matching <see cref="IProductModuleWithElements"/> predicate
            /// </summary>
            /// <param name="elementId">Element Id for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="categoryId">Category Id for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="name">Name for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="description">Description for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="canEdit">Can edit state for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="canAdd">Can add state for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="canDelete">Can delete state for the <see cref="IProductModuleWithElements"/></param>
            /// <param name="accessRolesApplicable">Access role applicable state for the <see cref="IProductModuleWithElements"/></param>
            [Theory]
            [InlineData(11, 1, "Foo", "FooBar", false, true, true, true)]
            [InlineData(8, 71, "Test", "Testing", true, false, false, true)]
            [InlineData(51, 341, "Bar", "BarFoo", false, false, false, false)]
            public void GetAccountFromSqlAndDecorateWithElements_GetByPredicate_ReturnsAccountConvertedToAccountWithElement
                (int elementId, int categoryId, string name, string description, bool canEdit, bool canAdd, bool canDelete, bool accessRolesApplicable)
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.ProductModuleWithElementsCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();
                this.ProductModuleCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader productModuleDataReader = new DataReaderHelper().GetSingleRecordReader((int)Modules.Expenses, "Expenses", "ExpensesDescription", "ExpensesBrandName", "ExpensesBrandNameHTML");
                productModuleDataReader.GetOrdinal("moduleID").Returns(0);
                productModuleDataReader.GetOrdinal("moduleName").Returns(1);
                productModuleDataReader.GetOrdinal("description").Returns(2);
                productModuleDataReader.GetOrdinal("brandName").Returns(3);
                productModuleDataReader.GetOrdinal("brandNameHTML").Returns(4);

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

                DbDataReader accountsWithElementsDataReader = new DataReaderHelper().GetSingleRecordReader((int)Modules.Expenses, elementId);
                accountsWithElementsDataReader.GetOrdinal("accountID").Returns(0);
                accountsWithElementsDataReader.GetOrdinal("elementID").Returns(1);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.ProductModuleMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(productModuleDataReader);
                this.ProductModuleMetabaseDataConnections.ClearReceivedCalls();

                this.ElementMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(elementDataReader);
                this.ElementMetabaseDataConnections.ClearReceivedCalls();

                this.IProductModuleWithElementsMetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(accountsWithElementsDataReader);
                this.IProductModuleWithElementsMetabaseDataConnections.ClearReceivedCalls();

                IList<IProductModuleWithElements> productModuleWithElements = this.SUT.Get(predicateIProductModuleWithElements => predicateIProductModuleWithElements.Id == (int)Modules.Expenses);

                Assert.NotNull(productModuleWithElements);

                Assert.Equal(elementId, productModuleWithElements[0].Elements[0].Id);
                Assert.Equal(categoryId, productModuleWithElements[0].Elements[0].CategoryId);
                Assert.Equal(name, productModuleWithElements[0].Elements[0].Name);
                Assert.Equal(description, productModuleWithElements[0].Elements[0].Description);
                Assert.Equal(canEdit, productModuleWithElements[0].Elements[0].AccessRolesCanEdit);
                Assert.Equal(canAdd, productModuleWithElements[0].Elements[0].AccessRolesCanAdd);
                Assert.Equal(canDelete, productModuleWithElements[0].Elements[0].AccessRolesCanDelete);
                Assert.Equal(accessRolesApplicable, productModuleWithElements[0].Elements[0].AccessRolesApplicable);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.ProductModuleMetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
            }

            /// <summary>
            /// Get the <see cref="IProductModuleWithElements"/> from cache
            /// </summary>
            [Fact]
            public void GetIProductModuleWithElementsFromCache_GetByPredicate_ReturnsIProductModuleWithElements()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                IProductModuleWithElements account = Substitute.For<IProductModuleWithElements>();
                account.Id = 1;

                List<IProductModuleWithElements> accounts
                    = Substitute.For<List<IProductModuleWithElements>>();
                accounts.Add(account);

                this.ProductModuleWithElementsCache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsForAnyArgs(accounts);

                IList<IProductModuleWithElements> accountsWithElements = this.SUT.Get(predicateIProductModuleWithElements => predicateIProductModuleWithElements.Id == 1);

                Assert.NotNull(accountsWithElements);
                Assert.Equal(account.Id, accountsWithElements[0].Id);
            }
        }

        /// <summary>
        /// Tests to ensure the Delete method of <see cref="SqlProductModulesWithElementsFactory"/> deliver the expected results.
        /// </summary>
        public class Delete : SqlProductModulesWithElementsFactoryFixture
        {
            /// <summary>
            /// Delete a ProductModule throws not implemented exception
            /// </summary>
            [Fact]
            public void DeleteProductModule_Delete_ThrowsNotImplementedException()
            {
                Assert.Throws<NotImplementedException>(() => this.SUT.Delete((Modules)1));
            }
        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="SqlProductModulesWithElementsFactory"/>
    /// </summary>
    public class SqlProductModulesWithElementsFactoryFixture
    {
        /// <summary>
        /// Mock'd <see cref="ICryptography"/>
        /// </summary>
        public ICryptography Cryptography { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public MetabaseCacheFactory<IProductModule, int> ProductModuleCacheFactory { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public MetabaseCacheFactory<IProductModuleWithElements, int> ProductModuleWithElementsCacheFactory { get; }

        /// <summary>
        /// Mock'd <see cref="ICache{T,TK}"/>
        /// </summary>
        public ICache<IProductModuleWithElements, int> ProductModuleWithElementsCache { get; }

        /// <summary>
        /// Mock'd <see cref="ICache{T,TK}"/>
        /// </summary>
        public ICache<IProductModule, int> ProductModuleCache { get; }

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
        public IMetabaseDataConnection<SqlParameter> ProductModuleMetabaseDataConnections { get; }

        /// <summary>
        /// Mock'd <see cref="IMetabaseDataConnection{T}"/>
        /// </summary>
        public IMetabaseDataConnection<SqlParameter> IProductModuleWithElementsMetabaseDataConnections { get; }

        /// <summary>
        /// Mock'd <see cref="IMetabaseDataConnection{T}"/>
        /// </summary>
        public IMetabaseDataConnection<SqlParameter> ElementMetabaseDataConnections { get; }

        /// <summary>
        /// System Under Test - <see cref="SqlProductModulesWithElementsFactory"/>
        /// </summary>
        public SqlProductModulesWithElementsFactory SUT { get; }

        /// <summary>
        /// Mock'd Backing Instance of the <see cref="SqlProductModulesFactory"/>
        /// </summary>
        public SqlProductModulesFactory SqlProductModulesFactory { get; }

        /// <summary>
        /// Mock'd Backing Instance of the <see cref="SqlElementFactory"/>
        /// </summary>
        public SqlElementFactory SqlElementFactory { get; }

        public SqlProductModulesWithElementsFactoryFixture()
        {
            this.ProductModuleCache = Substitute.For<ICache<IProductModule, int>>();

            this.ProductModuleCacheFactory =
                Substitute
                    .For<MetabaseCacheFactory<IProductModule, int>>(new RepositoryBase<IProductModule, int>(Substitute.For<ILog>()), this.ProductModuleCache, Substitute.For<MetabaseCacheKey<int>>(), Substitute.For<ILog>());


            this.ProductModuleWithElementsCache = Substitute.For<ICache<IProductModuleWithElements, int>>();

            this.ProductModuleWithElementsCacheFactory = Substitute.For<MetabaseCacheFactory<IProductModuleWithElements, int>>(new RepositoryBase<IProductModuleWithElements, int>(Substitute.For<ILog>()), this.ProductModuleWithElementsCache, Substitute.For<MetabaseCacheKey<int>>(), Substitute.For<ILog>());

            this.ElementsCacheFactory = Substitute.For<MetabaseCacheFactory<IElement, int>>(new RepositoryBase<IElement, int>(Substitute.For<ILog>()), Substitute.For<ICache<IElement, int>>(), Substitute.For<MetabaseCacheKey<int>>(), Substitute.For<ILog>());

            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();
            this.Cryptography = Substitute.For<ICryptography>();
            
            this.ProductModuleMetabaseDataConnections = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            this.ProductModuleMetabaseDataConnections.Parameters = new SqlDataParameters();

            this.IProductModuleWithElementsMetabaseDataConnections = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            this.IProductModuleWithElementsMetabaseDataConnections.Parameters = new SqlDataParameters();

            this.ElementMetabaseDataConnections = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            this.ElementMetabaseDataConnections.Parameters = new SqlDataParameters();

            this.SqlProductModulesFactory = new SqlProductModulesFactory(this.ProductModuleCacheFactory, this.ProductModuleMetabaseDataConnections);

            this.SqlElementFactory = new SqlElementFactory(this.ElementsCacheFactory, this.ElementMetabaseDataConnections);

            this.SUT = new SqlProductModulesWithElementsFactory(this.ProductModuleWithElementsCacheFactory, this.SqlProductModulesFactory, this.SqlElementFactory, this.IProductModuleWithElementsMetabaseDataConnections);
        }
    }
}

