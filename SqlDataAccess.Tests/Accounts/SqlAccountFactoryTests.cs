namespace SqlDataAccess.Tests.Accounts
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;

    using CacheDataAccess.Caching;
    
    using Common.Logging;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SQLDataAccess;
    using SQLDataAccess.Accounts;
    using SqlDataAccess.Tests.Helpers;

    using Utilities.Cryptography;

    using Xunit;

    public class SqlAccountFactoryTests
    {
        /// <summary>
        /// Constructor tests for <see cref="SqlAccountFactory"/>
        /// </summary>
        public class Constructor : SqlAccountFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="CacheFactory{T,TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCache_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountFactory(null, this.MetabaseDataConnections, this.DatabaseServerRepository, this.Cryptography));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IMetabaseDataConnection{T}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullMetabaseDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountFactory(this.CacheFactory, null, this.DatabaseServerRepository, this.Cryptography));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IDataFactory{T,TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullDatabaseRepository_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountFactory(this.CacheFactory, this.MetabaseDataConnections, null, this.Cryptography));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ICryptography"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCryptography_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlAccountFactory(this.CacheFactory, this.MetabaseDataConnections, this.DatabaseServerRepository, null));
            }
        }

        /// <summary>
        /// Tests to ensure the Indexer of <see cref="SqlAccountFactory"/> deliver the expected results.
        /// </summary>
        public class Indexer : SqlAccountFactoryFixture
        {
            /// <summary>
            /// Test to ensure if an <see cref="IAccount"/> is in <see cref="ICache{T,TK}"/> within a <see cref="CacheFactory{T,TK}"/> and has a matching ID that it is returned before retrieving from the Get method of <see cref="SqlAccountFactory"/>.
            /// </summary>
            [Fact]
            public void AccountInCache_Indexer_ShouldGetAccountFromCache()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 77;

                this.CacheFactory[account.Id].Returns(account);
                this.CacheFactory.ClearReceivedCalls();

                // Ensure the account returned is the one from cache
                Assert.Equal(account, this.SUT[account.Id]);

                // If the account is not in cache a get and an add will occur, this tests that no add was called
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(account);
            }

            /// <summary>
            /// Attempting to get an instance of <see cref="IAccount"/> which does not exist in <see cref="CacheFactory{T,TK}"/> and is not returned by the Get method of <see cref="SqlAccountFactory"/> should return <see langword="null"/> and not insert this into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Fact]
            public void GetAccountThatDoesNotExistInCacheOrSQL_Indexer_ShouldReturnNullAndNotAddToCache()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<MetabaseCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(5);

                // Make the mocked IMetabaseDataConnection.GetReader() return the mocked dataReader
                this.MetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.MetabaseDataConnections.ClearReceivedCalls();

                IAccount returnedAccount = this.SUT[69];

                Assert.Null(returnedAccount);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.MetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IAccount>());
            }

            /// <summary>
            /// Getting an <see cref="IAccount"/> which does not exist in <see cref="CacheFactory{T,TK}"/> should call the Get method on <see cref="SqlAccountFactory"/> and insert into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Fact]
            public void GetAccountNotInCache_Indexer_GetsFromDatabaseAndAddsToCache()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[69].ReturnsNull();

                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<IMetabaseCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[69].ReturnsNull();
                
                // Make the decrypt in the mocked IDatabaseServer return 
                this.Cryptography.DecryptString(Arg.Any<string>()).Returns("ThePassword");

                // Make sure any request for a database server returns a server with an ID of 11.
                IDatabaseServer databaseServer = Substitute.For<IDatabaseServer>();
                databaseServer.Id = 11;

                IDatabaseCatalogue databaseCatalogue = Substitute.For<IDatabaseCatalogue>();
                databaseCatalogue.Server.Returns(databaseServer);

                IAccount account = Substitute.For<IAccount>();
                account.DatabaseCatalogue.Returns(databaseCatalogue);

                this.DatabaseServerRepository[69].ReturnsForAnyArgs(databaseServer);

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(69, "TheDatabaseCatalogue", "TheUsername", "ThePassword", 11);

                // Make the mocked IMetabaseDataConnection.GetReader() return the mocked dataReader
                this.MetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.MetabaseDataConnections.ClearReceivedCalls();

                // Make sure the record returned populates the account correctly
                IAccount returnedAccount = this.SUT[69];

                Assert.Equal(69, returnedAccount.Id);
                Assert.Equal("TheDatabaseCatalogue", returnedAccount.DatabaseCatalogue.Catalogue);
                Assert.Equal("TheUsername", returnedAccount.DatabaseCatalogue.Username);
                Assert.Equal("ThePassword", returnedAccount.DatabaseCatalogue.Password);
                Assert.Equal(11, returnedAccount.DatabaseCatalogue.Server.Id);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.MetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IAccount>());
            }
        }

        /// <summary>
        /// Tests to ensure the Add method of <see cref="SqlAccountFactory"/> deliver the expected results.
        /// </summary>
        public class Add : SqlAccountFactoryFixture
        {
            /// <summary>
            /// Adding a <see cref="IAccount"/> to <see cref="SqlAccountFactory"/> should add the <see cref="IAccount"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="SqlAccountFactory"/>
            /// </summary>
            [Fact]
            public void ValidAccount_Add_ShouldAddToCache()
            {
                IAccount account = Substitute.For<IAccount>();

                this.SUT.Save(account);
                this.CacheFactory.Received(1).Save(account);
            }

            [Fact]
            public void NullEntity_Add_ShouldReturnNull()
            {
                Assert.Null(this.SUT.Save(null));
            }
        }

        public class Delete : SqlAccountFactoryFixture
        {
            [Fact]
            public void Call_Delete_ThrowsNotImplemented()
            {
                Assert.Throws<NotImplementedException>(() => this.SUT.Delete(15));
            }
        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="SqlAccountFactory"/>
    /// </summary>
    public class SqlAccountFactoryFixture
    {
        /// <summary>
        /// Mock'd <see cref="ICryptography"/>
        /// </summary>
        public ICryptography Cryptography { get; }

        /// <summary>
        /// Mock'd <see cref="RepositoryBase{T, TK}"/>
        /// </summary>
        public RepositoryBase<IAccount, int> RepositoryBase { get; }

        /// <summary>
        /// Mock'd <see cref="ICache{T,TK}"/>
        /// </summary>
        public ICache<IAccount, int> Cache { get; }

        public CacheKey<int> CacheKey { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public IMetabaseCacheFactory<IAccount, int> CacheFactory { get; }

        /// <summary>
        /// Mock'd <see cref="DatabaseServerRepository"/>
        /// </summary>
        public IDataFactory<IDatabaseServer, int> DatabaseServerRepository { get; }

        /// <summary>
        /// Mock'd <see cref="IMetabaseDataConnection{T}"/>
        /// </summary>
        public IMetabaseDataConnection<SqlParameter> MetabaseDataConnections { get; }

        /// <summary>
        /// System Under Test - <see cref="SqlAccountFactory"/>
        /// </summary>
        public SqlAccountFactory SUT { get; }

        public SqlAccountFactoryFixture()
        {
            ILog logger = Substitute.For<ILog>();
            this.RepositoryBase = Substitute.For<RepositoryBase<IAccount, int>>(logger);

            this.Cache = Substitute.For<ICache<IAccount, int>>();
            this.CacheKey = Substitute.For<MetabaseCacheKey<int>>();
            this.CacheFactory = Substitute.For<IMetabaseCacheFactory<IAccount, int>>();//(this.RepositoryBase, this.Cache, this.CacheKey, logger);

            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();
            this.Cryptography = Substitute.For<ICryptography>();

            //this.MetabaseDataConnections = Substitute.For<MetabaseDataConnection>(new MetabaseCatalogue(this.Cryptography, configurationManager), new SqlDataParameters());
            this.MetabaseDataConnections = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            this.MetabaseDataConnections.Parameters = new SqlDataParameters();

            this.SUT = new SqlAccountFactory(this.CacheFactory, this.MetabaseDataConnections, this.DatabaseServerRepository, this.Cryptography);
        }
    }
}
