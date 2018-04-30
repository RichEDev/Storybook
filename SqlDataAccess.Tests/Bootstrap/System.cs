namespace SqlDataAccess.Tests.Bootstrap
{
    using global::System.Data.SqlClient;

    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;

    using Common.Logging;

    using NSubstitute;

    /// <summary>
    /// The Mocked "System" objects used in unit tests
    /// </summary>
    public class System : ISystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="System"/> class.
        /// </summary>
        public System()
        {
            this.Logger = Substitute.For<ILog>();
            this.IdentityProvider = Substitute.For<IdentityProvider>(new IdentityContextStub(22, 22));
            this.DatabaseServerRepository = Substitute.For<IDataFactory<IDatabaseServer, int>>();
            this.CustomerDataConnection = Substitute.For<ICustomerDataConnection<SqlParameter>>();
        }

        /// <summary>
        /// Gets the Mock'd <see cref="ILog"/>
        /// </summary>
        public ILog Logger { get; }

        /// <summary>
        /// Gets the Mock'd <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/>
        /// </summary>
        public IDataFactory<IDatabaseServer, int> DatabaseServerRepository { get; }

        /// <summary>
        /// Gets the Mock'd <see cref="ICustomerDataConnection{T}"/>
        /// </summary>
        public ICustomerDataConnection<SqlParameter> CustomerDataConnection { get; }

        /// <summary>
        /// Gets the Mock'd <see cref="ISystem.IdentityProvider"/>
        /// </summary>
        public IdentityProvider IdentityProvider { get; }
    }
}