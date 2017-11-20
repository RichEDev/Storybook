namespace Spend_Management
{
    using System;

    using SpendManagementLibrary;

    /// <summary>
    /// This class deals with the saving of an aliased table to the custom tables database table 
    /// </summary>
    public class cCustomTables
    {
        private int nAccountID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AccountID"></param>
        public cCustomTables(int AccountID)
        {
            nAccountID = AccountID;
        }

        #region Properties

        public int AccountID
        {
            get { return nAccountID; }
        }

        #endregion

        /// <summary>
        /// Save the aliased custom table to the database and return the newly aliased table object
        /// </summary>
        /// <param name="table"></param>
        public cTable SaveCustomTable(int entityID, cTable table)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            cFields clsFields = new cFields(AccountID);
            cTables clsTables = new cTables(AccountID);
            Guid TableID = Guid.NewGuid();

            int tableCount = clsTables.checkAliasTableNameCount(entityID.ToString()) + 1;

            //The name is created with the prefix ct(custom table) then the entityID and then the number based on how many
            //table aliases exist for this custom entity
            string tableName = "ct_" + entityID.ToString() + "_" + tableCount.ToString();
           
            db.AddWithValue("@tableName", tableName, clsFields.GetFieldSize("customTables", "tablename"));
            db.sqlexecute.Parameters.AddWithValue("@joinType", 2); //Always need to be set to left join in case there are null values for the join field
            db.sqlexecute.Parameters.AddWithValue("@allowReportOn", table.AllowReportOn);

            if (table.Description != string.Empty)
            {
                db.AddWithValue("@description", table.Description, clsFields.GetFieldSize("customTables", "description"));
            }
            else
            {
                db.AddWithValue("@description", DBNull.Value, clsFields.GetFieldSize("customTables", "description"));
            }

            db.sqlexecute.Parameters.AddWithValue("@primaryKey", table.PrimaryKeyID);
            db.sqlexecute.Parameters.AddWithValue("@keyField", table.KeyFieldID);
            db.sqlexecute.Parameters.AddWithValue("@allowImport", table.AllowImport);
            db.sqlexecute.Parameters.AddWithValue("@allowWorkflow", table.AllowWorkflow);
            db.sqlexecute.Parameters.AddWithValue("@allowEntityRelationship", table.AllowEntityRelationship);
            db.sqlexecute.Parameters.AddWithValue("@tableID", TableID);
            db.sqlexecute.Parameters.AddWithValue("@hasUserdefinedFields", table.HasUserdefinedFields);
            db.sqlexecute.Parameters.AddWithValue("@userdefinedTable", table.UserDefinedTableID);
            db.sqlexecute.Parameters.AddWithValue("@parentTableID", table.TableID);

            db.ExecuteProc("dbo.SaveCustomTable");
            db.sqlexecute.Parameters.Clear();

            removeTableCache();

            return new cTable(this.AccountID,tableName, table.JoinType, table.Description, table.AllowReportOn, table.AllowImport, table.AllowWorkflow, table.AllowEntityRelationship, TableID, table.PrimaryKeyID,table.KeyFieldID, table.UserDefinedTableID, table.SubAccountIDFieldID, cTable.TableSourceType.CustomTables, (int)SpendManagementElement.CustomEntityInstances, table.SubAccountIDFieldID, false, string.Empty, false);

        }

        /// <summary>
        /// Remove the fields from cache to get the latest version   
        /// </summary>
        private void removeTableCache()
        {
            var cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
            cache.Remove("tables" + AccountID);
            var tables = new cTables(AccountID); //Calls the initialise data method to update the cache
            tables.ClearCustomerTablesCache();
        }
    }
}
