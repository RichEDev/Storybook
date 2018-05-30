namespace Spend_Management
{
    using System.Data.SqlClient;
    using Microsoft.SqlServer.Server;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;

    using Infragistics.WebUI.UltraWebGrid;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using Utilities.DistributedCaching;

    /// <summary>
    ///     Summary description for subcats.
    /// </summary>
    public class cSubcats
    {
        private readonly int _accountid;

        Dictionary<int, cSubcat> _list;

        /// <summary>
        /// The cache area.
        /// </summary>
        public const string CacheArea = "itemrolessubcats";

        /// <summary>
        /// The caching object .
        /// </summary>
        private readonly Cache caching = new Cache();

        public cSubcats(int accountId)
        {
            this._accountid = accountId;
            InitialiseData();
        }

        private void InitialiseData()
        {
            this._list = this.caching.Get(this._accountid, CacheArea, "0") as Dictionary<int, cSubcat>
                            ?? this.CacheList();
        }

        /// <summary>
        /// The invalidate cache.
        /// </summary>
        private void InvalidateCache()
        {
            this.caching.Delete(this._accountid, CacheArea, "0");
            this.InitialiseData();
        }

        private Dictionary<int, cSubcat> CacheList()
        {
            Dictionary<int, cSubcat> list = new Dictionary<int, cSubcat>();

            var tables = new cTables(this._accountid);
            var fields = new cFields(this._accountid);
            var userdefinedFields = new cUserdefinedFields(this._accountid);
            cTable table = tables.GetTableByID(new Guid("401b44d7-d6d8-497b-8720-7ffcc07d635d"));
            cTable udfTable = tables.GetTableByID(table.UserDefinedTableID);
            SortedList<int, SortedList<int, object>> lstUserdefined = userdefinedFields.GetAllRecords(udfTable, tables, fields);
            var subcats = new List<cSubcat>();

            Dictionary<int, List<cCountrySubcat>> countrySubcatList = this.GetCountries();
            List<cCountrySubcat> countries;

            Dictionary<int, List<int>> allowancesList = this.GetAllowances();
            List<int> allowances;

            Dictionary<int, List<int>> udfList = this.GetAssociatedUdFs();
            List<int> udfs;

            Dictionary<int, List<int>> subcatSplitList = this.GetSubcatSplit();
            List<int> splitSubcats;

            Dictionary<int, List<cSubcatVatRate>> subcatVatRateList = this.GetVatRates();
            List<cSubcatVatRate> rates;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                connection.ClearParameters();

                string sql = "select subcatid, categoryid, description, subcat, vatapp, vatamount, mileageapp, staffapp, othersapp, vatreceipt, tipapp, pmilesapp, bmilesapp, speedoapp, blitresapp, plitresapp, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, vatpercent, eventinhomeapp, receiptapp, calculation, passengersapp, nopassengersapp, passengernamesapp, comment, splitentertainment, entertainmentid, reimbursable, nonightsapp, attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumberapp, vatnumbermand, nopersonalguestsapp, noremoteworkersapp, alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, vatlimitwithout, vatlimitwith, reasonapp, otherdetailsapp, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, shortsubcat, fromapp, toapp, companyapp, enableHomeToLocationMileage, homeToLocationType, mileageCategory, isRelocationMileage, reimbursableSubcatID, allowHeavyBulkyMileage, homeToOfficeAlwaysZero, StartDate, EndDate, Validate, HomeToOfficeFixedMiles, PublicTransportRate, EnableDutyOfCare, RequireClassOneBusinessInsurance,EnforceHomeToOfficeMileageCap,HomeToOfficeMileageCap from dbo.subcats";

                using (IDataReader reader = connection.GetReader(sql))
                {
                    if (reader == null)
                    {
                        return null;
                    }

                    int startDateOrdinal = reader.GetOrdinal("StartDate");
                    int endDateOrdinal = reader.GetOrdinal("EndDate");
                    int subcatIdOrdinal = reader.GetOrdinal("subcatid");
                    int categoryIdOrdinal = reader.GetOrdinal("categoryid");
                    int subcatStringIdOrdinal = reader.GetOrdinal("subcat");
                    int descriptionOrdinal = reader.GetOrdinal("description");
                    int mileageAppOrdinal = reader.GetOrdinal("mileageapp");
                    int staffAppOrdinal = reader.GetOrdinal("staffapp");
                    int othersAppOrdinal = reader.GetOrdinal("othersapp");
                    int tipAppOrdinal = reader.GetOrdinal("tipapp");
                    int pmilesAppOrdinal = reader.GetOrdinal("pmilesapp");
                    int bmilesAppOrdinal = reader.GetOrdinal("bmilesapp");
                    int allowanceAmountOrdinal = reader.GetOrdinal("allowanceamount");
                    int accountCodeOrdinal = reader.GetOrdinal("accountcode");
                    int attendeesAppOrdinal = reader.GetOrdinal("attendeesapp");
                    int addAsNetOrdinal = reader.GetOrdinal("addasnet");
                    int pdCatIdOrdinal = reader.GetOrdinal("pdcatid");
                    int eventInHomeAppOrdinal = reader.GetOrdinal("eventinhomeapp");
                    int receiptAppOrdinal = reader.GetOrdinal("receiptapp");
                    int calculationOrdinal = reader.GetOrdinal("calculation");
                    int passengersAppOrdinal = reader.GetOrdinal("passengersapp");
                    int noPassengersAppOrdinal = reader.GetOrdinal("nopassengersapp");
                    int passengerNamesAppOrdinal = reader.GetOrdinal("passengernamesapp");
                    int commentOrdinal = reader.GetOrdinal("comment");
                    int splitEntertainmentOrdinal = reader.GetOrdinal("splitentertainment");
                    int entertainmentIdOrdinal = reader.GetOrdinal("entertainmentid");
                    int reimbursableOrdinal = reader.GetOrdinal("reimbursable");
                    int noNightsAppOrdinal = reader.GetOrdinal("nonightsapp");
                    int attendeesMandatoryOrdinal = reader.GetOrdinal("attendeesmand");
                    int noDirectorsAppOrdinal = reader.GetOrdinal("nodirectorsapp");
                    int hotelAppOrdinal = reader.GetOrdinal("hotelapp");
                    int noRoomsAppOrdinal = reader.GetOrdinal("noroomsapp");
                    int hotelMandatoryOrdinal = reader.GetOrdinal("hotelmand");
                    int vatNumberAppOrdinal = reader.GetOrdinal("vatnumberapp");
                    int vatNumberMandatoryOrdinal = reader.GetOrdinal("vatnumbermand");
                    int noPersonalGuestsAppOrdinal = reader.GetOrdinal("nopersonalguestsapp");
                    int noRemoteWorkersAppOrdinal = reader.GetOrdinal("noremoteworkersapp");
                    int alternateAccountCodeOrdinal = reader.GetOrdinal("alternateaccountcode");
                    int splitPersonalOrdinal = reader.GetOrdinal("splitpersonal");
                    int splitRemoteOrdinal = reader.GetOrdinal("splitremote");
                    int personalIdOrdinal = reader.GetOrdinal("personalid");
                    int remoteIdOrdinal = reader.GetOrdinal("remoteid");
                    int reasonAppOrdinal = reader.GetOrdinal("reasonapp");
                    int otherDetailsAppOrdinal = reader.GetOrdinal("otherdetailsapp");
                    int createdOnOrdinal = reader.GetOrdinal("createdon");
                    int createdByOrdinal = reader.GetOrdinal("createdby");
                    int modifiedOnOrdinal = reader.GetOrdinal("modifiedon");
                    int modifiedByOrdinal = reader.GetOrdinal("modifiedby");
                    int homeToLocationTypeOrdinal = reader.GetOrdinal("homeToLocationType");
                    int shortSubcatOrdinal = reader.GetOrdinal("shortsubcat");
                    int fromAppOrdinal = reader.GetOrdinal("fromapp");
                    int toAppOrdinal = reader.GetOrdinal("toapp");
                    int companyAppOrdinal = reader.GetOrdinal("companyapp");
                    int enableHomeToLocationMileageOrdinal = reader.GetOrdinal("enableHomeToLocationMileage");
                    int mileageCategoryOrdinal = reader.GetOrdinal("mileageCategory");
                    int isRelocationMileageOrdinal = reader.GetOrdinal("isRelocationMileage");
                    int reimbursableSubcatIdOrdinal = reader.GetOrdinal("reimbursableSubcatID");
                    int allowHeavyBulkyMileageOrdinal = reader.GetOrdinal("allowHeavyBulkyMileage");
                    int homeToOfficeAlwaysZeroOrdinal = reader.GetOrdinal("homeToOfficeAlwaysZero");
                    int homeToOfficeFixedMilesOrdinal = reader.GetOrdinal("HomeToOfficeFixedMiles");
                    int publicTransportRateOrdinal = reader.GetOrdinal("PublicTransportRate");
                    int enableDutyOfCareOrdinal = reader.GetOrdinal("EnableDutyOfCare");
                    int requireClass1BusinessInsuranceOrdinal = reader.GetOrdinal("RequireClassOneBusinessInsurance");
                    int enforceMileageCapOrdial = reader.GetOrdinal("EnforceHomeToOfficeMileageCap");
                    int homeToOfficeMileageCapOrdial = reader.GetOrdinal("HomeToOfficeMileageCap");
                    connection.sqlexecute.Parameters.Clear();

                    while (reader.Read())
                    {
                        int subcatId = reader.GetInt32(subcatIdOrdinal);
                        int categoryId = reader.GetInt32(categoryIdOrdinal);
                        string subcatStringId = reader.GetString(subcatStringIdOrdinal);
                        string description = reader.IsDBNull(descriptionOrdinal) == false
                            ? reader.GetString(descriptionOrdinal)
                            : string.Empty;
                        bool mileageApp = reader.GetBoolean(mileageAppOrdinal);
                        bool staffApp = reader.GetBoolean(staffAppOrdinal);
                        bool othersApp = reader.GetBoolean(othersAppOrdinal);
                        bool tipApp = reader.GetBoolean(tipAppOrdinal);
                        bool pmilesApp = reader.GetBoolean(pmilesAppOrdinal);
                        bool bmilesApp = reader.GetBoolean(bmilesAppOrdinal);
                        decimal allowanceAmount = reader.IsDBNull(allowanceAmountOrdinal) == false
                            ? reader.GetDecimal(allowanceAmountOrdinal)
                            : 0;
                        string accountCode = reader.IsDBNull(accountCodeOrdinal) == false
                            ? reader.GetString(accountCodeOrdinal)
                            : string.Empty;
                        bool attendeesApp = reader.GetBoolean(attendeesAppOrdinal);
                        bool addAsNet = reader.GetBoolean(addAsNetOrdinal);
                        int pdCatId = reader.IsDBNull(pdCatIdOrdinal) == false
                            ? reader.GetInt32(pdCatIdOrdinal)
                            : 0;
                        bool eventInHomeApp = reader.GetBoolean(eventInHomeAppOrdinal);
                        bool receiptApp = reader.GetBoolean(receiptAppOrdinal);
                        var calculation = (CalculationType)reader.GetByte(calculationOrdinal);
                        bool passengersApp = reader.GetBoolean(passengersAppOrdinal);
                        bool noPassengersApp = reader.GetBoolean(noPassengersAppOrdinal);
                        bool passengerNamesApp = reader.GetBoolean(passengerNamesAppOrdinal);
                        string comment = reader.IsDBNull(commentOrdinal) == false
                            ? reader.GetString(commentOrdinal)
                            : string.Empty;
                        bool splitEntertainment = reader.GetBoolean(splitEntertainmentOrdinal);
                        int entertainmentId = reader.IsDBNull(entertainmentIdOrdinal) == false
                            ? reader.GetInt32(entertainmentIdOrdinal)
                            : 0;
                        bool reimbursable = reader.GetBoolean(reimbursableOrdinal);
                        bool noNightsApp = reader.GetBoolean(noNightsAppOrdinal);
                        bool attendeesMandatory = reader.GetBoolean(attendeesMandatoryOrdinal);
                        bool noDirectorsApp = reader.GetBoolean(noDirectorsAppOrdinal);
                        bool hotelApp = reader.GetBoolean(hotelAppOrdinal);
                        bool noRoomsApp = reader.GetBoolean(noRoomsAppOrdinal);
                        bool hotelMandatory = reader.GetBoolean(hotelMandatoryOrdinal);
                        bool vatNumberApp = reader.GetBoolean(vatNumberAppOrdinal);
                        bool vatNumberMandatory = reader.GetBoolean(vatNumberMandatoryOrdinal);
                        bool noPersonalGuestsApp = reader.GetBoolean(noPersonalGuestsAppOrdinal);
                        bool noRemoteWorkersApp = reader.GetBoolean(noRemoteWorkersAppOrdinal);
                        string alternateAccountCode = reader.IsDBNull(alternateAccountCodeOrdinal)
                                                      == false
                            ? reader.GetString(alternateAccountCodeOrdinal)
                            : string.Empty;
                        bool splitPersonal = reader.GetBoolean(splitPersonalOrdinal);
                        bool splitRemote = reader.GetBoolean(splitRemoteOrdinal);
                        int personalId = reader.IsDBNull(personalIdOrdinal)
                            ? 0
                            : reader.GetInt32(personalIdOrdinal);
                        int remoteId = reader.IsDBNull(remoteIdOrdinal)
                            ? 0
                            : reader.GetInt32(remoteIdOrdinal);
                        bool reasonApp = reader.GetBoolean(reasonAppOrdinal);
                        bool otherDetailsApp = reader.GetBoolean(otherDetailsAppOrdinal);
                        DateTime createdOn = reader.IsDBNull(createdOnOrdinal)
                            ? new DateTime(1900, 01, 01)
                            : reader.GetDateTime(createdOnOrdinal);
                        int createdBy = reader.IsDBNull(createdByOrdinal)
                            ? 0
                            : reader.GetInt32(createdByOrdinal);
                        DateTime modifiedOn = reader.IsDBNull(modifiedOnOrdinal)
                            ? new DateTime(1900, 01, 01)
                            : reader.GetDateTime(modifiedOnOrdinal);
                        int modifiedBy = reader.IsDBNull(modifiedByOrdinal)
                            ? 0
                            : reader.GetInt32(modifiedByOrdinal);
                        string shortSubcat = reader.IsDBNull(shortSubcatOrdinal)
                            ? string.Empty
                            : reader.GetString(shortSubcatOrdinal);
                        bool fromApp = reader.GetBoolean(fromAppOrdinal);
                        bool toApp = reader.GetBoolean(toAppOrdinal);
                        bool companyApp = reader.GetBoolean(companyAppOrdinal);
                        bool enableHomeToLocationMileage =
                            reader.GetBoolean(enableHomeToLocationMileageOrdinal);
                        HomeToLocationType homeToLocationType;
                        bool validate = false;

                        countrySubcatList.TryGetValue(subcatId, out countries);
                        if (countries == null)
                        {
                            countries = new List<cCountrySubcat>();
                        }

                        allowancesList.TryGetValue(subcatId, out allowances);
                        if (allowances == null)
                        {
                            allowances = new List<int>();
                        }

                        udfList.TryGetValue(subcatId, out udfs);
                        if (udfs == null)
                        {
                            udfs = new List<int>();
                        }

                        subcatSplitList.TryGetValue(subcatId, out splitSubcats);
                        if (splitSubcats == null)
                        {
                            splitSubcats = new List<int>();
                        }

                        subcatVatRateList.TryGetValue(subcatId, out rates);
                        if (rates == null)
                        {
                            rates = new List<cSubcatVatRate>();
                        }

                        if (!reader.IsDBNull(homeToLocationTypeOrdinal))
                        {
                            homeToLocationType =
                                (HomeToLocationType)reader.GetByte(homeToLocationTypeOrdinal);
                        }
                        else
                        {
                            homeToLocationType = HomeToLocationType.None;
                        }

                        int? mileagecategory;

                        if (reader.IsDBNull(mileageCategoryOrdinal))
                        {
                            mileagecategory = null;
                        }
                        else
                        {
                            mileagecategory = reader.GetInt32(mileageCategoryOrdinal);
                        }

                        bool isRelocationMileage = reader.GetBoolean(isRelocationMileageOrdinal);
                        int? reimbursableSubcatId;

                        if (!reader.IsDBNull(reimbursableSubcatIdOrdinal))
                        {
                            reimbursableSubcatId = reader.GetInt32(reimbursableSubcatIdOrdinal);
                        }
                        else
                        {
                            reimbursableSubcatId = null;
                        }

                        bool allowHeavyBulkyMileage = reader.GetBoolean(allowHeavyBulkyMileageOrdinal);

                        SortedList<int, object> userdefined;
                        lstUserdefined.TryGetValue(subcatId, out userdefined);

                        bool homeToOfficeAsZero = reader.GetBoolean(homeToOfficeAlwaysZeroOrdinal);
                        DateTime? startDate = null;
                        DateTime? endDate = null;

                        if (reader.IsDBNull(startDateOrdinal) == false)
                        {
                            startDate = reader.GetDateTime(startDateOrdinal);
                        }

                        if (reader.IsDBNull(endDateOrdinal) == false)
                        {
                            endDate = reader.GetDateTime(endDateOrdinal);
                        }

                        // If the validation service is enabled for this account
                        var criteria = new List<ExpenseValidationCriterion>();
                        
                            // set whether the item should be validated
                            validate = reader.GetRequiredValue<bool>("Validate");

                            // grab the crtierion and conver the text
                            criteria = new ExpenseValidationManager(this._accountid).GetCriteriaForSubcat(subcatId);
                        

                        float? homeToOfficeFixedMiles = null;

                        if (!reader.IsDBNull(homeToOfficeFixedMilesOrdinal))
                        {
                            homeToOfficeFixedMiles =
                                (float?)reader.GetDouble(homeToOfficeFixedMilesOrdinal);
                        }

                        int? publicTransportRate = null;

                        if (!reader.IsDBNull(publicTransportRateOrdinal))
                        {
                            publicTransportRate = reader.GetInt32(publicTransportRateOrdinal);
                        }
                        
                        bool enforceToOfficeMileageCap = reader.IsDBNull(enforceMileageCapOrdial) ? false : reader.GetBoolean(enforceMileageCapOrdial);
                        float? homeToOfficeMileageCap = null;

                        if (!reader.IsDBNull(homeToOfficeMileageCapOrdial))
                        {
                            homeToOfficeMileageCap = (float?)reader.GetDouble(homeToOfficeMileageCapOrdial);
                        }

                        bool enableDoc = reader.IsDBNull(enableDutyOfCareOrdinal) ? false : reader.GetBoolean(enableDutyOfCareOrdinal);
                        bool requireClass1BusinessInsurance = reader.IsDBNull(enableDutyOfCareOrdinal) ? false : reader.GetBoolean(requireClass1BusinessInsuranceOrdinal);
                        var subcat = new cSubcat(
                            subcatId,
                            categoryId,
                            subcatStringId,
                            description,
                            mileageApp,
                            staffApp,
                            othersApp,
                            tipApp,
                            pmilesApp,
                            bmilesApp,
                            allowanceAmount,
                            accountCode,
                            attendeesApp,
                            addAsNet,
                            pdCatId,
                            eventInHomeApp,
                            receiptApp,
                            calculation,
                            passengersApp,
                            noPassengersApp,
                            passengerNamesApp,
                            comment,
                            splitEntertainment,
                            entertainmentId,
                            reimbursable,
                            noNightsApp,
                            attendeesMandatory,
                            noDirectorsApp,
                            hotelApp,
                            noRoomsApp,
                            hotelMandatory,
                            vatNumberApp,
                            vatNumberMandatory,
                            noPersonalGuestsApp,
                            noRemoteWorkersApp,
                            alternateAccountCode,
                            splitPersonal,
                            splitRemote,
                            personalId,
                            remoteId,
                            reasonApp,
                            otherDetailsApp,
                            userdefined,
                            createdOn,
                            createdBy,
                            modifiedOn,
                            modifiedBy,
                            shortSubcat,
                            fromApp,
                            toApp,
                            countries,
                            allowances,
                            udfs,
                            splitSubcats,
                            companyApp,
                            rates,
                            enableHomeToLocationMileage,
                            homeToLocationType,
                            mileagecategory,
                            isRelocationMileage,
                            reimbursableSubcatId,
                            allowHeavyBulkyMileage,
                            homeToOfficeAsZero,
                            homeToOfficeFixedMiles,
                            publicTransportRate,
                            startDate,
                            endDate,
                            validate,
                            criteria,
                            enableDoc,
                            requireClass1BusinessInsurance,
                            enforceToOfficeMileageCap,
                            homeToOfficeMileageCap);

                        list.Add(subcatId, subcat);
                        
                    }
                    reader.Close();
                }
            }

            this.caching.Add(this._accountid, CacheArea, "0", list);
            return list;
        }

        #region Public Methods and Operators

        /// <summary>
        /// Creates a dropdown for the subcats
        /// </summary>
        /// <returns></returns>
        public List<ListItem> CreateDropDown()
        {
            return (from subcat in _list.Values orderby subcat.subcat
                    select new ListItem(subcat.subcat, subcat.subcatid.ToString(CultureInfo.InvariantCulture))).ToList();
        }

        /// <summary>
        ///     Create Drop Down List Items for Card Transactions
        /// </summary>
        /// <returns>list of ListItems</returns>
        public ListItem[] CreateDropDownForCardTransactions()
        {
            var cardSubcatsList = new List<ListItem>();
            cardSubcatsList.Add(new ListItem(string.Empty, "0"));

            cardSubcatsList.AddRange((from subcat in _list.Values
                                      orderby subcat.subcat
                                      where subcat.calculation != CalculationType.FixedAllowance
                    && (subcat.calculation == CalculationType.NormalItem || subcat.calculation == CalculationType.Meal
                        || subcat.calculation == CalculationType.FuelReceipt
                        || subcat.calculation == CalculationType.FuelCardMileage)
                                      
                                      select new ListItem(subcat.subcat, subcat.subcatid.ToString(CultureInfo.InvariantCulture))).ToList());
           return cardSubcatsList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ListItem> CreateEntertainmentDropDown(List<SubcatBasic> subcats, int subcatId = 0)
        {
            var list = new List<ListItem>();

            foreach (var reqsubcat in subcats)
            {
                if (reqsubcat.SubcatId != subcatId && reqsubcat.CalculationType != CalculationType.PencePerMile && !reqsubcat.VatApp) list.Add(new ListItem(reqsubcat.Subcat, reqsubcat.SubcatId.ToString(CultureInfo.InvariantCulture)));
        }
            return list;
        }

        /// <summary>
        /// Creates a dropdown for reimbursable items
        /// </summary>
        /// <returns></returns>
        public List<ListItem> CreateReimbursableItemsDropDown()
        {
            return (from subcat in _list.Values
                    orderby subcat.subcat
                    where subcat.calculation == CalculationType.ItemReimburse
                    select new ListItem(subcat.subcat, subcat.subcatid.ToString(CultureInfo.InvariantCulture))).ToList();
        }

        /// <summary>
        /// Creates a dropdown for subsistence items
        /// </summary>
        /// <returns></returns>
        public List<ListItem> CreateSubsistenceDropDown(List<SubcatBasic> subcats)
        {
            return (from subcat in subcats
                    where subcat.CalculationType != CalculationType.PencePerMile && subcat.VatApp
                    select new ListItem(subcat.Subcat, subcat.SubcatId.ToString(CultureInfo.InvariantCulture))).ToList();
        }

        /// <summary>
        /// Creates a subcat value list
        /// </summary>
        /// <returns></returns>
        public ValueList CreateValueList()
        {
            var valueList = new ValueList();
            
            foreach (cSubcat subcat in _list.Values)
            {
                valueList.ValueListItems.Add(subcat.subcatid, subcat.subcat);
            }
            
            return valueList;
        }

        /// <summary>
        /// Adds splits
        /// </summary>
        /// <param name="subcatid">The main sub category ID</param>
        /// <param name="splitIds">The split item ID that is to be added.</param>
        /// <returns></returns>
        public object[] AddSplit(int subcatid, List<int> splitIds)
        {
            int returnvalue = -1;
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                ICurrentUser currentUser = cMisc.GetCurrentUser();

                foreach (int splitId in splitIds)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);

                    if (currentUser.isDelegate)
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }

                    connection.sqlexecute.Parameters.AddWithValue("@primarysubcatid", subcatid);
                    connection.sqlexecute.Parameters.AddWithValue("@splitsubcatid", splitId);
                    connection.AddReturn("@returnvalue", SqlDbType.Int);
                    connection.ExecuteProc("saveSubcatSplitItem");
                    returnvalue = connection.GetReturnValue<int>("@returnvalue");
                    if (returnvalue == -2)
                    {
                        var subcats = new cSubcats(currentUser.AccountID);
                        var subcat = subcats.GetSubcatById(subcatid);
                        var splitSubCat = subcats.GetSubcatById(splitId);
                        return new object[] { -2, splitSubCat.subcat, subcat.subcat };
                    }
                    else if (returnvalue != 0)
                    {
                        var subcats = new cSubcats(currentUser.AccountID);
                        var subcat = subcats.GetSubcatById(returnvalue);
                        var splitSubCat = subcats.GetSubcatById(splitId);
                        return new object[] { -1, splitSubCat.subcat, subcat.subcat };    
                    }

                    connection.sqlexecute.Parameters.Clear();
                }
            }

            this.InvalidateCache();
            return new object[] { returnvalue };
        }

        /// <summary>
        /// Calculates Fuel Card Reimbursable Pence Per Mile
        /// </summary>
        /// <param name="oldOdometerReading"></param>
        /// <param name="newOdometerReading"></param>
        /// <param name="numberOfBusinessMiles"></param>
        /// <param name="employeeId"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public decimal CalculateFuelCardReimbursablePencePerMiles(
            int oldOdometerReading,
            int newOdometerReading,
            decimal numberOfBusinessMiles,
            int employeeId,
            decimal total)
        {
            decimal pencePerMile = 0;
            decimal odometerDifference = newOdometerReading - oldOdometerReading;

            if (odometerDifference != 0)
            {
                pencePerMile = (total / odometerDifference);
            }

            return pencePerMile;
        }

        /// <summary>
        /// Deletes a split item
        /// </summary>
        /// <param name="subcatId"></param>
        /// <param name="splitSubCatId"></param>
        public void DeleteSplit(int subcatId, int splitSubCatId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                connection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);

                if (currentUser.isDelegate)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                connection.sqlexecute.Parameters.AddWithValue("@primarysubcatid", subcatId);
                connection.sqlexecute.Parameters.AddWithValue("@splitsubcatid", splitSubCatId);
                connection.ExecuteProc("deleteSubcatSplitItem");
                connection.sqlexecute.Parameters.Clear();
            }

            this.InvalidateCache();
        }

        /// <summary>
        ///     The delete subcat.
        /// </summary>
        /// <param name="subcatId">
        ///     The subcatid.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int DeleteSubcat(int subcatId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@subcatid", subcatId);
                SubcatBasic subcat = this.GetSubcatBasic(subcatId);
                string sql = "delete from dbo.additems where subcatid = @subcatid";
                connection.ExecuteSQL(sql);
                sql = "delete from dbo.subcat_split where primarysubcatid = @subcatid or splitsubcatid = @subcatid";
                connection.ExecuteSQL(sql);

                sql = "select count(subcatid) from flagAssociatedExpenseItems where subcatid = @subcatid";
                var count = connection.ExecuteScalar<int>(sql);
                if (count > 0)
                {
                    return 3;
                }

                sql = "select count(*) from dbo.savedexpenses where subcatid = @subcatid";
                count = connection.ExecuteScalar<int>(sql);

                if (count != 0)
                {
                    return 1; // cannot delete as it is still assigned to items 
                }

                // check to make sure subcat not used by n:1 custom entity attribute or udf
                var clsTables = new cTables(this._accountid);
                Guid subcatTableId = clsTables.GetTableByName("subcats").TableID;
                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@currentTableID", subcatTableId);
                connection.sqlexecute.Parameters.AddWithValue("@currentRecordID", subcatId);
                connection.sqlexecute.Parameters.Add("@retCode", SqlDbType.Int);
                connection.sqlexecute.Parameters["@retCode"].Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("checkReferencedBy");

                if (((int)connection.sqlexecute.Parameters["@retCode"].Value) != 0)
                {
                    return (int)connection.sqlexecute.Parameters["@retCode"].Value;
                }

                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@subcatid", subcatId);
                sql = "select count(*) from dbo.mobileExpenseItems where subcatid = @subcatid";
                count = connection.ExecuteScalar<int>(sql);
                if (count != 0)
                {
                    return 2; // cannot delete as it is still assigned to mobile items
                }

                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@subcatid", subcatId);
                sql = "select count(journeyId) from dbo.MobileJourneys where subcatid = @subcatid";
                count = connection.ExecuteScalar<int>(sql);
                if (count != 0)
                {
                    return 4; // cannot delete as it is still assigned to a mobile journey
                }

                sql = "delete from subcats where subcatid = @subcatid";
                connection.ExecuteSQL(sql);

                CurrentUser currentUser = cMisc.GetCurrentUser();
                var audit = new cAuditLog(this._accountid, currentUser.EmployeeID);

                if (subcat != null)
                {
                    audit.deleteRecord(SpendManagementElement.ExpenseItems, subcatId, subcat.Subcat);
                }

                connection.sqlexecute.Parameters.Clear();
            }

            var clsroles = new ItemRoles(this._accountid);
            clsroles.DeleteRolesubcatsBySubcatid(subcatId);
            this.InvalidateCache();
            return 0;
        }

        /// <summary>
        /// Delete VAT Rate
        /// </summary>
        /// <param name="vatRateId"></param>
        public void DeleteVatRate(int vatRateId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();

                connection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                connection.sqlexecute.Parameters.AddWithValue("@vatrateid", vatRateId);
                connection.ExecuteProc("deleteSubcatVatRate");
                connection.sqlexecute.Parameters.Clear();
            }
            this.InvalidateCache();
        }

        /// <summary>
         /// Gets the allowance ids for a subcat
        /// </summary>
         /// <param name="subcatId"></param>
         /// <returns>A list of allowance Ids</returns>
        private Dictionary<int, List<int>> GetAllowances()
        {
            Dictionary<int, List<int>> list = new Dictionary<int, List<int>>();
            List<int> allowanceSubcats;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                string sql = "select allowanceid, subcatid from dbo.[subcats_allowances]";
                connection.sqlexecute.Parameters.Clear();
                

                using (IDataReader reader = connection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        int allowanceId = reader.GetInt32(0);
                        int subcatId = reader.GetInt32(1);
                        list.TryGetValue(subcatId, out allowanceSubcats);
                        if (allowanceSubcats == null)
                        {
                            allowanceSubcats = new List<int>();
                            list.Add(subcatId, allowanceSubcats);
                        }

                        allowanceSubcats.Add(allowanceId);
                    }

                    reader.Close();
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a sorted list of subcats. 
        /// </summary>
        /// <param name="subcatIdList">List of subcat id's</param>
        /// <returns>A dictionary of subcat ids and their name </returns>
        public Dictionary<int, string> GetSubcatNamesByIdList(List<int> subcatIdList)
        {

            return (from subcat in _list.Values
                    orderby subcat.subcat
                    where subcatIdList.Contains(subcat.subcatid)
                    select subcat).ToDictionary(subcat => subcat.subcatid, subcat => subcat.subcat);
        }

        /// <summary>
        /// Get the countries for a subcatId
        /// </summary>
        /// <param name="subcatId"></param>
        /// <returns>A list of <see cref="cCountrySubcat"></see></returns>
        private Dictionary<int, List<cCountrySubcat>> GetCountries()
        {
            Dictionary<int, List<cCountrySubcat>> list = new Dictionary<int, List<cCountrySubcat>>();
            List<cCountrySubcat> countrySubcats;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                string sql = "select subcatid, countryid, accountcode from dbo.[subcats_countries]";
                connection.sqlexecute.Parameters.Clear();
                

                using (IDataReader reader = connection.GetReader(sql))
                {

                    while (reader.Read())
                    {
                        int subcatId = reader.GetInt32(0);
                        int countryId = reader.GetInt32(reader.GetOrdinal("countryid"));
                        string accountCode = reader.GetString(reader.GetOrdinal("accountcode"));

                        list.TryGetValue(subcatId, out countrySubcats);
                        if (countrySubcats == null)
                        {
                            countrySubcats = new List<cCountrySubcat>();
                            list.Add(subcatId, countrySubcats);
                        }
                        countrySubcats.Add(new cCountrySubcat(subcatId, countryId, accountCode));
                    }

                    reader.Close();
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a sorted list of subcats. 
        /// </summary>
        /// <param name="sortedBySubcatId">Defaults to sorting by subcat name, otherwise subcat id</param>
        /// <returns>A list of <see cref="SubcatBasic"></see></returns>
        public List<SubcatBasic> GetSortedList(bool sortedBySubcatId = false)
        {

            if (sortedBySubcatId)
            {
                return (from subcat in _list.Values
                        orderby subcat.subcatid
                        select new SubcatBasic { CalculationType = subcat.calculation, SubcatId = subcat.subcatid, Subcat = subcat.subcat, VatApp = subcat.vatapp, P11DCategoryId = subcat.pdcatid, CategoryId = subcat.categoryid, StartDate = subcat.StartDate, EndDate = subcat.EndDate }).ToList();
            }
            else
            {
                return (from subcat in _list.Values
                        orderby subcat.subcat
                        select new SubcatBasic { CalculationType = subcat.calculation, SubcatId = subcat.subcatid, Subcat = subcat.subcat, VatApp = subcat.vatapp, P11DCategoryId = subcat.pdcatid, CategoryId = subcat.categoryid, StartDate = subcat.StartDate, EndDate = subcat.EndDate }).ToList();
            }
            
        }

        /// <summary>
        /// Gets a basic version of the subcat with most commonly used fields
        /// </summary>
        /// <param name="subcatId">The subcat id</param>
        /// <returns><see cref="SubcatBasic"></see></returns>
        public SubcatBasic GetSubcatBasic(int subcatId)
        {
            return (from subcat in _list.Values
                    where subcat.subcatid == subcatId
                    select new SubcatBasic { CalculationType = subcat.calculation, SubcatId = subcat.subcatid, Subcat = subcat.subcat, VatApp = subcat.vatapp, P11DCategoryId = subcat.pdcatid, CategoryId = subcat.categoryid, StartDate = subcat.StartDate, EndDate = subcat.EndDate, Reimbursable = subcat.reimbursable, EnableHomeToLocationMileage = subcat.EnableHomeToLocationMileage, HomeToLocationType = subcat.HomeToLocationType, Mileageapp = subcat.mileageapp, EnabledDoc = subcat.EnableDoC, RequireClass1BusinessInsurance = subcat.RequireClass1BusinessInsurance }).FirstOrDefault();
        }

        /// <summary>
        /// Gets a sorted list of subcats. This is not particularly efficient returning the whole collection object. Only used presently in the API and for mobile.
        /// </summary>
        /// <returns>A list of <see cref="cSubcat"></see></returns>
        public List<cSubcat> GetBigSortedList()
        {

            return (from subcat in _list.Values
                    orderby subcat.subcat
                    select  subcat).ToList();
        }

        /// <summary>
        /// Get alls the subcats relating to mileage
        /// </summary>
        /// <returns>A list of <see cref="cSubcat"></see></returns>
        public List<cSubcat> GetMileageSubcats()
        {
            return (from subcat in _list.Values
                    orderby subcat.subcat
                    where subcat.toapp == true && subcat.fromapp == true && (subcat.calculation == CalculationType.PencePerMile || subcat.calculation == CalculationType.PencePerMileReceipt)
                    select subcat).ToList();
                   
        }

        /// <summary>
        /// Gets subcats by employee item roles
        /// </summary>
        /// <param name="employeeId">The ID of the <see cref="Employee"/>this request relates to</param>
        /// <param name="includeEmployeeItemRoleDates">True if the method should return "StartDate" and "EndDate" populated from the EmployeeItemRoles</param>
        /// <returns>A list of <see cref="SubcatItemRoleBasic"></see></returns>
        public List<SubcatItemRoleBasic> GetSubCatsByEmployeeItemRoles(int employeeId, bool includeEmployeeItemRoleDates = false)
        {
            var subcats = new List<SubcatItemRoleBasic>();
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                var globalCurrencies = new cGlobalCurrencies();
                var currencies = new cCurrencies(this._accountid, currentUser.CurrentSubAccountId);
                var subAccs = new cAccountSubAccounts(this._accountid);
                cAccountProperties properties = subAccs.getSubAccountById(subAccs.getFirstSubAccount().SubAccountID).SubAccountProperties;

                cCurrency currency = currencies.getCurrencyById((int)properties.BaseCurrency);
                string symbol = currency != null ? globalCurrencies.getGlobalCurrencyById(currency.globalcurrencyid).symbol : "£";

                connection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);

                using (IDataReader reader = connection.GetReader("ItemRoleBasedSubcatInfo", CommandType.StoredProcedure))
                {
                    connection.sqlexecute.Parameters.Clear();
                    var subCatIdOrd = reader.GetOrdinal("subcatid");
                    var maximumOrd = reader.GetOrdinal("maximum");
                    var receiptmaximumOrd = reader.GetOrdinal("receiptmaximum");
                    var subcatOrd = reader.GetOrdinal("subcat");
                    var shortsubcatOrd = reader.GetOrdinal("shortsubcat");
                    var categoryidOrd = reader.GetOrdinal("categoryid");
                    var calculationOrd = reader.GetOrdinal("calculation");
                    var fromappOrd = reader.GetOrdinal("fromapp");
                    var toappOrd = reader.GetOrdinal("toapp");
                    var descriptionOrd = reader.GetOrdinal("description");
                    var roleidOrd = reader.GetOrdinal("roleid");
                    var startDateOrd = reader.GetOrdinal("startDate");
                    var endDateOrd = reader.GetOrdinal("endDate");
                    while (reader.Read())
                    {
                        int subcatId = reader.GetInt32(subCatIdOrd);
                        decimal maximum = reader.IsDBNull(maximumOrd) ? 0 : reader.GetDecimal(maximumOrd);
                        decimal receiptMaximum = reader.IsDBNull(receiptmaximumOrd) ? 0 : reader.GetDecimal(receiptmaximumOrd);
                        string subcat = reader.GetString(subcatOrd);
                        string shortSubcat = reader.IsDBNull(shortsubcatOrd) ? string.Empty : reader.GetString(shortsubcatOrd);
                        int categoryId = reader.IsDBNull(categoryidOrd) ? 0 : reader.GetInt32(categoryidOrd);
                        var calculationType = (CalculationType)Convert.ToInt32(reader[calculationOrd]);
                        bool fromApp = !reader.IsDBNull(7) && reader.GetBoolean(fromappOrd);
                        bool toApp = !reader.IsDBNull(8) && reader.GetBoolean(toappOrd);
                        string description = reader.IsDBNull(descriptionOrd) ? string.Empty : reader.GetString(descriptionOrd);
                        int itemroleid = reader.GetInt32(roleidOrd);
                        var startDate = reader.IsDBNull(startDateOrd) || !includeEmployeeItemRoleDates
                            ? DateTime.MinValue
                            : reader.GetDateTime(startDateOrd);
                        var endDate = reader.IsDBNull(endDateOrd) || !includeEmployeeItemRoleDates
                            ? DateTime.MaxValue
                            : reader.GetDateTime(endDateOrd);
                        subcats.Add(new SubcatItemRoleBasic { Maximum = maximum, ReceiptMaximum = receiptMaximum, ShortSubcat = shortSubcat, Subcat = subcat, SubcatId = subcatId, CategoryId = categoryId, CalculationType = calculationType, FromApp = fromApp, ToApp = toApp, Description = description, ItemRoleID = itemroleid, CurrencySymbol = symbol, StartDate = startDate, EndDate = endDate});
                    }
                }
            }

            return subcats;
        }

        /// <summary>
        /// Gets the item role IDs that exists for the given subcat
        /// </summary>
        /// <param name="subcatID">The ID of the subcat to get the item roles for</param>
        /// <param name="connection">The database connection</param>
        /// <returns>A list of item role ids</returns>
        public List<int> GetItemRolesForSubcat(int subcatID, IDBConnection connection = null)
        {
            List<int> itemroles = new List<int>();
            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@subcatid", subcatID);
                using (
                    IDataReader reader = databaseConnection.GetReader(
                        "GetItemRolesForSubcat",
                        CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        itemroles.Add(reader.GetInt32(0));
                    }
                    reader.Close();
                }
            }

            return itemroles;
        }

        /// <summary>
        /// Gets a sub cat by id
        /// </summary>
        /// <param name="subcatid"></param>
        /// <returns></returns>
        public cSubcat GetSubcatById(int subcatid)
        {
            cSubcat subcat = null;
            _list.TryGetValue(subcatid, out subcat);
            return subcat;
        }

        /// <summary>
        /// Gets a subcat by string name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public cSubcat GetSubcatByString(string name)
        {
            return (from subcat in _list.Values
                    where subcat.subcat == name
                    select subcat).FirstOrDefault();
        }

        /// <summary>
        /// Saves a subcat
        /// </summary>
        /// <param name="subcat"></param>
        /// <returns></returns>
        public int SaveSubcat(cSubcat subcat)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int subcatId;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@subcatid", subcat.subcatid);
                connection.sqlexecute.Parameters.AddWithValue("@categoryid", subcat.categoryid);
                connection.sqlexecute.Parameters.AddWithValue("@subcat", subcat.subcat);
                connection.sqlexecute.Parameters.AddWithValue("@description", subcat.description);
                connection.sqlexecute.Parameters.AddWithValue("@mileageapp", Convert.ToByte(subcat.mileageapp));
                connection.sqlexecute.Parameters.AddWithValue("@staffapp", Convert.ToByte(subcat.staffapp));
                connection.sqlexecute.Parameters.AddWithValue("@othersapp", Convert.ToByte(subcat.othersapp));
                connection.sqlexecute.Parameters.AddWithValue("@tipapp", Convert.ToByte(subcat.tipapp));
                connection.sqlexecute.Parameters.AddWithValue("@pmilesapp", Convert.ToByte(subcat.pmilesapp));
                connection.sqlexecute.Parameters.AddWithValue("@bmilesapp", Convert.ToByte(subcat.bmilesapp));
                connection.sqlexecute.Parameters.AddWithValue("@allowanceamount", subcat.allowanceamount);
                connection.sqlexecute.Parameters.AddWithValue("@accountcode", subcat.accountcode);
                connection.sqlexecute.Parameters.AddWithValue("@attendeesapp", Convert.ToByte(subcat.attendeesapp));
                connection.sqlexecute.Parameters.AddWithValue("@addasnet", Convert.ToByte(subcat.addasnet));

                if (subcat.pdcatid == 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@pdcatid", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@pdcatid", subcat.pdcatid);
                }

                connection.sqlexecute.Parameters.AddWithValue("@eventinhomeapp", Convert.ToByte(subcat.eventinhomeapp));
                connection.sqlexecute.Parameters.AddWithValue("@receiptapp", Convert.ToByte(subcat.receiptapp));
                connection.sqlexecute.Parameters.AddWithValue("@nopassengersapp", Convert.ToByte(subcat.nopassengersapp));
                connection.sqlexecute.Parameters.AddWithValue("@passengernamesapp", Convert.ToByte(subcat.passengernamesapp));
                connection.sqlexecute.Parameters.AddWithValue("@calculation", (byte)subcat.calculation);
                connection.sqlexecute.Parameters.AddWithValue("@passengersapp", Convert.ToByte(subcat.passengersapp));
                connection.sqlexecute.Parameters.AddWithValue("@comment", subcat.comment);
                connection.sqlexecute.Parameters.AddWithValue("@splitentertainment", Convert.ToByte(subcat.splitentertainment));

                if (subcat.entertainmentid == 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@entertainmentid", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@entertainmentid", subcat.entertainmentid);
                }

                connection.sqlexecute.Parameters.AddWithValue("@reimbursable", subcat.reimbursable);
                connection.sqlexecute.Parameters.AddWithValue("@nonightsapp", Convert.ToByte(subcat.nonightsapp));
                connection.sqlexecute.Parameters.AddWithValue("@attendeesmand", Convert.ToByte(subcat.attendeesmand));
                connection.sqlexecute.Parameters.AddWithValue("@nodirectorsapp", Convert.ToByte(subcat.nodirectorsapp));
                connection.sqlexecute.Parameters.AddWithValue("@hotelapp", Convert.ToByte(subcat.hotelapp));
                connection.sqlexecute.Parameters.AddWithValue("@noroomsapp", Convert.ToByte(subcat.noroomsapp));
                connection.sqlexecute.Parameters.AddWithValue("@hotelmand", Convert.ToByte(subcat.hotelmand));
                connection.sqlexecute.Parameters.AddWithValue("@vatnumberapp", Convert.ToByte(subcat.vatnumberapp));
                connection.sqlexecute.Parameters.AddWithValue("@vatnumbermand", Convert.ToByte(subcat.vatnumbermand));
                connection.sqlexecute.Parameters.AddWithValue("@nopersonalguestsapp", Convert.ToByte(subcat.nopersonalguestsapp));
                connection.sqlexecute.Parameters.AddWithValue("@noremoteworkersapp", Convert.ToByte(subcat.noremoteworkersapp));

                if (subcat.alternateaccountcode == string.Empty)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@alternateaccountcode", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@alternateaccountcode", subcat.alternateaccountcode);
                }

                connection.sqlexecute.Parameters.AddWithValue("@splitpersonal", Convert.ToByte(subcat.splitpersonal));
                connection.sqlexecute.Parameters.AddWithValue("@splitremote", Convert.ToByte(subcat.splitremote));

                if (subcat.personalid == 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@personalid", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@personalid", subcat.personalid);
                }

                if (subcat.remoteid == 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@remoteid", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@remoteid", subcat.remoteid);
                }

                connection.sqlexecute.Parameters.AddWithValue("@reasonapp", Convert.ToByte(subcat.reasonapp));
                connection.sqlexecute.Parameters.AddWithValue("@otherdetailsapp", Convert.ToByte(subcat.otherdetailsapp));

                if (subcat.shortsubcat == string.Empty)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@shortsubcat", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@shortsubcat", subcat.shortsubcat);
                }

                connection.sqlexecute.Parameters.AddWithValue("@fromapp", Convert.ToByte(subcat.fromapp));
                connection.sqlexecute.Parameters.AddWithValue("@toapp", Convert.ToByte(subcat.toapp));
                connection.sqlexecute.Parameters.AddWithValue("@companyapp", Convert.ToByte(subcat.companyapp));

                if (subcat.subcatid > 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@date", subcat.modifiedon);
                    connection.sqlexecute.Parameters.AddWithValue("@userid", subcat.modifiedby);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@date", subcat.createdon);
                    connection.sqlexecute.Parameters.AddWithValue("@userid", subcat.createdby);
                }

                connection.sqlexecute.Parameters.AddWithValue("@enableHomeToLocationMileage", Convert.ToByte(subcat.EnableHomeToLocationMileage));
                connection.sqlexecute.Parameters.AddWithValue("@homeToLocationType", (byte)subcat.HomeToLocationType);

                if (subcat.MileageCategory == null || (int)subcat.MileageCategory == 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@mileageCategory", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@mileageCategory", (int)subcat.MileageCategory);
                }

                connection.sqlexecute.Parameters.AddWithValue("@isRelocationMileage", Convert.ToByte(subcat.IsRelocationMileage));

                if (subcat.reimbursableSubcatID != null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@reimbursableSubcatID", subcat.reimbursableSubcatID);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@reimbursableSubcatID", DBNull.Value);
                }

                connection.sqlexecute.Parameters.AddWithValue("@allowHeavyBulkyMileage", subcat.allowHeavyBulkyMileage);

                if (subcat.StartDate == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@startDate", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@startDate", subcat.StartDate);
                }

                if (subcat.EndDate == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@endDate", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@endDate", subcat.EndDate);
                }

                connection.sqlexecute.Parameters.AddWithValue("@validate", subcat.Validate);

                if (currentUser != null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                connection.sqlexecute.Parameters.AddWithValue("@homeToOfficeAlwaysZero", subcat.HomeToOfficeAlwaysZero);
                if (subcat.HomeToOfficeFixedMiles.HasValue)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@HomeToOfficeFixedMiles", subcat.HomeToOfficeFixedMiles);    
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@HomeToOfficeFixedMiles", DBNull.Value);
                }
                
                if (subcat.PublicTransportRate == null || (int)subcat.PublicTransportRate == 0)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@publicTransportRate", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@publicTransportRate", (int)subcat.PublicTransportRate);
                }
              
                connection.sqlexecute.Parameters.AddWithValue("@enableDutyOfCare", Convert.ToByte(subcat.EnableDoC));
                connection.sqlexecute.Parameters.AddWithValue("@requireClassOneBusinessInsurance", Convert.ToByte(subcat.RequireClass1BusinessInsurance));
                connection.sqlexecute.Parameters.AddWithValue("@enforceToOfficeMileageCap", Convert.ToByte(subcat.EnforceToOfficeMileageCap));
                if (subcat.HomeToOfficeMileageCap.HasValue)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@homeToOfficeMileageCap", subcat.HomeToOfficeMileageCap);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@homeToOfficeMileageCap", DBNull.Value);
                }


                connection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                connection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("saveSubcat");

                subcatId = (int)connection.sqlexecute.Parameters["@returnvalue"].Value;
            }

            if (subcatId == -1)
            {
                return -1;
            }

            subcat.updateID(subcatId);
            this.AddUserDefined(subcatId, subcat.associatedudfs);

            var tables = new cTables(this._accountid);
            var fields = new cFields(this._accountid);
            cTable table = tables.GetTableByID(new Guid("401b44d7-d6d8-497b-8720-7ffcc07d635d"));
            var userDefined = new cUserdefinedFields(this._accountid);

            userDefined.SaveValues(
                tables.GetTableByID(table.UserDefinedTableID),
                subcatId,
                subcat.userdefined,
                tables,
                fields,
                currentUser,
                elementId: (int)SpendManagementElement.ExpenseItems,
                record: subcat.subcat);

            this.InsertAllowances(subcat.allowances, subcatId);
            this.InsertCountries(subcatId, subcat.countries);

            // add / update the validation criteria if necessary.
            if (currentUser.Account.ValidationServiceEnabled)
            {
                // validation manager manages all Expedite expense validation
                var validationManager = new ExpenseValidationManager(currentUser.AccountID);

                // attempt to find custom crtieria for this subcat.
                var existingCriteria = validationManager.GetCriteriaForSubcat(subcatId);

                // loop through the subcat's criteria
                subcat.ValidationRequirements.ForEach(requirement =>
                {
                    // if it exists, update it or delete it depending on the text content.
                    if (existingCriteria.Select(x => x.Id).Contains(requirement.Id))
                    {
                        if (string.IsNullOrWhiteSpace(requirement.Requirements))
                        {
                            validationManager.DeleteCriterion(requirement.Id);
                        }
                        else
                        {
                            var criterion = existingCriteria.Find(x => x.Id == requirement.Id);
                            criterion.Requirements = requirement.Requirements;
                            validationManager.EditCriterion(criterion);
                        }
                    }
                    else  // create a new one.
                    {
                        if (!string.IsNullOrWhiteSpace(requirement.Requirements))
                        {
                            validationManager.AddCriterion(new ExpenseValidationCriterion
                            {
                                Enabled = subcat.Validate,
                                AccountId = currentUser.AccountID,
                                SubcatId = subcatId,
                                Requirements = requirement.Requirements
                            });
                        }
                    }

                });
            }

            this.InvalidateCache();
            return subcatId;
        }

        /// <summary>
        /// Save vat rate
        /// </summary>
        /// <param name="rate"></param>
        public void SaveVatRate(cSubcatVatRate rate)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();

                connection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                connection.sqlexecute.Parameters.AddWithValue("@vatrateid", rate.vatrateid);
                connection.sqlexecute.Parameters.AddWithValue("@subcatid", rate.subcatid);
                connection.sqlexecute.Parameters.AddWithValue("@vatamount", rate.vatamount);
                connection.sqlexecute.Parameters.AddWithValue("@vatreceipt", Convert.ToByte(rate.vatreceipt));
                connection.sqlexecute.Parameters.AddWithValue("@vatpercent", rate.vatpercent);

                if (rate.vatlimitwith == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@vatlimitwith", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@vatlimitwith", rate.vatlimitwith);
                }

                if (rate.vatlimitwithout == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@vatlimitwithout", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@vatlimitwithout", rate.vatlimitwithout);
                }

                connection.sqlexecute.Parameters.AddWithValue("@daterangetype", (byte)rate.daterangetype);

                if (rate.datevalue1 == null)
                {
                    connection.sqlexecute.Parameters.AddWithValue("@datevalue1", DBNull.Value);
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@datevalue1", ((DateTime)rate.datevalue1).ToString("yyyy/MM/dd"));
                }

                if (rate.daterangetype == DateRangeType.Between)
                {
                    if (rate.datevalue2 != null)
                    {
                        connection.sqlexecute.Parameters.AddWithValue("@datevalue2", ((DateTime)rate.datevalue2).ToString("yyyy/MM/dd"));
                    }
                }
                else
                {
                    connection.sqlexecute.Parameters.AddWithValue("@datevalue2", DBNull.Value);
                }

                connection.ExecuteProc("saveSubcatVatRate");
                connection.sqlexecute.Parameters.Clear();
            }
            this.InvalidateCache();
        }

        /// <summary>
        /// Validates a subcat to ensure it's a valid Subcat relating to mileage
        /// </summary>
        /// <param name="subcatId">The subcat</param>
        /// <param name="employeeId">The employeeId</param>
        /// <returns>False if the employee does not have access to the subcat, or the subcat is not valid for mileage, otherwise true.</returns>
        public bool ValidateSubcatForMileage(int subcatId, int employeeId)
        {
            var subCat = this.GetSubcatById(subcatId);

            if (subCat == null)
            {
                return false;
            }
        
            //Check employee has access to the mileage subcat
            List<SubcatItemRoleBasic> subcatItemRoleBasics = this.GetSubCatsByEmployeeItemRoles(employeeId).DistinctBy(x => x.SubcatId).ToList();

            if (!subcatItemRoleBasics.Any(x => x.SubcatId == subcatId))
            {
                return false;
            }


            if (subCat.calculation != CalculationType.PencePerMile &&
                subCat.calculation != CalculationType.PencePerMileReceipt)
            {
                return false;
            }
          
            if (!subCat.fromapp && !subCat.toapp)
            {
                return false;
            }
       
            return true;
        }

        /// <summary>
        /// Gets a SortedList of subcats for a category Id
        /// </summary>
        /// <param name="categoryId">The categoryId</param>
        /// <param name="isCorpCard">Whether it's for a corporate card reconciliation</param>
        /// <param name="isMobileJourney">Whether its for a mobile journey reconciliation</param>
        /// <param name="employeeId">The employee Id</param>
        /// <param name="itemRoleSubCats">A list of <see cref="SubcatItemRoleBasic"/></param>
        /// <returns>A SortedList of subcat details</returns>
        public SortedList<int, SubcatItemRoleBasic> GetExpenseSubCatsForCategory(
            int categoryId,
            bool isCorpCard,
            bool isMobileJourney,
            int employeeId,
            List<SubcatItemRoleBasic> itemRoleSubCats)
        {
            var subcats = new SortedList<int, SubcatItemRoleBasic>();

            var sorted = new SortedList<string, SubcatItemRoleBasic>();

            foreach (SubcatItemRoleBasic itemRoleSubCat in itemRoleSubCats)
            {
                if (itemRoleSubCat.CategoryId == categoryId && !sorted.ContainsKey(itemRoleSubCat.Subcat))
                {
                    sorted.Add(itemRoleSubCat.Subcat, itemRoleSubCat);
                }
            }

            foreach (SubcatItemRoleBasic rolesub in sorted.Values)
                
                if (isMobileJourney)
                {
                    if ((rolesub.CalculationType == CalculationType.PencePerMile ||
                         rolesub.CalculationType == CalculationType.PencePerMileReceipt) && (rolesub.FromApp && rolesub.ToApp))
                    {

                        if (!subcats.ContainsKey(rolesub.SubcatId))
                        {
                            subcats.Add(rolesub.SubcatId, rolesub);
                        }
                    }
                }
                else if (!isCorpCard ||
                          (isCorpCard && rolesub.CalculationType != CalculationType.FixedAllowance &&
                          (rolesub.CalculationType == CalculationType.NormalItem ||
                           rolesub.CalculationType == CalculationType.Meal ||
                           rolesub.CalculationType == CalculationType.FuelReceipt ||
                           rolesub.CalculationType == CalculationType.FuelCardMileage)))
                {
                    if (!subcats.ContainsKey(rolesub.SubcatId))
                    {
                        subcats.Add(rolesub.SubcatId, rolesub);
                    }
                }
            

            return subcats;
        }

        /// <summary>
        /// Assigns a P11D category to a list of subcats by their Id
        /// </summary>
        /// <param name="subcatids">The Ids of the subcats</param>
        /// <param name="pdcatid">The Id of the P11D Category/></param>
        public void AssignP11DToSubcats(int[] subcatids, int pdcatid)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                connection.AddWithValue("@pdcatid", pdcatid);
                var strsql = "update subcats set pdcatid = null where pdcatid = @pdcatid";

                connection.ExecuteSQL(strsql);
                connection.sqlexecute.Parameters.Clear();
            }

            if (subcatids.Length == 0) //no subcats, exit function
            {
                return;
            }

            int i = 0;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                connection.AddWithValue("@pdcatid", pdcatid);
                var strsql = "update subcats set pdcatid = " + pdcatid + " where ";

                for (i = 0; i < subcatids.Length; i++)
                {
                    strsql = strsql + "subcatid = @subcatid" + i + " or ";
                    connection.AddWithValue("@subcatid" + i, subcatids[i]);
                }
                strsql = strsql.Remove(strsql.Length - 4, 4);
                connection.ExecuteSQL(strsql);
                connection.sqlexecute.Parameters.Clear();
            }

            this.InvalidateCache();
        }

        #endregion

        #region Methods

        private void AddUserDefined(int subcatid, IEnumerable<int> userDefinedIds)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                int counter = 0;
                connection.sqlexecute.Parameters.AddWithValue("@subcatid", subcatid);
                string sql = "delete from dbo.[subcats_userdefined] where subcatid = @subcatid";
                connection.ExecuteSQL(sql);

                foreach (int userDefinedId in userDefinedIds)
                {
                    sql = string.Format("insert into dbo.[subcats_userdefined] (subcatid, userdefineid) values (@subcatid,@userdefined{0})", counter);
                    connection.sqlexecute.Parameters.AddWithValue("@userdefined" + counter, userDefinedId);
                    connection.ExecuteSQL(sql);
                    counter++;
                }

                connection.sqlexecute.Parameters.Clear();
            }
            this.InvalidateCache();
        }

        /// <summary>
        /// Gets the UDFs associated with a subcat
        /// </summary>
        /// <param name="subcatId"></param>
        /// <returns>A list of UDF ids</returns>
        private Dictionary<int, List<int>> GetAssociatedUdFs()
        {
            Dictionary<int, List<int>> list = new Dictionary<int, List<int>>();
            List<int> subcatUdfList;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                string sql = "select userdefineid, subcatid from dbo.subcats_userdefined";
                connection.sqlexecute.Parameters.Clear();
                

                using (IDataReader reader = connection.GetReader(sql))
                {
                    while (reader.Read())
                    {
                        int userDefinedId = reader.GetInt32(0);
                        int subcatid = reader.GetInt32(1);
                        list.TryGetValue(subcatid, out subcatUdfList);
                        if (subcatUdfList == null)
                        {
                            subcatUdfList = new List<int>();
                            list.Add(subcatid, subcatUdfList);
                        }

                        subcatUdfList.Add(userDefinedId);
                    }

                    reader.Close();
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the split items associated with a subcat
        /// </summary>
        /// <param name="subcatId"></param>
        /// <returns>A list of split item ids</returns>
        private Dictionary<int, List<int>> GetSubcatSplit()
        {
            Dictionary<int, List<int>> list = new Dictionary<int, List<int>>();
            List<int> subcatSplits;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                string sql = "select splitsubcatid, primarysubcatid from dbo.subcat_split";
                connection.sqlexecute.Parameters.Clear();
                
                using (IDataReader reader = connection.GetReader(sql))
                {

                    while (reader.Read())
                    {
                        int splitSubcatId = reader.GetInt32(0);
                        int subcatid = reader.GetInt32(1);
                        list.TryGetValue(subcatid, out subcatSplits);
                        if (subcatSplits == null)
                        {
                            subcatSplits = new List<int>();
                            list.Add(subcatid, subcatSplits);
                        }

                        subcatSplits.Add(splitSubcatId);
                    }

                    reader.Close();
                }

                connection.sqlexecute.Parameters.Clear();
            }

            return list;
        }

        /// <summary>
        /// Gets the VAT rates associated with a subcat
        /// </summary>
        /// <param name="subcatId"></param>
        /// <returns>A list of VAT rate ids</returns>
        private Dictionary<int, List<cSubcatVatRate>> GetVatRates()
        {
            Dictionary<int, List<cSubcatVatRate>> list = new Dictionary<int, List<cSubcatVatRate>>();
            List<cSubcatVatRate> subcatVatRates;
            string sql = "select vatrateid, subcatid, vatamount, vatreceipt, vatlimitwithout, vatlimitwith, " +
                                       "vatpercent, daterangetype, datevalue1, datevalue2 from dbo.subcat_vat_rates";

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                connection.sqlexecute.Parameters.Clear();
                

                using (IDataReader reader = connection.GetReader(sql))
                {
                    

                    int vateRateIdOrdinal = reader.GetOrdinal("vatrateid");
                    int vatAmountOrdinal = reader.GetOrdinal("vatamount");
                    int vatReceiptOrdinal = reader.GetOrdinal("vatreceipt");
                    int vatLimitWithoutOrdinal = reader.GetOrdinal("vatlimitwithout");
                    int vatLimitWithOrdinal = reader.GetOrdinal("vatlimitwith");
                    int vatPercentOrdinal = reader.GetOrdinal("vatpercent");
                    int dateRangeTypeOrdinal = reader.GetOrdinal("daterangetype");
                    int dateValue1Ordinal = reader.GetOrdinal("datevalue1");
                    int dateValue2Ordinal = reader.GetOrdinal("datevalue2");
                    int subcatIdOrdinal = reader.GetOrdinal("subcatid");

                    while (reader.Read())
                    {
                        int subcatId = reader.GetInt32(subcatIdOrdinal);
                        int vatRateId = reader.GetInt32(vateRateIdOrdinal);
                        double vatAmount = reader.GetDouble(vatAmountOrdinal);
                        bool vatReceipt = reader.GetBoolean(vatReceiptOrdinal);
                        decimal? vatLimitWithout;

                        if (reader.IsDBNull(vatLimitWithoutOrdinal))
                        {
                            vatLimitWithout = null;
                        }
                        else
                        {
                            vatLimitWithout = reader.GetDecimal(vatLimitWithoutOrdinal);
                        }

                        decimal? vatLimitWith;

                        if (reader.IsDBNull(vatLimitWithOrdinal))
                        {
                            vatLimitWith = null;
                        }
                        else
                        {
                            vatLimitWith = reader.GetDecimal(vatLimitWithOrdinal);
                        }

                        byte vatPercent = reader.GetByte(vatPercentOrdinal);
                        var dateRangeType = (DateRangeType)reader.GetByte(dateRangeTypeOrdinal);
                        DateTime? dateValue1;

                        if (reader.IsDBNull(dateValue1Ordinal))
                        {
                            dateValue1 = null;
                        }
                        else
                        {
                            dateValue1 = reader.GetDateTime(dateValue1Ordinal);
                        }

                        DateTime? datevalue2;

                        if (reader.IsDBNull(dateValue2Ordinal))
                        {
                            datevalue2 = null;
                        }
                        else
                        {
                            datevalue2 = reader.GetDateTime(dateValue2Ordinal);
                        }

                        list.TryGetValue(subcatId, out subcatVatRates);
                        if (subcatVatRates == null)
                        {
                            subcatVatRates = new List<cSubcatVatRate>();
                            list.Add(subcatId, subcatVatRates);
                        }

                        subcatVatRates.Add(
                            new cSubcatVatRate(
                                vatRateId,
                                subcatId,
                                vatAmount,
                                vatReceipt,
                                vatLimitWithout,
                                vatLimitWith,
                                vatPercent,
                                dateRangeType,
                                dateValue1,
                                datevalue2));
                    }

                    reader.Close();
                }
            }

            return list;
        }

        /// <summary>
        /// Deletes the allowances associated with a subcat and inserts the updated allowances for the subcat
        /// </summary>
        /// <param name="allowances"></param>
        /// <param name="subcatId"></param>
        private void InsertAllowances(IEnumerable<int> allowances, int subcatId)
        {
            if (!allowances.Any()) return;
            SqlMetaData[] tvpItems = { new SqlMetaData("ID", SqlDbType.Int) };
            var allowanceIds = new List<SqlDataRecord>();

            foreach (int allowanceId in allowances)
                {
                var row = new SqlDataRecord(tvpItems);
                row.SetInt32(0, allowanceId);
                allowanceIds.Add(row);
                }

            var db = new DBConnection(cAccounts.getConnectionString(this._accountid));    
            db.sqlexecute.Parameters.Clear();
            db.sqlexecute.Parameters.AddWithValue("@SubcatId", subcatId);
            db.sqlexecute.Parameters.Add("@AllowanceIds", SqlDbType.Structured);
            db.sqlexecute.Parameters["@AllowanceIds"].Direction = ParameterDirection.Input;
            db.sqlexecute.Parameters["@AllowanceIds"].Value = allowanceIds;
            db.ExecuteProc("SaveAllowancesToSubcat");
            this.InvalidateCache();
        }

        private void InsertCountries(int subcatId, IEnumerable<cCountrySubcat> countries)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                string sql = "delete from dbo.[subcats_countries] where subcatid = @subcatid";
                connection.sqlexecute.Parameters.AddWithValue("@subcatid", subcatId);
                connection.ExecuteSQL(sql);
                connection.sqlexecute.Parameters.Clear();

                foreach (cCountrySubcat countrysub in countries)
                {
                    sql = "insert into [subcats_countries] (subcatid, countryid, accountcode) values (@subcatid, @countryid, @accountcode)";
                    connection.sqlexecute.Parameters.AddWithValue("@subcatid", subcatId);
                    connection.sqlexecute.Parameters.AddWithValue("@countryid", countrysub.countryid);
                    connection.sqlexecute.Parameters.AddWithValue("@accountcode", countrysub.accountcode);
                    connection.ExecuteSQL(sql);
                    connection.sqlexecute.Parameters.Clear();
                }
            }
            this.InvalidateCache();
        }

        /// <summary>
        /// Get a list of subcats by a P11D Category Id
        /// </summary>
        /// <param name="pdcatId">The id of the P11D Category</param>
        /// <returns></returns>
        public DataSet GetSubCatList(int pdcatId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._accountid)))
            {
                connection.AddWithValue("@pdcatid", pdcatId);
                var strsql = pdcatId == 0 ? "select subcat, subcatid, pdcatid from subcats where pdcatid is null order by subcat" : "select subcat, subcatid, pdcatid from subcats where (pdcatid = @pdcatid or pdcatid is null) order by subcat";

                DataSet temp = connection.GetDataSet(strsql);
                connection.sqlexecute.Parameters.Clear();
                return temp;
            }
        }

        #endregion
    }
}