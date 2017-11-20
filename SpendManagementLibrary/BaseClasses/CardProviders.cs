using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;

namespace SpendManagementLibrary
{
    public class CardProviders
    {

        private string sConnectionString;
        const string sSQL = "select cardproviderid, cardprovider, card_type, createdon, createdby, modifiedon, modifiedby, AutoImport from dbo.card_providers";

        /// <summary>
        /// Constructor for Spend Management use
        /// </summary>
        public CardProviders(IDBConnection connection = null)
        {
            sConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
            InitialiseData(connection);
        }

        internal static Dictionary<int, cCardProvider> cardProviders;

        private void InitialiseData(IDBConnection connection = null)
        {
            if (cardProviders == null)
            {
                cardProviders = CacheList(connection);
            }

        }

        private Dictionary<int, cCardProvider> CacheList(IDBConnection connection = null)
        {
            return GetCollection(connection);
        }

        protected Dictionary<int, cCardProvider> GetCollection(IDBConnection connection = null)
        {
            IDBConnection expdata = connection ?? new DatabaseConnection(sConnectionString);

            var list = new Dictionary<int, cCardProvider>();

            using (var reader = expdata.GetReader(sSQL))
            {
                var autoImportOrd = reader.GetOrdinal("AutoImport");
                while (reader.Read())
                {
                    int cardproviderid = reader.GetInt32(0);
                    string cardprovider = reader.GetString(1);
                    CorporateCardType cardtype = (CorporateCardType)reader.GetByte(2);
                    DateTime createdon = reader.GetValueOrDefault("createdon", new DateTime(1900, 01, 01));
                    int createdby = reader.GetValueOrDefault("createdby", 0);
                    DateTime? modifiedon = reader.GetValueOrDefault("modifiedon", new DateTime(1900, 01, 01));
                    int? modifiedby = reader.GetValueOrDefault("modifiedby", 0);
                    bool autoImport = false;
                    if (!reader.IsDBNull(autoImportOrd))
                    {
                        autoImport = reader.GetBoolean(autoImportOrd);
                    }

                    cCardProvider newprov = new cCardProvider(cardproviderid, cardprovider, cardtype, createdon, createdby, modifiedon, modifiedby, autoImport);
                    list.Add(cardproviderid, newprov);
                }
                reader.Close();
            }

            return list;
        }

        /// <summary>
        /// Creates a list of all card providers for the dropdown
        /// </summary>
        /// <returns>A list of list of all card providers</returns>
        public ListItem[] CreateList()
        {
            return cardProviders.OrderBy(kvp => kvp.Value.cardprovider)
                                .Select(CreateListItem)
                                .ToArray();
        }

        /// <summary>
        /// Create the individual list item for the corporate card list
        /// </summary>
        /// <param name="kvp">The key value pair of corporate card id and the card provider</param>
        /// <returns>An individual list item</returns>
        private static ListItem CreateListItem(KeyValuePair<int, cCardProvider> kvp)
        {
             var result = new ListItem(kvp.Value.cardprovider, kvp.Key.ToString(CultureInfo.InvariantCulture));
            result.Attributes.Add("Auto", kvp.Value.AutoImport.ToString());
            return result;
        }

        public virtual cCardProvider getProviderByID(int id)
        {
            if (cardProviders.ContainsKey(id))
            {
                return cardProviders[id];
            }
            return null;
        }

        public cCardProvider getProviderByName(string name)
        {
            return cardProviders.Values.FirstOrDefault(provider => provider.cardprovider == name);
        }

        public Dictionary<int, cCardProvider> getCardProviders()
        {
            return cardProviders;
        }

    }
}
