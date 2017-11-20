using System;
using ExpensesLibrary;
using System.Collections.Generic;
using expenses.Old_App_Code;
using System.Web.Caching;
using SpendManagementLibrary;
namespace expenses
{
	/// <summary>
	/// Summary description for floats.
	/// </summary>
	/// 
	public class cFloats
	{
		int nAccountid = 0;
		string strsql;
		System.Data.DataSet rcdstfloats = new System.Data.DataSet();
		public System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
		System.Collections.SortedList list;
		 
		public cFloats(int accountid)
		{
			nAccountid = accountid;
            
			
			InitialiseData();
		}

		private int accountid
		{
			get {return nAccountid;}
		}
		private void InitialiseData()
		{
            list = (System.Collections.SortedList)Cache["advances" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			
		}


		private System.Collections.SortedList CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Collections.SortedList list = new System.Collections.SortedList();
			System.Data.SqlClient.SqlDataReader reader;
			int floatid, employeeid, currencyid;
			cFloat reqfloat;
			decimal floatamount;
			string name, reason;
			DateTime requiredby;
			bool approved;
			int approver;
			double exchangerate;
			byte stage;
			int issuenum, basecurrency;
			bool rejected, disputed, paid, settled;
			string rejectreason, dispute;
			DateTime datepaid;
            AggregateCacheDependency aggdep = new AggregateCacheDependency();
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
            
            strsql = "select floatid, expenseid, amount from dbo.float_allocations";
            SqlCacheDependency allocationdep = expdata.CreateSQLCacheDependency(strsql, new SortedList<string, object>());
			strsql = "select floatid, employeeid, currencyid, float, name, reason, requiredby, approved, approver, exchangerate, stage, rejected, rejectreason, disputed, dispute, paid, datepaid, issuenum, basecurrency, settled, createdon, createdby, modifiedon, modifiedby from dbo.[floats]";

            SqlCacheDependency dep = expdata.CreateSQLCacheDependency(strsql, new SortedList<string, object>());

            aggdep.Add(new CacheDependency[] { dep, allocationdep });

			reader = expdata.GetReader(strsql);
			while (reader.Read())
			{
				floatid = reader.GetInt32(reader.GetOrdinal("floatid"));
				employeeid = reader.GetInt32(reader.GetOrdinal("employeeid"));
				if (reader.IsDBNull(reader.GetOrdinal("currencyid")) == true)
				{
					currencyid = 0;
				}
				else
				{
					currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
				}
				floatamount = reader.GetDecimal(reader.GetOrdinal("float"));
				name = reader.GetString(reader.GetOrdinal("name"));
				if (reader.IsDBNull(reader.GetOrdinal("reason")) == true)
				{
					reason = "";
				}
				else
				{
					reason = reader.GetString(reader.GetOrdinal("reason"));
				}
				if (reader.IsDBNull(reader.GetOrdinal("requiredby")) == true)
				{
					requiredby = new DateTime(1900,01,01);
				}
				else
				{
					requiredby = (DateTime)reader.GetDateTime(reader.GetOrdinal("requiredby"));
				}
				approved = reader.GetBoolean(reader.GetOrdinal("approved"));
				approver = reader.GetInt32(reader.GetOrdinal("approver"));
				exchangerate = reader.GetDouble(reader.GetOrdinal("exchangerate"));
				stage = reader.GetByte(reader.GetOrdinal("stage"));
				rejected = reader.GetBoolean(reader.GetOrdinal("rejected"));
				disputed = reader.GetBoolean(reader.GetOrdinal("disputed"));
				if (reader.IsDBNull(reader.GetOrdinal("rejectreason")) == false)
				{
					rejectreason = reader.GetString(reader.GetOrdinal("rejectreason"));
				}
				else
				{
					rejectreason = "";
				}
				if (reader.IsDBNull(reader.GetOrdinal("dispute")) == false)
				{
					dispute = reader.GetString(reader.GetOrdinal("dispute"));
				}
				else
				{
					dispute = "";
				}
				paid = reader.GetBoolean(reader.GetOrdinal("paid"));
				if (reader.IsDBNull(reader.GetOrdinal("datepaid")) == false)
				{
					datepaid = reader.GetDateTime(reader.GetOrdinal("datepaid"));
				}
				else
				{
					datepaid = new DateTime(1900,01,01);
				}
				if (reader.IsDBNull(reader.GetOrdinal("issuenum")) == false)
				{
					issuenum = reader.GetInt32(reader.GetOrdinal("issuenum"));
				}
				else
				{
					issuenum = 0;
				}
                if (reader.IsDBNull(reader.GetOrdinal("basecurrency")) == false)
                {
                    basecurrency = reader.GetInt32(reader.GetOrdinal("basecurrency"));
                }
                else
                {
                    basecurrency = 0;
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
                settled = reader.GetBoolean(reader.GetOrdinal("settled"));
				reqfloat = new cFloat(accountid, floatid, employeeid, currencyid, name, reason, requiredby, approved, approver, floatamount, exchangerate, stage, rejected, rejectreason, disputed, dispute, paid, datepaid, issuenum, basecurrency, settled, getFloatAllocations(floatid), getFloatUsed(floatid), createdon, createdby, modifiedon, modifiedby);
				list.Add(floatid, reqfloat);
			}

			reader.Close();
			;

			Cache.Insert("advances" + accountid,list,aggdep,System.Web.Caching.Cache.NoAbsoluteExpiration,System.TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);
			return list;

		}

        private SortedList<int, decimal> getFloatAllocations(int floatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SortedList<int, decimal> allocations = new SortedList<int, decimal>();
            System.Data.SqlClient.SqlDataReader reader;
            int expenseid;
            decimal amount;
            strsql = "select * from float_allocations where floatid = @floatid";
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                expenseid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                amount = reader.GetDecimal(reader.GetOrdinal("amount"));
                allocations.Add(expenseid, amount);
            }
            reader.Close();
            return allocations;
        }
		

		

