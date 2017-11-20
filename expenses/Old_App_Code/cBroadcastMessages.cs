using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using expenses;
using expenses.Old_App_Code;
using System.Web.Caching;
using ExpensesLibrary;
using System.Collections.Generic;
using SpendManagementLibrary;
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
        strsql = "select broadcastid, title, startdate, enddate, expirewhenread, location, datestamp, oncepersession, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.broadcast_messages";
        cachedata.sqlexecute.CommandText = strsql;
        SqlCacheDependency dep = new SqlCacheDependency(cachedata.sqlexecute);
        cachedata.ExecuteSQL(strsql);


        strsql = "select broadcastid, title, message, startdate, enddate, expirewhenread, location, datestamp, oncepersession, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.broadcast_messages";
        
        expdata.sqlexecute.CommandText = strsql;
        
        reader = expdata.GetReader(strsql);
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
        

        Cache.Insert("broadcast" + accountid, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);
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

    public System.Data.DataTable getMessagesToDisplay(broadcastLocation location, cEmployee employee)
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

        SortedList sorted = sortByDate();
        for (int i = sorted.Count-1; i >= 0; i--)
        {
            msg = (cBroadcastMessage)sorted.GetByIndex(i);
            if (msg.location == location && (msg.startdate == new DateTime(1900,01,01) || msg.startdate <= DateTime.Today) && (msg.enddate == new DateTime(1900,01,01) || msg.enddate >= DateTime.Today) &&  ((msg.expirewhenread == false && msg.oncepersession == false) ||(msg.expirewhenread == true && employee.readBroadcast(msg.broadcastid) == false) || (msg.expirewhenread == false && msg.oncepersession == true && appinfo.Session["broadcast" + msg.broadcastid] == null)))
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

    public DataSet getGrid()
    {
        DataSet ds = new DataSet();
        DataTable tbl = new DataTable();
        cBroadcastMessage msg;
        object[] values;
        tbl.Columns.Add("broadcastid", System.Type.GetType("System.Int32"));
        tbl.Columns.Add("title", System.Type.GetType("System.String"));
        tbl.Columns.Add("startdate", System.Type.GetType("System.DateTime"));
        tbl.Columns.Add("enddate", System.Type.GetType("System.DateTime"));
        tbl.Columns.Add("expirewhenread", System.Type.GetType("System.Boolean"));
        tbl.Columns.Add("location", System.Type.GetType("System.Byte"));

        for (int i = 0; i < list.Count; i++)
        {
            msg = (cBroadcastMessage)list.GetByIndex(i);
            values = new object[6];
            values[0] = msg.broadcastid;
            values[1] = msg.title;
            if (msg.startdate != new DateTime(1900, 01, 01))
            {
                values[2] = msg.startdate;
            }
            if (msg.enddate != new DateTime(1900, 01, 01))
            {
                values[3] = msg.enddate;
            }
            values[4] = msg.expirewhenread;
            values[5] = Convert.ToByte(msg.location);
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
        strsql = "insert into broadcast_messages (title, message, startdate, enddate, expirewhenread, location, oncepersession) " +
            "values (@title, @message, @startdate, @enddate, @expirewhenread, @location, @oncepersession); select @identity = scope_identity();";
        
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

        cBroadcastMessage msg = new cBroadcastMessage(broadcastid, title, message, startdate, enddate, expirewhenread, location, DateTime.Now, oncepersession, createdon, userid, new DateTime(1900/01/01), 0);
        list.Add(broadcastid, msg);

        return 0;
    }

    public byte updateBroadcastMessage(int broadcastid, string title, string message, DateTime startdate, DateTime enddate, bool expirewhenread, broadcastLocation location, bool oncepersession, DateTime modifiedon, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "update broadcast_messages set message = @message, startdate = @startdate, enddate = @enddate, expirewhenread = @expirewhenread, location = @location, oncepersession = @oncepersession where broadcastid = @broadcastid";
        
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

        list[broadcastid] = new cBroadcastMessage(broadcastid, title, message, startdate, enddate, expirewhenread, location, DateTime.Now, oncepersession, new DateTime(1900/01/01), 0, modifiedon, userid);
        return 0;
    }

    public void deleteBroadcastMessage(int broadcastid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "delete from broadcast_messages where broadcastid = @broadcastid";
        expdata.sqlexecute.Parameters.AddWithValue("@broadcastid", broadcastid);
        expdata.ExecuteSQL(strsql);
        list.Remove(broadcastid);
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

    public List<int> getModifiedEmployeeReadBroadcasts(int employeeid, DateTime date)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Data.SqlClient.SqlDataReader reader;
        List<int> ids = new List<int>();

        strsql = "SELECT broadcastid FROM employee_readbroadcasts WHERE createdon > @date and employeeid = @employeeid";
        expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
        expdata.sqlexecute.Parameters.AddWithValue("@date", date);
        reader = expdata.GetReader(strsql);

        while (reader.Read())
        {
            ids.Add(reader.GetInt32(reader.GetOrdinal("broadcastid")));
        }
        reader.Close();
        expdata.sqlexecute.Parameters.Clear();

        return ids;

    }

}


