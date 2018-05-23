namespace SqlDataAccess.Tests.Reasons
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
    using BusinessLogic.Identity;
    using BusinessLogic.Reasons;

    using CacheDataAccess.Caching;
    using CacheDataAccess.Reasons;

    using Common.Logging;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SQLDataAccess.Reasons;

    using SqlDataAccess.Tests.Helpers;

    using Xunit;

    public class SqlReasonTests
    {
        /// <summary>
        /// Constructor tests for <see cref="SqlReasonsFactory"/>
        /// </summary>
        public class Constructor : SqlReasonsFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="CacheFactory{T,TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCache_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlReasonsFactory(null, this.CustomerDataConnection, this.IdentityProvider, this.Logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ICustomerDataConnection{T}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCustomerDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlReasonsFactory(this.CacheFactory, null, this.IdentityProvider, this.Logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IdentityProvider"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullIdentityProvider_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlReasonsFactory(this.CacheFactory, this.CustomerDataConnection, null, this.Logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ILog"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullLogger_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlReasonsFactory(this.CacheFactory, this.CustomerDataConnection, this.IdentityProvider, null));
            }
        }

        /// <summary>
        /// Tests to ensure the Indexer of <see cref="SqlReasonsFactory"/> deliver the expected results.
        /// </summary>
        public class Indexer : SqlReasonsFactoryFixture
        {
            /// <summary>
            /// Test to ensure if an <see cref="IReason"/> is in <see cref="ICache{T,TK}"/> within a <see cref="CacheFactory{T,TK}"/> and has a matching ID that it is returned before retrieving from the Get method of <see cref="SqlReasonsFactory"/>.
            /// </summary>
            [Fact]
            public void ReasonInCache_Indexer_ShouldGetReasonFromCache()
            {
                IReason Reason = Substitute.For<IReason>();
                Reason.Id = 77;

                this.CacheFactory[Reason.Id].Returns(Reason);
                this.CacheFactory.ClearReceivedCalls();

                // Ensure the Reason returned is the one from cache
                Assert.Equal(Reason, this.SUT[Reason.Id]);

                // If the Reason is not in cache a get and an add will occur, this tests that no add was called
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(Reason);
            }

            /// <summary>
            /// Attempting to get an instance of <see cref="IReason"/> which does not exist in <see cref="CacheFactory{T,TK}"/> and is not returned by the Get method of <see cref="SqlReasonsFactory"/> should return <see langword="null"/> and not insert this into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Fact]
            public void GetReasonThatDoesNotExistInCacheOrSQL_Indexer_ShouldReturnNullAndNotAddToCache()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.StringGet(Arg.Any<AccountCacheKey<int>>()).ReturnsNull();
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(2);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.CustomerDataConnection.ClearReceivedCalls();

                IReason returnedReason = this.SUT[69];

                Assert.Null(returnedReason);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.DidNotReceiveWithAnyArgs().Save(Arg.Any<IReason>());
            }

            /// <summary>
            /// Getting an <see cref="IReason"/> which does not exist in <see cref="CacheFactory{T,TK}"/> should call the Get method on <see cref="SqlReasonsFactory"/> and insert into <see cref="CacheFactory{T,TK}"/>
            /// </summary>
            [Theory]
            [InlineData(1, false, "desc1", "Bob", "", "", 1, 2)]
            [InlineData(2, true, "desc2", "Fred", "AccountCodeVat", "AccountCodeNoVat", 3, 4)]
            [InlineData(3, false, "desc3", "Laura", "Foo", "Bar", 1123124134, 13451345)]
            public void GetReasonNotInCache_Indexer_GetsFromDatabaseAndAddsToCache(int id, bool archived, string description, string name, string accountCodeVat, string accountCodeNoVat, int createdBy, int modifiedBy)
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[69].ReturnsNull();

                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.Cache.HashGet(Arg.Any<IAccountCacheKey<int>>(), "list", "69").ReturnsNull();
                this.CacheFactory[69].ReturnsNull();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(id, name, description, accountCodeVat, accountCodeNoVat, new DateTime(2000, 01, 01), createdBy, new DateTime(2000, 01, 02), modifiedBy, archived);
                dataReader.GetOrdinal("reasonid").Returns(0);
                dataReader.GetOrdinal("reason").Returns(1);
                dataReader.GetOrdinal("description").Returns(2);
                dataReader.GetOrdinal("accountcodevat").Returns(3);
                dataReader.GetOrdinal("accountcodenovat").Returns(4);
                dataReader.GetOrdinal("Archived").Returns(9);
                dataReader.GetOrdinal("createdon").Returns(5);
                dataReader.GetOrdinal("createdby").Returns(6);
                dataReader.GetOrdinal("modifiedon").Returns(7);
                dataReader.GetOrdinal("modifiedby").Returns(8);

                // Make the mocked IMetabaseDataConnection.GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.CustomerDataConnection.ClearReceivedCalls();

                // Make sure the record returned populates the Reason correctly
                IReason returnedReason = this.SUT[69];

                Assert.Equal(id, returnedReason.Id);
                Assert.Equal(name, returnedReason.Name);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.CustomerDataConnection.Received(1).GetReader(Arg.Any<string>());

                // Ensure that when an IAccount is returned from the SqlAccountFactory Get method it is then inserted into cache.
                this.CacheFactory.ReceivedWithAnyArgs(1).Save(Arg.Any<IReason>());
            }
        }

        /// <summary>
        /// Tests to ensure the Add method of <see cref="SqlReasonsFactory"/> deliver the expected results.
        /// </summary>
        public class Add : SqlReasonsFactoryFixture
        {
            /// <summary>
            /// Adding a <see cref="IReason"/> to <see cref="SqlReasonsFactory"/> should add the <see cref="IReason"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="SqlReasonsFactory"/>
            /// </summary>
            [Theory]
            [InlineData(11)]
            [InlineData(2)]
            [InlineData(4)]
            public void ValidReason_Add_ShouldAddToCache(int id)
            {
                IReason Reason = Substitute.For<IReason>();

                // Mock save store proc to return valid Id
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(id);

                // Ensure debug level is set to true
                this.Logger.IsDebugEnabled.Returns(true);

                // Add the Reason
                this.SUT.Save(Reason);

                // Ensure the Reason was added to cache
                this.CacheFactory.Received(1).Save(Reason);

                // Ensure Id was set correctly on returned Reason
                Assert.Equal(id, Reason.Id);
            }

            /// <summary>
            /// Attempt to add an invalid <see cref="IReason"/> to <see cref="SqlReasonsFactory"/> should not add the <see cref="IReason"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="SqlReasonsFactory"/>
            /// </summary>
            [Fact]
            public void InvalidReason_Add_ShouldNotAddToCache()
            {
                IReason Reason = Substitute.For<IReason>();
                Reason.Id = 10;

                // Mock save store proc to return invalid Id
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(-1);

                // Ensure debug level is set to true
                this.Logger.IsDebugEnabled.Returns(true);

                // Add the Reason
                this.SUT.Save(Reason);

                // Ensure the Reason was not added to cache
                this.CacheFactory.DidNotReceive().Save(Reason);

                // Ensure the returned Reason has an invalid Id
                Assert.Equal(-1, Reason.Id);
            }

            /// <summary>
            /// Adding <see langword="null"/> to <see cref="SqlReasonsFactory"/> should return null
            /// </summary>
            [Fact]
            public void NullEntity_Add_ShouldReturnNull()
            {
                Assert.Null(this.SUT.Save(null));
            }
        }

        /// <summary>
        /// Tests to ensure the Get method of <see cref="SqlReasonsFactory"/> deliver the expected results.
        /// </summary>
        public class Get : SqlReasonsFactoryFixture
        {
            /// <summary>
            /// Get a list of <see cref="Reason"/> from cache
            /// </summary>
            [Fact]
            public void GetReasonsInCache_Get_ShouldReturnListOfReasons()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of Reasons for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").Returns(new List<IReason>()
                {
                    new Reason(1, false, "desc1", "Bob", null, null, 1, DateTime.Now, null, null),
                    new Reason(2, true, "desc2", "Fred", null, null, 2, DateTime.Now.AddDays(-1), null, null),
                    new Reason(3, false, "desc3", "Laura", null, null, 3, DateTime.Now.AddDays(-2), null, null)
                });

                // Make sure the record returned populates the Reasons correctly
                var returnedReason = this.SUT.Get();

                Assert.Equal(1, returnedReason[0].Id);
                Assert.Equal("Bob", returnedReason[0].Name);

                Assert.Equal(2, returnedReason[1].Id);
                Assert.Equal("Fred", returnedReason[1].Name);

                Assert.Equal(3, returnedReason[2].Id);
                Assert.Equal("Laura", returnedReason[2].Name);
            }

            /// <summary>
            /// Get a <see cref="Reason"/> from data reader as cache is empty
            /// </summary>
            [Theory]
            [InlineData(1, false, "desc1", "Bob", "", "", 1, 2)]
            [InlineData(2, true, "desc2", "Fred", "AccountCodeVat", "AccountCodeNoVat", 3, 4)]
            [InlineData(3, false, "desc3", "Laura", "Foo", "Bar", 1123124134, 13451345)]
            public void GetReasonsInCache_WhereCacheIsEmpty_Get_ShouldReturnASingleReasonFromTheReader(int id, bool archived, string description, string name, string accountCodeVat, string accountCodeNoVat, int createdBy, int modifiedBy)
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of Reasons for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").ReturnsNullForAnyArgs();

                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(id, name, description, accountCodeVat, accountCodeNoVat, new DateTime(2000, 01, 01), createdBy, new DateTime(2000, 01, 02), modifiedBy, archived);
                dataReader.GetOrdinal("reasonid").Returns(0);
                dataReader.GetOrdinal("reason").Returns(1);
                dataReader.GetOrdinal("description").Returns(2);
                dataReader.GetOrdinal("accountcodevat").Returns(3);
                dataReader.GetOrdinal("accountcodenovat").Returns(4);
                dataReader.GetOrdinal("Archived").Returns(9);
                dataReader.GetOrdinal("createdon").Returns(5);
                dataReader.GetOrdinal("createdby").Returns(6);
                dataReader.GetOrdinal("modifiedon").Returns(7);
                dataReader.GetOrdinal("modifiedby").Returns(8);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.CustomerDataConnection.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.CustomerDataConnection.ClearReceivedCalls();

                // Make sure the record returned populates the Reasons correctly
                var returnedReason = this.SUT.Get();

                Assert.Equal(id, returnedReason[0].Id);
                Assert.Equal(name, returnedReason[0].Name);
                Assert.Equal(description, returnedReason[0].Description);
                Assert.Equal(archived, returnedReason[0].Archived);
                Assert.Equal(accountCodeVat, returnedReason[0].AccountCodeVat);
                Assert.Equal(accountCodeNoVat, returnedReason[0].AccountCodeNoVat);
                Assert.Equal(createdBy, returnedReason[0].CreatedBy);
                Assert.Equal(modifiedBy, returnedReason[0].ModifiedBy);
                Assert.Equal(new DateTime(2000, 01, 01), returnedReason[0].CreatedOn);
                Assert.Equal(new DateTime(2000, 01, 02), returnedReason[0].ModifiedOn);
            }
        }

        /// <summary>
        /// Tests to ensure the Get (passing a IReason predicate) method of <see cref="SqlReasonsFactory"/> deliver the expected results.
        /// </summary>
        public class GetByPredicate : SqlReasonsFactoryFixture
        {
            /// <summary>
            /// Add a list of <see cref="Reason"/> to cache and then get a matching Reason by a predicate
            /// </summary>
            [Fact]
            public void GetReasonInCacheWithMatchingPredicate_GetByPredicate_ShouldReturnMatchingReason()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of Reasons for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").Returns(new List<IReason>()
                {
                    new Reason(1, false, "desc1", "Bob", null, null, 1, DateTime.Now, null, null),
                    new Reason(2, true, "desc2", "Fred", null, null, 2, DateTime.Now.AddDays(-1), null, null),
                    new Reason(3, false, "desc3", "Laura", null, null, 3, DateTime.Now.AddDays(-2), null, null)
                });

                // Make sure the record returned populates the Reasons correctly
                var returnedReason = this.SUT.Get(Reason => Reason.Id == 1);

                Assert.Equal(1, returnedReason[0].Id);
                Assert.Equal("Bob", returnedReason[0].Name);
            }

            /// <summary>
            /// Pass <see langword="null"/> to the Get by predicate method and ensure an empty list is returned as no <see cref="Reason"/> were added to cache
            /// </summary>
            [Fact]
            public void GetReasonsFromCacheWithoutPredicate_NoReasonsInCache_GetByPredicate_ShouldReturnAnEmptyList()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the record returned populates the Reasons correctly
                var returnedReason = this.SUT.Get(null);

                Assert.Equal(0, returnedReason.Count);
            }

            /// <summary>
            /// Add a list of <see cref="Reason"/> to cache and then pass <see langword="null"/> to the get by predicate method and ensure the full list from cache is returned
            /// </summary>
            [Fact]
            public void GetReasonsFromCacheWithoutPredicate_WithReasonsInCache_GetByPredicate_ShouldReturnAListOfReasons()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                //Make sure the mocked cache factory returns a list of Reasons for any requests (imitate being in cache)
                this.Cache.HashGetAll(Arg.Any<IAccountCacheKey<int>>(), "list").Returns(new List<IReason>()
               {
                    new Reason(1, false, "desc1", "Bob", null, null, 1, DateTime.Now, null, null),
                    new Reason(2, true, "desc2", "Fred", null, null, 2, DateTime.Now.AddDays(-1), null, null),
                    new Reason(3, false, "desc3", "Laura", null, null, 3, DateTime.Now.AddDays(-2), null, null)
               });

                // Make sure the record returned populates the Reasons correctly
                var returnedReason = this.SUT.Get(null);

                Assert.Equal(3, returnedReason.Count);

                Assert.Equal(1, returnedReason[0].Id);
                Assert.Equal("Bob", returnedReason[0].Name);

                Assert.Equal(2, returnedReason[1].Id);
                Assert.Equal("Fred", returnedReason[1].Name);

                Assert.Equal(3, returnedReason[2].Id);
                Assert.Equal("Laura", returnedReason[2].Name);
            }
        }

        /// <summary>
        /// Tests to ensure the GetByCustom method of <see cref="SqlReasonsFactory"/> deliver the expected results.
        /// </summary>
        public class GetByCustom : SqlReasonsFactoryFixture
        {
            /// <summary>
            /// Add a list of <see cref="Reason"/> to cache and then get a matching Reason by a predicate
            /// </summary>
            [Fact]
            public void GetReasonInCacheWithMatchingName_GetByCustom_ShouldReturnMatchingReason()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Make sure the mocked cache factory returns a list of Reasons for any requests (imitate being in cache)
                this.Cache.HashGet(Arg.Any<IAccountCacheKey<int>>(), "names", "Bob").Returns(
                    new Reason(1, false, "desc1", "Bob", null, null, 1, DateTime.Now, null, null));

                // Make sure the record returned populates the Reasons correctly
                var returnedReason = this.SUT.GetByCustom(new GetByReasonName("Bob"));

                Assert.NotNull(returnedReason);

                this.Cache.Received(1).HashGet(Arg.Any<IAccountCacheKey<int>>(), "names", "Bob");
            }
        }

        /// <summary>
        /// Tests to ensure the GetByCustom method of <see cref="SqlReasonsFactory"/> deliver the expected results.
        /// </summary>
        public class Archive : SqlReasonsFactoryFixture
        {
            /// <summary>
            /// Attempt to archive a <see cref="Reason"/> returns valid response and adds to cache
            /// </summary>
            [Fact]
            public void ArchiveReasonReturnsValidCode_Archive_ShouldReturnValidResponseAndUpdateCache()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[77].ReturnsNull();

                //Mock reason
                IReason reason = Substitute.For<IReason>();
                reason.Id = 77;
                reason.Archived = false;

                //Make sure cache returns reason
                this.CacheFactory[reason.Id].Returns(reason);

                // Mock save store proc to return valid return code
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(0);

                var returnCode = this.SUT.Archive(reason.Id);

                Assert.Equal(0, returnCode);

                //Make sure cache received call
                this.CacheFactory.Received(1).Save(reason);
            }

            /// <summary>
            /// Attempt to archive a <see cref="Reason"/> returns invalid response and doesn't add to cache
            /// </summary>
            [Fact]
            public void ArchiveReasonReturnsInvalidCode_Archive_ShouldReturnInValidResponseAndNotAddToCache()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[77].ReturnsNull();

                //Mock reason
                IReason reason = Substitute.For<IReason>();
                reason.Id = 77;
                reason.Archived = false;

                //Make sure cache returns reason
                this.CacheFactory[reason.Id].Returns(reason);

                // Mock save store proc to return invalid return code
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(-1);

                var returnCode = this.SUT.Archive(reason.Id);

                Assert.Equal(-1, returnCode);

                //Make sure cache did not received call
                this.CacheFactory.Received(0).Save(reason);
            }
        }

        /// <summary>
        /// Tests to ensure the Delete method of <see cref="SqlReasonsFactory"/> deliver the expected results.
        /// </summary>
        public class Delete : SqlReasonsFactoryFixture
        {
            /// <summary>
            /// Delete a Reason from the database and delete it from cache
            /// </summary>
            [Fact]
            public void DeleteReason_Delete_ShouldDeleteFromCache()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Ensure debug level is set to true
                this.Logger.IsDebugEnabled.Returns(true);

                // Mock save store proc to return valid return code
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(0);

                // Delete the Reason
                var returnCode = this.SUT.Delete(1);

                // Ensure a valid return code was returned
                Assert.Equal(0, returnCode);

                // Ensure the call to delete the Reason from cache was called
                this.CacheFactory.ReceivedWithAnyArgs().Delete(Arg.Any<int>());
            }

            /// <summary>
            /// Fail to delete a <see cref="Reason"/> from the database and make sure it isnt delete from cache
            /// </summary>
            [Fact]
            public void DeleteReason_Delete_ShouldNotDeleteFromCache()
            {
                // Make sure the request from memory returns null (imitates not being in memory)
                this.RepositoryBase[32].ReturnsNull();

                // Ensure debug level is set to true
                this.Logger.IsDebugEnabled.Returns(true);

                // Mock save store proc to return an invalid return code
                this.CustomerDataConnection.ExecuteProc<int>(Arg.Any<string>()).Returns(-1);

                // Delete the Reason
                var returnCode = this.SUT.Delete(1);

                // Ensure the return code matches the exceute proc value
                Assert.Equal(-1, returnCode);

                // Ensure the method to delete the P11DCateogry from cache was not called
                this.CacheFactory.DidNotReceiveWithAnyArgs().Delete(Arg.Any<int>());
            }
        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="SqlReasonsFactory"/>
    /// </summary>
    public class SqlReasonsFactoryFixture
    {
        /// <summary>
        /// Mock'd <see cref="ILog"/>
        /// </summary>
        public ILog Logger { get; }

        /// <summary>
        /// Mock'd <see cref="RepositoryBase{T, TK}"/>
        /// </summary>
        public RepositoryBase<IReason, int> RepositoryBase { get; }

        /// <summary>
        /// Mock'd <see cref="ICache{T,TK}"/>
        /// </summary>
        public ICache<IReason, int> Cache { get; }

        /// <summary>
        /// Mock'd <see cref="CacheKey{TK}"/>
        /// </summary>
        public CacheKey<int> CacheKey { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public AccountCacheFactory<IReason, int> CacheFactory { get; }

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
        /// System Under Test - <see cref="SqlReasonsFactory"/>
        /// </summary>
        public SqlReasonsFactory SUT { get; }

        public SqlReasonsFactoryFixture()
        {
            this.Logger = Substitute.For<ILog>();
            this.RepositoryBase = Substitute.For<RepositoryBase<IReason, int>>(this.Logger);

            this.IdentityProvider = Substitute.For<IdentityProvider>(new IdentityContextStub(22, 22));
            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();

            this.Cache = Substitute.For<ICache<IReason, int>>();
            this.CacheKey = Substitute.For<AccountCacheKey<int>>(new Account(79, null, false));
            this.CacheFactory =
                Substitute
                    .For<AccountCacheFactory<IReason, int>
                    >(new RepositoryBase<IReason, int>(this.Logger), this.Cache, this.CacheKey, this.Logger);

            this.CustomerDataConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();
            this.CustomerDataConnection.Parameters = Substitute.For<DataParameters<SqlParameter>>();

            this.SUT = new SqlReasonsFactory(this.CacheFactory, this.CustomerDataConnection,
                this.IdentityProvider, this.Logger);
        }
    }
}
