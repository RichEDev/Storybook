namespace SpendManagementApi.Models.Types
{
    using SpendManagementApi.Interfaces;

    using Spend_Management;

    /// <summary>
    /// The bank account type.
    /// </summary>
    public class BankAccount : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Account.BankAccount, BankAccount>
    {
        /// <summary>
        /// Get or sets the bank account ID 
        /// </summary>
        public int BankAccountId { get; set; }

        /// <summary>
        /// Get or sets the employeeID of the employee who owns this bank account
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Get or sets the name given to this account by the user
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Get or sets the unique number given to this account by the user
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Get or sets the account type
        /// </summary>
        public int AccountType { get; set; }

        /// <summary>
        /// Get or sets the sort code
        /// </summary>
        public string SortCode { get; set; }

        /// <summary>
        /// Get or sets the reference value
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Get or sets the currency id
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the currency name.
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// Get or sets country id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// archived
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// The outcome of an action against a bank account i.e. save/edit
        /// </summary>
        public string Outcome { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public BankAccount From(SpendManagementLibrary.Account.BankAccount dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.BankAccountId = dbType.BankAccountId;
            this.EmployeeId = dbType.EmployeeId;
            this.AccountName = dbType.AccountName;
            this.AccountNumber = dbType.AccountNumber;
            this.AccountType = dbType.AccountType;
            this.SortCode = dbType.SortCode;
            this.Reference = dbType.Reference;
            this.CurrencyId = dbType.CurrencyId;
            this.CountryId = dbType.CountryId;
            this.Archived = dbType.Archived;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Account.BankAccount To(IActionContext actionContext)
        {
           return new SpendManagementLibrary.Account.BankAccount(this.BankAccountId, this.EmployeeId, this.AccountName, this.AccountNumber, this.AccountType, this.SortCode, this.Reference, this.CurrencyId, this.CountryId,this.Archived);
        }
    }
}