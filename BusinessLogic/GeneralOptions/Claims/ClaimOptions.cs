namespace BusinessLogic.GeneralOptions.Claims
{
    /// <summary>
    /// The claim options.
    /// </summary>
    public class ClaimOptions : IClaimOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether single claim is enabled.
        /// </summary>
        public bool SingleClaim { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether attach receipts is enabled.
        /// </summary>
        public bool AttachReceipts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether part submit is enabled.
        /// </summary>
        public bool PartSubmit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only cash and credit items are enabled.
        /// </summary>
        public bool OnlyCashCredit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether limit frequency is enabled.
        /// </summary>
        public bool LimitFrequency => this.FrequencyValue > 0;

        /// <summary>
        /// Gets or sets the frequency type.
        /// </summary>
        public byte FrequencyType { get; set; }

        /// <summary>
        /// Gets or sets the frequency value.
        /// </summary>
        public int FrequencyValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether claimant declaration is enabled.
        /// </summary>
        public bool ClaimantDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the declaration msg.
        /// </summary>
        public string DeclarationMsg { get; set; }

        /// <summary>
        /// Gets or sets the approver declaration msg.
        /// </summary>
        public string ApproverDeclarationMsg { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether editing previous claims is enabled.
        /// </summary>
        public bool EditPreviousClaims { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the full home address on claims.
        /// </summary>
        public bool ShowFullHomeAddressOnClaims { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pre approval is enabled.
        /// </summary>
        public bool PreApproval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow team member to approve own claim.
        /// </summary>
        public bool AllowTeamMemberToApproveOwnClaim { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow employee in own signoff group.
        /// </summary>
        public bool AllowEmployeeInOwnSignoffGroup { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to block unmatched expense items being submitted.
        /// </summary>
        public bool BlockUnmatchedExpenseItemsBeingSubmitted { get; set; }
    }
}
