namespace SpendManagementApi.Common.Enum
{
    /// <summary>
    /// Represents the field type of the claim submit rejection reason.
    /// </summary>
    public enum SubmitRejectionReason
    {
        /// <summary>
        /// Success.
        /// </summary>
        Success,

        /// <summary>
        /// No items.
        /// </summary>
        NoItems,

        /// <summary>
        /// The no signoff group.
        /// </summary>
        NoSignoffGroup,

        /// <summary>
        /// Delegates prohibited.
        /// </summary>
        DelegatesProhibited,

        /// <summary>
        /// Cannot signoff own claim.
        /// </summary>
        CannotSignoffOwnClaim,

        /// <summary>
        /// Outstanding flags.
        /// </summary>
        OutstandingFlags,

        /// <summary>
        /// Minimum amount not reached.
        /// </summary>
        MinimumAmountNotReached,

        /// <summary>
        /// Maximum amount exceeded.
        /// </summary>
        MaximumAmountExceeded,

        /// <summary>
        /// No line manager.
        /// </summary>
        NoLineManager,

        /// <summary>
        /// Approver on holiday.
        /// </summary>
        ApproverOnHoliday,

        /// <summary>
        /// Frequency limit breached.
        /// </summary>
        FrequencyLimitBreached,

        /// <summary>
        /// Assignment supervisor not specified.
        /// </summary>
        AssignmentSupervisorNotSpecified,

        /// <summary>
        /// Cost code owner not specified.
        /// </summary>
        CostCodeOwnerNotSpecified,

        /// <summary>
        /// Credit card has ureconciled items.
        /// </summary>
        CreditCardHasUreconciledItems,

        /// <summary>
        /// Employee has unmatched card items.
        /// </summary>
        EmployeeHasUnmatchedCardItems,

        /// <summary>
        /// User not allowed to approve own claim despite signoff group.
        /// </summary>
        UserNotAllowedToApproveOwnClaimDespiteSignoffGroup,

        /// <summary>
        /// User not allowed to approve own claim.
        /// </summary>
        UserNotAllowedToApproveOwnClaim,

        /// <summary>
        /// Invalid item checker.
        /// </summary>
        InvalidItemChecker,

        /// <summary>
        /// Claim paid.
        /// </summary>
        ClaimPaid,

        /// <summary>
        /// Stage requires further checkers.
        /// </summary>
        StageRequiresFurtherCheckers,

        /// <summary>
        /// Claim sent to next stage.
        /// </summary>
        ClaimSentToNextStage,

        /// <summary>
        /// Approver cannot approve own claim.
        /// </summary>
        ApproverCannotApproveOwnClaim,

        /// <summary>
        /// Items still to approve.
        /// </summary>
        ItemsStillToApprove,

        /// <summary>
        /// Claim name already exists.
        /// </summary>
        ClaimNameAlreadyExists,

        /// <summary>
        /// Outstanding flags requiring justification by approver.
        /// </summary>
        OutstandingFlagsRequiringJustificationByApprover,

        /// <summary>
        /// Assignment supervisor not specified when approving.
        /// </summary>
        AssignmentSupervisorNotSpecifiedWhenApproving,

        /// <summary>
        /// Cost code owner not specified when approving.
        /// </summary>
        CostCodeOwnerNotSpecifiedWhenApproving,

        /// <summary>
        /// Claim has already been submitted
        /// </summary>
        AlreadySubmitted
    }
}