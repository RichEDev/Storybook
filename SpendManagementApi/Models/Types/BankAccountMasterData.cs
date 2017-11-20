namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;

    /// <summary>
    /// A class to describe the master data used to build up the Add/Edit Bank Accounts
    /// </summary>
    public class BankAccountMasterData
    {
        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="Country"/>
        /// </summary>
        public IList<Country> CountryList { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="Currency"/>
        /// </summary>
        public IList<Currency> CurrencyList { get; set; }

        /// <summary>
        /// Gets or sets the primary currency id.
        /// </summary>
        public int PrimaryCurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the primary country id.
        /// </summary>
        public int PrimaryCountryId { get; set; }

    }
}