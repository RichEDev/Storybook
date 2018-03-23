using Infragistics.Web.UI.GridControls;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;


namespace SpendManagementLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Definitions.JoinVia;

    public class cQueryBuilder
    {
        protected string sConnectionString;
        protected string sMetabaseConnectionString;
        protected cTables clstables;
        protected cFields clsfields;
        protected int nAccountid;
        protected cTable clsBaseTable;
        public List<cQueryField> lstColumns = new List<cQueryField>();
        public Dictionary<Guid, Int32> CustomEntityListOrders = new Dictionary<Guid, Int32>();
        protected List<cQueryFilter> lstCriteria = new List<cQueryFilter>();
        protected List<cQueryFilterGroup> lstFilterGroup = new List<cQueryFilterGroup>();
        protected List<cQueryField> lstSortedColumns = new List<cQueryField>();
        protected List<cQueryFilterString> lstFilterStrings = new List<cQueryFilterString>();
        protected SelectType eSelectType = SelectType.Fields;
        protected bool bPagingActivated;
        protected int nStartRow;
        protected int nEndRow;
        protected int TopLimit;

        /// <summary>
        /// Gets or sets the where clause for the SQL query. If set all other filters will be ignored.
        /// </summary>
        public string WhereClause { get; set; }
        public cQueryBuilder()
        {

        }

        public cQueryBuilder(int accountid, string connectionstring, string metabaseconnectionstring, cTable basetable, cTables tables, cFields fields, int toplimit = 0)
        {
            sConnectionString = connectionstring;
            clstables = tables;
            clsfields = fields;
            sMetabaseConnectionString = metabaseconnectionstring;
            nAccountid = accountid;
            clsBaseTable = basetable;
            TopLimit = toplimit;
        }



        public cQueryBuilder(int accountid, string connectionstring, string metabaseconnectionstring, cTable basetable, cTables tables, cFields fields, int startrow, int endrow)
        {
            sConnectionString = connectionstring;
            clstables = tables;
            clsfields = fields;
            sMetabaseConnectionString = metabaseconnectionstring;
            nAccountid = accountid;
            clsBaseTable = basetable;
            bPagingActivated = true;
            nStartRow = startrow;
            nEndRow = endrow;

        }


        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        public string connectionstring
        {
            get { return sConnectionString; }
        }
        public string metabaseconnectionstring
        {
            get { return sMetabaseConnectionString; }
        }
        public cTable basetable
        {
            get { return clsBaseTable; }
            set { clsBaseTable = value; }
        }
        public cTables tables
        {
            get { return clstables; }
        }
        public cFields fields
        {
            get { return clsfields; }
        }

        public SelectType selectType
        {
            set { eSelectType = value; }
        }

        /// <summary>
        /// Limits the number of rows to be returned
        /// </summary>
        public int topLimit
        {
            set { TopLimit = value; }
        }


        public bool Distinct { get; set; }

        #endregion

        #region add a column
        public void addColumn(cField field)
        {
            lstColumns.Add(new cQueryField(field));
        }
        public void addColumn(cField field, string alias)
        {
            lstColumns.Add(new cQueryField(field, alias));
        }

        public void addColumn(cField field, SelectType selType)
        {
            lstColumns.Add(new cQueryField(field, selType));
        }

        public void addColumn(cField field, string alias, SelectType selType)
        {
            lstColumns.Add(new cQueryField(field, alias, selType));
        }
        public void addColumn(cField field, bool useListItemText)
        {
            cQueryField qf = new cQueryField(field);
            qf.showListItemText = useListItemText;
            lstColumns.Add(qf);
        }
        public void addColumn(cField field, string alias, bool useListItemText)
        {
            cQueryField qf = new cQueryField(field, alias);
            qf.showListItemText = useListItemText;
            lstColumns.Add(qf);
        }
        /// <summary>
        /// Used for isForeignKey type fields
        /// </summary>
        /// <param name="field">the field to display</param>
        /// <param name="joinVia">how to get there from the basetable</param>
        /// <param name="alias">optional column name to override default</param>
        public void addColumn(cField field, JoinVia joinVia, string alias = "")
        {
            cQueryField newField = null;
            newField = joinVia == null ? new cQueryField(field) : new cQueryField(field, joinVia, alias: alias);
            this.lstColumns.Add(newField);
        }

        public void addSortableColumn(cField field, SortDirection sortdirection)
        {
            lstSortedColumns.Add(new cQueryField(field, sortdirection));
        }

        public void addSortableColumn(cField field, SortDirection sortdirection, string alias)
        {
            lstSortedColumns.Add(new cQueryField(field, sortdirection, alias));
        }

        public void addSortableColumn(cField field, SortDirection sortdirection, JoinVia joinVia)
        {
            lstSortedColumns.Add(new cQueryField(field, joinVia, sortdirection));
        }

        public void addStaticColumn(string staticValue, string static_fieldname)
        {
            lstColumns.Add(new cQueryField(staticValue, static_fieldname));
        }

        //public void addColumn(string tablename, string fieldname)
        //{
        //    lstColumns.Add("[" + tablename + "].[" + fieldname + "]");
        //}

        //public void addColumn(string tablename, string fieldname, string alias)
        //{
        //    lstColumns.Add("[" + tablename + "].[" + fieldname + "] AS [" + alias + "]");
        //}

        #endregion

        #region filters
        public void addFilter(cField field, ConditionType condition, object[] value1, object[] value2, ConditionJoiner joiner, JoinVia joinVia)
        {
            List<object> lstValue1 = new List<object>();
            List<object> lstValue2 = new List<object>();

            if (value1 != null)
            {
                lstValue1 = (from o in value1
                             select o).ToList();

                //    foreach (object o in value1)
                //    {
                //        lstValue1.Add(o);
                //    }
            }
            if (value2 != null)
            {
                lstValue2 = (from o in value2
                             select o).ToList();
                //    foreach (object o in value2)
                //    {
                //        lstValue2.Add(o);
                //    }
            }
            cQueryFilter filter = new cQueryFilter(field, condition, lstValue1, lstValue2, joiner, joinVia);
            lstCriteria.Add(filter);
        }

        public void addFilter(cQueryFilter filter)
        {
            lstCriteria.Add(filter);
        }

        public void addFilterGroup(cQueryFilterGroup filterGroup)
        {
            lstFilterGroup.Add(filterGroup);
        }

        public void addFilterString(cQueryFilterString filter)
        {
            lstFilterStrings.Add(filter);
        }

        /// <summary>
        /// Set the criteria parameters for the current query.
        /// </summary>
        /// <param name="expdata">
        /// The current instance of <see cref="DBConnection"/>
        /// </param>
        public void setCriteriaParameters(ref DBConnection expdata)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@salt", "2FD583C9-BF7E-4B4E-B6E6-5FC9375AD069");
            if (!string.IsNullOrEmpty(WhereClause))
            {
                foreach (cQueryFilter filter in lstCriteria)
                {
                    expdata.sqlexecute.Parameters.AddWithValue(filter.parameterName, filter.parameterValue);
                }
            }
            else
            {
                int filterNumber = 0;

                foreach (cQueryFilterGroup filterGroup in lstFilterGroup)
                {
                    foreach (cQueryFilter filter in filterGroup.QueryFilters)
                    {
                        CreateParametersForFilter(ref expdata, filter, filterNumber);
                        filterNumber++;
                    }
                }

                foreach (cQueryFilter filter in lstCriteria)
                {
                    CreateParametersForFilter(ref expdata, filter, filterNumber);
                    filterNumber++;
                }
            }
        }


        /// <summary>
        /// Create SQL Parameters for the given cQueryFilter
        /// </summary>
        /// <param name="expdata"></param>
        /// <param name="filter"></param>
        /// <param name="indexOfFilterInList"></param>
        private void CreateParametersForFilter(ref DBConnection expdata, cQueryFilter filter, int indexOfFilterInList)
        {
            DateTime[] tempdates;

            if (filter.field.GenList || filter.field.ValueList)
            {
                if (filter.condition != ConditionType.ContainsData && filter.condition != ConditionType.DoesNotContainData)
                {
                    if (filter.condition == ConditionType.Like || filter.condition == ConditionType.NotLike)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, filter.value1[0]);
                    }
                    else
                    {
                        foreach (object x in filter.value1)
                        {
                            var filterName = "@filter1_" + indexOfFilterInList + "_" +
                            filter.value1.IndexOf(x);
                            if (x.GetType() == typeof(List<SqlDataRecord>))
                            {
                                expdata.sqlexecute.Parameters.Add(
                                    filterName,
                                    SqlDbType.Structured);
                                expdata.sqlexecute.Parameters[filterName].SqlDbType = SqlDbType.Structured;
                                expdata.sqlexecute.Parameters[filterName].DbType = DbType.Object;
                                expdata.sqlexecute.Parameters[filterName].Value = x;
                                expdata.sqlexecute.Parameters[filterName].TypeName = "dbo.LineManagers";
                                expdata.sqlexecute.Parameters[filterName].Direction = ParameterDirection.Input;
                            }
                            else
                            {
                                expdata.sqlexecute.Parameters.AddWithValue(filterName, x);
                            }
                        }
                    }
                }
            }
            else
            {
                DateTime startdate, enddate;
                switch (filter.condition)
                {
                    case ConditionType.WithAccessRoles:
                    case ConditionType.AtMyHierarchy:
                    case ConditionType.AtMyClaimsHierarchy:
                    case ConditionType.AtMyCostCodeHierarchy:
                        for (int i = 0; i < filter.value1.Count; i++)
                        {
                            List<SqlDataRecord> records = null;
                            bool objectType = false;
                            object x = filter.value1[i];
                            var filterName = "@filter1_" + indexOfFilterInList + "_" + filter.value1.IndexOf(x);
                            if (x == null || x.GetType() == typeof(List<SqlDataRecord>) || x.GetType() == typeof(object[]))
                            {
                                if (x != null)
                                {
                                    records = new List<SqlDataRecord>();
                                    if (x.GetType() == typeof(object[]))
                                    {
                                        // employee ids have arrived in a List<int> as SqlDataRecord does not serialise to javascript.
                                        objectType = true;
                                        var employeeIds = ((object[])x).Cast<int>().ToList();
                                        SqlMetaData[] metaData = { new SqlMetaData("employeeid", SqlDbType.Int) };

                                        foreach (int employeeId in employeeIds.Distinct())
                                        {
                                            var row = new SqlDataRecord(metaData);
                                            row.SetInt32(0, employeeId);
                                            records.Add(row);
                                        }
                                    }
                                }

                                expdata.sqlexecute.Parameters.Add(filterName, SqlDbType.Structured);
                                expdata.sqlexecute.Parameters[filterName].DbType = DbType.Object;
                                expdata.sqlexecute.Parameters[filterName].Value = objectType ? records : x;
                                expdata.sqlexecute.Parameters[filterName].TypeName = "dbo.LineManagers";
                                expdata.sqlexecute.Parameters[filterName].Direction = ParameterDirection.Input;
                            }
                            else
                            {
                                expdata.sqlexecute.Parameters.AddWithValue(filterName, x);
                            }

                            if (objectType)
                            {
                                filter.value1[i] = records;
                            }
                        }

                        break;
                    case ConditionType.Equals:
                    case ConditionType.DoesNotEqual:
                        if (filter.value1.Count == 1)
                        {
                            expdata.sqlexecute.Parameters.AddWithValue(
                                "@filter1_" + indexOfFilterInList + "_0",
                                filter.value1[0]);
                        }
                        else
                        {
                            foreach (object x in filter.value1)
                            {
                                expdata.sqlexecute.Parameters.AddWithValue(
                                    "@filter1_" + indexOfFilterInList + "_" + filter.value1.IndexOf(x),
                                    x);
                            }
                        }
                        break;
                    case ConditionType.On:
                    case ConditionType.NotOn:
                    case ConditionType.GreaterThan:
                    case ConditionType.After:
                    case ConditionType.LessThan:
                    case ConditionType.Before:
                    case ConditionType.GreaterThanEqualTo:
                    case ConditionType.OnOrAfter:
                    case ConditionType.LessThanEqualTo:
                    case ConditionType.OnOrBefore:
                    case ConditionType.Like:
                    case ConditionType.NotLike:
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, filter.value1[0]);
                        break;
                    case ConditionType.Between:
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, filter.value1[0]);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, filter.value2[0]);
                        break;
                    case ConditionType.Today:
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, DateTime.Today);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, DateTime.Today.AddDays(1).AddSeconds(-1));
                        break;
                    case ConditionType.OnOrBeforeToday:
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, DateTime.Today);
                        break;
                    case ConditionType.OnOrAfterToday:
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, DateTime.Today);
                        break;
                    case ConditionType.Tomorrow:
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, DateTime.Today.AddDays(1));
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, DateTime.Today.AddDays(2).AddSeconds(-1));
                        break;
                    case ConditionType.Yesterday:
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, DateTime.Today.AddDays(-1));
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, DateTime.Today.AddSeconds(-1));
                        break;
                    case ConditionType.ThisYear:
                        startdate = new DateTime(DateTime.Today.Year, 01, 01);
                        enddate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.NextYear:
                        startdate = new DateTime(DateTime.Today.Year + 1, 01, 01);
                        enddate = new DateTime(DateTime.Today.Year + 1, 12, 31, 23, 59, 59);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.LastYear:
                        startdate = new DateTime(DateTime.Today.Year - 1, 01, 01);
                        enddate = new DateTime(DateTime.Today.Year - 1, 12, 31, 23, 59, 59);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.LastXYears:
                        startdate = new DateTime(DateTime.Today.Year - (int)filter.value1[0], 01, 01);
                        enddate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.ThisTaxYear:
                        if (DateTime.Today.Month >= 4)
                        {
                            startdate = new DateTime(DateTime.Today.Year, 04, 06);
                            enddate = new DateTime(DateTime.Today.Year + 1, 04, 05, 23, 59, 59);
                        }
                        else
                        {
                            startdate = new DateTime(DateTime.Today.Year - 1, 04, 06);
                            enddate = new DateTime(DateTime.Today.Year, 04, 05, 23, 59, 59);
                        }
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.ThisFinancialYear:
                        //comment to force merge
                        tempdates = getFinancialYear();
                        startdate = tempdates[0];
                        enddate = tempdates[1];
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.LastTaxYear:
                        if (DateTime.Today.Month >= 4)
                        {
                            startdate = new DateTime(DateTime.Today.Year - 1, 04, 06);
                            enddate = new DateTime(DateTime.Today.Year, 04, 05, 23, 59, 59);
                        }
                        else
                        {
                            startdate = new DateTime(DateTime.Today.Year - 2, 04, 06);
                            enddate = new DateTime(DateTime.Today.Year - 1, 04, 05, 23, 59, 59);
                        }
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.LastFinancialYear:
                        tempdates = getFinancialYear();
                        startdate = tempdates[0];
                        enddate = tempdates[1];
                        startdate = startdate.AddYears(-1);
                        enddate = enddate.AddYears(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.NextTaxYear:
                        if (DateTime.Today.Month >= 4)
                        {
                            startdate = new DateTime(DateTime.Today.Year + 1, 04, 06);
                            enddate = new DateTime(DateTime.Today.Year + 2, 04, 05, 23, 59, 59);
                        }
                        else
                        {
                            startdate = new DateTime(DateTime.Today.Year, 04, 06);
                            enddate = new DateTime(DateTime.Today.Year + 1, 04, 05, 23, 59, 59);
                        }
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.NextFinancialYear:
                        tempdates = getFinancialYear();
                        startdate = tempdates[0];
                        enddate = tempdates[1];
                        startdate = startdate.AddYears(1);
                        enddate = enddate.AddYears(1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.NextXYears:
                        startdate = new DateTime(DateTime.Today.Year, 01, 01);
                        enddate = new DateTime(DateTime.Today.Year + (int)filter.value1[0], 12, 31, 23, 59, 59);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.ThisMonth:
                        startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                        enddate = new DateTime(startdate.Year, startdate.Month, DateTime.DaysInMonth(startdate.Year, startdate.Month), 23, 59, 59);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.LastMonth:
                        startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                        startdate = startdate.AddMonths(-1);
                        enddate = new DateTime(startdate.Year, startdate.Month, DateTime.DaysInMonth(startdate.Year, startdate.Month), 23, 59, 59);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.NextMonth:
                        startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                        startdate = startdate.AddMonths(1);
                        enddate = new DateTime(startdate.Year, startdate.Month, DateTime.DaysInMonth(startdate.Year, startdate.Month), 23, 59, 59);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.LastXMonths:
                        startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                        startdate = startdate.AddMonths((int)filter.value1[0] / -1);
                        enddate = DateTime.Today.AddDays(1).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.NextXMonths:
                        startdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                        enddate = startdate.AddMonths((int)filter.value1[0]).AddDays(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.Last7Days:
                        startdate = DateTime.Today.AddDays(-7);
                        enddate = DateTime.Today.AddDays(1).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.LastWeek:
                        startdate = getStartOfWeek();
                        startdate = startdate.AddDays(-7);
                        enddate = startdate.AddDays(7).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.LastXDays:
                        startdate = DateTime.Today.AddDays(((int)filter.value1[0] / -1));
                        enddate = DateTime.Today.AddDays(1).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.LastXWeeks:
                        startdate = DateTime.Today;
                        startdate = startdate.AddDays(-7 * (int)filter.value1[0]);
                        enddate = DateTime.Today.AddDays(1).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.Next7Days:
                        startdate = DateTime.Today;
                        enddate = DateTime.Today.AddDays(8).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.NextWeek:
                        startdate = getStartOfWeek().AddDays(7);
                        enddate = startdate.AddDays(7).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.NextXDays:
                        startdate = DateTime.Today;
                        enddate = DateTime.Today.AddDays((int)filter.value1[0] + 1).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.NextXWeeks:
                        startdate = DateTime.Today;
                        enddate = startdate.AddDays((7 * (int)filter.value1[0]) + 1).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                    case ConditionType.ThisWeek:
                        startdate = getStartOfWeek();
                        enddate = startdate.AddDays(7).AddSeconds(-1);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter1_" + indexOfFilterInList, startdate);
                        expdata.sqlexecute.Parameters.AddWithValue("@filter2_" + indexOfFilterInList, enddate);
                        break;
                }
            }
        }


        private DateTime[] getFinancialYear()
        {
            DBConnection expdata = new DBConnection(connectionstring);
            string start = "06/04";
            string end = "05/04";

            SqlDataReader reader;
            string strsql = "select yearstart, yearend from FinancialYears where [Primary] = 1";

            using (reader = expdata.GetReader(strsql))
            {
                #region ordinals
                int fystart_ord = reader.GetOrdinal("yearstart");
                int fyend_ord = reader.GetOrdinal("yearend");
                #endregion

                while (reader.Read())
                {
                    if (!reader.IsDBNull(fystart_ord))
                    {
                        start = reader.GetString(fystart_ord);
                    }
                    if (!reader.IsDBNull(fyend_ord))
                    {
                        end = reader.GetString(fyend_ord);
                    }
                }
                reader.Close();
            }
            string[] startitems = start.Split('/');
            string[] enditems = end.Split('/');

            DateTime financialyearstart = new DateTime(DateTime.Today.Year, int.Parse(startitems[1]), int.Parse(startitems[0]));
            DateTime financialyearend = new DateTime(DateTime.Today.AddYears(1).Year, int.Parse(enditems[1]), int.Parse(enditems[0]), 23, 59, 59);
            if (int.Parse(enditems[1]) > int.Parse(startitems[1]))
            {
                financialyearend = financialyearend.AddYears(-1);
            }
            return new DateTime[] { financialyearstart, financialyearend };
        }
        private DateTime getStartOfWeek()
        {
            DateTime date = DateTime.Today;

            while (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(-1);
            }

            return date;
        }
        #endregion
        public void removeColumn(cField field)
        {
            lstColumns = (from x in lstColumns
                          where x.field != field
                          select x).ToList();

        }

        private string generateSelect()
        {
            StringBuilder output = new StringBuilder();
            StringBuilder extrafields = new StringBuilder();
            StringBuilder extrafield = new StringBuilder();

            output.Append("SELECT ");
            string fieldname;

            if (Distinct)
            {
                output.Append(" DISTINCT ");
            }

            if (TopLimit > 0)
            {
                output.Append("TOP " + TopLimit + " ");
            }

            if (bPagingActivated)
            {
                foreach (cQueryField queryField in lstColumns)
                {
                    if (!string.IsNullOrEmpty(queryField.alias))
                    {
                        fieldname = "[" + queryField.alias + "_sorted" + "]";
                    }
                    else
                    {
                        fieldname = "[" + queryField.field.GetParentTable().TableName + "].[" + queryField.field.FieldName + "]";
                    }

                    switch (queryField.selectType)
                    {
                        case SelectType.Count:
                            output.Append("COUNT(" + fieldname + ")");
                            break;
                        case SelectType.Distinct:
                            output.Append("DISTINCT(" + fieldname + ")");
                            break;
                        case SelectType.Max:
                            output.Append("MAX(" + fieldname + ")");
                            break;
                        case SelectType.Min:
                            output.Append("MIN(" + fieldname + ")");
                            break;
                        case SelectType.Sum:
                            output.Append("SUM(" + fieldname + ")");
                            break;
                        default:
                            if (queryField.isStaticColumn)
                            {
                                output.Append("'" + queryField.staticValue + "'");
                            }
                            else
                            {
                                if (queryField.field.FieldType == "RW")
                                {
                                    output.Append("null");
                                }
                                else if (queryField.field.FieldType == "FD")
                                {
                                    output.Append(fieldname);
                                }
                                else
                                {
                                    if ((queryField.field.ValueList || queryField.field.GenList) && queryField.field.ListItems.Count > 0)
                                    {
                                        // construct CASE statement to provide sorting capability or text values for datasets
                                        string fieldsuffix = "";
                                        if (!queryField.showListItemText)
                                        {
                                            // output normal field in addition to valuelist text as may be referenced
                                            fieldsuffix = "_text";

                                            output.Append("[" + queryField.alias + "_sorted]");
                                            output.Append(", ");
                                        }

                                        extrafield.Append("[" + queryField.field.FieldName + fieldsuffix + "]");
                                        extrafield.Append(" = CASE ");
                                        foreach (object s in queryField.field.ListItems.Keys)
                                        {
                                            if (queryField.field.FieldName.Contains("dbo."))
                                            {
                                                extrafield.Append("WHEN [" + queryField.field.FieldName + "] = " + s + " THEN '" + queryField.field.ListItems[s].Replace("'", "''") + "' ");
                                            }
                                            else
                                            {
                                                extrafield.Append("WHEN " + fieldname + " = " + s + " THEN '" + queryField.field.ListItems[s].Replace("'", "''") + "' ");
                                            }
                                        }
                                        extrafield.Append("ELSE 'Unknown' ");
                                        extrafield.Append("END");

                                        if (queryField.showListItemText)
                                        {
                                            // output extra field now rather than at end of query
                                            output.Append(extrafield.ToString());
                                        }
                                        else
                                        {
                                            output.Remove(output.Length - 2, 2);
                                            extrafields.Append(extrafield + ", ");
                                        }
                                        extrafield.Remove(0, extrafield.Length);
                                    }
                                    else
                                    {
                                        if (queryField.field.GenList && queryField.field.FieldSource == cField.FieldSourceType.CustomEntity)
                                        {
                                            // dynamically provide join using lookup table and lookupfield
                                            cField relatedKeyField = queryField.field.GetLookupTable().GetKeyField();
                                            extrafields.Append("[" + relatedKeyField.FieldName + "_text], ");
                                        }
                                        else if (queryField.field.ValueList && queryField.field.FieldSource == cField.FieldSourceType.CustomEntity)
                                        {
                                            extrafields.Append("[item_text]");
                                        }
                                        output.Append(fieldname);
                                    }
                                }
                            }
                            break;
                    }

                    if (!string.IsNullOrEmpty(queryField.alias))
                    {
                        output.Append(" AS [" + queryField.alias + "]");
                    }
                    output.Append(", ");
                }
                if (extrafields.Length > 0)
                {
                    output.Append(extrafields.ToString());
                    extrafields.Remove(0, extrafields.Length);
                }
                if (lstColumns.Count > 0)
                {
                    output.Remove(output.Length - 2, 2);
                }
            }
            else
            {
                foreach (cQueryField queryField in lstColumns)
                {
                    if (!queryField.isStaticColumn)
                    {
                        if (queryField.JoinVia != null)
                        {
                            fieldname = "[" + queryField.JoinVia.TableAlias + "].[" + queryField.field.FieldName + "]";
                        }
                        else
                        {
                            fieldname = "[" + queryField.field.GetParentTable().TableName + "].[" + queryField.field.FieldName + "]";
                        }

                        if (queryField.field.Encrypted)
                        {
                            fieldname = $"CAST(DECRYPTBYPASSPHRASE(@salt, {fieldname} ) AS NVARCHAR(Max))";
                        }
                    }
                    else
                    {
                        fieldname = "";
                    }

                    switch (queryField.selectType)
                    {
                        case SelectType.Count:
                            output.Append("COUNT(" + fieldname + ")");
                            break;
                        case SelectType.Distinct:
                            output.Append("DISTINCT(" + fieldname + ")");
                            break;
                        case SelectType.Max:
                            output.Append("MAX(" + fieldname + ")");
                            break;
                        case SelectType.Min:
                            output.Append("MIN(" + fieldname + ")");
                            break;
                        case SelectType.Sum:
                            output.Append("SUM(" + fieldname + ")");
                            break;
                        default:
                            if (queryField.isStaticColumn)
                            {
                                output.Append("'" + queryField.staticValue + "'");
                            }
                            else
                            {
                                if (queryField.field.FieldType == "RW")
                                {
                                    output.Append("null");
                                }
                                else if (queryField.field.FieldType == "FD")
                                {
                                    output.Append(queryField.field.FieldName);
                                }
                                else
                                {
                                    if ((queryField.field.ValueList || queryField.field.GenList) && queryField.field.ListItems.Count > 0)
                                    {
                                        // construct CASE statement to provide sorting capability or text values for datasets
                                        string fieldsuffix = "";
                                        if (!queryField.showListItemText)
                                        {
                                            // output normal field in addition to valuelist text as may be referenced
                                            fieldsuffix = "_text";

                                            if (queryField.JoinVia != null)
                                            {
                                                output.Append("[" + queryField.JoinVia.TableAlias + "].[" + queryField.field.FieldName + "]");
                                            }
                                            else
                                            {
                                                output.Append("[" + queryField.field.FieldName + "]");
                                                // Perhaps add "[" + queryField.field.ParentTable.TableName + "]."
                                            }

                                            if (!string.IsNullOrEmpty(queryField.alias))
                                            {
                                                output.Append(" AS [" + queryField.alias + "]");
                                            }
                                            output.Append(", ");
                                        }

                                        if (!queryField.showListItemText && queryField.JoinVia != null && !string.IsNullOrEmpty(queryField.alias))
                                        {
                                            extrafield.Append("[" + queryField.alias + fieldsuffix + "]");
                                        }
                                        else
                                        {
                                            extrafield.Append("[" + queryField.field.FieldName + fieldsuffix + "]");
                                        }

                                        extrafield.Append(" = CASE ");
                                        foreach (object s in queryField.field.ListItems.Keys)
                                        {
                                            if (queryField.field.FieldName.Contains("dbo."))
                                            {
                                                extrafield.Append("WHEN [" + queryField.field.FieldName + "] = " + s + " THEN '" + queryField.field.ListItems[s].Replace("'", "''") + "' ");
                                            }
                                            else
                                            {
                                                extrafield.Append("WHEN " + fieldname + " = " + s + " THEN '" + queryField.field.ListItems[s].Replace("'", "''") + "' ");
                                            }
                                        }
                                        extrafield.Append("ELSE 'Unknown' ");
                                        extrafield.Append("END");

                                        if (queryField.showListItemText)
                                        {
                                            // output extra field now rather than at end of query
                                            output.Append(extrafield.ToString());
                                        }
                                        else
                                        {
                                            output.Remove(output.Length - 2, 2);
                                            extrafields.Append(extrafield + ", ");
                                        }
                                        extrafield.Remove(0, extrafield.Length);
                                    }
                                    else
                                    {
                                        if (queryField.field.GenList && queryField.field.FieldSource == cField.FieldSourceType.CustomEntity)
                                        {
                                            // dynamically provide join using lookup table and lookupfield
                                            cField relatedKeyField = fields.GetFieldByID(queryField.field.GetLookupTable().KeyFieldID);
                                            output.Append("[" + queryField.field.GetParentTable().TableName + "].[" + queryField.field.FieldName + "]");

                                            extrafields.Append("[" + queryField.field.GetLookupTable().TableName + "].[" + relatedKeyField.FieldName + "] AS [" + relatedKeyField.FieldName + "_text], ");
                                        }
                                        else
                                        {
                                            if (queryField.field.FieldName.Contains("dbo."))
                                            {
                                                string tmpFieldName = queryField.field.FieldName;

                                                if (queryField.JoinVia != null)
                                                {
                                                    tmpFieldName = cReport.ReplaceTableNameWithJoinViaAlias(queryField, tmpFieldName);
                                                }

                                                output.Append(tmpFieldName);
                                            }
                                            else
                                            {
                                                output.Append("" + fieldname + "");
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    if (!queryField.isStaticColumn)
                    {
                        if (queryField.JoinVia == null)
                        {
                            if (!string.IsNullOrEmpty(queryField.alias) && !(queryField.field.ValueList))
                            {
                                output.Append(" AS [" + queryField.alias + "]");
                            }
                        }
                        else if (!(queryField.field.ValueList))
                        {
                            output.Append(" AS [" + queryField.alias + "]");
                        }
                    }
                    output.Append(", ");
                }
                if (extrafields.Length > 0)
                {
                    output.Append(extrafields.ToString());
                    extrafields.Remove(0, extrafields.Length);
                }
                if (lstColumns.Count > 0)
                {
                    output.Remove(output.Length - 2, 2);
                }
            }
            if (bPagingActivated)
            {
                foreach (cQueryField field in lstColumns)  // what are we actually doing here?     !!!!!!!!
                {
                    output.Replace("[" + field.field.GetParentTable().TableName + "].", "");
                }
                output.Append(" FROM (SELECT ROW_NUMBER() OVER (ORDER BY ");
                if (lstSortedColumns[0].field.GenList && lstSortedColumns[0].field.FieldSource == cField.FieldSourceType.CustomEntity)   // what's going to happen to genlists? obsolete?    !!!!!!
                {
                    cField relatedKeyField = this.lstSortedColumns[0].field.GetLookupTable().GetKeyField();
                    output.Append("[").Append(this.lstSortedColumns[0].field.GetLookupTable().TableName).Append("].[").Append(relatedKeyField.FieldName).Append("] ");
                }
                else
                {
                    var fieldName = string.Empty;
                    if (lstSortedColumns[0].JoinVia != null)
                    {
                        if (lstSortedColumns[0].field.FieldName.Contains("dbo."))
                        {
                            fieldName = this.lstSortedColumns[0].field.FieldName;
                            fieldName = cReport.ReplaceTableNameWithJoinViaAlias(this.lstSortedColumns[0], fieldName);
                        }
                        else
                        {
                            fieldName = $"[{this.lstSortedColumns[0].JoinVia.TableAlias}].[{this.lstSortedColumns[0].field.FieldName}] ";
                        }
                    }
                    else
                    {
                        switch (this.lstSortedColumns[0].field.FieldType)
                        {
                            case "FD":
                            case "FC":
                            case "FI":
                            case "FS":
                            case "FU":
                                fieldName = this.lstSortedColumns[0].field.FieldName;
                                
                                break;
                            default:
                                fieldName = $"[{this.lstSortedColumns[0].field.GetParentTable().TableName}].[{this.lstSortedColumns[0].field.FieldName}] ";
                                
                                break;
                        }
                    }

                    if (this.lstSortedColumns[0].field.Encrypted)
                    {
                        fieldName = $"CAST(DECRYPTBYPASSPHRASE(@salt, {fieldName} ) AS NVARCHAR(Max)) ";
                    }

                    output.Append(fieldName);
                }


                if (lstSortedColumns[0].SortDirection == SortDirection.Ascending)
                {
                    output.Append("ASC");
                }
                else
                {
                    output.Append("DESC");
                }

                output.Append(") ");

                output.Append(" AS row, ");
                foreach (cQueryField queryField in lstColumns)
                {
                    if (queryField.JoinVia != null)
                    {
                        fieldname = "[" + queryField.JoinVia.TableAlias + "].[" + queryField.field.FieldName + "]";
                    }
                    else
                    {
                        fieldname = "[" + queryField.field.GetParentTable().TableName + "].[" + queryField.field.FieldName + "]";
                    }

                    if (queryField.field.Encrypted)
                    {
                        fieldname = $"CAST(DECRYPTBYPASSPHRASE(@salt, {fieldname} ) AS NVARCHAR(Max))";
                    }

                    switch (queryField.selectType)
                    {
                        case SelectType.Count:
                            output.Append("COUNT(" + fieldname + ")");
                            break;
                        case SelectType.Distinct:
                            output.Append("DISTINCT(" + fieldname + ")");
                            break;
                        case SelectType.Max:
                            output.Append("MAX(" + fieldname + ")");
                            break;
                        case SelectType.Min:
                            output.Append("MIN(" + fieldname + ")");
                            break;
                        case SelectType.Sum:
                            output.Append("SUM(" + fieldname + ")");
                            break;
                        default:
                            if (queryField.isStaticColumn)
                            {
                                output.Append("'" + queryField.staticValue + "'");
                            }
                            else
                            {
                                if (queryField.field.FieldType == "RW")
                                {
                                    output.Append("null");
                                }
                                else
                                {
                                    if (queryField.field.GenList && queryField.field.FieldSource == cField.FieldSourceType.CustomEntity)
                                    {
                                        // dynamically provide join using lookup table and lookupfield
                                        cField relatedKeyField = queryField.field.GetLookupTable().GetKeyField();
                                        output.Append("[" + queryField.field.GetParentTable().TableName + "].[" + queryField.field.FieldName + "]"); // does this need to be fieldname (as all the others of this combo have been swapped out to) or does it not need the via treatment?

                                        extrafields.Append("[" + queryField.field.GetLookupTable().TableName + "].[" + relatedKeyField.FieldName + "] AS [" + relatedKeyField.FieldName + "_text], ");  // why extra fields and not a straight replacement?   !!!!!!!!
                                    }
                                    else
                                    {
                                        if (queryField.field.FieldName.Contains("dbo."))
                                        {
                                            string tmpFieldName = queryField.field.FieldName;

                                            if (queryField.JoinVia != null)
                                            {
                                                tmpFieldName = cReport.ReplaceTableNameWithJoinViaAlias(queryField,
                                                    tmpFieldName);
                                            }

                                            output.Append(tmpFieldName);
                                        }
                                        else
                                        {
                                            output.Append("" + fieldname + "");
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    if (!string.IsNullOrEmpty(queryField.alias))
                    {
                        output.Append(" AS [" + queryField.alias + "_sorted]");
                    }
                    output.Append(", ");
                }
                if (extrafields.Length > 0)
                {
                    output.Append(extrafields.ToString());
                }
                if (lstColumns.Count > 0)
                {
                    output.Remove(output.Length - 2, 2);
                }

                SortedList<Guid, JoinVia> lstVia = new SortedList<Guid, JoinVia>();

                output.Append(" FROM [dbo].[" + clsBaseTable.TableName + "]");
                cJoins clsjoins = new cJoins(accountid);
                SetCustomEntityListFieldOrders();
                output.Append(" " + clsjoins.createJoinSQL(getDistinctFields(ref lstVia), basetable.TableID, lstVia, CustomEntityListOrders));
                output.Append(" " + generateWhere(true));
                output.Append(") AS [" + clsBaseTable.TableName + "_sorted] ");
            }
            else
            {
                output.Append(" FROM [" + clsBaseTable.TableName + "]");
            }
            return output.ToString();
        }

        private void SetCustomEntityListFieldOrders()
        {
            if (CustomEntityListOrders.Count == 0)
            {
                int order = 0;
                for (int i = 0; i < lstColumns.Count; i++)
                {
                    if (!lstColumns[i].isStaticColumn && lstColumns[i].field.ValueList && (lstColumns[i].field.FieldSource == cField.FieldSourceType.CustomEntity || lstColumns[i].field.FieldSource == cField.FieldSourceType.Userdefined) && !CustomEntityListOrders.ContainsKey(lstColumns[i].field.FieldID))
                    {
                        CustomEntityListOrders.Add(lstColumns[i].field.FieldID, order++);
                    }
                }
                if (string.IsNullOrEmpty(WhereClause))
                {
                    for (int i = 0; i < lstCriteria.Count; i++)
                    {
                        if (lstCriteria[i].field.ValueList && (lstCriteria[i].field.FieldSource == cField.FieldSourceType.CustomEntity || lstCriteria[i].field.FieldSource == cField.FieldSourceType.Userdefined))
                        {
                            if (!CustomEntityListOrders.ContainsKey(lstCriteria[i].field.FieldID))
                            {
                                CustomEntityListOrders.Add(lstCriteria[i].field.FieldID, order++);
                            }
                        }
                    }
                }
            }
        }

        protected string generateWhere(bool subQuery)
        {
            bool filtersDoNotExist = lstCriteria.Count == 0 && lstFilterStrings.Count == 0 &&
                (lstFilterGroup.Count == 0 || lstFilterGroup.Any(x => x.QueryFilters.Count > 0) == false);

            if (filtersDoNotExist && !bPagingActivated && string.IsNullOrEmpty(WhereClause))
            {
                return "";
            }

            StringBuilder output = new StringBuilder();

            if (!bPagingActivated || (bPagingActivated && subQuery))
            {
                if (filtersDoNotExist && string.IsNullOrEmpty(WhereClause))
                {
                    return "";
                }

                if (!string.IsNullOrEmpty(WhereClause))
                {
                    output.Append("WHERE " + WhereClause);
                }
                else
                {


                    output.Append("WHERE (");

                    int filterNumber = 0;

                    foreach (cQueryFilterGroup filterGroup in lstFilterGroup)
                    {
                        output.Append(" (");

                        if (lstFilterGroup.IndexOf(filterGroup) > 0)
                        {
                            switch (filterGroup.Joiner)
                            {
                                case ConditionJoiner.And:
                                    output.Append(" AND (");
                                    break;
                                case ConditionJoiner.Or:
                                    output.Append(" OR (");
                                    break;
                            }
                        }

                        foreach (cQueryFilter filter in filterGroup.QueryFilters)
                        {
                            output.Append(CreateWhereClauseForFilter(filter, subQuery, filterNumber));
                            filterNumber++;
                        }

                        output.Append(")");
                    }

                    foreach (cQueryFilter filter in lstCriteria)
                    {
                        output.Append(CreateWhereClauseForFilter(filter, subQuery, filterNumber));
                        filterNumber++;
                    }
                }
            }
            else
            {
                output.Append("WHERE ");
                output.Append("row >= " + nStartRow + " and row <= " + nEndRow);
                if (lstFilterStrings.Count > 0)
                {
                    output.Append(" AND ");
                }
            }

            if (lstFilterStrings.Count > 0)
            {
                foreach (cQueryFilterString filter in lstFilterStrings)
                {
                    if (lstFilterStrings.IndexOf(filter) > 0 || (lstCriteria.Count > 0 && (!output.ToString().EndsWith(" AND ") && !output.ToString().EndsWith(" OR "))))
                    {
                        switch (filter.joiner)
                        {
                            case ConditionJoiner.And:
                                output.Append(" AND ");
                                break;
                            case ConditionJoiner.Or:
                                output.Append(" OR ");
                                break;
                        }
                    }

                    output.Append(filter.filterString);
                }

            }

            return output.ToString();
        }


        /// <summary>
        /// Creates the SQL WHERE clause for a cQueryFilter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="subQuery"></param>
        /// <param name="indexOfFilterInList"></param>
        /// <returns></returns>
        private string CreateWhereClauseForFilter(cQueryFilter filter, bool subQuery, int indexOfFilterInList)
        {
            StringBuilder output = new StringBuilder();
            string tableName = string.Empty;
            string fieldName = string.Empty;

            if (indexOfFilterInList > 0)
            {
                switch (filter.joiner)
                {
                    case ConditionJoiner.And:
                        output.Append(" AND (");
                        break;
                    case ConditionJoiner.Or:
                        output.Append(" OR (");
                        break;
                }
            }

            if ((filter.field.ValueList || filter.field.GenList))
            {
                if (!bPagingActivated || (bPagingActivated && subQuery))
                {
                    if (filter.JoinVia != null)
                    {
                        tableName = String.Format("[{0}].", filter.JoinVia.TableAlias);
                    }
                    else
                    {
                        if (filter.field.ValueList && filter.field.FieldSource == cField.FieldSourceType.CustomEntity)
                        {
                            int order = 0;
                            CustomEntityListOrders.TryGetValue(filter.field.FieldID, out order);
                            tableName = String.Format("[customEntityAttributeListItems_{0}].", order);
                        }
                        else if (filter.field.ValueList && filter.field.FieldSource == cField.FieldSourceType.Userdefined)
                        {
                            int order = 0;
                            CustomEntityListOrders.TryGetValue(filter.field.FieldID, out order);
                            tableName = String.Format("[userdefined_list_items_{0}].", order);
                        }
                        else
                        {
                            tableName = String.Format("[{0}].", filter.field.GetParentTable().TableName);
                        }
                    }
                    output.Append(tableName);
                }

                switch (filter.condition)
                {
                    case ConditionType.Like:
                    case ConditionType.NotLike:
                        fieldName = filter.field.ValueList && (filter.field.FieldSource == cField.FieldSourceType.CustomEntity || filter.field.FieldSource == cField.FieldSourceType.Userdefined)
                                        ? "[item] "
                                        : String.Format("[{0}]", filter.field.FieldName);
                        break;
                    case ConditionType.WithAccessRoles:
                    case ConditionType.AtMyHierarchy:
                    case ConditionType.AtMyClaimsHierarchy:
                    case ConditionType.AtMyCostCodeHierarchy:
                        fieldName = String.Format("[{0}] ", filter.field.GetParentTable().GetPrimaryKey().FieldName);
                        break;
                    default:
                        if (filter.field.ValueList)
                        {
                            if ((filter.field.FieldSource == cField.FieldSourceType.CustomEntity || filter.field.FieldSource == cField.FieldSourceType.Userdefined))
                            {
                                fieldName = "[valueid] ";
                            }
                            else
                            {
                                fieldName = String.Format("[{0}] ", filter.field.FieldName);
                            }
                        }
                        else
                        {
                            fieldName = String.Format("[{0}] ", filter.field.GetParentTable().GetKeyField().FieldName);
                        }
                        break;
                }

                output.Append(fieldName);

                bool includeNulls = false;

                switch (filter.condition)
                {
                    case ConditionType.WithAccessRoles:
                    case ConditionType.AtMe:
                    case ConditionType.AtMyHierarchy:
                    case ConditionType.AtMyClaimsHierarchy:
                    case ConditionType.AtMyCostCodeHierarchy:
                        output.Append(" IN (");
                        foreach (object x in filter.value1)
                        {
                            if (x.GetType() == typeof(List<SqlDataRecord>))
                            {
                                output.Append("SELECT employeeid FROM @filter1_" + indexOfFilterInList + "_" + filter.value1.IndexOf(x) + ",");
                            }
                            else
                            {
                                output.Append("@filter1_" + indexOfFilterInList + "_" + filter.value1.IndexOf(x) + ",");
                            }
                        }
                        if (filter.value1.Count > 0)
                        {
                            output.Remove(output.Length - 1, 1);
                        }
                        output.Append(")");
                        break;
                    case ConditionType.Equals:
                        if (filter.value1.Count == 1)
                        {
                            if (filter.value1[0] == DBNull.Value)
                            {
                                output.Append(" is null");
                            }
                            else
                            {
                                output.Append(" = ");
                                output.Append("@filter1_" + indexOfFilterInList + "_0");
                            }
                        }
                        else
                        {
                            output.Append(" IN (");
                            foreach (object x in filter.value1)
                            {
                                output.Append("@filter1_" + indexOfFilterInList + "_" + filter.value1.IndexOf(x) + ",");
                            }
                            if (filter.value1.Count > 0)
                            {
                                output.Remove(output.Length - 1, 1);
                            }
                            output.Append(")");
                        }
                        break;
                    case ConditionType.DoesNotEqual:
                        output.Append(" NOT IN (");
                        foreach (object x in filter.value1)
                        {
                            output.Append("@filter1_" + indexOfFilterInList + "_" + filter.value1.IndexOf(x) + ",");
                        }
                        if (filter.value1.Count > 0)
                        {
                            output.Remove(output.Length - 1, 1);
                        }
                        output.Append(")");
                        includeNulls = true;
                        break;
                    case ConditionType.ContainsData:
                        output.Append(" IS NOT NULL");
                        break;
                    case ConditionType.DoesNotContainData:
                        output.Append(" IS NULL");
                        break;
                    case ConditionType.Like:
                        output.Append(" LIKE @filter1_" + indexOfFilterInList);
                        break;
                    case ConditionType.NotLike:
                        output.Append(" NOT LIKE @filter1_" + indexOfFilterInList);
                        includeNulls = true;
                        break;
                }
                if (includeNulls)
                {
                    output.Append($" OR {tableName}{fieldName} IS NULL");
                }
                output.Append(")");
            }
            else
            {
                if (!bPagingActivated || (bPagingActivated && subQuery))
                {
                    string whereField = string.Empty;
                    tableName = string.Empty;
                    if ((!bPagingActivated && filter.field.FieldName.Contains("dbo.") == false) ||
                        (bPagingActivated && subQuery))
                    {
                        tableName = filter.JoinVia != null
                            ? $"[{filter.JoinVia.TableAlias}]."
                                        : $"[{filter.field.GetParentTable().TableName}].";
                    }

                    fieldName = String.Format(filter.field.FieldName.Contains("dbo.") ? "{0}" : "[{0}]",
                        filter.field.FieldName);
                    if (filter.JoinVia != null)
                    {
                        fieldName = cReport.ReplaceTableNameWithJoinViaAlias(filter.JoinVia, filter.field, fieldName);
                    }

                    if (filter.condition == ConditionType.AtMyHierarchy ||
                        filter.condition == ConditionType.AtMyClaimsHierarchy || 
                        filter.condition == ConditionType.AtMyCostCodeHierarchy ||
                        filter.condition == ConditionType.WithAccessRoles)
                    {
                        fieldName = $"[{filter.field.GetParentTable().GetPrimaryKey().FieldName}] ";
                    }

                    if (fieldName.Contains("dbo."))
                    {
                        whereField = fieldName;
                    }
                    else
                    {
                        whereField = tableName + fieldName;
                    }

                    if (filter.condition == ConditionType.Like)
                    {
                        //globalization --- start with uk for now.
                        int internationalFormat = 103;
                        switch (filter.field.FieldType)
                        {
                            case "T":
                                whereField = $"convert(varchar(5), {tableName}{fieldName}, 8)";
                                break;
                            case "D":
                                whereField = $"convert(varchar(10),{tableName}{fieldName},{internationalFormat})";
                                break;
                            case "DT":
                                whereField =
                                    $"convert(varchar(10),{tableName}{fieldName},{internationalFormat}) + ' ' + convert(varchar(5), {tableName}{fieldName}, 8)";
                                break;
                        }
                    }

                    if (filter.field.Encrypted)
                    {
                        whereField = $"CAST(DECRYPTBYPASSPHRASE(@salt, {whereField} ) AS NVARCHAR(Max))";
                    }

                    output.Append(whereField + " ");

                    bool includeNulls = false;
                    switch (filter.condition)
                    {
                        case ConditionType.WithAccessRoles:
                        case ConditionType.AtMyHierarchy:
                        case ConditionType.AtMyClaimsHierarchy:
                        case ConditionType.AtMyCostCodeHierarchy:
                            output.Append(" IN (");
                            foreach (object x in filter.value1)
                            {
                                if (x == null || x.GetType() == typeof (List<SqlDataRecord>))
                                {
                                    output.AppendFormat("SELECT employeeid FROM @filter1_{0}_{1},", indexOfFilterInList, filter.value1.IndexOf(x));
                                }
                                else
                                {
                                    output.AppendFormat("@filter1_{0}_{1},", indexOfFilterInList, filter.value1.IndexOf(x));
                                }
                            }

                            if (filter.value1.Count > 0)
                            {
                                output.Remove(output.Length - 1, 1);
                            }

                            output.Append(")");
                            break;
                        case ConditionType.Equals:
                            output.Append(" IN (");
                            foreach (object x in filter.value1)
                            {
                                output.Append("@filter1_" + indexOfFilterInList + "_" + filter.value1.IndexOf(x) + ",");
                            }
                            if (filter.value1.Count > 0)
                            {
                                output.Remove(output.Length - 1, 1);
                            }
                            output.Append(")");
                            break;
                        case ConditionType.On:
                            output.Append(" = @filter1_" + indexOfFilterInList);
                            break;
                        case ConditionType.DoesNotEqual:
                            output.Append(" NOT IN (");
                            foreach (object x in filter.value1)
                            {
                                output.Append("@filter1_" + indexOfFilterInList + "_" + filter.value1.IndexOf(x) + ",");
                            }
                            if (filter.value1.Count > 0)
                            {
                                output.Remove(output.Length - 1, 1);
                            }
                            output.Append(")");
                            includeNulls = true;
                            break;
                        case ConditionType.NotOn:
                            foreach (object x in filter.value1)
                            {
                                output.Append(" <> @filter1_" + indexOfFilterInList);
                            }
                            includeNulls = true;
                            break;
                        case ConditionType.GreaterThan:
                        case ConditionType.After:
                            output.Append(" > @filter1_" + indexOfFilterInList);
                            break;
                        case ConditionType.LessThan:
                        case ConditionType.Before:
                            output.Append(" < @filter1_" + indexOfFilterInList);
                            break;
                        case ConditionType.GreaterThanEqualTo:
                        case ConditionType.OnOrAfter:
                            output.Append(" >= @filter1_" + indexOfFilterInList);
                            break;
                        case ConditionType.OnOrAfterToday:
                            output.Append(" >= @filter1_" + indexOfFilterInList);
                            includeNulls = true;
                            break;
                        case ConditionType.LessThanEqualTo:
                        case ConditionType.OnOrBefore:
                            output.Append(" <= @filter1_" + indexOfFilterInList);
                            break;
                        case ConditionType.OnOrBeforeToday:
                            output.Append(" <= @filter1_" + indexOfFilterInList);
                            includeNulls = true;
                            break;                     
                        case ConditionType.Like:
                            output.Append(" LIKE @filter1_" + indexOfFilterInList);
                            break;
                        case ConditionType.NotLike:
                            output.Append(" NOT LIKE @filter1_" + indexOfFilterInList);
                            includeNulls = true;
                            break;
                        case ConditionType.ContainsData:
                            output.Append(" IS NOT NULL");
                            break;
                        case ConditionType.DoesNotContainData:
                            output.Append(" IS NULL");
                            break;
                        case ConditionType.Between:
                        case ConditionType.Today:                       
                        case ConditionType.Tomorrow:
                        case ConditionType.Yesterday:
                        case ConditionType.Last7Days:
                        case ConditionType.LastTaxYear:
                        case ConditionType.LastFinancialYear:
                        case ConditionType.LastMonth:
                        case ConditionType.LastWeek:
                        case ConditionType.LastXDays:
                        case ConditionType.LastXMonths:
                        case ConditionType.LastXWeeks:
                        case ConditionType.LastXYears:
                        case ConditionType.LastYear:
                        case ConditionType.Next7Days:
                        case ConditionType.NextTaxYear:
                        case ConditionType.NextFinancialYear:
                        case ConditionType.NextMonth:
                        case ConditionType.NextWeek:
                        case ConditionType.NextXDays:
                        case ConditionType.NextXMonths:
                        case ConditionType.NextXWeeks:
                        case ConditionType.NextXYears:
                        case ConditionType.NextYear:
                        case ConditionType.ThisTaxYear:
                        case ConditionType.ThisFinancialYear:
                        case ConditionType.ThisMonth:
                        case ConditionType.ThisWeek:
                        case ConditionType.ThisYear:
                            output.Append(" BETWEEN @filter1_" + indexOfFilterInList + " and @filter2_" +
                                          indexOfFilterInList);
                            break;
                    }
                    if (includeNulls)
                    {
                        output.Append(String.Format(" OR {0}{1} IS NULL", tableName, fieldName));
                    }
                }
                output.Append(")");
            }

            return output.ToString();
        }


        /// <summary>
        /// This will get a list of the non-static columns used in the query
        /// </summary>
        /// <param name="lstJoinVia"></param>
        /// <returns></returns>
        private SortedList<Guid, cField> getDistinctFields(ref SortedList<Guid, JoinVia> lstJoinVia)
        {
            var lstFields = new SortedList<Guid, cField>();
            var lstFieldIds = new List<Tuple<Guid, Guid>>();
            var lstTmp = new List<cField>();
            Guid gKey;

            foreach (cQueryField queryField in lstColumns)
            {
                if (!queryField.isStaticColumn)
                {
                    gKey = Guid.NewGuid();

                    if (queryField.JoinVia != null)
                    {
                        gKey = queryField.JoinVia.JoinViaAS;
                        if (lstJoinVia.ContainsKey(gKey) == false)
                        {
                            lstJoinVia.Add(gKey, queryField.JoinVia);
                        }
                    }

                    if (lstFields.ContainsKey(gKey) == false)
                    {
                        lstFields.Add(gKey, queryField.field);
                    }
                }
            }

            foreach (cQueryFilter queryFilter in lstCriteria)
            {
                gKey = Guid.NewGuid();
                bool isBaseTable = queryFilter.JoinVia == null;

                if (isBaseTable == false)
                {
                    gKey = queryFilter.JoinVia.JoinViaAS;
                    if (lstJoinVia.ContainsKey(gKey) == false)
                    {
                        lstJoinVia.Add(gKey, queryFilter.JoinVia);
                    }
                }

                bool listContainsKey = lstFields.ContainsKey(gKey);
                bool listContainsValue = lstFieldIds.Any(field => field.Item1 == queryFilter.field.FieldID);

                if (listContainsKey == false && (listContainsValue == false || isBaseTable == false))
                {
                    lstFields.Add(gKey, queryFilter.field);
                    lstFieldIds.Add(new Tuple<Guid, Guid>(queryFilter.field.FieldID, gKey));
                }
            }

            foreach (cQueryFilterGroup filterGroup in lstFilterGroup)
            {
                foreach (cQueryFilter queryFilter in filterGroup.QueryFilters)
                {
                    gKey = Guid.NewGuid();
                    bool isBaseTable = queryFilter.JoinVia == null;

                    if (isBaseTable == false)
                    {
                        gKey = queryFilter.JoinVia.JoinViaAS;
                        if (lstJoinVia.ContainsKey(gKey) == false)
                        {
                            lstJoinVia.Add(gKey, queryFilter.JoinVia);
                        }
                    }

                    bool listContainsKey = lstFields.ContainsKey(gKey);
                    bool listContainsValue = lstFieldIds.Any(field => field.Item1 == queryFilter.field.FieldID);

                    if (listContainsKey == false && (listContainsValue == false || isBaseTable == false))
                    {
                        lstFields.Add(gKey, queryFilter.field);
                        lstFieldIds.Add(new Tuple<Guid, Guid>(queryFilter.field.FieldID, gKey));
                    }
                }
            }

            return lstFields;
        }

        public string sql
        {
            get
            {
                StringBuilder output = new StringBuilder();

                output.Append(generateSelect());
                if (!bPagingActivated)
                {
                    cJoins clsjoins = new cJoins(accountid);
                    SortedList<Guid, JoinVia> lstVia = new SortedList<Guid, JoinVia>();
                    SetCustomEntityListFieldOrders();
                    output.Append(clsjoins.createJoinSQL(getDistinctFields(ref lstVia), basetable.TableID, lstVia, CustomEntityListOrders));
                }
                output.Append(" " + generateWhere(false));
                output.Append(" " + generateOrderby());

                return output.ToString();
            }
        }

        private string generateOrderby()
        {
            StringBuilder output = new StringBuilder();
            if (lstSortedColumns.Count == 0)
            {
                return "";
            }

            output.Append("ORDER BY ");
            foreach (cQueryField queryField in lstSortedColumns)
            {
                var fieldName = string.Empty;
                if (bPagingActivated)
                {
                    if (!string.IsNullOrEmpty(queryField.alias))
                    {
                        fieldName = "[" + queryField.alias + "_sorted]";
                    }
                    else
                    {
                        if (!queryField.showListItemText && !(queryField.field.GenList && queryField.field.ListItems.Count == 0 && queryField.field.FieldSource == cField.FieldSourceType.CustomEntity))
                        {
                            fieldName = "[" + queryField.field.FieldName + "]";
                        }
                        else
                        {
                            cField relatedFieldValueField = clsfields.GetFieldByID(queryField.field.GetLookupTable().KeyFieldID);
                            fieldName = "[" + relatedFieldValueField.FieldName + "_text]";
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(queryField.alias))
                    {
                        fieldName = "[" + queryField.alias + "]";
                    }
                    else
                    {
                        if (!(queryField.field.GenList || queryField.field.ValueList))
                        {
                            if (queryField.field.FieldName.Contains("dbo."))
                            {
                                string tmpFieldName = queryField.field.FieldName;
                                if (queryField.JoinVia != null)
                                {
                                    tmpFieldName = cReport.ReplaceTableNameWithJoinViaAlias(queryField, tmpFieldName);
                                    output.Append("[" + tmpFieldName + "]");
                                }
                                else
                                {
                                    output.Append(tmpFieldName + " " );
                                }
                            }
                            else
                            {
                                if (queryField.JoinVia != null)
                                {
                                    fieldName = "[" + queryField.JoinVia.TableAlias + "].[" + queryField.field.FieldName + "]";
                                }
                                else
                                {
                                    fieldName = "[" + queryField.field.GetParentTable().TableName + "].[" + queryField.field.FieldName + "]";
                                }
                            }
                        }
                        else
                        {
                            if (!queryField.showListItemText && queryField.field.ListItems.Count > 0)
                            {
                                fieldName = "[" + queryField.field.FieldName + "_text]";
                            }
                            else if (queryField.field.GenList && queryField.field.ListItems.Count == 0 && queryField.field.FieldSource == cField.FieldSourceType.CustomEntity)
                            {
                                cField relatedFieldValueField = clsfields.GetFieldByID(queryField.field.GetLookupTable().KeyFieldID);
                                fieldName = "[" + relatedFieldValueField.FieldName + "_text]";
                            }
                            else
                            {
                                fieldName = "[" + queryField.field.FieldName + "]";
                            }
                        }
                    }
                }

                if (queryField.field.Encrypted && !fieldName.EndsWith("_text]"))
                {
                    fieldName = $"CAST(DECRYPTBYPASSPHRASE(@salt, {fieldName} ) AS NVARCHAR(Max))";
                }

                output.Append(fieldName);

                output.Append(queryField.SortDirection == SortDirection.Ascending ? " ASC" : " DESC");
                output.Append(",");
            }
            output.Remove(output.Length - 1, 1);
            return output.ToString();
        }

        public SqlDataReader getReader()
        {
            DBConnection expdata = new DBConnection(connectionstring);
            SqlDataReader reader;
            setCriteriaParameters(ref expdata);
            reader = expdata.GetReader(sql);
            expdata.sqlexecute.Parameters.Clear();
            return reader;
        }

        public DataSet getDataset()
        {
            var expdata = new DBConnection(connectionstring);
            setCriteriaParameters(ref expdata);
            DataSet ds = expdata.GetDataSet(sql);
            expdata.sqlexecute.Parameters.Clear();
            return ds;
        }

        public int GetCount()
        {
            int count = 0;
            DBConnection expdata = new DBConnection(connectionstring);
            setCriteriaParameters(ref expdata);
            count = expdata.getcount(sql);
            expdata.sqlexecute.Parameters.Clear();
            return count;
        }
    }

    public class cUpdateQuery : cQueryBuilder
    {

        List<SqlParameter> lstParameters = new List<SqlParameter>();
        public cUpdateQuery(int accountid, string connectionstring, string metabaseconnectionstring, cTable basetable, cTables tables, cFields fields)
            : base(accountid, connectionstring, metabaseconnectionstring, basetable, tables, fields)
        {

        }

        public void addColumn(cField field, object value)
        {
            lstColumns.Add(new cQueryField(field));
            lstParameters.Add(new SqlParameter("@" + (lstParameters.Count + 1), value));
        }


        private string generateInsert()
        {
            StringBuilder output = new StringBuilder();

            output.Append("INSERT INTO [dbo].[" + basetable.TableName + "] (");
            foreach (cQueryField column in lstColumns)
            {
                output.Append("[" + column.field.FieldName + "], ");
            }

            output.Remove(output.Length - 2, 2);
            output.Append(") VALUES (");
            for (var index = 0; index < this.lstParameters.Count; index++)
            {
                var column = this.lstColumns[index];
                SqlParameter parameter = this.lstParameters[index];
                if (column.field.Encrypted)
                {
                    output.Append($"ENCRYPTBYPASSPHRASE(@salt, {parameter.ParameterName} ),");
                }
                else
                {
                    output.Append(parameter.ParameterName + ", ");    
                }
            }

            output.Remove(output.Length - 2, 2);
            output.Append(");select @identity = @@identity;");
            return output.ToString();
        }

        private string generateUpdate()
        {
            StringBuilder output = new StringBuilder();
            output.Append("UPDATE [dbo].[" + basetable.TableName + "] SET ");
            foreach (cQueryField column in lstColumns)
            {
                output.Append(
                    column.field.Encrypted
                        ? $"[{column.field.FieldName}] = ENCRYPTBYPASSPHRASE(@salt, {this.lstParameters[this.lstColumns.IndexOf(column)].ParameterName}), "
                        : $"[{column.field.FieldName}] = {this.lstParameters[this.lstColumns.IndexOf(column)].ParameterName}, ");
            }
            output.Remove(output.Length - 2, 2);
            output.Append(" " + generateWhere(false));
            return output.ToString();
        }

        public string insertSQL
        {
            get { return generateInsert(); }
        }
        public string updateSQL
        {
            get { return generateUpdate(); }
        }
        public int executeInsertStatement()
        {
            DBConnection expdata = new DBConnection(connectionstring);
            expdata.sqlexecute.Parameters.AddWithValue("@salt", "2FD583C9-BF7E-4B4E-B6E6-5FC9375AD069");
            foreach (SqlParameter parameter in lstParameters)
            {
                expdata.sqlexecute.Parameters.Add(parameter);
            }
            expdata.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
            expdata.ExecuteSQL(generateInsert());
            int id = -1;
            if (expdata.sqlexecute.Parameters["@identity"].Value != null && expdata.sqlexecute.Parameters["@identity"].Value != DBNull.Value)
            {
                id = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            }
            expdata.sqlexecute.Parameters.Clear();
            return id;
        }

        public void executeUpdateStatement()
        {
            DBConnection expdata = new DBConnection(connectionstring);
            foreach (SqlParameter parameter in lstParameters)
            {
                expdata.sqlexecute.Parameters.Add(parameter);
            }
            setCriteriaParameters(ref expdata);
            expdata.ExecuteSQL(generateUpdate());
            expdata.sqlexecute.Parameters.Clear();
        }
    }

        public class cDeleteQuery : cQueryBuilder
        {

            List<SqlParameter> lstParameters = new List<SqlParameter>();

            /// <summary>
            /// Create a new <see cref="cDeleteQuery"/>object.Initialise the cDeleteQuery with the parameters necessary to build the delete query on a any table
            /// </summary>
            public cDeleteQuery(int accountid, string connectionstring, string metabaseconnectionstring, cTable basetable, cTables tables, cFields fields)
                : base(accountid, connectionstring, metabaseconnectionstring, basetable, tables, fields)
            {
            }

            /// <summary>
            /// Generate Delete query
            /// </summary>
            /// <returns>SQL string for delete operation</returns>
            private string GenerateDelete()
             {
                    StringBuilder output = new StringBuilder();
                output.Append("DELETE FROM [dbo].[" + basetable.TableName + "]");
                output.Append(" " + generateWhere(false));
                return output.ToString();
             }

        /// <summary>
        /// Execute the delete query 
        /// </summary>
        public void ExecuteDeleteStatement()
        {
            DBConnection expdata = new DBConnection(connectionstring);
            foreach (SqlParameter parameter in lstParameters)
            {
                expdata.sqlexecute.Parameters.Add(parameter);
            }

            setCriteriaParameters(ref expdata);
            expdata.ExecuteSQL(GenerateDelete());
            expdata.sqlexecute.Parameters.Clear();
        }
    }

        public class cQueryField
        {
            private cField clsfield;
            private string sAlias;
            private SelectType eSelectType = SelectType.Fields;
            private SortDirection eSortDirection;
            private bool bStatic;
            private string sStaticValue;
            private bool bshowListItemText;
            private JoinVia _joinVia;

            public cQueryField(cField field)
            {
                clsfield = field;
                sAlias = "";
            }
            public cQueryField(cField field, string alias)
            {
                clsfield = field;
                sAlias = alias;
            }

            public cQueryField(cField field, SelectType selType)
            {
                clsfield = field;
                sAlias = "";
                eSelectType = selType;
            }

            public cQueryField(cField field, string alias, SelectType selType)
            {
                clsfield = field;
                sAlias = alias;
                eSelectType = selType;
            }
            public cQueryField(cField field, SortDirection sortdirection)
            {
                clsfield = field;
                eSortDirection = sortdirection;
            }

            public cQueryField(cField field, SortDirection sortdirection, string alias)
            {
                clsfield = field;
                eSortDirection = sortdirection;
                sAlias = alias;
            }
            public cQueryField(string static_value, string field_name)
            {
                clsfield = null;
                bStatic = true;
                sStaticValue = static_value;
                sAlias = field_name;
            }
            public cQueryField(cField field, JoinVia joinVia, SortDirection sortDirection = SortDirection.None, string alias = "")
            {
                clsfield = field;
                sAlias = string.IsNullOrWhiteSpace(alias) ? field.FieldName + "_" + joinVia.JoinViaID : alias;
                if (joinVia == null) { throw new ArgumentNullException("joinVia", "cQueryField joinVia constructor parameter can not be null"); }
                _joinVia = joinVia;
                if (sortDirection != SortDirection.None) { eSortDirection = sortDirection; }
            }

            #region properties
            public cField field
            {
                get { return clsfield; }
            }
            public string alias
            {
                get { return sAlias; }
            }

            public SelectType selectType
            {
                get { return eSelectType; }
            }
            public SortDirection SortDirection
            {
                get { return eSortDirection; }
            }
            public bool isStaticColumn
            {
                get { return bStatic; }
            }
            public string staticValue
            {
                get { return sStaticValue; }
            }
            public bool showListItemText
            {
                get { return bshowListItemText; }
                set { bshowListItemText = value; }
            }
            /// <summary>
            /// JoinVia - Avoids normal JoinTables, null unless the query needs to bypass normal jointables
            /// </summary>
            public JoinVia JoinVia
            {
                get { return _joinVia; }
            }
            #endregion
        }

        public class cQueryFilterString
        {
            private string sFilterString;
            private ConditionJoiner cjJoiner;
            #region properties
            public string filterString
            {
                get { return sFilterString; }
            }
            public ConditionJoiner joiner
            {
                get { return cjJoiner; }
            }
            #endregion

            public cQueryFilterString(string sql_filterstring, ConditionJoiner sql_joiner)
            {
                sFilterString = sql_filterstring;
                cjJoiner = sql_joiner;
            }
        }

        public class cQueryFilter
        {
            private cField clsfield;
            private ConditionType ctCondition;
            private List<object> lstValue1 = new List<object>();
            private List<object> lstValue2 = new List<object>();
            private ConditionJoiner cjJoiner;
            private JoinVia _joinVia;
            private bool IsParentFilter;

        public cQueryFilter(cField field, ConditionType condition, List<object> value1, List<object> value2, ConditionJoiner joiner, JoinVia joinVia)
            {
                clsfield = field;
                ctCondition = condition;
                lstValue1 = value1;
                lstValue2 = value2;
                cjJoiner = joiner;
                _joinVia = joinVia;
            }

            public cQueryFilter(cField field, string parametername, object parametervalue)
            {
                clsfield = field;
                parameterName = parametername;
                parameterValue = parametervalue;
            }
            #region properties
            public cField field
            {
                get { return clsfield; }
            }
            public ConditionType condition
            {
                get { return ctCondition; }
            }
            public List<object> value1
            {
                get { return lstValue1; }
            }
            public List<object> value2
            {
                get { return lstValue2; }
            }
            public ConditionJoiner joiner
            {
                get { return cjJoiner; }
            }
            /// <summary>
            /// JoinVia - Avoids normal JoinTables, null unless the query needs to bypass normal jointables
            /// </summary>
            public JoinVia JoinVia
            {
                get { return _joinVia; }
            }

            /// <summary>
            /// Gets or sets a parameter name used with an explicit where caluse
            /// </summary>
            public string parameterName { get; set; }
            /// <summary>
            /// Gets or sets a parameter value used with an explicit where caluse
            /// </summary>
            public object parameterValue { get; set; }

            #endregion
        }

        public class cQueryFilterGroup
        {
            private List<cQueryFilter> lstQueryFilters = new List<cQueryFilter>();
            private ConditionJoiner ctJoiner;

            public cQueryFilterGroup(List<cQueryFilter> queryFilters, ConditionJoiner joiner)
            {
                lstQueryFilters = queryFilters;
                ctJoiner = joiner;
            }

            #region properties

            public List<cQueryFilter> QueryFilters
            {
                get { return lstQueryFilters; }
            }

            public ConditionJoiner Joiner
            {
                get { return ctJoiner; }
            }

            #endregion
        }

        public enum SelectType
        {
            Fields,
            Count,
            Distinct,
            Min,
            Max,
            Sum
        }

        [Serializable]
        public enum SortDirection
        {
            None,
            Ascending,
            Descending
        }
    }
