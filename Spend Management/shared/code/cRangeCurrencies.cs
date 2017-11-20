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
using System.Web.Caching;
using System.Collections.Generic;
using SpendManagementLibrary;
using Spend_Management;

/// <summary>
/// Summary description for cRangeCurrencies
/// </summary>
public class cRangeCurrencies : cCurrencies
{
    /// <summary>
    /// Constructor for use if sub Accounts are being used
    /// </summary>
    /// <param name="nAccountid"></param>
    /// <param name="subAccountID"></param>
    public cRangeCurrencies(int nAccountid, int? subAccountID) : base (nAccountid, subAccountID)
    {
        InitialiseData();
    }

    /// <summary>
    /// Get SQL string of the columns used for the range currency grid
    /// </summary>
    /// <returns>SQL string</returns>
    public string getRangeGrid()
    {
        return "SELECT currencyranges.currencyrangeid, currencyranges.startdate, currencyranges.enddate FROM currencyranges";
    }

    /// <summary>
    /// Save the currency date range to the database
    /// </summary>
    /// <param name="range">Currency date range</param>
    /// <returns>ID of the saved currency date range</returns>
    public int saveCurrencyRange(cCurrencyRange range)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
        int currencyrangeid;
        bool update = false;

        expdata.sqlexecute.Parameters.AddWithValue("@currencyid", range.currencyid);
        expdata.sqlexecute.Parameters.AddWithValue("@currencyrangeid", range.currencyrangeid);
        expdata.sqlexecute.Parameters.AddWithValue("@startdate", range.startdate.Year + "/" + range.startdate.Month + "/" + range.startdate.Day);
        expdata.sqlexecute.Parameters.AddWithValue("@enddate", range.enddate.Year + "/" + range.enddate.Month + "/" + range.enddate.Day);

        if (range.currencyrangeid > 0)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@date", range.createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", range.createdby);
            update = true;
        }
        else
        {
            expdata.sqlexecute.Parameters.AddWithValue("@date", range.modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", range.modifiedby);
        }

        CurrentUser currentUser = cMisc.GetCurrentUser();

        if (currentUser != null)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
        }
        else
        {
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
        }
        expdata.sqlexecute.Parameters.AddWithValue("@id", SqlDbType.Int);
        expdata.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;

        expdata.ExecuteProc("dbo.saveCurrencyRange");
        currencyrangeid = (int)expdata.sqlexecute.Parameters["@id"].Value;
        expdata.sqlexecute.Parameters.Clear();

        addExchangeRates(currencyrangeid, CurrencyType.Range, range.exchangerates, range.createdon, range.createdby);

        cRangeCurrency rangecurrency = getCurrencyById(range.currencyid);

        if (update)
        {
            rangecurrency.updateCurrencyRange(new cCurrencyRange(nAccountID, range.currencyid, currencyrangeid, range.startdate, range.enddate, range.createdon, range.createdby, range.modifiedon, range.modifiedby, range.exchangerates));
        }
        else
        {
            rangecurrency.addCurrencyRange(new cCurrencyRange(nAccountID, range.currencyid, currencyrangeid, range.startdate, range.enddate, range.createdon, range.createdby, null, null, range.exchangerates));
        }

        cCurrency.RemoveFromCache(nAccountID, range.currencyid);
        return currencyrangeid;
    }

    /// <summary>
    /// Delete the specified currency date range from the database
    /// </summary>
    /// <param name="currencyid">ID of the currency of the associated currency date range</param>
    /// <param name="currencyrangeid">ID of the currency date range</param>
    public void deleteCurrencyRange(int currencyid, int currencyrangeid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
        cRangeCurrency reqcur = getCurrencyById(currencyid);

        expdata.sqlexecute.Parameters.AddWithValue("@currencyrangeid", currencyrangeid);

        CurrentUser currentUser = cMisc.GetCurrentUser();
        if (currentUser != null)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
        }
        else
        {
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
        }

        expdata.ExecuteProc("dbo.deleteCurrencyRange");
        expdata.sqlexecute.Parameters.Clear();

        reqcur.deleteCurrencyRange(currencyrangeid);
    }

    /// <summary>
    /// Get an instance of a currency date range from the parent class 'cCurrencies' cached list of currencies
    /// </summary>
    /// <param name="currencyid">ID of the currency</param>
    /// <returns>A new instance of a currency date range</returns>
    public new cRangeCurrency getCurrencyById(int currencyid)
    {
        return (cRangeCurrency)list[currencyid];
    }

    /// <summary>
    /// Get the modified currency date ranges for expensesConnect
    /// </summary>
    /// <param name="date">Date of the last expensesConnect data synchronization</param>
    /// <returns>Collection of modified currency date ranges</returns>
    public Dictionary<int, cCurrencyRange> getModifiedCurrencyRanges(DateTime date)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
        Dictionary<int, cCurrencyRange> lstCurRanges = new Dictionary<int, cCurrencyRange>();
        System.Data.SqlClient.SqlDataReader reader;
        int currencyid, currencyrangeid;

        strsql = "SELECT * FROM currencyranges WHERE createdon > @date OR modifiedon > @date;";
        expdata.sqlexecute.Parameters.AddWithValue("@date", date);
        using (reader = expdata.GetReader(strsql))
        {
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
        }
        expdata.sqlexecute.Parameters.Clear();

        return lstCurRanges;
    }

    /// <summary>
    /// Get a list of all the currency date range ID's for expensesConnect
    /// </summary>
    /// <returns>collection of currency date range ID's</returns>
    public SortedList<int, int> getCurrencyRangeIds()
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
        System.Data.SqlClient.SqlDataReader reader;
        SortedList<int, int> lstids = new SortedList<int, int>();

        strsql = "SELECT currencyrangeid, currencyid FROM currencyranges";
        using (reader = expdata.GetReader(strsql))
        {
            while (reader.Read())
            {
                lstids.Add(reader.GetInt32(reader.GetOrdinal("currencyrangeid")), reader.GetInt32(reader.GetOrdinal("currencyid")));
            }
            reader.Close();
        }
        return lstids;
    }
}

