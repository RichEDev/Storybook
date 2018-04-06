namespace SpendManagementLibrary
{
    using System;
    using Enumerators;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Helpers;
    using System.Data;

    /// <summary>
    /// Account properties class
    /// </summary>
    [Serializable]
    public class cAccountProperties
    {
        private int nSubAccountID;

        private int nAccountID;

        private string sEmailServerAddress;

        private bool bShowProductInSearch;

        private string sEmailServerFromAddress;

        private string sDocumentRepository;

        private string sAuditorEmail;

        private bool bKeepInvoiceForecasts;

        private bool bAllowMenuAddContract;

        private bool bAllowArchivedNotesAdd;

        private string sErrorEmailAddress;

        private string sErrorEmailFromAddress;

        private string sEmailAdministrator;

        // password policy properties
        private bool bPwdMCN;

        private bool bPwdMCU;

        private bool bPwdSymbol;

        private bool bPwdExpires;

        private int nPwdExpiryDays;

        private PasswordLength nPwdConstraint;

        private int nPwdLength1;

        private int nPwdLength2;

        private byte nPwdMaxRetries;

        private int nPwdHistoryNum;

        private string sContractKey;

        private bool bAutoUpdateACV;

        private int nDefaultPageSize;

        private string sApplicationURL;

        // expenses properties (formerly global properties)
        private int nMileage;

        private string sServer;

        private CurrencyType eCurrencyType;

        private short nDBVersion;

        private string sCompanyPolicy;

        [Obsolete()]
        private int nNumRows;

        private string sBroadcast;

        [Obsolete()]
        private int nThresholdType;

        private int nHomeCountry;

        private byte bytePolicyType;

        private byte byteLimits;

        private bool bDuplicates;

        private int nCurImportId;

        private byte byteCompMileage;

        private bool bWeekend;

        private bool bUseCostCodes;

        private bool bUseDepartmentCodes;

        private bool bImportCC;

        private bool bCCAdmin;

        private bool bSingleClaim;

        private bool bUseCostCodeDescription;

        private bool bUseDepartmentCodeDescription;

        private bool bAttachReceipts;

        private bool bCCUserSettles;

        private bool bLimitDates;

        private DateTime? dtInitialDate;

        private int? nLimitMonths;

        private bool bFlagDate;

        private byte byteLimitsReceipt;

        private int nMainAdministrator;

        private bool bIncreaseOthers;

        private bool bSearchEmployees;

        private bool bAllowEmployeeToNotifyOfChangeOfDetails;

        private bool bPreApproval;

        private bool bShowReviews;

        private int nMileagePrev;

        private decimal dMinClaimAmount;

        private decimal dMaxClaimAmount;

        private bool bExchangeReadOnly;

        private int nTipLimit;

        private bool bUseProjectCodes;

        private bool bUseProjectCodeDescription;

        private bool bRecordOdometer;

        private byte byteOdometerDay;

        private bool bAddLocations;

        private bool bRejectTip;

        private bool bCostCodesOn;

        private bool bDepartmentsOn;

        private bool bProjectCodesOn;

        private bool bPartSubmit;

        private bool bOnlyCashCredit;

        private string sLanguage;

        private bool bLimitFrequency;

        private byte byteFrequencyType;

        private int nFrequencyValue;

        private bool bOverrideHome;

        private byte byteSourceAddress;

        private bool bEditMyDetails;

        private bool bAutoAssignAllocation;

        private bool bEnterOdometerOnSubmit;

        private bool bDisplayFlagAdded;

        private string sFlagMessage;

        private int? nBaseCurrency;

        private bool bImportPurchaseCard;

        private bool bAllowSelfReg;

        private bool bSelfRegEmployeeContact;

        private bool bSelfRegHomeAddress;

        private bool bSelfRegEmployeeInfo;

        private bool bSelfRegRole;

        private bool bSelfRegSignOff;

        private bool bSelfRegAdvancesSignOff;

        private bool bSelfRegDepartmentCostCode;

        private bool bSelfRegBankDetails;

        private bool bSelfRegCarDetails;

        private bool bSelfRegUDF;

        private bool bSelfRegItemRole;

        private int? nDefaultRole;

        private int? nDefaultItemRole;

        private int nPurchaseCardSubCatId;

        private bool bSingleClaimCC;

        private bool bSingleClaimPC;

        private bool bDisplayLimits;

        private Guid? gDrilldownReport;

        private bool bBlockCashCC;

        private bool bBlockCashPC;

        private bool bBlockDrivingLicence;

        private bool bBlockTaxExpiry;

        private bool bBlockMOTExpiry;

        private bool bBlockInsuranceExpiry;

        private bool _blockBreakdownCoverExpiry;

        private bool bDelSetup;

        private bool bDelEmployeeAdmin;

        private bool bDelEmployeeAccounts;

        private bool bDelReports;

        private bool bDelReportsClaimants;

        private bool bDelCheckAndPay;

        private bool bDelQEDesign;

        private bool bDelCorporateCards;

        //private bool bDelPurchaseCards;
        private bool bDelApprovals;

        private bool bDelExports;

        private bool bDelAuditLog;

        private bool bDelSubmitClaims;

        private bool bSendReviewRequests;

        private bool bClaimantDeclaration;

        private string sDeclarationMsg;

        private string sApproverDeclarationMsg;

        //private bool bAddCompanies;
        private bool bAllowMultipleDestinations;

        private bool bUseMapPoint;

        private bool bMandatoryPostcodeForAddresses;

        private bool bUseCostCodeOnGenDetails;

        private bool bUseDeptOnGenDetails;

        private bool bUseProjectCodeOnGenDetails;

        private bool bHomeToOffice;

        private bool bCalcHomeToLocation;

        private bool bShowMileageCatsForUsers;

        private bool bActivateCarOnUserAdd;

        private bool bAutoCalcHomeToLocation;

        private bool bAllowUsersToAddCars;

        private byte byteMileageCalcType;

        private FieldType byteProductFieldType;

        private FieldType byteSupplierFieldType;

        private string sPONumberName;

        private bool bPONumberGenerate;

        private int nPONumberSequence;

        private string sPONumberFormat;

        private string sDateApprovedName;

        private string sTotalName;

        private string sOrderRecurrenceName;

        private string sOrderEndDateName;

        private string sCommentsName;

        private string sProductName;

        private string sCountryName;

        private string sCurrencyName;

        private string sExchangeRateName;

        private bool bRecurring;

        private short nArchiveGracePeriod;

        private int nGlobalLocalID;

        private cRechargeSetting RechargeSetting;

        // original fwparams properties
        private bool bTaskStartDateMandatory;

        private bool bTaskEndDateMandatory;

        private bool bTaskDueDateMandatory;

        private bool bAutoUpdateLicenceTotal;

        private string sConcatTitle;

        private bool bInflatorActive;

        private bool bInvFreqActive;

        private bool bTermTypeActive;

        private string sValueComments;

        private string sContractDescTitle;

        private string sContractDescShortTitle;

        private bool bContractNumGen;

        private int nContractNumSeq;

        private bool bSupplierStatusEnforced;

        private bool bContractCatMandatory;

        private bool bSupplierLastFinStatusEnabled;

        private bool bSupplierLastFinCheckEnabled;

        private bool bSupplierFYEEnabled;

        private bool bSupplierNumEmployeesEnabled;

        private bool bSupplierTurnoverEnabled;

        private bool bSupplierIntContactEnabled;

        private string sSupplierCatTitle;

        private byte byteOpenSaveAttachments;

        private bool bEnableAttachmentHyperlink;

        private bool bEnableAttachmentUpload;

        private int nCacheTimeout;

        private bool bEnableFlashingNotesIcon;

        private bool bEnableContractNumUpdate;

        private bool bEnableNotesUpdate;

        private string sFYStarts;

        private string sFYEnds;

        private string sRechargeUnrecoveredTitle;

        private string sSupplierRegionTitle;

        private string sSupplierPrimaryTitle;

        private string sSupplierVariationTitle;

        private int nTaskEscalationRepeat;

        private bool bAutoUpdateCVRechargeLive;

        private byte byteLinkAttachmentDefault;

        private string sPenaltyClauseTitle;

        private string sContractScheduleDefault;

        private bool bEnableVariationAutoSeq;

        private int nMaxUploadSize;

        private bool bUseCPExtraInfo;

        private bool bSupplierCatMandatory;

        private bool bEnableContractSavings;

        private bool bEnableRecharge;

        private bool bAllowTeamMemberToApproveOwnClaim;

        private bool bContractDatesMandatory;

        private string sLogoPath;

        private int nCachePeriodShort;

        private int nCachePeriodNormal;

        private int nCachePeriodLong;

        private bool bClaimantsCanAddCompanyAddresses;

        private bool bAllowEmployeeInOwnSignoffGroup;

        private bool bAllowEmpToSpecifyCarStartDateOnAdd;

        private bool bAllowEmpToSpecifyCarDOCOnAdd;

        private bool bAllowEmpToSpecifyCarStartDateOnAddMandatory;

        // help section
        private string sCustomerHelpInformation;

        private string sCustomerHelpContactName;

        private string sCustomerHelpContactTelephone;

        private string sCustomerHelpContactFax;

        private string sCustomerHelpContactAddress;

        private string sCustomerHelpContactEmailAddress;

        //ESR Options
        private AutoArchiveType eAutoArchiveType;

        private AutoActivateType eAutoActivateType;

        private string sImportUsernameFormat;

        private string sImportHomeAddressFormat;

        private bool bCheckESRAssignmentOnEmployeeAdd;

        // Colour options
        private string sColoursMenuBarBackground;

        private string sColoursMenuBarForeground;

        private string sColoursTitleBarBackground;

        private string sColoursTitleBarForeground;

        private string sColoursRowBackground;

        private string sColoursRowForeground;

        private string sColoursAlternateRowBackground;

        private string sColoursAlternateRowForeground;

        private string sColoursFieldBackground;

        private string sColoursFieldForeground;

        private string sColoursPageOptionForeground;

        private string sColoursHover;

        private string sColoursTooltipBackground;

        private string sColoursTooltipText;

        private string sColoursGreenLightField;

        private string sColoursGreenLightSectionText;

        private string sColoursGreenLightSectionBackground;

        private string sColoursGreenLightSectionUnderline;

        // Mobile Devices
        private bool bUseMobileDevices;

        //Expedite Validation Option
        private bool allowReceiptTotalToPassValidation;

        /// <summary>
        /// The corporate diligence start page.
        /// </summary>
        private string corporateDiligenceStartPage;

        /// <summary>
        /// The enable ESR diagnostics.
        /// </summary>
        private bool enableEsrDiagnostics;

        private IOwnership defaultCostCodeOwner;
        private string _retainLabelsTime;

        /// <summary>
        //Duty of care email reminder for claimant on document expiry
        /// </summary>
        private bool remindClaimantOnDOCDocumentExpiryDays;

        /// <summary>
        //Duty of care email reminder for line manager on claimant's document expiry
        /// </summary>
        private bool remindLineManagerOnDOCDocumentExpiryDays;

        /// <summary>
        /// Creates an empty account properties.
        /// </summary>
        public cAccountProperties()
        {

        }

        /// <summary>
        /// Copeis this object.
        /// </summary>
        /// <returns></returns>
        public cAccountProperties Clone()
        {
            cAccountProperties clsProperties;
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                formatter.Serialize(stream, this);
                formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                clsProperties = (cAccountProperties)formatter.Deserialize(stream);
            }
            return clsProperties;
        }

        #region properties

        /// <summary>
        /// Gets or sets if the employee is allowed to signoff their own claim
        /// If false, stops a signoff group being assigned to them that they are in
        /// also hides their claims in any check and pay screens
        /// </summary>
        public bool AllowEmployeeInOwnSignoffGroup { get; set; }

        /// <summary>
        /// Gets or Sets the AccountID that all properties belong to
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// Gets the SubAccountID that all properties belong
        /// </summary>
        public int SubAccountID { get; set; }

        /// <summary>
        /// Gets or sets the email server address
        /// </summary>
        public string EmailServerAddress { get; set; }

        /// <summary>
        /// Gets or set the default 'from' address for customer account
        /// </summary>
        public string EmailServerFromAddress { get; set; }

        /// <summary>
        /// Email address to send application and sql error emails to
        /// </summary>
        public string ErrorEmailAddress { get; set; }

        /// <summary>
        /// Sender Email address for sending application and sql error emails from
        /// </summary>
        public string ErrorEmailFromAddress { get; set; }

        /// <summary>
        /// Gets or sets whether product name is returned in framework searches
        /// </summary>
        public bool ShowProductInSearch { get; set; }

        /// <summary>
        /// Gets or set the repository location for framework attachments
        /// </summary>
        public string DocumentRepository { get; set; }

        /// <summary>
        /// Gets or sets the recipient email address for the audit log purge
        /// </summary>
        public string AuditorEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets whether forecast entry is left in place when moved from forecasted to actual invoice
        /// </summary>
        public bool KeepInvoiceForecasts { get; set; }

        /// <summary>
        /// Gets or sets whether a contract can be added from home page
        /// </summary>
        public bool AllowMenuContractAdd { get; set; }

        /// <summary>
        /// Gets or sets whether notes can be added to archived records
        /// </summary>
        public bool AllowArchivedNotesAdd { get; set; }

        /// <summary>
        /// Gets or sets whether password must contain a number
        /// </summary>
        public bool PwdMustContainNumbers { get; set; }

        /// <summary>
        /// Gets or sets whether password must contain an upper case character
        /// </summary>
        public bool PwdMustContainUpperCase { get; set; }

        /// <summary>
        /// Gets or sets whether password must contain a symbol character
        /// </summary>
        public bool PwdMustContainSymbol { get; set; }

        /// <summary>
        /// Gets or sets whether passwords expire or not
        /// </summary>
        public bool PwdExpires { get; set; }

        /// <summary>
        /// Gets or sets the number of days before a password expires
        /// </summary>
        public int PwdExpiryDays { get; set; }

        /// <summary>
        /// Gets or sets the password length constraint
        /// </summary>
        public PasswordLength PwdConstraint { get; set; }

        /// <summary>
        /// Gets or sets the minimum lower bound password length
        /// </summary>
        public int PwdLength1 { get; set; }

        /// <summary>
        /// Gets or sets the minimum upper bound password length
        /// </summary>
        public int PwdLength2 { get; set; }

        /// <summary>
        /// Gets or sets maximum retries before user account is frozen
        /// </summary>
        public byte PwdMaxRetries { get; set; }

        /// <summary>
        /// Gets or sets number of previous passwords to keep to prevent re-use
        /// </summary>
        public int PwdHistoryNum { get; set; }

        /// <summary>
        /// Gets or sets the Contract Key Prefix
        /// </summary>
        public string ContractKey { get; set; }

        /// <summary>
        /// Gets or sets whether Annual Contract Value is automatically updated from Contract Products
        /// </summary>
        public bool AutoUpdateAnnualContractValue { get; set; }

        /// <summary>
        /// Gets or sets the default page size for grid or reports
        /// </summary>
        public int DefaultPageSize { get; set; }

        /// <summary>
        /// URL of the application for use in email links
        /// </summary>
        public string ApplicationURL { get; set; }

        public int Mileage { get; set; }

        public CurrencyType currencyType { get; set; }

        public short DBVersion { get; set; }

        public string CompanyPolicy { get; set; }

        [Obsolete()]
        public int NumRows { get; set; }

        public string Broadcast { get; set; }

        [Obsolete()]
        public int ThresholdType { get; set; }

        /// <summary>
        /// Determines if postcodes must be entered when users add new addresses
        /// </summary>
        public bool MandatoryPostcodeForAddresses { get; set; }

        /// <summary>
        /// Default country for this customer
        /// </summary>
        public int HomeCountry { get; set; }

        public byte PolicyType { get; set; }



        public int CurImportId { get; set; }


        public bool Weekend { get; set; }

        public bool UseCostCodes { get; set; }

        public bool UseDepartmentCodes { get; set; }

        public bool ImportCC { get; set; }

        public bool CCAdmin { get; set; }

        public bool SingleClaim { get; set; }

        public bool UseCostCodeDescription { get; set; }

        public bool UseDepartmentCodeDescription { get; set; }

        public bool AttachReceipts { get; set; }

        public bool CCUserSettles { get; set; }

        public bool LimitDates { get; set; }

        public DateTime? InitialDate
        {
            get
            {
                return dtInitialDate;
            }
            set
            {
                dtInitialDate = value;
            }
        }

        public int? LimitMonths { get; set; }

        public bool FlagDate { get; set; }



        public int MainAdministrator { get; set; }



        public bool SearchEmployees { get; set; }

        public bool AllowEmployeeToNotifyOfChangeOfDetails { get; set; }

        public bool PreApproval { get; set; }

        public bool ShowReviews { get; set; }

        public int MileagePrev { get; set; }

        public decimal MinClaimAmount { get; set; }

        public decimal MaxClaimAmount { get; set; }

        public bool ExchangeReadOnly { get; set; }



        public bool UseProjectCodes { get; set; }

        public bool UseProjectCodeDescription { get; set; }

        public bool RecordOdometer { get; set; }

        public byte OdometerDay { get; set; }

        /// <summary>
        /// Can claimants add company addresses
        /// </summary>
        public bool ClaimantsCanAddCompanyLocations { get; set; }

        /// <summary>
        /// Can claimants add to and from locations
        /// </summary>
        public bool AddLocations { get; set; }



        public bool CostCodesOn { get; set; }

        public bool DepartmentsOn { get; set; }

        public bool ProjectCodesOn { get; set; }

        public bool PartSubmit { get; set; }

        public bool OnlyCashCredit { get; set; }

        public string Language { get; set; }

        public bool LimitFrequency
        {
            get
            {
                return (FrequencyValue > 0);
            }
        }

        public byte FrequencyType { get; set; }

        public int FrequencyValue { get; set; }

        public bool OverrideHome { get; set; }

        public byte SourceAddress { get; set; }

        public bool EditMyDetails { get; set; }

        public bool EditPreviousClaims { get; set; }

        public bool AutoAssignAllocation { get; set; }

        public bool EnterOdometerOnSubmit { get; set; }

        public bool DisplayFlagAdded { get; set; }

        public string FlagMessage { get; set; }

        public int? BaseCurrency { get; set; }

        public bool ImportPurchaseCard { get; set; }

        public bool AllowSelfReg { get; set; }

        public bool AllowSelfRegEmployeeContact { get; set; }

        public bool AllowSelfRegHomeAddress { get; set; }

        public bool AllowSelfRegEmployeeInfo { get; set; }

        public bool AllowSelfRegRole { get; set; }

        public bool AllowSelfRegSignOff { get; set; }

        public bool AllowSelfRegItemRole { get; set; }

        public bool AllowSelfRegAdvancesSignOff { get; set; }

        public bool AllowSelfRegDepartmentCostCode { get; set; }

        public bool AllowSelfRegBankDetails { get; set; }

        public bool AllowSelfRegCarDetails { get; set; }

        public bool AllowSelfRegUDF { get; set; }

        public int? DefaultRole { get; set; }

        public int? DefaultItemRole { get; set; }

        public int PurchaseCardSubCatId { get; set; }

        public bool SingleClaimCC { get; set; }

        public bool SingleClaimPC { get; set; }



        public Guid? DrilldownReport { get; set; }

        public bool BlockCashCC { get; set; }

        public bool BlockCashPC { get; set; }

        public bool BlockDrivingLicence { get; set; }

        public bool BlockTaxExpiry { get; set; }

        public bool BlockMOTExpiry { get; set; }

        public bool BlockInsuranceExpiry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether driving licence review is enabled.
        /// </summary>
        public bool EnableDrivingLicenceReview { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether driving licence review reminders are enabled.
        /// </summary>
        public bool DrivingLicenceReviewReminder { get; set; }

        /// <summary>
        /// Gets or sets the number of days before review expiry that claimant reminder email is sent
        /// </summary>
        public byte DrivingLicenceReviewReminderDays { get; set; }

        /// <summary>
        /// Gets the if Breakdown Cover Expiry should block
        /// </summary>
        public bool BlockBreakdownCoverExpiry { get; set; }

        public bool UseDateOfExpenseForDutyOfCareChecks { get; set; }

        public bool DelSetup { get; set; }

        public bool DelEmployeeAdmin { get; set; }

        public bool DelEmployeeAccounts { get; set; }

        public bool DelReports { get; set; }

        public bool DelReportsClaimants { get; set; }

        public bool DelCheckAndPay { get; set; }

        public bool DelQEDesign { get; set; }

        public bool DelCorporateCards { get; set; }

        public bool DelApprovals { get; set; }

        public bool DelExports { get; set; }

        public bool DelAuditLog { get; set; }

        public bool DelSubmitClaim { get; set; }

        public bool SendReviewRequests { get; set; }

        public bool ClaimantDeclaration { get; set; }

        public string DeclarationMsg { get; set; }

        public string ApproverDeclarationMsg { get; set; }

        public bool AllowMultipleDestinations { get; set; }

        public bool UseMapPoint { get; set; }

        public bool UseCostCodeOnGenDetails { get; set; }

        public bool UseDeptOnGenDetails { get; set; }

        public bool UseProjectCodeOnGenDetails { get; set; }

        public bool HomeToOffice { get; set; }

        public bool CalcHomeToLocation { get; set; }

        public bool ShowMileageCatsForUsers { get; set; }

        public bool ActivateCarOnUserAdd { get; set; }

        public bool AutoCalcHomeToLocation { get; set; }

        public bool AllowUsersToAddCars { get; set; }

        public byte MileageCalcType { get; set; }

        public FieldType ProductFieldType { get; set; }

        public FieldType SupplierFieldType { get; set; }

        public string PONumberName { get; set; }

        public bool PONumberGenerate { get; set; }

        public int PONumberSequence { get; set; }

        public string PONumberFormat { get; set; }

        public string DateApprovedName { get; set; }

        public string TotalName { get; set; }

        public string OrderRecurrenceName { get; set; }

        public string OrderEndDateName { get; set; }

        public string CommentsName { get; set; }

        public string ProductName { get; set; }

        public string CountryName { get; set; }

        public string CurrencyName { get; set; }

        public bool AllowViewFundDetails { get; set; }

        /// <summary>
        /// Gets the ExchangeRateName
        /// </summary>
        public string ExchangeRateName { get; set; }

        /// <summary>
        /// Recurring
        /// </summary>
        public bool AllowRecurring { get; set; }

        /// <summary>
        /// AutoArchiveType
        /// </summary>
        public AutoArchiveType AutoArchiveType { get; set; }

        /// <summary>
        /// AutoActivateType
        /// </summary>
        public AutoActivateType AutoActivateType { get; set; }

        /// <summary>
        /// ArchiveGracePeriod
        /// </summary>
        public short ArchiveGracePeriod { get; set; }

        /// <summary>
        /// Gets or sets the Import Username Format
        /// </summary>
        public string ImportUsernameFormat { get; set; }

        /// <summary>
        /// Gets or sets the Import Home Address Format
        /// </summary>
        public string ImportHomeAddressFormat { get; set; }

        /// <summary>
        /// Check the employee has an ESR assignment number set when saving the employee, this is just for NHS Customers
        /// </summary>
        public bool CheckESRAssignmentOnEmployeeAdd { get; set; }

        /// <summary>
        /// Gets or set the Global Locale ID
        /// </summary>
        public int GlobalLocaleID { get; set; }

        /// <summary>
        /// Gets or sets the Recharge Settings parameters
        /// </summary>
        public cRechargeSetting RechargeSettings { get; set; }

        /// <summary>
        /// Gets or sets whether the task start date is mandatory
        /// </summary>
        public bool TaskStartDateMandatory { get; set; }

        /// <summary>
        /// Gets or sets whether the task end date is mandatory
        /// </summary>
        public bool TaskEndDateMandatory { get; set; }

        /// <summary>
        /// Gets or sets whether the task due date is mandatory
        /// </summary>
        public bool TaskDueDateMandatory { get; set; }

        public bool AutoUpdateLicenceTotal { get; set; }

        public string ContractCategoryTitle { get; set; }

        public bool InflatorActive { get; set; }

        public bool InvoiceFreqActive { get; set; }

        public bool TermTypeActive { get; set; }

        public string ValueComments { get; set; }

        public string ContractDescTitle { get; set; }

        public string ContractDescShortTitle { get; set; }

        public bool ContractNumGen { get; set; }

        public int ContractNumSeq { get; set; }

        /// <summary>
        /// Actually Supplier Status Mandatory
        /// </summary>
        public bool SupplierStatusEnforced { get; set; }

        public bool ContractCatMandatory { get; set; }

        public bool SupplierLastFinStatusEnabled { get; set; }

        public bool SupplierLastFinCheckEnabled { get; set; }

        public bool SupplierFYEEnabled { get; set; }

        public bool SupplierNumEmployeesEnabled { get; set; }

        public bool SupplierTurnoverEnabled { get; set; }

        public bool SupplierIntContactEnabled { get; set; }

        public string SupplierCatTitle { get; set; }

        public byte OpenSaveAttachments { get; set; }

        public bool EnableAttachmentHyperlink { get; set; }

        public bool EnableAttachmentUpload { get; set; }

        public int CacheTimeout { get; set; }

        public bool EnableFlashingNotesIcon { get; set; }

        public bool EnableContractNumUpdate { get; set; }

        public bool EnableNotesUpdate { get; set; }

        public string FYStarts { get; set; }

        public string FYEnds { get; set; }

        public string RechargeUnrecoveredTitle { get; set; }

        public string SupplierRegionTitle { get; set; }

        public string SupplierPrimaryTitle { get; set; }

        public string SupplierVariationTitle { get; set; }

        public int TaskEscalationRepeat { get; set; }

        public bool AutoUpdateCVRechargeLive { get; set; }

        public byte LinkAttachmentDefault { get; set; }

        public string PenaltyClauseTitle { get; set; }

        public string ContractScheduleDefault { get; set; }

        public bool EnableVariationAutoSeq { get; set; }

        /// <summary>
        /// Consent reminders frequency.
        /// </summary>
        public string FrequencyOfConsentRemindersLookup { get; set; }

        /// <summary>
        /// Look up claimant driving licence automatically.
        /// </summary>
        public bool EnableAutomaticDrivingLicenceLookup { get; set; }

        /// <summary>
        /// Drivinglicence lookup frequency
        /// </summary>
        public string DrivingLicenceLookupFrequency { get; set; }

        /// <summary>
        /// Gets or Sets maximum file attachment size (in bytes 1Mb = 1024)
        /// </summary>
        public int MaxUploadSize { get; set; }

        /// <summary>
        /// Gets or set whether the Contract Property Extra Information is to be used
        /// </summary>
        public bool UseCPExtraInfo { get; set; }

        /// <summary>
        /// Gets or sets whether supplier category selection is mandatory
        /// </summary>
        public bool SupplierCatMandatory { get; set; }

        /// <summary>
        /// Gets or Sets whether the contract savings functionality is activated
        /// </summary>
        public bool EnableContractSavings { get; set; }

        /// <summary>
        /// Gets or Sets whether the recharge functionality is activated
        /// </summary>
        public bool EnableRecharge { get; set; }

        /// <summary>
        /// Gets or set whether a Team Member can approve their own claim
        /// </summary>
        public bool AllowTeamMemberToApproveOwnClaim { get; set; }

        /// <summary>
        /// Gets or sets whether the contract dates entry is mandatory or not
        /// </summary>
        public bool ContractDatesMandatory { get; set; }

        /// <summary>
        /// Gets or Sets the number of minutes for low use data
        /// </summary>
        public int CachePeriodShort { get; set; }

        /// <summary>
        /// Gets or Sets the number of minutes for medium use data
        /// </summary>
        public int CachePeriodNormal { get; set; }

        /// <summary>
        /// Gets or Sets the number of minutes for high use data
        /// </summary>
        public int CachePeriodLong { get; set; }

        /// <summary>
        /// The HTML text advice to display on the help and support page
        /// </summary>
        public string CustomerHelpInformation { get; set; }

        /// <summary>
        /// Internal help contact's Name
        /// </summary>
        public string CustomerHelpContactName { get; set; }

        /// <summary>
        /// Internal help contact's Telephone Number
        /// </summary>
        public string CustomerHelpContactTelephone { get; set; }

        /// <summary>
        /// Internal help contact's Fax Number
        /// </summary>
        public string CustomerHelpContactFax { get; set; }

        /// <summary>
        /// Internal help contact's Postal Address
        /// </summary>
        public string CustomerHelpContactAddress { get; set; }

        /// <summary>
        /// Internal help contact's Email Address
        /// </summary>
        public string CustomerHelpContactEmailAddress { get; set; }

        /// <summary>
        /// Gets or Sets the customer logo path for the current sub-Account
        /// </summary>
        public string LogoPath { get; set; }

        /// <summary>
        /// Gets or sets the email administrator
        /// </summary>
        public string EmailAdministrator { get; set; }

        /// <summary>
        /// Alters the Header Colour
        /// </summary>
        public string ColoursHeaderBackground { get; set; }

        /// <summary>
        /// Alters the Header Colour
        /// </summary>
        public string ColoursHeaderBreadcrumbText { get; set; }

        /// <summary>
        /// Alters the Page Title Colour
        /// </summary>
        public string ColoursPageTitleText { get; set; }

        /// <summary>
        /// Alters the Section Heading Colour
        /// </summary>
        public string ColoursSectionHeadingUnderline { get; set; }

        /// <summary>
        /// Alters the Section Heading Colour
        /// </summary>
        public string ColoursSectionHeadingText { get; set; }

        /// <summary>
        /// Alters the Page Options Colour
        /// </summary>
        public string ColoursPageOptionsBackground { get; set; }

        /// <summary>
        /// Alters the Page Options Colour
        /// </summary>
        public string ColoursPageOptionsText { get; set; }

        /// <summary>
        /// Alters the Table Header Colour
        /// </summary>
        public string ColoursTableHeaderBackground { get; set; }

        /// <summary>
        /// Alters the Table Header Colour
        /// </summary>
        public string ColoursTableHeaderText { get; set; }

        /// <summary>
        /// Alters the Tab Option Colour
        /// </summary>
        public string ColoursTabOptionBackground { get; set; }

        /// <summary>
        /// Alters the Tab Option Colour
        /// </summary>
        public string ColoursTabOptionText { get; set; }
        /// <summary>
        /// Alters the Row Colour
        /// </summary>
        public string ColoursRowBackground { get; set; }

        /// <summary>
        /// Alters the Row Colour
        /// </summary>
        public string ColoursRowText { get; set; }

        /// <summary>
        /// Alters the Alternate Row Colour
        /// </summary>
        public string ColoursAlternateRowBackground { get; set; }

        /// <summary>
        /// Alters the Alternate Row Colour
        /// </summary>
        public string ColoursAlternateRowText { get; set; }

        /// <summary>
        /// Alters the Field Colour
        /// </summary>
        public string ColoursFieldText { get; set; }


        /// <summary>
        /// Alters the Menu Option Colour
        /// </summary>
        public string ColoursMenuOptionStandardText { get; set; }

        /// <summary>
        /// Alters the Menu Option Colour
        /// </summary>
        public string ColoursMenuOptionHoverText { get; set; }

        /// <summary>
        /// Alters the Tooltip Background Colour
        /// </summary>
        public string ColoursTooltipBackground { get; set; }

        /// <summary>
        /// Alters the tooltip text colour
        /// </summary>
        public string ColoursTooltipText { get; set; }

        /// <summary>
        /// Alters the GreenLight Field Colour
        /// </summary>
        public string ColoursGreenLightField { get; set; }

        /// <summary>
        /// Alters the GreenLight Section Text Colour
        /// </summary>
        public string ColoursGreenLightSectionText { get; set; }

        /// <summary>
        /// Alters the GreenLight Section Background Colour
        /// </summary>
        public string ColoursGreenLightSectionBackground { get; set; }

        /// <summary>
        /// Alters the GreenLight Section Underline Colour
        /// </summary>
        public string ColoursGreenLightSectionUnderline { get; set; }

        /// <summary>
        /// Defines whether mobile devices should be utilised
        /// </summary>
        public bool UseMobileDevices { get; set; }

        /// <summary>
        /// Defines if expenses items pass validation if claimed amount is less than the receipt total
        /// </summary>
        public bool AllowReceiptTotalToPassValidation { get; set; }

        /// <summary>
        /// Defines whether an employee can provide their own duty of care documentation when adding a new car themselves
        /// </summary>
        public bool AllowEmpToSpecifyCarDOCOnAdd { get; set; }

        /// <summary>
        /// Defines whether an employee can specify their car start date when adding a new car themselves
        /// </summary>
        public bool AllowEmpToSpecifyCarStartDateOnAdd { get; set; }

        public bool EmpToSpecifyCarStartDateOnAddMandatory { get; set; }

        /// <summary>
        /// Gets or sets Corporate Diligence Start page - if null or blank use home.ASPX
        /// </summary>
        public string CorporateDStartPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable ESR diagnostics.
        /// </summary>
        public bool EnableEsrDiagnostics { get; set; }

        /// <summary>
        /// Gets or sets the single sign on lookup field.
        /// This is used for Single Sign on to validate the "username" attribute on the SAML assertion.
        /// </summary>
        public Guid? SingleSignOnLookupField { get; set; }

        public string CostCodeOwnerBaseKey { get; set; }

        /// <summary>
        /// Gets or sets the include assignment details used in add/edit expenses.
        /// </summary>
        public IncludeEsrDetails IncludeAssignmentDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to auto activate ESR car.
        /// </summary>
        public bool EsrAutoActivateCar { get; set; }

        /// <summary>
        /// Gets or sets the number of approvers to remember for claimant in approval matrix claim.
        /// </summary>
        public int NumberOfApproversToRememberForClaimantInApprovalMatrixClaim { get; set; }

        /// <summary>
        /// Gets or sets whether cars should be treated as inactive if the date is outside of the start/end date of the car
        /// </summary>
        public bool DisableCarOutsideOfStartEndDate { get; set; }

        public string RetainLabelsTime
        {
            get { return string.IsNullOrEmpty(_retainLabelsTime) ? "3" : _retainLabelsTime; }
            set { _retainLabelsTime = value; }
        }

        /// <summary>
        /// Whether to show the full home address on editing claims rather than just "Home" for data protection reasons
        /// </summary>
        public bool ShowFullHomeAddressOnClaims { get; set; }


        /// <summary>
        /// The shortcut keyword for quick entry of the user's home address, usually "home"
        /// </summary>
        public string HomeAddressKeyword { get; set; }


        /// <summary>
        /// The shortcut keyword for quick entry of the user's work address, usually "office"
        /// </summary>
        public string WorkAddressKeyword { get; set; }

        /// <summary>
        /// Whether to force users to enter an address name before they can use the address, this option is only available to Address+ Standard (TeleAtlas) accounts
        /// </summary>
        public bool ForceAddressNameEntry { get; set; }

        /// <summary>
        /// The message presented to users when they are prompted to enter an address name
        /// </summary>
        public string AddressNameEntryMessage { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes of inactivity from the user before that user is classed as idle.
        /// </summary>
        public int IdleTimeout { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds that the countdown warning will show before the user is automatically logged off the system once they have become inactive.
        /// </summary>
        public int CountdownTimer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not internal support tickets are enabled
        /// </summary>
        public bool EnableInternalSupportTickets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to display addresses imported by ESR in address search results
        /// </summary>
        public bool DisplayEsrAddressesInSearchResults { get; set; }


        /// <summary>
        /// The number of days to wait after an envelope is marked as sent before 
        /// a notification is sent to the claimant asking for an update.
        /// </summary>
        public int NotifyWhenEnvelopeNotReceivedDays { get; set; }

        /// <summary>
        /// The number of days before the email reminder sent to claimant on DOC Document Expiry 
        /// A reminder email is sent to claimant on DOC Document Expiry
        /// </summary>
        public int RemindClaimantOnDOCDocumentExpiryDays { get; set; }

        /// <summary>
        /// The number of days before the email reminder sent to Approver on claimant DOC Document Expiry 
        /// A reminder email is sent to Approver on claimant's DOC Document Expiry
        /// </summary>
        public int RemindApproverOnDOCDocumentExpiryDays { get; set; }

        /// <summary>
        /// Gets or sets the driving licence review frequency.
        /// </summary>
        public int DrivingLicenceReviewFrequency { get; set; }        

        /// <summary>
        /// Approver can be a Team or Line manager
        /// A reminder email is sent to approver on DOC Document Expiry
        /// </summary>
        public string DutyOfCareApprover { get; set; }

        /// <summary>
        /// Team as the approver
        /// A reminder email is sent to a Team on DOC Document Expiry
        /// Gets or sets the esr rounding type
        /// Down
        /// Up (maths)
        /// Force up.
        /// </summary>
        public EsrRoundingType EsrRounding { get; set; }

        /// <summary>
        /// Enable/Disable Delegate options settings to users with Delegate Access Role
        /// </summary>
        public string DutyOfCareTeamAsApprover { get; set; }

        /// <summary>
        /// If the ESR inbound file will be created as a summary or not
        /// </summary>
        public bool SummaryEsrInboundFile { get; set; }
        /// <summary>
        /// Flag to enable approver reminder of pending claims 
        /// </summary>
        public bool EnableClaimApprovalReminders { get; set; }

        /// <summary>
        /// Flag to enable current unsubmitted- claims reminder
        /// </summary>
        public bool EnableCurrentClaimsReminders { get; set; }
        /// <summary>
        /// remind for every days set in frequency.
        /// </summary>
        public int ClaimApprovalReminderFrequency { get; set; }

        /// <summary>
        /// remind for every days set in frequency.
        /// </summary>
        public int CurrentClaimsReminderFrequency { get; set; }


        /// <summary>
        /// Enable/Disable Delegate options settings to users with Delegate Access Role
        /// </summary>
        public bool EnableDelegateOptionsForDelegateAccessRole { get; set; }

        /// <summary>
        /// Enable/Disable Ability to manually edit the AssignmentSupervisor for ESR
        /// </summary>
        public bool EnableESRManualAssignmentSupervisor { get; set; }

        /// <summary>
        /// Enable/Disable Automatic Calculations For Allocating FuelReceipt VAT to Mileage
        /// </summary>
        public bool EnableCalculationsForAllocatingFuelReceiptVatToMileage { get; set; }

        /// <summary>
        /// Allow the user to pick from a list of currently (based on the expense item date) active work addresses in order to establish the current "work" address.
        /// </summary>
        public bool MultipleWorkAddress { get; set; }


        /// <summary>
        /// Gets or sets the exchange rates of all active currencies will be automatically updated.
        /// </summary>
        public bool EnableAutoUpdateOfExchangeRates { get; set; }

        /// <summary>
        /// Gets or sets the date that the <see cref="EnableAutoUpdateOfExchangeRates"/> was set to true
        /// </summary>
        public DateTime? EnableAutoUpdateOfExchangeRatesActivatedDate { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate provider
        /// </summary>
        public ExchangeRateProvider ExchangeRateProvider { get; set; }

        /// <summary>
        /// Gets or sets the message shown when a user locks their account.
        /// </summary>
        public string AccountLockedMessage { get; set; }

        /// <summary>
        /// Gets or sets the message shown when a user tries to login to a lock account.
        /// </summary>
        public string AccountCurrentlyLockedMessage { get; set; }

        /// <summary>
        /// Gets or sets enabling primary home addresses only for esr
        /// </summary>
        public bool EsrPrimaryAddressOnly { get; set; }

        /// <summary>
        /// Gets or sets whether expense items should be blocked from being submitted if it has been set as a credit or purchase card item but not matched to a credit card transaction
        /// </summary>
        public bool BlockUnmachedExpenseItemsBeingSubmitted { get; set; }

        /// <summary>
        /// Gets or sets wether the use can get vehicle information from an external service.
        /// </summary>
        public bool VehicleLookup { get; set; }

        #endregion
    }
}