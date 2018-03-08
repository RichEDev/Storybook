namespace ManagementAPI.Repositories.Accounts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ManagementAPI.Models;
    using System.Data.SqlClient;
    using System.Data;
    using ManagementAPI.ViewModels;
    using ManagementAPI.Enums;

    public class AccountRepository : IAccountRepository
    {
        #region Accounts

        /// <summary>
        /// Returns a list of all accounts.
        /// </summary>
        /// <returns></returns>
        public List<Account> GetAll()
        {
            var accounts = new List<Account>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.GetAccounts", connection) { CommandType = CommandType.StoredProcedure })
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var account = new Account
                        {
                            AccountId = reader.GetInt32(reader.GetOrdinal("accountid"))
                        };

                        account = BuildAccountFromReader(account, reader);
                        accounts.Add(account);
                    }
                }
            }

            return accounts.OrderBy(account => account.CompanyId).ToList();
        }

        /// <summary>
        /// Returns a single account by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Account Get(int id)
        {
            var account = new Account();

            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("dbo.GetAccount", connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                command.Parameters.AddWithValue("@accountid", id);

                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        account.AccountId = id;
                        account = BuildAccountFromReader(account, reader);
                    }
            }
            return account;
        }

        /// <summary>
        /// Saves an account to the database.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public bool Save(AccountViewModel vm)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.SaveAccount", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.AddWithValue("@username", vm.Username);
                    command.Parameters.AddWithValue("@action", vm.Account.AccountId == 0 ? "Add Account" : "Edit Account");
                    command.Parameters.AddWithValue("@accountid", vm.Account.AccountId);

                    // General Details
                    command.Parameters.AddWithValue("@companyname", vm.Account.CompanyName);
                    command.Parameters.AddWithValue("@companyid", vm.Account.CompanyId);
                    command.Parameters.AddWithValue("@contact", vm.Account.Contact);
                    command.Parameters.AddWithValue("@expiry", vm.Account.Expiry);
                    command.Parameters.AddWithValue("@archived", vm.Account.Archived);
                    command.Parameters.AddWithValue("@employeeSearchEnabled", vm.Account.EmployeeSearchEnabled);
                    command.Parameters.AddWithValue("@hotelReviewsEnabled", vm.Account.HotelReviewsEnabled);
                    command.Parameters.AddWithValue("@advancesEnabled", vm.Account.AdvancesEnabled);
                    command.Parameters.AddWithValue("@isNHSCustomer", vm.Account.IsNhsCustomer);
                    if (vm.Account.ContactEmail != null)
                    {
                        command.Parameters.AddWithValue("@contactEmail", vm.Account.ContactEmail);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@contactEmail", DBNull.Value);
                    }

                    // Database
                    command.Parameters.AddWithValue("@dbserver", vm.Account.DatabaseServer);
                    command.Parameters.AddWithValue("@dbname", vm.Account.DbName);
                    command.Parameters.AddWithValue("@dbusername", vm.Account.DbUsername);
                    command.Parameters.AddWithValue("@dbpassword", vm.Account.DbPassword);

                    // Licensing
                    command.Parameters.AddWithValue("@licenceType", vm.Account.LicenceType);
                    command.Parameters.AddWithValue("@nousers", vm.Account.NumberOfUsers);
                    if (vm.Account.LicensedUsers != null)
                    {
                        command.Parameters.AddWithValue("@licencedUsers", vm.Account.LicensedUsers);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@licencedUsers", DBNull.Value);
                    }
                    command.Parameters.AddWithValue("@postcodeAnywhereEnabled", vm.Account.PostcodeAnywhereEnabled);
                    command.Parameters.AddWithValue("@corporateCardsEnabled", vm.Account.CorporateCardsEnabled);
                    command.Parameters.AddWithValue("@contactHelpDeskAllowed", vm.Account.ContactHelpDeskAllowed);
                    command.Parameters.AddWithValue("@SingleSignOnEnabled", vm.Account.SingleSignonEnabled);
                    command.Parameters.AddWithValue("@annualContract", vm.Account.AnnualContract);

                    // Addresses
                    command.Parameters.AddWithValue("@mapsEnabled", vm.Account.MapsEnabled);
                    command.Parameters.AddWithValue("@addressLookupProvider", vm.Account.AddressLookupProvider);
                    command.Parameters.AddWithValue("@addressLookupsChargeable", vm.Account.AddressLookupsChargeable);
                    command.Parameters.AddWithValue("@addressLookupPsmaAgreement", vm.Account.AddressLookupPsmaAgreement);
                    command.Parameters.AddWithValue("@addressInternationalLookupsAndCoordinates",
                        vm.Account.AddressInternationalLookupsAndCoordinates);
                    command.Parameters.AddWithValue("@addressDistanceLookupsRemaining",
                        vm.Account.AddressDistanceLookupsRemaining);
                    command.Parameters.AddWithValue("@addressLookupsRemaining", vm.Account.AddressLookupsRemaining);

                    // API Keys
                    command.Parameters.AddWithValue("@postcodeAnywhereKey", vm.Account.PostcodeAnywhereKey);
                    if (vm.Account.PostcodeAnywherePaymentServiceKey != null)
                    {
                        command.Parameters.AddWithValue("@PostcodeAnywherePaymentServiceKey",
                            vm.Account.PostcodeAnywherePaymentServiceKey);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@PostcodeAnywherePaymentServiceKey", DBNull.Value);
                    }

                    if (vm.Account.DvlaLookupKey != null)
                    {
                        command.Parameters.AddWithValue("@DVLALookUpKey", vm.Account.DvlaLookupKey);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@DVLALookUpKey", DBNull.Value);
                    }

                    // Expedite
                    command.Parameters.AddWithValue("@ReceiptServiceEnabled", vm.Account.ReceiptServiceEnabled);
                    command.Parameters.AddWithValue("@ValidationServiceEnabled", vm.Account.ValidationServiceEnabled);
                    command.Parameters.AddWithValue("@PaymentServiceEnabled", vm.Account.PaymentServiceEnabled);
                    if (vm.Account.DaysToWaitBeforeSentEnvelopeIsMissing != null)
                    {
                        command.Parameters.AddWithValue("@DaysToWaitBeforeSentEnvelopeIsMissing",
                            vm.Account.DaysToWaitBeforeSentEnvelopeIsMissing);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@DaysToWaitBeforeSentEnvelopeIsMissing", DBNull.Value);
                    }

                    if (vm.Account.FundLimit != null)
                    {
                        command.Parameters.AddWithValue("@FundLimit", vm.Account.FundLimit);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@FundLimit", DBNull.Value);
                    }

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Associates a hostname with an account.
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public bool SaveHostnameToAccount(HostnameLicensedElementBundle bundle)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.SaveHostnameToAccount", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.AddWithValue("@username", bundle.Username);
                    command.Parameters.AddWithValue("@hostnameid", bundle.ElementId);
                    command.Parameters.AddWithValue("@accountid", bundle.AccountId);
                    command.Parameters.AddWithValue("@action", "Add Hostname");

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Dissociates a hostname from an account.
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public bool RemoveHostnameFromAccount(HostnameLicensedElementBundle bundle)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("dbo.RemoveHostnameFromAccount", connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                command.Parameters.AddWithValue("@accountid", bundle.AccountId);
                command.Parameters.AddWithValue("@hostnameid", bundle.ElementId);
                command.Parameters.AddWithValue("@username", bundle.Username);
                command.Parameters.AddWithValue("@action", "Remove Hostname");

                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Associates a licensed element with an account.
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public bool SaveLicensedElementToAccount(HostnameLicensedElementBundle bundle)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.SaveLicensedElementToAccount", connection) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.AddWithValue("@username", bundle.Username);
                    command.Parameters.AddWithValue("@elementid", bundle.ElementId);
                    command.Parameters.AddWithValue("@accountid", bundle.AccountId);
                    command.Parameters.AddWithValue("@action", "Add Licensed Element");

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Dissociates a licensed element from an account.
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public bool RemoveLicensedElementFromAccount(HostnameLicensedElementBundle bundle)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("dbo.RemoveLicensedElementFromAccount", connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                command.Parameters.AddWithValue("@accountid", bundle.AccountId);
                command.Parameters.AddWithValue("@elementid", bundle.ElementId);
                command.Parameters.AddWithValue("@username", bundle.Username);
                command.Parameters.AddWithValue("@action", "Remove Licensed Element");

                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Delete an account from the database by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("dbo.DeleteAccount", connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                command.Parameters.AddWithValue("@accountId", id);

                return command.ExecuteNonQuery() > 0;
            }
        }

        #endregion

        #region Hostnames and Licensed Elements

        /// <summary>
        /// Returns a list of all hostnames.
        /// </summary>
        /// <returns></returns>
        public List<Hostname> GetHostnames()
        {
            var hostnames = new List<Hostname>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.GetHostnames", connection) { CommandType = CommandType.StoredProcedure })
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var hostname = new Hostname
                        {
                            HostnameId = reader.GetInt32(reader.GetOrdinal("hostnameID"))
                        };

                        hostname = BuildHostnameFromReader(hostname, reader);
                        hostnames.Add(hostname);
                    }
                }
            }

            return hostnames.OrderBy(hostname => hostname.HostnameId).ToList();
        }

        /// <summary>
        /// Returns a list of all hostnames associated with an account by the account's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<int> GetHostnamesByAccountId(int id)
        {
            var hostnameIDs = new List<int>();

            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("dbo.GetSelectedHostnames", connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                command.Parameters.AddWithValue("@accountid", id);

                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var hostname = new Hostname();

                        hostname = BuildHostnameFromReader(hostname, reader);
                        hostnameIDs.Add(hostname.HostnameId);
                    }
            }

            return hostnameIDs;
        }

        /// <summary>
        /// Returns a list of all licensed elements.
        /// </summary>
        /// <returns></returns>
        public List<LicensedElement> GetLicensedElements()
        {
            var licensedElements = new List<LicensedElement>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("dbo.GetLicensedElementsForForm", connection) { CommandType = CommandType.StoredProcedure })
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var licensedElement = new LicensedElement
                        {
                            LicensedElementId = reader.GetInt32(reader.GetOrdinal("elementID"))
                        };

                        licensedElement = BuildLicensedElementFromReader(licensedElement, reader);
                        licensedElements.Add(licensedElement);
                    }
                }
            }

            return licensedElements.OrderBy(licensedElement => licensedElement.LicensedElementId).ToList();
        }

        /// <summary>
        /// Returns a list of all licensed elements associated with an account by the account's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<int> GetLicensedElementsByAccountId(int id)
        {
            var licensedElementIDs = new List<int>();

            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("dbo.GetSelectedLicensedElements", connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                command.Parameters.AddWithValue("@accountid", id);

                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        var licensedElement = new LicensedElement();

                        licensedElement = BuildLicensedElementFromReader(licensedElement, reader);
                        licensedElementIDs.Add(licensedElement.LicensedElementId);
                    }
            }

            return licensedElementIDs;
        }

        /// <summary>
        /// Returns a list of all database servers.
        /// </summary>
        /// <returns></returns>
        public List<DatabaseServer> GetDatabaseServers()
        {
            var databaseServers = new List<DatabaseServer>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT DatabaseID, Hostname FROM databases", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var databaseServer = new DatabaseServer
                        {
                            DatabaseServerId = reader.GetInt32(reader.GetOrdinal("databaseID")),
                            Hostname = reader.GetString(reader.GetOrdinal("hostname"))
                        };

                        databaseServers.Add(databaseServer);
                    }
                }
            }

            return databaseServers;
        }

        #endregion

        /// <summary>
        /// Populates the properties of an account using an initialised data reader.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Account BuildAccountFromReader(Account account, SqlDataReader reader)
        {
            // General Details
            account.CompanyName = reader.GetString(reader.GetOrdinal("companyname"));
            account.CompanyId = reader.GetString(reader.GetOrdinal("companyid"));
            account.Contact = reader.GetString(reader.GetOrdinal("contact"));
            account.Expiry = reader.GetDateTime(reader.GetOrdinal("expiry"));
            account.DatabaseServer = reader.GetInt32(reader.GetOrdinal("dbserver"));
            account.DbName = reader.GetString(reader.GetOrdinal("dbname"));
            account.DbUsername = reader.GetString(reader.GetOrdinal("dbusername"));
            account.DbPassword = reader.GetString(reader.GetOrdinal("dbpassword"));
            account.Archived = reader.GetBoolean(reader.GetOrdinal("archived"));
            account.EmployeeSearchEnabled = reader.GetBoolean(reader.GetOrdinal("employeeSearchEnabled"));
            account.HotelReviewsEnabled = reader.GetBoolean(reader.GetOrdinal("hotelReviewsEnabled"));
            account.AdvancesEnabled = reader.GetBoolean(reader.GetOrdinal("advancesEnabled"));
            account.IsNhsCustomer = reader.GetBoolean(reader.GetOrdinal("isNHSCustomer"));
            account.ContactEmail = reader.IsDBNull(reader.GetOrdinal("contactEmail")) ? null : reader.GetString(reader.GetOrdinal("contactEmail"));

            // Licensing
            account.NumberOfUsers = reader.GetInt32(reader.GetOrdinal("nousers"));
            account.PostcodeAnywhereEnabled = reader.GetBoolean(reader.GetOrdinal("postcodeAnyWhereEnabled"));
            account.CorporateCardsEnabled = reader.GetBoolean(reader.GetOrdinal("corporateCardsEnabled"));
            account.ContactHelpDeskAllowed = reader.GetBoolean(reader.GetOrdinal("contactHelpDeskAllowed"));
            account.LicensedUsers = reader.IsDBNull(reader.GetOrdinal("licencedUsers")) ? null : reader.GetString(reader.GetOrdinal("licencedUsers"));
            account.MapsEnabled = reader.GetBoolean(reader.GetOrdinal("mapsenabled"));
            account.SingleSignonEnabled = reader.GetBoolean(reader.GetOrdinal("SingleSignOnEnabled"));
            account.AddressLookupProvider = reader.IsDBNull(reader.GetOrdinal("addressLookupProvider")) ? AddressLookupProviderEnum.None : (AddressLookupProviderEnum)reader.GetByte(reader.GetOrdinal("addressLookupProvider"));
            account.AddressLookupsChargeable = reader.GetBoolean(reader.GetOrdinal("addressLookupsChargeable"));
            account.AddressLookupPsmaAgreement = reader.GetBoolean(reader.GetOrdinal("addressLookupPsmaAgreement"));
            account.AddressInternationalLookupsAndCoordinates = reader.GetBoolean(reader.GetOrdinal("addressInternationalLookupsAndCoordinates"));
            account.AddressLookupsRemaining = reader.GetInt32(reader.GetOrdinal("addressLookupsRemaining"));
            account.AddressDistanceLookupsRemaining = reader.GetInt32(reader.GetOrdinal("addressDistanceLookupsRemaining"));
            account.LicenceType = reader.IsDBNull(reader.GetOrdinal("licenceType")) ? LicenceTypeEnum.None : (LicenceTypeEnum)reader.GetByte(reader.GetOrdinal("licenceType"));
            
            account.AnnualContract = reader.GetBoolean(reader.GetOrdinal("annualContract"));

            // API Keys
            account.PostcodeAnywhereKey = reader.IsDBNull(reader.GetOrdinal("PostcodeAnywhereKey")) ? null : reader.GetString(reader.GetOrdinal("PostcodeAnywhereKey"));
            account.PostcodeAnywherePaymentServiceKey = reader.IsDBNull(reader.GetOrdinal("PostcodeAnywherePaymentServiceKey")) ? null : reader.GetString(reader.GetOrdinal("PostcodeAnywherePaymentServiceKey"));
            account.DvlaLookupKey = reader.IsDBNull(reader.GetOrdinal("DVLALookUpKey")) ? null : reader.GetString(reader.GetOrdinal("DVLALookUpKey"));

            // Expedite
            account.ReceiptServiceEnabled = reader.GetBoolean(reader.GetOrdinal("ReceiptServiceEnabled"));
            account.ValidationServiceEnabled = reader.GetBoolean(reader.GetOrdinal("ValidationServiceEnabled"));
            account.PaymentServiceEnabled = reader.GetBoolean(reader.GetOrdinal("PaymentServiceEnabled"));
            account.DaysToWaitBeforeSentEnvelopeIsMissing = reader.IsDBNull(reader.GetOrdinal("DaysToWaitBeforeSentEnvelopeIsMissing")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("DaysToWaitBeforeSentEnvelopeIsMissing"));
            account.FundLimit = reader.IsDBNull(reader.GetOrdinal("FundLimit")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("FundLimit"));

            return account;
        }

        /// <summary>
        /// Populates the properties of a hostname using an initialised data reader.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Hostname BuildHostnameFromReader(Hostname hostname, SqlDataReader reader)
        {
            hostname.HostnameId = reader.GetInt32(reader.GetOrdinal("hostnameID"));
            hostname.HostName = reader.GetString(reader.GetOrdinal("hostname"));
            hostname.ModuleId = reader.GetInt32(reader.GetOrdinal("moduleID"));

            return hostname;
        }

        /// <summary>
        /// Populates the properties of a licensed element using an initialised data reader.
        /// </summary>
        /// <param name="licensedElement"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static LicensedElement BuildLicensedElementFromReader(LicensedElement licensedElement, SqlDataReader reader)
        {
            licensedElement.LicensedElementId = reader.GetInt32(reader.GetOrdinal("elementID"));
            licensedElement.LicensedElementName = reader.GetString(reader.GetOrdinal("elementFriendlyName"));

            return licensedElement;
        }
    }
}