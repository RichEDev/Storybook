using System;
using System.Collections.Generic;
using SpendManagementLibrary.API;
using SpendManagementLibrary.Expedite;

namespace SpendManagementLibrary
{
    using System.Linq;

    using SpendManagementLibrary.Definitions;
    using SpendManagementLibrary.Enumerators;

    [Serializable()]
    public class cAccount
    {
        private int nAccountid;
        private string sCompanyname;
        private string sCompanyID;
        private string sContact;
        private int nNumUsers;
        private DateTime dtExpiry;
        private byte bAccounttype;
        private string sDBServer;
        private int nDBServerID;
        private string sDBName;
        private string sDBUsername;
        private string sDBPassword;
        private bool bArchived;
        private List<cAccountModuleLicenses> lstAccountModules = new List<cAccountModuleLicenses>();
        private List<cElement> lstElements = new List<cElement>();
        private int? nReportDatabaseID;
        private bool bIsNHSCustomer;
        private bool bContactHelpDeskAllowed;
        bool bQuickEntryFormsEnabled, bEmployeeSearchEnabled, bHotelReviewsEnabled, bAdvancesEnabled, bPostcodeAnyWhereEnabled, bCorporateCardsEnabled;
        private int nStartYear;
        private string sPostcodeAnywhereKey;
        private int nLicencedUsers;
        private string sConnectionString;

        /// <summary>
        /// DVLA Look up licence key for the account
        /// </summary>
        private readonly string _dvlaLookUpLicenceKey;
        private List<int> hostNameIds;
        
        public cAccount()
        {
        }

        /// <summary>
        /// Create a new instance of  cAccount class
        /// </summary>
        /// <param name="postCodeAnywherePaymentServiceKey">The PostcodeAnywhere payment service key</param>
        public cAccount(int accountid, string companyname, string companyid, string contact, int numusers, DateTime expiry, byte accounttype, int dbserverid, string dbserver, string dbname, string dbusername, string dbpassword, bool archived, bool quickEntryFormsEnabled, bool employeeSearchEnabled, bool hotelReviewsEnabled, bool advancesEnabled, bool postcodeAnyWhereEnabled, bool corporateCardsEnabled, int? reportDatabaseID, List<int> hostnameIDs, bool isNHSCustomer, int startYear, bool contactHelpDeskAllowed, string postcodeAnywhereKey, string licencedusers, string connectionString, bool mapsEnabled, bool receiptServiceEnabled, AddressLookupProvider addressLookupProvider, bool addressLookupsChargeable, bool addressLookupPsmaAgreement, bool addressInternationalLookupsAndCoordinates, int addressLookupsRemaining, int addressDistanceLookupsRemaining, string dvlaLookUpLicenceKey, byte? licenceType, bool annualContract, string renewalDate, string contactEmailAddress, bool validationServiceEnabled,bool paymentServiceEnabled, string postCodeAnywherePaymentServiceKey, int? daysToWaitUntilSentEnvelopeIsMissing = null)
        {
            nAccountid = accountid;
            sCompanyname = companyname;
            sCompanyID = companyid;
            sContact = contact;
            nNumUsers = numusers;
            dtExpiry = expiry;
            bAccounttype = accounttype;
            sDBServer = dbserver;
            nDBServerID = dbserverid;
            sDBName = dbname;
            sDBUsername = dbusername;
            sDBPassword = dbpassword;
            bArchived = archived;
            bQuickEntryFormsEnabled = quickEntryFormsEnabled;
            bEmployeeSearchEnabled = employeeSearchEnabled;
            bHotelReviewsEnabled = hotelReviewsEnabled;
            bAdvancesEnabled = advancesEnabled;
            bPostcodeAnyWhereEnabled = postcodeAnyWhereEnabled;
            bCorporateCardsEnabled = corporateCardsEnabled;
            nReportDatabaseID = reportDatabaseID;
            hostNameIds = hostnameIDs;
            bIsNHSCustomer = isNHSCustomer;
            bContactHelpDeskAllowed = contactHelpDeskAllowed;
            nStartYear = startYear;
            sPostcodeAnywhereKey = postcodeAnywhereKey;
            sConnectionString = connectionString;

            this._dvlaLookUpLicenceKey = dvlaLookUpLicenceKey;
            this.MapsEnabled = mapsEnabled;
            this.ReceiptServiceEnabled = receiptServiceEnabled;
            this.ValidationServiceEnabled = validationServiceEnabled;
            this.DaysToWaitUntilSentEnvelopeIsMissing = daysToWaitUntilSentEnvelopeIsMissing;
            this.AddressLookupProvider = addressLookupProvider;
            this.AddressLookupChargeable = addressLookupsChargeable;
            this.AddressLookupPsmaAgreement = addressLookupPsmaAgreement;
            this.AddressInternationalLookupsAndCoordinates = addressInternationalLookupsAndCoordinates;
            this.AddressLookupsRemaining = addressLookupsRemaining;
            this.AddressDistanceLookupsRemaining = addressDistanceLookupsRemaining;
            this.LicenceType = licenceType;
            this.AnnualContract = annualContract;
            this.RenewalDate = renewalDate;
            this.ContactEmailAddress = contactEmailAddress;            

            this.SqlDependencyStarted = false;

            if (licencedusers != string.Empty)
            {
                // decrypt the licencedUsers parameter
                cSecureData crypt = new cSecureData();
                int.TryParse(crypt.Decrypt(licencedusers), out nLicencedUsers);
            }
            this.PaymentServiceEnabled = paymentServiceEnabled;
            this.PostCodeAnywherePaymentServiceKey = postCodeAnywherePaymentServiceKey;
        }
        
