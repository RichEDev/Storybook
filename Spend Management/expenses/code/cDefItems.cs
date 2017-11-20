using System;
using SpendManagementLibrary;


namespace Spend_Management
{
	/// <summary>
	/// Summary description for cDefItems.
	/// </summary>
	public abstract class cDefItems
	{
		
		public string strsql;
		
		public int accountid;
		public System.Collections.SortedList list;

        public System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
		public cDefItems()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void deleteItem(string table, string idfield, int idvalue)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			strsql = "delete from " + table + " where " + idfield + " = @idvalue";
			
			
			expdata.sqlexecute.Parameters.AddWithValue("@idvalue",idvalue);
			expdata.ExecuteSQL(strsql);

			expdata.sqlexecute.Parameters.Clear();
			InvalidateCache(table,accountid);
		}

		public void addItem (string table, System.Collections.SortedList values)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int i;
			string valuesql = "";

			
			strsql = "insert into [" + table + "] (";
			valuesql = " values (";
			for (i = 0; i < values.Count; i++)
			{
				strsql += "[" + values.GetKey(i) + "],";
				valuesql += "@" + values.GetKey(i) + ",";
				expdata.sqlexecute.Parameters.AddWithValue("@" + values.GetKey(i), values.GetByIndex(i));
			}
			strsql = strsql.Remove(strsql.Length-1,1);
			strsql += ")";
			valuesql = valuesql.Remove(valuesql.Length - 1,1);
			valuesql += ")";

			strsql += valuesql;

			
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			InvalidateCache(table,accountid);
		}
		public void updateItem(string table, string idfield, int idvalue, System.Collections.SortedList values)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int i;
			strsql = "update " + table;
			strsql += " set ";

			for (i = 0; i < values.Count; i++)
			{
				strsql += "[" + values.GetKey(i) + "] = @" + values.GetKey(i) + ",";
				expdata.sqlexecute.Parameters.AddWithValue("@" + values.GetKey(i), values.GetByIndex(i));
			}
			strsql = strsql.Remove(strsql.Length - 1,1);
			strsql += " where " + idfield + " = @idvalue";
			expdata.sqlexecute.Parameters.AddWithValue("@idvalue",idvalue);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			InvalidateCache(table, accountid);
														
		}

		public abstract System.Data.DataTable getGrid();
		public abstract void InitialiseData();
		public abstract System.Collections.SortedList CacheList();
		//public abstract System.Text.StringBuilder CreateStringDropDown();
		public abstract bool alreadyExists(string sValue, bool update, int id);

		public void CreateCacheDependency(string table, int accountid)
		{
		    if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
		    {
		        if (Cache[table + "dependency" + accountid] == null)
		        {
		            Cache.Insert(table + "dependency" + accountid, 1);

		        }
		    }
		}

		public void InvalidateCache(string table, int accountid)
		{
			Cache.Remove(table + "dependency" + accountid);
			CreateCacheDependency(table,accountid);
		}
		
	}

	
}
