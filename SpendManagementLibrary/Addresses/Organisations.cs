namespace SpendManagementLibrary.Addresses
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    using Helpers;
    using Interfaces;

    public class Organisations
    {
        /// <summary>
        /// Retrieve a list of ListItems containing Organisation information
        /// </summary>
        /// <param name="accountIdentifier">The account to retrieve the organisation from</param>
        /// <param name="subAccountIdentifier">Sub-account to retrieve the organisation from, must be positive</param>
        /// <param name="showArchived">Include archived organisations in the list if true</param>
        /// <param name="connection">A connection override interface, usually used for unit testing</param>
        /// <returns></returns>
        public static ReadOnlyCollection<ListItem> GetListItems(int accountIdentifier, int subAccountIdentifier, bool showArchived = false, IDBConnection connection = null)
        {
            var organisations = new List<ListItem> { new ListItem("[None]", "0") };
            organisations.AddRange(Organisations.GetAll(accountIdentifier, connection)
                .Where(org => showArchived || !org.IsArchived)
                .Select(org => new ListItem
                    {
                        Value = org.Identifier.ToString(CultureInfo.InvariantCulture),
                        Text = org.Name
                    }));

            return organisations.AsReadOnly();
        }

        /// <summary>
        /// Retrieve a list of Organisations
        /// </summary>
        /// <param name="accountIdentifier">The account to retrieve the organisation from</param>
        /// <param name="connection">A connection override interface, usually used for unit testing</param>
        /// <returns></returns>
        public static ReadOnlyCollection<Organisation> GetAll(int accountIdentifier, IDBConnection connection = null)
        {
            var organisations = new List<Organisation>();

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                using (IDataReader reader = databaseConnection.GetReader("getOrganisations", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        organisations.Add(Organisations.Populate(reader));
                    }
                }
            }

            return organisations.AsReadOnly();
        }

        /// <summary>
        /// Retrieves a list of Organisations based on the supplied criteria
        /// </summary>
        /// <param name="accountIdentifier">The account to retrieve the organisation from</param>
        /// <param name="organisationName">The organisational name or label</param>
        /// <param name="comment">The comment</param>
        /// <param name="code">The code</param>
        /// <param name="line1">The first line of the address</param>
        /// <param name="city">The city</param>
        /// <param name="postCode">The postcode</param>
        /// <param name="archived">Whether the organisation is archived</param>
        /// <param name="connection">A connection override interface, usually used for unit testing</param>
        /// <returns></returns>
        public  ReadOnlyCollection<Organisation> GetByCriteria(int accountIdentifier, string organisationName, string comment, string code, string line1, string city, string postCode, bool archived = false, IDBConnection connection = null)
        {
            var organisations = new List<Organisation>();

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                if (!string.IsNullOrWhiteSpace(organisationName))
                {
                    databaseConnection.AddWithValue("@OrganisationName", organisationName);
                }

                if (!string.IsNullOrWhiteSpace(comment))
                {
                    databaseConnection.AddWithValue("@comment", comment);
                }

                if (!string.IsNullOrWhiteSpace(code))
                {
                    databaseConnection.AddWithValue("@code", code);
                }

                if (!string.IsNullOrWhiteSpace(line1))
                {
                    databaseConnection.AddWithValue("@line1", line1);
                }

                if (!string.IsNullOrWhiteSpace(city))
                {
                    databaseConnection.AddWithValue("@city", city);
                }

                if (!string.IsNullOrWhiteSpace(postCode))
                {
                    databaseConnection.AddWithValue("@postCode", postCode);
                }

                databaseConnection.AddWithValue("@Archived", archived);
      
                using (IDataReader reader = databaseConnection.GetReader("GetOrganisationsByCriteria", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        organisations.Add(Organisations.Populate(reader));
                    }
                }
            }

            return organisations.AsReadOnly();
        }

        /// <summary>
        /// Get an Organisation by its ID
        /// </summary>
        /// <param name="accountIdentifier">The account to retrieve the organisation from</param>
        /// <param name="organisationId">The ID of the Organisation to get</param>
        /// <param name="connection">A connection override interface, usually used for unit testing</param>
        /// <returns></returns>
        public static Organisation Get(int accountIdentifier, int organisationId, IDBConnection connection = null)
        {
            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountIdentifier)))
            {
                databaseConnection.AddWithValue("@OrganisationID", organisationId);
                using (IDataReader reader = databaseConnection.GetReader("getOrganisations", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        return Organisations.Populate(reader);
                    }
                }
            }

            return null;
        }

        private static Organisation Populate(IDataReader reader)
        {
            int primaryAddressIdOrdinal = reader.GetOrdinal("PrimaryAddressId");

            return new Organisation
            {
                Identifier = reader.GetRequiredValue<int>("OrganisationID"),
                Name = reader.GetRequiredValue<string>("OrganisationName"),
                ParentOrganisationIdentifier = reader.GetNullable<int>("ParentOrganisationId"),
                Comment = reader.GetNullableRef<String>("Comment"),
                Code = reader.GetNullableRef<String>("Code"),
                IsArchived = reader.GetRequiredValue<bool>("IsArchived"),
                PrimaryAddress = (reader.IsDBNull(primaryAddressIdOrdinal) ? null : new PrimaryAddress
                {
                    Identifier = reader.GetInt32(primaryAddressIdOrdinal),
                    FriendlyName = Address.GenerateFriendlyName(
                        reader.GetNullableRef<string>("AddressName"),
                        reader.GetNullableRef<string>("Line1"),
                        reader.GetNullableRef<string>("City"),
                        reader.GetNullableRef<string>("Postcode"))
                }),
                CreatedBy = reader.GetValueOrDefault("CreatedBy", Organisation.IntNull),
                CreatedOn = reader.GetValueOrDefault("CreatedOn", Organisation.DateTimeNull),
                ModifiedBy = reader.GetValueOrDefault("ModifiedBy", Organisation.IntNull),
                ModifiedOn = reader.GetValueOrDefault("ModifiedOn", Organisation.DateTimeNull),
            };
        }

    }
}
