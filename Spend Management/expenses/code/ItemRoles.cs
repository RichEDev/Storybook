using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using SpendManagementLibrary.Employees;

using SpendManagementLibrary;
using Spend_Management;
using Utilities.DistributedCaching;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;
/// <summary>
/// Summary description for cItemRoles
/// </summary>
public class ItemRoles
{
    Dictionary<int, ItemRole> _lstitems;

    /// <summary>
    /// The cache area.
    /// </summary>
    public const string CacheArea = "itemroles";

    /// <summary>
    /// The caching object .
    /// </summary>
    private readonly Cache _caching = new Cache();

    /// <summary>
    /// Contsructor for cItemRoles
    /// </summary>
    /// <param name="accountid"></param>
    public ItemRoles(int accountid)
    {
        this.AccountId = accountid;
        this.InitialiseData();
    }

    /// <summary>
    /// Gets a dictionary of the item roles with the itemRoleId as the key
    /// </summary>
    public Dictionary<int, ItemRole> List
    {
        get
        {
            if (this._lstitems == null)
            {
                this.InitialiseData();
            }


            return this._lstitems;
        }
    }

    private int AccountId { get; }


    private void InitialiseData()
    {
        this._lstitems = this._caching.Get(this.AccountId, CacheArea, "0") as Dictionary<int, ItemRole>
                            ?? this.CacheList();

    }

    private Dictionary<int, ItemRole> CacheList(IDBConnection connection = null)
    {
        Dictionary<int, ItemRole> list = new Dictionary<int, ItemRole>();

        SortedList<int, Dictionary<int, RoleSubcat>> lstSubcats = this.GetSubcatDetails();


        var strsql = "select  itemroleid, rolename, description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.item_roles";

        using (var databaseConnection =
            connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
        {
            using (IDataReader reader = databaseConnection.GetReader(strsql))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    var itemroleid = reader.GetInt32(reader.GetOrdinal("itemroleid"));
                    var rolename = reader.GetString(reader.GetOrdinal("rolename"));
                    var description = reader.IsDBNull(reader.GetOrdinal("description")) == false
                        ? reader.GetString(reader.GetOrdinal("description"))
                        : "";
                    var createdon = reader.IsDBNull(reader.GetOrdinal("createdon"))
                        ? new DateTime(1900, 01, 01)
                        : reader.GetDateTime(reader.GetOrdinal("createdon"));
                    var createdby = reader.IsDBNull(reader.GetOrdinal("createdby"))
                        ? 0
                        : reader.GetInt32(reader.GetOrdinal("createdby"));

                    var modifiedon = reader.IsDBNull(reader.GetOrdinal("modifiedon"))
                        ? new DateTime(1900, 01, 01)
                        : reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    var modifiedby = reader.IsDBNull(reader.GetOrdinal("modifiedby"))
                        ? 0
                        : reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    lstSubcats.TryGetValue(itemroleid, out var subcats);
                    if (subcats == null)
                    {
                        subcats = new Dictionary<int, RoleSubcat>();
                    }

                    var newrole = new ItemRole(itemroleid, rolename, description, subcats, createdon, createdby,
                        modifiedon, modifiedby);
                    list.Add(itemroleid, newrole);
                }

                reader.Close();
            }
        }

        this._caching.Add(this.AccountId, CacheArea, "0", list);

