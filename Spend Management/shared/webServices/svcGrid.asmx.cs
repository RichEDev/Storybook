using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Reflection;

using SpendManagementLibrary;
using SpendManagementLibrary.DocumentMerge;
using SpendManagementLibrary.Extentions;

namespace Spend_Management.shared.webServices
{
    using System.Web.UI.WebControls;
    using System.Xml;

    using Infragistics.WebUI.UltraWebGrid;

    using System.Web.UI.WebControls;

    using SpendManagementLibrary.Definitions.JoinVia;

    using SortDirection = SpendManagementLibrary.SortDirection;

    /// <summary>
    /// Summary description for svcGrid
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    [System.Web.Script.Services.ScriptService]
    public class svcGrid : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public string[] changePage(int accountid, string gridid, int pageNumber, string filter, cJSGridDetail gridDetails, string serviceClassForInitialiseRowEvent = "", string methodForInitialiseRowEvent = "")
        {
            ConvertFilterTicksToDateTime(accountid, gridDetails);
            CurrentUser user = cMisc.GetCurrentUser();

            var clstables = new cTables(accountid);
            var clsfields = new cFields(accountid);
            var basetable = clstables.GetTableByID(gridDetails.baseTableID);


            cGridNew clsgrid = this.InitaliseGrid(accountid, gridid, gridDetails, user, clsfields, basetable); 
            HookInitialiseRowEvent(serviceClassForInitialiseRowEvent, methodForInitialiseRowEvent, clsgrid);

            clsgrid.currentpagenumber = pageNumber;
            clsgrid.deletelink = gridDetails.deleteLink;
            clsgrid.editlink = gridDetails.editLink;
            clsgrid.enabledeleting = gridDetails.enableDeleting;
            clsgrid.enablepaging = gridDetails.enablePaging;
            clsgrid.enableupdating = gridDetails.enableUpdating;
            clsgrid.pagesize = gridDetails.pageSize;
            clsgrid.showfooters = gridDetails.showFooters;
            clsgrid.showheaders = gridDetails.showHeaders;
            clsgrid.enablearchiving = gridDetails.enableArchiving;
            clsgrid.EnableSorting = gridDetails.EnableSorting;
            clsgrid.archivelink = gridDetails.archiveLink;
            clsgrid.ArchiveField = gridDetails.archiveField;
            clsgrid.KeyField = gridDetails.keyField;
            clsgrid.DisplayFilter = gridDetails.displayFilter;
            clsgrid.FilterCriteria = filter;
            clsgrid.EnableSelect = gridDetails.enableSelect;
            clsgrid.SourceIsDataSet = gridDetails.isDataSet;
            clsgrid.EmptyText = gridDetails.emptyText;
            clsgrid.WhereClause = gridDetails.WhereClause;
            clsgrid.DefaultCurrencySymbol = gridDetails.DefaultCurrencySymbol;
            if (gridDetails.sortedColumnFieldID != Guid.Empty)
            {
                clsgrid.SortedColumn = clsgrid.getColumnByID(gridDetails.sortedColumnFieldID, gridDetails.sortedColumnJoinViaID);
            }
            clsgrid.SortDirection = gridDetails.sortDirection;
            clsgrid.GridSelectType = gridDetails.gridSelectType;
            foreach (cJSGridFilter jsfilter in gridDetails.filters)
            {
                if (string.IsNullOrEmpty(clsgrid.WhereClause))
                {
                    object[] value1 = FieldFilters.GetGridFilterWhereValues(jsfilter.values1.ToArray(), jsfilter.condition);
                    object[] values2 = (jsfilter.values2.Count > 0 ? jsfilter.values2.ToArray() : null);
                    JoinVia jv = (jsfilter.JoinVia == null) ? null : JoinVias.ConvertJSToC(jsfilter.JoinVia);                
                    clsgrid.addFilter(clsfields.GetFieldByID(jsfilter.fieldID), (ConditionType)jsfilter.condition, value1, values2, (ConditionJoiner)jsfilter.joiner, jv);
                }
                else
                {
                    clsgrid.addFilter(clsfields.GetFieldByID(jsfilter.fieldID), jsfilter.ParameterName, jsfilter.ParameterValue);
                }
                
            }
            if (gridDetails.cssClass != "")
            {
                clsgrid.CssClass = gridDetails.cssClass;
            }
            if (gridDetails.currencyId > 0)
            {
                clsgrid.CurrencyId = gridDetails.currencyId;
            }
            if (gridDetails.currencyColumnName != "")
            {
                clsgrid.CurrencyColumnName = gridDetails.currencyColumnName;
            }
            clsgrid.ServiceClassMethodForInitialiseRowEvent = gridDetails.ServiceClassMethodForInitialiseRowEvent;
            clsgrid.ServiceClassForInitialiseRowEvent = gridDetails.ServiceClassForInitialiseRowEvent;
            clsgrid.InitialiseRowGridInfo = gridDetails.InitialiseRowGridInfo;
            clsgrid.HiddenRecords = gridDetails.HiddenRecords;
            switch ((System.Web.UI.WebControls.UnitType)gridDetails.WidthUnit)
            {
                case System.Web.UI.WebControls.UnitType.Percentage:
                    clsgrid.Width = System.Web.UI.WebControls.Unit.Percentage(gridDetails.Width);
                    break;
                case System.Web.UI.WebControls.UnitType.Pixel:
                    clsgrid.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(gridDetails.Width));
                    break;
            }

