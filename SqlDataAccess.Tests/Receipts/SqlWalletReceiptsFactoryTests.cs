namespace SqlDataAccess.Tests.Receipts
{
    using System;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Elements;
    using BusinessLogic.Identity;
    using BusinessLogic.Images;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;

    using SQLDataAccess;
    using SQLDataAccess.Accounts;
    using SQLDataAccess.Elements;
    using SQLDataAccess.Receipts;

    using Utilities.Cryptography;

    using Xunit;

    public class SqlWalletReceiptsFactoryTests
    {
        /// <summary>
        /// Tests to ensure the Constructor of <see cref="SqlWalletReceiptsFactory"/> deliver the expected results.
        /// </summary>
        public class Constructor : SqlWalletReceiptsFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ICustomerDataConnection{T)"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCustomerDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlWalletReceiptsFactory(
                    null, this.IdentityProvider, this.Logger, this.SqlAccountWithElementsFactory, this.SqlElementFactory, this.ImageConversion, this.ImageManipulation));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IdentityProvider"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullIdentityProvider_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlWalletReceiptsFactory
                    (this.CustomerDataConnection, null, this.Logger, this.SqlAccountWithElementsFactory, this.SqlElementFactory, this.ImageConversion, this.ImageManipulation));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ILog"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullLogger_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlWalletReceiptsFactory(
                    this.CustomerDataConnection, this.IdentityProvider, null, this.SqlAccountWithElementsFactory, this.SqlElementFactory, this.ImageConversion, this.ImageManipulation));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="SqlAccountWithElementsFactory"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullSqlLicencedElementFactory_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlWalletReceiptsFactory(
                    this.CustomerDataConnection, this.IdentityProvider, this.Logger, null, this.SqlElementFactory, this.ImageConversion, this.ImageManipulation));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="SqlElementFactory"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullElementFactory_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlWalletReceiptsFactory(
                    this.CustomerDataConnection, this.IdentityProvider, this.Logger, this.SqlAccountWithElementsFactory, null, this.ImageConversion, this.ImageManipulation));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IImageConversion"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullImageConversion_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlWalletReceiptsFactory(
                    this.CustomerDataConnection, this.IdentityProvider, this.Logger, this.SqlAccountWithElementsFactory, this.SqlElementFactory, null, this.ImageManipulation));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IImageManipulation"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullImageManipulation_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlWalletReceiptsFactory(
                    this.CustomerDataConnection, this.IdentityProvider, this.Logger, this.SqlAccountWithElementsFactory, this.SqlElementFactory, this.ImageConversion, null));
            }

        }

        //TODO: Once SELCloud has been extracted to nuget package add Sql unit tests

        public class Indexer : SqlWalletReceiptsFactoryFixture
        {
        }
        
        public class Save : SqlWalletReceiptsFactoryFixture
        {

        }
        
        public class Get : SqlWalletReceiptsFactoryFixture
        {

        }

        public class GetByPredicate : SqlWalletReceiptsFactoryFixture
        {

        }

        public class Delete : SqlWalletReceiptsFactoryFixture
        {

        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="SqlWalletReceiptsFactoryFixture"/>
    /// </summary>
    public class SqlWalletReceiptsFactoryFixture
    {
        /// <summary>
        /// Mock'd <see cref="ILog"/>
        /// </summary>
        public ILog Logger { get; }

        /// <summary>
        /// Mock'd <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/>
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
        /// System Under Test - <see cref="SqlWalletReceiptsFactory"/>
        /// </summary>
        public SqlWalletReceiptsFactory SUT { get; }

        /// <summary>
        /// Mock'd Backing instance of the <see cref="SqlAccountWithElementsFactory"/>
        /// </summary>
        public SqlAccountWithElementsFactory SqlAccountWithElementsFactory { get; }

        /// <summary>
        /// Mock'd Backing instance of <see cref="SqlElementFactory"/>
        /// </summary>
        public SqlElementFactory SqlElementFactory { get; }

        /// <summary>
        /// An instance of <see cref="IImageConversion"/> for converting <see cref="WalletReceipt"/> to a different file type.
        /// </summary>
        public IImageConversion ImageConversion { get; }

        /// <summary>
        /// An instance of <see cref="IImageManipulation"/> for manipulating <see cref="WalletReceipt"/> image data.
        /// </summary>
        public IImageManipulation ImageManipulation { get; }

        public SqlWalletReceiptsFactoryFixture()
        {
            this.Logger = Substitute.For<ILog>();

            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();

            this.CustomerDataConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();

            this.CustomerDataConnection.Parameters = Substitute.For<DataParameters<SqlParameter>>();

            this.IdentityProvider = Substitute.For<IdentityProvider>(new IdentityContextStub(22, 22));

            var metabaseDataConnection = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            metabaseDataConnection.Parameters = new SqlDataParameters();

            this.ImageConversion = Substitute.For<IImageConversion>();

            this.ImageManipulation = Substitute.For<IImageManipulation>();

            this.SqlElementFactory = new SqlElementFactory(new MetabaseCacheFactory<IElement, int>(Substitute.For<RepositoryBase<IElement, int>>(this.Logger), Substitute.For<ICache<IElement, int>>(), Substitute.For<MetabaseCacheKey<int>>(), this.Logger), metabaseDataConnection);
            
            this.SqlAccountWithElementsFactory = new SqlAccountWithElementsFactory(
                new MetabaseCacheFactory<IAccountWithElement, int>(Substitute.For<RepositoryBase<IAccountWithElement, int>>(this.Logger), Substitute.For<ICache<IAccountWithElement, int>>(), Substitute.For<MetabaseCacheKey<int>>(), this.Logger)
                , new SqlAccountFactory(
                    Substitute.For<IMetabaseCacheFactory<IAccount, int>>()
                    , metabaseDataConnection
                    , Substitute.For<IDataFactory<IDatabaseServer, int>>()
                    , Substitute.For<ICryptography>())
                ,this.SqlElementFactory
                , metabaseDataConnection);

            this.SUT = new SqlWalletReceiptsFactory(this.CustomerDataConnection,this.IdentityProvider, this.Logger,
                this.SqlAccountWithElementsFactory, this.SqlElementFactory, this.ImageConversion, this.ImageManipulation);
        }
    }
}
