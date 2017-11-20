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
/// Summary description for cMonthlyCurrencies
/// </summary>
public class cMonthlyCurrencies : cCurrencies
{
    /// <summary>
    /// Constructor for use if sub Accounts are being used
    /// </summary>
    /// <param name="subAccountID"></param>
    public cMonthlyCurrencies(int nAccountid, int? subAccountID): base(nAccountid, subAccountID)
    {
        InitialiseData();
    }

    /// <summary>
    /// Get SQL string of the columns used for the monthly currency grid
    /// </summary>
    /// <returns>SQL string</returns>
    public string getMonthGrid()
    {
        return "SELECT currencymonths.currencymonthid, currencymonths.month, currencymonths.year FROM currencymonths";
    }

    /// <summary>
    /// Save the currency month to the database
    /// </summary>
    /// <param name="month">Monthly currency object</param>
    /// <returns>ID of the saved currency month</returns>
    public int saveCurrencyMonth(cCurrencyMonth month)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
        int currencymonthid;
        bool update = false;

        expdata.sqlexecute.Parameters.AddWithValue("@currencyid", month.currencyid);
        expdata.sqlexecute.Parameters.AddWithValue("@currencymonthid", month.currencymonthid);
        expdata.sqlexecute.Parameters.AddWithValue("@year", month.year);
        expdata.sqlexecute.Parameters.AddWithValue("@month", month.month); 
        
        if (month.currencymonthid == 0)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@date", month.createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", month.createdby);
            
        }
        else
        {
            expdata.sqlexecute.Parameters.AddWithValue("@date", month.modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", month.modifiedby);
            update = true;
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

        expdata.ExecuteProc("saveCurrencyMonth");
        currencymonthid = (int)expdata.sqlexecute.Parameters["@id"].Value;
        expdata.sqlexecute.Parameters.Clear();

        addExchangeRates(currencymonthid, CurrencyType.Monthly, month.exchangerates, month.createdon, month.createdby);

        cMonthlyCurrency clscurrency = getCurrencyById(month.currencyid);

        if (update)
        {
            clscurrency.updateCurrencyMonth(new cCurrencyMonth(nAccountID, month.currencyid, currencymonthid, month.month, month.year, month.createdon, month.createdby, month.modifiedon, month.modifiedby, month.exchangerates));
        }
        else
        {
            clscurrency.addCurrencyMonth(new cCurrencyMonth(nAccountID, month.currencyid, currencymonthid, month.month, month.year, month.createdon, month.createdby, null, null, month.exchangerates));
        }

        return currencymonthid;
    }

    /// <summary>
    /// Delete the currency month from the database
    /// </summary>
    /// <param name="currencyid">ID of the currency</param>
    /// <param name="currencymonthid">ID of the currency month</param>
    public void deleteCurrencyMonth(int currencyid, int currencymonthid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
        cMonthlyCurrency reqcur = getCurrencyById(currencyid);

        expdata.sqlexecute.Parameters.AddWithValue("@currencymonthid", currencymonthid);
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
        expdata.ExecuteProc("dbo.deleteCurrencyMonth");
        expdata.sqlexecute.Parameters.Clear();
        reqcur.deleteCurrencyMonth(currencymonthid);
    }

    /// <summary>
    /// Get the monthly currency object from the cached list
    /// </summary>
    /// <param name="currencyid">ID of the currency</param>
    /// <returns>Monthly currency object</returns>
    public new cMonthlyCurrency getCurrencyById(int currencyid)
    {
        return (cMonthlyCurrency)list[currencyid];
    }


    public Dictionary<int, cCurrencyMonth> getModifiedCurrencyMonths(DateTime date)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
        Dictionary<int, cCurrencyMonth> lstCurMonths = new Dictionary<int, cCurrencyMonth>();
        System.Data.SqlClient.SqlDataReader reader;
        int currencyid, currencymonthid;

        strsql = "SELECT * FROM currencymonths WHERE createdon > @date OR modifiedon > @date;";
        expdata.sqlexecute.Parameters.AddWithValue("@date", date);

        using (reader = expdata.GetReader(strsql))
        {
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
        }

        expdata.sqlexecute.Parameters.Clear();

        return lstCurMonths;
    }

    public SortedList<int, int> getCurrencyMonthIds()
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(nAccountID));
        System.Data.SqlClient.SqlDataReader reader;
        SortedList<int, int> lstids = new SortedList<int, int>();

        strsql = "SELECT currencymonthid, currencyid FROM currencymonths";

        using (reader = expdata.GetReader(strsql))
        {
            while (reader.Read())
            {
                lstids.Add(reader.GetInt32(reader.GetOrdinal("currencymonthid")), reader.GetInt32(reader.GetOrdinal("currencyid")));
            }

            reader.Close();
        }

        return lstids;
    }
}


