namespace BusinessLogic.GeneralOptions.AddEditExpense
{
    /// <summary>
    /// Defines a <see cref="IAddEditExpenseOptions"/> and it's members
    /// </summary>
    public interface IAddEditExpenseOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether exchange read only.
        /// </summary>
        bool ExchangeReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether claimants can add company locations.
        /// </summary>
        bool ClaimantsCanAddCompanyLocations { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IncludeEsrDetails"/>
        /// </summary>
        IncludeEsrDetails IncludeAssignmentDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable a car outside of the start end date.
        /// </summary>
        bool DisableCarOutsideOfStartEndDate { get; set; }

        /// <summary>
        /// Gets or sets the home address keyword.
        /// </summary>
        string HomeAddressKeyword { get; set; }

        /// <summary>
        /// Gets or sets the work address keyword.
        /// </summary>
        string WorkAddressKeyword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force address name entry.
        /// </summary>
        bool ForceAddressNameEntry { get; set; }

        /// <summary>
        /// Gets or sets the address name entry message.
        /// </summary>
        string AddressNameEntryMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display ESR addresses in search results.
        /// </summary>
        bool DisplayEsrAddressesInSearchResults { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple work address are enabled.
        /// </summary>
        bool MultipleWorkAddress { get; set; }
    }
}
