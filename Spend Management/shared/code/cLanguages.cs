using System;
using System.Configuration;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
	/// <summary>
	/// Summary description for cLanguages.
	/// </summary>
	public class cLanguages
	{
		System.Collections.SortedList languages = new System.Collections.SortedList();
        DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
		System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpContext.Current.Cache;

		string strsql;
		string sLanguage;
		public cLanguages()
		{
			getLanguage();
			InitialiseData();
		}

		private void getLanguage()
		{
            
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            
            
            if (appinfo.User.Identity.Name == "")
            {
                return;
            }

            CurrentUser user = cMisc.GetCurrentUser();

            cMisc clsmisc = new cMisc(user.AccountID);
            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(user.AccountID);

            sLanguage = clsproperties.language;
		}
		private void InitialiseData()
		{
			languages = (System.Collections.SortedList)Cache["languages"];
			if (languages == null)
			{
				languages = CacheList();
			}
			
		}

		public string convertPhrase(string phrase)
		{
			if (language == "")
			{
				return phrase;
			}

			string convertedPhrase;
			System.Collections.SortedList phrases = (System.Collections.SortedList)languages[language];
			if ((string)phrases[phrase] == "" || phrases[phrase] == null)
			{
				convertedPhrase = phrase;
			}
			else
			{
				convertedPhrase = (string)phrases[phrase];
			}
			return convertedPhrase;
		}
		public string[] getLangaugeList()
		{
			int i;
			string[] list = new string[languages.Count + 1];

			list[0] = "English";
			for (i = 0; i < languages.Count; i++)
			{
				list[i+1] = languages.GetKey(i).ToString();
				
			}
			return list;
		}
		private System.Collections.SortedList CacheList()
		{
			System.Collections.SortedList languages = new System.Collections.SortedList();
			System.Collections.SortedList language;
			
			int i;
			string phrase;
			string conversion;
			System.Data.SqlClient.SqlDataReader reader;

			//set up the lists;
			strsql = "exec sp_columns @table_name = 'languages'";
		    using (reader = expdata.GetReader(strsql))
		    {
		        while (reader.Read())
		        {
		            if (reader.GetString(reader.GetOrdinal("column_name")) != "phraseid" && reader.GetString(reader.GetOrdinal("column_name")) != "phrase")
		            {
		                languages.Add(reader.GetString(reader.GetOrdinal("column_name")), new System.Collections.SortedList());
		            }
		        }
		        reader.Close();
		    }

		    strsql = "select  Dutch, phrase, phraseid from dbo.languages";
            expdata.sqlexecute.CommandText = strsql;
            
			reader = expdata.GetReader(strsql);
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("languages", languages, dep, Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Permanent), CacheItemPriority.Default,
                    null);
            }
			while (reader.Read())
			{
				
				phrase = reader.GetString(reader.GetOrdinal("phrase"));
				for (i = 0; i < reader.FieldCount; i++)
				{
					if (reader.GetName(i) != "phraseid" && reader.GetName(i) != "phrase")
					{
						language = (System.Collections.SortedList)languages[reader.GetName(i)];
						if (reader.IsDBNull(i) == false)
						{
							conversion = reader.GetString(i);
						}
						else
						{
							conversion = "";
						}
						language.Add(phrase,conversion);
					}
				}
			}
			reader.Close();
			
            //Cache["languages"] = languages;

		    return languages;
		}

		#region properties
		public string language
		{
			get {return sLanguage;}
		}
		#endregion
	}

	public class cLanguage
	{
		private int nPhraseid;
		private string sPhrase;
		private string sConversion;

		public cLanguage (int phraseid, string phrase, string conversion)
		{
			nPhraseid = phraseid;
			sPhrase = phrase;
			sConversion = conversion;
		}

		#region properties
		public int phraseid
		{
			get {return nPhraseid;}
		}
		public string phrase
		{
			get {return sPhrase;}
		}
		public string conversion
		{
			get {return sConversion;}
		}
		#endregion
	
	}
}
