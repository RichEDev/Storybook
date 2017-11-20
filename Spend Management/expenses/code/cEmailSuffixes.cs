using System;
using System.Data;
using System.Linq;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    using System.Data.SqlClient;

    /// <summary>
    /// Summary description for cEmailSuffixes
    /// </summary>
    public class cEmailSuffixes
    {
        string strsql;
        int nAccountid = 0;

        DBConnection expdata;
        System.Collections.SortedList list;
        System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpContext.Current.Cache;

        

        public cEmailSuffixes(int accountid)
        {
            nAccountid = accountid;
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            
            InitialiseData();
        }

        public int accountid
        {
            get { return nAccountid; }
        }        

        private void InitialiseData()
        {
            list = (System.Collections.SortedList)Cache["suffixes" + accountid];
            if (list == null)
            {
                list = CacheList();
            }

        }

        private System.Collections.SortedList CacheList()
        {
            System.Collections.SortedList list = new System.Collections.SortedList();
            string suffix;
            int suffixid;
            cEmailSuffix clssuffix;
            strsql = "select suffixid, suffix from dbo.email_suffixes";
            
            expdata.sqlexecute.CommandText = strsql;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("suffixes" + accountid, list, dep, Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes(15));
            }

            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    suffixid = (int)reader.GetInt32(reader.GetOrdinal("suffixid"));
                    suffix = reader.GetString(reader.GetOrdinal("suffix"));
                    clssuffix = new cEmailSuffix(suffixid, accountid, suffix);
                    list.Add(suffixid, clssuffix);
                }

                reader.Close();
            }

            return list;
        }

        private bool alreadyExists(Action action, int suffixid, string suffix)
        {
            cEmailSuffix clssuffix;

            for (int i = 0; i < list.Count; i++)
            {
                clssuffix = (cEmailSuffix)list.GetByIndex(i);
                if (action == Action.Add)
                {
                    if (clssuffix.suffix.ToLower().Trim() == suffix.ToLower().Trim())
                    {
                        return true;
                    }
                }
                else if (action == Action.Edit)
                {
                    if (clssuffix.suffix.ToLower().Trim() == suffix.ToLower().Trim() && clssuffix.suffixid != suffixid)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public ReturnCode addSuffix(string suffix)
        {
            if (alreadyExists(Spend_Management.Action.Add, 0, suffix) == true)
            {
                return ReturnCode.AlreadyExists;
            }

            int suffixid;
            strsql = "insert into email_suffixes (suffix) " +
                "values (@suffix); select @identity = @@identity";
            
            expdata.sqlexecute.Parameters.AddWithValue("@suffix", suffix);
            expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
            expdata.ExecuteSQL(strsql);
            suffixid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            cEmailSuffix clssuffix = new cEmailSuffix(suffixid, accountid, suffix);
            list.Add(suffixid, clssuffix);

            return ReturnCode.OK;
        }

        public ReturnCode updateSuffix(int suffixid, string suffix)
        {
            if (alreadyExists(Spend_Management.Action.Edit, suffixid, suffix) == true)
            {
                return ReturnCode.AlreadyExists;
            }
            
            strsql = "update email_suffixes set suffix = @suffix where suffixid = @suffixid ";                
            
            expdata.sqlexecute.Parameters.AddWithValue("@suffix", suffix);
            expdata.sqlexecute.Parameters.AddWithValue("@suffixid", suffixid);
            
            expdata.ExecuteSQL(strsql);
            
            expdata.sqlexecute.Parameters.Clear();

            cEmailSuffix clssuffix = new cEmailSuffix(suffixid, accountid, suffix);
            if (list[suffixid] != null)
            {
                list[suffixid] = clssuffix;
            }
            return ReturnCode.OK;
        }

        public void deleteSuffix(int suffixid)
        {
            strsql = "delete from email_suffixes where suffixid = @suffixid";
            expdata.sqlexecute.Parameters.AddWithValue("@suffixid", suffixid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            list.Remove(suffixid);
        }

        public DataSet getGrid()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("suffixid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("suffix", System.Type.GetType("System.String"));

            cEmailSuffix clssuffix;
            object[] values;

            for (int i = 0; i < list.Count; i++)
            {
                clssuffix = (cEmailSuffix)list.GetByIndex(i);
                values = new object[2];
                values[0] = clssuffix.suffixid;
                values[1] = clssuffix.suffix;
                tbl.Rows.Add(values);
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(tbl);
            return ds;
        }

        public cEmailSuffix getSuffixById(int suffixid)
        {
            return (cEmailSuffix)list[suffixid];
        }

		public cEmailSuffix getSuffixByValue(string suffix)
		{
			return list.Values.Cast<cEmailSuffix>().FirstOrDefault(x => x.suffix == suffix);
		}
    }

    public class cEmailSuffix
    {
        private int nSuffixid;
        private int nAccountid;
        private string sSuffix;

        public cEmailSuffix(int suffixid, int accountid, string suffix)
        {
            nSuffixid = suffixid;
            nAccountid = accountid;
            sSuffix = suffix;
        }

        #region properties
        public int suffixid
        {
            get { return nSuffixid; }
        }
        public int accountid
        {
            get { return nAccountid; }
        }
        public string suffix
        {
            get { return sSuffix; }
        }
        #endregion
    }
}
