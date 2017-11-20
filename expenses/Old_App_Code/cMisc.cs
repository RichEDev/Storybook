using System;
using ExpensesLibrary;
using System.Collections;
using expenses.Old_App_Code;
using System.Web.Caching;
using System.Data;
using System.Collections.Generic;
using SpendManagementLibrary;
using System.Configuration;
namespace expenses
{
	/// <summary>
	/// Summary description for cMisc.
	/// </summary>
	public class cMisc
	{
		
		
		int accountid = 0;

        

		
		string strsql;
		public cMisc(int intaccountid)
		{
			accountid = intaccountid;
            
			
			
		}

		public static CurrentUser getCurrentUser(string identity)
        {
            string[] temp = identity.Split(',');
            CurrentUser user = new CurrentUser();
            user.accountid = int.Parse(temp[0]);
            user.employeeid = int.Parse(temp[1]);
            return user;
        }

        public static string path
        {
            get
            {
                if (System.Web.HttpRuntime.AppDomainAppVirtualPath == "/")
                {
                    return "";
                }
                else
                {
                    return System.Web.HttpRuntime.AppDomainAppVirtualPath;
                }
            }
        }

        public cFieldToDisplay getGeneralFieldByCode(string code)
        {
            SortedList<string, cFieldToDisplay> lstFieldsToDisplay = (SortedList<string,cFieldToDisplay>)System.Web.HttpRuntime.Cache["fieldsToDisplay" + accountid];
            if (lstFieldsToDisplay == null)
            {
                lstFieldsToDisplay = getGeneralFieldsToDisplay();
            }

            return (cFieldToDisplay)lstFieldsToDisplay[code];

            
        }

		

        private SortedList<string,cFieldToDisplay> getGeneralFieldsToDisplay()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cFieldToDisplay field;
            SortedList<string, cFieldToDisplay> list = new SortedList<string, cFieldToDisplay>();
            Guid fieldid;
            bool display, mandatory, individual, displaycc, displaypc, mandatorycc, mandatorypc;
            string code, description;
            System.Data.SqlClient.SqlDataReader reader;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            strsql = "select addscreen.fieldid, display, mandatory, code, description, individual, displaycc, displaypc, mandatorycc, mandatorypc, createdon, createdby, modifiedon, modifiedby from dbo.addscreen";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
            reader = expdata.GetReader(strsql);


            

            cFields clsfields = new cFields(accountid);
            cField reqfield;
           while (reader.Read())
            {
                fieldid = reader.GetGuid(reader.GetOrdinal("fieldid"));
                display = reader.GetBoolean(reader.GetOrdinal("display"));
                mandatory = reader.GetBoolean(reader.GetOrdinal("mandatory"));
                code = reader.GetString(reader.GetOrdinal("code"));
                if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
                {
                    description = reader.GetString(reader.GetOrdinal("description"));
                }
                else
                {
                    reqfield = clsfields.getFieldById(fieldid);
                    description = reqfield.description;
                }
                individual = reader.GetBoolean(reader.GetOrdinal("individual"));
                displaycc = reader.GetBoolean(reader.GetOrdinal("displaycc"));
                displaypc = reader.GetBoolean(reader.GetOrdinal("displaypc"));
                mandatorycc = reader.GetBoolean(reader.GetOrdinal("mandatorycc"));
                mandatorypc = reader.GetBoolean(reader.GetOrdinal("mandatorypc"));
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
                field = new cFieldToDisplay(fieldid, code, description, display, mandatory, individual, displaycc, mandatorycc, displaypc, mandatorypc, createdon, createdby, modifiedon, modifiedby);
                list.Add(code, field);
            }
           reader.Close();
            expdata.sqlexecute.Parameters.Clear();

            System.Web.HttpRuntime.Cache.Add("fieldsToDisplay" + accountid, list, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);
            return list;            
        }

		


        public sOnlineAddscreenItemsInfo getModifiedAddscreenItems(DateTime date)
        {
            sOnlineAddscreenItemsInfo addscreeninfo = new sOnlineAddscreenItemsInfo();
            SortedList<string,cFieldToDisplay> fields = getGeneralFieldsToDisplay();
            Dictionary<Guid, cFieldToDisplay> lstAddScreenFields = new Dictionary<Guid, cFieldToDisplay>();
            List<Guid> lstaddscreenids = new List<Guid>();
            

            foreach (cFieldToDisplay field in fields.Values)
            {
                if (field.modifiedon > date)
                {
                    modified = true;
                }

                if (modified)
                {
                    lstAddScreenFields.Add(field.fieldid, field);
                }

                lstaddscreenids.Add(field.fieldid);
            }
            addscreeninfo.lstonlineaddscreenitems = lstAddScreenFields;
            addscreeninfo.lstaddscreenids = lstaddscreenids;

            return addscreeninfo;
        }

        

