using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
	public class cFWEmployees : cFWEmployeesBase
	{
		private Cache Cache = (Cache)System.Web.HttpRuntime.Cache;

		public cFWEmployees(cFWSettings fws, UserInfo uinfo)
			: base(fws, uinfo)
		{
			CreateDependency();

			if (Cache["employees_" + fws.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + "_" + cUInfo.ActiveLocation.ToString()] == null)
			{
				employeelist = CacheItems();
			}
			else
			{
				employeelist = (SortedList<int, cFWEmployee>)Cache["employees_" + fws.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + "_" + cUInfo.ActiveLocation.ToString()];
			}
		}

		private void CreateDependency()
		{
			if (Cache["employeedependency_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + "_" + cUInfo.ActiveLocation.ToString()] == null)
			{
				Cache.Insert("employeedependency_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + "_" + cUInfo.ActiveLocation.ToString(), 1);
			}
		}

		System.Web.Caching.CacheDependency getDependency()
		{
			System.Web.Caching.CacheDependency dep;
			String[] dependency;
			dependency = new string[1];
			dependency[0] = "employeedependency_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + "_" + cUInfo.ActiveLocation.ToString();
			dep = new System.Web.Caching.CacheDependency(null, dependency);
			return dep;
		}

		public void InvalidateCache()
		{
			Cache.Remove("employeedependency_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + "_" + cUInfo.ActiveLocation.ToString());
			CreateDependency();
		}

		private SortedList<int, cFWEmployee> CacheItems()
		{
			SortedList<int, cFWEmployee> list = new SortedList<int, cFWEmployee>();
			cFWDBConnection db = new cFWDBConnection();
			System.Data.SqlClient.SqlDataReader reader;

			db.DBOpen(cFWS, false);

			string sql = "SELECT [Staff Id], [Staff Name], ISNULL([Staff Number],'') AS [Staff Number],ISNULL([Telephone Number],'') AS [Telephone Number], ISNULL([Mobile Number],'') AS [Mobile Number], ISNULL([Fax Number],'') AS [Fax Number], ISNULL([Position],'') AS [Position],ISNULL([Email Address],'') AS [Email Address] FROM [staff_details] WHERE [Location Id] = @locId";
			db.AddDBParam("locId", cUInfo.ActiveLocation, true);

			reader = db.GetReader(sql);
			while (reader.Read())
			{
				int sid = reader.GetInt32(reader.GetOrdinal("Staff Id"));
				string num = reader.GetString(reader.GetOrdinal("Staff Number"));
				string name = reader.GetString(reader.GetOrdinal("Staff Name"));
				string telnum = reader.GetString(reader.GetOrdinal("Telephone Number"));
				string mobile = reader.GetString(reader.GetOrdinal("Mobile Number"));
				string fax = reader.GetString(reader.GetOrdinal("Fax Number"));
				string email = reader.GetString(reader.GetOrdinal("Email Address"));
				string position = reader.GetString(reader.GetOrdinal("Position"));

				cFWEmployee employee = new cFWEmployee(sid, num, name, telnum, mobile, fax, email, position);
				list.Add(sid, employee);
			}
			reader.Close();
			db.DBClose();

			Cache.Insert("employees_" + cFWS.MetabaseAccountKey.Replace(" ", "_") + cFWS.glDatabase.Replace(" ", "_") + "_" + cUInfo.ActiveLocation.ToString(), list, getDependency(), System.Web.Caching.Cache.NoAbsoluteExpiration, System.TimeSpan.FromMinutes(60), CacheItemPriority.NotRemovable, null);
			return list;
		}
	}
}
