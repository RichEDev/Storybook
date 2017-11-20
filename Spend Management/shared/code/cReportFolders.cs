using System;
using System.Web.Caching;
using System.Configuration;
using SpendManagementLibrary;
using Spend_Management;
using System.Collections.Generic;

/// <summary>
/// Summary description for cReportFolders.
/// </summary>
public class cReportFolders
{

    string strsql = "";

    int nAccountid;

    readonly Cache cache = System.Web.HttpContext.Current.Cache;
    System.Collections.SortedList list;

    public cReportFolders(int accountid)
    {
        nAccountid = accountid;


        InitialiseData();
    }



    private void InitialiseData()
    {
        list = (System.Collections.SortedList)this.cache["reportfolders" + accountid];
        if (list == null)
        {
            list = CacheList();
        }
    }


    private System.Collections.SortedList CacheList()
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Collections.SortedList list = new System.Collections.SortedList();
        cReportFolder newfolder;
        Guid folderid;
        int employeeid;
        string folder;
        bool personal;
        ReportArea reportArea;
        SqlCacheDependency dependency = null;
        System.Data.SqlClient.SqlDataReader reader;


        strsql = "select folderid, reportArea, foldername, employeeid, personal from dbo.reportfolders";
        expdata.sqlexecute.CommandText = strsql;

        if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
        {
            dependency = new SqlCacheDependency(expdata.sqlexecute);
        }

