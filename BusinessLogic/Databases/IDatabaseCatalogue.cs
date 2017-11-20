namespace BusinessLogic.Databases
{
    /// <summary>
    /// An interface that defines the commmone database Catalogue fields
    /// </summary>
    public interface IDatabaseCatalogue
    {
        /// <summary>
        /// The catalogue
        /// </summary>
        string Catalogue { get; }

        /// <summary>
        /// The connection string for the Catalogue
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// The password for the Catalogue
        /// </summary>
        string Password { get; }

        /// <summary>
        // The database server the Catalogue resides on
        /// </summary>
        IDatabaseServer Server { get; }

        /// <summary>
        /// The Catalogue username
        /// </summary>
        string Username { get; }
    }
}