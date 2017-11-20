using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Web.Caching;

using SpendManagementLibrary;

namespace expenses.Old_App_Code
{
    public class cViewGroups
    {
        private SortedList<Guid, cViewGroup> list;
        public System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

        public cViewGroups()
        {
            InitialiseData();
        }

        private void InitialiseData()
        {
            list = (SortedList<Guid, cViewGroup>)Cache["viewgroups"];
            if (list == null)
            {
                list = CacheList();
            }
        }

        private SortedList<Guid, cViewGroup> CacheList()
        {
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            string strsql;
            SortedList<Guid, cViewGroup> list = new SortedList<Guid, cViewGroup>();
            Guid parentid;
            int level;
			Guid viewgroupid;
            string groupname;
            cViewGroup viewgroup;
            DateTime amendedon;
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select viewgroupid, groupname, parentid, [level], amendedon from dbo.viewgroups_base";
            expdata.sqlexecute.CommandText = strsql;
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("viewgroups", list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Permanent), CacheItemPriority.Default, null);
            }

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    viewgroupid = reader.GetGuid(reader.GetOrdinal("viewgroupid"));
                    groupname = reader.GetString(reader.GetOrdinal("groupname"));
                    if (reader.IsDBNull(reader.GetOrdinal("parentid")) == true)
                    {
                        parentid = Guid.Empty;
                    }
                    else
                    {
                        parentid = reader.GetGuid(reader.GetOrdinal("parentid"));
                    }
                    level = reader.GetInt32(reader.GetOrdinal("level"));
                    if (reader.IsDBNull(reader.GetOrdinal("amendedon")) == true)
                    {
                        amendedon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        amendedon = reader.GetDateTime(reader.GetOrdinal("amendedon"));
                    }
                    viewgroup = new cViewGroup(viewgroupid, groupname, parentid, level, amendedon);
                    list.Add(viewgroupid, viewgroup);
                }
                reader.Close();
            }

            foreach (cViewGroup group in list.Values)
            {
                getChildren(group.viewgroupid, ref list);
            }

            return list;
        }

        private void getChildren(Guid parentid, ref SortedList<Guid,cViewGroup> groups)
        {
            cViewGroup parent;

            SortedList<Guid, cViewGroup> children = new SortedList<Guid, cViewGroup>();
            string strsql;
            DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select viewgroupid from viewgroups_base where parentid = @parentid";
            expdata.sqlexecute.Parameters.AddWithValue("@parentid", parentid);
            using (reader = expdata.GetReader(strsql))
            {
                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    children.Add(reader.GetGuid(0), groups[reader.GetGuid(0)]);
                }
                reader.Close();
            }
            parent = groups[parentid];
            parent.children = children;
        }

        public List<cViewGroup> getParentGroups()
        {
            List<cViewGroup> groups = new List<cViewGroup>();
            foreach (cViewGroup group in list.Values)
            {
                if (group.parentid == Guid.Empty)
                {
                    groups.Add(group);
                }
            }
            return groups;
        }

        public cViewGroup getViewGroupById(Guid viewgroupid)
        {
            cViewGroup viewgroup = null;
            list.TryGetValue(viewgroupid, out viewgroup);
            return viewgroup;
        }
    }

    
}
