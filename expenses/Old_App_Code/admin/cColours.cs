using System;
using expenses.Old_App_Code;
using System.Web.Caching;

namespace expenses
{
	/// <summary>
	/// Summary description for cColours.
	/// </summary>
	public class cColours
	{
		private int nAccountid;

		public const string sMenubarBGColour = "#003768";
		public const string sMenubarFGColour = "#fff";
		public const string sTitlebarBGColour = "#6280a7";
		public const string sTitlebarFGColour = "#ffffff";
		public const string sRowColour = "#fff";
		public const string sAlternateRowColour = "#D2E4EE";
		public const string sFieldBG = "#cae8f4";
		public const string sFieldFG = "#000";
		public const string sHoverColour = "#b6677b";

        public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpContext.Current.Cache;
		private System.Collections.SortedList colours;
		DBConnection expdata;
		string strsql;

		#region defaults
		public string defaultMenubarBgColour
		{
			get {return sMenubarBGColour;}
		}
		public string defaultMenubarFgColour
		{
			get {return sMenubarFGColour;}
		}
		public string defaultTitlebarBGColour
		{
			get {return sTitlebarBGColour;}
		}
		public string defaultRowColour
		{
			get {return sRowColour;}
		}
		public string defaultAltRowColour
		{
			get {return sAlternateRowColour;}
		}
		public string defaultFieldBg
		{
			get {return sFieldBG;}
		}
		public string defaultFieldFG
		{
			get {return sFieldFG;}
		}
		public string defaultHoverColour
		{
			get {return sHoverColour;}
		}
		#endregion
		public cColours(int accountid)
		{
			nAccountid = accountid;
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            colours = (System.Collections.SortedList)Cache["colours" + accountid];
            if (colours == null)
            {
                getColours();
            }
		}

		private void getColours()
		{
            colours = new System.Collections.SortedList();
			string colourcode, colour;
			System.Data.SqlClient.SqlDataReader reader;

			strsql = "SELECT colour, colourcode FROM dbo.colours";
			
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
			reader = expdata.GetReader(strsql);
			expdata.sqlexecute.Parameters.Clear();
			while (reader.Read())
			{
				colourcode = reader.GetString(reader.GetOrdinal("colourcode"));
				colour = reader.GetString(reader.GetOrdinal("colour"));
				colours.Add(colourcode,colour);
			}
			reader.Close();
            Cache.Insert("colours" + accountid, colours, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
		}

		public void restoreDefaults()
		{
			deleteColours();
			getColours();
		}
		public void updateColours(string menubarbgcolour, string menubarfgcolour, string titlebarbgcolour, string titlebarfgcolour, string fieldbgcolour, string fieldfgcolour, string rowbgcolour, string altrowbgcolour, string hovercolour)
		{
			deleteColours();

			strsql = "insert into colours (colourcode, colour) " + 
					"values (@colourcode, @colour)";

			if (menubarbgcolour != sMenubarBGColour)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@colourcode","menubarbgcolour");
				expdata.sqlexecute.Parameters.AddWithValue("@colour",menubarbgcolour);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

			if (menubarfgcolour != sMenubarFGColour)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@colourcode","menubarfgcolour");
				expdata.sqlexecute.Parameters.AddWithValue("@colour",menubarfgcolour);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

			if (titlebarbgcolour != sTitlebarBGColour)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@colourcode","titlebarbgcolour");
				expdata.sqlexecute.Parameters.AddWithValue("@colour",titlebarbgcolour);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

			if (titlebarfgcolour != sTitlebarFGColour)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@colourcode","titlebarfgcolour");
				expdata.sqlexecute.Parameters.AddWithValue("@colour",titlebarfgcolour);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

			if (fieldbgcolour != sFieldBG)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@colourcode","fieldbgcolour");
				expdata.sqlexecute.Parameters.AddWithValue("@colour",fieldbgcolour);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

			if (fieldfgcolour != sFieldFG)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@colourcode","fieldfgcolour");
				expdata.sqlexecute.Parameters.AddWithValue("@colour",fieldfgcolour);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

			if (rowbgcolour != sRowColour)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@colourcode","rowcolour");
				expdata.sqlexecute.Parameters.AddWithValue("@colour",rowbgcolour);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

			if (altrowbgcolour != sAlternateRowColour)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@colourcode","altrowcolour");
				expdata.sqlexecute.Parameters.AddWithValue("@colour",altrowbgcolour);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

			if (hovercolour != sHoverColour)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@colourcode","hovercolour");
				expdata.sqlexecute.Parameters.AddWithValue("@colour",hovercolour);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}
			getColours();

		}

		private void deleteColours()
		{
			strsql = "delete from colours";
			
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public string menubarBGColour
		{
			get 
			{
				string colour;
				if (colours["menubarbgcolour"] == null)
				{
					colour = sMenubarBGColour;
				}
				else
				{
					colour = (string)colours["menubarbgcolour"];
				}
				return colour;
			}
		}

		public string menubarFGColour
		{
			get 
			{
				string colour;
				if (colours["menubarfgcolour"] == null)
				{
					colour = sMenubarFGColour;
				}
				else
				{
					colour = (string)colours["menubarfgcolour"];
				}
				return colour;
			}
		}

		public string titlebarBGColour
		{
			get 
			{
				string colour;
				if (colours["titlebarbgcolour"] == null)
				{
					colour = sTitlebarBGColour;
				}
				else
				{
					colour = (string)colours["titlebarbgcolour"];
				}
				return colour;
			}
		}

		public string titlebarFGColour
		{
			get 
			{
				string colour;
				if (colours["titlebarfgcolour"] == null)
				{
					colour = sTitlebarFGColour;
				}
				else
				{
					colour = (string)colours["titlebarfgcolour"];
				}
				return colour;
			}
		}

		public string rowColour
		{
			get 
			{
				string colour;
				if (colours["rowcolour"] == null)
				{
					colour = sRowColour;
				}
				else
				{
					colour = (string)colours["rowcolour"];
				}
				return colour;
			}
		}

		public string altRowColour
		{
			get 
			{
				string colour;
				if (colours["altrowcolour"] == null)
				{
					colour = sAlternateRowColour;
				}
				else
				{
					colour = (string)colours["altrowcolour"];
				}
				return colour;
			}
		}

		public string fieldBG
		{
			get 
			{
				string colour;
				if (colours["fieldbgcolour"] == null)
				{
					colour = sFieldBG;
				}
				else
				{
					colour = (string)colours["fieldbgcolour"];
				}
				return colour;
			}
		}

		public string fieldFG
		{
			get 
			{
				string colour;
				if (colours["fieldfgcolour"] == null)
				{
					colour = sFieldFG;
				}
				else
				{
					colour = (string)colours["fieldfgcolour"];
				}
				return colour;
			}
		}
		public string hovercolour
		{
			get 
			{
				string colour;
				if (colours["hovercolour"] == null)
				{
					colour = sHoverColour;
				}
				else
				{
					colour = (string)colours["hovercolour"];
				}
				return colour;
			}
		}
		#region properties
		public int accountid
		{
			get {return nAccountid;}
		}
		#endregion
	}
}
