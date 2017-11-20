using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using SpendManagementLibrary;

namespace Spend_Management
{
    [Serializable()]
    public class cSupplierAddresses
    {
        DBConnection db = null;
        Dictionary<int, cAddress> lstitems;
        string strsql;
        int nAccountid = 0;

        //System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

        public cSupplierAddresses(int accountid)
        {
            nAccountid = accountid;

            InitialiseData();
        }

        public int accountid
        {
            get { return nAccountid; }
        }

        private void InitialiseData()
        {
            //lstitems = (Dictionary<int, cAddress>)Cache["addresses" + accountid];
            //if (lstitems == null)
            //{
            lstitems = CacheList();
            //}
            return;
        }

        public Dictionary<int, cAddress> CacheList()
        {
            Dictionary<int, cAddress> list = new Dictionary<int, cAddress>();
            db = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "SELECT addressid, address_title, addr_line1, addr_line2, town, county, postcode, countryid, switchboard, fax, private_address, createdon, createdby, modifiedon, modifiedby FROM dbo.supplier_addresses";
            db.sqlexecute.CommandText = strsql;
            //SqlCacheDependency dep = new SqlCacheDependency(db.sqlexecute);

            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strsql))
            {
                cAddress addr = null;

                while (reader.Read())
                {
                    int addressId = reader.GetInt32(reader.GetOrdinal("addressid"));
                    
                    string addrtitle = "";
                    
                    if(!reader.IsDBNull(reader.GetOrdinal("address_title")))
                    {
                        addrtitle = reader.GetString(reader.GetOrdinal("address_title"));
                    }
                    
                    string addr1 = "";

                    if (!reader.IsDBNull(reader.GetOrdinal("addr_line1")))
                    {
                        addr1 = reader.GetString(reader.GetOrdinal("addr_line1"));
                    }

                    string addr2 = "";

                    if (!reader.IsDBNull(reader.GetOrdinal("addr_line2")))
                    {
                        addr2 = reader.GetString(reader.GetOrdinal("addr_line2"));
                    }

                    string town = "";

                    if (!reader.IsDBNull(reader.GetOrdinal("town")))
                    {
                        town = reader.GetString(reader.GetOrdinal("town"));
                    }

                    string county = "";

                    if (!reader.IsDBNull(reader.GetOrdinal("county")))
                    {
                        county = reader.GetString(reader.GetOrdinal("county"));
                    }

                    string postcode = "";

                    if (!reader.IsDBNull(reader.GetOrdinal("postcode")))
                    {
                        postcode = reader.GetString(reader.GetOrdinal("postcode"));
                    }

                    int countryid = 0;

                    if(!reader.IsDBNull(reader.GetOrdinal("countryid")))
                    {
                        countryid = reader.GetInt32(reader.GetOrdinal("countryid"));
                    }

                    string switchboard = "";

                    if (!reader.IsDBNull(reader.GetOrdinal("switchboard")))
                    {
                        switchboard = reader.GetString(reader.GetOrdinal("switchboard"));
                    }

                    string fax = "";

                    if (!reader.IsDBNull(reader.GetOrdinal("fax")))
                    {
                        fax = reader.GetString(reader.GetOrdinal("fax"));
                    }

                    bool private_addr = reader.GetBoolean(reader.GetOrdinal("private_address"));

                    DateTime created = new DateTime(1900, 01, 01);

                    if (!reader.IsDBNull(reader.GetOrdinal("createdon")))
                    {
                        created = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }

                    int createdby = 0;

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

                    addr = new cAddress(addressId, addrtitle, addr1, addr2, town, county, postcode, countryid, switchboard, fax, private_addr, created, createdby, modified, modifiedby);
                    list.Add(addressId, addr);
                }
                reader.Close();
            }

            //if (list.Count > 0)
            //{
            //    Cache.Insert("addresses" + accountid, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
            //}
            return list;
        }

        public cAddress getAddressById(int addressId)
        {
            cAddress retAddr = null;

            if (lstitems.ContainsKey(addressId))
            {
                retAddr = (cAddress)lstitems[addressId];
            }

            return retAddr;
        }

        public Dictionary<int, cAddress> getSupplierAddresses()
        {
            return lstitems;
        }

        public cAddress getAddressByTitle(string address_title)
        {
            cAddress retAddr = null;

            foreach (KeyValuePair<int,cAddress> i in lstitems)
            {
                cAddress addr = (cAddress)i.Value;

                if (addr.AddressTitle == address_title)
                {
                    retAddr = addr;
                    break;
                }
            }

            return retAddr;
        }

        public void DeleteAddress(int addressId)
        {
            db = new DBConnection(cAccounts.getConnectionString(accountid));
            db.sqlexecute.Parameters.AddWithValue("@addressid", addressId);

            CurrentUser currentUser = cMisc.GetCurrentUser();
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            db.ExecuteProc("deleteSupplierAddress");

            db.sqlexecute.Parameters.Clear();

            return;
        }

        public int UpdateAddress(cAddress address, int SupplierID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int retAddressId = 0;
            db = new DBConnection(cAccounts.getConnectionString(accountid));

            db.sqlexecute.Parameters.Add("@ReturnId", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@ReturnId"].Direction = System.Data.ParameterDirection.ReturnValue;
            db.sqlexecute.Parameters.AddWithValue("@addressid", address.AddressId);
            if (SupplierID == 0)
            {
                db.sqlexecute.Parameters.AddWithValue("@supplierid", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@supplierid", SupplierID);
            }
            db.sqlexecute.Parameters.AddWithValue("@address_title", address.AddressTitle);
            db.sqlexecute.Parameters.AddWithValue("@addr_line1", address.AddressLine1);
            db.sqlexecute.Parameters.AddWithValue("@addr_line2", address.AddressLine2);
            db.sqlexecute.Parameters.AddWithValue("@town", address.Town);
            db.sqlexecute.Parameters.AddWithValue("@county", address.County);
            db.sqlexecute.Parameters.AddWithValue("@postcode", address.PostCode);
            if (address.CountryId > 0)
            {
                db.sqlexecute.Parameters.AddWithValue("@countryid", address.CountryId);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@countryid", DBNull.Value);
            }
            db.sqlexecute.Parameters.AddWithValue("@switchboard", address.Switchboard);
            db.sqlexecute.Parameters.AddWithValue("@fax", address.Fax);
            db.sqlexecute.Parameters.AddWithValue("@private_address", address.IsPrivateAddress);
            db.sqlexecute.Parameters.AddWithValue("@userid", currentUser.EmployeeID);
            //db.sqlexecute.Parameters.AddWithValue("@subAccountID", currentUser.CurrentSubAccountId);

            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            db.ExecuteProc("saveSupplierAddress");

            retAddressId = (int)db.sqlexecute.Parameters["@ReturnId"].Value;

            db.sqlexecute.Parameters.Clear();

            return retAddressId;
        }
    }
}
