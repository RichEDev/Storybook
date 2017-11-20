using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using Infragistics.WebUI.UltraWebGrid;
using SpendManagementLibrary;
namespace expenses
{
	/// <summary>
	/// Summary description for subcats.
	/// </summary>
	public class cSubcats
	{

		
		
		int accountid = 0;
		SortedList<int,cSubcat> list;
		System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
		public cSubcats(int nAccountid)
		{
			accountid = nAccountid;
            InitialiseData();
		}

		
		private void InitialiseData()
		{
            list = (SortedList<int,cSubcat>)Cache["subcats" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			

		}

		private SortedList<int,cSubcat> CacheList()
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            SortedList<int, cSubcat> list = new SortedList<int, cSubcat>();
			cSubcat reqsubcat;
            cCategories clscategories = new cCategories(accountid);
            Dictionary<int, object> userdefined;
            System.Data.SqlClient.SqlDataReader reader;
            int categoryid, subcatid;
            string subcat, description;
            bool vatapp, vatreceipt, mileageapp, othersapp, staffapp;
            double vatamount;
            bool tipapp, pmilesapp, bmilesapp, allowance;
            decimal allowanceamount, vatlimitwithout, vatlimitwith;
            string accountcode, alternateaccountcode;
            bool attendeesapp, addasnet;
            int pdcatid;
            byte vatpercent;
            bool eventinhomeapp, receiptapp, noroomsapp;
            bool splitpersonal, splitremote, reasonapp, otherdetailsapp;
            int personalid, remoteid;
			CalculationType calculation;
			
            cNewUserDefined clsuserdefined = null;
			bool passengersapp, nopassengersapp;
			string comment, shortsubcat;
			bool splitentertainment;
			int entertainmentid;
			bool reimbursable, nonightsapp, attendeesmand;
			bool nodirectorsapp, hotelapp, hotelmand, vatnumberapp, vatnumbermand;
            bool nopersonalguestsapp, noremoteworkersapp;
            bool fromapp, toapp, companyapp;


            SortedList<int, List<cCountrySubcat>> lstCountries = getCountries();
            List<cCountrySubcat> countries;

            SortedList<int, List<int>> lstAllowances = getAllowances();
            List<int> allowances;

            SortedList<int, List<int>> lstUDFs = getAssociatedUDFs();
            List<int> udfs;

            SortedList<int, List<int>> lstSplitsubcats = getSubcatSplit();
            List<int> split;
			strsql = "select  subcatid, categoryid, description, subcat, vatapp, vatamount, mileageapp, staffapp, othersapp, vatreceipt, tipapp, pmilesapp, bmilesapp, speedoapp, blitresapp, plitresapp, allowance, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, vatpercent, eventinhomeapp, receiptapp, calculation, passengersapp, nopassengersapp, comment, splitentertainment, entertainmentid, reimbursable, nonightsapp, attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumberapp, vatnumbermand, nopersonalguestsapp, noremoteworkersapp, alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, vatlimitwithout, vatlimitwith, reasonapp, otherdetailsapp, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, shortsubcat, fromapp, toapp, companyapp from dbo.subcats order by subcat";
            DateTime createdon, modifiedon;
            int createdby, modifiedby;

            SortedList<int, List<cSubcatVatRate>> lstrates = getVatRates();
            List<cSubcatVatRate> rates;
			
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                if (clsuserdefined == null)
                {
                    clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
                }
				subcatid = reader.GetInt32(reader.GetOrdinal("subcatid"));
				categoryid = reader.GetInt32(reader.GetOrdinal("categoryid"));
				subcat = reader.GetString(reader.GetOrdinal("subcat"));
				if (reader.IsDBNull(reader.GetOrdinal("description")) == false)
				{
					description = reader.GetString(reader.GetOrdinal("description"));
				}
				else
				{
					description = "";
				}
				
				mileageapp = reader.GetBoolean(reader.GetOrdinal("mileageapp"));
				staffapp = reader.GetBoolean(reader.GetOrdinal("staffapp"));
				othersapp = reader.GetBoolean(reader.GetOrdinal("othersapp"));
				tipapp = reader.GetBoolean(reader.GetOrdinal("tipapp"));
				pmilesapp = reader.GetBoolean(reader.GetOrdinal("pmilesapp"));
				bmilesapp = reader.GetBoolean(reader.GetOrdinal("bmilesapp"));
				allowance = reader.GetBoolean(reader.GetOrdinal("allowance"));
				if (reader.IsDBNull(reader.GetOrdinal("allowanceamount")) == false)
				{
					allowanceamount = reader.GetDecimal(reader.GetOrdinal("allowanceamount"));
				}
				else
				{
					allowanceamount = 0;
				}
				if (reader.IsDBNull(reader.GetOrdinal("accountcode")) == false)
				{
					accountcode = reader.GetString(reader.GetOrdinal("accountcode"));
				}
				else
				{
					accountcode = "";
				}
				attendeesapp = reader.GetBoolean(reader.GetOrdinal("attendeesapp"));
				addasnet = reader.GetBoolean(reader.GetOrdinal("addasnet"));
				if (reader.IsDBNull(reader.GetOrdinal("pdcatid")) == false)
				{
					pdcatid = reader.GetInt32(reader.GetOrdinal("pdcatid"));
				}
				else
				{
					pdcatid = 0;
				}
				
				eventinhomeapp = reader.GetBoolean(reader.GetOrdinal("eventinhomeapp"));
				receiptapp = reader.GetBoolean(reader.GetOrdinal("receiptapp"));
				calculation = (CalculationType)reader.GetByte(reader.GetOrdinal("calculation"));
				passengersapp = reader.GetBoolean(reader.GetOrdinal("passengersapp"));
				nopassengersapp = reader.GetBoolean(reader.GetOrdinal("nopassengersapp"));
				if (reader.IsDBNull(reader.GetOrdinal("comment")) == false)
				{
					comment = reader.GetString(reader.GetOrdinal("comment"));
				}
				else
				{
					comment = "";
				}
				splitentertainment = reader.GetBoolean(reader.GetOrdinal("splitentertainment"));
				if (reader.IsDBNull(reader.GetOrdinal("entertainmentid")) == false)
				{
					entertainmentid = reader.GetInt32(reader.GetOrdinal("entertainmentid"));
				}
				else
				{
					entertainmentid = 0;
				}
				reimbursable = reader.GetBoolean(reader.GetOrdinal("reimbursable"));
				nonightsapp = reader.GetBoolean(reader.GetOrdinal("nonightsapp"));
				attendeesmand = reader.GetBoolean(reader.GetOrdinal("attendeesmand"));
				nodirectorsapp = reader.GetBoolean(reader.GetOrdinal("nodirectorsapp"));
				hotelapp = reader.GetBoolean(reader.GetOrdinal("hotelapp"));
				noroomsapp = reader.GetBoolean(reader.GetOrdinal("noroomsapp"));
				hotelmand = reader.GetBoolean(reader.GetOrdinal("hotelmand"));
				vatnumberapp = reader.GetBoolean(reader.GetOrdinal("vatnumberapp"));
				vatnumbermand = reader.GetBoolean(reader.GetOrdinal("vatnumbermand"));
                nopersonalguestsapp = reader.GetBoolean(reader.GetOrdinal("nopersonalguestsapp"));
                noremoteworkersapp = reader.GetBoolean(reader.GetOrdinal("noremoteworkersapp"));
                if (reader.IsDBNull(reader.GetOrdinal("alternateaccountcode")) == false)
                {
                    alternateaccountcode = reader.GetString(reader.GetOrdinal("alternateaccountcode"));
                }
                else
                {
                    alternateaccountcode = "";
                }
                splitpersonal = reader.GetBoolean(reader.GetOrdinal("splitpersonal"));
                splitremote = reader.GetBoolean(reader.GetOrdinal("splitremote"));
                if (reader.IsDBNull(reader.GetOrdinal("personalid")) == true)
                {
                    personalid = 0;
                }
                else
                {
                    personalid = reader.GetInt32(reader.GetOrdinal("personalid"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("remoteid")) == true)
                {
                    remoteid = 0;
                }
                else
                {
                    remoteid = reader.GetInt32(reader.GetOrdinal("remoteid"));
                }
                
                reasonapp = reader.GetBoolean(reader.GetOrdinal("reasonapp"));
                otherdetailsapp = reader.GetBoolean(reader.GetOrdinal("otherdetailsapp"));
                userdefined = clsuserdefined.getValues(AppliesTo.ItemCategory, subcatid);

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
                if (reader.IsDBNull(reader.GetOrdinal("shortsubcat")) == true)
                {
                    shortsubcat = "";
                }
                else
                {
                    shortsubcat = reader.GetString(reader.GetOrdinal("shortsubcat"));
                }
                fromapp = reader.GetBoolean(reader.GetOrdinal("fromapp"));
                toapp = reader.GetBoolean(reader.GetOrdinal("toapp"));
                companyapp = reader.GetBoolean(reader.GetOrdinal("companyapp"));
                lstrates.TryGetValue(subcatid, out rates);
                if (rates == null)
                {
                    rates = new List<cSubcatVatRate>();
                }

                lstCountries.TryGetValue(subcatid, out countries);
                if (countries == null)
                {
                    countries = new List<cCountrySubcat>();
                }
                lstAllowances.TryGetValue(subcatid, out allowances);
                if (allowances == null)
                {
                    allowances = new List<int>();
                }

                lstUDFs.TryGetValue(subcatid, out udfs);
                if (udfs == null)
                {
                    udfs = new List<int>();
                }

                lstSplitsubcats.TryGetValue(subcatid, out split);
                if (split == null)
                {
                    split = new List<int>();
                }
                reqsubcat = new cSubcat(subcatid, categoryid, subcat, description, mileageapp, staffapp, othersapp, tipapp, pmilesapp, bmilesapp, allowance, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, eventinhomeapp, receiptapp, calculation, passengersapp, nopassengersapp, comment, splitentertainment, entertainmentid, reimbursable, nonightsapp, attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumberapp, vatnumbermand, nopersonalguestsapp, noremoteworkersapp, alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, reasonapp, otherdetailsapp, userdefined, createdon, createdby, modifiedon, modifiedby, shortsubcat, fromapp, toapp, countries, allowances, udfs, split, companyapp, rates);
				list.Add(subcatid,reqsubcat);
			}
			reader.Close();
			
			Cache.Insert("subcats" + accountid,list,dep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
			return list;

        }

        private SortedList<int, List<cSubcatVatRate>> getVatRates()
        {

            SortedList<int, List<cSubcatVatRate>> lstrates = new SortedList<int, List<cSubcatVatRate>>();
            List<cSubcatVatRate> rates;
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            double vatamount;
            int subcatid, vatrateid;
            bool vatreceipt;
            decimal? vatlimitwithout;
            decimal? vatlimitwith;
            byte vatpercent;
            DateRangeType daterangetype;
            DateTime datevalue1;
            DateTime? datevalue2;
            strsql = "select * from subcat_vat_rates";
            SqlDataReader reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                vatrateid = reader.GetInt32(reader.GetOrdinal("vatrateid"));
                subcatid = reader.GetInt32(reader.GetOrdinal("subcatid"));
                vatamount = reader.GetDouble(reader.GetOrdinal("vatamount"));
                vatreceipt = reader.GetBoolean(reader.GetOrdinal("vatreceipt"));
                if (reader.IsDBNull(reader.GetOrdinal("vatlimitwithout")))
                {
                    vatlimitwithout = null;
                }
                else
                {
                    vatlimitwithout = reader.GetDecimal(reader.GetOrdinal("vatlimitwithout"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("vatlimitwith")))
                {
                    vatlimitwith = null;
                }
                else
                {
                    vatlimitwith = reader.GetDecimal(reader.GetOrdinal("vatlimitwith"));
                }
                vatpercent = reader.GetByte(reader.GetOrdinal("vatpercent"));
                daterangetype = (DateRangeType)reader.GetByte(reader.GetOrdinal("daterangetype"));
                datevalue1 = reader.GetDateTime(reader.GetOrdinal("datevalue1"));
                if (reader.IsDBNull(reader.GetOrdinal("datevalue2")))
                {
                    datevalue2 = null;
                }
                else
                {
                    datevalue2 = reader.GetDateTime(reader.GetOrdinal("datevalue2"));
                }
                lstrates.TryGetValue(subcatid, out rates);
                if (rates == null)
                {
                    rates = new List<cSubcatVatRate>();
                    lstrates.Add(subcatid, rates);
                }
                rates.Add(new cSubcatVatRate(vatrateid, subcatid, vatamount, vatreceipt, vatlimitwithout, vatlimitwith, vatpercent, daterangetype, datevalue1, datevalue2));
            }
            reader.Close();
            return lstrates;
        }
		public System.Data.DataSet getSplitDS(int subcatid)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			System.Data.DataSet ds;
			strsql = "select subcatid, category, subcat, subcats.description from subcats inner join categories on categories.categoryid = subcats.categoryid where subcatid <> @subcatid order by category, subcat";
			expdata.sqlexecute.Parameters.AddWithValue("@subcatid",subcatid);
			
			ds = expdata.GetDataSet(strsql);
			expdata.sqlexecute.Parameters.Clear();
			return ds;
		}
		

