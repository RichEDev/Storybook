namespace EsrGo2FromNhsWcfLibrary.Spend_Management
{
    /// <summary>
    /// Account details.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Account"/> class.
        /// </summary>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="databaseServer">
        /// The database server.
        /// </param>
        /// <param name="databaseName">
        /// The database name.
        /// </param>
        /// <param name="databaseUserName">
        /// The database user name.
        /// </param>
        /// <param name="databasePassword">
        /// The database password.
        /// </param>
        public Account(
            int accountid, string databaseServer, string databaseName, string databaseUserName, string databasePassword)
        {
            this.AccountId = accountid;
            this.DatabaseName = databaseName;
            this.DatabasePassword = databasePassword;
            this.DatabaseServer = databaseServer;
            this.DatabaseUserName = databaseUserName;
        }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the database server.
        /// </summary>
        public string DatabaseServer { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the database user name.
        /// </summary>
        public string DatabaseUserName { get; set; }

        /// <summary>
        /// Gets or sets the database password.
        /// </summary>
        public string DatabasePassword { get; set; }
    }
}