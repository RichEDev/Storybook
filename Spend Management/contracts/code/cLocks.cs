using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Data;
using SpendManagementLibrary;

namespace Spend_Management
{
	public class cLocks
	{
		public static bool IsLocked(Cache fwCache, string cacheKeyPrefix, int LockId, int employeeid)
		{
			bool bIsLocked = false;
            int lockEmpId = 0;
			IDictionaryEnumerator cacheEnum = fwCache.GetEnumerator();

			while (cacheEnum.MoveNext())
			{
                if (cacheEnum.Key == (cacheKeyPrefix.Trim() + "_" + LockId.ToString()))
				{
                    lockEmpId = (int)cacheEnum.Value;

                    if (lockEmpId != employeeid)
					{
						// must be locked by another user
						bIsLocked = true;
						break;
					}
				}
			}

			return bIsLocked;
		}

		public static void LockContract(int accountId, string connStr, Cache fwCache, string cacheKeyPrefix, int contractId, int cacheTimeout, int employeeid)
		{
			StringBuilder sql = new StringBuilder();
			DataSet dset = new DataSet();
            DBConnection db = new DBConnection(connStr);
			sql.Append("SELECT dbo.IsVariation([contract_details].[ContractId]) AS [IsVariation],dbo.VariationCount([contract_details].[ContractId]) AS [VariationCount] FROM [contract_details] WHERE [ContractId] = @conId");
			db.sqlexecute.Parameters.AddWithValue("@conId", contractId);
            dset = db.GetDataSet(sql.ToString());

            
			if (dset.Tables[0].Rows.Count > 0)
			{
                DataRow drow = dset.Tables[0].Rows[0];
                short isVariation = (short)drow["IsVariation"];
                int variationCount = (int)drow[ "VariationCount"];

				if (isVariation == 1)
				{
					// contract is a variation - so get other variations and the primary contract to lock
					int PrimaryContractId;
                    db.sqlexecute.Parameters.Clear();
					sql.Remove(0, sql.Length);
					dset.Clear();
					sql.Append("SELECT [primaryContractId] FROM [link_variations] WHERE [variationContractId] = @varconId");
					db.sqlexecute.Parameters.AddWithValue("@varconId", contractId);
					dset = db.GetDataSet(sql.ToString());

                    drow = dset.Tables[0].Rows[0];
					PrimaryContractId = (int)drow["primaryContractId"];

                    AddExpireItemToCache(fwCache, cacheKeyPrefix.Trim() + "_" + PrimaryContractId.ToString(), cacheTimeout, employeeid);

                    db.sqlexecute.Parameters.Clear();
					sql.Remove(0, sql.Length);
					sql.Append("SELECT [variationContractId] FROM [link_variations] WHERE [primaryContractId] = @primConId");
					db.sqlexecute.Parameters.AddWithValue("@primConId", PrimaryContractId);

					dset.Clear();
					dset = db.GetDataSet(sql.ToString());

					foreach (DataRow dsubrow in dset.Tables[0].Rows)
					{
                        AddExpireItemToCache(fwCache, cacheKeyPrefix.Trim() + "_" + dsubrow["variationContractId"].ToString(), cacheTimeout, employeeid);
					}
				}
				else if (variationCount > 0)
				{
					// not a variation - check if their are any variations to this contract
                        db.sqlexecute.Parameters.Clear();
					sql.Remove(0, sql.Length);
					sql.Append("SELECT [variationContractId] FROM [link_variations] WHERE [PrimaryContractId] = @primConId");
					db.sqlexecute.Parameters.AddWithValue("@primConId", contractId);

					dset.Clear();
					dset = db.GetDataSet(sql.ToString());

					// lock the primary contract
                    AddExpireItemToCache(fwCache, cacheKeyPrefix.Trim() + "_" + contractId.ToString(), cacheTimeout, employeeid);

					foreach (DataRow dsubrow in dset.Tables[0].Rows)
					{
                        AddExpireItemToCache(fwCache, cacheKeyPrefix.Trim() + "_" + dsubrow["variationContractId"].ToString(), cacheTimeout, employeeid);
					}
				}
				else
				{
					// stand alone contract, just lock it
                    AddExpireItemToCache(fwCache, cacheKeyPrefix.Trim() + "_" + contractId.ToString(), cacheTimeout, employeeid);
				}

				dset.Dispose();
			}
		}

		public static void UpdateCacheExpireItem(Cache fwCache, string cacheKey, int cacheTimeout, object StoreItem)
		{
			IDictionaryEnumerator cacheEnum = fwCache.GetEnumerator();
			bool found = false;

			if (cacheTimeout == 0)
			{
				cacheTimeout = 5;
			}

			while (cacheEnum.MoveNext())
			{
				if (cacheEnum.Key == cacheKey)
				{
					fwCache[cacheKey] = StoreItem;
					found = true;
				}
			}

			if (!found)
			{
				fwCache.Add(cacheKey, StoreItem, null, Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes(cacheTimeout), System.Web.Caching.CacheItemPriority.Default, null);
			}
		}

