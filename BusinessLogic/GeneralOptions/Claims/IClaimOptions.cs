namespace BusinessLogic.GeneralOptions.Claims
{
    /// <summary>
    /// Defines a <see cref="IClaimOptions"/> and it's members
    /// </summary>
    public interface IClaimOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether single claim is enabled.
        /// </summary>
        bool SingleClaim { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether attach receipts is enabled.
        /// </summary>
        bool AttachReceipts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether part submit is enabled.
        /// </summary>
        bool PartSubmit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only cash and credit items are enabled.
        /// </summary>
        bool OnlyCashCredit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether limit frequency is enabled.
        /// </summary>
        bool LimitFrequency { get; }

        /// <summary>
        /// Gets or sets the frequency type.
        /// </summary>
        byte FrequencyType { get; set; }

        /// <summary>
        /// Gets or sets the frequency value.
        /// </summary>
        int FrequencyValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether claimant declaration is enabled.
        /// </summary>
        bool ClaimantDeclaration { get; set; }

        /// <summary>
        /// Gets or sets the declaration msg.
        /// </summary>
        string DeclarationMsg { get; set; }

        /// <summary>
        /// Gets or sets the approver declaration msg.
        /// </summary>
        string ApproverDeclarationMsg { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether editing previous claims is enabled.
        /// </summary>
        bool EditPreviousClaims { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the full home address on claims.
        /// </summary>
        bool ShowFullHomeAddressOnClaims { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pre approval is enabled.
        /// </summary>
        bool PreApproval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow team member to approve own claim.
        /// </summary>
        bool AllowTeamMemberToApproveOwnClaim { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow employee in own signoff group.
        /// </summary>
        bool AllowEmployeeInOwnSignoffGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to block unmatched expense items being submitted.
        /// </summary>
        bool BlockUnmatchedExpenseItemsBeingSubmitted { get; set; }
    }
}
