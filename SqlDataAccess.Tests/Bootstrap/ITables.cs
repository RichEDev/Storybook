namespace SqlDataAccess.Tests.Bootstrap
{
    using BusinessLogic.Tables;

    /// <summary>
    /// The "Tables" objects mocked for unit tests
    /// </summary>
    public interface ITables
    {
        /// <summary>
        /// Gets or sets the Mock'd <see cref="TableRepository"/>
        /// </summary>
        TableRepository TableRepository { get; set; }
    }
}