using System;
using System.Web;
using System.Web.Caching;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using FWClasses;
using Spend_Management;

namespace FWClasses
{
    #region cDataGrid
    public class cDataGrid
    {
        private int nAccountId;
        private int nUserId;
        private cFWSettings fws;
        private bool bHeaders;
        private bool bFooters;
        private DataView tblview;
        private ArrayList columns;
        private ArrayList rows;
        private bool bDataBound;
        private DataSet dsetData;
        private cSourceColumnNames srcColumnNames;
        private int sSort;
        private string sSortColumn;
        private string sSortDirection;
        private string sCurpage;
        private int aGrid;
        private cDataGridColumn idColumn;
        private string tableWidth = "0";
        private string sAlign;
        private string sTableid;
        private string sTbodyid;
        private string sClass;
        private bool bAllowsorting = true;
        private string sEmptyText;
        private bool bIsSubFolder;
		private CurrentUser curUser;

#region properties
        public cSourceColumnNames SourceColumnNames
        {
            get { return srcColumnNames; }
            set { srcColumnNames = value; }
        }

        public bool isDataBound
        {
            get { return bDataBound; }
        }

        public string width
        {
            get { return tableWidth; }
            set { tableWidth = value; }
        }
        public bool allowsorting
        {
            get { return bAllowsorting; }
            set { bAllowsorting = value; }
        }
        public string align
        {
            get { return sAlign; }
            set { sAlign = value; }
        }
        public string tblclass
        {
            get { return sClass; }
            set { sClass = value; }
        }
        public string sortcolumn
        {
            get { return sSortColumn; }
        }
        private string sortdirection
        {
            get { return sSortDirection; }
        }
        private string curpage
        {
            get { return sCurpage; }
            set { sCurpage = value; }
        }
        public int grid
        {
            get { return aGrid; }

        }
        public string emptytext
        {
            get { return sEmptyText; }
            set { sEmptyText = value; }
        }
        public System.Collections.ArrayList gridcolumns
        {
            get { return columns; }
        }
        public System.Collections.ArrayList gridrows
        {
            get { return rows; }
        }
        public string tableid
        {
            get { return sTableid; }
            set { sTableid = value; }
        }
        public string tbodyid
        {
            get { return sTbodyid; }
            set { sTbodyid = value; }
        }
        public cDataGridColumn idcolumn
        {
            get { return idColumn; }
            set { idColumn = value; }
        }
        public int accountid
        {
            get { return nAccountId; }
        }
        public bool isSubFolder
        {
            get { return bIsSubFolder; }
            set { bIsSubFolder = value; }
        }
#endregion

		public cDataGrid(cFWSettings cFWS, CurrentUser curuser, System.Data.DataSet rcdsttemp, bool headers, bool footers)
        {
            if (rcdsttemp == null)
            {
                return;
            }

            fws = cFWS;
			curUser = curuser;
            nAccountId = fws.MetabaseCustomerId;
            nUserId = curuser.EmployeeID;
            bHeaders = headers;
            bFooters = footers;
            dsetData = rcdsttemp;

            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            columns = new ArrayList();
            rows = new ArrayList();

			tblview = rcdsttemp.Tables[0].DefaultView;

            if (bDataBound)
            {
                sCurpage = appinfo.Request.Url.PathAndQuery;
                if (appinfo.Request.QueryString["sortby"] != null)
                {
                    int gridsort = int.Parse(appinfo.Request.QueryString["sort"]);
                    sSort = gridsort;
                    sSortColumn = appinfo.Request.QueryString["sortby"];
                    sSortDirection = appinfo.Request.QueryString["direction"];
                    sortData();
                    saveDefaultSort(sSort, sSortColumn, sSortDirection);
                }
                else
                {
                    getDefaultSort();
                    sortData();
                }
            }
        }

        public void BindDataToGrid()
        {
            try
            {
                foreach (DataRow drow in dsetData.Tables[0].Rows)
                {
                    string desc = (string)drow[SourceColumnNames.Description];
                    string fieldtype = (string)drow[SourceColumnNames.FieldType];
                    bool cantotal = (bool)drow[SourceColumnNames.CanTotal];
                    string tablecolumn = (string)drow[SourceColumnNames.TableColumn];

                    cDataGridColumn col = new cDataGridColumn(desc, fieldtype, "", cantotal, tablecolumn);
                    columns.Add(col);
                }

                getData();

                bDataBound = true;

            }
            catch
            {
                
                throw;
            }
        }

