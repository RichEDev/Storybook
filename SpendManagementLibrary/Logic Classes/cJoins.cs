namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    /// <summary>
    /// The joins class.
    /// </summary>
    public class cJoins
    {
        #region Static Fields

        /// <summary>
        /// The cache.
        /// </summary>
        private static readonly Cache Cache;

        #endregion

        #region Fields

        /// <summary>
        ///     The account id.
        /// </summary>
        private readonly int accountID;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises static members of the <see cref="cJoins"/> class.
        /// </summary>
        static cJoins()
        {
            Cache = new Cache();
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="cJoins" /> class.
        /// </summary>
        public cJoins()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="cJoins"/> class.
        /// </summary>
        /// <param name="accountID">
        /// The account id.
        /// </param>
        /// <param name="connection">
        /// The database connection.
        /// </param>
        public cJoins(int accountID)
        {
            this.accountID = accountID;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        private static string CacheKey
        {
            get
            {
                return "joins_";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Create custom joins in the database
        /// </summary>
        /// <param name="entityHierarchy">
        /// Hierarchical parent entities of the system view
        /// </param>
        /// <param name="sysViewEntity">
        /// System View Entity
        /// </param>
        /// <param name="relationshipAttributeId">
        /// Attribute ID of the 1:n attribute
        /// </param>
        public void ConstructJoin(
            List<cCustomEntity> entityHierarchy, cCustomEntity sysViewEntity, int relationshipAttributeId)
        {
            for (int entityIdx = entityHierarchy.Count - 1; entityIdx >= 0; entityIdx--)
            {
                int joinIdx = 1;
                SortedList<int, cJoinStep> steps = new SortedList<int, cJoinStep>();
                cCustomEntity topEntity = entityHierarchy[entityIdx];

                cJoin curJoin = this.GetJoin(topEntity.table.TableID, sysViewEntity.table.TableID);

                if (curJoin == null)
                {
                    // need to calculate join
                    this.CalculateJoinSteps(
                        topEntity.table.TableID, sysViewEntity.table.TableID, ref joinIdx, ref steps);

                    if (steps.Count > 0)
                    {
                        var newJoin = new cJoin(
                            this.accountID,
                            Guid.Empty,
                            entityHierarchy[0].table.TableID,
                            sysViewEntity.table.TableID,
                            entityHierarchy[0].entityname + " to " + sysViewEntity.entityname,
                            DateTime.Now);
                        this.addCustomEntityJoin(newJoin);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a join from the cached collection from source to destination tables
        /// </summary>
        /// <param name="basetableid">
        /// Source table id
        /// </param>
        /// <param name="destinationtableid">
        /// Destination table id
        /// </param>
        /// <returns>
        /// cJoin entity or NULL if requested join does not exist
        /// </returns>
        public cJoin GetJoin(Guid basetableid, Guid destinationtableid)
        {
            cJoin reqJoin = null;
            string key = this.GetJoinKey(basetableid, destinationtableid);

            if (!string.IsNullOrEmpty(key))
            {
                reqJoin = this.GetCache(key);

                if (reqJoin == null)
                {
                    reqJoin = this.CacheJoin(basetableid, destinationtableid).FirstOrDefault();
                    if (reqJoin == null)
                    {
                        cEventlog.LogEntry(
                            "Missing joins (getJoin()), basetableid: " + basetableid.ToString()
                            + ", destinationtableid: " + destinationtableid.ToString());
                    }
                }
            }

            return reqJoin;
        }

        /// <summary>
        /// Gives the SQL string for the aliased joins in a series of JoinViaParts within one JoinVia
        /// </summary>
        /// <param name="joins">
        /// The by-reference list of joins that is being built up for the query
        /// </param>
        /// <param name="trunk">
        /// The by-reference start of the tree for tracking the joins that have been created and their final alias names.
        ///     <para>
        /// The starting value should be of the form new ViaBranch("baseTableName")
        /// </para>
        /// </param>
        /// <param name="joinVia">
        /// The JoinVia that the SQL should be created for
        /// </param>
        /// <param name="field">
        /// The final field that is being joined by the JoinVia
        /// </param>
        /// <param name="baseTableId">
        /// The table id for the root table the query is based upon
        /// </param>
        /// <returns>
        /// Boolean indicating the success of adding the SQL joins for this request
        /// </returns>
        public bool GetJoinViaSQL(
            ref List<string> joins, ref ViaBranch trunk, JoinVia joinVia, cField field, Guid baseTableId)
        {
            // validate inputs
            if (joinVia == null || joinVia.JoinViaList == null)
            {
                return false;
            }

            var sql = new StringBuilder();
            var clsTables = new cTables(this.accountID);
            var clsFields = new cFields(this.accountID);

            cTable queryBaseTable = clsTables.GetTableByID(baseTableId);

            // The ViaBranch tree
            // The trunk ViaBranch holds the base table name and a collection of other ViaBranches (on the first call of this method within a query, this should be empty).
            // The collection of ViaBranches represents the tables that the base table is directly joined to and is indexed by tableId Guid.
            // Each ViaBranch in this collection holds the alias that has been used to reference the table on the right of the join, and a collection of the tables it is further joined to.
            // These aliases do get updated as the query is built *if* two different JoinVias share some of their "path" and the one that first creates the ViaBranch uses it as an intermediate point in its join and then a subsequent join in the query uses that ViaBranch as the "end point" for its join.
            // This is done to prevent having duplicate joins created for different parts of a query.
            // Duplicate joins using different aliases would otherwise make the query seriously under-perform with relatively few occurrances.
            ViaBranch branch = trunk;

            // reference to current context branch in the join tree, always starts at the trunk for each new JoinVia
            ViaBranch twig = null; // for creating new branches on the tree

            // <--- VIA[0]RelatedTable AS joinID ON prevTableOrASName.VIA[0]ID = currentAS.VIA[0]RelatedTablePK		*last step after a field via OR the last field via?*
            // <--- VIA[0]RelatedTable AS joinID_STEPNO ON prevTableOrASName.VIA[0]ID = currentAS.VIA[0]RelatedTablePK
            // <--- 	    jointable AS joinID_STEPNO ON prevTableOrASName.JT_PK = currentAS.JT_FK
            // <--- 	    VIA[1]TableID AS joinID ON prevTableOrAsName.JT_FK = currentAS.JT_PK
            // <--- VIA[0]RelatedTable AS joinID_STEPNO ON prevTableOrASName.VIA[0]ID = currentAS.VIA[0]RelatedTablePK
            // <--- 	    jointable AS joinID_STEPNO ON prevTableOrASName.JT_PK = currentAS.JT_FK
            // <--- 	    VIA[1]TableID AS joinID_STEPNO ON prevTableOrAsName.JT_FK = currentAS.JT_PK
            //// <--- 	    VIA[2]RelatedTable AS joinID ON prevTableOrASName.VIA[2]ID = currentAS.VIA[2]RelatedTablePK
            int guidSuffix = 1;
            string currentAs = queryBaseTable.TableName;
            Guid previousTableId = baseTableId;

            // used in the "table" type JoinViaPart lookups (but needs to be maintained in both types)
            foreach (KeyValuePair<int, JoinViaPart> viaKeyValuePair in joinVia.JoinViaList)
            {
                twig = new ViaBranch();
                var via = viaKeyValuePair.Value;
                if (branch.UnderBranches.ContainsKey(via.ViaID))
                {
                    // set the "previousTableId" to the final table of this existing part
                    switch (via.ViaIDType)
                    {
                        case JoinViaPart.IDType.Field:
                            cField viaField = clsFields.GetFieldByID(via.ViaID);
                            if (viaField == null || viaField.GetRelatedTable() == null
                                || viaField.GetRelatedTable().GetPrimaryKey() == null)
                            {
                                return false;
                            }

                            previousTableId = viaField.GetRelatedTable().TableID == baseTableId ? viaField.TableID : viaField.GetRelatedTable().TableID;
                            break;

                        case JoinViaPart.IDType.Table:
                            previousTableId = via.ViaID;
                            break;
                    }

                    // reuse the existing alias and branch
                    currentAs = branch.UnderBranches[via.ViaID].TableName;
                    twig = branch.UnderBranches[via.ViaID];
                }
                else
                {
                    // Create the new branch and context info
                    List<JoinViaPartDetails> joinDetails;
                    if (
                        !this.GetDetailsForJoinViaPart(
                            ref clsTables, ref clsFields, out joinDetails, via, previousTableId))
                    {
                        return false;
                    }

                    foreach (JoinViaPartDetails join in joinDetails)
                    {
                        string previousAs = currentAs;
                        currentAs = $"{via.ViaID}_{viaKeyValuePair.Key}";
                        guidSuffix++;
                        var leftThenRight = via.ViaIDType == JoinViaPart.IDType.RelatedTable && @join.JoinType == JoinViaPart.JoinType.LEFT && @join.Left.Column.GetParentTable().TableName != previousAs;
                        JoinViaPart.JoinType joinType;
                        if (leftThenRight)
                        {
                            joinType = join.Left.Column.GetParentTable().JoinType == 1 ? JoinViaPart.JoinType.INNER : JoinViaPart.JoinType.LEFT;
                        }
                        else
                        {
                            joinType = join.Right.Table.JoinType == 1
                                ? JoinViaPart.JoinType.INNER
                                : JoinViaPart.JoinType.LEFT;
                        }

                        sql.Append(
                            $" {joinType} JOIN [{(leftThenRight ? @join.Left.Column.GetParentTable().TableName : @join.Right.Table.TableName)}] AS [{currentAs}] ON [{previousAs}].[{(leftThenRight ? @join.Right.Column.FieldName : @join.Left.Column.FieldName)}] = [{currentAs}].[{(leftThenRight ? @join.Left.Column.FieldName : @join.Right.Column.FieldName)}]");
                        if (leftThenRight)
                        {
                            previousTableId = joinDetails.Last().Left.Column.TableID;
                        }
                        else
                        {
                            previousTableId = joinDetails.Last().Right.Table.TableID;
                        }

                    }

                    if (joinDetails.Count > 0)
                    {
                        branch.UnderBranches.Add(via.ViaID, twig);
                        twig.TableName = currentAs;
                    }
                }

                branch = twig; // update the context to the newly created or reused branch
            }

            if (twig != null)
            {
                // Add this JoinVias SQL to the overall Query if it does not already exist.
                var currentJoins = string.Join("", joins).Replace(" ", "");
                var newJoin = sql.ToString().Replace(" ", "");
                var sqlJoin = sql.ToString();
                if (!string.IsNullOrEmpty(newJoin) && !currentJoins.Contains(newJoin))
                {
                    var newItem = true;
                    for (int index = 0; index < joins.Count; index++)
                    {
                        string joinValue = joins[index];
                        if (joinValue.StartsWith(sqlJoin))
                        {
                            newItem = false;
                        }

                        if (sqlJoin.StartsWith(joinValue))
                        {
                            joins[index] = sqlJoin;
                            newItem = false;
                        }
                    }

                    if (newItem)
                    {
                        joins.Add(sqlJoin);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The add custom entity join.
        /// </summary>
        /// <param name="join">
        /// The join.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int addCustomEntityJoin(cJoin join, IDBConnection connection = null)
        {
            int retJoinId;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountID)))
            {
                retJoinId = 0;

                expdata.sqlexecute.Parameters.Clear();
                Guid joinTableId = @join.jointableid;

                if (joinTableId == Guid.Empty)
                {
                    // get a new GUID
                    joinTableId = Guid.NewGuid();
                }

                expdata.sqlexecute.Parameters.AddWithValue("@jointableid", joinTableId);
                expdata.sqlexecute.Parameters.AddWithValue("@tableid", @join.destinationtableid);
                expdata.sqlexecute.Parameters.AddWithValue("@basetableid", @join.basetableid);
                expdata.sqlexecute.Parameters.AddWithValue("@description", @join.description);
                expdata.sqlexecute.Parameters.AddWithValue("@amendedon", DateTime.Now);
                string sql =
                    "insert into customJoinTables (jointableid, tableid, basetableid, description, amendedon) values (@jointableid, @tableid, @basetableid, @description, @amendedon)";
                expdata.ExecuteSQL(sql);

                sql =
                    "insert into customJoinBreakdown (jointableid, [order], tableid, sourcetable, joinkey, destinationkey, amendedon) values (@jointableid, @order, @tableid, @sourcetableid, @joinkey, @destinationkey, @amendedon)";

                // insert the join breakdown entries
                foreach (var s in @join.Steps.Steps)
                {
                    cJoinStep step = s.Value;
                    expdata.sqlexecute.Parameters.Clear();
                    expdata.sqlexecute.Parameters.AddWithValue("@jointableid", joinTableId);
                    expdata.sqlexecute.Parameters.AddWithValue("@order", step.order);
                    expdata.sqlexecute.Parameters.AddWithValue("@tableid", step.destinationtableid);
                    expdata.sqlexecute.Parameters.AddWithValue("@joinkey", step.joinkey);
                    expdata.sqlexecute.Parameters.AddWithValue("@sourcetableid", step.sourcetableid);
                    expdata.sqlexecute.Parameters.AddWithValue("@destinationkey", step.destinationkey);
                    expdata.sqlexecute.Parameters.AddWithValue("@amendedon", step.amendedon);

                    expdata.ExecuteSQL(sql);
                }
            }

            this.CacheAdd(join);

            return retJoinId;
        }

        /// <summary>
        /// cQueryBuilder, cMisc, cQeForms
        /// </summary>
        /// <param name="flds">
        /// </param>
        /// <param name="basetable">
        /// </param>
        /// <param name="lstJoinVia">
        /// </param>
        /// <param name="customEntityListFieldOrders">
        /// The custom Entity List Field Orders.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string createJoinSQL(
            SortedList<Guid, cField> flds,
            Guid basetable,
            SortedList<Guid, JoinVia> lstJoinVia,
            Dictionary<Guid, int> customEntityListFieldOrders = null)
        {
            var output = new StringBuilder();
            var tableids = new List<Guid>();
            var otmJoins = new Dictionary<string, string>();
            cJoin join;
            var trunk = new ViaBranch("baseTable");

            var joins = new List<string>();
            var sysviewJoins = new List<string>();
            cField fld;
            Guid tableGuid;

            if (lstJoinVia == null)
            {
                lstJoinVia = new SortedList<Guid, JoinVia>();
            }

            foreach (var kvp in flds)
            {
                fld = kvp.Value;

                // This "if" checks to make sure the field is not one of ProjectCode, Department or CostCode
                // or if it is one of these, then that the base table is not one of the savedexpenses tables (current/previous/combined)
                if (fld.FieldID != new Guid(ReportFields.CostCodesCostCode)
                    && fld.FieldID != new Guid(ReportFields.DepartmentsDepartment)
                    && fld.FieldID != new Guid(ReportFields.ProjectCodesProjectCode)
                    || ((fld.FieldID == new Guid(ReportFields.CostCodesCostCode)
                         || fld.FieldID == new Guid(ReportFields.DepartmentsDepartment)
                         || fld.FieldID == new Guid(ReportFields.ProjectCodesProjectCode))
                        && basetable != new Guid(ReportTable.SavedExpenses)))
                {
                    if ((tableids.Contains(fld.GetParentTable().TableID) == false && fld.GetParentTable().TableID != basetable)
                        || lstJoinVia.ContainsKey(kvp.Key))
                    {
                        // If the field is an alias table field then the parent table of the alias fields alias table needs to be returned
                        if (fld.FieldCategory == FieldCategory.AliasTableField)
                        {
                            tableGuid = fld.GetParentTable().TableID;
                        }
                        else
                        {
                            tableGuid = fld.TableID;
                        }

                        if (!lstJoinVia.ContainsKey(kvp.Key))
                        {
                            tableids.Add(tableGuid);

                            join = this.GetJoinByTable(basetable + "," + fld.TableID);
                            this.GetJoinSQL(ref joins, join, fld, ref sysviewJoins);
                        }
                        else
                        {
                            this.GetJoinViaSQL(ref joins, ref trunk, lstJoinVia[kvp.Key], fld, basetable);
                        }
                    }
                }

                if (fld.FieldSource == cField.FieldSourceType.CustomEntity && fld.GenList && fld.ListItems.Count == 0)
                {
                    // n:1 relationship field, so create join using lookup field and lookup table
                    string joinStr = " LEFT JOIN [" + fld.GetLookupTable().TableName + "] ON [" + fld.GetLookupTable().TableName
                                     + "].[" + fld.GetLookupTable().GetPrimaryKey().FieldName + "] = ["
                                     + fld.GetParentTable().TableName + "].[" + fld.FieldName + "] ";
                    if (!otmJoins.ContainsKey(basetable.ToString() + "," + fld.GetLookupTable().TableID.ToString()))
                    {
                        otmJoins.Add(basetable.ToString() + "," + fld.GetLookupTable().TableID.ToString(), joinStr);
                    }
                }

                if ((fld.FieldSource == cField.FieldSourceType.CustomEntity
                     || fld.FieldSource == cField.FieldSourceType.Userdefined) && fld.ValueList)
                {
                    int order = 0;
                    if (customEntityListFieldOrders != null)
                    {
                        customEntityListFieldOrders.TryGetValue(fld.FieldID, out order);
                    }

                    // int numCeListItemTableJoins =
                    // (from x in joins where x.Contains("customEntityAttributeListItems") select x).Count();
                    string listItemsTable = fld.FieldSource == cField.FieldSourceType.CustomEntity
                                                ? "customEntityAttributeListItems"
                                                : "userdefined_list_items";

                    string joinTable = string.Format("{1}_{0}", order, listItemsTable);
                    string entityValueListJoin =
                        string.Format(
                            " LEFT JOIN [{3}] AS [{0}] ON [{0}].[valueid] = [{1}].[{2}] ",
                            joinTable,
                            lstJoinVia.ContainsKey(kvp.Key)
                                ? lstJoinVia[kvp.Key].TableAlias
                                : fld.GetParentTable().TableName,
                            fld.FieldName,
                            listItemsTable);
                    if (!joins.Contains(entityValueListJoin))
                    {
                        joins.Add(entityValueListJoin);
                    }
                }
            }

            foreach (Guid tbl in tableids)
            {
                if (otmJoins.ContainsKey(basetable + "," + tbl))
                {
                    // if dynamically generated one-to-many joins have been generated, no need to add again
                    otmJoins.Remove(basetable + "," + tbl);
                }
            }

            foreach (var kvp in otmJoins)
            {
                if (!joins.Contains(kvp.Value))
                {
                    joins.Add(kvp.Value);
                }
            }

            foreach (string strjoin in joins)
            {
                output.Append(strjoin);
            }

            foreach (string strjoin in sysviewJoins)
            {
                output.Append(strjoin);
            }

            return output.ToString();
        }

        /// <summary>
        /// cEmployees - ONLY USED IN OLD USERVIEWS
        /// </summary>
        /// <param name="flds">
        /// </param>
        /// <param name="userview">
        /// </param>
        /// <param name="defaultView">
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string createJoinSQL(List<cField> flds, UserView userview, bool defaultView)
        {
            var output = new StringBuilder();
            var tableids = new List<Guid>();
            cJoin join;

            // get list of tables
            Guid basetable;
            var joins = new List<string>();
            var dummy = new List<string>();

            if (defaultView)
            {
                basetable = new Guid(ReportTable.SavedExpenses);
            }
            else
            {
                if (userview == UserView.Previous)
                {
                    basetable = new Guid(ReportTable.SavedExpenses);
                }
                else
                {
                    basetable = new Guid(ReportTable.SavedExpenses);
                }
            }

            foreach (cField fld in flds)
            {
                if (fld.FieldType != "FD" && fld.FieldType != "FS")
                {
                    if (tableids.Contains(fld.GetParentTable().TableID) == false && fld.GetParentTable().TableID != basetable)
                    {
                        if (fld.FieldCategory == FieldCategory.AliasTableField)
                        {
                            tableids.Add(fld.GetParentTable().TableID);

                            join = this.GetJoinByTable(basetable + "," + fld.TableID);
                            this.GetJoinSQL(ref joins, join, fld, ref dummy);
                        }
                        else
                        {
                            tableids.Add(fld.TableID);

                            join = this.GetJoinByTable(basetable + "," + fld.TableID);
                            this.GetJoinSQL(ref joins, join, fld, ref dummy);
                        }
                    }
                }
            }

            foreach (string strjoin in joins)
            {
                output.Append(strjoin);
            }

            return output.ToString();
        }

        /// <summary>
        /// cReports and cExports
        /// </summary>
        /// <param name="flds">
        /// </param>
        /// <param name="basetable">
        /// </param>
        /// <param name="accessrolelevel">
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string createReportJoinSQL(List<cField> flds, Guid basetable, AccessRoleLevel accessrolelevel)
        {
            var output = new StringBuilder();
            var tableids = new List<Guid>();
            cJoin join;

            // get list of tables
            var joins = new List<string>();
            var dummy = new List<string>(); // param only for custom entities

            if (basetable == new Guid(ReportTable.SavedExpenses)
                && tableids.Contains(new Guid(ReportTable.Claims)) == false)
            {
                tableids.Add(new Guid(ReportTable.Claims));
                join = this.GetJoinByTable(string.Format("{0},{1}", basetable, ReportTable.Claims));
                this.GetJoinSQL(ref joins, join, null, ref dummy);
            }

            if ((basetable == new Guid(ReportTable.Claims)
                 || basetable == new Guid(ReportTable.Holidays)
                 || basetable == new Guid(ReportTable.SavedExpenses))
                && tableids.Contains(new Guid(ReportTable.Employees)) == false)
            {
                tableids.Add(new Guid(ReportTable.Employees));
                join = this.GetJoinByTable(string.Format("{0},{1}", basetable, ReportTable.Employees));
                this.GetJoinSQL(ref joins, join, null, ref dummy);
            }

            if (accessrolelevel == AccessRoleLevel.EmployeesResponsibleFor && cReport.canFilterByRole(basetable)
                && !tableids.Contains(new Guid(ReportTable.Employees)))
            {
                tableids.Add(new Guid(ReportTable.Employees));
                join = this.GetJoinByTable(basetable + "," + ReportTable.Employees);
                this.GetJoinSQL(ref joins, join, null, ref dummy);
            }

            switch (basetable.ToString().ToUpper())
            {
                case ReportTable.ContractProductDetails:
                case ReportTable.InvoiceDetails:
                case ReportTable.InvoiceForecasts:
                    if (!tableids.Contains(new Guid(ReportTable.ContractDetails)))
                    {
                        tableids.Add(new Guid(ReportTable.ContractDetails));
                        this.GetJoinSQL(
                            ref joins,
                            this.GetJoinByTable(basetable + "," + ReportTable.ContractDetails.ToLower()),
                            null,
                            ref dummy);
                    }

                    break;
                case ReportTable.SupplierContacts:
                    if (!tableids.Contains(new Guid(ReportTable.SupplierDetails)))
                    {
                        tableids.Add(new Guid(ReportTable.SupplierDetails));
                        this.GetJoinSQL(
                            ref joins,
                            this.GetJoinByTable(basetable + "," + ReportTable.SupplierDetails.ToLower()),
                            null,
                            ref dummy);
                    }

                    break;
                default:
                    break;
            }

            foreach (cField fld in flds)
            {
                if (fld == null)
                {
                    continue;
                }

                if (fld.FieldID.ToString().ToUpper() == "8E430C33-87E7-4458-B871-3C7CF8406B03")
                {
                    join = this.GetJoinByTable(basetable + "," + "BF13D307-6F4A-40CB-906D-72530F64791E");
                    this.GetJoinSQL(ref joins, join, fld, ref dummy);
                }
                else if (tableids.Contains(fld.GetParentTable().TableID) == false && fld.GetParentTable().TableID != basetable)
                {
                    if (fld.FieldCategory == FieldCategory.AliasTableField)
                    {
                        tableids.Add(fld.GetParentTable().TableID);

                        join = this.GetJoinByTable(basetable + "," + fld.TableID);
                        this.GetJoinSQL(ref joins, join, fld, ref dummy);
                    }
                    else
                    {
                        tableids.Add(fld.TableID);

                        join = this.GetJoinByTable(basetable + "," + fld.TableID);
                        this.GetJoinSQL(ref joins, join, fld, ref dummy);
                    }
                }
            }

            // loop and create LEFT JOIN for any relationship text box udfs or n:1 relationship fields
            foreach (cField fld in flds)
            {
                if (fld == null)
                {
                    continue;
                }

                if (fld.FieldSource != cField.FieldSourceType.Metabase && fld.FieldType == "N"
                    && fld.GetRelatedTable() != null && fld.IsForeignKey)
                {
                    string joinSQL = " left join [" + fld.GetRelatedTable().TableName + "] as [rel" + fld.FieldName
                                     + "] on [rel" + fld.FieldName + "].[" + fld.GetRelatedTable().GetPrimaryKey().FieldName
                                     + "] = [" + fld.GetParentTable().TableName + "].[" + fld.FieldName + "]";

                    if (!dummy.Contains(joinSQL))
                    {
                        dummy.Add(joinSQL);
                    }
                }

                if (fld.FieldType == "R")
                {
                    string joinSQL = " left join [" + fld.GetRelatedTable().TableName + "] as [rel" + fld.FieldName
                                     + "] on [rel" + fld.FieldName + "].[" + fld.GetRelatedTable().GetPrimaryKey().FieldName
                                     + "] = [" + fld.GetParentTable().TableName + "].[" + fld.FieldName + "]";

                    if (!joins.Contains(joinSQL))
                    {
                        joins.Add(joinSQL);
                    }
                }
            }

            foreach (string strjoin in joins)
            {
                output.Append(strjoin);
            }

            // add any custom entity joins after standard field joins
            foreach (string strjoin in dummy)
            {
                output.Append(strjoin);
            }

            return output + " ";
        }

        #endregion

        #region Methods

        /// <summary>
        /// The cache add.
        /// </summary>
        /// <param name="join">
        /// The join.
        /// </param>
        private void CacheAdd(cJoin join)
        {
            if (join.Steps.Count > 0)
            {
                Cache.Add(this.accountID, CacheKey, this.GetJoinKey(join), join);
            }
        }

        /// <summary>
        /// The cache delete.
        /// </summary>
        /// <param name="join">
        /// The join.
        /// </param>
        public void CacheDelete(cJoin join)
        {
            Cache.Delete(this.accountID, CacheKey, this.GetJoinKey(join));
        }

        /// <summary>
        /// Cache Join.
        /// </summary>
        /// <param name="baseTable">
        /// The base table.
        /// </param>
        /// <param name="destinationTable">
        /// The destination table.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private IEnumerable<cJoin> CacheJoin(Guid baseTable, Guid destinationTable, IDBConnection connection = null)
        {
            List<cJoin> result;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountID)))
            {
                result = new List<cJoin>();

                expdata.sqlexecute.Parameters.Clear();
                string strSQL = "SELECT jointableid, tableid, basetableid, description, amendedon FROM [dbo].";

                strSQL += "[jointables] WHERE [tableid] = @destinationtableid and [basetableid] = @basetableid ";
                expdata.sqlexecute.Parameters.AddWithValue("@destinationtableid", destinationTable);
                expdata.sqlexecute.Parameters.AddWithValue("@basetableid", baseTable);

                using (IDataReader reader = expdata.GetReader(strSQL))
                {
                    int joinTableID_ordID = reader.GetOrdinal("jointableid");
                    int tableID_ordID = reader.GetOrdinal("tableid");
                    int baseTableID_ordID = reader.GetOrdinal("basetableid");
                    int description_ordID = reader.GetOrdinal("description");
                    int amendedon_ordID = reader.GetOrdinal("amendedon");

                    while (reader.Read())
                    {
                        Guid joinTableID = reader.GetGuid(joinTableID_ordID);
                        Guid tableID = reader.GetGuid(tableID_ordID);
                        Guid baseTableID = reader.GetGuid(baseTableID_ordID);
                        string description = reader.IsDBNull(description_ordID)
                                                 ? string.Empty
                                                 : reader.GetString(description_ordID);
                        DateTime amendedon = reader.IsDBNull(amendedon_ordID)
                                                 ? new DateTime(1900, 01, 01)
                                                 : reader.GetDateTime(amendedon_ordID);
                        var tmpJoin = new cJoin(
                            this.accountID, joinTableID, baseTableID, tableID, description, amendedon);
                        this.CacheAdd(tmpJoin);
                        result.Add(tmpJoin);
                    }

                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Calls stored procedure that calculate the join steps from source to destination table (if possible)
        /// </summary>
        /// <param name="sourceTableId">
        /// ID of source table
        /// </param>
        /// <param name="destinationTableId">
        /// ID of destination table
        /// </param>
        /// <param name="joinIdx">
        /// Current sequence index of join step
        /// </param>
        /// <param name="steps">
        /// Current collection of join steps
        /// </param>
        private void CalculateJoinSteps(
            Guid sourceTableId, Guid destinationTableId, ref int joinIdx, ref SortedList<int, cJoinStep> steps, IDBConnection connection = null)
        {
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountID)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@baseTableId", sourceTableId);
                expdata.sqlexecute.Parameters.AddWithValue("@tableId", destinationTableId);

                using (IDataReader reader = expdata.GetReader("calculateCustomJoinSteps", CommandType.StoredProcedure))
                {
                    Guid srcTableId;
                    Guid destinationKey;
                    Guid dstTableId;
                    Guid joinKey;

                    int srcTblOrd = reader.GetOrdinal("sourceTableId");
                    int srcFldOrd = reader.GetOrdinal("sourceFieldId");
                    int dstTblOrd = reader.GetOrdinal("destinationTableId");
                    int dstFldOrd = reader.GetOrdinal("destinationFieldId");

                    while (reader.Read())
                    {
                        srcTableId = reader.GetGuid(srcTblOrd);
                        destinationKey = reader.GetGuid(srcFldOrd);
                        dstTableId = reader.GetGuid(dstTblOrd);
                        joinKey = reader.GetGuid(dstFldOrd);

                        steps.Add(
                            joinIdx,
                            new cJoinStep(
                                Guid.Empty,
                                Guid.Empty,
                                Convert.ToByte(joinIdx),
                                dstTableId,
                                srcTableId,
                                joinKey,
                                DateTime.Now,
                                destinationKey));
                        joinIdx++;
                    }

                    reader.Close();
                }
            }

            return;
        }

        /// <summary>
        /// Generates the details of the individual joins that make up the JoinViaPart, used for GetJoinViaSQL
        /// </summary>
        /// <param name="clsTables">
        /// A reference to the tables collection
        /// </param>
        /// <param name="clsFields">
        /// A reference to the fields collection
        /// </param>
        /// <param name="joinDetails">
        /// A collection of SQL join details
        /// </param>
        /// <param name="via">
        /// The current JoinViaPart
        /// </param>
        /// <param name="previousTableId">
        /// The last join endpoint's table id
        /// </param>
        /// <returns>
        /// Boolean indicating success
        /// </returns>
        private bool GetDetailsForJoinViaPart(
            ref cTables clsTables,
            ref cFields clsFields,
            out List<JoinViaPartDetails> joinDetails,
            JoinViaPart via,
            Guid previousTableId)
        {
            joinDetails = new List<JoinViaPartDetails>();

            /* 
             * Field Joins - join logic is a straight foreignKey/primaryKey joins
             * Table Joins - join logic is constructed from entries in jointables with basetableid of previousTableId and tableid of via.ViaID
             */
            var oldPreviousTableId = previousTableId;
            switch (via.ViaIDType)
            {
                case JoinViaPart.IDType.Field:

                    cField viaField = clsFields.GetFieldByID(via.ViaID);

                    if (viaField == null || viaField.GetRelatedTable() == null
                        || viaField.GetRelatedTable().GetPrimaryKey() == null)
                    {
                        // Something wrong, so see if we can find the join from the previous table
                        if (viaField != null)
                        {
                            var newViaField =
                                clsFields
                                    .GetFieldsByTableID(previousTableId).FirstOrDefault(f => f.RelatedTableID == viaField.TableID);

                            // The join exists between contract_details and supplier_details and not supplierdetailsummary_view which previously existed. Hence hard coding this join.
                            if (viaField.TableID == new Guid(ReportTable.SupplierDetailsSummaryView)
                                   && previousTableId == new Guid(ReportTable.ContractDetails))
                            {
                                newViaField = clsFields.GetFieldByID(new Guid(ReportFields.ContractDetailsSupplierId));
                            }

                            if (newViaField != null)
                            {
                                viaField = newViaField;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (viaField.FieldSource == cField.FieldSourceType.CustomEntity && !viaField.IsForeignKey)
                    {
                        joinDetails.Add(
                            new JoinViaPartDetails
                            {
                                JoinType = via.TypeOfJoin,
                                Left =
                                        new JoinViaPartDetails.JoinSide
                                        { Column = clsTables.GetTableByID(previousTableId).GetPrimaryKey() },
                                Right =
                                        new JoinViaPartDetails.JoinSide
                                        {
                                            Table = viaField.GetParentTable(),
                                            Column = viaField
                                        }
                            });
                    }
                    else
                    {
                        if (viaField.IsForeignKey && viaField.TableID != previousTableId
                            && viaField.RelatedTableID == previousTableId)
                        {
                            joinDetails.Add(
                                new JoinViaPartDetails
                                {
                                    JoinType = via.TypeOfJoin,
                                    Left =
                                            new JoinViaPartDetails.JoinSide
                                            {
                                                Column =
                                                        clsTables
                                                        .GetTableByID(
                                                            previousTableId)
                                                        .GetPrimaryKey()
                                            },
                                    Right =
                                            new JoinViaPartDetails.JoinSide
                                            {
                                                Table =
                                                        viaField
                                                        .GetParentTable(),
                                                Column = viaField
                                            }
                                });
                        }
                        else
                        {
                            joinDetails.Add(
                                new JoinViaPartDetails
                                {
                                    JoinType = via.TypeOfJoin,
                                    Left = new JoinViaPartDetails.JoinSide { Column = viaField },
                                    Right =
                                            new JoinViaPartDetails.JoinSide
                                            {
                                                Table =
                                                        viaField
                                                        .GetRelatedTable(),
                                                Column =
                                                        viaField
                                                        .GetRelatedTable()
                                                        .GetPrimaryKey()
                                            }
                                });
                        }
                    }

                    break;

                case JoinViaPart.IDType.Table:
                    {
                        var leftTable = clsTables.GetTableByID(previousTableId);
                        var leftField = clsFields.GetFieldByID(leftTable.PrimaryKeyID);
                        var rightTable = clsTables.GetTableByID(via.ViaID);
                        var relatedToLeftField =
                            clsFields.GetAllRelatedFields(previousTableId).FirstOrDefault(x => x.TableID == rightTable.TableID);
                        cField rightField = null;
                        if (relatedToLeftField == null)
                        {
                            rightField = rightTable.GetPrimaryKey();
                        }
                        else
                        {
                            rightField = relatedToLeftField;
                        }

                        if (rightField == null)
                        {
                            return false;
                        }

                        if (leftField == null)
                        {
                            var relatedToRightField =
                            clsFields.GetAllRelatedFields(rightTable.TableID).FirstOrDefault(x => x.TableID == leftTable.TableID);
                            if (relatedToRightField != null)
                            {
                                leftField = relatedToRightField;
                            }
                        }

                        joinDetails.Add(
                            new JoinViaPartDetails
                            {
                                JoinType = via.TypeOfJoin,
                                Left = new JoinViaPartDetails.JoinSide { Column = leftField },
                                Right =
                                        new JoinViaPartDetails.JoinSide
                                        {
                                            Table = rightTable,
                                            Column = rightField
                                        }
                            });

                        previousTableId = rightTable.TableID;
                    }

                    break;
                case JoinViaPart.IDType.RelatedTable:
                    // at this point the via.ID is the destinanation field.  The source field is the primary key on the previousTable
                    var tables = new cTables(this.accountID);
                    var fields = new cFields(this.accountID);

                    var currentViaField = fields.GetFieldByID(via.ViaID);
                    if (currentViaField != null)
                    {
                        var relatedTable = currentViaField.GetRelatedTable();
                        if (relatedTable != null)
                        {
                            var targetField = relatedTable.GetPrimaryKey();
                            if (targetField != null)
                            {
                                cTable previousTable = tables.GetTableByID(previousTableId);

                                cField sourceField = fields.GetFieldByID(via.ViaID);

                                if (oldPreviousTableId != previousTable.TableID)
                                {
                                    // does not match previous table so need to add a step
                                    var related = fields.GetAllRelatedFields(previousTableId);
                                    cField relatedField =
                                        related.FirstOrDefault(field => field.TableID == previousTableId);

                                    if (relatedField != null)
                                    {
                                        joinDetails.Add(
                                            new JoinViaPartDetails
                                            {
                                                JoinType = via.TypeOfJoin,
                                                Left =
                                                        new JoinViaPartDetails.JoinSide
                                                        {
                                                            Column =
                                                                    tables
                                                                    .GetTableByID(previousTableId)
                                                                    .GetPrimaryKey()
                                                        },
                                                Right =
                                                        new JoinViaPartDetails.JoinSide
                                                        {
                                                            Table = relatedField.GetParentTable(),
                                                            Column = relatedField
                                                        }
                                            });
                                    }
                                }

                                joinDetails.Add(
                                    new JoinViaPartDetails
                                    {
                                        JoinType = via.TypeOfJoin,
                                        Left =
                                                new JoinViaPartDetails.JoinSide
                                                {
                                                    Column = sourceField
                                                },
                                        Right =
                                                new JoinViaPartDetails.JoinSide
                                                {
                                                    Table = targetField.GetParentTable(),
                                                    Column = targetField
                                                }
                                    });
                            }
                        }
                    }


                    break;
            }

            return true;
        }

        /// <summary>
        /// The get join by table key (base table id,destination table id).
        /// </summary>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="cJoin"/>.
        /// </returns>
        private cJoin GetJoinByTable(string ids)
        {
            cJoin reqJoin = null;

            if (!string.IsNullOrEmpty(ids))
            {
                reqJoin = GetCache(ids);
                if (reqJoin == null)
                {
                    string[] parts = ids.Split(',');
                    if (parts.Count() == 2)
                    {
                        var basetableid = new Guid(parts[0]);
                        var destinationtableid = new Guid(parts[1]);
                        reqJoin = this.CacheJoin(basetableid, destinationtableid).FirstOrDefault();
                        if (reqJoin == null)
                        {
                            cEventlog.LogEntry(
                                "Missing joins (getJoin()), basetableid: " + basetableid.ToString()
                                + ", destinationtableid: " + destinationtableid.ToString());
                        }
                    }
                    else
                    {
                        cEventlog.LogEntry("Missing joins (getJoin()), ids: " + ids);
                    }
                }
            }

            return reqJoin;
        }

        private cJoin GetCache(string ids)
        {
            cJoin reqJoin = (cJoin)Cache.Get(this.accountID, CacheKey, ids);
            return reqJoin;
        }

        /// <summary>
        /// The get join key.
        /// </summary>
        /// <param name="reqJoin">
        /// The join.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetJoinKey(cJoin reqJoin)
        {
            return this.GetJoinKey(reqJoin.basetableid, reqJoin.destinationtableid);
        }

        /// <summary>
        /// The get join key.
        /// </summary>
        /// <param name="baseTableID">
        /// The base table id.
        /// </param>
        /// <param name="destinationTableID">
        /// The destination table id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetJoinKey(Guid baseTableID, Guid destinationTableID)
        {
            return baseTableID + "," + destinationTableID;
        }

        /// <summary>
        /// The get join sql.
        /// </summary>
        /// <param name="joins">
        /// The joins.
        /// </param>
        /// <param name="jointable">
        /// The jointable.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="sysviewJoins">
        /// The sysview joins.
        /// </param>
        private void GetJoinSQL(ref List<string> joins, cJoin jointable, cField field, ref List<string> sysviewJoins)
        {
            var output = new StringBuilder();

            var clsTables = new cTables(this.accountID);
            var clsFields = new cFields(this.accountID);

            cField fldjoinkey;
            cField destinationkey;
            cTable sourcetable;
            cTable destinationtable;

            if (jointable == null)
            {
                return;
            }

            foreach (cJoinStep step in jointable.Steps.Steps.Values.OrderBy(x => x.order))
            {
                if (output.Length > 0)
                {
                    output.Remove(0, output.Length);
                }

                sourcetable = clsTables.GetTableByID(step.sourcetableid);
                destinationtable = clsTables.GetTableByID(step.destinationtableid);

                if (destinationtable.TableID != new Guid("18bbe307-1323-4743-b8af-d4f0ca22f9dc"))
                {
                    fldjoinkey = clsFields.GetFieldByID(step.joinkey);

                    // If the field is an alias table field then create the alias table join to allow multiple joins to the sourcetable
                    if (field != null && field.FieldCategory == FieldCategory.AliasTableField)
                    {
                        destinationtable = clsTables.GetTableByID(field.GetParentTable().TableID);
                        cTable fieldTable = clsTables.GetTableByID(field.TableID);

                        // Always left join as the many to one relationship box might not have a value set and an inner join will not allow the data for an entity to show in a grid 
                        output.Append(
                            " LEFT JOIN [" + destinationtable.TableName + "] AS " + fieldTable.TableName + " ON ["
                            + fieldTable.TableName + "].[" + fldjoinkey.FieldName + "] = [" + sourcetable.TableName
                            + "]");
                    }
                    else
                    {
                        switch (destinationtable.JoinType)
                        {
                            case 1:
                                output.Append(" INNER JOIN");
                                break;
                            case 2:
                                output.Append(" LEFT JOIN");
                                break;
                        }

                        output.Append(
                            " [" + destinationtable.TableName + "] ON [" + destinationtable.TableName + "].["
                            + fldjoinkey.FieldName + "] = [" + sourcetable.TableName + "]");
                    }

                    if (step.destinationkey != Guid.Empty)
                    {
                        destinationkey = clsFields.GetFieldByID(step.destinationkey);

                        output.Append(".[" + destinationkey.FieldName + "]");
                    }
                    else
                    {
                        output.Append(".[" + fldjoinkey.FieldName + "]");
                    }

                    if (sysviewJoins != null && (sourcetable.IsSystemView || destinationtable.IsSystemView))
                    {
                        if (!sysviewJoins.Contains(output.ToString().ToLower()))
                        {
                            sysviewJoins.Add(output.ToString().ToLower());
                        }
                    }
                    else
                    {
                        if (!joins.Contains(output.ToString().ToLower()))
                        {
                            joins.Add(output.ToString().ToLower());
                        }
                    }
                }
            }
        }

        #endregion
    }
}
