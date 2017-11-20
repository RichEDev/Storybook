namespace SpendManagementLibrary.Claims
{
    using System.Collections.Generic;

    using SpendManagementLibrary.Mileage;

    /// <summary>
    /// When submitting a claim contains the necessary properties to display the relevant information/questions to the claimant.
    /// </summary>
    public class ClaimSubmissionDetails
    {
        /// <summary>
        /// Gets or sets the claim name.
        /// </summary>
        public string ClaimName { get; set; }

        /// <summary>
        /// Gets or sets the claim description.
        /// </summary>
        public string ClaimDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the claim can be part submitted.
        /// </summary>
        public bool PartSubmittal { get; set; }

        /// <summary>
        /// Gets or sets the the HTML to render the part submittal section.
        /// </summary>
        public string PartSubmittalForm { get; set; }

        /// <summary>
        /// Gets or sets the HTML to render the approval section.
        /// </summary>
        public string Approvers { get; set; }

        /// <summary>
        /// Gets or sets the claimant declaration text.
        /// </summary>
        public string Declaration { get; set; }

        /// <summary>
        /// Gets or sets the message displayed to the claimant if flags exist on the claim.
        /// </summary>
        public string FlagMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the claimant needs to enter odometer readings.
        /// </summary>
        public bool OdometerRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the claim includes fuel card mileage items.
        /// </summary>
        public bool ClaimIncludesFuelCardMileage { get; set; }

        /// <summary>
        /// Gets or sets the odometer readings the claimant needs to complete.
        /// </summary>
        public List<OdometerReading> OdometerReadings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the business mileage section.
        /// </summary>
        public bool ShowBusinessMileage { get; set; }
    }
}
