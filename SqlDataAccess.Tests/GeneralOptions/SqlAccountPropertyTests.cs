namespace SqlDataAccess.Tests.GeneralOptions
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.AccountProperties;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Identity;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SqlDataAccess.Tests.Helpers;
    using SQLDataAccess.AccountProperties;

    using Xunit;

    public class SqlAccountPropertyTests
    {
        /// <summary>
        /// Constructor tests for <see cref="SqlAccountPropertiesFactory"/>
        /// </summary>
        public class Constructor : SqlAccountPropertyFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="CacheFactory{T,TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCache_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountPropertiesFactory(null, this.CustomerDataConnection, this.IdentityProvider, this.Logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ICustomerDataConnection{T}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCustomerDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountPropertiesFactory(this.CacheFactory, null, this.IdentityProvider, this.Logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IdentityProvider"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullIdentityProvider_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountPropertiesFactory(this.CacheFactory, this.CustomerDataConnection, null, this.Logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ILog"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullLogger_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountPropertiesFactory(this.CacheFactory, this.CustomerDataConnection, this.IdentityProvider, null));
            }
        }

        /// <summary>
        /// Tests to ensure the Indexer of <see cref="SqlAccountPropertiesFactory"/> deliver the expected results.
        /// </summary>
        public class Indexer : SqlAccountPropertyFactoryFixture
        {
            /// <summary>
            /// Test to ensure if an <see cref="IAccountProperty"/> is in <see cref="ICache{T,TK}"/> within a <see cref="CacheFactory{T,TK}"/> and has a matching ID that it is returned before retrieving from the Get method of <see cref="SqlAccountPropertiesFactory"/>.
            /// </summary>
            [Fact]
            public void AccountPropertyInCache_Indexer_ShouldGetAccountPropertyFromCache()
            {
                IAccountProperty accountProperty = Substitute.For<IAccountProperty>();
                accountProperty = new AccountProperty("showFullHomeAddress", null, 1);

                this.CacheFactory[accountProperty.Id].Returns(accountProperty);
                this.CacheFactory.ClearReceivedCalls();

                // Ensure the AccountProperty returned is the one from cache
                Assert.Equal(accountProperty, this.SUT[new AccountPropertyCacheKey("showFullHomeAddress", "1")]);

                // If the AccountProperty is not in cache a get and an add will occur, this tests that no add was called
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(accountProperty);
            }

            /// <summary>
            /// Attempting to get an instance of <see cref="IAccountProperty"/> which does not exist in <see cref="CacheFactory{T,TK}"/> and is not returned by the Get method of <see cref="SqlAccountPropertiesFactory"/> should return <see langword="null"/> and not insert this into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Fact]
            public void GetAccountPropertyThatDoesNotExistInCacheOrSQL_Indexer_ShouldReturnNullAndNotAddToCache()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<AccountCacheKey<string>>()).ReturnsNull();
                this.CacheFactory[string.Empty].ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.CustomerDataConnection.ClearReceivedCalls();

                IAccountProperty returnedAccountProperty = this.SUT[new AccountPropertyCacheKey("showFullHomeAddress", "1")];

                Assert.Null(returnedAccountProperty);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccountProperty>());
            }

            /// <summary>
            /// Getting an <see cref="IAccountProperty"/> which does not exist in <see cref="CacheFactory{T,TK}"/> should call the Get method on <see cref="SqlAccountPropertiesFactory"/> and insert into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Theory]
            [InlineData(11, "ref", "", "", false)]
            [InlineData(2, "ted", "", "", false)]
            [InlineData(4, "Henry", "", "", false)]
            public void GetAccountPropertyNotInCache_Indexer_GetsFromDatabaseAndAddsToCache(int subAccountId, string key, string value, string formPostKey, bool isGlobal)
            {
                var cacheKey = new AccountPropertyCacheKey(key, subAccountId.ToString());

                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[cacheKey.CacheKey].ReturnsNull();

                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.HashGet(Arg.Any<IAccountCacheKey<string>>(), "list", "69").ReturnsNull();

                this.CacheFactory[cacheKey.CacheKey].ReturnsNull();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(subAccountId, key, value, formPostKey, isGlobal);
                dataReader.GetOrdinal("subAccountId").Returns(0);
                dataReader.GetOrdinal("stringKey").Returns(1);
                dataReader.GetOrdinal("stringValue").Returns(2);
                dataReader.GetOrdinal("formpostkey").Returns(3);
                dataReader.GetOrdinal("isglobal").Returns(4);

                // Make the mocked IMetabaseDataConnection.GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.CustomerDataConnection.ClearReceivedCalls();

                // Make sure the record returned populates the AccountProperty correctly
                IAccountProperty returnedAccountProperty = this.SUT[cacheKey];

                Assert.Equal(subAccountId, returnedAccountProperty.SubAccountId);
                Assert.Equal(key, returnedAccountProperty.Key);
                Assert.Equal(value, returnedAccountProperty.Value);
                Assert.Equal(formPostKey, returnedAccountProperty.PostKey);
                Assert.Equal(isGlobal, returnedAccountProperty.IsGlobal);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IAccountProperty>());
            }
        }

        /// <summary>
        /// Tests to ensure the Add method of <see cref="SqlAccountPropertiesFactory"/> deliver the expected results.
        /// </summary>
        public class Add : SqlAccountPropertyFactoryFixture
        {
            /// <summary>
            /// Adding a <see cref="IAccountProperty"/> to <see cref="SqlAccountPropertyFactory"/> should add the <see cref="IAccountProperty"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="SqlAccountPropertiesFactory"/>
            /// </summary>
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("false")]
            [InlineData("true")]
            [InlineData("12")]
            public void ValidAccountProperty_Add_ShouldAddToCache(string value)
            {
                IAccountProperty accountProperty = Substitute.For<IAccountProperty>();
                accountProperty = new AccountProperty("showFullHomeAddress", value, 1);

                // Ensure debug level is set to true
                this.Logger.IsDebugEnabled.Returns(true);

                // Add the AccountProperty
                var returnedAccountProperty = this.SUT.Save(accountProperty);

                // Ensure the AccountProperty was added to cache
                this.CacheFactory.Received(1).Save(accountProperty);

                Assert.Equal("showFullHomeAddress", returnedAccountProperty.Key);
            }

            /// <summary>
            /// Adding <see langword="null"/> to <see cref="SqlAccountPropertiesFactory"/> should return null
            /// </summary>
            [Fact]
            public void NullEntity_Add_ShouldReturnNull()
            {
                Assert.Null(this.SUT.Save(null));
            }
        }

        /// <summary>
        /// Tests to ensure the Get method of <see cref="SqlAccountPropertiesFactory"/> deliver the expected results.
        /// </summary>
        public class Get : SqlAccountPropertyFactoryFixture
        {
            /// <summary>
            /// Get a list of <see cref="IAccountProperty"/> from cache
            /// </summary>
            [Fact]
            public void GetAccountPropertiesInCache_Get_ShouldReturnListOfAccountProperties()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[Arg.Any<string>()].ReturnsNull();

                // Make sure the mocked cache factory returns a list of AccountProperties for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<string>>(), "list").Returns(new List<IAccountProperty>()
                {
                    new AccountProperty("ref", string.Empty, 11)
                        {
                            PostKey = string.Empty,
                            IsGlobal = false
                        },
                    new AccountProperty("ted", string.Empty, 2)
                        {
                            PostKey = string.Empty,
                            IsGlobal = false
                        },
                    new AccountProperty("Henry", string.Empty, 4)
                        {
                            PostKey = string.Empty,
                            IsGlobal = false
                        }
                });

                // Make sure the record returned populates the AccountProperties correctly
                var returnedAccountProperty = this.SUT.Get();

                Assert.Equal(3, returnedAccountProperty.Count);

                Assert.Equal("ref", returnedAccountProperty[0].Key);

                Assert.Equal("ted", returnedAccountProperty[1].Key);

                Assert.Equal("Henry", returnedAccountProperty[2].Key);
            }

            /// <summary>
            /// Get a <see cref="IAccountProperty"/> from data reader as cache is empty
            /// </summary>
            [Theory]
            [InlineData(11, "ref", "", "", false)]
            [InlineData(2, "ted", "", "", false)]
            [InlineData(4, "Henry", "", "", false)]
            public void GetAccountPropertiesInCache_WhereCacheIsEmpty_Get_ShouldReturnASingleAccountPropertyFromTheReader(int subAccountId, string key, string value, string formPostKey, bool isGlobal)
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[Arg.Any<string>()].ReturnsNull();

                // Make sure the mocked cache factory returns a list of AccountProperties for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<string>>(), "list").ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(subAccountId, key, value, formPostKey, isGlobal);
                dataReader.GetOrdinal("subAccountId").Returns(0);
                dataReader.GetOrdinal("stringKey").Returns(1);
                dataReader.GetOrdinal("stringValue").Returns(2);
                dataReader.GetOrdinal("formpostkey").Returns(3);
                dataReader.GetOrdinal("isglobal").Returns(4);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.CustomerDataConnection.ClearReceivedCalls();

                // Make sure the record returned populates the AccountProperties correctly
                var returnedAccountProperty = this.SUT.Get()[0];

                Assert.Equal(subAccountId, returnedAccountProperty.SubAccountId);
                Assert.Equal(key, returnedAccountProperty.Key);
                Assert.Equal(value, returnedAccountProperty.Value);
                Assert.Equal(formPostKey, returnedAccountProperty.PostKey);
                Assert.Equal(isGlobal, returnedAccountProperty.IsGlobal);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IAccountProperty>());
            }
        }

        /// <summary>
        /// Tests to ensure the Get (passing a IAccountProperty predicate) method of <see cref="SqlAccountPropertiesFactory"/> deliver the expected results.
        /// </summary>
        public class GetByPredicate : SqlAccountPropertyFactoryFixture
        {
            /// <summary>
            /// Add a list of <see cref="IAccountProperty"/> to cache and then get a matching AccountProperty by a predicate
            /// </summary>
            [Fact]
            public void GetAccountPropertyInCacheWithMatchingPredicate_GetByPredicate_ShouldReturnMatchingAccountProperty()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[Arg.Any<string>()].ReturnsNull();

                // Make sure the mocked cache factory returns a list of AccountProperties for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<string>>(), "list").Returns(new List<IAccountProperty>()
                {
                    new AccountProperty("ref", string.Empty, 11)
                        {
                            PostKey = string.Empty,
                            IsGlobal = false
                        },
                    new AccountProperty("ted", string.Empty, 2)
                        {
                            PostKey = string.Empty,
                            IsGlobal = false
                        },
                    new AccountProperty("Henry", string.Empty, 4)
                        {
                            PostKey = string.Empty,
                            IsGlobal = false
                        }
                });

                // Make sure the record returned populates the AccountProperties correctly
                var returnedAccountProperties = this.SUT.Get(accountProperty => accountProperty.Key == "ref");

                AccountPropertyCacheKey accountPropertyCacheKey = new AccountPropertyCacheKey("ref", "11");

                var returnedAccountProperty = returnedAccountProperties[0];

                Assert.Equal(accountPropertyCacheKey.CacheKey, returnedAccountProperty.Id);
                Assert.Equal(11, returnedAccountProperty.SubAccountId);
                Assert.Equal("ref", returnedAccountProperty.Key);
                Assert.Equal(string.Empty, returnedAccountProperty.Value);
                Assert.Equal(string.Empty, returnedAccountProperty.PostKey);
                Assert.Equal(false, returnedAccountProperty.IsGlobal);
            }

            /// <summary>
            /// Pass <see langword="null"/> to the Get by predicate method and ensure an empty list is returned as no <see cref="IAccountProperty"/> were added to cache
            /// </summary>
            [Fact]
            public void GetAccountPropertiesFromCacheWithoutPredicate_NoAccountPropertiesInCache_GetByPredicate_ShouldReturnAnEmptyList()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[Arg.Any<string>()].ReturnsNull();

                // Make sure the record returned populates the AccountProperties correctly
                var returnedAccountProperties = this.SUT.Get(null);

                Assert.Equal(0, returnedAccountProperties.Count);
            }

            /// <summary>
            /// Add a list of <see cref="IAccountProperty"/> to cache and then pass <see langword="null"/> to the get by predicate method and ensure the full list from cache is returned
            /// </summary>
            [Fact]
            public void GetAccountPropertiesFromCacheWithoutPredicate_WithAccountPropertiesInCache_GetByPredicate_ShouldReturnAListOfAccountProperties()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[Arg.Any<string>()].ReturnsNull();

                //Make sure the mocked cache factory returns a list of AccountProperties for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<string>>(), "list").Returns(new List<IAccountProperty>()
                {
                    new AccountProperty("ref", string.Empty, 11)
                        {
                            PostKey = string.Empty,
                            IsGlobal = false
                        },
                    new AccountProperty("ted", string.Empty, 2)
                        {
                            PostKey = string.Empty,
                            IsGlobal = false
                        },
                    new AccountProperty("Henry", string.Empty, 4)
                        {
                            PostKey = string.Empty,
                            IsGlobal = false
                        }
                });

                // Make sure the record returned populates the AccountProperties correctly
                var returnedAccountProperty = this.SUT.Get(null);

                Assert.Equal(3, returnedAccountProperty.Count);

                Assert.Equal("ref", returnedAccountProperty[0].Key);

                Assert.Equal("ted", returnedAccountProperty[1].Key);

                Assert.Equal("Henry", returnedAccountProperty[2].Key);
            }
        }

        /// <summary>
        /// Tests to ensure the Delete method of <see cref="SqlAccountPropertiesFactory"/> deliver the expected results.
        /// </summary>
        public class Delete : SqlAccountPropertyFactoryFixture
        {
            /// <summary>
            /// Attempt to delete AccountProperty throws <see cref="NotImplementedException"/>
            /// </summary>
            [Fact]
            public void DeleteIAccountProperty_Delete_ShouldThrowArgumentNullException()
            {
                Assert.Throws<NotImplementedException> (() => this.SUT.Delete(new AccountPropertyCacheKey("showFullHomeAddress", "1")));
            }
        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="SqlAccountPropertiesFactory"/>
    /// </summary>
    public class SqlAccountPropertyFactoryFixture
    {
        /// <summary>
        /// Gets a Mock'd <see cref="ILog"/>
        /// </summary>
        public ILog Logger { get; }

        /// <summary>
        /// Gets a Mock'd <see cref="RepositoryBase{T, TK}"/>
        /// </summary>
        public RepositoryBase<IAccountProperty, string> RepositoryBase { get; }

        /// <summary>
        /// Gets a Mock'd <see cref="ICache{T,TK}"/>
        /// </summary>
        public ICache<IAccountProperty, string> Cache { get; }

        /// <summary>
        /// Gets a Mock'd <see cref="CacheKey{TK}"/>
        /// </summary>
        public CacheKey<string> CacheKey { get; }

        /// <summary>
        /// Gets a Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public AccountCacheFactory<IAccountProperty, string> CacheFactory { get; }

        /// <summary>
        /// Gets a Mock'd <see cref="IDataFactory{T, TK}"/>
        /// </summary>
        public IDataFactory<IDatabaseServer, int> DatabaseServerRepository { get; }

        /// <summary>
        /// Gets a Mock'd <see cref="ICustomerDataConnection{T}"/>
        /// </summary>
        public ICustomerDataConnection<SqlParameter> CustomerDataConnection { get; }

        /// <summary>
        /// Gets a Mock'd <see cref="IdentityProvider"/>
        /// </summary>
        public IdentityProvider IdentityProvider { get; }

        /// <summary>
        /// Gets a System Under Test - <see cref="SqlAccountPropertiesFactory"/>
        /// </summary>
        public SqlAccountPropertiesFactory SUT { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAccountPropertyFactoryFixture"/> class.
        /// </summary>
        public SqlAccountPropertyFactoryFixture()
        {
            this.Logger = Substitute.For<ILog>();
            this.RepositoryBase = Substitute.For<RepositoryBase<IAccountProperty, string>>(this.Logger);

            this.IdentityProvider = Substitute.For<IdentityProvider>(new IdentityContextStub(22, 22));
            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();

            this.Cache = Substitute.For<ICache<IAccountProperty, string>>();
            this.CacheKey = Substitute.For<AccountCacheKey<string>>(new Account(79, null, false));
            this.CacheFactory =
                Substitute.For<AccountCacheFactory<IAccountProperty, string>>(
                    new RepositoryBase<IAccountProperty, string>(this.Logger),
                    this.Cache,
                    this.CacheKey,
                    this.Logger);

            this.CustomerDataConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();
            this.CustomerDataConnection.Parameters = Substitute.For<DataParameters<SqlParameter>>();

            this.SUT = new SqlAccountPropertiesFactory(
                this.CacheFactory,
                this.CustomerDataConnection,
                this.IdentityProvider,
                this.Logger);
        }
    }
}