		public System.Data.DataSet getGrid(bool approver, int employeeid, bool active)
		{
			object[] values;
			bool display;
			System.Data.DataSet ds = new System.Data.DataSet();
			int i;
			cFloat reqfloat;
			System.Data.DataTable tbl = new System.Data.DataTable();
			tbl.Columns.Add("floatid",System.Type.GetType("System.Int32"));
			tbl.Columns.Add("employeeid",System.Type.GetType("System.Int32"));
			tbl.Columns.Add("name",System.Type.GetType("System.String"));
			tbl.Columns.Add("reason",System.Type.GetType("System.String"));
			tbl.Columns.Add("originalcurrency",System.Type.GetType("System.Int32"));
			tbl.Columns.Add("exchangerate",System.Type.GetType("System.Double"));
			tbl.Columns.Add("Total Prior To Convert",System.Type.GetType("System.Decimal"));
			tbl.Columns.Add("floatamount",System.Type.GetType("System.Decimal"));
			tbl.Columns.Add("floatused",System.Type.GetType("System.Decimal"));
			tbl.Columns.Add("floatavailable",System.Type.GetType("System.Decimal"));
			tbl.Columns.Add("requiredby",System.Type.GetType("System.DateTime"));
			tbl.Columns.Add("approver",System.Type.GetType("System.Int32"));
			tbl.Columns.Add("approved",System.Type.GetType("System.Boolean"));
			tbl.Columns.Add("stage",System.Type.GetType("System.Byte"));
			tbl.Columns.Add("rejected",System.Type.GetType("System.Boolean"));
			tbl.Columns.Add("rejectreason",System.Type.GetType("System.String"));
			tbl.Columns.Add("disputed",System.Type.GetType("System.Boolean"));
			tbl.Columns.Add("dispute",System.Type.GetType("System.String"));
			tbl.Columns.Add("paid",System.Type.GetType("System.Boolean"));
			tbl.Columns.Add("issuenum",System.Type.GetType("System.Int32"));
            tbl.Columns.Add("basecurrency", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("settled", typeof(System.Boolean));
			for (i = 0; i < list.Count; i++)
			{
				display = false;
				reqfloat = (cFloat)list.GetByIndex(i);
				
				if (approver == true)
				{
					if (reqfloat.approver == employeeid && reqfloat.paid == false && active == false)
					{
						
							if (reqfloat.rejected == false || (reqfloat.rejected == true && reqfloat.disputed == true))
							{
								display = true;
							}
						
						
					}
					else if (reqfloat.approver == employeeid && reqfloat.paid == true && active == true && !reqfloat.settled)
					{
						
							display = true;
						
					}
				}
				else
				{
					if (reqfloat.employeeid == employeeid && !reqfloat.settled)
					{
						display = true;
					}
				}
				if (display == true)
				{
					
						values = new object[22];
						values[0] = reqfloat.floatid;
						values[1] = reqfloat.employeeid;
						values[2] = reqfloat.name;
						values[3] = reqfloat.reason;
						values[4] = reqfloat.currencyid;
						values[5] = Math.Round(reqfloat.exchangerate,4);
					if (reqfloat.exchangerate > 0)
					{
						values[6] = Math.Round(reqfloat.floatamount / Math.Round((decimal)reqfloat.exchangerate,4),2);
					}
						values[7] = reqfloat.floatamount;
						values[8] = reqfloat.floatused;
						values[9] = reqfloat.floatamount - reqfloat.floatused;
						values[10] = reqfloat.requiredby;
						values[11] = reqfloat.approver;
						values[12] = reqfloat.approved;
						values[13] = reqfloat.stage;
						values[14] = reqfloat.rejected;
						values[15] = reqfloat.rejectreason;
						values[16] = reqfloat.disputed;
						values[17] = reqfloat.dispute;
						values[18] = reqfloat.paid;
						values[19] = reqfloat.issuenum;
                        values[20] = reqfloat.basecurrency;
                        values[21] = reqfloat.settled;
					
					tbl.Rows.Add(values);
				}
			}

			ds.Tables.Add(tbl);
			return ds;
		}

        public System.Data.DataSet getSettledGrid(bool approver, int employeeid, bool active)
        {
            object[] values;
            bool display;
            System.Data.DataSet ds = new System.Data.DataSet();
            int i;
            cFloat reqfloat;
            System.Data.DataTable tbl = new System.Data.DataTable();
            tbl.Columns.Add("floatid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("employeeid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("name", System.Type.GetType("System.String"));
            tbl.Columns.Add("reason", System.Type.GetType("System.String"));
            tbl.Columns.Add("originalcurrency", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("exchangerate", System.Type.GetType("System.Double"));
            tbl.Columns.Add("Total Prior To Convert", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("floatamount", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("floatused", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("floatavailable", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("requiredby", System.Type.GetType("System.DateTime"));
            tbl.Columns.Add("approver", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("approved", System.Type.GetType("System.Boolean"));
            tbl.Columns.Add("stage", System.Type.GetType("System.Byte"));
            tbl.Columns.Add("rejected", System.Type.GetType("System.Boolean"));
            tbl.Columns.Add("rejectreason", System.Type.GetType("System.String"));
            tbl.Columns.Add("disputed", System.Type.GetType("System.Boolean"));
            tbl.Columns.Add("dispute", System.Type.GetType("System.String"));
            tbl.Columns.Add("paid", System.Type.GetType("System.Boolean"));
            tbl.Columns.Add("issuenum", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("basecurrency", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("settled", typeof(System.Boolean));
            for (i = 0; i < list.Count; i++)
            {
                display = false;
                reqfloat = (cFloat)list.GetByIndex(i);

                if (approver == true)
                {
                    if (reqfloat.approver == employeeid && reqfloat.paid == true && active == true && reqfloat.settled)
                    {

                        display = true;

                    }
                }
                
                if (display == true)
                {

                    values = new object[22];
                    values[0] = reqfloat.floatid;
                    values[1] = reqfloat.employeeid;
                    values[2] = reqfloat.name;
                    values[3] = reqfloat.reason;
                    values[4] = reqfloat.currencyid;
                    values[5] = Math.Round(reqfloat.exchangerate, 4);
                    if (reqfloat.exchangerate > 0)
                    {
                        values[6] = Math.Round(reqfloat.floatamount /  Math.Round((decimal)reqfloat.exchangerate,4), 2);
                    }
                    values[7] = reqfloat.floatamount;
                    values[8] = reqfloat.floatused;
                    values[9] = reqfloat.floatamount - reqfloat.floatused;
                    values[10] = reqfloat.requiredby;
                    values[11] = reqfloat.approver;
                    values[12] = reqfloat.approved;
                    values[13] = reqfloat.stage;
                    values[14] = reqfloat.rejected;
                    values[15] = reqfloat.rejectreason;
                    values[16] = reqfloat.disputed;
                    values[17] = reqfloat.dispute;
                    values[18] = reqfloat.paid;
                    values[19] = reqfloat.issuenum;
                    values[20] = reqfloat.basecurrency;
                    values[21] = reqfloat.settled;

                    tbl.Rows.Add(values);
                }
            }

            ds.Tables.Add(tbl);
            return ds;
        }
		

		

		

		
		public void deleteFloat(int floatid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@floatid",floatid);
			strsql = "delete from floats where floatid = @floatid";
			expdata.ExecuteSQL(strsql);
			InitialiseData();
		}

		public cFloat GetFloatById(int floatid)
		{
			return (cFloat)list[floatid];
		}

		public cFloat getFloatByName(int employeeid, string name)
		{
			int i;
			cFloat reqfloat;

			for (i = 0; i < list.Count; i++)
			{
				reqfloat = (cFloat)list.GetByIndex(i);
				if (reqfloat.employeeid == employeeid && reqfloat.name == name)
				{
					return reqfloat;
				}
			}
			return null;
		}
		public int getFloatCount(int employeeid, int currencyid)
		{
			int i;
			cFloat reqfloat;
			int count = 0;
			
			for (i = 0; i < list.Count; i++)
			{
				reqfloat = (cFloat)list.GetByIndex(i);
				if (reqfloat.employeeid == employeeid  && reqfloat.approved == true && reqfloat.floatavailable > 0)
				{
					count++;
				}
			}	
			
			return count;
		}

		public byte updateFloat(int employeeid, int floatid, string name, string reason, decimal amount, int currencyid, string requiredby)
		{

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			decimal exchangerate = 0;
			decimal newamount = 0;
			
			if (currencyid == 0)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@currencyid",DBNull.Value);
			}
			else
			{
				expdata.sqlexecute.Parameters.AddWithValue("@currencyid",currencyid);

			
				newamount = convertTotals(employeeid, currencyid,amount);
				exchangerate = newamount / amount;
				if (exchangerate == 0)
				{
					return 1;
				}
				amount = newamount;
				
			}
            amount = Math.Round(amount, 2);

			expdata.sqlexecute.Parameters.AddWithValue("@amount",amount);
			expdata.sqlexecute.Parameters.AddWithValue("@exchangerate",exchangerate);
			expdata.sqlexecute.Parameters.AddWithValue("@name",name);
			expdata.sqlexecute.Parameters.AddWithValue("@reason",reason);
			expdata.sqlexecute.Parameters.AddWithValue("@floatid",floatid);
			
			
			if (requiredby == "")
			{
				expdata.sqlexecute.Parameters.AddWithValue("@requiredby",DBNull.Value);
			}
			else
			{
				DateTime dtrequiredby;
				dtrequiredby = DateTime.Parse(requiredby);
				expdata.sqlexecute.Parameters.AddWithValue("@requiredby",dtrequiredby.Year + "/" + dtrequiredby.Month + "/" + dtrequiredby.Day);
				
			}
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", employeeid);

			strsql = "update [floats] set disputed = 1, [name] = @name, exchangerate = @exchangerate, [float] = @amount, requiredby = @requiredby, reason = @reason, modifiedon = @modifiedon, modifiedby = @modifiedby where floatid = @floatid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			return 0;
		}
		public byte requestFloat(int employeeid, string name, string reason, decimal amount, int currencyid, string requiredby, int basecurrency)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int floatid;
			cFloat reqfloat;
			decimal exchangerate = 0;
			decimal newamount = 0;
            DateTime createdon = DateTime.Now.ToUniversalTime();
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);

            
            cEmployees clsemployees = new cEmployees(accountid);
            cMisc clsmisc = new cMisc(accountid);

            
           

            
           
			if (currencyid == basecurrency)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@currencyid",basecurrency);
			}
			else
			{
				expdata.sqlexecute.Parameters.AddWithValue("@currencyid",currencyid);

				//convert to sterling
				
				newamount = convertTotals(employeeid, currencyid,amount);
				exchangerate = newamount / amount;
				if (exchangerate == 0)
				{
					return 1;
				}
               
				amount = newamount;
				
			}

            amount = Math.Round(amount, 2);
			expdata.sqlexecute.Parameters.AddWithValue("@amount",amount);
			expdata.sqlexecute.Parameters.AddWithValue("@exchangerate",exchangerate);
			expdata.sqlexecute.Parameters.AddWithValue("@name",name);
			expdata.sqlexecute.Parameters.AddWithValue("@reason",reason);

            DateTime dtrequiredby;
			if (requiredby == "")
			{
				expdata.sqlexecute.Parameters.AddWithValue("@requiredby",DBNull.Value);
                dtrequiredby = new DateTime(1900, 01, 01);
			}
			else
			{
				
				dtrequiredby = DateTime.Parse(requiredby);
				expdata.sqlexecute.Parameters.AddWithValue("@requiredby",dtrequiredby.Year + "/" + dtrequiredby.Month + "/" + dtrequiredby.Day);
				
			}
            expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", basecurrency);
			expdata.sqlexecute.Parameters.AddWithValue("@identity",System.Data.SqlDbType.Int);
			expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", employeeid);

			strsql = "insert into [floats] (employeeid, currencyid, [float], [name], reason, requiredby, exchangerate, basecurrency, createdon, createdby) " +
				"values (@employeeid, @currencyid, @amount, @name, @reason, @requiredby, @exchangerate,  @basecurrency, @createdon, @createdby);set @identity = @@identity";
			expdata.ExecuteSQL(strsql);
			floatid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            reqfloat = new cFloat(accountid, floatid, employeeid, currencyid, name, reason, dtrequiredby, false, 0, amount, Convert.ToDouble(exchangerate), 0, false, "", false, "", false, new DateTime(1900, 01, 01), 0, basecurrency, false, new SortedList<int, decimal>(), 0, createdon, employeeid, new DateTime(1900, 01, 01), 0);
            

            //reqfloat = GetFloatById(floatid);
			SendClaimToNextStage(reqfloat, true, 0, employeeid);
			
			return 0;
		}

        private decimal convertTotals(int employeeid, int currencyid, decimal floatamount)
        {
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            double exchangerate = 0;
            cCurrencies clscurrencies = new cCurrencies(accountid);


            cCurrency reqcurrency = clscurrencies.getCurrencyById(currencyid);

            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);

            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
            

            int basecurrency;

            if (reqemp.primarycurrency != 0)
            {
                basecurrency = reqemp.primarycurrency;
            }
            else
            {
                basecurrency = clsproperties.basecurrency;
            }

            reqcurrency = clscurrencies.getCurrencyById(basecurrency);
            switch (clsproperties.currencytype)
            {
                case 1: //static
                    cStaticCurrency reqstandard = (cStaticCurrency)reqcurrency;
                    exchangerate = reqstandard.getExchangeRate(currencyid);
                    break;
                case 2: //monthly
                    cMonthlyCurrency reqmonthly = (cMonthlyCurrency)reqcurrency;
                    exchangerate = reqmonthly.getExchangeRate((byte)DateTime.Today.Month, DateTime.Today.Year,currencyid);
                    break;
                case 3: //range
                    cRangeCurrency reqrange = (cRangeCurrency)reqcurrency;
                    exchangerate = reqrange.getExchangeRate(DateTime.Today,currencyid);
                    break;

            }

            if (exchangerate == 0)
            {
                return 0;
            }
            decimal convertedtotal;
            decimal total;



            
			
			switch (clsproperties.currencytype)
			{
				case 1: //static
					cStaticCurrency reqstandard = (cStaticCurrency)reqcurrency;
					exchangerate = reqstandard.getExchangeRate(currencyid);
					break;
				case 2: //monthly
					cMonthlyCurrency reqmonthly = (cMonthlyCurrency)reqcurrency;
					exchangerate = reqmonthly.getExchangeRate((byte)DateTime.Today.Month,DateTime.Today.Year, currencyid);
					break;
				case 3: //range
					cRangeCurrency reqrange = (cRangeCurrency)reqcurrency;
					exchangerate = reqrange.getExchangeRate(DateTime.Today, currencyid);
					break;
					
			}

            convertedtotal = floatamount;

            total = convertedtotal * (1 / (decimal)exchangerate);

            return total;
        }

        public List<System.Web.UI.WebControls.ListItem> CreateDropDown(int employeeid, int currencyid, int floatid)
        {
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            cCurrencies clscurrencies = new cCurrencies(accountid);
			cCurrency reqcur;
            List<System.Web.UI.WebControls.ListItem> items = new List<System.Web.UI.WebControls.ListItem>();
            decimal floatavailable;

            items.Add(new System.Web.UI.WebControls.ListItem("", "0"));
            foreach (cFloat advance in list.Values)
            {
                if (advance.employeeid == employeeid && advance.paid == true && advance.currencyid == currencyid)
                {
                    floatavailable = advance.floatamount - advance.floatused;

                    if (floatavailable > 0 || advance.floatid == floatid)
                    {
                        reqcur = clscurrencies.getCurrencyById(advance.currencyid);
                        if (reqcur != null)
                        {
                            items.Add(new System.Web.UI.WebControls.ListItem(advance.name + " (" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label + ")", advance.floatid.ToString()));
                        }
                    }
                }
               
            }
            return items;
        }
		public void getStringFloatDropdown(ref System.Text.StringBuilder output, int employeeid, int currencyid, int floatid)
		{
			cFloat reqfloat;
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
			cCurrencies clscurrencies = new cCurrencies(accountid);
			cCurrency reqcur;
			int i;
			decimal floatavailable;
			
			output.Append("<option value=\"0\"></option>");
			for (i = 0; i < list.Count; i++)
			{
				reqfloat = (cFloat)list.GetByIndex(i);
				if (reqfloat.employeeid == employeeid && reqfloat.paid == true)
				{
					floatavailable = reqfloat.floatamount - reqfloat.floatused;

					if (floatavailable > 0)
					{
						output.Append("<option value=\"" + reqfloat.floatid + "\"");
						if (reqfloat.floatid == floatid)
						{
							output.Append(" selected");
						}
						output.Append(">");
						output.Append(reqfloat.name);
						if (reqfloat.currencyid == 0)
						{
							output.Append(" (GBP)");
						}
						else
						{
							reqcur = clscurrencies.getCurrencyById(reqfloat.currencyid);
							output.Append(" (" + clsglobalcurrencies.getGlobalCurrencyById(reqcur.globalcurrencyid).label + ")");
						}
						output.Append("</option>");
					}
				}

				
			}
			

		}

		
		public decimal getAvailableFloatAmount(int employeeid, int currencyid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.SqlClient.SqlDataReader reader;
			decimal totalavailable = 0;
			decimal floatused = 0;
			strsql = "select sum([float]) from floats where approved = 1 and employeeid = @employeeid and currencyid = @currencyid";
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid",employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
			reader = expdata.GetReader(strsql);
			while (reader.Read())
			{
				if (reader.IsDBNull(0) == false)
				{
					totalavailable = reader.GetDecimal(0);
				}
			}

			reader.Close();

			strsql = "select sum(amount) from float_allocations where floatid in (select floatid from floats where employeeid = @employeeid and currencyid = @currencyid)";
			reader = expdata.GetReader(strsql);
			while (reader.Read())
			{
				if (reader.IsDBNull(0) == false)
				{
					floatused = reader.GetDecimal(0);
				}
			}
			reader.Close();

			totalavailable -= floatused;
			return totalavailable;

		}

        public void changeAmount(int floatid, decimal newAmount)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            decimal amount;
            cFloat reqfloat = GetFloatById(floatid);
            if (reqfloat.currencyid == 0)
            {
                amount = newAmount;
            }
            else
            {
                if (reqfloat.exchangerate != 0)
                {
                    amount = newAmount * (decimal)reqfloat.exchangerate;
                }
                else
                {
                    amount = newAmount;
                }
            }

            amount = Math.Round(amount, 2);
            strsql = "update [floats] set [float] = @amount, modifiedon = @modifiedon, modifiedby = @modifiedby where floatid = @floatid";
            expdata.sqlexecute.Parameters.AddWithValue("@amount", amount);
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", reqfloat.employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            
        }
        private decimal getFloatUsed(int floatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            decimal floatused = 0;
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select sum(amount) from float_allocations where floatid = " + floatid;
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    floatused = reader.GetDecimal(0);
                }
            }
            reader.Close();
            ;
            return floatused;
        }

        public void topUpAdvance(int floatid, decimal amount)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cFloat reqfloat = GetFloatById(floatid);
            string strsql;
            decimal newtotal = reqfloat.floatamount + amount;


            strsql = "update [floats] set [float] = @newtotal, modifiedon = @modifiedon, modifiedby = @modifiedby where floatid = @floatid";
            expdata.sqlexecute.Parameters.AddWithValue("@newtotal", newtotal);
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", reqfloat.employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

        }

        private int getNextCheckerId(cFloat reqfloat, byte signofftype, int relid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int nextcheckerid = 0;

            switch (signofftype)
            {
                case 1: //costcode
                    expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
                    strsql = "select employeeid from budgetholders where budgetholderid = @relid";
                    nextcheckerid = expdata.getcount(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                    break;
                case 2:
                    nextcheckerid = relid;
                    break;
                case 3:
                    nextcheckerid = 0;
                    break;
                case 4:
                    cEmployees clsemployees = new cEmployees(accountid);
                    cEmployee reqemp = clsemployees.GetEmployeeById(reqfloat.employeeid);
                    nextcheckerid = reqemp.linemanager;
                    break;
            }

            return nextcheckerid;


        }
        public int SendClaimToNextStage(cFloat reqfloat, bool submitting, int authoriser, int employeeid)
        {
            //return codes
            //1 = user not assigned to a group
            //2 = reached last stage, claim now needs paying

            cStage reqstage;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            expdata.sqlexecute.Parameters.Clear();
            System.Data.DataSet rcdststages = new System.Data.DataSet();
            bool includestage = false;
            bool isOnHoliday = false;
            int stage = reqfloat.stage - 1;
            byte signofftype = 0;
            int onholiday = 0;
            int relid = 0;
            StageInclusionType include = StageInclusionType.None;
            int notify = 0;
            int[] nextcheckerid;
            decimal maxamount = 0;
            decimal claimamount = 0;
            string sql = "";

            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            CurrentUser user = cMisc.getCurrentUser(appinfo.User.Identity.Name);
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp;
            reqemp = clsemployees.GetEmployeeById(user.employeeid);
            cEmails clsemails = new cEmails(reqemp.accountid);
            cGroups clsgroups = new cGroups(reqemp.accountid);
            cEmployee claimemp = clsemployees.GetEmployeeById(employeeid);
            cGroup reqgroup = clsgroups.GetGroupById(claimemp.advancegroup);




            //get the stages the claim has to go through
            if (reqgroup == null)
            {
                return 3;
            }
            if (reqgroup.stages.Count == 0)
            {
                return 3;
            }



            //increment stage

            if (reqfloat.stage >= (reqgroup.stages.Count) && submitting == false) //reached last stage, now needs payin
            {
                approveFloat(reqfloat.floatid);
                return 2;
            }
            //resetApproval();
            //expdata.sqlexecute.Parameters.AddWithValue("@claimid",claimid);
            //expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            //			if (nStage == 0)
            //			{
            //				nStage = 1;
            //			}
            for (stage = reqfloat.stage; stage < reqgroup.stages.Count; stage++)
            {
                reqstage = (cStage)reqgroup.stages.Values[stage];
                includestage = true;
                signofftype = reqstage.signofftype;
                relid = reqstage.relid;
                //is the stage on holiday?
                isOnHoliday = userOnHoliday(signofftype, relid);
                if (isOnHoliday == true)
                {
                    onholiday = reqstage.onholiday;
                    switch (onholiday)
                    {
                        case 1: //take no action - wait for user to get back of their hols
                            includestage = true;
                            break;
                        case 2: //skip stage

                            includestage = false;
                            break;
                        case 3: //assign to someoneelse
                            signofftype = (byte)reqstage.holidaytype;
                            relid = reqstage.relid;
                            includestage = true;
                            break;

                    }
                }

                //got the stage we need. 
                if (includestage == true)
                {
                    notify = reqstage.notify;
                    include = reqstage.include;

                    switch (include)
                    {
                        case StageInclusionType.ClaimTotalExceeds: //claim exceeded specified amount
                            maxamount = reqstage.amount;
                            if (claimamount < maxamount)
                            {
                                includestage = false;
                            }
                            break;

                    }


                }

                //check to see if limit existed. if this was exceeded includestage will have been changed to false
                if (includestage == true)
                {
                    nextcheckerid = new int[1];
                    nextcheckerid[0] = getNextCheckerId(reqfloat, signofftype, relid);
                    if (nextcheckerid[0] == 0)
                    {
                        return 1;
                    }

                    reqfloat.stage++;
                    if (notify == 1) //just notify of claim by email
                    {
                        clsemails.advance = reqfloat;
                        clsemails.sendMessage(12, employeeid, nextcheckerid);
                    }
                    else
                    {

                        sql = "update [floats] set stage = " + (stage + 1) + ", ";


                        if (signofftype == 3)
                        {
                            sql = sql + "approver = null";
                            sql = sql + ", teamid = " + relid;

                        }
                        else
                        {
                            sql = sql + "approver = " + nextcheckerid[0];
                            reqfloat.approver = nextcheckerid[0];
                        }
                        sql = sql + ", modifiedon = @modifiedon, modifiedby = @modifiedby where floatid = @floatid";


                        expdata.sqlexecute.Parameters.AddWithValue("@floatid", reqfloat.floatid);
                        expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
                        expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
                        expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", employeeid);

                        expdata.ExecuteSQL(sql);
                        clsemails.advance = reqfloat;
                        clsemails.sendMessage(9, employeeid, nextcheckerid);
                        int[] arremp = new int[1];
                        arremp[0] = employeeid;
                        //						if (reqstage.claimantmail == true)
                        //						{
                        //							clsemails.sendMessage(11,nextcheckerid[0],arremp,floatid,new int[0]);
                        //						}
                        break;
                    }

                }

            }
            expdata.sqlexecute.Parameters.Clear();
            return 0;
        }

        private bool userOnHoliday(byte signofftype, int relid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            bool userOnHoliday = false;
            int count = 0;
            DateTime today = DateTime.Today;
            expdata.sqlexecute.Parameters.AddWithValue("@relid", relid);
            switch (signofftype)
            {
                case 1: //cost code
                    strsql = "select count(*) from holidays inner join budgetholders on budgetholders.employeeid = holidays.employeeid where budgetholders.employeeid = @relid and ('" + today.Year + "/" + today.Month + "/" + today.Day + "' between startdate and enddate)";
                    count = expdata.getcount(strsql);
                    if (count != 0)
                    {
                        userOnHoliday = true;
                    }
                    break;
                case 2: //employee
                    strsql = "select count(*) as holidaycount from holidays where employeeid = @relid and ('" + today.Year + "/" + today.Month + "/" + today.Day + "' between startdate and enddate)";
                    count = expdata.getcount(strsql);
                    if (count != 0)
                    {
                        userOnHoliday = true;
                    }
                    break;
            }

            expdata.sqlexecute.Parameters.Clear();
            return userOnHoliday;

        }
        private void approveFloat(int floatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            strsql = "update [floats] set approved = 1, modifiedon = @modifiedon, modifiedby = @modifiedby where floatid = @floatid";

            cFloat reqfloat = GetFloatById(floatid);
                reqfloat.approveAdvance();
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", reqfloat.employeeid);

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            
        }

        public void addAllocation(int floatid, int expenseid, decimal allocation)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            strsql = "insert into float_allocations (floatid, expenseid, [amount]) " +
                "values (@floatid, @expenseid, @amount)";
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@amount", allocation);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            
        }

        public void settleAdvance(int floatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            strsql = "update [floats] set settled = 1, modifiedon = @modifiedon, modifiedby = @modifiedby where floatid = @floatid";

            cFloat reqfloat = GetFloatById(floatid);
            reqfloat.settleAdvance();

            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", reqfloat.employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            
        }

        public void updateAllocation(int floatid, int expenseid, decimal allocation)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            strsql = "update float_allocations set [amount] = @amount where floatid = @floatid and expenseid = @expenseid";

            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@amount", allocation);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            
        }

