using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using expenses.postcodeanywhereuk;
using SpendManagementLibrary;
namespace expenses
{
	
	/// <summary>
	/// Summary description for companies.
	/// Cache["companieslist" + accountid[
	/// </summary>
	/// 
	
	public class cCompanies
	{
        public System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
		
        private int nAccountid;


		public cCompanies(int accountid)
		{
            nAccountid = accountid;
            
        }

        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion
        
		private cCompanyDistance[] CacheCompanyDistances()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
			System.Data.SqlClient.SqlDataReader reader;
			int count;
			int i = 0;
			int locationa, locationb, createdby;
            decimal distance;
DateTime  createdon;
			strsql = "select count(*) from location_distances where locationa in (select companyid from companies) or locationb in (select companyid from companies)";
			count = expdata.getcount(strsql);

			cCompanyDistance[] compdist = new cCompanyDistance[count];

			strsql = "select locationa, locationb, distance, createdon, createdby from dbo.location_distances where locationa in (select companyid from dbo.companies) or locationb in (select companyid from dbo.companies)";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
			reader = expdata.GetReader(strsql);

			while (reader.Read())
			{
				
				locationa = reader.GetInt32(reader.GetOrdinal("locationa"));
				locationb = reader.GetInt32(reader.GetOrdinal("locationb"));
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
                distance = reader.GetDecimal(reader.GetOrdinal("distance"));
				compdist[i] = new cCompanyDistance(locationa, locationb, distance, createdon, createdby);
				distance = reader.GetDecimal(reader.GetOrdinal("distance"));

				i++;
			}
			reader.Close();
			
			Cache.Insert("locationdistances" + accountid,compdist,dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
			return compdist;
		}

		
		

		public int addOther(string company, bool from, int employeeid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SqlDataReader reader;
			int companyid = 0;
			int i;
			cCompany clscompany;

            string strsql = "select companyid from companies where company = @company";
            expdata.sqlexecute.Parameters.AddWithValue("@company", company);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                companyid = reader.GetInt32(0);
            }
            reader.Close();
            if (companyid > 0)
            {
                return companyid;
            }
			

			if (from == true)
			{
				companyid = addCompany(company,"","",true,false,new int[0,0], employeeid);
			}
			else
			{
				companyid =addCompany(company,"","",false,true,new int[0,0], employeeid);
			}
			
			

			return companyid;
			
		}

        public DataTable searchForLocations(string name, CompanyType companytype)
        {
            
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "select top 20 companyid, company from companies where (company like @name or postcode like @name) ";
            expdata.sqlexecute.Parameters.AddWithValue("@name", name + "%");
            switch (companytype)
            {
                case CompanyType.Company:
                    strsql += "and iscompany = 1";
                    break;
                case CompanyType.From:
                    strsql += "and showfrom = 1";
                    break;
                case CompanyType.To:
                    strsql += "and showto = 1";
                    break;
            }

            DataSet ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();

            return ds.Tables[0];
        }

        public DataTable searchForLocations(string name, string address1, string address2, string city, string county, string postcode, int country, CompanyType companytype)
        {
            StringBuilder query = new StringBuilder();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "select companyid, company from companies where archived = 0";
            switch (companytype)
            {
                case CompanyType.Company:
                    strsql += " and iscompany = 1";
                    break;
                case CompanyType.From:
                    strsql += " and showfrom = 1";
                    break;
                case CompanyType.To:
                    strsql += " and showto = 1";
                    break;
            }
            if (name != "")
            {
                query.Append("company like @name and ");
                expdata.sqlexecute.Parameters.AddWithValue("@name", name + "%");
            }
            
            if (address1 != "")
            {
                query.Append("address1 like @address1 and ");
                expdata.sqlexecute.Parameters.AddWithValue("@address1", address1 + "%");
            }
            if (address2 != "")
            {
                query.Append("address2 like @address2 and ");
                expdata.sqlexecute.Parameters.AddWithValue("@address2", address2 + "%");
            }
            if (city != "")
            {
                query.Append("city like @city and ");
                expdata.sqlexecute.Parameters.AddWithValue("@city", city + "%");
            }
            if (county != "")
            {
                query.Append("county like @county and ");
                expdata.sqlexecute.Parameters.AddWithValue("@county", county + "%");
            }
            if (postcode != "")
            {
                query.Append("postcode like @postcode and ");
                expdata.sqlexecute.Parameters.AddWithValue("@postcode", postcode + "%");
            }
            if (country > 0)
            {
                query.Append("country = @country and ");
                expdata.sqlexecute.Parameters.AddWithValue("@country", country);
            }

            

            if (query.Length > 0)
            {
                query.Remove(query.Length - 5, 5);
                strsql += " and " + query.ToString();
            }
            DataSet ds = expdata.GetDataSet(strsql);

            expdata.sqlexecute.Parameters.Clear();
            return ds.Tables[0];
        }

        public DataTable searchForAutoLogLocation(string name, string address1, string address2, string postcode)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            string strSQL = "SELECT companyid, company, archived, address1, address2, city, county, postcode, country FROM companies WHERE ";
            StringBuilder sbQuery = new StringBuilder();

            DataSet ds;

            if (name != "")
            {
                sbQuery.Append(" company LIKE @name OR ");
                expdata.sqlexecute.Parameters.AddWithValue("@name", name + "%");
            }

            if (address1 != "")
            {
                sbQuery.Append(" address1 LIKE @address1 OR ");
                expdata.sqlexecute.Parameters.AddWithValue("@address1", address1 + "%");
            }

