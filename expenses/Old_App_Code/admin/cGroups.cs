using System;
using expenses.Old_App_Code;
using System.Web.Caching;
using ExpensesLibrary;
using System.Collections.Generic;
using System.Collections;
using SpendManagementLibrary;

namespace expenses
{
	/// <summary>
	/// Summary description for groups.
	/// </summary>

	public class cGroups
	{
		string strsql;
		SortedList list;
        public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;


		int nAccountid = 0;

		private int accountid
		{
			get {return nAccountid;}
		}

		public cGroups(int accountid)
		{
			nAccountid = accountid;
			
			InitialiseData();
		}

        public System.Collections.SortedList groupList
        {
            get { return list; }
        }
		

		private void InitialiseData()
		{
            list = (System.Collections.SortedList)Cache["groups" + accountid];
            if (list == null)
            {
                list = CacheList();
            }
			
		}

		private System.Collections.SortedList CacheList()
		{
			string groupname, description;
			System.Collections.SortedList list = new System.Collections.SortedList();
			int groupid;
			System.Data.SqlClient.SqlDataReader reader;
			cGroup reqgroup;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            DateTime createdon, modifiedon;
            int createdby, modifiedby;

            SortedList<string, object> parameters = new SortedList<string, object>();

            AggregateCacheDependency aggdep = new AggregateCacheDependency();

            strsql = "SELECT signoffid, groupid, signofftype, relid, include, amount, notify, stage, holidaytype, holidayid, onholiday, includeid, claimantmail, singlesignoff, sendmail, displaydeclaration, createdon, createdby, modifiedon, modifiedby FROM dbo.signoffs";
            SqlCacheDependency stgdep = expdata.CreateSQLCacheDependency(strsql, parameters);

            strsql = "select groupid, description, groupname, createdon, createdby, modifiedon, modifiedby from dbo.groups order by groupname";
            SqlCacheDependency grpdep = expdata.CreateSQLCacheDependency(strsql, parameters);

            aggdep.Add(new CacheDependency[] { grpdep, stgdep });

            reader = expdata.GetReader(strsql);
			expdata.sqlexecute.Parameters.Clear();
			while (reader.Read())
			{
				groupid = reader.GetInt32(reader.GetOrdinal("groupid"));
				groupname = reader.GetString(reader.GetOrdinal("groupname"));
				if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
				{
					description = reader.GetString(reader.GetOrdinal("description"));
				}
				else
				{
					description = "";
				}
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
				reqgroup = new cGroup(accountid, groupid, groupname, description, cAccounts.getConnectionString(accountid), createdon, createdby, modifiedon, modifiedby, getStages(groupid));
				list.Add(groupid, reqgroup);
			}	
			reader.Close();
			

			Cache.Insert("groups" + accountid,list,aggdep,System.Web.Caching.Cache.NoAbsoluteExpiration,TimeSpan.FromMinutes(60), System.Web.Caching.CacheItemPriority.NotRemovable, null);
			return list;
			

		}

		private SortedList sortGroups()
		{
			int i;
			cGroup reqgroup;
			System.Collections.SortedList sorted = new System.Collections.SortedList();

			for (i = 0; i < list.Count; i++)
			{
				reqgroup = (cGroup)list.GetByIndex(i);
				sorted.Add(reqgroup.groupname, reqgroup);
			}

			return sorted;
		}
		public System.Data.DataSet getGrid()
		{
			System.Collections.SortedList sorted = sortGroups();
			System.Data.DataSet ds = new System.Data.DataSet();
			System.Data.DataTable tbl = new System.Data.DataTable();
			cGroup reqgroup;
			object[] values;
			int i;
			
			tbl.Columns.Add("groupid",System.Type.GetType("System.Int32"));
			tbl.Columns.Add("groupname",System.Type.GetType("System.String"));
			tbl.Columns.Add("description",System.Type.GetType("System.String"));

			for (i = 0; i < sorted.Count; i++)
			{
				reqgroup = (cGroup)sorted.GetByIndex(i);
				values = new object[3];
				values[0] = reqgroup.groupid;
				values[1] = reqgroup.groupname;
				values[2] = reqgroup.description;
				tbl.Rows.Add(values);
			}

			ds.Tables.Add(tbl);
			return ds;
		}

		

