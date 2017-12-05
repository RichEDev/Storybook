namespace SpendManagementLibrary.Employees
{
    using System;

    /// <summary>
    /// Defines a bank account.
    /// </summary>
    [Serializable]
    public class BankAccount
    {
        public BankAccount(string accountHolderName, string accountNumber, string accountType, string sortCode, string accountReference, int countryId = 0, bool archived = false, string iban = null, string swiftCode = null)
        {
            this.AccountHolderName = accountHolderName;
            this.AccountNumber = accountNumber;
            this.AccountType = accountType;
            this.SortCode = sortCode;
            this.AccountReference = accountReference;
            this.CountryId = countryId;
            this.Iban = iban;
            this.SwiftCode = swiftCode;
        }

        /// <summary>
        /// Gets or sets the account holder name.
        /// </summary>
        public string AccountHolderName { get; set; }

        /// <summary>
        /// Gets or sets the account number.
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the account type.
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// Gets or sets the sort code.
        /// </summary>
        public string SortCode { get; set; }

        /// <summary>
        /// Gets or sets the account reference.
        /// </summary>
        public string AccountReference { get; set; }

        /// <summary>
        /// Gets or sets the bank country id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets Iban of an account number
        /// </summary>
        public string Iban { get; set; }

        /// <summary>
        /// Gets or sets the Swift code of an account number
        /// </summary>
        public string SwiftCode { get; set; }
    }
}