        /// <summary>
        /// Returns a list of modules for this account, sorted alphabetically with Spend Management at the 0 index
        /// </summary>
        /// <returns></returns>
        public List<cModule> AccountModules()
        {
            List<cModule> lstModules = new List<cModule>();
            SortedList<string, cModule> lstSortedModules = new SortedList<string, cModule>();


            if (lstAccountModules.Count > 0)
            {
                foreach (cAccountModuleLicenses module in lstAccountModules)
                {
                    if (module.Module.ModuleName == "Spend Management")
                    {
                        lstModules.Insert(0, module.Module);
                    }
                    else
                    {
                        lstSortedModules.Add(module.Module.ModuleName, module.Module);
                    }
                }

                foreach (KeyValuePair<string, cModule> kvp in lstSortedModules)
                {
                    lstModules.Add(kvp.Value);
                }
            }

            return lstModules;
        }

        #region properties

        /// <summary>
        /// The API calls left per minute and hour, the remaining free calls left today, and the total licensed (paid) calls for this account.
        /// </summary>
        public ApiLicenseStatus ApiLicenseStatus { get; set; }

        /// <summary>
        /// The get hostname id.
        /// </summary>
        /// <param name="currentModule"></param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetHostnameID(Modules currentModule)
        {
            return HostManager.GetMatchingHostnameIdFromModuleForAccountIds(this.hostNameIds, currentModule);
        }

        public int? ReportDatabaseID
        {
            get { return nReportDatabaseID; }
        }

        public virtual int accountid
        {
            get { return nAccountid; }
        }

        public bool CorporateCardsEnabled
        {
            get { return bCorporateCardsEnabled; }
        }
        public string companyname
        {
            get { return sCompanyname; }
        }
        public string companyid
        {
            get { return sCompanyID; }
        }
        public string contact
        {
            get { return sContact; }
        }
        public int numusers
        {
            get { return nNumUsers; }
        }
        public DateTime expiry
        {
            get { return dtExpiry; }
        }
        public byte accounttype
        {
            get { return bAccounttype; }
        }
        public string dbserver
        {
            get { return sDBServer; }
        }
        public int dbserverid
        {
            get { return nDBServerID; }
        }
        public string dbname
        {
            get { return sDBName; }
        }
        public string dbusername
        {
            get { return sDBUsername; }
        }
        public string dbpassword
        {
            get { return sDBPassword; }
        }
        public bool archived
        {
            get { return bArchived; }
        }

        public bool QuickEntryFormsEnabled
        {
            get { return bQuickEntryFormsEnabled; }
        }

        public bool EmployeeSearchEnabled
        {
            get { return bEmployeeSearchEnabled; }
        }

        public bool HotelReviewsEnabled
        {
            get { return bHotelReviewsEnabled; }
        }

        public bool AdvancesEnabled
        {
            get { return bAdvancesEnabled; }
        }

        /// <summary>
        /// Gets a DVLA Look up Licence key for the account
        /// </summary>
        public string DvlaLookUpLicenceKey
        {
            get { return this._dvlaLookUpLicenceKey; }
        }

        public bool PostcodeAnyWhereEnabled
        {
            get { return bPostcodeAnyWhereEnabled; }
        }

        /// <summary>
        /// Gets or sets the modules which this account can use
        /// </summary>
        public List<cAccountModuleLicenses> AccountModuleLicenses
        {
            get { return lstAccountModules; }
            set { lstAccountModules = value; }
        }

        /// <summary>
        /// Gets or sets the elements that this account is licenses for
        /// </summary>
        public List<cElement> AccountElements
        {
            get { return lstElements; }
            set { lstElements = value; }
        }

