using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using expenses;
using expenses.Old_App_Code;
using System.Web.Caching;
using ExpensesLibrary;
using System.Collections;
using SpendManagementLibrary;
using Spend_Management;
/// <summary>
/// Summary description for cItemRoles
/// </summary>
public class cItemRoles
{
    string strsql;
    int nAccountid = 0;

    Dictionary<int, cItemRole> lstitems;
    System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

    public cItemRoles(int accountid)
    {
        nAccountid = accountid;
        
        InitialiseData();
    }

    private int accountid
    {
        get { return nAccountid; }
    }

    private void InitialiseData()
    {
        lstitems = (Dictionary<int, cItemRole>)Cache["itemroles" + accountid];
        if (lstitems == null)
        {
            lstitems = CacheList();
        }
        
    }

    private Dictionary<int, cItemRole> CacheList()
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        int itemroleid;
        string rolename, description;
        DateTime createdon, modifiedon;
        int createdby, modifiedby;
        Dictionary<int, cItemRole> list = new Dictionary<int, cItemRole>();
        System.Data.SqlClient.SqlDataReader reader;
        cItemRole newrole;
        strsql = "select  itemroleid, rolename, description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.item_roles";
        
        expdata.sqlexecute.CommandText = strsql;
        SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
        reader = expdata.GetReader(strsql);
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
            newrole = new cItemRole(itemroleid, rolename, description, getSubcatDetails(itemroleid), createdon, createdby, modifiedon, modifiedby);
            list.Add(itemroleid, newrole);
        }
        reader.Close();

        Cache.Insert("itemroles" + accountid, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        return list;
    }

    public Dictionary<int, cRoleSubcat> getSubcatDetails(int roleid)
    {
        Dictionary<int, cRoleSubcat> list = new Dictionary<int, cRoleSubcat>();
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Data.SqlClient.SqlDataReader reader;
        string strsql;
        cRoleSubcat clssubcat;
        cSubcats clssubcats = new cSubcats(accountid);
        int rolesubcatid, subcatid;
        decimal maximum, receiptmaximum;
        bool isadditem;
        strsql = "select * from rolesubcats where roleid = @roleid";
        expdata.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
        reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();
        while (reader.Read())
        {
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
            clssubcat = new cRoleSubcat(rolesubcatid, roleid, clssubcats.getSubcatById(subcatid), maximum, receiptmaximum, isadditem);
            list.Add(subcatid, clssubcat);
        }
        reader.Close();

        return list;

    }
    public cItemRole getItemRoleById(int id)
    {
        cItemRole role = null;
        lstitems.TryGetValue(id, out role);
        return role;
        
    }

    private SortedList<string, cItemRole> sortList()
    {
        SortedList<string, cItemRole> sorted = new SortedList<string, cItemRole>();

        foreach (cItemRole role in lstitems.Values)
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
    public void deleteRole(int itemroleid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "delete from employee_roles where itemroleid = @itemroleid";
        expdata.sqlexecute.Parameters.AddWithValue("@itemroleid", itemroleid);
        expdata.ExecuteSQL(strsql);

        strsql = "delete from item_roles where itemroleid = @itemroleid";
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        lstitems.Remove(itemroleid);
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
        foreach (cItemRole role in lstitems.Values)
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
    public int addRole(string rolename, string description, int userid)
    {
        int roleid = 0;

        //CurrentUser user = new CurrentUser();
        //user = cMisc.getCurrentUser(System.Web.HttpContext.Current.User.Identity.Name);

        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        
        expdata.sqlexecute.Parameters.AddWithValue("@rolename", rolename);
        expdata.sqlexecute.Parameters.AddWithValue("@description", description);
        
        expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
        expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
        expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
        expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
        strsql = "insert into item_roles (rolename, description, createdon, createdby) " +
            "values (@rolename,@description, @createdon, @createdby);select @identity = @@identity";
        expdata.ExecuteSQL(strsql);
        roleid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
        expdata.sqlexecute.Parameters.Clear();

        cItemRole role = new cItemRole(roleid, rolename, description, new Dictionary<int, cRoleSubcat>(), DateTime.Now.ToUniversalTime(), userid, new DateTime(1900,01,01), 0);
        lstitems.Add(roleid, role);
        cAuditLog clsaudit = new cAuditLog(accountid, userid);
        clsaudit.addRecord("Item Role", rolename);
        
        return roleid;
    }

    public void updateRole(int itemroleid, string rolename, string description, List<cRoleSubcat> items, int userid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "update item_roles set rolename = @rolename, description = @description, modifiedon = @modifiedon, modifiedby = @modifiedby where itemroleid = @itemroleid";
        expdata.sqlexecute.Parameters.AddWithValue("@rolename", rolename);
        expdata.sqlexecute.Parameters.AddWithValue("@description", description);
        expdata.sqlexecute.Parameters.AddWithValue("@itemroleid", itemroleid);
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
        expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();

        addRoleSubcats(itemroleid,items);

        cItemRole oldrole = getItemRoleById(itemroleid);
        if (lstitems[itemroleid] != null)
        {
            lstitems[itemroleid] = new cItemRole(itemroleid, rolename, description, getSubcatDetails(itemroleid), oldrole.createdon, oldrole.createdby, DateTime.Now.ToUniversalTime(),userid);
        }

    }

    private void addRoleSubcats(int itemroleid, List<cRoleSubcat> items)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        deleteRoleSubcats(itemroleid);
        strsql = "";
        foreach (cRoleSubcat rolesub in items)
        {
            strsql += "insert into rolesubcats (roleid, subcatid, maximum, receiptmaximum, isadditem) " +
            "values (" + itemroleid + "," + rolesub.subcat.subcatid + "," + rolesub.maximum + "," + rolesub.receiptmaximum + "," + Convert.ToByte(rolesub.isadditem) + ");";
        }

        if (strsql != "")
        {
            expdata.ExecuteSQL(strsql);
            deleteItemFromAddItems(itemroleid);
        }
    }

    private void deleteItemFromAddItems(int itemroleid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "delete from additems where employeeid in (select employeeid from employee_roles where itemroleid = @itemroleid) and subcatid not in (select subcatid from rolesubcats where roleid = @itemroleid)";
        expdata.sqlexecute.Parameters.AddWithValue("@itemroleid", itemroleid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
    }
    private void deleteRolesubcats(int itemroleid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "delete from rolesubcats where roleid = @roleid";
        expdata.sqlexecute.Parameters.AddWithValue("@roleid", itemroleid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
    }

    public void deleteRolesubcatsBySubcatid(int subcatid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        strsql = "delete from rolesubcats where subcatid = @subcatid";
        expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
        
        foreach (cItemRole role in lstitems.Values)
        {
            role.deleteSubcat(subcatid);    
        }
    }

    public void addRoleSubcat(cRoleSubcat rolesubcat, int roleid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        expdata.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
        expdata.sqlexecute.Parameters.AddWithValue("@subcatid", rolesubcat.subcat.subcatid);
        expdata.sqlexecute.Parameters.AddWithValue("@maximum", rolesubcat.maximum);
        expdata.sqlexecute.Parameters.AddWithValue("@receiptmaximum", rolesubcat.receiptmaximum);
        
        expdata.sqlexecute.Parameters.AddWithValue("@isadditem", Convert.ToByte(rolesubcat.isadditem));

        strsql = "insert into rolesubcats (roleid, subcatid, maximum, receiptmaximum, isadditem) " +
            "values (@roleid,@subcatid,@maximum,@receiptmaximum,@isadditem)";
        expdata.ExecuteSQL(strsql);
        expdata.sqlexecute.Parameters.Clear();
    }

    private void deleteRoleSubcats(int roleid)
    {
        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        expdata.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
        strsql = "delete from rolesubcats where roleid = @roleid";
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

        foreach (cItemRole role in lstitems.Values)
        {
            foreach (cRoleSubcat rolesub in role.items.Values)
            {
                if (rolesub.subcat.subcatid == subcat.subcatid)
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
        Dictionary<int, cItemRole> lst = new Dictionary<int,cItemRole>();
        foreach (cItemRole val in lstitems.Values)
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
        foreach (cItemRole val in lstitems.Values)
        {
            ids.Add(val.itemroleid);
        }
        return ids;
    }

    public Dictionary<int, cItemRole> itemRoles
    {
        get { return lstitems; }
    }

    public cItemRole getItemRoleByName(string name)
    {
        cItemRole reqRole = null;
        foreach (cItemRole val in lstitems.Values)
        {
            if (val.rolename == name)
            {
                reqRole = val;
                break;
            }
        }
        return reqRole;
    }
}



   


