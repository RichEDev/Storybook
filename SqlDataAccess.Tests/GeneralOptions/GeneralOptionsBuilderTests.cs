namespace SqlDataAccess.Tests.GeneralOptions
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;

    using BusinessLogic;
    using BusinessLogic.AccountProperties;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Enums;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Identity;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;

    using SQLDataAccess.AccountProperties;
    using SQLDataAccess.Builder;
    using SQLDataAccess.GeneralOptions;

    using Xunit;

    public class GeneralOptionsBuilderTests
    {
        public class WithAll : GeneralOptionsBuilderFixture
        {
            /// <summary>
            /// Get a <see cref="GeneralOptionsBuilder"/> and call WithAll function to create all properties
            /// </summary>
            [Fact]
            public void GeneralOptionsBuilder_WithAll_GetsAGeneralOptionsBuilderAndCreatesAllProperties()
            {
                var generalOptionsBuilder = this.SUT[1].WithAll();

                Assert.NotNull(generalOptionsBuilder);

                PropertyInfo[] properties = typeof(GeneralOptionsBuilder).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    var generalOptionsProperty = property.GetValue(generalOptionsBuilder);

                    if (property.PropertyType != typeof(string))
                    {
                        Assert.NotNull(generalOptionsProperty);
                    }
                }
            }

            /// <summary>
            /// Get a <see cref="GeneralOptionsBuilder"/> and call WithAll function to create all properties and check there properties are populated correctly
            /// </summary>
            [Fact]
            public void GeneralOptionsBuilder_WithAll_GetsAGeneralOptionsBuilderAndCreatesAllPropertiesAndCheckPopulated()
            {
                var generalOptionsBuilder = this.SUT[1].WithAll();

                Assert.NotNull(generalOptionsBuilder);

                PropertyInfo[] properties = typeof(GeneralOptionsBuilder).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    var generalOptionsProperty = property.GetValue(generalOptionsBuilder);

                    if (property.PropertyType == typeof(string)) continue;

                    var subProperties = property.PropertyType.GetProperties();

                    foreach (var subProperty in subProperties)
                    {
                        var subPropertyValue = subProperty.GetValue(generalOptionsProperty);

                        if (subProperty.PropertyType == typeof(bool))
                        {
                            if (subProperty.Name != "LimitFrequency")
                            {
                                Assert.Equal(false, subPropertyValue);
                            }
                        }
                        else if (subProperty.PropertyType == typeof(string))
                        {
                            if (subPropertyValue != null && subProperty.Name != "RetainLabelsTime")
                            {
                                Assert.Equal(string.Empty, subPropertyValue);
                            }
                        }
                        else if (subProperty.PropertyType == typeof(int))
                        {
                            Assert.Equal(0, subPropertyValue);
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Fixture for unit testing <see cref="GeneralOptionsBuilder"/>
    /// </summary>
    public class GeneralOptionsBuilderFixture
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
        /// Initializes a new instance of the <see cref="GeneralOptionsBuilderFixture"/> class. 
        /// </summary>
        public GeneralOptionsBuilderFixture()
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

            this.CreateAccountProperties();

            this.SUT = new SqlGeneralOptionsFactory(this.AccountPropertiesFactory);
        }

        private void CreateAccountProperties()
        {
            var enumsValues = Enum.GetValues(typeof(AccountPropertyKeys)).Cast<AccountPropertyKeys>().ToList();

            var accountProperties = new List<IAccountProperty>();

            foreach (var enumValue in enumsValues)
            {
                accountProperties.Add(new AccountProperty(enumValue.GetDescription(), null, 1));
            }

            this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<string>>(), "list").Returns(accountProperties);
        }
    }
}
