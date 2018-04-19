namespace BusinessLogic.GeneralOptions.AddEditExpense
{
    /// <summary>
    /// The add edit expense options.
    /// </summary>
    public class AddEditExpenseOptions : IAddEditExpenseOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether exchange read only.
        /// </summary>
        public bool ExchangeReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether claimants can add company locations.
        /// </summary>
        public bool ClaimantsCanAddCompanyLocations { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IncludeEsrDetails"/>
        /// </summary>
        public IncludeEsrDetails IncludeAssignmentDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable a car outside of the start end date.
        /// </summary>
        public bool DisableCarOutsideOfStartEndDate { get; set; }

        /// <summary>
        /// Gets or sets the home address keyword.
        /// </summary>
        public string HomeAddressKeyword { get; set; }

        /// <summary>
        /// Gets or sets the work address keyword.
        /// </summary>
        public string WorkAddressKeyword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force address name entry.
        /// </summary>
        public bool ForceAddressNameEntry { get; set; }

        /// <summary>
        /// Gets or sets the address name entry message.
        /// </summary>
        public string AddressNameEntryMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display ESR addresses in search results.
        /// </summary>
        public bool DisplayEsrAddressesInSearchResults { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple work address are permitted.
        /// </summary>
        public bool MultipleWorkAddress { get; set; }
    }
}
