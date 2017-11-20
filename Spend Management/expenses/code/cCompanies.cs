//namespace Spend_Management
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Data;
//    using System.Data.SqlClient;
//    using System.Globalization;
//    using System.Linq;
//    using System.Text;
//    using System.Web.Caching;
//    using System.Web.UI.WebControls;

//    using SpendManagementLibrary.Addresses;

//    using Spend_Management.shared.code.PostcodeAnywhere;
//    using SpendManagementLibrary;
//    using SpendManagementLibrary.Employees;

//    using PostcodeAnywhere = Spend_Management.shared.code.PostcodeAnywhere.PostcodeAnywhere;

//    /// <summary>
//    /// Summary description for companies.
//    /// Cache["companieslist" + accountid[
//    /// </summary>
//    public class cCompanies
//    {
//        public Cache Cache = System.Web.HttpRuntime.Cache;
		
//        private int nAccountid;
//        private cFields fields;

//        public cCompanies(int accountid)
//        {
//            nAccountid = accountid;
//            fields = new cFields(accountid);
//        }

//        #region properties
//        public int accountid
//        {
//            get { return nAccountid; }
//        }
//        #endregion
        
//        public DataTable searchForLocations(string name, CompanyType companytype)
//        {
            
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql = "select top 20 companyid, company from companies where (company like @name or postcode like @name) ";
//            expdata.sqlexecute.Parameters.AddWithValue("@name", name + "%");
//            switch (companytype)
//            {
//                case CompanyType.Company:
//                    strsql += "and iscompany = 1";
//                    break;
//                case CompanyType.From:
//                    strsql += "and showfrom = 1";
//                    break;
//                case CompanyType.To:
//                    strsql += "and showto = 1";
//                    break;
//            }

//            DataSet ds = expdata.GetDataSet(strsql);
//            expdata.sqlexecute.Parameters.Clear();

//            return ds.Tables[0];
//        }

//        public DataTable searchForLocations(string name, string address1, string address2, string city, string county, string postcode, int country, CompanyType companytype)
//        {
//            StringBuilder query = new StringBuilder();
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql = "select companyid, company from companies where archived = 0";
//            switch (companytype)
//            {
//                case CompanyType.Company:
//                    strsql += " and iscompany = 1";
//                    break;
//                case CompanyType.From:
//                    strsql += " and showfrom = 1";
//                    break;
//                case CompanyType.To:
//                    strsql += " and showto = 1";
//                    break;
//            }
//            if (name != "")
//            {
//                query.Append("company like @name and ");
//                expdata.AddWithValue("@name", name + "%", fields.GetFieldSize("companies", "company"));
//            }
            
//            if (address1 != "")
//            {
//                query.Append("address1 like @address1 and ");
//                expdata.AddWithValue("@address1", address1 + "%", fields.GetFieldSize("companies", "address1"));
//            }
//            if (address2 != "")
//            {
//                query.Append("address2 like @address2 and ");
//                expdata.AddWithValue("@address2", address2 + "%", fields.GetFieldSize("companies", "address2"));
//            }
//            if (city != "")
//            {
//                query.Append("city like @city and ");
//                expdata.AddWithValue("@city", city + "%", fields.GetFieldSize("companies", "city"));
//            }
//            if (county != "")
//            {
//                query.Append("county like @county and ");
//                expdata.AddWithValue("@county", county + "%", fields.GetFieldSize("companies", "county"));
//            }
//            if (postcode != "")
//            {
//                query.Append("postcode like @postcode and ");
//                expdata.AddWithValue("@postcode", postcode + "%", fields.GetFieldSize("companies", "postcode"));
//            }
//            if (country > 0)
//            {
//                query.Append("country = @country and ");
//                expdata.sqlexecute.Parameters.AddWithValue("@country", country);
//            }          

//            if (query.Length > 0)
//            {
//                query.Remove(query.Length - 5, 5);
//                strsql += " and " + query.ToString();
//            }
//            DataSet ds = expdata.GetDataSet(strsql);

//            expdata.sqlexecute.Parameters.Clear();
//            return ds.Tables[0];
//        }

//        public string searchForLocations(string name, string code, string address1, string address2, string city, string county, string postcode, int country, int parentcompanyid, string showfrom, string showto, string iscompany, SortedList<int,string> userdefined)
//        {
//            string strsql = "select companyid, company, comment, archived, companycode from companies";
//            return strsql;
//        }

//        public string getGrid()
//        {
//            //return "select companyid, company, comment, archived, companycode from companies";
//            return "select companyid, company, address1, postcode, archived, companycode from companies";
//        }

//        /// <summary>
//        /// Delete the address location from the database 
//        /// </summary>
//        /// <param name="companyid"></param>
//        /// <returns></returns>
//        public ReturnValues deleteCompany (int companyid)
//        {
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

//            CurrentUser currentUser = cMisc.GetCurrentUser();

//            expdata.sqlexecute.Parameters.AddWithValue("@companyID", companyid);
//            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
//            if (currentUser.isDelegate == true)
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
//            }
//            else
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
//            }
//            expdata.sqlexecute.Parameters.Add("@returncode", SqlDbType.Int);
//            expdata.sqlexecute.Parameters["@returncode"].Direction = ParameterDirection.ReturnValue;
//            expdata.ExecuteProc("deleteCompany");
//            int returncode = (int)expdata.sqlexecute.Parameters["@returncode"].Value;

//            expdata.sqlexecute.Parameters.Clear();

//            return (ReturnValues)returncode;
//        }

//        public void changeStatus(int companyid, bool archive)
//        {
//            string strsql;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            cCompany reqcompany = GetCompanyById(companyid);
//            expdata.sqlexecute.Parameters.AddWithValue("@companyid",companyid);
			
//            if (archive == true)
//            {
//                strsql = "update companies set archived = 1 where companyid = @companyid";
//            }
//            else
//            {
//                strsql = "update companies set archived = 0 where companyid = @companyid";
//            }
//            expdata.ExecuteSQL(strsql);
//            expdata.sqlexecute.Parameters.Clear();

//            cAuditLog clsaudit = new cAuditLog();
//            clsaudit.editRecord(companyid, reqcompany.company, SpendManagementElement.Locations, new Guid("2EDE6033-62F1-4D65-97B7-9153D73BC29F"), reqcompany.archived.ToString(), archive.ToString());
//        }

//        /// <summary>
//        /// Save company details
//        /// </summary>
//        /// <param name="company">Pass in company details object</param>
//        /// <param name="user">(Optional)Pass in a CurrentUser for non-logged in users (self registration users)</param>
//        /// <returns></returns>
//        public int saveCompany(FullCompany company, CurrentUser user = null)
//        {
//            // Disallows new addresses called home or office
//            if ((company.company.Trim().ToLower() == "home" || company.company.Trim().ToLower() == "office") && company.companyid == 0)
//            {
//                return -2;
//            }

//            // Check if the postcode for this country is in a valid format
//            if (this.ValidatePostcode(company.country, company.postcode) == false && company.CreationMethod != cCompany.AddressCreationMethod.ESROutboundAdded)
//            {
//                return -3;
//            }

//            CurrentUser currentUser = user ?? cMisc.GetCurrentUser();
            
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
//            expdata.sqlexecute.Parameters.AddWithValue("@companyid", company.companyid);
//            expdata.AddWithValue("@company", company.company, this.fields.GetFieldSize("companies", "company"));

//            if (string.IsNullOrEmpty(company.companycode))
//            {
//                expdata.AddWithValue("@companycode", DBNull.Value, this.fields.GetFieldSize("companies", "companycode"));
//            }
//            else
//            {
//                expdata.AddWithValue("@companycode", company.companycode, this.fields.GetFieldSize("companies", "companycode"));
//            }

//            if (string.IsNullOrEmpty(company.comment))
//            {
//                expdata.AddWithValue("@comment", DBNull.Value, this.fields.GetFieldSize("companies", "comment"));
//            }
//            else
//            {
//                expdata.AddWithValue("@comment", company.comment, this.fields.GetFieldSize("companies", "comment"));
//            }

//            if (string.IsNullOrEmpty(company.address1))
//            {
//                expdata.AddWithValue("@address1", DBNull.Value, this.fields.GetFieldSize("companies", "address1"));
//            }
//            else
//            {
//                expdata.AddWithValue("@address1", company.address1, this.fields.GetFieldSize("companies", "address1"));
//            }

//            if (string.IsNullOrEmpty(company.address2))
//            {
//                expdata.AddWithValue("@address2", DBNull.Value, this.fields.GetFieldSize("companies", "address2"));
//            }
//            else
//            {
//                expdata.AddWithValue("@address2", company.address2, this.fields.GetFieldSize("companies", "address2"));
//            }

//            if (string.IsNullOrEmpty(company.city))
//            {
//                expdata.AddWithValue("@city", DBNull.Value, this.fields.GetFieldSize("companies", "city"));
//            }
//            else
//            {
//                expdata.AddWithValue("@city", company.city, this.fields.GetFieldSize("companies", "city"));
//            }

//            if (string.IsNullOrEmpty(company.county))
//            {
//                expdata.AddWithValue("@county", DBNull.Value, this.fields.GetFieldSize("companies", "county"));
//            }
//            else
//            {
//                expdata.AddWithValue("@county", company.county, this.fields.GetFieldSize("companies", "county"));
//            }

//            if (string.IsNullOrEmpty(company.postcode))
//            {
//                expdata.AddWithValue("@postcode", DBNull.Value, this.fields.GetFieldSize("companies", "postcode"));
//            }
//            else
//            {
//                expdata.AddWithValue("@postcode", company.postcode, this.fields.GetFieldSize("companies", "postcode"));
//            }

//            if (company.country == 0)
//            {
//                expdata.AddWithValue("@country", DBNull.Value, this.fields.GetFieldSize("companies", "country"));
//            }
//            else
//            {
//                expdata.AddWithValue("@country", company.country, this.fields.GetFieldSize("companies", "country"));
//            }

//            if (company.parentcompanyid == 0)
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@parentcompanyid", DBNull.Value);
//            }
//            else
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@parentcompanyid", company.parentcompanyid);
//            }

//            expdata.sqlexecute.Parameters.AddWithValue("@iscompany",Convert.ToBoolean(company.iscompany));
//            expdata.sqlexecute.Parameters.AddWithValue("@showfrom", Convert.ToBoolean(company.showfrom));
//            expdata.sqlexecute.Parameters.AddWithValue("@showto", Convert.ToBoolean(company.showto));
//            expdata.sqlexecute.Parameters.AddWithValue("@isPrivateAddress", Convert.ToBoolean(company.isPrivate));

//            expdata.sqlexecute.Parameters.AddWithValue("@addressCreationMethod", Convert.ToInt16(company.CreationMethod));

//            if (company.companyid > 0)
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@date", company.modifiedon);
//                expdata.sqlexecute.Parameters.AddWithValue("@userid", company.modifiedby);
//            }
//            else
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@date", company.createdon);
//                expdata.sqlexecute.Parameters.AddWithValue("@userid", company.createdby);
//            }

//            if (currentUser != null)
//            {
//                if (currentUser.isDelegate == true)
//                {
//                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
//                }
//                else
//                {
//                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
//                }
//            }
//            else
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
//            }

