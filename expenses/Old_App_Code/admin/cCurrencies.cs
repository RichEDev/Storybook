using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using expenses;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using System.Text;
using SpendManagementLibrary;
	/// <summary>
	/// Summary description for currencies.
	/// </summary>
	public class cCurrencies
	{
		protected int accountid = 0;
		CurrencyType nCurrencytype = 0;
		protected string strsql;
		
	

		protected System.Collections.SortedList list;
		
		public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

        public cCurrencies()
        {
        }
        public cCurrencies(int nAccountid)
        {
            accountid = nAccountid;

            InitialiseData();
        }

        #region currencies
        public CurrencyType currencytype
		{
			get 
			{
                if (nCurrencytype == 0)
                {
                    cMisc clsmisc = new cMisc(accountid);
                    cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

                    //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
                    nCurrencytype = (CurrencyType)((byte)clsproperties.currencytype);


                }
				return nCurrencytype;
			}
		}

		public void changeCurrencyType(byte currencytype)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			//System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			expdata.sqlexecute.Parameters.AddWithValue("@currencytype",currencytype);
			
			strsql = "update [other] set currencytype = @currencytype";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
            cMisc clsmisc = new cMisc(accountid);
            clsmisc.InvalidateGlobalProperties(accountid);

            Cache.Remove("currencies" + accountid);
            
		}

		public Infragistics.WebUI.UltraWebGrid.ValueList CreateVList()
		{
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
			int i = 0;
			cCurrency reqcur;
			Infragistics.WebUI.UltraWebGrid.ValueList vlist = new Infragistics.WebUI.UltraWebGrid.ValueList();
			vlist.ValueListItems.Add(DBNull.Value,"GBP");

			for (i = 0; i < list.Count; i++)
			{
				reqcur = (cCurrency)list.GetByIndex(i);
				vlist.ValueListItems.Add(reqcur.currencyid,clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label);
			}

			return vlist;
		}

		public cColumnList CreateColumnList()
		{
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
			int i = 0;
			cCurrency reqcur;
			cColumnList vlist = new cColumnList();
			vlist.addItem(0,"GBP");

			for (i = 0; i < list.Count; i++)
			{
				reqcur = (cCurrency)list.GetByIndex(i);
				vlist.addItem(reqcur.currencyid,clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label);
				
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



		protected void InitialiseData()
		{
            list = (System.Collections.SortedList)Cache["currencies" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			
		}

        
        public System.Collections.SortedList CacheList()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            int currencyid, globalcurrencyid;

            cCurrency reqcurrency = null;
            
            string clabel, currencycode, symbol;
            string purchasecardcurrencycode;
            DateTime createdon;
            int createdby;
            System.Collections.SortedList list = new System.Collections.SortedList();
            SortedList<string, object> parameters = new SortedList<string, object>();

            strsql = "select currencyid, globalcurrencyid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.currencies";
            SqlCacheDependency currencydep = expdata.CreateSQLCacheDependency(strsql, parameters);
            AggregateCacheDependency aggdep = new AggregateCacheDependency();
            
            switch (currencytype)
            {
                case CurrencyType.Static:
                    strsql = "SELECT     currencyid, tocurrencyid, exchangerate FROM dbo.static_exchangerates";
                    SqlCacheDependency staticdep = expdata.CreateSQLCacheDependency(strsql, parameters);
                    aggdep.Add(new CacheDependency[] { currencydep, staticdep });
                    break;
                case CurrencyType.Monthly:
                    strsql = "SELECT     currencymonthid, tocurrencyid, exchangerate FROM dbo.monthly_exchangerates";
                    SqlCacheDependency monthlyratesdep = expdata.CreateSQLCacheDependency(strsql, parameters);
                    strsql = "SELECT     currencymonthid, currencyid, exchangerate, month, year FROM dbo.currencymonths";
                    SqlCacheDependency monthlydep = expdata.CreateSQLCacheDependency(strsql, parameters);
                    aggdep.Add(new CacheDependency[] { monthlyratesdep, monthlydep });
                    break;
                case CurrencyType.Range:
                    strsql = "SELECT     currencyrangeid, currencyid, enddate, startdate, exchangerate FROM dbo.currencyranges";
                    SqlCacheDependency rangedep = expdata.CreateSQLCacheDependency(strsql, parameters);
                    strsql = "SELECT     currencyrangeid, tocurrencyid, exchangerate FROM dbo.range_exchangerates";
                    SqlCacheDependency rangeratesdep = expdata.CreateSQLCacheDependency(strsql, parameters);
                    aggdep.Add(new CacheDependency[] { rangedep, rangeratesdep });
                    break;  
            }
            
            
            
            

            strsql = "select currencyid, globalcurrencyid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.currencies";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                globalcurrencyid = reader.GetInt32(reader.GetOrdinal("globalcurrencyid"));
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                switch (currencytype)
                {
                    case CurrencyType.Static:
                        reqcurrency = new cStaticCurrency(accountid, currencyid, globalcurrencyid, createdon, createdby, getStaticExchangerates(currencyid));
                        break;
                    case CurrencyType.Monthly:
                        reqcurrency = new cMonthlyCurrency(accountid, currencyid, globalcurrencyid,createdon,createdby, getMonthlyExchangeRates(currencyid));
                        break;
                    case CurrencyType.Range:
                        reqcurrency = new cRangeCurrency(accountid, currencyid, globalcurrencyid,createdon,createdby, getRangeExchangeRates(currencyid));
                        break;
                }
                list.Add(currencyid, reqcurrency);
            }
            reader.Close();
            

            Cache.Insert("currencies" + accountid, list, aggdep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30), System.Web.Caching.CacheItemPriority.NotRemovable, null);
            
            return list;
        }
        public string CreateUpdateTable(int currencyid)
        {
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            cCurrency currency;
            string rowclass = "row1";
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            SortedList sorted = sortList();

            output.Append("<table class=datatbl>");
            output.Append("<tr><th>Currency</th><th>Exchange Rate</th></tr>");
            for (int i = 0; i < sorted.Count; i++)
            {
                currency = (cCurrency)sorted.GetByIndex(i);
                if (currency.currencyid != currencyid)
                {
                    output.Append("<tr>");
                    output.Append("<td class=\"" + rowclass + "\">" + clsglobalcurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label + "</td>");
                    output.Append("<td class=\"" + rowclass + "\">");
                    output.Append("<input type=text onblur=\"validateItem('exchangerate" + currency.currencyid + "',1,'Exchange Rate');\" size=5 name=\"exchangerate" + currency.currencyid + "\" id=\"exchangerate" + currency.currencyid + "\">");
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
            }
            output.Append("</table>");
            return output.ToString();
        }



		public cCurrency getCurrencyById(int currencyid)
		{
			return (cCurrency)list[currencyid];
        }

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
            double exchangerate = 0;
            

            cCurrencies clscurrencies;
            clscurrencies = new cCurrencies(accountid);


            cEmployees clsemployees = new cEmployees(accountid);






            switch (clscurrencies.currencytype)
            {
                case CurrencyType.Static:
                    clscurrencies = new cStaticCurrencies(accountid);
                    cStaticCurrency reqstandard = (cStaticCurrency)clscurrencies.getCurrencyById(fromcurrency);
                    exchangerate = reqstandard.getExchangeRate(currencyid);

                    break;
                case CurrencyType.Monthly:
                    clscurrencies = new cMonthlyCurrencies(accountid);
                    cMonthlyCurrency reqmonthly = (cMonthlyCurrency)clscurrencies.getCurrencyById(fromcurrency);

                    exchangerate = reqmonthly.getExchangeRate((byte)date.Month, date.Year, currencyid);
                    break;
                case CurrencyType.Range:
                    clscurrencies = new cRangeCurrencies(accountid);

                    cRangeCurrency reqrange = (cRangeCurrency)clscurrencies.getCurrencyById(fromcurrency);
                    exchangerate = reqrange.getExchangeRate(date, currencyid);
                    break;
            }

            return exchangerate;
        }
        
        

		
		

		
		

		


		
		
		public void changeType(int currencytype)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			
			expdata.sqlexecute.Parameters.AddWithValue("@currencytype",currencytype);
			strsql = "update other set currencytype = @currencytype";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

            
            list.Clear();
            list = CacheList();
			
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

        public virtual string getGrid()
        {
            return "";
        }

		

		public int deleteCurrency(int currencyid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cAuditLog clsaudit = new cAuditLog();
			cCurrency reqcur = getCurrencyById(currencyid);

            int count;
            

            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
            //see if this is the default currency
            strsql = "select count(*) from [other] where basecurrency = @currencyid";
            
            count = expdata.getcount(strsql);

            if (count > 0)
            {
                expdata.sqlexecute.Parameters.Clear();
                return 1;
            }

            //see if anyone has used it on items or claims
            strsql = "select count(*) from savedexpenses where currencyid = @currencyid or basecurrency = @currencyid or globalbasecurrency = @currencyid";
            count = expdata.getcount(strsql);
            if (count > 0)
            {
                expdata.sqlexecute.Parameters.Clear();
                return 2;
            }
            strsql = "select count(*) from claims where currencyid = @currencyid";
            if (count > 0)
            {
                expdata.sqlexecute.Parameters.Clear();
                return 2;
            }

            count = expdata.getcount(strsql);
            //update employees primary currency
            strsql = "update employees set primarycurrency = null where primarycurrency = @currencyid";
            expdata.ExecuteSQL(strsql);

            //exchange rates
            strsql = "delete from static_exchangerates where currencyid = @currencyid or tocurrencyid = @currencyid";
            expdata.ExecuteSQL(strsql);

            strsql = "delete from monthly_exchangerates where currencymonthid in (select currencymonthid from currencymonths where currencyid = @currencyid)";
            expdata.ExecuteSQL(strsql);

            strsql = "delete from monthly_exchangerates where tocurrencyid = @currencyid";
            expdata.ExecuteSQL(strsql);

            strsql = "delete from range_exchangerates where currencyrangeid in (select currencyrangeid from currencyranges where currencyid = @currencyid)";
            expdata.ExecuteSQL(strsql);

            strsql = "delete from range_exchangerates where tocurrencyid = @currencyid";
            expdata.ExecuteSQL(strsql);

			strsql = "delete from currencymonths where currencyid = @currencyid";
			expdata.ExecuteSQL(strsql);

            strsql = "delete from currencyranges where currencyid = @currencyid";
            expdata.ExecuteSQL(strsql);

			strsql = "delete from currencies where currencyid = @currencyid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

            list.Remove(currencyid);
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
			clsaudit.deleteRecord("Currency",clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label);

            return 0;
		}

		

		public System.Web.UI.WebControls.ListItem[] CreateDropDown(int currencyid)
		{
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
			cCurrency reqcur;
			System.Web.UI.WebControls.ListItem[] tempitems = new System.Web.UI.WebControls.ListItem[list.Count];
			System.Collections.SortedList sorted = new System.Collections.SortedList();
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
				tempitems[i] = new System.Web.UI.WebControls.ListItem();
				tempitems[i].Text = clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label;
				tempitems[i].Value = reqcur.currencyid.ToString();
				if (currencyid == reqcur.currencyid)
				{
					tempitems[i].Selected = true;
				}
			}

			return tempitems;
		}

        public List<System.Web.UI.WebControls.ListItem> CreateDropDown()
        {
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            cCurrency currency;
            SortedList sorted = sortList();
            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();

            
            for (int i = 0; i < sorted.Count; i++)
            {
                currency = (cCurrency)sorted.GetByIndex(i);

                items.Add(new System.Web.UI.WebControls.ListItem(clsglobalcurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label, currency.currencyid.ToString()));
            }

            return items;


        }

        private SortedList sortList()
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

        public List<int> getCurrencyIds()
        {
            List<int> ids = new List<int>();
            foreach (int val in list.Keys)
            {
                ids.Add(val);
            }
            return ids;
        }

        #region exchange rates
        public SortedList getStaticExchangerates(int currencyid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            SortedList exchangerates = new SortedList();
            System.Data.SqlClient.SqlDataReader reader;
            string strsql;
            int tocurrencyid;
            double exchangerate;
            strsql = "select * from static_exchangerates where currencyid = @currencyid";
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                tocurrencyid = reader.GetInt32(reader.GetOrdinal("tocurrencyid"));
                exchangerate = reader.GetDouble(reader.GetOrdinal("exchangerate"));
                exchangerates.Add(tocurrencyid, exchangerate);

            }
            reader.Close();
            

            return exchangerates;
        }
        private SortedList getMonthlyExchangeRates(int currencyid)
        {
            SortedList list = new SortedList();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cCurrencyMonth curmonth;
            SqlDataReader reader;
            string strsql;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;

            int currencymonthid, year;
            byte month;
            strsql = "select * from currencymonths where currencyid = @currencyid";
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                currencymonthid = reader.GetInt32(reader.GetOrdinal("currencymonthid"));
                month = reader.GetByte(reader.GetOrdinal("month"));
                year = reader.GetInt16(reader.GetOrdinal("year"));
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }
                curmonth = new cCurrencyMonth(accountid, currencyid, currencymonthid, month, year, createdon, createdby, modifiedon, modifiedby, getExchangeRatesForMonth(currencymonthid));
                list.Add(currencymonthid, curmonth);
            }
            reader.Close();

            return list;
        }
        public SortedList getExchangeRatesForMonth(int currencymonthid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SqlDataReader reader;
            string strsql;
            SortedList exchangerates = new SortedList();

            strsql = "select tocurrencyid, exchangerate from monthly_exchangerates where currencymonthid = @currencymonthid";
            expdata.sqlexecute.Parameters.AddWithValue("@currencymonthid", currencymonthid);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                exchangerates.Add(reader.GetInt32(0), reader.GetDouble(1));
            }
            reader.Close();
            
            return exchangerates;
        }
        protected SortedList getExchangeRatesForRange(int currencyrangeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SqlDataReader reader;
            string strsql;


            SortedList exchangerates = new SortedList();
            strsql = "select tocurrencyid, exchangerate from range_exchangerates where currencyrangeid = @currencyrangeid";
            expdata.sqlexecute.Parameters.AddWithValue("@currencyrangeid", currencyrangeid);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                exchangerates.Add(reader.GetInt32(0), reader.GetDouble(1));
            }
            reader.Close();
            
            return exchangerates;
        }
        private SortedList getRangeExchangeRates(int currencyid)
        {
            SortedList list = new SortedList();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cCurrencyRange currange;
            SqlDataReader reader;
            string strsql;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;

            int currencyrangeid;
            DateTime startdate, enddate;
            strsql = "select * from currencyranges where currencyid = @currencyid";
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                currencyrangeid = reader.GetInt32(reader.GetOrdinal("currencyrangeid"));
                startdate = reader.GetDateTime(reader.GetOrdinal("startdate"));
                enddate = reader.GetDateTime(reader.GetOrdinal("enddate"));
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }
                currange = new cCurrencyRange(accountid, currencyid, currencyrangeid, startdate, enddate, createdon, createdby, modifiedon, modifiedby, getExchangeRatesForRange(currencyrangeid));
                list.Add(currencyrangeid, currange);
            }
            reader.Close();

            return list;
        }
        #endregion

        public int checkCurrencyExists(int globalcurrencyid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            int count = 0;

            strsql = "SELECT count(globalcurrencyid) from currencies WHERE globalcurrencyid = @globalcurrencyid";
            expdata.sqlexecute.Parameters.AddWithValue("@globalcurrencyid", globalcurrencyid);
            count = expdata.getcount(strsql);
            expdata.sqlexecute.Parameters.Clear();

            return count;
        }

    }




   