        public List<int> AccountElementIDs
        {
            get 
            {
                List<int> lstLicensedElementIDs = new List<int>();
                foreach (cElement tmpElement in this.AccountElements)
                {
                    if (lstLicensedElementIDs.Contains(tmpElement.ElementID) == false)
                    {
                        lstLicensedElementIDs.Add(tmpElement.ElementID);
                    }
                }
                return lstLicensedElementIDs;
            }
        }

        /// <summary>
        /// Checks the account's licensed elements for the specified Spend Management Element
        /// </summary>
        /// <param name="elementId">The element to look for</param>
        /// <returns>True if the specified element is found, otherwise false</returns>
        public bool HasLicensedElement(SpendManagementElement elementId)
        {
            return this.AccountElements.Any(accountElement => accountElement.ElementID == (int)elementId);
        }

        /// <summary>
        /// Checks the account's licensed elements for the specified Spend Management Element and check if DVLA lookup key is available for the account
        /// </summary>
        /// <param name="elementId">The element to look for</param>
        /// <returns>True if the specified element is found and check if Dvla lookup key is available for the account otherwise false</returns>
        public bool HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement elementId)
        {
            return this.AccountElements.Any(accountElement => accountElement.ElementID == (int)elementId) && !string.IsNullOrEmpty(this.DvlaLookUpLicenceKey);
        }

        /// <summary>
        /// Gets or sets if this account is for an NHS customer
        /// </summary>
        public bool IsNHSCustomer
        {
            get { return bIsNHSCustomer; }
            set { bIsNHSCustomer = value; }
        }

        /// <summary>
        /// Gets or sets whether the account is allowed to access the Service Desk's facilities
        /// </summary>
        public bool ContactHelpDeskAllowed
        {
            get { return bContactHelpDeskAllowed; }
            set { bContactHelpDeskAllowed = value; }
        }

        public int startYear
        {
            get { return nStartYear; }
            set { nStartYear = value; }
        }

        /// <summary>
        /// Returns the postcode anywhere license key for this account
        /// </summary>
        public string PostcodeAnywhereKey
        {
            get { return sPostcodeAnywhereKey; }
        }

        /// <summary>
        /// Gets the number of Concurrent licenced users
        /// </summary>
        public int LicencedUsers
        {
            get { return nLicencedUsers; }
        }

        /// <summary>
        /// Returns the connection string for this customers database
        /// </summary>
        public string ConnectionString
        {
            get { return sConnectionString; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether maps are enabled within Expenses when viewing Mileage expense items on claims
        /// </summary>
        public bool MapsEnabled { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether receipt services are enabled for the account.
        /// </summary>
        public bool ReceiptServiceEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how many days should pass before an <see cref="Envelope"/> of <see cref="Receipt"/>s
        /// counts as missing in the post or not actually sent. This should be flagged to the user.
        /// </summary>
        public int? DaysToWaitUntilSentEnvelopeIsMissing { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether validation services are enabled for the account.
        /// </summary>
        public bool ValidationServiceEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not an SqlDecependency.Start has been successfully called on this Account
        /// </summary>
        public bool SqlDependencyStarted { get; set; }

        /// <summary>
        /// Gets the hostname ids for the account.
        /// </summary>
        public List<int> HostnameIds
        {
            get
            {
                return this.hostNameIds;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating which address lookup database is linked to the account's PostcodeAnywhere license key
        /// </summary>
        public AddressLookupProvider AddressLookupProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the account is charged for address lookups
        /// </summary>
        public bool AddressLookupChargeable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the account has an agreement with PSMA for free address lookups
        /// </summary>
        public bool AddressLookupPsmaAgreement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the account is allowed to perform international address lookups, and distance lookups based on lat/long coordinates
        /// </summary>
        public bool AddressInternationalLookupsAndCoordinates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of address lookups the account has remaining
        /// If AddressLookupChargeable is false then this will be 0
        /// </summary>
        public int AddressLookupsRemaining { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of address distance lookups the account has remaining
        /// If AddressLookupChargeable is false then this will be 0
        /// </summary>
        public int AddressDistanceLookupsRemaining { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the how the account is licenced, either by number of claims (1) or by number of claimants (2) 
        /// </summary>
        public byte? LicenceType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not that account has an annual contract
        /// </summary>
        public bool AnnualContract { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the renewal date of the account's contract
        /// </summary>
        public string RenewalDate { get; set; }

        /// <summary>
        /// Gets or sets the contact email address for the account
        /// </summary>
        public string ContactEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether payment service is enabled for the account.
        /// </summary>
        public bool PaymentServiceEnabled { get; set; }

        /// <summary>
        /// The Post code anywhere Payment service key (if any)
        /// </summary>
        public string PostCodeAnywherePaymentServiceKey { get; }

        #endregion  
    }
}
