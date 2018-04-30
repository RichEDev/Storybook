namespace SqlDataAccess.Tests.Bootstrap
{
    using global::System.Data.SqlClient;

    using BusinessLogic.Databases;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;

    using Common.Logging;

    /// <summary>
    /// The Mocked "System" objects used in unit tests
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Gets the Mock'd <see cref="ILog"/>
        /// </summary>
        ILog Logger { get; }

        /// <summary>
        /// Gets the Mock'd <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/>
        /// </summary>
        IDataFactory<IDatabaseServer, int> DatabaseServerRepository { get; }

        /// <summary>
        /// Gets the Mock'd <see cref="ICustomerDataConnection{T}"/>
        /// </summary>
        ICustomerDataConnection<SqlParameter> CustomerDataConnection { get; }

        /// <summary>
        /// Gets the Mock'd <see cref="IdentityProvider"/>
        /// </summary>
        IdentityProvider IdentityProvider { get; }

    }
}