namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementApi.Common;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Account;

    using Spend_Management;
    using Spend_Management.shared.code;
    using Spend_Management.shared.code.Validation.BankAccount.PostCodeAnywhere;

    using BankAccount = SpendManagementApi.Models.Types.BankAccount;
    using SMBankAccountValidation = Spend_Management.shared.code.Validation.BankAccount;

    /// <summary>
    /// The bank account repository.
    /// </summary>
    internal class BankAccountRepository : BaseRepository<BankAccount>, ISupportsActionContext
    {
        /// <summary>
        /// The _action context.
        /// </summary>
        private readonly IActionContext _actionContext;

        private readonly IDataFactory<IGeneralOptions, int> _generalOptionsFactory;

        /// <summary>
        /// Creates a new BankAccountRepository.
        /// </summary>
        /// <param name="user">The Current User.</param>
        /// <param name="actionContext">The action context.</param>
        public BankAccountRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => x.BankAccountId, x => x.AccountName)
        {
            this._actionContext = actionContext;
            this._generalOptionsFactory = WebApiApplication.container.GetInstance<IDataFactory<IGeneralOptions, int>>();
        }

        /// <summary>
        /// Gets a <see cref="BankAccount">BankAccount</see> by its Id.
        /// </summary>
        /// <param name="id">
        /// The bank account Id.
        /// </param>
        /// <returns>
        /// The <see cref="BankAccount">BankAccount</see>.
        /// </returns>
        public override BankAccount Get(int id)
        {
            var bankAccount = this.ActionContext.BankAccountsAdmin.GetById(id);
            return new BankAccount().From(bankAccount, this._actionContext);
        }

        /// <summary>
        /// Gets all the <see cref="BankAccount"/> in the system.
        /// </summary>
        /// <returns>
        /// A list <see cref="BankAccount"/>
        /// </returns>
        public override IList<BankAccount> GetAll()
        {
            var bankAccounts = this._actionContext.BankAccountsAdmin.GetAll();

            var accounts = new List<BankAccount>();

            foreach (var bankAccount in bankAccounts)
            {
                accounts.Add(new BankAccount().From(bankAccount, this._actionContext));
            }

            return accounts;
        }

        /// <summary>
        /// Saves a Bank Account
        /// </summary>
        /// <param name="bankAccount">
        /// The <see cref="BankAccount">BankAccount</see> to add/update.
        /// </param>
        /// <returns>
        /// The <see cref="BankAccount">BankAccount</see>.
        /// </returns>
        public override BankAccount Add(BankAccount bankAccount)
        {
            this.CheckBankAccountAccessRolesPermissions(bankAccount, AccessRoleType.Add);

            var bankAccounts = new BankAccounts(this.User.AccountID, this.User.EmployeeID);
            var reqBankAcccount = bankAccount.To(this._actionContext);
            var bankAccountValidationResults = new SMBankAccountValidation.BankAccountValidationResults(this.User);
            var countries = new cCountries(this.User.AccountID, this.User.CurrentSubAccountId);
            var validator = new SMBankAccountValidation.BankAccountValidator(new PostCodeAnywhereBankAccountValidator(this.User.Account), bankAccountValidationResults, bankAccounts, countries);
            IBankAccountValid validateResult;

            try
            {
                validateResult = validator.Validate(reqBankAcccount);
            }
            catch (Exception)
            {
                bankAccount.Outcome = "There was a problem with the Bank Validation service.";
                return bankAccount;
            }
       
            if (validateResult.IsCorrect)
            {        
                var bankAccountId = this.ActionContext.BankAccountsCurrentUser.SaveBankAccount(reqBankAcccount, this.User.EmployeeID);

                if (bankAccountId == -1)
                {
                    bankAccount.Outcome = "The Account Name already exists.";
                    return bankAccount;
                }
                else
                {                                
                    reqBankAcccount.BankAccountId = bankAccountId;
                    bankAccountValidationResults.Save(reqBankAcccount, validateResult);

                    // re-instantiates to update internal list of bank accounts.
                    this.ActionContext.BankAccountsCurrentUser = new BankAccounts(User.AccountID, bankAccount.EmployeeId);
                    return this.Get(bankAccountId);
                }            
            }

            string outcome = ProcessBankAccoutValidation(validateResult);       
            bankAccount.Outcome = outcome;

            return bankAccount;
        }

        /// <summary>
        /// Updates a <see cref="BankAccount"/>
        /// </summary>
        /// <param name="bankAccount">
        /// The <see cref="BankAccount"/> to update updated.
        /// </param>
        /// <returns>
        /// The <see cref="BankAccount"/> with updated values.
        /// </returns>
        /// <exception cref="ApiException">
        /// Any exceptions during the processing of the <see cref="SpendManagementLibrary.Account.BankAccount"/>.
        /// </exception>
        public override BankAccount Update(BankAccount bankAccount)
        {
            if (bankAccount.BankAccountId < 1)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateUnsuccessfulMessage, ApiResources.ApiErrorUpdateObjectWithWrongIdMessage);
            }

            if (this.Get(bankAccount.BankAccountId) == null)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateUnsuccessfulMessage, ApiResources.APIErrorBankAccountDoesNotExistForId);
            }

            return this.Add(bankAccount);
        }

        /// <summary>
        /// Gets a list of <see cref="BankAccount"/> for the current user.
        /// </summary>
        /// <returns>
        /// A list of <see cref="BankAccount"/>.
        /// </returns>
        public List<BankAccount> GetBankAccountsForCurrentUser()
        {
            List<SpendManagementLibrary.Account.BankAccount> employeeBankAccounts = this.ActionContext.BankAccountsCurrentUser.GetAccountAsListByEmployeeId(this.User.EmployeeID);
            List<BankAccount> bankAccounts = new List<BankAccount>();

            foreach (var account in employeeBankAccounts)
            {
                BankAccount bankAccount = new BankAccount().From(account, this.ActionContext);
                bankAccount.CurrencyName = Helper.GetCurrencyDescriptionFromId(this.ActionContext.Currencies, this.ActionContext.GlobalCurrencies, bankAccount.CurrencyId);
                bankAccounts.Add(bankAccount);
            }

            return bankAccounts;
        }

        /// <summary>
        /// Gets the bank account master data for adding/editing a bank account.
        /// </summary>
        /// <returns>
        /// The <see cref="BankAccountMasterData"/>.
        /// </returns>
        public BankAccountMasterData GetBankAccountMasterData()
        {
            var employee = this.ActionContext.Employees.GetEmployeeById(this.User.EmployeeID);

            var generalOptions =
                this._generalOptionsFactory[this.User.CurrentSubAccountId].WithCurrency().WithCountry();

            var masterData = new BankAccountMasterData
            {
                EmployeeId = this.User.EmployeeID,
                CountryList =
                    new CountryRepository(this.User, this.ActionContext)
                        .GetActiveCountries(),
                CurrencyList =
                    new CurrencyRepository(this.User, this.ActionContext)
                        .GetActiveCurrencies(),
                PrimaryCurrencyId =
                    employee.PrimaryCurrency == 0
                        ? (int)generalOptions.Currency.BaseCurrency
                        : employee.PrimaryCurrency,
                PrimaryCountryId =
                    employee.PrimaryCountry == 0
                        ? generalOptions.Country.HomeCountry
                        : employee.PrimaryCountry
            };

            return masterData;
        }

        /// <summary>
        /// Deletes the current user's <see cref="BankAccount">BankAccount</see> by its Id.
        /// </summary>
        /// <param name="bankAccountId">The Id of the <see cref="BankAccount">BankAccount</see> to delete.</param>
        /// <returns>A string with the outcome of the action</returns>.
        public string DeleteBankAccount(int bankAccountId)
        {
            this.CheckBankAccountOwnership(bankAccountId);
            int outcome = this.ActionContext.BankAccountsCurrentUser.DeleteBankAccount(bankAccountId, this.User.EmployeeID);

            return this.ProcessDeleteBankAccountOutcome(outcome);
        }

        /// <summary>
        /// Deletes the current user's <see cref="BankAccount">BankAccount</see> by its Id.
        /// </summary>
        /// <param name="bankAccountId">The Id of the <see cref="BankAccount">BankAccount</see> to delete.</param>
        /// <returns>A string with the outcome of the action</returns>.
        public string Delete(int bankAccountId)
        {
            int outcome = this.ActionContext.BankAccountsAdmin.DeleteBankAccount(bankAccountId, this.User.EmployeeID);

            return this.ProcessDeleteBankAccountOutcome(outcome);
        }

        /// <summary>
        /// Sets the archive status of a <see cref="BankAccount">BankAccount</see> to either archived, or un-archived for the current user's bank account.
        /// </summary>
        /// <param name="bankAccountId">The Id of the <see cref="BankAccount">BankAccount</see> to update.</param>
        /// <param name="archive">The action to perform on the <see cref="BankAccount">BankAccount.</see></param>
        /// <returns>The outcome of the action</returns>.
        public string ChangeBankAccountStatus(int bankAccountId, bool archive)
        {
            this.CheckBankAccountOwnership(bankAccountId);
            int outcome = this.ActionContext.BankAccountsCurrentUser.ChangeStatus(bankAccountId, archive, this.User.EmployeeID);
            return this.ProcessChangeBankAccountStatusOutcome(outcome);
        }

        /// <summary>
        /// Sets the archive status of a <see cref="BankAccount">BankAccount</see> to either archived, or un-archived as an administrator.
        /// </summary>
        /// <param name="id">The Id of the <see cref="BankAccount">BankAccount</see> to archive.</param>
        /// <param name="archive">The archive action to perform on the <see cref="BankAccount">BankAccount.</see></param>
        /// <returns>The outcome of the action.</returns>
        public string Archive(int id, bool archive)
        {
            var bankAccount = this.Get(id);
            int outcome = this.ActionContext.BankAccountsAdmin.ChangeStatus(id, archive, bankAccount.EmployeeId);
            return this.ProcessChangeBankAccountStatusOutcome(outcome);
        }

        /// <summary>
        /// Checks the current user is the owner of the supplied <see cref="BankAccount">BankAccount</see> Id.
        /// </summary>
        /// <param name="bankAccountId">The <see cref="BankAccount">BankAccount</see> to check.</param>
        private void CheckBankAccountOwnership(int bankAccountId)
        {
            var bankAccount = this.Get(bankAccountId);

            if (bankAccount == null || bankAccount.EmployeeId != this.User.EmployeeID)
            {
                throw new ApiException(
                    ApiResources.ApiErrorInsufficientPermission, ApiResources.ApiErrorNotBankAccountOwner);
            }
        }

        /// <summary>
        /// Checks the access roles for bank accounts.
        /// </summary>
        /// <param name="bankAccount">The <see cref="BankAccount">BankAccount</see> to check.</param>
        /// <param name="accessRoleType">The <see cref="AccessRoleType"/> permission to check.</param>
        private void CheckBankAccountAccessRolesPermissions(BankAccount bankAccount, AccessRoleType accessRoleType)
        {
            if (bankAccount == null || (!this.User.CheckAccessRole(accessRoleType, SpendManagementElement.EmployeeBankAccounts, true) && (!this.User.CheckAccessRole(accessRoleType, SpendManagementElement.BankAccounts, true) || this.User.EmployeeID != bankAccount.EmployeeId)))
            {
                throw new ApiException(ApiResources.ApiErrorInsufficientPermission, ApiResources.HttpStatusCodeForbidden);
            }
        }

        /// <summary>
        /// Processes the outcome of the delete action, turning is into the appropritate message.
        /// </summary>
        /// <param name="outcome">The outcome of the delete bank account result to process.</param>
        /// <returns>The outcome of the action.</returns>
        private string ProcessDeleteBankAccountOutcome(int outcome)
        {
            switch (outcome)
            {
                case -1:
                    return "The bank account cannot be deleted as you have only one active account.";
                case -2:
                    return
                        "The bank account cannot be deleted as it is currently assigned to one or more Expense Items.";
                case -10:
                    return
                        "The bank account cannot be deleted as it currently assigned to a GreenLight or user defined field record.";
                default:
                    return "Success";
            }
        }

        /// <summary>
        /// Processes the outcome of the change status action, turning is into the appropritate message.
        /// </summary>
        /// <param name="outcome">
        /// The outcome of the bank account change status result to process.
        /// </param>
        /// <returns>
        /// <returns>The outcome of the action.</returns>
        /// </returns>
        private string ProcessChangeBankAccountStatusOutcome(int outcome)
        {
            switch (outcome)
            {
                case -1:
                    return "The bank account cannot be archived as you have only one active account.";
                default:
                    return "Success";
            }
        }

        /// <summary>
        /// Processes the result of a bank accounts validation failure, returning why validation failed. 
        /// </summary>
        /// <param name="validateResult">An instance of <see cref="SpendManagementLibrary.Account.IBankAccountValid"/></param>
        /// <returns>The outcome of the action.</returns>
        private static string ProcessBankAccoutValidation(SpendManagementLibrary.Account.IBankAccountValid validateResult)
        {
            string outcome;
        
            switch (validateResult.StatusInformation)
            {
                case "UnknownSortCode":
                    outcome = "The Sort Code entered is unknown.";
                    break;
                case "InvalidAccountNumber":
                    outcome = "The Account Number you have entered is invalid.";
                    break;
                case "DetailsChanged":
                    outcome =
                        $"The Account and Sortcode should be changed for BACS submission. The Sort Code should be '{validateResult.CorrectedSortCode}' and the Account Number should be '{validateResult.CorrectedAccountNumber}'";
                    break;
                default:
                    outcome = validateResult.StatusInformation;
                    break;
            }
       
            return outcome;
        }
    }
}