//            expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
//            expdata.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
//            expdata.ExecuteProc("saveCompany");
//            int companyid = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
//            if (companyid == -1)
//            {
//                return -1;
//            }
//            company.updateID(companyid);
//            expdata.sqlexecute.Parameters.Clear();

//            cTables clstables = new cTables(this.accountid);
//            cFields clsfields = new cFields(this.accountid);
//            cTable tbl = clstables.GetTableByID(new Guid("d0335558-1ef5-449b-87cd-3e6bbf126205"));
//            cUserdefinedFields clsuserdefined = new cUserdefinedFields(this.accountid);
//            clsuserdefined.SaveValues(clstables.GetTableByID(tbl.UserDefinedTableID), companyid, company.userdefined, clstables, clsfields, currentUser);

//            return companyid;
//        }

//        public bool alreadyExists(string company, bool update, int id)
//        {
//            int count;
			
//            string strsql;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            if (update)
//            {
//                strsql = "select  count(*) from companies where company = @company and companyid <> @companyid";
//                expdata.sqlexecute.Parameters.AddWithValue("@companyid", id);
//            }
//            else
//            {
//                strsql = "select count(*) from companies where company = @company";
//            }
//            expdata.AddWithValue("@company", company.Trim().ToLower(), fields.GetFieldSize("companies", "company"));
//            count = expdata.getcount(strsql);
//            expdata.sqlexecute.Parameters.Clear();
//            if (count > 0)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Generates a unique address name to assign to the new address
//        /// </summary>
//        /// <param name="addressLine1">Provide the first line of the address</param>
//        /// <param name="postcode">Provide the postcode</param>
//        /// <returns>A unique address name</returns>
//        public string GenerateAddressName(string addressLine1, string postcode)
//        {
//            string addressName;
//            int i = 0;

//            do
//            {
//                addressName = (i == 0) ? string.Format("{0} {1}", addressLine1, postcode) : string.Format("{0} {1} {2}", addressLine1, postcode, i);
//                i++;
//            }
//            while (this.alreadyExists(addressName, false, 0) == true);

//            return addressName;
//        }

//        public void deleteDistance(int distanceid)
//        {
//            CurrentUser currentUser = cMisc.GetCurrentUser();

//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            expdata.sqlexecute.Parameters.AddWithValue("@distanceid", distanceid);
//            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
//            if (currentUser.isDelegate == true)
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
//            }
//            else
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
//            }
//            expdata.ExecuteProc("deleteDistance");
//            expdata.sqlexecute.Parameters.Clear();
//        }

//        public cCompany GetCompanyById(int companyid)
//        {
//            cCompany company = null;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            SqlDataReader reader;
//            string postcode;
//            int createdby;
//            bool isPrivate, archived;
//            string strsql = "SELECT [dbo].companies.companyid, [dbo].companies.company, [dbo].companies.postcode, [dbo].companies.isPrivateAddress, [dbo].companies.CreatedBy, [dbo].companies.archived FROM [dbo].companies WHERE ([dbo].companies.companyid = @companyid)";
            
//            expdata.sqlexecute.Parameters.AddWithValue("@companyid", companyid);
//            using (reader = expdata.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    if (reader.IsDBNull(2))
//                    {
//                        postcode = "";
//                    }
//                    else
//                    {
//                        postcode = reader.GetString(2);
//                    }
//                    if (reader.IsDBNull(4) == true) //CreatedBy
//                    {
//                        createdby = 0;
//                    }
//                    else
//                    {
//                        createdby = reader.GetInt32(4);
//                    }

//                    isPrivate = reader.GetBoolean(3);
//                    archived = reader.GetBoolean(5);
//                    company = new cCompany(reader.GetInt32(0), reader.GetString(1), postcode, isPrivate, createdby, archived);
//                }
//                reader.Close();
//            }
            
//            expdata.sqlexecute.Parameters.Clear();
//            return company;
//        }

//        public CompanyWithAddress GetCompanyWithAddressById(int companyid)
//        {
//            CompanyWithAddress company = null;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            SqlDataReader reader;
//            string postcode, address1, address2, city, county;
//            int createdby, country;
//            bool isPrivate, archived;
//            string strsql = "SELECT  companyid, company, postcode, isPrivateAddress, createdby, address1, address2, city, county, country, archived FROM dbo.companies where companyid = @companyid";
//            expdata.sqlexecute.Parameters.AddWithValue("@companyid", companyid);
//            using (reader = expdata.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    if (reader.IsDBNull(2))
//                    {
//                        postcode = "";
//                    }
//                    else
//                    {
//                        postcode = reader.GetString(2);
//                    }
//                    if (reader.IsDBNull(4) == true) //CreatedBy
//                    {
//                        createdby = 0;
//                    }
//                    else
//                    {
//                        createdby = reader.GetInt32(4);
//                    }
//                    isPrivate = reader.GetBoolean(3);
//                    if (reader.IsDBNull(5) == true) //address1
//                    {
//                        address1 = "";
//                    }
//                    else
//                    {
//                        address1 = reader.GetString(5);
//                    }
//                    if (reader.IsDBNull(6) == true) //address2
//                    {
//                        address2 = "";
//                    }
//                    else
//                    {
//                        address2 = reader.GetString(6);
//                    }
//                    if (reader.IsDBNull(7) == true) //city
//                    {
//                        city = "";
//                    }
//                    else
//                    {
//                        city = reader.GetString(7);
//                    }
//                    if (reader.IsDBNull(8) == true) //county
//                    {
//                        county = "";
//                    }
//                    else
//                    {
//                        county = reader.GetString(8);
//                    }
                    
//                    if (reader.IsDBNull(9) == true) //country
//                    {
//                        country = 0;
//                    }
//                    else
//                    {
//                        country = reader.GetInt32(9);
//                    }
//                    archived = reader.GetBoolean(10);
//                    company = new CompanyWithAddress(reader.GetInt32(0), reader.GetString(1),address1, address2,city,county, postcode, country,isPrivate, createdby, archived);
//                }
//                reader.Close();
//            }
            
//            expdata.sqlexecute.Parameters.Clear();
//            return company;
//        }

//        public FullCompany GetFullCompanyById(int companyid)
//        {
//            return Cache["company_" + accountid + "_" + companyid] as FullCompany ?? getCompanyFromDB(companyid);
//        }
		
//        internal SortedList<int, cCompany> GetCompaniesById(List<int> companyIds)
//        {
//            if (companyIds.Count == 0)
//            {
//                return new SortedList<int, cCompany>();
//            }

//            // make sure there's a minimum number of items in the list so that the 
//            // sql execution plan will come from sql plan cache as often as possible
//            int itemCount = companyIds.Count; // don't use the count in the for directly as it'll change over iterations
//            for (int i = 0; i < (50 - itemCount); i++)
//            {
//                companyIds.Add(0);
//            }

//            SortedList<int, cCompany> companyObjects = new SortedList<int, cCompany>();
//            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
//            StringBuilder sql = new StringBuilder("SELECT companyid, company, postcode, isPrivateAddress, createdby, archived FROM dbo.companies WHERE");
//            StringBuilder sqlWhere = new StringBuilder();

//            int j = 0;
//            foreach (int loopCompanyId in companyIds)
//            {
//                sqlWhere.Append(" OR companyid = @c").Append(j.ToString(CultureInfo.InvariantCulture));
//                data.sqlexecute.Parameters.AddWithValue("@c" + j.ToString(CultureInfo.InvariantCulture), loopCompanyId);
//                j++;
//            }
//            sql.Append(sqlWhere.Remove(0, 3).ToString());

//            data.sqlexecute.CommandText = sql.ToString();

//            #region Read Data
//            using (SqlDataReader reader = data.GetReader(sql.ToString()))
//            {
//                data.sqlexecute.Parameters.Clear();

//                DateTime createdon, modifiedon;
//                int createdby, modifiedby, companyid, country, parentcompanyid;
//                string company, companycode, comment, address1, address2, city, county, postcode;
//                bool archived, iscompany, showto, showfrom, isPrivate;
//                cCompany.AddressCreationMethod addressCreationMethod;

//                while (reader.Read())
//                {
//                    companyid = reader.GetInt32(0); //companyid
//                    company = reader.GetString(1); //company
//                    if (reader.IsDBNull(2))
//                    {
//                        postcode = "";
//                    }
//                    else
//                    {
//                        postcode = reader.GetString(2);
//                    }
//                    if (reader.IsDBNull(4) == true) //CreatedBy
//                    {
//                        createdby = 0;
//                    }
//                    else
//                    {
//                        createdby = reader.GetInt32(4);
//                    }
                    
//                    isPrivate = reader.GetBoolean(3);
//                    archived = reader.GetBoolean(5);
//                    companyObjects.Add(companyid, new cCompany(companyid, company, postcode, isPrivate, createdby, archived));

//                }
//                reader.Close();
//            }
//            #endregion Read Data
			
//            return companyObjects;
//        }

//        /// <summary>
//        /// WARNING: This is UNCACHED as it is used in claims and cached under the expense item, it also DOES NOT fetch the UDF values
//        /// </summary>
//        /// <param name="companyIds"></param>
//        /// <returns></returns>
//        internal SortedList<int, cCompany> GetFullCompaniesById(List<int> companyIds)
//        {
//            if (companyIds.Count == 0)
//            {
//                return new SortedList<int, cCompany>();
//            }

//            // make sure there's a minimum number of items in the list so that the 
//            // sql execution plan will come from sql plan cache as often as possible
//            int itemCount = companyIds.Count; // don't use the count in the for directly as it'll change over iterations
//            for (int i = 0; i < (50 - itemCount); i++)
//            {
//                companyIds.Add(0);
//            }

//            SortedList<int, cCompany> companyObjects = new SortedList<int, cCompany>();
//            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
//            StringBuilder sql = new StringBuilder("SELECT companyid, company, archived, comment, companycode, showfrom, showto, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, address1, address2, city, county, postcode, country, parentcompanyid, iscompany, isPrivateAddress, CacheExpiry, addressCreationMethod FROM dbo.companies WHERE");
//            StringBuilder sqlWhere = new StringBuilder();

//            int j = 0;
//            foreach (int loopCompanyId in companyIds)
//            {
//                sqlWhere.Append(" OR companyid = @c").Append(j.ToString(CultureInfo.InvariantCulture));
//                data.sqlexecute.Parameters.AddWithValue("@c" + j.ToString(CultureInfo.InvariantCulture), loopCompanyId);
//                j++;
//            }
//            sql.Append(sqlWhere.Remove(0, 3).ToString());

//            data.sqlexecute.CommandText = sql.ToString();

//            #region Read Data
//            using (SqlDataReader reader = data.GetReader(sql.ToString()))
//            {
//                data.sqlexecute.Parameters.Clear();

//                DateTime createdon, modifiedon;
//                int createdby, modifiedby, companyid, country, parentcompanyid;
//                string company, companycode, comment, address1, address2, city, county, postcode;
//                bool archived, iscompany, showto, showfrom, isPrivate;
//                cCompany.AddressCreationMethod addressCreationMethod;

