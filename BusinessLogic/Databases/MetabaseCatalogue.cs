

namespace BusinessLogic.Databases
{
    using System.Data.SqlClient;

    using Configuration.Interface;
    using Utilities.Cryptography;

    /// <summary>
    /// <see cref="DatabaseCatalogue">DatabaseCatelogue</see> defines a Metabase Catalogue
    /// </summary>
    public class MetabaseCatalogue : IDatabaseCatalogue
    {
        /// <summary>
        /// an instance of <see cref="ICryptography"/>.
        /// </summary>
        private readonly ICryptography _cryptography;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetabaseCatalogue"/> class. 
        /// </summary>
        /// <param name="cryptography">An instance of <see cref="ICryptography"/></param>
        /// <param name="configurationManager"></param>
        public MetabaseCatalogue(ICryptography cryptography, IConfigurationManager configurationManager)
        {
            Guard.ThrowIfNull(cryptography, nameof(cryptography));
            Guard.ThrowIfNull(configurationManager, nameof(configurationManager));

            this._cryptography = cryptography;
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(configurationManager.ConnectionStrings["metabase"].ConnectionString);
            this.Password = sqlConnectionStringBuilder.Password;
            this.Catalogue = sqlConnectionStringBuilder.InitialCatalog;
            this.Username = sqlConnectionStringBuilder.UserID;
            this.Server = new DatabaseServer(0, sqlConnectionStringBuilder.DataSource);
            this.DecryptPassword();
        }

        /// <summary>
        /// Gets the Metabase Catalogue
        /// </summary>
        public string Catalogue { get; }

        /// <summary>
        /// Gets the connection string for the Metabase Catalogue
        /// </summary>
        public string ConnectionString => $"Data Source={this.Server.Hostname};Initial Catalog={this.Catalogue};User ID={this.Username};Password={this.Password};Max Pool Size=10000;Application Name=Expenses";

        /// <summary>
        /// Gets the password for the Metabase Catalogue
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the server of the Metabase Catalogue
        /// </summary>
        public IDatabaseServer Server { get; }

        /// <summary>
        /// Gets the username of the Metabase Catalogue
        /// </summary>
        public string Username { get; }


        /// <summary>
        /// Decrypt the Connection password.
        /// </summary>
        private void DecryptPassword()
        {
            this.Password = this._cryptography.DecryptString(this.Password);
        }

    }
}
