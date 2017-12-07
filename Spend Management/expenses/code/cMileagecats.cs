namespace Spend_Management
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Employees;
    using System.Linq;
    using System.Text.RegularExpressions;

    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.JourneyDeductionRules;    

    /// <summary>
    /// Summary description for mileagecats.
    /// </summary>
    public class cMileagecats
    {

        private Utilities.DistributedCaching.Cache Cache = new Utilities.DistributedCaching.Cache();
        int accountid = 0;

        /// <summary>
        /// A private instance of <see cref="Addresses"/>
        /// </summary>
        private Addresses _addresses;

        public cMileagecats(int nAccountid)
        {
            accountid = nAccountid;
            this._addresses = new Addresses(nAccountid);
        }

        private cMileageCat GetFromDatabase(int id)
        {
            var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid));

            var lstDateranges = getMileageDateRanges();

            var parameters = new SortedList<string, object>();

            cMileageCat reqmileage = null;

            expdata.sqlexecute.Parameters.AddWithValue("@mileageid", id);
            using (IDataReader reader = expdata.GetReader("GetMileageCategories", CommandType.StoredProcedure))
            {
                expdata.sqlexecute.Parameters.Clear();
                int mileageidOrd = reader.GetOrdinal("mileageid");
                int carsizeOrd = reader.GetOrdinal("carsize");
                int commentOrd = reader.GetOrdinal("comment");
                int thresholdtypeOrd = reader.GetOrdinal("thresholdtype");
                int calcmilestotalOrd = reader.GetOrdinal("calcmilestotal");
                int unitOrd = reader.GetOrdinal("unit");
                int catvalidcommentOrd = reader.GetOrdinal("catvalidcomment");
                int createdonOrd = reader.GetOrdinal("createdon");
                int createbyOrd = reader.GetOrdinal("createdby");
                int modifiedonOrd = reader.GetOrdinal("modifiedon");
                int modifiedbyOrd = reader.GetOrdinal("modifiedby");
                int userRatesTableOrd = reader.GetOrdinal("UserRatesTable");
                int userRatesEngineFromSizeOrd = reader.GetOrdinal("UserRatesFromEngineSize");
                int userRatesEngineToSizeOrd = reader.GetOrdinal("UserRatesToEngineSize");
                int financialYearIDOrd = reader.GetOrdinal("FinancialYearID");

                while (reader.Read())
                {
                    int mileageid = reader.GetInt32(mileageidOrd);
                    string carsize = reader.GetString(carsizeOrd);
                    string comment = reader.IsDBNull(commentOrd) == false ? reader.GetString(commentOrd) : string.Empty;
                    var thresholdtype = (ThresholdType)reader.GetByte(thresholdtypeOrd);
                    bool calcmilestotal = reader.GetBoolean(calcmilestotalOrd);
                    var mileUom = (MileageUOM)reader.GetByte(unitOrd);
                    bool catvalid = reader.GetBoolean(reader.GetOrdinal("catvalid"));
                    int currencyid = reader.GetInt32(reader.GetOrdinal("currencyid"));
                    string catvalidcomment = reader.IsDBNull(catvalidcommentOrd) == false ? reader.GetString(catvalidcommentOrd) : string.Empty;
                    DateTime createdon = reader.IsDBNull(createdonOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(createdonOrd);
                    int createdby = reader.IsDBNull(createbyOrd) ? 0 : reader.GetInt32(createbyOrd);
                    DateTime? modifiedon = reader.IsDBNull(modifiedonOrd) ? (DateTime?)null : reader.GetDateTime(modifiedonOrd);
                    int? modifiedby = reader.IsDBNull(modifiedbyOrd) ? (int?)null : reader.GetInt32(modifiedbyOrd);
                    List<cMileageDaterange> dateranges;
                    lstDateranges.TryGetValue(mileageid, out dateranges);

                    string userRatesTable = reader.IsDBNull(userRatesTableOrd)
                                                    ? string.Empty
                                                    : reader.GetString(userRatesTableOrd);
                    int userRatesEngineFromSize = reader.IsDBNull(userRatesEngineFromSizeOrd)
                                                          ? 0
                                                          : reader.GetInt32(userRatesEngineFromSizeOrd);
                    int userRatesEngineToSize = reader.IsDBNull(userRatesEngineToSizeOrd)
                                                        ? 0
                                                        : reader.GetInt32(userRatesEngineToSizeOrd);

                    int? financialYearID = reader.IsDBNull(financialYearIDOrd)
                                                        ? null
                                                        : (int?)reader.GetInt32(financialYearIDOrd);

                    if (dateranges == null)
                    {
                        dateranges = new List<cMileageDaterange>();
                    }

                    reqmileage = new cMileageCat(mileageid, carsize, comment, thresholdtype, calcmilestotal, dateranges, mileUom, catvalid, catvalidcomment, currencyid, createdon, createdby, modifiedon, modifiedby, userRatesTable, userRatesEngineFromSize, userRatesEngineToSize, financialYearID);

                    this.AddCache(reqmileage);
                }

                reader.Close();
            }

            return reqmileage;
        }

        private void AddCache(cMileageCat reqmileage)
        {
            this.Cache.Add(this.accountid, CacheArea, reqmileage.mileageid.ToString(), reqmileage);
        }

        public decimal getAnnualThresholdMiles(DateTime? datevalue1, DateTime? datevalue2, DateRangeType daterangetype, int employeeid)
        {
            decimal totalmiles;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            var strsql = "SELECT SUM(savedexpenses_journey_steps.num_miles) FROM savedexpenses_journey_steps inner join savedexpenses on savedexpenses.expenseid = savedexpenses_journey_steps.expenseid INNER JOIN " +
                     "claims_base ON claims_base.claimid = savedexpenses.claimid WHERE claims_base.employeeid = @employeeid " +
                     "AND (savedexpenses.subcatid IN (SELECT subcatid FROM subcats WHERE (calculation = 3)))";

            switch (daterangetype)
            {
                case DateRangeType.Any:
                    {
                        break;
                    }
                case DateRangeType.Before:
                    {
                        strsql += " AND savedexpenses.date < @datevalue1";
                        break;
                    }
                case DateRangeType.Between:
                    {
                        strsql += " AND savedexpenses.date >= @datevalue1 AND savedexpenses.date <= @datevalue2";
                        break;
                    }
                case DateRangeType.AfterOrEqualTo:
                    {
                        strsql += " AND savedexpenses.date >= @datevalue1";
                        break;
                    }
            }
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            if (datevalue1 != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@datevalue1", datevalue1);
            }
            if (datevalue2 != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@datevalue2", datevalue2);
            }
            cEmployees clsemps = new cEmployees(accountid);
            Employee reqemp = clsemps.GetEmployeeById(employeeid);

            totalmiles = expdata.getSum(strsql) + reqemp.MileageTotal;

            return totalmiles;
        }


        public System.Collections.SortedList sortMileage()
        {
            System.Collections.SortedList sorted = new System.Collections.SortedList();
            int i;

            foreach (int mileageId in this.GetMileageIDs())
            {
                var mileageItem = this.GetMileageCatById(mileageId);
                sorted.Add(mileageItem.carsize, mileageItem);
            }

            return sorted;
        }

        public List<int> GetMileageIDs(string nameFilter = "")
        {
            var mileageIds = new List<int>();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                var sql = "SELECT mileageid FROM mileage_categories WHERE catvalid = 1";
                if (!string.IsNullOrEmpty(nameFilter))
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@name", nameFilter);
                    sql = sql + " AND (carsize = @name OR comment = @name)";
                }

                using (var reader = expdata.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        mileageIds.Add(reader.GetInt32(0));
                    }
                }
            }

            return mileageIds;
        }

        public List<int> GetAllMileageIDs()
        {
            var mileageIds = new List<int>();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                var sql = "SELECT mileageid FROM mileage_categories";

                using (var reader = expdata.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        mileageIds.Add(reader.GetInt32(0));
                    }
                }
            }

            return mileageIds;
        }

        #region Mileage Categories

        public virtual cMileageCat GetMileageCatById(int mileageid)
        {
            cMileageCat mileage;
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                mileage = (cMileageCat)this.Cache.Get(this.accountid, CacheArea, mileageid.ToString()) ?? GetFromDatabase(mileageid);
            }

            return mileage;
        }

        public virtual cMileageCat getMileageCatByName(string name)
        {
            cMileageCat reqMileCat = null;
            foreach (int mileageID in this.GetMileageIDs(name))
            {
                var clsMileageCat = this.GetMileageCatById(mileageID);

                if (clsMileageCat.carsize == name || clsMileageCat.comment == name)
                {
                    reqMileCat = clsMileageCat;
                    break;
                }
            }
            return reqMileCat;
        }

        public List<ListItem> CreateDropDown()
        {
            SortedList lst = sortMileage();
            List<ListItem> items = new List<ListItem>();
            foreach (cMileageCat cat in lst.Values)
            {
                items.Add(new ListItem(cat.carsize, cat.mileageid.ToString()));
            }
            return items;

        }
        public System.Web.UI.WebControls.ListItem[] CreateMileageCatsDropdown(int mileageid, int UOM)
        {
            cMileageCat reqmileage;
            List<ListItem> lst = new List<ListItem>();
            int i = 0;
            foreach (var mileageId in this.GetMileageIDs())
            {
                reqmileage = this.GetMileageCatById(mileageId);

                if (reqmileage.catvalid && (int)reqmileage.mileUom == UOM)
                {
                    lst.Add(new ListItem(reqmileage.carsize, reqmileage.mileageid.ToString()));

                    if (reqmileage.mileageid == mileageid)
                    {
                        lst[i].Selected = true;
                    }
                }

                i++;
            }

            return lst.ToArray();

        }


        public void deleteMileageCat(int mileageid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            data.sqlexecute.Parameters.AddWithValue("@mileageid", mileageid);
            data.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            data.ExecuteProc("deleteVehicleJourneyRate");
            data.sqlexecute.Parameters.Clear();
            this.DeleteCache(mileageid);
        }

        private void DeleteCache(int mileageid)
        {
            this.Cache.Delete(this.accountid, CacheArea, mileageid.ToString());
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


            foreach (cMileageDaterange clsrange in dateRanges)
            {
                if (j == 0)
                {
                    if (clsrange.dateValue1 == null)
                    {
                        prevDate = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        prevDate = (DateTime)clsrange.dateValue1;
                    }
                }
                else
                {
                    //if(clsrange.dateValue1.HasValue && clsrange.dateValue2.HasValue) {


                    if (clsrange.dateValue1.HasValue == false || (clsrange.dateValue1.Value - prevDate).Days > 1)
                    {
                        comment = "This vehicle journey rate category is not valid as not all date ranges are covered.";
                        return comment;
                    }

                    if (clsrange.daterangetype == DateRangeType.Between)
                    {
                        prevDate = (DateTime)clsrange.dateValue2;
                    }
                    else
                    {
                        prevDate = (DateTime)clsrange.dateValue1;
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
                        comment = "The date range of " + rnge.daterangetype.ToString() + " " + ((DateTime)rnge.dateValue1).ToShortDateString() + " and " + ((DateTime)rnge.dateValue2).ToShortDateString() + " has no thresholds associated with it";
                    }
                    else
                    {
                        if (rnge.dateValue1 != null)
                        {
                            comment = "The date range of " + rnge.daterangetype.ToString() + " " + ((DateTime)rnge.dateValue1).ToShortDateString() + " has no thresholds associated with it";
                        }
                    }
                    return comment;

                }

                thresholds = new List<cMileageThreshold>();

                int id = 0;

                foreach (cMileageThreshold threshold in rnge.thresholds)
                {
                    id = 1;
                    isAny = false;

                    switch (threshold.RangeType)
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
                            if (threshold.RangeValue1 >= mthreshold.RangeValue1)
                            {

                            }
                            else
                            {
                                if (threshold.RangeValue1 == mthreshold.RangeValue1 && threshold.RangeType == RangeType.LessThan && mthreshold.RangeType == RangeType.Between)
                                {
                                    id = id - 1;
                                }
                                else if (threshold.RangeValue1 == mthreshold.RangeValue1 && threshold.RangeType == RangeType.Between && mthreshold.RangeType == RangeType.LessThan)
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
                        prevVal = (int)clsthreshold.RangeValue1;
                    }
                    else
                    {
                        diff = (int)clsthreshold.RangeValue1 - prevVal;

                        if (diff > 1)
                        {
                            comment = "This vehicle journey rate category is not valid as not all thresholds are covered.";
                            return comment;
                        }

                        if (clsthreshold.RangeType == RangeType.Between)
                        {
                            prevVal = (int)clsthreshold.RangeValue2;
                        }
                        else
                        {
                            prevVal = (int)clsthreshold.RangeValue1;
                        }
                    }

                    n++;
                }

            }

            return comment;
        }

        #endregion

        #region Mileage Date Ranges

        public SortedList<int, List<cMileageDaterange>> getMileageDateRanges()
        {
            SortedList<int, List<cMileageDaterange>> lst = new SortedList<int, List<cMileageDaterange>>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            List<cMileageDaterange> lstDateRanges;
            DateRangeType daterangetype;
            DateTime? datevalue1;
            DateTime? datevalue2;
            DateTime createdon;
            DateTime? modifiedon;
            int mileagedateid, createdby;
            int? modifiedby;
            int mileageid;
            cMileageDaterange reqdaterange;

            SortedList<int, List<cMileageThreshold>> lstThresholds = getMileageThresholds();
            List<cMileageThreshold> thresholds;
            using (SqlDataReader reader = expdata.GetProcReader("GetMileageDateRanges"))
            {
                while (reader.Read())
                {
                    mileagedateid = reader.GetInt32(reader.GetOrdinal("mileagedateid"));
                    mileageid = reader.GetInt32(reader.GetOrdinal("mileageid"));
                    if (reader.IsDBNull(reader.GetOrdinal("datevalue1")) == true)
                    {
                        datevalue1 = null;
                    }
                    else
                    {
                        datevalue1 = reader.GetDateTime(reader.GetOrdinal("datevalue1"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("datevalue2")) == true)
                    {
                        datevalue2 = null;
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
                        modifiedon = null;
                    }
                    else
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == true)
                    {
                        modifiedby = null;
                    }
                    else
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }
                    lst.TryGetValue(mileageid, out lstDateRanges);
                    if (lstDateRanges == null)
                    {
                        lstDateRanges = new List<cMileageDaterange>();
                        lst.Add(mileageid, lstDateRanges);
                    }
                    lstThresholds.TryGetValue(mileagedateid, out thresholds);
                    if (thresholds == null)
                    {
                        thresholds = new List<cMileageThreshold>();
                    }
                    reqdaterange = new cMileageDaterange(mileagedateid, mileageid, datevalue1, datevalue2, thresholds, daterangetype, createdon, createdby, modifiedon, modifiedby);
                    lstDateRanges.Add(reqdaterange);
                }
                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
            }


            return lst;
        }

        /// <summary>
        /// Return the Mileage Date Range for the selected date.
        /// </summary>
        /// <param name="reqmileage">The Vechile Journey Rate</param>
        /// <param name="date">The Expense Item Date</param>
        /// <returns>The valid date range from the given VJR</returns>
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
                            if (date.Date < range.dateValue1)
                            {
                                clsdaterange = range;
                            }
                            break;
                        }
                    case DateRangeType.Between:
                        {
                            if (date.Date >= range.dateValue1 && date.Date <= range.dateValue2)
                            {
                                clsdaterange = range;
                            }
                            break;
                        }
                    case DateRangeType.AfterOrEqualTo:
                        {
                            if (date.Date >= range.dateValue1)
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
                    values[1] = ((DateTime)range.dateValue1).ToShortDateString();
                }

                if (range.dateValue2 < DateTime.Parse("01/01/1901"))
                {
                    values[2] = "N/A";
                }
                else
                {
                    values[2] = ((DateTime)range.dateValue2).ToShortDateString();
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

        public string mileageDateRangeExists(int mileageid, int mileagedateid, ref DateTime? datevalue1, ref DateTime? datevalue2, ref DateRangeType daterangetype, int employeeid) //int action, 
        {
            string message = "";
            cMileageCat reqmileage = GetMileageCatById(mileageid);
            cMileageDaterange oldrange = null;
            bool keepExistingRange = false; ;

            if (mileagedateid == 0)
            {
                if (reqmileage.dateRanges.Count >= 1 && daterangetype == DateRangeType.Any)
                {
                    message = "You cannot add a date range of type 'Any' if other date ranges already exist for this category.";
                    return message;
                }
            }

            if (daterangetype == DateRangeType.Between)
            {
                if (datevalue2 < datevalue1)
                {
                    message = "Date value 2 must be greater than Date value 1.";
                    return message;
                }
            }

            foreach (cMileageDaterange range in reqmileage.dateRanges)
            {
                if (mileagedateid > 0)
                {
                    if (range.mileagedateid != mileagedateid)
                    {
                        switch (range.daterangetype)
                        {
                            case DateRangeType.Between:
                                {
                                    if ((datevalue1 > range.dateValue1 && datevalue1 <= range.dateValue2) && range.mileagedateid != mileagedateid)
                                    {
                                        message = "Date value 1 you have entered falls within an existing 'Between' date range of " + ((DateTime)range.dateValue1).ToShortDateString() + " to " + ((DateTime)range.dateValue2).ToShortDateString() + ". Please select a different value.";
                                        return message;
                                    }

                                    if (daterangetype == DateRangeType.Between)
                                    {
                                        if ((datevalue2 >= range.dateValue1 && datevalue2 <= range.dateValue2) && range.mileagedateid != mileagedateid)
                                        {
                                            message = "Date value 2 you have entered falls within an existing 'Between' date range of " + ((DateTime)range.dateValue1).ToShortDateString() + " to " + ((DateTime)range.dateValue2).ToShortDateString() + ". Please select a different value.";
                                            return message;
                                        }

                                        if ((range.dateValue1 > datevalue1 && range.dateValue2 < datevalue2) && range.mileagedateid != mileagedateid)
                                        {
                                            message = "A 'Between' date range of " + ((DateTime)range.dateValue1).ToShortDateString() + " to " + ((DateTime)range.dateValue2).ToShortDateString() + " exists within your new 'Between' date range. Please select different values.";
                                            return message;
                                        }
                                    }

                                    if (daterangetype == DateRangeType.AfterOrEqualTo)
                                    {
                                        if (datevalue1 == range.dateValue2 || datevalue1 == range.dateValue1)
                                        {
                                            message = "Your 'After' value falls within an existing 'Between' date range of " + ((DateTime)range.dateValue1).ToShortDateString() + " to " + ((DateTime)range.dateValue2).ToShortDateString();
                                            return message;
                                        }
                                    }
                                    break;
                                }
                            case DateRangeType.Before:
                                {
                                    if (daterangetype == DateRangeType.Before)
                                    {
                                        if ((datevalue1 == range.dateValue1) && range.mileagedateid != mileagedateid)
                                        {
                                            message = "The date you have entered is equal to an existing 'Before' date range with the date " + ((DateTime)range.dateValue1).ToShortDateString() + ". Please select a different value.";
                                            return message;
                                        }
                                    }

                                    if (daterangetype == DateRangeType.Between)
                                    {
                                        //if ((range.dateValue1 > datevalue1 && range.dateValue1 < datevalue2) && range.mileagedateid != mileagedateid)
                                        //{
                                        //    message = "An existing 'Before' date range of date " + range.dateValue1.ToShortDateString() + " falls in your 'Between' date range.";
                                        //    return message;
                                        //}

                                        if (datevalue1 < range.dateValue1 || datevalue2 < range.dateValue1)
                                        {
                                            message = "Your 'Between' date range has values that fall within an existing 'Before' date range of date " + ((DateTime)range.dateValue1).ToShortDateString();
                                            return message;
                                        }
                                    }

                                    break;
                                }
                            case DateRangeType.AfterOrEqualTo:
                                {
                                    switch (daterangetype)
                                    {
                                        case DateRangeType.AfterOrEqualTo:

                                            if (datevalue1 == range.dateValue1 && range.mileagedateid != mileagedateid)
                                            {
                                                message = "The date you have entered is equal to an existing 'After' date range with the date " + ((DateTime)range.dateValue1).ToShortDateString() + ". Please select a different value.";
                                                return message;
                                            }
                                            break;

                                        case DateRangeType.Between:

                                            //if ((range.dateValue1 > datevalue1 && range.dateValue1 < datevalue2) && range.mileagedateid != mileagedateid)
                                            //{
                                            //    message = "An existing 'After' date range of date " + range.dateValue1.ToShortDateString() + " falls in your 'Between' date range.";
                                            //    return message;
                                            //}

                                            if (datevalue1 >= range.dateValue1 || datevalue2 >= range.dateValue1)
                                            {
                                                message = "Your 'Between' date range has values that fall within an existing 'After' date range of date " + ((DateTime)range.dateValue1).ToShortDateString();
                                                return message;
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                }
                                break;
                        }
                    }
                }
                else
                {
                    switch (range.daterangetype)
                    {
                        case DateRangeType.Between:
                            {

                                switch (daterangetype)
                                {
                                    case DateRangeType.Between:

                                        if (datevalue1 >= range.dateValue1 && datevalue1 <= range.dateValue2)
                                        {
                                            message = "Date value 1 you have entered falls within an existing 'Between' date range of " + ((DateTime)range.dateValue1).ToShortDateString() + " to " + ((DateTime)range.dateValue2).ToShortDateString() + ". Please select a different value.";
                                            return message;
                                        }

                                        if (datevalue2 >= range.dateValue1 && datevalue2 <= range.dateValue2)
                                        {
                                            message = "Date value 2 you have entered falls within an existing 'Between' date range of " + ((DateTime)range.dateValue1).ToShortDateString() + " to " + ((DateTime)range.dateValue2).ToShortDateString() + ". Please select a different value.";
                                            return message;
                                        }

                                        if (range.dateValue1 > datevalue1 && range.dateValue2 < datevalue2)
                                        {
                                            message = "A 'Between' date range of " + ((DateTime)range.dateValue1).ToShortDateString() + " to " + ((DateTime)range.dateValue2).ToShortDateString() + " exists within your new 'Between' date range. Please select different values.";
                                            return message;
                                        }
                                        break;

                                    case DateRangeType.AfterOrEqualTo:

                                        if (datevalue1 == range.dateValue2 || datevalue1 == range.dateValue1)
                                        {
                                            message = "Your 'After' value falls within an existing 'Between' date range of " + ((DateTime)range.dateValue1).ToShortDateString() + " to " + ((DateTime)range.dateValue2).ToShortDateString();
                                            return message;
                                        }
                                        break;

                                    case DateRangeType.Before:

                                        if (datevalue1 > range.dateValue1 && datevalue1 <= range.dateValue2)
                                        {
                                            message = "Date value 1 you have entered falls within an existing 'Between' date range of " + ((DateTime)range.dateValue1).ToShortDateString() + " to " + ((DateTime)range.dateValue2).ToShortDateString() + ". Please select a different value.";
                                            return message;
                                        }
                                        break;
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
                                        message = "The date you have entered is equal to an existing 'Before' date range with the date " + ((DateTime)range.dateValue1).ToShortDateString() + ". Please select a different value.";
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
                                        message = "Your 'Between' date range has values that fall within an existing 'Before' date range of date " + ((DateTime)range.dateValue1).ToShortDateString();
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
                                        message = "The date you have entered is equal to an existing 'After' date range with the date " + ((DateTime)range.dateValue1).ToShortDateString() + ". Please select a different value.";
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
                                        message = "Your 'Between' date range has values that fall within an existing 'After' date range of date " + ((DateTime)range.dateValue1).ToShortDateString();
                                        return message;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            DateTime? tempDateVal1 = null;
            DateTime? tempDateVal2 = null;

            switch (daterangetype)
            {
                case DateRangeType.Before:

                    foreach (cMileageDaterange range in reqmileage.dateRanges)
                    {
                        if (range.daterangetype == DateRangeType.Between)
                        {
                            if (tempDateVal1 == null)
                            {
                                tempDateVal1 = range.dateValue1;
                            }
                            else
                            {
                                if (tempDateVal1 > range.dateValue1)
                                {
                                    tempDateVal1 = range.dateValue1;
                                }
                            }
                        }
                    }
                    foreach (cMileageDaterange range in reqmileage.dateRanges)
                    {
                        if (range.daterangetype == DateRangeType.AfterOrEqualTo)
                        {
                            if (tempDateVal1 == null)
                            {
                                if (datevalue1 > range.dateValue1)
                                {
                                    message = "The 'Before' date you have entered cannot be greater than or equal to an existing 'After' date range of " + ((DateTime)range.dateValue1).ToShortDateString();
                                }
                            }
                            else
                            {
                                if (datevalue1 > tempDateVal1)
                                {
                                    message = "The 'Before' date you have entered cannot be greater than or equal to an existing 'After' date range of " + ((DateTime)range.dateValue1).ToShortDateString();
                                }
                            }

                        }

                        if (range.daterangetype == DateRangeType.Between)
                        {
                            if (tempDateVal1 != null)
                            {
                                if (datevalue1 > tempDateVal1)
                                {
                                    message = "The 'Before' date you have entered cannot fall between date ranges not covered";
                                }
                            }

                        }
                    }
                    break;

                case DateRangeType.AfterOrEqualTo:

                    foreach (cMileageDaterange range in reqmileage.dateRanges)
                    {
                        if (range.daterangetype == DateRangeType.Between)
                        {
                            if (tempDateVal2 == null)
                            {
                                tempDateVal2 = range.dateValue2;
                            }
                            else
                            {
                                if (tempDateVal2 <= range.dateValue2)
                                {
                                    tempDateVal2 = range.dateValue2;
                                }
                            }
                        }
                    }

                    foreach (cMileageDaterange range in reqmileage.dateRanges)
                    {
                        if (range.daterangetype == DateRangeType.Before)
                        {
                            if (datevalue1 < range.dateValue1)
                            {
                                message = "The 'After' date you have entered cannot be less than an existing 'Before' date range of " + ((DateTime)range.dateValue1).ToShortDateString();
                            }

                        }

                        if (range.daterangetype == DateRangeType.Between)
                        {
                            if (tempDateVal2 != null)
                            {
                                if (datevalue1 <= tempDateVal2)
                                {
                                    message = "The 'After' date you have entered cannot fall between date ranges not covered";
                                }
                            }

                        }
                    }
                    break;
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
                                        datevalue1 = ((DateTime)dtrange.dateValue2).AddDays(1);
                                    }
                                    else
                                    {
                                        datevalue1 = ((DateTime)oldrange.dateValue1).AddDays(1);
                                    }
                                }
                            }
                            else
                            {
                                //updateMileageDateRange(oldrange.mileagedateid, oldrange.mileageid, ((DateTime)datevalue1).AddDays(1), oldrange.dateValue1, DateRangeType.Between, employeeid, false);
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
                                        datevalue2 = ((DateTime)dtrange.dateValue2).AddDays(-1);
                                    }
                                    else
                                    {
                                        datevalue2 = ((DateTime)oldrange.dateValue1).AddDays(-1);
                                    }
                                }
                            }
                            else
                            {
                                //updateMileageDateRange(oldrange.mileagedateid, oldrange.mileageid, oldrange.dateValue2, ((DateTime)datevalue2).AddDays(-1), DateRangeType.Between, employeeid, false);
                            }
                            break;
                        }
                }
            }

            return message;
        }

        public virtual int saveDateRange(cMileageDaterange range, int mileageid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            int mileagedateid;
            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            data.sqlexecute.Parameters.AddWithValue("@mileagedateid", range.mileagedateid);
            data.sqlexecute.Parameters.AddWithValue("@mileageid", range.mileageid);
            data.sqlexecute.Parameters.AddWithValue("@daterangetype", (byte)range.daterangetype);
            if (range.dateValue1 == null)
            {
                data.sqlexecute.Parameters.AddWithValue("@datevalue1", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@datevalue1", range.dateValue1);
            }
            if (range.dateValue2 == null)
            {
                data.sqlexecute.Parameters.AddWithValue("@datevalue2", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@datevalue2", range.dateValue2);
            }
            if (range.mileagedateid > 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@userid", range.modifiedby);
                data.sqlexecute.Parameters.AddWithValue("@date", range.modifiedon);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@userid", range.createdby);
                data.sqlexecute.Parameters.AddWithValue("@date", range.createdon);
            }
            if (currentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeID", range.mileagedateid == 0 ? range.createdby : range.modifiedby);
                data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveVehicleJourneyRateDateRange");
            mileagedateid = (int)data.sqlexecute.Parameters["@identity"].Value;
            Cache.Delete(this.accountid, CacheArea, mileageid.ToString());

            data.sqlexecute.Parameters.Clear();
            return mileagedateid;
        }

        public int saveThreshold(int mileageId, cMileageThreshold threshold)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            int mileagethresholdid;
            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            data.sqlexecute.Parameters.AddWithValue("@mileagethresholdid", threshold.MileageThresholdId);
            data.sqlexecute.Parameters.AddWithValue("@mileagedateid", threshold.MileageDateRangeId);
            data.sqlexecute.Parameters.AddWithValue("@rangetype", (byte)threshold.RangeType);
            if (threshold.RangeValue1 == null)
            {
                data.sqlexecute.Parameters.AddWithValue("@rangevalue1", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@rangevalue1", threshold.RangeValue1);
            }
            if (threshold.RangeValue2 == null)
            {
                data.sqlexecute.Parameters.AddWithValue("@rangevalue2", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@rangevalue2", threshold.RangeValue2);
            }
            data.sqlexecute.Parameters.AddWithValue("@passenger1", threshold.Passenger);
            data.sqlexecute.Parameters.AddWithValue("@passengerx", threshold.PassengerX);
            data.sqlexecute.Parameters.AddWithValue("@heavyBulkyEquipment", threshold.HeavyBulkyEquipment);
            if (threshold.MileageThresholdId > 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@userid", threshold.ModifiedBy);
                data.sqlexecute.Parameters.AddWithValue("@date", threshold.ModifiedOn);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@userid", threshold.CreatedBy);
                data.sqlexecute.Parameters.AddWithValue("@date", threshold.CreatedOn);
            }
            if (currentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeID", threshold.MileageThresholdId == 0 ? threshold.CreatedBy : threshold.ModifiedBy);
                data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveVehicleJourneyRateTreshold");
            mileagethresholdid = (int)data.sqlexecute.Parameters["@identity"].Value;
            data.sqlexecute.Parameters.Clear();

            this.DeleteCache(mileageId);

            return mileagethresholdid;
        }


        public void deleteMileageDateRange(int mileageid, int mileagedateid)
        {
            var reqmileage = GetMileageCatById(mileageid);
            var daterange = getMileageDateRangeById(reqmileage, mileagedateid);

            foreach (var threshold in daterange.thresholds)
            {
                deleteMileageThreshold(threshold.MileageThresholdId, mileageid);
            }

            var currentUser = cMisc.GetCurrentUser();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@mileagedateid", mileagedateid);
                expdata.ExecuteProc("dbo.DeleteMileageDateRange");
                expdata.sqlexecute.Parameters.Clear();
            }

            this.DeleteCache(mileageid);
        }

        #endregion

        #region Mileage Thresholds

        public SortedList<int, List<cMileageThreshold>> getMileageThresholds()
        {
            SortedList<int, List<cMileageThreshold>> lst = new SortedList<int, List<cMileageThreshold>>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            List<cMileageThreshold> lstThresholds;

            DateTime createdon, modifiedon;
            int mileagethresholdid, createdby, modifiedby;
            int mileagedateid;
            decimal passenger, passengerx, heavyBulkyEquipment,rangeValue1, rangeValue2;
            RangeType rangetype;

            cMileageThreshold reqthreshold;

            using (SqlDataReader reader = expdata.GetProcReader("GetMileageThresholds"))
            {

                while (reader.Read())
                {
                    mileagethresholdid = reader.GetInt32(reader.GetOrdinal("mileagethresholdid"));
                    mileagedateid = reader.GetInt32(reader.GetOrdinal("mileagedateid"));
                    rangetype = (RangeType)reader.GetByte(reader.GetOrdinal("rangetype"));

                    if (reader.IsDBNull(reader.GetOrdinal("rangevalue1")) == true)
                    {
                        rangeValue1 = 0;
                    }
                    else
                    {
                        rangeValue1 = reader.GetDecimal(reader.GetOrdinal("rangevalue1"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("rangevalue2")) == true)
                    {
                        rangeValue2 = 0;
                    }
                    else
                    {
                        rangeValue2 = reader.GetDecimal(reader.GetOrdinal("rangevalue2"));
                    }

                    passenger = reader.GetDecimal(reader.GetOrdinal("passenger1"));
                    passengerx = reader.GetDecimal(reader.GetOrdinal("passengerx"));
                    heavyBulkyEquipment = reader.GetDecimal(reader.GetOrdinal("heavyBulkyEquipment"));
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
                    lst.TryGetValue(mileagedateid, out lstThresholds);
                    if (lstThresholds == null)
                    {
                        lstThresholds = new List<cMileageThreshold>();
                        lst.Add(mileagedateid, lstThresholds);
                    }
                    reqthreshold = new cMileageThreshold(mileagethresholdid, mileagedateid, rangeValue1, rangeValue2, rangetype, passenger, passengerx, createdon, createdby, modifiedon, modifiedby, heavyBulkyEquipment);
                    lstThresholds.Add(reqthreshold);
                }
                expdata.sqlexecute.Parameters.Clear();
                reader.Close();
            }

            return lst;
        }

        public cMileageThreshold getMileageThreshold(cMileageDaterange range, decimal mileagetotal)
        {
            cMileageThreshold reqthreshold = null;

            foreach (cMileageThreshold threshold in range.thresholds)
            {
                switch (threshold.RangeType)
                {
                    case RangeType.Any:
                        {
                            reqthreshold = threshold;
                            break;
                        }
                    case RangeType.GreaterThanOrEqualTo:
                        {
                            if (mileagetotal >= threshold.RangeValue1)
                            {
                                reqthreshold = threshold;
                            }
                            break;
                        }
                    case RangeType.LessThan:
                        {
                            if (mileagetotal < threshold.RangeValue1)
                            {
                                reqthreshold = threshold;
                            }
                            break;
                        }
                    case RangeType.Between:
                        {
                            if (mileagetotal >= threshold.RangeValue1 && mileagetotal <= threshold.RangeValue2)
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
                if (threshold.MileageThresholdId == mileagethresholdid)
                {
                    mileagethreshold = threshold;
                }
            }

            return mileagethreshold;
        }

        public string mileageThresholdExists(int mileageid, int mileagedateid, int mileagethresholdid, ref RangeType rangetype, ref decimal? rangevalue1, ref decimal? rangevalue2, int employeeid)
        {
            string message = "";
            cMileageCat reqmileage = GetMileageCatById(mileageid);
            cMileageDaterange daterange = getMileageDateRangeById(reqmileage, mileagedateid);

            cMileageThreshold oldthreshold = null;
            bool keepExistingThreshold = false; ;

            if (daterange != null)
            {
                if (daterange.thresholds.Count >= 1 && rangetype == RangeType.Any && mileagethresholdid == 0)
                {
                    message = "You cannot add a threshold of type 'Any' if other thresholds already exist for this date range.";
                    return message;
                }
            }

            if (rangetype == RangeType.Between)
            {
                if (rangevalue2 < rangevalue1)
                {
                    message = "Threshold value 2 must be greater than threshold value 1.";
                    return message;
                }
            }



            foreach (cMileageThreshold threshold in daterange.thresholds)
            {
                if (mileagethresholdid > 0)
                {
                    if (threshold.MileageThresholdId != mileagethresholdid)
                    {
                        switch (threshold.RangeType)
                        {
                            case RangeType.Between:
                                {
                                    if ((rangevalue1 > threshold.RangeValue1 && rangevalue1 <= threshold.RangeValue2) && threshold.MileageThresholdId != mileagethresholdid)
                                    {
                                        message = "'Threshold value 1' you have entered falls within an existing 'Between' range of " + threshold.RangeValue1.ToString() + " to " + threshold.RangeValue2.ToString() + ". Please select a different value.";
                                        return message;
                                    }

                                    if (rangetype == RangeType.Between)
                                    {
                                        if ((rangevalue2 >= threshold.RangeValue1 && rangevalue2 <= threshold.RangeValue2) && threshold.MileageThresholdId != mileagethresholdid)
                                        {
                                            message = "'Threshold value 2' you have entered falls within an existing 'Between' range of " + threshold.RangeValue1.ToString() + " to " + threshold.RangeValue2.ToString() + ". Please select a different value.";
                                            return message;
                                        }

                                        if ((threshold.RangeValue1 > rangevalue1 && threshold.RangeValue2 < rangevalue2) && threshold.MileageThresholdId != mileagethresholdid)
                                        {
                                            message = "A 'Between' range of " + threshold.RangeValue1.ToString() + " to " + threshold.RangeValue2.ToString() + " exists within your new 'Between' range. Please select different values.";
                                        }
                                    }

                                    if (rangetype == RangeType.GreaterThanOrEqualTo)
                                    {
                                        if (rangevalue1 == threshold.RangeValue2 || rangevalue1 == threshold.RangeValue1)
                                        {
                                            message = "Your 'Greater than or equal to' value falls within an existing 'Between' range of " + threshold.RangeValue1.ToString() + " to " + threshold.RangeValue2.ToString() + ". Please select a different value.";
                                            return message;
                                        }
                                    }
                                    break;
                                }
                            case RangeType.LessThan:
                                {
                                    if (rangetype == RangeType.LessThan)
                                    {
                                        if ((rangevalue1 == threshold.RangeValue1) && threshold.MileageThresholdId != mileagethresholdid)
                                        {
                                            message = "The value you have entered is equal to an existing 'Less than' range of " + threshold.RangeValue1.ToString() + ". Please select a different value.";
                                            return message;
                                        }
                                    }

                                    if (rangetype == RangeType.Between)
                                    {
                                        //if ((threshold.rangeValue1 > rangevalue1 && threshold.rangeValue1 < rangevalue2) && threshold.mileageThresholdid != mileagethresholdid)
                                        //{
                                        //    message = "An existing 'Less than' range of " + threshold.rangeValue1.ToString() + " falls in your 'Between' range.";
                                        //    return message;
                                        //}

                                        if (rangevalue1 < threshold.RangeValue1 || rangevalue2 < threshold.RangeValue1)
                                        {
                                            message = "Your 'Between' range has values that fall within an existing 'Less than' range of " + threshold.RangeValue1.ToString();
                                            return message;
                                        }
                                    }

                                    break;
                                }
                            case RangeType.GreaterThanOrEqualTo:
                                {
                                    if (rangetype == RangeType.GreaterThanOrEqualTo)
                                    {
                                        if (rangevalue1 == threshold.RangeValue1 && threshold.MileageThresholdId != mileagethresholdid)
                                        {
                                            message = "The value you have entered is equal to an existing 'Greater than' range of " + threshold.RangeValue1.ToString() + ". Please select a different value.";
                                            return message;
                                        }
                                    }

                                    if (rangetype == RangeType.Between)
                                    {
                                        //if ((threshold.rangeValue1 > rangevalue1 && threshold.rangeValue1 < rangevalue2) && threshold.mileageThresholdid != mileagethresholdid)
                                        //{
                                        //    message = "An existing 'Greater than' range of " + threshold.rangeValue1.ToString() + " falls in your 'Between' range.";
                                        //    return message;
                                        //}

                                        if (rangevalue1 >= threshold.RangeValue1 || rangevalue2 >= threshold.RangeValue1)
                                        {
                                            message = "Your 'Between' range has values that fall within an existing 'Greater than' range of " + threshold.RangeValue1.ToString();
                                            return message;
                                        }
                                    }

                                }
                                break;
                        }
                    }
                }
                else
                {
                    switch (threshold.RangeType)
                    {
                        case RangeType.Between:
                            {
                                switch (rangetype)
                                {
                                    case RangeType.Between:

                                        if (rangevalue1 >= threshold.RangeValue1 && rangevalue1 <= threshold.RangeValue2)
                                        {
                                            message = "'Threshold value 1' you have entered falls within an existing 'Between' range of " + threshold.RangeValue1.ToString() + " to " + threshold.RangeValue2.ToString() + ". Please select a different value.";
                                            return message;
                                        }

                                        if (rangevalue2 >= threshold.RangeValue1 && rangevalue2 <= threshold.RangeValue2)
                                        {
                                            message = "'Threshold value 2' you have entered falls within an existing 'Between' range of " + threshold.RangeValue1.ToString() + " to " + threshold.RangeValue2.ToString() + ". Please select a different value.";
                                            return message;
                                        }

                                        if (threshold.RangeValue1 > rangevalue1 && threshold.RangeValue2 < rangevalue2)
                                        {
                                            message = "A 'Between' range of " + threshold.RangeValue1.ToString() + " to " + threshold.RangeValue2.ToString() + " exists within your new 'Between' range. Please select different values.";
                                            return message;
                                        }
                                        break;

                                    case RangeType.GreaterThanOrEqualTo:

                                        if (rangevalue1 == threshold.RangeValue2 || rangevalue1 == threshold.RangeValue1)
                                        {
                                            message = "Your 'Greater than or equal to' value falls within an existing 'Between' range of " + threshold.RangeValue1.ToString() + " to " + threshold.RangeValue2.ToString() + ". Please select a different value.";
                                            return message;
                                        }
                                        break;

                                    case RangeType.LessThan:

                                        if (rangevalue1 > threshold.RangeValue1 && rangevalue1 <= threshold.RangeValue2)
                                        {
                                            message = "'Threshold value 1' you have entered falls within an existing 'Between' range of " + threshold.RangeValue1.ToString() + " to " + threshold.RangeValue2.ToString() + ". Please select a different value.";
                                            return message;
                                        }
                                        break;
                                }

                                break;
                            }
                        case RangeType.LessThan:
                            {
                                if (rangetype == RangeType.LessThan)
                                {
                                    oldthreshold = threshold;

                                    if (rangevalue1 > threshold.RangeValue1)
                                    {
                                        keepExistingThreshold = true;
                                    }
                                    else if (rangevalue1 < threshold.RangeValue1)
                                    {
                                        keepExistingThreshold = false;
                                    }
                                    else if (rangevalue1 == threshold.RangeValue1)
                                    {
                                        message = "The value you have entered is equal to an existing 'Less Than' range of " + threshold.RangeValue1.ToString() + ". Please select a different value.";
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

                                    if (rangevalue1 < threshold.RangeValue1 || rangevalue2 < threshold.RangeValue1)
                                    {
                                        message = "Your 'Between' range has values that fall within an existing 'Less than' range of " + threshold.RangeValue1.ToString();
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

                                    if (rangevalue1 < threshold.RangeValue1)
                                    {
                                        keepExistingThreshold = true;
                                    }
                                    else if (rangevalue1 > threshold.RangeValue1)
                                    {
                                        keepExistingThreshold = false;
                                    }
                                    else if (rangevalue1 == threshold.RangeValue1)
                                    {
                                        message = "The value you have entered is equal to an existing 'Greater than' range of " + threshold.RangeValue1.ToString() + ". Please select a different value.";
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

                                    if (rangevalue1 > threshold.RangeValue1 || rangevalue2 > threshold.RangeValue1)
                                    {
                                        message = "Your 'Between' range has values that fall within an existing 'Greater than' range of " + threshold.RangeValue1.ToString();
                                        return message;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            decimal? tempVal1 = null;
            decimal? tempVal2 = null;

            switch (rangetype)
            {
                case RangeType.LessThan:

                    foreach (cMileageThreshold threshold in daterange.thresholds)
                    {
                        if (threshold.RangeType == RangeType.Between)
                        {
                            if (tempVal1 == null)
                            {
                                tempVal1 = threshold.RangeValue1;
                            }
                            else
                            {
                                if (tempVal1 > threshold.RangeValue1)
                                {
                                    tempVal1 = threshold.RangeValue1;
                                }
                            }
                        }
                    }

                    foreach (cMileageThreshold threshold in daterange.thresholds)
                    {
                        if (threshold.RangeType == RangeType.GreaterThanOrEqualTo)
                        {
                            if (tempVal1 == null)
                            {
                                if (rangevalue1 > threshold.RangeValue1)
                                {
                                    message = "The 'Less Than' threshold you have entered cannot be greater than or equal to an existing 'Greater Than' threshold range of " + threshold.RangeValue1.ToString();
                                }
                            }
                            else
                            {
                                if (rangevalue1 > tempVal1)
                                {
                                    message = "The 'Less Than' threshold you have entered cannot be greater than or equal to an existing 'Greater Than' threshold range of " + threshold.RangeValue1.ToString();
                                }
                            }

                        }

                        if (threshold.RangeType == RangeType.Between)
                        {
                            if (tempVal1 != null)
                            {
                                if (rangevalue1 > tempVal1)
                                {
                                    message = "The 'Less Than' threshold you have entered cannot fall between threshold ranges not covered";
                                }
                            }

                        }
                    }
                    break;

                case RangeType.GreaterThanOrEqualTo:

                    foreach (cMileageThreshold threshold in daterange.thresholds)
                    {
                        if (threshold.RangeType == RangeType.Between)
                        {
                            if (tempVal2 == null)
                            {
                                tempVal2 = threshold.RangeValue2;
                            }
                            else
                            {
                                if (tempVal2 <= threshold.RangeValue2)
                                {
                                    tempVal2 = threshold.RangeValue2;
                                }
                            }
                        }
                    }

                    foreach (cMileageThreshold threshold in daterange.thresholds)
                    {
                        if (threshold.RangeType == RangeType.LessThan)
                        {
                            if (rangevalue1 < threshold.RangeValue1)
                            {
                                message = "The 'Greater Than' threshold you have entered cannot be less than an existing 'Less Than' threshold range of " + threshold.RangeValue1.ToString();
                            }

                        }

                        if (threshold.RangeType == RangeType.Between)
                        {
                            if (tempVal2 != null)
                            {
                                if (rangevalue1 <= tempVal2)
                                {
                                    message = "The 'Greater Than or Equal to' threshold you have entered cannot fall between threshold ranges not covered";
                                }
                            }

                        }
                    }
                    break;
            }

            if (oldthreshold != null)
            {
                switch (oldthreshold.RangeType)
                {
                    case RangeType.LessThan:
                        {
                            if (keepExistingThreshold)
                            {
                                rangetype = RangeType.Between;
                                rangevalue2 = rangevalue1;

                                foreach (cMileageThreshold cThreshold in daterange.thresholds)
                                {
                                    if (cThreshold.RangeType == RangeType.Between && cThreshold.RangeValue2 > oldthreshold.RangeValue1)
                                    {
                                        rangevalue1 = ((int)cThreshold.RangeValue2 + 1);
                                    }
                                    else
                                    {
                                        rangevalue1 = ((int)oldthreshold.RangeValue1 + 1);
                                    }
                                }
                            }
                            else
                            {
                                //updateMileageThreshold(oldthreshold.mileageThresholdid, oldthreshold.mileageDateid, mileageid,RangeType.Between, ((int)rangevalue1 + 1), (int)oldthreshold.rangeValue1, 0, 0, 0, 0, 0, 0, 0, 0, employeeid, false);
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
                                    if (cThreshold.RangeType == RangeType.Between && cThreshold.RangeValue1 < oldthreshold.RangeValue2)
                                    {
                                        rangevalue2 = ((int)cThreshold.RangeValue2 - 1);
                                    }
                                    else
                                    {
                                        rangevalue2 = ((int)oldthreshold.RangeValue1 - 1);
                                    }
                                }
                            }
                            else
                            {
                                //updateMileageThreshold(oldthreshold.mileageThresholdid, oldthreshold.mileageDateid, mileageid, RangeType.Between, (int)oldthreshold.rangeValue2, ((int)rangevalue2 - 1), 0, 0, 0, 0, 0, 0, 0, 0, employeeid, false);
                            }
                            break;
                        }
                }
            }
            return message;
        }


        public void deleteMileageThreshold(int mileagethresholdid, int mileageid)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            using (var data = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
                data.sqlexecute.Parameters.AddWithValue("@mileagethresholdid", mileagethresholdid);
                data.ExecuteProc("deleteVehicleJourneyRateThreshold");
            }

            this.DeleteCache(mileageid);
        }

        #endregion


        /// <summary>
        /// Calculate the expense item value of the given journey.
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <param name="subcat">The expense item sub-category <see cref="cSubcat"/></param>
        /// <param name="expenseitem">The current expense item <see cref="cExpenseItem"/></param>
        /// <param name="employeeId">The claimant</param>
        /// <param name="vatCalculation">An instance of <see cref="cVat"/></param>
        /// <param name="thresholdtype">The threshold type. <see cref="ThresholdType"/></param>
        /// <returns></returns>
        public decimal CalculateVehicleJourneyTotal(ICurrentUserBase currentUser, cSubcat subcat, cExpenseItem expenseitem, int employeeId, cVat vatCalculation, ThresholdType thresholdtype)
        {
            cEmployees clsemployees = new cEmployees(accountid);
            VehicleJourneyRateThresholdRate publicTransportRate = null;
            

            decimal fuelcost = 0;
            decimal totalmiles = 0;
            decimal pencepermile = 0;
            decimal total = 0;
            decimal grandtotal = 0;
            decimal vat = 0;
            VehicleJourneyRateThresholdRate lastRate = null;
            cMileageCat reqmileage;
            var reqcar = GetCarForMileage(expenseitem, employeeId);
            var vehicleEngineTypeId = reqcar.VehicleEngineTypeId;
            var mileagecats = new cMileagecats(accountid);
            reqmileage = mileagecats.GetMileageCatById(expenseitem.mileageid);

            if (subcat.HomeToLocationType == HomeToLocationType.JuniorDoctorRotation && subcat.PublicTransportRate.HasValue)
            {
                var publicTransportMileageCat = mileagecats.GetMileageCatById(subcat.PublicTransportRate.Value);
                var officialJourney = expenseitem.journeysteps.Values.Any(X => X.OfficialJourney);
                if (!officialJourney)
                {
                    reqmileage = publicTransportMileageCat;
                }
                else
                {
                    var publicTransportRateDateRange = mileagecats.getMileageThreshold(mileagecats.getMileageDateRange(publicTransportMileageCat, expenseitem.date), totalmiles);
                    publicTransportRate = VehicleJourneyRateThresholdRate.Get(currentUser, publicTransportRateDateRange.MileageThresholdId, vehicleEngineTypeId);
                }
            }

            cMileageDaterange clsdaterange = mileagecats.getMileageDateRange(reqmileage, expenseitem.date);

            cMileageThreshold clsthreshold = null;
            if (thresholdtype == ThresholdType.Annual)
            {
                totalmiles = clsemployees.getMileageTotal(employeeId, expenseitem.date);
                clsthreshold = mileagecats.getMileageThreshold(clsdaterange, totalmiles);
            }

            cMileageThreshold threshold = null;
            decimal below = 0;
            decimal oldmiles = 0;

            decimal distanceCalculated = 0;
            bool updateOldMiles = true;

            foreach (cJourneyStep step in expenseitem.journeysteps.Values)
            {
                if (thresholdtype == ThresholdType.Journey)
                {
                    // Set the distance calculated so far, before totalmiles is updated
                    distanceCalculated = totalmiles;
                }

                oldmiles = totalmiles;
                totalmiles += step.nummiles;

                bool leave = false;
                var count = SetStartingThreshold(thresholdtype, clsdaterange, clsthreshold);
                for (int i = count; i < clsdaterange.thresholds.Count; i++)
                {
                    updateOldMiles = true;
                    threshold = clsdaterange.thresholds[i];
                    var rate = VehicleJourneyRateThresholdRate.Get(currentUser, threshold.MileageThresholdId, vehicleEngineTypeId);
                    lastRate = rate;
                    pencepermile = this.GetPencePerDistanceForThreshold(subcat, reqmileage, threshold, step, rate, publicTransportRate);
                    fuelcost = (rate == null ? 0 : rate.AmountForVat ?? 0);

                    switch (threshold.RangeType)
                    {
                        case RangeType.LessThan:

                            if (thresholdtype == ThresholdType.Annual || distanceCalculated < threshold.RangeValue1)
                            {
                                if (totalmiles >= threshold.RangeValue1)
                                {
                                    below = threshold.RangeValue1.Value - oldmiles;

                                    total = (below * pencepermile);
                                    grandtotal += total;
                                    vatCalculation.calculateVehicleJourneyVAT(subcat, ref vat, pencepermile, fuelcost, total);

                                }
                                else
                                {
                                    total = ((totalmiles - oldmiles) * pencepermile);
                                    oldmiles += (totalmiles - oldmiles);
                                    grandtotal += total;
                                    vatCalculation.calculateVehicleJourneyVAT(subcat, ref vat, pencepermile, fuelcost, total);

                                    leave = true;
                                }
                            }
                            else
                            {
                                updateOldMiles = false;
                            }
                            break;

                        case RangeType.Between:

                            if (thresholdtype == ThresholdType.Annual || distanceCalculated < threshold.RangeValue2)
                            {
                                if (totalmiles > threshold.RangeValue2)
                                {
                                    below = (int)threshold.RangeValue2 - oldmiles;

                                    total = (below * pencepermile);
                                    grandtotal += total;
                                    vatCalculation.calculateVehicleJourneyVAT(subcat, ref vat, pencepermile, fuelcost, total);

                                }
                                else
                                {
                                    total = ((totalmiles - oldmiles) * pencepermile);
                                    oldmiles += (totalmiles - oldmiles);
                                    grandtotal += total;
                                    vatCalculation.calculateVehicleJourneyVAT(subcat, ref vat, pencepermile, fuelcost, total);
                                    leave = true;
                                }
                            }
                            else
                            {
                                updateOldMiles = false;
                            }
                            break;
                        case RangeType.GreaterThanOrEqualTo:

                            total = ((totalmiles - oldmiles) * pencepermile);
                            oldmiles += (totalmiles - oldmiles);
                            grandtotal += total;
                            vatCalculation.calculateVehicleJourneyVAT(subcat, ref vat, pencepermile, fuelcost, total);

                            leave = true;
                            break;
                        case RangeType.Any:

                            total = ((totalmiles - oldmiles) * pencepermile);
                            oldmiles += (totalmiles - oldmiles);
                            grandtotal += total;
                            vatCalculation.calculateVehicleJourneyVAT(subcat, ref vat, pencepermile, fuelcost, total);

                            leave = true;
                            break;
                    }

                    if (leave)
                    {
                        break;
                    }

                    if (updateOldMiles)
                    {
                        oldmiles += below;
                    }
                }
                clsthreshold = threshold;
            }

            if (subcat.HomeToLocationType == HomeToLocationType.JuniorDoctorRotation)
            {
                var subAccount = new cAccountSubAccounts(currentUser.AccountID).getSubAccountById(currentUser.CurrentSubAccountId);
                cAccountProperties subAccountProperties = subAccount.SubAccountProperties;
                var employee = Employee.Get(employeeId, this.accountid);
                var workAddress = employee.GetWorkAddresses().GetBy(currentUser, expenseitem.date, (int)expenseitem.ESRAssignmentId);
                var homeAddress = employee.GetHomeAddresses().GetBy(expenseitem.date);
                var distance = new JourneyDistances(currentUser, subAccountProperties.UseMapPoint, subAccountProperties.MileageCalcType);
                this.AddFullRateMileageToJourney(employeeId, expenseitem, lastRate, publicTransportRate, ref grandtotal, ref vat, updateOldMiles, subcat, vatCalculation, subAccountProperties, workAddress, homeAddress, distance);
            }

            grandtotal = Math.Round(grandtotal, 2, MidpointRounding.AwayFromZero);

            vat = Math.Round(vat, 2, MidpointRounding.AwayFromZero);

            // Calculate the NET from the total and VAT to avoid rounding issues
            expenseitem.updateVAT(grandtotal - vat, vat, grandtotal);

            return grandtotal;
        }

        /// <summary>
        /// For junior doctor home to office rule, if official journey, add back the mileage from base to last official place times two as as additional.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="expenseitem"></param>
        /// <param name="lastRate"></param>
        /// <param name="publicTransportRate"></param>
        /// <param name="grandtotal"></param>
        /// <param name="vat"></param>
        /// <param name="updateOldMiles"></param>
        /// <param name="subcat"></param>
        /// <param name="vatCalculation"></param>
        /// <param name="homeAddress"></param>
        /// <param name="distance"></param>
        /// <param name="subAccountProperties"></param>
        /// <param name="workAddress"></param>
        internal void AddFullRateMileageToJourney(int employeeId, cExpenseItem expenseitem, VehicleJourneyRateThresholdRate lastRate, VehicleJourneyRateThresholdRate publicTransportRate, ref decimal grandtotal, ref decimal vat, bool updateOldMiles, cSubcat subcat, IVat vatCalculation, cAccountProperties subAccountProperties, cEmployeeWorkLocation workAddress, cEmployeeHomeLocation homeAddress, IJourneyDistances distance)
        {
            if (expenseitem == null || expenseitem.journeysteps.Count == 0 || expenseitem.homeToOfficeDeductionMethod != HomeToLocationType.JuniorDoctorRotation)
            {
                return;
            }

            if (publicTransportRate == null || lastRate == null ||  homeAddress == null || workAddress == null)
            {
                return;
            }

            if (lastRate.MileageThresholdRateId == publicTransportRate.MileageThresholdRateId)
            {
                return;
            }

            if (publicTransportRate.RatePerUnit != null)
            {
                var additionalRate = publicTransportRate.RatePerUnit.Value;

                var lastOfficialStep = expenseitem.journeysteps.Values.LastOrDefault(x => x.OfficialJourney);

                foreach (cJourneyStep journeyStep in expenseitem.journeysteps.Values.Where(step => step.OfficialJourney))
                {
                    if (journeyStep.startlocation.Identifier == homeAddress.LocationID || journeyStep.endlocation.Identifier == homeAddress.LocationID)
                    {
                        this.CalculateHomeStepForJuniorDoctorMileage(expenseitem, ref grandtotal, ref vat, updateOldMiles, subcat, vatCalculation, workAddress, homeAddress, distance, lastOfficialStep, additionalRate);
                    }
                    else
                    {
                        var additionalValue = journeyStep.nummiles * additionalRate;
                        grandtotal += additionalValue;
                    }
                }
            }
        }

        /// <summary>
        /// When calculating Junior Doctor Home to office, the last step has different rules.
        /// Only allow higher rate for the distance between the last official step and the current work address.  
        /// The rest of this step is at the lower rate.
        /// </summary>
        /// <param name="expenseitem">The current expense item</param>
        /// <param name="grandtotal">The current claim total</param>
        /// <param name="vat">The current vat total</param>
        /// <param name="updateOldMiles">Wether to update "old miles"</param>
        /// <param name="subcat">The current sub cat</param>
        /// <param name="vatCalculation">a IVat object</param>
        /// <param name="workAddress">The employees current work address</param>
        /// <param name="homeAddress">The employees current home address</param>
        /// <param name="distance">An IJourneyDistance object</param>
        /// <param name="lastOfficialStep">The step that has the last "official" flag set.</param>
        /// <param name="additionalRate">The value to be used to add additiional value to the last step.    </param>
        internal void CalculateHomeStepForJuniorDoctorMileage(
            cExpenseItem expenseitem,
            ref decimal grandtotal,
            ref decimal vat,
            bool updateOldMiles,
            cSubcat subcat,
            IVat vatCalculation,
            cEmployeeWorkLocation workAddress,
            cEmployeeHomeLocation homeAddress,
            IJourneyDistances distance,
            cJourneyStep lastOfficialStep,
            decimal additionalRate)
        {
            var lastOfficialAddressId = 0;
            if (lastOfficialStep.endlocation.Identifier != homeAddress.LocationID
                && lastOfficialStep.endlocation.Identifier != workAddress.LocationID)
            {
                lastOfficialAddressId = lastOfficialStep.endlocation.Identifier;
            }
            else
            {
                lastOfficialAddressId = lastOfficialStep.startlocation.Identifier;
            }

            var workToOfficialDistance = workAddress == null
                                             ? null
                                             : distance.GetRecommendedOrCustomDistance(
                                                this._addresses.GetAddressById(workAddress.LocationID),
                                                this._addresses.GetAddressById(lastOfficialAddressId));
            if (workToOfficialDistance.HasValue)
            {
                var maximumMileageAtFullRate = workToOfficialDistance.Value;
                var totalMileage = lastOfficialStep.nummiles;
                if (maximumMileageAtFullRate > totalMileage)
                {
                    maximumMileageAtFullRate = totalMileage;
                }

                var additionalValue = maximumMileageAtFullRate * additionalRate;
                grandtotal += additionalValue;
                if (!updateOldMiles)
                {
                    var fuelcost = 0M;
                    vatCalculation.calculateVehicleJourneyVAT(subcat, ref vat, additionalRate, fuelcost, additionalValue);
                }
            }
        }

        /// <summary>
        /// Return the pence per (mile or kilometer) rate for this step in this threshold.
        /// </summary>
        /// <param name="subcat">the current sub cat</param>
        /// <param name="mileage">The mileage category</param>
        /// <param name="threshold">The applicable mileage threshold</param>
        /// <param name="step">The current step</param>
        /// <param name="rate">The threshold rate</param>
        /// <param name="publicTransportRate">The public transport rate (if any) for Junior doctors home to office.</param>
        /// <returns>The Pence per distance for the given parameters.</returns>
        internal decimal GetPencePerDistanceForThreshold(cSubcat subcat,  cMileageCat mileage, cMileageThreshold threshold, cJourneyStep step, VehicleJourneyRateThresholdRate rate, VehicleJourneyRateThresholdRate publicTransportRate)
        {
            decimal pencepermile = (rate == null ? 0 : rate.RatePerUnit ?? 0);
            pencepermile = JuniorDoctorHomeToOfficePencePerMileDeduction(publicTransportRate, pencepermile);

            pencepermile = AddHeavyEquipmentPencePerMile(subcat, threshold, step, pencepermile);

            pencepermile = AddPassengersPencePerMile(subcat, threshold, step, pencepermile);

            pencepermile = ConvertPencePerMileToPencePerKilometer(mileage, pencepermile);

            return pencepermile;
        }

        /// <summary>
        /// Convert miles to kilometer if required for the mileage category.
        /// </summary>
        /// <param name="mileage">The mileage category</param>
        /// <param name="pencepermile">The current pence per mile</param>
        /// <returns>Pence per mile or pence per kilometer.</returns>
        internal static decimal ConvertPencePerMileToPencePerKilometer(cMileageCat mileage, decimal pencepermile)
        {
            if (mileage.mileUom == MileageUOM.KM)
            {
                pencepermile = pencepermile * (decimal)1.609344; // pence per kilometre
            }
            return pencepermile;
        }

        /// <summary>
        /// Add passenger rate, if appropriate
        /// </summary>
        /// <param name="subcat">The current sub cat</param>
        /// <param name="threshold">The current mileage threshold</param>
        /// <param name="step">The current step</param>
        /// <param name="pencepermile">The pence per mile for this step</param>
        /// <returns>The adjusted pence per mile.</returns>
        internal static decimal AddPassengersPencePerMile(
            cSubcat subcat,
            cMileageThreshold threshold,
            cJourneyStep step,
            decimal pencepermile)
        {
            if (subcat == null || threshold == null || step == null)
            {
                return pencepermile;
            }

            if (subcat.calculation == CalculationType.PencePerMile && subcat.passengersapp && step.numpassengers >= 1)
            {
                pencepermile = pencepermile + threshold.Passenger + (threshold.PassengerX * (step.numpassengers - 1));
            }
            return pencepermile;
        }

        /// <summary>
        /// Add heavy equipment mileage if required.
        /// </summary>
        /// <param name="subcat">The current sub cat</param>
        /// <param name="threshold">The current mileage threshold</param>
        /// <param name="step">The current step</param>
        /// <param name="pencepermile">The current pence per mile</param>
        /// <returns>The adjusted pence per mile</returns>
        internal static decimal AddHeavyEquipmentPencePerMile(
            cSubcat subcat,
            cMileageThreshold threshold,
            cJourneyStep step,
            decimal pencepermile)
        {
            if (subcat == null || threshold == null || step == null)
            {
                return pencepermile;
            }

            if (subcat.allowHeavyBulkyMileage && step.heavyBulkyEquipment)
            {
                pencepermile += threshold.HeavyBulkyEquipment;
            }

            return pencepermile;
        }

        /// <summary>
        /// Adjust the pence per mile for Junior Doctors home to office, if required.
        /// </summary>
        /// <param name="publicTransportRate">The pulic transport rate (if any)</param>
        /// <param name="pencepermile">The current pence per mile</param>
        /// <returns>The adjusted pence per mile.</returns>
        internal static decimal JuniorDoctorHomeToOfficePencePerMileDeduction(VehicleJourneyRateThresholdRate publicTransportRate, decimal pencepermile)
        {
            if (publicTransportRate != null && publicTransportRate.RatePerUnit != null && pencepermile > publicTransportRate.RatePerUnit)
            {
                pencepermile -= publicTransportRate.RatePerUnit.Value;
            }

            return pencepermile;
        }

        /// <summary>
        /// Iterate through the thresholds until the date range threshold is equal to the given threshold.
        /// </summary>
        /// <param name="thresholdtype">If annual, look for the matching threshold.</param>
        /// <param name="daterange">The current VJR date range</param>
        /// <param name="threshold">The threshold to match</param>
        /// <returns></returns>
        internal int SetStartingThreshold(ThresholdType thresholdtype, cMileageDaterange daterange, cMileageThreshold threshold)
        {
            var count = 0;
            if (thresholdtype == ThresholdType.Annual)
            {
                foreach (cMileageThreshold reqthreshold in daterange.thresholds)
                {

                    if (reqthreshold == threshold)
                    {
                        break;
                    }
                    count++;
                }
            }

            return count;
        }


        /// <summary>
        /// Compare the current running total (or journey) to the mileage threshold.
        /// </summary>
        /// <param name="totalmiles">The current mileage total</param>
        /// <param name="rangeValue">The value to compare against</param>
        /// <param name="below">Returns the mileage below the threshold (if any)</param>
        /// <param name="total">Returns the value of the expense item for this step</param>
        /// <param name="grandtotal">Returns the grand total of mileage for this journey so far</param>
        /// <param name="oldmiles">The previous grand total (without the current step)</param>
        /// <param name="pencepermile">The prevailing pence per mile</param>
        /// <returns>True if the calling method should stop processing threshold levels.</returns>
        internal bool CompareMileageToCurrentThreshold(decimal totalmiles, decimal rangeValue, out decimal below, out decimal total, ref decimal grandtotal, ref decimal oldmiles, ref decimal pencepermile)
        {
            var leave = false;
            below = 0;
            if (totalmiles >= rangeValue)
            {
                below = rangeValue - oldmiles;
                total = (below * pencepermile);
                grandtotal += total;
            }
            else
            {
                total = ((totalmiles - oldmiles) * pencepermile);
                oldmiles += (totalmiles - oldmiles);
                grandtotal += total;
                leave = true;
            }

            return leave;
        }

        /// <summary>
        /// Get the car for this expense item.  Either via the expense item car or the default car for this employee.
        /// </summary>
        /// <param name="expenseitem">The current expense item</param>
        /// <param name="employeeId">The current employee ID</param>
        /// <returns>The selected car.</returns>
        private cCar GetCarForMileage(cExpenseItem expenseitem, int employeeId)
        {
            cEmployeeCars clsEmployeeCars = new cEmployeeCars(accountid, employeeId);
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(accountid);
            cAccountProperties clsProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties; // NEEDS SUBACCOUNTING

            var carid = expenseitem.carid;
            if (carid == 0)
            {
                carid = clsEmployeeCars.GetDefaultCarID(clsProperties.BlockTaxExpiry, clsProperties.BlockMOTExpiry, clsProperties.BlockInsuranceExpiry, clsProperties.BlockBreakdownCoverExpiry, clsProperties.DisableCarOutsideOfStartEndDate, expenseitem.date);
            }
            return clsEmployeeCars.GetCarByID(carid);
        }

        public decimal convertMilesToKM(decimal miles)
        {
            decimal kmValue = miles * (decimal)1.609344;
            return decimal.Parse(kmValue.ToString("#0.00"));
        }

        /// <summary>
        /// Convert a distance in miles to the equivalent distance in kilometres
        /// </summary>
        /// <param name="miles">The number of miles</param>
        /// <returns>The distance in kilometres</returns>
        public static decimal ConvertMilesToKilometres(decimal miles)
        {
            return (miles == 0m) ? 0m : miles * 1.609344m;
        }

        /// <summary>
        /// Convert a distance in kilometres to the equivalent distance in miles
        /// </summary>
        /// <param name="kilometres">The number of kilometres</param>
        /// <returns>The distance in miles</returns>
        public static decimal ConvertKilometresToMiles(decimal kilometres)
        {
            return (kilometres == 0m) ? 0m : kilometres / 1.609344m;
        }

        public virtual int saveVehicleJourneyRate(cMileageCat mileagecat)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            int mileageid;
            DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
            data.sqlexecute.Parameters.AddWithValue("@mileageid", mileagecat.mileageid);
            data.sqlexecute.Parameters.AddWithValue("@carsize", mileagecat.carsize);
            data.sqlexecute.Parameters.AddWithValue("@comment", mileagecat.comment);
            data.sqlexecute.Parameters.AddWithValue("@calcmilestotal", Convert.ToByte(mileagecat.calcmilestotal));
            data.sqlexecute.Parameters.AddWithValue("@thresholdtype", (byte)mileagecat.thresholdType);
            data.sqlexecute.Parameters.AddWithValue("@unit", (byte)mileagecat.mileUom);
            data.sqlexecute.Parameters.AddWithValue("@currencyid", mileagecat.currencyid);
            data.sqlexecute.Parameters.AddWithValue("@userratetable", mileagecat.UserRatestable);
            data.sqlexecute.Parameters.AddWithValue("@userratefromengine", mileagecat.UserRatesFromEngineSize);
            data.sqlexecute.Parameters.AddWithValue("@userratetoengine", mileagecat.UserRatesToEngineSize);
            if (mileagecat.mileageid == 0)
            {
                data.sqlexecute.Parameters.AddWithValue("@date", mileagecat.createdon);
                data.sqlexecute.Parameters.AddWithValue("@userid", mileagecat.createdby);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@date", mileagecat.modifiedon);
                data.sqlexecute.Parameters.AddWithValue("@userid", mileagecat.modifiedby);
            }
            if (currentUser != null)
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    data.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@employeeID", (mileagecat.mileageid == 0 ? mileagecat.createdby : mileagecat.modifiedby));
                data.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            if (mileagecat.FinancialYearID == null)
            {
                data.sqlexecute.Parameters.AddWithValue("@FinancialYearID", DBNull.Value);
            }
            else
            {
                data.sqlexecute.Parameters.AddWithValue("@FinancialYearID", mileagecat.FinancialYearID);
            }

            data.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            data.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            data.ExecuteProc("saveVehicleJourneyRate");
            mileageid = (int)data.sqlexecute.Parameters["@identity"].Value;
            data.sqlexecute.Parameters.Clear();
            Cache.Delete(this.accountid, CacheArea, mileageid.ToString());
            return mileageid;
        }

        public const string CacheArea = "MileageCat";

        /// <summary>
        /// Gets a list item for a mileagecat to use in a dropdown.
        /// </summary>
        /// <param name="mileagecat">The mileagecat object.</param>
        /// <returns>A ListItem object.</returns>
        public static ListItem GetMileageCatListItem(cMileageCat mileagecat)
        {
            var listItem = new ListItem(mileagecat.carsize, mileagecat.mileageid.ToString());
            listItem.Attributes["data-uom"] = mileagecat.mileUom.ToString().ToLower();
            return listItem;
        }
        
        /// <summary>
        /// Gets a list of <see cref="cMileageCat"/> for the supplied vehicleId
        /// </summary>
        /// <param name="vehicleId">The vehicleId</param>
        /// <param name="user">The <see cref="ICurrentUser"/></param>
        /// <param name="employeeCars">An instance of <see cref="cEmployeeCars"/></param>
        /// <returns></returns>
        public List<cMileageCat> GetByVehicleId(int vehicleId, ICurrentUser user, cEmployeeCars employeeCars)
        {                    
            cCar car = employeeCars.GetCarByID(vehicleId) ??
                employeeCars.GetCarFromDB(vehicleId);

            var vehicleMileageCategories = new List<cMileageCat>();

            if (car == null)
            {
                return vehicleMileageCategories;
            }

            foreach (int catId in car.mileagecats)
            {
                vehicleMileageCategories.Add(this.GetMileageCatById(catId));
            }

            return vehicleMileageCategories;
        }

        /// <summary>
        /// Gets the mileage comment
        /// </summary>
        /// <param name="employee">The employee</param>
        /// <param name="mileageCategory">The mileage category</param>
        /// <param name="date"></param>
        /// <param name="car">The expense date</param>
        /// <param name="esrAssignmentId">The ESR assignmentId</param>
        /// <param name="user">THe current user</param>
        /// <returns></returns>
        public string GetMileageCommentOnly(Employee employee, cMileageCat mileageCategory, DateTime date, cCar car,
                                       ref long esrAssignmentId, ICurrentUser user)
        {
            string sUom = mileageCategory.mileUom.ToString();

            string comment = "";
            cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
            int subAccountID = employee.DefaultSubAccount;
            if (esrAssignmentId == 0)
            {
                var assignments = new cESRAssignments(accountid, user.EmployeeID).GetAvailableAssignmentListItems(false, date);
                if (assignments.Length > 0)
                {
                    esrAssignmentId = int.Parse(assignments[0].Value);
                }
            }

            cCurrencies clscurrencies = new cCurrencies(accountid, subAccountID);
            cEmployees clsemployees = new cEmployees(accountid);
            cCurrency reqcurrency = clscurrencies.getCurrencyById(mileageCategory.currencyid);

            string noRateComment = String.Format("Current vehicle journey rate is undefined using vehicle journey rate category {0}.<br />" +
                                   "If you continue your claim the fuel expense will have a value of {1}0.00.<br />" +
                                   "Please contact your organisation's administrator.",
                                   mileageCategory.carsize, clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol);

            if (car.defaultuom != mileageCategory.mileUom)
            {
                switch (car.defaultuom)
                {
                    case MileageUOM.KM:
                        {
                            comment =
                                "You cannot claim for this vehicle as its unit of measure is 'Kilometres' and your Vehicle Journey Rate Category to claim against is 'Miles'. Please contact your administrator.";
                            break;
                        }
                    case MileageUOM.Mile:
                        {
                            comment =
                                "You cannot claim for this vehicle as its unit of measure is 'Miles' and your Vehicle Journey Rate Category to claim against is 'Kilometres'. Please contact your administrator.";
                            break;
                        }
                }
                return comment;
            }

            cMileageDaterange clsdaterange = this.getMileageDateRange(mileageCategory, date);

            decimal totalmiles = 0;


            if (mileageCategory.thresholdType == ThresholdType.Journey) //always display the same comment
            {
                var noRate = true;
                comment = "<b>" + mileageCategory.carsize + "</b><br />Rates:<br /><br />";

                foreach (cMileageThreshold threshold in clsdaterange.thresholds)
                {
                    var rate = VehicleJourneyRateThresholdRate.Get(user, threshold.MileageThresholdId, car.VehicleEngineTypeId);
                    if (rate == null || rate.RatePerUnit == null)
                    {
                        continue;
                    }

                    noRate = false;

                    var mileagerate = (decimal)rate.RatePerUnit;
                    switch (threshold.RangeType)
                    {
                        case RangeType.Any:
                            comment += " - " +
                                       mileagerate.ToString(
                                           clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol +
                                           "###,###,##0.0000") + " rate per " + sUom + ".<br />";
                            break;
                        case RangeType.GreaterThanOrEqualTo:
                            comment += " - " +
                                       mileagerate.ToString(
                                           clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol +
                                           "###,###,##0.0000") + " rate per " + sUom + " for mileage greater than " +
                                       threshold.RangeValue1 + " " + sUom + "s .<br />";
                            break;

                        case RangeType.LessThan:
                            comment += " - " +
                                       mileagerate.ToString(
                                           clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol +
                                           "###,###,##0.0000") + " rate per " + sUom + " for mileage less than " +
                                       threshold.RangeValue1 + " " + sUom + "s .<br />";
                            break;

                        case RangeType.Between:
                            comment += " - " +
                                       mileagerate.ToString(
                                           clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol +
                                           "###,###,##0.0000") + " rate per " + sUom + " for mileage between " +
                                       threshold.RangeValue1 + " and " + threshold.RangeValue2 + " " +
                                       sUom + "s .<br />";
                            break;
                    }
                }

                if (noRate)
                {
                    return comment + noRateComment;
                }
            }
            else
            {
                totalmiles = clsemployees.getMileageTotal(employee.EmployeeID, date);
                cMileageThreshold threshold = this.getMileageThreshold(clsdaterange, totalmiles);

                if (threshold != null)
                {
                    var rate = VehicleJourneyRateThresholdRate.Get(user, threshold.MileageThresholdId, car.VehicleEngineTypeId);
                    if (rate == null || rate.RatePerUnit == null)
                    {
                        return noRateComment;
                    }

                    var mileagerate = (decimal)rate.RatePerUnit;
                    comment = "Current vehicle journey rate is " +
                              mileagerate.ToString(
                                  clsglobalcurrencies.getGlobalCurrencyById(reqcurrency.globalcurrencyid).symbol +
                                  "###,###,##0.0000") + " per " + sUom + " using vehicle journey rate category " +
                              mileageCategory.carsize + ".";
                }
                else
                {
                    comment = "";
                }
            }
            return comment;
        }


        /// <summary>
        /// Return the description of the current home to office rule.
        /// </summary>
        /// <param name="employee">The employee that this applies to.</param>
        /// <param name="mileageCategory">The mileage category for this item.</param>
        /// <param name="date">The date of the claim</param>
        /// <param name="car">The vehicle for the mileage item.</param>
        /// <param name="subcat">The expense item of the claim item.</param>
        /// <param name="mileageCategories">The mileage categories used.</param>
        /// <param name="esrAssignmentId">The ESR assignment ID (if any)</param>
        /// <returns></returns>
        public string GetHomeToOfficeComment(Employee employee, cMileageCat mileageCategory, DateTime date, cCar car,
                                             cSubcat subcat, cMileagecats mileageCategories, ICurrentUser currentUser, cAccountSubAccount subAccount, long esrAssignmentId = 0)
        {
            string sUom = mileageCategory.mileUom.ToString();
            string comment = string.Empty;
            if (subcat.EnableHomeToLocationMileage)
            {
                decimal hometooffice = 0;
                decimal officetohome = 0;
                string pluralUomHtO = "";
                string pluralUomOtH = "";

                if (car.ExemptFromHomeToOffice)
                {
                    comment += "<br /><br />This vehicle is exempt from any Home to Office Mileage Deductions";
                    return comment;
                }

                #region Get home to office distance if a home to office deduction is required

                if (subcat.HomeToLocationType != HomeToLocationType.None && subcat.HomeToLocationType != HomeToLocationType.CalculateHomeAndOfficeToLocationDiff && subcat.HomeToLocationType != HomeToLocationType.FlagHomeAndOfficeToLocationDiff)
                {
                    var workAddress = employee.GetWorkAddresses().GetBy(currentUser, date, (int)esrAssignmentId);
                    var homeAddress = employee.GetHomeAddresses().GetBy(date);
                    var workAddressId = workAddress != null ? workAddress.LocationID : 0;
                    var homeAddressId = homeAddress != null ? homeAddress.LocationID : 0;
                    var homeAddressRef = homeAddressId <= 0 ? null : this._addresses.GetAddressById(homeAddressId);
                    var workAddressRef = workAddressId <= 0 ? null :  this._addresses.GetAddressById(workAddressId);
                    officetohome = AddressDistance.GetRecommendedOrCustomDistance(workAddressRef, homeAddressRef, this.accountid, subAccount, currentUser) ?? 0;
                    hometooffice = AddressDistance.GetRecommendedOrCustomDistance(homeAddressRef, workAddressRef, this.accountid, subAccount, currentUser) ?? 0;

                    if (mileageCategory.mileUom == MileageUOM.KM)
                    {
                        officetohome = mileageCategories.convertMilesToKM(officetohome);
                        hometooffice = mileageCategories.convertMilesToKM(hometooffice);
                    }

                    if (hometooffice > 1)
                    {
                        pluralUomHtO = "s";
                    }
                    if (officetohome > 1)
                    {
                        pluralUomOtH = "s";
                    }
                }

                #endregion

                comment += cSubcat.GetMileageText(subcat, hometooffice, sUom, pluralUomHtO, officetohome, pluralUomOtH);

            }
            return comment;

        }

        /// <summary>
        /// Builds up the comment for a vehicle journey rate
        /// </summary>
        /// <param name="id">The mileageId</param>
        /// <param name="accountid">The accountId</param>
        /// <param name="employeeid">employeeId</param>
        /// <param name="carid">The carId</param>
        /// <param name="mileageid">The mileage Id</param>
        /// <param name="date">The expense date</param>
        /// <param name="subcatid">The subcatId</param>
        /// <param name="esrAssignmentId">The esr assignment Id</param>
        /// <returns></returns>
        public string[] GetComment(string id, int accountId, int employeeId, int carId, int mileageId, DateTime date, int subcatId, long esrAssignmentId = 0)
        {

            string[] data = new string[3];
            data[0] = id;
            data[1] = "";

            CurrentUser user = cMisc.GetCurrentUser();
            cSubcats subCats = new cSubcats(accountId);
            cMileagecats mileage = new cMileagecats(accountId);
            cEmployees employees = new cEmployees(accountId);
            Employee employee = employees.GetEmployeeById(employeeId);
            cEmployeeCars employeeCars = new cEmployeeCars(accountId, employeeId);
            cSubcat subcat = subCats.GetSubcatById(subcatId);
            cCar car = null;

            cMisc misc = new cMisc(accountId);
            cGlobalProperties properties = misc.GetGlobalProperties(accountId);
            cAccountSubAccounts subAccounts = new cAccountSubAccounts(accountId);
            cAccountProperties accountProperties = subAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties.Clone(); // clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();

            var subAccount = subAccounts.getSubAccountById(user.CurrentSubAccountId);

            if (carId == 0)
            {
                car = employeeCars.GetCarByID(employeeCars.GetDefaultCarID(properties.blocktaxexpiry, properties.blockmotexpiry, properties.blockinsuranceexpiry, properties.BlockBreakdownCoverExpiry, accountProperties.DisableCarOutsideOfStartEndDate, date));
            }
            else
            {
                car = employeeCars.GetCarByID(carId);
            }

            if (car != null)
            {
                if (mileageId == 0)
                {
                    if (subcat.MileageCategory != null)
                    {
                        mileageId = (int)subcat.MileageCategory;
                    }
                    else
                    {
                        if (car.mileagecats.Count > 0)
                        {
                            mileageId = car.mileagecats[0];
                        }
                    }
                }

                cMileageCat mileagecat = mileage.GetMileageCatById(mileageId);

                if (mileagecat.catvalid)
                {

                    string mileageComment = this.GetMileageCommentOnly(employee, mileagecat, date, car, ref esrAssignmentId, user);
              
                    data[1] = "<div style=\"width:400px;\">" + mileageComment + "</div>";
                
                    string homeToOfficeComment = this.GetHomeToOfficeComment(employee, mileagecat, date, car, subcat, mileage, user, subAccount, esrAssignmentId);

                    homeToOfficeComment = Regex.Replace(homeToOfficeComment, "^(?:" + Regex.Escape("<br />") + ")*", "");//remove leading line breaks
                    data[2] = "<div style=\"width:400px;\">" + homeToOfficeComment + "</div>";
                   
              
                }
            }

            return data;
        }
    }
}
