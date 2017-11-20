using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using SpendManagementLibrary.Employees;

using SpendManagementLibrary;
using Spend_Management;
using Utilities.DistributedCaching;
/// <summary>
/// Summary description for cItemRoles
/// </summary>
public class cItemRoles
{
    int nAccountid = 0;

    Dictionary<int, cItemRole> lstitems;

    /// <summary>
    /// The cache area.
    /// </summary>
    public const string CacheArea = "itemroles";

    /// <summary>
    /// The caching object .
    /// </summary>
    private readonly Cache caching = new Cache();

    public cItemRoles(int accountid)
    {
        nAccountid = accountid;
        this.InitialiseData();
    }

    public Dictionary<int, cItemRole> itemRoles
    {
        get
        {
            if (lstitems == null)
            {
                InitialiseData();
            }


            return lstitems;
        }
    }

    private int accountid
    {
        get { return nAccountid; }
    }

    private void InitialiseData()
    {
        this.lstitems = this.caching.Get(this.accountid, CacheArea, "0") as Dictionary<int, cItemRole>
                            ?? this.CacheList();

    }

    private Dictionary<int, cItemRole> CacheList()
    {
        int itemroleid;
        string rolename, description;
        DateTime createdon, modifiedon;
        int createdby, modifiedby;
        Dictionary<int, cItemRole> list = new Dictionary<int, cItemRole>();
        cItemRole newrole;

        SortedList<int, Dictionary<int, cRoleSubcat>> lstSubcats = getSubcatDetails();
        Dictionary<int, cRoleSubcat> subcats;

        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

        var strsql = "select  itemroleid, rolename, description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.item_roles";
        expdata.sqlexecute.CommandText = strsql;



        using (SqlDataReader reader = expdata.GetReader(strsql))
        {
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                itemroleid = reader.GetInt32(reader.GetOrdinal("itemroleid"));
                rolename = reader.GetString(reader.GetOrdinal("rolename"));
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
                lstSubcats.TryGetValue(itemroleid, out subcats);
                if (subcats == null)
                {
                    subcats = new Dictionary<int, cRoleSubcat>();
                }
                newrole = new cItemRole(itemroleid, rolename, description, subcats, createdon, createdby, modifiedon, modifiedby);
                list.Add(itemroleid, newrole);
            }
            reader.Close();
        }

        this.caching.Add(this.accountid, CacheArea, "0", list);