//                int companyidOrd = reader.GetOrdinal("companyid");
//                int companyOrd = reader.GetOrdinal("company");
//                int archivedOrd = reader.GetOrdinal("archived");
//                int commentOrd = reader.GetOrdinal("comment");
//                int companycodeOrd = reader.GetOrdinal("companycode");
//                int showfromOrd = reader.GetOrdinal("showfrom");
//                int showtoOrd = reader.GetOrdinal("showto");
//                int createdOnOrd = reader.GetOrdinal("CreatedOn");
//                int createdByOrd = reader.GetOrdinal("CreatedBy");
//                int modifiedOnOrd = reader.GetOrdinal("ModifiedOn");
//                int modifiedByOrd = reader.GetOrdinal("ModifiedBy");
//                int address1Ord = reader.GetOrdinal("address1");
//                int address2Ord = reader.GetOrdinal("address2");
//                int cityOrd = reader.GetOrdinal("city");
//                int countyOrd = reader.GetOrdinal("county");
//                int postcodeOrd = reader.GetOrdinal("postcode");
//                int countryOrd = reader.GetOrdinal("country");
//                int parentcompanyidOrd = reader.GetOrdinal("parentcompanyid");
//                int iscompanyOrd = reader.GetOrdinal("iscompany");
//                int isPrivateAddressOrd = reader.GetOrdinal("isPrivateAddress");
//                int cacheExpiryOrd = reader.GetOrdinal("CacheExpiry");
//                int addressCreationMethodOrd = reader.GetOrdinal("addressCreationMethod");

//                while (reader.Read())
//                {
//                    companyid = reader.GetInt32(companyidOrd); //companyid
//                    company = reader.GetString(companyOrd); //company
//                    archived = reader.GetBoolean(archivedOrd); //archived
//                    comment = reader.IsDBNull(commentOrd) ? string.Empty : reader.GetString(commentOrd);
//                    companycode = reader.IsDBNull(companycodeOrd) ? string.Empty : reader.GetString(companycodeOrd);
//                    showfrom = reader.GetBoolean(showfromOrd); //showfrom
//                    showto = reader.GetBoolean(showtoOrd); //showto
//                    createdon = reader.IsDBNull(createdOnOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(createdOnOrd);
//                    createdby = reader.IsDBNull(createdByOrd) ? 0 : reader.GetInt32(createdByOrd);
//                    modifiedon = reader.IsDBNull(modifiedOnOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(modifiedOnOrd);
//                    modifiedby = reader.IsDBNull(modifiedByOrd) ? 0 : reader.GetInt32(modifiedByOrd);
//                    address1 = reader.IsDBNull(address1Ord) ? string.Empty : reader.GetString(address1Ord);
//                    address2 = reader.IsDBNull(address2Ord) ? string.Empty : reader.GetString(address2Ord);
//                    city = reader.IsDBNull(cityOrd) ? string.Empty : reader.GetString(cityOrd);
//                    county = reader.IsDBNull(countyOrd) ? string.Empty : reader.GetString(countyOrd);
//                    postcode = reader.IsDBNull(postcodeOrd) ? string.Empty : reader.GetString(postcodeOrd);
//                    country = reader.IsDBNull(countryOrd) ? 0 : reader.GetInt32(countryOrd);
//                    parentcompanyid = reader.IsDBNull(parentcompanyidOrd) ? 0 : reader.GetInt32(parentcompanyidOrd);
//                    iscompany = reader.GetBoolean(iscompanyOrd); //iscompany
//                    isPrivate = reader.GetBoolean(isPrivateAddressOrd); //isPrivateAddress
//                    addressCreationMethod = (cCompany.AddressCreationMethod)reader.GetByte(addressCreationMethodOrd);

//                    companyObjects.Add(companyid, new FullCompany(companyid, company, companycode, archived, comment, showto, showfrom, createdon, createdby, modifiedon, modifiedby, address1, address2, city, county, postcode, country, parentcompanyid, iscompany, new SortedList<int, object>(), isPrivate, addressCreationMethod));

//                }
//                reader.Close();
//            }
//            #endregion Read Data

//            return companyObjects;
//        }

//        /// <summary>
//        /// Get the companyID if the address1, postcode and the name all match an entry in the database
//        /// </summary>
//        /// <param name="address1"></param>
//        /// <param name="postcode"></param>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        public int GetCompanyIDByAddressLine1AndPostcode(string address1, string postcode)
//        {
//            int companyid = 0;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql;

//            strsql = "select companyid from companies where address1 = @address1 AND postcode = @postcode";

//            expdata.AddWithValue("@address1", address1, fields.GetFieldSize("companies", "address1"));
//            expdata.AddWithValue("@postcode", postcode, fields.GetFieldSize("companies", "postcode"));

//            using (SqlDataReader reader = expdata.GetReader(strsql))
//            {
//                expdata.sqlexecute.Parameters.Clear();

//                while (reader.Read())
//                {
//                    companyid = reader.GetInt32(0);
//                }
//                reader.Close();
//            }

//            return companyid;
//        }

//        private FullCompany getCompanyFromDB(int companyid)
//        {
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql;
//            cTables clstables = new cTables(accountid);
//            cFields clsfields = new cFields(accountid);
//            cTable tbl = clstables.GetTableByID(new Guid("d0335558-1ef5-449b-87cd-3e6bbf126205"));
//            cTable udftbl = clstables.GetTableByID(tbl.UserDefinedTableID);
//            System.Data.SqlClient.SqlDataReader reader;
//            FullCompany clscompany = null;
//            //Dictionary<int, object> userdefined;
//            SortedList<int, object> userdefined;
//            cUserdefinedFields clsuserdefined = new cUserdefinedFields(accountid); ;
//            strsql = "SELECT  companyid, company, archived, comment, companycode, showfrom, showto, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, address1, address2, city, county, postcode, country, parentcompanyid, iscompany, isPrivateAddress, CacheExpiry, addressCreationMethod FROM dbo.companies where companyid = @companyid";
//            expdata.sqlexecute.Parameters.AddWithValue("@companyid", companyid);
//            expdata.sqlexecute.CommandText = strsql;
//            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);

//            #region Read Data

//            using (reader = expdata.GetReader(strsql))
//            {
//                expdata.sqlexecute.Parameters.Clear();
//                DateTime createdon, modifiedon;
//                int createdby, modifiedby;

//                string company, companycode, comment, address1, address2, city, county, postcode;
//                int country, parentcompanyid;
//                bool archived, iscompany;
//                bool showto, showfrom;
//                bool isPrivate;
//                cCompany.AddressCreationMethod addressCreationMethod;

//                while (reader.Read())
//                {
//                    companyid = reader.GetInt32(0); //companyid
//                    company = reader.GetString(1); //company
//                    archived = reader.GetBoolean(2); //archived

//                    if (reader.IsDBNull(3) == false) //comment
//                    {
//                        comment = reader.GetString(3);
//                    }
//                    else
//                    {
//                        comment = "";
//                    }

//                    if (reader.IsDBNull(4) == false) //companycode
//                    {
//                        companycode = reader.GetString(4);
//                    }
//                    else
//                    {
//                        companycode = "";
//                    }
                    
//                    showfrom = reader.GetBoolean(5); //showfrom
//                    showto = reader.GetBoolean(6); //showto
                    
//                    if (reader.IsDBNull(7) == true) //CreatedOn
//                    {
//                        createdon = new DateTime(1900, 01, 01);
//                    }
//                    else
//                    {
//                        createdon = reader.GetDateTime(7);
//                    }
//                    if (reader.IsDBNull(8) == true) //CreatedBy
//                    {
//                        createdby = 0;
//                    }
//                    else
//                    {
//                        createdby = reader.GetInt32(8);
//                    }
//                    if (reader.IsDBNull(9) == true) //ModifiedOn
//                    {
//                        modifiedon = new DateTime(1900, 01, 01);
//                    }
//                    else
//                    {
//                        modifiedon = reader.GetDateTime(9);
//                    }
//                    if (reader.IsDBNull(10) == true) //ModifiedBy
//                    {
//                        modifiedby = 0;
//                    }
//                    else
//                    {
//                        modifiedby = reader.GetInt32(10);
//                    }
//                    if (reader.IsDBNull(11) == true) //address1
//                    {
//                        address1 = "";
//                    }
//                    else
//                    {
//                        address1 = reader.GetString(11);
//                    }
//                    if (reader.IsDBNull(12) == true) //address2
//                    {
//                        address2 = "";
//                    }
//                    else
//                    {
//                        address2 = reader.GetString(12);
//                    }
//                    if (reader.IsDBNull(13) == true) //city
//                    {
//                        city = "";
//                    }
//                    else
//                    {
//                        city = reader.GetString(13);
//                    }
//                    if (reader.IsDBNull(14) == true) //county
//                    {
//                        county = "";
//                    }
//                    else
//                    {
//                        county = reader.GetString(14);
//                    }
//                    if (reader.IsDBNull(15) == true) //postcode
//                    {
//                        postcode = "";
//                    }
//                    else
//                    {
//                        postcode = reader.GetString(15);
//                    }
//                    if (reader.IsDBNull(16) == true) //country
//                    {
//                        country = 0;
//                    }
//                    else
//                    {
//                        country = reader.GetInt32(16);
//                    }
//                    if (reader.IsDBNull(17) == true) //parentcompanyid
//                    {
//                        parentcompanyid = 0;
//                    }
//                    else
//                    {
//                        parentcompanyid = reader.GetInt32(17);
//                    }
//                    iscompany = reader.GetBoolean(18); //iscompany
//                    userdefined = clsuserdefined.GetRecord(udftbl, companyid, clstables, clsfields);
//                    isPrivate = reader.GetBoolean(19); //isPrivateAddress
//                    addressCreationMethod = (cCompany.AddressCreationMethod)reader.GetByte(21);

//                    clscompany = new FullCompany(companyid, company, companycode, archived, comment, showto, showfrom, createdon, createdby, modifiedon, modifiedby, address1, address2, city, county, postcode, country, parentcompanyid, iscompany, userdefined, isPrivate, addressCreationMethod);

//                }
//                reader.Close();
//            }

//            #endregion

//            expdata.sqlexecute.Parameters.Clear();

//            if (clscompany != null)
//            {
//                cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
//                cAccountProperties accProperties = subaccs.getFirstSubAccount().SubAccountProperties;
//                int cacheTimeout = accProperties.cachePeriodNormal;
//                CacheItemRemovedCallback onRemove = new CacheItemRemovedCallback(CacheCompaniesRemovedCallback);

//                Cache.Insert("company_" + accountid + "_" + companyid, clscompany, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), CacheItemPriority.Default, onRemove);
//            }
//            return clscompany;
//        }

//        /// <summary>
//        /// This callback method is used to remove any query notifications on the service broker associated with the cache when it expires or is removed
//        /// </summary>
//        /// <param name="cacheKey">The unique cache key</param>
//        /// <param name="cacheValue">The object associated to the cache</param>
//        /// <param name="reason">The reason for the cache being removed</param>
//        public static void CacheCompaniesRemovedCallback(String cacheKey, Object cacheValue, CacheItemRemovedReason reason)
//        {
//            if (reason != CacheItemRemovedReason.DependencyChanged)
//            {
//                int startIndex = cacheKey.IndexOf('_') + 1;
//                int endIndex = cacheKey.LastIndexOf('_');
//                string strAccountID = cacheKey.Substring(startIndex, endIndex - startIndex);
//                int accID = 0;

//                int.TryParse(strAccountID, out accID);

