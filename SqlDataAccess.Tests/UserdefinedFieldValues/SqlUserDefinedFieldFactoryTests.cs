namespace SqlDataAccess.Tests.UserdefinedFieldValues
{
    using System;
    using System.Data.SqlClient;

    using BusinessLogic.Accounts;
    using BusinessLogic.Cache;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Fields.Type.Base;

    using Common.Logging;

    using NSubstitute;
    using SQLDataAccess.UserDefinedFieldValues;

    using Xunit;

    public class SqlUserDefinedFieldFactoryTests
    {
        public class Ctor
        {
            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="SqlUserDefinedFieldFactory"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullAccount_ctor_ShouldThrowArgumentNullException()
            {
                ICustomerDataConnection<SqlParameter> customerDataConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();
                ICache<IField, Guid> cache = Substitute.For<ICache<IField, Guid>>();
                ILog logger = Substitute.For<ILog>();

                Assert.Throws<ArgumentNullException>(() => new SqlUserDefinedFieldFactory(customerDataConnection, null, cache, logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="SqlUserDefinedFieldFactory"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullCache_ctor_ShouldThrowArgumentNullException()
            {
                ICustomerDataConnection<SqlParameter> customerDataConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();
                IAccount account = Substitute.For<IAccount>();
                ILog logger = Substitute.For<ILog>();

                Assert.Throws<ArgumentNullException>(() => new SqlUserDefinedFieldFactory(customerDataConnection, account, null, logger));
            }

            /// <summary>
            /// Test to ensure a <see langword="null"/> <see cref="SqlUserDefinedFieldFactory"/> causes the constructor to throw a <see cref="ArgumentNullException"/> correctly.
            /// </summary>
            [Fact]
            public void WithNullDataConnection_ctor_ShouldThrowArgumentNullException()
            {
                IAccount account = Substitute.For<IAccount>();
                ICache<IField, Guid> cache = Substitute.For<ICache<IField, Guid>>();
                ILog logger = Substitute.For<ILog>();

                Assert.Throws<ArgumentNullException>(() => new SqlUserDefinedFieldFactory(null, account, cache, logger));
            }
        }

        public class Get
        {
            // get by string
            // get by id
        }
    }
}
