using SpendManagementLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utilities.DistributedCaching;

namespace Spend_Management.expenses.code
{
    /// <summary>
    /// Summary description for generalOption.
    /// </summary>
    public class cGeneralOption
    {
        string strsql;
		int nAccountid = 0;
        readonly Cache cache = new Cache();

        /// <summary>
        /// The cache key area name
        /// </summary>
        public const string CacheArea = "cgeneralOption";

        List<cGeneralOptions> list;
        public cGeneralOption(int accountid)
		{
			nAccountid = accountid;
            
			InitialiseData();
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion
        public void InitialiseData()
        {
            list = this.cache.Get(this.nAccountid, CacheArea, "0") as List<cGeneralOptions> ?? this.CacheList();            
        }

        private List<cGeneralOptions> CacheList()
        {
            List<cGeneralOptions> list = new List<cGeneralOptions>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cGeneralOptions generalOption;
            int subAccountID;
            string stringKey, stringValue, formPostKey;
            int? modifiedby;
            DateTime? modifiedon;
            var createdOn = new DateTime(1900, 1, 1);
            int? createdby = null;
            bool isGlobal;

            using (System.Data.SqlClient.SqlDataReader reader = expdata.GetStoredProcReader("GetAccountProperties")) 
            {
                while (reader.Read())
                {
                    subAccountID = reader.GetInt32(reader.GetOrdinal("subAccountID"));
                    if (reader.IsDBNull(reader.GetOrdinal("stringKey")) == true)
                    {
                        stringKey = "";
                    }
                    else
                    {
                        stringKey = reader.GetString(reader.GetOrdinal("stringKey"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("stringValue")) == true)
                    {
                        stringValue = "";
                    }
                    else
                    {
                        stringValue = reader.GetString(reader.GetOrdinal("stringValue"));
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("createdon")))
                    {
                        createdOn = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("modifiedOn")) == true)
                    {
                        modifiedon = null;
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                    {
                        createdby = 0;
                    }
                    else
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("modifiedBy")) == true)
                    {
                        modifiedby = null;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedBy"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("formPostKey")) == true)
                    {
                        formPostKey = "";
                    }
                    else
                    {
                        formPostKey = reader.GetString(reader.GetOrdinal("formPostKey"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("isGlobal")) == true)
                    {
                        isGlobal = false;
                    }
                    else
                    {
                        isGlobal = reader.GetBoolean(reader.GetOrdinal("isGlobal"));
                    }
                    generalOption = new cGeneralOptions(subAccountID, stringKey, stringValue, createdOn, createdby.Value, modifiedon, modifiedby, formPostKey, isGlobal);
                    list.Add(generalOption);
                }
                reader.Close();                
            }
            this.cache.Add(this.nAccountid, CacheArea, "0", list);
            return list;
        }

        /// <summary>
        /// Gets a list of all the general options from the cache.
        /// </summary>
        /// <returns></returns>
        public List<cGeneralOptions> GetList()
        {
            if (list == null)
            {
                InitialiseData();
            }           
            // ReSharper disable once PossibleNullReferenceException
            return list;
        }

        /// <summary>
        /// Gets the general options from the cache.
        /// <param name="subAccountID">The subAccountID of the GeneralOption to get.</param>
        /// </summary>
        /// <returns></returns>
        public cGeneralOptions getcGeneralOptionById(int subAccountID)
        {            
            return list.FirstOrDefault(x => x.nSubAccountID == subAccountID);      
        } 

        /// <summary>
        /// Gets the general options from the cache.
        /// <param name="key">The key of the GeneralOption to get.</param>
        /// </summary>
        /// <returns></returns>
        public cGeneralOptions getcGeneralOptionByKey(string key)
        {            
            return list.FirstOrDefault(x => x.nKey == key);      
        } 
    }
}