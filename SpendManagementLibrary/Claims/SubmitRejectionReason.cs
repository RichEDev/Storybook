using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.Claims
{
    public enum SubmitRejectionReason
    {
        Success,
        NoItems,
        NoSignoffGroup,
        DelegatesProhibited,
        CannotSignoffOwnClaim,
        OutstandingFlags,
        MinimumAmountNotReached,
        MaximumAmountExceeded,
        NoLineManager,
        ApproverOnHoliday,
        FrequencyLimitBreached,
        AssignmentSupervisorNotSpecified,
        CostCodeOwnerNotSpecified,
        CreditCardHasUreconciledItems,
        EmployeeHasUnmatchedCardItems,
        UserNotAllowedToApproveOwnClaimDespiteSignoffGroup,
        UserNotAllowedToApproveOwnClaim,
        InvalidItemChecker,
        ClaimPaid,
        StageRequiresFurtherCheckers,
        ClaimSentToNextStage,
        ApproverCannotApproveOwnClaim,
        ItemsStillToApprove,
        ClaimNameAlreadyExists,
        OutstandingFlagsRequiringJustificationByApprover,
        AssignmentSupervisorNotSpecifiedWhenApproving,
        CostCodeOwnerNotSpecifiedWhenApproving,

        /// <summary>
        /// Claim has already been submitted
        /// </summary>
        AlreadySubmitted,

        /// <summary>
        /// Difference in Odometer Reading is less
        /// than total business mileage
        /// </summary>
        OdometerReadingLessThanBusinessMileage
    }
}
