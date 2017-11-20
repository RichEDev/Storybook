using System.Diagnostics;
using System.Linq;
using SpendManagementLibrary.Interfaces.Expedite;
using Spend_Management;

namespace SpendManagementLibrary
{
    using System.Data;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Expedite;

    using Addresses;
    using Enumerators.Expedite;
    using Helpers;
    using Interfaces;
    using SpendManagementLibrary.Flags;
    using System.Activities.Validation;

    [Serializable()]
    [DataContract]
    public class cExpenseItem
    {

        #region core fields
        private int nExpenseid;
        private ItemType itItemtype;
        private string sReason;
        private bool bReceipt;
        private decimal dNet;
        private decimal dVat;
        private decimal dTotal;
        private int nSubcatid;
        private DateTime dtDate;
        private int nCompanyid;
        private string sRefnum;
        private bool bPrimaryItem = true;
        private int nBasecurrency;
        private int nGlobalBasecurrency;
        private double dGlobalExchangerate;
        private decimal dGlobalTotal;
        #endregion

        #region mileage fields
        
        private decimal dBmiles;
        private decimal dPmiles;
        #endregion

        #region state fields
        private bool bReturned;
        private bool bCorrected;

        private string note;

        private string dispute;
        #endregion

        private byte bStaff;
        private byte bOthers;
        private bool bHome;

        public int nClaimid;
        private int nPlitres;
        private int nBlitres;
        private int nCurrencyid;
        private string sAttendees;
        private decimal dTip;
        private int nCountryid;
        private decimal dForeignvat;
        private decimal dConvertedtotal;
        private double dExchangerate;

        private bool bTempallow;
        
        private int nReasonid;
        private bool bNormalreceipt;
        private bool bReceiptattached;
        private DateTime dtAllowancestartdate;
        private DateTime dtAllowanceenddate;
        private int nCarid;
        private decimal dAllowancededuct;
        private int nAllowanceid;
        private byte bNonights;
        private byte bNorooms;
        private double dQuantity;
        private byte bDirectors;
        private decimal dAmountpayable;
        private int nHotelid;
        private string sVatnumber;
        private byte bPersonalguests;
        private byte bRemoteworkers;
        private string sAccountcode = "";
        private int nFloatid;
        private int nTransactionid;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime dtModifiedon;
        private int nModifiedby;
        private int nMileageid;
        private cExpenseItem clsPrimaryItem;
        private MileageUOM eJourneyUnit;
        //private string sAssignmentNum;
        private long nESRAssignmentId;
        private HomeToLocationType eHomeToOfficeDeductionMethod;
        private bool bIsMobileItem;
        private int nMobileDeviceTypeId;
        private int? nItemCheckerId;
        private int? nBankAccountId;

        List<cDepCostItem> lstCostcodeBreakdown;
        List<cExpenseItem> lstSplititems = new List<cExpenseItem>();
        List<FlagSummary> lstFlags;
        SortedList<int, object> lstUserdefined = new SortedList<int, object>();
        SortedList<int, cJourneyStep> lstJourneySteps = new SortedList<int, cJourneyStep>();

        /// <summary>
        /// Obtain a list of waypoints (postcodes) for a mileage expense item
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="expenseId">The expense item Id</param>
        /// <param name="connection">An alternative databaseconnection to use</param>
        /// <returns>A list of waypoints (postcodes)</returns>
        public static List<string> GetWaypointsByExpenseId(int accountId, int expenseId, IDBConnection connection = null)
        {
            var waypoints = new List<string>();

            using (IDBConnection databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                const string Sql = "SELECT startAddresses.AddressId, endAddresses.AddressId FROM savedexpenses_journey_steps "
                                 + "INNER JOIN addresses AS startAddresses ON startAddresses.AddressId = savedexpenses_journey_steps.StartAddressId "
                                 + "INNER JOIN addresses AS endAddresses ON endAddresses.AddressId = savedexpenses_journey_steps.EndAddressId "
                                 + "WHERE savedexpenses_journey_steps.expenseid = @expenseId ORDER BY step_number";
                var account = new cAccounts().GetAccountByID(accountId);

                databaseConnection.AddWithValue("@expenseId", expenseId);

                using (IDataReader reader = databaseConnection.GetReader(Sql))
                {
                    while (reader.Read())
                    {
                        // todo: return address information from the original query?
                        var origin = Address.Get(accountId, reader.GetInt32(0));
                        var destination = Address.Get(accountId, reader.GetInt32(1));

                        waypoints.Add(origin.GetLookupLocator(account.AddressInternationalLookupsAndCoordinates, account.AddressLookupProvider));
                        waypoints.Add(destination.GetLookupLocator(account.AddressInternationalLookupsAndCoordinates, account.AddressLookupProvider));
                    }

                    reader.Close();
                }
            }

            return waypoints;
        }

