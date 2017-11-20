using BusinessLogic.ProjectCodes;

namespace CacheDataAccess.Tests.Caching
{
    using System;
    using System.Collections.Generic;
    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using CacheDataAccess.Caching;
    using Common.Logging;
    using Common.Logging.NullLogger;
    using NSubstitute;
    using NSubstitute.ReturnsExtensions;
    using Xunit;

    public class CacheFactoryTests
    {
        public class AccountCacheFactoryCtor : CacheFactoryFixture
        {
            [Fact]
            public void NullBaseRepository_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new AccountCacheFactory<IProjectCode, int>(null, Substitute.For<ICache<IProjectCode, int>>(), new AccountCacheKey<int>(Substitute.For<IAccount>()), this.Log));
            }

            [Fact]
            public void NullCache_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new AccountCacheFactory<IProjectCode, int>(new RepositoryBase<IProjectCode, int>(this.Log), null, new AccountCacheKey<int>(Substitute.For<IAccount>()), this.Log));
            }

            [Fact]
            public void NullCacheKey_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new AccountCacheFactory<IProjectCode, int>(new RepositoryBase<IProjectCode, int>(this.Log), null, new AccountCacheKey<int>(Substitute.For<IAccount>()), this.Log));
            }

            [Fact]
            public void ValidParameters_Ctor_CanAccessCacheFactory()
            {
                AccountCacheFactory<IProjectCode, int> sut = new AccountCacheFactory<IProjectCode, int>(new RepositoryBase<IProjectCode, int>(this.Log), Substitute.For<ICache<IProjectCode, int>>(), new AccountCacheKey<int>(Substitute.For<IAccount>()), this.Log);
                IProjectCode projectCode = sut.Add(Substitute.For<IProjectCode>());

                Assert.NotNull(projectCode);
            }
        }

        public class Ctor : CacheFactoryFixture
        {
            [Fact]
            public void NullBaseRepository_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new MetabaseCacheFactory<IAccount, int>(null, this.Cache, this.CacheKey, this.Log));
            }

            [Fact]
            public void NullCache_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, null, this.CacheKey, this.Log));
            }

            [Fact]
            public void NullCacheKey_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, null, this.Log));
            }
        }

        public class Add : CacheFactoryFixture
        {
            [Fact]
            public void Account_Add_ShouldAddToMemoryAndCache()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                Assert.Equal(account, sut.Add(account));

                // Make sure the method to add to memory was also called

                Assert.True(this.RepositoryBase[account.Id] != null);

                // Make sure the method to add to cache was also called
                this.Cache.ReceivedWithAnyArgs().HashAdd(Arg.Any<IMetabaseCacheKey<int>>(), "list", account.Id.ToString(), Arg.Any<IAccount>());
            }
        }

        public class AddCollection : CacheFactoryFixture
        {
            [Fact]
            public void NullEntitiesParameter_Add_ThrowsNullArgumentException()
            {
                MetabaseCacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);
                Assert.Throws<ArgumentNullException>(() => sut.Add((IList<IAccount>)null));
            }

            [Fact]
            public void ZeroLenthParameter_Add_DoesNotAddToCache()
            {
                this.RepositoryBase = Substitute.For<RepositoryBase<IAccount, int>>(this.Log);
                MetabaseCacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                IList<IAccount> response = sut.Add(new List<IAccount>());

                Assert.Equal(0, response.Count);
                this.RepositoryBase.Received(0).Add(Arg.Any<IAccount>());
            }

            [Fact]
            public void MultipleEntryParameter_Add_RepositoryBaseAddCalledCorrectly()
            {
                this.RepositoryBase = Substitute.For<RepositoryBase<IAccount, int>>(this.Log);
                MetabaseCacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                IList<IAccount> accounts = new List<IAccount>
                {
                    new Account(1, null, false),
                    new Account(2, null, false),
                    new Account(3, null, false)
                };

                int onAddEventTimesCalled = 0;
                sut.OnAdd += delegate
                {
                    onAddEventTimesCalled++;
                };

                int onAddMultipleEventTimesCalled = 0;
                sut.OnAddMultiple += delegate
                {
                    onAddMultipleEventTimesCalled++;
                };

                IList<IAccount> response = sut.Add(accounts);

                Assert.Equal(3, response.Count);

                this.RepositoryBase.Received(accounts.Count).Add(Arg.Any<IAccount>());
                this.Cache.Received(1).HashAdd(Arg.Any<ICacheKey<int>>(), Arg.Any<string>(), Arg.Any<IDictionary<string, object>>());

                Assert.True(this.RepositoryBase[1] != null);
                Assert.True(this.RepositoryBase[2] != null);
                Assert.True(this.RepositoryBase[3] != null);
                Assert.True(onAddEventTimesCalled == 0);
                Assert.True(onAddMultipleEventTimesCalled == 1);
            }
        }

        public class Indexer : CacheFactoryFixture
        {

            [Fact]
            public void NotLoadedAccount_Indexer_ShouldReturnNull()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                this.Cache.HashGet(Arg.Any<IMetabaseCacheKey<int>>(), "list", account.Id.ToString()).ReturnsNull();
                Assert.Null(sut[account.Id]);
            }

            [Fact]
            public void AccountInMemory_Indexer_DoesNotCallAddOrCache()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;
                this.RepositoryBase.Add(account);

                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                // Check that we get the correct account back
                Assert.Equal(account, sut[account.Id]);

                // Check that get cache was not called
                this.Cache.DidNotReceiveWithAnyArgs().StringGet(Arg.Any<CacheKey<int>>());
            }

            [Fact]
            public void AccountNotInMemory_Indexer_ShouldGetFromCacheAndBaseRepositoryAdd()
            {
                IAccount account = Substitute.For<IAccount>();
                account.Id = 1;

                // Tell the cache to return the correct result
                CacheKey<int> cacheKey = Substitute.For<CacheKey<int>>();
                this.Cache.HashGet(cacheKey, "list", account.Id.ToString()).ReturnsForAnyArgs(account);
                this.Cache.ClearReceivedCalls();
                
                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                Assert.Equal(account, sut[account.Id]);

                // make sure cache is called
                this.Cache.ReceivedWithAnyArgs(1).HashGet(cacheKey, "list", account.Id.ToString());

                // make sure base repo add is called
                Assert.True(this.RepositoryBase[account.Id] != null);
            }
        }

        public class Get : CacheFactoryFixture
        {
            [Fact]
            public void MultipleCachedItems_Get_ReturnsAll()
            {
                this.Cache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), Arg.Any<string>()).Returns(new List<IAccount>()
                {
                    new Account(1, null, false),
                    new Account(2, null, false),
                    new Account(3, null, false)
                });

                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);
                IList<IAccount> accounts = sut.Get();

                Assert.Equal(3, accounts.Count);
                Assert.Equal(1, accounts[0].Id);
                Assert.Equal(2, accounts[1].Id);
                Assert.Equal(3, accounts[2].Id);

            }

            [Fact]
            public void ZeroCachedItems_Get_ReturnsNull()
            {
                this.Cache.HashGetAll(Arg.Any<IMetabaseCacheKey<int>>(), Arg.Any<string>()).ReturnsNull();

                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);
                IList<IAccount> accounts = sut.Get();

                Assert.Null(accounts);
            }
        }
        
        public class HashDelete : CacheFactoryFixture
        {
            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void NullEmptyOrWhiteSpaceHashName_HashDelete_ThrowsArgumentNullException(string value)
            {
                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                Assert.Throws<ArgumentNullException>(() => sut.HashDelete(value, "hashField"));
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void NullEmptyOrWhiteSpaceField_HashDelete_ThrowsArgumentNullException(string value)
            {
                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                Assert.Throws<ArgumentNullException>(() => sut.HashDelete("hashName", value));
            }

            [Fact]
            public void Valid_HashDelete_DeletesFromCache()
            {
                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                sut.HashDelete("hashName", "hashField");

                this.Cache.Received(1).HashDelete(Arg.Any<ICacheKey<int>>(), "hashName", "hashField");
            }
        }

        public class HashGet : CacheFactoryFixture
        {
            [Fact]
            public void AccountInCache_HashGet_GetsAccountFromCache()
            {
                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);

                this.Cache.HashGet(Arg.Any<IMetabaseCacheKey<int>>(), "hashName", "hashField").Returns(new Account(55, null, false));

                IAccount response = sut.HashGet("hashName", "hashField");

                this.Cache.Received(1).HashGet(Arg.Any<IMetabaseCacheKey<int>>(), "hashName", "hashField");
                Assert.NotNull(response);
            }

            [Fact]
            public void AccountNotCache_HashGet_ReturnsNull()
            {
                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);
                this.Cache.HashGet(Arg.Any<IMetabaseCacheKey<int>>(), "hashName", "hashField").ReturnsNull();
                IAccount response = sut.HashGet("hashName", "hashField");

                Assert.Null(response);
                this.Cache.Received(1).HashGet(Arg.Any<IMetabaseCacheKey<int>>(), "hashName", "hashField");
            }
        }

        public class Delete : CacheFactoryFixture
        {
            [Fact]
            public void Delete_Called_CallsCacheDelete()
            {
                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);
                this.RepositoryBase.Add(new Account(55, null, false));
                this.Cache.HashDelete(Arg.Any<IMetabaseCacheKey<int>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

                // Check that it returns the correct response as from the ICache
                Assert.Equal(true, sut.Delete(55));

                // Check that the ICache was called
                this.Cache.Received(1).HashDelete(Arg.Any<IMetabaseCacheKey<int>>(), "list", "55");
                Assert.True(this.RepositoryBase[55] != null);
            }

            [Fact]
            public void Delete_Called_CacheKeyIdPropertySet()
            {
                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);
                this.Cache.HashDelete(Arg.Any<IMetabaseCacheKey<int>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

                sut.Delete(55);
                Assert.Equal(55, this.CacheKey.Key);
            }

            [Fact]
            public void DeleteWithEventRegistered_DeleteCalled_EventRaised()
            {
                CacheFactory<IAccount, int> sut = new MetabaseCacheFactory<IAccount, int>(this.RepositoryBase, this.Cache, this.CacheKey, this.Log);
                this.Cache.HashDelete(Arg.Any<IMetabaseCacheKey<int>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

                bool onDeleteEventCalled = false;
                sut.OnDelete += delegate
                {
                    onDeleteEventCalled = true;
                };

                bool onDeleteMultipleEventCalled = false;
                sut.OnDeleteMultiple += delegate
                {
                    onDeleteMultipleEventCalled = true;
                };

                sut.Delete(55);

                // make sure the event was raised
                Assert.True(onDeleteEventCalled);
                Assert.False(onDeleteMultipleEventCalled);
            }
        }

        public abstract class CacheFactoryFixture
        {
            public RepositoryBase<IAccount, int> RepositoryBase { get; set; }

            public ICache<IAccount, int> Cache { get; }

            public IMetabaseCacheKey<int> CacheKey { get; }

            public ILog Log { get; }

            protected CacheFactoryFixture()
            {
                this.Log = new NullLoggerWrapper();
                this.Cache = Substitute.For<ICache<IAccount, int>>();
                this.RepositoryBase = new RepositoryBase<IAccount, int>(this.Log);
                this.CacheKey = new MetabaseCacheKey<int>();
            }
        }
    }
}