//                if (accID > 0)
//                {
//                    DBConnection db = new DBConnection(cAccounts.getConnectionString(accID));

//                    cCompany comp = (cCompany)cacheValue;

//                    #region Update the companies

//                    db.sqlexecute.Parameters.AddWithValue("@tablename", "companies");
//                    db.sqlexecute.Parameters.AddWithValue("@tablecolumn", "companyid");
//                    db.sqlexecute.Parameters.AddWithValue("@id", comp.companyid);
//                    db.ExecuteProc("updateCacheExpiry");
//                    db.sqlexecute.Parameters.Clear();

//                    #endregion
//                }
//            }
//        } 

//        public cCompany getCompanyFromName(string company)
//        {
//            int companyid = 0;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql = "select companyid from companies where company = @company";
//            expdata.AddWithValue("@company", company, fields.GetFieldSize("companies", "company"));
//            using (SqlDataReader reader = expdata.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    companyid = reader.GetInt32(0);
//                }
//                reader.Close();
//            }
//            expdata.sqlexecute.Parameters.Clear();

//            if (companyid > 0)
//            {
//                return GetCompanyById(companyid);
//            }
//            return null;
//        }

//        public FullCompany getFullCompanyFromName(string company)
//        {
//            int companyid = 0;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql = "select companyid from companies where company = @company";
//            expdata.AddWithValue("@company", company, fields.GetFieldSize("companies", "company"));
//            using (SqlDataReader reader = expdata.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    companyid = reader.GetInt32(0);
//                }

//                reader.Close();
//            }
//            expdata.sqlexecute.Parameters.Clear();

//            return companyid > 0 ? this.GetFullCompanyById(companyid) : null;
//            }

//        public string[] getArray(byte locationType)
//        {
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql;

//            if (locationType == 1)
//            {
//                strsql = "select company from companies where archived = 0 and showfrom = 1 order by company";
//            }
//            else
//            {
//                strsql = "select company from companies where archived = 0 and showto = 1 order by company";
//            }

//            List<string> companies = new List<string>();

//            using (SqlDataReader reader = expdata.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    companies.Add(reader.GetString(0));
//                }

//                reader.Close();
//            }

//            return companies.ToArray();
//        }

//        public DataSet getDistanceTable(int companyid)
//        {
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

//            expdata.sqlexecute.Parameters.AddWithValue("@companyid", companyid);
//            DataSet ds = expdata.GetProcDataSet("getLocationDistances");
//            expdata.sqlexecute.Parameters.Clear();

//            return ds;
//        }

//        public void updateDistance(int locationa, int locationb, decimal distance, decimal postcodeanywheredistance)
//        {
//            CurrentUser currentUser = cMisc.GetCurrentUser();

//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

//            cAccountSubAccounts clsAccountSubAccounts = new cAccountSubAccounts(accountid);
//            cAccountProperties clsProperties = clsAccountSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties; // clsAccountSubAccounts.getFirstSubAccount().SubAccountProperties;

//            expdata.sqlexecute.Parameters.AddWithValue("@locationa", locationa);
//            expdata.sqlexecute.Parameters.AddWithValue("@locationb", locationb);
//            expdata.AddWithValue("@distance", distance, 18, 2);
//            expdata.AddWithValue("@postcodeAnywhereDistance", postcodeanywheredistance, 18, 2);

//            if (clsProperties.MileageCalcType == 1)
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@mileageCalcType", 1);
                
//            }
//            else
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@mileageCalcType", 2);
//            }

//            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
//            if (currentUser.isDelegate == true)
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
//            }
//            else
//            {
//                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
//            }
//            expdata.ExecuteProc("saveLocationDistance");
//            expdata.sqlexecute.Parameters.Clear();
//        }

//        public cLocationDistance getDistanceByID(int id)
//        {
//            cLocationDistance clsdistance = null;
//            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
//            SqlDataReader reader;
//            string strsql = "select * from location_distances where distanceid = @distanceid";
//            data.sqlexecute.Parameters.AddWithValue("@distanceid", id);

//            using (reader = data.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    int locationa = reader.GetInt32(reader.GetOrdinal("locationa"));
//                    int locationb = reader.GetInt32(reader.GetOrdinal("locationb"));
//                    decimal distance = reader.GetDecimal(reader.GetOrdinal("distance"));
//                    decimal postcodeAnywhereFastestDistance = reader.IsDBNull(reader.GetOrdinal("postcodeAnywhereFastestDistance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("postcodeAnywhereFastestDistance"));
//                    decimal postcodeAnywhereShortestDistance = reader.IsDBNull(reader.GetOrdinal("postcodeAnywhereShortestDistance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("postcodeAnywhereShortestDistance"));
                    
//                    clsdistance = new cLocationDistance(locationa, locationb, distance, postcodeAnywhereFastestDistance, postcodeAnywhereShortestDistance);
//                }

//                reader.Close();
//            }

//            data.sqlexecute.Parameters.Clear();

//            return clsdistance;
//        }

//        /// <summary>
//        /// Checks if there are any manually entered distances set between the two addresses
//        /// </summary>
//        /// <param name="fromLocationID"></param>
//        /// <param name="toLocationID"></param>
//        /// <returns></returns>
//        public decimal? ManuallyEnteredCompanyToCompany(int fromLocationID, int toLocationID)
//        {
//            decimal? distance = null;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql = "select distance from location_distances where (locationa = @locationa and locationb = @locationb)";
//            expdata.sqlexecute.Parameters.AddWithValue("@locationa", fromLocationID);
//            expdata.sqlexecute.Parameters.AddWithValue("@locationb", toLocationID);

//            SqlDataReader reader = expdata.GetReader(strsql);

//            expdata.sqlexecute.Parameters.Clear();
//            while (reader.Read())
//            {
//                if (reader.IsDBNull(reader.GetOrdinal("distance")) == false)
//                {
//                    distance = reader.GetDecimal(reader.GetOrdinal("distance"));
//                }
//            }
//            reader.Close();

//            return distance;
//        }

//        /// <summary>
//        /// Retrieves any current distance from the customer database or at does a lookup to PostcodeAnywhere
//        /// </summary>
//        /// <param name="locationa"></param>
//        /// <param name="locationb"></param>
//        /// <param name="employeeid"></param>
//        /// <param name="date"></param>
//        /// <returns></returns>
//        public decimal getDistance(int locationa, int locationb, int employeeid, DateTime? date)
//        {
//            cEmployees clsemployees = new cEmployees(accountid);
//            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
//            decimal distance = 0;
//            decimal postcodeanywheredistance = 0;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            cCompany fromcompany = GetCompanyById(locationa);
//            cCompany tocompany = GetCompanyById(locationb);

//            if (fromcompany == null || tocompany == null)
//            {
//                return (decimal)distance;
//            }

//            if (date.HasValue)
//            {

//            if (fromcompany.company.Trim().ToLower() == "home")
//            {
//                    cEmployeeHomeLocation homeLocation = reqemp.GetHomeAddresses().GetBy(date.Value);

//                    if (homeLocation != null)
//                {
//                        locationa = homeLocation.LocationID;
//                        fromcompany = this.GetCompanyById(locationa);
//                }
//            }
//            else if (fromcompany.company.Trim().ToLower() == "office")
//            {
//                    cEmployeeWorkLocation workLocation = reqemp.GetWorkAddresses().GetBy(date.Value);
//                    if (workLocation != null)
//                {
//                        locationa = workLocation.LocationID;
//                        fromcompany = this.GetCompanyById(locationa);
//                }
//            }

//            if (tocompany.company.Trim().ToLower() == "home")
//            {
//                    cEmployeeHomeLocation homeLocation = reqemp.GetHomeAddresses().GetBy(date.Value);

//                    if (homeLocation != null)
//                {
//                        locationb = homeLocation.LocationID;
//                        tocompany = this.GetCompanyById(locationb);
//                }
//            }
//            else if (tocompany.company.Trim().ToLower() == "office")
//            {
//                    cEmployeeWorkLocation workLocation = reqemp.GetWorkAddresses().GetBy(date.Value);
//                    if (workLocation != null)
//                {
//                        locationb = workLocation.LocationID;
//                        tocompany = this.GetCompanyById(locationb);
//                    }
//                }
//            }

//            cAccountSubAccounts clsAccountSubAccounts = new cAccountSubAccounts(accountid);
//            cAccountProperties clsProperties = clsAccountSubAccounts.getSubAccountById(reqemp.DefaultSubAccount).SubAccountProperties; // clsAccountSubAccounts.getFirstSubAccount().SubAccountProperties;

//            string strsql = "select distance, postcodeAnywhereFastestDistance, postcodeAnywhereShortestDistance from location_distances where (locationa = @locationa and locationb = @locationb)";// or (locationa = @locationb and locationb = @locationa)";
//            expdata.sqlexecute.Parameters.AddWithValue("@locationa", locationa);
//            expdata.sqlexecute.Parameters.AddWithValue("@locationb", locationb);

//            SqlDataReader reader = expdata.GetReader(strsql);

//            expdata.sqlexecute.Parameters.Clear();
//            while (reader.Read())
//            {

//                if (reader.IsDBNull(reader.GetOrdinal("distance")) == false)
//                {
//                    distance = reader.GetDecimal(reader.GetOrdinal("distance"));
//                }

//                if (clsProperties.MileageCalcType == 1)
//                {
//                    if (reader.IsDBNull(reader.GetOrdinal("postcodeAnywhereShortestDistance")) == false)
//                    {
//                        postcodeanywheredistance = reader.GetDecimal(reader.GetOrdinal("postcodeAnywhereShortestDistance"));
//                    }
//                }
//                else
//                {
//                    if (reader.IsDBNull(reader.GetOrdinal("postcodeAnywhereFastestDistance")) == false)
//                    {
//                        postcodeanywheredistance = reader.GetDecimal(reader.GetOrdinal("postcodeAnywhereFastestDistance"));
//                    }
//                }
//            }

//            reader.Close();

//            // get from Postcode anywhere
//            try
//            {
//                if (clsProperties.UseMapPoint == true)
//                {
//                    if (distance == 0)
//                    {
//                        if (postcodeanywheredistance > 0)
//                        {
//                            distance = postcodeanywheredistance;
//                        }

//                        if (fromcompany.postcode.Length > 0 && tocompany.postcode.Length > 0 && postcodeanywheredistance == 0)
//                        {
//                            PostcodeAnywhere postcodeAnywhere = new PostcodeAnywhere(accountid);
//                            cAccounts accounts = new cAccounts();
//                            cAccount account = accounts.GetAccountByID(this.nAccountid);

//                            if (account.MapsEnabled)
//                            {
//                                postcodeanywheredistance = postcodeAnywhere.GetDistanceByClosestRoadOnRouteToPostcodeCentre(fromcompany.postcode, tocompany.postcode, string.Empty, clsProperties.MileageCalcType == 1 ? DistanceType.Shortest : DistanceType.Fastest);
//                            }
//                            else
//                            {                               
//                                postcodeanywheredistance = postcodeAnywhere.LookupDistanceByPostcodeCentres(fromcompany.postcode, tocompany.postcode, DistanceType.DRIVETIME, clsProperties.MileageCalcType == 1 ? Cost.DISTANCE : Cost.TIME);
//                            }

