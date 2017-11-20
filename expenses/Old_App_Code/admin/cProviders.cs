using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using SpendManagementLibrary;

namespace expenses.Old_App_Code.admin
{
    public class cProviders
    {
        DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
        string strsql;
        
        SortedList<int, cProvider> list;
        System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

        public cProviders()
        {
            InitialiseData();
        }

        private void InitialiseData()
        {
            list = (SortedList<int, cProvider>)Cache["providers"];
            if (list == null)
            {
                list = CacheList();
            }
        }

        private SortedList<int, cProvider> CacheList()
        {
            int providerid;
            string name;

            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select * from providers";
            SortedList<int, cProvider> lst = new SortedList<int, cProvider>();
            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    providerid = reader.GetInt32(0);
                    name = reader.GetString(1);
                    lst.Add(providerid, new cProvider(providerid, name));
                }
                reader.Close();
            }

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                Cache.Insert("providers", lst, null, Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int) Caching.CacheTimeSpans.Medium),
                    CacheItemPriority.Default, null);
            }
            return list;
        }

        public cProvider getProviderById(int id)
        {
            cProvider prov = null;
            list.TryGetValue(id, out prov);
            return prov;
        }
    }
}
