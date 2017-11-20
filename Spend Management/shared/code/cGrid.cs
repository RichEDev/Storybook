using System;
using System.Configuration;
using SpendManagementLibrary;
using Spend_Management;
using expenses;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for cGrid.
    /// </summary>
    public class cGrid
    {
        private System.Collections.ArrayList columns = new System.Collections.ArrayList();

        private System.Collections.ArrayList rows = new System.Collections.ArrayList();

        private bool bHeaders, bFooters;

        //System.Data.DataSet rcdst;
        private System.Data.DataView tblview;

        private string tableWidth = "0";

        private string sAlign;

        private string sTableid;

        private string sTbodyid;

        private string sClass;

        private Grid sSort;

        private string sSortColumn;

        private string sSortDirection;

        private string sCurpage;

        //private string sGridid = "";
        private Grid aGrid;

        private cGridColumn idColumn;

        private int nAccountid;

        private bool bAllowsorting = true;

        private string sEmptyText;

        private readonly CurrentUser _currentUser;

        public cGrid(System.Data.DataSet rcdsttemp, bool headers, bool footers)
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            if (rcdsttemp == null)
            {
                return;
            }


            this._currentUser = cMisc.GetCurrentUser();
            nAccountid = this._currentUser.AccountID;
            bHeaders = headers;
            bFooters = footers;
            //rcdst = rcdsttemp;
            tblview = rcdsttemp.Tables[0].DefaultView;
            getColumns(rcdsttemp);
            getData();

            sCurpage = appinfo.Request.Url.PathAndQuery;
            if (appinfo.Request.QueryString["sortby"] != null)
            {
                Grid gridsort = (Grid)int.Parse(appinfo.Request.QueryString["sort"]);
                sSort = gridsort;
                sSortColumn = appinfo.Request.QueryString["sortby"];
                sSortDirection = appinfo.Request.QueryString["direction"];
                sortData();
                this._currentUser.Employee.GetGridSortOrders().Add(gridsort, sSortColumn, sSortDirection);
            }
            else
            {
                getDefaultSort();
                sortData();
            }
        }

        public cGrid(int accountID, System.Data.DataSet rcdsttemp, bool headers, bool footers, Grid gridid)
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            this._currentUser = cMisc.GetCurrentUser();
            if (rcdsttemp == null)
            {
                return;
            }

            nAccountid = accountID;
            bHeaders = headers;
            bFooters = footers;
            tblview = rcdsttemp.Tables[0].DefaultView;
            getColumns(rcdsttemp);
            getData();
            aGrid = gridid;
            sCurpage = appinfo.Request.Url.PathAndQuery;
            if (appinfo.Request.QueryString["sortby"] != null)
            {
                Grid gridsort = (Grid)int.Parse(appinfo.Request.QueryString["sort"]);
                if (gridsort == gridid)
                {
                    sSort = gridsort;
                    sSortColumn = appinfo.Request.QueryString["sortby"];
                    sSortDirection = appinfo.Request.QueryString["direction"];
                    sortData();
                    this._currentUser.Employee.GetGridSortOrders().Add(gridsort, sSortColumn, sSortDirection);
                }
                else
                {
                    getDefaultSort();
                    sortData();
                }
            }
            else
            {
                getDefaultSort();
                sortData();
            }
        }

        public cGrid(System.Data.DataSet rcdsttemp, bool headers, bool footers, Grid gridid, string defaultSort)
        {
            System.Web.HttpApplication appinfo = System.Web.HttpContext.Current.ApplicationInstance;
            this._currentUser = cMisc.GetCurrentUser();
            if (rcdsttemp == null)
            {
                return;
            }

            nAccountid = this._currentUser.AccountID;
            bHeaders = headers;
            bFooters = footers;
            //rcdst = rcdsttemp;
            tblview = rcdsttemp.Tables[0].DefaultView;
            getColumns(rcdsttemp);
            getData();
            aGrid = gridid;
            sCurpage = appinfo.Request.Url.PathAndQuery;
            if (appinfo.Request.QueryString["sortby"] != null)
            {
                Grid gridsort = (Grid)int.Parse(appinfo.Request.QueryString["sort"]);
                sSort = gridsort;
                sSortColumn = appinfo.Request.QueryString["sortby"];
                sSortDirection = appinfo.Request.QueryString["direction"];
                sortData();
                this._currentUser.Employee.GetGridSortOrders().Add(gridsort, sSortColumn, sSortDirection);
            }
            else if (defaultSort != "")
            {
                sSort = aGrid;
                sSortColumn = defaultSort;
                sSortDirection = "asc";
                sortData();
            }
            else
            {
                getDefaultSort();
                sortData();
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

        public System.Data.DataView data
        {
            get
            {
                return tblview;
            }
        }

        private void getColumns(System.Data.DataSet rcdsttemp)
        {
            int i;
            int accountid;
            string description = "";
            string tablecolumn;
            cFields clsfields = new cFields(nAccountid);
            cField reqfield;
            accountid = this._currentUser.AccountID;

            cMisc clsmisc = new cMisc(accountid);
            cUserDefined clsuserdefined = new cUserDefined(accountid);

            cGridColumn column;
            for (i = 0; i < tblview.Table.Columns.Count; i++)
            {
                try
                {

                    reqfield = clsfields.getFieldByName(tblview.Table.Columns[i].ColumnName);
                    if (reqfield == null)
                    {
                        reqfield = clsfields.GetFieldByID(new Guid(tblview.Table.Columns[i].ColumnName));
                    }
                    if (reqfield == null) //userdefined?
                    {
                        reqfield = clsuserdefined.getUserDefinedDefinition(int.Parse(rcdsttemp.Tables[0].Columns[i].ColumnName));
                    }

                    switch (reqfield.FieldID.ToString())
                    {
                        case "7cf61909-8d25-4230-84a9-f5701268f94b":
                            description = clsmisc.GetGeneralFieldByCode("otherdetails").description;
                            break;
                        case "4D0F2409-0705-4F0F-9824-42057B25AEBE":
                            description = clsmisc.GetGeneralFieldByCode("organisation").description;
                            break;
                        case "1ee53ae2-2cdf-41b4-9081-1789adf03459":
                            description = clsmisc.GetGeneralFieldByCode("currency").description;
                            break;
                        case "ec527561-dfee-48c7-a126-0910f8e031b0":
                            description = clsmisc.GetGeneralFieldByCode("country").description;
                            break;
                        case "af839fe7-8a52-4bd1-962c-8a87f22d4a10":
                            description = clsmisc.GetGeneralFieldByCode("reason").description;
                            break;
                        case "3d8c699e-9e0e-4484-b821-b49b5cb4c098":
                            description = clsmisc.GetGeneralFieldByCode("from").description;
                            break;
                        case "359dfac9-74e6-4be5-949f-3fb224b1cbfc":
                            description = clsmisc.GetGeneralFieldByCode("costcode").description;
                            break;
                        case "9617a83e-6621-4b73-b787-193110511c17":
                            description = clsmisc.GetGeneralFieldByCode("department").description;
                            break;
                        case "6d06b15e-a157-4f56-9ff2-e488d7647219":
                            description = clsmisc.GetGeneralFieldByCode("projectcode").description;
                            break;
                        default:
                            description = reqfield.Description;
                            break;
                    }
                    tablecolumn = reqfield.FieldID.ToString();
                    column = new cGridColumn(description, reqfield.FieldType, reqfield.Comment, reqfield.CanTotal, tablecolumn);

                }
                catch
                {
                    tablecolumn = rcdsttemp.Tables[0].Columns[i].ColumnName;
                    column = new cGridColumn(rcdsttemp.Tables[0].Columns[i].ColumnName, "S", "", false, tablecolumn);
                }
                columns.Add(column);
            }
        }

        public string CreateGrid()
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            output.Append("<table");
            if (tableid != "")
            {
                output.Append(" id=\"" + tableid + "\"");
            }
            if (align == "")
            {
                output.Append(" align=\"center\"");
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
            int i;
            cGridColumn reqcolumn;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append("<tr>");
            string colpage;
            for (i = 0; i < columns.Count; i++)
            {

                reqcolumn = (cGridColumn)columns[i];
                colpage = curpage;
                if (reqcolumn.hidden == false)
                {
                    output.Append("<th");
                    if(reqcolumn.customcolumn == true)
                    {
                        output.Append(" width=\"21px\"");
                    }
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
                    if (reqcolumn.tablecolumn == sortcolumn && reqcolumn.customcolumn == false)
                    {
                        if (sortdirection == "asc")
                        {
                            output.Append("&nbsp;<img src=" + cMisc.Path + "/images/whitearrow_up.gif>");
                        }
                        else
                        {
                            output.Append("&nbsp;<img src=" + cMisc.Path + "/images/whitearrow_down.gif>");
                        }
                    }
                    if (reqcolumn.customcolumn == false)
                    {
                        output.Append("</a>");
                    }
                    output.Append("</th>\n");
                }
            }
            output.Append("</tr>\n\n");

            return output.ToString();
        }

        public void getDefaultSort()
        {
            var employee = this._currentUser.Employee;
            cGridSort gridSort = employee.GetGridSortOrders().GetBy(aGrid);
            if (gridSort != null)
            {
                sSortDirection = gridSort.sortorder == 1 ? "asc" : "desc";
                sSortColumn = gridSort.columnname;
                sSort = aGrid;
            }
        }

        public string width
        {
            get
            {
                return tableWidth;
            }
            set
            {
                tableWidth = value;
            }
        }

        public bool allowsorting
        {
            get
            {
                return bAllowsorting;
            }
            set
            {
                bAllowsorting = value;
            }
        }

        public string align
        {
            get
            {
                return sAlign;
            }
            set
            {
                sAlign = value;
            }
        }

        public string tblclass
        {
            get
            {
                return sClass;
            }
            set
            {
                sClass = value;
            }
        }

        public string sortcolumn
        {
            get
            {
                return sSortColumn;
            }
        }

        private string sortdirection
        {
            get
            {
                return sSortDirection;
            }
        }

        private string curpage
        {
            get
            {
                return sCurpage;
            }
            set
            {
                sCurpage = value;
            }
        }

        public Grid grid
        {
            get
            {
                return aGrid;
            }

        }

        public string emptytext
        {
            get
            {
                return sEmptyText;
            }
            set
            {
                sEmptyText = value;
            }
        }

        private string generateTable()
        {
            int i;
            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);
            cCurrency reqcurrency;
            string rownum = "row1";
            int x;
            string val;
            cGridRow clsrow;
            cGridCell reqcell;
            System.Text.StringBuilder output = new System.Text.StringBuilder();

            if (rows.Count == 0 && sEmptyText != "")
            {
                output.Append("<tr>");
                output.Append("<td colspan=\"" + columns.Count + "\" align=\"center\">" + emptytext + "</td>\n");
                output.Append("</tr>\n\n");
            }
            for (i = 0; i < rows.Count; i++)
            {
                clsrow = (cGridRow)rows[i];
                output.Append("<tr ");
                if (idcolumn != null)
                {
                    output.Append("id=\"" + clsrow.getCellByName(idcolumn.name).thevalue + "\"");
                }
                output.Append(">");
                for (x = 0; x < clsrow.rowCells.Count; x++)
                {
                    reqcell = (cGridCell)clsrow.rowCells[x];
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
                            case "FD":
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
                                case "N":
                                case "FD":
                                default:
                                    output.Append(reqcell.thevalue);
                                    break;
                                case "D":
                                    output.Append(DateTime.Parse(val).ToShortDateString());
                                    break;
                                case "T":
                                    output.Append(DateTime.Parse(val).ToString("dd/MM/yyyy HH:mm"));
                                    break;
                                case "X":
                                    output.Append("<input type=\"checkbox\" disabled ");
                                    if (reqcell.thevalue.ToString().ToLower() == "yes" || reqcell.thevalue.ToString().ToLower() == "true")
                                    {
                                        output.Append("checked");
                                    }
                                    output.Append(">");
                                    break;
                                case "C":
                                    switch (reqcell.columninfo.name)
                                    {
                                        case "Global Total":
                                            if (clsrow.getCellByName("globalbasecurrency") != null)
                                            {
                                                reqcurrency = clscurrencies.getCurrencyById((int)clsrow.getCellByName("globalbasecurrency").thevalue);
                                                output.Append(clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol);
                                            }
                                            break;
                                        case "Total Prior To Convert":
                                            if (clsrow.getCellByName("originalcurrency") != null)
                                            {
                                                if (clsrow.getCellByName("originalcurrency").thevalue != DBNull.Value)
                                                {
                                                    reqcurrency = clscurrencies.getCurrencyById((int)clsrow.getCellByName("originalcurrency").thevalue);
                                                    output.Append(clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol);
                                                }
                                            }
                                            break;
                                        default:
                                            if (clsrow.getCellByName("basecurrency") != null)
                                            {
                                                if (clsrow.getCellByName("basecurrency").thevalue != DBNull.Value)
                                                {
                                                    if ((int)clsrow.getCellByName("basecurrency").thevalue != 0)
                                                    {
                                                        reqcurrency = clscurrencies.getCurrencyById((int)clsrow.getCellByName("basecurrency").thevalue);
                                                        output.Append(clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol);
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
                                            break;
                                    }

                                    output.Append(decimal.Parse(val).ToString("#,###,##0.00"));
                                    break;
                                case "M":
                                    output.Append(decimal.Parse(val).ToString("#,###,##0.00"));
                                    break;
                                case "DT":
                                    output.Append(DateTime.Parse(val).ToString("dd/MM/yyyy HH:mm"));
                                    break;
                            }

                        }
                        output.Append("</td>\n");
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
                output.Append("</tr>\n\n");

            }
            return output.ToString();
        }

        public string getFooterValues()
        {
            int i;
            int x;
            decimal total = 0;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            cGridColumn reqcolumn;
            cGridRow reqrow;
            cGridCell reqcell;


            for (i = 0; i < columns.Count; i++)
            {
                reqcolumn = (cGridColumn)columns[i];
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

                            reqrow = (cGridRow)rows[x];
                            reqcell = (cGridCell)reqrow.rowCells[i];
                            total += Decimal.Parse(reqcell.thevalue.ToString());
                        }

                        switch (reqcolumn.fieldtype)
                        {
                            case "C":
                                output.Append(total.ToString("£#,###,##0.00"));
                                break;
                            case "M":
                            case "FD":
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
            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            int i;
            int x;
            decimal total = 0;
            cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);
            cCurrency reqcurrency;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            cGridColumn reqcolumn;
            cGridRow reqrow;
            cGridCell reqcell;
            string symbol = "";
            output.Append("<tr id=footerRow>");
            for (i = 0; i < columns.Count; i++)
            {
                reqcolumn = (cGridColumn)columns[i];
                if (reqcolumn.hidden == false)
                {
                    output.Append("<th ");
                    switch (reqcolumn.fieldtype)
                    {
                        case "C":
                        case "M":
                        case "N":
                        case "FD":
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

                            reqrow = (cGridRow)rows[x];
                            reqcell = (cGridCell)reqrow.rowCells[i];
                            if (reqcell.thevalue != DBNull.Value)
                            {

                                total += Math.Round(Decimal.Parse(reqcell.thevalue.ToString()), 2, MidpointRounding.AwayFromZero);
                            }
                            switch (reqcell.columninfo.name)
                            {
                                case "Global Total":
                                    if (reqrow.getCellByName("globalbasecurrency") != null)
                                    {
                                        if (reqrow.getCellByName("Global Total").thevalue != DBNull.Value)
                                        {
                                            reqcurrency = clscurrencies.getCurrencyById((int)reqrow.getCellByName("globalbasecurrency").thevalue);
                                            symbol = clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol;
                                        }
                                    }
                                    break;
                                case "Total Prior To Convert":
                                    if (reqrow.getCellByName("originalcurrency") != null)
                                    {
                                        if (reqrow.getCellByName("originalcurrency").thevalue != DBNull.Value)
                                        {
                                            reqcurrency = clscurrencies.getCurrencyById((int)reqrow.getCellByName("originalcurrency").thevalue);
                                            symbol = clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol;
                                        }
                                    }
                                    break;
                                default:
                                    if (reqrow.getCellByName("basecurrency") != null)
                                    {
                                        if (reqrow.getCellByName("basecurrency").thevalue != DBNull.Value)
                                        {
                                            reqcurrency = clscurrencies.getCurrencyById((int)reqrow.getCellByName("basecurrency").thevalue);
                                            if (reqcurrency != null)
                                            {
                                                symbol = clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol;
                                            }
                                        }
                                        else
                                        {
                                            symbol = "£";
                                        }
                                    }
                                    break;
                            }

                        }

                        switch (reqcolumn.fieldtype)
                        {
                            case "C":
                                output.Append(symbol);
                                output.Append(total.ToString("#,###,##0.00"));
                                break;
                            case "M":
                            case "FD":
                                output.Append(total.ToString("#,###,##0.00"));
                                break;
                            case "N":
                                //output.Append(int.Parse(total.ToString()));
                                output.Append(total);
                                break;
                        }
                    }
                    output.Append("</th>\n");
                }
            }
            output.Append("</tr>\n\n");

            return output.ToString();

        }

        public void getData()
        {
            int i, x;
            int tblcol = 0;
            System.Collections.ArrayList cells;
            cGridRow clsrow;
            cGridCell clscell;
            cGridColumn curcol;
            rows.Clear();
            for (i = 0; i < tblview.Count; i++)
            {
                tblcol = 0;
                cells = new System.Collections.ArrayList();
                for (x = 0; x < columns.Count; x++)
                {

                    curcol = (cGridColumn)columns[x];
                    if (curcol.customcolumn == false)
                    {
                        clscell = new cGridCell(curcol, tblview[i][tblcol]);
                        tblcol++;
                    }
                    else
                    {
                        clscell = new cGridCell(curcol, curcol.description);
                    }
                    cells.Add(clscell);
                }
                clsrow = new cGridRow(cells);
                rows.Add(clsrow);
            }
        }

        public cGridColumn getColumn(string description)
        {
            int i;
            cGridColumn reqcolumn;
            for (i = 0; i < columns.Count; i++)
            {
                reqcolumn = (cGridColumn)columns[i];
                if (reqcolumn.name.ToUpper() == description.ToUpper())
                {
                    return reqcolumn;
                }
            }

            return null;
        }

        #region properties

        public System.Collections.ArrayList gridcolumns
        {
            get
            {
                return columns;
            }
        }

        public System.Collections.ArrayList gridrows
        {
            get
            {
                return rows;
            }
        }

        public string tableid
        {
            get
            {
                return sTableid;
            }
            set
            {
                sTableid = value;
            }
        }

        public string tbodyid
        {
            get
            {
                return sTbodyid;
            }
            set
            {
                sTbodyid = value;
            }
        }

        public cGridColumn idcolumn
        {
            get
            {
                return idColumn;
            }
            set
            {
                idColumn = value;
            }
        }

        public int accountid
        {
            get
            {
                return nAccountid;
            }
        }

        #endregion
    }

    public class cGridRow
    {
        private System.Collections.ArrayList cells = new System.Collections.ArrayList();

        public cGridRow(System.Collections.ArrayList thecells)
        {
            cells = thecells;
        }

        public System.Collections.ArrayList rowCells
        {
            get
            {
                return cells;
            }
        }

        public cGridCell getCellByName(string name)
        {
            int i;
            cGridCell reqcell;
            for (i = 0; i < cells.Count; i++)
            {
                reqcell = (cGridCell)cells[i];
                if (reqcell.columninfo.name.ToUpper() == name.ToUpper())
                {
                    return reqcell;
                }
            }
            return null;
        }


    }

    public class cGridColumn
    {
        private string sDescription;

        private string sName;

        private string sFieldtype;

        private string sComment;

        private bool bHidden;

        private bool bCustomColumn;

        private bool bCantotal;

        private cColumnList valuelist = new cColumnList();

        private string sWidth;

        private string sAlign;

        private string sTableColumn;

        public cGridColumn(string description, string fieldtype, string comment, bool cantotal, string tablecolumn)
        {
            sName = description;
            sDescription = description;
            sFieldtype = fieldtype;
            sComment = comment;
            bCantotal = cantotal;
            sTableColumn = tablecolumn;
        }

        public cGridColumn(string name, string description, string fieldtype, string comment, bool cantotal, bool custom)
        {
            sName = name;
            sDescription = description;
            sFieldtype = fieldtype;
            sComment = comment;
            bCustomColumn = custom;
        }

        public cGridColumn(string name, string description, string fieldtype, string comment, bool cantotal, bool custom, string tablecolumn)
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
            get
            {
                return sDescription;
            }
            set
            {
                sDescription = value;
            }
        }

        public string name
        {
            get
            {
                return sName;
            }
        }

        public string fieldtype
        {
            get
            {
                return sFieldtype;
            }
            set
            {
                sFieldtype = value;
            }
        }

        public string comment
        {
            get
            {
                return sComment;
            }
        }

        public bool hidden
        {
            get
            {
                return bHidden;
            }
            set
            {
                bHidden = value;
            }
        }

        public bool customcolumn
        {
            get
            {
                return bCustomColumn;
            }
            set
            {
                bCustomColumn = value;
            }
        }

        public bool cantotal
        {
            get
            {
                return bCantotal;
            }
        }

        public cColumnList listitems
        {
            get
            {
                return valuelist;
            }
            set
            {
                valuelist = value;
            }
        }

        public string width
        {
            get
            {
                return sWidth;
            }
            set
            {
                sWidth = value;
            }
        }

        public string align
        {
            get
            {
                return sAlign;
            }
            set
            {
                sAlign = value;
            }
        }

        public string tablecolumn
        {
            get
            {
                return sTableColumn;
            }
        }
    }

    public class cGridCell
    {
        private cGridColumn column;

        private object cellvalue;

        private string sBgcolor;

        public cGridCell(cGridColumn theColumn, object thevalue)
        {
            column = theColumn;
            cellvalue = thevalue;
        }

        public cGridColumn columninfo
        {
            get
            {
                return column;
            }
        }

        public object thevalue
        {
            get
            {
                return cellvalue;
            }
            set
            {
                cellvalue = value;
            }
        }

        public string bgcolor
        {
            get
            {
                return sBgcolor;
            }
            set
            {
                sBgcolor = value;
            }
        }
    }

    public class cColumnList
    {
        private System.Collections.Hashtable items = new System.Collections.Hashtable();

        public void addItem(object key, string val)
        {
            items.Add(key, val);
        }

        public int count
        {
            get
            {
                return items.Count;
            }
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
}