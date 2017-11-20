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
/// Summary description for cRangeCurrencies
/// </summary>
public class cRangeCurrencies : cCurrencies
{
	public cRangeCurrencies(int nAccountid)
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

        currency = new cRangeCurrency(accountid, currencyid, globalcurrencyid, createdon, userid, new SortedList());

        list.Add(currencyid, currency);
        clsaudit.addRecord("Currency", clsglobalcurrencies.getGlobalCurrencyById(globalcurrencyid).label);
        return currencyid;
    }
    public override string getGrid()
    {
        cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
        cRangeCurrency reqcur;

        int i;
        string rowclass = "row1";

        System.Text.StringBuilder output = new System.Text.StringBuilder();
        output.Append("<table class=datatbl id=currencies>");
        output.Append("<tr><th>&nbsp;</th><th><img alt=\"Edit\" src=\"../icons/edit_blue.gif\"></th><th><img alt=\"Delete\" src=\"../icons/delete2_blue.gif\"></th><th>Currency</th><th>Alpha Code</th></tr>");
        for (i = 0; i < list.Count; i++)
        {
            reqcur = (cRangeCurrency)list.GetByIndex(i);
            
            
            output.Append("<tr id=\"" + reqcur.currencyid + "\">");
            output.Append("<td>");
            if (reqcur.exchangeratecount != 0)
            {
                output.Append("<img src=\"../buttons/open.gif\" onclick=\"toggle('" + i + "');\" id=\"img" + i + "\">");
            }
            output.Append("</td>");
            output.Append("<td class=\"" + rowclass + "\"><a href=\"aecurrencyranges.aspx?action=2&currencyid=" + reqcur.currencyid + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a></td>");
            output.Append("<td class=\"" + rowclass + "\"><a href=\"javascript:deleteCurrency(" + reqcur.currencyid + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a></td>");
            output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label + "</td>");
            output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).alphacode+ "</td>");

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

    public int addCurrencyRange(int currencyid, DateTime startdate, DateTime enddate, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        int currencyrangeid;
        DateTime createdon = DateTime.Now.ToUniversalTime();
        cAuditLog clsaudit = new cAuditLog(accountid, userid);
        strsql = "insert into currencyranges (currencyid, startdate, enddate, createdon, createdby) " +
            "values (@currencyid, @startdate, @enddate, @createdon, @createdby);select @identity = @@identity";
        expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
        expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
        expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
        expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
        expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
        expdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate.Year + "/" + startdate.Month + "/" + startdate.Day);
        expdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate.Year + "/" + enddate.Month + "/" + enddate.Day);        
        expdata.ExecuteSQL(strsql);
        currencyrangeid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
        expdata.sqlexecute.Parameters.Clear();

        cRangeCurrency clscurrency = getCurrencyById(currencyid);
        cCurrencyRange newrange = new cCurrencyRange(accountid, currencyid, currencyrangeid, startdate, enddate,createdon,userid,new DateTime(1900,01,01),0, new SortedList());
        clscurrency.addCurrencyRange(newrange);

        clsaudit.addRecord("Currency Range", startdate.ToShortDateString() + " - " + enddate.ToShortDateString());
        return currencyrangeid;
    }

    public byte updateCurrencyRange(int currencyid, int currencyrangeid, DateTime startdate, DateTime enddate, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        cAuditLog clsaudit = new cAuditLog(accountid, userid);
        DateTime modifiedon = DateTime.Now.ToUniversalTime();
        
        cRangeCurrency reqcur = getCurrencyById(currencyid);

        cCurrencyRange reqrange = reqcur.getCurrencyRangeById(currencyrangeid);


        strsql = "update currencyranges set startdate = @startdate, enddate = @enddate, modifiedon = @modifiedon, modifiedby = @modifiedby where currencyrangeid = @currencyrangeid";

        expdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate.Year + "/" + startdate.Month + "/" + startdate.Day);
        expdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate.Year + "/" + enddate.Month + "/" + enddate.Day);
        expdata.sqlexecute.Parameters.AddWithValue("@currencyrangeid", currencyrangeid);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        reqcur.deleteCurrencyRange(currencyrangeid);
        cCurrencyRange newrange = new cCurrencyRange(accountid, currencyid, currencyrangeid, startdate, enddate, reqcur.createdon, reqcur.createdby,modifiedon,userid, reqrange.exchangerates);
        reqcur.addCurrencyRange(newrange);
        #region auditlog
        if (startdate != reqrange.startdate)
        {
            clsaudit.editRecord(startdate.ToShortDateString() + " - " + enddate.ToShortDateString(), "Start Date", "Currency Range", reqrange.startdate.ToShortDateString(), startdate.ToShortDateString());
        }
        if (enddate != reqrange.enddate)
        {
            clsaudit.editRecord(startdate.ToShortDateString() + " - " + enddate.ToShortDateString(), "End Date", "Currency Range", reqrange.enddate.ToShortDateString(), enddate.ToShortDateString());
        }
        
        #endregion
        return 0;
    }

    public void deleteCurrencyRange(int currencyid, int currencyrangeid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        cAuditLog clsaudit = new cAuditLog();
        cRangeCurrency reqcur = getCurrencyById(currencyid);

        cCurrencyRange reqrange = reqcur.getCurrencyRangeById(currencyrangeid);

        deleteRangeExchangeRates(currencyrangeid);

        strsql = "delete from currencyranges where currencyrangeid = @currencyrangeid";
        expdata.sqlexecute.Parameters.AddWithValue("@currencyrangeid", currencyrangeid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        reqcur.deleteCurrencyRange(currencyrangeid);

        clsaudit.deleteRecord("Currency Range", reqrange.startdate.ToShortDateString() + " - " + reqrange.enddate.ToShortDateString());

       
    }

    public new cRangeCurrency getCurrencyById(int currencyid)
    {
        return (cRangeCurrency)list[currencyid];
    }


    #region exchange rates
    
    
    public void deleteRangeExchangeRates(int currencyrangeid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "delete from range_exchangerates where currencyrangeid = @currencyrangeid";
        expdata.sqlexecute.Parameters.AddWithValue("@currencyrangeid", currencyrangeid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
    }


    public void addRangeExchangeRates(int currencyid, int currencyrangeid, SortedList list)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));


        for (int i = 0; i < list.Count; i++)
        {

            strsql = "insert into range_exchangerates (currencyrangeid, tocurrencyid, exchangerate) " +
                "values (@currencyrangeid, @tocurrencyid, @exchangerate)";
            expdata.sqlexecute.Parameters.AddWithValue("@currencyrangeid", currencyrangeid);
            expdata.sqlexecute.Parameters.AddWithValue("@tocurrencyid", (int)list.GetKey(i));
            expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", (double)list.GetByIndex(i));
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        cRangeCurrency clsrange = getCurrencyById(currencyid);
        cCurrencyRange currentrange = clsrange.getCurrencyRangeById(currencyrangeid);
        currentrange.exchangerates = getExchangeRatesForRange(currencyrangeid);

    }
    #endregion

    public string getGrid(int currencyid, bool buttons)
    {
        cGrid clsgrid;
        int i;
        System.Data.DataSet ds = new System.Data.DataSet();
        System.Data.DataTable tbl = new System.Data.DataTable();
        object[] values;
        cRangeCurrency currency = getCurrencyById(currencyid);

        cCurrencyRange reqrange;
        tbl.Columns.Add("currencyrangeid", System.Type.GetType("System.Int32"));
        tbl.Columns.Add("startdate", System.Type.GetType("System.DateTime"));
        tbl.Columns.Add("enddate", System.Type.GetType("System.DateTime"));

        tbl.Columns.Add("currencyid", System.Type.GetType("System.Int32"));

        for (i = 0; i < currency.exchangerates.Count; i++)
        {
            reqrange = (cCurrencyRange)currency.exchangerates.GetByIndex(i);
            values = new object[4];
            values[0] = reqrange.currencyrangeid;
            values[1] = reqrange.startdate;
            values[2] = reqrange.enddate;
            values[3] = reqrange.currencyid;
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
        clsgrid.getColumn("currencyrangeid").hidden = true;
        clsgrid.getColumn("currencyid").hidden = true;
        clsgrid.getColumn("startdate").description = "Start Date";
        clsgrid.getColumn("enddate").description = "End Date";
        clsgrid.getColumn("startdate").fieldtype = "D";
        clsgrid.getColumn("enddate").fieldtype = "D";
        clsgrid.getData();
        if (buttons == true)
        {
            cGridRow reqrow;
            for (i = 0; i < clsgrid.gridrows.Count; i++)
            {
                reqrow = (cGridRow)clsgrid.gridrows[i];
                reqrow.getCellByName("Edit").thevalue = "<a href=\"aecurrencyrange.aspx?action=2&currencyrangeid=" + reqrow.getCellByName("currencyrangeid").thevalue + "&currencyid=" + reqrow.getCellByName("currencyid").thevalue + "\"><img src=\"../icons/edit.gif\" alt=\"Edit\"></a>";
                reqrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteRange(" + reqrow.getCellByName("currencyrangeid").thevalue + "," + reqrow.getCellByName("currencyid").thevalue + ");\"><img src=\"../icons/delete2.gif\" alt=\"Delete\"></a>";
            }
        }
        return clsgrid.CreateGrid();
    }

    public string CreateUpdateTable(int currencyid, int currencyrangeid)
    {
        cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Data.DataSet ds;
        string strsql;

        System.Text.StringBuilder output = new System.Text.StringBuilder();
        strsql = "select tocurrencyid, currencies.globalcurrencyid, null as label, range_exchangerates.exchangerate from range_exchangerates inner join currencies on range_exchangerates.tocurrencyid = currencies.currencyid where currencyrangeid = @currencyrangeid " +
            "union " +
            "select currencyid as tocurrencyid, currencies.globalcurrencyid, null as label, null as exchangerate from currencies where currencyid <> @currencyid and currencyid not in (select tocurrencyid from range_exchangerates where currencyrangeid = @currencyrangeid)";
        
        expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
        expdata.sqlexecute.Parameters.AddWithValue("@currencyrangeid", currencyrangeid);
        ds = expdata.GetDataSet(strsql);
        cGrid clsgrid = new cGrid(ds, true, false, Grid.RangeCurrencies);
        clsgrid.tblclass = "datatbl";
        clsgrid.getColumn("tocurrencyid").hidden = true;
        clsgrid.getColumn("label").description = "Currency";
        clsgrid.getColumn("exchangerate").description = "Exchange Rate";
        clsgrid.getColumn("globalcurrencyid").hidden = true;
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

    public Dictionary<int, cCurrencyRange> getModifiedCurrencyRanges(DateTime date)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        Dictionary<int, cCurrencyRange> lstCurRanges = new Dictionary<int, cCurrencyRange>();
        System.Data.SqlClient.SqlDataReader reader;
        int currencyid, currencyrangeid;

        strsql = "SELECT * FROM currencyranges WHERE createdon > @date OR modifiedon > @date;";
        expdata.sqlexecute.Parameters.AddWithValue("@date", date);
        reader = expdata.GetReader(strsql);

        while (reader.Read())
        {
            currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
            currencyrangeid = reader.GetInt32(reader.GetOrdinal("currencyrangeid"));

            if (list.Contains(currencyid))
            {
                cRangeCurrency reqcur = getCurrencyById(currencyid);
                cCurrencyRange reqrange = reqcur.getCurrencyRangeById(currencyrangeid);
                lstCurRanges.Add(currencyrangeid, reqrange);
            }
        }
        reader.Close();
        expdata.sqlexecute.Parameters.Clear();

        return lstCurRanges;
    }

    public SortedList<int, int> getCurrencyRangeIds()
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Data.SqlClient.SqlDataReader reader;
        SortedList<int, int> lstids = new SortedList<int, int>();

        strsql = "SELECT currencyrangeid, currencyid FROM currencyranges";
        reader = expdata.GetReader(strsql);

        while (reader.Read())
        {
            lstids.Add(reader.GetInt32(reader.GetOrdinal("currencyrangeid")), reader.GetInt32(reader.GetOrdinal("currencyid")));
        }
        reader.Close();
        return lstids;
    }
}

