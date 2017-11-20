using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    [Serializable()]
    public class cSupplierContacts
    {
        Dictionary<string, cSupplierContact> lstitems;
        string strsql;
        int nAccountid = 0;
        int nSupplierId = 0;

        //System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

        public cSupplierContacts(int accountid, int supplierid)
        {
            nAccountid = accountid;
            nSupplierId = supplierid;

            InitialiseData();
        }

        public int accountid
        {
            get { return nAccountid; }
        }

        public int supplierid
        {
            get { return nSupplierId; }
        }

        private void InitialiseData()
        {
            //lstitems = (Dictionary<string, cSupplierContact>)Cache["contacts_" + accountid.ToString() + "_" + supplierid.ToString()];
            //if (lstitems == null)
            //{
            lstitems = CacheList();
            //}
            return;
        }

        public Dictionary<string, cSupplierContact> CacheList()
        {
            cSupplierAddresses addresses = new cSupplierAddresses(accountid);
            cUserdefinedFields ufields = new cUserdefinedFields(accountid);
            cTables tables = new cTables(accountid);
            cFields fields = new cFields(accountid);
            cTable contactTable = tables.GetTableByName("supplier_contacts");
            Dictionary<string, cSupplierContact> list = new Dictionary<string, cSupplierContact>();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "SELECT contactid, supplierid, contactname, position, email, mobile, business_addressid, home_addressid, comments, main_contact, createdon, createdby, modifiedon, modifiedby FROM dbo.supplier_contacts WHERE supplierid = @supplierId";
            db.sqlexecute.CommandText = strsql;
            db.sqlexecute.Parameters.AddWithValue("@supplierId", supplierid);
            //SqlCacheDependency dep = new SqlCacheDependency(db.sqlexecute);

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strsql))
            {
                cSupplierContact contact = null;

                while (reader.Read())
                {
                    int contactId = reader.GetInt32(reader.GetOrdinal("contactid"));
                    int suppid = reader.GetInt32(reader.GetOrdinal("supplierid"));
                    string contactname = reader.GetString(reader.GetOrdinal("contactname"));
                    string position = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("position")))
                    {
                        position = reader.GetString(reader.GetOrdinal("position"));
                    }
                    string email = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("email")))
                    {
                        email = reader.GetString(reader.GetOrdinal("email"));
                    }
                    string mobile = "";
                    if (!reader.IsDBNull(reader.GetOrdinal("mobile")))
                    {
                        mobile = reader.GetString(reader.GetOrdinal("mobile"));
                    }
                    int business_addressid = 0;
                    cAddress business_addr = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("business_addressid")))
                    {
                        business_addressid = reader.GetInt32(reader.GetOrdinal("business_addressid"));
                        business_addr = addresses.getAddressById(business_addressid);
                    }
                    int home_addressid = 0;
                    cAddress home_addr = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("home_addressid")))
                    {
                        home_addressid = reader.GetInt32(reader.GetOrdinal("home_addressid"));
                        home_addr = addresses.getAddressById(home_addressid);
                    }
                    else
                    {
                        home_addr = new cAddress(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, string.Empty, string.Empty, true, DateTime.UtcNow, 0, null, null);
                    }
                    
                    string comments = reader.GetString(reader.GetOrdinal("comments"));
                    bool main_contact = reader.GetBoolean(reader.GetOrdinal("main_contact"));

                    DateTime? created = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("createdon")))
                    {
                        created = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }
                    int? createdby = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("createdby")))
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    DateTime? modified = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                    {
                        modified = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    int? modifiedby = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }

                    SortedList<int, object> userdefined = ufields.GetRecord(tables.GetTableByID(contactTable.UserDefinedTableID), contactId, tables, fields);
                    contact = new cSupplierContact(contactId, supplierid, contactname, position, mobile, email, business_addr, home_addr, comments, main_contact, created, createdby, modified, modifiedby, userdefined);
                    list.Add(contactId.ToString(), contact);
                }
                reader.Close();
            }

            //if (list.Count > 0)
            //{
            //    Cache.Insert("contacts_" + accountid.ToString() + "_" + supplierid.ToString(), list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
            //}

            return list;
        }

        public cSupplierContact getContactById(int contactid)
        {
            cSupplierContact contact = null;
            if (lstitems.ContainsKey(contactid.ToString()))
            {
                contact = (cSupplierContact)lstitems[contactid.ToString()];
            }

            return contact;
        }

        public cAddress getContactBusinessAddress(int contactid)
        {
            cAddress retAddr = null;
            cSupplierContact contact = getContactById(contactid);
            if (contact != null)
            {
                retAddr = contact.BusinessAddress;
            }
            return retAddr;
        }

        public cAddress getContactHomeAddress(int contactid)
        {
            cAddress retAddr = null;
            cSupplierContact contact = getContactById(contactid);
            if (contact != null)
            {
                retAddr = contact.HomeAddress;
            }
            return retAddr;
        }

        public cSupplierContact getContactByName(string name)
        {
            cSupplierContact retContact = null;
            foreach (KeyValuePair<string, cSupplierContact> i in lstitems)
            {
                cSupplierContact curContact = (cSupplierContact)i.Value;
                if (curContact.Name == name)
                {
                    retContact = curContact;
                    break;
                }
            }

            return retContact;
        }


        public Dictionary<string, cSupplierContact> getContacts()
        {
            return lstitems;
        }

        public int UpdateContact(cSupplierContact contact)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            int retId = 0;
            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));

            cSupplierAddresses clsSupplierAddresses = new cSupplierAddresses(accountid);

            int business_addressid = clsSupplierAddresses.UpdateAddress(contact.BusinessAddress, contact.SupplierId);

            int private_addressid = clsSupplierAddresses.UpdateAddress(contact.HomeAddress, contact.SupplierId);

            db.sqlexecute.Parameters.Add("@returnID", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@returnID"].Direction = System.Data.ParameterDirection.ReturnValue;
            db.sqlexecute.Parameters.AddWithValue("@userid", currentUser.EmployeeID);
            db.sqlexecute.Parameters.AddWithValue("@supplierid", contact.SupplierId);
            db.sqlexecute.Parameters.AddWithValue("@contactid", contact.ContactId);
            db.sqlexecute.Parameters.AddWithValue("@contactname", contact.Name);
            db.sqlexecute.Parameters.AddWithValue("@position", contact.Position);
            db.sqlexecute.Parameters.AddWithValue("@email", contact.Email);
            db.sqlexecute.Parameters.AddWithValue("@mobile", contact.Mobile);
            db.sqlexecute.Parameters.AddWithValue("@business_addressid", business_addressid);
            db.sqlexecute.Parameters.AddWithValue("@private_addressid", private_addressid);
            db.sqlexecute.Parameters.AddWithValue("@comments", contact.Comments);
            db.sqlexecute.Parameters.AddWithValue("@maincontact", contact.MainContact);

            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            db.ExecuteProc("saveSupplierContact");
            retId = (int)db.sqlexecute.Parameters["@returnID"].Value;
            db.sqlexecute.Parameters.Clear();

            cTables clstables = new cTables(accountid);
            cTable tbl = clstables.GetTableByName("supplier_contacts");
            cUserdefinedFields clsuserdefined = new cUserdefinedFields(accountid); ;
            clsuserdefined.SaveValues(clstables.GetTableByID(tbl.UserDefinedTableID), retId, contact.UserDefined, new cTables(accountid), new cFields(accountid), currentUser, elementId: (int)SpendManagementElement.SupplierContacts, record: contact.Name);

            return retId;
        }

        public void DeleteContact(int contactid)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));

            CurrentUser currentUser = cMisc.GetCurrentUser();
            db.sqlexecute.Parameters.AddWithValue("@contactid", contactid);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            db.ExecuteProc("deleteSupplierContact");
            db.sqlexecute.Parameters.Clear();

            return;
        }

		public string getGridSQL
		{
            get { return "SELECT contactid, contactname, position, email, mobile, main_contact FROM supplier_contacts"; }
		}

        /// <summary>
        /// Static method to obtain a contact by it's ID
        /// </summary>
        /// <param name="accountid">Metabase customer account ID</param>
        /// <param name="contactid">Contact ID to retrieve</param>
        /// <returns></returns>
        public static cSupplierContact getContactByContactId(int accountid, int contactid)
        {
            cSupplierContact retContact = null;
            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));

            string sql = "select supplierid from supplier_contacts where contactid = @contactid";
            db.sqlexecute.Parameters.AddWithValue("@contactid", contactid);
            int supplierid = db.getcount(sql);

            CurrentUser curUser = cMisc.GetCurrentUser();
            cSuppliers suppliers = new cSuppliers(accountid, curUser.CurrentSubAccountId);
            cSupplierContacts contacts = new cSupplierContacts(accountid, supplierid);
            if (contactid > 0)
            {
                retContact = contacts.getContactById(contactid);
            }

            return retContact;
        }
    }
}