        return list;
    }

    /// <summary>
    /// The invalidate cache.
    /// </summary>
    private void InvalidateCache()
    {
        this._caching.Delete(this.AccountId, CacheArea, "0");
        this._lstitems = null;
    }

    private SortedList<int, Dictionary<int, RoleSubcat>> GetSubcatDetails(IDBConnection connection = null)
    {
        SortedList<int, Dictionary<int, RoleSubcat>> subcats = new SortedList<int, Dictionary<int, RoleSubcat>>();

        using (var databaseConnection =
            connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
        {
            var strsql = "select * from rolesubcats";

            using (IDataReader reader = databaseConnection.GetReader(strsql))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    var roleid = reader.GetInt32(reader.GetOrdinal("roleid"));
                    var rolesubcatid = reader.GetInt32(reader.GetOrdinal("rolesubcatid"));
                    var subcatid = reader.GetInt32(reader.GetOrdinal("subcatid"));
                    var isadditem = reader.GetBoolean(reader.GetOrdinal("isadditem"));
                    var maximumLimitWithReceipt = reader.IsDBNull(reader.GetOrdinal("maximum")) == false
                        ? reader.GetDecimal(reader.GetOrdinal("maximum"))
                        : 0;
                    var maximumLimitWithoutReceipt = reader.IsDBNull(reader.GetOrdinal("receiptmaximum")) == false
                        ? reader.GetDecimal(reader.GetOrdinal("receiptmaximum"))
                        : 0;
                    subcats.TryGetValue(roleid, out var list);

                    if (list == null)
                    {
                        list = new Dictionary<int, RoleSubcat>();
                        subcats.Add(roleid, list);
                    }

                    var clssubcat = new RoleSubcat(rolesubcatid, roleid, subcatid, maximumLimitWithoutReceipt, maximumLimitWithReceipt, isadditem);

                    list.Add(subcatid, clssubcat);

                }

                reader.Close();

            }
        }

        return subcats;
    }

    /// <summary>
    /// Gets an instance of an item role for the id provided
    /// </summary>
    /// <param name="id">The id of the item role you would like to retrieve</param>
    /// <returns></returns>
    public ItemRole GetItemRoleById(int id)
    {
        this.List.TryGetValue(id, out var role);
        return role;
    }

    /// <summary>
    /// Gets an instance of a ItemRole based on the name of the role
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ItemRole GetItemRoleByName(string name)
    {
        foreach (ItemRole role in this.List.Values)
        {
            if (role.Rolename == name)
            {
                return role;
            }
        }
        return null;
    }

    /// <summary>
    /// Provides a list of ListItems used to create a drop down list
    /// </summary>
    /// <param name="selectedItemRole">The id of the role you would like selected when the list is populated</param>
    /// <param name="includeNone">Whether to include a [None] option </param>
    /// <returns></returns>
    public List<ListItem> CreateDropDown(int selectedItemRole, bool includeNone)
    {
        List<ListItem> items = new List<ListItem>();

        SortedList<string, ItemRole> roles = this.SortList();

        if (includeNone)
        {
            items.Add(new ListItem("[None]", "0"));
        }

        foreach (ItemRole role in roles.Values)
        {
            var tmpListItem = new ListItem(role.Rolename, role.ItemRoleId.ToString());
            if (role.ItemRoleId == selectedItemRole)
            {
                tmpListItem.Selected = true;
            }
            items.Add(tmpListItem);
        }

        return items;
    }

    private SortedList<string, ItemRole> SortList()
    {
        SortedList<string, ItemRole> sorted = new SortedList<string, ItemRole>();

        foreach (ItemRole role in this.List.Values)
        {
            if (sorted.ContainsKey(role.Rolename) == false)
            {
                sorted.Add(role.Rolename, role);
            }
        }
        return sorted;
    }

    /// <summary>
    /// Returns a SortedList of item roles sorted by rolename
    /// </summary>
    /// <returns></returns>
    public SortedList<string, ItemRole> GetSortedList()
    {
        return this.SortList();
    }

    /// <summary>
    /// Deletes an item role
    /// </summary>
    /// <param name="itemroleid">The id of the item role you wish to delete</param>
    /// <param name="connection"></param>
    /// <returns></returns>
    public int DeleteRole(int itemroleid, IDBConnection connection = null)
    {

        using (var databaseConnection =
            connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
        {
            databaseConnection.sqlexecute.Parameters.AddWithValue("@itemroleID", itemroleid);
            databaseConnection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
            databaseConnection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
            databaseConnection.ExecuteProc("DeleteItemRole");
            int returnvalue = (int)databaseConnection.sqlexecute.Parameters["@returnvalue"].Value;
            databaseConnection.sqlexecute.Parameters.Clear();

            if (returnvalue == -1)
            {
                return -1;
            }

            this.ClearCacheForAffectedEmployees(itemroleid);

            ItemRole itemRole = this.GetItemRoleById(itemroleid);
            this.List.Remove(itemroleid);

            CurrentUser currentUser = cMisc.GetCurrentUser();
            cAuditLog clsaudit = new cAuditLog(this.AccountId, currentUser.EmployeeID);

            if (itemRole != null)
            {
                clsaudit.deleteRecord(SpendManagementElement.ItemRoles, itemroleid, itemRole.Rolename);
            }

            this.InvalidateCache();
        }

        return 0;
    }

    private void ClearCacheForAffectedEmployees(int itemroleid, IDBConnection connection = null)
    {

        using (var databaseConnection =
            connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
        {
            var cache = new Cache();
            var sql = "SELECT employeeid from dbo.employee_roles where itemroleid = @itemroleid";
            databaseConnection.sqlexecute.Parameters.AddWithValue("@itemroleid", itemroleid);
            using (var reader = databaseConnection.GetReader(sql))
            {
                while (reader.Read())
                {
                    var employeeId = reader.GetInt32(0);
                    cache.Delete(this.AccountId, EmployeeSubCategories.CacheArea, employeeId.ToString());
                    cache.Delete(this.AccountId, EmployeeItemRoles.CacheArea, employeeId.ToString());
                }

                reader.Close();
            }
        }
    }

    /// <summary>
    /// Adds or updates an item role
    /// </summary>
    /// <param name="role">The instance of the role to add or update</param>
    /// <param name="currentUser">The current user executing the command</param>
    /// <param name="connection">The database connection to use if not the default</param>
    /// <returns></returns>
    public int SaveRole(ItemRole role, ICurrentUserBase currentUser, IDBConnection connection = null)
    {
        using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
        {
            databaseConnection.AddWithValue("@itemRoleId", role.ItemRoleId);
            databaseConnection.AddWithValue("@roleName", role.Rolename);
            if (string.IsNullOrWhiteSpace(role.Description))
            {
                databaseConnection.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                databaseConnection.AddWithValue("@description",
                    role.Description.Length > 4000 ? role.Description.Substring(0, 3999) : role.Description);
            }

            if (currentUser != null)
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            databaseConnection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
            databaseConnection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
            databaseConnection.ExecuteProc("SaveItemRole");
            int returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnvalue"].Value;
            databaseConnection.sqlexecute.Parameters.Clear();

            if (returnValue > 0)
            {
                this.InvalidateCache();
            }

            return returnValue;
        }
    }

    /// <summary>
    /// Deletes all rolesubcat records for a given expense item
    /// </summary>
    /// <param name="subcatid">The id of the expense item to delete the rolesubcats for</param>
    /// <param name="connection">The database connection to use if not the default</param>
    public void DeleteRolesubcatsBySubcatid(int subcatid, IDBConnection connection = null)
    {

        using (
            var databaseConnection = connection
                                     ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
        {
            var strsql = "delete from rolesubcats where subcatid = @subcatid";
            databaseConnection.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            databaseConnection.ExecuteSQL(strsql);
            databaseConnection.sqlexecute.Parameters.Clear();

            foreach (ItemRole role in this.List.Values)
            {
                role.DeleteSubcat(subcatid);
            }

            this.InvalidateCache();
        }
    }

    /// <summary>
    /// Saves an item role to expense item association (rolesubcat)
    /// </summary>
    /// <param name="rolesubcat">An instance of a rolesubcat to save</param>
    /// <param name="connection">The database connection to use if not the default</param>
    public void SaveRoleSubcat(RoleSubcat rolesubcat, IDBConnection connection = null)
    {
        CurrentUser currentUser = cMisc.GetCurrentUser();

        using (
            var databaseConnection = connection
                                     ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
        {
            databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate)
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            databaseConnection.sqlexecute.Parameters.AddWithValue("@roleid", rolesubcat.RoleId);
            databaseConnection.sqlexecute.Parameters.AddWithValue("@subcatid", rolesubcat.SubcatId);
            databaseConnection.sqlexecute.Parameters.AddWithValue("@maximum", rolesubcat.MaximumLimitWithReceipt);
            databaseConnection.sqlexecute.Parameters.AddWithValue("@receiptmaximum", rolesubcat.MaximumLimitWithoutReceipt);
            databaseConnection.sqlexecute.Parameters.AddWithValue("@isadditem", Convert.ToByte(rolesubcat.IsAddItem));
            databaseConnection.ExecuteProc("SaveRoleSubcat");
            databaseConnection.sqlexecute.Parameters.Clear();
            this.InvalidateCache();
        }
    }

    /// <summary>
    /// Deletes a item role to expense item association (rolesubcat(
    /// </summary>
    /// <param name="subcatid">The id of the expense item to delete the assocation from</param>
    /// <param name="roleid">The id of the role to delete the assocation from</param>
    /// <param name="connection">The database connection to use if not the default</param>
    public void DeleteRoleSubcat(int subcatid, int roleid, IDBConnection connection = null)
    {
        CurrentUser currentUser = cMisc.GetCurrentUser();
        
        using (
            var databaseConnection = connection
                                     ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
        {
            databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate)
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            databaseConnection.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            databaseConnection.sqlexecute.Parameters.AddWithValue("@roleid", roleid);
            databaseConnection.ExecuteProc("DeleteRoleSubcat");
            databaseConnection.sqlexecute.Parameters.Clear();

            this.InvalidateCache();
        }
    }

    /// <summary>
    /// Gets an instance of a RoleSubcat
    /// </summary>
    /// <param name="itemRoleId">The id of the item role the rolesubcat belongs to</param>
    /// <param name="roleSubcatId">The id of the rolesubcat to retrieve</param>
    /// <returns></returns>
    public RoleSubcat GetRoleSubcatById(int itemRoleId, int roleSubcatId)
    {
        ItemRole role = this.GetItemRoleById(itemRoleId);
        return role?.GetRoleSubcatById(roleSubcatId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">The rolesubcatid of the rolesubcat to delete</param>
    /// <param name="currentUser">The current user executing the command</param>
    /// <param name="connection">The database connection to use if not the default</param>
    public void DeleteExpenseItemToItemRoleAssociation(int id, ICurrentUserBase currentUser, IDBConnection connection = null)
    {
        using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
        {
            databaseConnection.AddWithValue("@rolesubcatid", id);
            databaseConnection.ExecuteProc("DeleteExpenseItemToItemRoleAssociation");
            databaseConnection.sqlexecute.Parameters.Clear();
        }
    }
    
}