        public DataSet getGrid()
        {
            DataSet ds = new DataSet();
            DataTable tbl = new DataTable();
            tbl.Columns.Add("subcatid", typeof(System.Int32));
            tbl.Columns.Add("categoryid", typeof(System.Int32));
            tbl.Columns.Add("subcat", typeof(System.String));
            tbl.Columns.Add("description", typeof(System.String));

            object[] values;
            cSubcat subcat;
            for (int i = 0; i < list.Count; i++)
            {
                subcat = (cSubcat)list.Values[i];
                values = new object[4];
                values[0] = subcat.subcatid;
                values[1] = subcat.categoryid;
                values[2] = subcat.subcat;
                values[3] = subcat.description;
                tbl.Rows.Add(values);
            }

            ds.Tables.Add(tbl);
            return ds;

        }
        
        public SortedList<string, cSubcat> getSortedList()
        {
            SortedList<string, cSubcat> sorted = new SortedList<string, cSubcat>();

            foreach (cSubcat subcat in list.Values)
            {
                sorted.Add(subcat.subcat, subcat);
            }
            return sorted;
        }
        //public int addSubcat (int categoryid, string subcat, string description, bool vatapp, decimal vatamount, bool vatreceipt, bool mileageapp, bool staffapp, bool othersapp, bool tipapp, bool pmilesapp, bool bmilesapp, bool speedoapp, bool blitresapp, bool plitresapp, bool allowance, decimal allowanceamount, string accountcode, bool attendeesapp, bool addasnet, int pdcatid, int vatpercent, bool eventinhomeapp, List<cRoleSubcat> rolesubcats, bool receipt, int[] userdefined, byte calculation, bool passengersapp, bool nopassengersapp, string comment, int[] allowances, object[,] countries, bool splitentertainment, int entertainmentid, bool reimbursable, bool nonightsapp, Dictionary<int,object> userdefinedfields, bool attendeesmand, bool nodirectorsapp, bool hotelapp, int[] split, bool noroomsapp, bool hotelmand, bool vatnumberapp, bool vatnumbermand, bool nopersonalguestsapp, bool noremoteworkersapp, string alternateaccountcode, bool splitpersonal, bool splitremote, int personalid, int remoteid, decimal vatlimitwithout, decimal vatlimitwith, bool reasonapp, bool otherdetailsapp, int userid, string shortsubcat, bool fromapp, bool toapp)
        //{
        //    if (checkExistance(categoryid,0,0,subcat) == true)
        //    {
        //        return 1;
        //    }
        //    int subcatid = 0;

			
        //    expdata.sqlexecute.Parameters.AddWithValue("@categoryid",categoryid);
        //    expdata.sqlexecute.Parameters.AddWithValue("@subcat",subcat);
        //    expdata.sqlexecute.Parameters.AddWithValue("@description",description);
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatapp",Convert.ToByte(vatapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatamount",vatamount);
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatreceipt",Convert.ToByte(vatreceipt));
        //    expdata.sqlexecute.Parameters.AddWithValue("@mileageapp",Convert.ToByte(mileageapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@staffapp",Convert.ToByte(staffapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@othersapp",Convert.ToByte(othersapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@tipapp",Convert.ToByte(tipapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@pmilesapp",Convert.ToByte(pmilesapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@bmilesapp",Convert.ToByte(bmilesapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@allowance",Convert.ToByte(allowance));
        //    expdata.sqlexecute.Parameters.AddWithValue("@allowanceamount",allowanceamount);
        //    expdata.sqlexecute.Parameters.AddWithValue("@accountcode",accountcode);
        //    expdata.sqlexecute.Parameters.AddWithValue("@speedoapp",Convert.ToByte(speedoapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@plitresapp",Convert.ToByte(plitresapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@blitresapp",Convert.ToByte(blitresapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@attendeesapp",Convert.ToByte(attendeesapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@addasnet",Convert.ToByte(addasnet));
        //    if (pdcatid == 0)
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@pdcatid",DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@pdcatid",pdcatid);
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatpercent",vatpercent);
        //    expdata.sqlexecute.Parameters.AddWithValue("@eventinhomeapp",Convert.ToByte(eventinhomeapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@receipt",Convert.ToByte(receipt));
        //    expdata.sqlexecute.Parameters.AddWithValue("@nopassengersapp",Convert.ToByte(nopassengersapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@calculation",calculation);
        //    expdata.sqlexecute.Parameters.AddWithValue("@passengersapp",Convert.ToByte(passengersapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@comment",comment);
        //    expdata.sqlexecute.Parameters.AddWithValue("@splitentertainment",Convert.ToByte(splitentertainment));
        //    if (entertainmentid == 0)
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@entertainmentid",DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@entertainmentid",entertainmentid);
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@reimbursable",reimbursable);
        //    expdata.sqlexecute.Parameters.AddWithValue("@nonightsapp",Convert.ToByte(nonightsapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@attendeesmand",Convert.ToByte(attendeesmand));
        //    expdata.sqlexecute.Parameters.AddWithValue("@nodirectorsapp",Convert.ToByte(nodirectorsapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@hotelapp",Convert.ToByte(hotelapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@noroomsapp",Convert.ToByte(noroomsapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@hotelmand",Convert.ToByte(hotelmand));
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatnumberapp",Convert.ToByte(vatnumberapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatnumbermand",Convert.ToByte(vatnumbermand));
        //    expdata.sqlexecute.Parameters.AddWithValue("@nopersonalguestsapp", Convert.ToByte(nopersonalguestsapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@noremoteworkersapp", Convert.ToByte(noremoteworkersapp));
        //    if (alternateaccountcode == "")
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@alternateaccountcode", DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@alternateaccountcode", alternateaccountcode);
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@splitpersonal", Convert.ToByte(splitpersonal));
        //    expdata.sqlexecute.Parameters.AddWithValue("@splitremote", Convert.ToByte(splitremote));
        //    if (personalid == 0)
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@personalid", DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@personalid", personalid);
        //    }
        //    if (remoteid == 0)
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@remoteid", DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@remoteid", remoteid);
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatlimitwithout", vatlimitwithout);
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatlimitwith", vatlimitwith);
        //    expdata.sqlexecute.Parameters.AddWithValue("@reasonapp", Convert.ToByte(reasonapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@otherdetailsapp", Convert.ToByte(otherdetailsapp));

        //    if (shortsubcat == "")
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@shortsubcat", DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@shortsubcat", shortsubcat);
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
        //    expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
        //    strsql = "insert into subcats (categoryid, subcat, description, vatapp, vatamount, vatreceipt, mileageapp, staffapp, othersapp, tipapp, pmilesapp, bmilesapp, calculation, speedoapp, blitresapp, plitresapp, allowance, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, vatpercent, eventinhomeapp, receiptapp, passengersapp, nopassengersapp, comment, splitentertainment, entertainmentid, reimbursable, nonightsapp, attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumberapp, vatnumbermand, nopersonalguestsapp, noremoteworkersapp, alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, vatlimitwithout, vatlimitwith, reasonapp, otherdetailsapp, createdon, createdby, shortsubcat) " +
        //        "values (@categoryid,@subcat,@description,@vatapp,@vatamount,@vatreceipt,@mileageapp,@staffapp,@othersapp,@tipapp,@pmilesapp,@bmilesapp,@calculation,@speedoapp,@blitresapp,@plitresapp,@allowance,@allowanceamount,@accountcode,@attendeesapp,@addasnet,@pdcatid,@vatpercent,@eventinhomeapp,@receipt,@passengersapp,@nopassengersapp,@comment, @splitentertainment, @entertainmentid, @reimbursable,@nonightsapp,@attendeesmand, @nodirectorsapp, @hotelapp, @noroomsapp, @hotelmand, @vatnumberapp, @vatnumbermand, @nopersonalguestsapp, @noremoteworkersapp, @alternateaccountcode, @splitpersonal, @splitremote, @personalid, @remoteid, @vatlimitwithout, @vatlimitwith, @reasonapp, @otherdetailsapp, @createdon, @createdby, @shortsubcat)";

			
			
        //    expdata.ExecuteSQL(strsql);
			
        //    strsql = "select subcatid from subcats where subcat = @subcat";
        //    subcatid = expdata.getcount(strsql);

        //    addRoleSubcats(subcatid, rolesubcats);
			
        //    //addCardCats(subcatid,cardcat);
        //    cNewUserDefined clsuserdefined = new cNewUserDefined(accountid);
        //    clsuserdefined.addValues(AppliesTo.ItemCategory, subcatid, userdefinedfields);
        //    //insertAllowances(allowances,subcatid);
        //    insertCountries(subcatid,countries);
        //    //addUserDefined(subcatid, userdefined);
        //    if (split.Length != 0)
        //    {
        //        addSplit(subcatid,split);
        //    }
        //    expdata.sqlexecute.Parameters.Clear();
        //    //remove items from Cache
			
        //    return 0;
        //}

        private void addRoleSubcats(int subcatid, List<cRoleSubcat> items)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            deleteRoleSubcats(subcatid);
            strsql = "";
            foreach (cRoleSubcat rolesub in items)
            {
                strsql += "insert into rolesubcats (roleid, subcatid, maximum, receiptmaximum, isadditem) " +
                "values (" + rolesub.roleid + "," + subcatid + "," + rolesub.maximum + "," + rolesub.receiptmaximum + "," + Convert.ToByte(rolesub.isadditem) + ");";
            }

            if (strsql != "")
            {
                expdata.ExecuteSQL(strsql);
            }

            Cache.Remove("itemroles" + accountid);
        }

        private void deleteRoleSubcats(int subcatid)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            strsql = "delete from rolesubcats where subcatid = @subcatid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
		public void addSplit(int subcatid, List<int> split)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int i;
            int count;
			strsql = "delete from subcat_split where primarysubcatid = " + subcatid;
			expdata.ExecuteSQL(strsql); 
			for (i = 0; i < split.Count; i++)
			{
                if (split[i] != 0)
                {
                    strsql = "select count(*) from subcat_split where primarysubcatid = " + subcatid + " and splitsubcatid = @split";
                    expdata.sqlexecute.Parameters.AddWithValue("@primary", subcatid);
                    expdata.sqlexecute.Parameters.AddWithValue("@split", split[i]);
                    count = expdata.getcount(strsql);
                    if (count == 0)
                    {
                        strsql = "insert into subcat_split (primarysubcatid, splitsubcatid) " +
                            "values (@primary, @split)";

                        expdata.ExecuteSQL(strsql);
                    }
                    expdata.sqlexecute.Parameters.Clear();
                }
			}
		}



        public int saveSubcat(cSubcat subcat, List<cRoleSubcat> rolesubcats)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcat.subcatid);
            expdata.sqlexecute.Parameters.AddWithValue("@categoryid", subcat.categoryid);
            expdata.sqlexecute.Parameters.AddWithValue("@subcat", subcat.subcat);
            expdata.sqlexecute.Parameters.AddWithValue("@description", subcat.description);
            
            expdata.sqlexecute.Parameters.AddWithValue("@mileageapp", Convert.ToByte(subcat.mileageapp));
            expdata.sqlexecute.Parameters.AddWithValue("@staffapp", Convert.ToByte(subcat.staffapp));
            expdata.sqlexecute.Parameters.AddWithValue("@othersapp", Convert.ToByte(subcat.othersapp));
            expdata.sqlexecute.Parameters.AddWithValue("@tipapp", Convert.ToByte(subcat.tipapp));
            expdata.sqlexecute.Parameters.AddWithValue("@pmilesapp", Convert.ToByte(subcat.pmilesapp));
            expdata.sqlexecute.Parameters.AddWithValue("@bmilesapp", Convert.ToByte(subcat.bmilesapp));
            expdata.sqlexecute.Parameters.AddWithValue("@allowance", Convert.ToByte(subcat.allowance));
            expdata.sqlexecute.Parameters.AddWithValue("@allowanceamount", subcat.allowanceamount);
            expdata.sqlexecute.Parameters.AddWithValue("@accountcode", subcat.accountcode);
            expdata.sqlexecute.Parameters.AddWithValue("@attendeesapp", Convert.ToByte(subcat.attendeesapp));
            expdata.sqlexecute.Parameters.AddWithValue("@addasnet", Convert.ToByte(subcat.addasnet));
            if (subcat.pdcatid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@pdcatid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@pdcatid", subcat.pdcatid);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@eventinhomeapp", Convert.ToByte(subcat.eventinhomeapp));
            expdata.sqlexecute.Parameters.AddWithValue("@receiptapp", Convert.ToByte(subcat.receiptapp));
            expdata.sqlexecute.Parameters.AddWithValue("@nopassengersapp", Convert.ToByte(subcat.nopassengersapp));
            expdata.sqlexecute.Parameters.AddWithValue("@calculation", (byte)subcat.calculation);
            expdata.sqlexecute.Parameters.AddWithValue("@passengersapp", Convert.ToByte(subcat.passengersapp));
            expdata.sqlexecute.Parameters.AddWithValue("@comment", subcat.comment);
            expdata.sqlexecute.Parameters.AddWithValue("@splitentertainment", Convert.ToByte(subcat.splitentertainment));
            if (subcat.entertainmentid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@entertainmentid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@entertainmentid", subcat.entertainmentid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@reimbursable", subcat.reimbursable);
            expdata.sqlexecute.Parameters.AddWithValue("@nonightsapp", Convert.ToByte(subcat.nonightsapp));
            expdata.sqlexecute.Parameters.AddWithValue("@attendeesmand", Convert.ToByte(subcat.attendeesmand));
            expdata.sqlexecute.Parameters.AddWithValue("@nodirectorsapp", Convert.ToByte(subcat.nodirectorsapp));
            expdata.sqlexecute.Parameters.AddWithValue("@hotelapp", Convert.ToByte(subcat.hotelapp));
            expdata.sqlexecute.Parameters.AddWithValue("@noroomsapp", Convert.ToByte(subcat.noroomsapp));
            expdata.sqlexecute.Parameters.AddWithValue("@hotelmand", Convert.ToByte(subcat.hotelmand));
            expdata.sqlexecute.Parameters.AddWithValue("@vatnumberapp", Convert.ToByte(subcat.vatnumberapp));
            expdata.sqlexecute.Parameters.AddWithValue("@vatnumbermand", Convert.ToByte(subcat.vatnumbermand));
            expdata.sqlexecute.Parameters.AddWithValue("@nopersonalguestsapp", Convert.ToByte(subcat.nopersonalguestsapp));
            expdata.sqlexecute.Parameters.AddWithValue("@noremoteworkersapp", Convert.ToByte(subcat.noremoteworkersapp));
            if (subcat.alternateaccountcode == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@alternateaccountcode", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@alternateaccountcode", subcat.alternateaccountcode);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@splitpersonal", Convert.ToByte(subcat.splitpersonal));
            expdata.sqlexecute.Parameters.AddWithValue("@splitremote", Convert.ToByte(subcat.splitremote));
            if (subcat.personalid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@personalid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@personalid", subcat.personalid);
            }
            if (subcat.remoteid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@remoteid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@remoteid", subcat.remoteid);
            }
            
            expdata.sqlexecute.Parameters.AddWithValue("@reasonapp", Convert.ToByte(subcat.reasonapp));
            expdata.sqlexecute.Parameters.AddWithValue("@otherdetailsapp", Convert.ToByte(subcat.otherdetailsapp));
            

            if (subcat.shortsubcat == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@shortsubcat", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@shortsubcat", subcat.shortsubcat);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@fromapp", Convert.ToByte(subcat.fromapp));
            expdata.sqlexecute.Parameters.AddWithValue("@toapp", Convert.ToByte(subcat.toapp));
            expdata.sqlexecute.Parameters.AddWithValue("@companyapp", Convert.ToByte(subcat.companyapp));
            if (subcat.subcatid > 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", subcat.modifiedon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", subcat.modifiedby);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", subcat.createdon);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", subcat.createdby);
            }

            expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveSubcat");
            int subcatid = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;

            if (subcatid == -1)
            {
                return -1;
            }

            subcat.updateID(subcatid);

            cItemRoles clsroles = new cItemRoles(accountid);
            addRoleSubcats(subcatid, rolesubcats);
            addUserDefined(subcatid, subcat.associatedudfs);
            //addCardCats(subcatid,cardcat);
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            clsuserdefined.addValues(AppliesTo.ItemCategory, subcatid, subcat.userdefined);
            
            insertAllowances(subcat.allowances, subcatid);
            insertCountries(subcatid, subcat.countries);
            insertVatRates(subcatid, subcat.vatrates);

            addSplit(subcatid, subcat.subcatsplit);


            return subcatid;
        }

        private void insertVatRates(int subcatid, List<cSubcatVatRate> list)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            expdata.ExecuteProc("deleteSubcatVatRates");
            expdata.sqlexecute.Parameters.Clear();

            foreach (cSubcatVatRate rate in list)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
                expdata.sqlexecute.Parameters.AddWithValue("@vatamount", rate.vatamount);
                expdata.sqlexecute.Parameters.AddWithValue("@vatreceipt", Convert.ToByte(rate.vatreceipt));
                expdata.sqlexecute.Parameters.AddWithValue("@vatpercent", rate.vatpercent);
                if (rate.vatlimitwith == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@vatlimitwith", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@vatlimitwith", rate.vatlimitwith);
                }
                if (rate.vatlimitwithout == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@vatlimitwithout", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@vatlimitwithout", rate.vatlimitwithout);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@daterangetype",(byte)rate.daterangetype);
                expdata.sqlexecute.Parameters.AddWithValue("@datevalue1",rate.datevalue1);
                if (rate.daterangetype == DateRangeType.Between)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@datevalue2", rate.datevalue2);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@datevalue2", DBNull.Value);
                }
                expdata.ExecuteProc("addSubcatVatRate");
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        public List<int> getSubcatIds()
        {
            List<int> ids = new List<int>();
            foreach (cSubcat val in list.Values)
            {
                ids.Add(val.subcatid);
            }
            return ids;
        }
        //public int updateSubcat (int subcatid, int categoryid, string subcat, string description, bool vatapp, decimal vatamount, bool vatreceipt, bool mileageapp, bool staffapp, bool othersapp, bool tipapp, bool pmilesapp, bool bmilesapp, bool speedoapp, bool blitresapp, bool plitresapp, bool allowance, decimal allowanceamount, string accountcode, bool attendeesapp, bool addasnet, int pdcatid, int vatpercent, bool eventinhomeapp, List<cRoleSubcat> rolesubcats, bool receipt, int[] userdefined, byte calculation, bool passengersapp, bool nopassengersapp, string comment, int[] allowances, object[,] countries, bool splitentertainment, int entertainmentid, bool reimbursable, bool nonightsapp, Dictionary<int,object> userdefinedfields, bool attendeesmand, bool nodirectorsapp, bool hotelapp, int[] split, bool noroomsapp, bool hotelmand, bool vatnumberapp, bool vatnumbermand, bool nopersonalguestsapp, bool noremoteworkersapp, string alternateaccountcode, bool splitpersonal, bool splitremote, int personalid, int remoteid, decimal vatlimitwithout, decimal vatlimitwith, bool reasonapp, bool otherdetailsapp, int userid, string shortsubcat)
        //{
        //    if (checkExistance(categoryid, subcatid, 2,subcat) == true)
        //    {
        //        return 1;
        //    }

        //    expdata.sqlexecute.Parameters.AddWithValue("@categoryid",categoryid);
        //    expdata.sqlexecute.Parameters.AddWithValue("@subcat",subcat);
        //    expdata.sqlexecute.Parameters.AddWithValue("@description",description);
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatapp",Convert.ToByte(vatapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatamount",vatamount);
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatreceipt",Convert.ToByte(vatreceipt));
        //    expdata.sqlexecute.Parameters.AddWithValue("@mileageapp",Convert.ToByte(mileageapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@staffapp",Convert.ToByte(staffapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@othersapp",Convert.ToByte(othersapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@tipapp",Convert.ToByte(tipapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@pmilesapp",Convert.ToByte(pmilesapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@bmilesapp",Convert.ToByte(bmilesapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@allowance",Convert.ToByte(allowance));
        //    expdata.sqlexecute.Parameters.AddWithValue("@allowanceamount",allowanceamount);
        //    expdata.sqlexecute.Parameters.AddWithValue("@accountcode",accountcode);
        //    expdata.sqlexecute.Parameters.AddWithValue("@speedoapp",Convert.ToByte(speedoapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@plitresapp",Convert.ToByte(plitresapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@blitresapp",Convert.ToByte(blitresapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@attendeesapp",Convert.ToByte(attendeesapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@addasnet",Convert.ToByte(addasnet));
        //    if (pdcatid == 0)
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@pdcatid",DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@pdcatid",pdcatid);
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatpercent",vatpercent);
        //    expdata.sqlexecute.Parameters.AddWithValue("@eventinhomeapp",Convert.ToByte(eventinhomeapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@receipt",Convert.ToByte(receipt));
        //    expdata.sqlexecute.Parameters.AddWithValue("@nopassengersapp",Convert.ToByte(nopassengersapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@calculation",calculation);
        //    expdata.sqlexecute.Parameters.AddWithValue("@passengersapp",Convert.ToByte(passengersapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@comment",comment);
        //    expdata.sqlexecute.Parameters.AddWithValue("@subcatid",subcatid);
        //    expdata.sqlexecute.Parameters.AddWithValue("@splitentertainment",Convert.ToByte(splitentertainment));

        //    if (entertainmentid == 0)
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@entertainmentid",DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@entertainmentid",entertainmentid);
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@reimbursable",reimbursable);
        //    expdata.sqlexecute.Parameters.AddWithValue("@nonightsapp",Convert.ToByte(nonightsapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@attendeesmand",Convert.ToByte(attendeesmand));
        //    expdata.sqlexecute.Parameters.AddWithValue("@nodirectorsapp",Convert.ToByte(nodirectorsapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@hotelapp",Convert.ToByte(hotelapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@noroomsapp",Convert.ToByte(noroomsapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@hotelmand",Convert.ToByte(hotelmand));
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatnumberapp",Convert.ToByte(vatnumberapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatnumbermand",Convert.ToByte(vatnumbermand));
        //    expdata.sqlexecute.Parameters.AddWithValue("@nopersonalguestsapp", Convert.ToByte(nopersonalguestsapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@noremoteworkersapp", Convert.ToByte(noremoteworkersapp));
        //    if (alternateaccountcode == "")
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@alternateaccountcode", DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@alternateaccountcode", alternateaccountcode);
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@splitpersonal", Convert.ToByte(splitpersonal));
        //    expdata.sqlexecute.Parameters.AddWithValue("@splitremote", Convert.ToByte(splitremote));
        //    if (personalid == 0)
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@personalid", DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@personalid", personalid);
        //    }
        //    if (remoteid == 0)
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@remoteid", DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@remoteid", remoteid);
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatlimitwithout", vatlimitwithout);
        //    expdata.sqlexecute.Parameters.AddWithValue("@vatlimitwith", vatlimitwith);
        //    expdata.sqlexecute.Parameters.AddWithValue("@reasonapp", Convert.ToByte(reasonapp));
        //    expdata.sqlexecute.Parameters.AddWithValue("@otherdetailsapp", Convert.ToByte(otherdetailsapp));
            
        //    expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
        //    expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
        //    if (shortsubcat == "")
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@shortsubcat", DBNull.Value);
        //    }
        //    else
        //    {
        //        expdata.sqlexecute.Parameters.AddWithValue("@shortsubcat", shortsubcat);
        //    }
        //    strsql = "update subcats set categoryid = @categoryid, pdcatid = @pdcatid, subcat = @subcat, accountcode = @accountcode, description = @description, vatapp = @vatapp, vatamount = @vatamount, vatpercent = @vatpercent, vatreceipt = @vatreceipt, calculation = @calculation, addasnet = @addasnet, allowance = @allowance, allowanceamount = @allowanceamount, mileageapp = @mileageapp, staffapp = @staffapp, othersapp = @othersapp, attendeesapp = @attendeesapp, pmilesapp = @pmilesapp, bmilesapp = @bmilesapp, speedoapp = @speedoapp, tipapp = @tipapp, plitresapp = @plitresapp, blitresapp = @blitresapp, eventinhomeapp = @eventinhomeapp, receiptapp = @receipt, splitentertainment = @splitentertainment, entertainmentid = @entertainmentid, reimbursable = @reimbursable, nonightsapp = @nonightsapp, nodirectorsapp = @nodirectorsapp";
			
        //    strsql += ", passengersapp = @passengersapp, nopassengersapp = @nopassengersapp, comment = @comment, attendeesmand = @attendeesmand, hotelapp = @hotelapp, noroomsapp = @noroomsapp, hotelmand = @hotelmand, vatnumberapp = @vatnumberapp, vatnumbermand = @vatnumbermand, nopersonalguestsapp = @nopersonalguestsapp, noremoteworkersapp = @noremoteworkersapp, alternateaccountcode = @alternateaccountcode, splitpersonal = @splitpersonal, splitremote = @splitremote, personalid = @personalid, remoteid = @remoteid, vatlimitwithout = @vatlimitwithout, vatlimitwith = @vatlimitwith, reasonapp = @reasonapp, otherdetailsapp = @otherdetailsapp, modifiedon = @modifiedon, modifiedby = @modifiedby, shortsubcat = @shortsubcat";
        //    strsql += " where subcatid = @subcatid";
        //    expdata.ExecuteSQL(strsql);
        //    expdata.sqlexecute.Parameters.Clear();
        //    cItemRoles clsroles = new cItemRoles(accountid);
        //    addRoleSubcats(subcatid, rolesubcats);
        //    //addUserDefined(subcatid, userdefined);
        //    //addCardCats(subcatid,cardcat);
        //    cNewUserDefined clsuserdefined = new cNewUserDefined(accountid);
        //    clsuserdefined.addValues(AppliesTo.ItemCategory, subcatid, userdefinedfields);

        //    //insertAllowances(allowances,subcatid);
        //    insertCountries(subcatid,countries);
			
			
        //    addSplit(subcatid,split);
			
			
        //    return 0;
        //}

        private void addUserDefined(int subcatid, List<int> userdefined)
        {
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int x = 0;
            //cUserDefined clsuserdefined = new cUserDefined(accountid);
            //cUserDefinedField requser;
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
            strsql = "delete from [subcats_userdefined] where subcatid = @subcatid";
            expdata.ExecuteSQL(strsql);

            foreach (int i in userdefined)
            {
                strsql = "insert into [subcats_userdefined] (subcatid, userdefineid) " +
                    "values (@subcatid,@userdefined" + x + ")";
                
                expdata.sqlexecute.Parameters.AddWithValue("@userdefined" + x, i);
                expdata.ExecuteSQL(strsql);
                x++;
            }

            expdata.sqlexecute.Parameters.Clear();
        }
		private void insertCountries (int subcatid, List<cCountrySubcat> countries)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			strsql = "delete from [subcats_countries] where subcatid = @subcatid";
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
			expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

			foreach (cCountrySubcat countrysub in countries)
			{
                
				strsql = "insert into [subcats_countries] (subcatid, countryid, accountcode) " + 
					"values (@subcatid, @countryid, @accountcode)";
				expdata.sqlexecute.Parameters.AddWithValue("@subcatid",subcatid);
				expdata.sqlexecute.Parameters.AddWithValue("@countryid",countrysub.countryid);
				expdata.sqlexecute.Parameters.AddWithValue("@accountcode",countrysub.accountcode);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}

		}
		private bool checkExistance(int categoryid, int subcatid, int action, string subcat)
		{
			int i;
			cSubcat reqsubcat;

			for (i = 0; i < list.Count; i++)
			{
				reqsubcat = (cSubcat)list.Values[i];
				if (action == 2 && reqsubcat.subcat.ToLower() == subcat.ToLower() && reqsubcat.subcatid != subcatid)
				{
					return true;
				}
				else if (action == 0 && reqsubcat.subcat.ToLower() == subcat.ToLower())
				{
					return true;
				}

			}

			return false;
			
		}
		public int deleteSubcat(int subcatid)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@subcatid",subcatid);
			int count = 0;

            strsql = "delete from additems where subcatid = @subcatid";
            expdata.ExecuteSQL(strsql);

            strsql = "delete from subcat_split where primarysubcatid = @subcatid or splitsubcatid = @subcatid";
            expdata.ExecuteSQL(strsql);

			strsql = "select count(*) from savedexpenses where subcatid = @subcatid";
			count = expdata.getcount(strsql);
			if (count != 0)
			{
				return 1; //cannot delete as it is still assigned to items
			}
			strsql = "delete from subcats where subcatid = @subcatid";
			expdata.ExecuteSQL(strsql);

			expdata.sqlexecute.Parameters.Clear();


			list.Remove(subcatid);

            cItemRoles clsroles = new cItemRoles(accountid);
            clsroles.deleteRolesubcatsBySubcatid(subcatid);
			return 0;
		}

        public List<cSubcat> getSubCatByType(CalculationType calcType)
        {
            List<cSubcat> lstSubCats = new List<cSubcat>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list.Values[i].calculation == calcType)
                {
                    lstSubCats.Add(list.Values[i]);
                }
            }
            return lstSubCats;
        }

        public cSubcat getSubcatById(int subcatid)
		{
            cSubcat subcat = null;
            list.TryGetValue(subcatid, out subcat);
            return subcat;
		}

		public System.Data.DataSet getCountryGrid(int subcatid)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cGlobalCountries clscountries = new cGlobalCountries();
            cGlobalCountry country;
            int countryid, globalcountryid;
            string accountcode;
			System.Data.DataSet rcdstcountries = new DataSet();
            DataTable tbl = new DataTable();
            tbl.Columns.Add("subcatid", typeof(System.Int32));
            tbl.Columns.Add("countryid", typeof(System.Int32));
            tbl.Columns.Add("country", typeof(System.String));
            tbl.Columns.Add("accountcode", typeof(System.String));

			strsql = "select subcatid, countries.countryid, countries.globalcountryid, accountcode from [subcats_countries] inner join countries on countries.countryid = [subcats_countries].countryid where subcatid = " + subcatid + " union select " + subcatid + " as subcatid, countryid, globalcountryid, null as accountcode from countries where countryid not in (select countryid from [subcats_countries] where subcatid = " + subcatid  +")";
            System.Data.SqlClient.SqlDataReader reader;
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                subcatid = reader.GetInt32(reader.GetOrdinal("subcatid"));
                countryid = reader.GetInt32(reader.GetOrdinal("countryid"));
                globalcountryid = reader.GetInt32(reader.GetOrdinal("globalcountryid"));
                country = clscountries.getGlobalCountryById(globalcountryid);
                if (reader.IsDBNull(reader.GetOrdinal("accountcode")) == true)
                {
                    accountcode = "";
                }
                else
                {
                    accountcode = reader.GetString(reader.GetOrdinal("accountcode"));
                }
                tbl.Rows.Add(new object[] { subcatid, countryid, country.country, accountcode });
            }
            reader.Close();
            rcdstcountries.Tables.Add(tbl);
			return rcdstcountries;

		}


		public System.Web.UI.WebControls.ListItem[] CreateEntertainmentDropDown(int subcatid, int employeeid)
		{
			int i;
			bool hascar = false;
			System.Collections.SortedList sorted = new SortedList();
			cSubcat reqsubcat;

			if (employeeid != 0)
			{

				cEmployees clsemployees = new cEmployees(accountid);
				cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
				if (reqemp.getActiveCarCount() > 0 || employeeid == 0)
				{
					hascar = true;
				}
			}
			//sort items
			for (i = 0; i < list.Count; i++)
			{
				reqsubcat = (cSubcat)list.Values[i];
				if ((reqsubcat.calculation != CalculationType.PencePerMile || ((reqsubcat.calculation == CalculationType.PencePerMile && hascar == true) || employeeid == 0)) && reqsubcat.vatapp == false)
				{
					sorted.Add(reqsubcat.subcat,reqsubcat);
				}
			}
			System.Web.UI.WebControls.ListItem[] items = new ListItem[sorted.Count];

			for (i = 0; i < sorted.Count; i++)
			{
				reqsubcat = (cSubcat)sorted.GetByIndex(i);
				if (reqsubcat.calculation != CalculationType.PencePerMile || ((reqsubcat.calculation == CalculationType.PencePerMile && hascar == true) || employeeid == 0))
				{
					items[i] = new ListItem();
					items[i].Text = reqsubcat.subcat;
					items[i].Value = reqsubcat.subcatid.ToString();
					if (items[i].Value == subcatid.ToString())
					{
						items[i].Selected = true;
					}
				}
			}

			return items;
		}

        public System.Web.UI.WebControls.ListItem[] CreateSubsistenceDropDown(int subcatid, int employeeid)
        {
            int i;
            bool hascar = false;
            System.Collections.SortedList sorted = new SortedList();
            cSubcat reqsubcat;

            if (employeeid != 0)
            {

                cEmployees clsemployees = new cEmployees(accountid);
                cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
                if (reqemp.getActiveCarCount() > 0 || employeeid == 0)
                {
                    hascar = true;
                }
            }
            //sort items
            for (i = 0; i < list.Count; i++)
            {
                reqsubcat = (cSubcat)list.Values[i];
                if ((reqsubcat.calculation != CalculationType.PencePerMile || ((reqsubcat.calculation == CalculationType.PencePerMile && hascar == true) || employeeid == 0)) && reqsubcat.vatapp == true)
                {
                    sorted.Add(reqsubcat.subcat, reqsubcat);
                }
            }
            System.Web.UI.WebControls.ListItem[] items = new ListItem[sorted.Count];

            for (i = 0; i < sorted.Count; i++)
            {
                reqsubcat = (cSubcat)sorted.GetByIndex(i);
                if (reqsubcat.calculation != CalculationType.PencePerMile || ((reqsubcat.calculation == CalculationType.PencePerMile && hascar == true) || employeeid == 0))
                {
                    items[i] = new ListItem();
                    items[i].Text = reqsubcat.subcat;
                    items[i].Value = reqsubcat.subcatid.ToString();
                    if (items[i].Value == subcatid.ToString())
                    {
                        items[i].Selected = true;
                    }
                }
            }

            return items;
        }

        public List<System.Web.UI.WebControls.ListItem> CreateDropDown()
        {
            List<System.Web.UI.WebControls.ListItem> lst = new List<ListItem>();
            SortedList<string, cSubcat> sorted = getSortedList();
            foreach (cSubcat subcat in sorted.Values)
            {
                lst.Add(new ListItem(subcat.subcat, subcat.subcatid.ToString()));
            }
            return lst;
        }

        public Infragistics.WebUI.UltraWebGrid.ValueList CreateValueList()
        {
            ValueList lst = new ValueList();
            foreach (cSubcat subcat in list.Values)
            {
                lst.ValueListItems.Add(subcat.subcatid, subcat.subcat);
            }
            return lst;
        }
		public System.Web.UI.WebControls.ListItem[] CreateDropDown(int subcatid, int employeeid, bool displayBlank)
		{
			int i;
			bool hascar = false;
			System.Collections.SortedList sorted = new SortedList();
			cSubcat reqsubcat;

			if (employeeid != 0)
			{

				cEmployees clsemployees = new cEmployees(accountid);
				cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
				if (reqemp.getActiveCarCount() > 0 || employeeid == 0)
				{
					hascar = true;
				}
			}
			//sort items
			for (i = 0; i < list.Count; i++)
			{
				reqsubcat = (cSubcat)list.Values[i];
				if (reqsubcat.calculation != CalculationType.PencePerMile || ((reqsubcat.calculation == CalculationType.PencePerMile && hascar == true) || employeeid == 0))
				{
					sorted.Add(reqsubcat.subcat,reqsubcat);
				}
			}
            int count = sorted.Count;
            if (displayBlank)
            {
                count++;
            }
			System.Web.UI.WebControls.ListItem[] items = new ListItem[count];

            count = 0;
            if (displayBlank)
            {
                items[0] = new ListItem();
                items[0].Text = "";
                items[0].Value = "0";
            }
            count++;
			for (i = 0; i < sorted.Count; i++)
			{
				reqsubcat = (cSubcat)sorted.GetByIndex(i);
				if (reqsubcat.calculation != CalculationType.PencePerMile || ((reqsubcat.calculation == CalculationType.PencePerMile && hascar == true) || employeeid == 0))
				{
					items[count] = new ListItem();
					items[count].Text = reqsubcat.subcat;
					items[count].Value = reqsubcat.subcatid.ToString();
					if (items[count].Value == subcatid.ToString())
					{
						items[count].Selected = true;
					}
                    count++;
				}
                
			}

            
			return items;
		}

        public ListItem[] CreateDropDownForCardTransactions()
        {
            List<ListItem> lst = new List<ListItem>();
            SortedList<string, cSubcat> sorted = sortList();

            lst.Add(new ListItem("", "0"));
            foreach (cSubcat subcat in sorted.Values)
            {
                if (!subcat.allowance && (subcat.calculation == CalculationType.NormalItem || subcat.calculation == CalculationType.Meal || subcat.calculation == CalculationType.FuelReceipt))
                {
                    lst.Add(new ListItem(subcat.subcat, subcat.subcatid.ToString()));
                }
            }
            return lst.ToArray();
        }

        private SortedList<string, cSubcat> sortList()
        {
            SortedList<string, cSubcat> lst = new SortedList<string, cSubcat>();
            foreach (cSubcat subcat in list.Values)
            {
                lst.Add(subcat.subcat, subcat);
            }
            return lst;
        }
		private void insertAllowances(List<int> allowances, int subcatid)
		{
            string strsql;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			strsql = "delete from [subcats_allowances] where subcatid = @subcatid";
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
			expdata.ExecuteSQL(strsql);

			strsql = "";
			foreach (int i in allowances)
			{
				strsql += "insert into [subcats_allowances] (subcatid, allowanceid) " +
					"values (" + subcatid + "," + i + ");";
				
			}
			if (strsql != "")
			{
				expdata.ExecuteSQL(strsql);
			}
		}
		
        public SortedList<int,cSubcat> subcats
        {
            get { return list; }
        }

        public sOnlineSubcatInfo getModifiedSubcats(DateTime date)
        {
            sOnlineSubcatInfo onlineSubcatInfo = new sOnlineSubcatInfo();
            List<int> lstSubcatIds = new List<int>();
            Dictionary<int, cSubcat> lstSubcats = new Dictionary<int, cSubcat>();
            List<cRoleSubcat> lstRoleSubcats = new List<cRoleSubcat>();

            cItemRoles clsItemRoles = new cItemRoles(accountid);

            foreach (cItemRole iRole in clsItemRoles.itemRoles.Values)
            {
                Dictionary<int, cRoleSubcat> roleSubcats = clsItemRoles.getSubcatDetails(iRole.itemroleid);

                foreach (cRoleSubcat rSubcat in roleSubcats.Values)
                {
                    lstRoleSubcats.Add(rSubcat);
                }
            }

            foreach (cSubcat val in list.Values)
            {
                lstSubcatIds.Add(val.subcatid);
                if (val.createdon > date || val.modifiedon > date)
                {
                    lstSubcats.Add(val.subcatid, val);
                }
            }
            
            onlineSubcatInfo.lstonlinesubcats = lstSubcats;
            onlineSubcatInfo.lstsubcatids = lstSubcatIds;
            onlineSubcatInfo.lstRoleSubcats = lstRoleSubcats;

            return onlineSubcatInfo;
        }

        public SortedList<int,List<cCountrySubcat>> getCountries()
        {

            SortedList<int, List<cCountrySubcat>> lst = new SortedList<int, List<cCountrySubcat>>();
            string strsql;
            int subcatid;
            int count = 0;
            int countryid;
            string accountcode;
            List<cCountrySubcat> countries;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select * from [subcats_countries]";
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                subcatid = reader.GetInt32(reader.GetOrdinal("subcatid"));
                countryid = reader.GetInt32(reader.GetOrdinal("countryid"));
                accountcode = (string)reader.GetString(reader.GetOrdinal("accountcode"));

                lst.TryGetValue(subcatid, out countries);
                if (countries == null)
                {
                    countries = new List<cCountrySubcat>();
                    lst.Add(subcatid, countries);
                }
                countries.Add(new cCountrySubcat(subcatid, countryid, accountcode));
            }
            reader.Close();
            


            return lst;
        }
        public SortedList<int,List<int>> getAllowances()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            List<int> allowances;
            int subcatid, allowanceid;
            SortedList<int, List<int>> lst = new SortedList<int, List<int>>();
            string strsql;
            System.Data.SqlClient.SqlDataReader reader;

            strsql = "select allowanceid, subcatid from [subcats_allowances]";
            
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                allowanceid = reader.GetInt32(0);
                subcatid = reader.GetInt32(1);
                lst.TryGetValue(subcatid, out allowances);
                if (allowances == null)
                {
                    allowances = new List<int>();
                    lst.Add(subcatid, allowances);
                }
                allowances.Add(allowanceid);

            }
            reader.Close();
            



            return lst;
        }

        private SortedList<int,List<int>> getAssociatedUDFs()
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            string strsql;
            SortedList<int, List<int>> lst = new SortedList<int, List<int>>();
            List<int> lstUDFs;
            int subcatid, userdefineid;
            strsql = "select userdefineid, subcatid from subcats_userdefined";
            
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                userdefineid = reader.GetInt32(0);
                subcatid = reader.GetInt32(1);
                lst.TryGetValue(subcatid, out lstUDFs);
                if (lstUDFs == null)
                {
                    lstUDFs = new List<int>();
                    lst.Add(subcatid, lstUDFs);
                }
                lstUDFs.Add(userdefineid);
                
            }
            reader.Close();
            return lst;
        }

        private SortedList<int,List<int>> getSubcatSplit()
        {
            SortedList<int, List<int>> lst = new SortedList<int, List<int>>();
            List<int> split;

            int subcatid, splitsubcatid;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            System.Data.SqlClient.SqlDataReader reader;


            strsql = "select splitsubcatid, primarysubcatid from subcat_split";
            
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                subcatid = reader.GetInt32(1);
                splitsubcatid = reader.GetInt32(0);
                lst.TryGetValue(subcatid, out split);
                if (split == null)
                {
                    split = new List<int>();
                    lst.Add(subcatid, split);
                }
                split.Add(splitsubcatid);

            }
            reader.Close();
            




            expdata.sqlexecute.Parameters.Clear();
            return lst;
        }

        public cSubcat getSubcatByString(string name)
        {
            cSubcat reqSubcat = null;
            foreach (cSubcat val in list.Values)
            {
                if (val.subcat == name)
                {
                    reqSubcat = val;
                    break;
                }
            }
            return reqSubcat;
        }

	}

	

    
}
