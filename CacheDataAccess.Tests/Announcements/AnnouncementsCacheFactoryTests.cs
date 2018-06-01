namespace CacheDataAccess.Tests.Announcements
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic;
    using BusinessLogic.Accounts;
    using BusinessLogic.Announcements;
    using BusinessLogic.Cache;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;
    using BusinessLogic.Validator;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using Xunit;

    /// <summary>
    /// Tests for <see cref="AnnouncementsCacheFactory"/>
    /// </summary>
    public class AnnouncementsCacheFactoryTests
    {
        /// <summary>
        /// Constructor tests for <see cref="AnnouncementsCacheFactory"/>
        /// </summary>
        public class Constructor : RedisAnnouncementFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="CacheFactory{T,TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCache_Constructor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new AnnouncementsCacheFactory(null, this.IdentityProvider, this.Logger, this.Account, this.EmployeeCombinedAccessRoles, this.ReadReceiptFactory));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IdentityProvider"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullIdentityProvider_Constructor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new AnnouncementsCacheFactory(this.CacheFactory, null, this.Logger, this.Account, this.EmployeeCombinedAccessRoles, this.ReadReceiptFactory));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="ILog"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullLogger_Constructor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new AnnouncementsCacheFactory(this.CacheFactory, this.IdentityProvider, null, this.Account, this.EmployeeCombinedAccessRoles, this.ReadReceiptFactory));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="EmployeeCombinedAccessRole"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullEmployeeAccessRoles_Constructor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new AnnouncementsCacheFactory(this.CacheFactory, this.IdentityProvider, this.Logger, this.Account, null, null));
            }
        }

        /// <summary>
        /// Tests to ensure the Indexer of <see cref="AnnouncementsCacheFactory"/> deliver the expected results.
        /// </summary>
        public class Indexer : RedisAnnouncementFactoryFixture
        {
            /// <summary>
            /// Test to ensure if an <see cref="IAnnouncement"/> is in <see cref="ICache{T,TK}"/> within a <see cref="CacheFactory{T,TK}"/> and has a matching ID that it is returned before retrieving from the Get method of <see cref="AnnouncementsCacheFactory"/>.
            /// </summary>
            [Fact]
            public void AnnouncementInCache_Indexer_ShouldGetAnnouncementFromCache()
            {                
                IAnnouncement announcement = Substitute.For<IAnnouncement>();
                announcement.Id = this.GuidOne;

                this.CacheFactory[announcement.Id].Returns(announcement);
                this.CacheFactory.ClearReceivedCalls();

                Assert.Equal(announcement, this.SUT[announcement.Id]);
            }

            /// <summary>
            /// Attempting to get an instance of <see cref="IAnnouncement"/> which does not exist in <see cref="CacheFactory{T,TK}"/> and is not returned by the Get method of <see cref="AnnouncementsCacheFactory"/> should return <see langword="null"/>"/>
            /// </summary>
            [Fact]
            public void GetAnnouncementThatDoesNotExistInCache_Indexer_ShouldReturnNull()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.CacheFactory.Get().ReturnsNull();
                this.CacheFactory[Arg.Any<Guid>()].ReturnsNullForAnyArgs();

                IAnnouncement returnedAnnouncement = this.SUT[Guid.NewGuid()];

                Assert.Null(returnedAnnouncement);
            }            
        }

        /// <summary>
        /// Tests to ensure the Add method of <see cref="AnnouncementsCacheFactory"/> deliver the expected results.
        /// </summary>
        public class Add : RedisAnnouncementFactoryFixture
        {
            /// <summary>
            /// Adding a <see cref="IAnnouncement"/> to <see cref="AnnouncementsCacheFactory"/> should add the <see cref="IAnnouncement"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="AnnouncementsCacheFactory"/>
            /// </summary>
            [Fact]
            public void ValidAnnouncement_Add_ShouldAddToCache()
            {
                IAnnouncement announcement = Substitute.For<IAnnouncement>();
                announcement.Id = this.GuidOne;
                this.Logger.IsDebugEnabled.Returns(true);
                this.SUT.Save(announcement);
                this.CacheFactory.Received(1).Save(announcement);
                Assert.Equal(this.GuidOne, announcement.Id);
            }
            
            /// <summary>
            /// Attempt to add an invalid <see cref="IAnnouncement"/> to <see cref="AnnouncementsCacheFactory"/> should not add the <see cref="IAnnouncement"/> to the <see cref="CacheFactory{T,TK}"/> passed in the constructor of <see cref="AnnouncementsCacheFactory"/>
            /// </summary>
            [Fact]
            public void InvalidAnnouncement_Add_ShouldNotAddToCache()
            {                
                IAnnouncement announcement = Substitute.For<IAnnouncement>();

                this.Logger.IsDebugEnabled.Returns(true);
                this.SUT.Save(null);
                
                // Ensure the announcement was not added
                this.CacheFactory.DidNotReceive().Save(announcement);

                // Ensure the announcement ID is empty
                Assert.Equal(Guid.Empty, announcement.Id);
            }

            /// <summary>
            /// Adding <see langword="null"/> to <see cref="AnnouncementsCacheFactory"/> should return null
            /// </summary>
            [Fact]
            public void NullEntityAddShouldReturnNull()
            {
                Assert.Null(this.SUT.Save(null));
            }
        }

        /// <summary>
        /// Tests to ensure the Delete method of <see cref="AnnouncementsCacheFactory"/> deliver the expected results
        /// </summary>
        public class Delete : RedisAnnouncementFactoryFixture
        {
            /// <summary>
            /// Delete a <see cref="Announcement"/> from the cache
            /// </summary>
            [Fact]
            public void DeleteAnnouncementCategory_Delete_ShouldDeleteFromCache()
            {
                // Make sure the request from memory returns null (mocks nothing in memory)
                this.RepositoryBase[this.GuidThree].ReturnsNull();
                this.Logger.IsDebugEnabled.Returns(true);

                // Make sure the mocked cache returns a list of announcements for requests (mocks announcements being in cache)
                this.CacheFactory.Get().Returns(new List<IAnnouncement>()
                {
                    new Announcement()
                        {
                            Id = this.GuidOne,
                            Message = "Announcement",
                        },                    
                });

                this.CacheFactory.Delete(Arg.Any<Guid>()).Returns(true);
                this.Cache.HashDelete(this.CacheKey, Arg.Any<string>(), Arg.Any<string>()).Returns(true);
                var returnCode = this.SUT.Delete(this.GuidOne);

                // Ensure 1 was returned (successful delete)
                Assert.Equal(1, returnCode);

                // Ensure the call to delete the Announcement from the cache was called
                this.CacheFactory.ReceivedWithAnyArgs().Delete(Arg.Any<Guid>());
            }            
        }

        /// <summary>
        /// Tests to ensure the Get method of <see cref="AnnouncementsCacheFactory"/> deliver the expected results
        /// </summary>
        public class Get : RedisAnnouncementFactoryFixture
        {
            /// <summary>
            /// Get a list of <see cref="Announcement"/> from cache
            /// </summary>
            [Fact]
            public void GetAnnouncementsInCache_Get_ShouldReturnListOfAnnouncements()
            {
                this.RepositoryBase[this.GuidThree].ReturnsNull();

                // Make sure the mocked cache returns a list of announcements for requests (mocks being in cache)
                this.CacheFactory.Get().Returns(new List<IAnnouncement>()
                {
                    new Announcement()
                        {
                            Id = this.GuidOne,
                            Message = "Zeroed",
                            ReadReceipts = this.ReadReceiptsList
                        },
                    new Announcement()
                        {
                            Id = this.GuidTwo,
                            Message = "First",
                            ReadReceipts = this.ReadReceiptsList
                        }
                });

                var returnedAnnouncements = this.SUT.Get();

                Assert.Equal(2, returnedAnnouncements.Count);
                Assert.Equal(this.GuidOne, returnedAnnouncements[0].Id);
                Assert.Equal("Zeroed", returnedAnnouncements[0].Message);
                Assert.Equal(this.ReadReceiptsList, returnedAnnouncements[0].ReadReceipts);
                Assert.Equal(this.GuidTwo, returnedAnnouncements[1].Id);
                Assert.Equal("First", returnedAnnouncements[1].Message);
                Assert.Equal(this.ReadReceiptsList, returnedAnnouncements[1].ReadReceipts);
            }
        }

        /// <summary>
        /// Tests to ensure the GetByPredicate method of <see cref="AnnouncementsCacheFactory"/> deliver the expected results
        /// </summary>
        public class GetByPredicate : RedisAnnouncementFactoryFixture
        {
            /// <summary>
            /// Add a list of <see cref="Announcement"/> to cache and then get a matching Announcement by a predicate
            /// </summary>
            [Fact]
            public void GetAnnouncementInCacheWithMatchingPredicate_GetByPredicate_ShouldReturnMatchingAnnouncement()
            {
                ReadReceipts emptyReadReceipts = new ReadReceipts();
                this.RepositoryBase[this.GuidThree].ReturnsNull();
                this.CacheFactory.Get().Returns(new List<IAnnouncement>()
                {
                    new Announcement()
                        {
                            Id = this.GuidOne,
                            Message = "Zeroed",
                            ReadReceipts = emptyReadReceipts,
                            Active = true,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today
                        },
                    new Announcement()
                        {
                            Id = this.GuidTwo,
                            Message = "First",
                            ReadReceipts = emptyReadReceipts,
                            Active = true,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today
                        },
                    new Announcement()
                        {
                            Id = this.GuidThree,
                            Message = "Second",
                            ReadReceipts = this.ReadReceiptsList,
                            Active = true,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today
                        }
                });

                var returnedAnnouncement = this.SUT.Get(announcement => announcement.ReadReceipts != this.ReadReceiptsList);

                Assert.Equal(2, returnedAnnouncement.Count);
                Assert.Equal(this.GuidOne, returnedAnnouncement[0].Id);
                Assert.Equal("Zeroed", returnedAnnouncement[0].Message);
                Assert.Equal(this.GuidTwo, returnedAnnouncement[1].Id);
                Assert.Equal("First", returnedAnnouncement[1].Message);
            }

            /// <summary>
            /// Add a list of <see cref="Announcement"/> to cache and then pass <see langword="null"/> to the get by predicate method and ensure the full list from cache is returned
            /// </summary>
            [Fact]
            public void GetAnnouncementFromCacheWithoutPredicateWithAnnouncementsInCache_GetByPredicate_ShouldReturnListOfAnnouncements()
            {
                this.RepositoryBase[this.GuidThree].ReturnsNull();
                this.CacheFactory.Get().Returns(new List<IAnnouncement>()
                {
                    new Announcement()
                        {
                            Id = this.GuidOne,
                            Message = "Zeroed",
                            ReadReceipts = this.ReadReceiptsList,
                            Active = true,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today
                        },
                    new Announcement()
                        {
                            Id = this.GuidTwo,
                            Message = "First",
                            ReadReceipts = this.ReadReceiptsList,
                            Active = true,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today
                        }
                });

                var returnedAnnouncement = this.SUT.Get(null);

                Assert.Equal(2, returnedAnnouncement.Count);
                Assert.Equal(this.GuidOne, returnedAnnouncement[0].Id);
                Assert.Equal("Zeroed", returnedAnnouncement[0].Message);
                Assert.Equal(this.ReadReceiptsList, returnedAnnouncement[0].ReadReceipts);
                Assert.Equal(this.GuidTwo, returnedAnnouncement[1].Id);
                Assert.Equal("First", returnedAnnouncement[1].Message);
                Assert.Equal(this.ReadReceiptsList, returnedAnnouncement[1].ReadReceipts);
            }
        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="AnnouncementsCacheFactory"/>
    /// </summary>
    public class RedisAnnouncementFactoryFixture
    {
        /// <summary>
        /// A mocked <see cref="ICache{T,TK}"/>.
        /// </summary>
        public ICache<IAnnouncement, Guid> Cache { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisAnnouncementFactoryFixture"/> class. 
        /// </summary>
        public RedisAnnouncementFactoryFixture()
        {
            this.Logger = Substitute.For<ILog>();
            this.RepositoryBase = Substitute.For<RepositoryBase<IAnnouncement, Guid>>(this.Logger);

            this.IdentityProvider = Substitute.For<IdentityProvider>(new IdentityContextStub(01, 11));

            this.Cache = Substitute.For<ICache<IAnnouncement, Guid>>();
            this.CacheKey = Substitute.For<MetabaseCacheKey<Guid>>();
            this.CacheFactory = Substitute.For<IMetabaseCacheFactory<IAnnouncement, Guid>>();

            this.Account = Substitute.For<IAccount>();
            this.EmployeeCombinedAccessRoles = Substitute.For<IEmployeeCombinedAccessRoles>();

            this.ReadReceiptFactory = Substitute.For<ReadReceiptFactory>();

            this.SUT = new AnnouncementsCacheFactory(
                this.CacheFactory,
                this.IdentityProvider,
                this.Logger,
                this.Account,
                this.EmployeeCombinedAccessRoles,
                this.ReadReceiptFactory);
            this.ReadReceiptsList = new ReadReceipts
                                        {
                                            new ReadReceipt { AccountId = 1234, EmployeeId = 4321 },
                                            new ReadReceipt { AccountId = 987, EmployeeId = 7890 }
                                        };

            this.Validators = new List<IValidator>();
            this.Validators.Add(new NonNhsValidator());

            this.GuidOne = new Guid("FFB37082-708F-44C6-8EE1-2F33F570CCE5");
            this.GuidTwo = new Guid("00582914-CFF4-405F-BF1E-AF1285378575");
            this.GuidThree = new Guid("59DFDABF-E978-4AB4-B117-0A65CD771680");
        }

        /// <summary>
        /// Gets or sets the read receipt factory (Mocked).
        /// </summary>
        public ReadReceiptFactory ReadReceiptFactory { get; set; }

        /// <summary>
        /// Gets mock'd <see cref="ILog"/>
        /// </summary>
        public ILog Logger { get; }

        /// <summary>
        /// Gets mock'd <see cref="RepositoryBase{T, TK}"/>
        /// </summary>
        public RepositoryBase<IAnnouncement, Guid> RepositoryBase { get; }

        /// <summary>
        /// Gets mock'd <see cref="CacheKey{TK}"/>
        /// </summary>
        public MetabaseCacheKey<Guid> CacheKey { get; }

        /// <summary>
        /// Gets mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public IMetabaseCacheFactory<IAnnouncement, Guid> CacheFactory { get; }

        /// <summary>
        /// Gets mock'd <see cref="IdentityProvider"/>
        /// </summary>
        public IdentityProvider IdentityProvider { get; }

        /// <summary>
        /// Gets mock'd <see cref="Account"/>
        /// </summary>
        public IAccount Account { get; }

        /// <summary>
        /// Gets mock'd <see cref="EmployeeCombinedAccessRoles"/>
        /// </summary>
        public IEmployeeCombinedAccessRoles EmployeeCombinedAccessRoles { get; }

        /// <summary>
        /// Gets System Under Test - <see cref="AnnouncementsCacheFactory"/>
        /// </summary>
        public AnnouncementsCacheFactory SUT { get; }

        /// <summary>
        /// Gets mock'd read receipts
        /// </summary>
        public ReadReceipts ReadReceiptsList { get; }

        /// <summary>
        /// Gets mock'd <see cref="GuidOne"/>
        /// </summary>
        public Guid GuidOne { get; }

        /// <summary>
        /// Gets mock'd <see cref="GuidTwo"/>
        /// </summary>
        public Guid GuidTwo { get; }

        /// <summary>
        /// Gets mock'd <see cref="GuidThree"/>
        /// </summary>
        public Guid GuidThree { get; }

        /// <summary>
        /// Gets or sets the mocked <see cref="List{T}"/> of <seealso cref="IValidator"/>
        /// </summary>
        public List<IValidator> Validators { get; set; }
    }
}
