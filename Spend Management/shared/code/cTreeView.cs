using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Web.Caching;
using System.Configuration;

namespace Spend_Management
{
    /// <summary>
    /// Ajax TreeView
    /// </summary>
    public abstract class cTreeView
    {
        /// <summary>
        /// Initialiser
        /// </summary>
        protected int nAccountID;
        private System.Web.Caching.Cache MetabaseCache = System.Web.HttpRuntime.Cache;
        private System.Web.Caching.Cache LocalCommonColumnCache = System.Web.HttpRuntime.Cache;
        private SortedList<Guid, List<cField>> lstMetabaseCommonColumns;
        private SortedList<Guid, List<cField>> lstLocalCommonColumns;

        /// <summary>
        /// Constructor of the parent class
        /// </summary>
        /// <param name="AccountID"></param>
        public cTreeView(int AccountID)
        {
            nAccountID = AccountID;

            InitialiseData();
        }

        #region Properties

        /// <summary>
        /// AccountID of the customer
        /// </summary>
        public int AccountID
        {
            get { return nAccountID; }
        }

        #endregion

        /// <summary>
        /// Abstract method that ensures all classes that derive from this base use this method for getting nodes
        /// </summary>
        /// <param name="TableID"></param>
        /// <param name="IsSubNode"></param>
        /// <returns></returns>
        public abstract List<cTreeNode> GetNodes(Guid TableID, bool IsSubNode);

        /// <summary>
        /// Abstract method that ensures all classes that derive from this base use this method for getting fields
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        protected abstract List<cField> GetFields(Guid ID);
        /// <summary>
        /// Populate the common columns from the cache 
        /// </summary>
        private void InitialiseData()
        {
            lstMetabaseCommonColumns = (SortedList<Guid, List<cField>>)MetabaseCache["MetabaseCommonColumns"];
            lstLocalCommonColumns = (SortedList<Guid, List<cField>>)LocalCommonColumnCache["CommonColumns_" + AccountID];

            if (lstMetabaseCommonColumns == null)
            {
                CacheMetaBaseCommonColumns();
            }

            if (lstLocalCommonColumns == null)
            {
                CacheTreeviewColumns();
            }
        }

        /// <summary>
        /// Cache the reports common columns from the metabase
        /// </summary>
        /// <returns></returns>
        private SortedList<Guid, List<cField>> CacheMetaBaseCommonColumns()
        {
            SortedList<Guid, List<cField>> lstMetabaseColumns = new SortedList<Guid, List<cField>>();
            DBConnection db = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);

            string strsql = "SELECT tableid, fieldid from dbo.reports_common_columns";
            db.sqlexecute.CommandText = strsql;

            SqlDataReader reader;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                var dep = new SqlCacheDependency(db.sqlexecute);
                MetabaseCache.Insert("MetabaseCommonColumns", lstMetabaseColumns, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
            }

            using (reader = db.GetStoredProcReader("dbo.GetCommonColumns"))
            {
                ReadColumnsFromDB(ref reader, ref lstMetabaseColumns);
                reader.Close();
            }

            return lstMetabaseColumns;
        }

        /// <summary>
        /// Cache the tree view common columns from the local account database
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>SortedList</cref>
        ///     </see>
        ///     .
        /// </returns>
        private SortedList<Guid, List<cField>> CacheTreeviewColumns()
        {
            var lstColumns = new SortedList<Guid, List<cField>>();
            var db = new DBConnection(cAccounts.getConnectionString(this.AccountID));

            string strsql = string.Format("SELECT CacheExpiry from dbo.customEntityAttributes where {0} = {0}", this.AccountID);
            db.sqlexecute.CommandText = strsql;

            SqlDataReader reader;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                var dep = new SqlCacheDependency(db.sqlexecute);
                this.LocalCommonColumnCache.Insert("CommonColumns_" + this.AccountID, lstColumns, dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }

            using (reader = db.GetStoredProcReader("dbo.GetCommonColumns"))
            {
                this.ReadColumnsFromDB(ref reader, ref lstColumns);
                reader.Close();
            }

            return lstColumns;
        }

