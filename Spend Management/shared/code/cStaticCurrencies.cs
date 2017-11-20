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
using System.Web.Caching;
using System.Collections.Generic;
using SpendManagementLibrary;
using Spend_Management;

/// <summary>
/// Summary description for cStaticCurrencies
/// </summary>
public class cStaticCurrencies : cCurrencies
{
    public SortedList<int, double> lstRates;

    /// <summary>
    /// Constructor for use if sub Accounts are being used
    /// </summary>
    /// <param name="AccountID"></param>
    /// <param name="subAccountID"></param>
    public cStaticCurrencies(int AccountID, int? subAccountID) : base (AccountID, subAccountID)
	{
        InitialiseData();
	}

    public cStaticCurrencies(int AccountID, SortedList<int, Double> rates)
    {
        lstRates = rates;
        nAccountID = AccountID;

        InitialiseData();
    }

    #region properties
    public SortedList<int, double> rates
    {
        get { return lstRates; }
    }
    
    #endregion

    //public string getGrid()
    //{
    //    return 
    //    //cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
    //    //cStaticCurrency reqcur;
    //    //int i;
    //    //string rowclass = "row1";

    //    //System.Text.StringBuilder output = new System.Text.StringBuilder();
    //    //output.Append("<table class=datatbl id=currencies>");
    //    //output.Append("<tr><th><img alt=\"Edit\" src=\"../icons/edit_blue.gif\"></th><th><img alt=\"Delete\" src=\"../icons/delete2_blue.gif\"></th><th>Currency</th><th>Alpha Code</th><th>Numeric Code</th><th>Symbol</th></tr>");
    //    //for (i = 0; i < list.Count; i++)
    //    //{
    //    //    reqcur = (cStaticCurrency)list.GetByIndex(i);
            
    //    //    output.Append("<tr id=\"" + reqcur.currencyid + "\">");
    //    //    output.Append("<td class=\"" + rowclass + "\"><a href=\"aecurrencystatic.aspx?action=2&currencyid=" + reqcur.currencyid + "\"><img alt=\"Edit\" src=\"../icons/edit.gif\"></a></td>");
    //    //    output.Append("<td class=\"" + rowclass + "\"><a href=\"javascript:deleteCurrency(" + reqcur.currencyid + ");\"><img alt=\"Delete\" src=\"../icons/delete2.gif\"></a></td>");
    //    //    output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label + "</td>");
    //    //    output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).alphacode + "</td>");
    //    //    output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).numericcode + "</td>");
    //    //    output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).symbol + "</td>");
    //    //    output.Append("</tr>");
    //    //    if (rowclass == "row1")
    //    //    {
    //    //        rowclass = "row2";
    //    //    }
    //    //    else
    //    //    {
    //    //        rowclass = "row1";
    //    //    }
    //    //}
    //    //output.Append("</table>");
    //    return output.ToString();

    //}


    //public new string CreateUpdateTable(int currencyid)
    //{
    //    cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
    //    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
    //    System.Data.DataSet ds;
    //    string strsql;

    //    System.Text.StringBuilder output = new System.Text.StringBuilder();
    //    strsql = "select tocurrencyid, currencies.globalcurrencyid, null as label, static_exchangerates.exchangerate from static_exchangerates inner join currencies on static_exchangerates.tocurrencyid = currencies.currencyid where static_exchangerates.currencyid = @currencyid " +
    //        "union " +
    //        "select currencyid as tocurrencyid, currencies.globalcurrencyid, null as label, null as exchangerate from currencies where currencyid <> @currencyid and currencyid not in (select tocurrencyid from static_exchangerates where currencyid = @currencyid)";
        
    //    expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
    //    ds = expdata.GetDataSet(strsql);
    //    cGrid clsgrid = new cGrid(ds, true, false, Grid.StaticCurrencyBreakdown);
    //    clsgrid.tblclass = "datatbl";
    //    clsgrid.getColumn("tocurrencyid").hidden = true;
    //    clsgrid.getColumn("label").description = "Currency";
    //    clsgrid.getColumn("exchangerate").description = "Exchange Rate";
    //    clsgrid.getColumn("globalcurrencyid").hidden = true;
    //    clsgrid.getData();

    //    string exchangerate;
    //    for (int i = 0; i < clsgrid.gridrows.Count; i++)
    //    {
    //        cGridRow row = (cGridRow)clsgrid.gridrows[i];
    //        row.getCellByName("label").thevalue = clsglobalcurrencies.getGlobalCurrencyById((int)row.getCellByName("globalcurrencyid").thevalue).label;
    //        exchangerate = "<input type=text onblur=\"validateItem('exchangerate" + row.getCellByName("tocurrencyid").thevalue + "',1,'Exchange Rate');\" size=5 name=\"exchangerate" + row.getCellByName("tocurrencyid").thevalue + "\" id=\"exchangerate" + row.getCellByName("tocurrencyid").thevalue + "\"";
    //        if (row.getCellByName("exchangerate").thevalue != DBNull.Value)
    //        {
    //            exchangerate += " value=\"" + row.getCellByName("exchangerate").thevalue + "\"";
    //        }
    //        exchangerate += ">";
    //        row.getCellByName("exchangerate").thevalue = exchangerate;
    //    }
    //    return clsgrid.CreateGrid();
    //}

    /// <summary>
    /// Get the modified static currencies for expensesConnect
    /// </summary>
    /// <param name="date">Date of the last expensesConnect data synchronization</param>
    /// <returns>Collection of modified static currencies</returns>
    public Dictionary<string, double> getModifiedStaticCurrencies(DateTime date)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
        System.Data.SqlClient.SqlDataReader reader;
        Dictionary<string, double> lstStaticCur = new Dictionary<string, double>();

        string curKey;
        int currencyid, tocurrencyid;
        double exchangerate;

        strsql = "SELECT * FROM static_exchangerates WHERE createdon > @date";
        expdata.sqlexecute.Parameters.AddWithValue("@date", date);
        using (reader = expdata.GetReader(strsql))
        {
            while (reader.Read())
            {
                currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                tocurrencyid = reader.GetInt32(reader.GetOrdinal("tocurrencyid"));
                exchangerate = reader.GetDouble(reader.GetOrdinal("exchangerate"));

                curKey = currencyid.ToString() + "," + tocurrencyid.ToString();
                lstStaticCur.Add(curKey, exchangerate);
            }
            reader.Close();
        }
        expdata.sqlexecute.Parameters.Clear();

        return lstStaticCur;
    }

}


