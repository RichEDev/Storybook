using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Threading;
using System.Globalization;
using SpendManagementLibrary.Employees;


namespace Spend_Management
{
    using Utilities.DistributedCaching;

    public class cLocales
    {
        private const string CacheAreaKey = "locales";
        public Cache Cache;
        SortedList<int, cLocale> lst;
        public cLocales()
        {
            Cache = new Cache();
            InitialiseData();
        }

        private void InitialiseData()
        {
            lst = this.Cache.Get(0, string.Empty, CacheAreaKey) as SortedList<int, cLocale>;
            if (lst == null)
            {
                lst = CacheList();
            }
        }

        private SortedList<int, cLocale> CacheList()
        {

            SortedList<int, cLocale> locales = new SortedList<int, cLocale>();
            DBConnection data = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            SqlDataReader reader;
            int localeID;
            string localeName, localeCode;
            bool active;

            string strsql = "select localeID, localeName, localeCode, active from dbo.locales";
            data.sqlexecute.CommandText = strsql;
            using (reader = data.GetReader(strsql))
            {
                while (reader.Read())
                {
                    localeID = reader.GetInt32(reader.GetOrdinal("localeID"));
                    localeName = reader.GetString(reader.GetOrdinal("localeName"));
                    localeCode = reader.GetString(reader.GetOrdinal("localeCode"));
                    active = reader.GetBoolean(reader.GetOrdinal("active"));
                    locales.Add(localeID, new cLocale(localeID, localeName, localeCode, active));
                }
                reader.Close();
            }
            Cache.Add(0, string.Empty, CacheAreaKey, locales);

            return locales;
        }

        /// <summary>
        /// Gets all the locales from the Metabase.
        /// Used currently from the API.
        /// </summary>
        /// <returns>A list of Locales</returns>
        public List<cLocale> GetAllLocales()
        {
            return lst.Values.ToList();
        }

        public List<ListItem> CreateActiveDropDown()
        {
            SortedList<string, cLocale> sorted = sortList();
            List<ListItem> items = new List<ListItem>();
            foreach (cLocale locale in sorted.Values)
            {
                if (locale.Active)
                {
                    items.Add(new ListItem(locale.LocaleName + " (" + locale.LocaleCode + ")", locale.LocaleID.ToString()));
                }
            }
            return items;
        }

        private SortedList<string, cLocale> sortList()
        {
            SortedList<string, cLocale> sorted = new SortedList<string, cLocale>();

            foreach (cLocale locale in lst.Values)
            {
                sorted.Add(locale.LocaleName, locale);
            }
            return sorted;
        }

        public virtual cLocale getLocaleByID(int id)
        {
            cLocale locale = null;
            lst.TryGetValue(id, out locale);
            return locale;
        }

        public static void setLocale()
        {
            //set culture for current user

            CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsemployees = new cEmployees(user.AccountID);
            Employee emp = clsemployees.GetEmployeeById(user.EmployeeID);
            int? localeid = null;
            if (emp.LocaleID != null)
            {
                localeid = (int)emp.LocaleID;
            }
            else
            {
                cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
                cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
                if (reqProperties.GlobalLocaleID > 0)
                {
                    localeid = reqProperties.GlobalLocaleID;
                }
            }

            if (localeid != null)
            {
                cLocales clslocales = new cLocales();
                cLocale locale = clslocales.getLocaleByID((int)localeid);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(locale.LocaleCode);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale.LocaleCode);
            }

        }


    }
}
