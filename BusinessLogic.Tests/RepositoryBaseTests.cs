namespace BusinessLogic.Tests
{
    using BusinessLogic.Databases;

    using Common.Logging;
    using Common.Logging.NullLogger;

    using NSubstitute;

    using Xunit;

    public class RepositoryBaseTests
    {
        /// <summary>
        ///  Stub to access abstract <see cref="RepositoryBase{T, TK}"/> - DO NOT IMPLEMENT ANY MEMBERS
        /// </summary>
        public class RepositoryStub : RepositoryBase<IDatabaseServer, int>
        {
            public RepositoryStub(ILog logger) : base(logger)
            {
            }
        }

        public class Add
        {
            private readonly RepositoryStub _sut = new RepositoryStub(new NullLoggerWrapper());

            /// <summary>
            /// Adding a null object to the SUT, this should return null.
            /// </summary>
            [Fact]
            public void Null_Add_IsNull()
            {
                Assert.Null(this._sut.Add(null));
            }

            /// <summary>
            /// Adding a <see cref="IDatabaseServer"/> should then be retrievable by it's Id.
            /// </summary>
            [Fact]
            public void DatabaseServer_Add_GetsTheCorrectDatabaseServer()
            {
                IDatabaseServer databaseServer = Substitute.For<IDatabaseServer>();
                databaseServer.Id = 43;

                this._sut.Add(databaseServer);

                Assert.Equal(databaseServer.Id, this._sut[43].Id);
            }


            /// <summary>
            /// Adding a <see cref="DatabaseServer"/> should remove the original and 
            /// </summary>
            [Fact]
            public void UpdatedDatabaseServer_Add_IsUpdated()
            {
                // Create a database server
                var databaseServerOriginal = Substitute.For<IDatabaseServer>();
                databaseServerOriginal.Id = 42;
                databaseServerOriginal.Hostname.Returns("originalHostname");

                // Create a new version of a database server with the same ID but new hostname
                var databaseServerUpdated = Substitute.For<IDatabaseServer>();
                databaseServerUpdated.Id = 42;
                databaseServerUpdated.Hostname.Returns("updatedHostname");

                this._sut.Add(databaseServerOriginal);
                Assert.Equal(databaseServerOriginal.Hostname, this._sut[42].Hostname);

                this._sut.Add(databaseServerUpdated);
                Assert.Equal(databaseServerUpdated.Hostname, this._sut[42].Hostname);
            }
        }

        public class Indexer
        {
            [Fact]
            public void GetExistingEntity_Indexer_MatchesEntity()
            {
                IDatabaseServer entity = Substitute.For<IDatabaseServer>();
                entity.Id.Returns(51);

                RepositoryStub sut = new RepositoryStub(new NullLoggerWrapper());
                sut.Add(entity);

                Assert.Equal(entity, sut[51]);
            }

            [Fact]
            public void GetNonExistingEntity_Indexer_ReturnsNull()
            {
                IDatabaseServer entity = Substitute.For<IDatabaseServer>();
                entity.Id.Returns(55);

                RepositoryStub sut = new RepositoryStub(new NullLoggerWrapper());

                // Add an entity to make sure it doesn't return a random/first one etc.
                sut.Add(entity);

                Assert.Null(sut[99]);
            }
        }
    }
}