//                            if (postcodeanywheredistance >= -1 )
//                            {

//                                updateDistance(locationa, locationb, distance, postcodeanywheredistance);

//                                distance = postcodeanywheredistance;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                return (decimal)distance;
//            }

//            return (decimal)distance;
//        }

//        public System.Data.DataSet searchLocations(bool from, string location, string locationcode)
//        {
//            string strsql;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            System.Data.DataSet ds;
//            strsql = "select companyid, company, companycode from companies where archived = 0 and ";
//            if (from == true)
//            {
//                strsql += "showfrom = 1";
//            }
//            else
//            {
//                strsql += "showto = 1";
//            }

//            if (location != "")
//            {
//                location = "%" + location + "%";
//                strsql += " and company like @location";
//                expdata.AddWithValue("@location", location, fields.GetFieldSize("companies", "company"));
//            }
//            if (locationcode != "")
//            {
//                locationcode = "%" + locationcode + "%";
//                strsql += " and companycode like @locationcode";
//                expdata.AddWithValue("@locationcode", locationcode, fields.GetFieldSize("companies", "companycode"));
//            }
//            strsql += " order by company";

//            ds = expdata.GetDataSet(strsql);
//            expdata.sqlexecute.Parameters.Clear();
//            return ds;
//        }

//        public List<System.Web.UI.WebControls.ListItem> CreateMergeDropDown(string defaultSelectOption)
//        {
//            SortedList<string, int> sorted = getCompaniesForDropdown();
//            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();
            
//            if (defaultSelectOption != null)
//            {
//                items.Add(new System.Web.UI.WebControls.ListItem(defaultSelectOption, "0"));
//            }

//            foreach (KeyValuePair<string,int> i in sorted)
//            {
                
//                items.Add(new System.Web.UI.WebControls.ListItem(i.Key, i.Value.ToString()));
//            }

//            return items;
//        }

//        private SortedList<string, int> getCompaniesForDropdown()
//        {
//            SortedList<string, int> list = new SortedList<string, int>();
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql = "select companyid, company from companies order by company";
//            using (SqlDataReader reader = expdata.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    list.Add(reader.GetString(1), reader.GetInt32(0));
//                }

//                reader.Close();
//            }

//            return list;
//        }

//        /// <summary>
//        /// Create a dropdown list
//        /// </summary>
//        /// <param name="type">Company, From, To or all</param>
//        /// <returns></returns>
//        public List<ListItem> CreateDropDown(CompanyType type)
//        {
//            var list = new SortedList<string, int> { { string.Empty, 0 } };
//            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
//            var strsql = "select companyid, company from companies where archived = 0 ";

//            switch (type)
//            {
//                case CompanyType.Company:
//                    strsql += "and iscompany = 1";
//                    break;
//                case CompanyType.From:
//                    strsql += "and showfrom = 1";
//                    break;
//                case CompanyType.To:
//                    strsql += "and showto = 1";
//                    break;
//            }

//            strsql += " order by company";

//            using (SqlDataReader reader = expdata.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    list.Add(reader.GetString(1), reader.GetInt32(0));
//                }

//                reader.Close();
//            }

//            return list.Select(i => new ListItem(i.Key, i.Value.ToString())).ToList();
//        }

//        public Dictionary<int, cCompany> getModifiedCompanies(DateTime date)
//        {
//            cCompany company;
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql = "select companyid from companies where createdon > @date or modifiedon > @date";
//            expdata.sqlexecute.Parameters.AddWithValue("@date", date);

//            Dictionary<int, cCompany> lst = new Dictionary<int, cCompany>();
//            using (SqlDataReader reader = expdata.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    company = GetCompanyById(reader.GetInt32(0));
//                    lst.Add(company.companyid, company);
//                }

//                reader.Close();
//            }

//            return lst;
//        }

//        public Dictionary<int, string> GetCompanyPostcodes(List<int> companyIDs)
//        {
//            DBConnection db = new DBConnection(cAccounts.getConnectionString(this.accountid));
//            Dictionary<int, string> companiesAndPostcodes = new Dictionary<int, string>();

//            string sql = companyIDs.Aggregate("SELECT companyid, postcode FROM companies WHERE companyid IN (", (current, companyID) => current + (companyID + ","));
//            sql = sql.Remove(sql.Length - 1) + ") AND postcode IS NOT NULL";

//            using (SqlDataReader reader = db.GetReader(sql))
//            {
//                while (reader.Read())
//                {
//                    companiesAndPostcodes.Add(reader.GetInt32(0), reader.GetString(1));
//                }

//                reader.Close();
//            }

//            return companiesAndPostcodes;
//        }

//        public List<int> getCompIds()
//        {
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strsql = "select companyid from companies";

//            List<int> ids = new List<int>();

//            using (SqlDataReader reader = expdata.GetReader(strsql))
//            {
//                while (reader.Read())
//                {
//                    ids.Add(reader.GetInt32(0));
//                }
//                reader.Close();
//            }
//            return ids;
//        }

//        public int count
//        {
//            get {
//                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
//                string strsql = "select count(*) from companies";
//                int count = expdata.getcount(strsql);
//                return count;
//            }
//        }

//        public string[] getAddress(string country, string postcode,int countryID)
//        {
//            List<PostcodeAnywhereAddress> tmpAddress;

//            //Check DB for Address
//            tmpAddress = SearchForAddresses("", "", "", "", "", postcode,countryID, CompanyType.Company, 0, ConditionJoiner.And);

//            if (tmpAddress.Count == 0)
//            {
//                //Check WS for Address
//                var postcodeAnywhere = new PostcodeAnywhere(accountid);
//                tmpAddress = postcodeAnywhere.LookupInternationalAddresses(string.Empty, postcode, country);
//            }

//            if (tmpAddress != null)
//            {
//                var address = new string[5];
//                address[0] = tmpAddress[0].AddressLine1;
//                address[1] = tmpAddress[0].AddressLine2;
//                address[2] = tmpAddress[0].City;
//                address[3] = tmpAddress[0].County;
//                address[4] = tmpAddress[0].Postcode;
//                return address;
//            }

//            return new string[0];
//        }

//        #region New AddressSearchModal methods
//        public enum GetRecentAddressType
//        {
//            MostVisited,
//            LastX
//        }

//        /// <summary>
//        /// Clears the address cache for an employee
//        /// </summary>
//        public void ClearGetLocationModalAddressListsCache(int employeeID)
//        {
//            Cache.Remove("locationSearch_" + CompanyType.Company + "_" + GetRecentAddressType.MostVisited + "_" + employeeID + "_" + accountid);
//            Cache.Remove("locationSearch_" + CompanyType.From + "_" + GetRecentAddressType.MostVisited + "_" + employeeID + "_" + accountid);
//            Cache.Remove("locationSearch_" + CompanyType.None + "_" + GetRecentAddressType.MostVisited + "_" + employeeID + "_" + accountid);
//            Cache.Remove("locationSearch_" + CompanyType.To + "_" + GetRecentAddressType.MostVisited + "_" + employeeID + "_" + accountid);
//            Cache.Remove("locationSearch_" + CompanyType.Company + "_" + GetRecentAddressType.LastX + "_" + employeeID + "_" + accountid);
//            Cache.Remove("locationSearch_" + CompanyType.From + "_" + GetRecentAddressType.LastX + "_" + employeeID + "_" + accountid);
//            Cache.Remove("locationSearch_" + CompanyType.None + "_" + GetRecentAddressType.LastX + "_" + employeeID + "_" + accountid);
//            Cache.Remove("locationSearch_" + CompanyType.To + "_" + GetRecentAddressType.LastX + "_" + employeeID + "_" + accountid);
//        }

//        /// <summary>
//        /// Returns a list of addresses for either most visited or last visited
//        /// </summary>
//        /// <param name="employeeID"></param>
//        /// <param name="companyType"></param>
//        /// <param name="type"></param>
//        /// <param name="limit"></param>
//        /// <returns></returns>
//        public List<string> GetLocationModalAddressLists(int employeeID, CompanyType companyType, GetRecentAddressType type, int limit)
//        {
//            List<string> lstResults = new List<string>();
//            string cacheKey = "locationSearch_" + companyType + "_" + type + "_" + employeeID + "_" + accountid;

//            lstResults = (List<string>)Cache[cacheKey];

//            if (lstResults == null)
//            {
//                lstResults = new List<string>();
//                DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
//                string strSQL = "GetLocationModalAddressLists";
//                db.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
//                db.sqlexecute.Parameters.AddWithValue("@companyType", Convert.ToInt32(companyType));
//                db.sqlexecute.Parameters.AddWithValue("@listType", Convert.ToInt32(type));

//                SqlDataReader reader;

//                using (reader = db.GetStoredProcReader(strSQL))
//                {
//                    while (reader.Read())
//                    {
//                        cEmployeeHomeLocation homeAddress = null;
//                        StringBuilder addressString = new StringBuilder();

//                        //Get the ID of the address location
//                        int addressID = reader.GetInt32(0);
//                        //Get whether the address location is private
//                        bool isPrivateAddress = reader.GetBoolean(5);

//                        //Check the private address is the employees current home address 
//                        if (isPrivateAddress)
//                        {
//                            cEmployees clsEmployees = new cEmployees(accountid);
//                            Employee employee = clsEmployees.GetEmployeeById(employeeID);
//                            homeAddress = employee.GetHomeAddresses().GetBy(addressID);
//                        }

//                        addressString.Append(addressID + "|");
//                        if (reader.IsDBNull(1) == false)
//                        {
//                            addressString.Append(reader.GetString(1) + "||");
//                            addressString.Append(reader.GetString(1) + ", ");
//                        }

//                        if (isPrivateAddress == false || (isPrivateAddress == true && homeAddress != null && homeAddress.EmployeeID == employeeID))
//                        if (!isPrivateAddress || homeAddress != null)
//                        {
//                            if (reader.IsDBNull(2) == false)
//                            {
//                                addressString.Append(reader.GetString(2) + ", ");
//                            }

//                            if (reader.IsDBNull(3) == false)
//                            {
//                                addressString.Append(reader.GetString(3) + ", ");
//                            }

//                            if (reader.IsDBNull(4) == false)
//                            {
//                                addressString.Append(reader.GetString(4) + ", ");

//                            }
//                        }

//                        addressString.Remove(addressString.Length - 2, 2);

//                        if (addressString.Length > (reader.GetInt32(0).ToString().Length + 1))
//                        {
//                            lstResults.Add(Convert.ToString(addressString.ToString()));
//                        }
//                    }
//                    reader.Close();
//                }
//                db.sqlexecute.Parameters.Clear();

//                // This is not being placed into cache due to the high chance the user could be adding a location - putting a dependancy on this is not a good idea
//                // However it does get cached in javascript via the AddressSearchModal object
//                if (lstResults.Count > 0)
//                {
//                    Cache.Insert(cacheKey, lstResults, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.UltraShort), CacheItemPriority.Default, null);
//                }
//            }

//            return lstResults;
//        }

