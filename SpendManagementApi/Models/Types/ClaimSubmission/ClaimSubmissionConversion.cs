namespace SpendManagementApi.Models.Types.ClaimSubmission
{
    using System.Collections.Generic;

    using SpendManagementApi.Common.Enum;
    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Models.Requests;

    using SpendManagementLibrary.Claims;

    using Approver = SpendManagementLibrary.Claims.ClaimSubmissionApprover;
    using ClaimSubmission = SpendManagementLibrary.Claims.ClaimSubmissionApi;

    /// <summary>
    /// ClaimSubmission Conversion.
    /// </summary>
    public static class ClaimSubmissionConversion
    {
        /// <summary>
        /// Convert data access layer Type to an api Type
        /// </summary>
        /// <param name="claimSubmission">
        /// Claim submission database object.
        /// </param>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <returns>
        /// <see cref="ClaimSubmissionApi">ClaimSubmission details</see>.
        /// </returns>
        internal static TResult Cast<TResult>(
            this ClaimSubmission claimSubmission, string claimSubmitRejectionReason, SubmitClaimResult submitClaimResult) where TResult : ClaimSubmissionApi, new()
        {
            if (claimSubmission == null)
            {
                return null;
            }

            var claimSubmissionApprovers = claimSubmission.Approvers;
            var approvers = new ClaimSubmissionApprover();
            if (claimSubmissionApprovers != null)
            {
                approvers.AllApprovers = claimSubmissionApprovers.AllApprovers;
                approvers.RecentApprovers = claimSubmissionApprovers.RecentApprovers;
                approvers.LastApproverId = claimSubmission.Approvers.LastApproverId;
                approvers.LastApprover = claimSubmission.Approvers.LastApprover;
            }

            List<ClaimEnvelopeInfo> claimEnvelopeInfo = null;

            if (claimSubmission.ClaimEnvelopeInfo != null)
            {
                claimEnvelopeInfo = Convert(claimSubmission.ClaimEnvelopeInfo);
            }

            return new TResult()
                       {
                           ClaimDescription = claimSubmission.ClaimDescription,
                           ClaimIncludesFuelCardMileage = claimSubmission.ClaimIncludesFuelCardMileage,
                           ClaimName = claimSubmission.ClaimName,
                           FlagMessage = claimSubmission.FlagMessage,
                           Declaration = claimSubmission.Declaration,
                           OdometerRequired = claimSubmission.OdometerRequired,
                           PartSubmittal = claimSubmission.PartSubmittal,
                           PartSubmittalFieldType = (PartSubmittalFieldType)claimSubmission.PartSubmittalFieldType,
                           PartSubmittalItems = claimSubmission.PartSubmittalItems,
                           ShowBusinessMileage = claimSubmission.ShowBusinessMileage,
                           Approvers = approvers,
                           OdometerReadings = claimSubmission.OdometerReadings,
                           ClaimSubmitRejectionReason = claimSubmitRejectionReason,
                           SubmitClaimResult = submitClaimResult,
                           ClaimReferenceNumber = claimSubmission.ClaimReferenceNumber,
                           ClaimEnvelopeInfo = claimEnvelopeInfo
            };
        }

        internal static List<ClaimEnvelopeInfo> Convert(List<SpendManagementLibrary.ClaimEnvelopeInfo> claimEnvelopeInfo)
        {
            var envelopeInfoList = new List<ClaimEnvelopeInfo>();

            foreach (var envelope in claimEnvelopeInfo)
            {
                var envelopeInfo = new ClaimEnvelopeInfo
                            {
                                ClaimEnvelopeId = envelope.ClaimEnvelopeId,
                                EnvelopeNumber = envelope.EnvelopeNumber,
                                ExcessCharge = envelope.ExcessCharge,
                                PhysicalState = envelope.PhysicalState,
                                ProcessedAfterMarkedLost = envelope.ProcessedAfterMarkedLost
                            };

                envelopeInfoList.Add(envelopeInfo);
            }

            return envelopeInfoList;
        }
    }
}