		private static void AddExpireItemToCache(Cache fwCache, string cacheKey, int cacheTimeout, object StoreItem)
		{
			fwCache.Add(cacheKey, StoreItem, null, Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes(cacheTimeout), System.Web.Caching.CacheItemPriority.Default, null);
		}

		public static void RemoveLockItem(int accountid, string connectionString, Cache fwCache, string cacheKeyPrefix, int contractId, int employeeid)
		{
			StringBuilder sql = new StringBuilder();
			DataSet dset = new DataSet();

            DBConnection db = new DBConnection(connectionString);

			sql.Append("SELECT dbo.IsVariation([contract_details].[ContractId]) AS [IsVariation],dbo.VariationCount([contract_details].[ContractId]) AS [VariationCount] FROM [contract_details] WHERE [ContractId] = @conId");
			db.sqlexecute.Parameters.AddWithValue("@conId", contractId);
			dset = db.GetDataSet(sql.ToString());

			if (dset.Tables[0].Rows.Count > 0)
			{
                DataRow drow = dset.Tables[0].Rows[0];
                short isVariation = (short)drow["IsVariation"];
                int variationCount = (int)drow["VariationCount"];

				if (isVariation == 1)
				{
					// contract is a variation - so get other variations and the primary contract to unlock
					int PrimaryContractId;

					sql.Remove(0, sql.Length);
					dset.Clear();
                    db.sqlexecute.Parameters.Clear();
					sql.Append("SELECT [PrimaryContractId] FROM [link_variations] WHERE [VariationContractId] = @varconId");
					db.sqlexecute.Parameters.AddWithValue("@varconId", contractId);
					dset = db.GetDataSet(sql.ToString());

                    drow = dset.Tables[0].Rows[0];
                    PrimaryContractId = (int)drow["PrimaryContractId"];

					RemoveCacheLockingItem(fwCache, cacheKeyPrefix.Trim() + "_" + PrimaryContractId.ToString(), employeeid);

					sql.Remove(0, sql.Length);
					sql.Append("SELECT [VariationContractId] FROM [link_variations] WHERE [PrimaryContractId] = @primConId");
                    db.sqlexecute.Parameters.Clear();
					db.sqlexecute.Parameters.AddWithValue("@primConId", PrimaryContractId);

					dset.Clear();
					dset = db.GetDataSet(sql.ToString());

					foreach (DataRow dsubrow in dset.Tables[0].Rows)
					{
						RemoveCacheLockingItem(fwCache, cacheKeyPrefix.Trim() + "_" + dsubrow["VariationContractId"].ToString(), employeeid);
					}
				}
				else if (variationCount > 0)
				{
					// not a variation - check if their are any variations to this contract
					sql.Remove(0, sql.Length);
					sql.Append("SELECT [VariationContractId] FROM [link_variations] WHERE [PrimaryContractId] = @primConId");
                    db.sqlexecute.Parameters.Clear();
					db.sqlexecute.Parameters.AddWithValue("@primConId", contractId);

					dset.Clear();
					dset = db.GetDataSet(sql.ToString());

					foreach (DataRow dsubrow in dset.Tables[0].Rows)
					{
                        RemoveCacheLockingItem(fwCache, cacheKeyPrefix.Trim() + "_" + dsubrow["VariationContractId"].ToString(), employeeid);
					}

					// remove the lock from the primary contract
                    RemoveCacheLockingItem(fwCache, cacheKeyPrefix.Trim() + "_" + contractId.ToString(), employeeid);
				}
				else
				{
					// stand alone contract, just lock it
                    RemoveCacheLockingItem(fwCache, cacheKeyPrefix.Trim() + "_" + contractId.ToString(), employeeid);
				}

				dset.Dispose();
			}
		}

        /// <summary>
        /// Remove a cache lock for a particular account / record / employee
        /// </summary>
        /// <param name="fwCache">Cache reference</param>
        /// <param name="cacheKey">Unique cache key to check lock status of</param>
        /// <param name="employeeid">Employee to remove cache lock for</param>
		private static void RemoveCacheLockingItem(Cache fwCache, string cacheKey, int employeeid)
		{
            if (fwCache[cacheKey] != null)
            {
                int lockEmployeeId = (int)fwCache[cacheKey];
                if (lockEmployeeId == employeeid)
                {
                    fwCache.Remove(cacheKey);
                }
            }
		}
	}
}