        public cExpenseItem(int expenseid, ItemType itemtype, decimal bmiles, decimal pmiles, string reason, bool receipt, decimal net, decimal vat, decimal total, int subcatid, DateTime date, byte staff, byte others, int companyid, bool returned, bool home, string refnum, int claimid, int plitres, int blitres, int currencyid, string attendees, decimal tip, int countryid, decimal foreignvat, decimal convertedtotal, double exchangerate, bool tempallow, int reasonid, bool normalreceipt, DateTime allowancestartdate, DateTime allowanceenddate, int carid, decimal allowancededuct, int allowanceid, byte nonights, double quantity, byte directors, decimal amountpayable, int hotelid, bool primaryitem, byte norooms, string vatnumber, byte personalguests, byte remoteworkers, string accountcode, int basecurrency, int globalbasecurrency, double globalexchangerate, decimal globaltotal, int floatid, bool corrected, bool receiptattached, int transactionid, DateTime createdon, int createdby, DateTime modifiedon, int modifiedby, int mileageid, MileageUOM journeyunit, long assignmentnum, HomeToLocationType hometoofficedeductionmethod, bool isMobileItem = false, int mobileDeviceTypeId = 0, string note = "", string dispute = "", int? itemCheckerId = null, int? itemCheckerTeamId = null, ExpenseValidationProgress validationProgress = ExpenseValidationProgress.ValidationServiceDisabled, int validationCount = 0, List<ExpenseValidationResult> validationResults = null, bool edited = false, bool paid = false, int? originalExpenseId = null, int bankAccountId = 0, int workAddressId = 0, bool? isItemEscalated = null,ExpediteOperatorValidationProgress operatorValidationProgress = ExpediteOperatorValidationProgress.Available, int? mobileMetricDeviceId = null)
        {
            
            nExpenseid = expenseid;
            itItemtype = itemtype;
            dBmiles = bmiles;
            dPmiles = pmiles;
            sReason = reason;
            bReceipt = receipt;
            dNet = net;
            dVat = vat;
            dTotal = total;
            nSubcatid = subcatid;


            dtDate = date;
            bStaff = staff;
            bOthers = others;
            nCompanyid = companyid;
            bReturned = returned;
            bCorrected = corrected;
            this.note = note;
            this.dispute = dispute;
            bHome = home;
            sRefnum = refnum;
            nClaimid = claimid;
            
            nPlitres = plitres;
            nBlitres = blitres;
            nCurrencyid = currencyid;
            sAttendees = attendees;
            dTip = tip;
            nCountryid = countryid;
            dForeignvat = foreignvat;
            dConvertedtotal = convertedtotal;
            dExchangerate = exchangerate;


            bTempallow = tempallow;
            nReasonid = reasonid;
            bReceiptattached = receiptattached;
            bNormalreceipt = normalreceipt;

            dtAllowancestartdate = allowancestartdate;
            dtAllowanceenddate = allowanceenddate;
            nCarid = carid;
            dAllowancededuct = allowancededuct;
            nAllowanceid = allowanceid;
            bNonights = nonights;
            dQuantity = quantity;
            bDirectors = directors;
            dAmountpayable = amountpayable;
            nHotelid = hotelid;
            bPrimaryItem = primaryitem;
            //**todo**
            //clsSplitItems = getSplitItems();
            bNorooms = norooms;
            sVatnumber = vatnumber;
            bPersonalguests = personalguests;
            bRemoteworkers = remoteworkers;
            sAccountcode = accountcode;
            nBasecurrency = basecurrency;
            nGlobalBasecurrency = globalbasecurrency;
            dGlobalTotal = globaltotal;
            dGlobalExchangerate = globalexchangerate;

            nFloatid = floatid;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
            nTransactionid = transactionid;
            nMileageid = mileageid;
            
            eJourneyUnit = journeyunit;
            //sAssignmentNum = assignmentnum;
            nESRAssignmentId = assignmentnum;
            eHomeToOfficeDeductionMethod = hometoofficedeductionmethod;

            bIsMobileItem = isMobileItem;
            nMobileDeviceTypeId = mobileDeviceTypeId;
            nItemCheckerId = itemCheckerId;
            this.ItemCheckerTeamId = itemCheckerTeamId;

            ValidationCount = validationCount;
            this.Edited = edited;
            this.Paid = paid;
            this.OriginalExpenseId = originalExpenseId;
            ValidationProgress = validationProgress;
            ValidationResults = validationResults ?? new List<ExpenseValidationResult>();
            nBankAccountId = bankAccountId;
            this.WorkAddressId = workAddressId;
            this.IsItemEscalated = isItemEscalated;
            OperatorValidationProgress = operatorValidationProgress;

            this.MobileMetricDeviceId = mobileMetricDeviceId;
        }
     
