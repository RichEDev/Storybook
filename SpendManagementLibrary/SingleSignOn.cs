namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Helpers;

    public class SingleSignOn
    {

        public string IssuerUri { get; set; }
	    public byte[] PublicCertificate { get; set; }
	    public string IdAttribute { get; set; }
	    public Guid IdLookupFieldId { get; set; }
	    public string CompanyIdAttribute { get; set; }
	    public string LoginErrorUrl { get; set; }
        public string TimeoutUrl { get; set; }
        public string ExitUrl { get; set; }

        public string PublicCertificateAsString
        {
            get
            {
                return (this.PublicCertificate == null ? null : Convert.ToBase64String(this.PublicCertificate));
            }
            set
            {
                this.PublicCertificate = (value == null ? null : Convert.FromBase64String(value));
            }
        }

        /// <summary>
        /// Queries the customer database and returns the Single Sign-on configuration for the current user
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <returns>Single Sign-on configuration, or null if none is set</returns>
        public static SingleSignOn Get(ICurrentUserBase currentUser)
        {
            return SingleSignOn.GetByAccountId(currentUser.AccountID);
        }

        /// <summary>
        /// Queries the metabase and returns all the Single Sign-on configurations for the specified Issuer Uri
        /// </summary>
        /// <param name="hostname">The hostname of this request</param>
        /// <param name="issuerUri">The IssuerUri to search for</param>
        /// <returns>An array of Single Sign-on configuration & account ids (or an empty array if none are found)</returns>
        public static Tuple<int, SingleSignOn>[] GetByIssuer(string hostname, string issuerUri)
        {
            var ssoAccounts = new List<Tuple<int, SingleSignOn>>();
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            {
                connection.AddWithValue("@Hostname", hostname);
                connection.AddWithValue("@IssuerUri", issuerUri);
                using (IDataReader reader = connection.GetReader("GetAccountIdsBySingleSignOnIssuerUri", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var accountId = (int)reader[0];
                        var sso = SingleSignOn.GetByAccountId(accountId);
                        if (sso != null)
                        {
                            ssoAccounts.Add(new Tuple<int, SingleSignOn>(accountId, sso));
                        }
                    }
                }
            }

            return ssoAccounts.ToArray();
        }

        private static SingleSignOn GetByAccountId(int accountId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                using (IDataReader reader = connection.GetReader("GetSingleSignOn", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        return new SingleSignOn().Populate(reader);
                    }
                }
            }

            return null;
        }

        private SingleSignOn Populate(IDataReader reader)
        {
            if (reader.IsClosed)
            {
                throw new InvalidOperationException("Cannot populate with a closed DataReader");
            }

            this.IssuerUri = (string)reader["IssuerUri"];
            this.PublicCertificate = (byte[])reader["PublicCertificate"];
            this.IdAttribute = (string)reader["IdAttribute"];
            this.IdLookupFieldId = (Guid)reader["IdLookupFieldId"];
            this.CompanyIdAttribute = (string)reader["CompanyIdAttribute"];
            this.LoginErrorUrl = (reader["LoginErrorUrl"] is DBNull ? null : (string)reader["LoginErrorUrl"]);
            this.TimeoutUrl = (reader["TimeoutUrl"] is DBNull ? null : (string)reader["TimeoutUrl"]);
            this.ExitUrl = (reader["ExitUrl"] is DBNull ? null : (string)reader["ExitUrl"]);

            return this;
        }

        /// <summary>
        /// Saves this instance of a Single Sign-on configuration to the database
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <returns>The saved instance</returns>
        public SingleSignOn Save(ICurrentUserBase currentUser)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.AddWithValue("@IssuerUri", this.IssuerUri);
                connection.AddWithValue("@PublicCertificate", this.PublicCertificate);
                connection.AddWithValue("@IdAttribute", this.IdAttribute);
                connection.AddWithValue("@IdLookupFieldId", this.IdLookupFieldId);
                connection.AddWithValue("@CompanyIdAttribute", this.CompanyIdAttribute);
                connection.AddWithValue("@LoginErrorUrl", (String.IsNullOrEmpty(this.LoginErrorUrl) ? DBNull.Value : (object)this.LoginErrorUrl));
                connection.AddWithValue("@TimeoutUrl", (String.IsNullOrEmpty(this.TimeoutUrl) ? DBNull.Value : (object)this.TimeoutUrl));
                connection.AddWithValue("@ExitUrl", (String.IsNullOrEmpty(this.ExitUrl) ? DBNull.Value : (object)this.ExitUrl));

                connection.AddWithValue("@CuEmployeeId", currentUser.EmployeeID);
                connection.AddWithValue("@CuDelegateId", (currentUser.isDelegate ? (object)currentUser.Delegate.EmployeeID : DBNull.Value));

                connection.ExecuteProc("SaveSingleSignOn");
            }

            return this;
        }
    }
}
