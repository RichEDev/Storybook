using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Server;
using System.Web.UI.WebControls;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;

namespace SpendManagementLibrary
{
    public abstract class cUserDefinedFieldGroupingsBase
    {
        protected int nAccountID;
        protected int nSubAccountID;
        protected string sConnectionString;
        protected const string sSQL = "SELECT userdefinedGroupID, groupName, [order], associatedTable, createdon, createdby, modifiedon, modifiedby FROM dbo.userdefinedGroupings";
        protected ConcurrentDictionary<int, SortedList<int, cUserdefinedFieldGrouping>> allGroupings = new ConcurrentDictionary<int, SortedList<int, cUserdefinedFieldGrouping>>();
        protected SortedList<int, cUserdefinedFieldGrouping> lstGroupings;

        #region properties

        public int AccountID
        {
            get { return nAccountID; }
        }

        #endregion

        protected static SortedList<int, cUserdefinedFieldGrouping> GetCollection(int accountId)
        {
            var connectionString = cAccounts.getConnectionString(accountId);
            var clsTables = new cTables(accountId);
            SortedList<int, cUserdefinedFieldGrouping> lstTemp = new SortedList<int, cUserdefinedFieldGrouping>();
            DBConnection data = new DBConnection(connectionString);
            SqlDataReader reader;
            using (reader = data.GetReader(sSQL))
            {
                while (reader.Read())
                {
                    int userdefinedGroupID = reader.GetInt32(reader.GetOrdinal("userdefinedGroupID"));
                    string groupName = reader.GetString(reader.GetOrdinal("groupName"));
                    int order = reader.GetInt32(reader.GetOrdinal("order"));
                    Guid associatedTbl = reader.GetGuid(reader.GetOrdinal("associatedTable"));
                    cTable tbl = clsTables.GetTableByID(associatedTbl);
                    DateTime createdOn = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    int createdBy;
                    if (!reader.IsDBNull(reader.GetOrdinal("createdby")))
                    {
                        createdBy = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    else
                    {
                        createdBy = 0;
                    }
                    DateTime? modifiedOn;
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                    {
                        modifiedOn = null;
                    }
                    else
                    {
                        modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    int? modifiedBy;
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                    {
                        modifiedBy = null;
                    }
                    else
                    {
                        modifiedBy = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }

                    lstTemp.Add(userdefinedGroupID, new cUserdefinedFieldGrouping(userdefinedGroupID, groupName, order, tbl, GetFilterCategories(userdefinedGroupID, connectionString), createdOn, createdBy, modifiedOn, modifiedBy));
                }
                reader.Close();
            }

            return lstTemp;
        }

        /// <summary>
        /// Return a specific grouping
        /// </summary>
        /// <param name="id">The ID of the grouping to return</param>
        /// <returns></returns>
        public cUserdefinedFieldGrouping GetGroupingByID(int id)
        {
            if (lstGroupings.ContainsKey(id))
            {
                return lstGroupings[id];
            }

            return null;
        }

        /// <summary>
        /// Returns a string for use in a cGridNew
        /// </summary>
        /// <returns></returns>
        public string GetGrid()
        {
            return "SELECT userdefinedGroupID, groupName, [tables].description FROM userdefinedGroupings";
        }

        protected int GetHighestOrder()
        {
            int highestOrder = 0;
            foreach (cUserdefinedFieldGrouping group in this.lstGroupings.Values)
            {
                if (group.Order > highestOrder)
                {
                    highestOrder = group.Order;
                }
            }

            return highestOrder;
        }

        protected int SaveGroupingBase(cCurrentUserBase clsCurrentUser, cUserdefinedFieldGrouping grouping, out DateTime lastUpdated)
        {
            int id;
            using (var sqlConnection = new SqlConnection(DatabaseConnection.GetConnectionStringWithDecryptedPassword(sConnectionString)))
            {
                sqlConnection.Open();
                using (var transaction = sqlConnection.BeginTransaction())
                {
                    var saveCommand = new SqlCommand("saveUserdefinedGrouping", sqlConnection, transaction) { CommandType = CommandType.StoredProcedure };
                    saveCommand.Parameters.AddWithValue("@userdefinedGroupID", grouping.UserdefinedGroupID);
                    saveCommand.Parameters.AddWithValue("@associatedTable", grouping.AssociatedTable.TableID);
                    saveCommand.Parameters.AddWithValue("@groupName", grouping.GroupName);

                    int groupOrder;
                    if (grouping.UserdefinedGroupID == 0)
                    {
                        groupOrder = this.GetHighestOrder() + 1;
                    }
                    else
                    {
                        groupOrder = grouping.Order;
                    }

                    saveCommand.Parameters.AddWithValue("@order", groupOrder);

                    if (grouping.ModifiedBy == null)
                    {
                        saveCommand.Parameters.AddWithValue("@date", grouping.CreatedOn);
                        saveCommand.Parameters.AddWithValue("@userid", grouping.CreatedBy);
                    }
                    else
                    {
                        saveCommand.Parameters.AddWithValue("@date", grouping.ModifiedOn);
                        saveCommand.Parameters.AddWithValue("@userid", grouping.ModifiedBy);
                    }
                    if (clsCurrentUser != null)
                    {
                        saveCommand.Parameters.AddWithValue("@CUemployeeID", clsCurrentUser.EmployeeID);
                        if (clsCurrentUser.isDelegate)
                        {
                            saveCommand.Parameters.AddWithValue("@CUdelegateID", clsCurrentUser.Delegate.EmployeeID);
                        }
                        else
                        {
                            saveCommand.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                        }
                    }
                    else
                    {
                        saveCommand.Parameters.AddWithValue("@CUemployeeID", 0);
                        saveCommand.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                    }
                    saveCommand.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
                    saveCommand.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
                    saveCommand.ExecuteNonQuery();

                    id = (int)saveCommand.Parameters["@returnvalue"].Value;

                    if (id > 0)
                    {

                        foreach (KeyValuePair<int, List<int>> kvp in grouping.FilterCategories)
                        {
                            var deleteCommand =
                                new SqlCommand(
                                    "delete from userdefinedGroupingAssociation where groupingId = @groupingId and subAccountId = @subAccountId", sqlConnection, transaction);
                            int subaccountId = kvp.Key;
                            deleteCommand.Parameters.AddWithValue("@groupingId", id);
                            deleteCommand.Parameters.AddWithValue("@subAccountId", subaccountId);
                            deleteCommand.ExecuteNonQuery();

                            List<int> cats = kvp.Value;
                            foreach (int catId in cats)
                            {
                                var insertCommand = new SqlCommand("insert into userdefinedGroupingAssociation (groupingId, categoryId, subAccountId) values (@groupingId, @categoryId, @subAccountId)", sqlConnection, transaction);
                                insertCommand.Parameters.Clear();
                                insertCommand.Parameters.AddWithValue("@subAccountId", subaccountId);
                                insertCommand.Parameters.AddWithValue("@groupingId", id);
                                insertCommand.Parameters.AddWithValue("@categoryId", catId);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    var lastUpdatedCommand = new SqlCommand("select getdate()", sqlConnection, transaction);
                    lastUpdated = (DateTime)lastUpdatedCommand.ExecuteScalar();
                    transaction.Commit();
                }
            }
            if (id == -1)
            {
                return -1;
            }

            return id;
        }

        protected void DeleteGroupingBase(cCurrentUserBase clsCurrentUser, int userDefinedGroupID, int employeeID, out DateTime lastUpdated)
        {
            IDBConnection data = new DatabaseConnection(sConnectionString);
            data.sqlexecute.Parameters.AddWithValue("@userdefinedGroupID", userDefinedGroupID);
            data.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);
            if (clsCurrentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", clsCurrentUser.EmployeeID);
                if (clsCurrentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", clsCurrentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            SqlParameter lastUpdatedParam = data.sqlexecute.Parameters.Add("@lastUpdated", SqlDbType.DateTime);
            lastUpdatedParam.Direction = ParameterDirection.Output;
            data.ExecuteProc("deleteUserdefinedGrouping");
            lastUpdated = (DateTime)lastUpdatedParam.Value;
            data.sqlexecute.Parameters.Clear();
        }

        public List<ListItem> CreateDropDown(Guid tableID)
        {
            List<ListItem> items = new List<ListItem>();
            SortedList<string, cUserdefinedFieldGrouping> sorted = this.SortListByTable(tableID);
            items.Add(new ListItem("[None]", "0"));
            foreach (cUserdefinedFieldGrouping grouping in sorted.Values)
            {
                items.Add(new ListItem(grouping.GroupName, grouping.UserdefinedGroupID.ToString()));
            }
            return items;
        }

        public Dictionary<int, cUserdefinedFieldGrouping> GetGroupingByAssocTable(Guid tableID)
        {
            Dictionary<int, cUserdefinedFieldGrouping> retColl = new Dictionary<int, cUserdefinedFieldGrouping>();

            foreach (cUserdefinedFieldGrouping grp in lstGroupings.Values)
            {
                if (grp.AssociatedTable.TableID == tableID)
                {
                    retColl.Add(grp.UserdefinedGroupID, grp);
                }
            }

            return retColl;
        }

        private SortedList<string, cUserdefinedFieldGrouping> SortListByTable(Guid tableID)
        {
            SortedList<string, cUserdefinedFieldGrouping> lst = new SortedList<string, cUserdefinedFieldGrouping>();

            foreach (cUserdefinedFieldGrouping grouping in lstGroupings.Values)
            {
                if (grouping.AssociatedTable.TableID == tableID)
                {
                    lst.Add(grouping.GroupName, grouping);
                }
            }
            return lst;
        }

        private static Dictionary<int, List<int>> GetFilterCategories(int groupingID, string connectionString)
        {
            DBConnection db = new DBConnection(connectionString);
            SqlDataReader reader;
            Dictionary<int, List<int>> cats = new Dictionary<int, List<int>>();
            string sql = "SELECT categoryId, subAccountId FROM userdefinedGroupingAssociation WHERE groupingId = @groupingId";
            db.sqlexecute.Parameters.AddWithValue("@groupingId", groupingID);

            using (reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    int catId = reader.GetInt32(0);
                    int subaccountId = reader.GetInt32(1);

                    if (!cats.ContainsKey(subaccountId))
                    {
                        cats.Add(subaccountId, new List<int>());
                    }
                    List<int> subaccCats = cats[subaccountId];
                    if (!subaccCats.Contains(catId))
                    {
                        subaccCats.Add(catId);
                    }
                }
                reader.Close();
            }
            return cats;
        }

        /// <summary>
        /// Saves the order of user defined groupings
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="appliesTo"></param>
        /// <param name="lastUpdated"></param>
        /// <returns></returns>
        protected void SaveOrdersBase(Dictionary<int, int> orders, cTable appliesTo, out DateTime lastUpdated)
        {
            List<SqlDataRecord> lstGroupOrderData = null; //pass null instead of an empty row set otherwise SQL throws an exception

            if (orders.Any())
            {
                lstGroupOrderData = new List<SqlDataRecord>();

                //-- Create the data type - required when passing in the sql table param below
                //CREATE TYPE dbo.UserdefinedGroupOrdering AS TABLE 
                //(
                // userdefinedGroupID int NOT NULL, 
                // displayOrder int NOT NULL, 
                // PRIMARY KEY (userdefinedGroupID)
                //)

                // Generate a sql table param and pass into the stored proc
                SqlMetaData[] tvpGroupOrder =
                    {
                        new SqlMetaData("userdefinedGroupID", SqlDbType.Int),
                        new SqlMetaData("displayOrder", SqlDbType.Int)
                    };

                foreach (KeyValuePair<int, int> kvp in orders)
                {
                    SqlDataRecord row = new SqlDataRecord(tvpGroupOrder);
                    row.SetInt32(0, kvp.Key);
                    row.SetInt32(1, kvp.Value);
                    lstGroupOrderData.Add(row);
                }
            }
            DBConnection smData = new DBConnection(sConnectionString);

            smData.sqlexecute.Parameters.Add("@groupOrder", System.Data.SqlDbType.Structured);
            smData.sqlexecute.Parameters["@groupOrder"].Direction = System.Data.ParameterDirection.Input;
            smData.sqlexecute.Parameters["@groupOrder"].Value = lstGroupOrderData;

            SqlParameter lastUpdatedParam = smData.sqlexecute.Parameters.Add("@lastUpdated", SqlDbType.DateTime);
            lastUpdatedParam.Direction = ParameterDirection.Output;
            smData.ExecuteProc("dbo.[SaveUserdefinedGroupOrder]");
            lastUpdated = (DateTime)lastUpdatedParam.Value;
        }
    }
}
