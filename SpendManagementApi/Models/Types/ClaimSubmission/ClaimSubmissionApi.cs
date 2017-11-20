namespace SpendManagementApi.Models.Types.ClaimSubmission
{
    using System.Collections.Generic;
    using SpendManagementApi.Common.Enum;
    using SpendManagementApi.Models.Requests;

    using SpendManagementLibrary.Claims;
    using SpendManagementLibrary.Mileage;

    /// <summary>
    /// When submitting a claim contains the necessary properties to display the relevant information/questions to the claimant.
    /// </summary>
    public class ClaimSubmissionApi
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ClaimSubmissionApi"/> class.
        /// </summary>
        public ClaimSubmissionApi()
        {
            this.PartSubmittalItems = new List<string>();
            this.OdometerReadings = new List<OdometerReading>(); 
        }

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
        ///  Gets or sets a items to show the part submittal section.
        /// </summary>
        public List<string> PartSubmittalItems { get; set; }

        /// <summary>
        ///  Gets or sets a type of items in part submittal section.
        /// </summary>
        public PartSubmittalFieldType PartSubmittalFieldType { get; set; }

        /// <summary>
        /// Gets or sets list of Approvers for approval section.
        /// </summary>
        public ClaimSubmissionApprover Approvers { get; set; }

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

        /// <summary>
        /// Gets or sets the claim submission result with data whether it can be submitted or not.
        /// </summary>
        public SubmitClaimResult SubmitClaimResult { get; set; }

        /// <summary>
        /// Gets or sets the claim submit rejection reason.
        /// </summary>
        public string ClaimSubmitRejectionReason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cash and credit items are allowed on same claim.
        /// </summary>
        public bool CashCreditItemsAllowedOnSameClaim { get; set; }

        /// <summary>
        /// Gets or sets the claim reference number.
        /// </summary>
        public string ClaimReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the list of envelope information.
        /// </summary>
        public List<ClaimEnvelopeInfo> ClaimEnvelopeInfo { get; set; }
    }
}