		private bool alreadyExists(string groupname, int groupid, int action)
		{
			cGroup reqgroup;
			int i;

			for (i = 0; i < list.Count; i++)
			{
				reqgroup = (cGroup)list.GetByIndex(i);
				if (action == 0) //add
				{
					if (reqgroup.groupname.ToLower().Trim() == groupname.ToLower().Trim())
					{
						return true;
					}
				}
				else 
				{
					if (reqgroup.groupname.ToLower().Trim() == groupname.ToLower().Trim() && reqgroup.groupid != groupid)
					{
						return true;
					}
				}
			}
			return false;
		}
		public int addGroup (string groupname, string description, int userid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			if (alreadyExists(groupname,0,0) == true)
			{
				return -1;
			}

            int groupid;
			expdata.sqlexecute.Parameters.AddWithValue("@groupname",groupname);
			expdata.sqlexecute.Parameters.AddWithValue("@description",description);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
            expdata.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
            strsql = "insert into groups (groupname, description, createdon, createdby) values (@groupname,@description, @createdon, @createdby); select @identity = scope_identity();";
			expdata.ExecuteSQL(strsql);
            groupid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
			expdata.sqlexecute.Parameters.Clear();

            cGroup group = new cGroup(accountid, groupid, groupname, description, "", DateTime.Now.ToUniversalTime(), userid, DateTime.Now.ToUniversalTime(), userid, new SortedList<int,cStage>());
            list.Add(groupid, group);
           
			return groupid;
		}

        public void invalidateCache()
        {
            Cache.Remove("groups" + accountid);
            InitialiseData();

        }
		