//        /// <summary>
//        /// A list of addresses will be returned based on the passed in address criteria
//        /// </summary>
//        /// <param name="addressName">Name of the address location</param>
//        /// <param name="addressLine1">First line of the address location</param>
//        /// <param name="addressLine2">Second line of the address location</param>
//        /// <param name="city">City or town of the address location</param>
//        /// <param name="county">County or state of the address location</param>
//        /// <param name="postcode">Postcode or zip of the address location</param>
//        /// <param name="countryID">Global country ID of the address location</param>
//        /// <param name="companyType">Type i.e Company, From Or To of the address location</param>
//        /// <param name="conditionJoiner">If all conditions must match or just some of them - valid options are and/or</param>
//        /// <returns>A collection of addresses</returns>
//        public List<PostcodeAnywhereAddress> SearchForAddresses(string addressName, string addressLine1, string addressLine2, string city, string county, string postcode, int countryID, CompanyType companyType, int employeeID, ConditionJoiner conditionJoiner)
//        {
//            List<PostcodeAnywhereAddress> lstMatches = new List<PostcodeAnywhereAddress>();

//            StringBuilder query = new StringBuilder();
//            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

//            // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
//            cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);
//            int subAccountID = subaccs.getFirstSubAccount().SubAccountID;

//            cGlobalCountries clsGlobalCountries = new cGlobalCountries();
//            cCountries clsCountries = new cCountries(accountid, subAccountID);
//            cCountry reqCountry = clsCountries.getCountryById(countryID);
//            cGlobalCountry reqGlobalCountry = null;

//            if(reqCountry != null) 
//            {
//                reqGlobalCountry = clsGlobalCountries.getGlobalCountryById(reqCountry.globalcountryid);
//            }

//            string strsql = "SELECT companies.companyid, companies.company, companies.address1, companies.address2, companies.city, companies.county, companies.postcode, companies.isPrivateAddress, global_countries.country FROM companies LEFT JOIN global_countries ON global_countries.globalcountryid = companies.country where companies.archived = 0";
//            switch (companyType)
//            {
//                case CompanyType.Company:
//                    strsql += " and companies.iscompany = 1";
//                    break;
//                case CompanyType.From:
//                    strsql += " and companies.showfrom = 1";
//                    break;
//                case CompanyType.To:
//                    strsql += " and companies.showto = 1";
//                    break;
//            }

//            #region Set the filter criteria for the query
//            string condition = "and ";
//            if (conditionJoiner == ConditionJoiner.Or)
//            {
//                condition = "or ";
//            }
            


//            if (addressName != "")
//            {
//                query.Append("companies.company like @name " + condition);
//                expdata.AddWithValue("@name", addressName + "%", 250);
//            }

//            if (addressLine1 != "")
//            {
//                query.Append("companies.address1 like @address1 " + condition);
//                expdata.AddWithValue("@address1", addressLine1 + "%", 250);
//            }
//            if (addressLine2 != "")
//            {
//                query.Append("companies.address2 like @address2 " + condition);
//                expdata.AddWithValue("@address2", addressLine2 + "%", 250);
//            }
//            if (city != "")
//            {
//                query.Append("companies.city like @city " + condition);
//                expdata.sqlexecute.Parameters.AddWithValue("@city", city + "%");
//            }
//            if (county != "")
//            {
//                query.Append("companies.county like @county " + condition);
//                expdata.AddWithValue("@county", county + "%", 250);
//            }
//            if (postcode != "")
//            {
//                query.Append("companies.postcode like @postcode " + condition);
//                expdata.AddWithValue("@postcode", postcode + "%", 250);
//            }

//            #endregion

//            if (query.Length > 0)
//            {
//                query.Remove(query.Length - 4, 4);
//                if (reqGlobalCountry != null)
//                {
//                    query.Append(" AND (companies.country = @country OR companies.country IS NULL)");
//                    expdata.AddWithValue("@country", reqGlobalCountry.globalcountryid, 250);
//                }
//                strsql += " and (" + query.ToString() + ")"; 
//            }


//            using (SqlDataReader reader = expdata.GetReader(strsql))
//            {
//                int rdrAddressID;
//                string rdrAddressName;
//                string rdrAddressLine1;
//                string rdrAddressLine2;
//                string rdrCity;
//                string rdrCounty;
//                string rdrPostcode;
//                string rdrCountry;
//                bool isPrivateAddress;

//                cEmployeeHomeLocation homeAddress;

//                while (reader.Read())
//                {
//                    #region Get values from reader

//                    rdrAddressLine1 = "";
//                    rdrAddressLine2 = "";
//                    rdrCity = "";
//                    rdrCounty = "";
//                    rdrPostcode = "";
//                    rdrCountry = "";
//                    homeAddress = null;

//                    rdrAddressID = reader.GetInt32(0);

//                    if (reader.IsDBNull(1) == false)
//                    {
//                        rdrAddressName = reader.GetString(1);
//                    }
//                    else
//                    {
//                        rdrAddressName = "";
//                    }

//                    isPrivateAddress = reader.GetBoolean(7);

//                    //Check the private address is the employees current home address 
//                    if (isPrivateAddress && employeeID != 0)
//                    {
//                        cEmployees clsEmployees = new cEmployees(accountid);
//                        Employee employee = clsEmployees.GetEmployeeById(employeeID);
//                        homeAddress = employee.GetHomeAddresses().GetBy(rdrAddressID);
//                    }

//                    if (!isPrivateAddress || homeAddress != null)
//                    {
//                        if (reader.IsDBNull(2) == false)
//                        {
//                            rdrAddressLine1 = reader.GetString(2);
//                        }

//                        if (reader.IsDBNull(3) == false)
//                        {
//                            rdrAddressLine2 = reader.GetString(3);
//                        }
                       
//                        if (reader.IsDBNull(4) == false)
//                        {
//                            rdrCity = reader.GetString(4);
//                        }
                        
//                        if (reader.IsDBNull(5) == false)
//                        {
//                            rdrCounty = reader.GetString(5);
//                        }

//                        if (reader.IsDBNull(6) == false)
//                        {
//                            rdrPostcode = reader.GetString(6);
//                        }

//                        if (reader.IsDBNull(8) == false)
//                        {
//                            rdrCountry = reader.GetString(8);
//                        }
//                    }
//                    #endregion

//                    //Add values to the collection
//                    lstMatches.Add(new PostcodeAnywhereAddress(rdrAddressID, rdrAddressName, rdrAddressLine1, rdrAddressLine2, rdrCity, rdrCounty, rdrPostcode, rdrCountry, isPrivateAddress));
//                }

//                reader.Close();
//            }

//            expdata.sqlexecute.Parameters.Clear();

//            return lstMatches;
//        }


//        /// <summary>
//        /// Cheks to see if the specified prefixText matches a single address, if it matches multiple then null is returned
//        /// </summary>
//        /// <param name="prefixText"></param>
//        /// <param name="companyType"></param>
//        /// <returns></returns>
//        public List<List<string>> CheckAddressLocation(string prefixText, DateTime date, CompanyType companyType, int employeeID, int esrAssigmentId) 
//        {
//            List<List<string>> locationDetails = new List<List<string>>();
//            List<string> locationSpecifics;
//            cEmployees clsEmployees = new cEmployees(accountid);
//            Employee employee = clsEmployees.GetEmployeeById(employeeID);

//            long? esrLocationId = null;
//            if (prefixText.Trim().ToLower() == "home" || prefixText.Trim().ToLower() == "office")
//            {

//                cCompany tmpCompany = null;

//                if (prefixText.Trim().ToLower() == "home")
//                {
//                    cEmployeeHomeLocation tmpHomeLocation = employee.GetHomeAddresses().GetBy(date);
//                    if (tmpHomeLocation != null)
//                    {
//                        tmpCompany = GetCompanyById(tmpHomeLocation.LocationID);
//                    }
//                }
//                else
//                {
                    
//                    // if an ESR assignment number is present then get the ESR location ID for that assignment
//                    if (esrAssigmentId != 0)
//                    {
//                        var esrAssignments = new cESRAssignments(this.accountid, employeeID);
//                        var esrAssignment = esrAssignments.getAssignmentById(esrAssigmentId);
//                        esrLocationId = esrAssignment.esrLocationId;
//                    }

//                    cEmployeeWorkLocation tmpWorkLocation = employee.GetWorkAddresses().GetBy(date, esrLocationId);
//                    if (tmpWorkLocation != null)
//                    {
//                        tmpCompany = GetCompanyById(tmpWorkLocation.LocationID);
//                    }
//                    else if (esrAssigmentId != 0)
//                    {
//                        tmpWorkLocation = employee.GetWorkAddresses().GetBy(date);
//                        if (tmpWorkLocation != null)
//                        {
//                            tmpCompany = GetCompanyById(tmpWorkLocation.LocationID);
//                        }   
//                    }
//                }

//                if (tmpCompany != null)
//                {
//                    locationSpecifics = new List<string>();
//                    locationSpecifics.Add(tmpCompany.companyid.ToString());
//                    locationSpecifics.Add(tmpCompany.company);
//                    locationSpecifics.Add(esrLocationId.ToString());
//                    locationDetails.Add(locationSpecifics);
//                }
//            }
//            else
//            {
//                SortedList<int, string> location = GetAddressLocationsByPrefixText(prefixText, companyType);

//                    foreach (KeyValuePair<int, string> kvp in location)
//                    {
//                        locationSpecifics = new List<string>();
//                        locationSpecifics.Add(kvp.Key.ToString());
//                        locationSpecifics.Add(kvp.Value);
//                        locationSpecifics.Add(esrLocationId.ToString());
//                        locationDetails.Add(locationSpecifics);
//                    }
//            }
//            return locationDetails;
//        }


//        /// <summary>
//        /// Get the matching address locations based on the location type and prefixed string value from the database. This Returns the top 15 matches only!
//        /// </summary>
//        /// <param name="prefixText">Prefixed location name value</param>
//        /// <param name="companyType">Location type</param>
//        /// <returns>A list of address locations</returns>
//        public SortedList<int, string> GetAddressLocationsByPrefixText(string prefixText, CompanyType companyType)
//        {
//            SortedList<int, string> lstAddressLocations = new SortedList<int, string>();
//            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strSQL = "GetAddressLocationsByPrefixText";
//            db.AddWithValue("@company", prefixText + "%", 250);
//            db.sqlexecute.Parameters.AddWithValue("@companyType", (byte)companyType);



//            using (SqlDataReader reader = db.GetStoredProcReader(strSQL))
//            {
//                while (reader.Read())
//                {
//                    lstAddressLocations.Add(reader.GetInt32(0), reader.GetString(1));
//                 }

//                reader.Close();
//            }

//            db.sqlexecute.Parameters.Clear();


//            /// See if home/office is in the list - if they are remove them as we don't want the user to be able to pick them via the autocompletes
//            int homeIndex = -1;
//            int officeIndex = -1;
//            foreach (KeyValuePair<int, string> kvp in lstAddressLocations)
//            {
//                if (kvp.Value.ToLower() == "home")
//                {
//                    homeIndex = kvp.Key;
//                }
//                else if (kvp.Value.ToLower() == "office")
//                {
//                    officeIndex = kvp.Key;
//                }
//            }

//            if (homeIndex > -1)
//            {
//                lstAddressLocations.Remove(homeIndex);
//            }

//            if (officeIndex > -1)
//            {
//                lstAddressLocations.Remove(officeIndex);
//            }
//            /// End home/office checking

//            return lstAddressLocations;
//        }