        /// <summary>
        /// Get a list of the common fields associated to a table
        /// </summary>
        /// <param name="TableID"></param>
        /// <returns></returns>
        protected List<cField> GetCommonFieldsByTableID(Guid TableID)
        {
            List<cField> lstFields = null;

            lstMetabaseCommonColumns.TryGetValue(TableID, out lstFields);

            if (lstFields == null)
            {
                lstLocalCommonColumns.TryGetValue(TableID, out lstFields);
            }

            return lstFields;
        }

        /// <summary>
        /// Read the columns from the database  and add them to the referenced collection from the specified datasource of the reader object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="lstColumns"></param>
        private void ReadColumnsFromDB(ref SqlDataReader reader, ref SortedList<Guid, List<cField>> lstColumns)
        {
            Guid TableID;
            Guid FieldID;
            cFields clsFields = new cFields(AccountID);
            cField field = null;
            List<cField> lstFields = null;

            while (reader.Read())
            {
                field = null;
                TableID = reader.GetGuid(0);
                FieldID = reader.GetGuid(1);
                field = clsFields.GetFieldByID(FieldID);

                if (lstColumns.ContainsKey(TableID))
                {
                    lstColumns[TableID].Add(field);
                }
                else
                {
                    lstFields = new List<cField>();
                    lstFields.Add(field);
                    lstColumns.Add(TableID, lstFields);
                }
            }
        }
    }

    /// <summary>
    /// The inherited tree view object for the reports section
    /// </summary>
    public class cReportTreeView : cTreeView
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="AccountID"></param>
        public cReportTreeView(int AccountID)
            : base(AccountID)
        {
        }

        /// <summary>
        /// Overridden method that gets the nodes for the specific inherited tree view type
        /// </summary>
        /// <param name="AssociatedID">This can be the tableID if not a sub node or the viewgroupID if is a sub node</param>
        /// <param name="IsSubNode"></param>
        /// <returns></returns>
        public override List<cTreeNode> GetNodes(Guid AssociatedID, bool IsSubNode)
        {
            List<cTreeNode> lstNodes = new List<cTreeNode>();

            DBConnection smData = new DBConnection(cAccounts.getConnectionString(nAccountID));
            smData.sqlexecute.Parameters.AddWithValue("@AssociatedID", AssociatedID);
            smData.sqlexecute.Parameters.AddWithValue("@IsSubNode", IsSubNode);

            #region Add the commonly used fields for the associated table or the fields associated to the view group
            List<cField> lstFields = null;

            if (!IsSubNode)
            {
                lstFields = GetCommonFieldsByTableID(AssociatedID);
            }
            else
            {
                lstFields = GetFields(AssociatedID);
            }

            if (lstFields != null)
            {
                lstNodes = (from fld in lstFields
                            select new cTreeNode(fld.FieldID, fld.FieldName, NodeType.Node)).ToList();

                //foreach (cField fld in lstFields)
                //{
                //    lstNodes.Add(new cTreeNode(fld.FieldID, fld.FieldName, NodeType.Node));
                //}
            }

            #endregion

            using (System.Data.SqlClient.SqlDataReader reader = smData.GetStoredProcReader("dbo.GetTreeViewNodes"))
            {
                Guid associatedID;
                string name;

                while (reader.Read())
                {
                    associatedID = reader.GetGuid(reader.GetOrdinal("viewgroupid"));
                    name = reader.GetString(reader.GetOrdinal("groupname"));

                    lstNodes.Add(new cTreeNode(associatedID, name, NodeType.Parent));
                }

                reader.Close();
            }

            smData.sqlexecute.Parameters.Clear();

            return lstNodes;
        }

        /// <summary>
        /// Get the fields for the associated view group
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        protected override List<cField> GetFields(Guid ID)
        {
            //List<cField> lstFields = new List<cField>();
            cFields clsFields = new cFields(nAccountID);
            SortedList<string, cField> viewGroupFields = clsFields.getFieldsByViewGroup(ID);

            //foreach (cField field in viewGroupFields.Values)
            //{
            //    lstFields.Add(field);
            //}

            List<cField> lstFields = (from x in viewGroupFields.Values
                         select x).ToList();
            return lstFields;
        }
    }
}