		public int updateGroup(int groupid, string groupname, string description, int userid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			if (alreadyExists(groupname,groupid,2) == true)
			{
				return 1;
			}
			expdata.sqlexecute.Parameters.AddWithValue("@groupname",groupname);
			expdata.sqlexecute.Parameters.AddWithValue("@description",description);
			expdata.sqlexecute.Parameters.AddWithValue("@groupid",groupid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
            strsql = "update groups set groupname = @groupname, description = @description, modifiedon = @modifiedon, modifiedby = @modifiedby where groupid = @groupid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			
			InitialiseData();
			return 0;
		}

		public int deleteGroup(int groupid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int count = 0;
			cGroup reqgroup = GetGroupById(groupid);
			expdata.sqlexecute.Parameters.AddWithValue("@groupid",groupid);
			strsql = "select count(*) from employees where groupid = @groupid";
			count = expdata.getcount(strsql);
			expdata.sqlexecute.Parameters.Clear();
			if (count != 0)
			{
				return 1;
			}
			
			deleteStages(groupid);
			expdata.sqlexecute.Parameters.AddWithValue("@groupid",groupid);
			strsql = "delete from groups where groupid = @groupid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			list.Remove(groupid);

			
			return 0;
		}

		public cGroup GetGroupById(int groupid)
		{
			return (cGroup)list[groupid];
		}

        public cGroup getGroupByName(string name)
        {
            cGroup reqGroup = null;
            foreach (cGroup clsGroup in list.Values)
            {
                if (clsGroup.groupname == name)
                {
                    reqGroup = clsGroup;
                    break;
                }
            }
            return reqGroup;
        }

		private System.Collections.SortedList sortList()
		{
			System.Collections.SortedList orderedlst = new System.Collections.SortedList();
			cGroup reqgroup;
			int i;
			for (i = 0 ; i < list.Count; i++)
			{
				reqgroup = (cGroup)list.GetByIndex(i);
				orderedlst.Add(reqgroup.groupname,reqgroup);
			}
			return orderedlst;
		}
		public System.Web.UI.WebControls.ListItem[] CreateDropDown(int groupid)
		{
			System.Collections.SortedList orderedlst = sortList();
			System.Web.UI.WebControls.ListItem[] tempItems = new System.Web.UI.WebControls.ListItem[list.Count + 1];
			cGroup reqgroup;
			int i = 0;
			tempItems[0] = new System.Web.UI.WebControls.ListItem();
			tempItems[0].Text = "";
			tempItems[0].Value = "";
			for (i = 0; i < list.Count; i++)
			{
				reqgroup = (cGroup)orderedlst.GetByIndex(i);
				tempItems[i + 1] = new System.Web.UI.WebControls.ListItem();
				tempItems[i + 1].Text = reqgroup.groupname;;
				tempItems[i + 1].Value = reqgroup.groupid.ToString();
				if (groupid == reqgroup.groupid)
				{
					tempItems[i + 1].Selected = true;
				}
			}
			return tempItems;
		}

        public sOnlineSignoffGroupInfo getModifiedSignoffGroups(DateTime date)
        {
            sOnlineSignoffGroupInfo groupinfo = new sOnlineSignoffGroupInfo();
            Dictionary<int, cGroup> lstgroups = new Dictionary<int, cGroup>();
            List<int> lstgroupids = new List<int>();
            Dictionary<int, List<cStage>> lststages = new Dictionary<int, List<cStage>>();
            List<cStage> stages;
            List<int> lststageids = new List<int>();

            foreach (cGroup group in list.Values)
            {
                if (group.createdon > date || group.modifiedon > date)
                {
                    lstgroups.Add(group.groupid, group);
                }

                lstgroupids.Add(group.groupid);

                stages = new List<cStage>();

                foreach (cStage stage in group.stages.Values)
                {
                    if (stage.createdon > date || stage.modifiedon > date)
                    {
                        stages.Add(stage);
                    }

                    lststageids.Add(stage.signoffid);
                }

                if (stages.Count > 0)
                {
                    lststages.Add(group.groupid, stages);
                }
            }

            groupinfo.lstonlinegroups = lstgroups;
            groupinfo.lstgroupids = lstgroupids;
            groupinfo.lstonlinestages = lststages;
            groupinfo.lststageids = lststageids;

            return groupinfo;
        }

        #region stage functions
        private SortedList<int, cStage> getStages(int groupid)
        {
            cStage reqstage;
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            decimal amount;
            int holidayid, holidaytype, notify, relid, signoffid, includeid;
            byte onholiday, signofftype, stage;
            bool claimantmail, singlesignoff, sendmail, displaydeclaration;
            StageInclusionType include;
            System.Data.SqlClient.SqlDataReader reader;
            SortedList<int, cStage> list = new SortedList<int, cStage>();
            DateTime createdon, modifiedon;
            int createdby, modifiedby;

            expdata.sqlexecute.Parameters.AddWithValue("@groupid", groupid);
            strsql = "select * from signoffs where groupid = @groupid";
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                amount = reader.GetDecimal(reader.GetOrdinal("amount"));

                holidayid = reader.GetInt32(reader.GetOrdinal("holidayid"));
                holidaytype = reader.GetInt32(reader.GetOrdinal("holidaytype"));
                include = (StageInclusionType)reader.GetInt32(reader.GetOrdinal("include"));
                notify = reader.GetInt32(reader.GetOrdinal("notify"));
                onholiday = reader.GetByte(reader.GetOrdinal("onholiday"));
                relid = reader.GetInt32(reader.GetOrdinal("relid"));
                signoffid = reader.GetInt32(reader.GetOrdinal("signoffid"));
                signofftype = reader.GetByte(reader.GetOrdinal("signofftype"));
                stage = reader.GetByte(reader.GetOrdinal("stage"));
                claimantmail = reader.GetBoolean(reader.GetOrdinal("claimantmail"));
                if (reader.IsDBNull(reader.GetOrdinal("includeid")) == false)
                {
                    includeid = reader.GetInt32(reader.GetOrdinal("includeid"));
                }
                else
                {
                    includeid = 0;
                }
                singlesignoff = reader.GetBoolean(reader.GetOrdinal("singlesignoff"));
                sendmail = reader.GetBoolean(reader.GetOrdinal("sendmail"));
                displaydeclaration = reader.GetBoolean(reader.GetOrdinal("displaydeclaration"));
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
                reqstage = new cStage(signoffid, signofftype, relid, include, amount, notify, stage, onholiday, holidaytype, holidayid, includeid, claimantmail, singlesignoff, sendmail, displaydeclaration, createdon, createdby, modifiedon, modifiedby);
                list.Add(signoffid, reqstage);
            }
            reader.Close();


            return list;
        }


