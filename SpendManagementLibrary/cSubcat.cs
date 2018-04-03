using System;
using System.Collections.Generic;
using System.Text;
using SpendManagementLibrary.Expedite;
using SpendManagementLibrary.JourneyDeductionRules;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cSubcat
    {
        public int nSubcatid;
        public int nCategoryid;
        public string sSubcat;
        public string sDescription;

        public bool bMileageapp;
        public bool bStaffapp;
        public bool bOthersapp;
        public bool bTipapp;
        public bool bPmilesapp;
        public bool bBmilesapp;
        public decimal dAllowanceamount;
        public string sAccountcode;
        public bool bAttendeesapp;
        public bool bAddasnet;
        public int nPdcatid;

        public bool bEventinhomeapp;
        public bool bReceiptapp;
        public CalculationType ctCalculation;

        private bool bPassengersapp;
        private bool bNopassengersapp;
        private bool bPassengerNamesapp;
        private string sComment;
        private List<int> lstAllowances;
        private List<cCountrySubcat> lstcountries;
        private bool bSplitentertainment;
        private int nEntertainmentid;
        private bool bReimbursable;
        private bool bNonightsapp;
        private bool bAttendeesmand;
        private bool bNodirectorsapp;
        private bool bHotelapp;

        private List<int> lstSplitItems;
        private bool bNoroomsapp;
        private bool bHotelmand;
        private bool bVatnumberapp;
        private bool bVatnumbermand;
        private bool bNopersonalguestsapp;
        private bool bNoremoteworkersapp;
        private string sAlternateAccountcode;
        private bool bSplitpersonal;
        private bool bSplitremote;
        private int nPersonalid;
        private int nRemoteid;

        private bool bReasonapp;
        private bool bOtherdetailsapp;
        private SortedList<int, object> lstUserdefined;
        private List<int> lstAssociatedUDFs;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime? dtModifiedon;
        private int? nModifiedby;

        private string sShortSubcat;
        private bool bFromapp;
        private bool bToapp;
        private bool bCompanyapp;
        private List<cSubcatVatRate> lstVatRates;
        private bool bEnableHomeToLocationMileage;
        private bool bHomeToOfficeAlwaysZero;
        /// <summary>
        /// Enforce cap on mileage for Home to office/office to home journey step
        /// </summary>
        private bool enforceHomeToOfficeMileageCap;
        private HomeToLocationType eHomeToLocationType;
        private int? nMileageCategory;
        private bool bIsRelocationMileage;
        private int? nReimbursableSubcatID;
        private bool bAllowHeavyBulkyMileage;
        private bool enableDoC;
        private bool requireClass1BusinessInsurance;

        public cSubcat()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="cSubcat"/> class.
        /// </summary>
        /// <param name="subcatid">
        /// The subcat id.
        /// </param>
        /// <param name="categoryid">
        /// The category id.
        /// </param>
        /// <param name="subcat">
        /// The subcat.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="mileageapp">
        /// The mileage app.
        /// </param>
        /// <param name="staffapp">
        /// The staff app.
        /// </param>
        /// <param name="othersapp">
        /// The others app.
        /// </param>
        /// <param name="tipapp">
        /// The tip app.
        /// </param>
        /// <param name="pmilesapp">
        /// The pmiles app.
        /// </param>
        /// <param name="bmilesapp">
        /// The bmiles app.
        /// </param>
        /// <param name="allowanceamount">
        /// The allowance amount.
        /// </param>
        /// <param name="accountcode">
        /// The account code.
        /// </param>
        /// <param name="attendeesapp">
        /// The attendees app.
        /// </param>
        /// <param name="addasnet">
        /// The add as net.
        /// </param>
        /// <param name="pdcatid">
        /// The pdcat id.
        /// </param>
        /// <param name="eventinhomeapp">
        /// The event in home app.
        /// </param>
        /// <param name="receiptapp">
        /// The receip tapp.
        /// </param>
        /// <param name="calculation">
        /// The calculation.
        /// </param>
        /// <param name="passengersapp">
        /// The passenger sapp.
        /// </param>
        /// <param name="nopassengersapp">
        /// The no passenger sapp.
        /// </param>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <param name="splitentertainment">
        /// The split entertainment.
        /// </param>
        /// <param name="entertainmentid">
        /// The entertainment id.
        /// </param>
        /// <param name="reimbursable">
        /// The reimbursable.
        /// </param>
        /// <param name="nonightsapp">
        /// The no nights app.
        /// </param>
        /// <param name="attendeesmand">
        /// The attendees mandatory.
        /// </param>
        /// <param name="nodirectorsapp">
        /// The no directors app.
        /// </param>
        /// <param name="hotelapp">
        /// The hotel app.
        /// </param>
        /// <param name="noroomsapp">
        /// The no rooms app.
        /// </param>
        /// <param name="hotelmand">
        /// The hotel mandatory.
        /// </param>
        /// <param name="vatnumberapp">
        /// The vat number app.
        /// </param>
        /// <param name="vatnumbermand">
        /// The vat number mandatory.
        /// </param>
        /// <param name="nopersonalguestsapp">
        /// The no personal guests app.
        /// </param>
        /// <param name="noremoteworkersapp">
        /// The no remote worker sapp.
        /// </param>
        /// <param name="alternateaccountcode">
        /// The alternate account code.
        /// </param>
        /// <param name="splitpersonal">
        /// The split personal.
        /// </param>
        /// <param name="splitremote">
        /// The split remote.
        /// </param>
        /// <param name="personalid">
        /// The personal id.
        /// </param>
        /// <param name="remoteid">
        /// The remote id.
        /// </param>
        /// <param name="reasonapp">
        /// The reason app.
        /// </param>
        /// <param name="otherdetailsapp">
        /// The other details app.
        /// </param>
        /// <param name="userdefined">
        /// The userdefined.
        /// </param>
        /// <param name="createdon">
        /// The created on.
        /// </param>
        /// <param name="createdby">
        /// The created by.
        /// </param>
        /// <param name="modifiedon">
        /// The modified on.
        /// </param>
        /// <param name="modifiedby">
        /// The modified by.
        /// </param>
        /// <param name="shortsubcat">
        /// The short subcat.
        /// </param>
        /// <param name="fromapp">
        /// The from app.
        /// </param>
        /// <param name="toapp">
        /// The toapp.
        /// </param>
        /// <param name="countries">
        /// The countries.
        /// </param>
        /// <param name="allowances">
        /// The allowances.
        /// </param>
        /// <param name="associatedudfs">
        /// The associated udfs.
        /// </param>
        /// <param name="split">
        /// The split.
        /// </param>
        /// <param name="companyapp">
        /// The company app.
        /// </param>
        /// <param name="vatrates">
        /// The vat rates.
        /// </param>
        /// <param name="enablehometolocationmileage">
        /// The enable home to location mileage.
        /// </param>
        /// <param name="hometolocationtype">
        /// The home to location type.
        /// </param>
        /// <param name="mileagecategory">
        /// The mileage category.
        /// </param>
        /// <param name="isRelocationMileage">
        /// The is relocation mileage.
        /// </param>
        /// <param name="reimbursableSubcatID">
        /// The reimbursable subcat id.
        /// </param>
        /// <param name="allowHeavyBulkyMileage">
        /// The allow heavy bulky mileage.
        /// </param>
        /// <param name="homeToOfficeAsZero">
        /// The home to office as zero.
        /// </param>
        /// <param name="homeToOfficeFixedMiles"></param>
        /// <param name="publicTransportRate">
        /// The public transport rate applied to the deduction rule
        /// </param>
        /// <param name="startDate">
        /// The date from which the expense item can be claimed.
        /// </param>
        /// <param name="endDate">
        /// The last date at which the expense item can be claimed.
        /// </param>
        /// <param name="validate">Whether expense items templated from this subcat should be validated, if the account has ValidationServiceEnabled.</param>
        /// <param name="validationCriteria">The notes for validators, which are built from a criterion in the metabase, if the account has ValidationServiceEnabled.</param>
        /// <param name="enableDoC">Enable Duty of care check at expense item level, for Mileage expense item type.</param>
        /// <param name="requireClass1BusinessInsurance">Enable the Class 1 Business Insurance check for the expense items.</param>
        /// <param name="enforceMileageCap">Option to enable cap on number of miles for journey between Home and Office.</param>
        /// <param name="homeToOfficeMileageCap">Mumber of miles set for Home to office/office to home journey if the option to enforce the mileage cap is enabled.</param>
        public cSubcat(int subcatid, int categoryid, string subcat, string description, bool mileageapp, bool staffapp, bool othersapp, bool tipapp, bool pmilesapp, bool bmilesapp, decimal allowanceamount, string accountcode, bool attendeesapp, bool addasnet, int pdcatid, bool eventinhomeapp, bool receiptapp, CalculationType calculation, bool passengersapp, bool nopassengersapp, bool passengernamesapp, string comment, bool splitentertainment, int entertainmentid, bool reimbursable, bool nonightsapp, bool attendeesmand, bool nodirectorsapp, bool hotelapp, bool noroomsapp, bool hotelmand, bool vatnumberapp, bool vatnumbermand, bool nopersonalguestsapp, bool noremoteworkersapp, string alternateaccountcode, bool splitpersonal, bool splitremote, int personalid, int remoteid, bool reasonapp, bool otherdetailsapp, SortedList<int, object> userdefined, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, string shortsubcat, bool fromapp, bool toapp, List<cCountrySubcat> countries, List<int> allowances, List<int> associatedudfs, List<int> split, bool companyapp, List<cSubcatVatRate> vatrates, bool enablehometolocationmileage, HomeToLocationType hometolocationtype, int? mileagecategory, bool isRelocationMileage, int? reimbursableSubcatID, bool allowHeavyBulkyMileage, bool homeToOfficeAsZero, float? homeToOfficeFixedMiles, int? publicTransportRate, DateTime? startDate = null, DateTime? endDate = null, bool validate = false, List<ExpenseValidationCriterion> validationCriteria = null,bool enableDoC=false,bool requireClass1BusinessInsurance=false,bool enforceMileageCap = false, float? homeToOfficeMileageCap=null)
        {
            nSubcatid = subcatid;
            nCategoryid = categoryid;
            sSubcat = subcat;
            sDescription = description;
            bMileageapp = mileageapp;
            bStaffapp = staffapp;
            bOthersapp = othersapp;
            bTipapp = tipapp;
            bPmilesapp = pmilesapp;
            bBmilesapp = bmilesapp;
            dAllowanceamount = allowanceamount;
            sAccountcode = accountcode;
            bAttendeesapp = attendeesapp;
            bAddasnet = addasnet;
            nPdcatid = pdcatid;
            bEventinhomeapp = eventinhomeapp;
            bReceiptapp = receiptapp;
            ctCalculation = calculation;

            bPassengersapp = passengersapp;
            bNopassengersapp = nopassengersapp;
            bPassengerNamesapp = passengernamesapp;
            sComment = comment;
            bSplitentertainment = splitentertainment;
            nEntertainmentid = entertainmentid;
            bReimbursable = reimbursable;
            bNonightsapp = nonightsapp;
            bAttendeesmand = attendeesmand;
            bNodirectorsapp = nodirectorsapp;
            bHotelapp = hotelapp;
            bNoroomsapp = noroomsapp;

            lstcountries = countries;
            lstAllowances = allowances;

            bHotelmand = hotelmand;
            bVatnumberapp = vatnumberapp;
            bVatnumbermand = vatnumbermand;
            bNopersonalguestsapp = nopersonalguestsapp;
            bNoremoteworkersapp = noremoteworkersapp;
            sAlternateAccountcode = alternateaccountcode;
            bSplitpersonal = splitpersonal;
            bSplitremote = splitremote;
            nPersonalid = personalid;
            nRemoteid = remoteid;

            lstSplitItems = split;
            bReasonapp = reasonapp;
            bOtherdetailsapp = otherdetailsapp;
            lstUserdefined = userdefined;
            lstAssociatedUDFs = associatedudfs;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
            sShortSubcat = shortsubcat;
            bFromapp = fromapp;
            bToapp = toapp;
            bCompanyapp = companyapp;
            lstVatRates = vatrates;
            bEnableHomeToLocationMileage = enablehometolocationmileage;
            eHomeToLocationType = hometolocationtype;
            nMileageCategory = mileagecategory;
            bIsRelocationMileage = isRelocationMileage;
            nReimbursableSubcatID = reimbursableSubcatID;
            bAllowHeavyBulkyMileage = allowHeavyBulkyMileage;
            bHomeToOfficeAlwaysZero = homeToOfficeAsZero;
            this.HomeToOfficeFixedMiles = homeToOfficeFixedMiles;
            this.PublicTransportRate = publicTransportRate;
            this.enforceHomeToOfficeMileageCap = enforceMileageCap;
            this.HomeToOfficeMileageCap = homeToOfficeMileageCap;
            this.StartDate = startDate;
            this.EndDate = endDate;

            Validate = validate;
            ValidationRequirements = validationCriteria;
            EnableDoC= enableDoC;
            RequireClass1BusinessInsurance = requireClass1BusinessInsurance;
        }

        /// <summary>
        /// The get mileage text for the mileage type.
        /// <param name="subcat">The sub cat for this item.</param>
        /// <param name="hometooffice">
        /// The home to office distance.
        /// </param>
        /// <param name="uom">
        /// The Unit of Measurement.
        /// </param>
        /// <param name="pluralUomHtO">
        /// The plural unit of measurement home to office.
        /// </param>
        /// <param name="officetohome">
        /// The office to home distance.
        /// </param>
        /// <param name="pluralUomOtH">
        /// The plural unit of measurement office to home.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns></summary>
        public static string GetMileageText(cSubcat subcat, decimal hometooffice, string uom, string pluralUomHtO, decimal officetohome, string pluralUomOtH)
        {
            string comment;
            string mileageCapcomment = null;
            // TODO this could be refactored into a TypeFactory, after the first two rules have been refactored
            if (subcat.EnforceToOfficeMileageCap && subcat.HomeToOfficeMileageCap.HasValue)
            {
                double mileageCap = subcat.HomeToOfficeMileageCap.Value;
                var distanceUom = mileageCap > 1 ? "Miles" : "Mile";
                mileageCapcomment = $"<br /><br />A reimbursable limit of {mileageCap} {distanceUom} is enforced on journeys between your Home and Office.";
            }

            switch (subcat.HomeToLocationType)
            {
                case HomeToLocationType.CalculateHomeAndOfficeToLocationDiff:
                    comment = "<br /><br />If you start your journey from home and the distance to your location is greater than if you start your journey from the office, then the 'Amount Payable' total will deduct the difference between home to your location and office to your address.";
                    break;
                case HomeToLocationType.FlagHomeAndOfficeToLocationDiff:
                    comment = "<br /><br />If you start your journey from home and the distance to your location is greater than if you start your journey from the office, then the 'Amount Payable' total will flag the difference between home to your location and office to your address.";
                    break;
                case HomeToLocationType.DeductHomeToOfficeFromEveryJourney:
                    comment = HomeToOfficeOnce.GetMessage(hometooffice, officetohome, uom, pluralUomHtO, pluralUomOtH);
                    break;
                case HomeToLocationType.DeductHomeToOfficeEveryTimeHomeIsVisited:
                    comment = HomeToOfficeForEveryHomeVisit.GetMessage(hometooffice, officetohome, uom, pluralUomHtO, pluralUomOtH);
                    break;
                case HomeToLocationType.DeductHomeToOfficeIfStartOrFinishHome:
                    comment = HomeToOfficeIfStartOrFinishHome.GetMessage(hometooffice, officetohome, uom, pluralUomHtO, pluralUomOtH);
                    break;
                case HomeToLocationType.DeductFirstAndLastHome:
                    comment = HomeToOfficeForFirstAndLastHome.GetMessage(hometooffice, officetohome, uom, pluralUomHtO, pluralUomOtH);
                    break;
                case HomeToLocationType.DeductFullHomeToOfficeEveryTimeHomeIsVisited:
                    comment = FullHomeToOfficeForEveryHomeVisit.GetMessage(hometooffice, officetohome, uom, pluralUomHtO, pluralUomOtH);
                    break;
                case HomeToLocationType.DeductFullHomeToOfficeIfStartOrFinishHome:
                    comment = FullHomeToOfficeIfStartOrFinishHome.GetMessage(hometooffice, officetohome, uom, pluralUomHtO, pluralUomOtH);
                    break;
                case HomeToLocationType.DeductFixedMilesIfStartOrFinishHome:
                    double fixedMiles = 0;
                    if (subcat.HomeToOfficeFixedMiles.HasValue)
                    {
                        fixedMiles = subcat.HomeToOfficeFixedMiles.Value;
                    }

                    comment = FixedDeductionIfStartOrFinishHome.GetMessage((decimal) fixedMiles, (decimal) fixedMiles, uom, pluralUomHtO, pluralUomOtH);
                    break;
                case HomeToLocationType.JuniorDoctorRotation:
                    comment = JuniorDoctorRotation.GetMessage(hometooffice, officetohome, uom, pluralUomHtO, pluralUomOtH);
                    break;
                default:
                    comment = string.Empty;
                    break;
            }
            if (subcat.HomeToOfficeAlwaysZero)
            {
                comment += "<br /><br />Mileage claims between home and office will have mileage enforced to zero.";
            }
            return string.Format("{0}{1}", mileageCapcomment, comment);
        }
        
        #region properties
        public int subcatid
        {
            get { return nSubcatid; }
        }
        public int categoryid
        {
            get { return nCategoryid; }
        }
        public string subcat
        {
            get { return sSubcat; }
        }
        public string description
        {
            get { return sDescription; }
        }
        public bool mileageapp
        {
            get { return bMileageapp; }
        }
        public bool staffapp
        {
            get { return bStaffapp; }
        }
        public bool othersapp
        {
            get { return bOthersapp; }
        }
        public bool tipapp
        {
            get { return bTipapp; }
        }
        public bool vatapp
        {
            get
            {
                if (lstVatRates.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool vatreceipt
        {
            get
            {
                foreach (cSubcatVatRate rate in lstVatRates)
                {
                    if (rate.vatreceipt)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool pmilesapp
        {
            get { return bPmilesapp; }
        }
        public bool bmilesapp
        {
            get { return bBmilesapp; }
        }

        public decimal allowanceamount
        {
            get { return dAllowanceamount; }
        }
        public string accountcode
        {
            get { return sAccountcode; }
        }
        public bool attendeesapp
        {
            get { return bAttendeesapp; }
        }
        public bool addasnet
        {
            get { return bAddasnet; }
        }
        public int pdcatid
        {
            get { return nPdcatid; }
        }

        public bool eventinhomeapp
        {
            get { return bEventinhomeapp; }
        }
        public bool receiptapp
        {
            get { return bReceiptapp; }
        }
        public CalculationType calculation
        {
            get { return ctCalculation; }
            internal set
            {
                this.ctCalculation = value;
            }
        }

        public bool passengersapp
        {
            get { return bPassengersapp; }
            internal set
            {
                this.bPassengersapp = value;
            }
        }
        public bool nopassengersapp
        {
            get { return bNopassengersapp; }
        }
        public bool passengernamesapp
        {
            get { return bPassengerNamesapp; }
        }
        public string comment
        {
            get { return sComment; }
        }
        public bool splitentertainment
        {
            get { return bSplitentertainment; }
        }
        public int entertainmentid
        {
            get { return nEntertainmentid; }
        }
        public bool reimbursable
        {
            get { return bReimbursable; }
        }
        public bool nonightsapp
        {
            get { return bNonightsapp; }
        }
        public bool attendeesmand
        {
            get { return bAttendeesmand; }
        }
        public List<int> allowances
        {
            get { return lstAllowances; }
        }
        public bool nodirectorsapp
        {
            get { return bNodirectorsapp; }
        }
        public bool hotelapp
        {
            get { return bHotelapp; }
        }
        public List<int> subcatsplit
        {
            get { return lstSplitItems; }
        }
        public bool noroomsapp
        {
            get { return bNoroomsapp; }
        }
        public bool hotelmand
        {
            get { return bHotelmand; }
        }
        public bool vatnumberapp
        {
            get { return bVatnumberapp; }
        }
        public bool vatnumbermand
        {
            get { return bVatnumbermand; }
        }
        public bool nopersonalguestsapp
        {
            get { return bNopersonalguestsapp; }
        }
        public bool noremoteworkersapp
        {
            get { return bNoremoteworkersapp; }
        }
        public string alternateaccountcode
        {
            get { return sAlternateAccountcode; }
        }
        public bool splitpersonal
        {
            get { return bSplitpersonal; }
        }
        public bool splitremote
        {
            get { return bSplitremote; }
        }
        public int personalid
        {
            get { return nPersonalid; }
        }
        public int remoteid
        {
            get { return nRemoteid; }
        }

        public bool reasonapp
        {
            get { return bReasonapp; }
        }
        public bool otherdetailsapp
        {
            get { return bOtherdetailsapp; }
        }
        public SortedList<int, object> userdefined
        {
            get { return lstUserdefined; }
        }
        public List<int> associatedudfs
        {
            get { return lstAssociatedUDFs; }
        }
        public DateTime createdon
        {
            get { return dtCreatedon; }
        }
        public int createdby
        {
            get { return nCreatedby; }
        }
        public DateTime? modifiedon
        {
            get { return dtModifiedon; }
        }
        public int? modifiedby
        {
            get { return nModifiedby; }
        }
        public string shortsubcat
        {
            get { return sShortSubcat; }
        }
        public bool fromapp
        {
            get { return bFromapp; }
        }
        public bool toapp
        {
            get { return bToapp; }
        }
        public List<cCountrySubcat> countries
        {
            get { return lstcountries; }
        }
        public bool companyapp
        {
            get { return bCompanyapp; }
        }
        /// <summary>
        /// Gets whether the home to location mileage calculation is enabled for this expense item
        /// </summary>
        public bool EnableHomeToLocationMileage
        {
            get { return bEnableHomeToLocationMileage; }
        }
        /// <summary>
        /// If EnableHomeToLocationMileage is true, gets whether commuter mileage is automatically deducted or excess mileage flagged for the user
        /// </summary>
        public HomeToLocationType HomeToLocationType
        {
            get { return eHomeToLocationType; }
        }
        /// <summary>
        /// If set, overrides the mileage category on a car record and calculates mileage at this rate
        /// </summary>
        public int? MileageCategory
        {
            get { return nMileageCategory; }
        }
        /// <summary>
        /// Gets whether relocation mileage is enabled. If so, mileage is automatically calculated from previous to new place of work
        /// </summary>
        public bool IsRelocationMileage
        {
            get { return bIsRelocationMileage; }
        }
        public int? reimbursableSubcatID
        {
            get { return nReimbursableSubcatID; }
        }

        /// <summary>
        /// Flag to allow the claiming of heavy bulky equipment on a mileage expense item
        /// </summary>
        public bool allowHeavyBulkyMileage
        {
            get { return bAllowHeavyBulkyMileage; }
            internal set
            {
                this.bAllowHeavyBulkyMileage = value;
            }
        }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        public DateTime? StartDate { get; private set; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        public DateTime? EndDate { get; private set; }

        /// <summary>
        /// Whether expense items for this subcat should be validated.
        /// </summary>
        public bool Validate { get; set; }

        /// <summary>
        /// The requirements against which items for this should be validated.
        /// Note that this is read from and converted to the ValidationCriteria table in metbase.
        /// </summary>
        public List<ExpenseValidationCriterion> ValidationRequirements { get; set; }

        /// <summary>
        /// Whether the Duty of Care expiry should be validated for the expense item
        /// Note that this is read from and converted to the ValidationCriteria table in metbase.
        /// </summary>
        public bool EnableDoC { get; set; }

        /// <summary>
        /// Whether the Class1 Business Insurance validity should be checked for the expense item
        /// </summary>
        public bool RequireClass1BusinessInsurance { get; set; }

        /// <summary>
        /// Indicates whether the home to office deduction is always set to zeo
        /// </summary>
        public bool HomeToOfficeAlwaysZero
        {
            get
            {
                return bHomeToOfficeAlwaysZero;
            }
        }

        public float? HomeToOfficeFixedMiles { get; internal set; }

        /// <summary>
        /// Indicates whether the home to office deduction is limit is enforced
        /// </summary>
        public bool EnforceToOfficeMileageCap
        {
            get
            {
                return this.enforceHomeToOfficeMileageCap;
            }
        }

        public float? HomeToOfficeMileageCap { get; internal set; }

        /// <summary>
        /// The public transport rate used on the rotational mileage deduction
        /// </summary>
        public int? PublicTransportRate { get; internal set; }

        #endregion

        /// <summary>
        /// Update the subcat ID 
        /// </summary>
        /// <param name="id">The new subcat id</param>
        public void updateID(int id)
        {
            nSubcatid = id;
        }

        public cSubcatVatRate getVatRateByDate(DateTime date)
        {

            foreach (cSubcatVatRate rate in lstVatRates)
            {
                switch (rate.daterangetype)
                {
                    case DateRangeType.Any:
                            return rate;
                    case DateRangeType.Before:
                        {
                            if (date < rate.datevalue1)
                            {
                                return rate;
                            }
                        }
                        break;
                    case DateRangeType.Between:
                        {
                            if (date >= rate.datevalue1 && date <= rate.datevalue2)
                            {
                                return rate;
                            }
                            break;
                        }
                    case DateRangeType.AfterOrEqualTo:
                        {
                            if (date >= rate.datevalue1)
                            {
                                return rate;
                            }
                            break;
                        }
                }
            }
            return null;
        }

        public cSubcatVatRate getVatRateByID(int id)
        {
            foreach (cSubcatVatRate rate in lstVatRates)
            {
                if (rate.vatrateid == id)
                {
                    return rate;
                }
            }
            return null;
        }

        public List<cSubcatVatRate> vatrates
        {
            get { return lstVatRates; }
        }

        public cCountrySubcat getCountry(int countryid)
        {
            foreach (cCountrySubcat country in lstcountries)
            {
                if (country.countryid == countryid)
                {
                    return country;
                }
            }
            return null;
        }
    }
    
    [Serializable()]
    public class cCountrySubcat
    {
        private int nSubcatid;
        private int nCountryid;
        private string sAccountcode;

        public cCountrySubcat(int subcatid, int countryid, string accountcode)
        {
            nSubcatid = subcatid;
            nCountryid = countryid;
            sAccountcode = accountcode;
        }

        public int countryid
        {
            get { return nCountryid; }
        }

        public int SubcatId
        {
            get { return nSubcatid; }
        }

        public string accountcode
        {
            get { return sAccountcode; }
        }

    }

    public enum CalculationType
    {
        NormalItem = 1,
        Meal,
        PencePerMile,
        DailyAllowance,
        FuelReceipt,
        PencePerMileReceipt,
        FixedAllowance,
        FuelCardMileage,
        ItemReimburse,
        ExcessMileage
    }

    [Serializable()]
    public struct sOnlineSubcatInfo
    {
        public Dictionary<int, cSubcat> lstonlinesubcats;
        public List<int> lstsubcatids;
        public List<RoleSubcat> lstRoleSubcats;
    }

    [Serializable]
    public class cSubcatVatRate
    {
        private int nVatRateID;
        private int nSubcatid;
        private double dVatamount;
        private bool bVatreceipt;
        private decimal? dVatlimitwithout;
        private decimal? dVatlimitwith;
        private byte bVatpercent;
        private DateRangeType eDateRangeType;
        private DateTime? dtDateValue1;
        private DateTime? dtDateValue2;

        public cSubcatVatRate(int vatrateid, int subcatid, double vatamount, bool vatreceipt, decimal? vatlimitwithout, decimal? vatlimitwith, byte vatpercent, DateRangeType daterangetype, DateTime? datevalue1, DateTime? datevalue2)
        {
            nVatRateID = vatrateid;
            nSubcatid = subcatid;
            dVatamount = vatamount;
            bVatreceipt = vatreceipt;
            dVatlimitwithout = vatlimitwithout;
            dVatlimitwith = vatlimitwith;
            bVatpercent = vatpercent;
            eDateRangeType = daterangetype;
            dtDateValue1 = datevalue1;
            dtDateValue2 = datevalue2;
        }

        #region properties
        public int vatrateid
        {
            get { return nVatRateID; }
        }
        public int subcatid
        {
            get { return nSubcatid; }
        }
        public double vatamount
        {
            get { return dVatamount; }
        }
        public bool vatreceipt
        {
            get { return bVatreceipt; }
        }
        public decimal? vatlimitwithout
        {
            get { return dVatlimitwithout; }
        }
        public decimal? vatlimitwith
        {
            get { return dVatlimitwith; }
        }
        public byte vatpercent
        {
            get { return bVatpercent; }
        }
        public DateRangeType daterangetype
        {
            get { return eDateRangeType; }
        }
        public DateTime? datevalue1
        {
            get { return dtDateValue1; }
        }
        public DateTime? datevalue2
        {
            get { return dtDateValue2; }
        }
        #endregion
    }

    public enum HomeToLocationType
    {
        /// <summary>
        /// no home to location set.
        /// </summary>
        None = 0,

        /// <summary>
        /// calculate home and office to location diff.
        /// </summary>
        CalculateHomeAndOfficeToLocationDiff = 1,

        /// <summary>
        /// flag home and office to location diff.
        /// </summary>
        FlagHomeAndOfficeToLocationDiff = 2,

        /// <summary>
        /// deduct home to office from every journey.
        /// </summary>
        DeductHomeToOfficeFromEveryJourney = 3,

        /// <summary>
        /// deduct home to office every time home is visited.
        /// </summary>
        DeductHomeToOfficeEveryTimeHomeIsVisited = 4,

        /// <summary>
        /// deduct home to office if start or finish home.
        /// </summary>
        DeductHomeToOfficeIfStartOrFinishHome = 5,

        /// <summary>
        /// deduct first and last home.
        /// </summary>
        DeductFirstAndLastHome = 6,

        /// <summary>
        /// deduct full home to office every time home is visited.
        /// </summary>
        DeductFullHomeToOfficeEveryTimeHomeIsVisited = 7,

        /// <summary>
        /// deduct full home to office if start or finish home.
        /// </summary>
        DeductFullHomeToOfficeIfStartOrFinishHome = 8,
        
        /// <summary>
        /// Deduct a fixed number of miles when home is the from on the first step or the to in the last step
        /// </summary>
        DeductFixedMilesIfStartOrFinishHome = 9,

        /// <summary>
        /// Deduct the public transport rate based on the journey for a claimant with rotational work addresses
        /// </summary>
        JuniorDoctorRotation = 10
        
    }
}