        public void getData()
        {
            int i, x;
            int tblcol = 0;
            System.Collections.ArrayList cells;
            cDataGridRow clsrow;
            cDataGridCell clscell;
            cDataGridColumn curcol;
            rows.Clear();

            for (i = 0; i < tblview.Count; i++)
            {
                tblcol = 0;
                cells = new System.Collections.ArrayList();
                for (x = 0; x < columns.Count; x++)
                {

                    curcol = (cDataGridColumn)columns[x];
                    if (curcol.customcolumn == false)
                    {
                        clscell = new cDataGridCell(curcol, tblview[i][tblcol]);
                        tblcol++;
                    }
                    else
                    {
                        clscell = new cDataGridCell(curcol, curcol.description);
                    }
                    cells.Add(clscell);
                }
                clsrow = new cDataGridRow(cells);
                rows.Add(clsrow);
            }
        }

        private void sortData()
        {
            if (aGrid == sSort)
            {
                try
                {
                    tblview.Sort = sortcolumn + " " + sortdirection;
                }
                catch
                {
                }
            }
        }

        public void getDefaultSort()
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            
            cFWDBConnection db = new cFWDBConnection();
            db.DBOpen(fws, false);

            string strsql;
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select columnname, defaultorder from default_sorts where employeeid = @employeeid and gridid = @gridid";
            db.AddDBParam("@employeeid", nUserId, true);
            db.AddDBParam("@gridid", aGrid, false);
            reader = db.GetReader(strsql);
            
            while (reader.Read())
            {
                sSort = aGrid;
                if (reader.IsDBNull(0) == false)
                {
                    sSortColumn = reader.GetString(0);
                }
                if (reader.IsDBNull(1) == false)
                {
                    if (reader.GetByte(1) == 1)
                    {
                        sSortDirection = "asc";
                    }
                    else
                    {
                        sSortDirection = "desc";
                    }
                }
            }

            reader.Close();
            db.DBClose();
        }

        public void saveDefaultSort(int gridid, string columnname, string direction)
        {
            byte sortorder;
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            cFWDBConnection db = new cFWDBConnection();
            db.DBOpen(fws, false);

            string strsql;

            strsql = "delete from default_sorts where gridid = @gridid and employeeid = @employeeid";
            //TODO:  Distributed caching clear.
            
            db.AddDBParam("@employeeid", nUserId, true);
            db.AddDBParam("@gridid", gridid, false);
            db.ExecuteSQL(strsql);

            if (direction.ToLower() == "asc")
            {
                sortorder = 1;
            }
            else
            {
                sortorder = 2;
            }

            strsql = "insert into default_sorts (employeeid, gridid, columnname, defaultorder) " +
                "values (@employeeid, @gridid, @columnname, @defaultorder)";
            db.AddDBParam("@columnname", columnname, true);
            db.AddDBParam("@defaultorder", sortorder, false);
            db.ExecuteSQL(strsql);

            return;
        }

        public string CreateGrid()
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            output.Append("<table");
            if (tableid != "")
            {
                output.Append(" id=" + tableid);
            }
            if (align == "")
            {
                output.Append(" align=center");
            }
            else
            {
                output.Append(" align=\"" + align + "\"");
            }
            if (tblclass != "")
            {
                output.Append(" class=\"" + tblclass + "\"");
            }
            output.Append(" cellspacing=0");
            if (tableWidth != "0")
            {
                output.Append(" width=\"" + tableWidth + "\"");
            }
            output.Append(">");
            output.Append("<tbody");
            if (tbodyid != "")
            {
                output.Append(" id=\"" + tbodyid + "\"");
            }
            output.Append(">");
            if (bHeaders == true)
            {
                output.Append(generateHeader());
            }

            output.Append(generateTable());
            if (bFooters == true)
            {
                output.Append(generateFooter());
            }
            output.Append("</tbody>");
            output.Append("</table>");