        return list;
    }

    /// <summary>
    /// The invalidate cache.
    /// </summary>
    private void InvalidateCache()
    {
        this.caching.Delete(this.accountid, CacheArea, "0");
        this.lstitems = null;
    }

    public SortedList<int, Dictionary<int, cRoleSubcat>> getSubcatDetails()
    {
        SortedList<int, Dictionary<int, cRoleSubcat>> subcats = new SortedList<int, Dictionary<int, cRoleSubcat>>();
        Dictionary<int, cRoleSubcat> list;
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        string strsql;
        cRoleSubcat clssubcat;
        cSubcats clssubcats = new cSubcats(accountid);
        int rolesubcatid, subcatid, roleid;
        decimal maximum, receiptmaximum;
        bool isadditem;
        strsql = "select * from rolesubcats";

        using (SqlDataReader reader = expdata.GetReader(strsql))
        {
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                roleid = reader.GetInt32(reader.GetOrdinal("roleid"));
                rolesubcatid = reader.GetInt32(reader.GetOrdinal("rolesubcatid"));
                subcatid = reader.GetInt32(reader.GetOrdinal("subcatid"));
                isadditem = reader.GetBoolean(reader.GetOrdinal("isadditem"));

                if (reader.IsDBNull(reader.GetOrdinal("maximum")) == false)
                {
                    maximum = reader.GetDecimal(reader.GetOrdinal("maximum"));
                }
                else
                {
                    maximum = 0;
                }

                if (reader.IsDBNull(reader.GetOrdinal("receiptmaximum")) == false)
                {
                    receiptmaximum = reader.GetDecimal(reader.GetOrdinal("receiptmaximum"));
                }
                else
                {
                    receiptmaximum = 0;
                }

                subcats.TryGetValue(roleid, out list);

                if (list == null)
                {
                    list = new Dictionary<int, cRoleSubcat>();
                    subcats.Add(roleid, list);
                }

                clssubcat = new cRoleSubcat(rolesubcatid, roleid, subcatid, maximum, receiptmaximum, isadditem);

                list.Add(subcatid, clssubcat);

            }

            reader.Close();

        }

        return subcats;
    }
    public cItemRole getItemRoleById(int id)
    {
        cItemRole role = null;
        itemRoles.TryGetValue(id, out role);
        return role;
    }

    public cItemRole getItemRoleByName(string name)
    {
        foreach (cItemRole role in itemRoles.Values)
        {
            if (role.rolename == name)
            {
                return role;
            }
        }
        return null;
    }

    public List<ListItem> CreateDropDown(int selectedItemRole, bool includeNone)
    {
        List<ListItem> items = new List<ListItem>();

        SortedList<string, cItemRole> roles = sortList();

        ListItem tmpListItem;

        if (includeNone == true)
        {
            items.Add(new ListItem("[None]", "0"));
        }

        foreach (cItemRole role in roles.Values)
        {
            tmpListItem = new ListItem(role.rolename, role.itemroleid.ToString());
            if (role.itemroleid == selectedItemRole)
            {
                tmpListItem.Selected = true;
            }
            items.Add(tmpListItem);
        }

        return items;
    }
    private SortedList<string, cItemRole> sortList()
    {
        SortedList<string, cItemRole> sorted = new SortedList<string, cItemRole>();

        foreach (cItemRole role in itemRoles.Values)
        {
            if (sorted.ContainsKey(role.rolename) == false)
            {
                sorted.Add(role.rolename, role);
            }
        }
        return sorted;
    }

    public SortedList<string, cItemRole> getSortedList()
    {
        return sortList();
    }
    public int deleteRole(int itemroleid)
    {

        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        expdata.sqlexecute.Parameters.AddWithValue("@itemroleID", itemroleid);
        expdata.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
        expdata.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
        expdata.ExecuteProc("DeleteItemRole");
        int returnvalue = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
        expdata.sqlexecute.Parameters.Clear();

        if (returnvalue == -1)
        {
            return -1;
        }

        this.ClearCacheForAffectedEmployees(itemroleid);

        cItemRole itemRole = getItemRoleById(itemroleid);
        itemRoles.Remove(itemroleid);

        CurrentUser currentUser = cMisc.GetCurrentUser();
        cAuditLog clsaudit = new cAuditLog(accountid, currentUser.EmployeeID);

        if (itemRole != null)
        {
            clsaudit.deleteRecord(SpendManagementElement.ItemRoles, itemroleid, itemRole.rolename);
        }

        this.InvalidateCache();

        return 0;
    }

    private void ClearCacheForAffectedEmployees(int itemroleid)
    {
        var cache = new Utilities.DistributedCaching.Cache();
        var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
        var sql = "SELECT employeeid from dbo.employee_roles where itemroleid = @itemroleid";
        var result = new List<int>();
        expdata.sqlexecute.Parameters.AddWithValue("@itemroleid", itemroleid);
        using (var reader = expdata.GetReader(sql))
        {
            while (reader.Read())
            {
                var employeeId = reader.GetInt32(0);
                cache.Delete(this.accountid, EmployeeSubCategories.CacheArea, employeeId.ToString());
                cache.Delete(this.accountid, EmployeeItemRoles.CacheArea, employeeId.ToString());
            }

            reader.Close();
        }
    }


    public DataSet getGrid()
    {
        DataSet ds = new DataSet();
        DataTable tbl = new DataTable();
        object[] values;
        tbl.Columns.Add("itemroleid", typeof(System.Int32));
        tbl.Columns.Add("rolename", typeof(System.String));
        tbl.Columns.Add("description", typeof(System.String));

        SortedList<string, cItemRole> sorted = sortList();

        foreach (cItemRole role in sorted.Values)
        {
            values = new object[3];
            values[0] = role.itemroleid;
            values[1] = role.rolename;
            values[2] = role.description;
            tbl.Rows.Add(values);
        }

        ds.Tables.Add(tbl);
        return ds;
    }

    public bool alreadyExists(int itemroleid, string rolename)
    {
        foreach (cItemRole role in itemRoles.Values)
        {
            if (itemroleid == 0)
            {
                if (role.rolename.ToLower().Trim() == rolename.ToLower().Trim())
                {
                    return true;
                }
            }
            else
            {
                if (role.itemroleid != itemroleid && role.rolename.ToLower().Trim() == rolename.ToLower().Trim())
                {
                    return true;
                }
            }
        }
        return false;
    }
    public int addRole(string rolename, string description, List<cRoleSubcat> items, int userid)
    {
        int roleid = 0;

        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        int count;
        var strsql = "select count(*) from item_roles where rolename = @rolename";

        expdata.sqlexecute.Parameters.AddWithValue("@rolename", rolename);
        count = expdata.getcount(strsql);
        if (count > 0)
        {
            expdata.sqlexecute.Parameters.Clear();
            return -1;
        }
        if (description.Length > 4000)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@description", description.Substring(0, 3999));
        }
        else
        {
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
        }

        expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
        expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
        expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
        expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
        strsql = "insert into item_roles (rolename, description, createdon, createdby) " +
            "values (@rolename,@description, @createdon, @createdby);select @identity = @@identity";
        expdata.ExecuteSQL(strsql);
        roleid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
        expdata.sqlexecute.Parameters.Clear();
        addRoleSubcats(roleid, items);
        cItemRole role = new cItemRole(roleid, rolename, description, new Dictionary<int, cRoleSubcat>(), DateTime.Now.ToUniversalTime(), userid, new DateTime(1900, 01, 01), 0);
        if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
        {
            itemRoles.Add(roleid, role);
        }
        cAuditLog clsaudit = new cAuditLog(accountid, userid);
        clsaudit.addRecord(SpendManagementElement.ItemRoles, rolename, roleid);

        this.InvalidateCache();
        return roleid;
    }

    public int updateRole(int itemroleid, string rolename, string description, List<cRoleSubcat> items, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        var strsql = "select count(*) from item_roles where rolename = @rolename and itemroleid <> @itemroleid";

        //Used for audit log
        cItemRole oldItemRole = getItemRoleById(itemroleid);

        expdata.sqlexecute.Parameters.AddWithValue("@rolename", rolename);
        expdata.sqlexecute.Parameters.AddWithValue("@itemroleid", itemroleid);
        int count = expdata.getcount(strsql);
        if (count > 0)
        {
            expdata.sqlexecute.Parameters.Clear();
            return -1;
        }
        strsql = "update item_roles set rolename = @rolename, description = @description, modifiedon = @modifiedon, modifiedby = @modifiedby where itemroleid = @itemroleid";

        if (description.Length > 4000)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@description", description.Substring(0, 3999));
        }
        else
        {
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
        }

        expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        addRoleSubcats(itemroleid, items);
        expdata.ExecuteProc("clearDisallowedAddItems");

        #region Audit Log

        cAuditLog clsaudit = new cAuditLog(accountid, userid);

        if (oldItemRole.rolename != rolename)
        {
            clsaudit.editRecord(itemroleid, rolename, SpendManagementElement.ItemRoles, new Guid("54825039-9125-4705-b2d4-eb340d1d30de"), oldItemRole.rolename, rolename);
        }

        if (oldItemRole.description != description)
        {
            clsaudit.editRecord(itemroleid, rolename, SpendManagementElement.ItemRoles, new Guid("dcc4c3e7-1ed8-40b9-94bc-f5c52897fd86"), oldItemRole.description, description);
        }


        #endregion

        this.InvalidateCache();
        return itemroleid;
    }

    private void addRoleSubcats(int itemroleid, List<cRoleSubcat> items)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        deleteRoleSubcats(itemroleid);
        var strsql = "";
        foreach (cRoleSubcat rolesub in items)
        {
            strsql += "insert into rolesubcats (roleid, subcatid, maximum, receiptmaximum, isadditem) " +
            "values (" + itemroleid + "," + rolesub.SubcatId + "," + rolesub.maximum + "," + rolesub.receiptmaximum + "," + Convert.ToByte(rolesub.isadditem) + ");";
        }

        if (strsql != "")
        {
            expdata.ExecuteSQL(strsql);
            deleteItemFromAddItems(itemroleid);
        }

        this.InvalidateCache();
    }

    private void deleteItemFromAddItems(int itemroleid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        var strsql = "delete from additems where employeeid in (select employeeid from employee_roles where itemroleid = @itemroleid) and subcatid not in (select subcatid from rolesubcats where roleid = @itemroleid)";
        expdata.sqlexecute.Parameters.AddWithValue("@itemroleid", itemroleid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
        this.ClearCacheForAffectedEmployees(itemroleid);
    }

    public void deleteRolesubcatsBySubcatid(int subcatid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        var strsql = "delete from rolesubcats where subcatid = @subcatid";
        expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        foreach (cItemRole role in itemRoles.Values)
        {
            role.deleteSubcat(subcatid);
        }

        this.InvalidateCache();
    }

    public void saveRoleSubcat(cRoleSubcat rolesubcat)
    {
        CurrentUser currentUser = cMisc.GetCurrentUser();
        DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));

        data.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
        if (currentUser.isDelegate == true)
        {
            data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
        }
        else
        {
            data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
        }
        data.sqlexecute.Parameters.AddWithValue("@roleid", rolesubcat.roleid);
        data.sqlexecute.Parameters.AddWithValue("@subcatid", rolesubcat.SubcatId);
        data.sqlexecute.Parameters.AddWithValue("@maximum", rolesubcat.maximum);
        data.sqlexecute.Parameters.AddWithValue("@receiptmaximum", rolesubcat.receiptmaximum);
        data.sqlexecute.Parameters.AddWithValue("@isadditem", Convert.ToByte(rolesubcat.isadditem));
        data.ExecuteProc("saveRoleSubcat");
        data.sqlexecute.Parameters.Clear();
        this.InvalidateCache();
    }

    public void deleteRoleSubcat(int subcatid, int roleid)
    {
        CurrentUser currentUser = cMisc.GetCurrentUser();
        DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));

        data.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
        if (currentUser.isDelegate == true)
        {
            data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
        }
        else
        {
            data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
        }
        data.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
        data.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
        data.ExecuteProc("deleteRoleSubcat");
        data.sqlexecute.Parameters.Clear();

        this.InvalidateCache();
    }
    private void deleteRoleSubcats(int roleid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        expdata.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
        var strsql = "delete from rolesubcats where roleid = @roleid";
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
    }

    public System.Data.DataSet getRoleGrid(cSubcat subcat)
    {
        DataSet ds = new DataSet();
        DataTable tbl = new DataTable();
        object[] values;
        tbl.Columns.Add("roleid", typeof(System.Int32));
        tbl.Columns.Add("rolename", typeof(System.String));
        tbl.Columns.Add("receiptmaximum", typeof(System.Decimal));
        tbl.Columns.Add("maximum", typeof(System.Decimal));

        foreach (cItemRole role in itemRoles.Values)
        {
            foreach (cRoleSubcat rolesub in role.items.Values)
            {
                if (rolesub.SubcatId == subcat.subcatid)
                {
                    values = new object[4];
                    values[0] = role.itemroleid;
                    values[1] = role.rolename;
                    values[2] = rolesub.receiptmaximum;
                    values[3] = rolesub.maximum;
                    tbl.Rows.Add(values);
                }
            }
        }

        ds.Tables.Add(tbl);
        return ds;
    }

    public Dictionary<int, cItemRole> getModifiedItemRoles(DateTime date)
    {
        Dictionary<int, cItemRole> lst = new Dictionary<int, cItemRole>();
        foreach (cItemRole val in itemRoles.Values)
        {
            if (val.createdon > date || val.modifiedon > date)
            {
                lst.Add(val.itemroleid, val);
            }
        }
        return lst;
    }

    public List<int> getItemroleIds()
    {
        List<int> ids = new List<int>();
        foreach (cItemRole val in itemRoles.Values)
        {
            ids.Add(val.itemroleid);
        }
        return ids;
    }
}






