using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Text;
using SpendManagementLibrary.Extentions;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Enumerators;
using SpendManagementLibrary.Helpers;

namespace Spend_Management
{
    using System.Globalization;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Definitions.JoinVia;

    /// <summary>
    /// Class used for generating data grids with enriched functionality
    /// </summary>
    public class cGridNew : System.Web.UI.Control
    {
        private bool bCanDisplayGrid;
        private string sGridDisplayError;

        private int nAccountid;
        private int nEmployeeid;
        private string sConnectionString;
        private string sMetabaseConnectionString;
        private bool bShowHeaders = true;
        private bool bShowFooters = false;
        private cTable clsBaseTable;
        private bool bEnablePaging = true;
        private int nPageSize = 20;
        private PagerPosition ePagerPosition = PagerPosition.TopAndBottom;
        private int nCurrentPageNumber = 1;
        private string sSql;
        private bool bEnableUpdating;
        private bool bEnableDeleting;
        private bool bEnableArchiving;
        private bool bAllowSelect;
        private GridSelectType eSelectType = GridSelectType.CheckBox;
        private bool bEnableSorting = true;
        private bool bEnableSelectOnChange;
        private string sEditLink;
        private string sDeleteLink;
        private string sSelectLink;
        private string sArchiveLink;
        private string sArchiveField;
        private string sCssClass = "cGrid";
        private string sRowClass = "row1";
        private int nRowCount;
        private int nRowHiddenCount;
        private string sGridID;
        private List<cNewGridColumn> lstColumns = new List<cNewGridColumn>();
        private List<cQueryFilter> lstCriteria = new List<cQueryFilter>();
        //private List<cEventColumn> lstEventColumns = new List<cEventColumn>();
        private List<cQueryFilterString> lstAudienceFilters = new List<cQueryFilterString>();
        private cNewGridColumn clsSortedColumn;
        private SpendManagementLibrary.SortDirection eSortDirection = SpendManagementLibrary.SortDirection.Ascending;
        private string sKeyField;
        private bool bDisplayFilter = true;
        private string sFilterCriteria;
        private string sEmptyText;
        private DataSet dsData;
        private bool bSourceIsDataSet = false;
        private List<object> lstSelectedItems = new List<object>();
        private int nCurrencyId;
        private string sCurrencyColumnName;
        private string sServiceClassForInitialiseRowEvent;
        private string sServiceClassMethodForInitialiseRowEvent;
        private SerializableDictionary<string, object> dicGridInfo;
        private List<int> lstHiddenRecords;

        public delegate void InitialiseRowEvent(cNewGridRow row, SerializableDictionary<string, object> gridInfo);
        public event InitialiseRowEvent InitialiseRow;
        private string _defaultCurrencySymbol;

        List<cNewGridRow> lstRows = new List<cNewGridRow>();

