using SpendManagementLibrary.Account;

namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Optimization;

    using SpendManagementApi.Common;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary;

    using Spend_Management;
    using Spend_Management.shared.code;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Utilities;

    using Currency = SpendManagementLibrary.Mobile.Currency;

    using Spend_Management.shared.code.Validation.BankAccount.PostCodeAnywhere;
    using SMBankAccountValidation = Spend_Management.shared.code.Validation.BankAccount;

    /// <summary>
    /// The bank account repository.
    /// </summary>
    internal class BankAccountRepository : BaseRepository<BankAccount>, ISupportsActionContext
    {
        private readonly IActionContext _actionContext;
        private BankAccounts _data;

        /// <summary>
        /// Creates a new BankAccountRepository.
        /// </summary>
        /// <param name="user">The Current User.</param>
        /// <param name="actionContext">The action context.</param>
        public BankAccountRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, x => x.BankAccountId, x => x.AccountName)
        {
            this._data = actionContext.BankAccounts;
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
            var bankAccount = this._data.GetBankAccountById(id);
            return new BankAccount().From(bankAccount, this._actionContext);
        }

        public override IList<BankAccount> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a Bank Account
        /// </summary>
        /// <param name="bankAccount">
        /// The <see cref="BankAccount">BankAccount</see>
        /// </param>
        /// <returns>
        /// The <see cref="BankAccount">BankAccount</see>
        /// </returns>
        public override BankAccount Add(BankAccount bankAccount)
        {
            var bankAccounts = new BankAccounts(this.User.AccountID, this.User.EmployeeID);
            var reqBankAcccount = bankAccount.To(this._actionContext);
            var bankAccountValidationResults = new SMBankAccountValidation.BankAccountValidationResults(this.User);
            var countries = new cCountries(this.User.AccountID, this.User.CurrentSubAccountId);
            var validator = new SMBankAccountValidation.BankAccountValidator(new PostCodeAnywhereBankAccountValidator(this.User.Account), bankAccountValidationResults, bankAccounts, countries);
            IBankAccountValid validateResult;

            try
            {
                validateResult =  validator.Validate(reqBankAcccount);
            }
            catch (Exception)
            {
                bankAccount.Outcome = "There was a problem with the Bank Validation service.";
                return bankAccount;
            }
       
            if (validateResult.IsCorrect)
            {        
                var bankAccountId = this._data.SaveBankAccount(reqBankAcccount, this.User.EmployeeID);

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
                    this._data = new BankAccounts(User.AccountID, bankAccount.EmployeeId);
                    return this.Get(bankAccountId);
                }            
            }

            string outcome = ProcessBankAccoutValidation(validateResult);       
            bankAccount.Outcome = outcome;

            return bankAccount;
        }

        /// <summary>
        /// Gets a list of <see cref="BankAccount"/> for the current user.
        /// </summary>
        /// <returns>
        /// A list of  <see cref="BankAccount"/>
        /// </returns>
        public List<BankAccount> GetBankAccountsForCurrentUser()
        {
            List<SpendManagementLibrary.Account.BankAccount> employeeBankAccounts = this._data.GetAccountAsListByEmployeeId(this.User.EmployeeID);
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
        /// Gets the bank account master data for adding/editing a bank account
        /// </summary>
        /// <returns>
        /// The <see cref="BankAccountMasterData"/>.
        /// </returns>
        public BankAccountMasterData GetBankAccountMasterData()
        {

            var employee = this.ActionContext.Employees.GetEmployeeById(this.User.EmployeeID);

            var misc = new cMisc(this.User.AccountID);
            cGlobalProperties properties = misc.GetGlobalProperties(this.User.AccountID);

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
                        ? properties.basecurrency
                        : employee.PrimaryCurrency,
                PrimaryCountryId =
                    employee.PrimaryCountry == 0
                        ? properties.homecountry
                        : employee.PrimaryCountry
            };

            return masterData;
        }

        /// <summary>
        /// Deletes a <see cref="BankAccount">BankAccount</see> by its Id
        /// </summary>
        /// <param name="bankAccountId">The Id of the <see cref="BankAccount">BankAccount</see> to delete</param>
        /// <returns></returns>
        public string DeleteBankAccount(int bankAccountId)
        {
            this.CheckBankAccountOwnership(bankAccountId);
            int outcome = this._data.DeleteBankAccount(bankAccountId, this.User.EmployeeID);

            return this.ProcessDeleteBankAccountOutcome(outcome);
        }

        /// <summary>
        /// Changes the status of a <see cref="BankAccount">BankAccount</see> to either archived, or un-archived
        /// </summary>
        /// <param name="bankAccountId">The Id of the <see cref="BankAccount">BankAccount</see> to update</param>
        /// <param name="archive">The action to perform on the <see cref="BankAccount">BankAccount</see></param>
        /// <returns></returns>
        public string ChangeBankAccountStatus(int bankAccountId, bool archive)
        {
            this.CheckBankAccountOwnership(bankAccountId);
            int outcome = this._data.ChangeStatus(bankAccountId, archive, this.User.EmployeeID);
            return this.ProcessChangeBankAccountStatusOutcome(outcome);
        }

        /// <summary>
        /// Checks the current user is the owner of the supplied <see cref="BankAccount">BankAccount</see> Id
        /// </summary>
        /// <param name="bankAccountId">The <see cref="BankAccount">BankAccount</see> to check</param>
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
        /// Processes the outcome of the delete action, turning is into the appropritate message
        /// </summary>
        /// <param name="outcome"></param>
        /// <returns></returns>
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
        /// Processes the outcome of the change status action, turning is into the appropritate message
        /// </summary>
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
        /// <returns></returns>
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