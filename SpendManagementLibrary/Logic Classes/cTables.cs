using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using SpendManagementLibrary.Helpers;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Logic_Classes.Tables;
    using SpendManagementLibrary.Report;

    public class cTables : ITables
    {
        #region Global Varaiables
        private int nAccountID;
        private ConcurrentDictionary<Guid, cTable> lstCachedMetabaseTables;
        private ConcurrentDictionary<Guid, cTable> lstCachedCustomerTables;
        private static ConcurrentDictionary<int, ConcurrentDictionary<Guid, cTable>> allTables = new ConcurrentDictionary<int, ConcurrentDictionary<Guid, cTable>>();
        private static ConcurrentDictionary<int, long> lastReadFromDatabaseTicks = new ConcurrentDictionary<int, long>();

        #endregion Global Variables

        #region Enumerators
        private enum CacheType
        {
            Metabase,
            Customer
        }
        public enum DropDownType
        {
            AllowImport,
            HasUserdefined,
            AllowedWorkflow
        }
        #endregion Enumerators

        #region Methods

        /// <summary>
        /// Used for interaction with tables collection within the product
        /// </summary>
        /// <param name="accountID"></param>
        public cTables(int accountID)
        {
            if (accountID == 0) throw new Exception("Invalid accountID passed to cTables");
            nAccountID = accountID;
            Initialize();
        }

        /// <summary>
        /// Only to be used for caching metabase tables
        /// </summary>
        public cTables()
        {
            nAccountID = 0;
            Initialize();
        }

        private void Initialize()
        {
            if (nAccountID > 0)
            {
                long lastUpdatedAllServers = cUserDefinedFieldsBase.GetLastUpdatedFromCache(nAccountID);
                long lastReadFromDatabaseThisServer = lastReadFromDatabaseTicks.GetOrAdd(nAccountID, 0);
                var forceUpdateFromDatabase = lastUpdatedAllServers > lastReadFromDatabaseThisServer;
                if (forceUpdateFromDatabase)
                {
                    ConcurrentDictionary<Guid, cTable> oldValue;
                    allTables.TryRemove(nAccountID, out oldValue);
                }
            }

            lstCachedMetabaseTables = allTables.GetOrAdd(0, CacheLists);
            if (nAccountID != 0)
            {
                lstCachedCustomerTables = allTables.GetOrAdd(nAccountID, CacheLists);
            }
        }

        const string recordsetError = "The SQL should return the date, then the tables.";

        private static ConcurrentDictionary<Guid, cTable> CacheLists(int nAccountID)
        {
            ConcurrentDictionary<Guid, cTable> lstCachedTables = new ConcurrentDictionary<Guid, cTable>();

            #region Reader vars

            string tableName = string.Empty;
            string description = string.Empty;
            #endregion Reader vars

            DateTime lastReadFromDatabase;
            string strSQL = "SELECT GETDATE() ";
            DatabaseConnection db;
            if (nAccountID == 0)
            {
                db = new DatabaseConnection(GlobalVariables.MetabaseConnectionString);
                strSQL += "SELECT tables_base.tableid, jointype, tablename, allowreporton, tables_base.allowimport, allowworkflow, allowentityrelationship, hasuserdefinedfields, tables_base.description, primarykey, keyfield, userdefined_table, tables_base.amendedon, elementid, fields_base.fieldid as subaccountidfield, tableFrom, tables_base.relabel_param, linkingTable from tables_base left join fields_base on fields_base.tableid = tables_base.tableid and fields_base.field = 'subaccountid'";
            }
            else
            {
                db = new DatabaseConnection(cAccounts.getConnectionString(nAccountID));
                strSQL += "SELECT tableid, jointype, tablename, allowreporton, allowimport, allowworkflow, allowentityrelationship, hasuserdefinedfields, description, primarykey, keyfield, userdefined_table, amendedon, elementid, subaccountidfield, tableFrom, relabel_param,isSystemView FROM [dbo].[tables] WHERE tableFrom <> @tableSrc";
                db.sqlexecute.Parameters.AddWithValue("@tableSrc", (int)cTable.TableSourceType.Metabase);
            }
            using (var reader = db.GetReader(strSQL))
            {
                if (!reader.Read()) throw new Exception(recordsetError);
                lastReadFromDatabase = reader.GetDateTime(0);
                Debug.WriteLine("warning: reading tables for {0} from database at {1}", nAccountID, lastReadFromDatabase);
                if (!reader.NextResult()) throw new Exception(recordsetError);

                while (reader.Read())
                {
                    tableName = (string)reader["tablename"];
                    description = reader.GetValueOrDefault("description", string.Empty);
                    Guid tableID = (Guid)reader["tableid"];
                    Guid primaryKey = reader.GetValueOrDefault("primarykey", Guid.Empty);
                    Guid keyField = reader.GetValueOrDefault("keyfield", Guid.Empty);
                    Guid userdefinedTable = reader.GetValueOrDefault("userdefined_table", Guid.Empty);
                    Guid subAccountIDField = reader.GetValueOrDefault("subAccountIDField", Guid.Empty);
                    byte joinType = reader.GetValueOrDefault("jointype", (byte)0);
                    int? elementID = reader.GetNullable<int>("elementID");
                    bool allowReportOn = (bool)reader["allowreporton"];
                    bool allowImport = (bool)reader["allowimport"];
                    bool allowWorkflow = (bool)reader["allowworkflow"];
                    bool allowEntityRelationship = (bool)reader["allowentityrelationship"];
                    bool isSystemView = (nAccountID != 0) && (bool)reader["isSystemView"];
                    cTable.TableSourceType tableFrom = (cTable.TableSourceType)reader.GetInt32(reader.GetOrdinal("tableFrom"));
                    string relabelParam = reader.GetValueOrDefault("relabel_param", string.Empty);
                    var linkingTable = false;
                    if (nAccountID == 0)
                    {
                        linkingTable = reader.GetBoolean(reader.GetOrdinal("linkingTable"));
                    }
                    cTable tmpTable = new cTable(nAccountID, tableName, joinType, description, allowReportOn, allowImport, allowWorkflow, allowEntityRelationship, tableID, primaryKey, keyField, userdefinedTable, subAccountIDField, tableFrom, elementID, subAccountIDField, isSystemView, relabelParam, linkingTable);
                    lstCachedTables.GetOrAdd(tmpTable.TableID, tmpTable);
                }
                reader.Close();
            }
            lastReadFromDatabaseTicks.AddOrUpdate(nAccountID,
                                        addValueFactory: accId => lastReadFromDatabase.Ticks,
                                        updateValueFactory: (accId, old) => lastReadFromDatabase.Ticks);


            return lstCachedTables;
        }

        /// <summary>
        /// Combines the metabase and customer cached dicionaries into one.
        /// </summary>
        /// <returns></returns>
        private Dictionary<Guid, cTable> CombineCachedDictionaries()
        {
            Dictionary<Guid, cTable> lstCombined = this.lstCachedMetabaseTables.Concat(this.lstCachedCustomerTables).DistinctBy(y => y.Key).ToDictionary(x => x.Key, x => x.Value);

            return lstCombined;
        }

        /// <summary>
        /// Retrieve table by its ID
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public cTable GetTableByID(Guid tableID)
        {
            cTable reqTable = null;
            if (tableID != Guid.Empty)
            {
                if (!this.lstCachedMetabaseTables.TryGetValue(tableID, out reqTable))
                {
                    this.lstCachedCustomerTables.TryGetValue(tableID, out reqTable);
                }
            }
            return reqTable;
        }


        /// <summary>
        /// Checks whether table exists in metabase or customer table list
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public bool IsTableExistsById(Guid tableId)
        {
            if (tableId != Guid.Empty)
            {
                if (this.lstCachedMetabaseTables.ContainsKey(tableId) || this.lstCachedMetabaseTables.ContainsKey(tableId))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a table by name
        /// </summary>
        /// <param name="tableName">Table name to locate</param>
        /// <returns>cTable class object</returns>
        public cTable GetTableByName(string tableName)
        {
            cTable reqTable = (from x in this.lstCachedMetabaseTables.Values
                               where x.TableName.ToLower() == tableName.ToLower()
                               select x).FirstOrDefault();

            if (reqTable == null)
            {
                reqTable = (from x in this.lstCachedCustomerTables.Values
                            where x.TableName.ToLower() == tableName.ToLower()
                            select x).FirstOrDefault();
            }

            return reqTable;
        }

        /// <summary>
        /// Gets a custom entity or userdefined table by name
        /// </summary>
        /// <param name="tableName">Table Name to locate</param>
        /// <returns>cTable class object</returns>
        public cTable GetCustomTableByName(string tableName)
        {
            cTable reqTable = (from x in this.lstCachedCustomerTables.Values
                               where x.TableName.ToLower() == tableName.ToLower()
                               select x).FirstOrDefault();

            return reqTable;
        }


        /// <summary>
        /// Returns a sorted list of DropDown items.
        /// </summary>
        /// <param name="dropDownType">
        /// The DropDownType enum value
        /// </param>
        /// <param name="excludeExpensesTables">
        /// Optionally don't include Expenses related tables
        /// </param>
        /// <param name="excludeNHSTables">
        /// Optionally don't include NHS related tables
        /// </param>
        public List<ListItem> CreateDropDown(DropDownType dropDownType, bool excludeExpensesTables = false, bool excludeNHSTables = false)
        {
            var items = new List<ListItem>();
            if (dropDownType == DropDownType.AllowedWorkflow)
            {
                items.Add(new ListItem("Expenses Approval", "d70d9e5f-37e2-4025-9492-3bcf6aa746a8")); // savedexpenses table
                items.Add(new ListItem("Self Registration", "618DB425-F430-4660-9525-EBAB444ED754"));
            }

            items.AddRange((from table in this.CombineCachedDictionaries().Values
                            let tableId = table.TableID.ToString()
                            orderby table.Description
                            where ((dropDownType == DropDownType.HasUserdefined && table.HasUserdefinedFields)
                                || (dropDownType == DropDownType.AllowImport && table.AllowImport)
                                || (dropDownType == DropDownType.AllowedWorkflow && table.AllowWorkflow))
                            && ((!excludeExpensesTables
                                || (tableId != "5d2d9191-83ea-4ed5-8a46-0aabb8190392" //p11d categories
                                    && tableId != "75c247c2-457e-4b14-bbec-1391cd77fb9e" //expense categories
                                    && tableId != "401b44d7-d6d8-497b-8720-7ffcc07d635d" //expense item categories
                                    && tableId != "d6ab6ff4-0ec4-4996-8566-458b816adc0d" //signoff groups
                                    && tableId != "db7d42fd-e1fa-4a42-84b4-e8b95c751bda" //item roles
                                    && tableId != "8211e41f-710e-42ab-a2df-1574fd003b31" //employee roles
                                    && tableId != "d6ab6ff4-0ec4-4996-8566-458b816adc0d")) //signoff groups
                            && (!excludeNHSTables
                                || (tableId != "c7009e76-4093-41ea-ad86-823876a95b5c" // NHS trusts
                                    && tableId != "bf9aa39a-82d6-4960-bfef-c5943bc0542d"))) // ESR assignments
                            select new ListItem(table.TableSource == cTable.TableSourceType.Metabase ? table.Description : table.Description + " - Custom Table", tableId)).ToList());

            return items;
        }

        public void ClearCustomerTablesCache()
        {
            lstCachedCustomerTables.Clear();
            ConcurrentDictionary<Guid, cTable> removed;
            allTables.TryRemove(nAccountID, out removed);

        }

        public cTable GetTableByUserdefineTableID(Guid userdefinedTableID)
        {
            cTable reqTable = (from x in this.lstCachedMetabaseTables.Values
                               where x.GetUserdefinedTable() != null && x.GetUserdefinedTable().TableID == userdefinedTableID
                               select x).FirstOrDefault();

            if (reqTable == null)
            {
                reqTable = (from x in this.lstCachedCustomerTables.Values
                            where x.GetUserdefinedTable() != null && x.GetUserdefinedTable().TableID == userdefinedTableID
                            select x).FirstOrDefault();
            }

            //foreach (cTable table in this.CachedTables.Values)
            //{
            //    if (table.UserdefinedTable != null && table.UserdefinedTable.TableID == userdefinedTableID)
            //    {
            //        reqTable = table;
            //        break;
            //    }
            //}
            return reqTable;
        }

        /// <summary>
        /// obtain the list of the account's licenced elements to only show the correct user defined tables for this module
        /// </summary>
        /// <param name="activeModule"></param>
        /// <returns></returns>
        public List<ListItem> CreateUserDefinedDropDown(Modules activeModule)
        {
            var clsElements = new cElements();
            List<int> lstLicencedElementIDs = clsElements.GetLicencedModuleIDs(this.nAccountID, activeModule);

            return (from x in this.CombineCachedDictionaries().Values
                          orderby x.Description
                          where (x.HasUserdefinedFields && x.ElementID.HasValue && lstLicencedElementIDs.Contains(x.ElementID.Value))
                          select new ListItem(x.Description, x.TableID.ToString())).ToList();
        }

        /// <summary>
        /// Get a list of reportable <see cref="cTable"/> objects.
        /// </summary>
        /// <param name="activeModule">The current active <see cref="Modules"/></param>
        /// <returns>A <see cref="List{T}"/> of <seealso cref="cTables"/>that have the report on attribute set to true.</returns>
        public List<cTable> GetReportableTables(Modules activeModule)
        {
            var clsElements = new cElements();
            var allowedTables = new AllowedTables();
            List<int> lstLicencedElementIDs = clsElements.GetLicencedModuleIDs(this.nAccountID, activeModule);

            var lstCustomItems = this.lstCachedCustomerTables.Values.Where(x1 => x1.AllowReportOn && x1.ElementID.HasValue && x1.TableSource == cTable.TableSourceType.CustomEntites && !string.IsNullOrEmpty(x1.Description));

            var lstItems = this.lstCachedMetabaseTables.Values.Where(x1 => x1.AllowReportOn && 
                                (x1.ElementID.HasValue && lstLicencedElementIDs.Contains(x1.ElementID.Value) || x1.ElementID == null )
                                && !string.IsNullOrEmpty(x1.Description)).ToList();

            lstItems.AddRange(lstCustomItems);
            return lstItems.OrderBy(x => x.Description).ToList();
        }

        /// <summary>
        /// Creates a drop down list of tables allowed to be reported on for the current active module
        /// </summary>
        /// <param name="activeModule">
        ///     the current active module
        /// </param>
        /// <returns>
        /// List Item Array of cTable records
        /// </returns>
        public List<cTable> GetItemsForReportDropDown(Modules activeModule)
        {
            var clsElements = new cElements();
            var allowedTables = new AllowedTables();
            List<int> lstLicencedElementIDs = clsElements.GetLicencedModuleIDs(this.nAccountID, activeModule);

            var lstCustomItems = this.lstCachedCustomerTables.Values.Where(x1 => x1.AllowReportOn && x1.ElementID.HasValue && x1.TableSource == cTable.TableSourceType.CustomEntites && !string.IsNullOrEmpty(x1.Description));

            var lstItems = this.lstCachedMetabaseTables.Values.Where(x1 => x1.AllowReportOn && x1.ElementID.HasValue && lstLicencedElementIDs.Contains(x1.ElementID.Value) && !string.IsNullOrEmpty(x1.Description)).ToList();

            var filteredList = FilterTableListBasedOnAllowedTables(lstItems, allowedTables);

            lstItems = filteredList;
            lstItems.AddRange(lstCustomItems);
            return lstItems.OrderBy(x => x.Description).ToList();
        }

        private static List<cTable> FilterTableListBasedOnAllowedTables(List<cTable> lstItems, AllowedTables allowedTables)
        {
            var filteredList = new List<cTable>();
            foreach (var item in lstItems)
            {
                if (allowedTables.Get(item.TableID) != null)
                {
                    filteredList.Add(item);
                }
            }
            return filteredList;
        }

        /// <summary>
        /// Gets list item elements for the RelationshipTextBox relate to areas
        /// </summary>
        /// <param name="includeNone">Indicate if a [None] entry should be inserted at the top of the list</param>
        /// <param name="customTablesOnly">Indicate if only custom entity tables are to be created. All tables included by default.</param>
        /// <param name="excludeTableList">List of table IDs to exclude as necessary</param>
        /// <param name="filterModule">Module enumerator to </param>
        /// <returns>List of permitted tables for inclusion in a drop downlist</returns>
        public List<ListItem> CreateEntityRelationshipDropDown(bool includeNone = false, bool customTablesOnly = false, List<Guid> excludeTableList = null, Modules filterModule = Modules.expenses)
        {
            // get a list of element IDs for the current module
            cModules modules = new cModules();

            List<int> lstLicencedElementIDs = modules.GetModuleElementIds(nAccountID, filterModule);

            List<ListItem> items;

            if (excludeTableList == null)
                excludeTableList = new List<Guid>();

            if (customTablesOnly)
            {
                items = (from x in this.lstCachedCustomerTables.Values
                         orderby x.Description
                         where !excludeTableList.Contains(x.TableID) && x.AllowEntityRelationship && !x.IsSystemView && ((x.ElementID.HasValue && lstLicencedElementIDs.Contains(x.ElementID.Value)) || (x.TableSource == cTable.TableSourceType.CustomEntites))
                         select new ListItem(x.Description, x.TableID.ToString())).ToList();

            }
            else
            {
                items = (from x in this.CombineCachedDictionaries().Values
                         orderby x.Description
                         where !excludeTableList.Contains(x.TableID) && x.AllowEntityRelationship && !x.IsSystemView && ((x.ElementID.HasValue && lstLicencedElementIDs.Contains(x.ElementID.Value)) || (x.TableSource == cTable.TableSourceType.CustomEntites))
                         select new ListItem(x.Description, x.TableID.ToString())).ToList();
            }


            if (includeNone)
                items.Insert(0, new ListItem("[None]", "0"));

            return items;
        }

        /// <summary>
        /// Create a dropdown with all the table names in
        /// </summary>
        /// <returns></returns>
        public ListItem[] CreateTablesDropDown()
        {
            List<ListItem> items = (from x in this.CombineCachedDictionaries().Values
                                    orderby x.Description
                                    select new ListItem(x.Description, x.TableID.ToString())).ToList();

            return items.ToArray();
        }

        public List<sTableBasics> getAllowedTables(Guid baseTableID, string connectionString)
        {
            List<sTableBasics> lstTables = new List<sTableBasics>();
            DBConnection expdata = new DBConnection(connectionString);

            expdata.sqlexecute.Parameters.AddWithValue("@baseTable", baseTableID);


            string strSQL = "SELECT DISTINCT(reports_allowedtables.tableid), tables.description FROM reports_allowedtables INNER JOIN tables ON tables.tableid=reports_allowedtables.tableid  AND reports_allowedtables.basetableid=@baseTable ORDER BY description";

            System.Data.SqlClient.SqlDataReader reader;

            using (reader = expdata.GetReader(strSQL))
            {
                sTableBasics tmpTableBasics;
                Guid tableid;
                string description;

                while (reader.Read())
                {
                    tableid = reader.GetGuid(0); //reader.GetOrdinal("tableid"));
                    description = reader.GetString(1); //reader.GetOrdinal("description"));
                    tmpTableBasics = new sTableBasics(tableid, description);
                    lstTables.Add(tmpTableBasics);
                }
                reader.Close();
            }
            return lstTables;
        }

        #endregion methods


        /// <summary>
        /// Get a count of all the existing alias table names associated to a custom entity
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int checkAliasTableNameCount(string name)
        {
            return (from x in CombineCachedDictionaries().Values
                    where x.TableID != Guid.Empty && x.TableName.Contains(name)
                    select x).Count();
        }
    }
}
