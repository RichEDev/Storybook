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
    public class cGlobalCountries
    {
        SortedList<int, cGlobalCountry> list;
        DBConnection expdata;
        string strsql;
        
        public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

        public cGlobalCountries()
        {
            expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
            InitialseData();
        }

        private void InitialseData()
        {
            list = (SortedList<int, cGlobalCountry>)Cache["globalcountries"];
            if (list == null)
            {
                list = CacheList();
            }
        }

        private SortedList<int, cGlobalCountry> CacheList()
        {
            SortedList<int, cGlobalCountry> countries = new SortedList<int, cGlobalCountry>();
            cGlobalCountry gcountry;
            DateTime createdon, modifiedon;
            int globalcountryid;
            string country, countrycode;
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select globalcountryid, country, countrycode, createdon, modifiedon from dbo.global_countries";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                globalcountryid = reader.GetInt32(0);
                country = reader.GetString(1);
                countrycode = reader.GetString(2);

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
                
                countries.Add(globalcountryid, new cGlobalCountry(globalcountryid, country, countrycode, createdon, modifiedon));
            }
            reader.Close();

            Cache.Insert("globalcountries", countries, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
            return countries;
        }

        public cGlobalCountry getGlobalCountryById(int id)
        {
            cGlobalCountry country = null;
            list.TryGetValue(id, out country);
            return country;
        }

        public cGlobalCountry getCountryByName(string name)
        {
            foreach (cGlobalCountry country in list.Values)
            {
                if (country.country.ToLower() == name.ToLower().Trim())
                {
                    return country;
                }
            }
            return null;
        }

        public cGlobalCountry getGlobalCountryByAlphaCode(string code)
        {
            foreach (cGlobalCountry country in list.Values)
            {
                if (country.countrycode.ToLower() == code.ToLower().Trim())
                {
                    return country;
                }
            }
            return null;
        }
        private SortedList<string, cGlobalCountry> sortList()
        {
            SortedList<string, cGlobalCountry> sorted = new SortedList<string, cGlobalCountry>();

            foreach (cGlobalCountry country in list.Values)
            {
                sorted.Add(country.country, country);
            }
            return sorted;
        }

        public List<ListItem> CreateDropDown()
        {
            SortedList<string, cGlobalCountry> sorted = sortList();

            List<ListItem> items = new List<ListItem>();

            foreach (cGlobalCountry country in sorted.Values)
            {
                items.Add(new ListItem(country.country, country.globalcountryid.ToString()));
            }

            return items;
        }

        public Dictionary<int, cGlobalCountry> getModifiedGlobalCountries(DateTime date)
        {
            Dictionary<int, cGlobalCountry> lst = new Dictionary<int, cGlobalCountry>();
            foreach (cGlobalCountry val in list.Values)
            {
                if (val.createdon > date || val.modifiedon > date)
                {
                    lst.Add(val.globalcountryid, val);
                }
            }
            return lst;
        }

        public List<int> getGlobalCountryIds()
        {
            List<int> ids = new List<int>();
            foreach (cGlobalCountry val in list.Values)
            {
                ids.Add(val.globalcountryid);
            }
            return ids;
        }
    }
}
