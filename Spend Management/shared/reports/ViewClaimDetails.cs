namespace Spend_Management.shared.reports
{
    /// <summary>
    /// A struct used to show claim details when the user clicks the "view claim" link while viewing a report.
    /// </summary>
    public struct ViewClaimDetails
    {
        /// <summary>
        /// Gets or sets the employee name.
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// Gets or sets the claim number.
        /// </summary>
        public string ClaimNumber { get; set; }

        /// <summary>
        /// Gets or sets the date paid.
        /// </summary>
        public string DatePaid { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the grid details.
        /// </summary>
        public string[] Grid { get; set; }
    }
}