        public SortedList sortStages(cGroup grp)
        {
            SortedList sorted = new SortedList();

            foreach (cStage stage in grp.stages.Values)
            {
                sorted.Add(stage.stage, stage);
            }
            return sorted;
        }

        public System.Data.DataSet getStagesGrid(cGroup grp)
        {
            SortedList sorted = sortStages(grp);
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();
            cStage reqstage;
            object[] values;
            int i;

            tbl.Columns.Add("signoffid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("stage", System.Type.GetType("System.Byte"));
            tbl.Columns.Add("signofftype", System.Type.GetType("System.Byte"));
            tbl.Columns.Add("relid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("include", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("notify", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("amount", System.Type.GetType("System.Decimal"));

            for (i = 0; i < sorted.Count; i++)
            {
                reqstage = (cStage)sorted.GetByIndex(i);
                values = new object[7];
                values[0] = reqstage.signoffid;
                values[1] = reqstage.stage;
                values[2] = reqstage.signofftype;
                values[3] = reqstage.relid;
                values[4] = reqstage.include;
                values[5] = reqstage.notify;
                values[6] = reqstage.amount;
                tbl.Rows.Add(values);
            }
            ds.Tables.Add(tbl);
            return ds;

        }
        public int checkLastStage(cGroup grp)
        {

            SortedList sorted = sortStages(grp);
            cStage reqstage;

            if (sorted.Count == 0)
            {
                return 0;
            }
            reqstage = (cStage)sorted.GetByIndex(sorted.Count - 1);

            if (reqstage.include != StageInclusionType.Always)
            {
                return 1;
            }
            if (reqstage.notify != 2)
            {
                return 2;
            }
            if (reqstage.onholiday == 2)
            {
                return 3;
            }

            return 0;

        }



        public void deleteStages(int groupid)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@groupid", groupid);
            strsql = "delete from signoffs where groupid = @groupid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public int updateStage(int signoffid, int signofftype, int relid, int include, decimal amount, int notify, int onholiday, int holidaytype, int holidayid, int includeid, bool claimantmail, bool singlesignoff, bool sendmail, bool displaydeclaration, int userid)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@signoffid", signoffid);
            expdata.sqlexecute.Parameters.AddWithValue("@signofftype", signofftype);
            expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
            expdata.sqlexecute.Parameters.AddWithValue("@include", include);
            expdata.sqlexecute.Parameters.AddWithValue("@amount", amount);
            expdata.sqlexecute.Parameters.AddWithValue("@notify", notify);
            expdata.sqlexecute.Parameters.AddWithValue("@onholiday", onholiday);
            expdata.sqlexecute.Parameters.AddWithValue("@holidaytype", holidaytype);
            expdata.sqlexecute.Parameters.AddWithValue("@holidayid", holidayid);
            expdata.sqlexecute.Parameters.AddWithValue("@claimantmail", Convert.ToByte(claimantmail));
            expdata.sqlexecute.Parameters.AddWithValue("@singlesignoff", Convert.ToByte(singlesignoff));
            if (includeid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@includeid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@includeid", includeid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@sendmail", Convert.ToByte(sendmail));
            expdata.sqlexecute.Parameters.AddWithValue("@displaydeclaration", Convert.ToByte(displaydeclaration));
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
            strsql = "update signoffs set signofftype = @signofftype, relid = @relid, include = @include, amount = @amount, notify = @notify, onholiday = @onholiday, holidaytype = @holidaytype, holidayid = @holidayid, includeid = @includeid, claimantmail = @claimantmail, singlesignoff = @singlesignoff, sendmail = @sendmail, displaydeclaration = @displaydeclaration, modifiedon = @modifiedon, modifiedby = @modifiedby";

            strsql += " where signoffid = @signoffid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            return 0;
        }

