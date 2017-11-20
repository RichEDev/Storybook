using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using expenses;
using System.Data.SqlClient;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using System.Collections.Generic;
using SpendManagementLibrary;
using Spend_Management;

/// <summary>
/// Summary description for cMonthlyCurrencies
/// </summary>
public class cMonthlyCurrencies : cCurrencies
{
    public cMonthlyCurrencies(int nAccountid)
	{
        accountid = nAccountid;
        
        InitialiseData();
	}

    public int addCurrency(int globalcurrencyid, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
        cAuditLog clsaudit = new cAuditLog(accountid, userid);
        //return codes
        //0 = currency added successfully
        //1 = currency already exists

        int currencyid;
        DateTime createdon = DateTime.Now.ToUniversalTime();


        
        expdata.sqlexecute.Parameters.AddWithValue("@globalcurrencyid", globalcurrencyid);
        expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
        expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
        expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
        expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
        strsql = "insert into currencies (globalcurrencyid, createdon, createdby) values (@globalcurrencyid, @createdon, @createdby);select @identity = @@identity";
        expdata.ExecuteSQL(strsql);

        currencyid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
        expdata.sqlexecute.Parameters.Clear();



        cCurrency currency = null;

        currency = new cMonthlyCurrency(accountid, currencyid, globalcurrencyid, createdon, userid, new SortedList());



        list.Add(currencyid, currency);
        clsaudit.addRecord("Currency", clsglobalcurrencies.getGlobalCurrencyById(globalcurrencyid).label);
        return currencyid;
    }