        public void payAdvance(int accountid, int floatid)
        {
            cFloat reqfloat = GetFloatById(floatid);
            int issuenum = 0;
            System.Data.SqlClient.SqlDataReader reader;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int[] recipient = new int[1];

            strsql = "select max(issuenum) from [floats]";

            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    issuenum = reader.GetInt32(0);
                }
            }
            reader.Close();
            ;
            issuenum++;
            strsql = "update [floats] set paid = 1, datepaid = @datepaid, issuenum = @issuenum, modifiedon = @modifiedon, modifiedby = @modifiedby where floatid = @floatid";

            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@datepaid", DateTime.Now);
            expdata.sqlexecute.Parameters.AddWithValue("@issuenum", issuenum);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", reqfloat.employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            reqfloat.payAdvance();

            cEmails clsemails = new cEmails(accountid);

            recipient[0] = reqfloat.employeeid;
            clsemails.advance = reqfloat;
            clsemails.sendMessage(16, reqfloat.approver, recipient);



        }

        public void rejectAdvance(int accountid, string reason, int floatid)
        {
            cFloat reqfloat = GetFloatById(floatid);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int[] recipient = new int[1];
            strsql = "update [floats] set rejected = 1, rejectreason = @rejectreason, modifiedon = @modifiedon, modifiedby = @modifiedby where floatid = @floatid";
            expdata.sqlexecute.Parameters.AddWithValue("@rejectreason", reason);
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", reqfloat.employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            

            cEmails clsemails = new cEmails(accountid);

            recipient[0] = reqfloat.employeeid;
            clsemails.advance = reqfloat;
            clsemails.sendMessage(14, reqfloat.approver, recipient);
        }

        public void disputeAdvance(int accountid, string reason, int floatid)
        {
            cFloat reqfloat = GetFloatById(floatid);
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            int[] recipient = new int[1];
            strsql = "update [floats] set disputed = 1, dispute = @dispute, modifiedon = @modifiedon, modifiedby = @modifiedby where floatid = @floatid";
            expdata.sqlexecute.Parameters.AddWithValue("@dispute", reason);
            expdata.sqlexecute.Parameters.AddWithValue("@floatid", floatid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", reqfloat.employeeid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
           
            cEmails clsemails = new cEmails(accountid);

            recipient[0] = reqfloat.approver;
            clsemails.advance = reqfloat;
            clsemails.sendMessage(15, reqfloat.employeeid, recipient);
        }

        public void deleteAllocation(int expenseid, int floatid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            decimal amount = 0;
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select [amount] from float_allocations where expenseid = @expenseid";
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                {
                    amount = reader.GetDecimal(0);
                }
            }
            reader.Close();
            ;

            if (amount != 0)
            {
                
                strsql = "delete from float_allocations where expenseid = @expenseid";
                expdata.ExecuteSQL(strsql);
            }
            expdata.sqlexecute.Parameters.Clear();
            

        }

        public sFloatInfo getModifiedFloatInfo(DateTime date)
        {
            sFloatInfo floatinfo = new sFloatInfo();
            Dictionary<int, cFloat> lstfloats = new Dictionary<int,cFloat>();
            List<int> lstfloatids = new List<int>();

            foreach (cFloat val in list.Values)
            {
                lstfloatids.Add(val.floatid);

                if (val.createdon > date || val.modifiedon > date)
                {
                    lstfloats.Add(val.floatid, val);
                }
            }

            floatinfo.lstfloats = lstfloats;
            floatinfo.lstfloatids = lstfloatids;

            return floatinfo;
        }
	}
	
	


}