            if (address2 != "")
            {
                sbQuery.Append(" address2 LIKE @address2 OR ");
                expdata.sqlexecute.Parameters.AddWithValue("@address2", address2 + "%");
            }

            if (postcode != "")
            {
                sbQuery.Append(" postcode LIKE @postcode OR ");
                expdata.sqlexecute.Parameters.AddWithValue("@postcode", postcode + "%");
            }

            strSQL += "(" + sbQuery.ToString();
            strSQL = strSQL.Remove(strSQL.Length - 4) + ")";

            ds = expdata.GetDataSet(strSQL);
            expdata.sqlexecute.Parameters.Clear();

            return ds.Tables[0];
        }
        public DataTable searchForLocations(string name, string code, string address1, string address2, string city, string county, string postcode, int country, int parentcompanyid, string showfrom, string showto, string iscompany, SortedList<int,string> userdefined)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            StringBuilder query = new StringBuilder();
            string strsql = "select companyid, company, comment, archived, companycode from companies";
            if (name != "")
            {
                query.Append("company like @name and ");
                expdata.sqlexecute.Parameters.AddWithValue("@name", name + "%");
            }
            if (code != "")
            {
                query.Append("companycode like @code and ");
                expdata.sqlexecute.Parameters.AddWithValue("@code", code + "%");
            }
            if (address1 != "")
            {
                query.Append("address1 like @address1 and ");
                expdata.sqlexecute.Parameters.AddWithValue("@address1", address1 + "%");
            }
            if (address2 != "")
            {
                query.Append("address2 like @address2 and ");
                expdata.sqlexecute.Parameters.AddWithValue("@address2", address2 + "%");
            }
            if (city != "")
            {
                query.Append("city like @city and ");
                expdata.sqlexecute.Parameters.AddWithValue("@city", city + "%");
            }
            if (county != "")
            {
                query.Append("county like @county and ");
                expdata.sqlexecute.Parameters.AddWithValue("@county", county + "%");
            }
            if (postcode != "")
            {
                query.Append("postcode like @postcode and ");
                expdata.sqlexecute.Parameters.AddWithValue("@postcode", postcode + "%");
            }
            if (country > 0)
            {
                query.Append("country = @country and ");
                expdata.sqlexecute.Parameters.AddWithValue("@country", country);
            }
            if (parentcompanyid > 0)
            {
                query.Append("parentcompanyid = @parentcompanyid and ");
                expdata.sqlexecute.Parameters.AddWithValue("@parentcompanyid", parentcompanyid);
            }
            if (showfrom != "")
            {
                query.Append("showfrom = @showfrom and ");
                if (showfrom == "Yes")
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@showfrom", 1);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@showfrom", 0);
                }
            }
            if (showto != "")
            {
                query.Append("showto = @showto and ");
                if (showto == "Yes")
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@showto", 1);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@showto", 0);
                }
            }
            if (iscompany != "")
            {
                query.Append("iscompany = @iscompany and ");
                if (iscompany == "Yes")
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@iscompany", 1);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@iscompany", 0);
                }
            }
            foreach (KeyValuePair<int,string> u in userdefined)
            {
                if (u.Value.Length > 0)
                {
                    query.Append("companyid in (select recordid from userdefined_values where userdefineid = " + u.Key + " and value like @value" + u.Key + ") and ");
                    expdata.sqlexecute.Parameters.AddWithValue("@value" + u.Key, u.Value + "%");
                }
            }
            if (query.Length > 0)
            {
                query.Remove(query.Length - 5, 5);
                strsql += " where " + query.ToString();
            }

            
            DataSet ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();

            return ds.Tables[0];
        }
		public System.Data.DataSet getGrid()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "select companyid, company, comment, archived, companycode from companies order by company";

            DataSet ds = expdata.GetDataSet(strsql);

            return ds;

			
			
		}

		public void deleteCompany (int companyid)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cCompany reqcompany = GetCompanyById(companyid);
			expdata.sqlexecute.Parameters.AddWithValue("@companyid",companyid);

            strsql = "UPDATE savedexpenses_journey_steps SET start_location = NULL WHERE start_location=@companyid;UPDATE savedexpenses_journey_steps SET end_location = NULL WHERE start_location=@companyid;";

			expdata.ExecuteSQL(strsql);

            strsql = "DELETE FROM companies WHERE companyid = @companyid";
            expdata.ExecuteSQL(strsql);

            expdata.sqlexecute.Parameters.Clear();
			cAuditLog clsaudit = new cAuditLog();
			clsaudit.deleteRecord("To Locations", reqcompany.company);
			
		}

		public void changeStatus(int companyid, bool archive)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cCompany reqcompany = GetCompanyById(companyid);
			expdata.sqlexecute.Parameters.AddWithValue("@companyid",companyid);
			
			if (archive == true)
			{
				strsql = "update companies set archived = 1 where companyid = @companyid";
			}
			else
			{
				strsql = "update companies set archived = 0 where companyid = @companyid";
			}
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			cAuditLog clsaudit = new cAuditLog();
			clsaudit.editRecord(reqcompany.company,"Archive Status", "To Locations", reqcompany.archived.ToString(), archive.ToString());
			
				
		}

        public int saveCompany(cCompany company)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@companyid", company.companyid);
            expdata.sqlexecute.Parameters.AddWithValue("@company",company.company);
            if (company.companycode == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companycode", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companycode", company.companycode);
            }
            if (company.comment == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@comment", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@comment", company.comment);
            }
            if (company.address1 == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@address1", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@address1", company.address1);
            }
            if (company.address2 == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@address2", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@address2", company.address2);
            }
            if (company.city == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@city", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@city", company.city);
            }
            if (company.county == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@county", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@county", company.county);
            }
            if (company.postcode == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@postcode", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@postcode", company.postcode);
            }
            if (company.country == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@country", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@country", company.country);
            }
            if (company.parentcompanyid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentcompanyid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@parentcompanyid", company.parentcompanyid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@iscompany",Convert.ToBoolean(company.iscompany));
            expdata.sqlexecute.Parameters.AddWithValue("@showfrom", Convert.ToBoolean(company.showfrom));
            expdata.sqlexecute.Parameters.AddWithValue("@showto", Convert.ToBoolean(company.showto));
            if (company.companyid > 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", company.modifiedon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", company.modifiedby);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", company.createdon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", company.createdby);
            }
            expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveCompany");
            int companyid = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
            company.updateID(companyid);
            expdata.sqlexecute.Parameters.Clear();

            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.Company, companyid, company.userdefined);

            return companyid;
        }
		public int addCompany(string company, string companycode, string comment, bool showfrom, bool showto, int[,] distances, int userid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            if (alreadyExists(company, false, 0))
            {
                return -1;
            }

            string strsql;
			System.Collections.SortedList values = new System.Collections.SortedList();
			values.Add("company",company);
			values.Add("companycode",companycode);
			values.Add("comment",comment);
            values.Add("createdon", DateTime.Now.ToUniversalTime());
            values.Add("createdby", userid);
			values.Add("showfrom",Convert.ToByte(showfrom));
			values.Add("showto",Convert.ToByte(showto));
			

			//companyid
			int companyid;
			strsql = "select companyid from companies where company = @company";
			expdata.sqlexecute.Parameters.AddWithValue("@company",company);
			companyid = expdata.getcount(strsql);
			expdata.sqlexecute.Parameters.Clear();

			insertDistances(companyid,distances);
			
			cAuditLog clsaudit = new cAuditLog(accountid, userid);
			clsaudit.addRecord("To Locations", company);
			
            //cCompany newcomp = new cCompany(companyid,company, companycode, false, comment, showto, showfrom, createdon, userid, new DateTime(1900,01,01), 0);
            //list.Add(companyid, newcomp);
			return companyid;
		}

		public bool alreadyExists(string company, bool update, int id)
		{
			
            int count;
			
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            if (update)
            {
                strsql = "select  count(*) from companies where company = @company and companyid <> @companyid";
                expdata.sqlexecute.Parameters.AddWithValue("@companyid", id);
            }
            else
            {
                strsql = "select count(*) from companies where company = @company";
            }
            expdata.sqlexecute.Parameters.AddWithValue("@company", company.Trim().ToLower());
            count = expdata.getcount(strsql);
            expdata.sqlexecute.Parameters.Clear();
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

		}
		
		public int updateCompany (int companyid, string company, string companycode, string comment, bool showfrom, bool showto, int[,] distances, int userid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            if (alreadyExists(company, true, companyid))
            {
                return -1;
            }

			cCompany reqcompany = GetCompanyById(companyid);
			System.Collections.SortedList values = new System.Collections.SortedList();

			values.Add("company",company);
			values.Add("companycode",companycode);
			values.Add("comment",comment);
			values.Add("showfrom",Convert.ToByte(showfrom));
			values.Add("showto",Convert.ToByte(showto));
            values.Add("modifiedon", DateTime.Now.ToUniversalTime());
            values.Add("modifiedby", userid);
			
			
			insertDistances(companyid,distances);

			cAuditLog clsaudit = new cAuditLog();
			if (reqcompany.company != company)
			{
				clsaudit.editRecord(company,"Location Name","To Locations",reqcompany.company,company);
			}
			if (reqcompany.companycode != companycode)
			{
				clsaudit.editRecord(company,"Company Code","To Locations",reqcompany.companycode,companycode);
			}
			if (reqcompany.comment != comment)
			{
				clsaudit.editRecord(company,"Comment","To Locations",reqcompany.comment,comment);
			}
			
			
			return 0;
		}

        public void deleteDistance(int distanceid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@distanceid", distanceid);
            expdata.ExecuteProc("deleteDistance");
            expdata.sqlexecute.Parameters.Clear();
        }
		private void deleteDistances(int companyid)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			strsql = "delete from location_distances where locationa = " + companyid + " or locationb = " + companyid;
			expdata.ExecuteSQL(strsql);
		}

		private void insertDistances(int companyid, int[,] distances)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int i;
			deleteDistances(companyid);

			strsql = "";
			for (i = 0; i < distances.GetLength(0); i++)
			{
				if (distances[i,1] != 0)
				{
					strsql += "insert into location_distances (locationa, locationb, distance) " +
						"values (" + companyid + "," + distances[i,0] + "," + distances[i,1] + ");";
				}
			}
			if (strsql != "")
			{
				expdata.ExecuteSQL(strsql);
			}

		}

		public cCompany GetCompanyById(int companyid)
		{
            cCompany company = (cCompany)Cache["company" + accountid + companyid];
            if (company == null)
            {
                company = getCompanyFromDB(companyid);
            }
            
            return company;
		}

        private cCompany getCompanyFromDB(int companyid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            
            System.Data.SqlClient.SqlDataReader reader;
            cCompany clscompany = null;
            Dictionary<int, object> userdefined;
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            strsql = "SELECT     companyid, company, archived, comment, companycode, showfrom, showto, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, address1, address2, city, county, postcode, country, parentcompanyid, iscompany FROM dbo.companies where companyid = @companyid";
            expdata.sqlexecute.Parameters.AddWithValue("@companyid", companyid);
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            
            string company, companycode, comment, address1, address2, city, county, postcode;
            int country, parentcompanyid;
            bool archived, iscompany;
            bool showto, showfrom;
            while (reader.Read())
            {


                companyid = reader.GetInt32(reader.GetOrdinal("companyid"));
                company = reader.GetString(reader.GetOrdinal("company"));
                if (reader.IsDBNull(reader.GetOrdinal("companycode")) == false)
                {
                    companycode = reader.GetString(reader.GetOrdinal("companycode"));
                }
                else
                {
                    companycode = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("comment")) == false)
                {
                    comment = reader.GetString(reader.GetOrdinal("comment"));
                }
                else
                {
                    comment = "";
                }
                archived = reader.GetBoolean(reader.GetOrdinal("archived"));

                showto = reader.GetBoolean(reader.GetOrdinal("showto"));
                showfrom = reader.GetBoolean(reader.GetOrdinal("showfrom"));
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
                if (reader.IsDBNull(reader.GetOrdinal("address1")) == true)
                {
                    address1 = "";
                }
                else
                {
                    address1 = reader.GetString(reader.GetOrdinal("address1"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("address2")) == true)
                {
                    address2 = "";
                }
                else
                {
                    address2 = reader.GetString(reader.GetOrdinal("address2"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("city")) == true)
                {
                    city = "";
                }
                else
                {
                    city = reader.GetString(reader.GetOrdinal("city"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("county")) == true)
                {
                    county = "";
                }
                else
                {
                    county = reader.GetString(reader.GetOrdinal("county"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("postcode")) == true)
                {
                    postcode = "";
                }
                else
                {
                    postcode = reader.GetString(reader.GetOrdinal("postcode"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("country")) == true)
                {
                    country = 0;
                }
                else
                {
                    country = reader.GetInt32(reader.GetOrdinal("country"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("parentcompanyid")) == true)
                {
                    parentcompanyid = 0;
                }
                else
                {
                    parentcompanyid = reader.GetInt32(reader.GetOrdinal("parentcompanyid"));
                }
                iscompany = reader.GetBoolean(reader.GetOrdinal("iscompany"));
                userdefined = clsuserdefined.getValues(AppliesTo.Company, companyid);
                clscompany = new cCompany(companyid, company, companycode, archived, comment, showto, showfrom, createdon, createdby, modifiedon, modifiedby, address1, address2, city, county, postcode, country, parentcompanyid, iscompany, userdefined);

            }
            reader.Close();

            expdata.sqlexecute.Parameters.Clear();

            if (clscompany != null)
            {
                Cache.Insert("company" + accountid + companyid, clscompany, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }
            return clscompany;
        }

		public cCompany getCompanyFromName(string company)
		{

            int companyid = 0;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "select companyid from companies where company = @company";
            expdata.sqlexecute.Parameters.AddWithValue("@company",company);
            SqlDataReader reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                companyid = reader.GetInt32(0);
            }
            reader.Close();

            if (companyid > 0)
            {
                return GetCompanyById(companyid);
            }
            return null;
			
		}
		

		public string[] getArray(byte locationType)
		{
			int i;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            if (locationType == 1)
            {
                strsql = "select company from companies where archived = 0 and showfrom = 1 order by company";
            }
            else
            {
                strsql = "select company from companies where archived = 0 and showto = 1 order by company";
            }
            SqlDataReader reader  = expdata.GetReader(strsql);
			System.Collections.SortedList sortedlst = new System.Collections.SortedList();
			
			

            List<string> companies = new List<string>();
            while (reader.Read())
            {
                companies.Add(reader.GetString(0));
            }
            reader.Close();
			return companies.ToArray();
		}

        public DataSet getDistanceTable(int companyid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            DataSet ds;
            expdata.sqlexecute.Parameters.AddWithValue("@companyid", companyid);
            ds = expdata.GetProcDataSet("getLocationDistances");
            expdata.sqlexecute.Parameters.Clear();

            return ds;
        }

        public void updateDistance(int locationa, int locationb, decimal distance, decimal postcodeanywheredistance)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("locationa", locationa);
            expdata.sqlexecute.Parameters.AddWithValue("locationb", locationb);
            expdata.sqlexecute.Parameters.AddWithValue("distance", distance);
            expdata.sqlexecute.Parameters.AddWithValue("@postcodeanywheredistance", postcodeanywheredistance);
            expdata.ExecuteProc("saveLocationDistance");
            expdata.sqlexecute.Parameters.Clear();
        }
		public decimal getDistance(int locationa, int locationb, int employeeid)
		{

            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
			int i;
            decimal distance = 0;
            decimal postcodeanywheredistance = 0;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cCompany fromcompany = GetCompanyById(locationa);
            cCompany tocompany = GetCompanyById(locationb);
            int count = 0;

            if (fromcompany == null || tocompany == null)
            {
                return (decimal)distance;
            }

            if (fromcompany.company.Trim().ToLower() == "home")
            {
                if (reqemp.homelocationid > 0)
                {
                    locationa = reqemp.homelocationid;
                    fromcompany = GetCompanyById(locationa);
                }
                
            }
            else if (fromcompany.company.Trim().ToLower() == "office")
            {
                if (reqemp.officelocationid > 0)
                {
                    locationa = reqemp.officelocationid;
                    fromcompany = GetCompanyById(locationa);
                }
            }

            if (tocompany.company.Trim().ToLower() == "home")
            {
                if (reqemp.homelocationid > 0)
                {
                    locationb = reqemp.homelocationid;
                    tocompany = GetCompanyById(locationb);
                }
                
            }
            else if (tocompany.company.Trim().ToLower() == "office")
            {
                if (reqemp.officelocationid > 0)
                {
                    locationb = reqemp.officelocationid;
                    tocompany = GetCompanyById(locationb);
                }
            }

            string strsql = "select distance, postcodeanywheredistance from location_distances where (locationa = @locationa and locationb = @locationb) or (locationa = @locationb and locationb = @locationa)";
            expdata.sqlexecute.Parameters.AddWithValue("@locationa",locationa);
            expdata.sqlexecute.Parameters.AddWithValue("@locationb",locationb);
            
            SqlDataReader reader = expdata.GetReader(strsql);
            
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                
                if (reader.IsDBNull(0) == false)
                {
                    distance = reader.GetDecimal(0);
                }
                if (reader.IsDBNull(1) == false)
                {
                    postcodeanywheredistance = reader.GetDecimal(1);
                }
            }

            reader.Close();
            //get from Postcode anywhere
            
            try
            {
                cMisc clsmisc = new cMisc(accountid);
                cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

                if (clsproperties.usemappoint)
                {
                    string mileagecalculation;
                    if (clsproperties.mileagecalculationtype == MileageCalculationType.Shortest)
                    {
                        mileagecalculation = "DISTANCE";
                    }
                    else
                    {
                        mileagecalculation = "TIME";
                    }
                    if (fromcompany.postcode.Length > 0 && tocompany.postcode.Length > 0 && postcodeanywheredistance == 0 && locationa != locationb)
                    {
                        postcodeanywhereuk.LookupUK pca = new expenses.postcodeanywhereuk.LookupUK();
                        if (ConfigurationManager.AppSettings["ProxyServer"] != null)
                        {

                            WebProxy proxy = new WebProxy();
                            proxy.Address = new Uri(ConfigurationManager.AppSettings["ProxyServer"]);
                            pca.Proxy = proxy;
                        }
                        DistanceResults results = pca.Distance2(fromcompany.postcode, tocompany.postcode, "DRIVETIME", mileagecalculation, "SOFTW11120", "BD99-GB22-RR15-XC56", "");

                        if (results.ErrorNumber == 0 && results.Results.GetLength(0) > 0)
                        {
                            postcodeanywheredistance = (decimal)results.Results[0].Distance;
                            if (distance == 0)
                            {
                                distance = postcodeanywheredistance;
                            }
                            updateDistance(locationa, locationb, distance, postcodeanywheredistance);
                        }

                        cAuditLog clsAudit = new cAuditLog(accountid, employeeid);

                        clsAudit.addRecord("Postcode look up checked", fromcompany.company + " to " + tocompany.company + " returned " + postcodeanywheredistance.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return (decimal)distance;
            }
            return (decimal)distance;
			
		}

        public decimal getPostcodeAnywhereDistance(int locationa, int locationb, int employeeid)
        {
            decimal postcodeanywheredistance = 0;
            cCompany fromcompany = GetCompanyById(locationa);
            cCompany tocompany = GetCompanyById(locationb);
            if (fromcompany.postcode.Length > 0 && tocompany.postcode.Length > 0 && locationa != locationb)
            {
                try
                {

                        postcodeanywhereuk.LookupUK pca = new expenses.postcodeanywhereuk.LookupUK();
                        if (ConfigurationManager.AppSettings["ProxyServer"] != null)
                        {

                            WebProxy proxy = new WebProxy();
                            proxy.Address = new Uri(ConfigurationManager.AppSettings["ProxyServer"]);
                            pca.Proxy = proxy;
                        }
                        DistanceResults results = pca.Distance2(fromcompany.postcode, tocompany.postcode, "DRIVETIME", "DISTANCE", "SOFTW11120", "BD99-GB22-RR15-XC56", "");

                        if (results.ErrorNumber == 0 && results.Results.GetLength(0) > 0)
                        {
                            postcodeanywheredistance = (decimal)results.Results[0].Distance;  
                        }

                        cAuditLog clsAudit = new cAuditLog(accountid, employeeid);

                        clsAudit.addRecord("Postcode look up checked", fromcompany.company + " to " + tocompany.company + " returned " + postcodeanywheredistance.ToString());
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            return postcodeanywheredistance;
        }
		

		

		public System.Data.DataSet searchLocations(bool from, string location, string locationcode)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.DataSet ds;
			strsql = "select companyid, company, companycode from companies where archived = 0 and ";
			if (from == true)
			{
				strsql += "showfrom = 1";
			}
			else
			{
				strsql += "showto = 1";
			}
			
			if (location != "")
			{
				location = "%" + location + "%";
				strsql += " and company like @location";
				expdata.sqlexecute.Parameters.AddWithValue("@location",location);
			}
			if (locationcode != "")
			{
				locationcode = "%" + locationcode + "%";
				strsql += " and companycode like @locationcode";
				expdata.sqlexecute.Parameters.AddWithValue("@locationcode",locationcode);
			}
			strsql += " order by company";
			
			ds = expdata.GetDataSet(strsql);
			expdata.sqlexecute.Parameters.Clear();
			return ds;
		}

        public int fromCount
        {
            get
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql;
                int count;
                if (Cache["fromcount" + accountid] == null)
                {
                    strsql = "select count(*) from companies where showfrom = 1";
                    
                    count = expdata.getcount(strsql);
                    expdata.sqlexecute.Parameters.Clear();

                    Cache.Add("fromcount" + accountid, count, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);

                }
                else
                {
                    count = (int)Cache["fromcount" + accountid];
                }
                return count;
            }

        }

        public int toCount
        {
            get
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                int count;
                if (Cache["tocount" + accountid] == null)
                {
                    string strsql;
                    strsql = "select count(*) from companies where showto = 1";
                    
                    count = expdata.getcount(strsql);
                    expdata.sqlexecute.Parameters.Clear();

                    Cache.Add("tocount" + accountid, count, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);

                }
                else
                {
                    count = (int)Cache["tocount" + accountid];
                }
                return count;
            }

        }

        public int companyCount
        {
            get
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                int count;
                if (Cache["companycount" + accountid] == null)
                {
                    string strsql;
                    strsql = "select count(*) from companies where iscompany = 1";

                    count = expdata.getcount(strsql);
                    expdata.sqlexecute.Parameters.Clear();

                    Cache.Add("companycount" + accountid, count, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);

                }
                else
                {
                    count = (int)Cache["companycount" + accountid];
                }
                return count;
            }

        }

        public List<System.Web.UI.WebControls.ListItem> CreateMergeDropDown(string defaultSelectOption)
        {

            cCompany company;
            SortedList<string, int> sorted = getCompaniesForDropdown();
            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();
            
            if (defaultSelectOption != null)
            {
                items.Add(new System.Web.UI.WebControls.ListItem(defaultSelectOption, "0"));
            }

            foreach (KeyValuePair<string,int> i in sorted)
            {
                
                items.Add(new System.Web.UI.WebControls.ListItem(i.Key, i.Value.ToString()));
            }

            return items;
        }

        private SortedList<string, int> getCompaniesForDropdown()
        {
            SortedList<string, int> list = new SortedList<string, int>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "select companyid, company from companies order by company";
            SqlDataReader reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                list.Add(reader.GetString(1), reader.GetInt32(0));
            }
            reader.Close();
            return list;
        }
        public List<System.Web.UI.WebControls.ListItem> CreateDropDown(CompanyType type)
        {
            cCompany company;
            SortedList<string, int> list = new SortedList<string, int>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "select companyid, company from companies where archived = 0 ";
            switch (type)
            {
                case CompanyType.Company:
                    strsql += "and iscompany = 1";
                    break;
                case CompanyType.From:
                    strsql += "and showfrom = 1";
                    break;
                case CompanyType.To:
                    strsql += "and showto = 1";
                    break;
            }
            strsql += " order by company";
            SqlDataReader reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                list.Add(reader.GetString(1), reader.GetInt32(0));
            }
            reader.Close();


            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();

            items.Add(new System.Web.UI.WebControls.ListItem("","0"));
            foreach (KeyValuePair<string,int> i in list)
            {
                

                
                    items.Add(new System.Web.UI.WebControls.ListItem(i.Key, i.Value.ToString()));
                

            }

            return items;
            
            
        }

       

        public Dictionary<int, cCompany> getModifiedCompanies(DateTime date)
        {
            cCompany company;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "select companyid from companies where createdon > @date or modifiedon > @date";
            expdata.sqlexecute.Parameters.AddWithValue("@date", date);
            SqlDataReader reader = expdata.GetReader(strsql);
            


            Dictionary<int, cCompany> lst = new Dictionary<int, cCompany>();
            while (reader.Read())
            {
                company = GetCompanyById(reader.GetInt32(0));
                lst.Add(company.companyid, company);
            }
            reader.Close();
            
            return lst;
        }

        public List<int> getCompIds()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql = "select companyid from companies";

            SqlDataReader reader = expdata.GetReader(strsql);

            List<int> ids = new List<int>();
            while(reader.Read())
            {
                ids.Add(reader.GetInt32(0));
            }
            reader.Close();
            return ids;
        }

        public Dictionary<int[], cCompanyDistance> getModifiedCompDistances(DateTime date)
        {
            Dictionary<int[], cCompanyDistance> lst = new Dictionary<int[], cCompanyDistance>();
            cCompanyDistance[] companyDistances = CacheCompanyDistances();
            foreach (cCompanyDistance comp in companyDistances)
            {
                if (comp.createdon > date)
                {
                    lst.Add(new int[] {comp.locationa, comp.locationb}, comp);
                }
            }
            return lst;
        }

        public bool mergeCompany(int intMergeToCompanyID, ListItemCollection lstMergeFromCompanies)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            
            StringBuilder sbMergeFromIDs = new StringBuilder();
            StringBuilder sbUpdateFromCompanies = new StringBuilder();
            StringBuilder sbUpdateToCompanies = new StringBuilder();

            List<int> lstCompaniesToMerge = new List<int>();
            cCompany cMergeToCompany = GetCompanyById(intMergeToCompanyID);

            foreach (ListItem item in lstMergeFromCompanies)
            {
                if (item.Selected)
                {
                    lstCompaniesToMerge.Add(int.Parse(item.Value));
                    sbMergeFromIDs.Append(item.Value + ",");
                }
            }

            sbMergeFromIDs.Remove(sbMergeFromIDs.Length - 1, 1);

            string strsql = "SELECT expenseid,start_location,end_location FROM savedexpenses_journey_steps WHERE start_location IN (" + sbMergeFromIDs.ToString() + ") OR end_location IN (" + sbMergeFromIDs.ToString() + ");";
            System.Data.SqlClient.SqlDataReader reader;
            reader = expdata.GetReader(strsql);

            if (reader.HasRows)
            {

                List<int> lstFromMatches = new List<int>();
                List<int> lstToMatches = new List<int>();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("start_location")))
                    {
                        if (lstCompaniesToMerge.Contains(reader.GetInt32(reader.GetOrdinal("start_location"))))
                        {
                            if (!lstFromMatches.Contains(reader.GetInt32(reader.GetOrdinal("start_location"))))
                            {
                                sbUpdateFromCompanies.Append(reader.GetInt32(reader.GetOrdinal("start_location")) + ",");
                                lstFromMatches.Add(reader.GetInt32(reader.GetOrdinal("start_location")));
                            }
                            
                        }
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("end_location")))
                    {
                        if (lstCompaniesToMerge.Contains(reader.GetInt32(reader.GetOrdinal("end_location"))))
                        {
                            if (!lstToMatches.Contains(reader.GetInt32(reader.GetOrdinal("end_location"))))
                            {
                                sbUpdateToCompanies.Append(reader.GetInt32(reader.GetOrdinal("end_location")) + ",");
                                lstToMatches.Add(reader.GetInt32(reader.GetOrdinal("end_location")));
                            }
                        }
                    }
                }
            }

            reader.Close();
            //sbMergeFromIDs
            strsql = "";

            if (sbUpdateFromCompanies.ToString().Length > 0)
            {
                sbUpdateFromCompanies.Remove(sbUpdateFromCompanies.Length - 1, 1);

                strsql += "UPDATE savedexpenses_journey_steps SET start_location=@newCompanyID WHERE start_location IN (" + sbUpdateFromCompanies.ToString() + ");";
            }

            if (sbUpdateToCompanies.ToString().Length > 0)
            {
                sbUpdateToCompanies.Remove(sbUpdateToCompanies.Length - 1, 1);
                strsql += "UPDATE savedexpenses_journey_steps SET end_location=@newCompanyID WHERE end_location IN (" + sbUpdateToCompanies.ToString() + ");";
            }

            expdata.sqlexecute.Parameters.AddWithValue("@newCompanyID", intMergeToCompanyID);

            if (strsql != "") // Can be NULL if the location is not used in savedexpenses_journey_steps
            {
                expdata.ExecuteSQL(strsql);
            }

            expdata.sqlexecute.Parameters.Clear();

            // Deal with savedexpenses_current and _previous companyid
            
            strsql = "SELECT savedexpenses.expenseid, claims_base.paid FROM savedexpenses INNER JOIN claims_base ON claims_base.claimid=savedexpenses.claimid AND savedexpenses.companyid IN ("+sbMergeFromIDs.ToString()+") ";

            reader = expdata.GetReader(strsql);
            int expenseID;
            bool paid;
            StringBuilder sbSavedExpenses = new StringBuilder();
            SortedList<int, bool> lstExpenseIDs = new SortedList<int, bool>();
            while (reader.Read())
            {
                expenseID = reader.GetInt32(reader.GetOrdinal("expenseid"));
                paid = reader.GetBoolean(reader.GetOrdinal("paid"));
                lstExpenseIDs.Add(expenseID, paid);
            }
            reader.Close();

            StringBuilder sbCurrentUpdates = new StringBuilder();
            StringBuilder sbPreviousUpdates = new StringBuilder();

            foreach (KeyValuePair<int, bool> expenseDetails in lstExpenseIDs)
            {
                if (expenseDetails.Value == true)
                {
                    sbPreviousUpdates.Append(expenseDetails.Key + ",");
                }
                else
                {
                    sbCurrentUpdates.Append(expenseDetails.Key + ",");
                }
            }

            expdata.sqlexecute.Parameters.AddWithValue("@newCompanyID", intMergeToCompanyID);


            strsql = "";

            if (sbPreviousUpdates.Length > 0)
            {
                sbPreviousUpdates.Remove(sbPreviousUpdates.Length - 1, 1);
                strsql += "UPDATE savedexpenses_previous SET companyid=@newCompanyID WHERE expenseid IN (" + sbPreviousUpdates.ToString() + "); ";
            }


            if (sbCurrentUpdates.Length > 0)
            {
                sbCurrentUpdates.Remove(sbCurrentUpdates.Length - 1, 1);
                strsql += "UPDATE savedexpenses_current SET companyid=@newCompanyID WHERE expenseid IN (" + sbCurrentUpdates.ToString() + "); ";
            }

            if (sbCurrentUpdates.Length > 0 || sbPreviousUpdates.Length > 0)
            {
                expdata.ExecuteSQL(strsql);
            }

            expdata.sqlexecute.Parameters.Clear();

            foreach (int item in lstCompaniesToMerge)
            {
                deleteDistances(item);
                deleteCompany(item);
            }

            return true;
        }

        public DataTable getTop10ToLocationsByEmployeeid(int employeeid)
        {
            DataTable tbllocations = (DataTable)Cache["top10tolocations" + employeeid];
            if (tbllocations == null)
            {
                tbllocations = new DataTable();
                tbllocations.Columns.Add("companyid", typeof(Int32));
                tbllocations.Columns.Add("company", typeof(System.String));
                System.Data.SqlClient.SqlDataReader reader;
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql = "getTop10ToLocationsByEmployeeid";
                expdata.command.Parameters.AddWithValue("@employeeid", employeeid);
                reader = expdata.getReader(strsql);
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    tbllocations.Rows.Add(new object[] {reader.GetInt32(0), reader.GetString(1)});
                }
                reader.Close();
                
                Cache.Insert("top10tolocations" + employeeid, tbllocations, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1), CacheItemPriority.NotRemovable, null);
            }

            return tbllocations;
        }

        public DataTable getTop10CompanyLocationsByEmployeeid(int employeeid)
        {
            DataTable tbllocations = (DataTable)Cache["top10companylocations" + employeeid];
            if (tbllocations == null)
            {
                tbllocations = new DataTable();
                tbllocations.Columns.Add("companyid", typeof(Int32));
                tbllocations.Columns.Add("company", typeof(System.String));
                System.Data.SqlClient.SqlDataReader reader;
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql = "getTop10CompanyLocationsByEmployeeid";
                expdata.command.Parameters.AddWithValue("@employeeid", employeeid);
                reader = expdata.getReader(strsql);
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    tbllocations.Rows.Add(new object[] { reader.GetInt32(0), reader.GetString(1) });
                }
                reader.Close();

                Cache.Insert("top10companylocations" + employeeid, tbllocations, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1), CacheItemPriority.NotRemovable, null);
            }

            return tbllocations;
        }

        public DataTable getLast10ToLocationsByEmployeeid(int employeeid)
        {
            DataTable tbllocations = (DataTable)Cache["last10tolocations" + employeeid];
            if (tbllocations == null)
            {
                tbllocations = new DataTable();
                tbllocations.Columns.Add("companyid", typeof(Int32));
                tbllocations.Columns.Add("company", typeof(System.String));
                System.Data.SqlClient.SqlDataReader reader;
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql = "getLast10ToLocationsByEmployeeid";
                expdata.command.Parameters.AddWithValue("@employeeid", employeeid);
                reader = expdata.getReader(strsql);
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    tbllocations.Rows.Add(new object[] { reader.GetInt32(0), reader.GetString(1) });
                }
                reader.Close();
                
                Cache.Insert("last10tolocations" + employeeid, tbllocations, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1), CacheItemPriority.NotRemovable, null);
            }

            return tbllocations;
        }

        public DataTable getLast10CompanyLocationsByEmployeeid(int employeeid)
        {
            DataTable tbllocations = (DataTable)Cache["last10companylocations" + employeeid];
            if (tbllocations == null)
            {
                tbllocations = new DataTable();
                tbllocations.Columns.Add("companyid", typeof(Int32));
                tbllocations.Columns.Add("company", typeof(System.String));
                System.Data.SqlClient.SqlDataReader reader;
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql = "getLast10CompanyLocationsByEmployeeid";
                expdata.command.Parameters.AddWithValue("@employeeid", employeeid);
                reader = expdata.getReader(strsql);
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    tbllocations.Rows.Add(new object[] { reader.GetInt32(0), reader.GetString(1) });
                }
                reader.Close();

                Cache.Insert("last10companylocations" + employeeid, tbllocations, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1), CacheItemPriority.NotRemovable, null);
            }

            return tbllocations;
        }

        public DataTable getTop10FromLocationsByEmployeeid(int employeeid)
        {
            DataTable tbllocations = (DataTable)Cache["top10fromlocations" + employeeid];
            if (tbllocations == null)
            {
                tbllocations = new DataTable();
                tbllocations.Columns.Add("companyid", typeof(Int32));
                tbllocations.Columns.Add("company", typeof(System.String));
                System.Data.SqlClient.SqlDataReader reader;
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql = "getTop10FromLocationsByEmployeeid";
                expdata.command.Parameters.AddWithValue("@employeeid", employeeid);
                reader = expdata.getReader(strsql);
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    tbllocations.Rows.Add(new object[] { reader.GetInt32(0), reader.GetString(1) });
                }
                reader.Close();
                
                Cache.Insert("top10fromlocations" + employeeid, tbllocations, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1), CacheItemPriority.NotRemovable, null);
            }

            return tbllocations;
        }

        public DataTable getLast10FromLocationsByEmployeeid(int employeeid)
        {
            DataTable tbllocations = (DataTable)Cache["last10fromlocations" + employeeid];
            if (tbllocations == null)
            {
                tbllocations = new DataTable();
                tbllocations.Columns.Add("companyid", typeof(Int32));
                tbllocations.Columns.Add("company", typeof(System.String));
                System.Data.SqlClient.SqlDataReader reader;
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql = "getLast10FromLocationsByEmployeeid";
                expdata.command.Parameters.AddWithValue("@employeeid", employeeid);
                reader = expdata.getReader(strsql);
                expdata.sqlexecute.Parameters.Clear();
                while (reader.Read())
                {
                    tbllocations.Rows.Add(new object[] { reader.GetInt32(0), reader.GetString(1) });
                }
                reader.Close();
                
                Cache.Insert("last10fromlocations" + employeeid, tbllocations, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1), CacheItemPriority.NotRemovable, null);
            }

            return tbllocations;
        }

        public int count
        {
            get {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql = "select count(*) from companies";
                int count = expdata.getcount(strsql);
                return count;
            }
        }

        public string[] getAddress(string country, string postcode)
        {
            expenses.postcodeanywhere.LookupInternational pca = new expenses.postcodeanywhere.LookupInternational();
            if (ConfigurationManager.AppSettings["ProxyServer"] != null)
            {

                WebProxy proxy = new WebProxy();
                proxy.Address = new Uri(ConfigurationManager.AppSettings["ProxyServer"]);
                pca.Proxy = proxy;
            }
            expenses.postcodeanywhere.StreetResults results = pca.FetchStreets("", postcode, country, "", "SOFTW11120", "BD99-GB22-RR15-XC56", "");

            string[] address = new string[5];
            if (results.ErrorNumber == 0 && results.Results.GetLength(0) > 0)
            {
                address[0] = results.Results[0].Street;
                address[1] = results.Results[0].District;
                address[2] = results.Results[0].City;
                address[3] = results.Results[0].State;
                address[4] = results.Results[0].Postcode;
                return address;
            }

            return new string[0];
        }
    }


    public enum CompanyType
    {
        Company,
        From,
        To,
        None
    }
}
