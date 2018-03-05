namespace SqlDataAccess.Tests.P11DCategories
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Collections.Generic;

    using BusinessLogic;
    using BusinessLogic.P11DCategories;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Accounts;
    using BusinessLogic.Identity;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;
    
    using SQLDataAccess.P11DCategories;
    using SqlDataAccess.Tests.Helpers;
    
    using Xunit;

    public class SqlP11DCategoryTests
    {
        /// <summary>
        /// Constructor tests for <see cref="SqlP11DCategoriesFactory"/>
        /// </summary>
        public class Constructor : SqlP11DCategoriesFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="CacheFactory{T,TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCache_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlP11DCategoriesFactory(null, this.CustomerDataConnection, this.IdentityProvider, this.Logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ICustomerDataConnection{T}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCustomerDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlP11DCategoriesFactory(this.CacheFactory, null, this.IdentityProvider, this.Logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IdentityProvider"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullIdentityProvider_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlP11DCategoriesFactory(this.CacheFactory, this.CustomerDataConnection, null, this.Logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ILog"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullLogger_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlP11DCategoriesFactory(this.CacheFactory, this.CustomerDataConnection, this.IdentityProvider, null));
            }
        }

        /// <summary>
        /// Tests to ensure the Indexer of <see cref="SqlP11DCategoriesFactory"/> deliver the expected results.
        /// </summary>
        public class Indexer : SqlP11DCategoriesFactoryFixture
        {
            /// <summary>
            /// Test to ensure if an <see cref="IP11DCategory"/> is in <see cref="ICache{T,TK}"/> within a <see cref="CacheFactory{T,TK}"/> and has a matching ID that it is returned before retrieving from the Get method of <see cref="SqlP11DCategoriesFactory"/>.
            /// </summary>
            [Fact]
            public void P11DcategoryInCache_Indexer_ShouldGetP11DCategoryFromCache()
            {
                IP11DCategory p11DCategory = Substitute.For<IP11DCategory>();
                p11DCategory.Id = 77;

                this.CacheFactory[p11DCategory.Id].Returns(p11DCategory);
                this.CacheFactory.ClearReceivedCalls();

                // Ensure the P11DCategory returned is the one from cache
                Assert.Equal(p11DCategory, this.SUT[p11DCategory.Id]);

                // If the P11DCategory is not in cache a get and an add will occur, this tests that no add was called
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(p11DCategory);
            }

            /// <summary>
            /// Attempting to get an instance of <see cref="IP11DCategory"/> which does not exist in <see cref="CacheFactory{T,TK}"/> and is not returned by the Get method of <see cref="SqlP11DCategoriesFactory"/> should return <see langword="null"/> and not insert this into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Fact]
            public void GetP11DcategoryThatDoesNotExistInCacheOrSQL_Indexer_ShouldReturnNullAndNotAddToCache()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<AccountCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.CustomerDataConnection.ClearReceivedCalls();

                IP11DCategory returnedP11DCategory = this.SUT[69];

                Assert.Null(returnedP11DCategory);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IP11DCategory>());
            }

            /// <summary>
            /// Getting an <see cref="IP11DCategory"/> which does not exist in <see cref="CacheFactory{T,TK}"/> should call the Get method on <see cref="SqlP11DCategoriesFactory"/> and insert into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Theory]
            [InlineData(11, "ref")]
            [InlineData(2, "ted")]
            [InlineData(4, "Henry")]
            public void GetP11DCategoryNotInCache_Indexer_GetsFromDatabaseAndAddsToCache(int id, string name)
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[69].ReturnsNull();

                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.HashGet(Arg.Any<IAccountCacheKey<int>>(), "list", "69").ReturnsNull();
                this.CacheFactory[69].ReturnsNull();
                
                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(id, name);
                dataReader.GetOrdinal("pdname").Returns(1);

                // Make the mocked IMetabaseDataConnection.GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.CustomerDataConnection.ClearReceivedCalls();

                // Make sure the record returned populates the p11DCategory correctly
                IP11DCategory returnedP11DCategory = this.SUT[69];

                Assert.Equal(id, returnedP11DCategory.Id);
                Assert.Equal(name, returnedP11DCategory.Name);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IP11DCategory>());
            }
        }

        /// <summary>
        /// Tests to ensure the Add method of <see cref="SqlP11DCategoriesFactory"/> deliver the expected results.
        /// </summary>
        public class Add : SqlP11DCategoriesFactoryFixture
        {
            /// <summary>
            /// Adding a <see cref="IP11DCategory"/> to <see cref="SqlP11DCategoriesFactory"/> should add the <see cref="IP11DCategory"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="SqlP11DCategoriesFactory"/>
            /// </summary>
            [Theory]
            [InlineData(11)]
            [InlineData(2)]
            [InlineData(4)]
            public void ValidP11DCategory_Add_ShouldAddToCache(int id)
            {
                IP11DCategory p11DCategory = Substitute.For<IP11DCategory>();

                // Mock save store proc to return valid Id
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(id);

                // Ensure debug level is set to true
                this.Logger.IsDebugEnabled.Returns(true);

                // Add the P11DCategory
                this.SUT.Save(p11DCategory);

                // Ensure the P11DCategory was added to cache
                this.CacheFactory.Received(1).Save(p11DCategory);

                // Ensure Id was set correctly on returned P11DCategory
                Assert.Equal(id, p11DCategory.Id);
            }

            /// <summary>
            /// Attempt to add an invalid <see cref="IP11DCategory"/> to <see cref="SqlP11DCategoriesFactory"/> should not add the <see cref="IP11DCategory"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="SqlP11DCategoriesFactory"/>
            /// </summary>
            [Fact]
            public void InvalidP11DCategory_Add_ShouldNotAddToCache()
            {
                IP11DCategory p11DCategory = Substitute.For<IP11DCategory>();
                p11DCategory.Id = 10;

                // Mock save store proc to return invalid Id
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(-1);

                // Ensure debug level is set to true
                this.Logger.IsDebugEnabled.Returns(true);

                // Add the P11DCategory
                this.SUT.Save(p11DCategory);

                // Ensure the P11DCategory was not added to cache
                this.CacheFactory.DidNotReceive().Save(p11DCategory);

                // Ensure the returned P11DCategory has an invalid Id
                Assert.Equal(-1, p11DCategory.Id);
            }

            /// <summary>
            /// Adding <see langword="null"/> to <see cref="SqlP11DCategoriesFactory"/> should return null
            /// </summary>
            [Fact]
            public void NullEntity_Add_ShouldReturnNull()
            {
                Assert.Null(this.SUT.Save(null));
            }
        }

        /// <summary>
        /// Tests to ensure the Get method of <see cref="SqlP11DCategoriesFactory"/> deliver the expected results.
        /// </summary>
        public class Get : SqlP11DCategoriesFactoryFixture
        {
            /// <summary>
            /// Get a list of <see cref="P11DCategory"/> from cache
            /// </summary>
            [Fact]
            public void GetP11DCategoriesInCache_Get_ShouldReturnListOfP11DCategories()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of P11DCategories for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").Returns(new List<IP11DCategory>()
                {
                    new P11DCategory(0, "Bob"),
                    new P11DCategory(1, "Fred"),
                    new P11DCategory (2, "Laura")
                });

                // Make sure the record returned populates the p11DCategories correctly
                var returnedP11DCategory = this.SUT.Get();

                Assert.Equal(0, returnedP11DCategory[0].Id);
                Assert.Equal("Bob", returnedP11DCategory[0].Name);

                Assert.Equal(1, returnedP11DCategory[1].Id);
                Assert.Equal("Fred", returnedP11DCategory[1].Name);

                Assert.Equal(2, returnedP11DCategory[2].Id);
                Assert.Equal("Laura", returnedP11DCategory[2].Name);
            }

            /// <summary>
            /// Get a <see cref="P11DCategory"/> from data reader as cache is empty
            /// </summary>
            [Theory]
            [InlineData(11, "ref")]
            [InlineData(2, "ted")]
            [InlineData(4, "Henry")]
            public void GetP11DCategoriesInCache_WhereCacheIsEmpty_Get_ShouldReturnASingleP11DCategoryFromTheReader(int id, string name)
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of P11DCategories for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(id, name);
                dataReader.GetOrdinal("pdname").Returns(1);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.CustomerDataConnection.ClearReceivedCalls();

                // Make sure the record returned populates the p11DCategories correctly
                var returnedP11DCategory = this.SUT.Get();

                Assert.Equal(id, returnedP11DCategory[0].Id);
                Assert.Equal(name, returnedP11DCategory[0].Name);
            }
        }

        /// <summary>
        /// Tests to ensure the Get (passing a IP11DCategory predicate) method of <see cref="SqlP11DCategoriesFactory"/> deliver the expected results.
        /// </summary>
        public class GetByPredicate : SqlP11DCategoriesFactoryFixture
        {
            /// <summary>
            /// Add a list of <see cref="P11DCategory"/> to cache and then get a matching P11DCategory by a predicate
            /// </summary>
            [Fact]
            public void GetP11DCategoryInCacheWithMatchingPredicate_GetByPredicate_ShouldReturnMatchingP11DCategory()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of P11DCategories for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").Returns(new List<IP11DCategory>()
                {
                    new P11DCategory(32, "Bob"),
                    new P11DCategory(1, "Fred"),
                    new P11DCategory(2, "Laura")
                });

                // Make sure the record returned populates the p11DCategories correctly
                var returnedP11DCategory = this.SUT.Get(p11DCategory => p11DCategory.Id == 32);

                Assert.Equal(32, returnedP11DCategory[0].Id);
                Assert.Equal("Bob", returnedP11DCategory[0].Name);
            }

            /// <summary>
            /// Pass <see langword="null"/> to the Get by predicate method and ensure an empty list is returned as no <see cref="P11DCategory"/> were added to cache
            /// </summary>
            [Fact]
            public void GetP11DCategoriesFromCacheWithoutPredicate_NoP11DCateogriesInCache_GetByPredicate_ShouldReturnAnEmptyList()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the record returned populates the p11DCategories correctly
                var returnedP11DCategory = this.SUT.Get(null);

                Assert.Equal(0, returnedP11DCategory.Count);
            }

            /// <summary>
            /// Add a list of <see cref="P11DCategory"/> to cache and then pass <see langword="null"/> to the get by predicate method and ensure the full list from cache is returned
            /// </summary>
            [Fact]
            public void GetP11DCategoriesFromCacheWithoutPredicate_WithP11DCateogriesInCache_GetByPredicate_ShouldReturnAListOfP11DCategories()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                //Make sure the mocked cache factory returns a list of P11DCategories for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").Returns(new List<IP11DCategory>()
               {
                    new P11DCategory(0, "Bob"),
                    new P11DCategory(1, "Fred"),
                    new P11DCategory(2, "Laura")
               });

                // Make sure the record returned populates the p11DCategories correctly
                var returnedP11DCategory = this.SUT.Get(null);

                Assert.Equal(3, returnedP11DCategory.Count);

                Assert.Equal(0, returnedP11DCategory[0].Id);
                Assert.Equal("Bob", returnedP11DCategory[0].Name);

                Assert.Equal(1, returnedP11DCategory[1].Id);
                Assert.Equal("Fred", returnedP11DCategory[1].Name);

                Assert.Equal(2, returnedP11DCategory[2].Id);
                Assert.Equal("Laura", returnedP11DCategory[2].Name);
            }
        }
        
        /// <summary>
        /// Tests to ensure the Delete method of <see cref="SqlP11DCategoriesFactory"/> deliver the expected results.
        /// </summary>
        public class Delete : SqlP11DCategoriesFactoryFixture
        {
            /// <summary>
            /// Delete a P11DCategory from the database and delete it from cache
            /// </summary>
            [Fact]
            public void DeleteP11DCategory_Delete_ShouldDeleteFromCache()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Ensure debug level is set to true
                this.Logger.IsDebugEnabled.Returns(true);

                // Mock save store proc to return valid return code
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(0);

                // Delete the P11DCategory
                var returnCode = this.SUT.Delete(1);

                // Ensure a valid return code was returned
                Assert.Equal(0, returnCode);

                // Ensure the call to delete the P11DCategory from cache was called
                this.CacheFactory.ReceivedWithAnyArgs().Delete(Arg.Any<int>());
            }

            /// <summary>
            /// Fail to delete a <see cref="P11DCategory"/> from the database and make sure it isnt delete from cache
            /// </summary>
            [Fact]
            public void DeleteP11DCategory_Delete_ShouldNotDeleteFromCache()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Ensure debug level is set to true
                this.Logger.IsDebugEnabled.Returns(true);
                
                // Mock save store proc to return an invalid return code
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(-1);

                // Delete the P11DCategory
                var returnCode = this.SUT.Delete(1);

                // Ensure the return code matches the exceute proc value
                Assert.Equal(-1, returnCode);

                // Ensure the method to delete the P11DCateogry from cache was not called
                this.CacheFactory.DidNotReceiveWithAnyArgs().Delete(Arg.Any<int>());
            }
        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="SqlP11DCategoriesFactory"/>
    /// </summary>
    public class SqlP11DCategoriesFactoryFixture
    {
        /// <summary>
        /// Mock'd <see cref="ILog"/>
        /// </summary>
        public ILog Logger { get; }

        /// <summary>
        /// Mock'd <see cref="RepositoryBase{T, TK}"/>
        /// </summary>
        public RepositoryBase<IP11DCategory, int> RepositoryBase { get; }

        /// <summary>
        /// Mock'd <see cref="ICache{T,TK}"/>
        /// </summary>
        public ICache<IP11DCategory, int> Cache { get; }

        /// <summary>
        /// Mock'd <see cref="CacheKey{TK}"/>
        /// </summary>
        public CacheKey<int> CacheKey { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public AccountCacheFactory<IP11DCategory, int> CacheFactory { get; }

        /// <summary>
        /// Mock'd <see cref="IDataFactory{T, TK}"/>
        /// </summary>
        public IDataFactory<IDatabaseServer, int> DatabaseServerRepository { get; }

        /// <summary>
        /// Mock'd <see cref="ICustomerDataConnection{T}"/>
        /// </summary>
        public ICustomerDataConnection<SqlParameter> CustomerDataConnection { get; }

        /// <summary>
        /// Mock'd <see cref="IdentityProvider"/>
        /// </summary>
        public IdentityProvider IdentityProvider { get; }

        /// <summary>
        /// System Under Test - <see cref="SqlP11DCategoriesFactory"/>
        /// </summary>
        public SqlP11DCategoriesFactory SUT { get; }
        
        public SqlP11DCategoriesFactoryFixture()
        {
            this.Logger = Substitute.For<ILog>();
            this.RepositoryBase = Substitute.For<RepositoryBase<IP11DCategory, int>>(this.Logger);

            this.IdentityProvider = Substitute.For<IdentityProvider>(new IdentityContextStub(22, 22));
            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();

            this.Cache = Substitute.For<ICache<IP11DCategory, int>>();
            this.CacheKey = Substitute.For<AccountCacheKey<int>>(new Account(79, null, false));
            this.CacheFactory =
                Substitute
                    .For<AccountCacheFactory<IP11DCategory, int>
                    >(new RepositoryBase<IP11DCategory, int>(this.Logger), this.Cache, this.CacheKey, this.Logger);

            this.CustomerDataConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();
            this.CustomerDataConnection.Parameters = Substitute.For<DataParameters<SqlParameter>>();

            this.SUT = new SqlP11DCategoriesFactory(this.CacheFactory, this.CustomerDataConnection,
                this.IdentityProvider, this.Logger);
        }
    }
}
