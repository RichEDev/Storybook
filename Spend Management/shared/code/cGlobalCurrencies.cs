using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.Caching;
using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;
using System.Data;
using System.Linq;

namespace Spend_Management
{
    public class cGlobalCurrencies
    {
        private static Lazy<SortedList<int, cGlobalCurrency>> _list;

        public cGlobalCurrencies()
        {
            if (_list == null)
            {
                this.InitialiseData();
            }
        }

        /// <summary>
        /// Gets the current value of the list, retrieving it if it is not already.
        /// </summary>
        private SortedList<int, cGlobalCurrency> list
        {
            get
            {
                return _list.Value;
            }
        }

        private void InitialiseData()
        {
            _list = new Lazy<SortedList<int, cGlobalCurrency>>(GetList);
        }

        private SortedList<int, cGlobalCurrency> GetList()
        {
            IDBConnection connection = null;
            SortedList<int, cGlobalCurrency> currencies = new SortedList<int, cGlobalCurrency>();

            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(
                                                ConfigurationManager.ConnectionStrings["metabase"].ConnectionString))
            {
                string strsql =
                    "select globalcurrencyid, label, alphacode, numericcode, symbol, createdon, modifiedon from dbo.global_currencies";
                using (IDataReader reader = databaseConnection.GetReader(strsql))
                {
                    while (reader.Read())
                    {
                        int globalcurrencyid = (int)reader["globalcurrencyid"];
                        string label = (string)reader["label"];
                        string alphacode = (string)reader["alphacode"];
                        string numericcode = (string)reader["numericcode"];
                        string symbol = reader.GetValueOrDefault("symbol", "");
                        DateTime createdon = reader.GetValueOrDefault("createdon", new DateTime(1900, 01, 01));
                        DateTime modifiedon = reader.GetValueOrDefault("modifiedon", new DateTime(1900, 01, 01));
                        cGlobalCurrency currency = new cGlobalCurrency(
                            globalcurrencyid,
                            label,
                            alphacode,
                            numericcode,
                            symbol,
                            createdon,
                            modifiedon);
                        currencies.Add(globalcurrencyid, currency);
                    }
                }
            }
            return currencies;
        }

        public cGlobalCurrency getGlobalCurrencyById(int id, IDBConnection connection = null)
        {
            cGlobalCurrency currency;
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
            get
            {
                this.InitialiseData();
                return list;
            }
        }

        public cGlobalCurrency getGlobalCurrencyByNumericCode(int code)
        {
            return this.list.Values.FirstOrDefault(currency => Convert.ToInt32(currency.numericcode) == code);
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

        /// <summary>
        /// Used to replace flag descriptions with the actual currency symbol of the expense item
        /// </summary>
        /// <param name="baseCurrency">The base currency of the expense item</param>
        /// <param name="description">The flag description to replace currency symbols for</param>
        /// <returns>The amended flag description</returns>
        public string ReplaceCurrencySymbol(int accountID, cCurrency currency, cGlobalCurrency globalCurrency, string description)
        {

            if (currency == null)
            {
                return description.Replace("{CurrencySymbol}", string.Empty);
            }

            if (globalCurrency == null)
            {
                return description.Replace("{CurrencySymbol}", string.Empty);
            }

            return description.Replace("{CurrencySymbol}", globalCurrency.symbol);
        }
    }
}
