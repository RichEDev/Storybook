using System;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace expenses
{
	/// <summary>
	/// Summary description for mileagecats.
	/// </summary>
	public class cMileagecats
	{
		string strsql;
		
		public System.Collections.SortedList list;

        public System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
		int accountid = 0;
		public cMileagecats(int nAccountid)
		{
			accountid = nAccountid;
            
			
			InitialiseData();
		}

		

		private void InitialiseData()
		{
            list = (System.Collections.SortedList)Cache["mileagecats" + accountid];
			if (list == null)
			{
				list = CacheList();
			}
			
		}

		private System.Collections.SortedList CacheList()
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			string carsize, comment, catvalidcomment;
            ThresholdType thresholdtype;
			bool calcmilestotal, catvalid;
			int mileageid, currencyid;
            MileageUOM mileUom;
            DateTime createdon, modifiedon;
            int createdby, modifiedby;
			cMileageCat reqmileage;
            SortedList<string, object> parameters = new SortedList<string, object>();

            AggregateCacheDependency aggdep = new AggregateCacheDependency();

			System.Collections.SortedList list = new System.Collections.SortedList();
			System.Data.SqlClient.SqlDataReader reader;

            strsql = "SELECT mileagedateid, mileageid, datevalue1, datevalue2 FROM dbo.mileage_dateranges";
            SqlCacheDependency mileagedatedep = expdata.CreateSQLCacheDependency(strsql, parameters);

            strsql = "SELECT mileagethresholdid, mileagedateid, rangetype, rangevalue1, rangevalue2, ppmpetrol, ppmdiesel, ppmlpg, amountforvatp, amountforvatd, amountforvatlpg, passenger1, passengerx, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.mileage_thresholds";
            SqlCacheDependency mileagethresholddep = expdata.CreateSQLCacheDependency(strsql, parameters);

            strsql = "SELECT  mileageid, carsize, comment, thresholdtype, calcmilestotal, unit, catvalid, catvalidcomment, currencyid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy from dbo.mileage_categories";
            SqlCacheDependency mileagecatdep = expdata.CreateSQLCacheDependency(strsql, parameters);
            aggdep.Add(new CacheDependency[] { mileagecatdep, mileagedatedep, mileagethresholddep });


			reader = expdata.GetReader(strsql);
			expdata.sqlexecute.Parameters.Clear();
			while (reader.Read())
			{
				mileageid = reader.GetInt32(reader.GetOrdinal("mileageid"));
				carsize = reader.GetString(reader.GetOrdinal("carsize"));
				if (reader.IsDBNull(reader.GetOrdinal("comment")) == false)
				{
					comment = reader.GetString(reader.GetOrdinal("comment"));
				}
				else
				{
					comment = "";
				}
                thresholdtype = (ThresholdType)reader.GetByte(reader.GetOrdinal("thresholdtype"));
				calcmilestotal = reader.GetBoolean(reader.GetOrdinal("calcmilestotal"));
                mileUom = (MileageUOM)reader.GetByte(reader.GetOrdinal("unit"));
                catvalid = reader.GetBoolean(reader.GetOrdinal("catvalid"));
                currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                if (reader.IsDBNull(reader.GetOrdinal("catvalidcomment")) == false)
                {
                    catvalidcomment = reader.GetString(reader.GetOrdinal("catvalidcomment"));
                }
                else
                {
                    catvalidcomment = "";
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
                reqmileage = new cMileageCat(mileageid, carsize, comment, thresholdtype, calcmilestotal, getMileageDateRanges(mileageid), mileUom, catvalid, catvalidcomment, currencyid, createdon, createdby, modifiedon, modifiedby);

                list.Add(mileageid, reqmileage);
			}
			reader.Close();
			

			Cache.Insert("mileagecats" + accountid, list, aggdep, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable,null);
			return list;


		}
        
        //public decimal getAnnualThresholdMiles(DateTime datevalue1, DateTime datevalue2, DateRangeType daterangetype, int employeeid)
        //{
        //    decimal totalmiles;
        //    DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

        //    strsql = "SELECT SUM(savedexpenses_journey_steps.num_miles) FROM savedexpenses_journey_steps inner join savedexpenses on savedexpenses.expenseid = savedexpenses_journey_steps.expenseid INNER JOIN " +
        //             "claims_base ON claims_base.claimid = savedexpenses.claimid WHERE claims_base.employeeid = @employeeid " +
        //             "AND (savedexpenses.subcatid IN (SELECT subcatid FROM subcats WHERE (calculation = 3)))";

        //    switch (daterangetype)
        //    {
        //        case DateRangeType.Any:
        //            {
        //                break;
        //            }
        //        case DateRangeType.Before:
        //            {
        //                strsql += " AND savedexpenses.date < @datevalue1";
        //                break;
        //            }
        //        case DateRangeType.Between:
        //            {
        //                strsql += " AND savedexpenses.date >= @datevalue1 AND savedexpenses.date <= @datevalue2";
        //                break;
        //            }
        //        case DateRangeType.AfterOrEqualTo:
        //            {
        //                strsql += " AND savedexpenses.date >= @datevalue1";
        //                break;
        //            }
        //    }
        //    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
        //    expdata.sqlexecute.Parameters.AddWithValue("@datevalue1", datevalue1);
        //    expdata.sqlexecute.Parameters.AddWithValue("@datevalue2", datevalue2);
        //    cEmployees clsemps = new cEmployees(accountid);
        //    cEmployee reqemp = clsemps.GetEmployeeById(employeeid);

        //    totalmiles = expdata.getSum(strsql) + reqemp.mileagetotal; ;

        //    return totalmiles;
        //}

		public Infragistics.WebUI.UltraWebGrid.ValueList CreateValueList()
		{
			
			cMileageCat reqmileage;
			Infragistics.WebUI.UltraWebGrid.ValueList temp = new Infragistics.WebUI.UltraWebGrid.ValueList();
			int i = 0;
			
			for (i = 0; i < list.Count; i++)
			{
				reqmileage = (cMileageCat)list.GetByIndex(i);

				temp.ValueListItems.Add(reqmileage.mileageid,reqmileage.carsize);
			}

			return temp;
		}

		public cColumnList CreateColumnList()
		{
			
			cColumnList temp = new cColumnList();
			int i = 0;
			cMileageCat reqmileage;
			for (i = 0; i < list.Count; i++)
			{
				reqmileage = (cMileageCat)list.GetByIndex(i);
				temp.addItem(reqmileage.mileageid,reqmileage.carsize);
			}

			return temp;
		}

		private System.Collections.SortedList sortMileage()
		{
			System.Collections.SortedList sorted = new System.Collections.SortedList();
			int i;
			cMileageCat reqmileage;
			for (i = 0; i < list.Count; i++)
			{
				reqmileage = (cMileageCat)list.GetByIndex(i);
				sorted.Add(reqmileage.carsize,reqmileage);
			}
			return sorted;
		}

        #region Mileage Categories

        public cMileageCat GetMileageCatById(int mileageid)
        {
            return (cMileageCat)list[mileageid];
        }

        public cMileageCat getMileageCatByName(string name)
        {
            cMileageCat reqMileCat = null;

            foreach (cMileageCat clsMileageCat in list.Values)
            {
                if (clsMileageCat.carsize == name)
                {
                    reqMileCat = clsMileageCat;
                    break;
                }
            }
            return reqMileCat;
        }

        public System.Data.DataSet getMileageCatGrid()
        {
            object[] values;
            int i;
            cMileageCat reqmileage;
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();

            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            cCurrencies clscurrencies = new cCurrencies(accountid);
            cCurrency reqcurr;

            System.Collections.SortedList sorted = sortMileage();

            tbl.Columns.Add("mileageid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("carsize", System.Type.GetType("System.String"));
            tbl.Columns.Add("comment", System.Type.GetType("System.String"));
            tbl.Columns.Add("thresholdtype", System.Type.GetType("System.String"));
            tbl.Columns.Add("calcmilestotal", System.Type.GetType("System.Boolean"));
            tbl.Columns.Add("mileuom", System.Type.GetType("System.String"));
            tbl.Columns.Add("currency", System.Type.GetType("System.String"));
            tbl.Columns.Add("catvalidcomment", System.Type.GetType("System.String"));

            for (i = 0; i < sorted.Count; i++)
            {
                reqmileage = (cMileageCat)sorted.GetByIndex(i);
                reqcurr = clscurrencies.getCurrencyById(reqmileage.currencyid);
                values = new object[8];
                values[0] = reqmileage.mileageid;
                values[1] = reqmileage.carsize;
                values[2] = reqmileage.comment;
                values[3] = reqmileage.thresholdType;
                values[4] = reqmileage.calcmilestotal;
                values[5] = reqmileage.mileUom;

                if (reqcurr == null)
                {
                    values[6] = "N/A";
                }
                else
                {
                    values[6] = clsglobalcurrencies.getGlobalCurrencyById(reqcurr.globalcurrencyid).label;
                }
                values[7] = reqmileage.catvalidcomment;
                tbl.Rows.Add(values);
            }

            ds.Tables.Add(tbl);
            return ds;


        }

        public System.Web.UI.WebControls.ListItem[] CreateMileageCatsDropdown(int mileageid, int UOM)
        {
            cMileageCat reqmileage;
            List<ListItem> lst = new List<ListItem>();
            //System.Web.UI.WebControls.ListItem[] tempitems = new System.Web.UI.WebControls.ListItem[list.Count];
            int i = 0;
            //tempitems[0] = new System.Web.UI.WebControls.ListItem();
            //tempitems[0].Text = "";
            //tempitems[0].Value = "";
            for (i = 0; i < list.Count; i++)
            {
                reqmileage = (cMileageCat)list.GetByIndex(i);

                if (reqmileage.catvalid && (int)reqmileage.mileUom == UOM)
                {
                    lst.Add(new ListItem(reqmileage.carsize, reqmileage.mileageid.ToString()));
                    
                    if (reqmileage.mileageid == mileageid)
                    {
                        lst[i].Selected = true;
                    }
                }
            }

            return lst.ToArray();

        }

        private bool mileageCategoryExists(string carsize, int mileageid, int action)
        {
            int i;
            cMileageCat reqmileage;

            for (i = 0; i < list.Count; i++)
            {
                reqmileage = (cMileageCat)list.GetByIndex(i);
                if (action == 2)
                {
                    if (reqmileage.carsize.ToLower() == carsize.ToLower() && reqmileage.mileageid != mileageid)
                    {
                        return true;
                    }
                }
                else
                {
                    if (reqmileage.carsize.ToLower() == carsize.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

		public int addMileageCat(string carsize, string comment, ThresholdType thresholdtype, bool calcmilestotal, MileageUOM mileUom, int currencyid, int userid)
		{
            int mileageid = 0;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			if (mileageCategoryExists(carsize,0,0) == true)
			{
				return 1;
			}

			expdata.sqlexecute.Parameters.AddWithValue("@carsize",carsize);
			expdata.sqlexecute.Parameters.AddWithValue("@comment",comment);
            expdata.sqlexecute.Parameters.AddWithValue("@thresholdtype", Convert.ToByte(thresholdtype));
            expdata.sqlexecute.Parameters.AddWithValue("@mileuom", Convert.ToByte(mileUom));
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
			expdata.sqlexecute.Parameters.AddWithValue("@calcmilestotal",Convert.ToByte(calcmilestotal));
			strsql = "insert into mileage_categories (carsize, comment, thresholdtype, calcmilestotal, unit, currencyid, createdon, createdby) values " +
				"(@carsize,@comment, @thresholdtype, @calcmilestotal, @mileuom, @currencyid, @createdon, @createdby);set @identity = @@identity";

            expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;

			expdata.ExecuteSQL(strsql);
            mileageid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
			expdata.sqlexecute.Parameters.Clear();

			cAuditLog clsaudit = new cAuditLog(accountid, userid);
			clsaudit.addRecord("Mileage Categories",carsize);

			return mileageid;
		}

		public int updateMileageCat(int mileageid, string carsize, string comment, ThresholdType thresholdtype, bool calcmilestotal, MileageUOM mileUom, int currencyid, int userid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            if (mileageCategoryExists(carsize, mileageid, 2) == true)
            {
                return 1;
            }
            cAuditLog clsaudit = new cAuditLog(accountid, userid);
            cMileageCat reqmileage = GetMileageCatById(mileageid);


            expdata.sqlexecute.Parameters.AddWithValue("@carsize", carsize);
            expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
            expdata.sqlexecute.Parameters.AddWithValue("@thresholdtype", Convert.ToByte(thresholdtype)); 
            expdata.sqlexecute.Parameters.AddWithValue("@mileageid", mileageid);
            expdata.sqlexecute.Parameters.AddWithValue("@mileuom", Convert.ToByte(mileUom));
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", currencyid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);
            expdata.sqlexecute.Parameters.AddWithValue("@calcmilestotal", Convert.ToByte(calcmilestotal));
            strsql = "update mileage_categories set carsize = @carsize, comment = @comment, thresholdtype = @thresholdtype, calcmilestotal = @calcmilestotal, unit = @mileuom, currencyid = @currencyid, modifiedon = @modifiedon, modifiedby = @modifiedby where mileageid = @mileageid";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            if (reqmileage.carsize != carsize)
            {
                clsaudit.editRecord(carsize, "Mileage Category", "Mileage Categories", reqmileage.carsize, carsize);
            }
            if (reqmileage.comment != comment)
            {
                clsaudit.editRecord(carsize, "Comment", "Mileage Categories", reqmileage.comment, comment);
            }
            
            if (reqmileage.calcmilestotal != calcmilestotal)
            {
                clsaudit.editRecord(carsize, "Calculate new Mileage Total", "Mileage Categories", reqmileage.calcmilestotal.ToString(), calcmilestotal.ToString());
            }

            if (reqmileage.thresholdType != thresholdtype)
            {
                clsaudit.editRecord(thresholdtype.ToString(), "Threshold Type", "Mileage Categories", reqmileage.thresholdType.ToString(), thresholdtype.ToString());
            }

            if (reqmileage.mileUom != mileUom)
            {
                clsaudit.editRecord(mileUom.ToString(), "Mileage Unit of measure", "Mileage Categories", reqmileage.mileUom.ToString(), mileUom.ToString());
            }

			return 0;
		}

        public void updateCategoryValidStatus(int mileageid, bool valid, string comment)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            if (valid)
            {
                strsql = "UPDATE mileage_categories SET catvalid = 1, catvalidcomment = '' WHERE mileageid = @mileageid";
            }
            else
            {
                strsql = "UPDATE mileage_categories SET catvalid = 0, catvalidcomment = @comment WHERE mileageid = @mileageid";
                expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@mileageid", mileageid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

		public void deleteMileageCat(int mileageid)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cMileageCat reqmileage = GetMileageCatById(mileageid);
			cAuditLog clsaudit = new cAuditLog();
			expdata.sqlexecute.Parameters.AddWithValue("@mileageid",mileageid);
			strsql = "update cars set mileageid = 0 where mileageid = @mileageid";
			
			expdata.ExecuteSQL(strsql);

            foreach (cMileageDaterange range in reqmileage.dateRanges)
            {
                foreach (cMileageThreshold threshold in range.thresholds)
                {
                    deleteMileageThreshold(mileageid, range.mileagedateid, threshold.mileageThresholdid);
                }

                deleteMileageDateRange(mileageid, range.mileagedateid);
            }

			strsql = "delete from mileage_categories where mileageid = @mileageid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			clsaudit.deleteRecord("Mileage Categories",reqmileage.carsize);
			list.Remove(mileageid);
        }

        public string checkCatDateRangeandThresholdLimits(cMileageCat reqmileage)
        {
            string comment = "";
            bool isAny = false;
            bool isBefore = false;
            bool isAfter = false;
            bool isLessThan = false;
            bool isGreaterThan = false;

            List<cMileageDaterange> dateRanges = new List<cMileageDaterange>();

            int i = 0;

            if (reqmileage.dateRanges.Count == 0)
            {
                comment = "This vehicle journey rate category has no date ranges associated with it.";
                return comment;
            }

            foreach (cMileageDaterange range in reqmileage.dateRanges)
            {
                i = 1;
                isAny = false;

                switch (range.daterangetype)
                {
                    case DateRangeType.Any:
                        {
                            isAny = true;
                            break;
                        }
                    case DateRangeType.Before:
                        {
                            isBefore = true;
                            break;
                        }
                    case DateRangeType.AfterOrEqualTo:
                        {
                            isAfter = true;
                            break;
                        }
                }

                if (dateRanges.Count == 0)
                {
                    dateRanges.Add(range);
                }
                else
                {
                    foreach (cMileageDaterange dtrange in dateRanges)
                    {
                        if (range.dateValue1 > dtrange.dateValue1)
                        {
                           
                        }
                        else
                        {
                            if (range.dateValue1 == dtrange.dateValue1 && range.daterangetype == DateRangeType.Before && dtrange.daterangetype == DateRangeType.Between)
                            {
                                i = i - 1;
                            }
                            else if (range.dateValue1 == dtrange.dateValue1 && range.daterangetype == DateRangeType.Between && dtrange.daterangetype == DateRangeType.Before)
                            {

                            }
                            else
                            {
                                i = i - 1;
                            }
                            break;
                        }

                        if (i == dateRanges.Count)
                        {
                            break;
                        }

                        i++;
                    }
                    dateRanges.Insert(i, range);
                }
            }

            if (isAny == false && (isBefore == false || isAfter == false))
            {
                comment = "This vehicle journey rate category is not valid as not all date ranges are covered.";
                return comment;
            }

            DateTime prevDate = new DateTime();
            int j = 0; 
            TimeSpan datediff = new TimeSpan();

            foreach (cMileageDaterange clsrange in dateRanges)
            {
                if (j == 0)
                {
                    prevDate = clsrange.dateValue1;
                }
                else
                {
                    datediff = clsrange.dateValue1 - prevDate;

                    if (datediff.Days > 1)
                    {
                        comment = "This vehicle journey rate category is not valid as not all date ranges are covered.";
                        return comment;
                    }

                    if (clsrange.daterangetype == DateRangeType.Between)
                    {
                        prevDate = clsrange.dateValue2;
                    }
                    else
                    {
                        prevDate = clsrange.dateValue1;
                    }
                }

                j++;
            }

            //Date ranges are valid need to check the thresholds.

            List<cMileageThreshold> thresholds;

            foreach (cMileageDaterange rnge in dateRanges)
            {
                isAny = false;
                isGreaterThan = false;
                isLessThan = false;

                if (rnge.thresholds.Count == 0)
                {
                    if (rnge.daterangetype == DateRangeType.Between)
                    {
                        comment = "The date range of " + rnge.daterangetype.ToString() + " " + rnge.dateValue1.ToShortDateString() + " and " + rnge.dateValue2.ToShortDateString() + " has no thresholds associated with it";
                    }
                    else
                    {
                        comment = "The date range of " + rnge.daterangetype.ToString() + " " + rnge.dateValue1.ToShortDateString() + " has no thresholds associated with it";
                    }
                    return comment;
                    
                }
    
                thresholds = new List<cMileageThreshold>();

                int id = 0;

                foreach (cMileageThreshold threshold in rnge.thresholds)
                {
                    id = 1;
                    isAny = false;

                    switch (threshold.rangeType)
                    {
                        case RangeType.Any:
                            {
                                isAny = true;
                                break;
                                
                            }
                        case RangeType.LessThan:
                            {
                                isLessThan = true;
                                break;
                            }
                        case RangeType.GreaterThanOrEqualTo:
                            {
                                isGreaterThan = true;
                                break;
                            }
                    }

                    if (thresholds.Count == 0)
                    {
                        thresholds.Add(threshold);
                    }
                    else
                    {
                        foreach (cMileageThreshold mthreshold in thresholds)
                        {
                            if (threshold.rangeValue1 >= mthreshold.rangeValue1)
                            {

                            }
                            else
                            {
                                if (threshold.rangeValue1 == mthreshold.rangeValue1 && threshold.rangeType == RangeType.LessThan && mthreshold.rangeType == RangeType.Between)
                                {
                                    id = id - 1;
                                }
                                else if (threshold.rangeValue1 == mthreshold.rangeValue1 && threshold.rangeType == RangeType.Between && mthreshold.rangeType == RangeType.LessThan)
                                {

                                }
                                else
                                {
                                    id = id - 1;
                                }

                                break;
                            }

                            if (id == thresholds.Count)
                            {
                                break;
                            }

                            id++;
                        }
                        thresholds.Insert(id, threshold);
                    }
                }

                if (isAny == false && (isLessThan == false || isGreaterThan == false))
                {
                    comment = "This vehicle journey rate category is not valid as not all thresholds are covered.";
                    return comment;
                }

                int prevVal = 0;
                int n = 0;
                int diff = 0;

                foreach (cMileageThreshold clsthreshold in thresholds)
                {
                    if (n == 0)
                    {
                        prevVal = clsthreshold.rangeValue1;
                    }
                    else
                    {
                        diff = clsthreshold.rangeValue1 - prevVal;

                        if (diff > 1)
                        {
                            comment = "This vehicle journey rate category is not valid as not all thresholds are covered.";
                            return comment;
                        }

                        if (clsthreshold.rangeType == RangeType.Between)
                        {
                            prevVal = clsthreshold.rangeValue2;
                        }
                        else
                        {
                            prevVal = clsthreshold.rangeValue1;
                        }
                    }

                    n++;
                }

            }

            return comment;
        }

        #endregion

        #region Mileage Date Ranges

        public List<cMileageDaterange> getMileageDateRanges(int mileageid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            List<cMileageDaterange> lstDateRanges = new List<cMileageDaterange>();
            DateRangeType daterangetype;
            DateTime datevalue1, datevalue2, createdon, modifiedon;
            int mileagedateid, createdby, modifiedby;
            cMileageDaterange reqdaterange;

            strsql = "SELECT * FROM mileage_dateranges WHERE mileageid = @mileageid";
            expdata.sqlexecute.Parameters.AddWithValue("@mileageid", mileageid);
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                mileagedateid = reader.GetInt32(reader.GetOrdinal("mileagedateid"));

                if (reader.IsDBNull(reader.GetOrdinal("datevalue1")) == true)
                {
                    datevalue1 = new DateTime(1900, 01, 01);
                }
                else
                {
                    datevalue1 = reader.GetDateTime(reader.GetOrdinal("datevalue1"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("datevalue2")) == true)
                {
                    datevalue2 = new DateTime(1900, 01, 01);
                }
                else
                {
                    datevalue2 = reader.GetDateTime(reader.GetOrdinal("datevalue2"));
                }
                daterangetype = (DateRangeType)reader.GetByte(reader.GetOrdinal("daterangetype"));
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

                reqdaterange = new cMileageDaterange(mileagedateid, mileageid, datevalue1, datevalue2, getMileageThresholds(mileagedateid), daterangetype, createdon, createdby, modifiedon, modifiedby);
                lstDateRanges.Add(reqdaterange);
            }
            expdata.sqlexecute.Parameters.Clear();
            reader.Close();
            

            return lstDateRanges;
        }

        public cMileageDaterange getMileageDateRange(cMileageCat reqmileage, DateTime date)
        {
            cMileageDaterange clsdaterange = null;

            foreach (cMileageDaterange range in reqmileage.dateRanges)
            {
                switch (range.daterangetype)
                {
                    case DateRangeType.Any:
                        {
                            clsdaterange = range;
                            break;
                        }
                    case DateRangeType.Before:
                        {
                            if (date < range.dateValue1)
                            {
                                clsdaterange = range; 
                            }
                            break;
                        }
                    case DateRangeType.Between:
                        {
                            if (date >= range.dateValue1 && date <= range.dateValue2)
                            {
                                clsdaterange = range;
                            }
                            break;
                        }
                    case DateRangeType.AfterOrEqualTo:
                        {
                            if (date >= range.dateValue1)
                            {
                                clsdaterange = range;    
                            }
                            break;
                        }
                }
            }

            return clsdaterange;
        }

        public cMileageDaterange getMileageDateRangeById(cMileageCat reqmileage, int mileagedateid)
        {
            cMileageDaterange daterange = null;

            foreach (cMileageDaterange range in reqmileage.dateRanges)
            {
                if (range.mileagedateid == mileagedateid)
                {
                    daterange = range;
                }
            }

            return daterange;
        }

        public System.Data.DataSet getMileageDateRangesGrid(int mileageid)
        {
            object[] values;

            cMileageCat reqmileage = GetMileageCatById(mileageid);
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();

            tbl.Columns.Add("mileagedateid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("datevalue1", System.Type.GetType("System.String"));
            tbl.Columns.Add("datevalue2", System.Type.GetType("System.String"));
            tbl.Columns.Add("daterangetype", System.Type.GetType("System.String"));
            tbl.Columns.Add("numofthresholds", System.Type.GetType("System.Int32"));

            foreach (cMileageDaterange range in reqmileage.dateRanges)
            {
                values = new object[5];
                values[0] = range.mileagedateid;

                if (range.dateValue1 < DateTime.Parse("01/01/1901"))
                {
                    values[1] = "N/A";
                }
                else
                {
                    values[1] = range.dateValue1.ToShortDateString();
                }

                if (range.dateValue2 < DateTime.Parse("01/01/1901"))
                {
                    values[2] = "N/A";
                }
                else
                {
                    values[2] = range.dateValue2.ToShortDateString();
                }

                if (range.daterangetype == DateRangeType.Between)
                {
                    values[3] = range.daterangetype.ToString() + " (Date value 1 to Date value 2)";
                }
                else
                {
                    values[3] = range.daterangetype.ToString();
                }
                values[4] = range.thresholds.Count;

                tbl.Rows.Add(values);
            }

            ds.Tables.Add(tbl);
            return ds;

        }

        private string mileageDateRangeExists(int mileageid, int mileagedateid, ref DateTime datevalue1, ref DateTime datevalue2, ref DateRangeType daterangetype, int action, int employeeid)
        {
            string message = "";

            cMileageCat reqmileage = GetMileageCatById(mileageid);
            cMileageDaterange oldrange = null;
            bool keepExistingRange = false; ;

            if (reqmileage != null)
            {
                if (reqmileage.dateRanges.Count >= 1 && daterangetype == DateRangeType.Any && action != 2)
                {
                    message = "You cannot add a date range of type 'Any' if other date ranges already exist for this category.";
                    return message;
                }
                else if (action == 2 && reqmileage.dateRanges.Count >= 1 && daterangetype == DateRangeType.Any)
                {
                    foreach (cMileageDaterange range in reqmileage.dateRanges)
                    {
                        if (range.daterangetype == DateRangeType.Any)
                        {
                            break;
                        }
                        else
                        {
                            message = "You cannot add a date range of type 'Any' if other date ranges already exist for this category.";
                            return message;
                        }
                    }
                    
                }

                foreach (cMileageDaterange range in reqmileage.dateRanges)
                {
                    if (range.mileagedateid != mileagedateid)
                    {
                        switch (range.daterangetype)
                        {
                            case DateRangeType.Between:
                                {
                                    if (datevalue1 > range.dateValue1 && datevalue1 < range.dateValue2)
                                    {
                                        message = "Date value 1 you have entered falls within an existing 'Between' date range of " + range.dateValue1.ToShortDateString() + " to " + range.dateValue2.ToShortDateString() + ". Please select a different value.";
                                        return message;
                                    }

                                    if (daterangetype == DateRangeType.Between)
                                    {
                                        if (datevalue2 >= range.dateValue1 && datevalue2 <= range.dateValue2)
                                        {
                                            message = "Date value 2 you have entered falls within an existing 'Between' date range of " + range.dateValue1.ToShortDateString() + " to " + range.dateValue2.ToShortDateString() + ". Please select a different value.";
                                            return message;
                                        }

                                        if (range.dateValue1 > datevalue1 && range.dateValue2 < datevalue2)
                                        {
                                            message = "A 'Between' date range of " + range.dateValue1.ToShortDateString() + " to " + range.dateValue2.ToShortDateString() + " exists within your new 'Between' date range. Please select different values.";
                                            return message;
                                        }
                                    }

                                    if (daterangetype == DateRangeType.AfterOrEqualTo)
                                    {
                                        if (datevalue1 == range.dateValue2)
                                        {
                                            message = "Your 'After' value falls within an existing 'Between' date range of " + range.dateValue1.ToShortDateString() + " to " + range.dateValue2.ToShortDateString();
                                            return message;
                                        }
                                    }

                                    break;
                                }
                            case DateRangeType.Before:
                                {
                                    if (daterangetype == DateRangeType.Before)
                                    {
                                        oldrange = range;

                                        if (datevalue1 > range.dateValue1)
                                        {
                                            keepExistingRange = true;
                                        }
                                        else if (datevalue1 < range.dateValue1)
                                        {
                                            keepExistingRange = false;
                                        }
                                        else if (datevalue1 == range.dateValue1)
                                        {
                                            message = "The date you have entered is equal to an existing 'Before' date range with the date " + range.dateValue1.ToShortDateString() + ". Please select a different value.";
                                            return message;
                                        }
                                    }

                                    if (daterangetype == DateRangeType.Between)
                                    {
                                        //if (range.dateValue1 > datevalue1 && range.dateValue1 < datevalue2)
                                        //{
                                        //    message = "An existing 'Before' date range of date " + range.dateValue1.ToShortDateString() + " falls in your 'Between' date range.";
                                        //    return message;
                                        //}

                                        if (datevalue1 < range.dateValue1 || datevalue2 < range.dateValue1)
                                        {
                                            message = "Your 'Between' date range has values that fall within an existing 'Before' date range of date " + range.dateValue1.ToShortDateString();
                                            return message;
                                        }
                                    }

                                    break;
                                }
                            case DateRangeType.AfterOrEqualTo:
                                {
                                    if (daterangetype == DateRangeType.AfterOrEqualTo)
                                    {
                                        oldrange = range;

                                        if (datevalue1 < range.dateValue1)
                                        {
                                            keepExistingRange = true;
                                        }
                                        else if (datevalue1 > range.dateValue1)
                                        {
                                            keepExistingRange = false;
                                        }
                                        else if (datevalue1 == range.dateValue1)
                                        {
                                            message = "The date you have entered is equal to an existing 'After' date range with the date " + range.dateValue1.ToShortDateString() + ". Please select a different value.";
                                            return message;
                                        }
                                    }

                                    if (daterangetype == DateRangeType.Between)
                                    {
                                        //if (range.dateValue1 > datevalue1 && range.dateValue1 < datevalue2)
                                        //{
                                        //    message = "An existing 'After' date range of date " + range.dateValue1.ToShortDateString() + " falls in your 'Between' date range.";
                                        //    return message;
                                        //}

                                        if (datevalue1 >= range.dateValue1 || datevalue2 >= range.dateValue1)
                                        {
                                            message = "Your 'Between' date range has values that fall within an existing 'After' date range of date " + range.dateValue1.ToShortDateString();
                                            return message;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                if (oldrange != null)
                {
                    switch (oldrange.daterangetype)
                    {
                        case DateRangeType.Before:
                            {
                                if (keepExistingRange)
                                {
                                    daterangetype = DateRangeType.Between;
                                    datevalue2 = datevalue1;

                                    foreach (cMileageDaterange dtrange in reqmileage.dateRanges)
                                    {
                                        if (dtrange.daterangetype == DateRangeType.Between && dtrange.dateValue2 > oldrange.dateValue1)
                                        {
                                            datevalue1 = dtrange.dateValue2.AddDays(1);
                                        }
                                        else
                                        {
                                            datevalue1 = oldrange.dateValue1.AddDays(1);
                                        }
                                    }
                                }
                                else
                                {
                                    updateMileageDateRange(oldrange.mileagedateid, oldrange.mileageid, datevalue1.AddDays(1), oldrange.dateValue1, DateRangeType.Between, employeeid, false);
                                }
                                break;
                            }
                        case DateRangeType.AfterOrEqualTo:
                            {
                                if (keepExistingRange)
                                {
                                    daterangetype = DateRangeType.Between;

                                    foreach (cMileageDaterange dtrange in reqmileage.dateRanges)
                                    {
                                        if (dtrange.daterangetype == DateRangeType.Between && dtrange.dateValue1 < oldrange.dateValue2)
                                        {
                                            datevalue2 = dtrange.dateValue2.AddDays(-1);
                                        }
                                        else
                                        {
                                            datevalue2 = oldrange.dateValue1.AddDays(-1);
                                        }
                                    }
                                }
                                else
                                {
                                    updateMileageDateRange(oldrange.mileagedateid, oldrange.mileageid, oldrange.dateValue2, datevalue2.AddDays(-1), DateRangeType.Between, employeeid, false);
                                }
                                break;
                            }
                    }
                }
            }
            return message;
        }

        public sMileageExistenceCheck addMileageDateRange(int mileageid, DateTime datevalue1, DateTime datevalue2, DateRangeType daterangetype, int userid)
        {
            sMileageExistenceCheck mileExCheck = new sMileageExistenceCheck();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            mileExCheck.message = mileageDateRangeExists(mileageid, 0, ref datevalue1, ref datevalue2, ref daterangetype, 0, userid);
            
            if (mileExCheck.message != "")
            {
                mileExCheck.id = 1;
                return mileExCheck;
            }

            int mileagedateid = 0;
            expdata.sqlexecute.Parameters.AddWithValue("@mileageid", mileageid);
            expdata.sqlexecute.Parameters.AddWithValue("@datevalue1", datevalue1);
            expdata.sqlexecute.Parameters.AddWithValue("@datevalue2", datevalue2);
            expdata.sqlexecute.Parameters.AddWithValue("@daterangetype", daterangetype);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
            
            strsql = "insert into mileage_dateranges (mileageid, datevalue1, datevalue2, daterangetype, createdon, createdby) values " +
                "(@mileageid, @datevalue1, @datevalue2, @daterangetype, @createdon, @createdby);set @identity = @@identity";

            expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;

            expdata.ExecuteSQL(strsql);
            mileagedateid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();

            cAuditLog clsaudit = new cAuditLog(accountid, userid);

            switch (daterangetype)
            {
                case DateRangeType.Before:
                    {
                        clsaudit.addRecord("Mileage Date Ranges", "Before date is " + datevalue1.ToString());
                        break;
                    }
                case DateRangeType.AfterOrEqualTo:
                    {
                        clsaudit.addRecord("Mileage Date Ranges", "After date is " + datevalue2.ToString());
                        break;
                    }
                case DateRangeType.Between:
                    {
                        clsaudit.addRecord("Mileage Date Ranges", datevalue1.ToString() + " to " + datevalue2.ToString());
                        break;
                    }
                case DateRangeType.Any:
                    {
                        clsaudit.addRecord("Mileage Date Ranges", "Any date");
                        break;
                    }
            }
            mileExCheck.id = mileagedateid;
            return mileExCheck;
        }

        public sMileageExistenceCheck updateMileageDateRange(int mileagedateid, int mileageid, DateTime datevalue1, DateTime datevalue2, DateRangeType daterangetype, int userid, bool checkExistence)
        {
            sMileageExistenceCheck mileExCheck = new sMileageExistenceCheck();

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            if (checkExistence)
            {
                mileExCheck.message = mileageDateRangeExists(mileageid, mileagedateid, ref datevalue1, ref datevalue2, ref daterangetype, 2, userid);
                if (mileExCheck.message != "")
                {
                    mileExCheck.id = 1;
                    return mileExCheck;
                }
            }

            cAuditLog clsaudit = new cAuditLog(accountid, userid);
            cMileageCat reqmileage = GetMileageCatById(mileageid);
            cMileageDaterange daterange = getMileageDateRangeById(reqmileage, mileagedateid);

            expdata.sqlexecute.Parameters.AddWithValue("@datevalue1", datevalue1);
            expdata.sqlexecute.Parameters.AddWithValue("@datevalue2", datevalue2);
            expdata.sqlexecute.Parameters.AddWithValue("@daterangetype", daterangetype);
            expdata.sqlexecute.Parameters.AddWithValue("@mileagedateid", mileagedateid);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);

            strsql = "update mileage_dateranges set datevalue1 = @datevalue1, datevalue2 = @datevalue2, daterangetype = @daterangetype, modifiedon = @modifiedon, modifiedby = @modifiedby where mileagedateid = @mileagedateid";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            if (daterange.dateValue1 != datevalue1)
            {
                clsaudit.editRecord(datevalue1.ToString(), "Mileage Before Date", "Mileage Date Range", daterange.dateValue1.ToString(), datevalue1.ToString());
            }
            if (daterange.dateValue2 != datevalue2)
            {
                clsaudit.editRecord(datevalue2.ToString(), "Mileage After Date", "Mileage Date Range", daterange.dateValue2.ToString(), datevalue2.ToString());
            }
            if (daterange.daterangetype != daterangetype)
            {
                clsaudit.editRecord(daterangetype.ToString(), "Mileage Date Range Type", "Mileage Date Range", daterange.daterangetype.ToString(), daterangetype.ToString());
            }
            mileExCheck.id = 0;

            return mileExCheck;
        }

        public void deleteMileageDateRange(int mileageid, int mileagedateid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cAuditLog clsaudit = new cAuditLog();
            cMileageCat reqmileage = GetMileageCatById(mileageid);
            cMileageDaterange daterange = getMileageDateRangeById(reqmileage, mileagedateid);

            foreach (cMileageThreshold threshold in daterange.thresholds)
            {
                deleteMileageThreshold(mileageid, mileagedateid, threshold.mileageThresholdid);
            }

            strsql = "DELETE FROM mileage_dateranges WHERE mileagedateid = @mileagedateid";
            expdata.sqlexecute.Parameters.AddWithValue("@mileagedateid", mileagedateid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            clsaudit.deleteRecord("Mileage Date Ranges", daterange.dateValue1.ToString() + " to " + daterange.dateValue2.ToString());
        }

        #endregion

        #region Mileage Thresholds

        public List<cMileageThreshold> getMileageThresholds(int mileagedateid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            List<cMileageThreshold> lstThresholds = new List<cMileageThreshold>();

            DateTime createdon, modifiedon;
            int mileagethresholdid, rangeValue1, rangeValue2, createdby, modifiedby;
            decimal pencePerMilePetrol, pencePerMileDiesel, pencePerMileLpg;
            decimal amountForVatP, amountForVatD, amountForVatLpg, passenger, passengerx;
            RangeType rangetype;

            cMileageThreshold reqthreshold;

            strsql = "SELECT * FROM mileage_thresholds WHERE mileagedateid = @mileagedateid";
            expdata.sqlexecute.Parameters.AddWithValue("@mileagedateid", mileagedateid);

            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                mileagethresholdid = reader.GetInt32(reader.GetOrdinal("mileagethresholdid"));

                rangetype = (RangeType)reader.GetByte(reader.GetOrdinal("rangetype"));

                if (reader.IsDBNull(reader.GetOrdinal("rangevalue1")) == true)
                {
                    rangeValue1 = 0;
                }
                else
                {
                    rangeValue1 = reader.GetInt32(reader.GetOrdinal("rangevalue1"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("rangevalue2")) == true)
                {
                    rangeValue2 = 0;
                }
                else
                {
                    rangeValue2 = reader.GetInt32(reader.GetOrdinal("rangevalue2"));
                }

                pencePerMilePetrol = reader.GetDecimal(reader.GetOrdinal("ppmpetrol"));
                pencePerMileDiesel = reader.GetDecimal(reader.GetOrdinal("ppmdiesel"));
                pencePerMileLpg = reader.GetDecimal(reader.GetOrdinal("ppmlpg"));
                amountForVatP = reader.GetDecimal(reader.GetOrdinal("amountforvatp"));
                amountForVatD = reader.GetDecimal(reader.GetOrdinal("amountforvatd"));
                amountForVatLpg = reader.GetDecimal(reader.GetOrdinal("amountforvatlpg"));
                passenger = reader.GetDecimal(reader.GetOrdinal("passenger1"));
                passengerx = reader.GetDecimal(reader.GetOrdinal("passengerx"));

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

                reqthreshold = new cMileageThreshold(mileagethresholdid, mileagedateid, rangeValue1, rangeValue2, rangetype, pencePerMilePetrol, pencePerMileDiesel, pencePerMileLpg, amountForVatP, amountForVatD, amountForVatLpg, passenger, passengerx, createdon, createdby, modifiedon, modifiedby);
                lstThresholds.Add(reqthreshold);
            }
            expdata.sqlexecute.Parameters.Clear();
            reader.Close();
            

            return lstThresholds;
        }

        public cMileageThreshold getMileageThreshold(cMileageDaterange range, decimal mileagetotal)
        {
            cMileageThreshold reqthreshold = null;

            foreach (cMileageThreshold threshold in range.thresholds)
            {
                switch (threshold.rangeType)
                {
                    case RangeType.Any:
                        {
                            reqthreshold = threshold;
                            break;
                        }
                    case RangeType.GreaterThanOrEqualTo:
                        {
                            if (mileagetotal >= threshold.rangeValue1)
                            {
                                reqthreshold = threshold;
                            }
                            break;
                        }
                    case RangeType.LessThan:
                        {
                            if (mileagetotal < threshold.rangeValue1)
                            {
                                reqthreshold = threshold;
                            }
                            break;
                        }
                    case RangeType.Between:
                        {
                            if (mileagetotal >= threshold.rangeValue1 && mileagetotal <= threshold.rangeValue2)
                            {
                                reqthreshold = threshold;
                            }
                            break;
                        }
                }
            }
            return reqthreshold;
        }

        public cMileageThreshold getMileageThresholdById(cMileageDaterange daterange, int mileagethresholdid)
        {
            cMileageThreshold mileagethreshold = null;

            foreach (cMileageThreshold threshold in daterange.thresholds)
            {
                if (threshold.mileageThresholdid == mileagethresholdid)
                {
                    mileagethreshold = threshold;
                }
            }

            return mileagethreshold;
        }

        public System.Data.DataSet getMileageThresholdsGrid(int mileageid, int mileagedateid)
        {
            object[] values;

            cMileageCat reqmileage = GetMileageCatById(mileageid);
            cMileageDaterange daterange = getMileageDateRangeById(reqmileage, mileagedateid);
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataTable tbl = new System.Data.DataTable();

            tbl.Columns.Add("mileagethresholdid", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("rangetype", System.Type.GetType("System.String"));
            tbl.Columns.Add("rangevalue1", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("rangevalue2", System.Type.GetType("System.Int32"));
            tbl.Columns.Add("pencepermilep", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("pencepermiled", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("pencepermilelpg", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("amountforvatp", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("amountforvatd", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("amountforvatlpg", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("passenger", System.Type.GetType("System.Decimal"));
            tbl.Columns.Add("passengerx", System.Type.GetType("System.Decimal"));

            foreach (cMileageThreshold threshold in daterange.thresholds)
            {
                values = new object[12];
                values[0] = threshold.mileageThresholdid;
                values[1] = threshold.rangeType.ToString();
                values[2] = threshold.rangeValue1;
                values[3] = threshold.rangeValue2;
                values[4] = threshold.pencePerMilePetrol;
                values[5] = threshold.pencePerMileDiesel;
                values[6] = threshold.pencePerMileLpg;
                values[7] = threshold.amountForVatP;
                values[8] = threshold.amountForVatD;
                values[9] = threshold.amountForVatLpg;
                values[10] = threshold.passenger;
                values[11] = threshold.passengerx;

                tbl.Rows.Add(values);
            }

            ds.Tables.Add(tbl);
            return ds;

        }

        private string mileageThresholdExists(int mileageid, int mileagedateid, int mileagethresholdid, ref RangeType rangetype, ref int rangevalue1, ref int rangevalue2, int action, int employeeid)
        {
            string message = "";

            cMileageCat reqmileage = GetMileageCatById(mileageid);

            if (reqmileage != null)
            {
                cMileageDaterange daterange = getMileageDateRangeById(reqmileage, mileagedateid);

                cMileageThreshold oldthreshold = null;
                bool keepExistingThreshold = false; ;

                if (daterange != null)
                {
                    if (daterange.thresholds.Count >= 1 && rangetype == RangeType.Any && action != 2)
                    {
                        message = "You cannot add a threshold of type 'Any' if other thresholds already exist for this date range.";
                        return message;
                    }
                    else if (action == 2 && daterange.thresholds.Count >= 1 && rangetype == RangeType.Any)
                    {
                        foreach (cMileageThreshold threshold in daterange.thresholds)
                        {
                            if (threshold.rangeType == RangeType.Any)
                            {
                                break;
                            }
                            else
                            {
                                message = "You cannot add a threshold of type 'Any' if other thresholds already exist for this date range.";
                                return message;
                            }
                        }

                    }


                    foreach (cMileageThreshold threshold in daterange.thresholds)
                    {
                        if (threshold.mileageThresholdid != mileagethresholdid)
                        {
                            switch (threshold.rangeType)
                            {
                                case RangeType.Between:
                                    {
                                        if (rangevalue1 > threshold.rangeValue1 && rangevalue1 < threshold.rangeValue2)
                                        {
                                            message = "'Threshold value 1' you have entered falls within an existing 'Between' range of " + threshold.rangeValue1.ToString() + " to " + threshold.rangeValue2.ToString() + ". Please select a different value.";
                                            return message;
                                        }

                                        if (rangetype == RangeType.Between)
                                        {
                                            if (rangevalue2 >= threshold.rangeValue1 && rangevalue2 <= threshold.rangeValue2)
                                            {
                                                message = "'Threshold value 2' you have entered falls within an existing 'Between' range of " + threshold.rangeValue1.ToString() + " to " + threshold.rangeValue2.ToString() + ". Please select a different value.";
                                                return message;
                                            }

                                            if (threshold.rangeValue1 > rangevalue1 && threshold.rangeValue2 < rangevalue2)
                                            {
                                                message = "A 'Between' range of " + threshold.rangeValue1.ToString() + " to " + threshold.rangeValue2.ToString() + " exists within your new 'Between' range. Please select different values.";
                                                return message;
                                            }
                                        }

                                        if (rangetype == RangeType.GreaterThanOrEqualTo)
                                        {
                                            if (rangevalue1 == threshold.rangeValue2)
                                            {
                                                message = "Your 'Greater than or equal to' value falls within an existing 'Between' range of " + threshold.rangeValue1.ToString() + " to " + threshold.rangeValue2.ToString() + ". Please select a different value.";
                                                return message;
                                            }
                                        }

                                        break;
                                    }
                                case RangeType.LessThan:
                                    {
                                        if (rangetype == RangeType.LessThan)
                                        {
                                            oldthreshold = threshold;

                                            if (rangevalue1 > threshold.rangeValue1)
                                            {
                                                keepExistingThreshold = true;
                                            }
                                            else if (rangevalue1 < threshold.rangeValue1)
                                            {
                                                keepExistingThreshold = false;
                                            }
                                            else if (rangevalue1 == threshold.rangeValue1)
                                            {
                                                message = "The value you have entered is equal to an existing 'Less Than' range of " + threshold.rangeValue1.ToString() + ". Please select a different value.";
                                                return message;
                                            }
                                        }

                                        if (rangetype == RangeType.Between)
                                        {
                                            //if (threshold.rangeValue1 > rangevalue1 && threshold.rangeValue1 < rangevalue2)
                                            //{
                                            //    message = "An existing 'Less than' range of " + threshold.rangeValue1.ToString() + " falls in your 'Between' range.";
                                            //    return message;
                                            //}

                                            if (rangevalue1 < threshold.rangeValue1 || rangevalue2 < threshold.rangeValue1)
                                            {
                                                message = "Your 'Between' range has values that fall within an existing 'Less than' range of " + threshold.rangeValue1.ToString();
                                                return message;
                                            }
                                        }

                                        break;
                                    }
                                case RangeType.GreaterThanOrEqualTo:
                                    {
                                        if (rangetype == RangeType.GreaterThanOrEqualTo)
                                        {
                                            oldthreshold = threshold;

                                            if (rangevalue1 < threshold.rangeValue1)
                                            {
                                                keepExistingThreshold = true;
                                            }
                                            else if (rangevalue1 > threshold.rangeValue1)
                                            {
                                                keepExistingThreshold = false;
                                            }
                                            else if (rangevalue1 == threshold.rangeValue1)
                                            {
                                                message = "The value you have entered is equal to an existing 'Greater than' range of " + threshold.rangeValue1.ToString() + ". Please select a different value.";
                                                return message;
                                            }
                                        }

                                        if (rangetype == RangeType.Between)
                                        {
                                            //if (threshold.rangeValue1 > rangevalue1 && threshold.rangeValue1 < rangevalue2)
                                            //{
                                            //    message = "An existing 'Greater than' range of " + threshold.rangeValue1.ToString() + " falls in your 'Between' range.";
                                            //    return message;
                                            //}

                                            if (rangevalue1 > threshold.rangeValue1 || rangevalue2 > threshold.rangeValue1)
                                            {
                                                message = "Your 'Between' range has values that fall within an existing 'Greater than' range of " + threshold.rangeValue1.ToString();
                                                return message;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    if (oldthreshold != null)
                    {
                        switch (oldthreshold.rangeType)
                        {
                            case RangeType.LessThan:
                                {
                                    if (keepExistingThreshold)
                                    {
                                        rangetype = RangeType.Between;
                                        rangevalue2 = rangevalue1;

                                        foreach (cMileageThreshold cThreshold in daterange.thresholds)
                                        {
                                            if (cThreshold.rangeType == RangeType.Between && cThreshold.rangeValue2 > oldthreshold.rangeValue1)
                                            {
                                                rangevalue1 = (cThreshold.rangeValue2 + 1);
                                            }
                                            else
                                            {
                                                rangevalue1 = (oldthreshold.rangeValue1 + 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        updateMileageThreshold(oldthreshold.mileageThresholdid, oldthreshold.mileageDateid, mileageid, RangeType.Between, (rangevalue1 + 1), oldthreshold.rangeValue1, 0, 0, 0, 0, 0, 0, 0, 0, employeeid, false);
                                    }
                                    break;
                                }
                            case RangeType.GreaterThanOrEqualTo:
                                {
                                    if (keepExistingThreshold)
                                    {
                                        rangetype = RangeType.Between;

                                        foreach (cMileageThreshold cThreshold in daterange.thresholds)
                                        {
                                            if (cThreshold.rangeType == RangeType.Between && cThreshold.rangeValue1 < oldthreshold.rangeValue2)
                                            {
                                                rangevalue2 = (cThreshold.rangeValue2 - 1);
                                            }
                                            else
                                            {
                                                rangevalue2 = (oldthreshold.rangeValue1 - 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        updateMileageThreshold(oldthreshold.mileageThresholdid, oldthreshold.mileageDateid, mileageid, RangeType.Between, oldthreshold.rangeValue2, (rangevalue2 - 1), 0, 0, 0, 0, 0, 0, 0, 0, employeeid, false);
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            return message;
        }

        public sMileageExistenceCheck addMileageThreshold(int mileagedateid, int mileageid, RangeType rangetype, int rangevalue1, int rangevalue2, decimal pencepermilep, decimal pencepermiled, decimal pencepermilelpg, decimal amountforvatp, decimal amountforvatd, decimal amountforvatlpg, decimal passenger1, decimal passengerx, int userid)
        {
            sMileageExistenceCheck mileExCheck = new sMileageExistenceCheck();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            mileExCheck.message = mileageThresholdExists(mileageid, mileagedateid, 0, ref rangetype, ref rangevalue1, ref rangevalue2, 0, userid);
            
            if (mileExCheck.message != "")
            {
                mileExCheck.id = 1;
                return mileExCheck;
            }

            if (rangetype == RangeType.Between && rangevalue1 == 0 || rangevalue1 == 1)
            {
                rangetype = RangeType.LessThan;
                rangevalue1 = rangevalue2;
                rangevalue2 = 0;
            }

            expdata.sqlexecute.Parameters.AddWithValue("@mileagedateid", mileagedateid);
            expdata.sqlexecute.Parameters.AddWithValue("@rangetype", Convert.ToByte(rangetype));
            expdata.sqlexecute.Parameters.AddWithValue("@rangevalue1", rangevalue1);
            expdata.sqlexecute.Parameters.AddWithValue("@rangevalue2", rangevalue2);
            expdata.sqlexecute.Parameters.AddWithValue("@pencepermilep", pencepermilep);
            expdata.sqlexecute.Parameters.AddWithValue("@pencepermiled", pencepermiled);
            expdata.sqlexecute.Parameters.AddWithValue("@pencepermilelpg", pencepermilelpg);
            expdata.sqlexecute.Parameters.AddWithValue("@amountforvatp", amountforvatp);
            expdata.sqlexecute.Parameters.AddWithValue("@amountforvatd", amountforvatd);
            expdata.sqlexecute.Parameters.AddWithValue("@amountforvatlpg", amountforvatlpg);
            expdata.sqlexecute.Parameters.AddWithValue("@passenger1", passenger1);
            expdata.sqlexecute.Parameters.AddWithValue("@passengerx", passengerx);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);

            strsql = "insert into mileage_thresholds (mileagedateid, rangetype, rangevalue1, rangevalue2, ppmpetrol, ppmdiesel, ppmlpg, amountforvatp, amountforvatd, amountforvatlpg, passenger1, passengerx, createdon, createdby) values " +
                "(@mileagedateid, @rangetype, @rangevalue1, @rangevalue2, @pencepermilep, @pencepermiled, @pencepermilelpg, @amountforvatp, @amountforvatd, @amountforvatlpg, @passenger1, @passengerx, @createdon, @createdby)";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            cAuditLog clsaudit = new cAuditLog(accountid, userid);

            if (rangetype == RangeType.Between)
            {
                clsaudit.addRecord("Mileage Thresholds", rangetype.ToString() + " " + rangevalue1.ToString() + " and " + rangevalue2.ToString());
            }
            else
            {
                clsaudit.addRecord("Mileage Thresholds", rangetype.ToString() + " " + rangevalue1.ToString());
            }
            mileExCheck.id = 0;
            return mileExCheck;
        }

        public sMileageExistenceCheck updateMileageThreshold(int mileagethresholdid, int mileagedateid, int mileageid, RangeType rangetype, int rangevalue1, int rangevalue2, decimal pencepermilep, decimal pencepermiled, decimal pencepermilelpg, decimal amountforvatp, decimal amountforvatd, decimal amountforvatlpg, decimal passenger1, decimal passengerx, int userid, bool checkExistence)
        {
            sMileageExistenceCheck mileExCheck = new sMileageExistenceCheck();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            if (checkExistence)
            {
                mileExCheck.message = mileageThresholdExists(mileageid, mileagedateid, mileagethresholdid, ref rangetype, ref rangevalue1, ref rangevalue2, 2, userid);

                if (mileExCheck.message != "")
                {
                    mileExCheck.id = 1;
                    return mileExCheck;
                }
            }

            if (rangetype == RangeType.Between && rangevalue1 == 0 || rangevalue1 == 1)
            {
                rangetype = RangeType.LessThan;
                rangevalue1 = rangevalue2;
                rangevalue2 = 0;
            }

            cAuditLog clsaudit = new cAuditLog(accountid, userid);
            cMileageCat reqmileage = GetMileageCatById(mileageid);
            cMileageDaterange daterange = getMileageDateRangeById(reqmileage, mileagedateid);
            cMileageThreshold mileagethreshold = getMileageThresholdById(daterange, mileagethresholdid);

            expdata.sqlexecute.Parameters.AddWithValue("@mileagethresholdid", mileagethresholdid);
            expdata.sqlexecute.Parameters.AddWithValue("@rangetype", Convert.ToByte(rangetype));
            expdata.sqlexecute.Parameters.AddWithValue("@rangevalue1", rangevalue1);
            expdata.sqlexecute.Parameters.AddWithValue("@rangevalue2", rangevalue2);
            expdata.sqlexecute.Parameters.AddWithValue("@pencepermilep", pencepermilep);
            expdata.sqlexecute.Parameters.AddWithValue("@pencepermiled", pencepermiled);
            expdata.sqlexecute.Parameters.AddWithValue("@pencepermilelpg", pencepermilelpg);
            expdata.sqlexecute.Parameters.AddWithValue("@amountforvatp", amountforvatp);
            expdata.sqlexecute.Parameters.AddWithValue("@amountforvatd", amountforvatd);
            expdata.sqlexecute.Parameters.AddWithValue("@amountforvatlpg", amountforvatlpg);
            expdata.sqlexecute.Parameters.AddWithValue("@passenger1", passenger1);
            expdata.sqlexecute.Parameters.AddWithValue("@passengerx", passengerx);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedby", userid);

            if (checkExistence)
            {
                strsql = "update mileage_thresholds set rangetype = @rangetype, rangevalue1 = @rangevalue1, rangevalue2 = @rangevalue2, ppmpetrol = @pencepermilep, ppmdiesel = @pencepermiled, ppmlpg = @pencepermilelpg, amountforvatp = @amountforvatp, amountforvatd = @amountforvatd, amountforvatlpg = @amountforvatlpg, passenger1 = @passenger1, passengerx = @passengerx, modifiedon = @modifiedon, modifiedby = @modifiedby where mileagethresholdid = @mileagethresholdid";
            }
            else
            {
                strsql = "update mileage_thresholds set rangetype = @rangetype, rangevalue1 = @rangevalue1, rangevalue2 = @rangevalue2, modifiedon = @modifiedon, modifiedby = @modifiedby where mileagethresholdid = @mileagethresholdid";
            }
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            if (mileagethreshold.rangeType != rangetype)
            {
                clsaudit.editRecord(reqmileage.carsize, "Range Type", "Mileage Thresholds", mileagethreshold.rangeType.ToString(), rangetype.ToString());
            }
            if (mileagethreshold.rangeValue1 != rangevalue1)
            {
                clsaudit.editRecord(reqmileage.carsize, "Threshold Value 1", "Mileage Thresholds", mileagethreshold.rangeValue1.ToString(), rangevalue1.ToString());
            }
            if (mileagethreshold.rangeValue2 != rangevalue2)
            {
                clsaudit.editRecord(reqmileage.carsize, "Threshold Value 2", "Mileage Thresholds", mileagethreshold.rangeValue2.ToString(), rangevalue2.ToString());
            }
            if (mileagethreshold.pencePerMilePetrol != pencepermilep && checkExistence == true)
            {
                clsaudit.editRecord(reqmileage.carsize, "Pence per mile (Petrol)", "Mileage Thresholds", mileagethreshold.pencePerMilePetrol.ToString("£###,##,##0.00"), pencepermilep.ToString("£###,##,##0.00"));
            }
            if (mileagethreshold.pencePerMileDiesel != pencepermiled && checkExistence == true)
            {
                clsaudit.editRecord(reqmileage.carsize, "Pence per mile (Diesel)", "Mileage Thresholds", mileagethreshold.pencePerMileDiesel.ToString("£###,##,##0.00"), pencepermiled.ToString("£###,##,##0.00"));
            }
            if (mileagethreshold.pencePerMileLpg != pencepermilelpg && checkExistence == true)
            {
                clsaudit.editRecord(reqmileage.carsize, "Pence per mile (LPG)", "Mileage Thresholds", mileagethreshold.pencePerMileLpg.ToString("£###,##,##0.00"), pencepermilelpg.ToString("£###,##,##0.00"));
            }
            if (mileagethreshold.amountForVatP != amountforvatp && checkExistence == true)
            {
                clsaudit.editRecord(reqmileage.carsize, "Amount for VAT (Petrol)", "Mileage Thresholds", mileagethreshold.amountForVatP.ToString("£###,##,##0.00"), amountforvatp.ToString("£###,##,##0.00"));
            }
            if (mileagethreshold.amountForVatD != amountforvatd && checkExistence == true)
            {
                clsaudit.editRecord(reqmileage.carsize, "Amount for VAT (Diesel)", "Mileage Thresholds", mileagethreshold.amountForVatD.ToString("£###,##,##0.00"), amountforvatd.ToString("£###,##,##0.00"));
            }
            if (mileagethreshold.amountForVatLpg != amountforvatlpg && checkExistence == true)
            {
                clsaudit.editRecord(reqmileage.carsize, "Amount for VAT (LPG)", "Mileage Thresholds", mileagethreshold.amountForVatLpg.ToString("£###,##,##0.00"), amountforvatlpg.ToString("£###,##,##0.00"));
            }
            if (mileagethreshold.passenger != passenger1 && checkExistence == true)
            {
                clsaudit.editRecord(reqmileage.carsize, "Passenger 1 Rate", "Mileage Thresholds", mileagethreshold.passenger.ToString("£###,##,##0.00"), passenger1.ToString("£###,##,##0.00"));
            }
            if (mileagethreshold.passengerx != passengerx && checkExistence == true)
            {
                clsaudit.editRecord(reqmileage.carsize, "Passenger X Rate", "Mileage Thresholds", mileagethreshold.passengerx.ToString("£###,##,##0.00"), passengerx.ToString("£###,##,##0.00"));
            }

            mileExCheck.id = 0;
            return mileExCheck;
        }

        public void deleteMileageThreshold(int mileageid, int mileagedateid, int mileagethresholdid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cAuditLog clsaudit = new cAuditLog();
            cMileageCat reqmileage = GetMileageCatById(mileageid);
            cMileageDaterange daterange = getMileageDateRangeById(reqmileage, mileagedateid);
            cMileageThreshold mileagethreshold = getMileageThresholdById(daterange, mileagethresholdid);
            
            strsql = "DELETE FROM mileage_thresholds WHERE mileagethresholdid = @mileagethresholdid";
            expdata.sqlexecute.Parameters.AddWithValue("@mileagethresholdid", mileagethresholdid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
            if (mileagethreshold.rangeType == RangeType.Between)
            {
                clsaudit.deleteRecord("Mileage Thresholds", mileagethreshold.rangeType.ToString() + " " + mileagethreshold.rangeValue1.ToString() + " and " + mileagethreshold.rangeValue2.ToString());
            }
            else
            {
                clsaudit.deleteRecord("Mileage Thresholds", mileagethreshold.rangeType.ToString() + " " + mileagethreshold.rangeValue1.ToString());
            }
        }

        #endregion


        public decimal calculateVehicleJourneyTotal(cSubcat reqsubcat, cExpenseItem expenseitem, cEmployee reqemp, cVat clsvat, ThresholdType thresholdtype)
        {
            cMileagecats clsmileagecats;
            cEmployees clsemployees = new cEmployees(accountid);
            cCar reqcar;

            decimal fuelcost = 0;
            int mileageid = 0;
            byte cartypeid = 0;
            decimal totalmiles = 0;
            decimal pencepermile = 0;
            decimal total = 0;
            int carid = 0;
            decimal grandtotal = 0;
            decimal net = 0;
            decimal vat = 0;
            cMileageCat reqmileage;

            clsmileagecats = new cMileagecats(accountid);

            carid = expenseitem.carid;
            if (carid == 0)
            {
                carid = reqemp.getDefaultCarId();
            }
            reqcar = reqemp.getCarById(carid);
            mileageid = expenseitem.mileageid;
            cartypeid = reqcar.cartypeid;
            reqmileage = clsmileagecats.GetMileageCatById(mileageid);

            cMileageDaterange clsdaterange = clsmileagecats.getMileageDateRange(reqmileage, expenseitem.date);

            if (thresholdtype == ThresholdType.Annual)
            {
                totalmiles = clsemployees.getMileageTotal(reqemp.employeeid, expenseitem.date);//clsmileagecats.getAnnualThresholdMiles(clsdaterange.dateValue1, clsdaterange.dateValue2, clsdaterange.daterangetype, reqemp.employeeid);
            }
            
            cMileageThreshold clsthreshold = clsmileagecats.getMileageThreshold(clsdaterange, totalmiles);
            cMileageThreshold threshold = null;
            decimal above = 0;
            decimal below = 0;
            decimal oldmiles = 0;

            foreach (cJourneyStep step in expenseitem.journeysteps.Values)
            {
                oldmiles = totalmiles;
                totalmiles += step.nummiles;

                int count = 0;
                bool leave = false;

                foreach (cMileageThreshold reqthreshold in clsdaterange.thresholds)
                {
                    
                    if (reqthreshold == clsthreshold)
                    {
                        break;
                    }
                    count++;
                }

                for (int i = count; i < clsdaterange.thresholds.Count; i++)
                {
                    threshold = clsdaterange.thresholds[i];

                    pencepermile = getPencePerMile(threshold, cartypeid);
                    fuelcost = getFuelCost(threshold, cartypeid);

                    if (reqsubcat.calculation == CalculationType.PencePerMile && reqsubcat.passengersapp == true)
                    {
                        if (step.numpassengers == 1)
                        {
                            pencepermile = pencepermile + threshold.passenger;
                        }
                        else if (step.numpassengers > 1)
                        {
                            pencepermile = pencepermile + threshold.passenger + (threshold.passengerx * (step.numpassengers - 1));
                        }
                    }

                    switch (threshold.rangeType)
                    {
                        case RangeType.LessThan:

                            if (totalmiles > threshold.rangeValue1)
                            {
                                below = (threshold.rangeValue1 - 1) - oldmiles;

                                total = (below * pencepermile);
                                grandtotal += total;
                                clsvat.calculateVehicleJourneyVAT(reqsubcat, ref net, ref vat, pencepermile, fuelcost, grandtotal);
                                
                            }
                            else
                            {
                                total = ((totalmiles - oldmiles) * pencepermile);
                                oldmiles += (totalmiles - oldmiles);
                                grandtotal += total;
                                clsvat.calculateVehicleJourneyVAT(reqsubcat, ref net, ref vat, pencepermile, fuelcost, grandtotal);
                                
                                leave = true;
                            }
                            break;

                        case RangeType.Between:

                            if (totalmiles > threshold.rangeValue2)
                            {
                                below = threshold.rangeValue2 - oldmiles;

                                total = (below * pencepermile);
                                grandtotal += total;
                                clsvat.calculateVehicleJourneyVAT(reqsubcat, ref net, ref vat, pencepermile, fuelcost, grandtotal);
                                
                            }
                            else
                            {
                                total = ((totalmiles - oldmiles) * pencepermile);
                                oldmiles += (totalmiles - oldmiles);
                                grandtotal += total;
                                clsvat.calculateVehicleJourneyVAT(reqsubcat, ref net, ref vat, pencepermile, fuelcost, grandtotal);
                                leave = true;
                            }
                            break;
                        case RangeType.GreaterThanOrEqualTo:

                            total = ((totalmiles - oldmiles) * pencepermile);
                            oldmiles += (totalmiles - oldmiles);
                            grandtotal += total;
                            clsvat.calculateVehicleJourneyVAT(reqsubcat, ref net, ref vat, pencepermile, fuelcost, grandtotal);
                            
                            leave = true;
                            break;
                        case RangeType.Any:

                            total = ((totalmiles - oldmiles) * pencepermile);
                            oldmiles += (totalmiles - oldmiles);
                            grandtotal += total;
                            clsvat.calculateVehicleJourneyVAT(reqsubcat, ref net, ref vat, pencepermile, fuelcost, grandtotal);

                            leave = true;
                            break;
                    }

                    if (leave)
                    {
                        break;
                    }
                    oldmiles += below;
                    
                }
                clsthreshold = threshold;
            }

            grandtotal = Math.Round(grandtotal, 2);

            expenseitem.updateVAT(net, vat, grandtotal);

            return grandtotal;
        }

        public decimal getPencePerMile(cMileageThreshold clsthreshold, byte cartypeid)
        {
            decimal pencepermile = 0;

            switch (cartypeid)
            {
                case 1:
                    pencepermile = clsthreshold.pencePerMilePetrol;
                    break;
                case 2:
                    pencepermile = clsthreshold.pencePerMileDiesel;
                    break;
                case 3:
                    pencepermile = clsthreshold.pencePerMileLpg;
                    break;
                default:
                    break;
            }

            return pencepermile;
        }

        public decimal getFuelCost(cMileageThreshold clsthreshold, byte cartypeid)
        {
            decimal fuelcost = 0;

            switch (cartypeid)
            {
                case 1:
                    fuelcost = clsthreshold.amountForVatP;
                    break;
                case 2:
                    fuelcost = clsthreshold.amountForVatD;
                    break;
                case 3:
                    fuelcost = clsthreshold.amountForVatLpg;
                    break;
                default:
                    break;
            }

            return fuelcost;
        }

        public void resetMileage(DateTime date)
		{
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			strsql = "update employees set mileagetotal = (select sum(miles) from savedexpenses where employeeid = employees.employeeid and date >= '" + date.Year + "/" + date.Month + "/" + date.Day + "')";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

        public RequiredFieldValidator createValidator(TableCell cell, TableRow row, string id, string control, string errorMsg)
        {
            RequiredFieldValidator reqval = new RequiredFieldValidator();
            reqval.ID = id;
            reqval.ControlToValidate = control;
            reqval.ErrorMessage = errorMsg;
            reqval.Text = "*";
            cell.Controls.Add(reqval);
            row.Cells.Add(cell);

            return reqval;
        }

        public decimal convertMilesToKM(decimal miles)
        {
            decimal kmValue = miles * (decimal)1.609344;
            return decimal.Parse(kmValue.ToString("#0.00"));
        }

        public sMileageInfo getModifiedMileage(DateTime date)
        {
            Dictionary<int, cMileageCat> lstmileagecats = new Dictionary<int, cMileageCat>();
            Dictionary<int, cMileageDaterange> lstdateranges = new Dictionary<int,cMileageDaterange>();
            Dictionary<int, cMileageThreshold> lstthresholds = new Dictionary<int, cMileageThreshold>();
            List<int> lstmileagecatids = new List<int>();
            List<int> lstmileagedateids = new List<int>();
            List<int> lstthresholdids = new List<int>();

            foreach (cMileageCat cat in list.Values)
            {
                lstmileagecatids.Add(cat.mileageid);

                if (cat.createdon > date || cat.modifiedon > date)
                {
                    lstmileagecats.Add(cat.mileageid, cat);
                }

                foreach (cMileageDaterange range in cat.dateRanges)
                {
                    lstmileagedateids.Add(range.mileagedateid);

                    if (range.createdon > date || range.modifiedon > date)
                    {
                        lstdateranges.Add(range.mileagedateid, range);
                    }

                    foreach (cMileageThreshold threshold in range.thresholds)
                    {
                        lstthresholdids.Add(threshold.mileageThresholdid);

                        if (threshold.createdon > date || threshold.modifiedon > date)
                        {
                            lstthresholds.Add(threshold.mileageThresholdid, threshold);
                        }
                    }
                }
            }

            sMileageInfo mileageinfo = new sMileageInfo();

            mileageinfo.lstmileagecats = lstmileagecats;
            mileageinfo.lstdateranges = lstdateranges;
            mileageinfo.lstthresholds = lstthresholds;
            mileageinfo.lstmileagecatids = lstmileagecatids;
            mileageinfo.lstmileagedateids = lstmileagedateids;
            mileageinfo.lstthresholdids = lstthresholdids;

            return mileageinfo;
        }
	}




	
}