        #region properties
        /// <summary>
        /// Gets the accountID of the database being used
        /// </summary>
        public int accountid
        {
            get { return nAccountid; }
        }
        /// <summary>
        /// Gets the connectionstring to the database
        /// </summary>
        private string connectionstring
        {
            get { return sConnectionString; }
        }
        /// <summary>
        /// Gets the metabase connectionstring to the database
        /// </summary>
        private string metabaseconnectionstring
        {
            get { return sMetabaseConnectionString; }
        }
        /// <summary>
        /// Gets the grid's base table
        /// </summary>
        public cTable basetable
        {
            get { return clsBaseTable; }
        }
        /// <summary>
        /// Switches paging on and off. Default is on.
        /// </summary>
        public bool enablepaging
        {
            get
            {
                if (bEnablePaging && RowCount > pagesize)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { bEnablePaging = value; }
        }
        /// <summary>
        /// Gets or sets the number of items per page. Default is 20.
        /// </summary>
        public int pagesize
        {
            get { return nPageSize; }
            set { nPageSize = value; }
        }
        /// <summary>
        /// Gets or Sets the current page number
        /// </summary>
        public int currentpagenumber
        {
            get { return nCurrentPageNumber; }
            set { nCurrentPageNumber = value; }
        }
        /// <summary>
        /// Gets or sets whether the header row should be displayed.
        /// </summary>
        public bool showheaders
        {
            get { return bShowHeaders; }
            set { bShowHeaders = value; }
        }
        /// <summary>
        /// Gets or sets whether the footer row should be displayed.
        /// </summary>
        public bool showfooters
        {
            get { return bShowFooters; }
            set { bShowFooters = value; }
        }
        /// <summary>
        /// Gets or sets whether the edit column should be displayed on the grid.
        /// </summary>
        public bool enableupdating
        {
            get { return bEnableUpdating; }
            set { bEnableUpdating = value; }
        }
        /// <summary>
        /// Gets or sets whether the delete column should be displayed on the grid.
        /// </summary>
        public bool enabledeleting
        {
            get
            {
                return bEnableDeleting;
            }
            set { bEnableDeleting = value; }
        }
        /// <summary>
        /// Gets or sets whether the archive/unarchive column should be displayed on the grid.
        /// </summary>
        public bool enablearchiving
        {
            get
            {
                if (bEnableArchiving && archivelink != null && ArchiveField != null && KeyField != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            set { bEnableArchiving = value; }
        }
        /// <summary>
        /// Gets or sets whether or not the checkboxes have an attached onchange event
        /// </summary>
        public bool EnableSelectOnChange
        {
            get { return bEnableSelectOnChange; }
            set { bEnableSelectOnChange = value; }
        }
        /// <summary>
        /// Gets or sets the href value of the edit link. A field name enclosed in {} will be replaced with the actual column value. For example specifying "aecostcode.aspx?costcodeid={costcodeid}" will replace {costcodeid} with the actual costcodeid of the row.
        /// </summary>
        public string editlink
        {
            get { return sEditLink; }
            set { sEditLink = value; }
        }
        /// <summary>
        /// Gets or sets the href value of the delete link. A field name enclosed in {} will be replaced with the actual column value. For example specifying "javascript:deleteCostCode({costcodeid});" will replace {costcodeid} with the actual costcodeid of the row.
        /// </summary>
        public string deletelink
        {
            get { return sDeleteLink; }
            set { sDeleteLink = value; }
        }
        /// <summary>
        /// Gets or sets the onchange value of the checkboxes. A field name enclosed in {} will be replaced with the actual column value. For example specifying "javascript:SelectCostCode({costcodeid});" will replace {costcodeid} with the actual costcodeid of the row.
        /// </summary>
        public string SelectLink
        {
            get { return sSelectLink; }
            set { sSelectLink = value; }
        }
        /// <summary>
        /// Gets or sets the href value of the archive link. A field name enclosed in {} will be replaced with the actual column value. For example specifying "javascript:archiveCostCode({costcodeid});" will replace {costcodeid} with the actual costcodeid of the row.
        /// </summary>
        public string archivelink
        {
            get { return sArchiveLink; }
            set { sArchiveLink = value; }
        }
        /// <summary>
        /// Gets or sets the field to check to determine if the record is archived or not. Archiving will be disabled if this has not been specified.
        /// </summary>
        public string ArchiveField
        {
            get { return sArchiveField; }
            set { sArchiveField = value; }
        }
        /// <summary>
        /// Gets or sets the css class of the table. Default is "datatbl".
        /// </summary>
        public string CssClass
        {
            get { return sCssClass; }
            set { sCssClass = value; }
        }
        private string rowclass
        {
            get { return sRowClass; }
        }
        /// <summary>
        /// Gets or sets the position of the pager. Default is Top and Bottom of grid.
        /// </summary>
        public PagerPosition PagerPosition
        {
            get { return ePagerPosition; }
            set { ePagerPosition = value; }
        }
        /// <summary>
        /// The number of rows in the grid
        /// </summary>
        public int RowCount
        {
            get { return nRowCount; }
        }

        /// <summary>
        /// The number of rows in the grid
        /// </summary>
        public int RowHiddenCount
        {
            get { return nRowHiddenCount; }
        }

        /// <summary>
        /// Gets or sets the ID of the grid
        /// </summary>
        public string GridID
        {
            get { return sGridID; }
        }
        /// <summary>
        /// Gets or the sets the column this grid should be sorted by.
        /// </summary>
        public cNewGridColumn SortedColumn
        {
            get { return clsSortedColumn; }
            set
            {
                clsSortedColumn = value;
            }
        }
        /// <summary>
        /// Gets or sets the direction of the sorted column.
        /// </summary>
        public SpendManagementLibrary.SortDirection SortDirection
        {
            get { return eSortDirection; }
            set { eSortDirection = value; }
        }

        /// <summary>
        /// Gets the employee ID of the user using the grid
        /// </summary>
        public int EmployeeID
        {
            get { return nEmployeeid; }
        }
        /// <summary>
        /// Gets the primary key field being used by the grid
        /// </summary>
        public string KeyField
        {
            get
            {
                if (string.IsNullOrEmpty(sKeyField))
                {
                    // obtain default from the metabase
                    if (clsBaseTable != null && this.clsBaseTable.GetPrimaryKey() != null)
                    {
                        sKeyField = this.clsBaseTable.GetPrimaryKey().FieldName;
                    }
                }

                return sKeyField;
            }
            set { sKeyField = value; }
        }
        /// <summary>
        /// Gets or sets whether the filter box should be displayed above the grid.
        /// </summary>
        public bool DisplayFilter
        {
            get { return bDisplayFilter; }
            set { bDisplayFilter = value; }
        }

        /// <summary>
        /// Gets or sets the value the grid should be filtered by.
        /// </summary>
        public string FilterCriteria
        {
            get { return sFilterCriteria; }
            set { sFilterCriteria = value; }
        }

        /// <summary>
        /// Gets or sets the text to display in the grid when there is no data to display
        /// </summary>
        public string EmptyText
        {
            get
            {
                if (!string.IsNullOrEmpty(sFilterCriteria))
                {
                    return "No Results Found";
                }
                else
                {
                    return sEmptyText;
                }

            }
            set
            {
                sEmptyText = value;
            }
        }
        /// <summary>
        /// Gets or sets whether the grid can be sorted. Default is true
        /// </summary>
        public bool EnableSorting
        {
            get { return bEnableSorting; }
            set { bEnableSorting = value; }
        }
        /// <summary>
        /// Gets or sets whether the grid displays a checkbox to allow selection of each row. Default is false
        /// </summary>
        public bool EnableSelect
        {
            get
            {
                if (bAllowSelect && KeyField != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { bAllowSelect = value; }
        }
        /// <summary>
        /// Gets or sets the record selection type
        /// </summary>
        public GridSelectType GridSelectType
        {
            get { return eSelectType; }
            set { eSelectType = value; }
        }
        /// <summary>
        /// Gets or sets the list of items that should be selected on grid creation
        /// </summary>
        public List<object> SelectedItems
        {
            get { return lstSelectedItems; }
            set { lstSelectedItems = value; }
        }
        /// <summary>
        /// Is the source data in a dataset
        /// </summary>
        public bool SourceIsDataSet
        {
            get { return bSourceIsDataSet; }
            set { bSourceIsDataSet = value; }
        }
        /// <summary>
        /// Gets the Grid Rows Collection
        /// </summary>
        public List<cNewGridRow> Rows
        {
            get { return lstRows; }
        }
        /// <summary>
        /// Gets or Sets the currency Id to be used for formatting currency fields. If not present employee Primary Currency will try and be used.
        /// </summary>
        public int CurrencyId
        {
            get { return nCurrencyId; }
            set { nCurrencyId = value; }
        }
        /// <summary>
        /// Gets or Sets the column name to use for currency formatting. This will override the CurrencyId property and employee primary currency
        /// </summary>
        public string CurrencyColumnName
        {
            get { return sCurrencyColumnName; }
            set { sCurrencyColumnName = value; }
        }

        /// <summary>
        /// gets/sets the namespace.class name for the web service class handling the initialiserow event of the grid
        /// </summary>
        public string ServiceClassMethodForInitialiseRowEvent
        {
            get { return sServiceClassMethodForInitialiseRowEvent; }
            set { sServiceClassMethodForInitialiseRowEvent = value; }
        }

        /// <summary>
        /// gets/sets the method within the web service class that handles the initialiserow event of the grid
        /// </summary>
        public string ServiceClassForInitialiseRowEvent
        {
            get { return sServiceClassForInitialiseRowEvent; }
            set { sServiceClassForInitialiseRowEvent = value; }
        }

        /// <summary>
        /// Gets or Sets additional property information values required during the InitialiseRow event handling
        /// </summary>
        public SerializableDictionary<string, object> InitialiseRowGridInfo
        {
            get { return dicGridInfo; }
            set { dicGridInfo = value; }
        }
        /// <summary>
        /// Gets or Sets any record IDs that are not to be output
        /// </summary>
        public List<int> HiddenRecords
        {
            get { return lstHiddenRecords; }
            set { lstHiddenRecords = value; }
        }


        /// <summary>
        /// Set the width of the Grid (e.g. 100% or 654px)
        /// </summary>
        public Unit Width { get; set; }

        /// <summary>
        /// Gets or sets the where clause in the SQL used to construct the grid. If set all filters in lstCriteria will be ignored. 
        /// </summary>
        public string WhereClause
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default symbol to use for formatting currencies cells
        /// </summary>
        public string DefaultCurrencySymbol
        {
            get { return _defaultCurrencySymbol; }
            set { _defaultCurrencySymbol = value; }
        }
        #endregion

        public cGridNew(int accountid, int employeeid, string gridid, string strsql)
        {
            sGridID = gridid;
            nAccountid = accountid;
            nEmployeeid = employeeid;
            sConnectionString = cAccounts.getConnectionString(accountid);
            sMetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;

            sSql = strsql;

            clsBaseTable = getBaseTable(strsql);
            if (clsBaseTable != null)
            {
                bCanDisplayGrid = true;
                getColumns(strsql);
            }
            else
            {
                bCanDisplayGrid = false;
                sGridDisplayError = "Could not retrieve base table definition";
            }
        }

        public cGridNew(int accountid, int employeeid, string gridid, cTable basetable, List<cNewGridColumn> columns)
        {
            sGridID = gridid;
            nAccountid = accountid;
            nEmployeeid = employeeid;
            sConnectionString = cAccounts.getConnectionString(accountid);
            sMetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;

            clsBaseTable = basetable;
            lstColumns = columns;
            if (clsBaseTable != null)
            {
                bCanDisplayGrid = true;
            }
            else
            {
                bCanDisplayGrid = false;
                sGridDisplayError = "Could not retrieve base table definition";
            }
        }


        public cGridNew(int accountid, int employeeid, cTable basetable)
        {

            nAccountid = accountid;
            nEmployeeid = employeeid;
            sConnectionString = cAccounts.getConnectionString(accountid);
            sMetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
            clsBaseTable = basetable;

            if (clsBaseTable != null)
            {
                bCanDisplayGrid = true;
            }
            else
            {
                bCanDisplayGrid = false;
                sGridDisplayError = "Could not retrieve base table definition";
            }
        }



        public cGridNew(int accountid, int employeeid, string gridid, cTable basetable, List<cNewGridColumn> columns, DataSet ds)
        {
            sGridID = gridid;
            nAccountid = accountid;
            nEmployeeid = employeeid;
            sConnectionString = cAccounts.getConnectionString(accountid);
            sMetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;

            clsBaseTable = basetable;
            lstColumns = columns;
            if (clsBaseTable != null)
            {
                bCanDisplayGrid = true;
            }
            else
            {
                bCanDisplayGrid = false;
                sGridDisplayError = "Could not retrieve base table definition";
            }
            dsData = ds;
            bSourceIsDataSet = true;
        }

        /// <summary>
        /// Create a new grid based on a dataset with no base table and no columns supplied.
        /// </summary>
        /// <param name="currentUser">Current user object.</param>
        /// <param name="dataSet">The complete data set to use</param>
        /// <param name="gridId">The grid ID</param>
        /// <param name="tableId">The table ID associated to grid</param>
        /// <param name="columns">The columns associated to grid</param>
        public cGridNew(ICurrentUser currentUser, DataSet dataSet, string gridId, Guid tableId = new Guid(), List<cNewGridColumn> columns = null)
        {
            sGridID = gridId;
            dsData = dataSet;
            nAccountid = currentUser.AccountID;
            nEmployeeid = currentUser.EmployeeID;
            sConnectionString = cAccounts.getConnectionString(accountid);
            sMetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;
            bSourceIsDataSet = true;
            bCanDisplayGrid = true;
            var clsTables = new cTables(nAccountid);
            clsBaseTable = clsTables.GetTableByID(tableId);
            if (dsData == null || dsData.Tables.Count == 0)
            {
                return;
            }

            if (clsBaseTable!=null)
            {
                if (columns != null)
                {
                    lstColumns = columns;
                }
                else
                {
                    foreach (DataColumn column in dsData.Tables[0].Columns)
                    {
                        var clsfields = new cFields(accountid);
                        cField field = clsfields.GetBy(tableId, column.ColumnName);
                        if (field != null)
                        {
                            lstColumns.Add(new cFieldColumn(field));
                        }
                    }
                }
            }
            else
            {
                foreach (DataColumn column in dsData.Tables[0].Columns)
                {
                    var col = new cFieldColumn(new cField(), column.ColumnName);
                    col.HeaderText = column.ColumnName;
                    col.ID = column.ColumnName;
                    lstColumns.Add(col);
                }
            }

        }

        /// <summary>
        /// Generates a DataSet containing the records for cGridNew
        /// </summary>
        /// <returns>A <see cref="DataSet"/> containing <see cref="cGridNew" /> data</returns>
        public DataSet generateDataSet()
        {
            cQueryBuilder query;

            nRowCount = getRowCount();

            if (enablepaging)
            {
                int endrow = currentpagenumber * pagesize;
                if (endrow < pagesize)
                {
                    endrow = pagesize;
                }
                int startrow = endrow - pagesize + 1;
                if (startrow < 0)
                {
                    startrow = 0;
                }

                query = new cQueryBuilder(accountid, connectionstring, metabaseconnectionstring, basetable, new cTables(accountid), new cFields(accountid), startrow, endrow);
            }
            else
            {
                query = new cQueryBuilder(accountid, connectionstring, metabaseconnectionstring, basetable, new cTables(accountid), new cFields(accountid));
            }
            foreach (cNewGridColumn column in lstColumns)
            {
                if (column.ColumnType == GridColumnType.Field)
                {
                    bool useListItemText = false;
                    if (((cFieldColumn)column).field.GenList && ((cFieldColumn)column).field.ListItems.Count == 0)
                    {
                        useListItemText = true;
                    }

                    if (((cFieldColumn)column).JoinVia != null)
                    {
                        query.addColumn(((cFieldColumn)column).field, ((cFieldColumn)column).JoinVia);
                    }
                    else if (!string.IsNullOrEmpty(column.Alias))
                    {
                        if (useListItemText)
                        {
                            query.addColumn(((cFieldColumn)column).field, column.Alias, true);
                        }
                        else
                        {
                            query.addColumn(((cFieldColumn)column).field, column.Alias);
                        }
                    }
                    else
                    {
                        if (useListItemText)
                        {
                            query.addColumn(((cFieldColumn)column).field, true);
                        }
                        else
                        {
                            query.addColumn(((cFieldColumn)column).field);
                        }
                    }
                }
                if (column.ColumnType == GridColumnType.TwoState)
                {
                    bool useListItemText = false;
                    if (((cTwoStateEventColumn)column).Field.GenList && ((cTwoStateEventColumn)column).Field.ListItems.Count == 0)
                    {
                        useListItemText = true;
                    }

                    if (((cTwoStateEventColumn)column).JoinVia != null)
                    {
                        query.addColumn(((cTwoStateEventColumn)column).Field, ((cTwoStateEventColumn)column).JoinVia);
                    }
                    else if (!string.IsNullOrEmpty(column.Alias))
                    {
                        query.addColumn(((cTwoStateEventColumn)column).Field, column.Alias, useListItemText);
                    }
                    else
                    {
                        query.addColumn(((cTwoStateEventColumn)column).Field, useListItemText);
                    }
                }
            }
            foreach (cQueryFilter filter in lstCriteria)
            {
                query.addFilter(filter);
            }



            if (SortedColumn != null && ((cFieldColumn)SortedColumn).field != null)
            {
                if (((cFieldColumn)SortedColumn).JoinVia != null)
                {
                    query.addSortableColumn(((cFieldColumn)SortedColumn).field, SortDirection, ((cFieldColumn)SortedColumn).JoinVia);
                }
                else if (!string.IsNullOrEmpty(SortedColumn.Alias))
                {
                    query.addSortableColumn(((cFieldColumn)SortedColumn).field, SortDirection, SortedColumn.Alias);
                }
                else
                {
                    query.addSortableColumn(((cFieldColumn)SortedColumn).field, SortDirection);
                }
            }
            else
            {
                throw new Exception("SortedColumn not correctly set.");
            }

            if (!string.IsNullOrEmpty(WhereClause))
            {
                query.WhereClause = WhereClause;
                if (!string.IsNullOrEmpty(FilterCriteria))
                {
                    string dateTimePrepend = String.Empty;
                    String filterString = String.Empty;
                    cFieldColumn sortedFieldColumn = (cFieldColumn)SortedColumn;
                    if (sortedFieldColumn.field.FieldType == "D" || sortedFieldColumn.field.FieldType == "DT" || sortedFieldColumn.field.FieldType == "T")
                    {
                        if (!FilterCriteria.StartsWith("%"))
                        {
                            dateTimePrepend = "%";
                        }
                    }
                    if (!string.IsNullOrEmpty(query.WhereClause))
                    {
                        query.WhereClause += " AND ";
                    }
                    filterString = String.Format("{0}{1}%", dateTimePrepend, FilterCriteria);
                    query.WhereClause += "[" + sortedFieldColumn.field.GetParentTable().TableName + "].[" + sortedFieldColumn.field.FieldName + "] LIKE @filtervalue";


                    query.addFilter(new cQueryFilter(sortedFieldColumn.field, "@filtervalue", filterString));

                }
            }
            else
            {

                if (!string.IsNullOrEmpty(FilterCriteria))
                {
                    string dateTimePrepend = String.Empty;
                    String filterString = String.Empty;
                    cFieldColumn sortedFieldColumn = (cFieldColumn)SortedColumn;
                    if (sortedFieldColumn != null)
                    {
                        if (sortedFieldColumn.field.FieldType == "D" || sortedFieldColumn.field.FieldType == "DT" || sortedFieldColumn.field.FieldType == "T")
                            if (!FilterCriteria.StartsWith("%"))
                            {
                                dateTimePrepend = "%";
                            }
                        filterString = String.Format("{0}{1}%", dateTimePrepend, FilterCriteria);
                        List<object> filter = new List<object>
                                          {
                                              filterString
                                          };
                        if (sortedFieldColumn.field.GenList && sortedFieldColumn.field.ListItems.Count == 0 && sortedFieldColumn.field.FieldSource == cField.FieldSourceType.CustomEntity)
                        {
                            // 1:n field, so need to look up the display value field to filter on
                            query.addFilter(new cQueryFilter(sortedFieldColumn.LookupDisplayValueField, ConditionType.Like, filter, null, ConditionJoiner.And, sortedFieldColumn.JoinVia));
                        }
                        else
                        {
                            query.addFilter(new cQueryFilter(sortedFieldColumn.field, ConditionType.Like, filter, null, ConditionJoiner.And, sortedFieldColumn.JoinVia));
                        }
                    }
                }

                foreach (cQueryFilterString audfilter in lstAudienceFilters)
                {
                    query.addFilterString(audfilter);
                }



                if (HiddenRecords != null && HiddenRecords.Count > 0)
                {
                    cNewGridColumn keyField = getKeyFieldColumn();
                    if (keyField.ColumnType == GridColumnType.Field)
                    {
                        List<object> hiddenParams = HiddenRecords.ConvertAll(x => (object)x);

                        query.addFilter(new cQueryFilter(((cFieldColumn)keyField).field, ConditionType.DoesNotEqual, hiddenParams, null, ConditionJoiner.And, null)); // null !!!!! right on keyfield?
                    }
                }


            }

            return query.getDataset();
        }
        /// <summary>
        /// Get record count for greenlight view.
        /// </summary>
        /// <param name="view">record count for view </param>
        /// /// <param name="user">logged in user </param>
        /// <returns>count of record</returns>
        public int getViewRecordCount(ICurrentUser user, cCustomEntityView view)
        {
            List<cQueryFilter> filters = new List<cQueryFilter>();

            foreach (KeyValuePair<byte, FieldFilter> kvp in view.filters)
            {
                FieldFilter curFilter = kvp.Value;
                FieldFilters.FieldFilterValues filterValues = FieldFilters.GetFilterValuesFromFieldFilter(curFilter, user);
                filterValues.valueOne = filterValues.valueOne == null ? new object[] { string.Empty } : filterValues.valueOne;
                filters.Add(new cQueryFilter(curFilter.Field, filterValues.conditionType, filterValues.valueOne.ToList(), filterValues.valueTwo.ToList(), ConditionJoiner.And, curFilter.JoinVia));
            }
            lstCriteria = filters;
            return getRowCount();
        }

        public string[] generateGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            StringBuilder output = new StringBuilder();
            List<string> retVals = new List<string>();

            if (!bCanDisplayGrid)
            {
                output.Append("<div id=\"" + GridID + "\">");
                output.Append("<table id=\"tbl_" + GridID + "\"");
                if (!string.IsNullOrEmpty(CssClass))
                {
                    output.Append(" class=\"" + CssClass + "\"");
                }
                output.Append(">");
                output.Append("<tr><th>An error has occurred compiling the data grid</th></tr>");
                output.Append("<tr><td class=\"" + rowclass + "\">" + sGridDisplayError + "</td></tr>");
                output.Append("</table>");
                output.Append("</div>");
                retVals.Add(""); // empty element to represent the javascript array element
                retVals.Add(output.ToString());
                return retVals.ToArray();
            }
            if (clsSortedColumn == null)
            {
                clsSortedColumn = getDefaultSortableColumn(currentUser);
            }

            DataSet ds;
            nRowHiddenCount = 0;

            if (bSourceIsDataSet == true)
            {
                // ! Currently only works with single-table datasets !

                if (dsData != null)
                {
                    // Overwrite any data for this grid if new data present (generally when first loaded rather than when sorting/filtering/paging)
                    // Store full dataset
                    if (HttpContext.Current.Session != null)
                    {
                        HttpContext.Current.Session["cGridNewDataset_" + GridID] = dsData;
                    }
                }
                else
                {
                    // fetch the data from session for paging/filtering/sorting grid webmethods
                    if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session["cGridNewDataset_" + GridID] != null)
                    {
                        dsData = (DataSet)System.Web.HttpContext.Current.Session["cGridNewDataset_" + GridID];
                    }
                }

                // create a dataview for the sorted/filtered version of the dataset
                DataView dv = dsData.Tables[0].DefaultView;
                dv.RowFilter = "";

                // only filter on string type DataColumns
                if (sFilterCriteria != null)
                {
                    if (sFilterCriteria != "")
                    {
                        if (clsSortedColumn.GetType() != typeof(cStaticGridColumn))
                        {
                            string columnName = string.IsNullOrWhiteSpace(((cFieldColumn)clsSortedColumn).Alias) ? ((cFieldColumn)clsSortedColumn).field.FieldName : ((cFieldColumn)clsSortedColumn).Alias;
                            if (dsData.Tables[0].Columns[columnName] != null && dsData.Tables[0].Columns[columnName].DataType == typeof(string))
                            {
                                string suffix = "";
                                if (((cFieldColumn)clsSortedColumn).JoinVia == null && ((cFieldColumn)clsSortedColumn).field.GenList && ((cFieldColumn)clsSortedColumn).field.ListItems.Count == 0)
                                {
                                    suffix = "_text";
                                }
                                dv.RowFilter = "[" + columnName + suffix + "] LIKE '" + sFilterCriteria.Replace("%", "*") + "*'";
                            }
                            else
                            {
                                dv.RowFilter = "";
                            }
                        }
                        else
                        {
                            if (dsData.Tables[0].Columns[((cStaticGridColumn)clsSortedColumn).staticFieldName] != null && dsData.Tables[0].Columns[((cStaticGridColumn)clsSortedColumn).staticFieldName].DataType == typeof(string))
                            {
                                dv.RowFilter = "[" + ((cStaticGridColumn)clsSortedColumn).staticFieldName + "] LIKE '" + sFilterCriteria.Replace("%", "*") + "*'";
                            }
                            else
                            {
                                dv.RowFilter = "";
                            }
                        }
                    }
                }

                if (HiddenRecords != null && HiddenRecords.Count > 0)
                {
                    string sbRecIDs = getHiddenRecIdCSV;
                    string dsetFilterString = "";
                    if (dv.RowFilter != "")
                    {
                        dsetFilterString += " AND ";
                    }
                    dsetFilterString += "[" + KeyField + "] NOT IN (" + sbRecIDs + ")";
                    dv.RowFilter += dsetFilterString;
                }

                string sSort;

                if (clsSortedColumn != null && ((clsSortedColumn.GetType() == typeof(cFieldColumn) && ((cFieldColumn)clsSortedColumn).field != null) || (clsSortedColumn.GetType() == typeof(cStaticGridColumn) && ((cStaticGridColumn)clsSortedColumn).staticFieldName != "")))
                {
                    if (!String.IsNullOrEmpty(clsSortedColumn.Alias))
                    {
                        if (((cFieldColumn)clsSortedColumn).Alias != "")
                        {
                            sSort = "[" + ((cFieldColumn)clsSortedColumn).Alias + "]";
                        }
                        else
                        {
                            sSort = "[" + ((cFieldColumn)clsSortedColumn).field.FieldName + "]";
                        }
                    }
                    else
                    {
                        if (clsSortedColumn.GetType() != typeof(cStaticGridColumn))
                        {
                            sSort = "[" + ((cFieldColumn)clsSortedColumn).field.FieldName + "]";
                        }
                        else
                        {
                            sSort = "[" + ((cStaticGridColumn)clsSortedColumn).staticFieldName + "]";
                        }
                    }

                    if (eSortDirection == SpendManagementLibrary.SortDirection.Descending)
                    {
                        sSort += " DESC";
                    }
                    else
                    {
                        sSort += " ASC";
                    }
                    dv.Sort = sSort;
                }

                ds = new DataSet();
                ds.Tables.Add(dv.ToTable());
                nRowCount = ds.Tables[0].Rows.Count;

                if (bEnablePaging && nRowCount > nPageSize)
                {
                    int endrow = nCurrentPageNumber * nPageSize;
                    int startrow = endrow - nPageSize + 1;

                    if (endrow < nRowCount)
                    {
                        for (int i = nRowCount; i > endrow; i--)
                        {
                            ds.Tables[0].Rows[i - 1].Delete();
                        }
                    }
                    for (int i = (startrow - 1); i > 0; i--)
                    {
                        ds.Tables[0].Rows[i - 1].Delete();
                    }
                }
            }
            else
            {
                ds = generateDataSet();
            }

            populateRows(ds);
            output.Append("<div id=\"" + GridID + "\">");

            if (enablepaging && (PagerPosition == PagerPosition.Top || PagerPosition == PagerPosition.TopAndBottom) || (enablepaging && DisplayFilter) || (!string.IsNullOrEmpty(FilterCriteria)))
            {
                output.Append("<div id=\"" + GridID + "_header\" class=\"cgridnew-header\">");
                output.Append("<span id=\"" + GridID + "_pagerContainer\" class=\"cgridnew-pagercontainer\">");
                if (enablepaging && (PagerPosition == PagerPosition.Top || PagerPosition == PagerPosition.TopAndBottom))
                {
                    output.Append(generatePager(ds));
                }
                output.Append("</span>");
                output.Append("<span id=\"" + GridID + "_filterContainer\" class=\"cgridnew-filtercontainer\">");
                if ((enablepaging && DisplayFilter) || (!string.IsNullOrEmpty(FilterCriteria)))
                {
                    output.Append(generateFilter());
                }
                output.Append("</span>");
                output.Append("<span id=\"" + GridID + "_wait\" style=\"display: none;\"><img src=\"/shared/images/ajax-loader.gif\" alt=\"Loading, please wait\" /></span>");

                output.Append("</div>");
            }
            output.Append("<table id=\"tbl_" + GridID + "\""); //  width=\"100%\"
            if (!string.IsNullOrEmpty(CssClass))
            {
                output.Append(" class=\"" + CssClass + "\"");
            }

            if (this.Width != null)
            {
                output.Append("style=\"width: " + this.Width.ToString() + ";\"");
            }
            output.Append(">");
            if (showheaders)
            {
                output.Append(generateHeaders());
            }
            output.Append(generateTable(ds));
            if (showfooters)
            {
                output.Append(generateFooter());
            }
            output.Append("</table>");
            output.Append("<div id=\"" + GridID + "_footer\" class=\"cgridnew-footer\">");
            output.Append("<span id=\"" + GridID + "_pagerContainer\" class=\"cgridnew-pagercontainer\">");
            if (enablepaging && (PagerPosition == PagerPosition.Bottom || PagerPosition == PagerPosition.TopAndBottom))
            {
                output.Append(generatePager(ds));
            }
            output.Append("</span>");
            output.Append("</div>");
            output.Append("</div>");
            retVals.Add(generateJavascript());
            retVals.Add(output.ToString());
            return retVals.ToArray();
        }

        private string generateFooter()
        {
            StringBuilder output = new StringBuilder();
            output.Append("<tr>");
            if (EnableSelect)
            {
                output.Append("<th style='width:21px'>&nbsp;");
            }
            if (enableupdating && editlink != null)
            {
                output.Append("<th style='width:21px'>&nbsp;</th>");
            }
            if (enabledeleting)
            {
                output.Append("<th style='width:21px'>&nbsp;</th>");
            }
            if (enablearchiving)
            {
                output.Append("<th style='width:21px'>&nbsp;</th>");
            }

            foreach (cNewGridColumn column in lstColumns)
            {
                object total;

                if (!column.hidden)
                {
                    output.Append("<th");
                    if (column.ColumnType == GridColumnType.Field && ((cFieldColumn)column).field.CanTotal)
                    {
                        output.Append(" align=\"right\"");
                    }
                    output.Append(">");

                    switch (column.ColumnType)
                    {
                        case GridColumnType.Field:
                            cFieldColumn fieldcolumn = (cFieldColumn)column;
                            if (fieldcolumn.field.CanTotal)
                            {
                                total = getFooterTotal(fieldcolumn);
                                switch (fieldcolumn.field.FieldType)
                                {
                                    case "C":
                                        output.Append(string.Format(DefaultCurrencySymbol + "{0:###,###,##0.00}", total));
                                        break;
                                    case "M":
                                        output.Append(string.Format("{0:###,###,##0.00}", total));
                                        break;
                                    default:
                                        output.Append(total);
                                        break;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    output.Append("</th>");
                }
            }
            output.Append("</tr>");
            return output.ToString();
        }

        private object getFooterTotal(cFieldColumn column)
        {
            decimal total = 0;
            cNewGridCell cell;
            foreach (cNewGridRow row in Rows)
            {
                cell = row.getCellByID(column.field.FieldName);
                if (cell.Value != DBNull.Value && cell.Value != null)
                {
                    total += Convert.ToDecimal(cell.Value);
                }
            }
            return (object)total;
        }

        private void populateRows(DataSet ds)
        {
            cNewGridRow gridRow;
            List<cNewGridCell> extraCells = new List<cNewGridCell>();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    gridRow = new cNewGridRow();
                    foreach (cNewGridColumn column in lstColumns)
                    {
                        switch (column.ColumnType)
                        {
                            case GridColumnType.Field:
                                if (((cFieldColumn)column).field.GenList && ((cFieldColumn)column).field.ListItems.Count == 0 && ((cFieldColumn)column).field.FieldSource == cField.FieldSourceType.CustomEntity)
                                {
                                    column.Format.FieldType = FieldType.Text; // displaying the list item text, so override grid column type
                                    gridRow.addCell(new cNewGridCell((object)row[((cFieldColumn)column).LookupDisplayValueField.FieldName + "_text"], column));
                                }
                                else
                                {
                                    if (((cFieldColumn)column).JoinVia != null && row.Table.Columns.Contains(((cFieldColumn)column).field.FieldName))
                                    {
                                        gridRow.addCell(new cNewGridCell((object)row[((cFieldColumn)column).field.FieldName], column));
                                    }
                                    else
                                    {
                                        gridRow.addCell(new cNewGridCell((object)row[((cFieldColumn)column).Alias], column));
                                    }

                                }
                                break;
                            case GridColumnType.Static:
                                gridRow.addCell(new cNewGridCell((object)row[((cStaticGridColumn)column).staticFieldName], column));
                                break;
                            case GridColumnType.Event:
                                gridRow.addCell(new cNewGridCell(null, column));
                                break;
                            case GridColumnType.ValueIcon:
                                var value = row.ItemArray.GetValue(row.Table.Columns.IndexOf(column.ID));
                                gridRow.addCell(new cNewGridCell(value, column));
                                break;
                            case GridColumnType.TwoState:
                                {
                                    if (((cTwoStateEventColumn)column).JoinVia != null && row.Table.Columns.Contains(((cTwoStateEventColumn)column).Field.FieldName))
                                    {
                                        gridRow.addCell(new cNewGridCell((object)row[((cTwoStateEventColumn)column).Field.FieldName], column));
                                    }
                                    else
                                    {
                                        object val = (object)row[((cTwoStateEventColumn)column).Alias];
                                        cNewGridCell newCell = new cNewGridCell(val, column);
                                        gridRow.addCell(newCell);
                                    }

                                }
                                break;
                        }
                    }

                    bool bAdd = gridRow.Cells.Where(c => c.Column.GetType() == typeof(cFieldColumn)).Any(c => c.Column.hidden == false);
                    if (bAdd)
                    {
                        lstRows.Add(gridRow);
                    }
                }
            }
        }
        private string generateFilter()
        {
            StringBuilder output = new StringBuilder();
            output.Append("<span class=\"cgridnew-filter\">");
            if (SortedColumn.ColumnType == GridColumnType.Field)
            {
                output.Append("<input type=\"text\" id=\"" + GridID + "_Filter\" title=\"" + ((cFieldColumn)SortedColumn).field.Description + "\" onKeyPress=\"javascript:SEL.Grid.doClick('" + GridID + "_FilterLink', '" + GridID + "', event);\"");
            }
            else
            {
                output.Append("<input type=\"text\" id=\"" + GridID + "_Filter\" title=\"" + ((cStaticGridColumn)SortedColumn).staticFieldName + "\" onKeyPress=\"javascript:SEL.Grid.doClick('" + GridID + "_FilterLink', '" + GridID + "', event);\"");
            }
            if (FilterCriteria != null)
            {
                // The filter criteria may relate to a valuelist item, if so put the friendly value in the filter textbox rather than the number
                var column = ((cFieldColumn)SortedColumn);
                int valueListFilterCriteria;
                if (column.HasList)
                {
                    if (int.TryParse(FilterCriteria, out valueListFilterCriteria))
                    {
                        FilterCriteria = column.getValueListItem(valueListFilterCriteria);
                        if (string.IsNullOrEmpty(FilterCriteria)) { FilterCriteria = valueListFilterCriteria.ToString(); }
                    }
                    else
                    {
                        var filterValue = FilterCriteria;
                        FilterCriteria = column.getValueListItemId(FilterCriteria).ToString();
                        if (FilterCriteria == 0.ToString()) { FilterCriteria = filterValue; }
                    }

                }

                output.Append("value=\"" + FilterCriteria + "\" ");
            }
            output.Append("/>&nbsp;");
            output.Append("<a id=\"").Append(GridID).Append("_FilterLink\" href=\"javascript:SEL.Grid.filterGrid('").Append(GridID).Append("');\" style=\"border: 0px; background-color: transparent;\"><img alt=\"Filter\" src=\"/shared/images/buttons/btn_filter.png\" border=\"0\" /></a>");

            /* Eventually will use something like this but will require the dynamic css testing in other master pages
             * 
             * output.Append("<span class='smallbuttonContainer'><span class='smallbuttonInner' id='");
             * output.Append(GridID);
             * output.Append("_FilterLink");
             * output.Append("' onclick=\"javascript:SEL.Grid.filterGrid('");
             * output.Append(GridID);
             * output.Append("');\" >filter</span></span>");
             */

            output.Append("</span>");
            return output.ToString();
        }
        private string generateJavascript()
        {
            cJSGridDetail gridDetails = new cJSGridDetail();
            cJSGridColumn gridCol;

            gridDetails.GridID = sGridID;
            gridDetails.isDataSet = bSourceIsDataSet;
            if (bSourceIsDataSet)
            {
                var xmlString = this.dsData.GetXml();
                gridDetails.XmlData = xmlString.Compress();
            }
            gridDetails.gridSelectType = eSelectType;
            gridDetails.keyField = sKeyField;
            gridDetails.pageSize = nPageSize;
            gridDetails.showFooters = bShowFooters;
            gridDetails.showHeaders = bShowHeaders;
            gridDetails.enableArchiving = bEnableArchiving;
            gridDetails.enableDeleting = bEnableDeleting;
            gridDetails.enablePaging = bEnablePaging;
            gridDetails.enableSelect = bAllowSelect;
            gridDetails.enableUpdating = bEnableUpdating;
            gridDetails.deleteLink = sDeleteLink;
            gridDetails.editLink = sEditLink;
            gridDetails.archiveField = sArchiveField;
            gridDetails.archiveLink = sArchiveLink;
            gridDetails.baseTableID = clsBaseTable == null ? Guid.Empty : clsBaseTable.TableID;
            gridDetails.currencyId = nCurrencyId;
            gridDetails.currencyColumnName = sCurrencyColumnName;
            gridDetails.cssClass = sCssClass;
            gridDetails.displayFilter = bDisplayFilter;
            gridDetails.sortDirection = eSortDirection;
            gridDetails.emptyText = sEmptyText;
            gridDetails.ServiceClassForInitialiseRowEvent = sServiceClassForInitialiseRowEvent;
            gridDetails.ServiceClassMethodForInitialiseRowEvent = sServiceClassMethodForInitialiseRowEvent;
            gridDetails.InitialiseRowGridInfo = dicGridInfo;
            gridDetails.rowCount = nRowCount;
            gridDetails.rowHiddenCount = nRowHiddenCount;
            gridDetails.HiddenRecords = lstHiddenRecords;
            gridDetails.sortedColumnJoinViaID = 0;
            gridDetails.FilterCriteria = sFilterCriteria;
            gridDetails.WhereClause = WhereClause;
            gridDetails.DefaultCurrencySymbol = DefaultCurrencySymbol;
            gridDetails.WidthUnit = (byte)Width.Type;
            gridDetails.Width = this.Width.Value;
            if (EnableSorting && SortedColumn != null)
            {
                switch (SortedColumn.ColumnType)
                {
                    case GridColumnType.Field:
                        gridDetails.sortedColumnFieldID = ((cFieldColumn)SortedColumn).field.FieldID;
                        if (((cFieldColumn)SortedColumn).JoinVia != null)
                        {
                            gridDetails.sortedColumnJoinViaID = ((cFieldColumn)SortedColumn).JoinVia.JoinViaID;
                        }
                        break;
                    case GridColumnType.Static:
                        gridDetails.sortedColumnFieldID = ((cStaticGridColumn)SortedColumn).staticFieldId;
                        break;
                    default:
                        break;
                }
            }

            foreach (cNewGridColumn column in lstColumns)
            {
                gridCol = new cJSGridColumn();
                gridCol.columnType = column.ColumnType;
                gridCol.hidden = column.hidden;
                gridCol.JoinVia = null;
                gridCol.ForceSingleLine = column.ForceSingleLine;
                gridCol.CustomDateFormat = column.CustomDateFormat;
                gridCol.Alias = column.Alias;
                gridCol.PixelWidth = (int)column.Width.Value;
                switch (column.ColumnType)
                {
                    case GridColumnType.Field:
                        gridCol.fieldID = ((cFieldColumn)column).field.FieldID;

                        if (((cFieldColumn)column).JoinVia != null)
                        {
                            gridCol.JoinVia = JoinVias.ConvertCToJS(((cFieldColumn)column).JoinVia);
                        }

                        if (((cFieldColumn)column).HasList)
                        {
                            foreach (KeyValuePair<int, string> x in ((cFieldColumn)column).getValueList())
                            {
                                gridCol.valuelist.Add(x.Key.ToString(), x.Value.Replace("\"", "&quot;"));
                            }
                        }
                        gridCol.HeaderText = column.HeaderText;
                        gridCol.ID = column.ID;
                        break;
                    case GridColumnType.Event:

                        gridCol.alternateText = ((cEventColumn)column).AlternateText;
                        gridCol.hyperlinkText = ((cEventColumn)column).HyperlinkText;
                        gridCol.onClickCommand = ((cEventColumn)column).OnClickCommand;
                        gridCol.iconPath = ((cEventColumn)column).IconPath;
                        gridCol.tooltip = ((cEventColumn)column).Tooltip;
                        gridCol.ID = ((cEventColumn)column).ID;
                        break;
                    case GridColumnType.ValueIcon:

                        var eventColumn = column as ValueIconEventColumn;
                        gridCol.Options = eventColumn.Options;
                        gridCol.ValueColumn = eventColumn.ValueColumn;
                        gridCol.ID = eventColumn.ID;
                        gridCol.alternateText = eventColumn.AlternateText;
                        gridCol.HeaderText = eventColumn.HeaderText;
                        gridCol.hyperlinkText = eventColumn.HyperlinkText;
                        gridCol.iconPath = eventColumn.IconPath;
                        gridCol.onClickCommand = eventColumn.OnClickCommand;
                        gridCol.tooltip = eventColumn.Tooltip;
                        gridCol.HeaderIconPath = eventColumn.HeaderIconPath;
                        gridCol.HeaderTooltip = eventColumn.HeaderTooltip;

                        break;
                    case GridColumnType.TwoState:
                        gridCol.fieldID = ((cTwoStateEventColumn)column).Field.FieldID;

                        if (((cTwoStateEventColumn)column).JoinVia != null)
                        {
                            gridCol.JoinVia = JoinVias.ConvertCToJS(((cTwoStateEventColumn)column).JoinVia);
                        }

                        if (((cTwoStateEventColumn)column).HasList)
                        {
                            foreach (KeyValuePair<int, string> x in ((cTwoStateEventColumn)column).ValueList)
                            {
                                gridCol.valuelist.Add(x.Key.ToString(), x.Value.Replace("\"", "&quot;"));
                            }
                        }
                        gridCol.StateOneValue = ((cTwoStateEventColumn)column).StateOneValue;
                        gridCol.AlternateTextStateOne = ((cTwoStateEventColumn)column).AlternateTextStateOne;
                        gridCol.HyperlinkTextStateOne = ((cTwoStateEventColumn)column).HyperlinkTextStateOne;
                        gridCol.OnClickCommandStateOne = ((cTwoStateEventColumn)column).OnClickCommandStateOne;
                        gridCol.IconPathStateOne = ((cTwoStateEventColumn)column).IconPathStateOne;
                        gridCol.TooltipStateOne = ((cTwoStateEventColumn)column).TooltipStateOne;
                        gridCol.StateTwoValue = ((cTwoStateEventColumn)column).StateTwoValue;
                        gridCol.AlternateTextStateTwo = ((cTwoStateEventColumn)column).AlternateTextStateTwo;
                        gridCol.HyperlinkTextStateTwo = ((cTwoStateEventColumn)column).HyperlinkTextStateTwo;
                        gridCol.OnClickCommandStateTwo = ((cTwoStateEventColumn)column).OnClickCommandStateTwo;
                        gridCol.IconPathStateTwo = ((cTwoStateEventColumn)column).IconPathStateTwo;
                        gridCol.TooltipStateTwo = ((cTwoStateEventColumn)column).TooltipStateTwo;

                        gridCol.ID = ((cTwoStateEventColumn)column).ID;
                        break;
                    case GridColumnType.Static:
                        gridCol.staticFieldID = ((cStaticGridColumn)column).staticFieldId;
                        gridCol.staticFieldName = ((cStaticGridColumn)column).staticFieldName;
                        break;
                }

                gridDetails.columns.Add(gridCol);
            }

            foreach (cQueryFilter filter in lstCriteria)
            {
                cJSGridFilter filterCol;

                if (string.IsNullOrEmpty(WhereClause))
                {
                    filterCol = new cJSGridFilter
                    {
                        joiner = (byte)filter.joiner,
                        fieldID = filter.field.FieldID,
                        condition = (byte)filter.condition,
                        JoinVia = null
                    };
                }
                else
                {
                    filterCol = new cJSGridFilter { fieldID = filter.field.FieldID, ParameterName = filter.parameterName, ParameterValue = filter.parameterValue };
                }

                // date, date-time and time only filters were being serialized in a peculiar manner, so they are converted to ticks, and reconverted in the webservice.
                foreach (object x in filter.value1)
                {
                    if (filter.condition == ConditionType.LastXDays || filter.condition == ConditionType.NextXDays || filter.condition == ConditionType.LastXWeeks || filter.condition == ConditionType.NextXWeeks || filter.condition == ConditionType.LastXYears || filter.condition == ConditionType.NextXYears || filter.condition == ConditionType.NextXMonths || filter.condition == ConditionType.LastXMonths)
                    {
                        filterCol.values1.Add(Convert.ToInt32(x.ToString()));
                    }
                    else
                    {
                        if ((filter.condition == ConditionType.AtMyHierarchy || filter.condition == ConditionType.AtMyClaimsHierarchy || filter.condition == ConditionType.WithAccessRoles) && filter.value1.GetType() == typeof(List<object>))
                        {
                            var list = (List<SqlDataRecord>)filter.value1[0];
                            List<int> identifiers = list == null ? null : list.Select(record => record.GetInt32(0)).ToList();
                            filterCol.values1.Add(identifiers);
                        }
                        else
                        {
                            filterCol.values1.Add(
                                (filter.field.FieldType == "T" || filter.field.FieldType == "D"
                                 || filter.field.FieldType == "DT") && !String.IsNullOrEmpty(x.ToString().Trim())
                                    ? (Convert.ToDateTime(x)).Ticks
                                    : x);
                        }
                    }
                }

                if (filter.value2 != null)
                {
                    foreach (object x in filter.value2)
                    {
                        filterCol.values2.Add((filter.field.FieldType == "T" || filter.field.FieldType == "D" || filter.field.FieldType == "DT") && !String.IsNullOrEmpty(x.ToString().Trim()) ? (Convert.ToDateTime(x)).Ticks : x);
                    }
                }
                if (filter.JoinVia != null)
                {
                    filterCol.JoinVia = JoinVias.ConvertCToJS(filter.JoinVia);
                }
                gridDetails.filters.Add(filterCol);
            }

            return gridDetails.createJavascript();

        }

        private string generatePager(DataSet ds)
        {
            //how many pages
            int startPage;
            int pagesToDisplay;
            int numpages = (int)Math.Ceiling((decimal)RowCount / (decimal)pagesize);

            if (currentpagenumber <= 3)
            {
                startPage = 1;
                pagesToDisplay = numpages >= 5 ? 5 : numpages;
            }
            else if (currentpagenumber >= numpages - 2)
            {
                if (numpages < 5)
                {
                    startPage = 1;
                    pagesToDisplay = numpages;
                }
                else
                {
                    startPage = numpages - 4;
                    pagesToDisplay = numpages;
                }
            }
            else
            {
                startPage = currentpagenumber - 2;
                pagesToDisplay = currentpagenumber + 2;
            }

            StringBuilder output = new StringBuilder();

            // insert the prev/start paging arrows
            if (currentpagenumber > 3)
            {
                output.Append("<a href=\"javascript:SEL.Grid.changeGridPage('").Append(GridID).Append("',1);\" class=\"cgridnew-pager\">&lt;&lt;</a>");
            }
            if (currentpagenumber > 1)
            {
                output.Append("<a href=\"javascript:SEL.Grid.changeGridPage('").Append(GridID).Append("',").Append(currentpagenumber - 1).Append(");\" class=\"cgridnew-pager\">&lt;</a>");
            }

            // page links
            for (int i = startPage; i <= pagesToDisplay; i++)
            {
                if (i != currentpagenumber)
                {
                    output.Append("<a href=\"javascript:SEL.Grid.changeGridPage('").Append(GridID).Append("',").Append(i).Append(");\" class=\"cgridnew-pager\">").Append(i).Append("</a>");
                }
                else
                {
                    output.Append("<span class=\"cgridnew-pager cgridnew-currentpage\">").Append(i).Append("</span>");
                }
            }

            // insert the next/end paging arrows
            if (numpages >= 5 && currentpagenumber < numpages)
            {
                output.Append("<a href=\"javascript:SEL.Grid.changeGridPage('").Append(GridID).Append("',").Append(currentpagenumber + 1).Append(");\" class=\"cgridnew-pager\">&gt;</a>");
            }
            if (numpages >= 5 && currentpagenumber < (numpages - 2))
            {
                output.Append("<a href=\"javascript:SEL.Grid.changeGridPage('").Append(GridID).Append("',").Append(numpages).Append(");\" class=\"cgridnew-pager\">&gt;&gt;</a>");
            }

            return output.ToString();
        }

        private string generateTable(DataSet ds)
        {
            StringBuilder output = new StringBuilder();
            int displayedRows = 0;

            // WHEN MULTIPLE SUB-ACCOUNTS SUPPORTED, THIS WILL NEED PULLING FROM THE IMPORT SPREADSHEET
            cAccountSubAccounts subaccounts = new cAccountSubAccounts(accountid);
            int subAccountID = subaccounts.getFirstSubAccount().SubAccountID;
            object keyvalue;

            foreach (cNewGridRow row in Rows)
            {
                if (InitialiseRow != null)
                {
                    InitialiseRow(row, dicGridInfo);
                }

                output.Append("<tr");
                if (!string.IsNullOrEmpty(KeyField))// != null )
                {
                    keyvalue = row.getCellByID(KeyField).Value;
                    output.Append(" id=\"tbl_" + GridID + "_" + keyvalue + "\"");
                }
                else
                {
                    keyvalue = 0;
                }

                if (row.hidden)
                {
                    output.Append(" style=\"display: none;\"");
                    nRowHiddenCount++;
                }
                output.Append(">");
                var rowClass = string.Format(
                        "{0} {1}",
                        string.IsNullOrEmpty(row.CssClass) ? this.rowclass : row.CssClass,
                        row.Highlight ? "rowHighlight" : string.Empty);
                if (EnableSelect)
                {
                    output.Append("<td class=\"" + rowClass + "\"");

                    output.Append(">");
                    output.Append("<input name=\"select" + GridID + "\" type=\"");
                    if (GridSelectType == GridSelectType.CheckBox)
                    {
                        output.Append("checkbox");
                    }
                    else
                    {
                        output.Append("radio");
                    }
                    output.Append("\" value=\"" + keyvalue + "\" ");
                    if (keyvalue.GetType() == typeof(Guid))
                    {
                        if (SelectedItems.Contains((Guid)keyvalue))
                        {
                            output.Append("checked ");
                        }
                    }
                    else
                    {
                        int intKeyValue;
                        if (int.TryParse(keyvalue.ToString(), out intKeyValue))
                        {
                            if (SelectedItems.Contains(intKeyValue))
                            {
                                output.Append("checked ");
                            }
                        }
                    }
                    if (EnableSelectOnChange)
                    {
                        output.Append("onchange=\"" + parseLink(row, SelectLink) + "\" ");
                    }
                    output.Append("/>");
                    output.Append("</td>");
                }

                if (enableupdating && !string.IsNullOrEmpty(editlink))
                {
                    
                    output.Append("<td id=\"edit" + GridID + "_" + keyvalue + "\" class=\"" + rowClass + "\"");

                    output.Append(">");
                    if (row.enableupdating)
                        output.Append("<a href=\"" + parseLink(row, editlink) + "\"><img alt=\"Edit\" title=\"Edit\" src=\"/shared/images/icons/edit.png\" /></a>");
                    else
                        output.Append("&nbsp;");
                    output.Append("</td>");
                }
                if (enabledeleting)
                {
                    output.Append("<td id=\"delete" + GridID + "_" + keyvalue + "\" class=\"" + rowClass + "\"");

                    output.Append(">");
                    if (row.enabledeleting)
                        output.Append("<a href=\"" + parseLink(row, deletelink) + "\"><img alt=\"Delete\" title=\"Delete\" src=\"/shared/images/icons/delete2.png\" /></a>");
                    else
                        output.Append("&nbsp;");
                    output.Append("</td>");
                }
                if (enablearchiving)
                {
                    output.Append("<td id=\"tbl_" + GridID + "_" + keyvalue + "_archiveStatus\" class=\"" + rowClass + "\"");
                    output.Append(">");
                    if (row.enablearchiving)
                    {
                        if ((bool)row.getCellByID(ArchiveField).Value == true)
                        {
                            output.Append(
                                "<a href=\"" + parseLink(row, archivelink)
                                + "\"><img title=\"Un-Archive\" src=\"/shared/images/icons/folder_into.png\" /></a>");
                        }
                        else
                        {
                            output.Append(
                                "<a href=\"" + parseLink(row, archivelink)
                                + "\"><img title=\"Archive\" src=\"/shared/images/icons/folder_lock.png\" /></a>");
                        }
                    }
                    else
                    {
                        output.Append("&nbsp;");
                    }
                    output.Append("</td>");
                }

                CurrentUser user = cMisc.GetCurrentUser();
                var customEntitiyImageData = new CustomEntityImageData(user.AccountID);
                foreach (cNewGridCell cell in row.Cells)
                {
                    if (!cell.Column.hidden)
                    {
                        output.Append("<td class=\"" + rowClass + "\"");
                    
                        switch (cell.Column.ColumnType)
                        {
                            case GridColumnType.Static:
                                output.Append(">");
                                if (cell.Value == DBNull.Value)
                                {
                                    output.Append("&nbsp;");
                                }
                                else
                                {
                                    output.Append(cell.Value);
                                }
                                break;
                            case GridColumnType.Field:
                                switch (((cFieldColumn)cell.Column).field.FieldType)
                                {
                                    case "C":
                                    case "M":
                                    case "FD":
                                        output.Append(" align=\"right\"");
                                        break;
                                    case "X":
                                    case "FX":
                                        output.Append(" align=\"center\"");
                                        break;
                                    default:
                                        break;
                                }
                                output.Append(">");
                                if (((cFieldColumn)cell.Column).HasList)
                                {
                                    if (cell.Value != DBNull.Value)
                                    {
                                        output.Append(
                                            parseLink(
                                                row,
                                                ((cFieldColumn)cell.Column).getValueListItem(
                                                    Convert.ToInt32(cell.Value))));
                                    }
                                }
                                else
                                {
                                    if (cell.Value == DBNull.Value)
                                    {
                                        output.Append("&nbsp;");
                                    }
                                    else
                                    {
                                        DateTime dateValue=DateTime.Now;
                                        bool isdateValue=false;

                                        if (((cFieldColumn)cell.Column).field.FieldType == "D" || ((cFieldColumn)cell.Column).field.FieldType == "T" ||
                                            ((cFieldColumn)cell.Column).field.FieldType == "DT")
                                        {
                                            isdateValue = DateTime.TryParse(cell.Value.ToString(), out dateValue);
                                        }

                                        switch (((cFieldColumn)cell.Column).field.FieldType)
                                        {
                                            case "D":
                                                if (cell.Column.ForceSingleLine)
                                                {
                                                    if (isdateValue)
                                                        output.Append(dateValue.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace(" ", "&nbsp;"));
                                                }
                                                else if (!string.IsNullOrEmpty(cell.Column.CustomDateFormat))
                                                {
                                                    if (isdateValue)
                                                        output.Append(dateValue.ToString(cell.Column.CustomDateFormat));
                                                }
                                                else
                                                {
                                                    if(isdateValue)
                                                        output.Append(dateValue.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                                                }
                                                break;
                                            case "DT":
                                                var dateTimeValue = DateTime.Parse(cell.Value.ToString());
                                                output.Append(
                                                    cell.Column.ForceSingleLine
                                                        ? dateValue.ToString("dd/MM/yyyy HH:mm").Replace(" ", "&nbsp;")
                                                        : dateValue.ToString("dd/MM/yyyy HH:mm")
                                                                                );
                                                break;
                                            case "T":
                                                output.Append(dateValue.ToString("HH:mm"));
                                                break;
                                            case "X":
                                            case "FX":
                                                output.Append("<input type=\"checkbox\" disabled ");
                                                bool ischecked=false;
                                                bool.TryParse(cell.Value.ToString(), out ischecked);
                                                if (ischecked == true)
                                                {
                                                    output.Append("checked");
                                                }

                                                output.Append(">");
                                                break;
                                            case "C":
                                                string sCVal = cell.Value.ToString();
                                                decimal cVal = decimal.Parse(sCVal);
                                                cField field = ((cFieldColumn)cell.Column).field;
                                                object formattedCurr = cVal.ToString("#,###,##0.00");
                                                bool bSkipFormat = false;
                                                if (field.FieldSource == cField.FieldSourceType.CustomEntity)
                                                {
                                                    cCustomEntities entities = new cCustomEntities(user);
                                                    cCustomEntity entity = entities.getEntityByTableId(field.TableID);
                                                    if (!entity.EnableCurrencies)
                                                    {
                                                        bSkipFormat = true;
                                                    }
                                                }
                                                else
                                                {
                                                    bSkipFormat = true;
                                                    if (string.IsNullOrEmpty(cell.Format.Symbol))
                                                    {
                                                        formattedCurr =
                                                            string.Format(
                                                                DefaultCurrencySymbol + "{0:###,###,##0.00}",
                                                                cVal);
                                                    }
                                                    else
                                                    {
                                                        formattedCurr =
                                                            string.Format(
                                                                cell.Format.Symbol + "{0:###,###,##0.00}",
                                                                cVal);
                                                    }
                                                }
                                                if (!bSkipFormat)
                                                {
                                                    cCurrencies clsCurrencies = new cCurrencies(accountid, subAccountID);
                                                    cEmployees clsEmployees = new cEmployees(accountid);
                                                    Employee emp = clsEmployees.GetEmployeeById(EmployeeID);

                                                    int formatCurrencyId = emp.PrimaryCurrency;
                                                    if (CurrencyId > 0)
                                                    {
                                                        formatCurrencyId = CurrencyId;
                                                    }

                                                    if (!string.IsNullOrEmpty(CurrencyColumnName)
                                                        && row.getCellByID(CurrencyColumnName) != null)
                                                    {
                                                        if (row.getCellByID(CurrencyColumnName).Value != DBNull.Value)
                                                        {
                                                            formatCurrencyId =
                                                                (int)row.getCellByID(CurrencyColumnName).Value;
                                                        }
                                                    }

                                                    if (formatCurrencyId > 0
                                                        && clsCurrencies.getCurrencyById(formatCurrencyId) != null)
                                                    {
                                                        cCurrency currency =
                                                            clsCurrencies.getCurrencyById(formatCurrencyId);
                                                        formattedCurr = clsCurrencies.FormatCurrency(
                                                            cVal,
                                                            currency,
                                                            false);
                                                    }
                                                }
                                                output.Append(formattedCurr.ToString());
                                                break;
                                            case "M":
                                            case "FD":
                                                if (cell.Column.Format.Format == AttributeFormat.FormattedText)
                                                {
                                                    output.Append(cell.Value);
                                                }
                                                else
                                                {
                                                    string sMVal = cell.Value.ToString();
                                                    decimal mVal = decimal.Parse(sMVal);
                                                    output.Append(mVal.ToString("#,###,##0.00"));
                                                }
                                                break;
                                            case "AT": // Attachment
                                                Guid fileGuid;
                                                if (Guid.TryParse(cell.Value.ToString(), out fileGuid))
                                                {
                                                    var fileOutput =
                                                        customEntitiyImageData.GetHtmlImageData(fileGuid.ToString());
                                                    if (fileOutput != null)
                                                    {
                                                        output.Append(
                                                            string.Format(
                                                                "<a href='javascript:viewFieldLevelAttachment(\"{0}\");'>",
                                                                fileOutput.fileID));
                                                        output.Append(fileOutput.fileName);
                                                        output.Append("</a>");
                                                    }
                                                }

                                                break;
                                            case "S":
                                            case "N":
                                            case "LT": // large text
                                            default:
                                                if (cell.Column.ForceSingleLine)
                                                {
                                                    output.Append(cell.Value.ToString().Replace(" ", "&nbsp;"));
                                                }
                                                else
                                                {
                                                    output.Append(cell.Value);
                                                }
                                                break;
                                        }
                                    }
                                }
                                break;
                            case GridColumnType.Event:
                            case GridColumnType.ValueIcon:
                                output.Append(">");
                                output.Append(parseLink(row, cell.Text));
                                output.Append("</td>");
                                break;
                            case GridColumnType.TwoState:
                                output.Append("id=\"tbl_" + GridID + "_" + keyvalue + "_" + cell.Column.ID + "\"");
                                output.Append(">");
                                output.Append(parseLink(row, cell.Text));
                                output.Append("</td>");
                                break;
                            default:
                                break;
                        }
                        output.Append("</td>");
                    }
                }
                output.Append("</tr>");
                if (!row.hidden)
                {
                    // don't bother to alternate colour if this row is hidden - keeps table display consistent
                    sRowClass = sRowClass == "row1" ? "row2" : "row1";
                    displayedRows++;
                }
            }

            if (displayedRows == 0)
            {
                // output the "emptytext" message as no data to display
                int colspan = 0;

                output.Append("<tr id=\"tbl_" + GridID + "_emptytext\">");

                if (enableupdating && editlink != null)
                {
                    colspan++;
                }
                if (enabledeleting)
                {
                    colspan++;
                }
                if (enablearchiving)
                {
                    colspan++;
                }
                if (EnableSelect)
                {
                    colspan++;
                }
                int columnindex;
                foreach (cNewGridColumn column in lstColumns)
                {
                    columnindex = lstColumns.IndexOf(column);
                    if (!column.hidden)
                    {
                        colspan++;
                    }
                }

                output.Append("<td class=\"" + rowclass + "\" colspan=\"" + colspan.ToString() + "\" align=\"center\">");
                output.Append(EmptyText);
                output.Append("</td></tr>");
            }
            return output.ToString();
        }

        private int getTableWidth()
        {
            int width = 0;
            foreach (cNewGridColumn column in lstColumns)
            {
                if (!column.hidden)
                {
                    switch (column.ColumnType)
                    {
                        case GridColumnType.Field:
                            cFieldColumn fieldcolumn = (cFieldColumn)column;
                            switch (fieldcolumn.field.FieldType)
                            {
                                case "D":
                                    width += 75;
                                    break;
                                case "C":
                                case "M":
                                    width += 50;
                                    break;
                                case "N":
                                    width += 50;
                                    break;
                                case "S":
                                    if (fieldcolumn.field.Width > 0)
                                    {
                                        width += fieldcolumn.field.Width;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }

            return width;
        }

        private string generateHeaders()
        {
            StringBuilder output = new StringBuilder();
            output.Append("<tr>");
            if (EnableSelect)
            {
                if (GridSelectType == GridSelectType.RadioButton)
                {
                    output.Append("<th>&nbsp;</th>");
                }
                else
                {
                    output.Append("<th style=\"width: 10px;\"><input type=\"checkbox\" id=\"selectAll" + GridID + "\" onclick=\"SEL.Grid.selectAllOnGrid('" + GridID + "');\" /></th>");
                }
            }
            if (enableupdating && editlink != null)
            {
                output.Append("<th style='width:21px'><img alt=\"Edit\" src=\"/shared/images/icons/edit_blue.gif\" /></th>"); //  style=\"max-width:20px;\"
            }
            if (enabledeleting)
            {
                output.Append("<th style='width:21px'><img alt=\"Delete\" src=\"/shared/images/icons/delete2_blue.gif\" /></th>"); //  style=\"max-width:20px;\"
            }
            if (enablearchiving)
            {
                output.Append("<th style='width:21px'><img alt=\"Archive / Un-Archive\" src=\"/shared/images/icons/folder_lock_blue.gif\" /></th>"); //  style=\"max-width:20px;\"
            }

            //int columnIncrement = 0;

            int noneHiddenColumnCount = lstColumns.Count(column => !column.hidden);

            if (noneHiddenColumnCount > 0)
            {
                foreach (cNewGridColumn column in lstColumns)
                {
                    if (!column.hidden)
                    {
                        output.Append("<th");
                        if ((column.ColumnType == GridColumnType.TwoState) || (column.ColumnType == GridColumnType.Event))
                        {
                            output.Append(" style=\"width: 21px;\"");
                        }

                        if (column.Width != null)
                        {
                            output.Append(" style=\"width:" + column.Width.ToString() + ";\"");
                        }

                        output.Append(">");
                        // for use when user story is forthcoming output.Append("<th id=\"").Append("gridHeaders_").Append(GridID).Append("_").Append(columnIncrement++).Append("\">");
                        if (EnableSorting)
                        {
                            switch (column.ColumnType)
                            {
                                case GridColumnType.Field:
                                    //output.Append("<a href=\"javascript:SEL.Grid.sortGrid('" + GridID + "','" + ((cFieldColumn)column).field.FieldID + "',0);\">");
                                    output.Append("<a href=\"javascript:SEL.Grid.sortGrid('" + GridID + "','" +
                                                  ((cFieldColumn)column).ID + "',0);\">");
                                    break;
                                case GridColumnType.Static:
                                    output.Append("<a href=\"javascript:SEL.Grid.sortGrid('" + GridID + "','" +
                                                  ((cStaticGridColumn)column).staticFieldName + "',1);\">");
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (column.ColumnType == GridColumnType.Static)
                        {
                            output.Append(((cStaticGridColumn)column).staticFieldName);
                        }
                        else
                        {
                            if (column.ColumnType == GridColumnType.Field)
                            {
                                bool addDescription = false;
                                cFieldColumn fieldColumn = (cFieldColumn)column;
                                if (fieldColumn.JoinVia != null)
                                {
                                    output.Append(fieldColumn.JoinVia.RuntimeDescription(accountid));
                                    addDescription = true;
                                }
                                else
                                {
                                    output.Append(column.HeaderText);
                                }
                                if (addDescription)
                                {
                                    output.Append(String.Format(" {0}", fieldColumn.field.Description));
                                }
                            }
                            else if (column.ColumnType == GridColumnType.TwoState)
                            {
                                var twoState = (cTwoStateEventColumn)column;
                                if (!string.IsNullOrEmpty(twoState.IconPathStateOne))
                                {
                                    output.Append(string.Format("<img alt=\"{0}\" src=\"{1}\" />",
                                        twoState.AlternateTextStateOne, twoState.IconPathStateOne));
                                }
                                else
                                {
                                    output.Append(string.Format("<img alt=\"{0}\" src=\"{1}\" />",
                                        twoState.AlternateTextStateTwo, twoState.IconPathStateTwo));
                                }
                            }
                            else if (column.ColumnType == GridColumnType.ValueIcon)
                            {
                                var iconColumn = column as ValueIconEventColumn;
                                output.Append(iconColumn.HeaderText);
                            }
                            else
                            {
                                output.Append(column.HeaderText);
                            }
                        }
                        if (EnableSorting)
                        {
                            if (column.ID == clsSortedColumn.ID)
                            {
                                if (SortDirection == SpendManagementLibrary.SortDirection.Ascending)
                                {
                                    output.Append("&nbsp;<img src=\"" + cMisc.Path + "/shared/images/whitearrow_up.gif\" />");
                                }
                                else
                                {
                                    output.Append("&nbsp;<img src=\"" + cMisc.Path + "/shared/images/whitearrow_down.gif\" />");
                                }
                            }
                            output.Append("</a>");
                        }
                        output.Append("</th>");
                    }
                }
            }
            else
            {
                output.Append("<th>");
                output.Append("</th>");

            }



            output.Append("</tr>");
            return output.ToString();
        }

        /// <summary>
        /// Returns a column of the grid 
        /// - JoinVia safe
        /// </summary>
        /// <param name="id">The actual field name as it is stored in the database - unless JoinVia'd in which case it's the fieldIDguid_JoinViaID</param>
        /// <returns>Grid column</returns>
        public cNewGridColumn getColumnByName(string id)
        {
            foreach (cNewGridColumn column in lstColumns)
            {
                switch (column.ColumnType)
                {
                    case GridColumnType.Static:
                        if (((cStaticGridColumn)column).staticFieldName.ToLower() == id.ToLower())
                        {
                            return column;
                        }
                        break;
                    case GridColumnType.Field:
                        if (((cFieldColumn)column).JoinVia == null && ((cFieldColumn)column).field != null && ((cFieldColumn)column).field.FieldName != null && ((cFieldColumn)column).field.FieldName.ToLower() == id.ToLower())
                        {
                            return column;
                        }
                        else if (((cFieldColumn)column).ID.ToLower() == id.ToLower())
                        {
                            return column;
                        }
                        break;
                    case GridColumnType.Event:
                        if (((cEventColumn)column).ID.ToLower() == id.ToLower())
                        {
                            return column;
                        }
                        break;
                    case GridColumnType.TwoState:
                        if (((cTwoStateEventColumn)column).ID.ToLower() == id.ToLower())
                        {
                            return column;
                        }
                        break;
                    default:
                        break;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a column of the grid 
        /// - JoinVia safe
        /// </summary>
        /// <param name="id">Field Id to obtain matching column for</param>
        /// <param name="joinViaID">default 0 for no joinvia check</param>
        /// <returns>Grid column</returns>
        public cNewGridColumn getColumnByID(Guid id, int joinViaID = 0)
        {
            foreach (cNewGridColumn column in lstColumns)
            {
                switch (column.ColumnType)
                {
                    case GridColumnType.Field:
                        if (((cFieldColumn)column).field.FieldID == id &&
                            (joinViaID == 0 || (((cFieldColumn)column).JoinVia != null && ((cFieldColumn)column).JoinVia.JoinViaID == joinViaID)))
                        {
                            return column;
                        }
                        break;
                    case GridColumnType.Static:
                        if (((cStaticGridColumn)column).staticFieldId == id)
                        {
                            return column;
                        }
                        break;
                    default:
                        break;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a column of the grid
        /// </summary>
        /// <param name="index">Column index to retrieve</param>
        /// <returns>Grid column</returns>
        public cNewGridColumn getColumnByIndex(int index)
        {
            int i = 0;
            foreach (cNewGridColumn column in lstColumns)
            {
                if (i == index)
                {
                    return column;
                }
                i++;
            }
            return null;
        }

        public cNewGridColumn getDefaultSortableColumn(ICurrentUser user)
        {
            //does the employee have an order?
            cEmployees clsEmployees = new cEmployees(accountid);
            cNewGridColumn column = null;

            cNewGridSort sort = user.Employee.GetNewGridSortOrders().GetBy(GridID);
            if (sort != null)
            {
                SortDirection = sort.SortDirection;
                column = getColumnByID(sort.SortedColumn, sort.JoinViaID);
            }

            if (column == null)
            {
                foreach (cNewGridColumn sortcolumn in lstColumns)
                {
                    if (!sortcolumn.hidden && sortcolumn.ColumnType == GridColumnType.Field)
                    {
                        column = sortcolumn;
                        break;
                    }
                }

                if (column == null && lstColumns.Count > 0)
                {
                    // Set the column as the first Field column in the list of columns                    
                    column = lstColumns.FirstOrDefault(x => x.ColumnType == GridColumnType.Field);

                    // if gets here must still be null, so as last resort set to first column regardless
                    // this should only be hit if the view has no standard fields       
                    if (column == null) column = lstColumns[0];
                }
            }
            return column;
        }

        #region utility functions
        private void getColumns(string strsql)
        {
            var clsfields = new cFields(accountid);
            var clsTables = new cTables(accountid);
            cField field;
            string tablename, fieldname;
            if (strsql.IndexOf("*") == -1)
            {
                int startindex = 7;
                int endindex = strsql.ToLower().IndexOf("from ");
                string tmpfields = strsql.Substring(startindex, endindex - startindex - 1);
                string[] arrFields = tmpfields.Split(',');
                foreach (string s in arrFields)
                {
                    if (!s.Trim().StartsWith("dbo.") && s.Contains(".")) //table name specified
                    {
                        string[] seperator = new string[] { "." };
                        string[] result = s.Split(seperator, StringSplitOptions.None);
                        tablename = result[0].ToString().Trim();
                        fieldname = result[1].ToString().Trim();
                    }
                    else
                    {
                        tablename = clsBaseTable.TableName;
                        fieldname = s.Trim();
                    }
                    // strip out any []
                    fieldname = fieldname.Replace("[", "").Replace("]", "");
                    tablename = tablename.Replace("[", "").Replace("]", "");
                    var table = clsTables.GetTableByName(tablename);
                    field = clsfields.GetBy(table.TableID, fieldname);
                    if (field != null)
                    {
                        lstColumns.Add(new cFieldColumn(field));
                    }
                }
            }
        }
        private cTable getBaseTable(string sql)
        {
            cTables clstables = new cTables(accountid);
            int startindex;
            int endindex;
            //get the table
            // strip out dbo. from table definition as this prevents correct detection
            sql = sql.ToLower().Replace("[dbo].", "");
            sql = sql.ToLower().Replace("dbo.", "");

            startindex = sql.IndexOf("from ") + 5;
            if (sql[startindex] == '[')
            {

                endindex = sql.IndexOf("]", startindex);
                startindex++;
            }
            else
            {
                endindex = sql.IndexOf(" ", startindex);
            }
            if (endindex == -1)
            {
                endindex = sql.Length;
            }
            string table = sql.Substring(startindex, endindex - startindex);
            return clstables.GetTableByName(table);
        }
        private string parseLink(cNewGridRow row, string link)
        {
            if (string.IsNullOrEmpty(link) == false)
            {
                foreach (cNewGridColumn column in lstColumns)
                {
                    switch (column.ColumnType)
                    {
                        case GridColumnType.Field:
                            string linkValue = "";
                            if (row.getCellByID(((cFieldColumn)column).ID).Value.GetType() == typeof(string) &&
                                ((cFieldColumn)column).Format.FieldType == FieldType.Integer && ((cFieldColumn)column).field != null && ((cFieldColumn)column).field.ValueList)
                            {
                                // if summary grid from custom entities (or dataset) then text value may exist in the cell rather than the number and it's valuelist values


                                foreach (KeyValuePair<object, string> kvp in ((cFieldColumn)column).field.ListItems)
                                {
                                    if (kvp.Value == row.getCellByID(((cFieldColumn)column).ID).Value.ToString())
                                    {
                                        linkValue = kvp.Value.ToString();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                linkValue = row.getCellByID(((cFieldColumn)column).Alias).Text.ToString().Replace("'", "\'");
                            }
                            if (((cFieldColumn)column).field == null || ((cFieldColumn)column).field.FieldName == null)
                            {
                                link = link.Replace("{" + ((cFieldColumn)column).ID + "}", linkValue);
                            }
                            else
                            {
                                link = link.Replace("{" + ((cFieldColumn)column).Alias + "}", linkValue);
                            }

                            break;
                        case GridColumnType.Static:
                            string staticColValue = row.getCellByID(((cStaticGridColumn)column).ID).Text.ToString().Replace("'", "\'");

                            link = link.Replace("{" + ((cStaticGridColumn)column).staticFieldName + "}", staticColValue);
                            break;
                        default:
                            break;
                    }
                }
            }
            return link;
        }

        /// <summary>
        /// Returns a comma separated list of record IDs specified for hiding (eg. audience)
        /// </summary>
        /// <returns>Comma separate string of int</returns>
        private string getHiddenRecIdCSV
        {
            get
            {
                StringBuilder sbRecIDs = new StringBuilder();
                if (HiddenRecords != null && HiddenRecords.Count > 0)
                {
                    string comma = "";
                    foreach (int recID in HiddenRecords)
                    {
                        sbRecIDs.Append(comma + recID.ToString());
                        comma = ",";
                    }
                }
                return sbRecIDs.ToString();
            }
        }

        private int getRowCount(string sql)
        {

            int startindex = 7;
            int endindex;
            //get the table
            sql = sql.ToLower();
            endindex = sql.IndexOf("from");
            sql = sql.Replace(sql.Substring(startindex, endindex - startindex), "count(*)");
            cQueryBuilder builder = new cQueryBuilder(accountid, connectionstring, metabaseconnectionstring, basetable, new cTables(accountid), new cFields(accountid));
            builder.addColumn(this.basetable.GetPrimaryKey(), SelectType.Count);
            return builder.GetCount();
        }
        private int getRowCount()
        {
            cQueryBuilder builder = new cQueryBuilder(accountid, connectionstring, metabaseconnectionstring, basetable, new cTables(accountid), new cFields(accountid));
            builder.addColumn(this.basetable.GetPrimaryKey(), SelectType.Count);


            //remove @filtervalue

            foreach (cQueryFilter filter in lstCriteria)
            {

                builder.addFilter(filter);

            }
            if (!string.IsNullOrEmpty(WhereClause))
            {
                builder.WhereClause = WhereClause;
                if (!string.IsNullOrEmpty(FilterCriteria))
                {
                    string dateTimePrepend = String.Empty;
                    String filterString = String.Empty;
                    cFieldColumn sortedFieldColumn = (cFieldColumn)SortedColumn;
                    if (sortedFieldColumn.field.FieldType == "D" || sortedFieldColumn.field.FieldType == "DT" || sortedFieldColumn.field.FieldType == "T")
                    {
                        if (!FilterCriteria.StartsWith("%"))
                        {
                            dateTimePrepend = "%";
                        }
                    }
                    if (!string.IsNullOrEmpty(builder.WhereClause))
                    {
                        builder.WhereClause += " AND ";
                    }
                    filterString = String.Format("{0}{1}%", dateTimePrepend, FilterCriteria);
                    builder.WhereClause += "[" + sortedFieldColumn.field.GetParentTable().TableName + "].[" + sortedFieldColumn.field.FieldName + "] LIKE @filtervalue";



                    builder.addFilter(new cQueryFilter(sortedFieldColumn.field, "@filtervalue", filterString));
                    //List<object> filter = new List<object>
                    //{
                    //     filterString
                    //};
                    //builder.addFilter(new cQueryFilter(sortedFieldColumn.field, ConditionType.Like, filter, null, ConditionJoiner.And, sortedFieldColumn.JoinVia));
                }
            }
            else
            {

                if (!string.IsNullOrEmpty(FilterCriteria))
                {
                    string dateTimePrepend = String.Empty;
                    String filterString = String.Empty;
                    cFieldColumn sortedFieldColumn = (cFieldColumn)SortedColumn;
                    if (sortedFieldColumn.field.FieldType == "D" || sortedFieldColumn.field.FieldType == "DT" || sortedFieldColumn.field.FieldType == "T")
                    {
                        if (!FilterCriteria.StartsWith("%"))
                        {
                            dateTimePrepend = "%";
                        }
                    }
                    filterString = String.Format("{0}{1}%", dateTimePrepend, FilterCriteria);
                    List<object> filter = new List<object>
                                          {
                                              filterString
                                          };
                    builder.addFilter(new cQueryFilter(sortedFieldColumn.field, ConditionType.Like, filter, null, ConditionJoiner.And, sortedFieldColumn.JoinVia));
                }
                if (HiddenRecords != null && HiddenRecords.Count > 0)
                {
                    cNewGridColumn keyColumn = getKeyFieldColumn();
                    if (keyColumn != null)
                    {
                        if (keyColumn.ColumnType == GridColumnType.Field)
                        {
                            List<object> hiddenRecParams = HiddenRecords.ConvertAll(x => (object)x);
                            builder.addFilter(new cQueryFilter(((cFieldColumn)keyColumn).field, ConditionType.DoesNotEqual, hiddenRecParams, null, ConditionJoiner.And, null)); // keyfield always on basetable so null? !!!!!!!
                        }
                    }
                }
            }
            return builder.GetCount();
        }

        /// <summary>
        /// If there is a claim id on the row, then we can check for bank account details and if the claim owner is not the current user, we can redact the details.
        /// </summary>
        /// <param name="row">the grid row to investigate</param>
        /// <param name="claimId">the claim id as an object</param>
        public static void CheckClaimRowForBankAccountRedaction(cNewGridRow row, object claimId)
        {
            // lets redact possible bank account details if the user is not the claim owner
            var sortCodeCell = row.getCellByID("dbo.getDecryptedValue(SortCode)") ?? row.getCellByID("ac1624ea-2f5f-4427-9555-12f2db7d0e52") ?? row.getCellByID("C505A9A1-B5E2-4FC0-AB45-C06BFE3330E8");
            var accountNumberCell = row.getCellByID("dbo.getDecryptedValue(AccountNumber)") ?? row.getCellByID("dda2c25b-d573-4d89-979d-5d1509917d4b") ?? row.getCellByID("d9b95e19-c476-41da-b159-bbfa573b2764");
            var accountNameCell = row.getCellByID("dbo.getDecryptedValue(AccountName)") ?? row.getCellByID("ad3452ff-c8f1-4353-8a6b-a7ac82554994") ?? row.getCellByID("834E897B-D639-4FC8-88F4-C66C3B28DD7A");
            var referenceCell = row.getCellByID("dbo.getDecryptedValue(Reference)") ?? row.getCellByID("4c6ff468-a5b5-41e6-94f5-fb2cc7770889") ?? row.getCellByID("8E6ABBC8-F081-45A8-B1EA-F2908F0CF7EF");

            if (sortCodeCell == null && accountNumberCell == null && accountNameCell == null && referenceCell == null)
            {
                return;
            }

            // we may have something to redact
            CurrentUser user = cMisc.GetCurrentUser();
            var claims = new cClaims(user.AccountID);
            int numericClaimId;

            if (!int.TryParse(claimId.ToString(), out numericClaimId))
            {
                // no claim id, no bank details
                return;
            }
            cClaim claim = claims.getClaimById(numericClaimId);

            if (!user.isDelegate && claim.employeeid == user.EmployeeID)
            {
                // all good, we have the claim owner
                return;
            }

            //naughty naughty, not the claim owner, lets redact (if the columns are there)
            const int CharsToKeep = 2;

            if (sortCodeCell != null)
            {
                sortCodeCell.Value = string.IsNullOrEmpty(sortCodeCell.Text) ? string.Empty : SpendManagementLibrary.Account.BankAccount.GetRedactedValues(sortCodeCell.Text, CharsToKeep);
            }

            if (accountNumberCell != null)
            {
                accountNumberCell.Value = string.IsNullOrEmpty(accountNumberCell.Text) ? string.Empty : SpendManagementLibrary.Account.BankAccount.GetRedactedValues(accountNumberCell.Text, CharsToKeep);
            }

            if (accountNameCell != null)
            {
                accountNameCell.Value = string.IsNullOrEmpty(accountNameCell.Text) ? string.Empty : SpendManagementLibrary.Account.BankAccount.GetRedactedValues(accountNameCell.Text, CharsToKeep);
            }

            if (referenceCell != null)
            {
                referenceCell.Value = string.IsNullOrEmpty(referenceCell.Text)
                    ? string.Empty
                    : SpendManagementLibrary.Account.BankAccount.GetRedactedValues(
                        referenceCell.Text,
                        CharsToKeep);
            }
        }

        /// <summary>
        /// Returns the column in the collection identified by the KeyField property
        /// </summary>
        /// <returns>cNewGridColumn in grid that represents the key field. NULL returned if not found</returns>
        public cNewGridColumn getKeyFieldColumn()
        {
            cNewGridColumn retColumn = null;

            foreach (cNewGridColumn col in lstColumns)
            {
                switch (col.ColumnType)
                {
                    case GridColumnType.Field:
                        // via-joined fields probably shouldn't be able to be a key
                        if (((cFieldColumn)col).field.FieldName == KeyField && ((cFieldColumn)col).JoinVia == null)
                            retColumn = col;
                        break;
                    case GridColumnType.Static:
                        if (((cStaticGridColumn)col).staticFieldName == KeyField)
                            retColumn = col;
                        break;
                    default:
                        break;
                }

                if (retColumn != null)
                    break;
            }
            return retColumn;
        }

        public void updateEmployeeSortOrder(CurrentUser user)
        {
            int joinViaID = (((cFieldColumn)SortedColumn).JoinVia != null) ? ((cFieldColumn)SortedColumn).JoinVia.JoinViaID : 0;

            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            data.sqlexecute.Parameters.AddWithValue("@employeeid", EmployeeID);
            data.sqlexecute.Parameters.AddWithValue("@gridid", GridID);
            data.sqlexecute.Parameters.AddWithValue("@sortedcolumn", ((cFieldColumn)SortedColumn).field.FieldID);
            data.sqlexecute.Parameters.AddWithValue("@sortorder", (byte)SortDirection);
            if (joinViaID > 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@sortJoinViaID", joinViaID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@sortJoinViaID", DBNull.Value);
            }
            data.ExecuteProc("updateGridSortOrder");
            data.sqlexecute.Parameters.Clear();
            user.Employee.GetNewGridSortOrders().Add(GridID, new cNewGridSort(GridID, ((cFieldColumn)SortedColumn).field.FieldID, SortDirection, joinViaID));
        }
        #endregion

        /// <summary>
        /// Adds a filter to the grid
        /// </summary>
        /// <param name="field">The field to filter by</param>
        /// <param name="condition">The condition that should be applied to the filter</param>
        /// <param name="value1">The first value</param>
        /// <param name="value2">The 2nd value</param>
        /// <param name="joiner"></param>
        public void addFilter(cField field, ConditionType condition, object[] value1, object[] value2, ConditionJoiner joiner, JoinVia joinVia = null)
        {
            List<object> lstValue1 = new List<object>();
            List<object> lstValue2 = new List<object>();

            if (value1 != null)
            {
                foreach (object o in value1)
                {
                    lstValue1.Add(o);
                }
            }
            if (value2 != null)
            {
                foreach (object o in value2)
                {
                    lstValue2.Add(o);
                }
            }
            cQueryFilter filter = new cQueryFilter(field, condition, lstValue1, lstValue2, joiner, joinVia);
            lstCriteria.Add(filter);
        }

        public void addFilter(cField field, string parameterName, object value)
        {
            cQueryFilter filter = new cQueryFilter(field, parameterName, value);
            lstCriteria.Add(filter);
        }
        /// <summary>
        /// Removes ALL filters applied to the grid. (Used primarily to flush occurrences of 'archived' in filterGridData
        /// </summary>
        /// <param name="filterField">Field definition to remove all occurrences of</param>
        public void clearFiltersForField(cField filterField)
        {
            List<cQueryFilter> newList = new List<cQueryFilter>();
            foreach (cQueryFilter curFilter in lstCriteria)
            {
                if (curFilter.field.FieldID != filterField.FieldID)
                {
                    newList.Add(curFilter);
                }
            }
            lstCriteria = newList;
        }

        //public void addAudienceFilter(Guid baseTableid, Guid audienceTableId, string parentField, int employeeid)
        //{
        //    cTables clsTables = new cTables(accountid);
        //    cTable audienceTable = clsTables.getTableById(audienceTableId);
        //    cTable baseTable = clsTables.getTableById(baseTableid);
        //    cQueryFilterString filterString;
        //    filterString = new cQueryFilterString("(((select count([id]) from [" + audienceTable.tablename + "] where " + baseTable.tablename + "." + parentField + " = " + audienceTable.tablename + ".parentID) > 0) AND (select sum(dbo.AudienceMemberCount(" + audienceTable.tablename + ".audienceID, " + EmployeeID.ToString() + ")) from " + audienceTable.tablename + " where " + baseTable.tablename + "." + parentField + " = " + audienceTable.tablename + ".parentID) > 0)", ConditionJoiner.And);
        //    //filterString = new cQueryFilterString("(((select count([id]) from [" + clsTables.getTableById(audienceTableId).tablename + "]) > 0 AND (dbo.AudienceMemberCount(" + parentTableDotField + ", " + EmployeeID.ToString() + ") = 0)", ConditionJoiner.Or);
        //    lstAudienceFilters.Add(filterString);
        //    filterString = new cQueryFilterString("((select count([id]) from [" + audienceTable.tablename + "] where " + baseTable.tablename + "." + parentField + " = " + audienceTable.tablename + ".parentID) = 0)", ConditionJoiner.Or);
        //    //filterString = new cQueryFilterString("(select count([id]) from [" + clsTables.getTableById(audienceTableId).tablename + "]) = 0)", ConditionJoiner.None);
        //    lstAudienceFilters.Add(filterString);
        //}

        #region cEventColumn_functions
        /// <summary>
        /// addEventColumn: Adds additional icon columns to appear immediately after edit, delete, archive icon columns
        /// </summary>
        /// <param name="id">Event column ID</param>
        /// <param name="iconPath">Path of icon to display in column</param>
        /// <param name="onclickCommand">Onclick command text</param>
        /// <param name="alternate_text">Alternative text to display if image not available</param>
        /// <param name="tooltip">Tooltip to appear when icon hovers over the event button</param>
        public void addEventColumn(string id, string iconPath, string onclickCommand, string alternate_text, string tooltip)
        {
            lstColumns.Insert(0, new cEventColumn(id, iconPath, onclickCommand, alternate_text, tooltip));
        }

        /// <summary>
        /// addEventColumn: Adds additional icon columns to appear immediately after edit, delete, archive icon columns
        /// </summary>
        /// <param name="id">Event column ID</param>
        /// <param name="hyperlinkText">Text to display for the hyperlink</param>
        /// <param name="onclickCommand">Command to execute when link is clicked</param>
        /// <param name="tooltip">Tooltip to display on hover</param>
        public void addEventColumn(string id, string hyperlinkText, string onclickCommand, string tooltip)
        {
            lstColumns.Insert(0, new cEventColumn(id, hyperlinkText, onclickCommand, tooltip));
        }

        /// <summary>
        /// Adds an icon column that changes the icon, tooltip and state based on the value of it's reference field column.
        /// </summary>
        /// <param name="id">The named Id of the column.</param>
        /// <param name="valueColumn">The reference column from which to grab the field value.</param>
        /// <param name="headerIconPath">The path of the icon to give the column header.</param>
        /// <param name="headerTooltip">The tooltip to give the header icon.</param>
        /// <param name="defaultIconPath">The path of the icon to use when the value does not have a match in <see cref="options"/>.</param>
        /// <param name="defaultTooltip">The tooltip to use (img.alt and a.title) when the value does not have a match in <see cref="options"/>.</param>
        /// <param name="defaultOnClick">The a.onClick behaviour to use when the value does not have a match in <see cref="options"/>.</param>
        /// <param name="options">
        /// A list of ValueBasedIconEventColumnOptions. 
        /// During the generation of cells, the value will be grabbed from the reference column.
        /// This list will be checked for an item that has a matching value, and if there is one, 
        /// its properties will be used to define the generated markup. If a match is not found,
        /// this column will fall back to the defaults provided.
        /// </param>
        public void AddValueIconColumn(string id, cFieldColumn valueColumn, string headerIconPath, string headerTooltip, string defaultIconPath, string defaultTooltip, string defaultOnClick, List<ValueIconEventColumnOptions> options)
        {
            lstColumns.Insert(0, new ValueIconEventColumn(id, valueColumn, headerIconPath, headerTooltip, defaultIconPath, defaultTooltip, defaultOnClick, options));
        }

        /// <summary>
        /// Add a new TwoStateEvent column o the grid.  This takes a value from a pre-exisiting column to determine the actions available.
        /// </summary>
        /// <param name="id"></param> Column 'id' to create
        /// <param name="ValueColumn"></param> The Column containing the values to compare
        /// <param name="StateOneValue"></param> The Value relating to StateOne
        /// <param name="StateTwoValue"></param> The Value relating to StateTwo
        /// <param name="iconPathStateOne"></param> The Icon path to use when ValueColumn = StateOneValue
        /// <param name="onclickCommandStateOne"></param> The OnClick command to use when ValueColumn = StateOneValue
        /// <param name="alternate_textStateOne"></param> The alternate text to use for StateOne
        /// <param name="tooltipStateOne"></param> The tooltip for StateOne
        /// <param name="iconPathStateTwo"></param> The Icon path to use when ValueColumn = StageTwoValue
        /// <param name="onclickCommandStateTwo"></param> The OnClick command to use when ValueColumn = StateTwoValue
        /// <param name="alternate_textStateTwo"></param> The alternate text to use for StateTwo
        /// <param name="tooltipStateTwo"></param> TheTooltip to use for StateTwo
        public void addTwoStateEventColumn(string id, cFieldColumn ValueColumn, object StateOneValue, object StateTwoValue, string iconPathStateOne, string onclickCommandStateOne, string alternate_textStateOne, string tooltipStateOne, string iconPathStateTwo, string onclickCommandStateTwo, string alternate_textStateTwo, string tooltipStateTwo, bool removeValueColumn = true)
        {
            cTwoStateEventColumn newColumn = null;
            if (ValueColumn != null)
            {
                foreach (cNewGridColumn column in lstColumns)
                {
                    if (ValueColumn.ID == column.ID)
                    {
                        newColumn = cTwoStateEventColumn.FromValueColumn(id, (cFieldColumn)column, StateOneValue, StateTwoValue, iconPathStateOne, onclickCommandStateOne, alternate_textStateOne, tooltipStateOne, iconPathStateTwo, onclickCommandStateTwo, alternate_textStateTwo, tooltipStateTwo);
                        if (removeValueColumn)
                        {
                            lstColumns.Remove(column);
                        }

                        lstColumns.Insert(0, newColumn);
                        break;
                    }
                }
            }

            if (newColumn == null)
            {
                lstColumns.Insert(0, new cTwoStateEventColumn(ValueColumn, StateOneValue, StateTwoValue, tooltipStateOne, onclickCommandStateOne, iconPathStateOne, alternate_textStateOne, alternate_textStateOne, true, tooltipStateTwo, onclickCommandStateTwo, iconPathStateTwo, alternate_textStateTwo, alternate_textStateTwo, true));
            }

        }
        #endregion

        public static string generateJS_init(string jsBlockID, List<string> gridJSObjects, Modules activeModuleID)
        {
            StringBuilder sbJS = new StringBuilder();

            // set the sel.grid javascript variables
            cModules clsmodules = new cModules();

            sbJS.Append("(function() {\n");
            sbJS.Append("var " + jsBlockID + " = function () {\n");
            sbJS.Append("SEL.Grid.applicationName = \"" + clsmodules.GetModuleByID((int)activeModuleID).BrandNamePlainText + "\";\n");

            foreach (string s in gridJSObjects)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    sbJS.Append("SEL.Grid.updateGrid('" + s + "');\n");
                }
            }
            sbJS.Append("};\n");
            sbJS.Append("Sys.Application.remove_init(" + jsBlockID + ");");
            //sbJS.Append(jsBlockID + ".registerClass(\"" + jsBlockID + "\", Sys.Component);\n");
            sbJS.Append("Sys.Application.add_init(\n");
            sbJS.Append("\tfunction(){\n");
            sbJS.Append("\t\t$create(" + jsBlockID + ", {\"id\":\"" + jsBlockID + "\"}, null, null, null);\n");
            sbJS.Append("\t}\n");
            sbJS.Append(");\n");

            sbJS.Append("})();\n");

            return sbJS.ToString();
        }

        /// <summary>
        /// Applies audience access and revoked permissions to a grid row. Should be called within InitialiseRow event if audiences enabled
        /// </summary>
        /// <param name="row">Grid row being initialised</param>
        /// <param name="gridID">ID of the Grid</param>
        /// <param name="keyfield">Grid key field providing unique record ID</param>
        /// <param name="audienceStatus">Audience Status collection</param>
        public static void InitialiseRowAudienceCheck(ref cNewGridRow row, string gridID, string keyfield, SerializableDictionary<string, object> audienceStatus)
        {
            if (row.getCellByID(keyfield) != null)
            {
                int recID = 0;
                int.TryParse(row.getCellByID(keyfield).Value.ToString(), out recID);

                if (recID > 0 && audienceStatus.ContainsKey(recID.ToString()))
                {
                    cAudienceRecordStatus audStatus = null;
                    if (audienceStatus[recID.ToString()].GetType() == typeof(Dictionary<string, object>))
                    {
                        // returned from javascript
                        Dictionary<string, object> d = (Dictionary<string, object>)audienceStatus[recID.ToString()];
                        audStatus = new cAudienceRecordStatus();
                        audStatus.AudienceID = (int?)d["AudienceID"];
                        audStatus.CanDelete = (bool)d["CanDelete"];
                        audStatus.CanEdit = (bool)d["CanEdit"];
                        audStatus.CanView = (bool)d["CanView"];
                        audStatus.RecordID = (int)d["RecordID"];
                        audStatus.Status = (int)d["Status"];
                    }
                    else
                    {
                        //returned from code behind
                        audStatus = (cAudienceRecordStatus)audienceStatus[recID.ToString()];
                    }

                    if (audStatus.Status == 0 || (audStatus.Status > 0 && !audStatus.CanView))
                    {
                        // audience status denies access
                        row.hidden = true;
                        return;
                    }

                    if (audStatus.Status > 0)
                    {
                        // check if edit rights revoked
                        row.enableupdating = audStatus.CanEdit;


                        // check if delete rights revoked
                        row.enabledeleting = audStatus.CanDelete;
                    }
                }
            }
            return;
        }
    }

    /// <summary>
    /// Class to hold a single column in the cGrid class
    /// </summary>
    public class cNewGridColumn
    {
        protected string sID;
        protected GridColumnType eColumnType;
        protected bool bHidden;
        protected string sAlias;
        protected string sHeaderText;
        protected cGridFormat clsformat = new cGridFormat();
        protected bool _ForceSingleLine = false;


        #region properties
        /// <summary>
        /// Gets or sets whether the column hidden at display time
        /// </summary>
        public bool hidden
        {
            get { return bHidden; }
            set { bHidden = value; }
        }

        /// <summary>
        /// Gets the alias name of the column
        /// </summary>
        public virtual string Alias
        {
            get
            {
                return sAlias ?? sID;
            }
            set { sAlias = value; }
        }

        /// <summary>
        /// Grid column format type
        /// </summary>
        public cGridFormat Format
        {
            get { return clsformat; }
            set { clsformat = value; }
        }

        /// <summary>
        /// Gets the type of column
        /// </summary>
        public GridColumnType ColumnType
        {
            get { return eColumnType; }
        }

        /// <summary>
        /// Gets or sets the column header text
        /// </summary>
        public string HeaderText
        {
            get { return sHeaderText; }
            set { sHeaderText = value; }
        }

        /// <summary>
        /// Grid column ID
        /// </summary>
        public string ID
        {
            get { return sID; }
            set { sID = value; }
        }

        /// <summary>
        /// Gets or sets whether a cell value should be forced on to a single line
        /// </summary>
        public bool ForceSingleLine
        {
            get { return _ForceSingleLine; }
            set { _ForceSingleLine = value; }
        }

        /// <summary>
        /// Set the width of the column (e.g. 100% or 654px)
        /// </summary>
        public Unit Width { get; set; }
        #endregion

        public string CustomDateFormat { get; set; }
    }

    /// <summary>
    /// Data Field Column class definition for cGridNew
    /// </summary>
    public class cFieldColumn : cNewGridColumn
    {
        private cField clsField;
        private SortedList<int, string> lstValueList = new SortedList<int, string>();
        private bool bHasList = false;
        private bool bIsLookupField;
        private cField clsLookupDisplayField;
        private JoinVia _joinVia = null;

        public cFieldColumn()
        {
        }

        /// <summary>
        /// Constructor to create a column for a particular report field to go into the parent grid
        /// </summary>
        /// <param name="field"></param>
        public cFieldColumn(cField field)
        {
            sID = field.FieldName;
            //sID = field.FieldID.ToString();
            eColumnType = GridColumnType.Field;
            clsField = field;
            sHeaderText = field.Description;
            setFieldFormat();
        }

        /// <summary>
        /// Constructor to create a column for a particular report field to go into the parent grid with an alias
        /// </summary>
        /// <param name="field"></param>
        /// <param name="alias"></param>
        public cFieldColumn(cField field, string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                sID = field.FieldName;
            }
            else
            {
                sID = alias;
            }
            //sID = field.FieldID.ToString();
            eColumnType = GridColumnType.Field;
            clsField = field;
            sAlias = alias;
            sHeaderText = field.Description;
            setFieldFormat();
        }

        /// <summary>
        /// Constructor to create a column that is a foreign key field and provides Text Value field used for display
        /// </summary>
        /// <param name="field">Field included</param>
        /// <param name="displayValueField">Foreign key value field that provide text value to display</param>
        public cFieldColumn(cField field, cField displayValueField)
        {
            sID = field.FieldName;
            //sID = field.FieldID.ToString();
            eColumnType = GridColumnType.Field;
            clsField = field;
            bIsLookupField = true;
            clsLookupDisplayField = displayValueField;
            sHeaderText = field.Description;
            setFieldFormat();
        }

        /// <summary>
        /// Constructor to create a column that is a field in a foreign table and provides a series of foreign key fields to get there from the base table
        /// </summary>
        /// <param name="field">Field included</param>
        /// <param name="joinVia">Foreign keys to traverse from the basetable to the "field"</param>
        public cFieldColumn(cField field, JoinVia joinVia)
        {
            //sID = field.FieldName;
            sID = field.FieldID.ToString() + "_" + joinVia.JoinViaID.ToString();
            sAlias = field.FieldName + "_" + joinVia.JoinViaID.ToString();
            eColumnType = GridColumnType.Field;
            clsField = field;
            sHeaderText = (joinVia.Description == string.Empty ? "" : "(" + joinVia.Description + ") ") + field.Description;
            _joinVia = joinVia;
            setFieldFormat();
        }

        private void setFieldFormat()
        {
            Format.FieldType = FieldType.NotSet;
            Format.Format = AttributeFormat.NotSet;
            switch (field.FieldType)
            {
                case "S":
                case "LT": // large text
                    Format.FieldType = FieldType.Text;
                    break;
                case "N":
                case "FI":
                    Format.FieldType = FieldType.Integer;
                    break;
                case "D":
                    Format.FieldType = FieldType.DateTime;
                    Format.Format = AttributeFormat.DateOnly;
                    break;
                case "DT":
                    Format.FieldType = FieldType.DateTime;
                    Format.Format = AttributeFormat.DateTime;
                    break;
                case "T":
                    Format.FieldType = FieldType.DateTime;
                    Format.Format = AttributeFormat.TimeOnly;
                    break;
                case "X":
                case "FX":
                    Format.FieldType = FieldType.TickBox;
                    break;
                case "C":
                case "M":
                    Format.FieldType = FieldType.Currency;
                    break;
                case "FD":
                    Format.FieldType = FieldType.Number;
                    break;
                case "AT":
                    Format.FieldType = FieldType.Attachment;
                    break;
            }
        }

        /// <summary>
        /// Add list items to the column using the names and values within an enum
        /// </summary>
        /// <param name="enumType">The Type of the enumerator to be used</param>
        public void addValueListItems(Type enumType)
        {
            foreach (int enumValue in Enum.GetValues(enumType))
            {
                addValueListItem(enumValue, Enum.GetName(enumType, enumValue).SplitCamel());
            }
        }

        /// <summary>
        /// Add a list item for the column
        /// </summary>
        /// <param name="id">unique ID value for the list item value</param>
        /// <param name="value">Display value for the list item</param>
        public void addValueListItem(int id, string value)
        {
            lstValueList.Add(id, value);
            bHasList = true;
        }

        /// <summary>
        /// Retrieve a defined list item value for the column
        /// </summary>
        /// <param name="id">Unique ID of the list item to be retrieved</param>
        /// <returns></returns>
        public string getValueListItem(int id)
        {
            string value;
            lstValueList.TryGetValue(id, out value);
            return value;
        }

        /// <summary>
        /// Retrieve the ID of a value list item by its display string value
        /// </summary>
        /// <param name="value">Display text value in the list</param>
        /// <returns>ID of the item</returns>
        public int getValueListItemId(string value)
        {
            int retVal = 0;
            foreach (KeyValuePair<int, string> kvp in lstValueList)
            {
                if (kvp.Value.ToLower() == value.ToLower())
                {
                    retVal = kvp.Key;
                    break;
                }
            }
            return retVal;
        }

        #region properties
        /// <summary>
        /// Gets the field definition for the column
        /// </summary>
        public cField field
        {
            get { return clsField; }

        }

        /// <summary>
        /// Gets whether the column has a list value collection
        /// </summary>
        public bool HasList
        {
            get { return bHasList; }
        }
        /// <summary>
        /// Gets the currently defined value list collection
        /// </summary>
        /// <returns>Returns a sorted list of int and value</returns>
        public SortedList<int, string> getValueList()
        {
            return lstValueList;
        }

        /// <summary>
        /// Gets or Sets the Alias column text
        /// </summary>
        public override string Alias
        {
            get
            {
                if (!string.IsNullOrEmpty(sAlias))
                {
                    return sAlias;
                }
                else
                {
                    return field.FieldName;
                }
            }
        }

        /// <summary>
        /// Identifies the column as a n:1 (genlist) field
        /// </summary>
        public bool IsLookupField
        {
            get { return bIsLookupField; }
            set { bIsLookupField = value; }
        }

        /// <summary>
        /// Field that provides the output text value for the lookup (genlist) field
        /// </summary>
        public cField LookupDisplayValueField
        {
            get { return clsLookupDisplayField; }

        }

        /// <summary>
        /// Describe the join fields to get to a distant field
        /// </summary>
        public JoinVia JoinVia
        {
            get { return _joinVia; }

        }

        #endregion
    }

    public class cEventColumn : cNewGridColumn
    {
        private string sTooltip;
        private string sOnClickCommand;
        private string sIconPath;
        private string sAlternateText;
        private string sHyperlinkText;
        private bool bIsHyperlink;

        #region properties
        public string Tooltip
        {
            get
            {
                return sTooltip;
            }
        }
        public string OnClickCommand
        {
            get
            {
                return sOnClickCommand;
            }
        }
        public string IconPath
        {
            get
            {
                return sIconPath;
            }
        }
        public string AlternateText
        {
            get
            {
                return sAlternateText;
            }
        }
        public string HyperlinkText
        {
            get
            {
                return sHyperlinkText;
            }
        }
        public bool IsHyperlink
        {
            get
            {
                return bIsHyperlink;
            }
        }
        #endregion

        public cEventColumn(string id, string iconPath, string onclickCommand, string alternate_text, string tooltip)
        {
            sID = id;
            eColumnType = GridColumnType.Event;
            bIsHyperlink = false;
            sHyperlinkText = "";
            sIconPath = iconPath;
            sOnClickCommand = onclickCommand;
            sTooltip = tooltip;
            sAlternateText = alternate_text;
            sHeaderText = string.Format("<img alt='{0}' src='{1}' />", AlternateText, IconPath);
        }

        public cEventColumn(string id, string hyperlinkText, string onclickCommand, string tooltip)
        {
            sID = id;
            eColumnType = GridColumnType.Event;
            bIsHyperlink = true;
            sHyperlinkText = hyperlinkText;
            sIconPath = "";
            sOnClickCommand = onclickCommand;
            sTooltip = tooltip;
            sAlternateText = "";
            sHeaderText = HyperlinkText;
        }
    }

    /// <summary>
    /// A column that shows a different icons, depending on the value of the associated column.
    /// </summary>
    public class ValueIconEventColumn : cEventColumn
    {
        /// <summary>
        /// The Column from which this Column will read the value, in order to determing the icon.
        /// </summary>
        public cFieldColumn ValueColumn { get; private set; }

        /// <summary>
        /// A list of ValueBasedIconEventColumnOptions. 
        /// During the generation of cells, the value will be grabbed from the reference column.
        /// This list will be checked for an item that has a matching value, and if there is one, 
        /// its properties will be used to define the generated markup. If a match is not found,
        /// this column will fall back to the defaults provided.
        /// </summary>
        public List<ValueIconEventColumnOptions> Options { get; private set; }

        /// <summary>
        /// The header tooltip text.
        /// </summary>
        public string HeaderTooltip { get; set; }

        /// <summary>
        /// The header icon path.
        /// </summary>
        public string HeaderIconPath { get; set; }

        /// <summary>
        /// Creates a new Value Based Icon Event Column.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="valueColumn">The reference column from which to grab the field value.</param>
        /// <param name="headerIconPath">The path of the icon to give the column header.</param>
        /// <param name="headerTooltip">The tooltip to give the header icon.</param>
        /// <param name="defaultIconPath">The path of the icon to use when the value does not have a match in <see cref="options"/>.</param>
        /// <param name="defaultTooltip">The tooltip to use (img.alt and a.title) when the value does not have a match in <see cref="options"/>.</param>
        /// <param name="defaultOnClick">The a.onClick behaviour to use when the value does not have a match in <see cref="options"/>.</param>
        /// <param name="options">
        /// A list of ValueBasedIconEventColumnOptions. 
        /// During the generation of cells, the value will be grabbed from the reference column.
        /// This list will be checked for an item that has a matching value, and if there is one, 
        /// its properties will be used to define the generated markup. If a match is not found,
        /// this column will fall back to the defaults provided.
        /// </param>
        public ValueIconEventColumn(string id, cFieldColumn valueColumn, string headerIconPath, string headerTooltip, string defaultIconPath, string defaultTooltip, string defaultOnClick, List<ValueIconEventColumnOptions> options)
            : base(id, defaultIconPath, defaultOnClick ?? "#", defaultTooltip, defaultTooltip)
        {
            sID = id;
            ValueColumn = valueColumn;
            ValueColumn.hidden = true;
            Options = options;
            HeaderTooltip = headerTooltip;
            HeaderIconPath = headerIconPath;
            sHeaderText = string.Format("<img alt='{0}' title='{0}' src='{1}' />", headerTooltip, headerIconPath);
            eColumnType = GridColumnType.ValueIcon;
        }
    }

    /// <summary>
    /// Defines a set of options for controlling the markup generated by a <see cref="ValueIconEventColumn"/>.
    /// </summary>
    public class ValueIconEventColumnOptions
    {
        /// <summary>
        /// The 'Key' of this item. When the value in the Column matches this,
        /// then the other data from this object will be used.
        /// </summary>
        public object ExpectedValue { get; set; }

        /// <summary>
        /// The optional of this item. When the value in the Column matches this,
        /// then the other data from this object will be used.
        /// </summary>
        public object SecondExpectedValue { get; set; }

        /// <summary>
        /// The path to the icon.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// The hover text (to be used in the img.alt and a.title).
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// A javascript function string to add to the onClick attribute of the 
        /// anchor surrounding the icon image. If null, the default for the column will be used.
        /// </summary>
        public string OnClickCommand { get; set; }
    }

    /// <summary>
    /// cTwoStateEventColumn:  Creates an event column with associated ata clumn, alowing a two state display e.g. Valid / Invalid.
    /// </summary>
    public class cTwoStateEventColumn : cNewGridColumn
    {
        private cFieldColumn valueColumn;
        private object stateOneValue;
        private object stateTwoValue;
        private string sTooltipStateOne;
        private string sOnClickCommandStateOne;
        private string sIconPathStateOne;
        private string sAlternateTextStateOne;
        private string sHyperlinkTextStateOne;
        private bool bIsHyperlinkStateOne;
        private string sTooltipStateTwo;
        private string sOnClickCommandStateTwo;
        private string sIconPathStateTwo;
        private string sAlternateTextStateTwo;
        private string sHyperlinkTextStateTwo;
        private bool bIsHyperlinkStateTwo;
        private cField clsField;

        private SortedList<int, string> lstValueList = new SortedList<int, string>();
        private bool bHasList = false;
        private bool bIsLookupField;
        private cField clsLookupDisplayField;
        private JoinVia _joinVia = null;

        /// <summary>
        /// Class Initializer for Two State Event Column
        /// </summary>
        /// <param name="valueColumn"></param> Value Column used to compare the StateValues
        /// <param name="stateOneValue"></param> The value of valueColumn that relates to StateOne
        /// <param name="stateTwoValue"></param> The value of valueColumn that relates to StateTwo
        /// <param name="sTooltipStateOne"></param> The Tooltip o use for StateOne
        /// <param name="sOnClickCommandStateOne"></param> The OnClickCommand to use for StateOne
        /// <param name="sIconPathStateOne"></param> the IconPath to usefor StateOne
        /// <param name="sAlternateTextStateOne"></param> Alternate Text to use or StateOne
        /// <param name="sHyperlinkTextStateOne"></param> Hyperlink Text to use for StateOne
        /// <param name="bIsHyperlinkStateOne"></param> True if hyperlink
        /// <param name="sTooltipStateTwo"></param> Tooltip text for StateTwo
        /// <param name="sOnClickCommandStateTwo"></param> OnClickCmand text for StateTwo
        /// <param name="sIconPathStateTwo"></param> Icon path for StateTwo
        /// <param name="sAlternateTextStateTwo"></param> Alternate text for StateTwo
        /// <param name="sHyperlinkTextStateTwo"></param> Hyperlink text for StateTwo
        /// <param name="bIsHyperlinkStateTwo"></param> True if hyperlink
        public cTwoStateEventColumn(cFieldColumn valueColumn, object stateOneValue, object stateTwoValue, string sTooltipStateOne, string sOnClickCommandStateOne, string sIconPathStateOne, string sAlternateTextStateOne, string sHyperlinkTextStateOne, bool bIsHyperlinkStateOne, string sTooltipStateTwo, string sOnClickCommandStateTwo, string sIconPathStateTwo, string sAlternateTextStateTwo, string sHyperlinkTextStateTwo, bool bIsHyperlinkStateTwo)
        {
            this.valueColumn = valueColumn;
            this.stateOneValue = stateOneValue;
            this.stateTwoValue = stateTwoValue;
            this.sTooltipStateOne = sTooltipStateOne;
            this.sOnClickCommandStateOne = sOnClickCommandStateOne;
            this.sIconPathStateOne = sIconPathStateOne;
            this.sAlternateTextStateOne = sAlternateTextStateOne;
            this.sHyperlinkTextStateOne = sHyperlinkTextStateOne;
            this.bIsHyperlinkStateOne = bIsHyperlinkStateOne;
            this.sTooltipStateTwo = sTooltipStateTwo;
            this.sOnClickCommandStateTwo = sOnClickCommandStateTwo;
            this.sIconPathStateTwo = sIconPathStateTwo;
            this.sAlternateTextStateTwo = sAlternateTextStateTwo;
            this.sHyperlinkTextStateTwo = sHyperlinkTextStateTwo;
            this.bIsHyperlinkStateTwo = bIsHyperlinkStateTwo;
            this.eColumnType = GridColumnType.TwoState;
            this.Alias = valueColumn.Alias;
            this.HeaderText = valueColumn.HeaderText;
            this.ID = valueColumn.ID;
            this.hidden = false;
            this.Format = valueColumn.Format;

            this.IsLookupField = valueColumn.IsLookupField;
            this.JoinVia = valueColumn.JoinVia;
            this.LookupDisplayField = valueColumn.LookupDisplayValueField;
            this.Field = valueColumn.field;

        }

        /// <summary>
        /// Return TwoStateEvent Column based on Input "ValueColumn"
        /// Defaults to state one, only state two if value matches value of state two.
        /// </summary>
        /// <param name="id">ID of new column</param> 
        /// <param name="ValueColumn">cFieldColumn used to create new TwoStageEvent Column an also to compare the value or StateOne ad Two</param> 
        /// <param name="StateOneValue">The value used for StateOne</param> 
        /// <param name="StateTwoValue">the Value used for StateTwo</param> 
        /// <param name="iconPathStateOne">Icon path when valueColumn = StateOneValue</param> 
        /// <param name="onclickCommandStateOne">OnClickCommand for StateOne</param> 
        /// <param name="alternate_textStateOne">Alternate Text for StateOne</param> 
        /// <param name="tooltipStateOne">Tooltip Text for StateOne</param> 
        /// <param name="iconPathStateTwo">Icon path for StateTwo</param> 
        /// <param name="onclickCommandStateTwo">OnClick Command for StateTwo</param> 
        /// <param name="alternate_textStateTwo">Alternate text for StateTwo</param> 
        /// <param name="tooltipStateTwo">tooltip text for StateTwo</param> 
        /// <returns>two state event column</returns>
        public static cTwoStateEventColumn FromValueColumn(string id, cFieldColumn ValueColumn, object StateOneValue, object StateTwoValue, string iconPathStateOne, string onclickCommandStateOne, string alternate_textStateOne, string tooltipStateOne, string iconPathStateTwo, string onclickCommandStateTwo, string alternate_textStateTwo, string tooltipStateTwo)
        {
            var result = new cTwoStateEventColumn(ValueColumn, StateOneValue, StateTwoValue, tooltipStateOne, onclickCommandStateOne, iconPathStateOne, alternate_textStateOne, alternate_textStateOne, true, tooltipStateTwo, onclickCommandStateTwo, iconPathStateTwo, alternate_textStateTwo, alternate_textStateTwo, true)
            {
                Alias = id,
                HeaderText = ValueColumn.HeaderText,
                ID = id,
                hidden = false,
                Format = ValueColumn.Format,
                IsLookupField = ValueColumn.IsLookupField,
                JoinVia = ValueColumn.JoinVia,
                LookupDisplayField = ValueColumn.LookupDisplayValueField,
                Field = ValueColumn.field
            };

            return result;
        }

        #region Properties

        /// <summary>
        /// he value associated with "State One" being displayed
        /// </summary>
        public object StateOneValue
        {
            get { return stateOneValue; }
            set { stateOneValue = value; }
        }
        /// <summary>
        /// the Value associated with "State Two" being displayed
        /// </summary>
        public object StateTwoValue
        {
            get { return stateTwoValue; }
            set { stateTwoValue = value; }
        }
        /// <summary>
        /// OnClickComand when StateOne is true
        /// </summary>
        public string OnClickCommandStateOne
        {
            get { return sOnClickCommandStateOne; }
            set { sOnClickCommandStateOne = value; }
        }
        /// <summary>
        /// Icon path when StateOne is true
        /// </summary>
        public string IconPathStateOne
        {
            get { return sIconPathStateOne; }
            set { sIconPathStateOne = value; }
        }
        /// <summary>
        /// Alternate Text when StateOne is true
        /// </summary>
        public string AlternateTextStateOne
        {
            get { return sAlternateTextStateOne; }
            set { sAlternateTextStateOne = value; }
        }
        /// <summary>
        /// Hyperlink text when StateOne is true
        /// </summary>
        public string HyperlinkTextStateOne
        {
            get { return sHyperlinkTextStateOne; }
            set { sHyperlinkTextStateOne = value; }
        }
        /// <summary>
        /// True if StateOne is hyperink
        /// </summary>
        public bool IsHyperlinkStateOne
        {
            get { return bIsHyperlinkStateOne; }
            set { bIsHyperlinkStateOne = value; }
        }
        /// <summary>
        /// Tooltip text for StateTwo
        /// </summary>
        public string TooltipStateTwo
        {
            get { return sTooltipStateTwo; }
            set { sTooltipStateTwo = value; }
        }
        /// <summary>
        /// OnClickCommand for StateTwo
        /// </summary>
        public string OnClickCommandStateTwo
        {
            get { return sOnClickCommandStateTwo; }
            set { sOnClickCommandStateTwo = value; }
        }
        /// <summary>
        /// IconPath for StateTwo
        /// </summary>
        public string IconPathStateTwo
        {
            get { return sIconPathStateTwo; }
            set { sIconPathStateTwo = value; }
        }
        /// <summary>
        /// Alternate text for StateTwo
        /// </summary>
        public string AlternateTextStateTwo
        {
            get { return sAlternateTextStateTwo; }
            set { sAlternateTextStateTwo = value; }
        }
        /// <summary>
        /// Hyperlink text for StateTwo
        /// </summary>
        public string HyperlinkTextStateTwo
        {
            get { return sHyperlinkTextStateTwo; }
            set { sHyperlinkTextStateTwo = value; }
        }
        /// <summary>
        /// True if StateTwo is hyperlink
        /// </summary>
        public bool IsHyperlinkStateTwo
        {
            get { return bIsHyperlinkStateTwo; }
            set { bIsHyperlinkStateTwo = value; }
        }
        /// <summary>
        /// Tooltip text for StateOne
        /// </summary>
        public string TooltipStateOne
        {
            get { return sTooltipStateOne; }
            set { sTooltipStateOne = value; }
        }
        /// <summary>
        /// Field details for ValueColumn
        /// </summary>
        public cField Field
        {
            get { return clsField; }
            set { clsField = value; }
        }
        /// <summary>
        /// SortedList if field is ValueList
        /// </summary>
        public SortedList<int, string> ValueList
        {
            get { return lstValueList; }
            set { lstValueList = value; }
        }
        /// <summary>
        /// True if field is value list
        /// </summary>
        public bool HasList
        {
            get { return bHasList; }
            set { bHasList = value; }
        }
        /// <summary>
        /// True i field is a lookup field
        /// </summary>
        public bool IsLookupField
        {
            get { return bIsLookupField; }
            set { bIsLookupField = value; }
        }
        /// <summary>
        /// True i field is a LookupDisplayField
        /// </summary>
        public cField LookupDisplayField
        {
            get { return clsLookupDisplayField; }
            set { clsLookupDisplayField = value; }
        }
        /// <summary>
        /// True if Field is a JoinVia
        /// </summary>
        public JoinVia JoinVia
        {
            get { return _joinVia; }
            set { _joinVia = value; }
        }
        #endregion Propeties
    }

    public class cStaticGridColumn : cNewGridColumn
    {
        private string sStaticFieldName;
        private Guid gStaticFieldId;

        /// <summary>
        /// Constructor to create a column for a static value to go in to the parent grid
        /// </summary>
        /// <param name="static_fieldname">Name of the field</param>
        public cStaticGridColumn(string static_fieldname, Guid static_fieldid)
        {
            eColumnType = GridColumnType.Static;
            sStaticFieldName = static_fieldname;
            gStaticFieldId = static_fieldid;
            sHeaderText = static_fieldname;
            sID = static_fieldname;
        }

        #region properties
        /// <summary>
        /// Gets the field name for the static column
        /// </summary>
        public string staticFieldName
        {
            get { return sStaticFieldName; }
        }
        /// <summary>
        /// Gets the field id for the static column
        /// </summary>
        public Guid staticFieldId
        {
            get { return gStaticFieldId; }
        }
        #endregion
    }
    public class cNewGridRow
    {
        private List<cNewGridCell> lstCells = new List<cNewGridCell>();
        private bool bEnableUpdating = true;
        private bool bEnableDeleting = true;
        private bool bEnableArchiving = true;
        private bool bHidden = false;
        private string _cssClass;

        public cNewGridRow()
        {
        }

        /// <summary>
        /// Adds a new cell to the row
        /// </summary>
        /// <param name="cell"></param>
        public void addCell(cNewGridCell cell)
        {
            lstCells.Add(cell);
        }

        /// <summary>
        /// Returns the cell specified by the given column ID 
        /// - JoinVia safe
        /// </summary>
        /// <param name="id">The field ID or alias of the column you which to retrieve</param>
        /// <returns></returns>
        public cNewGridCell getCellByID(string id)
        {
            foreach (cNewGridCell cell in lstCells)
            {
                if (cell.Column.ColumnType == GridColumnType.Field)
                {
                    if (cell.Column.ID.ToLower() == id.ToLower() ||
                        (((cFieldColumn)cell.Column).JoinVia == null && ((cFieldColumn)cell.Column).field != null && ((cFieldColumn)cell.Column).field.FieldID != Guid.Empty &&
                            ((cFieldColumn)cell.Column).field.FieldName.ToLower() == id.ToLower()) || ((cFieldColumn)cell.Column).Alias.ToLower() == id.ToLower())
                    {
                        return cell;
                    }
                }
                else
                {
                    if (cell.Column.ID.ToLower() == id.ToLower())
                    {
                        return cell;
                    }
                }
            }
            return null;
        }

        #region Properties
        /// <summary>
        /// Gets the cells for the rows
        /// </summary>
        public List<cNewGridCell> Cells
        {
            get { return lstCells; }
        }
        /// <summary>
        /// Gets or sets whether the edit column should be displayed on the grid.
        /// </summary>
        public bool enableupdating
        {
            get { return bEnableUpdating; }
            set { bEnableUpdating = value; }
        }
        /// <summary>
        /// Gets or sets whether the delete column should be displayed on the grid.
        /// </summary>
        public bool enabledeleting
        {
            get
            {
                return bEnableDeleting;

            }
            set { bEnableDeleting = value; }
        }
        /// <summary>
        /// Gets or sets whether the archive/unarchive column should be displayed on the grid.
        /// </summary>
        public bool enablearchiving
        {
            get
            {
                return bEnableArchiving;

            }
            set { bEnableArchiving = value; }
        }
        /// <summary>
        /// Indicates whether the row should be displayed
        /// </summary>
        public bool hidden
        {
            get { return bHidden; }
            set { bHidden = value; }
        }

        /// <summary>
        /// Gets or sets the css class that will override the default row css.
        /// </summary>
        public string CssClass
        {
            get
            {
                return this._cssClass;
            }
            set
            {
                this._cssClass = value;
            }
        }

        /// <summary>
        /// Gets or sets the highlight (in red) of the current row.
        /// </summary>
        public bool Highlight { get; set; }

        #endregion
    }

    public class cNewGridCell
    {
        private cNewGridColumn clsColumn;
        private object oValue;
        private cGridFormat clsFormat = new cGridFormat();
        public cNewGridCell(object value, cNewGridColumn column)
        {
            this.oValue = value;
            this.clsColumn = column;
            this.Format.FieldType = this.clsColumn.Format.FieldType;
            this.Format.Format = this.clsColumn.Format.Format;
            this.Format.Symbol = this.clsColumn.Format.Symbol;

            if (column.GetType() == typeof(ValueIconEventColumn))
            {
                var iconColumn = (ValueIconEventColumn)column;
                if (iconColumn.Options != null)
                {
                    var option = iconColumn.Options.FirstOrDefault(o => o.ExpectedValue.Equals(oValue));

                    if (option == null)
                    {
                        oValue = "<a href=\"" + iconColumn.OnClickCommand + "\" title=\"" + iconColumn.Tooltip + "\"><img alt=\"" + iconColumn.AlternateText + "\" src=\"" + iconColumn.IconPath + "\" /></a>";
                    }
                    else
                    {
                        oValue = "<a href=\"" + (option.OnClickCommand ?? iconColumn.OnClickCommand) + "\" title=\"" + (option.Tooltip ?? iconColumn.Tooltip) + "\"><img alt=\"" + (option.Tooltip ?? iconColumn.Tooltip) + "\" src=\"" + (option.IconPath ?? iconColumn.IconPath) + "\" /></a>";
                    }
                }
            }

            if (column.GetType() == typeof(cEventColumn))
            {
                var eventcolumn = (cEventColumn)column;
                if (eventcolumn.IsHyperlink)
                {
                    this.oValue = "<a href=\"" + eventcolumn.OnClickCommand + "\" title=\"" + eventcolumn.Tooltip + "\" >" + eventcolumn.HyperlinkText + "</a>";
                }
                else
                {
                    this.oValue = "<a href=\"" + eventcolumn.OnClickCommand + "\" title=\"" + eventcolumn.Tooltip + "\"><img alt=\"" + eventcolumn.AlternateText + "\" src=\"" + eventcolumn.IconPath + "\" /></a>";
                }
            }

            if (column.GetType() == typeof(cTwoStateEventColumn))
            {
                var twoStateEventColumn = (cTwoStateEventColumn)column;
                if (twoStateEventColumn.StateTwoValue == null)
                {
                    if (string.IsNullOrEmpty(value.ToString()))
                    {
                        this.oValue = FormatEventColumn(
                            twoStateEventColumn.OnClickCommandStateTwo.Replace("{value}", value.ToString()),
                            twoStateEventColumn.TooltipStateTwo,
                            twoStateEventColumn.AlternateTextStateTwo,
                                                   twoStateEventColumn.IconPathStateTwo);
                    }
                    else
                    {
                        this.oValue = FormatEventColumn(
                            twoStateEventColumn.OnClickCommandStateOne.Replace("{value}", value.ToString()),
                            twoStateEventColumn.TooltipStateOne,
                            twoStateEventColumn.AlternateTextStateOne,
                                                   twoStateEventColumn.IconPathStateOne);
                    }
                }
                else
                {
                    if (value.ToString() == twoStateEventColumn.StateTwoValue.ToString())
                    {
                        this.oValue = FormatEventColumn(
                            twoStateEventColumn.OnClickCommandStateTwo.Replace("{value}", value.ToString()),
                            twoStateEventColumn.TooltipStateTwo,
                            twoStateEventColumn.AlternateTextStateTwo,
                                                   twoStateEventColumn.IconPathStateTwo);
                    }
                    else
                    {
                        this.oValue = FormatEventColumn(
                            twoStateEventColumn.OnClickCommandStateOne.Replace("{value}", value.ToString()),
                            twoStateEventColumn.TooltipStateOne,
                            twoStateEventColumn.AlternateTextStateOne,
                                                   twoStateEventColumn.IconPathStateOne);
                    }
                }
            }
        }

        private static string FormatEventColumn(string onClickCommand, string toolTip, string alternateText, string iconPath)
        {
            if (string.IsNullOrWhiteSpace(onClickCommand))
            {
                return "<a></a>";
            }
            return "<a href=\"" + onClickCommand + "\" title=\"" + toolTip + "\"><img alt=\"" + alternateText + "\" src=\"" + iconPath + "\"></a>";
        }

        #region properties
        /// <summary>
        /// Gets or sets the value of the cell
        /// </summary>
        public object Value
        {
            get { return oValue; }
            set { oValue = value; }
        }
        public cNewGridColumn Column
        {
            get { return clsColumn; }
        }
        public cGridFormat Format
        {
            get { return clsFormat; }
        }
        public string Text
        {
            get { return (oValue == null || oValue == DBNull.Value ? String.Empty : oValue.ToString()); }
        }
        #endregion
    }

    /// <summary>
    /// Enumerated list of column alignment definitions
    /// </summary>
    public enum ColumnAlignment
    {
        None,
        Left,
        Center,
        Right
    }

    /// <summary>
    /// Page Position enumerator
    /// </summary>
    public enum PagerPosition
    {
        Top,
        Bottom,
        TopAndBottom
    }

    /// <summary>
    /// Grid Select Type enumerator
    /// </summary>
    public enum GridSelectType
    {
        CheckBox,
        RadioButton
    }

    /// <summary>
    /// Grid Format enumerator
    /// </summary>
    public class cGridFormat
    {
        private FieldType eFieldType;
        private AttributeFormat eFormat;
        private string sSymbol;

        #region properties
        /// <summary>
        /// Gets or Sets the field type for the grid format
        /// </summary>
        public FieldType FieldType
        {
            get { return eFieldType; }
            set { eFieldType = value; }
        }
        /// <summary>
        /// Gets or Sets the Attribute Format property for the grid format element
        /// </summary>
        public AttributeFormat Format
        {
            get { return eFormat; }
            set { eFormat = value; }
        }

        /// <summary>
        /// Gets or Sets the symbol for use by the grid format element
        /// </summary>
        public string Symbol
        {
            get { return sSymbol; }
            set { sSymbol = value; }
        }

        /// <summary>
        /// Gets or Sets the Format String for use by the grid format element
        /// </summary>
        public string FormatString
        {
            get
            {
                switch (eFieldType)
                {
                    case FieldType.Text:
                        return "{0}";
                    case FieldType.Currency:
                        return Symbol + "{0:###,###,##0.00}";
                    case FieldType.Integer:
                        return "{0:d}";
                }
                return "{0}";
            }
        }
        #endregion
    }

    /// <summary>
    /// Grid Column Type enumerator
    /// </summary>
    public enum GridColumnType
    {
        Field = 1,
        Event = 2,
        Static = 3,
        TwoState = 4,
        ValueIcon = 5
    }

    // JS Serializable class used by grid
    [Serializable()]
    public class cJSGridColumn
    {
        bool bHidden;
        Guid gFieldID;
        SerializableDictionary<string, string> kvpValuelist;
        GridColumnType eColumnType;
        string sStaticFieldName;
        Guid gStaticFieldID;
        string sAlternateText;
        string sHyperlinkText;
        string sIconPath;
        string sOnClickCommand;
        string sTooltip;
        string sID;
        JSJoinVia oJoinVia;
        // New Fields for cTwoStateField
        private cFieldColumn valueColumn;
        private object stateOneValue;
        private object stateTwoValue;
        private string sTooltipStateOne;
        private string sOnClickCommandStateOne;
        private string sIconPathStateOne;
        private string sAlternateTextStateOne;
        private string sHyperlinkTextStateOne;
        private bool bIsHyperlinkStateOne;
        private string sTooltipStateTwo;
        private string sOnClickCommandStateTwo;
        private string sIconPathStateTwo;
        private string sAlternateTextStateTwo;
        private string sHyperlinkTextStateTwo;
        private bool bIsHyperlinkStateTwo;
        private string headerText;
        private string _Alias;
        private bool _ForceSingleLine = false;

        public string HeaderTooltip { get; set; }
        public string HeaderIconPath { get; set; }
        public List<ValueIconEventColumnOptions> Options { get; set; }

        public int PixelWidth { get; set; }

        // End

        public cJSGridColumn() { }

        #region properties
        public bool hidden
        {
            get { return bHidden; }
            set { bHidden = value; }
        }
        public Guid fieldID
        {
            get { return gFieldID; }
            set { gFieldID = value; }
        }
        public SerializableDictionary<string, string> valuelist
        {
            get
            {
                if (kvpValuelist == null)
                {
                    kvpValuelist = new SerializableDictionary<string, string>();
                }
                return kvpValuelist;
            }
            set { kvpValuelist = value; }
        }
        public GridColumnType columnType
        {
            get { return eColumnType; }
            set { eColumnType = value; }
        }
        public string staticFieldName
        {
            get { return sStaticFieldName; }
            set { sStaticFieldName = value; }
        }
        public Guid staticFieldID
        {
            get { return gStaticFieldID; }
            set { gStaticFieldID = value; }
        }
        public string alternateText
        {
            get { return sAlternateText; }
            set { sAlternateText = value; }
        }
        public string hyperlinkText
        {
            get { return sHyperlinkText; }
            set { sHyperlinkText = value; }
        }
        public string iconPath
        {
            get { return sIconPath; }
            set { sIconPath = value; }
        }
        public string onClickCommand
        {
            get { return sOnClickCommand; }
            set { sOnClickCommand = value; }
        }
        public string tooltip
        {
            get { return sTooltip; }
            set { sTooltip = value; }
        }
        public string ID
        {
            get { return sID; }
            set { sID = value; }
        }
        public JSJoinVia JoinVia
        {
            get { return oJoinVia; }
            set { oJoinVia = value; }
        }

        public cFieldColumn ValueColumn
        {
            get { return valueColumn; }
            set { valueColumn = value; }
        }

        public object StateOneValue
        {
            get { return stateOneValue; }
            set { stateOneValue = value; }
        }

        public object StateTwoValue
        {
            get { return stateTwoValue; }
            set { stateTwoValue = value; }
        }

        public string TooltipStateOne
        {
            get { return sTooltipStateOne; }
            set { sTooltipStateOne = value; }
        }

        public string OnClickCommandStateOne
        {
            get { return sOnClickCommandStateOne; }
            set { sOnClickCommandStateOne = value; }
        }

        public string IconPathStateOne
        {
            get { return sIconPathStateOne; }
            set { sIconPathStateOne = value; }
        }

        public string AlternateTextStateOne
        {
            get { return sAlternateTextStateOne; }
            set { sAlternateTextStateOne = value; }
        }

        public string HyperlinkTextStateOne
        {
            get { return sHyperlinkTextStateOne; }
            set { sHyperlinkTextStateOne = value; }
        }

        public bool ISHyperlinkStateOne
        {
            get { return bIsHyperlinkStateOne; }
            set { bIsHyperlinkStateOne = value; }
        }

        public string TooltipStateTwo
        {
            get { return sTooltipStateTwo; }
            set { sTooltipStateTwo = value; }
        }

        public string OnClickCommandStateTwo
        {
            get { return sOnClickCommandStateTwo; }
            set { sOnClickCommandStateTwo = value; }
        }

        public string IconPathStateTwo
        {
            get { return sIconPathStateTwo; }
            set { sIconPathStateTwo = value; }
        }

        public string AlternateTextStateTwo
        {
            get { return sAlternateTextStateTwo; }
            set { sAlternateTextStateTwo = value; }
        }

        public string HyperlinkTextStateTwo
        {
            get { return sHyperlinkTextStateTwo; }
            set { sHyperlinkTextStateTwo = value; }
        }

        public bool ISHyperlinkStateTwo
        {
            get { return bIsHyperlinkStateTwo; }
            set { bIsHyperlinkStateTwo = value; }
        }

        /// <summary>
        /// Gets or sets the column header text
        /// </summary>
        public string HeaderText
        {
            get
            {
                return headerText;
            }
            set
            {
                headerText = value;
            }
        }

        /// <summary>
        /// Gets or sets the alias of this column
        /// </summary>
        public string Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }

        /// <summary>
        /// Gets or sets whether the force the cell value on to one line
        /// </summary>
        public bool ForceSingleLine
        {
            get { return _ForceSingleLine; }
            set { _ForceSingleLine = value; }
        }

        public string CustomDateFormat { get; set; }
        #endregion


    }

    [Serializable()]
    public class cJSGridFilter
    {
        Guid gFieldID;
        byte byteCondition;
        byte byteJoiner;
        List<object> lstValues1;
        List<object> lstValues2;
        JSJoinVia oJoinVia;

        public cJSGridFilter() { }

        public cJSGridFilter(Guid fieldID, string name, object value)
        {
            gFieldID = fieldID;
            this.ParameterName = name;
            this.ParameterValue = value;
        }

        #region properties
        public Guid fieldID
        {
            get { return gFieldID; }
            set { gFieldID = value; }
        }
        public byte condition
        {
            get { return byteCondition; }
            set { byteCondition = value; }
        }
        public byte joiner
        {
            get { return byteJoiner; }
            set { byteJoiner = value; }
        }
        public List<object> values1
        {
            get
            {
                if (lstValues1 == null)
                {
                    lstValues1 = new List<object>();
                }
                return lstValues1;
            }
            set { lstValues1 = value; }
        }
        public List<object> values2
        {
            get
            {
                if (lstValues2 == null)
                {
                    lstValues2 = new List<object>();
                }
                return lstValues2;
            }
            set { lstValues2 = value; }
        }
        public JSJoinVia JoinVia
        {
            get { return oJoinVia; }
            set { oJoinVia = value; }
        }
        /// <summary>
        /// Gets or sets a parameter name used with an explicit where caluse
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// Gets or sets a parameter value used with an explicit where caluse
        /// </summary>
        public object ParameterValue { get; set; }
        #endregion
    }

    [Serializable()]
    public class cJSGridDetail
    {
        string sGridID;
        bool bEnableUpdating;
        string sEditLink;
        bool bEnableDeleting;
        string sDeleteLink;
        bool bEnablePaging;
        int nPageSize;
        bool bShowHeaders;
        bool bShowFooters;
        Guid gSortedColumnFieldID;
        List<cJSGridColumn> lstColumns;
        Guid gBaseTableID;
        bool bEnableArchiving;
        string sArchiveLink;
        string sArchiveField;
        SpendManagementLibrary.SortDirection eSortDirection;
        string sKeyField;
        bool bDisplayFilter;
        List<cJSGridFilter> lstFilters;
        bool bEnableSelect;
        GridSelectType eGridSelectType;
        bool bIsDataSet;
        string sCssClass;
        string sCurrencyColumnName;
        int nCurrencyId;
        string sEmptyText;
        string sServiceClassForInitialiseRowEvent;
        string sServiceClassMethodForInitialiseRowEvent;
        SerializableDictionary<string, object> dicGridInfo;
        int nRowCount;
        int nRowHiddenCount;
        List<int> lstHiddenRecords;
        int nSortedColumnJoinViaID;
        string filterCriteria;  // dcp 44632
        string whereClause;
        string _defaultCurrencySymbol;


        public cJSGridDetail() { }

        #region properties
        public string GridID
        {
            get { return sGridID; }
            set { sGridID = value; }
        }
        public bool enableUpdating
        {
            get { return bEnableUpdating; }
            set { bEnableUpdating = value; }
        }
        public string editLink
        {
            get { return sEditLink; }
            set { sEditLink = value; }
        }
        public bool enableDeleting
        {
            get { return bEnableDeleting; }
            set { bEnableDeleting = value; }
        }
        public string deleteLink
        {
            get { return sDeleteLink; }
            set { sDeleteLink = value; }
        }
        public bool enablePaging
        {
            get { return bEnablePaging; }
            set { bEnablePaging = value; }
        }
        public int pageSize
        {
            get { return nPageSize; }
            set { nPageSize = value; }
        }
        public bool showHeaders
        {
            get { return bShowHeaders; }
            set { bShowHeaders = value; }
        }
        public bool showFooters
        {
            get { return bShowFooters; }
            set { bShowFooters = value; }
        }
        public Guid sortedColumnFieldID
        {
            get { return gSortedColumnFieldID; }
            set { gSortedColumnFieldID = value; }
        }
        public List<cJSGridColumn> columns
        {
            get
            {
                if (lstColumns == null)
                { lstColumns = new List<cJSGridColumn>(); }
                return lstColumns;
            }
            set { lstColumns = value; }
        }
        public Guid baseTableID
        {
            get { return gBaseTableID; }
            set { gBaseTableID = value; }
        }
        public bool enableArchiving
        {
            get { return bEnableArchiving; }
            set { bEnableArchiving = value; }
        }
        public string archiveLink
        {
            get { return sArchiveLink; }
            set { sArchiveLink = value; }
        }
        public string archiveField
        {
            get { return sArchiveField; }
            set { sArchiveField = value; }
        }
        public SpendManagementLibrary.SortDirection sortDirection
        {
            get { return eSortDirection; }
            set { eSortDirection = value; }
        }
        public string keyField
        {
            get { return sKeyField; }
            set { sKeyField = value; }
        }
        public bool displayFilter
        {
            get { return bDisplayFilter; }
            set { bDisplayFilter = value; }
        }
        public List<cJSGridFilter> filters
        {
            get
            {
                if (lstFilters == null)
                {
                    lstFilters = new List<cJSGridFilter>();
                }
                return lstFilters;
            }
            set { lstFilters = value; }
        }
        public bool enableSelect
        {
            get { return bEnableSelect; }
            set { bEnableSelect = value; }
        }
        public GridSelectType gridSelectType
        {
            get { return eGridSelectType; }
            set { eGridSelectType = value; }
        }
        public bool isDataSet
        {
            get { return bIsDataSet; }
            set { bIsDataSet = value; }
        }
        public string cssClass
        {
            get { return sCssClass; }
            set { sCssClass = value; }
        }
        public string currencyColumnName
        {
            get { return sCurrencyColumnName; }
            set { sCurrencyColumnName = value; }
        }
        public int currencyId
        {
            get { return nCurrencyId; }
            set { nCurrencyId = value; }
        }
        public string emptyText
        {
            get { return sEmptyText; }
            set { sEmptyText = value; }
        }
        public string FilterCriteria
        {
            get { return filterCriteria; }
            set { filterCriteria = value; }
        }

        /// <summary>
        /// gets/sets the namespace.class name for the web service class handling the initialiserow event of the grid
        /// </summary>
        public string ServiceClassMethodForInitialiseRowEvent
        {
            get { return sServiceClassMethodForInitialiseRowEvent; }
            set { sServiceClassMethodForInitialiseRowEvent = value; }
        }
        /// <summary>
        /// gets/sets the method within the web service class that handles the initialiserow event of the grid
        /// </summary>
        public string ServiceClassForInitialiseRowEvent
        {
            get { return sServiceClassForInitialiseRowEvent; }
            set { sServiceClassForInitialiseRowEvent = value; }
        }
        /// <summary>
        /// Gets or Sets the additional information properties that may be required during InitialiseRow event handling
        /// </summary>
        public SerializableDictionary<string, object> InitialiseRowGridInfo
        {
            get { return dicGridInfo; }
            set { dicGridInfo = value; }
        }
        /// <summary>
        /// Gets or sets the number of rows of data in the grid
        /// </summary>
        public int rowCount
        {
            get { return nRowCount; }
            set { nRowCount = value; }
        }
        /// <summary>
        /// Gets or sets the number of data rows hidden due to audience restrictions
        /// </summary>
        public int rowHiddenCount
        {
            get { return nRowHiddenCount; }
            set { nRowHiddenCount = value; }
        }
        /// <summary>
        /// Gets or sets any record IDs that should be excluded from the grid query
        /// </summary>
        public List<int> HiddenRecords
        {
            get { return lstHiddenRecords; }
            set { lstHiddenRecords = value; }
        }
        /// <summary>
        /// Gets or sets any joinVia ID that should be associated with the sorted column fieldID
        /// </summary>
        public int sortedColumnJoinViaID
        {
            get { return nSortedColumnJoinViaID; }
            set { nSortedColumnJoinViaID = value; }
        }

        /// <summary>
        /// Gets or sets the Where Clause used when the where clause is being set explicitly
        /// </summary>
        public string WhereClause
        {
            get { return whereClause; }
            set { whereClause = value; }
        }

        public string DefaultCurrencySymbol
        {
            get { return _defaultCurrencySymbol; }
            set { _defaultCurrencySymbol = value; }
        }

        /// <summary>
        /// A copy of the dataset data as XML compressed by zip.
        /// </summary>
        public string XmlData { get; set; }

        /// <summary>
        /// Gets or sets the width of the table
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the width unit of the table
        /// </summary>
        public byte WidthUnit { get; set; }
        #endregion

        #region methods

        /// <summary>
        /// Creates the javascript for this GridDetail.
        /// </summary>
        /// <returns>The Json version on this GridDetail.</returns>
        public string createJavascript()
        {
            var serialiser = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serialiser.Serialize(this);
        }

        #endregion

    }

}
