using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using SpendManagementLibrary;
using System.Data.SqlClient;
using BusinessLogic.Accounts;
using BusinessLogic.Cache;
using BusinessLogic.Databases;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;

namespace Spend_Management
{
    using System.Linq;

    using BusinessLogic.ProjectCodes;

    using CacheDataAccess.Caching;

    using Common.Logging;
    using Common.Logging.Log4Net;
    using Common.Logging.NullLogger;

    using Configuration.Core;
    using Configuration.Interface;

    /// <summary>
    /// Summary description for cProjectCodes.
    /// </summary>
    [Obsolete("This class should no longer be used, remaining areas of usage are due to not supporting dependency injection of IDataFactoryArchivable<IProjectCodeWithUserDefinedFields, int> projectCodes")]
    public class cProjectCodes
	{
        Dictionary<int, cProjectCode> list;
        
        public cProjectCodes(int accountid, IDBConnection connection = null)
		{
			this.AccountId = accountid;

		    if (this.list == null)
		    {
		        this.list = this.CacheList();
		    }
		}

		private int AccountId
		{
			get; set;
		}

        public List<System.Web.UI.WebControls.ListItem> CreateDropDown(bool usedesc, bool includeNoneOption = false)
        {
            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();

            SortedList<string, cProjectCode> sorted = new SortedList<string, cProjectCode>();
            if (usedesc)
            {
                foreach (cProjectCode code in list.Values)
                {
                    if (sorted.ContainsKey(code.description) == false)
                    {
                        sorted.Add(code.description, code);
                    }
                }
            }
            else
            {
                foreach (cProjectCode code in list.Values)
                {
                    if (!sorted.ContainsKey(code.projectcode))
                    {
                        sorted.Add(code.projectcode, code);
                    }
                }
            }

            foreach (cProjectCode reqcode in sorted.Values)
            {
                if (!reqcode.archived)
                {
                    if (usedesc)
                    {
                        items.Add(new System.Web.UI.WebControls.ListItem(reqcode.description, reqcode.projectcodeid.ToString()));
                    }
                    else
                    {
                        items.Add(new System.Web.UI.WebControls.ListItem(reqcode.projectcode, reqcode.projectcodeid.ToString()));
                    }
                }
            }

            if (includeNoneOption)
            {
                items.Insert(0, new System.Web.UI.WebControls.ListItem("[None]", "0"));
            }

            return items;
        }
        
	    public int SaveProjectCode(cProjectCode projectcode)
	    {
            DBConnection data = new DBConnection(cAccounts.getConnectionString(this.AccountId));
            data.sqlexecute.Parameters.AddWithValue("@projectcodeid", projectcode.projectcodeid);
            data.sqlexecute.Parameters.AddWithValue("@projectcode", projectcode.projectcode);

            if (projectcode.description.Length > 1999)
            {
                projectcode.description = projectcode.description.Substring(0, 1998);
            }

            data.sqlexecute.Parameters.AddWithValue("@description", projectcode.description);
            data.sqlexecute.Parameters.AddWithValue("@rechargeable", 0);

	        CurrentUser currentUser = cMisc.GetCurrentUser();
	        data.sqlexecute.Parameters.AddWithValue("@userid", DBNull.Value);
	        data.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);

            if (currentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                data.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            data.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveProjectcode");

            int id = (int)data.sqlexecute.Parameters["@returnvalue"].Value;
            data.sqlexecute.Parameters.Clear();

            if (id < 0)
            {
                return id;
            }
            
            return id;
        }

	    public cProjectCode GetProjectCodeByName(string name)
	    {
	        return this.GetSingleFromCache("names");
        }

	    public cProjectCode GetProjectCodeByDesc(string description)
	    {
	        return this.GetSingleFromCache("descriptions");
	    }

	    private cProjectCode GetSingleFromCache(string hash)
	    {
	        RedisCache<IProjectCode, int> redisCache = new RedisCache<IProjectCode, int>(new LogFactory<cProjectCodes>().GetLogger(), new WebConfigurationManagerAdapter(), new BinarySerializer());

	        IProjectCode projectCode = redisCache.HashGet(new AccountCacheKey<int>(new Account(this.AccountId, null, false)) { Area = typeof(IProjectCode).Name }, "descriptions", hash);

	        if (projectCode == null)
	        {
	            return null;
	        }

	        return new cProjectCode(projectCode.Id, projectCode.Name, projectCode.Description, projectCode.Archived);
        }

        public cProjectCode getProjectCodeById(int projectcodeid)
        {
            cProjectCode code;
            list.TryGetValue(projectcodeid, out code);
            return code;
        }

        private Dictionary<int, cProjectCode> CacheList(IDBConnection connection = null)
        {
            RedisCache<IProjectCode, int> redisCache = new RedisCache<IProjectCode, int>(new LogFactory<cProjectCodes>().GetLogger(), new WebConfigurationManagerAdapter(), new BinarySerializer());
            IList<IProjectCode> projectCodes = redisCache.HashGetAll(new AccountCacheKey<int>(new Account(this.AccountId, null, false)) { Area = typeof(IProjectCode).Name }, "list");

            if (projectCodes != null && projectCodes.Count > 0)
            {
                return projectCodes?.ToDictionary(projectCode => projectCode.Id, projectCode => new cProjectCode(projectCode.Id, projectCode.Name, projectCode.Description, projectCode.Archived));
            }

            return new Dictionary<int, cProjectCode>();
        }
	}
}
