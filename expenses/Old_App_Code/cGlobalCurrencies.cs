using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using ExpensesLibrary;
using System.Web.Caching;
using SpendManagementLibrary;

namespace expenses.Old_App_Code
{
    public class cGlobalCurrencies
    {
        SortedList<int, cGlobalCurrency> list;
        DBConnection expdata;
        string strsql;

        public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

        public cGlobalCurrencies()
        {
            expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
            InitialiseData();
        }

        private void InitialiseData()
        {
            list = (SortedList<int, cGlobalCurrency>)Cache["globalcurrencies"];
            if (list == null)
            {
                list = CacheList();
            }
        }

        private SortedList<int, cGlobalCurrency> CacheList()
        {
            int globalcurrencyid;
            string label, alphacode, numericcode, symbol;
            DateTime createdon, modifiedon;
            System.Data.SqlClient.SqlDataReader reader;
            SortedList<int, cGlobalCurrency> currencies = new SortedList<int, cGlobalCurrency>();
            cGlobalCurrency currency;
            strsql = "select globalcurrencyid, label, alphacode, numericcode, symbol, createdon, modifiedon from dbo.global_currencies";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                globalcurrencyid = reader.GetInt32(0);
                label = reader.GetString(1);
                alphacode = reader.GetString(2);
                numericcode = reader.GetString(3);
                if (reader.IsDBNull(4))
                {
                    symbol = "";
                }
                else
                {
                    symbol = reader.GetString(4);
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }

                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                }
                currency = new cGlobalCurrency(globalcurrencyid, label, alphacode, numericcode, symbol, createdon, modifiedon);
                currencies.Add(globalcurrencyid, currency);
            }
            reader.Close();

            Cache.Insert("globalcurrencies", currencies, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
            return currencies;
        }

        public cGlobalCurrency getGlobalCurrencyById(int id)
        {
            cGlobalCurrency currency = null;
            list.TryGetValue(id, out currency);
            return currency;
        }

        private SortedList<string, cGlobalCurrency> sortList()
        {
            SortedList<string, cGlobalCurrency> currencies = new SortedList<string, cGlobalCurrency>();

            foreach (cGlobalCurrency currency in list.Values)
            {
                currencies.Add(currency.label, currency);
            }
            return currencies;
        }

        public List<ListItem> CreateDropDown()
        {
            SortedList<string, cGlobalCurrency> sorted = sortList();
            List<ListItem> items = new List<ListItem>();
            foreach (cGlobalCurrency currency in sorted.Values)
            {
                items.Add(new ListItem(currency.label, currency.globalcurrencyid.ToString()));
            }
            return items;
        }

        public Dictionary<int, cGlobalCurrency> getModifiedGlobalCurrencies(DateTime date)
        {
            Dictionary<int, cGlobalCurrency> lst = new Dictionary<int, cGlobalCurrency>();
            foreach (cGlobalCurrency val in list.Values)
            {
                if (val.createdon > date || val.modifiedon > date)
                {
                    lst.Add(val.globalcurrencyid, val);
                }
            }
            return lst;
        }

        public List<int> getGlobalCurrencyIds()
        {
            List<int> ids = new List<int>();
            foreach (cGlobalCurrency val in list.Values)
            {
                ids.Add(val.globalcurrencyid);
            }
            return ids;
        }

        public SortedList<int, cGlobalCurrency> globalcurrencies
        {
            get { return list; }
        }

        public cGlobalCurrency getGlobalCurrencyByNumericCode(string code)
        {
            foreach (cGlobalCurrency currency in list.Values)
            {
                if (currency.numericcode == code)
                {
                    return currency;
                }
            }
            return null;
        }

        public cGlobalCurrency getGlobalCurrencyByAlphaCode(string code)
        {
            foreach (cGlobalCurrency currency in list.Values)
            {
                if (currency.alphacode == code)
                {
                    return currency;
                }
            }
            return null;
        }

        public cGlobalCurrency getGlobalCurrencyByLabel(string label)
        {
            foreach (cGlobalCurrency currency in list.Values)
            {
                if (currency.label.Trim().ToLower() == label.Trim().ToLower())
                {
                    return currency;
                }
            }
            return null;
        }
    }
}
