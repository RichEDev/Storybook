namespace SpendManagementLibrary.GeneralOptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utilities.DistributedCaching;
    using System.Data;
    using Helpers;

    public class GeneralOptions
    {
        private readonly int AccountId;
        private List<GeneralOption> List;
        readonly Cache cache = new Cache();
        private const string CacheArea = "GeneralOptions";
        
        public GeneralOptions(int accountId)
        {
            this.AccountId = accountId;   
        }

        private void InitialiseData(int subAccountId)
        {
            List = this.cache.Get(this.AccountId, CacheArea, subAccountId.ToString()) as List<GeneralOption> ?? this.CacheList(subAccountId);
        }

        private List<GeneralOption> CacheList(int subAccountId)
        {
            var generalOptions = new List<GeneralOption>();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                connection.AddWithValue("subaccountId", subAccountId);

                using (var reader = connection.GetReader("GetAccountProperties", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        var stringKey = reader.IsDBNull(reader.GetOrdinal("stringKey")) ? "" : reader.GetString(reader.GetOrdinal("stringKey"));
                        var stringValue = reader.IsDBNull(reader.GetOrdinal("stringValue")) ? "" : reader.GetString(reader.GetOrdinal("stringValue"));
                        var formPostKey = reader.IsDBNull(reader.GetOrdinal("formPostKey")) ? "" : reader.GetString(reader.GetOrdinal("formPostKey"));
                        var isGlobal = reader.IsDBNull(reader.GetOrdinal("isGlobal")) != true && reader.GetBoolean(reader.GetOrdinal("isGlobal"));

                        generalOptions.Add(new GeneralOption(subAccountId, stringKey, stringValue, formPostKey, isGlobal));

                    }
                    reader.Close();
                }
            }
            this.cache.Add(this.AccountId, CacheArea, subAccountId.ToString(), generalOptions);

            return generalOptions;
        }

        public void InvalidateCache(int subaccountId)
        {
            this.cache.Delete(this.AccountId, CacheArea, subaccountId.ToString());
        }

        /// <summary>
        /// Gets a list of all the general options for a subaccount Id
        /// </summary>
        /// <param name="subaccountId">The subaccount Id</param>
        /// <returns>A list of <see cref="GeneralOption">GeneralOption</see></returns>
        public List<GeneralOption> GetList(int subaccountId)
        {
            InitialiseData(subaccountId);
            return List;           
        }

        /// <summary>
        ///  Gets the general option that matches the key and the subaccount Id
        /// <param name="key">The key of the GeneralOption to get.</param>
        /// <param name="subAccountId">The subAccountId</param>
        /// </summary>
        /// <returns>A matching <see cref="GeneralOption">GeneralOption</see></returns>
        public GeneralOption GetGeneralOptionByKeyAndSubAccount(string key, int subAccountId)
        {
            InitialiseData(subAccountId);
            return List.FirstOrDefault(account => account.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        } 
    }
}