        using (reader = expdata.GetReader(strsql))
        {
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                folderid = reader.GetGuid(reader.GetOrdinal("folderid"));
                reportArea = (ReportArea)reader.GetInt32(reader.GetOrdinal("reportArea"));
                if (reader.IsDBNull(reader.GetOrdinal("employeeid")) == true)
                {
                    employeeid = 0;
                }
                else
                {
                    employeeid = reader.GetInt32(reader.GetOrdinal("employeeid"));
                }
                folder = reader.GetString(reader.GetOrdinal("foldername"));
                personal = reader.GetBoolean(reader.GetOrdinal("personal"));
                newfolder = new cReportFolder(folderid, folder, employeeid, personal, reportArea);
                list.Add(folderid, newfolder);
            }
            reader.Close();
        }

        if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
        {
            this.cache.Insert("reportfolders" + accountid, list, dependency, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15));
        }
        return list;
    }

    public int count
    {
        get { return list.Count; }
    }
    private bool alreadyExists(string folder, int action, Guid folderid)
    {
        int i;
        cReportFolder fold;

        for (i = 0; i < list.Count; i++)
        {
            fold = (cReportFolder)list.GetByIndex(i);
            if (action == 2)
            {
                if (fold.folder.ToLower().Trim() == folder.ToLower().Trim() && fold.folderid != folderid)
                {
                    return true;
                }
            }
            else
            {
                if (fold.folder.ToLower().Trim() == folder.ToLower().ToLower())
                {
                    return true;
                }
            }
        }
        return false;
    }
    public byte addFolder(string folder, int employeeid, bool personal)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        Guid folderid = Guid.NewGuid();
        if (alreadyExists(folder, 0, Guid.Empty) == true)
        {
            return 1;
        }

        strsql = "insert into report_folders (folderid, foldername, employeeid, personal) values (@folderid, @folder, @employeeid, @personal);";
        expdata.sqlexecute.Parameters.AddWithValue("@folderid", folderid);
        expdata.sqlexecute.Parameters.AddWithValue("@folder", folder);
        expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
        expdata.sqlexecute.Parameters.AddWithValue("@personal", Convert.ToByte(personal));
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        list.Add(folderid, new cReportFolder(folderid, folder, employeeid, personal, ReportArea.Custom));

        return 0;
    }

    /// <summary>
    /// Update a custom report folder.
    /// </summary>
    /// <param name="folderid">The real folderID</param>
    /// <param name="folder"></param>
    /// <param name="personal"></param>
    /// <returns></returns>
    public byte updateFolder(Guid folderid, string folder, bool personal)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        if (alreadyExists(folder, 2, folderid) == true)
        {
            return 1;
        }
        cReportFolder oldfolder;

        oldfolder = getFolderById(folderid);

        strsql = "update report_folders set foldername = @folder, personal = @personal " +
            "where folderid = @folderid";
        expdata.sqlexecute.Parameters.AddWithValue("@folder", folder);
        expdata.sqlexecute.Parameters.AddWithValue("@folderid", oldfolder.folderid);
        expdata.sqlexecute.Parameters.AddWithValue("@personal", Convert.ToByte(personal));
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        list[folderid] = new cReportFolder(oldfolder.folderid, folder, oldfolder.employeeid, personal, oldfolder.reportArea);
        #region auditlog
        //cAuditLog clsaudit = new cAuditLog();
        //if (oldfolder.folder != folder)
        //{
        //    clsaudit.editRecord(folder, "Folder", "Report Folders", oldfolder.folder, folder);
        //}
        //if (oldfolder.personal != personal)
        //{
        //    clsaudit.editRecord(folder, "Personal Folder", "Report Folders", oldfolder.personal.ToString(), personal.ToString());
        //}
        #endregion

        return 0;
    }

    /// <summary>
    /// Delete a custom report folder
    /// </summary>
    /// <param name="folderid">The real folderID</param>
    public void deleteFolder(Guid folderid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        cReportFolder oldfolder;

        oldfolder = getFolderById(folderid);

        expdata.sqlexecute.Parameters.AddWithValue("@folderid", folderid);

        strsql = "update reports set folderid = '" + Guid.Empty + "' where folderid = @folderid";
        expdata.ExecuteSQL(strsql);

        strsql = "delete from report_folders where folderid = @folderid";

        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        //cAuditLog clsaudit = new cAuditLog();
        //clsaudit.deleteRecord("Report Folders", oldfolder.folder);

        list.Remove(folderid);
    }

    public System.Data.DataSet getGrid()
    {
        cReportFolder folder;
        SortedList<string, cReportFolder> sorted = sortList();
        System.Data.DataSet ds = new System.Data.DataSet();
        System.Data.DataTable tbl = new System.Data.DataTable();
        object[] values;
        int i;

        tbl.Columns.Add("folderid", System.Type.GetType("System.Guid"));
        tbl.Columns.Add("folder", System.Type.GetType("System.String"));
        tbl.Columns.Add("employeeid", System.Type.GetType("System.Int32"));
        tbl.Columns.Add("personal", System.Type.GetType("System.Boolean"));
        tbl.Columns.Add("reportArea", System.Type.GetType("System.String"));

        foreach (KeyValuePair<string, cReportFolder> kvp in sorted)
        {
            folder = (cReportFolder)kvp.Value;
            //if ((reportArea == ReportArea.Global && folder.reportArea == ReportArea.Global) || (reportArea == ReportArea.Custom && folder.reportArea == ReportArea.Custom) || (reportArea == ReportArea.All))
            //{
            values = new object[5];
            values[0] = folder.folderid;
            values[1] = folder.folder;
            values[2] = folder.employeeid;
            values[3] = folder.personal;
            values[4] = (int)folder.reportArea;
            tbl.Rows.Add(values);
            //}
        }
        ds.Tables.Add(tbl);
        return ds;

    }

    public SortedList<string, cReportFolder> sortList()
    {
        int i;
        cReportFolder folder;
        SortedList<string, cReportFolder> sorted = new SortedList<string, cReportFolder>();

        for (i = 0; i < list.Count; i++)
        {
            folder = (cReportFolder)list.GetByIndex(i);
            if (!sorted.ContainsKey(folder.folder))
            {
                sorted.Add(folder.folder, folder);
            }
        }

        return sorted;
    }

    public cReportFolder getFolderById(Guid folderid)
    {
        if (list.Contains(folderid))
        {
            return (cReportFolder)list[folderid];
        }
        return null;
    }

    public System.Web.UI.WebControls.ListItem[] CreateDropDown()
    {
        cReportFolder folder;
        System.Web.UI.WebControls.ListItem[] items;
        int i;

        SortedList<string, cReportFolder> sorted = sortList();

        Dictionary<Guid, string> lstFolders = new Dictionary<Guid, string>();

        foreach (KeyValuePair<string, cReportFolder> folder_kvp in sorted)
        {
            folder = (cReportFolder)folder_kvp.Value;
            if (folder != null)
            {
                lstFolders.Add(folder.folderid, folder.folder);
            }
        }

        items = new System.Web.UI.WebControls.ListItem[lstFolders.Count + 1];
        items[0] = new System.Web.UI.WebControls.ListItem("[None]", Guid.Empty.ToString());
        i = 0;
        foreach (KeyValuePair<Guid, string> kvp in lstFolders)
        {
            items[i + 1] = new System.Web.UI.WebControls.ListItem(kvp.Value, kvp.Key.ToString());
            i++;
        }

        return items;
    }
    #region properties
    public int accountid
    {
        get { return nAccountid; }
    }
    #endregion
}


