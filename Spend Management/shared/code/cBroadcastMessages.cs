using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Caching;

using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

using Spend_Management;

/// <summary>
/// Summary description for cBroadcastMessages
/// </summary>
public class cBroadcastMessages
{

    string strsql;
    int nAccountid = 0;

    System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

    SortedList list;

    /// <summary>
    /// Not for general usage. The constructor is required by cGridNew when it tries to create an instance of cBroadcastMessages.
    /// </summary>
    public cBroadcastMessages()
    {
    }

    public cBroadcastMessages(int accountid)
    {
        nAccountid = accountid;


        InitialiseData();
    }

    #region properties
    public int accountid
    {
        get { return nAccountid; }
    }
    #endregion

    private void InitialiseData()
    {
        list = (System.Collections.SortedList)Cache["broadcast" + accountid];
        if (list == null)
        {
            list = CacheList();
        }

    }

    private SortedList CacheList()
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Data.SqlClient.SqlDataReader reader;
        SortedList list = new SortedList();
        int broadcastid;
        string message, title;
        DateTime startdate, enddate, datestamp;
        bool expirewhenread, oncepersession;
        broadcastLocation location;
        DateTime createdon, modifiedon;
        int createdby, modifiedby;

        cBroadcastMessage newmsg;
        DBConnection cachedata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = string.Format("select broadcastid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.broadcast_messages WHERE  {0} = {0}", this.accountid);
        cachedata.sqlexecute.CommandText = strsql;

        SqlCacheDependency dependency = null;
        if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
        {
            dependency = new SqlCacheDependency(cachedata.sqlexecute);
        }
        cachedata.ExecuteSQL(strsql);

        strsql = "select broadcastid, title, message, startdate, enddate, expirewhenread, location, datestamp, oncepersession, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.broadcast_messages";

        expdata.sqlexecute.CommandText = strsql;

        using (reader = expdata.GetReader(strsql))
        {
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                broadcastid = reader.GetInt32(reader.GetOrdinal("broadcastid"));
                title = reader.GetString(reader.GetOrdinal("title"));
                message = reader.GetString(reader.GetOrdinal("message"));
                if (reader.IsDBNull(reader.GetOrdinal("startdate")) == true)
                {
                    startdate = new DateTime(1900, 01, 01);
                }
                else
                {
                    startdate = reader.GetDateTime(reader.GetOrdinal("startdate"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("enddate")) == true)
                {
                    enddate = new DateTime(1900, 01, 01);
                }
                else
                {
                    enddate = reader.GetDateTime(reader.GetOrdinal("enddate"));
                }
                expirewhenread = reader.GetBoolean(reader.GetOrdinal("expirewhenread"));
                location = (broadcastLocation)reader.GetByte(reader.GetOrdinal("location"));
                datestamp = reader.GetDateTime(reader.GetOrdinal("datestamp"));
                oncepersession = reader.GetBoolean(reader.GetOrdinal("oncepersession"));
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
                newmsg = new cBroadcastMessage(broadcastid, title, message, startdate, enddate, expirewhenread, location, datestamp, oncepersession, createdon, createdby, modifiedon, modifiedby);
                list.Add(broadcastid, newmsg);
            }

            reader.Close();
        }

