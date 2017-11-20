using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// cGlobalProperties class
    /// </summary>
    [Serializable, Obsolete]
    public class cGlobalProperties
    {
        int nMileage;
        string sServer;
        byte bCurrenctype;
        Int16 nDbversion;
        string sCompanyPolicy;
        int nNumrows;
        
        byte bAttempts;
        int nExpiry;
        PasswordLength plPlength;
        int nLength1;
        int nLength2;
        bool bPupper;
        bool bPnumbers;
        int nPrevious;
        int nThresholdtype;
        int nHomecountry;
        byte bPolicytype;
        int nCurimportid;
        bool bUsecostcodes;
        bool bUsedepartmentcodes;
        int nAccountid;
        bool bSingleclaim;
        bool bUsecostcodedesc;
        bool bUsedepartmentdesc;
        bool bAttachreceipts;
       
        int nMainadministrator;
        bool bSearchemployees;
        bool bPreapproval;
        bool bShowreviews;
        int nMileageprev;
        decimal dMinclaimamount;
        decimal dMaxclaimamount;
        bool bExchangereadonly;

        bool bUseprojectcodes;
        bool bUseprojectcodedesc;

        bool bRecordodometer;
        byte bOdometerday;
        bool bAddlocations;

        bool bCostcodeson;
        bool bDepartmentson;
        bool bProjectcodeson;
        bool bPartsubmittal;
        bool bOnlycashcredit;
        string sLanguage;
        string sCurrencysymbol;
        string sCurrencydelimiter;
        bool bLimitfrequency;
        byte bFrequencytype;
        int nFrequencyvalue;
        bool bOverridehome;
        byte bSourceaddress;
        bool bEditmydetails;
        bool bAutoassignallocation;
        bool bEnterodometeronsubmit;
        string sFlagmessage;
        int nBasecurrency;
        

        private bool bAllowselfReg;
        private bool bSelfRegEmpContact;
        private bool bSelfRegHomeAddr;
        private bool bSelfRegEmpInfo;
        private bool bSelfRegRole;
        private bool bSelfRegSignoff;
        private bool bSelfRegAdvancesSignoff;
        private bool bSelfRegDepCostcode;
        private bool bSelfRegBankDetails;
        private bool bSelfRegCarDetails;
        private bool bSelfRegUDF;
        private int nDefaultRole;
        private bool bDisplaylimits;
        private Guid gDrilldownreportid;
        private bool bBlockDrivingLicence;
        private bool bBlockTaxExpiry;
        private bool bBlockMOTExpiry;
        private bool bBlockInsuranceExpiry;
        private bool _blockBreakdownCoverExpiry;
        private bool bDelsetup;
        private bool bDelemployeeadmin;
        private bool bDelemployeeaccounts;
        private bool bDelreports;
        private bool bDelreportsreadonly;
        private bool bDelcheckandpay;
        private bool bDelqedesign;
        private bool bDelcorporatecard;
        //private bool bDelpurchasecards;
        private bool bDelapprovals;
        private bool bDelexports;
        private bool bDelauditlog;
        private bool bSendReviewRequest;
        private bool bClaimantDeclaration;
        private string sDeclarationMsg;
        private string sApproverDeclarationMsg;
        //private bool bAddCompanies;
        private bool bAllowMultipleDestinations;
        private bool bUseMapPoint;
        private bool bUseCostcodeOnGenDet;
        private bool bUseDepartmentOnGenDet;
        private bool bUseProjectcodeOnGenDet;
        private bool bHomeToOffice;
        private DateTime dtModifiedon;
        private int nModifiedby;
        private bool bShowMileageCategoriesForUsers;
        private bool bActivateCarOnUserAdd;
        private bool bAllowUsersToAddCars;
        private bool bAllowEmployeeInOwnSignoffGroup;
        private bool bAllowViewFundDetails;


        /* PO */
        /*
        private FieldType eProductFieldType = FieldType.Text;
        private FieldType eSupplierFieldType = FieldType.RelationshipTextbox;
        private string sPurchaseOrderNumberName = "Purchase Order Number";
        private string sSupplierName = "Supplier";
        private string sDateApprovedName = "Date Approved";
        private string sTotalName = "Total";
        private string sOrderRecurrenceName = "Order Recurrence";
        private string sOrderEndDateName = "Order End Date";
        private string sCommentsName = "Comments";
        private string sProductName = "Product";
        private string sCountryName = "Country";
        private string sCurrencyName = "Currency";
        private string sExchangeRateName = "Exchange Rate";
        private bool bAllowRecurring = true;*/
        private FieldType eProductFieldType;
        private FieldType eSupplierFieldType;
        private string sPurchaseOrderNumberName;
        private string sSupplierName;
        private string sDateApprovedName;
        private string sTotalName;
        private string sOrderRecurrenceName;
        private string sOrderEndDateName;
        private string sCommentsName;
        private string sProductName;
        private string sCountryName;
        private string sCurrencyName;
        private string sExchangeRateName;
        private bool bAllowRecurring;
        // ESR
        private AutoArchiveType eAutoArchiveType;
        private AutoActivateType eAutoActivateType;
        private short nArchiveGracePeriod;

        // Mobile Devices
        private bool bUseMobileDevices;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mileage"></param>
        /// <param name="server"></param>
        /// <param name="currencytype"></param>
        /// <param name="dbversion"></param>
        /// <param name="companyPolicy"></param>
        /// <param name="numrows"></param>
        /// <param name="attempts"></param>
        /// <param name="expiry"></param>
        /// <param name="plength"></param>
        /// <param name="length1"></param>
        /// <param name="length2"></param>
        /// <param name="pupper"></param>
        /// <param name="pnumbers"></param>
        /// <param name="previous"></param>
        /// <param name="thresholdtype"></param>
        /// <param name="homecountry"></param>
        /// <param name="policytype"></param>
        /// <param name="curimportid"></param>
        /// <param name="usecostcodes"></param>
        /// <param name="usedepartmentcodes"></param>
        /// <param name="accountid"></param>
        /// <param name="ccadmin"></param>
        /// <param name="singleclaim"></param>
        /// <param name="usecostcodedesc"></param>
        /// <param name="usedepartmentdesc"></param>
        /// <param name="attachreceipts"></param>
        /// <param name="mainadministrator"></param>
        /// <param name="searchemployees"></param>
        /// <param name="preapproval"></param>
        /// <param name="showreviews"></param>
        /// <param name="mileageprev"></param>
        /// <param name="minclaimamount"></param>
        /// <param name="maxclaimamount"></param>
        /// <param name="exchangereadonly"></param>
        /// <param name="useprojectcodes"></param>
        /// <param name="useprojectcodedesc"></param>
        /// <param name="recordodometer"></param>
        /// <param name="odometerday"></param>
        /// <param name="addlocations"></param>
        /// <param name="costcodeson"></param>
        /// <param name="departmentson"></param>
        /// <param name="projectcodeson"></param>
        /// <param name="partsubmittal"></param>
        /// <param name="onlycashcredit"></param>
        /// <param name="language"></param>
        /// <param name="currencysymbol"></param>
        /// <param name="currencydelimiter"></param>
        /// <param name="limitfrequency"></param>
        /// <param name="frequencytype"></param>
        /// <param name="frequencyvalue"></param>
        /// <param name="overridehome"></param>
        /// <param name="sourceaddress"></param>
        /// <param name="editmydetails"></param>
        /// <param name="autoassignallocation"></param>
        /// <param name="enterodometeronsubmit"></param>
        /// <param name="flagmessage"></param>
        /// <param name="basecurrency"></param>
        /// <param name="allowselfreg"></param>
        /// <param name="selfregempcontact"></param>
        /// <param name="selfreghomeaddr"></param>
        /// <param name="selfregempinfo"></param>
        /// <param name="selfregrole"></param>
        /// <param name="selfregsignoff"></param>
        /// <param name="selfregadvancessignoff"></param>
        /// <param name="selfregdepcostcode"></param>
        /// <param name="selfregbankdetails"></param>
        /// <param name="selfregcardetails"></param>
        /// <param name="selfregudf"></param>
        /// <param name="defaultrole"></param>
        /// <param name="singleclaimcc"></param>
        /// <param name="singleclaimpc"></param>
        /// <param name="drilldownreportid"></param>
        /// <param name="blocklicenceexpiry"></param>
        /// <param name="blocktaxexpiry"></param>
        /// <param name="blockmotexpiry"></param>
        /// <param name="blockinsuranceexpiry"></param>
        /// <param name="blockBreakdownCoverExpiry"></param>
        /// <param name="delsetup"></param>
        /// <param name="delemployeeadmin"></param>
        /// <param name="delemployeeaccounts"></param>
        /// <param name="delreports"></param>
        /// <param name="delreportsreadonly"></param>
        /// <param name="delcheckandpay"></param>
        /// <param name="delqedesign"></param>
        /// <param name="delcorporatecard"></param>
        /// <param name="delapprovals"></param>
        /// <param name="delexports"></param>
        /// <param name="delauditlog"></param>
        /// <param name="sendreviewrequest"></param>
        /// <param name="claimantdeclaration"></param>
        /// <param name="declarationmsg"></param>
        /// <param name="approverdeclarationmsg"></param>
        /// <param name="blockcashcc"></param>
        /// <param name="blockcashpc"></param>
        /// <param name="allowmultipledestinations"></param>
        /// <param name="usemappoint"></param>
        /// <param name="usecostcodeongendet"></param>
        /// <param name="usedepartmentongendet"></param>
        /// <param name="useprojectcodeongendet"></param>
        /// <param name="hometooffice"></param>
        /// <param name="calchometolocation"></param>
        /// <param name="showMileageCategoriesForUsers"></param>
        /// <param name="activateCarOnUserAdd"></param>
        /// <param name="autoCalcHomeToLocation"></param>
        /// <param name="allowUsersToAddCars"></param>
        /// <param name="productFieldType"></param>
        /// <param name="supplierFieldType"></param>
        /// <param name="purchaseOrderNumberName"></param>
        /// <param name="supplierName"></param>
        /// <param name="dateApprovedName"></param>
        /// <param name="totalName"></param>
        /// <param name="orderRecurrenceName"></param>
        /// <param name="orderEndDateName"></param>
        /// <param name="commentsName"></param>
        /// <param name="productName"></param>
        /// <param name="countryName"></param>
        /// <param name="currencyName"></param>
        /// <param name="exchangeRateName"></param>
        /// <param name="allowRecurring"></param>
        /// <param name="autoArchiveType"></param>
        /// <param name="autoActivateType"></param>
        /// <param name="gracePeriod"></param>
        /// <param name="allowEmployeeInOwnSignoffGroup"></param>
        /// <param name="useMobileDevices"></param>
        /// <param name="allowViewFundDetails"></param>
        public cGlobalProperties(int mileage, string server, byte currencytype, Int16 dbversion, string companyPolicy, int numrows, byte attempts, int expiry, PasswordLength plength, int length1, int length2, bool pupper, bool pnumbers, int previous, int thresholdtype, int homecountry, byte policytype, int curimportid, bool usecostcodes, bool usedepartmentcodes, int accountid, bool ccadmin, bool singleclaim, bool usecostcodedesc, bool usedepartmentdesc, bool attachreceipts, int mainadministrator, bool searchemployees, bool preapproval, bool showreviews, int mileageprev, decimal minclaimamount, decimal maxclaimamount, bool exchangereadonly, bool useprojectcodes, bool useprojectcodedesc, bool recordodometer, byte odometerday, bool addlocations, bool costcodeson, bool departmentson, bool projectcodeson, bool partsubmittal, bool onlycashcredit, string language, string currencysymbol, string currencydelimiter, bool limitfrequency, byte frequencytype, int frequencyvalue, bool overridehome, byte sourceaddress, bool editmydetails, bool autoassignallocation, bool enterodometeronsubmit, string flagmessage, int basecurrency, bool allowselfreg, bool selfregempcontact, bool selfreghomeaddr, bool selfregempinfo, bool selfregrole, bool selfregsignoff, bool selfregadvancessignoff, bool selfregdepcostcode, bool selfregbankdetails, bool selfregcardetails, bool selfregudf, int defaultrole, bool singleclaimcc, bool singleclaimpc, Guid drilldownreportid, bool blockdrivinglicence, bool blocktaxexpiry, bool blockmotexpiry, bool blockinsuranceexpiry, bool blockBreakdownCoverExpiry, bool delsetup, bool delemployeeadmin, bool delemployeeaccounts, bool delreports, bool delreportsreadonly, bool delcheckandpay, bool delqedesign, bool delcorporatecard, bool delapprovals, bool delexports, bool delauditlog, bool sendreviewrequest, bool claimantdeclaration, string declarationmsg, string approverdeclarationmsg, bool blockcashcc, bool blockcashpc, bool allowmultipledestinations, bool usemappoint, bool usecostcodeongendet, bool usedepartmentongendet, bool useprojectcodeongendet, bool hometooffice, bool calchometolocation, bool showMileageCategoriesForUsers, bool activateCarOnUserAdd, bool autoCalcHomeToLocation, bool allowUsersToAddCars, FieldType productFieldType, FieldType supplierFieldType, string purchaseOrderNumberName, string supplierName, string dateApprovedName, string totalName, string orderRecurrenceName, string orderEndDateName, string commentsName, string productName, string countryName, string currencyName, string exchangeRateName, bool allowRecurring, AutoArchiveType autoArchiveType, AutoActivateType autoActivateType, short gracePeriod, bool allowEmployeeInOwnSignoffGroup, bool useMobileDevices, bool allowViewFundDetails) //bool addcompanies,
        {
            nMileage = mileage;
            sServer = server;
            bCurrenctype = currencytype;
            nDbversion = dbversion;
            sCompanyPolicy = companyPolicy;
            nNumrows = numrows;
            bAttempts = attempts;
            nExpiry = expiry;
            plPlength = plength;
            nLength1 = length1;
            nLength2 = length2;
            bPupper = pupper;
            bPnumbers = pnumbers;
            nPrevious = previous;
            nThresholdtype = thresholdtype;
            nHomecountry = homecountry;
            bPolicytype = policytype;
            bUsecostcodes = usecostcodes;
            bUsedepartmentcodes = usedepartmentcodes;
            nAccountid = accountid;
            bSingleclaim = singleclaim;
            bUsecostcodedesc = usecostcodedesc;
            bUsedepartmentdesc = usedepartmentdesc;
            bAttachreceipts = attachreceipts;
        
            nMainadministrator = mainadministrator;
            bSearchemployees = searchemployees;
            bPreapproval = preapproval;
            bShowreviews = showreviews;
            nMileageprev = mileageprev;
            dMinclaimamount = minclaimamount;
            dMaxclaimamount = maxclaimamount;
            bExchangereadonly = exchangereadonly;

            bUseprojectcodes = useprojectcodes;
            bUseprojectcodedesc = useprojectcodedesc;
            bRecordodometer = recordodometer;
            bOdometerday = odometerday;
            bAddlocations = addlocations;
            
            bCostcodeson = costcodeson;
            bDepartmentson = departmentson;
            bProjectcodeson = projectcodeson;
            bPartsubmittal = partsubmittal;
            bOnlycashcredit = onlycashcredit;
            sLanguage = language;
            sCurrencysymbol = currencysymbol;
            sCurrencydelimiter = currencydelimiter;
            bLimitfrequency = limitfrequency;
            bFrequencytype = frequencytype;
            nFrequencyvalue = frequencyvalue;
            bOverridehome = overridehome;
            bSourceaddress = sourceaddress;
            bEditmydetails = editmydetails;
            bAutoassignallocation = autoassignallocation;
            bEnterodometeronsubmit = enterodometeronsubmit;
            sFlagmessage = flagmessage;
            nBasecurrency = basecurrency;
            bAllowselfReg = allowselfreg;
            bSelfRegAdvancesSignoff = selfregadvancessignoff;
            bSelfRegBankDetails = selfregbankdetails;
            bSelfRegCarDetails = selfregcardetails;
            bSelfRegDepCostcode = selfregdepcostcode;
            bSelfRegEmpContact = selfregempcontact;
            bSelfRegEmpInfo = selfregempinfo;
            bSelfRegHomeAddr = selfreghomeaddr;
            bSelfRegRole = selfregrole;
            bSelfRegSignoff = selfregsignoff;
            bSelfRegUDF = selfregudf;
            nDefaultRole = defaultrole;
            gDrilldownreportid = drilldownreportid;
            bBlockInsuranceExpiry = blockinsuranceexpiry;
            _blockBreakdownCoverExpiry = blockBreakdownCoverExpiry;
            this.bBlockDrivingLicence =blockdrivinglicence;
            bBlockMOTExpiry = blockmotexpiry;
            bBlockTaxExpiry = blocktaxexpiry;
            bDelsetup = delsetup;
            bDelemployeeadmin = delemployeeadmin;
            bDelemployeeaccounts = delemployeeaccounts;
            bDelreports = delreports;
            bDelreportsreadonly = delreportsreadonly;
            bDelcheckandpay = delcheckandpay;
            bDelqedesign = delqedesign;
            bDelcorporatecard = delcorporatecard;
            //bDelpurchasecards = delpurchasecards;
            bDelapprovals = delapprovals;
            bDelexports = delexports;
            bDelauditlog = delauditlog;
            bSendReviewRequest = sendreviewrequest;
            bClaimantDeclaration = claimantdeclaration;
            sDeclarationMsg = declarationmsg;
            sApproverDeclarationMsg = approverdeclarationmsg;
            bAllowMultipleDestinations = allowmultipledestinations;
            bUseMapPoint = usemappoint;
            bUseCostcodeOnGenDet = usecostcodeongendet;
            bUseDepartmentOnGenDet = usedepartmentongendet;
            bUseProjectcodeOnGenDet = useprojectcodeongendet;
            bHomeToOffice = hometooffice;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
            bShowMileageCategoriesForUsers = showMileageCategoriesForUsers;
            bActivateCarOnUserAdd = activateCarOnUserAdd;
            bAllowUsersToAddCars = allowUsersToAddCars;
            bAllowEmployeeInOwnSignoffGroup = allowEmployeeInOwnSignoffGroup;
            eProductFieldType = productFieldType;
            eSupplierFieldType = supplierFieldType;
            sPurchaseOrderNumberName = purchaseOrderNumberName;
            sSupplierName = supplierName;
            sDateApprovedName = dateApprovedName;
            sTotalName = totalName;
            sOrderRecurrenceName = orderRecurrenceName;
            sOrderEndDateName = orderEndDateName;
            sCommentsName = commentsName;
            sProductName = productName;
            sCountryName = countryName;
            sCurrencyName = currencyName;
            sExchangeRateName = exchangeRateName;
            bAllowRecurring = allowRecurring;      
            eAutoActivateType=autoActivateType;
            eAutoArchiveType=autoArchiveType;
            nArchiveGracePeriod=gracePeriod;
            bUseMobileDevices = useMobileDevices;
            bAllowViewFundDetails = allowViewFundDetails;
        }

        #region properties
        public FieldType ProductFieldType { get { return eProductFieldType; } }
        public FieldType SupplierFieldType { get { return eSupplierFieldType; } }
        public string PurchaseOrderNumberName { get { return sPurchaseOrderNumberName; } }
        public string SupplierName { get { return sSupplierName; } }
        public string DateApprovedName { get { return sDateApprovedName; } }
        public string TotalName { get { return sTotalName; } }
        public string OrderRecurrenceName { get { return sOrderRecurrenceName; } }
        public string OrderEndDateName { get { return sOrderEndDateName; } }
        public string CommentsName { get { return sCommentsName; } }
        public string ProductName { get { return sProductName; } }
        public string CurrencyName { get { return sCurrencyName; } }
        public string ExchangeRateName { get { return sExchangeRateName; } }
        public bool AllowRecurring { get { return bAllowRecurring; } }
        public string CountryName { get { return sCountryName; } }


        public int mileage
        {
            get { return nMileage; }
        }
        public string server
        {
            get { return sServer; }
        }
        public byte currencytype
        {
            get { return bCurrenctype; }
        }
        public Int16 dbversion
        {
            get { return nDbversion; }
        }
        public string CompanyPolicy
        {
            get { return sCompanyPolicy; }
        }
        public int numrows
        {
            get { return nNumrows; }
        }
        public byte attempts
        {
            get { return bAttempts; }
        }
        public int expiry
        {
            get { return nExpiry; }
        }
        public PasswordLength plength
        {
            get { return plPlength; }
        }
        public int length1
        {
            get { return nLength1; }
        }
        public int length2
        {
            get { return nLength2; }
        }
        public bool pupper
        {
            get { return bPupper; }
        }
        public bool pnumbers
        {
            get { return bPnumbers; }
        }
        public int previous
        {
            get { return nPrevious; }
        }
        public int thresholdtype
        {
            get { return nThresholdtype; }
        }
        public int homecountry
        {
            get { return nHomecountry; }
        }
        
        public byte policytype
        {
            get { return bPolicytype; }
        }

        public int curimportid
        {
            get { return nCurimportid; }
        }

       
        public bool usecostcodes
        {
            get { return bUsecostcodes; }
        }
        public bool usedepartmentcodes
        {
            get { return bUsedepartmentcodes; }
        }
        public int accountid
        {
            get { return nAccountid; }
        }
        
        public bool singleclaim
        {
            get { return bSingleclaim; }
        }
        public bool usecostcodedesc
        {
            get { return bUsecostcodedesc; }
        }
        public bool usedepartmentdesc
        {
            get { return bUsedepartmentdesc; }
        }
        public bool attachreceipts
        {
            get { return bAttachreceipts; }
        }



        public int mainadministrator
        {
            get { return nMainadministrator; }
        }

        public bool searchemployees
        {
            get { return bSearchemployees; }
        }
        public bool preapproval
        {
            get { return bPreapproval; }
        }
        public bool showreviews
        {
            get { return bShowreviews; }
        }
        public int mileageprev
        {
            get { return nMileageprev; }
        }
        public decimal minclaimamount
        {
            get { return dMinclaimamount; }
        }
        public decimal maxclaimamount
        {
            get { return dMaxclaimamount; }
        }
        public bool exchangereadonly
        {
            get { return bExchangereadonly; }
        }
        
        public bool useprojectcodes
        {
            get { return bUseprojectcodes; }
        }
        public bool useprojectcodedesc
        {
            get { return bUseprojectcodedesc; }
        }
        public bool recordodometer
        {
            get { return bRecordodometer; }
        }
        public byte odometerday
        {
            get { return bOdometerday; }
        }
        public bool addlocations
        {
            get { return bAddlocations; }
        }
        

        public bool costcodeson
        {
            get { return bCostcodeson; }
        }
        public bool departmentson
        {
            get { return bDepartmentson; }
        }
        public bool projectcodeson
        {
            get { return bProjectcodeson; }
        }
        public bool partsubmittal
        {
            get { return bPartsubmittal; }
        }
        public bool onlycashcredit
        {
            get { return bOnlycashcredit; }
        }
        public string language
        {
            get { return sLanguage; }
        }
        public string currencysymbol
        {
            get { return sCurrencysymbol; }
        }
        public string currencydelimiter
        {
            get { return sCurrencydelimiter; }
        }
        public bool limitfrequency
        {
            get { return bLimitfrequency; }
        }
        public byte frequencytype
        {
            get { return bFrequencytype; }
        }
        public int frequencyvalue
        {
            get { return nFrequencyvalue; }
        }
        public bool overridehome
        {
            get { return bOverridehome; }
        }
        public byte sourceaddress
        {
            get { return bSourceaddress; }
        }
        public bool editmydetails
        {
            get { return bEditmydetails; }
        }
        public bool autoassignallocation
        {
            get { return bAutoassignallocation; }
        }
        public bool enterodometeronsubmit
        {
            get { return bEnterodometeronsubmit; }
        }

        public string flagmessage
        {
            get { return sFlagmessage; }
        }
        
        public int basecurrency
        {
            get { return nBasecurrency; }
        }
        
        public bool allowselfreg
        {
            get { return bAllowselfReg; }
        }
        public bool selfregempcontact
        {
            get { return bSelfRegEmpContact; }
        }
        public bool selfreghomeaddr
        {
            get { return bSelfRegHomeAddr; }
        }
        public bool selfregempinfo
        {
            get { return bSelfRegEmpInfo; }
        }
        public bool selfregrole
        {
            get { return bSelfRegRole; }
        }
        public bool selfregsignoff
        {
            get { return bSelfRegSignoff; }
        }
        public bool selfregadvancessignoff
        {
            get { return bSelfRegAdvancesSignoff; }
        }
        public bool selfregdepcostcode
        {
            get { return bSelfRegDepCostcode; }
        }
        public bool selfregbankdetails
        {
            get { return bSelfRegBankDetails; }
        }
        public bool selfregcardetails
        {
            get { return bSelfRegCarDetails; }
        }
        public bool selfregudf
        {
            get { return bSelfRegUDF; }
        }
        public int defaultrole
        {
            get { return nDefaultRole; }
        }
        
        public bool displaylimits
        {
            get { return bDisplaylimits; }
        }
        public Guid drilldownreportid
        {
            get { return gDrilldownreportid; }
        }

        public bool blockdrivinglicence
        {
            get { return this.bBlockDrivingLicence; }
        }

        public bool blocktaxexpiry
        {
            get { return bBlockTaxExpiry; }
        }
        public bool blockmotexpiry
        {
            get { return bBlockMOTExpiry; }
        }
        public bool blockinsuranceexpiry
        {
            get { return bBlockInsuranceExpiry; }
        }

        /// <summary>
        /// Gets if the Breakdown Cover Expiry should block
        /// </summary>
        public bool BlockBreakdownCoverExpiry
        {
            get { return _blockBreakdownCoverExpiry; }
        }
        public bool delsetup
        {
            get { return bDelsetup; }
        }
        public bool delemployeeadmin
        {
            get { return bDelemployeeadmin; }
        }
        public bool delemployeeaccounts
        {
            get { return bDelemployeeaccounts; }
        }
        public bool delreports
        {
            get { return bDelreports; }
        }
        public bool delreportsreadonly
        {
            get { return bDelreportsreadonly; }
        }
        public bool delcheckandpay
        {
            get { return bDelcheckandpay; }
        }
        public bool delqedesign
        {
            get { return bDelqedesign; }
        }
        public bool delcorporatecard
        {
            get { return bDelcorporatecard; }
        }
        //public bool delpurchasecards
        //{
        //    get { return bDelpurchasecards; }
        //}
        public bool delapprovals
        {
            get { return bDelapprovals; }
        }
        public bool delexports
        {
            get { return bDelexports; }
        }
        public bool delauditlog
        {
            get { return bDelauditlog; }
        }
        public bool sendreviewrequest
        {
            get { return bSendReviewRequest; }
        }
        public bool claimantdeclaration
        {
            get { return bClaimantDeclaration; }
        }
        public string declarationmsg
        {
            get { return sDeclarationMsg; }
        }
        public string approverdeclarationmsg
        {
            get { return sApproverDeclarationMsg; }
        }
        
        //public bool addcompanies
        //{
        //    get { return bAddCompanies; }
        //}
        public bool allowmultipledestinations
        {
            get { return bAllowMultipleDestinations; }
        }
        public bool usemappoint
        {
            get { return bUseMapPoint; }
        }
        public bool usecostcodeongendet
        {
            get { return bUseCostcodeOnGenDet; }
        }
        public bool usedepartmentongendet
        {
            get { return bUseDepartmentOnGenDet; }
        }
        public bool useprojectcodeongendet
        {
            get { return bUseProjectcodeOnGenDet; }
        }
        public bool hometooffice
        {
            get { return bHomeToOffice; }
        }

        public DateTime modifiedon
        {
            get { return dtModifiedon; }
        }
        public int modifiedby
        {
            get { return nModifiedby; }
        }

        public bool ShowMileageCategoriesForUsers
        {
            get { return bShowMileageCategoriesForUsers; }
        }

        public bool ActivateCarOnUserAdd
        {
            get { return bActivateCarOnUserAdd; }
        }

        public bool AllowUsersToAddCars
        {
            get { return bAllowUsersToAddCars; }
        }

        /// <summary>
        /// Check whether View Fund Details is allowed
        /// </summary>
        public bool AllowViewFundDetails
        {
            get { return bAllowViewFundDetails; }
        }

        /// <summary>
        /// Allow employees to sign off their own claims
        /// </summary>
        public bool AllowEmployeeInOwnSignoffGroup
        {
            get { return bAllowEmployeeInOwnSignoffGroup; }
        }

        /// <summary>
        /// AutoActivateType
        /// </summary>
        public AutoActivateType AutoActivateType { get { return eAutoActivateType; } }
        /// <summary>
        /// AutoArchiveType
        /// </summary>
        public AutoArchiveType AutoArchiveType { get { return eAutoArchiveType; } }
        /// <summary>
        /// AutoArchiveGracePeriod
        /// </summary>
        public short AutoArchiveGracePeriod { get { return nArchiveGracePeriod; } }

        /// <summary>
        /// Indicates whether mobile devices functionality is visible within the product
        /// </summary>
        public bool UseMobileDevices
        {
            get { return bUseMobileDevices; }
        }
        #endregion

    }

    /// <summary>
    /// Password length enumeration
    /// </summary>
    public enum PasswordLength
    {
        /// <summary>
        /// Any length
        /// </summary>
        AnyLength = 1,
        /// <summary>
        /// Equal To
        /// </summary>
        EqualTo,
        /// <summary>
        /// Greater Than
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Less Than
        /// </summary>
        LessThan,
        /// <summary>
        /// Between
        /// </summary>
        Between
    }

    /// <summary>
    /// Represents the columns that contain paths in the metabase table dbo.databases.
    /// </summary>
    public enum FilePathType
    {
        /// <summary>
        /// The receipts path column.
        /// </summary>
        Receipt = 0,

        /// <summary>
        /// The card template path column.
        /// </summary>
        CardTemplate = 1,

        /// <summary>
        /// The offline update path column.
        /// </summary>
        OfflineUpdate = 2,

        /// <summary>
        /// The policy file path column.
        /// </summary>
        PolicyFile = 3,

        /// <summary>
        /// The car document path column.
        /// </summary>
        CarDocument = 4,

        /// <summary>
        /// The logo path column.
        /// </summary>
        Logo = 5,

        /// <summary>
        /// The attachments path column.
        /// </summary>
        Attachments = 6
    }
}