        /// <summary>
        /// How many times this item has been validated. Currently, items can be validated a maximum of twice.
        /// </summary>
        public int ValidationCount { get; set; }
        
        #region properties
        [DataMember]
        public int expenseid
        {
            get { return nExpenseid; }
            set { nExpenseid = value; }
        }
        [DataMember]
        public ItemType itemtype
        {
            get { return itItemtype; }
            set { itItemtype = value; }
        }
        [DataMember]
        public decimal miles
        {
            get 
            {
                decimal nummiles = 0;
                if (lstJourneySteps != null)
                {
                    foreach (cJourneyStep step in lstJourneySteps.Values)
                    {
                        nummiles += step.NumActualMiles;
                    }
                }
                return nummiles;
            }
            protected set { ;}
        }
        [DataMember]
        public decimal bmiles
        {
            get { return dBmiles; }
            protected set { ; }
        }
        [DataMember]
        public decimal pmiles
        {
            get { return dPmiles; }
            protected set { ; }
        }
        [DataMember]
        public string reason
        {
            get { return sReason; }
            set { sReason = value; }
        }
        [DataMember]
        public bool receipt
        {
            get { return bReceipt; }
            protected set { ; }
        }
        [DataMember]
        public decimal net
        {
            get { return dNet; }
            protected set { ; }
        }
        [DataMember]
        public decimal vat
        {
            get { return dVat; }
            protected set { ; }
        }
        [DataMember]
        public decimal total
        {
            get { return dTotal; }
            set { dTotal = value; }
        }
        [DataMember]
        public int subcatid
        {
            get { return nSubcatid; }
            set { nSubcatid = value; }
        }
        [DataMember]
        public DateTime date
        {
            get { return dtDate; }
            protected set { ; }
        }
        [DataMember]
        public byte staff
        {
            get { return bStaff; }
            set { bStaff = value; }
        }
        [DataMember]
        public byte others
        {
            get
            {
                return bOthers;
            }
            set { bOthers = value; }
        }
        [DataMember]
        public int companyid
        {
            get { return nCompanyid; }
            set { nCompanyid = value; }
        }
        [DataMember]
        public bool returned
        {
            get { return bReturned; }
            set { bReturned = value; }
        }
        [DataMember]
        public bool home
        {
            get { return bHome; }
            protected set { ; }
        }
        [DataMember]
        public string refnum
        {
            get { return sRefnum; }
            set
            {
                sRefnum = value;

            }
        }
        [DataMember]
        public int claimid
        {
            get { return nClaimid; }
            set { nClaimid = value; }
        }
        [DataMember]
        public int plitres
        {
            get { return nPlitres; }
            protected set { ; }
        }
        [DataMember]
        public int blitres
        {
            get { return nBlitres; }
            protected set { ; }
        }
        [DataMember]
        public int currencyid
        {
            get { return nCurrencyid; }
            set { nCurrencyid = value; }
        }
        [DataMember]
        public string attendees
        {
            get { return sAttendees; }
            set { sAttendees = value; }
        }
        [DataMember]
        public decimal tip
        {
            get { return dTip; }
            protected set { ; }
        }
        [DataMember]
        public int countryid
        {
            get { return nCountryid; }
            protected set { ; }
        }
        [DataMember]
        public decimal foreignvat
        {
            get { return dForeignvat; }
            protected set { ; }
        }
        [DataMember]
        public decimal convertedtotal
        {
            get { return dConvertedtotal; }
            set { dConvertedtotal = value; }
        }
        [DataMember]
        public double exchangerate
        {
            get { return dExchangerate; }
            set { dExchangerate = value; }
        }