    public override string getGrid()
    {
        cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
        cMonthlyCurrency reqcur;
        int i;
        string rowclass = "row1";

        System.Text.StringBuilder output = new System.Text.StringBuilder();
        output.Append("<table class=datatbl id=currencies>");
        output.Append("<tr><th>&nbsp;</th><th><img alt=\"Edit\" src=\"../icons/edit_blue.gif\"></th><th><img alt=\"Delete\" src=\"../icons/delete2_blue.gif\"></th><th>Currency</th><th>Alpha Code</th></tr>");
        for (i = 0; i < list.Count; i++)
        {
            reqcur = (cMonthlyCurrency)list.GetByIndex(i);
            
            output.Append("<tr id=\"" + reqcur.currencyid + "\">");
            output.Append("<td>");
            if (reqcur.exchangeratecount != 0)
            {
                output.Append("<img src=\"../buttons/open.gif\" onclick=\"toggle('" + i + "');\" id=\"img" + i + "\">");
            }
            output.Append("</td>");
            output.Append("<td class=\"" + rowclass + "\"><a href=\"aecurrencymonthly.aspx?action=2&currencyid=" + reqcur.currencyid + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a></td>");
            output.Append("<td class=\"" + rowclass + "\"><a href=\"javascript:deleteCurrency(" + reqcur.currencyid + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a></td>");
            output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label + "</td>");
            output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).alphacode + "</td>");

            output.Append("</tr>");
            if (reqcur.exchangeratecount != 0)
            {

                output.Append("<tr style=\"display:none;\" id=\"" + i + "\">");
                output.Append("<td><td>");
                output.Append("<td colspan=4>");
                output.Append(getGrid(reqcur.currencyid,false));
                output.Append("</td>");
                output.Append("</tr>");

            }
            if (rowclass == "row1")
            {
                rowclass = "row2";
            }
            else
            {
                rowclass = "row1";
            }
        }
        output.Append("</table>");
        return output.ToString();
    }

    public int addCurrencyMonth(int currencyid, int year, int month, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        cAuditLog clsaudit = new cAuditLog(accountid, userid);
        DateTime createdon = DateTime.Now.ToUniversalTime();
        int currencymonthid;
        expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
        expdata.sqlexecute.Parameters.AddWithValue("@year", year);
        expdata.sqlexecute.Parameters.AddWithValue("@month", month);
        expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
        expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
        expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
        expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
        strsql = "insert into currencymonths (currencyid, [year], [month], createdon, createdby) values (@currencyid,@year,@month, @createdon, @createdby); select @identity =@@identity";

        expdata.ExecuteSQL(strsql);
        currencymonthid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
        expdata.sqlexecute.Parameters.Clear();

        cMonthlyCurrency clscurrency = getCurrencyById(currencyid);
        cCurrencyMonth newmonth = new cCurrencyMonth(accountid, currencyid, currencymonthid, month, year, createdon, userid, new DateTime(1900,01,01), 0, new SortedList());
        clscurrency.addCurrencyMonth(newmonth);

        clsaudit.addRecord("Currency Month", month + "/" + year);
        return currencymonthid;
    }

    public int updateCurrencyMonth(int currencyid, int currencymonthid, int year, int month, int userid)
    {
        cAuditLog clsaudit = new cAuditLog(accountid, userid);
        DateTime modifiedon = DateTime.Now.ToUniversalTime();
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        cMonthlyCurrency reqcur = getCurrencyById(currencyid);

        cCurrencyMonth reqmonth = reqcur.getCurrentMonthById(currencymonthid);
        expdata.sqlexecute.Parameters.AddWithValue("@year", year);
        expdata.sqlexecute.Parameters.AddWithValue("@month", month);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
        expdata.sqlexecute.Parameters.AddWithValue("@currencymonthid", currencymonthid);
        strsql = "update currencymonths set [year] = @year, [month] = @month, modifiedon = @modifiedon, modifiedby = @modifiedby where currencymonthid = @currencymonthid";

        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        reqcur.deleteCurrencyMonth(currencymonthid);
        cCurrencyMonth newmonth = new cCurrencyMonth(accountid, currencyid, currencymonthid, month, year, new DateTime(1900, 01, 01), 0, modifiedon, userid, reqmonth.exchangerates);
        reqcur.addCurrencyMonth(newmonth);

        #region audit log
        if (reqmonth.year != year)
        {
            clsaudit.editRecord(month + "/" + year, "Year", "Currency Month", reqmonth.year.ToString(), year.ToString());
        }
        if (reqmonth.month != month)
        {
            clsaudit.editRecord(month + "/" + year, "Month", "Currency Month", reqmonth.month.ToString(), month.ToString());
        }
        

        #endregion
        return currencymonthid;
    }

    public void deleteCurrencyMonth(int currencyid, int currencymonthid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        cAuditLog clsaudit = new cAuditLog();
        cMonthlyCurrency reqcur = getCurrencyById(currencyid);

        deleteMonthlyExchangeRates(currencymonthid);
        cCurrencyMonth reqmonth = reqcur.getCurrentMonthById(currencymonthid);
        expdata.sqlexecute.Parameters.AddWithValue("@currencymonthid", currencymonthid);
        strsql = "delete from currencymonths where currencymonthid = @currencymonthid";

        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
        reqcur.deleteCurrencyMonth(currencymonthid);

        clsaudit.deleteRecord("Currency Month", reqmonth.month + "/" + reqmonth.year);
    }

    public new cMonthlyCurrency getCurrencyById(int currencyid)
    {
        return (cMonthlyCurrency)list[currencyid];
    }

    #region exchange rates
    
    
    public void deleteMonthlyExchangeRates(int currencymonthid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "delete from monthly_exchangerates where currencymonthid = @currencymonthid";
        expdata.sqlexecute.Parameters.AddWithValue("@currencymonthid", currencymonthid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
    }


    public void addMonthlyExchangeRates(int currencyid, int currencymonthid, SortedList list)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));


        for (int i = 0; i < list.Count; i++)
        {

            strsql = "insert into monthly_exchangerates (currencymonthid, tocurrencyid, exchangerate) " +
                "values (@currencymonthid, @tocurrencyid, @exchangerate)";
            expdata.sqlexecute.Parameters.AddWithValue("@currencymonthid", currencymonthid);
            expdata.sqlexecute.Parameters.AddWithValue("@tocurrencyid", (int)list.GetKey(i));
            expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", (double)list.GetByIndex(i));
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        cMonthlyCurrency clsmonth = getCurrencyById(currencyid);
        cCurrencyMonth currentmonth = clsmonth.getCurrentMonthById(currencymonthid);
        currentmonth.exchangerates = getExchangeRatesForMonth(currencymonthid);
       
    }
    #endregion

    public string getGrid(int currencyid, bool buttons)
    {
        cGrid clsgrid;
        int i;
        System.Data.DataSet ds = new System.Data.DataSet();
        System.Data.DataTable tbl = new System.Data.DataTable();
        object[] values;
        cCurrencyMonth reqmonth;

        cMonthlyCurrency currency = getCurrencyById(currencyid);
        tbl.Columns.Add("currencymonthid", System.Type.GetType("System.Int32"));
        tbl.Columns.Add("month", System.Type.GetType("System.Byte"));
        tbl.Columns.Add("year", System.Type.GetType("System.Int32"));
        tbl.Columns.Add("currencyid", System.Type.GetType("System.Int32"));

        for (i = 0; i < currency.exchangerates.Count; i++)
        {
            reqmonth = (cCurrencyMonth)currency.exchangerates.GetByIndex(i);
            values = new object[4];
            values[0] = reqmonth.currencymonthid;
            values[1] = reqmonth.month;
            values[2] = reqmonth.year;
            values[3] = reqmonth.currencyid;
            tbl.Rows.Add(values);
        }
        ds.Tables.Add(tbl);

        clsgrid = new cGrid(ds, true, false);
        if (buttons == true)
        {
            cGridColumn newcol;
            newcol = new cGridColumn("Delete", "<img alt=\"Delete\" src=\"../icons/delete2_blue.gif\">", "S", "", false, true);
            clsgrid.gridcolumns.Insert(0, newcol);
            newcol = new cGridColumn("Edit", "<img alt=\"Edit\" src=\"../icons/edit_blue.gif\">", "S", "", false, true);
            clsgrid.gridcolumns.Insert(0, newcol);
        }
        clsgrid.tblclass = "datatbl";
        clsgrid.getColumn("currencymonthid").hidden = true;
        clsgrid.getColumn("currencyid").hidden = true;
        clsgrid.getColumn("month").description = "Month";
        clsgrid.getColumn("month").listitems.addItem((byte)1, "January");
        clsgrid.getColumn("month").listitems.addItem((byte)2, "February");
        clsgrid.getColumn("month").listitems.addItem((byte)3, "March");
        clsgrid.getColumn("month").listitems.addItem((byte)4, "April");
        clsgrid.getColumn("month").listitems.addItem((byte)5, "May");
        clsgrid.getColumn("month").listitems.addItem((byte)6, "June");
        clsgrid.getColumn("month").listitems.addItem((byte)7, "July");
        clsgrid.getColumn("month").listitems.addItem((byte)8, "August");
        clsgrid.getColumn("month").listitems.addItem((byte)9, "September");
        clsgrid.getColumn("month").listitems.addItem((byte)10, "October");
        clsgrid.getColumn("month").listitems.addItem((byte)11, "November");
        clsgrid.getColumn("month").listitems.addItem((byte)12, "December");
        clsgrid.getColumn("year").description = "Year";

        clsgrid.getData();
        if (buttons == true)
        {
            cGridRow reqrow;
            for (i = 0; i < clsgrid.gridrows.Count; i++)
            {
                reqrow = (cGridRow)clsgrid.gridrows[i];
                reqrow.getCellByName("Edit").thevalue = "<a href=\"aecurrencymonth.aspx?action=2&currencymonthid=" + reqrow.getCellByName("currencymonthid").thevalue + "&currencyid=" + reqrow.getCellByName("currencyid").thevalue + "\"><img src=\"../icons/edit.gif\" alt=\"Edit\"></a>";
                reqrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteMonth(" + reqrow.getCellByName("currencymonthid").thevalue + "," + reqrow.getCellByName("currencyid").thevalue + ");\"><img src=\"../icons/delete2.gif\" alt=\"Delete\"></a>";
            }
        }
        return clsgrid.CreateGrid();
    }

    public string CreateUpdateTable(int currencyid, int currencymonthid)
    {

        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Data.DataSet ds;
        string strsql;
        cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
        System.Text.StringBuilder output = new System.Text.StringBuilder();
        strsql = "select tocurrencyid, currencies.globalcurrencyid, null as label, monthly_exchangerates.exchangerate from monthly_exchangerates inner join currencies on monthly_exchangerates.tocurrencyid = currencies.currencyid where currencymonthid = @currencymonthid " +
            "union " +
            "select currencyid as tocurrencyid, currencies.globalcurrencyid, null as label, null as exchangerate from currencies where currencyid <> @currencyid and currencyid not in (select tocurrencyid from monthly_exchangerates where currencymonthid = @currencymonthid)";
        
        expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
        expdata.sqlexecute.Parameters.AddWithValue("@currencymonthid", currencymonthid);
        ds = expdata.GetDataSet(strsql);
        cGrid clsgrid = new cGrid(ds, true, false, Grid.MonthlyCurrencyBreakdown);
        clsgrid.tblclass = "datatbl";
        clsgrid.getColumn("tocurrencyid").hidden = true;
        clsgrid.getColumn("globalcurrencyid").hidden = true;
        clsgrid.getColumn("label").description = "Currency";
        clsgrid.getColumn("exchangerate").description = "Exchange Rate";
        clsgrid.getData();

        string exchangerate;
        for (int i = 0; i < clsgrid.gridrows.Count; i++)
        {
            cGridRow row = (cGridRow)clsgrid.gridrows[i];
            row.getCellByName("label").thevalue = clsglobalcurrencies.getGlobalCurrencyById((int)row.getCellByName("globalcurrencyid").thevalue).label;
            exchangerate = "<input type=text onblur=\"validateItem('exchangerate" + row.getCellByName("tocurrencyid").thevalue + "',1,'Exchange Rate');\" size=5 name=\"exchangerate" + row.getCellByName("tocurrencyid").thevalue + "\" id=\"exchangerate" + row.getCellByName("tocurrencyid").thevalue + "\"";
            if (row.getCellByName("exchangerate").thevalue != DBNull.Value)
            {
                exchangerate += " value=\"" + row.getCellByName("exchangerate").thevalue + "\"";
            }
            exchangerate += ">";
            row.getCellByName("exchangerate").thevalue = exchangerate;
        }
        return clsgrid.CreateGrid();
    }

    public Dictionary<int, cCurrencyMonth> getModifiedCurrencyMonths(DateTime date)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        Dictionary<int, cCurrencyMonth> lstCurMonths = new Dictionary<int, cCurrencyMonth>();
        System.Data.SqlClient.SqlDataReader reader;
        int currencyid, currencymonthid;

        strsql = "SELECT * FROM currencymonths WHERE createdon > @date OR modifiedon > @date;";
        expdata.sqlexecute.Parameters.AddWithValue("@date", date);
        reader = expdata.GetReader(strsql);

        while (reader.Read())
        {
            currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
            currencymonthid = reader.GetInt32(reader.GetOrdinal("currencymonthid"));

            if (list.Contains(currencyid))
            {
                cMonthlyCurrency reqcur = getCurrencyById(currencyid);
                cCurrencyMonth reqmonth = reqcur.getCurrentMonthById(currencymonthid);
                lstCurMonths.Add(currencymonthid, reqmonth);
            }
        }
        reader.Close();
        expdata.sqlexecute.Parameters.Clear();

        return lstCurMonths;
    }

    public SortedList<int, int> getCurrencyMonthIds()
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Data.SqlClient.SqlDataReader reader;
        SortedList<int, int> lstids = new SortedList<int, int>();

        strsql = "SELECT currencymonthid, currencyid FROM currencymonths";
        reader = expdata.GetReader(strsql);

        while (reader.Read())
        {
            lstids.Add(reader.GetInt32(reader.GetOrdinal("currencymonthid")), reader.GetInt32(reader.GetOrdinal("currencyid")));
        }
        reader.Close();
        return lstids;
    }
}


