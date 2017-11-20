namespace BusinessLogic.Databases
{
    using System;

    using Utilities.Cryptography;

    /// <summary>
    /// <see cref="DatabaseCatalogue">DatabaseCatelogue</see>  defines a database Catalogue
    /// </summary>
    [Serializable]
    public class DatabaseCatalogue : IDatabaseCatalogue
    {
        /// <summary>
        /// A local instance of <see cref="ICryptography"/>.
        /// </summary>
        private readonly ICryptography _cryptography;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseCatalogue"/> class. 
        /// </summary>
        /// <param name="server">
        /// the database server the Catalogue resides on
        /// </param>
        /// <param name="catalogue">
        /// The Catalogue name
        /// </param>
        /// <param name="username">
        /// The Catalogue username 
        /// </param>
        /// <param name="password">
        /// The password for the Catalogue
        /// </param>
        /// <param name="cryptography">
        /// An instance of <see cref="ICryptography"/>
        /// </param>
        public DatabaseCatalogue(IDatabaseServer server, string catalogue, string username, string password, ICryptography cryptography)
        {
            this._cryptography = cryptography;
            this.Server = server;
            this.Catalogue = catalogue;
            this.Username = username;
            this.Password = password;
            this.DecryptPassword();
        }

        /// <summary>
        /// Gets the database server the Catalogue resides on
        /// </summary>
        public IDatabaseServer Server { get; }

        /// <summary>
        /// Gets the Catalogue
        /// </summary>
        public string Catalogue { get; }

        /// <summary>
        /// Gets the username for the Catalogue
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the password for the Catalogue
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the connection string for the Catalogue
        /// </summary>
        public string ConnectionString =>
            $"Data Source={this.Server.Hostname};Initial Catalog={this.Catalogue};User ID={this.Username};Password={this.Password};Max Pool Size=10000;Application Name=Expenses";

        /// <summary>
        /// Decrypt the password for the connection string.
        /// </summary>
        private void DecryptPassword()
        {
            this.Password = this._cryptography.DecryptString(this.Password);
        }
    }
}