        public int addStage(int groupid, int signofftype, int relid, int include, decimal amount, int notify, int onholiday, int holidaytype, int holidayid, int includeid, bool claimantmail, bool singlesignoff, bool sendmail, bool displaydeclaration, int userid, int signoffid, bool offline)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            int stage = 0;

            expdata.sqlexecute.Parameters.AddWithValue("@signofftype", signofftype);
            expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
            expdata.sqlexecute.Parameters.AddWithValue("@include", include);
            expdata.sqlexecute.Parameters.AddWithValue("@amount", amount);
            expdata.sqlexecute.Parameters.AddWithValue("@notify", notify);
            expdata.sqlexecute.Parameters.AddWithValue("@onholiday", onholiday);
            expdata.sqlexecute.Parameters.AddWithValue("@holidaytype", holidaytype);
            expdata.sqlexecute.Parameters.AddWithValue("@holidayid", holidayid);
            expdata.sqlexecute.Parameters.AddWithValue("@claimantmail", Convert.ToByte(claimantmail));
            if (includeid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@includeid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@includeid", includeid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@sendmail", Convert.ToByte(sendmail));
            expdata.sqlexecute.Parameters.AddWithValue("@singlesignoff", Convert.ToByte(singlesignoff));
            expdata.sqlexecute.Parameters.AddWithValue("@groupid", groupid);
            expdata.sqlexecute.Parameters.AddWithValue("@displaydeclaration", displaydeclaration);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);

            cGroup grp = GetGroupById(groupid);
            stage = grp.stages.Count;
            stage++;


            expdata.sqlexecute.Parameters.AddWithValue("@stage", stage);
            
            strsql = "insert into signoffs (groupid, signofftype, relid, include, amount, notify, onholiday, holidaytype, holidayid, stage, includeid, claimantmail, singlesignoff, sendmail, displaydeclaration, createdon, createdby) " +
                "values (@groupid,@signofftype,@relid,@include,@amount,@notify,@onholiday,@holidaytype,@holidayid,@stage,@includeid,@claimantmail, @singlesignoff, @sendmail, @displaydeclaration, @createdon, @createdby)";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            return 0;
        }

        public void deleteStage(cGroup grp, int signoffid)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cStage reqstage = grp.stages[signoffid];

            byte stage = 0;

            expdata.sqlexecute.Parameters.AddWithValue("@signoffid", signoffid);

            stage = reqstage.stage;

            strsql = "delete from signoffs where signoffid = @signoffid";
            expdata.ExecuteSQL(strsql);

            //update stages
            updateStageNumbers(stage, grp.groupid);

            expdata.sqlexecute.Parameters.Clear();
        }

        private void updateStageNumbers(int stage, int groupid)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.DataSet rcdststages = new System.Data.DataSet();
            int curstage = 0;
            int signoffid = 0;
            int i = 0;
            curstage = stage;
            expdata.sqlexecute.Parameters.AddWithValue("@groupid", groupid);
            expdata.sqlexecute.Parameters.AddWithValue("@stage", stage);

            strsql = "select signoffid from signoffs where groupid = @groupid and stage > @stage order by stage";
            rcdststages = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            for (i = 0; i < rcdststages.Tables[0].Rows.Count; i++)
            {
                signoffid = (int)rcdststages.Tables[0].Rows[i]["signoffid"];
                strsql = "update signoffs set stage = " + curstage + " where signoffid = " + signoffid;
                expdata.ExecuteSQL(strsql);
                curstage++;
            }

        }
        #endregion
	}


	
   
}