        if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
        {
            Cache.Insert("broadcast" + accountid, list, dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), System.Web.Caching.CacheItemPriority.Default, null);
        }
        return list;
    }

    public SortedList sortByDate()
    {
        cBroadcastMessage msg;
        SortedList sorted = new SortedList();
        for (int i = 0; i < list.Count; i++)
        {
            msg = (cBroadcastMessage)list.GetByIndex(i);
            sorted.Add(msg.datestamp, msg);
        }
        return sorted;
    }

    public System.Data.DataTable getMessagesToDisplay(broadcastLocation location, Employee employee)
    {
        System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
        cBroadcastMessage msg;
        object[] values;
        DataTable tbl = new DataTable();
        tbl.Columns.Add("broadcastid", System.Type.GetType("System.Int32"));
        tbl.Columns.Add("title", System.Type.GetType("System.String"));
        tbl.Columns.Add("message", System.Type.GetType("System.String"));
        tbl.Columns.Add("expirewhenread", System.Type.GetType("System.Boolean"));
        tbl.Columns.Add("oncepersession", typeof(System.Boolean));
        if (location == broadcastLocation.notSet)
        {
            return tbl;
        }

        // ensure the employee read broadcast list is populated

        SortedList sorted = sortByDate();
        for (int i = sorted.Count - 1; i >= 0; i--)
        {
            msg = (cBroadcastMessage)sorted.GetByIndex(i);
            if (msg.location == location && (msg.startdate == new DateTime(1900, 01, 01) || msg.startdate <= DateTime.Today) && (msg.enddate == new DateTime(1900, 01, 01) || msg.enddate >= DateTime.Today) && ((msg.expirewhenread == false && msg.oncepersession == false) || (msg.expirewhenread == true && employee.GetBroadcastMessagesRead().Contains(msg.broadcastid) == false) || (msg.expirewhenread == false && msg.oncepersession == true && appinfo.Session["broadcast" + msg.broadcastid] == null)))
            {
                values = new object[5];
                values[0] = msg.broadcastid;
                values[1] = msg.title;
                values[2] = msg.message;
                values[3] = msg.expirewhenread;
                values[4] = msg.oncepersession;
                tbl.Rows.Add(values);

            }
        }

        return tbl;
    }

    /// <summary>
    /// The get grid.
    /// </summary>
    /// <param name="module">
    /// The module.
    /// </param>
    /// <returns>
    /// The <see cref="DataSet"/>.
    /// </returns>
    public DataSet getGrid(Modules module)
    {
        var ds = new DataSet();
        var tbl = new DataTable();
        tbl.Columns.Add("broadcastid", Type.GetType("System.Int32"));
        tbl.Columns.Add("title", Type.GetType("System.String"));
        tbl.Columns.Add("startdate", Type.GetType("System.DateTime"));
        tbl.Columns.Add("enddate", Type.GetType("System.DateTime"));
        tbl.Columns.Add("expirewhenread", Type.GetType("System.Boolean"));
        tbl.Columns.Add("location", Type.GetType("System.Byte"));

        for (int i = 0; i < this.list.Count; i++)
        {
            var message = (cBroadcastMessage)this.list.GetByIndex(i);

            if ((module == Modules.Greenlight || module == Modules.GreenlightWorkforce) && message.location == broadcastLocation.SubmitClaim)
            {
                continue;
            }

            var values = new object[6];
            values[0] = message.broadcastid;
            values[1] = message.title;
            if (message.startdate != new DateTime(1900, 01, 01))
            {
                values[2] = message.startdate;
            }

            if (message.enddate != new DateTime(1900, 01, 01))
            {
                values[3] = message.enddate;
            }

            values[4] = message.expirewhenread;
            values[5] = Convert.ToByte(message.location);

            tbl.Rows.Add(values);
        }

        ds.Tables.Add(tbl);
        return ds;
    }

    public cBroadcastMessage getBroadcastMessageById(int broadcastid)
    {
        return (cBroadcastMessage)list[broadcastid];
    }

    public byte addBroadcastMessage(string title, string message, DateTime startdate, DateTime enddate, bool expirewhenread, broadcastLocation location, bool oncepersession, DateTime createdon, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        int broadcastid;
        strsql = "insert into broadcast_messages (title, message, startdate, enddate, expirewhenread, location, oncepersession, createdOn, createdBy) " +
            "values (@title, @message, @startdate, @enddate, @expirewhenread, @location, @oncepersession, @createdon, @userid ); select @identity = scope_identity();";

        expdata.sqlexecute.Parameters.AddWithValue("@title", title);
        expdata.sqlexecute.Parameters.AddWithValue("@message", message);
        expdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate);
        expdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate);
        expdata.sqlexecute.Parameters.AddWithValue("@expirewhenread", Convert.ToByte(expirewhenread));
        expdata.sqlexecute.Parameters.AddWithValue("@location", Convert.ToByte(location));
        expdata.sqlexecute.Parameters.AddWithValue("@oncepersession", Convert.ToByte(oncepersession));
        expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
        expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
        expdata.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
        expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
        expdata.ExecuteSQL(strsql);
        broadcastid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
        expdata.sqlexecute.Parameters.Clear();

        cBroadcastMessage msg = new cBroadcastMessage(broadcastid, title, message, startdate, enddate, expirewhenread, location, DateTime.Now, oncepersession, createdon, userid, new DateTime(1900 / 01 / 01), 0);
        list.Add(broadcastid, msg);

        return 0;
    }

    public byte updateBroadcastMessage(int broadcastid, string title, string message, DateTime startdate, DateTime enddate, bool expirewhenread, broadcastLocation location, bool oncepersession, DateTime modifiedon, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "update broadcast_messages set title = @title, message = @message, startdate = @startdate, enddate = @enddate, expirewhenread = @expirewhenread, location = @location, oncepersession = @oncepersession, modifiedon = @modifiedon, modifiedby = @modifiedby where broadcastid = @broadcastid";

        expdata.sqlexecute.Parameters.AddWithValue("@title", title);
        expdata.sqlexecute.Parameters.AddWithValue("@message", message);
        expdata.sqlexecute.Parameters.AddWithValue("@startdate", startdate);
        expdata.sqlexecute.Parameters.AddWithValue("@enddate", enddate);
        expdata.sqlexecute.Parameters.AddWithValue("@expirewhenread", Convert.ToByte(expirewhenread));
        expdata.sqlexecute.Parameters.AddWithValue("@location", Convert.ToByte(location));
        expdata.sqlexecute.Parameters.AddWithValue("@broadcastid", broadcastid);
        expdata.sqlexecute.Parameters.AddWithValue("@oncepersession", Convert.ToByte(oncepersession));
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);

        expdata.ExecuteSQL(strsql);

        expdata.sqlexecute.Parameters.Clear();

        list[broadcastid] = new cBroadcastMessage(broadcastid, title, message, startdate, enddate, expirewhenread, location, DateTime.Now, oncepersession, new DateTime(1900 / 01 / 01), 0, modifiedon, userid);
        return 0;
    }

    public void deleteBroadcastMessage(int broadcastid)
    {
        ClearCacheForAffectedEmployees(broadcastid);
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "delete from broadcast_messages where broadcastid = @broadcastid";
        expdata.sqlexecute.Parameters.AddWithValue("@broadcastid", broadcastid);
        expdata.ExecuteSQL(strsql);
        list.Remove(broadcastid);
    }

    public void ClearCacheForAffectedEmployees(int broadcastID)
    {
        var cache = new Utilities.DistributedCaching.Cache();

        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        const string SQL = "SELECT employeeid FROM employee_readbroadcasts WHERE broadcastid = @broadcastID";
        expdata.sqlexecute.Parameters.AddWithValue("@broadcastID", broadcastID);

        using (SqlDataReader reader = expdata.GetReader(SQL))
        {
            while (reader.Read())
            {
                cache.Delete(accountid, EmployeeBroadcastMessages.CacheArea, reader.GetInt32(0).ToString());
            }

            reader.Close();
        }
    }

    public Dictionary<int, cBroadcastMessage> getModifiedBroadcastMessages(DateTime date)
    {
        Dictionary<int, cBroadcastMessage> lst = new Dictionary<int, cBroadcastMessage>();
        foreach (cBroadcastMessage val in list.Values)
        {
            if (val.createdon > date || val.modifiedon > date)
            {
                lst.Add(val.broadcastid, val);
            }
        }
        return lst;
    }

    public List<int> getBroadcastMessageIds()
    {
        List<int> ids = new List<int>();
        foreach (cBroadcastMessage val in list.Values)
        {
            ids.Add(val.broadcastid);
        }
        return ids;
    }

    [Obsolete("Not used apparently", true)]
    public List<int> getModifiedEmployeeReadBroadcasts(int employeeid, DateTime date)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Data.SqlClient.SqlDataReader reader;
        List<int> ids = new List<int>();

        strsql = "SELECT broadcastid FROM employee_readbroadcasts WHERE createdon > @date and employeeid = @employeeid";
        expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
        expdata.sqlexecute.Parameters.AddWithValue("@date", date);
        using (reader = expdata.GetReader(strsql))
        {
            while (reader.Read())
            {
                ids.Add(reader.GetInt32(reader.GetOrdinal("broadcastid")));
            }
            reader.Close();
        }
        expdata.sqlexecute.Parameters.Clear();

        return ids;

    }

    /// <summary>
    /// returns broadcast messages for the user
    /// </summary>
    /// <param name="user">the CurrentUser</param>
    /// <param name="path">the path of the current page that the user is on</param>
    /// <returns>a List of cBroadcastMessage</returns>
    public List<cBroadcastMessage> GetMessages(Employee employee, int accountId, broadcastLocation location = broadcastLocation.HomePage)
    {
        var messagesTable = this.getMessagesToDisplay(location, employee);

        var messageList = new List<cBroadcastMessage>();
        if (messagesTable.Rows.Count > 0)
        {
            foreach (DataRow messageRow in messagesTable.Rows)
            {
                var messageId = (int)messageRow["broadcastid"];

                if ((bool)messageRow["expirewhenread"])
                {
                    employee.GetBroadcastMessagesRead().Add(messageId);
                }

                messageList.Add(this.getBroadcastMessageById(messageId));
            }
        }

        return messageList;
    }

    /// <summary>
    /// Creates the broadcast message grid
    /// </summary>
    /// <param name="accountid">
    /// The accountid of the current user
    /// </param>
    /// <param name="employeeid">
    /// The employeeid of the current user.
    /// </param>
    /// <returns>
    /// The <see cref="DataSet"/> of the grid
    /// </returns>
    public string[] CreateGrid(int accountid, int employeeid)
    {
        cTables clstables = new cTables(accountid);
        cFields clsfields = new cFields(accountid);
        List<cNewGridColumn> columns = new List<cNewGridColumn>();
        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("D097D6BB-B51C-4BBB-8AF9-EA12F9DCF03B")))); // broadcastid
        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("E786773E-1C9A-44CD-A2F8-018EE3BECB16")))); // title
        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("6AEF1AF6-35F6-47BE-9577-A2D98FAEE2C8")))); // startdate
        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("D22D0481-4A90-4DBD-AF26-1CE3AA2178F4")))); // enddate
        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("4A6AAE5A-B343-4DB2-962D-4AACCD1E196B")))); // expirewhenread
        columns.Add(new cFieldColumn(clsfields.GetFieldByID(new Guid("0F539F73-AB6C-4627-AFAC-93CCED12C272")))); // location
        cGridNew clsgrid = new cGridNew(accountid, employeeid, "gridBroadcastMessages", clstables.GetTableByID(new Guid("A6472255-3751-4EA0-B485-132C932277EE")), columns); // broadcast_messages
        clsgrid.getColumnByName("broadcastid").hidden = true;
        clsgrid.KeyField = "broadcastid";
        clsgrid.enabledeleting = true;
        clsgrid.deletelink = "javascript:deleteBroadcast({broadcastid});";
        clsgrid.enableupdating = true;
        clsgrid.editlink = "aebroadcastmessage.aspx?broadcastid={broadcastid}";
        clsgrid.InitialiseRow += this.broadcastMessagesGrid_InitialiseRow;
        clsgrid.ServiceClassForInitialiseRowEvent = "cBroadcastMessages";
        clsgrid.ServiceClassMethodForInitialiseRowEvent = "broadcastMessagesGrid_InitialiseRow";
        return clsgrid.generateGrid();
    }

    /// <summary>
    /// The initialise row event of the grid
    /// </summary>
    /// <param name="row">The row in the grid</param>
    /// <param name="gridinfo">The grid information</param>
    public void broadcastMessagesGrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridinfo)
    {
        if ((DateTime)row.getCellByID("startdate").Value == new DateTime(1900, 01, 01))
        {
            row.getCellByID("startdate").Value = "";
        }

        if ((DateTime)row.getCellByID("enddate").Value == new DateTime(1900, 01, 01))
        {
            row.getCellByID("enddate").Value = "";
        }

        byte location = Convert.ToByte(row.getCellByID("location").Value);

        if (location == 1)
        {
            row.getCellByID("location").Value = "Home Page";
        }
        else if (location == 2)
        {
            row.getCellByID("location").Value = "Submit Claim";
        }
        else
        {
            row.getCellByID("location").Value = "Not Set";
        }

    }
}