        [DataMember]
        public bool tempallow
        {
            get { return bTempallow; }
            set { bTempallow = value; }
        }
        [DataMember]
        public int fromid
        {
            get 
            {
                int fromid = 0;
                if (lstJourneySteps != null)
                {
                    if (lstJourneySteps.Count > 0)
                    {
                        if (lstJourneySteps.Values[0].startlocation == null)
                        {
                            fromid = 0;
                        }
                        else
                        {
                            fromid = lstJourneySteps.Values[0].startlocation.Identifier;
                        }
                    }
                }
                return fromid;
            }
            protected set { ; }
        }
        [DataMember]
        public int toid
        {
            get
            {
                int toid = 0;
                if (lstJourneySteps != null)
                {
                    if (lstJourneySteps.Count > 0)
                    {
                        if (lstJourneySteps.Values[0].endlocation == null)
                        {
                            toid = 0;
                        }
                        else
                        {
                            toid = lstJourneySteps.Values[0].endlocation.Identifier;
                        }
                    }
                }
                return toid;
            }
            protected set { ; }
        }
        [DataMember]
        public int reasonid
        {
            get { return nReasonid; }
            protected set { ; }
        }
        [DataMember]
        public bool normalreceipt
        {
            get { return bNormalreceipt; }
            protected set { ; }
        }
        [DataMember]
        public bool receiptattached
        {
            get { return bReceiptattached; }
            set { bReceiptattached = value; }
        }
        [DataMember]
        public DateTime allowancestartdate
        {
            get { return dtAllowancestartdate; }
            protected set { ; }
        }
        [DataMember]
        public DateTime allowanceenddate
        {
            get { return dtAllowanceenddate; }
            protected set { ; }
        }
        [DataMember]
        public byte nopassengers
        {
            get
            {
                byte passengers = 0;
                if (lstJourneySteps != null)
                {
                    foreach (cJourneyStep step in lstJourneySteps.Values)
                    {
                        passengers += step.numpassengers;
                    }
                }
                return passengers;
            }
            protected set { ; }
        }
        [DataMember]
        public int carid
        {
            get { return nCarid; }
            set { nCarid = value; }
        }
        [DataMember]
        public decimal allowancededuct
        {
            get { return dAllowancededuct; }
            protected set { ; }
        }
        [DataMember]
        public int allowanceid
        {
            get { return nAllowanceid; }
            protected set { ; }
        }
        [DataMember]
        public byte nonights
        {
            get { return bNonights; }
            protected set { ; }
        }
        [DataMember]
        public double quantity
        {
            get { return dQuantity; }
            protected set { ; }
        }

        [DataMember]
        public byte directors
        {
            get { return bDirectors; }
            protected set { ; }
        }

        [DataMember]
        public decimal amountpayable
        {
            get { return dAmountpayable; }
            set { dAmountpayable = value; }
        }
        [DataMember]
        public int hotelid
        {
            get { return nHotelid; }
            protected set { ; }
        }
        [DataMember]
        public bool primaryitem
        {
            get { return bPrimaryItem; }
            protected set { ; }
        }
        [DataMember]
        public cExpenseItem parent
        {
            get { return clsPrimaryItem; }
            protected set { ; }
        }
        [DataMember]
        public byte norooms
        {
            get { return bNorooms; }
            protected set { ; }
        }
        [DataMember]
        public string vatnumber
        {
            get { return sVatnumber; }
            protected set { ; }
        }
        [DataMember]
        public byte personalguests
        {
            get
            {
                return bPersonalguests;

            }
            set { bPersonalguests = value; }
        }
        [DataMember]
        public byte remoteworkers
        {
            get
            {
                return bRemoteworkers;
            }
            set { bRemoteworkers = value; }
        }
        [DataMember]
        public string accountcode
        {
            get { return sAccountcode; }
            set { sAccountcode = value; }
        }
        [DataMember]
        public int basecurrency
        {
            get { return nBasecurrency; }
            protected set { ; }
        }
        [DataMember]
        public int globalbasecurrency
        {
            get { return nGlobalBasecurrency; }
            protected set { ; }
        }

        [DataMember]
        public double globalexchangerate
        {
            get { return dGlobalExchangerate; }
            protected set { ; }
        }
        [DataMember]
        public decimal globaltotal
        {
            get { return dGlobalTotal; }
            set { dGlobalTotal = value; }
        }
        [DataMember]
        public List<FlagSummary> flags
        {
            get { return lstFlags; }
            set { lstFlags = value; }
        }
        
        
        public List<cDepCostItem> costcodebreakdown
        {
            get { return lstCostcodeBreakdown; }
            set { lstCostcodeBreakdown = value; }
        }
        
        public int floatid
        {
            get { return nFloatid; }
            protected set { ; }
        }
        
        public List<cExpenseItem> splititems
        {
            get { return lstSplititems; }
            protected set { ; }
        }
        
        [DataMember]
        public bool corrected
        {
            get { return bCorrected; }
            set { bCorrected = value; }
        }

        [DataMember]
        public string Note
        {
            get { return this.note; }
            set { this.note = value; }
        }

        [DataMember]
        public string Dispute
        {
            get { return this.dispute; }
            set { this.dispute = value; }
        }

        public int transactionid
        {
            get { return nTransactionid; }
            set { nTransactionid = value; }
        }
        
