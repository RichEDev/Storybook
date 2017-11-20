namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// The reasons that a claim may not be un-submitted..
    /// </summary>
    public enum ClaimUnsubmittableReason
    {
        /// <summary>
        /// The claim can be un submitted.
        /// </summary>
        Unsubmitable = 0,

        /// <summary>
        /// The claim has been unsubmitted
        /// </summary>
        Unsubmitted = 1,

        /// <summary>
        /// This claim is a split approval and another checker has approved an item
        /// </summary>
        SplitApprovalApprovedByAnotherChecker = -1,

        /// <summary>
        /// Items on this claim are approved.
        /// </summary>
        ItemsApproved = -2,

        /// <summary>
        /// Not all items have been returned by all approvers.
        /// </summary>
        ItemsNotReturnedByOtherApprovers = -3,

        /// <summary>
        /// This claim has already been paid using Pay Before Validate
        /// </summary>
        AlreadyPaid = -4,

        /// <summary>
        /// This claim has started the approval process.
        /// </summary>
        StartedApprovalProcess = -5,

        /// <summary>
        /// This claim is in or has passed a SEL stage.  Scan and Attach or Validation.
        /// </summary>
        ClaimHasBeenInvolvedInSelStage = -6,

        /// <summary>
        /// This claim cannot be unsubmitted as it has been escalated to you by someone else.
        /// </summary>
         EscalatedProcess = -7,

    }
}
