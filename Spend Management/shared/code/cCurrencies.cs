using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;

using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Helpers;
using expenses;
using System.Web.Caching;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;
using System.Data;

	/// <summary>
	/// Summary description for currencies.
	/// </summary>
public class cCurrencies
{
	protected int nAccountID = 0;
    protected int? nSubAccountId = 0;
	CurrencyType nCurrencytype = 0;
	protected string strsql;

    private List<ListItem> _dropDownList;

    public cCurrencies()
	{
	}
	public cCurrencies(int AccountID, int? subAccountId)
	{
		nAccountID = AccountID;
        nSubAccountId = subAccountId;

		InitialiseData();
	}

    #region properties
    private cAccountProperties getProperties
    {
        get
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts(AccountID);
            if (SubAccountID.HasValue)
            {
                return subaccs.getSubAccountById(SubAccountID.Value).SubAccountProperties;
            }
            else
            {
                return subaccs.getFirstSubAccount().SubAccountProperties;
            }
        }
    }
    public int? SubAccountID
    {
        get { return nSubAccountId; }
    }
    public int AccountID
    {
        get { return nAccountID; }
    }
    private string cacheKey
    {
        get
        {
            string key = "currencies" + AccountID.ToString();
            if (nSubAccountId.HasValue)
            {
                key += "_" + SubAccountID.Value.ToString();
            }
            return key;
        }
    }
    #endregion

	#region currencies
	public CurrencyType currencytype
	{
		get
		{
			if (nCurrencytype == 0)
			{
				cMisc clsmisc = new cMisc(AccountID);
				cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(AccountID);

				//System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
				nCurrencytype = (CurrencyType)((byte)clsproperties.currencytype);


			}
			return nCurrencytype;
		}
	}

    /// <summary>
    /// Update the currency type for the account 
    /// </summary>
    /// <param name="currencyType">The currency type</param>
    /// <param name="employeeId">The employeeId</param>
	public void ChangeCurrencyType(CurrencyType currencyType, int employeeId)
	{
        CurrentUser currentUser = cMisc.GetCurrentUser();
        var subAccounts = new cAccountSubAccounts(AccountID);
        cAccountProperties reqAccountProperties =
            subAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties.Clone();
        reqAccountProperties.currencyType = currencyType;
       
        subAccounts.SaveAccountProperties(
            reqAccountProperties,
            currentUser.EmployeeID,
            currentUser.isDelegate ? currentUser.Delegate.EmployeeID : (int?)null);

        //Remove currencies from cache to ensure they pick up the new exchange rates for the 
        //currency type when they are re-cached.
        RemoveCurrenciesFromCache();
	}

    private void RemoveCurrenciesFromCache()
    {
        foreach (int currencyId in this.currencyList.Keys)
        {
            cCurrency.RemoveFromCache(this.AccountID, currencyId);
        }
    }

	public Infragistics.WebUI.UltraWebGrid.ValueList CreateVList()
	{
		cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
		int i = 0;
		cCurrency reqcur;
		Infragistics.WebUI.UltraWebGrid.ValueList vlist = new Infragistics.WebUI.UltraWebGrid.ValueList();
		vlist.ValueListItems.Add(DBNull.Value, "GBP");

		for (i = 0; i < list.Count; i++)
		{
			reqcur = (cCurrency)list.GetByIndex(i);
			vlist.ValueListItems.Add(reqcur.currencyid, clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label);
		}

		return vlist;
	}

	public cColumnList CreateColumnList()
	{
		cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
		int i = 0;
		cCurrency reqcur;
		cColumnList vlist = new cColumnList();
		vlist.addItem(0, "GBP");

		for (i = 0; i < list.Count; i++)
		{
			reqcur = (cCurrency)list.GetByIndex(i);
			vlist.addItem(reqcur.currencyid, clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label);

		}

		return vlist;
	}

	public string[] getArray()
	{
		cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
		int i;
		cCurrency reqcur;
		string[] currencies = new string[list.Count];
		for (i = 0; i < list.Count; i++)
		{
			reqcur = (cCurrency)list.GetByIndex(i);
			currencies[i] = clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label;
		}
		return currencies;
	}


	/// <summary>
	/// Initial check to get the list of currencies from the web cache or if it is invalidated get the currencies from the database and
	/// then get them stored into the web cache
	/// </summary>
	protected void InitialiseData()
	{
	    _list =  new Lazy<SortedList>(GetList);
    }


    private static int? AllCurrencies = null;
    private Lazy<SortedList> _list;

    /// <summary>
    /// Accesses the list which causes it to be created if it is not already
    /// </summary>
    public SortedList list { get { return _list.Value; } }

    private SortedList GetList()
    {
        return GetList(AllCurrencies);
    }

    /// <summary>
    /// Store an untyped collection of all currency types into the web cache
    /// </summary>
    /// <returns>Untyped collection of all currency types</returns>
    private SortedList GetList(int? currencyId)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
		SqlDataReader reader;

	    cCurrency reqcurrency = null;

	    SortedList currencies = new SortedList();
		SortedList<string, object> parameters = new SortedList<string, object>();
	    SortedList<int, SortedList<int, cCurrencyMonth>> lstMonthly = null;
	    SortedList<int, SortedList<int, cCurrencyRange>> lstRanges = null;
        SortedList<int, SortedList<int, double>> lstExchangeRates = null;

	    switch (currencytype)
		{
			case CurrencyType.Static:
                lstExchangeRates = getExchangeRates(CurrencyType.Static, currencyId);
				break;
			case CurrencyType.Monthly:
                lstMonthly = getMonthlyExchangeRates(currencyId);
				break;
			case CurrencyType.Range:
                lstRanges = getRangeExchangeRates(currencyId);
				break;
		}

	    List<string> conditions = new List<string>();
        strsql = 
            "select currencyid, globalcurrencyid, positiveFormat, negativeformat, archived, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, subAccountId " + 
            "from dbo.currencies ";
        if (currencyId.HasValue)
        {
            conditions.Add(" currencyid = @currencyid ");
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyId.Value);
        }

        if (SubAccountID.HasValue)
        {
            conditions.Add(" subAccountId = @subAccId ");
            expdata.sqlexecute.Parameters.AddWithValue("@subAccId", SubAccountID.Value);
        }

        if (conditions.Any())
        {
            strsql += " where " + string.Join(" AND ", conditions);
        }

        using (reader = expdata.GetReader(strsql))
        {
            while (reader.Read())
            {
                int currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                int globalcurrencyid = reader.GetInt32(reader.GetOrdinal("globalcurrencyid"));
                byte positiveFormat = reader.GetValueOrDefault("positiveFormat", (byte)0);
                byte negativeFormat = reader.GetValueOrDefault("negativeFormat",(byte)0);
                bool archived = reader.GetBoolean(reader.GetOrdinal("archived"));
                DateTime createdon = reader.GetValueOrDefault("createdon", new DateTime(1900, 01, 01));
                int createdby = reader.GetValueOrDefault("createdby", 0);
                DateTime? modifiedon = reader.GetNullable<DateTime>("modifiedon");
                int? modifiedby = reader.GetNullable<int>("modifiedby");

                switch (currencytype)
                {
                    case CurrencyType.Static:
                        SortedList<int, double> rates;
                        if (!lstExchangeRates.TryGetValue(currencyid, out rates))
                        {
                            rates = new SortedList<int, double>();
                        }
                        reqcurrency = new cStaticCurrency(AccountID, currencyid, globalcurrencyid, positiveFormat, negativeFormat, archived, createdon, createdby, modifiedon, modifiedby, rates);
                        break;
                    case CurrencyType.Monthly:
                        SortedList<int, cCurrencyMonth> monthly;
                        if (!lstMonthly.TryGetValue(currencyid, out monthly))
                        {
                            monthly = new SortedList<int, cCurrencyMonth>();
                        }
                        reqcurrency = new cMonthlyCurrency(AccountID, currencyid, globalcurrencyid, positiveFormat, negativeFormat, archived, createdon, createdby, modifiedon, modifiedby, monthly);
                        break;
                    case CurrencyType.Range:
                        SortedList<int, cCurrencyRange> ranges;
                        if (!lstRanges.TryGetValue(currencyid, out ranges))
                        {
                            ranges = new SortedList<int, cCurrencyRange>();
                        }
                        reqcurrency = new cRangeCurrency(AccountID, currencyid, globalcurrencyid, positiveFormat, negativeFormat, archived, createdon, createdby, modifiedon, modifiedby, ranges);
                        break;
                }
                currencies.Add(currencyid, reqcurrency);
            }
        }
		return currencies;
	}

    /// <summary>
    /// Generate the exchange rate table for the specified currency type
    /// </summary>
    /// <param name="currencyid">ID of the currency</param>
    /// <param name="id">ID of the currency type e.g. currencymonthid or currencyrangeid</param>
    /// <param name="currType">The type of currency</param>
    /// <returns>The HTML code to render to the page </returns>
    public string CreateExchangeTable(int currencyid, int id, CurrencyType currType)
	{
		DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
		System.Data.SqlClient.SqlDataReader reader;
		int tocurrencyid;
		double exchangeRate;
		string sExchangeRate;
		int globalcurrencyid;
		string rowclass = "row1";
		System.Text.StringBuilder output = new System.Text.StringBuilder();
		cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();

        StringBuilder strsql = new StringBuilder();

		switch (currType)
		{
			case CurrencyType.Static:
                strsql.Append("select tocurrencyid, currencies.globalcurrencyid, null as label, static_exchangerates.exchangerate from static_exchangerates inner join currencies on static_exchangerates.tocurrencyid = currencies.currencyid where static_exchangerates.currencyid = @currencyid ");
                if (SubAccountID.HasValue)
                {
                    strsql.Append(" and currencies.subAccountId = @subAccId ");
                }
                strsql.Append("union ");
                strsql.Append("select currencyid as tocurrencyid, currencies.globalcurrencyid, null as label, null as exchangerate from currencies where currencyid <> @currencyid and currencyid not in (select tocurrencyid from static_exchangerates where currencyid = @currencyid)");
                if (SubAccountID.HasValue)
                {
                    strsql.Append(" and subAccountId = @subAccId ");
                }
                break;
			case CurrencyType.Monthly:
                strsql.Append("select tocurrencyid, currencies.globalcurrencyid, null as label, monthly_exchangerates.exchangerate from monthly_exchangerates inner join currencies on monthly_exchangerates.tocurrencyid = currencies.currencyid where currencymonthid = @currencymonthid ");
                if (SubAccountID.HasValue)
                {
                    strsql.Append(" and currencies.subAccountId = @subAccId ");
                }
                strsql.Append("union ");
                strsql.Append("select currencyid as tocurrencyid, currencies.globalcurrencyid, null as label, null as exchangerate from currencies where currencyid <> @currencyid and currencyid not in (select tocurrencyid from monthly_exchangerates where currencymonthid = @currencymonthid)");
                if (SubAccountID.HasValue)
                {
                    strsql.Append(" and subAccountId = @subAccId ");
                }
				expdata.sqlexecute.Parameters.AddWithValue("@currencymonthid", id);
				break;
			case CurrencyType.Range:
                strsql.Append("select tocurrencyid, currencies.globalcurrencyid, null as label, range_exchangerates.exchangerate from range_exchangerates inner join currencies on range_exchangerates.tocurrencyid = currencies.currencyid where currencyrangeid = @currencyrangeid ");
                if (SubAccountID.HasValue)
                {
                    strsql.Append(" and currencies.subAccountId = @subAccId ");
                }
                strsql.Append("union ");
                strsql.Append("select currencyid as tocurrencyid, currencies.globalcurrencyid, null as label, null as exchangerate from currencies where currencyid <> @currencyid and currencyid not in (select tocurrencyid from range_exchangerates where currencyrangeid = @currencyrangeid)");
                if (SubAccountID.HasValue)
                {
                    strsql.Append(" and subAccountId = @subAccId ");
                }
				expdata.sqlexecute.Parameters.AddWithValue("@currencyrangeid", id);
				break;
		}

		expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
        if (SubAccountID.HasValue)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@subAccId", SubAccountID.Value);
        }

        using (reader = expdata.GetReader(strsql.ToString()))
        {
            output.Append("<table class=datatbl>");
            output.Append("<tr><th>Currency</th><th style='width: 120px;'>Exchange Rate</th></tr>");

            while (reader.Read())
            {
                tocurrencyid = reader.GetInt32(reader.GetOrdinal("tocurrencyid"));
                globalcurrencyid = reader.GetInt32(reader.GetOrdinal("globalcurrencyid"));

                if (reader.IsDBNull(reader.GetOrdinal("exchangerate")) == true)
                {
                    exchangeRate = 0;
                    sExchangeRate = "";
                }
                else
                {
                    exchangeRate = reader.GetDouble(reader.GetOrdinal("exchangerate"));
                    sExchangeRate = exchangeRate.ToString();
                }

                output.Append("<tr>");
                output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(globalcurrencyid).label + "</td>");
                output.Append("<td class=\"" + rowclass + "\">");


                output.Append("<input type=text style='margin-left:25px;width:50px;' onblur=\"validateItem('exchangerate" + tocurrencyid + "',1,'Exchange Rate');\" size=5 name=\"exchangerate" + tocurrencyid + "\" id=\"exchangerate" + tocurrencyid + "\" value=\"" + sExchangeRate + "\">");
                output.Append("</tr>");
                if (rowclass == "row1")
                {
                    rowclass = "row2";
                }
                else
                {
                    rowclass = "row1";
                }
            }
            reader.Close();
        }
		output.Append("</table>");

		return output.ToString();
	}

	    /// <summary>
	    /// Get a currency from the cached list of currencies by it's unique ID
	    /// </summary>
	    /// <param name="currencyid">ID of the currency</param>
	    /// <returns>The currency object associated with the ID</returns>
	    public virtual cCurrency getCurrencyById(int currencyid)
	    {
	        cCurrency currency = cCurrency.GetFromCache(AccountID, currencyid);
	        if (currency == null)
	        {
	            var listOfOne = GetList(currencyid);
                currency = (cCurrency)listOfOne[currencyid];
                if (currency != null)
                {
                    currency.AddToCache();    
                }
	        }

	        return currency;
	    }


	    /// <summary>
	/// Get a currency by a global ID which is stored in the metabase where all available currencies for all companies
	/// are stored
	/// </summary>
	/// <param name="id">Global ID of the currency</param>
	/// <returns>The currency object associated with the global ID</returns>
	public cCurrency getCurrencyByGlobalCurrencyId(int id)
	{
		foreach (cCurrency currency in list.Values)
		{
			if (currency.globalcurrencyid == id)
			{
				return currency;
			}
		}
		return null;
	}

	/// <summary>
	/// Get a currency by it's globally unique numeric code
	/// </summary>
	/// <param name="code">Globally unique numeric code</param>
	/// <returns>The currency object associated with the numeric code</returns>
	public cCurrency getCurrencyByNumericCode(string code)
	{
		cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
		cGlobalCurrency global;
		foreach (cCurrency currency in list.Values)
		{
			global = clsglobalcurrencies.getGlobalCurrencyById(currency.globalcurrencyid);
			if (global.numericcode.ToLower().Trim() == code.ToLower().Trim())
			{
				return currency;
			}
		}
		return null;
	}

	/// <summary>
	/// Get a currency by it's globally unique alpha code e.g. 'GBP' for pound sterling
	/// </summary>
	/// <param name="code">Globally unique alpha code</param>
	/// <returns>The currency object associated with the alpha code</returns>
	public cCurrency getCurrencyByAlphaCode(string code)
	{
		cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
		cGlobalCurrency global;
		foreach (cCurrency currency in list.Values)
		{
			global = clsglobalcurrencies.getGlobalCurrencyById(currency.globalcurrencyid);
			if (global.alphacode.ToLower().Trim() == code.ToLower().Trim())
			{
				return currency;
			}
		}
		return null;
	}

	public double getExchangeRate(int fromcurrency, int currencyid, DateTime date)
	{
	    cCurrencies clscurrencies = new cCurrencies(AccountID, SubAccountID);

	    return clscurrencies.getCurrencyById(fromcurrency).getExchangeRate(currencyid, date);
	}

	public cCurrency GetCurrencyByName(string currency)
	{
		cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
		int i;
		cCurrency reqcur;
		for (i = 0; i < list.Count; i++)
		{
			reqcur = (cCurrency)list.GetByIndex(i);
			if (clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label == currency)
			{
				return reqcur;
			}
		}
		return null;
	}

    /// <summary>
    /// Delete a currency from expenses
    /// </summary>
    /// <param name="currencyid">Unique ID for the currency</param>
    /// <returns>An int value signifying the success or fail of the deletion</returns>
    public int deleteCurrency(int currencyid)
    {
        cCurrency currency = getCurrencyById(currencyid);
        if (currency != null)
        {
            var employeeList = this.GetAffectedEmployees(currencyid);
            var expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
            expdata.sqlexecute.Parameters.AddWithValue("@globalcurrencyid", currency.globalcurrencyid);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", currency.createdby);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            else
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

            expdata.sqlexecute.Parameters.Add("@returncode", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returncode"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("dbo.deleteCurrency");
            var returncode = (int)expdata.sqlexecute.Parameters["@returncode"].Value;

            expdata.sqlexecute.Parameters.Clear();
            if (returncode > 0)
            {
                return returncode;
            }

            foreach (int employeeId in employeeList)
            {
                User.CacheRemove(employeeId, this.AccountID);    
            }

            list.Remove(currencyid);
            cCurrency.RemoveFromCache(AccountID, currencyid);
        }

        return 0;
    }

        /// <summary>
        /// Get employees affected by the currency delete.
        /// </summary>
        /// <param name="currencyid">
        /// The currency id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<int> GetAffectedEmployees(int currencyid)
        {
            var expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            var sql = "SELECT employeeid FROM dbo.employees WHERE primarycurrency = @currencyid";
            var result = new List<int>();
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
            using (var reader = expdata.GetReader(sql))
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0));
                }
            }

            return result;
        }

	    public ListItem[] CreateDropDown(int currencyid)
	{
		cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
		cCurrency reqcur;
		List<ListItem> tempitems = new List<ListItem>();
		SortedList sorted = new SortedList();
		int i = 0;

		//tempitems[0] = new System.Web.UI.WebControls.ListItem();
		//tempitems[0].Text = "GBP";
		//tempitems[0].Value = "0";

		for (i = 0; i < list.Count; i++) //sort the list
		{
			reqcur = (cCurrency)list.GetByIndex(i);
			sorted.Add(clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label, reqcur);
		}
		for (i = 0; i < sorted.Count; i++)
		{
			reqcur = (cCurrency)sorted.GetByIndex(i);
            if (!reqcur.archived || (reqcur.archived && reqcur.currencyid == currencyid))
            {
                ListItem item = new ListItem(clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label, reqcur.currencyid.ToString());
                if(currencyid == reqcur.currencyid)
                {
                    item.Selected = true;
                }
                tempitems.Add(item);
            }
		}

        return tempitems.ToArray();
	}

	public List<System.Web.UI.WebControls.ListItem> CreateDropDown()
	{
	    if (this._dropDownList == null)
	    {
	        using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.AccountID)))
	        {
                expdata.ClearParameters();
                var subAccountClause = "AND subAccountId = @subaccountid ";

                if (this.SubAccountID.HasValue)
                {
                    expdata.AddWithValue("@subAccountId", this.SubAccountID);

                }
                else
                {
                    subAccountClause = string.Empty;
                }
                this._dropDownList = new List<ListItem>();
                using (var reader = expdata.GetReader($"SELECT label, currencyid, subaccountid FROM currencies INNER JOIN global_currencies ON global_currencies.globalcurrencyid = currencies.globalcurrencyid WHERE archived = 0 {subAccountClause} ORDER BY label"))
                {
                    if (reader != null)
	                {
	                    while (reader.Read())
	                    {
	                        this._dropDownList.Add(new ListItem(reader.GetString(0), reader.GetInt32(1).ToString()));
	                    }
	                }
	            }
	        }
	    }

		return this._dropDownList;
	}

	public List<System.Web.UI.WebControls.ListItem> CreatePositiveFormatDropDown()
	{
		List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();

		foreach (KeyValuePair<int, string> kp in cPositiveFormat.lstPositiveFormats)
		{
			items.Add(new System.Web.UI.WebControls.ListItem(kp.Value, kp.Key.ToString()));
		}

		return items;
	}

	public List<System.Web.UI.WebControls.ListItem> CreateNegativeFormatDropDown()
	{
		List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();

		foreach (KeyValuePair<int, string> kp in cNegativeFormat.lstNegativeFormats)
		{
			items.Add(new System.Web.UI.WebControls.ListItem(kp.Value, kp.Key.ToString()));
		}

		return items;
	}

	protected SortedList sortList()
	{
		cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
		SortedList sorted = new SortedList();
		cCurrency currency;

		for (int i = 0; i < list.Count; i++)
		{
			currency = (cCurrency)list.GetByIndex(i);

			sorted.Add(clsglobalcurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label, currency);
		}
		return sorted;
	}

	#endregion

	public SortedList currencyList
	{
		get { return list; }
	}



	public SortedList getModifiedCurrencies(DateTime date)
	{
		SortedList lst = new SortedList();

		switch (currencytype)
		{
			case CurrencyType.Static:

				foreach (cStaticCurrency staVal in list.Values)
				{
					if (staVal.createdon > date)
					{
						lst.Add(staVal.currencyid, staVal);
					}
				}

				break;
			case CurrencyType.Monthly:

				foreach (cMonthlyCurrency monVal in list.Values)
				{
					if (monVal.createdon > date)
					{
						lst.Add(monVal.currencyid, monVal);
					}
				}

				break;
			case CurrencyType.Range:

				foreach (cRangeCurrency raVal in list.Values)
				{
					if (raVal.createdon > date)
					{
						lst.Add(raVal.currencyid, raVal);
					}
				}

				break;
		}

		return lst;
	}

	public string getGrid()
	{
		return "SELECT currencies.currencyid, currencies.archived, global_currencies.label, global_currencies.alphacode, global_currencies.numericcode, global_currencies.currencySymbol, currencies.subAccountID FROM currencies";
	}

	public virtual int saveCurrency(cCurrency currency)
	{
        if (currency.currencyid == 0)
        {
            if (currencyExists(currency.globalcurrencyid))
            {
                return -1;
            }
        }

		DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));

		int currencyid;
        if (SubAccountID.HasValue)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", SubAccountID.Value);
        }
        else
        {
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", DBNull.Value);
        }

		expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currency.currencyid);
		expdata.sqlexecute.Parameters.AddWithValue("@globalcurrencyid", currency.globalcurrencyid);
		expdata.sqlexecute.Parameters.AddWithValue("@positiveFormat", currency.positiveFormat);
		expdata.sqlexecute.Parameters.AddWithValue("@negativeFormat", currency.negativeFormat);

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

		if (currency.currencyid > 0)
		{
			expdata.sqlexecute.Parameters.AddWithValue("@date", currency.modifiedon);
		    if (currency.modifiedby.HasValue)
		    {
		        expdata.sqlexecute.Parameters.AddWithValue("@userid", currency.modifiedby);
		    }
		    else
		    {
		        expdata.sqlexecute.Parameters.AddWithValue("@userid", DBNull.Value);
            }
		}
		else
		{
			expdata.sqlexecute.Parameters.AddWithValue("@date", currency.createdon);
		    if (currency.createdby.HasValue)
		    {
		        expdata.sqlexecute.Parameters.AddWithValue("@userid", currency.createdby);
		    }
		    else
		    {
		        expdata.sqlexecute.Parameters.AddWithValue("@userid", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
			expdata.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
		}

		expdata.ExecuteProc("dbo.saveCurrency");

		if (currency.currencyid == 0)
		{
			currencyid = (int)expdata.sqlexecute.Parameters["@id"].Value;
		}
		else
		{
			currencyid = currency.currencyid;
		}

		expdata.sqlexecute.Parameters.Clear();
        cCurrency.RemoveFromCache(AccountID, currencyid);

		return currencyid;
	}

    private bool currencyExists(int GlobalCurrencyID)
    {
        foreach (cCurrency curr in list.Values)
        {
            if (curr.globalcurrencyid == GlobalCurrencyID)
            {
                return true;
            }
        }

        return false;
    }


	public int changeStatus(int currencyid, bool archive)
	{
		DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
		//cannot remove if assigned to an expense item or primary currency for the company

		expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
        expdata.sqlexecute.Parameters.AddWithValue("@archive", Convert.ToByte(archive));
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
		expdata.sqlexecute.Parameters.Add("@returncode", SqlDbType.Int);
		expdata.sqlexecute.Parameters["@returncode"].Direction = ParameterDirection.ReturnValue;
		expdata.ExecuteProc("changeCurrencyStatus");
		int returncode = (int)expdata.sqlexecute.Parameters["@returncode"].Value;
		expdata.sqlexecute.Parameters.Clear();
		if (returncode > 0)
		{
			return returncode;
		}

		return 0;
	}

	#region exchange rates

	    /// <summary>
	    /// Get the exchange rates for the specific curency type from the database
	    /// </summary>
	    /// <param name="currType">Enumerable type of the currency</param>
	    /// <param name="currencyId"></param>
	    /// <returns>A collection of exchange rates</returns>
	public SortedList<int, SortedList<int, double>> getExchangeRates(CurrencyType currType, int? currencyId)
	{
		DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
        SortedList<int, SortedList<int, double>> lstExchangeRates = new SortedList<int, SortedList<int, double>>();
	    SqlDataReader reader;
		string sql = "";
	        switch (currType)
		{
			case CurrencyType.Static:
                sql = "select currencyid as id, tocurrencyid, exchangerate from static_exchangerates ";
                if (currencyId.HasValue)
                {
                    sql += " where currencyid = @currencyid";
                    expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyId.Value);
                }
				break;
			case CurrencyType.Monthly:
				sql = "select currencymonthid as id, tocurrencyid, exchangerate from monthly_exchangerates m ";
                if (currencyId.HasValue)
                {
                    sql += 
                        " where exists " + 
                        "    (select * from currencymonths cm " + 
                        "     where cm.currencymonthid = m.currencymonthid " + 
                        "     and cm.currencyid = @currencyid) ";
                    expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyId.Value);
                }
				break;
			case CurrencyType.Range:
				sql = "select currencyrangeid as id, tocurrencyid, exchangerate from range_exchangerates r ";
                if (currencyId.HasValue)
                {
                    sql +=
                        " where exists " +
                        "    (select * from currencyranges cr " +
                        "     where cr.currencyrangeid = r.currencyrangeid " +
                        "     and cr.currencyid = @currencyid) ";
                    expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyId.Value);
                }
				break;
		}

        using (reader = expdata.GetReader(sql))
        {
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                int tocurrencyid = reader.GetInt32(reader.GetOrdinal("tocurrencyid"));
                double exchangerate = reader.GetDouble(reader.GetOrdinal("exchangerate"));
                SortedList<int, double> exchangerates;
                lstExchangeRates.TryGetValue(id, out exchangerates);
                if (exchangerates == null)
                {
                    exchangerates = new SortedList<int, double>();
                    lstExchangeRates.Add(id, exchangerates);
                }
                exchangerates.Add(tocurrencyid, exchangerate);

            }
            reader.Close();
        }
        return lstExchangeRates;
	}

	/// <summary>
	/// Add all exchange rates for the associated currency type
	/// </summary>
	/// <param name="id">ID of the currency type</param>
	/// <param name="currType">Enumerable currency type</param>
	/// <param name="exchangerates">Collection of exchange rates associtaed with the currency type</param>
	/// <param name="createdon">Date the exchange rate was created</param>
	/// <param name="createdby">ID of the user who created the exchange rate</param>
	public void addExchangeRates(int id, CurrencyType currType, SortedList<int, double> exchangerates, DateTime createdon, int createdby)
	{
		deleteExchangeRates(id, currType);

		DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
		foreach (KeyValuePair<int, double> kp in exchangerates)
		{
			byte type = (byte)currType;
			expdata.sqlexecute.Parameters.AddWithValue("@id", id);
			expdata.sqlexecute.Parameters.AddWithValue("@tocurrencyid", kp.Key);
            
			expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", kp.Value);
			expdata.sqlexecute.Parameters.AddWithValue("@tableType", type);
			expdata.sqlexecute.Parameters.AddWithValue("@date", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", createdby);
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
			expdata.ExecuteProc("dbo.addExchangeRate");
			expdata.sqlexecute.Parameters.Clear();
		}
	}

	/// <summary>
	/// Delete all exchange rates of the associated currency
	/// </summary>
	/// <param name="id">ID of the associated currency</param>
	/// <param name="currType">Enumerable currency type</param>
	public void deleteExchangeRates(int id, CurrencyType currType)
	{
		DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
		expdata.sqlexecute.Parameters.AddWithValue("@id", id);
        expdata.sqlexecute.Parameters.AddWithValue("@tableType", currType);
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
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
        }
		expdata.ExecuteProc("dbo.deleteExchangeRates");
		expdata.sqlexecute.Parameters.Clear();
	}

	private SortedList<int, SortedList<int, cCurrencyMonth>> getMonthlyExchangeRates(int? currencyId)
	{
        SortedList<int, SortedList<int, double>> lstExchangeRates = getExchangeRates(CurrencyType.Monthly, null);
	    SortedList<int, SortedList<int, cCurrencyMonth>> lstRates = new SortedList<int, SortedList<int, cCurrencyMonth>>();
	    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));

	    expdata.sqlexecute.Parameters.Clear();

	    var sql = "select currencyid, currencymonthid, month, year, createdon, createdby, modifiedon, modifiedby from currencymonths ";
        if (currencyId.HasValue)
        {
            sql += " where currencyid = @currencyid ";
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyId.Value);
        }

	    using (SqlDataReader reader = expdata.GetReader(sql))
        {
            while (reader.Read())
            {
                int currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                int currencymonthid = reader.GetInt32(reader.GetOrdinal("currencymonthid"));
                byte month = reader.GetByte(reader.GetOrdinal("month"));
                int year = reader.GetInt16(reader.GetOrdinal("year"));
                DateTime createdon = reader.GetValueOrDefault("createdon", new DateTime(1900, 01, 01));
                int createdby = reader.GetValueOrDefault("createdby", 0);
                DateTime modifiedon = reader.GetValueOrDefault("modifiedon", new DateTime(1900, 01, 01));
                int modifiedby = reader.GetValueOrDefault("modifiedby", 0);
                SortedList<int, cCurrencyMonth> cCurrencyMonths;
                if (!lstRates.TryGetValue(currencyid, out cCurrencyMonths))
                {
                    cCurrencyMonths = new SortedList<int, cCurrencyMonth>();
                    lstRates.Add(currencyid, cCurrencyMonths);
                }
                SortedList<int, double> rates;
                lstExchangeRates.TryGetValue(currencymonthid, out rates);
                if (rates == null)
                {
                    rates = new SortedList<int, double>();
                }
                cCurrencyMonth curmonth = new cCurrencyMonth(AccountID, currencyid, currencymonthid, month, year, createdon, createdby, modifiedon, modifiedby, rates);
                cCurrencyMonths.Add(currencymonthid, curmonth);
            }
        }

		return lstRates;
	}

	private SortedList<int, SortedList<int, cCurrencyRange>> getRangeExchangeRates(int? currencyId)
	{
        SortedList<int, SortedList<int, double>> lstExchangeRates = getExchangeRates(CurrencyType.Range, currencyId);

	    SortedList<int, SortedList<int, cCurrencyRange>> ranges = new SortedList<int, SortedList<int, cCurrencyRange>>();
	    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
	    expdata.sqlexecute.Parameters.Clear();

	    var sql = "select currencyid, currencyrangeid, startdate, enddate, createdon, createdby, modifiedon, modifiedby from currencyranges ";
        if (currencyId.HasValue)
        {
            sql += " where currencyid = @currencyid";
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyId.Value);
        }

	    using (SqlDataReader reader = expdata.GetReader(sql))
        {
            while (reader.Read())
            {
                int currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                int currencyrangeid = reader.GetInt32(reader.GetOrdinal("currencyrangeid"));
                DateTime startdate = reader.GetDateTime(reader.GetOrdinal("startdate"));
                DateTime enddate = reader.GetDateTime(reader.GetOrdinal("enddate"));
                DateTime createdon = reader.GetValueOrDefault("createdon", new DateTime(1900, 01, 01));
                int createdby = reader.GetValueOrDefault("createdby", 0);
                DateTime modifiedon = reader.GetValueOrDefault("modifiedon", new DateTime(1900, 01, 01));
                int modifiedby = reader.GetValueOrDefault("modifiedby", 0);
                SortedList<int, cCurrencyRange> cCurrencyRanges;
                if (!ranges.TryGetValue(currencyid, out cCurrencyRanges))
                {
                    cCurrencyRanges = new SortedList<int, cCurrencyRange>();
                    ranges.Add(currencyid, cCurrencyRanges);
                }
                SortedList<int, double> rates;
                if (!lstExchangeRates.TryGetValue(currencyrangeid, out rates))
                {
                    rates = new SortedList<int, double>();
                }
                cCurrencyRange currange = new cCurrencyRange(AccountID, currencyid, currencyrangeid, startdate, enddate, createdon, createdby, modifiedon, modifiedby, rates);
                cCurrencyRanges.Add(currencyrangeid, currange);
            }
            reader.Close();
        }
		return ranges;
	}
	#endregion

	public bool checkCurrencyMonthExists(int currencyid, int currMonthID, int month, int year)
	{
		foreach (cMonthlyCurrency monthcurr in list.Values)
		{
			foreach (cCurrencyMonth cmonth in monthcurr.exchangerates.Values)
			{
                if (currMonthID == 0)
                {
                    if (month == cmonth.month && year == cmonth.year && cmonth.currencyid == currencyid)
                    {
                        return true;
                    }
                }
                else
                {
                    if (month == cmonth.month && year == cmonth.year && cmonth.currencyid == currencyid && cmonth.currencymonthid != currMonthID)
                    {
                        return true;
                    }
                }
			}
		}
		return false;
	}

    /// <summary>
    /// Check to see if a range already exists, passing in a currRangeID checks to see if a different currencyRange covers that period
    /// </summary>
    /// <param name="currencyid">Currency to check</param>
    /// <param name="currRangeID">0 normally, specify a currRangeID if you want to exclude it from the check</param>
    /// <param name="startDate">Start Date</param>
    /// <param name="endDate">End Date</param>
    /// <returns>Boolean indicating whether the range overlaps</returns>
	public bool checkCurrencyRangeExists(int currencyid, int currRangeID, DateTime startDate, DateTime endDate)
	{
		foreach (cRangeCurrency rangecurr in list.Values)
		{
			foreach (cCurrencyRange crange in rangecurr.exchangerates.Values)
			{
                if (currRangeID == 0)
                {
                    if (startDate >= crange.startdate && startDate <= crange.enddate && crange.currencyid == currencyid)
                    {
                        return true;
                    }
                    else if (endDate >= crange.startdate && endDate <= crange.enddate && crange.currencyid == currencyid)
                    {
                        return true;
                    }
                }
                else
                {
                    if (startDate >= crange.startdate && startDate <= crange.enddate && crange.currencyid == currencyid && crange.currencyrangeid != currRangeID)
                    {
                        return true;
                    }
                    else if (endDate >= crange.startdate && endDate <= crange.enddate && crange.currencyid == currencyid && crange.currencyrangeid != currRangeID)
                    {
                        return true;
                    }
                }
			}
		}
		return false;
	}

    /// <summary>
    /// Overloaded version to take double parameter for legacy framework screens
    /// </summary>
    /// <param name="value">Monetary amount to be formatted</param>
    /// <param name="currency">Currency object</param>
    /// <param name="FormatForEdit">Flag to format with or without the currency symbol</param>
    /// <returns></returns>
    public object FormatCurrency(double value, cCurrency currency, bool FormatForEdit)
    {
        decimal cnvValue = 0;
        decimal.TryParse(value.ToString(), out cnvValue);

        return FormatCurrency(cnvValue, currency, FormatForEdit);
    }

	/// <summary>
	/// Format the currency to the stored positive or negative format
	/// </summary>
	/// <param name="value">Actual decimal value of the total</param>
	/// <param name="currency">Currency object</param>
	/// <param name="FormatForEdit">Flag to format with or without the currency symbol</param>
	/// <returns></returns>
	public object FormatCurrency(decimal value, cCurrency currency, bool FormatForEdit)
	{
        if (currency != null)
        {
            // This part is separate as it is called by the Currency Code maintenance, to show
            // examples of the chosen formats

            string CField;
            string temp;
            string CWhole;
            string CDecimal;
            int E;
            int x;

            cGlobalCurrencies globCurrencies = new cGlobalCurrencies();
            cGlobalCurrency globalCurrency = globCurrencies.getGlobalCurrencyById(currency.globalcurrencyid);

            string symbol = globalCurrency.symbol;
            // Set up as string of digits with single decimal point (if decimals present)
            CField = Math.Abs(value).ToString().Trim();

            // Round to required decimal places
            CField = Math.Round(double.Parse(CField), 2, MidpointRounding.AwayFromZero).ToString().Trim();

            // Split at the decimal point
            x = CField.IndexOf(".");
            if (x > 0)
            {
                string[] strTemp = CField.Split('.');
                CWhole = strTemp[0];
                CDecimal = strTemp[1];
            }
            else
            {
                CWhole = CField;
                CDecimal = "";
            }

            // Split the //whole// part into groups, using the group symbol
            if (CWhole.Length > 3)
            {
                E = CWhole.Length - 1;
                temp = "";
                while (E >= 0)
                {
                    for (x = 0; x < 3; x++)
                    {
                        temp = CWhole.Substring(E, 1) + temp;
                        E = E - 1;
                        if (E < 0)
                        {
                            break;
                        }
                    }

                    if (E >= 0)
                    {
                        temp = "," + temp;
                    }
                }
                CWhole = temp.Trim();
            }


            // pad out the decimal places with sufficient zeros
            while (CDecimal.Length < 2)
            {
                // pad it with zeros
                CDecimal = CDecimal + "0";
            }

            CDecimal = CDecimal.Substring(0, 2).Trim();


            if (!FormatForEdit)
            {
                // Format the field using the symbols from the Currency Code record
                if (value >= 0)
                {
                    switch (currency.positiveFormat)
                    {
                        case 1:      // X1.1
                            CField = symbol + CWhole + "." + CDecimal;
                            break;
                        case 2:      // 1.1X
                            CField = CWhole + "." + CDecimal + symbol;
                            break;
                        case 3:      // X 1.1
                            CField = symbol + " " + CWhole + "." + CDecimal;
                            break;
                        case 4:      // 1.1 X
                            CField = CWhole + "." + CDecimal + " " + symbol;
                            break;
                        default:
                            CField = symbol + CWhole + "." + CDecimal;
                            break;
                    }
                }
                else
                {
                    switch (currency.negativeFormat)
                    {
                        case 1:      // -X1.1
                            CField = "-" + symbol + CWhole + "." + CDecimal;
                            break;
                        case 2:      // (X1.1)
                            CField = "(" + symbol + CWhole + "." + CDecimal + ")";
                            break;
                        case 3:      // X-1.1
                            CField = symbol + "-" + CWhole + "." + CDecimal;
                            break;
                        case 4:      // X1.1-
                            CField = symbol + CWhole + "." + CDecimal + "-";
                            break;
                        case 5:      // (1.1X)
                            CField = "(" + CWhole + "." + CDecimal + symbol + ")";
                            break;
                        case 6:      // -1.1X
                            CField = "-" + CWhole + "." + CDecimal + symbol;
                            break;
                        case 7:      // 1.1-X
                            CField = CWhole + "." + CDecimal + "-" + symbol;
                            break;
                        case 8:      // 1.1X-
                            CField = CWhole + "." + CDecimal + symbol + "-";
                            break;
                        case 9:      // (X 1.1)
                            CField = "(" + symbol + " " + CWhole + "." + CDecimal + ")";
                            break;
                        case 10:     // -X 1.1
                            CField = "-" + symbol + " " + CWhole + "." + CDecimal;
                            break;
                        case 11:     // X -1.1
                            CField = symbol + " -" + CWhole + "." + CDecimal;
                            break;
                        case 12:     // X 1.1-
                            CField = symbol + " " + CWhole + "." + CDecimal + "-";
                            break;
                        case 13:     // (1.1 X)
                            CField = "(" + CWhole + "." + CDecimal + " " + symbol + ")";
                            break;
                        case 14:     // -1.1 X
                            CField = "-" + CWhole + "." + CDecimal + " " + symbol;
                            break;
                        case 15:     // 1.1- X
                            CField = CWhole + "." + CDecimal + "- " + symbol;
                            break;
                        case 16:     // 1.1 X-
                            CField = CWhole + "." + CDecimal + " " + symbol + "-";
                            break;
                        default:
                            CField = "-" + symbol + CWhole + "." + CDecimal;
                            break;
                    }
                }
            }
            else
            {
                if (value < 0)
                {
                    CField = "-" + CWhole + "." + CDecimal;
                }
                else
                {
                    CField = CWhole + "." + CDecimal;
                }
            }
            return CField.Trim();
        }

        return null;
	}

    public decimal convertAmount(int convertFromCurrencyID, decimal amountToConvert, int convertToCurrencyID, DateTime? exchangeRateDate)
    {
        cCurrency fromCurrency = getCurrencyById(convertFromCurrencyID);
        decimal convertedTotal = 0;

        switch (getProperties.currencyType)
        {
            case CurrencyType.Static:
                cStaticCurrency standard = (cStaticCurrency)fromCurrency;
                convertedTotal = standard.convertCurrencyValue(amountToConvert, convertToCurrencyID);
                break;
            case CurrencyType.Monthly:
                cMonthlyCurrency monthly = (cMonthlyCurrency)fromCurrency;
                if (exchangeRateDate.HasValue)
                {
                    convertedTotal = monthly.convertCurrencyValue(amountToConvert, convertToCurrencyID, exchangeRateDate.Value);
                }
                else
                {
                    convertedTotal = monthly.convertCurrencyValue(amountToConvert, convertToCurrencyID, DateTime.Today);
                }
                break;
            case CurrencyType.Range:
                cRangeCurrency range = (cRangeCurrency)fromCurrency;
                if (exchangeRateDate.HasValue)
                {
                    convertedTotal = range.convertCurrencyValue(amountToConvert, convertToCurrencyID, exchangeRateDate.Value);
                }
                else
                {
                    convertedTotal = range.convertCurrencyValue(amountToConvert, convertToCurrencyID, DateTime.Today);
                }
                break;
            default:
                break;
        }
        return convertedTotal;    
    }

    public decimal convertToBase(int convertFromCurrencyID, decimal amountToConvert, DateTime? exchangeRateDate)
    {
        decimal convertedAmount = amountToConvert;

        if (getProperties.BaseCurrency.HasValue && convertFromCurrencyID != 0)
        {
            int baseCurrencyId = getProperties.BaseCurrency.Value;

            convertedAmount = convertAmount(convertFromCurrencyID, amountToConvert, baseCurrencyId, exchangeRateDate);
        }

        return convertedAmount;
    }
}
