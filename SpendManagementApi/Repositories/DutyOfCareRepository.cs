namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SpendManagementApi.Interfaces;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees.DutyOfCare;

    using Spend_Management;

    /// <summary>
    /// The duty of care repository.
    /// </summary>
    internal class DutyOfCareRepository : BaseRepository<DocumentExpiry>, ISupportsActionContext
    {
        /// <summary>
        /// Base repository constructor
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="idSelector">Id property of T</param>
        /// <param name="nameSelector">Name property of T</param>
        public DutyOfCareRepository(ICurrentUser user, Func<DocumentExpiry, int> idSelector, Func<DocumentExpiry, string> nameSelector)
            : base(user, idSelector, nameSelector)
        {
        }

        public DutyOfCareRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => x.carId, null)
        {
    
      
        }

        /// <summary>
        /// Get all of T
        /// </summary>
        /// <returns></returns>
        public override IList<DocumentExpiry> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get item with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override DocumentExpiry Get(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the duty of care results for the expense date provided
        /// </summary>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <param name="employeeId">Employee id</param>
        /// <param name="expenseId">Expense id</param>
        /// <returns>
        /// The list of<see cref="DocumentExpiry"/>.
        /// </returns>
        public List<DocumentExpiry> GetDutyOfCareResults(DateTime expenseDate, int employeeId = 0, int expenseId = 0)
        {
            List<cCar> activeCars = new List<cCar>();
            var date = this.DetermineExpenseDate(expenseDate);

            if (employeeId > 0)
            {
                var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
                expenseItemRepository.ClaimantDataPermissionCheck(expenseId, employeeId);
                activeCars = new cEmployeeCars(User.AccountID, employeeId, date).GetActiveCars();
            }
            else
            {
                activeCars = this.ActionContext.EmployeeCars.GetActiveCars(date, false);
            }

            var dutyOfCareResults = this.ActionContext.DutyOfCareDocuments.PassesDutyOfCare(
                this.User.AccountID,
                activeCars,
                employeeId > 0 ? employeeId : this.User.EmployeeID,
                date,
                this.User.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect));

            string consentMessage = string.Empty;
            bool isManualDocumentValid = dutyOfCareResults.FirstOrDefault().Value;

            if (!isManualDocumentValid)
            {
                 consentMessage = this.GetDvlaConsentMessage();
            }
                
            var documentExpiryResults = new List<DocumentExpiry>();

            if (activeCars.Count == 0)
            {
                if (consentMessage != string.Empty)
                {
                    return documentExpiryResults;
                }

                //Add consent message to first entry as is applicable to the employee 
                documentExpiryResults.Add(new DocumentExpiry());
                documentExpiryResults.FirstOrDefault().DutyOfCareExpiryMessages = consentMessage;
                return documentExpiryResults;
            }
            else
            {

              
                var docResults = new List<DocumentExpiryResult>();

                foreach (DocumentExpiryResult result in dutyOfCareResults.Keys.First())
                {
                    // IsValidLicence is set to false in method PassesDutyOfCare if any vehicles fail DOC checks, even if the licence is valid. If licence is actually invalid then the carId is 0.   
                    // This if statement ensure the correct state is set for IsValidLicence.
                    if (result.carId == 0 && !result.IsValidLicence)
                    {
                        result.IsValidLicence = false;
                    }
                    else
                    {
                        result.IsValidLicence = true;
                    }

                    docResults.Add(result);
                }

                documentExpiryResults.AddRange(docResults.Select(result => new DocumentExpiry().From(result, this.ActionContext)));

                if (documentExpiryResults.Count == 0)
                {
                    bool isValid = false;
               
                    if (consentMessage == string.Empty)
                    {
                        //no consent message, so assume licence is valid
                        isValid = true;
                    }

                    var docExpiry = new DocumentExpiry { IsValidLicence = isValid };
                    documentExpiryResults.Add(docExpiry);
                }

                //Add consent message to first entry as is applicable to the employee 
                documentExpiryResults.FirstOrDefault().DutyOfCareExpiryMessages = consentMessage;

                return documentExpiryResults;
            }
        }

        /// <summary>
        /// Gets the vehicle documents required when a vehilce has been added
        /// </summary>
        /// <returns>
        /// A list of <see cref="DutyOfCareDocument">DutyOfCareDocument</see>
        /// </returns>
        public List<DutyOfCareDocument> VehicleDocumentsRequiredForDOC()
        {
            var docDocuments = new List<DutyOfCareDocument>();
            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            cAccountProperties accountProperties = subAccounts.getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties;

            if (accountProperties.AllowEmpToSpecifyCarDOCOnAdd)
            {
                if (accountProperties.BlockTaxExpiry)
                {
                    docDocuments.Add(new DutyOfCareDocument { DocumentName = "Tax" });
                }
                if (accountProperties.BlockMOTExpiry)
                {
                    docDocuments.Add(new DutyOfCareDocument { DocumentName = "MOT" });
                }

                if (accountProperties.BlockInsuranceExpiry)
                {
                    docDocuments.Add(new DutyOfCareDocument { DocumentName = "Insurance" });
                }

                if (accountProperties.BlockBreakdownCoverExpiry)
                {
                    docDocuments.Add(new DutyOfCareDocument { DocumentName = "Breakdown Cover" });
                }
            }

            return docDocuments;
        }

        /// <summary>
        /// The get sorn vehicles for the current user
        /// </summary>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <param name="employeeId">Employee id</param>
        /// <param name="expenseId">Expense id</param>
        /// <returns>
        /// A list of <see cref="SornVehicle">SornVehicle</see>
        /// </returns>
        public List<SornVehicle> GetSornVehicles(DateTime expenseDate, int employeeId = 0, int expenseId = 0)
        {
            if (employeeId > 0)
            {
                var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
                expenseItemRepository.ClaimantDataPermissionCheck(expenseId, employeeId);
            }

            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            var subAccountProperties = subAccounts.getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties;
            if (subAccountProperties.BlockTaxExpiry)
            {
                var date = this.DetermineExpenseDate(expenseDate);
                var sornDeclaredVehicles = this.ActionContext.EmployeeCars.GetSORNDeclaredCars(this.User.AccountID, employeeId > 0 ? employeeId : this.User.EmployeeID, date);
                return sornDeclaredVehicles.Select(vehicle => new SornVehicle { vehicleId = Convert.ToInt32(vehicle.Text), Registration = vehicle.Value }).ToList();
            }
            else
            {
                return new List<SornVehicle>();
            }
        }

        /// <summary>
        /// Get accounts with Driving licenceReview check enabled 
        /// </summary>
        /// <returns> Account details</returns>
        public static GeneralOptionAccountsResponse GetAccountsWithDrivingLicenceAndReviewExpiryEnabled()
        {
            var accountList = new GeneralOptionAccountsResponse();
            var accounts = new cAccounts().GetAllAccounts();
            if (accounts == null || accounts.Count == 0)
            {
                return accountList;
            }

            foreach (var account in accounts)
            {
                var subAccounts = new cAccountSubAccounts(account.accountid);
                var reqSubAccount = subAccounts.getFirstSubAccount();
                if (reqSubAccount.SubAccountProperties.BlockDrivingLicence && reqSubAccount.SubAccountProperties.EnableDrivingLicenceReview)
                {
                    accountList.AccountList.Add(new GeneralOptionEnabledAccount(account.accountid, account.companyid));
                }
            }

            return accountList;
        }

        /// <summary>
        /// Get accounts with Duty of care approver reminders are enabled 
        /// </summary>
        /// <returns> Account details</returns>
        public static GeneralOptionAccountsResponse GetAccountsWithApproverDutyOfCareDocumentsExpiryEnabled()
        {
            var accountList = new GeneralOptionAccountsResponse();
            var accounts = new cAccounts().GetAllAccounts();
            if (accounts == null || accounts.Count == 0)
            {
                return accountList;
            }

            foreach (var account in accounts)
            {
                var subAccounts = new cAccountSubAccounts(account.accountid);
                var reqSubAccount = subAccounts.getFirstSubAccount();
                if (reqSubAccount.SubAccountProperties.RemindApproverOnDOCDocumentExpiryDays != -1)
                {
                    accountList.AccountList.Add(new GeneralOptionEnabledAccount(account.accountid, account.companyid));
                }
            }

            return accountList;
        }

        /// <summary>
        /// Get accounts with Duty of care claimant reminders are enabled
        /// </summary>
        /// <returns> Account details</returns>
        public static GeneralOptionAccountsResponse GetAccountsWithClaimantDutyOfCareDocumentsExpiryEnabled()
        {
            var accountList = new GeneralOptionAccountsResponse();
            var accounts = new cAccounts().GetAllAccounts();
            if (accounts == null || accounts.Count == 0)
            {
                return accountList;
            }

            foreach (var account in accounts)
            {
                var subAccounts = new cAccountSubAccounts(account.accountid);
                var reqSubAccount = subAccounts.getFirstSubAccount();
                if (reqSubAccount.SubAccountProperties.RemindClaimantOnDOCDocumentExpiryDays != -1)
                {
                    accountList.AccountList.Add(new GeneralOptionEnabledAccount(account.accountid, account.companyid));
                }
            }

            return accountList;
        }


        /// <summary>
        /// Get accounts with Consent expiry is enabled 
        /// </summary>
        /// <returns> Account details</returns>
        public GeneralOptionAccountsResponse GetAccountsWithConsentExpiryEnabled()
        {
            var accountList = new GeneralOptionAccountsResponse();
            var accounts = new cAccounts().GetAllAccounts();
            if (accounts == null || accounts.Count == 0)
            {
                return accountList;
            }

            foreach (var account in accounts)
            {
                var subAccounts = new cAccountSubAccounts(account.accountid);
                var reqSubAccount = subAccounts.getFirstSubAccount();
                if (reqSubAccount.SubAccountProperties.EnableAutomaticDrivingLicenceLookup && account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect))
                {
                    accountList.AccountList.Add(new GeneralOptionEnabledAccount(account.accountid, account.companyid));
                }
            }

            return accountList;
        }

        /// <summary>
        /// The determine expense date using the account property UseDateOfExpenseForDutyOfCareChecks
        /// </summary>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <returns>
        /// The determined expense date <see cref="DateTime"/>.
        /// </returns>
        private DateTime DetermineExpenseDate(DateTime expenseDate)
        {
            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            cAccountProperties subAccountProperties =
                subAccounts.getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties;

            DateTime date = DateTime.Now;

            if (subAccountProperties.UseDateOfExpenseForDutyOfCareChecks)
            {
                date = expenseDate;
            }

            return date;
        }

        /// <summary>
        /// Generates the DVLA consent message, if any.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> dvla consent message, or an empty string if not required.
        /// </returns>
        private string GetDvlaConsentMessage()
        {
            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            cAccountProperties accountProperties = subAccounts.getSubAccountById(this.User.CurrentSubAccountId)
                .SubAccountProperties;

            if (accountProperties.BlockDrivingLicence && accountProperties.EnableAutomaticDrivingLicenceLookup
                && this.User.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect))
            {
                var employees = this.ActionContext.Employees;
                var employee = employees.GetEmployeeById(this.User.EmployeeID);

                var consentExpiryFrequency = accountProperties.FrequencyOfConsentRemindersLookup;
                var frequencyInDays = int.Parse(consentExpiryFrequency);

                // If we don't have a consent response on record, or we do and they've said yes
                if (employee.AgreeToProvideConsent.HasValue == false
                    || (employee.AgreeToProvideConsent.HasValue && employee.AgreeToProvideConsent.Value == true))
                {
                    if (employee.DriverId == null)
                    {
                        return "In order for you to claim mileage, we need to check that you have a valid driving licence with DVLA. You will need to log into the Expenses website and provide consent. This will allow us to perform a driving licence check using the details which you have entered.";
                    }

                    if (employee.AgreeToProvideConsent.HasValue == false && employee.DvlaConsentDate <= DateTime.UtcNow.Date.AddYears(-3).AddDays(frequencyInDays))
                    {
                        return "Your DVLA check consent has now expired. In order to continue claiming mileage, please log into the Expenses website and renew your consent.";
                    }
                }
            }

            return string.Empty;
        }
    }
}