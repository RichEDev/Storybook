namespace SqlDataAccess.Tests.ProductModules
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SqlDataAccess.Tests.Helpers;

    using SQLDataAccess.ProductModules;

    using Xunit;

    public class SqlProductModuleTests
    {
        /// <summary>
        /// Constructor tests for <see cref="SqlProductModulesFactory"/>
        /// </summary>
        public class Constructor : SqlProductModulesFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="CacheFactory{T,TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCache_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlProductModulesFactory(null, this.MetabaseDataConnection));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IMetabaseDataConnection{T}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCustomerDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlProductModulesFactory(this.CacheFactory, null));
            }
        }

        /// <summary>
        /// Tests to ensure the Indexer of <see cref="SqlProductModulesFactory"/> deliver the expected results.
        /// </summary>
        public class Indexer : SqlProductModulesFactoryFixture
        {
            /// <summary>
            /// Test to ensure if an <see cref="IProductModule"/> is in <see cref="ICache{T,TK}"/> within a <see cref="CacheFactory{T,TK}"/> and has a matching ID that it is returned before retrieving from the Get method of <see cref="SqlProductModulesFactory"/>.
            /// </summary>
            [Fact]
            public void ProductModuleInCache_Indexer_ShouldGetProductModuleFromCache()
            {
                IProductModule ProductModule = Substitute.For<IProductModule>();
                ProductModule.Id = 77;

                this.CacheFactory[ProductModule.Id].Returns(ProductModule);
                this.CacheFactory.ClearReceivedCalls();

                // Ensure the ProductModule returned is the one from cache
                Assert.Equal(ProductModule, this.SUT[(Modules)ProductModule.Id]);

                // If the ProductModule is not in cache a get and an add will occur, this tests that no add was called
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(ProductModule);
            }

            /// <summary>
            /// Attempting to get an instance of <see cref="IProductModule"/> which does not exist in <see cref="CacheFactory{T,TK}"/> and is not returned by the Get method of <see cref="SqlProductModulesFactory"/> should return <see langword="null"/> and not insert this into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Fact]
            public void GetProductModuleThatDoesNotExistInCacheOrSQL_Indexer_ShouldReturnNullAndNotAddToCache()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<AccountCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.MetabaseDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.MetabaseDataConnection.ClearReceivedCalls();

                IProductModule returnedProductModule = this.SUT[(Modules)1];

                Assert.Null(returnedProductModule);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.MetabaseDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IProductModule>());
            }

            /// <summary>
            /// Getting an <see cref="IProductModule"/> which does not exist in <see cref="CacheFactory{T,TK}"/> should call the Get method on <see cref="SqlProductModulesFactory"/> and insert into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Theory]
            [InlineData((int)Modules.Expenses, "Expenses", "ExpensesDescription", "ExpensesBrandName", "ExpensesBrandNameHTML")]
            [InlineData((int)Modules.Contracts, "Framework", "FrameworkDescription", "FBrandName", "FBrandNameHTML")]
            [InlineData((int)Modules.SpendManagement, "SpendManagement", "SpendManagementDescription", "SMBrandName", "SMBrandNameHTML")]
            [InlineData((int)Modules.PurchaseOrders, "PurchaseOrders", "PurchaseOrdersDescription", "PurchaseOrdersBrandName", "PurchaseOrdersBrandNameHTML")]
            [InlineData((int)Modules.Greenlight, "Greenlight", "GreenlightDescription", "GreenlightBrandName", "GreenlightBrandNameHTML")]
            [InlineData((int)Modules.CorporateDiligence, "CorporateDiligence", "CorporateDiligenceDescription", "CorporateDiligenceBrandName", "CorporateDiligenceBrandNameHTML")]
            [InlineData((int)Modules.GreenlightWorkforce, "GreenlightWorkforce", "GreenlightWorkforceDescription", "GreenlightWorkforceBrandName", "GreenlightWorkforceBrandNameHTML")]
            [InlineData((int)Modules.ESR, "ESR", "ESRDescription", "ESRBrandName", "ESRBrandNameHTML")]
            [InlineData(1000, "NullModule", "NullModuleDescription", "NullModuleBrandName", "NullModuleBrandNameHTML")]
            public void GetProductModuleNotInCache_Indexer_GetsFromDatabaseAndAddsToCache(int id, string name, string description, string brandName, string brandNameHtml)
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[id].ReturnsNull();

                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.HashGet(Arg.Any<IMetabaseCacheKey<int>>(), "list", id.ToString()).ReturnsNull();
                this.CacheFactory[id].ReturnsNull();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(id, name, description, brandName, brandNameHtml);
                dataReader.GetOrdinal("moduleID").Returns(0);
                dataReader.GetOrdinal("moduleName").Returns(1);
                dataReader.GetOrdinal("description").Returns(2);
                dataReader.GetOrdinal("brandName").Returns(3);
                dataReader.GetOrdinal("brandNameHTML").Returns(4);

                // Make the mocked IMetabaseDataConnection.GetReader() return the mocked dataReader
                this.MetabaseDataConnection.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.MetabaseDataConnection.ClearReceivedCalls();

                // Make sure the record returned populates the ProductModule correctly
                IProductModule returnedProductModule = this.SUT[(Modules)id];

                Assert.Equal(id, returnedProductModule.Id);
                Assert.Equal(name, returnedProductModule.Name);
                Assert.Equal(description, returnedProductModule.Description);
                Assert.Equal(brandName, returnedProductModule.BrandName);
                Assert.Equal(brandNameHtml, returnedProductModule.BrandNameHtml);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.MetabaseDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IProductModule>());
            }
        }

        /// <summary>
        /// Tests to ensure the Add method of <see cref="SqlProductModulesFactory"/> deliver the expected results.
        /// </summary>
        public class Add : SqlProductModulesFactoryFixture
        {
            /// <summary>
            /// Adding a <see cref="IProductModule"/> to <see cref="SqlProductModulesFactory"/> should add the <see cref="IProductModule"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="SqlProductModulesFactory"/>
            /// </summary>
            [Fact]
            public void ValidProductModule_Add_ShouldAddToCache()
            {
                IProductModule ProductModule = Substitute.For<IProductModule>();

                // Add the ProductModule
                this.SUT.Save(ProductModule);

                // Ensure the ProductModule was added to cache
                this.CacheFactory.Received(1).Save(ProductModule);
            }

            /// <summary>
            /// Adding <see langword="null"/> to <see cref="SqlProductModulesFactory"/> should return null
            /// </summary>
            [Fact]
            public void NullEntity_Add_ShouldReturnNull()
            {
                Assert.Null(this.SUT.Save(null));
            }
        }

        /// <summary>
        /// Tests to ensure the Get method of <see cref="SqlProductModulesFactory"/> deliver the expected results.
        /// </summary>
        public class Get : SqlProductModulesFactoryFixture
        {
            /// <summary>
            /// Get a list of <see cref="ProductModule"/> from cache
            /// </summary>
            [Fact]
            public void GetProductModulesInCache_Get_ShouldReturnListOfProductModules()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of ProductModules for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").Returns(new List<IProductModule>()
                {
                    new ExpensesProductModule((int)Modules.Expenses, "Expenses", "ExpensesDescription", "ExpensesBrandName", "ExpensesBrandNameHTML"),
                    new FrameworkProductModule((int)Modules.Contracts, "Framework", "FrameworkDescription", "FBrandName", "FBrandNameHTML"),
                    new SpendManagementProductModule((int)Modules.SpendManagement, "SpendManagement", "SpendManagementDescription", "SMBrandName", "SMBrandNameHTML")
                });

                // Make sure the record returned populates the ProductModules correctly
                var returnedProductModule = this.SUT.Get();

                Assert.Equal((int)Modules.Expenses, returnedProductModule[0].Id);
                Assert.Equal("Expenses", returnedProductModule[0].Name);
                Assert.Equal("ExpensesDescription", returnedProductModule[0].Description);
                Assert.Equal("ExpensesBrandName", returnedProductModule[0].BrandName);
                Assert.Equal("ExpensesBrandNameHTML", returnedProductModule[0].BrandNameHtml);

                Assert.Equal((int)Modules.Contracts, returnedProductModule[1].Id);
                Assert.Equal("Framework", returnedProductModule[1].Name);
                Assert.Equal("FrameworkDescription", returnedProductModule[1].Description);
                Assert.Equal("FBrandName", returnedProductModule[1].BrandName);
                Assert.Equal("FBrandNameHTML", returnedProductModule[1].BrandNameHtml);

                Assert.Equal((int)Modules.SpendManagement, returnedProductModule[2].Id);
                Assert.Equal("SpendManagement", returnedProductModule[2].Name);
                Assert.Equal("SpendManagementDescription", returnedProductModule[2].Description);
                Assert.Equal("SMBrandName", returnedProductModule[2].BrandName);
                Assert.Equal("SMBrandNameHTML", returnedProductModule[2].BrandNameHtml);
            }

            /// <summary>
            /// Get a <see cref="ProductModule"/> from data reader as cache is empty
            /// </summary>
            [Theory]
            [InlineData((int)Modules.Expenses, "Expenses", "ExpensesDescription", "ExpensesBrandName", "ExpensesBrandNameHTML")]
            [InlineData((int)Modules.Contracts, "Framework", "FrameworkDescription", "FBrandName", "FBrandNameHTML")]
            [InlineData((int)Modules.SpendManagement, "SpendManagement", "SpendManagementDescription", "SMBrandName", "SMBrandNameHTML")]
            public void GetProductModulesInCache_WhereCacheIsEmpty_Get_ShouldReturnASingleProductModuleFromTheReader(int id, string name, string description, string brandName, string brandNameHtml)
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of ProductModules for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(id, name, description, brandName, brandNameHtml);
                dataReader.GetOrdinal("moduleID").Returns(0);
                dataReader.GetOrdinal("moduleName").Returns(1);
                dataReader.GetOrdinal("description").Returns(2);
                dataReader.GetOrdinal("brandName").Returns(3);
                dataReader.GetOrdinal("brandNameHTML").Returns(4);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.MetabaseDataConnection.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.MetabaseDataConnection.ClearReceivedCalls();

                // Make sure the record returned populates the ProductModules correctly
                var returnedProductModule = this.SUT.Get();

                Assert.Equal(id, returnedProductModule[0].Id);
                Assert.Equal(name, returnedProductModule[0].Name);
                Assert.Equal(description, returnedProductModule[0].Description);
                Assert.Equal(brandName, returnedProductModule[0].BrandName);
                Assert.Equal(brandNameHtml, returnedProductModule[0].BrandNameHtml);
            }
        }

        /// <summary>
        /// Tests to ensure the Get (passing a IProductModule predicate) method of <see cref="SqlProductModulesFactory"/> deliver the expected results.
        /// </summary>
        public class GetByPredicate : SqlProductModulesFactoryFixture
        {
            /// <summary>
            /// Add a list of <see cref="ProductModule"/> to cache and then get a matching ProductModule by a predicate
            /// </summary>
            [Fact]
            public void GetProductModuleInCacheWithMatchingPredicate_GetByPredicate_ShouldReturnMatchingProductModule()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of ProductModules for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), "list").Returns(new List<IProductModule>()
                {
                    new ExpensesProductModule((int)Modules.Expenses, "Expenses", "ExpensesDescription", "ExpensesBrandName", "ExpensesBrandNameHTML"),
                    new FrameworkProductModule((int)Modules.Contracts, "Framework", "FrameworkDescription", "FBrandName", "FBrandNameHTML"),
                    new SpendManagementProductModule((int)Modules.SpendManagement, "SpendManagement", "SpendManagementDescription", "SMBrandName", "SMBrandNameHTML")
                });

                // Make sure the record returned populates the ProductModules correctly
                var returnedProductModule = this.SUT.Get(ProductModule => ProductModule.Id == (int)Modules.Expenses);

                Assert.Equal((int)Modules.Expenses, returnedProductModule[0].Id);
                Assert.Equal("Expenses", returnedProductModule[0].Name);
                Assert.Equal("ExpensesDescription", returnedProductModule[0].Description);
                Assert.Equal("ExpensesBrandName", returnedProductModule[0].BrandName);
                Assert.Equal("ExpensesBrandNameHTML", returnedProductModule[0].BrandNameHtml);
            }

            /// <summary>
            /// Pass <see langword="null"/> to the Get by predicate method and ensure an empty list is returned as no <see cref="ProductModule"/> were added to cache
            /// </summary>
            [Fact]
            public void GetProductModulesFromCacheWithoutPredicate_NoProductModulesInCache_GetByPredicate_ShouldReturnAnEmptyList()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the record returned populates the ProductModules correctly
                var returnedProductModule = this.SUT.Get(null);

                Assert.Null(returnedProductModule);
            }
        }
        
        /// <summary>
        /// Tests to ensure the Delete method of <see cref="SqlProductModulesFactory"/> deliver the expected results.
        /// </summary>
        public class Delete : SqlProductModulesFactoryFixture
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
    /// Fixture for unit testing <see cref="SqlProductModulesFactory"/>
    /// </summary>
    public class SqlProductModulesFactoryFixture
    {
        /// <summary>
        /// Mock'd <see cref="RepositoryBase{T, TK}"/>
        /// </summary>
        public RepositoryBase<IProductModule, int> RepositoryBase { get; }

        /// <summary>
        /// Mock'd <see cref="ICache{T,TK}"/>
        /// </summary>
        public ICache<IProductModule, int> Cache { get; }

        /// <summary>
        /// Mock'd <see cref="CacheKey{TK}"/>
        /// </summary>
        public MetabaseCacheKey<int> CacheKey { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public MetabaseCacheFactory<IProductModule, int> CacheFactory { get; }

        /// <summary>
        /// Mock'd <see cref="IDataFactory{T, TK}"/>
        /// </summary>
        public IDataFactory<IDatabaseServer, int> DatabaseServerRepository { get; }

        /// <summary>
        /// Mock'd <see cref="IMetabaseDataConnection{T}"/>
        /// </summary>
        public IMetabaseDataConnection<SqlParameter> MetabaseDataConnection { get; }

        /// <summary>
        /// System Under Test - <see cref="SqlProductModulesFactory"/>
        /// </summary>
        public SqlProductModulesFactory SUT { get; }
        
        public SqlProductModulesFactoryFixture()
        {
            this.RepositoryBase = Substitute.For<RepositoryBase<IProductModule, int>>(Substitute.For<ILog>());

            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();

            this.Cache = Substitute.For<ICache<IProductModule, int>>();
            this.CacheKey = Substitute.For<MetabaseCacheKey<int>>();
            this.CacheFactory =
                Substitute
                    .For<MetabaseCacheFactory<IProductModule, int>>(new RepositoryBase<IProductModule, int>(Substitute.For<ILog>()), this.Cache, Substitute.For<MetabaseCacheKey<int>>(), Substitute.For<ILog>());

            this.MetabaseDataConnection = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            this.MetabaseDataConnection.Parameters = Substitute.For<DataParameters<SqlParameter>>();

            this.SUT = new SqlProductModulesFactory(this.CacheFactory, this.MetabaseDataConnection);
        }
    }
}
