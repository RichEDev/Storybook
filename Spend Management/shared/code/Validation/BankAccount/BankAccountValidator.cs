namespace Spend_Management.shared.code.Validation.BankAccount
{
    using System;
    using SpendManagementLibrary.Account;

    /// <summary>
    /// A class to validate <see cref="BankAccount"/> records
    /// </summary>
    public class BankAccountValidator : IBankAccountValidator
    {
        private readonly IBankAccountValidator _validator;
        private readonly BankAccountValidationResults _bankAccountValidationResults;
        private readonly BankAccounts _bankAccounts;
        private readonly cCountries _countries;

        /// <summary>
        /// Create a new instance of <see cref="BankAccountValidator"/>
        /// </summary>
        /// <param name="validator">An instance of <see cref="IBankAccountValidator"/>to use to validate the <seealso cref="BankAccount"/></param>
        /// <param name="bankAccountValidationResults">An instance of <see cref="BankAccountValidationResults"/>store and read the <see cref="IBankAccountValid"/> data</param>
        /// <param name="bankAccounts">An instance of <see cref="BankAccounts"/></param>
        /// <param name="countries">An instance of <see cref="cCountries"/></param>
        public BankAccountValidator(IBankAccountValidator validator, BankAccountValidationResults bankAccountValidationResults, BankAccounts bankAccounts, cCountries countries)
        {
            this._validator = validator;
            this._bankAccountValidationResults = bankAccountValidationResults;
            this._bankAccounts = bankAccounts;
            this._countries = countries;
        }

        /// <summary>
        /// validate a specific bank account with a validation service
        /// NOTE:- Only available for UK bank accounts.
        /// </summary>
        /// <param name="bankAccount">An instance of <see cref="BankAccount"/>to validate</param>
        /// <returns>An instance of <see cref="IBankAccountValid"/></returns>
        public IBankAccountValid Validate(SpendManagementLibrary.Account.BankAccount bankAccount)
        {
            if (bankAccount == null)
            {
                throw new NullReferenceException("Bank Account Null or empty");
            }

            // Do we need to validate this bank account?
            var country = this._countries.getCountryById(bankAccount.CountryId);
            if (country == null || country.GlobalCountryId != 232)
            {
                // No validation available as not UK.
                return new SpendManagementNoValidation();
            }

            var currentBankAccount = this._bankAccounts.GetBankAccountById(bankAccount.BankAccountId);

            IBankAccountValid currentValidation = null;
            if (currentBankAccount != null)
            {
                currentValidation = this._bankAccountValidationResults.Get(bankAccount);
            }
                
            if (currentBankAccount == null || currentValidation == null || bankAccount.AccountNumber != currentBankAccount.AccountNumber || bankAccount.SortCode != currentBankAccount.SortCode)
            {
                // There is no validation or the account number or sort code have changed.
                return this._validator.Validate(bankAccount);
            }

            return currentValidation;
        }
    }
}
