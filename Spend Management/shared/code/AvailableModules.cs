
namespace Spend_Management.shared.code
{

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using System.Collections.Generic;
    using System.Data;
    using Utilities.DistributedCaching;

    /// <summary>
    /// Available modules for an account
    /// </summary>
    public class AvailableModules
    {
        /// <summary>
        /// Account id
        /// </summary>
        private int AccountId { get; }

        /// <summary>
        /// Logon Message object.
        /// </summary>
        private Dictionary<int, string> _availableModulesList;

        /// <summary>
        /// Cache object.
        /// </summary>
        private readonly Cache _cache = new Cache();

        /// <summary>
        /// Caching area.
        /// </summary>
        private const string CacheArea = "activeModulesForAnAccount";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AvailableModules(int accountid)
        {
            this.InitialiseData();
            this.AccountId = accountid;
        }

        /// <summary>
        /// Get all logonmessages to cache list.
        /// </summary>
        public Dictionary<int, string> CacheList()
        {
            Dictionary<int, string> availableModulesList = new Dictionary<int, string>();
            using (var connection = new DatabaseConnection(GlobalVariables.MetabaseConnectionString))
            using (var reader = connection.GetReader("getavailableactivemodules ", CommandType.StoredProcedure))
            {
                var moduleIdOrd = reader.GetOrdinal("moduleId");
                var moduleNameOrd = reader.GetOrdinal("BrandName");              

                while (reader.Read())
                {
                    var moduleId = reader.GetInt32(moduleIdOrd);
                    var moduleName = !reader.IsDBNull(moduleNameOrd) ? reader.GetString(moduleNameOrd) : string.Empty;
                    availableModulesList.Add(moduleId, moduleName);
                }
                reader.Close();


                this._cache.Add(this.AccountId, CacheArea, "0", availableModulesList);
                return availableModulesList;
            }
        }

        /// <summary>
        /// Get all  messages for module
        /// </summary>
        /// <returns>List of Logon Messages</returns>
        public Dictionary<int, string> GetAllModules()
        {
            return this._availableModulesList;
        }

        /// <summary>
        /// Initialise data.
        /// </summary>
        private void InitialiseData()
        {
            this._availableModulesList = this._cache.Get(this.AccountId, CacheArea, "0") as Dictionary<int, string>
                            ?? this.CacheList();
        }
               
    }
}