        public SortedList<int, object> userdefined
        {
            get
            {
                
                return lstUserdefined; 
            }
            set { lstUserdefined = value; }
        }
        [DataMember]
        public decimal grandtotal
        {
            get
            {
                decimal grandtotal = total;
                foreach (cExpenseItem splititem in lstSplititems)
                {
                    grandtotal += splititem.grandtotal;
                }
                return grandtotal;
            }
            protected set { ; }
        }
        
        public decimal grandGlobalTotal
        {
            get
            {
                decimal grandtotal = globaltotal;
                foreach (cExpenseItem splititem in lstSplititems)
                {
                    grandtotal += splititem.grandGlobalTotal;
                }
                return grandtotal;
            }
            protected set { ; }
        }
        [DataMember]
        public decimal convertedgrandtotal
        {
            get
            {
                decimal grandtotal = convertedtotal;
                foreach (cExpenseItem splititem in lstSplititems)
                {
                    grandtotal += splititem.convertedgrandtotal;
                }
                return grandtotal;
            }
            protected set { ; }
        }
        [DataMember]
        public decimal grandnettotal
        {
            get
            {
                decimal grandtotal = net;
                foreach (cExpenseItem splititem in lstSplititems)
                {
                    grandtotal += splititem.grandnettotal;
                }
                return grandtotal;
            }
            protected set { ; }
        }
        [DataMember]
        public decimal grandvattotal
        {
            get
            {
                decimal grandtotal = vat;
                foreach (cExpenseItem splititem in lstSplititems)
                {
                    grandtotal += splititem.grandvattotal;
                }
                return grandtotal;
            }
            protected set { ; }
        }

        [DataMember]
        public decimal grandamountpayabletotal
        {
            get
            {
                decimal grandtotal = amountpayable;
                foreach (cExpenseItem splititem in lstSplititems)
                {
                    grandtotal += splititem.grandamountpayabletotal;
                }
                return grandtotal;
            }
            protected set { ; }
        }
        [DataMember]
        public int othergrandtotal
        {
            get
            {
                int grandtotal = others;
                foreach (cExpenseItem splititem in lstSplititems)
                {
                    grandtotal += splititem.othergrandtotal;
                }
                return grandtotal;
            }
            protected set { ; }
        }
        [DataMember]
        public int remoteworkersgrandtotal
        {
            get
            {
                int grandtotal = remoteworkers;
                foreach (cExpenseItem splititem in lstSplititems)
                {
                    grandtotal += splititem.remoteworkersgrandtotal;
                }
                return grandtotal;
            }
            protected set { ; }
        }
        [DataMember]
        public int personalguestsgrandtotal
        {
            get
            {
                int grandtotal = personalguests;
                foreach (cExpenseItem splititem in lstSplititems)
                {
                    grandtotal += splititem.personalguestsgrandtotal;
                }
                return grandtotal;
            }
            protected set { ; }
        }
        
        public DateTime createdon
        {
            get { return dtCreatedon; }
            set { dtCreatedon = value; }
        }
        
        public int createdby
        {
            get { return nCreatedby; }
            protected set { ; }
        }
        
        public DateTime modifiedon
        {
            get { return dtModifiedon; }
            set { dtModifiedon = value; }
        }
        
        public int modifiedby
        {
            get { return nModifiedby; }
            protected set { ; }
        }
        
        public int mileageid
        {
            get { return nMileageid; }
            set { nMileageid = value; }
        }
        [DataMember]
        public SortedList<int, cJourneyStep> journeysteps
        {
            get { return lstJourneySteps; }
            set { lstJourneySteps = value; }
        }
        [DataMember]
        public MileageUOM journeyunit
        {
            get { return eJourneyUnit; }
            protected set { ; }
        }
        
        /// <summary>
        /// esrAssignID
        /// </summary>
        public long ESRAssignmentId
        {
            get { return nESRAssignmentId; }
            protected set { ; }
        }
        
        public HomeToLocationType homeToOfficeDeductionMethod
        {
            get { return eHomeToOfficeDeductionMethod; }
            set { eHomeToOfficeDeductionMethod = value; }
        }

        public bool addedAsMobileExpense
        {
            get { return bIsMobileItem; }
            set { bIsMobileItem = value; }
        }

        public int addedByMobileDeviceTypeId
        {
            get { return nMobileDeviceTypeId; }
            set { nMobileDeviceTypeId = value; }
        }

        public int? itemCheckerId
        {
            get
            {
                return nItemCheckerId;
            }
            set
            {
                nItemCheckerId = value;
            }
        }

        public int? ItemCheckerTeamId { get; set; }

        public int? bankAccountId
        {
            get { return nBankAccountId; }
            set { nBankAccountId = value; }
        }

        /// <summary>
        /// The work address ID chosen from a list of work addresses (if available) when claiming mileage
        /// </summary>
        public int WorkAddressId { get; set; }

