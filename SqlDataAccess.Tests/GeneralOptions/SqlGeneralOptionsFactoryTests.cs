namespace SqlDataAccess.Tests.GeneralOptions
{
    using System;
    using System.Collections.Generic;
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

    using SQLDataAccess.AccountProperties;
    using SQLDataAccess.Builder;
    using SQLDataAccess.GeneralOptions;

    using Xunit;

    public class SqlGeneralOptionsFactoryTests
    {
        /// <summary>
        /// Tests to ensure the Indexer method of <see cref="SqlGeneralOptionsFactory"/> deliver the expected results.
        /// </summary>
        public class Indexer : SqlGeneralOptionsFactoryFixture
        {
            /// <summary>
            /// Attempt to GetByPredicate GeneralOptions gets a <see cref="GeneralOptionsBuilder"/>
            /// </summary>
            [Fact]
            public void GeneralOptions_Indexer_GetsAGeneralOptionsBuilder()
            {
                var test = this.SUT[1];

                Assert.NotNull(test);
            }
        }

        /// <summary>
        /// Tests to ensure the Delete method of <see cref="SqlGeneralOptionsFactory"/> deliver the expected results.
        /// </summary>
        public class GetByPredicate : SqlGeneralOptionsFactoryFixture
        {
            /// <summary>
            /// Attempt to GetByPredicate AccountProperty throws <see cref="NotImplementedException"/>
            /// </summary>
            [Fact]
            public void GeneralOptions_GetByPredicate_ShouldThrowArgumentNullException()
            {
                Assert.Throws<NotImplementedException>(
                    () => this.SUT.Get(generalOptions => generalOptions.SubAccountID == 1));
            }
        }

        /// <summary>
        /// Tests to ensure the Get method of <see cref="SqlGeneralOptionsFactory"/> deliver the expected results.
        /// </summary>
        public class Get : SqlGeneralOptionsFactoryFixture
        {
            /// <summary>
            /// Attempt to get AccountProperty throws <see cref="NotImplementedException"/>
            /// </summary>
            [Fact]
            public void GeneralOptions_Get_ShouldThrowArgumentNullException()
            {
                Assert.Throws<NotImplementedException>(() => this.SUT.Get());
            }
        }

        /// <summary>
        /// Tests to ensure the Save method of <see cref="SqlGeneralOptionsFactory"/> deliver the expected results.
        /// </summary>
        public class Save : SqlGeneralOptionsFactoryFixture
        {
            /// <summary>
            /// Attempt to delete AccountProperty throws <see cref="NotImplementedException"/>
            /// </summary>
            [Fact]
            public void GeneralOptions_Save_ShouldThrowArgumentNullException()
            {
                Assert.Throws<NotImplementedException>(
                    () => this.SUT.Save(new GeneralOptionsBuilder(new List<IAccountProperty>())));
            }
        }

        /// <summary>
        /// Tests to ensure the Delete method of <see cref="SqlGeneralOptionsFactory"/> deliver the expected results.
        /// </summary>
        public class Delete : SqlGeneralOptionsFactoryFixture
        {
            /// <summary>
            /// Attempt to delete AccountProperty throws <see cref="NotImplementedException"/>
            /// </summary>
            [Fact]
            public void GeneralOptions_Delete_ShouldThrowArgumentNullException()
            {
                Assert.Throws<NotImplementedException>(() => this.SUT.Delete(1));
            }
        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="SqlGeneralOptionsFactory"/>
    /// </summary>
    public class SqlGeneralOptionsFactoryFixture
    {
        /// <summary>
        /// Gets a Mock'd <see cref="ILog"/>
        /// </summary>
        public ILog Logger { get; }

        /// <summary>
        /// Gets a Mock'd <see cref="RepositoryBase{T,TK}"/>
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
        /// Gets a Mock'd <see cref="CacheFactory{T,TK}"/>
        /// </summary>
        public AccountCacheFactory<IAccountProperty, string> CacheFactory { get; }

        /// <summary>
        /// Gets a Mock'd <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/>
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
        public SqlGeneralOptionsFactory SUT { get; }

        /// <summary>
        /// Gets a System Under Test - <see cref="SqlAccountPropertiesFactory"/>
        /// </summary>
        public SqlAccountPropertiesFactory AccountPropertiesFactory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlGeneralOptionsFactoryFixture"/> class.
        /// </summary>
        public SqlGeneralOptionsFactoryFixture()
        {
            this.Logger = Substitute.For<ILog>();
            this.RepositoryBase = Substitute.For<RepositoryBase<IAccountProperty, string>>(this.Logger);

            this.IdentityProvider = Substitute.For<IdentityProvider>(new IdentityContextStub(22, 22));
            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();

            this.Cache = Substitute.For<ICache<IAccountProperty, string>>();
            this.CacheKey = Substitute.For<AccountCacheKey<string>>(new Account(79, null, false));
            this.CacheFactory = Substitute.For<AccountCacheFactory<IAccountProperty, string>>(
                new RepositoryBase<IAccountProperty, string>(this.Logger),
                this.Cache,
                this.CacheKey,
                this.Logger);

            this.CustomerDataConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();
            this.CustomerDataConnection.Parameters = Substitute.For<DataParameters<SqlParameter>>();

            this.AccountPropertiesFactory = new SqlAccountPropertiesFactory(
                this.CacheFactory,
                this.CustomerDataConnection,
                this.IdentityProvider,
                this.Logger);

            // Make sure the mocked cache factory returns a list of AccountProperties for any requests (imitate being in cache)
            this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<string>>(), "list").Returns(new List<IAccountProperty>()
            {
                new AccountProperty("ref", string.Empty, 1)
                    {
                        PostKey = string.Empty,
                        IsGlobal = false
                    },
                new AccountProperty("ted", string.Empty, 1)
                    {
                        PostKey = string.Empty,
                        IsGlobal = false
                    },
                new AccountProperty("Henry", string.Empty, 1)
                    {
                        PostKey = string.Empty,
                        IsGlobal = false
                    }
            });

            this.SUT = new SqlGeneralOptionsFactory(this.AccountPropertiesFactory);
        }
    }
}