            string[] gridData = clsgrid.generateGrid();
            return new string[] { gridid, gridData[1], gridData[0] };
        }

        [WebMethod(EnableSession = true)]
        public string[] sortGrid(int accountid, string gridid, string newsortcolumnid, string filter, cJSGridDetail gridDetails, int is_static, string serviceClassForInitialiseRowEvent = "", string methodForInitialiseRowEvent = "")
        {
            ConvertFilterTicksToDateTime(accountid, gridDetails);
            CurrentUser user = cMisc.GetCurrentUser();
            cTables clstables = new cTables(accountid);
            cFields clsfields = new cFields(accountid);
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            cTable basetable = clstables.GetTableByID(gridDetails.baseTableID);

            SpendManagementLibrary.SortDirection currentSortdirection = gridDetails.sortDirection;

            cGridNew clsgrid = this.InitaliseGrid(accountid, gridid, gridDetails, user, clsfields, basetable);; 
            
            HookInitialiseRowEvent(serviceClassForInitialiseRowEvent, methodForInitialiseRowEvent, clsgrid);

            clsgrid.currentpagenumber = 1;
            clsgrid.deletelink = gridDetails.deleteLink;
            clsgrid.editlink = gridDetails.editLink;
            clsgrid.enabledeleting = gridDetails.enableDeleting;
            clsgrid.enablepaging = gridDetails.enablePaging;
            clsgrid.enableupdating = gridDetails.enableUpdating;
            clsgrid.pagesize = gridDetails.pageSize;
            clsgrid.showfooters = gridDetails.showFooters;
            clsgrid.showheaders = gridDetails.showHeaders;
            clsgrid.enablearchiving = gridDetails.enableArchiving;
            clsgrid.archivelink = gridDetails.archiveLink;
            clsgrid.ArchiveField = gridDetails.archiveField;
            clsgrid.KeyField = gridDetails.keyField;
            clsgrid.DisplayFilter = gridDetails.displayFilter;
            clsgrid.EmptyText = gridDetails.emptyText;
            clsgrid.WhereClause = gridDetails.WhereClause;
            switch ((System.Web.UI.WebControls.UnitType)gridDetails.WidthUnit)
            {
                case System.Web.UI.WebControls.UnitType.Percentage:
                    clsgrid.Width = System.Web.UI.WebControls.Unit.Percentage(gridDetails.Width);
                    break;
                case System.Web.UI.WebControls.UnitType.Pixel:
                    clsgrid.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(gridDetails.Width));
                    break;
            }
            if (is_static == 0)
            {
                //cField newsortablefield = clsfields.GetFieldByID(new Guid(newsortcolumnid));
                //clsgrid.SortedColumn = clsgrid.getColumnByName(newsortablefield.FieldName);
                clsgrid.SortedColumn = clsgrid.getColumnByName(newsortcolumnid);
            }
            else
            {
                clsgrid.SortedColumn = clsgrid.getColumnByName(newsortcolumnid);
            }
            clsgrid.EnableSelect = gridDetails.enableSelect;
            clsgrid.GridSelectType = gridDetails.gridSelectType;
            clsgrid.SourceIsDataSet = gridDetails.isDataSet;
            clsgrid.DefaultCurrencySymbol = gridDetails.DefaultCurrencySymbol;
            bool blnToggle = false;
            if (clsgrid.SortedColumn.ColumnType == GridColumnType.Static)
            {
                if (((cStaticGridColumn)clsgrid.SortedColumn).staticFieldId == gridDetails.sortedColumnFieldID)
                    blnToggle = true;
            }
            else
            {
                if (((cFieldColumn)clsgrid.SortedColumn).field.FieldID == gridDetails.sortedColumnFieldID && (((cFieldColumn)clsgrid.SortedColumn).JoinVia == null || ((cFieldColumn)clsgrid.SortedColumn).JoinVia.JoinViaID == gridDetails.sortedColumnJoinViaID))
                    blnToggle = true;
            }

            if (blnToggle)
            {
                if (currentSortdirection == SpendManagementLibrary.SortDirection.Ascending)
                {
                    clsgrid.SortDirection = SpendManagementLibrary.SortDirection.Descending;
                }
                else
                {
                    clsgrid.SortDirection = SpendManagementLibrary.SortDirection.Ascending;
                }
            }
            else
            {
                clsgrid.SortDirection = SpendManagementLibrary.SortDirection.Ascending;
            }

            clsgrid.FilterCriteria = filter;
            foreach (cJSGridFilter jsfilter in gridDetails.filters)
            {
                if (string.IsNullOrEmpty(clsgrid.WhereClause))
                {
                    object[] value1 = FieldFilters.GetGridFilterWhereValues(jsfilter.values1.ToArray(), jsfilter.condition);
                    object[] values2 = (jsfilter.values2.Count > 0 ? jsfilter.values2.ToArray() : null);
                    JoinVia jv = (jsfilter.JoinVia == null) ? null : JoinVias.ConvertJSToC(jsfilter.JoinVia);             
                    clsgrid.addFilter(clsfields.GetFieldByID(jsfilter.fieldID), (ConditionType)jsfilter.condition, value1, values2, (ConditionJoiner)jsfilter.joiner, jv);
                }
                else
                {
                    clsgrid.addFilter(clsfields.GetFieldByID(jsfilter.fieldID), jsfilter.ParameterName, jsfilter.ParameterValue);
                }
            }
            if (is_static == 0)
            {
                clsgrid.updateEmployeeSortOrder(user);
            }
            if (gridDetails.cssClass != "")
            {
                clsgrid.CssClass = gridDetails.cssClass;
            }
            if (gridDetails.currencyId > 0)
            {
                clsgrid.CurrencyId = gridDetails.currencyId;
            }
            if (gridDetails.currencyColumnName != "")
            {
                clsgrid.CurrencyColumnName = gridDetails.currencyColumnName;
            }
            clsgrid.ServiceClassMethodForInitialiseRowEvent = gridDetails.ServiceClassMethodForInitialiseRowEvent;
            clsgrid.ServiceClassForInitialiseRowEvent = gridDetails.ServiceClassForInitialiseRowEvent;
            clsgrid.InitialiseRowGridInfo = gridDetails.InitialiseRowGridInfo;
            clsgrid.HiddenRecords = gridDetails.HiddenRecords;

            string[] gridData = clsgrid.generateGrid();

            if (is_static == 0)
            {
                return new string[] { gridid, gridData[1], gridData[0], ((cFieldColumn)clsgrid.SortedColumn).field.FieldID.ToString(), clsgrid.SortDirection.ToString() };
            }
            else
            {
                return new string[] { gridid, gridData[1], gridData[0], ((cStaticGridColumn)clsgrid.SortedColumn).staticFieldId.ToString(), clsgrid.SortDirection.ToString() };
            }
        }

        private static void ConvertFilterTicksToDateTime(int accountid, cJSGridDetail gridDetails)
        {
            // Need to convert date, date-time and time filters back from ticks (weren't being serialized properly as datetimes)
            cFields fields = new cFields(accountid);
            foreach (var jsGridFilter in from jsGridFilter in gridDetails.filters
                                         let field = fields.GetFieldByID(jsGridFilter.fieldID)
                                         where field.FieldType == "T" || field.FieldType == "D" || field.FieldType == "DT"
                                         select jsGridFilter)
            {
                if (jsGridFilter.values1 != null && jsGridFilter.values1.Count > 0 && jsGridFilter.values1[0] != null)
                {
                    string val1 = jsGridFilter.values1[0].ToString().Trim();
                    if (String.IsNullOrEmpty(val1))
                    {
                        jsGridFilter.values1[0] = "";
                    }
                    else
                    {
                        switch (jsGridFilter.condition)
                        {
                            case (int)ConditionType.DoesNotEqual:
                            case (int)ConditionType.LessThan:
                            case (int)ConditionType.LessThanEqualTo:
                            case (int)ConditionType.GreaterThan:
                            case (int)ConditionType.GreaterThanEqualTo:
                            case (int)ConditionType.After:
                            case (int)ConditionType.Before:
                            case (int)ConditionType.On:
                            case (int)ConditionType.OnOrAfter:
                            case (int)ConditionType.OnOrBefore:
                            case (int)ConditionType.NotOn:
                            case (int)ConditionType.Between:

                                jsGridFilter.values1[0] = new DateTime(Convert.ToInt64(val1));
                                break;
                        }
                    }
                }
                if (jsGridFilter.values2 != null && jsGridFilter.values2.Count > 0 && jsGridFilter.values2[0] != null)
                {
                    string val2 = jsGridFilter.values2[0].ToString().Trim();
                    if (String.IsNullOrEmpty(val2))
                    {
                        jsGridFilter.values2[0] = "";
                    }
                    else
                    {
                        switch (jsGridFilter.condition)
                        {
                            case (int)ConditionType.DoesNotEqual:
                            case (int)ConditionType.LessThan:
                            case (int)ConditionType.LessThanEqualTo:
                            case (int)ConditionType.GreaterThan:
                            case (int)ConditionType.GreaterThanEqualTo:
                            case (int)ConditionType.After:
                            case (int)ConditionType.Before:
                            case (int)ConditionType.On:
                            case (int)ConditionType.OnOrAfter:
                            case (int)ConditionType.OnOrBefore:
                            case (int)ConditionType.NotOn:
                            case (int)ConditionType.Between:

                                jsGridFilter.values2[0] = new DateTime(Convert.ToInt64(val2));
                                break;
                        }
                    }
                }
            }
        }

        private static void HookInitialiseRowEvent(string serviceClassForInitialiseRowEvent, string methodForInitialiseRowEvent, cGridNew grid)
        {
            // use reflection to hook in to a webservice's delegate function for the initialise row event.
            if (!string.IsNullOrEmpty(serviceClassForInitialiseRowEvent))
            {
                Type type = Type.GetType(serviceClassForInitialiseRowEvent);
                MethodInfo mi = type.GetMethod(methodForInitialiseRowEvent, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance);
                var svcInstance = Activator.CreateInstance(type);
                grid.InitialiseRow += (cGridNew.InitialiseRowEvent)Delegate.CreateDelegate(typeof(cGridNew.InitialiseRowEvent), svcInstance, mi, true);
            }
            grid.ServiceClassForInitialiseRowEvent = serviceClassForInitialiseRowEvent;
            grid.ServiceClassMethodForInitialiseRowEvent = methodForInitialiseRowEvent;
        }

        [WebMethod(EnableSession = true)]
        public string[] filterGrid(int accountid, string gridid, string filter, cJSGridDetail gridDetails, string serviceClassForInitialiseRowEvent = "", string methodForInitialiseRowEvent = "")
        {
            return filterGridData(accountid, gridid, filter, gridDetails, false, string.Empty, serviceClassForInitialiseRowEvent, methodForInitialiseRowEvent);
        }

        [WebMethod(EnableSession = true)]
        public string[] filterGridByCmb(int accountid, string gridid, string cmbVal, cJSGridDetail gridDetails, string serviceClassForInitialiseRowEvent = "", string methodForInitialiseRowEvent = "")
        {
            return filterGridData(accountid, gridid, cmbVal, gridDetails, true, string.Empty, serviceClassForInitialiseRowEvent, methodForInitialiseRowEvent);
        }

        /// <summary>
        /// Filter a grid by drop down list.
        /// 
        /// This work in conjunction with the "_Filter" text box.
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="gridid"></param>
        /// <param name="cmbVal"></param>
        /// <param name="gridDetails"></param>
        /// <param name="columnName"></param>
        /// <param name="serviceClassForInitialiseRowEvent"></param>
        /// <param name="methodForInitialiseRowEvent"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string[] filterGridByCombo(int accountid, string gridid, string cmbVal, cJSGridDetail gridDetails, string columnName,  string serviceClassForInitialiseRowEvent = "", string methodForInitialiseRowEvent = "")
        {
            return filterGridData(accountid, gridid, cmbVal, gridDetails, false, columnName, serviceClassForInitialiseRowEvent, methodForInitialiseRowEvent);
        }


        /// <summary>
        /// Method to filter an existing grid
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="gridid"></param>
        /// <param name="filter"></param>
        /// <param name="gridDetails"></param>
        /// <param name="archiveFlag"></param>  True if filter = 0, 1 or 2 where 0 = all 1 = archived and 2 = unarchived
        /// <param name="filterColumn">The column to filter on if no archive and not filtering on sort column</param>
        /// <param name="serviceClassForInitialiseRowEvent"></param>
        /// <param name="methodForInitialiseRowEvent"></param>
        /// <returns></returns>
        private string[] filterGridData(int accountid, string gridid, string filter, cJSGridDetail gridDetails, bool archiveFlag, string filterColumn = "", string serviceClassForInitialiseRowEvent = "", string methodForInitialiseRowEvent = "")
        {
            ConvertFilterTicksToDateTime(accountid, gridDetails);
            CurrentUser user = cMisc.GetCurrentUser();

            var clstables = new cTables(accountid);
            var clsfields = new cFields(accountid);
            var basetable = clstables.GetTableByID(gridDetails.baseTableID);

            cGridNew clsgrid = this.InitaliseGrid(accountid, gridid, gridDetails, user, clsfields, basetable);

            HookInitialiseRowEvent(serviceClassForInitialiseRowEvent, methodForInitialiseRowEvent, clsgrid);

            clsgrid.deletelink = gridDetails.deleteLink;
            clsgrid.editlink = gridDetails.editLink;
            clsgrid.enabledeleting = gridDetails.enableDeleting;
            clsgrid.enablepaging = gridDetails.enablePaging;
            clsgrid.enableupdating = gridDetails.enableUpdating;
            clsgrid.DisplayFilter = gridDetails.displayFilter;
            clsgrid.pagesize = gridDetails.pageSize;
            clsgrid.showfooters = gridDetails.showFooters;
            clsgrid.showheaders = gridDetails.showHeaders;
            clsgrid.enablearchiving = gridDetails.enableArchiving;
            clsgrid.archivelink = gridDetails.archiveLink;
            clsgrid.ArchiveField = gridDetails.archiveField;
            clsgrid.KeyField = gridDetails.keyField;
            if (!archiveFlag && string.IsNullOrEmpty(filterColumn))
            {
                clsgrid.FilterCriteria = filter;
            }
            else
            {
                clsgrid.FilterCriteria = gridDetails.FilterCriteria;
            }

            clsgrid.EnableSelect = gridDetails.enableSelect;
            clsgrid.SourceIsDataSet = gridDetails.isDataSet;
            clsgrid.EmptyText = gridDetails.emptyText;
            clsgrid.WhereClause = gridDetails.WhereClause;
            clsgrid.DefaultCurrencySymbol = gridDetails.DefaultCurrencySymbol;
            if (gridDetails.sortedColumnFieldID != Guid.Empty)
            {
                clsgrid.SortedColumn = clsgrid.getColumnByID(gridDetails.sortedColumnFieldID, gridDetails.sortedColumnJoinViaID);
            }
            clsgrid.SortDirection = gridDetails.sortDirection;
            clsgrid.GridSelectType = gridDetails.gridSelectType;
            foreach (cJSGridFilter jsfilter in gridDetails.filters)
            {
                if (string.IsNullOrEmpty(clsgrid.WhereClause))
                {
                    object[] value1 = FieldFilters.GetGridFilterWhereValues(jsfilter.values1.ToArray(), jsfilter.condition);
                    object[] values2 = (jsfilter.values2.Count > 0 ? jsfilter.values2.ToArray() : null);
                    JoinVia jv = (jsfilter.JoinVia == null) ? null : JoinVias.ConvertJSToC(jsfilter.JoinVia);              
                    clsgrid.addFilter(clsfields.GetFieldByID(jsfilter.fieldID), (ConditionType)jsfilter.condition, value1, values2, (ConditionJoiner)jsfilter.joiner, jv);
                }
                else
                {
                    clsgrid.addFilter(clsfields.GetFieldByID(jsfilter.fieldID), jsfilter.ParameterName, jsfilter.ParameterValue);
                }
            }
            if (archiveFlag)
            {
                clsgrid.clearFiltersForField(((cFieldColumn)clsgrid.getColumnByName("archived")).field);

                switch (filter)
                {
                    case "0": //Un-Archived only
                        clsgrid.addFilter(((cFieldColumn)clsgrid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
                        break;
                    case "1"://Archived only
                        clsgrid.addFilter(((cFieldColumn)clsgrid.getColumnByName("archived")).field, ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.And);
                        break; 
                    default: // Show everything
                        break;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filter))
                {
                    clsgrid.clearFiltersForField(((cFieldColumn)clsgrid.getColumnByName(filterColumn)).field);
                    if (filter == "[None]")
                    {
                        clsgrid.WhereClause = clsgrid.WhereClause.Replace(
                            string.Format("{0} = @{1}", filterColumn, filterColumn), "");
                        if (clsgrid.WhereClause.EndsWith(" AND "))
                        {
                            var currentWhere = clsgrid.WhereClause;
                            clsgrid.WhereClause = clsgrid.WhereClause.Substring(0, currentWhere.Length - 4);
                        }
                    }
                    else
                    {
                        if (!clsgrid.WhereClause.Contains("@" + filterColumn))
                        {
                            if (!string.IsNullOrEmpty(clsgrid.WhereClause))
                            {
                                clsgrid.WhereClause = clsgrid.WhereClause + " AND ";
                            }

                            clsgrid.WhereClause = string.Format("{0}{1} = @{2}", clsgrid.WhereClause, filterColumn, filterColumn);
                        }

                        clsgrid.addFilter(((cFieldColumn)clsgrid.getColumnByName(filterColumn)).field, "@" + filterColumn, filter); ;        
                    }
                }
            }
            if (gridDetails.cssClass != "")
            {
                clsgrid.CssClass = gridDetails.cssClass;
            }
            if (gridDetails.currencyId > 0)
            {
                clsgrid.CurrencyId = gridDetails.currencyId;
            }
            if (gridDetails.currencyColumnName != "")
            {
                clsgrid.CurrencyColumnName = gridDetails.currencyColumnName;
            }
            clsgrid.ServiceClassMethodForInitialiseRowEvent = gridDetails.ServiceClassMethodForInitialiseRowEvent;
            clsgrid.ServiceClassForInitialiseRowEvent = gridDetails.ServiceClassForInitialiseRowEvent;
            clsgrid.InitialiseRowGridInfo = gridDetails.InitialiseRowGridInfo;
            clsgrid.HiddenRecords = gridDetails.HiddenRecords;

            switch ((System.Web.UI.WebControls.UnitType)gridDetails.WidthUnit)
            {
                case System.Web.UI.WebControls.UnitType.Percentage:
                    clsgrid.Width = System.Web.UI.WebControls.Unit.Percentage(gridDetails.Width);
                    break;
                case System.Web.UI.WebControls.UnitType.Pixel:
                    clsgrid.Width = System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(gridDetails.Width));
                    break;
            }

            string[] gridData = clsgrid.generateGrid();
            return new string[] { gridid, gridData[1], gridData[0] };
        }

        /// <summary>
        /// The function to initialise a grid from the XML.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <param name="gridid">
        /// The gridid.
        /// </param>
        /// <param name="gridDetails">
        /// The grid details.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="clsfields"> 
        /// The clsfields.
        /// </param>
        /// <param name="basetable">
        /// The basetable.
        /// </param>
        /// <returns>
        /// The <see cref="cGridNew"/>.
        /// </returns>
        private cGridNew InitaliseGrid(int accountid, string gridid, cJSGridDetail gridDetails, CurrentUser user, cFields clsfields, cTable basetable)
        {
            cGridNew clsgrid;
            List<cNewGridColumn> columns;
            if (gridDetails.isDataSet && gridDetails.XmlData != null)
            {
                using (var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(gridDetails.XmlData.Decompress())))
                {
                    var gridDataSet = new DataSet();
                    gridDataSet.ReadXml(xmlStream);
                    var firstColumn = gridDetails.columns[0];
                    if (firstColumn.fieldID == Guid.Empty && (firstColumn.onClickCommand == null || firstColumn.onClickCommand.Length == 0))
                    {
                        clsgrid = new cGridNew(user, gridDataSet, gridid);
                    }
                    else
                    {
                        columns = this.GenerateColumnFromArray(gridDetails.columns, clsfields);
                        clsgrid = new cGridNew(user, gridDataSet, gridid, basetable.TableID, columns);
                    }
                }
            }
            else
            {
                columns = this.GenerateColumnFromArray(gridDetails.columns, clsfields);
                clsgrid = new cGridNew(accountid, user.EmployeeID, gridid, basetable, columns);
            }

            return clsgrid;
        }


        private List<cNewGridColumn> GenerateColumnFromArray(List<cJSGridColumn> columnids, cFields clsfields)
        {             
            List<cNewGridColumn> columns = new List<cNewGridColumn>();
            SerializableDictionary<string, string> valuelist;
            GridColumnType columntype;
            cNewGridColumn column = null;
            bool hidden;
            Guid fieldid;
            string tooltip;

            foreach (cJSGridColumn gridCol in columnids)
            {
                hidden = gridCol.hidden;
                columntype = gridCol.columnType;
                switch (columntype)
                {
                    case GridColumnType.Field:
                        fieldid = gridCol.fieldID;
                        column = this.GetGridColumn(gridCol, fieldid, clsfields);

                        if (column != null)
                        {
                            column.hidden = hidden;
                            column.HeaderText = gridCol.HeaderText;
                            column.ForceSingleLine = gridCol.ForceSingleLine;
                            column.CustomDateFormat = gridCol.CustomDateFormat;
                            column.Alias = gridCol.Alias;
                            valuelist = gridCol.valuelist;
                            foreach (KeyValuePair<string, string> kvp in valuelist)
                            {
                                ((cFieldColumn)column).addValueListItem(int.Parse(kvp.Key), kvp.Value.Replace("&quot;", "\""));
                            }

                            if (gridCol.PixelWidth > 0)
                            {
                                column.Width = Unit.Pixel(gridCol.PixelWidth);
                            }

                            columns.Add(column);
                        }
                        break;
                    case GridColumnType.Static:
                        string fieldname = gridCol.staticFieldName;
                        fieldid = gridCol.staticFieldID;
                        column = new cStaticGridColumn(fieldname, fieldid);
                        if (column != null)
                        {
                            column.hidden = hidden;
                            columns.Add(column);
                        }
                        break;
                    case GridColumnType.Event:
                        string altText = gridCol.alternateText;
                        string hyperlink = gridCol.hyperlinkText;
                        string iconPath = gridCol.iconPath;
                        string onclickCmd = gridCol.onClickCommand;
                        tooltip = gridCol.tooltip;
                        string colID = gridCol.ID;
                        if (string.IsNullOrEmpty(hyperlink))
                        {
                            column = new cEventColumn(colID, iconPath, onclickCmd, altText, tooltip);
                        }
                        else
                        {
                            column = new cEventColumn(colID, hyperlink, onclickCmd, tooltip);
                        }

                        if (column != null)
                        {
                            column.hidden = hidden;
                            columns.Add(column);
                        }
                        break;
                    case GridColumnType.ValueIcon:

                        var valueField = clsfields.getFieldByFieldName(gridCol.ValueColumn.ID);
                        var valueColumn = new cFieldColumn(valueField);
                        column = new ValueIconEventColumn(gridCol.ID, valueColumn, gridCol.HeaderIconPath, gridCol.HeaderTooltip, gridCol.iconPath, gridCol.tooltip, gridCol.onClickCommand, gridCol.Options);
                        column.hidden = hidden;
                        columns.Add(column);

                        break;
                    case GridColumnType.TwoState:
                        //Field data
                        fieldid = gridCol.fieldID;
                        cField fieldTS = clsfields.GetFieldByID(fieldid);
                        if (fieldTS.GenList && fieldTS.ListItems.Count == 0 && fieldTS.FieldSource == cField.FieldSourceType.CustomEntity)
                        {
                            cField relatedFieldValueField = fieldTS.GetLookupTable().GetKeyField();
                            column = new cFieldColumn(fieldTS, relatedFieldValueField);
                        }
                        else if (gridCol.JoinVia != null) //  && gridCol.JoinVia.JoinViaID > 0 -- Summary Grids used "virtual" joinVias that are zero!
                        {
                            column = new cFieldColumn(fieldTS, JoinVias.ConvertJSToC(gridCol.JoinVia));
                        }
                        else
                        {
                            column = new cFieldColumn(fieldTS);
                        }

                        if (column != null)
                        {
                            column.hidden = hidden;
                            valuelist = gridCol.valuelist;
                            foreach (KeyValuePair<string, string> kvp in valuelist)
                            {
                                ((cFieldColumn)column).addValueListItem(int.Parse(kvp.Key), kvp.Value.Replace("&quot;", "\""));
                            }

                            columns.Add(column);
                        }


                        columns.Add(cTwoStateEventColumn.FromValueColumn(gridCol.ID, (cFieldColumn)column, gridCol.StateOneValue, gridCol.StateTwoValue, gridCol.IconPathStateOne, gridCol.OnClickCommandStateOne, gridCol.AlternateTextStateOne, gridCol.TooltipStateOne,gridCol.IconPathStateTwo, gridCol.OnClickCommandStateTwo, gridCol.AlternateTextStateTwo,  gridCol.TooltipStateTwo));

                        if (column != null)
                        {
                            columns.Remove(column);
                        }
                        break;
                    default:
                        break;
                }
            }

            return columns;
        }

        private cNewGridColumn GetGridColumn(cJSGridColumn gridCol, Guid fieldid, cFields fields)
        {
            if (fieldid == Guid.Empty)
            {
                // Must be an XML data stream that has not come from a query.
                return new cFieldColumn(new cField(), gridCol.HeaderText);
            }
            else
            {
                cField field = fields.GetFieldByID(fieldid);
                if (field.GenList && field.ListItems.Count == 0 && field.FieldSource == cField.FieldSourceType.CustomEntity)
                {
                    cField relatedFieldValueField = field.GetLookupTable().GetKeyField();
                    return new cFieldColumn(field, relatedFieldValueField);
                }

                else if (gridCol.JoinVia != null) //  && gridCol.JoinVia.JoinViaID > 0 -- Summary Grids used "virtual" joinVias that are zero!
                {
                    return new cFieldColumn(field, JoinVias.ConvertJSToC(gridCol.JoinVia));
                }
                else if (!string.IsNullOrEmpty(gridCol.Alias))
                {
                    return new cFieldColumn(field, gridCol.Alias);
                }
                else
                {
                    return new cFieldColumn(field);
                }
            }

        }
    }
}