//        /// <summary>
//        /// Validates a countries postcode against the global countries regex pattern
//        /// </summary>
//        /// <param name="globalCountryID"></param>
//        /// <param name="postcode"></param>
//        /// <returns></returns>
//        public bool ValidatePostcode(int globalCountryID, string postcode)
//        {
//            cGlobalCountries clsGlobalCountries = new cGlobalCountries();
//            cGlobalCountry clsGlobalCountry = clsGlobalCountries.getGlobalCountryById(globalCountryID);

//            if (clsGlobalCountry != null)
//            {

//                if (clsGlobalCountry.PostcodeRegex == string.Empty || postcode == string.Empty)
//                {
//                    return true;
//                }

//                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(clsGlobalCountry.PostcodeRegex, System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
//                if (regex.IsMatch(postcode) == true)
//                {
//                    return true;
//                }

//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }


//        #endregion New AddressSearchModal methods

//        #region AddressMerge

//        /// <summary>
//        /// Get a list of addresses that match a text prefix. This prefix can match to any of the values for the address
//        /// </summary>
//        /// <param name="prefixText">The text to search on</param>
//        /// <param name="filterName"></param>
//        /// <param name="filterAddress1"></param>
//        /// <param name="filterCity"></param>
//        /// <param name="filterCounty"></param>
//        /// <param name="filterPostcode"></param>
//        /// <returns></returns>
//        public List<PostcodeAnywhereAddress> GetAddressSearchResults(string prefixText, bool filterName, bool filterAddress1, bool filterCity, bool filterCounty, bool filterPostcode)
//        {
//            List<PostcodeAnywhereAddress> lstResults = new List<PostcodeAnywhereAddress>();
//            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
//            string strSQL = "GetAddressSearchResults";
//            db.AddWithValue("@prefixText", prefixText + "%", 250);
//            db.sqlexecute.Parameters.AddWithValue("@filterName", filterName);
//            db.sqlexecute.Parameters.AddWithValue("@filterAddress1", filterAddress1);
//            db.sqlexecute.Parameters.AddWithValue("@filterCity", filterCity);
//            db.sqlexecute.Parameters.AddWithValue("@filterCounty", filterCounty);
//            db.sqlexecute.Parameters.AddWithValue("@filterPostcode", filterPostcode);

//            using (SqlDataReader reader = db.GetStoredProcReader(strSQL))
//            {
//                int rdrAddressID;
//                string rdrAddressName;
//                string rdrAddressLine1;
//                string rdrCity;
//                string rdrCounty;
//                string rdrPostcode;

//                while (reader.Read())
//                {
//                    #region Get values from reader

//                    rdrAddressLine1 = "";
//                    rdrCity = "";
//                    rdrCounty = "";
//                    rdrPostcode = "";

//                    rdrAddressID = reader.GetInt32(0);

//                    if (reader.IsDBNull(1) == false)
//                    {
//                        rdrAddressName = reader.GetString(1);
//                    }
//                    else
//                    {
//                        rdrAddressName = "";
//                    }

//                    if (reader.IsDBNull(2) == false)
//                    {
//                        rdrAddressLine1 = reader.GetString(2);
//                    }
                   
//                    if (reader.IsDBNull(3) == false)
//                    {
//                        rdrCity = reader.GetString(3);
//                    }
                    
//                    if (reader.IsDBNull(4) == false)
//                    {
//                        rdrCounty = reader.GetString(4);
//                    }

//                    if (reader.IsDBNull(5) == false)
//                    {
//                        rdrPostcode = reader.GetString(5);
//                    }

//                    lstResults.Add(new PostcodeAnywhereAddress(rdrAddressID, rdrAddressName, rdrAddressLine1, "", rdrCity, rdrCounty, rdrPostcode, "", false));

//                    #endregion
//                }

//                reader.Close();
//            }

//            return lstResults;
//        }

//        /// <summary>
//        /// Get the counters of any addresses that will be affected by the merge
//        /// </summary>
//        /// <param name="lstMergeIDs"></param>
//        /// <returns></returns>
//        public List<int> GetInformationOnMerge(List<int> lstMergeIDs)
//        {
//            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
//            string mergeIDs = "";
//            List<int> lstInfoCounts = new List<int>();
//            int fromCount, toCount, companyCount, homeCount, workCount;

//            foreach(int val in lstMergeIDs)
//            {
//                mergeIDs += val + ", ";
//            }

//            if (mergeIDs != "")
//            {
//                mergeIDs = mergeIDs.Remove(mergeIDs.Length - 2, 2);
//            }

//            #region Get the count of all expenses items that have any of the merge IDs within the 'from' address of the journey

//            db.AddWithValue("@mergeIDs", mergeIDs, 4000);
//            db.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
//            db.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
//            db.ExecuteProc("GetExpenseItemFromAddressMergeCount");
//            fromCount = (int)db.sqlexecute.Parameters["@returnvalue"].Value;

//            lstInfoCounts.Add(fromCount);

//            #endregion

//            #region Get the count of all expenses items that have any of the merge IDs within the 'to' address of the journey

//            db.ExecuteProc("GetExpenseItemToAddressMergeCount");
//            toCount = (int)db.sqlexecute.Parameters["@returnvalue"].Value;

//            lstInfoCounts.Add(toCount);

//            #endregion

//            #region Get the count of all expenses items that have any of the merge IDs within the 'company' address of the expense item

//            db.ExecuteProc("GetExpenseItemCompanyAddressMergeCount");
//            companyCount = (int)db.sqlexecute.Parameters["@returnvalue"].Value;
//            lstInfoCounts.Add(companyCount);

//            #endregion

//            #region Get the count of all employee home addresses that have any of the merge IDs 

//            db.ExecuteProc("GetEmployeeHomeAddressMergeCount");
//            homeCount = (int)db.sqlexecute.Parameters["@returnvalue"].Value;
//            lstInfoCounts.Add(homeCount);

//            #endregion

//            #region Get the count of all employee work addresses that have any of the merge IDs

//            db.ExecuteProc("GetEmployeeWorkAddressMergeCount");
//            workCount = (int)db.sqlexecute.Parameters["@returnvalue"].Value;
//            lstInfoCounts.Add(workCount);

//            #endregion

//            db.sqlexecute.Parameters.Clear();

//            return lstInfoCounts;
//        }

//        /// <summary>
//        /// This method will merge the addresses that are alike an existing one replacing all addresses for the merge with the master
//        /// one to merge to, checks on the expense items, home addresses and work addresses are made to see what needs to be replaced.
//        /// Once the address replace is complete the obsolete addresses are deleted. This is all done via a stored procedure which is 
//        /// transacted so if an error occurs during the merge the whole process is rolled back.
//        /// </summary>
//        /// <param name="mergeToID"></param>
//        /// <param name="lstMergeIDs"></param>
//        /// <returns></returns>
//        public List<int> MergeAddresses(int mergeToID, List<int> lstMergeIDs)
//        {
//            CurrentUser currentUser = cMisc.GetCurrentUser();
//            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
//            List<int> lstUpdateCounts = new List<int>();
//            string mergeIDs = "";

//            foreach (int val in lstMergeIDs)
//            {
//                mergeIDs += val + ", ";
//            }

//            if (mergeIDs != "")
//            {
//                mergeIDs = mergeIDs.Remove(mergeIDs.Length - 2, 2);
//            }

//            db.sqlexecute.Parameters.AddWithValue("@newCompanyID", mergeToID);
//            db.AddWithValue("@mergeIDs", mergeIDs, 4000);
            
//            if (currentUser != null)
//            {
//                db.sqlexecute.Parameters.AddWithValue("@userID", currentUser.EmployeeID);
//                if (currentUser.isDelegate == true)
//                {
//                    db.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
//                }
//                else
//                {
//                    db.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
//                }
//            }
//            else
//            {
//                db.sqlexecute.Parameters.AddWithValue("@userID", -1);
//                db.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
//            }

//            using (SqlDataReader reader = db.GetStoredProcReader("MergeAddresses"))
//            {
//                #region The return information from the reader is the area address counters that got set from the merge update

//                while (reader.Read())
//                {
//                    lstUpdateCounts.Add(reader.GetInt32(0)); //From address count
//                    lstUpdateCounts.Add(reader.GetInt32(1)); //To address count
//                    lstUpdateCounts.Add(reader.GetInt32(2)); //Company address count
//                    lstUpdateCounts.Add(reader.GetInt32(3)); //Home address count
//                    lstUpdateCounts.Add(reader.GetInt32(4)); //Work address count
//                }

//                #endregion

//                reader.Close();
//            }

//            db.sqlexecute.Parameters.Clear();

//            if (lstUpdateCounts[3] > 0)
//            {
//                this.UpdateEmployeeAddressCache(mergeToID, true);
//            }

//            if (lstUpdateCounts[4] > 0)
//            {
//                this.UpdateEmployeeAddressCache(mergeToID, false);
//            }

//            return lstUpdateCounts;
//        }

//        #endregion

//        private void UpdateEmployeeAddressCache(int newLocationIdentity, bool home)
//        {
//            Utilities.DistributedCaching.Cache cache = new Utilities.DistributedCaching.Cache();
//            string cacheArea = home ? EmployeeHomeAddresses.CacheArea : EmployeeWorkAddresses.CacheArea;

//            DBConnection db = new DBConnection(cAccounts.getConnectionString(accountid));
//            db.sqlexecute.Parameters.AddWithValue("@locationIdentity", newLocationIdentity);
//            db.sqlexecute.Parameters.AddWithValue("@homeLocations", home);

//            using (SqlDataReader reader = db.GetStoredProcReader("GetEmployeesByLocation"))
//            {
//                while (reader.Read())
//                {
//                    cache.Delete(this.nAccountid, cacheArea, reader.GetInt32(0).ToString(CultureInfo.InvariantCulture));
//                }

//                reader.Close();
//            }

//            db.sqlexecute.Parameters.Clear();
//        }

//        #region New Addresses

//        /// <summary>
//        /// Gets the desired address by its Capture Plus Id.
//        /// </summary>
//        /// <param name="capturePlusId">The Capture Plus Id to search by</param>
//        /// <param name="employeeId">The Employee Id to use</param>
//        /// <returns>The desired Address object</returns>
//        public CompanyWithAddress GetAddressByCapturePlusId(string capturePlusId, int employeeId)
//        {
//            CompanyWithAddress address = null;

//            // todo get from cache
//            var globalCountries = new cGlobalCountries();

//                var connection = new SqlConnection(cAccounts.getConnectionString(this.accountid));
//                var sqlCommand = new SqlCommand("GetAddressByCapturePlusId", connection) { CommandType = CommandType.StoredProcedure };

//                sqlCommand.Parameters.AddWithValue("@CapturePlusId", capturePlusId);
//                var dataAdapter = new SqlDataAdapter(sqlCommand);

//            var dataTable = new DataTable();
//            using (dataTable)
//                {
//                dataAdapter.Fill(dataTable);