        /// <summary>
        /// Expenses item is escalated
        /// </summary>
        public bool? IsItemEscalated
        {
            get; 
            set;
        }

        /// <summary>
        /// The current progress of this item for receipt validation.
        /// </summary>
        public virtual ExpenseValidationProgress ValidationProgress { get; private set; }

        /// <summary>
        /// Expedite Operators Validation Progress Status
        /// </summary>
        public virtual ExpediteOperatorValidationProgress  OperatorValidationProgress{ get; private set; }

        
        /// <summary>
        /// The results of the most recent validation.
        /// </summary>
        public virtual List<ExpenseValidationResult> ValidationResults { get; set; }

        /// <summary>
        /// Gets the highest flag level from all flags on the records.
        /// </summary>
        public FlagColour HighestFlagLevel
        {
            get
            {
                if (this.lstFlags == null || lstFlags.Count == 0)
                {
                    return FlagColour.None;
                }
                
                FlagColour colour = FlagColour.None;
                foreach (FlagSummary flag in this.lstFlags.Where(flag => (byte)flag.FlaggedItem.FlagColour > (byte)colour))
                {
                    colour = flag.FlaggedItem.FlagColour;
                    if (colour == FlagColour.Red)
                    {
                        break;
                    }
                }

                return colour;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the expense item has been edited.
        /// </summary>
        public bool Edited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the expense item is paid.
        /// </summary>
        public bool Paid { get; set; }

        /// <summary>
        /// Gets or sets the original expense id if this is a copied item from a previously paid then edited expense item.
        /// </summary>
        public int? OriginalExpenseId { get; set; }

        /// <summary>
        /// Gets or sets the mobile metric device id, which is the internal identifier for the device.
        /// </summary>
        public int? MobileMetricDeviceId { get; set; }


        #endregion

        /// <summary>
        /// Updates the validation progress and results of this expense item, in case it is out of sync with the Validate property of the subcat.
        /// Also will populate the results if the account and subcat support it and the Progress is at the correct stage.
        /// </summary>
        /// <param name="account">The account under which to check the validation service.</param>
        /// <param name="subCatSetToValidate">The value of this item's Subcat.Validate property.</param>
        /// <param name="validateStageInSignoff">Whether the signoff group for this expense item contains the validate stage.</param>
        /// <param name="validationManager">The validation manager required to populate the results.</param>
        public virtual ExpenseValidationProgress DetermineValidationProgress(cAccount account, bool subCatSetToValidate, bool validateStageInSignoff, IManageExpenseValidation validationManager)
        {
            var originalProgress = ValidationProgress;

            // first handle if the validation service for the account has been disabled or never was enabled
            if (!account.ValidationServiceEnabled)
            {
                // if progress was complete, populate the items for history's sake.
                if (ValidationProgress > ExpenseValidationProgress.InProgress)
                {
                    ValidationResults = validationManager.GetResultsForExpenseItem(expenseid).ToList();
                }
                else
                {
                    // make sure the validation service is disabled
                    ValidationProgress = ExpenseValidationProgress.ValidationServiceDisabled;
                }

                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            // ditch out if subcat validation is disabled
            if (!subCatSetToValidate)
            {
                ValidationProgress = ExpenseValidationProgress.SubcatValidationDisabled;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            // ditch out if signoff group validation is disabled
            if (!validateStageInSignoff)
            {
                ValidationProgress = ExpenseValidationProgress.StageNotInSignoffGroup;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            // check here that the item has any receipts
            if (!receiptattached)
            {
                ValidationProgress = ExpenseValidationProgress.NoReceipts;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            // get the criteria for this item
            var criteriaForThisItem = validationManager.GetAllSubcatCriteria(subcatid);

            if (criteriaForThisItem.Count == 0)
            {
                ValidationProgress = ExpenseValidationProgress.NotRequired;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            // get the results for this item
            if (ValidationProgress > ExpenseValidationProgress.NoReceipts && ValidationResults.Count == 0)
            {
                ValidationResults = validationManager.GetResultsForExpenseItem(expenseid).ToList();
            }
            
            // do the check for invalidity
            if (ValidationProgress > ExpenseValidationProgress.InProgress)
            {
                var validityCheckValue = DetermineValidationProgressValidity();

                if (originalProgress != validityCheckValue)
                {
                    ValidationProgress = validityCheckValue;
                    return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
                }
            };

            // return here if we've already invalidated
            if (ValidationProgress > ExpenseValidationProgress.CompletedPassed)
            {
                return ValidationProgress;
            }

            // if there are none then validation is required
            if (ValidationResults.Count == 0)
            {
                ValidationProgress = ExpenseValidationProgress.Required;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            // if there are results, but less than the number of criteria, then it's in progress
            if (ValidationResults.Count < criteriaForThisItem.Count)
            {
                ValidationProgress = ExpenseValidationProgress.InProgress;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            var anyFailedForBusiness = ValidationResults.Any(r => r.BusinessStatus == ExpenseValidationResultStatus.Fail);

            // check if we need to return the item due to a business fail
            if (anyFailedForBusiness && !corrected && ValidationCount < 2)
            {
                ValidationProgress = ExpenseValidationProgress.WaitingForClaimant;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            // if we got this far, validation is complete for this expense item, 
            // so determine the resulting status based on the list.
            if (anyFailedForBusiness)
            {
                ValidationProgress = ExpenseValidationProgress.CompletedFailed;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }
            
            if (ValidationResults.Any(r => r.VATStatus == ExpenseValidationResultStatus.Fail))
            {
                ValidationProgress = ExpenseValidationProgress.CompletedWarning;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            if (ValidationResults.All(r => r.BusinessStatus == ExpenseValidationResultStatus.NotApplicable && r.VATStatus == ExpenseValidationResultStatus.NotApplicable))
            {
                ValidationProgress = ExpenseValidationProgress.CompletedWarning;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }

            // if we got this far all criteria passed.
            ValidationProgress = ExpenseValidationProgress.CompletedPassed;
            return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
        }

        /// <summary>
        /// Determines whether or not this item has been returned and subsequently edited.
        /// If so, and the validation was completed, then the Progress is updated to reflected this.
        /// </summary>
        /// <param name="forceInvalidation">Whether to force the item's validation is invalid.</param>
        /// <returns>The new Progress.</returns>
        public virtual ExpenseValidationProgress DetermineValidationProgressValidity(bool forceInvalidation = false)
        {
            // check here in case everything has been done and this claim line has been returned.
            if (ValidationProgress > ExpenseValidationProgress.InProgress && (corrected || forceInvalidation))
            {
                switch (ValidationProgress)
                {
                    case ExpenseValidationProgress.CompletedFailed:
                        ValidationProgress = ExpenseValidationProgress.InvalidatedFailed;
                        break;
                    case ExpenseValidationProgress.CompletedWarning:
                        ValidationProgress = ExpenseValidationProgress.InvalidatedWarning;
                        break;
                    case ExpenseValidationProgress.CompletedPassed:
                        ValidationProgress = ExpenseValidationProgress.InvalidatedPassed;
                        break;
                }
            }

            return ValidationProgress;
        }

        /// <summary>
        /// Resets the validation for this item, if it can be reset (the ValidationCount is low enough to allow revalidation).
        /// Deletes the existing results, and sets progress to Required. If not allowed, nothing changes and the original Progress gets returned.
        /// </summary>
        /// <param name="validationManager">A validation manager.</param>
        /// <param name="isCurrentlyInValidation">Whether the claim is currently in validation.</param>
        /// <returns>Either Required, if the operation was a success, or the original Progress.</returns>
        public virtual ExpenseValidationProgress ResetValidation(IManageExpenseValidation validationManager, bool isCurrentlyInValidation)
        {
            var originalProgress = ValidationProgress;

            if (!isCurrentlyInValidation)
            {
                return ValidationProgress;
            }

            if (ValidationCount < 2 && corrected)
            {
                ValidationProgress = ExpenseValidationProgress.Required;
                return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
            }
                
            ValidationProgress = DetermineValidationProgressValidity(true);
            return validationManager.UpdateProgressForExpenseItem(expenseid, originalProgress, ValidationProgress);
        }

        public cExpenseItem entertainmentsplititem()
        {
            foreach (cExpenseItem item in lstSplititems)
            {
                if (item.others > 0)
                {
                    return item;
                }
            }

            return null;
        }
        public cExpenseItem personalguestssplititem()
        {
            foreach (cExpenseItem item in lstSplititems)
            {
                if (item.personalguests > 0)
                {
                    return item;
                }
            }

            return null;
        }
        public cExpenseItem remoteworkerssplititem()
        {
            foreach (cExpenseItem item in lstSplititems)
            {
                if (item.remoteworkers > 0)
                {
                    return item;
                }
            }

            return null;
        }
        public void setPrimaryItem(cExpenseItem item)
        {
            clsPrimaryItem = item;
        }
        public void addSplitItem(cExpenseItem item)
        {
            lstSplititems.Add(item);
        }
        public cExpenseItem getSplitItemBySubcat(int subcatid)
        {
            foreach (cExpenseItem splititem in lstSplititems)
            {
                if (splititem.subcatid == subcatid)
                {
                    return splititem;
                }
            }
            return null;
        }
        public void setGlobalTotal(int currencyid, double exchangerate, decimal total)
        {
            nGlobalBasecurrency = currencyid;
            dGlobalExchangerate = exchangerate;
            dGlobalTotal = total;
        }

        public void updateVAT(decimal net, decimal vat, decimal total)
        {
            dNet = net;
            dVat = vat;
            dTotal = total;

        }

        public void updateVAT(decimal net, decimal vat, decimal total, decimal foreignvat)
        {
            dNet = net;
            dVat = vat;
            dTotal = total;

            dForeignvat = foreignvat;
        }

        public void addJourneyStep()
        {
            Address startlocation = null;
            if (lstJourneySteps.Count > 0)
            {
                if (lstJourneySteps.Values[lstJourneySteps.Count - 1].endlocation != null)
                {
                    startlocation = lstJourneySteps.Values[lstJourneySteps.Count - 1].endlocation;
                }
            }
            lstJourneySteps.Add(lstJourneySteps.Count, new cJourneyStep(expenseid, startlocation, null, 0, 0, 0, (byte)(lstJourneySteps.Count), 0, false));
        }

        public void removeJourneyStep(int index)
        {
            lstJourneySteps.RemoveAt(index);
        }

        public int? mobileID { get; set; }

        /// <summary>
        /// Copy an instance of <see cref="cExpenseItem"/> setting any ID's to zero.
        /// </summary>
        /// <returns>a new instance of <see cref="cExpenseItem"/></returns>
        public cExpenseItem Clone()
        {
            var expenseItem =  new cExpenseItem(0, ItemType.Cash, this.bmiles, this.pmiles, this.reason, this.receipt, this.net, this.vat, this.total, this.subcatid, this.date, this.staff, this.others, this.companyid, this.returned, this.home, this.refnum, this.claimid, this.plitres, this.blitres, this.currencyid, this.attendees, this.tip, this.countryid, this.foreignvat, this.convertedtotal, this.exchangerate, this.tempallow, this.reasonid, this.normalreceipt, this.allowancestartdate, this.allowanceenddate, this.carid, this.allowancededuct, this.allowanceid, this.nonights, this.quantity, this.directors, this.amountpayable, this.hotelid, this.primaryitem,this.norooms, this.vatnumber, this.personalguests, this.remoteworkers, this.accountcode, this.basecurrency, this.globalbasecurrency, this.globalexchangerate, this.globaltotal, 0, this.corrected, false, 0, DateTime.Now, this.createdby, this.modifiedon, this.modifiedby, this.mileageid, this.journeyunit, this.ESRAssignmentId, this.homeToOfficeDeductionMethod, bankAccountId:this.bankAccountId.HasValue? this.bankAccountId.Value : 0, workAddressId: this.WorkAddressId);
            foreach (cExpenseItem splititem in this.lstSplititems)
            {
                expenseItem.splititems.Add(splititem.Clone());
            }

            return expenseItem;
        }
    }

    [Serializable()]
    public class cDepCostItem
    {
        private int nDepartmentid;
        private int nCostcodeid;
        private int nProjectcodeid;
        private int nPercentused;

        public cDepCostItem()
        {
        }
        public cDepCostItem(int departmentid, int costcodeid, int projectcodeid, int percentused)
        {
            nDepartmentid = departmentid;
            nCostcodeid = costcodeid;
            nProjectcodeid = projectcodeid;
            nPercentused = percentused;
        }

        public int departmentid
        {
            get { return nDepartmentid; }
            set { nDepartmentid = value; }
        }
        public int costcodeid
        {
            get { return nCostcodeid; }
            set { nCostcodeid = value; }
        }
        public int percentused
        {
            get { return nPercentused; }
            set { nPercentused = value; }
        }
        public int projectcodeid
        {
            get { return nProjectcodeid; }
            set { nProjectcodeid = value; }
        }


    }

    [Serializable()]
    public struct sOnlineItemInfo
    {
        public Dictionary<int, cExpenseItem> lstCurModItems;
        public List<int> lstCurItemIds;
        public Dictionary<int, cExpenseItem> lstPrevModItems;
        public List<int> lstPrevItemIds;
    }

    [Serializable]
    public struct sReceiptFileInfo
    {
        public int expid;
        public string filename;
        public byte[] recFile;
    }

    [Serializable()]
    public struct sSplititemInfo
    {
        public int primaryexpid;
        public int splitexpid;
    }

    public enum ItemType
    {
        Cash = 1,
        CreditCard = 2,
        PurchaseCard = 3
    }





    public enum ItemState
    {
        Unapproved = 1,
        Returned = -1,
        Approved = 2
    }

    public enum Filter
    {
        None = 0,
        Cash = 1,
        CreditCard = 2,
        PurchaseCard = 3
    }
}
