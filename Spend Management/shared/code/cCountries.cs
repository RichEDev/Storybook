namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Utilities.DistributedCaching;

    /// <summary>
    ///     Summary description for cCountries.
    /// </summary>
    public class cCountries
    {
        private const string CacheArea = "countries";

        private readonly int _accountid;

        private readonly int? _subAccountId;

        private Dictionary<int, cCountry> _countries;

        public cCountries(int accountId, int? subAccountId, IDBConnection connection = null)
        {
            this._accountid = accountId;
            this._subAccountId = subAccountId;

            this.InitialiseData(connection);
        }

        public int? SubAccountID
        {
            get
            {
                return this._subAccountId;
            }
        }

        public int accountid
        {
            get
            {
                return this._accountid;
            }
        }

        public Dictionary<int, cCountry> list
        {
            get
            {
                return this._countries;
            }
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Populate ListItem's of the countries dropdown
        /// </summary>
        /// <returns>Collection of countries ListItem's</returns>
        public List<ListItem> CreateDropDown()
        {
            SortedList<string, cCountry> sorted = this.sortList();
            var items = new List<ListItem>();

            items.Add(new ListItem("", "0"));

            foreach (KeyValuePair<string, cCountry> kp in sorted)
            {
                items.Add(new ListItem(kp.Key, kp.Value.CountryId.ToString(CultureInfo.InvariantCulture)));
            }

            return items;
        }

        /// <summary>
        ///     Get the sql to populate the data source for the countries grid on admincountries.aspx
        /// </summary>
        /// <returns>Sql string value</returns>
        public string GetGrid()
        {
            return
                "SELECT countries.countryid, countries.archived, global_countries.country, countries.subAccountId, global_countries.PostcodeAnywhereEnabled FROM countries";
        }

        /// <summary>
        ///     Add all countries and their relevant Vat rates to Cache. This will get called on the instantiation of this class if
        ///     the
        ///     SQL Cache Dependency has been invalidated.
        /// </summary>
        /// <returns>A list of countries referenced in cache</returns>
        public Dictionary<int, cCountry> GetList(IDBConnection connection = null)
        {
            Dictionary<int, cCountry> returnedList;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                returnedList = new Dictionary<int, cCountry>();
                var lstAllVatRates = this.getForeignVATRates(connection);
                string strsql =
                    "SELECT countryid, globalcountryid, archived, CreatedOn, CreatedBy, subAccountId FROM dbo.countries";
                if (this.SubAccountID.HasValue)
                {
                    strsql += " WHERE SubAccountId = @SubAccountId";
                    expdata.sqlexecute.Parameters.AddWithValue("@SubAccountId", this.SubAccountID.Value);
                }

                using (var reader = expdata.GetReader(strsql))
                {
                    while (reader.Read())
                    {
                        var countryid = reader.GetInt32(reader.GetOrdinal("countryid"));
                        var globalcountryid = reader.GetInt32(reader.GetOrdinal("globalcountryid"));
                        var archived = reader.GetBoolean(reader.GetOrdinal("archived"));
                        var vatRates =
                            lstAllVatRates.Where(kp => kp.Value.CountryId == countryid)
                                .ToDictionary(kp => kp.Key, kp => kp.Value);
                        var createdon = reader.GetValueOrDefault("createdon", new DateTime(1900, 01, 01));
                        var createdby = reader.GetValueOrDefault("createdby", 0);
                        var reqcountry = new cCountry(
                            countryid,
                            globalcountryid,
                            archived,
                            vatRates,
                            createdon,
                            createdby);
                        returnedList.Add(countryid, reqcountry);
                    }
                    reader.Close();
                }

                expdata.sqlexecute.Parameters.Clear();
            }
            return returnedList;
        }

        /// <summary>
        ///     Gets a list item for use in a ddlist
        /// </summary>
        /// <param name="countryid">ID of country to generate item for</param>
        /// <returns>ListItem control for use in ddlist</returns>
        public ListItem GetListItem(int countryid)
        {
            var clsglobalcountries = new cGlobalCountries();
            if (this.list.ContainsKey(countryid))
            {
                var item =
                    new ListItem(
                        clsglobalcountries.getGlobalCountryById(this.list[countryid].GlobalCountryId).Country,
                        countryid.ToString(CultureInfo.InvariantCulture));

                return item;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Gets a filtered list of countries where PostcodeAnywhereEnabled is true
        /// </summary>
        /// <returns>A List of cGlobalCountry</returns>
        public List<cGlobalCountry> GetPostcodeAnywhereEnabledCountries()
        {
            SortedList<string, cCountry> sortedCountries = this.sortList();
            var filteredGlobalCountries = new List<cGlobalCountry>();

            var account = new cAccounts().GetAccountByID(this.accountid);
            var globalCountries = new cGlobalCountries();

            if (account.AddressInternationalLookupsAndCoordinates)
            {
                // if the account is allowed to perform international lookups then collect a list of all countries they have activated that are PostcodeAnywhereEnabled
                foreach (cCountry country in sortedCountries.Values)
                {
                    var globalCountry = globalCountries.getGlobalCountryById(country.GlobalCountryId);
                    if (globalCountry.PostcodeAnywhereEnabled)
                    {
                        filteredGlobalCountries.Add(globalCountry);
                    }
                }
            }
            else
            {
                // otherwise just return UK
                var globalCountry = globalCountries.GetGlobalCountryByAlpha3Code("GBR");
                filteredGlobalCountries.Add(globalCountry);
            }

            return filteredGlobalCountries;
        }

        /// <summary>
        ///     Checks the cache to see if has been invalidated
        /// </summary>
        /// <param name="connection"></param>
        public void InitialiseData(IDBConnection connection = null)
        {
            var cache = new Cache();
            this._countries =
                cache.Get(
                    this.accountid,
                    this.SubAccountID.HasValue ? this.SubAccountID.Value.ToString() : string.Empty,
                    CacheArea) as Dictionary<int, cCountry>;
            if (this._countries == null)
            {
                this._countries = this.GetList(connection);
                cache.Add(
                    this.accountid,
                    this.SubAccountID.HasValue ? this.SubAccountID.Value.ToString() : string.Empty,
                    CacheArea,
                    this._countries);
            }
        }

        /// <summary>
        ///     Set the archived status of a country in the database, by calling a stored procedure
        /// </summary>
        /// <param name="countryid">Unique ID of the country</param>
        /// <param name="archive">Flag to set archived status</param>
        /// <param name="connection">The database connection</param>
        /// <returns>Value to determine the success of setting the status of the country</returns>
        public int changeStatus(int countryid, bool archive, IDBConnection connection = null)
        {
            int returncode;
            using (
                IDBConnection expdata = connection
                                        ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", countryid);
                expdata.sqlexecute.Parameters.AddWithValue("@archive", Convert.ToByte(archive));
                CurrentUser currentUser = cMisc.GetCurrentUser();
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.Add("@returncode", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@returncode"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("changeCountryStatus");
                returncode = (int)expdata.sqlexecute.Parameters["@returncode"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }
            if (returncode == 0)
            {
                this.DeleteListFromCache();
            }

            return 0;
        }

        /// <summary>
        ///     Delete country from the database. A check is made to make sure no expense items have the country associated or that
        ///     the country is set as the primary country for an emolyee, before deletion
        /// </summary>
        /// <param name="countryid">ID of the country</param>
        /// <param name="connection">The database connection</param>
        /// <returns>An int value notifying of the success of the delete</returns>
        public int deleteCountry(int countryid, IDBConnection connection = null)
        {
            int returncode;
            using (
                IDBConnection expdata = connection
                                        ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                cCountry country = this.getCountryById(countryid);
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", countryid);
                expdata.sqlexecute.Parameters.AddWithValue("@globalcountryid", country.GlobalCountryId);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", country.CreatedBy);
                CurrentUser currentUser = cMisc.GetCurrentUser();
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.Add("@returncode", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@returncode"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("deleteCountry");
                returncode = (int)expdata.sqlexecute.Parameters["@returncode"].Value;

                expdata.sqlexecute.Parameters.Clear();
            }
            if (returncode > 0)
            {
                return returncode;
            }

            this.list.Remove(countryid);
            this.DeleteListFromCache();
            return 0;
        }

        /// <summary>
        ///     Delete all foreign VAT rates associated with a country from the database
        /// </summary>
        /// <param name="countrysubcatid">ID of the country</param>
        /// <param name="connection">The database connection</param>
        public void deleteVatRate(int countrysubcatid, IDBConnection connection = null)
        {
            using (
                IDBConnection expdata = connection
                                        ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countrysubcatid", countrysubcatid);
                const string strsql = "delete from countrysubcats where countrysubcatid = @countrysubcatid";
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
            this.DeleteListFromCache();
        }

        /// <summary>
        ///     Gets a string array of country names for the quick entry forms
        /// </summary>
        /// <returns>String array of countries</returns>
        public string[] getArray()
        {
            SortedList<string, cCountry> sortedLst = this.sortList();

            var countries = new string[sortedLst.Count];
            int i = 0;

            foreach (string strCountry in sortedLst.Keys)
            {
                countries[i] = strCountry;
                i++;
            }
            return countries;
        }

        /// <summary>
        ///     Retrieves a country class entity by it country code
        ///     If the code has a length of 3, it will match against the Alpha3Code otherwise
        ///     it will match the countrycode.
        /// </summary>
        /// <param name="code">Country code to retrieve record for</param>
        /// <returns>The country object matching the code or null</returns>
        public cCountry getCountryByCode(string code)
        {
            var clsglobalcountries = new cGlobalCountries();

            var alpha3Type = code.Length == 3;
            int numeric3Country = 0;
            int.TryParse(code, out numeric3Country);
            foreach (cCountry tempcountry in this.list.Values)
            {
                var globalcountry = clsglobalcountries.getGlobalCountryById(tempcountry.GlobalCountryId);
                if (globalcountry != null)
                {
                    if (numeric3Country > 0)
                    {
                        if (globalcountry.Numeric3CountryCode == numeric3Country)
                        {
                            return tempcountry;
                        }
                    }
                    else
                    {
                        if (alpha3Type)
                        {
                            if (globalcountry.Alpha3CountryCode.ToLower().Trim() == code.ToLower().Trim())
                            {
                                return tempcountry;
                            }
                        }
                        else
                        {
                            if (globalcountry.CountryCode.ToLower().Trim() == code.ToLower().Trim())
                            {
                                return tempcountry;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Retrieves a country class entity by it country ID
        /// </summary>
        /// <param name="id">Country ID to retrieve record for</param>
        /// <returns></returns>
        public cCountry getCountryByGlobalCountryId(int id)
        {
            foreach (cCountry country in this.list.Values)
            {
                if (country.GlobalCountryId == id)
                {
                    return country;
                }
            }
            return null;
        }

        /// <summary>
        ///     Get a specified country by its ID
        /// </summary>
        /// <param name="countryid"></param>
        /// <returns>The country object requested. If the country cannot be found a null value will be returned</returns>
        public virtual cCountry getCountryById(int countryid)
        {
            cCountry country;
            this.list.TryGetValue(countryid, out country);
            return country;
        }

        public List<int> getCountryIds()
        {
            return this.list.Values.Select(val => val.CountryId).ToList();
        }

        public string getForeignVATGrid()
        {
            return
                "SELECT countrysubcats.countrysubcatid, subcats.subcat, countrysubcats.subcatid, countrysubcats.vat, countrysubcats.vatpercent FROM countrysubcats";
        }

        public Dictionary<int, cCountry> getModifiedCountries(DateTime date)
        {
            var lst = new Dictionary<int, cCountry>();
            foreach (cCountry val in this.list.Values)
            {
                if (val.CreatedOn > date)
                {
                    lst.Add(val.CountryId, val);
                }
            }
            return lst;
        }

        public Dictionary<int, ForeignVatRate[]> getModifiedVatRates(DateTime date, IDBConnection connection = null)
        {
            Dictionary<int, ForeignVatRate[]> lstvatrates;
            using (
                IDBConnection expdata = connection
                                        ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                lstvatrates = new Dictionary<int, ForeignVatRate[]>();
                var lstcountryids = new List<int>();

                expdata.sqlexecute.Parameters.AddWithValue("@date", date);
                var strsql = "select countryid from [countrysubcats]";
                string additionCriteria = "";
                if (this.SubAccountID.HasValue)
                {
                    strsql += " left join countries on countrysubcats.countryid = countries.countryid ";
                    additionCriteria = " and countries.subAccountId = @subAccId";
                    expdata.sqlexecute.Parameters.AddWithValue("@subAccId", this.SubAccountID.Value);
                }
                strsql += " where createdon > @date" + additionCriteria;

                using (var reader = expdata.GetReader(strsql))
                {
                    while (reader.Read())
                    {
                        int countryid = reader.GetInt32(reader.GetOrdinal("countryid"));

                        if (!lstcountryids.Contains(countryid))
                        {
                            lstcountryids.Add(countryid);
                        }
                    }

                    reader.Close();
                }

                foreach (int i in lstcountryids)
                {
                    strsql = "select count(*) from [countrysubcats] where countryid = @countryid;";
                    expdata.sqlexecute.Parameters.AddWithValue("@countryid", i);
                    var count = expdata.ExecuteScalar<int>(strsql);

                    var vatRates = new ForeignVatRate[count];

                    if (count != 0)
                    {
                        int x = 0;
                        strsql = "select subcatid, vat, vatpercent from [countrysubcats] where countryid = @countryid;";
                        using (IDataReader reader = expdata.GetReader(strsql))
                        {
                            while (reader.Read())
                            {
                                var vatRate = new ForeignVatRate();
                                vatRate.CountryId = i;
                                vatRate.SubcatId = reader.GetInt32(reader.GetOrdinal("subcatid"));
                                vatRate.Vat = reader.GetDouble(reader.GetOrdinal("vat"));
                                vatRate.VatPercent = reader.GetDouble(reader.GetOrdinal("vatpercent"));
                                vatRates[x] = vatRate;
                                x++;
                            }

                            reader.Close();
                        }
                        lstvatrates.Add(i, vatRates);
                    }
                    expdata.sqlexecute.Parameters.Clear();
                }
            }
            return lstvatrates;
        }

        /// <summary>
        ///     Save a country and its associated foreign vat rates to the database
        /// </summary>
        /// <param name="country">Country object with the properties to save</param>
        /// <param name="connection">The database connection</param>
        /// <returns>An int value notifying of the success of the save</returns>
        public int saveCountry(cCountry country, IDBConnection connection = null)
        {
            int id;
            using (
                IDBConnection expdata = connection
                                        ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                id = country.CountryId;
                if (country.CountryId == 0)
                {
                    if (this.SubAccountID.HasValue)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", this.SubAccountID.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", DBNull.Value);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@countryid", country.CountryId);
                    expdata.sqlexecute.Parameters.AddWithValue("@globalcountryid", country.GlobalCountryId);
                    expdata.sqlexecute.Parameters.AddWithValue("@date", country.CreatedOn);

                    if (country.CreatedBy.HasValue)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@userid", country.CreatedBy);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@userid", DBNull.Value);
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
                    expdata.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
                    expdata.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
                    expdata.ExecuteProc("saveCountry");
                    id = (int)expdata.sqlexecute.Parameters["@id"].Value;
                    expdata.sqlexecute.Parameters.Clear();

                    if (id == -1)
                    {
                        return -1;
                    }
                }
            }

            this.list[id] = new cCountry(
                id,
                country.GlobalCountryId,
                country.Archived,
                country.VatRates,
                country.CreatedOn,
                country.CreatedBy);
            this.DeleteListFromCache();
            return id;
        }

        /// <summary>
        ///     Insert foreign VAT rate associated with a country into the database
        /// </summary>
        /// <param name="vatRate">Foreign VAT rate</param>
        /// <param name="countrysubcatid">ID of the country</param>
        /// <param name="employeeid">ID of employee</param>
        /// <param name="connection">The database connection</param>
        public void saveVatRate(
            ForeignVatRate vatRate,
            int countrysubcatid,
            int employeeid,
            IDBConnection connection = null)
        {
            using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countrysubcatid", countrysubcatid);
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", vatRate.CountryId);

                expdata.sqlexecute.Parameters.AddWithValue("@subcatid", vatRate.SubcatId);
                expdata.sqlexecute.Parameters.AddWithValue("@vat", vatRate.Vat);
                expdata.sqlexecute.Parameters.AddWithValue("@vatpercent", vatRate.VatPercent);
                expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now.ToUniversalTime());
                expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
                CurrentUser currentUser = cMisc.GetCurrentUser();
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("saveVatRate");
                countrysubcatid = (int)expdata.sqlexecute.Parameters["@id"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }

            cCountry country = this.getCountryById(vatRate.CountryId);

            if (country.VatRates.ContainsKey(countrysubcatid))
            {
                country.VatRates[countrysubcatid] = vatRate;
            }
            else
            {
                country.VatRates.Add(countrysubcatid, vatRate);
            }
            this.DeleteListFromCache();
        }

        /// <summary>
        ///     Sort the countries list into alphabetical order
        /// </summary>
        /// <param name="includeArchived">Optional parameter to include archived countries. Default: false</param>
        /// <returns>Collection of alphabetically sorted countries</returns>
        public SortedList<string, cCountry> sortList(bool includeArchived = false)
        {
            var clsglobalcountries = new cGlobalCountries();
            var sorted = new SortedList<string, cCountry>();

            foreach (cCountry country in this.list.Values)
            {
                if (includeArchived || (!includeArchived && !country.Archived))
                {
                    sorted.Add(clsglobalcountries.getGlobalCountryById(country.GlobalCountryId).Country, country);
                }
            }

            return sorted;
        }

        #endregion

        #region Methods

        private void DeleteListFromCache()
        {
            var cache = new Cache();
            cache.Delete(
                this.accountid,
                this.SubAccountID.HasValue ? this.SubAccountID.Value.ToString() : string.Empty,
                CacheArea);
        }

        /// <summary>
        ///     Get all foreign country VAT rates from the database
        /// </summary>
        /// <returns>A list of all VAT rates</returns>
        private Dictionary<int, ForeignVatRate> getForeignVATRates(IDBConnection connection = null)
        {
            Dictionary<int, ForeignVatRate> vatRates;
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                vatRates = new Dictionary<int, ForeignVatRate>();

                const string strsql =
                    "SELECT countrysubcatid, countryid, subcatid, vat, vatpercent, createdon, createdby FROM dbo.countrysubcats";

                using (var reader = expdata.GetReader(strsql))
                {
                    while (reader.Read())
                    {
                        var foreignVatRate = new ForeignVatRate();
                        int countrySubcatID = reader.GetInt32(reader.GetOrdinal("countrysubcatid"));
                        foreignVatRate.CountryId = reader.GetInt32(reader.GetOrdinal("countryid"));
                        foreignVatRate.SubcatId = reader.GetInt32(reader.GetOrdinal("subcatid"));
                        foreignVatRate.Vat = reader.GetDouble(reader.GetOrdinal("vat"));
                        foreignVatRate.VatPercent = reader.GetDouble(reader.GetOrdinal("vatpercent"));

                        vatRates.Add(countrySubcatID, foreignVatRate);
                    }
                }
            }
            return vatRates;
        }

        #endregion
    }
}