using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Caching;
using System.Configuration;
using SpendManagementLibrary;
using Spend_Management;
using System.Data.SqlClient;

namespace Spend_Management
{
    /// <summary>
    /// cSuppliers class
    /// </summary>
    public class cSuppliers
    {
        string strsql;
        private int nAccountId;
        private int nSubAccountId;

        System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

		/// <summary>
		/// cSuppliers: Constructor for supplier class
		/// </summary>
		/// <param name="accountid">Customer account ID</param>
        /// <param name="subaccountid">Database partition subaccount ID</param>
        public cSuppliers(int accountid, int subaccountid)
        {
            nAccountId = accountid;
            nSubAccountId = subaccountid;
		}

        /// <summary>
        /// Parameterless constructor to be used only by cGridNew
        /// </summary>
        public cSuppliers()
        {

        }

        #region properties
		/// <summary>
		/// AccountID: Current customer account id
		/// </summary>
		public int AccountID
        {
            get { return nAccountId; }
        }

		/// <summary>
		/// LocationID: Current database partition location id
		/// </summary>
        public int subAccountId
        {
            get { return nSubAccountId; }
		}
        private string cacheKey(int supplierId)
        {
            return "suppliers" + AccountID.ToString() + "_" + supplierId.ToString();
        }
		#endregion

		/// <summary>
		/// getSupplierById: Gets a supplier by its database ID
		/// </summary>
		/// <param name="supplierId">ID of supplier to be retrieved</param>
		/// <returns>Supplier details in the cSupplier class entity. Returns NULL if not found</returns>
		public cSupplier getSupplierById(int supplierId)
        {
            cSupplier retSupplier = (cSupplier)Cache[cacheKey(supplierId)];
            if (retSupplier == null)
            {
				DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

                cBaseDefinitions clsBaseDefs = null;

				cSupplierAddresses addresses = new cSupplierAddresses(AccountID);

                Dictionary<int, cSupplier> list = new Dictionary<int, cSupplier>();
                System.Data.SqlClient.SqlDataReader reader;
                AggregateCacheDependency aggdep = new AggregateCacheDependency();

                strsql = "SELECT supplierid, subaccountid, suppliername, suppliercode, primary_addressid, statusid, categoryid, annual_turnover, supplier_currency, numberofemployees, financial_ye, financial_statusid, weburl, supplierEmail, internalContact, financialStatusLastChecked, isSupplier, isReseller, createdon, createdby, modifiedon, modifiedby FROM dbo.supplier_details WHERE supplierid = @supplierId";
                db.sqlexecute.Parameters.AddWithValue("@supplierId", supplierId);
                db.sqlexecute.CommandText = strsql;

                SqlCacheDependency dep = null;
                if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                {
                    SortedList<string, object> depParams = new SortedList<string, object>();
                    depParams.Add("@supplierId", supplierId);
                    dep = db.CreateSQLCacheDependency("SELECT cacheExpiry FROM supplier_details where supplierId = @supplierId", depParams);
                }

                using (reader = db.GetReader(strsql))
                {
                    db.sqlexecute.Parameters.Clear();

                    SortedList<int, object> userdefined;
                    cUserdefinedFields ufields = new cUserdefinedFields(AccountID);
                    cTables tables = new cTables(AccountID);
                    cFields fields = new cFields(AccountID);
                    cTable table = tables.GetTableByName("supplier_details");
                    cTable udftable = tables.GetTableByID(table.UserDefinedTableID);

                    while (reader.Read())
                    {
                        int subaccId = reader.GetInt32(reader.GetOrdinal("subAccountId"));
                        string suppname = reader.GetString(reader.GetOrdinal("suppliername"));
                        cAddress address;
                        int primary_addrid = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("primary_addressid")))
                        {
                            primary_addrid = reader.GetInt32(reader.GetOrdinal("primary_addressid"));
                            address = addresses.getAddressById(primary_addrid);
                        }
                        else
                        {
                            address = new cAddress(primary_addrid, "", "", "", "", "", "", 0, "", "", false, DateTime.Now, 0, DateTime.Now, 0);
                        }
                        cSupplierCategory category = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("categoryid")))
                        {
                            clsBaseDefs = new cBaseDefinitions(AccountID, subAccountId, SpendManagementElement.SupplierCategory);
                            category = (cSupplierCategory)clsBaseDefs.GetDefinitionByID(reader.GetInt32(reader.GetOrdinal("categoryid")));
                        }

