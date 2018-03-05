namespace SqlDataAccess.Tests.Elements
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;

    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;

    using CacheDataAccess.Caching;

    using Common.Logging;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using SqlDataAccess.Tests.Helpers;
    using SQLDataAccess;
    using SQLDataAccess.Elements;

    using Xunit;

    public class SqlElementFactoryTests
    {
        /// <summary>
        /// Tests to ensure the Constructor of <see cref="SqlElementFactory"/> deliver the expected results.
        /// </summary>
        public class Constructor : SqlElementFactoryFixture
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IMetabaseCacheFactory{T, TK}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullMetabaseCacheFactory_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlElementFactory(null, this.MetabaseDataConnections));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="IMetabaseDataConnection{T}"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullMetabaseDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SqlElementFactory(this.CacheFactory, null));
            }
        }

        /// <summary>
        /// Tests to ensure the Id Indexer method of <see cref="SqlElementFactory"/> deliver the expected results.
        /// </summary>
        public class IdIndexer : SqlElementFactoryFixture
        {
            /// <summary>
            /// Get <see cref="IElement"/> by indexer from cache
            /// </summary>
            [Fact]
            public void ElementInCache_IdIndexer_ShouldGetFromCache()
            {
                IElement element = Substitute.For<IElement>();
                element.Id = 1;

                this.CacheFactory[element.Id].Returns(element);
                this.CacheFactory.ClearReceivedCalls();

                Assert.Equal(element, this.SUT[element.Id]);
            }

            /// <summary>
            /// Get <see cref="IElement"/> by indexer from sql
            /// </summary>
            /// <param name="elementId">Element Id for the <see cref="AccountWithElements"/></param>
            /// <param name="categoryId">Category Id for the <see cref="AccountWithElements"/></param>
            /// <param name="name">Name for the <see cref="AccountWithElements"/></param>
            /// <param name="description">Description for the <see cref="AccountWithElements"/></param>
            /// <param name="canEdit">Can edit state for the <see cref="AccountWithElements"/></param>
            /// <param name="canAdd">Can add state for the <see cref="AccountWithElements"/></param>
            /// <param name="canDelete">Can delete state for the <see cref="AccountWithElements"/></param>
            /// <param name="accessRolesApplicable">Access role applicable state for the <see cref="AccountWithElements"/></param>
            [Theory]
            [InlineData(11, 1, "Foo", "FooBar", false, true, true, true)]
            [InlineData(8, 71, "Test", "Testing", true, false, false, true)]
            [InlineData(51, 341, "Bar", "BarFoo", false, false, false, false)]
            public void ElementNotInCache_IdIndexer_ShouldGetFromDatabase(int elementId, int categoryId, string name, string description, bool canEdit, bool canAdd, bool canDelete, bool accessRolesApplicable)
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(elementId, categoryId, name, description, canEdit, canAdd, canDelete, "", accessRolesApplicable);
                dataReader.GetOrdinal("elementID").Returns(0);
                dataReader.GetOrdinal("categoryID").Returns(1);
                dataReader.GetOrdinal("elementName").Returns(2);
                dataReader.GetOrdinal("description").Returns(3);
                dataReader.GetOrdinal("accessRolesCanEdit").Returns(4);
                dataReader.GetOrdinal("accessRolesCanAdd").Returns(5);
                dataReader.GetOrdinal("accessRolesCanDelete").Returns(6);
                dataReader.GetOrdinal("elementFriendlyName").Returns(7);
                dataReader.GetOrdinal("accessRolesApplicable").Returns(8);

                // Make the mocked IMetabaseDataConnection.GetReader() return the mocked dataReader
                this.MetabaseDataConnections.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.MetabaseDataConnections.ClearReceivedCalls();

                // Make sure the record returned populates the p11DCategory correctly
                IElement returnedElement = this.SUT[elementId];

                Assert.Equal(elementId, returnedElement.Id);
                Assert.Equal(categoryId, returnedElement.CategoryId);
                Assert.Equal(name, returnedElement.Name);
                Assert.Equal(description, returnedElement.Description);
                Assert.Equal(canEdit, returnedElement.AccessRolesCanEdit);
                Assert.Equal(canAdd, returnedElement.AccessRolesCanAdd);
                Assert.Equal(canDelete, returnedElement.AccessRolesCanDelete);
                Assert.Equal(accessRolesApplicable, returnedElement.AccessRolesApplicable);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.MetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
            }

            /// <summary>
            /// Fail to get <see cref="IElement"/> by indexer from sql or cache
            /// </summary>
            [Fact]
            public void ElementDoesNotExist_IdIndexer_ShouldReturnNull()
            {
                // Make sure the mocked cache factory returns null for any requests (imitate not being in cache)
                this.CacheFactory[Arg.Any<int>()].ReturnsNullForAnyArgs();

                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(9);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.MetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.MetabaseDataConnections.ClearReceivedCalls();

                IElement returnedElement = this.SUT[69];

                Assert.Null(returnedElement);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.MetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());

            }
        }

        /// <summary>
        /// Tests to ensure the Name Indexer method of <see cref="SqlElementFactory"/> deliver the expected results.
        /// </summary>
        public class NameIndexer : SqlElementFactoryFixture
        {
            /// <summary>
            /// Get <see cref="IElement"/> by name indexer from sql
            /// </summary>
            /// <param name="elementId">Element Id for the <see cref="AccountWithElements"/></param>
            /// <param name="categoryId">Category Id for the <see cref="AccountWithElements"/></param>
            /// <param name="name">Name for the <see cref="AccountWithElements"/></param>
            /// <param name="description">Description for the <see cref="AccountWithElements"/></param>
            /// <param name="canEdit">Can edit state for the <see cref="AccountWithElements"/></param>
            /// <param name="canAdd">Can add state for the <see cref="AccountWithElements"/></param>
            /// <param name="canDelete">Can delete state for the <see cref="AccountWithElements"/></param>
            /// <param name="accessRolesApplicable">Access role applicable state for the <see cref="AccountWithElements"/></param>
            [Theory]
            [InlineData(11, 1, "Foo", "FooBar", false, true, true, true)]
            [InlineData(8, 71, "Test", "Testing", true, false, false, true)]
            [InlineData(51, 341, "Bar", "BarFoo", false, false, false, false)]
            public void ElementInDB_NameIndexer_ShouldGetFromDatabase(int elementId, int categoryId, string name, string description, bool canEdit, bool canAdd, bool canDelete, bool accessRolesApplicable)
            {
                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetSingleRecordReader(elementId, categoryId, name, description, canEdit, canAdd, canDelete, "", accessRolesApplicable);
                dataReader.GetOrdinal("elementID").Returns(0);
                dataReader.GetOrdinal("categoryID").Returns(1);
                dataReader.GetOrdinal("elementName").Returns(2);
                dataReader.GetOrdinal("description").Returns(3);
                dataReader.GetOrdinal("accessRolesCanEdit").Returns(4);
                dataReader.GetOrdinal("accessRolesCanAdd").Returns(5);
                dataReader.GetOrdinal("accessRolesCanDelete").Returns(6);
                dataReader.GetOrdinal("elementFriendlyName").Returns(7);
                dataReader.GetOrdinal("accessRolesApplicable").Returns(8);

                // Make the mocked IMetabaseDataConnection.GetReader() return the mocked dataReader
                this.MetabaseDataConnections.GetReader(Arg.Any<string>()).ReturnsForAnyArgs(dataReader);
                this.MetabaseDataConnections.ClearReceivedCalls();

                // Make sure the record returned populates the p11DCategory correctly
                IElement returnedElement = this.SUT[name];

                Assert.Equal(elementId, returnedElement.Id);
                Assert.Equal(categoryId, returnedElement.CategoryId);
                Assert.Equal(name, returnedElement.Name);
                Assert.Equal(description, returnedElement.Description);
                Assert.Equal(canEdit, returnedElement.AccessRolesCanEdit);
                Assert.Equal(canAdd, returnedElement.AccessRolesCanAdd);
                Assert.Equal(canDelete, returnedElement.AccessRolesCanDelete);
                Assert.Equal(accessRolesApplicable, returnedElement.AccessRolesApplicable);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.MetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
            }

            /// <summary>
            /// Fail to get <see cref="IElement"/> by name indexer from sql
            /// </summary>
            [Fact]
            public void ElementDoesNotExist_NameIndexer_ShouldReturnNull()
            {
                // Mock the IDataReader
                DbDataReader dataReader = new DataReaderHelper().GetEmptyRecordReader(9);

                // Make the mocked ICustomerDataConnection.GetReader() return the mocked dataReader
                this.MetabaseDataConnections.GetReader(string.Empty).ReturnsForAnyArgs(dataReader);
                this.MetabaseDataConnections.ClearReceivedCalls();

                IElement returnedElement = this.SUT["ExpediteEntry"];

                Assert.Null(returnedElement);

                // Ensure that the get reader method was called to retrieve the data from the IMetabaseDataConnection object
                this.MetabaseDataConnections.Received(1).GetReader(Arg.Any<string>());
            }
        }
    }

    /// <summary>
    /// Fixture for unit testing <see cref="SqlElementFactoryFixture"/>
    /// </summary>
    public class SqlElementFactoryFixture
    {
        /// <summary>
        /// Mock'd <see cref="ILog"/>
        /// </summary>
        public ILog Logger { get; }

        /// <summary>
        /// Mock'd <see cref="IMetabaseDataConnection{T}"/>
        /// </summary>
        public IMetabaseDataConnection<SqlParameter> MetabaseDataConnections { get; }

        /// <summary>
        /// Mock'd <see cref="IdentityProvider"/>
        /// </summary>
        public IdentityProvider IdentityProvider { get; }

        /// <summary>
        /// Mock'd <see cref="CacheFactory{T, TK}"/>
        /// </summary>
        public IMetabaseCacheFactory<IElement, int> CacheFactory { get; }

        /// <summary>
        /// System Under Test - <see cref="SqlElementFactory"/>
        /// </summary>
        public SqlElementFactory SUT { get; }

        public SqlElementFactoryFixture()
        {
            this.Logger = Substitute.For<ILog>();

            this.CacheFactory = Substitute.For<IMetabaseCacheFactory<IElement, int>>();

            this.IdentityProvider = Substitute.For<IdentityProvider>(new IdentityContextStub(22, 22));

            this.MetabaseDataConnections = Substitute.For<IMetabaseDataConnection<SqlParameter>>();
            this.MetabaseDataConnections.Parameters = new SqlDataParameters();

            this.SUT = new SqlElementFactory(this.CacheFactory, this.MetabaseDataConnections);
        }
    }
}
