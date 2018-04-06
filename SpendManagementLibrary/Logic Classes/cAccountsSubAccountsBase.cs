using System;
using SpendManagementLibrary.Logic_Classes;


namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using Utilities.DistributedCaching;

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
        /// Save subaccount properties to the database
        /// </summary>
        /// <param name="subAccountProperties">Properties collection</param>
        /// <param name="employeeID">EmployeeID performing the modification</param>
        /// <param name="delegateID">EmployeeID of user if acting as a delegate</param>
        public void SaveAccountProperties(cAccountProperties subAccountProperties, int employeeID, int? delegateID)
        {
            cAccountProperties originalSubAccountProperties = getSubAccountById(subAccountProperties.SubAccountID).SubAccountProperties;
            int subAccountID = subAccountProperties.SubAccountID;            

            if (originalSubAccountProperties.ActivateCarOnUserAdd != subAccountProperties.ActivateCarOnUserAdd)
            {
                SaveAccountProperty(subAccountID, "activateCarOnUserAdd", subAccountProperties.ActivateCarOnUserAdd, employeeID, delegateID);
            }

            //if (originalSubAccountProperties.AddCompanies != subAccountProperties.AddCompanies)
            //{
            //    SaveAccountProperty(subAccountID, "addCompanies", subAccountProperties.AddCompanies, EmployeeID);
            //}

            if (originalSubAccountProperties.AddLocations != subAccountProperties.AddLocations)
            {
                SaveAccountProperty(subAccountID, "addLocations", subAccountProperties.AddLocations, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowArchivedNotesAdd != subAccountProperties.AllowArchivedNotesAdd)
            {
                SaveAccountProperty(subAccountID, "allowArchivedNotesAdd", subAccountProperties.AllowArchivedNotesAdd, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowMenuContractAdd != subAccountProperties.AllowMenuContractAdd)
            {
                SaveAccountProperty(subAccountID, "allowMenuAddContract", subAccountProperties.AllowMenuContractAdd, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowMultipleDestinations != subAccountProperties.AllowMultipleDestinations)
            {
                SaveAccountProperty(subAccountID, "allowMultipleDestinations", subAccountProperties.AllowMultipleDestinations, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowRecurring != subAccountProperties.AllowRecurring)
            {
                SaveAccountProperty(subAccountID, "allowRecurring", subAccountProperties.AllowRecurring, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfReg != subAccountProperties.AllowSelfReg)
            {
                SaveAccountProperty(subAccountID, "allowSelfReg", subAccountProperties.AllowSelfReg, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfRegAdvancesSignOff != subAccountProperties.AllowSelfRegAdvancesSignOff)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegAdvancesSignOff", subAccountProperties.AllowSelfRegAdvancesSignOff, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfRegBankDetails != subAccountProperties.AllowSelfRegBankDetails)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegBankDetails", subAccountProperties.AllowSelfRegBankDetails, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfRegCarDetails != subAccountProperties.AllowSelfRegCarDetails)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegCarDetails", subAccountProperties.AllowSelfRegCarDetails, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfRegDepartmentCostCode != subAccountProperties.AllowSelfRegDepartmentCostCode)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegDeptCostCode", subAccountProperties.AllowSelfRegDepartmentCostCode, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfRegEmployeeContact != subAccountProperties.AllowSelfRegEmployeeContact)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegEmpContact", subAccountProperties.AllowSelfRegEmployeeContact, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfRegEmployeeInfo != subAccountProperties.AllowSelfRegEmployeeInfo)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegEmpInfo", subAccountProperties.AllowSelfRegEmployeeInfo, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfRegHomeAddress != subAccountProperties.AllowSelfRegHomeAddress)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegHomeAddress", subAccountProperties.AllowSelfRegHomeAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfRegRole != subAccountProperties.AllowSelfRegRole)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegRole", subAccountProperties.AllowSelfRegRole, employeeID, delegateID);
            }
            if (originalSubAccountProperties.AllowSelfRegItemRole != subAccountProperties.AllowSelfRegItemRole)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegItemRole", subAccountProperties.AllowSelfRegItemRole, employeeID, delegateID);
            }
            if (originalSubAccountProperties.AllowSelfRegSignOff != subAccountProperties.AllowSelfRegSignOff)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegSignOff", subAccountProperties.AllowSelfRegSignOff, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowSelfRegUDF != subAccountProperties.AllowSelfRegUDF)
            {
                SaveAccountProperty(subAccountID, "allowSelfRegUDF", subAccountProperties.AllowSelfRegUDF, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowTeamMemberToApproveOwnClaim != subAccountProperties.AllowTeamMemberToApproveOwnClaim)
            {
                SaveAccountProperty(subAccountID, "allowTeamMemberToApproveOwnClaim", subAccountProperties.AllowTeamMemberToApproveOwnClaim, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowUsersToAddCars != subAccountProperties.AllowUsersToAddCars)
            {
                SaveAccountProperty(subAccountID, "allowUsersToAddCars", subAccountProperties.AllowUsersToAddCars, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ApplicationURL != subAccountProperties.ApplicationURL)
            {
                SaveAccountProperty(subAccountID, "applicationURL", subAccountProperties.ApplicationURL, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ApproverDeclarationMsg != subAccountProperties.ApproverDeclarationMsg)
            {
                SaveAccountProperty(subAccountID, "approverDeclarationMessage", subAccountProperties.ApproverDeclarationMsg, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ArchiveGracePeriod != subAccountProperties.ArchiveGracePeriod)
            {
                SaveAccountProperty(subAccountID, "archiveGracePeriod", subAccountProperties.ArchiveGracePeriod, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AttachReceipts != subAccountProperties.AttachReceipts)
            {
                SaveAccountProperty(subAccountID, "attachReceipts", subAccountProperties.AttachReceipts, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AuditorEmailAddress != subAccountProperties.AuditorEmailAddress)
            {
                SaveAccountProperty(subAccountID, "auditorEmailAddress", subAccountProperties.AuditorEmailAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AutoActivateType != subAccountProperties.AutoActivateType)
            {
                SaveAccountProperty(subAccountID, "autoActivateType", (int)subAccountProperties.AutoActivateType, employeeID, delegateID); // enumerator
            }

            if (originalSubAccountProperties.AutoArchiveType != subAccountProperties.AutoArchiveType)
            {
                SaveAccountProperty(subAccountID, "autoArchiveType", (int)subAccountProperties.AutoArchiveType, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AutoAssignAllocation != subAccountProperties.AutoAssignAllocation)
            {
                SaveAccountProperty(subAccountID, "autoAssignAllocation", subAccountProperties.AutoAssignAllocation, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AutoCalcHomeToLocation != subAccountProperties.AutoCalcHomeToLocation)
            {
                SaveAccountProperty(subAccountID, "autoCalcHomeToLocation", subAccountProperties.AutoCalcHomeToLocation, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AutoUpdateAnnualContractValue != subAccountProperties.AutoUpdateAnnualContractValue)
            {
                SaveAccountProperty(subAccountID, "autoUpdateCV", subAccountProperties.AutoUpdateAnnualContractValue, employeeID, delegateID);
            }

            if (originalSubAccountProperties.BaseCurrency != subAccountProperties.BaseCurrency)
            {
                SaveAccountProperty(subAccountID, "baseCurrency", subAccountProperties.BaseCurrency, employeeID, delegateID);
            }

            if (originalSubAccountProperties.BlockCashCC != subAccountProperties.BlockCashCC)
            {
                SaveAccountProperty(subAccountID, "blockCashCC", subAccountProperties.BlockCashCC, employeeID, delegateID);
            }

            if (originalSubAccountProperties.BlockCashPC != subAccountProperties.BlockCashPC)
            {
                SaveAccountProperty(subAccountID, "blockCashPC", subAccountProperties.BlockCashPC, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableAutomaticDrivingLicenceLookup != subAccountProperties.EnableAutomaticDrivingLicenceLookup)
            {
                SaveAccountProperty(subAccountID, "enableAutomaticDrivingLicenceLookup", subAccountProperties.EnableAutomaticDrivingLicenceLookup, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DrivingLicenceLookupFrequency != subAccountProperties.DrivingLicenceLookupFrequency)
            {
                SaveAccountProperty(subAccountID, "drivingLicenceLookupFrequency", subAccountProperties.DrivingLicenceLookupFrequency, employeeID, delegateID);
            }

            if (originalSubAccountProperties.BlockInsuranceExpiry != subAccountProperties.BlockInsuranceExpiry)
            {
                SaveAccountProperty(subAccountID, "blockInsuranceExpiry", subAccountProperties.BlockInsuranceExpiry, employeeID, delegateID);
            }

            if (originalSubAccountProperties.BlockDrivingLicence != subAccountProperties.BlockDrivingLicence)
            {
                SaveAccountProperty(subAccountID, "blockDrivingLicence", subAccountProperties.BlockDrivingLicence, employeeID, delegateID);
            }

            if (originalSubAccountProperties.BlockMOTExpiry != subAccountProperties.BlockMOTExpiry)
            {
                SaveAccountProperty(subAccountID, "blockMOTExpiry", subAccountProperties.BlockMOTExpiry, employeeID, delegateID);
            }

            if (originalSubAccountProperties.BlockTaxExpiry != subAccountProperties.BlockTaxExpiry)
            {
                SaveAccountProperty(subAccountID, "blockTaxExpiry", subAccountProperties.BlockTaxExpiry, employeeID, delegateID);
            }

            if (originalSubAccountProperties.BlockBreakdownCoverExpiry != subAccountProperties.BlockBreakdownCoverExpiry)
            {
                SaveAccountProperty(subAccountID, "blockBreakdownCoverExpiry", subAccountProperties.BlockBreakdownCoverExpiry, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseDateOfExpenseForDutyOfCareChecks != subAccountProperties.UseDateOfExpenseForDutyOfCareChecks)
            {
                SaveAccountProperty(subAccountID, "useDateOfExpenseForDutyOfCareChecks", subAccountProperties.UseDateOfExpenseForDutyOfCareChecks, employeeID, delegateID);
            }

            if (originalSubAccountProperties.Broadcast != subAccountProperties.Broadcast)
            {
                SaveAccountProperty(subAccountID, "broadcast", subAccountProperties.Broadcast, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CalcHomeToLocation != subAccountProperties.CalcHomeToLocation)
            {
                SaveAccountProperty(subAccountID, "calcHomeToLocation", subAccountProperties.CalcHomeToLocation, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CCAdmin != subAccountProperties.CCAdmin)
            {
                SaveAccountProperty(subAccountID, "ccAdmin", subAccountProperties.CCAdmin, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CCUserSettles != subAccountProperties.CCUserSettles)
            {
                SaveAccountProperty(subAccountID, "ccUserSettles", subAccountProperties.CCUserSettles, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CheckESRAssignmentOnEmployeeAdd != subAccountProperties.CheckESRAssignmentOnEmployeeAdd)
            {
                SaveAccountProperty(subAccountID, "CheckESRAssignmentOnEmployeeSave", subAccountProperties.CheckESRAssignmentOnEmployeeAdd, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ClaimantDeclaration != subAccountProperties.ClaimantDeclaration)
            {
                SaveAccountProperty(subAccountID, "claimantDeclaration", subAccountProperties.ClaimantDeclaration, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CommentsName != subAccountProperties.CommentsName)
            {
                SaveAccountProperty(subAccountID, "commentsName", subAccountProperties.CommentsName, employeeID, delegateID);
            }



            if (originalSubAccountProperties.ContractKey != subAccountProperties.ContractKey)
            {
                SaveAccountProperty(subAccountID, "contractKey", subAccountProperties.ContractKey, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CostCodesOn != subAccountProperties.CostCodesOn)
            {
                SaveAccountProperty(subAccountID, "costCodesOn", subAccountProperties.CostCodesOn, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CountryName != subAccountProperties.CountryName)
            {
                SaveAccountProperty(subAccountID, "countryName", subAccountProperties.CountryName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CurImportId != subAccountProperties.CurImportId)
            {
                SaveAccountProperty(subAccountID, "curImportId", subAccountProperties.CurImportId, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CurrencyName != subAccountProperties.CurrencyName)
            {
                SaveAccountProperty(subAccountID, "currencyName", subAccountProperties.CurrencyName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.currencyType != subAccountProperties.currencyType)
            {
                SaveAccountProperty(subAccountID, "currencyType", (int)subAccountProperties.currencyType, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DateApprovedName != subAccountProperties.DateApprovedName)
            {
                SaveAccountProperty(subAccountID, "dateApprovedName", subAccountProperties.DateApprovedName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DBVersion != subAccountProperties.DBVersion)
            {
                SaveAccountProperty(subAccountID, "dbVersion", subAccountProperties.DBVersion, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DeclarationMsg != subAccountProperties.DeclarationMsg)
            {
                SaveAccountProperty(subAccountID, "declarationMessage", subAccountProperties.DeclarationMsg, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DefaultPageSize != subAccountProperties.DefaultPageSize)
            {
                SaveAccountProperty(subAccountID, "defaultPageSize", subAccountProperties.DefaultPageSize, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DefaultRole != subAccountProperties.DefaultRole)
            {
                SaveAccountProperty(subAccountID, "defaultRole", subAccountProperties.DefaultRole, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DefaultItemRole != subAccountProperties.DefaultItemRole)
            {
                SaveAccountProperty(subAccountID, "defaultItemRole", subAccountProperties.DefaultItemRole, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelApprovals != subAccountProperties.DelApprovals)
            {
                SaveAccountProperty(subAccountID, "delApprovals", subAccountProperties.DelApprovals, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelAuditLog != subAccountProperties.DelAuditLog)
            {
                SaveAccountProperty(subAccountID, "delAuditLog", subAccountProperties.DelAuditLog, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelCheckAndPay != subAccountProperties.DelCheckAndPay)
            {
                SaveAccountProperty(subAccountID, "delCheckAndPay", subAccountProperties.DelCheckAndPay, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelCorporateCards != subAccountProperties.DelCorporateCards)
            {
                SaveAccountProperty(subAccountID, "delCorporateCards", subAccountProperties.DelCorporateCards, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelEmployeeAccounts != subAccountProperties.DelEmployeeAccounts)
            {
                SaveAccountProperty(subAccountID, "delEmployeeAccounts", subAccountProperties.DelEmployeeAccounts, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelEmployeeAdmin != subAccountProperties.DelEmployeeAdmin)
            {
                SaveAccountProperty(subAccountID, "delEmployeeAdmin", subAccountProperties.DelEmployeeAdmin, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelExports != subAccountProperties.DelExports)
            {
                SaveAccountProperty(subAccountID, "delExports", subAccountProperties.DelExports, employeeID, delegateID);
            }

            //if (originalSubAccountProperties.DelPurchaseCards != subAccountProperties.DelPurchaseCards)
            //{
            //    SaveAccountProperty(subAccountID, "delPurchaseCards", subAccountProperties.DelPurchaseCards, EmployeeID);
            //}

            if (originalSubAccountProperties.DelQEDesign != subAccountProperties.DelQEDesign)
            {
                SaveAccountProperty(subAccountID, "delQEDesign", subAccountProperties.DelQEDesign, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelReports != subAccountProperties.DelReports)
            {
                SaveAccountProperty(subAccountID, "delReports", subAccountProperties.DelReports, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelReportsClaimants != subAccountProperties.DelReportsClaimants)
            {
                SaveAccountProperty(subAccountID, "delReportsClaimants", subAccountProperties.DelReportsClaimants, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelSetup != subAccountProperties.DelSetup)
            {
                SaveAccountProperty(subAccountID, "delSetup", subAccountProperties.DelSetup, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DepartmentsOn != subAccountProperties.DepartmentsOn)
            {
                SaveAccountProperty(subAccountID, "departmentsOn", subAccountProperties.DepartmentsOn, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DisplayFlagAdded != subAccountProperties.DisplayFlagAdded)
            {
                SaveAccountProperty(subAccountID, "displayFlagAdded", subAccountProperties.DisplayFlagAdded, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DocumentRepository != subAccountProperties.DocumentRepository)
            {
                SaveAccountProperty(subAccountID, "documentRepository", subAccountProperties.DocumentRepository, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DrilldownReport != subAccountProperties.DrilldownReport)
            {
                SaveAccountProperty(subAccountID, "drilldownReport", subAccountProperties.DrilldownReport, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EditMyDetails != subAccountProperties.EditMyDetails)
            {
                SaveAccountProperty(subAccountID, "editMyDetails", subAccountProperties.EditMyDetails, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EmailServerAddress != subAccountProperties.EmailServerAddress)
            {
                SaveAccountProperty(subAccountID, "emailServerAddress", subAccountProperties.EmailServerAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EmailServerFromAddress != subAccountProperties.EmailServerFromAddress)
            {
                SaveAccountProperty(subAccountID, "emailServerFromAddress", subAccountProperties.EmailServerFromAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ErrorEmailAddress != subAccountProperties.ErrorEmailAddress)
            {
                SaveAccountProperty(subAccountID, "errorEmailSubmitAddress", subAccountProperties.ErrorEmailAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ErrorEmailFromAddress != subAccountProperties.ErrorEmailFromAddress)
            {
                SaveAccountProperty(subAccountID, "errorEmailSubmitFromAddress", subAccountProperties.ErrorEmailFromAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnterOdometerOnSubmit != subAccountProperties.EnterOdometerOnSubmit)
            {
                SaveAccountProperty(subAccountID, "enterOdometerOnSubmit", subAccountProperties.EnterOdometerOnSubmit, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ExchangeRateName != subAccountProperties.ExchangeRateName)
            {
                SaveAccountProperty(subAccountID, "exchangeRateName", subAccountProperties.ExchangeRateName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ExchangeReadOnly != subAccountProperties.ExchangeReadOnly)
            {
                SaveAccountProperty(subAccountID, "exchangeReadOnly", subAccountProperties.ExchangeReadOnly, employeeID, delegateID);
            }

            if (originalSubAccountProperties.FlagDate != subAccountProperties.FlagDate)
            {
                SaveAccountProperty(subAccountID, "flagDate", subAccountProperties.FlagDate, employeeID, delegateID);
            }

            if (originalSubAccountProperties.FlagMessage != subAccountProperties.FlagMessage)
            {
                SaveAccountProperty(subAccountID, "flagMessage", subAccountProperties.FlagMessage, employeeID, delegateID);
            }

            if (originalSubAccountProperties.FrequencyType != subAccountProperties.FrequencyType)
            {
                SaveAccountProperty(subAccountID, "frequencyType", subAccountProperties.FrequencyType, employeeID, delegateID);
            }

            if (originalSubAccountProperties.FrequencyValue != subAccountProperties.FrequencyValue)
            {
                SaveAccountProperty(subAccountID, "frequencyValue", subAccountProperties.FrequencyValue, employeeID, delegateID);
            }

            if (originalSubAccountProperties.GlobalLocaleID != subAccountProperties.GlobalLocaleID)
            {
                SaveAccountProperty(subAccountID, "globalLocaleID", subAccountProperties.GlobalLocaleID, employeeID, delegateID);
            }

            if (originalSubAccountProperties.HomeCountry != subAccountProperties.HomeCountry)
            {
                SaveAccountProperty(subAccountID, "homeCountry", subAccountProperties.HomeCountry, employeeID, delegateID);
            }

            if (originalSubAccountProperties.HomeToOffice != subAccountProperties.HomeToOffice)
            {
                SaveAccountProperty(subAccountID, "homeToOffice", subAccountProperties.HomeToOffice, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ImportCC != subAccountProperties.ImportCC)
            {
                SaveAccountProperty(subAccountID, "importCC", subAccountProperties.ImportCC, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ImportHomeAddressFormat != subAccountProperties.ImportHomeAddressFormat)
            {
                SaveAccountProperty(subAccountID, "ImportHomeAddressFormat", subAccountProperties.ImportHomeAddressFormat, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ImportPurchaseCard != subAccountProperties.ImportPurchaseCard)
            {
                SaveAccountProperty(subAccountID, "importPurchaseCard", subAccountProperties.ImportPurchaseCard, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ImportUsernameFormat != subAccountProperties.ImportUsernameFormat)
            {
                SaveAccountProperty(subAccountID, "importUsernameFormat", subAccountProperties.ImportUsernameFormat, employeeID, delegateID);
            }

            if (originalSubAccountProperties.InitialDate != subAccountProperties.InitialDate)
            {
                SaveAccountProperty(subAccountID, "initialDate", subAccountProperties.InitialDate, employeeID, delegateID);
            }

            if (originalSubAccountProperties.KeepInvoiceForecasts != subAccountProperties.KeepInvoiceForecasts)
            {
                SaveAccountProperty(subAccountID, "keepInvoiceForecasts", subAccountProperties.KeepInvoiceForecasts, employeeID, delegateID);
            }

            if (originalSubAccountProperties.Language != subAccountProperties.Language)
            {
                SaveAccountProperty(subAccountID, "language", subAccountProperties.Language, employeeID, delegateID);
            }

            if (originalSubAccountProperties.LimitDates != subAccountProperties.LimitDates)
            {
                SaveAccountProperty(subAccountID, "limitDates", subAccountProperties.LimitDates, employeeID, delegateID);
            }

            if (originalSubAccountProperties.LimitFrequency != subAccountProperties.LimitFrequency)
            {
                SaveAccountProperty(subAccountID, "limitFrequency", subAccountProperties.LimitFrequency, employeeID, delegateID);
            }

            if (originalSubAccountProperties.LimitMonths != subAccountProperties.LimitMonths)
            {
                SaveAccountProperty(subAccountID, "limitMonths", subAccountProperties.LimitMonths, employeeID, delegateID);
            }

            if (originalSubAccountProperties.MainAdministrator != subAccountProperties.MainAdministrator)
            {
                SaveAccountProperty(subAccountID, "mainAdministrator", subAccountProperties.MainAdministrator, employeeID, delegateID);
            }

            if (originalSubAccountProperties.MaxClaimAmount != subAccountProperties.MaxClaimAmount)
            {
                SaveAccountProperty(subAccountID, "maxClaimAmount", subAccountProperties.MaxClaimAmount, employeeID, delegateID);
            }

            if (originalSubAccountProperties.MaxUploadSize != subAccountProperties.MaxUploadSize)
            {
                SaveAccountProperty(subAccountID, "maxUploadSize", subAccountProperties.MaxUploadSize, employeeID, delegateID);
            }

            if (originalSubAccountProperties.Mileage != subAccountProperties.Mileage)
            {
                SaveAccountProperty(subAccountID, "mileage", subAccountProperties.Mileage, employeeID, delegateID);
            }

            if (originalSubAccountProperties.MileageCalcType != subAccountProperties.MileageCalcType)
            {
                SaveAccountProperty(subAccountID, "mileageCalcType", subAccountProperties.MileageCalcType, employeeID, delegateID);
            }

            if (originalSubAccountProperties.MileagePrev != subAccountProperties.MileagePrev)
            {
                SaveAccountProperty(subAccountID, "mileagePrev", subAccountProperties.MileagePrev, employeeID, delegateID);
            }

            if (originalSubAccountProperties.MinClaimAmount != subAccountProperties.MinClaimAmount)
            {
                SaveAccountProperty(subAccountID, "minClaimAmount", subAccountProperties.MinClaimAmount, employeeID, delegateID);
            }

            if (originalSubAccountProperties.NumRows != subAccountProperties.NumRows)
            {
                SaveAccountProperty(subAccountID, "numRows", subAccountProperties.NumRows, employeeID, delegateID);
            }

            if (originalSubAccountProperties.OdometerDay != subAccountProperties.OdometerDay)
            {
                SaveAccountProperty(subAccountID, "odometerDay", subAccountProperties.OdometerDay, employeeID, delegateID);
            }

            if (originalSubAccountProperties.OnlyCashCredit != subAccountProperties.OnlyCashCredit)
            {
                SaveAccountProperty(subAccountID, "onlyCashCredit", subAccountProperties.OnlyCashCredit, employeeID, delegateID);
            }

            if (originalSubAccountProperties.OrderEndDateName != subAccountProperties.OrderEndDateName)
            {
                SaveAccountProperty(subAccountID, "orderEndDateName", subAccountProperties.OrderEndDateName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.OrderRecurrenceName != subAccountProperties.OrderRecurrenceName)
            {
                SaveAccountProperty(subAccountID, "orderRecurrenceName", subAccountProperties.OrderRecurrenceName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.OverrideHome != subAccountProperties.OverrideHome)
            {
                SaveAccountProperty(subAccountID, "overrideHome", subAccountProperties.OverrideHome, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PartSubmit != subAccountProperties.PartSubmit)
            {
                SaveAccountProperty(subAccountID, "partSubmittal", subAccountProperties.PartSubmit, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EditPreviousClaims != subAccountProperties.EditPreviousClaims)
            {
                SaveAccountProperty(subAccountID, "editPreviousClaims", subAccountProperties.EditPreviousClaims, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PolicyType != subAccountProperties.PolicyType)
            {
                SaveAccountProperty(subAccountID, "policyType", subAccountProperties.PolicyType, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PONumberName != subAccountProperties.PONumberName)
            {
                SaveAccountProperty(subAccountID, "poNumberName", subAccountProperties.PONumberName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PONumberGenerate != subAccountProperties.PONumberGenerate)
            {
                SaveAccountProperty(subAccountID, "poNumberGenerate", subAccountProperties.PONumberGenerate, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PONumberSequence != subAccountProperties.PONumberSequence)
            {
                SaveAccountProperty(subAccountID, "poNumberSequence", subAccountProperties.PONumberSequence, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PONumberFormat != subAccountProperties.PONumberFormat)
            {
                SaveAccountProperty(subAccountID, "poNumberFormat", subAccountProperties.PONumberFormat, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PreApproval != subAccountProperties.PreApproval)
            {
                SaveAccountProperty(subAccountID, "preApproval", subAccountProperties.PreApproval, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ProductFieldType != subAccountProperties.ProductFieldType)
            {
                SaveAccountProperty(subAccountID, "productFieldType", Convert.ToByte((int)subAccountProperties.ProductFieldType), employeeID, delegateID);
            }

            if (originalSubAccountProperties.ProductName != subAccountProperties.ProductName)
            {
                SaveAccountProperty(subAccountID, "productName", subAccountProperties.ProductName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ProjectCodesOn != subAccountProperties.ProjectCodesOn)
            {
                SaveAccountProperty(subAccountID, "projectCodesOn", subAccountProperties.ProjectCodesOn, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PurchaseCardSubCatId != subAccountProperties.PurchaseCardSubCatId)
            {
                SaveAccountProperty(subAccountID, "purchaseCardSubCatId", subAccountProperties.PurchaseCardSubCatId, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdConstraint != subAccountProperties.PwdConstraint)
            {
                SaveAccountProperty(subAccountID, "pwdConstraint", ((byte)subAccountProperties.PwdConstraint).ToString(), employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdExpires != subAccountProperties.PwdExpires)
            {
                SaveAccountProperty(subAccountID, "pwdExpires", subAccountProperties.PwdExpires, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdExpiryDays != subAccountProperties.PwdExpiryDays)
            {
                SaveAccountProperty(subAccountID, "pwdExpiryDays", subAccountProperties.PwdExpiryDays, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdHistoryNum != subAccountProperties.PwdHistoryNum)
            {
                SaveAccountProperty(subAccountID, "pwdHistoryNum", subAccountProperties.PwdHistoryNum, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdLength1 != subAccountProperties.PwdLength1)
            {
                SaveAccountProperty(subAccountID, "pwdLength1", subAccountProperties.PwdLength1, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdLength2 != subAccountProperties.PwdLength2)
            {
                SaveAccountProperty(subAccountID, "pwdLength2", subAccountProperties.PwdLength2, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdMaxRetries != subAccountProperties.PwdMaxRetries)
            {
                SaveAccountProperty(subAccountID, "pwdMaxRetries", subAccountProperties.PwdMaxRetries, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdMustContainNumbers != subAccountProperties.PwdMustContainNumbers)
            {
                SaveAccountProperty(subAccountID, "pwdMCN", subAccountProperties.PwdMustContainNumbers, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdMustContainUpperCase != subAccountProperties.PwdMustContainUpperCase)
            {
                SaveAccountProperty(subAccountID, "pwdMCU", subAccountProperties.PwdMustContainUpperCase, employeeID, delegateID);
            }

            if (originalSubAccountProperties.PwdMustContainSymbol != subAccountProperties.PwdMustContainSymbol)
            {
                SaveAccountProperty(subAccountID, "pwdSymbol", subAccountProperties.PwdMustContainSymbol, employeeID, delegateID);
            }

            if (originalSubAccountProperties.RecordOdometer != subAccountProperties.RecordOdometer)
            {
                SaveAccountProperty(subAccountID, "recordOdometer", subAccountProperties.RecordOdometer, employeeID, delegateID);
            }

            if (originalSubAccountProperties.SearchEmployees != subAccountProperties.SearchEmployees)
            {
                SaveAccountProperty(subAccountID, "searchEmployees", subAccountProperties.SearchEmployees, employeeID, delegateID);
            }

            if (originalSubAccountProperties.SendReviewRequests != subAccountProperties.SendReviewRequests)
            {
                SaveAccountProperty(subAccountID, "sendReviewRequests", subAccountProperties.SendReviewRequests, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ShowMileageCatsForUsers != subAccountProperties.ShowMileageCatsForUsers)
            {
                SaveAccountProperty(subAccountID, "showMileageCatsForUsers", subAccountProperties.ShowMileageCatsForUsers, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ShowProductInSearch != subAccountProperties.ShowProductInSearch)
            {
                SaveAccountProperty(subAccountID, "showProductInSearch", subAccountProperties.ShowProductInSearch, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ShowReviews != subAccountProperties.ShowReviews)
            {
                SaveAccountProperty(subAccountID, "showReviews", subAccountProperties.ShowReviews, employeeID, delegateID);
            }

            if (originalSubAccountProperties.SingleClaim != subAccountProperties.SingleClaim)
            {
                SaveAccountProperty(subAccountID, "singleClaim", subAccountProperties.SingleClaim, employeeID, delegateID);
            }

            if (originalSubAccountProperties.SingleClaimCC != subAccountProperties.SingleClaimCC)
            {
                SaveAccountProperty(subAccountID, "singleClaimCC", subAccountProperties.SingleClaimCC, employeeID, delegateID);
            }

            if (originalSubAccountProperties.SingleClaimPC != subAccountProperties.SingleClaimPC)
            {
                SaveAccountProperty(subAccountID, "singleClaimPC", subAccountProperties.SingleClaimPC, employeeID, delegateID);
            }

            if (originalSubAccountProperties.SourceAddress != subAccountProperties.SourceAddress)
            {
                SaveAccountProperty(subAccountID, "sourceAddress", subAccountProperties.SourceAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CompanyPolicy != subAccountProperties.CompanyPolicy)
            {
                SaveAccountProperty(subAccountID, "CompanyPolicy", subAccountProperties.CompanyPolicy, employeeID, delegateID);
            }

            if (originalSubAccountProperties.SupplierFieldType != subAccountProperties.SupplierFieldType)
            {
                SaveAccountProperty(subAccountID, "supplierFieldType", Convert.ToByte((int)subAccountProperties.SupplierFieldType), employeeID, delegateID);
            }

            if (originalSubAccountProperties.ThresholdType != subAccountProperties.ThresholdType)
            {
                SaveAccountProperty(subAccountID, "thresholdType", subAccountProperties.ThresholdType, employeeID, delegateID);
            }


            if (originalSubAccountProperties.TotalName != subAccountProperties.TotalName)
            {
                SaveAccountProperty(subAccountID, "totalName", subAccountProperties.TotalName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseCostCodeDescription != subAccountProperties.UseCostCodeDescription)
            {
                SaveAccountProperty(subAccountID, "useCostCodeDescription", subAccountProperties.UseCostCodeDescription, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseCostCodeOnGenDetails != subAccountProperties.UseCostCodeOnGenDetails)
            {
                SaveAccountProperty(subAccountID, "useCostCodeOnGenDetails", subAccountProperties.UseCostCodeOnGenDetails, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseCostCodes != subAccountProperties.UseCostCodes)
            {
                SaveAccountProperty(subAccountID, "useCostCodes", subAccountProperties.UseCostCodes, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseDepartmentCodeDescription != subAccountProperties.UseDepartmentCodeDescription)
            {
                SaveAccountProperty(subAccountID, "useDepartmentCodeDescription", subAccountProperties.UseDepartmentCodeDescription, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseDepartmentCodes != subAccountProperties.UseDepartmentCodes)
            {
                SaveAccountProperty(subAccountID, "useDepartmentCodes", subAccountProperties.UseDepartmentCodes, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseDeptOnGenDetails != subAccountProperties.UseDeptOnGenDetails)
            {
                SaveAccountProperty(subAccountID, "useDepartmentOnGenDetails", subAccountProperties.UseDeptOnGenDetails, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseMapPoint != subAccountProperties.UseMapPoint)
            {
                SaveAccountProperty(subAccountID, "useMapPoint", subAccountProperties.UseMapPoint, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseProjectCodeDescription != subAccountProperties.UseProjectCodeDescription)
            {
                SaveAccountProperty(subAccountID, "useProjectCodeDescription", subAccountProperties.UseProjectCodeDescription, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseProjectCodeOnGenDetails != subAccountProperties.UseProjectCodeOnGenDetails)
            {
                SaveAccountProperty(subAccountID, "useProjectCodeOnGenDetails", subAccountProperties.UseProjectCodeOnGenDetails, employeeID, delegateID);
            }

            if (originalSubAccountProperties.UseProjectCodes != subAccountProperties.UseProjectCodes)
            {
                SaveAccountProperty(subAccountID, "useProjectCodes", subAccountProperties.UseProjectCodes, employeeID, delegateID);
            }

            if (originalSubAccountProperties.Weekend != subAccountProperties.Weekend)
            {
                SaveAccountProperty(subAccountID, "weekend", subAccountProperties.Weekend, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DelSubmitClaim != subAccountProperties.DelSubmitClaim)
            {
                SaveAccountProperty(subAccountID, "delSubmitClaims", subAccountProperties.DelSubmitClaim, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ContractDatesMandatory != subAccountProperties.ContractDatesMandatory)
            {
                SaveAccountProperty(subAccountID, "contractDatesMandatory", subAccountProperties.ContractDatesMandatory, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableRecharge != subAccountProperties.EnableRecharge)
            {
                SaveAccountProperty(subAccountID, "enableRecharge", subAccountProperties.EnableRecharge, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CustomerHelpInformation != subAccountProperties.CustomerHelpInformation)
            {
                SaveAccountProperty(subAccountID, "customerHelpInformation", subAccountProperties.CustomerHelpInformation, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CustomerHelpContactName != subAccountProperties.CustomerHelpContactName)
            {
                SaveAccountProperty(subAccountID, "customerHelpContactName", subAccountProperties.CustomerHelpContactName, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CustomerHelpContactTelephone != subAccountProperties.CustomerHelpContactTelephone)
            {
                SaveAccountProperty(subAccountID, "customerHelpContactTelephone", subAccountProperties.CustomerHelpContactTelephone, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CustomerHelpContactFax != subAccountProperties.CustomerHelpContactFax)
            {
                SaveAccountProperty(subAccountID, "customerHelpContactFax", subAccountProperties.CustomerHelpContactFax, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CustomerHelpContactAddress != subAccountProperties.CustomerHelpContactAddress)
            {
                SaveAccountProperty(subAccountID, "customerHelpContactAddress", subAccountProperties.CustomerHelpContactAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CustomerHelpContactEmailAddress != subAccountProperties.CustomerHelpContactEmailAddress)
            {
                SaveAccountProperty(subAccountID, "customerHelpContactEmailAddress", subAccountProperties.CustomerHelpContactEmailAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.MandatoryPostcodeForAddresses != subAccountProperties.MandatoryPostcodeForAddresses)
            {
                SaveAccountProperty(subAccountID, "mandatoryPostcodeForAddresses", subAccountProperties.MandatoryPostcodeForAddresses, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ClaimantsCanAddCompanyLocations != subAccountProperties.ClaimantsCanAddCompanyLocations)
            {
                SaveAccountProperty(subAccountID, "ClaimantsCanAddCompanyLocations", subAccountProperties.ClaimantsCanAddCompanyLocations, employeeID, delegateID);
            }
            if (originalSubAccountProperties.LogoPath != subAccountProperties.LogoPath)
            {
                SaveAccountProperty(subAccountID, "logoPath", subAccountProperties.LogoPath, employeeID, delegateID);
            }
            if (originalSubAccountProperties.EmailAdministrator != subAccountProperties.EmailAdministrator)
            {
                SaveAccountProperty(subAccountID, "emailAdministrator", subAccountProperties.EmailAdministrator, employeeID, delegateID);
            }
            if (originalSubAccountProperties.AutoUpdateCVRechargeLive != subAccountProperties.AutoUpdateCVRechargeLive)
            {
                SaveAccountProperty(subAccountID, "autoUpdateCVRechargeLive", subAccountProperties.AutoUpdateCVRechargeLive, employeeID, delegateID);
            }
            if (originalSubAccountProperties.AutoUpdateLicenceTotal != subAccountProperties.AutoUpdateLicenceTotal)
            {
                SaveAccountProperty(subAccountID, "autoUpdateLicenceTotal", subAccountProperties.AutoUpdateLicenceTotal, employeeID, delegateID);
            }
            if (originalSubAccountProperties.CachePeriodLong != subAccountProperties.CachePeriodLong)
            {
                SaveAccountProperty(subAccountID, "cachePeriodLong", subAccountProperties.CachePeriodLong, employeeID, delegateID);
            }
            if (originalSubAccountProperties.CachePeriodNormal != subAccountProperties.CachePeriodNormal)
            {
                SaveAccountProperty(subAccountID, "cachePeriodNormal", subAccountProperties.CachePeriodNormal, employeeID, delegateID);
            }
            if (originalSubAccountProperties.CachePeriodShort != subAccountProperties.CachePeriodShort)
            {
                SaveAccountProperty(subAccountID, "cachePeriodShort", subAccountProperties.CachePeriodShort, employeeID, delegateID);
            }
            if (originalSubAccountProperties.CacheTimeout != subAccountProperties.CacheTimeout)
            {
                SaveAccountProperty(subAccountID, "cacheTimeout", subAccountProperties.CacheTimeout, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ContractCategoryTitle != subAccountProperties.ContractCategoryTitle)
            {
                this.RelabelReportColumns("CONTRACT_CAT_TITLE", originalSubAccountProperties.ContractCategoryTitle, subAccountProperties.ContractCategoryTitle);
                SaveAccountProperty(subAccountID, "contractCategoryTitle", subAccountProperties.ContractCategoryTitle, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ContractCatMandatory != subAccountProperties.ContractCatMandatory)
            {
                SaveAccountProperty(subAccountID, "contractCategoryMandatory", subAccountProperties.ContractCatMandatory, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierCatMandatory != subAccountProperties.SupplierCatMandatory)
            {
                SaveAccountProperty(subAccountID, "supplierCategoryMandatory", subAccountProperties.SupplierCatMandatory, employeeID, delegateID);
            }
            if (originalSubAccountProperties.TaskStartDateMandatory != subAccountProperties.TaskStartDateMandatory)
            {
                SaveAccountProperty(subAccountID, "taskStartDateMandatory", subAccountProperties.TaskStartDateMandatory, employeeID, delegateID);
            }
            if (originalSubAccountProperties.TaskEndDateMandatory != subAccountProperties.TaskEndDateMandatory)
            {
                SaveAccountProperty(subAccountID, "taskEndDateMandatory", subAccountProperties.TaskEndDateMandatory, employeeID, delegateID);
            }
            if (originalSubAccountProperties.TaskDueDateMandatory != subAccountProperties.TaskDueDateMandatory)
            {
                SaveAccountProperty(subAccountID, "taskDueDateMandatory", subAccountProperties.TaskDueDateMandatory, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ContractDescTitle != subAccountProperties.ContractDescTitle)
            {
                this.RelabelReportColumns("CONTRACT_DESC_TITLE", originalSubAccountProperties.ContractDescTitle, subAccountProperties.ContractDescTitle);
                SaveAccountProperty(subAccountID, "contractDescTitle", subAccountProperties.ContractDescTitle, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ContractDescShortTitle != subAccountProperties.ContractDescShortTitle)
            {
                SaveAccountProperty(subAccountID, "contractDescShortTitle", subAccountProperties.ContractDescShortTitle, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ContractNumSeq != subAccountProperties.ContractNumSeq)
            {
                SaveAccountProperty(subAccountID, "contractNumSeq", subAccountProperties.ContractNumSeq, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ContractNumGen != subAccountProperties.ContractNumGen)
            {
                SaveAccountProperty(subAccountID, "contractNumGen", subAccountProperties.ContractNumGen, employeeID, delegateID);
            }
            if (originalSubAccountProperties.EnableContractNumUpdate != subAccountProperties.EnableContractNumUpdate)
            {
                SaveAccountProperty(subAccountID, "contractNumUpdateEnabled", subAccountProperties.EnableContractNumUpdate, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierStatusEnforced != subAccountProperties.SupplierStatusEnforced)
            {
                SaveAccountProperty(subAccountID, "supplierStatusEnforced", subAccountProperties.SupplierStatusEnforced, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierLastFinStatusEnabled != subAccountProperties.SupplierLastFinStatusEnabled)
            {
                SaveAccountProperty(subAccountID, "supplierLastFinStatusEnabled", subAccountProperties.SupplierLastFinStatusEnabled, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierLastFinCheckEnabled != subAccountProperties.SupplierLastFinCheckEnabled)
            {
                SaveAccountProperty(subAccountID, "supplierLastFinCheckEnabled", subAccountProperties.SupplierLastFinCheckEnabled, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierFYEEnabled != subAccountProperties.SupplierFYEEnabled)
            {
                SaveAccountProperty(subAccountID, "supplierFYEEnabled", subAccountProperties.SupplierFYEEnabled, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierNumEmployeesEnabled != subAccountProperties.SupplierNumEmployeesEnabled)
            {
                SaveAccountProperty(subAccountID, "supplierNumEmployeesEnabled", subAccountProperties.SupplierNumEmployeesEnabled, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierTurnoverEnabled != subAccountProperties.SupplierTurnoverEnabled)
            {
                SaveAccountProperty(subAccountID, "supplierTurnoverEnabled", subAccountProperties.SupplierTurnoverEnabled, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierCatTitle != subAccountProperties.SupplierCatTitle)
            {
                this.RelabelReportColumns("SUPPLIER_CAT_TITLE", originalSubAccountProperties.SupplierCatTitle, subAccountProperties.SupplierCatTitle);
                SaveAccountProperty(subAccountID, "supplierCategoryTitle", subAccountProperties.SupplierCatTitle, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ContractScheduleDefault != subAccountProperties.ContractScheduleDefault)
            {
                SaveAccountProperty(subAccountID, "contractScheduleDefault", subAccountProperties.ContractScheduleDefault, employeeID, delegateID);
            }
            if (originalSubAccountProperties.EnableVariationAutoSeq != subAccountProperties.EnableVariationAutoSeq)
            {
                SaveAccountProperty(subAccountID, "variationAutoSeqEnabled", subAccountProperties.EnableVariationAutoSeq, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierVariationTitle != subAccountProperties.SupplierVariationTitle)
            {
                SaveAccountProperty(subAccountID, "supplierVariationTitle", subAccountProperties.SupplierVariationTitle, employeeID, delegateID);
            }
            if (originalSubAccountProperties.EnableAttachmentHyperlink != subAccountProperties.EnableAttachmentHyperlink)
            {
                SaveAccountProperty(subAccountID, "hyperlinkAttachmentsEnabled", subAccountProperties.EnableAttachmentHyperlink, employeeID, delegateID);
            }
            if (originalSubAccountProperties.OpenSaveAttachments != subAccountProperties.OpenSaveAttachments)
            {
                SaveAccountProperty(subAccountID, "openSaveAttachments", subAccountProperties.OpenSaveAttachments, employeeID, delegateID);
            }
            if (originalSubAccountProperties.EnableAttachmentUpload != subAccountProperties.EnableAttachmentUpload)
            {
                SaveAccountProperty(subAccountID, "attachmentUploadEnabled", subAccountProperties.EnableAttachmentUpload, employeeID, delegateID);
            }
            if (originalSubAccountProperties.LinkAttachmentDefault != subAccountProperties.LinkAttachmentDefault)
            {
                SaveAccountProperty(subAccountID, "linkAttachmentDefault", subAccountProperties.LinkAttachmentDefault, employeeID, delegateID);
            }
            if (originalSubAccountProperties.EnableFlashingNotesIcon != subAccountProperties.EnableFlashingNotesIcon)
            {
                SaveAccountProperty(subAccountID, "flashingNotesIconEnabled", subAccountProperties.EnableFlashingNotesIcon, employeeID, delegateID);
            }
            if (originalSubAccountProperties.FYEnds != subAccountProperties.FYEnds)
            {
                SaveAccountProperty(subAccountID, "FYEnds", subAccountProperties.FYEnds, employeeID, delegateID);
            }
            if (originalSubAccountProperties.FYStarts != subAccountProperties.FYStarts)
            {
                SaveAccountProperty(subAccountID, "FYStarts", subAccountProperties.FYStarts, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ValueComments != subAccountProperties.ValueComments)
            {
                SaveAccountProperty(subAccountID, "valueComments", subAccountProperties.ValueComments, employeeID, delegateID);
            }
            if (originalSubAccountProperties.InflatorActive != subAccountProperties.InflatorActive)
            {
                SaveAccountProperty(subAccountID, "inflatorActive", subAccountProperties.InflatorActive, employeeID, delegateID);
            }
            if (originalSubAccountProperties.InvoiceFreqActive != subAccountProperties.InvoiceFreqActive)
            {
                SaveAccountProperty(subAccountID, "invoiceFrequencyActive", subAccountProperties.InvoiceFreqActive, employeeID, delegateID);
            }
            if (originalSubAccountProperties.PenaltyClauseTitle != subAccountProperties.PenaltyClauseTitle)
            {
                this.RelabelReportColumns("PENALTY_CLAUSE_TITLE", originalSubAccountProperties.PenaltyClauseTitle, subAccountProperties.PenaltyClauseTitle);
                SaveAccountProperty(subAccountID, "penaltyClauseTitle", subAccountProperties.PenaltyClauseTitle, employeeID, delegateID);
            }
            if (originalSubAccountProperties.RechargeUnrecoveredTitle != subAccountProperties.RechargeUnrecoveredTitle)
            {
                SaveAccountProperty(subAccountID, "rechargeUnrecoveredTitle", subAccountProperties.RechargeUnrecoveredTitle, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierIntContactEnabled != subAccountProperties.SupplierIntContactEnabled)
            {
                SaveAccountProperty(subAccountID, "supplierIntContactEnabled", subAccountProperties.SupplierIntContactEnabled, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierPrimaryTitle != subAccountProperties.SupplierPrimaryTitle)
            {
                this.RelabelReportColumns("SUPPLIER_PRIMARY_TITLE", originalSubAccountProperties.SupplierPrimaryTitle, subAccountProperties.SupplierPrimaryTitle);
                SaveAccountProperty(subAccountID, "supplierPrimaryTitle", subAccountProperties.SupplierPrimaryTitle, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SupplierRegionTitle != subAccountProperties.SupplierRegionTitle)
            {
                this.RelabelReportColumns("SUPPLIER_REGION_TITLE", originalSubAccountProperties.SupplierRegionTitle, subAccountProperties.SupplierRegionTitle);
                SaveAccountProperty(subAccountID, "supplierRegionTitle", subAccountProperties.SupplierRegionTitle, employeeID, delegateID);
            }
            if (originalSubAccountProperties.TaskEscalationRepeat != subAccountProperties.TaskEscalationRepeat)
            {
                SaveAccountProperty(subAccountID, "taskEscalationRepeat", subAccountProperties.TaskEscalationRepeat, employeeID, delegateID);
            }
            if (originalSubAccountProperties.TermTypeActive != subAccountProperties.TermTypeActive)
            {
                SaveAccountProperty(subAccountID, "termTypeActive", subAccountProperties.TermTypeActive, employeeID, delegateID);
            }
            if (originalSubAccountProperties.UseCPExtraInfo != subAccountProperties.UseCPExtraInfo)
            {
                SaveAccountProperty(subAccountID, "useCPExtraInfo", subAccountProperties.UseCPExtraInfo, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursHeaderBackground != subAccountProperties.ColoursHeaderBackground)
            {
                SaveAccountProperty(subAccountID, "coloursHeaderBackground", subAccountProperties.ColoursHeaderBackground, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursHeaderBreadcrumbText != subAccountProperties.ColoursHeaderBreadcrumbText)
            {
                SaveAccountProperty(subAccountID, "coloursHeaderBreadcrumbText", subAccountProperties.ColoursHeaderBreadcrumbText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursPageTitleText != subAccountProperties.ColoursPageTitleText)
            {
                SaveAccountProperty(subAccountID, "coloursPageTitleText", subAccountProperties.ColoursPageTitleText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursSectionHeadingUnderline != subAccountProperties.ColoursSectionHeadingUnderline)
            {
                SaveAccountProperty(subAccountID, "coloursSectionHeadingUnderline", subAccountProperties.ColoursSectionHeadingUnderline, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursSectionHeadingText != subAccountProperties.ColoursSectionHeadingText)
            {
                SaveAccountProperty(subAccountID, "coloursSectionHeadingText", subAccountProperties.ColoursSectionHeadingText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursFieldText != subAccountProperties.ColoursFieldText)
            {
                SaveAccountProperty(subAccountID, "coloursFieldText", subAccountProperties.ColoursFieldText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursPageOptionsBackground != subAccountProperties.ColoursPageOptionsBackground)
            {
                SaveAccountProperty(subAccountID, "coloursPageOptionsBackground", subAccountProperties.ColoursPageOptionsBackground, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursPageOptionsText!= subAccountProperties.ColoursPageOptionsText)
            {
                SaveAccountProperty(subAccountID, "coloursPageOptionsText", subAccountProperties.ColoursPageOptionsText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursTableHeaderBackground != subAccountProperties.ColoursTableHeaderBackground)
            {
                SaveAccountProperty(subAccountID, "coloursTableHeaderBackground", subAccountProperties.ColoursTableHeaderBackground, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursTableHeaderText != subAccountProperties.ColoursTableHeaderText)
            {
                SaveAccountProperty(subAccountID, "coloursTableHeaderText", subAccountProperties.ColoursTableHeaderText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursTabOptionBackground != subAccountProperties.ColoursTabOptionBackground)
            {
                SaveAccountProperty(subAccountID, "coloursTabOptionBackground", subAccountProperties.ColoursTabOptionBackground, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursTabOptionText != subAccountProperties.ColoursTabOptionText)
            {
                SaveAccountProperty(subAccountID, "coloursTabOptionText", subAccountProperties.ColoursTabOptionText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursRowBackground != subAccountProperties.ColoursRowBackground)
            {
                SaveAccountProperty(subAccountID, "coloursRowBackground", subAccountProperties.ColoursRowBackground, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursRowText != subAccountProperties.ColoursRowText)
            {
                SaveAccountProperty(subAccountID, "coloursRowText", subAccountProperties.ColoursRowText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursAlternateRowBackground != subAccountProperties.ColoursAlternateRowBackground)
            {
                SaveAccountProperty(subAccountID, "coloursAlternateRowBackground", subAccountProperties.ColoursAlternateRowBackground, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursAlternateRowText != subAccountProperties.ColoursAlternateRowText)
            {
                SaveAccountProperty(subAccountID, "coloursAlternateRowText", subAccountProperties.ColoursAlternateRowText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursMenuOptionHoverText != subAccountProperties.ColoursMenuOptionHoverText)
            {
                SaveAccountProperty(subAccountID, "coloursMenuOptionHoverText", subAccountProperties.ColoursMenuOptionHoverText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursMenuOptionStandardText != subAccountProperties.ColoursMenuOptionStandardText)
            {
                SaveAccountProperty(subAccountID, "coloursMenuOptionStandardText", subAccountProperties.ColoursMenuOptionStandardText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursTooltipBackground != subAccountProperties.ColoursTooltipBackground)
            {
                SaveAccountProperty(subAccountID, "coloursTooltipBackground", subAccountProperties.ColoursTooltipBackground, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursTooltipText != subAccountProperties.ColoursTooltipText)
            {
                SaveAccountProperty(subAccountID, "coloursTooltipText", subAccountProperties.ColoursTooltipText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursGreenLightField != subAccountProperties.ColoursGreenLightField)
            {
                SaveAccountProperty(subAccountID, "coloursGreenLightField", subAccountProperties.ColoursGreenLightField, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursGreenLightSectionText != subAccountProperties.ColoursGreenLightSectionText)
            {
                SaveAccountProperty(subAccountID, "coloursGreenLightSectionText", subAccountProperties.ColoursGreenLightSectionText, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursGreenLightSectionBackground != subAccountProperties.ColoursGreenLightSectionBackground)
            {
                SaveAccountProperty(subAccountID, "coloursGreenLightSectionBackground", subAccountProperties.ColoursGreenLightSectionBackground, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ColoursGreenLightSectionUnderline != subAccountProperties.ColoursGreenLightSectionUnderline)
            {
                SaveAccountProperty(subAccountID, "coloursGreenLightSectionUnderline", subAccountProperties.ColoursGreenLightSectionUnderline, employeeID, delegateID);
            }
            if (originalSubAccountProperties.AllowEmployeeInOwnSignoffGroup != subAccountProperties.AllowEmployeeInOwnSignoffGroup)
            {
                SaveAccountProperty(subAccountID, "AllowEmployeeInOwnSignoffGroup", subAccountProperties.AllowEmployeeInOwnSignoffGroup, employeeID, delegateID);
            }
            if (originalSubAccountProperties.UseMobileDevices != subAccountProperties.UseMobileDevices)
            {
                SaveAccountProperty(subAccountID, "useMobileDevices", subAccountProperties.UseMobileDevices, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowEmpToSpecifyCarDOCOnAdd != subAccountProperties.AllowEmpToSpecifyCarDOCOnAdd)
            {
                this.SaveAccountProperty(subAccountID, "allowEmployeeToSpecifyCarDOCOnAdd", subAccountProperties.AllowEmpToSpecifyCarDOCOnAdd, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowEmpToSpecifyCarStartDateOnAdd != subAccountProperties.AllowEmpToSpecifyCarStartDateOnAdd)
            {
                this.SaveAccountProperty(subAccountID, "allowEmployeeToSpecifyCarStartDateOnAdd", subAccountProperties.AllowEmpToSpecifyCarStartDateOnAdd, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EmpToSpecifyCarStartDateOnAddMandatory != subAccountProperties.EmpToSpecifyCarStartDateOnAddMandatory)
            {
                this.SaveAccountProperty(subAccountID, "allowEmployeeToSpecifyCarStartDateOnAddMandatory", subAccountProperties.EmpToSpecifyCarStartDateOnAddMandatory, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AllowEmployeeToNotifyOfChangeOfDetails != subAccountProperties.AllowEmployeeToNotifyOfChangeOfDetails)
            {
                this.SaveAccountProperty(subAccountID, "allowEmployeeToNotifyOfChangeOfDetails", subAccountProperties.AllowEmployeeToNotifyOfChangeOfDetails, employeeID, delegateID);
            }
            if (originalSubAccountProperties.CorporateDStartPage != subAccountProperties.CorporateDStartPage)
            {
                this.SaveAccountProperty(subAccountID, "corporateDiligenceStartPage", subAccountProperties.CorporateDStartPage, employeeID, delegateID);
            }
            if (originalSubAccountProperties.AllowEmployeeToNotifyOfChangeOfDetails != subAccountProperties.AllowEmployeeToNotifyOfChangeOfDetails)
            {
                this.SaveAccountProperty(subAccountID, "allowEmployeeToNotifyOfChangeOfDetails", subAccountProperties.AllowEmployeeToNotifyOfChangeOfDetails, employeeID, delegateID);
            }
            if (originalSubAccountProperties.CorporateDStartPage != subAccountProperties.CorporateDStartPage)
            {
                this.SaveAccountProperty(subAccountID, "corporateDiligenceStartPage", subAccountProperties.CorporateDStartPage, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableEsrDiagnostics != subAccountProperties.EnableEsrDiagnostics)
            {
                this.SaveAccountProperty(subAccountID, "enableESRDiagnostics", subAccountProperties.EnableEsrDiagnostics, employeeID, delegateID);
            }
            if (originalSubAccountProperties.SingleSignOnLookupField != subAccountProperties.SingleSignOnLookupField)
            {
                this.SaveAccountProperty(subAccountID, "singleSignOnLookupField", subAccountProperties.SingleSignOnLookupField, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CostCodeOwnerBaseKey != subAccountProperties.CostCodeOwnerBaseKey)
            {
                this.SaveAccountProperty(subAccountID, "defaultCostCodeOwner", subAccountProperties.CostCodeOwnerBaseKey, employeeID, delegateID);
            }

            if (originalSubAccountProperties.IncludeAssignmentDetails != subAccountProperties.IncludeAssignmentDetails)
            {
                this.SaveAccountProperty(subAccountID, "includeAssignmentDetails", (byte)subAccountProperties.IncludeAssignmentDetails, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EsrAutoActivateCar != subAccountProperties.EsrAutoActivateCar)
            {
                this.SaveAccountProperty(subAccountID, "esrAutoActivateCar", subAccountProperties.EsrAutoActivateCar, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DisableCarOutsideOfStartEndDate != subAccountProperties.DisableCarOutsideOfStartEndDate)
            {
                this.SaveAccountProperty(subAccountID, "DisableCarOutsideOfStartEndDate", subAccountProperties.DisableCarOutsideOfStartEndDate, employeeID, delegateID);
            }

            if (originalSubAccountProperties.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim
                != subAccountProperties.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim)
            {
                this.SaveAccountProperty(subAccountID, "NumberOfApproversToRememberForClaimantInApprovalMatrixClaim", subAccountProperties.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim, employeeID, delegateID);
            }

            if (originalSubAccountProperties.RetainLabelsTime != subAccountProperties.RetainLabelsTime)
            {
                this.SaveAccountProperty(subAccountID, "retainLabelsTime", subAccountProperties.RetainLabelsTime, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ShowFullHomeAddressOnClaims != subAccountProperties.ShowFullHomeAddressOnClaims)
            {
                this.SaveAccountProperty(subAccountID, "showFullHomeAddress", subAccountProperties.ShowFullHomeAddressOnClaims, employeeID, delegateID);
            }

            if (originalSubAccountProperties.HomeAddressKeyword != subAccountProperties.HomeAddressKeyword)
            {
                this.SaveAccountProperty(subAccountID, "homeAddressKeyword", subAccountProperties.HomeAddressKeyword, employeeID, delegateID);
            }

            if (originalSubAccountProperties.WorkAddressKeyword != subAccountProperties.WorkAddressKeyword)
            {
                this.SaveAccountProperty(subAccountID, "workAddressKeyword", subAccountProperties.WorkAddressKeyword, employeeID, delegateID);
            }

            if (originalSubAccountProperties.ForceAddressNameEntry != subAccountProperties.ForceAddressNameEntry)
            {
                this.SaveAccountProperty(subAccountID, "forceAddressNameEntry", subAccountProperties.ForceAddressNameEntry, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AddressNameEntryMessage != subAccountProperties.AddressNameEntryMessage)
            {
                this.SaveAccountProperty(subAccountID, "addressNameEntryMessage", subAccountProperties.AddressNameEntryMessage, employeeID, delegateID);
            }

            if (originalSubAccountProperties.IdleTimeout != subAccountProperties.IdleTimeout)
            {
                this.SaveAccountProperty(subAccountID, "idleTimeout", subAccountProperties.IdleTimeout, employeeID, delegateID);
            }

            if (originalSubAccountProperties.CountdownTimer != subAccountProperties.CountdownTimer)
            {
                this.SaveAccountProperty(subAccountID, "countdownTimer", subAccountProperties.CountdownTimer, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableInternalSupportTickets != subAccountProperties.EnableInternalSupportTickets)
            {
                this.SaveAccountProperty(subAccountID, "EnableInternalSupportTickets", subAccountProperties.EnableInternalSupportTickets, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DisplayEsrAddressesInSearchResults != subAccountProperties.DisplayEsrAddressesInSearchResults)
            {
                this.SaveAccountProperty(subAccountID, "DisplayEsrAddressesInSearchResults", subAccountProperties.DisplayEsrAddressesInSearchResults, employeeID, delegateID);
            }
            if (originalSubAccountProperties.NotifyWhenEnvelopeNotReceivedDays != subAccountProperties.NotifyWhenEnvelopeNotReceivedDays)
            {
                this.SaveAccountProperty(subAccountID, "NotifyWhenEnvelopeNotReceivedDays", subAccountProperties.NotifyWhenEnvelopeNotReceivedDays, employeeID, delegateID);
            }
            if (originalSubAccountProperties.AllowReceiptTotalToPassValidation != subAccountProperties.AllowReceiptTotalToPassValidation)
            {
                SaveAccountProperty(subAccountID, "allowExpenseItemsLessThanTheReceiptTotalToPassValidation", subAccountProperties.AllowReceiptTotalToPassValidation, employeeID, delegateID);
            }
            if (originalSubAccountProperties.AllowViewFundDetails != subAccountProperties.AllowViewFundDetails)
            {
                SaveAccountProperty(subAccountID, "allowViewFundDetails", subAccountProperties.AllowViewFundDetails, employeeID, delegateID);
            }
            if (originalSubAccountProperties.RemindClaimantOnDOCDocumentExpiryDays != subAccountProperties.RemindClaimantOnDOCDocumentExpiryDays)
            {
                SaveAccountProperty(subAccountID, "dutyOfCareEmailReminderForClaimantDays", subAccountProperties.RemindClaimantOnDOCDocumentExpiryDays, employeeID, delegateID);
            }
            if (originalSubAccountProperties.RemindApproverOnDOCDocumentExpiryDays != subAccountProperties.RemindApproverOnDOCDocumentExpiryDays)
            {
                SaveAccountProperty(subAccountID, "dutyOfCareEmailReminderForApproverDays", subAccountProperties.RemindApproverOnDOCDocumentExpiryDays, employeeID, delegateID);
            }
            if (originalSubAccountProperties.DutyOfCareApprover != subAccountProperties.DutyOfCareApprover)
            {
                SaveAccountProperty(subAccountID, "dutyOfCareApprover", subAccountProperties.DutyOfCareApprover, employeeID, delegateID);
            }
            if (originalSubAccountProperties.DutyOfCareTeamAsApprover != subAccountProperties.DutyOfCareTeamAsApprover)
            {
                SaveAccountProperty(subAccountID, "dutyOfCareTeamAsApprover", subAccountProperties.DutyOfCareTeamAsApprover, employeeID, delegateID);
            }

            if (originalSubAccountProperties.SummaryEsrInboundFile != subAccountProperties.SummaryEsrInboundFile)
            {
                SaveAccountProperty(subAccountID, "SummaryESRInboundFile", subAccountProperties.SummaryEsrInboundFile, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EsrRounding != subAccountProperties.EsrRounding)
            {
                this.SaveAccountProperty(subAccountID, "EsrRounding", (byte)subAccountProperties.EsrRounding, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableClaimApprovalReminders != subAccountProperties.EnableClaimApprovalReminders)
            {
                SaveAccountProperty(subAccountID, "EnableClaimApprovalReminders", subAccountProperties.EnableClaimApprovalReminders, employeeID, delegateID);
            }
            if (originalSubAccountProperties.ClaimApprovalReminderFrequency != subAccountProperties.ClaimApprovalReminderFrequency)
            {
                SaveAccountProperty(subAccountID, "ClaimApprovalReminderFrequency", subAccountProperties.ClaimApprovalReminderFrequency, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableCurrentClaimsReminders != subAccountProperties.EnableCurrentClaimsReminders)
            {
                SaveAccountProperty(subAccountID, "EnableCurrentClaimsReminders", subAccountProperties.EnableCurrentClaimsReminders, employeeID, delegateID);

            }
            if (originalSubAccountProperties.CurrentClaimsReminderFrequency != subAccountProperties.CurrentClaimsReminderFrequency)
            {
                SaveAccountProperty(subAccountID, "CurrentClaimsReminderFrequency", subAccountProperties.CurrentClaimsReminderFrequency, employeeID, delegateID);
            }
            if (originalSubAccountProperties.EnableDelegateOptionsForDelegateAccessRole != subAccountProperties.EnableDelegateOptionsForDelegateAccessRole)
            {
                this.SaveAccountProperty(subAccountID, "delOptionsForDelegateAccessRole", subAccountProperties.EnableDelegateOptionsForDelegateAccessRole, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableESRManualAssignmentSupervisor != subAccountProperties.EnableESRManualAssignmentSupervisor)
            {
                this.SaveAccountProperty(subAccountID, "ESRManualSupervisorAssignment", subAccountProperties.EnableESRManualAssignmentSupervisor, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableCalculationsForAllocatingFuelReceiptVatToMileage != subAccountProperties.EnableCalculationsForAllocatingFuelReceiptVatToMileage)
            {
                this.SaveAccountProperty(subAccountID, "enableCalculationsForAllocatingFuelReceiptVATtoMileage", subAccountProperties.EnableCalculationsForAllocatingFuelReceiptVatToMileage, employeeID, delegateID);
            }

            if (originalSubAccountProperties.FrequencyOfConsentRemindersLookup != subAccountProperties.FrequencyOfConsentRemindersLookup)
            {
                this.SaveAccountProperty(subAccountID, "consentReminderFrequency", subAccountProperties.FrequencyOfConsentRemindersLookup, employeeID, delegateID);
            }

            if (originalSubAccountProperties.MultipleWorkAddress != subAccountProperties.MultipleWorkAddress)
            {
                this.SaveAccountProperty(subAccountID, "MultipleWorkAddress", subAccountProperties.MultipleWorkAddress, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DrivingLicenceReviewFrequency != subAccountProperties.DrivingLicenceReviewFrequency)
            {
                this.SaveAccountProperty(subAccountID, "DrivingLicenceReviewFrequency", subAccountProperties.DrivingLicenceReviewFrequency, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableDrivingLicenceReview != subAccountProperties.EnableDrivingLicenceReview)
            {
                this.SaveAccountProperty(subAccountID, "DrivingLicenceReviewPeriodically", subAccountProperties.EnableDrivingLicenceReview, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DrivingLicenceReviewReminder != subAccountProperties.DrivingLicenceReviewReminder)
            {
                this.SaveAccountProperty(subAccountID, "DrivingLicenceReviewReminder", subAccountProperties.DrivingLicenceReviewReminder, employeeID, delegateID);
            }

            if (originalSubAccountProperties.DrivingLicenceReviewReminderDays != subAccountProperties.DrivingLicenceReviewReminderDays)
            {
                this.SaveAccountProperty(subAccountID, "DrivingLicenceReviewReminderDays", subAccountProperties.DrivingLicenceReviewReminderDays, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EnableAutoUpdateOfExchangeRates != subAccountProperties.EnableAutoUpdateOfExchangeRates)
            {
                this.SaveAccountProperty(subAccountID, "EnableAutoUpdateOfExchangeRates", subAccountProperties.EnableAutoUpdateOfExchangeRates, employeeID, delegateID);
                if (subAccountProperties.EnableAutoUpdateOfExchangeRates)
                {
                    this.SaveAccountProperty(subAccountID, "EnableAutoUpdateOfExchangeRatesActivatedDate", DateTime.Now, employeeID, delegateID);
                    var exchangeRates = AutoUpdateExchangeRateModifier.New(originalSubAccountProperties.currencyType, this.AccountId);
                    exchangeRates.PopulateRanges(originalSubAccountProperties.EnableAutoUpdateOfExchangeRatesActivatedDate);
                    this.SaveAccountProperty(subAccountID, "currencyType", (int)CurrencyType.Range, employeeID, delegateID);
                } 
            }

            if (originalSubAccountProperties.ExchangeRateProvider != subAccountProperties.ExchangeRateProvider)
            {
                this.SaveAccountProperty(subAccountID, "ExchangeRateProvider", (int)subAccountProperties.ExchangeRateProvider, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AccountLockedMessage != subAccountProperties.AccountLockedMessage)
            {
                this.SaveAccountProperty(subAccountID, "AccountLockedMessage", subAccountProperties.AccountLockedMessage, employeeID, delegateID);
            }

            if (originalSubAccountProperties.AccountCurrentlyLockedMessage != subAccountProperties.AccountLockedMessage)
            {
                this.SaveAccountProperty(subAccountID, "AccountCurrentlyLockedMessage", subAccountProperties.AccountCurrentlyLockedMessage, employeeID, delegateID);
            }

            if (originalSubAccountProperties.EsrPrimaryAddressOnly != subAccountProperties.EsrPrimaryAddressOnly)
            {
                this.SaveAccountProperty(subAccountID, "esrPrimaryAddressOnly", subAccountProperties.EsrPrimaryAddressOnly, employeeID, delegateID);
            }

            if (originalSubAccountProperties.BlockUnmachedExpenseItemsBeingSubmitted != subAccountProperties.BlockUnmachedExpenseItemsBeingSubmitted)
            {
                this.SaveAccountProperty(subAccountID, "BlockUnmachedExpenseItemsBeingSubmitted", subAccountProperties.BlockUnmachedExpenseItemsBeingSubmitted, employeeID, delegateID);
            }
            
            if (originalSubAccountProperties.VehicleLookup != subAccountProperties.VehicleLookup)
            {
                this.SaveAccountProperty(subAccountID, "VehicleLookup", subAccountProperties.VehicleLookup, employeeID, delegateID);
            }

            this.InvalidateCache(subAccountID);
        }


        /// <summary>
        /// Relabel any report columns, changing old value to new value
        /// </summary>
        /// <param name="relabelParam">The Relabel Param to use</param>
        /// <param name="oldValue">The original value</param>
        /// <param name="newValue">The new replacement value</param>
        private void RelabelReportColumns(string relabelParam, string oldValue, string newValue)
        {
            var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            connection.sqlexecute.Parameters.Clear();
            connection.sqlexecute.Parameters.AddWithValue("@relabelParam", relabelParam);
            connection.sqlexecute.Parameters.AddWithValue("@oldValue", oldValue);
            connection.sqlexecute.Parameters.AddWithValue("@newValue", newValue);
            connection.ExecuteProc("dbo.RelabelReportColumns");
        }


        /// <summary>
        /// Update account properties contained in the provided collection
        /// </summary>
        /// <param name="subAccountID"></param>
        /// <param name="properties"></param>
        /// <param name="modifiedBy"></param>
        /// <param name="delegateId"></param>
        public void SaveProperties(int subAccountID, Dictionary<string, string> properties, int modifiedBy, int? delegateId)
        {
            foreach (KeyValuePair<string, string> kvp in properties)
            {
                SaveAccountProperty(subAccountID, kvp.Key, kvp.Value, modifiedBy, delegateId);
            }
        }

        /// <summary>
        /// Saves the individual account property
        /// </summary>
        /// <param name="subAccountID">Sub Account ID</param>
        /// <param name="key">Property Record Key</param>
        /// <param name="value">Value to be saved</param>
        /// <param name="modifiedBy">Employee ID performing the modification</param>
        /// <param name="delegateId">Delegate Employee ID if acting as delegate</param>
        private void SaveAccountProperty(int subAccountID, string key, object value, int modifiedBy, int? delegateId, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            expdata.sqlexecute.Parameters.Clear();
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountID", subAccountID);
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedBy", modifiedBy);
            expdata.sqlexecute.Parameters.AddWithValue("@stringKey", key);

            if (value == null)
            {
                value = string.Empty;
            }

            if (string.IsNullOrEmpty(value.ToString()))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@stringValue", DBNull.Value);
            }
            else
            {
                if (value.ToString() == "False")
                {
                    value = 0;
                }
                else if (value.ToString() == "True")
                {
                    value = 1;
                }

                expdata.sqlexecute.Parameters.AddWithValue("@stringValue", value.ToString());
            }

            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", modifiedBy);
            if (delegateId.HasValue)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", delegateId.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }

            expdata.ExecuteProc("dbo.saveSubAccountProperties");
            expdata.sqlexecute.Parameters.Clear();
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
        /// Obtain any properties that have been changed since a certain date
        /// </summary>
        /// <param name="modifiedSince">Date to check changes since</param>
        /// <param name="subAccountId">Sub-Account Id to filter by (NULL if not applicable)</param>
        /// <returns>Dictionary list of "property name","property value"</returns>
        public Dictionary<string, string> getPropertiesModified(DateTime modifiedSince, int? subAccountId, IDBConnection connection = null)
        {
            var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId));
            Dictionary<string, string> retList = new Dictionary<string, string>();
            expdata.sqlexecute.Parameters.Clear();
            string strSQL = "SELECT stringKey, stringValue FROM dbo.accountProperties where ((createdOn >= @modifiedSince and createdOn is not null) or (modifiedOn >= @modifiedSince and modifiedOn is not null))";
            if (subAccountId.HasValue)
            {
                strSQL += " and subAccountID = @subAccId";
                expdata.sqlexecute.Parameters.AddWithValue("@subAccId", subAccountId);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedSince", modifiedSince);
            expdata.sqlexecute.CommandText = strSQL;

            using (var reader = expdata.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    string stringKey = reader.GetString(reader.GetOrdinal("stringKey")).Trim();
                    string stringValue;
                    if (reader.IsDBNull(reader.GetOrdinal("stringValue")))
                    {
                        stringValue = string.Empty;
                    }
                    else
                    {
                        stringValue = reader.GetString(reader.GetOrdinal("stringValue")).Trim();
                    }

                    retList.Add(stringKey, stringValue);
                }
                reader.Close();
            }
            return retList;
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
                            case "VehicleLookup":
                                lstAccountProperties.VehicleLookup = Convert.ToBoolean(Convert.ToInt32(stringValue));
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

                var generalOptions = new GeneralOptions.GeneralOptions(this._accountId);
                generalOptions.InvalidateCache(subAccountId);
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

        public void IncrementPONumber(int subAccountId, int employeeId)
        {
            cAccountProperties properties = getSubAccountById(subAccountId).SubAccountProperties;
            properties.PONumberSequence++;

            SaveAccountProperty(subAccountId, "poNumberSequence", properties.PONumberSequence, employeeId, null);
        }
    }
}