//                if (dataTable.Rows.Count > 0)
//                {
//                    DataRow row = dataTable.Rows[0];
//                    address = new CompanyWithAddress(
//                                  int.Parse(row["AddressId"].ToString()),
//                                  row["company"].ToString(),
//                                  row["Address1"].ToString(),
//                                  row["Address2"].ToString(),
//                                  row["City"].ToString(),
//                                  row["County"].ToString(),
//                                  row["Postcode"].ToString(),
//                                  globalCountries.GetGlobalCountryIdByAlpha3Code(row["Country"].ToString()),
//                                  bool.Parse(row["IsPrivate"].ToString()),
//                                  (!row.IsNull("CreatedBy")) ? int.Parse(row["CreatedBy"].ToString()) : 0,
//                                  false);
//                }
//            }

//            if (address == null)
//            {
//                var pca = new PostcodeAnywhere(this.accountid);

//                var results = pca.CapturePlusInteractiveRetrieveById(capturePlusId);
//                var commonRows = results.Rows.Cast<DataRow>();
//                    var rows = commonRows as IList<DataRow> ?? commonRows.ToList();
//                    var line1 = rows.Where(r => r["FieldName"].ToString() == "Line1").Select(r => r["FormattedValue"]).First().ToString();

//                    var line2 = rows.Where(r => r["FieldName"].ToString() == "Line2").Select(r => r["FormattedValue"]).First().ToString();

//                    var city = rows.Where(r => r["FieldName"].ToString() == "City").Select(r => r["FormattedValue"]).First().ToString();

//                    var province = rows.Where(r => r["FieldName"].ToString() == "ProvinceName").Select(r => r["FormattedValue"]).First().ToString();

//                    var countryCode = rows.Where(r => r["FieldName"].ToString() == "CountryCode").Select(r => r["FormattedValue"]).First().ToString();

//                    var postCode = rows.Where(r => r["FieldName"].ToString() == "PostalCode").Select(r => r["FormattedValue"]).First().ToString();

//                    var name = this.GenerateAddressName(rows.Where(r => r["FieldName"].ToString() == "Label").Select(r => r["FormattedValue"]).First().ToString().Replace("\r\n", ", "), string.Empty);

//                    address = new CompanyWithAddress(0, name, line1, line2, city, province, postCode, globalCountries.GetGlobalCountryIdByAlpha3Code(countryCode), false, employeeId, false);

//                    address.companyid = this.StoreCapturePlusId(address, capturePlusId, employeeId);
//                }

//            return address;
//        }


//        /// <summary>
//        /// Stores the Capture Plus Id against all matching addresses. If the address does not exist it will be created.
//        /// </summary>
//        /// <param name="address">The postcode anywhere address to use</param>
//        /// <param name="capturePlusId">The capture plus Id to store</param>
//        /// <param name="employeeId">The employee Id to use</param>
//        /// <returns>An int representing the success value</returns>
//        private int StoreCapturePlusId(CompanyWithAddress address, string capturePlusId, int employeeId)
//        {
//            var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));

//            if (address.address1 == string.Empty && address.address2 == string.Empty)
//            {
//                // Do not save if empty address information is given
//                return -2;
//            }

//            expdata.AddWithValue("@company", address.company, this.fields.GetFieldSize("companies", "company"));

//            if (string.IsNullOrEmpty(address.address1))
//            {
//                expdata.AddWithValue("@address1", DBNull.Value, this.fields.GetFieldSize("companies", "address1"));
//            }
//            else
//            {
//                expdata.AddWithValue("@address1", address.address1, this.fields.GetFieldSize("companies", "address1"));
//            }

//            if (string.IsNullOrEmpty(address.address2))
//            {
//                expdata.AddWithValue("@address2", DBNull.Value, this.fields.GetFieldSize("companies", "address2"));
//            }
//            else
//            {
//                expdata.AddWithValue("@address2", address.address2, this.fields.GetFieldSize("companies", "address2"));
//            }

//            //// TODO: Update to use correct Field
//            /*if (string.IsNullOrEmpty(address.AddressLine3))
//            {
//                expdata.AddWithValue("@address3", DBNull.Value, this._fields.GetFieldSize("Address", "Address2"));
//            }
//            else
//            {
//                expdata.AddWithValue("@address3", address.AddressLine3, this._fields.GetFieldSize("Address", "Address2"));
//            }

//            //// TODO: Update to use correct Field
//            if (string.IsNullOrEmpty(address.AddressLine4))
//            {
//                expdata.AddWithValue("@address4", DBNull.Value, this._fields.GetFieldSize("Address", "Address2"));
//            }
//            else
//            {
//                expdata.AddWithValue("@address4", address.AddressLine4, this._fields.GetFieldSize("Address", "Address2"));
//            }*/

//            if (string.IsNullOrEmpty(address.city))
//            {
//                expdata.AddWithValue("@city", DBNull.Value, this.fields.GetFieldSize("companies", "city"));
//            }
//            else
//            {
//                expdata.AddWithValue("@city", address.city, this.fields.GetFieldSize("companies", "city"));
//            }

//            if (string.IsNullOrEmpty(address.county))
//            {
//                expdata.AddWithValue("@county", DBNull.Value, this.fields.GetFieldSize("companies", "county"));
//            }
//            else
//            {
//                expdata.AddWithValue("@county", address.county, this.fields.GetFieldSize("companies", "county"));
//            }

//            if (string.IsNullOrEmpty(address.postcode))
//            {
//                expdata.AddWithValue("@postcode", DBNull.Value, this.fields.GetFieldSize("companies", "postcode"));
//            }
//            else
//            {
//                expdata.AddWithValue("@postcode", address.postcode, this.fields.GetFieldSize("companies", "postcode"));
//            }

//            expdata.AddWithValue("@country", address.country, this.fields.GetFieldSize("companies", "country"));

//            expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeId);

//            expdata.sqlexecute.Parameters.AddWithValue("@capturePlusId", capturePlusId);

//            expdata.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
//            expdata.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;

//            expdata.ExecuteProc("SaveCapturePlusId");

//            var addressId = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;

//            expdata.sqlexecute.Parameters.Clear();

//            return addressId;
//        }        


//        /// <summary>
//        /// Searches the customer database for any address which match the given search term.
//        /// </summary>
//        /// <param name="searchTerm">The string to search for</param>
//        /// <returns>A list of matching addresses</returns>
//        public List<CompanyWithAddress> SearchAddresses(string searchTerm)
//        {
//            var connection = new SqlConnection(cAccounts.getConnectionString(this.accountid));

//            var sqlCommand = new SqlCommand("SearchAddresses", connection);

//            sqlCommand.Parameters.AddWithValue("@SearchTerm", searchTerm);

//            sqlCommand.CommandType = CommandType.StoredProcedure;

//            var dataAdapter = new SqlDataAdapter(sqlCommand);

//            var dataSet = new DataSet();

//            var matchedAddresses = new List<CompanyWithAddress>();

//            dataAdapter.Fill(dataSet);

//            if (dataSet.Tables[0] != null)
//            {
//                var globalCountries = new cGlobalCountries();

//                matchedAddresses.AddRange(
//                    from DataRow row in dataSet.Tables[0].Rows
//                    let company = row["company"].ToString()
//                    let companyId = int.Parse(row["AddressId"].ToString())
//                    let address1 = row["Address1"].ToString()
//                    let address2 = row["Address2"].ToString()
//                    let city = row["City"].ToString()
//                    let county = row["County"].ToString()
//                    let postcode = row["Postcode"].ToString()
//                    let isPrivate = bool.Parse(row["IsPrivate"].ToString())
//                    let createdBy = (!row.IsNull("CreatedBy")) ? int.Parse(row["CreatedBy"].ToString()) : 0 
//                    let countryId = globalCountries.GetGlobalCountryIdByAlpha3Code(row["Country"].ToString())
//                    select new CompanyWithAddress(companyId, company, address1, address2, city, county, postcode, countryId, isPrivate, createdBy, false));
//            }

//            return matchedAddresses;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="prefixText"></param>
//        /// <param name="date"></param>
//        /// <param name="companyType"></param>
//        /// <param name="employeeID"></param>
//        /// <param name="esrAssigmentId"></param>
//        /// <returns></returns>
//        public CompanyWithAddress GetAddressByReservedKeyword(string keyword, DateTime date, int employeeID, int esrAssigmentId)
//        {
//            keyword = keyword.Trim().ToLower();

//            List<List<string>> locationDetails = new List<List<string>>();
//            List<string> locationSpecifics;
//            cEmployees clsEmployees = new cEmployees(accountid);
//            Employee employee = clsEmployees.GetEmployeeById(employeeID);

//            CompanyWithAddress address = null;
//            long? esrLocationId = null;
//            if (keyword == "home")
//            {
//                cEmployeeHomeLocation tmpHomeLocation = employee.GetHomeAddresses().GetBy(date);
//                if (tmpHomeLocation != null)
//                {
//                    address = this.GetCompanyWithAddressById(tmpHomeLocation.LocationID);
//                }
//            }
//            else if (keyword == "office")
//            {
//                // if an ESR assignment number is present then get the ESR location ID for that assignment
//                if (esrAssigmentId != 0)
//                {
//                    var esrAssignments = new cESRAssignments(this.accountid, employeeID);
//                    var esrAssignment = esrAssignments.getAssignmentById(esrAssigmentId);
//                    esrLocationId = esrAssignment.esrLocationId;
//                }

//                cEmployeeWorkLocation tmpWorkLocation = employee.GetWorkAddresses().GetBy(date, esrLocationId);
//                if (tmpWorkLocation != null)
//                {
//                    address = this.GetCompanyWithAddressById(tmpWorkLocation.LocationID);
//                }
//            }

//            return address;
//        }

//        #endregion New Addresses
//    }


//    public enum CompanyType
//    {
//        Company,
//        From,
//        To,
//        None
//    }

//    public class cLocationDistance
//    {
//        private int nLocationA;
//        private int nLocationB;
//        private decimal dDistance;
//        //private decimal dPostcodeAnywhereDistance;
//        private decimal dPostcodeAnywhereFastestDistance;
//        private decimal dPostcodeAnywhereShortestDistance;

//        public cLocationDistance(int locationa, int locationb, decimal distance, decimal postcodeAnywhereFastestDistance, decimal postcodeAnywhereShortestDistance)
//        {
//            nLocationA = locationa;
//            nLocationB = locationb;
//            dDistance = distance;
//            dPostcodeAnywhereFastestDistance = postcodeAnywhereFastestDistance;
//            dPostcodeAnywhereShortestDistance = postcodeAnywhereShortestDistance;
//        }

//        #region properties

//        /// <summary>
//        /// ID of the first location
//        /// </summary>
//        public int LocationA
//        {
//            get { return nLocationA; }
//        }

//        /// <summary>
//        /// ID of the second location
//        /// </summary>
//        public int LocationB
//        {
//            get { return nLocationB; }
//        }

//        /// <summary>
//        /// Decimal value of the distance from Location A to B
//        /// </summary>
//        public decimal Distance
//        {
//            get {return dDistance;}
//        }
        

//        /// <summary>
//        /// This will be set if the global option for the postcode anywhere distance calculation is set to fastest
//        /// </summary>
//        public decimal PostcodeAnywhereFastestDistance
//        {
//            get { return dPostcodeAnywhereFastestDistance; }
//        }

//        /// <summary>
//        /// This will be set if the global option for the postcode anywhere distance calculation is set to shortest
//        /// </summary>
//        public decimal PostcodeAnywhereShortestDistance
//        {
//            get { return dPostcodeAnywhereShortestDistance; }
//        }
//        #endregion
//    }
//}