            return output.ToString();
        }

        private string generateHeader()
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            int i;
            cDataGridColumn reqcolumn;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append("<tr>");
            string colpage;
            for (i = 0; i < columns.Count; i++)
            {

                reqcolumn = (cDataGridColumn)columns[i];
                colpage = curpage;
                if (reqcolumn.hidden == false)
                {
                    output.Append("<th");
                    if (reqcolumn.width != "")
                    {
                        output.Append(" width=\"" + reqcolumn.width + "\"");
                    }
                    if (reqcolumn.align != "")
                    {
                        output.Append(" align=\"" + reqcolumn.align + "\"");
                    }
                    output.Append(">");
                    if (reqcolumn.customcolumn == false)
                    {

                        if (colpage.IndexOf("?") != -1)
                        {
                            if (colpage.IndexOf("sort") != -1)
                            {
                                colpage = colpage.Remove(colpage.IndexOf("sort") - 1, colpage.Length - colpage.IndexOf("sort") + 1);
                                //colpage = colpage.Replace("sort=" + gridid + "&sortby=" + sortcolumn + "&direction=asc","");
                                //colpage = colpage.Replace("sort=" + gridid + "&sortby=" + sortcolumn + "&direction=desc","");
                            }
                            if (colpage.IndexOf("?") != -1)
                            {
                                if (colpage.IndexOf("?") != (colpage.Length - 1))
                                {
                                    colpage += "&";
                                }
                            }
                            else
                            {
                                colpage += "?";
                            }

                        }
                        else
                        {
                            colpage += "?";
                        }

                        colpage += "sort=" + (int)aGrid + "&sortby=" + reqcolumn.tablecolumn;
                        if (reqcolumn.tablecolumn == sortcolumn)
                        {
                            if (sortdirection == "asc")
                            {

                                colpage += "&direction=desc";
                            }
                            else
                            {
                                colpage += "&direction=asc";
                            }
                        }
                        else
                        {
                            colpage += "&direction=asc";
                        }
                        if (allowsorting)
                        {
                            output.Append("<a href=\"" + colpage + "");
                            output.Append("\">");
                        }
                    }
                    output.Append(reqcolumn.description);

                    string pathPrefix = ".";
                    if (isSubFolder)
                    {
                        pathPrefix = "..";
                    }

                    if (reqcolumn.tablecolumn == sortcolumn && reqcolumn.customcolumn == false)
                    {
                        if (sortdirection == "asc")
                        {
                            output.Append("&nbsp;<img src=" + pathPrefix + "/images/whitearrow_up.gif>");
                        }
                        else
                        {
                            output.Append("&nbsp;<img src=" + pathPrefix + "/images/whitearrow_down.gif>");
                        }
                    }
                    if (reqcolumn.customcolumn == false)
                    {
                        output.Append("</a>");
                    }
                    output.Append("</th>");
                }
            }
            output.Append("</tr>");

            return output.ToString();
        }

        private string generateTable()
        {
            int i;
            cCurrencies clscurrencies = new cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId);
            cGlobalCurrencies glCurrencies = new cGlobalCurrencies();
            cCurrency reqcurrency;
            string rownum = "row1";
            int x;
            string val;
            cDataGridRow clsrow;
            cDataGridCell reqcell;
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            if (rows.Count == 0 && sEmptyText != "")
            {
                output.Append("<tr>");
                output.Append("<td>" + emptytext + "</td>");
                output.Append("</tr>");
            }
            for (i = 0; i < rows.Count; i++)
            {
                clsrow = (cDataGridRow)rows[i];
                output.Append("<tr ");
                if (idcolumn != null)
                {
                    output.Append("id=\"" + clsrow.getCellByName(idcolumn.name).thevalue + "\"");
                }
                output.Append(">");
                for (x = 0; x < clsrow.rowCells.Count; x++)
                {
                    reqcell = (cDataGridCell)clsrow.rowCells[x];
                    if (reqcell.columninfo.hidden == false)
                    {
                        output.Append("<td class=\"" + rownum + "\" ");

                        switch (reqcell.columninfo.fieldtype)
                        {
                            case "X":
                                output.Append("align=\"center\"");
                                break;
                            case "C":
                            case "M":
                            case "N":
                            case "F":
                                output.Append("align=\"right\"");
                                break;
                        }

                        if (reqcell.bgcolor != "")
                        {
                            output.Append(" style=\"background-color:" + reqcell.bgcolor + ";\"");
                        }
                        output.Append(">");

                        if (reqcell.columninfo.listitems.count != 0)
                        {
                            if (reqcell.columninfo.listitems.exists(reqcell.thevalue) == true)
                            {
                                output.Append(reqcell.columninfo.listitems.getValue(reqcell.thevalue));
                            }
                        }
                        else if (reqcell.thevalue == null || reqcell.thevalue == DBNull.Value || reqcell.thevalue.ToString() == "")
                        {
                            output.Append("&nbsp;");
                        }
                        else
                        {
                            val = reqcell.thevalue.ToString();
                            switch (reqcell.columninfo.fieldtype)
                            {
                                case "S":
                                case "T":
                                case "N":
                                case "F":
                                    output.Append(reqcell.thevalue);
                                    break;
                                case "D":
                                    output.Append(DateTime.Parse(val).ToShortDateString());
                                    break;
                                //case "T":
                                //    output.Append(DateTime.Parse(val).ToString("dd/MM/yyyy HH:mm"));
                                //    break;
                                case "X":
                                    output.Append("<input type=\"checkbox\" disabled ");
                                    if (reqcell.thevalue.ToString().ToLower() == "yes" || reqcell.thevalue.ToString().ToLower() == "true")
                                    {
                                        output.Append("checked");
                                    }
                                    output.Append(">");
                                    break;
                                case "C":
                                    //switch (reqcell.columninfo.name)
                                    //{
                                        //case "Global Total":
                                        //    if (clsrow.getCellByName("globalbasecurrency") != null)
                                        //    {
                                        //        reqcurrency = clscurrencies.getCurrencyById((int)clsrow.getCellByName("globalbasecurrency").thevalue);
                                        //        output.Append(clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol);
                                        //    }
                                        //    break;
                                        //case "Total Prior To Convert":
                                        //    if (clsrow.getCellByName("originalcurrency") != null)
                                        //    {
                                        //        if (clsrow.getCellByName("originalcurrency").thevalue != DBNull.Value)
                                        //        {
                                        //            reqcurrency = clscurrencies.getCurrencyById((int)clsrow.getCellByName("originalcurrency").thevalue);
                                        //            output.Append(clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol);
                                        //        }
                                        //    }
                                        //    break;
                                        //default:
                                    if (clsrow.getCellByName("basecurrency") != null)
                                    {
                                        if (clsrow.getCellByName("basecurrency").thevalue != DBNull.Value)
                                        {
                                            if ((int)clsrow.getCellByName("basecurrency").thevalue != 0)
                                            {
                                                reqcurrency = clscurrencies.getCurrencyById((int)clsrow.getCellByName("basecurrency").thevalue);
                                                output.Append(glCurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol);
                                            }
                                            else
                                            {
                                                output.Append("£");
                                            }
                                        }
                                        else
                                        {
                                            output.Append("£");
                                        }
                                    }
                                            //break;
                                    //}

                                    output.Append(decimal.Parse(val).ToString("#,###,##0.00"));
                                    break;
                                case "M":
                                    output.Append(decimal.Parse(val).ToString("#,###,##0.00"));
                                    break;
                                //case "DT":
                                //    output.Append(DateTime.Parse(val).ToString("dd/MM/yyyy HH:mm"));
                                //    break;
                            }

                        }
                        output.Append("</td>");
                    }
                }
                if (rownum == "row1")
                {
                    rownum = "row2";
                }
                else
                {
                    rownum = "row1";
                }
                output.Append("</tr>");

            }
            return output.ToString();
        }

        public string getFooterValues()
        {
            int i;
            int x;
            decimal total = 0;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            cDataGridColumn reqcolumn;
            cDataGridRow reqrow;
            cDataGridCell reqcell;
            
            for (i = 0; i < columns.Count; i++)
            {
                reqcolumn = (cDataGridColumn)columns[i];
                if (reqcolumn.hidden == false)
                {

                    if (reqcolumn.cantotal == false)
                    {
                        output.Append(",");
                    }
                    else
                    {
                        total = 0;
                        for (x = 0; x < rows.Count; x++)
                        {

                            reqrow = (cDataGridRow)rows[x];
                            reqcell = (cDataGridCell)reqrow.rowCells[i];
                            total += Decimal.Parse(reqcell.thevalue.ToString());
                        }

                        switch (reqcolumn.fieldtype)
                        {
                            case "C":
                                output.Append(total.ToString("£#,###,##0.00"));
                                break;
                            case "M":
                                output.Append(total.ToString("#,###,##0.00"));
                                break;
                            case "N":
                                output.Append(int.Parse(total.ToString()));
                                break;
                        }
                        output.Append(",");
                    }

                }
            }

            if (output.Length != 0)
            {
                output.Remove(output.Length - 1, 1);
            }
            return output.ToString();
        }

        public string generateFooter()
        {
            int i;
            int x;
            decimal total = 0;
			cCurrencies clscurrencies = new cCurrencies(curUser.AccountID, curUser.CurrentSubAccountId);
            cGlobalCurrencies glCurrencies = new cGlobalCurrencies();
            cCurrency reqcurrency;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            cDataGridColumn reqcolumn;
            cDataGridRow reqrow;
            cDataGridCell reqcell;
            string symbol = "";
            output.Append("<tr id=\"footerRow\">");
            for (i = 0; i < columns.Count; i++)
            {
                reqcolumn = (cDataGridColumn)columns[i];
                if (reqcolumn.hidden == false)
                {
                    output.Append("<th ");
                    switch (reqcolumn.fieldtype)
                    {
                        case "C":
                        case "M":
                        case "N":
                            output.Append("style=\"text-align:right;\"");
                            break;
                    }
                    output.Append(">");
                    if (reqcolumn.cantotal == false)
                    {
                        output.Append("&nbsp;");
                    }
                    else
                    {
                        total = 0;
                        for (x = 0; x < rows.Count; x++)
                        {

                            reqrow = (cDataGridRow)rows[x];
                            reqcell = (cDataGridCell)reqrow.rowCells[i];
                            if (reqcell.thevalue != DBNull.Value)
                            {
                                total += Math.Round(Decimal.Parse(reqcell.thevalue.ToString()), 2, MidpointRounding.AwayFromZero);
                            }
                            //switch (reqcell.columninfo.name)
                            //{
                            //    case "Global Total":
                            //        if (reqrow.getCellByName("globalbasecurrency") != null)
                            //        {
                            //            if (reqrow.getCellByName("Global Total").thevalue != DBNull.Value)
                            //            {
                            //                reqcurrency = clscurrencies.getCurrencyById((int)reqrow.getCellByName("globalbasecurrency").thevalue);
                            //                symbol = clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol;
                            //            }
                            //        }
                            //        break;
                            //    case "Total Prior To Convert":
                            //        if (reqrow.getCellByName("originalcurrency") != null)
                            //        {
                            //            if (reqrow.getCellByName("originalcurrency").thevalue != DBNull.Value)
                            //            {
                            //                reqcurrency = clscurrencies.getCurrencyById((int)reqrow.getCellByName("originalcurrency").thevalue);
                            //                symbol = clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol;
                            //            }
                            //        }
                            //        break;
                            //    default:
                            if (reqrow.getCellByName("basecurrency") != null)
                            {
                                if (reqrow.getCellByName("basecurrency").thevalue != DBNull.Value)
                                {
                                    reqcurrency = clscurrencies.getCurrencyById((int)reqrow.getCellByName("basecurrency").thevalue);
                                    if (reqcurrency != null)
                                    {
                                        symbol = glCurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol;
                                    }
                                }
                                else
                                {
                                    symbol = "£";
                                }
                            }
                            //        break;
                            //}
                        }

                        switch (reqcolumn.fieldtype)
                        {
                            case "C":
                                output.Append(symbol);
                                output.Append(total.ToString("#,###,##0.00"));
                                break;
                            case "M":
                                output.Append(total.ToString("#,###,##0.00"));
                                break;
                            case "N":
                                //output.Append(int.Parse(total.ToString()));
                                output.Append(total);
                                break;
                        }
                    }
                    output.Append("</th>");
                }
            }
            output.Append("</tr>");

            return output.ToString();
        }
    }
    #endregion

    #region cDataGridRow
    public class cDataGridRow
    {
        System.Collections.ArrayList cells = new System.Collections.ArrayList();
        public cDataGridRow(System.Collections.ArrayList thecells)
        {
            cells = thecells;
        }

        public System.Collections.ArrayList rowCells
        {
            get { return cells; }
        }

        public cDataGridCell getCellByName(string name)
        {
            int i;
            cDataGridCell reqcell;
            for (i = 0; i < cells.Count; i++)
            {
                reqcell = (cDataGridCell)cells[i];
                if (reqcell.columninfo.name == name)
                {
                    return reqcell;
                }
            }
            return null;
        }
    }
    #endregion

    #region cDataGridColumn
    public class cDataGridColumn
    {
        private string sDescription;
        private string sName;
        private string sFieldtype;
        private string sComment;
        private bool bHidden;
        private bool bCustomColumn;
        private bool bCantotal;
        private cDataColumnList valuelist = new cDataColumnList();
        private string sWidth;
        private string sAlign;
        private string sTableColumn;

        public cDataGridColumn(string description, string fieldtype, string comment, bool cantotal, string tablecolumn)
        {
            sName = description;
            sDescription = description;
            sFieldtype = fieldtype;
            sComment = comment;
            bCantotal = cantotal;
            sTableColumn = tablecolumn;
        }

        public cDataGridColumn(string name, string description, string fieldtype, string comment, bool cantotal, bool custom)
        {
            sName = name;
            sDescription = description;
            sFieldtype = fieldtype;
            sComment = comment;
            bCustomColumn = custom;
        }

        public cDataGridColumn(string name, string description, string fieldtype, string comment, bool cantotal, bool custom, string tablecolumn)
        {
            sName = name;
            sDescription = description;
            sFieldtype = fieldtype;
            sComment = comment;
            bCustomColumn = custom;
            sTableColumn = tablecolumn;
        }

        public string description
        {
            get { return sDescription; }
            set { sDescription = value; }
        }

        public string name
        {
            get { return sName; }
        }
        public string fieldtype
        {
            get { return sFieldtype; }
            set { sFieldtype = value; }
        }
        public string comment
        {
            get { return sComment; }
        }
        public bool hidden
        {
            get { return bHidden; }
            set { bHidden = value; }
        }
        public bool customcolumn
        {
            get { return bCustomColumn; }
            set { bCustomColumn = value; }
        }
        public bool cantotal
        {
            get { return bCantotal; }
        }

        public cDataColumnList listitems
        {
            get { return valuelist; }
            set { valuelist = value; }
        }
        public string width
        {
            get { return sWidth; }
            set { sWidth = value; }
        }
        public string align
        {
            get { return sAlign; }
            set { sAlign = value; }
        }
        public string tablecolumn
        {
            get { return sTableColumn; }
        }
    }
    #endregion

    #region cDataGridCell
    public class cDataGridCell
    {
        private cDataGridColumn column;
        private object cellvalue;
        private string sBgcolor;

        public cDataGridCell(cDataGridColumn theColumn, object thevalue)
        {
            column = theColumn;
            cellvalue = thevalue;
        }

        public cDataGridColumn columninfo
        {
            get { return column; }
        }

        public object thevalue
        {
            get { return cellvalue; }
            set { cellvalue = value; }
        }

        public string bgcolor
        {
            get { return sBgcolor; }
            set { sBgcolor = value; }
        }
    }
    #endregion

    #region cDataColumnList
    public class cDataColumnList
    {
        private System.Collections.Hashtable items = new System.Collections.Hashtable();

        public void addItem(object key, string val)
        {
            items.Add(key, val);
        }

        public int count
        {
            get { return items.Count; }
        }

        public bool exists(object item)
        {
            return items.Contains(item);
        }

        public string getValue(object item)
        {
            return (string)items[item];
        }

    }
    #endregion

    #region cSourceColumnNames
    public class cSourceColumnNames
    {
        public string Description { get; set; }
        public string FieldType { get; set; }
        public string CanTotal { get; set; }
        public string TableColumn { get; set; }
    }
    #endregion
}
