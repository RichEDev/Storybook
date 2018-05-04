namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using BusinessLogic.GeneralOptions.AddEditExpense;
    using BusinessLogic.GeneralOptions.Password;

    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Logic_Classes;

    using Utilities.DistributedCaching;

    using BusinessLogic.GeneralOptions.ESR;

    /// <summary>
    /// cAccountSubAccounts class
    /// </summary>
    public class cAccountSubAccountsBase
    {
        /// <summary>
        /// Customer Account Id
        /// </summary>
        private readonly int _accountId;

        /// <summary>
        /// SubAccount collection
        /// </summary>
        private readonly SortedList<int, cAccountSubAccount> _lstSubAccounts;

        //private IDBConnection expdata;

        /// <summary>
        /// Cache marshalling variable
        /// </summary>
        // private static List<int> accountsCaching;

        #region properties

        /// <summary>
        /// Gets the number of subaccounts defined
        /// </summary>
        public int Count
        {
            get { return _lstSubAccounts.Count; }
        }

        /// <summary>
        /// Gets the key for use in caching
        /// </summary>
        private string _cacheKey
        {
            get { return "accountsubaccounts"; }
        }

        private readonly Cache _cache;

        /// <summary>
        /// Gets the current AccountId
        /// </summary>
        protected int AccountId
        {
            get { return _accountId; }
        }

        #endregion

        /// <summary>
        /// cAccountSubAccountsBase constructor
        /// </summary>
        /// <param name="accountId">Customer DB Account Id</param>
        /// <param name="connection"></param>
        public cAccountSubAccountsBase(int accountId, IDBConnection connection = null)
        {
            _cache = new Cache();
            _accountId = accountId;
            var tmpList = (SortedList<int, cAccountSubAccount>)_cache.Get(accountId, string.Empty, this._cacheKey);
            this._lstSubAccounts = tmpList ?? this.GetSubAccounts();
        }

        /// <summary>
        /// Obtain sub account properties for a particular sub account id
        /// </summary>
        /// <param name="subAccountId">Sub Account Id to retrieve properties for</param>
        /// <returns>cAccountSubAccount class entity</returns>
        public cAccountSubAccount getSubAccountById(int subAccountId)
        {
            cAccountSubAccount subAccount = null;

            if (_lstSubAccounts.ContainsKey(subAccountId))
            {
                subAccount = (cAccountSubAccount)_lstSubAccounts[subAccountId];
            }

            return subAccount;
        }

        /// <summary>
        /// Gets the first subaccount in the database
        /// </summary>
        /// <returns>cAccountSubAccount class entity</returns>
        public cAccountSubAccount getFirstSubAccount()
        {
            cAccountSubAccount subacc = null;

            if (_lstSubAccounts.Count > 0)
            {
                subacc = _lstSubAccounts[_lstSubAccounts.Keys[0]];
            }

            return subacc;
        }

        /// <summary>
        /// Gets the sub account properties collection
        /// </summary>
        /// <returns>Collection of cAccountSubAccount class entities</returns>
        public SortedList<int, cAccountSubAccount> getSubAccountsCollection()
        {
            return _lstSubAccounts;
        }

        /// <summary>
        /// Populate exchange rates when user has setup auto update of exchange rates in general options
        /// </summary>
        /// <param name="currencyType">The currency type of the account</param>
        /// <param name="enableAutoUpdateOfExchangeRatesActivatedDate">When the option was enabled</param>
        public void PopulateExchangeRateRanges(CurrencyType currencyType, DateTime? enableAutoUpdateOfExchangeRatesActivatedDate)
        {
            var exchangeRates = AutoUpdateExchangeRateModifier.New(currencyType, this.AccountId);

            exchangeRates.PopulateRanges(enableAutoUpdateOfExchangeRatesActivatedDate);
        }


        /// <summary>
        /// Relabel any report columns, changing old value to new value
        /// </summary>
        /// <param name="relabelParam">The Relabel Param to use</param>
        /// <param name="oldValue">The original value</param>
        /// <param name="newValue">The new replacement value</param>
        public void RelabelReportColumns(string relabelParam, string oldValue, string newValue)
        {
            var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            connection.sqlexecute.Parameters.Clear();
            connection.sqlexecute.Parameters.AddWithValue("@relabelParam", relabelParam);
            connection.sqlexecute.Parameters.AddWithValue("@oldValue", oldValue);
            connection.sqlexecute.Parameters.AddWithValue("@newValue", newValue);
            connection.ExecuteProc("dbo.RelabelReportColumns");
        }

        /// <summary>
        /// Gets collection of sub account properties
        /// </summary>
        /// <returns></returns>
        private SortedList<int, cAccountSubAccount> GetSubAccounts(IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            const string Sql = "SELECT subAccountId, description, archived, createdon, createdby, modifiedon, modifiedby FROM dbo.accountsSubAccounts";
            var subAccounts = new SortedList<int, cAccountSubAccount>();
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.CommandText = Sql;

            using (var reader = expdata.GetReader(Sql))
            {
                while (reader.Read())
                {
                    int subaccId = reader.GetInt32(reader.GetOrdinal("subAccountId"));
                    string desc = reader.GetString(reader.GetOrdinal("description"));
                    bool archived = reader.GetBoolean(reader.GetOrdinal("archived"));
                    DateTime created = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    int createdby = 0;
                    if (!reader.IsDBNull(reader.GetOrdinal("createdby")))
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    DateTime? modifiedon = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifiedon")))
                    {
                        modifiedon = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                    }
                    int? modifiedby = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("modifiedby")))
                    {
                        modifiedby = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                    }

                    var subacc = new cAccountSubAccount(subaccId, desc, archived, this.getProperties(subaccId), created, createdby, modifiedon, modifiedby);
                    subAccounts.Add(subaccId, subacc);
                }
                reader.Close();
            }

            _cache.Add(this._accountId, string.Empty, this._cacheKey, subAccounts);

            expdata.sqlexecute.Parameters.Clear();

            return subAccounts;
        }

        /// <summary>
        /// Get the properties for a particular sub account id
        /// </summary>
        /// <param name="subAccountId">Sub account to retrieve properties for</param>
        /// <returns>cAccountProperties collection</returns>
        private cAccountProperties getProperties(int subAccountId, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            cAccountProperties lstAccountProperties = new cAccountProperties();
            expdata.sqlexecute.Parameters.Clear();
            //int subAccountID;
            //DateTime? createdOn, modifiedOn;
            //int? createdBy, modifiedBy;

            string rsRefAs = string.Empty;
            string rsEmpRefAs = string.Empty;
            int rsCPAction = 0;
            int rsFinYear = 0;
            int rsRechargePd = 0;

            for (int globalIdx = 0; globalIdx < 2; globalIdx++)
            {
                // first pass gets the sub-account specific properties, second pass will get the global properties (i.e. pwd policy etc.)
                expdata.sqlexecute.Parameters.Clear();

                string strSQL = "SELECT subAccountID, stringKey, stringValue, createdOn, createdBy, modifiedOn, modifiedBy FROM dbo.accountProperties where isGlobal = @isGlobal";
                if (globalIdx == 0)
                {
                    strSQL += " and subAccountID = @subAccId";
                    expdata.sqlexecute.Parameters.AddWithValue("@subAccId", subAccountId);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@isGlobal", globalIdx);
                expdata.sqlexecute.CommandText = strSQL;

                lstAccountProperties.AccountID = _accountId;
                lstAccountProperties.SubAccountID = subAccountId;

                using (var reader = expdata.GetReader(strSQL))
                {
                    var stringKeyOrd = reader.GetOrdinal("stringKey");
                    var stringValueOrd = reader.GetOrdinal("stringValue");

                    while (reader.Read())
                    {
                        //subAccountID = reader.GetInt32(reader.GetOrdinal("subAccountID"));
                        string stringKey = reader.GetString(stringKeyOrd).Trim();
                        string stringValue;
                        if (reader.IsDBNull(stringValueOrd))
                        {
                            stringValue = string.Empty;
                        }
                        else
                        {
                            stringValue = reader.GetString(stringValueOrd).Trim();
                        }

                        switch (stringKey)
                        {
                            case "emailServerAddress":
                                lstAccountProperties.EmailServerAddress = stringValue;
                                break;
                            case "emailServerFromAddress":
                                lstAccountProperties.EmailServerFromAddress = stringValue;
                                break;
                            case "showProductInSearch":
                                lstAccountProperties.ShowProductInSearch = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "documentRepository":
                                lstAccountProperties.DocumentRepository = stringValue;
                                break;
                            case "auditorEmailAddress":
                                lstAccountProperties.AuditorEmailAddress = stringValue;
                                break;
                            case "keepInvoiceForecasts":
                                lstAccountProperties.KeepInvoiceForecasts = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowMenuAddContract":
                                lstAccountProperties.AllowMenuContractAdd = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowArchivedNotesAdd":
                                lstAccountProperties.AllowArchivedNotesAdd = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "pwdMCN":
                                lstAccountProperties.PwdMustContainNumbers = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "pwdMCU":
                                lstAccountProperties.PwdMustContainUpperCase = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "pwdSymbol":
                                lstAccountProperties.PwdMustContainSymbol = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "pwdExpires":
                                lstAccountProperties.PwdExpires = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "pwdExpiryDays":
                                lstAccountProperties.PwdExpiryDays = Convert.ToInt32(stringValue);
                                break;
                            case "pwdConstraint":
                                lstAccountProperties.PwdConstraint = (PasswordLength)Convert.ToByte(stringValue);
                                break;
                            case "pwdLength1":
                                lstAccountProperties.PwdLength1 = Convert.ToInt32(stringValue);
                                break;
                            case "pwdLength2":
                                lstAccountProperties.PwdLength2 = Convert.ToInt32(stringValue);
                                break;
                            case "pwdMaxRetries":
                                lstAccountProperties.PwdMaxRetries = Convert.ToByte(stringValue);
                                break;
                            case "pwdHistoryNum":
                                lstAccountProperties.PwdHistoryNum = Convert.ToInt32(stringValue);
                                break;
                            case "contractKey":
                                lstAccountProperties.ContractKey = stringValue;
                                break;
                            case "autoUpdateCV":
                                lstAccountProperties.AutoUpdateAnnualContractValue = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "defaultPageSize":
                                lstAccountProperties.DefaultPageSize = Convert.ToInt32(stringValue);
                                break;
                            case "applicationURL":
                                lstAccountProperties.ApplicationURL = stringValue;
                                break;
                            case "mileage":
                                lstAccountProperties.Mileage = Convert.ToInt32(stringValue);
                                break;
                            case "currencyType":
                                lstAccountProperties.currencyType = (CurrencyType)Convert.ToInt32(stringValue);
                                break;
                            case "dbVersion":
                                lstAccountProperties.DBVersion = Convert.ToInt16(stringValue);
                                break;
                            case "CompanyPolicy":
                                lstAccountProperties.CompanyPolicy = stringValue;
                                break;
                            case "broadcast":
                                lstAccountProperties.Broadcast = stringValue;
                                break;
                            //case "attempts":
                            //    lstAccountProperties.Attempts = Convert.ToByte(stringValue);
                            //    break;
                            case "homeCountry":
                                lstAccountProperties.HomeCountry = Convert.ToInt32(stringValue);
                                break;
                            case "policyType":
                                lstAccountProperties.PolicyType = Convert.ToByte(stringValue);
                                break;

                            case "curImportId":
                                lstAccountProperties.CurImportId = Convert.ToInt32(stringValue);
                                break;
                            case "weekend":
                                lstAccountProperties.Weekend = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useCostCodes":
                                lstAccountProperties.UseCostCodes = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useDepartmentCodes":
                                lstAccountProperties.UseDepartmentCodes = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "importCC":
                                lstAccountProperties.ImportCC = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "ccAdmin":
                                lstAccountProperties.CCAdmin = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "singleClaim":
                                lstAccountProperties.SingleClaim = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useCostCodeDescription":
                                lstAccountProperties.UseCostCodeDescription = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useDepartmentCodeDescription":
                                lstAccountProperties.UseDepartmentCodeDescription = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "attachReceipts":
                                lstAccountProperties.AttachReceipts = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "ccUserSettles":
                                lstAccountProperties.CCUserSettles = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "limitDates":
                                lstAccountProperties.LimitDates = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "initialDate":
                                if (stringValue == string.Empty)
                                {
                                    lstAccountProperties.InitialDate = null; // new DateTime(1900, 1, 1);
                                }
                                else
                                {
                                    lstAccountProperties.InitialDate = Convert.ToDateTime(stringValue);
                                }
                                break;
                            case "limitMonths":
                                if (stringValue == string.Empty)
                                {
                                    lstAccountProperties.LimitMonths = null; // 0;
                                }
                                else
                                {
                                    lstAccountProperties.LimitMonths = Convert.ToInt32(stringValue);
                                }
                                break;
                            case "flagDate":
                                lstAccountProperties.FlagDate = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "mainAdministrator":
                                lstAccountProperties.MainAdministrator = Convert.ToInt32(stringValue);
                                break;
                            case "searchEmployees":
                                lstAccountProperties.SearchEmployees = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "preApproval":
                                lstAccountProperties.PreApproval = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "showReviews":
                                lstAccountProperties.ShowReviews = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "mileagePrev":
                                lstAccountProperties.MileagePrev = Convert.ToInt32(stringValue);
                                break;
                            case "minClaimAmount":
                                lstAccountProperties.MinClaimAmount = Convert.ToDecimal(stringValue);
                                break;
                            case "maxClaimAmount":
                                lstAccountProperties.MaxClaimAmount = Convert.ToDecimal(stringValue);
                                break;
                            case "exchangeReadOnly":
                                lstAccountProperties.ExchangeReadOnly = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useProjectCodes":
                                lstAccountProperties.UseProjectCodes = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useProjectCodeDescription":
                                lstAccountProperties.UseProjectCodeDescription = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "odometerDay":
                                lstAccountProperties.OdometerDay = Convert.ToByte(stringValue);
                                break;
                            case "addLocations":
                                lstAccountProperties.AddLocations = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "costCodesOn":
                                lstAccountProperties.CostCodesOn = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "departmentsOn":
                                lstAccountProperties.DepartmentsOn = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "projectCodesOn":
                                lstAccountProperties.ProjectCodesOn = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "partSubmittal":
                                lstAccountProperties.PartSubmit = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "onlyCashCredit":
                                lstAccountProperties.OnlyCashCredit = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "language":
                                lstAccountProperties.Language = stringValue;
                                break;
                            case "frequencyType":
                                if (stringValue == string.Empty)
                                {
                                    lstAccountProperties.FrequencyType = 0;
                                }
                                else
                                {
                                    lstAccountProperties.FrequencyType = Convert.ToByte(stringValue);
                                }
                                break;
                            case "frequencyValue":
                                if (stringValue == string.Empty)
                                {
                                    lstAccountProperties.FrequencyValue = 0;
                                }
                                else
                                {
                                    lstAccountProperties.FrequencyValue = Convert.ToInt32(stringValue);

                                }
                                break;
                            case "overrideHome":
                                lstAccountProperties.OverrideHome = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "sourceAddress":
                                lstAccountProperties.SourceAddress = Convert.ToByte(stringValue);
                                break;
                            case "editMyDetails":
                                lstAccountProperties.EditMyDetails = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "autoAssignAllocation":
                                lstAccountProperties.AutoAssignAllocation = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "enterOdometerOnSubmit":
                                lstAccountProperties.EnterOdometerOnSubmit = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "displayFlagAdded":
                                lstAccountProperties.DisplayFlagAdded = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "flagMessage":
                                lstAccountProperties.FlagMessage = stringValue;
                                break;
                            case "baseCurrency":
                                if (stringValue == string.Empty)
                                {
                                    lstAccountProperties.BaseCurrency = null;
                                }
                                else
                                {
                                    lstAccountProperties.BaseCurrency = Convert.ToInt32(stringValue);
                                }
                                break;
                            case "importPurchaseCard":
                                lstAccountProperties.ImportPurchaseCard = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfReg":
                                lstAccountProperties.AllowSelfReg = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegRole":
                                lstAccountProperties.AllowSelfRegRole = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegItemRole":
                                lstAccountProperties.AllowSelfRegItemRole = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegEmpContact":
                                lstAccountProperties.AllowSelfRegEmployeeContact = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegHomeAddress":
                                lstAccountProperties.AllowSelfRegHomeAddress = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegEmpInfo":
                                lstAccountProperties.AllowSelfRegEmployeeInfo = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegSignOff":
                                lstAccountProperties.AllowSelfRegSignOff = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegAdvancesSignOff":
                                lstAccountProperties.AllowSelfRegAdvancesSignOff = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegDeptCostCode":
                                lstAccountProperties.AllowSelfRegDepartmentCostCode = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegBankDetails":
                                lstAccountProperties.AllowSelfRegBankDetails = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegCarDetails":
                                lstAccountProperties.AllowSelfRegCarDetails = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowSelfRegUDF":
                                lstAccountProperties.AllowSelfRegUDF = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "purchaseCardSubCatId":
                                if (stringValue == string.Empty)
                                {
                                    lstAccountProperties.PurchaseCardSubCatId = 0;
                                }
                                else
                                {
                                    lstAccountProperties.PurchaseCardSubCatId = Convert.ToInt32(stringValue);
                                }
                                break;
                            case "defaultRole":
                                if (stringValue == string.Empty)
                                {
                                    lstAccountProperties.DefaultRole = null;
                                }
                                else
                                {
                                    lstAccountProperties.DefaultRole = Convert.ToInt32(stringValue);
                                }
                                break;
                            case "defaultItemRole":
                                if (stringValue == string.Empty)
                                {
                                    lstAccountProperties.DefaultItemRole = null;
                                }
                                else
                                {
                                    lstAccountProperties.DefaultItemRole = Convert.ToInt32(stringValue);
                                }
                                break;
                            case "singleClaimCC":
                                lstAccountProperties.SingleClaimCC = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "singleClaimPC":
                                lstAccountProperties.SingleClaimPC = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "drilldownReport":
                                if (string.IsNullOrEmpty(stringValue))
                                {
                                    lstAccountProperties.DrilldownReport = null; // Guid.Empty;
                                }
                                else
                                {
                                    lstAccountProperties.DrilldownReport = new Guid(stringValue);
                                }
                                break;
                            case "blockCashCC":
                                lstAccountProperties.BlockCashCC = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "blockCashPC":
                                lstAccountProperties.BlockCashPC = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "enableAutomaticDrivingLicenceLookup":
                                lstAccountProperties.EnableAutomaticDrivingLicenceLookup = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "consentReminderFrequency":
                                lstAccountProperties.FrequencyOfConsentRemindersLookup = stringValue;
                                break;
                            case "drivingLicenceLookupFrequency":
                                lstAccountProperties.DrivingLicenceLookupFrequency = stringValue;
                                break;
                            case "blockDrivingLicence":
                                lstAccountProperties.BlockDrivingLicence = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "blockTaxExpiry":
                                lstAccountProperties.BlockTaxExpiry = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "blockMOTExpiry":
                                lstAccountProperties.BlockMOTExpiry = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "blockInsuranceExpiry":
                                lstAccountProperties.BlockInsuranceExpiry = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "blockBreakdownCoverExpiry":
                                lstAccountProperties.BlockBreakdownCoverExpiry = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useDateOfExpenseForDutyOfCareChecks":
                                lstAccountProperties.UseDateOfExpenseForDutyOfCareChecks = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delSetup":
                                lstAccountProperties.DelSetup = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delEmployeeAccounts":
                                lstAccountProperties.DelEmployeeAccounts = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delEmployeeAdmin":
                                lstAccountProperties.DelEmployeeAdmin = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delReports":
                                lstAccountProperties.DelReports = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delReportsClaimants":
                                lstAccountProperties.DelReportsClaimants = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delCheckAndPay":
                                lstAccountProperties.DelCheckAndPay = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delQEDesign":
                                lstAccountProperties.DelQEDesign = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delCorporateCards":
                                lstAccountProperties.DelCorporateCards = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            //case "delPurchaseCards":
                            //    lstAccountProperties.DelPurchaseCards = Convert.ToBoolean(Convert.ToByte(stringValue));
                            //    break;
                            case "delApprovals":
                                lstAccountProperties.DelApprovals = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delSubmitClaims":
                                lstAccountProperties.DelSubmitClaim = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delExports":
                                lstAccountProperties.DelExports = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "delAuditLog":
                                lstAccountProperties.DelAuditLog = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "sendReviewRequests":
                                lstAccountProperties.SendReviewRequests = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "claimantDeclaration":
                                lstAccountProperties.ClaimantDeclaration = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "declarationMessage":
                                lstAccountProperties.DeclarationMsg = stringValue;
                                break;
                            case "approverDeclarationMessage":
                                lstAccountProperties.ApproverDeclarationMsg = stringValue;
                                break;
                            //case "addCompanies":
                            //    lstAccountProperties.AddCompanies = Convert.ToBoolean(Convert.ToByte(stringValue));
                            //    break;
                            case "allowMultipleDestinations":
                                lstAccountProperties.AllowMultipleDestinations = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useMapPoint":
                                lstAccountProperties.UseMapPoint = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useCostCodeOnGenDetails":
                                lstAccountProperties.UseCostCodeOnGenDetails = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useDepartmentOnGenDetails":
                                lstAccountProperties.UseDeptOnGenDetails = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useProjectCodeOnGenDetails":
                                lstAccountProperties.UseProjectCodeOnGenDetails = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "homeToOffice":
                                lstAccountProperties.HomeToOffice = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "calcHomeToLocation":
                                lstAccountProperties.CalcHomeToLocation = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "showMileageCatsForUsers":
                                lstAccountProperties.ShowMileageCatsForUsers = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "activateCarOnUserAdd":
                                lstAccountProperties.ActivateCarOnUserAdd = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "autoCalcHomeToLocation":
                                lstAccountProperties.AutoCalcHomeToLocation = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowUsersToAddCars":
                                lstAccountProperties.AllowUsersToAddCars = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "mileageCalcType":
                                lstAccountProperties.MileageCalcType = Convert.ToByte(stringValue);
                                break;
                            case "productFieldType":
                                lstAccountProperties.ProductFieldType = (FieldType)Convert.ToByte(stringValue);
                                break;
                            case "supplierFieldType":
                                lstAccountProperties.SupplierFieldType = (FieldType)Convert.ToByte(stringValue);
                                break;
                            case "poNumberName":
                                lstAccountProperties.PONumberName = stringValue;
                                break;
                            case "poNumberGenerate":
                                lstAccountProperties.PONumberGenerate = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "poNumberSequence":
                                lstAccountProperties.PONumberSequence = Convert.ToInt32(stringValue);
                                break;
                            case "poNumberFormat":
                                lstAccountProperties.PONumberFormat = stringValue;
                                break;
                            case "dateApprovedName":
                                lstAccountProperties.DateApprovedName = stringValue;
                                break;
                            case "totalName":
                                lstAccountProperties.TotalName = stringValue;
                                break;
                            case "orderRecurrenceName":
                                lstAccountProperties.OrderRecurrenceName = stringValue;
                                break;
                            case "orderEndDateName":
                                lstAccountProperties.OrderEndDateName = stringValue;
                                break;
                            case "commentsName":
                                lstAccountProperties.CommentsName = stringValue;
                                break;
                            case "productName":
                                lstAccountProperties.ProductName = stringValue;
                                break;
                            case "countryName":
                                lstAccountProperties.CountryName = stringValue;
                                break;
                            case "currencyName":
                                lstAccountProperties.CurrencyName = stringValue;
                                break;
                            case "exchangeRateName":
                                lstAccountProperties.ExchangeRateName = stringValue;
                                break;
                            case "allowRecurring":
                                lstAccountProperties.AllowRecurring = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "autoArchiveType":
                                lstAccountProperties.AutoArchiveType = (AutoArchiveType)Convert.ToByte(stringValue);
                                break;
                            case "autoActivateType":
                                lstAccountProperties.AutoActivateType = (AutoActivateType)Convert.ToByte(stringValue);
                                break;
                            case "archiveGracePeriod":
                                lstAccountProperties.ArchiveGracePeriod = Convert.ToInt16(stringValue);
                                break;
                            case "importUsernameFormat":
                                lstAccountProperties.ImportUsernameFormat = stringValue;
                                break;
                            case "ImportHomeAddressFormat":
                                lstAccountProperties.ImportHomeAddressFormat = stringValue;
                                break;
                            case "globalLocaleID":
                                lstAccountProperties.GlobalLocaleID = Convert.ToInt32(stringValue);
                                break;
                            case "rechargeReferenceAs":
                                rsRefAs = stringValue;
                                break;
                            case "employeeReferenceAs":
                                rsEmpRefAs = stringValue;
                                break;
                            case "rechargePeriod":
                                rsRechargePd = Convert.ToInt32(stringValue);
                                break;
                            case "financialYearCommences":
                                rsFinYear = Convert.ToInt32(stringValue);
                                break;
                            case "cpDeleteAction":
                                rsCPAction = Convert.ToInt32(stringValue);
                                break;
                            case "taskStartDateMandatory":
                                lstAccountProperties.TaskStartDateMandatory = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "taskEndDateMandatory":
                                lstAccountProperties.TaskEndDateMandatory = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "taskDueDateMandatory":
                                lstAccountProperties.TaskDueDateMandatory = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "autoUpdateLicenceTotal":
                                lstAccountProperties.AutoUpdateLicenceTotal = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "contractCategoryTitle":
                                lstAccountProperties.ContractCategoryTitle = stringValue;
                                break;
                            case "inflatorActive":
                                lstAccountProperties.InflatorActive = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "invoiceFrequencyActive":
                                lstAccountProperties.InvoiceFreqActive = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "termTypeActive":
                                lstAccountProperties.TermTypeActive = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "valueComments":
                                lstAccountProperties.ValueComments = stringValue;
                                break;
                            case "contractDescTitle":
                                lstAccountProperties.ContractDescTitle = stringValue;
                                break;
                            case "contractDescShortTitle":
                                lstAccountProperties.ContractDescShortTitle = stringValue;
                                break;
                            case "contractNumGen":
                                lstAccountProperties.ContractNumGen = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "contractNumSeq":
                                lstAccountProperties.ContractNumSeq = Convert.ToInt32(stringValue);
                                break;
                            case "supplierStatusEnforced":
                                lstAccountProperties.SupplierStatusEnforced = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "contractCategoryMandatory":
                                lstAccountProperties.ContractCatMandatory = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "supplierLastFinStatusEnabled":
                                lstAccountProperties.SupplierLastFinStatusEnabled = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "supplierLastFinCheckEnabled":
                                lstAccountProperties.SupplierLastFinCheckEnabled = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "supplierFYEEnabled":
                                lstAccountProperties.SupplierFYEEnabled = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "supplierNumEmployeesEnabled":
                                lstAccountProperties.SupplierNumEmployeesEnabled = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "supplierTurnoverEnabled":
                                lstAccountProperties.SupplierTurnoverEnabled = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "supplierIntContactEnabled":
                                lstAccountProperties.SupplierIntContactEnabled = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "supplierCategoryTitle":
                                lstAccountProperties.SupplierCatTitle = stringValue;
                                break;
                            case "openSaveAttachments":
                                lstAccountProperties.OpenSaveAttachments = Convert.ToByte(stringValue);
                                break;
                            case "hyperlinkAttachmentsEnabled":
                                lstAccountProperties.EnableAttachmentHyperlink = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "attachmentUploadEnabled":
                                lstAccountProperties.EnableAttachmentUpload = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "cacheTimeout":
                                lstAccountProperties.CacheTimeout = Convert.ToInt32(stringValue);
                                break;
                            case "flashingNotesIconEnabled":
                                lstAccountProperties.EnableFlashingNotesIcon = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "contractNumUpdateEnabled":
                                lstAccountProperties.EnableContractNumUpdate = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "FYStarts":
                                lstAccountProperties.FYStarts = stringValue;
                                break;
                            case "FYEnds":
                                lstAccountProperties.FYEnds = stringValue;
                                break;
                            case "rechargeUnrecoveredTitle":
                                lstAccountProperties.RechargeUnrecoveredTitle = stringValue;
                                break;
                            case "supplierRegionTitle":
                                lstAccountProperties.SupplierRegionTitle = stringValue;
                                break;
                            case "supplierPrimaryTitle":
                                lstAccountProperties.SupplierPrimaryTitle = stringValue;
                                break;
                            case "supplierVariationTitle":
                                lstAccountProperties.SupplierVariationTitle = stringValue;
                                break;
                            case "taskEscalationRepeat":
                                lstAccountProperties.TaskEscalationRepeat = Convert.ToInt32(stringValue);
                                break;
                            case "autoUpdateCVRechargeLive":
                                lstAccountProperties.AutoUpdateCVRechargeLive = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "linkAttachmentDefault":
                                lstAccountProperties.LinkAttachmentDefault = Convert.ToByte(stringValue);
                                break;
                            case "penaltyClauseTitle":
                                lstAccountProperties.PenaltyClauseTitle = stringValue;
                                break;
                            case "contractScheduleDefault":
                                lstAccountProperties.ContractScheduleDefault = stringValue;
                                break;
                            case "variationAutoSeqEnabled":
                                lstAccountProperties.EnableVariationAutoSeq = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "maxUploadSize":
                                lstAccountProperties.MaxUploadSize = Convert.ToInt32(stringValue);
                                break;
                            case "useCPExtraInfo":
                                lstAccountProperties.UseCPExtraInfo = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "supplierCategoryMandatory":
                                lstAccountProperties.SupplierCatMandatory = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "errorEmailSubmitAddress":
                                lstAccountProperties.ErrorEmailAddress = stringValue;
                                break;
                            case "errorEmailSubmitFromAddress":
                                lstAccountProperties.ErrorEmailFromAddress = stringValue;
                                break;
                            case "recordOdometer":
                                lstAccountProperties.RecordOdometer = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "enableRecharge":
                                lstAccountProperties.EnableRecharge = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowTeamMemberToApproveOwnClaim":
                                lstAccountProperties.AllowTeamMemberToApproveOwnClaim = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "contractDatesMandatory":
                                lstAccountProperties.ContractDatesMandatory = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "logoPath":
                                lstAccountProperties.LogoPath = stringValue;
                                break;
                            case "emailAdministrator":
                                lstAccountProperties.EmailAdministrator = stringValue;
                                break;
                            case "cachePeriodShort":
                                lstAccountProperties.CachePeriodShort = Convert.ToInt32(stringValue);
                                break;
                            case "cachePeriodNormal":
                                lstAccountProperties.CachePeriodNormal = Convert.ToInt32(stringValue);
                                break;
                            case "cachePeriodLong":
                                lstAccountProperties.CachePeriodLong = Convert.ToInt32(stringValue);
                                break;
                            case "customerHelpInformation":
                                lstAccountProperties.CustomerHelpInformation = stringValue;
                                break;
                            case "customerHelpContactName":
                                lstAccountProperties.CustomerHelpContactName = stringValue;
                                break;
                            case "customerHelpContactTelephone":
                                lstAccountProperties.CustomerHelpContactTelephone = stringValue;
                                break;
                            case "customerHelpContactFax":
                                lstAccountProperties.CustomerHelpContactFax = stringValue;
                                break;
                            case "customerHelpContactAddress":
                                lstAccountProperties.CustomerHelpContactAddress = stringValue;
                                break;
                            case "customerHelpContactEmailAddress":
                                lstAccountProperties.CustomerHelpContactEmailAddress = stringValue;
                                break;
                            case "mandatoryPostcodeForAddresses":
                                lstAccountProperties.MandatoryPostcodeForAddresses = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "ClaimantsCanAddCompanyLocations":
                                lstAccountProperties.ClaimantsCanAddCompanyLocations = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "CheckESRAssignmentOnEmployeeSave":
                                lstAccountProperties.CheckESRAssignmentOnEmployeeAdd = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "coloursHeaderBackground":
                                lstAccountProperties.ColoursHeaderBackground = stringValue;
                                break;
                            case "coloursHeaderBreadcrumbText":
                                lstAccountProperties.ColoursHeaderBreadcrumbText = stringValue;
                                break;
                            case "coloursPageTitleText":
                                lstAccountProperties.ColoursPageTitleText = stringValue;
                                break;
                            case "coloursSectionHeadingUnderline":
                                lstAccountProperties.ColoursSectionHeadingUnderline = stringValue;
                                break;
                            case "coloursSectionHeadingText":
                                lstAccountProperties.ColoursSectionHeadingText = stringValue;
                                break;
                            case "coloursFieldText":
                                lstAccountProperties.ColoursFieldText = stringValue;
                                break;
                            case "coloursPageOptionsBackground":
                                lstAccountProperties.ColoursPageOptionsBackground = stringValue;
                                break;
                            case "coloursPageOptionsText":
                                lstAccountProperties.ColoursPageOptionsText = stringValue;
                                break;
                            case "coloursTableHeaderBackground":
                                lstAccountProperties.ColoursTableHeaderBackground = stringValue;
                                break;
                            case "coloursTableHeaderText":
                                lstAccountProperties.ColoursTableHeaderText = stringValue;
                                break;
                            case "coloursTabOptionBackground":
                                lstAccountProperties.ColoursTabOptionBackground = stringValue;
                                break;
                            case "coloursTabOptionText":
                                lstAccountProperties.ColoursTabOptionText = stringValue;
                                break;
                            case "coloursRowBackground":
                                lstAccountProperties.ColoursRowBackground = stringValue;
                                break;
                            case "coloursRowText":
                                lstAccountProperties.ColoursRowText = stringValue;
                                break;
                            case "coloursAlternateRowBackground":
                                lstAccountProperties.ColoursAlternateRowBackground = stringValue;
                                break;
                            case "coloursAlternateRowText":
                                lstAccountProperties.ColoursAlternateRowText = stringValue;
                                break;
                            case "coloursMenuOptionHoverText":
                                lstAccountProperties.ColoursMenuOptionHoverText = stringValue;
                                break;
                            case "coloursMenuOptionStandardText":
                                lstAccountProperties.ColoursMenuOptionStandardText = stringValue;
                                break;
                            case "coloursTooltipBackground":
                                lstAccountProperties.ColoursTooltipBackground = stringValue;
                                break;
                            case "coloursTooltipText":
                                lstAccountProperties.ColoursTooltipText = stringValue;
                                break;
                            case "coloursGreenLightField":
                                lstAccountProperties.ColoursGreenLightField = stringValue;
                                break;
                            case "coloursGreenLightSectionText":
                                lstAccountProperties.ColoursGreenLightSectionText = stringValue;
                                break;
                            case "coloursGreenLightSectionBackground":
                                lstAccountProperties.ColoursGreenLightSectionBackground = stringValue;
                                break;
                            case "coloursGreenLightSectionUnderline":
                                lstAccountProperties.ColoursGreenLightSectionUnderline = stringValue;
                                break;
                            case "AllowEmployeeInOwnSignoffGroup":
                                lstAccountProperties.AllowEmployeeInOwnSignoffGroup = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "useMobileDevices":
                                lstAccountProperties.UseMobileDevices = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowEmployeeToSpecifyCarStartDateOnAdd":
                                lstAccountProperties.AllowEmpToSpecifyCarStartDateOnAdd = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowEmployeeToSpecifyCarDOCOnAdd":
                                lstAccountProperties.AllowEmpToSpecifyCarDOCOnAdd = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowEmployeeToSpecifyCarStartDateOnAddMandatory":
                                lstAccountProperties.EmpToSpecifyCarStartDateOnAddMandatory = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "enableESRDiagnostics":
                                lstAccountProperties.EnableEsrDiagnostics = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "allowEmployeeToNotifyOfChangeOfDetails":
                                lstAccountProperties.AllowEmployeeToNotifyOfChangeOfDetails = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "corporateDiligenceStartPage":
                                lstAccountProperties.CorporateDStartPage = stringValue;
                                break;
                            case "defaultCostCodeOwner":
                                lstAccountProperties.CostCodeOwnerBaseKey = stringValue;
                                break;
                            case "includeAssignmentDetails":
                                lstAccountProperties.IncludeAssignmentDetails = (IncludeEsrDetails)Convert.ToByte(stringValue);
                                break;
                            case "esrAutoActivateCar":
                                lstAccountProperties.EsrAutoActivateCar = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "singleSignOnLookUpField":
                                if (string.IsNullOrEmpty(stringValue))
                                {
                                    lstAccountProperties.SingleSignOnLookupField = null;
                                }
                                else
                                {
                                    lstAccountProperties.SingleSignOnLookupField = new Guid(stringValue);
                                }
                                break;
                            case "NumberOfApproversToRememberForClaimantInApprovalMatrixClaim":
                                lstAccountProperties.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim =
                                    Convert.ToInt32(stringValue);
                                break;
                            case "DisableCarOutsideOfStartEndDate":
                                lstAccountProperties.DisableCarOutsideOfStartEndDate = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "retainLabelsTime":
                                lstAccountProperties.RetainLabelsTime = stringValue;
                                break;
                            case "showFullHomeAddress":
                                lstAccountProperties.ShowFullHomeAddressOnClaims = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "homeAddressKeyword":
                                lstAccountProperties.HomeAddressKeyword = stringValue;
                                break;
                            case "workAddressKeyword":
                                lstAccountProperties.WorkAddressKeyword = stringValue;
                                break;
                            case "forceAddressNameEntry":
                                lstAccountProperties.ForceAddressNameEntry = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "addressNameEntryMessage":
                                lstAccountProperties.AddressNameEntryMessage = stringValue;
                                break;
                            case "idleTimeout":
                                lstAccountProperties.IdleTimeout = Convert.ToInt32(stringValue);
                                break;
                            case "countdownTimer":
                                lstAccountProperties.CountdownTimer = Convert.ToInt32(stringValue);
                                break;
                            case "EnableInternalSupportTickets":
                                lstAccountProperties.EnableInternalSupportTickets = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "DisplayEsrAddressesInSearchResults":
                                lstAccountProperties.DisplayEsrAddressesInSearchResults = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "editPreviousClaims":
                                lstAccountProperties.EditPreviousClaims = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "NotifyWhenEnvelopeNotReceivedDays":
                                lstAccountProperties.NotifyWhenEnvelopeNotReceivedDays = Convert.ToInt32(Convert.ToByte(stringValue));
                                break;
                            case "allowViewFundDetails":
                                lstAccountProperties.AllowViewFundDetails = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "dutyOfCareEmailReminderForClaimantDays":
                                lstAccountProperties.RemindClaimantOnDOCDocumentExpiryDays = Convert.ToInt32(stringValue);
                                break;
                            case "dutyOfCareEmailReminderForApproverDays":
                                lstAccountProperties.RemindApproverOnDOCDocumentExpiryDays = Convert.ToInt32(stringValue);
                                break;
                            case "dutyOfCareTeamAsApprover":
                                lstAccountProperties.DutyOfCareTeamAsApprover = stringValue;
                                break;
                            case "dutyOfCareApprover":
                                lstAccountProperties.DutyOfCareApprover = stringValue;
                                break;
                            //Expedite Tab - Validation Options
                            case "allowExpenseItemsLessThanTheReceiptTotalToPassValidation":
                                lstAccountProperties.AllowReceiptTotalToPassValidation = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "SummaryESRInboundFile":
                                lstAccountProperties.SummaryEsrInboundFile = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "EsrRounding":
                                lstAccountProperties.EsrRounding = (EsrRoundingType)Convert.ToByte(stringValue);
                                break;
                            case "delOptionsForDelegateAccessRole":
                                lstAccountProperties.EnableDelegateOptionsForDelegateAccessRole = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "EnableClaimApprovalReminders":
                                lstAccountProperties.EnableClaimApprovalReminders = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "ClaimApprovalReminderFrequency":
                                lstAccountProperties.ClaimApprovalReminderFrequency = Convert.ToInt32(stringValue);
                                break;

                            case "EnableCurrentClaimsReminders":
                                lstAccountProperties.EnableCurrentClaimsReminders = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "CurrentClaimsReminderFrequency":
                                lstAccountProperties.CurrentClaimsReminderFrequency = Convert.ToInt32(stringValue);
                                break;

                            case "ESRManualSupervisorAssignment":
                                lstAccountProperties.EnableESRManualAssignmentSupervisor = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "enableCalculationsForAllocatingFuelReceiptVATtoMileage":
                                lstAccountProperties.EnableCalculationsForAllocatingFuelReceiptVatToMileage = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "MultipleWorkAddress":
                                lstAccountProperties.MultipleWorkAddress =
                                    Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "DrivingLicenceReviewPeriodically":
                                lstAccountProperties.EnableDrivingLicenceReview = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "DrivingLicenceReviewFrequency":
                                lstAccountProperties.DrivingLicenceReviewFrequency = Convert.ToInt32(stringValue);
                                break;
                            case "DrivingLicenceReviewReminder":
                                lstAccountProperties.DrivingLicenceReviewReminder = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "DrivingLicenceReviewReminderDays":
                                lstAccountProperties.DrivingLicenceReviewReminderDays = Convert.ToByte(stringValue);
                                break;
                            case "EnableAutoUpdateOfExchangeRates":
                                lstAccountProperties.EnableAutoUpdateOfExchangeRates = Convert.ToBoolean(Convert.ToByte(stringValue));
                                break;
                            case "EnableAutoUpdateOfExchangeRatesActivatedDate":
                                if (stringValue == string.Empty)
                                {
                                    lstAccountProperties.EnableAutoUpdateOfExchangeRatesActivatedDate = null;
                                }
                                else
                                {
                                    try
                                    {
                                        lstAccountProperties.EnableAutoUpdateOfExchangeRatesActivatedDate = Convert.ToDateTime(stringValue);
                                    }
                                    catch (Exception)
                                    {
                                        lstAccountProperties.EnableAutoUpdateOfExchangeRatesActivatedDate = null;
                                    }

                                }

                                break;
                            case "ExchangeRateProvider":
                                lstAccountProperties.ExchangeRateProvider = (ExchangeRateProvider)Convert.ToInt32(stringValue);
                                break;
                            case "AccountCurrentlyLockedMessage":
                                lstAccountProperties.AccountCurrentlyLockedMessage = stringValue;
                                break;
                            case "AccountLockedMessage":
                                lstAccountProperties.AccountLockedMessage = stringValue;
                                break;
                            case "esrPrimaryAddressOnly":
                                lstAccountProperties.EsrPrimaryAddressOnly = Convert.ToBoolean(Convert.ToInt32(stringValue));
                                break;
                            case "BlockUnmachedExpenseItemsBeingSubmitted":
                                lstAccountProperties.BlockUnmachedExpenseItemsBeingSubmitted = Convert.ToBoolean(Convert.ToInt32(stringValue));
                                break;
                            case "PopulateDocumentsFromVehicleLookup":
                                lstAccountProperties.PopulateDocumentsFromVehicleLookup = Convert.ToBoolean(Convert.ToInt32(stringValue));
                                break;

                        }
                    }

                    reader.Close();
                }
            }

            lstAccountProperties.RechargeSettings = new cRechargeSetting(rsRefAs, rsEmpRefAs, rsRechargePd, rsFinYear, rsCPAction);

            return lstAccountProperties;
        }

        /// <summary>
        /// Gets drop down list elements containing available sub-accounts
        /// </summary>
        /// <param name="subAccountId">Current Sub-Account selection. NULL if none.</param>
        /// <returns></returns>
        public System.Web.UI.WebControls.ListItem[] CreateDropDown(int? subAccountId)
        {
            System.Web.UI.WebControls.ListItem[] tempitems = new System.Web.UI.WebControls.ListItem[_lstSubAccounts.Count];
            SortedList<string, int> subaccs = new SortedList<string, int>();

            foreach (KeyValuePair<int, cAccountSubAccount> kvp in _lstSubAccounts)
            {
                cAccountSubAccount sa = (cAccountSubAccount)kvp.Value;
                subaccs.Add(sa.Description, sa.SubAccountID);
            }

            int idx = 0;
            foreach (KeyValuePair<string, int> subaccounts in subaccs)
            {
                tempitems[idx] = new System.Web.UI.WebControls.ListItem();
                tempitems[idx].Text = subaccounts.Key;
                tempitems[idx].Value = subaccounts.Value.ToString();
                if (subAccountId.HasValue)
                {
                    if (subaccounts.Value == subAccountId.Value)
                    {
                        tempitems[idx].Selected = true;
                    }
                }
                idx++;
            }

            return tempitems;
        }

        /// <summary>
        /// Get the dataset for the sub accounts modal and show the sub accounts where the employee is associated only
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="CurrentSubAccountID"></param>
        /// <returns></returns>
        protected DataSet getEmployeeSubAccountDataset(int employeeID, int CurrentSubAccountID, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@EmployeeID", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CurrentSubAccountID", CurrentSubAccountID);
            DataSet ds = expdata.GetProcDataSet("GetEmployeeSubAccounts");
            expdata.sqlexecute.Parameters.Clear();
            return ds;
        }

        /// <summary>
        /// Updates the cacheExpiry field in db and unloads cached data
        /// </summary>
        /// <param name="subAccountId">SubAccountID modified</param>
        public void InvalidateCache(int subAccountId, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            expdata.sqlexecute.Parameters.AddWithValue("@subAccId", subAccountId);
            expdata.ExecuteSQL("update accountsSubAccounts set CacheExpiry = getdate() where subAccountID = @subAccId");
            expdata.sqlexecute.Parameters.Clear();

            var tmpList = (SortedList<int, cAccountSubAccount>)_cache.Get(this._accountId, string.Empty, this._cacheKey);

            if (tmpList != null)
            {
                _cache.Delete(this._accountId, string.Empty, this._cacheKey);
            }
        }

        /// <summary>
        /// (UNIT TESTS ONLY) Updates sub-account record in the database
        /// </summary>
        /// <param name="subaccount">Subaccount record to save</param>
        /// <param name="employeeId">Employee </param>
        /// <param name="associatedSubAccountID"></param>
        /// <param name="accountId"></param>
        /// <param name="currentSubAccountId"></param>
        /// <returns>ID of the subaccount</returns>
        public int UpdateSubAccount(cAccountSubAccount subaccount, int employeeId, int associatedSubAccountID, int accountId, int currentSubAccountId, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@ID", subaccount.SubAccountID);
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", currentSubAccountId);
            expdata.sqlexecute.Parameters.AddWithValue("@description", subaccount.Description);
            expdata.sqlexecute.Parameters.AddWithValue("@associatedSubAccountID", associatedSubAccountID);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@delegateId", DBNull.Value);
            expdata.sqlexecute.Parameters.Add("@returnID", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@returnID"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveSubAccount");
            int retID = (int)expdata.sqlexecute.Parameters["@returnID"].Value;
            expdata.sqlexecute.Parameters.Clear();

            InvalidateCache(subaccount.SubAccountID);

            return retID;
        }

        /// <summary>
        /// (UNIT TESTS ONLY) Delete a subaccount definition
        /// </summary>
        /// <param name="subAccountId">SubAccountID to delete</param>
        /// <param name="employeeId">EmployeeId making the delete request</param>
        /// <returns>ID of the subaccount</returns>
        public int DeleteSubAccount(int subAccountId, int employeeId, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            // USED ONLY FOR UNIT TESTS AT PRESENT - NEEDS MANY REF CHECKS FOR USE IN APPLICATION
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", subAccountId);
            expdata.ExecuteSQL("delete from auditLog where subAccountId = @subAccountId");
            expdata.ExecuteSQL("delete from [accountProperties] where subAccountId = @subAccountId");
            expdata.ExecuteSQL("delete from [employeeAccessRoles] where subAccountID = @subAccountId");
            expdata.sqlexecute.Parameters.Clear();

            expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", subAccountId);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            expdata.ExecuteProc("deleteSubAccount");
            expdata.sqlexecute.Parameters.Clear();

            InvalidateCache(subAccountId);

            return subAccountId;
        }
    }
}
