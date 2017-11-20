using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;
using Utilities.DistributedCaching;

namespace Spend_Management
{
    using System.Text.RegularExpressions;

    using SpendManagementLibrary.Interfaces;

    public class cGlobalCountries : IGlobalCountries
    {
        Dictionary<int, cGlobalCountry> list;
        DBConnection expdata;
        string strsql;
        
        public cGlobalCountries()
        {
            expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            InitialiseData();
        }

        private const string CacheArea = "globalcountries";

        private void InitialiseData()
        {
            var cache = new Cache();
            list = cache.Get(0, string.Empty, CacheArea) as Dictionary<int, cGlobalCountry>;
            if (list == null)
            {
                list = GetList();
                cache.Add(0, string.Empty, CacheArea, list);
            }
        }

        public Dictionary<int, cGlobalCountry> GetList()
        {
            var countries = new Dictionary<int, cGlobalCountry>();
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select globalcountryid, country, countrycode, createdon, modifiedon, postcodeRegexFormat, alpha3CountryCode, numeric3Code, postcodeAnywhereEnabled from dbo.global_countries";
            expdata.sqlexecute.CommandText = strsql;
            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    int globalcountryid = (int) reader["globalcountryid"];
                    string country = (string) reader["country"];
                    string countrycode = (string)reader["countrycode"];

                    DateTime createdon = reader.GetValueOrDefault("createdon", new DateTime(1900, 01, 01));
                    DateTime modifiedon = reader.GetValueOrDefault("modifiedon", new DateTime(1900, 01, 01));

                    string postcodeRegexFormat = reader.GetValueOrDefault("postcodeRegexFormat", string.Empty);
                    string alpha3CountryCode = reader.GetValueOrDefault("alpha3CountryCode", string.Empty);
                    int numeric3Code = (int) reader["numeric3Code"];
                    var postcodeAnywhereEnabled = (bool)reader["postcodeAnywhereEnabled"];

                    countries.Add(globalcountryid, new cGlobalCountry(globalcountryid, country, countrycode, createdon, modifiedon, postcodeRegexFormat, alpha3CountryCode, numeric3Code, postcodeAnywhereEnabled));
                }
                reader.Close();
            }
            return countries;
        }

        /// <summary>
        /// Returns a global country by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                if (country.Country.ToLower() == name.ToLower().Trim())
                {
                    return country;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the global country with the specified ISO 3166-1 alpha-3 code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public cGlobalCountry GetGlobalCountryByAlpha3Code(string code)
        {
            return list.Values.FirstOrDefault(country => country.Alpha3CountryCode.ToLower() == code.ToLower().Trim());
        }

        /// <summary>
        /// Returns the global country with the specified ISO 3166-1 alpha-3 code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public cGlobalCountry GetGlobalCountryByNumeric3Code(int code)
        {
            return list.Values.FirstOrDefault(country => country.Numeric3CountryCode == code);
        }

        public cGlobalCountry getGlobalCountryByAlphaCode(string code)
        {
            code = code.ToLower().Trim();
            return list.Values.FirstOrDefault(country => country.CountryCode.ToLower() == code);
        }



        /// <summary>
        /// Returns the global country Id for the specified ISO 3166-1 alpha-3 code.
        /// </summary>
        /// <param name="code">The alpha-3 code to search for</param>
        /// <returns>The global country Id for the matching GlobalCountry object</returns>
        public int GetGlobalCountryIdByAlpha3Code(string code)
        {
            code = code.Trim();
            return (from country in this.list.Values
                    where string.Compare(country.Alpha3CountryCode, code, StringComparison.OrdinalIgnoreCase) == 0
                    select country.GlobalCountryId).FirstOrDefault();
        }

        private SortedList<string, cGlobalCountry> sortList()
        {
            SortedList<string, cGlobalCountry> sorted = new SortedList<string, cGlobalCountry>();

            foreach (cGlobalCountry country in list.Values)
            {
                sorted.Add(country.Country, country);
            }
            return sorted;
        }

        public List<ListItem> CreateDropDown()
        {
            SortedList<string, cGlobalCountry> sorted = sortList();

            List<ListItem> items = new List<ListItem>();

            foreach (cGlobalCountry country in sorted.Values)
            {
                items.Add(new ListItem(country.Country, country.GlobalCountryId.ToString(CultureInfo.InvariantCulture)));
            }

            return items;
        }

        /// <summary>
        /// Gets a list of the Alpha 3 ISO country codes 
        /// </summary>
        /// <returns>A list of the Alpha 3 ISO country codes</returns>
        public List<ListItem> GetAlpha3List()
        {
            SortedList<string, cGlobalCountry> sorted = sortList();

            List<ListItem> items = new List<ListItem>();

            foreach (cGlobalCountry country in sorted.Values)
            {
                items.Add(new ListItem(country.Country, country.Alpha3CountryCode));
            }

            return items;
        }

        public Dictionary<int, cGlobalCountry> getModifiedGlobalCountries(DateTime date)
        {
            Dictionary<int, cGlobalCountry> lst = new Dictionary<int, cGlobalCountry>();
            foreach (cGlobalCountry val in list.Values)
            {
                if (val.CreatedOn > date || val.ModifiedOn > date)
                {
                    lst.Add(val.GlobalCountryId, val);
                }
            }
            return lst;
        }

        public List<int> getGlobalCountryIds()
        {
            return list.Values.Select(val => val.GlobalCountryId).ToList();
        }

        /// <summary>
        /// Validates a countries postcode against the regex pattern for that country
        /// </summary>
        /// <param name="globalCountryID">The global country</param>
        /// <param name="postcode">The postcode to validate</param>
        /// <returns></returns>
        public bool ValidatePostcode(int globalCountryID, string postcode)
        {
            cGlobalCountry clsGlobalCountry = this.getGlobalCountryById(globalCountryID);

            if (clsGlobalCountry != null)
            {
                if (string.IsNullOrWhiteSpace(clsGlobalCountry.PostcodeRegex) || string.IsNullOrWhiteSpace(postcode))
                {
                    return true;
                }

                return Regex.IsMatch(postcode, clsGlobalCountry.PostcodeRegex, RegexOptions.IgnoreCase);
            }

            return true;
        }
    }
}