		public void updatePolicy(string policy, byte policytype)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;	
			if (policytype == 1)
			{
				
				expdata.sqlexecute.Parameters.AddWithValue("@policy",policy);
				strsql = "update [other] set standards = @policy";
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
                //InvalidateGlobalProperties(accountid);
			}
			else
			{
				uploadFile(policy,1);
			}
		}
		private void uploadFile(string filelocation, int uploadtype)
		{
			byte[] responseArray;
			string uploadpath = "";
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;	
			System.Net.WebClient upload = new System.Net.WebClient();
			switch (uploadtype)
			{
				case 1: //policy upload
					uploadpath = appinfo.Server.MapPath("../policies/" + accountid + ".htm");
					break;
			}

			responseArray = upload.UploadFile(uploadpath,filelocation);



		}


		public void changePolicyType(byte policytype)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			
			expdata.sqlexecute.Parameters.AddWithValue("@policytype",policytype);
			strsql = "update [other] set policytype = @policytype";
			expdata.ExecuteSQL(strsql);
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            //InvalidateGlobalProperties(accountid);
			expdata.sqlexecute.Parameters.Clear();
		}

		public void updateCompanyDetails (string companyname, string address1, string address2, string city, string county, string postcode, string telno, string faxno, string email, string bankref, string accountno, string accounttype, string sortcode,string financialyearstart, string financialyearend, string companynumber)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int count = 0;
            
            expdata.sqlexecute.Parameters.AddWithValue("@companyname", companyname);
            expdata.sqlexecute.Parameters.AddWithValue("@address1", address1);
            expdata.sqlexecute.Parameters.AddWithValue("@address2", address2);
            expdata.sqlexecute.Parameters.AddWithValue("@city", city);
            expdata.sqlexecute.Parameters.AddWithValue("@county", county);
            expdata.sqlexecute.Parameters.AddWithValue("@postcode", postcode);
            expdata.sqlexecute.Parameters.AddWithValue("@telno", telno);
            expdata.sqlexecute.Parameters.AddWithValue("@faxno", faxno);
            expdata.sqlexecute.Parameters.AddWithValue("@email", email);
            expdata.sqlexecute.Parameters.AddWithValue("@bankref", bankref);
            expdata.sqlexecute.Parameters.AddWithValue("@accountno", accountno);
            expdata.sqlexecute.Parameters.AddWithValue("@accounttype", accounttype);
            expdata.sqlexecute.Parameters.AddWithValue("@sortcode", sortcode);
            expdata.sqlexecute.Parameters.AddWithValue("@companynumber", companynumber);

			strsql = "select count(*) from companydetails";
			count = expdata.getcount(strsql);
			if (count == 0)
			{
				strsql = "insert into companydetails (companyname, address1, address2, city, county, postcode, telno, faxno, email, financialyearstart, financialyearend, companynumber) " +
					"values (@companyname,@address1,@address2,@city,@county,@postcode,@telno,@faxno,@email,@financialyearstart, @financialyearend, @companynumber)";
			}
			else
			{
				strsql = "update companydetails set companyname = @companyname, address1 = @address1, address2 = @address2, city = @city, county = @county, postcode = @postcode, telno = @telno, faxno = @faxno, email = @email, financialyearstart = @financialyearstart, financialyearend = @financialyearend, companynumber = @companynumber";
			}
            expdata.sqlexecute.Parameters.AddWithValue("@financialyearstart", financialyearstart);
            expdata.sqlexecute.Parameters.AddWithValue("@financialyearend", financialyearend);
			expdata.ExecuteSQL(strsql);
			count = 0;
			strsql = "select count(*) from [company_bankdetails]";
			count = expdata.getcount(strsql);
			if (count == 0)
			{
				strsql = "insert into [company_bankdetails] (bankreference, accountnumber, accounttype, sortcode) " +
					"values (@bankref,@accountno,@accounttype,@sortcode)";
			}
			else
			{
				strsql = "update [company_bankdetails] set bankreference = @bankref, accountnumber = @accountno, accounttype = @accounttype, sortcode = @sortcode";
			}
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public System.Data.DataTable getFieldsGrid()
		{
            cFields clsfields = new cFields(accountid);
            bool display, mandatory, displaycc, mandatorycc, displaypc, mandatorypc, individual;
            Guid fieldid;
            string adddesc;
            cField field;
            DataTable tbl = new DataTable();
            tbl.Columns.Add("description", typeof(System.String));
            tbl.Columns.Add("fieldid", typeof(System.Int32));
            tbl.Columns.Add("adddesc", typeof(System.String));
            tbl.Columns.Add("display", typeof(System.Boolean));
            tbl.Columns.Add("mandatory", typeof(System.Boolean));
            tbl.Columns.Add("displaycc",typeof(System.Boolean));
            tbl.Columns.Add("mandatorycc",typeof(System.Boolean));
            tbl.Columns.Add("displaypc",typeof(System.Boolean));
            tbl.Columns.Add("mandatorypc",typeof(System.Boolean));
            tbl.Columns.Add("individual", typeof(System.Boolean));


            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.DataSet temp = new System.Data.DataSet();
			
			strsql = "select addscreen.fieldid, addscreen.description as adddesc, addscreen.display, addscreen.mandatory, addscreen.displaycc, addscreen.mandatorycc, addscreen.displaypc, addscreen.mandatorypc, addscreen.individual from addscreen";
            System.Data.SqlClient.SqlDataReader reader;
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                fieldid = reader.GetGuid(reader.GetOrdinal("fieldid"));
                field = clsfields.getFieldById(fieldid);
                display = reader.GetBoolean(reader.GetOrdinal("display"));
                mandatory = reader.GetBoolean(reader.GetOrdinal("mandatory"));
                displaycc = reader.GetBoolean(reader.GetOrdinal("displaycc"));
                mandatorycc = reader.GetBoolean(reader.GetOrdinal("mandatorycc"));
                displaypc = reader.GetBoolean(reader.GetOrdinal("displaypc"));
                mandatorypc = reader.GetBoolean(reader.GetOrdinal("mandatorypc"));
                individual = reader.GetBoolean(reader.GetOrdinal("individual"));
                if (reader.IsDBNull(reader.GetOrdinal("adddesc")) == true)
                {
                    adddesc = "";
                }
                else
                {
                    adddesc = reader.GetString(reader.GetOrdinal("adddesc"));
                }
                tbl.Rows.Add(new object[] { field.description, fieldid, adddesc, display, mandatory, displaycc, mandatorycc, displaypc, mandatorypc, individual });
            }
            reader.Close();
			expdata.sqlexecute.Parameters.Clear();
			return tbl;
		}

		public void updateFieldsToDisplay(List<cFieldToDisplay> fields)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			int i = 0;
			
            foreach (cFieldToDisplay field in fields)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@display" + i,Convert.ToByte(field.display));
				expdata.sqlexecute.Parameters.AddWithValue("@mandatory" + i, Convert.ToByte(field.mandatory));
                expdata.sqlexecute.Parameters.AddWithValue("@individual" + i, Convert.ToByte(field.individual));
				expdata.sqlexecute.Parameters.AddWithValue("@fieldid" + i,field.fieldid);
				if (fields[i].description == "")
				{
					expdata.sqlexecute.Parameters.AddWithValue("@description" + i,DBNull.Value);
				}
				else
				{
					expdata.sqlexecute.Parameters.AddWithValue("@description" + i,field.description);
				}
                expdata.sqlexecute.Parameters.AddWithValue("@displaycc" + i, Convert.ToByte(fields[i].displaycc));
                expdata.sqlexecute.Parameters.AddWithValue("@displaypc" + i, Convert.ToByte(fields[i].displaypc));
                expdata.sqlexecute.Parameters.AddWithValue("@mandatorycc" + i, Convert.ToByte(fields[i].mandatorycc));
                expdata.sqlexecute.Parameters.AddWithValue("@mandatorypc" + i, Convert.ToByte(fields[i].mandatorypc));
                expdata.sqlexecute.Parameters.AddWithValue("@modifiedon" + i, DateTime.Now.ToUniversalTime());
                strsql = "update addscreen set description = @description" + i + ", display = @display" + i + ", mandatory = @mandatory" + i + ", individual = @individual" + i + ", displaycc = @displaycc" + i + ", displaypc = @displaypc" + i + ", mandatorycc = @mandatorycc" + i + ", mandatorypc = @mandatorypc" + i + ", modifiedon = @modifiedon" + i + " where fieldid = @fieldid" + i;
				expdata.ExecuteSQL(strsql);
                i++;
			}
			expdata.sqlexecute.Parameters.Clear();
			
			
		}

		

        


		public sCompanyDetails getCompanyDetails()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			sCompanyDetails temp = new sCompanyDetails();

			System.Data.SqlClient.SqlDataReader compreader;
			
			strsql = "select * from companydetails";
			compreader = expdata.GetReader(strsql);
			while (compreader.Read())
			{
				if (compreader.IsDBNull(compreader.GetOrdinal("companyname")) == false)
				{
					temp.companyname = compreader.GetString(compreader.GetOrdinal("companyname"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("address1")) == false)
				{
					temp.address1 = compreader.GetString(compreader.GetOrdinal("address1"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("address2")) == false)
				{
					temp.address2 = compreader.GetString(compreader.GetOrdinal("address2"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("city")) == false)
				{
					temp.city = compreader.GetString(compreader.GetOrdinal("city"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("county")) == false)
				{
					temp.county = compreader.GetString(compreader.GetOrdinal("county"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("postcode")) == false)
				{
					temp.postcode = compreader.GetString(compreader.GetOrdinal("postcode"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("telno")) == false)
				{
					temp.telno = compreader.GetString(compreader.GetOrdinal("telno"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("faxno")) == false)
				{
					temp.faxno = compreader.GetString(compreader.GetOrdinal("faxno"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("email")) == false)
				{
					temp.email = compreader.GetString(compreader.GetOrdinal("email"));
				}
                if (compreader.IsDBNull(compreader.GetOrdinal("financialyearstart")) == false)
                {
                    temp.financialyearstart = compreader.GetString(compreader.GetOrdinal("financialyearstart"));
                }
                if (compreader.IsDBNull(compreader.GetOrdinal("financialyearend")) == false)
                {
                    temp.financialyearend = compreader.GetString(compreader.GetOrdinal("financialyearend"));
                }
                if (compreader.IsDBNull(compreader.GetOrdinal("companynumber")) == false)
                {
                    temp.companynumber = compreader.GetString(compreader.GetOrdinal("companynumber"));
                }
			}
			compreader.Close();
			strsql = "select * from [company_bankdetails]";
			compreader = expdata.GetReader(strsql);
			while (compreader.Read())
			{
				if (compreader.IsDBNull(compreader.GetOrdinal("bankreference")) == false)
				{
					temp.bankref = compreader.GetString(compreader.GetOrdinal("bankreference"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("accountnumber")) == false)
				{
					temp.accoutno = compreader.GetString(compreader.GetOrdinal("accountnumber"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("accounttype")) == false)
				{
					temp.accounttype = compreader.GetString(compreader.GetOrdinal("accounttype"));
				}
				if (compreader.IsDBNull(compreader.GetOrdinal("sortcode")) == false)
				{
					temp.sortcode = compreader.GetString(compreader.GetOrdinal("sortcode"));
				}
			}
			compreader.Close();
			expdata.sqlexecute.Parameters.Clear();
			return temp;
			
		}


		public void updatePasswordOptions(int attempts, int expiry, int plength, int length1, int length2, int previous, bool pupper, bool pnumbers)
		{
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			strsql = "update [other] set attempts = @attempts, expiry = @expiry, plength = @plength, length1 = @length1, length2 = @length2, previous = @previous, pupper = @pupper, pnumbers = @pnumbers, modifiedon = @modifiedon, modifiedby = @modifiedby";
			expdata.sqlexecute.Parameters.AddWithValue("@attempts",attempts);
			expdata.sqlexecute.Parameters.AddWithValue("@expiry",expiry);
			expdata.sqlexecute.Parameters.AddWithValue("@plength",plength);
			expdata.sqlexecute.Parameters.AddWithValue("@length1",length1);
			expdata.sqlexecute.Parameters.AddWithValue("@length2",length2);
			expdata.sqlexecute.Parameters.AddWithValue("@previous",previous);
			expdata.sqlexecute.Parameters.AddWithValue("@pupper",Convert.ToByte(pupper));
			expdata.sqlexecute.Parameters.AddWithValue("@pnumbers",Convert.ToByte(pnumbers));
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", user.employeeid);
			
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
            //InvalidateGlobalProperties(accountid);
			//getSystemOptions();
		}

        public void updateAddExpenses(bool costcodes, bool departments, bool singleclaim, bool usecostcodedesc, bool usedepartmentdesc, bool attachreceipts, bool exchangereadonly, bool useprojectcodes, bool useprojectcodedesc, bool addlocations, bool costcodeson, bool departmentson, bool projectcodeson, bool autoassignallocation, bool blocklicenceexpiry, bool blocktaxexpiry, bool blockmotexpiry, bool blockinsuranceexpiry, bool addcompanies, bool allowmultipledestinations, bool usemappoint, bool usecostcodeongendet, bool usedepartmentongendet, bool useprojectcodeongendet, bool hometooffice, bool calchometolocation, bool allowMilageForUsersAdd, bool activateCarsOnAdd, bool autocalchometolocation, bool enableAutolog,  bool allowUsersToAddCars, MileageCalculationType mileagecalculationtype)
		{
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "update [other] set usecostcodes = @usecostcodes, usedepartmentcodes = @usedepartments, usecostcodedesc = @usecostcodedesc, usedepartmentdesc = @usedepartmentdesc, attachreceipts = @attachreceipts, singleclaim = @singleclaim, exchangereadonly = @exchangereadonly, useprojectcodes = @useprojectcodes, useprojectcodedesc = @useprojectcodedesc, addlocations = @addlocations, costcodeson = @costcodeson, departmentson = @departmentson, projectcodeson = @projectcodeson, autoassignallocation = @autoassignallocation, blocklicenceexpiry = @blocklicenceexpiry, blocktaxexpiry = @blocktaxexpiry, blockmotexpiry = @blockmotexpiry, blockinsuranceexpiry = @blockinsuranceexpiry, addcompanies = @addcompanies, allowmultipledestinations = @allowmultipledestinations, usemappoint = @usemappoint, usecostcodeongendet = @usecostcodeongendet, usedepartmentongendet = @usedepartmentongendet, useprojectcodeongendet = @useprojectcodeongendet, hometooffice = @hometooffice, calchometolocation = @calchometolocation, modifiedon = @modifiedon, modifiedby = @modifiedby, showmileagecategoriesforusers=@allowMilageCats, activatecaronuseradd=@activateCarsOnAdd, autocalchometolocation = @autocalchometolocation, enableautolog = @enableautolog, allowuserstoaddcars = @allowuserstoaddcars, mileage_calculation_type = @mileagecalculationtype";
            expdata.sqlexecute.Parameters.AddWithValue("@allowuserstoaddcars", Convert.ToByte(allowUsersToAddCars));
            expdata.sqlexecute.Parameters.AddWithValue("@enableautolog", Convert.ToByte(enableAutolog));
            expdata.sqlexecute.Parameters.AddWithValue("@allowMilageCats", Convert.ToByte(allowMilageForUsersAdd));
            expdata.sqlexecute.Parameters.AddWithValue("@activateCarsOnAdd", Convert.ToByte(activateCarsOnAdd));
			expdata.sqlexecute.Parameters.AddWithValue("@usecostcodes",Convert.ToByte(costcodes));
			expdata.sqlexecute.Parameters.AddWithValue("@usedepartments",Convert.ToByte(departments));
			expdata.sqlexecute.Parameters.AddWithValue("@singleclaim",Convert.ToByte(singleclaim));
			expdata.sqlexecute.Parameters.AddWithValue("@usecostcodedesc",Convert.ToByte(usecostcodedesc));
			expdata.sqlexecute.Parameters.AddWithValue("@usedepartmentdesc",Convert.ToByte(usedepartmentdesc));
            expdata.sqlexecute.Parameters.AddWithValue("@attachreceipts", Convert.ToByte(attachreceipts));
			expdata.sqlexecute.Parameters.AddWithValue("@exchangereadonly",Convert.ToByte(exchangereadonly));
			expdata.sqlexecute.Parameters.AddWithValue("@useprojectcodes",Convert.ToByte(useprojectcodes));
			expdata.sqlexecute.Parameters.AddWithValue("@useprojectcodedesc",Convert.ToByte(useprojectcodedesc));
			expdata.sqlexecute.Parameters.AddWithValue("@addlocations",Convert.ToByte(addlocations));
			expdata.sqlexecute.Parameters.AddWithValue("@costcodeson",Convert.ToByte(costcodeson));
			expdata.sqlexecute.Parameters.AddWithValue("@departmentson",Convert.ToByte(departmentson));
			expdata.sqlexecute.Parameters.AddWithValue("@projectcodeson",Convert.ToByte(projectcodeson));
			expdata.sqlexecute.Parameters.AddWithValue("@autoassignallocation",Convert.ToByte(autoassignallocation));
            expdata.sqlexecute.Parameters.AddWithValue("@blocklicenceexpiry", Convert.ToByte(blocklicenceexpiry));
            expdata.sqlexecute.Parameters.AddWithValue("@blocktaxexpiry", Convert.ToByte(blocktaxexpiry));
            expdata.sqlexecute.Parameters.AddWithValue("@blockmotexpiry", Convert.ToByte(blockmotexpiry));
            expdata.sqlexecute.Parameters.AddWithValue("@blockinsuranceexpiry", Convert.ToByte(blockinsuranceexpiry));
            expdata.sqlexecute.Parameters.AddWithValue("@addcompanies", Convert.ToByte(addcompanies));
            expdata.sqlexecute.Parameters.AddWithValue("@allowmultipledestinations", Convert.ToByte(allowmultipledestinations));
            expdata.sqlexecute.Parameters.AddWithValue("@usemappoint", Convert.ToByte(usemappoint));
            expdata.sqlexecute.Parameters.AddWithValue("@usecostcodeongendet", Convert.ToByte(usecostcodeongendet));
            expdata.sqlexecute.Parameters.AddWithValue("@usedepartmentongendet", Convert.ToByte(usedepartmentongendet));
            expdata.sqlexecute.Parameters.AddWithValue("@useprojectcodeongendet", Convert.ToByte(useprojectcodeongendet));
            expdata.sqlexecute.Parameters.AddWithValue("@hometooffice", hometooffice);
            expdata.sqlexecute.Parameters.AddWithValue("@calchometolocation", calchometolocation);
            expdata.sqlexecute.Parameters.AddWithValue("@autocalchometolocation", autocalchometolocation);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", user.employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@mileagecalculationtype", (byte)mileagecalculationtype);
            expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

            //InvalidateGlobalProperties(accountid);
		}

		public void updateFlagLimits(int limits, bool duplicates, int mileage, bool weekend, bool limitdates, string initialdate, int limitmonths, bool flagdate, int limitsreceipt, bool increaseothers, int tiplimit, bool rejecttip, bool limitfrequency, int frequencyvalue, byte frequencytype, bool displayflagadded, string flagmessage, bool displaylimits)
		{
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			DateTime dtInitialdate;
            strsql = "update [other] set limits = @limits, limitsreceipt = @limitsreceipt, duplicates = @duplicates, compmileage = @mileage, weekend = @weekends, limitdates = @limitdates, initialdate = @initialdate, limitmonths = @limitmonths, increaseothers = @increaseothers, flagdate = @flagdate, tiplimit = @tiplimit, rejecttip = @rejecttip, limitfrequency = @limitfrequency, frequencyvalue = @frequencyvalue, frequencytype = @frequencytype, displayflagadded = @displayflagadded, flagmessage = @flagmessage, displaylimits = @displaylimits, modifiedon = @modifiedon, modifiedby = @modifiedby";
            
            expdata.sqlexecute.Parameters.AddWithValue("@limits", limits);
            expdata.sqlexecute.Parameters.AddWithValue("@limitsreceipt", limitsreceipt);
            expdata.sqlexecute.Parameters.AddWithValue("@duplicates", Convert.ToByte(duplicates));
            expdata.sqlexecute.Parameters.AddWithValue("@mileage", mileage);
            expdata.sqlexecute.Parameters.AddWithValue("@weekends", Convert.ToByte(weekend));
            expdata.sqlexecute.Parameters.AddWithValue("@limitdates", Convert.ToByte(limitdates));
            expdata.sqlexecute.Parameters.AddWithValue("@flagdate", Convert.ToByte(flagdate));
			
			if (limitdates == true)
			{
				if (initialdate == "")
				{
                    expdata.sqlexecute.Parameters.AddWithValue("@initialdate", DBNull.Value);
					
				}
				else
				{
					dtInitialdate = DateTime.Parse(initialdate);
                    expdata.sqlexecute.Parameters.AddWithValue("@initialdate", dtInitialdate.Year + "/" + dtInitialdate.Month + "/" + dtInitialdate.Day);
				}
				if (limitmonths == 0)
				{
                    expdata.sqlexecute.Parameters.AddWithValue("@limitmonths", DBNull.Value);
				}
				else
				{
                    expdata.sqlexecute.Parameters.AddWithValue("@limitmonths", limitmonths);
				}
			}
			else
			{
                expdata.sqlexecute.Parameters.AddWithValue("@initialdate", DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@limitmonths", DBNull.Value);
			}
            expdata.sqlexecute.Parameters.AddWithValue("@increaseothers", Convert.ToByte(increaseothers));
            expdata.sqlexecute.Parameters.AddWithValue("@tiplimit", tiplimit);
            expdata.sqlexecute.Parameters.AddWithValue("@rejecttip", Convert.ToByte(rejecttip));
            expdata.sqlexecute.Parameters.AddWithValue("@limitfrequency", Convert.ToByte(limitfrequency));
			if (limitfrequency == true)
			{
                expdata.sqlexecute.Parameters.AddWithValue("@frequencyvalue", frequencyvalue);
                expdata.sqlexecute.Parameters.AddWithValue("@frequencytype", frequencytype);
			}
			else
			{
                expdata.sqlexecute.Parameters.AddWithValue("@frequencyvalue", DBNull.Value);
                expdata.sqlexecute.Parameters.AddWithValue("@frequencytype", DBNull.Value);
			}
            expdata.sqlexecute.Parameters.AddWithValue("@displayflagadded", Convert.ToByte(displayflagadded));
            expdata.sqlexecute.Parameters.AddWithValue("@flagmessage", flagmessage);
            expdata.sqlexecute.Parameters.AddWithValue("@displaylimits", Convert.ToByte(displaylimits));
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", user.employeeid);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

            //InvalidateGlobalProperties(accountid);
		}
        public void updateGeneralOptions(bool searchemployees, bool preapproval, bool showreviews, bool showbankdetails, bool recordodometer, byte odometerday, bool partsubmittal, bool onlycashcredit, bool locationsearch, bool editmydetails, bool enterodometeronsubmit, bool allowselfreg, bool selfregempcontact, bool selfreghomeaddr, bool selfregempinfo, bool selfregrole, bool selfregsignoff, bool selfregadvancessignoff, bool selfregdepcostcode, bool selfregbankdetails, bool selfregcardetails, bool selfregudf, int defaultrole, int drilldownreport, bool delsetup, bool delemployeeadmin, bool delemployeeaccounts, bool delreports, bool delreportsreadonly, bool delcheckandpay, bool delqedesign, bool delcreditcard, bool delpurchasecards, bool delapprovals, bool delexports, bool delauditlog, bool sendreviewrequest, bool claimantdeclaration, string declarationmsg, string approverdeclarationmsg)
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "update [other] set searchemployees = @searchemployees, preapproval = @preapproval, showreviews = @showreviews, showbankdetails = @showbankdetails, recordodometer = @recordodometer, odometerday = @odometerday, partsubmittal = @partsubmittal, onlycashcredit = @onlycashcredit, locationsearch = @locationsearch, editmydetails = @editmydetails, enterodometeronsubmit = @enterodometeronsubmit, allowselfreg = @allowselfreg, selfregempcontact = @selfregempcontact, selfreghomeaddr = @selfreghomeaddr, selfregempinfo = @selfregempinfo, selfregrole = @selfregrole, selfregsignoff = @selfregsignoff, selfregadvancesignoff = @selfregadvancessignoff, selfregbankdetails = @selfregbankdetails, selfregdepcostcode = @selfregdepcostcode, selfregcardetails = @selfregcardetails, selfregudf = @selfregudf, defaultrole = @defaultrole, drilldownreport = @drilldownreport " +
                ", delsetup = @delsetup, delemployeeadmin = @delemployeeadmin, delemployeeaccounts = @delemployeeaccounts, delreports = @delreports, delreportsreadonly = @delreportsreadonly, delcheckandpay = @delcheckandpay, delqedesign = @delqedesign, delcreditcard = @delcreditcard, delpurchasecards = @delpurchasecards, delapprovals = @delapprovals, delexports = @delexports, delauditlog = @delauditlog, sendreviewrequest = @sendreviewrequest, claimantdeclaration = @claimantdeclaration, declarationmsg = @declarationmsg, approverdeclarationmsg = @approverdeclarationmsg, modifiedon = @modifiedon, modifiedby = @modifiedby ";
            
            

            expdata.sqlexecute.Parameters.AddWithValue("@searchemployees", Convert.ToByte(searchemployees));
            
            
            

            expdata.sqlexecute.Parameters.AddWithValue("@preapproval", Convert.ToByte(preapproval));
            expdata.sqlexecute.Parameters.AddWithValue("@showreviews", Convert.ToByte(showreviews));
            expdata.sqlexecute.Parameters.AddWithValue("@showbankdetails", Convert.ToByte(showbankdetails));
            expdata.sqlexecute.Parameters.AddWithValue("@recordodometer", Convert.ToByte(recordodometer));
            expdata.sqlexecute.Parameters.AddWithValue("@odometerday", odometerday);
            expdata.sqlexecute.Parameters.AddWithValue("@partsubmittal", Convert.ToByte(partsubmittal));
            expdata.sqlexecute.Parameters.AddWithValue("@onlycashcredit", Convert.ToByte(onlycashcredit));
            expdata.sqlexecute.Parameters.AddWithValue("@locationsearch", Convert.ToByte(locationsearch));


            expdata.sqlexecute.Parameters.AddWithValue("@editmydetails", Convert.ToByte(editmydetails));
            expdata.sqlexecute.Parameters.AddWithValue("@enterodometeronsubmit", Convert.ToByte(enterodometeronsubmit));
            
            
            expdata.sqlexecute.Parameters.AddWithValue("@allowselfreg", Convert.ToByte(allowselfreg));
            expdata.sqlexecute.Parameters.AddWithValue("@selfregempcontact", Convert.ToByte(selfregempcontact));
            expdata.sqlexecute.Parameters.AddWithValue("@selfreghomeaddr", Convert.ToByte(selfreghomeaddr));
            expdata.sqlexecute.Parameters.AddWithValue("@selfregempinfo", Convert.ToByte(selfregempinfo));
            expdata.sqlexecute.Parameters.AddWithValue("@selfregrole", Convert.ToByte(selfregrole));
            expdata.sqlexecute.Parameters.AddWithValue("@selfregsignoff", Convert.ToByte(selfregsignoff));
            expdata.sqlexecute.Parameters.AddWithValue("@selfregadvancessignoff", Convert.ToByte(selfregadvancessignoff));
            expdata.sqlexecute.Parameters.AddWithValue("@selfregdepcostcode", Convert.ToByte(selfregdepcostcode));
            expdata.sqlexecute.Parameters.AddWithValue("@selfregbankdetails", Convert.ToByte(selfregbankdetails));
            expdata.sqlexecute.Parameters.AddWithValue("@selfregcardetails", Convert.ToByte(selfregcardetails));
            expdata.sqlexecute.Parameters.AddWithValue("@selfregudf", Convert.ToByte(selfregudf));
            if (defaultrole == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@defaultrole", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@defaultrole", defaultrole);
            }
            
            
            if (drilldownreport == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@drilldownreport", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@drilldownreport", drilldownreport);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@delsetup", Convert.ToByte(delsetup));
            expdata.sqlexecute.Parameters.AddWithValue("@delemployeeadmin", Convert.ToByte(delemployeeadmin));
            expdata.sqlexecute.Parameters.AddWithValue("@delemployeeaccounts", Convert.ToByte(delemployeeaccounts));
            expdata.sqlexecute.Parameters.AddWithValue("@delreports", Convert.ToByte(delreports));
            expdata.sqlexecute.Parameters.AddWithValue("@delreportsreadonly", Convert.ToByte(delreportsreadonly));
            expdata.sqlexecute.Parameters.AddWithValue("@delcheckandpay", Convert.ToByte(delcheckandpay));
            expdata.sqlexecute.Parameters.AddWithValue("@delqedesign", Convert.ToByte(delqedesign));
            expdata.sqlexecute.Parameters.AddWithValue("@delcreditcard", Convert.ToByte(delcreditcard));
            expdata.sqlexecute.Parameters.AddWithValue("@delpurchasecards", Convert.ToByte(delpurchasecards));
            expdata.sqlexecute.Parameters.AddWithValue("@delapprovals", Convert.ToByte(delapprovals));
            expdata.sqlexecute.Parameters.AddWithValue("@delexports", Convert.ToByte(delexports));
            expdata.sqlexecute.Parameters.AddWithValue("@delauditlog", Convert.ToByte(delauditlog));
            expdata.sqlexecute.Parameters.AddWithValue("@sendreviewrequest", Convert.ToByte(sendreviewrequest));
            expdata.sqlexecute.Parameters.AddWithValue("@claimantdeclaration", Convert.ToByte(claimantdeclaration));
            expdata.sqlexecute.Parameters.AddWithValue("@declarationmsg", declarationmsg);
            expdata.sqlexecute.Parameters.AddWithValue("@approverdeclarationmsg", approverdeclarationmsg);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", user.employeeid);

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            //InvalidateGlobalProperties(accountid);
        }

        public void updateRegionalOptions(int homecountry, string language, int basecurrency)
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "update [other] set homecountry = @homecountry, [language] = @language, basecurrency = @basecurrency, modifiedon = @modifiedon, modifiedby = @modifiedby";
            
            expdata.sqlexecute.Parameters.AddWithValue("@homecountry",homecountry);
            if (language == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@language", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@language", language);
            }
            if (basecurrency == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", basecurrency);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", user.employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            //InvalidateGlobalProperties(accountid);
        }
        public void updateMainAdministrator(int adminid)
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "update [other] set mainadministrator = @mainadmin, modifiedon = @modifiedon, modifiedby = @modifiedby";
            
            if (adminid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mainadmin", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mainadmin", adminid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", user.employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            //InvalidateGlobalProperties(accountid);
        }
	public void updatePreferences ()
	{

        //InvalidateGlobalProperties(accountid);

			
			
		}
		
		public void addRoleDefaultsToTemplate (int employeeid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cEmployees clsemployees = new cEmployees(accountid);
			cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);

			
				System.Data.SqlClient.SqlDataReader reader;
				strsql = "select subcatid from rolesubcats where roleid = @roleid and isadditem = 1";
				expdata.sqlexecute.Parameters.AddWithValue("@roleid",reqemp.roleid);
				reader = expdata.GetReader(strsql);

				strsql = "";
				while (reader.Read())
				{
					strsql += "insert into additems (employeeid, subcatid) values (" + employeeid + "," + reader.GetInt32(0) + ");";
				}
				reader.Close();
				if (strsql != "")
				{
					expdata.ExecuteSQL(strsql);
				}
			
		}	
		public void addSubcatToTemplate (int employeeid, int subcatid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cEmployees clsemployees = new cEmployees(accountid);
			cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);

			if (reqemp.customiseditems == false)
			{
				addRoleDefaultsToTemplate(employeeid);
				reqemp.hasCustomisedItems();
			}
			strsql = "insert into additems (employeeid, subcatid) values (@employeeid, @subcatid)";
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			expdata.sqlexecute.Parameters.AddWithValue("@subcatid",subcatid);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

        
		public void removeSubcatToTemplate (int employeeid, int subcatid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cEmployees clsemployees = new cEmployees(accountid);
			cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
			if (reqemp.customiseditems == false)
			{
				addRoleDefaultsToTemplate(employeeid);
				reqemp.hasCustomisedItems();
			}
			strsql = "delete from additems where employeeid = @employeeid and subcatid = @subcatid";
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			expdata.sqlexecute.Parameters.AddWithValue("@subcatid",subcatid);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}


		public void updateMultipleItems(int employeeid, int[] subcatids)
		{

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int i = 0;
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
			strsql = "delete from additems where employeeid = @employeeid";
			expdata.ExecuteSQL(strsql);

			for (i = 0; i < subcatids.Length; i++)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@subcatid" + i,subcatids[i]);
				strsql = "insert into additems (employeeid, subcatid) " +
					"values (@employeeid,@subcatid" + i + ")";
				expdata.ExecuteSQL(strsql);
			}
			expdata.sqlexecute.Parameters.Clear();

			cEmployees clsemployees = new cEmployees(accountid);
			cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
            reqemp.useritems.Clear();
            foreach (int x in subcatids)
            {
                reqemp.useritems.Add(x);
            }
            
		}

		

		public bool checkAvailableLicences(int accountid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int usedcount = 0;
            cAccounts clsaccounts = new cAccounts();
			cAccount reqaccount = clsaccounts.getAccountById(accountid);
			
			strsql = "select count(*) from employees where archived = 0 and username not like 'admin%'";
			usedcount = expdata.getcount(strsql);

			expdata.sqlexecute.Parameters.Clear();
			
			if (usedcount < reqaccount.numusers)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public List<Guid> getPrintoutFields()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            List<Guid> fields = new List<Guid>();
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select fieldid from printout";
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                fields.Add(reader.GetGuid(0));
            }
            reader.Close();
            return fields;
        }
		

		public void updatePrintOut(int[] fieldids)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int i = 0;
			
			strsql = "delete from printout";
			expdata.ExecuteSQL(strsql);

			strsql = "";

			for (i = 0; i < fieldids.Length; i++)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@fieldid" + i,fieldids[i]);
				strsql += "insert into printout (fieldid) " +
					"values (@fieldid" + i + ")";
			}
			if (strsql != "")
			{
				expdata.ExecuteSQL(strsql);
			}
			expdata.sqlexecute.Parameters.Clear();
		}

		public void generatePrintOutFields(ref System.Web.UI.WebControls.Panel fields, int claimid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			string fieldsql;
			int i;
			System.Web.UI.WebControls.TableRow row = new System.Web.UI.WebControls.TableRow();;
			System.Web.UI.WebControls.TableCell cell;
			System.Data.SqlClient.SqlDataReader reader;
			int cellcount = 0;
			fieldsql = createPrintHeaderSQL();

			expdata.sqlexecute.Parameters.AddWithValue("@claimid",claimid);
			if (fieldsql == "")
			{
				return;
			}
            fieldsql += " where claims.claimid = @claimid;";

			reader = expdata.GetReader(fieldsql);
			while (reader.Read())
			{
				for (i = 0; i < reader.FieldCount; i++)
				{
					if (cellcount == 0)
					{
						row = new System.Web.UI.WebControls.TableRow();
					}
					
						cell = new System.Web.UI.WebControls.TableCell();
						cell.Text = reader.GetName(i).ToString() + ":";
						cell.CssClass = "labeltd";
						
						
						row.Cells.Add(cell);
						cell = new System.Web.UI.WebControls.TableCell();
						cell.Text = reader.GetValue(i).ToString();
						cell.CssClass = "inputtd";
						row.Cells.Add(cell);
						cellcount++;
						if (cellcount == 2)
						{
							fields.Controls.Add(row);
							cellcount = 0;
						}
					
				}
				fields.Controls.Add(row);
			}

			reader.Close();
			
			expdata.sqlexecute.Parameters.Clear();
		}
		private string createPrintHeaderSQL()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			string sql = "";
			int i;
			Guid basetableid;
			string jointype = "";
			cFields clsfields = new cFields(accountid);
            
			System.Data.DataSet rcdstfields = new System.Data.DataSet();
			System.Data.DataSet rcdsttables = new System.Data.DataSet();

            List<cField> fields = new List<cField>();
            List<int> tables = new List<int>();
            System.Data.SqlClient.SqlDataReader reader;

            cField reqField;

            basetableid = new Guid("0efa50b5-da7b-49c7-a9aa-1017d5f741d0");
			//get the list of fields required
            strsql = "select fieldid from printout";
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
fields.Add(clsfields.getFieldById(reader.GetGuid(0)));
                if (reqField != null)
                {
                    fields.Add(reqField);

                    if (!tables.Contains(reqField.tableid))
                    {
                        tables.Add(reqField.tableid);
                    }
                }
                
            }
            reader.Close();

            sql = "";


            if (tables.Contains(50))
            {
                sql += "if exists(Select * From tempdb.dbo.sysobjects Where name = '##employees_userdefined" + accountid + "')\n";
                sql += "begin\n";
                sql += "drop table ##employees_userdefined" + accountid + "\n";
                sql += "end\n";
                sql += "create table ##employees_userdefined" + accountid + " (label nvarchar(50), value nvarchar(2000), employeeid int)\n";
                sql += "EXEC	[dbo].[sp_Crosstab]\n";
                sql += "@DBFetch = N'select label, userdefined_values.value, recordid as employeeid from userdefined left join userdefined_values on userdefined.userdefineid = userdefined_values.userdefineid where userdefined.appliesto = 1',\n";
                sql += "@DBField = N'label',\n";
                sql += "@PCField = N'value',\n";
                sql += "@PCBuild = N'max',\n";
                sql += "@DBTable = N'##employees_userdefined" + accountid + "'\n";
            }

            if (tables.Contains(78))
            {
                sql += "if exists(Select * From tempdb.dbo.sysobjects Where name = '##companies_userdefined" + accountid + "')\n";
                sql += "begin\n";
                sql += "drop table ##companies_userdefined" + accountid + "\n";
                sql += "end\n";
                sql += "create table ##companies_userdefined" + accountid + " (label nvarchar(50), value nvarchar(2000), companyid int)\n";
                sql += "EXEC	[dbo].[sp_Crosstab]\n";
                sql += "@DBFetch = N'select label, userdefined_values.value, recordid as companyid from userdefined left join userdefined_values on userdefined.userdefineid = userdefined_values.userdefineid where userdefined.appliesto = 6',\n";
                sql += "@DBField = N'label',\n";
                sql += "@PCField = N'value',\n";
                sql += "@PCBuild = N'max',\n";
                sql += "@DBTable = N'##companies_userdefined" + accountid + "'\n";
            }

            if (tables.Contains(79))
            {
                sql += "if exists(Select * From tempdb.dbo.sysobjects Where name = '##costcodes_userdefined" + accountid + "')\n";
                sql += "begin\n";
                sql += "drop table ##costcodes_userdefined" + accountid + "\n";
                sql += "end\n";
                sql += "create table ##costcodes_userdefined" + accountid + " (label nvarchar(50), value nvarchar(2000), costcodeid int)\n";
                sql += "EXEC	[dbo].[sp_Crosstab]\n";
                sql += "@DBFetch = N'select label, userdefined_values.value, recordid as costcodeid from userdefined left join userdefined_values on userdefined.userdefineid = userdefined_values.userdefineid where userdefined.appliesto = 7',\n";
                sql += "@DBField = N'label',\n";
                sql += "@PCField = N'value',\n";
                sql += "@PCBuild = N'max',\n";
                sql += "@DBTable = N'##costcodes_userdefined" + accountid + "'\n";
            }

            if (tables.Contains(80))
            {
                sql += "if exists(Select * From tempdb.dbo.sysobjects Where name = '##departments_userdefined" + accountid + "')\n";
                sql += "begin\n";
                sql += "drop table ##departments_userdefined" + accountid + "\n";
                sql += "end\n";
                sql += "create table ##departments_userdefined" + accountid + " (label nvarchar(50), value nvarchar(2000), departmentid int)\n";
                sql += "EXEC	[dbo].[sp_Crosstab]\n";
                sql += "@DBFetch = N'select label, userdefined_values.value, recordid as departmentid from userdefined left join userdefined_values on userdefined.userdefineid = userdefined_values.userdefineid where userdefined.appliesto = 8',\n";
                sql += "@DBField = N'label',\n";
                sql += "@PCField = N'value',\n";
                sql += "@PCBuild = N'max',\n";
                sql += "@DBTable = N'##departments_userdefined" + accountid + "'\n";
            }

            if (tables.Contains(81))
            {
                sql += "if exists(Select * From tempdb.dbo.sysobjects Where name = '##projectcodes_userdefined" + accountid + "')\n";
                sql += "begin\n";
                sql += "drop table ##projectcodes_userdefined" + accountid + "\n";
                sql += "end\n";
                sql += "create table ##projectcodes_userdefined" + accountid + " (label nvarchar(50), value nvarchar(2000), projectcodeid int)\n";
                sql += "EXEC	[dbo].[sp_Crosstab]\n";
                sql += "@DBFetch = N'select label, userdefined_values.value, recordid as projectcodeid from userdefined left join userdefined_values on userdefined.userdefineid = userdefined_values.userdefineid where userdefined.appliesto = 9',\n";
                sql += "@DBField = N'label',\n";
                sql += "@PCField = N'value',\n";
                sql += "@PCBuild = N'max',\n";
                sql += "@DBTable = N'##projectcodes_userdefined" + accountid + "'\n";
            }

            if (tables.Contains(49))
            {
                sql += "if exists(Select * From tempdb.dbo.sysobjects Where name = '##savedexpenses_userdefined" + accountid + "')\n";
                sql += "begin\n";
                sql += "drop table ##savedexpenses_userdefined" + accountid + "\n";
                sql += "end\n";
                sql += "create table ##savedexpenses_userdefined" + accountid + " (label nvarchar(50), value nvarchar(2000), expenseid int)\n";
                sql += "EXEC	[dbo].[sp_Crosstab]\n";
                sql += "@DBFetch = N'select label, userdefined_values.value, recordid as expenseid from userdefined left join userdefined_values on userdefined.userdefineid = userdefined_values.userdefineid where userdefined.appliesto = 2',\n";
                sql += "@DBField = N'label',\n";
                sql += "@PCField = N'value',\n";
                sql += "@PCBuild = N'max',\n";
                sql += "@DBTable = N'##savedexpenses_userdefined" + accountid + "';\n";
                sql += "CREATE INDEX IX_udf_savedexpenses on ##savedexpenses_userdefined" + accountid + " (expenseid);\n";
            }


            if (tables.Contains(51))
            {
                sql += "if exists(Select * From tempdb.dbo.sysobjects Where name = '##claims_userdefined" + accountid + "')\n";
                sql += "begin\n";
                sql += "drop table ##claims_userdefined" + accountid + "\n";
                sql += "end\n";
                sql += "create table ##claims_userdefined" + accountid + " (label nvarchar(50), value nvarchar(2000), claimid int)\n";
                sql += "EXEC	[dbo].[sp_Crosstab]\n";
                sql += "@DBFetch = N'select label, userdefined_values.value, recordid as claimid from userdefined left join userdefined_values on userdefined.userdefineid = userdefined_values.userdefineid where userdefined.appliesto = 3',\n";
                sql += "@DBField = N'label',\n";
                sql += "@PCField = N'value',\n";
                sql += "@PCBuild = N'max',\n";
                sql += "@DBTable = N'##claims_userdefined" + accountid + "';\n";
                sql += "CREATE INDEX IX_udf_claims on ##claims_userdefined" + accountid + " (claimid);\n";
            }

            if (tables.Contains(55))
            {
                sql += "if exists(Select * From tempdb.dbo.sysobjects Where name = '##subcats_userdefined_values" + accountid + "')\n";
                sql += "begin\n";
                sql += "drop table ##subcats_userdefined_values" + accountid + "\n";
                sql += "end\n";
                sql += "create table ##subcats_userdefined_values" + accountid + " (label nvarchar(50), value nvarchar(2000), subcatid int)\n";
                sql += "EXEC	[dbo].[sp_Crosstab]\n";
                sql += "@DBFetch = N'select label, userdefined_values.value, recordid as subcatid from userdefined left join userdefined_values on userdefined.userdefineid = userdefined_values.userdefineid where userdefined.appliesto = 4',\n";
                sql += "@DBField = N'label',\n";
                sql += "@PCField = N'value',\n";
                sql += "@PCBuild = N'max',\n";
                sql += "@DBTable = N'##subcats_userdefined_values" + accountid + "';\n";
                sql += "CREATE INDEX IX_udf_subcats on ##subcats_userdefined_values" + accountid + " (subcatid);\n";
            }

            if (tables.Contains(74))
            {
                sql += "if exists(Select * From tempdb.dbo.sysobjects Where name = '##cars_userdefined" + accountid + "')\n";
                sql += "begin\n";
                sql += "drop table ##cars_userdefined" + accountid + "\n";
                sql += "end\n";
                sql += "create table ##cars_userdefined" + accountid + " (label nvarchar(50), value nvarchar(2000), carid int)\n";
                sql += "EXEC [dbo].[sp_Crosstab]\n";
                sql += "@DBFetch = N'select label, userdefined_values.value, recordid as carid from userdefined left join userdefined_values on userdefined.userdefineid = userdefined_values.userdefineid where userdefined.appliesto = 5',\n";
                sql += "@DBField = N'label',\n";
                sql += "@PCField = N'value',\n";
                sql += "@PCBuild = N'max',\n";
                sql += "@DBTable = N'##cars_userdefined" + accountid + "'\n";
                sql += "CREATE INDEX IX_udf_cars on ##cars_userdefined" + accountid + " (carid);\n";
            }

			sql += "select distinct";
			if (fields.Count == 0)
			{
				return "";
			}
			foreach (cField field in fields)
			{
                
                if (field.table.tablename.Substring(0, 2) == "##")
                {
                    sql = sql + "[" + field.table.tablename + accountid + "].[" + field.field  + "] as [" + field.description + "],";
                }
                else
                {
                    sql = sql + "[" + field.table.tablename + "].[" + field.field + "] as [" + field.description + "],";
                }
			}
			sql = sql.Remove(sql.Length - 1,1);
			sql = sql + " from claims";

            cJoins clsjoins = new cJoins(accountid,cAccounts.getConnectionString(accountid), ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString, new cTables(accountid), new cFields(accountid));
            sql += " " + clsjoins.createJoinSQL(fields, basetableid);
			
			
			
			
			expdata.sqlexecute.Parameters.Clear();
			return sql;
        }

        public int getAccountIdByEmailSuffix(string suffix)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            
            int count;
            strsql = "select count(*) from email_suffixes where suffix = @suffix";

            try
            {
                expdata.sqlexecute.Parameters.AddWithValue("@suffix", suffix);
                count = expdata.getcount(strsql);

                expdata.sqlexecute.Parameters.Clear();
                if (count > 0)
                {
                    return accountid;
                }
            }
            catch
            {
                return 0;
            }
            return 0;

        }
        #region global properties

       
        public cGlobalProperties getGlobalProperties(int accountid)
        {
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

            cGlobalProperties properties = (cGlobalProperties)Cache["globalproperties" + accountid];
            if (properties == null)
            {
                properties = getGlobalPropertiesFromDB(accountid);
            }
            return properties;
        }

        public void InvalidateGlobalProperties(int accountid)
        {
            System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpContext.Current.Cache;
            Cache.Remove("globalproperties" + accountid);
        }

        public void saveDefaultSort(Grid grid, string columnname, byte direction)
        {
            byte sortorder;
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);
            strsql = "delete from default_sorts where gridid = @gridid and employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", user.employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@gridid", (int)grid);
            expdata.ExecuteSQL(strsql);
           

            strsql = "insert into default_sorts (employeeid, gridid, columnname, defaultorder) " +
                "values (@employeeid, @gridid, @columnname, @defaultorder)";
            expdata.sqlexecute.Parameters.AddWithValue("@columnname", columnname);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultorder", direction);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp = clsemployees.GetEmployeeById(user.employeeid);
            cGridSort sort = reqemp.getGridSort(grid);
            if (sort != null)
            {
                sort.columnname = columnname;
                sort.sortorder = direction;
            }
        }
        public cGlobalProperties getGlobalPropertiesFromDB(int accountid)
        {
            System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            
            cGlobalProperties properties = null;
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select  mileage, server, currencytype, dbversion, numrows, attempts, expiry, plength, length1, length2, pupper, pnumbers, previous, thresholdtype, homeCountry, limits, policytype, duplicates, curimportid, compmileage, weekend, usecostcodes, usedepartmentcodes, ccadmin, usecostcodedesc, singleclaim, usedepartmentdesc, attachreceipts, ccusersettles, limitdates, initialdate, limitmonths, flagdate, limitsreceipt, mainadministrator, increaseothers, searchemployees, preapproval, showreviews, showbankdetails, mileageprev, minclaimamount, maxclaimamount, exchangereadonly, tiplimit, useprojectcodedesc, useprojectcodes, recordodometer, odometerday, addlocations, rejecttip, costcodeson, departmentson, projectcodeson, partsubmittal, onlycashcredit, locationsearch, language, currencysymbol, currencydelimiter, limitfrequency, frequencytype, frequencyvalue, overridehome, sourceaddress, editmydetails, autoassignallocation, enterodometeronsubmit, displayflagadded, flagmessage, basecurrency, selfregempcontact, allowselfreg, selfreghomeaddr, selfregempinfo, selfregrole, selfregsignoff, selfregadvancesignoff, selfregdepcostcode, selfregbankdetails, selfregcardetails, selfregudf, defaultrole, singleclaimcc, singleclaimpc, displaylimits, drilldownreport, blockcashcc, blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, delsetup, delemployeeadmin, delemployeeaccounts, delreports, delreportsreadonly, delcheckandpay, delqedesign, delcreditcard, delpurchasecards, delapprovals, delexports, delauditlog, sendreviewrequest, claimantdeclaration, blockcashpc, addcompanies, allowmultipledestinations, usemappoint, usecostcodeongendet, usedepartmentongendet, useprojectcodeongendet, hometooffice, calchometolocation, modifiedon, modifiedby, showmileagecategoriesforusers, activatecaronuseradd, autocalchometolocation, enableautolog, allowuserstoaddcars, mileage_calculation_type  from dbo.[other]";
            
            
            DBConnection cachedata = new DBConnection(cAccounts.getConnectionString(accountid));
            cachedata.sqlexecute.CommandText = strsql;
            
            SqlCacheDependency dep = new SqlCacheDependency(cachedata.sqlexecute);
            cachedata.ExecuteSQL(strsql);

            strsql = "select  mileage, server, currencytype, dbversion, standards, numrows, attempts, expiry, plength, length1, length2, pupper, pnumbers, previous, thresholdtype, homeCountry, limits, policytype, duplicates, curimportid, compmileage, weekend, usecostcodes, usedepartmentcodes, ccadmin, usecostcodedesc, singleclaim, usedepartmentdesc, attachreceipts, limitdates, initialdate, limitmonths, flagdate, limitsreceipt, mainadministrator, increaseothers, searchemployees, preapproval, showreviews, showbankdetails, mileageprev, minclaimamount, maxclaimamount, exchangereadonly, tiplimit, useprojectcodedesc, useprojectcodes, recordodometer, odometerday, addlocations, rejecttip, costcodeson, departmentson, projectcodeson, partsubmittal, onlycashcredit, locationsearch, language, currencysymbol, currencydelimiter, limitfrequency, frequencytype, frequencyvalue, overridehome, sourceaddress, editmydetails, autoassignallocation, enterodometeronsubmit, displayflagadded, flagmessage, basecurrency, selfregempcontact, allowselfreg, selfreghomeaddr, selfregempinfo, selfregrole, selfregsignoff, selfregadvancesignoff, selfregdepcostcode, selfregbankdetails, selfregcardetails, selfregudf, defaultrole, singleclaimcc, singleclaimpc, displaylimits, drilldownreport, blockcashcc, blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, delsetup, delemployeeadmin, delemployeeaccounts, delreports, delreportsreadonly, delcheckandpay, delqedesign, delcreditcard, delpurchasecards, delapprovals, delexports, delauditlog, sendreviewrequest, claimantdeclaration, declarationmsg, approverdeclarationmsg, blockcashpc, addcompanies, allowmultipledestinations, usemappoint, usecostcodeongendet, usedepartmentongendet, useprojectcodeongendet, hometooffice, calchometolocation, modifiedon, modifiedby, showmileagecategoriesforusers, activatecaronuseradd, autocalchometolocation, enableautolog, allowuserstoaddcars, mileage_calculation_type from dbo.[other]";
            //cachedata.sqlexecute.CommandText = strsql;

            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            
            
           

            int mileage, defaultrole;
            string server;
            byte currencytype;
            Int16 dbversion;
            string standards;
            int numrows;
            MileageCalculationType mileagecalculationtype;
            byte attempts;
            int expiry, length1, length2;
            PasswordLength plength;
            bool pupper, pnumbers, displaylimits;
            int previous, thresholdtype, homecountry;
            byte limits, policytype;
            bool duplicates;
            int curimportid;
            byte compmileage;
            bool weekend, usecostcodes, usedepartmentcodes, ccadmin, importcc, singleclaim, usecostcodedesc, usedepartmentdesc, attachreceipts, ccusersettles, limitdates;
            DateTime initialdate;
            int limitmonths;
            bool flagdate;
            byte limitsreceipt;
            int mainadministrator;
            bool increaseothers, searchemployees, preapproval, showreviews, showbankdetails;
            int mileageprev;
            decimal minclaimamount, maxclaimamount;
            bool exchangereadonly;
            int tiplimit;
            bool useprojectcodes;
            bool useprojectcodedesc;
            bool recordodometer;
            byte odometerday;
            bool addlocations, rejecttip, costcodeson, departmentson, projectcodeson, partsubmittal, onlycashcredit, locationsearch;
            string language, currencysymbol, currencydelimiter;
            bool limitfrequency;
            byte frequencytype;
            int frequencyvalue;
            bool overridehome;
            byte sourceaddress;
            bool editmydetails, autoassignallocation, enterodometeronsubmit, displayflagadded;
            string flagmessage;
            int basecurrency;
            int drilldownreport;
            
            bool allowselfreg, selfregempcontact, selfreghomeaddr, selfregempinfo, selfregrole, selfregsignoff, selfregadvancessignoff, selfregdepcostcode, selfregbankdetails, selfregcardetails, selfregudf;
            bool blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry;
            bool singleclaimcc, singleclaimpc;
            bool delsetup, delemployeeadmin, delemployeeaccounts, delreports, delreportsreadonly, delcheckandpay, delqedesign, delcreditcard, delpurchasecards, delapprovals, delexports, delauditlog, sendreviewrequest, claimantdeclaration;
            string declarationmsg, approverdeclarationmsg;
            bool blockcashcc, blockcashpc, addcompanies, allowmultipledestinations, usemappoint;
            bool usecostcodeongendet, usedepartmentongendet, useprojectcodeongendet, hometooffice, calchometolocation;
            DateTime modifiedon;
            int modifiedby;
            bool showmileagecategoriesforusers;
            bool activatecaronuseradd, autocalchometolocation;
            bool enableAutolog;
            bool allowUsersToAddCars;
             
            while (reader.Read())
            {
                mileage = reader.GetInt32(reader.GetOrdinal("mileage"));
                if (reader.IsDBNull(reader.GetOrdinal("server")) == false)
                {
                    server = reader.GetString(reader.GetOrdinal("server"));
                }
                else
                {
                    server = "";
                }
                currencytype = reader.GetByte(reader.GetOrdinal("currencytype"));
                dbversion = reader.GetInt16(reader.GetOrdinal("dbversion"));
                if (reader.IsDBNull(reader.GetOrdinal("standards")) == false)
                {
                    standards = reader.GetString(reader.GetOrdinal("standards"));
                }
                else
                {
                    standards = "";
                }
                numrows = reader.GetInt32(reader.GetOrdinal("numrows"));
                attempts = reader.GetByte(reader.GetOrdinal("attempts"));
                expiry = reader.GetInt32(reader.GetOrdinal("expiry"));
                plength = (PasswordLength)reader.GetInt32(reader.GetOrdinal("plength"));
                length1 = reader.GetInt32(reader.GetOrdinal("length1"));
                length2 = reader.GetInt32(reader.GetOrdinal("length2"));
                pupper = reader.GetBoolean(reader.GetOrdinal("pupper"));
                pnumbers = reader.GetBoolean(reader.GetOrdinal("pnumbers"));
                previous = reader.GetInt32(reader.GetOrdinal("previous"));
                thresholdtype = reader.GetInt32(reader.GetOrdinal("thresholdtype"));
                homecountry = reader.GetInt32(reader.GetOrdinal("homecountry"));
                limits = reader.GetByte(reader.GetOrdinal("limits"));
                policytype = reader.GetByte(reader.GetOrdinal("policytype"));
                duplicates = reader.GetBoolean(reader.GetOrdinal("duplicates"));
                curimportid = reader.GetInt32(reader.GetOrdinal("curimportid"));
                homecountry = reader.GetInt32(reader.GetOrdinal("homecountry"));
                compmileage = reader.GetByte(reader.GetOrdinal("compmileage"));
                attempts = reader.GetByte(reader.GetOrdinal("attempts"));
                weekend = reader.GetBoolean(reader.GetOrdinal("weekend"));
                usecostcodes = reader.GetBoolean(reader.GetOrdinal("usecostcodes"));
                usedepartmentcodes = reader.GetBoolean(reader.GetOrdinal("usedepartmentcodes"));
                
                ccadmin = reader.GetBoolean(reader.GetOrdinal("ccadmin"));
                singleclaim = reader.GetBoolean(reader.GetOrdinal("singleclaim"));
                usecostcodedesc = reader.GetBoolean(reader.GetOrdinal("usecostcodedesc"));
                usedepartmentdesc = reader.GetBoolean(reader.GetOrdinal("usedepartmentdesc"));
                attachreceipts = reader.GetBoolean(reader.GetOrdinal("attachreceipts"));
                limitdates = reader.GetBoolean(reader.GetOrdinal("limitdates"));
                if (reader.IsDBNull(reader.GetOrdinal("initialdate")) == false)
                {
                    initialdate = reader.GetDateTime(reader.GetOrdinal("initialdate"));
                }
                else
                {
                    initialdate = new DateTime(1900, 01, 01);
                }
                if (reader.IsDBNull(reader.GetOrdinal("limitmonths")) == false)
                {
                    limitmonths = reader.GetInt32(reader.GetOrdinal("limitmonths"));
                }
                else
                {
                    limitmonths = 0;
                }
                flagdate = reader.GetBoolean(reader.GetOrdinal("flagdate"));
                limitsreceipt = reader.GetByte(reader.GetOrdinal("limitsreceipt"));
                if (reader.IsDBNull(reader.GetOrdinal("mainadministrator")) == false)
                {
                    mainadministrator = reader.GetInt32(reader.GetOrdinal("mainadministrator"));
                }
                else
                {
                    mainadministrator = 0;
                }
                increaseothers = reader.GetBoolean(reader.GetOrdinal("increaseothers"));
                searchemployees = reader.GetBoolean(reader.GetOrdinal("searchemployees"));
                preapproval = reader.GetBoolean(reader.GetOrdinal("preapproval"));
                showreviews = reader.GetBoolean(reader.GetOrdinal("showreviews"));
                showbankdetails = reader.GetBoolean(reader.GetOrdinal("showbankdetails"));
                mileageprev = reader.GetInt32(reader.GetOrdinal("mileageprev"));
                minclaimamount = reader.GetDecimal(reader.GetOrdinal("minclaimamount"));
                maxclaimamount = reader.GetDecimal(reader.GetOrdinal("maxclaimamount"));
                exchangereadonly = reader.GetBoolean(reader.GetOrdinal("exchangereadonly"));
                tiplimit = reader.GetInt32(reader.GetOrdinal("tiplimit"));
                useprojectcodes = reader.GetBoolean(reader.GetOrdinal("useprojectcodes"));
                useprojectcodedesc = reader.GetBoolean(reader.GetOrdinal("useprojectcodedesc"));
                recordodometer = reader.GetBoolean(reader.GetOrdinal("recordodometer"));
                if (reader.IsDBNull(reader.GetOrdinal("odometerday")) == false)
                {
                    odometerday = reader.GetByte(reader.GetOrdinal("odometerday"));
                }
                else
                {
                    odometerday = 0;
                }
                addlocations = reader.GetBoolean(reader.GetOrdinal("addlocations"));
                
                rejecttip = reader.GetBoolean(reader.GetOrdinal("rejecttip"));
                costcodeson = reader.GetBoolean(reader.GetOrdinal("costcodeson"));
                departmentson = reader.GetBoolean(reader.GetOrdinal("departmentson"));
                projectcodeson = reader.GetBoolean(reader.GetOrdinal("projectcodeson"));
                partsubmittal = reader.GetBoolean(reader.GetOrdinal("partsubmittal"));
                onlycashcredit = reader.GetBoolean(reader.GetOrdinal("onlycashcredit"));
                locationsearch = reader.GetBoolean(reader.GetOrdinal("locationsearch"));
                
                if (reader.IsDBNull(reader.GetOrdinal("language")) == false)
                {
                    language = reader.GetString(reader.GetOrdinal("language"));
                }
                else
                {
                    language = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("currencysymbol")) == false)
                {
                    currencysymbol = reader.GetString(reader.GetOrdinal("currencysymbol"));
                }
                else
                {
                    currencysymbol = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("currencydelimiter")) == false)
                {
                    currencydelimiter = reader.GetString(reader.GetOrdinal("currencydelimiter"));
                }
                else
                {
                    currencydelimiter = "";
                }
                limitfrequency = reader.GetBoolean(reader.GetOrdinal("limitfrequency"));
                if (reader.IsDBNull(reader.GetOrdinal("frequencytype")) == false)
                {
                    frequencytype = reader.GetByte(reader.GetOrdinal("frequencytype"));
                }
                else
                {
                    frequencytype = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("frequencyvalue")) == false)
                {
                    frequencyvalue = reader.GetInt32(reader.GetOrdinal("frequencyvalue"));
                }
                else
                {
                    frequencyvalue = 0;
                }
                overridehome = reader.GetBoolean(reader.GetOrdinal("overridehome"));
                sourceaddress = reader.GetByte(reader.GetOrdinal("sourceaddress"));
                editmydetails = reader.GetBoolean(reader.GetOrdinal("editmydetails"));
                autoassignallocation = reader.GetBoolean(reader.GetOrdinal("autoassignallocation"));
                enterodometeronsubmit = reader.GetBoolean(reader.GetOrdinal("enterodometeronsubmit"));
                displayflagadded = reader.GetBoolean(reader.GetOrdinal("displayflagadded"));
                if (reader.IsDBNull(reader.GetOrdinal("flagmessage")) == false)
                {
                    flagmessage = reader.GetString(reader.GetOrdinal("flagmessage"));
                }
                else
                {
                    flagmessage = "";
                }
                
                if (reader.IsDBNull(reader.GetOrdinal("basecurrency")) == false)
                {
                    basecurrency = reader.GetInt32(reader.GetOrdinal("basecurrency"));
                }
                else
                {
                    basecurrency = 0;
                }
                
                allowselfreg = reader.GetBoolean(reader.GetOrdinal("allowselfreg"));
                selfregadvancessignoff = reader.GetBoolean(reader.GetOrdinal("selfregadvancesignoff"));
                selfregbankdetails = reader.GetBoolean(reader.GetOrdinal("selfregbankdetails"));
                selfregcardetails = reader.GetBoolean(reader.GetOrdinal("selfregcardetails"));
                selfregdepcostcode = reader.GetBoolean(reader.GetOrdinal("selfregdepcostcode"));
                selfregempcontact = reader.GetBoolean(reader.GetOrdinal("selfregempcontact"));
                selfregempinfo = reader.GetBoolean(reader.GetOrdinal("selfregempinfo"));
                selfreghomeaddr = reader.GetBoolean(reader.GetOrdinal("selfreghomeaddr"));
                selfregrole = reader.GetBoolean(reader.GetOrdinal("selfregrole"));
                selfregsignoff = reader.GetBoolean(reader.GetOrdinal("selfregsignoff"));
                selfregudf = reader.GetBoolean(reader.GetOrdinal("selfregudf"));
                if (reader.IsDBNull(reader.GetOrdinal("defaultrole")) == true)
                {
                    defaultrole = 0;
                }
                else
                {
                    defaultrole = reader.GetInt32(reader.GetOrdinal("defaultrole"));
                }
                singleclaimcc = reader.GetBoolean(reader.GetOrdinal("singleclaimcc"));
                singleclaimpc = reader.GetBoolean(reader.GetOrdinal("singleclaimpc"));
                displaylimits = reader.GetBoolean(reader.GetOrdinal("displaylimits"));
                if (reader.IsDBNull(reader.GetOrdinal("drilldownreport")) == true)
                {
                    drilldownreport = 0;
                }
                else
                {
                    drilldownreport = reader.GetInt32(reader.GetOrdinal("drilldownreport"));
                }
                blocklicenceexpiry = reader.GetBoolean(reader.GetOrdinal("blocklicenceexpiry"));
                blocktaxexpiry = reader.GetBoolean(reader.GetOrdinal("blocktaxexpiry"));
                blockmotexpiry = reader.GetBoolean(reader.GetOrdinal("blockmotexpiry"));
                blockinsuranceexpiry = reader.GetBoolean(reader.GetOrdinal("blockinsuranceexpiry"));
                delsetup = reader.GetBoolean(reader.GetOrdinal("delsetup"));
                delemployeeadmin = reader.GetBoolean(reader.GetOrdinal("delemployeeadmin"));
                delemployeeaccounts = reader.GetBoolean(reader.GetOrdinal("delemployeeaccounts"));
                delreports = reader.GetBoolean(reader.GetOrdinal("delreports"));
                delreportsreadonly = reader.GetBoolean(reader.GetOrdinal("delreportsreadonly"));
                delcheckandpay = reader.GetBoolean(reader.GetOrdinal("delcheckandpay"));
                delqedesign = reader.GetBoolean(reader.GetOrdinal("delqedesign"));
                delcreditcard = reader.GetBoolean(reader.GetOrdinal("delcreditcard"));
                delpurchasecards = reader.GetBoolean(reader.GetOrdinal("delpurchasecards"));
                delapprovals = reader.GetBoolean(reader.GetOrdinal("delapprovals"));
                delexports = reader.GetBoolean(reader.GetOrdinal("delexports"));
                delauditlog = reader.GetBoolean(reader.GetOrdinal("delauditlog"));
                sendreviewrequest = reader.GetBoolean(reader.GetOrdinal("sendreviewrequest"));
                claimantdeclaration = reader.GetBoolean(reader.GetOrdinal("claimantdeclaration"));
                if (reader.IsDBNull(reader.GetOrdinal("declarationmsg")) == true)
                {
                    declarationmsg = "";
                }
                else
                {
                    declarationmsg = reader.GetString(reader.GetOrdinal("declarationmsg"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("approverdeclarationmsg")) == true)
                {
                    approverdeclarationmsg = "";
                }
                else
                {
                    approverdeclarationmsg = reader.GetString(reader.GetOrdinal("approverdeclarationmsg"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == true)
                {
                    modifiedon = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                {
                    modifiedby = 0;
                }
                else
                {
                    modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                }
                
                blockcashcc = reader.GetBoolean(reader.GetOrdinal("blockcashcc"));
                blockcashpc = reader.GetBoolean(reader.GetOrdinal("blockcashpc"));
                addcompanies = reader.GetBoolean(reader.GetOrdinal("addcompanies"));
                allowmultipledestinations = reader.GetBoolean(reader.GetOrdinal("allowmultipledestinations"));
                usemappoint = reader.GetBoolean(reader.GetOrdinal("usemappoint"));
                usecostcodeongendet = reader.GetBoolean(reader.GetOrdinal("usecostcodeongendet"));
                usedepartmentongendet = reader.GetBoolean(reader.GetOrdinal("usedepartmentongendet"));
                useprojectcodeongendet = reader.GetBoolean(reader.GetOrdinal("useprojectcodeongendet"));
                if (reader.IsDBNull(reader.GetOrdinal("hometooffice")) == true)
                {
                    hometooffice = false;
                }
                else
                {
                    hometooffice = reader.GetBoolean(reader.GetOrdinal("hometooffice"));
                }
                calchometolocation = reader.GetBoolean(reader.GetOrdinal("calchometolocation"));
                showmileagecategoriesforusers = reader.GetBoolean(reader.GetOrdinal("showmileagecategoriesforusers"));
                activatecaronuseradd = reader.GetBoolean(reader.GetOrdinal("activatecaronuseradd"));
                autocalchometolocation = reader.GetBoolean(reader.GetOrdinal("autocalchometolocation"));
                enableAutolog = reader.GetBoolean(reader.GetOrdinal("enableautolog"));
                allowUsersToAddCars = reader.GetBoolean(reader.GetOrdinal("allowuserstoaddcars"));
                mileagecalculationtype = (MileageCalculationType)reader.GetByte(reader.GetOrdinal("mileage_calculation_type"));
                properties = new cGlobalProperties(mileage, server, currencytype, dbversion, standards, numrows, attempts, expiry, plength, length1, length2, pupper, pnumbers, previous, thresholdtype, homecountry, limits, policytype, duplicates, curimportid, compmileage, weekend, usecostcodes, usedepartmentcodes, accountid, ccadmin, singleclaim, usecostcodedesc, usedepartmentdesc, attachreceipts, limitdates, initialdate, limitmonths, flagdate, limitsreceipt, mainadministrator, increaseothers, searchemployees, preapproval, showreviews, showbankdetails, mileageprev, minclaimamount, maxclaimamount, exchangereadonly, tiplimit, useprojectcodes, useprojectcodedesc, recordodometer, odometerday, addlocations, rejecttip, costcodeson, departmentson, projectcodeson, partsubmittal, onlycashcredit, locationsearch, language, currencysymbol, currencydelimiter, limitfrequency, frequencytype, frequencyvalue, overridehome, sourceaddress, editmydetails, autoassignallocation, enterodometeronsubmit, displayflagadded, flagmessage, basecurrency, allowselfreg, selfregempcontact, selfreghomeaddr, selfregempinfo, selfregrole, selfregsignoff, selfregadvancessignoff, selfregdepcostcode, selfregbankdetails, selfregcardetails, selfregudf, defaultrole, singleclaimcc, singleclaimpc, displaylimits, drilldownreport, blocklicenceexpiry, blocktaxexpiry, blockmotexpiry, blockinsuranceexpiry, delsetup, delemployeeadmin, delemployeeaccounts, delreports, delreportsreadonly, delcheckandpay, delqedesign, delcreditcard, delpurchasecards, delapprovals, delexports, delauditlog, sendreviewrequest, claimantdeclaration, declarationmsg, approverdeclarationmsg, blockcashcc, blockcashpc, addcompanies, allowmultipledestinations, usemappoint, usecostcodeongendet, usedepartmentongendet, useprojectcodeongendet, hometooffice, calchometolocation, modifiedon, modifiedby, showmileagecategoriesforusers, activatecaronuseradd, autocalchometolocation, enableAutolog, allowUsersToAddCars, mileagecalculationtype);
            }
            reader.Close();
            Cache.Insert("globalproperties" + accountid, properties, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);

            return properties;
        }

        #endregion
    }

     
	

    

   public enum Action
    {
        Add = 1,
        Edit,
        Delete
    }



    public class wizardStep
    {
        private int nActualStep;
        private int nLogicalStep;
        private string sLabel;
        public wizardStep(int actualstep, int logicalstep, string label)
        {
            nActualStep = actualstep;
            nLogicalStep = logicalstep;
            sLabel = label;
        }

        #region properties
        public int actualstep
        {
            get { return nActualStep; }
        }
        public int logicalstep
        {
            get { return nLogicalStep; }
        }
        public string label
        {
            get { return sLabel; }
        }
        #endregion
    }

    

}