                        cSupplierStatus status = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("statusid")))
                        {
                            clsBaseDefs = new cBaseDefinitions(AccountID, subAccountId, SpendManagementElement.SupplierStatus);
                            status = (cSupplierStatus)clsBaseDefs.GetDefinitionByID(reader.GetInt32(reader.GetOrdinal("statusid")));
                        }
                        string suppliercode = "";
                        if (!reader.IsDBNull(reader.GetOrdinal("suppliercode")))
                        {
                            suppliercode = reader.GetString(reader.GetOrdinal("suppliercode"));
                        }
                        double turnover = reader.GetDouble(reader.GetOrdinal("annual_turnover"));
                        int? atci = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("supplier_currency")))
                        {
                            atci = reader.GetInt32(reader.GetOrdinal("supplier_currency"));
                        }

                        int numemployees = 0;

                        if (!reader.IsDBNull(reader.GetOrdinal("numberofemployees")))
                        {
                            numemployees = reader.GetInt32(reader.GetOrdinal("numberofemployees"));
                        }

                        string weburl = "";
                        if (!reader.IsDBNull(reader.GetOrdinal("weburl")))
                        {
                            weburl = reader.GetString(reader.GetOrdinal("weburl"));
                        }

                        short fye = 0;

                        if (!reader.IsDBNull(reader.GetOrdinal("financial_ye")))
                        {
                            fye = reader.GetInt16(reader.GetOrdinal("financial_ye"));
                        }

                        bool isSupplier = false;
                        if (!reader.IsDBNull(reader.GetOrdinal("isSupplier")))
                        {
                            isSupplier = reader.GetBoolean(reader.GetOrdinal("isSupplier"));
                        }

                        bool isReseller = false;
                        if (!reader.IsDBNull(reader.GetOrdinal("isReseller")))
                        {
                            isReseller = reader.GetBoolean(reader.GetOrdinal("isReseller"));
                        }

                        string suppEmail = "";
                        if (!reader.IsDBNull(reader.GetOrdinal("supplierEmail")))
                        {
                            suppEmail = reader.GetString(reader.GetOrdinal("supplierEmail"));
                        }
                        
                        string internalContact = "";
                        if (!reader.IsDBNull(reader.GetOrdinal("internalContact")))
                        {
                            internalContact = reader.GetString(reader.GetOrdinal("internalContact"));
                        }

                        DateTime? laststatuscheck = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("financialStatusLastChecked")))
                        {
                            laststatuscheck = reader.GetDateTime(reader.GetOrdinal("financialStatusLastChecked"));
                        }

                        cFinancialStatus lastfsid = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("financial_statusid")))
                        {
                            cBaseDefinitions clsBaseDefinitions = new cBaseDefinitions(AccountID, subAccountId, SpendManagementElement.FinancialStatus);
                            lastfsid = (cFinancialStatus)clsBaseDefinitions.GetDefinitionByID(reader.GetInt32(reader.GetOrdinal("financial_statusid")));
                        }
                        DateTime? createdon = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("createdon")))
                        {
                            createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                        }
                        int? createdby = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("createdby")))
                        {
                            createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                        }
                        DateTime? modifiedon = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                        {
                            modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                        }
                        int? modifiedby = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                        {
                            modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                        }
                        userdefined = ufields.GetRecord(udftable, supplierId, tables, fields);
                        cSupplierContacts contacts = new cSupplierContacts(AccountID, supplierId);
                        retSupplier = new cSupplier(supplierId, subaccId, suppname, status, category, suppliercode, address, weburl, fye, lastfsid, atci, turnover, numemployees, contacts.getContacts(), userdefined, laststatuscheck, internalContact, suppEmail, isSupplier, isReseller);

                        SqlCacheDependency contactdep = null;
                        if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                        {
                            strsql = "SELECT contactid, supplierid, contactname, position, email, mobile, business_addressid, home_addressid, comments, main_contact, createdon, createdby, modifiedon, modifiedby FROM dbo.supplier_contacts WHERE supplierid = @supplierId";
                            SortedList<string, object> contactparams = new SortedList<string, object>();
                            contactparams.Add("@supplierId", supplierId);
                            contactdep = db.CreateSQLCacheDependency(strsql, contactparams);
                        }

                        strsql = "SELECT addressid, address_title, addr_line1, addr_line2, town, county, postcode, countryid, switchboard, fax, private_address, createdon, createdby, modifiedon, modifiedby ";
                        strsql += "FROM dbo.supplier_addresses ";
                        strsql += "WHERE addressid = any ((select primary_addressid from supplier_details where supplierid = @supplierId) union ";
                        strsql += "(select business_addressid from supplier_contacts where supplierid = @supplierId) union ";
                        strsql += "(select home_addressid from supplier_contacts where supplierid = @supplierId) )";

                        SqlCacheDependency address_dep = null;
                        if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
                        {
                            SortedList<string, object> addressparams = new SortedList<string, object>();
                            addressparams.Add("@supplierId", supplierId);
                            address_dep = db.CreateSQLCacheDependency(strsql, addressparams);

                            aggdep.Add(new CacheDependency[] { address_dep, contactdep, dep });

                            if (category != null)
                            {
                                strsql = "SELECT categoryid, subAccountId, description, createdon, createdby, modifiedon, modifiedby FROM dbo.supplier_categories WHERE categoryid = @categoryId";
                                SortedList<string, object> catparams = new SortedList<string, object>();
                                catparams.Add("@categoryId", category.ID);
                                SqlCacheDependency category_dep = db.CreateSQLCacheDependency(strsql, catparams);
                                aggdep.Add(new SqlCacheDependency[] { category_dep });
                            }

                            if (status != null)
                            {
                                strsql = "SELECT statusid, subAccountId, description, sequence, deny_contract_add, createdon, createdby, modifiedon, modifiedby FROM dbo.supplier_status WHERE statusid = @statusId";
                                SortedList<string, object> statusparams = new SortedList<string, object>();
                                statusparams.Add("@statusId", status.ID);
                                SqlCacheDependency status_dep = db.CreateSQLCacheDependency(strsql, statusparams);
                                aggdep.Add(new SqlCacheDependency[] { status_dep });
                            }

                            Cache.Insert(cacheKey(supplierId), retSupplier, aggdep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), System.Web.Caching.CacheItemPriority.Default, null);
                        }
                    }
                    reader.Close();
                }
                db.sqlexecute.Parameters.Clear();
            }
            return retSupplier;
        }

        ///// <summary>
        ///// SupplierExists: Check if a supplier of a particular name already exists in the active database location
        ///// </summary>
        ///// <param name="supplier_name">Supplier name to check</param>
        ///// <returns>Returns "true" if supplier already exists, otherwise returns "false"</returns>
        //[Obsolete]
        //public bool SupplierExists(string supplier_name)
        //{
        //    bool exists = false;
        //    DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
        //    strsql = "SELECT COUNT(supplierid) FROM dbo.supplier_details WHERE suppliername = @suppliername";
        //    if (subAccountId >= 0)
        //    {
        //        strsql += " AND subAccountId = @subAccId";
        //        db.sqlexecute.Parameters.AddWithValue("@subAccId", subAccountId);
        //    }
        //    db.sqlexecute.CommandText = strsql;
        //    db.sqlexecute.Parameters.AddWithValue("@suppliername", supplier_name);

        //    int retCount = db.getcount(strsql);

        //    if (retCount > 0)
        //    {
        //        exists = true;
        //    }

        //    db.sqlexecute.Parameters.Clear();
        //    return exists;
        //}

        ///// <summary>
        ///// getSuppliersByStatus: Retrieve all suppliers with a particular status assigned
        ///// </summary>
        ///// <param name="status">Status of suppliers to retrieve</param>
        ///// <returns>Returns a Dictionary collection of supplierid and cSupplier class entities</returns>
        //[Obsolete]
        //public Dictionary<int, cSupplier> getSuppliersByStatus(cSupplierStatus status)
        //{
        //    Dictionary<int, cSupplier> retList = new Dictionary<int, cSupplier>();
        //    System.Collections.ArrayList ids = new System.Collections.ArrayList();

        //    DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

        //    strsql = "SELECT supplierid FROM dbo.supplier_details WHERE statusid = @statusId";
        //    db.sqlexecute.CommandText = strsql;
        //    db.sqlexecute.Parameters.AddWithValue("@statusId", status.ID);

        //    using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strsql))
        //    {
        //        while (reader.Read())
        //        {
        //            int id = reader.GetInt32(0);
        //            ids.Add(id);
        //        }
        //        reader.Close();
        //    }

        //    for (int x = 0; x < ids.Count; x++)
        //    {
        //        cSupplier supplier = getSupplierById((int)ids[x]);
        //        retList.Add(supplier.SupplierId, supplier);
        //    }
        //    return retList;
        //}

        ///// <summary>
        ///// getSuppliersByCategory: Retrieve all suppliers with a particular category assigned
        ///// </summary>
        ///// <param name="category">Category of suppliers to retrieve</param>
        ///// <returns>Returns a Dictionary collection of supplierid and cSupplier class entities</returns>
        //[Obsolete]
        //public Dictionary<int, cSupplier> getSuppliersByCategory(cSupplierCategory category)
        //{
        //    Dictionary<int, cSupplier> retList = new Dictionary<int, cSupplier>();
        //    System.Collections.ArrayList ids = new System.Collections.ArrayList();

        //    DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

        //    strsql = "SELECT supplierid FROM dbo.supplier_details WHERE categoryid = @catId";
        //    if (subAccountId >= 0)
        //    {
        //        strsql += " AND subAccountId = @subaccId";
        //        db.sqlexecute.Parameters.AddWithValue("@subaccId", subAccountId);
        //    }
        //    db.sqlexecute.CommandText = strsql;
        //    db.sqlexecute.Parameters.AddWithValue("@catId", category.ID);

        //    using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strsql))
        //    {
        //        while (reader.Read())
        //        {
        //            int id = reader.GetInt32(0);
        //            ids.Add(id);
        //        }
        //        reader.Close();
        //    }

        //    for (int x = 0; x < ids.Count; x++)
        //    {
        //        cSupplier supplier = getSupplierById((int)ids[x]);
        //        retList.Add(supplier.SupplierId, supplier);
        //    }
        //    return retList;
        //}

		/// <summary>
		/// UpdateSupplier: Update a particular supplier record
		/// </summary>
		/// <param name="supplier">Supplier record to be updated in the form of a cSupplier class entity</param>
		/// <returns>Database ID of supplier</returns>
        public int UpdateSupplier(cSupplier supplier)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            int retSupplierId = 0;
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cSupplierAddresses addresses = new cSupplierAddresses(AccountID);
            int addressid = addresses.UpdateAddress(supplier.PrimaryAddress, 0); // supplier id is zero because it is primary address
            supplier.PrimaryAddress.AddressId = addressid;

            db.sqlexecute.Parameters.Add("@ReturnId", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@ReturnId"].Direction = System.Data.ParameterDirection.ReturnValue;
            db.sqlexecute.Parameters.AddWithValue("@supplierid", supplier.SupplierId);
            db.sqlexecute.Parameters.AddWithValue("@userid", currentUser.EmployeeID);
            db.sqlexecute.Parameters.AddWithValue("@subaccountid", supplier.subAccountId);
            db.sqlexecute.Parameters.AddWithValue("@suppliername", supplier.SupplierName);
            if (supplier.PrimaryAddress == null || supplier.PrimaryAddress.AddressId == 0)
            {
                db.sqlexecute.Parameters.AddWithValue("@primary_addressid", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@primary_addressid", supplier.PrimaryAddress.AddressId);
            }
            db.sqlexecute.Parameters.AddWithValue("@suppliercode", supplier.SupplierCode);
            if (supplier.SupplierStatus == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@statusid", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@statusid", supplier.SupplierStatus.ID);
            }
            if (supplier.SupplierCategory == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@categoryid", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@categoryid", supplier.SupplierCategory.ID);
            }

            db.sqlexecute.Parameters.AddWithValue("@annual_turnover", supplier.AnnualTurnover);

            if (supplier.TurnoverCurrencyId == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@supplier_currency", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@supplier_currency", supplier.TurnoverCurrencyId);
            }
            db.sqlexecute.Parameters.AddWithValue("@numberofemployees", supplier.NumberOfEmployees);
            db.sqlexecute.Parameters.AddWithValue("@financial_ye", supplier.FinancialYearEnd);
            if (supplier.LastFinancialStatus == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@financial_statusid", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@financial_statusid", supplier.LastFinancialStatus.ID);
            }
            db.sqlexecute.Parameters.AddWithValue("@weburl", supplier.WebURL);

            db.sqlexecute.Parameters.AddWithValue("@supplierEmail", supplier.SupplierEmail);
            if (supplier.CreditRefChecked.HasValue)
            {
                db.sqlexecute.Parameters.AddWithValue("@financialStatusLastChecked", supplier.CreditRefChecked);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@financialStatusLastChecked", DBNull.Value);
            }
            db.sqlexecute.Parameters.AddWithValue("@internalContact", supplier.InternalContact);
            db.sqlexecute.Parameters.AddWithValue("@isSupplier", supplier.IsSupplier);
            db.sqlexecute.Parameters.AddWithValue("@isReseller", supplier.IsReseller);

            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            db.ExecuteProc("saveSupplier");
            retSupplierId = (int)db.sqlexecute.Parameters["@ReturnId"].Value;

                        
            if (retSupplierId > 0)
            {
                if (supplier.userdefined != null)
                {
                    cTables clstables = new cTables(currentUser.AccountID);
                    cTable tbl = clstables.GetTableByID(new Guid("299FF396-6947-4D46-A4DF-1983CD311A77"));
                    cUserdefinedFields clsuserdefined = new cUserdefinedFields(currentUser.AccountID);
                    clsuserdefined.SaveValues(clstables.GetTableByID(tbl.UserDefinedTableID), retSupplierId, supplier.userdefined, new cTables(AccountID), new cFields(AccountID), currentUser, elementId: (int)SpendManagementElement.SupplierDetails, record: supplier.SupplierName);
                }

                ResetCache(retSupplierId);
            }
            return retSupplierId;
        }

		/// <summary>
		/// DeleteSupplier: Permanently deletes a supplier from the database
		/// </summary>
		/// <param name="supplierId">Database ID of supplier to delete</param>
        public int DeleteSupplier(int supplierId)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser != null)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            db.sqlexecute.Parameters.AddWithValue("@supplierid", supplierId);
            db.sqlexecute.Parameters.Add("@retCode", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@retCode"].Direction = System.Data.ParameterDirection.ReturnValue;
            db.ExecuteProc("deleteSupplier");
            int retcode = (int)db.sqlexecute.Parameters["@retCode"].Value;

            db.sqlexecute.Parameters.Clear();

            ResetCache(supplierId);
            return retcode;
        }

		/// <summary>
		/// getSupplierByName: Retrieves a supplier record by name
		/// </summary>
		/// <param name="suppliername">Name of supplier to retrieve</param>
		/// <returns>Supplier record in the form of a cSupplier class entity. Returns NULL if not found</returns>
        public cSupplier getSupplierByName(string suppliername)
        {
            int supplierid = 0;
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            //string strsql = "SELECT supplierid FROM supplier_details WHERE suppliername = @suppliername AND (subaccountid = @subaccid OR subaccountid is null)";

            db.sqlexecute.Parameters.AddWithValue("@subAccountID", subAccountId);
            db.sqlexecute.Parameters.AddWithValue("@supplierName", suppliername);

            // Note: the stored procedure does not allow for NULL subaccountids, all should be updated

            //using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strsql))
            using (System.Data.SqlClient.SqlDataReader reader = db.GetStoredProcReader("GetSupplierByName"))
            {
                while (reader.Read())
                {
                    supplierid = reader.GetInt32(0);
                    break;
                }
                reader.Close();
            }
            cSupplier retSupplier = null;
            if (supplierid != 0)
            {
                retSupplier = getSupplierById(supplierid);
            }

            return retSupplier;
        }

        ///// <summary>
        ///// Returns the main SELECT sql statement for the class for use by cGrid
        ///// </summary>
        //[Obsolete]
        //public string getGridSQL
        //{
        //    get { return "select supplierid, subaccountid, suppliername, suppliercode, primary_addressid, statusid, categoryid, annual_turnover, supplier_currency, numberofemployees, financial_ye, financial_statusid, weburl, createdon, createdby, modifiedon, modifiedby from dbo.supplier_details"; }
        //}

        ///// <summary>
        ///// getListItems: Returns an array of supplier definitions for use in a drop down list
        ///// </summary>
        ///// <param name="includeNoneEntry">True to include a [None] selection item</param>
        ///// <returns>Array of list items</returns>
        //[Obsolete]
        //public ListItem[] getListItems(bool includeNoneEntry)
        //{
        //    List<ListItem> items = new List<ListItem>();
        //    DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

        //    string sql = "select supplierid, suppliername from dbo.supplier_details";
        //    if (subAccountId >= 0)
        //    {
        //        sql += " where [subaccountid] = @subaccId";
        //        db.sqlexecute.Parameters.AddWithValue("@subaccId", subAccountId);
        //    }

        //    sql += " order by suppliername";

        //    System.Data.SqlClient.SqlDataReader reader;
        //    using (reader = db.GetReader(sql))
        //    {
        //        while (reader.Read())
        //        {
        //            int supplierid = reader.GetInt32(0);
        //            string name = reader.GetString(1);

        //            ListItem item = new ListItem(name, supplierid.ToString());
        //            items.Add(item);
        //        }
        //        reader.Close();
        //    }
        //    db.sqlexecute.Parameters.Clear();

        //    if (includeNoneEntry)
        //    {
        //        ListItem blankItem = new ListItem("[None]", "0");
        //        items.Insert(0, blankItem);
        //    }

        //    return items.ToArray();
        //}

        /// <summary>
        /// getListItems: Returns an array of supplier definitions for use in a drop down list
        /// </summary>
        /// <param name="includeNoneEntry">True to include a [None] selection item</param>
        /// <returns>Array of list items</returns>
		public ListItem[] getListItemsForContractAdd(bool includeNoneEntry)
		{
			List<ListItem> items = new List<ListItem>();
			DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

            string sql = "select supplierid, suppliername from dbo.supplier_details left join supplier_status on supplier_status.statusid = supplier_details.statusid where (deny_contract_add <> 1 or supplier_details.statusid is null)";

			if (subAccountId >= 0)
			{
				sql += " and supplier_details.[subAccountId] = @subaccId";
				db.sqlexecute.Parameters.AddWithValue("@subaccId", subAccountId);
			}

			sql += " order by suppliername";

			System.Data.SqlClient.SqlDataReader reader;
            using (reader = db.GetReader(sql))
            {
                while (reader.Read())
                {
                    int supplierid = reader.GetInt32(reader.GetOrdinal("supplierid"));
                    string name = reader.GetString(reader.GetOrdinal("suppliername"));

                    ListItem item = new ListItem(name, supplierid.ToString());
                    items.Add(item);
                }
                reader.Close();
            }

            db.sqlexecute.Parameters.Clear();

			if (includeNoneEntry)
			{
				ListItem blankItem = new ListItem("[None]", "0");
				items.Insert(0, blankItem);
			}

			return items.ToArray();
		}

        /// <summary>
        /// Forces removal of supplier record from cache memory
        /// </summary>
        /// <param name="supplierId">Supplier ID to un-cache</param>
        public void ResetCache(int supplierId)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            db.sqlexecute.Parameters.AddWithValue("@supplierID", supplierId);
            //db.ExecuteSQL("update supplier_details set CacheExpiry = getdate() where supplierId = @supplierId");
            db.ExecuteProc("UpdateSupplierCacheExpiry");

            if (Cache[cacheKey(supplierId)] != null)
            {
                Cache.Remove(cacheKey(supplierId));
            }
        }

        /// <summary>
        /// Returns a list of supplier ids for the set sub accountid
        /// </summary>
        /// <returns>List of supplier id numbers</returns>
        public List<int> getSupplierIdsForSubAccount()
        {
            List<int> lstSupplierIds = new List<int>();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            SqlDataReader reader;

            strsql = "SELECT supplierid FROM dbo.supplier_details WHERE subaccountid = @subaccountid";
            db.sqlexecute.Parameters.AddWithValue("@subaccountid", subAccountId);
            db.sqlexecute.CommandText = strsql;

            using (reader = db.GetReader(strsql))
            {
                while (reader.Read())
                {
                    lstSupplierIds.Add(reader.GetInt32(1));
                }
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();
            return lstSupplierIds;
        }

        public string[] GetSuppliersGrid(int accountid, int employeeid, int subaccountid, bool enabledeleting)
        {
            cFields fields = new cFields(accountid);

            const string sql = "select supplierid, suppliername, supplier_categories.description, weburl from supplier_details";

            cGridNew supplierGrid = new cGridNew(accountid, employeeid, "suppliergrid", sql);

            supplierGrid.enablearchiving = false;
            supplierGrid.enabledeleting = enabledeleting;
            supplierGrid.enableupdating = true;  //curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.SupplierDetails, true);
            supplierGrid.CssClass = "datatbl";
            supplierGrid.editlink = "supplier_details.aspx?sid={supplierid}";
            supplierGrid.deletelink = "javascript:deleteSupplier({supplierid});";
            supplierGrid.getColumnByName("supplierid").hidden = true;
            supplierGrid.KeyField = "supplierid";
            supplierGrid.SortedColumn = supplierGrid.getColumnByName("suppliername");
            supplierGrid.addFilter(fields.GetFieldByID(new Guid("4A7B292D-B501-4DC8-AB81-ACCF5E0A3DDA")), ConditionType.Equals, new object[] { subaccountid }, null, ConditionJoiner.None);
            supplierGrid.InitialiseRowGridInfo = null;
            supplierGrid.InitialiseRow += new cGridNew.InitialiseRowEvent(suppliergrid_InitialiseRow);
            supplierGrid.ServiceClassForInitialiseRowEvent = "Spend_Management.cSuppliers";
            supplierGrid.ServiceClassMethodForInitialiseRowEvent = "suppliergrid_InitialiseRow";

            supplierGrid.addEventColumn("openurl", "images/icons/16/Plain/earth.png", "javascript:launchSupplierGridURL('{weburl}');", "Open Web Link", "Open Web Link");

            return supplierGrid.generateGrid();
        }

        public void suppliergrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            if (row != null)
            {
                cNewGridCell urlCell = row.getCellByID("weburl");
                if (urlCell != null)
                {
                    if (string.IsNullOrEmpty(urlCell.Text))
                    {
                        urlCell.Value = "&nbsp;";
                    }
                }
            }
        }
    }
}
